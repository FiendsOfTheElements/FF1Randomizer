using System.ComponentModel;
using System.Runtime.CompilerServices;
using static FF1Lib.FF1Rom;

namespace FF1Lib
{
	public class AllClasses
	{
		public bool? AllForced { get; set; }
		public bool? AllFighters { get; set; }
		public bool? AllThiefs { get; set; }
		public bool? AllBlackBelts { get; set; }
		public bool? AllRedMages { get; set; }
		public bool? AllWhiteMages { get; set; }
		public bool? AllBlackMages { get; set; }
		public bool? AllKnights { get; set; }
		public bool? AllNinjas { get; set; }
		public bool? AllMasters { get; set; }
		public bool? AllRedWizards { get; set; }
		public bool? AllWhiteWizards { get; set; }
		public bool? AllBlackWizards { get; set; }
		public bool? AllNones { get; set; }

	}

	public class FlagsViewModel : INotifyPropertyChanged
	{
		public FlagsViewModel()
		{
			Flags = new Flags();
			Preferences = new Preferences();
			AllClasses = new AllClasses();
		}

		public string Encoded => Flags.EncodeFlagsText(Flags);

		public event PropertyChangedEventHandler PropertyChanged;

		private void RaisePropertyChanged([CallerMemberName] string property = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
		}

		public (string name, Flags flags, IEnumerable<string> log) FromJson(string json) => Flags.FromJson(json);

		// At least this trick saves us from having to declare backing fields, and having to write a conversion from FlagsViewModel to Flags.
		private Flags _flags;
		public Flags Flags
		{
			get => _flags;
			set
			{
				_flags = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Flags"));
			}
		}

		private Preferences _preferences;
		public Preferences Preferences
		{
			get => _preferences;
			set
			{
				_preferences = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Preferences"));
			}
		}

		private AllClasses _allClasses;
		public AllClasses AllClasses
		{
			get => _allClasses;
			set
			{
				_allClasses = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("AllClasses"));
			}
		}

