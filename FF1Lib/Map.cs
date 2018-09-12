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

		public List<RomUtilities.Blob> Get(int y, int x, int width, int height)
		{
			List<RomUtilities.Blob> rows = new List<RomUtilities.Blob>();
			for (int i = y; i < y + height; ++i)
			{
				var row = new List<byte>();
				for (int j = x; j < x + width; ++j)
				{
					row.Add(_map[i, j]);
				}
				rows.Add(row.ToArray());
			}

			return rows;
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

		public Map(byte fill)
		{
			_map = new byte[RowCount, RowLength];
			for (int y = 0; y < RowCount; ++y)
			{
				for (int x = 0; x < RowLength; ++x)
				{
					_map[y, x] = fill;
				}
			}
		}

		public Map Clone()
		{
			Map map = new Map(0);
			for (int y = 0; y < RowCount; ++y)
			{
				for (int x = 0; x < RowLength; ++x)
				{
					map[y, x] = _map[y, x];
				}
			}
			return map;
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
