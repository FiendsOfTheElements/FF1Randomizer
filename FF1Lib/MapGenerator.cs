using RomUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FF1Lib
{
	public class CompleteMap
	{
		public Map Map;
		public Coordinate Entrance;
		public MapRequirements Requirements;
	}

	public struct MapRequirements
	{
		public const int Width = Map.RowLength;
		public const int Height = Map.RowCount;

		public MapId MapId;
		public Tile Floor;

		public IEnumerable<byte> Treasures;
		public IEnumerable<int> NPCs;

		public FF1Rom Rom;
	}

	public enum MapGeneratorStrategy
	{
		Cellular
	}

	public class MapGenerator
	{
		public CompleteMap Generate(MT19337 rng, MapGeneratorStrategy strategy, MapRequirements reqs)
		{
			return GetStrategy(strategy).Generate(rng, reqs);
		}

		private IMapGeneratorEngine GetStrategy(MapGeneratorStrategy strategy)
		{
			switch (strategy)
			{
				case MapGeneratorStrategy.Cellular:
					return new CellularGenerator();
				default:
					return null;
			}
		}
	}

	interface IMapGeneratorEngine
	{
		CompleteMap Generate(MT19337 rng, MapRequirements reqs);
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

				var room = GenerateTreasureRoom(rng, reqs.Treasures.ToList());

				complete.Map.Put(0, 0, room.ToArray());
				InitializeMap(rng, complete.Map);

				for (int i = 0; i < Steps; ++i)
				{
					complete.Map = DoSimulationStep(complete.Map);
				}

				byte x = (byte)rng.Between(0, MapRequirements.Width - 1);
				byte y = (byte)rng.Between(0, MapRequirements.Height - 1);

				complete.Map[y, x] = (byte)Tile.WarpUp;
				var results = FloodFill(complete.Map, new List<(int, int)> { (x, y) }, new List<Tile> { Tile.Doorway, Tile.WarpUp, reqs.Floor }, new List<Tile> { Tile.Lava }, reqs.Floor);

				Console.WriteLine($"FloorGen Results: Floor:{results[reqs.Floor]}, Doorways:{results[Tile.Doorway]}, Warps:{results[Tile.WarpUp]}");

				if (results[reqs.Floor] > 1000 && results[Tile.Doorway] == 1)
				{
					reqs.NPCs.ToList().ForEach(npc =>
					{
						while (true)
						{
							int x0 = rng.Between(0, MapRequirements.Width - 1);
							int y0 = rng.Between(0, MapRequirements.Height - 1);
							if (complete.Map[y0, x0] == (byte)reqs.Floor)
							{
								reqs.Rom.MoveNpc(reqs.MapId, npc, x0, y0, false, false);
								break;
							}
						}
					});

					complete.Entrance = new Coordinate(x, y, CoordinateLocale.Standard);
					return complete;
				}
			}
		}

		private List<Blob> GenerateTreasureRoom(MT19337 rng, List<byte> treasures)
		{
			int width = treasures.Count() + 2;
			int height = 5;

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
			for (int i = 0; i < treasures.Count(); ++i)
			{
				room.Last()[i + 1] = treasures[i];
			}

			for (int i = 1; i < height - 2; ++i)
			{
				room.Add(middle);
			}
			room.Add(bottom);
			room.Add(wall);
			room.Add(outside);
			return room;
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

		private Dictionary<Tile, int> FloodFill(Map map, List<(int, int)> coords, IEnumerable<Tile> counts, IEnumerable<Tile> finds, Tile replace)
		{
			Dictionary<Tile, int> results = counts.ToDictionary(tile => tile, tile => 0);
			FloodFill(results, map, coords, counts, finds, replace);
			return results;
		}

		private void FloodFill(Dictionary<Tile, int> results, Map map, List<(int, int)> coords, IEnumerable<Tile> counts, IEnumerable<Tile> finds, Tile replace)
		{
			const int Width = Map.RowLength;
			const int Height = Map.RowCount;

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
