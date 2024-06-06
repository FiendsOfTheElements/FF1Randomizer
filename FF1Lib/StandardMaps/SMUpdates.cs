using FF1Lib.Procgen;
using FF1Lib.Sanity;
using RomUtilities;
using System.IO.Compression;
using static FF1Lib.FF1Rom;

namespace FF1Lib
{
	public partial class StandardMaps
	{
		public void ProcgenDungeons(MT19337 rng)
		{
			if ((bool)flags.ProcgenEarth)
			{
				LoadPregenDungeon(rng, teleporters, "earthcaves.zip");

				// Here's the code to generate from scratch, but it takes too long in the browser.
				// So we get one from the pregen pack above.
				//
				// var newmaps = await NewDungeon.GenerateNewDungeon(rng, this, MapIndex.EarthCaveB1, maps, npcdata, this.Progress);
				// foreach (var newmap in newmaps) {
				//   this.ImportCustomMap(maps, teleporters, overworldMap, npcdata, newmap);
				//  }
			}

			ProcgenWaterfall((bool)flags.EFGWaterfall, teleporters, mapObjects[(int)MapIndex.Waterfall], rng);
			FlipMaps(flags, rng);
		}
		public void Update(ZoneFormations formations, MT19337 rng)
		{
			MoveToFBats((bool)flags.MoveToFBats);
			EnableTitansTrove((bool)flags.TitansTrove);
			EnableLefeinShops((bool)flags.LefeinShops);
			EnableMelmondClinic((bool)flags.MelmondClinic);
			RandomVampireAttack((bool)flags.RandomVampAttack, (bool)flags.LefeinShops, (bool)flags.RandomVampAttackIncludesConeria, rng);
			ShufflePravoka((bool)flags.ShufflePravokaShops, AttackedTown == MapIndex.Pravoka, rng);
			EnableGaiaShortcut((bool)flags.GaiaShortcut);
			MoveGaiaItemShop((bool)flags.MoveGaiaItemShop && (bool)flags.GaiaShortcut, rng);
			EnableLefeinSuperStore((bool)flags.LefeinSuperStore && (flags.ShopKillMode_White == ShopKillMode.None && flags.ShopKillMode_Black == ShopKillMode.None));
			ShuffleOrdeals((bool)flags.OrdealsPillars, rng, teleporters);
			SkyCastle4FMode(flags.SkyCastle4FMazeMode, flags.GameMode == GameModes.DeepDungeon, rng);
			ShuffleLavaTiles((bool)flags.ShuffleLavaTiles, rng);
			BahamutB1Encounters((bool)flags.MapHallOfDragons, formations);
			DragonsHoard((bool)flags.MapDragonsHoard);
			MermaidPrison((bool)flags.MermaidPrison && (flags.GameMode != GameModes.DeepDungeon));
			ConfusedOldMen((bool)flags.ConfusedOldMen, rng);
		}

