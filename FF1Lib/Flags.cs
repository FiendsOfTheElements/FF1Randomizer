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
		public bool? EverythingUnrunnable { get; set; } = false;
		public bool? EverythingRunnable { get; set; } = false;
		public bool? EnemyFormationsUnrunnable { get; set; } = false;
		public bool? EnemyFormationsSurprise { get; set; } = false;
		public bool? UnrunnablesStrikeFirstAndSurprise { get; set; } = false;
		public bool? EnemyTrapTiles { get; set; } = false;
		public bool? RemoveTrapTiles { get; set; } = false;
		public bool? RandomTrapFormations { get; set; } = false;

		public bool? SwolePirates { get; set; } = false;
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
		public bool? ClassAsNpcFiends { get; set; } = false;
		public bool? ClassAsNpcKeyNPC { get; set; } = false;
		public int ClassAsNpcCount { get; set; } = 6;
		public bool ClassAsNpcDuplicate { get; set; } = false;
		public bool? ClassAsNpcForcedFiends { get; set; } = false;
		public bool ClassAsNpcPromotion { get; set; } = false;
		public int BossScaleStatsLow { get; set; } = 50;
		public int BossScaleStatsHigh { get; set; } = 200;
		public int BossScaleHpLow { get; set; } = 50;
		public int BossScaleHpHigh { get; set; } = 200;
		public int EnemyScaleStatsLow { get; set; } = 50;
		public int EnemyScaleStatsHigh { get; set; } = 200;
		public int EnemyScaleHpLow { get; set; } = 50;
		public int EnemyScaleHpHigh { get; set; } = 200;

		public double PriceScaleFactor { get; set; } = 0;

		public double ExpMultiplier { get; set; } = 0;
		public int ExpBonus { get; set; } = 0;
		public double EncounterRate { get; set; } = 0;
		public double DungeonEncounterRate { get; set; } = 0;
		public ProgressiveScaleMode ProgressiveScaleMode { get; set; } = ProgressiveScaleMode.Disabled;

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
		public bool? HintsRngDungeon { get; set; } = false;
		public bool? HintsUseless { get; set; } = false;
		public bool? SpellcrafterRetainPermissions { get; set; } = false;
		public bool? RandomWeaponBonus { get; set; } = false;
		public bool? RandomArmorBonus { get; set; } = false;
		public bool? RandomWeaponBonusExcludeMasa { get; set; } = false;
		public int RandomWeaponBonusLow { get; set; } = -5;
		public int RandomWeaponBonusHigh { get; set; } = 5;
		public int RandomArmorBonusLow { get; set; } = -5;
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
		public bool LinearMPGrowth { get; set; } = false;
		public bool? RandomizeClass { get; set; } = false;
		public bool? RandomizeClassChaos { get; set; } = false;
		public int RandomizeClassMaxBonus { get; set; } = 2;
		public int RandomizeClassMaxMalus { get; set; } = 1;
		public bool? ChangeMaxMP { get; set; } = false;
		public int RedMageMaxMP { get; set; } = 9;
		public int WhiteMageMaxMP { get; set; } = 9;
		public int BlackMageMaxMP { get; set; } = 9;
		public int KnightMaxMP { get; set; } = 4;
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
			+ ((IncentivizeMasamune ?? false) ? 1 : 0)
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
			+ ((IncentivizeMasamune ?? true) ? 1 : 0)
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
			+ ((IncentivizeMasamune != null) ? (IncentivizeMasamune ?? false ? "Masmune\U0001F5E1 " : "") : ("Masmune?\U0001F5E1 "))
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

		public bool? FreeLute => ChaosRush | ShortToFR;

		public bool? DeepCastlesPossible => Entrances & Floors;
		public bool? DeepTownsPossible => Towns & Entrances & Floors & EntrancesMixedWithTowns;

		public bool EnemizerEnabled => (bool)RandomizeFormationEnemizer | (bool)RandomizeEnemizer;
		public bool EnemizerDontMakeNewScripts => (bool)EnemySkillsSpells & !((bool)BossSkillsOnly | (bool)EnemySkillsSpellsTiered);

		public static string EncodeFlagsText(Flags flags)
		{
			BigInteger sum = 0;
			sum = AddString(sum, 7, (FFRVersion.Sha.Length >= 7) ? FFRVersion.Sha.Substring(0,7) : FFRVersion.Sha.PadRight(7, 'X'));


			sum = AddNumeric(sum, Enum.GetValues(typeof(EvadeCapValues)).Cast<int>().Max() + 1, (int)flags.EvadeCap);
			sum = AddTriState(sum, flags.IncentivizeKatana);
			sum = AddTriState(sum, flags.IncentivizeVorpal);
			sum = AddTriState(sum, flags.Shops);
			sum = AddTriState(sum, flags.Treasures);
			sum = AddTriState(sum, flags.NPCItems);
			sum = AddTriState(sum, flags.NPCFetchItems);
			sum = AddTriState(sum, flags.RandomWares);
			sum = AddTriState(sum, flags.RandomLoot);
			sum = AddBoolean(sum, flags.ShardHunt);
			sum = AddNumeric(sum, Enum.GetValues(typeof(ShardCount)).Cast<int>().Max() + 1, (int)flags.ShardCount);
			sum = AddTriState(sum, flags.TransformFinalFormation);
			sum = AddBoolean(sum, flags.ChaosRush);
			sum = AddTriState(sum, flags.ShortToFR);
			sum = AddTriState(sum, flags.PreserveFiendRefights);
			sum = AddTriState(sum, flags.PreserveAllFiendRefights);
			sum = AddTriState(sum, flags.MagicShops);
			sum = AddTriState(sum, flags.MagicShopLocs);
			sum = AddTriState(sum, flags.MagicLevels);
			sum = AddTriState(sum, flags.MagicPermissions);
			sum = AddTriState(sum, flags.ItemMagic);
			sum = AddTriState(sum, flags.MagisizeWeapons);
			sum = AddTriState(sum, flags.MagisizeWeaponsBalanced);
			sum = AddTriState(sum, flags.MagicLevelsTiered);
			sum = AddTriState(sum, flags.MagicLevelsMixed);
			sum = AddNumeric(sum, Enum.GetValues(typeof(AutohitThreshold)).Cast<int>().Max() + 1, (int)flags.MagicAutohitThreshold);
			sum = AddTriState(sum, flags.Rng);
			sum = AddBoolean(sum, flags.FixMissingBattleRngEntry);
			sum = AddTriState(sum, flags.EverythingUnrunnable);
			sum = AddTriState(sum, flags.EverythingRunnable);
			sum = AddTriState(sum, flags.EnemyFormationsUnrunnable);
			sum = AddTriState(sum, flags.EnemyFormationsSurprise);
			sum = AddTriState(sum, flags.UnrunnablesStrikeFirstAndSurprise);
			sum = AddTriState(sum, flags.EnemyTrapTiles);
			sum = AddTriState(sum, flags.RemoveTrapTiles);
			sum = AddTriState(sum, flags.RandomTrapFormations);
			sum = AddTriState(sum, flags.SwolePirates);
			sum = AddTriState(sum, flags.EnemyScripts);
			sum = AddTriState(sum, flags.BossScriptsOnly);
			sum = AddTriState(sum, flags.EnemySkillsSpells);
			sum = AddTriState(sum, flags.BossSkillsOnly);
			sum = AddTriState(sum, flags.EnemySkillsSpellsTiered);
			sum = AddTriState(sum, flags.EnemyStatusAttacks);
			sum = AddTriState(sum, flags.RandomStatusAttacks);
			sum = AddTriState(sum, flags.AllowUnsafePirates);
			sum = AddTriState(sum, flags.AllowUnsafeMelmond);
			sum = AddNumeric(sum, Enum.GetValues(typeof(WarMECHMode)).Cast<int>().Max() + 1, (int)flags.WarMECHMode);
			sum = AddTriState(sum, flags.OrdealsPillars);
			sum = AddNumeric(sum, Enum.GetValues(typeof(SkyCastle4FMazeMode)).Cast<int>().Max() + 1, (int)flags.SkyCastle4FMazeMode);
			sum = AddTriState(sum, flags.TitansTrove);
			sum = AddTriState(sum, flags.LefeinShops);
			sum = AddTriState(sum, flags.ConfusedOldMen);
			sum = AddBoolean(sum, flags.SpookyFlag);
			sum = AddTriState(sum, flags.MapOpenProgression);
			sum = AddTriState(sum, flags.MapOpenProgressionDocks);
			sum = AddTriState(sum, flags.Entrances);
			sum = AddTriState(sum, flags.Towns);
			sum = AddTriState(sum, flags.Floors);
			sum = AddTriState(sum, flags.AllowDeepCastles);
			sum = AddTriState(sum, flags.AllowDeepTowns);
			sum = AddTriState(sum, flags.MapOpenProgressionExtended);
			sum = AddTriState(sum, flags.MapDwarvesNorthwest);
			sum = AddTriState(sum, flags.MapAirshipDock);
			sum = AddTriState(sum, flags.EntrancesIncludesDeadEnds);
			sum = AddTriState(sum, flags.EntrancesMixedWithTowns);
			sum = AddTriState(sum, flags.IncentivizeFreeNPCs);
			sum = AddTriState(sum, flags.IncentivizeFetchNPCs);
			sum = AddTriState(sum, flags.IncentivizeTail);
			sum = AddTriState(sum, flags.IncentivizeMainItems);
			sum = AddTriState(sum, flags.IncentivizeFetchItems);
			sum = AddTriState(sum, flags.IncentivizeAirship);
			sum = AddTriState(sum, flags.IncentivizeCanoeItem);
			sum = AddTriState(sum, flags.IncentivizeShipAndCanal);
			sum = AddBoolean(sum, flags.ClassicItemPlacement);
			sum = AddTriState(sum, flags.IncentivizeMarsh);
			sum = AddTriState(sum, flags.IncentivizeEarth);
			sum = AddTriState(sum, flags.IncentivizeVolcano);
			sum = AddTriState(sum, flags.IncentivizeIceCave);
			sum = AddTriState(sum, flags.IncentivizeOrdeals);
			sum = AddTriState(sum, flags.IncentivizeSeaShrine);
			sum = AddTriState(sum, flags.IncentivizeConeria);
			sum = AddTriState(sum, flags.IncentivizeMarshKeyLocked);
			sum = AddTriState(sum, flags.IncentivizeSkyPalace);
			sum = AddTriState(sum, flags.IncentivizeTitansTrove);
			sum = AddTriState(sum, flags.BetterTrapChests);
			sum = AddTriState(sum, flags.IncentivizeMasamune);
			sum = AddTriState(sum, flags.IncentivizeOpal);
			sum = AddTriState(sum, flags.IncentivizeRibbon);
			sum = AddTriState(sum, flags.IncentivizeDefCastArmor);
			sum = AddTriState(sum, flags.IncentivizeOffCastArmor);
			sum = AddTriState(sum, flags.IncentivizeOtherCastArmor);
			sum = AddTriState(sum, flags.IncentivizeDefCastWeapon);
			sum = AddTriState(sum, flags.IncentivizeOffCastWeapon);
			sum = AddBoolean(sum, flags.IncentivizeOtherCastWeapon);
			sum = AddTriState(sum, flags.EarlyKing);
			sum = AddTriState(sum, flags.EarlySarda);
			sum = AddTriState(sum, flags.EarlySage);
			sum = AddTriState(sum, flags.EarlyOrdeals);
			sum = AddTriState(sum, flags.ShuffleObjectiveNPCs);
			sum = AddBoolean(sum, flags.OnlyRequireGameIsBeatable);
			sum = AddTriState(sum, flags.FreeBridge);
			sum = AddTriState(sum, flags.FreeShip);
			sum = AddTriState(sum, flags.FreeAirship);
			sum = AddBoolean(sum, flags.FreeOrbs);
			sum = AddBoolean(sum, flags.EnableCritNumberDisplay);
			sum = AddTriState(sum, flags.FreeCanoe);
			sum = AddTriState(sum, flags.FreeCanal);
			sum = AddBoolean(sum, flags.EasyMode);
			sum = AddBoolean(sum, flags.HousesFillHp);
			sum = AddBoolean(sum, flags.SpeedHacks);
			sum = AddBoolean(sum, flags.NoPartyShuffle);
			sum = AddBoolean(sum, flags.Dash);
			sum = AddBoolean(sum, flags.BuyTen);
			sum = AddBoolean(sum, flags.BuyTenOld);
			sum = AddBoolean(sum, flags.IdentifyTreasures);
			sum = AddBoolean(sum, flags.ShopInfo);
			sum = AddBoolean(sum, flags.WaitWhenUnrunnable);
			sum = AddBoolean(sum, flags.HouseMPRestoration);
			sum = AddBoolean(sum, flags.WeaponStats);
			sum = AddBoolean(sum, flags.BBCritRate);
			sum = AddBoolean(sum, flags.WeaponCritRate);
			sum = AddBoolean(sum, flags.WeaponBonuses);
			sum = AddBoolean(sum, flags.ChanceToRun);
			sum = AddBoolean(sum, flags.SpellBugs);
			sum = AddBoolean(sum, flags.BlackBeltAbsorb);
			sum = AddBoolean(sum, flags.InventoryAutosort);
			sum = AddBoolean(sum, flags.BattleMagicMenuWrapAround);
			sum = AddBoolean(sum, flags.NPCSwatter);
			sum = AddBoolean(sum, flags.EnemyStatusAttackBug);
			sum = AddBoolean(sum, flags.EnemySpellsTargetingAllies);
			sum = AddBoolean(sum, flags.EnemyElementalResistancesBug);
			sum = AddBoolean(sum, flags.ImproveTurnOrderRandomization);
			sum = AddBoolean(sum, flags.FixHitChanceCap);
			sum = AddBoolean(sum, flags.StartingGold);
			sum = AddBoolean(sum, flags.WrapStatOverflow);
			sum = AddBoolean(sum, flags.WrapPriceOverflow);
			sum = AddBoolean(sum, flags.IncludeMorale);
			sum = AddTriState(sum, flags.RandomWaresIncludesSpecialGear);
			sum = AddBoolean(sum, flags.NoDanMode);
			sum = AddTriState(sum, flags.NoTail);
			sum = AddTriState(sum, flags.ClassAsNpcFiends);
			sum = AddTriState(sum, flags.ClassAsNpcKeyNPC);
			sum = AddNumeric(sum, 14, flags.ClassAsNpcCount);
			sum = AddBoolean(sum, flags.ClassAsNpcDuplicate);
			sum = AddTriState(sum, flags.ClassAsNpcForcedFiends);
			sum = AddBoolean(sum, flags.ClassAsNpcPromotion);
			sum = AddNumeric(sum, 51, flags.EnemyScaleStatsLow / 10);
			sum = AddNumeric(sum, 51, flags.EnemyScaleStatsHigh/ 10);
			sum = AddNumeric(sum, 51, flags.EnemyScaleHpLow/ 10);
			sum = AddNumeric(sum, 51, flags.EnemyScaleHpHigh/ 10);
			sum = AddNumeric(sum, 51, flags.BossScaleStatsLow/ 10);
			sum = AddNumeric(sum, 51, flags.BossScaleStatsHigh/ 10);
			sum = AddNumeric(sum, 51, flags.BossScaleHpLow/ 10);
			sum = AddNumeric(sum, 51, flags.BossScaleHpHigh/ 10);
			sum = AddNumeric(sum, 41, (int)(10.0 * flags.PriceScaleFactor) - 10);
			sum = AddNumeric(sum, 41, (int)(10.0 * flags.ExpMultiplier) - 10);
			sum = AddNumeric(sum, 51, (int)(flags.ExpBonus / 10.0));
			sum = AddNumeric(sum, 46, (int)flags.EncounterRate);
			sum = AddNumeric(sum, 46, (int)flags.DungeonEncounterRate);
			sum = AddNumeric(sum, Enum.GetValues(typeof(ProgressiveScaleMode)).Cast<int>().Max() + 1, (int)flags.ProgressiveScaleMode);
			sum = AddTriState(sum, flags.FIGHTER1);
			sum = AddTriState(sum, flags.THIEF1);
			sum = AddTriState(sum, flags.BLACK_BELT1);
			sum = AddTriState(sum, flags.RED_MAGE1);
			sum = AddTriState(sum, flags.WHITE_MAGE1);
			sum = AddTriState(sum, flags.BLACK_MAGE1);
			sum = AddTriState(sum, flags.FIGHTER2);
			sum = AddTriState(sum, flags.THIEF2);
			sum = AddTriState(sum, flags.BLACK_BELT2);
			sum = AddTriState(sum, flags.RED_MAGE2);
			sum = AddTriState(sum, flags.WHITE_MAGE2);
			sum = AddTriState(sum, flags.BLACK_MAGE2);
			sum = AddTriState(sum, flags.FIGHTER3);
			sum = AddTriState(sum, flags.THIEF3);
			sum = AddTriState(sum, flags.BLACK_BELT3);
			sum = AddTriState(sum, flags.RED_MAGE3);
			sum = AddTriState(sum, flags.WHITE_MAGE3);
			sum = AddTriState(sum, flags.BLACK_MAGE3);
			sum = AddTriState(sum, flags.FIGHTER4);
			sum = AddTriState(sum, flags.THIEF4);
			sum = AddTriState(sum, flags.BLACK_BELT4);
			sum = AddTriState(sum, flags.RED_MAGE4);
			sum = AddTriState(sum, flags.WHITE_MAGE4);
			sum = AddTriState(sum, flags.BLACK_MAGE4);
			sum = AddTriState(sum, flags.KNIGHT1);
			sum = AddTriState(sum, flags.KNIGHT2);
			sum = AddTriState(sum, flags.KNIGHT3);
			sum = AddTriState(sum, flags.KNIGHT4);
			sum = AddTriState(sum, flags.NINJA1);
			sum = AddTriState(sum, flags.NINJA2);
			sum = AddTriState(sum, flags.NINJA3);
			sum = AddTriState(sum, flags.NINJA4);
			sum = AddTriState(sum, flags.MASTER1);
			sum = AddTriState(sum, flags.MASTER2);
			sum = AddTriState(sum, flags.MASTER3);
			sum = AddTriState(sum, flags.MASTER4);
			sum = AddTriState(sum, flags.RED_WIZ1);
			sum = AddTriState(sum, flags.RED_WIZ2);
			sum = AddTriState(sum, flags.RED_WIZ3);
			sum = AddTriState(sum, flags.RED_WIZ4);
			sum = AddTriState(sum, flags.WHITE_WIZ1);
			sum = AddTriState(sum, flags.WHITE_WIZ2);
			sum = AddTriState(sum, flags.WHITE_WIZ3);
			sum = AddTriState(sum, flags.WHITE_WIZ4);
			sum = AddTriState(sum, flags.BLACK_WIZ1);
			sum = AddTriState(sum, flags.BLACK_WIZ2);
			sum = AddTriState(sum, flags.BLACK_WIZ3);
			sum = AddTriState(sum, flags.BLACK_WIZ4);
			sum = AddTriState(sum, flags.NONE_CLASS2);
			sum = AddTriState(sum, flags.NONE_CLASS3);
			sum = AddTriState(sum, flags.NONE_CLASS4);
			sum = AddTriState(sum, flags.FORCED1);
			sum = AddTriState(sum, flags.FORCED2);
			sum = AddTriState(sum, flags.FORCED3);
			sum = AddTriState(sum, flags.FORCED4);
			sum = AddTriState(sum, flags.TAVERN1);
			sum = AddTriState(sum, flags.TAVERN2);
			sum = AddTriState(sum, flags.TAVERN3);
			sum = AddTriState(sum, flags.TAVERN4);
			sum = AddTriState(sum, flags.TAVERN5);
			sum = AddTriState(sum, flags.TAVERN6);
			sum = AddBoolean(sum, flags.WeaponPermissions);
			sum = AddBoolean(sum, flags.ArmorPermissions);
			sum = AddTriState(sum, flags.RecruitmentMode);
			sum = AddTriState(sum, flags.RecruitmentModeHireOnly);
			sum = AddTriState(sum, flags.RecruitmentModeReplaceOnlyNone);
			sum = AddTriState(sum, flags.ClampMinimumStatScale);
			sum = AddTriState(sum, flags.ClampMinimumBossStatScale);
			sum = AddTriState(sum, flags.ClampMinimumPriceScale);
			sum = AddBoolean(sum, flags.EFGWaterfall);
			sum = AddBoolean(sum, flags.EFGEarth1);
			sum = AddBoolean(sum, flags.EFGEarth2);
			sum = AddTriState(sum, flags.FiendShuffle);
			sum = AddBoolean(sum, flags.DisableTentSaving);
			sum = AddBoolean(sum, flags.DisableInnSaving);
			sum = AddTriState(sum, flags.ShuffleAstos);
			sum = AddBoolean(sum, flags.SaveGameWhenGameOver);
			sum = AddBoolean(sum, flags.SaveGameDWMode);
			sum = AddBoolean(sum, flags.PacifistMode);
			sum = AddTriState(sum, flags.RandomizeEnemizer);
			sum = AddTriState(sum, flags.RandomizeFormationEnemizer);
			sum = AddTriState(sum, flags.GenerateNewSpellbook);
			sum = AddTriState(sum, flags.SpellcrafterMixSpells);
			sum = AddBoolean(sum, flags.ThiefHitRate);
			sum = AddBoolean(sum, flags.AllSpellLevelsForKnightNinja);
			sum = AddBoolean(sum, flags.BuffHealingSpells);
			sum = AddTriState(sum, flags.FreeTail);
			sum = AddTriState(sum, flags.HintsVillage);
			sum = AddTriState(sum, flags.HintsDungeon);
			sum = AddTriState(sum, flags.HintsRngDungeon);
			sum = AddTriState(sum, flags.HintsUseless);
			sum = AddTriState(sum, flags.SpellcrafterRetainPermissions);
			sum = AddTriState(sum, flags.RandomWeaponBonus);
			sum = AddTriState(sum, flags.RandomArmorBonus);
			sum = AddTriState(sum, flags.RandomWeaponBonusExcludeMasa);
			sum = AddNumeric(sum, 19, flags.RandomWeaponBonusLow + 9);
			sum = AddNumeric(sum, 19, flags.RandomWeaponBonusHigh + 9);
			sum = AddNumeric(sum, 19, flags.RandomArmorBonusLow + 9);
			sum = AddNumeric(sum, 19, flags.RandomArmorBonusHigh + 9);
			sum = AddTriState(sum, flags.BalancedItemMagicShuffle);
			sum = AddTriState(sum, flags.SeparateBossHPScaling);
			sum = AddTriState(sum, flags.SeparateEnemyHPScaling);
			sum = AddTriState(sum, flags.ClampBossHPScaling);
			sum = AddTriState(sum, flags.ClampEnemyHpScaling);
			sum = AddNumeric(sum, 10, flags.NinjaMaxMP);
			sum = AddNumeric(sum, 10, flags.KnightMaxMP);
			sum = AddNumeric(sum, 10, flags.BlackMageMaxMP);
			sum = AddNumeric(sum, 10, flags.WhiteMageMaxMP);
			sum = AddNumeric(sum, 10, flags.RedMageMaxMP);
			sum = AddTriState(sum, flags.ChangeMaxMP);
			sum = AddNumeric(sum, Enum.GetValues(typeof(LockHitMode)).Cast<int>().Max() + 1, (int)flags.LockMode);
			sum = AddNumeric(sum, Enum.GetValues(typeof(PoolSize)).Cast<int>().Max() + 1, (int)flags.PoolSize);
			sum = AddTriState(sum, flags.EnablePoolParty);
			sum = AddTriState(sum, flags.IncludePromClasses);
			sum = AddTriState(sum, flags.EnableRandomPromotions);
			sum = AddTriState(sum, flags.IncludeBaseClasses);
			sum = AddTriState(sum, flags.RandomPromotionsSpoilers);
			sum = AddBoolean(sum, flags.LinearMPGrowth);
			sum = AddTriState(sum, flags.RandomizeClass);
			sum = AddTriState(sum, flags.RandomizeClassChaos);
			sum = AddNumeric(sum, 5, flags.RandomizeClassMaxBonus);
			sum = AddNumeric(sum, 5, flags.RandomizeClassMaxMalus);
			sum = AddNumeric(sum, Enum.GetValues(typeof(FormationShuffleMode)).Cast<int>().Max() + 1, (int)flags.FormationShuffleMode);
			sum = AddNumeric(sum, Enum.GetValues(typeof(MDEFGrowthMode)).Cast<int>().Max() + 1, (int)flags.MDefMode);
			sum = AddNumeric(sum, Enum.GetValues(typeof(WorldWealthMode)).Cast<int>().Max() + 1, (int)flags.WorldWealth);
			sum = AddTriState(sum, flags.AllowUnsafeStartArea);
			sum = AddTriState(sum, flags.EarlierRuby);
			sum = AddTriState(sum, flags.GuaranteedRuseItem);
			sum = AddTriState(sum, flags.DisableStunTouch);
			sum = AddBoolean(sum, flags.TournamentSafe);
			sum = AddBoolean(sum, flags.Spoilers);

			return BigIntegerToString(sum);
		}

		public static Flags DecodeFlagsText(string text)
		{
			var sum = StringToBigInteger(text);

			var flags = new Flags
			{
				Spoilers = GetBoolean(ref sum),
				TournamentSafe = GetBoolean(ref sum),
				DisableStunTouch = GetTriState(ref sum),
				GuaranteedRuseItem = GetTriState(ref sum),
				EarlierRuby = GetTriState(ref sum),
				AllowUnsafeStartArea = GetTriState(ref sum),
				WorldWealth = (WorldWealthMode)GetNumeric(ref sum, Enum.GetValues(typeof(WorldWealthMode)).Cast<int>().Max() + 1),
				MDefMode = (MDEFGrowthMode)GetNumeric(ref sum, Enum.GetValues(typeof(MDEFGrowthMode)).Cast<int>().Max() + 1),
				FormationShuffleMode = (FormationShuffleMode)GetNumeric(ref sum, Enum.GetValues(typeof(FormationShuffleMode)).Cast<int>().Max() + 1),
				RandomizeClassMaxMalus = GetNumeric(ref sum, 5),
				RandomizeClassMaxBonus = GetNumeric(ref sum, 5),
				RandomizeClassChaos = GetTriState(ref sum),
				RandomizeClass = GetTriState(ref sum),
				LinearMPGrowth = GetBoolean(ref sum),
				RandomPromotionsSpoilers = GetTriState(ref sum),
				IncludeBaseClasses = GetTriState(ref sum),
				EnableRandomPromotions = GetTriState(ref sum),
				IncludePromClasses = GetTriState(ref sum),
				EnablePoolParty = GetTriState(ref sum),
				PoolSize = (PoolSize)GetNumeric(ref sum, Enum.GetValues(typeof(PoolSize)).Cast<int>().Max() + 1),
				LockMode = (LockHitMode)GetNumeric(ref sum, Enum.GetValues(typeof(LockHitMode)).Cast<int>().Max() + 1),
				ChangeMaxMP = GetTriState(ref sum),
				RedMageMaxMP = GetNumeric(ref sum, 10),
				WhiteMageMaxMP = GetNumeric(ref sum, 10),
				BlackMageMaxMP = GetNumeric(ref sum, 10),
				KnightMaxMP = GetNumeric(ref sum, 10),
				NinjaMaxMP = GetNumeric(ref sum, 10),
				ClampEnemyHpScaling = GetTriState(ref sum),
				ClampBossHPScaling = GetTriState(ref sum),
				SeparateEnemyHPScaling = GetTriState(ref sum),
				SeparateBossHPScaling = GetTriState(ref sum),
				BalancedItemMagicShuffle = GetTriState(ref sum),
				RandomArmorBonusHigh = GetNumeric(ref sum, 19) - 9,
				RandomArmorBonusLow = GetNumeric(ref sum, 19) - 9,
				RandomWeaponBonusHigh = GetNumeric(ref sum, 19) - 9,
				RandomWeaponBonusLow = GetNumeric(ref sum, 19) - 9,
				RandomWeaponBonusExcludeMasa = GetTriState(ref sum),
				RandomArmorBonus = GetTriState(ref sum),
				RandomWeaponBonus = GetTriState(ref sum),
				SpellcrafterRetainPermissions = GetTriState(ref sum),
				HintsUseless = GetTriState(ref sum),
				HintsRngDungeon = GetTriState(ref sum),
				HintsDungeon = GetTriState(ref sum),
				HintsVillage = GetTriState(ref sum),
				FreeTail = GetTriState(ref sum),
				BuffHealingSpells = GetBoolean(ref sum),
				AllSpellLevelsForKnightNinja = GetBoolean(ref sum),
				ThiefHitRate = GetBoolean(ref sum),
				SpellcrafterMixSpells = GetTriState(ref sum),
				GenerateNewSpellbook = GetTriState(ref sum),
				RandomizeFormationEnemizer = GetTriState(ref sum),
				RandomizeEnemizer = GetTriState(ref sum),
				PacifistMode = GetBoolean(ref sum),
				SaveGameDWMode = GetBoolean(ref sum),
				SaveGameWhenGameOver = GetBoolean(ref sum),
				ShuffleAstos = GetTriState(ref sum),
				DisableInnSaving = GetBoolean(ref sum),
				DisableTentSaving = GetBoolean(ref sum),
				FiendShuffle = GetTriState(ref sum),
				EFGEarth2 = GetBoolean(ref sum),
				EFGEarth1 = GetBoolean(ref sum),
				EFGWaterfall = GetBoolean(ref sum),
				ClampMinimumPriceScale = GetTriState(ref sum),
				ClampMinimumBossStatScale = GetTriState(ref sum),
				ClampMinimumStatScale = GetTriState(ref sum),
				RecruitmentModeReplaceOnlyNone = GetTriState(ref sum),
				RecruitmentModeHireOnly = GetTriState(ref sum),
				RecruitmentMode = GetTriState(ref sum),
				ArmorPermissions = GetBoolean(ref sum),
				WeaponPermissions = GetBoolean(ref sum),
				TAVERN6 = GetTriState(ref sum),
				TAVERN5 = GetTriState(ref sum),
				TAVERN4 = GetTriState(ref sum),
				TAVERN3 = GetTriState(ref sum),
				TAVERN2 = GetTriState(ref sum),
				TAVERN1 = GetTriState(ref sum),
				FORCED4 = GetTriState(ref sum),
				FORCED3 = GetTriState(ref sum),
				FORCED2 = GetTriState(ref sum),
				FORCED1 = GetTriState(ref sum),
				NONE_CLASS4 = GetTriState(ref sum),
				NONE_CLASS3 = GetTriState(ref sum),
				NONE_CLASS2 = GetTriState(ref sum),
				BLACK_WIZ4 = GetTriState(ref sum),
				BLACK_WIZ3 = GetTriState(ref sum),
				BLACK_WIZ2 = GetTriState(ref sum),
				BLACK_WIZ1 = GetTriState(ref sum),
				WHITE_WIZ4 = GetTriState(ref sum),
				WHITE_WIZ3 = GetTriState(ref sum),
				WHITE_WIZ2 = GetTriState(ref sum),
				WHITE_WIZ1 = GetTriState(ref sum),
				RED_WIZ4 = GetTriState(ref sum),
				RED_WIZ3 = GetTriState(ref sum),
				RED_WIZ2 = GetTriState(ref sum),
				RED_WIZ1 = GetTriState(ref sum),
				MASTER4 = GetTriState(ref sum),
				MASTER3 = GetTriState(ref sum),
				MASTER2 = GetTriState(ref sum),
				MASTER1 = GetTriState(ref sum),
				NINJA4 = GetTriState(ref sum),
				NINJA3 = GetTriState(ref sum),
				NINJA2 = GetTriState(ref sum),
				NINJA1 = GetTriState(ref sum),
				KNIGHT4 = GetTriState(ref sum),
				KNIGHT3 = GetTriState(ref sum),
				KNIGHT2 = GetTriState(ref sum),
				KNIGHT1 = GetTriState(ref sum),
				BLACK_MAGE4 = GetTriState(ref sum),
				WHITE_MAGE4 = GetTriState(ref sum),
				RED_MAGE4 = GetTriState(ref sum),
				BLACK_BELT4 = GetTriState(ref sum),
				THIEF4 = GetTriState(ref sum),
				FIGHTER4 = GetTriState(ref sum),
				BLACK_MAGE3 = GetTriState(ref sum),
				WHITE_MAGE3 = GetTriState(ref sum),
				RED_MAGE3 = GetTriState(ref sum),
				BLACK_BELT3 = GetTriState(ref sum),
				THIEF3 = GetTriState(ref sum),
				FIGHTER3 = GetTriState(ref sum),
				BLACK_MAGE2 = GetTriState(ref sum),
				WHITE_MAGE2 = GetTriState(ref sum),
				RED_MAGE2 = GetTriState(ref sum),
				BLACK_BELT2 = GetTriState(ref sum),
				THIEF2 = GetTriState(ref sum),
				FIGHTER2 = GetTriState(ref sum),
				BLACK_MAGE1 = GetTriState(ref sum),
				WHITE_MAGE1 = GetTriState(ref sum),
				RED_MAGE1 = GetTriState(ref sum),
				BLACK_BELT1 = GetTriState(ref sum),
				THIEF1 = GetTriState(ref sum),
				FIGHTER1 = GetTriState(ref sum),
				ProgressiveScaleMode = (ProgressiveScaleMode)GetNumeric(ref sum, Enum.GetValues(typeof(ProgressiveScaleMode)).Cast<int>().Max() + 1),
				DungeonEncounterRate = GetNumeric(ref sum, 46),
				EncounterRate = GetNumeric(ref sum, 46),
				ExpBonus = GetNumeric(ref sum, 51) * 10,
				ExpMultiplier = (GetNumeric(ref sum, 41) + 10) / 10.0,
				PriceScaleFactor = (GetNumeric(ref sum, 41) + 10) / 10.0,
				BossScaleHpHigh = GetNumeric(ref sum, 51) * 10,
				BossScaleHpLow = GetNumeric(ref sum, 51) * 10,
				BossScaleStatsHigh = GetNumeric(ref sum, 51) * 10,
				BossScaleStatsLow = GetNumeric(ref sum, 51) * 10,
				EnemyScaleHpHigh = GetNumeric(ref sum, 51) * 10,
				EnemyScaleHpLow = GetNumeric(ref sum, 51) * 10,
				EnemyScaleStatsHigh = GetNumeric(ref sum, 51) * 10,
				EnemyScaleStatsLow = GetNumeric(ref sum, 51) * 10,
				ClassAsNpcPromotion = GetBoolean(ref sum),
				ClassAsNpcForcedFiends = GetTriState(ref sum),
				ClassAsNpcDuplicate = GetBoolean(ref sum),
				ClassAsNpcCount = GetNumeric(ref sum, 14),
				ClassAsNpcKeyNPC = GetTriState(ref sum),
				ClassAsNpcFiends = GetTriState(ref sum),
				NoTail = GetTriState(ref sum),
				NoDanMode = GetBoolean(ref sum),
				RandomWaresIncludesSpecialGear = GetTriState(ref sum),
				IncludeMorale = GetBoolean(ref sum),
				WrapPriceOverflow = GetBoolean(ref sum),
				WrapStatOverflow = GetBoolean(ref sum),
				StartingGold = GetBoolean(ref sum),
				FixHitChanceCap = GetBoolean(ref sum),
				ImproveTurnOrderRandomization = GetBoolean(ref sum),
				EnemyElementalResistancesBug = GetBoolean(ref sum),
				EnemySpellsTargetingAllies = GetBoolean(ref sum),
				EnemyStatusAttackBug = GetBoolean(ref sum),
				NPCSwatter = GetBoolean(ref sum),
				BattleMagicMenuWrapAround = GetBoolean(ref sum),
				InventoryAutosort = GetBoolean(ref sum),
				BlackBeltAbsorb = GetBoolean(ref sum),
				SpellBugs = GetBoolean(ref sum),
				ChanceToRun = GetBoolean(ref sum),
				WeaponBonuses = GetBoolean(ref sum),
				WeaponCritRate = GetBoolean(ref sum),
				BBCritRate = GetBoolean(ref sum),
				WeaponStats = GetBoolean(ref sum),
				HouseMPRestoration = GetBoolean(ref sum),
				WaitWhenUnrunnable = GetBoolean(ref sum),
				ShopInfo = GetBoolean(ref sum),
				IdentifyTreasures = GetBoolean(ref sum),
				BuyTenOld = GetBoolean(ref sum),
				BuyTen = GetBoolean(ref sum),
				Dash = GetBoolean(ref sum),
				NoPartyShuffle = GetBoolean(ref sum),
				SpeedHacks = GetBoolean(ref sum),
				HousesFillHp = GetBoolean(ref sum),
				EasyMode = GetBoolean(ref sum),
				FreeCanal = GetTriState(ref sum),
				FreeCanoe = GetTriState(ref sum),
				EnableCritNumberDisplay = GetBoolean(ref sum),
				FreeOrbs = GetBoolean(ref sum),
				FreeAirship = GetTriState(ref sum),
				FreeShip = GetTriState(ref sum),
				FreeBridge = GetTriState(ref sum),
				OnlyRequireGameIsBeatable = GetBoolean(ref sum),
				ShuffleObjectiveNPCs = GetTriState(ref sum),
				EarlyOrdeals = GetTriState(ref sum),
				EarlySage = GetTriState(ref sum),
				EarlySarda = GetTriState(ref sum),
				EarlyKing = GetTriState(ref sum),
				IncentivizeOtherCastWeapon = GetBoolean(ref sum),
				IncentivizeOffCastWeapon = GetTriState(ref sum),
				IncentivizeDefCastWeapon = GetTriState(ref sum),
				IncentivizeOtherCastArmor = GetTriState(ref sum),
				IncentivizeOffCastArmor = GetTriState(ref sum),
				IncentivizeDefCastArmor = GetTriState(ref sum),
				IncentivizeRibbon = GetTriState(ref sum),
				IncentivizeOpal = GetTriState(ref sum),
				IncentivizeMasamune = GetTriState(ref sum),
				BetterTrapChests = GetTriState(ref sum),
				IncentivizeTitansTrove = GetTriState(ref sum),
				IncentivizeSkyPalace = GetTriState(ref sum),
				IncentivizeMarshKeyLocked = GetTriState(ref sum),
				IncentivizeConeria = GetTriState(ref sum),
				IncentivizeSeaShrine = GetTriState(ref sum),
				IncentivizeOrdeals = GetTriState(ref sum),
				IncentivizeIceCave = GetTriState(ref sum),
				IncentivizeVolcano = GetTriState(ref sum),
				IncentivizeEarth = GetTriState(ref sum),
				IncentivizeMarsh = GetTriState(ref sum),
				ClassicItemPlacement = GetBoolean(ref sum),
				IncentivizeShipAndCanal = GetTriState(ref sum),
				IncentivizeCanoeItem = GetTriState(ref sum),
				IncentivizeAirship = GetTriState(ref sum),
				IncentivizeFetchItems = GetTriState(ref sum),
				IncentivizeMainItems = GetTriState(ref sum),
				IncentivizeTail = GetTriState(ref sum),
				IncentivizeFetchNPCs = GetTriState(ref sum),
				IncentivizeFreeNPCs = GetTriState(ref sum),
				EntrancesMixedWithTowns = GetTriState(ref sum),
				EntrancesIncludesDeadEnds = GetTriState(ref sum),
				MapAirshipDock = GetTriState(ref sum),
				MapDwarvesNorthwest = GetTriState(ref sum),
				MapOpenProgressionExtended = GetTriState(ref sum),
				AllowDeepTowns = GetTriState(ref sum),
				AllowDeepCastles = GetTriState(ref sum),
				Floors = GetTriState(ref sum),
				Towns = GetTriState(ref sum),
				Entrances = GetTriState(ref sum),
				MapOpenProgressionDocks = GetTriState(ref sum),
				MapOpenProgression = GetTriState(ref sum),
				SpookyFlag = GetBoolean(ref sum),
				ConfusedOldMen = GetTriState(ref sum),
				LefeinShops = GetTriState(ref sum),
				TitansTrove = GetTriState(ref sum),
				SkyCastle4FMazeMode = (SkyCastle4FMazeMode)GetNumeric(ref sum, Enum.GetValues(typeof(SkyCastle4FMazeMode)).Cast<int>().Max() + 1),
				OrdealsPillars = GetTriState(ref sum),
				WarMECHMode = (WarMECHMode)GetNumeric(ref sum, Enum.GetValues(typeof(WarMECHMode)).Cast<int>().Max() + 1),
				AllowUnsafeMelmond = GetTriState(ref sum),
				AllowUnsafePirates = GetTriState(ref sum),
				RandomStatusAttacks = GetTriState(ref sum),
				EnemyStatusAttacks = GetTriState(ref sum),
				EnemySkillsSpellsTiered = GetTriState(ref sum),
				BossSkillsOnly = GetTriState(ref sum),
				EnemySkillsSpells = GetTriState(ref sum),
				BossScriptsOnly = GetTriState(ref sum),
				EnemyScripts = GetTriState(ref sum),
				SwolePirates = GetTriState(ref sum),
				RandomTrapFormations = GetTriState(ref sum),
				RemoveTrapTiles = GetTriState(ref sum),
				EnemyTrapTiles = GetTriState(ref sum),
				UnrunnablesStrikeFirstAndSurprise = GetTriState(ref sum),
				EnemyFormationsSurprise = GetTriState(ref sum),
				EnemyFormationsUnrunnable = GetTriState(ref sum),
				EverythingRunnable = GetTriState(ref sum),
				EverythingUnrunnable = GetTriState(ref sum),
				FixMissingBattleRngEntry = GetBoolean(ref sum),
				Rng = GetTriState(ref sum),
				MagicAutohitThreshold = (AutohitThreshold)GetNumeric(ref sum, Enum.GetValues(typeof(AutohitThreshold)).Cast<int>().Max() + 1),
				MagicLevelsMixed = GetTriState(ref sum),
				MagicLevelsTiered = GetTriState(ref sum),
				MagisizeWeaponsBalanced = GetTriState(ref sum),
				MagisizeWeapons = GetTriState(ref sum),
				ItemMagic = GetTriState(ref sum),
				MagicPermissions = GetTriState(ref sum),
				MagicLevels = GetTriState(ref sum),
				MagicShopLocs = GetTriState(ref sum),
				MagicShops = GetTriState(ref sum),
				PreserveAllFiendRefights = GetTriState(ref sum),
				PreserveFiendRefights = GetTriState(ref sum),
				ShortToFR = GetTriState(ref sum),
				ChaosRush = GetBoolean(ref sum),
				TransformFinalFormation = GetTriState(ref sum),
				ShardCount = (ShardCount)GetNumeric(ref sum, Enum.GetValues(typeof(ShardCount)).Cast<int>().Max() + 1),
				ShardHunt = GetBoolean(ref sum),
				RandomLoot = GetTriState(ref sum),
				RandomWares = GetTriState(ref sum),
				NPCFetchItems = GetTriState(ref sum),
				NPCItems = GetTriState(ref sum),
				Treasures = GetTriState(ref sum),
				Shops = GetTriState(ref sum),
				IncentivizeVorpal = GetTriState(ref sum),
				IncentivizeKatana = GetTriState(ref sum),
				EvadeCap = (EvadeCapValues)GetNumeric(ref sum, Enum.GetValues(typeof(EvadeCapValues)).Cast<int>().Max() + 1),
			};
			string EncodedSha = GetString(ref sum, 7);
			if (((FFRVersion.Sha.Length >= 7) ? FFRVersion.Sha.Substring(0, 7) : FFRVersion.Sha.PadRight(7, 'X')) != EncodedSha)
			{
				throw new Exception("The encoded version does not match the expected version");
			}

			return flags;
		}

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

		private const string Base64Chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789!-";

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
