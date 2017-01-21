using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;

namespace FF1Randomizer
{
	/// <summary>
	/// Interaction logic for AboutWindow.xaml
	/// </summary>
	public partial class AboutWindow : Window
	{
		public AboutWindow(string version)
		{
			InitializeComponent();

			VersionTextBlock.Text = $"Final Fantasy 1 Randomizer{Environment.NewLine}Version {version}";
		}

		private void OkButton_Click(object sender, RoutedEventArgs e)
		{
			DialogResult = true;

			Close();
		}
	}
}
