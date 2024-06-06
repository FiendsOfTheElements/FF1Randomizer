using FF1Lib.Sanity;
using Newtonsoft.Json;
using System.ComponentModel;
using System.Diagnostics;
using FF1Lib.archipelago;
using Newtonsoft.Json.Linq;

namespace FF1Lib
{
	public class Archipelago
	{
		public const int ItemOffset = 0x100;
		public const int ChestOffset = 0x100;
		public const int NpcOffset = 0x200;
		public const int ShopSlot = 0x2FF;

		FF1Rom rom;
		SCLogic logic;
		Flags flags;
		Flags originalFlags;
		Preferences preferences;
		ExpChests expChests;
		PlacementContext incentivesData;
		OwLocationData locations;
		ItemPlacement itemPlacement;
		Blob seed;

		public string Json { get; private set; }

		public Archipelago(FF1Rom _rom, ItemPlacement _itemPlacement, SanityCheckerV2 checker, ExpChests _expChests, PlacementContext _incentivesData, OwLocationData _locations, Blob _seed, Flags _flags, Flags _originflags, Preferences _preferences)
		{
			rom = _rom;
			expChests = _expChests;
			incentivesData = _incentivesData;
			itemPlacement = _itemPlacement;
			flags = _flags;
			originalFlags = _originflags;
			seed = _seed;
			locations = _locations;
			preferences = _preferences;
			var generatedPlacement = itemPlacement.PlacedItems;

			var kiPlacement = generatedPlacement.Where(r => ItemLists.AllQuestItems.Contains(r.Item) && r.Item != Item.Bridge).ToList();

			if (flags.ArchipelagoConsumables)
			{
				if (flags.ExtConsumablesEnabled)
				{
					kiPlacement.AddRange(generatedPlacement.Where(r => r.Item >= Item.Tent && r.Item <= Item.Rapier));
				}
				else
				{
					kiPlacement.AddRange(generatedPlacement.Where(r => r.Item >= Item.Tent && r.Item <= Item.Soft));
				}
			}

			if (flags.ArchipelagoShards)
			{
				kiPlacement.AddRange(generatedPlacement.Where(r => r.Item == Item.Shard));
			}

			if (flags.ArchipelagoGold)
			{
				kiPlacement.AddRange(generatedPlacement.Where(r => r.Item >= Item.Gold10 && r.Item < expChests.FirstExpItem));
			}

			switch (flags.ArchipelagoEquipment)
			{
				case ArchipelagoEquipment.Common:
					AddCommonEquipment(kiPlacement, generatedPlacement);
					AddRareEquipment(kiPlacement, generatedPlacement);
					AddLegendaryEquipment(kiPlacement, generatedPlacement);
					AddCasterEquipment(kiPlacement, generatedPlacement);
					break;
				case ArchipelagoEquipment.Rare:
					AddRareEquipment(kiPlacement, generatedPlacement);
					AddLegendaryEquipment(kiPlacement, generatedPlacement);
					AddCasterEquipment(kiPlacement, generatedPlacement);
					break;
				case ArchipelagoEquipment.Legendary:
					AddLegendaryEquipment(kiPlacement, generatedPlacement);
					AddCasterEquipment(kiPlacement, generatedPlacement);
					break;
				case ArchipelagoEquipment.CasterItems:
					AddCasterEquipment(kiPlacement, generatedPlacement);
					break;
				case ArchipelagoEquipment.Incentivized:
					AddIncentivizedEquipment(kiPlacement, generatedPlacement);
					break;
			}

			//Remove ToFr and distinct by address to remove duplicates
			kiPlacement = kiPlacement.Where(r => !ItemLocations.ToFR.Any(l => l.Address == r.Address)).GroupBy(r => r.Address).Select(g => g.First()).ToList();

			logic = new SCLogic(rom, checker.Main, kiPlacement, locations, flags, true);
		}

		private void AddCommonEquipment(List<IRewardSource> kiPlacement, List<IRewardSource> generatedPlacement)
		{
			HashSet<Item> items = new HashSet<Item>(ItemLists.CommonWeaponTier.Concat(ItemLists.CommonArmorTier));

			if (flags.ExtConsumablesEnabled)
			{
				items.Remove(Item.WoodenNunchucks);
				items.Remove(Item.SmallKnife);
				items.Remove(Item.WoodenRod);
				items.Remove(Item.Rapier);
			}

			kiPlacement.AddRange(generatedPlacement.Where(r => items.Contains(r.Item)));
		}

		private void AddRareEquipment(List<IRewardSource> kiPlacement, List<IRewardSource> generatedPlacement)
		{
			HashSet<Item> items = new HashSet<Item>(ItemLists.RareWeaponTier.Concat(ItemLists.RareArmorTier));
			kiPlacement.AddRange(generatedPlacement.Where(r => items.Contains(r.Item)));
		}

