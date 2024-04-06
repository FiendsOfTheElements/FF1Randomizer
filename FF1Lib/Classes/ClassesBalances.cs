using RomUtilities;
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
	public enum ThiefOptions
	{
		[Description("None")]
		None = 0,
		[Description("Double Hit% Growth")]
		DoubleHit,
		[Description("Raised Agility")]
		RaisedAgility,
		[Description("Lockpicking")]
		Lockpicking,
	}
	public enum WhiteMageOptions
	{
		[Description("None")]
		None = 0,
		[Description("Harm Hurts All Type")]
		ImprovedHarm,
	}

	public static partial class FlagRules
	{
		public static FlagRule ThiefModeStandard { get; set; } = new FlagRule()
		{
			Conditions = new() { new() { new FlagCondition() { Name = "ThiefMode", Type = SettingType.OptionList, Value = (int)ThiefOptions.None } } },
			Actions = new List<FlagAction>()
				{
					new FlagAction() { Setting = "ThiefAgilityBuff", Action = FlagActions.SetValue, Value = (int)ThiefAGI.Vanilla },
				}
		};
		public static FlagRule ThiefModeDoubleHit { get; set; } = new FlagRule()
		{
			Conditions = new() { new() { new FlagCondition() { Name = "ThiefMode", Type = SettingType.OptionList, Value = (int)ThiefOptions.DoubleHit } } },
			Actions = new List<FlagAction>()
				{
					new FlagAction() { Setting = "ThieHitRate", Action = FlagActions.Enable },
					new FlagAction() { Setting = "ThiefAgilityBuff", Action = FlagActions.SetValue, Value = (int)ThiefAGI.Vanilla },
				}
		};
		public static FlagRule ThiefModeAgiBuff { get; set; } = new FlagRule()
		{
			Conditions = new() { new() { new FlagCondition() { Name = "ThiefMode", Type = SettingType.OptionList, Value = (int)ThiefOptions.RaisedAgility } } },
			Actions = new List<FlagAction>()
				{
					new FlagAction() { Setting = "ThiefAgilityBuff", Action = FlagActions.SetValue, Value = (int)ThiefAGI.Agi80 },
				}
		};
		public static FlagRule ThiefModeLockpicking { get; set; } = new FlagRule()
		{
			Conditions = new() { new() { new FlagCondition() { Name = "ThiefMode", Type = SettingType.OptionList, Value = (int)ThiefOptions.Lockpicking } } },
			Actions = new List<FlagAction>()
				{
					new FlagAction() { Setting = "ThiefAgilityBuff", Action = FlagActions.SetValue, Value = (int)ThiefAGI.Vanilla },
					new FlagAction() { Setting = "ThiefLockpicking", Action = FlagActions.Enable },
					new FlagAction() { Setting = "LockpickingLevelRequirement", Action = FlagActions.SetValue, Value = 15 },
				}
		};
		public static FlagRule WhiteMageModeImprovedHarm { get; set; } = new FlagRule()
		{
			Conditions = new() { new() { new FlagCondition() { Name = "WhiteMageMode", Type = SettingType.OptionList, Value = (int)WhiteMageOptions.ImprovedHarm } } },
			Actions = new List<FlagAction>()
				{
					new FlagAction() { Setting = "WhiteMageHarmEveryone", Action = FlagActions.Enable },
				}
		};
	}
	public partial class FF1Rom
	{
		//public void ClassesBalances(Settings settings, MT19337 rng)
		public void ClassesBalances(Flags flags, MT19337 rng)
		{

			// Stats
			// Move to ClassesData
			//if (settings.GetBool("ReducedLuck"))
			if ((bool)flags.ReducedLuck)
			{
				ReducedLuck();
			}

			// MDef
			// Put into classesData
			//var mdefgrowth = (MDEFGrowthMode)settings.GetInt("MDefMode");
			var mdefgrowth = flags.MDefMode;
			if (mdefgrowth != MDEFGrowthMode.None)
			{
				MDefChanges(mdefgrowth);
			}

			// Classes specific
			//if (settings.GetBool("DontDoubleBBCritRates"))
			if ((bool)flags.BBCritRate)
			{
				DontDoubleBBCritRates();
			}

			//if (settings.GetBool("WhiteMageHarmEveryone"))
			if ((bool)flags.WhiteMageHarmEveryone)
			{
				WhiteMageHarmEveryone();
			}

			//if (settings.GetBool("ThiefLockpicking"))
			if ((bool)flags.Lockpicking)
			{
				EnableLockpicking();
				//SetLockpickingLevel(settings.GetInt("ThiefLockpickingLevel"));
				SetLockpickingLevel((int)flags.LockpickingLevelRequirement);
			}

			// MP Growth
			//if (settings.GetBool("KnightNinjaChargesForAllLevels"))
			if ((bool)flags.AllSpellLevelsForKnightNinja)
			{
				KnightNinjaChargesForAllLevels();
			}

			//new ChanceToRun(this, settings).FixChanceToRun();
			new ChanceToRun(this, flags).FixChanceToRun();

			MoveLoadPlayerIBStats();

			// XP
			SetupClassAltXp();

			//new StartingLevels(this, settings).SetStartingLevels();
			new StartingLevels(this, flags).SetStartingLevels();

			//SetMaxLevel(settings.GetInt("MaxLevel"), StartingLevels.GetLevelNumber((StartingLevel)settings.GetInt("StartingLevel")));
			SetMaxLevel(rng.Between(flags.MaxLevelLow, flags.MaxLevelHigh), StartingLevels.GetLevelNumber(flags.StartingLevel));
		}

		public void DontDoubleBBCritRates()
		{
			// Don't double BB crit
			Put(0x32DDD, new byte[] { 0xEA });
		}
		public void MDefChanges(MDEFGrowthMode mode)
		{
			if (mode == MDEFGrowthMode.BBFix)
			{
				//Black Belt & Master growth rates are separate
				ClassData[Classes.BlackBelt].MDefGrowth = 3;
				ClassData[Classes.Master].MDefGrowth = 4;
			}
			if (mode == MDEFGrowthMode.Invert)
			{
				for (int i = 0; i < 12; i++)
				{
					ClassData[(Classes)i].MDefGrowth = (byte)(5 - ClassData[(Classes)i].MDefGrowth);
				}
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
		public void WhiteMageHarmEveryone()
		{
			PutInBank(0x0C, 0xB905, Blob.FromHex("2073B820F9B8202DB8A9A248A90348A91C4C03FEEAEAEAEAEAEAEAEAEA60"));
			PutInBank(0x1C, 0xA200, Blob.FromHex("004080C0AD86682908D030A9008D56688D5768AD896C297FAABD00A2A8B90061C904F017C90AF013A9008D58688D5968A9B948A92148A90C4C03FEA9B948A92248A90C4C03FE"));
		}

		//doesn't change any XP until it actually happens in blessed/cursed classes, just lays the groundwork in the asm
		public void SetupClassAltXp()
		{
			PutInBank(0x1B, 0x886E, Blob.FromHex("20189560"));
			//lut for class xp table pointers
			PutInBank(0x1B, 0x9500, Blob.FromHex("818C3595C8955B96EE968197818C3595C8955B96EE968197"));
			//actual new level up function
			PutInBank(0x1B, 0x9518, Blob.FromHex("A000B1860AAAA026B1860A187186187D00958582E8A9007D0095858360"));
		}

		public void MoveLoadPlayerIBStats()
		{
			PutInBank(0x0C, 0xAD21, Blob.FromHex("A99448A91548A91B4C03FEEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEA"));

			PutInBank(0x1B, 0x9400, Blob.FromHex("00000000000000000000000048B182AA68A88A918060A000A901200C94A001A902200C94A00AA903200C94A00BA904200C94A021A905200C94A025A906200C94A023A907200C94A022A908200C94A020A909200C94A000B182AAA024B1821D0094A00A9180A9AD48A92B48A90C4C03FE"));
		}

		// Move to Class
		public void ReducedLuck()
		{
			for (int i = 0; i < 12; i++)
			{
				ClassData[(Classes)i].LckStarting = (byte)(Math.Max(ClassData[(Classes)i].LckStarting - 4, 0));
			}
		}

		public void SetMaxLevel(int maxlevel, int startinglevel)
		{
			if (maxlevel >= 50)
			{
				return;
			}

			//This seems to be needed because if you're above the level cap it seems to continue giving xp.
			//Thus  the fix for starter levels > max levels is to just set max levels = starter levels so it doesnt increase xp further after those levels.
			if (maxlevel < startinglevel) maxlevel = startinglevel;
			maxlevel = Math.Min(maxlevel, 50);
			maxlevel = Math.Max(maxlevel, 1);
			maxlevel = maxlevel - 1;
			//new level up check is at 0x6C46F
			Put(0x6C46F, new byte[] { (byte)maxlevel });
			PutInBank(0x1B, 0x87E8, new byte[] { (byte)maxlevel });
		}
	}
}
