using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FF1Lib
{
	public class Map
	{
		public const int RowLength = 64;
		public const int RowCount = 64;

		private readonly byte[,] _map;

		public byte this[int y, int x]
		{
			get => _map[y, x];
			set => _map[y, x] = value;
		}

		public void Put(int y, int x, RomUtilities.Blob[] rows)
		{
			Array.ForEach(rows, blob =>
			{
				for (int i = 0; i < blob.Length; ++i)
				{
					this[y, x + i] = blob[i];
				}
				++y;
			});
		}

		public Map(byte[] data)
		{
			_map = new byte[RowCount, RowLength];

			int dataOffset = 0;
			int x = 0, y = 0;
			while (y < RowCount)
			{
				byte tile = data[dataOffset++];
				if ((tile & 0x80) != 0)
				{
					tile &= 0x7F;
					int count = data[dataOffset++];
					if (count == 0)
					{
						count = 256;
					}
					for (int j = 0; j < count; j++)
					{
						_map[y, x++] = tile;
						if (x >= RowLength)
						{
							x = 0;
							y++;
						}
					}
				}
				else
				{
					_map[y, x++] = tile;
					if (x >= RowLength)
					{
						x = 0;
						y++;
					}
				}
			}
		}

		public byte[] GetCompressedData()
		{
			var compressedData = new List<byte>();

			var data = new byte[_map.Length];
			Buffer.BlockCopy(_map, 0, data, 0, _map.Length);

			int dataOffset = 0;
			while (dataOffset < data.Length)
			{
				var tile = data[dataOffset++];
				if (dataOffset >= data.Length || data[dataOffset] != tile)
				{
					compressedData.Add(tile);
					continue;
				}

				byte count = 1;
				while (dataOffset < data.Length && count != 0 && data[dataOffset] == tile)
				{
					count++;
					dataOffset++;
				}

				tile |= 0x80;
				compressedData.Add(tile);
				compressedData.Add(count);
			}

			compressedData.Add(0xFF);

			return compressedData.ToArray();
		}
	}
}
