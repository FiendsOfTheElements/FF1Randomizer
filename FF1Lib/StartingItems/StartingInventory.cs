using System.ComponentModel;

namespace FF1Lib
{
	public enum StartingItemSet
	{
		[Description("None")]
		None,

		[Description("SafetyBit")]
		SafetyBit,

		[Description("Beggar's Choice")]
		BeggarsChoice,

		[Description("Just Tents")]
		JustTents,

		[Description("Explorer Pack")]
		ExplorerStartingPack,

		[Description("Real Estate")]
		RealEstate,

		[Description("Warrior's Standard")]
		WarriorsStandard,

		[Description("Royal Packmule")]
		RoyalPackmule,

		[Description("Duckling Boon")]
		DucklingBoon,

		[Description("R. Beggar's Choice")]
		RandomizedBeggarsChoice,

		[Description("R. Explorer Pack")]
		RandomizedExplorerStartingPack,

		[Description("R. Warrior's Standard")]
		RandomizedWarriorsStandard,

		[Description("R. Royal Packmule")]
		RandomizedRoyalPackmule,

		[Description("R. High Rolling")]
		RandomizedHighRolling,

		[Description("Randomized")]
		Randomized,
	}

	public enum ExtStartingItemSet
	{
		[Description("None")]
		None,

		[Description("Lifeline")]
		Lifeline,

		[Description("Just Smokes")]
		JustSmokes,

		[Description("Bangs and Smokes")]
		BangsAndSmokes,

		[Description("Jester's Secret")]
		JestersSecret,

		[Description("Adventurer's Fortune")]
		AdventurersFortune,

		[Description("Blackbelt's Favorite")]
		BlackbeltsFavorite,

		[Description("Illegal Advantage")]
		IllegalAdvantage,

		[Description("R. Jester's Secret")]
		RandomizedJestersSecret,

		[Description("R. Adventurer's Fortune")]
		RandomizedAdventurersFortune,

		[Description("R. Blackbelt's Favorite")]
		RandomizedBlackbeltsFavorite,

		[Description("R. High Rolling")]
		RandomizedHighRolling,

		[Description("Randomized")]
		Randomized,
	}

	public class StartingInventory
	{
		MT19337 rng;
		Flags flags;
		FF1Rom rom;

		StartingItems ItemData;

		public StartingInventory(MT19337 _rng, Flags _flags, FF1Rom _rom)
		{
			rng = _rng;
			flags = _flags;
			rom = _rom;

			ItemData = new StartingItems(rom);
		}

		public void SetStartingInventory()
		{
			ItemData.LoadTable();

			foreach (var e in StartingItemSetDic[flags.StartingItemSet])
			{
				if (e.Cnt.HasValue)
				{
					ItemData[e.Item] = (byte)e.Cnt;
				}
				else
				{
					var min = e.Min;
					var max = e.Max;
					var rmin = e.RMin ?? min;
					var rmax = e.RMax ?? max;

					ItemData[e.Item] = (byte)Math.Min(Math.Max(rng.Between(rmin, rmax), min), max);
				}
			}

			if (flags.ExtConsumableSet != ExtConsumableSet.None)
			{
				foreach (var e in ExtStartingItemSetDic[flags.ExtStartingItemSet])
				{
					if (e.Cnt.HasValue)
					{
						ItemData[e.Item] = (byte)e.Cnt;
					}
					else
					{
						var min = e.Min;
						var max = e.Max;
						var rmin = e.RMin ?? min;
						var rmax = e.RMax ?? max;

						ItemData[e.Item] = (byte)Math.Min(Math.Max(rng.Between(rmin, rmax), min), max);
					}
				}
			}

			ItemData.StoreTable();
		}

