using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FF1Lib.Sanity
{

	public struct SCOwTileQueueEntry
	{
		public SCCoords Coords { get; set; }

		public short Area { get; set; }

		public SCBitFlags Tile { get; set; }
	}

	public class SCOwTileQueueEntryComparer : IEqualityComparer<SCOwTileQueueEntry>
	{
		public bool Equals(SCOwTileQueueEntry x, SCOwTileQueueEntry y)
		{
			return x.Coords.X == y.Coords.X && x.Coords.Y == y.Coords.Y && x.Area == y.Area;
		}

		public int GetHashCode([DisallowNull] SCOwTileQueueEntry obj)
		{
			return obj.Coords.X & 0x0F + (obj.Coords.Y & 0x0f) * 16 + (obj.Coords.X & 0xF0) * 256 + (obj.Coords.Y & 0xF0) * 4096 + ((int)obj.Area) * 65536;
		}
	}
}
