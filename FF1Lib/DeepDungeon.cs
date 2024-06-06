using System.ComponentModel;
using static FF1Lib.FF1Rom;
using static System.Math;

namespace FF1Lib
{
	public enum TailBahamutMode
	{
		[Description("Random")]
		Random = 0,
		[Description("Tail First")]
		TailFirst,
		[Description("Bahamut First")]
		BahamutFirst,
		[Description("Same Floor")]
		SameFloor
	}
	public enum DeepDungeonGeneratorMode
	{
		[Description("Progressive")]
		Progressive = 0,
		[Description("Chaotic")]
		Chaotic,
	}
	public static class FloorsWeightSelector
	{
		public static Dictionary<MapLocation, int> GetWeights(List<IRewardSource> chestList, Flags flags)
		{
			Dictionary<MapLocation, int> weightedFloors = new();

			if (flags.GameMode == GameModes.DeepDungeon)
			{
				weightedFloors = Enumerable.Range(0, (int)chestList.OrderByDescending(x => x.MapLocation).First().MapLocation + 1).Select(x => ((MapLocation)x, x)).ToDictionary(x => x.Item1, x => x.Item2);
			}
			else
			{
				weightedFloors = new()
				{
					{ MapLocation.StartingLocation, 0 },
					{ MapLocation.AirshipLocation, 0 },
					{ MapLocation.Coneria, 0 },
					{ MapLocation.Pravoka, 11 },
					{ MapLocation.Elfland, 18 },
					{ MapLocation.Melmond, 25 },
					{ MapLocation.CrescentLake, 32 },
					{ MapLocation.Gaia, 39 },
					{ MapLocation.Onrac, 46 },
					{ MapLocation.Lefein, 53 },
					{ MapLocation.ConeriaCastle1, 7 },
					{ MapLocation.ConeriaCastle2, 16 },
					{ MapLocation.ConeriaCastleRoom1, 12 },
					{ MapLocation.ConeriaCastleRoom2, 12 },
					{ MapLocation.ElflandCastle, 13 },
					{ MapLocation.ElflandCastleRoom1, 15 },
					{ MapLocation.NorthwestCastle, 13 },
					{ MapLocation.NorthwestCastleRoom2, 15 },
					{ MapLocation.CastleOrdeals1, 33 },
					{ MapLocation.CastleOrdealsMaze, 33 },
					{ MapLocation.CastleOrdealsTop, 33 },
					{ MapLocation.TempleOfFiends1, 8 },
					{ MapLocation.TempleOfFiends1Room1, 8 },
					{ MapLocation.TempleOfFiends1Room2, 16 },
					{ MapLocation.TempleOfFiends1Room3, 8 },
					{ MapLocation.TempleOfFiends1Room4, 16 },
					{ MapLocation.TempleOfFiends2, 54 },
					{ MapLocation.TempleOfFiends3, 54 },
					{ MapLocation.TempleOfFiendsChaos, 54 },
					{ MapLocation.TempleOfFiendsAir, 54 },
					{ MapLocation.TempleOfFiendsEarth, 54 },
					{ MapLocation.TempleOfFiendsFire, 54 },
					{ MapLocation.TempleOfFiendsWater, 54 },
					{ MapLocation.TempleOfFiendsPhantom, 54 },
					{ MapLocation.EarthCave1, 18 },
					{ MapLocation.EarthCave2, 19 },
					{ MapLocation.EarthCaveVampire, 20 },
					{ MapLocation.EarthCave4, 21 },
					{ MapLocation.EarthCaveLich, 22 },
					{ MapLocation.GurguVolcano1, 25 },
					{ MapLocation.GurguVolcano2, 28 },
					{ MapLocation.GurguVolcano3, 28 },
					{ MapLocation.GurguVolcano4, 28 },
					{ MapLocation.GurguVolcano5, 28 },
					{ MapLocation.GurguVolcano6, 28 },
					{ MapLocation.GurguVolcanoKary, 28 },
					{ MapLocation.IceCave1, 30 },
					{ MapLocation.IceCave2, 30 },
					{ MapLocation.IceCave3, 30 },
					{ MapLocation.IceCave5, 30 },
					{ MapLocation.IceCaveBackExit, 30 },
					{ MapLocation.IceCaveFloater, 30 },
					{ MapLocation.IceCavePitRoom, 30 },
					{ MapLocation.SeaShrine1, 37 },
					{ MapLocation.SeaShrine2, 38 },
					{ MapLocation.SeaShrine2Room2, 38 },
					{ MapLocation.SeaShrine4, 41 },
					{ MapLocation.SeaShrine5, 41 },
					{ MapLocation.SeaShrine6, 41 },
					{ MapLocation.SeaShrine7, 43 },
					{ MapLocation.SeaShrine8, 43 },
					{ MapLocation.SeaShrineKraken, 43 },
					{ MapLocation.SeaShrineMermaids, 40 },
					{ MapLocation.Cardia1, 34 },
					{ MapLocation.Cardia2, 34 },
					{ MapLocation.BahamutCave1, 34 },
					{ MapLocation.BahamutCave2, 34 },
					{ MapLocation.Cardia4, 35 },
					{ MapLocation.Cardia5, 35 },
					{ MapLocation.Cardia6, 35 },
					{ MapLocation.Waterfall, 36 },
					{ MapLocation.DwarfCave, 9 },
					{ MapLocation.DwarfCaveRoom3, 14 },
					{ MapLocation.MatoyasCave, 9 },
					{ MapLocation.SardasCave, 13 },
					{ MapLocation.MarshCave1, 10 },
					{ MapLocation.MarshCave3, 10 },
					{ MapLocation.MarshCaveBottom, 11 },
					{ MapLocation.MarshCaveBottomRoom13, 17 },
					{ MapLocation.MarshCaveBottomRoom14, 17 },
					{ MapLocation.MarshCaveBottomRoom16, 17 },
					{ MapLocation.MarshCaveTop, 10 },
					{ MapLocation.MirageTower1, 44 },
					{ MapLocation.MirageTower2, 46 },
					{ MapLocation.MirageTower3, 46 },
					{ MapLocation.SkyPalace1, 48 },
					{ MapLocation.SkyPalace2, 50 },
					{ MapLocation.SkyPalace3, 53 },
					{ MapLocation.SkyPalaceMaze, 53 },
					{ MapLocation.SkyPalaceTiamat, 53 },
					{ MapLocation.TitansTunnelEast, 20 },
					{ MapLocation.TitansTunnelWest, 20 },
					{ MapLocation.TitansTunnelRoom, 20 },
					{ MapLocation.Caravan, 0 },
				};
			}

			return weightedFloors;
		}
	}

	public class DeepDungeonItemGenerator : IItemGenerator
	{
		private List<IRewardSource> _chestList;
		private List<IRewardSource> _placedItems;
		private Dictionary<MapLocation, int> _floorsWeight;
		private List<Treasure> treasures = new List<Treasure>();
		private List<int> _priceList;
		//private FF1Rom _rom;
		private int _currentChest;
		private List<Item> potionspinner0 = new List<Item>()
			{
				Item.Heal, Item.Pure, Item.Soft
			};
		private List<Item> potionspinner1 = new List<Item>()
			{
				Item.Heal, Item.Heal, Item.Heal, Item.Heal,
				Item.Pure, Item.Pure, Item.Pure,
				Item.Soft
			};
		private List<Item> potionspinner2 = new List<Item>()
			{
				Item.Pure, Item.Pure, Item.Pure, Item.Pure,
				Item.Heal, Item.Heal, Item.Heal,
				Item.Soft
			};
		private List<Item> potionspinner3 = new List<Item>()
			{
				Item.Soft, Item.Soft, Item.Soft, Item.Soft,
				Item.Pure, Item.Pure, Item.Pure,
				Item.Heal
			};

		private struct Treasure
		{
			public int value;
			public byte index;

			public Treasure(int v, byte id)
			{
				value = v;
				index = id;
				if (index == (byte)Item.WhiteShirt || index == (byte)Item.BlackShirt || index == (byte)Item.Ribbon)
				{
					value = 50000;
				}
			}
		}

		public int RollDice(MT19337 rng, int dice, int sides)
		{
			// Roll a number of dice and add up their results.
			int result = 0;
			for (int i = 0; i < dice; i++)
			{
				result += rng.Between(1, sides);
			}
			return result;
		}

		public DeepDungeonItemGenerator(List<IRewardSource> chestList, List<Item> unusedGoldItems, List<Item> removedItems, List<IRewardSource> placedItems, Dictionary<MapLocation, int> weightedFloors, List<int> priceList, bool deepDungeonEnabled, bool etherizerEnabled)
		{
			_placedItems = placedItems;
			_chestList = chestList;
			//_rom = rom;
			_priceList = priceList;
			_currentChest = 0;

			_floorsWeight = weightedFloors;


			// The shelters are placeholders for ethers.
			// It will only include those if the ether flag is checked.
			if (etherizerEnabled)
			{
				potionspinner0.Add(Item.Tent);
				potionspinner0.Add(Item.Cabin);
				potionspinner0.Add(Item.House);
				potionspinner1.Add(Item.Tent);
				potionspinner1.Add(Item.Tent);
				potionspinner2.Add(Item.Cabin);
				potionspinner2.Add(Item.Cabin);
				potionspinner3.Add(Item.House);
				potionspinner3.Add(Item.House);
			}

			// For the non-potion items, we read all the potential treasure contents and sort them
			// by price. The "Treasure" constructor looks at the index to determine if something is
			// an end game item with an absurdly low price and adjust accordingly. That way you can
			// set the price scaling to whatever you want without having to worry about if you made
			// something cost 2 GP and thus end up in late game chests the way the standalone
			// executable version does.
			//var v = _rom.Get(0x37C00, 0x200).ToUShorts().Chunk(2);

			// People have been getting some kind of blank key item in chests on or near the final
			// floor. I suspect there's an off-by-one error somewhere, hopefully this was it?
			//for (int i = 0x1C; i < 0xAF; i++)
			for (int i = 0x1C; i < 0xAE; i++)
			{
				//treasures.Add(new Treasure(v[i][0] + v[i][1] * 0x100, (byte)i));
				treasures.Add(new Treasure(_priceList[i], (byte)i));
			}
			foreach (var golditem in unusedGoldItems)
			{
				treasures.RemoveAll(x => x.index == (int)golditem);
			}
			foreach (var removeitem in removedItems)
			{
				treasures.RemoveAll(x => x.index == (int)removeitem);
			}
			treasures.Sort((x, y) => x.value.CompareTo(y.value));
		}
		public int GetLowestIndex(int floorid)
		{
			int calculatedLowest = Min(Max(0, floorid - 8), 9 - 8) * 10 +
					Min(Max(0, floorid - 9), 12 - 9) * 5 +
					Min(Max(0, floorid - 12), 16 - 12) * 4 +
					Min(Max(0, floorid - 16), 21 - 16) * 3 +
					Max(0, floorid - 21) * 2;

			return calculatedLowest;
		}
		public Item GetItem(MT19337 rng)
		{
			byte spunitem = 0;
			int treasurediesize = 30;
			int lowest = 0;
			if (RollDice(rng, 1, 5) == 1)
			{
				switch ((_floorsWeight[_chestList[_currentChest].MapLocation] - 8) / 13)
				{
				case 0:
					spunitem = (byte)potionspinner1.PickRandom(rng);
					break;
				case 1:
					spunitem = (byte)potionspinner2.PickRandom(rng);
					break;
				default:
					spunitem = (byte)potionspinner3.PickRandom(rng);
					break;
				}
			}
			else
			{
				lowest = Math.Min(GetLowestIndex(_floorsWeight[_chestList[_currentChest].MapLocation]), treasures.Count() - treasurediesize - 1);
				Treasure picked = treasures[RollDice(rng, 1, treasurediesize) + (int)lowest];
				spunitem = picked.index;
			}

			if (_chestList[_currentChest] is NpcReward)
			{
				_placedItems.Add(new NpcReward((NpcReward)_chestList[_currentChest], (Item)spunitem));
			}
			else if (_chestList[_currentChest] is ItemShopSlot)
			{
				_placedItems.Add(new ItemShopSlot((ItemShopSlot)_chestList[_currentChest], (Item)spunitem));
			}
			else
			{
				_placedItems.Add(new TreasureChest(_chestList[_currentChest], (Item)spunitem));
			}


			if (_currentChest < _chestList.Count)
			{
				_currentChest++;
			}

			return (Item)spunitem;
		}
	}


	public class DeepDungeon
	{
		private const byte northdir = 0;
		private const byte southdir = 1;
		private const byte westdir = 2;
		private const byte eastdir = 3;
		private const byte CityWall = 0x7C;
		private List<byte> domainlist = new List<byte>();
		private byte[] tilesetmappings = new byte[61];
		private List<byte> fullteleportdeck = new List<byte>();
		private List<byte> tilesetspinner = new List<byte>();
		private byte[] chestsonfloor = new byte[61];
		private List<Treasure> treasures = new List<Treasure>();
		private List<IRewardSource> chests = new();
		private int bahamutfloor = 0;
		private int tailfloor = 0;

		private FF1Rom _rom;
		private StandardMaps standardMaps;
		private Teleporters teleporters;
		private NpcObjectData npcData;
		private TileSetsData tileSetsData;
		private List<TileSets> mapTileSets;
		private Flags flags;

		private int tntfloor = 8 + 7;
		private int rubyfloor = 22 + 7;
		private int oxyfloor = 36 + 7;
		public int WarMechFloor { get; set; }
		public List<IRewardSource> PlacedItems { get; private set;}
		public List<IRewardSource> ChestLocations { get; private set; }

		private struct Treasure
		{
			public int value;
			public byte index;

			public Treasure(int v, byte id)
			{
				value = v;
				index = id;
				if (index == (byte)Item.WhiteShirt || index == (byte)Item.BlackShirt || index == (byte)Item.Ribbon)
				{
					value = 50000;
				}
			}
		}
		private struct TileGraphic
		{
			public byte topleft;
			public byte topright;
			public byte botleft;
			public byte botright;

