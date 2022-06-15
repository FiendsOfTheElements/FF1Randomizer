﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FF1Lib.Procgen;
using RomUtilities;
using static FF1Lib.FF1Text;
using FF1Lib.Sanity;
using System.Threading.Tasks;

namespace FF1Lib
{
	public enum MapId : byte
	{
		Coneria = 0,
		Pravoka,
		Elfland,
		Melmond,
		CrescentLake,
		Gaia,
		Onrac,
		Lefein,
		ConeriaCastle1F,
		ElflandCastle,
		NorthwestCastle,
		CastleOfOrdeals1F,
		TempleOfFiends,
		EarthCaveB1,
		GurguVolcanoB1,
		IceCaveB1,
		Cardia,
		BahamutsRoomB1,
		Waterfall,
		DwarfCave,
		MatoyasCave,
		SardasCave,
		MarshCaveB1,
		MirageTower1F,
		ConeriaCastle2F,
		CastleOfOrdeals2F,
		CastleOfOrdeals3F,
		MarshCaveB2,
		MarshCaveB3,
		EarthCaveB2,
		EarthCaveB3,
		EarthCaveB4,
		EarthCaveB5,
		GurguVolcanoB2,
		GurguVolcanoB3,
		GurguVolcanoB4,
		GurguVolcanoB5,
		IceCaveB2,
		IceCaveB3,
		BahamutsRoomB2,
		MirageTower2F,
		MirageTower3F,
		SeaShrineB5,
		SeaShrineB4,
		SeaShrineB3,
		SeaShrineB2,
		SeaShrineB1,
		SkyPalace1F,
		SkyPalace2F,
		SkyPalace3F,
		SkyPalace4F,
		SkyPalace5F,
		TempleOfFiendsRevisited1F,
		TempleOfFiendsRevisited2F,
		TempleOfFiendsRevisited3F,
		TempleOfFiendsRevisitedEarth,
		TempleOfFiendsRevisitedFire,
		TempleOfFiendsRevisitedWater,
		TempleOfFiendsRevisitedAir,
		TempleOfFiendsRevisitedChaos,
		TitansTunnel
	}

	public enum Tile
	{
		RoomBackLeft = 0x00,
		RoomBackCenter = 0x01,
		RoomBackRight = 0x02,
		RoomLeft = 0x03,
		RoomCenter = 0x04,
		RoomRight = 0x05,
		RoomFrontLeft = 0x06,
		RoomFrontCenter = 0x07,
		RoomFrontRight = 0x08,
		Ladder = 0x09,
		LadderHole = 0x0A,
		WarpUp = 0x18,
		EarthCaveInside = 0x2E,
		InsideWall = 0x30,
		FloorSafe = 0x31,
		HallwayLeft = 0x32,
		HallwayRight = 0x33,
		WallLeft = 0x34,
		WallRight = 0x35,
		Impassable = 0x3D,
		Door = 0x36,
		EarthCaveOOB = 0x38,
		Doorway = 0x3A,
		DoorLocked = 0x3B,
		Lava = 0x39,
		EarthCaveRockA = 0x3E,
		EarthCaveRockB = 0x3F,
		MarshCaveOOB = 0x3F,
		EarthCaveRandomEncounters = 0x41,
		WaterfallInside = 0x46,
		WaterfallSpikeTile = 0x48,
		WaterfallRandomEncounters = 0x49,
		PortalWarp = 0x40,
		ToFRNoEncounter = 0x31,
		ToFREncounter = 0x5C,
		// Begin Town Tiles
		TownGrass = 0x00,
		TownGrassShadow = 0x01,
		TownGrassShadowDiagonal = 0x02,
		TownHorizontalWallLeft = 0x03,
		TownHorizontalWallCenter = 0x04,
		TownHorizontalWallRight = 0x05,
		TownHorizontalWallOffsetLeft = 0x06,
		TownHorizontalWallOffsetCenter = 0x07,
		TownHorizontalWallOffsetRight = 0x08,
		TownHorizontalWallOffsetRightShadow = 0x09,
		TownVerticalWallRight = 0x0A,
		TownVerticalWallLeftShadowTopCorner = 0x0B,
		TownVerticalWallLeftShadow = 0x0C,
		TownVerticalWallLeftBottomCorner = 0x0D,
		TownTree = 0x0E,
		TownBushes = 0x0F,
		TownStonePath = 0x10,
		TownStonePathRoundedTL = 0x11,
		TownStonePathRoundedTR = 0x12,
		TownStonePathRoundedBL = 0x13,
		TownStonePathRoundedBR = 0x14,
		TownStonePathShadowCorner = 0x15,
		TownStonePathShadowEdge = 0x16,
		TownBuildingRoofTopLeftEdge = 0x17,
		TownBuildingRoof = 0x18,
		TownBuildingRoofTopRightEdge = 0x19, // may be identical to 0x17
		TownSignClinic = 0x1A,
		TownBuildingRoofPitched = 0x1B,
		TownBuildingWindow = 0x1C,
		TownBuildingWindowDouble = 0x1D,
		TownDoor1 = 0x1E, // weapon shop, coneria
		TownDoor2 = 0x1F, // weapon shop, pravoka
		TownSignItem = 0x20,
		TownSignWeapon = 0x21,
		TownSignArmor = 0x22,
		TownSignWhiteMagic = 0x23,
		TownSignBlackMagic = 0x24,
		TownSignInn = 0x25,
		TownDoor3 = 0x26, // weapon shop, elfland
		TownWaterway = 0x27,
		TownWaterwayShadow = 0x28,
		TownWaterwayShadowDiagonal = 0x29,
		TownHorizonalBridge = 0x2A,
		TownVerticalBridge = 0x2B,
		TownBoardwalk = 0x2C, // unused?
		TownDoor4 = 0x2D, // weapon shop, melmond
		TownFountain = 0x2E,
		TownTomb1 = 0x2F, // ordinary tomb
		TownFence = 0x30,
		TownWell = 0x31,
		TownSand = 0x32,
		TownFlowers = 0x33,
		TownPalmTrees = 0x34, // unused?
		TownBoardwalk2 = 0x35, // unused? // may be identical to 0x2C
		TownDoor5 = 0x36, // weapon shop, crescent lake
		TownDoorOpen = 0x37,
		TownStonePath2 = 0x38, // may be identical to 0x10
		TownGrass2 = 0x39, // may be identical to 0x00
		TownVerticalBridge2 = 0x3A, // may be identical to 0x2B
		TownWaterway2 = 0x3B, // may be identical to 0x27
		TownSand2 = 0x3C, // may be identical to 0x32
		TownFlowers2 = 0x3D, // may be identical to 0x33
		TownDoor6 = 0x3E, // weapon shop, gaia
		TownDoor7 = 0x3F, // weapon shop, empty #1
		TownDoor8 = 0x40, // weapon shop, empty #2
		TownDoor9 = 0x41, // weapon shop, empty #3
		TownDoor10 = 0x42, // armor shop, empty #1
		TownDoor11 = 0x43, // armor shop, coneria
		TownDoor12 = 0x44, // armor shop, pravoka
		TownDoor13 = 0x45, // armor shop, elfland
		TownDoor14 = 0x46, // armor shop, melmond
		TownGrassWarp = 0x47,
		TownSubmarineWarp = 0x48,
		TownDoor15 = 0x49, // armor shop, crescent lake
		TownDoor16 = 0x4A, // armor shop, gaia
		TownDoor17 = 0x4B, // armor shop, empty #2
		TownDoor18 = 0x4C, // armor shop, empty #3
		TownDoor19 = 0x4D, // armor shop, empty #4
		TownDoor20 = 0x4E, // armor shop, empty #5
		TownDoor21 = 0x4F, // white magic shop, L1 coneria
		TownDoor22 = 0x50, // white magic shop, L2 pravoka
		TownDoor23 = 0x51, // white magic shop, L3 elfland
		TownDoor24 = 0x52, // white magic shop, L5 melmond
		TownDoor25 = 0x53, // white magic shop, L6 crescent lake
		TownDoor26 = 0x54, // white magic shop, L4 elfland
		TownDoor27 = 0x55, // white magic shop, L7 gaia
		TownDoor28 = 0x56, // white magic shop, L8 gaia
		TownDoor29 = 0x57, // white magic shop, L7 onrac
		TownDoor30 = 0x58, // white magic shop, L8 lefein
		TownDoor31 = 0x59, // black magic shop, L1 coneria
		TownDoor32 = 0x5A, // black magic shop, L2 pravoka
		TownDoor33 = 0x5B, // black magic shop, L3 elfland
		TownDoor34 = 0x5C, // black magic shop, L5 melmond
		TownDoor35 = 0x5D, // black magic shop, L6 crescent lake
		TownDoor36 = 0x5E, // black magic shop, L4 elfland
		TownDoor37 = 0x5F, // black magic shop, L7 gaia
		TownDoor38 = 0x60, // black magic shop, L8 gaia
		TownDoor39 = 0x61, // black magic shop, L7 onrac
		TownDoor40 = 0x62, // black magic shop, L8 lefein
		TownDoor41 = 0x63, // clinic, coneria
		TownDoor42 = 0x64, // clinic, elfland
		TownDoor43 = 0x65, // clinic, crescent lake
		TownDoor44 = 0x66, // clinic, gaia
		TownDoor45 = 0x67, // clinic, onrac
		TownDoor46 = 0x68, // clinic, pravoka
		TownDoor47 = 0x69, // clinic, unused #1
		TownDoor48 = 0x6A, // clinic, unused #2
		TownDoor49 = 0x6B, // clinic, unused #3
		TownDoor50 = 0x6C, // clinic, unused #4
		TownDoor51 = 0x6D, // inn, coneria
		TownDoor52 = 0x6E, // inn, pravoka
		TownDoor53 = 0x6F, // inn, elfland
		TownDoor54 = 0x70, // inn, melmond
		TownDoor55 = 0x71, // inn, crescent lake
		TownDoor56 = 0x72, // inn, gaia
		TownDoor57 = 0x73, // inn, onrac
		TownDoor58 = 0x74, // inn, unused #1
		TownDoor59 = 0x75, // inn, unused #2
		TownDoor60 = 0x76, // inn, unused #3
		TownDoor61 = 0x77, // item shop, coneria
		TownDoor62 = 0x78, // item shop, pravoka
		TownDoor63 = 0x79, // item shop, elfland
		TownDoor64 = 0x7A, // item shop, crescent lake
		TownDoor65 = 0x7B, // item shop, gaia
		TownDoor66 = 0x7C, // item shop, onrac
		TownDoor67 = 0x7D, // item shop, empty #1
		TownDoor68 = 0x7E, // item shop, empty #2
		TownTomb2 = 0x7F, // easter egg tomb

		// Cardia tiles
		CardiaFloor = 0x04,
		CardiaCandles = 0x0D,
		CardiaChest1 = 0x68,
		CardiaChest2 = 0x69,
		CardiaChest3 = 0x6A,
		CardiaChest4 = 0x6B,
		CardiaChest5 = 0x6C,
		CardiaChest6 = 0x6D,
		CardiaChest7 = 0x6E,
		CardiaChest8 = 0x6F,
		CardiaChest9 = 0x70,
		CardiaChest10 = 0x71,
		CardiaChest11 = 0x72,
		CardiaChest12 = 0x73,
		CardiaChest13 = 0x74,
		CardiaEncounters = 0x49,
	}

	public enum WarMECHMode
	{
		Vanilla,
		Patrolling,
		Required,
		Unleashed
	}

	public enum SkyCastle4FMazeMode
	{
		Normal,
		Teleporters,
		Maze
	}

	public struct NPC
	{
		public ObjectId ObjectId;
		public int Index;
		public (int x, int y) Coord;
		public bool InRoom;
		public bool Stationary;
	}

	public partial class FF1Rom : NesRom
	{
		public const int MapPointerOffset = 0x10000;
		public const int MapPointerSize = 2;
		public const int MapCount = 61;
		public const int MapDataOffset = 0x10080;

		public const int TeleportOffset = 0x3F000;
		public const int TeleportCount = 256;

		public const int TilesetDataOffset = 0x00800;
		public const int TilesetDataSize = 2;
		public const int TilesetDataCount = 128;
		public const int TilesetCount = 8;

		public const int MapObjJumpTableOffset = 0x390D3;
		public const int JumpTablePointerSize = 2;
		public const int MapObjOffset = 0x395D5;
		public const int MapObjGfxOffset = 0x02E00;
		public const int MapObjSize = 4;
		public const int MapObjCount = 0xD0;

		public const int FirstBossEncounterIndex = 0x73;
		public const int LastBossEncounterIndex = 0x7F;

		const ushort TalkFight = 0x94AA;

		bool IsBattleTile(Blob tuple) => tuple[0] == 0x0A;
		bool IsRandomBattleTile(Blob tuple) => IsBattleTile(tuple) && (tuple[1] & 0x80) != 0x00;
		bool IsNonBossTrapTile(Blob tuple) => IsBattleTile(tuple) && tuple[1] > 0 && tuple[1] < FirstBossEncounterIndex;
		bool IsNonBossTrapTileEx(Blob tuple) => IsBattleTile(tuple) && ((tuple[1] > 0 && tuple[1] < FirstBossEncounterIndex) || tuple[1] > LastBossEncounterIndex);
		bool IsBossTrapTile(Blob tuple) => IsBattleTile(tuple) && tuple[1] <= LastBossEncounterIndex && tuple[1] >= FirstBossEncounterIndex;

		public void RemoveTrapTiles(bool extendedtraptiles)
		{
			// This must be called before shuffle trap tiles since it uses the vanilla format for random encounters
			var tilesets = Get(TilesetDataOffset, TilesetDataCount * TilesetDataSize * TilesetCount).Chunk(TilesetDataSize).ToList();
			tilesets.ForEach(tile =>
			{

				if (extendedtraptiles ? IsNonBossTrapTileEx(tile) : IsNonBossTrapTile(tile))
				{
					tile[1] = (byte)(extendedtraptiles ? 0x00 : 0x80);
				}
			});
			Put(TilesetDataOffset, tilesets.SelectMany(tileset => tileset.ToBytes()).ToArray());
		}

		public void ShuffleTrapTiles(MT19337 rng, bool randomize, bool fightBahamut)
		{
			// This is magic BNE code that enables formation 1 trap tiles but we have to change
			// all the 0x0A 0x80 into 0x0A 0x00 and use 0x00 for random encounters instead of 0x80.
			Data[0x7CDC5] = 0xD0;
			var tilesets = Get(TilesetDataOffset, TilesetDataCount * TilesetDataSize * TilesetCount).Chunk(TilesetDataSize).ToList();

			List<byte> encounters;
			if (randomize)
			{
				encounters = Enumerable.Range(128, FirstBossEncounterIndex).Select(value => (byte)value).ToList();
				encounters.Add(0xFF); // IronGOL
				if (fightBahamut)
				{
					encounters.Remove(0x80 + 0x71); // ANKYLO (used for Bahamut)
				}
			}
			else
			{
				var traps = tilesets.Where(IsNonBossTrapTile).ToList();
				encounters = traps.Select(trap => trap[1]).ToList();
			}

			tilesets.ForEach(tile =>
			{
				if (IsNonBossTrapTile(tile))
				{
					tile[1] = encounters.SpliceRandom(rng);
				}
				else if (IsRandomBattleTile(tile))
				{
					tile[1] = 0x00;
				}
			});

			Put(TilesetDataOffset, tilesets.SelectMany(tileset => tileset.ToBytes()).ToArray());
		}

		private struct OrdealsRoom
		{
			public byte Entrance;
			public List<(int, int)> Teleporters;
		}

