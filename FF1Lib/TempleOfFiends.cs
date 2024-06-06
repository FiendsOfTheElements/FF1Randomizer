using System.ComponentModel;
using static FF1Lib.FF1Rom;

namespace FF1Lib
{
	public enum ToFRMode
	{
		[Description("Long ToFR")]
		Long,
		[Description("Mid ToFR")]
		Mid,
		[Description("Short ToFR")]
		Short,
		[Description("Random")]
		Random,
	}
	public enum FiendsRefights
	{
		[Description("All")]
		All,
		[Description("Two Paths")]
		TwoPaths,
		[Description("None")]
		None,
		[Description("Random")]
		Random,
	}
	public partial class FF1Rom : NesRom
	{
		private void UpdateToFR(StandardMaps maps, Teleporters teleporters, TileSetsData tilesets, Flags flags, MT19337 rng)
		{
			// Collapse Flags
			ToFRMode mode = flags.ToFRMode == ToFRMode.Random ? (ToFRMode)rng.Between(0, (Enum.GetNames(typeof(ToFRMode)).Length - 2)) : flags.ToFRMode;
			FiendsRefights fiendsrefights = flags.FiendsRefights == FiendsRefights.Random ? (FiendsRefights)rng.Between(0, (Enum.GetNames(typeof(FiendsRefights)).Length - 2)) : flags.FiendsRefights;

			if (flags.GameMode == GameModes.DeepDungeon)
			{
				if (mode == ToFRMode.Short)
				{
					UpdateDeepDungeonToFR(maps, teleporters, tilesets, fiendsrefights, (bool)flags.ChaosFloorEncounters, rng);
					return;
				}
				else
				{
					if ((bool)flags.ChaosFloorEncounters)
					{
						EnableChaosFloorEncounters(maps);
					}
					return;
				}
			}

			// Update inRoom teleporters since these aren't manually defined in Teleporters
			var tof2tele = teleporters.StandardMapTeleporters[TeleportIndex.TempleOfFiends2];
			teleporters.StandardMapTeleporters[TeleportIndex.TempleOfFiends2] = new TeleportDestination(tof2tele, new Coordinate(tof2tele.Coordinates.X, tof2tele.Coordinates.Y, CoordinateLocale.StandardInRoom));
			var tof6tele = teleporters.StandardMapTeleporters[TeleportIndex.TempleOfFiends6];
			teleporters.StandardMapTeleporters[TeleportIndex.TempleOfFiends6] = new TeleportDestination(tof6tele, new Coordinate(tof6tele.Coordinates.X, tof6tele.Coordinates.Y, CoordinateLocale.StandardInRoom));

			// Change the warp tile requirements from 4_ORBS to None, global change, ToFR access is always blocked by the Black orb
			tilesets[5].Tiles[0x40].PropertyType = 0x80;

			// Update ToFR Maps
			if (mode == ToFRMode.Mid)
			{
				MidToFR(maps);
			}
			else if (mode == ToFRMode.Short)
			{
				ShortenToFR(maps, teleporters, fiendsrefights, true, rng);
			}

			// Update Fiends Refights
			if (fiendsrefights == FiendsRefights.None)
			{
				maps[MapIndex.TempleOfFiendsRevisitedEarth].Map[0x1E, 0x24] = 0x5C;
				maps[MapIndex.TempleOfFiendsRevisitedFire].Map[0x16, 0x10] = 0x5C;
				maps[MapIndex.TempleOfFiendsRevisitedWater].Map[0x1C, 0x18] = 0x5C;
				maps[MapIndex.TempleOfFiendsRevisitedAir].Map[0x08, 0x10] = 0x5C;
			}
			else if(fiendsrefights == FiendsRefights.TwoPaths)
			{
				CreateTwoPathsToFR(maps, mode, rng);
			}

			// Add ToFR Exit
			if ((bool)flags.ExitToFR)
			{
				EnableToFRExit(maps, mode);
			}

			// Add Encounters to Chaos' Floor
			if ((bool)flags.ChaosFloorEncounters)
			{
				EnableChaosFloorEncounters(maps);
			}

			// Unlock ToFR
			if ((bool)flags.ChaosRush)
			{
				EnableChaosRush(tilesets);
			}
		}
		private void UpdateDeepDungeonToFR(StandardMaps maps, Teleporters teleporters, TileSetsData tilesets, FiendsRefights fiendsRefights, bool chaosfloorsencouters, MT19337 rng)
		{
			teleporters.StandardMapTeleporters[TeleportIndex.TempleOfFiends10] = new TeleportDestination(MapIndex.TempleOfFiendsRevisitedChaos, new Coordinate(0x0F, 0x03, CoordinateLocale.StandardInRoom));

			ShortenToFR(maps, teleporters, fiendsRefights, false, rng);

			// Add Encounters to Chaos' Floor
			if (chaosfloorsencouters)
			{
				EnableChaosFloorEncounters(maps);
			}

			EnableChaosRush(tilesets);
		}

