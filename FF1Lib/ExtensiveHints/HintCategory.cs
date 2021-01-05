using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FF1Lib
{
	public enum HintCategory
	{
		LooseItemFloor, //Loose in MarshTop
		LooseItemName, //Lute in Earth
		IncentiveItemName, //Lute in Earth
		FloorHint, //Earth1 is a Dud
		EquipmentFloor, //Legendary Weapon in Earth2
		EquipmentName //Masamune in Sky
	}

	public enum HintCategoryCoverage
	{
		[Description("None - Guaranteed")]
		HintCategoryCoverageNone = 0,

		[Description("20% - Best Effort")]
		HintCategoryCoverage20,

		[Description("40% - Best Effort")]
		HintCategoryCoverage40,

		[Description("60% - Best Effort")]
		HintCategoryCoverage60,

		[Description("80% - Best Effort")]
		HintCategoryCoverage80,

		[Description("All - Best Effort")]
		HintCategoryCoverageAll,

		[Description("All - Prioritized")]
		HintCategoryCoveragePrioritized,
	}

	public enum HintPlacementStrategy
	{
		[Description("Coneria - DwarfCave")]
		InnerSea,

		[Description("Coneria - CrescentLake")]
		ConeriaToCrescent,

		[Description("Elfland - CrescentLake")]
		ElflandToCrescent,

		[Description("Elfland - Gaia")]
		ElflandPlus,

		[Description("Melmond - Onrac")]
		MelmondOnrac,

		[Description("Melmond - Mermaids")]
		MelmondMermaids,

		[Description("Melmond - Gaia")]
		MelmondPlus,

		[Description("Cardia, Lefane, Gaia")]
		FloaterRequired,

		[Description("Everywhere")]
		Everywhere,

		[Description("Depending on Dungeon")]
		Tiered,
	}
}
