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
		List<byte> fullteleportdeck = new List<byte>();
		List<byte> tilesetspinner = new List<byte>();

		private struct TileGraphic
		{
			public byte topleft;
			public byte topright;
			public byte botleft;
			public byte botright;

			public TileGraphic(byte tl, byte tr, byte bl, byte br)
			{
				topleft = tl;
				topright = tr;
				botleft = bl;
				botright = br;
			}
		}
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
			public TileGraphic warpgraphic;
			public TileGraphic teleportgraphic;
			public TileGraphic laddergraphic;
			public List<byte> teleportdeck;

			public byte[] Solids()
			{
				byte[] result = {
					walltile, leftwalltile, rightwalltile, wallupperleft, wallupperright, abysstile,
					roomright, roomleft, roomupper, roomupperright, roomupperleft, roomlowerright, roomlowerleft
				};
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
		private struct FormationLevel
		{
			public byte index;
			public int level;
			public Encounters.FormationData formation;

		}
		private int MonsterLevel(EnemyInfo monster)
		{
			int result = 0;
			if (monster.exp == 1 && monster.gp == 1)
			{
				result = (int)Math.Pow(32000, 2) * 2;
			}
			else
			{
				result += (int)Math.Pow(monster.exp, 2);
				result += (int)Math.Pow(monster.gp, 2);
			}
			return result;
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
			tilesets[1].warpgraphic.topleft = 0x28;
			tilesets[1].warpgraphic.topright = 0x29;
			tilesets[1].warpgraphic.botleft = 0x38;
			tilesets[1].warpgraphic.botright = 0x39;
			tilesets[1].teleportgraphic.topleft = 0x26;
			tilesets[1].teleportgraphic.topright = 0x27;
			tilesets[1].teleportgraphic.botleft = 0x36;
			tilesets[1].teleportgraphic.botright = 0x37;
			tilesets[1].laddergraphic.topleft = 0x10;
			tilesets[1].laddergraphic.topright = 0x2F;
			tilesets[1].laddergraphic.botleft = 0x10;
			tilesets[1].laddergraphic.botright = 0x2F;
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
			tilesets[2].warpgraphic.topleft = 0x28;
			tilesets[2].warpgraphic.topright = 0x29;
			tilesets[2].warpgraphic.botleft = 0x38;
			tilesets[2].warpgraphic.botright = 0x39;
			tilesets[2].teleportgraphic.topleft = 0x26;
			tilesets[2].teleportgraphic.topright = 0x27;
			tilesets[2].teleportgraphic.botleft = 0x36;
			tilesets[2].teleportgraphic.botright = 0x37;
			tilesets[2].laddergraphic.topleft = 0x10;
			tilesets[2].laddergraphic.topright = 0x6A;
			tilesets[2].laddergraphic.botleft = 0x10;
			tilesets[2].laddergraphic.botright = 0x6A;
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
			tilesets[2].teleportdeck.Add(0x09 + 0x80);
			tilesets[2].teleportdeck.Add(0x0F + 0x80);
			tilesets[2].teleportdeck.Add(0x1F + 0x80);

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
			tilesets[3].warpgraphic.topleft = 0x28;
			tilesets[3].warpgraphic.topright = 0x29;
			tilesets[3].warpgraphic.botleft = 0x38;
			tilesets[3].warpgraphic.botright = 0x39;
			tilesets[3].teleportgraphic.topleft = 0x26;
			tilesets[3].teleportgraphic.topright = 0x27;
			tilesets[3].teleportgraphic.botleft = 0x36;
			tilesets[3].teleportgraphic.botright = 0x37;
			tilesets[3].laddergraphic.topleft = 0x4C;
			tilesets[3].laddergraphic.topright = 0x4D;
			tilesets[3].laddergraphic.botleft = 0x5C;
			tilesets[3].laddergraphic.botright = 0x5D;
			tilesets[3].teleportdeck = new List<byte>();
			tilesets[3].teleportdeck.Add(0x19);
			tilesets[3].teleportdeck.Add(0x1A);
			tilesets[3].teleportdeck.Add(0x1B);
			tilesets[3].teleportdeck.Add(0x29);
			tilesets[3].teleportdeck.Add(0x2A);
			tilesets[3].teleportdeck.Add(0x2B);
			tilesets[3].teleportdeck.Add(0x2C);
			tilesets[3].teleportdeck.Add(0x2D + 0x80);
			tilesets[3].teleportdeck.Add(0x2E + 0x80);

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
			tilesets[4].warpgraphic.topleft = 0x28;
			tilesets[4].warpgraphic.topright = 0x29;
			tilesets[4].warpgraphic.botleft = 0x38;
			tilesets[4].warpgraphic.botright = 0x39;
			tilesets[4].teleportgraphic.topleft = 0x26;
			tilesets[4].teleportgraphic.topright = 0x27;
			tilesets[4].teleportgraphic.botleft = 0x36;
			tilesets[4].teleportgraphic.botright = 0x37;
			tilesets[4].laddergraphic.topleft = 0x10;
			tilesets[4].laddergraphic.topright = 0x61;
			tilesets[4].laddergraphic.botleft = 0x10;
			tilesets[4].laddergraphic.botright = 0x61;
			tilesets[4].teleportdeck = new List<byte>();
			tilesets[4].teleportdeck.Add(0x20);
			tilesets[4].teleportdeck.Add(0x22);
			tilesets[4].teleportdeck.Add(0x27);
			tilesets[4].teleportdeck.Add(0x28);
			tilesets[4].teleportdeck.Add(0x09 + 0x80);

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
			tilesets[5].warpgraphic.topleft = 0x28;
			tilesets[5].warpgraphic.topright = 0x29;
			tilesets[5].warpgraphic.botleft = 0x38;
			tilesets[5].warpgraphic.botright = 0x39;
			tilesets[5].teleportgraphic.topleft = 0x26;
			tilesets[5].teleportgraphic.topright = 0x27;
			tilesets[5].teleportgraphic.botleft = 0x36;
			tilesets[5].teleportgraphic.botright = 0x37;
			tilesets[5].laddergraphic.topleft = 0x44;
			tilesets[5].laddergraphic.topright = 0x45;
			tilesets[5].laddergraphic.botleft = 0x54;
			tilesets[5].laddergraphic.botright = 0x55;
			tilesets[5].teleportdeck = new List<byte>();
			tilesets[5].teleportdeck.Add(0x43);
			tilesets[5].teleportdeck.Add(0x44);
			tilesets[5].teleportdeck.Add(0x45);
			tilesets[5].teleportdeck.Add(0x46);
			tilesets[5].teleportdeck.Add(0x4B);
			tilesets[5].teleportdeck.Add(0x4C);
			tilesets[5].teleportdeck.Add(0x4E);
			tilesets[5].teleportdeck.Add(0x4F);
			tilesets[5].teleportdeck.Add(0x0F + 0x80);
			tilesets[5].teleportdeck.Add(0x40 + 0x80);

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
			tilesets[6].warpgraphic.topleft = 0x62;
			tilesets[6].warpgraphic.topright = 0x63;
			tilesets[6].warpgraphic.botleft = 0x72;
			tilesets[6].warpgraphic.botright = 0x73;
			tilesets[6].teleportgraphic.topleft = 0x26;
			tilesets[6].teleportgraphic.topright = 0x27;
			tilesets[6].teleportgraphic.botleft = 0x36;
			tilesets[6].teleportgraphic.botright = 0x37;
			tilesets[6].laddergraphic.topleft = 0x28;
			tilesets[6].laddergraphic.topright = 0x29;
			tilesets[6].laddergraphic.botleft = 0x38;
			tilesets[6].laddergraphic.botright = 0x39;
			tilesets[6].teleportdeck = new List<byte>();
			tilesets[6].teleportdeck.Add(0x41);
			tilesets[6].teleportdeck.Add(0x42);
			tilesets[6].teleportdeck.Add(0x43);
			tilesets[6].teleportdeck.Add(0x44);
			tilesets[6].teleportdeck.Add(0x40 + 0x80);

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
			tilesets[7].warpgraphic.topleft = 0x28;
			tilesets[7].warpgraphic.topright = 0x29;
			tilesets[7].warpgraphic.botleft = 0x38;
			tilesets[7].warpgraphic.botright = 0x39;
			tilesets[7].teleportgraphic.topleft = 0x26;
			tilesets[7].teleportgraphic.topright = 0x27;
			tilesets[7].teleportgraphic.botleft = 0x36;
			tilesets[7].teleportgraphic.botright = 0x37;
			tilesets[7].laddergraphic.topleft = 0x10;
			tilesets[7].laddergraphic.topright = 0x6E;
			tilesets[7].laddergraphic.botleft = 0x10;
			tilesets[7].laddergraphic.botright = 0x6E;
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
			tilesets[7].teleportdeck.Add(0x09 + 0x80);
			tilesets[7].teleportdeck.Add(0x40 + 0x80);

			// Read which maps have which tilesets
			tilesetmappings = Get(0x2CC0, 61).ToBytes();

			// Correct the tile graphics so that "back" stairs look like they go down and
			// "forward" stairs look like they go up.
			for (int i = 1; i < 8; i++)
			{
				Put(0x1000 + i * 0x200 + tilesets[i].warptile + 0x000, Blob.FromHex(Convert.ToHexString(new byte[] { tilesets[i].warpgraphic.topleft })));
				Put(0x1000 + i * 0x200 + tilesets[i].warptile + 0x080, Blob.FromHex(Convert.ToHexString(new byte[] { tilesets[i].warpgraphic.topright })));
				Put(0x1000 + i * 0x200 + tilesets[i].warptile + 0x100, Blob.FromHex(Convert.ToHexString(new byte[] { tilesets[i].warpgraphic.botleft })));
				Put(0x1000 + i * 0x200 + tilesets[i].warptile + 0x180, Blob.FromHex(Convert.ToHexString(new byte[] { tilesets[i].warpgraphic.botright })));
				for (int j = 0; j < tilesets[i].teleportdeck.Count(); j++)
				{
					if (tilesets[i].teleportdeck[j] >= 0x80)
					{
						Put(0x1000 + i * 0x200 + 0x000 + (tilesets[i].teleportdeck[j] % 0x80), Blob.FromHex(Convert.ToHexString(new byte[] { tilesets[i].laddergraphic.topleft })));
						Put(0x1000 + i * 0x200 + 0x080 + (tilesets[i].teleportdeck[j] % 0x80), Blob.FromHex(Convert.ToHexString(new byte[] { tilesets[i].laddergraphic.topright })));
						Put(0x1000 + i * 0x200 + 0x100 + (tilesets[i].teleportdeck[j] % 0x80), Blob.FromHex(Convert.ToHexString(new byte[] { tilesets[i].laddergraphic.botleft })));
						Put(0x1000 + i * 0x200 + 0x180 + (tilesets[i].teleportdeck[j] % 0x80), Blob.FromHex(Convert.ToHexString(new byte[] { tilesets[i].laddergraphic.botright })));
					}
					else
					{
						Put(0x1000 + i * 0x200 + 0x000 + tilesets[i].teleportdeck[j], Blob.FromHex(Convert.ToHexString(new byte[] { tilesets[i].teleportgraphic.topleft })));
						Put(0x1000 + i * 0x200 + 0x080 + tilesets[i].teleportdeck[j], Blob.FromHex(Convert.ToHexString(new byte[] { tilesets[i].teleportgraphic.topright })));
						Put(0x1000 + i * 0x200 + 0x100 + tilesets[i].teleportdeck[j], Blob.FromHex(Convert.ToHexString(new byte[] { tilesets[i].teleportgraphic.botleft })));
						Put(0x1000 + i * 0x200 + 0x180 + tilesets[i].teleportdeck[j], Blob.FromHex(Convert.ToHexString(new byte[] { tilesets[i].teleportgraphic.botright })));
					}
				}
			}

			for (int i = 1; i < 8; i++)
			{
				tilesetspinner.Add((byte)i);
			}
		}
		public void CreateDomains(MT19337 rng, List<Map> maps) 
		{
			// It's 0x72 because we want to exclude the "boss battles".
			FormationLevel[] formationlevels = new FormationLevel[0x72 * 2];
			EnemyInfo[] monsters = new EnemyInfo[EnemyCount];
			var enemies = Get(EnemyOffset, EnemySize * EnemyCount).Chunk(EnemySize);
			var formations = Get(FormationDataOffset, FormationDataSize * 0x72).Chunk(FormationDataSize);

			// Get the data for all the individual monsters so we can examine their rewards.
			for (int i = 0; i < EnemyCount; i++)
			{
				monsters[i] = new EnemyInfo();
				monsters[i].decompressData(enemies[i]);
			}

			// Determine the level of each encounter (both sides) based on the monsters it contains.
			for (int i = 0; i < 0x72; i++)
			{
				formationlevels[i] = new FormationLevel();
				formationlevels[i].formation = new Encounters.FormationData(formations[i]);
				formationlevels[i].level = 0;
				formationlevels[i].index = (byte)i;
				formationlevels[i].level += MonsterLevel(monsters[formationlevels[i].formation.enemy1]) * (formationlevels[i].formation.minmax1.Item1 + formationlevels[i].formation.minmax1.Item2) / 2;
				formationlevels[i].level += MonsterLevel(monsters[formationlevels[i].formation.enemy2]) * (formationlevels[i].formation.minmax2.Item1 + formationlevels[i].formation.minmax2.Item2) / 2;
				formationlevels[i].level += MonsterLevel(monsters[formationlevels[i].formation.enemy3]) * (formationlevels[i].formation.minmax3.Item1 + formationlevels[i].formation.minmax3.Item2) / 2;
				formationlevels[i].level += MonsterLevel(monsters[formationlevels[i].formation.enemy4]) * (formationlevels[i].formation.minmax4.Item1 + formationlevels[i].formation.minmax4.Item2) / 2;
				formationlevels[i + 0x72] = new FormationLevel();
				formationlevels[i + 0x72].formation = new Encounters.FormationData(formations[i]);
				formationlevels[i + 0x72].level = 0;
				formationlevels[i + 0x72].index = (byte)(i + 0x80);
				formationlevels[i + 0x72].level += MonsterLevel(monsters[formationlevels[i + 0x72].formation.enemy1]) * (formationlevels[i + 0x72].formation.minmaxB1.Item1 + formationlevels[i + 0x72].formation.minmaxB1.Item2) / 2;
				formationlevels[i + 0x72].level += MonsterLevel(monsters[formationlevels[i + 0x72].formation.enemy2]) * (formationlevels[i + 0x72].formation.minmaxB2.Item1 + formationlevels[i + 0x72].formation.minmaxB2.Item2) / 2;
			}

			// Sort the list by level.
			Array.Sort(formationlevels, (x, y) => x.level.CompareTo(y.level));

			// Assign random formations of appropriate level to each floor.
			var lowest = 0;
			for (int i = 8; i < 61; i++)
			{
				lowest = (i - 8) * ((0x72 * 2 - 8) / 52);
				FormationLevel[] spinner = new FormationLevel[12];
				Array.Copy(formationlevels, lowest, spinner, 0, 12);
				for (int j = 0; j < 8; j++)
				{
					Put(0x2C200 + i * 8 + j, Blob.FromHex(Convert.ToHexString(new byte[] { spinner.PickRandom(rng).index })));
				}
			}

		}
		public void DeepDungeon(MT19337 rng, OverworldMap overworldMap, List<Map> maps)
		{
			InitializeTilesets();

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

			// Kill all the NPCs.
			KillNPCs();

			// Generate new monster domains based on "estimated power level"
			CreateDomains(rng, maps);

			// Generate the map layouts.
			for (int i = 8; i < 61; i++)
			{
				// Set the encounter rate.
				Put(0x2CC00 + i, Blob.FromHex("08"));

				// Pick a tileset with unused exit tiles.
				tilesetmappings[i] = tilesetspinner.PickRandom(rng);
				Put(0x2CC0 + i, Blob.FromHex(Convert.ToHexString(new byte[] { tilesetmappings[i] })));
				overworldMap.PutPalette(OverworldTeleportIndex.ConeriaCastle1, (MapIndex)i);

				// Start from a clean slate.
				WipeMap(maps[i], tilesets[tilesetmappings[i]]);

				// Which algorithm to use; right now it's the only one.
				GenerateMapBoxStyle(rng, maps[i], tilesets[tilesetmappings[i]]);

				// Make the tiles look right.
				Beautify(maps[i], tilesets[tilesetmappings[i]]);

				// Connect it to the next map.
				if (i < 60)
				{
					Put(0x800 + tilesetmappings[i] * 0x100 + 2 * PlaceExit(rng, maps[i], tilesets[tilesetmappings[i]]), Blob.FromHex("80" + Convert.ToHexString(new byte[]{(byte)(i - 8)})));
					Put(0x2D00 + i - 8, Blob.FromHex("20"));
					Put(0x2D40 + i - 8, Blob.FromHex("A0"));
					Put(0x2D80 + i - 8, Blob.FromHex(Convert.ToHexString(new byte[] { (byte)(i + 1) })));
				}

				// If all its exits are now used, remove the tileset from the list of available ones.
				if (tilesets[tilesetmappings[i]].teleportdeck.Count() == 0)
				{
					tilesetspinner.Remove(tilesetmappings[i]);
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
								//if (m[j + 1, i] != t.roomtile && m[j + 1, i] != t.doortile)
								//{
								//	m[j + 1, i] = t.walltile;
								//}
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

		private bool Traversible(Map m, Tileset t, int x, int y, int w, int h, bool inside = false)
		{
			// To determine if placing a feature in a potential location preserves map
			// traversibility, this function traces around the perimeter of the feature.
			// While tracing, if it flips from solid to walkable and back more than twice,
			// it breaks traversibility and rejects it.
			bool result = true;
			byte[] solids = t.Solids();
			if (!inside) solids.Append(t.roomtile);
			bool solid = solids.Contains(m[y - 1, x - 1]);
			int flips = 0;
			for (int i = x - 1; i < x + w; i++)
			{
				if (solid != solids.Contains(m[y - 1, i]))
				{
					flips++;
					solid = !solid;
				}
			}
			for (int i = y - 1; i < y + h; i++)
			{
				if (solid != solids.Contains(m[i, x + w]))
				{
					flips++;
					solid = !solid;
				}
			}
			for (int i = x + w; i >= x - 1; i--)
			{
				if (solid != solids.Contains(m[y + h, i]))
				{
					flips++;
					solid = !solid;
				}
			}
			for (int i = y + h; i >= y - 1; i--)
			{
				if (solid != solids.Contains(m[i, x - 1]))
				{
					flips++;
					solid = !solid;
				}
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
				m.Fill((x, y + h - 1), (w, 1), t.walltile);
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
				//c = DrawCard<Candidate>(rng, candidates);
				c = candidates.SpliceRandom(rng);
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
			Candidate c;
			byte exittile = t.teleportdeck.SpliceRandom(rng);
			for (int i = 0; i < 64; i++)
			{
				for (int j = 0; j < 64; j++)
				{
					if (exittile >= 0x80)
					{
						if (m[j, i] == t.roomtile)
						{
							if (Traversible(m, t, i, j, 1, 1, true)) candidates.Add(new Candidate(i, j));
						}
					}
					else
					{
						if (m[j, i] == t.floortile)
						{
							if (Traversible(m, t, i, j, 1, 1)) candidates.Add(new Candidate(i, j));
						}
					}
				}
			}
			c = candidates.SpliceRandom(rng);
			exittile = (byte)(exittile % 0x80);
			m[c.y, c.x] = exittile;
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
				//c = DrawCard<Candidate>(rng, candidates);
				c = candidates.SpliceRandom(rng);
				//byte d = DrawCard<byte>(rng, c.dirs);
				byte d = c.dirs.SpliceRandom(rng);
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
