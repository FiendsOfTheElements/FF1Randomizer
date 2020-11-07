namespace FF1Lib
{
	public interface IIncentiveFlags : IItemShuffleFlags
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
		bool IncentivizeCaravan { get; }
		bool? IncentivizeOrdeals { get; }
		bool? IncentivizeIceCave { get; }
		bool? IncentivizeVolcano { get; }
		bool? IncentivizeConeria { get; }
		bool? IncentivizeMarsh { get; }
		bool? IncentivizeMarshKeyLocked { get; }
		bool? IncentivizeSkyPalace { get; }
		bool? IncentivizeTitansTrove { get; }
		bool? IncentivizeEarth { get; }
		bool? IncentivizeSeaShrine { get; }

		bool? IncentivizeXcalber { get; }
		bool? IncentivizeMasamune { get; }
		bool? IncentivizeKatana { get; }
		bool? IncentivizeVorpal { get; }
		bool? IncentivizeRibbon { get; }
		bool IncentivizeBridge { get; }
		bool? IncentivizeShip { get; }
		bool? IncentivizeCanal { get; }
		bool? IncentivizeLute { get; }
		bool? IncentivizeCrown { get; }
		bool? IncentivizeCrystal { get; }
		bool? IncentivizeHerb { get; }
		bool? IncentivizeKey { get; }
		bool? IncentivizeTnt { get; }
		bool? IncentivizeAdamant { get; }
		bool? IncentivizeSlab { get; }
		bool? IncentivizeRuby { get; }
		bool? IncentivizeRod { get; }
		bool? IncentivizeFloater { get; }
		bool? IncentivizeChime { get; }
		bool? IncentivizePromotion { get; }
		bool? IncentivizeCube { get; }
		bool? IncentivizeBottle { get; }
		bool? IncentivizeOxyale { get; }
		bool? IncentivizeCanoe { get; }
		bool IncentivizeRibbon2 { get; }
		bool? IncentivizeOtherCastArmor { get; }
		bool? IncentivizeDefCastArmor { get; }
		bool? IncentivizeOffCastArmor { get; }
		bool? IncentivizeDefCastWeapon { get; }
		bool? IncentivizeOffCastWeapon { get; }
		bool IncentivizeOtherCastWeapon { get; }
		bool? IncentivizeOpal { get; }
		bool Incentivize65K { get; }
		bool IncentivizeBad { get; }
	}
	public interface IMapEditFlags : IItemPlacementFlags
	{
		bool? MapAirshipDock { get; }
		bool? MapOnracDock { get; }
		bool? MapMirageDock { get; }
		bool? TitansTrove { get; }
	}
	public interface IItemPlacementFlags : IItemShuffleFlags, IVictoryConditionFlags
	{
		bool Spoilers { get; }
		bool ClassicItemPlacement { get; }
		bool? MapCanalBridge { get; }
		bool? MapConeriaDwarves { get; }
		bool? MapVolcanoIceRiver { get; }
		bool? MapDwarvesNorthwest { get; }
		bool? RandomWares { get; }
		bool? RandomLoot { get; }
		bool? BetterTrapChests { get; }
		bool? EarlierRuby { get; }
		bool? GuaranteedRuseItem { get; }
		WorldWealthMode WorldWealth { get; }
	}
	public interface IItemShuffleFlags
	{
		bool? Treasures { get; }
		bool? NPCItems { get; }
		bool? NPCFetchItems { get; }
		bool? EarlyKing { get; }
		bool? EarlySarda { get; }
		bool? EarlySage { get; }
		bool? EarlyOrdeals { get; }
	}
	public interface IScaleFlags
	{
		bool StartingGold { get; }
		bool WrapPriceOverflow { get; }
		bool WrapStatOverflow { get; }
		double ExpMultiplier { get; }
		double PriceScaleFactor { get; }
	}
	public interface IFloorShuffleFlags
	{
		bool Spoilers { get; }
		bool? Entrances { get; }
		bool? EntrancesIncludesDeadEnds { get; }
		bool? EntrancesMixedWithTowns { get; }
		bool? Towns { get; }
		bool? Floors { get; }
		bool? DeepCastlesPossible { get; }
		bool? AllowDeepCastles { get; }
		bool? DeepTownsPossible { get; }
		bool? AllowDeepTowns { get; }
		bool? AllowUnsafeStartArea { get; }
	}
	public interface IVictoryConditionFlags
	{
		bool OnlyRequireGameIsBeatable { get; }
		bool ShardHunt { get; }
		bool? ShortToFR { get; }
		bool? FreeBridge { get; }
		bool? FreeAirship { get; }
		bool? FreeShip { get; }
		bool? FreeCanal { get; }
		bool? FreeCanoe { get; }
		bool? FreeLute { get; }
		bool? FreeTail { get; }
		bool? NoTail { get; }
	}
}
