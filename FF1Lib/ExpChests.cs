using FF1Lib.Data;
using RomUtilities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FF1Lib
{
	public class ExpChests
	{
		public const int BaseExp = 4000;

		FF1Rom rom;
		Flags flags;
		MT19337 rng;

		TreasureData treasureData;
		ItemPrices itemPrices;
		string[] itemNames;

		public ExpChests(FF1Rom _rom, Flags _flags, MT19337 _rng)
		{
			rom = _rom;
			flags = _flags;
			rng = _rng;

			treasureData = new TreasureData(rom);
			itemPrices = new ItemPrices(rom);
		}

		public void BuildExpChests()
		{
			rom.PutInBank(0x1F, 0xDDD0, Blob.FromHex("A9B648A9FF48A9114C03FE8A20DA8760"));

			int expChestCountPercent = rng.Between(flags.ExpChestConversionMin, flags.ExpChestConversionMax);

			int expChestCount = treasureData.Data.Where(g => g >= Item.Gold10).Count() * expChestCountPercent / 100;
			double lowScale = (double)flags.ExpChestMinReward / (double)BaseExp;
			double highScale = (double)flags.ExpChestMaxReward / (double)BaseExp;

			if (expChestCount == 0) return;

			LoadData();

			var unusedGoldDic = new HashSet<int>(FF1Rom.UnusedGoldItems.Cast<int>());

			// construct a dictionary and get a shuffled index into it.
			var goldItems = ItemLists.AllGoldTreasure.Select(g => (item: g, price: itemPrices[g], name: itemNames[(int)g])).ToList();
			goldItems.Shuffle(rng);
			var goldItemsDic = goldItems.Select((g, i) => (shuffleindex: i, item: g.item, price: g.price, name: g.name)).ToDictionary(g => g.item);

			var expItems = new HashSet<Item>(treasureData.Data
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

			rom.PutInBank(0x11, 0xB447, new byte[] { (byte)firstExpItem });


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

			for (int i = 0; i < treasureData.Data.Length; i++)
			{
				if (repackDic.TryGetValue(treasureData[i], out var newId)) treasureData[i] = newId;
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
			treasureData.LoadTable();
			itemNames = rom.ReadText(FF1Rom.ItemTextPointerOffset, FF1Rom.ItemTextPointerBase, FF1Rom.ItemTextPointerCount);
		}

		private void StoreData()
		{
			itemPrices.StoreTable();
			treasureData.StoreTable();
			rom.WriteText(itemNames, FF1Rom.ItemTextPointerOffset, FF1Rom.ItemTextPointerBase, FF1Rom.ItemTextOffset, FF1Rom.UnusedGoldItems);
		}
	}
}
