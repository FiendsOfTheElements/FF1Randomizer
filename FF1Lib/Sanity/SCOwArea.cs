﻿namespace FF1Lib.Sanity
{
	public class SCOwArea
	{
		public short Index { get; set; }

		public SCBitFlags Tile { get; set; }

		public HashSet<short> Links { get; set; }

		public SCCoords Start { get; set; }

		public List<SCPointOfInterest> PointsOfInterest { get; private set; } = new List<SCPointOfInterest>();

		public SCOwArea(short index, SCBitFlags tile)
		{
			Index = index;
			Tile = tile;
			Links = new HashSet<short>();
		}

		public void AddLink(SCOwArea area)
		{
			//One is Blocked
			if ((Tile & SCBitFlags.Blocked) > 0 || (area.Tile & SCBitFlags.Blocked) > 0) return;

			//One is Ocean
			if ((Tile & SCBitFlags.Ocean) > 0 && (area.Tile & (SCBitFlags.River | SCBitFlags.ShipDock | SCBitFlags.Canal | SCBitFlags.Bridge)) == 0) return;
			if ((area.Tile & SCBitFlags.Ocean) > 0 && (Tile & (SCBitFlags.River | SCBitFlags.ShipDock | SCBitFlags.Canal | SCBitFlags.Bridge)) == 0) return;

			area.Links.Add(Index);
			Links.Add(area.Index);
		}

		public SCRequirements Requirements
		{
			get
			{
				var req = SCRequirements.None;

				if (Tile.HasFlag(SCBitFlags.River)) req |= SCRequirements.Canoe;
				if (Tile.HasFlag(SCBitFlags.Ocean)) req |= SCRequirements.Ship;
				if (Tile.HasFlag(SCBitFlags.Chime)) req |= SCRequirements.Chime;

				return req;
			}
		}
	}
}
