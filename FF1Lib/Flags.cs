using System.Numerics;
using System.Reflection;
using Newtonsoft.Json;
using System.IO.Compression;
using static FF1Lib.FF1Rom;
using FF1Lib.Sanity;

namespace FF1Lib
{
	public class Flags : IIncentiveFlags, IScaleFlags, IVictoryConditionFlags, IFloorShuffleFlags, IItemPlacementFlags
	{
		#region StartingEquipment

		public bool? StartingEquipmentMasamune { get; set; } = false;

		public bool? StartingEquipmentKatana { get; set; } = false;

		public bool? StartingEquipmentHealStaff { get; set; } = false;

		public bool? StartingEquipmentZeusGauntlet { get; set; } = false;

		public bool? StartingEquipmentWhiteShirt { get; set; } = false;

		public bool? StartingEquipmentRibbon { get; set; } = false;

		public bool? StartingEquipmentDragonslayer { get; set; } = false;

		public bool? StartingEquipmentLegendKit { get; set; } = false;

		public bool? StartingEquipmentRandomEndgameWeapon { get; set; } = false;

		public bool? StartingEquipmentRandomAoe { get; set; } = false;

		public bool? StartingEquipmentRandomCasterItem { get; set; } = false;

		public bool? StartingEquipmentGrandpasSecretStash { get; set; } = false;

		public bool? StartingEquipmentOneItem { get; set; } = false;

		public bool? StartingEquipmentRandomCrap { get; set; } = false;

		public bool? StartingEquipmentStarterPack { get; set; } = false;

		public bool? StartingEquipmentRandomTypeWeapon { get; set; } = false;

		public bool StartingEquipmentRemoveFromPool { get; set; } = false;

		public bool StartingEquipmentNoDuplicates { get; set; } = false;

		#endregion

		public bool? ReversedFloors { get; set; } = false;
		public bool BuffTier1DamageSpells { get; set; } = false;
		public bool NoEmptyScripts { get; set; } = false;
		public bool LaterLoose { get; set; } = false;
		public bool? MermaidPrison { get; set; } = false;

		public GuaranteedDefenseItem GuaranteedDefenseItem { get; set; } = GuaranteedDefenseItem.None;

		public GuaranteedPowerItem GuaranteedPowerItem { get; set; } = GuaranteedPowerItem.None;

		public ScriptTouchMultiplier ScriptMultiplier { get; set; } = ScriptTouchMultiplier.Vanilla;
		public ScriptTouchMultiplier TouchMultiplier { get; set; } = ScriptTouchMultiplier.Vanilla;
		public TouchPool TouchPool { get; set; } = TouchPool.All;
		public TouchMode TouchMode { get; set; } = TouchMode.Standard;
		public RibbonMode RibbonMode { get; set; } = RibbonMode.Vanilla;

		public bool Archipelago { get; set; } = false;
		public bool ArchipelagoGold { get; set; } = false;
		public bool ArchipelagoConsumables { get; set; } = false;
		public bool ArchipelagoShards { get; set; } = false;
		public ArchipelagoEquipment ArchipelagoEquipment { get; set; } = ArchipelagoEquipment.None;

		public ItemMagicMode ItemMagicMode { get; set; } = ItemMagicMode.Vanilla;
		public ItemMagicPool ItemMagicPool { get; set; } = ItemMagicPool.All;
		public bool? MagisizeWeapons { get; set; } = false;

		public bool DisableMinimap { get; set; } = false;

		public bool LooseItemsForwardPlacement { get; set; } = false;

		public bool LooseItemsSpreadPlacement { get; set; } = false;

		public bool LooseItemsNpcBalance { get; set; } = false;
		public bool AllowUnsafePlacement { get; set; } = false;
		public bool ShipCanalBeforeFloater { get; set; } = false;

		[IntegerFlag(0, 100, 10)]
		public int ExpChestConversionMin { get; set; } = 0;

		[IntegerFlag(0, 100, 10)]
		public int ExpChestConversionMax { get; set; } = 0;

		[IntegerFlag(0, 20000, 500)]
		public int ExpChestMinReward { get; set; } = 2000;

		[IntegerFlag(0, 20000, 500)]
		public int ExpChestMaxReward { get; set; } = 8000;

		public SpellNameMadness SpellNameMadness { get; set; } = SpellNameMadness.None;

		public ExtConsumableSet ExtConsumableSet { get; set; } = ExtConsumableSet.None;

		public bool ExtConsumablesEnabled => ExtConsumableSet != ExtConsumableSet.None;

		public bool EnableSoftInBattle { get; set; } = false;
		public LifeInBattleSetting EnableLifeInBattle { get; set; } = LifeInBattleSetting.LifeInBattleAll;

		public bool? NormalShopsHaveExtConsumables { get; set; } = false;

		public bool? LegendaryShopHasExtConsumables { get; set; } = false;

		public TreasureStackSize ExtConsumableTreasureStackSize { get; set; } = TreasureStackSize.Default;

		public ExtStartingItemSet ExtStartingItemSet { get; set; } = ExtStartingItemSet.None;

		public ExtConsumableChestSet ExtConsumableChests { get; set; } = ExtConsumableChestSet.None;

		public OwMapExchanges OwMapExchange { get; set; } = OwMapExchanges.None;
		public bool OwShuffledAccess { get; set; } = false;
		public bool OwUnsafeStart { get; set; } = false;
		public bool OwRandomPregen { get; set; } = false;

		public bool? RelocateChests { get; set; } = false;
		public bool RelocateChestsTrapIndicator { get; set; } = false;

		public bool? ShuffleChimeAccess { get; set; } = false;
		public bool? ShuffleChimeIncludeTowns { get; set; } = false;

		public GameModes GameMode { get; set; } = GameModes.Standard;

		[IntegerFlag(0, Int32.MaxValue-1)]
		public int MapGenSeed { get; set; } = 0;

		public OwMapExchangeData ReplacementMap { get; set; } = null;

		public string ResourcePack { get; set; } = null;

		#region ShopKiller

		public ShopKillMode ShopKillMode_Weapons { get; set; } = ShopKillMode.None;
		public ShopKillMode ShopKillMode_Armor { get; set; } = ShopKillMode.None;
		public ShopKillMode ShopKillMode_Item { get; set; } = ShopKillMode.None;
		public ShopKillMode ShopKillMode_Black { get; set; } = ShopKillMode.None;
		public ShopKillMode ShopKillMode_White { get; set; } = ShopKillMode.None;

		public ShopKillFactor ShopKillFactor_Weapons { get; set; } = ShopKillFactor.Kill20Percent;
		public ShopKillFactor ShopKillFactor_Armor { get; set; } = ShopKillFactor.Kill20Percent;
		public ShopKillFactor ShopKillFactor_Item { get; set; } = ShopKillFactor.Kill20Percent;
		public ShopKillFactor ShopKillFactor_Black { get; set; } = ShopKillFactor.Kill20Percent;
		public ShopKillFactor ShopKillFactor_White { get; set; } = ShopKillFactor.Kill20Percent;

		public bool ShopKillExcludeConeria_Weapons { get; set; } = false;
		public bool ShopKillExcludeConeria_Armor { get; set; } = false;
		public bool ShopKillExcludeConeria_Item { get; set; } = false;
		public bool ShopKillExcludeConeria_Black { get; set; } = false;
		public bool ShopKillExcludeConeria_White { get; set; } = false;

		#endregion

		public bool? ExcludeGoldFromScaling { get; set; } = false;
		public bool CheapVendorItem { get; set; } = false;

		public StartingLevel StartingLevel { get; set; }
		public TransmooglifierVariance TransmooglifierVariance { get; set; }

		[IntegerFlag(1, 50)]
		public int MaxLevelLow { get; set; } = 50;
		[IntegerFlag(1, 50)]
		public int MaxLevelHigh { get; set; } = 50;

		public bool Spoilers { get; set; } = false;
		public bool TournamentSafe { get; set; } = false;
		public bool BlindSeed { get; set; } = false;
		public bool? Shops { get; set; } = false;
		public bool? Treasures { get; set; } = false;
		public bool? NPCItems { get; set; } = false;
		public bool? NPCFetchItems { get; set; } = false;
		public bool? RandomWares { get; set; } = false;
		public bool? RandomWaresIncludesSpecialGear { get; set; } = false;
		public bool? RandomLoot { get; set; } = false;

		public bool ShardHunt { get; set; } = false;
		public ShardCount ShardCount { get; set; } = ShardCount.Count16;