			public TileGraphic(byte tl, byte tr, byte bl, byte br)
			{
				topleft = tl;
				topright = tr;
				botleft = bl;
				botright = br;
			}
		}
		private struct Tileset
		{
			public byte abysstile;
			public byte floortile;
			public byte warptile;
			public byte walltile;
			public byte leftwalltile;
			public byte rightwalltile;
			public byte wallupperleft;
			public byte wallupperright;
			public byte roomtile;
			public byte roomupperleft;
			public byte roomupper;
			public byte roomupperright;
			public byte roomlowerleft;
			public byte roomlower;
			public byte roomlowerright;
			public byte roomleft;
			public byte roomright;
			public byte doortile;
			public byte closedoortile;
			public byte obstacletile;
			public TileGraphic warpgraphic;
			public TileGraphic teleportgraphic;
			public TileGraphic laddergraphic;
			public List<byte> teleportdeck;
			public List<byte> treasuredeck;

			public byte[] Solids()
			{
				byte[] result = {
					walltile, leftwalltile, rightwalltile, wallupperleft, wallupperright, abysstile,
					roomright, roomleft, roomupper, roomupperright, roomupperleft, roomlowerright, roomlowerleft
				};
				for (int i = 0; i < treasuredeck.Count(); i++)
				{
					// If this is ever used - Append may not work like this and need fixing
					result.Append(treasuredeck[i]);
				}
				return result;
			}
		}

		private Tileset[] tilesets = new Tileset[8];

		private struct Candidate
		{
			public int x;
			public int y;
			public List<byte> dirs;
			public Candidate(int newx, int newy)
			{
				x = newx;
				y = newy;
				dirs = new List<byte>();
			}
		}
		private class SnakeHead
		{
			public int x;
			public int y;
			public int facing;

			public SnakeHead(int newx, int newy, int newfacing = 0)
			{
				x = newx;
				y = newy;
				facing = (newfacing + 8) % 8;
			}
			public void Rotate(int amount)
			{
				facing = (facing + amount + 8) % 8;
			}
			public void Step()
			{
				switch (facing)
				{
					case 0:
						y--;
						break;
					case 1:
						x++;
						y--;
						break;
					case 2:
						x++;
						break;
					case 3:
						x++;
						y++;
						break;
					case 4:
						y++;
						break;
					case 5:
						x--;
						y++;
						break;
					case 6:
						x--;
						break;
					case 7:
						x--;
						y--;
						break;
				}

			}
		}
		private struct FormationLevel
		{
			public byte index;
			public long level;
			public Encounters.FormationData formation;

		}

