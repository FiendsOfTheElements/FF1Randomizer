using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FF1Lib
{
	public struct TeleData
	{
		public MapId Map;
		public byte X;
		public byte Y;
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

			TeleX = new MemTable<byte>(rom, 0x2D00, 64);
			TeleY = new MemTable<byte>(rom, 0x2D40, 64);
			TeleMap = new MemTable<MapId>(rom, 0x2D80, 64);
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
			for (int i = 0; i < 64; i++) yield return this[i];
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			for (int i = 0; i < 64; i++) yield return this[i];
		}
	}
}