		[IntegerFlag(0, 5)]
		public int OrbsRequiredCount { get; set; } = 4;
		public OrbsRequiredMode OrbsRequiredMode { get; set; } = OrbsRequiredMode.Any;
		public bool? OrbsRequiredSpoilers { get; set; } = false;
		public FinalFormation TransformFinalFormation { get; set; } = FinalFormation.None;
		public bool? ChaosRush { get; set; } = false;
		public ToFRMode ToFRMode { get; set; } = ToFRMode.Long;
		public FiendsRefights FiendsRefights { get; set; } = FiendsRefights.All;
		public bool? ExitToFR { get; set; } = false;
		public bool? ChaosFloorEncounters { get; set; } = false;
		public bool? MagicShops { get; set; } = false;
		public bool? MagicShopLocs { get; set; } = false;
		public bool? MagicShopLocationPairs { get; set; } = false;
		public bool? MagicLevels { get; set; } = false;
		public bool? MagicPermissions { get; set; } = false;
		public bool? Weaponizer { get; set; } = false;
		public bool? WeaponizerNamesUseQualityOnly { get; set; } = false;
		public bool? WeaponizerCommonWeaponsHavePowers { get; set; } = false;
		public bool? ArmorCrafter { get; set; } = false;
		public bool? MagicLevelsTiered { get; set; } = false;
		public bool? MagicLevelsMixed { get; set; } = false;

		public AutohitThreshold MagicAutohitThreshold { get; set; } = AutohitThreshold.Vanilla;

		public bool? Rng { get; set; } = false;
		public bool FixMissingBattleRngEntry { get; set; } = false;

		public bool? UnrunnableShuffle { get; set; } = true;
		[IntegerFlag(0, 100, 4)]
		public int UnrunnablesLow { get; set; } = 0;
		[IntegerFlag(0, 100, 4)]
		public int UnrunnablesHigh { get; set; } = 0;
		public bool? EnemyFormationsSurprise { get; set; } = false;
		public bool? UnrunnablesStrikeFirstAndSurprise { get; set; } = false;

		public TrapTileMode EnemyTrapTiles { get; set; } = TrapTileMode.Vanilla;
		public FormationPool TCFormations { get; set; } = FormationPool.AltFormationDist;
		public TCOptions TCBetterTreasure { get; set; } = TCOptions.None;
		public TCOptions TCKeyItems { get; set; } = TCOptions.None;
		public TCOptions TCShards { get; set; } = TCOptions.None;
		public bool TCExcludeCommons { get; set; } = false;

		[IntegerFlag(0, 13)]
		public int TCChestCount { get; set; } = 0;
		public bool TCProtectIncentives { get; set; } = false;
		public bool? TCMasaGuardian { get; set; } = false;
		public bool? TrappedChaos { get; set; } = false;
		public bool? TCIndicator { get; set; } = false;
		public bool? SwolePirates { get; set; } = false;
		public bool? ShuffleScriptsEnemies { get; set; } = false;
		public bool? RemoveBossScripts { get; set; } = false;
		public bool? ShuffleScriptsBosses { get; set; } = false;
		public bool? ShuffleSkillsSpellsEnemies { get; set; } = false;
		public bool? ShuffleSkillsSpellsBosses { get; set; } = false;
		public bool? NoConsecutiveNukes { get; set; } = false;
		public bool TranceHasStatusElement { get; set; } = false;
		public bool? EnemySkillsSpellsTiered { get; set; } = false;
		public bool? AllowUnsafePirates { get; set; } = false;
		public bool? AllowUnsafeMelmond { get; set; } = false;

		public WarMECHMode WarMECHMode { get; set; } = WarMECHMode.Vanilla;
		public bool? OrdealsPillars { get; set; } = false;
		public bool? ShuffleLavaTiles { get; set; } = false;
		public SkyCastle4FMazeMode SkyCastle4FMazeMode { get; set; } = SkyCastle4FMazeMode.Vanilla;
		public bool? TitansTrove { get; set; } = false;
		public bool? LefeinSuperStore { get; set; } = false;
		public bool? LefeinShops { get; set; } = false;
		public bool? RandomVampAttack { get; set; } = false;
		public bool? RandomVampAttackIncludesConeria { get; set; } = false;
		public bool? FightBahamut { get; set; } = false;
		public bool? SwoleBahamut { get; set; } = false;
		public bool? SwoleAstos { get; set; } = false;
		public bool? ConfusedOldMen { get; set; } = false;
		public bool? GaiaShortcut { get; set; } = false;

		[IntegerFlag(0, 10, 1)]
		public int DamageTileLow { get; set; } = 1;
		[IntegerFlag(0, 10, 1)]
		public int DamageTileHigh { get; set; } = 1;
		public bool? OWDamageTiles { get; set; } = false;
		public bool? DamageTilesKill { get; set; } = false;
		public bool? ArmorResistsDamageTileDamage { get; set; } = false;

		public bool? MoveGaiaItemShop { get; set; } = false;
		public bool? ShufflePravokaShops { get; set; } = false;
		public bool? FlipDungeons { get; set; } = false;
		public bool? VerticallyFlipDungeons { get; set; } = false;
		public bool SpookyFlag { get; set; } = false;
		public bool DraculasFlag { get; set; } = false;
		public bool? MapOpenProgression { get; set; } = false;
		public bool? MapOpenProgressionDocks { get; set; } = false;
		public bool? Entrances { get; set; } = false;
		public bool? Towns { get; set; } = false;
		public bool? IncludeConeria { get; set; } = false;
		public bool? Floors { get; set; } = false;
		public bool? AllowDeepCastles { get; set; } = false;
		public bool? AllowDeepTowns { get; set; } = false;
		public bool? MapOpenProgressionExtended { get; set; } = false;
		public bool? MapAirshipDock { get; set; } = false;
		public bool? MapBahamutCardiaDock  { get; set; } = false;
		public bool? MapLefeinRiver  { get; set; } = false;
		public bool? MapBridgeLefein { get; set; } = false;
		public bool? MapRiverToMelmond { get; set; } = false;
		public bool? MapGaiaMountainPass { get; set; } = false;
		public bool? MapHighwayToOrdeals { get; set; } = false;
		public bool? MapDragonsHoard { get; set; } = false;
		public bool? MapHallOfDragons { get; set; } = false;
		public bool? EntrancesIncludesDeadEnds { get; set; } = false;
		public bool? EntrancesMixedWithTowns { get; set; } = false;

		public bool? IncentivizeFreeNPCs { get; set; } = false;
		public bool? IncentivizeFetchNPCs { get; set; } = false;
		public bool? IncentivizeTail { get; set; } = false;
		public bool? IncentivizeMainItems { get; set; } = false;
		public bool? IncentivizeFetchItems { get; set; } = false;
		public bool? IncentivizeCanoeItem { get; set; } = false;
		public bool? IncentivizeAirship { get; set; } = false;
		public bool? IncentivizeShipAndCanal { get; set; } = false;
		public bool? IncentivizeBridgeItem { get; set; } = false;

		public bool? IncentivizeMarsh { get; set; } = false;
		public bool? IncentivizeEarth { get; set; } = false;
		public bool? IncentivizeVolcano { get; set; } = false;
		public bool? IncentivizeIceCave { get; set; } = false;
		public bool? IncentivizeOrdeals { get; set; } = false;
		public bool? IncentivizeSeaShrine { get; set; } = false;
		public bool? IncentivizeConeria { get; set; } = false;
		public bool? IncentivizeMarshKeyLocked { get; set; } = false;
		public bool? IncentivizeSkyPalace { get; set; } = false;
		public bool? IncentivizeTitansTrove { get; set; } = false;
		public bool? IncentivizeCardia { get; set; } = false;

		public IncentivePlacementType IceCaveIncentivePlacementType { get; set; } = IncentivePlacementType.Vanilla;
		public IncentivePlacementType OrdealsIncentivePlacementType { get; set; } = IncentivePlacementType.Vanilla;
		public IncentivePlacementType MarshIncentivePlacementType { get; set; } = IncentivePlacementType.Vanilla;
		public IncentivePlacementType TitansIncentivePlacementType { get; set; } = IncentivePlacementType.Vanilla;
		public IncentivePlacementTypeEarth EarthIncentivePlacementType { get; set; } = IncentivePlacementTypeEarth.Vanilla;
		public IncentivePlacementTypeVolcano VolcanoIncentivePlacementType { get; set; } = IncentivePlacementTypeVolcano.Vanilla;
		public IncentivePlacementTypeSea SeaShrineIncentivePlacementType { get; set; } = IncentivePlacementTypeSea.Vanilla;
		public IncentivePlacementTypeSky SkyPalaceIncentivePlacementType { get; set; } = IncentivePlacementTypeSky.Vanilla;
		public IncentivePlacementType CorneriaIncentivePlacementType { get; set; } = IncentivePlacementType.Vanilla;
		public IncentivePlacementType MarshLockedIncentivePlacementType { get; set; } = IncentivePlacementType.Vanilla;
		public IncentivePlacementType CardiaIncentivePlacementType { get; set; } = IncentivePlacementType.Vanilla;

		public bool? BetterTrapChests { get; set; } = false;
		public bool? IncentivizeMasamune { get; set; } = false;
		public bool? IncentivizeKatana { get; set; } = false;
		public bool? IncentivizeXcalber { get; set; } = false;
		public bool? IncentivizeVorpal { get; set; } = false;
		public bool? IncentivizeOpal { get; set; } = false;
		public bool? IncentivizeRibbon { get; set; } = false;
		public bool IncentivizeRibbon2 => false;
		public bool Incentivize65K => false;
		public bool IncentivizeBad => false;