		public List<(Item item, int qty)> ReturnStartingInventory()
		{
			List<(Item item, int qty)> itemList = new();

			foreach (var e in StartingItemSetDic[flags.StartingItemSet])
			{
				if (e.Cnt.HasValue)
				{
					itemList.Add((e.Item, (int)e.Cnt));
				}
				else
				{
					var min = e.Min;
					var max = e.Max;
					var rmin = e.RMin ?? min;
					var rmax = e.RMax ?? max;

					itemList.Add((e.Item, (int)Math.Min(Math.Max(rng.Between(rmin, rmax), min), max)));
				}
			}

			if (flags.ExtConsumableSet != ExtConsumableSet.None)
			{
				foreach (var e in ExtStartingItemSetDic[flags.ExtStartingItemSet])
				{
					if (e.Cnt.HasValue)
					{
						itemList.Add((e.Item, (int)e.Cnt));
					}
					else
					{
						var min = e.Min;
						var max = e.Max;
						var rmin = e.RMin ?? min;
						var rmax = e.RMax ?? max;

						itemList.Add((e.Item, (int)Math.Min(Math.Max(rng.Between(rmin, rmax), min), max)));
					}
				}
			}

			return itemList;
		}
		private class StartingItem
		{
			public Item Item { get; set; }

			public int? Cnt { get; set; }

			public int Min { get; set; }

			public int Max { get; set; }

			public int? RMin { get; set; }

			public int? RMax { get; set; }
		}

