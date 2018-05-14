using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using FF1Lib;

namespace FF1Randomizer
{
	public class MainWindowViewModel : INotifyPropertyChanged
	{
		public MainWindowViewModel()
		{
			Flags = new ViewModelFlags();
			Flags.PropertyChanged += (sender, args) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Flags"));
		}

		public event PropertyChangedEventHandler PropertyChanged;

		public string WindowTitle => $"FF1 Randomizer {FF1Rom.Version}";

		private string _filename;
		public string Filename
		{
			get => _filename;
			set
			{
				_filename = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Filename"));
			}
		}

		private string _seed;
		public string Seed
		{
			get => _seed;
			set
			{
				_seed = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Seed"));
			}
		}

		private ViewModelFlags _flags;
		public ViewModelFlags Flags
		{
			get => _flags;
			set
			{
				_flags = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Flags"));
			}
		}

		// This is so, so unfortunate.  Goddamn you WPF.
		public class ViewModelFlags : INotifyPropertyChanged
		{
			public ViewModelFlags()
			{
				Flags = new Flags();
			}

			public event PropertyChangedEventHandler PropertyChanged;

			// At least this trick saves us from having to declare backing fields, and having to write a conversion from ViewModelFlags to Flags.
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

			public bool Shops
			{
				get => Flags.Shops;
				set
				{
					Flags.Shops = value;
					PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Shops"));
				}
			}
			public bool Treasures
			{
				get => Flags.Treasures;
				set
				{
					Flags.Treasures = value;
					PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Treasures"));
				}
			}
			public bool NPCItems
			{
				get => Flags.NPCItems;
				set
				{
					Flags.NPCItems = value;
					PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("NPCItems"));
				}
			}
			public bool NPCFetchItems
			{
				get => Flags.NPCFetchItems;
				set
				{
					Flags.NPCFetchItems = value;
					PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("NPCFetchItems"));
				}
			}
			public bool RandomWares
			{
				get => Flags.RandomWares;
				set
				{
					Flags.RandomWares = value;
					PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("RandomWares"));
				}
			} // Planned 2.x feature - random weapons and armor in shops
			public bool RandomLoot
			{
				get => Flags.RandomLoot;
				set
				{
					Flags.RandomLoot = value;
					PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("RandomLoot"));
				}
			} // Planned 2.x feature - random non-quest-item treasures

			public bool ShardHunt
			{
				get => Flags.ShardHunt;
				set
				{
					Flags.ShardHunt = value;
					PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("ShardHunt"));
				}
			}

			public bool ShardHuntEnabled => !ChaosRush;

			public bool ExtraShards
			{
				get => Flags.ExtraShards;
				set
				{
					Flags.ExtraShards = value;
					PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("ExtraShards"));
				}
			}

			public bool ExtraShardsEnabled => ShardHunt && !ChaosRush;

			public bool TransformFinalFormation
			{
				get => Flags.TransformFinalFormation;
				set
				{
					Flags.TransformFinalFormation = value;
					PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("TransformFinalFormation"));
				}
			}

			public bool MagicShops
			{
				get => Flags.MagicShops;
				set
				{
					Flags.MagicShops = value;
					PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("MagicShops"));
				}
			}
			public bool MagicLevels
			{
				get => Flags.MagicLevels;
				set
				{
					Flags.MagicLevels = value;
					PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("MagicLevels"));
				}
			}
			public bool MagicPermissions
			{
				get => Flags.MagicPermissions;
				set
				{
					Flags.MagicPermissions = value;
					PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("MagicPermissions"));
				}
			}
			public bool ItemMagic
			{
				get => Flags.ItemMagic;
				set
				{
					Flags.ItemMagic = value;
					PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("ItemMagic"));
				}
			}

			public bool Rng
			{
				get => Flags.Rng;
				set
				{
					Flags.Rng = value;
					PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Rng"));
				}
			}
			public bool EnemyFormationsFrequency
			{
				get => Flags.EnemyFormationsFrequency;
				set
				{
					Flags.EnemyFormationsFrequency = value;
					PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("EnemyFormationsFrequency"));
				}
			}
			public bool EnemyFormationsUnrunnable
			{
				get => Flags.EnemyFormationsUnrunnable;
				set
				{
					Flags.EnemyFormationsUnrunnable = value;
					PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("EnemyFormationsUnrunnable"));
				}
			}
			public bool EnemyFormationsSurprise
			{
				get => Flags.EnemyFormationsSurprise;
				set
				{
					Flags.EnemyFormationsSurprise = value;
					PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("EnemyFormationsSurprise"));
				}
			}

			public bool EnemyScripts
			{
				get => Flags.EnemyScripts;
				set
				{
					Flags.EnemyScripts = value;
					PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("EnemyScripts"));
				}
			}
			public bool EnemySkillsSpells
			{
				get => Flags.EnemySkillsSpells;
				set
				{
					Flags.EnemySkillsSpells = value;
					PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("EnemySkillsSpells"));
				}
			}
			public bool EnemyStatusAttacks
			{
				get => Flags.EnemyStatusAttacks;
				set
				{
					Flags.EnemyStatusAttacks = value;
					PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("EnemyStatusAttacks"));
				}
			}
			public bool AllowUnsafePirates
			{
				get => Flags.AllowUnsafePirates;
				set
				{
					Flags.AllowUnsafePirates = value;
					PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("AllowUnsafePirates"));
				}
			}

			public bool OrdealsPillars
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

			public bool SkyCastle4FTeleporters
			{
				get => Flags.SkyCastle4FTeleporters;
				set
				{
					Flags.SkyCastle4FTeleporters = value;
					PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("SkyCastle4FTeleporters"));
				}
			}
			public bool TitansTrove
			{
				get => Flags.TitansTrove;
				set
				{
					Flags.TitansTrove = value;
					PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("TitansTrove"));
				}
			}
			public bool CrownlessOrdeals
			{
				get => Flags.CrownlessOrdeals;
				set
				{
					Flags.CrownlessOrdeals = value;
					PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("CrownlessOrdeals"));
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

			public bool ChaosRushEnabled => !ShardHunt;

			public bool Floors
			{
				get => Flags.Floors;
				set
				{
					Flags.Floors = value;
					PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Floors"));
				}
			} // Planned x.x feature - interior floors shuffle

			public bool MapOpenProgression
			{
				get => Flags.MapOpenProgression;
				set
				{
					Flags.MapOpenProgression = value;
					PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("MapOpenProgression"));
				}
			}
			public bool Entrances
			{
				get => Flags.Entrances;
				set
				{
					Flags.Entrances = value;
					PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Entrances"));
				}
			} // Planned x.x feature - non-town entrance shuffle
			public bool Towns
			{
				get => Flags.Towns;
				set
				{
					Flags.Towns = value;
					PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Towns"));
				}
			} // Planned x.x feature - town entrance shuffle

			public bool IncentivizeFreeNPCs
			{
				get => Flags.IncentivizeFreeNPCs;
				set
				{
					Flags.IncentivizeFreeNPCs = value;
					PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("IncentivizeFreeNPCs"));
				}
			}
			public bool IncentivizeFetchNPCs
			{
				get => Flags.IncentivizeFetchNPCs;
				set
				{
					Flags.IncentivizeFetchNPCs = value;
					PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("IncentivizeFetchNPCs"));
				}
			}

			public bool IncentivizeFreeNPCsEnabled => Treasures && NPCItems;
			public bool IncentivizeFetchNPCsEnabled => Treasures && NPCFetchItems;

			public bool IncentivizeTail
			{
				get => Flags.IncentivizeTail;
				set
				{
					Flags.IncentivizeTail = value;
					PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("IncentivizeTail"));
				}
			}
			public bool IncentivizeFetchItems
			{
				get => Flags.IncentivizeFetchItems;
				set
				{
					Flags.IncentivizeFetchItems = value;
					PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("IncentivizeFetchItems"));
				}
			}

			public bool IncentivizeMarsh
			{
				get => Flags.IncentivizeMarsh;
				set
				{
					Flags.IncentivizeMarsh = value;
					PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("IncentivizeMarsh"));
				}
			}
			public bool IncentivizeEarth
			{
				get => Flags.IncentivizeEarth;
				set
				{
					Flags.IncentivizeEarth = value;
					PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("IncentivizeEarth"));
				}
			}
			public bool IncentivizeVolcano
			{
				get => Flags.IncentivizeVolcano;
				set
				{
					Flags.IncentivizeVolcano = value;
					PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("IncentivizeVolcano"));
				}
			}
			public bool IncentivizeIceCave
			{
				get => Flags.IncentivizeIceCave;
				set
				{
					Flags.IncentivizeIceCave = value;
					PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("IncentivizeIceCave"));
				}
			}
			public bool IncentivizeOrdeals
			{
				get => Flags.IncentivizeOrdeals;
				set
				{
					Flags.IncentivizeOrdeals = value;
					PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("IncentivizeOrdeals"));
				}
			}
			public bool IncentivizeSeaShrine
			{
				get => Flags.IncentivizeSeaShrine;
				set
				{
					Flags.IncentivizeSeaShrine = value;
					PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("IncentivizeSeaShrine"));
				}
			}

			public bool IncentivizeConeria
			{
				get => Flags.IncentivizeConeria;
				set
				{
					Flags.IncentivizeConeria = value;
					PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("IncentivizeConeria"));
				}
			}
			public bool IncentivizeMarshKeyLocked
			{
				get => Flags.IncentivizeMarshKeyLocked;
				set
				{
					Flags.IncentivizeMarshKeyLocked = value;
					PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("IncentivizeMarshKeyLocked"));
				}
			}
			public bool IncentivizeSkyPalace
			{
				get => Flags.IncentivizeSkyPalace;
				set
				{
					Flags.IncentivizeSkyPalace = value;
					PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("IncentivizeSkyPalace"));
				}
			}

			public bool IncentivizeMasamune
			{
				get => Flags.IncentivizeMasamune;
				set
				{
					Flags.IncentivizeMasamune = value;
					PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("IncentivizeMasamune"));
				}
			}
			public bool IncentivizeOpal
			{
				get => Flags.IncentivizeOpal;
				set
				{
					Flags.IncentivizeOpal = value;
					PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("IncentivizeOpal"));
				}
			}
			public bool IncentivizeRibbon
			{
				get => Flags.IncentivizeRibbon;
				set
				{
					Flags.IncentivizeRibbon = value;
					PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("IncentivizeRibbon"));
				}
			}
			public bool IncentivizeRibbon2
			{
				get => Flags.IncentivizeRibbon2;
				set
				{
					Flags.IncentivizeRibbon2 = value;
					PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("IncentivizeRibbon2"));
				}
			}
			public bool Incentivize65K
			{
				get => Flags.Incentivize65K;
				set
				{
					Flags.Incentivize65K = value;
					PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Incentivize65K"));
				}
			}
			public bool IncentivizeBad
			{
				get => Flags.IncentivizeBad;
				set
				{
					Flags.IncentivizeBad = value;
					PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("IncentivizeBad"));
				}
			}

			public bool IncentivizeDefCastArmor
			{
				get => Flags.IncentivizeDefCastArmor;
				set
				{
					Flags.IncentivizeDefCastArmor = value;
					PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("IncentivizeDefCastArmor"));
				}
			}
			public bool IncentivizeOffCastArmor
			{
				get => Flags.IncentivizeOffCastArmor;
				set
				{
					Flags.IncentivizeOffCastArmor = value;
					PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("IncentivizeOffCastArmor"));
				}
			}
			public bool IncentivizeOtherCastArmor
			{
				get => Flags.IncentivizeOtherCastArmor;
				set
				{
					Flags.IncentivizeOtherCastArmor = value;
					PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("IncentivizeOtherCastArmor"));
				}
			}
			public bool IncentivizeDefCastWeapon
			{
				get => Flags.IncentivizeDefCastWeapon;
				set
				{
					Flags.IncentivizeDefCastWeapon = value;
					PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("IncentivizeDefCastWeapon"));
				}
			}
			public bool IncentivizeOffCastWeapon
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

			public bool EarlySarda
			{
				get => Flags.EarlySarda;
				set
				{
					Flags.EarlySarda = value;
					PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("EarlySarda"));
				}
			}
			public bool EarlySage
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

			public bool FreeBridge
			{
				get => Flags.FreeBridge;
				set
				{
					Flags.FreeBridge = value;
					PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("FreeBridge"));
				}
			}
			public bool FreeAirship
			{
				get => Flags.FreeAirship;
				set
				{
					Flags.FreeAirship = value;
					PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("FreeAirship"));
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

			public bool IdentifyTreasures
			{
				get => Flags.IdentifyTreasures;
				set
				{
					Flags.IdentifyTreasures = value;
					PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("IdentifyTreasures"));
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

			public bool HouseMPRestoration
			{
				get => Flags.HouseMPRestoration;
				set
				{
					Flags.HouseMPRestoration = value;
					PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("HouseMPRestoration"));
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

			public double EnemyScaleFactor
			{
				get => Flags.EnemyScaleFactor;
				set
				{
					Flags.EnemyScaleFactor = value;
					PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("EnemyScaleFactor"));
				}
			}
			public double PriceScaleFactor
			{
				get => Flags.PriceScaleFactor;
				set
				{
					Flags.PriceScaleFactor = value;
					PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("PriceScaleFactor"));
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
			public int ExpBonus
			{
				get => Flags.ExpBonus;
				set
				{
					Flags.ExpBonus = value;
					PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("ExpBonus"));
				}
			}
			public int ForcedPartyMembers
			{
				get => Flags.ForcedPartyMembers;
				set
				{
					Flags.ForcedPartyMembers = value;
					PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("ForcedPartyMembers"));
				}
			}

			public bool ModernBattlefield
			{
				get => Flags.ModernBattlefield;
				set
				{
					Flags.ModernBattlefield = value;
					PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("ModernBattlefield"));
				}
			}
			public bool FunEnemyNames
			{
				get => Flags.FunEnemyNames;
				set
				{
					Flags.FunEnemyNames = value;
					PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("FunEnemyNames"));
				}
			}
			public bool PaletteSwap
			{
				get => Flags.PaletteSwap;
				set
				{
					Flags.PaletteSwap = value;
					PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("PaletteSwap"));
				}
			}
			public bool TeamSteak
			{
				get => Flags.TeamSteak;
				set
				{
					Flags.TeamSteak = value;
					PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("TeamSteak"));
				}
			}
			public MusicShuffle Music
			{
				get => Flags.Music;
				set
				{
					Flags.Music = value;
					PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Music"));
				}
	}
}
	}

	public class FlagsToStringConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) => Flags.EncodeFlagsText(((MainWindowViewModel.ViewModelFlags)value).Flags);
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => new MainWindowViewModel.ViewModelFlags { Flags = Flags.DecodeFlagsText((string)value) };
	}
}
