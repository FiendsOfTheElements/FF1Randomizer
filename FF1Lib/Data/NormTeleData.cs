using System.Collections;

namespace FF1Lib
{
	public struct TeleData
	{
	    public TeleData(MapId m, byte x, byte y) {
		Map = m;
		X = x;
		Y = y;
	    }
		public MapId Map;
		public byte X;
		public byte Y;

		public void FlipXcoordinate()
		{
			var x1 = X & 0x3F;
			var x2 = X & 0xC0;

			X = (byte)((64 - x1 - 1) | x2);
		}

		public void FlipYcoordinate()
		{
			var y1 = Y & 0x3F;
			var y2 = Y & 0xC0;

			Y = (byte)((64 - y1 - 1) | y2);
		}
	}

	public class NormTeleData : IEnumerable<TeleData>
	{
		FF1Rom rom;
		MemTable<byte> TeleX;
		MemTable<byte> TeleY;
		MemTable<MapId> TeleMap;

		public NormTeleData(FF1Rom _rom)
		{
			rom = _rom;

			TeleX = new MemTable<byte>(rom, 0x3F000, 256);
			TeleY = new MemTable<byte>(rom, 0x3F100, 256);
			TeleMap = new MemTable<MapId>(rom, 0x3F200, 256);
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
			for (int i = 0; i < 256; i++) yield return this[i];
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			for (int i = 0; i < 256; i++) yield return this[i];
		}
	}
}