		public DeepDungeon(StandardMaps _maps, Teleporters _teleporters, NpcObjectData _npcs, TileSetsData _tilesets, Flags _flags, FF1Rom rom)
		{
			_rom = rom;
			standardMaps = _maps;
			teleporters = _teleporters;
			npcData = _npcs;
			tileSetsData = _tilesets;
			mapTileSets = standardMaps.MapTileSets;
			flags = _flags;
			PlacedItems = new();
		}
		private long MonsterLevel(EnemyInfo monster)
		{
			long result = 0;
			if (monster.exp == 1 && monster.gp == 1)
			{
				result = (int)Math.Pow(32000, 2) * 2;
			}
			else
			{
				result += (int)Math.Pow(monster.exp, 2);
				result += (int)Math.Pow(monster.gp, 2);
			}
			return result;
		}
		public void InitializeTilesets()
		{
			// Castle
			tilesets[1].abysstile = 0x3C;
			tilesets[1].floortile = 0x60;
			tilesets[1].warptile = 0x48;
			tilesets[1].walltile = 0x30;
			tilesets[1].leftwalltile = 0x32;
			tilesets[1].rightwalltile = 0x33;
			tilesets[1].wallupperleft = 0x34;
			tilesets[1].wallupperright = 0x35;
			tilesets[1].roomtile = 0x5E;
			tilesets[1].roomupperleft = 0x00;
			tilesets[1].roomupper = 0x01;
			tilesets[1].roomupperright = 0x02;
			tilesets[1].roomlowerleft = 0x06;
			tilesets[1].roomlower = 0x07;
			tilesets[1].roomlowerright = 0x08;
			tilesets[1].roomleft = 0x03;
			tilesets[1].roomright = 0x05;
			tilesets[1].doortile = 0x36;
			tilesets[1].closedoortile = 0x3A;
			tilesets[1].obstacletile = 0x38;
			tilesets[1].warpgraphic.topleft = 0x26;
			tilesets[1].warpgraphic.topright = 0x27;
			tilesets[1].warpgraphic.botleft = 0x36;
			tilesets[1].warpgraphic.botright = 0x37;
			tilesets[1].teleportgraphic.topleft = 0x28;
			tilesets[1].teleportgraphic.topright = 0x29;
			tilesets[1].teleportgraphic.botleft = 0x38;
			tilesets[1].teleportgraphic.botright = 0x39;
			tilesets[1].laddergraphic.topleft = 0x2E;
			tilesets[1].laddergraphic.topright = 0x2F;
			tilesets[1].laddergraphic.botleft = 0x3E;
			tilesets[1].laddergraphic.botright = 0x3F;
			tilesets[1].teleportdeck = new List<byte>();
			tilesets[1].teleportdeck.Add(0x44);
			tilesets[1].teleportdeck.Add(0x45);
			tilesets[1].teleportdeck.Add(0x49);
			tilesets[1].teleportdeck.Add(0x4C);
			tilesets[1].teleportdeck.Add(0x4D);
			tilesets[1].teleportdeck.Add(0x4E);
			tilesets[1].teleportdeck.Add(0x4F);
			tilesets[1].teleportdeck.Add(0x50);
			tilesets[1].teleportdeck.Add(0x51);
			tilesets[1].teleportdeck.Add(0x52);
			tilesets[1].teleportdeck.Add(0x53);
			tilesets[1].teleportdeck.Add(0x54);
			tilesets[1].teleportdeck.Add(0x55);
			tilesets[1].treasuredeck = new List<byte>();
			for (int i = 0x63; i <= 0x78; i++)
			{
				tilesets[1].treasuredeck.Add((byte)i);
			}

			// Earth/Volcano
			tilesets[2].abysstile = 0x38;
			tilesets[2].floortile = 0x41;
			tilesets[2].warptile = 0x18;
			tilesets[2].walltile = 0x30;
			tilesets[2].leftwalltile = 0x32;
			tilesets[2].rightwalltile = 0x33;
			tilesets[2].wallupperleft = 0x34;
			tilesets[2].wallupperright = 0x35;
			tilesets[2].roomtile = 0x2E;
			tilesets[2].roomupperleft = 0x00;
			tilesets[2].roomupper = 0x01;
			tilesets[2].roomupperright = 0x02;
			tilesets[2].roomlowerleft = 0x06;
			tilesets[2].roomlower = 0x07;
			tilesets[2].roomlowerright = 0x08;
			tilesets[2].roomleft = 0x03;
			tilesets[2].roomright = 0x05;
			tilesets[2].doortile = 0x36;
			tilesets[2].closedoortile = 0x3A;
			tilesets[2].obstacletile = 0x3E;
			tilesets[2].warpgraphic.topleft = 0x26;
			tilesets[2].warpgraphic.topright = 0x27;
			tilesets[2].warpgraphic.botleft = 0x36;
			tilesets[2].warpgraphic.botright = 0x37;
			tilesets[2].teleportgraphic.topleft = 0x28;
			tilesets[2].teleportgraphic.topright = 0x29;
			tilesets[2].teleportgraphic.botleft = 0x38;
			tilesets[2].teleportgraphic.botright = 0x39;
			tilesets[2].laddergraphic.topleft = 0x2E;
			tilesets[2].laddergraphic.topright = 0x2F;
			tilesets[2].laddergraphic.botleft = 0x3E;
			tilesets[2].laddergraphic.botright = 0x3F;
			tilesets[2].teleportdeck = new List<byte>();
			tilesets[2].teleportdeck.Add(0x15);
			tilesets[2].teleportdeck.Add(0x16);
			tilesets[2].teleportdeck.Add(0x19);
			tilesets[2].teleportdeck.Add(0x24);
			tilesets[2].teleportdeck.Add(0x25);
			tilesets[2].teleportdeck.Add(0x26);
			tilesets[2].teleportdeck.Add(0x27);
			tilesets[2].teleportdeck.Add(0x28);
			tilesets[2].teleportdeck.Add(0x29);
			tilesets[2].teleportdeck.Add(0x2A);
			tilesets[2].teleportdeck.Add(0x2B);
			tilesets[2].teleportdeck.Add(0x2C);
			tilesets[2].teleportdeck.Add(0x09 + 0x80);
			tilesets[2].teleportdeck.Add(0x0F + 0x80);
			tilesets[2].teleportdeck.Add(0x1F + 0x80);
			tilesets[2].treasuredeck = new List<byte>();
			for (int i = 0x42; i <= 0x7E; i++)
			{
				tilesets[2].treasuredeck.Add((byte)i);
			}

			// Ice Cave
			tilesets[3].abysstile = 0x3C;
			tilesets[3].floortile = 0x49;
			tilesets[3].warptile = 0x18;
			tilesets[3].walltile = 0x30;
			tilesets[3].leftwalltile = 0x32;
			tilesets[3].rightwalltile = 0x33;
			tilesets[3].wallupperleft = 0x34;
			tilesets[3].wallupperright = 0x35;
			tilesets[3].roomtile = 0x46;
			tilesets[3].roomupperleft = 0x00;
			tilesets[3].roomupper = 0x01;
			tilesets[3].roomupperright = 0x02;
			tilesets[3].roomlowerleft = 0x06;
			tilesets[3].roomlower = 0x07;
			tilesets[3].roomlowerright = 0x08;
			tilesets[3].roomleft = 0x03;
			tilesets[3].roomright = 0x05;
			tilesets[3].doortile = 0x36;
			tilesets[3].closedoortile = 0x3A;
			tilesets[3].obstacletile = 0x3D;
			tilesets[3].warpgraphic.topleft = 0x26;
			tilesets[3].warpgraphic.topright = 0x27;
			tilesets[3].warpgraphic.botleft = 0x36;
			tilesets[3].warpgraphic.botright = 0x37;
			tilesets[3].teleportgraphic.topleft = 0x28;
			tilesets[3].teleportgraphic.topright = 0x29;
			tilesets[3].teleportgraphic.botleft = 0x38;
			tilesets[3].teleportgraphic.botright = 0x39;
			tilesets[3].laddergraphic.topleft = 0x2E;
			tilesets[3].laddergraphic.topright = 0x2F;
			tilesets[3].laddergraphic.botleft = 0x3E;
			tilesets[3].laddergraphic.botright = 0x3F;
			tilesets[3].teleportdeck = new List<byte>();
			tilesets[3].teleportdeck.Add(0x19);
			tilesets[3].teleportdeck.Add(0x1A);
			tilesets[3].teleportdeck.Add(0x1B);
			tilesets[3].teleportdeck.Add(0x29);
			tilesets[3].teleportdeck.Add(0x2A);
			tilesets[3].teleportdeck.Add(0x2B);
			tilesets[3].teleportdeck.Add(0x2C);
			tilesets[3].teleportdeck.Add(0x2D + 0x80);
			tilesets[3].teleportdeck.Add(0x2E + 0x80);
			tilesets[3].treasuredeck = new List<byte>();
			for (int i = 0x4B; i <= 0x7E; i++)
			{
				tilesets[3].treasuredeck.Add((byte)i);
			}

			// Marsh/Tower
			tilesets[4].abysstile = 0x3F;
			tilesets[4].floortile = 0x40;
			tilesets[4].warptile = 0x23;
			tilesets[4].walltile = 0x30;
			tilesets[4].leftwalltile = 0x32;
			tilesets[4].rightwalltile = 0x33;
			tilesets[4].wallupperleft = 0x34;
			tilesets[4].wallupperright = 0x35;
			tilesets[4].roomtile = 0x2D;
			tilesets[4].roomupperleft = 0x00;
			tilesets[4].roomupper = 0x01;
			tilesets[4].roomupperright = 0x02;
			tilesets[4].roomlowerleft = 0x06;
			tilesets[4].roomlower = 0x07;
			tilesets[4].roomlowerright = 0x08;
			tilesets[4].roomleft = 0x03;
			tilesets[4].roomright = 0x05;
			tilesets[4].doortile = 0x36;
			tilesets[4].closedoortile = 0x3A;
			tilesets[4].obstacletile = 0x39;
			tilesets[4].warpgraphic.topleft = 0x26;
			tilesets[4].warpgraphic.topright = 0x27;
			tilesets[4].warpgraphic.botleft = 0x36;
			tilesets[4].warpgraphic.botright = 0x37;
			tilesets[4].teleportgraphic.topleft = 0x28;
			tilesets[4].teleportgraphic.topright = 0x29;
			tilesets[4].teleportgraphic.botleft = 0x38;
			tilesets[4].teleportgraphic.botright = 0x39;
			tilesets[4].laddergraphic.topleft = 0x2E;
			tilesets[4].laddergraphic.topright = 0x2F;
			tilesets[4].laddergraphic.botleft = 0x3E;
			tilesets[4].laddergraphic.botright = 0x3F;
			tilesets[4].teleportdeck = new List<byte>();
			tilesets[4].teleportdeck.Add(0x20);
			tilesets[4].teleportdeck.Add(0x22);
			tilesets[4].teleportdeck.Add(0x27);
			tilesets[4].teleportdeck.Add(0x28);
			tilesets[4].teleportdeck.Add(0x09 + 0x80);
			tilesets[4].treasuredeck = new List<byte>();
			for (int i = 0x41; i <= 0x67; i++)
			{
				tilesets[4].treasuredeck.Add((byte)i);
			}

			// Shrine/Temple
			tilesets[5].abysstile = 0x39;
			tilesets[5].floortile = 0x55;
			tilesets[5].warptile = 0x42;
			tilesets[5].walltile = 0x30;
			tilesets[5].leftwalltile = 0x32;
			tilesets[5].rightwalltile = 0x33;
			tilesets[5].wallupperleft = 0x34;
			tilesets[5].wallupperright = 0x35;
			tilesets[5].roomtile = 0x53;
			tilesets[5].roomupperleft = 0x00;
			tilesets[5].roomupper = 0x01;
			tilesets[5].roomupperright = 0x02;
			tilesets[5].roomlowerleft = 0x06;
			tilesets[5].roomlower = 0x07;
			tilesets[5].roomlowerright = 0x08;
			tilesets[5].roomleft = 0x03;
			tilesets[5].roomright = 0x05;
			tilesets[5].doortile = 0x36;
			tilesets[5].closedoortile = 0x3A;
			tilesets[5].obstacletile = 0x38;
			tilesets[5].warpgraphic.topleft = 0x26;
			tilesets[5].warpgraphic.topright = 0x27;
			tilesets[5].warpgraphic.botleft = 0x36;
			tilesets[5].warpgraphic.botright = 0x37;
			tilesets[5].teleportgraphic.topleft = 0x28;
			tilesets[5].teleportgraphic.topright = 0x29;
			tilesets[5].teleportgraphic.botleft = 0x38;
			tilesets[5].teleportgraphic.botright = 0x39;
			tilesets[5].laddergraphic.topleft = 0x2E;
			tilesets[5].laddergraphic.topright = 0x2F;
			tilesets[5].laddergraphic.botleft = 0x3E;
			tilesets[5].laddergraphic.botright = 0x3F;
			tilesets[5].teleportdeck = new List<byte>();
			tilesets[5].teleportdeck.Add(0x43);
			tilesets[5].teleportdeck.Add(0x44);
			tilesets[5].teleportdeck.Add(0x45);
			tilesets[5].teleportdeck.Add(0x46);
			tilesets[5].teleportdeck.Add(0x4B);
			tilesets[5].teleportdeck.Add(0x4C);
			tilesets[5].teleportdeck.Add(0x4E);
			tilesets[5].teleportdeck.Add(0x4F);
			tilesets[5].teleportdeck.Add(0x0F + 0x80);
			tilesets[5].teleportdeck.Add(0x40 + 0x80);
			tilesets[5].treasuredeck = new List<byte>();
			for (int i = 0x56; i <= 0x7B; i++)
			{
				tilesets[5].treasuredeck.Add((byte)i);
			}

			// Sky
			tilesets[6].abysstile = 0x39;
			tilesets[6].floortile = 0x4B;
			tilesets[6].warptile = 0x45;
			tilesets[6].walltile = 0x30;
			tilesets[6].leftwalltile = 0x32;
			tilesets[6].rightwalltile = 0x33;
			tilesets[6].wallupperleft = 0x34;
			tilesets[6].wallupperright = 0x35;
			tilesets[6].roomtile = 0x49;
			tilesets[6].roomupperleft = 0x00;
			tilesets[6].roomupper = 0x01;
			tilesets[6].roomupperright = 0x02;
			tilesets[6].roomlowerleft = 0x06;
			tilesets[6].roomlower = 0x07;
			tilesets[6].roomlowerright = 0x08;
			tilesets[6].roomleft = 0x03;
			tilesets[6].roomright = 0x05;
			tilesets[6].doortile = 0x36;
			tilesets[6].closedoortile = 0x3A;
			tilesets[6].obstacletile = 0x39;
			tilesets[6].warpgraphic.topleft = 0x26;
			tilesets[6].warpgraphic.topright = 0x27;
			tilesets[6].warpgraphic.botleft = 0x36;
			tilesets[6].warpgraphic.botright = 0x37;
			tilesets[6].teleportgraphic.topleft = 0x62;
			tilesets[6].teleportgraphic.topright = 0x63;
			tilesets[6].teleportgraphic.botleft = 0x72;
			tilesets[6].teleportgraphic.botright = 0x73;
			tilesets[6].laddergraphic.topleft = 0x64;
			tilesets[6].laddergraphic.topright = 0x65;
			tilesets[6].laddergraphic.botleft = 0x74;
			tilesets[6].laddergraphic.botright = 0x75;
			tilesets[6].teleportdeck = new List<byte>();
			tilesets[6].teleportdeck.Add(0x41);
			tilesets[6].teleportdeck.Add(0x42);
			tilesets[6].teleportdeck.Add(0x43);
			tilesets[6].teleportdeck.Add(0x44);
			tilesets[6].teleportdeck.Add(0x40 + 0x80);
			tilesets[6].treasuredeck = new List<byte>();
			for (int i = 0x4C; i <= 0x6D; i++)
			{
				tilesets[6].treasuredeck.Add((byte)i);
			}

			// Final dungeon
			tilesets[7].abysstile = 0x3F;
			tilesets[7].floortile = 0x5C;
			tilesets[7].warptile = 0x4F;
			tilesets[7].walltile = 0x30;
			tilesets[7].leftwalltile = 0x32;
			tilesets[7].rightwalltile = 0x33;
			tilesets[7].wallupperleft = 0x34;
			tilesets[7].wallupperright = 0x35;
			tilesets[7].roomtile = 0x55;
			tilesets[7].roomupperleft = 0x00;
			tilesets[7].roomupper = 0x01;
			tilesets[7].roomupperright = 0x02;
			tilesets[7].roomlowerleft = 0x06;
			tilesets[7].roomlower = 0x07;
			tilesets[7].roomlowerright = 0x08;
			tilesets[7].roomleft = 0x03;
			tilesets[7].roomright = 0x05;
			tilesets[7].doortile = 0x36;
			tilesets[7].closedoortile = 0x3A;
			tilesets[7].obstacletile = 0x38;
			tilesets[7].warpgraphic.topleft = 0x26;
			tilesets[7].warpgraphic.topright = 0x27;
			tilesets[7].warpgraphic.botleft = 0x36;
			tilesets[7].warpgraphic.botright = 0x37;
			tilesets[7].teleportgraphic.topleft = 0x28;
			tilesets[7].teleportgraphic.topright = 0x29;
			tilesets[7].teleportgraphic.botleft = 0x38;
			tilesets[7].teleportgraphic.botright = 0x39;
			tilesets[7].laddergraphic.topleft = 0x2E;
			tilesets[7].laddergraphic.topright = 0x2F;
			tilesets[7].laddergraphic.botleft = 0x3E;
			tilesets[7].laddergraphic.botright = 0x3F;
			tilesets[7].teleportdeck = new List<byte>();
			tilesets[7].teleportdeck.Add(0x42);
			tilesets[7].teleportdeck.Add(0x43);
			tilesets[7].teleportdeck.Add(0x4B);
			tilesets[7].teleportdeck.Add(0x4C);
			tilesets[7].teleportdeck.Add(0x4D);
			tilesets[7].teleportdeck.Add(0x4E);
			tilesets[7].teleportdeck.Add(0x50);
			tilesets[7].teleportdeck.Add(0x51);
			tilesets[7].teleportdeck.Add(0x52);
			tilesets[7].teleportdeck.Add(0x53);
			tilesets[7].teleportdeck.Add(0x09 + 0x80);
			tilesets[7].teleportdeck.Add(0x40 + 0x80);
			tilesets[7].treasuredeck = new List<byte>();
			for (int i = 0x5D; i <= 0x64; i++)
			{
				tilesets[7].treasuredeck.Add((byte)i);
			}

			// Read which maps have which tilesets
			tilesetmappings = mapTileSets.Select(x => (byte)x).ToArray();

			// Correct the tile graphics so that "back" stairs look like they go up and
			// "forward" stairs look like they go down.
			for (int i = 1; i < 8; i++)
			{
				tileSetsData[i].Tiles[tilesets[i].warptile].TileGraphic = new() { tilesets[i].warpgraphic.topleft, tilesets[i].warpgraphic.topright, tilesets[i].warpgraphic.botleft, tilesets[i].warpgraphic.botright };

				/*
				_rom.Put(0x1000 + i * 0x200 + tilesets[i].warptile + 0x000, Blob.FromHex(Convert.ToHexString(new byte[] { tilesets[i].warpgraphic.topleft })));
				_rom.Put(0x1000 + i * 0x200 + tilesets[i].warptile + 0x080, Blob.FromHex(Convert.ToHexString(new byte[] { tilesets[i].warpgraphic.topright })));
				_rom.Put(0x1000 + i * 0x200 + tilesets[i].warptile + 0x100, Blob.FromHex(Convert.ToHexString(new byte[] { tilesets[i].warpgraphic.botleft })));
				_rom.Put(0x1000 + i * 0x200 + tilesets[i].warptile + 0x180, Blob.FromHex(Convert.ToHexString(new byte[] { tilesets[i].warpgraphic.botright })));*/
				for (int j = 0; j < tilesets[i].teleportdeck.Count(); j++)
				{
					if (tilesets[i].teleportdeck[j] >= 0x80)
					{
						tileSetsData[i].Tiles[tilesets[i].teleportdeck[j] % 0x80].TileGraphic = new() { tilesets[i].laddergraphic.topleft, tilesets[i].laddergraphic.topright, tilesets[i].laddergraphic.botleft, tilesets[i].laddergraphic.botright };
/*
						_rom.Put(0x1000 + i * 0x200 + 0x000 + (tilesets[i].teleportdeck[j] % 0x80), Blob.FromHex(Convert.ToHexString(new byte[] { tilesets[i].laddergraphic.topleft })));
						_rom.Put(0x1000 + i * 0x200 + 0x080 + (tilesets[i].teleportdeck[j] % 0x80), Blob.FromHex(Convert.ToHexString(new byte[] { tilesets[i].laddergraphic.topright })));
						_rom.Put(0x1000 + i * 0x200 + 0x100 + (tilesets[i].teleportdeck[j] % 0x80), Blob.FromHex(Convert.ToHexString(new byte[] { tilesets[i].laddergraphic.botleft })));
						_rom.Put(0x1000 + i * 0x200 + 0x180 + (tilesets[i].teleportdeck[j] % 0x80), Blob.FromHex(Convert.ToHexString(new byte[] { tilesets[i].laddergraphic.botright })));*/
					}
					else
					{
						tileSetsData[i].Tiles[tilesets[i].teleportdeck[j]].TileGraphic = new() { tilesets[i].teleportgraphic.topleft, tilesets[i].teleportgraphic.topright, tilesets[i].teleportgraphic.botleft, tilesets[i].teleportgraphic.botright };
						/*
						_rom.Put(0x1000 + i * 0x200 + 0x000 + tilesets[i].teleportdeck[j], Blob.FromHex(Convert.ToHexString(new byte[] { tilesets[i].teleportgraphic.topleft })));
						_rom.Put(0x1000 + i * 0x200 + 0x080 + tilesets[i].teleportdeck[j], Blob.FromHex(Convert.ToHexString(new byte[] { tilesets[i].teleportgraphic.topright })));
						_rom.Put(0x1000 + i * 0x200 + 0x100 + tilesets[i].teleportdeck[j], Blob.FromHex(Convert.ToHexString(new byte[] { tilesets[i].teleportgraphic.botleft })));
						_rom.Put(0x1000 + i * 0x200 + 0x180 + tilesets[i].teleportdeck[j], Blob.FromHex(Convert.ToHexString(new byte[] { tilesets[i].teleportgraphic.botright })));*/
					}
				}
			}

			for (int i = 1; i < 8; i++)
			{
				tilesetspinner.Add((byte)i);
			}
		}
		public void CreateDomains(MT19337 rng)
		{
			// It's 0x72 because we want to exclude the "boss battles".
			FormationLevel[] formationlevels = new FormationLevel[0x72 * 2];
			EnemyInfo[] monsters = new EnemyInfo[EnemyCount];
			var enemies = _rom.Get(EnemyOffset, EnemySize * EnemyCount).Chunk(EnemySize);
			var formations = _rom.Get(FormationDataOffset, FormationDataSize * 0x72).Chunk(FormationDataSize);

			// Get the data for all the individual monsters so we can examine their rewards.
			for (int i = 0; i < EnemyCount; i++)
			{
				monsters[i] = new EnemyInfo();
				monsters[i].decompressData(enemies[i]);
			}

			// Determine the level of each encounter (both sides) based on the monsters it contains.
			for (int i = 0; i < 0x72; i++)
			{
				formationlevels[i] = new FormationLevel();
				formationlevels[i].formation = new Encounters.FormationData(formations[i]);
				formationlevels[i].level = 0;
				formationlevels[i].index = (byte)i;
				formationlevels[i].level += MonsterLevel(monsters[formationlevels[i].formation.enemy1]) * (formationlevels[i].formation.minmax1.Item1 + formationlevels[i].formation.minmax1.Item2) / 2;
				formationlevels[i].level += MonsterLevel(monsters[formationlevels[i].formation.enemy2]) * (formationlevels[i].formation.minmax2.Item1 + formationlevels[i].formation.minmax2.Item2) / 2;
				formationlevels[i].level += MonsterLevel(monsters[formationlevels[i].formation.enemy3]) * (formationlevels[i].formation.minmax3.Item1 + formationlevels[i].formation.minmax3.Item2) / 2;
				formationlevels[i].level += MonsterLevel(monsters[formationlevels[i].formation.enemy4]) * (formationlevels[i].formation.minmax4.Item1 + formationlevels[i].formation.minmax4.Item2) / 2;
				formationlevels[i + 0x72] = new FormationLevel();
				formationlevels[i + 0x72].formation = new Encounters.FormationData(formations[i]);
				formationlevels[i + 0x72].level = 0;
				formationlevels[i + 0x72].index = (byte)(i + 0x80);
				formationlevels[i + 0x72].level += MonsterLevel(monsters[formationlevels[i + 0x72].formation.enemy1]) * (formationlevels[i + 0x72].formation.minmaxB1.Item1 + formationlevels[i + 0x72].formation.minmaxB1.Item2) / 2;
				formationlevels[i + 0x72].level += MonsterLevel(monsters[formationlevels[i + 0x72].formation.enemy2]) * (formationlevels[i + 0x72].formation.minmaxB2.Item1 + formationlevels[i + 0x72].formation.minmaxB2.Item2) / 2;
			}

			// Sort the list by level.
			Array.Sort(formationlevels, (x, y) => x.level.CompareTo(y.level));

			// Assign random formations of appropriate level to each floor.
			var lowest = 0;
			for (int i = 8; i < 61; i++)
			{
				lowest = (i - 8) * ((0x72 * 2 - 8) / 52);
				FormationLevel[] spinner = new FormationLevel[12];
				Array.Copy(formationlevels, lowest, spinner, 0, 12);
				for (int j = 0; j < 8; j++)
				{
					_rom.Put(0x2C200 + i * 8 + j, Blob.FromHex(Convert.ToHexString(new byte[] { spinner.PickRandom(rng).index })));
				}
			}

		}
		public void Generate(MT19337 rng, Overworld overworld, EncounterRate encounterRates, DialogueData dialogues)
		{
			/*
			if (!(bool)flags.Treasures)
			{
				throw new Exception("Treasures Shuffle must be enabled with Deep Dungeon.");
			}*/

			InitializeTilesets();
			var overworldMap = overworld.OverworldMap;

			// Close the city wall around Coneria to prevent exploring the world normally, as that
			// breaks things horribly.
			overworldMap.MapEditsToApply.Add(new List<MapEdit>
			{
				new MapEdit { X = 0x97, Y = 0xA3, Tile = CityWall },
				new MapEdit { X = 0x98, Y = 0xA3, Tile = CityWall },
				new MapEdit { X = 0x99, Y = 0xA3, Tile = CityWall },
				new MapEdit { X = 0x9A, Y = 0xA3, Tile = CityWall }
			});

			// Move the player's starting location up so that they're within the city wall.
			overworld.Locations.StartingLocation = new Sanity.SCCoords(overworld.Locations.StartingLocation.X, 0xA2);

			// Change the destination for Coneria Castle overworld teleporter so it puts us on the
			// "back" stairs for floor 1 of the Deep Dungeon.
			teleporters.OverworldTeleporters[OverworldTeleportIndex.ConeriaCastle1] = new TeleportDestination(MapLocation.ConeriaCastle1, MapIndex.TitansTunnel, new Coordinate(0x20, 0x20, CoordinateLocale.Standard));

			// Kill all the NPCs.
			KillNPCs();

			// Generate new monster domains based on "estimated power level".
			CreateDomains(rng);

			// Gaia and Onrac should really be encountered in the other order.
			byte[] dungeontowns = { 1, 2, 3, 4, 6, 5, 7 };

			// Speaking of Onrac, let's get rid of the submarine, as that breaks things horribly too.
			standardMaps[MapIndex.Onrac].Map[0x1E, 0x2E] = 0x27;

			// Pre-determine what floors will have town branches
			int[] townfloors = new int[7];
			Candidate[] towndestinations = new Candidate[7];
			var nexttown = 0;
			for (int i = 0; i < 7; i++)
			{
				townfloors[i] = RollDice(rng, 2, 3) + i * 7 + 7;
			}
			towndestinations[0].x = 0x13;
			towndestinations[0].y = 0x20;
			towndestinations[1].x = 0x29;
			towndestinations[1].y = 0x16;
			towndestinations[2].x = 0x01;
			towndestinations[2].y = 0x10;
			towndestinations[3].x = 0x0B;
			towndestinations[3].y = 0x17;
			towndestinations[4].x = 0x01;
			towndestinations[4].y = 0x0C;
			towndestinations[5].x = 0x3D;
			towndestinations[5].y = 0x3D;
			towndestinations[6].x = 0x13;
			towndestinations[6].y = 0x17;

			// Construct the entrance floor
			string deepdungeonentrance;
			if (flags.DDFiendOrbs)
			{
				deepdungeonentrance = "3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C34303030303030303030303030303030303030303030353C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C32313831313138313131313131313138383131313831333C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C32383831313831383131313131313131313831383138333C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C32313831313831383131343035313131383131383138333C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C32313831313831383131324C33313138313131383138333C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C32383838313138313131323133313138383831313831333C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C32313134303531313131323133313131313430353131333C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C323131324D333131313132313331313131324E333131333C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C32313132313331313131303130313131313231333131333C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C32313132313331313131313131313131313231333131333C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C32313132313030303031313131313030303231333131333C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C32313132313131313131314831313131313131333131333C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C32313132313530303031313131313030303231333131333C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C32313132313331313131313131313131313231333131333C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C32313132313331313131313131313131313231333131333C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3231313250333131313131313131313131324F333131333C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C32313130303031313131313131313131313030303131333C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C32383831313138313131313131313138313831313831333C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C32313138313831383131313131313138313831383138333C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C32313831313831383131313131313138383831383138333C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C32313138313831383131313131313131313831383138333C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C32383831313138313131313131313131313831313831333C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C30303030303030303030303030303030303030303030303C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C";
				standardMaps[(MapIndex)60].Map = new Map(Blob.FromHex(deepdungeonentrance).ToBytes());
				var mainentrance = tilesets[1].teleportdeck.SpliceRandom(rng);
				var skipentrance1 = tilesets[1].teleportdeck.SpliceRandom(rng);
				var skipentrance2 = tilesets[1].teleportdeck.SpliceRandom(rng);
				var skipentrance3 = tilesets[1].teleportdeck.SpliceRandom(rng);
				var skipentrance4 = tilesets[1].teleportdeck.SpliceRandom(rng);
				standardMaps[(MapIndex)60].Map[0x19, 0x20] = mainentrance;
				standardMaps[(MapIndex)60].Map[0x1C, 0x19] = skipentrance1;
				standardMaps[(MapIndex)60].Map[0x1C, 0x27] = skipentrance2;
				standardMaps[(MapIndex)60].Map[0x24, 0x19] = skipentrance3;
				standardMaps[(MapIndex)60].Map[0x24, 0x27] = skipentrance4;

				standardMaps[(MapIndex)60].MapTileSet = TileSets.Castle;
				tileSetsData[1].Tiles[mainentrance].Properties = new TileProp(0x80, 0x00);
				tileSetsData[1].Tiles[skipentrance1].Properties = new TileProp(0x80, 0x01);
				tileSetsData[1].Tiles[skipentrance2].Properties = new TileProp(0x80, 0x02);
				tileSetsData[1].Tiles[skipentrance3].Properties = new TileProp(0x80, 0x03);
				tileSetsData[1].Tiles[skipentrance4].Properties = new TileProp(0x80, 0x04);

				teleporters.StandardMapTeleporters[(TeleportIndex)0x00] = new TeleportDestination((MapIndex)0x08, new Coordinate(0x20, 0xA0, CoordinateLocale.Standard)); // Floor 1
				teleporters.StandardMapTeleporters[(TeleportIndex)0x01] = new TeleportDestination((MapIndex)0x11, new Coordinate(0x20, 0xA0, CoordinateLocale.Standard)); // Floor 10
				teleporters.StandardMapTeleporters[(TeleportIndex)0x02] = new TeleportDestination((MapIndex)0x1B, new Coordinate(0x20, 0xA0, CoordinateLocale.Standard)); // Floor 20
				teleporters.StandardMapTeleporters[(TeleportIndex)0x03] = new TeleportDestination((MapIndex)0x25, new Coordinate(0x20, 0xA0, CoordinateLocale.Standard)); // Floor 30
				teleporters.StandardMapTeleporters[(TeleportIndex)0x04] = new TeleportDestination((MapIndex)0x2F, new Coordinate(0x20, 0xA0, CoordinateLocale.Standard)); // Floor 40
				/*
				_rom.Put(0x800 + 0x100 + 2 * mainentrance, Blob.FromHex("8000"));
				_rom.Put(0x800 + 0x100 + 2 * skipentrance1, Blob.FromHex("8001"));
				_rom.Put(0x800 + 0x100 + 2 * skipentrance2, Blob.FromHex("8002"));
				_rom.Put(0x800 + 0x100 + 2 * skipentrance3, Blob.FromHex("8003"));
				_rom.Put(0x800 + 0x100 + 2 * skipentrance4, Blob.FromHex("8004"));
				_rom.Put(0x3F000 + 0, Blob.FromHex("20"));
				_rom.Put(0x3F100 + 0, Blob.FromHex("A0"));
				_rom.Put(0x3F200 + 0, Blob.FromHex("08")); // Floor 1
				_rom.Put(0x3F000 + 1, Blob.FromHex("20"));
				_rom.Put(0x3F100 + 1, Blob.FromHex("A0"));
				_rom.Put(0x3F200 + 1, Blob.FromHex("11")); // Floor 10
				_rom.Put(0x3F000 + 2, Blob.FromHex("20"));
				_rom.Put(0x3F100 + 2, Blob.FromHex("A0"));
				_rom.Put(0x3F200 + 2, Blob.FromHex("1B")); // Floor 20
				_rom.Put(0x3F000 + 3, Blob.FromHex("20"));
				_rom.Put(0x3F100 + 3, Blob.FromHex("A0"));
				_rom.Put(0x3F200 + 3, Blob.FromHex("25")); // Floor 30
				_rom.Put(0x3F000 + 4, Blob.FromHex("20"));
				_rom.Put(0x3F100 + 4, Blob.FromHex("A0"));
				_rom.Put(0x3F200 + 4, Blob.FromHex("2F")); // Floor 40*/
				// Put the fiend orbs as guards in front of the other entrances
				standardMaps[(MapIndex)60].MapObjects.SetNpc(0, ObjectId.CastleOrdealsOldMan, 0x20, 0x1D, false, true);
				standardMaps[(MapIndex)60].MapObjects.SetNpc(1, (ObjectId)0x1B, 0x19, 0x1D, false, true);
				standardMaps[(MapIndex)60].MapObjects.SetNpc(2, (ObjectId)0x1C, 0x27, 0x1D, false, true);
				standardMaps[(MapIndex)60].MapObjects.SetNpc(3, (ObjectId)0x1D, 0x19, 0x23, false, true);
				standardMaps[(MapIndex)60].MapObjects.SetNpc(4, (ObjectId)0x1E, 0x27, 0x23, false, true);
				dialogues[0x2D] = "Welcome to Deep Dungeon.\nThis entrance takes you\nto the first floor.\nIf you can defeat the\nfiends, you can skip\nahead. GOOD LUCK!";
			}
			else
			{
				deepdungeonentrance = "3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C34303030303030303030353C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C32313131313131313131333C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C32313131313131313131333C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C32313131313131313131333C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C32313131315831313131333C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C32313131585831313131333C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C32313131315831313131333C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C34303030303030303032313131315831313131333030303030303030353C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C32313131313131313131313131585858313131313131313131313131333C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C32313131313131313131313131343035313131313131313131313131333C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C32313131315858313158583131324C33315858313131585831313131333C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C32313131313131583131315831323133313131583158313131313131333C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C32313131313158313131583131323133315858313158585831313131333C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C32313131315831313158313131323133313131583158315831313131333C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C32313131315858583158585834303130355858313131583131313131333C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C32313131313131313131313132313131333131313131313131313131333C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C32313131313134303030303030313131303030303030353131313131333C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C323131313131324D313131313131483131313131314E333131313131333C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C32313131313130303030303032313131333030303030303131313131333C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C32313131313131313131313132313131333131313131313131313131333C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C32313131313131313131313130343135303131313131313131313131333C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C30303030303030303032313131323133313131333030303030303030303C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C32313131323133313131333C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C32313131323133313131333C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C32313131324F33313131333C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C32313131303030313131333C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C32313131383838313131333C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C32313138313131383131333C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C32313131383838313131333C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C32313138313131383131333C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C32313131383838313131333C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C32313131313131313131333C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C32313131313131313131333C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C32313131313131313131333C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C30303030303030303030303C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C";
				standardMaps[(MapIndex)60].Map = new Map(Blob.FromHex(deepdungeonentrance).ToBytes());
				var mainentrance = tilesets[1].teleportdeck.SpliceRandom(rng);
				var skipentrance1 = tilesets[1].teleportdeck.SpliceRandom(rng);
				var skipentrance2 = tilesets[1].teleportdeck.SpliceRandom(rng);
				var skipentrance3 = tilesets[1].teleportdeck.SpliceRandom(rng);
				standardMaps[(MapIndex)60].Map[0x19, 0x20] = mainentrance;
				standardMaps[(MapIndex)60].Map[0x27, 0x20] = skipentrance1;
				standardMaps[(MapIndex)60].Map[0x20, 0x19] = skipentrance2;
				standardMaps[(MapIndex)60].Map[0x20, 0x27] = skipentrance3;

				standardMaps[(MapIndex)60].MapTileSet = TileSets.Castle;
				tileSetsData[1].Tiles[mainentrance].Properties = new TileProp(0x80, 0x00);
				tileSetsData[1].Tiles[skipentrance1].Properties = new TileProp(0x80, 0x01);
				tileSetsData[1].Tiles[skipentrance2].Properties = new TileProp(0x80, 0x02);
				tileSetsData[1].Tiles[skipentrance3].Properties = new TileProp(0x80, 0x03);


				teleporters.StandardMapTeleporters[(TeleportIndex)0x00] = new TeleportDestination((MapIndex)0x08, new Coordinate(0x20, 0xA0, CoordinateLocale.Standard)); // Floor 1
				teleporters.StandardMapTeleporters[(TeleportIndex)0x01] = new TeleportDestination((MapIndex)0x0F, new Coordinate(0x20, 0xA0, CoordinateLocale.Standard)); // Floor 8
				teleporters.StandardMapTeleporters[(TeleportIndex)0x02] = new TeleportDestination((MapIndex)0x1D, new Coordinate(0x20, 0xA0, CoordinateLocale.Standard)); // Floor 22
				teleporters.StandardMapTeleporters[(TeleportIndex)0x03] = new TeleportDestination((MapIndex)0x2B, new Coordinate(0x20, 0xA0, CoordinateLocale.Standard)); // Floor 36
				/*
_rom.Put(0x800 + 0x100 + 2 * mainentrance, Blob.FromHex("8000"));
_rom.Put(0x800 + 0x100 + 2 * skipentrance1, Blob.FromHex("8001"));
_rom.Put(0x800 + 0x100 + 2 * skipentrance2, Blob.FromHex("8002"));
_rom.Put(0x800 + 0x100 + 2 * skipentrance3, Blob.FromHex("8003"));*/

				/*
				_rom.Put(0x3F000 + 0, Blob.FromHex("20"));
				_rom.Put(0x3F100 + 0, Blob.FromHex("A0"));
				_rom.Put(0x3F200 + 0, Blob.FromHex("08")); // Floor 1
				_rom.Put(0x3F000 + 1, Blob.FromHex("20"));
				_rom.Put(0x3F100 + 1, Blob.FromHex("A0"));
				_rom.Put(0x3F200 + 1, Blob.FromHex("0F")); // Floor 8
				_rom.Put(0x3F000 + 2, Blob.FromHex("20"));
				_rom.Put(0x3F100 + 2, Blob.FromHex("A0"));
				_rom.Put(0x3F200 + 2, Blob.FromHex("1D")); // Floor 22
				_rom.Put(0x3F000 + 3, Blob.FromHex("20"));
				_rom.Put(0x3F100 + 3, Blob.FromHex("A0"));
				_rom.Put(0x3F200 + 3, Blob.FromHex("2B")); // Floor 36*/
				// Put some guards in front of the other entrances
				// The "ordeals man" will be in front of 1, explaining what's going on and then vanishing
				// The "tnt dwarf" will guard the entrance for 8
				// The "titan" will be in front of the entrance for 22
				// The "submarine girl" will be in front of the entrance for 36
				standardMaps[(MapIndex)60].MapObjects.SetNpc(0, ObjectId.CastleOrdealsOldMan, 0x20, 0x1D, false, true);
				standardMaps[(MapIndex)60].MapObjects.SetNpc(1, ObjectId.Nerrick, 0x20, 0x26, false, true);
				standardMaps[(MapIndex)60].MapObjects.SetNpc(2, ObjectId.Titan, 0x1A, 0x20, false, true);
				standardMaps[(MapIndex)60].MapObjects.SetNpc(3, ObjectId.SubEngineer, 0x26, 0x20, false, true);

				dialogues[0x2D] = "Welcome to Deep Dungeon.\nThis entrance takes you\nto the first floor.\nBring these other folks\nthe items they need to\nskip ahead. GOOD LUCK!";
				dialogues[0x13] = "Bring me TNT if you wish\nto skip to floor 8.";
				dialogues[0x29] = "Bring me a RUBY if\nyou wish to skip to\nfloor 22.";
				dialogues[0x25] = "Bring me OXYALE if you\nwish to skip to floor 36.";
			}


			// Construct the Chaos floor
			// Put Chaos NPCs back (the initial NPC genocide would have removed them)
			standardMaps[(MapIndex)59].MapObjects.SetNpc(0, (ObjectId)0x18, 0x0F, 0x13, true, true);
			standardMaps[(MapIndex)59].MapObjects.SetNpc(1, (ObjectId)0x19, 0x0F, 0x12, true, true);
			standardMaps[(MapIndex)59].MapObjects.SetNpc(2, (ObjectId)0x1A, 0x0F, 0x11, true, true);

			// No encounters
			encounterRates.UpdateRate((MapIndex)59, 0);

			// Generate the map layouts.
			for (int i = 8; i < 59; i++)
			{
				// Set the encounter rate
				encounterRates.UpdateRate((MapIndex)i, 8);

				// Pick a tileset with unused exit tiles.
				if (flags.DDProgressiveTilesets)
				{
					tilesetmappings[i] = tilesetspinner[0];
				}
				else
				{
					tilesetmappings[i] = tilesetspinner.PickRandom(rng);
				}
				if (townfloors[nexttown] == i)
				{
					// If it's a town branch floor we need at least two exits available.
					// If there aren't, we bump the town up to the next floor.
					if (tilesets[tilesetmappings[i]].teleportdeck.Count() < 2)
					{
						townfloors[nexttown]++;
					}
				}
				standardMaps[(MapIndex)i].MapTileSet = (TileSets)tilesetmappings[i];

				// Start from a clean slate.
				WipeMap(standardMaps[(MapIndex)i].Map, tilesets[tilesetmappings[i]]);

				// Which layout generation algorithm to use:
				//  "Box Style" looks a bit like earth/volcano lower floors where you have
				//              interconnecting boxes.
				//  "Snake Style" looks a bit like marsh B1 where you have a long meandering
				//                path that may branch.
                //  "Grid Style" looks a bit like marsh B3 or even the maze floor in the sky
                //               palace where the map is divided into a grid of large squares
                //               that could each have various things in them like rooms or
                //               holes or random obstacles.
				//  I have other ideas for styles to implement but I want to get something
				//  working out the door first and then gradually improve it later. Baby steps.
				switch (RollDice(rng, 1, 10))
				{
					case 1:
						GenerateMapGridStyle(rng, standardMaps[(MapIndex)i].Map, tilesets[tilesetmappings[i]]);
						break;
					case 2:
					case 3:
					case 4:
						GenerateMapSnakeStyle(rng, standardMaps[(MapIndex)i].Map, tilesets[tilesetmappings[i]]);
						break;
					default:
						GenerateMapBoxStyle(rng, standardMaps[(MapIndex)i].Map, tilesets[tilesetmappings[i]]);
						break;
				}

				// Make the tiles look right.
				Beautify(standardMaps[(MapIndex)i].Map, tilesets[tilesetmappings[i]]);

				// Connect it to the next map.
				if (i < 58)
				{
					tileSetsData[tilesetmappings[i]].Tiles[PlaceExit(rng, standardMaps[(MapIndex)i].Map, tilesets[tilesetmappings[i]])].Properties = new TileProp(0x80, (byte)(i - 3));
					teleporters.StandardMapTeleporters[(TeleportIndex)(i - 3)] = new TeleportDestination((MapIndex)(i + 1), new Coordinate(0x20, 0xA0, CoordinateLocale.Standard));

					/*
					_rom.Put(0x800 + tilesetmappings[i] * 0x100 + 2 * PlaceExit(rng, maps[i], tilesets[tilesetmappings[i]]), Blob.FromHex("80" + Convert.ToHexString(new byte[] { (byte)(i - 3) })));
					_rom.Put(0x3F000 + i - 3, Blob.FromHex("20"));
					_rom.Put(0x3F100 + i - 3, Blob.FromHex("A0"));
					_rom.Put(0x3F200 + i - 3, Blob.FromHex(Convert.ToHexString(new byte[] { (byte)(i + 1) })));*/
					if (townfloors[nexttown] == i)
					{
						tileSetsData[tilesetmappings[i]].Tiles[PlaceExit(rng, standardMaps[(MapIndex)i].Map, tilesets[tilesetmappings[i]])].Properties = new TileProp(0x80, (byte)(nexttown + 57));
						teleporters.StandardMapTeleporters[(TeleportIndex)(nexttown + 57)] = new TeleportDestination((MapIndex)dungeontowns[nexttown], new Coordinate((byte)towndestinations[nexttown].x, (byte)(towndestinations[nexttown].y + 0x80), CoordinateLocale.Standard));
						/*
						_rom.Put(0x800 + tilesetmappings[i] * 0x100 + 2 * PlaceExit(rng, maps[i], tilesets[tilesetmappings[i]]), Blob.FromHex("80" + Convert.ToHexString(new byte[] { (byte)(nexttown + 57) })));
						_rom.Put(0x3F000 + nexttown + 57, Blob.FromHex(Convert.ToHexString(new byte[] { (byte)towndestinations[nexttown].x })));
						_rom.Put(0x3F100 + nexttown + 57, Blob.FromHex(Convert.ToHexString(new byte[] { (byte)(towndestinations[nexttown].y + 0x80) })));
						_rom.Put(0x3F200 + nexttown + 57, Blob.FromHex(Convert.ToHexString(new byte[] { dungeontowns[nexttown] })));*/
						if (nexttown < 6) nexttown++;
					}
				}
				else
				{
					tileSetsData[tilesetmappings[i]].Tiles[PlaceExit(rng, standardMaps[(MapIndex)i].Map, tilesets[tilesetmappings[i]])].Properties = new TileProp(0x80, (byte)(i - 3));
					teleporters.StandardMapTeleporters[(TeleportIndex)(i - 3)] = new TeleportDestination((MapIndex)(i + 1), new Coordinate(0x0F, 0x87, CoordinateLocale.Standard));
					/*
					_rom.Put(0x800 + tilesetmappings[i] * 0x100 + 2 * PlaceExit(rng, maps[i], tilesets[tilesetmappings[i]]), Blob.FromHex("80" + Convert.ToHexString(new byte[] { (byte)(i - 3) })));
					_rom.Put(0x3F000 + i - 3, Blob.FromHex("0F"));
					_rom.Put(0x3F100 + i - 3, Blob.FromHex("87"));*/
					if (flags.ToFRMode == ToFRMode.Short)
					{
						//teleporters.StandardMapTeleporters[(TeleportIndex)(i - 3)] = new TeleportDestination((MapIndex)(i + 1), new Coordinate(0x0F, 0x83, CoordinateLocale.StandardInRoom));

						//_rom.Put(0x3F100 + i - 3, Blob.FromHex("89"));
						standardMaps[(MapIndex)59].Map[0x07, 0x0F] = tilesets[7].doortile;
						standardMaps[(MapIndex)59].Map[0x09, 0x0F] = tilesets[7].warptile;
						standardMaps[(MapIndex)59].Map[0x04, 0x0F] = tilesets[7].roomtile;
					}
					//_rom.Put(0x3F200 + i - 3, Blob.FromHex(Convert.ToHexString(new byte[] { (byte)(i + 1) })));
					//PlaceChaos(rng, maps[i], tilesets[tilesetmappings[i]]);
				}

				// If all its exits are now used, remove the tileset from the list of available ones.
				if (tilesets[tilesetmappings[i]].teleportdeck.Count() == 0)
				{
					tilesetspinner.Remove(tilesetmappings[i]);
				}
			}

			// Distribute chests, but don't put treasure in them.
			DistributeTreasure(rng, flags);

			// Put Bahamut and a TAIL somewhere in the dungeon.
			PlaceBahamut(rng, flags);

			// Placce WarMech if it's enabled
			PlaceWarMech(rng, flags);

			// Assign random palettes to the maps.
			SpinPalettes(rng);

			PlacedItems = PlaceTreasures(flags, rng);
			ChestLocations = chests.Where(c => !PlacedItems.Select(p => p.Address).Contains(c.Address)).ToList();

			// Debug information
			for (int i = 0; i < tilesetspinner.Count; i++)
			{
				Console.WriteLine("Tileset " + tilesetspinner[i] + " has teleports remaining: " + tilesets[tilesetspinner[i]].teleportdeck.Count);
			}
		}

