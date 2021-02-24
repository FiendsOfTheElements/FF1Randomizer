using RomUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static FF1Lib.FF1Rom;

namespace FF1Lib
{
	public class LooseNameHintSource : BaseHintSource
	{
		public LooseNameHintSource(MT19337 _rng, NPCdata _npcData, Flags _flags, OverworldMap _overworldMap, FF1Rom _rom) : base(_rng, _npcData, _flags, _overworldMap, _rom)
		{
		}

		public override List<GeneratedHint> GetHints()
		{
			List<GeneratedHint> hints = GenerateLooseItemNameHints();

			return hints;
		}

		private List<GeneratedHint> GenerateLooseItemNameHints()
		{
			List<GeneratedHint> hints = new();

			List<Item> incentivePool = ItemLists.GetIncentivePool(flags);
			List<string> incentiveChests = ItemLists.GetIncentiveChests(flags);

			foreach (Item item in incentivePool)
			{
				IRewardSource placement = rom.generatedPlacement.Find(x => x.Item == item);
				if (placement != null)
				{
					if (placement is TreasureChest && !incentiveChests.Contains(placement.Name))
					{
						hints.Add(new GeneratedHint
						{
							Coverage = flags.ExtensiveHints_LooseItemNameCoverage,
							PlacementStrategy = flags.ExtensiveHints_LooseItemNamePlacement,
							MapLocation = placement.MapLocation,
							Text = translator.TranslateItem(placement.Item) + "\nis found in\n" + translator.TranslateOverWorldLocation(placement.MapLocation) + "!"
						});
					}
				}
			}

			return hints.Distinct().ToList();
		}
	}
}
