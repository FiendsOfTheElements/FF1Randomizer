using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RomUtilities;

namespace FF1Lib
{
	public partial class FF1Rom : NesRom
	{
		public const int ShopPointerOffset = 0x38302; // 0x38300 technically, but the first one is unused.
		public const int ShopPointerBase = 0x30000;
		public const int ShopPointerSize = 2;
		public const int ShopPointerCount = 70;
		public const int ShopSectionSize = 10;
		public const ushort ShopNullPointer = 0x838E;
        private List<MapLocation> ShopMapLocationsByIndex = new List<MapLocation>{
                MapLocation.Coneria, MapLocation.Pravoka, MapLocation.Elfland,
                MapLocation.CrescentLake, MapLocation.Gaia, MapLocation.Onrac,
                MapLocation.Gaia, MapLocation.Gaia, MapLocation.Gaia,
                MapLocation.Caravan
            };
		private enum ShopType
		{
			Weapon = 0,
			Armor = 10,
			White = 20,
			Black = 30,
			Clinic = 40,
			Inn = 50,
			Item = 60
		}

		public ItemShopSlot ShuffleShops(MT19337 rng, bool earlyAilments, bool randomizeWeaponsAndArmor, IEnumerable<Item> excludeItemsFromRandomShops, WorldWealthMode wealth)
		{
			var pointers = Get(ShopPointerOffset, ShopPointerCount * ShopPointerSize).ToUShorts();

			RepackShops(pointers);

			ShuffleShopType(ShopType.Weapon, pointers, rng, randomizeWeaponsAndArmor, excludeItemsFromRandomShops, wealth);
			ShuffleShopType(ShopType.Armor, pointers, rng, randomizeWeaponsAndArmor, excludeItemsFromRandomShops, wealth);
            ItemShopSlot result = null;
			do
			{
				result = ShuffleShopType(ShopType.Item, pointers, rng);
			} while (earlyAilments && !AilmentsCovered(pointers));
            if (result == null)
                throw new InvalidOperationException("Shop Location for Bottle was not set");

			Put(ShopPointerOffset, Blob.FromUShorts(pointers));
            return result;
		}

		public void ShuffleMagicShops(MT19337 rng)
		{
			var pointers = Get(ShopPointerOffset, ShopPointerCount * ShopPointerSize).ToUShorts();

			RepackShops(pointers);

			ShuffleShopType(ShopType.White, pointers, rng);
			ShuffleShopType(ShopType.Black, pointers, rng);

			Put(ShopPointerOffset, Blob.FromUShorts(pointers));
		}

		private void ShuffleMagicLocations(MT19337 rng)
		{
			var pointers = Get(ShopPointerOffset, ShopPointerCount * ShopPointerSize).ToUShorts();

			RepackShops(pointers);
			var WhiteShops = GetShops(ShopType.White, pointers);

			List<ushort> WhitePointers = new List<ushort>(9);
			List<ushort> BlackPointers = new List<ushort>(9);
			for (int i = 0; i < 9; i++)
			{
				WhitePointers.Add(pointers[(int)ShopType.White + i + 1]);
				BlackPointers.Add(pointers[(int)ShopType.Black + i + 1]);
			}

			WhitePointers.Shuffle(rng);
			BlackPointers.Shuffle(rng);

			for (int i = 0; i < 9; i++)
			{
				pointers[(int)ShopType.White + i + 1] = WhitePointers[i];
				pointers[(int)ShopType.Black + i + 1] = BlackPointers[i];
			}

			Put(ShopPointerOffset, Blob.FromUShorts(pointers));
		}

		private bool AilmentsCovered(ushort[] pointers)
		{
			var shops = GetShops(ShopType.Item, pointers);

			const byte Pure = 0x1A;
			const byte Soft = 0x1B;
			return shops[0].Contains(Pure) && shops[0].Contains(Soft);
		}

		private void RepackShops(ushort[] pointers)
		{
			var allEntries = new List<byte>();
			var pointer = pointers[0];
			for (int shopType = 0; shopType < (int)ShopType.Item + ShopSectionSize; shopType += ShopSectionSize)
			{
				var sectionShops = GetShops((ShopType)shopType, pointers);
				allEntries.AddRange(sectionShops.SelectMany(list =>
				{
					if (list.Any())
					{
						list.Add(0);
					}

					return list;
				}));

				for (int i = 0; i < ShopSectionSize; i++)
				{
					if (pointers[shopType + i] != ShopNullPointer)
					{
						pointers[shopType + i] = pointer;
						pointer += (ushort)sectionShops[i].Count;
					}
				}
			}

			Put(ShopPointerOffset, Blob.FromUShorts(pointers));
			Put(ShopPointerBase + pointers[0], allEntries.ToArray());
		}

