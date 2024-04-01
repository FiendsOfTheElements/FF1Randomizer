using FF1Lib.Sanity;

namespace FF1Lib
{
	public class ShipLocations
	{
		ShipLocation[] data;

		OwLocationData locations;

		public ShipLocations(OwLocationData _locations, ShipLocation[] _data)
		{
			locations = _locations;
			data = _data;
		}

		
		public SCCoords SetShipLocation(int dungeonIndex)
		{
			locations.LoadData();

			var shiplocation = data.FirstOrDefault(s => s.TeleporterIndex == dungeonIndex);

			if (shiplocation == null) shiplocation = data.FirstOrDefault(s => s.TeleporterIndex == 255);

			if (shiplocation != null)
			{
				locations.ShipLocation = new SCCoords(shiplocation.X, shiplocation.Y);
			}

			locations.StoreData();

			return locations.ShipLocation;
		}
		public ShipLocation GetShipLocation(int dungeonIndex)
		{
			var shiplocation = data.FirstOrDefault(s => s.TeleporterIndex == dungeonIndex);

			if (shiplocation == null) shiplocation = data.FirstOrDefault(s => s.TeleporterIndex == 255);

			return shiplocation;
		}
		public void UpdateDocks(Settings settings)
		{
			if(settings.)
		}
	}
}
