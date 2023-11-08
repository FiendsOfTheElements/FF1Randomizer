namespace FF1Lib.Sanity
{
	public class SCOwMap
	{
		public const int Size = 0x100;

		public SCMapCheckFlags CFlags { get; private set; }

		public SCOwTile[,] Tiles { get; private set; }

		public List<SCPointOfInterest> PointsOfInterest { get; private set; } = new List<SCPointOfInterest>();

		public List<SCTeleport> Exits { get; private set; } = new List<SCTeleport>();

		public List<SCEntrance> Entrances { get; private set; }

		public Dictionary<short, SCOwArea> Areas { get; private set; }

		public SCCoords Bridge { get; private set; }

		public SCCoords Canal { get; private set; }

		FF1Rom rom;
		OverworldMap map;
		SCTileSet tileSet;

		EnterTeleData enter;
		ExitTeleData exit;


		public List<SCOwArea> areadic;
		Queue<SCOwTileQueueEntry> immediatequeue = new Queue<SCOwTileQueueEntry>(256);
		Queue<SCOwTileQueueEntry> deferredqueue = new Queue<SCOwTileQueueEntry>(1024);

		public SCOwMap(OverworldMap _map, SCMapCheckFlags cflags, FF1Rom _rom, SCTileSet _tileSet, EnterTeleData _enter, ExitTeleData _exit, SCCoords bridge, SCCoords canal)
		{
			CFlags = cflags;

			map = _map;
			rom = _rom;
			tileSet = _tileSet;

			enter = _enter;
			exit = _exit;

			Bridge = bridge;
			Canal = canal;

			ProcessTiles();
			ProcessTeleporters();
			ProcessSpecialPointOfInterests();

			DoPathing();

			ProcessPointsOfInterest();

			FilterAreas();
		}

		private void ProcessTiles()
		{
			var compresedMap = map.GetCompressedMapRows();
			var decompressedMap = map.DecompressMapRows(compresedMap);

			Tiles = new SCOwTile[Size, Size];

			for (int x = 0; x < Size; x++)
				for (int y = 0; y < Size; y++)
				{
					var tileId = decompressedMap[y][x];
					var tileDef = tileSet.Tiles[tileId];

					Tiles[x, y] = new SCOwTile(tileDef.OWBitFlags);

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

			if ((tileDef.OWBitFlags & SCBitFlags.Enter) > 0)
			{
				var overworldTeleport = (OverworldTeleportIndex)(tileDef.TileProp.Byte2 & 0x3F);
				var teleDef = enter[(int)overworldTeleport];
				var t = new SCTeleport { Coords = poi.Coords, Type = SCPointOfInterestType.OwEntrance, TargetMap = teleDef.Map, TargetCoords = new SCCoords { X = teleDef.X, Y = teleDef.Y }, OverworldTeleport = overworldTeleport };
				Exits.Add(t);

				poi.Type = SCPointOfInterestType.Tele;
				poi.Teleport = t;
				PointsOfInterest.Add(poi);
			}
			else if ((tileDef.OWBitFlags & SCBitFlags.Caravan) > 0)
			{
				poi.Type = SCPointOfInterestType.Shop;
				poi.ShopId = 70;
				PointsOfInterest.Add(poi);
			}
		}

		private void ProcessTeleporters()
		{
			foreach (var t in exit)
			{
				PointsOfInterest.Add(new SCPointOfInterest { Coords = new SCCoords { X = t.X, Y = t.Y }, Type = SCPointOfInterestType.Exit });
			}
		}

		private void ProcessSpecialPointOfInterests()
		{
			Tiles[Bridge.X, Bridge.Y].Tile = SCBitFlags.Bridge;
			Tiles[Canal.X, Canal.Y].Tile = SCBitFlags.Canal;
		}


		private void DoPathing()
		{
			areadic = new List<SCOwArea>(4096);

			var start = new SCCoords(0, 0);

			var startingarea = new SCOwArea(0, Tiles[start.X, start.Y].Tile);
			startingarea.Start = start;
			areadic.Add(startingarea);
			Tiles[start.X, start.Y].Area = startingarea.Index;

			CheckTile(start.OwLeft, startingarea.Index, startingarea.Tile);
			CheckTile(start.OwRight, startingarea.Index, startingarea.Tile);
			CheckTile(start.OwUp, startingarea.Index, startingarea.Tile);
			CheckTile(start.OwDown, startingarea.Index, startingarea.Tile);

			ProcessImmediates();

			while (deferredqueue.Count > 0)
			{
				ProcessDeferred();
				ProcessImmediates();
			}
		}

		private void ProcessDeferred()
		{
			var entry = deferredqueue.Dequeue();

			var tile = Tiles[entry.Coords.X, entry.Coords.Y];

			if (tile.Area >= 0)
			{
				areadic[tile.Area].AddLink(areadic[entry.Area]);
			}
			else
			{
				var area = new SCOwArea((short)areadic.Count, tile.Tile);
				area.Start = entry.Coords;
				areadic.Add(area);
				Tiles[entry.Coords.X, entry.Coords.Y].Area = area.Index;

				area.AddLink(areadic[entry.Area]);

				CheckTile(entry.Coords.OwLeft, area.Index, tile.Tile);
				CheckTile(entry.Coords.OwRight, area.Index, tile.Tile);
				CheckTile(entry.Coords.OwUp, area.Index, tile.Tile);
				CheckTile(entry.Coords.OwDown, area.Index, tile.Tile);
			}
		}

		private void ProcessImmediates()
		{
			while (immediatequeue.Count > 0)
			{
				var entry = immediatequeue.Dequeue();
				CheckTile(entry.Coords.OwLeft, entry.Area, entry.Tile);
				CheckTile(entry.Coords.OwRight, entry.Area, entry.Tile);
				CheckTile(entry.Coords.OwUp, entry.Area, entry.Tile);
				CheckTile(entry.Coords.OwDown, entry.Area, entry.Tile);
			}
		}

		private void CheckTile(SCCoords coords, short area, SCBitFlags tile)
		{
			if (Tiles[coords.X, coords.Y].Area >= 0) return;

			if (Tiles[coords.X, coords.Y].Tile == tile)
			{
				Tiles[coords.X, coords.Y].Area = area;

				immediatequeue.Enqueue(new SCOwTileQueueEntry { Coords = coords, Area = area, Tile = tile });
			}
			else
			{
				deferredqueue.Enqueue(new SCOwTileQueueEntry { Coords = coords, Area = area });
			}
		}

		private void ProcessPointsOfInterest()
		{
			foreach (var poi in PointsOfInterest)
			{
				var areaIndex = Tiles[poi.Coords.X, poi.Coords.Y].Area;
				areadic[areaIndex].PointsOfInterest.Add(poi);
			}
		}

		private void FilterAreas()
		{
			Areas = areadic.Where(a => (a.Tile & SCBitFlags.Blocked) == 0 && a.Links.Count > 0).ToDictionary(a => a.Index);
		}
	}
}