		public bool? IncentivizeDefCastArmor { get; set; } = false;
		public bool? IncentivizeOffCastArmor { get; set; } = false;
		public bool? IncentivizeOtherCastArmor { get; set; } = false;
		public bool? IncentivizePowerRod { get; set; } = false;
		public bool? IncentivizeDefCastWeapon { get; set; } = false;
		public bool? IncentivizeOffCastWeapon { get; set; } = false;
		public bool IncentivizeOtherCastWeapon { get; set; } = false;
		public bool? LooseExcludePlacedDungeons { get; set; } = false;
		public bool? EarlyKing { get; set; } = false;
		public bool? EarlySarda { get; set; } = false;
		public bool? EarlySage { get; set; } = false;
		public bool? EarlyOrdeals { get; set; } = false;
		public bool? ShuffleObjectiveNPCs { get; set; } = false;
		public bool OnlyRequireGameIsBeatable { get; set; } = true;
		public bool? FreeBridge { get; set; } = false;
		public bool? FreeShip { get; set; } = false;
		public bool? FreeAirship { get; set; } = false;
		public bool? FreeLute { get; set; } = false;
		public bool? FreeRod { get; set; } = false;
		public bool EnableCritNumberDisplay { get; set; } = false;
		public bool? FreeCanal { get; set; } = false;
		public bool? FreeCanoe { get; set; } = false;
		public bool EasyMode { get; set; } = false;

		public bool HousesFillHp { get; set; } = false;

		public bool SpeedHacks { get; set; } = false;
		public bool NoPartyShuffle { get; set; } = false;
		public bool Dash { get; set; } = false;
		public bool SpeedBoat { get; set; } = false;
		public bool? AirBoat { get; set; } = false;
		public bool BuyTen { get; set; } = false;
		public bool IdentifyTreasures { get; set; } = false;
		public bool ShopInfo { get; set; } = false;
		public bool ChestInfo { get; set; } = false;
		public bool IncentiveChestItemsFanfare { get; set; } = false;
		public bool WaitWhenUnrunnable { get; set; } = false;
		public bool ImprovedClinic { get; set; } = false;
		public bool Etherizer { get; set; } = false;
		// Done
		public bool HouseMPRestoration { get; set; } = false;
		public bool WeaponStats { get; set; } = false;
		public bool BBCritRate { get; set; } = false;
		public bool WeaponCritRate { get; set; } = false;
		public bool WeaponBonuses { get; set; } = false;
		public ThiefAGI ThiefAgilityBuff { get; set; } = ThiefAGI.Vanilla;
		public SpoilerBatHints SkyWarriorSpoilerBats { get; set; } = SpoilerBatHints.Vanilla;
		public bool? SpoilerBatsDontCheckOrbs { get; set; } = false;
		public bool? MoveToFBats { get; set; } = false;

		[IntegerFlag(0, 50)]
		public int WeaponTypeBonusValue { get; set; } = 10;

		public ChanceToRunMode ChanceToRun { get; set; } = ChanceToRunMode.Vanilla;

		public bool SpellBugs { get; set; } = false;
		public bool BlackBeltAbsorb { get; set; } = false;
		public bool NPCSwatter { get; set; } = false;
		public bool BattleMagicMenuWrapAround { get; set; } = false;
		public bool MagicMenuSpellReordering { get; set; } = false;
		public bool InventoryAutosort { get; set; } = false;
		public bool RepeatedHealPotionUse { get; set; } = false;
		public bool AutoRetargeting { get; set; } = false;
		public bool EnemyStatusAttackBug { get; set; } = false;
		public bool ImproveTurnOrderRandomization { get; set; } = false;
		public bool FixHitChanceCap { get; set; } = false;

		public bool? MelmondClinic { get; set; } = false;
		public bool DDProgressiveTilesets { get; set; } = false;
		public bool DDFiendOrbs { get; set; } = false;
		public TailBahamutMode TailBahamutMode { get; set; } = TailBahamutMode.Random;
		public StartingGold StartingGold { get; set; } = StartingGold.Gp400;
		public bool IncludeMorale { get; set; } = false;
		public bool DeadsGainXP { get; set; } = false;
		public bool NonesGainXP { get; set; } = false;
		public bool? NoTail { get; set; } = false;
		public bool? NoFloater { get; set; } = false;
		public bool? GuaranteedMasamune { get; set; } = false;
		public bool? SendMasamuneHome { get; set; } = false;

		public ConsumableChestSet MoreConsumableChests { get; set; } = ConsumableChestSet.Vanilla;

		public bool? NoMasamune { get; set; } = false;
		public bool? NoXcalber { get; set; } = false;
		public bool? ClassAsNpcFiends { get; set; } = false;
		public bool? ClassAsNpcKeyNPC { get; set; } = false;

		[IntegerFlag(0, 13)]
		public int ClassAsNpcCount { get; set; } = 6;
		public bool? ClassAsNpcDuplicate { get; set; } = false;
		public bool? ClassAsNpcForcedFiends { get; set; } = false;
		public bool? ClassAsNpcPromotion { get; set; } = false;

		[IntegerFlag(0, 500, 10)]
		public int BossScaleStatsLow { get; set; } = 50;

		[IntegerFlag(0, 500, 10)]
		public int BossScaleStatsHigh { get; set; } = 200;

		[IntegerFlag(0, 500, 10)]
		public int BossScaleHpLow { get; set; } = 50;

		[IntegerFlag(0, 500, 10)]
		public int BossScaleHpHigh { get; set; } = 200;

		[IntegerFlag(0, 500, 10)]
		public int EnemyScaleStatsLow { get; set; } = 50;

		[IntegerFlag(0, 500, 10)]
		public int EnemyScaleStatsHigh { get; set; } = 200;

		[IntegerFlag(0, 500, 10)]
		public int EnemyScaleHpLow { get; set; } = 50;

		[IntegerFlag(0, 500, 10)]
		public int EnemyScaleHpHigh { get; set; } = 200;

		[IntegerFlag(0, 500, 10)]
		public int PriceScaleFactorLow { get; set; } = 50;

		[IntegerFlag(0, 500, 10)]
		public int PriceScaleFactorHigh { get; set; } = 200;

		[DoubleFlag(1.0, 5.0, 0.1)]
		public double ExpMultiplier { get; set; } = 0;

		[IntegerFlag(0, 250, 25)]
		public int ExpBonus { get; set; } = 0;

		[DoubleFlag(0.5, 3.0, 0.1)]
		public double ExpMultiplierFighter { get; set; } = 1.0;

		[DoubleFlag(0.5, 3.0, 0.1)]
		public double ExpMultiplierThief { get; set; } = 1.0;

		[DoubleFlag(0.5, 3.0, 0.1)]
		public double ExpMultiplierBlackBelt { get; set; } = 1.0;

		[DoubleFlag(0.5, 3.0, 0.1)]
		public double ExpMultiplierRedMage { get; set; } = 1.0;

		[DoubleFlag(0.5, 3.0, 0.1)]
		public double ExpMultiplierWhiteMage { get; set; } = 1.0;

		[DoubleFlag(0.5, 3.0, 0.1)]
		public double ExpMultiplierBlackMage { get; set; } = 1.0;

		[DoubleFlag(0, 45)]
		public double EncounterRate { get; set; } = 0;

		[DoubleFlag(0, 45)]
		public double DungeonEncounterRate { get; set; } = 0;
		public ProgressiveScaleMode ProgressiveScaleMode { get; set; } = ProgressiveScaleMode.Disabled;

		public StartingItemSet StartingItemSet { get; set; } = StartingItemSet.None;

		public TreasureStackSize ConsumableTreasureStackSize { get; set; } = TreasureStackSize.Default;

		public bool? FIGHTER1 { get; set; } = false;
		public bool? THIEF1 { get; set; } = false;
		public bool? BLACK_BELT1 { get; set; } = false;
		public bool? RED_MAGE1 { get; set; } = false;
		public bool? WHITE_MAGE1 { get; set; } = false;
		public bool? BLACK_MAGE1 { get; set; } = false;

		public bool? FIGHTER2 { get; set; } = false;
		public bool? THIEF2 { get; set; } = false;
		public bool? BLACK_BELT2 { get; set; } = false;
		public bool? RED_MAGE2 { get; set; } = false;
		public bool? WHITE_MAGE2 { get; set; } = false;
		public bool? BLACK_MAGE2 { get; set; } = false;

		public bool? FIGHTER3 { get; set; } = false;
		public bool? THIEF3 { get; set; } = false;
		public bool? BLACK_BELT3 { get; set; } = false;
		public bool? RED_MAGE3 { get; set; } = false;
		public bool? WHITE_MAGE3 { get; set; } = false;
		public bool? BLACK_MAGE3 { get; set; } = false;

		public bool? FIGHTER4 { get; set; } = false;
		public bool? THIEF4 { get; set; } = false;
		public bool? BLACK_BELT4 { get; set; } = false;
		public bool? RED_MAGE4 { get; set; } = false;
		public bool? WHITE_MAGE4 { get; set; } = false;
		public bool? BLACK_MAGE4 { get; set; } = false;

