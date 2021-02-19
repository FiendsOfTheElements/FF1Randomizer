using RomUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static FF1Lib.FF1Rom;

namespace FF1Lib
{
	public class EqFloorHintSource : BaseHintSource
	{
		public EqFloorHintSource(MT19337 _rng, NPCdata _npcData, Flags _flags, OverworldMap _overworldMap, FF1Rom _rom) : base(_rng, _npcData, _flags, _overworldMap, _rom)
		{
		}

		public override List<GeneratedHint> GetHints()
		{
			List<GeneratedHint> hints = GenerateEquipmentFloorHints();

			return hints;
		}

		private List<GeneratedHint> GenerateEquipmentFloorHints()
		{
			List<Item> WowItems = ItemLists.UberTier.Concat(ItemLists.LegendaryWeaponTier).Concat(ItemLists.LegendaryArmorTier).Concat(ItemLists.RareWeaponTier).Concat(ItemLists.RareArmorTier).ToList();

			List<GeneratedHint> hints = new List<GeneratedHint>();

			foreach (var item in WowItems)
			{
				var placement = rom.generatedPlacement.Find(x => x.Item == item);
				if (placement != null)
				{
					if (placement is TreasureChest)
					{
						hints.Add(new GeneratedHint
						{
							Coverage = flags.ExtensiveHints_EquipmentFloorCoverage,
							PlacementStrategy = flags.ExtensiveHints_EquipmentFloorPlacement,
							MapLocation = placement.MapLocation,
							Text = translator.TranslateItem(placement.Item) + "\nis found in\n" + translator.TranslateFloor(placement.MapLocation) + "!"
						});
					}
				}
			}

			return hints.Distinct().ToList();
		}
	}
}
