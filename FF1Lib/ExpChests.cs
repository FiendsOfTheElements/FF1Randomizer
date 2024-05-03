using FF1Lib.Data;
using System.ComponentModel;

namespace FF1Lib
{
	public class ExpChests
	{
		public const int BaseExp = 4000;

		public Item FirstExpItem { get; private set; } = (Item)0xB0;

		public enum ChestXPType
		{
			[Description("Random")]
			Random,
			[Description("Random Progressive")]
			RandomProgressive,
			[Description("Linear Progressive")]
			LinearProgressive,
			[Description("Exponential Progressive")]
			ExponentialProgressive
		}

		FF1Rom rom;
		Flags flags;
		MT19337 rng;

		TreasureData treasureData;
		ItemPrices itemPrices;
		ItemNames itemNames;
		List<IRewardSource> placedItems;

		public ExpChests(List<IRewardSource> _placedItems, FF1Rom _rom, Flags _flags, MT19337 _rng)
		{
			rom = _rom;
			flags = _flags;
			rng = _rng;

			placedItems = _placedItems;
			treasureData = new TreasureData(rom);
			itemPrices = new ItemPrices(rom);
		}

		public void BuildExpChests()
		{
			rom.PutInBank(0x1F, 0xDDD0, Blob.FromHex("A9B648A9FF48A9114C03FE8A20DA8760"));

			int expChestCountPercent = rng.Between(flags.ExpChestConversionMin, flags.ExpChestConversionMax);

			var goldChests = placedItems.Where(g => g.Item >= Item.Gold10 && g.Item <= Item.Gold65000 && g.GetType() == typeof(TreasureChest)).Select(g => g.Item).ToList();
			int expChestCount = goldChests.Count() * expChestCountPercent / 100;
			//int expChestCount = treasureData.Data.Where(g => g >= Item.Gold10).Count() * expChestCountPercent / 100;
			double lowScale = (double)flags.ExpChestMinReward / (double)BaseExp;
			double highScale = (double)flags.ExpChestMaxReward / (double)BaseExp;

			if (expChestCount == 0) return;

			LoadData();

			var unusedGoldDic = new HashSet<int>(ItemLists.UnusedGoldItems.Select(x => (int)x));

			// construct a dictionary and get a shuffled index into it.
			var goldItems = ItemLists.AllGoldTreasure.Select(g => (item: g, price: itemPrices[g], name: itemNames[(int)g])).ToList();
			goldItems.Shuffle(rng);
			var goldItemsDic = goldItems.Select((g, i) => (shuffleindex: i, item: g.item, price: g.price, name: g.name)).ToDictionary(g => g.item);

			var expItems = new HashSet<Item>(goldChests
				.Where(g => goldItemsDic.ContainsKey(g) &&  !unusedGoldDic.Contains((int)g))
				.Select(g => (item: g, shuffleindex: goldItemsDic[g].shuffleindex))
				.OrderBy(g => g.shuffleindex)
				.Take(expChestCount)
				.Select(g => g.item)
				.Distinct());

			var firstExpItem = RepackGoldExpItems(goldItemsDic, expItems, unusedGoldDic);

			for (int i = (int)firstExpItem; i < 176; i++)
			{
				if (unusedGoldDic.Contains(i)) continue;

				var e = (Item)i;

				var exp = (ushort)Math.Min(Math.Max(FF1Rom.RangeScale(BaseExp, lowScale, highScale, 1.0, rng), 0), 65535);
				itemPrices[e] = exp;
				itemNames[(int)e] = exp.ToString() + " EXP";
			}

			FirstExpItem = firstExpItem;
			if (flags.Archipelago)
			{
				rom.PutInBank(0x11, 0xB446, new byte[] { (byte)firstExpItem });
			}
			else
			{
				rom.PutInBank(0x11, 0xB447, new byte[] { (byte)firstExpItem });
			}


			var result = treasureData.Data.Where(x => x > Item.Gold10).OrderBy(x => x).Select(x => itemNames[(int)x]).ToList();

			StoreData();
		}

