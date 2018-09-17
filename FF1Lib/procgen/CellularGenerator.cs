using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RomUtilities;

namespace FF1Lib.Procgen
{
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
			map.Flood(inCoord, element =>
			{
				// Update if desired
				if (finds.Contains(element.Tile))
				{
					element.Tile = replace;
				}

				// Accumulate results and recurse if this is a counter tile.
				if (counts.Contains(element.Tile))
				{
					results[element.Tile]++;
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
