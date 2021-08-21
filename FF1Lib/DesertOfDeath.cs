using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using RomUtilities;

namespace FF1Lib
{
	public partial class FF1Rom
	{

		public enum MapDirection : int
		{
			North = 0,
			NorthWest = 1,
			West = 2,
			SouthWest = 3,
			South = 4,
			SouthEast = 5,
			East = 6,
			NorthEast = 7
		}

		public enum OWTile : byte
		{
			ConeriaCastle = 0x01,
			MountainCenter = 0x21,
			ForestCenter = 0x14,
			DesertCenter = 0x45,
		}

		public List<(MapLocation, byte)> OWTileLink = new()
		{
			(MapLocation.ConeriaCastle1, 0x01),
			(MapLocation.EarthCave1, 0x0E),
			(MapLocation.ElflandCastle, 0x1B),
			(MapLocation.MirageTower1, 0x1D),
			(MapLocation.NorthwestCastle, 0x29),
			(MapLocation.IceCave1, 0x2B),
			(MapLocation.DwarfCave, 0x2F),
			(MapLocation.MatoyasCave, 0x32),
			(MapLocation.TitansTunnelEast, 0x34),
			(MapLocation.TitansTunnelWest, 0x35),
			(MapLocation.Caravan, 0x36),
			(MapLocation.CastleOrdeals1, 0x38),
			(MapLocation.SardasCave, 0x3A),
			(MapLocation.Waterfall, 0x46),
			(MapLocation.Coneria, 0x49),
			(MapLocation.Pravoka, 0x4A),
			(MapLocation.Elfland, 0x4C),
			(MapLocation.Melmond, 0x4D),
			(MapLocation.CrescentLake, 0x4E),
			(MapLocation.TempleOfFiends1, 0x57),
			(MapLocation.Gaia, 0x5A),
			(MapLocation.Onrac, 0x5D),
			(MapLocation.GurguVolcano1, 0x64),
			(MapLocation.Cardia2, 0x66),
			(MapLocation.Cardia4, 0x67),
			(MapLocation.Cardia5, 0x68),
			(MapLocation.Cardia6, 0x69),
			(MapLocation.Cardia1, 0x6A),
			(MapLocation.BahamutCave1, 0x6C),
			(MapLocation.Lefein, 0x6D),
			(MapLocation.MarshCave1, 0x6E),
		};

