using RomUtilities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FF1Lib
{
	public enum TreasureStackSize
	{
		[Description("Default")]
		Default,

		[Description("Stingy")]
		Stingy,

		[Description("Adequat")]
		Adequat,

		[Description("Generous")]
		Generous,

		[Description("Ridiculous")]
		Ridiculous
	}

	public class TreasureStacks
	{
		FF1Rom rom;
		Flags flags;

		public TreasureStacks(FF1Rom _rom, Flags _flags)
		{
			rom = _rom;
			flags = _flags;
		}

		public void SetTreasureStacks()
		{
			rom.PutInBank(0x1F, 0xDD78, Blob.FromHex("A9002003FEA645BC00B1A9112003FE982006B0B00AA445B9006209049900628A60"));
			rom.PutInBank(0x11, 0xB000, GetStackArray());
			rom.PutInBank(0x11, 0xB006, Blob.FromHex("A0118C580085616920C93CB028"));
			rom.PutInBank(0x11, 0xB013, Blob.FromHex("AAC90CD005DE0060B018"));
			rom.PutInBank(0x11, 0xB01D, Blob.FromHex("C9369011"));
			rom.PutInBank(0x11, 0xB021, Blob.FromHex("E936A8BD0060C963B03D"));
			rom.PutInBank(0x11, 0xB02B, Blob.FromHex("7900B09D00608A"));
			rom.PutInBank(0x11, 0xB032, Blob.FromHex("FE0060C931B02A902B"));
			rom.PutInBank(0x11, 0xB03B, Blob.FromHex("A561C96C900920B9EC20EADD4C63B0C944B0092034DDB007A9E590072046DDB00CA9BD65619D006118E67DE67DA2F09004EEB960E88A60"));
		}

		private Blob GetStackArray()
		{
			switch (flags.ConsumableTreasureStackSize)
			{
				case TreasureStackSize.Stingy:     return Blob.FromHex("010000040000");
				case TreasureStackSize.Adequat:    return Blob.FromHex("020100090101");
				case TreasureStackSize.Generous:   return Blob.FromHex("040201130201");
				case TreasureStackSize.Ridiculous: return Blob.FromHex("090402310404");
				default:                           return Blob.FromHex("000000000000");
			}
		}
	}
}
