using System;
using System.Collections.Generic;
using System.Text;
using RomUtilities;

namespace FF1Lib.Procgen
{
	public class Maze
	{
		public struct Cell
		{
			public int X { get; }
			public int Y { get; }

			public Cell(int x, int y)
			{
				X = x;
				Y = y;
			}
		}

		public struct Wall
		{
			public EquivalenceNode<Cell> One { get; }
			public EquivalenceNode<Cell> Two { get; }

			public Wall(EquivalenceNode<Cell> one, EquivalenceNode<Cell> two)
			{
				One = one;
				Two = two;
			}
		}

		public static List<Wall> DoSkyCastle4FMaze(MT19337 rng)
		{
			EquivalenceNode<Cell>[,] cells = new EquivalenceNode<Cell>[8, 8];
			for (int i = 0; i < 8; i++)
			{
				for (int j = 0; j < 8; j++)
				{
					if (i % 2 == 0 || j % 2 == 0)
					{
						cells[j, i] = new EquivalenceNode<Cell>(new Cell(i, j));
					}
				}
			}

			List<Wall> walls = new List<Wall>();
			for (int i = 0; i < 8; i += 2)
			{
				for (int j = 0; j < 8; j++)
				{
					if (j == 7)
					{
						walls.Add(new Wall(cells[j, i], cells[0, i]));
					}
					else
					{
						walls.Add(new Wall(cells[j, i], cells[j + 1, i]));
					}
				}
			}

			for (int j = 0; j < 8; j += 2)
			{
				for (int i = 0; i < 8; i++)
				{
					if (i == 7)
					{
						walls.Add(new Wall(cells[j, i], cells[j, 0]));
					}
					else
					{
						walls.Add(new Wall(cells[j, i], cells[j, i + 1]));
					}
				}
			}

			walls.Shuffle(rng);

			for (int i = 0; i < walls.Count;)
			{
				if (!walls[i].One.IsEquivalentTo(walls[i].Two))
				{
					walls[i].One.MakeEquivalentTo(walls[i].Two);
					walls.RemoveAt(i);
				}
				else
				{
					i++;
				}
			}

			return walls;
		}
	}
}
