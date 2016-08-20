using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using RomUtilities;

namespace FF1Randomizer
{
	public class FF1Rom : NesRom
	{
		public const int TreasureOffset = 0x3100;
		public const int TreasureSize = 1;
		public const int TreasureCount = 256;

		public const int EnemyOffset = 0x30520;
		public const int EnemySize = 20;
		public const int EnemyCount = 128;

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

		public FF1Rom(string filename) : base(filename)
		{}

		public override bool Validate()
		{
			return Get(0, 16) == Blob.FromHex("06400e890e890e401e400e400e400b42");
		}

		public void ShuffleTreasures(MT19337 rng)
		{
			DirectedGraph<byte> graph;
			do
			{
				var treasureBlob = Get(TreasureOffset, TreasureSize*TreasureCount);

				var usedIndices = Enumerable.Range(0, TreasureCount).Except(TreasureConditions.NotUsed).ToList();
				var usedTreasures = usedIndices.Select(i => treasureBlob[i]).ToList();

				usedTreasures.Shuffle(rng);

				for (int i = 0; i < usedIndices.Count; i++)
				{
					treasureBlob[usedIndices[i]] = usedTreasures[i];
				}

				Put(TreasureOffset, treasureBlob);

				var blockages = new List<Tuple<byte, int, List<int>>>
				{
					Tuple.Create((byte)QuestItems.Crown,   Array.IndexOf(treasureBlob, (byte)QuestItems.Crown),   TreasureConditions.CrownBlocked),
					Tuple.Create((byte)QuestItems.Tnt,     Array.IndexOf(treasureBlob, (byte)QuestItems.Tnt),     TreasureConditions.TntBlocked),
					Tuple.Create((byte)QuestItems.Ruby,    Array.IndexOf(treasureBlob, (byte)QuestItems.Ruby),    TreasureConditions.RubyBlocked),
					Tuple.Create((byte)QuestItems.Floater, Array.IndexOf(treasureBlob, (byte)QuestItems.Floater), TreasureConditions.FloaterBlocked),
					Tuple.Create((byte)QuestItems.Slab,    Array.IndexOf(treasureBlob, (byte)QuestItems.Slab),    TreasureConditions.SlabBlocked)
				};

				graph = new DirectedGraph<byte>();
				foreach (var blockage in blockages)
				{
					graph.AddNode(blockage.Item1);
				}

				foreach (var blocker in blockages)
				{
					foreach (var blockee in blockages)
					{
						if (blocker.Item3.Contains(blockee.Item2))
						{
							graph.AddEdge(blockee.Item1, blocker.Item1);
						}
					}
				}
			} while (graph.HasCycles());
		}

		public void ShuffleShops(MT19337 rng)
		{
			var pointers = Get(ShopPointerOffset, ShopPointerCount*ShopPointerSize).ToUShorts();

			RepackShops(pointers);

			ShuffleShopType(ShopType.Weapon, pointers, rng);
			ShuffleShopType(ShopType.Armor, pointers, rng);
			ShuffleShopType(ShopType.Item, pointers, rng);

			Put(ShopPointerOffset, Blob.FromUShorts(pointers));
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
					newShops[i].Add(allEntries[entry]);
				}

				entry++;
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

		public void ExpGoldBoost(double factor)
		{
			var enemyBlob = Get(EnemyOffset, EnemySize*EnemyCount);
			var enemies = enemyBlob.Chunk(EnemySize);

			foreach (var enemy in enemies)
			{
				var exp = BitConverter.ToUInt16(enemy, 0);
				var gold = BitConverter.ToUInt16(enemy, 2);

				exp = (ushort)Math.Min(exp*factor, 0x7FFF);
				gold = (ushort)Math.Min(gold*factor, 0x7FFF);

				var expBytes = BitConverter.GetBytes(exp);
				var goldBytes = BitConverter.GetBytes(gold);
				Array.Copy(expBytes, 0, enemy, 0, 2);
				Array.Copy(goldBytes, 0, enemy, 2, 2);
			}

			enemyBlob = Blob.Concat(enemies);

			Put(EnemyOffset, enemyBlob);
		}
	}
}
