using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Newtonsoft.Json;
using RomUtilities;

namespace FF1Lib
{
	public class Flags : IIncentiveFlags, IMapEditFlags, IScaleFlags, IFloorShuffleFlags
	{
		public bool Spoilers { get; set; }

		public bool Shops { get; set; }
		public bool Treasures { get; set; }
		public bool NPCItems { get; set; }
		public bool NPCFetchItems { get; set; }
		public bool RandomWares { get; set; }
		public bool RandomWaresIncludesSpecialGear { get; set; }
		public bool RandomLoot { get; set; }

		public bool ShardHunt { get; set; }
		public bool ExtraShards { get; set; }
		public bool TransformFinalFormation { get; set; }
		public bool ChaosRush { get; set; }
		public bool ShortToFR { get; set; }
		public bool PreserveFiendRefights { get; set; }

		public bool MagicShops { get; set; }
		public bool MagicLevels { get; set; }
		public bool MagicPermissions { get; set; }
		public bool ItemMagic { get; set; }
		public bool RebalanceSpells { get; set; }

		public bool Rng { get; set; }
		public bool EverythingUnrunnable { get; set; }
		public bool EnemyFormationsUnrunnable { get; set; }
		public bool EnemyFormationsSurprise { get; set; }
		public bool EnemyTrapTiles { get; set; }
		public bool RandomTrapFormations { get; set; }

		public bool EnemyScripts { get; set; }
		public bool EnemySkillsSpells { get; set; }
		public bool EnemyStatusAttacks { get; set; }
		public bool AllowUnsafePirates { get; set; }
		public bool AllowUnsafeMelmond { get; set; }

		public WarMECHMode WarMECHMode { get; set; }
		public bool OrdealsPillars { get; set; }
		public bool SkyCastle4FTeleporters { get; set; }
		public bool TitansTrove { get; set; }
		public bool LefeinShops { get; set; }
		public bool ConfusedOldMen { get; set; }

		public bool MapOpenProgression { get; set; }
		public bool Entrances { get; set; }
		public bool Towns { get; set; }
		public bool Floors { get; set; }
		public bool AllowDeepCastles { get; set; }
		public bool MapOpenProgressionExtended { get; set; }

		public bool EntrancesIncludesDeadEnds { get; set; }
		public bool EntrancesMixedWithTowns { get; set; }

		public bool IncentivizeFreeNPCs { get; set; }
		public bool IncentivizeFetchNPCs { get; set; }
		public bool IncentivizeTail { get; set; }
		public bool IncentivizeMainItems { get; set; }
		public bool IncentivizeFetchItems { get; set; }
		public bool IncentivizeCanoeItem { get; set; }
		public bool IncentivizeAirship { get; set; }
		public bool IncentivizeShipAndCanal { get; set; }
		public bool ClassicItemPlacement { get; set; }

		public bool IncentivizeMarsh { get; set; }
		public bool IncentivizeEarth { get; set; }
		public bool IncentivizeVolcano { get; set; }
		public bool IncentivizeIceCave { get; set; }
		public bool IncentivizeOrdeals { get; set; }
		public bool IncentivizeSeaShrine { get; set; }

		public bool IncentivizeConeria { get; set; }
		public bool IncentivizeMarshKeyLocked { get; set; }
		public bool IncentivizeSkyPalace { get; set; }
		public bool IncentivizeTitansTrove { get; set; }
		public bool BetterTrapChests { get; set; }


		public bool IncentivizeMasamune { get; set; }
		public bool IncentivizeOpal { get; set; }
		public bool IncentivizeRibbon { get; set; }
		public bool IncentivizeRibbon2 { get; set; }
		public bool Incentivize65K { get; set; }
		public bool IncentivizeBad { get; set; }

		public bool IncentivizeDefCastArmor { get; set; }
		public bool IncentivizeOffCastArmor { get; set; }
		public bool IncentivizeOtherCastArmor { get; set; }
		public bool IncentivizeDefCastWeapon { get; set; }
		public bool IncentivizeOffCastWeapon { get; set; }
		public bool IncentivizeOtherCastWeapon { get; set; }

		public bool EarlySarda { get; set; }
		public bool EarlySage { get; set; }
		public bool EarlyOrdeals { get; set; }
		public bool ShuffleObjectiveNPCs { get; set; }
		public bool OnlyRequireGameIsBeatable { get; set; }

		public bool FreeBridge { get; set; }
		public bool FreeAirship { get; set; }
		public bool FreeOrbs { get; set; }
		public bool EnableCritNumberDisplay { get; set; }
		public bool FreeCanal { get; set; }
		public bool EasyMode { get; set; }

		public bool HousesFillHp { get; set; }

		public bool SpeedHacks { get; set; }
		public bool NoPartyShuffle { get; set; }
		public bool Dash { get; set; }
		public bool BuyTen { get; set; }
		public bool IdentifyTreasures { get; set; }
		public bool WaitWhenUnrunnable { get; set; }

		public bool HouseMPRestoration { get; set; }
		public bool WeaponStats { get; set; }
		public bool ChanceToRun { get; set; }
		public bool SpellBugs { get; set; }
		public bool BlackBeltAbsorb { get; set; }
		public bool BlackBeltMDEF { get; set; }
		public bool NPCSwatter { get; set; }

