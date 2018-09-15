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

			var start_loc = (x: 0x39, y: 0x38); //Currently in (y,x) notation
			var room_loc = (x: 0x35, y: 0x30);

			var starting_x = rng.Between(-3, 0) + start_loc.x;
			var starting_y = rng.Between(-4, 0) + start_loc.y;
			var endPoint = (x: starting_x, y: starting_y);

			complete.Map.Fill(endPoint, (4, 5), 0x49);

			List<(int x, int y)> visited = new List<(int x, int y)>();
			List<List<Directions>> visited_dir = new List<List<Directions>>();
			var index = 0;
			var outerLoops = 0;
			Directions curDirection = (Directions)rng.Between(0, 3);
			visited.Add(endPoint);
			visited_dir.Add(fullDirs());
			visited_dir[0].Remove(curDirection);
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
					List<Directions> newDirs = fullDirs();
					newDirs.Remove(3 - curDirection);
					visited_dir.Add(newDirs);
				}

				//--------//

				index = rng.Between(0, visited.Count - 1);
				endPoint = visited[index];
				var randDirection = rng.Between(0, visited_dir[index].Count - 1);
				curDirection = visited_dir[index][randDirection];
				//curDirection = (Directions)rng.Between(0, 3);
				outerLoops++;
			} while (outerLoops < 35);

			var distances = FloodFillDist(complete.Map, start_loc, 0x49);
			int max_dist = distances.Cast<int>().Max();

			List<(int x, int y)> roomPlacements = new List<(int x, int y)>();
			for (var i = 1; i < 54; i++)
			{
				for (var j = 1; j < 54; j++)
				{
					if (Math.Abs(i - start_loc.x) + Math.Abs(j - start_loc.y) < 64)
					{
						roomPlacements.Add((x: i, y: j));
					}
				}
			}

			var doneWithRoomPlacement = false;
			//Now, pick a place for the room.
			do
			{
				var targetPlace = rng.Between(0, roomPlacements.Count - 1);
				room_loc = roomPlacements[targetPlace];
				roomPlacements.RemoveAt(targetPlace);

				var tempRoom = (byte[,])reqs.Rooms.First().Tiledata.Clone();
				// Place the room
				for (var i = 0; i < 6; i++)
				{
					for (var j = 0; j < 8; j++)
					{
						tempRoom[i, j] = complete.Map[room_loc.y + i, room_loc.x + j];
						complete.Map[room_loc.y + i, room_loc.x + j] = reqs.Rooms.First().Tiledata[i, j];
					}
				}
				// Test the room
				doneWithRoomPlacement = FloodFillReachable(complete.Map, start_loc, 0x49, 0x3A);
				//If not done, reverse
				if (!doneWithRoomPlacement)
				{
					for (var i = 0; i < 6; i++)
					{
						for (var j = 0; j < 8; j++)
						{
							complete.Map[room_loc.y + i, room_loc.x + j] = tempRoom[i, j];
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
			complete.Map[start_loc.y, start_loc.x] = (byte)Tile.WarpUp;
			//and the robot.
			reqs.Rooms.First().NPCs.ToList().ForEach(npc =>
			{
				npc.Coord.x += room_loc.x;
				npc.Coord.y += room_loc.y;
				reqs.Rom.MoveNpc(reqs.MapId, npc);
			});

			complete.Entrance = new Coordinate((byte)start_loc.x, (byte)start_loc.y, CoordinateLocale.Standard);

			return complete;
		}

		private List<Directions> fullDirs()
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

		private int[,] FloodFillDist(Map map, (int x, int y) start, byte live)
		{
			int[,] dist = new int[64, 64];
			dist[start.x, start.y] = 1;
			Queue<(int x, int y)> exploreQueue = new Queue<(int x, int y)>();
			exploreQueue.Enqueue(start);
			int[,] changes = { { 1, 0 }, { -1, 0 }, { 0, 1 }, { 0, -1 } };
			while (exploreQueue.Count != 0)
			{
				var expand = exploreQueue.Dequeue();
				for (var i = 0; i < 4; i++)
				{
					var cur_x = expand.x + changes[i, 0];
					var cur_y = expand.y + changes[i, 1];
					if (dist[cur_x, cur_y] == 0 && map[cur_y, cur_x] == live)
					{
						exploreQueue.Enqueue((cur_x, y: cur_y));
						dist[cur_x, cur_y] = dist[expand.x, expand.y] + 1;
					}
				}
			}
			return dist;
		}

		private bool FloodFillReachable(Map map, (int x, int y) start, byte live, byte target)
		{
			int[,] visited = new int[64, 64];
			visited[start.x, start.y] = 1;
			Queue<(int x, int y)> exploreQueue = new Queue<(int x, int y)>();
			exploreQueue.Enqueue(start);
			int[,] changes = { { 1, 0 }, { -1, 0 }, { 0, 1 }, { 0, -1 } };
			while (exploreQueue.Count != 0)
			{
				var expand = exploreQueue.Dequeue();
				for (var i = 0; i < 4; i++)
				{
					var cur_x = expand.x + changes[i, 0];
					var cur_y = expand.y + changes[i, 1];
					if (visited[cur_x, cur_y] == 0 && map[cur_y, cur_x] == live)
					{
						exploreQueue.Enqueue((cur_x, y: cur_y));
						visited[cur_x, cur_y] = 1;
					}
					if (map[cur_y, cur_x] == target)
					{
						return true;
					}
				}
			}
			return false;
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
