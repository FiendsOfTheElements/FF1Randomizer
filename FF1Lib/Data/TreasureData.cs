namespace FF1Lib.Data
{
	public class TreasureData : MemTable<Item>
	{
		public TreasureData(FF1Rom _rom) : base(_rom, 0x3100, 256)
		{

		}
	}
}