		public static List<(MapLocation, List<List<byte>>)> MapModifications = new ()
		{
			(MapLocation.ConeriaCastle1, new List<List<byte>>() {
					new List<byte> { 0x45, 0x45, 0x09, 0x0A, 0x45, 0x45 },
					new List<byte> { 0x45, 0x2C, 0x19, 0x1A, 0x2E, 0x45 },
					new List<byte> { 0x3B, 0x3C, 0x01, 0x02, 0x3E, 0x3F },
					new List<byte> { 0x4B, 0x49, 0x3D, 0x3D, 0x49, 0x4F },
					new List<byte> { 0x5B, 0x49, 0x3D, 0x3D, 0x49, 0x5F },
					new List<byte> { 0x6B, 0x49, 0x3D, 0x3D, 0x49, 0x6F },
					new List<byte> { 0x7B, 0x7D, 0x3D, 0x3D, 0x7E, 0x7F },
			}),
			(MapLocation.TempleOfFiends1, new List<List<byte>>() {
					new List<byte> { 0x45, 0x47, 0x48, 0x45 },
					new List<byte> { 0x56, 0x57, 0x58, 0x59 },
			}),
			(MapLocation.MatoyasCave, new List<List<byte>>() {
					new List<byte> { 0x45, 0x10, 0x11, 0x12 },
					new List<byte> { 0x10, 0x21, 0x21, 0x22 },
					new List<byte> { 0x30, 0x31, 0x32, 0x33 },
			}),
			(MapLocation.Pravoka, new List<List<byte>>() {
					new List<byte> { 0x45, 0x2C, 0x2D, 0x2E, 0x45 },
					new List<byte> { 0x3B, 0x3C, 0x4A, 0x3E, 0x3F },
					new List<byte> { 0x4B, 0x4A, 0x3D, 0x4A, 0x4F },
					new List<byte> { 0x5C, 0x7D, 0x3D, 0x7E, 0x5E },
			}),
			(MapLocation.DwarfCave, new List<List<byte>>() {
					new List<byte> { 0x10, 0x11, 0x11, 0x11, 0x12 },
					new List<byte> { 0x20, 0x21, 0x22, 0x30, 0x33 },
					new List<byte> { 0x30, 0x2F, 0x33, 0x45, 0x45 },
			}),
			(MapLocation.NorthwestCastle, new List<List<byte>>() {
					new List<byte> { 0x09, 0x0A },
					new List<byte> { 0x29, 0x2A },
			}),
			(MapLocation.ElflandCastle, new List<List<byte>>() {
					new List<byte> { 0x45, 0x0B, 0x0C, 0x45 },
					new List<byte> { 0x4C, 0x1B, 0x1C, 0x4C },
					new List<byte> { 0x4C, 0x45, 0x45, 0x4C },
			}),
			(MapLocation.MarshCave1, new List<List<byte>>() {
					new List<byte> { 0x6E },
			}),
			(MapLocation.Melmond, new List<List<byte>>() {
					new List<byte> { 0x4D, 0x45 },
					new List<byte> { 0x4D, 0x4D },
			}),
			(MapLocation.EarthCave1, new List<List<byte>>() {
					new List<byte> { 0x45, 0x45, 0x10, 0x11, 0x12 },
					new List<byte> { 0x10, 0x11, 0x21, 0x21, 0x33 },
					new List<byte> { 0x20, 0x21, 0x0E, 0x21, 0x12 },
					new List<byte> { 0x30, 0x33, 0x45, 0x30, 0x33 },
			}),
			(MapLocation.SardasCave, new List<List<byte>>() {
					new List<byte> { 0x45, 0x10, 0x11, 0x11, 0x11, 0x12, 0x45 },
					new List<byte> { 0x45, 0x30, 0x3A, 0x31, 0x35, 0x21, 0x12 },
					new List<byte> { 0x10, 0x12, 0x45, 0x45, 0x45, 0x30, 0x33 },
					new List<byte> { 0x30, 0x21, 0x11, 0x11, 0x11, 0x11, 0x12 },
					new List<byte> { 0x45, 0x30, 0x31, 0x31, 0x31, 0x34, 0x33 },
			}),
			(MapLocation.CrescentLake, new List<List<byte>>() {
					new List<byte> { 0x45, 0x2C, 0x2D, 0x2E, 0x45 },
					new List<byte> { 0x3B, 0x3C, 0x3D, 0x3E, 0x3F },
					new List<byte> { 0x4B, 0x3D, 0x4E, 0x4E, 0x4F },
					new List<byte> { 0x5B, 0x4E, 0x3D, 0x4E, 0x5F },
					new List<byte> { 0x6B, 0x4E, 0x3D, 0x3D, 0x6F },
					new List<byte> { 0x7B, 0x7D, 0x3D, 0x7E, 0x7F },
			}),
			(MapLocation.GurguVolcano1, new List<List<byte>>() {
					new List<byte> { 0x64, 0x65 },
					new List<byte> { 0x74, 0x75 },
			}),
			(MapLocation.IceCave1, new List<List<byte>>() {
					new List<byte> { 0x10, 0x11, 0x11, 0x11, 0x12 },
					new List<byte> { 0x30, 0x31, 0x2B, 0x21, 0x22 },
					new List<byte> { 0x45, 0x45, 0x45, 0x30, 0x33 },
					new List<byte> { 0x10, 0x11, 0x11, 0x11, 0x12 },
					new List<byte> { 0x30, 0x31, 0x31, 0x31, 0x33 },
			}),
			(MapLocation.Caravan, new List<List<byte>>() {
					new List<byte> { 0x45, 0x13, 0x13, 0x45 }, // palm tree
					new List<byte> { 0x45, 0x36, 0x40, 0x41 }, // change caravan tile to tent
					new List<byte> { 0x13, 0x45, 0x50, 0x51 },
			}),
			(MapLocation.Onrac, new List<List<byte>>() {
					new List<byte> { 0x5D, 0x5D },
					new List<byte> { 0x5D, 0x5D }, 
			}),
			(MapLocation.Waterfall, new List<List<byte>>() {
					new List<byte> { 0x46 }, // Find something for waterfall
			}),
			(MapLocation.Cardia1, new List<List<byte>>() {
					new List<byte> { 0x6A },
			}),
			(MapLocation.Cardia2, new List<List<byte>>() {
					new List<byte> { 0x66 },
			}),
			(MapLocation.Cardia4, new List<List<byte>>() {
					new List<byte> { 0x67 },
			}),
			(MapLocation.Cardia5, new List<List<byte>>() {
					new List<byte> { 0x68 },
			}),
			(MapLocation.Cardia6, new List<List<byte>>() {
					new List<byte> { 0x69 },
			}),
			(MapLocation.BahamutCave1, new List<List<byte>>() {
					new List<byte> { 0x6C },
			}),
			(MapLocation.CastleOrdeals1, new List<List<byte>>() {
					new List<byte> { 0x0B, 0x0C },
					new List<byte> { 0x38, 0x39 },
			}),
			(MapLocation.MirageTower1, new List<List<byte>>() {
					new List<byte> { 0x0D, 0x45 },
					new List<byte> { 0x1D, 0x1E },
			}),
			(MapLocation.Gaia, new List<List<byte>>() {
					new List<byte> { 0x45, 0x5A, 0x5A },
					new List<byte> { 0x5A, 0x5A, 0x45 },
			}),
			(MapLocation.Lefein, new List<List<byte>>() {
					new List<byte> { 0x45, 0x2C, 0x2D, 0x2E, 0x45 },
					new List<byte> { 0x3B, 0x3C, 0x6D, 0x3E, 0x3F },
					new List<byte> { 0x4B, 0x6D, 0x6D, 0x6D, 0x4F },
					new List<byte> { 0x5C, 0x7D, 0x3D, 0x7E, 0x5E },
			}),
		};

