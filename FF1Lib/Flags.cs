using System;
using System.Collections.Generic;
using System.Linq;

namespace FF1Lib
{
	public interface IIncentiveFlags
	{
		bool IncentivizeKingConeria { get; }
		bool IncentivizePrincess { get; }
		bool IncentivizeMatoya { get; }
		bool IncentivizeBikke { get; }
		bool IncentivizeElfPrince { get; }
		bool IncentivizeAstos { get; }
		bool IncentivizeNerrick { get; }
		bool IncentivizeSmith { get; }
		bool IncentivizeSarda { get; }
		bool IncentivizeCanoeSage { get; }
		bool IncentivizeCubeBot { get; }
		bool IncentivizeFairy { get; }
		bool IncentivizeLefein { get; }
		bool IncentivizeOrdeals { get; }
		bool IncentivizeIceCave { get; }
		bool IncentivizeVolcano { get; }
		bool IncentivizeConeria { get; }
		bool IncentivizeMarsh { get; }
		bool IncentivizeEarth { get; }
		bool IncentivizeSeaShrine { get; }

		bool IncentivizeXcalber { get; }
		bool IncentivizeMasamune { get; }
		bool IncentivizeRibbon { get; }
		bool IncentivizeBridge { get; }
		bool IncentivizeShip { get; }
		bool IncentivizeCanal { get; }
		bool IncentivizeLute { get; }
		bool IncentivizeCrown { get; }
		bool IncentivizeCrystal { get; }
		bool IncentivizeHerb { get; }
		bool IncentivizeKey { get; }
		bool IncentivizeTnt { get; }
		bool IncentivizeAdamant { get; }
		bool IncentivizeSlab { get; }
		bool IncentivizeRuby { get; }
		bool IncentivizeRod { get; }
		bool IncentivizeFloater { get; }
		bool IncentivizeChime { get; }
		bool IncentivizeTail { get; }
		bool IncentivizeCube { get; }
		bool IncentivizeBottle { get; }
		bool IncentivizeOxyale { get; }
		bool IncentivizeCanoe { get; }
		bool IncentivizeRibbon2 { get; }
		bool IncentivizePowerGauntlet { get; }
		bool IncentivizeWhiteShirt { get; }
		bool IncentivizeBlackShirt { get; }
		bool IncentivizeOpal { get; }
		bool Incentivize65K { get; }
		bool IncentivizeBad { get; }
	}
	public interface ITreasureShuffleFlags
	{
		bool Treasures { get; }
		bool NPCItems { get; }
		bool EarlyRod { get; }
		bool EarlyCanoe { get; }
		bool EarlyOrdeals { get; }
		bool EarlyBridge { get; }

		bool MapConeriaDwarves { get; }
		bool MapVolcanoIceRiver { get; }
		bool MapTitansTrove { get; }
	}
	public class Flags : ITreasureShuffleFlags, IIncentiveFlags
	{
		[FlagString(Character = 0, FlagBit = 1)]
		public bool Treasures { get; set; }
		[FlagString(Character = 0, FlagBit = 2)]
		public bool NPCItems { get; set; }
		[FlagString(Character = 0, FlagBit = 4)]
		public bool Shops { get; set; }

		[FlagString(Character = 1, FlagBit = 1)]
		public bool EarlyRod { get; set; }
		[FlagString(Character = 1, FlagBit = 2)]
		public bool EarlyCanoe { get; set; }
		[FlagString(Character = 1, FlagBit = 4)]
		public bool EarlyOrdeals { get; set; }
		[FlagString(Character = 1, FlagBit = 8)]
		public bool EarlyBridge { get; set; }

		[FlagString(Character = 2, FlagBit = 1)]
		public bool MagicShops { get; set; }
		[FlagString(Character = 2, FlagBit = 2)]
		public bool MagicLevels { get; set; }
		[FlagString(Character = 2, FlagBit = 4)]
		public bool MagicPermissions { get; set; }

		[FlagString(Character = 3, FlagBit = 1)]
		public bool Rng { get; set; }
		[FlagString(Character = 3, FlagBit = 2)]
		public bool EnemyScripts { get; set; }
		[FlagString(Character = 3, FlagBit = 4)]
		public bool EnemySkillsSpells { get; set; }
		[FlagString(Character = 3, FlagBit = 8)]
		public bool EnemyStatusAttacks { get; set; }
		[FlagString(Character = 3, FlagBit = 32)]
		public bool EasyMode { get; set; }

		[FlagString(Character = 4, FlagBit = 1)]
		public bool Ordeals { get; set; }
		[FlagString(Character = 4, FlagBit = 2)]
		public bool MapTitansTrove { get; set; }
		[FlagString(Character = 4, FlagBit = 4)]
		public bool MapConeriaDwarves { get; set; }
		[FlagString(Character = 4, FlagBit = 8)]
		public bool MapVolcanoIceRiver { get; set; }

