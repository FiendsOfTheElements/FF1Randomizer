using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FF1Lib.Procgen;
using RomUtilities;
using static FF1Lib.FF1Text;

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
		Lava = 0x39,
		EarthCaveRockA = 0x3E,
		EarthCaveRockB = 0x3F,
		MarshCaveOOB = 0x3F,
		EarthCaveRandomEncounters = 0x41,
		WaterfallInside = 0x46,
		WaterfallSpikeTile = 0x48,
		WaterfallRandomEncounters = 0x49
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

		public const int TeleportOffset = 0x02D00;
		public const int TeleportCount = 64;

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
					tile[1] = extendedtraptiles ? 0x00 : 0x80;
				}
			});
			Put(TilesetDataOffset, tilesets.SelectMany(tileset => tileset.ToBytes()).ToArray());
		}

		public void ShuffleTrapTiles(MT19337 rng, bool randomize)
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

		public void EnableLefeinShops(List<Map> maps)
		{
			var lefein = maps[(byte)MapId.Lefein];
			lefein[0x05, 0x11] = 0x25; // Inn Sign
			lefein[0x06, 0x11] = 0x71; // Crescent Lake Inn
			lefein[0x05, 0x15] = 0x1A; // Clinic Sign
			lefein[0x06, 0x15] = 0x65; // Crescent Lake Clinic
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
					if (tempNpc.Coord != (0,0))
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
			if (mapsToFlip.Contains(MapId.SeaShrineB5))	teleporters.SeaShrineKraken.FlipXcoordinate(); overworld.PutStandardTeleport(TeleportIndex.SeaShrineKraken, teleporters.SeaShrineKraken, OverworldTeleportIndex.Onrac);

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
				"I think you ought to\nknow, I'm feeling very\ndepressed.",
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
					tempNPC.Index = i;
					tempNPC.Coord = (Data[offset + 1] & 0x3F, Data[offset + 2]);
					tempNPC.InRoom = (Data[offset + 1] & 0x80) > 0;
					tempNPC.Stationary = (Data[offset + 1] & 0x40) > 0;
					break;
				}
			}
			return tempNPC;
		}
		public NPC GetNpc(MapId mapId, int position)
		{
			var tempNPC = new NPC();

			int offset = MapSpriteOffset + ((byte)mapId * MapSpriteCount + position) * MapSpriteSize;

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

	}
}