		public void SpinPalettes(MT19337 rng)
		{
			// Assigns the inner map with the given index a random palette.
			var palettes = OverworldMap.GeneratePalettes(_rom.Get(OverworldMap.MapPaletteOffset, OverworldMap.MapCount * OverworldMap.MapPaletteSize).Chunk(OverworldMap.MapPaletteSize));
			for (int i = 8; i < 61; i++)
			{
				var pal = palettes[(OverworldMap.Palette)rng.Between(1, palettes.Count() - 1)];
				if (tilesetmappings[i] == 1)
				{
					_rom.Put(OverworldMap.MapPaletteOffset + i * OverworldMap.MapPaletteSize + 8, pal.SubBlob(8, 8));
					_rom.Put(OverworldMap.MapPaletteOffset + i * OverworldMap.MapPaletteSize + 40, pal.SubBlob(40, 8));
				}
				else
				{
					var paletteIndex = 32;
					_rom.Put(OverworldMap.MapPaletteOffset + i * OverworldMap.MapPaletteSize + paletteIndex, pal.SubBlob(paletteIndex, 48 - paletteIndex));
					_rom.Put(OverworldMap.MapPaletteOffset + i * OverworldMap.MapPaletteSize, pal.SubBlob(0, 16));
				}
				// Make NPC palette look right
				_rom.Put(OverworldMap.MapPaletteOffset + i * OverworldMap.MapPaletteSize + 24, Blob.FromHex("0F0F27360F0F1436"));
			}
		}
		public void DistributeTreasure(MT19337 rng, Flags flags)
		{
			List<byte> mapspinner;
			List<Candidate> candidates;
			List<byte> currentdeck;
			Candidate c;
			List<byte> accessibleroomtiles;
			int chestAddress = 0x3101;
			int chestCount = 0;

			// First we distribute the actual treasure box tiles, not caring what's in them.
			// The boxes are organized by tileset, so we deal with them one tileset at a time.
			for (int i = 1; i <= 7; i++)
			{
				accessibleroomtiles = new List<byte>();
				accessibleroomtiles.Add(tilesets[i].roomtile);
				accessibleroomtiles.Add(tilesets[i].roomlower);
				// Create a "deck" of the tileset's treasure tiles.
				currentdeck = new List<byte>();
				for (int j = 0; j < tilesets[i].treasuredeck.Count(); j++)
				{
					currentdeck.Add(tilesets[i].treasuredeck[j]);
				}
				// Next we need a "spinner" (list) of all the maps that have that tileset where we
				// can put the boxes.
				mapspinner = new List<byte>();
				for (int j = 8; j < 61; j++)
				{
					if (tilesetmappings[j] == i)
					{
						mapspinner.Add((byte)j);
					}
				}
				// Keep dropping boxes until there are no boxes left to drop or no maps left with
				// available spots to put boxes.
				while (mapspinner.Count() > 0 && currentdeck.Count() > 0)
				{
					var currentmap = mapspinner.PickRandom(rng);
					// Create a list of legal places to put boxes.
					// This has to be done each time a box is placed because boxes are solid and
					// thus could block the way to previously available tiles.
					candidates = new List<Candidate>();
					for (int j = 1; j < 63; j++)
					{
						for (int k = 1; k < 63; k++)
						{
							if (standardMaps[(MapIndex)currentmap].Map[k, j] == tilesets[i].roomtile)
							{
								// To be a viable candidate, the spot has to not only not block
								// the path, but also be accessible by one of the cardinal directions.
								if (accessibleroomtiles.Contains(standardMaps[(MapIndex)currentmap].Map[k - 1, j]) ||
									accessibleroomtiles.Contains(standardMaps[(MapIndex)currentmap].Map[k + 1, j]) ||
									accessibleroomtiles.Contains(standardMaps[(MapIndex)currentmap].Map[k, j - 1]) ||
									accessibleroomtiles.Contains(standardMaps[(MapIndex)currentmap].Map[k, j + 1]))
								{
									c = new Candidate(j, k);
									if (Traversible(standardMaps[(MapIndex)currentmap].Map, tilesets[i], j, k, 1, 1, true))
									{
										candidates.Add(c);
									}
								}
							}
						}
					}
					// If the list of candidates is empty, this is no longer a viable tileset for
					// placing treasures in.
					if (candidates.Count() == 0)
					{
						mapspinner.Remove(currentmap);
					}
					else
					{
						c = candidates.SpliceRandom(rng);
						standardMaps[(MapIndex)currentmap].Map[c.y, c.x] = currentdeck.SpliceRandom(rng);
						chests.Add(new TreasureChest(chestAddress + chestCount,
							"DeepDungeon" + (currentmap - 7) + "B" + "_Chest" + chestCount,
							(MapLocation)currentmap,
							Item.None,
							flags.DDFiendOrbs ? AccessRequirement.None :
							((currentmap > tntfloor ? AccessRequirement.Tnt : AccessRequirement.None) |
							(currentmap > rubyfloor ? AccessRequirement.Ruby : AccessRequirement.None) |
							(currentmap > oxyfloor ? AccessRequirement.Oxyale : AccessRequirement.None))));
						tileSetsData[tilesetmappings[currentmap]].Tiles[standardMaps[(MapIndex)currentmap].Map[c.y, c.x]].Properties = new TileProp(0x09, (byte)(chestCount + 1));
						//_rom.Put(0x800 + tilesetmappings[currentmap] * 0x100 + maps[currentmap][c.y, c.x] * 2, Blob.FromHex("09" + Convert.ToHexString(new byte[] { (byte)(chestCount + 1) })));
						chestCount++;
					}
				}
			}
		}
		public List<IRewardSource> PlaceTreasures(Flags flags, MT19337 rng)
		{
			/*
			int TreasureOffset = 0x03100;
			int TreasureSize = 1;
			int TreasureCount = 256;*/

			List<IRewardSource> placedItems = new();

				if (!flags.DDFiendOrbs)
			{
				// Place TNT
				var postTNTchests = chests.Where(x => (int)x.MapLocation >= tntfloor && !placedItems.Any(y => y.Address == x.Address)).ToList();
				postTNTchests.Shuffle(rng);
				placedItems.Add(new TreasureChest(postTNTchests.OrderBy(x => x.MapLocation).First(), Item.Tnt));

				// Place Ruby
				var postRubyChests = chests.Where(x => (int)x.MapLocation >= rubyfloor && !placedItems.Any(y => y.Address == x.Address)).ToList();
				postRubyChests.Shuffle(rng);
				placedItems.Add(new TreasureChest(postRubyChests.OrderBy(x => x.MapLocation).First(), Item.Ruby));

				// Place Oxyale
				var postOxyaleChests = chests.Where(x => (int)x.MapLocation >= oxyfloor && !placedItems.Any(y => y.Address == x.Address)).ToList();
				postOxyaleChests.Shuffle(rng);
				placedItems.Add(new TreasureChest(postOxyaleChests.OrderBy(x => x.MapLocation).First(), Item.Oxyale));
			}

			// Place Tail
			var postTailChests = chests.Where(x => (int)x.MapLocation >= tailfloor && !placedItems.Any(y => y.Address == x.Address)).ToList();
			postTailChests.Shuffle(rng);
			placedItems.Add(new TreasureChest(postTailChests.OrderBy(x => x.MapLocation).First(), Item.Tail));

			// Three chests in three disjoint parts of the dungeon are replaced with Ribbon.
			for (int i = 1; i <= 3; i++)
			{
				var ribbonfloor = RollDice(rng, 1, 8) + 8 + i * 8;
				var postRibbonChests = chests.Where(x => (int)x.MapLocation >= ribbonfloor && !placedItems.Any(y => y.Address == x.Address)).ToList();
				postRibbonChests.Shuffle(rng);
				placedItems.Add(new TreasureChest(postRibbonChests.OrderBy(x => x.MapLocation).First(), Item.Ribbon));
				//treasurePool.Remove(Item.Ribbon);
			}

			/*
			// Place guaranteed casting items
			if (flags.GuaranteedDefenseItem != GuaranteedDefenseItem.None && !(flags.ItemMagicMode == ItemMagicMode.None))
			{
				var postGuaranttedChests = chests.Where(x => (int)x.MapLocation >= 33 && !placedItems.Any(y => y.Address == x.Address)).ToList();
				placedItems.Add(new TreasureChest(postGuaranttedChests.PickRandom(rng), Item.PowerRod));
			}

			if (flags.GuaranteedPowerItem != GuaranteedPowerItem.None && !(flags.ItemMagicMode == ItemMagicMode.None))
			{
				var postGuaranttedChests = chests.Where(x => (int)x.MapLocation >= 33 && !placedItems.Any(y => y.Address == x.Address)).ToList();
				placedItems.Add(new TreasureChest(postGuaranttedChests.PickRandom(rng), Item.PowerGauntlets));
			}

			// Place Shards
			for (int i = 0; i < shardsAdded; i++)
			{
				var validShardsLocations = chests.Where(x => !placedItems.Any(y => y.Address == x.Address)).ToList();
				treasurePool.Remove(Item.Shard);
				placedItems.Add(new TreasureChest(validShardsLocations.PickRandom(rng), Item.Shard));
			}

			// Fill pool with cabins to account for removed items
			var locationCount = chests.Where(x => !placedItems.Any(y => y.Address == x.Address)).Count();

			if (locationCount > treasurePool.Count())
			{
				treasurePool.AddRange(Enumerable.Repeat(Item.Heal, locationCount - treasurePool.Count()));
			}

			List<IRewardSource> normalTreasures = new();

			var randomizeTreasureMode = (flags.RandomizeTreasure == RandomizeTreasureMode.Random) ? (RandomizeTreasureMode)rng.Between(0, 2) : flags.RandomizeTreasure;

			if (randomizeTreasureMode != RandomizeTreasureMode.None)
			{
				IItemGenerator generator;
				// We want to leave out anything incentivized (and thus already placed), but
				// add all the other stuff that you can't find in vanilla.
				if (randomizeTreasureMode == RandomizeTreasureMode.DeepDungeon)
				{
					var validchests = chests.Where(x => !placedItems.Any(y => y.Address == x.Address)).ToList();

					generator = new DeepDungeonItemGenerator(chests.Where(x => !placedItems.Any(y => y.Address == x.Address)).ToList(), ItemLists.UnusedGoldItems.ToList(), incentiveData.RemovedItems.ToList(), normalTreasures, FloorsWeightSelector.GetWeights(validchests, flags), priceList, (flags.GameMode == GameModes.DeepDungeon), flags.Etherizer);
				}
				else
				{
					generator = new ItemGenerator(treasurePool.ToList(), ItemLists.UnusedGoldItems.ToList(), incentiveData.RemovedItems.ToList(), flags.WorldWealth);
				}

				treasurePool = treasurePool.Select(treasure => generator.GetItem(rng)).ToList();
			}

			// Place treasures
			if (randomizeTreasureMode == RandomizeTreasureMode.DeepDungeon && flags.DeepDungeonGenerator == DeepDungeonGeneratorMode.Progressive)
			{
				placedItems.AddRange(normalTreasures);
			}
			else
			{
				var itemLocationPool = chests.Where(x => !placedItems.Any(y => y.Address == x.Address)).ToList();
				itemLocationPool.Shuffle(rng);
				treasurePool.Shuffle(rng);

				var leftovers = treasurePool.Zip(itemLocationPool, (treasure, location) => new TreasureChest(location, treasure));
				placedItems.AddRange(leftovers);
			}

			// Output the results to the ROM
			foreach (var item in placedItems.Where(x => !x.IsUnused && x.Address < 0x80000 && (x is TreasureChest)))
			{
				//Debug.WriteLine(item.SpoilerText);
				item.Put(_rom);
			}
			*/
			return placedItems;
		}
		/*
		public List<IRewardSource> PlaceTreasures(Flags flags, PlacementContext incentiveData, MT19337 rng)
		{
			int TreasureOffset = 0x03100;
			int TreasureSize = 1;
			int TreasureCount = 256;

			List<IRewardSource> placedItems = new();

			var treasureBlob = _rom.Get(TreasureOffset, TreasureSize * TreasureCount);
			var treasurePool = UsedTreasureIndices.Select(x => (Item)treasureBlob[x])
							.Concat(ItemLists.AllNonTreasureChestItems).Where(x => !ItemLists.AllQuestItems.Contains(x)).ToList();
			var priceList = _rom.Get(0x37C00, 0x200).ToUShorts().Select(x => (int)x).ToList(); // Temprorary until we extract price
			int shardsAdded = 0;

			if (flags.ShardHunt)
			{
				treasurePool = treasurePool.Select(_rom.ShardHuntTreasureSelector).ToList();
				shardsAdded = treasurePool.Count(item => item == Item.Shard);
			}

			if (!flags.DDFiendOrbs)
			{
				// Place TNT
				var postTNTchests = chests.Where(x => (int)x.MapLocation >= tntfloor && !placedItems.Any(y => y.Address == x.Address)).ToList();
				postTNTchests.Shuffle(rng);
				placedItems.Add(new TreasureChest(postTNTchests.OrderBy(x => x.MapLocation).First(), Item.Tnt));

				// Place Ruby
				var postRubyChests = chests.Where(x => (int)x.MapLocation >= rubyfloor && !placedItems.Any(y => y.Address == x.Address)).ToList();
				postRubyChests.Shuffle(rng);
				placedItems.Add(new TreasureChest(postRubyChests.OrderBy(x => x.MapLocation).First(), Item.Ruby));

				// Place Oxyale
				var postOxyaleChests = chests.Where(x => (int)x.MapLocation >= oxyfloor && !placedItems.Any(y => y.Address == x.Address)).ToList();
				postOxyaleChests.Shuffle(rng);
				placedItems.Add(new TreasureChest(postOxyaleChests.OrderBy(x => x.MapLocation).First(), Item.Oxyale));
			}

			// Place Tail
			var postTailChests = chests.Where(x => (int)x.MapLocation >= tailfloor && !placedItems.Any(y => y.Address == x.Address)).ToList();
			postTailChests.Shuffle(rng);
			placedItems.Add(new TreasureChest(postTailChests.OrderBy(x => x.MapLocation).First(), Item.Tail));

			// Three chests in three disjoint parts of the dungeon are replaced with Ribbon.
			for (int i = 1; i <= 3; i++)
			{
				var ribbonfloor = RollDice(rng, 1, 8) + 8 + i * 8;
				var postRibbonChests = chests.Where(x => (int)x.MapLocation >= ribbonfloor && !placedItems.Any(y => y.Address == x.Address)).ToList();
				postRibbonChests.Shuffle(rng);
				placedItems.Add(new TreasureChest(postRibbonChests.OrderBy(x => x.MapLocation).First(), Item.Ribbon));
				treasurePool.Remove(Item.Ribbon);
			}

			// Place guaranteed casting items
			if (flags.GuaranteedDefenseItem != GuaranteedDefenseItem.None && !(flags.ItemMagicMode == ItemMagicMode.None))
			{
				var postGuaranttedChests = chests.Where(x => (int)x.MapLocation >= 33 && !placedItems.Any(y => y.Address == x.Address)).ToList();
				placedItems.Add(new TreasureChest(postGuaranttedChests.PickRandom(rng), Item.PowerRod));
			}

			if (flags.GuaranteedPowerItem != GuaranteedPowerItem.None && !(flags.ItemMagicMode == ItemMagicMode.None))
			{
				var postGuaranttedChests = chests.Where(x => (int)x.MapLocation >= 33 && !placedItems.Any(y => y.Address == x.Address)).ToList();
				placedItems.Add(new TreasureChest(postGuaranttedChests.PickRandom(rng), Item.PowerGauntlets));
			}

			// Place Shards
			for (int i = 0; i < shardsAdded; i++)
			{
				var validShardsLocations = chests.Where(x => !placedItems.Any(y => y.Address == x.Address)).ToList();
				treasurePool.Remove(Item.Shard);
				placedItems.Add(new TreasureChest(validShardsLocations.PickRandom(rng), Item.Shard));
			}

			// Fill pool with cabins to account for removed items
			var locationCount = chests.Where(x => !placedItems.Any(y => y.Address == x.Address)).Count();

			if (locationCount > treasurePool.Count())
			{
				treasurePool.AddRange(Enumerable.Repeat(Item.Heal, locationCount - treasurePool.Count()));
			}

			List<IRewardSource> normalTreasures = new();

			var randomizeTreasureMode = (flags.RandomizeTreasure == RandomizeTreasureMode.Random) ? (RandomizeTreasureMode)rng.Between(0, 2) : flags.RandomizeTreasure;

			if (randomizeTreasureMode != RandomizeTreasureMode.None)
			{
				IItemGenerator generator;
				// We want to leave out anything incentivized (and thus already placed), but
				// add all the other stuff that you can't find in vanilla.
				if (randomizeTreasureMode == RandomizeTreasureMode.DeepDungeon)
				{
					var validchests = chests.Where(x => !placedItems.Any(y => y.Address == x.Address)).ToList();

					generator = new DeepDungeonItemGenerator(chests.Where(x => !placedItems.Any(y => y.Address == x.Address)).ToList(), ItemLists.UnusedGoldItems.ToList(), incentiveData.RemovedItems.ToList(), normalTreasures, FloorsWeightSelector.GetWeights(validchests, flags), priceList, (flags.GameMode == GameModes.DeepDungeon), flags.Etherizer);
				}
				else
				{
					generator = new ItemGenerator(treasurePool.ToList(), ItemLists.UnusedGoldItems.ToList(), incentiveData.RemovedItems.ToList(), flags.WorldWealth);
				}

				treasurePool = treasurePool.Select(treasure => generator.GetItem(rng)).ToList();
			}

			// Place treasures
			if (randomizeTreasureMode == RandomizeTreasureMode.DeepDungeon && flags.DeepDungeonGenerator == DeepDungeonGeneratorMode.Progressive)
			{
				placedItems.AddRange(normalTreasures);
			}
			else
			{
				var itemLocationPool = chests.Where(x => !placedItems.Any(y => y.Address == x.Address)).ToList();
				itemLocationPool.Shuffle(rng);
				treasurePool.Shuffle(rng);

				var leftovers = treasurePool.Zip(itemLocationPool, (treasure, location) => new TreasureChest(location, treasure));
				placedItems.AddRange(leftovers);
			}

			// Output the results to the ROM
			foreach (var item in placedItems.Where(x => !x.IsUnused && x.Address < 0x80000 && (x is TreasureChest)))
			{
				//Debug.WriteLine(item.SpoilerText);
				item.Put(_rom);
			}

			return placedItems;
		}
		*/
		public int RollDice(MT19337 rng, int dice, int sides)
		{
			// Roll a number of dice and add up their results.
			int result = 0;
			for (int i = 0; i < dice; i++)
			{
				result += rng.Between(1, sides);
			}
			return result;
		}
		private void WipeMap(Map m, Tileset t)
		{
			// Clear the whole map.
			m.Fill((0, 0), (64, 64), t.abysstile);
		}

