using System.ComponentModel;

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

		private void UpdateToFR(List<Map> maps, Flags flags, MT19337 rng)
		{
			// Collapse Flags
			ToFRMode mode = flags.ToFRMode == ToFRMode.Random ? (ToFRMode)rng.Between(0, (Enum.GetNames(typeof(ToFRMode)).Length - 2)) : flags.ToFRMode;
			FiendsRefights fiendsrefights = flags.FiendsRefights == FiendsRefights.Random ? (FiendsRefights)rng.Between(0, (Enum.GetNames(typeof(FiendsRefights)).Length - 2)) : flags.FiendsRefights;

			// Update ToFR Maps
			if (mode == ToFRMode.Mid)
			{
				MidToFR(maps);
			}
			else if (mode == ToFRMode.Short)
			{
				ShortenToFR(maps, fiendsrefights, rng);
			}

			// Update Fiends Refights
			if (fiendsrefights == FiendsRefights.None)
			{
				maps[(int)MapId.TempleOfFiendsRevisitedEarth][0x1E, 0x24] = 0x5C;
				maps[(int)MapId.TempleOfFiendsRevisitedFire][0x16, 0x10] = 0x5C;
				maps[(int)MapId.TempleOfFiendsRevisitedWater][0x1C, 0x18] = 0x5C;
				maps[(int)MapId.TempleOfFiendsRevisitedAir][0x08, 0x10] = 0x5C;
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
				EnableChaosRush();
			}
		}

		private void MidToFR(List<Map> maps)
		{
			// 1F
			List<Blob> entrance1F = new List<Blob> //10 to 18
			{
				Blob.FromHex("2020202020202020"),
				Blob.FromHex("2020202020202020"),
			};

			maps[(int)MapId.TempleOfFiendsRevisited1F].Put((0x10, 0x13), entrance1F.ToArray());
			maps[(int)MapId.TempleOfFiendsRevisited1F][0x16, 0x14] = 0x3B; // Lock Door
			maps[(int)MapId.TempleOfFiendsRevisited1F][0x1C, 0x15] = 0x39; // Block passages to Stairs B
			maps[(int)MapId.TempleOfFiendsRevisited1F][0x11, 0x20] = 0x39;
			maps[(int)MapId.TempleOfFiendsRevisited1F][0x10, 0x10] = 0x62; // Move Lute Chests
			maps[(int)MapId.TempleOfFiendsRevisited1F][0x10, 0x18] = 0x63;
			maps[(int)MapId.TempleOfFiendsRevisited1F][0x23, 0x01] = 0x52; // Left stairs point to Earth floor

			AddLutePlateToFloor1F(maps);

			// Lich Floor
			List<Blob> eraseLichStairs = new List<Blob> 
			{
				Blob.FromHex("30303B30"),
				Blob.FromHex("5C5C3A5C"),
				Blob.FromHex("5C5C5C5C"),
			};

			maps[(int)MapId.TempleOfFiendsRevisitedEarth].Put((0x07, 0x1A), eraseLichStairs.ToArray()); // Open lower corridor
			maps[(int)MapId.TempleOfFiendsRevisitedEarth][0x10, 0x10] = 0x5F; // Move Kary Chests
			maps[(int)MapId.TempleOfFiendsRevisitedEarth][0x11, 0x16] = 0x5E;

			// Kary Floor
			List<Blob> openKaryLowerFloors = new List<Blob> 
			{
				Blob.FromHex("5C5C5C5C5C5C"),
				Blob.FromHex("5C5C5C5C5C5C"),
			};

			maps[(int)MapId.TempleOfFiendsRevisitedFire].Put((0x0C, 0x1B), openKaryLowerFloors.ToArray()); // Open lower corridor

			// Kraken Floor
			maps[(int)MapId.TempleOfFiendsRevisitedWater][0x1B, 0x08] = 0x5C; // Open direct access to bottom
			maps[(int)MapId.TempleOfFiendsRevisitedWater][0x1B, 0x16] = 0x5C;
			maps[(int)MapId.TempleOfFiendsRevisitedWater][0x0F, 0x0F] = 0x5D; // Move Masa Chest
		}
		private void ShortenToFR(List<Map> maps, FiendsRefights fiendsrefights, MT19337 rng)
		{
			// Black Orb tile Warp destination change straight to an edit Chaos floor with all the ToFR Chests.
			Data[0x00D80] = 0x80; // Map edits
			Data[0x3F001] = 0x0F;
			Data[0x3F101] = 0x03;
			Data[0x3F201] = 0x3B;

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
			maps[(int)MapId.TempleOfFiendsRevisitedChaos].Put((0x0A, 0x00), landingArea.ToArray());

			AddLutePlateToChaosFloor(maps);
		}

		private void AddLutePlateToChaosFloor(List<Map> maps)
		{
			// add lute plate (can't use mapNpcIndex 0-2, those belong to Garland)
			SetNpc(MapId.TempleOfFiendsRevisitedChaos, mapNpcIndex: 3, ObjectId.LutePlate, 15, 5, inRoom: true, stationary: true);

			// replace "The tune plays,\nrevealing a stairway." text (0x385BA) originally "9DAB1AB7B8B11AB3AFA4BCB6BF05B5A8B92BAF1FAA2024B7A4ACB55DBCC000"
			Put(0x385BA, FF1Text.TextToBytes("The tune plays,\nopening the pathway.", useDTE: true));

			// make lute plate a single color
			MakeGarlandsBorderTransparent(); // so lute plate change doesn't conflict with Garland, he'll look the same on a black background
			Put(0x02B2D, Blob.FromHex("27")); // change bottom lute plate palette
		}

		private void AddLutePlateToFloor1F(List<Map> maps)
		{
			// add lute plate (can't use mapNpcIndex 0-2, those belong to Garland)
			SetNpc(MapId.TempleOfFiendsRevisited1F, mapNpcIndex: 0, ObjectId.LutePlate, 0x14, 0x15, inRoom: true, stationary: true);

			// replace "The tune plays,\nrevealing a stairway." text (0x385BA) originally "9DAB1AB7B8B11AB3AFA4BCB6BF05B5A8B92BAF1FAA2024B7A4ACB55DBCC000"
			Put(0x385BA, FF1Text.TextToBytes("The tune plays,\nopening the pathway.", useDTE: true));

			// make lute plate a single color
			PutInBank(0x00, 0xA000 + ((int)MapId.TempleOfFiendsRevisited1F * 0x30) + 0x18, Blob.FromHex("3000001030000010"));
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
		private void EnableChaosFloorEncounters(List<Map> maps)
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
		private void EnableToFRExit(List<Map> maps, ToFRMode mode)
		{
			if (mode == ToFRMode.Short)
			{
				// add warp portal to alternate map, allowing player to Exit ToFR
				maps[(byte)MapId.TempleOfFiendsRevisitedChaos][3, 15] = (byte)Tile.PortalWarp;
			}
			else
			{
				// add warp portal to ToFR 1st floor
				maps[(byte)MapId.TempleOfFiendsRevisited1F][17, 20] = (byte)Tile.PortalWarp;
			}
		}

		private void CreateTwoPathsToFR(List<Map> maps, ToFRMode mode, MT19337 rng)
		{
			byte toChaosStairs = 0x4B;

			List<(MapId map, (int x, int y) coord, byte stairs)> stairsLocations = new() {
				(MapId.TempleOfFiendsRevisitedEarth, (0x24, 0x1F), 0x52),
				(MapId.TempleOfFiendsRevisitedFire, (0x10, 0x15), 0x53),
				(MapId.TempleOfFiendsRevisitedWater, (0x1D, 0x1D), 0x4D),
				(MapId.TempleOfFiendsRevisitedAir, (0x1C, 0x03), 0x4C),
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
				maps[(int)MapId.TempleOfFiendsRevisited1F][0x1C, 0x15] = 0x5C;

				maps[(int)MapId.TempleOfFiendsRevisited1F][0x23, 0x01] = floorleft1.stairs;
				maps[(int)MapId.TempleOfFiendsRevisited1F][0x23, 0x28] = floorright1.stairs;
			}
			else if (mode == ToFRMode.Long)
			{
				maps[(int)MapId.TempleOfFiendsRevisited1F][0x01, 0x01] = 0x5C;

				maps[(int)MapId.TempleOfFiendsRevisited1F][0x04, 0x01] = floorleft1.stairs;
				maps[(int)MapId.TempleOfFiendsRevisited1F][0x01, 0x04] = floorright1.stairs;
			}

			maps[(int)floorleft1.map][floorleft1.coord.y, floorleft1.coord.x] = floorleft2.stairs;
			maps[(int)floorleft2.map][floorleft2.coord.y, floorleft2.coord.x] = toChaosStairs;

			maps[(int)floorright1.map][floorright1.coord.y, floorright1.coord.x] = floorright2.stairs;
			maps[(int)floorright2.map][floorright2.coord.y, floorright2.coord.x] = toChaosStairs;
		}

		public void EnableChaosRush()
		{
			// Overwrite Keylocked door in ToFR tileset with normal door.
			Put(0x0F76, Blob.FromHex("0300"));
		}
	}
}
