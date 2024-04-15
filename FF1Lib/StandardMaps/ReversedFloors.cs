using System.Reflection.Metadata.Ecma335;

namespace FF1Lib
{
	public class ReversedFloors
	{
		FF1Rom rom;
		StandardMaps maps;
		MT19337 rng;
		//NormTeleData tele;
		//EnterTeleData enter;
		Teleporters teleporters;
		List<MapIndex> vflippedMaps;

		public ReversedFloors(FF1Rom _rom, StandardMaps _maps, MT19337 _rng, Teleporters _teleporters, List<MapIndex> _vflippedMaps)
		{
			rom = _rom;
			maps = _maps;
			rng = _rng;
			vflippedMaps = _vflippedMaps;
			teleporters = _teleporters;
		}

		public void Work()
		{

			SwapTilesAndTele(MapIndex.TempleOfFiends, 0x14, 0x1E, 0x14, 0x06);

			SwapTilesAndTele(MapIndex.EarthCaveB1, 0x17, 0x18, 0x13, 0x24, 0x1C, 0x2F);

			SwapTilesAndTele(MapIndex.GurguVolcanoB1, 0x1B, 0x0F, 0x15, 0x17, 0x06, 0x10);

			SwapTilesAndTele(MapIndex.IceCaveB1, 0x07, 0x01, 0x1E, 0x0B);

			SwapTilesAndTele(MapIndex.IceCaveB1, 0x06, 0x14, 0x19, 0x12);

			SwapTilesAndTele(MapIndex.BahamutCaveB1, 0x02, 0x02, 0x02, 0x35);

			SwapTilesAndTele(MapIndex.MarshCaveB1, 0x15, 0x1B, 0x01, 0x07, 0x2D, 0x1A);

			SwapTilesAndTele(MapIndex.MirageTower1F, 0x0E, 0x1F, 0x0E, 0x01);
			SwapTilesAndTele(MapIndex.MirageTower1F, 0x11, 0x1F, 0x11, 0x01);
			DuplicateTile(MapIndex.MirageTower1F, 0x0F, 0x1F, 0x0F, 0x01);
			DuplicateTile(MapIndex.MirageTower1F, 0x10, 0x1F, 0x10, 0x01);

			SwapTilesAndTele(MapIndex.MarshCaveB2, 0x12, 0x10, 0x01, 0x0E, 0x12, 0x03);

			SwapTilesAndTele(MapIndex.MarshCaveB2, 0x22, 0x25, 0x0E, 0x32, 0x34, 0x20);

			SwapTilesAndTele(MapIndex.MarshCaveB3, 0x05, 0x06, 0x38, 0x06, 0x2C, 0x27);

			SwapTilesAndTele(MapIndex.EarthCaveB2, 0x0A, 0x09, 0x1B, 0x0A, 0x14, 0x23);

			SwapTilesAndTele(MapIndex.EarthCaveB3, 0x1B, 0x2D, 0x34, 0x13);

			SwapTilesAndTele(MapIndex.EarthCaveB4, 0x3D, 0x21, 0x09, 0x09);
			SwapTilesAndTele(MapIndex.EarthCaveB4, 0x09, 0x09, 0x13, 0x0B);
			SwapTilesAndTele(MapIndex.EarthCaveB4, 0x3D, 0x21, 0x35, 0x28);

			SwapTilesAndTele(MapIndex.EarthCaveB5, 0x19, 0x35, 0x26, 0x37, 0x11, 0x17);

			SwapTilesAndTele(MapIndex.GurguVolcanoB2, 0x1E, 0x20, 0x01, 0x20);

			SwapVolcanoB3(MapIndex.GurguVolcanoB3, 0x12, 0x02, 0x15, 0x02);

			SwapTilesAndTele(MapIndex.GurguVolcanoB3, 0x2E, 0x17, 0x01, 0x14, 0x2C, 0x31);

			SwapTilesAndTele(MapIndex.GurguVolcanoB4, 0x03, 0x17, 0x17, 0x2D);

			SwapTilesAndTele(MapIndex.GurguVolcanoB4, 0x23, 0x06, 0x28, 0x2D);

			SwapTilesAndTele(MapIndex.GurguVolcanoB5, 0x20, 0x1F, 0x32, 0x05, 0x0D, 0x05);

			SwapTilesAndTele(MapIndex.IceCaveB2, 0x1E, 0x02, 0x05, 0x02);

			SwapIceB3();

			SwapTwoTilesAndTele(MapIndex.MirageTower2F, 0x10, 0x1F, 0x17, 0x03, 0x10, 0x03, 0x12, 0x1C);

			SwapTilesAndTele(MapIndex.SeaShrineB1, 0x0C, 0x1A, 0x1E, 0x13, 0x0C, 0x01);

			SwapSeaShrineB2();

			SwapTilesAndTele(MapIndex.SeaShrineB3, 0x15, 0x2A, 0x02, 0x02, 0x1F, 0x07);

			SwapTilesAndTele(MapIndex.SeaShrineB3, 0x30, 0x0A, 0x2F, 0x1D);

			SwapTilesAndTele(MapIndex.SeaShrineB4, 0x3D, 0x31, 0x26, 0x31, 0x14, 0x30);

			SwapTilesAndTele(MapIndex.SeaShrineB4, 0x2D, 0x14, 0x28, 0x0E, 0x1B, 0x1A);

			SwapTilesAndTele(MapIndex.SeaShrineB5, 0x32, 0x30, 0x34, 0x18, 0x14, 0x37);

			SwapSky1F(MapIndex.SkyPalace1F, 0x11, 0x11, 0x11, 0x22, 0x01, 0x11);

			SwapTwoTilesAndTele(MapIndex.SkyPalace2F, 0x13, 0x04, 0x07, 0x23, 0x13, 0x26, 0x20, 0x08);

			SwapTilesAndTele(MapIndex.SkyPalace3F, 0x18, 0x17, 0x11, 0x29, 0x1B, 0x37);

			SwapTilesAndTele(MapIndex.IceCaveB3, 0x03, 0x02, 0x05, 0x04);

			SwapTwoTilesAndTele(MapIndex.SeaShrineB2, 0x3D, 0x31, 0x35, 0x2C, 0x36, 0x29, 0x3C, 0x2C);

			SwapTilesAndTele(MapIndex.SeaShrineB3, 0x2F, 0x27, 0x31, 0x25);

			SwapMirage3F();
		}

