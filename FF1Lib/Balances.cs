using System.ComponentModel;

namespace FF1Lib
{
	public enum MDEFGrowthMode
	{
		[Description("Vanilla")]
		None = 0,
		[Description("BB/Master +3/+4")]
		BBFix,
		[Description("Invert All")]
		Invert
	}

	public enum ThiefAGI
	{
	    [Description("10 (Vanilla)")]
	    Vanilla = 0,

		[Description("30")]
		Agi30 = 4,

		[Description("50")]
		Agi50 = 5,

	    [Description("80")]
	    Agi80 = 1,

	    [Description("100")]
	    Agi100 = 2,

	    [Description("120")]
	    Agi120 = 3
	}

	public partial class FF1Rom
	{
		public void DontDoubleBBCritRates()
		{
			// Don't double BB crit
			Put(0x32DDD, new byte[] { 0xEA });
		}

		public void DoubleWeaponCritRates()
		{
			// Increase crit rate of all weapons
			var weapons = Get(WeaponOffset, WeaponSize * WeaponCount).Chunk(WeaponSize);
			foreach (var weapon in weapons)
			{
				weapon[2] *= 2;
			}
			Put(WeaponOffset, weapons.SelectMany(weapon => weapon.ToBytes()).ToArray());
		}

		public void IncreaseWeaponBonus(int weaponBonusValue)
		{
			//change the weapon bonus from +4 to +X
			Put(0x326F5, new byte[] { (byte) weaponBonusValue });
		}

		public void MDefChanges(MDEFGrowthMode mode)
		{
			if (mode == MDEFGrowthMode.BBFix)
			{
				RemakeStyleMasterMDEF();
			}
			if (mode == MDEFGrowthMode.Invert)
			{
				InvertedMDEF();
			}
		}

		public void KnightNinjaChargesForAllLevels()
		{
			for(int cur_pointer = NewLevelUpDataOffset; cur_pointer < NewLevelUpDataOffset + 196; cur_pointer += 2) // we need to cycle through the 49 levelups for Fighter and the 49 levelups for Thief, each are two bytes
			{
				if (Data[cur_pointer + 1] != 0)
					Data[cur_pointer + 1] = 0xFF; // every spell charge gain that isn't equal to 0 is changed to FF, so each spell level will gain a charge instead of just the first three / four
			}
		}

		public void RemakeStyleMasterMDEF()
		{
			//Black Belt & Master growth rates are separate
			ClassData[Classes.BlackBelt].MDefGrowth = 3;
			ClassData[Classes.Master].MDefGrowth = 4;
		}

		public void InvertedMDEF()
		{
		    for (int i = 0; i < 12; i++) {
			ClassData[(Classes)i].MDefGrowth = (byte)(5 - ClassData[(Classes)i].MDefGrowth);
		    }
		}

		public void FixHitChanceCap()
		{
			Put(0x6CA9A, Blob.FromHex("FB"));
			Put(0x6CA9E, Blob.FromHex("FA"));
		}
	}
}
