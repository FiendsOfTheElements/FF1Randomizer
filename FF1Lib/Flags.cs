using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Numerics;
using System.Reflection;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Schema;
using RomUtilities;

namespace FF1Lib
{
	public class Flags : IIncentiveFlags, IMapEditFlags, IScaleFlags, IFloorShuffleFlags
	{
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
		public bool? TransformFinalFormation { get; set; } = false;
		public bool ChaosRush { get; set; } = false;
		public bool? ShortToFR { get; set; } = false;
		public bool? PreserveFiendRefights { get; set; } = false;
		public bool? PreserveAllFiendRefights { get; set; } = false;

		public bool? MagicShops { get; set; } = false;
		public bool? MagicShopLocs { get; set; } = false;
		public bool? MagicLevels { get; set; } = false;
		public bool? MagicPermissions { get; set; } = false;
		public bool? ItemMagic { get; set; } = false;
		public bool? MagisizeWeapons { get; set; } = false;
		public bool? MagisizeWeaponsBalanced { get; set; } = false;
		public bool? MagicLevelsTiered { get; set; } = false;
		public bool? MagicLevelsMixed { get; set; } = false;

		public AutohitThreshold MagicAutohitThreshold { get; set; } = AutohitThreshold.Vanilla;

		public bool? Rng { get; set; } = false;
		public bool FixMissingBattleRngEntry { get; set; } = false;
		public Runnability Runnability { get; set; } = Runnability.Normal;
		public bool? EnemyFormationsSurprise { get; set; } = false;
		public bool? UnrunnablesStrikeFirstAndSurprise { get; set; } = false;
		public bool? EnemyTrapTiles { get; set; } = false;
		public bool? RemoveTrapTiles { get; set; } = false;
		public bool? RandomTrapFormations { get; set; } = false;
		public bool? TrappedChests { get; set; } = false;
		public bool? TCMasaGuardian { get; set; } = false;
		public bool? TrappedShards { get; set; } = false;
		public bool? SwolePirates { get; set; } = false;
		public bool? ScaryImps { get; set; } = false;
		public bool? EnemyScripts { get; set; } = false;
		public bool? BossScriptsOnly { get; set; } = false;
		public bool? EnemySkillsSpells { get; set; } = false;
		public bool? BossSkillsOnly { get; set; } = false;
		public bool? EnemySkillsSpellsTiered { get; set; } = false;
		public bool? EnemyStatusAttacks { get; set; } = false;
		public bool? RandomStatusAttacks { get; set; } = false;
		public bool? AllowUnsafePirates { get; set; } = false;
		public bool? AllowUnsafeMelmond { get; set; } = false;

		public WarMECHMode WarMECHMode { get; set; } = WarMECHMode.Vanilla;
		public bool? OrdealsPillars { get; set; } = false;
		public SkyCastle4FMazeMode SkyCastle4FMazeMode { get; set; } = SkyCastle4FMazeMode.Normal;
		public bool? TitansTrove { get; set; } = false;
		public bool? LefeinShops { get; set; } = false;
		public bool? ConfusedOldMen { get; set; } = false;
		public bool? FlipDungeons { get; set; } = false;
		public bool SpookyFlag { get; set; } = false;
		public bool? MapOpenProgression { get; set; } = false;
		public bool? MapOpenProgressionDocks { get; set; } = false;
		public bool? Entrances { get; set; } = false;
		public bool? Towns { get; set; } = false;
		public bool? Floors { get; set; } = false;
		public bool? AllowDeepCastles { get; set; } = false;
		public bool? AllowDeepTowns { get; set; } = false;
		public bool? MapOpenProgressionExtended { get; set; } = false;
		public bool? MapDwarvesNorthwest { get; set; } = false;
		public bool? MapAirshipDock { get; set; } = false;
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
		public bool ClassicItemPlacement { get; set; } = false;

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
		public bool? IncentivizeRandomChestInLocation { get; set; } = false;
		public bool? IncentivizeRandomChestIncludeExtra { get; set; } = false;
		public bool? BetterTrapChests { get; set; } = false;


		public bool? IncentivizeMasamune { get; set; } = false;
		public bool? IncentivizeKatana { get; set; } = false;
		public bool? IncentivizeVorpal { get; set; } = false;
		public bool? IncentivizeOpal { get; set; } = false;
		public bool? IncentivizeRibbon { get; set; } = false;
		public bool IncentivizeRibbon2 => false;
		public bool Incentivize65K => false;
		public bool IncentivizeBad => false;

		public bool? IncentivizeDefCastArmor { get; set; } = false;
		public bool? IncentivizeOffCastArmor { get; set; } = false;
		public bool? IncentivizeOtherCastArmor { get; set; } = false;
		public bool? IncentivizeDefCastWeapon { get; set; } = false;
		public bool? IncentivizeOffCastWeapon { get; set; } = false;
		public bool IncentivizeOtherCastWeapon { get; set; } = false;

