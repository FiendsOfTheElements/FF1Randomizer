using FF1Lib.Procgen;
using RomUtilities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FF1Lib.procgen
{
	class RectilinearGenerator : IMapGeneratorEngine
	{
		private readonly byte SentinelAlive = 0xFF;

		private readonly int LocalMax = 48;

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

				int hallwayCount = rng.Between(24, 30);
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

				var locations = complete.Map.Where(element => element.Tile == reqs.Floor).ToList();
				Dictionary<Tile, List<MapElement>> results = null;

				var start = locations.SpliceRandom(rng);
				results = FloodFill(complete.Map, start.Coord, new List<Tile> { reqs.Floor, Tile.WarpUp, Tile.Doorway });
				if (results[reqs.Floor].Count() < 500)
				{
					continue;
				}

				// All these locations are reachable so we don't need to sanity that we can reach these portals.
				locations = results[reqs.Floor].Where(element => element.Surrounding().All(el => el.Tile == reqs.Floor)).ToList();
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

				// Place rooms once we have a viable floorplan, and then we'll check again after.
				Dictionary<Tile, int> requiredTiles = reqs.Portals.ToDictionary(portal => (Tile)portal, portal => 1);
				requiredTiles[Tile.Doorway] = PlaceRoomsAndChests(rng, complete, results[reqs.Floor].Select(el => el.Up()).Where(el => el.Tile == Tile.InsideWall).ToList(), reqs);
				requiredTiles[reqs.Floor] = 500;

				results = FloodFill(complete.Map, entrance.Coord, requiredTiles.Keys);
				if (requiredTiles.Any(tileReq => tileReq.Value > results[tileReq.Key].Count))
				{
					Console.WriteLine("Failing due to invalid room placement.");
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

		private byte[,] CreateEmptyRoom((int w, int h) dimensions, int doorX)
		{
			if (dimensions.w < 3 || dimensions.h < 3)
				throw new ArgumentOutOfRangeException();


			byte[,] room = new byte[dimensions.h, dimensions.w];
			for (int y = 1; y < dimensions.h - 2; ++y)
			{
				for (int x = 1; x < dimensions.w - 1; ++x)
				{
					room[y, x] = (byte)Tile.RoomCenter;
				}
			}
			for (int x = 1; x < dimensions.w - 1; ++x)
			{
				room[0, x] = (byte)Tile.RoomBackCenter;
				room[dimensions.h - 2, x] = (byte)Tile.RoomFrontCenter;
				room[dimensions.h - 1, x] = (byte)Tile.InsideWall;
			}
			for (int y = 1; y < dimensions.h - 1; ++y)
			{
				room[y, 0] = (byte)Tile.RoomLeft;
				room[y, dimensions.w - 1] = (byte)Tile.RoomRight;
			}
			room[0, 0] = (byte)Tile.RoomBackLeft;
			room[0, dimensions.w - 1] = (byte)Tile.RoomBackRight;
			room[dimensions.h - 2, 0] = (byte)Tile.RoomFrontLeft;
			room[dimensions.h - 2, dimensions.w - 1] = (byte)Tile.RoomFrontRight;
			room[dimensions.h - 1, 0] = (byte)Tile.InsideWall;
			room[dimensions.h - 1, dimensions.w - 1] = (byte)Tile.InsideWall;

			room[dimensions.h - 1, doorX] = (byte)Tile.Door;

			return room;
		}

		private int PlaceRoomsAndChests(MT19337 rng, CompleteMap complete, List<MapElement> possibilties, MapRequirements reqs)
		{
			int sanity = 0;
			while (++sanity < 100)
			{
				var rooms = complete.Map;
				var roomsAdded = 0;

				var positions = new List<MapElement>();
				var potentialEntrances = possibilties.Where(el => el.Left().Tile == Tile.InsideWall && el.Right().Tile == Tile.InsideWall
					&& el.Down().Tile == reqs.Floor && el.Tile == Tile.InsideWall).ToList();
				while (potentialEntrances.Any())
				{
					var potential = potentialEntrances.SpliceRandom(rng);
					if (!positions.Any(pos => Math.Abs(pos.X - potential.X) < 12 || Math.Abs(pos.Y - potential.Y) < 12))
					{
						positions.Add(potential);
					}
				}

				for (int i = 0; i < 4 && i < positions.Count(); ++i)
				{
					var dimensions = (w: rng.Between(4, 6) * 2 + 1, h: rng.Between(2, 3) * 2 + 1);
					int doorX = rng.Between(0, (dimensions.w - 2) / 2) * 2 + 1;
					byte[,] room = CreateEmptyRoom(dimensions, doorX);

					var roomTarget = (positions[i].X - doorX, positions[i].Y - dimensions.h + 1);
					rooms.Put(roomTarget, room);
					++roomsAdded;
				}

				foreach (var door in rooms.Where(el => el.Tile == Tile.Door))
				{
					door.Down().Tile = Tile.Doorway;
				}

				// Place chests now
				var chestLocations = rooms.Where(el => el.Up().Tile == Tile.RoomBackCenter).ToList();
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
				return roomsAdded;
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
