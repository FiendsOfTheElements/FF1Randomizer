using System.ComponentModel;

namespace FF1Lib
{
	public enum SpoilerBatHints {
	[Description("Vanilla")]
	Vanilla = 0,
	[Description("Hints")]
	Hints,
	[Description("Stars")]
	Stars,
	[Description("Full Stats")]
	FullStats,
    };

	public class LocationHints
	{
		SanityCheckerV2 checker;
		ShopData shopData;
		NpcObjectData npcData;
		ItemPlacement itemPlacement;
		bool noOverworld;
		public List<LocationHintsInfo> hints { get; }

		public LocationHints(SanityCheckerV2 _checker, ItemPlacement _itemPlacement, ShopData _shopdata, NpcObjectData _npcdata, bool _nooverworld)
		{
			checker = _checker;
			itemPlacement = _itemPlacement;
			shopData = _shopdata;
			npcData = _npcdata;
			hints = new();
			noOverworld = _nooverworld;

			if (noOverworld)
			{
				GenerateNoOW();
			}
			else
			{
				Generate();
			}
		}
		private void Generate()
		{
			foreach (var d in checker.Main.Dungeons)
			{
				var entranceArea = d.Areas.FirstOrDefault(a => a != null && a.IsRoot == true);

				if (entranceArea != null)
				{
					CrawlArea(d.OverworldTeleport, entranceArea, 1, "");
				}
			}
		}
		private void GenerateNoOW()
		{
			var entranceArea = checker.Main.Dungeons.Find(x => x.Areas.FirstOrDefault(a => a != null).Map.MapIndex == MapIndex.ConeriaCastle1F).Areas.FirstOrDefault(a => a != null);

			CrawlAreaNoOW(OverworldTeleportIndex.ConeriaCastle1, entranceArea, 1, "");
		}
		private void ProcessPointOfInterest(OverworldTeleportIndex overworld, Sanity.SCArea a, int depth, string split)
		{
			foreach (var p in a.PointsOfInterest)
			{
				switch (p.Type)
				{
					case Sanity.SCPointOfInterestType.Shop:
						var shop = shopData.Shops.First(x => x.Index == p.ShopId - 1);
						if (shop != null && shop.Entries != null && shop.Entries.Where(x => x <= Item.Oxyale).Any())
						{
							var newshophint = new LocationHintsInfo(overworld, a.Map.MapIndex, p.Type, depth, split, p.ShopId, shop.Entries.Find(x => x <= Item.Oxyale));

							if (!hints.Where(x => x.type == newshophint.type && x.id == newshophint.id).Any())
							{
								hints.Add(newshophint);
							}
						}
						break;
					case Sanity.SCPointOfInterestType.Treasure:
						if (itemPlacement.PlacedItems.TryFind(t => t.Address == 0x3100 + p.TreasureId, out var chest))
						{
							var newchesthint = new LocationHintsInfo(overworld, a.Map.MapIndex, p.Type, depth, split, p.TreasureId, chest.Item);
							if (!hints.Where(x => x.type == newchesthint.type && x.id == newchesthint.id).Any())
							{
								hints.Add(newchesthint);
							}
						}
						break;
					case Sanity.SCPointOfInterestType.QuestNpc:
						var npcitem = (Item)npcData[p.Npc.ObjectId].Item;
						var newnpchint = new LocationHintsInfo(overworld, a.Map.MapIndex, p.Type, depth, split, (int)p.Npc.ObjectId, npcitem);
						if (!hints.Where(x => x.type == newnpchint.type && x.id == newnpchint.id).Any())
						{
							hints.Add(newnpchint);
						}
						break;
				}
			}
		}
		private void CrawlAreaNoOW(OverworldTeleportIndex overworld, Sanity.SCArea a, int depth, string split)
		{
			ProcessPointOfInterest(overworld, a, depth, split);

			var nextarea = a.ChildAreas.Distinct().ToList();

			if (depth < 10)
			{ 
				foreach (var a2 in nextarea)
				{
					CrawlAreaNoOW(overworld, a2, depth + 1, "");
				}
			}
		}
		private void CrawlArea(OverworldTeleportIndex overworld, Sanity.SCArea a, int depth, string split)
		{
			ProcessPointOfInterest(overworld, a, depth, split);

			foreach (var a2 in a.ChildAreas)
			{
				string cardinalstext = split;

				if (a.ChildAreas.Count > 1 && a.Map.MapIndex != MapIndex.CastleOrdeals2F)	
				{
					List<string> cardinals = new();

					var targetExit = a.Exits.Find(e => e.TargetCoords == a2.Entrances[0].Coords);

					if (targetExit != null)
					{
						if (targetExit.Coords.Y < (a.Entrances[0].Coords.Y - 5))
						{
							cardinals.Add("North");
						}
						else if (targetExit.Coords.Y > (a.Entrances[0].Coords.Y + 5))
						{
							cardinals.Add("South");
						}

						if (targetExit.Coords.X < (a.Entrances[0].Coords.X - 5))
						{
							cardinals.Add("West");
						}
						else if (targetExit.Coords.X > (a.Entrances[0].Coords.X + 5))
						{
							cardinals.Add("East");
						}

						cardinalstext = String.Join('-', cardinals) + " path";

						if (split != "")
						{
							cardinalstext = split + ", then " + cardinalstext;
						}
					}
				}

				CrawlArea(overworld, a2, depth + 1, cardinalstext);
			}
		}
	}

