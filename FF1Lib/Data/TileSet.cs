using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace FF1Lib
{
	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public struct TileProp
	{
		public byte Byte1;
		public byte Byte2;

		public byte ShopId
		{
			get { return Byte2; }
			set { Byte2 = value; }
		}

		public TileProp(byte p1, byte p2)
		{
			Byte1 = p1;
			Byte2 = p2;
		}
	}

	public class TileSet
	{
		public MemTable<Item> TileProperties;
		public MemTable<Item> TileAttributes;
		public MemTable<Item> TopLeftTiles;
		public MemTable<Item> TopRightTiles;
		public MemTable<Item> BottemLeftTiles;
		public MemTable<Item> BottemRightTiles;

		public TileSet(FF1Rom _rom, byte idx)
		{
			TileProperties = new MemTable<Item>(_rom, 0x800 + 0x100 * idx, 128, 1);
			TileAttributes = new MemTable<Item>(_rom, 0x400 + 0x80 * idx, 128, 1);

			TopLeftTiles = new MemTable<Item>(_rom, 0x1000 + 0x200 * idx, 128, 1);
			TopRightTiles = new MemTable<Item>(_rom, 0x1080 + 0x200 * idx, 128, 1);
			BottemLeftTiles = new MemTable<Item>(_rom, 0x1100 + 0x200 * idx, 128, 1);
			BottemRightTiles = new MemTable<Item>(_rom, 0x1180 + 0x200 * idx, 128, 1);
		}

		public void LoadData()
		{
			TileProperties.LoadTable();
			TileAttributes.LoadTable();
			TopLeftTiles.LoadTable();
			TopRightTiles.LoadTable();
			BottemLeftTiles.LoadTable();
			BottemRightTiles.LoadTable();
		}

		public void StoreData()
		{
			TileProperties.StoreTable();
			TileAttributes.StoreTable();
			TopLeftTiles.StoreTable();
			TopRightTiles.StoreTable();
			BottemLeftTiles.StoreTable();
			BottemRightTiles.StoreTable();
		}
	}
}
