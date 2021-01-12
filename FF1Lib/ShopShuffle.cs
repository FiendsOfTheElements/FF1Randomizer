using RomUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FF1Lib
{
	public class ShopShuffle
	{
		MT19337 rng;
		Flags flags;
		FF1Rom rom;

		ShopData ShopData;

		public ShopShuffle(MT19337 _rng, Flags _flags, FF1Rom _rom)
		{
			rng = _rng;
			flags = _flags;
			rom = _rom;



			ShopData = new ShopData(rom);
		}

		public IncentiveData ShuffleShops(IncentiveData incentivesData)
		{
			if (((bool)flags.Shops))
			{
				var excludeItemsFromRandomShops = new List<Item>();
				if ((bool)flags.Treasures)
				{
					excludeItemsFromRandomShops = incentivesData.ForcedItemPlacements.Select(x => x.Item).Concat(incentivesData.IncentiveItems).ToList();
				}

				if (!((bool)flags.RandomWaresIncludesSpecialGear))
				{
					excludeItemsFromRandomShops.AddRange(ItemLists.SpecialGear);
					if ((bool)flags.GuaranteedRuseItem)
						excludeItemsFromRandomShops.Add(Item.PowerRod);
				}

				if ((bool)flags.NoMasamune)
				{
					excludeItemsFromRandomShops.Add(Item.Masamune);
				}

				var shopItemLocation = ShuffleShops(rng, (bool)flags.ImmediatePureAndSoftRequired, ((bool)flags.RandomWares), excludeItemsFromRandomShops, flags.WorldWealth);
				incentivesData = new IncentiveData(rng, flags, incentivesData.OverworldMap, shopItemLocation);
			}

			return incentivesData;
		}

		private ItemShopSlot ShuffleShops(MT19337 rng, bool earlyAilments, bool randomizeWeaponsAndArmor, IEnumerable<Item> excludeItemsFromRandomShops, WorldWealthMode wealth)
		{
			ShopData.LoadData();

			ShuffleShopType(ShopType.Weapon, rng, randomizeWeaponsAndArmor, excludeItemsFromRandomShops, wealth);
			ShuffleShopType(ShopType.Armor, rng, randomizeWeaponsAndArmor, excludeItemsFromRandomShops, wealth);
			ItemShopSlot result = ShuffleShopType(ShopType.Item, rng, randomizeWeaponsAndArmor, excludeItemsFromRandomShops, wealth, earlyAilments);

			if (result == null) throw new InvalidOperationException("Shop Location for Bottle was not set");

			return result;
		}

		public void ShuffleMagicShops()
		{
			if (((bool)flags.MagicShops))
			{
				ShuffleMagicShops(rng);
			}
		}

		private void ShuffleMagicShops(MT19337 rng)
		{
			ShopData.LoadData();

			ShuffleShopType(ShopType.White, rng);
			ShuffleShopType(ShopType.Black, rng);
		}

		public void ShuffleMagicLocations()
		{
			if ((bool)flags.MagicShopLocs)
			{
				ShuffleMagicLocations(rng);
			}
		}

		private void ShuffleMagicLocations(MT19337 rng)
		{
			ShopData.LoadData();

			var whiteShops = ShopData.Shops.Where(s => s.Type == ShopType.White).ToList();
			var whiteShopLists = whiteShops.Select(s => s.Entries).ToList();

			whiteShopLists.Shuffle(rng);

			for (int i = 0; i < whiteShops.Count; i++) whiteShops[i].Entries = whiteShopLists[i];

			var blackShops = ShopData.Shops.Where(s => s.Type == ShopType.Black).ToList();
			var blackShopLists = whiteShops.Select(s => s.Entries).ToList();

			ShopData.StoreData();
		}

		private ItemShopSlot ShuffleShopType(ShopType shopType, MT19337 rng, bool randomize = false, IEnumerable<Item> excludeItemsFromRandomShops = null, WorldWealthMode wealth = WorldWealthMode.Normal, bool earlyAilments = false)
		{

			var newShops = ShopData.Shops.Where(s => s.Type == shopType).Select(s => s.CloneEmpty()).ToList();

			var allEntries = ShopData.Shops.Where(s => s.Type == shopType).SelectMany(list => list.Entries).ToList();

			if (earlyAilments && shopType == ShopType.Item)
			{
				allEntries.RemoveAt(allEntries.IndexOf(Item.Pure));
				allEntries.RemoveAt(allEntries.IndexOf(Item.Soft));

				var coneriaItemShop = newShops.First(s => s.Type == ShopType.Item && s.Location == MapLocation.Coneria);
				coneriaItemShop.Entries.Add(Item.Pure);
				coneriaItemShop.Entries.Add(Item.Soft);
			}

			allEntries = allEntries.GroupBy(x => x).OrderBy(x => -x.Count()).SelectMany(x => x).ToList();

			int entry = 0;

			foreach (var shop in newShops)
			{
				if (shop.Entries.Count == 0)
				{
					var item = allEntries.PickRandom(rng);
					allEntries.Remove(item);
					shop.Entries.Add(item);
				}
			}

			while (entry < allEntries.Count)
			{
				var eligibleShops = new List<int>();
				for (int i = 0; i < newShops.Count; i++)
				{
					if (newShops[i].Entries.Count < 5 && !newShops[i].Entries.Contains(allEntries[entry]))
					{
						eligibleShops.Add(i);
					}
				}

				if (eligibleShops.Count == 0) throw new Exception("Should not happen!");

				int shopIndex = eligibleShops[rng.Between(0, eligibleShops.Count - 1)];
				newShops[shopIndex].Entries.Add(allEntries[entry++]);
			}

			if (randomize)
			{
				if (shopType == ShopType.Weapon || shopType == ShopType.Armor)
				{
					// Shuffle up a byte array of random weapons or armor and assign them in place of the existing items.
					var baseIndex = shopType == ShopType.Armor ? Item.Cloth : Item.WoodenNunchucks;
					var indeces = Enumerable.Range((int)baseIndex, 40).Select(i => (Item)i).ToList();
					foreach (var exclusion in excludeItemsFromRandomShops ?? new List<Item>())
					{
						indeces.Remove(exclusion);
					}

					ItemGenerator generator = new ItemGenerator(indeces, wealth);
					for (int i = 0; i < newShops.Count; i++)
					{
						newShops[i].Entries = newShops[i].Entries.Select(x => generator.SpliceItem(rng)).ToList();
					}
				}
			}

			foreach (var shop in newShops) shop.Entries.Shuffle(rng);

			ShopData.SetShops(newShops);
			ShopData.StoreData();

			var bottleshop = ShopData.Shops.FirstOrDefault(s => s.Type == ShopType.Item && s.Entries.Contains(Item.Bottle));
			if (bottleshop != null)
			{
				var index = bottleshop.Entries.IndexOf(Item.Bottle);
				var entrypointer = ShopData.GetShopEntryPointer(bottleshop, index);
				return new ItemShopSlot(entrypointer, $"{bottleshop.Location.ToString()}Shop{index + 1}", bottleshop.Location, Item.Bottle);
			}

			return null;
		}
	}
}
