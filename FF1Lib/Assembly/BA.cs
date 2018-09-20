using System;
using System.Collections.Generic;
using System.Text;

namespace FF1Lib.Assembly
{
	public enum MemoryMode
	{
		MMC1 = 1,
		MMC3 = 3
	};

	/// <summary>
	/// Bank and (run) Address.
	/// </summary>
	public struct BA
	{
		// apparently I can put statics in a struct???
		/// <summary>
		/// Adjusts last bank based on mapper in use. Change this when the ROM changes.
		/// </summary>
		public static MemoryMode MemoryMode = MemoryMode.MMC1;

		// these should be ushorts of course, but it makes everything annoying (casts everywhere)
		public readonly int Bank;
		public readonly int Addr;
		public BA(int bank, int addr)
		{
			Bank = bank;
			Addr = addr;
		}

		public int ToRomLocation()
		{
			return Bank * 0x4000 + (Addr - (Bank == LastBank() ? 0xC000 : 0x8000));
		}

		public static int TopOfBank(int bank)
		{
			return bank == LastBank() ? 0xFFFF : 0xBFFF;
		}

		public static int LastBank()
		{
			switch (MemoryMode)
			{
				case MemoryMode.MMC1:
					return 0x0F;
				case MemoryMode.MMC3:
					return 0x1F;
				default:
					throw new Exception("weird memoryMode value");
			}
		}

		public static BA FromRomLocation(int romOffset)
		{
			int bank, addr;
			if (romOffset >= 0x7C000)
			{
				bank = LastBank();
				addr = romOffset - (0x7C000 - 0xC000);
			}
			else
			{
				bank = romOffset / 0x4000;
				addr = romOffset % 0x4000 + 0x8000;
			}
			return new BA(bank, addr);
		}

	}

}
