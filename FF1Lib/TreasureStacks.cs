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

		[Description("Stingy (2 Tents, 5 Heals)")]
		Stingy,

		[Description("Adequat (3 Tents, 2 Cabins, 10 Heals, 2 Pures, 2 Softs)")]
		Adequat,

		[Description("Generous (5 Tents, 3 Cabins, 2 Houses, 20 Heals, 3 Pures, 2 Softs))")]
		Generous,

		[Description("Ridiculous (10 Tents, 5 Cabins, 3 Houses, 50 Heals, 5 Pures, 5 Softs))")]
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
			rom.PutInBank(0x11, 0xB006, Blob.FromHex("85616920C93CB028"));
			rom.PutInBank(0x11, 0xB00E, Blob.FromHex("AAC90CD005DE0060B018"));
			rom.PutInBank(0x11, 0xB018, Blob.FromHex("C9369011"));
			rom.PutInBank(0x11, 0xB01C, Blob.FromHex("E936A8BD0060C963B03D"));
			rom.PutInBank(0x11, 0xB026, Blob.FromHex("7900B09D00608A"));
			rom.PutInBank(0x11, 0xB02D, Blob.FromHex("FE0060C931B02A902B"));
			rom.PutInBank(0x11, 0xB036, Blob.FromHex("A561C96C900920B9EC20EADD4CD6DDC944B0092034DDB007A9E590072046DDB00CA9BD65619D006118E67DE67DA2F09004EEB960E88A60"));
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
