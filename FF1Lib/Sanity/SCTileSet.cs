﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FF1Lib.Sanity
{
	public class SCTileSet
	{
		public int Index { get; private set; }
		public TileSet TileSet { get; private set; }

		public SCTileDef[] Tiles = new SCTileDef[128];

		FF1Rom rom;

		public SCTileSet(FF1Rom _rom, int i)
		{
			rom = _rom;
			Index = i;

			TileSet = new TileSet(rom, (byte)i);
			TileSet.LoadData();

			for (int j = 0; j < 128; j++) Tiles[j] = new SCTileDef(TileSet.TileProperties[j]);
		}
	}
}