		private void KillNPCs()
		{
			// We don't want NPCs in weird places, and in fact we don't need them at all.
			for (int i = 0; i < 61; i++)
			{
				for (int j = 0; j < 16; j++)
				{
					standardMaps[(MapIndex)i].MapObjects.SetNpc(j, 0, 0, 0, false, true);
				}
			}
		}

		private void Beautify(Map m, Tileset t)
		{
			// The map generator algorithms are a bit rough around the edges so this
			// is here to make sure the corner tiles look correct and the rooms are
			// properly bordered and such.
			bool upper = false;
			bool lower = false;
			bool left = false;
			bool right = false;
			for (int i = 0; i < 64; i++)
			{
				for (int j = 0; j < 64; j++)
				{
					upper = false;
					lower = false;
					left = false;
					right = false;
					if (m[j, i] == t.walltile)
					{
						if (j < 63)
						{
							if (m[j + 1, i] == t.leftwalltile)
							{
								m[j, i] = t.wallupperleft;
							}
							else if (m[j + 1, i] == t.rightwalltile)
							{
								m[j, i] = t.wallupperright;
							}
						}
					}
					else if (m[j, i] == t.wallupperleft || m[j, i] == t.wallupperright)
					{
						if (j < 63)
						{
							if (m[j + 1, i] == t.floortile)
							{
								m[j, i] = t.walltile;
							}
						}
					}
					else if (m[j, i] == t.abysstile)
					{
						if (m[(64 + j - 1) % 64, i] == t.floortile || m[(64 + j - 1) % 64, i] == t.closedoortile || m[(64 + j - 1) % 64, i] == t.warptile)
						{
							m[j, i] = t.obstacletile;
						}
						if (m[(64 + j + 1) % 64, i] == t.floortile || m[(64 + j + 1) % 64, i] == t.closedoortile || m[(64 + j + 1) % 64, i] == t.warptile)
						{
							m[j, i] = t.obstacletile;
						}
						if (m[j, (64 + i - 1) % 64] == t.floortile || m[j, (64 + i - 1) % 64] == t.closedoortile || m[j, (64 + i - 1) % 64] == t.warptile)
						{
							m[j, i] = t.obstacletile;
						}
						if (m[j, (64 + i + 1) % 64] == t.floortile || m[j, (64 + i + 1) % 64] == t.closedoortile || m[j, (64 + i + 1) % 64] == t.warptile)
						{
							m[j, i] = t.obstacletile;
						}
						//if (j > 0)
						//{
						//	if (m[j - 1, i] == t.floortile || m[j - 1, i] == t.closedoortile)
						//	{
						//		m[j, i] = t.obstacletile;
						//	}
						//}
						//if (j < 63)
						//{
						//	if (m[j + 1, i] == t.floortile || m[j + 1, i] == t.closedoortile)
						//	{
						//		m[j, i] = t.obstacletile;
						//	}
						//}
						//if (i > 0)
						//{
						//	if (m[j, i - 1] == t.floortile || m[j, i - 1] == t.closedoortile)
						//	{
						//		m[j, i] = t.obstacletile;
						//	}
						//}
						//if (i < 63)
						//{
						//	if (m[j, i + 1] == t.floortile || m[j, i + 1] == t.closedoortile)
						//	{
						//		m[j, i] = t.obstacletile;
						//	}
						//}
					}
					else if (m[j, i] == t.roomtile)
					{
						if (j > 0)
						{
							if (m[j - 1, i] != t.roomtile && m[j - 1, i] != t.roomleft && m[j - 1, i] != t.roomright && m[j - 1, i] != t.roomupperleft && m[j - 1, i] != t.roomupper && m[j - 1, i] != t.roomupperright)
							{
								upper = true;
							}
						}
						if (j < 63)
						{
							if (m[j + 1, i] != t.roomtile) lower = true;
						}
						if (i > 0)
						{
							if (m[j, i - 1] != t.roomtile && m[j, i - 1] != t.roomupper && m[j, i - 1] != t.roomlower && m[j, i - 1] != t.roomleft && m[j, i - 1] != t.roomupperleft && m[j, i - 1] != t.roomlowerleft)
							{
								left = true;
							}
						}
						if (i < 63)
						{
							if (m[j, i + 1] != t.roomtile) right = true;
						}
						if (upper)
						{
							m[j, i] = t.roomupper;
							if (left) m[j, i] = t.roomupperleft;
							if (right) m[j, i] = t.roomupperright;
						}
						else if (lower)
						{
							m[j, i] = t.roomlower;
							if (left) m[j, i] = t.roomlowerleft;
							if (right) m[j, i] = t.roomlowerright;
						}
						else if (left)
						{
							m[j, i] = t.roomleft;
						}
						else if (right)
						{
							m[j, i] = t.roomright;
						}
					}
				}
			}
		}