		[FlagString(Character = 14, FlagBit = 1)]
		public bool SpeedHacks { get; set; }
		[FlagString(Character = 14, FlagBit = 2)]
		public bool NoPartyShuffle { get; set; }
		[FlagString(Character = 14, FlagBit = 4)]
		public bool Dash { get; set; }
		[FlagString(Character = 14, FlagBit = 8)]
		public bool BuyTen { get; set; }
		[FlagString(Character = 14, FlagBit = 16)]
		public bool IdentifyTreasures { get; set; }
		[FlagString(Character = 14, FlagBit = 32)]
		public bool ModernBattlefield { get; set; }

		[FlagString(Character = 15, FlagBit = 1)]
		public bool HouseMPRestoration { get; set; }
		[FlagString(Character = 15, FlagBit = 2)]
		public bool WeaponStats { get; set; }
		[FlagString(Character = 15, FlagBit = 4)]
		public bool ChanceToRun { get; set; }
		[FlagString(Character = 15, FlagBit = 8)]
		public bool SpellBugs { get; set; }
		[FlagString(Character = 15, FlagBit = 16)]
		public bool EnemyStatusAttackBug { get; set; }

		public bool FunEnemyNames { get; set; }
		public bool PaletteSwap { get; set; }
		public bool TeamSteak { get; set; }
		public MusicShuffle Music { get; set; }

		[FlagString(Character = 16, Multiplier = 0.1)]
		public double EnemyScaleFactor { get; set; }
		[FlagString(Character = 17, Multiplier = 0.1)]
		public double PriceScaleFactor { get; set; }
		[FlagString(Character = 18, Multiplier = 0.1)]
		public double ExpMultiplier { get; set; }
		[FlagString(Character = 19, Multiplier = 10)]
		public int ExpBonus { get; set; }
		[FlagString(Character = 20, Multiplier = 1)]
		public int ForcedPartyMembers { get; set; }

		[FlagString(Character = 5, FlagBit = 1)]
		public bool IncentivizeMarsh { get; set; }
		[FlagString(Character = 5, FlagBit = 2)]
		public bool IncentivizeEarth { get; set; }
		[FlagString(Character = 5, FlagBit = 4)]
		public bool IncentivizeVolcano { get; set; }
		[FlagString(Character = 5, FlagBit = 8)]
		public bool IncentivizeIceCave { get; set; }
		[FlagString(Character = 5, FlagBit = 16)]
		public bool IncentivizeOrdeals { get; set; }
		[FlagString(Character = 5, FlagBit = 32)]
		public bool IncentivizeSeaShrine { get; set; }

		[FlagString(Character = 6, FlagBit = 1)]
		public bool IncentivizeCrown { get; set; }
		[FlagString(Character = 6, FlagBit = 2)]
		public bool IncentivizeRuby { get; set; }
		[FlagString(Character = 6, FlagBit = 4)]
		public bool IncentivizeTnt { get; set; }
		[FlagString(Character = 6, FlagBit = 8)]
		public bool IncentivizeFloater { get; set; }
		[FlagString(Character = 6, FlagBit = 16)]
		public bool IncentivizeTail { get; set; }
		[FlagString(Character = 6, FlagBit = 32)]
		public bool IncentivizeSlab { get; set; }

		[FlagString(Character = 7, FlagBit = 1)]
		public bool IncentivizeKingConeria { get; set; }
		[FlagString(Character = 7, FlagBit = 2)]
		public bool IncentivizePrincess { get; set; }
		[FlagString(Character = 7, FlagBit = 4)]
		public bool IncentivizeBikke { get; set; }
		[FlagString(Character = 7, FlagBit = 8)]
		public bool IncentivizeAstos { get; set; }
		[FlagString(Character = 7, FlagBit = 16)]
		public bool IncentivizeMatoya { get; set; }
		[FlagString(Character = 7, FlagBit = 32)]
		public bool IncentivizeElfPrince { get; set; }

		[FlagString(Character = 8, FlagBit = 1)]
		public bool IncentivizeNerrick { get; set; }
		[FlagString(Character = 8, FlagBit = 2)]
		public bool IncentivizeSarda { get; set; }
		[FlagString(Character = 8, FlagBit = 4)]
		public bool IncentivizeCanoeSage { get; set; }
		[FlagString(Character = 8, FlagBit = 8)]
		public bool IncentivizeFairy { get; set; }
		[FlagString(Character = 8, FlagBit = 16)]
		public bool IncentivizeLefein { get; set; }
		[FlagString(Character = 8, FlagBit = 32)]
		public bool IncentivizeCubeBot { get; set; }