		private void AddLegendaryEquipment(List<IRewardSource> kiPlacement, List<IRewardSource> generatedPlacement)
		{
			HashSet<Item> items = new HashSet<Item>(ItemLists.LegendaryWeaponTier.Concat(ItemLists.LegendaryArmorTier).Concat(ItemLists.UberTier));
			kiPlacement.AddRange(generatedPlacement.Where(r => items.Contains(r.Item)));
		}

		private void AddCasterEquipment(List<IRewardSource> kiPlacement, List<IRewardSource> generatedPlacement)
		{
			var weapons = Weapon.LoadAllWeapons(rom, flags).Where(w => w.Spell != Spell.None).ToList();
			var armors = Armor.LoadAllArmors(rom, flags).Where(a => a.Spell != Spell.None).ToList();

			HashSet<Item> items = new HashSet<Item>(weapons.Select(w => w.Id).Concat(armors.Select(a => a.Id)));
			kiPlacement.AddRange(generatedPlacement.Where(r => items.Contains(r.Item)));
		}

		private void AddIncentivizedEquipment(List<IRewardSource> kiPlacement, List<IRewardSource> generatedPlacement)
		{
			HashSet<Item> items = new HashSet<Item>(incentivesData.IncentiveItems.Where(i => !ItemLists.AllQuestItems.Any(k => k == i)));

			var incentivizedGearInIncentivizedLocations = generatedPlacement.Where(r => items.Contains(r.Item) && incentivesData.IncentiveLocations.Any(l => l.Address == r.Address)).ToList();

			items.RemoveWhere(i => incentivizedGearInIncentivizedLocations.Any(r => r.Item == i));

			var oneInstanceOfEachOfTheRemainingItems = generatedPlacement.Where(r => items.Contains(r.Item)).GroupBy(r => r.Item).Select(g => g.First()).ToList();

			kiPlacement.AddRange(incentivizedGearInIncentivizedLocations);
			kiPlacement.AddRange(oneInstanceOfEachOfTheRemainingItems);
		}

		public string Work()
		{
			rom.ItemsText[(int)Item.FireOrb] = "AP Item";

			var apLocationNames = JObject
				.Parse(Encoding.UTF8.GetString(ApResources.locations))
				.ToObject<Dictionary<string, int>>()
				.ToDictionary(l => l.Value, l => l.Key);

			foreach (var rewardSource in logic.RewardSources)
			{
				if (itemPlacement.PlacedItems.TryFind(r => r.Address == rewardSource.RewardSource.Address, out var placeditem))
				{
					var index = itemPlacement.PlacedItems.FindIndex(r => r.Address == rewardSource.RewardSource.Address);

					if (placeditem.GetType() == typeof(TreasureChest))
					{
						itemPlacement.PlacedItems[index] = new TreasureChest(placeditem, Item.FireOrb);
					}
					else if (placeditem.GetType() == typeof(NpcReward))
					{
						itemPlacement.PlacedItems[index] = new NpcReward(placeditem, Item.FireOrb);
					}
					else if (placeditem.GetType() == typeof(ItemShopSlot))
					{
						itemPlacement.PlacedItems[index] = new ItemShopSlot((ItemShopSlot)placeditem, Item.FireOrb);
					}
				}
			}

			var data = new ArchipelagoOptions
			{
				game = "Final Fantasy",
				description = "Hurray",
				name = (preferences.PlayerName.Length > 16) ? preferences.PlayerName.Substring(0,16) : preferences.PlayerName,
				permalink = ((FFRVersion.Branch == "master") ? FFRVersion.Version.Replace('.', '-') : "beta-" + FFRVersion.Sha.PadRight(8).Substring(0, 8)) + ".finalfantasyrandomizer.com/?s=" + seed.ToHex() + "&f=" + Flags.EncodeFlagsText(originalFlags),
				options = new ArchipelagoFFROptions
				{
					items = logic.RewardSources.GroupBy(r => GetItemId(r.RewardSource.Item)).ToDictionary(r => GetItemName(r.First().RewardSource.Item), r => new ArchipelagoItem { id = r.Key, count = r.Count(), incentive = incentivesData.IncentiveItems.Contains(r.First().RewardSource.Item) }),
					locations = logic.RewardSources.ToDictionary(
						r => apLocationNames.TryGetValue(GetLocationId(r), out var locname) ? locname : r.RewardSource.Name,
						r => GetLocationId(r)),
					locations2 = logic.RewardSources.ToDictionary(
						r => apLocationNames.TryGetValue(GetLocationId(r), out var locname) ? locname : r.RewardSource.Name,
						r => new ArchipelagoLocation { id = GetLocationId(r), incentive = IsLocationIncentivized(r) }),
					rules = logic.RewardSources.ToDictionary(
						r => apLocationNames.TryGetValue(GetLocationId(r), out var locname) ? locname : r.RewardSource.Name,
						r => GetRule(r))
				}
			};

			Json = JsonConvert.SerializeObject(data, Formatting.Indented);

			//Write PlayerName into Rom
			var playerName = LimitByteLength((preferences.PlayerName.Length > 16) ? preferences.PlayerName.Substring(0, 16) : preferences.PlayerName, 0x40);
			byte[] buffer = Encoding.UTF8.GetBytes(playerName);
			Debug.Assert(buffer.Length <= 0x40, "PlayerName wasn't shortened correctly.");

			rom.PutInBank(0x1E, 0xBCC0, buffer);

			//Clear false Initialization of Archipelago Counters
			rom.PutInBank(0x00, 0xB00B, new byte[] { 0 });
			rom.PutInBank(0x00, 0xB00F, new byte[] { 0 });

			//ChaosDeath Breakout
			rom.PutInBank(0x0B, 0x9ADF, Blob.FromHex("20409B"));
			rom.PutInBank(0x0B, 0x9B40, Blob.FromHex("A0FEB9006209029900624C52A0"));

			//CopyEquipToItemBox
			rom.PutInBank(0x0E, 0xBD9D, Blob.FromHex("A666A000BD0061990003E8C88A2903D0F38A18693CAA90ECC918F006AD0B6085FC60AD076085FC60"));
			rom.PutInBank(0x0E, 0xBDC8, Blob.FromHex("A666A000B900039D0061E8C88A2903D0F38A18693CAA90ECC918F006A5FC8D0B6060A5FC8D076060"));

			return Json;
		}