		private void MoveToFBats(bool movebats)
		{
			if (!movebats)
			{
				return;
			}
			mapObjects[(int)MapIndex.TempleOfFiends].MoveNpc(2, 0x0C, 0x0D, false, false);
			mapObjects[(int)MapIndex.TempleOfFiends].MoveNpc(3, 0x1D, 0x0B, false, false);
			mapObjects[(int)MapIndex.TempleOfFiends].MoveNpc(4, 0x1A, 0x19, false, false);
			mapObjects[(int)MapIndex.TempleOfFiends].MoveNpc(5, 0x0F, 0x18, false, false);
			mapObjects[(int)MapIndex.TempleOfFiends].MoveNpc(6, 0x14, 0x0C, false, false);
		}
		private void ConfusedOldMen(bool confuse, MT19337 rng)
		{
			if (!confuse)
			{
				return;
			}
			List<(byte, byte)> coords = new List<(byte, byte)> {
				( 0x2A, 0x0A ), ( 0x28, 0x0B ), ( 0x26, 0x0B ), ( 0x24, 0x0A ), ( 0x23, 0x08 ), ( 0x23, 0x06 ),
				( 0x24, 0x04 ), ( 0x26, 0x03 ), ( 0x28, 0x03 ), ( 0x28, 0x04 ), ( 0x2B, 0x06 ), ( 0x2B, 0x08 )
			};
			coords.Shuffle(rng);

			List<int> sages = Enumerable.Range(0, 12).ToList(); // But the 12th Sage is actually id 12, not 11.
			sages.ForEach(sage => mapObjects[(int)MapIndex.CrescentLake].MoveNpc(sage < 11 ? sage : 12, coords[sage].Item1, coords[sage].Item2, false, false));
		}
		private void FlipMaps(Flags flags, MT19337 rng)
		{
			var mapFlipper = new FlippedMaps(rom, this, flags, teleporters, rng);
			VerticalFlippedMaps = mapFlipper.VerticalFlipStep1();

			if ((bool)flags.ReversedFloors) new ReversedFloors(rom, this, rng, teleporters, VerticalFlippedMaps).Work((bool)flags.ProcgenEarth);

			if((bool)flags.VerticallyFlipDungeons) mapFlipper.VerticalFlipStep2();

			if ((bool)flags.FlipDungeons)
			{
				HorizontalFlippedMaps = mapFlipper.HorizontalFlip(rng, this, teleporters);
			}
		}
		private void EnableTitansTrove(bool movetitan)
		{
			if (!movetitan)
			{
				return;
			}
			mapObjects[(int)MapIndex.TitansTunnel].MoveNpc(0, 4, 8, false, true); // Move the Titan
			maps[(int)MapIndex.TitansTunnel][9, 3] = 0x3F; // Block the tunnel
		}
		private void EnableLefeinShops(bool lefeinshop)
		{
			if (!lefeinshop)
			{
				return;
			}
			var lefein = maps[(byte)MapIndex.Lefein];
			lefein[0x05, 0x11] = 0x25; // Inn Sign
			lefein[0x06, 0x11] = 0x71; // Crescent Lake Inn
			lefein[0x05, 0x15] = 0x1A; // Clinic Sign
			lefein[0x06, 0x15] = 0x65; // Crescent Lake Clinic
		}
		private void EnableMelmondClinic(bool melmondclinic)
		{
			if (!melmondclinic)
			{
				return;
			}
			var melmond = maps[(byte)MapIndex.Melmond];
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
		private void ShufflePravoka(bool shufflepravokashops, bool blightedPravoka, MT19337 rng)
		{
			if (!shufflepravokashops)
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
				maps[(int)MapIndex.Pravoka][0x08, 0x1A] = 0x1B;

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
				maps[(int)MapIndex.Pravoka][coord.y, coord.x] = 0x18;
			}

			foreach (var coord in wallTiles)
			{
				maps[(int)MapIndex.Pravoka].Put(coord, new List<Blob> { Blob.FromHex("1D1D1D") }.ToArray());
			}

			foreach (var coord in highWallTiles)
			{
				maps[(int)MapIndex.Pravoka].Put(coord, new List<Blob> { Blob.FromHex("1D"), Blob.FromHex("16") }.ToArray());
			}

			// then place the shops
			foreach (var shop in shopTiles)
			{
				var coord = validLocations.SpliceRandom(rng);
				maps[(int)MapIndex.Pravoka][coord.y, coord.x] = shop.sign;
				maps[(int)MapIndex.Pravoka][coord.y + 1, coord.x] = shop.door;
			}
		}
		private void EnableGaiaShortcut(bool gaiashortcut)
		{
			if (!gaiashortcut)
			{
				return;
			}
			// Place grass tiles, creating shortcut
			for (int x = 41; x <= 43; x++)
			{
				for (int y = 36; y <= 47; y++)
				{
					maps[(byte)MapIndex.Gaia][y, x] = (byte)Tile.TownGrass;
				}
			}
			for (int y = 44; y <= 47; y++)
			{
				maps[(byte)MapIndex.Gaia][y, 40] = (byte)Tile.TownGrass;
			}

			// Restore building shadow
			maps[(byte)MapIndex.Gaia][39, 41] = (byte)Tile.TownGrassShadow;
			maps[(byte)MapIndex.Gaia][40, 41] = (byte)Tile.TownGrassShadowDiagonal;

			// In the spirit of Gaia's original landscaping, add some trees
			// (Without these it looked like a too perfectly trimmed pathway)
			for (int y = 41; y <= 44; y++)
			{
				maps[(byte)MapIndex.Gaia][y, 43] = (byte)Tile.TownBushes;
			}
			maps[(byte)MapIndex.Gaia][36, 41] = (byte)Tile.TownTree;
			maps[(byte)MapIndex.Gaia][38, 43] = (byte)Tile.TownTree;
			maps[(byte)MapIndex.Gaia][45, 43] = (byte)Tile.TownTree;
			maps[(byte)MapIndex.Gaia][45, 40] = (byte)Tile.TownBushes;
			maps[(byte)MapIndex.Gaia][46, 40] = (byte)Tile.TownTree;
		}

