using System.Diagnostics;

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

		[DebuggerStepThrough]
		public static T SpliceRandomItemWeighted<T>(this IList<(T Item, int Weight)> weightedList, MT19337 rng)
		{
			int offset = 0;
			(T Item, int RangeTo, int Weight)[] rangedItems = weightedList
				.OrderBy(item => item.Weight)
				.Select(entry => (entry.Item, RangeTo: offset += entry.Weight, entry.Weight))
				.ToArray();

			int randomNumber = rng.Between(1, weightedList.Sum(item => item.Weight));
			(T Item, int RangeTo, int Weight) picked = rangedItems.First(item => randomNumber <= item.RangeTo);

			//remove the picked item from the list
			for(int i = 0; i < weightedList.Count; i++)
			{
				if(weightedList[i].Item.Equals(picked.Item) && weightedList[i].Weight == picked.Weight)
				{
					weightedList.RemoveAt(i);
					break;
				}
			}
			
			return picked.Item;
		}
		[DebuggerStepThrough]
		public static bool TryFind<T>(this IList<T> fromList, Predicate<T> query, out T result)
		{
			int resultIndex = fromList.ToList().FindIndex(query);
			if (resultIndex < 0)
			{
				result = default;
				return false;
			}
			else
			{
				result = fromList[resultIndex];
				return true;
			}
		}
	}

}
