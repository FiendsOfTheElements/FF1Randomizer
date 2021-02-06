using RomUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static FF1Lib.FF1Rom;

namespace FF1Lib
{
	public class IncNameHintSource : BaseHintSource
	{
		public IncNameHintSource(MT19337 _rng, NPCdata _npcData, Flags _flags, OverworldMap _overworldMap, FF1Rom _rom) : base(_rng, _npcData, _flags, _overworldMap, _rom)
		{
		}

		public override List<GeneratedHint> GetHints()
		{
			List<GeneratedHint> hints = GenerateIncentiveItemNameHints();

			return hints;
		}

		private List<GeneratedHint> GenerateIncentiveItemNameHints()
		{
			List<GeneratedHint> hints = new List<GeneratedHint>();

			var incentivePool = ItemLists.GetIncentivePool(flags);
			var incentiveChests = ItemLists.GetIncentiveChests(flags);

			foreach (var item in incentivePool)
			{
				var placement = rom.generatedPlacement.Find(x => x.Item == item);
				if (placement != null)
				{
					if (placement is TreasureChest && incentiveChests.Contains(placement.Name))
					{
						hints.Add(new GeneratedHint
						{
							Coverage = flags.ExtensiveHints_IncentiveItemNameCoverage,
							PlacementStrategy = flags.ExtensiveHints_IncentiveItemNamePlacement,
							MapLocation = placement.MapLocation,
							Text = translator.TranslateItem(placement.Item) + "\nis found in\n" + translator.TranslateOverWorldLocation(placement.MapLocation) + "!"
						});
					}
					else if (placement is MapObject)
					{
						hints.Add(new GeneratedHint
						{
							Coverage = flags.ExtensiveHints_IncentiveItemNameCoverage,
							PlacementStrategy = flags.ExtensiveHints_IncentiveItemNamePlacement,
							MapLocation = placement.MapLocation,
							Text = translator.TranslateItem(placement.Item) + "\nis held by\n" + translator.TranslateNpc(placement.Name) + "!"
						});

					}
					else if (placement is ItemShopSlot)
					{
						hints.Add(new GeneratedHint
						{
							Coverage = flags.ExtensiveHints_IncentiveItemNameCoverage,
							PlacementStrategy = flags.ExtensiveHints_IncentiveItemNamePlacement,
							MapLocation = placement.MapLocation,
							Text = translator.TranslateItem(placement.Item) + "\nis for sale in\n" + translator.TranslateFloor(placement.MapLocation) + "!"
						});
					}
				}
			}

			return hints.Distinct().ToList();
		}
	}
}
