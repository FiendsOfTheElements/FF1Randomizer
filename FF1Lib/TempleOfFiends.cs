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
	public enum ShortToFRFiendsRefights
	{
		[Description("All")]
		All,
		[Description("Two Paths")]
		TwoPaths,
		[Description("Lopsided")]
		Lopsided,
		[Description("Progressive")]
		Progressive,
		[Description("Anything Goes")]
		AnythingGoes,
		[Description("Rude")]
		Rude,
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
			ShortToFRFiendsRefights shorttofrfiendsrefights = flags.ShortToFRFiendsRefights == ShortToFRFiendsRefights.Random ? (ShortToFRFiendsRefights)rng.Between(0, (Enum.GetNames(typeof(ShortToFRFiendsRefights)).Length - 2)) : flags.ShortToFRFiendsRefights;

			if (flags.GameMode == GameModes.DeepDungeon)
			{
				if (mode == ToFRMode.Short)
				{
					UpdateDeepDungeonToFR(maps, flags, teleporters, tilesets, shorttofrfiendsrefights, (bool)flags.ChaosFloorEncounters, rng);
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
				ShortenToFR(maps, flags, teleporters, shorttofrfiendsrefights, true, false, rng);
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
		private void UpdateDeepDungeonToFR(StandardMaps maps, Flags flags, Teleporters teleporters, TileSetsData tilesets, ShortToFRFiendsRefights shorttofrfiendsrefights, bool chaosfloorsencouters, MT19337 rng)
		{
			teleporters.StandardMapTeleporters[TeleportIndex.TempleOfFiends10] = new TeleportDestination(MapIndex.TempleOfFiendsRevisitedChaos, new Coordinate(0x0F, 0x03, CoordinateLocale.StandardInRoom));

			ShortenToFR(maps, flags, teleporters, shorttofrfiendsrefights, false, true, rng);

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
		private void ShortenToFR(StandardMaps maps, Flags flags, Teleporters teleporters, ShortToFRFiendsRefights shorttofrfiendsrefights, bool addLutePlate, bool deepdungeon, MT19337 rng)
		{
			// Black Orb tile Warp destination change straight to an edit Chaos floor with all the ToFR Chests.
			if (!deepdungeon)
			{
				teleporters.StandardMapTeleporters[TeleportIndex.TempleOfFiends2] = new TeleportDestination(MapIndex.TempleOfFiendsRevisitedChaos, new Coordinate(0x0F, 0x03, CoordinateLocale.StandardInRoom));
			}

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

			if (shorttofrfiendsrefights == ShortToFRFiendsRefights.All)
			{
				landingArea.Add(Blob.FromHex($"31{battles[3]:X2}{battles[2]:X2}{battles[1]:X2}{battles[0]:X2}31{battles[0]:X2}{battles[1]:X2}{battles[2]:X2}{battles[3]:X2}31"));
			}
			else if (shorttofrfiendsrefights == ShortToFRFiendsRefights.TwoPaths)
			{
				battles.Shuffle(rng);
				landingArea.Add(Blob.FromHex($"31{battles[0]:X2}3131{battles[1]:X2}31{battles[2]:X2}3131{battles[3]:X2}31"));
			}
			else if (shorttofrfiendsrefights == ShortToFRFiendsRefights.Lopsided)
			{
				// Roll 1d4 to see which fiend gets buffed.
				int fiend = Rng.Between(rng, 0, 3);
				byte buffedFiend = 0;
				byte fiendReference = 0;

				// Roll 1d2 to see which side (left/right) will have the single buffed fiend.
				bool flipLopsided = Rng.Between(rng, 0, 1) > 0;

				switch (fiend)
				{
					case 0:
						buffedFiend = 0x57;
						fiendReference = Enemy.Lich2;
						battles = new List<byte> { 0x58, 0x59, 0x5A };
						break;
					case 1:
						buffedFiend = 0x58;
						fiendReference = Enemy.Kary2;
						battles = new List<byte> { 0x57, 0x59, 0x5A };
						break;
					case 2:
						buffedFiend = 0x59;
						fiendReference = Enemy.Kraken2;
						battles = new List<byte> { 0x57, 0x58, 0x5A };
						break;
					case 3:
						buffedFiend = 0x5A;
						fiendReference = Enemy.Tiamat2;
						battles = new List<byte> { 0x57, 0x58, 0x59 };
						break;
				}

				battles.Shuffle(rng);
				ScaleSingleEnemyStats(fiendReference, flags.BossScaleStatsLow * 2, flags.BossScaleStatsHigh * 2, flags.IncludeMorale, rng,
						  (bool)flags.SeparateBossHPScaling, flags.BossScaleHpLow * 2, flags.BossScaleHpHigh * 2, GetEvadeIntFromFlag(flags.EvadeCap));
				if (flipLopsided)
				{
					landingArea.Add(Blob.FromHex($"31313131{buffedFiend:X2}31{battles[0]:X2}{battles[1]:X2}{battles[2]:X2}3131"));
				}
				else
				{
					landingArea.Add(Blob.FromHex($"3131{battles[2]:X2}{battles[1]:X2}{battles[0]:X2}31{buffedFiend:X2}31313131"));
				}
			}
			else if (shorttofrfiendsrefights == ShortToFRFiendsRefights.Progressive)
			{
				var easyBattles = new List<byte> { 0x57, 0x58 };
				var hardBattles = new List<byte> { 0x59, 0x5A };
				easyBattles.Shuffle(rng);
				hardBattles.Shuffle(rng);
				landingArea.Add(Blob.FromHex($"31{hardBattles[0]:X2}3131{easyBattles[0]:X2}31{easyBattles[1]:X2}3131{hardBattles[1]:X2}31"));
			}
			else if (shorttofrfiendsrefights == ShortToFRFiendsRefights.AnythingGoes)
			{
				/** 
				 * 0 - No encounter
				 * 1 - Lich2
				 * 2 - Kary2
				 * 3 - Kraken2
				 * 4 - Kraken2
				 * 5 - Tiamat2
				 * 6 - Tiamat2 **/
				String anythingGoesHex = "31";
				for (int i = 0; i < 8; i++)
				{
					int encounter = Rng.Between(rng, 0, 6);
					switch (encounter)
					{
						case 0: // no encounter, 14.3%
							anythingGoesHex += "31";
							break;
						case 1: // lich2, 14.3%
							anythingGoesHex += $"{0x57:X2}";
							break;
						case 2: // kary2, 14.3%
							anythingGoesHex += $"{0x58:X2}";
							break;
						case 3:
						case 4: // kraken2, 28.6%
							anythingGoesHex += $"{0x59:X2}";
							break;
						case 5:
						case 6: // tiamat2, 28.6%
							anythingGoesHex += $"{0x5A:X2}";
							break;
					}
					if (i == 3)
					{
						anythingGoesHex += "31"; // center tile
					}
				}
				anythingGoesHex += "31";
				landingArea.Add(Blob.FromHex(anythingGoesHex));
			}
			else if (shorttofrfiendsrefights == ShortToFRFiendsRefights.Rude)
			{
				// Mimic "All" option
				landingArea.Add(Blob.FromHex($"31{battles[3]:X2}{battles[2]:X2}{battles[1]:X2}{battles[0]:X2}31{battles[0]:X2}{battles[1]:X2}{battles[2]:X2}{battles[3]:X2}31"));

				// Add 5th fiend encounter
				int fiend = Rng.Between(rng, 0, 3);
				byte extraFiend = 0;
				extraFiend = 0x57;
				switch (fiend)
				{
					case 0: // lich
						extraFiend = 0x57;
						break;
					case 1: // kary
						extraFiend = 0x58;
						break;
					case 2: // kraken
						extraFiend = 0x59;
						break;
					case 3: // tiamat
						extraFiend = 0x5A;
						break;
				}
				maps[MapIndex.TempleOfFiendsRevisitedChaos].Map[22, 15] = (byte)extraFiend;
			}
			else
			{
				landingArea.Add(Blob.FromHex("3131313131313131313131"));
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
			MenuText.MenuStrings[(int)FF1Text.MenuString.UseLuteSuccess] = FF1Text.TextToBytes("The tune plays,\nopening the pathway.", useDTE: true);

			// make lute plate a single color
			// The lute plate sprite uses two different palettes for some reason, as does the rod plate, despite looking the same
			// top and bottom. The rod plate uses the color of the vampire's hair and skin, which are at different indices of the palettes for
			// the vampire's top and bottom. In short ToFR, the lute plate and Garland need to share a palette, but Garland's skin is the same
			// index in the top and bottom palettes. We could remap all of this, but in order to import NPCs, it's easier to just black out Garland's room
			// in ToFR, map the dark blue color used for his outline and shadow at the end of the game to transparent, and then add the gold
			// from Garland's head to the bottom palette, unused in Garland but used in the lute plate.
			MakeGarlandsBorderTransparent(); // so lute plate change doesn't conflict with Garland, he'll look the same on a black background
			Put(0x02B2D, Blob.FromHex("27")); // change bottom lute plate palette (and Garland's bottom palette)
		}

		private void AddLutePlateToFloor1F(StandardMaps maps)
		{
			// add lute plate (can't use mapNpcIndex 0-2, those belong to Garland)
			maps[MapIndex.TempleOfFiendsRevisited1F].MapObjects.SetNpc(0, ObjectId.LutePlate, 0x14, 0x15, inRoom: true, stationary: true);

			// replace "The tune plays,\nrevealing a stairway." text (0x385BA) originally "9DAB1AB7B8B11AB3AFA4BCB6BF05B5A8B92BAF1FAA2024B7A4ACB55DBCC000"
			MenuText.MenuStrings[(int)FF1Text.MenuString.UseLuteSuccess] = FF1Text.TextToBytes("The tune plays,\nopening the pathway.", useDTE: true);

			// make lute plate a single color
			PutInBank(0x00, 0xA000 + ((int)MapIndex.TempleOfFiendsRevisited1F * 0x30) + 0x18, Blob.FromHex("3000001030000010"));
		}

		private void MakeGarlandsBorderTransparent()
		{
			// replaces the outline of Garland, stuff that normally displays in dark blue or black
			// with transparency, for use with making a single color lute plate
			// Garland's NPC tiles are at 0x0B400; we only need to replace one complete sprite (4 tiles) because
			// he only ever faces down.
			// Originally, this used the following hard-coding of part of the bit-planes of each tile
			// Put(0x0B400, Blob.FromHex("0000000601050703"));
			// Put(0x0B410, Blob.FromHex("0000006080A0E0C0"));
			// Put(0x0B420, Blob.FromHex("0000006060000000"));
			// Put(0x0B430, Blob.FromHex("0006060000000000"));
			
			// Instead of hard-coding the above, we need to be more flexible in case of NPC-import,
			// so we simply map color 1 to color 0 in these tiles. Strictly speaking we probably
			// only need to do this for the bottom two tiles, but there could be weird interactions.
			for (int n = 0, offset = 0x0B400; n < 4; n++ )
			{
				int index = offset + n*0x10;
				byte[] tile = DecodePPU(Get(index,0x10));
				// map color 1 to color 0
				tile = tile.Select(i => i == 1? (byte)0 : i).ToArray();
				Put(index, EncodeForPPU(tile));

			}
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
