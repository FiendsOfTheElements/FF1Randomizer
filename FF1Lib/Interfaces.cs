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
		bool? IncentivizeCardia { get; }
		IncentivePlacementType IceCaveIncentivePlacementType { get; }
		IncentivePlacementType OrdealsIncentivePlacementType { get; }
		IncentivePlacementType MarshIncentivePlacementType { get; }
		IncentivePlacementType TitansIncentivePlacementType { get; }
		IncentivePlacementTypeEarth EarthIncentivePlacementType { get; }
		IncentivePlacementTypeVolcano VolcanoIncentivePlacementType { get; }
		IncentivePlacementTypeSea SeaShrineIncentivePlacementType { get; }
		IncentivePlacementTypeSky SkyPalaceIncentivePlacementType { get; }
		IncentivePlacementType CorneriaIncentivePlacementType { get; }
		IncentivePlacementType MarshLockedIncentivePlacementType { get; }
		IncentivePlacementType CardiaIncentivePlacementType { get; }
		bool? MermaidPrison { get; }

		bool? IncentivizeXcalber { get; }
		bool? IncentivizeMasamune { get; }
		bool? IncentivizeKatana { get; }
		bool? IncentivizeVorpal { get; }
		bool? IncentivizeRibbon { get; }
		bool? IncentivizeBridge { get; }
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
		bool? IncentivizePowerRod { get; }
		bool? IncentivizeDefCastArmor { get; }
		bool? IncentivizeOffCastArmor { get; }
		bool? IncentivizeDefCastWeapon { get; }
		bool? IncentivizeOffCastWeapon { get; }
		bool IncentivizeOtherCastWeapon { get; }
		bool? IncentivizeOpal { get; }
		bool Incentivize65K { get; }
		bool IncentivizeBad { get; }
		bool? NoMasamune { get; }
		bool? NoXcalber { get; }
		bool? IsFloaterRemoved { get;  }
		bool? NoTail { get; }
		bool? IsCanoeFree { get; }
		bool? FreeLute { get; }
		bool? FreeTail { get; }
		bool? IsBridgeFree { get; }
		bool? IsCanalFree { get; }
		bool? IsShipFree { get; }
		bool? GuaranteedMasamune { get; }

	}
	public interface IMapEditFlags : IItemPlacementFlags
	{
		bool? MapAirshipDock { get; }
		bool? MapOnracDock { get; }
		bool? MapMirageDock { get; }
		bool? MapBahamutCardiaDock { get; }
		bool? MapDragonsHoard { get; }
		bool? MapLefeinRiver { get; }
		bool? MapBridgeLefein { get; }
		bool? MapRiverToMelmond { get; }
		bool? MapGaiaMountainPass { get; }
		bool? MapHighwayToOrdeals { get; }
		bool? TitansTrove { get; }
		bool? GaiaShortcut { get; }
		bool? MoveGaiaItemShop { get; }
		bool DisableOWMapModifications { get; }
		//OwMapExchanges OwMapExchange { get; }
	}
	public interface IItemPlacementFlags : IItemShuffleFlags, IVictoryConditionFlags
	{
		bool LaterLoose { get; }
		bool Spoilers { get; }
		bool? MapCanalBridge { get; }
		bool? MapConeriaDwarves { get; }
		bool? MapVolcanoIceRiver { get; }
		bool? MapDwarvesNorthwest { get; }
		bool? RandomWares { get; }
		bool? RandomLoot { get; }
		bool? BetterTrapChests { get; }
		bool? EarlierRuby { get; }
		GuaranteedDefenseItem GuaranteedDefenseItem { get; }
		GuaranteedPowerItem GuaranteedPowerItem { get; }
		bool? GuaranteedMasamune { get; }
		bool? SendMasamuneHome { get; }
		bool? NoMasamune { get; }
		bool? NoXcalber { get; }
		RandomizeTreasureMode RandomizeTreasure { get; }
		bool OpenChestsInOrder { get; }
		WorldWealthMode WorldWealth { get; }
		DeepDungeonGeneratorMode DeepDungeonGenerator { get; }
		ConsumableChestSet MoreConsumableChests { get; }
		ExtConsumableSet ExtConsumableSet { get; }
		ExtConsumableChestSet ExtConsumableChests { get; }
		bool IncentiveChestItemsFanfare { get; }
		ItemMagicMode ItemMagicMode { get; }
		bool LooseItemsForwardPlacement { get; }
		bool LooseItemsSpreadPlacement { get; }
		bool LooseItemsNpcBalance { get; }
		bool? Entrances { get; }
		OwMapExchanges OwMapExchange { get; }
		GameModes GameMode { get;  }
		bool Archipelago { get; }
		bool PredictivePlacement { get;}
		bool AllowUnsafePlacement { get; }
		bool Etherizer { get; }
		bool ShipCanalBeforeFloater { get; }
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
		bool NoOverworld { get; }
	}
	public interface IScaleFlags
	{
		StartingGold StartingGold { get; }
		double ExpMultiplier { get; }
		int PriceScaleFactorLow { get; }
		int PriceScaleFactorHigh { get; }
		bool? ExcludeGoldFromScaling { get; }
		bool CheapVendorItem { get; }
		ExtConsumableSet ExtConsumableSet { get; }
	}
	public interface IFloorShuffleFlags
	{
		bool Spoilers { get; }
		bool? Entrances { get; }
		bool? EntrancesIncludesDeadEnds { get; }
		bool? EntrancesMixedWithTowns { get; }
		bool? Towns { get; }
		bool? IncludeConeria { get; }
		bool? Floors { get; }
		bool? DeepCastlesPossible { get; }
		bool? AllowDeepCastles { get; }
		bool? DeepTownsPossible { get; }
		bool? AllowDeepTowns { get; }
		bool? AllowUnsafeStartArea { get; }
		bool? IsFloaterRemoved { get; }
	    bool? IsAirshipFree { get; }
		bool? MapBahamutCardiaDock { get; }
	    OwMapExchanges OwMapExchange { get; }
		GameModes GameMode { get; }
	}
	public interface IVictoryConditionFlags
	{
		bool OnlyRequireGameIsBeatable { get; }
		bool ShardHunt { get; }
		bool? IsBridgeFree { get; }
		bool? IsAirshipFree { get; }
		bool? IsShipFree { get; }
		bool? IsCanalFree { get; }
		bool? IsCanoeFree { get; }
		bool? FreeLute { get; }
		bool? FreeTail { get; }
		bool? NoTail { get; }
		bool? IsFloaterRemoved { get; }
		bool? AirBoat { get; }
		bool? LooseExcludePlacedDungeons { get; }
		bool NoOverworld { get; }
		bool DesertOfDeath { get; }
	}
}