		private void MidToFR(StandardMaps maps)
		{
			// 1F
			List<Blob> entrance1F = new List<Blob> //10 to 18
			{
				Blob.FromHex("2020202020202020"),
				Blob.FromHex("2020202020202020"),
			};

			maps[MapIndex.TempleOfFiendsRevisited1F].Map.Put((0x10, 0x13), entrance1F.ToArray());
			maps[MapIndex.TempleOfFiendsRevisited1F].Map[0x16, 0x14] = 0x3B; // Lock Door
			maps[MapIndex.TempleOfFiendsRevisited1F].Map[0x1C, 0x15] = 0x39; // Block passages to Stairs B
			maps[MapIndex.TempleOfFiendsRevisited1F].Map[0x11, 0x20] = 0x39;
			maps[MapIndex.TempleOfFiendsRevisited1F].Map[0x10, 0x10] = 0x62; // Move Lute Chests
			maps[MapIndex.TempleOfFiendsRevisited1F].Map[0x10, 0x18] = 0x63;
			maps[MapIndex.TempleOfFiendsRevisited1F].Map[0x23, 0x01] = 0x52; // Left stairs point to Earth floor

			AddLutePlateToFloor1F(maps);

			// Lich Floor
			List<Blob> eraseLichStairs = new List<Blob> 
			{
				Blob.FromHex("30303B30"),
				Blob.FromHex("5C5C3A5C"),
				Blob.FromHex("5C5C5C5C"),
			};

			maps[MapIndex.TempleOfFiendsRevisitedEarth].Map.Put((0x07, 0x1A), eraseLichStairs.ToArray()); // Open lower corridor
			maps[MapIndex.TempleOfFiendsRevisitedEarth].Map[0x10, 0x10] = 0x5F; // Move Kary Chests
			maps[MapIndex.TempleOfFiendsRevisitedEarth].Map[0x11, 0x16] = 0x5E;

			// Kary Floor
			List<Blob> openKaryLowerFloors = new List<Blob> 
			{
				Blob.FromHex("5C5C5C5C5C5C"),
				Blob.FromHex("5C5C5C5C5C5C"),
			};

			maps[MapIndex.TempleOfFiendsRevisitedFire].Map.Put((0x0C, 0x1B), openKaryLowerFloors.ToArray()); // Open lower corridor

			// Kraken Floor
			maps[MapIndex.TempleOfFiendsRevisitedWater].Map[0x1B, 0x08] = 0x5C; // Open direct access to bottom
			maps[MapIndex.TempleOfFiendsRevisitedWater].Map[0x1B, 0x16] = 0x5C;
			maps[MapIndex.TempleOfFiendsRevisitedWater].Map[0x0F, 0x0F] = 0x5D; // Move Masa Chest
		}
		private void ShortenToFR(StandardMaps maps, Teleporters teleporters, FiendsRefights fiendsrefights, bool addLutePlate, MT19337 rng)
		{
			// Black Orb tile Warp destination change straight to an edit Chaos floor with all the ToFR Chests.
			teleporters.StandardMapTeleporters[TeleportIndex.TempleOfFiends2] = new TeleportDestination(MapIndex.TempleOfFiendsRevisitedChaos, new Coordinate(0x0F, 0x03, CoordinateLocale.StandardInRoom));

			// ToFR Map Hack
			List<Blob> landingArea = new List<Blob>
			{
				Blob.FromHex("3F3F000101010101023F3F"),
				Blob.FromHex("3F00045D5E5F606104023F"),
				Blob.FromHex("0004622020202020630402"),
				Blob.FromHex("0304042020202020040405"),
				Blob.FromHex("0604042020202020040408"),
				Blob.FromHex("3006040410041104040830"),
				Blob.FromHex("3130060710071107083031"),
				Blob.FromHex("31313030303B3030303131"),
				Blob.FromHex("31383831383A3831383831"),
			};

			var battles = new List<byte> { 0x57, 0x58, 0x59, 0x5A };

			if (fiendsrefights == FiendsRefights.All)
			{
				landingArea.Add(Blob.FromHex($"31{battles[3]:X2}{battles[2]:X2}{battles[1]:X2}{battles[0]:X2}31{battles[0]:X2}{battles[1]:X2}{battles[2]:X2}{battles[3]:X2}31"));
			}
			else if (fiendsrefights == FiendsRefights.TwoPaths)
			{
				battles.Shuffle(rng);
				landingArea.Add(Blob.FromHex($"31{battles[0]:X2}3131{battles[1]:X2}31{battles[2]:X2}3131{battles[3]:X2}31"));
			}
			maps[MapIndex.TempleOfFiendsRevisitedChaos].Map.Put((0x0A, 0x00), landingArea.ToArray());

			if (addLutePlate)
			{
				AddLutePlateToChaosFloor(maps);
			}
		}
		private void AddLutePlateToChaosFloor(StandardMaps maps)
		{
			// add lute plate (can't use mapNpcIndex 0-2, those belong to Garland)
			maps[MapIndex.TempleOfFiendsRevisitedChaos].MapObjects.SetNpc(3, ObjectId.LutePlate, 15, 5, inRoom: true, stationary: true);

			// replace "The tune plays,\nrevealing a stairway." text (0x385BA) originally "9DAB1AB7B8B11AB3AFA4BCB6BF05B5A8B92BAF1FAA2024B7A4ACB55DBCC000"
			Put(0x385BA, FF1Text.TextToBytes("The tune plays,\nopening the pathway.", useDTE: true));

			// make lute plate a single color
			MakeGarlandsBorderTransparent(); // so lute plate change doesn't conflict with Garland, he'll look the same on a black background
			Put(0x02B2D, Blob.FromHex("27")); // change bottom lute plate palette
		}

