namespace FF1Lib.Sanity
{
	public struct SCOwTile
	{
		public SCBitFlags Tile;

		public short Area;

		public SCOwTile(SCBitFlags tile)
		{
			Tile = tile;
			Area = -1;
		}
	}
}