		public void MoveGaiaItemShop(bool movegaiaitemshop, MT19337 rng)
		{
			if (!movegaiaitemshop)
			{
				return;
			}
			// Top left corner of original Item Shop Location
			const int yItemShop = 33;
			const int xItemShop = 10;

			// How much to move it
			int yDisplacement = 11;
			int xDisplacement = 33;

			// place a tree in the old Item Shop grid first, it often looks better in the moved location
			maps[(byte)MapIndex.Gaia][yItemShop, xItemShop + 4] = (byte)Tile.TownTree;

			// something to put in place of the old item shop
			var replacementTileOptions = new List<Tile> { Tile.TownWaterway, Tile.TownFlowers, Tile.TownGrass, Tile.TownStonePath, Tile.TownSand, Tile.TownTomb1, Tile.TownTree };
			Tile replacementTile = replacementTileOptions.PickRandom(rng);

			for (int y = yItemShop; y <= yItemShop + 3; y++)
			{
				for (int x = xItemShop; x <= xItemShop + 4; x++)
				{
					// lift and shift
					maps[(byte)MapIndex.Gaia][y + yDisplacement, x + xDisplacement] = maps[(byte)MapIndex.Gaia][y, x];
					maps[(byte)MapIndex.Gaia][y, x] = (byte)replacementTile;
				}
			}
		}
		public void EnableLefeinSuperStore(bool lefeinsuperstore)
		{
			if (!lefeinsuperstore)
			{
				return;
			}
			// define
			List<Blob> superStore = new List<Blob>
			{
				//              1 2 3 4 WM 5 6 7 8  XX  1 2 3 4 BM 5 6 7 8
				Blob.FromHex("171717171717171717171700171717171717171717171700"),
				Blob.FromHex("181818181823181818181801181818181824181818181801"),
				Blob.FromHex("1D4F5051541D525357581D021D595A5B5E1D5C5D61621D02"),
			};
			// place
			maps[(int)MapIndex.Lefein].Put((0x28, 0x01), superStore.ToArray());
			// cleanup (removes single tree)
			maps[(int)MapIndex.Lefein][0x00, 0x34] = (byte)Tile.TownGrass;
		}
		private void SkyCastle4FMode(SkyCastle4FMazeMode mode, bool deepdungeon, MT19337 rng)
		{
			if (deepdungeon || mode == SkyCastle4FMazeMode.Vanilla)
			{
				return;
			}

			if (mode == SkyCastle4FMazeMode.Maze)
			{
				DoSkyCastle4FMaze(rng);
			}
			else if(mode == SkyCastle4FMazeMode.Teleporters)
			{
				ShuffleSkyCastle4F(rng);
			}
		}
		private void DoSkyCastle4FMaze(MT19337 rng)
		{
			var map = maps[(int)MapIndex.SkyPalace4F];
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

			ShuffleSkyCastle4F(rng);
		}