		private void AddLutePlateToFloor1F(StandardMaps maps)
		{
			// add lute plate (can't use mapNpcIndex 0-2, those belong to Garland)
			maps[MapIndex.TempleOfFiendsRevisited1F].MapObjects.SetNpc(0, ObjectId.LutePlate, 0x14, 0x15, inRoom: true, stationary: true);

			// replace "The tune plays,\nrevealing a stairway." text (0x385BA) originally "9DAB1AB7B8B11AB3AFA4BCB6BF05B5A8B92BAF1FAA2024B7A4ACB55DBCC000"
			Put(0x385BA, FF1Text.TextToBytes("The tune plays,\nopening the pathway.", useDTE: true));

			// make lute plate a single color
			PutInBank(0x00, 0xA000 + ((int)MapIndex.TempleOfFiendsRevisited1F * 0x30) + 0x18, Blob.FromHex("3000001030000010"));
		}

		private void MakeGarlandsBorderTransparent()
		{
			// replaces the outline of Garland, stuff that normally displays in dark blue or black
			// with transparency, for use with making a single color lute plate
			Put(0x0B400, Blob.FromHex("0000000601050703"));
			Put(0x0B410, Blob.FromHex("0000006080A0E0C0"));
			Put(0x0B420, Blob.FromHex("0000006060000000"));
			Put(0x0B430, Blob.FromHex("0006060000000000"));
		}
		private void EnableChaosFloorEncounters(StandardMaps maps)
		{
			// Replace floor tiles with encounter tiles
			for (int x = 0; x < 32; x++)
			{
				for (int y = 0; y < 32; y++)
				{
					if (maps[MapIndex.TempleOfFiendsRevisitedChaos].Map[x, y] == (byte)Tile.ToFRNoEncounter)
					{
						maps[MapIndex.TempleOfFiendsRevisitedChaos].Map[x, y] = (byte)Tile.ToFREncounter;
					}
				}
			}
		}
		private void EnableToFRExit(StandardMaps maps, ToFRMode mode)
		{
			if (mode == ToFRMode.Short)
			{
				// add warp portal to alternate map, allowing player to Exit ToFR
				maps[MapIndex.TempleOfFiendsRevisitedChaos].Map[3, 15] = (byte)Tile.PortalWarp;
			}
			else
			{
				// add warp portal to ToFR 1st floor
				maps[MapIndex.TempleOfFiendsRevisited1F].Map[17, 20] = (byte)Tile.PortalWarp;
			}
		}