		private bool IsLocationIncentivized(SCLogicRewardSource r)
		{
			if (r.RewardSource is ItemShopSlot && incentivesData.IncentiveLocations.Contains(ItemLocations.CaravanItemShop1)) return true;
			return incentivesData.IncentiveLocations.Any(i => i.Address == r.RewardSource.Address);
		}

		public static string LimitByteLength(string input, int maxLength)
		{
			return new string(input
				.TakeWhile((c, i) =>
					Encoding.UTF8.GetByteCount(input.Substring(0, i + 1)) <= maxLength)
				.ToArray());
		}


		private int GetLocationId(SCLogicRewardSource r)
		{
			if (r.RewardSource is TreasureChest)
			{
				return r.RewardSource.Address - 0x3100 + ChestOffset;
			}
			else if (r.RewardSource is NpcReward npc)
			{
				return (int)npc.ObjectId + NpcOffset;
			}
			else if (r.RewardSource is ItemShopSlot shop)
			{
				return 0x2FF;
			}

			throw new NotSupportedException();
		}

		private List<List<string>> GetRule(SCLogicRewardSource r)
		{
			return r.Requirements.Select(l => GetRule(l)).ToList();
		}

		private List<string> GetRule(SCRequirements l)
		{
			var list = new List<string>();

			if (l.HasFlag(SCRequirements.Lute)) list.Add(Item.Lute.ToString());
			if (l.HasFlag(SCRequirements.Crown)) list.Add(Item.Crown.ToString());
			if (l.HasFlag(SCRequirements.Key)) list.Add(Item.Key.ToString());
			if (l.HasFlag(SCRequirements.Ruby)) list.Add(Item.Ruby.ToString());
			if (l.HasFlag(SCRequirements.Rod)) list.Add(Item.Rod.ToString());
			if (l.HasFlag(SCRequirements.Chime)) list.Add(Item.Chime.ToString());
			if (l.HasFlag(SCRequirements.Cube)) list.Add(Item.Cube.ToString());
			if (l.HasFlag(SCRequirements.Oxyale)) list.Add(Item.Oxyale.ToString());
			if (l.HasFlag(SCRequirements.Tnt)) list.Add(Item.Tnt.ToString());
			if (l.HasFlag(SCRequirements.Canoe) && !flags.NoOverworld) list.Add(Item.Canoe.ToString());
			if (l.HasFlag(SCRequirements.Canoe) && flags.NoOverworld) list.Add("Mark");
			if (l.HasFlag(SCRequirements.Floater) && !flags.NoOverworld) list.Add(Item.Floater.ToString());
			if (l.HasFlag(SCRequirements.Floater) && flags.NoOverworld) list.Add("Sigil");
			if (l.HasFlag(SCRequirements.Bridge)) list.Add(Item.Bridge.ToString());
			if (l.HasFlag(SCRequirements.Canal)) list.Add(Item.Canal.ToString());
			if (l.HasFlag(SCRequirements.Bottle)) list.Add(Item.Bottle.ToString());
			if (l.HasFlag(SCRequirements.Crystal)) list.Add(Item.Crystal.ToString());
			if (l.HasFlag(SCRequirements.Herb)) list.Add(Item.Herb.ToString());
			if (l.HasFlag(SCRequirements.Adamant)) list.Add(Item.Adamant.ToString());
			if (l.HasFlag(SCRequirements.Slab)) list.Add(Item.Slab.ToString());
			if (l.HasFlag(SCRequirements.Ship)) list.Add(Item.Ship.ToString());

			return list;
		}