		public bool? EarlyKing { get; set; } = false;
		public bool? EarlySarda { get; set; } = false;
		public bool? EarlySage { get; set; } = false;
		public bool? EarlyOrdeals { get; set; } = false;
		public bool? ShuffleObjectiveNPCs { get; set; } = false;
		public bool OnlyRequireGameIsBeatable { get; set; } = true;

		public bool? FreeBridge { get; set; } = false;
		public bool? FreeShip { get; set; } = false;
		public bool? FreeAirship { get; set; } = false;
		public bool? FreeLuteFlag { get; set; } = false;
		public bool FreeOrbs { get; set; } = false;
		public bool EnableCritNumberDisplay { get; set; } = false;
		public bool? FreeCanal { get; set; } = false;
		public bool? FreeCanoe { get; set; } = false;
		public bool EasyMode { get; set; } = false;

		public bool HousesFillHp { get; set; } = false;

		public bool SpeedHacks { get; set; } = false;
		public bool NoPartyShuffle { get; set; } = false;
		public bool Dash { get; set; } = false;
		public bool BuyTen { get; set; } = false;
		public bool BuyTenOld { get; set; } = false;
		public bool IdentifyTreasures { get; set; } = false;
		public bool ShopInfo { get; set; } = false;
		public bool WaitWhenUnrunnable { get; set; } = false;

		public bool HouseMPRestoration { get; set; } = false;
		public bool WeaponStats { get; set; } = false;
		public bool BBCritRate { get; set; } = false;
		public bool WeaponCritRate { get; set; } = false;
		public bool WeaponBonuses { get; set; } = false;

		[IntegerFlag(0, 50)]
		public int WeaponTypeBonusValue { get; set; } = 10;
		public bool ChanceToRun { get; set; } = false;
		public bool SpellBugs { get; set; } = false;
		public bool BlackBeltAbsorb { get; set; } = false;
		public bool NPCSwatter { get; set; } = false;
		public bool BattleMagicMenuWrapAround { get; set; } = false;
		public bool InventoryAutosort { get; set; } = false;
		public bool EnemyStatusAttackBug { get; set; } = false;
		public bool EnemySpellsTargetingAllies { get; set; } = false;
		public bool EnemyElementalResistancesBug { get; set; } = false;
		public bool ImproveTurnOrderRandomization { get; set; } = false;
		public bool FixHitChanceCap { get; set; } = false;

		public bool StartingGold { get; set; } = false;
		public bool WrapStatOverflow { get; set; } = false;
		public bool WrapPriceOverflow { get; set; } = false;
		public bool IncludeMorale { get; set; } = false;
		public bool NoDanMode { get; set; } = false;
		public bool? NoTail { get; set; } = false;
		public bool? GuaranteedMasamune { get; set; } = false;
		public bool? SendMasamuneHome { get; set; } = false;
		public bool? NoMasamune { get; set; } = false;
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

		[IntegerFlag(0, 500, 10)]
		public int ExpBonus { get; set; } = 0;

		[DoubleFlag(0, 45)]
		public double EncounterRate { get; set; } = 0;

		[DoubleFlag(0, 45)]
		public double DungeonEncounterRate { get; set; } = 0;
		public ProgressiveScaleMode ProgressiveScaleMode { get; set; } = ProgressiveScaleMode.Disabled;

		public StartingItemSet StartingItemSet { get; set; } = StartingItemSet.None;

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
		public bool EFGEarth1 { get; set; } = false;
		public bool EFGEarth2 { get; set; } = false;
		public bool? FiendShuffle { get; set; } = false;
		public bool DisableTentSaving { get; set; } = false;
		public bool DisableInnSaving { get; set; } = false;
		public bool SaveGameWhenGameOver { get; set; } = false;
		public bool SaveGameDWMode { get; set; } = false;
		public bool PacifistMode { get; set; } = false;
		public bool? ShuffleAstos { get; set; } = false;
		public bool? RandomizeEnemizer { get; set; } = false;
		public bool? RandomizeFormationEnemizer { get; set; } = false;
		public bool? GenerateNewSpellbook { get; set; } = false;
		public bool? SpellcrafterMixSpells { get; set; } = false;
		public bool ThiefHitRate { get; set; } = false;
		public bool AllSpellLevelsForKnightNinja { get; set; } = false;
		public bool BuffHealingSpells { get; set; } = false;
		public bool? FreeTail { get; set; } = false;
		public bool? HintsVillage { get; set; } = false;
		public bool? HintsDungeon { get; set; } = false;
		public bool? HintsUseless { get; set; } = false;
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
		public bool? BalancedItemMagicShuffle { get; set; } = false;
		public bool? SeparateBossHPScaling { get; set; } = false;
		public bool? SeparateEnemyHPScaling { get; set; } = false;
		public bool? ClampBossHPScaling { get; set; } = false;
		public bool? ClampEnemyHpScaling { get; set; } = false;
		public PoolSize PoolSize { get; set; } = PoolSize.Size6;
		public bool? EnablePoolParty { get; set; } = false;
		public bool? IncludePromClasses { get; set; } = false;
		public bool? EnableRandomPromotions { get; set; } = false;
		public bool? IncludeBaseClasses { get; set; } = false;
		public bool? RandomPromotionsSpoilers { get; set; } = false;
		public bool? RandomizeClass { get; set; } = false;
		public bool? RandomizeClassNoCasting { get; set; } = false;
		public bool? RandomizeClassChaos { get; set; } = false;

