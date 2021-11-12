using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FF1Lib.Sanity
{
	public class ArchipelagoExporter
	{
		public const int ItemOffset = 0x100;
		public const int ChestOffset = 0x100;
		public const int NpcOffset = 0x200;
		public const int ShopSlot = 0x2FF;

		SCLogic logic;
		IVictoryConditionFlags flags;
		Preferences preferences;

		public string Json { get; private set; }

		public ArchipelagoExporter(FF1Rom rom, List<IRewardSource> generatedPlacement, SanityCheckerV2 checker, IVictoryConditionFlags _flags, Preferences _preferences)
		{
			flags = _flags;
			preferences = _preferences;

			var kiPlacement = generatedPlacement.Where(r => ItemLists.AllQuestItems.Contains(r.Item)).ToList();

			logic = new SCLogic(rom, checker.Main, kiPlacement, flags);
		}

		public string Work()
		{
			var data = new ArchipelagoOptions
			{
				game = "Final Fantasy",
				description = "Hurray",
				name = preferences.PlayerName,
				options = new ArchipelagoFFROptions
				{
					items = logic.RewardSources.GroupBy(r => (int)r.RewardSource.Item + ItemOffset).ToDictionary(r => r.First().RewardSource.Item.ToString(), r => new ArchipelagoItem { id = r.Key, count = r.Count() }),
					locations = logic.RewardSources.ToDictionary(r => r.RewardSource.Name, r => GetLocationId(r)),
					rules = logic.RewardSources.ToDictionary(r => r.RewardSource.Name, r => GetRule(r))
				}
			};

			Json = JsonConvert.SerializeObject(data, Formatting.Indented);

			return Json;
		}

		private int GetLocationId(SCLogicRewardSource r)
		{
			if (r.RewardSource is TreasureChest)
			{
				return r.RewardSource.Address - 0x3100 + ChestOffset;
			}
			else if (r.RewardSource is MapObject npc)
			{
				return (int)npc.ObjectId + NpcOffset;
			}
			else if(r.RewardSource is ItemShopSlot shop)
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
			if (l.HasFlag(SCRequirements.Canoe)) list.Add(Item.Canoe.ToString());
			if (l.HasFlag(SCRequirements.Floater)) list.Add(Item.Floater.ToString());
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
	}

	public class ArchipelagoOptions
	{
		public string game { get; set; }

		public string description { get; set; }

		public string name { get; set; }

		[JsonProperty("Final Fantasy")]
		public ArchipelagoFFROptions options { get; set; }
	}
	public class ArchipelagoFFROptions
	{
		public Dictionary<string, ArchipelagoItem> items { get; set; }

		public Dictionary<string, int> locations { get; set; }

		public Dictionary<string, List<List<string>>> rules { get; set; }
	}

	public class ArchipelagoItem
	{
		public int id { get; set; }

		public int count { get; set; }
	}
}