		[FlagString(Character = 9, FlagBit = 1)]
		public bool IncentivizeSmith { get; set; }
		[FlagString(Character = 9, FlagBit = 2)]
		public bool IncentivizeCaravan { get; set; }
		[FlagString(Character = 9, FlagBit = 4)]
		public bool IncentivizeConeria { get; set; }

		[FlagString(Character = 10, FlagBit = 1)]
		public bool IncentivizeBridge { get; set; }
		[FlagString(Character = 10, FlagBit = 2)]
		public bool IncentivizeLute { get; set; }
		[FlagString(Character = 10, FlagBit = 4)]
		public bool IncentivizeShip { get; set; }
		[FlagString(Character = 10, FlagBit = 8)]
		public bool IncentivizeCrystal { get; set; }
		[FlagString(Character = 10, FlagBit = 16)]
		public bool IncentivizeHerb { get; set; }
		[FlagString(Character = 10, FlagBit = 32)]
		public bool IncentivizeKey { get; set; }

		[FlagString(Character = 11, FlagBit = 1)]
		public bool IncentivizeCanal { get; set; }
		[FlagString(Character = 11, FlagBit = 2)]
		public bool IncentivizeRod { get; set; }
		[FlagString(Character = 11, FlagBit = 4)]
		public bool IncentivizeCanoe { get; set; }
		[FlagString(Character = 11, FlagBit = 8)]
		public bool IncentivizeOxyale { get; set; }
		[FlagString(Character = 11, FlagBit = 16)]
		public bool IncentivizeChime { get; set; }
		[FlagString(Character = 11, FlagBit = 32)]
		public bool IncentivizeCube { get; set; }

		[FlagString(Character = 12, FlagBit = 1)]
		public bool IncentivizeXcalber { get; set; }
		[FlagString(Character = 12, FlagBit = 2)]
		public bool IncentivizeBottle { get; set; }
		[FlagString(Character = 12, FlagBit = 4)]
		public bool IncentivizeAdamant { get; set; }
		[FlagString(Character = 12, FlagBit = 8)]
		public bool IncentivizeMasamune { get; set; }
		[FlagString(Character = 12, FlagBit = 16)]
		public bool IncentivizeRibbon { get; set; }
		[FlagString(Character = 12, FlagBit = 32)]
		public bool IncentivizeRibbon2 { get; set; }

		[FlagString(Character = 13, FlagBit = 1)]
		public bool IncentivizePowerGauntlet { get; set; }
		[FlagString(Character = 13, FlagBit = 2)]
		public bool IncentivizeWhiteShirt { get; set; }
		[FlagString(Character = 13, FlagBit = 4)]
		public bool IncentivizeBlackShirt { get; set; }
		[FlagString(Character = 13, FlagBit = 8)]
		public bool IncentivizeOpal { get; set; }
		[FlagString(Character = 13, FlagBit = 16)]
		public bool Incentivize65K { get; set; }
		[FlagString(Character = 13, FlagBit = 32)]
		public bool IncentivizeBad { get; set; }

		public static Dictionary<string, FlagStringAttribute> GetFlagStringAttributes()
		{
			var allProps =
				typeof(Flags).GetProperties()
					.Where(x => (x.GetCustomAttributes(typeof(FlagStringAttribute), false).FirstOrDefault() as FlagStringAttribute) != null);
			return allProps
				.ToDictionary(x => x.Name,
							x => x.GetCustomAttributes(typeof(FlagStringAttribute), false)
								.FirstOrDefault() as FlagStringAttribute);
		}
	}

	public class FlagStringAttribute : Attribute
	{
		public int Character { get; set; }
		public int FlagBit { get; set; }
		public double Multiplier { get; set; }

		public string ToVueComputedPropertyString()
		{
			if (FlagBit > 0)
				return $"{{get:function(){{if(this.flagString.length <= {Character}) return false;" +
				$"return (base64Chars.indexOf(this.flagString.charAt({Character})) & {FlagBit}) > 0; }}, " +
				$"set:function(){{while(this.flagString.length <= {Character})this.flagString += base64Chars[0];" +
				$"var toggled = (base64Chars.indexOf(this.flagString.charAt({Character}))) ^ {FlagBit};" +
				$"this.flagString = this.flagString.substr(0,{Character}) + base64Chars[toggled] + this.flagString.substr({Character + 1});}}}}";

			return $"{{get:function (){{ if (this.flagString.length <= {Character}) return 0; " +
			$"return base64Chars.indexOf(this.flagString[{Character}]) * {Multiplier};}}," +
			$"set:function(newValue){{while(this.flagString.length <= {Character})this.flagString += base64Chars[0];" +
			$"var scaledValue = (newValue / {Multiplier}).toFixed() % base64Chars.length;" +
			$"this.flagString = this.flagString.substr(0,{Character}) + base64Chars[scaledValue] + this.flagString.substr({Character + 1});}} }}";

		}
	}
}
