using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ROMUtilities
{
	public static class Rng
	{
		public static int Between(this MT19337 rng, int min, int max)
		{
			int range = max - min;
			uint mask;
			int maskDisplacement;
			GetMask(range, out mask, out maskDisplacement);

			return min + GetRandom(rng, range, mask, maskDisplacement);
		}

		private static int GetRandom(MT19337 rng, int maxValue, uint mask, int maskDisplacement)
		{
			uint value;
			do
			{
				value = rng.Next();
				value &= mask;
				value >>= maskDisplacement; // Slide the bits back down.
			} while (value > (uint)maxValue); // Throw away values until we get one in the desired range.
											  // For example, we may generate values 0-7 for a 6-sided die (since we need 3 bits to represent
											  // values 0-5).

			return (int)value;
		}

		private static void GetMask(int range, out uint mask, out int maskDisplacement)
		{
			// We want to mask off the fewest number of pseudorandom bits that gives us the full range.
			mask = 0x00000003;
			maskDisplacement = 8 * sizeof(uint) - 2;
			while (((uint)range & mask) < (uint)range)
			{
				mask <<= 1;
				mask |= 1;
				maskDisplacement--;
			}
			mask <<= maskDisplacement; // We want the most significant bits, so slide the mask over.
		}
	}
}
