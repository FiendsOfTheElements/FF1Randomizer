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

		public const string Version = "1.0.4";

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
				rom.SpeedHacks();
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
			if (parts.Length != 2 || parts[0].Length != 8 || parts[1].Length != 7)
			{
				MessageBox.Show("Format not recognized.  Paste should look like SSSSSSSS_FFFFFFF", "Invalid Format");

				return;
			}

			SeedTextBox.Text = parts[0];
			SetSeed();

			DecodeFlagsText(parts[1]);
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

			var bits = new BitArray(13);
			bits[0] = TreasuresCheckBox.IsChecked == true;
			bits[1] = ShopsCheckBox.IsChecked == true;
			bits[2] = MagicShopsCheckBox.IsChecked == true;
			bits[3] = MagicLevelsCheckBox.IsChecked == true;
			bits[4] = MagicPermissionsCheckBox.IsChecked == true;
			bits[5] = RngCheckBox.IsChecked == true;
			bits[6] = EnemyScriptsCheckBox.IsChecked == true;
			bits[7] = EnemySkillsSpellsCheckBox.IsChecked == true;
			bits[8] = EnemyStatusAttacksCheckBox.IsChecked == true;

			bits[9] = EarlyRodCheckBox.IsChecked == true;
			bits[10] = EarlyCanoeCheckBox.IsChecked == true;
			bits[11] = NoPartyShuffleCheckBox.IsChecked == true;
			bits[12] = SpeedHacksCheckBox.IsChecked == true;

			var bytes = new byte[2];
			bits.CopyTo(bytes, 0);

			FlagsTextBox.Text = Convert.ToBase64String(bytes);
			FlagsTextBox.Text = FlagsTextBox.Text.TrimEnd('=');
			FlagsTextBox.Text = FlagsTextBox.Text.Replace('+', '!');
			FlagsTextBox.Text = FlagsTextBox.Text.Replace('/', '%');

			FlagsTextBox.Text += SliderToBase64((int)(10*PriceScaleFactorSlider.Value));
			FlagsTextBox.Text += SliderToBase64((int)(10*EnemyScaleFactorSlider.Value));
			FlagsTextBox.Text += SliderToBase64((int)(10*ExpMultiplierSlider.Value));
			FlagsTextBox.Text += SliderToBase64((int)ExpBonusSlider.Value);
		}

		private void DecodeFlagsText(string text)
		{
			var bitString = text.Substring(0, 3);
			bitString += '=';
			bitString = bitString.Replace('!', '+');
			bitString = bitString.Replace('%', '/');

			var bytes = Convert.FromBase64String(bitString);
			var bits = new BitArray(bytes);

			TreasuresCheckBox.IsChecked = bits[0];
			ShopsCheckBox.IsChecked = bits[1];
			MagicShopsCheckBox.IsChecked = bits[2];
			MagicLevelsCheckBox.IsChecked = bits[3];
			MagicPermissionsCheckBox.IsChecked = bits[4];
			RngCheckBox.IsChecked = bits[5];
			EnemyScriptsCheckBox.IsChecked = bits[6];
			EnemySkillsSpellsCheckBox.IsChecked = bits[7];
			EnemyStatusAttacksCheckBox.IsChecked = bits[8];

			EarlyRodCheckBox.IsChecked = bits[9];
			EarlyCanoeCheckBox.IsChecked = bits[10];
			NoPartyShuffleCheckBox.IsChecked = bits[11];
			SpeedHacksCheckBox.IsChecked = bits[12];

			PriceScaleFactorSlider.Value = Base64ToSlider(text[3])/10.0;
			EnemyScaleFactorSlider.Value = Base64ToSlider(text[4])/10.0;
			ExpMultiplierSlider.Value = Base64ToSlider(text[5])/10.0;
			ExpBonusSlider.Value = Base64ToSlider(text[6]);
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
