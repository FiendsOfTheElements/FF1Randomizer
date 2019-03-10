using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace FF1Lib
{
	public class Flags : IIncentiveFlags, IMapEditFlags, IScaleFlags, IFloorShuffleFlags
	{
		// Character Groupings
		private const int ITEMS = 0;
		private const int ALT_GAME_MODE = 1;
		private const int MAGIC = 2;
		private const int ENCOUNTERS = 3;
		private const int BATTLES = 4;
		private const int STANDARD_MAPS = 5;
		private const int OVERWORLD_MAP = 6;
		private const int INCENTIVES_MAIN = 7;
		private const int INCENTIVES_CHESTS1 = 8;
		private const int INCENTIVES_CHESTS2 = 9;
		private const int INCENTIVES_ITEMS1 = 10;
		private const int INCENTIVES_ITEMS2 = 11;
		private const int ITEM_REQUIREMENTS = 12;
		private const int FILTHY_CASUALS = 13;
		private const int CONVENIENCES = 14;
		private const int BUG_FIXES = 15;
		private const int ENEMY_BUG_FIXES = 16;
		private const int SCALE = 17;
		private const int ENEMY_SCALE_FACTOR = 18;
		private const int PRICE_SCALE_FACTOR = 19;
		private const int EXP_MULTIPLIER = 20;
		private const int EXP_BONUS = 21;
		private const int ENCOUNTER_RATE = 22;
		private const int PARTY_1 = 23;
		private const int PROGRESSIVE_SCALE = 24;
		private const int DUNGEON_ENCOUNTER_RATE = 25;
		private const int BOSS_SCALE_FACTOR = 26;
		private const int PARTY_2 = 27;
		private const int PARTY_3 = 28;
		private const int PARTY_4 = 29;
		private const int PARTY_EXTRA = 30;
		private const int PARTY_EXTRA2 = 31;
		private const int SCALE_CLAMPING = 32;
		private const int PROCEDURAL_GENERATION = 33;
		private const int FORMATION = 34;
		private const int WEALTH = 35;


		[FlagString(Character = ITEMS, FlagBit = 1)]
		public bool Shops { get; set; }
		[FlagString(Character = ITEMS, FlagBit = 2)]
		public bool Treasures { get; set; }
		[FlagString(Character = ITEMS, FlagBit = 4)]
		public bool NPCItems { get; set; }
		[FlagString(Character = ITEMS, FlagBit = 8)]
		public bool NPCFetchItems { get; set; }
		[FlagString(Character = ITEMS, FlagBit = 16)]
		public bool RandomWares { get; set; }
		[FlagString(Character = ITEMS, FlagBit = 32)]
		public bool RandomLoot { get; set; }

		[FlagString(Character = ALT_GAME_MODE, FlagBit = 1)]
		public bool ShardHunt { get; set; }
		[FlagString(Character = ALT_GAME_MODE, FlagBit = 2)]
		public bool ExtraShards { get; set; }
		[FlagString(Character = ALT_GAME_MODE, FlagBit = 4)]
		public bool TransformFinalFormation { get; set; }
		[FlagString(Character = ALT_GAME_MODE, FlagBit = 8)]
		public bool ChaosRush { get; set; }
		[FlagString(Character = ALT_GAME_MODE, FlagBit = 16)]
		public bool ShortToFR { get; set; }
		[FlagString(Character = ALT_GAME_MODE, FlagBit = 32)]
		public bool PreserveFiendRefights { get; set; }

		[FlagString(Character = MAGIC, FlagBit = 1)]
		public bool MagicShops { get; set; }
		[FlagString(Character = MAGIC, FlagBit = 2)]
		public bool MagicLevels { get; set; }
		[FlagString(Character = MAGIC, FlagBit = 4)]
		public bool MagicPermissions { get; set; }
		[FlagString(Character = MAGIC, FlagBit = 8)]
		public bool ItemMagic { get; set; }

		[FlagString(Character = ENCOUNTERS, FlagBit = 1)]
		public bool Rng { get; set; }
		[FlagString(Character = ENCOUNTERS, FlagBit = 4)]
		public bool EnemyFormationsUnrunnable { get; set; }
		[FlagString(Character = ENCOUNTERS, FlagBit = 8)]
		public bool EnemyFormationsSurprise { get; set; }
		[FlagString(Character = ENCOUNTERS, FlagBit = 16)]
		public bool EnemyTrapTiles { get; set; }
		[FlagString(Character = ENCOUNTERS, FlagBit = 32)]
		public bool RandomTrapFormations { get; set; }

		[FlagString(Character = BATTLES, FlagBit = 1)]
		public bool EnemyScripts { get; set; }
		[FlagString(Character = BATTLES, FlagBit = 2)]
		public bool EnemySkillsSpells { get; set; }
		[FlagString(Character = BATTLES, FlagBit = 4)]
		public bool EnemyStatusAttacks { get; set; }
		[FlagString(Character = BATTLES, FlagBit = 8)]
		public bool AllowUnsafePirates { get; set; }
		[FlagString(Character = BATTLES, FlagBit = 16)]
		public bool AllowUnsafeMelmond { get; set; }

		[FlagString(Character = STANDARD_MAPS, FlagBit = 3)]
		public WarMECHMode WarMECHMode { get; set; }
		[FlagString(Character = STANDARD_MAPS, FlagBit = 4)]
		public bool OrdealsPillars { get; set; }
		[FlagString(Character = STANDARD_MAPS, FlagBit = 8)]
		public bool SkyCastle4FTeleporters { get; set; }
		[FlagString(Character = STANDARD_MAPS, FlagBit = 16)]
		public bool TitansTrove { get; set; }
		[FlagString(Character = STANDARD_MAPS, FlagBit = 32)]
		public bool ConfusedOldMen { get; set; }

		[FlagString(Character = OVERWORLD_MAP, FlagBit = 1)]
		public bool MapOpenProgression { get; set; }
		[FlagString(Character = OVERWORLD_MAP, FlagBit = 2)]
		public bool Entrances { get; set; }
		[FlagString(Character = OVERWORLD_MAP, FlagBit = 4)]
		public bool Towns { get; set; }
		[FlagString(Character = OVERWORLD_MAP, FlagBit = 8)]
		public bool Floors { get; set; }
		[FlagString(Character = OVERWORLD_MAP, FlagBit = 16)]
		public bool AllowDeepCastles { get; set; }
		[FlagString(Character = OVERWORLD_MAP, FlagBit = 32)]
		public bool MapOpenProgressionExtended { get; set; }

		[FlagString(Character = INCENTIVES_MAIN, FlagBit = 1)]
		public bool IncentivizeFreeNPCs { get; set; }
		[FlagString(Character = INCENTIVES_MAIN, FlagBit = 2)]
		public bool IncentivizeFetchNPCs { get; set; }
		[FlagString(Character = INCENTIVES_MAIN, FlagBit = 4)]
		public bool IncentivizeTail { get; set; }
		[FlagString(Character = INCENTIVES_MAIN, FlagBit = 8)]
		public bool IncentivizeFetchItems { get; set; }

		[FlagString(Character = INCENTIVES_CHESTS1, FlagBit = 1)]
		public bool IncentivizeMarsh { get; set; }
		[FlagString(Character = INCENTIVES_CHESTS1, FlagBit = 2)]
		public bool IncentivizeEarth { get; set; }
		[FlagString(Character = INCENTIVES_CHESTS1, FlagBit = 4)]
		public bool IncentivizeVolcano { get; set; }
		[FlagString(Character = INCENTIVES_CHESTS1, FlagBit = 8)]
		public bool IncentivizeIceCave { get; set; }
		[FlagString(Character = INCENTIVES_CHESTS1, FlagBit = 16)]
		public bool IncentivizeOrdeals { get; set; }
		[FlagString(Character = INCENTIVES_CHESTS1, FlagBit = 32)]
		public bool IncentivizeSeaShrine { get; set; }

		[FlagString(Character = INCENTIVES_CHESTS2, FlagBit = 1)]
		public bool IncentivizeConeria { get; set; }
		[FlagString(Character = INCENTIVES_CHESTS2, FlagBit = 2)]
		public bool IncentivizeMarshKeyLocked { get; set; }
		[FlagString(Character = INCENTIVES_CHESTS2, FlagBit = 4)]
		public bool IncentivizeSkyPalace { get; set; }

		[FlagString(Character = INCENTIVES_ITEMS1, FlagBit = 1)]
		public bool IncentivizeMasamune { get; set; }
		[FlagString(Character = INCENTIVES_ITEMS1, FlagBit = 2)]
		public bool IncentivizeOpal { get; set; }
		[FlagString(Character = INCENTIVES_ITEMS1, FlagBit = 4)]
		public bool IncentivizeRibbon { get; set; }
		[FlagString(Character = INCENTIVES_ITEMS1, FlagBit = 8)]
		public bool IncentivizeRibbon2 { get; set; }
		[FlagString(Character = INCENTIVES_ITEMS1, FlagBit = 16)]
		public bool Incentivize65K { get; set; }
		[FlagString(Character = INCENTIVES_ITEMS1, FlagBit = 32)]
		public bool IncentivizeBad { get; set; }

		[FlagString(Character = INCENTIVES_ITEMS2, FlagBit = 1)]
		public bool IncentivizeDefCastArmor { get; set; }
		[FlagString(Character = INCENTIVES_ITEMS2, FlagBit = 2)]
		public bool IncentivizeOffCastArmor { get; set; }
		[FlagString(Character = INCENTIVES_ITEMS2, FlagBit = 4)]
		public bool IncentivizeOtherCastArmor { get; set; }
		[FlagString(Character = INCENTIVES_ITEMS2, FlagBit = 8)]
		public bool IncentivizeDefCastWeapon { get; set; }
		[FlagString(Character = INCENTIVES_ITEMS2, FlagBit = 16)]
		public bool IncentivizeOffCastWeapon { get; set; }
		[FlagString(Character = INCENTIVES_ITEMS2, FlagBit = 32)]
		public bool IncentivizeOtherCastWeapon { get; set; }

		[FlagString(Character = ITEM_REQUIREMENTS, FlagBit = 1)]
		public bool EarlySarda { get; set; }
		[FlagString(Character = ITEM_REQUIREMENTS, FlagBit = 2)]
		public bool EarlySage { get; set; }
		[FlagString(Character = ITEM_REQUIREMENTS, FlagBit = 4)]
		public bool CrownlessOrdeals { get; set; }
		[FlagString(Character = ITEM_REQUIREMENTS, FlagBit = 8)]
		public bool ShuffleObjectiveNPCs { get; set; }
		[FlagString(Character = ITEM_REQUIREMENTS, FlagBit = 32)]
		public bool OnlyRequireGameIsBeatable { get; set; }

		[FlagString(Character = FILTHY_CASUALS, FlagBit = 1)]
		public bool FreeBridge { get; set; }
		[FlagString(Character = FILTHY_CASUALS, FlagBit = 2)]
		public bool FreeAirship { get; set; }
		[FlagString(Character = FILTHY_CASUALS, FlagBit = 4)]
		public bool FreeOrbs { get; set; }
		[FlagString(Character = FILTHY_CASUALS, FlagBit = 8)]
		public bool EnableCritNumberDisplay { get; set; }
		[FlagString(Character = FILTHY_CASUALS, FlagBit = 16)]
		public bool FreeCanal { get; set; }
		[FlagString(Character = FILTHY_CASUALS, FlagBit = 32)]
		public bool EasyMode { get; set; }

		[FlagString(Character = CONVENIENCES, FlagBit = 1)]
		public bool SpeedHacks { get; set; }
		[FlagString(Character = CONVENIENCES, FlagBit = 2)]
		public bool NoPartyShuffle { get; set; }
		[FlagString(Character = CONVENIENCES, FlagBit = 4)]
		public bool Dash { get; set; }
		[FlagString(Character = CONVENIENCES, FlagBit = 8)]
		public bool BuyTen { get; set; }
		[FlagString(Character = CONVENIENCES, FlagBit = 16)]
		public bool IdentifyTreasures { get; set; }
		[FlagString(Character = CONVENIENCES, FlagBit = 32)]
		public bool WaitWhenUnrunnable { get; set; }

		[FlagString(Character = BUG_FIXES, FlagBit = 1)]
		public bool HouseMPRestoration { get; set; }
		[FlagString(Character = BUG_FIXES, FlagBit = 2)]
		public bool WeaponStats { get; set; }
		[FlagString(Character = BUG_FIXES, FlagBit = 4)]
		public bool ChanceToRun { get; set; }
		[FlagString(Character = BUG_FIXES, FlagBit = 8)]
		public bool SpellBugs { get; set; }
		[FlagString(Character = BUG_FIXES, FlagBit = 16)]
		public bool BlackBeltAbsorb { get; set; }

		[FlagString(Character = ENEMY_BUG_FIXES, FlagBit = 1)]
		public bool EnemyStatusAttackBug { get; set; }
		[FlagString(Character = ENEMY_BUG_FIXES, FlagBit = 2)]
		public bool EnemySpellsTargetingAllies { get; set; }
		[FlagString(Character = ENEMY_BUG_FIXES, FlagBit = 4)]
		public bool EnemyElementalResistancesBug { get; set; }
		[FlagString(Character = ENEMY_BUG_FIXES, FlagBit = 8)]
		public bool ImproveTurnOrderRandomization { get; set; }

		[FlagString(Character = SCALE, FlagBit = 1)]
		public bool StartingGold { get; set; }
		[FlagString(Character = SCALE, FlagBit = 2)]
		public bool WrapStatOverflow { get; set; }
		[FlagString(Character = SCALE, FlagBit = 4)]
		public bool WrapPriceOverflow { get; set; }
		[FlagString(Character = SCALE, FlagBit = 8)]
		public bool IncludeMorale { get; set; }
		[FlagString(Character = SCALE, FlagBit = 16)]
		public bool RandomWaresIncludesSpecialGear { get; set; }
		[FlagString(Character = SCALE, FlagBit = 32)]
		public bool NoDanMode { get; set; }

		[FlagString(Character = ENEMY_SCALE_FACTOR, Multiplier = 0.1)]
		public double EnemyScaleFactor { get; set; }
		[FlagString(Character = BOSS_SCALE_FACTOR, Multiplier = 0.1)]
		public double BossScaleFactor { get; set; }
		[FlagString(Character = PRICE_SCALE_FACTOR, Multiplier = 0.1)]
		public double PriceScaleFactor { get; set; }
		[FlagString(Character = EXP_MULTIPLIER, Multiplier = 0.1)]
		public double ExpMultiplier { get; set; }
		[FlagString(Character = EXP_BONUS, Multiplier = 10)]
		public int ExpBonus { get; set; }
		[FlagString(Character = ENCOUNTER_RATE, Multiplier = 1)]
		public double EncounterRate { get; set; }
		[FlagString(Character = DUNGEON_ENCOUNTER_RATE, Multiplier = 1)]
		public double DungeonEncounterRate { get; set; }
		[FlagString(Character = PROGRESSIVE_SCALE, FlagBit = 7)]
		public ProgressiveScaleMode ProgressiveScaleMode { get; set; }

		[FlagString(Character = PARTY_1, FlagBit = 1)]
		public bool FIGHTER1 { get; set; }
		[FlagString(Character = PARTY_1, FlagBit = 2)]
		public bool THIEF1 { get; set; }
		[FlagString(Character = PARTY_1, FlagBit = 4)]
		public bool BLACK_BELT1 { get; set; }
		[FlagString(Character = PARTY_1, FlagBit = 8)]
		public bool RED_MAGE1 { get; set; }
		[FlagString(Character = PARTY_1, FlagBit = 16)]
		public bool WHITE_MAGE1 { get; set; }
		[FlagString(Character = PARTY_1, FlagBit = 32)]
		public bool BLACK_MAGE1 { get; set; }

		[FlagString(Character = PARTY_2, FlagBit = 1)]
		public bool FIGHTER2 { get; set; }
		[FlagString(Character = PARTY_2, FlagBit = 2)]
		public bool THIEF2 { get; set; }
		[FlagString(Character = PARTY_2, FlagBit = 4)]
		public bool BLACK_BELT2 { get; set; }
		[FlagString(Character = PARTY_2, FlagBit = 8)]
		public bool RED_MAGE2 { get; set; }
		[FlagString(Character = PARTY_2, FlagBit = 16)]
		public bool WHITE_MAGE2 { get; set; }
		[FlagString(Character = PARTY_2, FlagBit = 32)]
		public bool BLACK_MAGE2 { get; set; }

		[FlagString(Character = PARTY_3, FlagBit = 1)]
		public bool FIGHTER3 { get; set; }
		[FlagString(Character = PARTY_3, FlagBit = 2)]
		public bool THIEF3 { get; set; }
		[FlagString(Character = PARTY_3, FlagBit = 4)]
		public bool BLACK_BELT3 { get; set; }
		[FlagString(Character = PARTY_3, FlagBit = 8)]
		public bool RED_MAGE3 { get; set; }
		[FlagString(Character = PARTY_3, FlagBit = 16)]
		public bool WHITE_MAGE3 { get; set; }
		[FlagString(Character = PARTY_3, FlagBit = 32)]
		public bool BLACK_MAGE3 { get; set; }

		[FlagString(Character = PARTY_4, FlagBit = 1)]
		public bool FIGHTER4 { get; set; }
		[FlagString(Character = PARTY_4, FlagBit = 2)]
		public bool THIEF4 { get; set; }
		[FlagString(Character = PARTY_4, FlagBit = 4)]
		public bool BLACK_BELT4 { get; set; }
		[FlagString(Character = PARTY_4, FlagBit = 8)]
		public bool RED_MAGE4 { get; set; }
		[FlagString(Character = PARTY_4, FlagBit = 16)]
		public bool WHITE_MAGE4 { get; set; }
		[FlagString(Character = PARTY_4, FlagBit = 32)]
		public bool BLACK_MAGE4 { get; set; }

		[FlagString(Character = PARTY_EXTRA, FlagBit = 1)]
		public bool NONE_CLASS2 { get; set; }
		[FlagString(Character = PARTY_EXTRA, FlagBit = 2)]
		public bool NONE_CLASS3 { get; set; }
		[FlagString(Character = PARTY_EXTRA, FlagBit = 4)]
		public bool NONE_CLASS4 { get; set; }
		[FlagString(Character = PARTY_EXTRA, FlagBit = 8)]
		public bool FORCED1 { get; set; }
		[FlagString(Character = PARTY_EXTRA, FlagBit = 16)]
		public bool FORCED2 { get; set; }
		[FlagString(Character = PARTY_EXTRA, FlagBit = 32)]
		public bool FORCED3 { get; set; }

		[FlagString(Character = PARTY_EXTRA2, FlagBit = 1)]
		public bool FORCED4 { get; set; }
		[FlagString(Character = PARTY_EXTRA2, FlagBit = 2)]
		public bool WeaponPermissions { get; set; }
		[FlagString(Character = PARTY_EXTRA2, FlagBit = 4)]
		public bool ArmorPermissions { get; set; }

		[FlagString(Character = SCALE_CLAMPING, FlagBit = 1)]
		public bool ClampMinimumStatScale { get; set; }
		[FlagString(Character = SCALE_CLAMPING, FlagBit = 2)]
		public bool ClampMinimumBossStatScale { get; set; }
		[FlagString(Character = SCALE_CLAMPING, FlagBit = 4)]
		public bool ClampMinimumPriceScale { get; set; }

		[FlagString(Character = PROCEDURAL_GENERATION, FlagBit = 1)]
		public bool ExperimentalFloorGeneration { get; set; }

		[FlagString(Character = FORMATION, FlagBit = 3)]
		public FormationShuffleModeEnum FormationShuffleMode { get; set; }

		[FlagString(Character = WEALTH, FlagBit = 7)]
		public WorldWealth WorldWealth { get; set; }

		public bool ModernBattlefield { get; set; }
		public bool ThirdBattlePalette { get; set; }
		public bool DisableDamageTileFlicker { get; set; }
		public bool FunEnemyNames { get; set; }
		public bool PaletteSwap { get; set; }
		public bool TeamSteak { get; set; }
		public MusicShuffle Music { get; set; }

		public bool AllowStartAreaDanager { get; set; } = false;

		public bool MapCanalBridge => NPCItems || NPCFetchItems;
		public bool MapOnracDock => MapOpenProgression;
		public bool MapMirageDock => MapOpenProgression;
		public bool MapConeriaDwarves => MapOpenProgression;
		public bool MapVolcanoIceRiver => MapOpenProgression;
		public bool MapDwarvesNorthwest => MapOpenProgressionExtended;
		public bool MapAirshipDock => MapOpenProgressionExtended;

		public bool IncentivizeAdamant => IncentivizeFetchItems;
		public bool IncentivizeRuby => (!EarlySage && !NPCItems) || IncentivizeFetchItems;
		public bool IncentivizeCrown => !NPCFetchItems || IncentivizeFetchItems;
		public bool IncentivizeTnt => (!NPCFetchItems && !NPCItems) || IncentivizeFetchItems; // If Canoe and Fetch Quests are unshuffled then TNT is required
		public bool IncentivizeSlab => !NPCFetchItems || IncentivizeFetchItems;
		public bool IncentivizeBottle => !NPCFetchItems || IncentivizeFetchItems;

		public bool IncentivizeFloater => !FreeAirship;
		public bool IncentivizeBridge => false;
		public bool IncentivizeLute => !ShortToFR;
		public bool IncentivizeShip => !MapOpenProgression || IncentivizeFetchItems;
		public bool IncentivizeRod => true;
		public bool IncentivizeCanoe => true;
		public bool IncentivizeCube => true;

		public bool IncentivizeCrystal => IncentivizeFetchItems;
		public bool IncentivizeHerb => IncentivizeFetchItems;
		public bool IncentivizeKey => true;
		public bool IncentivizeCanal => !FreeCanal && (!NPCItems || IncentivizeFetchItems); // If Canoe is unshuffled then Canal is Required
		public bool IncentivizeChime => true;
		public bool IncentivizeOxyale => true;
		public bool IncentivizeXcalber => false;

		public bool IncentivizeKingConeria => IncentivizeFreeNPCs;
		public bool IncentivizePrincess => IncentivizeFreeNPCs;
		public bool IncentivizeBikke => IncentivizeFreeNPCs;
		public bool IncentivizeSarda => IncentivizeFreeNPCs;
		public bool IncentivizeCanoeSage => IncentivizeFreeNPCs;
		public bool IncentivizeCaravan => IncentivizeFreeNPCs;
		public bool IncentivizeCubeBot => IncentivizeFreeNPCs;

		public bool IncentivizeFairy => IncentivizeFetchNPCs;
		public bool IncentivizeAstos => IncentivizeFetchNPCs;
		public bool IncentivizeMatoya => IncentivizeFetchNPCs;
		public bool IncentivizeElfPrince => IncentivizeFetchNPCs;
		public bool IncentivizeNerrick => IncentivizeFetchNPCs;
		public bool IncentivizeLefein => IncentivizeFetchNPCs;
		public bool IncentivizeSmith => IncentivizeFetchNPCs;

		public bool ImmediatePureAndSoftRequired => EnemyStatusAttacks || Entrances || MapOpenProgression;

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

		// ! and % are printable in FF, + and / are not.
		private const string base64CharString = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789-!";
		public static string EncodeFlagsText(Flags flags)
		{
			var base64Chars = base64CharString.ToCharArray();
			var flagAttributes = GetFlagStringAttributes().GroupBy(x => x.Value.Character).OrderBy(x => x.Key);
			var result = "";

			foreach (var flagCharacterPropertyGroup in flagAttributes)
			{
				var flagCharacterIndex = 0;
				foreach (var flagProperty in flagCharacterPropertyGroup)
				{
					var flagsPropertyValue = typeof(Flags).GetProperty(flagProperty.Key).GetValue(flags, null);
					if (flagProperty.Value.FlagBit > 0)
					{
						if ((flagsPropertyValue as bool?).GetValueOrDefault())
							flagCharacterIndex += flagProperty.Value.FlagBit;
						else if (flagsPropertyValue is Enum)
							flagCharacterIndex += (int)flagsPropertyValue;
						continue;
					}
					flagCharacterIndex += Convert.ToInt32((Convert.ToDouble(flagsPropertyValue) / flagProperty.Value.Multiplier));
				}
				flagCharacterIndex = flagCharacterIndex % 64;
				result += base64Chars[flagCharacterIndex];
			}
			return result;
		}

		public static Flags DecodeFlagsText(string text)
		{
			var inputCharacters = text.ToCharArray();
			var flagAttributes = GetFlagStringAttributes().GroupBy(x => x.Value.Character).ToDictionary(x => x.Key, x => x.ToList());
			var result = new Flags();
			var index = 0;
			foreach (var inputChar in inputCharacters)
			{
				var charFlagValue = base64CharString.IndexOf(inputChar);
				var flagAttributesForChar = flagAttributes[index++];
				if (flagAttributesForChar.Any(x => x.Value.FlagBit < 1))
				{
					var multiplierAttribute = flagAttributesForChar.First(x => x.Value.FlagBit < 1);

					var outputValue = charFlagValue * multiplierAttribute.Value.Multiplier;
					var property = typeof(Flags).GetProperty(multiplierAttribute.Key);
					property.SetValue(result, Convert.ChangeType(outputValue, property.PropertyType));
					continue;
				}
				foreach (var flagAttribute in flagAttributesForChar)
				{
					var property = typeof(Flags).GetProperty(flagAttribute.Key);
					var outputValue = charFlagValue & flagAttribute.Value.FlagBit;
					if (property.PropertyType == typeof(bool))
					{
						property.SetValue(result, outputValue > 0);
					}
					else
					{
						property.SetValue(result, outputValue);
					}
				}
			}
			return result;
		}

		public class Preset
		{
			public string Name { get; set; }
			public Flags Flags { get; set; }
		}

		public static Flags FromJson(string json) => JsonConvert.DeserializeObject<Preset>(json).Flags;
	}

	public class FlagStringAttribute : Attribute
	{
		public int Character { get; set; }
		public int FlagBit { get; set; }
		public double Multiplier { get; set; }

		public string ToVueComputedPropertyString()
		{
			if (FlagBit == 1 || FlagBit == 2 || FlagBit == 4 || FlagBit == 8 || FlagBit == 16 || FlagBit == 32)
			{
				return
					$"{{\n" +
					$"  get: function() {{\n" +
					$"    if(this.flagString.length <= {Character}) return false;\n" +
					$"    return (base64Chars.indexOf(this.flagString.charAt({Character})) & {FlagBit}) > 0;\n" +
					$"  }},\n" +
					$"  set: function() {{\n" +
					$"    while(this.flagString.length <= {Character})this.flagString += base64Chars[0];\n" +
					$"    var toggled = (base64Chars.indexOf(this.flagString.charAt({Character}))) ^ {FlagBit};\n" +
					$"    this.flagString = this.flagString.substr(0,{Character}) + base64Chars[toggled] + this.flagString.substr({Character + 1});\n" +
					$"  }}\n" +
					$"}}\n";
			}
			else if (FlagBit > 0)
			{
				return
					$"{{\n" +
					$"  get: function () {{\n" +
					$"    if (this.flagString.length <= {Character}) return 0;\n" +
					$"    return base64Chars.indexOf(this.flagString[{Character}]) & {FlagBit};\n" +
					$"  }},\n" +
					$"  set: function(newValue) {{\n" +
					$"    while(this.flagString.length <= {Character})this.flagString += base64Chars[0];\n" +
					$"    var combinedValue = base64Chars.indexOf(this.flagString.charAt({Character})) & {63 - FlagBit} | newValue;\n" +
					$"    this.flagString = this.flagString.substr(0,{Character}) + base64Chars[combinedValue] + this.flagString.substr({Character + 1});\n" +
					$"  }}\n" +
					$"}}\n";
			}
			else
			{
				return
					$"{{\n" +
					$"  get: function () {{\n" +
					$"    if (this.flagString.length <= {Character}) return 0;\n" +
					$"    return base64Chars.indexOf(this.flagString[{Character}]) * {Multiplier};\n" +
					$"  }},\n" +
					$"  set: function(newValue) {{\n" +
					$"    while(this.flagString.length <= {Character})this.flagString += base64Chars[0];\n" +
					$"    var scaledValue = (newValue / {Multiplier}).toFixed() % base64Chars.length;\n" +
					$"    this.flagString = this.flagString.substr(0,{Character}) + base64Chars[scaledValue] + this.flagString.substr({Character + 1});\n" +
					$"  }}\n" +
					$"}}\n";
			}
		}
	}
}
