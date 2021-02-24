using RomUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace FF1Lib
{
	public class MemTable<T> where T : unmanaged
	{
		private readonly FF1Rom rom;
		private readonly int address;
		private readonly int count;

		public T[] Data { get; private set; }

		public MemTable(FF1Rom _rom, int _address, int _count)
		{
			rom = _rom;
			address = _address;
			count = _count;

			LoadTable();
		}

		public void LoadTable()
		{
			Data = new T[count];

			byte[] buffer = rom.Get(address, count * Marshal.SizeOf<T>());

			Buffer.BlockCopy(buffer, 0, Data, 0, buffer.Length);
		}

		public void StoreTable()
		{
			byte[] buffer = new byte[count * Marshal.SizeOf<T>()];

			Buffer.BlockCopy(Data, 0, buffer, 0, buffer.Length);

			rom.Put(address, buffer);
		}

		public T this[int idx]
		{
			get => Data[idx];
			set => Data[idx] = value;
		}
	}

	public class MemTable<T, I> : MemTable<T> where T : unmanaged where I : Enum
	{
		public MemTable(FF1Rom _rom, int _address, int _count) : base(_rom, _address, _count)
		{
		}

		public T this[I idx]
		{
			get => Data[Convert.ToInt32(idx)];
			set => Data[Convert.ToInt32(idx)] = value;
		}
	}
}
