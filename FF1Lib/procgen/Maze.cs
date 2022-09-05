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
			public int x { get; }
			public int y { get; }

			public Cell(int x, int y)
			{
				this.x = x;
				this.y = y;
			}
		}

		public struct Wall
		{
			public EquivalenceNode<Cell> one { get; }
			public EquivalenceNode<Cell> two { get; }

			public Wall(EquivalenceNode<Cell> one, EquivalenceNode<Cell> two)
			{
				this.one = one;
				this.two = two;
			}
		}

		public static List<Wall> DoSkyCastle4FMaze(MT19337 rng)
		{
			var cells = new EquivalenceNode<Cell>[8, 8];
			for (int i = 0; i < 8; i++)
				for (int j = 0; j < 8; j++)
					if (i % 2 == 0 || j % 2 == 0)
						cells[j, i] = new EquivalenceNode<Cell>(new Cell(i, j));

			var walls = new List<Wall>();
			for (int i = 0; i < 8; i += 2)
				for (int j = 0; j < 8; j++)
					if (j == 7)
						walls.Add(new Wall(cells[j, i], cells[0, i]));
					else
						walls.Add(new Wall(cells[j, i], cells[j + 1, i]));
			for (int j = 0; j < 8; j += 2)
				for (int i = 0; i < 8; i++)
					if (i == 7)
						walls.Add(new Wall(cells[j, i], cells[j, 0]));
					else
						walls.Add(new Wall(cells[j, i], cells[j, i + 1]));

			walls.Shuffle(rng);

			for (int i = 0; i < walls.Count;)
			{
				if (!walls[i].one.IsEquivalentTo(walls[i].two))
				{
					walls[i].one.MakeEquivalentTo(walls[i].two);
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
