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
		private static readonly List<int>[] Percentages = {
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

		private List<List<Item>> _tiers = new List<List<Item>>();
		private List<int> _percentages;
		private Dictionary<Tier, int> _results;

		public ItemGenerator(WorldWealth wealth, List<Item> treasurePool)
		{
			// Make sure we copy all the input lists so we don't modify anything static.
			_tiers.Add(ItemLists.LegendaryTier.ToList());

			List<Item> rares = ItemLists.RareTier.ToList();
			rares.AddRange(treasurePool.Where(x => x >= Item.Gold20000 && x <= Item.Gold65000));
			_tiers.Add(rares);

			_tiers.Add(ItemLists.CommonTier.ToList());

			_tiers.Add(ItemLists.AllConsumables.ToList());

			_tiers.Add(treasurePool.Where(x => x >= Item.Gold10 && x < Item.Gold20000).ToList());

			_percentages = Percentages[(int)wealth];

			_results = new Dictionary<Tier, int>()
			{
				{Tier.Legendary, 0},
				{Tier.Rare, 0},
				{Tier.Common, 0},
				{Tier.Consumable, 0},
				{Tier.Gold, 0},
			};

			System.Diagnostics.Debug.Assert(_tiers.Count == _percentages.Count);
		}

		public Item GenerateItem(MT19337 rng)
		{
			int dart = rng.Between(1, 100);
			for (int i = 0; i < _tiers.Count; ++i)
			{
				dart -= _percentages[i];
				if (dart <= 0)
				{
					_results[(Tier)i]++;
					return _tiers[i].PickRandom(rng);
				}
			}

			throw new InsaneException("No tier for item.");
		}

		public void Dump()
		{
			Console.WriteLine($"Generated Tiers: Legendary[{_results[Tier.Legendary]}] Rare[{_results[Tier.Rare]}] Common[{_results[Tier.Common]}] Items[{_results[Tier.Consumable]}] Gold[{ _results[Tier.Gold]}]");
		}

	}
}
