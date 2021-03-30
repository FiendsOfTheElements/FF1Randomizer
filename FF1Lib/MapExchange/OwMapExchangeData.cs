using FF1Lib.Sanity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FF1Lib
{
	public class OwMapExchangeData
	{
		public SCCoords? StartingLocation { get; set; }

		public SCCoords? AirShipLocation { get; set; }

		public ShipLocation[] ShipLocations { get; set; }

		public TeleportFixup[] TeleporterFixups { get; set; }

		public DomainFixup[] DomainFixups { get; set; }

	}
}
