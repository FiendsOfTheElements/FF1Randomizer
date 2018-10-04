using RomUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FF1Lib
{
	public enum WorldWealth
	{
		High,
		Normal,
		Low,
		Impoverished,
	}

	public class ItemGenerator
	{
		// Type:          Vanilla Chance:  Vanilla Percent:
		// Weapon          35 / 234        15
		// Armor           51 / 234        22
		// Consumable      36 / 234        15
		// Gold           112 / 234        48
		private static readonly List<int>[] Ratios = {
			new List<int> { 10, 25, 15, 10, 40 },
			new List<int> {  7, 15, 18, 15, 45 },
			new List<int> {  4, 10, 25, 20, 41 },
			new List<int> {  1,  9, 40, 25, 25 },
		};

		private enum Tier
		{
			Legendary,
			Rare,
			Common,
			Consumable,
			Gold
		}

		private List<List<Item>> _pool;

		public ItemGenerator(List<Item> seedPool, WorldWealth wealth)
		{
			// Make a copy
			var treasurePool = seedPool.ToList();

			// Make sure we copy all the input lists so we don't modify anything static.
			List<List<Item>> tiers = new List<List<Item>>
			{
				ItemLists.LegendaryTier.Where(item => treasurePool.Remove(item)).ToList(),
				ItemLists.RareTier.Where(item => treasurePool.Remove(item)).ToList(),
				ItemLists.CommonTier.Where(item => treasurePool.Remove(item)).ToList(),
				ItemLists.AllConsumables.Where(item => treasurePool.Remove(item)).ToList(),
				treasurePool.Where(x => x >= Item.Gold10 && x <= Item.Gold65000).ToList(),
			};

			List<int> ratios = Ratios[(int)wealth];
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
			return _pool.PickRandom(rng).PickRandom(rng);
		}

		public Item SpliceItem(MT19337 rng)
		{
			Item item = GetItem(rng);
			_pool.ForEach(pool => pool.RemoveAll(i => i == item));
			_pool.RemoveAll(pool => !pool.Any());
			return item;
		}

	}
}