		public bool? KNIGHT1 { get; set; } = false;
		public bool? KNIGHT2 { get; set; } = false;
		public bool? KNIGHT3 { get; set; } = false;
		public bool? KNIGHT4 { get; set; } = false;
		public bool? NINJA1 { get; set; } = false;
		public bool? NINJA2 { get; set; } = false;
		public bool? NINJA3 { get; set; } = false;
		public bool? NINJA4 { get; set; } = false;
		public bool? MASTER1 { get; set; } = false;
		public bool? MASTER2 { get; set; } = false;
		public bool? MASTER3 { get; set; } = false;
		public bool? MASTER4 { get; set; } = false;
		public bool? RED_WIZ1 { get; set; } = false;
		public bool? RED_WIZ2 { get; set; } = false;
		public bool? RED_WIZ3 { get; set; } = false;
		public bool? RED_WIZ4 { get; set; } = false;
		public bool? WHITE_WIZ1 { get; set; } = false;
		public bool? WHITE_WIZ2 { get; set; } = false;
		public bool? WHITE_WIZ3 { get; set; } = false;
		public bool? WHITE_WIZ4 { get; set; } = false;
		public bool? BLACK_WIZ1 { get; set; } = false;
		public bool? BLACK_WIZ2 { get; set; } = false;
		public bool? BLACK_WIZ3 { get; set; } = false;
		public bool? BLACK_WIZ4 { get; set; } = false;
		public bool? NONE_CLASS2 { get; set; } = false;
		public bool? NONE_CLASS3 { get; set; } = false;
		public bool? NONE_CLASS4 { get; set; } = false;
		public bool? FORCED1 { get; set; } = false;
		public bool? FORCED2 { get; set; } = false;
		public bool? FORCED3 { get; set; } = false;
		public bool? FORCED4 { get; set; } = false;
		public bool? DraftFighter { get; set; } = false;
		public bool? DraftThief { get; set; } = false;
		public bool? DraftBlackBelt { get; set; } = false;
		public bool? DraftRedMage { get; set; } = false;
		public bool? DraftWhiteMage { get; set; } = false;
		public bool? DraftBlackMage { get; set; } = false;
		public bool? DraftKnight { get; set; } = false;
		public bool? DraftNinja { get; set; } = false;
		public bool? DraftMaster { get; set; } = false;
		public bool? DraftRedWiz { get; set; } = false;
		public bool? DraftWhiteWiz { get; set; } = false;
		public bool? DraftBlackWiz { get; set; } = false;
		public bool? TAVERN1 { get; set; } = false;
		public bool? TAVERN2 { get; set; } = false;
		public bool? TAVERN3 { get; set; } = false;
		public bool? TAVERN4 { get; set; } = false;
		public bool? TAVERN5 { get; set; } = false;
		public bool? TAVERN6 { get; set; } = false;

		public bool WeaponPermissions { get; set; } = false;
		public bool ArmorPermissions { get; set; } = false;
		public bool? RecruitmentMode { get; set; } = false;
		public bool? RecruitmentModeHireOnly { get; set; } = false;
		public bool? RecruitmentModeReplaceOnlyNone { get; set; } = false;
		public bool? ClampMinimumStatScale { get; set; } = false;
		public bool? ClampMinimumBossStatScale { get; set; } = false;
		public bool? ClampMinimumPriceScale { get; set; } = false;
		public bool EFGWaterfall { get; set; } = false;
		public bool? FiendShuffle { get; set; } = false;
		public bool DisableTentSaving { get; set; } = false;
		public bool DisableInnSaving { get; set; } = false;
		public bool SaveGameWhenGameOver { get; set; } = false;
		public bool SaveGameDWMode { get; set; } = false;
		public bool? ShuffleAstos { get; set; } = false;
		public bool UnsafeAstos { get; set; } = false;
		public bool? RandomizeEnemizer { get; set; } = false;
		public bool? RandomizeFormationEnemizer { get; set; } = false;
		public bool? GenerateNewSpellbook { get; set; } = false;
		public bool? SpellcrafterMixSpells { get; set; } = false;
		public bool ThiefHitRate { get; set; } = false;
		public bool AllSpellLevelsForKnightNinja { get; set; } = false;
		public bool BuffHealingSpells { get; set; } = false;
		public bool IntAffectsSpells { get; set; } = false;
		public bool? AddDamageTiles { get; set; } = false;
		public bool? DamageTilesCastles { get; set; } = false;
		public bool? DamageTilesDungeons { get; set; } = false;
		public bool? DamageTilesCaves { get; set; } = false;
		public bool? DamageTilesTowns { get; set; } = false;
		public bool? DamageTilesTof { get; set; } = false;
		public DamageTileStrategies DamageTileStrategy { get; set; } = DamageTileStrategies.ConditionalNeighbors;
		public bool? FreeTail { get; set; } = false;
		public bool? HintsVillage { get; set; } = false;
		public bool? SpellcrafterRetainPermissions { get; set; } = false;
		public bool? RandomWeaponBonus { get; set; } = false;
		public bool? RandomArmorBonus { get; set; } = false;
		public bool? RandomWeaponBonusExcludeMasa { get; set; } = false;

		[IntegerFlag(-9, 9)]
		public int RandomWeaponBonusLow { get; set; } = -5;

		[IntegerFlag(-9, 9)]
		public int RandomWeaponBonusHigh { get; set; } = 5;

		[IntegerFlag(-9, 9)]
		public int RandomArmorBonusLow { get; set; } = -5;

		[IntegerFlag(-9, 9)]
		public int RandomArmorBonusHigh { get; set; } = 5;
		public bool? SeparateBossHPScaling { get; set; } = false;
		public bool? SeparateEnemyHPScaling { get; set; } = false;
		public bool? ClampBossHPScaling { get; set; } = false;
		public bool? ClampEnemyHpScaling { get; set; } = false;
		public PoolSize PoolSize { get; set; } = PoolSize.Size6;
		public bool? EnablePoolParty { get; set; } = false;
		public bool SafePoolParty { get; set; } = false;
		public bool? IncludePromClasses { get; set; } = false;
		public bool? EnableRandomPromotions { get; set; } = false;
		public bool? IncludeBaseClasses { get; set; } = false;
		public bool? RandomPromotionsSpoilers { get; set; } = false;
		public bool? RandomizeClassCasting { get; set; } = false;
		public bool? RandomizeClassKeyItems { get; set; } = false;
		public bool? RandomizeClassIncludeXpBonus { get; set; } = false;
		public bool? AlternateFiends { get; set; } = false;
		public bool? FinalFantasy2Fiends { get; set; } = false;
		public bool? FinalFantasy3Fiends { get; set; } = false;
		public bool? FinalFantasy4Fiends { get; set; } = false;
		public bool? FinalFantasy5Fiends { get; set; } = false;
		public bool? FinalFantasy6Fiends { get; set; } = false;
		public bool? FinalFantasy1BonusFiends { get; set; } = false;
 		public bool? NoBossSkillScriptShuffle { get; set; } = false;

		public bool? MooglieWeaponBalance { get; set; } = false;
		public bool? GuaranteeCustomClassComposition { get; set; } = false;

		public bool? LegendaryWeaponShop { get; set; } = false;
		public bool? LegendaryArmorShop { get; set; } = false;
		public bool? LegendaryBlackShop { get; set; } = false;
		public bool? LegendaryWhiteShop { get; set; } = false;
		public bool? LegendaryItemShop { get; set; } = false;

		public bool ExclusiveLegendaryWeaponShop { get; set; } = false;
		public bool ExclusiveLegendaryArmorShop { get; set; } = false;
		public bool ExclusiveLegendaryBlackShop { get; set; } = false;
		public bool ExclusiveLegendaryWhiteShop { get; set; } = false;
		public bool ExclusiveLegendaryItemShop { get; set; } = false;
		public ClassRandomizationMode RandomizeClassMode { get; set; } = ClassRandomizationMode.None;

		[IntegerFlag(0, 3)]
		public int RandomizeClassMaxBonus { get; set; } = 2;

		[IntegerFlag(0, 3)]
		public int RandomizeClassMaxMalus { get; set; } = 1;
		public bool? EarlierHighTierMagic { get; set; } = false;
		public bool? ChangeMaxMP { get; set; } = false;

		[IntegerFlag(0, 9)]
		public int RedMageMaxMP { get; set; } = 9;

		[IntegerFlag(0, 9)]
		public int WhiteMageMaxMP { get; set; } = 9;

		[IntegerFlag(0, 9)]
		public int BlackMageMaxMP { get; set; } = 9;

		[IntegerFlag(0, 9)]
		public int KnightMaxMP { get; set; } = 4;

		[IntegerFlag(0, 9)]
		public int NinjaMaxMP { get; set; } = 4;

		public bool? Knightlvl4 { get; set; } = false;
		public bool? PinkMage { get; set; } = false;
		public bool? BlackKnight { get; set; } = false;
		public bool? BlackKnightKeep { get; set; } = false;
		public bool? WhiteNinja { get; set; } = false;
		public bool? WhiteNinjaKeep { get; set; } = false;

		public MpGainOnMaxGain MpGainOnMaxGainMode { get; set; } = MpGainOnMaxGain.None;