	public class LocationHintsInfo
	{
		public OverworldTeleportIndex overworldlocation { get; }
		public MapIndex map { get; }
		public Sanity.SCPointOfInterestType type { get; }
		public int floor { get;  }
		public string splitPosition { get; }
		public int id { get; }
		public Item item { get; }
		public LocationHintsInfo(OverworldTeleportIndex _overworldlocation, MapIndex _map, Sanity.SCPointOfInterestType _type, int _floor, string _split, int _value, Item _item)
		{
			overworldlocation = _overworldlocation;
			map = _map;
			type = _type;
			floor = _floor;
			splitPosition = _split;
			id = _value;
			item = _item;
		}
		public int TreasureAddress()
		{
			return (id + 0x3100);
		}
	}

	public partial class FF1Rom
	{
		public bool RedMageHasLife()
		{
			var magicPermissions = Get(MagicPermissionsOffset, 8 * 12).Chunk(8);
			var magicArray = new List<byte> { 0x80, 0x40, 0x20, 0x10, 0x08, 0x04, 0x02, 0x01 };

			var firstLifeIndex = ItemsText.ToList().FindIndex(x => x.ToLower().Contains("lif")) - MagicNamesIndexInItemText;
			var secondLifeIndex = ItemsText.ToList().FindIndex(firstLifeIndex + MagicNamesIndexInItemText + 1, x => x.ToLower().Contains("lif")) - MagicNamesIndexInItemText;

			var firstlife = firstLifeIndex >= 0 ? (((~magicPermissions[3][firstLifeIndex / 8] & magicArray[firstLifeIndex % 8]) > 0) ? true : false) : false;
			var secondlife = secondLifeIndex >= 0 ? (((~magicPermissions[3][secondLifeIndex / 8] & magicArray[secondLifeIndex % 8]) > 0) ? true : false) : false;

			return (firstlife || secondlife);
		}
		public string LocationText(LocationHints locationhints, LocationHintsInfo location, bool floorshuffle, bool nooverworld)
		{
			Dictionary<OverworldTeleportIndex, string> LocationOverworldNames = new Dictionary<OverworldTeleportIndex, string>
			{
				{OverworldTeleportIndex.Coneria,"Coneria"},
				{OverworldTeleportIndex.Pravoka,"Pravoka"},
				{OverworldTeleportIndex.Elfland,"Elfland"},
				{OverworldTeleportIndex.Melmond,"Melmond"},
				{OverworldTeleportIndex.CrescentLake,"Crescent Lake"},
				{OverworldTeleportIndex.Gaia,"Gaia"},
				{OverworldTeleportIndex.Onrac,"Onrac"},
				{OverworldTeleportIndex.Lefein,"Lefein"},
				{OverworldTeleportIndex.ConeriaCastle1,"Coneria Castle"},
				{OverworldTeleportIndex.ElflandCastle,"the Castle of Elfland"},
				{OverworldTeleportIndex.NorthwestCastle,"Northwest Castle"},
				{OverworldTeleportIndex.CastleOrdeals1,"the Castle of Ordeals"},
				{OverworldTeleportIndex.TempleOfFiends1,"the Temple of Fiends"},
				{OverworldTeleportIndex.EarthCave1,"the Earth Cave"},
				{OverworldTeleportIndex.GurguVolcano1,"Gurgu Volcano"},
				{OverworldTeleportIndex.IceCave1,"the Ice Cave"},
				{OverworldTeleportIndex.Cardia1,"the Northernemost Cave of Cardia"}, // To check
				{OverworldTeleportIndex.Cardia2,"the Western plains of Cardia"},
				{OverworldTeleportIndex.BahamutCave1,"Bahamut's Cave"},
				{OverworldTeleportIndex.Cardia4,"the Marshes of Cardia"},
				{OverworldTeleportIndex.Cardia5,"the Tiny island's Cave of Cardia"}, // To check
				{OverworldTeleportIndex.Cardia6,"the Eastern forest of Cardia"},
				{OverworldTeleportIndex.Waterfall,"the Waterfall"},
				{OverworldTeleportIndex.DwarfCave,"the Dwarves' Cave"},
				{OverworldTeleportIndex.MatoyasCave,"Matoya's Cave"},
				{OverworldTeleportIndex.SardasCave,"Sarda's Cave"},
				{OverworldTeleportIndex.MarshCave1,"the Marsh Cave"},
				{OverworldTeleportIndex.MirageTower1,"the Mirage Tower"},
				{OverworldTeleportIndex.TitansTunnelEast,"the Titan's tunnel"},
				{OverworldTeleportIndex.TitansTunnelWest,"the Titan's tunnel"},
			};

			Dictionary<MapIndex, string> LocationMapNames = new Dictionary<MapIndex, string>
			{
				{MapIndex.ConeriaTown,"in Coneria"},
				{MapIndex.Pravoka,"in Pravoka"},
				{MapIndex.Elfland,"in Elfland"},
				{MapIndex.Melmond,"in Melmond"},
				{MapIndex.CrescentLake,"in Crescent Lake"},
				{MapIndex.Gaia,"in Gaia"},
				{MapIndex.Onrac,"in Onrac"},
				{MapIndex.Lefein,"in Lefein"},
				{MapIndex.ConeriaCastle1F,"on floor 1 of Coneria Castle"},
				{MapIndex.ConeriaCastle2F,"on floor 2 of Coneria Castle"},
				{MapIndex.ElflandCastle,"on the Castle of Efland"},
				{MapIndex.NorthwestCastle,"in Northwest Castle"},
				{MapIndex.CastleOrdeals1F,"on floor 1 of the Castle of Ordeals"},
				{MapIndex.CastleOrdeals2F,"on floor 2 of the Castle of Ordeals"},
				{MapIndex.CastleOrdeals3F,"on floor 3 of the Castle of Ordeals"},
				{MapIndex.TempleOfFiends,"on floor 1 of the Temple of Fiends"},
				{MapIndex.TempleOfFiendsRevisited1F,"on floor 2 of the Temple of Fiends"},
				{MapIndex.TempleOfFiendsRevisited2F,"on floor 3 of the Temple of Fiends"},
				{MapIndex.TempleOfFiendsRevisited3F,"on floor 4 of the Temple of Fiends"},
				{MapIndex.TempleOfFiendsRevisitedEarth,"on floor 5 of the Temple of Fiends"},
				{MapIndex.TempleOfFiendsRevisitedFire,"on floor 6 of the Temple of Fiends"},
				{MapIndex.TempleOfFiendsRevisitedWater,"on floor 7 of the Temple of Fiends"},
				{MapIndex.TempleOfFiendsRevisitedAir,"on floor 8 of the Temple of Fiends"},
				{MapIndex.TempleOfFiendsRevisitedChaos,"on floor 9 of the Temple of Fiends"},
				{MapIndex.EarthCaveB1,"on floor 1 of the Earth Cave"},
				{MapIndex.EarthCaveB2,"on floor 2 of the Earth Cave"},
				{MapIndex.EarthCaveB3,"on floor 3 of the Earth Cave"},
				{MapIndex.EarthCaveB4,"on floor 4 of the Earth Cave"},
				{MapIndex.EarthCaveB5,"on floor 5 of the Earth Cave"},
				{MapIndex.GurguVolcanoB1,"on floor 1 of the Gurgu Volcano"},
				{MapIndex.GurguVolcanoB2,"on floor 2 of the Gurgu Volcano"},
				{MapIndex.GurguVolcanoB3,"on floor 3 of the Gurgu Volcano"},
				{MapIndex.GurguVolcanoB4,"on floor 4 of the Gurgu Volcano"},
				{MapIndex.GurguVolcanoB5,"on floor 5 of the Gurgu Volcano"},
				{MapIndex.IceCaveB1,"on floor 1 of the Ice Cave"},
				{MapIndex.IceCaveB2,"on floor 2 of the Ice Cave"},
				{MapIndex.IceCaveB3,"on floor 3 of the Ice Cave"},
				{MapIndex.Cardia, "in the Cardias"},
				{MapIndex.BahamutCaveB1, "on floor 1 of Bahamut's Cave"},
				{MapIndex.BahamutCaveB2, "on floor 2 of Bahamut's Cave"},
				{MapIndex.SeaShrineB1, "on floor 1 of the Sea Shrine"},
				{MapIndex.SeaShrineB2, "on floor 2 of the Sea Shrine"},
				{MapIndex.SeaShrineB3, "on floor 3 of the Sea Shrine"},
				{MapIndex.SeaShrineB4, "on floor 4 of the Sea Shrine"},
				{MapIndex.SeaShrineB5, "on floor 5 of the Sea Shrine"},
				{MapIndex.Waterfall,"in the Waterfall"},
				{MapIndex.DwarfCave,"in the Dwarves' Cave"},
				{MapIndex.MatoyasCave,"in Matoya's Cave"},
				{MapIndex.SardasCave,"in Sarda's Cave"},
				{MapIndex.MarshCaveB1,"on floor 1 of the Marsh Cave"},
				{MapIndex.MarshCaveB2,"on floor 2 of the Marsh Cave"},
				{MapIndex.MarshCaveB3,"on floor 3 of the Marsh Cave"},
				{MapIndex.MirageTower1F,"on floor 1 of the Mirage Tower"},
				{MapIndex.MirageTower2F,"on floor 2 of the Mirage Tower"},
				{MapIndex.MirageTower3F,"on floor 3 of the Mirage Tower"},
				{MapIndex.SkyPalace1F,"on floor 1 of the Sky Palace"},
				{MapIndex.SkyPalace2F,"on floor 2 of the Sky Palace"},
				{MapIndex.SkyPalace3F,"on floor 3 of the Sky Palace"},
				{MapIndex.SkyPalace4F,"on floor 4 of the Sky Palace"},
				{MapIndex.SkyPalace5F,"on floor 5 of the Sky Palace"},
				{MapIndex.TitansTunnel,"in the Titan's tunnel"}
			};

			bool includefloor = false;

			int locationfloor = location.floor;
			string locationname = LocationOverworldNames[location.overworldlocation];

			if (nooverworld)
			{
				return LocationMapNames[location.map];
			}

			if (locationhints.hints.Where(x => x.overworldlocation == location.overworldlocation).Where(y => y.floor > 1).Any())
			{
				includefloor = true;
			}

			string link = includefloor ? "of" : "in";

			if (location.overworldlocation == OverworldTeleportIndex.Onrac && !floorshuffle)
			{
				if (locationfloor > 1)
				{
					locationfloor--;
					locationname = "the Sea Shrine";
				}
				else
				{
					includefloor = false;
					link = "in";
				}
			}
			else if (location.overworldlocation == OverworldTeleportIndex.MirageTower1 && !floorshuffle)
			{
				if (locationfloor > 3)
				{
					locationfloor -= 3;
					locationname = "the Sky Palace";
				}
			}
			else if (location.overworldlocation == OverworldTeleportIndex.CastleOrdeals1)
			{
				int maxfloor = locationhints.hints.Where(x => x.overworldlocation == location.overworldlocation).Select(x => x.floor).ToList().Max();

				if (locationfloor == maxfloor)
				{
					locationfloor = 3;
				}
				else if (locationfloor > 1)
				{
					locationfloor = 2;
				}
			}
			else if (location.overworldlocation == OverworldTeleportIndex.TitansTunnelEast && floorshuffle)
			{
				locationname = "Titan's tunnel East";
			}
			else if (location.overworldlocation == OverworldTeleportIndex.TitansTunnelWest && floorshuffle)
			{
				locationname = "Titan's tunnel West";
			}
			else if (location.type == Sanity.SCPointOfInterestType.Shop && location.id == 0x46)
			{
				locationname = "the Caravan";
				link = "at";
			}

			string path = "";

			if (location.splitPosition != "")
			{
				path = ", " + location.splitPosition;
			}

			return (includefloor ? "on floor " + locationfloor + path + " " : "") + link + " " + locationname;
		}
		public static Dictionary<ObjectId, string> NpcNames = new Dictionary<ObjectId, string>
		{
			{ObjectId.Astos, "the kindly old King from Northwest Castle"},
			{ObjectId.Bikke, "Bikke the Pirate"},
			{ObjectId.CanoeSage, "the Sage from Crescent Lake"},
			{ObjectId.CubeBot, "a Robot in the Waterfall"},
			{ObjectId.ElfPrince, "the Elf Prince"},
			{ObjectId.Fairy, "a Fairy in a Bottle"},
			{ObjectId.King, "the King of Coneria"},
			{ObjectId.Lefein, "a man in Lefein"},
			{ObjectId.Matoya, "Matoya the Witch"},
			{ObjectId.Nerrick, "Nerrick the Dwarf"},
			{ObjectId.Princess2, "the Princess of Coneria"},
			{ObjectId.Sarda, "Sarda the Sage"},
			{ObjectId.Smith, "the Blacksmith"},
		};
		string FormatText(string st, int width=25, int maxlines=6) {
		// Word wrap text
		string wrapped = "";
		int start = 0;
		int end = 0;
		int lineno = 0;
		while (end < st.Length) {
		    lineno++;
		    if (lineno > maxlines) {
			break;
		    }

		    start = end;
		    while (st[start] == ' ') {
			start++;
		    }

		    end = Math.Min(end+width, st.Length);
		    while (end != st.Length && st[end] != ' ') {
			end--;
		    }

		    if (start > 0) {
			wrapped += "\n";
		    }
		    wrapped += st.Substring(start, end-start);
		}

		wrapped = wrapped.Substring(0, 1).ToUpper() + wrapped.Substring(1);

			return wrapped;
	    }
		public void NPCHints(MT19337 rng, NpcObjectData npcdata, StandardMaps maps, DialogueData dialogues, Flags flags, PlacementContext incentivedata, SanityCheckerV2 sanitychecker, ItemPlacement itemPlacement, ShopData shopdata)
		{
			if (!(bool)flags.HintsVillage || flags.GameMode == GameModes.DeepDungeon || flags.Archipelago )
			{
				return;
			}

			var locationshints = new LocationHints(sanitychecker, itemPlacement, shopdata, npcdata, flags.NoOverworld);

			// Het all game dialogs, get all item names, set dialog templates
			var hintschests = new List<string>() { "The $ is #.", "The $? It's # I believe.", "Did you know that the $ is #?", "My grandpa used to say 'The $ is #'.", "Did you hear? The $ is #!", "Wanna hear a secret? The $ is #!", "I've read somewhere that the $ is #.", "I used to have the $. I lost it #!", "I've hidden the $ #, can you find it?", "Interesting! This book says the $ is #!", "Duh, everyone knows that the $ is #!", "I saw the $ while I was #." };
			var hintsnpc = new List<string>() { "& has the $.", "The $? Did you try asking &?", "The $? & will never part with it!", "& stole the $ from ME! I swear!", "& told me not to reveal they have the $.", "& is hiding something. I bet it's the $!" };
			var hintsvendormed = new List<string>() { "The $ is for sale #.", "I used to have the $. I sold it #!", "There's a fire sale for the $ #.", "I almost bought the $ for sale #." };
			var uselesshints = new List<string>() { "GET A SILK BAG FROM THE\nGRAVEYARD DUCK TO LIVE\nLONGER.", "You spoony bard!", "Press A to talk\nto NPCs!", "A crooked trader is\noffering bum deals in\nthis town.", "The game doesn't start\nuntil you say 'yes'.", "Thieves run away\nreally fast.", "No, I won't move quit\npushing me.", "Dr. Unnes instant\ntranslation services,\njust send one slab\nand 299 GP for\nprocessing.", "I am error.", "Kraken has a good chance\nto one-shot your knight.", "If NPC guillotine is on,\npress reset now or your\nemulator will crash!", "GET EQUIPPED WITH\nTED WOOLSEY.", "8 and palm trees.\nGet it?" };

			if (!RedMageHasLife())
			{
				uselesshints.Add("Red Mages have no life!");
			}

			// Priority order
			var priorityList = new List<Item> { Item.Lute, Item.Key, Item.Rod, Item.Oxyale, Item.Chime, Item.Cube, Item.Floater, Item.Canoe, Item.Ship, Item.Bridge, Item.Canal, Item.Bottle, Item.Slab, Item.Ruby, Item.Crown, Item.Crystal, Item.Herb, Item.Adamant, Item.Tnt, Item.Tail };
			var priorityOrder = priorityList.ToDictionary(x => x, x => priorityList.IndexOf(x));

			var npcSelected = new List<ObjectId>();
			var dialogueID = new List<byte>();

			npcSelected.AddRange(new List<ObjectId> { ObjectId.ConeriaOldMan, ObjectId.PravokaOldMan, ObjectId.ElflandScholar1, ObjectId.MelmondOldMan2, ObjectId.CrescentSage11, ObjectId.OnracOldMan2, ObjectId.GaiaWitch, ObjectId.LefeinMan12 });
			dialogueID.AddRange(new List<byte> { 0x45, 0x53, 0x69, 0x82, 0xA0, 0xAA, 0xCB, 0xDC });
			maps[MapIndex.Lefein].MapObjects.MoveNpc(0x0C, 0x0E, 0x15, false, true);

			var hintedItems = new List<LocationHintsInfo>();
			var incentivizedHintItems = new List<LocationHintsInfo>();
			var looseHintItems = new List<LocationHintsInfo>();

			var hintableItems = incentivedata.IncentiveItems.ToList().Concat(ItemLists.AllQuestItems).Distinct().ToList();

			foreach (var item in hintableItems)
			{
				var itemlocation = locationshints.hints.Where(x => x.item == item);
				if (itemlocation.Any())
				{
					foreach (var loc in itemlocation)
					{ 
						if (loc.type == Sanity.SCPointOfInterestType.Treasure)
						{
							if (incentivedata.IncentiveLocations.Where(x => x.GetType().Equals(typeof(TreasureChest)) && x.Address == loc.TreasureAddress()).Any())
							{
								incentivizedHintItems.Add(loc);
							}
							else if(loc.item <= Item.Oxyale)
							{
								looseHintItems.Add(loc);
							}
						}
						else if (loc.type == Sanity.SCPointOfInterestType.QuestNpc)
						{
							if (incentivedata.IncentiveLocations.Where(x => x.GetType().Equals(typeof(NpcReward)) && ((NpcReward)x).ObjectId == (ObjectId)loc.id).Any())
							{
								incentivizedHintItems.Add(loc);
							}
							else if(loc.item <= Item.Oxyale)
							{
								looseHintItems.Add(loc);
							}
						}
						else if (loc.type == Sanity.SCPointOfInterestType.Shop)
						{
							incentivizedHintItems.Add(loc);
						}
					}
				}
			}

			incentivizedHintItems.Shuffle(rng);
			hintedItems.AddRange(looseHintItems.OrderBy(x => priorityOrder[x.item]));
			hintedItems.AddRange(incentivizedHintItems.ToList());

			hintedItems = hintedItems.DistinctBy(x => x.item).ToList();

			// Declare hints string for each hinted at item
			var hintsList = new List<string>();

			// Create hint for a random item in the pool for each NPC
			bool floorshuffle = (bool)flags.Entrances && (bool)flags.Floors;
			bool nooverworld = flags.NoOverworld;

			var attempts = 0;
			while (++attempts < 50)
			{
				int itemid = 0;

				while(hintsList.Count < npcSelected.Count)
				{
					var tempItem = hintedItems[itemid];
					string tempHint;
					string tempName;

					if (tempItem.item == Item.Ship) tempName = FF1Text.BytesToText(Get(0x2B5D0, 4));
					else if (tempItem.item == Item.Bridge) tempName = FF1Text.BytesToText(Get(0x2B5D0 + 16, 6));
					else if (tempItem.item == Item.Canal) tempName = FF1Text.BytesToText(Get(0x2B5D0 + 24, 5));
					else if (tempItem.item == Item.Canoe) tempName = FF1Text.BytesToText(Get(0x2B5D0 + 36, 5));
					else tempName = ItemsText[(int)tempItem.item].Replace(" ", "");

					if (tempItem.type == Sanity.SCPointOfInterestType.Treasure)
					{
						tempHint = hintschests.PickRandom(rng);
						tempHint = tempHint.Split('$')[0] + tempName + tempHint.Split('$')[1];

						tempHint = FormatText(tempHint.Split('#')[0] + LocationText(locationshints, tempItem, floorshuffle, nooverworld) + tempHint.Split('#')[1]);
						hintsList.Add(tempHint);
						itemid++;
					}
					else if (tempItem.type == Sanity.SCPointOfInterestType.QuestNpc)
					{
						tempHint = hintsnpc.PickRandom(rng);
						tempHint = tempHint.Split('$')[0] + tempName + tempHint.Split('$')[1];
						tempHint = FormatText(tempHint.Split('&')[0] + NpcNames[(ObjectId)tempItem.id] + tempHint.Split('&')[1]);
						hintsList.Add(tempHint);
						itemid++;
					}
					else if (tempItem.type == Sanity.SCPointOfInterestType.Shop)
					{
						tempHint = hintsvendormed.PickRandom(rng);
						tempHint = tempHint.Split('$')[0] + tempName + tempHint.Split('$')[1];
						tempHint = FormatText(tempHint.Split('#')[0] + LocationText(locationshints, tempItem, floorshuffle, nooverworld) + tempHint.Split('#')[1]);
						hintsList.Add(tempHint);
						itemid++;
					}
				}

				hintsList.Reverse();
				hintsList.RemoveRange(0, 1);
				hintsList.Add(uselesshints.PickRandom(rng));

				//var hintsList = hints.ToList();
				hintsList.Shuffle(rng);

				Dictionary<int, string> hintDialogues = new Dictionary<int, string>();

				// Set NPCs new dialogs
				for (int i = 0; i < npcSelected.Count; i++)
				{
					npcdata[npcSelected[i]].Dialogue2 = dialogueID[i];
					npcdata[npcSelected[i]].Script = TalkScripts.Talk_norm;

					hintDialogues.Add(dialogueID[i], hintsList.First());
					hintsList.RemoveRange(0, 1);
				}

				try
				{
					dialogues.InsertDialogues(hintDialogues);
				}
				catch (Exception e)
				{
					if (e == new Exception("Dialogs maximum length exceeded."))
						continue;
				}
				if(attempts > 1) Console.WriteLine($"NPC Hints generated in {attempts} attempts.");
				return;
			}
			throw new Exception("Couldn't generate hints in 50 tries.");
		}

