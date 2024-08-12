using System.Numerics;
using System.Reflection;
using Newtonsoft.Json;
using System.IO.Compression;
using static FF1Lib.FF1Rom;
using FF1Lib.Sanity;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using static System.Net.Mime.MediaTypeNames;

namespace FF1Lib
{
	public partial class Flags : IIncentiveFlags, IScaleFlags, IVictoryConditionFlags, IFloorShuffleFlags, IItemPlacementFlags
	{
		public string Encoded => Flags.EncodeFlagsText(this);

		public event PropertyChangedEventHandler PropertyChanged;

		private void RaisePropertyChanged([CallerMemberName] string property = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
		}
		private static bool ConvertTriState(bool? tristate, MT19337 rng)
		{
			int rngval = rng.Between(0, 1);
			bool rval = tristate ?? (rngval == 0);
			return rval;
		}

		public static Flags ConvertAllTriState(Flags flags, MT19337 rng)
		{
			Flags newflags = flags.ShallowCopy();
			PropertyInfo[] properties = newflags.GetType().GetProperties();
			foreach (var property in properties)
			{
				if (property.PropertyType == typeof(bool?) && property.GetValue(newflags) == null)
				{
					bool newvalue = ConvertTriState((bool?)property.GetValue(newflags), rng);
					property.SetValue(newflags, newvalue);
				}
			}

			if (flags.ItemMagicMode == ItemMagicMode.Random) newflags.ItemMagicMode = (ItemMagicMode)rng.Between(0, 2);
			if (flags.ItemMagicPool == ItemMagicPool.Random) newflags.ItemMagicPool = (ItemMagicPool)rng.Between(0, 4);

			if (flags.GuaranteedDefenseItem == GuaranteedDefenseItem.Random) newflags.GuaranteedDefenseItem = rng.Between(0, 1) > 0 ? GuaranteedDefenseItem.Any : GuaranteedDefenseItem.None;
			if (flags.GuaranteedPowerItem == GuaranteedPowerItem.Random) newflags.GuaranteedPowerItem = rng.Between(0, 1) > 0 ? GuaranteedPowerItem.Any : GuaranteedPowerItem.None;

			if (newflags.GuaranteedDefenseItem == GuaranteedDefenseItem.Any) newflags.GuaranteedDefenseItem = (GuaranteedDefenseItem)rng.Between(1, 3);
			if (newflags.GuaranteedPowerItem == GuaranteedPowerItem.Any) newflags.GuaranteedPowerItem = (GuaranteedPowerItem)rng.Between(1, 3);

			return newflags;
		}

