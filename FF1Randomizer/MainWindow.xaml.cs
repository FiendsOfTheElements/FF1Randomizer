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

		public const string Version = "1.2.0";

		private class MainWindowViewModel
		{
			public string WindowTitle => $"FF1 Randomizer {Version}";
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
			var rng = new MT19337(BitConverter.ToUInt32(_seed, 0));

			rom.EasterEggs();

			if (TreasuresCheckBox.IsChecked == true)
			{
				rom.ShuffleTreasures(rng, EarlyCanoeCheckBox.IsChecked == true);
			}

			if (ShopsCheckBox.IsChecked == true)
			{
				rom.ShuffleShops(rng, EnemyStatusAttacksCheckBox.IsChecked == true);
			}

			if (MagicShopsCheckBox.IsChecked == true)
			{
				rom.ShuffleMagicShops(rng);
			}

			if (MagicLevelsCheckBox.IsChecked == true)
			{
				rom.ShuffleMagicLevels(rng, MagicPermissionsCheckBox.IsChecked ?? false);
			}

			if (RngCheckBox.IsChecked == true)
			{
				rom.ShuffleRng(rng);
			}

			if (EnemyScriptsCheckBox.IsChecked == true)
			{
				rom.ShuffleEnemyScripts(rng);
			}

			if (EnemySkillsSpellsCheckBox.IsChecked == true)
			{
				rom.ShuffleEnemySkillsSpells(rng);
			}

			if (EnemyStatusAttacksCheckBox.IsChecked == true)
			{
				rom.ShuffleEnemyStatusAttacks(rng);
			}

			if (EarlyRodCheckBox.IsChecked == true)
			{
				rom.EnableEarlyRod();
			}

			if (EarlyCanoeCheckBox.IsChecked == true)
			{
				rom.EnableEarlyCanoe();
			}

			if (NoPartyShuffleCheckBox.IsChecked == true)
			{
				rom.DisablePartyShuffle();
			}

			if (SpeedHacksCheckBox.IsChecked == true)
			{
				rom.EnableSpeedHacks();
			}

			if (IdentifyTreasuresCheckBox.IsChecked == true)
			{
				rom.EnableIdentifyTreasures();
			}

			if (DashCheckBox.IsChecked == true)
			{
				rom.EnableDash();
			}

			if (BuyTenCheckBox.IsChecked == true)
			{
				rom.EnableBuyTen();
			}

			if (HouseMPRestorationCheckBox.IsChecked == true)
			{
				rom.FixHouse();
			}

            if (WeaponStatsCheckBox.IsChecked == true)
            {
                rom.FixWeaponStats();
            }

			if (PriceScaleFactorSlider.Value > 1)
			{
				rom.ScalePrices(PriceScaleFactorSlider.Value, rng);
			}

			if (EnemyScaleFactorSlider.Value > 1)
			{
				rom.ScaleEnemyStats(EnemyScaleFactorSlider.Value, rng);
			}

			if (ExpMultiplierSlider.Value > 1 || ExpBonusSlider.Value > 0)
			{
				rom.ExpGoldBoost(ExpBonusSlider.Value*10, ExpMultiplierSlider.Value);
			}

			var seedText = _seed.ToHex();
			rom.WriteSeedAndFlags(Version, seedText, FlagsTextBox.Text);

			var fileRoot = _filename.Substring(0, _filename.LastIndexOf("."));
			var outputFilename = $"{fileRoot}_{seedText}_{FlagsTextBox.Text}.nes";
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
			ExpBonusSlider.Value = Math.Round(ExpBonusSlider.Value);

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
			if (parts.Length != 2 || parts[0].Length != 8 || parts[1].Length != 8)
			{
				MessageBox.Show("Format not recognized.  Paste should look like SSSSSSSS_FFFFFFFF", "Invalid Format");

				return;
			}

			SeedTextBox.Text = parts[0];
			SetSeed();

			ApplyFlags(DecodeFlagsText(parts[1]));
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
				ExpScaleFactorLabel.Content = $"{ExpMultiplierSlider.Value}x + {ExpBonusSlider.Value*10}";
			}
		}

		private void SetFlagsText(object sender, RoutedEventArgs e)
        {
            if (!IsInitialized || FlagsTextBox == null)
            {
                return;
            }

            FlagsTextBox.Text = EncodeFlagsText(new Flags
			{
				Treasures = TreasuresCheckBox.IsChecked == true,
				Shops = ShopsCheckBox.IsChecked == true,
				MagicShops = MagicShopsCheckBox.IsChecked == true,
				MagicLevels = MagicLevelsCheckBox.IsChecked == true,
				MagicPermissions = MagicPermissionsCheckBox.IsChecked == true,
				Rng = RngCheckBox.IsChecked == true,
				EnemyScripts = EnemyScriptsCheckBox.IsChecked == true,
				EnemySkillsSpells = EnemySkillsSpellsCheckBox.IsChecked == true,
				EnemyStatusAttacks = EnemyStatusAttacksCheckBox.IsChecked == true,

				EarlyRod = EarlyRodCheckBox.IsChecked == true,
				EarlyCanoe = EarlyCanoeCheckBox.IsChecked == true,
				NoPartyShuffle = NoPartyShuffleCheckBox.IsChecked == true,
				SpeedHacks = SpeedHacksCheckBox.IsChecked == true,
				IdentifyTreasures = IdentifyTreasuresCheckBox.IsChecked == true,
				Dash = DashCheckBox.IsChecked == true,
				BuyTen = BuyTenCheckBox.IsChecked == true,

				HouseMPRestoration = HouseMPRestorationCheckBox.IsChecked == true,
				WeaponStats = WeaponStatsCheckBox.IsChecked == true,

				PriceScaleFactor = PriceScaleFactorSlider.Value,
				EnemyScaleFactor = EnemyScaleFactorSlider.Value,
				ExpMultiplier = ExpMultiplierSlider.Value,
				ExpBonus = ExpBonusSlider.Value
			});
        }

		void ApplyFlags(Flags flags)
		{
			TreasuresCheckBox.IsChecked = flags.Treasures;
			ShopsCheckBox.IsChecked = flags.Shops;
			MagicShopsCheckBox.IsChecked = flags.MagicShops;
			MagicLevelsCheckBox.IsChecked = flags.MagicLevels;
			MagicPermissionsCheckBox.IsChecked = flags.MagicPermissions;
			RngCheckBox.IsChecked = flags.Rng;
			EnemyScriptsCheckBox.IsChecked = flags.EnemyScripts;
			EnemySkillsSpellsCheckBox.IsChecked = flags.EnemySkillsSpells;
			EnemyStatusAttacksCheckBox.IsChecked = flags.EnemyStatusAttacks;

			EarlyRodCheckBox.IsChecked = flags.EarlyRod;
			EarlyCanoeCheckBox.IsChecked = flags.EarlyCanoe;
			NoPartyShuffleCheckBox.IsChecked = flags.NoPartyShuffle;
			SpeedHacksCheckBox.IsChecked = flags.SpeedHacks;
			IdentifyTreasuresCheckBox.IsChecked = flags.IdentifyTreasures;
			DashCheckBox.IsChecked = flags.Dash;
			BuyTenCheckBox.IsChecked = flags.BuyTen;

			HouseMPRestorationCheckBox.IsChecked = flags.HouseMPRestoration;
			WeaponStatsCheckBox.IsChecked = flags.WeaponStats;

			PriceScaleFactorSlider.Value = flags.PriceScaleFactor;
			EnemyScaleFactorSlider.Value = flags.EnemyScaleFactor;
			ExpMultiplierSlider.Value = flags.ExpMultiplier;
			ExpBonusSlider.Value = flags.ExpBonus;
		}

		private string EncodeFlagsText(Flags flags)
        {
            var bits = new BitArray(18);

            bits[0] = flags.Treasures;
            bits[1] = flags.Shops;
            bits[2] = flags.MagicShops;
            bits[3] = flags.MagicLevels;
            bits[4] = flags.MagicPermissions;
            bits[5] = flags.Rng;
            bits[6] = flags.EnemyScripts;
            bits[7] = flags.EnemySkillsSpells;
            bits[8] = flags.EnemyStatusAttacks;

            bits[9] = flags.EarlyRod;
            bits[10] = flags.EarlyCanoe;
            bits[11] = flags.NoPartyShuffle;
            bits[12] = flags.SpeedHacks;
            bits[13] = flags.IdentifyTreasures;
            bits[14] = flags.Dash;
            bits[15] = flags.BuyTen;

            bits[16] = flags.HouseMPRestoration;
            bits[17] = flags.WeaponStats;

            var bytes = new byte[3];
            bits.CopyTo(bytes, 0);

            var text = Convert.ToBase64String(bytes);
            text = text.TrimEnd('=');
            text = text.Replace('+', '!');
            text = text.Replace('/', '%');

            text += SliderToBase64((int)(10 * flags.PriceScaleFactor));
            text += SliderToBase64((int)(10 * flags.EnemyScaleFactor));
            text += SliderToBase64((int)(10 * flags.ExpMultiplier));
            text += SliderToBase64((int)flags.ExpBonus);

            return text;
        }

        private Flags DecodeFlagsText(string text)
		{
			var bitString = text.Substring(0, 4);
			bitString = bitString.Replace('!', '+');
			bitString = bitString.Replace('%', '/');

			var bytes = Convert.FromBase64String(bitString);
			var bits = new BitArray(bytes);

			return new Flags
			{
				Treasures = bits[0],
				Shops = bits[1],
				MagicShops = bits[2],
				MagicLevels = bits[3],
				MagicPermissions = bits[4],
				Rng = bits[5],
				EnemyScripts = bits[6],
				EnemySkillsSpells = bits[7],
				EnemyStatusAttacks = bits[8],

				EarlyRod = bits[9],
				EarlyCanoe = bits[10],
				NoPartyShuffle = bits[11],
				SpeedHacks = bits[12],
				IdentifyTreasures = bits[13],
				Dash = bits[14],
				BuyTen = bits[15],

				HouseMPRestoration = bits[16],
				WeaponStats = bits[17],

				PriceScaleFactor = Base64ToSlider(text[4]) / 10.0,
				EnemyScaleFactor = Base64ToSlider(text[5]) / 10.0,
				ExpMultiplier = Base64ToSlider(text[6]) / 10.0,
				ExpBonus = Base64ToSlider(text[7])
			};
		}

		private char SliderToBase64(int value)
		{
			if (value < 0 || value > 63)
			{
				throw new ArgumentOutOfRangeException(nameof(value), value, "Value must be between 0 and 63.");
			}
			else if (value < 10)
			{
				return (char)('0' + value);
			}
			else if (value < 36)
			{
				return (char)('A' + value - 10);
			}
			else if (value < 62)
			{
				return (char)('a' + value - 36);
			}
			else if (value == 62)
			{
				return '!';
			}
			else
			{
				return '%';
			}
		}

		private int Base64ToSlider(char value)
		{
			if (value >= '0' && value <= '9')
			{
				return value - '0';
			}
			else if (value >= 'A' && value <= 'Z')
			{
				return value - 'A' + 10;
			}
			else if (value >= 'a' && value <= 'z')
			{
				return value - 'a' + 36;
			}
			else if (value == '!')
			{
				return 62;
			}
			else
			{
				return 63;
			}
		}

		private void AboutButton_Click(object sender, RoutedEventArgs e)
		{
			var aboutWindow = new AboutWindow(Version) { Owner = this };

			aboutWindow.ShowDialog();
		}
	}
}
