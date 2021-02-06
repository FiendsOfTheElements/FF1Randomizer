using System;
using System.Collections.Generic;
using System.Linq;
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
		private List<byte> domainlist = new List<byte>();
		private byte[] tilesetmappings = new byte[61];

		private struct Tileset
		{
			public byte abysstile;
			public byte floortile;
			public byte warptile;
			public byte walltile;
			public byte leftwalltile;
			public byte rightwalltile;
			public byte wallupperleft;
			public byte wallupperright;
			public byte roomtile;
			public byte roomupperleft;
			public byte roomupper;
			public byte roomupperright;
			public byte roomlowerleft;
			public byte roomlower;
			public byte roomlowerright;
			public byte roomleft;
			public byte roomright;
			public byte doortile;
			public byte closedoortile;
			public List<byte> teleportdeck;

			public byte[] Solids()
			{
				byte[] result = { walltile, leftwalltile, rightwalltile, wallupperleft, wallupperright, abysstile };
				return result;
			}
		}

		private Tileset[] tilesets = new Tileset[8];
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

		public void InitializeTilesets()
		{
			// Castle
			tilesets[1].abysstile = 0x3C;
			tilesets[1].floortile = 0x60;
			tilesets[1].warptile = 0x48;
			tilesets[1].walltile = 0x30;
			tilesets[1].leftwalltile = 0x32;
			tilesets[1].rightwalltile = 0x33;
			tilesets[1].wallupperleft = 0x34;
			tilesets[1].wallupperright = 0x35;
			tilesets[1].roomtile = 0x5E;
			tilesets[1].roomupperleft = 0x00;
			tilesets[1].roomupper = 0x01;
			tilesets[1].roomupperright = 0x02;
			tilesets[1].roomlowerleft = 0x06;
			tilesets[1].roomlower = 0x07;
			tilesets[1].roomlowerright = 0x08;
			tilesets[1].roomleft = 0x03;
			tilesets[1].roomright = 0x05;
			tilesets[1].doortile = 0x36;
			tilesets[1].closedoortile = 0x3A;
			tilesets[1].teleportdeck = new List<byte>();
			tilesets[1].teleportdeck.Add(0x44);
			tilesets[1].teleportdeck.Add(0x45);
			tilesets[1].teleportdeck.Add(0x49);
			tilesets[1].teleportdeck.Add(0x4C);
			tilesets[1].teleportdeck.Add(0x4D);
			tilesets[1].teleportdeck.Add(0x4E);
			tilesets[1].teleportdeck.Add(0x4F);
			tilesets[1].teleportdeck.Add(0x50);
			tilesets[1].teleportdeck.Add(0x51);
			tilesets[1].teleportdeck.Add(0x52);
			tilesets[1].teleportdeck.Add(0x53);
			tilesets[1].teleportdeck.Add(0x54);
			tilesets[1].teleportdeck.Add(0x55);

			// Earth/Volcano
			tilesets[2].abysstile = 0x38;
			tilesets[2].floortile = 0x41;
			tilesets[2].warptile = 0x18;
			tilesets[2].walltile = 0x30;
			tilesets[2].leftwalltile = 0x32;
			tilesets[2].rightwalltile = 0x33;
			tilesets[2].wallupperleft = 0x34;
			tilesets[2].wallupperright = 0x35;
			tilesets[2].roomtile = 0x2E;
			tilesets[2].roomupperleft = 0x00;
			tilesets[2].roomupper = 0x01;
			tilesets[2].roomupperright = 0x02;
			tilesets[2].roomlowerleft = 0x06;
			tilesets[2].roomlower = 0x07;
			tilesets[2].roomlowerright = 0x08;
			tilesets[2].roomleft = 0x03;
			tilesets[2].roomright = 0x05;
			tilesets[2].doortile = 0x36;
			tilesets[2].closedoortile = 0x3A;
			tilesets[2].teleportdeck = new List<byte>();
			tilesets[2].teleportdeck.Add(0x15);
			tilesets[2].teleportdeck.Add(0x16);
			tilesets[2].teleportdeck.Add(0x19);
			tilesets[2].teleportdeck.Add(0x24);
			tilesets[2].teleportdeck.Add(0x25);
			tilesets[2].teleportdeck.Add(0x26);
			tilesets[2].teleportdeck.Add(0x27);
			tilesets[2].teleportdeck.Add(0x28);
			tilesets[2].teleportdeck.Add(0x29);
			tilesets[2].teleportdeck.Add(0x2A);
			tilesets[2].teleportdeck.Add(0x2B);
			tilesets[2].teleportdeck.Add(0x2C);

			// Ice Cave
			tilesets[3].abysstile = 0x3C;
			tilesets[3].floortile = 0x49;
			tilesets[3].warptile = 0x18;
			tilesets[3].walltile = 0x30;
			tilesets[3].leftwalltile = 0x32;
			tilesets[3].rightwalltile = 0x33;
			tilesets[3].wallupperleft = 0x34;
			tilesets[3].wallupperright = 0x35;
			tilesets[3].roomtile = 0x46;
			tilesets[3].roomupperleft = 0x00;
			tilesets[3].roomupper = 0x01;
			tilesets[3].roomupperright = 0x02;
			tilesets[3].roomlowerleft = 0x06;
			tilesets[3].roomlower = 0x07;
			tilesets[3].roomlowerright = 0x08;
			tilesets[3].roomleft = 0x03;
			tilesets[3].roomright = 0x05;
			tilesets[3].doortile = 0x36;
			tilesets[3].closedoortile = 0x3A;
			tilesets[3].teleportdeck = new List<byte>();
			tilesets[3].teleportdeck.Add(0x19);
			tilesets[3].teleportdeck.Add(0x1A);
			tilesets[3].teleportdeck.Add(0x1B);
			tilesets[3].teleportdeck.Add(0x29);
			tilesets[3].teleportdeck.Add(0x2A);
			tilesets[3].teleportdeck.Add(0x2B);
			tilesets[3].teleportdeck.Add(0x2C);

			// Marsh/Tower
			tilesets[4].abysstile = 0x3F;
			tilesets[4].floortile = 0x40;
			tilesets[4].warptile = 0x23;
			tilesets[4].walltile = 0x30;
			tilesets[4].leftwalltile = 0x32;
			tilesets[4].rightwalltile = 0x33;
			tilesets[4].wallupperleft = 0x34;
			tilesets[4].wallupperright = 0x35;
			tilesets[4].roomtile = 0x2D;
			tilesets[4].roomupperleft = 0x00;
			tilesets[4].roomupper = 0x01;
			tilesets[4].roomupperright = 0x02;
			tilesets[4].roomlowerleft = 0x06;
			tilesets[4].roomlower = 0x07;
			tilesets[4].roomlowerright = 0x08;
			tilesets[4].roomleft = 0x03;
			tilesets[4].roomright = 0x05;
			tilesets[4].doortile = 0x36;
			tilesets[4].closedoortile = 0x3A;
			tilesets[4].teleportdeck = new List<byte>();
			tilesets[4].teleportdeck.Add(0x20);
			tilesets[4].teleportdeck.Add(0x22);
			tilesets[4].teleportdeck.Add(0x27);
			tilesets[4].teleportdeck.Add(0x28);

			// Shrine/Temple
			tilesets[5].abysstile = 0x39;
			tilesets[5].floortile = 0x55;
			tilesets[5].warptile = 0x42;
			tilesets[5].walltile = 0x30;
			tilesets[5].leftwalltile = 0x32;
			tilesets[5].rightwalltile = 0x33;
			tilesets[5].wallupperleft = 0x34;
			tilesets[5].wallupperright = 0x35;
			tilesets[5].roomtile = 0x53;
			tilesets[5].roomupperleft = 0x00;
			tilesets[5].roomupper = 0x01;
			tilesets[5].roomupperright = 0x02;
			tilesets[5].roomlowerleft = 0x06;
			tilesets[5].roomlower = 0x07;
			tilesets[5].roomlowerright = 0x08;
			tilesets[5].roomleft = 0x03;
			tilesets[5].roomright = 0x05;
			tilesets[5].doortile = 0x36;
			tilesets[5].closedoortile = 0x3A;
			tilesets[5].teleportdeck = new List<byte>();
			tilesets[5].teleportdeck.Add(0x43);
			tilesets[5].teleportdeck.Add(0x44);
			tilesets[5].teleportdeck.Add(0x45);
			tilesets[5].teleportdeck.Add(0x46);
			tilesets[5].teleportdeck.Add(0x4B);
			tilesets[5].teleportdeck.Add(0x4C);
			tilesets[5].teleportdeck.Add(0x4E);
			tilesets[5].teleportdeck.Add(0x4F);

			// Sky
			tilesets[6].abysstile = 0x39;
			tilesets[6].floortile = 0x4B;
			tilesets[6].warptile = 0x45;
			tilesets[6].walltile = 0x30;
			tilesets[6].leftwalltile = 0x32;
			tilesets[6].rightwalltile = 0x33;
			tilesets[6].wallupperleft = 0x34;
			tilesets[6].wallupperright = 0x35;
			tilesets[6].roomtile = 0x49;
			tilesets[6].roomupperleft = 0x00;
			tilesets[6].roomupper = 0x01;
			tilesets[6].roomupperright = 0x02;
			tilesets[6].roomlowerleft = 0x06;
			tilesets[6].roomlower = 0x07;
			tilesets[6].roomlowerright = 0x08;
			tilesets[6].roomleft = 0x03;
			tilesets[6].roomright = 0x05;
			tilesets[6].doortile = 0x36;
			tilesets[6].closedoortile = 0x3A;
			tilesets[6].teleportdeck = new List<byte>();
			tilesets[6].teleportdeck.Add(0x41);
			tilesets[6].teleportdeck.Add(0x42);
			tilesets[6].teleportdeck.Add(0x43);
			tilesets[6].teleportdeck.Add(0x44);

			// Final dungeon
			tilesets[7].abysstile = 0x3F;
			tilesets[7].floortile = 0x5C;
			tilesets[7].warptile = 0x4F;
			tilesets[7].walltile = 0x30;
			tilesets[7].leftwalltile = 0x32;
			tilesets[7].rightwalltile = 0x33;
			tilesets[7].wallupperleft = 0x34;
			tilesets[7].wallupperright = 0x35;
			tilesets[7].roomtile = 0x55;
			tilesets[7].roomupperleft = 0x00;
			tilesets[7].roomupper = 0x01;
			tilesets[7].roomupperright = 0x02;
			tilesets[7].roomlowerleft = 0x06;
			tilesets[7].roomlower = 0x07;
			tilesets[7].roomlowerright = 0x08;
			tilesets[7].roomleft = 0x03;
			tilesets[7].roomright = 0x05;
			tilesets[7].doortile = 0x36;
			tilesets[7].closedoortile = 0x3A;
			tilesets[7].teleportdeck = new List<byte>();
			tilesets[7].teleportdeck.Add(0x42);
			tilesets[7].teleportdeck.Add(0x43);
			tilesets[7].teleportdeck.Add(0x4B);
			tilesets[7].teleportdeck.Add(0x4C);
			tilesets[7].teleportdeck.Add(0x4D);
			tilesets[7].teleportdeck.Add(0x4E);
			tilesets[7].teleportdeck.Add(0x50);
			tilesets[7].teleportdeck.Add(0x51);
			tilesets[7].teleportdeck.Add(0x52);
			tilesets[7].teleportdeck.Add(0x53);

			// Read which maps have which tilesets
			tilesetmappings = Get(0x2CC0, 61).ToBytes();
		}
		public void CreateDomains(MT19337 rng, List<Map> maps) 
		{

		}
		public void DeepDungeon(MT19337 rng, OverworldMap overworldMap, List<Map> maps)
		{
			InitializeTilesets();

			// Evaluate the monster formations and create domains accordingly.
			CreateDomains(rng, maps);

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

			// Kill all the NPCs
			KillNPCs();

			// Generate the map layouts
			for (int i = 8; i < 61; i++)
			{
				Put(0x2CC00 + i, Blob.FromHex("08")); // Set the encounter rate
				WipeMap(maps[i], tilesets[tilesetmappings[i]]); // Start from a clean slate
				GenerateMapBoxStyle(rng, maps[i], tilesets[tilesetmappings[i]]); // Which algorithm to use; right now it's the only one
				Beautify(maps[i], tilesets[tilesetmappings[i]]); // Make the tiles look right
				// Connect it to the next map
				if (i < 60)
				{
					Put(0x800 + tilesetmappings[i] * 0x100 + 2 * PlaceExit(rng, maps[i], tilesets[tilesetmappings[i]]), Blob.FromHex("80" + Convert.ToHexString(new byte[]{(byte)(i - 8)})));
					Put(0x2D00 + i - 8, Blob.FromHex("20"));
					Put(0x2D40 + i - 8, Blob.FromHex("20"));
					Put(0x2D80 + i - 8, Blob.FromHex(Convert.ToHexString(new byte[] { (byte)(i + 1) })));
				}
			}

			// Commit the overworld edits.
			overworldMap.ApplyMapEdits();
		}

		public int RollDice(MT19337 rng, int dice, int sides)
		{
			// I can't believe this isn't already a function...
			int result = 0;
			for (int i = 0; i < dice; i++)
			{
				result += rng.Between(1, sides);
			}
			return result;
		}
		private T DrawCard<T>(MT19337 rng, List<T> list)
		{
			// Pick a random element from the list, remove it, and return it.
			int roll = rng.Between(0, list.Count - 1);
			T result = list[roll];
			list.RemoveAt(roll);
			return result;
		}
		private void WipeMap(Map m, Tileset t)
		{
			// Clear the whole map.
			m.Fill((0, 0), (64, 64), t.abysstile);
		}

		private void KillNPCs()
		{
			// We don't want NPCs in weird places, and in fact we don't need them at all.
			for (int i = 0; i < 61; i++)
			{
				for (int j = 0; j < 16; j++)
				{
					SetNpc((MapId)i, j, 0, 0, 0, false, true);
				}
			}
		}

		private void Beautify(Map m, Tileset t)
		{
			// The map generator algorithms are a bit rough around the edges so this
			// is here to make sure the corner tiles look correct and the rooms are
			// properly bordered and such.
			bool upper = false;
			bool lower = false;
			bool left = false;
			bool right = false;
			for (int i = 0; i < 64; i++)
			{
				for (int j = 0; j < 64; j++)
				{
					upper = false;
					lower = false;
					left = false;
					right = false;
					if (m[j, i] == t.walltile)
					{
						if (j < 63)
						{
							if (m[j + 1, i] == t.leftwalltile)
							{
								m[j, i] = t.wallupperleft;
							}
							else if (m[j + 1, i] == t.rightwalltile)
							{
								m[j, i] = t.wallupperright;
							}
						}
					}
					else if (m[j, i] == t.wallupperleft || m[j, i] == t.wallupperright)
					{
						if (j < 63)
						{
							if (m[j + 1, i] == t.floortile)
							{
								m[j, i] = t.walltile;
							}
						}
					}
					else if (m[j, i] == t.roomtile)
					{
						if (j > 0)
						{
							if (m[j - 1, i] != t.roomtile && m[j - 1, i] != t.roomleft && m[j - 1, i] != t.roomright && m[j - 1, i] != t.roomupperleft && m[j - 1, i] != t.roomupper && m[j - 1, i] != t.roomupperright)
							{
								upper = true;
							}
						}
						if (j < 63)
						{
							if (m[j + 1, i] != t.roomtile) lower = true;
						}
						if (i > 0)
						{
							if (m[j, i - 1] != t.roomtile && m[j, i - 1] != t.roomupper && m[j, i - 1] != t.roomlower && m[j, i - 1] != t.roomleft && m[j, i - 1] != t.roomupperleft && m[j, i - 1] != t.roomlowerleft)
							{
								left = true;
							}
						}
						if (i < 63)
						{
							if (m[j, i + 1] != t.roomtile) right = true;
						}
						if (upper)
						{
							m[j, i] = t.roomupper;
							if (left) m[j, i] = t.roomupperleft;
							if (right) m[j, i] = t.roomupperright;
						}
						else if (lower)
						{
							m[j, i] = t.roomlower;
							if (left) m[j, i] = t.roomlowerleft;
							if (right) m[j, i] = t.roomlowerright;
							if (j < 63)
							{
								if (m[j + 1, i] != t.roomtile && m[j + 1, i] != t.doortile)
								{
									m[j + 1, i] = t.walltile;
								}
							}
						}
						else if (left)
						{
							m[j, i] = t.roomleft;
						}
						else if (right)
						{
							m[j, i] = t.roomright;
						}
					}
				}
			}
		}

		private bool Traversible(Map m, Tileset t, int x, int y, int w, int h)
		{
			// To determine if placing a feature in a potential location preserves map
			// traversibility, this function traces around the perimeter of the feature.
			// While tracing, if it flips from solid to walkable and back more than twice,
			// it breaks traversibility and rejects it.
			bool result = true;
			bool solid = t.Solids().Contains(m[y - 1, x - 1]);
			int flips = 0;
			for (int i = x - 1; i < x + w; i++)
			{
				if (solid != t.Solids().Contains(m[y - 1, i]))
				{
					flips++;
					solid = !solid;
					//m[i, y - 1] = (byte)flips;
				}
				//else m[i, y - 1] = 0x21;
			}
			for (int i = y - 1; i < y + h; i++)
			{
				if (solid != t.Solids().Contains(m[i, x + w]))
				{
					flips++;
					solid = !solid;
					//m[x + w, i] = (byte)flips;
				}
				//else m[x + w, i] = 0x21;
			}
			for (int i = x + w; i >= x - 1; i--)
			{
				if (solid != t.Solids().Contains(m[y + h, i]))
				{
					flips++;
					solid = !solid;
					//m[i, y + h] = (byte)flips;
				}
				//else m[i, y + h] = 0x21;
			}
			for (int i = y + h; i >= y - 1; i--)
			{
				if (solid != t.Solids().Contains(m[i, x - 1]))
				{
					flips++;
					solid = !solid;
					//m[x - 1, i] = (byte)flips;
				}
				//else m[x - 1, i] = 0x21;
			}
			if (flips > 2) result = false;
			return result;
		}

		private bool Legal(Map m, Tileset t, int x, int y, int w, int h, bool obstacle = false)
		{
			// Make sure the feature is not about to be placed out of bounds or on top
			// of something that shouldn't be erased.
			bool result = true;
			if (x < 1 || y < 1 || x + w > 62 || y + h > 62)
			{
				result = false;
			}
			else
			{
				for (int i = x; i < x + w; i++)
				{
					for (int j = y; j < y + h; j++)
					{
						if (obstacle)
						{
							if (m[j, i] == t.doortile || m[j, i] == t.closedoortile || m[j, i] == t.roomtile || m[j, i] == t.warptile)
							{
								result = false;
								break;
							}
						}
						else if (m[j, i] == t.warptile || m[j, i] == t.floortile)
						{
							result = false;
							break;
						}
					}
					if (!result) break;
				}
				if (result && obstacle) result = Traversible(m, t, x, y, w, h);
			}
			return result;
		}
		private bool CarveBox(Map m, Tileset t, int x, int y, int w, int h)
		{
			bool result = Legal(m, t, x, y, w, h);
			if (result)
			{
				m.Fill((x, y), (w, h), t.floortile);
				for (int i = 0; i < w; i++)
				{
					m[y, x + i] = t.walltile;
					m[y + h - 1, x + i] = t.walltile;
				}
				for (int i = 0; i < h - 1; i++)
				{
					m[y + i, x] = t.leftwalltile;
					m[y + i, x + w - 1] = t.rightwalltile;
				}
				m[y, x] = t.wallupperleft;
				m[y, x + w - 1] = t.wallupperright;
			}
			return result;
		}

		private bool CarveRoom(Map m, Tileset t, int x, int y, int w, int h)
		{
			bool result = Legal(m, t, x, y, w, h, true);
			if (result)
			{
				m.Fill((x, y), (w, h - 1), t.roomtile);
			}
			return result;
		}
		private void GenerateRooms(MT19337 rng, Map m, Tileset t)
		{
			List<Candidate> candidates = new List<Candidate>();
			Candidate c;
			int x;
			int y;
			int w;
			int h;
			for (int i = 0; i < 63; i++)
			{
				for (int j = 0; j < 63; j++)
				{
					if (m[j, i] == t.floortile) candidates.Add(new Candidate(i, j));
				}
			}
			int attempts = 40;
			for (int attempt = 0; attempt < attempts; attempt++)
			{
				c = DrawCard<Candidate>(rng, candidates);
				w = RollDice(rng, 3, 4);
				h = RollDice(rng, 3, 4) + 1;
				x = c.x - RollDice(rng, 1, w - 2);
				y = c.y - h;
				if (CarveRoom(m, t, x, y, w, h))
				{
					m[c.y - 1, c.x] = t.doortile;
					m[c.y, c.x] = t.closedoortile;
				}
			}
		}

		private byte PlaceExit(MT19337 rng, Map m, Tileset t)
		{
			List<Candidate> candidates = new List<Candidate>();
			byte exittile = 0;
			if (t.teleportdeck.Count() > 0)
			{
				exittile = DrawCard<byte>(rng, t.teleportdeck);
				for (int i = 0; i < 64; i++)
				{
					for (int j = 0; j < 64; j++)
					{
						if (m[j, i] == t.floortile)
						{
							candidates.Add(new Candidate(i, j));
						}
					}
				}
				Candidate c = DrawCard<Candidate>(rng, candidates);
				m[c.y, c.x] = exittile;
			}
			return exittile;
		}
		private void GenerateMapBoxStyle(MT19337 rng, Map m, Tileset t)
		{
			// Draw the initial box containing the "back" staircase.
			int w = RollDice(rng, 3, 4);
			int h = RollDice(rng, 3, 4);
			CarveBox(m, t, 32 - w / 2, 32 - h / 2, w, h);
			m[32, 32] = t.warptile;

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
						if (m[j, i] == t.floortile)
						{
							if (m[j - 1, i] != t.floortile || m[j + 1, i] != t.floortile || m[j, i - 1] != t.floortile || m[j, i + 1] != t.floortile)
							{
								c = new Candidate(i, j);
								if (m[j - 1, i] != t.floortile) c.dirs.Add(northdir);
								if (m[j + 1, i] != t.floortile) c.dirs.Add(southdir);
								if (m[j, i - 1] != t.floortile) c.dirs.Add(westdir);
								if (m[j, i + 1] != t.floortile) c.dirs.Add(eastdir);
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
						if (CarveBox(m, t, c.x - w / 2, c.y - h, w, h)) m[c.y - 1, c.x] = t.floortile;
						break;
					case southdir:
						if (CarveBox(m, t, c.x - w / 2, c.y + 1, w, h)) m[c.y + 1, c.x] = t.floortile;
						break;
					case westdir:
						if (CarveBox(m, t, c.x - w, c.y - h / 2, w, h)) m[c.y, c.x - 1] = t.floortile;
						break;
					case eastdir:
						if (CarveBox(m, t, c.x + 1, c.y - h / 2, w, h)) m[c.y, c.x + 1] = t.floortile;
						break;
				}

			}
			GenerateRooms(rng, m, t);
		}
	}
}
