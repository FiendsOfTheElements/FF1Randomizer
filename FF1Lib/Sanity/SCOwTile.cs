using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FF1Lib.Sanity
{
	public struct SCOwTile
	{
		public SCBitFlags Tile;

		public short Area;

		public SCOwTile(SCBitFlags tile)
		{
			Tile = tile;
			Area = -1;
		}
	}
}