		public bool EnemyStatusAttackBug { get; set; }
		public bool EnemySpellsTargetingAllies { get; set; }
		public bool EnemyElementalResistancesBug { get; set; }
		public bool ImproveTurnOrderRandomization { get; set; }

		public bool StartingGold { get; set; }
		public bool WrapStatOverflow { get; set; }
		public bool WrapPriceOverflow { get; set; }
		public bool IncludeMorale { get; set; }
		public bool NoDanMode { get; set; }

		public double EnemyScaleFactor { get; set; }
		public double BossScaleFactor { get; set; }
		public double PriceScaleFactor { get; set; }
		public double ExpMultiplier { get; set; }
		public int ExpBonus { get; set; }
		public double EncounterRate { get; set; }
		public double DungeonEncounterRate { get; set; }
		public ProgressiveScaleMode ProgressiveScaleMode { get; set; }

		public bool FIGHTER1 { get; set; }
		public bool THIEF1 { get; set; }
		public bool BLACK_BELT1 { get; set; }
		public bool RED_MAGE1 { get; set; }
		public bool WHITE_MAGE1 { get; set; }
		public bool BLACK_MAGE1 { get; set; }

		public bool FIGHTER2 { get; set; }
		public bool THIEF2 { get; set; }
		public bool BLACK_BELT2 { get; set; }
		public bool RED_MAGE2 { get; set; }
		public bool WHITE_MAGE2 { get; set; }
		public bool BLACK_MAGE2 { get; set; }

		public bool FIGHTER3 { get; set; }
		public bool THIEF3 { get; set; }
		public bool BLACK_BELT3 { get; set; }
		public bool RED_MAGE3 { get; set; }
		public bool WHITE_MAGE3 { get; set; }
		public bool BLACK_MAGE3 { get; set; }

		public bool FIGHTER4 { get; set; }
		public bool THIEF4 { get; set; }
		public bool BLACK_BELT4 { get; set; }
		public bool RED_MAGE4 { get; set; }
		public bool WHITE_MAGE4 { get; set; }
		public bool BLACK_MAGE4 { get; set; }

		public bool NONE_CLASS2 { get; set; }
		public bool NONE_CLASS3 { get; set; }
		public bool NONE_CLASS4 { get; set; }
		public bool FORCED1 { get; set; }
		public bool FORCED2 { get; set; }
		public bool FORCED3 { get; set; }

		public bool FORCED4 { get; set; }
		public bool WeaponPermissions { get; set; }
		public bool ArmorPermissions { get; set; }
		public bool RecruitmentMode { get; set; }

		public bool ClampMinimumStatScale { get; set; }
		public bool ClampMinimumBossStatScale { get; set; }
		public bool ClampMinimumPriceScale { get; set; }

		public bool ExperimentalFloorGeneration { get; set; }
		public bool FiendShuffle { get; set; }

		public FormationShuffleModeEnum FormationShuffleMode { get; set; }

		public WorldWealth WorldWealth { get; set; }

		public bool AllowStartAreaDanager { get; set; } = false;

		public bool MapCanalBridge => NPCItems || NPCFetchItems || MapOpenProgressionExtended;
		public bool MapOnracDock => MapOpenProgression;
		public bool MapMirageDock => MapOpenProgression;
		public bool MapConeriaDwarves => MapOpenProgression;
		public bool MapVolcanoIceRiver => MapOpenProgression;
		public bool MapDwarvesNorthwest => MapOpenProgressionExtended;
		public bool MapAirshipDock => MapOpenProgressionExtended;

		// The philosophy governing item incentivizations works something like this:
		// 1. If the item is NOT being shuffled to another location it cannot be incentivized. (Duh)
		// 2. If the item is required to unlock any location OR is given to a not-shuffled NPC who gives
		//    such an item in return it is considered a MAIN item. (e.g. CROWN/SLAB with vanilla fetch quests)
		// 3. If the item is given to an NPC who themselves is shuffled it's considered a FETCH item.
		// 4. The vehicles now have their own incentivization flags apart from other progression items.

		// Ruby is required if Sarda is Required for the ROD
		public bool RequiredRuby => !EarlySage && !NPCItems;
		public bool IncentivizeRuby => (RequiredRuby && IncentivizeMainItems) || (!RequiredRuby && IncentivizeFetchItems);

		// If Canoe and Fetch Quests are unshuffled then TNT is required
		public bool RequiredTnt => !NPCFetchItems && !NPCItems;
		public bool IncentivizeTnt => (RequiredTnt && IncentivizeMainItems) || (!RequiredTnt && IncentivizeFetchItems);

		public bool IncentivizeCrown => (!NPCFetchItems && IncentivizeMainItems) || (NPCFetchItems && IncentivizeFetchItems);
		public bool IncentivizeSlab => (!NPCFetchItems && IncentivizeMainItems) || (NPCFetchItems && IncentivizeFetchItems);
		public bool IncentivizeBottle => (!NPCFetchItems && IncentivizeMainItems) || (NPCFetchItems && IncentivizeFetchItems);

