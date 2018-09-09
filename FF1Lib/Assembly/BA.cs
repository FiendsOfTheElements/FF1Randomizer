using System;
using System.Collections.Generic;
using System.Text;

namespace FF1Lib.Assembly
{
	/// <summary>
	/// Bank and Address.
	/// </summary>
	public struct BA
	{
		// these should be ushorts of course, but it makes everything annoying (casts everywhere)
		public int bank;
		public int addr;
		public BA(int bank, int addr)
		{
			this.bank = bank;
			this.addr = addr;
		}

		public int MMC3RomLocation()
		{
			return bank * 0x4000 + (addr - (bank == 0x1F ? 0xC000 : 0x8000));
		}

		public static BA FromMMC1RomLocation(int romOffset)
		{
			return _fromRomLocation(0x0F, romOffset);
		}

		public static BA FromMMC3RomLocation(int romOffset)
		{
			return _fromRomLocation(0x1F, romOffset);
		}

		private static BA _fromRomLocation(int lastBank, int romOffset)
		{
			int bank, addr;
			if (romOffset >= 0x7C000)
			{
				bank = lastBank;
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