		public Flags ShallowCopy()
		{
			return (Flags)this.MemberwiseClone();
		}
		public static string EncodeFlagsText(Flags flags)
		{
			var properties = typeof(Flags).GetProperties(BindingFlags.Instance | BindingFlags.Public);
			var flagproperties = properties.Where(p => p.CanWrite && !Attribute.IsDefined(p, typeof(FlagIsAllClasses)))
				.OrderBy(p => p.Name)
				.ToList();

			BigInteger sum = 0;
			sum = AddString(sum, 7, (FFRVersion.Sha.Length >= 7) ? FFRVersion.Sha.Substring(0, 7) : FFRVersion.Sha.PadRight(7, 'X'));

			foreach (var p in flagproperties)
			{
				if (Nullable.GetUnderlyingType(p.PropertyType) == typeof(bool))
				{
					sum = AddTriState(sum, (bool?)p.GetValue(flags));
				}
				else if (p.PropertyType == typeof(bool))
				{
					sum = AddBoolean(sum, (bool)p.GetValue(flags));
				}
				else if (p.PropertyType.IsEnum)
				{
					sum = AddNumeric(sum, Enum.GetValues(p.PropertyType).Cast<int>().Max() + 1, (int)p.GetValue(flags));
				}
				else if (p.PropertyType == typeof(int))
				{
					IntegerFlagAttribute ia = p.GetCustomAttribute<IntegerFlagAttribute>();
					var radix = (ia.Max - ia.Min) / ia.Step + 1;
					var val = (int)p.GetValue(flags);
					var raw_val = (val - ia.Min) / ia.Step;
					sum = AddNumeric(sum, radix, raw_val);
				}
				else if (p.PropertyType == typeof(double))
				{
					DoubleFlagAttribute ia = p.GetCustomAttribute<DoubleFlagAttribute>();
					var radix = (int)Math.Ceiling((ia.Max - ia.Min) / ia.Step) + 1;
					var val = (double)p.GetValue(flags);
					var raw_val = (int)Math.Round((val - ia.Min) / ia.Step);
					sum = AddNumeric(sum, radix, raw_val);
				}
			}

			return BigIntegerToString(sum);
		}
		public void ReadFromFlags(Flags newflags)
		{
			var properties = typeof(Flags).GetProperties(BindingFlags.Instance | BindingFlags.Public);
			var flagproperties = properties.Where(p => p.CanWrite && !Attribute.IsDefined(p, typeof(FlagIsAllClasses)))
				.OrderBy(p => p.Name)
				.Reverse()
				.ToList();

			foreach (var p in flagproperties)
			{
				if (Nullable.GetUnderlyingType(p.PropertyType) == typeof(bool))
				{
					p.SetValue(this, (bool?)p.GetValue(newflags));
				}
				else if (p.PropertyType == typeof(bool))
				{
					p.SetValue(this, (bool)p.GetValue(newflags));
				}
				else if (p.PropertyType.IsEnum)
				{
					p.SetValue(this, (int)p.GetValue(newflags));
				}
				else if (p.PropertyType == typeof(int))
				{
					p.SetValue(this, (int)p.GetValue(newflags));
				}
				else if (p.PropertyType == typeof(double))
				{
					p.SetValue(this, (double)p.GetValue(newflags));
				}
			}
		}

		public static Flags DecodeFlagsText(string text)
		{
			var properties = typeof(Flags).GetProperties(BindingFlags.Instance | BindingFlags.Public);
			var flagproperties = properties.Where(p => p.CanWrite && !Attribute.IsDefined(p, typeof(FlagIsAllClasses)))
				.OrderBy(p => p.Name)
				.Reverse()
				.ToList();

			var sum = StringToBigInteger(text);
			var flags = new Flags();

			foreach (var p in flagproperties)
			{
				if (Nullable.GetUnderlyingType(p.PropertyType) == typeof(bool))
				{
					p.SetValue(flags, GetTriState(ref sum));
				}
				else if (p.PropertyType == typeof(bool))
				{
					p.SetValue(flags, GetBoolean(ref sum));
				}
				else if (p.PropertyType.IsEnum)
				{
					p.SetValue(flags, GetNumeric(ref sum, Enum.GetValues(p.PropertyType).Cast<int>().Max() + 1));
				}
				else if (p.PropertyType == typeof(int))
				{
					IntegerFlagAttribute ia = p.GetCustomAttribute<IntegerFlagAttribute>();
					var radix = (ia.Max - ia.Min) / ia.Step + 1;
					var raw_val = GetNumeric(ref sum, radix);
					var val = raw_val * ia.Step + ia.Min;
					p.SetValue(flags, val);
				}
				else if (p.PropertyType == typeof(double))
				{
					DoubleFlagAttribute ia = p.GetCustomAttribute<DoubleFlagAttribute>();
					var radix = (int)Math.Ceiling((ia.Max - ia.Min) / ia.Step) + 1;
					var raw_val = GetNumeric(ref sum, radix);
					var val = Math.Min(Math.Max(raw_val * ia.Step + ia.Min, ia.Min), ia.Max);
					p.SetValue(flags, val);
				}
			}

			string EncodedSha = GetString(ref sum, 7);
			if (((FFRVersion.Sha.Length >= 7) ? FFRVersion.Sha.Substring(0, 7) : FFRVersion.Sha.PadRight(7, 'X')) != EncodedSha)
			{
				throw new Exception("The encoded version does not match the expected version");
			}

			return flags;
		}