		[IntegerFlag(0, 4)]
		public int RandomizeClassMaxBonus { get; set; } = 2;

		[IntegerFlag(0, 4)]
		public int RandomizeClassMaxMalus { get; set; } = 1;
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

		public LockHitMode LockMode { get; set; } = LockHitMode.Vanilla;

		public MDEFGrowthMode MDefMode { get; set; } = MDEFGrowthMode.None;

		public FormationShuffleMode FormationShuffleMode { get; set; } = FormationShuffleMode.None;

		public WorldWealthMode WorldWealth { get; set; } = WorldWealthMode.Normal;

		public EvadeCapValues EvadeCap { get; set; } = EvadeCapValues.medium;

		public bool? AllowUnsafeStartArea { get; set; } = false;

		public bool? EarlierRuby { get; set; } = false;
		public bool? GuaranteedRuseItem { get; set; } = false;
		public bool? DisableStunTouch { get; set; } = false;
		public bool? MapCanalBridge => (NPCItems) | (NPCFetchItems) | MapOpenProgression | MapOpenProgressionExtended;
		public bool? MapOnracDock => MapOpenProgressionDocks;
		public bool? MapMirageDock => MapOpenProgressionDocks;
		public bool? MapConeriaDwarves => MapOpenProgression;
		public bool? MapVolcanoIceRiver => MapOpenProgression;
		// public bool? MapDwarvesNorthwest => MapOpenProgression;
		// public bool? MapAirshipDock => MapOpenProgression;

		// The philosophy governing item incentivizations works something like this:
		// 1. If the item is NOT being shuffled to another location it cannot be incentivized. (Duh)
		// 2. If the item is required to unlock any location OR is given to a not-shuffled NPC who gives
		//    such an item in return it is considered a MAIN item. (e.g. CROWN/SLAB with vanilla fetch quests)
		// 3. If the item is given to an NPC who themselves is shuffled it's considered a FETCH item.
		// 4. The vehicles now have their own incentivization flags apart from other progression items.

		// Ruby is required if Sarda is Required for the ROD
		public bool? RequiredRuby => !EarlySage & !NPCItems & !FreeAirship;
		public bool? UselessRuby => FreeAirship & !TitansTrove;
		public bool? IncentivizeRuby => (RequiredRuby & IncentivizeMainItems) | (!RequiredRuby & IncentivizeFetchItems & !UselessRuby);

		// If Canoe and Fetch Quests are unshuffled and there is no free canal or airship then TNT is required
		public bool? RequiredTnt => !NPCFetchItems & !NPCItems & !(FreeCanal | FreeAirship);
		// If Fetch Items are vanilla and the player has a free Canal, do not incentivize TNT even if Other Quest Items are in the pool since there would be absolutely nothing to gain from TNT
		public bool? UselessTnt => !NPCFetchItems & (FreeCanal | (FreeAirship & !MapOpenProgression));
		public bool? IncentivizeTnt => (RequiredTnt & IncentivizeMainItems) | (!RequiredTnt & IncentivizeFetchItems & !UselessTnt);

		public bool? IncentivizeCrown => (!(NPCFetchItems ?? false) && (IncentivizeMainItems ?? false)) || ((NPCFetchItems ?? false) && (IncentivizeFetchItems ?? false));
		public bool? IncentivizeSlab => (!(NPCFetchItems ?? false) && (IncentivizeMainItems ?? false)) || ((NPCFetchItems ?? false) && (IncentivizeFetchItems ?? false));
		public bool? IncentivizeBottle => (!(NPCFetchItems ?? false) && (IncentivizeMainItems ?? false)) || ((NPCFetchItems ?? false) && (IncentivizeFetchItems ?? false));

