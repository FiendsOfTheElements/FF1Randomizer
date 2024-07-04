using FF1Lib.Helpers;
using Microsoft.VisualBasic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System;
using System.ComponentModel;
using System.Numerics;
using System.Reflection;
using System.Xml.Linq;
using System.IO.Compression;
using FF1Lib.Procgen;
using RomUtilities;

namespace FF1Lib
{
	public enum SettingType
	{
		Root,
		Toggle,
		OptionList,
		IntegerRange,
		SettingList
	}
	public struct Option
	{
		public string Name { get; set; }
		public int Value { get; set; }
		public int Weight { get; set; }
	}
/*	public class FloatOption : Option
	{
		public float Value { get; set; }
	}
	public class IntOption : Option
	{
		public int Value { get; set; }
	}*/

	public class Setting
	{
		public string Name { get; set; }
		public SettingType Type { get; set; }
		public bool Randomize { get; set; }
		public int CurrentValue { get; set; }
		public List<Option> Options { get; set; }
		public Setting(string _name, SettingType _type)
		{
			Name = _name;
			Type = _type;
			Randomize = false;
		}
		public Setting(Setting copysetting)
		{
			Name = copysetting.Name;
			Type = copysetting.Type;
			Randomize = copysetting.Randomize;
			CurrentValue = copysetting.CurrentValue;
			Options = copysetting.Options.Select(o => new Option() { Name = o.Name, Value = o.Value, Weight = o.Weight }).ToList();
		}
		public Setting(string _name, SettingType _type, Type _enumtype = null)
		{
			Name = _name;
			Type = _type;
			Randomize = false;
			CurrentValue = 0;
			if (Type == SettingType.Toggle)
			{
				Options = new() { new Option() { Name = "False", Value = 0, Weight = 1 }, new Option() { Name = "True", Value = 1, Weight = 1 } };
			}
			else if (Type == SettingType.OptionList)
			{

				Options = Enum.GetNames(_enumtype).ToList().Select(x => new Option()
				{
					Name = GetDescriptionFromEnumValue((Enum)Enum.Parse(_enumtype, x)),
					Value = (int)Enum.Parse(_enumtype, x),
					Weight = 1
				}).ToList();
			}
		}
		public Setting(string _name, int value = 0)
		{
			Name = _name;
			Type = SettingType.Toggle;
			Randomize = false;
			CurrentValue = value;
			Options = new() { new() { Name = "False", Value = 0, Weight = 1 }, new() { Name = "True", Value = 1, Weight = 1 } };
		}
		public Setting(string _name, Type _enumtype, int value = 0)
		{
			Name = _name;
			Type = SettingType.OptionList;
			Randomize = false;
			CurrentValue = value;
			Options = Enum.GetNames(_enumtype).ToList().Select(x => new Option()
			{
				Name = GetDescriptionFromEnumValue((Enum)Enum.Parse(_enumtype, x)),
				Value = (int)Enum.Parse(_enumtype, x),
				Weight = 1
			}).ToList();
		}
		public Setting(string _name, int max, int min, int step, int denom, int value = 0)
		{
			Name = _name;
			Type = SettingType.IntegerRange;
			Randomize = false;
			CurrentValue = value;
			if (Type == SettingType.IntegerRange)
			{
				Options = new()
				{
					new() { Name = "Min", Value = min, Weight = 0 },
					new() { Name = "Max", Value = max, Weight = 0 },
					new() { Name = "Step", Value = step, Weight = 0 },
					new() { Name = "Denom", Value = denom, Weight = 0 },
				};
			}
		}