		public void ShuffleOrdeals(MT19337 rng, List<Map> maps)
		{
			// Here are all the teleporter rooms except the one you start in.
			// The last one is not normally accessible in the game.  We'll rewrite the teleporter located in that
			// room to go TO that room, and then shuffle it into one of the other locations so that you can go
			// through that room.
			var rooms = new List<OrdealsRoom>
			{
				new OrdealsRoom
				{
					Entrance = 0x4C,
					Teleporters = new List<(int, int)> { (0x0F, 0x09) }
				},
				new OrdealsRoom
				{
					Entrance = 0x4D,
					Teleporters = new List<(int, int)> { (0x08, 0x08), (0x08, 0x0A) }
				},
				new OrdealsRoom
				{
					Entrance = 0x4F,
					Teleporters = new List<(int, int)> { (0x09, 0x0F) }
				},
				new OrdealsRoom
				{
					Entrance = 0x50,
					Teleporters = new List<(int, int)> { (0x04, 0x14), (0x04, 0x16) }
				},
				new OrdealsRoom
				{
					Entrance = 0x51,
					Teleporters = new List<(int, int)> { (0x04, 0x12) }
				},
				new OrdealsRoom
				{
					Entrance = 0x52,
					Teleporters = new List<(int, int)> { (0x01, 0x01), (0x03, 0x02) }
				},
				new OrdealsRoom
				{
					Entrance = 0x53,
					Teleporters = new List<(int, int)> { (0x06, 0x08), (0x14, 0x0B), (0x14, 0x0D), (0x12, 0x10) }
				},
				new OrdealsRoom
				{
					Entrance = 0x54, // Normally inaccessible
					Teleporters = new List<(int, int)> { (0x08, 0x12) }
				}
			};

			// Shuffle the rooms, make sure the 4-pad room isn't the first room.
			do
			{
				rooms.Shuffle(rng);
			} while (rooms[0].Teleporters.Count == 4);

			// The room you start in always remains the same, but we need to adjust where it teleports you to.
			rooms.Insert(0, new OrdealsRoom
			{
				Entrance = 0x4E,
				Teleporters = new List<(int, int)> { (0x10, 0x0F) }
			});

			// First we choose a teleporter to link to the next room.
			byte exit = 0x55;
			var map = maps[(byte)MapId.CastleOfOrdeals2F];
			for (int i = 0; i < rooms.Count; i++)
			{
				int teleporter = rng.Between(0, rooms[i].Teleporters.Count - 1);
				var (x, y) = rooms[i].Teleporters[teleporter];
				rooms[i].Teleporters.RemoveAt(teleporter);

				if (i < rooms.Count - 1)
				{
					map[y, x] = rooms[i + 1].Entrance;
				}
				else
				{
					map[y, x] = exit;
				}
			}

			// Now we make all the other teleporters go to a random previous room, or the same room.
			for (int i = 0; i < rooms.Count; i++)
			{
				if (rooms[i].Teleporters.Count == 1)
				{
					var (x, y) = rooms[i].Teleporters[0];
					var backDestination = rng.Between(0, i);
					map[y, x] = rooms[backDestination].Entrance;
				}
				// Special rules for the 4-pad room: two teleporters go back, one goes to a totally random room,
				// and none of the four teleporters can be the same.
				else if (rooms[i].Teleporters.Count == 3)
				{
					rooms[i].Teleporters.Shuffle(rng);

					var (x, y) = rooms[i].Teleporters[0];
					var backDestination = rng.Between(0, i);
					map[y, x] = rooms[backDestination].Entrance;

					(x, y) = rooms[i].Teleporters[1];
					var otherBackDestination = backDestination;
					while (otherBackDestination == backDestination)
					{
						otherBackDestination = rng.Between(0, i);
					}
					map[y, x] = rooms[otherBackDestination].Entrance;

					(x, y) = rooms[i].Teleporters[2];
					var randomDestination = backDestination;
					while (randomDestination == backDestination || randomDestination == otherBackDestination || randomDestination == i + 1)
					{
						randomDestination = rng.Between(0, rooms.Count - 1);
					}
					map[y, x] = rooms[randomDestination].Entrance;
				}
			}

			// Now let's rewrite that teleporter.  The X coordinates are packed together, followed by the Y coordinates,
			// followed by the map indices.  Maybe we'll make a data structure for that someday soon.
			const byte LostTeleportIndex = 0x3C;
			Put(TeleportOffset + LostTeleportIndex, new byte[] { 0x10 });
			Put(TeleportOffset + TeleportCount + LostTeleportIndex, new byte[] { 0x12 });
		}

		public void DoSkyCastle4FMaze(MT19337 rng, List<Map> maps)
		{
			var map = maps[(int)MapId.SkyPalace4F];
			var walls = Maze.DoSkyCastle4FMaze(rng);

			// We make two passes and do vertical walls, then horizontal walls, because it works
			// out nicely if we just let the horizontal wall tiles overwrite the vertical ones
			// at the corners.
			foreach (var wall in walls)
			{
				if (wall.one.Item.y == wall.two.Item.y) // vertical wall
				{
					int x;
					int y = 8 * wall.one.Item.y;
					byte tile;

					// The first item will always have the lower coordinate, unless it's a wraparound.
					if (wall.one.Item.x % 2 == 0)
					{
						x = 8 * wall.one.Item.x + 7;
						tile = 0x33;
					}
					else
					{
						x = (8 * wall.one.Item.x + 8) % 64;
						tile = 0x32;
					}

					for (int i = 0; i < 8; i++)
					{
						map[y + i, x] = tile;
					}
				}
			}

			foreach (var wall in walls)
			{
				if (wall.one.Item.x == wall.two.Item.x) // horizontal wall
				{
					int x = 8 * wall.one.Item.x;
					int y = (8 * wall.one.Item.y + 8) % 64;

					map[y, x] = 0x34;
					for (int i = 1; i < 7; i++)
						map[y, x + i] = 0x30;
					map[y, x + 7] = 0x35;
				}
			}

			ShuffleSkyCastle4F(rng, maps);
		}

		public void ShuffleSkyCastle4F(MT19337 rng, List<Map> maps)
		{
			// Don't shuffle the return teleporter as Floor and Entrance shuffle might want to edit it.
			var map = maps[(byte)MapId.SkyPalace4F];
			var upTeleporter = (x: 0x23, y: 0x23);
			var dest = GetSkyCastleFloorTile(rng, map);
			SwapTiles(map, upTeleporter, dest);
		}

		private (int x, int y) GetSkyCastleFloorTile(MT19337 rng, Map map)
		{
			int x, y;
			do
			{
				x = rng.Between(0, 63);
				y = rng.Between(0, 63);

			} while (map[y, x] != 0x4B);

			return (x, y);
		}

		private void SwapTiles(Map map, (int x, int y) src, (int x, int y) dest)
		{
			byte temp = map[dest.y, dest.x];
			map[dest.y, dest.x] = map[src.y, src.x];
			map[src.y, src.x] = temp;
		}

		public void EnableEarlyOrdeals()
		{
			// Remove CROWN requirement for Ordeals.
			const int OrdealsTileset = 1;
			var ordealsTilesetOffset = TilesetDataOffset + OrdealsTileset * TilesetDataCount * TilesetDataSize;
			var ordealsTilesetData = Get(ordealsTilesetOffset, TilesetDataCount * TilesetDataSize).ToUShorts();

			// The 4 masked-out bits are special flags for a tile.  We wipe the flags for the two throne teleportation tiles,
			// which normally indicate that the CROWN is required to use them.
			const ushort specialMask = 0b11111111_11100001;
			ordealsTilesetData[0x61] &= specialMask;
			ordealsTilesetData[0x62] &= specialMask;

			Put(ordealsTilesetOffset, Blob.FromUShorts(ordealsTilesetData));
		}

		public void EnableTitansTrove(List<Map> maps)
		{
			MoveNpc(MapId.TitansTunnel, 0, 4, 8, inRoom: false, stationary: true); // Move the Titan
			maps[(byte)MapId.TitansTunnel][9, 3] = 0x3F; // Block the tunnel
		}

		public void EnableGaiaShortcut(List<Map> maps)
		{
			// Place grass tiles, creating shortcut
			for (int x = 41; x <= 43; x++)
			{
				for (int y = 36; y <= 47; y++)
				{
					maps[(byte)MapId.Gaia][y, x] = (byte)Tile.TownGrass;
				}
			}
			for (int y = 44; y <= 47; y++)
			{
				maps[(byte)MapId.Gaia][y, 40] = (byte)Tile.TownGrass;
			}

			// Restore building shadow
			maps[(byte)MapId.Gaia][39, 41] = (byte)Tile.TownGrassShadow;
			maps[(byte)MapId.Gaia][40, 41] = (byte)Tile.TownGrassShadowDiagonal;

			// In the spirit of Gaia's original landscaping, add some trees
			// (Without these it looked like a too perfectly trimmed pathway)
			for (int y = 41; y <= 44; y++)
			{
				maps[(byte)MapId.Gaia][y, 43] = (byte)Tile.TownBushes;
			}
			maps[(byte)MapId.Gaia][36, 41] = (byte)Tile.TownTree;
			maps[(byte)MapId.Gaia][38, 43] = (byte)Tile.TownTree;
			maps[(byte)MapId.Gaia][45, 43] = (byte)Tile.TownTree;
			maps[(byte)MapId.Gaia][45, 40] = (byte)Tile.TownBushes;
			maps[(byte)MapId.Gaia][46, 40] = (byte)Tile.TownTree;
		}

		public void MoveGaiaItemShop(List<Map> maps, MT19337 rng)
		{
			// Top left corner of original Item Shop Location
			const int yItemShop = 33;
			const int xItemShop = 10;

			// How much to move it
			int yDisplacement = 11;
			int xDisplacement = 33;

			// place a tree in the old Item Shop grid first, it often looks better in the moved location
			maps[(byte)MapId.Gaia][yItemShop, xItemShop + 4] = (byte)Tile.TownTree;

			// something to put in place of the old item shop
			var replacementTileOptions = new List<Tile> { Tile.TownWaterway, Tile.TownFlowers, Tile.TownGrass, Tile.TownStonePath, Tile.TownSand, Tile.TownTomb1, Tile.TownTree };
			Tile replacementTile = replacementTileOptions.PickRandom(rng);

			for (int y = yItemShop; y <= yItemShop + 3; y++)
			{
				for (int x = xItemShop; x <= xItemShop + 4; x++)
				{
					// lift and shift
					maps[(byte)MapId.Gaia][y + yDisplacement, x + xDisplacement] = maps[(byte)MapId.Gaia][y, x];
					maps[(byte)MapId.Gaia][y, x] = (byte)replacementTile;
				}
			}
		}

		public void EnableLefeinSuperStore(List<Map> maps)
		{
			// define
			List<Blob> superStore = new List<Blob>
			{
				//              1 2 3 4 WM 5 6 7 8  XX  1 2 3 4 BM 5 6 7 8
				Blob.FromHex("171717171717171717171700171717171717171717171700"),
				Blob.FromHex("181818181823181818181801181818181824181818181801"),
				Blob.FromHex("1D4F5051541D525357581D021D595A5B5E1D5C5D61621D02"),
			};
			// place
			maps[(int)MapId.Lefein].Put((0x28, 0x01), superStore.ToArray());
			// cleanup (removes single tree)
			maps[(int)MapId.Lefein][0x00, 0x34] = (byte)Tile.TownGrass;
		}

		public void EnableLefeinShops(List<Map> maps)
		{
			var lefein = maps[(byte)MapId.Lefein];
			lefein[0x05, 0x11] = 0x25; // Inn Sign
			lefein[0x06, 0x11] = 0x71; // Crescent Lake Inn
			lefein[0x05, 0x15] = 0x1A; // Clinic Sign
			lefein[0x06, 0x15] = 0x65; // Crescent Lake Clinic
		}

		public void EnableMelmondClinic(List<Map> maps)
		{
			var melmond = maps[(byte)MapId.Melmond];
			melmond[0x09, 0x1A] = 0x17; // Roof top
			melmond[0x09, 0x1B] = 0x17;
			melmond[0x09, 0x1C] = 0x17;
			melmond[0x0A, 0x1A] = 0x18; // Roof middle
			melmond[0x0A, 0x1B] = 0x1A; // Clinic sign
			melmond[0x0A, 0x1C] = 0x18;
			melmond[0x0A, 0x1D] = 0x01; // Full shadow
			melmond[0x0B, 0x1A] = 0x1D; // Building front, two windows
			melmond[0x0B, 0x1B] = 0x65; // Crescent Lake clinic
			melmond[0x0B, 0x1C] = 0x1D;
			melmond[0x0B, 0x1D] = 0x02; // Corner shadow
		}

		public void EnableChaosFloorEncounters(List<Map> maps)
		{
			// Replace floor tiles with encounter tiles
			for (int x = 0; x < 32; x++)
			{
				for (int y = 0; y < 32; y++)
				{
					if (maps[(byte)MapId.TempleOfFiendsRevisitedChaos][x, y] == (byte)Tile.ToFRNoEncounter)
					{
						maps[(byte)MapId.TempleOfFiendsRevisitedChaos][x, y] = (byte)Tile.ToFREncounter;
					}
				}
			}

			// Change base rate for encounters
			Put(ThreatLevelsOffset + 60, Blob.FromHex("0D"));
			// threat level reference for comparison: 08 = most dungeon floors; 18 = sky bridge; 09 = ToFR earth; 0A = ToFR fire; 0B = ToFR water; 0C = ToFR air; 01 = ToFR chaos
		}

		public void EnableToFRExit(List<Map> maps)
		{
			// add warp portal to ToFR 1st floor
			maps[(byte)MapId.TempleOfFiendsRevisited1F][17, 20] = (byte)Tile.PortalWarp;
		}