		public LockHitMode LockMode { get; set; } = LockHitMode.Vanilla;
		// Done
		public MDEFGrowthMode MDefMode { get; set; } = MDEFGrowthMode.None;

		public FormationShuffleMode FormationShuffleMode { get; set; } = FormationShuffleMode.None;

		public RandomizeTreasureMode RandomizeTreasure { get; set; } = RandomizeTreasureMode.None;
		public bool OpenChestsInOrder { get; set; } = false;
		public bool SetRNG { get; set; } = false;
		public WorldWealthMode WorldWealth { get; set; } = WorldWealthMode.Standard;
		public DeepDungeonGeneratorMode DeepDungeonGenerator { get; set; } = DeepDungeonGeneratorMode.Progressive;
		public EvadeCapValues EvadeCap { get; set; } = EvadeCapValues.medium;

		public bool? AllowUnsafeStartArea { get; set; } = false;

		public bool? IncreaseDarkPenalty { get; set; } = false;
		public PoisonModeOptions PoisonMode { get; set; } = PoisonModeOptions.Vanilla;

		public bool? TouchIncludeBosses { get; set; } = false;

		public bool? Lockpicking { get; set; } = false;
		// Done
		public bool? ReducedLuck { get; set; } = false;

		[IntegerFlag(1, 50)]
		public int LockpickingLevelRequirement { get; set; } = 10;

		public bool WhiteMageHarmEveryone { get; set; } = false;

		public bool? ProcgenEarth { get; set; } = false;

		public bool? EarlierRuby { get; set; } = false;
		public bool? MapCanalBridge => ((NPCItems) | (NPCFetchItems) | MapOpenProgression | MapOpenProgressionExtended) & (!DesertOfDeath);
		public bool DisableOWMapModifications => GameMode == GameModes.Standard && OwMapExchange != OwMapExchanges.None;

		// The philosophy governing item incentivizations works something like this:
		// 1. If the item is NOT being shuffled to another location it cannot be incentivized. (Duh)
		// 2. If the item is required to unlock any location OR is given to a not-shuffled NPC who gives
		//    such an item in return it is considered a MAIN item. (e.g. CROWN/SLAB with vanilla fetch quests)
		// 3. If the item is given to an NPC who themselves is shuffled it's considered a FETCH item.
		// 4. The vehicles now have their own incentivization flags apart from other progression items.

		// Ruby is required if Sarda is Required for the ROD
		public bool? RequiredRuby => !EarlySage & !NPCItems & !IsAirshipFree;
		public bool? UselessRuby => IsAirshipFree & !TitansTrove;
		public bool? IncentivizeRuby => (RequiredRuby & IncentivizeMainItems) | (!RequiredRuby & IncentivizeFetchItems & !UselessRuby);

		// If Canoe and Fetch Quests are unshuffled and there is no free canal or airship then TNT is required
		public bool? RequiredTnt => !NPCFetchItems & !NPCItems & !(IsCanalFree | IsAirshipFree);
		// If Fetch Items are vanilla and the player has a free Canal, do not incentivize TNT even if Other Quest Items are in the pool since there would be absolutely nothing to gain from TNT
		public bool? UselessTnt => !NPCFetchItems & (IsCanalFree | (IsAirshipFree & !MapOpenProgression));
		public bool? IncentivizeTnt => (RequiredTnt & IncentivizeMainItems) | (!RequiredTnt & IncentivizeFetchItems & !UselessTnt);

		public bool? IncentivizeCrown => (!(NPCFetchItems ?? false) && (IncentivizeMainItems ?? false)) || ((NPCFetchItems ?? false) && (IncentivizeFetchItems ?? false));
		public bool? IncentivizeSlab => (!(NPCFetchItems ?? false) && (IncentivizeMainItems ?? false)) || ((NPCFetchItems ?? false) && (IncentivizeFetchItems ?? false));
		public bool? IncentivizeBottle => (!(NPCFetchItems ?? false) && (IncentivizeMainItems ?? false)) || ((NPCFetchItems ?? false) && (IncentivizeFetchItems ?? false));
		public bool NoOverworld => GameMode == GameModes.NoOverworld;
		public bool DesertOfDeath => (GameMode == GameModes.Standard & OwMapExchange == OwMapExchanges.Desert);
		public bool? IsShipFree => FreeShip | NoOverworld;
		public bool? IsCanoeFree => FreeCanoe | DesertOfDeath;
		public bool? IsAirshipFree => FreeAirship & !NoOverworld & !DesertOfDeath;
		public bool? IsBridgeFree => FreeBridge | NoOverworld | DesertOfDeath;
		public bool? IsCanalFree => (FreeCanal & !NoOverworld) | DesertOfDeath;
		public bool? IsFloaterRemoved => ((NoFloater|IsAirshipFree) & !NoOverworld) | DesertOfDeath;
		public bool? IncentivizeBridge => NPCItems & IncentivizeBridgeItem & !IsBridgeFree & !NoOverworld;
		public bool? IncentivizeCanoe => NPCItems & IncentivizeCanoeItem & !FreeCanoe & !DesertOfDeath;
		public bool? IncentivizeLute => NPCItems & !FreeLute & IncentivizeMainItems;
		public bool? IncentivizeShip => NPCItems & IncentivizeShipAndCanal & !IsShipFree & !NoOverworld;
		public bool? IncentivizeRod => NPCItems & !FreeRod & IncentivizeMainItems ;
		public bool? IncentivizeCube => NPCItems & IncentivizeMainItems;
		public bool? IncentivizeFloater => !IsAirshipFree & !IsFloaterRemoved & IncentivizeAirship;
		public bool? IncentivizePromotion => !FreeTail & !NoTail & IncentivizeTail;

		public bool? IncentivizeCanal => NPCFetchItems & IncentivizeShipAndCanal & !IsCanalFree & !NoOverworld;
		public bool? IncentivizeCrystal => NPCFetchItems & IncentivizeFetchItems;
		public bool? IncentivizeHerb => NPCFetchItems & IncentivizeFetchItems;
		public bool? IncentivizeKey => NPCFetchItems & IncentivizeMainItems;
		public bool? IncentivizeChime => NPCFetchItems & IncentivizeMainItems;
		public bool? IncentivizeOxyale => NPCFetchItems & IncentivizeMainItems;

		public bool? IncentivizeAdamant => IncentivizeFetchItems;

		public int IncentivizedItemCountMin => 0
			+ ((IncentivizePromotion ?? false) ? 1 : 0)
			+ ((IncentivizeMasamune & !NoMasamune ?? false) ? 1 : 0)
			+ ((IncentivizeKatana ?? false) ? 1 : 0)
			+ ((IncentivizeVorpal ?? false) ? 1 : 0)
			+ ((IncentivizeOpal ?? false) ? 1 : 0)
			+ ((IncentivizeRibbon ?? false) ? 1 : 0)
			+ ((IncentivizeDefCastArmor ?? false) ? 1 : 0)
			+ ((IncentivizeOffCastArmor ?? false) ? 1 : 0)
			+ ((IncentivizeOtherCastArmor ?? false) ? 1 : 0)
			+ ((IncentivizePowerRod ?? false) ? 1 : 0)
			+ ((IncentivizeDefCastWeapon ?? false) ? 1 : 0)
			+ ((IncentivizeOffCastWeapon ?? false) ? 1 : 0)
			+ (IncentivizeOtherCastWeapon ? 1 : 0)
			+ ((IncentivizeAdamant ?? false) ? 1 : 0)
			+ ((IncentivizeRuby ?? false) ? 1 : 0)
			+ ((IncentivizeCrown ?? false) ? 1 : 0)
			+ ((IncentivizeTnt ?? false) ? 1 : 0)
			+ ((IncentivizeSlab ?? false) ? 1 : 0)
			+ ((IncentivizeBottle ?? false) ? 1 : 0)
			+ ((IncentivizeFloater ?? false) ? 1 : 0)
			+ ((IncentivizeBridge ?? false) ? 1 : 0)
			+ ((IncentivizeLute ?? false) ? 1 : 0)
			+ ((IncentivizeShip ?? false) ? 1 : 0)
			+ ((IncentivizeRod ?? false) ? 1 : 0)
			+ ((IncentivizeCanoe ?? false) ? 1 : 0)
			+ ((IncentivizeCube ?? false) ? 1 : 0)
			+ ((IncentivizeCrystal ?? false) ? 1 : 0)
			+ ((IncentivizeHerb ?? false) ? 1 : 0)
			+ ((IncentivizeKey ?? false) ? 1 : 0)
			+ ((IncentivizeCanal ?? false) ? 1 : 0)
			+ ((IncentivizeChime ?? false) ? 1 : 0)
			+ ((IncentivizeOxyale ?? false) ? 1 : 0)
			+ ((IncentivizeXcalber ?? false) ? 1 : 0);

