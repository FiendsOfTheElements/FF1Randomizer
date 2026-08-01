namespace FF1Lib
{
	public partial class FF1Rom
	{
		List<AlternateFiends> FF5AltFiendsList = new()
		{
			new AlternateFiends
			{
				Name = "ABDUCTOR",
				ElementalWeakness = SpellElement.Status,
				MonsterType = MonsterType.MAGICAL,
				SpellChance1 = 0x00,
				Spells1 = new List<byte>
				{
					(byte)SpellByte.NONE,
					(byte)SpellByte.NONE,
					(byte)SpellByte.NONE,
					(byte)SpellByte.NONE,
					(byte)SpellByte.NONE,
					(byte)SpellByte.NONE,
					(byte)SpellByte.NONE,
					(byte)SpellByte.NONE,
				},
				SkillChance1 = 0x40,
				Skills1 = new List<byte>
				{
					(byte)EnemySkills.Stare,
					(byte)EnemySkills.Dazzle,
					(byte)EnemySkills.Trance,
					(byte)EnemySkills.Cremate,
				},
				SpellChance2 = 0x00,
				Spells2 = new List<byte>
				{
					(byte)SpellByte.NONE,
					(byte)SpellByte.NONE,
					(byte)SpellByte.NONE,
					(byte)SpellByte.NONE,
					(byte)SpellByte.NONE,
					(byte)SpellByte.NONE,
					(byte)SpellByte.NONE,
					(byte)SpellByte.NONE,
				},
				SkillChance2 = 0x40,
				Skills2 = new List<byte>
				{
					(byte)EnemySkills.Toxic,
					(byte)EnemySkills.Poison_Damage,
					(byte)EnemySkills.Blaze,
					(byte)EnemySkills.Crack,
				},
			},
			new AlternateFiends
			{
				Name = "ALTAROIT",
				ElementalWeakness = SpellElement.Time,
				MonsterType = MonsterType.MAGE,
				SpellChance1 = 0x40,
				Spells1 = new List<byte>
				{
					(byte)SpellByte.ICE2,
					(byte)SpellByte.SLOW,
					(byte)SpellByte.SLEP,
					(byte)SpellByte.LIT2,
					(byte)SpellByte.MUTE,
					(byte)SpellByte.ICE2,
					(byte)SpellByte.LIT2,
					(byte)SpellByte.INVS,
				},
				SkillChance1 = 0x00,
				Skills1 = new List<byte>
				{
					(byte)EnemySkills.None,
					(byte)EnemySkills.None,
					(byte)EnemySkills.None,
					(byte)EnemySkills.None,
				},
				SpellChance2 = 0x40,
				Spells2 = new List<byte>
				{
					(byte)SpellByte.ZAP,
					(byte)SpellByte.ICE3,
					(byte)SpellByte.LIT2,
					(byte)SpellByte.XXXX,
					(byte)SpellByte.CUR3,
					(byte)SpellByte.ICE2,
					(byte)SpellByte.SLO2,
					(byte)SpellByte.LIT2,
				},
				SkillChance2 = 0x00,
				Skills2 = new List<byte>
				{
					(byte)EnemySkills.None,
					(byte)EnemySkills.None,
					(byte)EnemySkills.None,
					(byte)EnemySkills.None,
				},
			},
			new AlternateFiends
			{
				Name = "APANDA",
				ElementalWeakness = SpellElement.Fire,
				MonsterType = MonsterType.MAGICAL,
				SpellChance1 = 0x40,
				Spells1 = new List<byte>
				{
					(byte)SpellByte.SLOW,
					(byte)SpellByte.CUR2,
					(byte)SpellByte.LIT,
					(byte)SpellByte.LIT2,
					(byte)SpellByte.FOG,
					(byte)SpellByte.FOG,
					(byte)SpellByte.CUR2,
					(byte)SpellByte.SLOW,
				},
				SkillChance1 = 0x40,
				Skills1 = new List<byte>
				{
					(byte)EnemySkills.Trance,
					(byte)EnemySkills.Stinger,
					(byte)EnemySkills.Frost,
					(byte)EnemySkills.Flash,
				},
				SpellChance2 = 0x40,
				Spells2 = new List<byte>
				{
					(byte)SpellByte.LIT3,
					(byte)SpellByte.FOG2,
					(byte)SpellByte.CUR3,
					(byte)SpellByte.LOK2,
					(byte)SpellByte.LIT2,
					(byte)SpellByte.FOG2,
					(byte)SpellByte.CUR3,
					(byte)SpellByte.LIT3,
				},
				SkillChance2 = 0x40,
				Skills2 = new List<byte>
				{
					(byte)EnemySkills.Tornado,
					(byte)EnemySkills.Nuclear,
					(byte)EnemySkills.Poison_Stone,
					(byte)EnemySkills.Glare,
				},
			},
			new AlternateFiends
			{
				Name = "ARCHAVIS",
				ElementalWeakness = SpellElement.Lightning,
				MonsterType = MonsterType.DRAGON,
				SpellChance1 = 0x40,
				Spells1 = new List<byte>
				{
					(byte)SpellByte.BANE,
					(byte)SpellByte.FAST,
					(byte)SpellByte.HOLD,
					(byte)SpellByte.FIR2,
					(byte)SpellByte.SLEP,
					(byte)SpellByte.FIR2,
					(byte)SpellByte.MUTE,
					(byte)SpellByte.FIR2,
				},
				SkillChance1 = 0x00,
				Skills1 = new List<byte>
				{
					(byte)EnemySkills.None,
					(byte)EnemySkills.None,
					(byte)EnemySkills.None,
					(byte)EnemySkills.None,
				},
				SpellChance2 = 0x00,
				Spells2 = new List<byte>
				{
					(byte)SpellByte.NONE,
					(byte)SpellByte.NONE,
					(byte)SpellByte.NONE,
					(byte)SpellByte.NONE,
					(byte)SpellByte.NONE,
					(byte)SpellByte.NONE,
					(byte)SpellByte.NONE,
					(byte)SpellByte.NONE,
				},
				SkillChance2 = 0x40,
				Skills2 = new List<byte>
				{
					(byte)EnemySkills.Tornado,
					(byte)EnemySkills.Toxic,
					(byte)EnemySkills.Glance,
					(byte)EnemySkills.Thunder,
				},
			},
			new AlternateFiends
			{
				Name = "ATMOS",
				ElementalWeakness = SpellElement.Lightning,
				MonsterType = MonsterType.MAGICAL,
				SpellChance1 = 0x40,
				Spells1 = new List<byte>
				{
					(byte)SpellByte.SLOW,
					(byte)SpellByte.FAST,
					(byte)SpellByte.INVS,
					(byte)SpellByte.HOLD,
					(byte)SpellByte.SLOW,
					(byte)SpellByte.HOLD,
					(byte)SpellByte.LOCK,
					(byte)SpellByte.LOCK,
				},
				SkillChance1 = 0x00,
				Skills1 = new List<byte>
				{
					(byte)EnemySkills.None,
					(byte)EnemySkills.None,
					(byte)EnemySkills.None,
					(byte)EnemySkills.None,
				},
				SpellChance2 = 0x40,
				Spells2 = new List<byte>
				{
					(byte)SpellByte.BANE,
					(byte)SpellByte.ZAP,
					(byte)SpellByte.XXXX,
					(byte)SpellByte.SLO2,
					(byte)SpellByte.ZAP,
					(byte)SpellByte.SLO2,
					(byte)SpellByte.XXXX,
					(byte)SpellByte.CUR3,
				},
				SkillChance2 = 0x00,
				Skills2 = new List<byte>
				{
					(byte)EnemySkills.None,
					(byte)EnemySkills.None,
					(byte)EnemySkills.None,
					(byte)EnemySkills.None,
				},
			},
			new AlternateFiends
			{
				Name = "ARCHDEMN",
				ElementalWeakness = SpellElement.Fire,
				MonsterType = MonsterType.UNDEAD,
				SpellChance1 = 0x40,
				Spells1 = new List<byte>
				{
					(byte)SpellByte.FIR2,
					(byte)SpellByte.LOCK,
					(byte)SpellByte.FIR2,
					(byte)SpellByte.STUN,
					(byte)SpellByte.FIR2,
					(byte)SpellByte.SLOW,
					(byte)SpellByte.FIR2,
					(byte)SpellByte.LOCK,
				},
				SkillChance1 = 0x40,
				Skills1 = new List<byte>
				{
					(byte)EnemySkills.Dazzle,
					(byte)EnemySkills.Trance,
					(byte)EnemySkills.Snorting,
					(byte)EnemySkills.Stinger,
				},
				SpellChance2 = 0x40,
				Spells2 = new List<byte>
				{
					(byte)SpellByte.FIR3,
					(byte)SpellByte.INVS,
					(byte)SpellByte.NUKE,
					(byte)SpellByte.FAST,
					(byte)SpellByte.FIR3,
					(byte)SpellByte.STOP,
					(byte)SpellByte.MUTE,
					(byte)SpellByte.FIR3,
				},
				SkillChance2 = 0x00,
				Skills2 = new List<byte>
				{
					(byte)EnemySkills.None,
					(byte)EnemySkills.None,
					(byte)EnemySkills.None,
					(byte)EnemySkills.None,
				},
			},
			new AlternateFiends
			{
				Name = "AZULMAGE",
				ElementalWeakness = SpellElement.Poison,
				MonsterType = MonsterType.MAGE,
				SpellChance1 = 0x40,
				Spells1 = new List<byte>
				{
					(byte)SpellByte.FIR2,
					(byte)SpellByte.ICE2,
					(byte)SpellByte.LIT2,
					(byte)SpellByte.SLOW,
					(byte)SpellByte.FOG,
					(byte)SpellByte.INVS,
					(byte)SpellByte.FOG,
					(byte)SpellByte.MUTE,
				},
				SkillChance1 = 0x40,
				Skills1 = new List<byte>
				{
					(byte)EnemySkills.Cremate,
					(byte)EnemySkills.Frost,
					(byte)EnemySkills.Gaze,
					(byte)EnemySkills.Heat,
				},
				SpellChance2 = 0x40,
				Spells2 = new List<byte>
				{
					(byte)SpellByte.FIR3,
					(byte)SpellByte.ICE3,
					(byte)SpellByte.LIT3,
					(byte)SpellByte.STOP,
					(byte)SpellByte.LOK2,
					(byte)SpellByte.ZAP,
					(byte)SpellByte.CUR4,
					(byte)SpellByte.INVS,
				},
				SkillChance2 = 0x40,
				Skills2 = new List<byte>
				{
					(byte)EnemySkills.Toxic,
					(byte)EnemySkills.Blaze,
					(byte)EnemySkills.Trance,
					(byte)EnemySkills.Stare,
				},
			},
			new AlternateFiends
			{
				Name = "BYBLOS",
				ElementalWeakness = SpellElement.Fire,
				MonsterType = MonsterType.MAGICAL,
				SpellChance1 = 0x00,
				Spells1 = new List<byte>
				{
					(byte)SpellByte.NONE,
					(byte)SpellByte.NONE,
					(byte)SpellByte.NONE,
					(byte)SpellByte.NONE,
					(byte)SpellByte.NONE,
					(byte)SpellByte.NONE,
					(byte)SpellByte.NONE,
					(byte)SpellByte.NONE,
				},
				SkillChance1 = 0x40,
				Skills1 = new List<byte>
				{
					(byte)EnemySkills.Squint,
					(byte)EnemySkills.Dazzle,
					(byte)EnemySkills.Frost,
					(byte)EnemySkills.Trance,
				},
				SpellChance2 = 0x00,
				Spells2 = new List<byte>
				{
					(byte)SpellByte.NONE,
					(byte)SpellByte.NONE,
					(byte)SpellByte.NONE,
					(byte)SpellByte.NONE,
					(byte)SpellByte.NONE,
					(byte)SpellByte.NONE,
					(byte)SpellByte.NONE,
					(byte)SpellByte.NONE,
				},
				SkillChance2 = 0x40,
				Skills2 = new List<byte>
				{
					(byte)EnemySkills.Crack,
					(byte)EnemySkills.Tornado,
					(byte)EnemySkills.Poison_Damage,
					(byte)EnemySkills.Toxic,
				},
			},
			new AlternateFiends
			{
				Name = "C.BRAIN",
				ElementalWeakness = SpellElement.Time,
				MonsterType = MonsterType.MAGICAL,
				SpellChance1 = 0x40,
				Spells1 = new List<byte>
				{
					(byte)SpellByte.ICE,
					(byte)SpellByte.STUN,
					(byte)SpellByte.FIRE,
					(byte)SpellByte.SLOW,
					(byte)SpellByte.ICE2,
					(byte)SpellByte.STUN,
					(byte)SpellByte.FIR2,
					(byte)SpellByte.TMPR,
				},
				SkillChance1 = 0x40,
				Skills1 = new List<byte>
				{
					(byte)EnemySkills.Heat,
					(byte)EnemySkills.Ink,
					(byte)EnemySkills.Stinger,
					(byte)EnemySkills.Dazzle,
				},
				SpellChance2 = 0x40,
				Spells2 = new List<byte>
				{
					(byte)SpellByte.ICE3,
					(byte)SpellByte.RUB,
					(byte)SpellByte.FIR3,
					(byte)SpellByte.SLP2,
					(byte)SpellByte.ICE2,
					(byte)SpellByte.XFER,
					(byte)SpellByte.FIR2,
					(byte)SpellByte.FAST,
				},
				SkillChance2 = 0x40,
				Skills2 = new List<byte>
				{
					(byte)EnemySkills.Stinger,
					(byte)EnemySkills.Swirl,
					(byte)EnemySkills.Stare,
					(byte)EnemySkills.Poison_Stone,
				},
			},
			new AlternateFiends
			{
				Name = "CALOFIST",
				ElementalWeakness = SpellElement.Status,
				MonsterType = MonsterType.MAGE,
				SpellChance1 = 0x40,
				Spells1 = new List<byte>
				{
					(byte)SpellByte.FAST,
					(byte)SpellByte.FOG,
					(byte)SpellByte.CUR2,
					(byte)SpellByte.SLOW,
					(byte)SpellByte.INVS,
					(byte)SpellByte.STUN,
					(byte)SpellByte.SLOW,
					(byte)SpellByte.SLEP,
				},
				SkillChance1 = 0x00,
				Skills1 = new List<byte>
				{
					(byte)EnemySkills.None,
					(byte)EnemySkills.None,
					(byte)EnemySkills.None,
					(byte)EnemySkills.None,
				},
				SpellChance2 = 0x40,
				Spells2 = new List<byte>
				{
					(byte)SpellByte.INVS,
					(byte)SpellByte.FOG2,
					(byte)SpellByte.CUR4,
					(byte)SpellByte.SLO2,
					(byte)SpellByte.FADE,
					(byte)SpellByte.WALL,
					(byte)SpellByte.BANE,
					(byte)SpellByte.MUTE,
				},
				SkillChance2 = 0x00,
				Skills2 = new List<byte>
				{
					(byte)EnemySkills.None,
					(byte)EnemySkills.None,
					(byte)EnemySkills.None,
					(byte)EnemySkills.None,
				},
			},
			new AlternateFiends
			{
				Name = "CATOBLEP",
				ElementalWeakness = SpellElement.Poison,
				MonsterType = MonsterType.DRAGON,
				SpellChance1 = 0x40,
				Spells1 = new List<byte>
				{
					(byte)SpellByte.FIR2,
					(byte)SpellByte.STUN,
					(byte)SpellByte.SLOW,
					(byte)SpellByte.FIRE,
					(byte)SpellByte.BRAK,
					(byte)SpellByte.FIR2,
					(byte)SpellByte.STUN,
					(byte)SpellByte.BRAK,
				},
				SkillChance1 = 0x00,
				Skills1 = new List<byte>
				{
					(byte)EnemySkills.None,
					(byte)EnemySkills.None,
					(byte)EnemySkills.None,
					(byte)EnemySkills.None,
				},
				SpellChance2 = 0x00,
				Spells2 = new List<byte>
				{
					(byte)SpellByte.NONE,
					(byte)SpellByte.NONE,
					(byte)SpellByte.NONE,
					(byte)SpellByte.NONE,
					(byte)SpellByte.NONE,
					(byte)SpellByte.NONE,
					(byte)SpellByte.NONE,
					(byte)SpellByte.NONE,
				},
				SkillChance2 = 0x40,
				Skills2 = new List<byte>
				{
					(byte)EnemySkills.Poison_Stone,
					(byte)EnemySkills.Blaze,
					(byte)EnemySkills.Glance,
					(byte)EnemySkills.Gaze,
				},
			},
			new AlternateFiends
			{
				Name = "CRAYCLAW",
				ElementalWeakness = SpellElement.Lightning,
				MonsterType = MonsterType.MAGICAL,
				SpellChance1 = 0x00,
				Spells1 = new List<byte>
				{
					(byte)SpellByte.NONE,
					(byte)SpellByte.NONE,
					(byte)SpellByte.NONE,
					(byte)SpellByte.NONE,
					(byte)SpellByte.NONE,
					(byte)SpellByte.NONE,
					(byte)SpellByte.NONE,
					(byte)SpellByte.NONE,
				},
				SkillChance1 = 0x40,
				Skills1 = new List<byte>
				{
					(byte)EnemySkills.Stinger,
					(byte)EnemySkills.Stare,
					(byte)EnemySkills.Stare,
					(byte)EnemySkills.Gaze,
				},
				SpellChance2 = 0x00,
				Spells2 = new List<byte>
				{
					(byte)SpellByte.NONE,
					(byte)SpellByte.NONE,
					(byte)SpellByte.NONE,
					(byte)SpellByte.NONE,
					(byte)SpellByte.NONE,
					(byte)SpellByte.NONE,
					(byte)SpellByte.NONE,
					(byte)SpellByte.NONE,
				},
				SkillChance2 = 0x40,
				Skills2 = new List<byte>
				{
					(byte)EnemySkills.Crack,
					(byte)EnemySkills.Toxic,
					(byte)EnemySkills.Tornado,
					(byte)EnemySkills.Blaze,
				},
			},
			new AlternateFiends
			{
				Name = "CTSTRPHE",
				ElementalWeakness = SpellElement.Status,
				MonsterType = MonsterType.MAGICAL,
				SpellChance1 = 0x40,
				Spells1 = new List<byte>
				{
					(byte)SpellByte.SABR,
					(byte)SpellByte.INVS,
					(byte)SpellByte.QAKE,
					(byte)SpellByte.CUR2,
					(byte)SpellByte.SABR,
					(byte)SpellByte.INVS,
					(byte)SpellByte.QAKE,
					(byte)SpellByte.CUR2,
				},
				SkillChance1 = 0x00,
				Skills1 = new List<byte>
				{
					(byte)EnemySkills.None,
					(byte)EnemySkills.None,
					(byte)EnemySkills.None,
					(byte)EnemySkills.None,
				},
				SpellChance2 = 0x40,
				Spells2 = new List<byte>
				{
					(byte)SpellByte.QAKE,
					(byte)SpellByte.BRAK,
					(byte)SpellByte.NUKE,
					(byte)SpellByte.XXXX,
					(byte)SpellByte.RUB,
					(byte)SpellByte.BRAK,
					(byte)SpellByte.CUR3,
					(byte)SpellByte.QAKE,
				},
				SkillChance2 = 0x00,
				Skills2 = new List<byte>
				{
					(byte)EnemySkills.None,
					(byte)EnemySkills.None,
					(byte)EnemySkills.None,
					(byte)EnemySkills.None,
				},
			},
			new AlternateFiends
			{
				Name = "D.POD",
				ElementalWeakness = SpellElement.Death,
				MonsterType = MonsterType.REGENERATIVE,
				SpellChance1 = 0x40,
				Spells1 = new List<byte>
				{
					(byte)SpellByte.SLEP,
					(byte)SpellByte.SLOW,
					(byte)SpellByte.STUN,
					(byte)SpellByte.HOLD,
					(byte)SpellByte.SLEP,
					(byte)SpellByte.SLOW,
					(byte)SpellByte.HOLD,
					(byte)SpellByte.STUN,
				},
				SkillChance1 = 0x40,
				Skills1 = new List<byte>
				{
					(byte)EnemySkills.Stinger,
					(byte)EnemySkills.Gaze,
					(byte)EnemySkills.Stinger,
					(byte)EnemySkills.Gaze,
				},
				SpellChance2 = 0x40,
				Spells2 = new List<byte>
				{
					(byte)SpellByte.SLP2,
					(byte)SpellByte.MUTE,
					(byte)SpellByte.SLP2,
					(byte)SpellByte.SLO2,
					(byte)SpellByte.MUTE,
					(byte)SpellByte.BRAK,
					(byte)SpellByte.LOK2,
					(byte)SpellByte.MUTE,
				},
				SkillChance2 = 0x00,
				Skills2 = new List<byte>
				{
					(byte)EnemySkills.None,
					(byte)EnemySkills.None,
					(byte)EnemySkills.None,
					(byte)EnemySkills.None,
				},
			},
			new AlternateFiends
			{
				Name = "ENKIDU",
				ElementalWeakness = SpellElement.Time,
				MonsterType = MonsterType.GIANT,
				SpellChance1 = 0x40,
				Spells1 = new List<byte>
				{
					(byte)SpellByte.LIT2,
					(byte)SpellByte.STUN,
					(byte)SpellByte.LIT,
					(byte)SpellByte.SLOW,
					(byte)SpellByte.LIT,
					(byte)SpellByte.TMPR,
					(byte)SpellByte.INVS,
					(byte)SpellByte.LIT2,
				},
				SkillChance1 = 0x40,
				Skills1 = new List<byte>
				{
					(byte)EnemySkills.Glare,
					(byte)EnemySkills.Trance,
					(byte)EnemySkills.Flash,
					(byte)EnemySkills.Dazzle,
				},
				SpellChance2 = 0x40,
				Spells2 = new List<byte>
				{
					(byte)SpellByte.LIT3,
					(byte)SpellByte.CUR3,
					(byte)SpellByte.SLO2,
					(byte)SpellByte.LIT3,
					(byte)SpellByte.FOG2,
					(byte)SpellByte.LOK2,
					(byte)SpellByte.LIT3,
					(byte)SpellByte.STOP,
				},
				SkillChance2 = 0x40,
				Skills2 = new List<byte>
				{
					(byte)EnemySkills.Tornado,
					(byte)EnemySkills.Squint,
					(byte)EnemySkills.Tornado,
					(byte)EnemySkills.Glare,
				},
			},
			new AlternateFiends
			{
				Name = "EXDEATH",
				ElementalWeakness = SpellElement.None,
				MonsterType = MonsterType.REGENERATIVE,
				SpellChance1 = 0x40,
				Spells1 = new List<byte>
				{
					(byte)SpellByte.LIT2,
					(byte)SpellByte.SLOW,
					(byte)SpellByte.FIR2,
					(byte)SpellByte.INVS,
					(byte)SpellByte.ICE2,
					(byte)SpellByte.CUR2,
					(byte)SpellByte.FOG,
					(byte)SpellByte.RUB,
				},
				SkillChance1 = 0x00,
				Skills1 = new List<byte>
				{
					(byte)EnemySkills.None,
					(byte)EnemySkills.None,
					(byte)EnemySkills.None,
					(byte)EnemySkills.None,
				},
				SpellChance2 = 0x40,
				Spells2 = new List<byte>
				{
					(byte)SpellByte.ZAP,
					(byte)SpellByte.BRAK,
					(byte)SpellByte.QAKE,
					(byte)SpellByte.ICE3,
					(byte)SpellByte.CUR3,
					(byte)SpellByte.FIR3,
					(byte)SpellByte.LIT3,
					(byte)SpellByte.BRAK,
				},
				SkillChance2 = 0x40,
				Skills2 = new List<byte>
				{
					(byte)EnemySkills.Crack,
					(byte)EnemySkills.Blaze,
					(byte)EnemySkills.Blizzard,
					(byte)EnemySkills.Poison_Stone,
				},
			},
			new AlternateFiends
			{
				Name = "FORZA",
				ElementalWeakness = SpellElement.Status,
				MonsterType = MonsterType.GIANT,
				SpellChance1 = 0x00,
				Spells1 = new List<byte>
				{
					(byte)SpellByte.NONE,
					(byte)SpellByte.NONE,
					(byte)SpellByte.NONE,
					(byte)SpellByte.NONE,
					(byte)SpellByte.NONE,
					(byte)SpellByte.NONE,
					(byte)SpellByte.NONE,
					(byte)SpellByte.NONE,
				},
				SkillChance1 = 0x00,
				Skills1 = new List<byte>
				{
					(byte)EnemySkills.None,
					(byte)EnemySkills.None,
					(byte)EnemySkills.None,
					(byte)EnemySkills.None,
				},
				SpellChance2 = 0x00,
				Spells2 = new List<byte>
				{
					(byte)SpellByte.NONE,
					(byte)SpellByte.NONE,
					(byte)SpellByte.NONE,
					(byte)SpellByte.NONE,
					(byte)SpellByte.NONE,
					(byte)SpellByte.NONE,
					(byte)SpellByte.NONE,
					(byte)SpellByte.NONE,
				},
				SkillChance2 = 0x00,
				Skills2 = new List<byte>
				{
					(byte)EnemySkills.None,
					(byte)EnemySkills.None,
					(byte)EnemySkills.None,
					(byte)EnemySkills.None,
				},
			},
			new AlternateFiends
			{
				Name = "GARULA",
				ElementalWeakness = SpellElement.Status,
				MonsterType = MonsterType.MAGICAL,
				SpellChance1 = 0x00,
				Spells1 = new List<byte>
				{
					(byte)SpellByte.NONE,
					(byte)SpellByte.NONE,
					(byte)SpellByte.NONE,
					(byte)SpellByte.NONE,
					(byte)SpellByte.NONE,
					(byte)SpellByte.NONE,
					(byte)SpellByte.NONE,
					(byte)SpellByte.NONE,
				},
				SkillChance1 = 0x00,
				Skills1 = new List<byte>
				{
					(byte)EnemySkills.None,
					(byte)EnemySkills.None,
					(byte)EnemySkills.None,
					(byte)EnemySkills.None,
				},
				SpellChance2 = 0x00,
				Spells2 = new List<byte>
				{
					(byte)SpellByte.NONE,
					(byte)SpellByte.NONE,
					(byte)SpellByte.NONE,
					(byte)SpellByte.NONE,
					(byte)SpellByte.NONE,
					(byte)SpellByte.NONE,
					(byte)SpellByte.NONE,
					(byte)SpellByte.NONE,
				},
				SkillChance2 = 0x00,
				Skills2 = new List<byte>
				{
					(byte)EnemySkills.None,
					(byte)EnemySkills.None,
					(byte)EnemySkills.None,
					(byte)EnemySkills.None,
				},
			},
			new AlternateFiends
			{
				Name = "GILGAMSH",
				ElementalWeakness = SpellElement.Time,
				MonsterType = MonsterType.NONE,
				SpellChance1 = 0x40,
				Spells1 = new List<byte>
				{
					(byte)SpellByte.FAST,
					(byte)SpellByte.XFER,
					(byte)SpellByte.SLOW,
					(byte)SpellByte.WALL,
					(byte)SpellByte.FAST,
					(byte)SpellByte.XFER,
					(byte)SpellByte.SLOW,
					(byte)SpellByte.WALL,
				},
				SkillChance1 = 0x00,
				Skills1 = new List<byte>
				{
					(byte)EnemySkills.None,
					(byte)EnemySkills.None,
					(byte)EnemySkills.None,
					(byte)EnemySkills.None,
				},
				SpellChance2 = 0x40,
				Spells2 = new List<byte>
				{
					(byte)SpellByte.FAST,
					(byte)SpellByte.XFER,
					(byte)SpellByte.SLO2,
					(byte)SpellByte.SABR,
					(byte)SpellByte.XXXX,
					(byte)SpellByte.SABR,
					(byte)SpellByte.SLO2,
					(byte)SpellByte.WALL,
				},
				SkillChance2 = 0x00,
				Skills2 = new List<byte>
				{
					(byte)EnemySkills.None,
					(byte)EnemySkills.None,
					(byte)EnemySkills.None,
					(byte)EnemySkills.None,
				},
			},
			new AlternateFiends
			{
				Name = "GOGO",
				ElementalWeakness = SpellElement.None,
				MonsterType = MonsterType.GIANT,
				SpellChance1 = 0x40,
				Spells1 = new List<byte>
				{
					(byte)SpellByte.ICE2,
					(byte)SpellByte.INVS,
					(byte)SpellByte.LOCK,
					(byte)SpellByte.FIR2,
					(byte)SpellByte.LAMP,
					(byte)SpellByte.TMPR,
					(byte)SpellByte.SLOW,
					(byte)SpellByte.LIT2,
				},
				SkillChance1 = 0x40,
				Skills1 = new List<byte>
				{
					(byte)EnemySkills.Frost,
					(byte)EnemySkills.Cremate,
					(byte)EnemySkills.Dazzle,
					(byte)EnemySkills.Ink,
				},
				SpellChance2 = 0x40,
				Spells2 = new List<byte>
				{
					(byte)SpellByte.FADE,
					(byte)SpellByte.FAST,
					(byte)SpellByte.SLO2,
					(byte)SpellByte.INVS,
					(byte)SpellByte.RUB,
					(byte)SpellByte.TMPR,
					(byte)SpellByte.CUR3,
					(byte)SpellByte.XFER,
				},
				SkillChance2 = 0x40,
				Skills2 = new List<byte>
				{
					(byte)EnemySkills.Blizzard,
					(byte)EnemySkills.Thunder,
					(byte)EnemySkills.Poison_Damage,
					(byte)EnemySkills.Toxic,
				},
			},
			new AlternateFiends
			{
				Name = "GOLEM",
				ElementalWeakness = SpellElement.None,
				MonsterType = MonsterType.REGENERATIVE,
				SpellChance1 = 0x40,
				Spells1 = new List<byte>
				{
					(byte)SpellByte.INVS,
					(byte)SpellByte.INVS,
					(byte)SpellByte.FOG,
					(byte)SpellByte.TMPR,
					(byte)SpellByte.CUR2,
					(byte)SpellByte.TMPR,
					(byte)SpellByte.MUTE,
					(byte)SpellByte.LOCK,
				},
				SkillChance1 = 0x00,
				Skills1 = new List<byte>
				{
					(byte)EnemySkills.None,
					(byte)EnemySkills.None,
					(byte)EnemySkills.None,
					(byte)EnemySkills.None,
				},
				SpellChance2 = 0x40,
				Spells2 = new List<byte>
				{
					(byte)SpellByte.FAST,
					(byte)SpellByte.CUR3,
					(byte)SpellByte.INVS,
					(byte)SpellByte.WALL,
					(byte)SpellByte.FOG2,
					(byte)SpellByte.SABR,
					(byte)SpellByte.SLOW,
					(byte)SpellByte.CUR3,
				},
				SkillChance2 = 0x00,
				Skills2 = new List<byte>
				{
					(byte)EnemySkills.None,
					(byte)EnemySkills.None,
					(byte)EnemySkills.None,
					(byte)EnemySkills.None,
				},
			},
			new AlternateFiends
			{
				Name = "KARLABOS",
				ElementalWeakness = SpellElement.Lightning,
				MonsterType = MonsterType.AQUATIC,
				SpellChance1 = 0x00,
				Spells1 = new List<byte>
				{
					(byte)SpellByte.NONE,
					(byte)SpellByte.NONE,
					(byte)SpellByte.NONE,
					(byte)SpellByte.NONE,
					(byte)SpellByte.NONE,
					(byte)SpellByte.NONE,
					(byte)SpellByte.NONE,
					(byte)SpellByte.NONE,
				},
				SkillChance1 = 0x40,
				Skills1 = new List<byte>
				{
					(byte)EnemySkills.Dazzle,
					(byte)EnemySkills.Gaze,
					(byte)EnemySkills.Dazzle,
					(byte)EnemySkills.Gaze,
				},
				SpellChance2 = 0x00,
				Spells2 = new List<byte>
				{
					(byte)SpellByte.NONE,
					(byte)SpellByte.NONE,
					(byte)SpellByte.NONE,
					(byte)SpellByte.NONE,
					(byte)SpellByte.NONE,
					(byte)SpellByte.NONE,
					(byte)SpellByte.NONE,
					(byte)SpellByte.NONE,
				},
				SkillChance2 = 0x40,
				Skills2 = new List<byte>
				{
					(byte)EnemySkills.Swirl,
					(byte)EnemySkills.Dazzle,
					(byte)EnemySkills.Nuclear,
					(byte)EnemySkills.Dazzle,
				},
			},
			new AlternateFiends
			{
				Name = "L.FLAME",
				ElementalWeakness = SpellElement.Ice,
				MonsterType = MonsterType.MAGICAL,
				SpellChance1 = 0x40,
				Spells1 = new List<byte>
				{
					(byte)SpellByte.FIR2,
					(byte)SpellByte.FIRE,
					(byte)SpellByte.FIRE,
					(byte)SpellByte.SLOW,
					(byte)SpellByte.FOG,
					(byte)SpellByte.FIR2,
					(byte)SpellByte.FIRE,
					(byte)SpellByte.STUN,
				},
				SkillChance1 = 0x40,
				Skills1 = new List<byte>
				{
					(byte)EnemySkills.Heat,
					(byte)EnemySkills.Cremate,
					(byte)EnemySkills.Flash,
					(byte)EnemySkills.Flash,
				},
				SpellChance2 = 0x40,
				Spells2 = new List<byte>
				{
					(byte)SpellByte.FIR3,
					(byte)SpellByte.FOG,
					(byte)SpellByte.FIR3,
					(byte)SpellByte.FIR3,
					(byte)SpellByte.SABR,
					(byte)SpellByte.HEL3,
					(byte)SpellByte.FIR3,
					(byte)SpellByte.FIR3,
				},
				SkillChance2 = 0x40,
				Skills2 = new List<byte>
				{
					(byte)EnemySkills.Inferno,
					(byte)EnemySkills.Glare,
					(byte)EnemySkills.Toxic,
					(byte)EnemySkills.Blaze,
				},
			},
			new AlternateFiends
			{
				Name = "MAGISSA",
				ElementalWeakness = SpellElement.None,
				MonsterType = MonsterType.MAGE,
				SpellChance1 = 0x40,
				Spells1 = new List<byte>
				{
					(byte)SpellByte.CUR2,
					(byte)SpellByte.FIR2,
					(byte)SpellByte.ICE2,
					(byte)SpellByte.LIT,
					(byte)SpellByte.FIRE,
					(byte)SpellByte.CUR2,
					(byte)SpellByte.LIT2,
					(byte)SpellByte.ICE,
				},
				SkillChance1 = 0x40,
				Skills1 = new List<byte>
				{
					(byte)EnemySkills.Stinger,
					(byte)EnemySkills.Stinger,
					(byte)EnemySkills.Stinger,
					(byte)EnemySkills.Stinger,
				},
				SpellChance2 = 0x40,
				Spells2 = new List<byte>
				{
					(byte)SpellByte.FIR3,
					(byte)SpellByte.ICE3,
					(byte)SpellByte.CUR3,
					(byte)SpellByte.ICE3,
					(byte)SpellByte.INVS,
					(byte)SpellByte.HEL3,
					(byte)SpellByte.BANE,
					(byte)SpellByte.LIT3,
				},
				SkillChance2 = 0x00,
				Skills2 = new List<byte>
				{
					(byte)EnemySkills.None,
					(byte)EnemySkills.None,
					(byte)EnemySkills.None,
					(byte)EnemySkills.None,
				},
			},
			new AlternateFiends
			{
				Name = "MELUSINE",
				ElementalWeakness = SpellElement.Poison,
				MonsterType = MonsterType.MAGE,
				SpellChance1 = 0x40,
				Spells1 = new List<byte>
				{
					(byte)SpellByte.LIT2,
					(byte)SpellByte.MUTE,
					(byte)SpellByte.ICE2,
					(byte)SpellByte.SLOW,
					(byte)SpellByte.FIR2,
					(byte)SpellByte.INVS,
					(byte)SpellByte.STUN,
					(byte)SpellByte.FOG,
				},
				SkillChance1 = 0x00,
				Skills1 = new List<byte>
				{
					(byte)EnemySkills.None,
					(byte)EnemySkills.None,
					(byte)EnemySkills.None,
					(byte)EnemySkills.None,
				},
				SpellChance2 = 0x40,
				Spells2 = new List<byte>
				{
					(byte)SpellByte.FIR3,
					(byte)SpellByte.SLO2,
					(byte)SpellByte.LIT3,
					(byte)SpellByte.WALL,
					(byte)SpellByte.FAST,
					(byte)SpellByte.MUTE,
					(byte)SpellByte.XXXX,
					(byte)SpellByte.BANE,
				},
				SkillChance2 = 0x00,
				Skills2 = new List<byte>
				{
					(byte)EnemySkills.None,
					(byte)EnemySkills.None,
					(byte)EnemySkills.None,
					(byte)EnemySkills.None,
				},
			},
			new AlternateFiends
			{
				Name = "MINOS",
				ElementalWeakness = SpellElement.None,
				MonsterType = MonsterType.GIANT,
				SpellChance1 = 0x00,
				Spells1 = new List<byte>
				{
					(byte)SpellByte.NONE,
					(byte)SpellByte.NONE,
					(byte)SpellByte.NONE,
					(byte)SpellByte.NONE,
					(byte)SpellByte.NONE,
					(byte)SpellByte.NONE,
					(byte)SpellByte.NONE,
					(byte)SpellByte.NONE,
				},
				SkillChance1 = 0x40,
				Skills1 = new List<byte>
				{
					(byte)EnemySkills.Snorting,
					(byte)EnemySkills.Stare,
					(byte)EnemySkills.Dazzle,
					(byte)EnemySkills.Trance,
				},
				SpellChance2 = 0x00,
				Spells2 = new List<byte>
				{
					(byte)SpellByte.NONE,
					(byte)SpellByte.NONE,
					(byte)SpellByte.NONE,
					(byte)SpellByte.NONE,
					(byte)SpellByte.NONE,
					(byte)SpellByte.NONE,
					(byte)SpellByte.NONE,
					(byte)SpellByte.NONE,
				},
				SkillChance2 = 0x40,
				Skills2 = new List<byte>
				{
					(byte)EnemySkills.Crack,
					(byte)EnemySkills.Toxic,
					(byte)EnemySkills.Poison_Damage,
					(byte)EnemySkills.Poison_Stone,
				},
			},
			new AlternateFiends
			{
				Name = "NCROPHBE",
				ElementalWeakness = SpellElement.All,
				MonsterType = MonsterType.GIANT,
				SpellChance1 = 0x40,
				Spells1 = new List<byte>
				{
					(byte)SpellByte.FIR2,
					(byte)SpellByte.AFIR,
					(byte)SpellByte.ICE2,
					(byte)SpellByte.AICE,
					(byte)SpellByte.LIT2,
					(byte)SpellByte.ALIT,
					(byte)SpellByte.RUB,
					(byte)SpellByte.HEL2,
				},
				SkillChance1 = 0x40,
				Skills1 = new List<byte>
				{
					(byte)EnemySkills.Cremate,
					(byte)EnemySkills.Frost,
					(byte)EnemySkills.Dazzle,
					(byte)EnemySkills.Trance,
				},
				SpellChance2 = 0x40,
				Spells2 = new List<byte>
				{
					(byte)SpellByte.FIR3,
					(byte)SpellByte.WALL,
					(byte)SpellByte.XXXX,
					(byte)SpellByte.XFER,
					(byte)SpellByte.SLO2,
					(byte)SpellByte.ICE3,
					(byte)SpellByte.MUTE,
					(byte)SpellByte.ZAP,
				},
				SkillChance2 = 0x40,
				Skills2 = new List<byte>
				{
					(byte)EnemySkills.Poison_Stone,
					(byte)EnemySkills.Swirl,
					(byte)EnemySkills.Inferno,
					(byte)EnemySkills.Stare,
				},
			},
			new AlternateFiends
			{
				Name = "OMEGA",
				ElementalWeakness = SpellElement.None,
				MonsterType = MonsterType.REGENERATIVE,
				SpellChance1 = 0x40,
				Spells1 = new List<byte>
				{
					(byte)SpellByte.HOLD,
					(byte)SpellByte.QAKE,
					(byte)SpellByte.FIR2,
					(byte)SpellByte.ICE2,
					(byte)SpellByte.INVS,
					(byte)SpellByte.SLOW,
					(byte)SpellByte.LIT2,
					(byte)SpellByte.CUR2,
				},
				SkillChance1 = 0x40,
				Skills1 = new List<byte>
				{
					(byte)EnemySkills.Stare,
					(byte)EnemySkills.Cremate,
					(byte)EnemySkills.Dazzle,
					(byte)EnemySkills.Stinger,
				},
				SpellChance2 = 0x40,
				Spells2 = new List<byte>
				{
					(byte)SpellByte.NUKE,
					(byte)SpellByte.XXXX,
					(byte)SpellByte.LOK2,
					(byte)SpellByte.ICE3,
					(byte)SpellByte.CUR3,
					(byte)SpellByte.FAST,
					(byte)SpellByte.SLO2,
					(byte)SpellByte.ICE3,
				},
				SkillChance2 = 0x40,
				Skills2 = new List<byte>
				{
					(byte)EnemySkills.Glance,
					(byte)EnemySkills.Toxic,
					(byte)EnemySkills.Nuclear,
					(byte)EnemySkills.Crack,
				},
			},
			new AlternateFiends
			{
				Name = "OMNISENT",
				ElementalWeakness = SpellElement.Earth,
				MonsterType = MonsterType.MAGE,
				SpellChance1 = 0x40,
				Spells1 = new List<byte>
				{
					(byte)SpellByte.MUTE,
					(byte)SpellByte.FIR2,
					(byte)SpellByte.LIT2,
					(byte)SpellByte.STUN,
					(byte)SpellByte.ICE2,
					(byte)SpellByte.MUTE,
					(byte)SpellByte.ZAP,
					(byte)SpellByte.FOG,
				},
				SkillChance1 = 0x00,
				Skills1 = new List<byte>
				{
					(byte)EnemySkills.None,
					(byte)EnemySkills.None,
					(byte)EnemySkills.None,
					(byte)EnemySkills.None,
				},
				SpellChance2 = 0x40,
				Spells2 = new List<byte>
				{
					(byte)SpellByte.INVS,
					(byte)SpellByte.FADE,
					(byte)SpellByte.CUR3,
					(byte)SpellByte.LIT3,
					(byte)SpellByte.XXXX,
					(byte)SpellByte.STOP,
					(byte)SpellByte.INVS,
					(byte)SpellByte.NUKE,
				},
				SkillChance2 = 0x00,
				Skills2 = new List<byte>
				{
					(byte)EnemySkills.None,
					(byte)EnemySkills.None,
					(byte)EnemySkills.None,
					(byte)EnemySkills.None,
				},
			},
			new AlternateFiends
			{
				Name = "SHINRYU",
				ElementalWeakness = SpellElement.None,
				MonsterType = MonsterType.DRAGON,
				SpellChance1 = 0x40,
				Spells1 = new List<byte>
				{
					(byte)SpellByte.INVS,
					(byte)SpellByte.ZAP,
					(byte)SpellByte.ICE2,
					(byte)SpellByte.ICE2,
					(byte)SpellByte.LIT2,
					(byte)SpellByte.FAST,
					(byte)SpellByte.SLOW,
					(byte)SpellByte.LIT2,
				},
				SkillChance1 = 0x40,
				Skills1 = new List<byte>
				{
					(byte)EnemySkills.Cremate,
					(byte)EnemySkills.Frost,
					(byte)EnemySkills.Gaze,
					(byte)EnemySkills.Trance,
				},
				SpellChance2 = 0x40,
				Spells2 = new List<byte>
				{
					(byte)SpellByte.QAKE,
					(byte)SpellByte.FADE,
					(byte)SpellByte.CUR3,
					(byte)SpellByte.FAST,
					(byte)SpellByte.SLO2,
					(byte)SpellByte.SABR,
					(byte)SpellByte.RUB,
					(byte)SpellByte.XXXX,
				},
				SkillChance2 = 0x40,
				Skills2 = new List<byte>
				{
					(byte)EnemySkills.Toxic,
					(byte)EnemySkills.Blizzard,
					(byte)EnemySkills.Swirl,
					(byte)EnemySkills.Crack,
				},
			},
			new AlternateFiends
			{
				Name = "SIREN",
				ElementalWeakness = SpellElement.Fire,
				MonsterType = MonsterType.UNDEAD,
				SpellChance1 = 0x00,
				Spells1 = new List<byte>
				{
					(byte)SpellByte.NONE,
					(byte)SpellByte.NONE,
					(byte)SpellByte.NONE,
					(byte)SpellByte.NONE,
					(byte)SpellByte.NONE,
					(byte)SpellByte.NONE,
					(byte)SpellByte.NONE,
					(byte)SpellByte.NONE,
				},
				SkillChance1 = 0x40,
				Skills1 = new List<byte>
				{
					(byte)EnemySkills.Stinger,
					(byte)EnemySkills.Trance,
					(byte)EnemySkills.Frost,
					(byte)EnemySkills.Glance,
				},
				SpellChance2 = 0x00,
				Spells2 = new List<byte>
				{
					(byte)SpellByte.NONE,
					(byte)SpellByte.NONE,
					(byte)SpellByte.NONE,
					(byte)SpellByte.NONE,
					(byte)SpellByte.NONE,
					(byte)SpellByte.NONE,
					(byte)SpellByte.NONE,
					(byte)SpellByte.NONE,
				},
				SkillChance2 = 0x40,
				Skills2 = new List<byte>
				{
					(byte)EnemySkills.Toxic,
					(byte)EnemySkills.Poison_Stone,
					(byte)EnemySkills.Crack,
					(byte)EnemySkills.Squint,
				},
			},
			new AlternateFiends
			{
				Name = "S.CANNON",
				ElementalWeakness = SpellElement.Lightning,
				MonsterType = MonsterType.REGENERATIVE,
				SpellChance1 = 0x00,
				Spells1 = new List<byte>
				{
					(byte)SpellByte.NONE,
					(byte)SpellByte.NONE,
					(byte)SpellByte.NONE,
					(byte)SpellByte.NONE,
					(byte)SpellByte.NONE,
					(byte)SpellByte.NONE,
					(byte)SpellByte.NONE,
					(byte)SpellByte.NONE,
				},
				SkillChance1 = 0x40,
				Skills1 = new List<byte>
				{
					(byte)EnemySkills.Cremate,
					(byte)EnemySkills.Crack,
					(byte)EnemySkills.Stare,
					(byte)EnemySkills.Trance,
				},
				SpellChance2 = 0x00,
				Spells2 = new List<byte>
				{
					(byte)SpellByte.NONE,
					(byte)SpellByte.NONE,
					(byte)SpellByte.NONE,
					(byte)SpellByte.NONE,
					(byte)SpellByte.NONE,
					(byte)SpellByte.NONE,
					(byte)SpellByte.NONE,
					(byte)SpellByte.NONE,
				},
				SkillChance2 = 0x40,
				Skills2 = new List<byte>
				{
					(byte)EnemySkills.Glance,
					(byte)EnemySkills.Poison_Damage,
					(byte)EnemySkills.Tornado,
					(byte)EnemySkills.Thunder,
				},
			},
			new AlternateFiends
			{
				Name = "TWINTANI",
				ElementalWeakness = SpellElement.Ice,
				MonsterType = MonsterType.DRAGON,
				SpellChance1 = 0x40,
				Spells1 = new List<byte>
				{
					(byte)SpellByte.INVS,
					(byte)SpellByte.FIR2,
					(byte)SpellByte.FIR2,
					(byte)SpellByte.HEL2,
					(byte)SpellByte.MUTE,
					(byte)SpellByte.LIT2,
					(byte)SpellByte.LIT2,
					(byte)SpellByte.SLOW,
				},
				SkillChance1 = 0x40,
				Skills1 = new List<byte>
				{
					(byte)EnemySkills.Trance,
					(byte)EnemySkills.Stinger,
					(byte)EnemySkills.Snorting,
					(byte)EnemySkills.Heat,
				},
				SpellChance2 = 0x40,
				Spells2 = new List<byte>
				{
					(byte)SpellByte.FAST,
					(byte)SpellByte.LIT3,
					(byte)SpellByte.LOCK,
					(byte)SpellByte.LOCK,
					(byte)SpellByte.NUKE,
					(byte)SpellByte.LIT3,
					(byte)SpellByte.SLO2,
					(byte)SpellByte.XXXX,
				},
				SkillChance2 = 0x40,
				Skills2 = new List<byte>
				{
					(byte)EnemySkills.Crack,
					(byte)EnemySkills.Poison_Stone,
					(byte)EnemySkills.Squint,
					(byte)EnemySkills.Blaze,
				},
			},
			new AlternateFiends
			{
				Name = "W.RAPTOR",
				ElementalWeakness = SpellElement.Lightning,
				MonsterType = MonsterType.GIANT,
				SpellChance1 = 0x00,
				Spells1 = new List<byte>
				{
					(byte)SpellByte.NONE,
					(byte)SpellByte.NONE,
					(byte)SpellByte.NONE,
					(byte)SpellByte.NONE,
					(byte)SpellByte.NONE,
					(byte)SpellByte.NONE,
					(byte)SpellByte.NONE,
					(byte)SpellByte.NONE,
				},
				SkillChance1 = 0x40,
				Skills1 = new List<byte>
				{
					(byte)EnemySkills.Stare,
					(byte)EnemySkills.Glare,
					(byte)EnemySkills.Squint,
					(byte)EnemySkills.Trance,
				},
				SpellChance2 = 0x00,
				Spells2 = new List<byte>
				{
					(byte)SpellByte.NONE,
					(byte)SpellByte.NONE,
					(byte)SpellByte.NONE,
					(byte)SpellByte.NONE,
					(byte)SpellByte.NONE,
					(byte)SpellByte.NONE,
					(byte)SpellByte.NONE,
					(byte)SpellByte.NONE,
				},
				SkillChance2 = 0x40,
				Skills2 = new List<byte>
				{
					(byte)EnemySkills.Tornado,
					(byte)EnemySkills.Poison_Damage,
					(byte)EnemySkills.Toxic,
					(byte)EnemySkills.Tornado,
				},
			},
		};

		List<AlternateFiends> FF5AltFiendListHarcore = new()
		{
			new AlternateFiends
			{
				Name = "ABDUCTOR",
				ElementalWeakness = SpellElement.Status,
				MonsterType = MonsterType.MAGICAL,
				SpellChance1 = 0x00,
				Spells1 = new List<byte>
				{
					(byte)SpellByte.NONE,
					(byte)SpellByte.NONE,
					(byte)SpellByte.NONE,
					(byte)SpellByte.NONE,
					(byte)SpellByte.NONE,
					(byte)SpellByte.NONE,
					(byte)SpellByte.NONE,
					(byte)SpellByte.NONE,
				},
				SkillChance1 = 0x40,
				Skills1 = new List<byte>
				{
					(byte)EnemySkills.Stare,
					(byte)EnemySkills.Dazzle,
					(byte)EnemySkills.Trance,
					(byte)EnemySkills.Cremate,
				},
				SpellChance2 = 0x00,
				Spells2 = new List<byte>
				{
					(byte)SpellByte.NONE,
					(byte)SpellByte.NONE,
					(byte)SpellByte.NONE,
					(byte)SpellByte.NONE,
					(byte)SpellByte.NONE,
					(byte)SpellByte.NONE,
					(byte)SpellByte.NONE,
					(byte)SpellByte.NONE,
				},
				SkillChance2 = 0x40,
				Skills2 = new List<byte>
				{
					(byte)EnemySkills.Toxic,
					(byte)EnemySkills.Poison_Damage,
					(byte)EnemySkills.Blaze,
					(byte)EnemySkills.Crack,
				},
			},
			new AlternateFiends
			{
				Name = "ALTAROIT",
				ElementalWeakness = SpellElement.Time,
				MonsterType = MonsterType.MAGE,
				SpellChance1 = 0x40,
				Spells1 = new List<byte>
				{
					(byte)SpellByte.ICE2,
					(byte)SpellByte.SLOW,
					(byte)SpellByte.SLEP,
					(byte)SpellByte.LIT2,
					(byte)SpellByte.MUTE,
					(byte)SpellByte.ICE2,
					(byte)SpellByte.LIT2,
					(byte)SpellByte.INVS,
				},
				SkillChance1 = 0x00,
				Skills1 = new List<byte>
				{
					(byte)EnemySkills.None,
					(byte)EnemySkills.None,
					(byte)EnemySkills.None,
					(byte)EnemySkills.None,
				},
				SpellChance2 = 0x40,
				Spells2 = new List<byte>
				{
					(byte)SpellByte.ZAP,
					(byte)SpellByte.ICE3,
					(byte)SpellByte.LIT3,
					(byte)SpellByte.XXXX,
					(byte)SpellByte.CUR3,
					(byte)SpellByte.ICE3,
					(byte)SpellByte.SLO2,
					(byte)SpellByte.LIT3,
				},
				SkillChance2 = 0x00,
				Skills2 = new List<byte>
				{
					(byte)EnemySkills.None,
					(byte)EnemySkills.None,
					(byte)EnemySkills.None,
					(byte)EnemySkills.None,
				},
			},
			new AlternateFiends
			{
				Name = "APANDA",
				ElementalWeakness = SpellElement.Fire,
				MonsterType = MonsterType.MAGICAL,
				SpellChance1 = 0x40,
				Spells1 = new List<byte>
				{
					(byte)SpellByte.SLOW,
					(byte)SpellByte.CUR2,
					(byte)SpellByte.LIT2,
					(byte)SpellByte.LIT2,
					(byte)SpellByte.FOG,
					(byte)SpellByte.FOG,
					(byte)SpellByte.CUR2,
					(byte)SpellByte.SLOW,
				},
				SkillChance1 = 0x40,
				Skills1 = new List<byte>
				{
					(byte)EnemySkills.Trance,
					(byte)EnemySkills.Stinger,
					(byte)EnemySkills.Frost,
					(byte)EnemySkills.Flash,
				},
				SpellChance2 = 0x40,
				Spells2 = new List<byte>
				{
					(byte)SpellByte.LIT3,
					(byte)SpellByte.FOG2,
					(byte)SpellByte.CUR3,
					(byte)SpellByte.LOK2,
					(byte)SpellByte.LIT3,
					(byte)SpellByte.FOG2,
					(byte)SpellByte.CUR3,
					(byte)SpellByte.LIT3,
				},
				SkillChance2 = 0x40,
				Skills2 = new List<byte>
				{
					(byte)EnemySkills.Tornado,
					(byte)EnemySkills.Nuclear,
					(byte)EnemySkills.Poison_Stone,
					(byte)EnemySkills.Glare,
				},
			},
			new AlternateFiends
			{
				Name = "ARCHAVIS",
				ElementalWeakness = SpellElement.Lightning,
				MonsterType = MonsterType.DRAGON,
				SpellChance1 = 0x40,
				Spells1 = new List<byte>
				{
					(byte)SpellByte.BANE,
					(byte)SpellByte.FAST,
					(byte)SpellByte.HOLD,
					(byte)SpellByte.FIR2,
					(byte)SpellByte.SLEP,
					(byte)SpellByte.FIR2,
					(byte)SpellByte.MUTE,
					(byte)SpellByte.FIR2,
				},
				SkillChance1 = 0x00,
				Skills1 = new List<byte>
				{
					(byte)EnemySkills.None,
					(byte)EnemySkills.None,
					(byte)EnemySkills.None,
					(byte)EnemySkills.None,
				},
				SpellChance2 = 0x00,
				Spells2 = new List<byte>
				{
					(byte)SpellByte.NONE,
					(byte)SpellByte.NONE,
					(byte)SpellByte.NONE,
					(byte)SpellByte.NONE,
					(byte)SpellByte.NONE,
					(byte)SpellByte.NONE,
					(byte)SpellByte.NONE,
					(byte)SpellByte.NONE,
				},
				SkillChance2 = 0x40,
				Skills2 = new List<byte>
				{
					(byte)EnemySkills.Tornado,
					(byte)EnemySkills.Toxic,
					(byte)EnemySkills.Glance,
					(byte)EnemySkills.Thunder,
				},
			},
			new AlternateFiends
			{
				Name = "ATMOS",
				ElementalWeakness = SpellElement.Lightning,
				MonsterType = MonsterType.MAGICAL,
				SpellChance1 = 0x40,
				Spells1 = new List<byte>
				{
					(byte)SpellByte.SLOW,
					(byte)SpellByte.FAST,
					(byte)SpellByte.INVS,
					(byte)SpellByte.HOLD,
					(byte)SpellByte.SLOW,
					(byte)SpellByte.HOLD,
					(byte)SpellByte.LOCK,
					(byte)SpellByte.LOCK,
				},
				SkillChance1 = 0x00,
				Skills1 = new List<byte>
				{
					(byte)EnemySkills.None,
					(byte)EnemySkills.None,
					(byte)EnemySkills.None,
					(byte)EnemySkills.None,
				},
				SpellChance2 = 0x40,
				Spells2 = new List<byte>
				{
					(byte)SpellByte.BANE,
					(byte)SpellByte.ZAP,
					(byte)SpellByte.XXXX,
					(byte)SpellByte.SLO2,
					(byte)SpellByte.ZAP,
					(byte)SpellByte.SLO2,
					(byte)SpellByte.XXXX,
					(byte)SpellByte.CUR3,
				},
				SkillChance2 = 0x00,
				Skills2 = new List<byte>
				{
					(byte)EnemySkills.None,
					(byte)EnemySkills.None,
					(byte)EnemySkills.None,
					(byte)EnemySkills.None,
				},
			},
			new AlternateFiends
			{
				Name = "ARCHDEMN",
				ElementalWeakness = SpellElement.Fire,
				MonsterType = MonsterType.UNDEAD,
				SpellChance1 = 0x40,
				Spells1 = new List<byte>
				{
					(byte)SpellByte.FIR2,
					(byte)SpellByte.LOCK,
					(byte)SpellByte.FIR2,
					(byte)SpellByte.STUN,
					(byte)SpellByte.FIR2,
					(byte)SpellByte.SLOW,
					(byte)SpellByte.FIR2,
					(byte)SpellByte.LOCK,
				},
				SkillChance1 = 0x40,
				Skills1 = new List<byte>
				{
					(byte)EnemySkills.Dazzle,
					(byte)EnemySkills.Trance,
					(byte)EnemySkills.Snorting,
					(byte)EnemySkills.Stinger,
				},
				SpellChance2 = 0x40,
				Spells2 = new List<byte>
				{
					(byte)SpellByte.FIR3,
					(byte)SpellByte.INVS,
					(byte)SpellByte.NUKE,
					(byte)SpellByte.FAST,
					(byte)SpellByte.FIR3,
					(byte)SpellByte.STOP,
					(byte)SpellByte.MUTE,
					(byte)SpellByte.FIR3,
				},
				SkillChance2 = 0x00,
				Skills2 = new List<byte>
				{
					(byte)EnemySkills.None,
					(byte)EnemySkills.None,
					(byte)EnemySkills.None,
					(byte)EnemySkills.None,
				},
			},
			new AlternateFiends
			{
				Name = "AZULMAGE",
				ElementalWeakness = SpellElement.Poison,
				MonsterType = MonsterType.MAGE,
				SpellChance1 = 0x40,
				Spells1 = new List<byte>
				{
					(byte)SpellByte.FIR2,
					(byte)SpellByte.ICE2,
					(byte)SpellByte.LIT2,
					(byte)SpellByte.SLOW,
					(byte)SpellByte.FOG,
					(byte)SpellByte.INVS,
					(byte)SpellByte.FOG,
					(byte)SpellByte.MUTE,
				},
				SkillChance1 = 0x40,
				Skills1 = new List<byte>
				{
					(byte)EnemySkills.Cremate,
					(byte)EnemySkills.Frost,
					(byte)EnemySkills.Gaze,
					(byte)EnemySkills.Heat,
				},
				SpellChance2 = 0x40,
				Spells2 = new List<byte>
				{
					(byte)SpellByte.FIR3,
					(byte)SpellByte.ICE3,
					(byte)SpellByte.LIT3,
					(byte)SpellByte.STOP,
					(byte)SpellByte.LOK2,
					(byte)SpellByte.ZAP,
					(byte)SpellByte.CUR4,
					(byte)SpellByte.INVS,
				},
				SkillChance2 = 0x40,
				Skills2 = new List<byte>
				{
					(byte)EnemySkills.Nuclear,
					(byte)EnemySkills.Blaze,
					(byte)EnemySkills.Trance,
					(byte)EnemySkills.Stare,
				},
			},
			new AlternateFiends
			{
				Name = "BYBLOS",
				ElementalWeakness = SpellElement.Fire,
				MonsterType = MonsterType.MAGICAL,
				SpellChance1 = 0x00,
				Spells1 = new List<byte>
				{
					(byte)SpellByte.NONE,
					(byte)SpellByte.NONE,
					(byte)SpellByte.NONE,
					(byte)SpellByte.NONE,
					(byte)SpellByte.NONE,
					(byte)SpellByte.NONE,
					(byte)SpellByte.NONE,
					(byte)SpellByte.NONE,
				},
				SkillChance1 = 0x40,
				Skills1 = new List<byte>
				{
					(byte)EnemySkills.Squint,
					(byte)EnemySkills.Dazzle,
					(byte)EnemySkills.Frost,
					(byte)EnemySkills.Trance,
				},
				SpellChance2 = 0x00,
				Spells2 = new List<byte>
				{
					(byte)SpellByte.NONE,
					(byte)SpellByte.NONE,
					(byte)SpellByte.NONE,
					(byte)SpellByte.NONE,
					(byte)SpellByte.NONE,
					(byte)SpellByte.NONE,
					(byte)SpellByte.NONE,
					(byte)SpellByte.NONE,
				},
				SkillChance2 = 0x40,
				Skills2 = new List<byte>
				{
					(byte)EnemySkills.Crack,
					(byte)EnemySkills.Tornado,
					(byte)EnemySkills.Poison_Damage,
					(byte)EnemySkills.Toxic,
				},
			},
			new AlternateFiends
			{
				Name = "C.BRAIN",
				ElementalWeakness = SpellElement.Time,
				MonsterType = MonsterType.MAGICAL,
				SpellChance1 = 0x40,
				Spells1 = new List<byte>
				{
					(byte)SpellByte.ICE2,
					(byte)SpellByte.STUN,
					(byte)SpellByte.FIR2,
					(byte)SpellByte.SLOW,
					(byte)SpellByte.ICE2,
					(byte)SpellByte.STUN,
					(byte)SpellByte.FIR2,
					(byte)SpellByte.TMPR,
				},
				SkillChance1 = 0x40,
				Skills1 = new List<byte>
				{
					(byte)EnemySkills.Heat,
					(byte)EnemySkills.Ink,
					(byte)EnemySkills.Stinger,
					(byte)EnemySkills.Dazzle,
				},
				SpellChance2 = 0x40,
				Spells2 = new List<byte>
				{
					(byte)SpellByte.ICE3,
					(byte)SpellByte.RUB,
					(byte)SpellByte.FIR3,
					(byte)SpellByte.SLP2,
					(byte)SpellByte.ICE3,
					(byte)SpellByte.XFER,
					(byte)SpellByte.FIR3,
					(byte)SpellByte.FAST,
				},
				SkillChance2 = 0x40,
				Skills2 = new List<byte>
				{
					(byte)EnemySkills.Stinger,
					(byte)EnemySkills.Swirl,
					(byte)EnemySkills.Stare,
					(byte)EnemySkills.Poison_Stone,
				},
			},
			new AlternateFiends
			{
				Name = "CALOFIST",
				ElementalWeakness = SpellElement.Status,
				MonsterType = MonsterType.MAGE,
				SpellChance1 = 0x40,
				Spells1 = new List<byte>
				{
					(byte)SpellByte.FAST,
					(byte)SpellByte.FOG,
					(byte)SpellByte.CUR2,
					(byte)SpellByte.SLOW,
					(byte)SpellByte.INVS,
					(byte)SpellByte.STUN,
					(byte)SpellByte.SLOW,
					(byte)SpellByte.SLEP,
				},
				SkillChance1 = 0x00,
				Skills1 = new List<byte>
				{
					(byte)EnemySkills.None,
					(byte)EnemySkills.None,
					(byte)EnemySkills.None,
					(byte)EnemySkills.None,
				},
				SpellChance2 = 0x40,
				Spells2 = new List<byte>
				{
					(byte)SpellByte.INVS,
					(byte)SpellByte.FOG2,
					(byte)SpellByte.CUR4,
					(byte)SpellByte.SLO2,
					(byte)SpellByte.FADE,
					(byte)SpellByte.WALL,
					(byte)SpellByte.BANE,
					(byte)SpellByte.MUTE,
				},
				SkillChance2 = 0x00,
				Skills2 = new List<byte>
				{
					(byte)EnemySkills.None,
					(byte)EnemySkills.None,
					(byte)EnemySkills.None,
					(byte)EnemySkills.None,
				},
			},
			new AlternateFiends
			{
				Name = "CATOBLEP",
				ElementalWeakness = SpellElement.Poison,
				MonsterType = MonsterType.DRAGON,
				SpellChance1 = 0x40,
				Spells1 = new List<byte>
				{
					(byte)SpellByte.FIR2,
					(byte)SpellByte.STUN,
					(byte)SpellByte.SLOW,
					(byte)SpellByte.FIR2,
					(byte)SpellByte.BRAK,
					(byte)SpellByte.FIR2,
					(byte)SpellByte.STUN,
					(byte)SpellByte.BRAK,
				},
				SkillChance1 = 0x00,
				Skills1 = new List<byte>
				{
					(byte)EnemySkills.None,
					(byte)EnemySkills.None,
					(byte)EnemySkills.None,
					(byte)EnemySkills.None,
				},
				SpellChance2 = 0x00,
				Spells2 = new List<byte>
				{
					(byte)SpellByte.NONE,
					(byte)SpellByte.NONE,
					(byte)SpellByte.NONE,
					(byte)SpellByte.NONE,
					(byte)SpellByte.NONE,
					(byte)SpellByte.NONE,
					(byte)SpellByte.NONE,
					(byte)SpellByte.NONE,
				},
				SkillChance2 = 0x40,
				Skills2 = new List<byte>
				{
					(byte)EnemySkills.Poison_Stone,
					(byte)EnemySkills.Blaze,
					(byte)EnemySkills.Glance,
					(byte)EnemySkills.Gaze,
				},
			},
			new AlternateFiends
			{
				Name = "CRAYCLAW",
				ElementalWeakness = SpellElement.Lightning,
				MonsterType = MonsterType.MAGICAL,
				SpellChance1 = 0x00,
				Spells1 = new List<byte>
				{
					(byte)SpellByte.NONE,
					(byte)SpellByte.NONE,
					(byte)SpellByte.NONE,
					(byte)SpellByte.NONE,
					(byte)SpellByte.NONE,
					(byte)SpellByte.NONE,
					(byte)SpellByte.NONE,
					(byte)SpellByte.NONE,
				},
				SkillChance1 = 0x40,
				Skills1 = new List<byte>
				{
					(byte)EnemySkills.Stinger,
					(byte)EnemySkills.Stare,
					(byte)EnemySkills.Stare,
					(byte)EnemySkills.Gaze,
				},
				SpellChance2 = 0x00,
				Spells2 = new List<byte>
				{
					(byte)SpellByte.NONE,
					(byte)SpellByte.NONE,
					(byte)SpellByte.NONE,
					(byte)SpellByte.NONE,
					(byte)SpellByte.NONE,
					(byte)SpellByte.NONE,
					(byte)SpellByte.NONE,
					(byte)SpellByte.NONE,
				},
				SkillChance2 = 0x40,
				Skills2 = new List<byte>
				{
					(byte)EnemySkills.Crack,
					(byte)EnemySkills.Toxic,
					(byte)EnemySkills.Tornado,
					(byte)EnemySkills.Blaze,
				},
			},
			new AlternateFiends
			{
				Name = "CTSTRPHE",
				ElementalWeakness = SpellElement.Status,
				MonsterType = MonsterType.MAGICAL,
				SpellChance1 = 0x40,
				Spells1 = new List<byte>
				{
					(byte)SpellByte.SABR,
					(byte)SpellByte.INVS,
					(byte)SpellByte.QAKE,
					(byte)SpellByte.CUR2,
					(byte)SpellByte.SABR,
					(byte)SpellByte.INVS,
					(byte)SpellByte.QAKE,
					(byte)SpellByte.CUR2,
				},
				SkillChance1 = 0x00,
				Skills1 = new List<byte>
				{
					(byte)EnemySkills.None,
					(byte)EnemySkills.None,
					(byte)EnemySkills.None,
					(byte)EnemySkills.None,
				},
				SpellChance2 = 0x40,
				Spells2 = new List<byte>
				{
					(byte)SpellByte.QAKE,
					(byte)SpellByte.BRAK,
					(byte)SpellByte.NUKE,
					(byte)SpellByte.XXXX,
					(byte)SpellByte.RUB,
					(byte)SpellByte.BRAK,
					(byte)SpellByte.CUR3,
					(byte)SpellByte.QAKE,
				},
				SkillChance2 = 0x00,
				Skills2 = new List<byte>
				{
					(byte)EnemySkills.None,
					(byte)EnemySkills.None,
					(byte)EnemySkills.None,
					(byte)EnemySkills.None,
				},
			},
			new AlternateFiends
			{
				Name = "D.POD",
				ElementalWeakness = SpellElement.Death,
				MonsterType = MonsterType.REGENERATIVE,
				SpellChance1 = 0x40,
				Spells1 = new List<byte>
				{
					(byte)SpellByte.SLEP,
					(byte)SpellByte.SLOW,
					(byte)SpellByte.STUN,
					(byte)SpellByte.HOLD,
					(byte)SpellByte.SLEP,
					(byte)SpellByte.SLOW,
					(byte)SpellByte.HOLD,
					(byte)SpellByte.STUN,
				},
				SkillChance1 = 0x40,
				Skills1 = new List<byte>
				{
					(byte)EnemySkills.Stinger,
					(byte)EnemySkills.Gaze,
					(byte)EnemySkills.Stinger,
					(byte)EnemySkills.Gaze,
				},
				SpellChance2 = 0x40,
				Spells2 = new List<byte>
				{
					(byte)SpellByte.SLP2,
					(byte)SpellByte.MUTE,
					(byte)SpellByte.SLP2,
					(byte)SpellByte.SLO2,
					(byte)SpellByte.MUTE,
					(byte)SpellByte.BRAK,
					(byte)SpellByte.LOK2,
					(byte)SpellByte.MUTE,
				},
				SkillChance2 = 0x00,
				Skills2 = new List<byte>
				{
					(byte)EnemySkills.None,
					(byte)EnemySkills.None,
					(byte)EnemySkills.None,
					(byte)EnemySkills.None,
				},
			},
			new AlternateFiends
			{
				Name = "ENKIDU",
				ElementalWeakness = SpellElement.Time,
				MonsterType = MonsterType.GIANT,
				SpellChance1 = 0x40,
				Spells1 = new List<byte>
				{
					(byte)SpellByte.LIT2,
					(byte)SpellByte.STUN,
					(byte)SpellByte.LIT2,
					(byte)SpellByte.SLOW,
					(byte)SpellByte.LIT2,
					(byte)SpellByte.TMPR,
					(byte)SpellByte.INVS,
					(byte)SpellByte.LIT2,
				},
				SkillChance1 = 0x40,
				Skills1 = new List<byte>
				{
					(byte)EnemySkills.Glare,
					(byte)EnemySkills.Trance,
					(byte)EnemySkills.Flash,
					(byte)EnemySkills.Dazzle,
				},
				SpellChance2 = 0x40,
				Spells2 = new List<byte>
				{
					(byte)SpellByte.LIT3,
					(byte)SpellByte.CUR4,
					(byte)SpellByte.SLO2,
					(byte)SpellByte.LIT3,
					(byte)SpellByte.FOG2,
					(byte)SpellByte.LOK2,
					(byte)SpellByte.LIT3,
					(byte)SpellByte.STOP,
				},
				SkillChance2 = 0x40,
				Skills2 = new List<byte>
				{
					(byte)EnemySkills.Tornado,
					(byte)EnemySkills.Squint,
					(byte)EnemySkills.Tornado,
					(byte)EnemySkills.Glare,
				},
			},
			new AlternateFiends
			{
				Name = "EXDEATH",
				ElementalWeakness = SpellElement.None,
				MonsterType = MonsterType.REGENERATIVE,
				SpellChance1 = 0x40,
				Spells1 = new List<byte>
				{
					(byte)SpellByte.LIT2,
					(byte)SpellByte.SLOW,
					(byte)SpellByte.FIR2,
					(byte)SpellByte.INVS,
					(byte)SpellByte.ICE2,
					(byte)SpellByte.CUR2,
					(byte)SpellByte.FOG,
					(byte)SpellByte.RUB,
				},
				SkillChance1 = 0x00,
				Skills1 = new List<byte>
				{
					(byte)EnemySkills.None,
					(byte)EnemySkills.None,
					(byte)EnemySkills.None,
					(byte)EnemySkills.None,
				},
				SpellChance2 = 0x40,
				Spells2 = new List<byte>
				{
					(byte)SpellByte.ZAP,
					(byte)SpellByte.NUKE,
					(byte)SpellByte.QAKE,
					(byte)SpellByte.ICE3,
					(byte)SpellByte.CUR4,
					(byte)SpellByte.FIR3,
					(byte)SpellByte.LIT3,
					(byte)SpellByte.BRAK,
				},
				SkillChance2 = 0x40,
				Skills2 = new List<byte>
				{
					(byte)EnemySkills.Crack,
					(byte)EnemySkills.Blaze,
					(byte)EnemySkills.Blizzard,
					(byte)EnemySkills.Poison_Stone,
				},
			},
			new AlternateFiends
			{
				Name = "FORZA",
				ElementalWeakness = SpellElement.Status,
				MonsterType = MonsterType.GIANT,
				SpellChance1 = 0x00,
				Spells1 = new List<byte>
				{
					(byte)SpellByte.NONE,
					(byte)SpellByte.NONE,
					(byte)SpellByte.NONE,
					(byte)SpellByte.NONE,
					(byte)SpellByte.NONE,
					(byte)SpellByte.NONE,
					(byte)SpellByte.NONE,
					(byte)SpellByte.NONE,
				},
				SkillChance1 = 0x00,
				Skills1 = new List<byte>
				{
					(byte)EnemySkills.None,
					(byte)EnemySkills.None,
					(byte)EnemySkills.None,
					(byte)EnemySkills.None,
				},
				SpellChance2 = 0x00,
				Spells2 = new List<byte>
				{
					(byte)SpellByte.NONE,
					(byte)SpellByte.NONE,
					(byte)SpellByte.NONE,
					(byte)SpellByte.NONE,
					(byte)SpellByte.NONE,
					(byte)SpellByte.NONE,
					(byte)SpellByte.NONE,
					(byte)SpellByte.NONE,
				},
				SkillChance2 = 0x00,
				Skills2 = new List<byte>
				{
					(byte)EnemySkills.None,
					(byte)EnemySkills.None,
					(byte)EnemySkills.None,
					(byte)EnemySkills.None,
				},
			},
			new AlternateFiends
			{
				Name = "GARULA",
				ElementalWeakness = SpellElement.Status,
				MonsterType = MonsterType.MAGICAL,
				SpellChance1 = 0x00,
				Spells1 = new List<byte>
				{
					(byte)SpellByte.NONE,
					(byte)SpellByte.NONE,
					(byte)SpellByte.NONE,
					(byte)SpellByte.NONE,
					(byte)SpellByte.NONE,
					(byte)SpellByte.NONE,
					(byte)SpellByte.NONE,
					(byte)SpellByte.NONE,
				},
				SkillChance1 = 0x00,
				Skills1 = new List<byte>
				{
					(byte)EnemySkills.None,
					(byte)EnemySkills.None,
					(byte)EnemySkills.None,
					(byte)EnemySkills.None,
				},
				SpellChance2 = 0x00,
				Spells2 = new List<byte>
				{
					(byte)SpellByte.NONE,
					(byte)SpellByte.NONE,
					(byte)SpellByte.NONE,
					(byte)SpellByte.NONE,
					(byte)SpellByte.NONE,
					(byte)SpellByte.NONE,
					(byte)SpellByte.NONE,
					(byte)SpellByte.NONE,
				},
				SkillChance2 = 0x00,
				Skills2 = new List<byte>
				{
					(byte)EnemySkills.None,
					(byte)EnemySkills.None,
					(byte)EnemySkills.None,
					(byte)EnemySkills.None,
				},
			},
			new AlternateFiends
			{
				Name = "GILGAMSH",
				ElementalWeakness = SpellElement.Time,
				MonsterType = MonsterType.NONE,
				SpellChance1 = 0x40,
				Spells1 = new List<byte>
				{
					(byte)SpellByte.FAST,
					(byte)SpellByte.XFER,
					(byte)SpellByte.SLOW,
					(byte)SpellByte.WALL,
					(byte)SpellByte.FAST,
					(byte)SpellByte.XFER,
					(byte)SpellByte.SLOW,
					(byte)SpellByte.WALL,
				},
				SkillChance1 = 0x00,
				Skills1 = new List<byte>
				{
					(byte)EnemySkills.None,
					(byte)EnemySkills.None,
					(byte)EnemySkills.None,
					(byte)EnemySkills.None,
				},
				SpellChance2 = 0x40,
				Spells2 = new List<byte>
				{
					(byte)SpellByte.FAST,
					(byte)SpellByte.XFER,
					(byte)SpellByte.SLO2,
					(byte)SpellByte.SABR,
					(byte)SpellByte.XXXX,
					(byte)SpellByte.SABR,
					(byte)SpellByte.SLO2,
					(byte)SpellByte.WALL,
				},
				SkillChance2 = 0x00,
				Skills2 = new List<byte>
				{
					(byte)EnemySkills.None,
					(byte)EnemySkills.None,
					(byte)EnemySkills.None,
					(byte)EnemySkills.None,
				},
			},
			new AlternateFiends
			{
				Name = "GOGO",
				ElementalWeakness = SpellElement.None,
				MonsterType = MonsterType.GIANT,
				SpellChance1 = 0x40,
				Spells1 = new List<byte>
				{
					(byte)SpellByte.ICE2,
					(byte)SpellByte.INVS,
					(byte)SpellByte.LOCK,
					(byte)SpellByte.FIR2,
					(byte)SpellByte.LAMP,
					(byte)SpellByte.TMPR,
					(byte)SpellByte.SLOW,
					(byte)SpellByte.LIT2,
				},
				SkillChance1 = 0x40,
				Skills1 = new List<byte>
				{
					(byte)EnemySkills.Frost,
					(byte)EnemySkills.Cremate,
					(byte)EnemySkills.Dazzle,
					(byte)EnemySkills.Ink,
				},
				SpellChance2 = 0x40,
				Spells2 = new List<byte>
				{
					(byte)SpellByte.FADE,
					(byte)SpellByte.FAST,
					(byte)SpellByte.SLO2,
					(byte)SpellByte.INVS,
					(byte)SpellByte.RUB,
					(byte)SpellByte.TMPR,
					(byte)SpellByte.CUR3,
					(byte)SpellByte.XFER,
				},
				SkillChance2 = 0x40,
				Skills2 = new List<byte>
				{
					(byte)EnemySkills.Blizzard,
					(byte)EnemySkills.Thunder,
					(byte)EnemySkills.Poison_Damage,
					(byte)EnemySkills.Toxic,
				},
			},
			new AlternateFiends
			{
				Name = "GOLEM",
				ElementalWeakness = SpellElement.None,
				MonsterType = MonsterType.REGENERATIVE,
				SpellChance1 = 0x40,
				Spells1 = new List<byte>
				{
					(byte)SpellByte.INVS,
					(byte)SpellByte.INVS,
					(byte)SpellByte.FOG,
					(byte)SpellByte.TMPR,
					(byte)SpellByte.CUR2,
					(byte)SpellByte.TMPR,
					(byte)SpellByte.MUTE,
					(byte)SpellByte.LOCK,
				},
				SkillChance1 = 0x00,
				Skills1 = new List<byte>
				{
					(byte)EnemySkills.None,
					(byte)EnemySkills.None,
					(byte)EnemySkills.None,
					(byte)EnemySkills.None,
				},
				SpellChance2 = 0x40,
				Spells2 = new List<byte>
				{
					(byte)SpellByte.FAST,
					(byte)SpellByte.CUR3,
					(byte)SpellByte.INVS,
					(byte)SpellByte.WALL,
					(byte)SpellByte.FOG2,
					(byte)SpellByte.SABR,
					(byte)SpellByte.SLOW,
					(byte)SpellByte.CUR3,
				},
				SkillChance2 = 0x00,
				Skills2 = new List<byte>
				{
					(byte)EnemySkills.None,
					(byte)EnemySkills.None,
					(byte)EnemySkills.None,
					(byte)EnemySkills.None,
				},
			},
			new AlternateFiends
			{
				Name = "KARLABOS",
				ElementalWeakness = SpellElement.Lightning,
				MonsterType = MonsterType.AQUATIC,
				SpellChance1 = 0x00,
				Spells1 = new List<byte>
				{
					(byte)SpellByte.NONE,
					(byte)SpellByte.NONE,
					(byte)SpellByte.NONE,
					(byte)SpellByte.NONE,
					(byte)SpellByte.NONE,
					(byte)SpellByte.NONE,
					(byte)SpellByte.NONE,
					(byte)SpellByte.NONE,
				},
				SkillChance1 = 0x40,
				Skills1 = new List<byte>
				{
					(byte)EnemySkills.Dazzle,
					(byte)EnemySkills.Gaze,
					(byte)EnemySkills.Dazzle,
					(byte)EnemySkills.Gaze,
				},
				SpellChance2 = 0x00,
				Spells2 = new List<byte>
				{
					(byte)SpellByte.NONE,
					(byte)SpellByte.NONE,
					(byte)SpellByte.NONE,
					(byte)SpellByte.NONE,
					(byte)SpellByte.NONE,
					(byte)SpellByte.NONE,
					(byte)SpellByte.NONE,
					(byte)SpellByte.NONE,
				},
				SkillChance2 = 0x40,
				Skills2 = new List<byte>
				{
					(byte)EnemySkills.Swirl,
					(byte)EnemySkills.Dazzle,
					(byte)EnemySkills.Nuclear,
					(byte)EnemySkills.Dazzle,
				},
			},
			new AlternateFiends
			{
				Name = "L.FLAME",
				ElementalWeakness = SpellElement.Ice,
				MonsterType = MonsterType.MAGICAL,
				SpellChance1 = 0x40,
				Spells1 = new List<byte>
				{
					(byte)SpellByte.FIR2,
					(byte)SpellByte.FIRE,
					(byte)SpellByte.FIR2,
					(byte)SpellByte.SLOW,
					(byte)SpellByte.FIR2,
					(byte)SpellByte.FIR2,
					(byte)SpellByte.FIRE,
					(byte)SpellByte.STUN,
				},
				SkillChance1 = 0x40,
				Skills1 = new List<byte>
				{
					(byte)EnemySkills.Heat,
					(byte)EnemySkills.Cremate,
					(byte)EnemySkills.Flash,
					(byte)EnemySkills.Flash,
				},
				SpellChance2 = 0x40,
				Spells2 = new List<byte>
				{
					(byte)SpellByte.FIR3,
					(byte)SpellByte.FOG,
					(byte)SpellByte.FIR3,
					(byte)SpellByte.FIR3,
					(byte)SpellByte.SABR,
					(byte)SpellByte.HEL3,
					(byte)SpellByte.FIR3,
					(byte)SpellByte.FIR3,
				},
				SkillChance2 = 0x40,
				Skills2 = new List<byte>
				{
					(byte)EnemySkills.Inferno,
					(byte)EnemySkills.Nuclear,
					(byte)EnemySkills.Toxic,
					(byte)EnemySkills.Blaze,
				},
			},
			new AlternateFiends
			{
				Name = "MAGISSA",
				ElementalWeakness = SpellElement.None,
				MonsterType = MonsterType.MAGE,
				SpellChance1 = 0x40,
				Spells1 = new List<byte>
				{
					(byte)SpellByte.CUR2,
					(byte)SpellByte.FIR2,
					(byte)SpellByte.ICE2,
					(byte)SpellByte.LIT2,
					(byte)SpellByte.FIR2,
					(byte)SpellByte.CUR2,
					(byte)SpellByte.LIT2,
					(byte)SpellByte.ICE2,
				},
				SkillChance1 = 0x40,
				Skills1 = new List<byte>
				{
					(byte)EnemySkills.Stinger,
					(byte)EnemySkills.Stinger,
					(byte)EnemySkills.Stinger,
					(byte)EnemySkills.Stinger,
				},
				SpellChance2 = 0x40,
				Spells2 = new List<byte>
				{
					(byte)SpellByte.FIR3,
					(byte)SpellByte.ICE3,
					(byte)SpellByte.CUR4,
					(byte)SpellByte.ICE3,
					(byte)SpellByte.INVS,
					(byte)SpellByte.HEL3,
					(byte)SpellByte.NUKE,
					(byte)SpellByte.LIT3,
				},
				SkillChance2 = 0x00,
				Skills2 = new List<byte>
				{
					(byte)EnemySkills.None,
					(byte)EnemySkills.None,
					(byte)EnemySkills.None,
					(byte)EnemySkills.None,
				},
			},
			new AlternateFiends
			{
				Name = "MELUSINE",
				ElementalWeakness = SpellElement.Poison,
				MonsterType = MonsterType.MAGE,
				SpellChance1 = 0x40,
				Spells1 = new List<byte>
				{
					(byte)SpellByte.LIT2,
					(byte)SpellByte.MUTE,
					(byte)SpellByte.ICE2,
					(byte)SpellByte.SLOW,
					(byte)SpellByte.FIR2,
					(byte)SpellByte.INVS,
					(byte)SpellByte.STUN,
					(byte)SpellByte.FOG,
				},
				SkillChance1 = 0x00,
				Skills1 = new List<byte>
				{
					(byte)EnemySkills.None,
					(byte)EnemySkills.None,
					(byte)EnemySkills.None,
					(byte)EnemySkills.None,
				},
				SpellChance2 = 0x40,
				Spells2 = new List<byte>
				{
					(byte)SpellByte.FIR3,
					(byte)SpellByte.SLO2,
					(byte)SpellByte.LIT3,
					(byte)SpellByte.WALL,
					(byte)SpellByte.ICE3,
					(byte)SpellByte.MUTE,
					(byte)SpellByte.XXXX,
					(byte)SpellByte.BANE,
				},
				SkillChance2 = 0x00,
				Skills2 = new List<byte>
				{
					(byte)EnemySkills.None,
					(byte)EnemySkills.None,
					(byte)EnemySkills.None,
					(byte)EnemySkills.None,
				},
			},
			new AlternateFiends
			{
				Name = "MINOS",
				ElementalWeakness = SpellElement.None,
				MonsterType = MonsterType.GIANT,
				SpellChance1 = 0x00,
				Spells1 = new List<byte>
				{
					(byte)SpellByte.NONE,
					(byte)SpellByte.NONE,
					(byte)SpellByte.NONE,
					(byte)SpellByte.NONE,
					(byte)SpellByte.NONE,
					(byte)SpellByte.NONE,
					(byte)SpellByte.NONE,
					(byte)SpellByte.NONE,
				},
				SkillChance1 = 0x40,
				Skills1 = new List<byte>
				{
					(byte)EnemySkills.Snorting,
					(byte)EnemySkills.Stare,
					(byte)EnemySkills.Dazzle,
					(byte)EnemySkills.Trance,
				},
				SpellChance2 = 0x00,
				Spells2 = new List<byte>
				{
					(byte)SpellByte.NONE,
					(byte)SpellByte.NONE,
					(byte)SpellByte.NONE,
					(byte)SpellByte.NONE,
					(byte)SpellByte.NONE,
					(byte)SpellByte.NONE,
					(byte)SpellByte.NONE,
					(byte)SpellByte.NONE,
				},
				SkillChance2 = 0x40,
				Skills2 = new List<byte>
				{
					(byte)EnemySkills.Crack,
					(byte)EnemySkills.Toxic,
					(byte)EnemySkills.Poison_Damage,
					(byte)EnemySkills.Poison_Stone,
				},
			},
			new AlternateFiends
			{
				Name = "NCROPHBE",
				ElementalWeakness = SpellElement.All,
				MonsterType = MonsterType.GIANT,
				SpellChance1 = 0x40,
				Spells1 = new List<byte>
				{
					(byte)SpellByte.FIR2,
					(byte)SpellByte.AFIR,
					(byte)SpellByte.ICE2,
					(byte)SpellByte.AICE,
					(byte)SpellByte.LIT2,
					(byte)SpellByte.ALIT,
					(byte)SpellByte.RUB,
					(byte)SpellByte.HEL2,
				},
				SkillChance1 = 0x40,
				Skills1 = new List<byte>
				{
					(byte)EnemySkills.Cremate,
					(byte)EnemySkills.Frost,
					(byte)EnemySkills.Dazzle,
					(byte)EnemySkills.Trance,
				},
				SpellChance2 = 0x40,
				Spells2 = new List<byte>
				{
					(byte)SpellByte.FIR3,
					(byte)SpellByte.WALL,
					(byte)SpellByte.XXXX,
					(byte)SpellByte.NUKE,
					(byte)SpellByte.SLO2,
					(byte)SpellByte.ICE3,
					(byte)SpellByte.MUTE,
					(byte)SpellByte.ZAP,
				},
				SkillChance2 = 0x40,
				Skills2 = new List<byte>
				{
					(byte)EnemySkills.Poison_Stone,
					(byte)EnemySkills.Swirl,
					(byte)EnemySkills.Inferno,
					(byte)EnemySkills.Stare,
				},
			},
			new AlternateFiends
			{
				Name = "OMEGA",
				ElementalWeakness = SpellElement.None,
				MonsterType = MonsterType.REGENERATIVE,
				SpellChance1 = 0x40,
				Spells1 = new List<byte>
				{
					(byte)SpellByte.HOLD,
					(byte)SpellByte.QAKE,
					(byte)SpellByte.FIR2,
					(byte)SpellByte.ICE2,
					(byte)SpellByte.INVS,
					(byte)SpellByte.SLOW,
					(byte)SpellByte.LIT2,
					(byte)SpellByte.CUR2,
				},
				SkillChance1 = 0x40,
				Skills1 = new List<byte>
				{
					(byte)EnemySkills.Stare,
					(byte)EnemySkills.Cremate,
					(byte)EnemySkills.Dazzle,
					(byte)EnemySkills.Stinger,
				},
				SpellChance2 = 0x40,
				Spells2 = new List<byte>
				{
					(byte)SpellByte.NUKE,
					(byte)SpellByte.XXXX,
					(byte)SpellByte.LOK2,
					(byte)SpellByte.ICE3,
					(byte)SpellByte.CUR3,
					(byte)SpellByte.FAST,
					(byte)SpellByte.SLO2,
					(byte)SpellByte.ICE3,
				},
				SkillChance2 = 0x40,
				Skills2 = new List<byte>
				{
					(byte)EnemySkills.Thunder,
					(byte)EnemySkills.Blaze,
					(byte)EnemySkills.Nuclear,
					(byte)EnemySkills.Crack,
				},
			},
			new AlternateFiends
			{
				Name = "OMNISENT",
				ElementalWeakness = SpellElement.Earth,
				MonsterType = MonsterType.MAGE,
				SpellChance1 = 0x40,
				Spells1 = new List<byte>
				{
					(byte)SpellByte.MUTE,
					(byte)SpellByte.FIR2,
					(byte)SpellByte.LIT2,
					(byte)SpellByte.STUN,
					(byte)SpellByte.ICE2,
					(byte)SpellByte.MUTE,
					(byte)SpellByte.ZAP,
					(byte)SpellByte.FOG,
				},
				SkillChance1 = 0x00,
				Skills1 = new List<byte>
				{
					(byte)EnemySkills.None,
					(byte)EnemySkills.None,
					(byte)EnemySkills.None,
					(byte)EnemySkills.None,
				},
				SpellChance2 = 0x40,
				Spells2 = new List<byte>
				{
					(byte)SpellByte.INVS,
					(byte)SpellByte.FADE,
					(byte)SpellByte.CUR3,
					(byte)SpellByte.LIT3,
					(byte)SpellByte.XXXX,
					(byte)SpellByte.STOP,
					(byte)SpellByte.INVS,
					(byte)SpellByte.NUKE,
				},
				SkillChance2 = 0x00,
				Skills2 = new List<byte>
				{
					(byte)EnemySkills.None,
					(byte)EnemySkills.None,
					(byte)EnemySkills.None,
					(byte)EnemySkills.None,
				},
			},
			new AlternateFiends
			{
				Name = "SHINRYU",
				ElementalWeakness = SpellElement.None,
				MonsterType = MonsterType.DRAGON,
				SpellChance1 = 0x40,
				Spells1 = new List<byte>
				{
					(byte)SpellByte.INVS,
					(byte)SpellByte.ZAP,
					(byte)SpellByte.ICE2,
					(byte)SpellByte.ICE2,
					(byte)SpellByte.LIT2,
					(byte)SpellByte.FAST,
					(byte)SpellByte.SLOW,
					(byte)SpellByte.LIT2,
				},
				SkillChance1 = 0x40,
				Skills1 = new List<byte>
				{
					(byte)EnemySkills.Cremate,
					(byte)EnemySkills.Frost,
					(byte)EnemySkills.Gaze,
					(byte)EnemySkills.Trance,
				},
				SpellChance2 = 0x40,
				Spells2 = new List<byte>
				{
					(byte)SpellByte.QAKE,
					(byte)SpellByte.FADE,
					(byte)SpellByte.CUR4,
					(byte)SpellByte.FAST,
					(byte)SpellByte.SLO2,
					(byte)SpellByte.SABR,
					(byte)SpellByte.NUKE,
					(byte)SpellByte.XXXX,
				},
				SkillChance2 = 0x40,
				Skills2 = new List<byte>
				{
					(byte)EnemySkills.Toxic,
					(byte)EnemySkills.Blizzard,
					(byte)EnemySkills.Swirl,
					(byte)EnemySkills.Crack,
				},
			},
			new AlternateFiends
			{
				Name = "SIREN",
				ElementalWeakness = SpellElement.Fire,
				MonsterType = MonsterType.UNDEAD,
				SpellChance1 = 0x00,
				Spells1 = new List<byte>
				{
					(byte)SpellByte.NONE,
					(byte)SpellByte.NONE,
					(byte)SpellByte.NONE,
					(byte)SpellByte.NONE,
					(byte)SpellByte.NONE,
					(byte)SpellByte.NONE,
					(byte)SpellByte.NONE,
					(byte)SpellByte.NONE,
				},
				SkillChance1 = 0x40,
				Skills1 = new List<byte>
				{
					(byte)EnemySkills.Stinger,
					(byte)EnemySkills.Trance,
					(byte)EnemySkills.Frost,
					(byte)EnemySkills.Glance,
				},
				SpellChance2 = 0x00,
				Spells2 = new List<byte>
				{
					(byte)SpellByte.NONE,
					(byte)SpellByte.NONE,
					(byte)SpellByte.NONE,
					(byte)SpellByte.NONE,
					(byte)SpellByte.NONE,
					(byte)SpellByte.NONE,
					(byte)SpellByte.NONE,
					(byte)SpellByte.NONE,
				},
				SkillChance2 = 0x40,
				Skills2 = new List<byte>
				{
					(byte)EnemySkills.Toxic,
					(byte)EnemySkills.Poison_Stone,
					(byte)EnemySkills.Crack,
					(byte)EnemySkills.Squint,
				},
			},
			new AlternateFiends
			{
				Name = "S.CANNON",
				ElementalWeakness = SpellElement.Lightning,
				MonsterType = MonsterType.REGENERATIVE,
				SpellChance1 = 0x00,
				Spells1 = new List<byte>
				{
					(byte)SpellByte.NONE,
					(byte)SpellByte.NONE,
					(byte)SpellByte.NONE,
					(byte)SpellByte.NONE,
					(byte)SpellByte.NONE,
					(byte)SpellByte.NONE,
					(byte)SpellByte.NONE,
					(byte)SpellByte.NONE,
				},
				SkillChance1 = 0x40,
				Skills1 = new List<byte>
				{
					(byte)EnemySkills.Cremate,
					(byte)EnemySkills.Crack,
					(byte)EnemySkills.Glare,
					(byte)EnemySkills.Trance,
				},
				SpellChance2 = 0x00,
				Spells2 = new List<byte>
				{
					(byte)SpellByte.NONE,
					(byte)SpellByte.NONE,
					(byte)SpellByte.NONE,
					(byte)SpellByte.NONE,
					(byte)SpellByte.NONE,
					(byte)SpellByte.NONE,
					(byte)SpellByte.NONE,
					(byte)SpellByte.NONE,
				},
				SkillChance2 = 0x40,
				Skills2 = new List<byte>
				{
					(byte)EnemySkills.Nuclear,
					(byte)EnemySkills.Poison_Damage,
					(byte)EnemySkills.Tornado,
					(byte)EnemySkills.Thunder,
				},
			},
			new AlternateFiends
			{
				Name = "TWINTANI",
				ElementalWeakness = SpellElement.Ice,
				MonsterType = MonsterType.DRAGON,
				SpellChance1 = 0x40,
				Spells1 = new List<byte>
				{
					(byte)SpellByte.INVS,
					(byte)SpellByte.FIR2,
					(byte)SpellByte.FIR2,
					(byte)SpellByte.HEL2,
					(byte)SpellByte.MUTE,
					(byte)SpellByte.LIT2,
					(byte)SpellByte.LIT2,
					(byte)SpellByte.SLOW,
				},
				SkillChance1 = 0x40,
				Skills1 = new List<byte>
				{
					(byte)EnemySkills.Trance,
					(byte)EnemySkills.Stinger,
					(byte)EnemySkills.Snorting,
					(byte)EnemySkills.Heat,
				},
				SpellChance2 = 0x40,
				Spells2 = new List<byte>
				{
					(byte)SpellByte.FAST,
					(byte)SpellByte.LIT3,
					(byte)SpellByte.LOCK,
					(byte)SpellByte.LOCK,
					(byte)SpellByte.NUKE,
					(byte)SpellByte.LIT3,
					(byte)SpellByte.SLO2,
					(byte)SpellByte.XXXX,
				},
				SkillChance2 = 0x40,
				Skills2 = new List<byte>
				{
					(byte)EnemySkills.Swirl,
					(byte)EnemySkills.Poison_Stone,
					(byte)EnemySkills.Crack,
					(byte)EnemySkills.Blaze,
				},
			},
			new AlternateFiends
			{
				Name = "W.RAPTOR",
				ElementalWeakness = SpellElement.Lightning,
				MonsterType = MonsterType.GIANT,
				SpellChance1 = 0x00,
				Spells1 = new List<byte>
				{
					(byte)SpellByte.NONE,
					(byte)SpellByte.NONE,
					(byte)SpellByte.NONE,
					(byte)SpellByte.NONE,
					(byte)SpellByte.NONE,
					(byte)SpellByte.NONE,
					(byte)SpellByte.NONE,
					(byte)SpellByte.NONE,
				},
				SkillChance1 = 0x40,
				Skills1 = new List<byte>
				{
					(byte)EnemySkills.Stare,
					(byte)EnemySkills.Glare,
					(byte)EnemySkills.Squint,
					(byte)EnemySkills.Trance,
				},
				SpellChance2 = 0x00,
				Spells2 = new List<byte>
				{
					(byte)SpellByte.NONE,
					(byte)SpellByte.NONE,
					(byte)SpellByte.NONE,
					(byte)SpellByte.NONE,
					(byte)SpellByte.NONE,
					(byte)SpellByte.NONE,
					(byte)SpellByte.NONE,
					(byte)SpellByte.NONE,
				},
				SkillChance2 = 0x40,
				Skills2 = new List<byte>
				{
					(byte)EnemySkills.Tornado,
					(byte)EnemySkills.Poison_Damage,
					(byte)EnemySkills.Toxic,
					(byte)EnemySkills.Tornado,
				},
			},
		};
	}
}
