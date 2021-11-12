using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FF1Lib.Sanity
{
	public class SCRequirementsSet : List<SCRequirements>
	{
		public SCRequirementsSet() : base() { }

		public SCRequirementsSet(params SCRequirements[] values) : base(values) { }

		public SCRequirementsSet(IEnumerable<SCRequirements> values) : base(values) { }

		public SCRequirementsSet(AccessRequirement accessRequirement)
		{
			var requirement = SCRequirements.None;
			if (accessRequirement.HasFlag(AccessRequirement.Key)) requirement |= SCRequirements.Key;
			if (accessRequirement.HasFlag(AccessRequirement.Rod)) requirement |= SCRequirements.Rod;
			if (accessRequirement.HasFlag(AccessRequirement.Oxyale)) requirement |= SCRequirements.Oxyale;
			if (accessRequirement.HasFlag(AccessRequirement.Cube)) requirement |= SCRequirements.Cube;
			if (accessRequirement.HasFlag(AccessRequirement.Bottle)) requirement |= SCRequirements.Bottle;
			if (accessRequirement.HasFlag(AccessRequirement.Lute)) requirement |= SCRequirements.Lute;
			if (accessRequirement.HasFlag(AccessRequirement.Crown)) requirement |= SCRequirements.Crown;
			if (accessRequirement.HasFlag(AccessRequirement.Crystal)) requirement |= SCRequirements.Crystal;
			if (accessRequirement.HasFlag(AccessRequirement.Herb)) requirement |= SCRequirements.Herb;
			if (accessRequirement.HasFlag(AccessRequirement.Tnt)) requirement |= SCRequirements.Tnt;
			if (accessRequirement.HasFlag(AccessRequirement.Adamant)) requirement |= SCRequirements.Adamant;
			if (accessRequirement.HasFlag(AccessRequirement.Slab)) requirement |= SCRequirements.Slab;
			if (accessRequirement.HasFlag(AccessRequirement.Ruby)) requirement |= SCRequirements.Ruby;

			Add(requirement);
		}

		public SCRequirementsSet Restrict(SCRequirements flags)
		{
			return new SCRequirementsSet(this.Select(f => f | flags));
		}

		public SCRequirementsSet Restrict(SCRequirementsSet flags)
		{
			return new SCRequirementsSet(this.Select(f1 => flags.Select(f2 => f1 | f2)).SelectMany(x => x));
		}

		public bool Merge(SCRequirements requirements)
		{
			if (this.Where(req => req.IsSubsetOf(requirements)).Any()) return false;

			var toremove = this.Where(req => req.IsSupersetOf(requirements)).ToArray();
			foreach (var e in toremove) Remove(e);

			Add(requirements);

			return true;
		}

		internal void Set(SCRequirements requirements)
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

		public bool Merge(SCRequirementsSet bitFlagSet)
		{
			bool result = false;
			foreach (var fl in bitFlagSet)
			{
				result |= Merge(fl);
			}

			return result;
		}

		public bool Merge(SCRequirementsSet bitFlagSet, SCRequirementsSet requirements)
		{
			bool result = false;
			foreach (var req in requirements)
				foreach (var fl in bitFlagSet)
				{
					result |= Merge(fl | req);
				}

			return result;
		}

		public SCRequirementsSet Ease(SCRequirements requirements)
		{
			var and = 0xFFFFFFFF - (int)requirements;
			return new SCRequirementsSet(this.Select(f => (SCRequirements)((int)f & and)));
		}

		public static SCRequirementsSet NoRequirements { get; } = new SCRequirementsSet(SCRequirements.None);
	}


	public static class SCRequirementsExtensions
	{
		public static bool IsStrictSupersetOf(this SCRequirements left, SCRequirements right)
		{
			return (right & left) == right && left != right;
		}

		public static bool IsStrictSubsetOf(this SCRequirements left, SCRequirements right)
		{
			return (right & left) == left && left != right;
		}

		public static bool IsSupersetOf(this SCRequirements left, SCRequirements right)
		{
			return (right & left) == right;
		}

		public static bool IsSubsetOf(this SCRequirements left, SCRequirements right)
		{
			return (right & left) == left;
		}

		public static bool IsOrthogonalTo(this SCRequirements left, SCRequirements right)
		{
			return !left.IsSupersetOf(right) && !right.IsSupersetOf(left);
		}

		public static bool IsEqual(this SCRequirements left, SCRequirements right)
		{
			return left == right;
		}
	}
}
