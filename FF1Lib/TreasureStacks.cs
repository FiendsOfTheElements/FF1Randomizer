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
			rom.PutInBank(0x1F, 0xDD78, Blob.FromHex("A9002003FEA645BC00B1A9112003FE982010B4B00AA445B9006209049900628A60"));
			rom.PutInBank(0x11, 0xB400, Blob.Concat(GetStackArray(), GetExtStackArray()));

			var ItemEnd = "3C";
			if (flags.ExtConsumableSet != ExtConsumableSet.None) ItemEnd = "40";

			if (flags.Archipelago)
			{
				rom.PutInBank(0x11, 0xB410, Blob.FromHex($"A011845885616920C9{ItemEnd}B023AAC908F015C932F014E936A8BD0060C963B0517900B49D00608AFE0060C931B037903FA561C9B0900920B9EC208AB44C74B4C96C900920B9EC20EADD4C74B4C944B0092034DDB01CA9E590072046DDB013A9BD65619D006118A645BD00B6D002E67DE67DA2F09004EEB960E88A60AD006209808D0062A5108588A5118589A9008DD16A8DD26AA2C020E0B4A28020E0B4A24020E0B4A20020E0B420ECB4A5888D7868A5898D7968A90020FDB4A90120FDB4A90220FDB4A90320FDB4AD0062297F8D006260BD01614A2ED16A4A2ED26A60A9DD48A9D548A98B48A94248A91B4C03FEAAA9DD48A9D548A9DD48A9DA48A91B4C03FE"));
			}
			else
			{
				rom.PutInBank(0x11, 0xB410, Blob.FromHex($"A011845885616920C9{ItemEnd}B028AAC90CD005DE0060B018C9369011E936A8BD0060C963B0517900B49D00608AFE0060C931B037903FA561C9B0900920B9EC208FB44C79B4C96C900920B9EC20EADD4C79B4C944B0092034DDB01CA9E590072046DDB013A9BD65619D006118A645BD00B6D002E67DE67DA2F09004EEB960E88A60AD006209808D0062A5108588A5118589A9008DD16A8DD26AA2C020E5B4A28020E5B4A24020E5B4A20020E5B420F1B4A5888D7868A5898D7968A9002002B5A9012002B5A9022002B5A9032002B5AD0062297F8D006260BD01614A2ED16A4A2ED26A60A9DD48A9D548A98B48A94248A91B4C03FEAAA9DD48A9D548A9DD48A9DA48A91B4C03FE"));
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
