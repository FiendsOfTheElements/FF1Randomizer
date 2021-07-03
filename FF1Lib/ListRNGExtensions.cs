using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using RomUtilities;

namespace FF1Lib
{
    public static class ListRNGExtensions
    {
        [DebuggerStepThrough]
        public static T PickRandom<T>(this IList<T> fromList, MT19337 rng)
        {
            return fromList[rng.Between(0, fromList.Count - 1)];
        }
        [DebuggerStepThrough]
        public static T SpliceRandom<T>(this IList<T> fromList, MT19337 rng)
        {
            var splice = fromList.PickRandom<T>(rng);
            fromList.Remove(splice);
            return splice;
        }
        [DebuggerStepThrough]
        public static T PickRandom<T>(this HashSet<T> fromSet, MT19337 rng)
        {
            return fromSet.ToList().PickRandom<T>(rng);
        }
        [DebuggerStepThrough]
        public static T SpliceRandom<T>(this HashSet<T> fromSet, MT19337 rng)
        {
            var splice = fromSet.ToList().PickRandom<T>(rng);
            fromSet.Remove(splice);
            return splice;
        }

		[DebuggerStepThrough]
		public static T PickRandomItemWeighted<T>(this IList<(T Item, int Weight)> weightedList, MT19337 rng)
		{
			int offset = 0;
			(T Item, int RangeTo)[] rangedItems = weightedList
				.OrderBy(item => item.Weight)
				.Select(entry => (entry.Item, RangeTo: offset += entry.Weight))
				.ToArray();

			int randomNumber = rng.Between(1, weightedList.Sum(item => item.Weight));
			return rangedItems.First(item => randomNumber <= item.RangeTo).Item;
		}
	}

}