		public void BuildProgressiveExpChests(ChestXPType progressiveChestXPType)
		{
			//In progress
			rom.PutInBank(0x1F, 0xDDD0, Blob.FromHex("A9B648A9FF48A9114C03FE8A20DA8760"));
			rom.PutInBank(0x11, 0xB514, Blob.FromHex("A200A000BD00622904F01BA9B548A93248A91148A9BF48A98F48A9004C03FEA510C97B9001C8E8D0DB98C93F9002A93FAABD00B9856120B9EC60"));
			//JSR to this new function instead of directly to load price
			rom.PutInBank(0x11, 0xB44A, Blob.FromHex("2014B5"));
			//add the item lookup function to Bank 00
			rom.PutInBank(0x00, 0xBF90, Blob.FromHex("BD00B18510684C03FE60"));

			int expChestCountPercent = rng.Between(flags.ExpChestConversionMin, flags.ExpChestConversionMax);
			int expChestCount = treasureData.Data.Where(g => g >= Item.Gold10).Count() * expChestCountPercent / 100;
			if (expChestCount == 0) return;

			LoadData();

			var unusedGoldDic = new HashSet<int>(ItemLists.UnusedGoldItems.Cast<int>());

			// construct a dictionary and get a shuffled index into it.
			var goldItems = ItemLists.AllGoldTreasure.Select(g => (item: g, price: itemPrices[g], name: itemNames[(int)g])).ToList();
			goldItems.Shuffle(rng);
			var goldItemsDic = goldItems.Select((g, i) => (shuffleindex: i, item: g.item, price: g.price, name: g.name)).ToDictionary(g => g.item);

			var expItems = new HashSet<Item>(treasureData.Data
				.Where(g => goldItemsDic.ContainsKey(g) && !unusedGoldDic.Contains((int)g))
				.Select(g => (item: g, shuffleindex: goldItemsDic[g].shuffleindex))
				.OrderBy(g => g.shuffleindex)
				.Take(expChestCount)
				.Select(g => g.item)
				.Distinct());

			var firstExpItem = RepackGoldExpItems(goldItemsDic, expItems, unusedGoldDic);

			List<(ushort, int)> expChests = new List<(ushort, int)>();
			int currentChestIndex = 0;
			for (int i = (int)firstExpItem; i < 176; i++)
			{
				if (unusedGoldDic.Contains(i)) continue;
				var e = (Item)i;

				if(progressiveChestXPType == ChestXPType.RandomProgressive)
				{
					var exp = (ushort)rng.Between(flags.ExpChestMinReward, flags.ExpChestMaxReward);
					itemPrices[e] = exp;
					itemNames[(int)e] = exp.ToString() + " EXP";
					expChests.Add((exp, i));
				} else if(progressiveChestXPType == ChestXPType.LinearProgressive) {
					int expChestsTotal = expItems.Count();
					double maxXpIndex = Math.Max((expChestsTotal * 0.80), 1.0);
					double chestMultiplier = (flags.ExpChestMaxReward - flags.ExpChestMinReward) / maxXpIndex;
					ushort exp = Math.Min((ushort)(currentChestIndex * chestMultiplier + flags.ExpChestMinReward), (ushort)flags.ExpChestMaxReward);
					itemPrices[e] = exp;
					itemNames[(int)e] = exp.ToString() + " EXP";
					expChests.Add((exp, i));
				} else if(progressiveChestXPType == ChestXPType.ExponentialProgressive) {
					int expChestsTotal = expItems.Count();
					double maxXpIndex = Math.Max((expChestsTotal * 0.80), 1.0);
					double chestMultiplier = (flags.ExpChestMaxReward - flags.ExpChestMinReward) / (maxXpIndex * maxXpIndex);
					ushort exp = Math.Min((ushort)(currentChestIndex * currentChestIndex * chestMultiplier + flags.ExpChestMinReward), (ushort)flags.ExpChestMaxReward);
					itemPrices[e] = exp;
					itemNames[(int)e] = exp.ToString() + " EXP";
					expChests.Add((exp, i));
				}

				currentChestIndex++;
			}

			//sort ascending
			expChests.Sort((x, y) => x.Item1.CompareTo(y.Item1));
			currentChestIndex = 0;

			int TreasureOffset = 0x03100;
			int TreasureCount = 256;

			byte[] treasurePlacementData = rom.Get(TreasureOffset, TreasureCount);
			for (int i = 0; i < expChests.Count; i++)
			{
				//find out how many of these chests exist, there could be more than one of each, put them in order
				//for(int j = 0; j < treasurePlacementData.Where(x => x == expChests[i].Item2).Count(); j++)
				//{
				//	rom.PutInBank(0x11, 0xB900 + currentChestIndex, new byte[] { (byte)expChests[i].Item2 });
				//	currentChestIndex++;
				//	currentChestIndex = Math.Min(currentChestIndex, 63);
				//}

				rom.PutInBank(0x11, 0xB900 + currentChestIndex, new byte[] { (byte)expChests[i].Item2 });
				currentChestIndex++;
				rom.PutInBank(0x11, 0xB900 + currentChestIndex, new byte[] { (byte)expChests[i].Item2 });
				currentChestIndex++;
			}

			//fill out the rest of the table
			for (; currentChestIndex < 64; currentChestIndex++)
			{
				rom.PutInBank(0x11, 0xB900 + currentChestIndex, new byte[] { (byte)expChests.Last().Item2 });
			}

			FirstExpItem = firstExpItem;
			if (flags.Archipelago)
			{
				rom.PutInBank(0x11, 0xB446, new byte[] { (byte)firstExpItem });
			}
			else
			{
				rom.PutInBank(0x11, 0xB447, new byte[] { (byte)firstExpItem });
			}

			//put the comparision in the chest counter
			rom.PutInBank(0x11, 0xB536, new byte[] { (byte)firstExpItem });

			var result = treasureData.Data.Where(x => x > Item.Gold10).OrderBy(x => x).Select(x => itemNames[(int)x]).ToList();

			StoreData();
		}


