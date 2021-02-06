using System;
using System.Collections.Generic;
using RomUtilities;

namespace FF1Lib
{
	public partial class FF1Rom
	{
		private const byte northdir = 0;
		private const byte southdir = 1;
		private const byte westdir = 2;
		private const byte eastdir = 3;
		private const byte CityWall = 0x7C;
		private const byte abysstile = 0x3C;
		//private const byte floortile = 0x60;
		private const byte floortile = 0x31; // For testing purposes I don't want to be fighting monsters
		private const byte warptile = 0x48;
		private const byte walltile = 0x30;
		private const byte leftwalltile = 0x32;
		private const byte rightwalltile = 0x33;
		private const byte wallupperleft = 0x34;
		private const byte wallupperright = 0x35;
		private byte[] illegals =
		{
			warptile, floortile
		};

		private struct Candidate
		{
			public int x;
			public int y;
			public List<byte> dirs;

			public Candidate(int newx, int newy)
			{
				x = newx;
				y = newy;
				dirs = new List<byte>();
			}
		}
		public void DeepDungeon(MT19337 rng, OverworldMap overworldMap, List<Map> maps)
		{
			// Close the city wall around Coneria to prevent exploring the world normally.
			overworldMap.MapEditsToApply.Add(new List<MapEdit>
			{
				new MapEdit { X = 0x97, Y = 0xA3, Tile = CityWall },
				new MapEdit { X = 0x98, Y = 0xA3, Tile = CityWall },
				new MapEdit { X = 0x99, Y = 0xA3, Tile = CityWall },
				new MapEdit { X = 0x9A, Y = 0xA3, Tile = CityWall }
			});

			// Move the player's starting location up so that they're within the city wall.
			Put(0x3011, Blob.FromHex("9B"));

			// Change the destination for Coneria Castle overworld teleporter so it puts us on the
			// "back" stairs for floor 1 of the Deep Dungeon.
			overworldMap.PutOverworldTeleport(OverworldTeleportIndex.ConeriaCastle1, new TeleportDestination(MapLocation.ConeriaCastle1, MapIndex.ConeriaCastle1F, new Coordinate(0x20, 0x20, CoordinateLocale.Standard)));

			//tilesetindexes[0] = 0;

			WipeMap(maps[8]);
			GenerateMapBoxStyle(rng, maps[8]);

			// Commit the overworld edits.
			overworldMap.ApplyMapEdits();
		}

		private int RollDice(MT19337 rng, int dice, int sides)
		{
			int result = 0;
			for (int i = 0; i < dice; i++)
			{
				result += rng.Between(1, sides);
			}
			return result;
		}
		private T DrawCard<T>(MT19337 rng, List<T> list)
		{
			int roll = rng.Between(0, list.Count - 1);
			T result = list[roll];
			list.RemoveAt(roll);
			return result;
		}
		private void WipeMap(Map m)
		{
			m.Fill((0, 0), (64, 64), abysstile);
		}

		private bool Legal(Map m, int x, int y, int w, int h)
		{
			bool result = true;
			for (int i = x; i < x + w; i++)
			{
				for (int j = y; j < y + h; j++)
				{
					if (Array.Exists(illegals, element => element == m[j, i]))
					{
						result = false;
						break;
					}
				}
				if (!result) break;
			}
			return result;
		}
		private bool CarveBox(Map m, int x, int y, int w, int h)
		{
			bool result = Legal(m, x, y, w, h);
			if (result)
			{
				m.Fill((x, y), (w, h), floortile);
				for (int i = 0; i < w; i++)
				{
					m[y, x + i] = walltile;
					m[y + h - 1, x + i] = walltile;
				}
				for (int i = 0; i < h - 1; i++)
				{
					m[y + i, x] = leftwalltile;
					m[y + i, x + w - 1] = rightwalltile;
				}
				m[y, x] = wallupperleft;
				m[y, x + w - 1] = wallupperright;
			}
			return result;
		}

		private void GenerateMapBoxStyle(MT19337 rng, Map m)
		{
			// Draw the initial box containing the "back" staircase.
			int w = RollDice(rng, 3, 4);
			int h = RollDice(rng, 3, 4);
			CarveBox(m, 32 - w / 2, 32 - h / 2, w, h);
			m[32, 32] = warptile;

			int attempts = 40;
			for (int attempt = 0; attempt < attempts; attempt++)
			{
				// Determine which tiles are suitable for attaching a new box.
				List<Candidate> candidates = new List<Candidate>();
				Candidate c;
				for (int i = 1; i < 63; i++)
				{
					for (int j = 1; j < 63; j++)
					{
						if (m[j, i] == floortile)
						{
							if (m[j - 1, i] != floortile || m[j + 1, i] != floortile || m[j, i - 1] != floortile || m[j, i + 1] != floortile)
							{
								c = new Candidate(i, j);
								if (m[j - 1, i] != floortile) c.dirs.Add(northdir);
								if (m[j + 1, i] != floortile) c.dirs.Add(southdir);
								if (m[j, i - 1] != floortile) c.dirs.Add(westdir);
								if (m[j, i + 1] != floortile) c.dirs.Add(eastdir);
								candidates.Add(c);
							}
						}
					}
				}

				// Select a candidate at random and attach a box to it in a random legal direction
				c = DrawCard<Candidate>(rng, candidates);
				byte d = DrawCard<byte>(rng, c.dirs);
				w = RollDice(rng, 3, 4);
				h = RollDice(rng, 3, 4);
				switch (d)
				{
					case northdir:
						if (CarveBox(m, c.x - w / 2, c.y - h, w, h)) m[c.y - 1, c.x] = floortile;
						break;
					case southdir:
						if (CarveBox(m, c.x - w / 2, c.y + 1, w, h)) m[c.y + 1, c.x] = floortile;
						break;
					case westdir:
						if (CarveBox(m, c.x - w, c.y - h / 2, w, h)) m[c.y, c.x - 1] = floortile;
						break;
					case eastdir:
						if (CarveBox(m, c.x + 1, c.y - h / 2, w, h)) m[c.y, c.x + 1] = floortile;
						break;
				}

			}
		}
	}
}
