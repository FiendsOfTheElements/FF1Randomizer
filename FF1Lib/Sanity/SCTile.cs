using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace FF1Lib.Sanity
{
	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public unsafe struct SCTile
	{
		private const int ExtFlagCount = 5;

		public SCBitFlags Tile;

		public fixed ushort ExtFlags[ExtFlagCount];

		public SCTile(SCBitFlags bitflags)
		{
			Tile = bitflags;

			for (int i = 1; i < ExtFlagCount; i++) ExtFlags[i] = (ushort)SCBitFlags.None;
		}

		public override string ToString()
		{
			return ExtFlags[0].ToString("X") + ":" + Tile.ToString("X");
		}

		public void AddExtFlag(SCBitFlags req)
		{
			for (int i = 1; i < ExtFlagCount; i++)
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

			for (; i < ExtFlagCount; i++)
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

			if (orthogonal && i >= ExtFlagCount) i = CompactFlags();

			if (orthogonal && i < ExtFlagCount)
			{
				ExtFlags[i] = (ushort)req;
				result = true;
			}

			return result;
		}

		private int CompactFlags()
		{
			int result = ExtFlagCount;
			for (int i = ExtFlagCount - 1; i > 0; i--)
				for (int j = i - 1; j >= 0; j--)
				{
					if (((SCBitFlags)ExtFlags[i]).IsSupersetOf((SCBitFlags)ExtFlags[j]))
					{
						ExtFlags[i] = (ushort)SCBitFlags.None;
						result--;
					}
				}

			if (result == ExtFlagCount)
			{
				throw new MadnessException("There are more the 7 orthogonal requirements for a tile!");
			}

			return result;
		}

		public bool IsSubsetOf(SCBitFlags req)
		{
			for (int i = 0; i < ExtFlagCount; i++)
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
			for (int i = 0; i < ExtFlagCount; i++)
			{
				if (ExtFlags[i] == (ushort)SCBitFlags.None) break;
				result.Add((SCBitFlags)ExtFlags[i]);
			}

			return result;
		}

		public SCBitFlagSet GetPassableBitFlagSet()
		{
			CompactFlags();

			SCBitFlagSet result = new SCBitFlagSet();
			for (int i = 0; i < ExtFlagCount; i++)
			{
				if (ExtFlags[i] == (ushort)SCBitFlags.None) break;
				if (!((SCBitFlags)ExtFlags[i]).IsImpassable()) result.Add((SCBitFlags)ExtFlags[i]);
			}

			return result;
		}

		public bool IsOrthogonalTo(SCBitFlags req)
		{
			for (int i = 1; i < ExtFlagCount; i++)
			{
				var flags = (SCBitFlags)ExtFlags[i];

				if (flags == SCBitFlags.None) return true;
				if (!flags.IsOrthogonalTo(req)) return false;
			}

			return true;
		}

		public bool IsSupersetOf(SCBitFlags req)
		{
			for (int i = 1; i < ExtFlagCount; i++)
			{
				var flags = (SCBitFlags)ExtFlags[i];

				if (flags == SCBitFlags.None) return false;
				if (flags.IsSupersetOf(req)) return true;
			}

			return false;
		}
	}
}
