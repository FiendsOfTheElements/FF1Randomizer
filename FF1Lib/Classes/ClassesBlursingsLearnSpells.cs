using FF1Lib.Helpers;
using RomUtilities;
using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FF1Lib
{
	public partial class GameClasses
	{
		private struct SpellLearning
		{
			public Spell Name;
			public byte Id;
			public List<Classes> CanLearn;
		}

		private Dictionary<Spell, SpellLearning> spellsLearning = new();

		private void BuildSpellIdDict(FF1Rom rom)
		{
			SpellHelper spellHelper = new(rom);
			List<(Spell spell, byte id)> spellsFound = new();

			// Lamp
			var lampCandidates = spellHelper.FindSpells(SpellRoutine.CureAilment, SpellTargeting.Any).Where(s => (s.Info.effect & 0x08) > 0).Select(x => (byte)x.Id).ToList();
			spellsFound.Add((Spell.LAMP, lampCandidates.Any() ? (byte)(lampCandidates.First() - 0xB0 + 1) : (byte)0xFF));

			// A-Spells
			var aSpellsCandidates = spellHelper.FindSpells(SpellRoutine.DefElement, SpellTargeting.AllCharacters).ToList();
			spellsFound.Add((Spell.AFIR, aSpellsCandidates.TryFind(s => (s.Info.effect & 0x10) > 0, out var afirespell) ? (byte)(afirespell.Id - 0xB0 + 1) : (byte)0xFF));
			spellsFound.Add((Spell.AICE, aSpellsCandidates.TryFind(s => (s.Info.effect & 0x20) > 0, out var aicespell) ? (byte)(aicespell.Id - 0xB0 + 1) : (byte)0xFF));
			spellsFound.Add((Spell.ALIT, aSpellsCandidates.TryFind(s => (s.Info.effect & 0x40) > 0, out var alitspell) ? (byte)(alitspell.Id - 0xB0 + 1) : (byte)0xFF));
			spellsFound.Add((Spell.ARUB, aSpellsCandidates.TryFind(s => (s.Info.effect & 0x80) > 0, out var arubspell) ? (byte)(arubspell.Id - 0xB0 + 1) : (byte)0xFF));

			var aMuteCandidates = spellHelper.FindSpells(SpellRoutine.CureAilment, SpellTargeting.Any).Where(s => (s.Info.effect & 0x40) > 0).Select(x => (byte)x.Id).ToList();
			spellsFound.Add((Spell.AMUT, aMuteCandidates.Any() ? (byte)(aMuteCandidates.First() - 0xB0 + 1) : (byte)0xFF));

			// Dark
			var darkCandidates = spellHelper.FindSpells(SpellRoutine.InflictStatus, SpellTargeting.Any).ToList();
			spellsFound.Add((Spell.DARK, darkCandidates.TryFind(s => (s.Info.effect & 0x08) > 0, out var darkspell) ? (byte)(darkspell.Id - 0xB0 + 1) : (byte)0xFF));

			// Sleeps
			var sleepCandidates = spellHelper.FindSpells(SpellRoutine.InflictStatus, SpellTargeting.Any).ToList();
			spellsFound.Add((Spell.SLEP, darkCandidates.TryFind(s => (s.Info.effect & 0x20) > 0 && s.Info.targeting == SpellTargeting.AllEnemies, out var slepspell) ? (byte)(slepspell.Id - 0xB0 + 1) : (byte)0xFF));
			spellsFound.Add((Spell.SLP2, darkCandidates.TryFind(s => (s.Info.effect & 0x20) > 0 && s.Info.targeting == SpellTargeting.OneEnemy, out var slp2spell) ? (byte)(slp2spell.Id - 0xB0 + 1) : (byte)0xFF));

			// Slows
			var slowCandidates = spellHelper.FindSpells(SpellRoutine.Slow, SpellTargeting.Any).ToList();
			spellsFound.Add((Spell.SLOW, slowCandidates.TryFind(s => s.Info.targeting == SpellTargeting.AllEnemies, out var slowspell) ? (byte)(slowspell.Id - 0xB0 + 1) : (byte)0xFF));
			spellsFound.Add((Spell.SLO2, slowCandidates.TryFind(s =>s.Info.targeting == SpellTargeting.OneEnemy, out var slo2spell) ? (byte)(slo2spell.Id - 0xB0 + 1) : (byte)0xFF));


			foreach (var spell in spellsFound)
			{
				SpellSlots spellslot = (SpellSlots)(spell.id - 1);
				List<Classes> learners = _spellPermissions.PermissionsFor(spellslot);
				learners = learners.Select(l => (int)l >= 6 ? (l - 6) : l).Distinct().ToList();

				spellsLearning.Add(spell.spell, new SpellLearning() { Name = spell.spell, Id = spell.id, CanLearn = new(learners) });
			}
		}
		private List<BonusMalus> CreateSpellLearningBlessings(FF1Rom rom)
		{
			BuildSpellIdDict(rom);

			List<BonusMalus> strongBlessings = new();

			if (spellsLearning[Spell.LAMP].Id != 0xFF)
			{
				strongBlessings.Add(new BonusMalus(BonusMalusAction.LearnLampRibbon, "Learn LAMP\n Resist All", Classes: spellsLearning[Spell.LAMP].CanLearn));
			}

			if (spellsLearning[Spell.DARK].Id != 0xFF)
			{
				strongBlessings.Add(new BonusMalus(BonusMalusAction.LearDarkEvade, "Learn DARK\n +Evade", Classes: spellsLearning[Spell.DARK].CanLearn));
			}

			if (spellsLearning[Spell.SLEP].Id != 0xFF || spellsLearning[Spell.SLP2].Id != 0xFF)
			{
				var allLearners = spellsLearning[Spell.SLEP].CanLearn.Concat(spellsLearning[Spell.SLP2].CanLearn).Distinct().ToList();
				strongBlessings.Add(new BonusMalus(BonusMalusAction.LearnSleepMDef, "Learn SLEEP\n +MDef", Classes: allLearners));
			}

			if (spellsLearning[Spell.SLOW].Id != 0xFF || spellsLearning[Spell.SLO2].Id != 0xFF)
			{
				var allLearners = spellsLearning[Spell.SLOW].CanLearn.Concat(spellsLearning[Spell.SLO2].CanLearn).Distinct().ToList();
				strongBlessings.Add(new BonusMalus(BonusMalusAction.LearnSlowAbsorb, "Learn SLOW\n +Absorb", Classes: allLearners));
			}

			if (spellsLearning[Spell.ARUB].Id != 0xFF || spellsLearning[Spell.AFIR].Id != 0xFF || spellsLearning[Spell.AICE].Id != 0xFF || spellsLearning[Spell.ALIT].Id != 0xFF || spellsLearning[Spell.AMUT].Id != 0xFF) 
			{
				var learners = spellsLearning[Spell.ARUB].CanLearn;
				var weakLearners = spellsLearning[Spell.AFIR].CanLearn.Concat(spellsLearning[Spell.AICE].CanLearn).Concat(spellsLearning[Spell.ALIT].CanLearn).Concat(spellsLearning[Spell.AMUT].CanLearn).GroupBy(l => l);

				learners.AddRange(weakLearners.Where(l => l.Count() > 2).Select(l => l.Key));

				var allLearners = learners.Distinct().ToList();
				strongBlessings.Add(new BonusMalus(BonusMalusAction.ASpellsAutocast, "A-Spells\n Autocast", Classes: allLearners));
			}

			return strongBlessings;
		}
	}
}