		private bool Traversible(Map m, Tileset t, int x, int y, int w, int h, bool inside = false)
		{
			// To determine if placing a feature in a potential location preserves map
			// traversibility, this function traces around the perimeter of the feature.
			// While tracing, if it flips from solid to walkable and back more than twice,
			// it breaks traversibility and rejects it.
			// Don't ask me to explain the logic about why that works but trust me it does.
			bool result = true;
			List<byte> solids = new List<byte>(t.treasuredeck);
			solids.Add(t.walltile);
			solids.Add(t.leftwalltile);
			solids.Add(t.rightwalltile);
			solids.Add(t.wallupperleft);
			solids.Add(t.wallupperright);
			solids.Add(t.abysstile);
			solids.Add(t.roomleft);
			solids.Add(t.roomright);
			solids.Add(t.roomupper);
			solids.Add(t.roomupperleft);
			solids.Add(t.roomupperright);
			solids.Add(t.roomlowerleft);
			solids.Add(t.roomlowerright);
			if (!inside)
			{
				solids.Add(t.roomtile);
			}
			bool solid = solids.Contains(m[y - 1, x - 1]);
			int flips = 0;
			for (int i = x - 1; i <= x + w; i++)
			{
				if (solid != solids.Contains(m[y - 1, i]))
				{
					flips++;
					solid = !solid;
				}
			}
			for (int i = y - 1; i <= y + h; i++)
			{
				if (solid != solids.Contains(m[i, x + w]))
				{
					flips++;
					solid = !solid;
				}
			}
			for (int i = x + w; i >= x - 1; i--)
			{
				if (solid != solids.Contains(m[y + h, i]))
				{
					flips++;
					solid = !solid;
				}
			}
			for (int i = y + h; i >= y - 1; i--)
			{
				if (solid != solids.Contains(m[i, x - 1]))
				{
					flips++;
					solid = !solid;
				}
			}
			if (flips > 2) result = false;
			// Make sure it's not blocking the only access to a neighbouring chest.
			if (result && inside)
			{
				if (t.treasuredeck.Contains(m[y - 1, x]))
				{
					if (m[y - 2, x] != t.roomtile && m[y - 1, x - 1] != t.roomtile && m[y - 1, x + 1] != t.roomtile)
					{
						result = false;
					}
				}
				if (t.treasuredeck.Contains(m[y + 1, x]))
				{
					if (m[y + 2, x] != t.roomtile && m[y + 1, x - 1] != t.roomtile && m[y + 1, x + 1] != t.roomtile)
					{
						result = false;
					}
				}
				if (t.treasuredeck.Contains(m[y, x - 1]))
				{
					if (m[y, x - 2] != t.roomtile && m[y + 1, x - 1] != t.roomtile && m[y - 1, x - 1] != t.roomtile)
					{
						result = false;
					}
				}
				if (t.treasuredeck.Contains(m[y, x + 1]))
				{
					if (m[y, x + 2] != t.roomtile && m[y + 1, x + 1] != t.roomtile && m[y - 1, x + 1] != t.roomtile)
					{
						result = false;
					}
				}
			}
			return result;
		}

