using System.Collections;

namespace FF1Lib
{
	public class EnterTeleData : IEnumerable<TeleData>
	{
		FF1Rom rom;
		MemTable<byte> TeleX;
		MemTable<byte> TeleY;
		MemTable<MapIndex> TeleMap;

		public EnterTeleData(FF1Rom _rom)
		{
			rom = _rom;

			TeleX = new MemTable<byte>(rom, 0x2C00, 32);
			TeleY = new MemTable<byte>(rom, 0x2C20, 32);
			TeleMap = new MemTable<MapIndex>(rom, 0x2C40, 32);
		}

		public void LoadData()
		{
			TeleX.LoadTable();
			TeleY.LoadTable();
			TeleMap.LoadTable();
		}

		public void StoreData()
		{
			TeleX.StoreTable();
			TeleY.StoreTable();
			TeleMap.StoreTable();
		}

		public TeleData this[int i]
		{
			get
			{
				return new TeleData { X = TeleX[i], Y = TeleY[i], Map = TeleMap[i] };
			}
			set
			{
				TeleX[i] = value.X;
				TeleY[i] = value.Y;
				TeleMap[i] = value.Map;
			}
		}

		public IEnumerator<TeleData> GetEnumerator()
		{
			for (int i = 0; i < 32; i++) yield return this[i];
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			for (int i = 0; i < 32; i++) yield return this[i];
		}
	}
}
