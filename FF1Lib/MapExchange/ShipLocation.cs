namespace FF1Lib
{
	public class ShipLocation
	{
	    public ShipLocation() { }

	    public ShipLocation(byte x, byte y, byte teleporterIndex) {
		X = x;
		Y = y;
		TeleporterIndex = teleporterIndex;
	    }
		public byte TeleporterIndex { get; set; }

		public byte X { get; set; }

		public byte Y { get; set; }
	}
}
