using static FF1Lib.FF1Rom;

namespace FF1Lib.Sanity
{
	public class SCMap
	{
		public const int Size = 0x40;

		public MapIndex MapIndex { get; private set; }

		public SCMapCheckFlags CFlags { get; private set; }

		public SCTile[,] Tiles { get; private set; }

		public List<SCPointOfInterest> PointsOfInterest { get; private set; } = new List<SCPointOfInterest>();

		public List<SCTeleport> Exits { get; private set; } = new List<SCTeleport>();

		public List<SCEntrance> Entrances { get; private set; }

		public List<SCArea> Areas { get; private set; } = new List<SCArea>();

		FF1Rom rom;
		MapDataGroup map;
		SCTileSet tileSet;

		//EnterTeleData enter;
		//ExitTeleData exit;
		//NormTeleData tele;
		Teleporters teleporters;
		NpcObjectData npcdata;

		public SCMap(MapIndex _MapIndex, MapDataGroup _map, SCMapCheckFlags cflags, FF1Rom _rom, NpcObjectData _npcData, SCTileSet _tileSet, Teleporters _teleporters)
		{
			MapIndex = _MapIndex;
			CFlags = cflags;

			map = _map;
			rom = _rom;
			tileSet = _tileSet;
			teleporters = _teleporters;

			//enter = _enter;
			//exit = _exit;
			//tele = _tele;

			npcdata = _npcData;

			ProcessTiles();
			ProcessNPCs();
			ProcessTeleporters();
			ProcessPointsOfInterest();
			ComposeAreas();
		}

		private void ProcessTiles()
		{
			Tiles = new SCTile[Size, Size];

			for (int x = 0; x < Size; x++)
				for (int y = 0; y < Size; y++)
				{
					//The map is organized the other way around. I don't care about the cache inefficiency here. My sanity comes first.
					var tileId = map.Map[y, x];
					var tileDef = tileSet.Tiles[tileId];

					Tiles[x, y] = new SCTile(tileDef.BitFlags);

					ProcessSpecialTileProps(x, y, tileId, tileDef);
				}
		}

		private void ProcessSpecialTileProps(int x, int y, byte tileId, SCTileDef tileDef)
		{
			var poi = new SCPointOfInterest
			{
				Coords = new SCCoords(x, y),
				TileDef = tileDef,
				TileId = tileId
			};

			if (tileDef.TileProp.TilePropFunc == (TilePropFunc.TP_NOMOVE | TilePropFunc.TP_SPEC_DOOR) && tileDef.TileProp.ShopId > 0)
			{
				poi.Type = SCPointOfInterestType.Shop;
				poi.ShopId = tileDef.TileProp.ShopId;
				PointsOfInterest.Add(poi);
			}
			else if (tileDef.SpBitFlags == SCBitFlags.Warp)
			{
				if ((CFlags & SCMapCheckFlags.NoWarp) > 0) return;

				var t = new SCTeleport { Coords = poi.Coords, Type = SCPointOfInterestType.Warp };
				Exits.Add(t);

				poi.Type = SCPointOfInterestType.Warp;
				poi.Teleport = t;
				PointsOfInterest.Add(poi);
			}
			else if (tileDef.SpBitFlags == SCBitFlags.Teleport)
			{
				var teleDef = new TeleData(teleporters.StandardMapTeleporters[(TeleportIndex)tileDef.TileProp.Byte2]);
				//var teleDef = tele[tileDef.TileProp.Byte2];
				var t = new SCTeleport { Coords = poi.Coords, Type = SCPointOfInterestType.Tele, TargetMap = teleDef.Map, TargetCoords = new SCCoords { X = teleDef.X, Y = teleDef.Y }.SmClamp };
				Exits.Add(t);

				poi.Type = SCPointOfInterestType.Tele;
				poi.Teleport = t;
				PointsOfInterest.Add(poi);
			}
			else if ((tileDef.SpBitFlags & SCBitFlags.Exit) == SCBitFlags.Exit)
			{
				var teleDef = new TeleData(teleporters.ExitTeleporters[(ExitTeleportIndex)tileDef.TileProp.Byte2]);
				//var teleDef = exit[tileDef.TileProp.Byte2];
				var t = new SCTeleport { Coords = poi.Coords, Type = SCPointOfInterestType.Exit, TargetMap = teleDef.Map, TargetCoords = new SCCoords { X = teleDef.X, Y = teleDef.Y } };
				Exits.Add(t);

				poi.Type = SCPointOfInterestType.Exit;
				poi.Teleport = t;
				PointsOfInterest.Add(poi);
			}
			else if (tileDef.SpBitFlags == SCBitFlags.Treasure)
			{
				poi.Type = SCPointOfInterestType.Treasure;
				poi.TreasureId = tileDef.TileProp.Byte2;
				PointsOfInterest.Add(poi);

				Tiles[x, y].Tile |= SCBitFlags.Blocked;
			}
			else if (tileDef.SpBitFlags == SCBitFlags.EarthOrb)
			{
				poi.Type = SCPointOfInterestType.Orb;
				poi.ItemId = Item.EarthOrb;
				PointsOfInterest.Add(poi);
			}
			else if (tileDef.SpBitFlags == SCBitFlags.FireOrb)
			{
				poi.Type = SCPointOfInterestType.Orb;
				poi.ItemId = Item.FireOrb;
				PointsOfInterest.Add(poi);
			}
			else if (tileDef.SpBitFlags == SCBitFlags.WaterOrb)
			{
				poi.Type = SCPointOfInterestType.Orb;
				poi.ItemId = Item.WaterOrb;
				PointsOfInterest.Add(poi);
			}
			else if (tileDef.SpBitFlags == SCBitFlags.AirOrb)
			{
				poi.Type = SCPointOfInterestType.Orb;
				poi.ItemId = Item.AirOrb;
				PointsOfInterest.Add(poi);
			}
		}

