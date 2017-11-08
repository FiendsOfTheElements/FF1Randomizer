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
			var openFileDialog = new OpenFileDialog
			{
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
			rom.Randomize(_seed, FF1Rom.DecodeFlagsText(FlagsTextBox.Text));

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

		private void CopyButton_Click(object sender, RoutedEventArgs e)
		{
			Clipboard.SetText(SeedTextBox.Text + "_" + FlagsTextBox.Text);
		}

		private void PasteButton_Click(object sender, RoutedEventArgs e)
		{
			var text = Clipboard.GetText();
			var parts = text.Split('_');
			if (parts.Length != 2 || parts[0].Length != 8 || parts[1].Length != 10)
			{
				MessageBox.Show("Format not recognized.  Paste should look like SSSSSSSS_FFFFFFFFFF", "Invalid Format");

				return;
			}

			SeedTextBox.Text = parts[0];
			SetSeed();

			ApplyFlags(FF1Rom.DecodeFlagsText(parts[1]));
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

		private void SetFlagsText(object sender, RoutedEventArgs e)
        {
            if (!IsInitialized || FlagsTextBox == null)
            {
                return;
            }

            FlagsTextBox.Text = FF1Rom.EncodeFlagsText(new Flags
			{
				Treasures = TreasuresCheckBox.IsChecked == true,
				IncentivizeIceCave = IncentivizeIceCaveCheckBox.IsChecked == true,
				IncentivizeOrdeals = IncentivizeOrdealsCheckBox.IsChecked == true,
				Shops = ShopsCheckBox.IsChecked == true,
				MagicShops = MagicShopsCheckBox.IsChecked == true,
				MagicLevels = MagicLevelsCheckBox.IsChecked == true,
				MagicPermissions = MagicPermissionsCheckBox.IsChecked == true,
				Rng = RngCheckBox.IsChecked == true,
				EnemyScripts = EnemyScriptsCheckBox.IsChecked == true,
				EnemySkillsSpells = EnemySkillsSpellsCheckBox.IsChecked == true,
				EnemyStatusAttacks = EnemyStatusAttacksCheckBox.IsChecked == true,
				Ordeals = OrdealsCheckBox.IsChecked == true,

				EarlyRod = EarlyRodCheckBox.IsChecked == true,
				EarlyCanoe = EarlyCanoeCheckBox.IsChecked == true,
				EarlyOrdeals = EarlyOrdealsCheckBox.IsChecked == true,
				NoPartyShuffle = NoPartyShuffleCheckBox.IsChecked == true,
				SpeedHacks = SpeedHacksCheckBox.IsChecked == true,
				IdentifyTreasures = IdentifyTreasuresCheckBox.IsChecked == true,
				Dash = DashCheckBox.IsChecked == true,
				BuyTen = BuyTenCheckBox.IsChecked == true,

				HouseMPRestoration = HouseMPRestorationCheckBox.IsChecked == true,
				WeaponStats = WeaponStatsCheckBox.IsChecked == true,
				ChanceToRun = ChanceToRunCheckBox.IsChecked == true,
				SpellBugs = SpellBugsCheckBox.IsChecked == true,
				EnemyStatusAttackBug = EnemyStatusAttacksBugCheckBox.IsChecked == true,

				FunEnemyNames = FunEnemyNamesCheckBox.IsChecked == true,
				PaletteSwap = PaletteSwapCheckBox.IsChecked == true,
				TeamSteak = TeamTyroComboBox.SelectedValue.ToString() == "Team STEAK",

				PriceScaleFactor = PriceScaleFactorSlider.Value,
				EnemyScaleFactor = EnemyScaleFactorSlider.Value,
				ExpMultiplier = ExpMultiplierSlider.Value,
				ExpBonus = ExpBonusSlider.Value
			});
        }

		void ApplyFlags(Flags flags)
		{
			TreasuresCheckBox.IsChecked = flags.Treasures;
			IncentivizeIceCaveCheckBox.IsChecked = flags.IncentivizeIceCave;
			IncentivizeOrdealsCheckBox.IsChecked = flags.IncentivizeOrdeals;
			ShopsCheckBox.IsChecked = flags.Shops;
			MagicShopsCheckBox.IsChecked = flags.MagicShops;
			MagicLevelsCheckBox.IsChecked = flags.MagicLevels;
			MagicPermissionsCheckBox.IsChecked = flags.MagicPermissions;
			RngCheckBox.IsChecked = flags.Rng;
			EnemyScriptsCheckBox.IsChecked = flags.EnemyScripts;
			EnemySkillsSpellsCheckBox.IsChecked = flags.EnemySkillsSpells;
			EnemyStatusAttacksCheckBox.IsChecked = flags.EnemyStatusAttacks;
			OrdealsCheckBox.IsChecked = flags.Ordeals;

			EarlyRodCheckBox.IsChecked = flags.EarlyRod;
			EarlyCanoeCheckBox.IsChecked = flags.EarlyCanoe;
			EarlyOrdealsCheckBox.IsChecked = flags.EarlyOrdeals;
			NoPartyShuffleCheckBox.IsChecked = flags.NoPartyShuffle;
			SpeedHacksCheckBox.IsChecked = flags.SpeedHacks;
			IdentifyTreasuresCheckBox.IsChecked = flags.IdentifyTreasures;
			DashCheckBox.IsChecked = flags.Dash;
			BuyTenCheckBox.IsChecked = flags.BuyTen;

			HouseMPRestorationCheckBox.IsChecked = flags.HouseMPRestoration;
			WeaponStatsCheckBox.IsChecked = flags.WeaponStats;
			ChanceToRunCheckBox.IsChecked = flags.ChanceToRun;
			SpellBugsCheckBox.IsChecked = flags.SpellBugs;
			EnemyStatusAttacksBugCheckBox.IsChecked = flags.EnemyStatusAttackBug;

			FunEnemyNamesCheckBox.IsChecked = flags.FunEnemyNames;
			PaletteSwapCheckBox.IsChecked = flags.PaletteSwap;
			TeamTyroComboBox.SelectedValue = flags.TeamSteak ? "Team STEAK" : "Team TYRO";

			PriceScaleFactorSlider.Value = flags.PriceScaleFactor;
			EnemyScaleFactorSlider.Value = flags.EnemyScaleFactor;
			ExpMultiplierSlider.Value = flags.ExpMultiplier;
			ExpBonusSlider.Value = flags.ExpBonus;
		}

		private void AboutButton_Click(object sender, RoutedEventArgs e)
		{
			var aboutWindow = new AboutWindow(FF1Rom.Version) { Owner = this };

			aboutWindow.ShowDialog();
		}
	}
}