		public bool IncentivizeBridge => false;
		public bool IncentivizeCanoe => NPCItems && IncentivizeCanoeItem;
		public bool IncentivizeLute => NPCItems && !(ShortToFR || ChaosRush) && IncentivizeMainItems;
		public bool IncentivizeShip => NPCItems && IncentivizeShipAndCanal;
		public bool IncentivizeRod => NPCItems && IncentivizeMainItems;
		public bool IncentivizeCube => NPCItems && IncentivizeMainItems;
		public bool IncentivizeFloater => !FreeAirship && IncentivizeAirship;

		public bool IncentivizeCanal => NPCFetchItems && IncentivizeShipAndCanal && !FreeCanal;
		public bool IncentivizeCrystal => NPCFetchItems && IncentivizeFetchItems;
		public bool IncentivizeHerb => NPCFetchItems && IncentivizeFetchItems;
		public bool IncentivizeKey => NPCFetchItems && IncentivizeMainItems;
		public bool IncentivizeChime => NPCFetchItems && IncentivizeMainItems;
		public bool IncentivizeOxyale => NPCFetchItems && IncentivizeMainItems;

		public bool IncentivizeAdamant => IncentivizeFetchItems;
		public bool IncentivizeXcalber => false;

		public int IncentivizedItemCount => 0
			+ (IncentivizeTail ? 1 : 0)
			+ (IncentivizeMasamune ? 1 : 0)
			+ (IncentivizeOpal ? 1 : 0)
			+ (IncentivizeRibbon ? 1 : 0)
			+ (IncentivizeRibbon2 ? 1 : 0)
			+ (Incentivize65K ? 1 : 0)
			+ (IncentivizeBad ? 1 : 0)
			+ (IncentivizeDefCastArmor ? 1 : 0)
			+ (IncentivizeOffCastArmor ? 1 : 0)
			+ (IncentivizeOtherCastArmor ? 1 : 0)
			+ (IncentivizeDefCastWeapon ? 1 : 0)
			+ (IncentivizeOffCastWeapon ? 1 : 0)
			+ (IncentivizeOtherCastWeapon ? 1 : 0)
			+ (IncentivizeAdamant ? 1 : 0)
			+ (IncentivizeRuby ? 1 : 0)
			+ (IncentivizeCrown ? 1 : 0)
			+ (IncentivizeTnt ? 1 : 0)
			+ (IncentivizeSlab ? 1 : 0)
			+ (IncentivizeBottle ? 1 : 0)
			+ (IncentivizeFloater ? 1 : 0)
			+ (IncentivizeBridge ? 1 : 0)
			+ (IncentivizeLute ? 1 : 0)
			+ (IncentivizeShip ? 1 : 0)
			+ (IncentivizeRod ? 1 : 0)
			+ (IncentivizeCanoe ? 1 : 0)
			+ (IncentivizeCube ? 1 : 0)
			+ (IncentivizeCrystal ? 1 : 0)
			+ (IncentivizeHerb ? 1 : 0)
			+ (IncentivizeKey ? 1 : 0)
			+ (IncentivizeCanal ? 1 : 0)
			+ (IncentivizeChime ? 1 : 0)
			+ (IncentivizeOxyale ? 1 : 0)
			+ (IncentivizeXcalber ? 1 : 0);

		public string IncentivizedItems => ""
			+ (IncentivizeAdamant ? "Adamant " : "")
			+ (IncentivizeBridge ? "Bridge " : "")
			+ (IncentivizeBottle ? "Bottle " : "")
			+ (IncentivizeCanal ? "Canal " : "")
			+ (IncentivizeCanoe ? "Canoe " : "")
			+ (IncentivizeChime ? "Chime " : "")
			+ (IncentivizeBad ? "Cloth " : "")
			+ (IncentivizeCrown ? "Crown " : "")
			+ (IncentivizeCrystal ? "Crystal " : "")
			+ (IncentivizeCube ? "Cube " : "")
			+ (IncentivizeDefCastWeapon ? "Defense " : "")
			+ (IncentivizeFloater ? "Floater " : "")
			+ (IncentivizeHerb ? "Herb " : "")
			+ (IncentivizeKey ? "Key " : "")
			+ (IncentivizeLute ? "Lute " : "")
			+ (IncentivizeOtherCastWeapon ? "Mage " : "")
			+ (IncentivizeMasamune ? "Masmune " : "")
			+ (IncentivizeOpal ? "Opal " : "")
			+ (IncentivizeOxyale ? "Oxyale " : "")
			+ (IncentivizeOffCastArmor ? "Power " : "")
			+ (IncentivizeRibbon ? "Ribbon " : "")
			+ (IncentivizeRibbon2 ? "Ribbon " : "")
			+ (IncentivizeRod ? "Rod " : "")
			+ (IncentivizeRuby ? "Ruby " : "")
			+ (IncentivizeShip ? "Ship " : "")
			+ (IncentivizeSlab ? "Slab " : "")
			+ (IncentivizeTail ? "Tail " : "")
			+ (IncentivizeOffCastWeapon ? "Thor " : "")
			+ (IncentivizeTnt ? "Tnt " : "")
			+ (IncentivizeDefCastArmor ? "White " : "")
			+ (IncentivizeXcalber ? "XCalber " : "")
			+ (IncentivizeOtherCastArmor ? "Zeus " : "")
			+ (Incentivize65K ? "65000G " : "");