		private static Dictionary<StartingItemSet, StartingItem[]> StartingItemSetDic = new Dictionary<StartingItemSet, StartingItem[]>
		{
			//{ StartingItemSet.None, Array.Empty<StartingItem>() },
			{ StartingItemSet.None, new StartingItem[]
				{
				}
			},
			{ StartingItemSet.SafetyBit, new StartingItem[]
				{
					new StartingItem { Item = Item.Tent, Cnt = 1 },
					new StartingItem { Item = Item.Pure, Cnt = 1 },
					new StartingItem { Item = Item.Soft, Cnt = 1 },
				}
			},
			{ StartingItemSet.BeggarsChoice, new StartingItem[]
				{
					new StartingItem { Item = Item.Tent, Cnt = 5 },
					new StartingItem { Item = Item.Heal, Cnt = 20 },
					new StartingItem { Item = Item.Pure, Cnt = 2 },
					new StartingItem { Item = Item.Soft, Cnt = 1 },
				}
			},
			{ StartingItemSet.JustTents, new StartingItem[]
				{
					new StartingItem { Item = Item.Tent, Cnt = 10 },
				}
			},
			{ StartingItemSet.ExplorerStartingPack, new StartingItem[]
				{
					new StartingItem { Item = Item.Tent, Cnt = 10 },
					new StartingItem { Item = Item.Heal, Cnt = 50 },
					new StartingItem { Item = Item.Pure, Cnt = 4 },
					new StartingItem { Item = Item.Soft, Cnt = 2 },
				}
			},
			{ StartingItemSet.RealEstate, new StartingItem[]
				{
					new StartingItem { Item = Item.Tent, Cnt = 15 },
					new StartingItem { Item = Item.Cabin, Cnt = 5 },
					new StartingItem { Item = Item.House, Cnt = 1 },
				}
			},
			{ StartingItemSet.WarriorsStandard, new StartingItem[]
				{
					new StartingItem { Item = Item.Tent, Cnt = 15 },
					new StartingItem { Item = Item.Cabin, Cnt = 5 },
					new StartingItem { Item = Item.House, Cnt = 1 },
					new StartingItem { Item = Item.Heal, Cnt = 50 },
					new StartingItem { Item = Item.Pure, Cnt = 6 },
					new StartingItem { Item = Item.Soft, Cnt = 3 },
				}
			},
			{ StartingItemSet.RoyalPackmule, new StartingItem[]
				{
					new StartingItem { Item = Item.Tent, Cnt = 50 },
					new StartingItem { Item = Item.Cabin, Cnt = 15 },
					new StartingItem { Item = Item.House, Cnt = 5 },
					new StartingItem { Item = Item.Heal, Cnt = 99 },
					new StartingItem { Item = Item.Pure, Cnt = 10 },
					new StartingItem { Item = Item.Soft, Cnt = 5 },
				}
			},
			{ StartingItemSet.DucklingBoon, new StartingItem[]
				{
					new StartingItem { Item = Item.Tent, Cnt = 99 },
					new StartingItem { Item = Item.Cabin, Cnt = 99 },
					new StartingItem { Item = Item.House, Cnt = 99 },
					new StartingItem { Item = Item.Heal, Cnt = 99 },
					new StartingItem { Item = Item.Pure, Cnt = 99 },
					new StartingItem { Item = Item.Soft, Cnt = 99 },
				}
			},
			{ StartingItemSet.RandomizedBeggarsChoice, new StartingItem[]
				{
					new StartingItem { Item = Item.Tent, Min = 1, Max = 5, RMin = 0, RMax = 6 },
					new StartingItem { Item = Item.Heal, Min = 0, Max = 20 },
					new StartingItem { Item = Item.Pure, Min = 0, Max = 2 },
					new StartingItem { Item = Item.Soft, Min = 0, Max = 1 },
				}
			},
			{ StartingItemSet.RandomizedExplorerStartingPack, new StartingItem[]
				{
					new StartingItem { Item = Item.Tent, Min = 1, Max = 10, RMin = -2, RMax = 12 },
					new StartingItem { Item = Item.Heal, Min = 0, Max = 50 },
					new StartingItem { Item = Item.Pure, Min = 0, Max = 4 },
					new StartingItem { Item = Item.Soft, Min = 0, Max = 2 },
				}
			},
			{ StartingItemSet.RandomizedWarriorsStandard, new StartingItem[]
				{
					new StartingItem { Item = Item.Tent, Min = 3, Max = 15 },
					new StartingItem { Item = Item.Cabin, Min = 0, Max = 3 },
					new StartingItem { Item = Item.House, Min = 0, Max = 1 },
					new StartingItem { Item = Item.Heal, Min = 20, Max = 50 },
					new StartingItem { Item = Item.Pure, Min = 2, Max = 6 },
					new StartingItem { Item = Item.Soft, Min = 1, Max = 3 },
				}
			},
			{ StartingItemSet.RandomizedRoyalPackmule, new StartingItem[]
				{
					new StartingItem { Item = Item.Tent, Min = 10, Max = 50 },
					new StartingItem { Item = Item.Cabin, Min = 5, Max = 15 },
					new StartingItem { Item = Item.House, Min = 1, Max = 5 },
					new StartingItem { Item = Item.Heal, Min = 30, Max = 99 },
					new StartingItem { Item = Item.Pure, Min = 5, Max = 10 },
					new StartingItem { Item = Item.Soft, Min = 2, Max = 5 },
				}
			},
			{ StartingItemSet.RandomizedHighRolling, new StartingItem[]
				{
					new StartingItem { Item = Item.Tent, Min = 30, Max = 99 },
					new StartingItem { Item = Item.Cabin, Min = 10, Max = 25 },
					new StartingItem { Item = Item.House, Min = 5, Max = 10 },
					new StartingItem { Item = Item.Heal, Min = 30, Max = 99 },
					new StartingItem { Item = Item.Pure, Min = 10, Max = 20 },
					new StartingItem { Item = Item.Soft, Min = 5, Max = 10 },
				}
			},
			{ StartingItemSet.Randomized, new StartingItem[]
				{
					new StartingItem { Item = Item.Tent, Min = 0, Max = 99, RMin = -30, RMax = 114 },
					new StartingItem { Item = Item.Cabin, Min = 0, Max = 99, RMin = -30, RMax = 114 },
					new StartingItem { Item = Item.House, Min = 0, Max = 99, RMin = -30, RMax = 114 },
					new StartingItem { Item = Item.Heal, Min = 0, Max = 99, RMin = -15, RMax = 114 },
					new StartingItem { Item = Item.Pure, Min = 0, Max = 20, RMin = -3, RMax = 23 },
					new StartingItem { Item = Item.Soft, Min = 0, Max = 10, RMin = -2, RMax = 12 },
				}
			},
		};

