using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FF1Lib
{
	public class TilePropTable
	{
		MemTable<byte> TileProperties;

		public TileProp[] Data { get; private set; } = new TileProp[128];

		public TilePropTable(FF1Rom _rom, byte idx)
		{
			TileProperties = new MemTable<byte>(_rom, 0x800 + 0x100 * idx, 256);
		}

		public void LoadData()
		{
			TileProperties.LoadTable();

			for (int i = 0; i < 128; i++)
			{
				Data[i] = new TileProp(TileProperties[2 * i], TileProperties[2 * i + 1]);
			}
		}

		public void StoreData()
		{
			for (int i = 0; i < 128; i++)
			{
				TileProperties[2 * i] = Data[i].Byte1;
				TileProperties[2 * i + 1] = Data[i].Byte2;
			}

			TileProperties.StoreTable();
		}

		public TileProp this[int idx]
		{
			get
			{
				return Data[idx];
			}
			set
			{
				Data[idx] = value;
			}
		}
	}
}