		private void ShuffleSkyCastle4F(MT19337 rng)
		{
			// Don't shuffle the return teleporter as Floor and Entrance shuffle might want to edit it.
			var map = maps[(byte)MapIndex.SkyPalace4F];
			var upTeleporter = (x: 0x23, y: 0x23);
			var dest = GetSkyCastleFloorTile(rng, map);
			map.SwapTiles(upTeleporter, dest);
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

		private void ShuffleLavaTiles(bool shufflelavatiles, MT19337 rng)
		{
			if (!shufflelavatiles)
			{
				return;
			}

			List<MapIndex> lavaMaps = new() { MapIndex.GurguVolcanoB1, MapIndex.GurguVolcanoB2, MapIndex.GurguVolcanoB3, MapIndex.GurguVolcanoB4, MapIndex.GurguVolcanoB5 };

			byte lavaTile = 0x3D;
			byte encounterTile = 0x41;

			foreach (var map in lavaMaps)
			{
				int lavacount = 0;
				int enccount = 0;

				List<SCCoords> tileCoords = new();
				List<SCCoords> lavaCoords = new();
				List<SCCoords> encounterCoords = new();

				for (int x = 0; x < 0x40; x++)
				{
					for (int y = 0; y < 0x40; y++)
					{
						byte currentile = maps[(int)map].MapBytes[x, y];
						if (currentile == lavaTile)
						{
							tileCoords.Add(new SCCoords(x, y));
							lavacount++;
						}
						else if (currentile == encounterTile)
						{
							tileCoords.Add(new SCCoords(x, y));
							enccount++;
						}
					}
				}

				for (int i = 0; i < lavacount; i++)
				{
					var lavatilecoord = tileCoords.SpliceRandom(rng);
					maps[(int)map].MapBytes[lavatilecoord.X, lavatilecoord.Y] = lavaTile;
				}

				foreach (var coordleft in tileCoords)
				{
					maps[(int)map].MapBytes[coordleft.X, coordleft.Y] = encounterTile;
				}
			}
		}
		public void BahamutB1Encounters(bool hallofdragons, ZoneFormations zoneformations)
		{
			if (!hallofdragons)
			{
				return;
			}

			// Adds dragon-themed encounters to the long
			// hallway to Bahamut's room
			var formation = zoneformations[64 + (int)MapIndex.BahamutCaveB1];

			formation.Formations[0] = 0x2A + 0x80; // 2-4 Red D
			formation.Formations[1] = 0x30 + 0x80; // 3-4 Frost D
			formation.Formations[2] = 0x4B + 0x80; // 2-4 Zombie D
			formation.Formations[3] = 0x4E + 0x80; // 2-3 Blue D
			formation.Formations[4] = 0x59 + 0x80; // 2-4 Gas D
			formation.Formations[5] = 0x4E + 0x80; // 2-3 Blue D
			formation.Formations[6] = 0x59 + 0x80; // 2-4 Gas D
			formation.Formations[7] = 0x77; // Tiamat 1 (!)

			maps[(byte)MapIndex.BahamutCaveB1][1, 1] = (byte)Tile.CardiaEncounters;
			maps[(byte)MapIndex.BahamutCaveB1][1, 2] = (byte)Tile.CardiaEncounters;
			maps[(byte)MapIndex.BahamutCaveB1][1, 3] = (byte)Tile.CardiaEncounters;
			maps[(byte)MapIndex.BahamutCaveB1][2, 1] = (byte)Tile.CardiaEncounters;
			maps[(byte)MapIndex.BahamutCaveB1][2, 3] = (byte)Tile.CardiaEncounters;
			maps[(byte)MapIndex.BahamutCaveB1][3, 1] = (byte)Tile.CardiaEncounters;
			maps[(byte)MapIndex.BahamutCaveB1][3, 2] = (byte)Tile.CardiaEncounters;
			maps[(byte)MapIndex.BahamutCaveB1][3, 3] = (byte)Tile.CardiaEncounters;
			for (int i = 4; i <= 0x32; i++)
			{
				maps[(byte)MapIndex.BahamutCaveB1][i, 2] = (byte)Tile.CardiaEncounters;
			}
			maps[(byte)MapIndex.BahamutCaveB1][0x33, 1] = (byte)Tile.CardiaEncounters;
			maps[(byte)MapIndex.BahamutCaveB1][0x33, 2] = (byte)Tile.CardiaEncounters;
			maps[(byte)MapIndex.BahamutCaveB1][0x33, 3] = (byte)Tile.CardiaEncounters;
			maps[(byte)MapIndex.BahamutCaveB1][0x34, 1] = (byte)Tile.CardiaEncounters;
			maps[(byte)MapIndex.BahamutCaveB1][0x34, 2] = (byte)Tile.CardiaEncounters;
			maps[(byte)MapIndex.BahamutCaveB1][0x34, 3] = (byte)Tile.CardiaEncounters;
			maps[(byte)MapIndex.BahamutCaveB1][0x35, 1] = (byte)Tile.CardiaEncounters;
			maps[(byte)MapIndex.BahamutCaveB1][0x35, 3] = (byte)Tile.CardiaEncounters;
			maps[(byte)MapIndex.BahamutCaveB1][0x36, 1] = (byte)Tile.CardiaEncounters;
			maps[(byte)MapIndex.BahamutCaveB1][0x36, 2] = (byte)Tile.CardiaEncounters;
			maps[(byte)MapIndex.BahamutCaveB1][0x36, 3] = (byte)Tile.CardiaEncounters;
		}
		public void DragonsHoard(bool enable)
		{
			// Replaces the area around/behind Bahamut with
			// all the Cardia chests.  (Does not delete the
			// original chests, they will be linked)

			if (enable)
			{
				maps[(byte)MapIndex.BahamutCaveB2][1, 17] = (byte)Tile.CardiaCandles;
				maps[(byte)MapIndex.BahamutCaveB2][1, 18] = (byte)Tile.CardiaChest1;
				maps[(byte)MapIndex.BahamutCaveB2][1, 19] = (byte)Tile.CardiaChest2;
				maps[(byte)MapIndex.BahamutCaveB2][1, 20] = (byte)Tile.CardiaChest3;
				maps[(byte)MapIndex.BahamutCaveB2][1, 21] = (byte)Tile.CardiaChest4;
				maps[(byte)MapIndex.BahamutCaveB2][1, 22] = (byte)Tile.CardiaChest5;
				maps[(byte)MapIndex.BahamutCaveB2][1, 23] = (byte)Tile.CardiaChest6;
				maps[(byte)MapIndex.BahamutCaveB2][1, 24] = (byte)Tile.CardiaChest7;
				maps[(byte)MapIndex.BahamutCaveB2][1, 25] = (byte)Tile.CardiaCandles;

				maps[(byte)MapIndex.BahamutCaveB2][2, 17] = (byte)Tile.CardiaChest8;
				maps[(byte)MapIndex.BahamutCaveB2][2, 18] = (byte)Tile.CardiaFloor;
				maps[(byte)MapIndex.BahamutCaveB2][2, 19] = (byte)Tile.CardiaFloor;
				maps[(byte)MapIndex.BahamutCaveB2][2, 20] = (byte)Tile.CardiaFloor;
				maps[(byte)MapIndex.BahamutCaveB2][2, 21] = (byte)Tile.CardiaFloor;
				maps[(byte)MapIndex.BahamutCaveB2][2, 22] = (byte)Tile.CardiaFloor;
				maps[(byte)MapIndex.BahamutCaveB2][2, 23] = (byte)Tile.CardiaFloor;
				maps[(byte)MapIndex.BahamutCaveB2][2, 24] = (byte)Tile.CardiaFloor;
				maps[(byte)MapIndex.BahamutCaveB2][2, 25] = (byte)Tile.CardiaChest9;

				maps[(byte)MapIndex.BahamutCaveB2][3, 17] = (byte)Tile.CardiaCandles;
				maps[(byte)MapIndex.BahamutCaveB2][3, 18] = (byte)Tile.CardiaChest10;
				maps[(byte)MapIndex.BahamutCaveB2][3, 19] = (byte)Tile.CardiaFloor;
				maps[(byte)MapIndex.BahamutCaveB2][3, 20] = (byte)Tile.CardiaFloor;
				maps[(byte)MapIndex.BahamutCaveB2][3, 21] = (byte)Tile.CardiaFloor;
				maps[(byte)MapIndex.BahamutCaveB2][3, 22] = (byte)Tile.CardiaFloor;
				maps[(byte)MapIndex.BahamutCaveB2][3, 23] = (byte)Tile.CardiaFloor;
				maps[(byte)MapIndex.BahamutCaveB2][3, 24] = (byte)Tile.CardiaChest11;
				maps[(byte)MapIndex.BahamutCaveB2][3, 25] = (byte)Tile.CardiaCandles;

				maps[(byte)MapIndex.BahamutCaveB2][4, 17] = (byte)Tile.CardiaFloor;
				maps[(byte)MapIndex.BahamutCaveB2][4, 18] = (byte)Tile.CardiaCandles;
				maps[(byte)MapIndex.BahamutCaveB2][4, 19] = (byte)Tile.CardiaChest12;
				maps[(byte)MapIndex.BahamutCaveB2][4, 20] = (byte)Tile.CardiaFloor;
				maps[(byte)MapIndex.BahamutCaveB2][4, 21] = (byte)Tile.CardiaFloor;
				maps[(byte)MapIndex.BahamutCaveB2][4, 22] = (byte)Tile.CardiaFloor;
				maps[(byte)MapIndex.BahamutCaveB2][4, 23] = (byte)Tile.CardiaChest13;
				maps[(byte)MapIndex.BahamutCaveB2][4, 24] = (byte)Tile.CardiaCandles;
				maps[(byte)MapIndex.BahamutCaveB2][4, 25] = (byte)Tile.CardiaFloor;

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
		public void MermaidPrison(bool enable)
		{
			if (enable)
			{
				for (int y = 0; y < 64; y++)
					for (int x = 0; x < 64; x++)
					{
						if (maps[(byte)MapIndex.SeaShrineB1][x, y] == (byte)Tile.Door) maps[(byte)MapIndex.SeaShrineB1][x, y] = (byte)Tile.DoorLocked;
					}

				// Have locked rooms draw inside NPCs, instead of outside NPCs
				rom.PutInBank(0x1F, 0xCEDE, new byte[] { 0x81 });
			}
		}
		private void ProcgenWaterfall(bool procgenwaterfall, Teleporters teleporters, MapObjects waterfallObjects, MT19337 rng)
		{
			if (!procgenwaterfall)
			{
				return;
			}

			MapRequirements reqs;
			MapGeneratorStrategy strategy;
			MapGenerator generator = new MapGenerator();

			reqs = new MapRequirements
			{
				MapIndex = MapIndex.Waterfall,
				Rom = rom,
				MapObjects = waterfallObjects,
			};
			strategy = MapGeneratorStrategy.WaterfallClone;
			CompleteMap waterfall = generator.Generate(rng, strategy, reqs);

			// Should add more into the reqs so that this can be done inside the generator.
			//teleporters.Waterfall.SetTeleporter(waterfall.Entrance);
			teleporters.OverworldTeleporters[OverworldTeleportIndex.Waterfall] = new TeleportDestination(teleporters.OverworldTeleporters[OverworldTeleportIndex.Waterfall], waterfall.Entrance);
			//overworldMap.PutOverworldTeleport(OverworldTeleportIndex.Waterfall, Teleporters.Waterfall);
			maps[(int)MapIndex.Waterfall].CopyFrom(waterfall.Map);
		}
	}
}
