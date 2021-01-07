using RomUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static FF1Lib.FF1Rom;

namespace FF1Lib
{
	public class LooseFloorHintSource : BaseHintSource
	{
		int LooseCount;

		public LooseFloorHintSource(MT19337 _rng, NPCdata _npcData, Flags _flags, OverworldMap _overworldMap, FF1Rom _rom) : base(_rng, _npcData, _flags, _overworldMap, _rom)
		{
		}

		public override List<GeneratedHint> GetHints()
		{
			LooseCount = 0;
			List<GeneratedHint> hints = GenerateLooseItemFloorHints();

			return hints;
		}

		private List<GeneratedHint> GenerateLooseItemFloorHints()
		{
			List<GeneratedHint> hints = new List<GeneratedHint>();

			var incentivePool = ItemLists.GetIncentivePool(flags);
			var incentiveChests = ItemLists.GetIncentiveChests(flags);

			foreach (var item in incentivePool)
			{
				var placement = rom.generatedPlacement.Find(x => x.Item == item);
				if (placement != null)
				{
					if (placement is TreasureChest && !incentiveChests.Contains(placement.Name))
					{
						LooseCount++;
						hints.Add(new GeneratedHint
						{
							Coverage = flags.ExtensiveHints_LooseItemFloorCoverage,
							PlacementStrategy = flags.ExtensiveHints_LooseItemFloorPlacement,
							MapLocation = placement.MapLocation,
							Text = "There is {0} Loose\nfound in\n" + translator.TranslateFloor(placement.MapLocation) + "!"
						});
					}
				}
			}

			hints.Add(new GeneratedHint
			{
				Coverage = HintCategoryCoverage.HintCategoryCoveragePrioritized,
				PlacementStrategy = HintPlacementStrategy.InnerSeaTownsAndDwarfCave,
				FixedNpc = ObjectId.ConeriaOldMan,
				MapLocation = MapLocation.Coneria,
				Text = "I think there are " + LooseCount.ToString() + "\nLoose in the world.\n\nHappy Hunting!"
			});

			return hints.GroupBy(h => h.Text).Select(h => new GeneratedHint
			{
				Coverage = flags.ExtensiveHints_LooseItemFloorCoverage,
				PlacementStrategy = flags.ExtensiveHints_LooseItemFloorPlacement,
				FixedNpc = h.First().FixedNpc,
				MapLocation = h.First().MapLocation,
				Text = h.First().Text.Replace("{0}", h.Count().ToString())
			}).ToList();
		}
	}
}
