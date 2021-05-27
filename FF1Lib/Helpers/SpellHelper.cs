using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static FF1Lib.FF1Rom;

namespace FF1Lib.Helpers
{
	public enum SpellRoutine : byte
	{
		Damage = 0x01,
		DamageUndead = 0x02,
		Heal = 0x07,
		CureAilment = 0x08,
		FullHeal = 0x0F,
		ArmorUp = 0x09,
		DefElement = 0x0A,
		Fast = 0x0C,
		Sabr = 0x0D,
		Lock = 0x0E,
		Ruse = 0x10,
		PowerWord = 0x12,
		InflictStatus = 0x03,
		Life = 0xF0,
		Smoke = 0xF1
	}

	public enum SpellTargeting : byte
	{
		Any = 0xFF,
		AllEnemies = 0x01,
		OneEnemy = 0x02,
		Self = 0x04,
		AllCharacters = 0x08,
		OneCharacters = 0x10
	}

	public enum SpellElement : byte
	{
		Any = 0b10101010,
		None = 0x00,
		Earth = 0b10000000,
		Lightning = 0b01000000,
		Ice = 0b00100000,
		Fire = 0b00010000,
		Death =	0b00001000,
		Time = 0b00000100,
		Poison = 0b00000010,
		Elem01 = 0b00000001
	}

	public enum SpellStatus : byte
	{
		Any = 0xFF,
		Confuse = 0b10000000,
		Mute = 0b01000000,
		Dark = 0b00001000,
		Stun = 0b00010000,
		Sleep = 0b00100000,
		Stone = 0b00000010,
		Death = 0b00000001,
		Poison = 0b00000100
	}

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
