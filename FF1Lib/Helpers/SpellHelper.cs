namespace FF1Lib.Helpers
{

	public class SpellHelper
	{
		List<MagicSpell> SpellInfos;

		public SpellHelper(FF1Rom _rom)
		{
			SpellInfos = _rom.GetSpells();
		}

		public SpellHelper(List<MagicSpell> _spellInfos)
		{
		    SpellInfos = _spellInfos;
		}

		public IEnumerable<(Spell Id, MagicSpell Info)> FindSpells(SpellRoutine routine, SpellTargeting targeting,
									   SpellElement element = SpellElement.Any,
									   SpellStatus status = SpellStatus.Any,
									   OOBSpellRoutine oobSpell = OOBSpellRoutine.None
		)
		{
			IEnumerable<MagicSpell> foundSpells;

			if (routine == SpellRoutine.Life)
			{
				foundSpells = SpellInfos.Where(s =>
					(s.routine == SpellRoutine.CureAilment) &&
					(s.targeting == targeting || targeting == SpellTargeting.Any) &&
							       (s.effect == (byte)SpellStatus.Death));
			}
			else if (routine == SpellRoutine.Smoke)
			{
				foundSpells = SpellInfos.Where(s =>
					(s.routine == SpellRoutine.CureAilment) &&
					(s.targeting == targeting || targeting == SpellTargeting.Any) &&
					(s.effect == 0));
			}
			else if (routine == SpellRoutine.InflictStatus || routine == SpellRoutine.PowerWord || routine == SpellRoutine.CureAilment)
			{
				foundSpells = SpellInfos.Where(s =>
					(s.routine == routine) &&
					(s.targeting == targeting || targeting == SpellTargeting.Any) &&
					(s.elem == element || element == SpellElement.Any) &&
							       (s.effect == (byte)status || status == SpellStatus.Any));
			}
			else
			{
				foundSpells = SpellInfos.Where(s =>
					(s.routine == routine) &&
					(s.targeting == targeting || targeting == SpellTargeting.Any) &&
					(s.elem == element || element == SpellElement.Any) &&
                                        (s.oobSpellRoutine == oobSpell || oobSpell == OOBSpellRoutine.None));
			}

			return foundSpells.Select(s => ((Spell)Convert.ToByte((int)Spell.CURE + s.Index), s)).ToList();
		}

		public IEnumerable<(Spell Id, MagicSpell Info)> GetAllSpells()
		{
			//yeah stupid way of doing it, but i don't care
			return SpellInfos.Select(s => ((Spell)Convert.ToByte((int)Spell.CURE + s.Index), s)).ToList();
		}

		public IEnumerable<(Spell Id, MagicSpell Info)> GetAoEAttackSpells()
		{
				var damageAoes = FindSpells(SpellRoutine.Damage, SpellTargeting.AllEnemies);
				var instaAoes = FindSpells(SpellRoutine.InflictStatus, SpellTargeting.AllEnemies, SpellElement.Any, SpellStatus.Death);
				var powerWordAoes = FindSpells(SpellRoutine.PowerWord, SpellTargeting.AllEnemies, SpellElement.Any, SpellStatus.Death);

				return damageAoes.Concat(instaAoes).Concat(powerWordAoes);
		}
	}
}
