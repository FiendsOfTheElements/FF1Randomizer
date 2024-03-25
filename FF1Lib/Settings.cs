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

namespace FF1Lib
{
	public enum SettingType
	{
		Root,
		Toggle,
		OptionList,
		IntegerValue,
		IntegerRange,
		FloatRange,
		SettingList,
		Randomize,
		RandomMin,
		RandomMax,
		Value,
		ValueMin,
		ValueMax,
		Weight,
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
	public class Settings
	{
		private List<Setting> settings;
		private Dictionary<string, Setting> fullsettings;
		string flagstringtext;
		public int Init()
		{
			settings = new();
			fullsettings = new();
			settings.Add(new Setting("Free Lute"));

			settings.Add(new Setting("ThiefMode", typeof(ThiefOptions), (int)ThiefOptions.Lockpicking));

			settings.Add(new Setting("Bugfixes"));

			settings.Find(s => s.Name == "Bugfixes").CurrentValue = 1;
			settings.Find(s => s.Name == "Free Lute").Randomize = true;
			//settings.Find(s => s.Name == "ThiefMode").CurrentValue = (int)ThiefAGI.Agi120;

			var content = JsonConvert.SerializeObject(settings);
			return 0;
		}
		public void SetStandardFlags()
		{
			settings.Add(new Setting("Bugfixes", 1));
			settings.Add(new Setting("Bugfixes", 1));

		}
		public void GenerateFlagstring()
		{
			BigInteger sum = 0;
			settings = settings.OrderBy(s => s.Name).ToList();

			foreach (var setting in settings)
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
		}
		public void ReadFlagstring(string flagstring)
		{
			BigInteger sum = StringToBigInteger(flagstring);
			settings = settings.OrderByDescending(s => s.Name).ToList();

			foreach (var setting in settings)
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
		public int CollapseRandomSettings(MT19337 rng)
		{
			foreach (var setting in settings)
			{
				setting.CollapseValue(rng);
				fullsettings.Add(setting.Name, setting);
			}

			return 0;
		}
		public bool GetBool(string name)
		{
			if (fullsettings.TryGetValue(name, out var value))
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
			if (fullsettings.TryGetValue(name, out var result))
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
			if (fullsettings.TryGetValue(name, out var result))
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
			if (fullsettings.TryGetValue(name, out var result))
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
			if (fullsettings.TryGetValue(name, out var result))
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
			if (fullsettings.TryGetValue(name, out var result))
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
			if (fullsettings.TryGetValue(name, out var value))
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
			if (fullsettings.TryGetValue(name, out var value))
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
			if (fullsettings.TryGetValue(name, out var value))
			{
				value.CurrentValue = updatevalue;
			}
			else
			{
				fullsettings.Add(name, new() { Name = name, CurrentValue = updatevalue });
			}
		}
		public int SetValue()
		{
			string content = "[{\"Name\":\"Free Lute\",\"Type\":1,\"Randomize\":true,\"CurrentValue\":0,\"Options\":[{\"Name\":\"False\",\"Value\":0,\"Weight\":1},{\"Name\":\"True\",\"Value\":1,\"Weight\":1}]},{\"Name\":\"Thief AGI Buff\",\"Type\":2,\"Randomize\":true,\"CurrentValue\":0,\"Options\":[{\"Name\":\"10 (Vanilla)\",\"Value\":0,\"Weight\":1},{\"Name\":\"80\",\"Value\":1,\"Weight\":1},{\"Name\":\"100\",\"Value\":2,\"Weight\":1},{\"Name\":\"120\",\"Value\":3,\"Weight\":1},{\"Name\":\"30\",\"Value\":4,\"Weight\":1},{\"Name\":\"50\",\"Value\":5,\"Weight\":1}]}]";
			settings = JsonConvert.DeserializeObject<List<Setting>>(content);


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
				if (flagrule.Type == SettingType.Toggle)
				{
					if ((GetBool(flagrule.Name) && flagrule.Value == 1) || (!GetBool(flagrule.Name) && flagrule.Value == 0))
					{
						foreach (var rule in flagrule.Actions)
						{
							ProcessRule(flagrule, 0);
						}
					}
				}
				else if (flagrule.Type == SettingType.OptionList || flagrule.Type == SettingType.IntegerRange)
				{
					if (TryGetInt(flagrule.Name, out var value))
					{
						if (value == flagrule.Value)
						{
							foreach (var rule in flagrule.Actions)
							{
								ProcessRule(flagrule, value);
							}
						}
					}
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
		public string Name { get; set; }
		public SettingType Type { get; set; }
		public int Value { get; set; }
		public List<FlagAction> Actions { get; set; }
	}
	public struct FlagAction
	{
		public FlagActions Action { get; set; }
		public int Value { get; set; }
		public string Setting { get; set; }
		public Type Type { get; set; }
	}

	// Website [strings] > Convert to Flags/Expand settings
	// flagstring > Convert to Flags > Expand settings
	public static partial class FlagRules
	{
		/*
		private static List<FlagRule> Rules = new()
		{
			new FlagRule() { Name = "Bugfixes", Type = typeof(bool), Settings = new List<FlagAction>()
			{
				new FlagAction() { Setting = "FixHouseMP", Action = FlagActions.Enable },
				new FlagAction() { Setting = "FixHouseHP", Action = FlagActions.Enable },
				new FlagAction() { Setting = "FixWeaponStats", Action = FlagActions.Enable },
				new FlagAction() { Setting = "FixSpellBugs", Action = FlagActions.Enable },
				new FlagAction() { Setting = "FixEnemyStatusAttack", Action = FlagActions.Enable },
				new FlagAction() { Setting = "FixBBAbsorbBug", Action = FlagActions.Enable },
			} },
		};
		private static Dictionary<string, FlagRule> RulesDict = new()
		{
			{ "Bugfixes", new FlagRule() { Name = "Bugfixes", Type2 = SettingType.Toggle, Settings = new List<FlagAction>()
				{
					new FlagAction() { Setting = "FixHouseMP", Action = FlagActions.Enable },
					new FlagAction() { Setting = "FixHouseHP", Action = FlagActions.Enable },
					new FlagAction() { Setting = "FixWeaponStats", Action = FlagActions.Enable },
					new FlagAction() { Setting = "FixSpellBugs", Action = FlagActions.Enable },
					new FlagAction() { Setting = "FixEnemyStatusAttack", Action = FlagActions.Enable },
					new FlagAction() { Setting = "FixBBAbsorbBug", Action = FlagActions.Enable },
				} }
			},
			{ "StartingGold", new FlagRule() { Name = "StartingGold", Type2 = SettingType.OptionList, Settings = new List<FlagAction>()
				{
					new FlagAction() { Setting = "StartingGold", Action = FlagActions.ReadOption, Type = typeof(StartingGold) },
				} }
			}
		};*/
	}
}
