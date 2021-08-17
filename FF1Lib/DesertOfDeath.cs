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
		public List<MapDirection> InvalidDirections(MapDirection direction)
		{

			List<MapDirection> invalidDirections = new();

			invalidDirections.Add((MapDirection)(((int)direction + 4) % 8));
			invalidDirections.Add((MapDirection)(((int)direction + 3) % 8));
			invalidDirections.Add((MapDirection)(((int)direction + 5) % 8));

			return invalidDirections;
		}
		public void GenerateDesert(MT19337 rng)
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
			nodePositions.Add((MapLocation.ConeriaCastle1, Rng.Between(rng, 7, 9), Rng.Between(rng, 7, 9)));
			MapGrid[nodePositions[0].Item2][nodePositions[0].Item3] = nodePositions[0].Item1;

			foreach (var node in distance)
			{
				bool validnode = false;
				bool placementerror = false;
				int node_check = 0;

				while (!validnode)
				{
					node_check++;

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

						if (placementerror)
						{
							if (node_check > 20)
							{
								for (int i = 0; i < 16; i++)
								{
									var rowvalues = string.Join(";", MapGrid.Select(x => ((int)x[i]).ToString("X2")));
									Console.WriteLine(rowvalues);
								}

								throw new InsaneException("Bzzz!");
							}

							foreach (var square in placedSquares)
							{
								MapGrid[square.Item1][square.Item2] = 0;
							}

							placedSquares.Clear();
							validnode = false;
							Console.WriteLine("Error at: " + node.Item1);


						}
						else
						{
							validnode = true;
						}
					}
				}
			}


			for (int i = 0; i < 16; i++)
			{
				var rowvalues = string.Join(";", MapGrid.Select(x => ((int)x[i]).ToString("X2")));
				Console.WriteLine(rowvalues);

			}
			// Palette


			// Placement

			// Ship





		}

		
	}
}