		private bool Legal(Map m, Tileset t, int x, int y, int w, int h, bool obstacle = false)
		{
			// Make sure the feature is not about to be placed out of bounds or on top
			// of something that shouldn't be erased.
			bool result = true;
			if (x < 1 || y < 1 || x + w > 62 || y + h > 62)
			{
				result = false;
			}
			else
			{
				for (int i = x; i <= x + w; i++)
				{
					for (int j = y; j <= y + h; j++)
					{
						if (obstacle)
						{
							if (m[j, i] == t.doortile || m[j, i] == t.closedoortile || m[j, i] == t.roomtile || m[j, i] == t.warptile)
							{
								result = false;
								break;
							}
						}
						else if (m[j, i] == t.warptile || m[j, i] == t.floortile)
						{
							result = false;
							break;
						}
					}
					if (!result) break;
				}
				if (result) if (obstacle) result = Traversible(m, t, x, y, w, h);
			}
			return result;
		}
		private bool CarveBox(Map m, Tileset t, int x, int y, int w, int h)
		{
			// This creates one of the "boxes" in the box style algorithm.
			bool result = Legal(m, t, x, y, w, h);
			if (result)
			{
				m.Fill((x, y), (w, h), t.floortile);
				for (int i = 0; i < w; i++)
				{
					m[y, x + i] = t.walltile;
					m[y + h - 1, x + i] = t.walltile;
				}
				for (int i = 0; i < h - 1; i++)
				{
					m[y + i, x] = t.leftwalltile;
					m[y + i, x + w - 1] = t.rightwalltile;
				}
				m[y, x] = t.wallupperleft;
				m[y, x + w - 1] = t.wallupperright;
			}
			return result;
		}

		private bool CarveRoom(Map m, Tileset t, int x, int y, int w, int h)
		{
			// This tries to create an "inner room".
			// While creating rooms, only the "center" room tiles are created; a separate
			// routine takes care of giving it sides and corners later.
			bool result = Legal(m, t, x, y, w, h, true);
			if (result)
			{
				m.Fill((x, y), (w, h - 1), t.roomtile);
				m.Fill((x, y + h - 1), (w, 1), t.walltile);
			}
			return result;
		}
		private void GenerateRooms(MT19337 rng, Map m, Tileset t)
		{
			// This attempts to create a variety of "inner rooms" to populate the floor.
			List<Candidate> candidates;
			Candidate c;
			int x;
			int y;
			int w;
			int h;
			int attempts = 40;
			for (int attempt = 0; attempt < attempts; attempt++)
			{
				candidates = new List<Candidate>();
				for (int i = 0; i < 63; i++)
				{
					for (int j = 0; j < 63; j++)
					{
						if (m[j, i] == t.floortile) candidates.Add(new Candidate(i, j));
					}
				}
				if (candidates.Count > 0)
				{
					c = candidates.SpliceRandom(rng);
					w = RollDice(rng, 3, 4);
					h = RollDice(rng, 3, 4) + 1;
					x = c.x - RollDice(rng, 1, w - 2);
					y = c.y - h;
					if (CarveRoom(m, t, x, y, w, h))
					{
						m[c.y - 1, c.x] = t.doortile;
						m[c.y, c.x] = t.closedoortile;
					}
				}
			}
		}

		private byte PlaceExit(MT19337 rng, Map m, Tileset t)
		{
			// This places a teleport to the next floor in a random place.
			List<Candidate> candidates = new List<Candidate>();
			Candidate c;
			byte exittile = t.teleportdeck.SpliceRandom(rng);
			for (int i = 0; i < 64; i++)
			{
				for (int j = 0; j < 64; j++)
				{
					if (exittile >= 0x80)
					{
						if (m[j, i] == t.roomtile)
						{
							if (Traversible(m, t, i, j, 1, 1, true)) candidates.Add(new Candidate(i, j));
						}
					}
					else
					{
						if (m[j, i] == t.floortile)
						{
							if (Traversible(m, t, i, j, 1, 1)) candidates.Add(new Candidate(i, j));
						}
					}
				}
			}
			if (candidates.Count() == 0) Console.WriteLine("Can't place exit");
			c = candidates.SpliceRandom(rng);
			exittile = (byte)(exittile % 0x80);
			m[c.y, c.x] = exittile;
			return exittile;
		}