		private string GetItemName(Item item)
		{
			if (flags.ExtConsumableSet == ExtConsumableSet.SetA)
			{
				switch (item)
				{
					case Item.WoodenNunchucks: return "FullCure";
					case Item.SmallKnife: return "Phoenix";
					case Item.WoodenRod: return "Blast";
					case Item.Rapier: return "Smoke";
				}
			}
			else if (flags.ExtConsumableSet == ExtConsumableSet.SetB)
			{
				switch (item)
				{
					case Item.WoodenNunchucks: return "Refresh";
					case Item.SmallKnife: return "Flare";
					case Item.WoodenRod: return "Black";
					case Item.Rapier: return "Guard";
				}
			}
			else if (flags.ExtConsumableSet == ExtConsumableSet.SetC)
			{
				switch (item)
				{
					case Item.WoodenNunchucks: return "Quick";
					case Item.SmallKnife: return "HighPotion";
					case Item.WoodenRod: return "Wizard";
					case Item.Rapier: return "Cloak";
				}
			}

			if (flags.GameMode == GameModes.NoOverworld)
			{
				switch (item)
				{
					case Item.Floater: return "Sigil";
					case Item.Canoe: return "Mark";
				}
			}

			return item.ToString();
		}

		private int GetItemId(Item item)
		{
			if (flags.ExtConsumableSet == ExtConsumableSet.SetA)
			{
				switch (item)
				{
					case Item.WoodenNunchucks: return 176 + ItemOffset;
					case Item.SmallKnife: return 177 + ItemOffset;
					case Item.WoodenRod: return 178 + ItemOffset;
					case Item.Rapier: return 179 + ItemOffset;
				}
			}
			else if (flags.ExtConsumableSet == ExtConsumableSet.SetB)
			{
				switch (item)
				{
					case Item.WoodenNunchucks: return 180 + ItemOffset;
					case Item.SmallKnife: return 181 + ItemOffset;
					case Item.WoodenRod: return 182 + ItemOffset;
					case Item.Rapier: return 183 + ItemOffset;
				}
			}
			else if (flags.ExtConsumableSet == ExtConsumableSet.SetC)
			{
				switch (item)
				{
					case Item.WoodenNunchucks: return 184 + ItemOffset;
					case Item.SmallKnife: return 185 + ItemOffset;
					case Item.WoodenRod: return 186 + ItemOffset;
					case Item.Rapier: return 187 + ItemOffset;
				}
			}

			if (flags.GameMode == GameModes.NoOverworld)
			{
				switch (item)
				{
					case Item.Floater: return 243 + ItemOffset;
					case Item.Canoe: return 244 + ItemOffset;
				}
			}

			return (int)item + ItemOffset;
		}
	}

	public class ArchipelagoOptions
	{
		public string game { get; set; }

		public string description { get; set; }
		public string permalink { get; set; }

		public string name { get; set; }

		[JsonProperty("Final Fantasy")]
		public ArchipelagoFFROptions options { get; set; }
	}
	public class ArchipelagoFFROptions
	{
		public Dictionary<string, ArchipelagoItem> items { get; set; }

		public Dictionary<string, int> locations { get; set; }

		public Dictionary<string, ArchipelagoLocation> locations2 { get; set; }

		public Dictionary<string, List<List<string>>> rules { get; set; }
	}

	public class ArchipelagoLocation
	{
		public int id { get; set; }

		public bool incentive { get; set; }
	}

	public class ArchipelagoItem
	{
		public int id { get; set; }

		public int count { get; set; }

		public bool incentive { get; set; }
	}

	public enum ArchipelagoEquipment
	{
		[Description("None")]
		None,

		[Description("All Equipment")]
		Common,

		[Description("Rare, Legendary & Caster Items")]
		Rare,

		[Description("Legendary & Caster Items")]
		Legendary,

		[Description("Caster Items")]
		CasterItems,

		[Description("Incentivized Equipment")]
		Incentivized
	}
}

