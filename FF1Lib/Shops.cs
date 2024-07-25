using System.Reflection;

namespace FF1Lib
{
	public partial class ShopData
	{
		private int coneriaEntranceShopIndex = 0;
		public void ShuffleShops(MT19337 rng, IEnumerable<Item> excludeItemsFromRandomShops, int _coneriaEntranceShopIndex)
		{
			bool enabled = (bool)flags.Shops;
			bool randomizeWeaponsAndArmor = (bool)flags.RandomWares;
			WorldWealthMode wealth = flags.WorldWealth;
			coneriaEntranceShopIndex = _coneriaEntranceShopIndex;

			if (!enabled)
			{
				return;
			}

			ShuffleShopType(ShopType.Weapon, rng, randomizeWeaponsAndArmor, excludeItemsFromRandomShops, wealth);
			ShuffleShopType(ShopType.Armor, rng, randomizeWeaponsAndArmor, excludeItemsFromRandomShops, wealth);
			ShuffleShopType(ShopType.Item, rng);
		}
		public void ShuffleMagicShops(bool enable, MT19337 rng)
		{
			if (!enable)
			{
				return;
			}

			ShuffleShopType(ShopType.White, rng);
			ShuffleShopType(ShopType.Black, rng);
		}
		public void ShuffleMagicLocations(bool enable, bool keepPairs, MT19337 rng)
		{
			if(!enable)
			{
				return;
			}

			/*
			List<int> whiteShops = Shops.Where(s => s.Type == ShopType.White).Select(s => s.Index).ToList();
			List<int> blackShops = Shops.Where(s => s.Type == ShopType.Black).Select(s => s.Index).ToList();
			List<(int white, int black)> pairedShops = whiteShops.Select((s, i) => (s, blackShops[i])).ToList();

			whiteShops.Shuffle(rng);
			blackShops.Shuffle(rng);
			pairedShops.Shuffle(rng);
			*/

			List<List<Item>> whiteShops = Shops.Where(s => s.Type == ShopType.White).Select(s => s.Entries.ToList()).ToList();
			List<List<Item>> blackShops = Shops.Where(s => s.Type == ShopType.Black).Select(s => s.Entries.ToList()).ToList();
			List<(List<Item> white, List<Item> black)> pairedShops = whiteShops.Select((s, i) => (s, blackShops[i])).ToList();

			whiteShops.Shuffle(rng);
			blackShops.Shuffle(rng);
			pairedShops.Shuffle(rng);

			if (keepPairs)
			{
				for (int i = 0; i < pairedShops.Count; i++)
				{
					Shops.Find(s => s.Index == i + 20).Entries = pairedShops[i].white;
					Shops.Find(s => s.Index == i + 30).Entries = pairedShops[i].black;
				}
			}
			else
			{
				for (int i = 0; i < whiteShops.Count; i++)
				{
					Shops.Find(s => s.Index == i + 20).Entries = whiteShops[i];
					Shops.Find(s => s.Index == i + 30).Entries = blackShops[i];
				}
			}
		}
		private void ShuffleShopType(ShopType shopType, MT19337 rng, bool randomize = false, IEnumerable<Item> excludeItemsFromRandomShops = null, WorldWealthMode wealth = WorldWealthMode.Standard)
		{
			var shops = Shops.Where(s => s.Type == shopType).ToList();
			var newShopEntries = shops.ToDictionary(s => s.Index, s => new List<Item>());
			bool shopsBlocked;
			bool requiredAdded = false;

			do
			{
				shopsBlocked = false;

				// Get Shop Items
				var allEntries = shops.SelectMany(s => s.Entries).ToList();
				allEntries.Shuffle(rng);

				// Insert guaranteed items
				if (shopType == ShopType.Item && (bool)flags.ImmediatePureAndSoftRequired && !requiredAdded)
				{
					allEntries.Remove(Item.Pure);
					allEntries.Remove(Item.Soft);

					newShopEntries[60 + coneriaEntranceShopIndex].AddRange(new List<Item>(){ Item.Pure, Item.Soft });
					requiredAdded = true;
				}

				int entry = 0;
				foreach (var shop in newShopEntries)
				{
					if (!shop.Value.Any())
					{
						shop.Value.Add(allEntries[entry++]);
					}
				}

				while (entry < allEntries.Count)
				{
					var eligibleShops = newShopEntries.Where(s => s.Value.Count < 5 && !s.Value.Contains(allEntries[entry])).ToList();

					// We might be unable to place an item in any shop because they're all full, or they already have that item.  Start over.
					if (!eligibleShops.Any())
					{
						shopsBlocked = true;
						break;
					}

					eligibleShops.PickRandom(rng).Value.Add(allEntries[entry++]);
				}
			} while (shopsBlocked);

			if (randomize)
			{
				if (shopType == ShopType.Weapon || shopType == ShopType.Armor)
				{
					// Shuffle up a byte array of random weapons or armor and assign them in place of the existing items.
					var baseIndex = shopType == ShopType.Armor ? Item.Cloth : Item.WoodenNunchucks;
					var indices = Enumerable.Range((int)baseIndex, 40).Select(i => (Item)i).ToList();
					foreach (var exclusion in excludeItemsFromRandomShops ?? new List<Item>())
					{
						indices.Remove(exclusion);
					}

					ShopItemGenerator generator = new ShopItemGenerator(indices, ItemLists.UnusedGoldItems.ToList(), new List<Item>());
					foreach (var shop in newShopEntries)
					{
						newShopEntries[shop.Key] = newShopEntries[shop.Key].Select(x => generator.GetItem(rng)).ToList();
					}
				}
			}

			if (shopType == ShopType.Item)
			{
				// Reshuffle the guaranteed pure+soft item shop
				newShopEntries[60 + coneriaEntranceShopIndex].Shuffle(rng);

				// Update bottle location
				var bottleshops = newShopEntries.Where(s => s.Value.Contains(Item.Bottle));
				if (!bottleshops.Any())
				{
					throw new InvalidOperationException("Shop Location for Bottle was not set");
				}
				var bottleshop = bottleshops.First();
				var location = ShopPrototypes[bottleshop.Key].Location;
				ItemShopSlot = new ItemShopSlot(0,
					$"{Enum.GetName(typeof(MapLocation), location)}Shop{bottleshop.Value.FindIndex(x => x == Item.Bottle) + 1}",
					location,
					Item.Bottle,
					(byte)bottleshop.Key);
			}

			foreach (var shop in newShopEntries)
			{
				var shopIndex = Shops.FindIndex(s => s.Index == shop.Key);
				if (shopIndex > -1)
				{
					Shops[shopIndex].Entries = shop.Value;
				}
			}
		}
	}


	public partial class FF1Rom : NesRom
	{
		/*
		public const int ShopPointerOffset = 0x38302; // 0x38300 technically, but the first one is unused.
		public const int ShopPointerBase = 0x30000;
		public const int ShopPointerSize = 2;
		public const int ShopPointerCount = 70;
		public const int ShopSectionSize = 10;
		public const ushort ShopNullPointer = 0x838E;*/
	}
}
