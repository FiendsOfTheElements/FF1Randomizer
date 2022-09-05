using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FF1Lib.Sanity
{

	public struct SCTileQueueEntry
	{
		public SCBitFlags BitFlags { get; set; }

		public SCCoords Coords { get; set; }
	}

	public class SCTileQueueEntryEqualityComparer : IEqualityComparer<SCTileQueueEntry>
	{
		public bool Equals(SCTileQueueEntry x, SCTileQueueEntry y)
		{
			return x.Coords.X == y.Coords.X && x.Coords.Y == y.Coords.Y && x.BitFlags == y.BitFlags;
		}

		public int GetHashCode([DisallowNull] SCTileQueueEntry obj)
		{
			return obj.Coords.X & 0x0F + (obj.Coords.Y & 0x0f) * 16 + (obj.Coords.X & 0xF0) * 256 + (obj.Coords.Y & 0xF0) * 4096 + ((int)obj.BitFlags & 0x0FFF) * 65536;
		}
	}
}
