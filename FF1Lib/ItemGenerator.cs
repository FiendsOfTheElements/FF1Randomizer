using System.ComponentModel;

namespace FF1Lib
{
	public enum WorldWealthMode
	{
		[Description("High Wealth")]
		High,
		[Description("Vanilla Wealth")]
		Standard,
		[Description("Melmond Wealth")]
		Melmond,
	}
	public enum RandomizeTreasureMode
	{
		[Description("None")]
		None,
		[Description("Randomized")]
		RandomizeTreasure,
		[Description("Deep Dungeon")]
		DeepDungeon,
		[Description("Random")]
		Random,
	}
	public interface IItemGenerator
	{
		public Item GetItem(MT19337 rng);
	}
	public class ItemGenerator : IItemGenerator
	{
		// Type:          Vanilla Count:
		// Unique (Masa)       1
		// Legendary Weapon    4
		// Legendary Armor    10
		// Rare Weapon        18
		// Rare Armor         18
		// Common Weapon      35 - 1 - 4 - 18 = 12
		// Common Armor       51 - 10 - 18    = 23
		// Weapon          35 / 234        15
		// Armor           51 / 234        22
		// Consumable      36 / 234        15
		// Gold           112 / 234        48

		private static readonly List<int>[] RelativeRatios = {
			new List<int> {  1,  3, 7, 7, 9, -4, -5, -13, -5 },  // High Wealth
			new List<int> {  0,  0, 0, 0, 0,  0,  0,   0,  0 },
			//new List<int> {  0,  -1, -2, -3, -3, 4, 5, 0,  0 },
			//new List<int> {  0,  -1, -3, -5, -4, 6, 7, 0,  0 },
			new List<int> {  -1, -2, -4, -5, -5, 14, 15, 0, -12 }, //Melmond Wealth
		};

		private List<List<Item>> _pool;

		public ItemGenerator(List<Item> seedPool, List<Item> unusedGoldItems, List<Item> removedItems, WorldWealthMode wealth)
		{
			// Make a copy
			var treasurePool = seedPool.ToList();

			// Make sure we copy all the input lists so we don't modify anything static.
			List<List<Item>> tiers = new List<List<Item>>
			{
				ItemLists.UberTier.Where(x => !removedItems.Contains(x)).ToList(),
				ItemLists.LegendaryWeaponTier.Where(x => !removedItems.Contains(x)).ToList(),
				ItemLists.LegendaryArmorTier.ToList(),
				ItemLists.RareWeaponTier.ToList(),
				ItemLists.RareArmorTier.ToList(),
				ItemLists.CommonWeaponTier.ToList(),
				ItemLists.CommonArmorTier.ToList(),
				ItemLists.AllConsumables.ToList(),
				ItemLists.AllGoldTreasure.Where(x => !unusedGoldItems.Contains(x)).ToList(),
			};

			List<int> ratios = RelativeRatios[(int)wealth].ToList();
			System.Diagnostics.Debug.Assert(tiers.Count == ratios.Count);

			ratios[0] += treasurePool.Where(x => ItemLists.UberTier.Contains(x)).Count();
			ratios[1] += treasurePool.Where(x => ItemLists.LegendaryWeaponTier.Contains(x)).Count();
			ratios[2] += treasurePool.Where(x => ItemLists.LegendaryArmorTier.Contains(x)).Count();
			ratios[3] += treasurePool.Where(x => ItemLists.RareWeaponTier.Contains(x)).Count();
			ratios[4] += treasurePool.Where(x => ItemLists.RareArmorTier.Contains(x)).Count();
			ratios[5] += treasurePool.Where(x => ItemLists.CommonWeaponTier.Contains(x)).Count();
			ratios[6] += treasurePool.Where(x => ItemLists.CommonArmorTier.Contains(x)).Count();
			ratios[7] += treasurePool.Where(x => ItemLists.AllConsumables.Contains(x)).Count();
			ratios[8] += treasurePool.Where(x => ItemLists.AllGoldTreasure.Contains(x)).Count();

			if (!tiers[0].Any())
			{
				ratios[1] += ratios[0];
				ratios[0] = 0;
			}

			for (int i = 0; i < ratios.Count - 1; i++)
			{
				if (ratios[i] < 0)
				{
					ratios[8] += ratios[i];
					ratios[i] = 0;
				}
			}

			// Now populate the combined pool with a weighted average of all those above lists.
			_pool = new List<List<Item>>();
			for (int i = 0; i < ratios.Count(); ++i)
			{
				for (int j = 0; j < ratios[i]; ++j)
				{
					if (tiers[i].Any())
						_pool.Add(tiers[i]);
				}
			}

			System.Diagnostics.Debug.Assert(treasurePool.Count == _pool.Count);
		}

		public Item GetItem(MT19337 rng)
		{
			return _pool.SpliceRandom(rng).PickRandom(rng);
		}
	}

	public class ShopItemGenerator : IItemGenerator
	{
		private static readonly List<int> Ratios = new() { 1, 4, 10, 18, 18, 12, 23, 36, 112 };

		private List<List<Item>> _pool;

		public ShopItemGenerator(List<Item> seedPool, List<Item> unusedGoldItems, List<Item> removedItems)
		{
			// Make a copy
			var treasurePool = seedPool.Where(x => !removedItems.Contains(x)).ToList();

			// Make sure we copy all the input lists so we don't modify anything static.
			List<List<Item>> tiers = new List<List<Item>>
			{
				ItemLists.UberTier.Where(item => treasurePool.Remove(item)).ToList(),
				ItemLists.LegendaryWeaponTier.Where(item => treasurePool.Remove(item)).ToList(),
				ItemLists.LegendaryArmorTier.Where(item => treasurePool.Remove(item)).ToList(),
				ItemLists.RareWeaponTier.Where(item => treasurePool.Remove(item)).ToList(),
				ItemLists.RareArmorTier.Where(item => treasurePool.Remove(item)).ToList(),
				ItemLists.CommonWeaponTier.Where(item => treasurePool.Remove(item)).ToList(),
				ItemLists.CommonArmorTier.Where(item => treasurePool.Remove(item)).ToList(),
				ItemLists.AllConsumables.Where(item => treasurePool.Remove(item)).ToList(),
				ItemLists.AllGoldTreasure.Where(x => !unusedGoldItems.Contains(x)).Where(item => treasurePool.Remove(item)).ToList(),
			};

			List<int> ratios = Ratios;
			System.Diagnostics.Debug.Assert(tiers.Count == ratios.Count);

			// Now populate the comined pool with a weighted average of all those above lists.
			_pool = new List<List<Item>>();
			for (int i = 0; i < ratios.Count(); ++i)
			{
				for (int j = 0; j < ratios[i]; ++j)
				{
					if (tiers[i].Any())
						_pool.Add(tiers[i]);
				}
			}
		}

		public Item GetItem(MT19337 rng)
		{
			Item item = _pool.PickRandom(rng).PickRandom(rng);
			_pool.ForEach(pool => pool.RemoveAll(i => i == item));
			_pool.RemoveAll(pool => !pool.Any());
			return item;
		}
	}
}
