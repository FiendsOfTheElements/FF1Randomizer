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
using ROMUtilities;

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
				if (randomizer.Validate())
				{
					MessageBox.Show("ROM appears to be valid.");
				}
				else
				{
					MessageBox.Show("ROM does not appear to be valid.  Proceed at your own risk.");
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
			try
			{
				_seed = Blob.FromHex(SeedTextBox.Text);
			}
			catch (Exception)
			{
				MessageBox.Show("Invalid seed.  Seeds must be eight hexadecimal characters (0-9, A-F).  Generating new seed.");

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

			if (ExpGoldBoostCheckBox.IsChecked == true)
			{
				rom.ExpGoldBoost(10);
			}

			var outputFilename = _filename.Substring(0, _filename.LastIndexOf(".")) + "_" + _seed.ToHex() + ".nes";
			rom.Save(outputFilename);

			MessageBox.Show("Done!");
		}

		private void ScaleFactorSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
		{
			ScaleFactorSlider.Value = Math.Round(ScaleFactorSlider.Value, 1);

			var lower = Math.Round(100/ScaleFactorSlider.Value);
			var upper = Math.Round(100*ScaleFactorSlider.Value);

			ScaleFactorLabel.Content = $"{lower}% - {upper}%";
		}
	}
}