		public bool IncentivizeKingConeria => NPCItems && IncentivizeFreeNPCs;
		public bool IncentivizePrincess => NPCItems && IncentivizeFreeNPCs;
		public bool IncentivizeBikke => NPCItems && IncentivizeFreeNPCs;
		public bool IncentivizeSarda => NPCItems && IncentivizeFreeNPCs;
		public bool IncentivizeCanoeSage => NPCItems && IncentivizeFreeNPCs;
		public bool IncentivizeCaravan => NPCItems && IncentivizeFreeNPCs;
		public bool IncentivizeCubeBot => NPCItems && IncentivizeFreeNPCs;

		public bool IncentivizeFairy => NPCFetchItems && IncentivizeFetchNPCs;
		public bool IncentivizeAstos => NPCFetchItems && IncentivizeFetchNPCs;
		public bool IncentivizeMatoya => NPCFetchItems && IncentivizeFetchNPCs;
		public bool IncentivizeElfPrince => NPCFetchItems && IncentivizeFetchNPCs;
		public bool IncentivizeNerrick => NPCFetchItems && IncentivizeFetchNPCs;
		public bool IncentivizeLefein => NPCFetchItems && IncentivizeFetchNPCs;
		public bool IncentivizeSmith => NPCFetchItems && IncentivizeFetchNPCs;

		public int IncentivizedLocationCount => 0
			+ (NPCItems && IncentivizeFreeNPCs ? 7 : 0)
			+ (NPCFetchItems && IncentivizeFetchNPCs ? 7 : 0)
			+ (IncentivizeMarsh ? 1 : 0)
			+ (IncentivizeEarth ? 1 : 0)
			+ (IncentivizeVolcano ? 1 : 0)
			+ (IncentivizeIceCave ? 1 : 0)
			+ (IncentivizeOrdeals ? 1 : 0)
			+ (IncentivizeSeaShrine ? 1 : 0)
			+ (IncentivizeConeria ? 1 : 0)
			+ (IncentivizeMarshKeyLocked ? 1 : 0)
			+ (IncentivizeTitansTrove ? 1 : 0)
			+ (IncentivizeSkyPalace ? 1 : 0);

		public bool ImmediatePureAndSoftRequired => EnemyStatusAttacks || Entrances || MapOpenProgression;

