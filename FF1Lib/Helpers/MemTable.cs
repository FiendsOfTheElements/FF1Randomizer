using RomUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FF1Lib
{
	public class MemTable<T> where T : unmanaged
	{
		private FF1Rom rom;
		private int address;
		private int count;

		public T[] Data { get; private set; }

		public MemTable(FF1Rom _rom, int _address, int _count)
		{
			rom = _rom;
			address = _address;
			count = _count;

			LoadTable();
		}

		public unsafe void LoadTable()
		{
			Data = new T[count];

			byte[] buffer = rom.Get(address, count * sizeof(T));

			fixed (byte* p = buffer)
			{
				T* pBuffer = (T*)p;

				for (int i = 0; i < count; i++)
				{
					Data[i] = pBuffer[i];
				}
			}
		}

		public unsafe void StoreTable()
		{
			byte[] buffer = new byte[count * sizeof(T)];

			fixed (byte* p = buffer)
			{
				T* pBuffer = (T*)p;

				for (int i = 0; i < count; i++)
				{
					pBuffer[i] = Data[i];
				}
			}

			rom.Put(address, buffer);
		}
	}

	public class MemTable<T, I> : MemTable<T> where T : unmanaged where I : Enum
	{
		public MemTable(FF1Rom _rom, int _address, int _count) : base (_rom, _address, _count)
		{
		}

		public T this[I idx]
		{
			get
			{
				return Data[Convert.ToInt32(idx)];
			}
			set
			{
				Data[Convert.ToInt32(idx)] = value;
			}
		}
	}
}
