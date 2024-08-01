using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Data;
using FF1Lib;

namespace FF1Randomizer
{
	public class MainWindowViewModel : INotifyPropertyChanged
	{
		public MainWindowViewModel()
		{
			Flags = new Flags();
			Preferences = new Preferences();
			Flags.PropertyChanged += (sender, args) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Flags"));
			FlagsFilter = "";
		}

		public event PropertyChangedEventHandler PropertyChanged;

		public string WindowTitle => $"FF1 Randomizer {FFRVersion.Version}";

		private string _filename;
		public string Filename
		{
			get => _filename;
			set
			{
				_filename = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Filename"));
			}
		}

		private string _seed;
		public string Seed
		{
			get => _seed;
			set
			{
				_seed = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Seed"));
			}
		}

		private string _flagstext;
		public string FlagsText
		{
			get => _flagstext;
			set
			{
				_flagstext = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("FlagsText"));
			}
		}

		private Flags _flags;
		public Flags Flags
		{
			get => _flags;
			set
			{
				_flags = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Flags"));
			}
		}
		private Preferences _preferences;
		public Preferences Preferences
		{
			get => _preferences;
			set
			{
				_preferences = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Preferences"));
			}
		}

		private string _flagsFilter;
		public string FlagsFilter
		{
			get => _flagsFilter;
			set
			{
				_flagsFilter = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("FlagsFilter"));
			}
		}
		private string _flagsString;
		public string FlagsString
		{
			get => _flagsString;
			set
			{
				_flagsString = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("FlagsString"));
			}
		}
		private string _preferencesFilter;
		public string PreferencesFilter
		{
			get => _preferencesFilter;
			set
			{
				_preferencesFilter = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("PreferencesFilter"));
			}
		}
	}

	public class FlagsToStringConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) => Flags.EncodeFlagsText((Flags)value);
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => Flags.DecodeFlagsText((string)value);
	}
}