		private ItemShopSlot ShuffleShopType(ShopType shopType, ushort[] pointers, MT19337 rng, bool randomize = false, IEnumerable<Item> excludeItemsFromRandomShops = null, WorldWealthMode wealth = WorldWealthMode.Normal)
		{
			var shops = GetShops(shopType, pointers);

			bool shopsBlocked;
			List<byte>[] newShops;
			do
			{
				shopsBlocked = false;
				newShops = new List<byte>[ShopSectionSize];

				var allEntries = shops.SelectMany(list => list).ToList();
				allEntries.Shuffle(rng);

				int entry = 0;
				for (int i = 0; i < ShopSectionSize; i++)
				{
					newShops[i] = new List<byte>();
					if (pointers[(int)shopType + i] != ShopNullPointer)
					{
						newShops[i].Add(allEntries[entry++]);
					}
				}

				while (entry < allEntries.Count)
				{
					var eligibleShops = new List<int>();
					for (int i = 0; i < newShops.Length; i++)
					{
						if (newShops[i].Count > 0 && newShops[i].Count < 5 && !newShops[i].Contains(allEntries[entry]))
						{
							eligibleShops.Add(i);
						}
					}

					// We might be unable to place an item in any shop because they're all full, or they already have that item.  Start over.
					if (eligibleShops.Count == 0)
					{
						shopsBlocked = true;
						break;
					}

					int shopIndex = eligibleShops[rng.Between(0, eligibleShops.Count - 1)];
					newShops[shopIndex].Add(allEntries[entry++]);
				}
			} while (shopsBlocked);

			if (randomize)
			{
				if (shopType == ShopType.Weapon || shopType == ShopType.Armor) {
					// Shuffle up a byte array of random weapons or armor and assign them in place of the existing items.
					var baseIndex = shopType == ShopType.Armor ? Item.Cloth : Item.WoodenNunchucks;
					var indeces = Enumerable.Range((int)baseIndex, 40).Select(i => (Item)i).ToList();
					foreach (var exclusion in excludeItemsFromRandomShops ?? new List<Item>()) 
					{ 
						indeces.Remove(exclusion);
					}

					ItemGenerator generator = new ItemGenerator(indeces, wealth);
					for (int i = 0; i < newShops.Length; i++)
					{
						newShops[i] = newShops[i].Select(x => (byte)generator.SpliceItem(rng)).ToList();
					}
				}
			}
			// Zero-terminate the new shops.
			foreach (var newShop in newShops)
			{
				newShop.Add(0);
			}

            ItemShopSlot result = null;
			var pointer = pointers[(int)shopType];
			for (int i = 0; i < ShopSectionSize; i++)
			{
				if (newShops[i].Count > 1)
				{
                    var bottle = 
                        newShops[i]
                            .Select((item, index) => new { item, index })
                            .FirstOrDefault(x => ((Item)x.item) == Item.Bottle);
                    if (bottle != null)
                    {
                        var location = ShopMapLocationsByIndex[i];
                        result = new ItemShopSlot(ShopPointerBase + pointer + bottle.index,
                                                  $"{Enum.GetName(typeof(MapLocation), location)}Shop{bottle.index + 1}",
                                                  location,
                                                  Item.Bottle);
                    }
					Put(ShopPointerBase + pointer, newShops[i].ToArray());

					pointers[(int)shopType + i] = pointer;
					pointer += (ushort)newShops[i].Count;
				}
			}
            return result;
		}

		private List<byte>[] GetShops(ShopType shopType, ushort[] pointers)
		{
			var shops = new List<byte>[ShopSectionSize];
			for (int i = 0; i < ShopSectionSize; i++)
			{
				shops[i] = new List<byte>();
				if (pointers[(int)shopType + i] != ShopNullPointer)
				{
					if (shopType == ShopType.Clinic || shopType == ShopType.Inn)
					{
						shops[i].AddRange(Get(ShopPointerBase + pointers[(int)shopType + i], 2).ToBytes());
					}
					else
					{
						var shopEntries = Get(ShopPointerBase + pointers[(int)shopType + i], 5);
						for (int j = 0; j < 5 && shopEntries[j] != 0; j++)
						{
							shops[i].Add(shopEntries[j]);
						}
					}
				}
			}

			return shops;
		}
	}
}
