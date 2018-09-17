using RomUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FF1Lib
{
	public interface IMapGeneratorEngine
	{
		CompleteMap Generate(MT19337 rng, MapRequirements reqs);
	}

	public class WaterfallEngine : IMapGeneratorEngine
	{
		private enum Directions : int
		{
			up = 0,
			left,
			right,
			down
		}

		public CompleteMap Generate(MT19337 rng, MapRequirements reqs)
		{
			CompleteMap complete = new CompleteMap
			{
				Map = new Map((byte)Tile.WaterfallInside)
			};

			//Map map = complete.Map;

			var startLoc = (x: 0x39, y: 0x38);
			var roomLoc = (x: 0x35, y: 0x30);

			var startingX = rng.Between(-3, 0) + startLoc.x;
			var startingY = rng.Between(-4, 0) + startLoc.y;
			var endPoint = (x: startingX, y: startingY);

			complete.Map.Fill(endPoint, (4, 5), 0x49);

			List<(int x, int y)> visited = new List<(int x, int y)>();
			List<List<Directions>> visitedDir = new List<List<Directions>>();
			var index = 0;
			var outerLoops = 0;
			Directions curDirection = (Directions)rng.Between(0, 3);
			visited.Add(endPoint);
			visitedDir.Add(FullDirs());
			visitedDir[0].Remove(curDirection);
			do
			{
				var iterCount = rng.Between(8, 20);
				var offset = endPoint;
				for (int j = 0; j < iterCount; j++)
				{
					var newOffset = (x: 0, y: 0);
					switch (curDirection)
					{
						case Directions.up:
							newOffset = (x: offset.x + rng.Between(-2, 2), y: offset.y - rng.Between(2, 4));
							break;
						case Directions.down:
							newOffset = (x: offset.x + rng.Between(-2, 2), y: offset.y + rng.Between(2, 4));
							break;
						case Directions.right:
							newOffset = (x: offset.x + rng.Between(2, 4), y: offset.y + rng.Between(-2, 3));
							break;
						case Directions.left:
							newOffset = (x: offset.x - rng.Between(2, 4), y: offset.y + rng.Between(-2, 3));
							break;
					}
					if (newOffset.x < 2 || newOffset.x > 57 || newOffset.y < 2 || newOffset.y > 56)
						break;
					offset = newOffset;
					complete.Map.Fill(offset, (4, 5), 0x49);
				}
				if (offset.x != endPoint.x && offset.y != endPoint.y)
				{
					visited.Add(offset);
					List<Directions> newDirs = FullDirs();
					newDirs.Remove(3 - curDirection);
					visitedDir.Add(newDirs);
				}

				index = rng.Between(0, visited.Count - 1);
				endPoint = visited[index];
				var randDirection = rng.Between(0, visitedDir[index].Count - 1);
				curDirection = visitedDir[index][randDirection];
				outerLoops++;
			} while (outerLoops < 35);

			//Now, we need to smooth out any 1x1 chunks, because those don't render well

			var smoothIterations = 0;
			while (SmoothFilter(complete.Map, Tile.WaterfallInside, Tile.WaterfallRandomEncounters))
				smoothIterations++;
			//SmoothFilter(complete.Map, Tile.WaterfallInside, Tile.WaterfallRandomEncounters);

			List<Tile> liveTiles = new List<Tile>() { Tile.WaterfallRandomEncounters };

			var distances = FloodFillDist(complete.Map, startLoc, liveTiles);
			int maxDist = distances.Cast<int>().Max();

			List<(int x, int y)> roomPlacements = new List<(int x, int y)>();
			for (var i = 1; i < 54; i++)
			{
				for (var j = 1; j < 54; j++)
				{
					if (Math.Abs(i - startLoc.x) + Math.Abs(j - startLoc.y) > 64)
					{
						roomPlacements.Add((x: i, y: j));
					}
				}
			}

			//Clean up the 1-by-1 chunks

			var doneWithRoomPlacement = false;
			List<Tile> targetTiles = new List<Tile>() { Tile.Doorway };
			//Now, pick a place for the room.
			do
			{
				var targetPlace = rng.Between(0, roomPlacements.Count - 1);
				roomLoc = roomPlacements[targetPlace];
				roomPlacements.RemoveAt(targetPlace);

				var tempRoom = (byte[,])reqs.Rooms.First().Tiledata.Clone();
				// Place the room
				for (var i = 0; i < 6; i++)
				{
					for (var j = 0; j < 8; j++)
					{
						tempRoom[i, j] = complete.Map[roomLoc.y + i, roomLoc.x + j];
						complete.Map[roomLoc.y + i, roomLoc.x + j] = reqs.Rooms.First().Tiledata[i, j];
					}
				}
				// Test the room
				doneWithRoomPlacement = FloodFillReachable(complete.Map, startLoc, liveTiles, targetTiles)[Tile.Doorway];
				//If not done, reverse
				if (!doneWithRoomPlacement)
				{
					for (var i = 0; i < 6; i++)
					{
						for (var j = 0; j < 8; j++)
						{
							complete.Map[roomLoc.y + i, roomLoc.x + j] = tempRoom[i, j];
						}
					}
				}

			} while (!doneWithRoomPlacement);

			byte[] tempOutside = { (byte)Tile.WaterfallRandomEncounters, (byte)Tile.Doorway, (byte)Tile.InsideWall };
			List<byte> outsideTiles = new List<byte>(tempOutside);
			//Okay, now, we need to to do the touch up!
			for (var i = 0; i < 63; i++)
			{
				for (var j = 0; j < 63; j++)
				{
					var curTile = (Tile)complete.Map[j, i];

					if (curTile == Tile.WaterfallInside && i != 0 && outsideTiles.Contains(complete.Map[j, i - 1]))
					{
						curTile = Tile.RoomLeft;
					}
					if (curTile == Tile.WaterfallInside && i != 63 && outsideTiles.Contains(complete.Map[j, i + 1]))
					{
						curTile = Tile.RoomRight;
					}
					if (curTile == Tile.WaterfallInside && j != 63 && outsideTiles.Contains(complete.Map[j + 1, i]))
					{
						curTile = Tile.RoomFrontCenter;
					}
					if (curTile == Tile.RoomLeft && j != 63 && outsideTiles.Contains(complete.Map[j + 1, i]))
					{
						curTile = Tile.RoomFrontLeft;
					}
					if (curTile == Tile.RoomRight && j != 63 && outsideTiles.Contains(complete.Map[j + 1, i]))
					{
						curTile = Tile.RoomFrontRight;
					}
					if (curTile == Tile.WaterfallInside && j != 0 && outsideTiles.Contains(complete.Map[j - 1, i]))
					{
						curTile = Tile.RoomBackCenter;
					}
					if (curTile == Tile.RoomLeft && j != 0 && outsideTiles.Contains(complete.Map[j - 1, i]))
					{
						curTile = Tile.RoomBackLeft;
					}
					if (curTile == Tile.RoomRight && j != 0 && outsideTiles.Contains(complete.Map[j - 1, i]))
					{
						curTile = Tile.RoomBackRight;
					}
					if (curTile == Tile.WaterfallRandomEncounters && j != 0 && (complete.Map[j - 1, i] <= 0x08 || complete.Map[j - 1, i] == 0x46))
						curTile = Tile.InsideWall;
					complete.Map[j, i] = (byte)curTile;
				}
			}

			//Finally, add the start
			complete.Map[startLoc.y, startLoc.x] = (byte)Tile.WarpUp;
			//and the robot.
			reqs.Rooms.First().NPCs.ToList().ForEach(npc =>
			{
				npc.Coord.x += roomLoc.x;
				npc.Coord.y += roomLoc.y;
				reqs.Rom.MoveNpc(reqs.MapId, npc);
			});

			complete.Entrance = new Coordinate((byte)startLoc.x, (byte)startLoc.y, CoordinateLocale.Standard);

			return complete;
		}

		private T[,] TransposeFilter<T>(T[,] filter)
		{
			T[,] newFilter = (T[,])filter.Clone();
			for (var i = 0; i < filter.GetLength(0); i++)
			{
				for (var j = 0; j < filter.GetLength(1); j++)
				{
					newFilter[j, i] = filter[i, j];
				}
			}
			return newFilter;
		}

		//Looks for patterns of single smoothTarget tiles and massages them out.
		//Uses a more general Filter function that we set up
		//But the Equality2DArrayComparer is the big trick to get this to work
		//Returns true if changes were made
		private bool SmoothFilter(Map map, Tile smoothTarget, Tile background)
		{
			byte bgTile = (byte) background;
			byte target = (byte) smoothTarget;

			byte[,] pattern1 = {
				{ bgTile, bgTile, bgTile },
				{ bgTile, target, bgTile },
				{ bgTile, bgTile, bgTile } };

			byte[,] replacement1 = {
				{ bgTile, bgTile, bgTile },
				{ bgTile, bgTile, bgTile },
				{ bgTile, bgTile, bgTile } };

			byte[,] pattern2 = {
				{ bgTile, bgTile, bgTile },
				{ bgTile, bgTile, target },
				{ bgTile, bgTile, bgTile } };

			byte[,] replacement2 = {
				{ bgTile, bgTile, bgTile },
				{ bgTile, bgTile, bgTile },
				{ bgTile, bgTile, bgTile } };

			byte[,] pattern3 = {
				{ bgTile, bgTile, bgTile },
				{ target, bgTile, bgTile },
				{ bgTile, bgTile, bgTile } };

			byte[,] replacement3 = {
				{ bgTile, bgTile, bgTile },
				{ bgTile, bgTile, bgTile },
				{ bgTile, bgTile, bgTile } };

			byte[,] pattern4 = {
				{ bgTile, bgTile, bgTile },
				{ bgTile, target, target },
				{ bgTile, bgTile, bgTile } };

			byte[,] replacement4 = {
				{ bgTile, bgTile, bgTile },
				{ bgTile, bgTile, bgTile },
				{ bgTile, bgTile, bgTile } };

			byte[,] pattern5 = {
				{ bgTile, bgTile, bgTile },
				{ target, target, bgTile },
				{ bgTile, bgTile, bgTile } };

			byte[,] replacement5 = {
				{ bgTile, bgTile, bgTile },
				{ bgTile, bgTile, bgTile },
				{ bgTile, bgTile, bgTile } };

			byte[,] pattern6 = {
				{ bgTile, bgTile, bgTile },
				{ target, target, target },
				{ bgTile, bgTile, bgTile } };

			byte[,] replacement6 = {
				{ bgTile, bgTile, bgTile },
				{ bgTile, bgTile, bgTile },
				{ bgTile, bgTile, bgTile } };

			Dictionary<byte[,], byte[,]> filter = new Dictionary<byte[,], byte[,]>(new Equality2DArrayComparer())
			{
				{ pattern1, replacement1 },
				{ pattern2, replacement2 },
				{ pattern3, replacement3 },
				{ pattern4, replacement4 },
				{ pattern5, replacement5 },
				{ pattern6, replacement6 },
				{ TransposeFilter(pattern2), TransposeFilter(replacement2) },
				{ TransposeFilter(pattern3), TransposeFilter(replacement3) },
				{ TransposeFilter(pattern4), TransposeFilter(replacement4) },
				{ TransposeFilter(pattern5), TransposeFilter(replacement5) },
				{ TransposeFilter(pattern6), TransposeFilter(replacement6) }
			};

			var tetst = filter.ContainsKey(pattern1);
			return map.Filter(filter, (x: 3, y: 3));
		}

		//A class that allows us to actually compare two byte[,] arrays
		public class Equality2DArrayComparer : IEqualityComparer<byte[,]>
		{
			public bool Equals(byte[,] x, byte[,] y)
			{
				if (x.Length != y.Length || x.GetLength(0) != y.GetLength(0) || x.GetLength(1) != y.GetLength(1))
				{
					return false;
				}
				for (var i = 0; i < x.GetLength(0); i++)
				{
					for (var j = 0; j < x.GetLength(1); j++)
					{
						if (x[i, j] != y[i, j])
							return false;
					}
				}
				return true;
			}

			public int GetHashCode(byte[,] obj)
			{
				int result = 17;
				for (int i = 0; i < obj.GetLength(0); i++)
				{
					for (int j = 0; j < obj.GetLength(1); j++)
						unchecked
						{
							result = result * 23 + obj[i, j];
						}
				}
				return result;
			}
		}

		private List<Directions> FullDirs()
		{
			List<Directions> allDirs = new List<Directions>
			{
				Directions.up,
				Directions.down,
				Directions.left,
				Directions.right
			};
			return allDirs;
		}

		private int[,] FloodFillDist(Map map, (int x, int y) start, IEnumerable<Tile> live)
		{
			int[,] dist = new int[Map.RowCount, Map.RowLength];
			dist[start.x, start.y] = 1;
			map.Flood(start, ((int x, int y) coord, byte t) =>
			{
				Tile tile = (Tile)t;
				if (live.Contains(tile))
				{
					int minDistance = Map.RowCount * Map.RowLength + 1;
					int upDistance = dist[coord.x, (Map.RowCount + coord.y - 1) % Map.RowCount];
					int downDistance = dist[coord.x, (Map.RowCount + coord.y + 1) % Map.RowCount];
					int rightDistance = dist[(Map.RowLength + coord.x + 1) % Map.RowLength, coord.y];
					int leftDistance = dist[(Map.RowLength + coord.x - 1) % Map.RowLength, coord.y];
					bool updated = false;

					if (upDistance > 0 && upDistance < minDistance)
					{
						minDistance = upDistance;
						updated = true;
					}
					if (downDistance > 0 && downDistance < minDistance)
					{
						minDistance = downDistance;
						updated = true;
					}
					if (rightDistance > 0 && rightDistance < minDistance)
					{
						minDistance = rightDistance;
						updated = true;
					}
					if (leftDistance > 0 && leftDistance < minDistance)
					{
						minDistance = leftDistance;
						updated = true;
					}
					if (updated)
					{
						dist[coord.x, coord.y] = minDistance;
					}
					return true;
				}

				return false;
			});


			return dist;
		}

		private Dictionary<Tile, bool> FloodFillReachable(Map map, (int x, int y) start, IEnumerable<Tile> live, IEnumerable<Tile> targets)
		{
			Dictionary<Tile, bool> results = targets.ToDictionary(tile => tile, tile => false);
			map.Flood(start, ((int x, int y) coord, byte t) =>
			{
				Tile tile = (Tile)t;
				if (targets.Contains(tile))
				{
					results[tile] = true;
					return true;
				}
				if (live.Contains(tile))
				{
					return true;
				}

				return false;
			});
			return results;
		}
	}

	class CellularGenerator : IMapGeneratorEngine
	{
		public double InitialAliveChance { get; set; }
		public int BirthLimit = 4;
		public int DeathLimit = 3;
		public int Steps = 5;

		public CellularGenerator()
		{
			InitialAliveChance = 0.4;
		}

		public CompleteMap Generate(MT19337 rng, MapRequirements reqs)
		{
			int sanity = 0;
			while (true)
			{
				if (++sanity == 500)
					throw new InsaneException("Failed to generate map!");

				CompleteMap complete = new CompleteMap
				{
					Map = new Map((byte)Tile.Lava)
				};

				complete.Map.Put((0, 0), reqs.Rooms.First().Tiledata);
				reqs.Rooms.First().NPCs.ToList().ForEach(npc => reqs.Rom.MoveNpc(reqs.MapId, npc));

				InitializeMap(rng, complete.Map);

				for (int i = 0; i < Steps; ++i)
				{
					complete.Map = DoSimulationStep(complete.Map);
				}

				(int x, int y) coord = (rng.Between(0, MapRequirements.Width - 1), rng.Between(0, MapRequirements.Height - 1));
				complete.Map[coord] = (byte)Tile.WarpUp;
				var results = FloodFill(complete.Map, coord, new List<Tile> { Tile.Doorway, Tile.WarpUp, reqs.Floor }, new List<Tile> { Tile.Lava }, reqs.Floor);

				Console.WriteLine($"FloorGen Results: Floor:{results[reqs.Floor]}, Doorways:{results[Tile.Doorway]}, Warps:{results[Tile.WarpUp]}");

				if (results[reqs.Floor] > 1000 && results[Tile.Doorway] == 1)
				{
					complete.Entrance = new Coordinate((byte)coord.x, (byte)coord.y, CoordinateLocale.Standard);
					return complete;
				}
			}
		}

		private void InitializeMap(MT19337 rng, Map map)
		{
			for (int x = 0; x < Map.RowLength; x++)
			{
				for (int y = 0; y < Map.RowCount; y++)
				{
					if (map[y, x] == (byte)Tile.Lava && ((double)rng.Next() / uint.MaxValue < InitialAliveChance))
					{
						map[y, x] = (byte)Tile.Impassable;
					}
				}
			}
		}

		private Dictionary<Tile, int> FloodFill(Map map, (int, int) coord, IEnumerable<Tile> counts, IEnumerable<Tile> finds, Tile replace)
		{
			Dictionary<Tile, int> results = counts.ToDictionary(tile => tile, tile => 0);
			FloodFill(results, map, coord, counts, finds, replace);
			return results;
		}

		private void FloodFill(Dictionary<Tile, int> results, Map map, (int, int) inCoord, IEnumerable<Tile> counts, IEnumerable<Tile> finds, Tile replace)
		{
			map.Flood(inCoord, ((int x, int y) coord, byte t) =>
			{
				Tile tile = (Tile)t;

				// Update if desired
				if (finds.Contains(tile))
				{
					tile = replace;
					map[coord] = (byte)tile;
				}

				// Accumulate results and recurse if this is a counter tile.
				if (counts.Contains(tile))
				{
					results[tile]++;
					return true;
				}

				return false;
			});
		}

		private Map DoSimulationStep(Map old)
		{
			Map map = old.Clone();
			// Loop over each row and column of the map

			for (int x = 0; x < MapRequirements.Width; x++)
			{
				for (int y = 0; y < MapRequirements.Height; y++)
				{
					int nbs = CountAliveNeighbours(old, x, y);
					//The new value is based on our simulation rules
					//First, if a cell is alive but has too few neighbours, kill it.
					if (old[y, x] == (byte)Tile.Impassable)
					{
						if (nbs < DeathLimit)
						{
							map[y, x] = (byte)Tile.Lava;
						}
						else
						{
							map[y, x] = (byte)Tile.Impassable;
						}
					} //Otherwise, if the cell is dead now, check if it has the right number of neighbours to be 'born'
					else if (old[y, x] == (byte)Tile.Lava)
					{
						if (nbs > BirthLimit)
						{
							map[y, x] = (byte)Tile.Impassable;
						}
						else
						{
							map[y, x] = (byte)Tile.Lava;
						}
					}
					else
					{
						map[y, x] = old[y, x];
					}
				}
			}
			return map;
		}

		//Returns the number of cells in a ring around (x,y) that are alive.
		public int CountAliveNeighbours(Map map, int x, int y)
		{
			int count = 0;
			for (int i = -1; i < 2; i++)
			{
				for (int j = -1; j < 2; j++)
				{
					if (i == 0 && j == 0) continue;

					if (map[(y + j + MapRequirements.Height) % MapRequirements.Height, (x + i + MapRequirements.Width) % MapRequirements.Width] == (byte)Tile.Impassable)
					{
						count = count + 1;
					}
				}
			}
			return count;
		}
	}
}
