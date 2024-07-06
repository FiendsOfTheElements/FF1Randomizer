using RomUtilities;
using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FF1Lib
{
	public static partial class FlagRules
	{
		public static FlagRule EnemizerRule { get; set; } = new FlagRule()
		{
			Conditions = new()
			{
				new() { new FlagCondition() { Name = "RandomizeEnemizer", Type = SettingType.Toggle, Value = 1 } },
				new() { new FlagCondition() { Name = "RandomizeFormationEnemizer", Type = SettingType.Toggle, Value = 1 } }

			},
			Actions = new List<FlagAction>()
			{
				new FlagAction() { Setting = "EnemizerEnabled", Action = FlagActions.Enable },
			}
		};
	}
	public partial class Settings
	{
		private static List<Setting> AdvancedSettings = new()
		{
			new Setting("FixHouseMP", 1),
			new Setting("FixHouseHP", 1),
			new Setting("FixWeaponStats", 1),
			new Setting("FixSpellBugs", 1),
			new Setting("FixEnemyStatusAttack", 1),
			new Setting("FixBBAbsorbBug", 1),
			new Setting("ChanceToRun", typeof(ChanceToRunMode), (int)ChanceToRunMode.Fixed),
			new Setting("DontDoubleBBCritRates", 1),
			new Setting("KnightNinjaChargesForAllLevels", 0),
			new Setting("StartingLevel", typeof(StartingLevel), (int)StartingLevel.Level01),
			new Setting("MaxLevel", 1, 50, 1, 1, 50),
			new Setting("FixWeaponStats", 1),
			new Setting("ThiefAgilityBuff", typeof(ThiefAGI), (int)ThiefAGI.Agi80),
			new Setting("ThiefHitRate", 1),
			new Setting("ThiefLockpicking", 0),
			new Setting("LockpickingLevelRequirement", 1, 50, 1, 1, 15),
			new Setting("WhiteMageHarmEveryone", 0),
			new Setting("Spooky", 0)
		};
	}

	public partial class LayoutData
	{
		private static List<LayoutSection> AdvancedLayout = new()
		{
			new LayoutSection() { Name = "Classes", Flags = new()
			{
				new LayoutFlag() { Name = "ThiefMode", DisplayName = "Thief", Tooltip = "Select Thief Mode..." },
				new LayoutFlag() { Name = "WhiteMageMode", DisplayName = "White Mage", Tooltip = "Select White Mage Mode..." },
			} },
			new LayoutSection() { Name = "Misc", Flags = new()
			{
				new LayoutFlag() { Name = "Bugfixes", DisplayName = "Enable Bugfixes", Tooltip = "Allow whole set of different bugfixes..." },
			} },
		};
	}

}