		private static string GetDescriptionFromEnumValue(Enum value)
		{
			DescriptionAttribute attribute = value.GetType()
				.GetField(value.ToString())
				.GetCustomAttributes(typeof(DescriptionAttribute), false)
				.SingleOrDefault() as DescriptionAttribute;
			return attribute == null ? value.ToString() : attribute.Description;
		}
		public Setting() {	}
		public void CollapseValue(MT19337 rng)
		{
			if (!Randomize)
			{
				return;
			}

			if (Type == SettingType.Toggle || Type == SettingType.OptionList)
			{
				int maxrange = Options.Sum(o => o.Weight);
				int rngpick = rng.Between(0, maxrange - 1);

				Options = Options.OrderBy(o => o.Value).ToList();
				int cumul = 0;

				foreach (var option in Options)
				{
					cumul += option.Weight;
					if (rngpick < cumul)
					{
						CurrentValue = option.Value;
						break;
					}
				}
			}
			else if (Type == SettingType.IntegerRange)
			{
				int minrange = Options.Find(o => o.Name == "Min").Value;
				int maxrange = Options.Find(o => o.Name == "Max").Value;

				CurrentValue = rng.Between(minrange, maxrange);
			}
		}
	}
	public struct LayoutFlag
	{
		public string Name { get; set; }
		public string DisplayName { get; set; }
		public string Tooltip { get; set; }
		public List<string> IncompatibleFlags { get; set; }
	}
	public struct LayoutSection
	{
		public string Name { get; set; }
		public List<LayoutFlag> Flags { get; set; }
	}
	public partial class LayoutData
	{
		public List<LayoutSection> Layout { get; set; }
		public LayoutData(bool standardflags)
		{
			if (standardflags)
			{
				Layout = StandardLayout;
			}
		}
	}

