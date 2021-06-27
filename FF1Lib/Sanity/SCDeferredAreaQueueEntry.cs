using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FF1Lib.Sanity
{
	public struct SCDeferredAreaQueueEntry
	{
		public short Source { get; set; }

		public short Target { get; set; }

		public SCDeferredAreaQueueEntry(short source, short target) : this()
		{
			Source = source;
			Target = target;
		}
	}

	public class SCDeferredAreaQueueEntryEqualityComparer : IEqualityComparer<SCDeferredAreaQueueEntry>
	{
		public bool Equals(SCDeferredAreaQueueEntry x, SCDeferredAreaQueueEntry y)
		{
			return x.Source == y.Source && x.Target == y.Target;
		}

		public int GetHashCode([DisallowNull] SCDeferredAreaQueueEntry obj)
		{
			return (int)obj.Source + 2048 * (int)obj.Target;
		}
	}
}