		public static string EncodeFlagsText(Flags flags)
		{
			BigInteger sum = 0;

			sum = AddBoolean(sum, flags.Shops);
			sum = AddBoolean(sum, flags.Treasures);
			sum = AddBoolean(sum, flags.NPCItems);
			sum = AddBoolean(sum, flags.NPCFetchItems);
			sum = AddBoolean(sum, flags.RandomWares);
			sum = AddBoolean(sum, flags.RandomLoot);
			sum = AddBoolean(sum, flags.ShardHunt);
			sum = AddBoolean(sum, flags.ExtraShards);
			sum = AddBoolean(sum, flags.TransformFinalFormation);
			sum = AddBoolean(sum, flags.ChaosRush);
			sum = AddBoolean(sum, flags.ShortToFR);
			sum = AddBoolean(sum, flags.PreserveFiendRefights);
			sum = AddBoolean(sum, flags.MagicShops);
			sum = AddBoolean(sum, flags.MagicLevels);
			sum = AddBoolean(sum, flags.MagicPermissions);
			sum = AddBoolean(sum, flags.ItemMagic);
			sum = AddBoolean(sum, flags.RebalanceSpells);
			sum = AddBoolean(sum, flags.Rng);
			sum = AddBoolean(sum, flags.EverythingUnrunnable);
			sum = AddBoolean(sum, flags.EnemyFormationsUnrunnable);
			sum = AddBoolean(sum, flags.EnemyFormationsSurprise);
			sum = AddBoolean(sum, flags.EnemyTrapTiles);
			sum = AddBoolean(sum, flags.RandomTrapFormations);
			sum = AddBoolean(sum, flags.EnemyScripts);
			sum = AddBoolean(sum, flags.EnemySkillsSpells);
			sum = AddBoolean(sum, flags.EnemyStatusAttacks);
			sum = AddBoolean(sum, flags.AllowUnsafePirates);
			sum = AddBoolean(sum, flags.AllowUnsafeMelmond);
			sum = AddNumeric(sum, Enum.GetValues(typeof(WarMECHMode)).Cast<int>().Max() + 1, (int)flags.WarMECHMode);
			sum = AddBoolean(sum, flags.OrdealsPillars);
			sum = AddBoolean(sum, flags.SkyCastle4FTeleporters);
			sum = AddBoolean(sum, flags.TitansTrove);
			sum = AddBoolean(sum, flags.LefeinShops);
			sum = AddBoolean(sum, flags.ConfusedOldMen);
			sum = AddBoolean(sum, flags.MapOpenProgression);
			sum = AddBoolean(sum, flags.Entrances);
			sum = AddBoolean(sum, flags.Towns);
			sum = AddBoolean(sum, flags.Floors);
			sum = AddBoolean(sum, flags.AllowDeepCastles);
			sum = AddBoolean(sum, flags.MapOpenProgressionExtended);
			sum = AddBoolean(sum, flags.EntrancesIncludesDeadEnds);
			sum = AddBoolean(sum, flags.EntrancesMixedWithTowns);
			sum = AddBoolean(sum, flags.IncentivizeFreeNPCs);
			sum = AddBoolean(sum, flags.IncentivizeFetchNPCs);
			sum = AddBoolean(sum, flags.IncentivizeTail);
			sum = AddBoolean(sum, flags.IncentivizeMainItems);
			sum = AddBoolean(sum, flags.IncentivizeFetchItems);
			sum = AddBoolean(sum, flags.IncentivizeAirship);
			sum = AddBoolean(sum, flags.IncentivizeCanoeItem);
			sum = AddBoolean(sum, flags.IncentivizeShipAndCanal);
			sum = AddBoolean(sum, flags.ClassicItemPlacement);
			sum = AddBoolean(sum, flags.IncentivizeMarsh);
			sum = AddBoolean(sum, flags.IncentivizeEarth);
			sum = AddBoolean(sum, flags.IncentivizeVolcano);
			sum = AddBoolean(sum, flags.IncentivizeIceCave);
			sum = AddBoolean(sum, flags.IncentivizeOrdeals);
			sum = AddBoolean(sum, flags.IncentivizeSeaShrine);
			sum = AddBoolean(sum, flags.IncentivizeConeria);
			sum = AddBoolean(sum, flags.IncentivizeMarshKeyLocked);
			sum = AddBoolean(sum, flags.IncentivizeSkyPalace);
			sum = AddBoolean(sum, flags.IncentivizeTitansTrove);
			sum = AddBoolean(sum, flags.BetterTrapChests);
			sum = AddBoolean(sum, flags.IncentivizeMasamune);
			sum = AddBoolean(sum, flags.IncentivizeOpal);
			sum = AddBoolean(sum, flags.IncentivizeRibbon);
			sum = AddBoolean(sum, flags.IncentivizeRibbon2);
			sum = AddBoolean(sum, flags.Incentivize65K);
			sum = AddBoolean(sum, flags.IncentivizeBad);
			sum = AddBoolean(sum, flags.IncentivizeDefCastArmor);
			sum = AddBoolean(sum, flags.IncentivizeOffCastArmor);
			sum = AddBoolean(sum, flags.IncentivizeOtherCastArmor);
			sum = AddBoolean(sum, flags.IncentivizeDefCastWeapon);
			sum = AddBoolean(sum, flags.IncentivizeOffCastWeapon);
			sum = AddBoolean(sum, flags.IncentivizeOtherCastWeapon);
			sum = AddBoolean(sum, flags.EarlySarda);
			sum = AddBoolean(sum, flags.EarlySage);
			sum = AddBoolean(sum, flags.EarlyOrdeals);
			sum = AddBoolean(sum, flags.ShuffleObjectiveNPCs);
			sum = AddBoolean(sum, flags.OnlyRequireGameIsBeatable);
			sum = AddBoolean(sum, flags.FreeBridge);
			sum = AddBoolean(sum, flags.FreeAirship);
			sum = AddBoolean(sum, flags.FreeOrbs);
			sum = AddBoolean(sum, flags.EnableCritNumberDisplay);
			sum = AddBoolean(sum, flags.FreeCanal);
			sum = AddBoolean(sum, flags.EasyMode);
			sum = AddBoolean(sum, flags.HousesFillHp);
			sum = AddBoolean(sum, flags.SpeedHacks);
			sum = AddBoolean(sum, flags.NoPartyShuffle);
			sum = AddBoolean(sum, flags.Dash);
			sum = AddBoolean(sum, flags.BuyTen);
			sum = AddBoolean(sum, flags.IdentifyTreasures);
			sum = AddBoolean(sum, flags.WaitWhenUnrunnable);
			sum = AddBoolean(sum, flags.HouseMPRestoration);
			sum = AddBoolean(sum, flags.WeaponStats);
			sum = AddBoolean(sum, flags.ChanceToRun);
			sum = AddBoolean(sum, flags.SpellBugs);
			sum = AddBoolean(sum, flags.BlackBeltAbsorb);
			sum = AddBoolean(sum, flags.BlackBeltMDEF);
			sum = AddBoolean(sum, flags.NPCSwatter);
			sum = AddBoolean(sum, flags.EnemyStatusAttackBug);
			sum = AddBoolean(sum, flags.EnemySpellsTargetingAllies);
			sum = AddBoolean(sum, flags.EnemyElementalResistancesBug);
			sum = AddBoolean(sum, flags.ImproveTurnOrderRandomization);
			sum = AddBoolean(sum, flags.StartingGold);
			sum = AddBoolean(sum, flags.WrapStatOverflow);
			sum = AddBoolean(sum, flags.WrapPriceOverflow);
			sum = AddBoolean(sum, flags.IncludeMorale);
			sum = AddBoolean(sum, flags.RandomWaresIncludesSpecialGear);
			sum = AddBoolean(sum, flags.NoDanMode);
			sum = AddNumeric(sum, 41, (int)(10.0*flags.EnemyScaleFactor) - 10);
			sum = AddNumeric(sum, 41, (int)(10.0 * flags.BossScaleFactor) - 10);
			sum = AddNumeric(sum, 41, (int)(10.0 * flags.PriceScaleFactor) - 10);
			sum = AddNumeric(sum, 41, (int)(10.0 * flags.ExpMultiplier) - 10);
			sum = AddNumeric(sum, 51, (int)(flags.ExpBonus/10.0));
			sum = AddNumeric(sum, 46, (int)flags.EncounterRate);
			sum = AddNumeric(sum, 46, (int)flags.DungeonEncounterRate);
			sum = AddNumeric(sum, Enum.GetValues(typeof(ProgressiveScaleMode)).Cast<int>().Max() + 1, (int)flags.ProgressiveScaleMode);
			sum = AddBoolean(sum, flags.FIGHTER1);
			sum = AddBoolean(sum, flags.THIEF1);
			sum = AddBoolean(sum, flags.BLACK_BELT1);
			sum = AddBoolean(sum, flags.RED_MAGE1);
			sum = AddBoolean(sum, flags.WHITE_MAGE1);
			sum = AddBoolean(sum, flags.BLACK_MAGE1);
			sum = AddBoolean(sum, flags.FIGHTER2);
			sum = AddBoolean(sum, flags.THIEF2);
			sum = AddBoolean(sum, flags.BLACK_BELT2);
			sum = AddBoolean(sum, flags.RED_MAGE2);
			sum = AddBoolean(sum, flags.WHITE_MAGE2);
			sum = AddBoolean(sum, flags.BLACK_MAGE2);
			sum = AddBoolean(sum, flags.FIGHTER3);
			sum = AddBoolean(sum, flags.THIEF3);
			sum = AddBoolean(sum, flags.BLACK_BELT3);
			sum = AddBoolean(sum, flags.RED_MAGE3);
			sum = AddBoolean(sum, flags.WHITE_MAGE3);
			sum = AddBoolean(sum, flags.BLACK_MAGE3);
			sum = AddBoolean(sum, flags.FIGHTER4);
			sum = AddBoolean(sum, flags.THIEF4);
			sum = AddBoolean(sum, flags.BLACK_BELT4);
			sum = AddBoolean(sum, flags.RED_MAGE4);
			sum = AddBoolean(sum, flags.WHITE_MAGE4);
			sum = AddBoolean(sum, flags.BLACK_MAGE4);
			sum = AddBoolean(sum, flags.NONE_CLASS2);
			sum = AddBoolean(sum, flags.NONE_CLASS3);
			sum = AddBoolean(sum, flags.NONE_CLASS4);
			sum = AddBoolean(sum, flags.FORCED1);
			sum = AddBoolean(sum, flags.FORCED2);
			sum = AddBoolean(sum, flags.FORCED3);
			sum = AddBoolean(sum, flags.FORCED4);
			sum = AddBoolean(sum, flags.WeaponPermissions);
			sum = AddBoolean(sum, flags.ArmorPermissions);
			sum = AddBoolean(sum, flags.RecruitmentMode);
			sum = AddBoolean(sum, flags.ClampMinimumStatScale);
			sum = AddBoolean(sum, flags.ClampMinimumBossStatScale);
			sum = AddBoolean(sum, flags.ClampMinimumPriceScale);
			sum = AddBoolean(sum, flags.ExperimentalFloorGeneration);
			sum = AddBoolean(sum, flags.FiendShuffle);
			sum = AddNumeric(sum, Enum.GetValues(typeof(FormationShuffleModeEnum)).Cast<int>().Max() + 1, (int)flags.FormationShuffleMode);
			sum = AddNumeric(sum, Enum.GetValues(typeof(WorldWealth)).Cast<int>().Max() + 1, (int)flags.WorldWealth);
			sum = AddBoolean(sum, flags.AllowStartAreaDanager);
			sum = AddBoolean(sum, flags.Spoilers);

			return BigIntegerToString(sum);
		}

