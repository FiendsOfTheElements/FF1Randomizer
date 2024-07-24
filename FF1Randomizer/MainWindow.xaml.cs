using FF1Lib;
using Microsoft.Win32;
using RomUtilities;
using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Threading.Tasks;

namespace FF1Randomizer
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		private readonly MainWindowViewModel _model;

		public MainWindow()
		{
			InitializeComponent();

			_model = new MainWindowViewModel();
			_model.Flags = Flags.FromJson(File.ReadAllText("presets/default.json")).flags;
			DataContext = _model;

			TryOpenSavedFilename();
			GenerateSeed();

			SetScaleFactorLabel(PriceScaleFactorSlider, PriceScaleFactorLabel, ClampPricesCheckBox);
			SetScaleFactorLabel(EnemyScaleFactorSlider, EnemyScaleFactorLabel, ClampEnemiesCheckBox);
			SetScaleFactorLabel(BossScaleFactorSlider, BossScaleFactorLabel, ClampBossesCheckBox);
			SetExpLabel();
		}

		private void TryOpenSavedFilename()
		{
			if (String.IsNullOrEmpty(Settings.Default.RomFilename))
			{
				return;
			}

			if (!File.Exists(Settings.Default.RomFilename))
			{
				Settings.Default.RomFilename = null;
				Settings.Default.Save();

				return;
			}

			ValidateRom(Settings.Default.RomFilename);
		}

		private void GenerateSeed()
		{
			_model.Seed = Blob.Random(4).ToHex();
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

				Settings.Default.RomFilename = _model.Filename;
				Settings.Default.Save();
			}
		}

		private void ValidateRom(string filename)
		{
			var rom = new FF1Rom(filename);
			if (!rom.Validate())
			{
				MessageBox.Show("ROM does not appear to be valid.  Proceed at your own risk.", "Validation Error");
			}

			_model.Filename = filename;
			var slashIndex = filename.LastIndexOfAny(new[] { '/', '\\' });
			RomTextBox.Text = filename.Substring(slashIndex + 1);
			GenerateButton.IsEnabled = true;
		}

		private void SeedButton_Click(object sender, RoutedEventArgs e)
		{
			GenerateSeed();
		}

		private async void GenerateButton_Click(object sender, RoutedEventArgs e)
		{
			var rom = new FF1Rom(_model.Filename);
			await rom.Randomize(Blob.FromHex(_model.Seed), _model.Flags, _model.Preferences);

			var fileRoot = _model.Filename.Substring(0, _model.Filename.LastIndexOf("."));
			var outputFilename = $"{fileRoot}_{_model.Seed}_{FlagsTextBox.Text}.nes";
			rom.Save(outputFilename);

			MessageBox.Show($"Finished generating new ROM: {outputFilename}", "Done");
		}

		private void CopyButton_Click(object sender, RoutedEventArgs e)
		{
			string export = $"http://finalfantasyrandomizer.com/Randomize?s={SeedTextBox.Text}&f={FlagsTextBox.Text}";
			Clipboard.SetText(export);
			MessageBox.Show($"Copied URL {export} to system clipboard.");
		}

		private void PasteButton_Click(object sender, RoutedEventArgs e)
		{
			var text = Clipboard.GetText();
			var parts = text.Split('=');
			if (parts.Length != 3 || parts[1].Length < 8 || parts[2].Length < 32)
			{
				MessageBox.Show("Format not recognized.  Paste should look like (url)?s=SSSSSSSS&f=FFFFFFFFFFFFFFFFFFFFFFFFFFF", "Invalid Format");

				return;
			}

			_model.Seed = parts[1].Substring(0, 8); // we only take the first 8 characters of the seed provided
			_model.Flags = Flags.DecodeFlagsText(parts[2]);
		}

		private void SetScaleFactorLabel(Slider slider, Label label, CheckBox clamp)
		{
			var lower = (bool)clamp.IsChecked ? 100 : Math.Round(100 / slider.Value);
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

		private void AboutButton_Click(object sender, RoutedEventArgs e)
		{
			var aboutWindow = new AboutWindow(FFRVersion.Version) { Owner = this };

			aboutWindow.ShowDialog();
		}

		private void PriceScaleFactorSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
		{
			((Slider)sender).Value = Math.Round(e.NewValue, 1);
			SetScaleFactorLabel(PriceScaleFactorSlider, PriceScaleFactorLabel, ClampPricesCheckBox);
		}

		private void EnemyScaleFactorSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
		{
			((Slider)sender).Value = Math.Round(e.NewValue, 1);
			SetScaleFactorLabel(EnemyScaleFactorSlider, EnemyScaleFactorLabel, ClampEnemiesCheckBox);
		}

		private void BossScaleFactorSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
		{
			((Slider)sender).Value = Math.Round(e.NewValue, 1);
			SetScaleFactorLabel(BossScaleFactorSlider, BossScaleFactorLabel, ClampBossesCheckBox);
		}

		private void ExpMultiplierSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
		{
			((Slider)sender).Value = Math.Round(e.NewValue, 1);
			SetExpLabel();
		}

		private void ExpBonusSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
		{
			((Slider)sender).Value = Math.Round(e.NewValue / 10.0) * 10.0;
			SetExpLabel();
		}
		private void EncounterRate_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
		{
			((Slider)sender).Value = Math.Round(e.NewValue);
			EncounterRateFactorLabel.Content = $"{Math.Round(EncounterRateSlider.Value / 30.0, 2)}x";
		}
		private void DungeonEncounterRate_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
		{
			((Slider)sender).Value = Math.Round(e.NewValue);
			DungeonEncounterRateFactorLabel.Content = $"{Math.Round(DungeonEncounterRateSlider.Value / 30.0, 2)}x";
		}

		private void LoadPreset(object sender, RoutedEventArgs e)
		{
			var openFileDialog = new OpenFileDialog
			{
				Filter = "JSON files (*.json)|*.json",
				InitialDirectory = System.IO.Path.Combine(Directory.GetCurrentDirectory(), "presets"),
			};

			var result = openFileDialog.ShowDialog(this);
			if (result == true)
			{
				_model.Flags = Flags.FromJson(File.ReadAllText(openFileDialog.FileName)).flags;
			}
		}

		private void ClampPricesCheckBox_ValueChanged(object sender, RoutedEventArgs e)
		{
			SetScaleFactorLabel(PriceScaleFactorSlider, PriceScaleFactorLabel, ClampPricesCheckBox);
		}

		private void ClampEnemiesCheckBox_ValueChanged(object sender, RoutedEventArgs e)
		{
			SetScaleFactorLabel(EnemyScaleFactorSlider, EnemyScaleFactorLabel, ClampEnemiesCheckBox);
		}

		private void ClampBossesCheckBox_ValueChanged(object sender, RoutedEventArgs e)
		{
			SetScaleFactorLabel(BossScaleFactorSlider, BossScaleFactorLabel, ClampBossesCheckBox);
		}

	}
}
