using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static FF1Lib.FF1Rom;
using FF1Lib;

namespace FF1Lib.Helpers
{

	public class SpellHelper
	{
		FF1Rom rom;

		List<SpellInfo> SpellInfos;

		public SpellHelper(FF1Rom _rom)
		{
			rom = _rom;

			SpellInfos = rom.LoadSpells().ToList();
		}

		public IEnumerable<(Spell Id, SpellInfo Info)> FindSpells(SpellRoutine routine, SpellTargeting targeting, SpellElement element = SpellElement.Any, SpellStatus status = SpellStatus.Any)
		{
			IEnumerable<SpellInfo> foundSpells;

			if (routine == SpellRoutine.Life)
			{
				foundSpells = SpellInfos.Where(s =>
					(s.routine == (byte)SpellRoutine.CureAilment) &&
					(s.targeting == (byte)targeting || targeting == SpellTargeting.Any) &&
					(s.effect == (byte)SpellStatus.Death));
			}
			else if (routine == SpellRoutine.Smoke)
			{
				foundSpells = SpellInfos.Where(s =>
					(s.routine == (byte)SpellRoutine.CureAilment) &&
					(s.targeting == (byte)targeting || targeting == SpellTargeting.Any) &&
					(s.effect == 0));
			}
			else if (routine == SpellRoutine.InflictStatus || routine == SpellRoutine.PowerWord || routine == SpellRoutine.CureAilment)
			{
				foundSpells = SpellInfos.Where(s =>
					(s.routine == (byte)routine) &&
					(s.targeting == (byte)targeting || targeting == SpellTargeting.Any) &&
					(s.elem == (byte)element || element == SpellElement.Any) &&
					(s.effect == (byte)status || status == SpellStatus.Any));
			}
			else
			{
				foundSpells = SpellInfos.Where(s =>
					(s.routine == (byte)routine) &&
					(s.targeting == (byte)targeting || targeting == SpellTargeting.Any) &&
					(s.elem == (byte)element || element == SpellElement.Any));
			}

			return foundSpells.Select(s => ((Spell)Convert.ToByte((int)Spell.CURE + SpellInfos.IndexOf(s)), s)).ToList();
		}

		public IEnumerable<(Spell Id, SpellInfo Info)> GetAllSpells()
		{
			//yeah stupid way of doing it, but i don't care
			return SpellInfos.Select(s => ((Spell)Convert.ToByte((int)Spell.CURE + SpellInfos.IndexOf(s)), s)).ToList();
		}
	}
}