		private void CreateTwoPathsToFR(StandardMaps maps, ToFRMode mode, MT19337 rng)
		{
			byte toChaosStairs = 0x4B;

			List<(MapIndex map, (int x, int y) coord, byte stairs)> stairsLocations = new() {
				(MapIndex.TempleOfFiendsRevisitedEarth, (0x24, 0x1F), 0x52),
				(MapIndex.TempleOfFiendsRevisitedFire, (0x10, 0x15), 0x53),
				(MapIndex.TempleOfFiendsRevisitedWater, (0x1D, 0x1D), 0x4D),
				(MapIndex.TempleOfFiendsRevisitedAir, (0x1C, 0x03), 0x4C),
			};

			var floorleft1 = stairsLocations.SpliceRandom(rng);
			var floorright1 = stairsLocations.SpliceRandom(rng);
			var floorleft2 = stairsLocations.SpliceRandom(rng);
			var floorright2 = stairsLocations.SpliceRandom(rng);

			if (mode == ToFRMode.Short)
			{
				return;
			}
			else if (mode == ToFRMode.Mid)
			{
				maps[MapIndex.TempleOfFiendsRevisited1F].Map[0x1C, 0x15] = 0x5C;

				maps[MapIndex.TempleOfFiendsRevisited1F].Map[0x23, 0x01] = floorleft1.stairs;
				maps[MapIndex.TempleOfFiendsRevisited1F].Map[0x23, 0x28] = floorright1.stairs;
			}
			else if (mode == ToFRMode.Long)
			{
				maps[MapIndex.TempleOfFiendsRevisited1F].Map[0x01, 0x01] = 0x5C;

				maps[MapIndex.TempleOfFiendsRevisited1F].Map[0x04, 0x01] = floorleft1.stairs;
				maps[MapIndex.TempleOfFiendsRevisited1F].Map[0x01, 0x04] = floorright1.stairs;
			}

			maps[floorleft1.map].Map[floorleft1.coord.y, floorleft1.coord.x] = floorleft2.stairs;
			maps[floorleft2.map].Map[floorleft2.coord.y, floorleft2.coord.x] = toChaosStairs;

			maps[floorright1.map].Map[floorright1.coord.y, floorright1.coord.x] = floorright2.stairs;
			maps[floorright2.map].Map[floorright2.coord.y, floorright2.coord.x] = toChaosStairs;
		}

		public void EnableChaosRush(TileSetsData tileSetsData)
		{
			// MapTilesets
			// Overwrite Keylocked door in ToFR tileset with normal door.
			tileSetsData[(int)TileSets.ToFR].Tiles[0x3B].Properties = new TileProp(0x03, 0x00);
		}
	}
}
