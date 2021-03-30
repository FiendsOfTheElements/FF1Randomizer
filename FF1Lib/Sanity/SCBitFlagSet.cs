using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FF1Lib.Sanity
{
	public class SCBitFlagSet : List<SCBitFlags>
	{
		public SCBitFlagSet() : base() { }

		public SCBitFlagSet(params SCBitFlags[] values) : base(values) { }

		public SCBitFlagSet(IEnumerable<SCBitFlags> values) : base(values) { }

		public bool Merge(SCBitFlags requirements)
		{
			if (this.Where(req => req.IsSubsetOf(requirements)).Any()) return false;

			var toremove = this.Where(req => req.IsSupersetOf(requirements)).ToArray();
			foreach (var e in toremove) Remove(e);

			Add(requirements);

			return true;
		}

		internal void Set(SCBitFlags requirements)
		{
			Clear();
			Add(requirements);
		}

		public override string ToString()
		{
			if (Count == 1) return this[0].ToString("X");
			if (Count > 1) return this[0].ToString("X") + ":" + Count.ToString();
			return "Nope";
		}

		public bool Merge(SCBitFlagSet bitFlagSet)
		{
			bool result = false;
			foreach (var fl in bitFlagSet)
			{
				result |= Merge(fl);
			}

			return result;
		}

		public bool Merge(SCBitFlagSet bitFlagSet, SCBitFlagSet requirements)
		{
			bool result = false;
			foreach (var req in requirements)
				foreach (var fl in bitFlagSet)
				{
					result |= Merge(fl | req);
				}

			return result;
		}

		public List<AccessRequirement> ToRequirements()
		{
			List<AccessRequirement> flags = new List<AccessRequirement>();

			foreach (var bitflags in this)
			{
				flags.Add(bitflags.ToRequirements());
			}

			return flags;
		}

		public static SCBitFlagSet NoRequirements { get; } = new SCBitFlagSet(SCBitFlags.None);
	}

	public class SCBitFlagSetEqualityComparer : IEqualityComparer<SCBitFlagSet>
	{
		public bool Equals(SCBitFlagSet x, SCBitFlagSet y)
		{
			if (x.Count != y.Count) return false;

			for (int i = 0; i < x.Count; i++) if (x[i] != y[i]) return false;

			return true;
		}

		public int GetHashCode([DisallowNull] SCBitFlagSet obj)
		{
			uint result = 0;
			for (int i = 0; i < obj.Count; i++)
			{
				result = (result ^ (uint)obj[i]);
				result = (result >> 13) | (result << (32 - 13));
			}

			return unchecked((int)result);
		}
	}

	public static class SCBitFlagsExtensions
	{
		public static bool IsStrictSupersetOf(this SCBitFlags left, SCBitFlags right)
		{
			var ul = (ushort)left & 0x1FFF;
			var ur = (ushort)right & 0x1FFF;
			return (ur & ul) == ur && ul != ur;
		}

		public static bool IsStrictSubsetOf(this SCBitFlags left, SCBitFlags right)
		{
			var ul = (ushort)left & 0x1FFF;
			var ur = (ushort)right & 0x1FFF;
			return (ur & ul) == ul && ul != ur;
		}

		public static bool IsSupersetOf(this SCBitFlags left, SCBitFlags right)
		{
			var ul = (ushort)left & 0x1FFF;
			var ur = (ushort)right & 0x1FFF;
			return (ur & ul) == ur;
		}

		public static bool IsSubsetOf(this SCBitFlags left, SCBitFlags right)
		{
			var ul = (ushort)left & 0x1FFF;
			var ur = (ushort)right & 0x1FFF;
			return (ur & ul) == ul;
		}

		public static bool IsOrthogonalTo(this SCBitFlags left, SCBitFlags right)
		{
			return !left.IsSupersetOf(right) && !right.IsSupersetOf(left);
		}

		public static bool IsEqual(this SCBitFlags left, SCBitFlags right)
		{
			return ((ushort)left & 0x1FFF) == ((ushort)right & 0x1FFF);
		}

		public static bool IsDone(this SCBitFlags left)
		{
			return (left & SCBitFlags.Done) > 0;
		}

		public static bool IsBlocked(this SCBitFlags left)
		{
			return (left & SCBitFlags.Blocked) > 0;
		}

		public static bool IsImpassable(this SCBitFlags left)
		{
			return (left & SCBitFlags.Impassable) > 0;
		}

		public static AccessRequirement ToRequirements(this SCBitFlags left)
		{
			AccessRequirement flags = AccessRequirement.None;

			if ((left & SCBitFlags.Lute) > 0) flags |= AccessRequirement.Lute;
			if ((left & SCBitFlags.Crown) > 0) flags |= AccessRequirement.Crown;
			if ((left & SCBitFlags.Key) > 0) flags |= AccessRequirement.Key;
			if ((left & SCBitFlags.Ruby) > 0) flags |= AccessRequirement.Ruby;
			if ((left & SCBitFlags.Rod) > 0) flags |= AccessRequirement.Rod;
			if ((left & SCBitFlags.Cube) > 0) flags |= AccessRequirement.Cube;
			if ((left & SCBitFlags.Oxyale) > 0) flags |= AccessRequirement.Oxyale;
			if ((left & SCBitFlags.Orbs) > 0) flags |= AccessRequirement.EarthOrb | AccessRequirement.FireOrb | AccessRequirement.WaterOrb | AccessRequirement.AirOrb;

			return flags;
		}

		public static bool IsAccessible(this List<AccessRequirement> flags, AccessRequirement req)
		{
			foreach (var flag in flags)
			{
				if ((flag & req) == flag) return true;
			}

			return false;
		}
	}
}