		public MapId RandomVampireAttack(List<Map> maps, bool lefeinHospitality, bool includeConeria, MT19337 rng)
		{
			// Essentially picks a random town to have the vampire attack rather than Melmond.
			// I mean, it could randomly pick Melmond in which case it's just vanilla.
			// Otherwise, though, it replaces Melmond's map with a "fixed" version and the
			// rolled map with a "destroyed" version that has no clinic or item shop.

			// These are just hard-coded maps (layouts courtesy of DarkmoonEX)
			const string repaired_melmond = "4747474747474747470A0404040404040404040404040B2F2F2F2F2F2F0027474747474747474747474747474747474747474747474747474747474747474747474747474747470A0405020E000E1717170F0F0F000003040B2F00002F2F2747474747474747474747474747474747474747474747474747474747474747474747474747470A04050200000000001822180F0F0F0E00000003040B00000027474747474747474747474747474747474747474747474747474747474747474747474747470A05020000000E0F0F0F1D461D0F0F0F0E0000000000030B002727474747474747474747474747474747474747474747474747474747474747474747474747470A0100000000000E0E0E0E100E0E0E0E000000000000000D0027004747474747474747474747474747474747474747474747474747474747474747474747470305020000000000111010101010101010101010101010101010270047474747474747474747474747474747474747474747474747474747474747474747101010101010101010101400272727272727272B2727272727272727270047474747474747474747474747474747474747474747474747474747474747474747100B000000000000000000002700000A040405100B00000000002727270047474747474747474747474747474747474747474747474747474747474747474747100C00000000272727272727270A05050210101003040B0011101010272727274747474747474747474747474747474747474747474747474747474747474747100C00000027270000000000000A01000010171717000C0010171717100A01274747474747474747474747474747474747474747474747474747474747474747100C303030271717171717000A05022F2F1018211801030B101B1A1B150A01272727272727270047474747474747474747474747474747474747474747474747100C272727271818181818000A00000000101D2D1D02310C101C651C160A01474747474747274747474747474747474747474747474747474747474747474747100C303030301D1D251D1D010A000000001310100000000C10101010100A01474747474747272727272747474747474747474747474747474747474747474747100C000000001D1D701D1D0205000000000000100000000C10171717100A01474747474747474747472747474747474747474747474747474747474747474747100C000000000000381010101010101010101010000A050D10182018100A01474747474747474747472727474747474747474747474747474747474747474747100D00000000000010000000000303050B0000100A050200101D771D100A014747474747474747474747474747474747474747474747474747474747474747471010101210101010140000000000000003030B100400000013101010100A01474747474747474747474747474747474747474747474747474747474747474747000B000000000010000000000017171700000D1000000E000E001010140A01474747474747474747474747474747474747474747474747474747474747474747000C0000000000100000000000182318010000101010101010101014000A0147474747474747474747474747474747474747474747474747474747474747474700030B000000001000000000001C521C020000100E000E000E000E0A04040247474747474747474747474747474747474747474747474747474747474747474700000C00000E0010101010101010101010101014000000000000000A0100004747474747474747474747474747474747474747474747474747474747474747474700030B0F0F0F100E0E0E100E0E0E0E0E10000000000000000A04040247474747474747474747474747474747474747474747474747474747474747474747474700000C0F0F0F100E0E0E10000000000010171717000000000A01000047474747474747474747474747474747474747474747474747474747474747474747474747000C0E0E0E13101010100E0E0E0E0E10182418010000000A014747474747474747474747474747474747474747474747474747474747474747474747474747470003030B0E0E0E102E103030303030101C5C1C0200000A040247474747474747474747474747474747474747474747474747474747474747474747474747474700000003040B0E131010101010101010103814000A04040200474747474747474747474747474747474747474747474747474747474747474747474747474747474700000003040B000000000000000000000A04040200474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747000000030404040000000000000404020000474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747000000000000000000000000004747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747004747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474727474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747274747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747472747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474727474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747274747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747472747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474727474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747274747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747472747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474727474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747274747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747472747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474727474747474747474747474747474747474747474747474747474747474747474747";
			const string blighted_coneria = "0B03030303030303030303030303052F102F030404040404040404040404040B47474747474747474747474747474747474747474747474747474747474747470C2F0E0E000000000000002F2F2F2F2F102F2F2F2F2F2F2F2F2F2F2F2F2F2F0C47474747474747474747474747474747474747474747474747474747474747470C2F171717001717170000322F272F00100032320F0F00101915002F2F0E0E0C47474747474747474747474747474747474747474747474747474747474747470C0018241801182318013232002700001000320F0F0E32101818010E2F27000C47474747474747474747474747474747474747474747474747474747474747470C001C591C021C4F1C020032102A101010000E0E0E32321C161C02002F27000C47474747474747474747474747474747474747474747474747474747474747470C0000380000003800000010002700001000000000003232103232000027000C47474747474747474747474747474747474747474747474747474747474747470C0032321010101032321032322700001000000000303000103230300027000C47474747474747474747474747474747474747474747474747474747474747470C0032000000003232000000322727272B272727272727272B2727272727320C47474747474747474747474747474747474747474747474747474747474747470C0000001717171700171717170000001000000000000000103210171527000C47474747474747474747474747474747474747474747474747474747474747470C2F000018182218011818211801000E100E000000000000100010181828000C47474747474747474747474747474747474747474747474747474747474747470C2F2F001D1D431D021D1D1E1D021110101010000000000010321D161C29000E47474747474747474747474747474747474747474747474747474747474747470C2F2F2F0000383200000038000E10101010100E00000032323200103227002F47474747474747474747474747474747474747474747474747474747474747470C2F0E0E000013101032321010101010323210101010101010101010102A000047474747474747474747474747474747474747474747474747474747474747470C2F31320000000000003232320E10103232323200000000000000000027000E47474747474747474747474747474747474747474747474747474747474747470C0E32320000000000000032000013101032320032272727272727272727000E474747474747474747474747474747474747474747474747474747474747474703040404040404040B1717171717000E323200003227000A0404040404040405474747474747474747474747474747474747474747474747474747474747474747474747474747470C18181818180000100000323227000A0147474747474700474747474747474747474747474747474747474747474747474747474747474747474747474747470C1D1D251D1D010E100E32320027000A0147474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747470C1D1D6D1D1D0232100000323227000A0147474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747470C00003800003232100E00000027000A0147474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747470C00301030320032320000000027000A0147474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747470C00001310101010103232322F272F0A0147474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747470C0000000000000E100E32000E0E2F0A0147474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747470304040404050E0E10323203030505050247474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747";
			const string blighted_pravoka = "0A040404040404040404040404040404040404040404040404040404040404040404040404040B474747474747474747474747474747474747474747474747470A282727272727272727272727272727171717171717172727272727272727272727272727270C474747474747474747474747474747474747474747474747470A2817171717171710101010101010271B18181B18181B2810171717171710101010101010270C004747474747474747474747474747474747474747474747470A281B181B18181B10101010101010281D1D1D251D1D1D28101B181B181B10101717171010280C004747474747474747474747474747474747474747474747470A281D1D1D1D1D1D15101717171717281D1D1D6E1D1D1D28101D1D1D1D1D15321B231B1510280C004747474747474747474747474747474747474747474747470A281D1D1D161D1D16101B181B181B281010103810101029101D1D1D1D1D16321C501C1610280C004747474747474747474747474747474747474747474747470A2810101010323232101D1D1D1D1D282727272B2727272710321010101010101038321010280C004747474747474747474747474747474747474747474747470A2810101010103232321D1D1D1D1D28272710101028272732321017171010171010101010280C004747474747474747474747474747474747474747474747470A2810101010323210103232101010282710101010322827321018151B10101B18151B1510280C004747474747474747474747474747474747474747474747470A28101010321017171717171010102810100E000E3232281D1518181D15101D1D161D1610280C004747474747474747474747474747474747474747474747470A2810171717101B181B181B10103210100E0000000010101D1D161D1D16101D1D1D1D1032280C004747474747474747474747474747474747474747474747470A28101B181B101D1D1D171717171710100000000032101010101010101010101010103232280C004747474747474747474747474747474747474747474747470A28101D1D1D151D1D1D1B181B181B10100E00000032323210171717171010171717171710280C004747474747474747474747474747474747474747474747470A28101D1D1D161010101D1D1D1D1D2810100E0032323229101B18181B10101B181B181B10280C004747474747474747474747474747474747474747474747470A2810101010101010101D1D161D1D282710101032322927101D1D1D1D15101D1D1D1D1D15280C004747474747474747474747474747474747474747474747470A2810101010101010101010101010102827101032282710101D1D1D1D16101D1D16171717280C004747474747474747474747474747474747474747474747470A2817171710101717171717101010101028101032281010101017103232103232101B241B280C004747474747474747474747474747474747474747474747470A281B181B10101B181B181B10101717172810323228101010101B323232171717101D5A1D280C004747474747474747474747474747474747474747474747470A281D1D1D15101D1D17171717171B221B28103232283210101D1D1532101B211B15103810280C004747474747474747474747474747474747474747474747470A281D1D1D16101D1D1B181B181B1C441C283232102832101D1D171717171D1F1D16101010280C004747474747474747474747474747474747474747474747470A28323232101010101D1D1D1D1D1538102832101028323210101B18181B10381717171710280C004747474747474747474747474747474747474747474747470A28323210171510101D1D161D1D1610102832101028103232101D1D1D1D15101B18181B10280C004747474747474747474747474747474747474747474747470A281010101818151010101010101010102810101028323232101D1D161D16101D1D1D1D15280C004747474747474747474747474747474747474747474747470A2832101D161C161010101010101010102832101028101717171717101010101D1D161D16280C004747474747474747474747474747474747474747474747470A283232101010323232101010101B10102832101028101B181B181B101010101010101010280C004747474747474747474747474747474747474747474747470A281017151010103232323210321D15322832321028101D1D1D1D1D151017171717101010280C004747474747474747474747474747474747474747474747470A28101B1610101B1010323232101D16322832323228101D1D161D1D16101B18181B101010280C004747474747474747474747474747474747474747474747470A28101D1D161D1D151010103232101010293232102810101010323210101D1D1D1D151010280C004747474747474747474747474747474747474747474747470A28101D161D1D1D161010323228272727273232102827272727321010101D1D1D1D161032280C004747474747474747474747474747474747474747474747470A281010101010101032323232290B32323232101000000E0A273232321010101010101010290C004747474747474747474747474747474747474747474747470A282727272727272727272727270C32320032101000000E0A282727272727272727272727270C004747474747474747474747474747474747474747474747470607070707070707070707070707090E320010101000000E060707070707070707070707070709004747474747474747474747474747474747474747474747474747474747474747474747474747470E320010101000000E4747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747";
			const string blighted_elfland = "470F0F0F0F0F0F0F0F0E0E0E0E0E0E0E0000000000000000000000000000000E0E0E0E0E0E0E0E000000474747474747474747474747474747474747474747470F0F0F0F0F0F0F0F0F272727272727272727270000000000000000000000003200000000000000000E0E474747474747474747474747474747474747474747470F1717170F0F0F0F0F0F0F0F17171700000027000E0F0F00000000000000323200003232000000000000474747474747474747474747474747474747474747470F1824180F0F0F0F0F0F0F0E1823180100002A00000E0F0F000000000032323232323200000000000000474747474747474747474747474747474747474747470E1C5E1C0E0F0F0F0F0E0E001C541C020000270000000F0F0F00000000000032000032323200000000004747474747474747474747474747474747474747474700003932320F0F0F0E000000003900323232270000000E0F0F000000323232323232320000000000000047474747474747474747474747474747474747474747000032320F0F0F0E32323232000032320F0F27000000000F0F0F32323200003232323232000000000000474747474747474747474747474747474747474747470F0F00320E0E0E323232000000000F0F0F0F272F2F00000E0F0F0F0F0000000032000000000000000000474747474747474747474747474747474747474747470F0F0F00000032323200000032320F0F0F0F272F2F0000000E0E0F0F0F0F0F0F0F0F0F00000000000000474747474747474747474747474747474747474747470F0F0F0F00000032000F0F0F0F0F0F0F0F0E272F2F2F000000000E0F0F0F0F0F0E0E0E0E0E0000000000474747474747474747474747474747474747474747470F0F0F0F0000000F0F0F0F0F0F0F0F0F0F27272F2F2F2F000000000E0E0F0F0E007F2F2F000E0F000000474747474747474747474747474747474747474747470F0F0F0E00000F0F0F0F0F0F0F0F0F0F0F272F2F2F2F2F00000000000E0F0F0E3200320000000E00000E474747474747474747474747474747474747474747470F0F0F272B270F0F0F0F0F0F0F0E0E0E0E272F2F2F2F2F00000F0F1717170F323232320000000000000E474747474747474747474747474747474747474747470F0F0F00000F0F0F0F0E0E0E0E2727272727272F2F2F00000F0F0F1823180E320F0F0F0F0F0F0F000000474747474747474747474747474747474747474747470E0F0F00000F0F0F0E27272727270010171527000000000E0E0F0F1C511C02000E0F0F0F0F0F0F00000047474747474747474747474747474747474747474747000F0F00320F0F0E00320000000000101818282727000000000E0E0039000000000E0E0E0F0F0E00000047474747474747474747474747474747474747474747470F0F32320E0E32323200003232321D161C020027272B272727272727272727272727270E0E0000000F47474747474747474747474747474747474747474747470F0F000032323200323232323232320000003232320000000F0F171717000000000000000000000F0F47474747474747474747474747474747474747474747470E0F0F00000000000000000032323200003232320000000F0F0F182418010000000F1717171717170F4747474747474747474747474747474747474747474747470F0F0F0000000000000000003232323232000F0F0F0F0F0F0F1C5B1C02000F0F0F1825181818180F4747474747474747474747474747474747474747474747470F0F0F0F0F0F0F0F0000000000000F0F0F0F0F0F0F0F0F0F0F0F390032000F0F0F1D6F1D1D1D1D0F4747474747474747474747474747474747474747474747470E0F0F0F0F0F0F0E0000000F0F0F0F0F0E0E0F1010100F0F0F0F323232320E0E0E0E3932320E0E0E474747474747474747474747474747474747474747474747470E0F0F0F0F0F0000000F0F0F0F0F0E000010101915170E0E0E0F0F0F323232101032101010101047474747474747474747474747474747474747474747474747470E0E0F0F0F000F0F0F0F0F0F0F0000001B1518181B0100000E0E0F0F001032323232000E0E0E474747474747474747474747474747474747474747474747474747470F0F0E000F0F0F0F0F0F0F3200001D1D161D1D02000000000E0E00103200000000000000474747474747474747474747474747474747474747474747474747470F0E00000E1717170E0F0F0E000000003832000000000000000000100F0F323232000000474747474747474747474747474747474747474747474747474747470F00000000182218010E0F00000030303232300000000000321132140F0F0F3200000000004747474747474747474747474747474747474747474747474747470E0F0000001D451D02000E000F323232100000000032323232100F0F17171700000F000047474747474747474747474747474747474747474747474747474747470F0F0000003900000000000F320000131010323232101010140F0F18211801000F000047474747474747474747474747474747474747474747474747474747470E0F0F000000000F0F0F0F0F0F323200000000000F0F0F0F0F0F0E1D261D02000F00004747474747474747474747474747474747474747474747474747474747470E0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F00003900000F0F0000474747474747474747474747474747474747474747474747474747474747470E0E0E0E0E0E0E0E0E0E0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F00000F0F0F000047474747474747474747474747474747474747474747474747474747474747474747474747474747470E0E0E0E0E0E0E0E0E0E0E0E0E0E0E0E0E0E0E0E0E00004747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747";
			const string blighted_crescent = "0A040404040404040404040404040404040405000E0E0F0F0F0E0E0E0F0F0F0F0F0F0F0F0F0E0F0F0E0F0F0F47474747474747474747474747474747474747470A012F2F00002F2F2F2F2F2F003232000000003200000E0E0E0000000E0F0F0F0F0E0F0F0E000E0E000F0E0F0F0F4747474747474747474747474747474747470A0100000000322F2F2F2F2F17171700171717000000000000000000000F0F0F0F000E0E00000000000E000F0F0F4747474747474747474747474747474747470A0100171717002727270E0F18231801182418010B0F000000000F00000F0F0F0E0F00000000000000000E0E0F0F0F47474747474747474747474747474747470A013218211828272727270E1D531D021D5D1D020C0E0F0E00000F00000F0F0F000F0E0000000000000000000E0F0F47474747474747474747474747474747470A01001D361D29272727270030380000323830300C320F0E000F0F00000E0F0F0E0E00000000000000000000000F0F0F474747474747474747474747474747470A0130303830302727272727272B2727272B27270C320E0F000F0F0F00000F0F000000000000000000000000000F0F0F0F4747474747474747474747474747470A0132001310102A2A2A2A2A10101010101400320C32320E000F0F0F00000F0F000000000000000000000000000E0F0F0F4747474747474747474747474747470A012F32322F00272727272732323200000000000C3232320E0F0F0F00000F0F000000000000000000000000000E0F0F0F4747474747474747474747474747470A012F320F0F0F272727272710322F2F2F2F0F0F0C32320F320F0F0F00000F0E00000000000000000000000000000E0F0F0F47474747474747474747474747470A0132321717172727272727100F0F0F0F000F0F0C32320F0F0F0F0F00000F0F0F000000000000000000000000000F0F0F0F47474747474747474747474747470A0132321822182827272727100E101915000F0F0C320F0F0F0F0F0F00000F0F0F0E000000000000000000000F0F0F0F0F0F0F474747474747474747474747470A0132321C491C29272727271000101818010F0F0C320F0F0F0F0F0F00000E0F0F00000000000000000000000F0F0F0F0F0F0F474747474747474747474747470A01323232383232322F2F2F10001D161C020F0F0C0F0F0F0F0F0F0F0000000F0F0F0F000F0F00000E0F00000E0F0F0F0F0F0F474747474747474747474747470A0132323213101032101010140F321032000F0F0C0F0F0F0F0F0F0F0F00000F0F0F0F0F0F0E00000F0F0F0F000F0F0F0F0F0E474747474747474747474747470A010F0F0F0F321032320F0F0F0F3210320F0F0F0C0F0F0F0F0F0F0F0F0E000F0F0F0E0E0E0000000F0F0F0F0F0F0F0F0F0E47474747474747474747474747470A010F1017150F100F171717170F0010000E0F0F0C0F0F0E0F0F0F0F0F00000E0F0E0000000E00000F0F0F0F0F0F0F0F0F4747474747474747474747474747470A010F1018180F100F181818180F0010322F0E0F0C0F0E320F0F0F0F0F000E0E0E0000000000000F0F0F0F0F0F0F0F0F0F4747474747474747474747474747470A010F1D161C0F100F1C1C251C0F00323232320F0C0F32320E0F0F0F0F000000000000000F0F0F0F0F0F0F0F0F0F0F0F0F4747474747474747474747474747470A010E3210320F100E1C1C711C0F3210002F320F0C0F0F0F320F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F4747474747474747474747474747470A01323210300E1032000038320E00102F002F0F0C0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F4747474747474747474747474747470A0100323210101032323210101010143232000E0C0E0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F4747474747474747474747474747470A010000000000320E0E0E100E0E0E320E0E0E0E0C000E0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F47474747474747474747474747474706070707070707070707091006070707070707070900000F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F47474747474747474747474747474747474747474747474747474747474747474747474700000E0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F4747474747474747474747474747474747474747474747474747474747474747474747000000000F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F4747474747474747474747474747474747474747474747474747474747474747474747470000000E0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F474747474747474747474747474747474747474747474747474747474747474747474747000000000E0E0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F4747474747474747474747474747474747474747474747474747474747474747474747470000000000000F0F0F0F0F0F0F0F0E0F0F0F0F0F0F0F0F0F0F0F0F0F4747474747474747474747474747474747474747474747474747474747474747474747470000000000000E0F0F0F0F0F0F0F000F0F0F0F0F0F0F0F0F0F0F0F0F474747474747474747474747474747474747474747474747474747474747474747474747000000000000000E0F0F0F0E0E0E000E0E0F0F0E0E0E0E0E0E0E0E0E47474747474747474747474747474747474747474747474747474747474747474747474700004747474747470E0E0E0E000000000E0E0E00000000000000000047474747474747474747474747474747474747474747474747474747474747474747474700004747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747000047474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747470000474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747470F0F0F0F0F0F47474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747470F0F0F0F0F0F0F0F0F0F4747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747470F0F0F0F0F0F0F0F0F0F0F0F0F0F474747474747474747474747474747474747474747474747474747474747474747474747474747474747000000000F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F474747474747474747474747474747474747474747474747474747474747474747474747474747474747000F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F47474747474747474747474747474747474747474747474747474747474747474747474747474747470F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F474747474747474747474747474747474747474747";
			const string blighted_onrac = "0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F272727272727272747474747474747470F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0E0E0E0E0E0E0E0E0E0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F272727272727272747474747474747470F0F0F0F0E0E0E0E0F0F0F0F0F0F0E0E0E0E0E0E0E0F0F0A040303030303030B0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F272727272727272747474747474747470F0F0F0E323232320E0F0F0F0F0F310032320000000E0F0A2F002F002F002F0C0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F272727272727272747474747474747470F0F0E3132101715000F0F0F0F0E00323210100000000F0A000000000000000C0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F272727272727272747474747474747470F0F003232101818010E0F0F0F0000001019150000000F0A002F002F002F000C0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F272727272727272747474747474747470F0F0032321D161C02000F0F0E0032321018180100000F0A000000000000000C0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F272727272727272747474747474747470F0F32003232320032000F0F323232321D161C0200000F0A002F002F002F000C0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F272727272727272747474747474747470E0F00323232103200000F0F003232323210000000000F0A000000000000000C0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F27272727272727274747474747474747470E323200323232322F0E0E2F00002F3232002F002F0F0A2F00000000002F0C0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F27272727272727274747474747474747470030303032100030300000303030300010003030300E0607070800060707090F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F27272727272727274747474747474747470000000032320000003232000000000010000000000000002F0010002F00000F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F2727272727272727474747474747474747000032323232103210103232323210101032321010101010101014003200000F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F2727272727272727474747474747474747000000003232000000003232000000001032323232323200000000003232000F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F2727272727272727474747474747474747000F0F0F0F0F0F0F0F0F0F32320032321032003232321000000000323232000F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F27272727272727274747474747474747470F0F0E0E0E0E0E0E0E0F0F0F0000000010000000323210000000000000000F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F272727272727272747474747474747470F0E2727272727272727270E0F32321010140032320000323232320032000F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F272727272727272747474747474747470F27171717171717171717270E3232000E0E0E0E0E0E3232320E0E0E0E0E0E0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F272727272727272747474747474747470F271818181B181B181818270E1000000E11171717171710171717171712000F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F272727272727272747474747474747470F271818181D251D181818280E1032320E101B1823181B151B1824181B15000F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F272727272727272747474747474747470F271B181B1D731D1B181B280E1032320E101C1C571C1C161C1C611C1C16000F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F272727272727272747474747474747470F271D1D1D0138001D1D1D280E3232320E1310103C101010101038101014000F0F0F0F0F0F0F0F0F0F0F0F0F0E0E0E0E272727272727272747474747474747470F271D1D1D0232321D1D1D290E1032000E0E0E0E0E0E0E0E0E320E0E0E0E0F0F0F0F0F0F0F0F0F0F0F0F0F0E27272727272727272727272747474747474747470F270E000000103232000E270E10101010101010101010101032323232000F0F0F0F0F0F0F0F0F0F0F0F0F27270E470E272727272727272747474747474747470F270E000011101200000E270E10323232000000323200000000323232320F0F0F0F0F0F0F0F0F0F0F0F0F270E00000E272727272727272747474747474747470F2732323232321000000E270E10323232323232000032003232323200000F0F0F0F0F0F0F0F0E0E0E0E0E2732000E0E272727272727272747474747474747470F270E323213101400000E27272B272727272727272727272727320032320F0F0F0F0F0F0F0F282727272727000E0E00272727272727272747474747474747470F270E323232323232000E270E10000000000000000000000027323232320F0F0F0F0E0E0E0E290E0E0000000032000E272727272727272747474747474747470F270E0E0E0E100E0E0E0E273232323232000000000000000027272727270F0F0F0F2827272727323232000E0E0E0E27272727272727272747474747474747470F27272727272B27272727270E10323232323232320000000027003232000E0E0E0E0E32000E0E000E0E0E0E0E111010272727272727272747474747474747470F0F0F0F0F0F100E0E0E0E0E111010101010101032321032102A10103232101010101010101010101010101010104827272727272727272747474747474747470F0F0F0F0F0F13101010101014000000000032323200000000270E0E0E0E32323200000E0E0E0E0E00000E0E0E131010272727272727272747474747474747470F0F0F0F0F0F0F0F0F0F0F0F0F0F0000003232000F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F000E0E0E27272727272727272747474747474747470F0F0F0F0F0F0F0F0F0F0F0F0F0F0F000000000F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F000E0E0E272727272727272747474747474747470F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F00000F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F27272727272747474747474747470F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F00000F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F27272727272747474747474747470F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F00000F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F27272727272747474747474747470F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F00000F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F27272727272747474747474747470F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F00000F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0E27272727272747474747474747470E0E0E0E0E0E0E0E0E0E0E0E0E0E0E0E00000E0E0E0E0E0E0E0E0E0E0E0E0E0E0E0E0E0E0E0E0E0E0E0E0E0E0E0E0E0E0E47272727272727474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747472727272727274747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747472727272727272747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474727272727272727474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474727272727272727274747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747272727272727272747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747472727272727272727474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474727272727272727274747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747272727272727272747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747472727272727272727474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474727272727272727274747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747272727272727272747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747472727272727272727474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474727272727272727274747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747272727272727272747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747472727272727272727474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474727272727272727274747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747272727272727272747474747474747470F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F";
			const string blighted_gaia = "474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747470F0F0F0F0F0F47474747474747474747474747474747474747474747474747474747474747474747474747470047474747474747474730303030474747470F0F0F0F0F0F0F0F0F0F0F474747474747470F0F0F0F0F0F0F0F0F0F0F0F4747474747474747474747474747474747474747474747474747474747474747470F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F00000000000000004747474747474747474747474747474747470F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0000303030000047474747474747474747474730303030300F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F00000047474747474747474747474747470F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F00474747474747474747473030300F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F4747474747474747474747470F0F0F0F0F0E0E0E0E0E0E0E0E0E0E0E0E0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F474747474747474747470F0F0F0F0F0F0E3030303030323210103230300E0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0E0E0E0E0E0E0F0F0F0F0F0F0F0F0F0F4747474747474747470F0F0F0F0F0F0F000F31000E0E32101815180E0F000E0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0E2727272727270E0F0F0F0F0F0F0F0F0F4747474747474747470F0F0F0F0F0F0E000F00000032101018181B010F00000E0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F27272727272727270F0F0F0F0F0F0F0F0F4747474747474747470F0F0F0F0F0E00000F00000000321C161C1C020F000E000E0F0F0F0F0F0F0F0F0F0F0F0F0F0F27272727272727270F0F0F0F0F0F0F0F0F4747474747474747470F0F0F0F0E00000030300F00000F0F100F0F000F000000000E0F0F0E0E0E0E0F0F0F0F0F0F0F27272727272727270F0F0F0F0F0F0F0F0F4747474747474747470E0F0F0F0000000000320F00000000100000003200000000000E0E2F2F2F2F0F0F0F0F0F0F0F0E2727272727270F0F0F0F0F0F0F0F0F0F474747474747474747320E0E0F0000003200003030303030113030323000000E000E00002F2F2F2F0F0F0F0F0F0F0F0F0E000000000E0F0F0F0F0F0F0F0F0F0F4747474747474747470000000E0000323200000000000000100000000000000000000032000000000E0F0F0F0F0F0F0F0F0E000E000F0E0F0F0F0F0F0F0F0F0F47474747474747474700320032000000000000000000320010000000000F323200000E0000000E00000E0F0F0F0F0F0F0E0000000E0E0F0F0F0F0F0F0F0F0F0F4747474747474747470F3200000000000000000000000000320000000F0F0F0F320000000F0F000000000F0F0F0F0F0F000E000E000F0F0F0F0F0F0F0F0F0F0E4747474747474747470F0F0000003200001110101010101032320F0F0F0F0F0F0F0F0F0F0F0F000000000F0F0F0F0F0E00000000000E0F0F0F0F0E0E0E0E0E004747474747474747470F0F0F00323200323200000F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0E00000F0F0F0F0F0F0000000E0000000E0E0E0E00000E0000004747474747474747470F0F0F0F0F0F320032000F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F00000E0F0F0F0F0F0F00000E0000000000000000000000000E004747474747474747470F0F0F0F0F0F0F00100F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0E0000000F0F0F0F0F0F0F00000000000E000F0F0F0F0F0F0F0F004747474747474747470F0F0F0F0F0F0F00100F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0E0000000F0F0F0F0F0F0F0F0F00000E00000F0F0F0F0F0F0F0F0F0F4747474747474747470F0F0F0F0E0E0F0F100F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F000E00000F0F0F0F0F0F0F0F0F0F000000000F0F0F0F0F0F0F0F0F0F4747474747474747470F0F0F0E32000E0F100F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F000000000F0F0F0F0F0F0F0F0F0E0000000F0F0F0F0F0F0F0F0F0F0F4747474747474747470F0F0E000000000E100F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F00000E0E0E0F0F0F0F0E0E0E00000F0F0F0F0F0F0F0F0F0F0F0F0F4747474747474747470F0E323232101010100F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F00000000000E0E0E0E000000000F0F0F0F0F0F0F0F0F0F0F0F0F0F4747474747474747470F32320E10171532100F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F00000000000E0000000F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F4747474747474747470F0E103232181801100F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F000E000000000F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F4747474747474747470F00320E101D1D02100F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F000000000F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F4747474747474747470F0E323210303030100F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0E0F0F0E0E0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0E4747474747474747470F0E131032101010140F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F000E0E00000E0E0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0E0E0E004747474747474747470F0F00001000320F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0E001717171717000E0F0F0F0F0F0F0F0F0F0F0F0F0F0F0E000000004747474747474747470F0F0F0010000F0F0F0F0F0F0F0E0E0E0F0F0F0F0F0F0F0F0E0E0E000E182318241801000F0F0F0F0F0F0F0F0F0F0F0F0E0E00000000004747474747474747470F0F0F0F10000F0F0F0F0F0E0E2F2F2F0E0F0F0F0F0F0F0E00000000321D561D601D020E0F0F0F0F0F0F0F0F0F0F0F0E000000000000004747474747474747470F0F0E0E10000F0F0F0F0E0000000000000E0E0E0E0E0E00320E0E0E00003900390000320F0F0F0F0F0F0F0F0F0F0E000000000E0E0E0E4747474747474747470F0F3200100F0F0F0E0E0000000000000000000000003232320032000000320E0E0E00000F0F0F0F0F0F0F0E0E0E000000000E000000004747474747474747470F0F3232320F0F0E000000002F2F2F2F2F000000000032003200320E0E0E0E000000320F0F0F0F0F0F0F0E0000000000000000000000004747474747474747470F0F0F00100E0E00000F0F0F2F2F2F2F2F0F0F0F0F0F0F3200000032000E0000000F0F0F0F0F0F0F0F0E000000000000000000000000004747474747474747470E0F0F32103200000F0F0F0F0F2F322F0E0E0F0F0F0F0F0F000032000000000F320F0F0F0F0F0F0F0E00000000000E0E0E320E320E0000474747474747474747000F0F0F100000000F0F0F0F0E00320000000E0E0E0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0E0E0E000000000000003200323200000E00474747474747474747000E0F0F1000320F0F0E0E0E003232320000000000320E0E0E0E0E0E0E0E0E0E0E0E0E0E0E00320000000000000000003200000000000E47474747474747474700000F0E3232320F0F1010101010323232101032323210101010101010101010101010101032101010101032321032101010101010101247474747474747474700000F001032000F0F1010101010101010321010321010171717171717171010321010103232101010103210321010101010101010101047474747474747474700000E0010000F0E0E1010171717171717171717000A031818181B181818050B001010102F0E0E171717172F2F0F301717171730300F104747474747474747470000000010000E00001010182118181818182218010A0F1C1C1C251C1C1C010C001010102F000018231818012F0F111818241815120F1047474747474747474730300000103030303010101D3E1D1C1D1C1D4A1D020A0E1D1D1D721D1D1D020C001010102F00001D551D1D02320F131C1C5F1C16140F1047474747474747474700000000103200000010102F3C2F2F2F2F323800000607070709380607070709003210100E0E0E0E380E0E0E0E0E303030383030300E104747474747474747471010323210323210321010101010101032321010101032101010101032101010101010101010101010101010101010101032101032101047474747474747474732101032101032101010101032101032101010101010103210101010101010103210321010101010101010101010323232101010323214474747474747474747000000000000003200000032320000000032000000000000000000000000003232000000000000000000000010100032000000000000004747474747474747473030303030303030303030303200000000000000000000000032003030303030303030303030303030303000101000303030303030300047474747474747474700000000000000000000000000320000003200000000000000323200000000000000000000000000000000001010000000000000000000474747474747474747000000000000000000000E0E0E0E0E0E0E0E00000000000032000000000000000E0E0E0E0E0E0000000000001010101010101010101012474747474747474747000000000000000000000000000000000000000000000000000000000000000000000000000000000000000013101010101010101010104747474747474747470000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000101047474747474747474700004700003030303030303000000030303030303030303000000000303030303000000000303030303030303030303030303030001314474747474747474747474747474747474747474747474747474747474747470047474747474747474747474747474747474747474747474747474747474747474747";
			const string blighted_lefein = "000000000000000000000000000000000000000000000047474747474747474700000000000000000000000000000000000000000E0000000000000000000000000000000000000000000000000000000000000000000047474747474747474700000E000E000000000000000000000000000E0E0017171717000E000000000047000A040404040504040404040404040B47474747470A040404040404040404040E0E0E0E0E00000000000000000000000E0E0E001A23241A010E0E0000000047000A10101010101010101010101010030404040404051010101010101010101010100E0E32320000000000000000000E000E00001C58621C020E000000000047000A1010171510171717171717171017171517151715101717171010171510171717103232323200320000000000000000000000000000000000000000000000000A10101818151B181B181B181B151B181818161818151B181B15101818151B181B150E00323232000032320000000000000E0E000E00000E0E000000000000000A101D161C161C1D1C1D1C1D1C161D161C161D161C161C1D1C161D161C161C1D1C160B0000000000000032323232323200000E0E0000000E00000000000000000A102F002F102F002F002F002F1032322F102F002F102F002F102F002F102F002F100E00000000000000323232320000000000000000000000000000000000000A1010103232101010101010323232323232101010101010101010101010101010100E0E000000320000320000000000000000000000000000000000000000000A103232320E0E0E0E0E0E0E0E0E0E0E1032320E0E0E0E0E0E0E0E0E0E0E0E1032100B0E0E3232003200000000000000000000000000000000000000000000000A323232100E0304040404040405100E1010100E10030404040404040405323232100C0E2F2F2F2F002F2F2F2F002F2F2F2F002F2F2F2F002F2F2F2F000000000A10282727272727272727272727100E1010100E10282727272727272727272727100C002F2F2F2F002F2F2F2F002F2F2F2F002F2F2F2F002F2F2F2F000000000A32283232321010101010101028100E1010100E10283232321010321010101028100C002F2F2F2F002F2F2F2F002F2F2F2F002F2F2F2F002F2F2F2F000000000A3228101010103232103232102810101010101010281010323232103232101028100C002F2F2F2F002F2F2F2F002F2F2F2F002F2F2F2F002F2F2F2F000047470A102A2A2A2A32322A2A10323228103232320E1010281010102A2A2A2A2A2A2A2A100C00000000000000000000000000000000000000000000000000000000000A1028272732321010103232102832323210100E10283210101010323232282727100C00000000000000000000000047000000000000000000000000000047470A1028272732323210323232322810101032101010283232101032321010282727320C47474747474747474747474747000000000000000000000000000000000A102A2A2A2A2A2A2A2A10323228320E323232321028323232322A2A2A32282727320C47474747474747474747474747000000000000000000000000000000000A1028272727271010101010102810100E10323232283232321010102827272727320C47474747474747474747474747000000000000000000000000000000000A3228272727271032323232102810101010101010281010101010102827272727100C47474747474747474747474747000000000000000000000000000000000A32282727272732323210101028100E1010100E10283232102A2A2A2A2A2A2A2A100C47474747474747474747474747000000000000000000000000000000000A32282727272727272710101029100E1032320E10281032322827272727272727100C47474747474747474747474747470000000000000000000000000000000A32282727272727272727272727100E1010320E10282727272727272727272727100C4747474747474747474747474747000000000000000000000000000000060707070707070707070707070813101010101014060707070707070707070707070947474747474747474747474747470000000000000000000000000000004747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474700000000000000000000000000000047474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747000000000000000000000000000000474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747470000000000000000000000000000004747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474700000000000000000000000000000047474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747000000000000000000000000000000474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747470000000000000000000000000000004747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474700000000000000000000000000000047474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747000000000000000000000000004700474747474747474747474747474747004747474747474747474747474747470047474747474747474747474747474700474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747004747474747474747474747474747470047474747474747474747474747474700474747474747474747474747474747004747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747474747";

			MapId selectedMap = MapId.Melmond;

			// Roll 1d8 to see which town was destroyed.
			int start = includeConeria ? 1 : 2;
			switch (Rng.Between(rng, start, 8))
			{
				case 1:
					maps[(byte)MapId.Melmond] = new Map(Blob.FromHex(repaired_melmond).ToBytes());
					maps[(byte)MapId.Coneria] = new Map(Blob.FromHex(blighted_coneria).ToBytes());
					selectedMap = MapId.Coneria;
					// The "repaired Melmond" map defaults to having Coneria's item shop.
					break;
				case 2:
					maps[(byte)MapId.Melmond] = new Map(Blob.FromHex(repaired_melmond).ToBytes());
					maps[(byte)MapId.Pravoka] = new Map(Blob.FromHex(blighted_pravoka).ToBytes());
					// Replace the item shop in "repaired Melmond" with the blighted town's item shop.
					maps[(byte)MapId.Melmond][0x0F, 0x1B] = 0x78;
					selectedMap = MapId.Pravoka;
					break;
				case 3:
					maps[(byte)MapId.Melmond] = new Map(Blob.FromHex(repaired_melmond).ToBytes());
					maps[(byte)MapId.Elfland] = new Map(Blob.FromHex(blighted_elfland).ToBytes());
					maps[(byte)MapId.Melmond][0x0F, 0x1B] = 0x79;
					selectedMap = MapId.Elfland;
					break;
				case 4:
					maps[(byte)MapId.Melmond] = new Map(Blob.FromHex(repaired_melmond).ToBytes());
					maps[(byte)MapId.CrescentLake] = new Map(Blob.FromHex(blighted_crescent).ToBytes());
					maps[(byte)MapId.Melmond][0x0F, 0x1B] = 0x7A;
					selectedMap = MapId.CrescentLake;
					break;
				case 5:
					maps[(byte)MapId.Melmond] = new Map(Blob.FromHex(repaired_melmond).ToBytes());
					maps[(byte)MapId.Gaia] = new Map(Blob.FromHex(blighted_gaia).ToBytes());
					maps[(byte)MapId.Melmond][0x0F, 0x1B] = 0x7B;
					selectedMap = MapId.Gaia;
					break;
				case 6:
					maps[(byte)MapId.Melmond] = new Map(Blob.FromHex(repaired_melmond).ToBytes());
					maps[(byte)MapId.Onrac] = new Map(Blob.FromHex(blighted_onrac).ToBytes());
					maps[(byte)MapId.Melmond][0x0F, 0x1B] = 0x7C;
					selectedMap = MapId.Onrac;
					break;
				case 7:
					maps[(byte)MapId.Melmond] = new Map(Blob.FromHex(repaired_melmond).ToBytes());
					maps[(byte)MapId.Lefein] = new Map(Blob.FromHex(blighted_lefein).ToBytes());
					// If Lefein is attacked when Hospitality is active, restore the Inn (Clinic is still destroyed)
					if (lefeinHospitality)
					{
						List<Blob> restoredInn = new List<Blob>
						{
							Blob.FromHex("171717"),
							Blob.FromHex("1B251B"),
							Blob.FromHex("1C711C"),
						};
						maps[(int)MapId.Lefein].Put((0x10, 0x04), restoredInn.ToArray());
					}
					selectedMap = MapId.Lefein;
					// Lefein never had an item shop so we fall back on the default Coneria.
					break;
				case 8:
					selectedMap = MapId.Melmond;
					break;
					// Case 8 is that Melmond stays the blighted one so nothing needs to be done.
			}

			return selectedMap;
		}
		public void ShufflePravoka(Flags flags, MT19337 rng, List<Map> maps, bool blightedPravoka)
		{
			if (!(bool)flags.ShufflePravokaShops)
			{
				return;
			}

			// weapon, armor, white, black, inn
			List<(byte sign, byte door)> shopTiles = new() { (0x21, 0x1F), (0x22, 0x44), (0x23, 0x50), (0x24, 0x5A), (0x25, 0x6E) };

			// prep coordinates
			List<(int x, int y)> roofTiles = new() { (0x0F, 0x12), (0x21, 0x04), (0x23, 0x10), (0x1F, 0x12) };
			List<(int x, int y)> wallTiles = new() { (0x22, 0x11), (0x1E, 0x13), (0x20, 0x05), (0x0E, 0x13) };
			List<(int x, int y)> highWallTiles = new() { (0x13, 0x03) };

			List<(int x, int y)> validLocations = new()
			{
				(0x03, 0x12),
				(0x04, 0x0C),
				(0x05, 0x04),
				(0x08, 0x12),
				(0x09, 0x0B),
				(0x0C, 0x06),
				(0x0C, 0x0D),
				(0x13, 0x03),
				(0x19, 0x19),
				(0x1B, 0x04),
				(0x1B, 0x0E),
				(0x1C, 0x15),
				(0x1F, 0x12),
				(0x1F, 0x1B),
				(0x21, 0x04),
				(0x21, 0x0E),
				(0x22, 0x16),
				(0x23, 0x10),
			};

			// we can add the item shop and the clinic and a few more potential locations if pravoka hasn't been attacked by vampire
			if (!blightedPravoka)
			{
				shopTiles.Add((0x20, 0x78)); // Item Shop
				shopTiles.Add((0x1A, 0x68)); // Clinic

				// Clinic
				highWallTiles.Add((0x1A, 0x09));
				maps[(int)MapId.Pravoka][0x08, 0x1A] = 0x1B;

				// Item Shop
				roofTiles.Add((0x05, 0x016));
				wallTiles.Add((0x04, 0x17));

				validLocations.AddRange(new List<(int x, int y)> {
					(0x04, 0x1B),
					(0x05, 0x16),
					(0x0C, 0x19),
					(0x19, 0x12),
					(0x1A, 0x09),
					(0x20, 0x09),
				});
			}

			// wipe the map first
			foreach (var coord in roofTiles)
			{
				maps[(int)MapId.Pravoka][coord.y, coord.x] = 0x18;
			}

			foreach (var coord in wallTiles)
			{
				maps[(int)MapId.Pravoka].Put(coord, new List<Blob> { Blob.FromHex("1D1D1D") }.ToArray());
			}

			foreach (var coord in highWallTiles)
			{
				maps[(int)MapId.Pravoka].Put(coord, new List<Blob> { Blob.FromHex("1D"), Blob.FromHex("16") }.ToArray());
			}

			// then place the shops
			foreach (var shop in shopTiles)
			{
				var coord = validLocations.SpliceRandom(rng);
				maps[(int)MapId.Pravoka][coord.y, coord.x] = shop.sign;
				maps[(int)MapId.Pravoka][coord.y + 1, coord.x] = shop.door;
			}
		}

