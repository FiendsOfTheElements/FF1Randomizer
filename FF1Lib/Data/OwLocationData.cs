using FF1Lib.Sanity;
using RomUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FF1Lib
{
	public class OwLocationData
	{
		public SCCoords StartingLocation { get; set; }

		public SCCoords ShipLocation { get; set; }

		public SCCoords AirShipLocation { get; set; }

		//public SCCoords BridgeLocation { get; set; }

		//public SCCoords CanalLocation { get; set; }

		FF1Rom rom;

		public OwLocationData(FF1Rom _rom)
		{
			rom = _rom;
		}

		public void LoadData()
		{
			StartingLocation = LoadCoords(0x3000 + UnsramIndex.OverworldScrollX);
			ShipLocation = LoadCoords2(0x3000 + UnsramIndex.ShipX);
			AirShipLocation = LoadCoords2(0x3000 + UnsramIndex.AirshipX);
			//BridgeLocation = LoadCoords(0x3000 + UnsramIndex.BridgeX);
			//CanalLocation = LoadCoords(0x3000 + UnsramIndex.CanalX);
		}

		private SCCoords LoadCoords(int address)
		{
			byte[] data = rom.Get(address, 2);
			var X = (((int)data[0] + 7) & 0xFF);
			var Y = (((int)data[1] + 7) & 0xFF);
			return new SCCoords(X, Y);
		}

		private SCCoords LoadCoords2(int address)
		{
			byte[] data = rom.Get(address, 2);
			return new SCCoords(data[0], data[1]);
		}

			public void StoreData()
		{
			StoreCoords(0x3000 + UnsramIndex.OverworldScrollX, StartingLocation);
			StoreCoords2(0x3000 + UnsramIndex.ShipX, ShipLocation);
			//StoreCoords2(0x3000 + UnsramIndex.AirshipX, AirShipLocation);
			//StoreCoords2(0x3000 + UnsramIndex.BridgeX, BridgeLocation);
			//StoreCoords2(0x3000 + UnsramIndex.CanalX, CanalLocation);
		}

		private void StoreCoords(int address, SCCoords coords)
		{
			var X = coords.X;
			var Y = coords.Y;
			rom.Put(address, new byte[] { (byte)(((int)X - 7) & 0xFF), (byte)(((int)Y - 7) & 0xFF) });
		}

		private void StoreCoords2(int address, SCCoords coords)
		{
			rom.Put(address, new byte[] { coords.X, coords.Y });
		}
	}
}
