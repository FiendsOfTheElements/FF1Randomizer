using RomUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static FF1Lib.FF1Rom;

namespace FF1Lib
{
	public abstract class BaseHintSource : IHintSource
	{
		protected MT19337 rng;
		protected NPCdata npcData;
		protected Flags flags;
		protected OverworldMap overworldMap;
		protected FF1Rom rom;

		protected Translator translator;

		public BaseHintSource(MT19337 _rng, NPCdata _npcData, Flags _flags, OverworldMap _overworldMap, FF1Rom _rom)
		{
			rng = _rng;
			npcData = _npcData;
			flags = _flags;
			overworldMap = _overworldMap;
			rom = _rom;

			translator = new Translator(rom);
		}

		public abstract List<GeneratedHint> GetHints();
	}
}
