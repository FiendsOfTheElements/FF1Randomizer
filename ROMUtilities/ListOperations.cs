using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ROMUtilities
{
	public static class ListOperations
	{
		public static void Shuffle<T>(this List<T> items, MT19337 rng)
		{
			for (int i = 0; i < items.Count; i++)
			{
				items.Swap(i, rng.Between(i, items.Count - 1));
			}
		}

		public static void Swap<T>(this List<T> items, int i, int j)
		{
			T temp = items[i];
			items[i] = items[j];
			items[j] = temp;
		}
	}
}
