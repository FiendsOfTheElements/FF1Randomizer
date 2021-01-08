using RomUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static FF1Lib.FF1Rom;

namespace FF1Lib
{
	public class FloorHintsHintSource : BaseHintSource
	{
		ItemPrices Prices;

		public FloorHintsHintSource(MT19337 _rng, NPCdata _npcData, Flags _flags, OverworldMap _overworldMap, FF1Rom _rom) : base(_rng, _npcData, _flags, _overworldMap, _rom)
		{
			Prices = new ItemPrices(_rom);
		}

		public override List<GeneratedHint> GetHints()
		{
			List<GeneratedHint> hints = GenerateFloorHints();

			return hints;
		}

		private List<GeneratedHint> GenerateFloorHints()
		{
			List<GeneratedHint> hints = new List<GeneratedHint>();

			HashSet<Item> WowItems = new HashSet<Item>(ItemLists.UberTier.Concat(ItemLists.LegendaryWeaponTier).Concat(ItemLists.LegendaryArmorTier).Concat(ItemLists.AllQuestItems).Distinct());
			HashSet<Item> MehItems = new HashSet<Item>(ItemLists.RareWeaponTier.Concat(ItemLists.RareArmorTier).Distinct());
			HashSet<Item> SparkleItems = new HashSet<Item>(ItemLists.AllGoldTreasure);

			var locations = rom.generatedPlacement.Where(p => p is TreasureChest).GroupBy(p => p.MapLocation);

			foreach (var location in locations)
			{
				bool wow = location.FirstOrDefault(p => WowItems.Contains(p.Item)) != null;
				bool meh = location.FirstOrDefault(p => MehItems.Contains(p.Item)) != null;
				bool sparkle = location.Where(p => SparkleItems.Contains(p.Item)).Sum(p => Prices[p.Item]) < 10000;

				if (wow && sparkle)
				{
					hints.Add(new GeneratedHint
					{
						Coverage = flags.ExtensiveHints_FloorHintCoverage,
						PlacementStrategy = flags.ExtensiveHints_FloorHintPlacement,
						MapLocation = location.Key,
						Text = translator.TranslateFloor(location.Key) + "\nis\nWow and Sparkle!"
					});
				}
				else if (wow && !sparkle)
				{
					hints.Add(new GeneratedHint
					{
						Coverage = flags.ExtensiveHints_FloorHintCoverage,
						PlacementStrategy = flags.ExtensiveHints_FloorHintPlacement,
						MapLocation = location.Key,
						Text = translator.TranslateFloor(location.Key) + "\nis\nWow!"
					});
				}
				else if (meh && sparkle)
				{
					hints.Add(new GeneratedHint
					{
						Coverage = flags.ExtensiveHints_FloorHintCoverage,
						PlacementStrategy = flags.ExtensiveHints_FloorHintPlacement,
						MapLocation = location.Key,
						Text = translator.TranslateFloor(location.Key) + "\nis\nMeh but Sparkle!"
					});
				}
				else if (meh && !sparkle)
				{
					hints.Add(new GeneratedHint
					{
						Coverage = flags.ExtensiveHints_FloorHintCoverage,
						PlacementStrategy = flags.ExtensiveHints_FloorHintPlacement,
						MapLocation = location.Key,
						Text = translator.TranslateFloor(location.Key) + "\nis\nMeh!"
					});
				}
				else if (sparkle)
				{
					hints.Add(new GeneratedHint
					{
						Coverage = flags.ExtensiveHints_FloorHintCoverage,
						PlacementStrategy = flags.ExtensiveHints_FloorHintPlacement,
						MapLocation = location.Key,
						Text = translator.TranslateFloor(location.Key) + "\nis\njust Sparkle!"
					});
				}
				else
				{
					hints.Add(new GeneratedHint
					{
						Coverage = flags.ExtensiveHints_FloorHintCoverage,
						PlacementStrategy = flags.ExtensiveHints_FloorHintPlacement,
						MapLocation = location.Key,
						Text = translator.TranslateFloor(location.Key) + "\nis\na Dud!"
					});
				}
			}

			return hints.Distinct().ToList();
		}
	}
}
