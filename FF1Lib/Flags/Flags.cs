using System.Numerics;
using System.Reflection;
using Newtonsoft.Json;
using System.IO.Compression;
using static FF1Lib.FF1Rom;
using FF1Lib.Sanity;
using System.ComponentModel;

namespace FF1Lib
{
	public partial class Flags : IIncentiveFlags, IScaleFlags, IVictoryConditionFlags, IFloorShuffleFlags, IItemPlacementFlags
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
		public bool? ChestsKeyItems { get; set; } = false;
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
		public ShortToFRFiendsRefights ShortToFRFiendsRefights { get; set; } = ShortToFRFiendsRefights.All;
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

		public bool? EncounterPrng { get; set; } = false;

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

		[IntegerFlag(0, 260, 20)]
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
		public bool? OverworldFiendsOnly { get; set; } = false;
		public bool? TempleOfFiendRefightsOnly { get; set; } = false;
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
		public DamageTilesQuantity DamageTilesQuantity { get; set; } = DamageTilesQuantity.Normal;
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
		public bool? HardcoreAltFiends { get; set; } = false;
		public bool? FinalFantasy2Fiends { get; set; } = false;
		public bool? FinalFantasy3Fiends { get; set; } = false;
		public bool? FinalFantasy4Fiends { get; set; } = false;
		public bool? FinalFantasy5Fiends { get; set; } = false;
		public bool? FinalFantasy6Fiends { get; set; } = false;
		public bool? FinalFantasy1BonusFiends { get; set; } = false;
		public bool? WinnerCircleFiends { get; set; } = false;
		public bool? BlackOrbFiends { get; set; } = false;
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
		public bool? IncreaseRegeneration { get; set; } = false;
		public PoisonModeOptions PoisonMode { get; set; } = PoisonModeOptions.Constant;
		[IntegerFlag(1,500)]

		public int PoisonSetDamageValue { get; set; } = 3;

		public bool? TouchIncludeBosses { get; set; } = false;

		public bool? Lockpicking { get; set; } = false;
		// Done
		public bool? ReducedLuck { get; set; } = false;

		[IntegerFlag(1, 50)]
		public int LockpickingLevelRequirement { get; set; } = 10;

		public bool WhiteMageHarmEveryone { get; set; } = false;

		public bool? ProcgenEarth { get; set; } = false;
	}
}