		public List<MapId> HorizontalFlipDungeons(MT19337 rng, List<Map> maps, TeleportShuffle teleporters, OverworldMap overworld)
		{
			var validMaps = new List<MapId> { MapId.EarthCaveB1, MapId.EarthCaveB2, MapId.EarthCaveB3, MapId.EarthCaveB4, MapId.EarthCaveB5,
				MapId.GurguVolcanoB1, MapId.GurguVolcanoB2, MapId.GurguVolcanoB3, MapId.GurguVolcanoB4, MapId.GurguVolcanoB5, MapId.IceCaveB1,
				MapId.IceCaveB2, MapId.IceCaveB3, MapId.MarshCaveB1, MapId.MarshCaveB2, MapId.MarshCaveB3, MapId.MirageTower1F, MapId.MirageTower2F,
				MapId.SeaShrineB1, MapId.SeaShrineB2, MapId.SeaShrineB3, MapId.SeaShrineB4, MapId.SeaShrineB5, MapId.SkyPalace1F,
				MapId.SkyPalace2F, MapId.SkyPalace3F, MapId.Waterfall, MapId.TempleOfFiends,
				// No teleporter definitions for ToFR
				// MapId.TempleOfFiendsRevisited1F, MapId.TempleOfFiendsRevisited2F, MapId.TempleOfFiendsRevisited3F, MapId.TempleOfFiendsRevisitedAir,
				// MapId.TempleOfFiendsRevisitedEarth,	MapId.TempleOfFiendsRevisitedFire, MapId.TempleOfFiendsRevisitedWater
			};

			// Select maps to flip
			validMaps.Shuffle(rng);
			var mapsToFlip = validMaps.GetRange(0, rng.Between((int)(validMaps.Count * 0.33), (int)(validMaps.Count * 0.75)));

			//var mapsToFlip = validMaps;


			foreach (MapId map in mapsToFlip)
			{
				maps[(int)map].FlipHorizontal();

				// Switch room wall tiles and some other wall tiles for look
				maps[(int)map].Replace(0x00, 0xFF);
				maps[(int)map].Replace(0x02, 0x00);
				maps[(int)map].Replace(0xFF, 0x02);

				maps[(int)map].Replace(0x03, 0xFF);
				maps[(int)map].Replace(0x05, 0x03);
				maps[(int)map].Replace(0xFF, 0x05);

				maps[(int)map].Replace(0x06, 0xFF);
				maps[(int)map].Replace(0x08, 0x06);
				maps[(int)map].Replace(0xFF, 0x08);

				maps[(int)map].Replace(0x32, 0xFF);
				maps[(int)map].Replace(0x33, 0x32);
				maps[(int)map].Replace(0xFF, 0x33);

				maps[(int)map].Replace(0x34, 0xFF);
				maps[(int)map].Replace(0x35, 0x34);
				maps[(int)map].Replace(0xFF, 0x35);

				// Flip NPCs position
				for (int i = 0; i < 16; i++)
				{
					var tempNpc = GetNpc(map, i);
					if (tempNpc.Coord != (0, 0))
					{
						tempNpc.Coord.x = 64 - tempNpc.Coord.x - 1;
						MoveNpc(map, tempNpc);
					}
				}
			}

			// Update entrance and teleporters coordinate, it has to be done manually for entrance/floor shuffles
			if (mapsToFlip.Contains(MapId.EarthCaveB1)) teleporters.EarthCave1.FlipXcoordinate(); overworld.PutOverworldTeleport(OverworldTeleportIndex.EarthCave1, teleporters.EarthCave1);
			if (mapsToFlip.Contains(MapId.EarthCaveB2)) teleporters.EarthCave2.FlipXcoordinate(); overworld.PutStandardTeleport(TeleportIndex.EarthCave2, teleporters.EarthCave2, OverworldTeleportIndex.EarthCave1);
			if (mapsToFlip.Contains(MapId.EarthCaveB3)) teleporters.EarthCaveVampire.FlipXcoordinate(); overworld.PutStandardTeleport(TeleportIndex.EarthCaveVampire, teleporters.EarthCaveVampire, OverworldTeleportIndex.EarthCave1);
			if (mapsToFlip.Contains(MapId.EarthCaveB4)) teleporters.EarthCave4.FlipXcoordinate(); overworld.PutStandardTeleport(TeleportIndex.EarthCave4, teleporters.EarthCave4, OverworldTeleportIndex.EarthCave1);
			if (mapsToFlip.Contains(MapId.EarthCaveB5)) teleporters.EarthCaveLich.FlipXcoordinate(); overworld.PutStandardTeleport(TeleportIndex.EarthCaveLich, teleporters.EarthCaveLich, OverworldTeleportIndex.EarthCave1);

			if (mapsToFlip.Contains(MapId.GurguVolcanoB1)) teleporters.GurguVolcano1.FlipXcoordinate(); overworld.PutOverworldTeleport(OverworldTeleportIndex.GurguVolcano1, teleporters.GurguVolcano1);
			if (mapsToFlip.Contains(MapId.GurguVolcanoB2)) teleporters.GurguVolcano2.FlipXcoordinate(); overworld.PutStandardTeleport(TeleportIndex.GurguVolcano2, teleporters.GurguVolcano2, OverworldTeleportIndex.GurguVolcano1);
			if (mapsToFlip.Contains(MapId.GurguVolcanoB3))
			{
				teleporters.GurguVolcano3.FlipXcoordinate(); overworld.PutStandardTeleport(TeleportIndex.GurguVolcano3, teleporters.GurguVolcano3, OverworldTeleportIndex.GurguVolcano1);
				teleporters.GurguVolcano5.FlipXcoordinate(); overworld.PutStandardTeleport(TeleportIndex.GurguVolcano5, teleporters.GurguVolcano5, OverworldTeleportIndex.GurguVolcano1);
			}
			if (mapsToFlip.Contains(MapId.GurguVolcanoB4))
			{
				teleporters.GurguVolcano4.FlipXcoordinate(); overworld.PutStandardTeleport(TeleportIndex.GurguVolcano4, teleporters.GurguVolcano4, OverworldTeleportIndex.GurguVolcano1);
				teleporters.GurguVolcano6.FlipXcoordinate(); overworld.PutStandardTeleport(TeleportIndex.GurguVolcano6, teleporters.GurguVolcano6, OverworldTeleportIndex.GurguVolcano1);
			}
			if (mapsToFlip.Contains(MapId.GurguVolcanoB5)) teleporters.GurguVolcanoKary.FlipXcoordinate(); overworld.PutStandardTeleport(TeleportIndex.GurguVolcanoKary, teleporters.GurguVolcanoKary, OverworldTeleportIndex.GurguVolcano1);

			if (mapsToFlip.Contains(MapId.IceCaveB1))
			{
				teleporters.IceCave1.FlipXcoordinate(); overworld.PutOverworldTeleport(OverworldTeleportIndex.IceCave1, teleporters.IceCave1);
				overworld.PutStandardTeleport(TeleportIndex.IceCave5, new TeleportDestination(MapLocation.IceCaveBackExit, MapIndex.IceCaveB1, new Coordinate(0x39, 0x14, CoordinateLocale.Standard), TeleportIndex.IceCave5), OverworldTeleportIndex.IceCave1);
			}
			if (mapsToFlip.Contains(MapId.IceCaveB2))
			{
				teleporters.IceCave2.FlipXcoordinate(); overworld.PutStandardTeleport(TeleportIndex.IceCave2, teleporters.IceCave2, OverworldTeleportIndex.IceCave1);
				teleporters.IceCavePitRoom.FlipXcoordinate(); overworld.PutStandardTeleport(TeleportIndex.IceCavePitRoom, teleporters.IceCavePitRoom, OverworldTeleportIndex.IceCave1);
				overworld.PutStandardTeleport(TeleportIndex.IceCave7, new TeleportDestination(MapLocation.IceCaveFloater, MapIndex.IceCaveB2, new Coordinate(0x0C, 0x0B, CoordinateLocale.StandardInRoom), TeleportIndex.IceCave7), OverworldTeleportIndex.IceCave1);
			}
			if (mapsToFlip.Contains(MapId.IceCaveB3))
			{
				teleporters.IceCave3.FlipXcoordinate(); overworld.PutStandardTeleport(TeleportIndex.IceCave3, teleporters.IceCave3, OverworldTeleportIndex.IceCave1);
				overworld.PutStandardTeleport(TeleportIndex.IceCave4, new TeleportDestination(MapLocation.IceCave3, MapIndex.IceCaveB3, new Coordinate(0x18, 0x06, CoordinateLocale.StandardInRoom), TeleportIndex.IceCave4), OverworldTeleportIndex.IceCave1);
				overworld.PutStandardTeleport(TeleportIndex.IceCave6, new TeleportDestination(MapLocation.IceCave3, MapIndex.IceCaveB3, new Coordinate(0x04, 0x21, CoordinateLocale.Standard), TeleportIndex.IceCave6), OverworldTeleportIndex.IceCave1);
			}

			if (mapsToFlip.Contains(MapId.MarshCaveB1)) teleporters.MarshCave1.FlipXcoordinate(); overworld.PutOverworldTeleport(OverworldTeleportIndex.MarshCave1, teleporters.MarshCave1);
			if (mapsToFlip.Contains(MapId.MarshCaveB2))
			{
				teleporters.MarshCaveTop.FlipXcoordinate(); overworld.PutStandardTeleport(TeleportIndex.MarshCaveTop, teleporters.MarshCaveTop, OverworldTeleportIndex.MarshCave1);
				teleporters.MarshCave3.FlipXcoordinate(); overworld.PutStandardTeleport(TeleportIndex.MarshCave3, teleporters.MarshCave3, OverworldTeleportIndex.MarshCave1);
			}
			if (mapsToFlip.Contains(MapId.MarshCaveB3)) teleporters.MarshCaveBottom.FlipXcoordinate(); overworld.PutStandardTeleport(TeleportIndex.MarshCaveBottom, teleporters.MarshCaveBottom, OverworldTeleportIndex.MarshCave1);

			if (mapsToFlip.Contains(MapId.MirageTower1F)) teleporters.MirageTower1.FlipXcoordinate(); overworld.PutOverworldTeleport(OverworldTeleportIndex.MirageTower1, teleporters.MirageTower1);
			if (mapsToFlip.Contains(MapId.MirageTower2F)) teleporters.MirageTower2.FlipXcoordinate(); overworld.PutStandardTeleport(TeleportIndex.MirageTower2, teleporters.MirageTower2, OverworldTeleportIndex.MirageTower1);

			if (mapsToFlip.Contains(MapId.SeaShrineB1)) teleporters.SeaShrineMermaids.FlipXcoordinate(); overworld.PutStandardTeleport(TeleportIndex.SeaShrineMermaids, teleporters.SeaShrineMermaids, OverworldTeleportIndex.Onrac);
			if (mapsToFlip.Contains(MapId.SeaShrineB2))
			{
				teleporters.SeaShrine2.FlipXcoordinate(); overworld.PutStandardTeleport(TeleportIndex.SeaShrine2, teleporters.SeaShrine2, OverworldTeleportIndex.Onrac);
				teleporters.SeaShrine6.FlipXcoordinate(); overworld.PutStandardTeleport(TeleportIndex.SeaShrine6, teleporters.SeaShrine6, OverworldTeleportIndex.Onrac);
			}
			if (mapsToFlip.Contains(MapId.SeaShrineB3))
			{
				teleporters.SeaShrine1.FlipXcoordinate(); overworld.PutStandardTeleport(TeleportIndex.SeaShrine1, teleporters.SeaShrine1, OverworldTeleportIndex.Onrac);
				teleporters.SeaShrine5.FlipXcoordinate(); overworld.PutStandardTeleport(TeleportIndex.SeaShrine5, teleporters.SeaShrine5, OverworldTeleportIndex.Onrac);
				teleporters.SeaShrine7.FlipXcoordinate(); overworld.PutStandardTeleport(TeleportIndex.SeaShrine7, teleporters.SeaShrine7, OverworldTeleportIndex.Onrac);
			}
			if (mapsToFlip.Contains(MapId.SeaShrineB4))
			{
				teleporters.SeaShrine4.FlipXcoordinate(); overworld.PutStandardTeleport(TeleportIndex.SeaShrine4, teleporters.SeaShrine4, OverworldTeleportIndex.Onrac);
				teleporters.SeaShrine8.FlipXcoordinate(); overworld.PutStandardTeleport(TeleportIndex.SeaShrine8, teleporters.SeaShrine8, OverworldTeleportIndex.Onrac);
			}
			if (mapsToFlip.Contains(MapId.SeaShrineB5)) teleporters.SeaShrineKraken.FlipXcoordinate(); overworld.PutStandardTeleport(TeleportIndex.SeaShrineKraken, teleporters.SeaShrineKraken, OverworldTeleportIndex.Onrac);

			if (mapsToFlip.Contains(MapId.SkyPalace1F)) teleporters.SkyPalace1.FlipXcoordinate(); overworld.PutStandardTeleport(TeleportIndex.SkyPalace1, teleporters.SkyPalace1, OverworldTeleportIndex.MirageTower1);
			if (mapsToFlip.Contains(MapId.SkyPalace2F)) teleporters.SkyPalace2.FlipXcoordinate(); overworld.PutStandardTeleport(TeleportIndex.SkyPalace2, teleporters.SkyPalace2, OverworldTeleportIndex.MirageTower1);
			if (mapsToFlip.Contains(MapId.SkyPalace3F)) teleporters.SkyPalace3.FlipXcoordinate(); overworld.PutStandardTeleport(TeleportIndex.SkyPalace3, teleporters.SkyPalace3, OverworldTeleportIndex.MirageTower1);

			if (mapsToFlip.Contains(MapId.TempleOfFiends)) teleporters.TempleOfFiends.FlipXcoordinate(); overworld.PutOverworldTeleport(OverworldTeleportIndex.TempleOfFiends1, teleporters.TempleOfFiends);

			if (mapsToFlip.Contains(MapId.Waterfall)) teleporters.Waterfall.FlipXcoordinate(); overworld.PutOverworldTeleport(OverworldTeleportIndex.Waterfall, teleporters.Waterfall);

			return mapsToFlip;
		}
		public void WarMECHNpc(WarMECHMode mode, NPCdata npcpdata, MT19337 rng, List<Map> maps)
		{
			const byte UnusedTextPointer = 0xF7;
			const byte WarMECHEncounter = 0x56;
			const byte RobotGfx = 0x15;

			// Set up the map object.
			npcpdata.GetTalkArray(ObjectId.WarMECH)[(int)TalkArrayPos.dialogue_2] = UnusedTextPointer;
			npcpdata.GetTalkArray(ObjectId.WarMECH)[(int)TalkArrayPos.battle_id] = WarMECHEncounter;
			Data[MapObjGfxOffset + (byte)ObjectId.WarMECH] = RobotGfx;

			// Set the action when you talk to WarMECH.
			npcpdata.SetRoutine(ObjectId.WarMECH, newTalkRoutines.Talk_fight);

			// Change the dialogue.
			var dialogueStrings = new List<string>
			{
				"I. aM. WarMECH.",
				"Give me the AllSpark,\nand you may live to be\nmy pet!",
				"I'm afraid that's\nsomething I cannot allow\nto happen, Dave.",
				"Bite my shiny metal ass!",
				"Put down your weapons.\nYou have 15 seconds to\ncomply.",
				"rEsIsTaNcE iS fUtIlE.",
				"Hasta la vista, baby.",
				"NoOo DiSaSsEmBlE!",
				"Bring back life form.\nPriority one.\nAll other priorities\nrescinded."
			};

			InsertDialogs(UnusedTextPointer, dialogueStrings.PickRandom(rng));

			// Get rid of random WarMECH encounters.  Group 8 is now also group 7.
			var formationOffset = ZoneFormationsOffset + ZoneFormationsSize * (64 + (byte)MapId.SkyPalace5F);
			var formations = Get(formationOffset, ZoneFormationsSize);
			formations[6] = formations[7];
			Put(formationOffset, formations);

			if (mode != WarMECHMode.Unleashed)
				MakeWarMECHUnrunnable();

			if (mode == WarMECHMode.Required)
			{
				// Can't use mapNpcIndex 0, that's the Wind ORB.
				SetNpc(MapId.SkyPalace5F, 1, ObjectId.WarMECH, 0x07, 0x0E, inRoom: false, stationary: true);

				Data[0x029AB] = 0x14; // we can only change one color without messing up the Wind ORB.
			}
			else if (mode == WarMECHMode.Patrolling)
			{
				var (x, y) = GetSkyCastleFloorTile(rng, maps[(byte)MapId.SkyPalace4F]);
				SetNpc(MapId.SkyPalace4F, 0, ObjectId.WarMECH, x, y, inRoom: false, stationary: false);

				// We can change all the colors here.
				Put(0x02978, Blob.FromHex("0F0F18140F0F1714"));
			}
		}

