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
		private NesRom _rom;

		public MainWindow()
		{
			InitializeComponent();
		}

		private void ScaleFactorSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
		{
			ScaleFactorSlider.Value = Math.Round(10*ScaleFactorSlider.Value)/10.0;
			ScaleFactorLabel.Content = $"(1/{ScaleFactorSlider.Value})x - {ScaleFactorSlider.Value}x";
		}

		private void RomButton_Click(object sender, RoutedEventArgs e)
		{
			var openFileDialog = new Microsoft.Win32.OpenFileDialog()
			{
				Filter = "NES ROM files (*.nes)|*.nes"
			};

			var result = openFileDialog.ShowDialog(this);
			if (result == true)
			{
				_rom = new NesRom(openFileDialog.FileName);

				GenerateButton.IsEnabled = true;
			}
		}
	}
}