		public int IncentivizedItemCountMax => 0
			+ ((IncentivizePromotion ?? true) ? 1 : 0)
			+ ((IncentivizeMasamune & !NoMasamune ?? true) ? 1 : 0)
			+ ((IncentivizeKatana ?? true) ? 1 : 0)
			+ ((IncentivizeVorpal ?? true) ? 1 : 0)
			+ ((IncentivizeOpal ?? true) ? 1 : 0)
			+ ((IncentivizeRibbon ?? true) ? 1 : 0)
			+ ((IncentivizeDefCastArmor ?? true) ? 1 : 0)
			+ ((IncentivizeOffCastArmor ?? true) ? 1 : 0)
			+ ((IncentivizeOtherCastArmor ?? true) ? 1 : 0)
			+ ((IncentivizePowerRod ?? true) ? 1 : 0)
			+ ((IncentivizeDefCastWeapon ?? true) ? 1 : 0)
			+ ((IncentivizeOffCastWeapon ?? true) ? 1 : 0)
			+ (IncentivizeOtherCastWeapon ? 1 : 0)
			+ ((IncentivizeAdamant ?? true) ? 1 : 0)
			+ ((IncentivizeRuby ?? true) ? 1 : 0)
			+ ((IncentivizeCrown ?? true) ? 1 : 0)
			+ ((IncentivizeTnt ?? true) ? 1 : 0)
			+ ((IncentivizeSlab ?? true) ? 1 : 0)
			+ ((IncentivizeBottle ?? true) ? 1 : 0)
			+ ((IncentivizeFloater ?? true) ? 1 : 0)
			+ ((IncentivizeBridge ?? true) ? 1 : 0)
			+ ((IncentivizeLute ?? true) ? 1 : 0)
			+ ((IncentivizeShip ?? true) ? 1 : 0)
			+ ((IncentivizeRod ?? true) ? 1 : 0)
			+ ((IncentivizeCanoe ?? true) ? 1 : 0)
			+ ((IncentivizeCube ?? true) ? 1 : 0)
			+ ((IncentivizeCrystal ?? true) ? 1 : 0)
			+ ((IncentivizeHerb ?? true) ? 1 : 0)
			+ ((IncentivizeKey ?? true) ? 1 : 0)
			+ ((IncentivizeCanal ?? true) ? 1 : 0)
			+ ((IncentivizeChime ?? true) ? 1 : 0)
			+ ((IncentivizeOxyale ?? true) ? 1 : 0)
			+ ((IncentivizeXcalber ?? true) ? 1 : 0);

		public string IncentivizedItems => ""
			+ ((IncentivizeAdamant != null) ? (IncentivizeAdamant ?? false ? "Adamant " : "") : ("Adamant? "))
			+ ((IncentivizeBridge != null) ? (IncentivizeBridge ?? false ? "Bridge " : "") : ("Bridge? "))
			+ ((IncentivizeBottle != null) ? (IncentivizeBottle ?? false ? "Bottle " : "") : ("Bottle? "))
			+ ((IncentivizeCanal != null) ? (IncentivizeCanal ?? false ? "Canal " : "") : ("Canal? "))
			+ ((IncentivizeCanoe != null) ? (IncentivizeCanoe ?? false ? "Canoe " : "") : ("Canoe? "))
			+ ((IncentivizeChime != null) ? (IncentivizeChime ?? false ? "Chime " : "") : ("Chime? "))
			+ (IncentivizeBad ? "Cloth " : "")
			+ ((IncentivizeCrown != null) ? (IncentivizeCrown ?? false ? "Crown " : "") : ("Crown? "))
			+ ((IncentivizeCrystal != null) ? (IncentivizeCrystal ?? false ? "Crystal " : "") : ("Crystal? "))
			+ ((IncentivizeCube != null) ? (IncentivizeCube ?? false ? "Cube " : "") : ("Cube? "))
			+ ((IncentivizeFloater != null) ? (IncentivizeFloater ?? false ? "Floater " : "") : ("Floater? "))
			+ ((IncentivizeHerb != null) ? (IncentivizeHerb ?? false ? "Herb " : "") : ("Herb? "))
			+ ((IncentivizeKey != null) ? (IncentivizeKey ?? false ? "Key " : "") : ("Key? "))
			+ ((IncentivizeLute != null) ? (IncentivizeLute ?? false ? "Lute " : "") : ("Lute? "))
			+ ((IncentivizeOxyale != null) ? (IncentivizeOxyale ?? false ? "Oxyale " : "") : ("Oxyale? "))
			+ ((IncentivizeRod != null) ? (IncentivizeRod ?? false ? "Rod " : "") : ("Rod? "))
			+ ((IncentivizeRuby != null) ? (IncentivizeRuby ?? false ? "Ruby " : "") : ("Ruby? "))
			+ ((IncentivizeShip != null) ? (IncentivizeShip ?? false ? "Ship " : "") : ("Ship? "))
			+ ((IncentivizeSlab != null) ? (IncentivizeSlab ?? false ? "Slab " : "") : ("Slab? "))
			+ ((IncentivizePromotion != null) ? (IncentivizePromotion ?? false ? "Tail " : "") : ("Tail? "))
			+ ((IncentivizeTnt != null) ? (IncentivizeTnt ?? false ? "Tnt " : "") : ("Tnt? "))
			+ (((IncentivizeMasamune & !NoMasamune) != null) ? ((IncentivizeMasamune & !NoMasamune) ?? false ? "Masmune\U0001F5E1 " : "") : ("Masmune?\U0001F5E1 "))
			+ ((IncentivizeKatana != null) ? (IncentivizeKatana ?? false ? "Katana\U0001F5E1 " : "") : ("Katana?\U0001F5E1 "))
			+ ((IncentivizeVorpal != null) ? (IncentivizeVorpal ?? false ? "Vorpal\U0001F5E1 " : "") : ("Vorpal?\U0001F5E1 "))
			+ ((IncentivizeXcalber != null) ? (IncentivizeXcalber ?? false ? "XCalber\U0001F5E1 " : "") : ("XCalber?\U0001F5E1 "))
			+ ((IncentivizeDefCastWeapon != null) ? (IncentivizeDefCastWeapon ?? false ? "Defense\U0001F5E1 " : "") : ("Defense?\U0001F5E1 "))
			+ (IncentivizeOtherCastWeapon ? "Mage\U0001F9D9 " : "")
			+ ((IncentivizeOffCastWeapon != null) ? (IncentivizeOffCastWeapon ?? false ? "Thor\U0001F528 " : "") : ("Thor?\U0001F528 "))
			+ ((IncentivizeOpal != null) ? (IncentivizeOpal ?? false ? "Opal\U0001F48D " : "") : ("Opal?\U0001F48D "))
			+ ((IncentivizeOtherCastArmor != null) ? (IncentivizeOtherCastArmor ?? false ? "Power\U0001F94A " : "") : ("Power?\U0001F94A "))
			+ ((IncentivizePowerRod != null) ? (IncentivizePowerRod ?? false ? "PowerRod " : "") : ("PowerRod? "))
			+ ((IncentivizeOffCastArmor != null) ? (IncentivizeOffCastArmor ?? false ? "Black\U0001F9E5 " : "") : ("Black?\U0001F9E5 "))
			+ ((IncentivizeDefCastArmor != null) ? (IncentivizeDefCastArmor ?? false ? "White\U0001F455 " : "") : ("White?\U0001F455 "))
			+ ((IncentivizeRibbon != null) ? (IncentivizeRibbon ?? false ? "Ribbon\U0001F380 " : "") : ("Ribbon?\U0001F380 "))
			+ (IncentivizeRibbon2 ? "Ribbon\U0001F380 " : "")
			+ (Incentivize65K ? "65000G " : "");

		public bool IncentivizeKingConeria => (NPCItems ?? false) && (IncentivizeFreeNPCs ?? false);
		public bool IncentivizePrincess => (NPCItems ?? false) && (IncentivizeFreeNPCs ?? false);
		public bool IncentivizeBikke => (NPCItems ?? false) && (IncentivizeFreeNPCs ?? false);
		public bool IncentivizeSarda => (NPCItems ?? false) && (IncentivizeFreeNPCs ?? false);
		public bool IncentivizeCanoeSage => (NPCItems ?? false) && (IncentivizeFreeNPCs ?? false);
		public bool IncentivizeCaravan => (NPCItems ?? false) && (IncentivizeFreeNPCs ?? false);
		public bool IncentivizeCubeBot => (NPCItems ?? false) && (IncentivizeFreeNPCs ?? false);

		public bool IncentivizeFairy => (NPCFetchItems ?? false) && (IncentivizeFetchNPCs ?? false);
		public bool IncentivizeAstos => (NPCFetchItems ?? false) && (IncentivizeFetchNPCs ?? false);
		public bool IncentivizeMatoya => (NPCFetchItems ?? false) && (IncentivizeFetchNPCs ?? false);
		public bool IncentivizeElfPrince => (NPCFetchItems ?? false) && (IncentivizeFetchNPCs ?? false);
		public bool IncentivizeNerrick => (NPCFetchItems ?? false) && (IncentivizeFetchNPCs ?? false) && !NoOverworld;
		public bool IncentivizeLefein => (NPCFetchItems ?? false) && (IncentivizeFetchNPCs ?? false);
		public bool IncentivizeSmith => (NPCFetchItems ?? false) && (IncentivizeFetchNPCs ?? false);

