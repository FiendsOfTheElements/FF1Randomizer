using RomUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FF1Lib
{
	class MapGenerator
	{
		public double InitialAliveChance { get; set; }
		public int BirthLimit = 4;
		public int DeathLimit = 3;
		public int Steps = 5;

		public int Width { get; set; }
		public int Height { get; set; }

		public Tile Floor { get; private set; }

		public Map Map { get; protected set; }
		public Coordinate Entrance { get; protected set; }

		public MapGenerator()
		{
			InitialAliveChance = 0.4;
			Width = 64;
			Height = 64;
			Floor = Tile.FloorSafe;
		}

		public void Generate(MT19337 rng, Tile floor)
		{
			Floor = floor;
			Map map = null;
			int sanity = 0;
			while(true)
			{
				if (++sanity == 500)
					throw new InsaneException("Failed to generate map!");

				map = new Map((byte)Tile.Lava);
				var room = GenerateRoom(rng, 8, 3);

				map.Put(10, 10, room.ToArray());
				InitializeMap(rng, map);

				for (int i = 0; i < Steps; ++i)
				{
					map = DoSimulationStep(map);
				}

				byte x = (byte)rng.Between(0, Width - 1);
				byte y = (byte)rng.Between(0, Height - 1);

				map[y, x] = (byte)Tile.WarpUp;
				var results = FloodFill(map, new List<(int, int)> { (x, y) }, new List<Tile> { Tile.Doorway, Tile.WarpUp, Floor }, new List<Tile> { Tile.Lava }, Floor);

				Console.WriteLine($"FloorGen Results: Floor:{results[Floor]}, Doorways:{results[Tile.Doorway]}, Warps:{results[Tile.WarpUp]}");

				if (results[Floor] > 1000 || results[Tile.Doorway] > 1)
				{
					Entrance = new Coordinate(x, y, CoordinateLocale.Standard);
					break;
				}

			} while (false);

			Map = map;
		}

		private List<Blob> GenerateRoom(MT19337 rng, int interiorWidth, int interiorHeight)
		{

			int width = interiorWidth + 2;
			int height = interiorHeight + 2;

			Blob top = Enumerable.Repeat(Tile.RoomBackCenter, width).Select(x => (byte)x).ToArray();
			top[0] = (byte)Tile.RoomBackLeft;
			top[width - 1] = (byte)Tile.RoomBackRight;

			Blob treasureArea = Enumerable.Repeat(0x04, width).Select(x => (byte)x).ToArray();
			treasureArea[0] = 0x03;
			treasureArea[width - 1] = 0x05;

			Blob middle = Enumerable.Repeat(0x04, width).Select(x => (byte)x).ToArray();
			middle[0] = 0x03;
			middle[width - 1] = 0x05;

			Blob bottom = Enumerable.Repeat(0x07, width).Select(x => (byte)x).ToArray();
			bottom[0] = 0x06;
			bottom[width - 1] = 0x08;

			int doorX = rng.Between(1, width - 2);
			Blob wall = Enumerable.Repeat(0x30, width).Select(x => (byte)x).ToArray();
			wall[doorX] = 0x36;
			Blob outside = Enumerable.Repeat(Tile.Lava, width).Select(x => (byte)x).ToArray();
			outside[doorX] = 0x3A;

			List<Blob> room = new List<Blob> { top, treasureArea };

			// Chests
			for (int i = 1; i < 7; ++i)
			{
				room.Last()[i] = (byte)(0x78 + i);
			}

			for (int i = 1; i < interiorHeight; ++i)
			{
				room.Add(middle);
			}
			room.Add(bottom);
			room.Add(wall);
			room.Add(outside);
			return room;
		}

		public (int, int) GetNPCCoordinate(MT19337 rng)
		{
			while (true)
			{
				int x = rng.Between(0, Width - 1);
				int y = rng.Between(0, Height - 1);
				if (Map[y, x] == (byte)Floor)
				{
					return (x, y);
				}
			}
		}

		private Dictionary<Tile, int> FloodFill(Map map, List<( int, int)> coords, IEnumerable<Tile> counts, IEnumerable<Tile> finds, Tile replace)
		{
			Dictionary<Tile, int> results = counts.ToDictionary(tile => tile, tile => 0);
			FloodFill(results, map, coords, counts, finds, replace);
			return results;
		}

		private void FloodFill(Dictionary<Tile, int> results, Map map, List<(int, int)> coords, IEnumerable<Tile> counts, IEnumerable<Tile> finds, Tile replace)
		{
			for (int i = 0; i < coords.Count(); ++i)
			{
				int x = coords[i].Item1;
				int y = coords[i].Item2;

				Tile tile = (Tile)map[y, x];

				// Update if desired
				if (finds.Contains(tile))
				{
					tile = replace;
					map[y, x] = (byte)tile;
				}

				// Accumulate results and recurse if this is a counter tile.
				if (counts.Contains(tile))
				{
					results[tile]++;

					if (!coords.Contains(((Width + x - 1) % Width, y))) { coords.Add(((Width + x - 1) % Width, y)); }
					if (!coords.Contains(((Width + x + 1) % Width, y))) { coords.Add(((Width + x + 1) % Width, y)); }
					if (!coords.Contains((x, (Height + y - 1) % Height))) { coords.Add((x, (Height + y - 1) % Height)); }
					if (!coords.Contains((x, (Height + y + 1) % Height))) { coords.Add((x, (Height + y + 1) % Height)); }
				}
			}
		}

		private void InitializeMap(MT19337 rng, Map map)
		{
			for (int x = 0; x < Width; x++)
			{
				for (int y = 0; y < Height; y++)
				{
					if (map[y, x] == (byte)Tile.Lava && ((double)rng.Next() / uint.MaxValue < InitialAliveChance))
					{
						map[y, x] = (byte)Tile.Impassable;
					}
				}
			}
		}

		private Map DoSimulationStep(Map old)
		{
			Map map = old.Clone();
			// Loop over each row and column of the map

			for (int x = 0; x < Width; x++)
			{
				for (int y = 0; y < Height; y++)
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

					if (map[(y + j + Height) % Height, (x + i + Width) % Width] == (byte)Tile.Impassable)
					{
						count = count + 1;
					}
				}
			}
			return count;
		}
	}
}