		private static BigInteger AddEnum<T>(BigInteger sum, T value) => AddNumeric(sum, Enum.GetValues(typeof(T)).Cast<int>().Max() + 1, Convert.ToInt32(value));

		private static BigInteger AddNumeric(BigInteger sum, int radix, int value) => sum * radix + value;
		private static BigInteger AddString(BigInteger sum, int length, string str)
		{
			Encoding AsciiEncoding = Encoding.ASCII;
			byte[] bytes = AsciiEncoding.GetBytes(str);
			BigInteger StringAsBigInt = new BigInteger(bytes);
			BigInteger LargestInt = new BigInteger(Math.Pow(0xFF, bytes.Length) - 1);


			return sum * LargestInt + StringAsBigInt;
		}
		private static BigInteger AddBoolean(BigInteger sum, bool value) => AddNumeric(sum, 2, value ? 1 : 0);
		private static int TriStateValue(bool? value) => value.HasValue ? (value.Value ? 1 : 0) : 2;
		private static BigInteger AddTriState(BigInteger sum, bool? value) => AddNumeric(sum, 3, TriStateValue(value));

		private static T GetEnum<T>(ref BigInteger sum) where T : Enum => (T)(object)GetNumeric(ref sum, Enum.GetValues(typeof(T)).Cast<int>().Max() + 1);

		private static int GetNumeric(ref BigInteger sum, int radix)
		{
			sum = BigInteger.DivRem(sum, radix, out var value);

			return (int)value;
		}
		private static string GetString(ref BigInteger sum, int length)
		{
			BigInteger LargestInt = new BigInteger(Math.Pow(0xFF, length) - 1);
			sum = BigInteger.DivRem(sum, LargestInt, out BigInteger value);
			Encoding AsciiEncoding = Encoding.ASCII;
			byte[] bytes = value.ToByteArray();
			string str = AsciiEncoding.GetString(bytes);

			return str;
		}
		private static bool GetBoolean(ref BigInteger sum) => GetNumeric(ref sum, 2) != 0;
		private static bool? ValueTriState(int value) => value == 0 ? (bool?)false : value == 1 ? (bool?)true : null;
		private static bool? GetTriState(ref BigInteger sum) => ValueTriState(GetNumeric(ref sum, 3));

		private const string Base64Chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789.-";

		private static string BigIntegerToString(BigInteger sum)
		{
			var s = "";

			while (sum > 0)
			{
				var digit = GetNumeric(ref sum, 64);
				s += Base64Chars[digit];
			}

			return s;
		}

		private static BigInteger StringToBigInteger(string s)
		{
			var sum = new BigInteger(0);

			foreach (char c in s.Reverse())
			{
				int index = Base64Chars.IndexOf(c);
				if (index < 0) throw new IndexOutOfRangeException($"{c} is not valid FFR-style Base64.");

				sum = AddNumeric(sum, 64, index);
			}

			return sum;
		}


		public class Preset
		{
			public string Name { get; set; }
			public Flags Flags { get; set; }
		}

		//public static Flags FromJson(string json) => JsonConvert.DeserializeObject<Preset>(json).Flags;

		public class Preset2
		{
			public string Name { get; set; }
			public Dictionary<string, object> Flags { get; set; }
		}

		public static (string name, Flags flags, IEnumerable<string> log) FromJson(string json)
		{
		    var flags = new Flags();
		    string name;
		    IEnumerable<string> log;
		    (name, log) = flags.LoadFromJson(json);
		    return (name, flags, log);
		}

