using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Win32;
using FF1Lib;
using RomUtilities;

namespace FF1Randomizer
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		private string _filename;
		private Blob _seed;
		private Flags _flags;

		private class MainWindowViewModel
		{
			public string WindowTitle => $"FF1 Randomizer {FF1Rom.Version}";
		}

		public MainWindow()
		{
			InitializeComponent();

			DataContext = new MainWindowViewModel();

			TryOpenSavedFilename();
			GenerateSeed();

			SetScaleFactorLabel(PriceScaleFactorSlider, PriceScaleFactorLabel);
			SetScaleFactorLabel(EnemyScaleFactorSlider, EnemyScaleFactorLabel);
			SetExpLabel();
			SetPartyLabel();
			SetFlagsText(null, null);
		}

		private void TryOpenSavedFilename()
		{
			if (String.IsNullOrEmpty(Properties.Settings.Default.RomFilename))
			{
				return;
			}

			if (!File.Exists(Properties.Settings.Default.RomFilename))
			{
				Properties.Settings.Default.RomFilename = null;
				Properties.Settings.Default.Save();

				return;
			}

			ValidateRom(Properties.Settings.Default.RomFilename);
		}

		private void GenerateSeed()
		{
			_seed = Blob.Random(4);

			SeedTextBox.Text = _seed.ToHex();
		}

		private void RomButton_Click(object sender, RoutedEventArgs e)
		{
			var openFileDialog = new OpenFileDialog {
				Filter = "NES ROM files (*.nes)|*.nes"
			};

			var result = openFileDialog.ShowDialog(this);
			if (result == true)
			{
				ValidateRom(openFileDialog.FileName);

				Properties.Settings.Default.RomFilename = _filename;
				Properties.Settings.Default.Save();
			}
		}

		private void ValidateRom(string filename)
		{
			var rom = new FF1Rom(filename);
			if (!rom.Validate())
			{
				MessageBox.Show("ROM does not appear to be valid.  Proceed at your own risk.", "Validation Error");
			}

			_filename = filename;
			var slashIndex = filename.LastIndexOfAny(new[] { '/', '\\' });
			RomTextBox.Text = filename.Substring(slashIndex + 1);
			GenerateButton.IsEnabled = true;
		}

		private void SeedButton_Click(object sender, RoutedEventArgs e)
		{
			GenerateSeed();
		}

		private void SeedTextBox_LostFocus(object sender, RoutedEventArgs e)
		{
			SeedTextBox.Text = SeedTextBox.Text.Trim();

			SetSeed();
		}

		private void SetSeed()
		{
			try
			{
				_seed = Blob.FromHex(SeedTextBox.Text);
			}
			catch (Exception)
			{
				MessageBox.Show("Seeds must be eight hexadecimal characters (0-9, A-F).  Generating new seed.", "Invalid Seed");

				GenerateSeed();
			}
		}

		private void GenerateButton_Click(object sender, RoutedEventArgs e)
		{
			var rom = new FF1Rom(_filename);
			rom.Randomize(_seed, _flags);

			var fileRoot = _filename.Substring(0, _filename.LastIndexOf("."));
			var outputFilename = $"{fileRoot}_{_seed.ToHex()}_{FlagsTextBox.Text}.nes";
			rom.Save(outputFilename);

			MessageBox.Show($"Finished generating new ROM: {outputFilename}", "Done");
		}

		private void MagicLevelsCheckBox_OnChecked(object sender, RoutedEventArgs e)
		{
			if (MagicPermissionsCheckBox != null)
			{
				MagicPermissionsCheckBox.IsEnabled = true;
			}

			SetFlagsText(sender, e);
		}

		private void MagicLevelsCheckBox_OnUnchecked(object sender, RoutedEventArgs e)
		{
			if (MagicPermissionsCheckBox != null)
			{
				MagicPermissionsCheckBox.IsEnabled = false;
			}

			SetFlagsText(sender, e);
		}

		private void TreasuresCheckBox_OnChecked(object sender, RoutedEventArgs e)
		{
			if (IncentivizeIceCaveCheckBox != null)
			{
				IncentivizeIceCaveCheckBox.IsEnabled = true;
			}
			if (IncentivizeOrdealsCheckBox != null)
			{
				IncentivizeOrdealsCheckBox.IsEnabled = true;
			}
			if(IncentivizeMarshCheckBox != null)
			{
				IncentivizeMarshCheckBox.IsEnabled = true;
			}
			if (IncentivizeEarthCheckBox != null)
			{
				IncentivizeEarthCheckBox.IsEnabled = true;
			}
			if (IncentivizeVolcanoCheckBox != null)
			{
				IncentivizeVolcanoCheckBox.IsEnabled = true;
			}
			if (IncentivizeSeaShrineCheckBox != null)
			{
				IncentivizeSeaShrineCheckBox.IsEnabled = true;
			}
			if (IncentivizeSkyPalaceCheckBox != null)
			{
				IncentivizeSkyPalaceCheckBox.IsEnabled = true;
			}
			if (IncentivizeConeriaCheckBox != null)
			{
				IncentivizeConeriaCheckBox.IsEnabled = true;
			}
			if (IncentivizeMarshKeyLockedCheckBox != null)
			{
				IncentivizeMarshKeyLockedCheckBox.IsEnabled = true;
			}
			if (IncentivizeTailCheckBox != null)
			{
				IncentivizeTailCheckBox.IsEnabled = true;
			}
			if (IncentivizeFetchItemsCheckBox != null)
			{
				IncentivizeFetchItemsCheckBox.IsEnabled = true;
			}
			if (IncentivizeMasamuneCheckBox != null)
			{
				IncentivizeMasamuneCheckBox.IsEnabled = true;
			}
			if (IncentivizeRibbonCheckBox != null)
			{
				IncentivizeRibbonCheckBox.IsEnabled = true;
			}
			if (IncentivizeOpalCheckBox != null)
			{
				IncentivizeOpalCheckBox.IsEnabled = true;
			}
			if (IncentivizeDefCastArmorCheckBox != null)
			{
				IncentivizeDefCastArmorCheckBox.IsEnabled = true;
			}
			if (IncentivizeOtherCastArmorCheckBox != null)
			{
				IncentivizeOtherCastArmorCheckBox.IsEnabled = true;
			}
			if (IncentivizeDefCastWeaponCheckBox != null)
			{
				IncentivizeDefCastWeaponCheckBox.IsEnabled = true;
			}
			if (IncentivizeOffCastWeaponCheckBox != null)
			{
				IncentivizeOffCastWeaponCheckBox.IsEnabled = true;
			}
			if (IncentivizeFreeNPCsCheckBox != null)
			{
				if (NPCItemsCheckBox != null)
				{
					IncentivizeFreeNPCsCheckBox.IsEnabled = NPCItemsCheckBox.IsEnabled;
				}
				else
				{
					IncentivizeFreeNPCsCheckBox.IsEnabled = false;
				}
			}
			if (IncentivizeFetchNPCsCheckBox != null)
			{
				if (NPCFetchItemsCheckBox != null)
				{
					IncentivizeFetchNPCsCheckBox.IsEnabled = NPCFetchItemsCheckBox.IsEnabled;
				}
				else
				{
					IncentivizeFetchNPCsCheckBox.IsEnabled = false;
				}
			}

			SetFlagsText(sender, e);
		}

		private void TreasuresCheckBox_OnUnchecked(object sender, RoutedEventArgs e)
		{
			if (IncentivizeIceCaveCheckBox != null)
			{
				IncentivizeIceCaveCheckBox.IsEnabled = false;
			}
			if (IncentivizeOrdealsCheckBox != null)
			{
				IncentivizeOrdealsCheckBox.IsEnabled = false;
			}
			if (IncentivizeMarshCheckBox != null)
			{
				IncentivizeMarshCheckBox.IsEnabled = false;
			}
			if (IncentivizeEarthCheckBox != null)
			{
				IncentivizeEarthCheckBox.IsEnabled = false;
			}
			if (IncentivizeVolcanoCheckBox != null)
			{
				IncentivizeVolcanoCheckBox.IsEnabled = false;
			}
			if (IncentivizeSeaShrineCheckBox != null)
			{
				IncentivizeSeaShrineCheckBox.IsEnabled = false;
			}
			if (IncentivizeSkyPalaceCheckBox != null)
			{
				IncentivizeSkyPalaceCheckBox.IsEnabled = false;
			}
			if (IncentivizeConeriaCheckBox != null)
			{
				IncentivizeConeriaCheckBox.IsEnabled = false;
			}
			if (IncentivizeMarshKeyLockedCheckBox != null)
			{
				IncentivizeMarshKeyLockedCheckBox.IsEnabled = false;
			}
			if (IncentivizeTailCheckBox != null)
			{
				IncentivizeTailCheckBox.IsEnabled = false;
			}
			if (IncentivizeFetchItemsCheckBox != null)
			{
				IncentivizeFetchItemsCheckBox.IsEnabled = false;
			}
			if (IncentivizeMasamuneCheckBox != null)
			{
				IncentivizeMasamuneCheckBox.IsEnabled = false;
			}
			if (IncentivizeRibbonCheckBox != null)
			{
				IncentivizeRibbonCheckBox.IsEnabled = false;
			}
			if (IncentivizeOpalCheckBox != null)
			{
				IncentivizeOpalCheckBox.IsEnabled = false;
			}
			if (IncentivizeDefCastArmorCheckBox != null)
			{
				IncentivizeDefCastArmorCheckBox.IsEnabled = false;
			}
			if (IncentivizeOtherCastArmorCheckBox != null)
			{
				IncentivizeOtherCastArmorCheckBox.IsEnabled = false;
			}
			if (IncentivizeDefCastWeaponCheckBox != null)
			{
				IncentivizeDefCastWeaponCheckBox.IsEnabled = false;
			}
			if (IncentivizeOffCastWeaponCheckBox != null)
			{
				IncentivizeOffCastWeaponCheckBox.IsEnabled = false;
			}
			if (IncentivizeFreeNPCsCheckBox != null)
			{
				IncentivizeFreeNPCsCheckBox.IsEnabled = false;
			}
			if (IncentivizeFetchNPCsCheckBox != null)
			{
				IncentivizeFetchNPCsCheckBox.IsEnabled = false;
			}

			SetFlagsText(sender, e);
		}

		private void NPCItemsCheckBox_OnChecked(object sender, RoutedEventArgs e)
		{
			if (IncentivizeFreeNPCsCheckBox != null)
			{
				if (TreasuresCheckBox != null)
				{
					IncentivizeFreeNPCsCheckBox.IsEnabled = TreasuresCheckBox.IsEnabled;
				}
				else
				{
					IncentivizeFreeNPCsCheckBox.IsEnabled = false;
				}
			}

			SetFlagsText(sender, e);
		}

		private void NPCItemsCheckBox_OnUnchecked(object sender, RoutedEventArgs e)
		{
			if (IncentivizeFreeNPCsCheckBox != null)
			{
				IncentivizeFreeNPCsCheckBox.IsEnabled = false;
			}

			SetFlagsText(sender, e);
		}

		private void NPCFetchItemsCheckBox_OnChecked(object sender, RoutedEventArgs e)
		{
			if (IncentivizeFetchNPCsCheckBox != null)
			{
				if (TreasuresCheckBox != null)
				{
					IncentivizeFetchNPCsCheckBox.IsEnabled = TreasuresCheckBox.IsEnabled;
				}
				else
				{
					IncentivizeFetchNPCsCheckBox.IsEnabled = false;
				}
			}

			SetFlagsText(sender, e);
		}

		private void NPCFetchItemsCheckBox_OnUnchecked(object sender, RoutedEventArgs e)
		{
			if (IncentivizeFetchNPCsCheckBox != null)
			{
				IncentivizeFetchNPCsCheckBox.IsEnabled = false;
			}

			SetFlagsText(sender, e);
		}

		private void PriceScaleFactorSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
		{
			PriceScaleFactorSlider.Value = Math.Round(PriceScaleFactorSlider.Value, 1);

			SetScaleFactorLabel(PriceScaleFactorSlider, PriceScaleFactorLabel);
			SetFlagsText(sender, e);
		}

		private void EnemyScaleFactorSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
		{
			EnemyScaleFactorSlider.Value = Math.Round(EnemyScaleFactorSlider.Value, 1);

			SetScaleFactorLabel(EnemyScaleFactorSlider, EnemyScaleFactorLabel);
			SetFlagsText(sender, e);
		}

		private void ExpMultiplierSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
		{
			ExpMultiplierSlider.Value = Math.Round(ExpMultiplierSlider.Value, 1);

			SetExpLabel();
			SetFlagsText(sender, e);
		}

		private void ExpBonusSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
		{
			ExpBonusSlider.Value = 10.0 * Math.Round(ExpBonusSlider.Value / 10.0);

			SetExpLabel();
			SetFlagsText(sender, e);
		}

		private void PartyScaleFactorSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
		{
			PartyScaleFactorSlider.Value = Math.Round(PartyScaleFactorSlider.Value);

			SetPartyLabel();
			SetFlagsText(sender, e);
		}

		private void CopyButton_Click(object sender, RoutedEventArgs e)
		{
			Clipboard.SetText(SeedTextBox.Text + "_" + FlagsTextBox.Text);
		}

		private void ChaosRushCheckBox_OnChecked(object sender, RoutedEventArgs e)
		{
			if (ShardHuntCheckBox != null)
			{
				ShardHuntCheckBox.IsEnabled = true;
			}

			if (ExtraShardsCheckBox != null)
			{
				ExtraShardsCheckBox.IsEnabled = (ShardHuntCheckBox != null && ShardHuntCheckBox.IsEnabled && ShardHuntCheckBox.IsChecked.Value);
			}

			SetFlagsText(sender, e);
		}

		private void ChaosRushCheckBox_OnUnchecked(object sender, RoutedEventArgs e)
		{
			if (ShardHuntCheckBox != null)
			{
				ShardHuntCheckBox.IsEnabled = false;
			}

			if (ExtraShardsCheckBox != null)
			{
				ExtraShardsCheckBox.IsEnabled = false;
			}

			SetFlagsText(sender, e);
		}

		private void ShardHuntCheckBox_OnChecked(object sender, RoutedEventArgs e)
		{
			if (ExtraShardsCheckBox != null)
			{
				ExtraShardsCheckBox.IsEnabled = true;
			}

			SetFlagsText(sender, e);
		}

		private void ShardHuntCheckBox_OnUnchecked(object sender, RoutedEventArgs e)
		{
			if (ExtraShardsCheckBox != null)
			{
				ExtraShardsCheckBox.IsEnabled = false;
			}

			SetFlagsText(sender, e);
		}

		private void PasteButton_Click(object sender, RoutedEventArgs e)
		{
			var text = Clipboard.GetText();
			var parts = text.Split('_');
			if (parts.Length != 2 || parts[0].Length != 8 || parts[1].Length != 11)
			{
				MessageBox.Show("Format not recognized.  Paste should look like SSSSSSSS_FFFFFFFFFFF", "Invalid Format");

				return;
			}

			SeedTextBox.Text = parts[0];
			SetSeed();

			ApplyFlags(Flags.DecodeFlagsText(parts[1]));
		}

		private void SetScaleFactorLabel(Slider slider, Label label)
		{
			var lower = Math.Round(100 / slider.Value);
			var upper = Math.Round(100 * slider.Value);

			label.Content = $"{lower}% - {upper}%";
		}

		private void SetExpLabel()
		{
			if (ExpMultiplierSlider != null && ExpBonusSlider != null && ExpScaleFactorLabel != null)
			{
				ExpScaleFactorLabel.Content = $"{ExpMultiplierSlider.Value}x + {ExpBonusSlider.Value}";
			}
		}

		private void SetPartyLabel()
		{
			if (PartyScaleFactorSlider != null)
			{
				PartyScaleFactorLabel.Content = PartyScaleFactorSlider.Value;
			}
		}

		private void SetFlagsText(object sender, RoutedEventArgs e)
		{
			if (!IsInitialized || FlagsTextBox == null)
			{
				return;
			}


			_flags = new Flags {
				Treasures = TreasuresCheckBox.IsChecked == true,
				NPCItems = NPCItemsCheckBox.IsChecked == true,
				NPCFetchItems = NPCFetchItemsCheckBox.IsChecked == true,
				IncentivizeFreeNPCs = IncentivizeFreeNPCsCheckBox.IsChecked == true,
				IncentivizeFetchNPCs = IncentivizeFetchNPCsCheckBox.IsChecked == true,
				IncentivizeIceCave = IncentivizeIceCaveCheckBox.IsChecked == true,
				IncentivizeOrdeals = IncentivizeOrdealsCheckBox.IsChecked == true,
				IncentivizeMarsh = IncentivizeMarshCheckBox.IsChecked == true,
				IncentivizeEarth = IncentivizeEarthCheckBox.IsChecked == true,
				IncentivizeVolcano = IncentivizeVolcanoCheckBox.IsChecked == true,
				IncentivizeSeaShrine = IncentivizeSeaShrineCheckBox.IsChecked == true,
				IncentivizeSkyPalace = IncentivizeSkyPalaceCheckBox.IsChecked == true,
				IncentivizeConeria = IncentivizeConeriaCheckBox.IsChecked == true,
				IncentivizeMarshKeyLocked = IncentivizeMarshKeyLockedCheckBox.IsChecked == true,
				IncentivizeTail = IncentivizeTailCheckBox.IsChecked == true,
				IncentivizeFetchItems = IncentivizeFetchItemsCheckBox.IsChecked == true,
				IncentivizeMasamune = IncentivizeMasamuneCheckBox.IsChecked == true,
				IncentivizeRibbon = IncentivizeRibbonCheckBox.IsChecked == true,
				IncentivizeOpal = IncentivizeOpalCheckBox.IsChecked == true,
				IncentivizeDefCastArmor = IncentivizeDefCastArmorCheckBox.IsChecked == true,
				IncentivizeOtherCastArmor = IncentivizeOtherCastArmorCheckBox.IsChecked == true,
				IncentivizeDefCastWeapon = IncentivizeDefCastWeaponCheckBox.IsChecked == true,
				IncentivizeOffCastWeapon = IncentivizeOffCastWeaponCheckBox.IsChecked == true,
				Shops = ShopsCheckBox.IsChecked == true,
				ShardHunt = ShardHuntCheckBox.IsChecked == true,
				ExtraShards = ExtraShardsCheckBox.IsChecked == true,
				TransformFinalFormation = TransformFinalFormationCheckBox.IsChecked == true,
				MagicShops = MagicShopsCheckBox.IsChecked == true,
				MagicLevels = MagicLevelsCheckBox.IsChecked == true,
				MagicPermissions = MagicPermissionsCheckBox.IsChecked == true,
				Rng = RngCheckBox.IsChecked == true,
				EnemyScripts = EnemyScriptsCheckBox.IsChecked == true,
				EnemySkillsSpells = EnemySkillsSpellsCheckBox.IsChecked == true,
				EnemyStatusAttacks = EnemyStatusAttacksCheckBox.IsChecked == true,
				EnemyFormationsUnrunnable = EnemyFormationsUnrunnableCheckBox.IsChecked == true,
				EnemyFormationsSurprise = EnemyFormationsSurpriseCheckBox.IsChecked == true,
				EnemyFormationsFrequency = EnemyFormationsFrequencyCheckBox.IsChecked == true,
				OrdealsPillars = OrdealsPillarsCheckBox.IsChecked == true,
				SkyCastle4FTeleporters = SkyCastle4FTeleportersCheckBox.IsChecked == true,
				TitansTrove = TitansTroveCheckBox.IsChecked == true,
				ChaosRush = ChaosRushCheckBox.IsChecked == true,
				MapOpenProgression = MapOpenProgressionCheckBox.IsChecked == true,

				EarlySarda = EarlySardaCheckBox.IsChecked == true,
				EarlySage = EarlySageCheckBox.IsChecked == true,
				CrownlessOrdeals = CrownlessOrdealsCheckBox.IsChecked == true,
				FreeAirship = FreeAirshipCheckBox.IsChecked == true,
				FreeBridge = FreeBridgeCheckBox.IsChecked == true,
				FreeOrbs = FreeOrbsCheckBox.IsChecked == true,
				EasyMode = EasyModeCheckBox.IsChecked == true,
				NoPartyShuffle = NoPartyShuffleCheckBox.IsChecked == true,
				SpeedHacks = SpeedHacksCheckBox.IsChecked == true,
				IdentifyTreasures = IdentifyTreasuresCheckBox.IsChecked == true,
				Dash = DashCheckBox.IsChecked == true,
				BuyTen = BuyTenCheckBox.IsChecked == true,
				WaitWhenUnrunnable = WaitWhenUnrunnableCheckBox.IsChecked == true,

				HouseMPRestoration = HouseMPRestorationCheckBox.IsChecked == true,
				WeaponStats = WeaponStatsCheckBox.IsChecked == true,
				ChanceToRun = ChanceToRunCheckBox.IsChecked == true,
				SpellBugs = SpellBugsCheckBox.IsChecked == true,
				EnemyStatusAttackBug = EnemyStatusAttackBugCheckBox.IsChecked == true,
				BlackBeltAbsorb = BlackBeltAbsorbCheckBox.IsChecked == true,
				EnemySpellsTargetingAllies = EnemySpellsTargetingAlliesCheckBox.IsChecked == true,
				EnemyElementalResistancesBug = EnemyElementalResistancesBugCheckBox.IsChecked == true,

				FunEnemyNames = FunEnemyNamesCheckBox.IsChecked == true,
				PaletteSwap = PaletteSwapCheckBox.IsChecked == true,
				ModernBattlefield = ModernBattlefieldCheckBox.IsChecked == true,
				TeamSteak = TeamTyroComboBox.SelectedValue.ToString() == "Team STEAK",
				Music =
					MusicComboBox.SelectedValue.ToString() == "Standard Music Shuffle" ? MusicShuffle.Standard :
					MusicComboBox.SelectedValue.ToString() == "Nonsensical Music Shuffle" ? MusicShuffle.Nonsensical :
					MusicComboBox.SelectedValue.ToString() == "Disable Music" ? MusicShuffle.MusicDisabled :
					MusicShuffle.None,

				PriceScaleFactor = PriceScaleFactorSlider.Value,
				EnemyScaleFactor = EnemyScaleFactorSlider.Value,
				ExpMultiplier = ExpMultiplierSlider.Value,
				ExpBonus = (int)ExpBonusSlider.Value,
				ForcedPartyMembers = (int)PartyScaleFactorSlider.Value
			};

			FlagsTextBox.Text = Flags.EncodeFlagsText(_flags);
		}

		void ApplyFlags(Flags flags)
		{
			TreasuresCheckBox.IsChecked = flags.Treasures;
			NPCItemsCheckBox.IsChecked = flags.NPCItems;
			NPCFetchItemsCheckBox.IsChecked = flags.NPCFetchItems;
			IncentivizeFreeNPCsCheckBox.IsChecked = flags.IncentivizeFreeNPCs;
			IncentivizeFetchNPCsCheckBox.IsChecked = flags.IncentivizeFetchNPCs;
			IncentivizeIceCaveCheckBox.IsChecked = flags.IncentivizeIceCave;
			IncentivizeOrdealsCheckBox.IsChecked = flags.IncentivizeOrdeals;
			IncentivizeMarshCheckBox.IsChecked = flags.IncentivizeMarsh;
			IncentivizeEarthCheckBox.IsChecked = flags.IncentivizeEarth;
			IncentivizeVolcanoCheckBox.IsChecked = flags.IncentivizeVolcano;
			IncentivizeSeaShrineCheckBox.IsChecked = flags.IncentivizeSeaShrine;
			IncentivizeSkyPalaceCheckBox.IsChecked = flags.IncentivizeSkyPalace;
			IncentivizeConeriaCheckBox.IsChecked = flags.IncentivizeConeria;
			IncentivizeMarshKeyLockedCheckBox.IsChecked = flags.IncentivizeMarshKeyLocked;
			IncentivizeTailCheckBox.IsChecked = flags.IncentivizeTail;
			IncentivizeFetchItemsCheckBox.IsChecked = flags.IncentivizeFetchItems;
			IncentivizeMasamuneCheckBox.IsChecked = flags.IncentivizeMasamune;
			IncentivizeRibbonCheckBox.IsChecked = flags.IncentivizeRibbon;
			IncentivizeOpalCheckBox.IsChecked = flags.IncentivizeOpal;
			IncentivizeDefCastArmorCheckBox.IsChecked = flags.IncentivizeDefCastArmor;
			IncentivizeOtherCastArmorCheckBox.IsChecked = flags.IncentivizeOtherCastArmor;
			IncentivizeDefCastWeaponCheckBox.IsChecked = flags.IncentivizeDefCastWeapon;
			IncentivizeOffCastWeaponCheckBox.IsChecked = flags.IncentivizeOffCastWeapon;
			ShopsCheckBox.IsChecked = flags.Shops;
			ShardHuntCheckBox.IsChecked = flags.ShardHunt;
			ExtraShardsCheckBox.IsChecked = flags.ExtraShards;
			TransformFinalFormationCheckBox.IsChecked = flags.TransformFinalFormation;
			MagicShopsCheckBox.IsChecked = flags.MagicShops;
			MagicLevelsCheckBox.IsChecked = flags.MagicLevels;
			MagicPermissionsCheckBox.IsChecked = flags.MagicPermissions;
			RngCheckBox.IsChecked = flags.Rng;
			EnemyScriptsCheckBox.IsChecked = flags.EnemyScripts;
			EnemySkillsSpellsCheckBox.IsChecked = flags.EnemySkillsSpells;
			EnemyStatusAttacksCheckBox.IsChecked = flags.EnemyStatusAttacks;
			EnemyFormationsUnrunnableCheckBox.IsChecked = flags.EnemyFormationsUnrunnable;
			EnemyFormationsSurpriseCheckBox.IsChecked = flags.EnemyFormationsSurprise;
			EnemyFormationsFrequencyCheckBox.IsChecked = flags.EnemyFormationsFrequency;
			OrdealsPillarsCheckBox.IsChecked = flags.OrdealsPillars;
			SkyCastle4FTeleportersCheckBox.IsChecked = flags.SkyCastle4FTeleporters;
			TitansTroveCheckBox.IsChecked = flags.TitansTrove;
			ChaosRushCheckBox.IsChecked = flags.ChaosRush;
			MapOpenProgressionCheckBox.IsChecked = flags.MapOpenProgression;

			EarlySardaCheckBox.IsChecked = flags.EarlySarda;
			EarlySageCheckBox.IsChecked = flags.EarlySage;
			CrownlessOrdealsCheckBox.IsChecked = flags.CrownlessOrdeals;
			FreeAirshipCheckBox.IsChecked = flags.FreeAirship;
			FreeBridgeCheckBox.IsChecked = flags.FreeBridge;
			FreeOrbsCheckBox.IsChecked = flags.FreeOrbs;
			EasyModeCheckBox.IsChecked = flags.EasyMode;
			NoPartyShuffleCheckBox.IsChecked = flags.NoPartyShuffle;
			SpeedHacksCheckBox.IsChecked = flags.SpeedHacks;
			IdentifyTreasuresCheckBox.IsChecked = flags.IdentifyTreasures;
			DashCheckBox.IsChecked = flags.Dash;
			BuyTenCheckBox.IsChecked = flags.BuyTen;
			WaitWhenUnrunnableCheckBox.IsChecked = flags.WaitWhenUnrunnable;

			HouseMPRestorationCheckBox.IsChecked = flags.HouseMPRestoration;
			WeaponStatsCheckBox.IsChecked = flags.WeaponStats;
			ChanceToRunCheckBox.IsChecked = flags.ChanceToRun;
			SpellBugsCheckBox.IsChecked = flags.SpellBugs;
			EnemyStatusAttackBugCheckBox.IsChecked = flags.EnemyStatusAttackBug;
			BlackBeltAbsorbCheckBox.IsChecked = flags.BlackBeltAbsorb;
			EnemySpellsTargetingAlliesCheckBox.IsChecked = flags.EnemySpellsTargetingAllies;
			EnemyElementalResistancesBugCheckBox.IsChecked = flags.EnemyElementalResistancesBug;

			FunEnemyNamesCheckBox.IsChecked = flags.FunEnemyNames;
			PaletteSwapCheckBox.IsChecked = flags.PaletteSwap;
			ModernBattlefieldCheckBox.IsChecked = flags.ModernBattlefield;
			TeamTyroComboBox.SelectedValue = flags.TeamSteak ? "Team STEAK" : "Team TYRO";
			MusicComboBox.SelectedValue =
				flags.Music == MusicShuffle.Standard ? "Standard Music Shuffle" :
				flags.Music == MusicShuffle.Nonsensical ? "Nonsensical Music Shuffle" :
				flags.Music == MusicShuffle.MusicDisabled ? "Disable Music" :
				"No Music Shuffle";

			PriceScaleFactorSlider.Value = flags.PriceScaleFactor;
			EnemyScaleFactorSlider.Value = flags.EnemyScaleFactor;
			ExpMultiplierSlider.Value = flags.ExpMultiplier;
			ExpBonusSlider.Value = flags.ExpBonus;
			PartyScaleFactorSlider.Value = flags.ForcedPartyMembers;
		}

		private void AboutButton_Click(object sender, RoutedEventArgs e)
		{
			var aboutWindow = new AboutWindow(FF1Rom.Version) { Owner = this };

			aboutWindow.ShowDialog();
		}
	}
}
