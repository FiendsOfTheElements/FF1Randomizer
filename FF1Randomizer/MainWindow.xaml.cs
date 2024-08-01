using FF1Lib;
using Microsoft.Win32;
using RomUtilities;
using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Threading.Tasks;
using System.Linq;
using System.Numerics;
using System.Reflection;
using Newtonsoft.Json.Linq;
using System.Windows.Data;
using System.ComponentModel;
using System.Collections;
using Microsoft.VisualBasic.FileIO;
using System.Collections.Generic;
using System.Collections.ObjectModel;

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
			_model = new MainWindowViewModel();
			_model.Flags = Flags.FromJson(File.ReadAllText("presets/default.json")).flags;
			_model.FlagsText = GenerateFlagsView(_model.Flags);

			InitializeComponent();

			DataContext = _model;

			TryOpenSavedFilename();
			GenerateSeed();
			foreach (var panel in GenerateFlagsList())
			{
				FlagsList.Items.Add(panel);
			};

			foreach (var panel in GeneratePreferencesList())
			{
				PrefrencesList.Items.Add(panel);
			};

			FlagsList.Items.Filter = new Predicate<object>(FlagsFilter);
			PrefrencesList.Items.Filter = new Predicate<object>(PreferencesFilter);
		}

		private bool FlagsFilter(object item)
		{
			if (String.IsNullOrEmpty(_model.FlagsFilter))
				return true;
			else
				return ((item as StackPanel).Name.IndexOf(_model.FlagsFilter, StringComparison.OrdinalIgnoreCase) >= 0);
		}

		private bool PreferencesFilter(object item)
		{
			if (String.IsNullOrEmpty(_model.PreferencesFilter))
				return true;
			else
				return ((item as StackPanel).Name.IndexOf(_model.PreferencesFilter, StringComparison.OrdinalIgnoreCase) >= 0);
		}

		private string GenerateFlagsView(Flags flags)
		{
			var properties = typeof(Flags).GetProperties(BindingFlags.Instance | BindingFlags.Public);
			var flagproperties = properties.Where(p => p.CanWrite).OrderBy(p => p.Name).ToList();

			string flagText = "";

			foreach (var p in flagproperties)
			{
				if (Nullable.GetUnderlyingType(p.PropertyType) == typeof(bool))
				{
					flagText += p.Name + ": " + ((bool?)p.GetValue(flags)).ToString() + "\n";
				}
				else if (p.PropertyType == typeof(bool))
				{
					flagText += p.Name + ": " + ((bool)p.GetValue(flags)).ToString() + "\n";
				}
				else if (p.PropertyType.IsEnum)
				{
					flagText += p.Name + ": " + Enum.GetNames(p.PropertyType)[(int)p.GetValue(flags)] + "\n";
				}
				else if (p.PropertyType == typeof(int))
				{
					IntegerFlagAttribute ia = p.GetCustomAttribute<IntegerFlagAttribute>();

					var radix = (ia.Max - ia.Min) / ia.Step + 1;
					var val = (int)p.GetValue(flags);
					var raw_val = (val - ia.Min) / ia.Step;

					flagText += p.Name + ": " + raw_val + "/" + radix + "\n";
				}
				else if (p.PropertyType == typeof(double))
				{
					DoubleFlagAttribute ia = p.GetCustomAttribute<DoubleFlagAttribute>();
					var radix = (int)Math.Ceiling((ia.Max - ia.Min) / ia.Step) + 1;
					var val = (double)p.GetValue(flags);
					var raw_val = (int)Math.Round((val - ia.Min) / ia.Step);

					flagText += p.Name + ": " + raw_val + "/" + radix + "\n";
				}
			}

			return flagText;
		}
		private List<StackPanel> GeneratePreferencesList()
		{
			var properties = typeof(Preferences).GetProperties(BindingFlags.Instance | BindingFlags.Public);
			var flagproperties = properties.Where(p => p.CanWrite).OrderBy(p => p.Name).ToList();

			return GenerateSettingsList(flagproperties);
		}
		private List<StackPanel> GenerateFlagsList()
		{
			var properties = typeof(Flags).GetProperties(BindingFlags.Instance | BindingFlags.Public);
			var flagproperties = properties.Where(p => p.CanWrite).OrderBy(p => p.Name).ToList();

			return GenerateSettingsList(flagproperties);
		}
		private List<StackPanel> GenerateSettingsList(List<PropertyInfo> flagproperties)
		{
			List<StackPanel> panelList = new();

			foreach (var p in flagproperties)
			{
				string bindingString = "Flags." + p.Name;
				Binding myBinding = new Binding();
				myBinding.Path = new PropertyPath(bindingString);

				if (Nullable.GetUnderlyingType(p.PropertyType) == typeof(bool))
				{
					var checkBox = new CheckBox() { Name = p.Name, IsThreeState = true, ToolTip = p.Name, Margin = new Thickness(20,0,0,0) };
					checkBox.SetBinding(CheckBox.IsCheckedProperty, myBinding);
					var label = new Label() { Name = p.Name + "Label", Content = p.Name };
					var checkBoxView = new StackPanel() { Name = p.Name + "Stack" };

					checkBoxView.Children.Add(label);
					checkBoxView.Children.Add(checkBox);

					panelList.Add(checkBoxView);
				}
				else if (p.PropertyType == typeof(bool))
				{
					var checkBox = new CheckBox() { Name = p.Name, ToolTip = p.Name, Margin = new Thickness(20, 0, 0, 0) };
					checkBox.SetBinding(CheckBox.IsCheckedProperty, myBinding);
					var label = new Label() { Name = p.Name + "Label", Content = p.Name };
					var checkBoxView = new StackPanel() { Name = p.Name + "Stack" };

					checkBoxView.Children.Add(label);
					checkBoxView.Children.Add(checkBox);

					panelList.Add(checkBoxView);
				}
				else if (p.PropertyType.IsEnum)
				{
					var dropDown = new ComboBox { Name = p.Name, ToolTip = p.Name, SelectedValuePath = "Tag", Margin = new Thickness(20, 0, 0, 0) };
					dropDown.SetBinding(ComboBox.SelectedValueProperty, myBinding);
					var label = new Label() { Name = p.Name + "Label", Content = p.Name };

					var itemValues = Enum.GetValues(p.PropertyType);

					foreach (var item in itemValues)
					{
						var type = item.GetType();
						var memberInfo = type.GetMember(item.ToString());
						var attributes = memberInfo[0].GetCustomAttributes(typeof(DescriptionAttribute), false);
						var description = attributes.Length > 0 ? ((DescriptionAttribute)attributes[0]).Description : item.ToString();

						dropDown.Items.Add(new ComboBoxItem() { Tag = item, Content = description });
					}

					var dropDownView = new StackPanel() { Name = p.Name + "Stack" };

					dropDownView.Children.Add(label);
					dropDownView.Children.Add(dropDown);

					panelList.Add(dropDownView);
				}
				else if (p.PropertyType == typeof(int))
				{
					IntegerFlagAttribute ia = p.GetCustomAttribute<IntegerFlagAttribute>();

					var slider = new Slider { Name = p.Name, ToolTip = p.Name, Maximum = ia.Max, Minimum = ia.Min, Margin = new Thickness(20, 0, 0, 0) };
					slider.SetBinding(Slider.ValueProperty, myBinding);

					slider.ValueChanged += new RoutedPropertyChangedEventHandler<double>(Slider_ValueChanged);

					var label = new Label() { Name = p.Name + "Label", Content = p.Name + ":", Width = 200 };

					var sliderView = new StackPanel() { Name = p.Name + "Stack" };

					sliderView.Children.Add(label);
					sliderView.Children.Add(slider);

					panelList.Add(sliderView);
				}
				else if (p.PropertyType == typeof(double))
				{
					DoubleFlagAttribute ia = p.GetCustomAttribute<DoubleFlagAttribute>();
					var slider = new Slider { Name = p.Name, ToolTip = p.Name, Maximum = ia.Max, Minimum = ia.Min, Margin = new Thickness(20, 0, 0, 0) };
					slider.SetBinding(Slider.ValueProperty, myBinding);

					slider.ValueChanged += new RoutedPropertyChangedEventHandler<double>(Slider_ValueChanged);

					var label = new Label() { Name = p.Name + "Label", Content = p.Name + ":", Width = 200 };

					var sliderView = new StackPanel() { Name = p.Name + "Stack" };

					sliderView.Children.Add(label);
					sliderView.Children.Add(slider);

					panelList.Add(sliderView);
				}
			}

			return panelList;
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
			if (_model.Flags.ResourcePack != null)
			{
				_model.Flags.ResourcePack = Convert.ToBase64String(File.ReadAllBytes(ResourcePackTextBox.Text));
			}
			await rom.Randomize(Blob.FromHex(_model.Seed), _model.Flags, _model.Preferences);

			var fileRoot = _model.Filename.Substring(0, _model.Filename.LastIndexOf("."));
			var outputFilename = $"{fileRoot}_{_model.Seed}_{rom.GetHash()}.nes";
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

		private void SetSliderDisplayValue(Slider slider, Label label)
		{
			var labelName = label.Content.ToString().Split(':')[0];

			label.Content = labelName + ": " + $"{slider.Value}";
		}

		private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
		{
			var objectName = ((Slider)sender).Name;
			var stackObject = (StackPanel)((Slider)sender).Parent;
			var labelName = objectName + "Label";
			var stackName = objectName + "Stack";

			if (stackObject == null) return;

			var labelObject = stackObject.Children.OfType<Label>().First();

			if (labelObject != null) SetSliderDisplayValue((Slider)sender, labelObject);
		}

		private void FlagsFilter_TextChanged(object sender, TextChangedEventArgs e)
		{
			_model.FlagsFilter = (sender as TextBox).Text;
			FlagsList.Items.Filter = FlagsList.Items.Filter; // seesh
		}

		private void PreferencesFilter_TextChanged(object sender, TextChangedEventArgs e)
		{
			_model.PreferencesFilter = (sender as TextBox).Text;
			PrefrencesList.Items.Filter = PrefrencesList.Items.Filter; // seesh
		}

		private void AboutButton_Click(object sender, RoutedEventArgs e)
		{
			var aboutWindow = new AboutWindow(FFRVersion.Version) { Owner = this };

			aboutWindow.ShowDialog();
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

		private void Button_Click(object sender, RoutedEventArgs e)
		{
			var openFileDialog = new OpenFileDialog
			{
				Filter = "Zip files (*.zip)|*.zip"
			};

			var result = openFileDialog.ShowDialog(this);
			if (result == true)
			{
				ResourcePackTextBox.Text = openFileDialog.FileName;
			}
		}
	}
}
