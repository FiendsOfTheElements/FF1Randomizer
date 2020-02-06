using RomUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FF1Lib
{
	public enum WorldWealthMode
	{
		High,
		Normal,
		Low,
		Impoverished,
		Melmond,
	}

	public class ItemGenerator
	{
		// Type:          Vanilla Count:
		// Unique (Masa)       1
		// Legendary Weapon    4
		// Legendary Armor    12
		// Rare Weapon        17
		// Rare Armor         16
		// Common Weapon      35 - 1 - 4 - 17 = 13
		// Common Armor       51 - 12 - 16    = 23
		// Weapon          35 / 234        15
		// Armor           51 / 234        22
		// Consumable      36 / 234        15
		// Gold           112 / 234        48
		private static readonly List<int>[] Ratios = {
			new List<int> {  1,  6, 18, 20, 20,  8, 15, 20,  90 },
			new List<int> {  1,  4, 12, 17, 16, 13, 23, 36, 112 },
			new List<int> {  1,  3, 10, 14, 13, 17, 28, 36, 112 },
			new List<int> {  1,  3,  9, 12, 12, 19, 30, 36, 112 },
			new List<int> {  0,  2,  8, 12, 12, 25, 35, 36, 100 },
		};

		private enum Tier
		{
			Unique,
			LegendaryWeapon,
			LegendaryArmor,
			RareWeapon,
			RareArmor,
			CommonWeapon,
			CommonArmor,
			Consumable,
			Gold
		}

		private List<List<Item>> _pool;

		public ItemGenerator(List<Item> seedPool, WorldWealthMode wealth)
		{
			// Make a copy
			var treasurePool = seedPool.ToList();

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
				treasurePool.Where(x => x >= Item.Gold10 && x <= Item.Gold65000).ToList(),
			};

			List<int> ratios = Ratios[(int)wealth];
			System.Diagnostics.Debug.Assert(tiers.Count == ratios.Count);
			System.Diagnostics.Debug.Assert(Enum.GetValues(typeof(WorldWealthMode)).Length == Ratios.Length);

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
