using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FF1Lib.SpellCrafter
{
	public enum SpellRoutine : byte
	{
		None = 0x00,
		Damage = 0x01,
		DamageUndead = 0x02,
		Slow = 0x04,
		Fear = 0x05,
		Heal = 0x07,
		CureAilment = 0x08,
		FullHeal = 0x0F,
		ArmorUp = 0x09,
		Wall = 0x0A,
		Fast = 0x0C,
		Sabr = 0x0D,
		Lock = 0x0E,
		Ruse = 0x10,
		Xfer = 0x11,
		PowerWord = 0x12,
		InflictStatus = 0x03
	}

	public enum SpellTargeting : byte
	{
		AllEnemies = 0x01,
		OneEnemy = 0x02,
		Self = 0x04,
		AllCharacters = 0x08,
		OneCharacters = 0x10
	}

	public enum SpellElement : byte
	{
		None = 0b00000000,
		Status = 0b00000001,
		Poison = 0b00000010,
		Time = 0b00000100,
		Death = 0b00001000,
		Fire = 0b00010000,
		Ice = 0b00100000,
		Lightning = 0b01000000,
		Earth = 0b10000000
	}

	public enum SpellStatus : byte
	{
		None = 0b00000000,
		Death = 0b00000001,
		Stone = 0b00000010,
		Poison = 0b00000100,
		Dark = 0b00001000,
		Stun = 0b00010000,
		Sleep = 0b00100000,
		Mute = 0b01000000,
		Confuse = 0b10000000
	}

	public enum SpellPermission : short
	{
		None = 0x000,
		Figher = 0x001,
		Thief = 0x002,
		BlackBelt = 0x004,
		RedMage = 0x008,
		WhiteMage = 0x010,
		BlackMage = 0x020,
		Knight = 0x040,
		Ninja = 0x080,
		Master = 0x100,
		RedWizard = 0x200,
		WhiteWizard = 0x400,
		BlackWizard = 0x800,
		All = 0xFFF
	}

	public enum SpellSchool : byte
	{
		White,
		Black
	}

	public enum SpellPalette : byte
	{
		White = 0x20,
		LightBlue = 0x21,
		DarkBlue = 0x22,
		Violet = 0x23,
		Purple = 0x24,
		Magenta = 0x25,
		Red = 0x26,
		Orange = 0x27,
		Brown = 0x28,
		LightGreen = 0x29,
		DarkGreen = 0x2A,
		MintGreen = 0x2B,
		Teal = 0x2C,
		Gray = 0x2D,
		Black1 = 0x2E,
		Black2 = 0x2F
	}

	public enum SpellGraphic : byte
	{
		None = 0x00,
		BarOfLight = 0xB0,
		FourSparkles = 0xB8,
		Stars = 0xC0,
		EnergyBeam = 0xC8,
		EnergyFlare = 0xD0,
		GlowingBall = 0xD8,
		LargeSparkle = 0xE0,
		SparklingHand = 0xE8
	}

	public enum SpellMessage : byte
	{
		None,
		HpUp,
		ArmorUp,
		EasyToDodge,
		Asleep,
		EasyToHit,
		SightRecovered,
		Silenced,
		DefendLighnting,
		Darkness,
		WeaponsStronger,
		LostIntelligence,//Really
		DefendFire,
		AttackHalted,
		Neutralized,
		BecameTerridied,
		DefendCold,
		BreakTheSilence,
		QuickShot,
		Confused,
		Poisoned,
		Erased,
		FellIntoCrack,
		Paralyzed,
		HpMax,
		DefendMagic,
		BrokenIntoPieces,
		WeaponEnchanted,
		DefendAll,
		Defenseless,
		TimeStopped,
		Exiled,
		Slain,
		Ineffective,
		ChanceToFirstStrike,
		MonstersStrikeFirst,
		CantRun,
		RunAway,
		CloseCall,
		WokeUp,
		Sleeping,
		Cured,
		Paralyzed2,
		Hits,
		CriticalHit,
		MagicBlocked,
		Dmg,
		Stopped,
		LevelUp,
		HpMax2,
		Pts,
		Str,
		Agi,
		Int,
		Vit,
		Luck,
		Up,
		IDontKnow,
		IDontKnow2,
		Down,
		Perished,
		Monsters,
		Party,
		Terminated,
		Missed,
		IDontKnow3,
		IDontKnow4,
		IDontKnow5,
		Dark,
		Stun,
		IDontKnow6,
		Mute,
		Hp,
		ExpUp,
		IneffectiveNow,
		Silence,
		GoMad,
		PoisonSmoke,
		NothingHappens
	}

	public enum OOBSpellEffects
	{
		CURE,
		CUR2,
		CUR3,
		CUR4,
		HEAL,
		HEL3,
		HEL2,
		PURE,
		LIFE,
		LIF2,
		WARP,
		SOFT,
		EXIT
	}

	public enum SpellTag
	{
		PURE,
		SOFT,
		LIFE,
		LIF2,
		WARP,
		EXIT,
		RUSE,
		INVS,
		FAST,
		TMPR,
		SABR,
		LOCK,
		SMOKE,
		FOG,
	}
}