		public void MoveNpc(MapId mapId, NPC npc)
		{
			MoveNpc(mapId, npc.Index, npc.Coord.x, npc.Coord.y, npc.InRoom, npc.Stationary);
		}

		public void MoveNpc(MapId mapId, int mapNpcIndex, int x, int y, bool inRoom, bool stationary)
		{
			int offset = MapSpriteOffset + ((byte)mapId * MapSpriteCount + mapNpcIndex) * MapSpriteSize;

			byte firstByte = (byte)x;
			firstByte |= (byte)(inRoom ? 0x80 : 0x00);
			firstByte |= (byte)(stationary ? 0x40 : 0x00);

			Data[offset + 1] = firstByte;
			Data[offset + 2] = (byte)y;
		}

		public void SetNpc(MapId mapId, int mapNpcIndex, ObjectId mapObjId, int x, int y, bool inRoom, bool stationary)
		{
			int offset = MapSpriteOffset + ((byte)mapId * MapSpriteCount + mapNpcIndex) * MapSpriteSize;

			byte firstByte = (byte)x;
			firstByte |= (byte)(inRoom ? 0x80 : 0x00);
			firstByte |= (byte)(stationary ? 0x40 : 0x00);

			Data[offset] = (byte)mapObjId;
			Data[offset + 1] = firstByte;
			Data[offset + 2] = (byte)y;
		}