	    int ThreatRating(List<int> ranges, int value) {
		int i = 0;
		for (; i < ranges.Count; i++) {
		    if (value < ranges[i]) {
			return i > 0 ? i-1 : 0;
		    }
		}
		return i > 0 ? i-1 : 0;
	    }

	    string ChooseDescription(List<int> ranges, List<string> descriptions, int value) {
		var tr = ThreatRating(ranges, value);
		if (tr >= descriptions.Count) {
		    tr = descriptions.Count-1;
		    Console.WriteLine($"WARNING value {value} doesn't have a corresponding description, using {descriptions[tr]}");
		}
		return descriptions[tr];
	    }

	    public void SkyWarriorSpoilerBats(MT19337 rng, Flags flags, NpcObjectData npcdata, DialogueData dialogues, EnemyScripts enemyScripts) {
			if (flags.SkyWarriorSpoilerBats == SpoilerBatHints.Vanilla)
			{
				return;
			}
			if ((bool)flags.RemoveBossScripts)
			{
				return;
			}
		List<MagicSpell> spellList = GetSpells();
		var enemies = GetAllEnemyStats();
		// var enemyText = ReadText(EnemyTextPointerOffset, EnemyTextPointerBase, EnemyCount);
		var enemyText = ReadEnemyText();
		var bosses = new[] { new { index = Enemy.Lich2, dialog=0x4D },
				     new { index = Enemy.Kary2, dialog=0x4E },
				     new { index = Enemy.Kraken2, dialog=0x4F },
				     new { index = Enemy.Tiamat2, dialog=0xDB },
				     new { index = Enemy.Chaos, dialog=0x51 },
		};

		var skillNames = ReadText(EnemySkillTextPointerOffset, EnemySkillTextPointerBase, EnemySkillCount);

		var hpRanges = new List<int> {0, 600, 900, 1200, 1500, 2000, 2500, 3000, 3500, 4000};
		var hpDescriptions = new List<string> {
		    "is a speed bump",           // 0-599
		    "is not very tough",     // 600-899
		    "",                   // 900-1199
		    "is pretty tough",       // 1200-1499
		    "is extra tough",        // 1500-1999
		    "is very tough",         // 2000-2499
		    "is super tough",        // 2500-2999
		    "is incredibly tough",   // 3000-3499
		    "is insanely tough",     // 3500-3999
		    "is like a brick wall",  // 4000+
		};
		var dmgRanges = new List<int> {0, 50, 75, 100, 125, 150, 175, 200, 250};
		var dmgDescriptions = new List<string> {
		    "has a pathetic attack",     // 0-49
		    "has a weak attack",     // 50-74
		    "",                      // 75-99
		    "has a pretty powerful attack",   // 100-124
		    "has an extra powerful attack",    // 125-149
		    "has a very powerful attack",     // 150-174
		    "has an incredibly powerful attack",         // 175-199
		    "has a insanely powerful attack",           // 200-249
		    "can destroy you with one finger", // 250+
		};
		var hitCountRanges = new List<int> {0, 6, 7, 10, 15};
		var hitCountDescriptions = new List<string> {
		    "", // 1-5
		    "can hit you half a dozen times", // 6
		    "can hit you an incredible number of times", // 7-9
		    "can hit you an insane number of times", // 10-14
		    "will punch you until you are dead, then punch you some more", // 15+
		};
		var evadeRanges = new List<int> {0, 50, 75, 125, 150, 175, 200, 220, 240};
		var evadeDescriptions = new List<string> {
		    "is a sitting duck",         // 0-49
		    "is easy to hit",            // 50-74
		    "",                          // 75-124,
		    "is somewhat hard to hit",   // 125-149
		    "is very hard to hit",       // 150-174
		    "is super hard to hit",      // 175-199
		    "is incredibly hard to hit", // 200-219
		    "is insanely hard to hit",   // 220-239
		    "is nearly impossible to hit", // 240+
		};
		var defRanges = new List<int> {0, 40, 70, 100, 125, 150, 175, 200, 225};
		var defDescriptions = new List<string> {
		    "has pathetic armor",           // 0-39
		    "has weak armor",               // 40-69
		    "",                             // 70-99
		    "has somewhat thick armor",     // 100-124
		    "has thick armor",        			// 125-149
		    "has very thick armor",         // 150-174
		    "has incredibly thick armor",   // 175-199
		    "has insanely thick armor",     // 200-224
		    "can't be hurt with your puny weapons", // 225+
		};

		var intros = new List<string> {
		    "Legend has it that",
		    "The Sages say that",
		    "The inscriptions say that",
		    "Uncle Caleb says that",
		    "The ancient scrolls say that",
		    "Analyzing the ROM, I discovered that",
		    "Kee.. Kee..",
		    "In Sky Warrior school, I learned that",
		    "Time 4 Lern!",
		    "Listen up!",
		    "Dude,",
		    "I overhead Garland saying that",
		    "Captain, sensors indicate that",
		    "Hear this, Light Warrior!",
		    "Tetron wants you to know that",
		    "Good news, everyone!"
		};



		foreach (var b in bosses) {
		    int hp = (int)BitConverter.ToUInt16(enemies[b.index], EnemyStat.HP);
		    var enemy = enemies[b.index];
		    var scriptIndex = enemy[EnemyStat.Scripts];
		    var spells = enemyScripts[scriptIndex].spell_list.Where(s => s != 0xFF).ToList();
		    var skills = enemyScripts[scriptIndex].skill_list.Where(s => s != 0xFF).ToList();
		    string enemyName;
		    if (b.index == Enemy.Chaos) {
			enemyName = enemyText[b.index].PadRight(8, '_');
		    } else {
			// alt fiends puts the name in the fiend1 spot
			// but does not duplicate it in the fiend2
			// spot.
			enemyName = enemyText[b.index-1].PadRight(8, '_'); ;
		    }

		    var bossStats = new[] {
			new { name="HP", ranges=hpRanges, descriptions=hpDescriptions, bossvalue=hp },
			new { name="Attack", ranges=dmgRanges, descriptions=dmgDescriptions, bossvalue=(int)enemy[EnemyStat.Strength] },
			new { name="Hits", ranges=hitCountRanges, descriptions=hitCountDescriptions, bossvalue=(int)enemy[EnemyStat.Hits] },
			new { name="Evade",ranges=evadeRanges, descriptions=evadeDescriptions, bossvalue=(int)enemy[EnemyStat.Evade] },
			new { name="Defense",ranges=defRanges, descriptions=defDescriptions, bossvalue=(int)enemy[EnemyStat.Defense] },
		    };

		    bool hasEarlyNukeOrNuclear = false;
		    bool hasCUR4 = false;
		    bool hasCRACK = false;
		    for (int i = 0; i < spells.Count; i++) {
			var s = spells[i];
			var spellname = spellList[s].Name;
			if (spellname == "NUKE" && i<4) {
			    hasEarlyNukeOrNuclear = true;
			}
			if (spellname == "CUR4") {
			    hasCUR4 = true;
			}
		    }
		    for (int i = 0; i < skills.Count; i++) {
			var s = skills[i];
			if (skillNames[s] == "NUCLEAR" && i<2) {
			    hasEarlyNukeOrNuclear = true;
			}
			if (skillNames[s] == "CRACK") {
			    hasCRACK = true;
			}
		    }

		    string dialogtext = "";
		    if (flags.SkyWarriorSpoilerBats == SpoilerBatHints.FullStats) {
			string spellscript = "";
			foreach (var s in spells) {
			    var spellname = spellList[s].Name;
			    if (spellscript != "") {
				if (spellscript.Length+spellname.Length > 23 && spellscript.IndexOf("\n") == -1) {
				    spellscript += "\n";
				}
				spellscript += "-";
			    }
			    spellscript += spellname;
			}
			string skillscript = "";
			foreach (var s in skills) {
			    if (skillscript != "") {
				if (skillscript.Length+skillNames[s].Length > 23 && skillscript.IndexOf("\n") == -1) {
				    skillscript += "\n";
				}
				skillscript += "-";
			    }
			    skillscript += skillNames[s];
			}
			dialogtext = $"{enemyName} {hp,4} HP\n"+
			    $"Attack  {enemy[EnemyStat.HitPercent],3}% +{enemy[EnemyStat.Strength],3} x{enemy[EnemyStat.Hits],2}\n"+
			    $"Defense {enemy[EnemyStat.Evade],3     }% -{enemy[EnemyStat.Defense],3}\n"+
			    $"{spellscript}\n"+
			    $"{skillscript}";
		    }
		    else if (flags.SkyWarriorSpoilerBats == SpoilerBatHints.Hints) {

			var chooseIntro = rng.Between(0, intros.Count-1);

			var lines = new List<string>();

			for (int i = 0; i < bossStats.Length; i++) {
			    var bs = bossStats[i];
			    string desc;
			    if (i == 0 && b.index == Enemy.Chaos) {
				// Chaos has a lot more HP than the
				// other fiends adjust it down a bit
				// so the hints make more sense.
				desc = ChooseDescription(bs.ranges, bs.descriptions, bs.bossvalue-1000);
			    }
			    else {
				desc = ChooseDescription(bs.ranges, bs.descriptions, bs.bossvalue);
			    }
			    if (desc != "") {
				lines.Add(desc);
			    }
			}

			if (hasEarlyNukeOrNuclear) {
			    lines.Add("will probably nuke you");
			}
			if (hasCRACK) {
			    lines.Add("can crack you like an egg");
			}
			if (hasCUR4) {
			    lines.Add("can heal");
			}
			if (spells.Count == 0 && skills.Count == 0) {
			    lines.Add("doesn't like to use magic");
			}

			int countlines = 0;
			do {
			    if (countlines > 0) {
				// Hints don't fit in the dialog
				// box, delete a hint at random.
				dialogtext = "";
				lines.RemoveAt(rng.Between(0, lines.Count-1));
			    }

			    if (lines.Count == 0) {
				dialogtext = "pretty boring";
			    } else if (lines.Count == 1) {
				dialogtext = lines[0];
			    } else {
				for (int n = 0; n < lines.Count-1; n++) {
				    dialogtext += $"{lines[n]}, ";
				}
				dialogtext += $"and {lines[lines.Count-1]}";
			    }
			    dialogtext = $"{intros[chooseIntro]} {enemyName} {dialogtext}.";
			    dialogtext = FormatText(dialogtext, 24, 99);
			    countlines = dialogtext.Count(f => f == '\n');
			} while(countlines > 6);

			intros.RemoveAt(chooseIntro); // so each bat will get a different intro.
		    }
		    else if (flags.SkyWarriorSpoilerBats == SpoilerBatHints.Stars) {
			dialogtext = $"Scouting report, {enemyName}";
			for (int i = 0; i < bossStats.Length; i++) {
			    var bs = bossStats[i];
			    int threat;
			    if (i == 0 && b.index == Enemy.Chaos) {
				// Chaos has a lot more HP than the
				// other fiends adjust it down a bit
				// so the hints make more sense.
				threat = ThreatRating(bs.ranges, bs.bossvalue-1000);
			    }
			    else {
				threat = ThreatRating(bs.ranges, bs.bossvalue);
			    }
			    var normalized = (float)(threat+1)/(float)bs.ranges.Count;
			    string stars = new String('+', (int)Math.Max(Math.Round(normalized*10), 1));
			    dialogtext += $"\n{bs.name,7} {stars}";
			}

			dialogtext += $"\nMagic";
			if (hasEarlyNukeOrNuclear) {
			    dialogtext += " NUKE";
			}
			if (hasCRACK) {
			    dialogtext += " CRACK";
			}
			if (hasCUR4) {
			    dialogtext += " CUR4";
			}
		    }

			dialogtext = dialogtext.Replace("_", String.Empty);
			dialogues[b.dialog] = dialogtext;
		}

			if ((bool)flags.SpoilerBatsDontCheckOrbs)
			{
				// Tweak the bat's dialog script
				npcdata[ObjectId.SkyWarrior1].Script = TalkScripts.Talk_norm;
				npcdata[ObjectId.SkyWarrior2].Script = TalkScripts.Talk_norm;
				npcdata[ObjectId.SkyWarrior3].Script = TalkScripts.Talk_norm;
				npcdata[ObjectId.SkyWarrior4].Script = TalkScripts.Talk_norm;
				npcdata[ObjectId.SkyWarrior5].Script = TalkScripts.Talk_norm;
			}
	    }
	}
}
