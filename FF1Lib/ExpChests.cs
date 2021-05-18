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

		public void SetExpChests()
		{			
			rom.PutInBank(0x11,0xB0A0,Blob.FromHex("48A9FA2083B1202DB1B0106820B9EC20CBB018E67DE67DA2FB8A606820B9EC20EADD18E67DE67DA2FC8A60AD006209808D0062A5108588A5118589A9008DD16A8DD26AA2C02021B1A2802021B1A2402021B1A2002021B12097B1A5888D7868A5898D7968A90020A8B1A90120A8B1A90220A8B1A9034CA8B1AD0062297F8D006260BD01614A2ED16A4A2ED26A60A90085248525A9038512A935857EA98E20C7D6A5FF4901851AA908851B208EB1A51B18690B851BC96090F2208EB120C2D7A52438D005A52518F0F008208EB1A51B38E90B851BC912B0F2A90085248525A01184582860AAA9DD48A9CF488A4CB1D4A9DD48A9CF484CA1D6A9DD48A9CF48A98B48A94248A91B4C03FEAAA9DD48A9CF48A9DD48A9D448A91B4C03FE"));

			rom.PutInBank(0x11, 0xB04A, Blob.FromHex("4CA0B0"));

			rom.PutInBank(0x1F,0xDDD0,Blob.FromHex("A9114C03FE8A20DA8760"));


			Dictionary<int, string> dialogs = new Dictionary<int, string>();

			dialogs.Add(0x14A, "In the treasure box,\nyou found..\n#\n\n\nA..Take Gold\nB..Take Exp");
			dialogs.Add(0x14B, "A sacrifice for power!\n\nExperience taken.");
			dialogs.Add(0x14C, "The greed has\noverwhelmed you!\n\nGold taken.");

			rom.InsertDialogs(dialogs);
		}

		public void BuildExpChests()
		{
			LoadData();

			var result1 = treasureData.Data.Where(x => x >= Item.Gold10 && x <= Item.Gold65000).OrderBy(x => x).Select(x => itemNames[(int)x]).ToList();

			int expChestCount = 50;
			double lowScale = 0.5;
			double highScale = 2.0;

			// construct a dictionary and get a shuffled index into it.
			var goldItems = ItemLists.AllGoldTreasure.Select(g => (item: g, price: itemPrices[g], name: itemNames[(int)g])).ToList();
			goldItems.Shuffle(rng);
			var goldItemsDic = goldItems.Select((g, i) => (shuffleindex: i, item: g.item, price: g.price, name: g.name)).ToDictionary(g => g.item);

			var expItems = new HashSet<Item>(treasureData.Data
				.Where(g => goldItemsDic.ContainsKey(g))
				.Select(g => (item: g, shuffleindex: goldItemsDic[g].shuffleindex))
				.OrderBy(g => g.shuffleindex)
				.Take(expChestCount)
				.Select(g => g.item)
				.Distinct());

			var firstExpItem = RepackGoldExpItems(goldItemsDic, expItems);

			for (int i = (int)firstExpItem; i < 176; i++)
			{
				var e = (Item)i;

				var exp = (ushort)Math.Min(Math.Max(FF1Rom.RangeScale(BaseExp, lowScale, highScale, 1.0, rng), 0), 65535);
				itemPrices[e] = exp;
				itemNames[(int)e] = exp.ToString() + " EXP";
			}

			var result = treasureData.Data.Where(x => x >= Item.Gold10 && x <= Item.Gold65000).OrderBy(x => x).Select(x => itemNames[(int)x]).ToList();

			StoreData();
		}


		//For simplicity only chests are corrected. Npc rewards will be collateral damage.
		private Item RepackGoldExpItems(Dictionary<Item, (int shuffleindex, Item item, ushort price, string name)> goldItemsDic, HashSet<Item> expItems)
		{
			var repackDic = ItemLists.AllGoldTreasure
							.Where(g => !expItems.Contains(g))
							.Concat(expItems)
							.Select((g, i) => (oldId: g, newId: (Item)((int)Item.Gold10 + i)))
							.ToDictionary(x => x.oldId, x => x.newId);

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
			rom.WriteText(itemNames, FF1Rom.ItemTextPointerOffset, FF1Rom.ItemTextPointerBase, FF1Rom.ItemTextOffset);
		}
	}
}
