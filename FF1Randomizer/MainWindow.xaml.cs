using System;
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

		public MainWindow()
		{
			InitializeComponent();

			GenerateSeed();

			SetScaleFactorLabel();
			SetExpLabel();
			SetFlagsText(null, null);
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
				var randomizer = new FF1Rom(openFileDialog.FileName);
				if (!randomizer.Validate())
				{
					MessageBox.Show("ROM does not appear to be valid.  Proceed at your own risk.", "Validation Error");
				}

				_filename = openFileDialog.FileName;
				RomTextBox.Text = openFileDialog.SafeFileName;
				GenerateButton.IsEnabled = true;
			}
		}

		private void SeedButton_Click(object sender, RoutedEventArgs e)
		{
			GenerateSeed();
		}

		private void SeedTextBox_LostFocus(object sender, RoutedEventArgs e)
		{
			SeedTextBox.Text = SeedTextBox.Text.Trim();

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
				rom.ShuffleTreasures(rng);
			}

			if (ShopsCheckBox.IsChecked == true)
			{
				rom.ShuffleShops(rng);
			}

			if (MagicShopsCheckBox.IsChecked == true)
			{
				rom.ShuffleMagicShops(rng);
			}

			if (MagicLevelsCheckBox.IsChecked == true)
			{
				rom.ShuffleMagicLevels(rng, MagicPermissionsCheckBox.IsChecked ?? false);
			}

			if (EnemyStatsCheckBox.IsChecked == true)
			{
				rom.ScaleEnemyStats(ScaleFactorSlider.Value, rng);
			}

			if (ExpGoldBoostCheckBox.IsChecked == true)
			{
				rom.ExpGoldBoost(ExpBonusSlider.Value*10, ExpMultiplierSlider.Value);
			}

			var seedText = _seed.ToHex();
			rom.WriteSeedAndFlags(seedText, FlagsTextBox.Text);

			var fileRoot = _filename.Substring(0, _filename.LastIndexOf("."));
			var outputFilename = $"{fileRoot}_{FlagsTextBox.Text}_{seedText}.nes";
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

		private void ScaleFactorSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
		{
			ScaleFactorSlider.Value = Math.Round(ScaleFactorSlider.Value, 1);

			SetScaleFactorLabel();
			SetFlagsText(sender, e);
		}

		private void SetScaleFactorLabel()
		{
			var lower = Math.Round(100 / ScaleFactorSlider.Value);
			var upper = Math.Round(100 * ScaleFactorSlider.Value);

			ScaleFactorLabel.Content = $"{lower}% - {upper}%";
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

		private void SetExpLabel()
		{
			if (ExpMultiplierSlider != null && ExpBonusSlider != null && ExpLabel != null)
			{
				ExpLabel.Content = $"{ExpMultiplierSlider.Value}x + {ExpBonusSlider.Value*10}";
			}
		}

		private void SetFlagsText(object sender, RoutedEventArgs e)
		{
			if (FlagsTextBox == null)
			{
				return;
			}

			FlagsTextBox.Text = "";

			FlagsTextBox.Text += TreasuresCheckBox.IsChecked == true ? "T" : "t";
			FlagsTextBox.Text += ShopsCheckBox.IsChecked == true ? "S" : "s";
			FlagsTextBox.Text += MagicShopsCheckBox.IsChecked == true ? "M" : "m";
			FlagsTextBox.Text += MagicLevelsCheckBox.IsChecked == true ? "L" : "l";
			FlagsTextBox.Text += MagicPermissionsCheckBox.IsChecked == true ? "P" : "p";

			if (EnemyStatsCheckBox.IsChecked == true || PricesCheckBox.IsChecked == true)
			{
				if (EnemyStatsCheckBox.IsChecked == true)
				{
					FlagsTextBox.Text += "S";
				}
				if (PricesCheckBox.IsChecked == true)
				{
					FlagsTextBox.Text += "P";
				}

				FlagsTextBox.Text += SliderToBase64((int)(10*ScaleFactorSlider.Value));
			}

			if (ExpGoldBoostCheckBox.IsChecked == true)
			{
				FlagsTextBox.Text += "B";
				FlagsTextBox.Text += SliderToBase64((int)(10*ExpMultiplierSlider.Value));
				FlagsTextBox.Text += SliderToBase64((int)ExpBonusSlider.Value);
			}
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
				return '?';
			}
		}
	}
}