		private void ProcessNPCs()
		{
			for (int i = 0; i < 16; i++)
			{
				

				MapObject npc = map.MapObjects[i];

				ProcessNPC(ref npc);
			}
		}

		private void ProcessNPC(ref MapObject npc)
		{
			switch(npc.ObjectId)
			{
				case ObjectId.SubEngineer:
					Tiles[npc.Coords.X, npc.Coords.Y].Tile |= SCBitFlags.Oxyale;
					break;
				case ObjectId.Titan:
					Tiles[npc.Coords.X, npc.Coords.Y].Tile |= SCBitFlags.Ruby;
					break;
				case ObjectId.BlackOrb:
					Tiles[npc.Coords.X, npc.Coords.Y].Tile |= SCBitFlags.Orbs;
					break;
				case ObjectId.LutePlate:
					Tiles[npc.Coords.X, npc.Coords.Y].Tile |= SCBitFlags.Lute;
					CheckUseLuteRodSanity(npc.Coords.X, npc.Coords.Y, SCBitFlags.UseLute, ObjectId.LutePlate);
					break;
				case ObjectId.RodPlate:
					Tiles[npc.Coords.X, npc.Coords.Y].Tile |= SCBitFlags.Rod;
					CheckUseLuteRodSanity(npc.Coords.X, npc.Coords.Y, SCBitFlags.UseRod, ObjectId.RodPlate);
					break;
				default:
					ProcessDefaultNPCs(ref npc);
					break;
			}
		}

		private void ProcessDefaultNPCs(ref MapObject npc)
		{
			var routine = npcdata[npc.ObjectId].Script;

			switch (routine)
			{
				case TalkScripts.Talk_Princess1:
				case TalkScripts.Talk_Bikke:
				case TalkScripts.Talk_Bahamut:
				case TalkScripts.Talk_ElfDocUnne:
				case TalkScripts.Spooky_Unne:
				case TalkScripts.Talk_GiveItemOnFlag:
				case TalkScripts.Talk_TradeItems:
				case TalkScripts.Talk_GiveItemOnItem:
				case TalkScripts.Spooky_GiveOnItem:
				case TalkScripts.Spooky_GiveOnFlag:
				case TalkScripts.Talk_Astos:
				case TalkScripts.Talk_Chaos:
					ProcessQuestNpc(ref npc);
					break;
				case TalkScripts.Talk_Nerrick:
					Tiles[npc.Coords.X, npc.Coords.Y].Tile |= SCBitFlags.Tnt;
					ProcessQuestNpc(ref npc);
					break;
				case TalkScripts.NoOW_Floater:
					Tiles[npc.Coords.X, npc.Coords.Y].Tile |= SCBitFlags.Floater;
					break;
				case TalkScripts.NoOW_Chime:
					Tiles[npc.Coords.X, npc.Coords.Y].Tile |= SCBitFlags.Chime;
					break;
				case TalkScripts.NoOW_Canoe:
					Tiles[npc.Coords.X, npc.Coords.Y].Tile |= SCBitFlags.Canoe;
					break;
				default:
					if (npc.ObjectId == ObjectId.Princess1 || npc.ObjectId == ObjectId.Vampire || npc.ObjectId == ObjectId.ElfDoc || npc.ObjectId == ObjectId.Unne)
					{
						ProcessQuestNpc(ref npc);
					}
					break;
			}
		}

