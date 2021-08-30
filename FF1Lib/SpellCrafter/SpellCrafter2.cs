using RomUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FF1Lib.SpellCrafter
{
	public class SpellCrafter2
	{
		MT19337 rng;
		Flags flags;
		FF1Rom rom;
		SpellHelper helper;

		private List<Spell> Spells;

		private HashSet<int> SpellIndices;

		public SpellCrafter2(MT19337 _rng, Flags _flags, FF1Rom _rom)
		{
			rng = _rng;
			flags = _flags;
			rom = _rom;
			helper = new SpellHelper(rom);
		}

		public void CraftSpells()
		{
			for (int i = 0; i < 8; i++)
			{
				if (TryCraftSpells())
				{
					FixPermissions();
					SpellHelper.WriteAllSpells(rom, Spells);
					return;
				}
			}

			throw new InsaneException("Nope, spellgeneration failed!");
		}

		private void FixPermissions()
		{
			foreach (var s in Spells)
			{
				if (s.Level > 3) s.Permissions = s.Permissions & (SpellPermission)(SpellPermission.All - SpellPermission.Knight);
				if (s.Level > 4) s.Permissions = s.Permissions & (SpellPermission)(SpellPermission.All - SpellPermission.Ninja);
				if (s.Level > 5) s.Permissions = s.Permissions & (SpellPermission)(SpellPermission.All - SpellPermission.RedMage);
			}
		}

		private bool TryCraftSpells()
		{
			SpellIndices = new HashSet<int>();
			Spells = new List<Spell>();

			for (int i = 0; i < FF1Rom.MagicCount; i++) SpellIndices.Add(i);

			CraftGuaranteedSpells();
			CraftSemiGuaranteedSpells();

			return CraftFillerSpells();
		}

		private void CraftGuaranteedSpells()
		{
			int lvl;

			lvl = CraftCureSpell(SpellSchool.White, 1, 3, 1);
			lvl = CraftCureSpell(SpellSchool.White, Math.Max(lvl + 1, 2), 5, 2);
			lvl = CraftCureSpell(SpellSchool.White, Math.Max(lvl + 1, 4), 7, 3);
			lvl = CraftCureSpell(SpellSchool.White, Math.Max(lvl + 1, 6), 8, 4);


			lvl = CraftHealSpell(SpellSchool.White, 1, 4, 1);
			lvl = CraftHealSpell(SpellSchool.White, Math.Max(lvl + 1, 3), 6, 2);
			lvl = CraftHealSpell(SpellSchool.White, Math.Max(lvl + 1, 5), 8, 3);


			lvl = CraftLifeSpell(SpellSchool.White, 5, 7, 1);
			lvl = CraftLifeSpell(SpellSchool.White, Math.Max(lvl + 1, 6), 8, 2);


			lvl = CraftWarpExitSpell(SpellSchool.White, 1, 4, 1);
			lvl = CraftWarpExitSpell(SpellSchool.White, Math.Max(lvl + 1, 3), 6, 2);


			lvl = CraftPureSoftSpell(SpellSchool.White, 1, 3, 1);
			lvl = CraftPureSoftSpell(SpellSchool.White, 1, 3, 2);

			lvl = CraftSmokeSpell(SpellSchool.White, 1, 3, 1);
			lvl = CraftVoxSpell(SpellSchool.White, 1, 3, 1);
		}

		private void CraftSemiGuaranteedSpells()
		{
			int lvl;
			int set;
			List<int> sets;

			lvl = 0;
			sets = GetCombinationSet(2, 1, 2);
			set = sets.PickRandom(rng);
			if ((set & 0x01) > 0) lvl = CraftFastSpell(SpellSchool.White, 2, 5, 1);
			if ((set & 0x02) > 0) lvl = CraftFastSpell(SpellSchool.White, Math.Max(lvl + 1, 4), 7, 2);

			lvl = 0;
			sets = GetCombinationSet(6, 2, 3);
			set = sets.PickRandom(rng);
			if ((set & 0x01) > 0) lvl = CraftTemperSpell(SpellSchool.Black, 2, 5, 1);
			if ((set & 0x02) > 0) lvl = CraftTemperSpell(SpellSchool.Black, Math.Max(lvl + 1, 4), 7, 2);
			if ((set & 0x04) > 0) lvl = CraftTemperSpell(SpellSchool.Black, 2, 5, 3);
			if ((set & 0x08) > 0) lvl = CraftTemperSpell(SpellSchool.Black, Math.Max(lvl + 1, 4), 7, 4);
			if ((set & 0x10) > 0) lvl = CraftTemperSpell(SpellSchool.Black, 2, 5, 5);
			if ((set & 0x20) > 0) lvl = CraftTemperSpell(SpellSchool.Black, Math.Max(lvl + 1, 4), 7, 6);

			lvl = 0;
			sets = GetCombinationSet(6, 2, 4);
			set = sets.PickRandom(rng);
			if ((set & 0x01) > 0) lvl = CraftRuseSpell(SpellSchool.White, 1, 3, 1);
			if ((set & 0x02) > 0) lvl = CraftRuseSpell(SpellSchool.White, Math.Max(lvl + 1, 3), 6, 2);
			if ((set & 0x04) > 0) lvl = CraftRuseSpell(SpellSchool.White, 3, 5, 3);
			if ((set & 0x08) > 0) lvl = CraftRuseSpell(SpellSchool.White, Math.Max(lvl + 1, 5), 7, 4);
			if ((set & 0x10) > 0) lvl = CraftRuseSpell(SpellSchool.White, 5, 6, 5);
			if ((set & 0x20) > 0) lvl = CraftRuseSpell(SpellSchool.White, Math.Max(lvl + 1, 6), 8, 6);

			lvl = 0;
			sets = GetCombinationSet(4, 2, 3);
			set = sets.PickRandom(rng);
			if ((set & 0x01) > 0) lvl = CraftLockSpell(SpellSchool.Black, 2, 5, 1);
			if ((set & 0x02) > 0) lvl = CraftLockSpell(SpellSchool.Black, Math.Max(lvl + 1, 4), 7, 2);
			if ((set & 0x04) > 0) lvl = CraftLockSpell(SpellSchool.Black, 2, 5, 3);
			if ((set & 0x08) > 0) lvl = CraftLockSpell(SpellSchool.Black, Math.Max(lvl + 1, 4), 7, 4);

			lvl = 0;
			sets = GetCombinationSet(4, 2, 4);
			set = sets.PickRandom(rng);
			if ((set & 0x01) > 0) lvl = CraftFogSpell(SpellSchool.Black, 2, 5, 1);
			if ((set & 0x02) > 0) lvl = CraftFogSpell(SpellSchool.Black, Math.Max(lvl + 1, 4), 7, 2);
			if ((set & 0x04) > 0) lvl = CraftFogSpell(SpellSchool.Black, 2, 5, 3);
			if ((set & 0x08) > 0) lvl = CraftFogSpell(SpellSchool.Black, Math.Max(lvl + 1, 4), 7, 4);

			lvl = 0;
			sets = GetCombinationSet(6, 2, 4);
			set = sets.PickRandom(rng);
			if ((set & 0x01) > 0) lvl = CraftWallSpell(SpellSchool.White, 2, 5, 1);
			if ((set & 0x02) > 0) lvl = CraftWallSpell(SpellSchool.White, Math.Max(lvl + 1, 4), 7, 2);
			if ((set & 0x04) > 0) lvl = CraftWallSpell(SpellSchool.White, 2, 5, 3);
			if ((set & 0x08) > 0) lvl = CraftWallSpell(SpellSchool.White, Math.Max(lvl + 1, 4), 7, 4);
			if ((set & 0x10) > 0) lvl = CraftWallSpell(SpellSchool.White, 4, 6, 5);
			if ((set & 0x20) > 0) lvl = CraftWallSpell(SpellSchool.White, Math.Max(lvl + 1, 6), 8, 6);

			lvl = 0;
			sets = GetCombinationSet(2, 1, 1);
			set = sets.PickRandom(rng);
			if ((set & 0x01) > 0) lvl = CraftSlowSpell(SpellSchool.Black, 1, 3, 1);
			if ((set & 0x02) > 0) lvl = CraftSlowSpell(SpellSchool.Black, Math.Max(lvl + 1, 2), 5, 2);

			lvl = 0;
			sets = GetCombinationSet(2, 1, 1);
			set = sets.PickRandom(rng);
			if ((set & 0x01) > 0) lvl = CraftXferSpell(SpellSchool.Black, 5, 7, 1);
			if ((set & 0x02) > 0) lvl = CraftXferSpell(SpellSchool.Black, Math.Max(lvl + 1, 6), 8, 2);

			lvl = 0;
			sets = GetCombinationSet(4, 2, 2);
			set = sets.PickRandom(rng);
			if ((set & 0x01) > 0) lvl = CraftFlareSpell(SpellSchool.White, 4, 7, 1);
			if ((set & 0x02) > 0) lvl = CraftFlareSpell(SpellSchool.White, Math.Max(lvl + 1, 4), 8, 2);
			if ((set & 0x04) > 0) lvl = CraftFlareSpell(SpellSchool.White, 6, 7, 3);
			if ((set & 0x08) > 0) lvl = CraftFlareSpell(SpellSchool.White, Math.Max(lvl + 1, 4), 8, 4);

			lvl = 0;
			sets = GetCombinationSet(2, 1, 1);
			set = sets.PickRandom(rng);
			if ((set & 0x01) > 0) lvl = CraftFadeSpell(SpellSchool.White, 6, 8, 1);
			if ((set & 0x02) > 0) lvl = CraftFadeSpell(SpellSchool.White, 6, 8, 2);

			lvl = 0;
			sets = GetCombinationSet(4, 1, 3);
			set = sets.PickRandom(rng);
			if ((set & 0x01) > 0) lvl = CraftWhiteStatusSpell(SpellSchool.White, 1, 4, 1);
			if ((set & 0x02) > 0) lvl = CraftWhiteStatusSpell(SpellSchool.White, 5, 8, 2);
			if ((set & 0x04) > 0) lvl = CraftWhiteStatusSpell(SpellSchool.White, 1, 4, 3);
			if ((set & 0x08) > 0) lvl = CraftWhiteStatusSpell(SpellSchool.White, 5, 8, 4);

			lvl = 0;
			sets = GetCombinationSet(4, 1, 3);
			set = sets.PickRandom(rng);
			if ((set & 0x01) > 0) lvl = CraftWhiteStatusSpell(SpellSchool.White, 1, 4, 5);
			if ((set & 0x02) > 0) lvl = CraftWhiteStatusSpell(SpellSchool.White, 5, 8, 6);
			if ((set & 0x04) > 0) lvl = CraftWhiteStatusSpell(SpellSchool.White, 1, 4, 7);
			if ((set & 0x08) > 0) lvl = CraftWhiteStatusSpell(SpellSchool.White, 5, 8, 8);

			lvl = 0;
			sets = GetCombinationSet(2, 1, 1);
			set = sets.PickRandom(rng);
			if ((set & 0x01) > 0) lvl = CraftNukeSpell(SpellSchool.Black, 6, 8, 1);
			if ((set & 0x02) > 0) lvl = CraftNukeSpell(SpellSchool.Black, 6, 8, 2);

			lvl = 0;
			sets = GetCombinationSet(4, 1, 2);
			set = sets.PickRandom(rng);
			if ((set & 0x01) > 0) lvl = CraftBlackStatusSpell(SpellSchool.Black, 1, 4, 1);
			if ((set & 0x02) > 0) lvl = CraftBlackStatusSpell(SpellSchool.Black, 5, 8, 2);
			if ((set & 0x04) > 0) lvl = CraftBlackStatusSpell(SpellSchool.Black, 1, 4, 3);
			if ((set & 0x08) > 0) lvl = CraftBlackStatusSpell(SpellSchool.Black, 5, 8, 4);

			lvl = 0;
			sets = GetCombinationSet(4, 1, 2);
			set = sets.PickRandom(rng);
			if ((set & 0x01) > 0) lvl = CraftBlackStatusSpell(SpellSchool.Black, 1, 4, 5);
			if ((set & 0x02) > 0) lvl = CraftBlackStatusSpell(SpellSchool.Black, 5, 8, 6);
			if ((set & 0x04) > 0) lvl = CraftBlackStatusSpell(SpellSchool.Black, 1, 4, 7);
			if ((set & 0x08) > 0) lvl = CraftBlackStatusSpell(SpellSchool.Black, 5, 8, 8);

			lvl = 0;
			sets = GetCombinationSet(4, 1, 2);
			set = sets.PickRandom(rng);
			if ((set & 0x01) > 0) lvl = CraftBlackStatusSpell(SpellSchool.Black, 1, 4, 9);
			if ((set & 0x02) > 0) lvl = CraftBlackStatusSpell(SpellSchool.Black, 5, 8, 10);
			if ((set & 0x04) > 0) lvl = CraftBlackStatusSpell(SpellSchool.Black, 1, 4, 11);
			if ((set & 0x08) > 0) lvl = CraftBlackStatusSpell(SpellSchool.Black, 5, 8, 12);
		}

		//White 20-25(rem: 00-06)
		//Black 08-12(rem: 13-20)

		private bool CraftFillerSpells()
		{
			CraftHarmSpell(SpellSchool.White, 1, 4, 1);
			CraftHarmSpell(SpellSchool.White, 5, 8, 2);
			CraftHarmSpell(SpellSchool.White, 1, 4, 3);
			CraftHarmSpell(SpellSchool.White, 5, 8, 4);

			for(int i = 1; i <= 8; i++) CraftLampSpell(SpellSchool.White, 1, 8, i);

			var remWhite = SpellIndices.Where(i => Spell.GetSpellSchool(i) == SpellSchool.White).Count();
			if (remWhite > 0) return false;

			var remBlack = SpellIndices.Where(i => Spell.GetSpellSchool(i) == SpellSchool.Black).Count();

			var instaBlack = remBlack / 2;

			var stInstaBlack = rng.Between(1, instaBlack / 3);
			var pstInstaBlack = rng.Between(1, instaBlack / 6);

			instaBlack = instaBlack - stInstaBlack - pstInstaBlack;

			var paoeInstaBlack = instaBlack / 4;
			var aoeInstaBlack = instaBlack - paoeInstaBlack;

			var sets = GetCombinationSet(6, stInstaBlack, stInstaBlack);
			var set = sets.PickRandom(rng);
			if ((set & 0x01) > 0) CraftInstaKillSpell(SpellSchool.Black, 1, 4, false, SpellTargeting.OneEnemy, SpellElement.Death);
			if ((set & 0x02) > 0) CraftInstaKillSpell(SpellSchool.Black, 1, 4, false, SpellTargeting.OneEnemy, SpellElement.Poison);
			if ((set & 0x04) > 0) CraftInstaKillSpell(SpellSchool.Black, 1, 4, false, SpellTargeting.OneEnemy, SpellElement.Time);
			if ((set & 0x08) > 0) CraftInstaKillSpell(SpellSchool.Black, 1, 4, false, SpellTargeting.OneEnemy, SpellElement.Earth);
			if ((set & 0x10) > 0) CraftInstaKillSpell(SpellSchool.Black, 1, 4, false, SpellTargeting.OneEnemy, SpellElement.Fire);
			if ((set & 0x20) > 0) CraftInstaKillSpell(SpellSchool.Black, 1, 4, false, SpellTargeting.OneEnemy, SpellElement.None);

			sets = GetCombinationSet(6, pstInstaBlack, pstInstaBlack);
			set = sets.PickRandom(rng);
			if ((set & 0x01) > 0) CraftInstaKillSpell(SpellSchool.Black, 1, 4, true, SpellTargeting.OneEnemy, SpellElement.Death);
			if ((set & 0x02) > 0) CraftInstaKillSpell(SpellSchool.Black, 1, 4, true, SpellTargeting.OneEnemy, SpellElement.Poison);
			if ((set & 0x04) > 0) CraftInstaKillSpell(SpellSchool.Black, 1, 4, true, SpellTargeting.OneEnemy, SpellElement.Time);
			if ((set & 0x08) > 0) CraftInstaKillSpell(SpellSchool.Black, 1, 4, true, SpellTargeting.OneEnemy, SpellElement.Earth);
			if ((set & 0x10) > 0) CraftInstaKillSpell(SpellSchool.Black, 1, 4, true, SpellTargeting.OneEnemy, SpellElement.Fire);
			if ((set & 0x20) > 0) CraftInstaKillSpell(SpellSchool.Black, 1, 4, true, SpellTargeting.OneEnemy, SpellElement.None);

			sets = GetCombinationSet(6, aoeInstaBlack, aoeInstaBlack);
			set = sets.PickRandom(rng);
			if ((set & 0x01) > 0) CraftInstaKillSpell(SpellSchool.Black, 5, 8, false, SpellTargeting.AllEnemies, SpellElement.Death);
			if ((set & 0x02) > 0) CraftInstaKillSpell(SpellSchool.Black, 5, 8, false, SpellTargeting.AllEnemies, SpellElement.Poison);
			if ((set & 0x04) > 0) CraftInstaKillSpell(SpellSchool.Black, 5, 8, false, SpellTargeting.AllEnemies, SpellElement.Time);
			if ((set & 0x08) > 0) CraftInstaKillSpell(SpellSchool.Black, 5, 8, false, SpellTargeting.AllEnemies, SpellElement.Earth);
			if ((set & 0x10) > 0) CraftInstaKillSpell(SpellSchool.Black, 5, 8, false, SpellTargeting.AllEnemies, SpellElement.Fire);
			if ((set & 0x20) > 0) CraftInstaKillSpell(SpellSchool.Black, 5, 8, false, SpellTargeting.AllEnemies, SpellElement.None);

			sets = GetCombinationSet(6, aoeInstaBlack, aoeInstaBlack);
			set = sets.PickRandom(rng);
			if ((set & 0x01) > 0) CraftInstaKillSpell(SpellSchool.Black, 5, 8, true, SpellTargeting.AllEnemies, SpellElement.Death);
			if ((set & 0x02) > 0) CraftInstaKillSpell(SpellSchool.Black, 5, 8, true, SpellTargeting.AllEnemies, SpellElement.Poison);
			if ((set & 0x04) > 0) CraftInstaKillSpell(SpellSchool.Black, 5, 8, true, SpellTargeting.AllEnemies, SpellElement.Time);
			if ((set & 0x08) > 0) CraftInstaKillSpell(SpellSchool.Black, 5, 8, true, SpellTargeting.AllEnemies, SpellElement.Earth);
			if ((set & 0x10) > 0) CraftInstaKillSpell(SpellSchool.Black, 5, 8, true, SpellTargeting.AllEnemies, SpellElement.Fire);
			if ((set & 0x20) > 0) CraftInstaKillSpell(SpellSchool.Black, 5, 8, true, SpellTargeting.AllEnemies, SpellElement.None);

			var blackIndices = SpellIndices.Where(i => Spell.GetSpellSchool(i) == SpellSchool.Black).OrderBy(i => i).ToList();

			var elements = new List<SpellElement> { SpellElement.Earth, SpellElement.Fire, SpellElement.Ice, SpellElement.Lightning };

			var elementList = new List<SpellElement>();

			elements.Shuffle(rng);
			elementList.AddRange(elements);

			elements.Shuffle(rng);
			elementList.AddRange(elements);

			elements.Shuffle(rng);
			elementList.AddRange(elements);

			elements.Shuffle(rng);
			elementList.AddRange(elements);

			for (int i = 0; i < blackIndices.Count; i++)
			{
				if (i < blackIndices.Count / 4) CraftDamageSpell(blackIndices[i], 1, elementList[i]);
				else if (i < blackIndices.Count / 2) CraftDamageSpell(blackIndices[i], 2, elementList[i]);
				else if (i < blackIndices.Count * 3 / 4) CraftDamageSpell(blackIndices[i], 3, elementList[i]);
				else CraftDamageSpell(blackIndices[i], 4, elementList[i]);
			}

			for (int i = 1; i <= 8; i++) CraftDarkSpell(SpellSchool.Black, 1, 8, i);

			remBlack = SpellIndices.Where(i => Spell.GetSpellSchool(i) == SpellSchool.Black).Count();
			if (remBlack > 0) return false;

			if (Spells.Select(s => s.Name).Distinct().Count() != FF1Rom.MagicCount) return false;

			return true;
		}

		private int CraftCureSpell(SpellSchool school, int lolvl, int hilvl, int tier)
		{
			var index = GetSpellIndex(school, lolvl, hilvl);
			if (index < 0) return 0;

			switch (tier)
			{
				case 1:
					Spells.Add(new Spell
					{
						Index = index,
						Name = "CUR1",
						Type = SpellRoutine.Heal,
						Targeting = SpellTargeting.OneCharacters,
						Effectivity = (byte)rng.Between(24, 48),
						Permissions = SpellPermission.WhiteMage |SpellPermission.WhiteWizard | SpellPermission.RedMage | SpellPermission.RedWizard | SpellPermission.Knight,
						GraphicEffect = SpellGraphic.Stars,
						Palette = SpellPalette.DarkGreen,
						Message = SpellMessage.HpUp
					});
					helper.SetOOBSpellEffect(OOBSpellEffects.CURE, index, "291F0910");
					SpellIndices.Remove(index);
					return Spell.GetSpellLevel(index);
				case 2:
					Spells.Add(new Spell
					{
						Index = index,
						Name = "CUR2",
						Type = SpellRoutine.Heal,
						Targeting = SpellTargeting.OneCharacters,
						Effectivity = (byte)rng.Between(48, 96),
						Permissions = SpellPermission.WhiteMage | SpellPermission.WhiteWizard | SpellPermission.RedMage | SpellPermission.RedWizard | SpellPermission.Knight,
						GraphicEffect = SpellGraphic.Stars,
						Palette = SpellPalette.LightGreen,
						Message = SpellMessage.HpUp
					});
					helper.SetOOBSpellEffect(OOBSpellEffects.CUR2, index, "293F0920");
					SpellIndices.Remove(index);
					return Spell.GetSpellLevel(index);
				case 3:
					Spells.Add(new Spell
					{
						Index = index,
						Name = "CUR3",
						Type = SpellRoutine.Heal,
						Targeting = SpellTargeting.OneCharacters,
						Effectivity = (byte)rng.Between(96, 192),
						Permissions = SpellPermission.WhiteMage | SpellPermission.WhiteWizard | SpellPermission.RedWizard,
						GraphicEffect = SpellGraphic.Stars,
						Palette = SpellPalette.MintGreen,
						Message = SpellMessage.HpUp
					});
					helper.SetOOBSpellEffect(OOBSpellEffects.CUR3, index, "297F0940");
					SpellIndices.Remove(index);
					return Spell.GetSpellLevel(index);
				case 4:
					Spells.Add(new Spell
					{
						Index = index,
						Name = "CUR4",
						Type = SpellRoutine.FullHeal,
						Targeting = SpellTargeting.OneCharacters,
						Permissions = SpellPermission.WhiteWizard,
						GraphicEffect = SpellGraphic.Stars,
						Palette = SpellPalette.Teal,
						Message = SpellMessage.HpMax
					});
					helper.SetOOBSpellEffect(OOBSpellEffects.CUR4, index);
					SpellIndices.Remove(index);
					return Spell.GetSpellLevel(index);
				default:
					return 0;
			}
		}

		private int CraftHealSpell(SpellSchool school, int lolvl, int hilvl, int tier)
		{
			var index = GetSpellIndex(school, lolvl, hilvl);
			if (index < 0) return 0;

			switch (tier)
			{
				case 1:
					Spells.Add(new Spell
					{
						Index = index,
						Name = "HEL1",
						Type = SpellRoutine.Heal,
						Targeting = SpellTargeting.AllCharacters,
						Effectivity = (byte)rng.Between(12, 24),
						Permissions = SpellPermission.WhiteMage | SpellPermission.WhiteWizard | SpellPermission.RedWizard,
						GraphicEffect = SpellGraphic.Stars,
						Palette = SpellPalette.Brown,
						Message = SpellMessage.HpUp
					});
					helper.SetOOBSpellEffect(OOBSpellEffects.HEAL, index, "290F186908");
					SpellIndices.Remove(index);
					return Spell.GetSpellLevel(index);
				case 2:
					Spells.Add(new Spell
					{
						Index = index,
						Name = "HEL2",
						Type = SpellRoutine.Heal,
						Targeting = SpellTargeting.AllCharacters,
						Effectivity = (byte)rng.Between(24, 48),
						Permissions = SpellPermission.WhiteMage | SpellPermission.WhiteWizard | SpellPermission.RedWizard,
						GraphicEffect = SpellGraphic.Stars,
						Palette = SpellPalette.Orange,
						Message = SpellMessage.HpUp
					});
					helper.SetOOBSpellEffect(OOBSpellEffects.HEL2, index, "291F186910");
					SpellIndices.Remove(index);
					return Spell.GetSpellLevel(index);
				case 3:
					Spells.Add(new Spell
					{
						Index = index,
						Name = "HEL3",
						Type = SpellRoutine.Heal,
						Targeting = SpellTargeting.AllCharacters,
						Effectivity = (byte)rng.Between(48, 96),
						Permissions = SpellPermission.WhiteWizard,
						GraphicEffect = SpellGraphic.Stars,
						Palette = SpellPalette.Red,
						Message = SpellMessage.HpUp
					});
					helper.SetOOBSpellEffect(OOBSpellEffects.HEL3, index, "293F186920");
					SpellIndices.Remove(index);
					return Spell.GetSpellLevel(index);
				default:
					return 0;
			}
		}

		private int CraftLifeSpell(SpellSchool school, int lolvl, int hilvl, int tier)
		{
			var index = GetSpellIndex(school, lolvl, hilvl);
			if (index < 0) return 0;

			switch (tier)
			{
				case 1:
					Spells.Add(new Spell
					{
						Index = index,
						Name = "LIF1",
						Type = SpellRoutine.None,
						Permissions = SpellPermission.WhiteMage | SpellPermission.WhiteWizard | SpellPermission.RedWizard,
						Tag = SpellTag.LIFE
					});
					helper.SetOOBSpellEffect(OOBSpellEffects.LIFE, index);
					SpellIndices.Remove(index);
					return Spell.GetSpellLevel(index);
				case 2:
					Spells.Add(new Spell
					{
						Index = index,
						Name = "LIF2",
						Type = SpellRoutine.None,
						Permissions = SpellPermission.WhiteWizard,
						Tag = SpellTag.LIF2
					});
					helper.SetOOBSpellEffect(OOBSpellEffects.LIF2, index);
					SpellIndices.Remove(index);
					return Spell.GetSpellLevel(index);
				default:
					return 0;
			}
		}

		private int CraftWarpExitSpell(SpellSchool school, int lolvl, int hilvl, int tier)
		{
			var index = GetSpellIndex(school, lolvl, hilvl);
			if (index < 0) return 0;

			switch (tier)
			{
				case 1:
					Spells.Add(new Spell
					{
						Index = index,
						Name = "EXIT",
						Type = SpellRoutine.None,
						Permissions = SpellPermission.WhiteMage | SpellPermission.WhiteWizard | SpellPermission.RedWizard,
						Tag = SpellTag.EXIT
					});
					helper.SetOOBSpellEffect(OOBSpellEffects.EXIT, index);
					SpellIndices.Remove(index);
					return Spell.GetSpellLevel(index);
				case 2:
					Spells.Add(new Spell
					{
						Index = index,
						Name = "WARP",
						Type = SpellRoutine.None,
						Permissions = SpellPermission.WhiteWizard,
						Tag = SpellTag.WARP
					});
					helper.SetOOBSpellEffect(OOBSpellEffects.WARP, index);
					SpellIndices.Remove(index);
					return Spell.GetSpellLevel(index);
				default:
					return 0;
			}
		}

		private int CraftPureSoftSpell(SpellSchool school, int lolvl, int hilvl, int tier)
		{
			var index = GetSpellIndex(school, lolvl, hilvl);
			if (index < 0) return 0;

			switch (tier)
			{
				case 1:
					Spells.Add(new Spell
					{
						Index = index,
						Name = "PURE",
						Type = SpellRoutine.CureAilment,
						Targeting = SpellTargeting.OneCharacters,
						Status = SpellStatus.Poison,
						Permissions = SpellPermission.WhiteMage| SpellPermission.WhiteWizard | SpellPermission.RedMage | SpellPermission.RedWizard | SpellPermission.Knight,
						GraphicEffect = SpellGraphic.LargeSparkle,
						Palette = SpellPalette.DarkGreen,
						Tag = SpellTag.PURE
					});
					helper.SetOOBSpellEffect(OOBSpellEffects.PURE, index);
					SpellIndices.Remove(index);
					return Spell.GetSpellLevel(index);
				case 2:
					Spells.Add(new Spell
					{
						Index = index,
						Name = "SOFT",
						Type = SpellRoutine.None,
						Targeting = SpellTargeting.OneCharacters,
						Status = SpellStatus.Stone,
						Permissions = SpellPermission.WhiteMage | SpellPermission.WhiteWizard | SpellPermission.RedMage | SpellPermission.RedWizard | SpellPermission.Knight,
						GraphicEffect = SpellGraphic.LargeSparkle,
						Palette = SpellPalette.Brown,
						Tag = SpellTag.SOFT,
					});
					helper.SetOOBSpellEffect(OOBSpellEffects.SOFT, index);
					SpellIndices.Remove(index);
					return Spell.GetSpellLevel(index);
				default:
					return 0;
			}
		}

		private int CraftSmokeSpell(SpellSchool school, int lolvl, int hilvl, int tier)
		{
			var index = GetSpellIndex(school, lolvl, hilvl);
			if (index < 0) return 0;

			switch (tier)
			{
				case 1:
					Spells.Add(new Spell
					{
						Index = index,
						Name = "SMKB",
						Type = SpellRoutine.CureAilment,
						Targeting = SpellTargeting.Self,
						Status = SpellStatus.None,
						Permissions = SpellPermission.WhiteMage | SpellPermission.WhiteWizard | SpellPermission.RedMage | SpellPermission.RedWizard,
						GraphicEffect = SpellGraphic.None,
						Palette = SpellPalette.Black1,
						Tag = SpellTag.SMOKE
					});
					SpellIndices.Remove(index);
					return Spell.GetSpellLevel(index);
				default:
					return 0;
			}
		}

		private int CraftVoxSpell(SpellSchool school, int lolvl, int hilvl, int tier)
		{
			var index = GetSpellIndex(school, lolvl, hilvl);
			if (index < 0) return 0;

			switch (tier)
			{
				case 1:
					Spells.Add(new Spell
					{
						Index = index,
						Name = "VOX ",
						Type = SpellRoutine.CureAilment,
						Targeting = SpellTargeting.AllCharacters,
						Status = SpellStatus.Mute,
						Permissions = SpellPermission.WhiteMage | SpellPermission.WhiteWizard | SpellPermission.RedMage | SpellPermission.RedWizard,
						GraphicEffect = SpellGraphic.None,
						Palette = SpellPalette.Black1
					});
					SpellIndices.Remove(index);
					return Spell.GetSpellLevel(index);
				default:
					return 0;
			}
		}

		private int CraftFastSpell(SpellSchool school, int lolvl, int hilvl, int tier)
		{
			var index = GetSpellIndex(school, lolvl, hilvl);
			if (index < 0) return 0;

			switch (tier)
			{
				case 1:
					Spells.Add(new Spell
					{
						Index = index,
						Name = "FST1",
						Type = SpellRoutine.Fast,
						Targeting = SpellTargeting.OneCharacters,
						Permissions = SpellPermission.WhiteMage | SpellPermission.WhiteWizard | SpellPermission.RedMage | SpellPermission.RedWizard | SpellPermission.Knight,
						GraphicEffect = SpellGraphic.FourSparkles,
						Palette = SpellPalette.DarkBlue
					});
					SpellIndices.Remove(index);
					return Spell.GetSpellLevel(index);
				case 2:
					Spells.Add(new Spell
					{
						Index = index,
						Name = "FST2",
						Type = SpellRoutine.Fast,
						Targeting = SpellTargeting.AllCharacters,
						Permissions = SpellPermission.WhiteWizard | SpellPermission.RedWizard | SpellPermission.Knight,
						GraphicEffect = SpellGraphic.FourSparkles,
						Palette = SpellPalette.LightBlue
					});
					SpellIndices.Remove(index);
					return Spell.GetSpellLevel(index);
				default:
					return 0;
			}
		}

		private int CraftTemperSpell(SpellSchool school, int lolvl, int hilvl, int tier)
		{
			var index = GetSpellIndex(school, lolvl, hilvl);
			if (index < 0) return 0;

			switch (tier)
			{
				case 1:
					Spells.Add(new Spell
					{
						Index = index,
						Name = "SAB1",
						Type = SpellRoutine.Sabr,
						Effectivity = (byte)rng.Between(16, 24),
						Targeting = SpellTargeting.Self,
						Permissions = SpellPermission.BlackMage | SpellPermission.BlackWizard | SpellPermission.RedMage | SpellPermission.RedWizard | SpellPermission.Ninja,
						GraphicEffect = SpellGraphic.FourSparkles,
						Palette = SpellPalette.Brown,
						Tag = SpellTag.SABR
					});
					SpellIndices.Remove(index);
					return Spell.GetSpellLevel(index);
				case 2:
					Spells.Add(new Spell
					{
						Index = index,
						Name = "SAB2",
						Type = SpellRoutine.Sabr,
						Effectivity = (byte)rng.Between(24, 32),
						Targeting = SpellTargeting.Self,
						Permissions = SpellPermission.BlackMage | SpellPermission.BlackWizard | SpellPermission.RedMage | SpellPermission.RedWizard | SpellPermission.Ninja,
						GraphicEffect = SpellGraphic.FourSparkles,
						Palette = SpellPalette.LightGreen,
						Tag = SpellTag.SABR
					});
					SpellIndices.Remove(index);
					return Spell.GetSpellLevel(index);
				case 3:
					Spells.Add(new Spell
					{
						Index = index,
						Name = "TMP1",
						Type = SpellRoutine.Sabr,
						Effectivity = (byte)rng.Between(8, 16),
						Targeting = SpellTargeting.OneCharacters,
						Permissions = SpellPermission.BlackMage | SpellPermission.BlackWizard | SpellPermission.RedMage | SpellPermission.RedWizard | SpellPermission.Ninja,
						GraphicEffect = SpellGraphic.FourSparkles,
						Palette = SpellPalette.Orange,
						Tag = SpellTag.TMPR
					});
					SpellIndices.Remove(index);
					return Spell.GetSpellLevel(index);
				case 4:
					Spells.Add(new Spell
					{
						Index = index,
						Name = "TMP2",
						Type = SpellRoutine.Sabr,
						Effectivity = (byte)rng.Between(16, 24),
						Targeting = SpellTargeting.OneCharacters,
						Permissions = SpellPermission.BlackMage | SpellPermission.BlackWizard | SpellPermission.RedMage | SpellPermission.RedWizard | SpellPermission.Ninja,
						GraphicEffect = SpellGraphic.FourSparkles,
						Palette = SpellPalette.Red,
						Tag = SpellTag.TMPR
					});
					SpellIndices.Remove(index);
					return Spell.GetSpellLevel(index);
				case 5:
					Spells.Add(new Spell
					{
						Index = index,
						Name = "TMP3",
						Type = SpellRoutine.Sabr,
						Effectivity = (byte)rng.Between(6, 12),
						Targeting = SpellTargeting.AllCharacters,
						Permissions = SpellPermission.BlackMage | SpellPermission.BlackWizard | SpellPermission.RedWizard,
						GraphicEffect = SpellGraphic.FourSparkles,
						Palette = SpellPalette.Violet,
						Tag = SpellTag.TMPR
					});
					SpellIndices.Remove(index);
					return Spell.GetSpellLevel(index);
				case 6:
					Spells.Add(new Spell
					{
						Index = index,
						Name = "TMP4",
						Type = SpellRoutine.Sabr,
						Effectivity = (byte)rng.Between(12, 18),
						Targeting = SpellTargeting.AllCharacters,
						Permissions = SpellPermission.BlackWizard,
						GraphicEffect = SpellGraphic.FourSparkles,
						Palette = SpellPalette.Purple,
						Tag = SpellTag.TMPR
					});
					SpellIndices.Remove(index);
					return Spell.GetSpellLevel(index);
				default:
					return 0;
			}
		}

		private int CraftRuseSpell(SpellSchool school, int lolvl, int hilvl, int tier)
		{
			var index = GetSpellIndex(school, lolvl, hilvl);
			if (index < 0) return 0;

			switch (tier)
			{
				case 1:
					Spells.Add(new Spell
					{
						Index = index,
						Name = "RUS1",
						Type = SpellRoutine.Ruse,
						Effectivity = (byte)rng.Between(30, 60),
						Targeting = SpellTargeting.Self,
						Permissions = SpellPermission.WhiteMage | SpellPermission.WhiteWizard | SpellPermission.RedMage | SpellPermission.RedWizard | SpellPermission.Knight,
						GraphicEffect = SpellGraphic.BarOfLight,
						Palette = SpellPalette.DarkBlue,
						Tag = SpellTag.RUSE
					});
					SpellIndices.Remove(index);
					return Spell.GetSpellLevel(index);
				case 2:
					Spells.Add(new Spell
					{
						Index = index,
						Name = "RUS2",
						Type = SpellRoutine.Ruse,
						Effectivity = (byte)rng.Between(60, 90),
						Targeting = SpellTargeting.Self,
						Permissions = SpellPermission.WhiteMage | SpellPermission.WhiteWizard | SpellPermission.RedWizard,
						GraphicEffect = SpellGraphic.BarOfLight,
						Palette = SpellPalette.LightBlue,
						Tag = SpellTag.RUSE
					});
					SpellIndices.Remove(index);
					return Spell.GetSpellLevel(index);
				case 3:
					Spells.Add(new Spell
					{
						Index = index,
						Name = "CLK1",
						Type = SpellRoutine.Ruse,
						Effectivity = (byte)rng.Between(15, 30),
						Targeting = SpellTargeting.OneCharacters,
						Permissions = SpellPermission.WhiteMage | SpellPermission.WhiteWizard | SpellPermission.RedMage | SpellPermission.RedWizard | SpellPermission.Knight,
						GraphicEffect = SpellGraphic.BarOfLight,
						Palette = SpellPalette.Teal,
						Tag = SpellTag.INVS
					});
					SpellIndices.Remove(index);
					return Spell.GetSpellLevel(index);
				case 4:
					Spells.Add(new Spell
					{
						Index = index,
						Name = "CLK2",
						Type = SpellRoutine.Ruse,
						Effectivity = (byte)rng.Between(30, 60),
						Targeting = SpellTargeting.OneCharacters,
						Permissions = SpellPermission.WhiteMage | SpellPermission.WhiteWizard | SpellPermission.RedWizard,
						GraphicEffect = SpellGraphic.BarOfLight,
						Palette = SpellPalette.MintGreen,
						Tag = SpellTag.INVS
					});
					SpellIndices.Remove(index);
					return Spell.GetSpellLevel(index);
				case 5:
					Spells.Add(new Spell
					{
						Index = index,
						Name = "INV1",
						Type = SpellRoutine.Ruse,
						Effectivity = (byte)rng.Between(15, 30),
						Targeting = SpellTargeting.AllCharacters,
						Permissions = SpellPermission.WhiteMage | SpellPermission.WhiteWizard | SpellPermission.RedMage | SpellPermission.RedWizard | SpellPermission.Knight,
						GraphicEffect = SpellGraphic.BarOfLight,
						Palette = SpellPalette.Teal,
						Tag = SpellTag.INVS
					});
					SpellIndices.Remove(index);
					return Spell.GetSpellLevel(index);
				case 6:
					Spells.Add(new Spell
					{
						Index = index,
						Name = "INV2",
						Type = SpellRoutine.Ruse,
						Effectivity = (byte)rng.Between(30, 60),
						Targeting = SpellTargeting.AllCharacters,
						Permissions = SpellPermission.WhiteMage | SpellPermission.WhiteWizard | SpellPermission.RedWizard,
						GraphicEffect = SpellGraphic.BarOfLight,
						Palette = SpellPalette.MintGreen,
						Tag = SpellTag.INVS
					});
					SpellIndices.Remove(index);
					return Spell.GetSpellLevel(index);
				default:
					return 0;
			}
		}

		private int CraftLockSpell(SpellSchool school, int lolvl, int hilvl, int tier)
		{
			var index = GetSpellIndex(school, lolvl, hilvl);
			if (index < 0) return 0;

			switch (tier)
			{
				case 1:
					Spells.Add(new Spell
					{
						Index = index,
						Name = "LOK1",
						Type = SpellRoutine.Lock,
						Effectivity = (byte)rng.Between(15, 25),
						Targeting = SpellTargeting.OneEnemy,
						Permissions = SpellPermission.BlackMage | SpellPermission.BlackWizard | SpellPermission.RedMage | SpellPermission.RedWizard | SpellPermission.Ninja,
						GraphicEffect = SpellGraphic.GlowingBall,
						Palette = SpellPalette.Brown,
						Tag = SpellTag.LOCK
					});
					SpellIndices.Remove(index);
					return Spell.GetSpellLevel(index);
				case 2:
					Spells.Add(new Spell
					{
						Index = index,
						Name = "LOK2",
						Type = SpellRoutine.Lock,
						Effectivity = (byte)rng.Between(25, 35),
						Targeting = SpellTargeting.OneEnemy,
						Permissions = SpellPermission.BlackMage | SpellPermission.BlackWizard | SpellPermission.RedMage | SpellPermission.RedWizard | SpellPermission.Ninja,
						GraphicEffect = SpellGraphic.GlowingBall,
						Palette = SpellPalette.Orange,
						Tag = SpellTag.LOCK
					});
					SpellIndices.Remove(index);
					return Spell.GetSpellLevel(index);
				case 3:
					Spells.Add(new Spell
					{
						Index = index,
						Name = "LOK3",
						Type = SpellRoutine.Lock,
						Effectivity = (byte)rng.Between(8, 16),
						Targeting = SpellTargeting.AllEnemies,
						Permissions = SpellPermission.BlackMage | SpellPermission.BlackWizard | SpellPermission.RedMage | SpellPermission.RedWizard | SpellPermission.Ninja,
						GraphicEffect = SpellGraphic.GlowingBall,
						Palette = SpellPalette.DarkGreen,
						Tag = SpellTag.LOCK
					});
					SpellIndices.Remove(index);
					return Spell.GetSpellLevel(index);
				case 4:
					Spells.Add(new Spell
					{
						Index = index,
						Name = "LOK4",
						Type = SpellRoutine.Lock,
						Effectivity = (byte)rng.Between(16, 24),
						Targeting = SpellTargeting.AllEnemies,
						Permissions = SpellPermission.BlackMage | SpellPermission.BlackWizard | SpellPermission.RedMage | SpellPermission.RedWizard | SpellPermission.Ninja,
						GraphicEffect = SpellGraphic.GlowingBall,
						Palette = SpellPalette.MintGreen,
						Tag = SpellTag.LOCK
					});
					SpellIndices.Remove(index);
					return Spell.GetSpellLevel(index);
				default:
					return 0;
			}
		}

		private int CraftFogSpell(SpellSchool school, int lolvl, int hilvl, int tier)
		{
			var index = GetSpellIndex(school, lolvl, hilvl);
			if (index < 0) return 0;

			switch (tier)
			{
				case 1:
					Spells.Add(new Spell
					{
						Index = index,
						Name = "GRD1",
						Type = SpellRoutine.ArmorUp,
						Effectivity = (byte)rng.Between(16, 24),
						Targeting = SpellTargeting.OneCharacters,
						Permissions = SpellPermission.BlackMage | SpellPermission.BlackWizard | SpellPermission.RedMage | SpellPermission.RedWizard | SpellPermission.Ninja,
						GraphicEffect = SpellGraphic.BarOfLight,
						Palette = SpellPalette.Orange,
						Tag = SpellTag.FOG
					});
					SpellIndices.Remove(index);
					return Spell.GetSpellLevel(index);
				case 2:
					Spells.Add(new Spell
					{
						Index = index,
						Name = "GRD2",
						Type = SpellRoutine.ArmorUp,
						Effectivity = (byte)rng.Between(24, 32),
						Targeting = SpellTargeting.OneCharacters,
						Permissions = SpellPermission.BlackMage | SpellPermission.BlackWizard | SpellPermission.RedMage | SpellPermission.RedWizard | SpellPermission.Ninja,
						GraphicEffect = SpellGraphic.BarOfLight,
						Palette = SpellPalette.Red,
						Tag = SpellTag.FOG
					});
					SpellIndices.Remove(index);
					return Spell.GetSpellLevel(index);
				case 3:
					Spells.Add(new Spell
					{
						Index = index,
						Name = "FOG1",
						Type = SpellRoutine.ArmorUp,
						Effectivity = (byte)rng.Between(8, 16),
						Targeting = SpellTargeting.AllCharacters,
						Permissions = SpellPermission.BlackMage | SpellPermission.BlackWizard | SpellPermission.RedMage | SpellPermission.RedWizard,
						GraphicEffect = SpellGraphic.BarOfLight,
						Palette = SpellPalette.Violet,
						Tag = SpellTag.FOG
					});
					SpellIndices.Remove(index);
					return Spell.GetSpellLevel(index);
				case 4:
					Spells.Add(new Spell
					{
						Index = index,
						Name = "FOG2",
						Type = SpellRoutine.ArmorUp,
						Effectivity = (byte)rng.Between(16, 24),
						Targeting = SpellTargeting.AllCharacters,
						Permissions = SpellPermission.BlackMage | SpellPermission.BlackWizard | SpellPermission.RedWizard,
						GraphicEffect = SpellGraphic.BarOfLight,
						Palette = SpellPalette.Magenta,
						Tag = SpellTag.FOG
					});
					SpellIndices.Remove(index);
					return Spell.GetSpellLevel(index);
				default:
					return 0;
			}
		}

		private int CraftWallSpell(SpellSchool school, int lolvl, int hilvl, int tier)
		{
			var index = GetSpellIndex(school, lolvl, hilvl);
			if (index < 0) return 0;

			switch (tier)
			{
				case 1:
					Spells.Add(new Spell
					{
						Index = index,
						Name = "AEL1",
						Type = SpellRoutine.Wall,
						Protection = SpellElement.Earth | SpellElement.Fire | SpellElement.Ice | SpellElement.Lightning,
						Targeting = SpellTargeting.OneCharacters,
						Permissions = SpellPermission.WhiteMage | SpellPermission.WhiteWizard | SpellPermission.RedMage | SpellPermission.RedWizard | SpellPermission.Knight,
						GraphicEffect = SpellGraphic.BarOfLight,
						Palette = SpellPalette.DarkGreen
					});
					SpellIndices.Remove(index);
					return Spell.GetSpellLevel(index);
				case 2:
					Spells.Add(new Spell
					{
						Index = index,
						Name = "AEL2",
						Type = SpellRoutine.Wall,
						Protection = SpellElement.Earth | SpellElement.Fire | SpellElement.Ice | SpellElement.Lightning,
						Targeting = SpellTargeting.AllCharacters,
						Permissions = SpellPermission.WhiteMage | SpellPermission.WhiteWizard | SpellPermission.RedWizard,
						GraphicEffect = SpellGraphic.BarOfLight,
						Palette = SpellPalette.LightGreen
					});
					SpellIndices.Remove(index);
					return Spell.GetSpellLevel(index);
				case 3:
					Spells.Add(new Spell
					{
						Index = index,
						Name = "AST1",
						Type = SpellRoutine.Wall,
						Protection = SpellElement.Status | SpellElement.Poison | SpellElement.Time | SpellElement.Death,
						Targeting = SpellTargeting.OneCharacters,
						Permissions = SpellPermission.WhiteMage | SpellPermission.WhiteWizard | SpellPermission.RedMage | SpellPermission.RedWizard | SpellPermission.Knight,
						GraphicEffect = SpellGraphic.BarOfLight,
						Palette = SpellPalette.Magenta
					});
					SpellIndices.Remove(index);
					return Spell.GetSpellLevel(index);
				case 4:
					Spells.Add(new Spell
					{
						Index = index,
						Name = "AST2",
						Type = SpellRoutine.Wall,
						Protection = SpellElement.Status | SpellElement.Poison | SpellElement.Time | SpellElement.Death,
						Targeting = SpellTargeting.AllCharacters,
						Permissions = SpellPermission.WhiteMage | SpellPermission.WhiteWizard | SpellPermission.RedWizard,
						GraphicEffect = SpellGraphic.BarOfLight,
						Palette = SpellPalette.Purple
					});
					SpellIndices.Remove(index);
					return Spell.GetSpellLevel(index);
				case 5:
					Spells.Add(new Spell
					{
						Index = index,
						Name = "WAL1",
						Type = SpellRoutine.Wall,
						Protection = SpellElement.Status | SpellElement.Poison | SpellElement.Time | SpellElement.Death | SpellElement.Earth | SpellElement.Fire | SpellElement.Ice | SpellElement.Lightning,
						Targeting = SpellTargeting.OneCharacters,
						Permissions = SpellPermission.WhiteMage | SpellPermission.WhiteWizard | SpellPermission.RedMage | SpellPermission.RedWizard | SpellPermission.Knight,
						GraphicEffect = SpellGraphic.BarOfLight,
						Palette = SpellPalette.White
					});
					SpellIndices.Remove(index);
					return Spell.GetSpellLevel(index);
				case 6:
					Spells.Add(new Spell
					{
						Index = index,
						Name = "WAL2",
						Type = SpellRoutine.Wall,
						Protection = SpellElement.Status | SpellElement.Poison | SpellElement.Time | SpellElement.Death | SpellElement.Earth | SpellElement.Fire | SpellElement.Ice | SpellElement.Lightning,
						Targeting = SpellTargeting.AllCharacters,
						Permissions = SpellPermission.WhiteMage | SpellPermission.WhiteWizard | SpellPermission.RedWizard,
						GraphicEffect = SpellGraphic.BarOfLight,
						Palette = SpellPalette.White
					});
					SpellIndices.Remove(index);
					return Spell.GetSpellLevel(index);
				default:
					return 0;
			}
		}

		private int CraftSlowSpell(SpellSchool school, int lolvl, int hilvl, int tier)
		{
			var index = GetSpellIndex(school, lolvl, hilvl);
			if (index < 0) return 0;

			switch (tier)
			{
				case 1:
					Spells.Add(new Spell
					{
						Index = index,
						Name = "SLO1",
						Type = SpellRoutine.Slow,
						Targeting = SpellTargeting.OneEnemy,
						Accuracy = 0x40,
						Permissions = SpellPermission.BlackMage | SpellPermission.BlackWizard | SpellPermission.RedMage | SpellPermission.RedWizard | SpellPermission.Ninja,
						GraphicEffect = SpellGraphic.SparklingHand,
						Palette = SpellPalette.DarkGreen
					});
					SpellIndices.Remove(index);
					return Spell.GetSpellLevel(index);
				case 2:
					Spells.Add(new Spell
					{
						Index = index,
						Name = "SLO2",
						Type = SpellRoutine.Slow,
						Targeting = SpellTargeting.AllEnemies,
						Accuracy = 0x40,
						Permissions = SpellPermission.BlackMage | SpellPermission.BlackWizard | SpellPermission.RedMage | SpellPermission.RedWizard | SpellPermission.Ninja,
						GraphicEffect = SpellGraphic.SparklingHand,
						Palette = SpellPalette.Teal
					});
					SpellIndices.Remove(index);
					return Spell.GetSpellLevel(index);
				default:
					return 0;
			}
		}


		private int CraftFearSpell(SpellSchool school, int lolvl, int hilvl, int tier)
		{
			var index = GetSpellIndex(school, lolvl, hilvl);
			if (index < 0) return 0;

			switch (tier)
			{
				case 1:
					Spells.Add(new Spell
					{
						Index = index,
						Name = "FEA1",
						Type = SpellRoutine.Fear,
						Targeting = SpellTargeting.AllEnemies,
						Element = SpellElement.Status,
						Effectivity = 40,
						Accuracy = 0x20,
						Permissions = SpellPermission.BlackMage | SpellPermission.BlackWizard | SpellPermission.RedMage | SpellPermission.RedWizard | SpellPermission.Ninja,
						GraphicEffect = SpellGraphic.SparklingHand,
						Palette = SpellPalette.DarkGreen
					});
					SpellIndices.Remove(index);
					return Spell.GetSpellLevel(index);
				case 2:
					Spells.Add(new Spell
					{
						Index = index,
						Name = "FEA2",
						Type = SpellRoutine.Fear,
						Targeting = SpellTargeting.AllEnemies,
						Element = SpellElement.Status,
						Effectivity = 60,
						Accuracy = 0x40,
						Permissions = SpellPermission.BlackMage | SpellPermission.BlackWizard | SpellPermission.RedMage | SpellPermission.RedWizard | SpellPermission.Ninja,
						GraphicEffect = SpellGraphic.SparklingHand,
						Palette = SpellPalette.Teal
					});
					SpellIndices.Remove(index);
					return Spell.GetSpellLevel(index);
				default:
					return 0;
			}
		}

		private int CraftXferSpell(SpellSchool school, int lolvl, int hilvl, int tier)
		{
			var index = GetSpellIndex(school, lolvl, hilvl);
			if (index < 0) return 0;

			switch (tier)
			{
				case 1:
					Spells.Add(new Spell
					{
						Index = index,
						Name = "XFR1",
						Type = SpellRoutine.Xfer,
						Targeting = SpellTargeting.OneEnemy,
						Accuracy = 0x6B,
						Permissions = SpellPermission.BlackMage | SpellPermission.BlackWizard | SpellPermission.RedWizard,
						GraphicEffect = SpellGraphic.FourSparkles,
						Palette = SpellPalette.Gray
					});
					SpellIndices.Remove(index);
					return Spell.GetSpellLevel(index);
				case 2:
					Spells.Add(new Spell
					{
						Index = index,
						Name = "XFR2",
						Type = SpellRoutine.Xfer,
						Targeting = SpellTargeting.AllEnemies,
						Accuracy = 0x6B,
						Permissions = SpellPermission.BlackWizard,
						GraphicEffect = SpellGraphic.FourSparkles,
						Palette = SpellPalette.White
					});
					SpellIndices.Remove(index);
					return Spell.GetSpellLevel(index);
				default:
					return 0;
			}
		}


		private int CraftFlareSpell(SpellSchool school, int lolvl, int hilvl, int tier)
		{
			var index = GetSpellIndex(school, lolvl, hilvl);
			if (index < 0) return 0;

			switch (tier)
			{
				case 1:
					Spells.Add(new Spell
					{
						Index = index,
						Name = "FLR1",
						Type = SpellRoutine.Damage,
						Targeting = SpellTargeting.OneEnemy,
						Accuracy = 0x6B,
						Element = SpellElement.Earth | SpellElement.Fire | SpellElement.Ice |SpellElement.Lightning,
						Effectivity = (byte)rng.Between(30, 60),
						Permissions = SpellPermission.WhiteMage | SpellPermission.WhiteWizard | SpellPermission.RedWizard,
						GraphicEffect = SpellGraphic.EnergyFlare,
						Palette = SpellPalette.Gray
					});
					SpellIndices.Remove(index);
					return Spell.GetSpellLevel(index);
				case 2:
					Spells.Add(new Spell
					{
						Index = index,
						Name = "FLR2",
						Type = SpellRoutine.Damage,
						Targeting = SpellTargeting.OneEnemy,
						Accuracy = 0x6B,
						Element = SpellElement.Earth | SpellElement.Fire | SpellElement.Ice | SpellElement.Lightning,
						Effectivity = (byte)rng.Between(60, 100),
						Permissions = SpellPermission.WhiteMage | SpellPermission.WhiteWizard | SpellPermission.RedWizard,
						GraphicEffect = SpellGraphic.EnergyFlare,
						Palette = SpellPalette.Gray
					});
					SpellIndices.Remove(index);
					return Spell.GetSpellLevel(index);
				case 3:
					Spells.Add(new Spell
					{
						Index = index,
						Name = "FLR3",
						Type = SpellRoutine.Damage,
						Targeting = SpellTargeting.AllEnemies,
						Accuracy = 0x40,
						Element = SpellElement.Earth | SpellElement.Fire | SpellElement.Ice | SpellElement.Lightning,
						Effectivity = (byte)rng.Between(20, 40),
						Permissions = SpellPermission.WhiteMage | SpellPermission.WhiteWizard,
						GraphicEffect = SpellGraphic.EnergyFlare,
						Palette = SpellPalette.White
					});
					SpellIndices.Remove(index);
					return Spell.GetSpellLevel(index);
				case 4:
					Spells.Add(new Spell
					{
						Index = index,
						Name = "FLR4",
						Type = SpellRoutine.Damage,
						Targeting = SpellTargeting.AllEnemies,
						Accuracy = 0x40,
						Element = SpellElement.Earth | SpellElement.Fire | SpellElement.Ice | SpellElement.Lightning,
						Effectivity = (byte)rng.Between(40, 80),
						Permissions = SpellPermission.WhiteMage | SpellPermission.WhiteWizard,
						GraphicEffect = SpellGraphic.EnergyFlare,
						Palette = SpellPalette.White
					});
					SpellIndices.Remove(index);
					return Spell.GetSpellLevel(index);
				default:
					return 0;
			}
		}

		private int CraftFadeSpell(SpellSchool school, int lolvl, int hilvl, int tier)
		{
			var index = GetSpellIndex(school, lolvl, hilvl);
			if (index < 0) return 0;

			switch (tier)
			{
				case 1:
					Spells.Add(new Spell
					{
						Index = index,
						Name = "FAD1",
						Type = SpellRoutine.Damage,
						Targeting = SpellTargeting.OneEnemy,
						Accuracy = 0x6B,
						Effectivity = (byte)rng.Between(120, 160),
						Permissions = SpellPermission.WhiteMage | SpellPermission.WhiteWizard,
						GraphicEffect = SpellGraphic.EnergyBeam,
						Palette = SpellPalette.Teal
					});
					SpellIndices.Remove(index);
					return Spell.GetSpellLevel(index);
				case 2:
					Spells.Add(new Spell
					{
						Index = index,
						Name = "FAD2",
						Type = SpellRoutine.Damage,
						Targeting = SpellTargeting.AllEnemies,
						Accuracy = 0x6B,
						Effectivity = (byte)rng.Between(60, 120),
						Permissions = SpellPermission.WhiteWizard,
						GraphicEffect = SpellGraphic.EnergyBeam,
						Palette = SpellPalette.Teal
					});
					SpellIndices.Remove(index);
					return Spell.GetSpellLevel(index);
				default:
					return 0;
			}
		}

		private int CraftHarmSpell(SpellSchool school, int lolvl, int hilvl, int tier)
		{
			var index = GetSpellIndex(school, lolvl, hilvl);
			if (index < 0) return 0;

			switch (tier)
			{
				case 1:
					Spells.Add(new Spell
					{
						Index = index,
						Name = "HRM1",
						Type = SpellRoutine.DamageUndead,
						Targeting = SpellTargeting.OneEnemy,
						Accuracy = 0x40,
						Effectivity = (byte)rng.Between(40, 80),
						Permissions = SpellPermission.WhiteMage | SpellPermission.WhiteWizard | SpellPermission.RedMage | SpellPermission.RedWizard,
						GraphicEffect = SpellGraphic.EnergyFlare,
						Palette = SpellPalette.Magenta
					});
					SpellIndices.Remove(index);
					return Spell.GetSpellLevel(index);
				case 2:
					Spells.Add(new Spell
					{
						Index = index,
						Name = "HRM2",
						Type = SpellRoutine.DamageUndead,
						Targeting = SpellTargeting.OneEnemy,
						Accuracy = 0x40,
						Effectivity = (byte)rng.Between(80, 120),
						Permissions = SpellPermission.WhiteMage | SpellPermission.WhiteWizard | SpellPermission.RedMage | SpellPermission.RedWizard,
						GraphicEffect = SpellGraphic.EnergyFlare,
						Palette = SpellPalette.Red
					});
					SpellIndices.Remove(index);
					return Spell.GetSpellLevel(index);
				case 3:
					Spells.Add(new Spell
					{
						Index = index,
						Name = "HRM3",
						Type = SpellRoutine.DamageUndead,
						Targeting = SpellTargeting.AllEnemies,
						Accuracy = 0x40,
						Effectivity = (byte)rng.Between(40, 60),
						Permissions = SpellPermission.WhiteMage | SpellPermission.WhiteWizard | SpellPermission.RedWizard,
						GraphicEffect = SpellGraphic.EnergyFlare,
						Palette = SpellPalette.Orange
					});
					SpellIndices.Remove(index);
					return Spell.GetSpellLevel(index);
				case 4:
					Spells.Add(new Spell
					{
						Index = index,
						Name = "HRM4",
						Type = SpellRoutine.DamageUndead,
						Targeting = SpellTargeting.AllEnemies,
						Accuracy = 0x40,
						Effectivity = (byte)rng.Between(60, 100),
						Permissions = SpellPermission.WhiteMage | SpellPermission.WhiteWizard | SpellPermission.RedWizard,
						GraphicEffect = SpellGraphic.EnergyFlare,
						Palette = SpellPalette.Brown
					});
					SpellIndices.Remove(index);
					return Spell.GetSpellLevel(index);
				default:
					return 0;
			}
		}

		private int CraftWhiteStatusSpell(SpellSchool school, int lolvl, int hilvl, int tier)
		{
			var index = GetSpellIndex(school, lolvl, hilvl);
			if (index < 0) return 0;

			switch (tier)
			{
				case 1:
					Spells.Add(new Spell
					{
						Index = index,
						Name = "SLP1",
						Type = SpellRoutine.InflictStatus,
						Targeting = SpellTargeting.OneEnemy,
						Accuracy = 0x40,
						Status = SpellStatus.Sleep,
						Permissions = SpellPermission.WhiteMage | SpellPermission.WhiteWizard | SpellPermission.RedMage | SpellPermission.RedWizard | SpellPermission.Knight,
						GraphicEffect = SpellGraphic.SparklingHand,
						Palette = SpellPalette.DarkGreen
					});
					SpellIndices.Remove(index);
					return Spell.GetSpellLevel(index);
				case 2:
					Spells.Add(new Spell
					{
						Index = index,
						Name = "SLP2",
						Type = SpellRoutine.InflictStatus,
						Targeting = SpellTargeting.AllEnemies,
						Accuracy = 0x30,
						Status = SpellStatus.Sleep,
						Permissions = SpellPermission.WhiteMage | SpellPermission.WhiteWizard | SpellPermission.RedMage | SpellPermission.RedWizard | SpellPermission.Knight,
						GraphicEffect = SpellGraphic.SparklingHand,
						Palette = SpellPalette.DarkGreen
					});
					SpellIndices.Remove(index);
					return Spell.GetSpellLevel(index);
				case 3:
					Spells.Add(new Spell
					{
						Index = index,
						Name = "PSL1",
						Type = SpellRoutine.PowerWord,
						Targeting = SpellTargeting.OneEnemy,
						Element = SpellElement.Status,
						Status = SpellStatus.Sleep,
						Permissions = SpellPermission.WhiteMage | SpellPermission.WhiteWizard | SpellPermission.RedMage | SpellPermission.RedWizard | SpellPermission.Knight,
						GraphicEffect = SpellGraphic.SparklingHand,
						Palette = SpellPalette.LightGreen
					});
					SpellIndices.Remove(index);
					return Spell.GetSpellLevel(index);
				case 4:
					Spells.Add(new Spell
					{
						Index = index,
						Name = "PSL2",
						Type = SpellRoutine.PowerWord,
						Targeting = SpellTargeting.AllEnemies,
						Element = SpellElement.Status,
						Status = SpellStatus.Sleep,
						Permissions = SpellPermission.WhiteMage | SpellPermission.WhiteWizard | SpellPermission.RedMage | SpellPermission.RedWizard | SpellPermission.Knight,
						GraphicEffect = SpellGraphic.SparklingHand,
						Palette = SpellPalette.LightGreen
					});
					SpellIndices.Remove(index);
					return Spell.GetSpellLevel(index);
				case 5:
					Spells.Add(new Spell
					{
						Index = index,
						Name = "CON1",
						Type = SpellRoutine.InflictStatus,
						Targeting = SpellTargeting.OneEnemy,
						Element = SpellElement.Status,
						Accuracy = 0x40,
						Status = SpellStatus.Confuse,
						Permissions = SpellPermission.WhiteMage | SpellPermission.WhiteWizard | SpellPermission.RedMage | SpellPermission.RedWizard | SpellPermission.Knight,
						GraphicEffect = SpellGraphic.SparklingHand,
						Palette = SpellPalette.DarkBlue
					});
					SpellIndices.Remove(index);
					return Spell.GetSpellLevel(index);
				case 6:
					Spells.Add(new Spell
					{
						Index = index,
						Name = "CON2",
						Type = SpellRoutine.InflictStatus,
						Targeting = SpellTargeting.AllEnemies,
						Element = SpellElement.Status,
						Accuracy = 0x30,
						Status = SpellStatus.Confuse,
						Permissions = SpellPermission.WhiteMage | SpellPermission.WhiteWizard | SpellPermission.RedMage | SpellPermission.RedWizard | SpellPermission.Knight,
						GraphicEffect = SpellGraphic.SparklingHand,
						Palette = SpellPalette.DarkBlue
					});
					SpellIndices.Remove(index);
					return Spell.GetSpellLevel(index);
				case 7:
					Spells.Add(new Spell
					{
						Index = index,
						Name = "PCN1",
						Type = SpellRoutine.PowerWord,
						Targeting = SpellTargeting.OneEnemy,
						Element = SpellElement.Status,
						Status = SpellStatus.Confuse,
						Permissions = SpellPermission.WhiteMage | SpellPermission.WhiteWizard | SpellPermission.RedMage | SpellPermission.RedWizard | SpellPermission.Knight,
						GraphicEffect = SpellGraphic.SparklingHand,
						Palette = SpellPalette.LightBlue
					});
					SpellIndices.Remove(index);
					return Spell.GetSpellLevel(index);
				case 8:
					Spells.Add(new Spell
					{
						Index = index,
						Name = "PCN2",
						Type = SpellRoutine.PowerWord,
						Targeting = SpellTargeting.AllEnemies,
						Element = SpellElement.Status,
						Status = SpellStatus.Confuse,
						Permissions = SpellPermission.WhiteMage | SpellPermission.WhiteWizard | SpellPermission.RedMage | SpellPermission.RedWizard | SpellPermission.Knight,
						GraphicEffect = SpellGraphic.SparklingHand,
						Palette = SpellPalette.LightBlue
					});
					SpellIndices.Remove(index);
					return Spell.GetSpellLevel(index);
				default:
					return 0;
			}
		}

		private int CraftLampSpell(SpellSchool school, int lolvl, int hilvl, int tier)
		{
			var index = GetSpellIndex(school, lolvl, hilvl);
			if (index < 0) return 0;

			Spells.Add(new Spell
			{
				Index = index,
				Name = "LMP" + tier.ToString(),
				Type = SpellRoutine.CureAilment,
				Targeting = SpellTargeting.AllCharacters,
				Status = SpellStatus.Dark,
				Permissions = SpellPermission.WhiteMage | SpellPermission.WhiteWizard | SpellPermission.RedMage | SpellPermission.RedWizard | SpellPermission.Knight,
				GraphicEffect = SpellGraphic.LargeSparkle,
				Palette = SpellPalette.Orange
			});
			SpellIndices.Remove(index);
			return Spell.GetSpellLevel(index);
		}

		private int CraftNukeSpell(SpellSchool school, int lolvl, int hilvl, int tier)
		{
			var index = GetSpellIndex(school, lolvl, hilvl);
			if (index < 0) return 0;

			switch (tier)
			{
				case 1:
					Spells.Add(new Spell
					{
						Index = index,
						Name = "NUKE",
						Type = SpellRoutine.Damage,
						Targeting = SpellTargeting.OneEnemy,
						Accuracy = 0x6B,
						Effectivity = (byte)rng.Between(120, 160),
						Permissions = SpellPermission.BlackMage | SpellPermission.BlackWizard,
						GraphicEffect = SpellGraphic.EnergyFlare,
						Palette = SpellPalette.Magenta
					});
					SpellIndices.Remove(index);
					return Spell.GetSpellLevel(index);
				case 2:
					Spells.Add(new Spell
					{
						Index = index,
						Name = "NUKE",
						Type = SpellRoutine.Damage,
						Targeting = SpellTargeting.AllEnemies,
						Accuracy = 0x6B,
						Effectivity = (byte)rng.Between(80, 120),
						Permissions = SpellPermission.BlackWizard,
						GraphicEffect = SpellGraphic.EnergyFlare,
						Palette = SpellPalette.Purple
					});
					SpellIndices.Remove(index);
					return Spell.GetSpellLevel(index);
				default:
					return 0;
			}
		}

		private int CraftBlackStatusSpell(SpellSchool school, int lolvl, int hilvl, int tier)
		{
			var index = GetSpellIndex(school, lolvl, hilvl);
			if (index < 0) return 0;

			switch (tier)
			{
				case 1:
					Spells.Add(new Spell
					{
						Index = index,
						Name = "MUT1",
						Type = SpellRoutine.InflictStatus,
						Status = SpellStatus.Mute,
						Targeting = SpellTargeting.OneEnemy,
						Element = SpellElement.Status,
						Accuracy = 0x40,
						Permissions = SpellPermission.BlackMage | SpellPermission.BlackWizard | SpellPermission.RedMage | SpellPermission.RedWizard | SpellPermission.Ninja,
						GraphicEffect = SpellGraphic.SparklingHand,
						Palette = SpellPalette.Violet
					});
					SpellIndices.Remove(index);
					return Spell.GetSpellLevel(index);
				case 2:
					Spells.Add(new Spell
					{
						Index = index,
						Name = "MUT2",
						Type = SpellRoutine.InflictStatus,
						Status = SpellStatus.Mute,
						Targeting = SpellTargeting.AllEnemies,
						Element = SpellElement.Status,
						Accuracy = 0x30,
						Permissions = SpellPermission.BlackMage | SpellPermission.BlackWizard | SpellPermission.RedMage | SpellPermission.RedWizard | SpellPermission.Ninja,
						GraphicEffect = SpellGraphic.SparklingHand,
						Palette = SpellPalette.Violet
					});
					SpellIndices.Remove(index);
					return Spell.GetSpellLevel(index);
				case 3:
					Spells.Add(new Spell
					{
						Index = index,
						Name = "PMT1",
						Type = SpellRoutine.PowerWord,
						Status = SpellStatus.Mute,
						Targeting = SpellTargeting.OneEnemy,
						Element = SpellElement.Status,
						Permissions = SpellPermission.BlackMage | SpellPermission.BlackWizard | SpellPermission.RedMage | SpellPermission.RedWizard | SpellPermission.Ninja,
						GraphicEffect = SpellGraphic.SparklingHand,
						Palette = SpellPalette.Violet
					});
					SpellIndices.Remove(index);
					return Spell.GetSpellLevel(index);
				case 4:
					Spells.Add(new Spell
					{
						Index = index,
						Name = "PMT2",
						Type = SpellRoutine.PowerWord,
						Status = SpellStatus.Mute,
						Targeting = SpellTargeting.AllEnemies,
						Element = SpellElement.Status,
						Permissions = SpellPermission.BlackMage | SpellPermission.BlackWizard | SpellPermission.RedMage | SpellPermission.RedWizard | SpellPermission.Ninja,
						GraphicEffect = SpellGraphic.SparklingHand,
						Palette = SpellPalette.Violet
					});
					SpellIndices.Remove(index);
					return Spell.GetSpellLevel(index);
				case 5:
					Spells.Add(new Spell
					{
						Index = index,
						Name = "HLD1",
						Type = SpellRoutine.InflictStatus,
						Status = SpellStatus.Stun,
						Targeting = SpellTargeting.OneEnemy,
						Element = SpellElement.Status,
						Accuracy = 0x40,
						Permissions = SpellPermission.BlackMage | SpellPermission.BlackWizard | SpellPermission.RedMage | SpellPermission.RedWizard | SpellPermission.Ninja,
						GraphicEffect = SpellGraphic.SparklingHand,
						Palette = SpellPalette.Orange
					});
					SpellIndices.Remove(index);
					return Spell.GetSpellLevel(index);
				case 6:
					Spells.Add(new Spell
					{
						Index = index,
						Name = "HLD2",
						Type = SpellRoutine.InflictStatus,
						Status = SpellStatus.Stun,
						Targeting = SpellTargeting.AllEnemies,
						Element = SpellElement.Status,
						Accuracy = 0x30,
						Permissions = SpellPermission.BlackMage | SpellPermission.BlackWizard | SpellPermission.RedMage | SpellPermission.RedWizard | SpellPermission.Ninja,
						GraphicEffect = SpellGraphic.SparklingHand,
						Palette = SpellPalette.Orange
					});
					SpellIndices.Remove(index);
					return Spell.GetSpellLevel(index);
				case 7:
					Spells.Add(new Spell
					{
						Index = index,
						Name = "PHD1",
						Type = SpellRoutine.PowerWord,
						Status = SpellStatus.Stun,
						Targeting = SpellTargeting.OneEnemy,
						Element = SpellElement.Status,
						Permissions = SpellPermission.BlackMage | SpellPermission.BlackWizard | SpellPermission.RedMage | SpellPermission.RedWizard | SpellPermission.Ninja,
						GraphicEffect = SpellGraphic.SparklingHand,
						Palette = SpellPalette.Orange
					});
					SpellIndices.Remove(index);
					return Spell.GetSpellLevel(index);
				case 8:
					Spells.Add(new Spell
					{
						Index = index,
						Name = "PHD2",
						Type = SpellRoutine.PowerWord,
						Status = SpellStatus.Stun,
						Targeting = SpellTargeting.AllEnemies,
						Element = SpellElement.Status,
						Permissions = SpellPermission.BlackMage | SpellPermission.BlackWizard | SpellPermission.RedMage | SpellPermission.RedWizard | SpellPermission.Ninja,
						GraphicEffect = SpellGraphic.SparklingHand,
						Palette = SpellPalette.Orange
					});
					SpellIndices.Remove(index);
					return Spell.GetSpellLevel(index);
				case 9:
					Spells.Add(new Spell
					{
						Index = index,
						Name = "STP1",
						Type = SpellRoutine.InflictStatus,
						Status = SpellStatus.Stun,
						Targeting = SpellTargeting.OneEnemy,
						Element = SpellElement.Time,
						Accuracy = 0x30,
						Permissions = SpellPermission.BlackMage | SpellPermission.BlackWizard | SpellPermission.RedMage | SpellPermission.RedWizard | SpellPermission.Ninja,
						GraphicEffect = SpellGraphic.SparklingHand,
						Palette = SpellPalette.White
					});
					SpellIndices.Remove(index);
					return Spell.GetSpellLevel(index);
				case 10:
					Spells.Add(new Spell
					{
						Index = index,
						Name = "STP2",
						Type = SpellRoutine.InflictStatus,
						Status = SpellStatus.Stun,
						Targeting = SpellTargeting.AllEnemies,
						Element = SpellElement.Time,
						Accuracy = 0x20,
						Permissions = SpellPermission.BlackMage | SpellPermission.BlackWizard | SpellPermission.RedMage | SpellPermission.RedWizard | SpellPermission.Ninja,
						GraphicEffect = SpellGraphic.SparklingHand,
						Palette = SpellPalette.White
					});
					SpellIndices.Remove(index);
					return Spell.GetSpellLevel(index);
				case 11:
					Spells.Add(new Spell
					{
						Index = index,
						Name = "PST1",
						Type = SpellRoutine.PowerWord,
						Status = SpellStatus.Stun,
						Targeting = SpellTargeting.OneEnemy,
						Element = SpellElement.Time,
						Permissions = SpellPermission.BlackMage | SpellPermission.BlackWizard | SpellPermission.RedMage | SpellPermission.RedWizard | SpellPermission.Ninja,
						GraphicEffect = SpellGraphic.SparklingHand,
						Palette = SpellPalette.White
					});
					SpellIndices.Remove(index);
					return Spell.GetSpellLevel(index);
				case 12:
					Spells.Add(new Spell
					{
						Index = index,
						Name = "PST2",
						Type = SpellRoutine.PowerWord,
						Status = SpellStatus.Stun,
						Targeting = SpellTargeting.AllEnemies,
						Element = SpellElement.Time,
						Permissions = SpellPermission.BlackMage | SpellPermission.BlackWizard | SpellPermission.RedMage | SpellPermission.RedWizard | SpellPermission.Ninja,
						GraphicEffect = SpellGraphic.SparklingHand,
						Palette = SpellPalette.White
					});
					SpellIndices.Remove(index);
					return Spell.GetSpellLevel(index);
				default:
					return 0;
			}
		}

		private int CraftDarkSpell(SpellSchool school, int lolvl, int hilvl, int tier)
		{
			var index = GetSpellIndex(school, lolvl, hilvl);
			if (index < 0) return 0;

			Spells.Add(new Spell
			{
				Index = index,
				Name = "DRK" + tier.ToString(),
				Type = SpellRoutine.InflictStatus,
				Targeting = SpellTargeting.AllCharacters,
				Status = SpellStatus.Dark,
				Accuracy = 0x6B,
				Permissions = SpellPermission.BlackMage | SpellPermission.BlackWizard | SpellPermission.RedMage | SpellPermission.RedWizard | SpellPermission.Ninja,
				GraphicEffect = SpellGraphic.LargeSparkle,
				Palette = SpellPalette.Orange
			});
			SpellIndices.Remove(index);
			return Spell.GetSpellLevel(index);
		}

		private int CraftInstaKillSpell(SpellSchool school, int lolvl, int hilvl, bool powerWord, SpellTargeting targeting, SpellElement element)
		{
			var index = GetSpellIndex(school, lolvl, hilvl);
			if (index < 0) return 0;

			string name;
			SpellPalette palette;
			SpellGraphic graphic;

			if (!powerWord)
			{
				switch (element)
				{
					case SpellElement.Death:
						name = targeting == SpellTargeting.OneEnemy ? "RUB1" : "RUB2";
						graphic = SpellGraphic.EnergyBeam;
						palette = SpellPalette.Gray;
						break;
					case SpellElement.Poison:
						name = targeting == SpellTargeting.OneEnemy ? "BNE1" : "BNE2";
						graphic = SpellGraphic.SparklingHand;
						palette = SpellPalette.DarkGreen;
						break;
					case SpellElement.Time:
						name = targeting == SpellTargeting.OneEnemy ? "ZAP1" : "ZAP2";
						graphic = SpellGraphic.EnergyBeam;
						palette = SpellPalette.White;
						break;
					case SpellElement.Earth:
						name = targeting == SpellTargeting.OneEnemy ? "QAK1" : "QAK2";
						graphic = SpellGraphic.GlowingBall;
						palette = SpellPalette.Brown;
						break;
					case SpellElement.Fire:
						name = targeting == SpellTargeting.OneEnemy ? "BLZ1" : "BLZ2";
						graphic = SpellGraphic.EnergyFlare;
						palette = SpellPalette.Red;
						break;
					default:
						name = targeting == SpellTargeting.OneEnemy ? "XXX1" : "XXX2";
						graphic = SpellGraphic.SparklingHand;
						palette = SpellPalette.Purple;
						break;
				}
			}
			else
			{
				switch (element)
				{
					case SpellElement.Death:
						name = targeting == SpellTargeting.OneEnemy ? "PRB1" : "PRB2";
						graphic = SpellGraphic.EnergyBeam;
						palette = SpellPalette.Gray;
						break;
					case SpellElement.Poison:
						name = targeting == SpellTargeting.OneEnemy ? "PBN1" : "PBN2";
						graphic = SpellGraphic.SparklingHand;
						palette = SpellPalette.DarkGreen;
						break;
					case SpellElement.Time:
						name = targeting == SpellTargeting.OneEnemy ? "PZP1" : "PZP2";
						graphic = SpellGraphic.EnergyBeam;
						palette = SpellPalette.White;
						break;
					case SpellElement.Earth:
						name = targeting == SpellTargeting.OneEnemy ? "PQK1" : "PQK1";
						graphic = SpellGraphic.GlowingBall;
						palette = SpellPalette.Brown;
						break;
					case SpellElement.Fire:
						name = targeting == SpellTargeting.OneEnemy ? "PBL1" : "PBL2";
						graphic = SpellGraphic.EnergyFlare;
						palette = SpellPalette.Red;
						break;
					default:
						name = targeting == SpellTargeting.OneEnemy ? "PXX1" : "PXX2";
						graphic = SpellGraphic.SparklingHand;
						palette = SpellPalette.Purple;
						break;
				}
			}

			var permissions = SpellPermission.BlackMage | SpellPermission.BlackWizard;
			if (targeting == SpellTargeting.OneEnemy) permissions |= SpellPermission.RedMage | SpellPermission.RedWizard;

			Spells.Add(new Spell
			{
				Index = index,
				Name = name,
				Type = powerWord ? SpellRoutine.PowerWord : SpellRoutine.InflictStatus,
				Targeting = targeting,
				Element = element,
				Status = element == SpellElement.Earth ? SpellStatus.Stone : SpellStatus.Death,
				Accuracy = powerWord ? (byte)0 : (targeting == SpellTargeting.OneEnemy ? (byte)rng.Between(0x28, 0x48) : (byte)rng.Between(0x08, 0x28)),
				Permissions = permissions,
				GraphicEffect = graphic,
				Palette = palette
			});
			SpellIndices.Remove(index);
			return Spell.GetSpellLevel(index);
		}

		private int CraftDamageSpell(int index, int tier, SpellElement element)
		{
			var targeting = tier >= 3 ? SpellTargeting.AllEnemies : SpellTargeting.OneEnemy;

			string name;
			SpellGraphic graphic;
			SpellPalette palette;
			switch (element)
			{
				case SpellElement.Earth:
					name = "CRS" + tier.ToString();
					graphic = SpellGraphic.GlowingBall;
					palette = SpellPalette.Brown;
					break;
				case SpellElement.Fire:
					name = "FIR" + tier.ToString();
					graphic = SpellGraphic.EnergyFlare;
					palette = SpellPalette.Red;
					break;
				case SpellElement.Ice:
					name = "ICE" + tier.ToString();
					graphic = SpellGraphic.FourSparkles;
					palette = SpellPalette.DarkBlue;
					break;
				default:
					name = "LIT" + tier.ToString();
					graphic = SpellGraphic.EnergyBeam;
					palette = SpellPalette.LightBlue;
					break;
			}

			byte effectivity;
			switch (tier)
			{
				case 1:
					effectivity = (byte)rng.Between(40, 80);
					break;
				case 2:
					effectivity = (byte)rng.Between(80, 120);
					break;
				case 3:
					effectivity = (byte)rng.Between(30, 60);
					break;
				default:
					effectivity = (byte)rng.Between(60, 90);
					break;
			}

			Spells.Add(new Spell
			{
				Index = index,
				Name = name,
				Type = SpellRoutine.Damage,
				Targeting = targeting,
				Element = element,
				Effectivity = effectivity,
				Accuracy = targeting == SpellTargeting.OneEnemy ? (byte)rng.Between(0x28, 0x48) : (byte)rng.Between(0x08, 0x28),
				Permissions = SpellPermission.BlackMage | SpellPermission.BlackWizard | SpellPermission.RedMage | SpellPermission.RedWizard | SpellPermission.Ninja,
				GraphicEffect = graphic,
				Palette = palette
			});
			SpellIndices.Remove(index);
			return Spell.GetSpellLevel(index);
		}

		private int GetSpellIndex(SpellSchool school, int lolvl, int hilvl)
		{			
			var candidates = SpellIndices.Where(i => Spell.GetSpellSchool(i) == school && Spell.GetSpellLevel(i) >= lolvl && Spell.GetSpellLevel(i) <= hilvl).ToList();

			if (candidates.Count > 0)
			{
				return candidates.PickRandom(rng);
			}
			else
			{
				return -1;
			}
		}

		private List<int> GetCombinationSet(int choices, int min, int max)
		{
			int cmax = (int)Math.Pow(2, choices);

			List<int> result = new List<int>();

			for (int i = 0; i < cmax; i++)
			{
				var bits = NumberOfSetBits(i);
				if (bits >= min && bits <= max) result.Add(i);
			}

			return result;
		}

		private int NumberOfSetBits(int i)
		{
			i = i - ((i >> 1) & 0x55555555);
			i = (i & 0x33333333) + ((i >> 2) & 0x33333333);
			return (((i + (i >> 4)) & 0x0F0F0F0F) * 0x01010101) >> 24;
		}
	}
}
