using FF1Lib;
using RomUtilities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FF1Lib
{
	public class StartingLevels
	{
		FF1Rom rom;
		Flags flags;

		public StartingLevels(FF1Rom _rom, Flags _flags)
		{
			rom = _rom;
			flags = _flags;
		}

		public void SetStartingLevels(int StartingLevelFromFlags, List<byte[]> levelRequirementBytes)
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

			// If Starting Level is 1, set xp to 0; otherwise pull from the level up table
			var exp = 0;
			if (StartingLevelFromFlags != 1)
				exp = (int)(Math.Max(BitConverter.ToUInt32(levelRequirementBytes[StartingLevelFromFlags - 2], 0), 1)) / 16 + 1;

			
			//Fill in Exp divided by 16 LowByte
			rom.PutInBank(0x1B, 0x84A4, new byte[] { (byte)(exp & 0xFF) });

			//Fill in Exp divided by 16 HighByte
			rom.PutInBank(0x1B, 0x84A9, new byte[] { (byte)(exp / 0xFF) });
		}
	}
}
