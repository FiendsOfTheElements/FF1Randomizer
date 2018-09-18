using FF1Lib.Procgen;
using RomUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FF1Lib.procgen
{
	class RectilinearGenerator : IMapGeneratorEngine
	{
		private byte SentinelDead = 0xFE;
		private byte SentinelAlive = 0xFF;

		private int LocalMax = 48;

		public CompleteMap Generate(MT19337 rng, MapRequirements reqs)
		{
			int sanity = 0;
			while (++sanity < 500)
			{
				CompleteMap complete = new CompleteMap
				{
					Map = new Map(SentinelAlive),
					Requirements = reqs,
				};

				PlaceRoomsAndChests(rng, complete, reqs);

				int hallwayCount = rng.Between(18, 20) * 2;
				for (int i = 0; i < hallwayCount; ++i)
				{
					var dimensions = (rng.Between(2, 8) * 2 + 1, rng.Between(2, 5) * 2 + 1);
					var pos = (rng.Between(0, LocalMax - dimensions.Item1) / 2 * 2, rng.Between(0, LocalMax - dimensions.Item2) / 2 * 2);
					PlaceHallway(complete.Map, pos, dimensions, reqs.Floor);
				}

				foreach (var side in complete.Map.Where(el => (el.Tile == Tile.HallwayLeft || el.Tile == Tile.HallwayRight) && el.Left().Tile == reqs.Floor && el.Right().Tile == reqs.Floor))
				{
					if (rng.Between(0, 2) == 0 && side.Up().Tile != reqs.Floor && side.Down().Tile != reqs.Floor)
					{
						side.Tile = reqs.Floor;
					}
				}

				foreach (var side in complete.Map.Where(el => el.Tile == Tile.InsideWall && el.Up().Tile == reqs.Floor && el.Down().Tile == reqs.Floor))
				{
					if (rng.Between(0, 2) == 0 && side.Left().Tile != reqs.Floor && side.Right().Tile != reqs.Floor)
					{
						side.Tile = reqs.Floor;
					}
				}

				var locations = complete.Map.Where(element => element.Tile == reqs.Floor && element.Surrounding().All(el => el.Tile == reqs.Floor)).ToList();
				MapElement entrance = null;
				reqs.Portals.ToList().ForEach(portal =>
				{
					var location = locations.SpliceRandom(rng);
					complete.Map[location.Y, location.X] = portal;

					if (portal == (byte)Tile.WarpUp)
					{
						entrance = location;
						complete.Entrance = new Coordinate((byte)location.X, (byte)location.Y, CoordinateLocale.Standard);
					}
				});

				var results = FloodFill(complete.Map, entrance.Coord, new List<Tile> { reqs.Floor, Tile.WarpUp, Tile.Doorway });
				if (results[reqs.Floor].Count() < 500 || results[Tile.Doorway].Count() < 3)
				{
					continue;
				}

				PolishWalls(complete.Map, reqs);

				foreach (var el in complete.Map.Where(el => el.Value == SentinelAlive))
				{
					el.Tile = Tile.EarthCaveOOB;
				}

				Console.WriteLine($"Finished RectilinearGenerate in {sanity} attempts.");
				return complete;
			}
			throw new InsaneException("Couldn't finish room in 500 tries.");
		}

		private void PlaceRoomsAndChests(MT19337 rng, CompleteMap complete, MapRequirements reqs)
		{
			int sanity = 0;
			while (++sanity < 100)
			{
				var rooms = complete.Map;
				int roomCount = rng.Between(2, 4);
				(int, int)[] positions = { (rng.Between(0, 8) * 2, rng.Between(0, 6) * 2), (rng.Between(0, 8) * 2, rng.Between(8, 12) * 2),
										   (rng.Between(16, 24) * 2, rng.Between(0, 12) * 2) };
				for (int i = 0; i < positions.Length; ++i)
				{
					var dimensions = (rng.Between(4, 6) * 2 + 1, rng.Between(2, 3) * 2);
					rooms.Fill(positions[i], dimensions, SentinelDead);
				}

				var clone = rooms.Clone();
				foreach (var el in clone.Where(el => el.Value == SentinelDead))
				{
					var roomTile = rooms[el.Coord];
					if (el.Left().Value == SentinelAlive)
					{
						if (el.Up().Value == SentinelAlive)
						{
							roomTile.Tile = Tile.RoomBackLeft;
						}
						else if (el.Down().Value == SentinelAlive)
						{
							roomTile.Tile = Tile.RoomFrontLeft;
							roomTile.Down().Tile = Tile.InsideWall;
						}
						else
						{
							roomTile.Tile = Tile.RoomLeft;
						}
					}
					else if (el.Right().Value == SentinelAlive)
					{
						if (el.Up().Value == SentinelAlive)
						{
							roomTile.Tile = Tile.RoomBackRight;
						}
						else if (el.Down().Value == SentinelAlive)
						{
							roomTile.Tile = Tile.RoomFrontRight;
							roomTile.Down().Tile = Tile.InsideWall;
						}
						else
						{
							roomTile.Tile = Tile.RoomRight;
						}
					}
					else if (el.Up().Value == SentinelAlive)
					{
						roomTile.Tile = Tile.RoomBackCenter;
					}
					else if (el.Down().Value == SentinelAlive)
					{
						roomTile.Tile = Tile.RoomFrontCenter;
						roomTile.Down().Tile = Tile.InsideWall;
					}
					else
					{
						roomTile.Tile = Tile.RoomCenter;
					}
				}

				// Carve out a door to all the accessible room tiles.
				var roomTiles = rooms.Where(el => el.Tile == Tile.RoomCenter);
				var doorways = new List<MapElement> { };

				foreach (var innerTile in roomTiles)
				{
					var results = FloodFill(rooms, innerTile.Coord, new List<Tile> { Tile.RoomCenter, Tile.RoomFrontCenter, Tile.Door });
					if (results[Tile.Door].Any())
					{
						continue;
					}

					var potentialDoorways = results[Tile.RoomFrontCenter].Where(tile => tile.X % 2 == 1).ToList();
					if (potentialDoorways.Any())
					{
						var entryway = potentialDoorways.SpliceRandom(rng);
						var door = entryway.Down();
						var doorway = door.Down();

						System.Diagnostics.Debug.Assert(door.Tile == Tile.InsideWall);

						if (doorway.Value != SentinelAlive)
						{
							continue;
						}

						door.Tile = Tile.Door;
						doorway.Tile = Tile.Doorway;
						doorways.Add(doorway);
					}
				}

				// Place chests now
				var chestLocations = roomTiles.Where(el => el.Up().Tile == Tile.RoomBackCenter).ToList();
				var trapLocations = new List<MapElement>();
				if (reqs.Objects.Count() > chestLocations.Count())
				{
					continue;
				}

				foreach (byte chest in reqs.Objects)
				{
					var location = chestLocations.SpliceRandom(rng);
					location.Value = chest;
					trapLocations.Add(location.Down());
				}

				trapLocations.AddRange(rooms.Where(el => el.Down().Tile == Tile.Door));
				foreach (byte trap in reqs.Traps)
				{
					var location = trapLocations.SpliceRandom(rng);
					location.Value = trap;
				}


				complete.Map = rooms;
				return;
			}
			throw new InsaneException("Couldn't place rooms.");
		}

		private void PlaceHallway(Map map, (int x, int y) coord, (int w, int h) dimensions, Tile center)
		{
			for (int i = 0; i < dimensions.h; ++i)
			{
				for (int j = 0; j < dimensions.w; ++j)
				{
					Update(map[(coord.x + j, coord.y + i)], HallwayTile((j, i), dimensions, center), center);
				}
			}
		}

		private void Update(MapElement last, Tile next, Tile floor)
		{
			if (last.Value == SentinelAlive)
			{
				last.Tile = next;
			}
			else if ((last.Tile == Tile.InsideWall && next == Tile.HallwayLeft)
				|| (last.Tile == Tile.HallwayLeft && next == Tile.InsideWall))
			{
				last.Tile = Tile.WallLeft;
			}
			else if ((last.Tile == Tile.InsideWall && next == Tile.HallwayRight)
				|| (last.Tile == Tile.HallwayRight && next == Tile.InsideWall))
			{
				last.Tile = Tile.WallRight;
			}
		}

		private void PolishWalls(Map map, MapRequirements reqs)
		{
			foreach (var tile in map.Where(tile => tile.Tile == Tile.WallLeft))
			{
				if (tile.Down().Tile == Tile.HallwayRight)
				{
					tile.Tile = Tile.WallRight;
				}
				else if (tile.Down().Tile != Tile.HallwayLeft)
				{
					tile.Tile = Tile.InsideWall;
				}
				else if (tile.Right().Tile == reqs.Floor && tile.Left().Tile == reqs.Floor)
				{
					tile.Tile = Tile.HallwayLeft;
				}
			}
			foreach (var tile in map.Where(tile => tile.Tile == Tile.WallRight))
			{
				if (tile.Down().Tile == Tile.HallwayLeft)
				{
					tile.Tile = Tile.WallLeft;
				}
				else if (tile.Down().Tile != Tile.HallwayRight)
				{
					tile.Tile = Tile.InsideWall;
				}
				else if (tile.Right().Tile == reqs.Floor && tile.Left().Tile == reqs.Floor)
				{
					tile.Tile = Tile.HallwayRight;
				}
			}
			foreach (var tile in map.Where(tile => tile.Tile == Tile.InsideWall))
			{
				if (tile.Down().Tile == Tile.HallwayLeft)
				{
					tile.Tile = Tile.WallLeft;
				}
				else if (tile.Down().Tile == Tile.HallwayRight)
				{
					tile.Tile = Tile.WallRight;
				}
			}

		}

		private Tile HallwayTile((int x, int y) coord, (int w, int h) dimensions, Tile center)
		{
			if (coord.y == dimensions.h - 1) return Tile.InsideWall;
			if (coord.y == 0)
			{
				if (coord.x == 0) return Tile.WallLeft;
				if (coord.x == dimensions.w - 1) return Tile.WallRight;
				return Tile.InsideWall;
			}
			if (coord.x == 0) return Tile.HallwayLeft;
			if (coord.x == dimensions.w - 1) return Tile.HallwayRight;
			return center;
		}

		private Dictionary<Tile, List<MapElement>> FloodFill(Map map, (int, int) coord, IEnumerable<Tile> counts, IEnumerable<Tile> finds = null, Tile replace = Tile.FloorSafe)
		{
			var results = counts.ToDictionary(tile => tile, tile => new List<MapElement> { });
			FloodFill(results, map, coord, counts, finds, replace);
			return results;
		}

		private void FloodFill(Dictionary<Tile, List<MapElement>> results, Map map, (int, int) inCoord, IEnumerable<Tile> counts, IEnumerable<Tile> finds = null, Tile replace = Tile.FloorSafe)
		{
			map.Flood(inCoord, element =>
			{
				// Update if desired
				bool? found = finds?.Contains(element.Tile);
				if (found.HasValue && found.Value)
				{
					element.Tile = replace;
				}

				// Accumulate results and recurse if this is a counter tile.
				if (counts.Contains(element.Tile))
				{
					results[element.Tile].Add(element);
					return true;
				}

				return false;
			});
		}
	}
}
