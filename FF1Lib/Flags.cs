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
		bool AllowIceShip { get; }

		bool MapConeriaDwarves { get; }
		bool MapVolcanoIceRiver { get; }
		bool MapTitansTrove { get; }
	}
	public class Flags : ITreasureShuffleFlags, IIncentiveFlags
	{
		public bool Treasures { get; set; }
		public bool NPCItems { get; set; }

		public bool Shops { get; set; }
		public bool MagicShops { get; set; }
		public bool MagicLevels { get; set; }
		public bool MagicPermissions { get; set; }
		public bool Rng { get; set; }
		public bool EnemyScripts { get; set; }
		public bool EnemySkillsSpells { get; set; }
		public bool EnemyStatusAttacks { get; set; }
		public bool Ordeals { get; set; }

		public bool EarlyRod { get; set; }
		public bool EarlyCanoe { get; set; }
		public bool EarlyOrdeals { get; set; }
		public bool EarlyBridge { get; set; }
		public bool NoPartyShuffle { get; set; }
		public bool SpeedHacks { get; set; }
		public bool IdentifyTreasures { get; set; }
		public bool Dash { get; set; }
		public bool BuyTen { get; set; }

		public bool HouseMPRestoration { get; set; }
		public bool WeaponStats { get; set; }
		public bool ChanceToRun { get; set; }
		public bool SpellBugs { get; set; }
		public bool EnemyStatusAttackBug { get; set; }

		public bool FunEnemyNames { get; set; }
		public bool PaletteSwap { get; set; }
		public bool TeamSteak { get; set; }
		public bool ModernBattlefield { get; set; }
		public MusicShuffle Music { get; set; }

		public int ForcedPartyMembers { get; set; }
		public double EnemyScaleFactor { get; set; }
		public double PriceScaleFactor { get; set; }
		public double ExpMultiplier { get; set; }
		public int ExpBonus { get; set; }

		public bool MapConeriaDwarves { get; set; }
		public bool MapVolcanoIceRiver { get; set; }
		public bool MapTitansTrove { get; set; }
		public bool AllowIceShip => false;
		public bool EasyMode { get; set; }

		public bool IncentivizeKingConeria { get; set; }
		public bool IncentivizePrincess { get; set; }
		public bool IncentivizeMatoya { get; set; }
		public bool IncentivizeBikke { get; set; }
		public bool IncentivizeElfPrince { get; set; }
		public bool IncentivizeAstos { get; set; }
		public bool IncentivizeNerrick { get; set; }
		public bool IncentivizeSmith { get; set; }
		public bool IncentivizeSarda { get; set; }
		public bool IncentivizeCanoeSage { get; set; }
		public bool IncentivizeCubeBot { get; set; }
		public bool IncentivizeFairy { get; set; }
		public bool IncentivizeLefein { get; set; }
		public bool IncentivizeMarsh { get; set; }
		public bool IncentivizeVolcano { get; set; }
		public bool IncentivizeConeria { get; set; }
		public bool IncentivizeEarth { get; set; }
		public bool IncentivizeIceCave { get; set; }
		public bool IncentivizeOrdeals { get; set; }
		public bool IncentivizeSeaShrine { get; set; }

		public bool IncentivizeBridge { get; set; }
		public bool IncentivizeShip { get; set; }
		public bool IncentivizeCanal { get; set; }
		public bool IncentivizeLute { get; set; }
		public bool IncentivizeCrown { get; set; }
		public bool IncentivizeCrystal { get; set; }
		public bool IncentivizeHerb { get; set; }
		public bool IncentivizeKey { get; set; }
		public bool IncentivizeTnt { get; set; }
		public bool IncentivizeAdamant { get; set; }
		public bool IncentivizeSlab { get; set; }
		public bool IncentivizeRuby { get; set; }
		public bool IncentivizeRod { get; set; }
		public bool IncentivizeFloater { get; set; }
		public bool IncentivizeChime { get; set; }
		public bool IncentivizeTail { get; set; }
		public bool IncentivizeCube { get; set; }
		public bool IncentivizeBottle { get; set; }
		public bool IncentivizeOxyale { get; set; }
		public bool IncentivizeCanoe { get; set; }
		public bool IncentivizeXcalber { get; set; }
		public bool IncentivizeMasamune { get; set; }
		public bool IncentivizeRibbon { get; set; }
		public bool IncentivizeRibbon2 { get; set; }
		public bool IncentivizePowerGauntlet { get; set; }
		public bool IncentivizeWhiteShirt { get; set; }
		public bool IncentivizeBlackShirt { get; set; }
		public bool IncentivizeOpal { get; set; }
		public bool Incentivize65K { get; set; }
		public bool IncentivizeBad { get; set; }
	}
}