		public bool IncentivizeBridge => false;
		public bool? IncentivizeCanoe => NPCItems & IncentivizeCanoeItem & !FreeCanoe;
		public bool? IncentivizeLute => NPCItems & !FreeLute & IncentivizeMainItems;
		public bool? IncentivizeShip => NPCItems & IncentivizeShipAndCanal & !FreeShip;
		public bool? IncentivizeRod => NPCItems & IncentivizeMainItems;
		public bool? IncentivizeCube => NPCItems & IncentivizeMainItems;
		public bool? IncentivizeFloater => !FreeAirship & IncentivizeAirship;
		public bool? IncentivizePromotion => !FreeTail & !NoTail & IncentivizeTail;

		public bool? IncentivizeCanal => NPCFetchItems & IncentivizeShipAndCanal & !FreeCanal;
		public bool? IncentivizeCrystal => NPCFetchItems & IncentivizeFetchItems;
		public bool? IncentivizeHerb => NPCFetchItems & IncentivizeFetchItems;
		public bool? IncentivizeKey => NPCFetchItems & IncentivizeMainItems;
		public bool? IncentivizeChime => NPCFetchItems & IncentivizeMainItems;
		public bool? IncentivizeOxyale => NPCFetchItems & IncentivizeMainItems;

		public bool? IncentivizeAdamant => IncentivizeFetchItems;
		public bool? IncentivizeXcalber => false;

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
			+ (IncentivizeBridge ? 1 : 0)
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
			+ (IncentivizeBridge ? 1 : 0)
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
			+ (IncentivizeBridge ? "Bridge " : "")
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
		public bool IncentivizeNerrick => (NPCFetchItems ?? false) && (IncentivizeFetchNPCs ?? false);
		public bool IncentivizeLefein => (NPCFetchItems ?? false) && (IncentivizeFetchNPCs ?? false);
		public bool IncentivizeSmith => (NPCFetchItems ?? false) && (IncentivizeFetchNPCs ?? false);

		public int IncentivizedLocationCountMin => 0
			+ ((NPCItems ?? false) && (IncentivizeFreeNPCs ?? false) ? 7 : 0)
			+ ((NPCFetchItems ?? false) && (IncentivizeFetchNPCs ?? false) ? 7 : 0)
			+ ((IncentivizeMarsh ?? false) ? 1 : 0)
			+ ((IncentivizeEarth ?? false) ? 1 : 0)
			+ ((IncentivizeVolcano ?? false) ? 1 : 0)
			+ ((IncentivizeIceCave ?? false) ? 1 : 0)
			+ ((IncentivizeOrdeals ?? false) ? 1 : 0)
			+ ((IncentivizeSeaShrine ?? false) ? 1 : 0)
			+ ((IncentivizeConeria ?? false) ? 1 : 0)
			+ ((IncentivizeMarshKeyLocked ?? false) ? 1 : 0)
			+ ((IncentivizeTitansTrove ?? false) ? 1 : 0)
			+ ((IncentivizeSkyPalace ?? false) ? 1 : 0);


		public int IncentivizedLocationCountMax => 0
			+ ((NPCItems ?? true) && (IncentivizeFreeNPCs ?? true) ? 7 : 0)
			+ ((NPCFetchItems ?? true) && (IncentivizeFetchNPCs ?? true) ? 7 : 0)
			+ ((IncentivizeMarsh ?? true) ? 1 : 0)
			+ ((IncentivizeEarth ?? true) ? 1 : 0)
			+ ((IncentivizeVolcano ?? true) ? 1 : 0)
			+ ((IncentivizeIceCave ?? true) ? 1 : 0)
			+ ((IncentivizeOrdeals ?? true) ? 1 : 0)
			+ ((IncentivizeSeaShrine ?? true) ? 1 : 0)
			+ ((IncentivizeConeria ?? true) ? 1 : 0)
			+ ((IncentivizeMarshKeyLocked ?? true) ? 1 : 0)
			+ ((IncentivizeTitansTrove ?? true) ? 1 : 0)
			+ ((IncentivizeSkyPalace ?? true) ? 1 : 0);


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
			return newflags;
		}

		private Flags ShallowCopy()
		{
			return (Flags)this.MemberwiseClone();
		}

		public bool? ImmediatePureAndSoftRequired => EnemyStatusAttacks | Entrances | MapOpenProgression | RandomizeFormationEnemizer | RandomizeEnemizer;


		public bool? FreeLute => FreeLuteFlag | ShortToFR;

		public bool? DeepCastlesPossible => Entrances & Floors;
		public bool? DeepTownsPossible => Towns & Entrances & Floors & EntrancesMixedWithTowns;

		public bool EnemizerEnabled => (bool)RandomizeFormationEnemizer | (bool)RandomizeEnemizer;
		public bool EnemizerDontMakeNewScripts => (bool)EnemySkillsSpells & !((bool)BossSkillsOnly | (bool)EnemySkillsSpellsTiered);


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

		public static Flags FromJson(string json) => JsonConvert.DeserializeObject<Preset>(json).Flags;
	}
}
