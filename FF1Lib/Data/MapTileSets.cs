namespace FF1Lib
{
	public class MapTileSets : MemTable<byte, MapId>
	{
		public MapTileSets(FF1Rom _rom) : base(_rom, 0x2CC0, 64)
		{

		}
	}
}