	public partial class Settings
	{
		private Dictionary<string, Setting> initialSettings;
		private Dictionary<string, Setting> settings;
		string flagstringtext;
		public int Init()
		{
			initialSettings = new();
			
			initialSettings.Add("Free Lute", new Setting("Free Lute"));

			initialSettings.Add("ThiefMode", new Setting("ThiefMode", typeof(ThiefOptions), (int)ThiefOptions.Lockpicking));

			initialSettings.Add("Bugfixes", new Setting("Bugfixes"));

			initialSettings["Bugfixes"].CurrentValue = 1;
			initialSettings["Free Lute"].Randomize = true;
			//settings.Find(s => s.Name == "ThiefMode").CurrentValue = (int)ThiefAGI.Agi120;

			settings = initialSettings;

			var content = JsonConvert.SerializeObject(initialSettings.Select(s => s.Value).ToList());

			// load initial
			// settings = initialSettings
			// collapse, initialSettings stay same, setting is new
			return 0;
		}
		public Settings(bool standardflags)
		{
			if (standardflags)
			{
				initialSettings = StandardSettings.ToDictionary(s => s.Name, s => s);
			}
			else
			{
				initialSettings = AdvancedSettings.ToDictionary(s => s.Name, s => s);
			}

			var content = JsonConvert.SerializeObject(initialSettings.Select(s => s.Value).ToList());

			settings = initialSettings;

		}
		public void SetStandardFlags(MT19337 rng)
		{
			CreateSettingsCopy();
			CollapseRandomSettings(rng);
			ProcessStandardFlags();
		}
		public void SetAdvancedFlags(MT19337 rng)
		{
			CreateSettingsCopy();
			CollapseRandomSettings(rng);
		}
		public string GenerateFlagstring()
		{
			BigInteger sum = 0;
			var tempsettings = initialSettings.OrderBy(s => s.Key).Select(s => s.Value).ToList();

			foreach (var setting in tempsettings)
			{
				if (setting.Type == SettingType.Toggle || setting.Type == SettingType.OptionList)
				{
					sum = sum * setting.Options.Count + setting.CurrentValue;
					sum = sum * 2 + (setting.Randomize ? 1 : 0);
				}
				else if (setting.Type == SettingType.IntegerRange)
				{
					int min = setting.Options.Find(o => o.Name == "Min").Value;
					int max = setting.Options.Find(o => o.Name == "Max").Value;
					int step = setting.Options.Find(o => o.Name == "Step").Value;
					int size = (max - min) / step;

					sum = sum * size + setting.CurrentValue;
					sum = sum * 2 + (setting.Randomize ? 1 : 0);
				}
			}

			flagstringtext = BigIntegerToString(sum);
			return flagstringtext;
		}
		public void ReadFlagstring(string flagstring)
		{
			BigInteger sum = StringToBigInteger(flagstring);
			var tempsettings = initialSettings.OrderByDescending(s => s.Key).Select(s => s.Value).ToList();

			foreach (var setting in tempsettings)
			{
				if (setting.Type == SettingType.Toggle || setting.Type == SettingType.OptionList)
				{
					BigInteger result = 0;

					sum = BigInteger.DivRem(sum, 2, out result);
					setting.Randomize = result == 1;

					sum = BigInteger.DivRem(sum, setting.Options.Count, out result);
					setting.CurrentValue = (int)result;
				}
				else if (setting.Type == SettingType.IntegerRange)
				{
					int min = setting.Options.Find(o => o.Name == "Min").Value;
					int max = setting.Options.Find(o => o.Name == "Max").Value;
					int step = setting.Options.Find(o => o.Name == "Step").Value;
					int size = (max - min) / step;

					BigInteger result = 0;

					sum = BigInteger.DivRem(sum, 2, out result);
					setting.Randomize = result == 1;

					sum = BigInteger.DivRem(sum, size, out result);
					setting.CurrentValue = (int)result;
				}
			}
		}
		private const string Base64Chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789.-";
		private static string BigIntegerToString(BigInteger sum)
		{
			var s = "";

			while (sum > 0)
			{
				sum = BigInteger.DivRem(sum, 64, out var result); ;
				s += Base64Chars[(int)result];
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

				sum = sum * 64 + index;
			}

			return sum;
		}
		private void CreateSettingsCopy()
		{
			settings = new();

			foreach (var setting in initialSettings)
			{
				settings.Add(setting.Value.Name, new Setting(setting.Value));
			}
		}
		public int CollapseRandomSettings(MT19337 rng)
		{

			foreach (var setting in settings)
			{
				setting.Value.CollapseValue(rng);
			}

			return 0;
		}
		public bool GetBool(string name)
		{
			if (settings.TryGetValue(name, out var value))
			{
				return value.CurrentValue == 1;
			}
			else
			{
				return false;
			}
		}
		public bool TryGetBool(string name, out bool value)
		{
			if (settings.TryGetValue(name, out var result))
			{
				value = result.CurrentValue == 1;
				return true;
			}
			else
			{
				value = false;
				return false;
			}
		}
		public bool TryGetInt(string name, out int value)
		{
			if (settings.TryGetValue(name, out var result))
			{
				value = result.CurrentValue;
				return true;
			}
			else
			{
				value = -1;
				return false;
			}
		}
		public bool TryGetFloat(string name, out float value)
		{
			if (settings.TryGetValue(name, out var result))
			{
				var denomlist = result.Options.Where(x => x.Name == "Denom");

				float denom = 1.0f;
				if (denomlist.Any())
				{
					denom = (float)denomlist.First().Value;
				}

				value = (float)result.CurrentValue / denom;
				return true;
			}
			else
			{
				value = -1;
				return false;
			}
		}
		public float GetFloat(string name)
		{
			if (settings.TryGetValue(name, out var result))
			{
				var denomlist = result.Options.Where(x => x.Name == "Denom");

				float denom = 1.0f;
				if (denomlist.Any())
				{
					denom = (float)denomlist.First().Value;
				}

				return (float)result.CurrentValue / denom;
			}
			else
			{
				return -1;
			}
		}
		public bool TryGetEnum(string name, Type type, out object value)
		{
			if (settings.TryGetValue(name, out var result))
			{
				value = Enum.ToObject(type, result.CurrentValue);
				return true;
			}
			else
			{
				value = null;
				return false;
			}
		}
		public int GetInt(string name)
		{
			if (settings.TryGetValue(name, out var value))
			{
				return value.CurrentValue;
			}
			else
			{
				return -1;
			}
		}
		public object GetEnum(string name, Type type)
		{
			if (settings.TryGetValue(name, out var value))
			{
				return Enum.ToObject(type, value.CurrentValue);
			}
			else
			{
				return "wrong";
			}
		}
		public void UpdateSetting(string name, int updatevalue)
		{
			if (settings.TryGetValue(name, out var value))
			{
				value.CurrentValue = updatevalue;
			}
			else
			{
				settings.Add(name, new() { Name = name, CurrentValue = updatevalue });
			}
		}
		public int SetValue()
		{
			string content = "[{\"Name\":\"Bugfixes\",\"Type\":1,\"Randomize\":false,\"CurrentValue\":1,\"Options\":[{\"Name\":\"False\",\"Value\":0,\"Weight\":1},{\"Name\":\"True\",\"Value\":1,\"Weight\":1}]},{\"Name\":\"ThiefMode\",\"Type\":2,\"Randomize\":false,\"CurrentValue\":1,\"Options\":[{\"Name\":\"None\",\"Value\":0,\"Weight\":1},{\"Name\":\"Double Hit% Growth\",\"Value\":1,\"Weight\":1},{\"Name\":\"Raised Agility\",\"Value\":2,\"Weight\":1},{\"Name\":\"Lockpicking\",\"Value\":3,\"Weight\":1}]},{\"Name\":\"WhiteMageMode\",\"Type\":2,\"Randomize\":false,\"CurrentValue\":1,\"Options\":[{\"Name\":\"None\",\"Value\":0,\"Weight\":1},{\"Name\":\"Harm Hurts All Type\",\"Value\":1,\"Weight\":1}]},{\"SubSettings\":[{\"Name\":\"Lute\",\"Type\":1,\"Randomize\":false,\"CurrentValue\":0,\"Options\":[{\"Name\":\"False\",\"Value\":0,\"Weight\":1},{\"Name\":\"True\",\"Value\":1,\"Weight\":1}]},{\"Name\":\"HealPotion\",\"Type\":1,\"Randomize\":false,\"CurrentValue\":99,\"Options\":[{\"Name\":\"False\",\"Value\":0,\"Weight\":1},{\"Name\":\"True\",\"Value\":1,\"Weight\":1}]}],\"Name\":\"TestFlagList\",\"Type\":4,\"Randomize\":false,\"CurrentValue\":0,\"Options\":null}]";
			initialSettings = JsonConvert.DeserializeObject<List<Setting>>(content).ToDictionary(s => s.Name, s => s);


			///Name = deserializedcontent.Name;
			//Children = deserializedcontent.Children;

			return 0;
		}
		public void ProcessRule(FlagRule rule, int value)
		{
			foreach (var action in rule.Actions)
			{
				if (action.Action == FlagActions.Enable)
				{
					UpdateSetting(action.Setting, 1);
				}
				else if (action.Action == FlagActions.TransferValue)
				{
					UpdateSetting(action.Setting, value);
				}/*
				else if (setting.Action == FlagActions.SelectOption)
				{
					settings.Put(setting.Setting + "/" + Enum.GetName(setting.Type, Enum.ToObject(setting.Type, value)));
				}*/
				else if (action.Action == FlagActions.SetValue)
				{
					UpdateSetting(action.Setting, action.Value);
				}
			}
		}
		public void ProcessStandardFlags()
		{
			var properties = typeof(FlagRules).GetProperties();
			var flagrules = properties.Where(p => p.PropertyType == typeof(FlagRule)).Select(p => (FlagRule)p.GetValue(null)).ToList();

			foreach (var flagrule in flagrules)
			{
				bool conditionmet = false;

				foreach (var conditionset in flagrule.Conditions)
				{
					conditionmet = true;

					foreach (var condition in conditionset)
					{
						if (TryGetInt(condition.Name, out var value))
						{
							conditionmet &= (value == condition.Value);
						}
						else
						{
							conditionmet = false;
							break;
						}
					}
				}

				if (conditionmet)
				{
					ProcessRule(flagrule, 0);
				}
			}
		}
	}

	public enum FlagActions
	{
		Enable,
		SetValue,
		TransferValue,
		//ReadOption,
		//SelectOption,
		//SelectValue,
	}
	public struct FlagRule
	{
		public List<List<FlagCondition>> Conditions { get; set; }
		public List<FlagAction> Actions { get; set; }
	}
	public struct FlagCondition
	{
		public string Name { get; set; }
		public SettingType Type { get; set; }
		public int Value { get; set; }
	}
	public struct FlagAction
	{
		public FlagActions Action { get; set; }
		public int Value { get; set; }
		public string Setting { get; set; }
		public Type Type { get; set; }
	}
	public static partial class FlagRules
	{

	}
}