		public List<MapEdit> ConvertTileArrayToMapEdit(List<List<byte>> mapedit, int target_x, int target_y)
		{
			List<MapEdit> convertedMapEdit = new();

			for (int y = 0; y < mapedit.Count; y++)
			{
				for (int x = 0; x < mapedit[y].Count; x++)
				{
					convertedMapEdit.Add(new MapEdit { X = (byte)(x + target_x), Y = (byte)(y + target_y), Tile = (byte)mapedit[y][x] });
				}
			}

			return convertedMapEdit;
		}


		public List<MapDirection> InvalidDirections(MapDirection direction)
		{

			List<MapDirection> invalidDirections = new();

			invalidDirections.Add((MapDirection)(((int)direction + 4) % 8));
			invalidDirections.Add((MapDirection)(((int)direction + 3) % 8));
			invalidDirections.Add((MapDirection)(((int)direction + 5) % 8));

			return invalidDirections;
		}
		public void GenerateDesert(OverworldMap overworldmap, MT19337 rng)
		{

			int LoopedValue(int value, int max) => (value < 0) ? (max + value) % max : (value % max);

			List<List<MapLocation>> MapGrid = new();

			for (int i = 0; i < 16; i++)
			{
				MapGrid.Add(new List<MapLocation> { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 });
			}

			List<(MapLocation, MapLocation)> nodepairs = new()
			{
				(MapLocation.ConeriaCastle1, MapLocation.TempleOfFiends1),
				(MapLocation.ConeriaCastle1, MapLocation.MatoyasCave),
				(MapLocation.ConeriaCastle1, MapLocation.Pravoka),
				(MapLocation.Pravoka, MapLocation.DwarfCave),
				(MapLocation.Pravoka, MapLocation.NorthwestCastle),
				(MapLocation.Pravoka, MapLocation.Elfland),
				(MapLocation.Elfland, MapLocation.MarshCave1),
				(MapLocation.Elfland, MapLocation.Melmond),
				(MapLocation.Elfland, MapLocation.CrescentLake),
				(MapLocation.Melmond, MapLocation.EarthCave1),
				(MapLocation.Melmond, MapLocation.SardasCave),
				(MapLocation.Melmond, MapLocation.Onrac),
				(MapLocation.CrescentLake, MapLocation.GurguVolcano1),
				(MapLocation.CrescentLake, MapLocation.IceCave1),
				(MapLocation.CrescentLake, MapLocation.Gaia),
				(MapLocation.Onrac, MapLocation.Waterfall),
				(MapLocation.Onrac, MapLocation.Caravan),
				(MapLocation.Onrac, MapLocation.CastleOrdeals1),
				(MapLocation.Gaia, MapLocation.MirageTower1),
				(MapLocation.Gaia, MapLocation.Lefein),
				(MapLocation.Gaia, MapLocation.BahamutCave1)
			};

			List<(MapLocation, int)> distance = new()
			{
				(MapLocation.ConeriaCastle1, 1),
				(MapLocation.Pravoka, 1),
				(MapLocation.Elfland, 2),
				(MapLocation.Melmond, 2),
				(MapLocation.CrescentLake, 2),
				(MapLocation.Onrac, 3),
				(MapLocation.Gaia, 3),
			};

			List<(int, int)> DirectionCoordinates = new()
			{
				(0, -1),
				(-1, -1),
				(-1, 0),
				(-1, 1),
				(0, 1),
				(1, 1),
				(1, 0),
				(1, -1)
			};


			List<(MapLocation, int, int)> nodePositions = new();
			List<(MapLocation, MapDirection)> nodeDirection = new();

			List<(int, int)> placedSquares = new();

			bool validmap = false;
			int attempt_count = 0;
			while (!validmap)
			{
				attempt_count++;
				nodePositions.Clear();
				nodeDirection.Clear();
				MapGrid.Clear();

				for (int i = 0; i < 16; i++)
				{
					MapGrid.Add(new List<MapLocation> { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 });
				}

				nodePositions.Add((MapLocation.ConeriaCastle1, Rng.Between(rng, 7, 9), Rng.Between(rng, 7, 9)));

				MapGrid[nodePositions[0].Item2][nodePositions[0].Item3] = nodePositions[0].Item1;

				bool placementerror = false;

				foreach (var node in distance)
				{
					//bool validnode = false;
					
					//int node_check = 0;

					//node_check++;

					var loctoplace = nodepairs.Where(x => x.Item1 == node.Item1).ToList();
					var origin = (nodePositions.Find(x => x.Item1 == node.Item1).Item2, nodePositions.Find(x => x.Item1 == node.Item1).Item3);
					var originDirection = nodeDirection.Find(x => x.Item1 == node.Item1).Item2;

					foreach (var loc in loctoplace)
					{
						MapDirection direction = MapDirection.North;
						bool validSquare = false;
						List<MapDirection> validDirections = Enum.GetValues(typeof(MapDirection)).Cast<MapDirection>().ToList();
						if (loc.Item1 != MapLocation.ConeriaCastle1)
						{
							validDirections = validDirections.Where(x => !InvalidDirections(originDirection).Contains(x)).ToList();
						}

						List<MapDirection> invalidDirections = new();
						(int, int) newOrigin = (0, 0);
						int loop_check = 0;

						while (!validSquare)
						{
							loop_check++;
							if (loop_check > 20)
							{
								placementerror = true;
								break;
							}
							direction = (MapDirection)Rng.Between(rng, 0, 7);
							int target_x = LoopedValue(origin.Item1 + DirectionCoordinates[(int)direction].Item1, 16);
							int target_y = LoopedValue(origin.Item2 + DirectionCoordinates[(int)direction].Item2, 16);

							if (MapGrid[target_x][target_y] == 0)
							{
								validDirections = validDirections.Where(x => !InvalidDirections(direction).Contains(x)).ToList();

								Console.WriteLine(loc.Item2);

								foreach (var dir in validDirections)
								{
									Console.WriteLine(dir);
								}
							
							newOrigin = (target_x, target_y);
							//invalidDirections.AddRange(InvalidDirections(direction));
							validSquare = true;
							}
						}

				
						for (int i = 0; i < node.Item2; i++)
						{
							MapGrid[newOrigin.Item1][newOrigin.Item2] = MapLocation.AirshipLocation;
							placedSquares.Add((newOrigin.Item1, newOrigin.Item2));
							validSquare = false;
							loop_check = 0;

							while (!validSquare)
							{
								loop_check++;
								if (loop_check > 20)
								{
									placementerror = true;
									break;
								}

								direction = validDirections.PickRandom(rng);
								int target_x = LoopedValue(newOrigin.Item1 + DirectionCoordinates[(int)direction].Item1, 16);
								int target_y = LoopedValue(newOrigin.Item2 + DirectionCoordinates[(int)direction].Item2, 16);

								if (MapGrid[target_x][target_y] == 0)
								{
									validDirections = validDirections.Where(x => !InvalidDirections(direction).Contains(x)).ToList();

									Console.WriteLine(loc.Item2);

									foreach (var dir in validDirections)
									{
										Console.WriteLine(dir);
									}
									newOrigin = (target_x, target_y);
									validSquare = true;
								}
							}
						}

						MapGrid[newOrigin.Item1][newOrigin.Item2] = loc.Item2;
						placedSquares.Add((newOrigin.Item1, newOrigin.Item2));

						if (distance.Select(x => x.Item1).Contains(loc.Item2))
						{
							nodePositions.Add((loc.Item2, newOrigin.Item1, newOrigin.Item2));
							nodeDirection.Add((loc.Item2, direction));
						}
					}
				}

				if (placementerror)
				{
					if (attempt_count > 20)
					{
						for (int i = 0; i < 16; i++)
						{
							var rowvalues = string.Join(";", MapGrid.Select(x => ((int)x[i]).ToString("X2")));
							Console.WriteLine(rowvalues);
						}

						throw new InsaneException("Bzzz!");
					}

					validmap = false;
				}
				else
				{
					validmap = true;
				}
			}


			Console.WriteLine("Attempt: " + attempt_count);

			for (int i = 0; i < 16; i++)
			{
				var rowvalues = string.Join(";", MapGrid.Select(x => ((int)x[i]).ToString("X2")));
				Console.WriteLine(rowvalues);

			}


			List<byte> newTileList = new() { 0x03, 0x04, 0x05, 0x13, 0x14, 0x15 };
			for (int x = 0; x < 16; x++)
			{

				for (int y = 0; y < 16; y++)
				{
					if (MapGrid[x][y] != 0)
					{
						if (MapGrid[x][y] == MapLocation.AirshipLocation)
						{
							overworldmap.MapEditsToApply.Add(new List<MapEdit>
								{
								new MapEdit{X = (byte)((x * 16) + Rng.Between(rng, 0,15)), Y = (byte)((y * 16) + Rng.Between(rng, 0,15)), Tile = newTileList.PickRandom(rng)},
								});
						}
						else
						{
							if (MapModifications.Where(z => z.Item1 == MapGrid[x][y]).Any())
							{
								var targetmod = MapModifications.Find(z => z.Item1 == MapGrid[x][y]);

								overworldmap.MapEditsToApply.Add(ConvertTileArrayToMapEdit(targetmod.Item2,
									(x * 16) + Rng.Between(rng, 0, 15 - targetmod.Item2[0].Count),
									(y * 16) + Rng.Between(rng, 0, 15 - targetmod.Item2.Count)));

							}
							else
							{ 
							overworldmap.MapEditsToApply.Add(new List<MapEdit>
								{
								new MapEdit{X = (byte)((x * 16) + Rng.Between(rng, 0,15)), Y = (byte)((y * 16) + Rng.Between(rng, 0,15)), Tile = OWTileLink.Find(maploc => maploc.Item1 == MapGrid[x][y]).Item2},
								});
							}
						}
					}

				}

			}



			// Palette
			for(int i = 1; i<16; i+=4)
			{
				Put(0x0380+i, Blob.FromHex("37"));
			}

			List<byte> holes = new List<byte> { 0x66, 0x67, 0x68, 0x69, 0x6A, 0x6C, 0x6E };

			foreach (var hole in holes)
			{
				Put(0x0300 + hole, Blob.FromHex("55"));
			}

			List<byte> newTilesPalette = new List<byte> { 0x14, 0x15 };

			foreach (var newtile in newTilesPalette)
			{
				Put(0x0300 + newtile, Blob.FromHex("55"));
			}

			// New graphic tiles
			var newGraphicTiles =
				"FFFFFFFFFFF7E36300000000000C1EDE" +
				"FFFF7F7F7FFFFF9F0080C0C0C0008080" +
				"E36382E3E3D3B9A0CAFF77161E3E6763" +
				"FFFFFFFFFFFFFBEB0000000000000C1E" +
				"FFBFFFBF7FFF3F1F00C0C0C080000000" +
				"7BEB6F8BEAEBFAEADEDEFE7F1F1E1E1E" +
				"FFFFEFE7ADA7EFE90000181A5E781818" +
				"C680051DF0E2C2CC397DE6870E5A310B" +
				"F926A3091209000006D9509EF1FC9FB1" +
				"FCFDFDFFFFF6E0FC0A0A0A480C040000" +
				"66E2FBFFFF60233F3844404040602030" +
				"E181C0808080C0BFE3D9DCBCBEBEC0FF" +
				"EDBFFBDF767F0BBFFFFFFFFF7F7F1FFF" +
				"EDBFFBC7820100BFFFFFFFC7B37900FF" +
				"F6BFFBDFF6FFE8B9FFFFFFFFFFFFFCFB" +
				"EDBFFBDFF6FF2BDFFFFFFFFFFFFF3FDF" +
				"E1A0C28505A0A0BFF5EAD7AD0DA8A0FF" +
				"EDF743BD808701BFEFF77BBD80B701FF";

			Put(0x8210, Blob.FromHex(newGraphicTiles));

			List<(byte, byte, byte, byte, byte)> tilesToUpdate = new()
			{
				(0x03, 0x21, 0x20, 0x23, 0x22), // Cactuar
				(0x04, 0x24, 0x20, 0x26, 0x25), // Cactus1
				(0x05, 0x24, 0x27, 0x26, 0x25), // Cactus2
				(0x13, 0x28, 0x29, 0x2A, 0x2B), // Palm
				(0x14, 0x54, 0x2E, 0x2C, 0x2D), // Rock1
				(0x15, 0x54, 0x54, 0x2C, 0x2D), // Rock2
				(0x36, 0x2F, 0x30, 0x31, 0x32), // Caravan
			};

			foreach (var tile in tilesToUpdate)
			{
				Put(0x0100 + tile.Item1, new byte[] { tile.Item2 });
				Put(0x0180 + tile.Item1, new byte[] { tile.Item3 });
				Put(0x0200 + tile.Item1, new byte[] { tile.Item4 });
				Put(0x0280 + tile.Item1, new byte[] { tile.Item5 });
			}

			// Placement

			// Ship

		}



	}

		
	
}
