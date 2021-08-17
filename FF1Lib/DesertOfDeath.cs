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

			nodePositions.Add((MapLocation.ConeriaCastle1, Rng.Between(rng, 7, 9), Rng.Between(rng, 7, 9)));


			foreach (var node in distance)
			{
				var loctoplace = nodepairs.Where(x => x.Item1 == node.Item1);
				var origin = (nodePositions.Find(x => x.Item1 == node.Item1).Item2, nodePositions.Find(x => x.Item1 == node.Item1).Item3);
				foreach (var loc in loctoplace)
				{
					MapDirection direction = MapDirection.North;
					bool validSquare = false;
					List<MapDirection> validDirections = Enum.GetValues(typeof(MapDirection)).Cast<MapDirection>().ToList();
					List<MapDirection> invalidDirections = new();
					(int, int) newOrigin = (0, 0);

					while (!validSquare)
					{
						direction = (MapDirection)Rng.Between(rng, 0, 7);
						if (MapGrid[origin.Item1 + DirectionCoordinates[(int)direction].Item1][origin.Item2 + DirectionCoordinates[(int)direction].Item2] == 0)
						{
							validDirections = validDirections.Where(x => !InvalidDirections(direction).Contains(x)).ToList();
							newOrigin = (origin.Item1 + DirectionCoordinates[(int)direction].Item1, origin.Item2 + DirectionCoordinates[(int)direction].Item2);
							//invalidDirections.AddRange(InvalidDirections(direction));
							validSquare = true;
						}
					}

					for (int i = 0; i < node.Item2; i++)
					{
						MapGrid[newOrigin.Item1 + DirectionCoordinates[(int)direction].Item1][newOrigin.Item2 + DirectionCoordinates[(int)direction].Item2] = MapLocation.AirshipLocation;
						validSquare = false;
						while (!validSquare)
						{
							direction = validDirections.PickRandom(rng);
							if (MapGrid[newOrigin.Item1 + DirectionCoordinates[(int)direction].Item1][newOrigin.Item2 + DirectionCoordinates[(int)direction].Item2] == 0)
							{
								validDirections = validDirections.Where(x => !InvalidDirections(direction).Contains(x)).ToList();
								newOrigin = (newOrigin.Item1 + DirectionCoordinates[(int)direction].Item1, newOrigin.Item2 + DirectionCoordinates[(int)direction].Item2);
								validSquare = true;
							}
						}
					}


				}


			}


			// Palette


			// Placement

			// Ship





		}

		
	}
}