		public int IncentivizedLocationCountMin => 0
			+ ((NPCItems ?? false) && (IncentivizeFreeNPCs ?? false) ? 7 : 0)
			+ ((NPCFetchItems ?? false) && (IncentivizeFetchNPCs ?? false) ? (!NoOverworld ? 7 : 6) : 0)
			+ ((IncentivizeMarsh ?? false) ? 1 : 0)
			+ ((IncentivizeEarth ?? false) ? 1 : 0)
			+ ((IncentivizeVolcano ?? false) ? 1 : 0)
			+ ((IncentivizeIceCave ?? false) ? 1 : 0)
			+ ((IncentivizeOrdeals ?? false) ? 1 : 0)
			+ ((IncentivizeSeaShrine ?? false) ? 1 : 0)
			+ ((IncentivizeConeria ?? false) ? 1 : 0)
			+ ((IncentivizeMarshKeyLocked ?? false) ? 1 : 0)
			+ ((IncentivizeTitansTrove ?? false) ? 1 : 0)
			+ ((IncentivizeSkyPalace ?? false) ? 1 : 0)
			+ ((IncentivizeCardia ?? false) ? 1 : 0);


		public int IncentivizedLocationCountMax => 0
			+ ((NPCItems ?? true) && (IncentivizeFreeNPCs ?? true) ? 7 : 0)
			+ ((NPCFetchItems ?? true) && (IncentivizeFetchNPCs ?? true) ? (!NoOverworld ? 7 : 6) : 0)
			+ ((IncentivizeMarsh ?? true) ? 1 : 0)
			+ ((IncentivizeEarth ?? true) ? 1 : 0)
			+ ((IncentivizeVolcano ?? true) ? 1 : 0)
			+ ((IncentivizeIceCave ?? true) ? 1 : 0)
			+ ((IncentivizeOrdeals ?? true) ? 1 : 0)
			+ ((IncentivizeSeaShrine ?? true) ? 1 : 0)
			+ ((IncentivizeConeria ?? true) ? 1 : 0)
			+ ((IncentivizeMarshKeyLocked ?? true) ? 1 : 0)
			+ ((IncentivizeTitansTrove ?? true) ? 1 : 0)
			+ ((IncentivizeSkyPalace ?? true) ? 1 : 0)
			+ ((IncentivizeCardia ?? true) ? 1 : 0);

		public int TrappedChestsFloor => 0
			+ ((TCShards == TCOptions.All) ? 32 : 0)
			+ ((TCKeyItems == TCOptions.All) ? 16 : 0)
		    + ((TCBetterTreasure == TCOptions.All) ? 50 : 0)
		    + ((TCMasaGuardian == true && TCBetterTreasure != TCOptions.All) ? 1 : 0)
			+ ((TrappedChaos == true) ? 1 : 0);

		private static bool ConvertTriState(bool? tristate, MT19337 rng)
		{
			int rngval = rng.Between(0, 1);
			bool rval = tristate ?? (rngval == 0);
			return rval;
		}

		public static Flags ConvertAllTriState(Flags flags, MT19337 rng)
		{
			Flags newflags = flags.ShallowCopy();
			PropertyInfo[] properties = newflags.GetType().GetProperties();
			foreach (var property in properties)
			{
				if (property.PropertyType == typeof(bool?) && property.GetValue(newflags) == null)
				{
					bool newvalue = ConvertTriState((bool?)property.GetValue(newflags), rng);
					property.SetValue(newflags, newvalue);
				}
			}

			if (flags.ItemMagicMode == ItemMagicMode.Random) newflags.ItemMagicMode = (ItemMagicMode)rng.Between(0, 2);
			if (flags.ItemMagicPool == ItemMagicPool.Random) newflags.ItemMagicPool = (ItemMagicPool)rng.Between(0, 4);

			if (flags.GuaranteedDefenseItem == GuaranteedDefenseItem.Random) newflags.GuaranteedDefenseItem = rng.Between(0, 1) > 0 ? GuaranteedDefenseItem.Any : GuaranteedDefenseItem.None;
			if (flags.GuaranteedPowerItem == GuaranteedPowerItem.Random) newflags.GuaranteedPowerItem = rng.Between(0, 1) > 0 ? GuaranteedPowerItem.Any : GuaranteedPowerItem.None;

			if (newflags.GuaranteedDefenseItem == GuaranteedDefenseItem.Any) newflags.GuaranteedDefenseItem = (GuaranteedDefenseItem)rng.Between(1, 3);
			if (newflags.GuaranteedPowerItem == GuaranteedPowerItem.Any) newflags.GuaranteedPowerItem = (GuaranteedPowerItem)rng.Between(1, 3);

			return newflags;
		}

		public Flags ShallowCopy()
		{
			return (Flags)this.MemberwiseClone();
		}

		public bool? ImmediatePureAndSoftRequired => (TouchMode != TouchMode.Standard) | Entrances | MapOpenProgression | RandomizeFormationEnemizer | RandomizeEnemizer;
		public bool? DeepCastlesPossible => Entrances & Floors;
		public bool? DeepTownsPossible => Towns & Entrances & Floors & EntrancesMixedWithTowns;
		public bool EnemizerEnabled => (bool)RandomizeFormationEnemizer | (bool)RandomizeEnemizer;
		public bool EnemizerDontMakeNewScripts => (bool)ShuffleSkillsSpellsEnemies & !((bool)EnemySkillsSpellsTiered);

		public bool? TrappedChestsEnabled => (bool)TrappedChaos | (bool)TCMasaGuardian | (TCBetterTreasure == TCOptions.All | TCKeyItems == TCOptions.All | TCShards == TCOptions.All) | ((TCBetterTreasure == TCOptions.Pooled | TCKeyItems == TCOptions.Pooled | TCShards == TCOptions.Pooled) & TCChestCount > 0) | (TCChestCount > 0);

		public bool IsAnythingLoose => (IncentivizedItemCountMax > IncentivizedLocationCountMin) || IncentivizeMainItems != true || IncentivizeFetchItems != true || (IncentivizeAirship != true && FreeAirship != true && NoFloater != true) || (IncentivizeCanoeItem != true && FreeCanoe != true) || (IncentivizeShipAndCanal != true && (FreeShip != true || FreeCanal != true)) || (IncentivizeBridgeItem != true && FreeBridge != true) || (IncentivizeTail != true && FreeTail != true && NoTail != true);

		public static string EncodeFlagsText(Flags flags)
		{
			var properties = typeof(Flags).GetProperties(BindingFlags.Instance | BindingFlags.Public);
			var flagproperties = properties.Where(p => p.CanWrite).OrderBy(p => p.Name).ToList();

			BigInteger sum = 0;
			sum = AddString(sum, 7, (FFRVersion.Sha.Length >= 7) ? FFRVersion.Sha.Substring(0, 7) : FFRVersion.Sha.PadRight(7, 'X'));

			foreach (var p in flagproperties)
			{
				if (Nullable.GetUnderlyingType(p.PropertyType) == typeof(bool))
				{
					sum = AddTriState(sum, (bool?)p.GetValue(flags));
				}
				else if (p.PropertyType == typeof(bool))
				{
					sum = AddBoolean(sum, (bool)p.GetValue(flags));
				}
				else if (p.PropertyType.IsEnum)
				{
					sum = AddNumeric(sum, Enum.GetValues(p.PropertyType).Cast<int>().Max() + 1, (int)p.GetValue(flags));
				}
				else if (p.PropertyType == typeof(int))
				{
					IntegerFlagAttribute ia = p.GetCustomAttribute<IntegerFlagAttribute>();
					var radix = (ia.Max - ia.Min) / ia.Step + 1;
					var val = (int)p.GetValue(flags);
					var raw_val = (val - ia.Min) / ia.Step;
					sum = AddNumeric(sum, radix, raw_val);
				}
				else if (p.PropertyType == typeof(double))
				{
					DoubleFlagAttribute ia = p.GetCustomAttribute<DoubleFlagAttribute>();
					var radix = (int)Math.Ceiling((ia.Max - ia.Min) / ia.Step) + 1;
					var val = (double)p.GetValue(flags);
					var raw_val = (int)Math.Round((val - ia.Min) / ia.Step);
					sum = AddNumeric(sum, radix, raw_val);
				}
			}

			return BigIntegerToString(sum);
		}

