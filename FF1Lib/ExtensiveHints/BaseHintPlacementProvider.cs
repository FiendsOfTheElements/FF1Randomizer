using RomUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static FF1Lib.FF1Rom;

namespace FF1Lib
{
	public abstract class BaseHintPlacementProvider : IHintPlacementProvider
	{
		protected MT19337 rng;
		protected NPCdata npcData;
		protected Flags flags;
		protected OverworldMap overworldMap;
		protected FF1Rom rom;

		public BaseHintPlacementProvider(MT19337 _rng, NPCdata _npcData, Flags _flags, OverworldMap _overworldMap, FF1Rom _rom)
		{
			rng = _rng;
			npcData = _npcData;
			flags = _flags;
			overworldMap = _overworldMap;
			rom = _rom;
		}

		public abstract IEnumerable<HintPlacementStrategy> SupportedStrategies { get; }

		public abstract List<ObjectId> GetNpcPool(GeneratedHint hint, HashSet<ObjectId> usedIds);
	}
}
