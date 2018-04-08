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
    }

}
