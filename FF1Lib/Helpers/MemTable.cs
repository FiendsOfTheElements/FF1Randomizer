using RomUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FF1Lib
{
	public class MemTable
	{
		private FF1Rom rom;
		private int address;
		private int count;
		private int size;

		public List<Blob> Table { get; private set; }

		public MemTable(FF1Rom _rom, int _address, int _count, int _size)
		{
			rom = _rom;
			address = _address;
			count = _count;
			size = _size;

			LoadTable();
		}

		public void LoadTable()
		{
			Table = rom.Get(address, count).Chunk(size);
		}

		public void StoreTable()
		{
			rom.Put(address, Blob.Concat(Table));
		}
	}

	public class MemTable<T> : MemTable where T : Enum
	{
		public MemTable(FF1Rom _rom, int _address, int _count, int _size) : base (_rom, _address, _count, _size)
		{
		}

		public Blob this[T idx]
		{
			get
			{
				return Table[Convert.ToInt32(idx)];
			}
			set
			{
				Table[Convert.ToInt32(idx)] = value;
			}
		}
	}
}