		private void SwapMirage3F()
		{
			if (rng.Between(0, 1) == 0) return;

			var map = maps[MapIndex.MirageTower3F].Map;

			SwapTiles(map, 0x08, 0x01, 0x0E, 0x08);
			SwapTele(MapIndex.MirageTower3F, 0x08, 0x01, 0x0E, 0x08);

			DuplicateTile(MapIndex.MirageTower3F, 0x03, 0x07, 0x02, 0x07);
			DuplicateTile(MapIndex.MirageTower3F, 0x03, 0x08, 0x02, 0x08);
			DuplicateTile(MapIndex.MirageTower3F, 0x03, 0x09, 0x02, 0x09);
			DuplicateTile(MapIndex.MirageTower3F, 0x03, 0x0A, 0x02, 0x0A);

			SwapTiles(map, 0x08, 0x0B, 0x03, 0x08);
			SwapTiles(map, 0x08, 0x0C, 0x03, 0x09);
			SwapTiles(map, 0x08, 0x0D, 0x03, 0x0A);
			SwapTiles(map, 0x08, 0x0E, 0x03, 0x0B);

			DuplicateTile(MapIndex.MirageTower3F, 0x08, 0x03, 0x03, 0x07);
			DuplicateTile(MapIndex.MirageTower3F, 0x07, 0x0C, 0x08, 0x0C);
			DuplicateTile(MapIndex.MirageTower3F, 0x07, 0x0B, 0x08, 0x0B);
		}

		private void SwapSeaShrineB2()
		{
			SwapTilesAndTele(MapIndex.SeaShrineB2, 0x2D, 0x08, 0x04, 0x13, 0x0D, 0x2D);

			DuplicateTile(MapIndex.SeaShrineB2, 0x2C, 0x30, 0x0E, 0x2D);
			DuplicateTile(MapIndex.SeaShrineB2, 0x2C, 0x30, 0x0F, 0x2D);
			DuplicateTile(MapIndex.SeaShrineB2, 0x2C, 0x30, 0x10, 0x2D);
			DuplicateTile(MapIndex.SeaShrineB2, 0x2C, 0x30, 0x11, 0x2D);
			DuplicateTile(MapIndex.SeaShrineB2, 0x2C, 0x30, 0x0E, 0x2E);
			DuplicateTile(MapIndex.SeaShrineB2, 0x2C, 0x30, 0x0F, 0x2E);
			DuplicateTile(MapIndex.SeaShrineB2, 0x2C, 0x30, 0x10, 0x2E);
			DuplicateTile(MapIndex.SeaShrineB2, 0x2C, 0x30, 0x11, 0x2E);
		}

		private void SwapSky1F(MapIndex mapindex, int x1, int y1, int x2, int y2, int x3, int y3)
		{
			var map = maps[mapindex].Map;

			int tx;
			int ty;
			switch (rng.Between(0, 2))
			{
				case 1:
					tx = x2;
					ty = y2;
					break;
				case 2:
					tx = x3;
					ty = y3;
					break;
				default:
					return;
			}

			for (int x = 0; x <= 4; x++)
			{
				for (int y = 0; y <= 7; y++)
				{
					if (x == 0 && y < 2) continue;
					if (x == 4 && y < 2) continue;
					SwapTiles(map, x1 + x, y1 + y, tx + x, ty + y);
					SwapTele(mapindex, x1 + x, y1 + y, tx + x, ty + y);
				}
			}
		}

