using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace FF1Lib.Sanity
{
	[StructLayout(LayoutKind.Explicit, Size = 16)]
	public unsafe struct SCTile
	{
		[FieldOffset(0)]
		public SCBitFlags Tile;

		[FieldOffset(2)]
		public SCBitFlags Flags;

		[FieldOffset(2)]
		public fixed ushort ExtFlags[7];

		public SCTile(SCTileDef tileDef)
		{
			Tile = tileDef.BitFlags;
			Flags = SCBitFlags.None;

			ExtFlags[1] = (ushort)SCBitFlags.None;
			ExtFlags[2] = (ushort)SCBitFlags.None;
			ExtFlags[3] = (ushort)SCBitFlags.None;
			ExtFlags[4] = (ushort)SCBitFlags.None;
			ExtFlags[5] = (ushort)SCBitFlags.None;
			ExtFlags[6] = (ushort)SCBitFlags.None;
		}

		public override string ToString()
		{
			return Flags.ToString("X") + ":" + Tile.ToString("X");
		}

		public void AddExtFlag(SCBitFlags req)
		{
			for (int i = 1; i < 7; i++)
			{
				if (ExtFlags[i] == (ushort)SCBitFlags.None)
				{
					ExtFlags[i] = (ushort)req;
					return;
				}
			}

		}

		public bool MergeFlag(SCBitFlags req)
		{
			bool result = false;
			bool orthogonal = true;
			int i = 0;

			for (; i < 7; i++)
			{
				if (ExtFlags[i] == (ushort)SCBitFlags.None) break;
				if (((SCBitFlags)ExtFlags[i]).IsStrictSupersetOf(req))
				{
					ExtFlags[i] = (ushort)req;
					result = true;
				}
				else if (!((SCBitFlags)ExtFlags[i]).IsOrthogonalTo(req))
				{
					orthogonal = false;
				}
			}

			if (orthogonal && i >= 7) i = CompactFlags();

			if (orthogonal && i < 7)
			{
				ExtFlags[i] = (ushort)req;
				result = true;
			}

			return result;
		}

		private int CompactFlags()
		{
			int result = 7;
			for (int i = 6; i > 0; i--)
				for (int j = i - 1; j >= 0; j--)
				{
					if (((SCBitFlags)ExtFlags[i]).IsSupersetOf((SCBitFlags)ExtFlags[j]))
					{
						ExtFlags[i] = (ushort)SCBitFlags.None;
						result--;
					}
				}

			if (result == 7)
			{
				throw new MadnessException("There are more the 7 orthogonal requirements for a tile!");
			}

			return result;
		}

		public bool IsSubsetOf(SCBitFlags req)
		{
			for (int i = 0; i < 7; i++)
			{
				var flags = (SCBitFlags)ExtFlags[i];

				if (flags == SCBitFlags.None) return false;
				if (flags.IsSubsetOf(req)) return true;
			}

			return false;
		}

		public SCBitFlagSet GetBitFlagSet()
		{
			CompactFlags();

			SCBitFlagSet result = new SCBitFlagSet();
			for (int i = 0; i < 7; i++)
			{
				if (ExtFlags[i] == (ushort)SCBitFlags.None) break;
				result.Add((SCBitFlags)ExtFlags[i]);
			}

			return result;
		}

		public bool IsOrthogonalTo(SCBitFlags req)
		{
			for (int i = 1; i < 7; i++)
			{
				var flags = (SCBitFlags)ExtFlags[i];

				if (flags == SCBitFlags.None) return true;
				if (!flags.IsOrthogonalTo(req)) return false;
			}

			return true;
		}

		public bool IsSupersetOf(SCBitFlags req)
		{
			for (int i = 1; i < 7; i++)
			{
				var flags = (SCBitFlags)ExtFlags[i];

				if (flags == SCBitFlags.None) return false;
				if (flags.IsSupersetOf(req)) return true;
			}

			return false;
		}
	}
}
