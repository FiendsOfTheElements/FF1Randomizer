﻿using FF1Lib.Sanity;

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
			SetShipLocation(255);
		}
		public SCCoords SetShipLocation(int dungeonIndex)
		{
			//locations.LoadData();

			var shiplocation = data.FirstOrDefault(s => s.TeleporterIndex == dungeonIndex);

			if (shiplocation == null) shiplocation = data.FirstOrDefault(s => s.TeleporterIndex == 255);

			if (shiplocation != null)
			{
				locations.ShipLocation = new SCCoords(shiplocation.X, shiplocation.Y);
			}

			//locations.StoreData();

			return locations.ShipLocation;
		}
		public ShipLocation GetShipLocation(int dungeonIndex)
		{
			var shiplocation = data.FirstOrDefault(s => s.TeleporterIndex == dungeonIndex);

			if (shiplocation == null) shiplocation = data.FirstOrDefault(s => s.TeleporterIndex == 255);

			return shiplocation;
		}
		public void UpdateDocks(Flags flags)
		{
			// Ships found at Matoya's need to spawn at Coneria when Matoya's Dock no longer exists
			if ((bool)flags.MapBridgeLefein && flags.OwMapExchange == OwMapExchanges.None)
			{
				var changeDockLocation = GetShipLocation((int)OverworldTeleportIndex.MatoyasCave);
				if (changeDockLocation.TeleporterIndex != 255)
				{
					changeDockLocation.X = Dock.Coneria[0];
					changeDockLocation.Y = Dock.Coneria[1];
				}
			}
		}
	}
}
