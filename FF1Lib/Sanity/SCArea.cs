using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FF1Lib.Sanity
{
	public class SCArea
	{
		public SCMap Map { get; private set; }

		public List<SCPointOfInterest> PointsOfInterest { get; private set; }

		public List<SCTeleport> Exits { get; private set; }

		public List<SCEntrance> Entrances { get; private set; }

		public SCArea(SCMap _map, List<SCEntrance> _entrances)
		{
			Map = _map;
			Entrances = _entrances;

			ComposeArea();
		}

		private void ComposeArea()
		{
			PointsOfInterest = Entrances
				.SelectMany(e => e.PointsOfInterest)
				.Where(p => p.BitFlagSet.Count > 0)
				.GroupBy(p => p.Coords, new SCCoordsEqualityComparer())
				.Select(g => g.First())
				.ToList();

			Exits = PointsOfInterest.Where(p => p.Type == SCPointOfInterestType.Tele || p.Type == SCPointOfInterestType.Warp || p.Type == SCPointOfInterestType.Exit).Select(p => p.Teleport).ToList();
		}
	}
}