		public bool? AllFighters
		{
			get
			{
				if (Flags.FIGHTER1 == Flags.FIGHTER2 && Flags.FIGHTER2 == Flags.FIGHTER3 && Flags.FIGHTER3 == Flags.FIGHTER4)
				{
					AllClasses.AllFighters = Flags.FIGHTER1;
				}
				else { AllClasses.AllFighters = false; }

				return AllClasses.AllFighters;
			}
			set
			{
				Flags.FIGHTER1 = value;
				Flags.FIGHTER2 = value;
				Flags.FIGHTER3 = value;
				Flags.FIGHTER4 = value;
				AllClasses.AllFighters = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("AllFighters"));
			}
		}
		public bool? AllThiefs
		{
			get
			{
				if (Flags.THIEF1 == Flags.THIEF2 && Flags.THIEF2 == Flags.THIEF3 && Flags.THIEF3 == Flags.THIEF4)
				{
					AllClasses.AllThiefs = Flags.THIEF1;
				}
				else { AllClasses.AllThiefs = false; }

				return AllClasses.AllThiefs;
			}
			set
			{
				Flags.THIEF1 = value;
				Flags.THIEF2 = value;
				Flags.THIEF3 = value;
				Flags.THIEF4 = value;
				AllClasses.AllThiefs = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("AllThiefs"));
			}
		}
		public bool? AllBlackBelts
		{
			get
			{
				if (Flags.BLACK_BELT1 == Flags.BLACK_BELT2 && Flags.BLACK_BELT2 == Flags.BLACK_BELT3 && Flags.BLACK_BELT3 == Flags.BLACK_BELT4)
				{
					AllClasses.AllBlackBelts = Flags.BLACK_BELT1;
				}
				else { AllClasses.AllBlackBelts = false; }

				return AllClasses.AllBlackBelts;
			}
			set
			{
				Flags.BLACK_BELT1 = value;
				Flags.BLACK_BELT2 = value;
				Flags.BLACK_BELT3 = value;
				Flags.BLACK_BELT4 = value;
				AllClasses.AllBlackBelts = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("AllBlackBelts"));
			}
		}

		public bool? AllRedMages
		{
			get
			{
				if (Flags.RED_MAGE1 == Flags.RED_MAGE2 && Flags.RED_MAGE2 == Flags.RED_MAGE3 && Flags.RED_MAGE3 == Flags.RED_MAGE4)
				{
					AllClasses.AllRedMages = Flags.RED_MAGE1;
				}
				else { AllClasses.AllRedMages = false; }

				return AllClasses.AllRedMages;
			}
			set
			{
				Flags.RED_MAGE1 = value;
				Flags.RED_MAGE2 = value;
				Flags.RED_MAGE3 = value;
				Flags.RED_MAGE4 = value;
				AllClasses.AllRedMages = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("AllRedMages"));
			}
		}
		public bool? AllWhiteMages
		{
			get
			{
				if (Flags.WHITE_MAGE1 == Flags.WHITE_MAGE2 && Flags.WHITE_MAGE2 == Flags.WHITE_MAGE3 && Flags.WHITE_MAGE3 == Flags.WHITE_MAGE4)
				{
					AllClasses.AllWhiteMages = Flags.WHITE_MAGE1;
				}
				else { AllClasses.AllWhiteMages = false; }

				return AllClasses.AllWhiteMages;
			}
			set
			{
				Flags.WHITE_MAGE1 = value;
				Flags.WHITE_MAGE2 = value;
				Flags.WHITE_MAGE3 = value;
				Flags.WHITE_MAGE4 = value;
				AllClasses.AllWhiteMages = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("AllWhiteMages"));
			}
		}
		public bool? AllBlackMages
		{
			get
			{
				if (Flags.BLACK_MAGE1 == Flags.BLACK_MAGE2 && Flags.BLACK_MAGE2 == Flags.BLACK_MAGE3 && Flags.BLACK_MAGE3 == Flags.BLACK_MAGE4)
				{
					AllClasses.AllBlackMages = Flags.BLACK_MAGE1;
				}
				else { AllClasses.AllBlackMages = false; }

				return AllClasses.AllBlackMages;
			}
			set
			{
				Flags.BLACK_MAGE1 = value;
				Flags.BLACK_MAGE2 = value;
				Flags.BLACK_MAGE3 = value;
				Flags.BLACK_MAGE4 = value;
				AllClasses.AllBlackMages = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("AllBlackMages"));
			}
		}
		public bool? AllKnights
		{
			get
			{
				if (Flags.KNIGHT1 == Flags.KNIGHT2 && Flags.KNIGHT2 == Flags.KNIGHT3 && Flags.KNIGHT3 == Flags.KNIGHT4)
				{
					AllClasses.AllKnights = Flags.KNIGHT1;
				}
				else { AllClasses.AllKnights = false; }

				return AllClasses.AllKnights;
			}
			set
			{
				Flags.KNIGHT1 = value;
				Flags.KNIGHT2 = value;
				Flags.KNIGHT3 = value;
				Flags.KNIGHT4 = value;
				AllClasses.AllKnights = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("AllKnights"));
			}
		}
		public bool? AllNinjas
		{
			get
			{
				if (Flags.NINJA1 == Flags.NINJA2 && Flags.NINJA2 == Flags.NINJA3 && Flags.NINJA3 == Flags.NINJA4)
				{
					AllClasses.AllNinjas = Flags.NINJA1;
				}
				else { AllClasses.AllNinjas = false; }

				return AllClasses.AllNinjas;
			}
			set
			{
				Flags.NINJA1 = value;
				Flags.NINJA2 = value;
				Flags.NINJA3 = value;
				Flags.NINJA4 = value;
				AllClasses.AllNinjas = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("AllNinjas"));
			}
		}
		public bool? AllMasters
		{
			get
			{
				if (Flags.MASTER1 == Flags.MASTER2 && Flags.MASTER2 == Flags.MASTER3 && Flags.MASTER3 == Flags.MASTER4)
				{
					AllClasses.AllMasters = Flags.MASTER1;
				}
				else { AllClasses.AllMasters = false; }

				return AllClasses.AllMasters;
			}
			set
			{
				Flags.MASTER1 = value;
				Flags.MASTER2 = value;
				Flags.MASTER3 = value;
				Flags.MASTER4 = value;
				AllClasses.AllMasters = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("AllMasters"));
			}
		}
		public bool? AllRedWizards
		{
			get
			{
				if (Flags.RED_WIZ1 == Flags.RED_WIZ2 && Flags.RED_WIZ2 == Flags.RED_WIZ3 && Flags.RED_WIZ3 == Flags.RED_WIZ4)
				{
					AllClasses.AllRedWizards = Flags.RED_WIZ1;
				}
				else { AllClasses.AllRedWizards = false; }

				return AllClasses.AllRedWizards;
			}
			set
			{
				Flags.RED_WIZ1 = value;
				Flags.RED_WIZ2 = value;
				Flags.RED_WIZ3 = value;
				Flags.RED_WIZ4 = value;
				AllClasses.AllRedWizards = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("AllRedWizards"));
			}
		}
		public bool? AllWhiteWizards
		{
			get
			{
				if (Flags.WHITE_WIZ1 == Flags.WHITE_WIZ2 && Flags.WHITE_WIZ2 == Flags.WHITE_WIZ3 && Flags.WHITE_WIZ3 == Flags.WHITE_WIZ4)
				{
					AllClasses.AllWhiteWizards = Flags.WHITE_WIZ1;
				}
				else { AllClasses.AllWhiteWizards = false; }

				return AllClasses.AllWhiteWizards;
			}
			set
			{
				Flags.WHITE_WIZ1 = value;
				Flags.WHITE_WIZ2 = value;
				Flags.WHITE_WIZ3 = value;
				Flags.WHITE_WIZ4 = value;
				AllClasses.AllWhiteWizards = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("AllWhiteWizards"));
			}
		}
		public bool? AllBlackWizards
		{
			get
			{
				if (Flags.BLACK_WIZ1 == Flags.BLACK_WIZ2 && Flags.BLACK_WIZ2 == Flags.BLACK_WIZ3 && Flags.BLACK_WIZ3 == Flags.BLACK_WIZ4)
				{
					AllClasses.AllBlackWizards = Flags.BLACK_WIZ1;
				}
				else { AllClasses.AllBlackWizards = false; }

				return AllClasses.AllBlackWizards;
			}
			set
			{
				Flags.BLACK_WIZ1 = value;
				Flags.BLACK_WIZ2 = value;
				Flags.BLACK_WIZ3 = value;
				Flags.BLACK_WIZ4 = value;
				AllClasses.AllBlackWizards = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("AllBlackWizards"));
			}
		}
		public bool? AllForced
		{
			get
			{
				if (Flags.FORCED1 == Flags.FORCED2 && Flags.FORCED2 == Flags.FORCED3 && Flags.FORCED3 == Flags.FORCED4)
				{
					AllClasses.AllForced = Flags.FORCED1;
				}
				else { AllClasses.AllForced = false; }

				return AllClasses.AllForced;
			}
			set
			{
				Flags.FORCED1 = value;
				Flags.FORCED2 = value;
				Flags.FORCED3 = value;
				Flags.FORCED4 = value;
				AllClasses.AllForced = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("AllForced"));
			}
		}
		public bool? AllNones
		{
			get
			{
				if (Flags.NONE_CLASS2 == Flags.NONE_CLASS3 && Flags.NONE_CLASS3 == Flags.NONE_CLASS4)
				{
					AllClasses.AllNones = Flags.NONE_CLASS2;
				}
				else { AllClasses.AllNones = false; }

				return AllClasses.AllNones;
			}
			set
			{
				Flags.NONE_CLASS2 = value;
				Flags.NONE_CLASS3 = value;
				Flags.NONE_CLASS4 = value;
				AllClasses.AllNones = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("AllNones"));
			}
		}
		public bool Spoilers
		{
			get => Flags.Spoilers;
			set
			{
				Flags.Spoilers = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Spoilers"));
			}
		}
		public bool BlindSeed
		{
			get => Flags.BlindSeed;
			set
			{
				Flags.BlindSeed = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("BlindSeed"));
			}
		}

		public bool TournamentSafe
		{
			get => Flags.TournamentSafe;
			set
			{
				Flags.TournamentSafe = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("TournamentSafe"));
			}
		}
		public bool? Shops
		{
			get => Flags.Shops;
			set
			{
				Flags.Shops = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Shops"));
			}
		}
		public bool? Treasures
		{
			get => Flags.Treasures;
			set
			{
				Flags.Treasures = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Treasures"));
			}
		}
		public bool? NPCItems
		{
			get => Flags.NPCItems;
			set
			{
				Flags.NPCItems = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("NPCItems"));
			}
		}
		public bool? NPCFetchItems
		{
			get => Flags.NPCFetchItems;
			set
			{
				Flags.NPCFetchItems = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("NPCFetchItems"));
			}
		}
		public bool? BetterTrapTreasure
		{
			get => Flags.BetterTrapChests;
			set
			{
				Flags.BetterTrapChests = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("BetterTrapChests"));
			}
		}

		public bool? ShuffleObjectiveNPCs
		{
			get => Flags.ShuffleObjectiveNPCs;
			set
			{
				Flags.ShuffleObjectiveNPCs = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("ShuffleObjectiveNPCs"));
			}
		}
		public bool? RandomWares
		{
			get => Flags.RandomWares;
			set
			{
				Flags.RandomWares = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("RandomWares"));
			}
		}
		public bool? RandomLoot
		{
			get => Flags.RandomLoot;
			set
			{
				Flags.RandomLoot = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("RandomLoot"));
			}
		}

		public FormationShuffleMode FormationShuffleMode
		{
			get => Flags.FormationShuffleMode;
			set
			{
				Flags.FormationShuffleMode = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("FormationShuffleMode"));
			}
		}
		public RandomizeTreasureMode RandomizeTreasure
		{
			get => Flags.RandomizeTreasure;
			set
			{
				Flags.RandomizeTreasure = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("RandomizeTreasure"));
			}
		}
		public bool OpenChestsInOrder
		{
			get => Flags.OpenChestsInOrder;
			set
			{
				Flags.OpenChestsInOrder = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("OpenChestsInOrder"));
			}
		}
		public bool SetRNG
		{
			get => Flags.SetRNG;
			set
			{
				Flags.SetRNG = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("SetRNG"));
			}
		}
		public WorldWealthMode WorldWealth
		{
			get => Flags.WorldWealth;
			set
			{
				Flags.WorldWealth = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("WorldWealthMode"));
			}
		}
		public DeepDungeonGeneratorMode DeepDungeonGenerator
		{
			get => Flags.DeepDungeonGenerator;
			set
			{
				Flags.DeepDungeonGenerator = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("DeepDungeonGenerator"));
			}
		}

		public bool ShardHunt
		{
			get => Flags.ShardHunt;
			set
			{
				Flags.ShardHunt = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("ShardHunt"));
			}
		}

		public bool ShardHuntEnabled => (Flags.OrbsRequiredCount == 4 || ShardHunt);

		public ShardCount ShardCount
		{
			get => Flags.ShardCount;
			set
			{
				Flags.ShardCount = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("ShardCount"));
			}
		}

		public int OrbsRequiredCount
		{
			get => Flags.OrbsRequiredCount;
			set
			{
				Flags.OrbsRequiredCount = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("OrbsRequiredCount"));
			}
		}

		public OrbsRequiredMode OrbsRequiredMode
		{
			get => Flags.OrbsRequiredMode;
			set
			{
				Flags.OrbsRequiredMode = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("OrbsRequiredMode"));
			}
		}


		public bool? OrbsRequiredSpoilers
		{
			get => Flags.OrbsRequiredSpoilers;
			set
			{
				Flags.OrbsRequiredSpoilers = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("OrbsRequiredSpoilers"));
			}
		}

		public bool OrbsRequiredEnabled => !ShardHunt && (Flags.GameMode != GameModes.DeepDungeon);
		public bool OrbsRequiredOptionsEnabled => OrbsRequiredEnabled && (Flags.OrbsRequiredCount != 4 && Flags.OrbsRequiredCount != 0);

		public FinalFormation TransformFinalFormation
		{
			get => Flags.TransformFinalFormation;
			set
			{
				Flags.TransformFinalFormation = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("TransformFinalFormation"));
			}
		}
		public bool? ChaosFloorEncounters
		{
			get => Flags.ChaosFloorEncounters;
			set
			{
				Flags.ChaosFloorEncounters = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("ChaosFloorEncounters"));
			}
		}
		public bool? ExitToFR
		{
			get => Flags.ExitToFR;
			set
			{
				Flags.ExitToFR = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("ExitToFR"));
			}
		}
		public ToFRMode ToFRMode
		{
			get => Flags.ToFRMode;
			set
			{
				Flags.ToFRMode = value;
				RaisePropertyChanged();
			}
		}
		public FiendsRefights FiendsRefights
		{
			get => Flags.FiendsRefights;
			set
			{
				Flags.FiendsRefights = value;
				RaisePropertyChanged();
			}
		}
		public bool? MagicShops
		{
			get => Flags.MagicShops;
			set
			{
				Flags.MagicShops = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("MagicShops"));
			}
		}

		public bool? MagicShopLocs
		{
			get => Flags.MagicShopLocs;
			set
			{
				Flags.MagicShopLocs = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("MagicShopLocs"));
			}
		}
		public bool? MagicShopLocationPairs
		{
			get => Flags.MagicShopLocationPairs;
			set
			{
				Flags.MagicShopLocationPairs = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("MagicShopLocationPairs"));
			}
		}
		public bool? MagicLevels
		{
			get => Flags.MagicLevels;
			set
			{
				Flags.MagicLevels = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("MagicLevels"));
			}
		}
		public bool? MagicPermissions
		{
			get => Flags.MagicPermissions;
			set
			{
				Flags.MagicPermissions = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("MagicPermissions"));
			}
		}
		public ItemMagicMode ItemMagicMode
		{
			get => Flags.ItemMagicMode;
			set
			{
				Flags.ItemMagicMode = value;
				RaisePropertyChanged();
			}
		}

		public bool? MagisizeWeapons
		{
			get => Flags.MagisizeWeapons;
			set
			{
				Flags.MagisizeWeapons = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("MagisizeWeapons"));
			}
		}

		public bool? Weaponizer
		{
			get => Flags.Weaponizer;
			set
			{
				Flags.Weaponizer = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Weaponizer"));
			}
		}
		public bool? WeaponizerNamesUseQualityOnly
		{
			get => Flags.WeaponizerNamesUseQualityOnly;
			set
			{
				Flags.WeaponizerNamesUseQualityOnly = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("WeaponizerNamesUseQualityOnly"));
			}
		}
		public bool? WeaponizerCommonWeaponsHavePowers
		{
			get => Flags.WeaponizerCommonWeaponsHavePowers;
			set
			{
				Flags.WeaponizerCommonWeaponsHavePowers = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("WeaponizerCommonWeaponsHavePowers"));
			}
		}
		public bool? ArmorCrafter
		{
			get => Flags.ArmorCrafter;
			set
			{
				Flags.ArmorCrafter = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("ArmorCrafter"));
			}
		}

		public bool? MagicLevelsTiered
		{
			get => Flags.MagicLevelsTiered;
			set
			{
				Flags.MagicLevelsTiered = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("MagicLevelsTiered"));
			}
		}
		public bool? MagicLevelsMixed
		{
			get => Flags.MagicLevelsMixed;
			set
			{
				Flags.MagicLevelsMixed = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("MagicLevelsMixed"));
			}
		}

		public AutohitThreshold MagicAutohitThreshold
		{
			get => Flags.MagicAutohitThreshold;
			set
			{
				Flags.MagicAutohitThreshold = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("MagicAutohitThreshold"));
			}
		}

		public bool? Rng
		{
			get => Flags.Rng;
			set
			{
				Flags.Rng = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Rng"));
			}
		}
		public bool FixMissingBattleRngEntry
		{
			get => Flags.FixMissingBattleRngEntry;
			set
			{
				Flags.FixMissingBattleRngEntry = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("FixMissingBattleRngEntry"));
			}
		}
		public bool? UnrunnableShuffle
		{
			get => Flags.UnrunnableShuffle;
			set
			{
				Flags.UnrunnableShuffle = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("UnrunnableShuffle"));
			}
		}
		public int UnrunnablesLow
		{
			get => Flags.UnrunnablesLow;
			set
			{
				Flags.UnrunnablesLow = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("UnrunnablesLow"));
			}
		}
		public int UnrunnablesHigh
		{
			get => Flags.UnrunnablesHigh;
			set
			{
				Flags.UnrunnablesHigh = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("UnrunnablesHigh"));
			}
		}
		public bool? UnrunnablesStrikeFirstAndSurprise
		{
			get => Flags.UnrunnablesStrikeFirstAndSurprise;
			set
			{
				Flags.UnrunnablesStrikeFirstAndSurprise = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("UnrunnablesStrikeFirstAndSurprise"));
			}
		}
		public bool? EnemyFormationsSurprise
		{
			get => Flags.EnemyFormationsSurprise;
			set
			{
				Flags.EnemyFormationsSurprise = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("EnemyFormationsSurprise"));
			}
		}

		public MDEFGrowthMode MDefMode
		{
			get => Flags.MDefMode;
			set
			{
				Flags.MDefMode = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("MDefChangesEnum"));
			}
		}

		public FormationShuffleMode FormationShuffleModeEnum
		{
			get => Flags.FormationShuffleMode;
			set
			{
				Flags.FormationShuffleMode = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("FormationShuffleModeEnum"));
			}
		}

		public bool? ShuffleScriptsEnemies
		{
			get => Flags.ShuffleScriptsEnemies;
			set
			{
				Flags.ShuffleScriptsEnemies = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("ShuffleScriptsEnemies"));
			}
		}

		public bool? ShuffleSkillsSpellsEnemies
		{
			get => Flags.ShuffleSkillsSpellsEnemies;
			set
			{
				Flags.ShuffleSkillsSpellsEnemies = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("ShuffleSkillsSpellsEnemies"));
			}
		}
		public bool? ShuffleSkillsSpellsBosses
		{
			get => Flags.ShuffleSkillsSpellsBosses;
			set
			{
				Flags.ShuffleSkillsSpellsBosses = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("ShuffleSkillsSpellsBosses"));
			}
		}
		public bool? NoConsecutiveNukes
		{
			get => Flags.NoConsecutiveNukes;
			set
			{
				Flags.NoConsecutiveNukes = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("NoConsecutiveNukes"));
			}
		}
		public bool TranceHasStatusElement
		{
			get => Flags.TranceHasStatusElement;
			set
			{
				Flags.TranceHasStatusElement = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("TranceHasStatusElement"));
			}
		}
		public bool? EnemySkillsSpellsTiered
		{
			get => Flags.EnemySkillsSpellsTiered;
			set
			{
				Flags.EnemySkillsSpellsTiered = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("EnemySkillsSpellsTiered"));
			}
		}

		public bool? AllowUnsafePirates
		{
			get => Flags.AllowUnsafePirates;
			set
			{
				Flags.AllowUnsafePirates = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("AllowUnsafePirates"));
			}
		}
		public bool? AllowUnsafeStartArea
		{
			get => Flags.AllowUnsafeStartArea;
			set
			{
				Flags.AllowUnsafeStartArea = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("AllowUnsafeStartArea"));
			}
		}
		public bool? EarlierRuby
		{
			get => Flags.EarlierRuby;
			set
			{
				Flags.EarlierRuby = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("EarlierRuby"));
			}
		}
		public GuaranteedDefenseItem GuaranteedDefenseItem
		{
			get => Flags.GuaranteedDefenseItem;
			set
			{
				Flags.GuaranteedDefenseItem = value;
				RaisePropertyChanged();
			}
		}
		public GuaranteedPowerItem GuaranteedPowerItem
		{
			get => Flags.GuaranteedPowerItem;
			set
			{
				Flags.GuaranteedPowerItem = value;
				RaisePropertyChanged();
			}
		}

		public TrapTileMode EnemyTrapTiles
		{
			get => Flags.EnemyTrapTiles;
			set
			{
				Flags.EnemyTrapTiles = value;
				RaisePropertyChanged();
			}
		}
		public FormationPool TCFormations
		{
			get => Flags.TCFormations;
			set
			{
				Flags.TCFormations = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("TCFormations"));
			}
		}
		public TCOptions TCBetterTreasure
		{
			get => Flags.TCBetterTreasure;
			set
			{
				Flags.TCBetterTreasure = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("TCBetterTreasure"));
			}
		}
		public TCOptions TCKeyItems
		{
			get => Flags.TCKeyItems;
			set
			{
				Flags.TCKeyItems = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("TCKeyItems"));
			}
		}
		public TCOptions TCShards
		{
			get => Flags.TCShards;
			set
			{
				Flags.TCShards = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("TCShards"));
			}
		}
		public bool TCExcludeCommons
		{
			get => Flags.TCExcludeCommons;
			set
			{
				Flags.TCExcludeCommons = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("TCExcludeCommons"));
			}
		}
		public int TCChestCount
		{
			get
			{
				if ((Flags.TCChestCount * 20) < Flags.TrappedChestsFloor)
				{
					return Flags.TrappedChestsFloor;
				}
				else
				{
					return Flags.TCChestCount * 20;
				}
			}
			set
			{
				Flags.TCChestCount = (value / 20);
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("TCInteger"));
			}
		}
		public bool TCProtectIncentives
		{
			get => Flags.TCProtectIncentives;
			set
			{
				Flags.TCProtectIncentives = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("TCProtectIncentives"));
			}
		}
		public bool? TCMasaGuardian
		{
			get => Flags.TCMasaGuardian;
			set
			{
				Flags.TCMasaGuardian = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("TCMasaGuardian"));
			}
		}
		public bool? TrappedChaos
		{
			get => Flags.TrappedChaos;
			set
			{
				Flags.TrappedChaos = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("TrappedChaos"));
			}
		}
		public bool? TCIndicator
		{
			get => Flags.TCIndicator;
			set
			{
				Flags.TCIndicator = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("TCIndicator"));
			}
		}

		public bool? OrdealsPillars
		{
			get => Flags.OrdealsPillars;
			set
			{
				Flags.OrdealsPillars = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("OrdealsPillars"));
			}
		}

		public WarMECHMode WarMECHMode
		{
			get => Flags.WarMECHMode;
			set
			{
				Flags.WarMECHMode = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("WarMECHMode"));
			}
		}

		public SkyCastle4FMazeMode SkyCastle4FMazeMode
		{
			get => Flags.SkyCastle4FMazeMode;
			set
			{
				Flags.SkyCastle4FMazeMode = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("SkyCastle4FMazeMode"));
			}
		}
		public bool? TitansTrove
		{
			get => Flags.TitansTrove;
			set
			{
				Flags.TitansTrove = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("TitansTrove"));
			}
		}
		public bool? LefeinSuperStore
		{
			get => Flags.LefeinSuperStore;
			set
			{
				Flags.LefeinSuperStore = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("LefeinSuperStore"));
			}
		}
		public bool LefeinSuperStoreEnabled => (ShopKillMode_White.Equals(ShopKillMode.None) && ShopKillMode_Black.Equals(ShopKillMode.None));
		public bool? LefeinShops
		{
			get => Flags.LefeinShops;
			set
			{
				Flags.LefeinShops = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("LefeinShops"));
			}
		}
		public bool? RandomVampAttack
		{
			get => Flags.RandomVampAttack;
			set
			{
				Flags.RandomVampAttack = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("RandomVampAttack"));
			}
		}
		public bool? RandomVampAttackIncludesConeria
		{
			get => Flags.RandomVampAttackIncludesConeria;
			set
			{
				Flags.RandomVampAttackIncludesConeria = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("RandomVampAttackIncludesConeria"));
			}
		}
		public bool? FightBahamut
		{
			get => Flags.FightBahamut;
			set
			{
				Flags.FightBahamut = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("FightBahamut"));
			}
		}
		public bool? SwoleBahamut
		{
			get => Flags.SwoleBahamut;
			set
			{
				Flags.SwoleBahamut = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("SwoleBahamut"));
			}
		}
		public bool? ConfusedOldMen
		{
			get => Flags.ConfusedOldMen;
			set
			{
				Flags.ConfusedOldMen = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("ConfusedOldMen"));
			}
		}
		public bool? GaiaShortcut
		{
			get => Flags.GaiaShortcut;
			set
			{
				Flags.GaiaShortcut = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("GaiaShortcut"));
			}
		}
		public int DamageTileLow
		{
			get => Flags.DamageTileLow;
			set
			{
				Flags.DamageTileLow = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("DamageTileLow"));
			}
		}
		public int DamageTileHigh
		{
			get => Flags.DamageTileHigh;
			set
			{
				Flags.DamageTileHigh = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("DamageTileHigh"));
			}
		}
		public bool? OWDamageTiles
		{
			get => Flags.OWDamageTiles;
			set
			{
				Flags.OWDamageTiles = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("OWDamageTiles"));
			}
		}
		public bool? DamageTilesKill
		{
			get => Flags.DamageTilesKill;
			set
			{
				Flags.DamageTilesKill = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("DamageTilesKill"));
			}
		}
		public bool? MoveGaiaItemShop
		{
			get => Flags.MoveGaiaItemShop;
			set
			{
				Flags.MoveGaiaItemShop = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("MoveGaiaItemShop"));
			}
		}
		public bool? ShufflePravokaShops
		{
			get => Flags.ShufflePravokaShops;
			set
			{
				Flags.ShufflePravokaShops = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("ShufflePravokaShops"));
			}
		}
		public bool? FlipDungeons
		{
			get => Flags.FlipDungeons;
			set
			{
				Flags.FlipDungeons = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("FlipDungeons"));
			}
		}
		public bool? VerticallyFlipDungeons
		{
			get => Flags.VerticallyFlipDungeons;
			set
			{
				Flags.VerticallyFlipDungeons = value;
				RaisePropertyChanged();
			}
		}
		public bool SpookyFlag
		{
			get => Flags.SpookyFlag;
			set
			{
				Flags.SpookyFlag = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("SpookyFlag"));
			}
		}
		public bool DraculasFlag
		{
		    get => Flags.DraculasFlag;
			set
			{
				Flags.DraculasFlag = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("DraculasFlag"));
			}
		}

		public bool? AllowUnsafeMelmond
		{
			get => Flags.AllowUnsafeMelmond;
			set
			{
				Flags.AllowUnsafeMelmond = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("AllowUnsafeMelmond"));
			}
		}
		public bool? EarlyOrdeals
		{
			get => Flags.EarlyOrdeals;
			set
			{
				Flags.EarlyOrdeals = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("EarlyOrdeals"));
			}
		}
		public bool? ChaosRush
		{
			get => Flags.ChaosRush;
			set
			{
				Flags.ChaosRush = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("ChaosRush"));
			}
		}

		public bool IsAnythingLoose => Flags.IsAnythingLoose;

		public bool? DeepCastlesPossible => Flags.DeepCastlesPossible;
		public bool? DeepTownsPossible => Flags.DeepTownsPossible;

		public bool? Floors
		{
			get => Flags.Floors;
			set
			{
				Flags.Floors = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Floors"));
			}
		}

		public bool? AllowDeepCastles
		{
			get => Flags.AllowDeepCastles;
			set
			{
				Flags.AllowDeepCastles = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("AllowDeepCastles"));
			}
		}

		public bool? AllowDeepTowns
		{
			get => Flags.AllowDeepTowns;
			set
			{
				Flags.AllowDeepTowns = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("AllowDeepTowns"));
			}
		}

		public bool? MapOpenProgression
		{
			get => Flags.MapOpenProgression;
			set
			{
				Flags.MapOpenProgression = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("MapOpenProgression"));
			}
		}

		public bool? MapOpenProgressionDocks
		{
			get => Flags.MapOpenProgressionDocks;
			set
			{
				Flags.MapOpenProgressionDocks = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("MapOpenProgressionDocks"));
			}
		}
		public bool? MapOpenProgressionExtended
		{
			get => Flags.MapOpenProgressionExtended;
			set
			{
				Flags.MapOpenProgressionExtended = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("MapOpenProgressionExtended"));
			}
		}
		public bool? MapDwarvesNorthwest
		{
			get => Flags.MapDwarvesNorthwest;
			set
			{
				Flags.MapDwarvesNorthwest = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("MapDwarvesNorthwest"));
			}
		}
		public bool? MapAirshipDock
		{
			get => Flags.MapAirshipDock;
			set
			{
				Flags.MapAirshipDock = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("MapAirshipDock"));
			}
		}
		public bool? MapBahamutCardiaDock
		{
			get => Flags.MapBahamutCardiaDock;
			set
			{
				Flags.MapBahamutCardiaDock = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("MapBahamutCardiaDock"));
			}
		}
		public bool? MapLefeinRiver
		{
			get => Flags.MapLefeinRiver;
			set
			{
				Flags.MapLefeinRiver = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("MapLefeinRiver"));
			}
		}
		public bool? MapBridgeLefein
		{
			get => Flags.MapBridgeLefein;
			set
			{
				Flags.MapBridgeLefein = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("MapBridgeLefein"));
			}
		}
		public bool? MapRiverToMelmond
		{
			get => Flags.MapRiverToMelmond;
			set
			{
				Flags.MapRiverToMelmond = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("MapRiverToMelmond"));
			}
		}
		public bool? MapGaiaMountainPass
		{
			get => Flags.MapGaiaMountainPass;
			set
			{
				Flags.MapGaiaMountainPass = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("GaiaMountainPass"));
			}
		}
		public bool? MapHighwayToOrdeals
		{
			get => Flags.MapHighwayToOrdeals;
			set
			{
				Flags.MapHighwayToOrdeals = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("MapHighwayToOrdeals"));
			}
		}
		public bool? MapDragonsHoard
		{
			get => Flags.MapDragonsHoard;
			set
			{
				Flags.MapDragonsHoard = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("MapDragonsHoard"));
			}
		}
		public bool? MapHallOfDragons
		{
			get => Flags.MapHallOfDragons;
			set
			{
				Flags.MapHallOfDragons = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("MapHallOfDragons"));
			}
		}

		public bool? Entrances
		{
			get => Flags.Entrances;
			set
			{
				Flags.Entrances = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Entrances"));
			}
		}
		public bool? Towns
		{
			get => Flags.Towns;
			set
			{
				Flags.Towns = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Towns"));
			}
		}
		public bool? IncludeConeria
		{
			get => Flags.IncludeConeria;
			set
			{
				Flags.IncludeConeria = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("IncludeConeria"));
			}
		}
		public bool? EntrancesIncludesDeadEnds
		{
			get => Flags.EntrancesIncludesDeadEnds;
			set
			{
				Flags.EntrancesIncludesDeadEnds = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("EntrancesIncludesDeadEnds"));
			}
		}
		public bool? EntrancesMixedWithTowns
		{
			get => Flags.EntrancesMixedWithTowns;
			set
			{
				Flags.EntrancesMixedWithTowns = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("EntrancesMixedWithTowns"));
			}
		}

		public bool? IncentivizeFreeNPCs
		{
			get => Flags.IncentivizeFreeNPCs;
			set
			{
				Flags.IncentivizeFreeNPCs = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("IncentivizeFreeNPCs"));
			}
		}
		public bool? IncentivizeFetchNPCs
		{
			get => Flags.IncentivizeFetchNPCs;
			set
			{
				Flags.IncentivizeFetchNPCs = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("IncentivizeFetchNPCs"));
			}
		}

		public bool IncentivizeFreeNPCsEnabled => (Treasures ?? true) && (NPCItems ?? true);
		public bool IncentivizeFetchNPCsEnabled => (Treasures ?? true) && (NPCFetchItems ?? true);

		public bool? IncentivizeTail
		{
			get => Flags.IncentivizeTail;
			set
			{
				Flags.IncentivizeTail = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("IncentivizeTail"));
			}
		}
		public bool? IncentivizeMainItems
		{
			get => Flags.IncentivizeMainItems;
			set
			{
				Flags.IncentivizeMainItems = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("IncentivizeMainItems"));
			}
		}
		public bool? IncentivizeFetchItems
		{
			get => Flags.IncentivizeFetchItems;
			set
			{
				Flags.IncentivizeFetchItems = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("IncentivizeFetchItems"));
			}
		}
		public bool? IncentivizeAirship
		{
			get => Flags.IncentivizeAirship;
			set
			{
				Flags.IncentivizeAirship = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("IncentivizeAirship"));
			}
		}
		public bool? IncentivizeCanoeItem
		{
			get => Flags.IncentivizeCanoeItem;
			set
			{
				Flags.IncentivizeCanoeItem = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("IncentivizeCanoeItem"));
			}
		}

		public bool? IncentivizeMarsh
		{
			get => Flags.IncentivizeMarsh;
			set
			{
				Flags.IncentivizeMarsh = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("IncentivizeMarsh"));
			}
		}

		public bool? IncentivizeTitansTrove
		{
			get => Flags.IncentivizeTitansTrove;
			set
			{
				Flags.IncentivizeTitansTrove = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("IncentivizeTitansTrove"));
			}
		}
		public bool? IncentivizeEarth
		{
			get => Flags.IncentivizeEarth;
			set
			{
				Flags.IncentivizeEarth = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("IncentivizeEarth"));
			}
		}
		public bool? IncentivizeVolcano
		{
			get => Flags.IncentivizeVolcano;
			set
			{
				Flags.IncentivizeVolcano = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("IncentivizeVolcano"));
			}
		}
		public bool? IncentivizeIceCave
		{
			get => Flags.IncentivizeIceCave;
			set
			{
				Flags.IncentivizeIceCave = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("IncentivizeIceCave"));
			}
		}

		public bool? IncentivizeOrdeals
		{
			get => Flags.IncentivizeOrdeals;
			set
			{
				Flags.IncentivizeOrdeals = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("IncentivizeOrdeals"));
			}
		}

		public bool? IncentivizeSeaShrine
		{
			get => Flags.IncentivizeSeaShrine;
			set
			{
				Flags.IncentivizeSeaShrine = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("IncentivizeSeaShrine"));
			}
		}

		public bool? IncentivizeConeria
		{
			get => Flags.IncentivizeConeria;
			set
			{
				Flags.IncentivizeConeria = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("IncentivizeConeria"));
			}
		}
		public bool? IncentivizeMarshKeyLocked
		{
			get => Flags.IncentivizeMarshKeyLocked;
			set
			{
				Flags.IncentivizeMarshKeyLocked = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("IncentivizeMarshKeyLocked"));
			}
		}
		public bool? IncentivizeSkyPalace
		{
			get => Flags.IncentivizeSkyPalace;
			set
			{
				Flags.IncentivizeSkyPalace = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("IncentivizeSkyPalace"));
			}
		}
		public bool? IncentivizeCardia
		{
			get => Flags.IncentivizeCardia;
			set
			{
				Flags.IncentivizeCardia = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("IncentivizeCardia"));
			}
		}

		public IncentivePlacementType IceCaveIncentivePlacementType
		{
			get => Flags.IceCaveIncentivePlacementType;
			set
			{
				Flags.IceCaveIncentivePlacementType = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("IceCaveIncentivePlacementType"));
			}
		}

		public IncentivePlacementType OrdealsIncentivePlacementType
		{
			get => Flags.OrdealsIncentivePlacementType;
			set
			{
				Flags.OrdealsIncentivePlacementType = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("OrdealsIncentivePlacementType"));
			}
		}

		public IncentivePlacementType MarshIncentivePlacementType
		{
			get => Flags.MarshIncentivePlacementType;
			set
			{
				Flags.MarshIncentivePlacementType = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("MarshIncentivePlacementType"));
			}
		}

		public IncentivePlacementType TitansIncentivePlacementType
		{
			get => Flags.TitansIncentivePlacementType;
			set
			{
				Flags.TitansIncentivePlacementType = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("TitansIncentivePlacementType"));
			}
		}

		public IncentivePlacementTypeEarth EarthIncentivePlacementType
		{
			get => Flags.EarthIncentivePlacementType;
			set
			{
				Flags.EarthIncentivePlacementType = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("EarthIncentivePlacementType"));
			}
		}

		public IncentivePlacementTypeVolcano VolcanoIncentivePlacementType
		{
			get => Flags.VolcanoIncentivePlacementType;
			set
			{
				Flags.VolcanoIncentivePlacementType = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("VolcanoIncentivePlacementType"));
			}
		}

		public IncentivePlacementTypeSea SeaShrineIncentivePlacementType
		{
			get => Flags.SeaShrineIncentivePlacementType;
			set
			{
				Flags.SeaShrineIncentivePlacementType = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("SeaShrineIncentivePlacementType"));
			}
		}

		public IncentivePlacementTypeSky SkyPalaceIncentivePlacementType
		{
			get => Flags.SkyPalaceIncentivePlacementType;
			set
			{
				Flags.SkyPalaceIncentivePlacementType = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("SkyPalaceIncentivePlacementType"));
			}
		}

		public IncentivePlacementType CorneriaIncentivePlacementType
		{
			get => Flags.CorneriaIncentivePlacementType;
			set
			{
				Flags.CorneriaIncentivePlacementType = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("CorneriaIncentivePlacementType"));
			}
		}

		public IncentivePlacementType MarshLockedIncentivePlacementType
		{
			get => Flags.MarshLockedIncentivePlacementType;
			set
			{
				Flags.MarshLockedIncentivePlacementType = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("MarshLockedIncentivePlacementType"));
			}
		}

		public IncentivePlacementType CardiaIncentivePlacementType
		{
			get => Flags.CardiaIncentivePlacementType;
			set
			{
				Flags.CardiaIncentivePlacementType = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("CardiaIncentivePlacementType"));
			}
		}

		public bool? IncentivizeMasamune
		{
			get => Flags.IncentivizeMasamune;
			set
			{
				Flags.IncentivizeMasamune = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("IncentivizeMasamune"));
			}
		}
		public bool? IncentivizeKatana
		{
			get => Flags.IncentivizeKatana;
			set
			{
				Flags.IncentivizeKatana = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("IncentivizeKatana"));
			}
		}
		public bool? IncentivizeVorpal
		{
			get => Flags.IncentivizeVorpal;
			set
			{
				Flags.IncentivizeVorpal = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("IncentivizeVorpal"));
			}
		}
		public bool? IncentivizeXcalber
		{
			get => Flags.IncentivizeXcalber;
			set
			{
				Flags.IncentivizeXcalber = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("IncentivizeXcalber"));
			}
		}

		public bool? IncentivizeOpal
		{
			get => Flags.IncentivizeOpal;
			set
			{
				Flags.IncentivizeOpal = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("IncentivizeOpal"));
			}
		}
		public bool? IncentivizeRibbon
		{
			get => Flags.IncentivizeRibbon;
			set
			{
				Flags.IncentivizeRibbon = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("IncentivizeRibbon"));
			}
		}
		public bool? IncentivizeDefCastArmor
		{
			get => Flags.IncentivizeDefCastArmor;
			set
			{
				Flags.IncentivizeDefCastArmor = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("IncentivizeDefCastArmor"));
			}
		}
		public bool? IncentivizeOffCastArmor
		{
			get => Flags.IncentivizeOffCastArmor;
			set
			{
				Flags.IncentivizeOffCastArmor = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("IncentivizeOffCastArmor"));
			}
		}
		public bool? IncentivizeOtherCastArmor
		{
			get => Flags.IncentivizeOtherCastArmor;
			set
			{
				Flags.IncentivizeOtherCastArmor = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("IncentivizeOtherCastArmor"));
			}
		}
		public bool? IncentivizePowerRod
		{
			get => Flags.IncentivizePowerRod;
			set
			{
				Flags.IncentivizePowerRod = value;
				RaisePropertyChanged();
			}
		}
		public bool? IncentivizeDefCastWeapon
		{
			get => Flags.IncentivizeDefCastWeapon;
			set
			{
				Flags.IncentivizeDefCastWeapon = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("IncentivizeDefCastWeapon"));
			}
		}
		public bool? IncentivizeOffCastWeapon
		{
			get => Flags.IncentivizeOffCastWeapon;
			set
			{
				Flags.IncentivizeOffCastWeapon = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("IncentivizeOffCastWeapon"));
			}
		}
		public bool IncentivizeOtherCastWeapon
		{
			get => Flags.IncentivizeOtherCastWeapon;
			set
			{
				Flags.IncentivizeOtherCastWeapon = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("IncentivizeOtherCastWeapon"));
			}
		}
		public bool? IncentivizeShipAndCanal
		{
			get => Flags.IncentivizeShipAndCanal;
			set
			{
				Flags.IncentivizeShipAndCanal = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("IncentivizeShipAndCanal"));
			}
		}
		public bool? IncentivizeBridgeItem
		{
			get => Flags.IncentivizeBridgeItem;
			set
			{
				Flags.IncentivizeBridgeItem = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("IncentivizeBridgeItem"));
			}
		}
		public bool? LooseExcludePlacedDungeons
		{
			get => Flags.LooseExcludePlacedDungeons;
			set
			{
				Flags.LooseExcludePlacedDungeons = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("LooseExcludePlacedDungeons"));
			}
		}
		public bool? EarlyKing
		{
			get => Flags.EarlyKing;
			set
			{
				Flags.EarlyKing = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("EarlyKing"));
			}
		}
		public bool? EarlySarda
		{
			get => Flags.EarlySarda;
			set
			{
				Flags.EarlySarda = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("EarlySarda"));
			}
		}
		public bool? EarlySage
		{
			get => Flags.EarlySage;
			set
			{
				Flags.EarlySage = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("EarlySage"));
			}
		}
		public bool OnlyRequireGameIsBeatable
		{
			get => Flags.OnlyRequireGameIsBeatable;
			set
			{
				Flags.OnlyRequireGameIsBeatable = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("OnlyRequireGameIsBeatable"));
			}
		}

		public bool? FreeBridge
		{
			get => Flags.FreeBridge;
			set
			{
				Flags.FreeBridge = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("FreeBridge"));
			}
		}
		public bool? FreeShip
		{
			get => Flags.FreeShip;
			set
			{
				Flags.FreeShip = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("FreeShip"));
			}
		}

		public bool? FreeAirship
		{
			get => Flags.FreeAirship;
			set
			{
				Flags.FreeAirship = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("FreeAirship"));
			}
		}
		public bool? FreeCanal
		{
			get => Flags.FreeCanal;
			set
			{
				Flags.FreeCanal = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("FreeCanal"));
			}
		}
		public bool? FreeCanoe
		{
			get => Flags.FreeCanoe;
			set
			{
				Flags.FreeCanoe = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("FreeCanoe"));
			}
		}

		public bool? FreeLute
		{
			get => Flags.FreeLute;
			set
			{
				Flags.FreeLute = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("FreeLute"));
			}
		}

		public bool? FreeRod
		{
			get => Flags.FreeRod;
			set
			{
				Flags.FreeRod = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("FreeRod"));
			}
		}

		public bool FreeOrbsEnabled => !ShardHunt;

		public bool? MelmondClinic
        {
            get => Flags.MelmondClinic;
            set
            {
                Flags.MelmondClinic = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("MelmondClinic"));
            }
        }

		public bool DDProgressiveTilesets
		{
			get => Flags.DDProgressiveTilesets;
			set
			{
				Flags.DDProgressiveTilesets = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("DDProgressiveTilesets"));
			}
		}
		public bool DDFiendOrbs
		{
			get => Flags.DDFiendOrbs;
			set
			{
				Flags.DDFiendOrbs = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("DDFiendOrbs"));
			}
		}
		public TailBahamutMode TailBahamutMode
		{
			get => Flags.TailBahamutMode;
			set
			{
				Flags.TailBahamutMode = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("TailBahamutMode"));
			}
		}

		public StartingGold StartingGold
		{
			get => Flags.StartingGold;
			set
			{
				Flags.StartingGold = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("StartingGold"));
			}
		}
		public bool EasyMode
		{
			get => Flags.EasyMode;
			set
			{
				Flags.EasyMode = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("EasyMode"));
			}
		}

		public bool SpeedHacks
		{
			get => Flags.SpeedHacks;
			set
			{
				Flags.SpeedHacks = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("SpeedHacks"));
			}
		}
		public bool InventoryAutosort
		{
			get => Flags.InventoryAutosort;
			set
			{
				Flags.InventoryAutosort = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("InventoryAutosort"));
			}
		}
		public bool RenounceAutosort
		{
			get => Preferences.RenounceAutosort;
			set
			{
				Preferences.RenounceAutosort = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("RenouceAutosort"));
			}
		}
		public bool RenounceChestInfo
		{
			get => Preferences.RenounceChestInfo;
			set
			{
				Preferences.RenounceChestInfo = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("RenounceChestInfo"));
			}
		}
		public bool RenounceCantHoldRed
		{
			get => Preferences.RenounceCantHoldRed;
			set
			{
				Preferences.RenounceCantHoldRed = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("RenounceCantHoldRed"));
			}
		}
		public bool NoPartyShuffle
		{
			get => Flags.NoPartyShuffle;
			set
			{
				Flags.NoPartyShuffle = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("NoPartyShuffle"));
			}
		}
		public bool Dash
		{
			get => Flags.Dash;
			set
			{
				Flags.Dash = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Dash"));
			}
		}
		public bool SpeedBoat
		{
			get => Flags.SpeedBoat;
			set
			{
				Flags.SpeedBoat = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("SpeedBoat"));
			}
		}
		public bool? AirBoat
		{
			get => Flags.AirBoat;
			set
			{
				Flags.AirBoat = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("AirBoat"));
			}
		}
		public bool BuyTen
		{
			get => Flags.BuyTen;
			set
			{
				Flags.BuyTen = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("BuyTen"));
			}
		}

		public bool IdentifyTreasures
		{
			get => Flags.IdentifyTreasures;
			set
			{
				Flags.IdentifyTreasures = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("IdentifyTreasures"));
			}
		}
		public bool ShopInfo
		{
			get => Flags.ShopInfo;
			set
			{
				Flags.ShopInfo = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("ShopInfo"));
			}
		}
		public bool ChestInfo
		{
			get => Flags.ChestInfo;
			set
			{
				Flags.ChestInfo = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("ChestInfo"));
			}
		}
		public bool IncentiveChestItemsFanfare
		{
			get => Flags.IncentiveChestItemsFanfare;
			set
			{
				Flags.IncentiveChestItemsFanfare = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("IncentiveChestItemsFanfare"));
			}
		}
		public bool WaitWhenUnrunnable
		{
			get => Flags.WaitWhenUnrunnable;
			set
			{
				Flags.WaitWhenUnrunnable = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("WaitWhenUnrunnable"));
			}
		}
		public bool EnableCritNumberDisplay
		{
			get => Flags.EnableCritNumberDisplay;
			set
			{
				Flags.EnableCritNumberDisplay = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("EnableCritNumberDisplay"));
			}
		}

		public bool HouseMPRestoration
		{
			get => Flags.HouseMPRestoration;
			set
			{
				Flags.HouseMPRestoration = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("HouseMPRestoration"));
			}
		}
		public bool HousesFillHP
		{
			get => Flags.HousesFillHp;
			set
			{
				Flags.HousesFillHp = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("HousesFillHp"));
			}
		}
		public bool WeaponStats
		{
			get => Flags.WeaponStats;
			set
			{
				Flags.WeaponStats = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("WeaponStats"));
			}
		}
		public bool BBCritRate
		{
			get => Flags.BBCritRate;
			set
			{
				Flags.BBCritRate = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("BBCritRate"));
			}
		}
		public bool WeaponCritRate
		{
			get => Flags.WeaponCritRate;
			set
			{
				Flags.WeaponCritRate = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("WeaponCritRate"));
			}
		}
		public bool WeaponBonuses
		{
			get => Flags.WeaponBonuses;
			set
			{
				Flags.WeaponBonuses = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("WeaponBonuses"));
			}
		}

		public int WeaponTypeBonusValue
		{
			get => Flags.WeaponTypeBonusValue;
			set
			{
				Flags.WeaponTypeBonusValue = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("WeaponTypeBonusValue"));
			}
		}

		public ChanceToRunMode ChanceToRun
		{
			get => Flags.ChanceToRun;
			set
			{
				Flags.ChanceToRun = value;
				RaisePropertyChanged();
			}
		}
		public bool SpellBugs
		{
			get => Flags.SpellBugs;
			set
			{
				Flags.SpellBugs = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("SpellBugs"));
			}
		}
		public bool BlackBeltAbsorb
		{
			get => Flags.BlackBeltAbsorb;
			set
			{
				Flags.BlackBeltAbsorb = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("BlackBeltAbsorb"));
			}
		}
		public bool NPCSwatter
		{
			get => Flags.NPCSwatter;
			set
			{
				Flags.NPCSwatter = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("NPCSwatter"));
			}
		}

		public bool BattleMagicMenuWrapAround
		{
			get => Flags.BattleMagicMenuWrapAround;
			set
			{
				Flags.BattleMagicMenuWrapAround = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("BattleMagicMenuWrapAround"));
			}
		}
		public bool EnemyStatusAttackBug
		{
			get => Flags.EnemyStatusAttackBug;
			set
			{
				Flags.EnemyStatusAttackBug = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("EnemyStatusAttackBug"));
			}
		}
		public bool ImproveTurnOrderRandomization
		{
			get => Flags.ImproveTurnOrderRandomization;
			set
			{
				Flags.ImproveTurnOrderRandomization = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("ImproveTurnOrderRandomization"));
			}
		}
		public bool FixHitChanceCap
		{
			get => Flags.FixHitChanceCap;
			set
			{
				Flags.FixHitChanceCap = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("FixHitChanceCap"));
			}
		}

		public int BossScaleStatsLow
		{
			get => Flags.BossScaleStatsLow;
			set
			{
				Flags.BossScaleStatsLow = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("BossScaleStatsLow"));
			}
		}
		public int BossScaleStatsHigh
		{
			get => Flags.BossScaleStatsHigh;
			set
			{
				Flags.BossScaleStatsHigh = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("BossScaleStatsHigh"));
			}
		}
		public int BossScaleHpLow
		{
			get => Flags.BossScaleHpLow;
			set
			{
				Flags.BossScaleHpLow = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("BossScaleHpLow"));
			}
		}
		public int BossScaleHpHigh
		{
			get => Flags.BossScaleHpHigh;
			set
			{
				Flags.BossScaleHpHigh = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("BossScaleHpHigh"));
			}
		}

		public int EnemyScaleStatsLow
		{
			get => Flags.EnemyScaleStatsLow;
			set
			{
				Flags.EnemyScaleStatsLow = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("EnemyScaleStatsLow"));
			}
		}
		public int EnemyScaleStatsHigh
		{
			get => Flags.EnemyScaleStatsHigh;
			set
			{
				Flags.EnemyScaleStatsHigh = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("EnemyScaleStatsHigh"));
			}
		}
		public int EnemyScaleHpLow
		{
			get => Flags.EnemyScaleHpLow;
			set
			{
				Flags.EnemyScaleHpLow = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("EnemyScaleHpLow"));
			}
		}
		public int EnemyScaleHpHigh
		{
			get => Flags.EnemyScaleHpHigh;
			set
			{
				Flags.EnemyScaleHpHigh = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("EnemyScaleHpHigh"));
			}
		}

		public int PriceScaleFactorLow
		{
			get => Flags.PriceScaleFactorLow;
			set
			{
				Flags.PriceScaleFactorLow = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("PriceScaleFactorLow"));
			}
		}

		public int PriceScaleFactorHigh
		{
			get => Flags.PriceScaleFactorHigh;
			set
			{
				Flags.PriceScaleFactorHigh = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("PriceScaleFactorHigh"));
			}
		}

		public bool IncludeMorale
		{
			get => Flags.IncludeMorale;
			set
			{
				Flags.IncludeMorale = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("IncludeMorale"));
			}
		}
		public bool DeadsGainXP
		{
			get => Flags.DeadsGainXP;
			set
			{
				Flags.DeadsGainXP = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("DeadsGainXP"));
			}
		}
		public bool? NoTail
		{
			get => Flags.NoTail;
			set
			{
				Flags.NoTail = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("NoTail"));
			}
		}
		public bool? NoFloater
		{
			get => Flags.NoFloater;
			set
			{
				Flags.NoFloater = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("NoFloater"));
			}
		}
		public bool? GuaranteedMasamune
		{
			get => Flags.GuaranteedMasamune;
			set
			{
				Flags.GuaranteedMasamune = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("GuaranteedMasamune"));
			}
		}
		public bool? SendMasamuneHome
		{
			get => Flags.SendMasamuneHome;
			set
			{
				Flags.SendMasamuneHome = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("SendMasamuneHome"));
			}
		}
		public bool? NoMasamune
		{
			get => Flags.NoMasamune;
			set
			{
				Flags.NoMasamune = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("NoMasamune"));
			}
		}

		public bool? NoXcalber
		{
			get => Flags.NoXcalber;
			set
			{
				Flags.NoXcalber = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("NoXcalber"));
			}
		}

		public bool? ClassAsNpcFiends
		{
			get => Flags.ClassAsNpcFiends;
			set
			{
				Flags.ClassAsNpcFiends = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("ClassAsNpcFiends"));
			}
		}
		public bool? ClassAsNpcKeyNPC
		{
			get => Flags.ClassAsNpcKeyNPC;
			set
			{
				Flags.ClassAsNpcKeyNPC = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("ClassAsNpcKeyNPC"));
			}
		}
		public int ClassAsNpcCount
		{
			get => Flags.ClassAsNpcCount;
			set
			{
				Flags.ClassAsNpcCount = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("ClassAsNpcCount"));
			}
		}
		public bool? ClassAsNpcDuplicate
		{
			get => Flags.ClassAsNpcDuplicate;
			set
			{
				Flags.ClassAsNpcDuplicate = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("ClassAsNpcDuplicate"));
			}
		}
		public bool? ClassAsNpcForcedFiends
		{
			get => Flags.ClassAsNpcForcedFiends;
			set
			{
				Flags.ClassAsNpcForcedFiends = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("ClassAsNpcForcedFiends"));
			}
		}
		public bool? ClassAsNpcPromotion
		{
			get => Flags.ClassAsNpcPromotion;
			set
			{
				Flags.ClassAsNpcPromotion = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("ClassAsNpcPromotion"));
			}
		}
		public ProgressiveScaleMode ProgressiveScaleMode
		{
			get => Flags.ProgressiveScaleMode;
			set
			{
				Flags.ProgressiveScaleMode = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("ProgressiveScaleMode"));
			}
		}
		public bool? ClampMinimumStatScale
		{
			get => Flags.ClampMinimumStatScale;
			set
			{
				Flags.ClampMinimumStatScale = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("ClampMinimumStatScale"));
			}
		}
		public bool? ClampMinimumBossStatScale
		{
			get => Flags.ClampMinimumBossStatScale;
			set
			{
				Flags.ClampMinimumBossStatScale = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("ClampMinimumBossStatScale"));
			}
		}
		public bool? ClampMinimumPriceScale
		{
			get => Flags.ClampMinimumPriceScale;
			set
			{
				Flags.ClampMinimumPriceScale = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("ClampMinimumPriceScale"));
			}
		}

		public double EncounterRate
		{
			get => Flags.EncounterRate;
			set
			{
				Flags.EncounterRate = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("EncounterRate"));
			}
		}
		public double DungeonEncounterRate
		{
			get => Flags.DungeonEncounterRate;
			set
			{
				Flags.DungeonEncounterRate = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("DungeonEncounterRate"));
			}
		}

		public double ExpMultiplier
		{
			get => Flags.ExpMultiplier * 10.0;
			set
			{
				Flags.ExpMultiplier = value * 0.1;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("ExpMultiplier"));
			}
		}
		public double ExpBonus
		{
			get => Flags.ExpBonus;
			set
			{
				Flags.ExpBonus = (int)value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("ExpBonus"));
			}
		}
		public double ExpMultiplierFighter
		{
			get => Flags.ExpMultiplierFighter * 10.0;
			set
			{
				Flags.ExpMultiplierFighter = value * 0.1;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("ExpMultiplierFighter"));
			}
		}
		public double ExpMultiplierThief
		{
			get => Flags.ExpMultiplierThief * 10.0;
			set
			{
				Flags.ExpMultiplierThief = value * 0.1;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("ExpMultiplierThief"));
			}
		}
		public double ExpMultiplierBlackBelt
		{
			get => Flags.ExpMultiplierBlackBelt * 10.0;
			set
			{
				Flags.ExpMultiplierBlackBelt = value * 0.1;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("ExpMultiplierBlackBelt"));
			}
		}
		public double ExpMultiplierRedMage
		{
			get => Flags.ExpMultiplierRedMage * 10.0;
			set
			{
				Flags.ExpMultiplierRedMage = value * 0.1;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("ExpMultiplierRedMage"));
			}
		}
		public double ExpMultiplierWhiteMage
		{
			get => Flags.ExpMultiplierWhiteMage * 10.0;
			set
			{
				Flags.ExpMultiplierWhiteMage = value * 0.1;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("ExpMultiplierWhiteMage"));
			}
		}
		public double ExpMultiplierBlackMage
		{
			get => Flags.ExpMultiplierBlackMage * 10.0;
			set
			{
				Flags.ExpMultiplierBlackMage = value * 0.1;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("ExpMultiplierBlackMage"));
			}
		}
		/*public int ForcedPartyMembers
		{
			get => Flags.ForcedPartyMembers;
			set
			{
				Flags.ForcedPartyMembers = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("ForcedPartyMembers"));
			}
		}*/

		public bool ModernBattlefield
		{
			get => Preferences.ModernBattlefield;
			set
			{
				Preferences.ModernBattlefield = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("ModernBattlefield"));
			}
		}
		public bool ThirdBattlePalette
		{
			get => Preferences.ThirdBattlePalette;
			set
			{
				Preferences.ThirdBattlePalette = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("ThirdBattlePalette"));
			}
		}
		public bool FunEnemyNames
		{
			get => Preferences.FunEnemyNames;
			set
			{
				Preferences.FunEnemyNames = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("FunEnemyNames"));
			}
		}
		public bool PaletteSwap
		{
			get => Preferences.PaletteSwap;
			set
			{
				Preferences.PaletteSwap = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("PaletteSwap"));
			}
		}
		public bool TeamSteak
		{
			get => Preferences.TeamSteak;
			set
			{
				Preferences.TeamSteak = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("TeamSteak"));
			}
		}
		public bool randomShardNames
		{
			get => Preferences.randomShardNames;
			set
			{
				Preferences.randomShardNames = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("randomShardNames"));
			}
		}
		public MusicShuffle Music
		{
			get => Preferences.Music;
			set
			{
				Preferences.Music = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Music"));
			}
		}
		public bool DisableDamageTileFlicker
		{
			get => Preferences.DisableDamageTileFlicker;
			set
			{
				Preferences.DisableDamageTileFlicker = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("DisableDamageTileFlicker"));
			}
		}
		public bool DisableDamageTileSFX
		{
			get => Preferences.DisableDamageTileSFX;
			set
			{
				Preferences.DisableDamageTileSFX = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("DisableDamageTileSFX"));
			}
		}
		public MenuColor MenuColor
		{
			get => Preferences.MenuColor;
			set
			{
				Preferences.MenuColor = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("MenuColor"));
			}
		}
		public bool ChangeLute
		{
			get => Preferences.ChangeLute;
			set
			{
				Preferences.ChangeLute = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("ChangeLute"));
			}
		}
		public bool AccessibleSpellNames
		{
			get => Preferences.AccessibleSpellNames;
			set
			{
				Preferences.AccessibleSpellNames = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("ModernizeSpellNames"));
			}
		}
		public bool CleanBlursedEquipmentNames
		{
			get => Preferences.CleanBlursedEquipmentNames;
			set
			{
				Preferences.CleanBlursedEquipmentNames = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("CleanBlursedEquipmentNames"));
			}
		}
		public bool ShopInfoIcons
		{
			get => Preferences.ShopInfoIcons;
			set
			{
				Preferences.ShopInfoIcons = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("ShopInfoIcons"));
			}
		}
		public bool NoTabLayout
		{
			get => Preferences.NoTabLayout;
			set
			{
				Preferences.NoTabLayout = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("NoTabLayout"));
			}
		}
		public bool BlandSite
		{
			get => Preferences.BlandSite;
			set
			{
				Preferences.BlandSite = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("BlandSite"));
			}
		}
		public TitanSnack TitanSnack
		{
			get => Preferences.TitanSnack;
			set
			{
				Preferences.TitanSnack = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("TitanSnack"));
			}
		}
		public Fate HurrayDwarfFate
		{
			get => Preferences.HurrayDwarfFate;
			set
			{
				Preferences.HurrayDwarfFate = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("HurrayDwarfFate"));
			}
		}

		public MapmanSlot MapmanSlot
		{
			get => Preferences.MapmanSlot;
			set
			{
				Preferences.MapmanSlot = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("MapmanSlot"));
			}
		}

		public bool DisableSpellCastFlash
		{
			get => Preferences.DisableSpellCastFlash;
			set
			{
				Preferences.DisableSpellCastFlash = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("DisableSpellCastFlash"));
			}
		}

		public bool LockRespondRate
		{
			get => Preferences.LockRespondRate;
			set
			{
				Preferences.LockRespondRate = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("LockRespondRate"));
			}
		}

		public int RespondRate
		{
			get => Preferences.RespondRate;
			set
			{
				Preferences.RespondRate = value;
				RaisePropertyChanged();
			}
		}

		public bool UninterruptedMusic
		{
			get => Preferences.UninterruptedMusic;
			set
			{
				Preferences.UninterruptedMusic = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("UninterruptedMusic"));
			}
		}

		public bool NoEmptyScripts
		{
			get => Flags.NoEmptyScripts;
			set
			{
				Flags.NoEmptyScripts = value;
				RaisePropertyChanged();
			}
		}
		public string SpriteSheet
		{
			get => Preferences.SpriteSheet;
			set
			{
				Preferences.SpriteSheet = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("SpriteSheet"));
			}
		}
		public string ResourcePack
		{
			get => Flags.ResourcePack;
			set
			{
				Flags.ResourcePack = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("ResourcePack"));
			}
		}

		public bool? RecruitmentMode
		{
			get => Flags.RecruitmentMode;
			set
			{
				Flags.RecruitmentMode = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("RecruitmentMode"));
			}
		}
		public bool? RecruitmentModeHireOnly
		{
			get => Flags.RecruitmentModeHireOnly;
			set
			{
				Flags.RecruitmentModeHireOnly = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("RecruitmentModeHireOnly"));
			}
		}
		public bool? RecruitmentModeReplaceOnlyNone
		{
			get => Flags.RecruitmentModeReplaceOnlyNone;
			set
			{
				Flags.RecruitmentModeReplaceOnlyNone = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("RecruitmentModeReplaceOnlyNone"));
			}
		}
		public bool? FORCED1
		{
			get => Flags.FORCED1;
			set
			{
				Flags.FORCED1 = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("FORCED1"));
			}
		}
		public bool? FORCED2
		{
			get => Flags.FORCED2;
			set
			{
				Flags.FORCED2 = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("FORCED2"));
			}
		}
		public bool? FORCED3
		{
			get => Flags.FORCED3;
			set
			{
				Flags.FORCED3 = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("FORCED3"));
			}
		}
		public bool? FORCED4
		{
			get => Flags.FORCED4;
			set
			{
				Flags.FORCED4 = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("FORCED4"));
			}
		}
		public bool? FIGHTER1
		{
			get => Flags.FIGHTER1;
			set
			{
				Flags.FIGHTER1 = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("FIGHTER1"));
			}
		}
		public bool? FIGHTER2
		{
			get => Flags.FIGHTER2;
			set
			{
				Flags.FIGHTER2 = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("FIGHTER2"));
			}
		}
		public bool? FIGHTER3
		{
			get => Flags.FIGHTER3;
			set
			{
				Flags.FIGHTER3 = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("FIGHTER3"));
			}
		}
		public bool? FIGHTER4
		{
			get => Flags.FIGHTER4;
			set
			{
				Flags.FIGHTER4 = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("FIGHTER4"));
			}
		}
		public bool? THIEF1
		{
			get => Flags.THIEF1;
			set
			{
				Flags.THIEF1 = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("THIEF1"));
			}
		}
		public bool? THIEF2
		{
			get => Flags.THIEF2;
			set
			{
				Flags.THIEF2 = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("THIEF2"));
			}
		}
		public bool? THIEF3
		{
			get => Flags.THIEF3;
			set
			{
				Flags.THIEF3 = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("THIEF3"));
			}
		}
		public bool? THIEF4
		{
			get => Flags.THIEF4;
			set
			{
				Flags.THIEF4 = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("THIEF4"));
			}
		}
		public bool? BLACK_BELT1
		{
			get => Flags.BLACK_BELT1;
			set
			{
				Flags.BLACK_BELT1 = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("BLACK_BELT1"));
			}
		}
		public bool? BLACK_BELT2
		{
			get => Flags.BLACK_BELT2;
			set
			{
				Flags.BLACK_BELT2 = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("BLACK_BELT2"));
			}
		}
		public bool? BLACK_BELT3
		{
			get => Flags.BLACK_BELT3;
			set
			{
				Flags.BLACK_BELT3 = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("BLACK_BELT3"));
			}
		}
		public bool? BLACK_BELT4
		{
			get => Flags.BLACK_BELT4;
			set
			{
				Flags.BLACK_BELT4 = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("BLACK_BELT4"));
			}
		}
		public bool? RED_MAGE1
		{
			get => Flags.RED_MAGE1;
			set
			{
				Flags.RED_MAGE1 = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("RED_MAGE1"));
			}
		}
		public bool? RED_MAGE2
		{
			get => Flags.RED_MAGE2;
			set
			{
				Flags.RED_MAGE2 = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("RED_MAGE2"));
			}
		}
		public bool? RED_MAGE3
		{
			get => Flags.RED_MAGE3;
			set
			{
				Flags.RED_MAGE3 = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("RED_MAGE3"));
			}
		}
		public bool? RED_MAGE4
		{
			get => Flags.RED_MAGE4;
			set
			{
				Flags.RED_MAGE4 = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("RED_MAGE4"));
			}
		}
		public bool? WHITE_MAGE1
		{
			get => Flags.WHITE_MAGE1;
			set
			{
				Flags.WHITE_MAGE1 = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("WHITE_MAGE1"));
			}
		}
		public bool? WHITE_MAGE2
		{
			get => Flags.WHITE_MAGE2;
			set
			{
				Flags.WHITE_MAGE2 = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("WHITE_MAGE2"));
			}
		}
		public bool? WHITE_MAGE3
		{
			get => Flags.WHITE_MAGE3;
			set
			{
				Flags.WHITE_MAGE3 = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("WHITE_MAGE3"));
			}
		}
		public bool? WHITE_MAGE4
		{
			get => Flags.WHITE_MAGE4;
			set
			{
				Flags.WHITE_MAGE4 = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("WHITE_MAGE4"));
			}
		}
		public bool? BLACK_MAGE1
		{
			get => Flags.BLACK_MAGE1;
			set
			{
				Flags.BLACK_MAGE1 = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("BLACK_MAGE1"));
			}
		}
		public bool? BLACK_MAGE2
		{
			get => Flags.BLACK_MAGE2;
			set
			{
				Flags.BLACK_MAGE2 = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("BLACK_MAGE2"));
			}
		}
		public bool? BLACK_MAGE3
		{
			get => Flags.BLACK_MAGE3;
			set
			{
				Flags.BLACK_MAGE3 = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("BLACK_MAGE3"));
			}
		}
		public bool? BLACK_MAGE4
		{
			get => Flags.BLACK_MAGE4;
			set
			{
				Flags.BLACK_MAGE4 = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("BLACK_MAGE4"));
			}
		}
		public bool? KNIGHT1
		{
			get => Flags.KNIGHT1;
			set
			{
				Flags.KNIGHT1 = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("KNIGHT1"));
			}
		}
		public bool? KNIGHT2
		{
			get => Flags.KNIGHT2;
			set
			{
				Flags.KNIGHT2 = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("KNIGHT2"));
			}
		}
		public bool? KNIGHT3
		{
			get => Flags.KNIGHT3;
			set
			{
				Flags.KNIGHT3 = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("KNIGHT3"));
			}
		}
		public bool? KNIGHT4
		{
			get => Flags.KNIGHT4;
			set
			{
				Flags.KNIGHT4 = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("KNIGHT4"));
			}
		}
		public bool? NINJA1
		{
			get => Flags.NINJA1;
			set
			{
				Flags.NINJA1 = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("NINJA1"));
			}
		}
		public bool? NINJA2
		{
			get => Flags.NINJA2;
			set
			{
				Flags.NINJA2 = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("NINJA2"));
			}
		}
		public bool? NINJA3
		{
			get => Flags.NINJA3;
			set
			{
				Flags.NINJA3 = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("NINJA3"));
			}
		}
		public bool? NINJA4
		{
			get => Flags.NINJA4;
			set
			{
				Flags.NINJA4 = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("NINJA4"));
			}
		}
		public bool? MASTER1
		{
			get => Flags.MASTER1;
			set
			{
				Flags.MASTER1 = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("MASTER1"));
			}
		}
		public bool? MASTER2
		{
			get => Flags.MASTER2;
			set
			{
				Flags.MASTER2 = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("MASTER2"));
			}
		}
		public bool? MASTER3
		{
			get => Flags.MASTER3;
			set
			{
				Flags.MASTER3 = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("MASTER3"));
			}
		}
		public bool? MASTER4
		{
			get => Flags.MASTER4;
			set
			{
				Flags.MASTER4 = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("MASTER4"));
			}
		}
		public bool? RED_WIZ1
		{
			get => Flags.RED_WIZ1;
			set
			{
				Flags.RED_WIZ1 = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("RED_WIZ1"));
			}
		}
		public bool? RED_WIZ2
		{
			get => Flags.RED_WIZ2;
			set
			{
				Flags.RED_WIZ2 = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("RED_WIZ2"));
			}
		}
		public bool? RED_WIZ3
		{
			get => Flags.RED_WIZ3;
			set
			{
				Flags.RED_WIZ3 = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("RED_WIZ3"));
			}
		}
		public bool? RED_WIZ4
		{
			get => Flags.RED_WIZ4;
			set
			{
				Flags.RED_WIZ4 = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("RED_WIZ4"));
			}
		}
		public bool? WHITE_WIZ1
		{
			get => Flags.WHITE_WIZ1;
			set
			{
				Flags.WHITE_WIZ1 = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("WHITE_WIZ1"));
			}
		}
		public bool? WHITE_WIZ2
		{
			get => Flags.WHITE_WIZ2;
			set
			{
				Flags.WHITE_WIZ2 = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("WHITE_WIZ2"));
			}
		}
		public bool? WHITE_WIZ3
		{
			get => Flags.WHITE_WIZ3;
			set
			{
				Flags.WHITE_WIZ3 = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("WHITE_WIZ3"));
			}
		}
		public bool? WHITE_WIZ4
		{
			get => Flags.WHITE_WIZ4;
			set
			{
				Flags.WHITE_WIZ4 = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("WHITE_WIZ4"));
			}
		}
		public bool? BLACK_WIZ1
		{
			get => Flags.BLACK_WIZ1;
			set
			{
				Flags.BLACK_WIZ1 = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("BLACK_WIZ1"));
			}
		}
		public bool? BLACK_WIZ2
		{
			get => Flags.BLACK_WIZ2;
			set
			{
				Flags.BLACK_WIZ2 = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("BLACK_WIZ2"));
			}
		}
		public bool? BLACK_WIZ3
		{
			get => Flags.BLACK_WIZ3;
			set
			{
				Flags.BLACK_WIZ3 = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("BLACK_WIZ3"));
			}
		}
		public bool? BLACK_WIZ4
		{
			get => Flags.BLACK_WIZ4;
			set
			{
				Flags.BLACK_WIZ4 = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("BLACK_WIZ4"));
			}
		}
		public bool? NONE_CLASS2
		{
			get => Flags.NONE_CLASS2;
			set
			{
				Flags.NONE_CLASS2 = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("NONE_CLASS2"));
			}
		}
		public bool? NONE_CLASS3
		{
			get => Flags.NONE_CLASS3;
			set
			{
				Flags.NONE_CLASS3 = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("NONE_CLASS3"));
			}
		}
		public bool? NONE_CLASS4
		{
			get => Flags.NONE_CLASS4;
			set
			{
				Flags.NONE_CLASS4 = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("NONE_CLASS4"));
			}
		}

		public bool? DraftFighter
		{
			get => Flags.DraftFighter;
			set
			{
				Flags.DraftFighter = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("DraftFighter"));
			}
		}
		public bool? DraftThief
		{
			get => Flags.DraftThief;
			set
			{
				Flags.DraftThief = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("DraftThief"));
			}
		}
		public bool? DraftBlackBelt
		{
			get => Flags.DraftBlackBelt;
			set
			{
				Flags.DraftBlackBelt = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("DraftBlackBelt"));
			}
		}
		public bool? DraftRedMage
		{
			get => Flags.DraftRedMage;
			set
			{
				Flags.DraftRedMage = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("DraftRedMage"));
			}
		}
		public bool? DraftWhiteMage
		{
			get => Flags.DraftWhiteMage;
			set
			{
				Flags.DraftWhiteMage = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("DraftWhiteMage"));
			}
		}
		public bool? DraftBlackMage
		{
			get => Flags.DraftBlackMage;
			set
			{
				Flags.DraftBlackMage = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("DraftBlackMage"));
			}
		}
		public bool? DraftKnight
		{
			get => Flags.DraftKnight;
			set
			{
				Flags.DraftKnight = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("DraftKnight"));
			}
		}
		public bool? DraftNinja
		{
			get => Flags.DraftNinja;
			set
			{
				Flags.DraftNinja = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("DraftNinja"));
			}
		}
		public bool? DraftMaster
		{
			get => Flags.DraftMaster;
			set
			{
				Flags.DraftMaster = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("DraftMaster"));
			}
		}
		public bool? DraftRedWiz
		{
			get => Flags.DraftRedWiz;
			set
			{
				Flags.DraftRedWiz = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("DraftRedWiz"));
			}
		}
		public bool? DraftWhiteWiz
		{
			get => Flags.DraftWhiteWiz;
			set
			{
				Flags.DraftWhiteWiz = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("DraftWhiteWiz"));
			}
		}
		public bool? DraftBlackWiz
		{
			get => Flags.DraftBlackWiz;
			set
			{
				Flags.DraftBlackWiz = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("DraftBlackWiz"));
			}
		}
		public bool? TAVERN1
		{
			get => Flags.TAVERN1;
			set
			{
				Flags.TAVERN1 = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("TAVERN1"));
			}
		}
		public bool? TAVERN2
		{
			get => Flags.TAVERN2;
			set
			{
				Flags.TAVERN2 = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("TAVERN2"));
			}
		}

		public bool? TAVERN3
		{
			get => Flags.TAVERN3;
			set
			{
				Flags.TAVERN3 = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("TAVERN3"));
			}
		}

		public bool? TAVERN4
		{
			get => Flags.TAVERN4;
			set
			{
				Flags.TAVERN4 = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("TAVERN4"));
			}
		}

		public bool? TAVERN5
		{
			get => Flags.TAVERN5;
			set
			{
				Flags.TAVERN5 = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("TAVERN5"));
			}
		}

		public bool? TAVERN6
		{
			get => Flags.TAVERN6;
			set
			{
				Flags.TAVERN6 = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("TAVERN6"));
			}
		}


		public bool? RandomWaresIncludesSpecialGear
		{
			get => Flags.RandomWaresIncludesSpecialGear;
			set
			{
				Flags.RandomWaresIncludesSpecialGear = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("RandomWaresIncludesSpecialGear"));
			}
		}
		public bool CanIncludeSpecialGear
		{
			get => (Flags.Shops ?? true) && (Flags.RandomWares ?? true);
			set
			{
			}
		}

		public bool? ClampPrices
		{
			get => Flags.ClampMinimumPriceScale;
			set
			{
				Flags.ClampMinimumPriceScale = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("ClampPrices"));
			}
		}
		public bool? ClampEnemies
		{
			get => Flags.ClampMinimumStatScale;
			set
			{
				Flags.ClampMinimumStatScale = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("ClampEnemies"));
			}
		}
		public bool? ClampBosses
		{
			get => Flags.ClampMinimumBossStatScale;
			set
			{
				Flags.ClampMinimumBossStatScale = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("ClampBosses"));
			}
		}
		public bool? ObjectiveNPCs
		{
			get => Flags.ShuffleObjectiveNPCs;
			set
			{
				Flags.ShuffleObjectiveNPCs = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("ObjectiveNPCs"));
			}
		}
		public bool? FiendShuffle
		{
			get => Flags.FiendShuffle;
			set
			{
				Flags.FiendShuffle = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("FiendShuffle"));
			}
		}

		public bool EFGWaterfall
		{
			get => Flags.EFGWaterfall;
			set
			{
				Flags.EFGWaterfall = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("EFGWaterfall"));
			}
		}
		public bool? ProcgenEarth
		{
			get => Flags.ProcgenEarth;
			set
			{
				Flags.ProcgenEarth = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("ProcgenEarth"));
			}
		}
		public bool DisableTentSaving
		{
			get => Flags.DisableTentSaving;
			set
			{
				Flags.DisableTentSaving = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("DisableTentSaving"));
			}
		}
		public bool DisableInnSaving
		{
			get => Flags.DisableInnSaving;
			set
			{
				Flags.DisableInnSaving = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("DisableInnSaving"));
			}
		}
		public bool SaveGameWhenGameOver
		{
			get => Flags.SaveGameWhenGameOver;
			set
			{
				Flags.SaveGameWhenGameOver = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("SaveGameWhenGameOver"));
			}
		}
		public bool SaveGameDWMode
		{
			get => Flags.SaveGameDWMode;
			set
			{
				Flags.SaveGameDWMode = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("SaveGameDWMode"));
			}
		}

		public bool? ShuffleAstos
		{
			get => Flags.ShuffleAstos;
			set
			{
				Flags.ShuffleAstos = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("ShuffleAstos"));
			}
		}
		public bool UnsafeAstos
		{
			get => Flags.UnsafeAstos;
			set
			{
				Flags.UnsafeAstos = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("UnsafeAstos"));
			}
		}

		public bool? RandomizeEnemizer
		{
			get => Flags.RandomizeEnemizer;
			set
			{
				Flags.RandomizeEnemizer = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("RandomizeEnemizer"));
			}
		}
		public bool? RandomizeFormationEnemizer
		{
			get => Flags.RandomizeFormationEnemizer;
			set
			{
				Flags.RandomizeFormationEnemizer = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("RandomizeFormationEnemizer"));
			}
		}
		public bool? GenerateNewSpellbook
		{
			get => Flags.GenerateNewSpellbook;
			set
			{
				Flags.GenerateNewSpellbook = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("GenerateNewSpellbook"));
			}
		}
		public bool? SpellcrafterMixSpells
		{
			get => Flags.SpellcrafterMixSpells;
			set
			{
				Flags.SpellcrafterMixSpells = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("SpellcrafterMixSpells"));
			}
		}
		public bool ThiefHitRate
		{
			get => Flags.ThiefHitRate;
			set
			{
				Flags.ThiefHitRate = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("ThiefHitRate"));
			}
		}
		public bool AllSpellLevelsForKnightNinja
		{
			get => Flags.AllSpellLevelsForKnightNinja;
			set
			{
				Flags.AllSpellLevelsForKnightNinja = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("AllSpellLevelsForKnightNinja"));
			}
		}
		public bool BuffHealingSpells
		{
			get => Flags.BuffHealingSpells;
			set
			{
				Flags.BuffHealingSpells = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("BuffHealingSpells"));
			}
		}
		public bool BuffTier1DamageSpells
		{
			get => Flags.BuffTier1DamageSpells;
			set
			{
				Flags.BuffTier1DamageSpells = value;
				RaisePropertyChanged();
			}
		}
		public bool? FreeTail
		{
			get => Flags.FreeTail;
			set
			{
				Flags.FreeTail = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("FreeTail"));
			}
		}
		public bool? HintsVillage
		{
			get => Flags.HintsVillage;
			set
			{
				Flags.HintsVillage = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("HintsVillage"));
			}
		}

		public bool? SpellcrafterRetainPermissions
		{
			get => Flags.SpellcrafterRetainPermissions;
			set
			{
				Flags.SpellcrafterRetainPermissions = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("SpellcrafterRetainPermissions"));
			}
		}

		public bool? RandomWeaponBonus
		{
			get => Flags.RandomWeaponBonus;
			set
			{
				Flags.RandomWeaponBonus = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("RandomWeaponBonus"));
			}
		}

		public bool? RandomArmorBonus
		{
			get => Flags.RandomArmorBonus;
			set
			{
				Flags.RandomArmorBonus = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("RandomArmorBonus"));
			}
		}

		public bool? RandomWeaponBonusExcludeMasa
		{
			get => Flags.RandomWeaponBonusExcludeMasa;
			set
			{
				Flags.RandomWeaponBonusExcludeMasa = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("RandomWeaponBonusExcludeMasa"));
			}
		}

		public int RandomWeaponBonusLow
		{
			get => Flags.RandomWeaponBonusLow;
			set
			{
				Flags.RandomWeaponBonusLow = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("RandomWeaponBonusLow"));
			}
		}
		public int RandomWeaponBonusHigh
		{
			get => Flags.RandomWeaponBonusHigh;
			set
			{
				Flags.RandomWeaponBonusHigh = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("RandomWeaponBonusHigh"));
			}
		}

		public int RandomArmorBonusLow
		{
			get => Flags.RandomArmorBonusLow;
			set
			{
				Flags.RandomArmorBonusLow = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("RandomArmorBonusLow"));
			}
		}
		public int RandomArmorBonusHigh
		{
			get => Flags.RandomArmorBonusHigh;
			set
			{
				Flags.RandomArmorBonusHigh = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("RandomArmorBonusHigh"));
			}
		}

		public ItemMagicPool ItemMagicPool
		{
			get => Flags.ItemMagicPool;
			set
			{
				Flags.ItemMagicPool = value;
				RaisePropertyChanged();
			}
		}

		public bool? SeparateBossHPScaling
		{
			get => Flags.SeparateBossHPScaling;
			set
			{
				Flags.SeparateBossHPScaling = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("SeparateBossHPScaling"));
			}
		}

		public bool? SeparateEnemyHPScaling
		{
			get => Flags.SeparateEnemyHPScaling;
			set
			{
				Flags.SeparateEnemyHPScaling = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("SeparateEnemyHPScaling"));
			}
		}

		public bool? ClampBossHPScaling
		{
			get => Flags.ClampBossHPScaling;
			set
			{
				Flags.ClampBossHPScaling = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("ClampBossHPScaling"));
			}
		}

		public bool? ClampEnemyHpScaling
		{
			get => Flags.ClampEnemyHpScaling;
			set
			{
				Flags.ClampEnemyHpScaling = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("ClampEnemyHpScaling"));
			}
		}

		public bool? EarlierHighTierMagic
		{
			get => Flags.EarlierHighTierMagic;
			set
			{
				Flags.EarlierHighTierMagic = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("EarlierHighTierMagic"));
			}
		}

		public bool? ChangeMaxMP
		{
			get => Flags.ChangeMaxMP;
			set
			{
				Flags.ChangeMaxMP = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("ChangeMaxMP"));
			}
		}

		public int RedMageMaxMP
		{
			get => Flags.RedMageMaxMP;
			set
			{
				Flags.RedMageMaxMP = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("RedMageMaxMP"));
			}
		}

		public int WhiteMageMaxMP
		{
			get => Flags.WhiteMageMaxMP;
			set
			{
				Flags.WhiteMageMaxMP = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("WhiteMageMaxMP"));
			}
		}

		public int BlackMageMaxMP
		{
			get => Flags.BlackMageMaxMP;
			set
			{
				Flags.BlackMageMaxMP = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("BlackMageMaxMP"));
			}
		}

		public int KnightMaxMP
		{
			get => Flags.KnightMaxMP;
			set
			{
				Flags.KnightMaxMP = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("KnightMaxMP"));
			}
		}
		public int NinjaMaxMP
		{
			get => Flags.NinjaMaxMP;
			set
			{
				Flags.NinjaMaxMP = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("NinjaMaxMP"));
			}
		}

		public MpGainOnMaxGain MpGainOnMaxGainMode
		{
			get => Flags.MpGainOnMaxGainMode;
			set
			{
				Flags.MpGainOnMaxGainMode = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("MpGainOnMaxGainMode"));
			}
		}

		public LockHitMode LockMode
		{
			get => Flags.LockMode;
			set
			{
				Flags.LockMode = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("LockMode"));
			}
		}

		public PoolSize PoolSize
		{
			get => Flags.PoolSize;
			set
			{
				Flags.PoolSize = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("PoolSize"));
			}
		}
		public bool? EnablePoolParty
		{
			get => Flags.EnablePoolParty;
			set
			{
				Flags.EnablePoolParty = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("EnablePoolParty"));
			}
		}
		public bool SafePoolParty
		{
			get => Flags.SafePoolParty;
			set
			{
				Flags.SafePoolParty = value;
				RaisePropertyChanged();
			}
		}
		public bool? IncludePromClasses
		{
			get => Flags.IncludePromClasses;
			set
			{
				Flags.IncludePromClasses = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("IncludePromClasses"));
			}
		}
		public bool? EnableRandomPromotions
		{
			get => Flags.EnableRandomPromotions;
			set
			{
				Flags.EnableRandomPromotions = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("EnableRandomPromotions"));
			}
		}
		public bool? IncludeBaseClasses
		{
			get => Flags.IncludeBaseClasses;
			set
			{
				Flags.IncludeBaseClasses = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("IncludeBaseClasses"));
			}
		}
		public bool? RandomPromotionsSpoilers
		{
			get => Flags.RandomPromotionsSpoilers;
			set
			{
				Flags.RandomPromotionsSpoilers = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("RandomPromotionsSpoilers"));
			}
		}

		public bool? RandomizeClass
		{
			get => Flags.RandomizeClass;
			set
			{
				Flags.RandomizeClass = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("RandomizeClass"));
			}
		}
		public bool? RandomizeClassChaos
		{
			get => Flags.RandomizeClassChaos;
			set
			{
				Flags.RandomizeClassChaos = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("RandomizeClassChaos"));
			}
		}
		public bool? MooglieWeaponBalance
		{
			get => Flags.MooglieWeaponBalance;
			set
			{
				Flags.MooglieWeaponBalance = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("MooglieWeaponBalance"));
			}
		}
		public bool? Transmooglifier
		{
			get => Flags.Transmooglifier;
			set
			{
				Flags.Transmooglifier = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Transmooglifier"));
			}
		}
		public bool? GuaranteeCustomClassComposition
		{
			get => Flags.GuaranteeCustomClassComposition;
			set
			{
				Flags.GuaranteeCustomClassComposition = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("GuaranteeCustomClassComposition"));
			}
		}
		public bool? RandomizeClassCasting
		{
			get => Flags.RandomizeClassCasting;
			set
			{
				Flags.RandomizeClassCasting = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("RandomizeClassCasting"));
			}
		}
		public bool? RandomizeClassKeyItems
		{
			get => Flags.RandomizeClassKeyItems;
			set
			{
				Flags.RandomizeClassKeyItems = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("RandomizeClassKeyItems"));
			}
		}
		public bool? RandomizeClassIncludeXpBonus
		{
			get => Flags.RandomizeClassIncludeXpBonus;
			set
			{
				Flags.RandomizeClassIncludeXpBonus = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("RandomizeClassIncludeXpBonus"));
			}
		}
		public int RandomizeClassMaxBonus
		{
			get => Flags.RandomizeClassMaxBonus;
			set
			{
				Flags.RandomizeClassMaxBonus = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("RandomizeClassMaxBonus"));
			}
		}
		public int RandomizeClassMaxMalus
		{
			get => Flags.RandomizeClassMaxMalus;
			set
			{
				Flags.RandomizeClassMaxMalus = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("RandomizeClassMaxMalus"));
			}
		}
		public bool? AlternateFiends
		{
			get => Flags.AlternateFiends;
			set
			{
				Flags.AlternateFiends = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("AlternateFiends"));
			}
		}
		public bool? ShuffleScriptsBosses
		{
			get => Flags.ShuffleScriptsBosses;
			set
			{
				Flags.ShuffleScriptsBosses = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("ShuffleScriptsBosses"));
			}
		}
		public bool? SwolePirates
		{
			get => Flags.SwolePirates;
			set
			{
				Flags.SwolePirates = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("SwolePirates"));
			}
		}
		public bool? SwoleAstos
		{
			get => Flags.SwoleAstos;
			set
			{
				Flags.SwoleAstos = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("SwoleAstos"));
			}
		}

		public EvadeCapValues EvadeCap
		{
			get => Flags.EvadeCap;
			set
			{
				Flags.EvadeCap = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("EvadeCap"));
			}
		}

		public StartingItemSet StartingItemSet
		{
			get => Flags.StartingItemSet;
			set
			{
				Flags.StartingItemSet = value;
				RaisePropertyChanged();
			}
		}

		public ShopKillMode ShopKillMode_Weapons
		{
			get => Flags.ShopKillMode_Weapons;
			set
			{
				Flags.ShopKillMode_Weapons = value;
				RaisePropertyChanged();
			}
		}

		public ShopKillMode ShopKillMode_Armor
		{
			get => Flags.ShopKillMode_Armor;
			set
			{
				Flags.ShopKillMode_Armor = value;
				RaisePropertyChanged();
			}
		}

		public ShopKillMode ShopKillMode_Item
		{
			get => Flags.ShopKillMode_Item;
			set
			{
				Flags.ShopKillMode_Item = value;
				RaisePropertyChanged();
			}
		}

		public ShopKillMode ShopKillMode_Black
		{
			get => Flags.ShopKillMode_Black;
			set
			{
				Flags.ShopKillMode_Black = value;
				RaisePropertyChanged();
			}
		}

		public ShopKillMode ShopKillMode_White
		{
			get => Flags.ShopKillMode_White;
			set
			{
				Flags.ShopKillMode_White = value;
				RaisePropertyChanged();
			}
		}

		public ShopKillFactor ShopKillFactor_Weapons
		{
			get => Flags.ShopKillFactor_Weapons;
			set
			{
				Flags.ShopKillFactor_Weapons = value;
				RaisePropertyChanged();
			}
		}

		public ShopKillFactor ShopKillFactor_Armor
		{
			get => Flags.ShopKillFactor_Armor;
			set
			{
				Flags.ShopKillFactor_Armor = value;
				RaisePropertyChanged();
			}
		}

		public ShopKillFactor ShopKillFactor_Item
		{
			get => Flags.ShopKillFactor_Item;
			set
			{
				Flags.ShopKillFactor_Item = value;
				RaisePropertyChanged();
			}
		}

		public ShopKillFactor ShopKillFactor_Black
		{
			get => Flags.ShopKillFactor_Black;
			set
			{
				Flags.ShopKillFactor_Black = value;
				RaisePropertyChanged();
			}
		}

		public ShopKillFactor ShopKillFactor_White
		{
			get => Flags.ShopKillFactor_White;
			set
			{
				Flags.ShopKillFactor_White = value;
				RaisePropertyChanged();
			}
		}

		public bool ShopKillExcludeConeria_Weapons
		{
			get => Flags.ShopKillExcludeConeria_Weapons;
			set
			{
				Flags.ShopKillExcludeConeria_Weapons = value;
				RaisePropertyChanged();
			}
		}

		public bool ShopKillExcludeConeria_Armor
		{
			get => Flags.ShopKillExcludeConeria_Armor;
			set
			{
				Flags.ShopKillExcludeConeria_Armor = value;
				RaisePropertyChanged();
			}
		}

		public bool ShopKillExcludeConeria_Item
		{
			get => Flags.ShopKillExcludeConeria_Item;
			set
			{
				Flags.ShopKillExcludeConeria_Item = value;
				RaisePropertyChanged();
			}
		}

		public bool ShopKillExcludeConeria_Black
		{
			get => Flags.ShopKillExcludeConeria_Black;
			set
			{
				Flags.ShopKillExcludeConeria_Black = value;
				RaisePropertyChanged();
			}
		}

		public bool ShopKillExcludeConeria_White
		{
			get => Flags.ShopKillExcludeConeria_White;
			set
			{
				Flags.ShopKillExcludeConeria_White = value;
				RaisePropertyChanged();
			}
		}

		public bool? LegendaryWeaponShop
		{
			get => Flags.LegendaryWeaponShop;
			set
			{
				Flags.LegendaryWeaponShop = value;
				RaisePropertyChanged();
			}
		}

		public bool? LegendaryArmorShop
		{
			get => Flags.LegendaryArmorShop;
			set
			{
				Flags.LegendaryArmorShop = value;
				RaisePropertyChanged();
			}
		}

		public bool? LegendaryWhiteShop
		{
			get => Flags.LegendaryWhiteShop;
			set
			{
				Flags.LegendaryWhiteShop = value;
				RaisePropertyChanged();
			}
		}

		public bool? LegendaryBlackShop
		{
			get => Flags.LegendaryBlackShop;
			set
			{
				Flags.LegendaryBlackShop = value;
				RaisePropertyChanged();
			}
		}

		public bool? LegendaryItemShop
		{
			get => Flags.LegendaryItemShop;
			set
			{
				Flags.LegendaryItemShop = value;
				RaisePropertyChanged();
			}
		}

		public bool ExclusiveLegendaryWeaponShop
		{
			get => Flags.ExclusiveLegendaryWeaponShop;
			set
			{
				Flags.ExclusiveLegendaryWeaponShop = value;
				RaisePropertyChanged();
			}
		}

		public bool ExclusiveLegendaryArmorShop
		{
			get => Flags.ExclusiveLegendaryArmorShop;
			set
			{
				Flags.ExclusiveLegendaryArmorShop = value;
				RaisePropertyChanged();
			}
		}

		public bool ExclusiveLegendaryWhiteShop
		{
			get => Flags.ExclusiveLegendaryWhiteShop;
			set
			{
				Flags.ExclusiveLegendaryWhiteShop = value;
				RaisePropertyChanged();
			}
		}

		public bool ExclusiveLegendaryBlackShop
		{
			get => Flags.ExclusiveLegendaryBlackShop;
			set
			{
				Flags.ExclusiveLegendaryBlackShop = value;
				RaisePropertyChanged();
			}
		}

		public bool ExclusiveLegendaryItemShop
		{
			get => Flags.ExclusiveLegendaryItemShop;
			set
			{
				Flags.ExclusiveLegendaryItemShop = value;
				RaisePropertyChanged();
			}
		}

		public bool NonesGainXP
		{
			get => Flags.NonesGainXP;
			set
			{
				Flags.NonesGainXP = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("NonesGainXP"));
			}
		}

		public bool ImprovedClinic
		{
			get => Flags.ImprovedClinic;
			set
			{
				Flags.ImprovedClinic = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("ImprovedClinic"));
			}
		}


		public bool Etherizer
		{
			get => Flags.Etherizer;
			set
			{
				Flags.Etherizer = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Etherizer"));
			}
		}
		public bool LaterLoose
		{
			get => Flags.LaterLoose;
			set
			{
				Flags.LaterLoose = value;
				RaisePropertyChanged();
			}
		}
		public TreasureStackSize ConsumableTreasureStackSize
		{
			get => Flags.ConsumableTreasureStackSize;
			set
			{
				Flags.ConsumableTreasureStackSize = value;
				RaisePropertyChanged();
			}
		}
		public StartingLevel StartingLevel
		{
			get => Flags.StartingLevel;
			set
			{
				Flags.StartingLevel = value;
				RaisePropertyChanged();
			}
		}
		public TransmooglifierVariance TransmooglifierVariance
		{
			get => Flags.TransmooglifierVariance;
			set
			{
				Flags.TransmooglifierVariance = value;
				RaisePropertyChanged();
			}
		}
		public int MaxLevelLow
		{
			get => Flags.MaxLevelLow;
			set
			{
				Flags.MaxLevelLow = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("MaxLevelLow"));
			}
		}
		public int MaxLevelHigh
		{
			get => Flags.MaxLevelHigh;
			set
			{
				Flags.MaxLevelHigh = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("MaxLevelHigh"));
			}
		}
		public ConsumableChestSet MoreConsumableChests
		{
			get => Flags.MoreConsumableChests;
			set
			{
				Flags.MoreConsumableChests = value;
				RaisePropertyChanged();
			}
		}
		public ThiefAGI ThiefAgilityBuff
		{
			get => Flags.ThiefAgilityBuff;
			set
			{
				Flags.ThiefAgilityBuff = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("ThiefAgilityBuff"));
			}
		}

		public bool? ExcludeGoldFromScaling
		{
			get => Flags.ExcludeGoldFromScaling;
			set
			{
				Flags.ExcludeGoldFromScaling = value;
				RaisePropertyChanged();
			}
		}

		public bool CheapVendorItem
		{
			get => Flags.CheapVendorItem;
			set
			{
				Flags.CheapVendorItem = value;
				RaisePropertyChanged();
			}
		}

		public OwMapExchanges OwMapExchange
		{
			get => Flags.OwMapExchange;
			set
			{
				Flags.OwMapExchange = value;
				RaisePropertyChanged();
			}
		}
		public bool OwShuffledAccess
		{
			get => Flags.OwShuffledAccess;
			set
			{
				Flags.OwShuffledAccess = value;
				RaisePropertyChanged();
			}
		}
		public bool OwUnsafeStart
		{
			get => Flags.OwUnsafeStart;
			set
			{
				Flags.OwUnsafeStart = value;
				RaisePropertyChanged();
			}
		}
		public bool OwRandomPregen
		{
			get => Flags.OwRandomPregen;
			set
			{
				Flags.OwRandomPregen = value;
				RaisePropertyChanged();
			}
		}
		public GameModes GameMode
		{
			get => Flags.GameMode;
			set
			{
				Flags.GameMode = value;
				RaisePropertyChanged();
			}
		}


		public SpoilerBatHints SkyWarriorSpoilerBats
		{
		    get => Flags.SkyWarriorSpoilerBats;
		    set
		    {
			Flags.SkyWarriorSpoilerBats = value;
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("SkyWarriorSpoilerBats"));
		    }
		}
		public bool? SpoilerBatsDontCheckOrbs
		{
		    get => Flags.SpoilerBatsDontCheckOrbs;
		    set
		    {
			Flags.SpoilerBatsDontCheckOrbs = value;
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("SpoilerBatsDontCheckOrbs"));
		    }
		}
		public bool? MoveToFBats
		{
		    get => Flags.MoveToFBats;
		    set
		    {
			Flags.MoveToFBats = value;
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("MoveToFBats"));
		    }
		}

		public bool SpoilerBatsDontCheckOrbsEnabled => !SkyWarriorSpoilerBats.Equals(SpoilerBatHints.Vanilla);

		public bool SanityCheckerV2
		{
			get => Flags.SanityCheckerV2;
			set
			{
				Flags.SanityCheckerV2 = value;
				RaisePropertyChanged();
			}
		}

		public int MapGenSeed
		{
			get => Flags.MapGenSeed;
			set
			{
				Flags.MapGenSeed = value;
				RaisePropertyChanged();
			}
		}

		public OwMapExchangeData ReplacementMap
		{
			get => Flags.ReplacementMap;
			set
			{
				Flags.ReplacementMap = value;
				RaisePropertyChanged();
			}
		}

		public ExtConsumableSet ExtConsumableSet
		{
			get => Flags.ExtConsumableSet;
			set
			{
				Flags.ExtConsumableSet = value;
				RaisePropertyChanged();
				RaisePropertyChanged(nameof(ExtConsumablesEnabled));
			}
		}

		public bool ExtConsumablesEnabled => Flags.ExtConsumablesEnabled;

		public LifeInBattleSetting EnableLifeInBattle
		{
			get => Flags.EnableLifeInBattle;
			set
			{
				Flags.EnableLifeInBattle = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("EnableLifeInBattle"));
			}
		}

		public bool EnableSoftInBattle
		{
			get => Flags.EnableSoftInBattle;
			set
			{
				Flags.EnableSoftInBattle = value;
				RaisePropertyChanged();
			}
		}

		public bool? NormalShopsHaveExtConsumables
		{
			get => Flags.NormalShopsHaveExtConsumables;
			set
			{
				Flags.NormalShopsHaveExtConsumables = value;
				RaisePropertyChanged();
			}
		}

		public bool? LegendaryShopHasExtConsumables
		{
			get => Flags.LegendaryShopHasExtConsumables;
			set
			{
				Flags.LegendaryShopHasExtConsumables = value;
				RaisePropertyChanged();
			}
		}

		public TreasureStackSize ExtConsumableTreasureStackSize
		{
			get => Flags.ExtConsumableTreasureStackSize;
			set
			{
				Flags.ExtConsumableTreasureStackSize = value;
				RaisePropertyChanged();
			}
		}

		public ExtStartingItemSet ExtStartingItemSet
		{
			get => Flags.ExtStartingItemSet;
			set
			{
				Flags.ExtStartingItemSet = value;
				RaisePropertyChanged();
			}
		}

		public ExtConsumableChestSet ExtConsumableChests
		{
			get => Flags.ExtConsumableChests;
			set
			{
				Flags.ExtConsumableChests = value;
				RaisePropertyChanged();
			}
		}


		public SpellNameMadness SpellNameMadness
		{
			get => Flags.SpellNameMadness;
			set
			{
				Flags.SpellNameMadness = value;
				RaisePropertyChanged();
			}
		}

		public bool? Lockpicking
		{
			get => Flags.Lockpicking;
			set
			{
				Flags.Lockpicking = value;
				RaisePropertyChanged();
			}
		}

		public bool? ReducedLuck
		{
			get => Flags.ReducedLuck;
			set
			{
				Flags.ReducedLuck = value;
				RaisePropertyChanged();
			}
		}

		public bool? IncreaseDarkPenalty
		{
			get => Flags.IncreaseDarkPenalty;
			set
			{
				Flags.IncreaseDarkPenalty = value;
				RaisePropertyChanged();
			}
		}

		public bool? TouchIncludeBosses
		{
			get => Flags.TouchIncludeBosses;
			set
			{
				Flags.TouchIncludeBosses = value;
				RaisePropertyChanged();
			}
		}

		public int LockpickingLevelRequirement
		{
			get => Flags.LockpickingLevelRequirement;
			set
			{
				Flags.LockpickingLevelRequirement = value;
				RaisePropertyChanged();
			}
		}

		public bool WhiteMageHarmEveryone
		{
			get => Flags.WhiteMageHarmEveryone;
			set
			{
				Flags.WhiteMageHarmEveryone = value;
				RaisePropertyChanged();
			}
		}

		public bool CropScreen
		{
			get => Preferences.CropScreen;
			set
			{
				Preferences.CropScreen = value;
				RaisePropertyChanged();
			}
		}

		public int ExpChestConversionMin
		{
			get => Flags.ExpChestConversionMin;
			set
			{
				Flags.ExpChestConversionMin = value;
				RaisePropertyChanged();
			}
		}

		public int ExpChestConversionMax
		{
			get => Flags.ExpChestConversionMax;
			set
			{
				Flags.ExpChestConversionMax = value;
				RaisePropertyChanged();
			}
		}

		public int ExpChestMinReward
		{
			get => Flags.ExpChestMinReward;
			set
			{
				Flags.ExpChestMinReward = value;
				RaisePropertyChanged();
			}
		}

		public int ExpChestMaxReward
		{
			get => Flags.ExpChestMaxReward;
			set
			{
				Flags.ExpChestMaxReward = value;
				RaisePropertyChanged();
			}
		}

		public bool LooseItemsNpcBalance
		{
			get => Flags.LooseItemsNpcBalance;
			set
			{
				Flags.LooseItemsNpcBalance = value;
				RaisePropertyChanged();
			}
		}

		#region StartingEquipment

		public bool? StartingEquipmentMasamune
		{
			get => Flags.StartingEquipmentMasamune;
			set
			{
				Flags.StartingEquipmentMasamune = value;
				RaisePropertyChanged();
			}
		}

		public bool? StartingEquipmentKatana
		{
			get => Flags.StartingEquipmentKatana;
			set
			{
				Flags.StartingEquipmentKatana = value;
				RaisePropertyChanged();
			}
		}

		public bool? StartingEquipmentHealStaff
		{
			get => Flags.StartingEquipmentHealStaff;
			set
			{
				Flags.StartingEquipmentHealStaff = value;
				RaisePropertyChanged();
			}
		}

		public bool? StartingEquipmentZeusGauntlet
		{
			get => Flags.StartingEquipmentZeusGauntlet;
			set
			{
				Flags.StartingEquipmentZeusGauntlet = value;
				RaisePropertyChanged();
			}
		}

		public bool? StartingEquipmentWhiteShirt
		{
			get => Flags.StartingEquipmentWhiteShirt;
			set
			{
				Flags.StartingEquipmentWhiteShirt = value;
				RaisePropertyChanged();
			}
		}

		public bool? StartingEquipmentRibbon
		{
			get => Flags.StartingEquipmentRibbon;
			set
			{
				Flags.StartingEquipmentRibbon = value;
				RaisePropertyChanged();
			}
		}

		public bool? StartingEquipmentDragonslayer
		{
			get => Flags.StartingEquipmentDragonslayer;
			set
			{
				Flags.StartingEquipmentDragonslayer = value;
				RaisePropertyChanged();
			}
		}

		public bool? StartingEquipmentLegendKit
		{
			get => Flags.StartingEquipmentLegendKit;
			set
			{
				Flags.StartingEquipmentLegendKit = value;
				RaisePropertyChanged();
			}
		}

		public bool? StartingEquipmentRandomEndgameWeapon
		{
			get => Flags.StartingEquipmentRandomEndgameWeapon;
			set
			{
				Flags.StartingEquipmentRandomEndgameWeapon = value;
				RaisePropertyChanged();
			}
		}

		public bool? StartingEquipmentRandomAoe
		{
			get => Flags.StartingEquipmentRandomAoe;
			set
			{
				Flags.StartingEquipmentRandomAoe = value;
				RaisePropertyChanged();
			}
		}

		public bool? StartingEquipmentRandomCasterItem
		{
			get => Flags.StartingEquipmentRandomCasterItem;
			set
			{
				Flags.StartingEquipmentRandomCasterItem = value;
				RaisePropertyChanged();
			}
		}

		public bool? StartingEquipmentGrandpasSecretStash
		{
			get => Flags.StartingEquipmentGrandpasSecretStash;
			set
			{
				Flags.StartingEquipmentGrandpasSecretStash = value;
				RaisePropertyChanged();
			}
		}

		public bool? StartingEquipmentOneItem
		{
			get => Flags.StartingEquipmentOneItem;
			set
			{
				Flags.StartingEquipmentOneItem = value;
				RaisePropertyChanged();
			}
		}

		public bool? StartingEquipmentRandomCrap
		{
			get => Flags.StartingEquipmentRandomCrap;
			set
			{
				Flags.StartingEquipmentRandomCrap = value;
				RaisePropertyChanged();
			}
		}

		public bool? StartingEquipmentStarterPack
		{
			get => Flags.StartingEquipmentStarterPack;
			set
			{
				Flags.StartingEquipmentStarterPack = value;
				RaisePropertyChanged();
			}
		}

		public bool? StartingEquipmentRandomTypeWeapon
		{
			get => Flags.StartingEquipmentRandomTypeWeapon;
			set
			{
				Flags.StartingEquipmentRandomTypeWeapon = value;
				RaisePropertyChanged();
			}
		}

		public bool StartingEquipmentRemoveFromPool
		{
			get => Flags.StartingEquipmentRemoveFromPool;
			set
			{
				Flags.StartingEquipmentRemoveFromPool = value;
				RaisePropertyChanged();
			}
		}

		public bool StartingEquipmentNoDuplicates
		{
			get => Flags.StartingEquipmentNoDuplicates;
			set
			{
				Flags.StartingEquipmentNoDuplicates = value;
				RaisePropertyChanged();
			}
		}

		#endregion

		public ScriptTouchMultiplier ScriptMultiplier
		{
			get => Flags.ScriptMultiplier;
			set
			{
				Flags.ScriptMultiplier = value;
				RaisePropertyChanged();
			}
		}

		public ScriptTouchMultiplier TouchMultiplier
		{
			get => Flags.TouchMultiplier;
			set
			{
				Flags.TouchMultiplier = value;
				RaisePropertyChanged();
			}
		}
		public TouchPool TouchPool
		{
			get => Flags.TouchPool;
			set
			{
				Flags.TouchPool = value;
				RaisePropertyChanged();
			}
		}
		public TouchMode TouchMode
		{
			get => Flags.TouchMode;
			set
			{
				Flags.TouchMode = value;
				RaisePropertyChanged();
			}
		}

		public bool OptOutSpeedHackWipes
		{
			get => Preferences.OptOutSpeedHackWipes;
			set
			{
				Preferences.OptOutSpeedHackWipes = value;
				RaisePropertyChanged();
			}
		}

		public bool OptOutSpeedHackMessages
		{
			get => Preferences.OptOutSpeedHackMessages;
			set
			{
				Preferences.OptOutSpeedHackMessages = value;
				RaisePropertyChanged();
			}
		}
		public bool OptOutSpeedHackDash
		{
			get => Preferences.OptOutSpeedHackDash;
			set
			{
				Preferences.OptOutSpeedHackDash = value;
				RaisePropertyChanged();
			}
		}
		public bool QuickJoy2Reset
		{
			get => Preferences.QuickJoy2Reset;
			set
			{
				Preferences.QuickJoy2Reset = value;
				RaisePropertyChanged();
			}
		}

		public bool DisableMinimap
		{
			get => Flags.DisableMinimap;
			set
			{
				Flags.DisableMinimap = value;
				RaisePropertyChanged();
			}
		}

		public bool Archipelago
		{
			get => Flags.Archipelago;
			set
			{
				Flags.Archipelago = value;
				RaisePropertyChanged();
			}
		}

		public bool ArchipelagoConsumables
		{
			get => Flags.ArchipelagoConsumables;
			set
			{
				Flags.ArchipelagoConsumables = value;
				RaisePropertyChanged();
			}
		}

		public bool ArchipelagoGold
		{
			get => Flags.ArchipelagoGold;
			set
			{
				Flags.ArchipelagoGold = value;
				RaisePropertyChanged();
			}
		}

		public bool ArchipelagoShards
		{
			get => Flags.ArchipelagoShards;
			set
			{
				Flags.ArchipelagoShards = value;
				RaisePropertyChanged();
			}
		}

		public ArchipelagoEquipment ArchipelagoEquipment
		{
			get => Flags.ArchipelagoEquipment;
			set
			{
				Flags.ArchipelagoEquipment = value;
				RaisePropertyChanged();
			}
		}

		public bool? MermaidPrison
		{
			get => Flags.MermaidPrison;
			set
			{
				Flags.MermaidPrison = value;
				RaisePropertyChanged();
			}
		}

		public bool? ReversedFloors
		{
			get => Flags.ReversedFloors;
			set
			{
				Flags.ReversedFloors = value;
				RaisePropertyChanged();
			}
		}

		public bool? RelocateChests
		{
		    get => Flags.RelocateChests;
			set
			{
				Flags.RelocateChests = value;
				RaisePropertyChanged();
			}
		}
		public bool RelocateChestsTrapIndicator
		{
		    get => Flags.RelocateChestsTrapIndicator;
			set
			{
				Flags.RelocateChestsTrapIndicator = value;
				RaisePropertyChanged();
			}
		}

		public bool? ShuffleChimeAccess
		{
		    get => Flags.ShuffleChimeAccess;
			set
			{
				Flags.ShuffleChimeAccess = value;
				RaisePropertyChanged();
			}
		}
		public bool? ShuffleChimeIncludeTowns
		{
		    get => Flags.ShuffleChimeIncludeTowns;
			set
			{
				Flags.ShuffleChimeIncludeTowns = value;
				RaisePropertyChanged();
			}
		}

		public string PlayerName
		{
			get => Preferences.PlayerName;
			set
			{
				Preferences.PlayerName = value;
				RaisePropertyChanged();
			}
		}

		public RibbonMode RibbonMode
		{
			get => Flags.RibbonMode;
			set
			{
				Flags.RibbonMode = value;
				RaisePropertyChanged();
			}
		}

		public bool ShipCanalBeforeFloater
		{
			get => Flags.ShipCanalBeforeFloater;
			set
			{
				Flags.ShipCanalBeforeFloater = value;
				RaisePropertyChanged();
			}
		}

		public void LoadResourcePackFlags(Stream stream) {
		    Flags.LoadResourcePackFlags(stream);
		    RaisePropertyChanged();
		}

		public KeyItemPlacementMode KeyItemPlacementMode
		{
			get
			{
				if (Flags.PredictivePlacement == false) return KeyItemPlacementMode.Vanilla;
				if (Flags.PredictivePlacement == true && Flags.AllowUnsafePlacement == false) return KeyItemPlacementMode.Predictive;
				return KeyItemPlacementMode.PredictiveUnsafe;
			}
			set
			{
				switch (value)
				{
					case KeyItemPlacementMode.Vanilla:
						Flags.PredictivePlacement = false;
						Flags.AllowUnsafePlacement = false;
						break;
					case KeyItemPlacementMode.Predictive:
						Flags.PredictivePlacement = true;
						Flags.AllowUnsafePlacement = false;
						break;
					case KeyItemPlacementMode.PredictiveUnsafe:
						Flags.PredictivePlacement = true;
						Flags.AllowUnsafePlacement = true;
						break;
				}

				RaisePropertyChanged();
			}
		}

		public LoosePlacementMode LoosePlacementMode
		{
			get
			{
				if (Flags.LooseItemsSpreadPlacement == false) return LoosePlacementMode.Vanilla;
				if (Flags.LooseItemsSpreadPlacement == true && Flags.LooseItemsForwardPlacement == false) return LoosePlacementMode.Spread;
				return LoosePlacementMode.Forward;
			}
			set
			{
				switch (value)
				{
					case LoosePlacementMode.Vanilla:
						Flags.LooseItemsSpreadPlacement = false;
						Flags.LooseItemsForwardPlacement = false;
						break;
					case LoosePlacementMode.Spread:
						Flags.LooseItemsSpreadPlacement = true;
						Flags.LooseItemsForwardPlacement = false;
						break;
					case LoosePlacementMode.Forward:
						Flags.LooseItemsSpreadPlacement = true;
						Flags.LooseItemsForwardPlacement = true;
						break;
				}

				RaisePropertyChanged();
			}
		}
	}
}
