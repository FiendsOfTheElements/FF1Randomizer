using System.ComponentModel;

namespace FF1Lib
{
	public enum StartingLevel
	{
		[Description("1")]
		Level01,

		[Description("3")]
		Level03,

		[Description("5")]
		Level05,

		[Description("10")]
		Level10,

		[Description("16")]
		Level16,

		[Description("25")]
		Level25,

		[Description("36")]
		Level36,

		[Description("50")]
		Level50
	}

	public class StartingLevels
	{
		FF1Rom rom;
		Settings settings;

		public StartingLevels(FF1Rom _rom, Settings _settings)
		{
			rom = _rom;
			settings = _settings;
		}

		public void SetStartingLevels()
		{
			//Call NewGame_LoadStartingLevels instead of NewGame_LoadStartingStats
			rom.PutInBank(0x1F, 0xC0B4, Blob.FromHex("209ADD"));

			//NewGame_LoadStartingLevels: call NewGame_LoadStartingStats, LoadStartingEquipment, then call AwardStartingEXP
			rom.PutInBank(0x1F, 0xDD9A, Blob.FromHex("206DC7A91B2003FE204085209684A90E2003FEA2002056B5A2402056B5A2802056B5A2C02056B520F3AB60"));

			//AwardStartingEXP
			rom.PutInBank(0x1B, 0x8496, Blob.FromHex("AD006209808D0062A9108D7668A9FF8D7868A9FF8D7968A90020DA87A90120DA87A90220DA87A90320DA87CE7668D0E7AD0062297F8D0062A200BD0C619D0A61BD0D619D0B61A240BD0C619D0A61BD0D619D0B61A280BD0C619D0A61BD0D619D0B61A2C0BD0C619D0A61BD0D619D0B6160"));

			//Routine to en/disable LvlUp_Display. If the bit 7 of item_lute is set, we are at gamestart and don't want LvlUp_Display to be called
			rom.PutInBank(0x1B, 0x8508, Blob.FromHex("AD00622980D00320E38960"));

			//JSR to the routine above instead of LvlUp_Display
			rom.PutInBank(0x1B, 0x8853, Blob.FromHex("200885"));

			var exp = (int)(GetExp() / settings.GetFloat("ExpMultiplier")) / 16 + 1;

			if (settings.GetInt("StartingLevel") == (int)StartingLevel.Level01) exp = 0;

			//Fill in Exp divided by 16 LowByte
			rom.PutInBank(0x1B, 0x84A4, new byte[] { (byte)(exp & 0xFF) });

			//Fill in Exp divided by 16 HighByte
			rom.PutInBank(0x1B, 0x84A9, new byte[] { (byte)(exp / 0xFF) });
		}

		private double GetExp()
		{
			switch (settings.GetEnum("StartingLevel", typeof(StartingLevel)))
			{
				case StartingLevel.Level01:
					return 0;
				case StartingLevel.Level03:
					return 196;
				case StartingLevel.Level05:
					return 1171;
				case StartingLevel.Level10:
					return 11116;
				case StartingLevel.Level16:
					return 48361;
				case StartingLevel.Level25:
					return 191103;
				case StartingLevel.Level36:
					return 530448;
				case StartingLevel.Level50:
					return 999999;
			}

			return 0;
		}

		//RFM: couldnt get byte, type casted enum to work, brute force method instead. The flag auto encoder had a problem with startinglevel being a byte type.
		public static int GetLevelNumber(StartingLevel startingLevel)
		{
			switch (startingLevel)
			{
				case StartingLevel.Level01:
					return 1;
				case StartingLevel.Level03:
					return 3;
				case StartingLevel.Level05:
					return 5;
				case StartingLevel.Level10:
					return 10;
				case StartingLevel.Level16:
					return 16;
				case StartingLevel.Level25:
					return 25;
				case StartingLevel.Level36:
					return 36;
				case StartingLevel.Level50:
					return 50;
			}

			return 1;
		}
	}
}