		public NPC FindNpc(MapId mapId, ObjectId mapObjId)
		{
			var tempNPC = new NPC();

			for (int i = 0; i < MapSpriteCount; i++)
			{
				int offset = MapSpriteOffset + ((byte)mapId * MapSpriteCount + i) * MapSpriteSize;

				if (Data[offset] == (byte)mapObjId)
				{
					tempNPC.ObjectId = (ObjectId)Data[offset];
					tempNPC.Index = i;
					tempNPC.Coord = (Data[offset + 1] & 0x3F, Data[offset + 2]);
					tempNPC.InRoom = (Data[offset + 1] & 0x80) > 0;
					tempNPC.Stationary = (Data[offset + 1] & 0x40) > 0;
					break;
				}
			}
			return tempNPC;
		}

		public IEnumerable<(MapId, NPC)> FindNpc(ObjectId mapObjId)
		{
			var tempNPC = new NPC();

			//not good, but quick
			foreach (var mid in Enum.GetValues<MapId>())
			{
				for (int i = 0; i < MapSpriteCount; i++)
				{
					int offset = MapSpriteOffset + ((byte)mid * MapSpriteCount + i) * MapSpriteSize;

					if (Data[offset] == (byte)mapObjId)
					{
						tempNPC.Index = i;
						tempNPC.Coord = (Data[offset + 1] & 0x3F, Data[offset + 2]);
						tempNPC.InRoom = (Data[offset + 1] & 0x80) > 0;
						tempNPC.Stationary = (Data[offset + 1] & 0x40) > 0;

						yield return (mid, tempNPC);
					}
				}
			}
		}


		public NPC GetNpc(MapId mapId, int position)
		{
			var tempNPC = new NPC();

			int offset = MapSpriteOffset + ((byte)mapId * MapSpriteCount + position) * MapSpriteSize;

			tempNPC.ObjectId = (ObjectId)Data[offset];
			tempNPC.Index = position;
			tempNPC.Coord = (Data[offset + 1] & 0x3F, Data[offset + 2]);
			tempNPC.InRoom = (Data[offset + 1] & 0x80) > 0;
			tempNPC.Stationary = (Data[offset + 1] & 0x40) > 0;

			return tempNPC;
		}
		public List<Map> ReadMaps()
		{
			var pointers = Get(MapPointerOffset, MapCount * MapPointerSize).ToUShorts();

			return pointers.Select(pointer => new Map(Get(MapPointerOffset + pointer, Map.RowCount * Map.RowLength))).ToList();
		}

		public void WriteMaps(List<Map> maps)
		{
			var data = maps.Select(map => map.GetCompressedData()).ToList();

			var pointers = new ushort[MapCount];
			pointers[0] = MapDataOffset - MapPointerOffset;
			for (int i = 1; i < MapCount; i++)
			{
				pointers[i] = (ushort)(pointers[i - 1] + data[i - 1].Length);
			}

			Put(MapPointerOffset, Blob.FromUShorts(pointers));
			for (int i = 0; i < MapCount; i++)
			{
				Put(MapPointerOffset + pointers[i], data[i]);
			}
		}

		public void ExpandNormalTeleporters()
		{
			// Code for extension is included in ExtraTrackingAndInitCode() in FF1Rom.cs
			//  see 0F_9200_TeleportXYInroom.asm
			const int BANK_TELEPORTINFO = 0x00;
			const int BANK_EXTTELEPORTINFO = 0x0F;

			const int lut_NormTele_X = 0xAD00;
			const int lut_NormTele_Y = 0xAD40;
			const int lut_NormTele_Map = 0xAD80;
			const int NormTele_qty = 0x40;

			const int lut_NormTele_X_ext = 0xB000;
			const int lut_NormTele_Y_ext = 0xB100;
			const int lut_NormTele_Map_ext = 0xB200;
			//const int NormTele_ext_qty = 0x100;

			var NormTele_X = GetFromBank(BANK_TELEPORTINFO, lut_NormTele_X, NormTele_qty);
			var NormTele_Y = GetFromBank(BANK_TELEPORTINFO, lut_NormTele_Y, NormTele_qty);
			var NormTele_Map = GetFromBank(BANK_TELEPORTINFO, lut_NormTele_Map, NormTele_qty);

			PutInBank(BANK_EXTTELEPORTINFO, lut_NormTele_X_ext, NormTele_X);
			PutInBank(BANK_EXTTELEPORTINFO, lut_NormTele_Y_ext, NormTele_Y);
			PutInBank(BANK_EXTTELEPORTINFO, lut_NormTele_Map_ext, NormTele_Map);



		}

		public void BahamutB1Encounters(List<Map> maps)
		{
			// Adds dragon-themed encounters to the long
			// hallway to Bahamut's room

			var bahamutB1ZoneOffset = ZoneFormationsOffset + (ZoneFormationsSize * (64 + (int)MapId.BahamutsRoomB1));
			var formation = Get(bahamutB1ZoneOffset, ZoneFormationsSize);
			formation[0] = 0x2A + 0x80; // 2-4 Red D
			formation[1] = 0x30 + 0x80; // 3-4 Frost D
			formation[2] = 0x4B + 0x80; // 2-4 Zombie D
			formation[3] = 0x4E + 0x80; // 2-3 Blue D
			formation[4] = 0x59 + 0x80; // 2-4 Gas D
			formation[5] = 0x4E + 0x80; // 2-3 Blue D
			formation[6] = 0x59 + 0x80; // 2-4 Gas D
			formation[7] = 0x77; // Tiamat 1 (!)
			Put(bahamutB1ZoneOffset, formation);

			Put(ThreatLevelsOffset + (int)MapId.BahamutsRoomB1 + 1, Blob.FromHex("18"));

			maps[(byte)MapId.BahamutsRoomB1][1, 1] = (byte)Tile.CardiaEncounters;
			maps[(byte)MapId.BahamutsRoomB1][1, 2] = (byte)Tile.CardiaEncounters;
			maps[(byte)MapId.BahamutsRoomB1][1, 3] = (byte)Tile.CardiaEncounters;
			maps[(byte)MapId.BahamutsRoomB1][2, 1] = (byte)Tile.CardiaEncounters;
			maps[(byte)MapId.BahamutsRoomB1][2, 3] = (byte)Tile.CardiaEncounters;
			maps[(byte)MapId.BahamutsRoomB1][3, 1] = (byte)Tile.CardiaEncounters;
			maps[(byte)MapId.BahamutsRoomB1][3, 2] = (byte)Tile.CardiaEncounters;
			maps[(byte)MapId.BahamutsRoomB1][3, 3] = (byte)Tile.CardiaEncounters;
			for (int i = 4; i <= 0x32; i++)
			{
				maps[(byte)MapId.BahamutsRoomB1][i, 2] = (byte)Tile.CardiaEncounters;
			}
			maps[(byte)MapId.BahamutsRoomB1][0x33, 1] = (byte)Tile.CardiaEncounters;
			maps[(byte)MapId.BahamutsRoomB1][0x33, 2] = (byte)Tile.CardiaEncounters;
			maps[(byte)MapId.BahamutsRoomB1][0x33, 3] = (byte)Tile.CardiaEncounters;
			maps[(byte)MapId.BahamutsRoomB1][0x34, 1] = (byte)Tile.CardiaEncounters;
			maps[(byte)MapId.BahamutsRoomB1][0x34, 2] = (byte)Tile.CardiaEncounters;
			maps[(byte)MapId.BahamutsRoomB1][0x34, 3] = (byte)Tile.CardiaEncounters;
			maps[(byte)MapId.BahamutsRoomB1][0x35, 1] = (byte)Tile.CardiaEncounters;
			maps[(byte)MapId.BahamutsRoomB1][0x35, 3] = (byte)Tile.CardiaEncounters;
			maps[(byte)MapId.BahamutsRoomB1][0x36, 1] = (byte)Tile.CardiaEncounters;
			maps[(byte)MapId.BahamutsRoomB1][0x36, 2] = (byte)Tile.CardiaEncounters;
			maps[(byte)MapId.BahamutsRoomB1][0x36, 3] = (byte)Tile.CardiaEncounters;
		}