		private void PlaceChaos(MT19337 rng, Map m, Tileset t)
		{
			// Puts the final boss somewhere in a room on the final floor.
			List<Candidate> candidates = new List<Candidate>();
			Candidate c;
			for (int i = 0; i < 64; i++)
			{
				for (int j = 0; j < 64; j++)
				{
					if (m[j, i] == t.roomtile)
					{
						// You have to be able to talk to Chaos so he needs a place next to him open.
						if (m[j - 1, i] == t.roomtile ||
							m[j + 1, i] == t.roomtile ||
							m[j, i - 1] == t.roomtile ||
							m[j, i + 1] == t.roomtile)
						{
							if (Traversible(m, t, i, j, 1, 1, true)) candidates.Add(new Candidate(i, j));
						}
					}
				}
			}
			c = candidates.SpliceRandom(rng);
			_rom.SetNpc((MapIndex)59, 0, (ObjectId)0x18, c.x, c.y, true, true);
			_rom.SetNpc((MapIndex)59, 1, (ObjectId)0x19, c.x, c.y, true, true);
			_rom.SetNpc((MapIndex)59, 2, (ObjectId)0x1A, c.x, c.y, true, true);
		}
		private void PlaceWarMech(MT19337 rng, Flags flags)
		{
			List<Candidate> candidates = new List<Candidate>();
			Candidate c;
			int warmechfloor = 0;
			bool warmechstationary = false;
			Map m;
			Tileset t;

			if (flags.WarMECHMode == WarMECHMode.Vanilla || flags.WarMECHMode == WarMECHMode.Unleashed)
			{
				WarMechFloor = 0;
				return;
			}
			if (flags.WarMECHMode == WarMECHMode.Required || flags.WarMECHMode == WarMECHMode.All)
			{
				warmechfloor = 59;
				warmechstationary = true;
				standardMaps[(MapIndex)warmechfloor].MapObjects.SetNpc(4, ObjectId.WarMECH, 0x0F, 0x16, false, warmechstationary);
			}
			// This needs to be second because the floor information needs to be passed on later
			if (flags.WarMECHMode == WarMECHMode.Patrolling || flags.WarMECHMode == WarMECHMode.All)
			{
				warmechfloor = RollDice(rng, 1, 6) + 45 + 7;
				warmechstationary = false;
				m = standardMaps[(MapIndex)warmechfloor].Map;
				t = tilesets[tilesetmappings[warmechfloor]];

				for (int i = 1; i < 63; i++)
				{
					for (int j = 1; j < 63; j++)
					{
						if (m[j, i] == t.floortile)
						{
							if (Traversible(m, t, i, j, 1, 1, false)) candidates.Add(new Candidate(i, j));
						}
					}
				}
			}
			
			WarMechFloor = warmechfloor;
			c = candidates.SpliceRandom(rng);
			standardMaps[(MapIndex)warmechfloor].MapObjects.SetNpc(4, ObjectId.WarMECH, c.x, c.y, false, warmechstationary);
		}
		private void PlaceBahamut(MT19337 rng, Flags flags)
		{
			// Puts Bahamut in a room somewhere and the TAIL in a random chest.
			// The floor for each is 4d6 + 6, rolled independently.
			// This means each could be as low as floor 10 or as high as floor 30, but with
			// weighting towards floor 20.
			List<Candidate> candidates = new List<Candidate>();
			Candidate c;
			bahamutfloor = RollDice(rng, 4, 6) + 6 + 7;
			tailfloor = RollDice(rng, 4, 6) + 6 + 7;
			switch (flags.TailBahamutMode)
			{
				case TailBahamutMode.TailFirst:
					if (bahamutfloor < tailfloor)
					{
						var temp = tailfloor;
						tailfloor = bahamutfloor;
						bahamutfloor = temp;
					}
					break;
				case TailBahamutMode.BahamutFirst:
					if (tailfloor < bahamutfloor)
					{
						var temp = tailfloor;
						tailfloor = bahamutfloor;
						bahamutfloor = temp;
					}
					break;
				case TailBahamutMode.SameFloor:
					tailfloor = bahamutfloor;
					break;
			}
			Map m = standardMaps[(MapIndex)bahamutfloor].Map;
			Tileset t = tilesets[tilesetmappings[bahamutfloor]];
			for (int i = 1; i < 63; i++)
			{
				for (int j = 1; j < 63; j++)
				{
					if (m[j, i] == t.roomtile)
					{
						// You have to be able to talk to Bahamut so he needs a place next to him open.
						if (m[j - 1, i] == t.roomtile ||
							m[j + 1, i] == t.roomtile ||
							m[j, i - 1] == t.roomtile ||
							m[j, i + 1] == t.roomtile)
						{
							if (Traversible(m, t, i, j, 1, 1, true)) candidates.Add(new Candidate(i, j));
						}
					}
				}
			}
			c = candidates.SpliceRandom(rng);
			standardMaps[(MapIndex)bahamutfloor].MapObjects.SetNpc(0, ObjectId.Bahamut, c.x, c.y, true, true);
		}
		private void GenerateMapBoxStyle(MT19337 rng, Map m, Tileset t)
		{
			// Draw the initial box containing the "back" staircase.
			int w = RollDice(rng, 3, 4);
			int h = RollDice(rng, 3, 4);
			CarveBox(m, t, 32 - w / 2, 32 - h / 2, w, h);
			m[32, 32] = t.warptile;

			int attempts = 120;
			for (int attempt = 0; attempt < attempts; attempt++)
			{
				// Determine which tiles are suitable for attaching a new box.
				List<Candidate> candidates = new List<Candidate>();
				Candidate c;
				for (int i = 1; i < 63; i++)
				{
					for (int j = 1; j < 63; j++)
					{
						if (m[j, i] == t.floortile)
						{
							if (m[j - 1, i] != t.floortile || m[j + 1, i] != t.floortile || m[j, i - 1] != t.floortile || m[j, i + 1] != t.floortile)
							{
								c = new Candidate(i, j);
								if (m[j - 1, i] != t.floortile) c.dirs.Add(northdir);
								if (m[j + 1, i] != t.floortile) c.dirs.Add(southdir);
								if (m[j, i - 1] != t.floortile) c.dirs.Add(westdir);
								if (m[j, i + 1] != t.floortile) c.dirs.Add(eastdir);
								candidates.Add(c);
							}
						}
					}
				}

				// Select a candidate at random and attach a box to it in a random legal direction
				c = candidates.SpliceRandom(rng);
				byte d = c.dirs.SpliceRandom(rng);
				w = RollDice(rng, 3, 4);
				h = RollDice(rng, 3, 4);
				switch (d)
				{
					case northdir:
						if (CarveBox(m, t, c.x - w / 2, c.y - h, w, h)) m[c.y - 1, c.x] = t.floortile;
						break;
					case southdir:
						if (CarveBox(m, t, c.x - w / 2, c.y + 1, w, h)) m[c.y + 1, c.x] = t.floortile;
						break;
					case westdir:
						if (CarveBox(m, t, c.x - w, c.y - h / 2, w, h)) m[c.y, c.x - 1] = t.floortile;
						break;
					case eastdir:
						if (CarveBox(m, t, c.x + 1, c.y - h / 2, w, h)) m[c.y, c.x + 1] = t.floortile;
						break;
				}

			}
			GenerateRooms(rng, m, t);
		}
		private void GenerateMapSnakeStyle(MT19337 rng, Map m, Tileset t)
		{
			var totalpushes = 100;
			var minheads = 2;
			var maxheads = 6;
			var newheadchance = 400;
			var killheadchance = 25;
			List<SnakeHead> heads = new List<SnakeHead>();
			List<SnakeHead> newheadlist;

			// We start with two "heads" for the snake that move independently.
			// They start facing in opposite directions.
			heads.Add(new SnakeHead(0x20, 0x20, RollDice(rng, 1, 8) - 1));
			heads.Add(new SnakeHead(0x20, 0x20, heads[0].facing + 4));
			m.Fill((0x20 - 1, 0x20 - 1), (3, 3), t.floortile);
			for (int p = 0; p < totalpushes; p++)
			{
				// Each iteration, we create a new list of heads based on whether the existing
				// heads split and/or got cut off.
				newheadlist = new List<SnakeHead>();
				for (int i = 0; i < heads.Count(); i++)
				{
					// Move the head one tile in the direction it's facing.
					heads[i].Step();
					// If it's out of bounds, we don't draw anything and it doesn't survive into
					// the next iteration.
					if (heads[i].x > 1 && heads[i].y > 1 && heads[i].x < 62 && heads[i].y < 62)
					{
						// Draw a 3x3 box of floor at the head's current location.
						m.Fill((heads[i].x - 1, heads[i].y - 1), (3, 3), t.floortile);
						if (heads.Count() < maxheads && RollDice(rng, 1, newheadchance) == 1)
						{
							// Split the head into two, each facing right angles to each other.
							newheadlist.Add(new SnakeHead(heads[i].x, heads[i].y, heads[i].facing + 2));
							heads[i].Rotate(-2);
						}
						else
						{
							// The head might rotate up to two steps in either direction, weighted
							// towards staying in the same facing.
							heads[i].Rotate(RollDice(rng, 2, 3) - 4);
						}
						if (!(heads.Count() > minheads && RollDice(rng, 1, killheadchance) == 1))
						{
							// If the head wasn't killed, it survives into the next iteration.
							newheadlist.Add(heads[i]);
						}
					}
				}
				// Copy the new list over.
				heads = new List<SnakeHead>();
				foreach (SnakeHead s in newheadlist)
				{
					heads.Add(s);
				}
			}
			// The process above erases the original warp tile so let's put it back ^_^;
			m[0x20, 0x20] = t.warptile;
			GenerateRooms(rng, m, t);
		}
		private void GenerateMapGridStyle(MT19337 rng, Map m, Tileset t)
		{
			var cellsize = RollDice(rng, 1, 9) + 7; // The size of each "cell" in the grid
			bool canwrap = (cellsize == 8 || cellsize == 16);
			int gridsize = (int)Math.Floor((double)64 / cellsize);
			int totalactivecells = 1;
			double targetcells = (gridsize * gridsize * 1) / 4;
			int centercell = (int)Math.Floor((double)0x20 / cellsize);
			bool[,] active = new bool[gridsize, gridsize];
			int celltype;
			bool maderoom = false;
			var boxtile = t.obstacletile;
			// Sky looks weird with the usual obstacle tile
			if (t.Equals(tilesets[6])) boxtile = t.walltile;

			// The floor is divided into evenly sized "cells".
			// Each cell can be "active" or "inactive".
			// All the cells start inactive except for the one containing the warp tile.
			for (int i = 0; i < gridsize; i++)
			{
				for (int j = 0; j < gridsize; j++)
				{
					active[i, j] = false;
				}
			}
			active[centercell, centercell] = true;

			// Then we expand the floor by randomly activating cells adjacent to the
			// current active area. We do this until we reach a certain percentage of
			// the floor is active.
			while (totalactivecells < targetcells)
			{
				var cx = RollDice(rng, 1, gridsize) - 1;
				var cy = RollDice(rng, 1, gridsize) - 1;
				bool valid = false;
				if (!active[cx, cy])
				{
					if (cx > 0) if (active[cx - 1, cy]) valid = true;
					if (cx < gridsize - 1) if (active[cx + 1, cy]) valid = true;
					if (cy > 0) if (active[cx, cy - 1]) valid = true;
					if (cy < gridsize - 1) if (active[cx, cy + 1]) valid = true;
					if (cx == 0 && canwrap) if (active[gridsize - 1, cy]) valid = true;
					if (cx == gridsize - 1 && canwrap) if (active[0, cy]) valid = true;
					if (cy == 0 && canwrap) if (active[cx, gridsize - 1]) valid = true;
					if (cy == gridsize - 1 && canwrap) if (active[cx, 0]) valid = true;
				}
				if (valid)
				{
					active[cx, cy] = true;
					totalactivecells++;
				}
			}

			// Then for each active cell, we fill it with floor and possibly some features.
			for (int i = 0; i < gridsize; i++)
			{
				for (int j = 0; j < gridsize; j++)
				{
					if (active[i, j])
					{
						// We count back down the number of active cells so that we can
						// try to guarantee at least one room.
						totalactivecells--;
						m.Fill((cellsize * i, cellsize * j), (cellsize, cellsize), t.floortile);
						if (i == centercell && j == centercell)
						{
							// The cell containing the warp tile can't have features
							// in case they clash with the warp tile.
							celltype = 1;
						}
						else
						{
							celltype = RollDice(rng, 1, 6);
							if (totalactivecells == 0 && !maderoom)
							{
								// If we are on the last active cell and still haven't
								// create a room, let's create one just in case the exit
								// dropper tries to place an in-room exit tile.
								celltype = 6;
							}
						}
						switch (celltype)
						{
							case 1:
								// Blank floor
								break;
							case 2:
								// A pattern of obstacle tiles
								m.Fill((cellsize * i + 1, cellsize * j + 1), (cellsize - 2, 1), boxtile);
								m.Fill((cellsize * i + 1, cellsize * j + cellsize - 2), (cellsize - 2, 1), boxtile);
								m.Fill((cellsize * i + 1, cellsize * j + 3), (1, cellsize - 6), boxtile);
								m.Fill((cellsize * i + cellsize - 2, cellsize * j + 3), (1, cellsize - 6), boxtile);
								break;
							case 3:
								// A hollow square
								m.Fill((cellsize * i + 1, cellsize * j + 1), (cellsize - 2, cellsize - 2), boxtile);
								m.Fill((cellsize * i + 2, cellsize * j + 2), (cellsize - 4, cellsize - 4), t.abysstile);
								break;
							default:
								// A room
								if (CarveRoom(m, t, cellsize * i + 1, cellsize * j + 1, cellsize - 2, cellsize - 2))
								{
									m[cellsize * j + cellsize - 2, cellsize * i + cellsize / 2] = t.doortile;
									m[cellsize * j + cellsize - 1, cellsize * i + cellsize / 2] = t.closedoortile;
									maderoom = true;
								}
								break;
						}
					}
				}
			}
			m[0x20, 0x20] = t.warptile;
		}

	}
}