		private void ProcessQuestNpc(ref MapObject npc)
		{
			var routine = npcdata[npc.ObjectId].Script;
			var requirement = npcdata[npc.ObjectId].Requirement;

			var poi = new SCPointOfInterest
			{
				Coords = new SCCoords(npc.Coords.X, npc.Coords.Y),
				Type = SCPointOfInterestType.QuestNpc,
				Npc = npc,
				TalkRoutine = routine,
				//TalkArray = talkArray
				NpcRequirement = requirement
			};

			PointsOfInterest.Add(poi);

			//if (blocked) Tiles[poi.Coords.X, poi.Coords.Y].Tile |= SCBitFlags.Blocked;
		}

		private void CheckUseLuteRodSanity(int x, int y, SCBitFlags check, ObjectId plate)
		{
			if ((CFlags & SCMapCheckFlags.NoUseTiles) > 0) return;

			if (tileSet.Tiles[map.Map[y - 1, x]].SpBitFlags == check) return;
			if (tileSet.Tiles[map.Map[y + 1, x]].SpBitFlags == check) return;
			if (tileSet.Tiles[map.Map[y, x - 1]].SpBitFlags == check) return;
			if (tileSet.Tiles[map.Map[y, x + 1]].SpBitFlags == check) return;

			throw new NopeException(MapIndex, "There is no " + check.ToString() + " tile next to " + plate.ToString());
		}

		private void ProcessTeleporters()
		{
			ProcessEntranceTeleporters();
			ProcessNormalTeleporters();
		}

		private void ProcessEntranceTeleporters()
		{
			foreach (var t in teleporters.OverworldTeleporters)
			{
				if (t.Value.Index == MapIndex)
				{
					PointsOfInterest.Add(new SCPointOfInterest { Coords = new SCCoords { X = t.Value.Coordinates.X, Y = t.Value.Coordinates.Y }.SmClamp, Type = SCPointOfInterestType.OwEntrance });
				}
			}
		}

		private void ProcessNormalTeleporters()
		{
			foreach (var t in teleporters.StandardMapTeleporters)
			{
				if (t.Value.Index == MapIndex)
				{
					PointsOfInterest.Add(new SCPointOfInterest { Coords = new SCCoords { X = t.Value.Coordinates.X, Y = t.Value.Coordinates.Y }.SmClamp, Type = SCPointOfInterestType.SmEntrance });
				}
			}
		}

		private void ProcessPointsOfInterest()
		{
			// 250
			if (PointsOfInterest.Count > 800) throw new NoYouDon_LowerCaseT_Exception(MapIndex, "Excess PointsOfInterest found.");

			var conflicts = PointsOfInterest.Where(p => p.Type < SCPointOfInterestType.OwEntrance).GroupBy(p => p.Coords, new SCCoordsEqualityComparer()).Where(g => g.Count() > 1);
			if (conflicts.Any()) throw new NopeException(MapIndex, "There is a PointOfInterest conflict.");

			PointsOfInterest = PointsOfInterest.Distinct(new SCPointOfInterestEqualityComparer()).ToList();

			Entrances = PointsOfInterest
				.Where(p => p.Type == SCPointOfInterestType.SmEntrance || p.Type == SCPointOfInterestType.OwEntrance)
				.Select(p => p.Coords)
				.Distinct()
				.Select(c => new SCEntrance(this, c)).ToList();
		}


		private void ComposeAreas()
		{
			HashSet<SCCoords> doneEntrances = new HashSet<SCCoords>(new SCCoordsEqualityComparer());

			foreach (var e in Entrances)
			{
				if (!doneEntrances.Contains(e.Coords))
				{
					ComposeArea(e, doneEntrances);
				}
			}
		}

		private void ComposeArea(SCEntrance entrance, HashSet<SCCoords> doneEntrances)
		{
			var poiDic = entrance.PointsOfInterest
				.Where(e => e.Type == SCPointOfInterestType.SmEntrance || e.Type == SCPointOfInterestType.OwEntrance)
				.Where(e => e.BitFlagSet.Count > 0)
				.Select(e => e.Coords)
				.Distinct(new SCCoordsEqualityComparer())
				.ToDictionary(e => e, new SCCoordsEqualityComparer());

			var entranceList = Entrances.Where(e => poiDic.ContainsKey(e.Coords)).ToList();

			foreach (var e in entranceList) if (!doneEntrances.Contains(e.Coords)) doneEntrances.Add(e.Coords);

			var area = new SCArea(this, entranceList);

			Areas.Add(area);
		}
	}
}
