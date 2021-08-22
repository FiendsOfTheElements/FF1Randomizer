using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FF1Lib
{
	public class ShipLocation
	{
	    public ShipLocation(int x, int y, int teleporterIndex) {
		X = x;
		Y = y;
		TeleporterIndex = teleporterIndex;
	    }
		public byte TeleporterIndex { get; set; }

		public byte X { get; set; }

		public byte Y { get; set; }
	}
}