		private static Dictionary<ExtStartingItemSet, StartingItem[]> ExtStartingItemSetDic = new Dictionary<ExtStartingItemSet, StartingItem[]>
		{
			//{ StartingItemSet.None, Array.Empty<StartingItem>() },
			{ ExtStartingItemSet.None, new StartingItem[]
				{
				}
			},
			{ ExtStartingItemSet.Lifeline, new StartingItem[]
				{
					new StartingItem { Item = Item.WoodenNunchucks, Cnt = 1 },
					new StartingItem { Item = Item.SmallKnife, Cnt = 1 },
				}
			},
			{ ExtStartingItemSet.JustSmokes, new StartingItem[]
				{
					new StartingItem { Item = Item.Rapier, Cnt = 5 },
				}
			},
			{ ExtStartingItemSet.BangsAndSmokes, new StartingItem[]
				{
					new StartingItem { Item = Item.Rapier, Cnt = 10 },
					new StartingItem { Item = Item.WoodenRod, Cnt = 10 },
				}
			},
			{ ExtStartingItemSet.JestersSecret, new StartingItem[]
				{
					new StartingItem { Item = Item.WoodenNunchucks, Cnt = 2 },
					new StartingItem { Item = Item.SmallKnife, Cnt = 1 },
					new StartingItem { Item = Item.Rapier, Cnt = 5 },
					new StartingItem { Item = Item.WoodenRod, Cnt = 5 },
				}
			},
			{ ExtStartingItemSet.AdventurersFortune, new StartingItem[]
				{
					new StartingItem { Item = Item.WoodenNunchucks, Cnt = 3 },
					new StartingItem { Item = Item.SmallKnife, Cnt = 2 },
					new StartingItem { Item = Item.Rapier, Cnt = 10 },
					new StartingItem { Item = Item.WoodenRod, Cnt = 10 },
				}
			},
			{ ExtStartingItemSet.BlackbeltsFavorite, new StartingItem[]
				{
					new StartingItem { Item = Item.WoodenNunchucks, Cnt = 5 },
					new StartingItem { Item = Item.SmallKnife, Cnt = 3 },
					new StartingItem { Item = Item.Rapier, Cnt = 20 },
					new StartingItem { Item = Item.WoodenRod, Cnt = 20 },
				}
			},
			{ ExtStartingItemSet.IllegalAdvantage, new StartingItem[]
				{
					new StartingItem { Item = Item.WoodenNunchucks, Cnt = 99 },
					new StartingItem { Item = Item.SmallKnife, Cnt = 99 },
					new StartingItem { Item = Item.Rapier, Cnt = 99 },
					new StartingItem { Item = Item.WoodenRod, Cnt = 99 },
				}
			},
			{ ExtStartingItemSet.RandomizedJestersSecret, new StartingItem[]
				{
					new StartingItem { Item = Item.WoodenNunchucks, Min = 0, Max = 2 },
					new StartingItem { Item = Item.SmallKnife, Min = 0, Max = 1 },
					new StartingItem { Item = Item.Rapier, Min = 0, Max = 5 },
					new StartingItem { Item = Item.WoodenRod, Min = 0, Max = 5 },
				}
			},
			{ ExtStartingItemSet.RandomizedAdventurersFortune, new StartingItem[]
				{
					new StartingItem { Item = Item.WoodenNunchucks, Min = 0, Max = 3 },
					new StartingItem { Item = Item.SmallKnife, Min = 0, Max = 2 },
					new StartingItem { Item = Item.Rapier, Min = 0, Max = 10 },
					new StartingItem { Item = Item.WoodenRod, Min = 0, Max = 10 },
				}
			},
			{ ExtStartingItemSet.RandomizedBlackbeltsFavorite, new StartingItem[]
				{
					new StartingItem { Item = Item.WoodenNunchucks, Min = 0, Max = 5 },
					new StartingItem { Item = Item.SmallKnife, Min = 1, Max = 3 },
					new StartingItem { Item = Item.Rapier, Min = 0, Max = 20 },
					new StartingItem { Item = Item.WoodenRod, Min = 0, Max = 20 },
				}
			},
			{ ExtStartingItemSet.RandomizedHighRolling, new StartingItem[]
				{
					new StartingItem { Item = Item.WoodenNunchucks, Min = 2, Max = 5 },
					new StartingItem { Item = Item.SmallKnife, Min = 2, Max = 5 },
					new StartingItem { Item = Item.Rapier, Min = 5, Max = 30 },
					new StartingItem { Item = Item.WoodenRod, Min = 5, Max = 30 },
				}
			},
			{ ExtStartingItemSet.Randomized, new StartingItem[]
				{
					new StartingItem { Item = Item.WoodenNunchucks, Min = 0, Max = 15, RMin = -3, RMax = 15 },
					new StartingItem { Item = Item.SmallKnife, Min = 0, Max = 10, RMin = -3, RMax = 10 },
					new StartingItem { Item = Item.Rapier, Min = 0, Max = 50 },
					new StartingItem { Item = Item.WoodenRod, Min = 0, Max = 50 },
				}
			},
		};
	}
}