		private void SwapIceB3()
		{
			var mapindex = MapIndex.IceCaveB3;
			int x1 = 0x27, y1 = 0x06, x2 = 0x1F, y2 = 0x16, x3 = 0x3B, y3 = 0x1E;

			if (vflippedMaps.Contains(mapindex)) y3 = 0x23;

			var map = maps[mapindex].Map;

			int tx;
			int ty;
			int ymin = -1;
			switch (rng.Between(0, 2))
			{
				case 1:
					tx = x2;
					ty = y2;
					break;
				case 2:
					tx = x3;
					ty = y3;
					if (vflippedMaps.Contains(mapindex)) ymin = -2;
					break;
				default:
					return;
			}

			for (int x = -1; x <= 1; x++)
			{
				for (int y = ymin; y <= 3; y++)
				{
					if (x == -1 && y == 3) continue;
					if (x == 1 && y == 3) continue;
					SwapTiles(map, x1 + x, y1 + y, tx + x, ty + y);
					SwapTele(mapindex, x1 + x, y1 + y, tx + x, ty + y);
				}
			}
		}

		private void SwapVolcanoB3(MapIndex mapindex, int x1, int y1, int x2, int y2)
		{
			if (rng.Between(0, 1) == 0) return;
			SwapTiles(maps[mapindex].Map, x1, y1, x2, y2);
			SwapTele(mapindex, x1, y1, x2, y2);
		}

		private void DuplicateTile(MapIndex mapindex, int x1, int y1, int x2, int y2)
		{
			var map = maps[mapindex].Map;
			map[y2, x2] = map[y1, x1];
		}

		private void SwapTwoTilesAndTele(MapIndex mapindex, int x1, int y1, int x2, int y2, int x3, int y3, int x4, int y4)
		{
			if (rng.Between(0, 1) == 0) return;
			SwapTiles(maps[mapindex].Map, x1, y1, x2, y2);
			SwapTiles(maps[mapindex].Map, x3, y3, x4, y4);
			SwapTele(mapindex, x1, y1, x2, y2);
			SwapTele(mapindex, x3, y3, x4, y4);
		}

		private void SwapTilesAndTele(MapIndex mapindex, int x1, int y1, int x2, int y2, int x3, int y3)
		{
			switch (rng.Between(0, 2))
			{
				case 1:
					SwapTiles(maps[mapindex].Map, x1, y1, x2, y2);
					SwapTele(mapindex, x1, y1, x2, y2);
					return;
				case 2:
					SwapTiles(maps[mapindex].Map, x1, y1, x3, y3);
					SwapTele(mapindex, x1, y1, x3, y3);
					return;
				default:
					return;
			}
		}

		private void SwapTilesAndTele(MapIndex mapindex, int x1, int y1, int x2, int y2)
		{
			if (rng.Between(0, 1) == 0) return;
			SwapTiles(maps[mapindex].Map, x1, y1, x2, y2);
			SwapTele(mapindex, x1, y1, x2, y2);
		}

		private void SwapTiles(Map map, int x1, int y1, int x2, int y2)
		{
			var t = map[y1, x1];
			map[y1, x1] = map[y2, x2];
			map[y2, x2] = t;
		}

		private void SwapTele(MapIndex mapindex, int x1, int y1, int x2, int y2)
		{
			var overtoswap = teleporters.OverworldTeleporters.Where(t => t.Value.Index == mapindex && t.Value.Coordinates.X == x1 && t.Value.Coordinates.Y == y1).ToList();
			overtoswap.ForEach(t => t.Value.SetTeleporter(new Coordinate((byte)x2, (byte)y2, t.Value.Coordinates.Context)));

			var teletoswap = teleporters.StandardMapTeleporters.Where(t => t.Value.Index == mapindex && t.Value.Coordinates.X == x1 && t.Value.Coordinates.Y == y1).ToList();
			teletoswap.ForEach(t => t.Value.SetTeleporter(new Coordinate((byte)x2, (byte)y2, t.Value.Coordinates.Context)));
			/*

			for (int i = 0; i < 64; i++)
			{
				var t = tele[i];
				if (t.Map == MapIndex && t.X == x1 && t.Y == y1)
				{
					t.X = (byte)(x2 & 0x3F | t.X & 0xC0);
					t.Y = (byte)(y2 & 0x3F | t.Y & 0xC0);
					tele[i] = t;
					break;
				}
			}

			for (int i = 0; i < 32; i++)
			{
				var t = enter[i];
				if (t.Map == MapIndex && t.X == x1 && t.Y == y1)
				{
					t.X = (byte)(x2 & 0x3F | t.X & 0xC0);
					t.Y = (byte)(y2 & 0x3F | t.Y & 0xC0);
					enter[i] = t;
					break;
				}
			}*/
		}
	}
}