		public static Flags DecodeFlagsText(string text)
		{
			var sum = StringToBigInteger(text);

			var flags = new Flags
			{
				Spoilers = GetBoolean(ref sum),
				AllowStartAreaDanager = GetBoolean(ref sum),
				WorldWealth = (WorldWealth)GetNumeric(ref sum, Enum.GetValues(typeof(WorldWealth)).Cast<int>().Max() + 1),
				FormationShuffleMode = (FormationShuffleModeEnum)GetNumeric(ref sum, Enum.GetValues(typeof(FormationShuffleModeEnum)).Cast<int>().Max() + 1),
				FiendShuffle = GetBoolean(ref sum),
				ExperimentalFloorGeneration = GetBoolean(ref sum),
				ClampMinimumPriceScale = GetBoolean(ref sum),
				ClampMinimumBossStatScale = GetBoolean(ref sum),
				ClampMinimumStatScale = GetBoolean(ref sum),
				RecruitmentMode = GetBoolean(ref sum),
				ArmorPermissions = GetBoolean(ref sum),
				WeaponPermissions = GetBoolean(ref sum),
				FORCED4 = GetBoolean(ref sum),
				FORCED3 = GetBoolean(ref sum),
				FORCED2 = GetBoolean(ref sum),
				FORCED1 = GetBoolean(ref sum),
				NONE_CLASS4 = GetBoolean(ref sum),
				NONE_CLASS3 = GetBoolean(ref sum),
				NONE_CLASS2 = GetBoolean(ref sum),
				BLACK_MAGE4 = GetBoolean(ref sum),
				WHITE_MAGE4 = GetBoolean(ref sum),
				RED_MAGE4 = GetBoolean(ref sum),
				BLACK_BELT4 = GetBoolean(ref sum),
				THIEF4 = GetBoolean(ref sum),
				FIGHTER4 = GetBoolean(ref sum),
				BLACK_MAGE3 = GetBoolean(ref sum),
				WHITE_MAGE3 = GetBoolean(ref sum),
				RED_MAGE3 = GetBoolean(ref sum),
				BLACK_BELT3 = GetBoolean(ref sum),
				THIEF3 = GetBoolean(ref sum),
				FIGHTER3 = GetBoolean(ref sum),
				BLACK_MAGE2 = GetBoolean(ref sum),
				WHITE_MAGE2 = GetBoolean(ref sum),
				RED_MAGE2 = GetBoolean(ref sum),
				BLACK_BELT2 = GetBoolean(ref sum),
				THIEF2 = GetBoolean(ref sum),
				FIGHTER2 = GetBoolean(ref sum),
				BLACK_MAGE1 = GetBoolean(ref sum),
				WHITE_MAGE1 = GetBoolean(ref sum),
				RED_MAGE1 = GetBoolean(ref sum),
				BLACK_BELT1 = GetBoolean(ref sum),
				THIEF1 = GetBoolean(ref sum),
				FIGHTER1 = GetBoolean(ref sum),
				ProgressiveScaleMode = (ProgressiveScaleMode)GetNumeric(ref sum, Enum.GetValues(typeof(ProgressiveScaleMode)).Cast<int>().Max() + 1),
				DungeonEncounterRate = GetNumeric(ref sum, 46),
				EncounterRate = GetNumeric(ref sum, 46),
				ExpBonus = GetNumeric(ref sum, 51) * 10,
				ExpMultiplier = (GetNumeric(ref sum, 41) + 10) / 10.0,
				PriceScaleFactor = (GetNumeric(ref sum, 41) + 10) / 10.0,
				BossScaleFactor = (GetNumeric(ref sum, 41) + 10) / 10.0,
				EnemyScaleFactor = (GetNumeric(ref sum, 41) + 10) / 10.0,
				NoDanMode = GetBoolean(ref sum),
				RandomWaresIncludesSpecialGear = GetBoolean(ref sum),
				IncludeMorale = GetBoolean(ref sum),
				WrapPriceOverflow = GetBoolean(ref sum),
				WrapStatOverflow = GetBoolean(ref sum),
				StartingGold = GetBoolean(ref sum),
				ImproveTurnOrderRandomization = GetBoolean(ref sum),
				EnemyElementalResistancesBug = GetBoolean(ref sum),
				EnemySpellsTargetingAllies = GetBoolean(ref sum),
				EnemyStatusAttackBug = GetBoolean(ref sum),
				NPCSwatter = GetBoolean(ref sum),
				BlackBeltMDEF = GetBoolean(ref sum),
				BlackBeltAbsorb = GetBoolean(ref sum),
				SpellBugs = GetBoolean(ref sum),
				ChanceToRun = GetBoolean(ref sum),
				WeaponStats = GetBoolean(ref sum),
				HouseMPRestoration = GetBoolean(ref sum),
				WaitWhenUnrunnable = GetBoolean(ref sum),
				IdentifyTreasures = GetBoolean(ref sum),
				BuyTen = GetBoolean(ref sum),
				Dash = GetBoolean(ref sum),
				NoPartyShuffle = GetBoolean(ref sum),
				SpeedHacks = GetBoolean(ref sum),
				HousesFillHp = GetBoolean(ref sum),
				EasyMode = GetBoolean(ref sum),
				FreeCanal = GetBoolean(ref sum),
				EnableCritNumberDisplay = GetBoolean(ref sum),
				FreeOrbs = GetBoolean(ref sum),
				FreeAirship = GetBoolean(ref sum),
				FreeBridge = GetBoolean(ref sum),
				OnlyRequireGameIsBeatable = GetBoolean(ref sum),
				ShuffleObjectiveNPCs = GetBoolean(ref sum),
				EarlyOrdeals = GetBoolean(ref sum),
				EarlySage = GetBoolean(ref sum),
				EarlySarda = GetBoolean(ref sum),
				IncentivizeOtherCastWeapon = GetBoolean(ref sum),
				IncentivizeOffCastWeapon = GetBoolean(ref sum),
				IncentivizeDefCastWeapon = GetBoolean(ref sum),
				IncentivizeOtherCastArmor = GetBoolean(ref sum),
				IncentivizeOffCastArmor = GetBoolean(ref sum),
				IncentivizeDefCastArmor = GetBoolean(ref sum),
				IncentivizeBad = GetBoolean(ref sum),
				Incentivize65K = GetBoolean(ref sum),
				IncentivizeRibbon2 = GetBoolean(ref sum),
				IncentivizeRibbon = GetBoolean(ref sum),
				IncentivizeOpal = GetBoolean(ref sum),
				IncentivizeMasamune = GetBoolean(ref sum),
				BetterTrapChests = GetBoolean(ref sum),
				IncentivizeTitansTrove = GetBoolean(ref sum),
				IncentivizeSkyPalace = GetBoolean(ref sum),
				IncentivizeMarshKeyLocked = GetBoolean(ref sum),
				IncentivizeConeria = GetBoolean(ref sum),
				IncentivizeSeaShrine = GetBoolean(ref sum),
				IncentivizeOrdeals = GetBoolean(ref sum),
				IncentivizeIceCave = GetBoolean(ref sum),
				IncentivizeVolcano = GetBoolean(ref sum),
				IncentivizeEarth = GetBoolean(ref sum),
				IncentivizeMarsh = GetBoolean(ref sum),
				ClassicItemPlacement = GetBoolean(ref sum),
				IncentivizeShipAndCanal = GetBoolean(ref sum),
				IncentivizeCanoeItem = GetBoolean(ref sum),
				IncentivizeAirship = GetBoolean(ref sum),
				IncentivizeFetchItems = GetBoolean(ref sum),
				IncentivizeMainItems = GetBoolean(ref sum),
				IncentivizeTail = GetBoolean(ref sum),
				IncentivizeFetchNPCs = GetBoolean(ref sum),
				IncentivizeFreeNPCs = GetBoolean(ref sum),
				EntrancesMixedWithTowns = GetBoolean(ref sum),
				EntrancesIncludesDeadEnds = GetBoolean(ref sum),
				MapOpenProgressionExtended = GetBoolean(ref sum),
				AllowDeepCastles = GetBoolean(ref sum),
				Floors = GetBoolean(ref sum),
				Towns = GetBoolean(ref sum),
				Entrances = GetBoolean(ref sum),
				MapOpenProgression = GetBoolean(ref sum),
				ConfusedOldMen = GetBoolean(ref sum),
				LefeinShops = GetBoolean(ref sum),
				TitansTrove = GetBoolean(ref sum),
				SkyCastle4FTeleporters = GetBoolean(ref sum),
				OrdealsPillars = GetBoolean(ref sum),
				WarMECHMode = (WarMECHMode)GetNumeric(ref sum, Enum.GetValues(typeof(WarMECHMode)).Cast<int>().Max() + 1),
				AllowUnsafeMelmond = GetBoolean(ref sum),
				AllowUnsafePirates = GetBoolean(ref sum),
				EnemyStatusAttacks = GetBoolean(ref sum),
				EnemySkillsSpells = GetBoolean(ref sum),
				EnemyScripts = GetBoolean(ref sum),
				RandomTrapFormations = GetBoolean(ref sum),
				EnemyTrapTiles = GetBoolean(ref sum),
				EnemyFormationsSurprise = GetBoolean(ref sum),
				EnemyFormationsUnrunnable = GetBoolean(ref sum),
				EverythingUnrunnable = GetBoolean(ref sum),
				Rng = GetBoolean(ref sum),
				RebalanceSpells = GetBoolean(ref sum),
				ItemMagic = GetBoolean(ref sum),
				MagicPermissions = GetBoolean(ref sum),
				MagicLevels = GetBoolean(ref sum),
				MagicShops = GetBoolean(ref sum),
				PreserveFiendRefights = GetBoolean(ref sum),
				ShortToFR = GetBoolean(ref sum),
				ChaosRush = GetBoolean(ref sum),
				TransformFinalFormation = GetBoolean(ref sum),
				ExtraShards = GetBoolean(ref sum),
				ShardHunt = GetBoolean(ref sum),
				RandomLoot = GetBoolean(ref sum),
				RandomWares = GetBoolean(ref sum),
				NPCFetchItems = GetBoolean(ref sum),
				NPCItems = GetBoolean(ref sum),
				Treasures = GetBoolean(ref sum),
				Shops = GetBoolean(ref sum)
			};

			return flags;
		}

		private static BigInteger AddNumeric(BigInteger sum, int radix, int value) => sum * radix + value;
		private static BigInteger AddBoolean(BigInteger sum, bool value) => AddNumeric(sum, 2, value ? 1 : 0);
		private static int TriStateValue(bool? value) => value.HasValue ? (value.Value ? 1 : 0) : 2;
		private static BigInteger AddTriState(BigInteger sum, bool? value) => AddNumeric(sum, 3, TriStateValue(value));

		private static int GetNumeric(ref BigInteger sum, int radix)
		{
			sum = BigInteger.DivRem(sum, radix, out var value);

			return (int)value;
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
