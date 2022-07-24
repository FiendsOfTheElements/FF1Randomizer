using System.Diagnostics.CodeAnalysis;

namespace FF1Lib.Sanity
{
	public class SCBitFlagSet : List<SCBitFlags>
	{
		public SCBitFlagSet() : base() { }

		public SCBitFlagSet(params SCBitFlags[] values) : base(values) { }

		public SCBitFlagSet(IEnumerable<SCBitFlags> values) : base(values) { }

		public SCBitFlagSet Restrict(SCBitFlags flags)
		{
			return new SCBitFlagSet(this.Select(f => f | flags));
		}

		public SCBitFlagSet Restrict(SCBitFlagSet flags)
		{
			return new SCBitFlagSet(this.Select(f1 => flags.Select(f2 => f1 | f2)).SelectMany(x => x));
		}

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
			if (Count == 0) return "Nope";

			return string.Join(" OR ", this.Select(b => GetInnerRule(b)));
		}

		private string GetInnerRule(SCBitFlags b)
		{
			var list = new List<string>();

			if (b.HasFlag(SCBitFlags.Key)) list.Add(Item.Key.ToString());
			if (b.HasFlag(SCBitFlags.Rod)) list.Add(Item.Rod.ToString());
			if (b.HasFlag(SCBitFlags.Oxyale)) list.Add(Item.Oxyale.ToString());
			if (b.HasFlag(SCBitFlags.Cube)) list.Add(Item.Cube.ToString());
			if (b.HasFlag(SCBitFlags.Lute)) list.Add(Item.Lute.ToString());
			if (b.HasFlag(SCBitFlags.Crown)) list.Add(Item.Crown.ToString());
			if (b.HasFlag(SCBitFlags.Ruby)) list.Add(Item.Ruby.ToString());
			if (b.HasFlag(SCBitFlags.Orbs)) list.Add("BlackOrb");
			if (b.HasFlag(SCBitFlags.Tnt)) list.Add(Item.Tnt.ToString());
			if (b.HasFlag(SCBitFlags.Chime)) list.Add(Item.Chime.ToString());
			if (b.HasFlag(SCBitFlags.Canoe)) list.Add(Item.Canoe.ToString());
			if (b.HasFlag(SCBitFlags.Floater)) list.Add(Item.Floater.ToString());

			return "(" + string.Join(" AND ", list) + ")";
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

		public bool IsAccessible(AccessRequirement req, MapChange chg)
		{
			SCBitFlags v2req = SCBitFlags.None;
			if (req.HasFlag(AccessRequirement.Key)) v2req |= SCBitFlags.Key;
			if (req.HasFlag(AccessRequirement.Rod)) v2req |= SCBitFlags.Rod;
			if (req.HasFlag(AccessRequirement.Oxyale)) v2req |= SCBitFlags.Oxyale;
			if (req.HasFlag(AccessRequirement.Cube)) v2req |= SCBitFlags.Cube;
			if (req.HasFlag(AccessRequirement.Lute)) v2req |= SCBitFlags.Lute;
			if (req.HasFlag(AccessRequirement.Crown)) v2req |= SCBitFlags.Crown;
			if (req.HasFlag(AccessRequirement.Ruby)) v2req |= SCBitFlags.Ruby;
			if (req.HasFlag(AccessRequirement.BlackOrb)) v2req |= SCBitFlags.Orbs;
			if (req.HasFlag(AccessRequirement.Tnt)) v2req |= SCBitFlags.Tnt;

			if (chg.HasFlag(MapChange.Chime)) v2req |= SCBitFlags.Chime;
			if (chg.HasFlag(MapChange.Canoe)) v2req |= SCBitFlags.Canoe;
			if (chg.HasFlag(MapChange.Airship)) v2req |= SCBitFlags.Floater;

			foreach (var flag in this)
			{
				if (v2req.IsSupersetOf(flag)) return true;
			}

			return false;
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
	}
}