		public static Flags DecodeFlagsText(string text)
		{
			var properties = typeof(Flags).GetProperties(BindingFlags.Instance | BindingFlags.Public);
			var flagproperties = properties.Where(p => p.CanWrite).OrderBy(p => p.Name).Reverse().ToList();

			var sum = StringToBigInteger(text);
			var flags = new Flags();

			foreach (var p in flagproperties)
			{
				if (Nullable.GetUnderlyingType(p.PropertyType) == typeof(bool))
				{
					p.SetValue(flags, GetTriState(ref sum));
				}
				else if (p.PropertyType == typeof(bool))
				{
					p.SetValue(flags, GetBoolean(ref sum));
				}
				else if (p.PropertyType.IsEnum)
				{
					p.SetValue(flags, GetNumeric(ref sum, Enum.GetValues(p.PropertyType).Cast<int>().Max() + 1));
				}
				else if (p.PropertyType == typeof(int))
				{
					IntegerFlagAttribute ia = p.GetCustomAttribute<IntegerFlagAttribute>();
					var radix = (ia.Max - ia.Min) / ia.Step + 1;
					var raw_val = GetNumeric(ref sum, radix);
					var val = raw_val * ia.Step + ia.Min;
					p.SetValue(flags, val);
				}
				else if (p.PropertyType == typeof(double))
				{
					DoubleFlagAttribute ia = p.GetCustomAttribute<DoubleFlagAttribute>();
					var radix = (int)Math.Ceiling((ia.Max - ia.Min) / ia.Step) + 1;
					var raw_val = GetNumeric(ref sum, radix);
					var val = Math.Min(Math.Max(raw_val * ia.Step + ia.Min, ia.Min), ia.Max);
					p.SetValue(flags, val);
				}
			}

			string EncodedSha = GetString(ref sum, 7);
			if (((FFRVersion.Sha.Length >= 7) ? FFRVersion.Sha.Substring(0, 7) : FFRVersion.Sha.PadRight(7, 'X')) != EncodedSha)
			{
				throw new Exception("The encoded version does not match the expected version");
			}

			return flags;
		}

		private static BigInteger AddEnum<T>(BigInteger sum, T value) => AddNumeric(sum, Enum.GetValues(typeof(T)).Cast<int>().Max() + 1, Convert.ToInt32(value));

		private static BigInteger AddNumeric(BigInteger sum, int radix, int value) => sum * radix + value;
		private static BigInteger AddString(BigInteger sum, int length, string str)
		{
			Encoding AsciiEncoding = Encoding.ASCII;
			byte[] bytes = AsciiEncoding.GetBytes(str);
			BigInteger StringAsBigInt = new BigInteger(bytes);
			BigInteger LargestInt = new BigInteger(Math.Pow(0xFF, bytes.Length) - 1);


			return sum * LargestInt + StringAsBigInt;
		}
		private static BigInteger AddBoolean(BigInteger sum, bool value) => AddNumeric(sum, 2, value ? 1 : 0);
		private static int TriStateValue(bool? value) => value.HasValue ? (value.Value ? 1 : 0) : 2;
		private static BigInteger AddTriState(BigInteger sum, bool? value) => AddNumeric(sum, 3, TriStateValue(value));

		private static T GetEnum<T>(ref BigInteger sum) where T : Enum => (T)(object)GetNumeric(ref sum, Enum.GetValues(typeof(T)).Cast<int>().Max() + 1);

		private static int GetNumeric(ref BigInteger sum, int radix)
		{
			sum = BigInteger.DivRem(sum, radix, out var value);

			return (int)value;
		}
		private static string GetString(ref BigInteger sum, int length)
		{
			BigInteger LargestInt = new BigInteger(Math.Pow(0xFF, length) - 1);
			sum = BigInteger.DivRem(sum, LargestInt, out BigInteger value);
			Encoding AsciiEncoding = Encoding.ASCII;
			byte[] bytes = value.ToByteArray();
			string str = AsciiEncoding.GetString(bytes);

			return str;
		}
		private static bool GetBoolean(ref BigInteger sum) => GetNumeric(ref sum, 2) != 0;
		private static bool? ValueTriState(int value) => value == 0 ? (bool?)false : value == 1 ? (bool?)true : null;
		private static bool? GetTriState(ref BigInteger sum) => ValueTriState(GetNumeric(ref sum, 3));

		private const string Base64Chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789.-";

		private static string BigIntegerToString(BigInteger sum)
		{
			var s = "";

			while (sum > 0)
			{
				var digit = GetNumeric(ref sum, 64);
				s += Base64Chars[digit];
			}

			return s;
		}

		private static BigInteger StringToBigInteger(string s)
		{
			var sum = new BigInteger(0);

			foreach (char c in s.Reverse())
			{
				int index = Base64Chars.IndexOf(c);
				if (index < 0) throw new IndexOutOfRangeException($"{c} is not valid FFR-style Base64.");

				sum = AddNumeric(sum, 64, index);
			}

			return sum;
		}


		public class Preset
		{
			public string Name { get; set; }
			public Flags Flags { get; set; }
		}

		//public static Flags FromJson(string json) => JsonConvert.DeserializeObject<Preset>(json).Flags;

		public class Preset2
		{
			public string Name { get; set; }
			public Dictionary<string, object> Flags { get; set; }
		}

		public static (string name, Flags flags, IEnumerable<string> log) FromJson(string json)
		{
		    var flags = new Flags();
		    string name;
		    IEnumerable<string> log;
		    (name, log) = flags.LoadFromJson(json);
		    return (name, flags, log);
		}

		public  (string name, IEnumerable<string> log) LoadFromJson(string json) {
			var w = new System.Diagnostics.Stopwatch();
			w.Restart();

			var preset = JsonConvert.DeserializeObject<Preset2>(json);
			var preset_dic = preset.Flags.ToDictionary(kv => kv.Key.ToLower());

			var properties = typeof(Flags).GetProperties(BindingFlags.Instance | BindingFlags.Public);
			var flagproperties = properties.Where(p => p.CanWrite).OrderBy(p => p.Name).Reverse().ToList();

			List<string> warnings = new List<string>();

			foreach (var pi in flagproperties)
			{
				if (preset_dic.TryGetValue(pi.Name.ToLower(), out var obj))
				{
					var result = SetValue(pi, this, obj.Value);

					if (result != null) warnings.Add(result);

					preset.Flags.Remove(obj.Key);
				}
				else
				{
					//warnings.Add($"\"{pi.Name}\" was missing in preset and set to default \"{pi.GetValue(flags)}\".");
				}
			}

			foreach (var flag in preset.Flags)
			{
				warnings.Add($"\"{flag.Key}\" with value \"{flag.Value}\" does not exist and has been discarded.");
			}

			warnings.Sort();

			w.Stop();
			return (preset.Name, warnings);
		}

		public void LoadResourcePackFlags(Stream stream) {
		    var archive = new ZipArchive(stream);

		    var fj = archive.GetEntry("flags.json");
		    if (fj != null) {
			using (var s = fj.Open()) {
			    using (StreamReader rd = new StreamReader(s)) {
				this.LoadFromJson(rd.ReadToEnd());
			    }
			}
		    }
		    var overworld = archive.GetEntry("overworld.json");
		    if (overworld != null) {
			using (var s = overworld.Open()) {
			    using (StreamReader rd = new StreamReader(s)) {
				this.ReplacementMap = JsonConvert.DeserializeObject<OwMapExchangeData>(rd.ReadToEnd());
			    }
			}
		    }
		}


		private static string SetValue(PropertyInfo p, Flags flags, object obj)
		{
			try
			{
				if (Nullable.GetUnderlyingType(p.PropertyType) == typeof(bool))
				{
					var t = obj == null ? (bool?)null : (bool?)(bool)obj;
					p.SetValue(flags, t);
				}
				else if (p.PropertyType == typeof(bool))
				{
					if (obj == null) throw new ArgumentNullException();
					p.SetValue(flags, obj);
				}
				else if (p.PropertyType.IsEnum)
				{
					if (obj == null) throw new ArgumentNullException();

					var values = Enum.GetValues(p.PropertyType);

					if (obj is string v)
					{
						foreach (var e in values)
						{
							if (v.ToLower() == e.ToString().ToLower())
							{
								p.SetValue(flags, e);
								return null;
							}
						}
					}
					else if (obj is IConvertible)
					{
						int v2 = Convert.ToInt32(obj);

						foreach (var e in values)
						{
							if (v2 == Convert.ToInt32(e))
							{
								p.SetValue(flags, e);
								return null;
							}
						}
					}

					throw new ArgumentException();
				}
				else if (p.PropertyType == typeof(int))
				{
					IntegerFlagAttribute ia = p.GetCustomAttribute<IntegerFlagAttribute>();
					var v3 = Convert.ToInt32(obj);

					p.SetValue(flags, v3);

					if (v3 > ia.Max)
					{
						return $"\"{p.Name}\" with value \"{obj}\" exceeds the maximum but will be kept.";
					}
					else if (v3 < ia.Min)
					{
						return $"\"{p.Name}\" with value \"{obj}\" deceedes the minimum but will be kept.";
					}
				}
				else if (p.PropertyType == typeof(double))
				{
					DoubleFlagAttribute da = p.GetCustomAttribute<DoubleFlagAttribute>();
					var v3 = Convert.ToDouble(obj);

					p.SetValue(flags, v3);

					if (v3 > da.Max)
					{
						return $"\"{p.Name}\" with value \"{obj}\" exceeds the maximum but will be kept.";
					}
					else if (v3 < da.Min)
					{
						return $"\"{p.Name}\" with value \"{obj}\" deceedes the minimum but will be kept.";
					}
				}
			}
			catch
			{
				return $"\"{p.Name}\" with value \"{obj}\" was invalid and set to default \"{p.GetValue(flags)}\".";
			}

			return null;
		}
	}
}
