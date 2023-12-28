using System.ComponentModel;

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
				rom.PutInBank(0x11, 0xB410, Blob.FromHex($"A011845885616920C9{ItemEnd}B027AAC908F019C932F018C935F011E936A8BD0060C963B0517900B49D00608AFE0060C931B037903FA561C9B0900920B9EC208EB44C78B4C96C900920B9EC20EADD4C78B4C944B0092034DDB01CA9E590072046DDB013A9BD65619D006118A645BD00B6D002E67DE67DA2F09004EEB960E88A60AD006209808D0062A5108588A5118589A9008DD16A8DD26AA2C020E4B4A28020E4B4A24020E4B4A20020E4B420F0B4A5888D7868A5898D7968A9002001B5A9012001B5A9022001B5A9032001B5AD0062297F8D006260BD0161AABD15B54A2ED16A60A9DD48A9D548A98B48A94248A91B4C03FEAAA9DD48A9D548A9DD48A9DA48A91B4C03FEEAEA00010100"));
			}
			else
			{
				rom.PutInBank(0x11, 0xB410, Blob.FromHex($"A011845885616920C9{ItemEnd}B028AAC90CD005DE0060B018C9369011E936A8BD0060C963B0517900B49D00608AFE0060C931B037903FA561C9B0900920B9EC208FB44C79B4C96C900920B9EC20EADD4C79B4C944B0092034DDB01CA9E590072046DDB013A9BD65619D006118A645BD00B6D002E67DE67DA2F09004EEB960E88A60AD006209808D0062A5108588A5118589A9008DD16A8DD26AA2C020E5B4A28020E5B4A24020E5B4A20020E5B420F1B4A5888D7868A5898D7968A9002002B5A9012002B5A9022002B5A9032002B5AD0062297F8D006260BD0161AABD15B54A2ED16A60A9DD48A9D548A98B48A94248A91B4C03FEAAA9DD48A9D548A9DD48A9DA48A91B4C03FEEA00010100"));
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
