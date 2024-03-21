using FF1Lib.Helpers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System.Numerics;
using System.Reflection;
using System.Xml.Linq;

namespace FF1Lib
{
	public class Setting
	{
		protected Setting() { }
		public Setting(string name, Setting parent)
		{
			Name = name;
			Parent = parent;
		}
		public Setting(string name)
		{
			Name = name;
			Parent = null;
		}
		public Setting(string name, List<string> namestrings)
		{
			Name = name;
			Parent = null;
			Put(namestrings);
		}
		public string Name { get; set; }
		protected Setting Parent { get; set; }
		public Setting GetParent()
		{
			return Parent;
		}
		public Setting[] Children
		{
			get
			{
				return ChildrenDict.Values.ToArray();
			}

			set
			{
				ChildrenDict.Clear();
				if (value == null || value.Count() <= 0) return;
				foreach (Setting child in value)
				{
					Add(child);
				}
			}
		}
		protected Dictionary<string, Setting> ChildrenDict { get; } = new Dictionary<string, Setting>();
		public Setting Add(Setting child)
		{
			ChildrenDict.Add(child.Name, child);
			child.Parent = this;
			return child;
		}
		public Setting Get(string namestring)
		{
			string[] names = namestring.Split('/');
			Setting depth = this;

			foreach (var name in names)
			{
				if (depth.ChildrenDict.TryGetValue(name, out var value))
				{
					depth = value;
				}
				else
				{
					return null;
				}
			}

			return depth;
		}
		public bool GetBool(string namestring)
		{
			return Get(namestring) != null;
		}
		public string GetEnum(string namestring, Type type)
		{
			var node = Get(namestring);
			if (node != null)
			{
				foreach (var child in node.Children)
				{
					if (Enum.TryParse(type, child.Name, out var result))
					{
						return child.Name;
					}
				}
			}

			return "";
		}
		public void Put(string namestring)
		{
			string[] names = namestring.Split('/');
			Setting depth = this;

			foreach (var name in names)
			{
				if (depth.ChildrenDict.TryGetValue(name, out var value))
				{
					depth = value;
				}
				else
				{
					Setting newname = new(name, depth);
					depth.Add(newname);
					depth = newname;
				}
			}
		}
		public void Replace(string namestring)
		{
			string[] names = namestring.Split('/');
			Setting depth = this;

			for (int i = 0; i < names.Length - 2; i++)
			{
				if (depth.ChildrenDict.TryGetValue(names[i], out var value))
				{
					depth = value;
				}
				else
				{
					Setting newname = new(names[i], depth);
					depth.Add(newname);
					depth = newname;
				}
			}

			depth.Remove(names[names.Length - 2]);

			for (int i = names.Length - 2; i < names.Length; i++)
			{
				Setting newname = new(names[i], depth);
				depth.Add(newname);
				depth = newname;
			}
		}
		public void Merge(Setting settings)
		{

		}
		public void Put(List<string> namestrings)
		{
			foreach (var namestring in namestrings)
			{
				Put(namestring);
			}
		}
		public bool Remove(string name)
		{
			return ChildrenDict.Remove(name);

		}
	}
	public class Settings : Setting
	{
		public int Init()
		{
			Setting sartingItemSetting = new Setting("StartingItems");
			sartingItemSetting.Add(new Setting("Lute"));
			sartingItemSetting.Add(new Setting("Rod"));

			Add(sartingItemSetting);
			var content = JsonConvert.SerializeObject(this);
			return 0;
		}
		public int SetValue()
		{
			string content = "{\"Name\":\"root\",\"Children\":[{\"Name\":\"StartingItems\",\"Children\":[{\"Name\":\"Lute\",\"Children\":[]},{\"Name\":\"Rod\",\"Children\":[]}]}]}";
			var deserializedcontent = JsonConvert.DeserializeObject<Setting>(content);

			Name = deserializedcontent.Name;
			Children = deserializedcontent.Children;

			return 0;
		}
		public int GetSomething()
		{
			//settings.
			var startingitems = Get("StartingItems");
			List<Item> startwith = new();
			foreach (var item in startingitems.Children)
			{

				startwith.Add(Enum.Parse<Item>(item.Name));

			}

			var someother = Get("Blogu");
			var someotherone = Get("StartingItems/Lute");
			Put("StartingItems/Floater");

			return 0;
		}
		public void ProcessFlags(Flags flags, MT19337 rng)
		{
			Flags collapsedFlags = flags.ShallowCopy();
			collapsedFlags = Flags.ConvertAllTriState(collapsedFlags, rng);

			if ((bool)collapsedFlags.FreeLute)
			{
				Put("StartingItems/Lute");
			}
		}
	}

