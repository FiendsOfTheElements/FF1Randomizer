using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FF1Lib
{
	public enum ChanceToRunMode
	{
		[Description("Vanilla")]
		Vanilla,

		[Description("Fixed")]
		Fixed,

		[Description("25%")]
		Fix25,

		[Description("40%")]
		Fix40,

		[Description("55%")]
		Fix55,

		[Description("70%")]
		Fix70,

		[Description("85%")]
		Fix85,

		[Description("100%")]
		Fix100
	}

	public class ChanceToRun
	{
		FF1Rom rom;
		Flags flags;

		public ChanceToRun(FF1Rom _rom, Flags _flags)
		{
			rom = _rom;
			flags = _flags;
		}

		public void FixChanceToRun()
		{
			switch (flags.ChanceToRun)
			{
				case ChanceToRunMode.Vanilla:
					break;
				case ChanceToRunMode.Fixed:
					//Load Level instead of whatever the vanilla game loads
					rom.Put(0x323EF, new byte[] { 0x82 });
					break;
				case ChanceToRunMode.Fix25:
					//Load 100 instead of level, the CLC can stay, remove Add 16
					rom.Put(0x323EE, new byte[] { 0xA9, 0x63, 0x18, 0xEA, 0xEA });
					//Load 25 instead of Luck
					rom.Put(0x323FF, new byte[] { 0xA9, 0x19});
					break;
				case ChanceToRunMode.Fix40:
					rom.Put(0x323EE, new byte[] { 0xA9, 0x63, 0x18, 0xEA, 0xEA });
					rom.Put(0x323FF, new byte[] { 0xA9, 0x28 });
					break;
				case ChanceToRunMode.Fix55:
					rom.Put(0x323EE, new byte[] { 0xA9, 0x63, 0x18, 0xEA, 0xEA });
					rom.Put(0x323FF, new byte[] { 0xA9, 0x37 });
					break;
				case ChanceToRunMode.Fix70:
					rom.Put(0x323EE, new byte[] { 0xA9, 0x63, 0x18, 0xEA, 0xEA });
					rom.Put(0x323FF, new byte[] { 0xA9, 0x46 });
					break;
				case ChanceToRunMode.Fix85:
					rom.Put(0x323EE, new byte[] { 0xA9, 0x63, 0x18, 0xEA, 0xEA });
					rom.Put(0x323FF, new byte[] { 0xA9, 0x55 });
					break;
				case ChanceToRunMode.Fix100:
					rom.Put(0x323EE, new byte[] { 0xA9, 0x63, 0x18, 0xEA, 0xEA });
					rom.Put(0x323FF, new byte[] { 0xA9, 0x64 });
					break;

			}

		}
	}
}
