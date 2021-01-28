using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;

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
		public bool ClassicItemPlacement
		{
			get => Flags.ClassicItemPlacement;
			set
			{
				Flags.ClassicItemPlacement = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("ClassicItemPlacement"));
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
		public WorldWealthMode WorldWealth
		{
			get => Flags.WorldWealth;
			set
			{
				Flags.WorldWealth = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("WorldWealthMode"));
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

		public bool ShardHuntEnabled => !FreeOrbs;

		public ShardCount ShardCount
		{
			get => Flags.ShardCount;
			set
			{
				Flags.ShardCount = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("ShardCount"));
			}
		}

		public bool ExtraShardsEnabled => ShardHunt && !FreeOrbs;

		public bool? TransformFinalFormation
		{
			get => Flags.TransformFinalFormation;
			set
			{
				Flags.TransformFinalFormation = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("TransformFinalFormation"));
			}
		}
		public bool? ShortToFR
		{
			get => Flags.ShortToFR;
			set
			{
				Flags.ShortToFR = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("ShortToFR"));
			}
		}
		public bool? PreserveFiendRefights
		{
			get => Flags.PreserveFiendRefights;
			set
			{
				Flags.PreserveFiendRefights = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("PreserveFiendRefights"));
			}
		}

		public bool? PreserveAllFiendRefights
		{
			get => Flags.PreserveAllFiendRefights;
			set
			{
				Flags.PreserveAllFiendRefights = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("PreserveAllFiendRefights"));
			}
		}

		public bool CanPreserveAllFiendRefights
		{
			get => (Flags.ShortToFR ?? true) && (Flags.PreserveFiendRefights ?? true);
			set
			{
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
		public bool? ItemMagic
		{
			get => Flags.ItemMagic;
			set
			{
				Flags.ItemMagic = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("ItemMagic"));
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
		public bool? MagisizeWeaponsBalanced
		{
			get => Flags.MagisizeWeaponsBalanced;
			set
			{
				Flags.MagisizeWeaponsBalanced = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("MagisizeWeaponsBalanced"));
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
		public Runnability Runnability
		{
			get => Flags.Runnability;
			set
			{
				Flags.Runnability = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Runnability"));
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

		public bool? EnemyScripts
		{
			get => Flags.EnemyScripts;
			set
			{
				Flags.EnemyScripts = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("EnemyScripts"));
			}
		}
		public bool? BossScriptsOnly
		{
			get => Flags.BossScriptsOnly;
			set
			{
				Flags.BossScriptsOnly = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("BossScripsOnly"));
			}
		}
		public bool? EnemySkillsSpells
		{
			get => Flags.EnemySkillsSpells;
			set
			{
				Flags.EnemySkillsSpells = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("EnemySkillsSpells"));
			}
		}
		public bool? BossSkillsOnly
		{
			get => Flags.BossSkillsOnly;
			set
			{
				Flags.BossSkillsOnly = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("BossSkillsOnly"));
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
		public bool? EnemyStatusAttacks
		{
			get => Flags.EnemyStatusAttacks;
			set
			{
				Flags.EnemyStatusAttacks = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("EnemyStatusAttacks"));
			}
		}
		public bool? RandomStatusAttacks
		{
			get => Flags.RandomStatusAttacks;
			set
			{
				Flags.RandomStatusAttacks = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("RandomStatusAttacks"));
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
		public bool? GuaranteedRuseItem
		{
			get => Flags.GuaranteedRuseItem;
			set
			{
				Flags.GuaranteedRuseItem = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("GuaranteedRuseItem"));
			}
		}
		public bool? DisableStunTouch
		{
			get => Flags.DisableStunTouch;
			set
			{
				Flags.DisableStunTouch = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("DisableStunTouch"));
			}
		}
		public bool CanShuffleTrapTiles => !((Flags.RandomizeEnemizer ?? false) || (Flags.RemoveTrapTiles ?? false));

		public bool? EnemyTrapTiles
		{
			get => Flags.EnemyTrapTiles;
			set
			{
				Flags.EnemyTrapTiles = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("EnemyTrapTiles"));
			}
		}
		public bool? RandomTrapFormations
		{
			get => Flags.RandomTrapFormations;
			set
			{
				Flags.RandomTrapFormations = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("RandomTrapFormations"));
			}
		}
		public bool? TrappedChests
		{
			get => Flags.TrappedChests;
			set
			{
				Flags.TrappedChests = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("TrappedChests"));
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
		public bool? TrappedShards
		{
			get => Flags.TrappedShards;
			set
			{
				Flags.TrappedShards = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("TrappedShards"));
			}
		}
		public bool? RemoveTrapTiles
		{
			get => Flags.RemoveTrapTiles;
			set
			{
				Flags.RemoveTrapTiles = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("RemoveTrapTiles"));
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
		public bool? LefeinShops
		{
			get => Flags.LefeinShops;
			set
			{
				Flags.LefeinShops = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("LefeinShops"));
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
		public bool? FlipDungeons
		{
			get => Flags.FlipDungeons;
			set
			{
				Flags.FlipDungeons = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("FlipDungeons"));
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
		public bool ChaosRush
		{
			get => Flags.ChaosRush;
			set
			{
				Flags.ChaosRush = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("ChaosRush"));
			}
		}

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
		public bool? IncentivizeRandomChestInLocation
		{
			get => Flags.IncentivizeRandomChestInLocation;
			set
			{
				Flags.IncentivizeRandomChestInLocation = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("IncentivizeRandomChestInLocation"));
			}
		}
		public bool? IncentivizeRandomChestIncludeExtra
		{
			get => Flags.IncentivizeRandomChestIncludeExtra;
			set
			{
				Flags.IncentivizeRandomChestIncludeExtra = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("IncentivizeRandomChestIncludeExtra"));
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
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("FreeAirship"));
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
		public bool FreeOrbs
		{
			get => Flags.FreeOrbs;
			set
			{
				Flags.FreeOrbs = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("FreeOrbs"));
			}
		}

		public bool? FreeLuteFlag
		{
			get => Flags.FreeLuteFlag;
			set
			{
				Flags.FreeLuteFlag = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("FreeLuteFlag"));
			}
		}

		public bool? FreeLute => FreeLuteFlag | ShortToFR;

		public bool FreeOrbsEnabled => !ShardHunt;

		public bool StartingGold
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
		public bool BuyTen
		{
			get => Flags.BuyTen;
			set
			{
				Flags.BuyTen = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("BuyTen"));
			}
		}
		public bool BuyTenOld
		{
			get => Flags.BuyTenOld;
			set
			{
				Flags.BuyTenOld = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("BuyTenOld"));
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

		public bool ChanceToRun
		{
			get => Flags.ChanceToRun;
			set
			{
				Flags.ChanceToRun = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("ChanceToRun"));
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
		public bool EnemySpellsTargetingAllies
		{
			get => Flags.EnemySpellsTargetingAllies;
			set
			{
				Flags.EnemySpellsTargetingAllies = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("EnemySpellsTargetingAllies"));
			}
		}
		public bool EnemyElementalResistancesBug
		{
			get => Flags.EnemyElementalResistancesBug;
			set
			{
				Flags.EnemyElementalResistancesBug = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("EnemyElementalResistancesBug"));
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

		public bool WrapPriceOverflow
		{
			get => Flags.WrapPriceOverflow;
			set
			{
				Flags.WrapPriceOverflow = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("WrapPriceOverflow"));
			}
		}
		public bool WrapStatOverflow
		{
			get => Flags.WrapStatOverflow;
			set
			{
				Flags.WrapStatOverflow = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("WrapStatOverflow"));
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
		public bool NoDanMode
		{
			get => Flags.NoDanMode;
			set
			{
				Flags.NoDanMode = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("NoDanMode"));
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
			get => Flags.ExpMultiplier;
			set
			{
				Flags.ExpMultiplier = value;
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
		public bool NoTabLayout
		{
			get => Preferences.NoTabLayout;
			set
			{
				Preferences.NoTabLayout = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("NoTabLayout"));
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
		public bool? RandomTreasure
		{
			get => Flags.RandomLoot;
			set
			{
				Flags.RandomLoot = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("RandomTreasure"));
			}
		}
		public WorldWealthMode WorldWealthEnum
		{
			get => Flags.WorldWealth;
			set
			{
				Flags.WorldWealth = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("WorldWealthEnum"));
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
		public bool EFGEarth1
		{
			get => Flags.EFGEarth1;
			set
			{
				Flags.EFGEarth1 = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("EFGEarth1"));
			}
		}
		public bool EFGEarth2
		{
			get => Flags.EFGEarth2;
			set
			{
				Flags.EFGEarth2 = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("EFGEarth2"));
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
		public bool PacifistMode
		{
			get => Flags.PacifistMode;
			set
			{
				Flags.PacifistMode = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("PacifistMode"));
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
		public bool? HintsDungeon
		{
			get => Flags.HintsDungeon;
			set
			{
				Flags.HintsDungeon = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("HintsDungeon"));
			}
		}
		public bool? HintsUseless
		{
			get => Flags.HintsUseless;
			set
			{
				Flags.HintsUseless = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("HintsUseless"));
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

		public bool? BalancedItemMagicShuffle
		{
			get => Flags.BalancedItemMagicShuffle;
			set
			{
				Flags.BalancedItemMagicShuffle = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("BalancedItemMagicShuffle"));
			}
		}

		public bool ?SeparateBossHPScaling
		{
			get => Flags.SeparateBossHPScaling;
			set
			{
				Flags.SeparateBossHPScaling = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("SeparateBossHPScaling"));
			}
		}

		public bool ?SeparateEnemyHPScaling
		{
			get => Flags.SeparateEnemyHPScaling;
			set
			{
				Flags.SeparateEnemyHPScaling = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("SeparateEnemyHPScaling"));
			}
		}

		public bool ?ClampBossHPScaling
		{
			get => Flags.ClampBossHPScaling;
			set
			{
				Flags.ClampBossHPScaling = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("ClampBossHPScaling"));
			}
		}

		public bool ?ClampEnemyHpScaling
		{
			get => Flags.ClampEnemyHpScaling;
			set
			{
				Flags.ClampEnemyHpScaling = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("ClampEnemyHpScaling"));
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
		public bool? RandomizeClassNoCasting
		{
			get => Flags.RandomizeClassNoCasting;
			set
			{
				Flags.RandomizeClassNoCasting = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("RandomizeClassNoCasting"));
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
		public bool? SwolePirates
		{
			get => Flags.SwolePirates;
			set
			{
				Flags.SwolePirates = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("SwolePirates"));
			}
		}
		public bool? ScaryImps
		{
			get => Flags.ScaryImps;
			set
			{
				Flags.ScaryImps = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("ScaryImps"));
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
	}
}