		public  (string name, IEnumerable<string> log) LoadFromJson(string json) {
			var w = new System.Diagnostics.Stopwatch();
			w.Restart();

			var preset = JsonConvert.DeserializeObject<Preset2>(json);
			var preset_dic = preset.Flags.ToDictionary(kv => kv.Key.ToLower());

			var properties = typeof(Flags).GetProperties(BindingFlags.Instance | BindingFlags.Public);
			var flagproperties = properties.Where(p => p.CanWrite).OrderBy(p => p.Name).Reverse().ToList();

			List<string> warnings = new List<string>();

			foreach (var pi in flagproperties)
			{
				if (preset_dic.TryGetValue(pi.Name.ToLower(), out var obj))
				{
					var result = SetValue(pi, this, obj.Value);

					if (result != null) warnings.Add(result);

					preset.Flags.Remove(obj.Key);
				}
				else
				{
					//warnings.Add($"\"{pi.Name}\" was missing in preset and set to default \"{pi.GetValue(flags)}\".");
				}
			}

			foreach (var flag in preset.Flags)
			{
				warnings.Add($"\"{flag.Key}\" with value \"{flag.Value}\" does not exist and has been discarded.");
			}

			warnings.Sort();

			w.Stop();
			return (preset.Name, warnings);
		}

		public void LoadResourcePackFlags(Stream stream) {
		    var archive = new ZipArchive(stream);

		    var fj = archive.GetEntry("flags.json");
		    if (fj != null) {
			using (var s = fj.Open()) {
			    using (StreamReader rd = new StreamReader(s)) {
				this.LoadFromJson(rd.ReadToEnd());
			    }
			}
		    }
		    var overworld = archive.GetEntry("overworld.json");
		    if (overworld != null) {
			using (var s = overworld.Open()) {
			    using (StreamReader rd = new StreamReader(s)) {
				this.ReplacementMap = JsonConvert.DeserializeObject<OwMapExchangeData>(rd.ReadToEnd());
			    }
			}
		    }
		}


		private static string SetValue(PropertyInfo p, Flags flags, object obj)
		{
			try
			{
				if (Nullable.GetUnderlyingType(p.PropertyType) == typeof(bool))
				{
					var t = obj == null ? (bool?)null : (bool?)(bool)obj;
					p.SetValue(flags, t);
				}
				else if (p.PropertyType == typeof(bool))
				{
					if (obj == null) throw new ArgumentNullException();
					p.SetValue(flags, obj);
				}
				else if (p.PropertyType.IsEnum)
				{
					if (obj == null) throw new ArgumentNullException();

					var values = Enum.GetValues(p.PropertyType);

					if (obj is string v)
					{
						foreach (var e in values)
						{
							if (v.ToLower() == e.ToString().ToLower())
							{
								p.SetValue(flags, e);
								return null;
							}
						}
					}
					else if (obj is IConvertible)
					{
						int v2 = Convert.ToInt32(obj);

						foreach (var e in values)
						{
							if (v2 == Convert.ToInt32(e))
							{
								p.SetValue(flags, e);
								return null;
							}
						}
					}

					throw new ArgumentException();
				}
				else if (p.PropertyType == typeof(int))
				{
					IntegerFlagAttribute ia = p.GetCustomAttribute<IntegerFlagAttribute>();
					var v3 = Convert.ToInt32(obj);

					p.SetValue(flags, v3);

					if (v3 > ia.Max)
					{
						return $"\"{p.Name}\" with value \"{obj}\" exceeds the maximum but will be kept.";
					}
					else if (v3 < ia.Min)
					{
						return $"\"{p.Name}\" with value \"{obj}\" deceedes the minimum but will be kept.";
					}
				}
				else if (p.PropertyType == typeof(double))
				{
					DoubleFlagAttribute da = p.GetCustomAttribute<DoubleFlagAttribute>();
					var v3 = Convert.ToDouble(obj);

					p.SetValue(flags, v3);

					if (v3 > da.Max)
					{
						return $"\"{p.Name}\" with value \"{obj}\" exceeds the maximum but will be kept.";
					}
					else if (v3 < da.Min)
					{
						return $"\"{p.Name}\" with value \"{obj}\" deceedes the minimum but will be kept.";
					}
				}
			}
			catch
			{
				return $"\"{p.Name}\" with value \"{obj}\" was invalid and set to default \"{p.GetValue(flags)}\".";
			}

			return null;
		}
	}
}
