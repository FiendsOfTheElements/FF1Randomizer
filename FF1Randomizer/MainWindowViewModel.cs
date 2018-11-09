using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using FF1Lib;

namespace FF1Randomizer
{
	public class MainWindowViewModel : INotifyPropertyChanged
	{
		public MainWindowViewModel()
		{
			Flags = new ViewModelFlags();
			Flags.PropertyChanged += (sender, args) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Flags"));
		}

		public event PropertyChangedEventHandler PropertyChanged;

		public string WindowTitle => $"FF1 Randomizer {FF1Rom.Version}";

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

		private ViewModelFlags _flags;
		public ViewModelFlags Flags
		{
			get => _flags;
			set
			{
				_flags = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Flags"));
			}
		}
	}

	public class FlagsToStringConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) => Flags.EncodeFlagsText(((ViewModelFlags)value).Flags);
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => new ViewModelFlags { Flags = Flags.DecodeFlagsText((string)value) };
	}
}