		public void DragonsHoard(List<Map> maps, bool enable)
		{
			// Replaces the area around/behind Bahamut with
			// all the Cardia chests.  (Does not delete the
			// original chests, they will be linked)

			if (enable)
			{
				maps[(byte)MapId.BahamutsRoomB2][1, 17] = (byte)Tile.CardiaCandles;
				maps[(byte)MapId.BahamutsRoomB2][1, 18] = (byte)Tile.CardiaChest1;
				maps[(byte)MapId.BahamutsRoomB2][1, 19] = (byte)Tile.CardiaChest2;
				maps[(byte)MapId.BahamutsRoomB2][1, 20] = (byte)Tile.CardiaChest3;
				maps[(byte)MapId.BahamutsRoomB2][1, 21] = (byte)Tile.CardiaChest4;
				maps[(byte)MapId.BahamutsRoomB2][1, 22] = (byte)Tile.CardiaChest5;
				maps[(byte)MapId.BahamutsRoomB2][1, 23] = (byte)Tile.CardiaChest6;
				maps[(byte)MapId.BahamutsRoomB2][1, 24] = (byte)Tile.CardiaChest7;
				maps[(byte)MapId.BahamutsRoomB2][1, 25] = (byte)Tile.CardiaCandles;

				maps[(byte)MapId.BahamutsRoomB2][2, 17] = (byte)Tile.CardiaChest8;
				maps[(byte)MapId.BahamutsRoomB2][2, 18] = (byte)Tile.CardiaFloor;
				maps[(byte)MapId.BahamutsRoomB2][2, 19] = (byte)Tile.CardiaFloor;
				maps[(byte)MapId.BahamutsRoomB2][2, 20] = (byte)Tile.CardiaFloor;
				maps[(byte)MapId.BahamutsRoomB2][2, 21] = (byte)Tile.CardiaFloor;
				maps[(byte)MapId.BahamutsRoomB2][2, 22] = (byte)Tile.CardiaFloor;
				maps[(byte)MapId.BahamutsRoomB2][2, 23] = (byte)Tile.CardiaFloor;
				maps[(byte)MapId.BahamutsRoomB2][2, 24] = (byte)Tile.CardiaFloor;
				maps[(byte)MapId.BahamutsRoomB2][2, 25] = (byte)Tile.CardiaChest9;

				maps[(byte)MapId.BahamutsRoomB2][3, 17] = (byte)Tile.CardiaCandles;
				maps[(byte)MapId.BahamutsRoomB2][3, 18] = (byte)Tile.CardiaChest10;
				maps[(byte)MapId.BahamutsRoomB2][3, 19] = (byte)Tile.CardiaFloor;
				maps[(byte)MapId.BahamutsRoomB2][3, 20] = (byte)Tile.CardiaFloor;
				maps[(byte)MapId.BahamutsRoomB2][3, 21] = (byte)Tile.CardiaFloor;
				maps[(byte)MapId.BahamutsRoomB2][3, 22] = (byte)Tile.CardiaFloor;
				maps[(byte)MapId.BahamutsRoomB2][3, 23] = (byte)Tile.CardiaFloor;
				maps[(byte)MapId.BahamutsRoomB2][3, 24] = (byte)Tile.CardiaChest11;
				maps[(byte)MapId.BahamutsRoomB2][3, 25] = (byte)Tile.CardiaCandles;

				maps[(byte)MapId.BahamutsRoomB2][4, 17] = (byte)Tile.CardiaFloor;
				maps[(byte)MapId.BahamutsRoomB2][4, 18] = (byte)Tile.CardiaCandles;
				maps[(byte)MapId.BahamutsRoomB2][4, 19] = (byte)Tile.CardiaChest12;
				maps[(byte)MapId.BahamutsRoomB2][4, 20] = (byte)Tile.CardiaFloor;
				maps[(byte)MapId.BahamutsRoomB2][4, 21] = (byte)Tile.CardiaFloor;
				maps[(byte)MapId.BahamutsRoomB2][4, 22] = (byte)Tile.CardiaFloor;
				maps[(byte)MapId.BahamutsRoomB2][4, 23] = (byte)Tile.CardiaChest13;
				maps[(byte)MapId.BahamutsRoomB2][4, 24] = (byte)Tile.CardiaCandles;
				maps[(byte)MapId.BahamutsRoomB2][4, 25] = (byte)Tile.CardiaFloor;

				ItemLocations.Cardia1.ChangeMapLocation(MapLocation.BahamutCave2);
				ItemLocations.Cardia2.ChangeMapLocation(MapLocation.BahamutCave2);
				ItemLocations.Cardia3.ChangeMapLocation(MapLocation.BahamutCave2);
				ItemLocations.Cardia4.ChangeMapLocation(MapLocation.BahamutCave2);
				ItemLocations.Cardia5.ChangeMapLocation(MapLocation.BahamutCave2);
				ItemLocations.Cardia6.ChangeMapLocation(MapLocation.BahamutCave2);
				ItemLocations.Cardia7.ChangeMapLocation(MapLocation.BahamutCave2);
				ItemLocations.Cardia8.ChangeMapLocation(MapLocation.BahamutCave2);
				ItemLocations.Cardia9.ChangeMapLocation(MapLocation.BahamutCave2);
				ItemLocations.Cardia10.ChangeMapLocation(MapLocation.BahamutCave2);
				ItemLocations.Cardia11.ChangeMapLocation(MapLocation.BahamutCave2);
				ItemLocations.Cardia12.ChangeMapLocation(MapLocation.BahamutCave2);
				ItemLocations.Cardia13.ChangeMapLocation(MapLocation.BahamutCave2);
			}
			else
			{
				// If the user enabled Dragon's hoard,
				// generated a seed, and then turned it off
				// again, the locations will remain changed.
				ItemLocations.Cardia1.ChangeMapLocation(MapLocation.Cardia6);
				ItemLocations.Cardia2.ChangeMapLocation(MapLocation.Cardia6);
				ItemLocations.Cardia3.ChangeMapLocation(MapLocation.Cardia6);
				ItemLocations.Cardia4.ChangeMapLocation(MapLocation.Cardia6);
				ItemLocations.Cardia5.ChangeMapLocation(MapLocation.Cardia6);
				ItemLocations.Cardia6.ChangeMapLocation(MapLocation.Cardia4);
				ItemLocations.Cardia7.ChangeMapLocation(MapLocation.Cardia4);
				ItemLocations.Cardia8.ChangeMapLocation(MapLocation.Cardia4);
				ItemLocations.Cardia9.ChangeMapLocation(MapLocation.Cardia2);
				ItemLocations.Cardia10.ChangeMapLocation(MapLocation.Cardia2);
				ItemLocations.Cardia11.ChangeMapLocation(MapLocation.Cardia2);
				ItemLocations.Cardia12.ChangeMapLocation(MapLocation.Cardia6);
				ItemLocations.Cardia13.ChangeMapLocation(MapLocation.Cardia6);
			}
		}

		public void MermaidPrison(List<Map> maps, bool enable)
		{
			if (enable)
			{
				for (int y = 0; y < 64; y++)
					for (int x = 0; x < 64; x++)
					{
						if (maps[(byte)MapId.SeaShrineB1][x, y] == (byte)Tile.Door) maps[(byte)MapId.SeaShrineB1][x, y] = (byte)Tile.DoorLocked;
					}

				// Have locked rooms draw inside NPCs, instead of outside NPCs
				PutInBank(0x1F, 0xCEDE, new byte[] { 0x81 });
			}
		}

		class Room {
		    public MapId mapId;
		    public MapElement start;
		    public List<MapElement> floor = new();
		    public List<MapElement> doors = new ();
		    public List<MapElement> chests = new ();
		    public List<MapElement> teleIn = new ();
		    public List<MapElement> teleOut = new ();
		    public List<MapElement> npcs = new ();
		    public List<MapElement> killablenpcs = new ();
		    public bool hasBattles = false;
		    public Room() { }
		    public Room(Room copyfrom) {
			mapId = copyfrom.mapId;
			start = copyfrom.start;
			floor = new List<MapElement>(copyfrom.floor);
			doors = new List<MapElement>(copyfrom.doors);
			chests = new List<MapElement>(copyfrom.chests);
			teleIn = new List<MapElement>(copyfrom.teleIn);
			teleOut = new List<MapElement>(copyfrom.teleOut);
			npcs = new List<MapElement>(copyfrom.npcs);
			killablenpcs = new List<MapElement>(copyfrom.killablenpcs);
			hasBattles = copyfrom.hasBattles;
		    }
		    public void Replace(Room copyfrom) {
			mapId = copyfrom.mapId;
			start = copyfrom.start;
			floor.Clear();
			floor.AddRange(copyfrom.floor);

			doors.Clear();
			doors.AddRange(copyfrom.doors);

			chests.Clear();
			chests.AddRange(copyfrom.chests);

			teleIn.Clear();
			teleIn.AddRange(copyfrom.teleIn);

			teleOut.Clear();
			teleOut.AddRange(copyfrom.teleOut);

			npcs.Clear();
			npcs.AddRange(copyfrom.npcs);

			killablenpcs.Clear();
			killablenpcs.AddRange(copyfrom.killablenpcs);

			hasBattles = copyfrom.hasBattles;
		    }
		}