		//For simplicity only chests are corrected. Npc rewards will be collateral damage.
		private Item RepackGoldExpItems(Dictionary<Item, (int shuffleindex, Item item, ushort price, string name)> goldItemsDic, HashSet<Item> expItems, HashSet<int> unusedGoldDic)
		{
			var repackDic = new Dictionary<Item, Item>();

			var goldItems = ItemLists.AllGoldTreasure.Where(g => !expItems.Contains(g) && !unusedGoldDic.Contains((int)g));

			var goldItemsEnumerator = goldItems.GetEnumerator();
			var expItemsEnumerator = expItems.GetEnumerator();
			for (int i = (int)Item.Gold10; i < 176; i++)
			{
				if (!unusedGoldDic.Contains(i))
				{
					if (goldItemsEnumerator.MoveNext())
					{
						repackDic.Add(goldItemsEnumerator.Current, (Item)i);
					}
					else if (expItemsEnumerator.MoveNext())
					{
						repackDic.Add(expItemsEnumerator.Current, (Item)i);
					}
				}
			}

			/*
			var repackDic = ItemLists.AllGoldTreasure
							.Where(g => !expItems.Contains(g))
							.Concat(expItems)
							.Select((g, i) => (oldId: g, newId: (Item)((int)Item.Gold10 + i)))
							.ToDictionary(x => x.oldId, x => x.newId);
			*/

			for (int i = 0; i < placedItems.Count; i++)
			{
				if (repackDic.TryGetValue(placedItems[i].Item, out var newId))
				{
					if (placedItems[i].GetType() == typeof(TreasureChest))
					{
						placedItems[i] = new TreasureChest(placedItems[i], newId);
					}
					else if (placedItems[i].GetType() == typeof(NpcReward))
					{
						placedItems[i] = new NpcReward(placedItems[i], newId);
					}
				}
			}

			var firstExpItem = (Item)176;
			foreach (var kv in repackDic)
			{
				itemPrices[kv.Value] = goldItemsDic[kv.Key].price;
				itemNames[(int)kv.Value] = goldItemsDic[kv.Key].price.ToString() + " G";

				if (expItems.Contains(kv.Key) && kv.Value < firstExpItem) firstExpItem = kv.Value;
			}

			return firstExpItem;
		}

		private void LoadData()
		{
			itemPrices.LoadTable();
			//treasureData.LoadTable();
			itemNames = rom.ItemsText;
		}

		private void StoreData()
		{
			itemPrices.StoreTable();
			//treasureData.StoreTable();
		}
	}
}
