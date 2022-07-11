﻿using System.Diagnostics.CodeAnalysis;
using static FF1Lib.FF1Rom;

namespace FF1Lib.Sanity
{
	public class SCPointOfInterest
	{
		public MapId MapId { get; set; }

		public SCCoords Coords { get; set; }

		public SCPointOfInterestType Type { get; set; }

		public byte TileId { get; set; }

		public SCTileDef TileDef { get; set; }

		public Item ItemId { get; set; }

		public byte ShopId { get; set; }

		public byte TreasureId { get; set; }

		public SCTeleport Teleport { get; set; }

		public NPC Npc { get; set; }

		public newTalkRoutines TalkRoutine { get; set; }

		public byte[] TalkArray { get; set; }

		public SCBitFlagSet BitFlagSet { get;  set; }

		public bool Done { get; set; }

		public byte DungeonIndex { get; set; }

		public SCPointOfInterest Clone()
		{
			return new SCPointOfInterest
			{
				MapId = MapId,
				Coords = Coords,
				Type = Type,
				ItemId = ItemId,
				ShopId = ShopId,
				TreasureId = TreasureId,
				Teleport = Teleport,
				Npc = Npc,
				TalkRoutine = TalkRoutine,
				TalkArray = TalkArray
			};
		}

		public override string ToString()
		{
			return Type.ToString() + " " + Coords.ToString() + " - " + (BitFlagSet?.ToString() ?? "") + (Done ? " - Done" : "");
		}

		public SCRequirementsSet Requirements
		{
			get
			{
				return new SCRequirementsSet(BitFlagSet.Select(f => (SCRequirements)((ushort)f & 0x1FFF)));
			}
		}
	}

	public class SCPointOfInterestEqualityComparer : IEqualityComparer<SCPointOfInterest>
	{
		SCCoordsEqualityComparer cec = new SCCoordsEqualityComparer();

		public bool Equals(SCPointOfInterest x, SCPointOfInterest y)
		{
			return x.Type == y.Type && cec.Equals(x.Coords, y.Coords);
		}

		public int GetHashCode([DisallowNull] SCPointOfInterest obj)
		{
			return (int)obj.Type * 65536 + cec.GetHashCode(obj.Coords);
		}
	}

	public class SCPointOfInterestIdentityComparer : IEqualityComparer<SCPointOfInterest>
	{
		SCPointOfInterestEqualityComparer cec = new SCPointOfInterestEqualityComparer();

		public bool Equals(SCPointOfInterest x, SCPointOfInterest y)
		{
			return x.MapId == y.MapId && cec.Equals(x, y);
		}

		public int GetHashCode([DisallowNull] SCPointOfInterest obj)
		{
			return (int)obj.MapId * 524288 + cec.GetHashCode(obj);
		}
	}
}
