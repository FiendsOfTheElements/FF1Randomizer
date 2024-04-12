using System.Diagnostics.CodeAnalysis;

namespace FF1Lib.Sanity
{
	public class SCTeleport
	{
		public SCCoords Coords { get; set; }

		public SCPointOfInterestType Type { get; set; }

		public MapIndex TargetMap { get; set; }

		public SCCoords TargetCoords { get; set; }

		public SCBitFlagSet BitFlagSet { get; set; }

		public OverworldTeleportIndex OverworldTeleport { get; set; }

		public SCTeleport()
		{
		}

		public SCTeleport(TeleData t, SCPointOfInterestType type)
		{
			Type = type;
			TargetMap = t.Map;
			TargetCoords = new SCCoords(t.X, t.Y).SmClamp;
		}

		public override string ToString()
		{
			if (Type == SCPointOfInterestType.Warp)
			{
				return Coords.ToString() + "->Warp";
			}
			else if(Type == SCPointOfInterestType.Exit)
			{
				return Coords.ToString() + "->Exit";
			}
			{
				return Coords.ToString() + "->" + TargetMap.ToString() + TargetCoords.ToString();
			}
		}
	}

	public class SCTEleportTargetEqualityComparer : IEqualityComparer<SCTeleport>
	{
		SCCoordsEqualityComparer cec = new SCCoordsEqualityComparer();

		public bool Equals(SCTeleport x, SCTeleport y)
		{
			return x.TargetMap == y.TargetMap && cec.Equals(x.TargetCoords, y.TargetCoords);
		}

		public int GetHashCode([DisallowNull] SCTeleport obj)
		{
			return ((int)obj.TargetMap) * 4096 + cec.GetHashCode(obj.TargetCoords);
		}
	}
}
