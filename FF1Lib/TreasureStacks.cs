﻿using RomUtilities;
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

		[Description("Adequate")]
		Adequat,

		[Description("Generous")]
		Generous,

		[Description("Ridiculous")]
		Ridiculous
	}

	public enum ExtTreasureStackSize
	{
		[Description("Default")]
		Default,

		[Description("Stingy")]
		Stingy,

		[Description("Adequate")]
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
			rom.PutInBank(0x1F, 0xDD78, Blob.FromHex("A9002003FEA645BC00B1A9112003FE982010B0B00AA445B9006209049900628A60"));
			rom.PutInBank(0x11, 0xB000, Blob.Concat(GetStackArray(), GetExtStackArray()));

			if (flags.EnableExtConsumables)
			{
				//C940 instead of C93C
				rom.PutInBank(0x11, 0xB010, Blob.FromHex("A011845885616920C940B028AAC90CD005DE0060B018C9369011E936A8BD0060C963B0517900B09D00608AFE0060C931B037903FA561C9B0900920B9EC208FB04C79B0C96C900920B9EC20EADD4C79B0C944B0092034DDB01CA9E590072046DDB013A9BD65619D006118A645BD00B2D002E67DE67DA2F09004EEB960E88A60AD006209808D0062A5108588A5118589A9008DD16A8DD26AA2C020E5B0A28020E5B0A24020E5B0A20020E5B020F1B0A5888D7868A5898D7968A9002002B1A9012002B1A9022002B1A9032002B1AD0062297F8D006260BD01614A2ED16A4A2ED26A60A9DD48A9D548A98B48A94248A91B4C03FEAAA9DD48A9D548A9DD48A9DA48A91B4C03FE"));
			}
			else
			{
				rom.PutInBank(0x11,0xB010,Blob.FromHex("A011845885616920C93CB028AAC90CD005DE0060B018C9369011E936A8BD0060C963B0517900B09D00608AFE0060C931B037903FA561C9B0900920B9EC208FB04C79B0C96C900920B9EC20EADD4C79B0C944B0092034DDB01CA9E590072046DDB013A9BD65619D006118A645BD00B2D002E67DE67DA2F09004EEB960E88A60AD006209808D0062A5108588A5118589A9008DD16A8DD26AA2C020E5B0A28020E5B0A24020E5B0A20020E5B020F1B0A5888D7868A5898D7968A9002002B1A9012002B1A9022002B1A9032002B1AD0062297F8D006260BD01614A2ED16A4A2ED26A60A9DD48A9D548A98B48A94248A91B4C03FEAAA9DD48A9D548A9DD48A9DA48A91B4C03FE"));
			}

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

		private Blob GetExtStackArray()
		{
			switch (flags.ExtConsumableTreasureStackSize)
			{
				case TreasureStackSize.Stingy: return Blob.FromHex("00000101");
				case TreasureStackSize.Adequat: return Blob.FromHex("01000202");
				case TreasureStackSize.Generous: return Blob.FromHex("02010404");
				case TreasureStackSize.Ridiculous: return Blob.FromHex("04020909");
				default: return Blob.FromHex("00000000");
			}
		}
	}
}