		public async Task shuffleChestLocations(MT19337 rng, List<Map> maps, MapId[] ids, List<(MapId,byte)> preserveChests,
							NPCdata npcdata, byte randomEncounter, bool spreadPlacement, bool markSpikeTiles) {
		    // For a tileset, I need to determine:
		    //
		    // * doors and locked doors
		    // * floor tiles with the move bit that are empty.

		    bool debug = false;

		    if (debug) Console.WriteLine($"\nTiles for {ids[0]}");

		    bool keepLinkedChests = false;

		    var tileset = new TileSet(this, GetMapTilesetIndex(ids[0]));

		    List<byte> blankPPUTiles = new();

		    for (byte i = 0; i < 128; i++) {
			// Go through the pattern table, and find the CHR which
			// is entirely color 1.

			var chr = Get(TILESETPATTERNTABLE_OFFSET + (GetMapTilesetIndex(ids[0]) << 11) + (i * 16), 16);
			var dec = DecodePPU(chr);

			bool blank = true;
			// Check that entire tile is color 1
			for (int j = 0; j < dec.Length; j++) {
			    if (dec[j] != 1) {
				blank = false;
				break;
			    }
			}
			if (blank) {
			    blankPPUTiles.Add(i);
			}
		    }
		    foreach(var b in blankPPUTiles) {
			if (debug) Console.WriteLine($"blank chr {b:X}");
		    }

		    // Go through the all the map tiles and find ones
		    // with certain properties.
		    List<byte> doorTiles = new() {0x36, 0x37, 0x3B};
		    List<byte> floorTiles = new();
		    List<byte> spikeTiles = new();
		    List<byte> battleTiles = new();

		    for (byte i = 0; i < 128; i++) {
			if ((tileset.TileProperties[i].TilePropFunc & TilePropFunc.TP_NOMOVE) != 0) {
			    continue;
			}
			// Allowed to walk onto this tile

			if ((tileset.TileAttributes[i] & 3) != 1 &&
			    (tileset.TileAttributes[i] & 3) != 2) {
			    continue;
			}
			// This tile has the "room" palette

			if (!blankPPUTiles.Contains(tileset.TopLeftTiles[i]) ||
			    !blankPPUTiles.Contains(tileset.TopRightTiles[i]) ||
			    !blankPPUTiles.Contains(tileset.BottomLeftTiles[i]) ||
			    !blankPPUTiles.Contains(tileset.BottomRightTiles[i]))
			{
			    continue;
			}
			// The visual tile is "blank" (flips between
			// floor color (black) and ceiling color
			// (generally white/off white).

			if (tileset.TileProperties[i].TilePropFunc == TilePropFunc.TP_SPEC_BATTLE) {
			    if (tileset.TileProperties[i].BattleId != randomEncounter) {
				// This is a spike tile
				spikeTiles.Add(i);
				if (debug) Console.WriteLine($"spike tile {i:X} BattleId {tileset.TileProperties[i].BattleId:X}");
			    } else {
				// This is random battle tile
				battleTiles.Add(i);
				if (debug) Console.WriteLine($"battle tile {i:X}");
			    }
			    continue;
			}

			// Found a plain, no-encounter floor tile
			floorTiles.Add(i);
			if (debug) Console.WriteLine($"floor tile {i:X}");
		    }

		    byte vanillaTeleporters = 0x41;
		    List<TeleporterSM> teleporters = new();
		    for (int i = 0; i < vanillaTeleporters; i++) {
			teleporters.Add(new TeleporterSM(this, i));
		    }

		    List<byte> chestPool = new();
		    List<byte> spikePool = new();

		    // To relocate chests in a dungeon (a group of maps)
		    //
		    // * Find the all the chest tiles and spike tiles
		    // * Wipe all the tiles with the floor tile in the tileset
		    // * Find all the doors
		    // * For each door, flood fill search for floor tiles, other doors, and teleports & record what we found
		    // * Also record the positions of NPCs

		    List<Room> rooms = new();

		    foreach (var mapId in ids) {
			if (debug) Console.WriteLine($"\nFinding rooms for map {mapId}");

			var map = maps[(int)mapId];

			List<SCCoords> startCoords = new();
			for (int y = 0; y < 64; y++) {
			    for (int x = 0; x < 64; x++) {
				var tf = tileset.TileProperties[map[y, x]];
				bool wipe = false;

				if (tf.TilePropFunc == (TilePropFunc.TP_SPEC_TREASURE | TilePropFunc.TP_NOMOVE)) {
				    bool skip = false;
				    foreach (var pc in preserveChests) {
					if (mapId == pc.Item1 && pc.Item2 == map[y, x]) {
					    skip = true;
					}
				    }
				    if (!skip) {
					if (keepLinkedChests || !chestPool.Contains(map[y, x])) {
					    chestPool.Add(map[y, x]);
					}
					if (debug) Console.WriteLine($"add {map[y, x]:X} to chest pool");
					wipe = true;
				    }
				}
				if (spikeTiles.Contains(map[y, x])) {
				    spikePool.Add(map[y, x]);
				    if (debug) Console.WriteLine($"add {map[y, x]:X} to spike pool");
				    wipe = true;
				}
				if (doorTiles.Contains(map[y, x])) {
				    startCoords.Add(new SCCoords(x, y-1));
				    if (debug) Console.WriteLine($"Found door {map[y, x]:X} at {x},{y}");
				}

				if (wipe) {
				    map[y, x] = 0xff;
				}
			    }
			}

			foreach (var t in teleporters) {
			    if (t.Destination == (byte)mapId &&
				(floorTiles.Contains(map[t.Y,t.X]) || battleTiles.Contains(map[t.Y,t.X])))
			    {
				startCoords.Add(new SCCoords(t.X, t.Y));
			    }
			}

			List<(int x, int y)> searched = new();
			foreach (var st in startCoords) {
			    var room = new Room();
			    room.mapId = mapId;
			    room.start = map[(st.X, st.Y)];
			    bool newRoom = true;
			    if (debug) Console.WriteLine($"Searching from {st}");

			    var logit = false;

			    map.Flood((st.X, st.Y), (MapElement me) => {

				if (logit) {
				    if (debug) Console.WriteLine($"Search {me.Coord} {me.Value:X}");
				}

				bool hasNpc = false;
				bool hasKillableNpc = false;
				for (int i = 0; i < 16; i++) {
				    var npc = GetNpc(mapId, i);
				    if (npc.Coord == me.Coord) {
					hasNpc = true;
					hasKillableNpc = (npcdata.GetRoutine(npc.ObjectId) == newTalkRoutines.Talk_fight ||
							  npcdata.GetRoutine(npc.ObjectId) == newTalkRoutines.Talk_kill);
					room.npcs.Add(me);
					if (hasKillableNpc) {
					    room.killablenpcs.Add(me);
					}
				    }
				}

				if (me.Value == 0xff) {
				    // This square previously contained
				    // a chest or spike tile
				    if (!hasNpc) {
					// not occupied by NPC
					room.floor.Add(me);
					return true;
				    }
				    // Is occupied by an NPC, but if killable, we search past it.
				    return hasKillableNpc;
				}

				if (tileset.TileProperties[me.Value].TilePropFunc == (TilePropFunc.TP_SPEC_TREASURE | TilePropFunc.TP_NOMOVE)) {
				    // A preserved chest (vanilla
				    // incentive location) needs to be
				    // included in logic checking so
				    // we don't accept a placement
				    // that puts another chest
				    // blocking it.
				    room.chests.Add(me);
				    return false;
				}

				if (doorTiles.Contains(me.Value)) {
				    room.doors.Add(me);
				    if (searched.Contains(me.Coord)) {
					if (debug) Console.WriteLine($"Saw {me.Coord} already");
					newRoom = false;
				    } else {
					if (debug) Console.WriteLine($"{me.Coord} is new door");
					searched.Add(me.Coord);
				    }
				    return false;
				}

				bool teleporterTarget = false;
				foreach (var t in teleporters) {
				    if (t.Destination == (byte)mapId && me.Coord == (t.X, t.Y)) {
					if (debug) Console.WriteLine($"Found teleport in {me.Coord}");
					room.teleIn.Add(me);
					teleporterTarget = true;
				    }
				}

				if (tileset.TileProperties[me.Value].TilePropFunc == TilePropFunc.TP_SPEC_BATTLE &&
				    tileset.TileProperties[me.Value].BattleId == randomEncounter)
				{
				    room.hasBattles = true;
				}

				if (!hasNpc && !teleporterTarget && (floorTiles.Contains(me.Value) || battleTiles.Contains(me.Value) || spikeTiles.Contains(me.Value)) &&
				    (me.Coord != (st.X, st.Y)))
				{
				    room.floor.Add(me);
				}

				if ((tileset.TileProperties[me.Value].TilePropFunc & TilePropFunc.TP_TELE_MASK) != 0) {
				    if (debug) Console.WriteLine($"Found teleport out {me.Coord}");
				    room.teleOut.Add(me);
				    return false;
				}

				return (!hasNpc || hasKillableNpc) &&
				    (tileset.TileProperties[me.Value].TilePropFunc & TilePropFunc.TP_NOMOVE) == 0;
			    });
			    if (newRoom) {
				if (debug) Console.WriteLine($"Added new room");
				rooms.Add(room);
			    }

			    foreach (var me in room.floor) {
				if (me.Value == 0xff) {
				    if ((room.hasBattles && battleTiles.Count > 0) || floorTiles.Count == 0) {
					me.Value = battleTiles[0];
				    } else {
					me.Value = floorTiles[0];
				    }
				}
			    }
			}

			for (int y = 0; y < 64; y++) {
			    for (int x = 0; x < 64; x++) {
				if (map[y,x] == 0xff) {
				    if (floorTiles.Count > 0) {
					map[y,x] = floorTiles[0];
				    } else {
					map[y,x] = battleTiles[0];
				    }
				}
			    }
			}
		    }

		    // make a copy of the rooms
		    List<Room> workingrooms = new(rooms.Count);
		    foreach (var r in rooms) {
			workingrooms.Add(new Room());
		    }

		    bool needRetry = true;
		    List<(Room,MapElement)> placedChests = new();
		    List<Room> roomsToSanityCheck = new();
		    List<(Room,MapElement)> allCandidates = new();

		    int attempts;
		    await this.Progress($"Relocating chests for group of floors containing {ids[0]}", 1);
		    for (attempts = 0; needRetry && attempts < 800; attempts++) {
			if (attempts % 20 == 0) {
			    await this.Progress("", 20);
			    System.GC.Collect(System.GC.MaxGeneration);
			}

			// Make a copy of the rooms
			placedChests.Clear();
			roomsToSanityCheck.Clear();
			needRetry = false;

			for (int i = 0; i < rooms.Count; i++) {
			    // We do this goofy replacement thing
			    // instead of just allocating new objects
			    // because we need this inner loop to be
			    // as close to constant memory usage as
			    // possible, otherwise it'll fall over as
			    // the number of iterations gets large and
			    // it starts grabbing more and more
			    // memory.
			    workingrooms[i].Replace(rooms[i]);
			}

			allCandidates.Clear();
			foreach (var r in workingrooms) {
			    foreach (var f in r.floor) {
				//f.Value = 0xD;
				allCandidates.Add((r, f));
			    }
			}

			if (debug) Console.WriteLine($"rooms {rooms.Count}");

			foreach (var c in chestPool) {
			    (Room,MapElement) me;
			    if (spreadPlacement) {
				Room r;
				do {
				    r = workingrooms.PickRandom(rng);
				} while (r.floor.Count == 0);
				me = (r, r.floor.SpliceRandom(rng));
			    } else {
				// full random
				me = allCandidates.SpliceRandom(rng);
			    }
			    me.Item2.Value = c;
			    me.Item1.chests.Add(me.Item2);
			    placedChests.Add((me.Item1, me.Item2));
			    if (!roomsToSanityCheck.Contains(me.Item1)) {
				roomsToSanityCheck.Add(me.Item1);
			    }
			}

			foreach (var r in roomsToSanityCheck) {
			    r.start.Map.Flood((r.start.X, r.start.Y), (MapElement me) => {
				if (r.doors.Remove(me) || r.chests.Remove(me) || r.teleOut.Remove(me) || r.npcs.Remove(me)) {
				    // found something, don't traverse
				    if (!r.killablenpcs.Contains(me)) {
					return false;
				    }
				}
				r.teleIn.Remove(me);
				return (tileset.TileProperties[me.Value].TilePropFunc & TilePropFunc.TP_NOMOVE) == 0;
			    });
			    if (r.doors.Count > 0 || r.chests.Count > 0 || r.teleOut.Count > 0 || r.npcs.Count > 0) {
				if (debug) Console.WriteLine($"Room at {r.mapId} {r.start.X}, {r.start.Y} failed sanity check: {r.doors.Count} {r.chests.Count} {r.teleOut.Count} {r.npcs.Count}");
				needRetry = true;
				break;
			    }
			}

			if (needRetry) {
			    // Clear failed chest attempt
			    foreach (var ch in placedChests) {
				if ((ch.Item1.hasBattles && battleTiles.Count > 0) || floorTiles.Count == 0) {
				    ch.Item2.Value = battleTiles[0];
				} else {
				    ch.Item2.Value = floorTiles[0];
				}
			    }
			}
		    }

		    if (!needRetry) {
			Console.WriteLine($"{ids[0]} success after {attempts} attempts");
		    }

		    if (needRetry) {
			throw new Exception($"{ids[0]} Couldn't place chests after {attempts} attempts");
		    }

		    // Finally, add spike tiles.
		    if (debug) Console.WriteLine($"spikes {placedChests.Count} {spikePool.Count}");

		    if (placedChests.Count == 0) {
			return;
		    }

		    if (spikePool.Count > placedChests.Count)  {
			Console.WriteLine($"WARNING spikePool.Count > placedChests.Count something is wrong");
			return;
		    }

		    if (markSpikeTiles) {
			var ts = GetMapTilesetIndex(ids[0]);
			foreach (var sp in spikePool) {
			    tileset.TopRightTiles[sp] = 0x7D;
			}
			tileset.TopRightTiles.StoreTable();
		    }

		    var directions = new Direction[] { Direction.Down, Direction.Down, Direction.Down,
			Direction.Right, Direction.Left, Direction.Up };
		    while (spikePool.Count > 0) {
			var pc = placedChests.PickRandom(rng);
			var dir = directions.PickRandom(rng);
			var me = pc.Item2.Neighbor(dir);
			if (floorTiles.Contains(me.Value) || battleTiles.Contains(me.Value)) {
			    me.Value = spikePool[spikePool.Count-1];
			    //me.Value = 0xD;
			    spikePool.RemoveAt(spikePool.Count-1);
			}
			var roll = rng.Between(1, 20);
			if (roll == 1 && spikePool.Count > 0 && roomsToSanityCheck.Count > 0) {
			    var rm = roomsToSanityCheck.SpliceRandom(rng);
			    rm.start.Value = spikePool[spikePool.Count-1];
			    //rm.start.Value = 0xD;
			    spikePool.RemoveAt(spikePool.Count-1);
			}
		    }

		    //
		    // * Place each chest tile randomly on a room floor tiles
		    // * Place the spike tile randomly on a room floor tile adjacent to chest tiles
		    // ** Weighting 40% below chest, 20% left/right/top
		    // * Finally, sanity check each room that we can reach all chests, doors, teleports and NPCs in the room
		}

		public async Task RandomlyRelocateChests(MT19337 rng, List<Map> maps, NPCdata npcdata, Flags flags) {
		    // Groups of maps that make up a shuffle pool
		    // They need to all use the same tileset.

		    List<MapId[]> dungeons = new() {
			new MapId[] { MapId.ConeriaCastle1F, MapId.ConeriaCastle2F, MapId.ElflandCastle, MapId.NorthwestCastle },

			new MapId[] { MapId.MarshCaveB1, MapId.MarshCaveB2, MapId.MarshCaveB3 }, // Marsh
			new MapId[] { MapId.IceCaveB1, MapId.IceCaveB2, MapId.IceCaveB3 }, // Ice Cave
			new MapId[] { MapId.CastleOfOrdeals1F, MapId.CastleOfOrdeals2F, MapId.CastleOfOrdeals3F }, // Ordeals
			new MapId[] { MapId.MirageTower1F, MapId.MirageTower2F, MapId.MirageTower3F }, // Mirage
			new MapId[] { MapId.TempleOfFiendsRevisited1F,
			    MapId.TempleOfFiendsRevisited2F,
			    MapId.TempleOfFiendsRevisited3F,
			    MapId.TempleOfFiendsRevisitedEarth,
			    MapId.TempleOfFiendsRevisitedFire,
			    MapId.TempleOfFiendsRevisitedWater,
			    MapId.TempleOfFiendsRevisitedAir,
			    MapId.TempleOfFiendsRevisitedChaos }, // ToFR

			// There's no space to shuffle anything.
			new MapId[] { MapId.TempleOfFiends }, // ToF
			// new MapId[] { MapId.TitansTunnel }, // Titan
		    };

		    List<MapId[]> spreadPlacementDungeons = new() {
			new MapId[] { MapId.Waterfall, MapId.DwarfCave, MapId.MatoyasCave, MapId.SardasCave },
			new MapId[] { MapId.Cardia, MapId.BahamutsRoomB1, MapId.BahamutsRoomB2 }, // Cardia
			new MapId[] { MapId.SeaShrineB1, MapId.SeaShrineB2, MapId.SeaShrineB3, MapId.SeaShrineB4, MapId.SeaShrineB5 }, // Sea Shrine
			new MapId[] { MapId.SkyPalace1F, MapId.SkyPalace2F, MapId.SkyPalace3F, MapId.SkyPalace4F, MapId.SkyPalace5F }, // Sky Castle
		    };

		    List<(MapId,byte)> preserveChests = new();

		    if ((bool)flags.IncentivizeMarsh && flags.MarshIncentivePlacementType == IncentivePlacementType.Vanilla) {
			preserveChests.Add((MapId.MarshCaveB3, 0x49));
		    }

		    bool addearth = true;
		    if ((bool)flags.IncentivizeEarth) {
			if (flags.EarthIncentivePlacementType == IncentivePlacementTypeGated.Vanilla) {
			    preserveChests.Add((MapId.EarthCaveB3, 0x51));
			}

			if (flags.EarthIncentivePlacementType == IncentivePlacementTypeGated.RandomNoGating ||
			    flags.EarthIncentivePlacementType == IncentivePlacementTypeGated.RandomBehindGating)
			{
			    // Split shuffle before/after gating
			    dungeons.Add(new MapId[] { MapId.EarthCaveB1, MapId.EarthCaveB2, MapId.EarthCaveB3 });
			    dungeons.Add(new MapId[] { MapId.EarthCaveB4, MapId.EarthCaveB5 });
			    addearth = false;
			}
		    }
		    if (addearth) {
			dungeons.Add(new MapId[] { MapId.EarthCaveB1, MapId.EarthCaveB2, MapId.EarthCaveB3, MapId.EarthCaveB4, MapId.EarthCaveB5 });
		    }

		    dungeons.Add(new MapId[] { MapId.GurguVolcanoB1, MapId.GurguVolcanoB2, MapId.GurguVolcanoB3, MapId.GurguVolcanoB4, MapId.GurguVolcanoB5 });

		    if ((bool)flags.IncentivizeVolcano && flags.VolcanoIncentivePlacementType == IncentivePlacementType.Vanilla) {
			preserveChests.Add((MapId.GurguVolcanoB5, 0x7E));
		    }

		    if ((bool)flags.IncentivizeIceCave && flags.IceCaveIncentivePlacementType == IncentivePlacementType.Vanilla) {
			preserveChests.Add((MapId.IceCaveB2, 0x5F));
		    }

		    if ((bool)flags.IncentivizeOrdeals && flags.OrdealsIncentivePlacementType == IncentivePlacementType.Vanilla) {
			preserveChests.Add((MapId.CastleOfOrdeals3F, 0x78));
		    }

		    if ((bool)flags.IncentivizeSeaShrine) {
			if (flags.SeaShrineIncentivePlacementType == IncentivePlacementTypeGated.Vanilla)
			{
			    preserveChests.Add((MapId.SeaShrineB1, 0x7B));
			}

			if (flags.SeaShrineIncentivePlacementType == IncentivePlacementTypeGated.RandomNoGating ||
			    flags.SeaShrineIncentivePlacementType == IncentivePlacementTypeGated.RandomBehindGating)
			{
			    preserveChests.Add((MapId.SeaShrineB2, 0x6C));
			}
		    }

		    if ((bool)flags.IncentivizeConeria) {
			if (flags.CorneriaIncentivePlacementType == IncentivePlacementType.Vanilla) {
			    preserveChests.Add((MapId.ConeriaCastle1F, 0x65));
			}
			if (flags.CorneriaIncentivePlacementType == IncentivePlacementType.RandomAtLocation) {
			    preserveChests.Add((MapId.ConeriaCastle1F, 0x63));
			    preserveChests.Add((MapId.ConeriaCastle1F, 0x64));
			    preserveChests.Add((MapId.ConeriaCastle1F, 0x65));
			    preserveChests.Add((MapId.ConeriaCastle1F, 0x66));
			    preserveChests.Add((MapId.ConeriaCastle1F, 0x67));
			    preserveChests.Add((MapId.ConeriaCastle1F, 0x68));
			}
		    }

		    if ((bool)flags.IncentivizeMarshKeyLocked) {
			if (flags.MarshLockedIncentivePlacementType == IncentivePlacementType.Vanilla) {
			    preserveChests.Add((MapId.MarshCaveB3, 0x4D));
			}
			if (flags.MarshLockedIncentivePlacementType == IncentivePlacementType.RandomAtLocation) {
			    preserveChests.Add((MapId.MarshCaveB3, 0x4B));
			    preserveChests.Add((MapId.MarshCaveB3, 0x4C));
			    preserveChests.Add((MapId.MarshCaveB3, 0x4D));
			}
		    }

		    if ((bool)flags.IncentivizeSkyPalace && flags.SkyPalaceIncentivePlacementType == IncentivePlacementTypeGated.Vanilla) {
			preserveChests.Add((MapId.SkyPalace2F, 0x5F));
		    }

		    if ((bool)flags.IncentivizeCardia && flags.CardiaIncentivePlacementType == IncentivePlacementType.Vanilla) {
			preserveChests.Add((MapId.Cardia, 0x6B));
		    }

		    foreach (MapId[] b in dungeons) {
			await shuffleChestLocations(rng, maps, b, preserveChests, npcdata,
					      (byte)(flags.EnemizerEnabled ? 0x00 : 0x80),
						    false, flags.RelocateChestsTrapIndicator);
		    }

		    foreach (MapId[] b in spreadPlacementDungeons) {
			await shuffleChestLocations(rng, maps, b, preserveChests, npcdata,
					      (byte)(flags.EnemizerEnabled ? 0x00 : 0x80),
						    true, flags.RelocateChestsTrapIndicator);
		    }
		}
	}
}
