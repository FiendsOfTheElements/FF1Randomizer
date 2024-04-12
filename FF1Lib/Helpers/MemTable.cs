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
		public MemTable(FF1Rom _rom, int _bank, int _address, int _count)
		{
			rom = _rom;
			address = _bank * 0x4000 + (_address - 0x8000);
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

		public T this[int idx]
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

		public int Count {
		    get { return count; }
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
