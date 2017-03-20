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

		public void ShuffleShops(MT19337 rng, bool earlyAilments)
		{
			var pointers = Get(ShopPointerOffset, ShopPointerCount * ShopPointerSize).ToUShorts();

			RepackShops(pointers);

			ShuffleShopType(ShopType.Weapon, pointers, rng);
			ShuffleShopType(ShopType.Armor, pointers, rng);
			do
			{
				ShuffleShopType(ShopType.Item, pointers, rng);
			} while (earlyAilments && !AilmentsCovered(pointers));

			Put(ShopPointerOffset, Blob.FromUShorts(pointers));
		}

		public void ShuffleMagicShops(MT19337 rng)
		{
			var pointers = Get(ShopPointerOffset, ShopPointerCount * ShopPointerSize).ToUShorts();

			RepackShops(pointers);

			ShuffleShopType(ShopType.White, pointers, rng);
			ShuffleShopType(ShopType.Black, pointers, rng);

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

			Put(ShopPointerBase + pointers[0], allEntries.ToArray());
		}

		private void ShuffleShopType(ShopType shopType, ushort[] pointers, MT19337 rng)
		{
			var shops = GetShops(shopType, pointers);

			var allEntries = shops.SelectMany(list => list).ToList();
			allEntries.Shuffle(rng);

			var newShops = new List<byte>[ShopSectionSize];
			var entry = 0;
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
				var tryShop = newShops[rng.Between(0, ShopSectionSize - 1)];
				if (tryShop.Count > 0 && tryShop.Count < 5 && !tryShop.Contains(allEntries[entry]))
				{
					tryShop.Add(allEntries[entry++]);
				}
			}

			foreach (var newShop in newShops)
			{
				newShop.Add(0);
			}

			var pointer = pointers[(int)shopType];
			for (int i = 0; i < ShopSectionSize; i++)
			{
				if (newShops[i].Count > 1)
				{
					Put(ShopPointerBase + pointer, newShops[i].ToArray());

					pointers[(int)shopType + i] = pointer;
					pointer += (ushort)newShops[i].Count;
				}
			}
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
