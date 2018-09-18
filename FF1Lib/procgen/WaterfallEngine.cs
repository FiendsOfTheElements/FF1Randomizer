using RomUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FF1Lib.Procgen
{

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
			while (MapHelper.SmoothFilter(complete.Map, Tile.WaterfallInside, Tile.WaterfallRandomEncounters))
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
			map.Flood(start, element =>
			{
				if (live.Contains(element.Tile))
				{
					int minDistance = Map.RowCount * Map.RowLength + 1;
					int upDistance = dist[element.X, (Map.RowCount + element.Y - 1) % Map.RowCount];
					int downDistance = dist[element.X, (Map.RowCount + element.Y + 1) % Map.RowCount];
					int rightDistance = dist[(Map.RowLength + element.X + 1) % Map.RowLength, element.Y];
					int leftDistance = dist[(Map.RowLength + element.X - 1) % Map.RowLength, element.Y];
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
						dist[element.X, element.Y] = minDistance;
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
			map.Flood(start, element =>
			{
				if (targets.Contains(element.Tile))
				{
					results[element.Tile] = true;
					return true;
				}
				if (live.Contains(element.Tile))
				{
					return true;
				}

				return false;
			});
			return results;
		}
	}
}
