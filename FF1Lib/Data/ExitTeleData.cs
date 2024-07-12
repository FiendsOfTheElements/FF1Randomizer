using System.Collections;

namespace FF1Lib
{
	public class ExitTeleData : IEnumerable<TeleData>
	{
		FF1Rom rom;
		MemTable<byte> TeleX;
		MemTable<byte> TeleY;

		public ExitTeleData(FF1Rom _rom)
		{
			rom = _rom;

			TeleX = new MemTable<byte>(rom, 0x2C60, 16);
			TeleY = new MemTable<byte>(rom, 0x2C70, 16);
		}

		public void LoadData()
		{
			TeleX.LoadTable();
			TeleY.LoadTable();
		}

		public void StoreData()
		{
			TeleX.StoreTable();
			TeleY.StoreTable();
		}

		public TeleData this[int i]
		{
			get
			{
				return new TeleData { X = TeleX[i], Y = TeleY[i], Map = (MapIndex)0xFF };
			}
			set
			{
				TeleX[i] = value.X;
				TeleY[i] = value.Y;
			}
		}

		public IEnumerator<TeleData> GetEnumerator()
		{
			for (int i = 0; i < 16; i++) yield return this[i];
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			for (int i = 0; i < 16; i++) yield return this[i];
		}
	}
}
