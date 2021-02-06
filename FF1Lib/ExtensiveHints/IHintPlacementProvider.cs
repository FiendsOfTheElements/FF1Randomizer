using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FF1Lib
{
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

		[Description("Inner Sea Towns and Dwarf Cave")]
		InnerSeaTownsAndDwarfCave,
	}

	public interface IHintPlacementProvider
	{
		IEnumerable<HintPlacementStrategy> SupportedStrategies { get; }

		List<ObjectId> GetNpcPool(GeneratedHint hint, HashSet<ObjectId> usedIds); 
	}
}