	public enum FlagActions
	{
		Enable,
		ReadOption,
		SelectOption,
		SelectValue,

	}
	public class NewFlags
	{
		public bool Bugfixes { get; set; } = true;
		public StartingGold StartingGold { get; set; } = StartingGold.Gp400;
		public NewFlags() { }

	}
	public struct FlagRule
	{
		public string Name { get; set; }
		public Type Type { get; set; }
		public List<FlagAction> Settings { get; set; }
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
			{ "Bugfixes", new FlagRule() { Name = "Bugfixes", Type = typeof(bool), Settings = new List<FlagAction>()
				{
					new FlagAction() { Setting = "FixHouseMP", Action = FlagActions.Enable },
					new FlagAction() { Setting = "FixHouseHP", Action = FlagActions.Enable },
					new FlagAction() { Setting = "FixWeaponStats", Action = FlagActions.Enable },
					new FlagAction() { Setting = "FixSpellBugs", Action = FlagActions.Enable },
					new FlagAction() { Setting = "FixEnemyStatusAttack", Action = FlagActions.Enable },
					new FlagAction() { Setting = "FixBBAbsorbBug", Action = FlagActions.Enable },
				} }
			},
			{ "StartingGold", new FlagRule() { Name = "StartingGold", Type = typeof(StartingGold), Settings = new List<FlagAction>()
				{
					new FlagAction() { Setting = "StartingGold", Action = FlagActions.ReadOption, Type = typeof(StartingGold) },
				} }
			}
		};
		private static BigInteger AddNumeric(BigInteger sum, int radix, int value) => sum * radix + value;
		private static void ProcessFlags(Setting settings, NewFlags flags)
		{
			BigInteger flagstring = 0;

			foreach (var rule in Rules)
			{
				if (rule.Type == typeof(bool))
				{
					int value = 0;
					if (settings.GetBool(rule.Name))
					{
						value = 1;
						//settings.Put(rule.Settings);
					}

					AddNumeric(flagstring, 2, value);
				}
				else if (rule.Type.IsEnum)
				{
					string value = settings.GetEnum(rule.Name, rule.Type);
					if (value != "")
					{


					}

				}

			}

		}

		public static void ProcessRule(FlagRule rule, Setting settings, int value)
		{
			foreach (var setting in rule.Settings)
			{
				if (setting.Action == FlagActions.Enable)
				{
					settings.Put(setting.Setting);
				}
				else if (setting.Action == FlagActions.ReadOption)
				{
					settings.Put(setting.Setting + "/" + Enum.GetName(setting.Type, Enum.ToObject(setting.Type, value)));
				}/*
				else if (setting.Action == FlagActions.SelectOption)
				{
					settings.Put(setting.Setting + "/" + Enum.GetName(setting.Type, Enum.ToObject(setting.Type, value)));
				}*/
				else if (setting.Action == FlagActions.SelectValue)
				{
					settings.Put(setting.Setting + "/" + value);
				}
			}
		}

		public static Setting CollectFlags(NewFlags flags)
		{
			var flagprop = typeof(NewFlags).GetProperties(BindingFlags.Instance | BindingFlags.Public).ToList();
			//var rulesprop = typeof(FlagRules).GetProperties(BindingFlags.Instance | BindingFlags.Public).ToList();

			Setting flagsettings = new("root");

			foreach (var flag in flagprop)
			{
				if (Nullable.GetUnderlyingType(flag.PropertyType) == typeof(bool))
				{
					if ((bool)flag.GetValue(flags))
					{
						if (RulesDict.TryGetValue(flag.Name, out var flagrule))
						{
							ProcessRule(flagrule, flagsettings, 0);
						};
					}
				}
				else if (flag.PropertyType == typeof(bool))
				{
					if ((bool)flag.GetValue(flags))
					{
						if (RulesDict.TryGetValue(flag.Name, out var flagrule))
						{
							ProcessRule(flagrule, flagsettings, 0);
						};
					}
				}
				else if (flag.PropertyType.IsEnum || flag.PropertyType == typeof(int))
				{
					if (RulesDict.TryGetValue(flag.Name, out var flagrule))
					{
						ProcessRule(flagrule, flagsettings, (int)flag.GetValue(flags));
					};
				}
			}

			return flagsettings;



			// if it's a bool, true, add, false don't
			// if it's a dropdown, select appropriate setting to add (one setting per option...)

		}

	}
}
