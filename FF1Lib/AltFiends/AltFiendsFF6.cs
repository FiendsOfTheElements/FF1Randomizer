namespace FF1Lib
{
	public partial class FF1Rom
	{
		List<AlternateFiends> FF6AltFiendsList = new()
		{
			new AlternateFiends
			{
				Name = "A.WORM",
				ElementalWeakness = SpellElement.None,
				MonsterType = MonsterType.NONE,
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
					(byte)EnemySkills.Stinger,
					(byte)EnemySkills.Trance,
					(byte)EnemySkills.Dazzle,
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
					(byte)EnemySkills.Crack,
					(byte)EnemySkills.Glance,
				},
			},
			new AlternateFiends
			{
				Name = "ATMA",
				ElementalWeakness = SpellElement.None,
				MonsterType = MonsterType.MAGICAL,
				SpellChance1 = 0x40,
				Spells1 = new List<byte>
				{
					(byte)SpellByte.FIRE,
					(byte)SpellByte.STUN,
					(byte)SpellByte.ICE2,
					(byte)SpellByte.SLOW,
					(byte)SpellByte.LIT,
					(byte)SpellByte.MUTE,
					(byte)SpellByte.ZAP,
					(byte)SpellByte.ICE2,
				},
				SkillChance1 = 0x40,
				Skills1 = new List<byte>
				{
					(byte)EnemySkills.Frost,
					(byte)EnemySkills.Heat,
					(byte)EnemySkills.Trance,
					(byte)EnemySkills.Stinger,
				},
				SpellChance2 = 0x40,
				Spells2 = new List<byte>
				{
					(byte)SpellByte.FIR3,
					(byte)SpellByte.QAKE,
					(byte)SpellByte.ICE3,
					(byte)SpellByte.SLO2,
					(byte)SpellByte.FOG,
					(byte)SpellByte.XXXX,
					(byte)SpellByte.FADE,
					(byte)SpellByte.WALL,
				},
				SkillChance2 = 0x40,
				Skills2 = new List<byte>
				{
					(byte)EnemySkills.Blizzard,
					(byte)EnemySkills.Inferno,
					(byte)EnemySkills.Poison_Stone,
					(byte)EnemySkills.Nuclear,
				},
			},
			new AlternateFiends
			{
				Name = "B.DRAGON",
				ElementalWeakness = SpellElement.Lightning,
				MonsterType = MonsterType.DRAGON,
				SpellChance1 = 0x40,
				Spells1 = new List<byte>
				{
					(byte)SpellByte.ICE2,
					(byte)SpellByte.SLOW,
					(byte)SpellByte.DARK,
					(byte)SpellByte.ICE2,
					(byte)SpellByte.MUTE,
					(byte)SpellByte.ICE2,
					(byte)SpellByte.SLOW,
					(byte)SpellByte.ICE2,
				},
				SkillChance1 = 0x40,
				Skills1 = new List<byte>
				{
					(byte)EnemySkills.Stinger,
					(byte)EnemySkills.Frost,
					(byte)EnemySkills.Trance,
					(byte)EnemySkills.Frost,
				},
				SpellChance2 = 0x40,
				Spells2 = new List<byte>
				{
					(byte)SpellByte.ICE3,
					(byte)SpellByte.ICE2,
					(byte)SpellByte.SLO2,
					(byte)SpellByte.STOP,
					(byte)SpellByte.XXXX,
					(byte)SpellByte.MUTE,
					(byte)SpellByte.LOCK,
					(byte)SpellByte.ICE3,
				},
				SkillChance2 = 0x40,
				Skills2 = new List<byte>
				{
					(byte)EnemySkills.Poison_Damage,
					(byte)EnemySkills.Blizzard,
					(byte)EnemySkills.Toxic,
					(byte)EnemySkills.Swirl,
				},
			},
			new AlternateFiends
			{
				Name = "CHDRNOOK",
				ElementalWeakness = SpellElement.Fire,
				MonsterType = MonsterType.UNDEAD,
				SpellChance1 = 0x40,
				Spells1 = new List<byte>
				{
					(byte)SpellByte.LIT2,
					(byte)SpellByte.LIT,
					(byte)SpellByte.FOG,
					(byte)SpellByte.CUR2,
					(byte)SpellByte.SLEP,
					(byte)SpellByte.FOG,
					(byte)SpellByte.LIT2,
					(byte)SpellByte.LIT,
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
					(byte)SpellByte.LIT3,
					(byte)SpellByte.FAST,
					(byte)SpellByte.LIT2,
					(byte)SpellByte.SLP2,
					(byte)SpellByte.LIT3,
					(byte)SpellByte.CUR3,
					(byte)SpellByte.LIT2,
					(byte)SpellByte.RUB,
				},
				SkillChance2 = 0x40,
				Skills2 = new List<byte>
				{
					(byte)EnemySkills.Thunder,
					(byte)EnemySkills.Squint,
					(byte)EnemySkills.Glare,
					(byte)EnemySkills.Thunder,
				},
			},
			new AlternateFiends
			{
				Name = "CRANE",
				ElementalWeakness = SpellElement.Ice,
				MonsterType = MonsterType.NONE,
				SpellChance1 = 0x40,
				Spells1 = new List<byte>
				{
					(byte)SpellByte.LIT2,
					(byte)SpellByte.FOG,
					(byte)SpellByte.LIT,
					(byte)SpellByte.INVS,
					(byte)SpellByte.LIT2,
					(byte)SpellByte.LIT,
					(byte)SpellByte.FOG,
					(byte)SpellByte.LIT,
				},
				SkillChance1 = 0x40,
				Skills1 = new List<byte>
				{
					(byte)EnemySkills.Trance,
					(byte)EnemySkills.Gaze,
					(byte)EnemySkills.Trance,
					(byte)EnemySkills.Gaze,
				},
				SpellChance2 = 0x40,
				Spells2 = new List<byte>
				{
					(byte)SpellByte.FOG2,
					(byte)SpellByte.LIT3,
					(byte)SpellByte.WALL,
					(byte)SpellByte.LIT2,
					(byte)SpellByte.RUB,
					(byte)SpellByte.LOK2,
					(byte)SpellByte.STOP,
					(byte)SpellByte.LIT3,
				},
				SkillChance2 = 0x40,
				Skills2 = new List<byte>
				{
					(byte)EnemySkills.Dazzle,
					(byte)EnemySkills.Thunder,
					(byte)EnemySkills.Toxic,
					(byte)EnemySkills.Tornado,
				},
			},
			new AlternateFiends
			{
				Name = "DADALUMA",
				ElementalWeakness = SpellElement.Poison,
				MonsterType = MonsterType.GIANT,
				SpellChance1 = 0x40,
				Spells1 = new List<byte>
				{
					(byte)SpellByte.CURE,
					(byte)SpellByte.FOG,
					(byte)SpellByte.HEL2,
					(byte)SpellByte.FOG,
					(byte)SpellByte.CURE,
					(byte)SpellByte.INVS,
					(byte)SpellByte.HEL2,
					(byte)SpellByte.TMPR,
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
					(byte)SpellByte.CUR3,
					(byte)SpellByte.FAST,
					(byte)SpellByte.FOG2,
					(byte)SpellByte.MUTE,
					(byte)SpellByte.SABR,
					(byte)SpellByte.HEL3,
					(byte)SpellByte.CUR3,
					(byte)SpellByte.FOG2,
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
				Name = "DOOMGAZE",
				ElementalWeakness = SpellElement.Fire,
				MonsterType = MonsterType.UNDEAD,
				SpellChance1 = 0x40,
				Spells1 = new List<byte>
				{
					(byte)SpellByte.ICE2,
					(byte)SpellByte.RUB,
					(byte)SpellByte.FOG,
					(byte)SpellByte.ICE2,
					(byte)SpellByte.ZAP,
					(byte)SpellByte.FOG,
					(byte)SpellByte.MUTE,
					(byte)SpellByte.ICE2,
				},
				SkillChance1 = 0x40,
				Skills1 = new List<byte>
				{
					(byte)EnemySkills.Gaze,
					(byte)EnemySkills.Stare,
					(byte)EnemySkills.Stare,
					(byte)EnemySkills.Gaze,
				},
				SpellChance2 = 0x40,
				Spells2 = new List<byte>
				{
					(byte)SpellByte.QAKE,
					(byte)SpellByte.ICE3,
					(byte)SpellByte.XXXX,
					(byte)SpellByte.ZAP,
					(byte)SpellByte.WALL,
					(byte)SpellByte.ICE3,
					(byte)SpellByte.BRAK,
					(byte)SpellByte.BANE,
				},
				SkillChance2 = 0x40,
				Skills2 = new List<byte>
				{
					(byte)EnemySkills.Toxic,
					(byte)EnemySkills.Squint,
					(byte)EnemySkills.Poison_Stone,
					(byte)EnemySkills.Tornado,
				},
			},
			new AlternateFiends
			{
				Name = "DULLAHAN",
				ElementalWeakness = SpellElement.Fire,
				MonsterType = MonsterType.MAGE,
				SpellChance1 = 0x40,
				Spells1 = new List<byte>
				{
					(byte)SpellByte.SLEP,
					(byte)SpellByte.ICE2,
					(byte)SpellByte.ICE,
					(byte)SpellByte.MUTE,
					(byte)SpellByte.ICE2,
					(byte)SpellByte.SLOW,
					(byte)SpellByte.ICE,
					(byte)SpellByte.RUB,
				},
				SkillChance1 = 0x40,
				Skills1 = new List<byte>
				{
					(byte)EnemySkills.Stinger,
					(byte)EnemySkills.Frost,
					(byte)EnemySkills.Dazzle,
					(byte)EnemySkills.Frost,
				},
				SpellChance2 = 0x40,
				Spells2 = new List<byte>
				{
					(byte)SpellByte.ICE3,
					(byte)SpellByte.WALL,
					(byte)SpellByte.FADE,
					(byte)SpellByte.CUR4,
					(byte)SpellByte.ICE3,
					(byte)SpellByte.SLO2,
					(byte)SpellByte.ICE3,
					(byte)SpellByte.BRAK,
				},
				SkillChance2 = 0x40,
				Skills2 = new List<byte>
				{
					(byte)EnemySkills.Blizzard,
					(byte)EnemySkills.Poison_Damage,
					(byte)EnemySkills.Toxic,
					(byte)EnemySkills.Blizzard,
				},
			},
			new AlternateFiends
			{
				Name = "E.DRAGON",
				ElementalWeakness = SpellElement.Ice,
				MonsterType = MonsterType.DRAGON,
				SpellChance1 = 0x40,
				Spells1 = new List<byte>
				{
					(byte)SpellByte.QAKE,
					(byte)SpellByte.LIT2,
					(byte)SpellByte.XFER,
					(byte)SpellByte.LIT2,
					(byte)SpellByte.SLOW,
					(byte)SpellByte.LIT2,
					(byte)SpellByte.QAKE,
					(byte)SpellByte.XFER,
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
					(byte)SpellByte.QAKE,
					(byte)SpellByte.XXXX,
					(byte)SpellByte.BANE,
					(byte)SpellByte.LIT3,
					(byte)SpellByte.BRAK,
					(byte)SpellByte.LIT3,
					(byte)SpellByte.QAKE,
				},
				SkillChance2 = 0x40,
				Skills2 = new List<byte>
				{
					(byte)EnemySkills.Crack,
					(byte)EnemySkills.Thunder,
					(byte)EnemySkills.Crack,
					(byte)EnemySkills.Gaze,
				},
			},
			new AlternateFiends
			{
				Name = "F.EATER",
				ElementalWeakness = SpellElement.Ice,
				MonsterType = MonsterType.MAGICAL,
				SpellChance1 = 0x40,
				Spells1 = new List<byte>
				{
					(byte)SpellByte.FIRE,
					(byte)SpellByte.FIR2,
					(byte)SpellByte.FOG,
					(byte)SpellByte.FIR2,
					(byte)SpellByte.HEL2,
					(byte)SpellByte.FIRE,
					(byte)SpellByte.FIR2,
					(byte)SpellByte.FOG,
				},
				SkillChance1 = 0x40,
				Skills1 = new List<byte>
				{
					(byte)EnemySkills.Heat,
					(byte)EnemySkills.Heat,
					(byte)EnemySkills.Cremate,
					(byte)EnemySkills.Cremate,
				},
				SpellChance2 = 0x40,
				Spells2 = new List<byte>
				{
					(byte)SpellByte.FIR3,
					(byte)SpellByte.FOG2,
					(byte)SpellByte.FIR3,
					(byte)SpellByte.FIR2,
					(byte)SpellByte.FOG2,
					(byte)SpellByte.FIR3,
					(byte)SpellByte.HEL3,
					(byte)SpellByte.NUKE,
				},
				SkillChance2 = 0x40,
				Skills2 = new List<byte>
				{
					(byte)EnemySkills.Inferno,
					(byte)EnemySkills.Tornado,
					(byte)EnemySkills.Inferno,
					(byte)EnemySkills.Tornado,
				},
			},
			new AlternateFiends
			{
				Name = "FIEND",
				ElementalWeakness = SpellElement.None,
				MonsterType = MonsterType.REGENERATIVE,
				SpellChance1 = 0x40,
				Spells1 = new List<byte>
				{
					(byte)SpellByte.ICE,
					(byte)SpellByte.INVS,
					(byte)SpellByte.ICE2,
					(byte)SpellByte.TMPR,
					(byte)SpellByte.ICE,
					(byte)SpellByte.FOG,
					(byte)SpellByte.ICE2,
					(byte)SpellByte.STUN,
				},
				SkillChance1 = 0x40,
				Skills1 = new List<byte>
				{
					(byte)EnemySkills.Frost,
					(byte)EnemySkills.Dazzle,
					(byte)EnemySkills.Trance,
					(byte)EnemySkills.Frost,
				},
				SpellChance2 = 0x40,
				Spells2 = new List<byte>
				{
					(byte)SpellByte.ICE3,
					(byte)SpellByte.FAST,
					(byte)SpellByte.ICE3,
					(byte)SpellByte.SABR,
					(byte)SpellByte.ICE2,
					(byte)SpellByte.ICE3,
					(byte)SpellByte.FOG2,
					(byte)SpellByte.ICE3,
				},
				SkillChance2 = 0x40,
				Skills2 = new List<byte>
				{
					(byte)EnemySkills.Blizzard,
					(byte)EnemySkills.Nuclear,
					(byte)EnemySkills.Toxic,
					(byte)EnemySkills.Poison_Damage,
				},
			},
			new AlternateFiends
			{
				Name = "FLANPRIN",
				ElementalWeakness = SpellElement.Fire,
				MonsterType = MonsterType.NONE,
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
					(byte)EnemySkills.Trance,
					(byte)EnemySkills.Dazzle,
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
					(byte)EnemySkills.Poison_Stone,
					(byte)EnemySkills.Toxic,
					(byte)EnemySkills.Glance,
					(byte)EnemySkills.Squint,
				},
			},
			new AlternateFiends
			{
				Name = "G.DRAGON",
				ElementalWeakness = SpellElement.Ice,
				MonsterType = MonsterType.DRAGON,
				SpellChance1 = 0x40,
				Spells1 = new List<byte>
				{
					(byte)SpellByte.INVS,
					(byte)SpellByte.LIT,
					(byte)SpellByte.LIT2,
					(byte)SpellByte.SLEP,
					(byte)SpellByte.LIT2,
					(byte)SpellByte.LIT,
					(byte)SpellByte.STUN,
					(byte)SpellByte.LIT2,
				},
				SkillChance1 = 0x40,
				Skills1 = new List<byte>
				{
					(byte)EnemySkills.Snorting,
					(byte)EnemySkills.Trance,
					(byte)EnemySkills.Flash,
					(byte)EnemySkills.Stare,
				},
				SpellChance2 = 0x40,
				Spells2 = new List<byte>
				{
					(byte)SpellByte.LIT3,
					(byte)SpellByte.WALL,
					(byte)SpellByte.LIT2,
					(byte)SpellByte.FAST,
					(byte)SpellByte.SLOW,
					(byte)SpellByte.XXXX,
					(byte)SpellByte.LIT3,
					(byte)SpellByte.CUR3,
				},
				SkillChance2 = 0x40,
				Skills2 = new List<byte>
				{
					(byte)EnemySkills.Thunder,
					(byte)EnemySkills.Poison_Damage,
					(byte)EnemySkills.Thunder,
					(byte)EnemySkills.Squint,
				},
			},
			new AlternateFiends
			{
				Name = "GARGANT",
				ElementalWeakness = SpellElement.Poison,
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
					(byte)EnemySkills.Flash,
					(byte)EnemySkills.Stare,
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
					(byte)EnemySkills.Crack,
					(byte)EnemySkills.Tornado,
				},
			},
			new AlternateFiends
			{
				Name = "GODDESS",
				ElementalWeakness = SpellElement.None,
				MonsterType = MonsterType.REGENERATIVE,
				SpellChance1 = 0x40,
				Spells1 = new List<byte>
				{
					(byte)SpellByte.SLEP,
					(byte)SpellByte.LIT2,
					(byte)SpellByte.FOG,
					(byte)SpellByte.LIT,
					(byte)SpellByte.INVS,
					(byte)SpellByte.LIT2,
					(byte)SpellByte.SLEP,
					(byte)SpellByte.LIT,
				},
				SkillChance1 = 0x40,
				Skills1 = new List<byte>
				{
					(byte)EnemySkills.Dazzle,
					(byte)EnemySkills.Trance,
					(byte)EnemySkills.Flash,
					(byte)EnemySkills.Stare,
				},
				SpellChance2 = 0x40,
				Spells2 = new List<byte>
				{
					(byte)SpellByte.SLP2,
					(byte)SpellByte.LIT3,
					(byte)SpellByte.FAST,
					(byte)SpellByte.LIT3,
					(byte)SpellByte.BANE,
					(byte)SpellByte.CUR3,
					(byte)SpellByte.SLO2,
					(byte)SpellByte.LOCK,
				},
				SkillChance2 = 0x40,
				Skills2 = new List<byte>
				{
					(byte)EnemySkills.Toxic,
					(byte)EnemySkills.Thunder,
					(byte)EnemySkills.Crack,
					(byte)EnemySkills.Poison_Stone,
				},
			},
			new AlternateFiends
			{
				Name = "H.DRAGON",
				ElementalWeakness = SpellElement.None,
				MonsterType = MonsterType.DRAGON,
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
				SpellChance2 = 0x40,
				Spells2 = new List<byte>
				{
					(byte)SpellByte.FADE,
					(byte)SpellByte.XFER,
					(byte)SpellByte.FAST,
					(byte)SpellByte.XFER,
					(byte)SpellByte.FADE,
					(byte)SpellByte.SLO2,
					(byte)SpellByte.MUTE,
					(byte)SpellByte.SLP2,
				},
				SkillChance2 = 0x40,
				Skills2 = new List<byte>
				{
					(byte)EnemySkills.Trance,
					(byte)EnemySkills.Squint,
					(byte)EnemySkills.Poison_Stone,
					(byte)EnemySkills.Tornado,
				},
			},
			new AlternateFiends
			{
				Name = "H.RIDER",
				ElementalWeakness = SpellElement.Time,
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
					(byte)EnemySkills.Stinger,
					(byte)EnemySkills.Glance,
					(byte)EnemySkills.Stinger,
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
					(byte)EnemySkills.Crack,
					(byte)EnemySkills.Tornado,
					(byte)EnemySkills.Squint,
					(byte)EnemySkills.Toxic,
				},
			},
			new AlternateFiends
			{
				Name = "I.DRAGON",
				ElementalWeakness = SpellElement.Fire,
				MonsterType = MonsterType.DRAGON,
				SpellChance1 = 0x40,
				Spells1 = new List<byte>
				{
					(byte)SpellByte.ICE2,
					(byte)SpellByte.STUN,
					(byte)SpellByte.ICE,
					(byte)SpellByte.HOLD,
					(byte)SpellByte.ICE2,
					(byte)SpellByte.STUN,
					(byte)SpellByte.ICE,
					(byte)SpellByte.HOLD,
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
					(byte)SpellByte.ICE3,
					(byte)SpellByte.STOP,
					(byte)SpellByte.ICE2,
					(byte)SpellByte.XFER,
					(byte)SpellByte.ICE3,
					(byte)SpellByte.STOP,
					(byte)SpellByte.ICE3,
					(byte)SpellByte.SLOW,
				},
				SkillChance2 = 0x40,
				Skills2 = new List<byte>
				{
					(byte)EnemySkills.Blizzard,
					(byte)EnemySkills.Trance,
					(byte)EnemySkills.Frost,
					(byte)EnemySkills.Blizzard,
				},
			},
			new AlternateFiends
			{
				Name = "IPOOH",
				ElementalWeakness = SpellElement.Fire,
				MonsterType = MonsterType.NONE,
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
				Name = "K.BEHEM",
				ElementalWeakness = SpellElement.Fire,
				MonsterType = MonsterType.REGENERATIVE,
				SpellChance1 = 0x40,
				Spells1 = new List<byte>
				{
					(byte)SpellByte.ICE,
					(byte)SpellByte.FOG,
					(byte)SpellByte.ICE2,
					(byte)SpellByte.ICE,
					(byte)SpellByte.SLOW,
					(byte)SpellByte.SLEP,
					(byte)SpellByte.CUR2,
					(byte)SpellByte.ICE2,
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
					(byte)SpellByte.ICE3,
					(byte)SpellByte.FAST,
					(byte)SpellByte.WALL,
					(byte)SpellByte.SLO2,
					(byte)SpellByte.ICE3,
					(byte)SpellByte.FOG,
					(byte)SpellByte.FADE,
					(byte)SpellByte.FAST,
				},
				SkillChance2 = 0x40,
				Skills2 = new List<byte>
				{
					(byte)EnemySkills.Stare,
					(byte)EnemySkills.Blizzard,
					(byte)EnemySkills.Poison_Stone,
					(byte)EnemySkills.Squint,
				},
			},
			new AlternateFiends
			{
				Name = "K.DRAGON",
				ElementalWeakness = SpellElement.None,
				MonsterType = MonsterType.DRAGON,
				SpellChance1 = 0x40,
				Spells1 = new List<byte>
				{
					(byte)SpellByte.ICE2,
					(byte)SpellByte.FIRE,
					(byte)SpellByte.SLOW,
					(byte)SpellByte.INVS,
					(byte)SpellByte.LIT2,
					(byte)SpellByte.RUB,
					(byte)SpellByte.ICE2,
					(byte)SpellByte.FIRE,
				},
				SkillChance1 = 0x40,
				Skills1 = new List<byte>
				{
					(byte)EnemySkills.Dazzle,
					(byte)EnemySkills.Cremate,
					(byte)EnemySkills.Frost,
					(byte)EnemySkills.Stare,
				},
				SpellChance2 = 0x40,
				Spells2 = new List<byte>
				{
					(byte)SpellByte.NUKE,
					(byte)SpellByte.XXXX,
					(byte)SpellByte.FAST,
					(byte)SpellByte.FIR3,
					(byte)SpellByte.LIT3,
					(byte)SpellByte.FOG2,
					(byte)SpellByte.SABR,
					(byte)SpellByte.BANE,
				},
				SkillChance2 = 0x40,
				Skills2 = new List<byte>
				{
					(byte)EnemySkills.Stinger,
					(byte)EnemySkills.Thunder,
					(byte)EnemySkills.Toxic,
					(byte)EnemySkills.Crack,
				},
			},
			new AlternateFiends
			{
				Name = "KATNSOUL",
				ElementalWeakness = SpellElement.Poison,
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
					(byte)EnemySkills.Frost,
					(byte)EnemySkills.Cremate,
					(byte)EnemySkills.Stare,
					(byte)EnemySkills.Squint,
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
					(byte)EnemySkills.Inferno,
					(byte)EnemySkills.Thunder,
					(byte)EnemySkills.Blizzard,
					(byte)EnemySkills.Tornado,
				},
			},
			new AlternateFiends
			{
				Name = "KEFKA",
				ElementalWeakness = SpellElement.None,
				MonsterType = MonsterType.MAGE,
				SpellChance1 = 0x40,
				Spells1 = new List<byte>
				{
					(byte)SpellByte.SLOW,
					(byte)SpellByte.ICE2,
					(byte)SpellByte.INVS,
					(byte)SpellByte.FIR2,
					(byte)SpellByte.LIT2,
					(byte)SpellByte.RUB,
					(byte)SpellByte.CUR2,
					(byte)SpellByte.ZAP,
				},
				SkillChance1 = 0x40,
				Skills1 = new List<byte>
				{
					(byte)EnemySkills.Stinger,
					(byte)EnemySkills.Frost,
					(byte)EnemySkills.Cremate,
					(byte)EnemySkills.Trance,
				},
				SpellChance2 = 0x40,
				Spells2 = new List<byte>
				{
					(byte)SpellByte.FIR3,
					(byte)SpellByte.LIT3,
					(byte)SpellByte.SLO2,
					(byte)SpellByte.FOG,
					(byte)SpellByte.NUKE,
					(byte)SpellByte.XXXX,
					(byte)SpellByte.XFER,
					(byte)SpellByte.ICE3,
				},
				SkillChance2 = 0x40,
				Skills2 = new List<byte>
				{
					(byte)EnemySkills.Tornado,
					(byte)EnemySkills.Swirl,
					(byte)EnemySkills.Crack,
					(byte)EnemySkills.Poison_Damage,
				},
			},
			new AlternateFiends
			{
				Name = "M.MASTER",
				ElementalWeakness = SpellElement.None,
				MonsterType = MonsterType.MAGE,
				SpellChance1 = 0x40,
				Spells1 = new List<byte>
				{
					(byte)SpellByte.ICE2,
					(byte)SpellByte.FIR2,
					(byte)SpellByte.LIT2,
					(byte)SpellByte.MUTE,
					(byte)SpellByte.RUB,
					(byte)SpellByte.MUTE,
					(byte)SpellByte.HOLD,
					(byte)SpellByte.ZAP,
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
					(byte)SpellByte.SLO2,
					(byte)SpellByte.XXXX,
					(byte)SpellByte.MUTE,
					(byte)SpellByte.ICE3,
					(byte)SpellByte.BANE,
					(byte)SpellByte.INVS,
					(byte)SpellByte.LIT3,
					(byte)SpellByte.BRAK,
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
				Name = "NERAPA",
				ElementalWeakness = SpellElement.Lightning,
				MonsterType = MonsterType.MAGICAL,
				SpellChance1 = 0x40,
				Spells1 = new List<byte>
				{
					(byte)SpellByte.FIR2,
					(byte)SpellByte.RUB,
					(byte)SpellByte.ALIT,
					(byte)SpellByte.FIRE,
					(byte)SpellByte.SLOW,
					(byte)SpellByte.FIR2,
					(byte)SpellByte.SLEP,
					(byte)SpellByte.FIRE,
				},
				SkillChance1 = 0x40,
				Skills1 = new List<byte>
				{
					(byte)EnemySkills.Heat,
					(byte)EnemySkills.Frost,
					(byte)EnemySkills.Trance,
					(byte)EnemySkills.Ink,
				},
				SpellChance2 = 0x40,
				Spells2 = new List<byte>
				{
					(byte)SpellByte.FAST,
					(byte)SpellByte.FIR3,
					(byte)SpellByte.SLO2,
					(byte)SpellByte.FIR2,
					(byte)SpellByte.XXXX,
					(byte)SpellByte.SLP2,
					(byte)SpellByte.FIR3,
					(byte)SpellByte.BRAK,
				},
				SkillChance2 = 0x40,
				Skills2 = new List<byte>
				{
					(byte)EnemySkills.Cremate,
					(byte)EnemySkills.Inferno,
					(byte)EnemySkills.Inferno,
					(byte)EnemySkills.Glance,
				},
			},
			new AlternateFiends
			{
				Name = "NO128",
				ElementalWeakness = SpellElement.Fire,
				MonsterType = MonsterType.WERE,
				SpellChance1 = 0x40,
				Spells1 = new List<byte>
				{
					(byte)SpellByte.HOLD,
					(byte)SpellByte.ICE2,
					(byte)SpellByte.FAST,
					(byte)SpellByte.ICE,
					(byte)SpellByte.CUR2,
					(byte)SpellByte.ICE,
					(byte)SpellByte.BRAK,
					(byte)SpellByte.ICE2,
				},
				SkillChance1 = 0x40,
				Skills1 = new List<byte>
				{
					(byte)EnemySkills.Frost,
					(byte)EnemySkills.Ink,
					(byte)EnemySkills.Trance,
					(byte)EnemySkills.Frost,
				},
				SpellChance2 = 0x40,
				Spells2 = new List<byte>
				{
					(byte)SpellByte.INVS,
					(byte)SpellByte.ICE3,
					(byte)SpellByte.STOP,
					(byte)SpellByte.ICE3,
					(byte)SpellByte.STUN,
					(byte)SpellByte.ICE2,
					(byte)SpellByte.FAST,
					(byte)SpellByte.BANE,
				},
				SkillChance2 = 0x40,
				Skills2 = new List<byte>
				{
					(byte)EnemySkills.Blaze,
					(byte)EnemySkills.Tornado,
					(byte)EnemySkills.Crack,
					(byte)EnemySkills.Toxic,
				},
			},
			new AlternateFiends
			{
				Name = "P.TRAIN",
				ElementalWeakness = SpellElement.Fire,
				MonsterType = MonsterType.UNDEAD,
				SpellChance1 = 0x40,
				Spells1 = new List<byte>
				{
					(byte)SpellByte.SLEP,
					(byte)SpellByte.HOLD,
					(byte)SpellByte.STUN,
					(byte)SpellByte.TMPR,
					(byte)SpellByte.SLOW,
					(byte)SpellByte.SLEP,
					(byte)SpellByte.STUN,
					(byte)SpellByte.DARK,
				},
				SkillChance1 = 0x40,
				Skills1 = new List<byte>
				{
					(byte)EnemySkills.Frost,
					(byte)EnemySkills.Trance,
					(byte)EnemySkills.Frost,
					(byte)EnemySkills.Frost,
				},
				SpellChance2 = 0x40,
				Spells2 = new List<byte>
				{
					(byte)SpellByte.STOP,
					(byte)SpellByte.SLO2,
					(byte)SpellByte.SLP2,
					(byte)SpellByte.FAST,
					(byte)SpellByte.SABR,
					(byte)SpellByte.ZAP,
					(byte)SpellByte.XXXX,
					(byte)SpellByte.STOP,
				},
				SkillChance2 = 0x40,
				Skills2 = new List<byte>
				{
					(byte)EnemySkills.Tornado,
					(byte)EnemySkills.Swirl,
					(byte)EnemySkills.Poison_Stone,
					(byte)EnemySkills.Poison_Damage,
				},
			},
			new AlternateFiends
			{
				Name = "PHUNBABA",
				ElementalWeakness = SpellElement.Poison,
				MonsterType = MonsterType.GIANT,
				SpellChance1 = 0x40,
				Spells1 = new List<byte>
				{
					(byte)SpellByte.LIT2,
					(byte)SpellByte.LIT,
					(byte)SpellByte.HOLD,
					(byte)SpellByte.LIT2,
					(byte)SpellByte.SLOW,
					(byte)SpellByte.LOCK,
					(byte)SpellByte.LIT,
					(byte)SpellByte.LIT2,
				},
				SkillChance1 = 0x40,
				Skills1 = new List<byte>
				{
					(byte)EnemySkills.Stare,
					(byte)EnemySkills.Gaze,
					(byte)EnemySkills.Trance,
					(byte)EnemySkills.Stinger,
				},
				SpellChance2 = 0x40,
				Spells2 = new List<byte>
				{
					(byte)SpellByte.LIT3,
					(byte)SpellByte.FAST,
					(byte)SpellByte.TMPR,
					(byte)SpellByte.LIT3,
					(byte)SpellByte.SABR,
					(byte)SpellByte.LIT3,
					(byte)SpellByte.SLO2,
					(byte)SpellByte.SLP2,
				},
				SkillChance2 = 0x40,
				Skills2 = new List<byte>
				{
					(byte)EnemySkills.Squint,
					(byte)EnemySkills.Tornado,
					(byte)EnemySkills.Swirl,
					(byte)EnemySkills.Toxic,
				},
			},
			new AlternateFiends
			{
				Name = "POLTRGST",
				ElementalWeakness = SpellElement.Poison,
				MonsterType = MonsterType.MAGICAL,
				SpellChance1 = 0x40,
				Spells1 = new List<byte>
				{
					(byte)SpellByte.FOG,
					(byte)SpellByte.FIR2,
					(byte)SpellByte.FOG,
					(byte)SpellByte.FIRE,
					(byte)SpellByte.ZAP,
					(byte)SpellByte.FIR2,
					(byte)SpellByte.STUN,
					(byte)SpellByte.HOLD,
				},
				SkillChance1 = 0x40,
				Skills1 = new List<byte>
				{
					(byte)EnemySkills.Heat,
					(byte)EnemySkills.Cremate,
					(byte)EnemySkills.Dazzle,
					(byte)EnemySkills.Gaze,
				},
				SpellChance2 = 0x40,
				Spells2 = new List<byte>
				{
					(byte)SpellByte.FOG2,
					(byte)SpellByte.BANE,
					(byte)SpellByte.FIR3,
					(byte)SpellByte.FIR2,
					(byte)SpellByte.BRAK,
					(byte)SpellByte.STOP,
					(byte)SpellByte.SLP2,
					(byte)SpellByte.FIR3,
				},
				SkillChance2 = 0x40,
				Skills2 = new List<byte>
				{
					(byte)EnemySkills.Blaze,
					(byte)EnemySkills.Inferno,
					(byte)EnemySkills.Toxic,
					(byte)EnemySkills.Tornado,
				},
			},
			new AlternateFiends
			{
				Name = "R.DRAGON",
				ElementalWeakness = SpellElement.Ice,
				MonsterType = MonsterType.DRAGON,
				SpellChance1 = 0x40,
				Spells1 = new List<byte>
				{
					(byte)SpellByte.FIRE,
					(byte)SpellByte.FIR2,
					(byte)SpellByte.FOG,
					(byte)SpellByte.MUTE,
					(byte)SpellByte.FIR2,
					(byte)SpellByte.FIRE,
					(byte)SpellByte.SLOW,
					(byte)SpellByte.FIR2,
				},
				SkillChance1 = 0x40,
				Skills1 = new List<byte>
				{
					(byte)EnemySkills.Heat,
					(byte)EnemySkills.Cremate,
					(byte)EnemySkills.Heat,
					(byte)EnemySkills.Cremate,
				},
				SpellChance2 = 0x40,
				Spells2 = new List<byte>
				{
					(byte)SpellByte.FIR2,
					(byte)SpellByte.NUKE,
					(byte)SpellByte.XXXX,
					(byte)SpellByte.FIR3,
					(byte)SpellByte.FAST,
					(byte)SpellByte.FIR3,
					(byte)SpellByte.SLO2,
					(byte)SpellByte.XFER,
				},
				SkillChance2 = 0x40,
				Skills2 = new List<byte>
				{
					(byte)EnemySkills.Poison_Stone,
					(byte)EnemySkills.Blaze,
					(byte)EnemySkills.Squint,
					(byte)EnemySkills.Toxic,
				},
			},
			new AlternateFiends
			{
				Name = "RIZOPAS",
				ElementalWeakness = SpellElement.Lightning,
				MonsterType = MonsterType.AQUATIC,
				SpellChance1 = 0x40,
				Spells1 = new List<byte>
				{
					(byte)SpellByte.ICE,
					(byte)SpellByte.ICE2,
					(byte)SpellByte.LIT2,
					(byte)SpellByte.INVS,
					(byte)SpellByte.ICE,
					(byte)SpellByte.SLOW,
					(byte)SpellByte.LOCK,
					(byte)SpellByte.LIT2,
				},
				SkillChance1 = 0x40,
				Skills1 = new List<byte>
				{
					(byte)EnemySkills.Frost,
					(byte)EnemySkills.Trance,
					(byte)EnemySkills.Stinger,
					(byte)EnemySkills.Dazzle,
				},
				SpellChance2 = 0x40,
				Spells2 = new List<byte>
				{
					(byte)SpellByte.ICE2,
					(byte)SpellByte.SABR,
					(byte)SpellByte.LIT3,
					(byte)SpellByte.SLO2,
					(byte)SpellByte.ICE2,
					(byte)SpellByte.XFER,
					(byte)SpellByte.XXXX,
					(byte)SpellByte.ICE3,
				},
				SkillChance2 = 0x40,
				Skills2 = new List<byte>
				{
					(byte)EnemySkills.Thunder,
					(byte)EnemySkills.Blizzard,
					(byte)EnemySkills.Swirl,
					(byte)EnemySkills.Stinger,
				},
			},
			new AlternateFiends
			{
				Name = "SKDRAGON",
				ElementalWeakness = SpellElement.Fire,
				MonsterType = MonsterType.UNDEAD,
				SpellChance1 = 0x40,
				Spells1 = new List<byte>
				{
					(byte)SpellByte.XFER,
					(byte)SpellByte.SLOW,
					(byte)SpellByte.BANE,
					(byte)SpellByte.DARK,
					(byte)SpellByte.BRAK,
					(byte)SpellByte.XFER,
					(byte)SpellByte.SLOW,
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
					(byte)SpellByte.XXXX,
					(byte)SpellByte.BANE,
					(byte)SpellByte.BRAK,
					(byte)SpellByte.QAKE,
					(byte)SpellByte.XFER,
					(byte)SpellByte.RUB,
					(byte)SpellByte.BANE,
					(byte)SpellByte.BRAK,
				},
				SkillChance2 = 0x40,
				Skills2 = new List<byte>
				{
					(byte)EnemySkills.Crack,
					(byte)EnemySkills.Toxic,
					(byte)EnemySkills.Squint,
					(byte)EnemySkills.Glance,
				},
			},
			new AlternateFiends
			{
				Name = "STDRAGON",
				ElementalWeakness = SpellElement.Lightning,
				MonsterType = MonsterType.DRAGON,
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
					(byte)EnemySkills.Trance,
					(byte)EnemySkills.Crack,
					(byte)EnemySkills.Stare,
					(byte)EnemySkills.Stare,
				},
				SpellChance2 = 0x40,
				Spells2 = new List<byte>
				{
					(byte)SpellByte.FAST,
					(byte)SpellByte.CUR4,
					(byte)SpellByte.LOK2,
					(byte)SpellByte.MUTE,
					(byte)SpellByte.SLO2,
					(byte)SpellByte.STOP,
					(byte)SpellByte.TMPR,
					(byte)SpellByte.FOG2,
				},
				SkillChance2 = 0x40,
				Skills2 = new List<byte>
				{
					(byte)EnemySkills.Tornado,
					(byte)EnemySkills.Swirl,
					(byte)EnemySkills.Tornado,
					(byte)EnemySkills.Gaze,
				},
			},
			new AlternateFiends
			{
				Name = "T.ARMOR",
				ElementalWeakness = SpellElement.Lightning,
				MonsterType = MonsterType.REGENERATIVE,
				SpellChance1 = 0x40,
				Spells1 = new List<byte>
				{
					(byte)SpellByte.FIRE,
					(byte)SpellByte.ICE2,
					(byte)SpellByte.LIT,
					(byte)SpellByte.BRAK,
					(byte)SpellByte.FIR2,
					(byte)SpellByte.ICE,
					(byte)SpellByte.LIT2,
					(byte)SpellByte.BRAK,
				},
				SkillChance1 = 0x40,
				Skills1 = new List<byte>
				{
					(byte)EnemySkills.Stinger,
					(byte)EnemySkills.Ink,
					(byte)EnemySkills.Cremate,
					(byte)EnemySkills.Frost,
				},
				SpellChance2 = 0x40,
				Spells2 = new List<byte>
				{
					(byte)SpellByte.FIR3,
					(byte)SpellByte.BANE,
					(byte)SpellByte.ICE2,
					(byte)SpellByte.LIT3,
					(byte)SpellByte.BRAK,
					(byte)SpellByte.ICE3,
					(byte)SpellByte.FIR2,
					(byte)SpellByte.LIT2,
				},
				SkillChance2 = 0x40,
				Skills2 = new List<byte>
				{
					(byte)EnemySkills.Poison_Stone,
					(byte)EnemySkills.Poison_Damage,
					(byte)EnemySkills.Toxic,
					(byte)EnemySkills.Inferno,
				},
			},
			new AlternateFiends
			{
				Name = "TENTACLE",
				ElementalWeakness = SpellElement.Earth,
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
					(byte)EnemySkills.Dazzle,
					(byte)EnemySkills.Trance,
					(byte)EnemySkills.Gaze,
				},
				SpellChance2 = 0x40,
				Spells2 = new List<byte>
				{
					(byte)SpellByte.STOP,
					(byte)SpellByte.BANE,
					(byte)SpellByte.STUN,
					(byte)SpellByte.SLO2,
					(byte)SpellByte.HOLD,
					(byte)SpellByte.BRAK,
					(byte)SpellByte.STOP,
					(byte)SpellByte.BANE,
				},
				SkillChance2 = 0x40,
				Skills2 = new List<byte>
				{
					(byte)EnemySkills.Toxic,
					(byte)EnemySkills.Squint,
					(byte)EnemySkills.Glare,
					(byte)EnemySkills.Poison_Damage,
				},
			},
			new AlternateFiends
			{
				Name = "TONBERRY",
				ElementalWeakness = SpellElement.Fire,
				MonsterType = MonsterType.NONE,
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
				SpellChance2 = 0x40,
				Spells2 = new List<byte>
				{
					(byte)SpellByte.FAST,
					(byte)SpellByte.SABR,
					(byte)SpellByte.FOG,
					(byte)SpellByte.SLP2,
					(byte)SpellByte.TMPR,
					(byte)SpellByte.STOP,
					(byte)SpellByte.SLO2,
					(byte)SpellByte.SABR,
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
				Name = "TRITOCH",
				ElementalWeakness = SpellElement.Fire,
				MonsterType = MonsterType.MAGICAL,
				SpellChance1 = 0x40,
				Spells1 = new List<byte>
				{
					(byte)SpellByte.ICE2,
					(byte)SpellByte.MUTE,
					(byte)SpellByte.ICE,
					(byte)SpellByte.STUN,
					(byte)SpellByte.ICE2,
					(byte)SpellByte.MUTE,
					(byte)SpellByte.ICE,
					(byte)SpellByte.SLOW,
				},
				SkillChance1 = 0x40,
				Skills1 = new List<byte>
				{
					(byte)EnemySkills.Frost,
					(byte)EnemySkills.Frost,
					(byte)EnemySkills.Stare,
					(byte)EnemySkills.Trance,
				},
				SpellChance2 = 0x40,
				Spells2 = new List<byte>
				{
					(byte)SpellByte.ICE3,
					(byte)SpellByte.SLO2,
					(byte)SpellByte.ICE2,
					(byte)SpellByte.CUR3,
					(byte)SpellByte.ICE3,
					(byte)SpellByte.QAKE,
					(byte)SpellByte.ICE2,
					(byte)SpellByte.SLP2,
				},
				SkillChance2 = 0x40,
				Skills2 = new List<byte>
				{
					(byte)EnemySkills.Blizzard,
					(byte)EnemySkills.Squint,
					(byte)EnemySkills.Blizzard,
					(byte)EnemySkills.Crack,
				},
			},
			new AlternateFiends
			{
				Name = "TYPHON",
				ElementalWeakness = SpellElement.Ice,
				MonsterType = MonsterType.MAGICAL,
				SpellChance1 = 0x40,
				Spells1 = new List<byte>
				{
					(byte)SpellByte.FIR2,
					(byte)SpellByte.STUN,
					(byte)SpellByte.FIRE,
					(byte)SpellByte.SLOW,
					(byte)SpellByte.FIR2,
					(byte)SpellByte.INVS,
					(byte)SpellByte.DARK,
					(byte)SpellByte.FIRE,
				},
				SkillChance1 = 0x40,
				Skills1 = new List<byte>
				{
					(byte)EnemySkills.Heat,
					(byte)EnemySkills.Cremate,
					(byte)EnemySkills.Stinger,
					(byte)EnemySkills.Gaze,
				},
				SpellChance2 = 0x40,
				Spells2 = new List<byte>
				{
					(byte)SpellByte.FIR3,
					(byte)SpellByte.STOP,
					(byte)SpellByte.FIR2,
					(byte)SpellByte.FAST,
					(byte)SpellByte.SLO2,
					(byte)SpellByte.FIR3,
					(byte)SpellByte.STOP,
					(byte)SpellByte.SLO2,
				},
				SkillChance2 = 0x40,
				Skills2 = new List<byte>
				{
					(byte)EnemySkills.Inferno,
					(byte)EnemySkills.Tornado,
					(byte)EnemySkills.Blaze,
					(byte)EnemySkills.Squint,
				},
			},
			new AlternateFiends
			{
				Name = "ULTROS",
				ElementalWeakness = SpellElement.Fire,
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
					(byte)EnemySkills.Ink,
					(byte)EnemySkills.Ink,
					(byte)EnemySkills.Trance,
					(byte)EnemySkills.Ink,
				},
				SpellChance2 = 0x40,
				Spells2 = new List<byte>
				{
					(byte)SpellByte.FAST,
					(byte)SpellByte.ICE3,
					(byte)SpellByte.BRAK,
					(byte)SpellByte.LIT2,
					(byte)SpellByte.WALL,
					(byte)SpellByte.FIR2,
					(byte)SpellByte.FOG,
					(byte)SpellByte.LIT3,
				},
				SkillChance2 = 0x40,
				Skills2 = new List<byte>
				{
					(byte)EnemySkills.Crack,
					(byte)EnemySkills.Swirl,
					(byte)EnemySkills.Ink,
					(byte)EnemySkills.Swirl,
				},
			},
			new AlternateFiends
			{
				Name = "VARGAS",
				ElementalWeakness = SpellElement.Poison,
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
				SkillChance2 = 0x40,
				Skills2 = new List<byte>
				{
					(byte)EnemySkills.Tornado,
					(byte)EnemySkills.Poison_Stone,
					(byte)EnemySkills.Toxic,
					(byte)EnemySkills.Tornado,
				},
			},
			new AlternateFiends
			{
				Name = "WELK",
				ElementalWeakness = SpellElement.None,
				MonsterType = MonsterType.MAGICAL,
				SpellChance1 = 0x40,
				Spells1 = new List<byte>
				{
					(byte)SpellByte.SLOW,
					(byte)SpellByte.LIT,
					(byte)SpellByte.SLOW,
					(byte)SpellByte.LIT2,
					(byte)SpellByte.FOG,
					(byte)SpellByte.LIT,
					(byte)SpellByte.INVS,
					(byte)SpellByte.LIT2,
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
					(byte)SpellByte.LIT3,
					(byte)SpellByte.SLO2,
					(byte)SpellByte.LIT2,
					(byte)SpellByte.FOG2,
					(byte)SpellByte.WALL,
					(byte)SpellByte.LIT3,
					(byte)SpellByte.CUR3,
					(byte)SpellByte.LIT2,
				},
				SkillChance2 = 0x40,
				Skills2 = new List<byte>
				{
					(byte)EnemySkills.Gaze,
					(byte)EnemySkills.Poison_Stone,
					(byte)EnemySkills.Glance,
					(byte)EnemySkills.Thunder,
				},
			},
			new AlternateFiends
			{
				Name = "WREXSOUL",
				ElementalWeakness = SpellElement.Ice,
				MonsterType = MonsterType.UNDEAD,
				SpellChance1 = 0x40,
				Spells1 = new List<byte>
				{
					(byte)SpellByte.LIT,
					(byte)SpellByte.LIT2,
					(byte)SpellByte.RUB,
					(byte)SpellByte.LIT2,
					(byte)SpellByte.STUN,
					(byte)SpellByte.DARK,
					(byte)SpellByte.LIT,
					(byte)SpellByte.LIT2,
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
					(byte)SpellByte.LIT3,
					(byte)SpellByte.XXXX,
					(byte)SpellByte.SLO2,
					(byte)SpellByte.LIT3,
					(byte)SpellByte.BANE,
					(byte)SpellByte.XXXX,
					(byte)SpellByte.LIT3,
					(byte)SpellByte.ZAP,
				},
				SkillChance2 = 0x40,
				Skills2 = new List<byte>
				{
					(byte)EnemySkills.Thunder,
					(byte)EnemySkills.Toxic,
					(byte)EnemySkills.Crack,
					(byte)EnemySkills.Blaze,
				},
			},
			new AlternateFiends
			{
				Name = "YETI",
				ElementalWeakness = SpellElement.Fire,
				MonsterType = MonsterType.GIANT,
				SpellChance1 = 0x40,
				Spells1 = new List<byte>
				{
					(byte)SpellByte.ICE2,
					(byte)SpellByte.CUR2,
					(byte)SpellByte.ICE,
					(byte)SpellByte.TMPR,
					(byte)SpellByte.CUR2,
					(byte)SpellByte.ICE2,
					(byte)SpellByte.ICE,
					(byte)SpellByte.TMPR,
				},
				SkillChance1 = 0x40,
				Skills1 = new List<byte>
				{
					(byte)EnemySkills.Frost,
					(byte)EnemySkills.Frost,
					(byte)EnemySkills.Frost,
					(byte)EnemySkills.Stare,
				},
				SpellChance2 = 0x40,
				Spells2 = new List<byte>
				{
					(byte)SpellByte.ICE3,
					(byte)SpellByte.CUR4,
					(byte)SpellByte.FAST,
					(byte)SpellByte.ICE2,
					(byte)SpellByte.SABR,
					(byte)SpellByte.ICE2,
					(byte)SpellByte.TMPR,
					(byte)SpellByte.ICE3,
				},
				SkillChance2 = 0x40,
				Skills2 = new List<byte>
				{
					(byte)EnemySkills.Blizzard,
					(byte)EnemySkills.Blizzard,
					(byte)EnemySkills.Glance,
					(byte)EnemySkills.Crack,
				},
			},
			new AlternateFiends
			{
				Name = "ZIGFRIED",
				ElementalWeakness = SpellElement.Fire,
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
		};

		List<AlternateFiends> FF6AltFiendListHardcore = new()
		{
			new AlternateFiends
			{
				Name = "A.WORM",
				ElementalWeakness = SpellElement.None,
				MonsterType = MonsterType.NONE,
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
					(byte)EnemySkills.Stinger,
					(byte)EnemySkills.Trance,
					(byte)EnemySkills.Dazzle,
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
					(byte)EnemySkills.Crack,
					(byte)EnemySkills.Glance,
				},
			},
			new AlternateFiends
			{
				Name = "ATMA",
				ElementalWeakness = SpellElement.None,
				MonsterType = MonsterType.MAGICAL,
				SpellChance1 = 0x40,
				Spells1 = new List<byte>
				{
					(byte)SpellByte.FIR2,
					(byte)SpellByte.STUN,
					(byte)SpellByte.ICE2,
					(byte)SpellByte.SLOW,
					(byte)SpellByte.LIT2,
					(byte)SpellByte.MUTE,
					(byte)SpellByte.ZAP,
					(byte)SpellByte.ICE2,
				},
				SkillChance1 = 0x40,
				Skills1 = new List<byte>
				{
					(byte)EnemySkills.Frost,
					(byte)EnemySkills.Heat,
					(byte)EnemySkills.Trance,
					(byte)EnemySkills.Stinger,
				},
				SpellChance2 = 0x40,
				Spells2 = new List<byte>
				{
					(byte)SpellByte.FIR3,
					(byte)SpellByte.QAKE,
					(byte)SpellByte.ICE3,
					(byte)SpellByte.SLO2,
					(byte)SpellByte.LIT3,
					(byte)SpellByte.XXXX,
					(byte)SpellByte.FADE,
					(byte)SpellByte.WALL,
				},
				SkillChance2 = 0x40,
				Skills2 = new List<byte>
				{
					(byte)EnemySkills.Blizzard,
					(byte)EnemySkills.Inferno,
					(byte)EnemySkills.Poison_Stone,
					(byte)EnemySkills.Nuclear,
				},
			},
			new AlternateFiends
			{
				Name = "B.DRAGON",
				ElementalWeakness = SpellElement.Lightning,
				MonsterType = MonsterType.DRAGON,
				SpellChance1 = 0x40,
				Spells1 = new List<byte>
				{
					(byte)SpellByte.ICE2,
					(byte)SpellByte.SLOW,
					(byte)SpellByte.DARK,
					(byte)SpellByte.ICE2,
					(byte)SpellByte.MUTE,
					(byte)SpellByte.ICE2,
					(byte)SpellByte.SLOW,
					(byte)SpellByte.ICE2,
				},
				SkillChance1 = 0x40,
				Skills1 = new List<byte>
				{
					(byte)EnemySkills.Stinger,
					(byte)EnemySkills.Frost,
					(byte)EnemySkills.Trance,
					(byte)EnemySkills.Frost,
				},
				SpellChance2 = 0x40,
				Spells2 = new List<byte>
				{
					(byte)SpellByte.ICE3,
					(byte)SpellByte.XXXX,
					(byte)SpellByte.SLO2,
					(byte)SpellByte.ICE3,
					(byte)SpellByte.SLO2,
					(byte)SpellByte.MUTE,
					(byte)SpellByte.LOCK,
					(byte)SpellByte.ICE3,
				},
				SkillChance2 = 0x40,
				Skills2 = new List<byte>
				{
					(byte)EnemySkills.Poison_Damage,
					(byte)EnemySkills.Blizzard,
					(byte)EnemySkills.Toxic,
					(byte)EnemySkills.Swirl,
				},
			},
			new AlternateFiends
			{
				Name = "CHDRNOOK",
				ElementalWeakness = SpellElement.Fire,
				MonsterType = MonsterType.UNDEAD,
				SpellChance1 = 0x40,
				Spells1 = new List<byte>
				{
					(byte)SpellByte.LIT2,
					(byte)SpellByte.LIT,
					(byte)SpellByte.FOG,
					(byte)SpellByte.CUR2,
					(byte)SpellByte.LIT2,
					(byte)SpellByte.FOG,
					(byte)SpellByte.LIT2,
					(byte)SpellByte.LIT,
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
					(byte)SpellByte.LIT3,
					(byte)SpellByte.FAST,
					(byte)SpellByte.LIT3,
					(byte)SpellByte.SLP2,
					(byte)SpellByte.LIT3,
					(byte)SpellByte.CUR3,
					(byte)SpellByte.LIT3,
					(byte)SpellByte.RUB,
				},
				SkillChance2 = 0x40,
				Skills2 = new List<byte>
				{
					(byte)EnemySkills.Thunder,
					(byte)EnemySkills.Squint,
					(byte)EnemySkills.Glare,
					(byte)EnemySkills.Thunder,
				},
			},
			new AlternateFiends
			{
				Name = "CRANE",
				ElementalWeakness = SpellElement.Ice,
				MonsterType = MonsterType.NONE,
				SpellChance1 = 0x40,
				Spells1 = new List<byte>
				{
					(byte)SpellByte.LIT2,
					(byte)SpellByte.FOG,
					(byte)SpellByte.LIT2,
					(byte)SpellByte.INVS,
					(byte)SpellByte.LIT2,
					(byte)SpellByte.LIT,
					(byte)SpellByte.FOG,
					(byte)SpellByte.LIT,
				},
				SkillChance1 = 0x40,
				Skills1 = new List<byte>
				{
					(byte)EnemySkills.Trance,
					(byte)EnemySkills.Gaze,
					(byte)EnemySkills.Trance,
					(byte)EnemySkills.Gaze,
				},
				SpellChance2 = 0x40,
				Spells2 = new List<byte>
				{
					(byte)SpellByte.FOG2,
					(byte)SpellByte.LIT3,
					(byte)SpellByte.WALL,
					(byte)SpellByte.LIT3,
					(byte)SpellByte.RUB,
					(byte)SpellByte.LIT3,
					(byte)SpellByte.STOP,
					(byte)SpellByte.LIT3,
				},
				SkillChance2 = 0x40,
				Skills2 = new List<byte>
				{
					(byte)EnemySkills.Dazzle,
					(byte)EnemySkills.Thunder,
					(byte)EnemySkills.Toxic,
					(byte)EnemySkills.Tornado,
				},
			},
			new AlternateFiends
			{
				Name = "DADALUMA",
				ElementalWeakness = SpellElement.Poison,
				MonsterType = MonsterType.GIANT,
				SpellChance1 = 0x40,
				Spells1 = new List<byte>
				{
					(byte)SpellByte.CURE,
					(byte)SpellByte.FOG,
					(byte)SpellByte.HEL2,
					(byte)SpellByte.FOG,
					(byte)SpellByte.CURE,
					(byte)SpellByte.INVS,
					(byte)SpellByte.HEL2,
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
					(byte)SpellByte.CUR3,
					(byte)SpellByte.FAST,
					(byte)SpellByte.FOG2,
					(byte)SpellByte.MUTE,
					(byte)SpellByte.TMPR,
					(byte)SpellByte.HEL3,
					(byte)SpellByte.CUR3,
					(byte)SpellByte.FOG2,
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
				Name = "DOOMGAZE",
				ElementalWeakness = SpellElement.Fire,
				MonsterType = MonsterType.UNDEAD,
				SpellChance1 = 0x40,
				Spells1 = new List<byte>
				{
					(byte)SpellByte.ICE2,
					(byte)SpellByte.RUB,
					(byte)SpellByte.FOG,
					(byte)SpellByte.ICE2,
					(byte)SpellByte.ZAP,
					(byte)SpellByte.FOG,
					(byte)SpellByte.MUTE,
					(byte)SpellByte.ICE2,
				},
				SkillChance1 = 0x40,
				Skills1 = new List<byte>
				{
					(byte)EnemySkills.Gaze,
					(byte)EnemySkills.Stare,
					(byte)EnemySkills.Stare,
					(byte)EnemySkills.Gaze,
				},
				SpellChance2 = 0x40,
				Spells2 = new List<byte>
				{
					(byte)SpellByte.QAKE,
					(byte)SpellByte.ICE3,
					(byte)SpellByte.XXXX,
					(byte)SpellByte.ZAP,
					(byte)SpellByte.WALL,
					(byte)SpellByte.ICE3,
					(byte)SpellByte.BRAK,
					(byte)SpellByte.BANE,
				},
				SkillChance2 = 0x40,
				Skills2 = new List<byte>
				{
					(byte)EnemySkills.Toxic,
					(byte)EnemySkills.Nuclear,
					(byte)EnemySkills.Poison_Stone,
					(byte)EnemySkills.Tornado,
				},
			},
			new AlternateFiends
			{
				Name = "DULLAHAN",
				ElementalWeakness = SpellElement.Fire,
				MonsterType = MonsterType.MAGE,
				SpellChance1 = 0x40,
				Spells1 = new List<byte>
				{
					(byte)SpellByte.SLEP,
					(byte)SpellByte.ICE2,
					(byte)SpellByte.ICE2,
					(byte)SpellByte.MUTE,
					(byte)SpellByte.ICE2,
					(byte)SpellByte.SLOW,
					(byte)SpellByte.ICE2,
					(byte)SpellByte.RUB,
				},
				SkillChance1 = 0x40,
				Skills1 = new List<byte>
				{
					(byte)EnemySkills.Stinger,
					(byte)EnemySkills.Frost,
					(byte)EnemySkills.Dazzle,
					(byte)EnemySkills.Frost,
				},
				SpellChance2 = 0x40,
				Spells2 = new List<byte>
				{
					(byte)SpellByte.ICE3,
					(byte)SpellByte.WALL,
					(byte)SpellByte.FADE,
					(byte)SpellByte.CUR4,
					(byte)SpellByte.ICE3,
					(byte)SpellByte.SLO2,
					(byte)SpellByte.ICE3,
					(byte)SpellByte.BRAK,
				},
				SkillChance2 = 0x40,
				Skills2 = new List<byte>
				{
					(byte)EnemySkills.Blizzard,
					(byte)EnemySkills.Poison_Damage,
					(byte)EnemySkills.Toxic,
					(byte)EnemySkills.Blizzard,
				},
			},
			new AlternateFiends
			{
				Name = "E.DRAGON",
				ElementalWeakness = SpellElement.Ice,
				MonsterType = MonsterType.DRAGON,
				SpellChance1 = 0x40,
				Spells1 = new List<byte>
				{
					(byte)SpellByte.QAKE,
					(byte)SpellByte.LIT2,
					(byte)SpellByte.XFER,
					(byte)SpellByte.LIT2,
					(byte)SpellByte.SLOW,
					(byte)SpellByte.LIT2,
					(byte)SpellByte.QAKE,
					(byte)SpellByte.XFER,
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
					(byte)SpellByte.LIT3,
					(byte)SpellByte.QAKE,
					(byte)SpellByte.XXXX,
					(byte)SpellByte.BANE,
					(byte)SpellByte.LIT3,
					(byte)SpellByte.BRAK,
					(byte)SpellByte.LIT3,
					(byte)SpellByte.QAKE,
				},
				SkillChance2 = 0x40,
				Skills2 = new List<byte>
				{
					(byte)EnemySkills.Crack,
					(byte)EnemySkills.Thunder,
					(byte)EnemySkills.Crack,
					(byte)EnemySkills.Thunder,
				},
			},
			new AlternateFiends
			{
				Name = "F.EATER",
				ElementalWeakness = SpellElement.Ice,
				MonsterType = MonsterType.MAGICAL,
				SpellChance1 = 0x40,
				Spells1 = new List<byte>
				{
					(byte)SpellByte.FIRE,
					(byte)SpellByte.FIR2,
					(byte)SpellByte.FOG,
					(byte)SpellByte.FIR2,
					(byte)SpellByte.HEL2,
					(byte)SpellByte.FIRE,
					(byte)SpellByte.FIR2,
					(byte)SpellByte.FOG,
				},
				SkillChance1 = 0x40,
				Skills1 = new List<byte>
				{
					(byte)EnemySkills.Heat,
					(byte)EnemySkills.Heat,
					(byte)EnemySkills.Cremate,
					(byte)EnemySkills.Cremate,
				},
				SpellChance2 = 0x40,
				Spells2 = new List<byte>
				{
					(byte)SpellByte.FIR3,
					(byte)SpellByte.FOG2,
					(byte)SpellByte.FIR3,
					(byte)SpellByte.NUKE,
					(byte)SpellByte.FOG2,
					(byte)SpellByte.FIR3,
					(byte)SpellByte.HEL3,
					(byte)SpellByte.NUKE,
				},
				SkillChance2 = 0x40,
				Skills2 = new List<byte>
				{
					(byte)EnemySkills.Inferno,
					(byte)EnemySkills.Tornado,
					(byte)EnemySkills.Inferno,
					(byte)EnemySkills.Tornado,
				},
			},
			new AlternateFiends
			{
				Name = "FIEND",
				ElementalWeakness = SpellElement.None,
				MonsterType = MonsterType.REGENERATIVE,
				SpellChance1 = 0x40,
				Spells1 = new List<byte>
				{
					(byte)SpellByte.ICE,
					(byte)SpellByte.INVS,
					(byte)SpellByte.ICE2,
					(byte)SpellByte.TMPR,
					(byte)SpellByte.ICE,
					(byte)SpellByte.FOG,
					(byte)SpellByte.ICE2,
					(byte)SpellByte.ICE2,
				},
				SkillChance1 = 0x40,
				Skills1 = new List<byte>
				{
					(byte)EnemySkills.Frost,
					(byte)EnemySkills.Dazzle,
					(byte)EnemySkills.Trance,
					(byte)EnemySkills.Frost,
				},
				SpellChance2 = 0x40,
				Spells2 = new List<byte>
				{
					(byte)SpellByte.ICE3,
					(byte)SpellByte.FAST,
					(byte)SpellByte.ICE3,
					(byte)SpellByte.SABR,
					(byte)SpellByte.ICE2,
					(byte)SpellByte.ICE3,
					(byte)SpellByte.FOG2,
					(byte)SpellByte.ICE3,
				},
				SkillChance2 = 0x40,
				Skills2 = new List<byte>
				{
					(byte)EnemySkills.Blizzard,
					(byte)EnemySkills.Nuclear,
					(byte)EnemySkills.Toxic,
					(byte)EnemySkills.Poison_Damage,
				},
			},
			new AlternateFiends
			{
				Name = "FLANPRIN",
				ElementalWeakness = SpellElement.Fire,
				MonsterType = MonsterType.NONE,
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
					(byte)EnemySkills.Trance,
					(byte)EnemySkills.Dazzle,
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
					(byte)EnemySkills.Poison_Stone,
					(byte)EnemySkills.Toxic,
					(byte)EnemySkills.Glance,
					(byte)EnemySkills.Squint,
				},
			},
			new AlternateFiends
			{
				Name = "G.DRAGON",
				ElementalWeakness = SpellElement.Ice,
				MonsterType = MonsterType.DRAGON,
				SpellChance1 = 0x40,
				Spells1 = new List<byte>
				{
					(byte)SpellByte.INVS,
					(byte)SpellByte.LIT,
					(byte)SpellByte.LIT2,
					(byte)SpellByte.SLEP,
					(byte)SpellByte.LIT2,
					(byte)SpellByte.LIT2,
					(byte)SpellByte.STUN,
					(byte)SpellByte.LIT2,
				},
				SkillChance1 = 0x40,
				Skills1 = new List<byte>
				{
					(byte)EnemySkills.Snorting,
					(byte)EnemySkills.Trance,
					(byte)EnemySkills.Flash,
					(byte)EnemySkills.Stare,
				},
				SpellChance2 = 0x40,
				Spells2 = new List<byte>
				{
					(byte)SpellByte.LIT3,
					(byte)SpellByte.WALL,
					(byte)SpellByte.LIT3,
					(byte)SpellByte.FAST,
					(byte)SpellByte.LIT3,
					(byte)SpellByte.XXXX,
					(byte)SpellByte.LIT3,
					(byte)SpellByte.CUR3,
				},
				SkillChance2 = 0x40,
				Skills2 = new List<byte>
				{
					(byte)EnemySkills.Thunder,
					(byte)EnemySkills.Poison_Damage,
					(byte)EnemySkills.Thunder,
					(byte)EnemySkills.Squint,
				},
			},
			new AlternateFiends
			{
				Name = "GARGANT",
				ElementalWeakness = SpellElement.Poison,
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
					(byte)EnemySkills.Flash,
					(byte)EnemySkills.Stare,
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
					(byte)EnemySkills.Crack,
					(byte)EnemySkills.Tornado,
				},
			},
			new AlternateFiends
			{
				Name = "GODDESS",
				ElementalWeakness = SpellElement.None,
				MonsterType = MonsterType.REGENERATIVE,
				SpellChance1 = 0x40,
				Spells1 = new List<byte>
				{
					(byte)SpellByte.SLEP,
					(byte)SpellByte.LIT2,
					(byte)SpellByte.FOG,
					(byte)SpellByte.LIT2,
					(byte)SpellByte.INVS,
					(byte)SpellByte.LIT2,
					(byte)SpellByte.SLEP,
					(byte)SpellByte.LIT2,
				},
				SkillChance1 = 0x40,
				Skills1 = new List<byte>
				{
					(byte)EnemySkills.Dazzle,
					(byte)EnemySkills.Trance,
					(byte)EnemySkills.Flash,
					(byte)EnemySkills.Stare,
				},
				SpellChance2 = 0x40,
				Spells2 = new List<byte>
				{
					(byte)SpellByte.SLP2,
					(byte)SpellByte.LIT3,
					(byte)SpellByte.FAST,
					(byte)SpellByte.LIT3,
					(byte)SpellByte.BANE,
					(byte)SpellByte.LIT3,
					(byte)SpellByte.SLO2,
					(byte)SpellByte.LOCK,
				},
				SkillChance2 = 0x40,
				Skills2 = new List<byte>
				{
					(byte)EnemySkills.Toxic,
					(byte)EnemySkills.Thunder,
					(byte)EnemySkills.Crack,
					(byte)EnemySkills.Poison_Stone,
				},
			},
			new AlternateFiends
			{
				Name = "H.DRAGON",
				ElementalWeakness = SpellElement.None,
				MonsterType = MonsterType.DRAGON,
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
				SpellChance2 = 0x40,
				Spells2 = new List<byte>
				{
					(byte)SpellByte.FADE,
					(byte)SpellByte.XFER,
					(byte)SpellByte.FAST,
					(byte)SpellByte.XFER,
					(byte)SpellByte.FADE,
					(byte)SpellByte.SLO2,
					(byte)SpellByte.MUTE,
					(byte)SpellByte.SLP2,
				},
				SkillChance2 = 0x40,
				Skills2 = new List<byte>
				{
					(byte)EnemySkills.Trance,
					(byte)EnemySkills.Squint,
					(byte)EnemySkills.Poison_Stone,
					(byte)EnemySkills.Tornado,
				},
			},
			new AlternateFiends
			{
				Name = "H.RIDER",
				ElementalWeakness = SpellElement.Time,
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
					(byte)EnemySkills.Stinger,
					(byte)EnemySkills.Glance,
					(byte)EnemySkills.Stinger,
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
					(byte)EnemySkills.Crack,
					(byte)EnemySkills.Nuclear,
					(byte)EnemySkills.Squint,
					(byte)EnemySkills.Toxic,
				},
			},
			new AlternateFiends
			{
				Name = "I.DRAGON",
				ElementalWeakness = SpellElement.Fire,
				MonsterType = MonsterType.DRAGON,
				SpellChance1 = 0x40,
				Spells1 = new List<byte>
				{
					(byte)SpellByte.ICE2,
					(byte)SpellByte.STUN,
					(byte)SpellByte.ICE2,
					(byte)SpellByte.HOLD,
					(byte)SpellByte.ICE2,
					(byte)SpellByte.STUN,
					(byte)SpellByte.ICE2,
					(byte)SpellByte.HOLD,
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
					(byte)SpellByte.ICE3,
					(byte)SpellByte.STOP,
					(byte)SpellByte.ICE3,
					(byte)SpellByte.XFER,
					(byte)SpellByte.ICE3,
					(byte)SpellByte.STOP,
					(byte)SpellByte.ICE3,
					(byte)SpellByte.SLOW,
				},
				SkillChance2 = 0x40,
				Skills2 = new List<byte>
				{
					(byte)EnemySkills.Blizzard,
					(byte)EnemySkills.Trance,
					(byte)EnemySkills.Frost,
					(byte)EnemySkills.Blizzard,
				},
			},
			new AlternateFiends
			{
				Name = "IPOOH",
				ElementalWeakness = SpellElement.Fire,
				MonsterType = MonsterType.NONE,
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
				Name = "K.BEHEM",
				ElementalWeakness = SpellElement.Fire,
				MonsterType = MonsterType.REGENERATIVE,
				SpellChance1 = 0x40,
				Spells1 = new List<byte>
				{
					(byte)SpellByte.ICE,
					(byte)SpellByte.FOG,
					(byte)SpellByte.ICE2,
					(byte)SpellByte.ICE2,
					(byte)SpellByte.SLOW,
					(byte)SpellByte.SLEP,
					(byte)SpellByte.ICE2,
					(byte)SpellByte.ICE2,
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
					(byte)SpellByte.ICE3,
					(byte)SpellByte.FAST,
					(byte)SpellByte.FADE,
					(byte)SpellByte.SLO2,
					(byte)SpellByte.ICE3,
					(byte)SpellByte.FOG,
					(byte)SpellByte.FADE,
					(byte)SpellByte.FAST,
				},
				SkillChance2 = 0x40,
				Skills2 = new List<byte>
				{
					(byte)EnemySkills.Nuclear,
					(byte)EnemySkills.Blizzard,
					(byte)EnemySkills.Poison_Stone,
					(byte)EnemySkills.Squint,
				},
			},
			new AlternateFiends
			{
				Name = "K.DRAGON",
				ElementalWeakness = SpellElement.None,
				MonsterType = MonsterType.DRAGON,
				SpellChance1 = 0x40,
				Spells1 = new List<byte>
				{
					(byte)SpellByte.ICE2,
					(byte)SpellByte.FIR2,
					(byte)SpellByte.SLOW,
					(byte)SpellByte.INVS,
					(byte)SpellByte.LIT2,
					(byte)SpellByte.RUB,
					(byte)SpellByte.ICE2,
					(byte)SpellByte.FIR2,
				},
				SkillChance1 = 0x40,
				Skills1 = new List<byte>
				{
					(byte)EnemySkills.Dazzle,
					(byte)EnemySkills.Cremate,
					(byte)EnemySkills.Frost,
					(byte)EnemySkills.Stare,
				},
				SpellChance2 = 0x40,
				Spells2 = new List<byte>
				{
					(byte)SpellByte.NUKE,
					(byte)SpellByte.XXXX,
					(byte)SpellByte.FAST,
					(byte)SpellByte.FIR3,
					(byte)SpellByte.LIT3,
					(byte)SpellByte.FOG2,
					(byte)SpellByte.SABR,
					(byte)SpellByte.BANE,
				},
				SkillChance2 = 0x40,
				Skills2 = new List<byte>
				{
					(byte)EnemySkills.Inferno,
					(byte)EnemySkills.Thunder,
					(byte)EnemySkills.Toxic,
					(byte)EnemySkills.Crack,
				},
			},
			new AlternateFiends
			{
				Name = "KATNSOUL",
				ElementalWeakness = SpellElement.Poison,
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
					(byte)EnemySkills.Frost,
					(byte)EnemySkills.Cremate,
					(byte)EnemySkills.Stare,
					(byte)EnemySkills.Squint,
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
					(byte)EnemySkills.Inferno,
					(byte)EnemySkills.Thunder,
					(byte)EnemySkills.Blizzard,
					(byte)EnemySkills.Tornado,
				},
			},
			new AlternateFiends
			{
				Name = "KEFKA",
				ElementalWeakness = SpellElement.None,
				MonsterType = MonsterType.MAGE,
				SpellChance1 = 0x40,
				Spells1 = new List<byte>
				{
					(byte)SpellByte.SLOW,
					(byte)SpellByte.ICE2,
					(byte)SpellByte.INVS,
					(byte)SpellByte.FIR2,
					(byte)SpellByte.LIT2,
					(byte)SpellByte.RUB,
					(byte)SpellByte.CUR2,
					(byte)SpellByte.ZAP,
				},
				SkillChance1 = 0x40,
				Skills1 = new List<byte>
				{
					(byte)EnemySkills.Stinger,
					(byte)EnemySkills.Frost,
					(byte)EnemySkills.Cremate,
					(byte)EnemySkills.Trance,
				},
				SpellChance2 = 0x40,
				Spells2 = new List<byte>
				{
					(byte)SpellByte.FIR3,
					(byte)SpellByte.LIT3,
					(byte)SpellByte.SLO2,
					(byte)SpellByte.FOG,
					(byte)SpellByte.NUKE,
					(byte)SpellByte.XXXX,
					(byte)SpellByte.XFER,
					(byte)SpellByte.ICE3,
				},
				SkillChance2 = 0x40,
				Skills2 = new List<byte>
				{
					(byte)EnemySkills.Tornado,
					(byte)EnemySkills.Swirl,
					(byte)EnemySkills.Crack,
					(byte)EnemySkills.Poison_Damage,
				},
			},
			new AlternateFiends
			{
				Name = "M.MASTER",
				ElementalWeakness = SpellElement.None,
				MonsterType = MonsterType.MAGE,
				SpellChance1 = 0x40,
				Spells1 = new List<byte>
				{
					(byte)SpellByte.ICE2,
					(byte)SpellByte.FIR2,
					(byte)SpellByte.LIT2,
					(byte)SpellByte.MUTE,
					(byte)SpellByte.RUB,
					(byte)SpellByte.MUTE,
					(byte)SpellByte.HOLD,
					(byte)SpellByte.ZAP,
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
					(byte)SpellByte.NUKE,
					(byte)SpellByte.XXXX,
					(byte)SpellByte.MUTE,
					(byte)SpellByte.ICE3,
					(byte)SpellByte.SLO2,
					(byte)SpellByte.INVS,
					(byte)SpellByte.LIT3,
					(byte)SpellByte.BRAK,
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
				Name = "NERAPA",
				ElementalWeakness = SpellElement.Lightning,
				MonsterType = MonsterType.MAGICAL,
				SpellChance1 = 0x40,
				Spells1 = new List<byte>
				{
					(byte)SpellByte.FIR2,
					(byte)SpellByte.RUB,
					(byte)SpellByte.ALIT,
					(byte)SpellByte.FIR2,
					(byte)SpellByte.SLOW,
					(byte)SpellByte.FIR2,
					(byte)SpellByte.SLEP,
					(byte)SpellByte.FIR2,
				},
				SkillChance1 = 0x40,
				Skills1 = new List<byte>
				{
					(byte)EnemySkills.Heat,
					(byte)EnemySkills.Frost,
					(byte)EnemySkills.Trance,
					(byte)EnemySkills.Ink,
				},
				SpellChance2 = 0x40,
				Spells2 = new List<byte>
				{
					(byte)SpellByte.FAST,
					(byte)SpellByte.FIR3,
					(byte)SpellByte.SLO2,
					(byte)SpellByte.FIR3,
					(byte)SpellByte.XXXX,
					(byte)SpellByte.SLP2,
					(byte)SpellByte.FIR3,
					(byte)SpellByte.BRAK,
				},
				SkillChance2 = 0x40,
				Skills2 = new List<byte>
				{
					(byte)EnemySkills.Cremate,
					(byte)EnemySkills.Inferno,
					(byte)EnemySkills.Inferno,
					(byte)EnemySkills.Glance,
				},
			},
			new AlternateFiends
			{
				Name = "NO128",
				ElementalWeakness = SpellElement.Fire,
				MonsterType = MonsterType.WERE,
				SpellChance1 = 0x40,
				Spells1 = new List<byte>
				{
					(byte)SpellByte.HOLD,
					(byte)SpellByte.ICE2,
					(byte)SpellByte.FAST,
					(byte)SpellByte.ICE2,
					(byte)SpellByte.CUR2,
					(byte)SpellByte.ICE2,
					(byte)SpellByte.BRAK,
					(byte)SpellByte.ICE2,
				},
				SkillChance1 = 0x40,
				Skills1 = new List<byte>
				{
					(byte)EnemySkills.Frost,
					(byte)EnemySkills.Ink,
					(byte)EnemySkills.Trance,
					(byte)EnemySkills.Frost,
				},
				SpellChance2 = 0x40,
				Spells2 = new List<byte>
				{
					(byte)SpellByte.INVS,
					(byte)SpellByte.ICE3,
					(byte)SpellByte.STOP,
					(byte)SpellByte.ICE3,
					(byte)SpellByte.STUN,
					(byte)SpellByte.ICE3,
					(byte)SpellByte.FAST,
					(byte)SpellByte.BANE,
				},
				SkillChance2 = 0x40,
				Skills2 = new List<byte>
				{
					(byte)EnemySkills.Blaze,
					(byte)EnemySkills.Tornado,
					(byte)EnemySkills.Crack,
					(byte)EnemySkills.Toxic,
				},
			},
			new AlternateFiends
			{
				Name = "P.TRAIN",
				ElementalWeakness = SpellElement.Fire,
				MonsterType = MonsterType.UNDEAD,
				SpellChance1 = 0x40,
				Spells1 = new List<byte>
				{
					(byte)SpellByte.SLEP,
					(byte)SpellByte.HOLD,
					(byte)SpellByte.STUN,
					(byte)SpellByte.TMPR,
					(byte)SpellByte.SLOW,
					(byte)SpellByte.SLEP,
					(byte)SpellByte.STUN,
					(byte)SpellByte.DARK,
				},
				SkillChance1 = 0x40,
				Skills1 = new List<byte>
				{
					(byte)EnemySkills.Frost,
					(byte)EnemySkills.Trance,
					(byte)EnemySkills.Frost,
					(byte)EnemySkills.Frost,
				},
				SpellChance2 = 0x40,
				Spells2 = new List<byte>
				{
					(byte)SpellByte.STOP,
					(byte)SpellByte.SLO2,
					(byte)SpellByte.SLP2,
					(byte)SpellByte.FAST,
					(byte)SpellByte.SABR,
					(byte)SpellByte.ZAP,
					(byte)SpellByte.XXXX,
					(byte)SpellByte.STOP,
				},
				SkillChance2 = 0x40,
				Skills2 = new List<byte>
				{
					(byte)EnemySkills.Tornado,
					(byte)EnemySkills.Swirl,
					(byte)EnemySkills.Poison_Stone,
					(byte)EnemySkills.Poison_Damage,
				},
			},
			new AlternateFiends
			{
				Name = "PHUNBABA",
				ElementalWeakness = SpellElement.Poison,
				MonsterType = MonsterType.GIANT,
				SpellChance1 = 0x40,
				Spells1 = new List<byte>
				{
					(byte)SpellByte.LIT2,
					(byte)SpellByte.LIT2,
					(byte)SpellByte.HOLD,
					(byte)SpellByte.LIT2,
					(byte)SpellByte.SLOW,
					(byte)SpellByte.LOCK,
					(byte)SpellByte.LIT2,
					(byte)SpellByte.LIT2,
				},
				SkillChance1 = 0x40,
				Skills1 = new List<byte>
				{
					(byte)EnemySkills.Stare,
					(byte)EnemySkills.Gaze,
					(byte)EnemySkills.Trance,
					(byte)EnemySkills.Stinger,
				},
				SpellChance2 = 0x40,
				Spells2 = new List<byte>
				{
					(byte)SpellByte.LIT3,
					(byte)SpellByte.FAST,
					(byte)SpellByte.TMPR,
					(byte)SpellByte.LIT3,
					(byte)SpellByte.TMPR,
					(byte)SpellByte.LIT3,
					(byte)SpellByte.SLO2,
					(byte)SpellByte.SLP2,
				},
				SkillChance2 = 0x40,
				Skills2 = new List<byte>
				{
					(byte)EnemySkills.Squint,
					(byte)EnemySkills.Tornado,
					(byte)EnemySkills.Swirl,
					(byte)EnemySkills.Toxic,
				},
			},
			new AlternateFiends
			{
				Name = "POLTRGST",
				ElementalWeakness = SpellElement.Poison,
				MonsterType = MonsterType.MAGICAL,
				SpellChance1 = 0x40,
				Spells1 = new List<byte>
				{
					(byte)SpellByte.FOG,
					(byte)SpellByte.FIR2,
					(byte)SpellByte.FOG,
					(byte)SpellByte.FIR2,
					(byte)SpellByte.ZAP,
					(byte)SpellByte.FIR2,
					(byte)SpellByte.STUN,
					(byte)SpellByte.HOLD,
				},
				SkillChance1 = 0x40,
				Skills1 = new List<byte>
				{
					(byte)EnemySkills.Heat,
					(byte)EnemySkills.Cremate,
					(byte)EnemySkills.Dazzle,
					(byte)EnemySkills.Gaze,
				},
				SpellChance2 = 0x40,
				Spells2 = new List<byte>
				{
					(byte)SpellByte.FOG2,
					(byte)SpellByte.BANE,
					(byte)SpellByte.FIR3,
					(byte)SpellByte.FIR3,
					(byte)SpellByte.BRAK,
					(byte)SpellByte.STOP,
					(byte)SpellByte.SLP2,
					(byte)SpellByte.FIR3,
				},
				SkillChance2 = 0x40,
				Skills2 = new List<byte>
				{
					(byte)EnemySkills.Blaze,
					(byte)EnemySkills.Inferno,
					(byte)EnemySkills.Toxic,
					(byte)EnemySkills.Tornado,
				},
			},
			new AlternateFiends
			{
				Name = "R.DRAGON",
				ElementalWeakness = SpellElement.Ice,
				MonsterType = MonsterType.DRAGON,
				SpellChance1 = 0x40,
				Spells1 = new List<byte>
				{
					(byte)SpellByte.FIRE,
					(byte)SpellByte.FIR2,
					(byte)SpellByte.FOG,
					(byte)SpellByte.MUTE,
					(byte)SpellByte.FIR2,
					(byte)SpellByte.FIR2,
					(byte)SpellByte.SLOW,
					(byte)SpellByte.FIR2,
				},
				SkillChance1 = 0x40,
				Skills1 = new List<byte>
				{
					(byte)EnemySkills.Heat,
					(byte)EnemySkills.Cremate,
					(byte)EnemySkills.Heat,
					(byte)EnemySkills.Cremate,
				},
				SpellChance2 = 0x40,
				Spells2 = new List<byte>
				{
					(byte)SpellByte.FIR3,
					(byte)SpellByte.NUKE,
					(byte)SpellByte.XXXX,
					(byte)SpellByte.FIR3,
					(byte)SpellByte.FAST,
					(byte)SpellByte.FIR3,
					(byte)SpellByte.SLO2,
					(byte)SpellByte.XFER,
				},
				SkillChance2 = 0x40,
				Skills2 = new List<byte>
				{
					(byte)EnemySkills.Inferno,
					(byte)EnemySkills.Blaze,
					(byte)EnemySkills.Squint,
					(byte)EnemySkills.Toxic,
				},
			},
			new AlternateFiends
			{
				Name = "RIZOPAS",
				ElementalWeakness = SpellElement.Lightning,
				MonsterType = MonsterType.AQUATIC,
				SpellChance1 = 0x40,
				Spells1 = new List<byte>
				{
					(byte)SpellByte.ICE,
					(byte)SpellByte.ICE2,
					(byte)SpellByte.LIT2,
					(byte)SpellByte.INVS,
					(byte)SpellByte.ICE2,
					(byte)SpellByte.SLOW,
					(byte)SpellByte.LOCK,
					(byte)SpellByte.LIT2,
				},
				SkillChance1 = 0x40,
				Skills1 = new List<byte>
				{
					(byte)EnemySkills.Frost,
					(byte)EnemySkills.Trance,
					(byte)EnemySkills.Stinger,
					(byte)EnemySkills.Dazzle,
				},
				SpellChance2 = 0x40,
				Spells2 = new List<byte>
				{
					(byte)SpellByte.ICE3,
					(byte)SpellByte.SABR,
					(byte)SpellByte.LIT3,
					(byte)SpellByte.SLO2,
					(byte)SpellByte.ICE3,
					(byte)SpellByte.XFER,
					(byte)SpellByte.XXXX,
					(byte)SpellByte.ICE3,
				},
				SkillChance2 = 0x40,
				Skills2 = new List<byte>
				{
					(byte)EnemySkills.Thunder,
					(byte)EnemySkills.Blizzard,
					(byte)EnemySkills.Swirl,
					(byte)EnemySkills.Stinger,
				},
			},
			new AlternateFiends
			{
				Name = "SKDRAGON",
				ElementalWeakness = SpellElement.Fire,
				MonsterType = MonsterType.UNDEAD,
				SpellChance1 = 0x40,
				Spells1 = new List<byte>
				{
					(byte)SpellByte.XFER,
					(byte)SpellByte.SLOW,
					(byte)SpellByte.BANE,
					(byte)SpellByte.DARK,
					(byte)SpellByte.BRAK,
					(byte)SpellByte.XFER,
					(byte)SpellByte.SLOW,
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
					(byte)SpellByte.XXXX,
					(byte)SpellByte.BANE,
					(byte)SpellByte.BRAK,
					(byte)SpellByte.QAKE,
					(byte)SpellByte.XFER,
					(byte)SpellByte.RUB,
					(byte)SpellByte.BANE,
					(byte)SpellByte.BRAK,
				},
				SkillChance2 = 0x40,
				Skills2 = new List<byte>
				{
					(byte)EnemySkills.Crack,
					(byte)EnemySkills.Toxic,
					(byte)EnemySkills.Squint,
					(byte)EnemySkills.Glance,
				},
			},
			new AlternateFiends
			{
				Name = "STDRAGON",
				ElementalWeakness = SpellElement.Lightning,
				MonsterType = MonsterType.DRAGON,
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
					(byte)EnemySkills.Trance,
					(byte)EnemySkills.Crack,
					(byte)EnemySkills.Stare,
					(byte)EnemySkills.Stare,
				},
				SpellChance2 = 0x40,
				Spells2 = new List<byte>
				{
					(byte)SpellByte.FAST,
					(byte)SpellByte.CUR4,
					(byte)SpellByte.LOK2,
					(byte)SpellByte.MUTE,
					(byte)SpellByte.SLO2,
					(byte)SpellByte.STOP,
					(byte)SpellByte.TMPR,
					(byte)SpellByte.FOG2,
				},
				SkillChance2 = 0x40,
				Skills2 = new List<byte>
				{
					(byte)EnemySkills.Tornado,
					(byte)EnemySkills.Swirl,
					(byte)EnemySkills.Tornado,
					(byte)EnemySkills.Gaze,
				},
			},
			new AlternateFiends
			{
				Name = "T.ARMOR",
				ElementalWeakness = SpellElement.Lightning,
				MonsterType = MonsterType.REGENERATIVE,
				SpellChance1 = 0x40,
				Spells1 = new List<byte>
				{
					(byte)SpellByte.FIR2,
					(byte)SpellByte.ICE2,
					(byte)SpellByte.LIT2,
					(byte)SpellByte.BRAK,
					(byte)SpellByte.FIR2,
					(byte)SpellByte.ICE2,
					(byte)SpellByte.LIT2,
					(byte)SpellByte.BRAK,
				},
				SkillChance1 = 0x40,
				Skills1 = new List<byte>
				{
					(byte)EnemySkills.Stinger,
					(byte)EnemySkills.Ink,
					(byte)EnemySkills.Cremate,
					(byte)EnemySkills.Frost,
				},
				SpellChance2 = 0x40,
				Spells2 = new List<byte>
				{
					(byte)SpellByte.FIR3,
					(byte)SpellByte.BANE,
					(byte)SpellByte.ICE3,
					(byte)SpellByte.LIT3,
					(byte)SpellByte.BRAK,
					(byte)SpellByte.ICE3,
					(byte)SpellByte.FIR3,
					(byte)SpellByte.LIT3,
				},
				SkillChance2 = 0x40,
				Skills2 = new List<byte>
				{
					(byte)EnemySkills.Poison_Stone,
					(byte)EnemySkills.Poison_Damage,
					(byte)EnemySkills.Toxic,
					(byte)EnemySkills.Inferno,
				},
			},
			new AlternateFiends
			{
				Name = "TENTACLE",
				ElementalWeakness = SpellElement.Earth,
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
					(byte)EnemySkills.Dazzle,
					(byte)EnemySkills.Trance,
					(byte)EnemySkills.Gaze,
				},
				SpellChance2 = 0x40,
				Spells2 = new List<byte>
				{
					(byte)SpellByte.STOP,
					(byte)SpellByte.BANE,
					(byte)SpellByte.STUN,
					(byte)SpellByte.SLO2,
					(byte)SpellByte.HOLD,
					(byte)SpellByte.BRAK,
					(byte)SpellByte.STOP,
					(byte)SpellByte.BANE,
				},
				SkillChance2 = 0x40,
				Skills2 = new List<byte>
				{
					(byte)EnemySkills.Toxic,
					(byte)EnemySkills.Squint,
					(byte)EnemySkills.Glare,
					(byte)EnemySkills.Poison_Damage,
				},
			},
			new AlternateFiends
			{
				Name = "TONBERRY",
				ElementalWeakness = SpellElement.Fire,
				MonsterType = MonsterType.NONE,
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
				SpellChance2 = 0x40,
				Spells2 = new List<byte>
				{
					(byte)SpellByte.FAST,
					(byte)SpellByte.SABR,
					(byte)SpellByte.FOG,
					(byte)SpellByte.SLP2,
					(byte)SpellByte.TMPR,
					(byte)SpellByte.STOP,
					(byte)SpellByte.SLO2,
					(byte)SpellByte.SABR,
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
				Name = "TRITOCH",
				ElementalWeakness = SpellElement.Fire,
				MonsterType = MonsterType.MAGICAL,
				SpellChance1 = 0x40,
				Spells1 = new List<byte>
				{
					(byte)SpellByte.ICE2,
					(byte)SpellByte.MUTE,
					(byte)SpellByte.ICE2,
					(byte)SpellByte.STUN,
					(byte)SpellByte.ICE2,
					(byte)SpellByte.MUTE,
					(byte)SpellByte.ICE2,
					(byte)SpellByte.SLOW,
				},
				SkillChance1 = 0x40,
				Skills1 = new List<byte>
				{
					(byte)EnemySkills.Frost,
					(byte)EnemySkills.Frost,
					(byte)EnemySkills.Stare,
					(byte)EnemySkills.Trance,
				},
				SpellChance2 = 0x40,
				Spells2 = new List<byte>
				{
					(byte)SpellByte.ICE3,
					(byte)SpellByte.SLO2,
					(byte)SpellByte.ICE3,
					(byte)SpellByte.CUR3,
					(byte)SpellByte.ICE3,
					(byte)SpellByte.QAKE,
					(byte)SpellByte.ICE3,
					(byte)SpellByte.SLP2,
				},
				SkillChance2 = 0x40,
				Skills2 = new List<byte>
				{
					(byte)EnemySkills.Blizzard,
					(byte)EnemySkills.Squint,
					(byte)EnemySkills.Blizzard,
					(byte)EnemySkills.Crack,
				},
			},
			new AlternateFiends
			{
				Name = "TYPHON",
				ElementalWeakness = SpellElement.Ice,
				MonsterType = MonsterType.MAGICAL,
				SpellChance1 = 0x40,
				Spells1 = new List<byte>
				{
					(byte)SpellByte.FIR2,
					(byte)SpellByte.STUN,
					(byte)SpellByte.FIR2,
					(byte)SpellByte.SLOW,
					(byte)SpellByte.FIR2,
					(byte)SpellByte.INVS,
					(byte)SpellByte.DARK,
					(byte)SpellByte.FIR2,
				},
				SkillChance1 = 0x40,
				Skills1 = new List<byte>
				{
					(byte)EnemySkills.Heat,
					(byte)EnemySkills.Cremate,
					(byte)EnemySkills.Stinger,
					(byte)EnemySkills.Gaze,
				},
				SpellChance2 = 0x40,
				Spells2 = new List<byte>
				{
					(byte)SpellByte.FIR3,
					(byte)SpellByte.STOP,
					(byte)SpellByte.FIR3,
					(byte)SpellByte.NUKE,
					(byte)SpellByte.SLO2,
					(byte)SpellByte.FIR3,
					(byte)SpellByte.STOP,
					(byte)SpellByte.SLO2,
				},
				SkillChance2 = 0x40,
				Skills2 = new List<byte>
				{
					(byte)EnemySkills.Inferno,
					(byte)EnemySkills.Tornado,
					(byte)EnemySkills.Blaze,
					(byte)EnemySkills.Squint,
				},
			},
			new AlternateFiends
			{
				Name = "ULTROS",
				ElementalWeakness = SpellElement.Fire,
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
					(byte)EnemySkills.Ink,
					(byte)EnemySkills.Ink,
					(byte)EnemySkills.Trance,
					(byte)EnemySkills.Ink,
				},
				SpellChance2 = 0x40,
				Spells2 = new List<byte>
				{
					(byte)SpellByte.FAST,
					(byte)SpellByte.ICE3,
					(byte)SpellByte.BRAK,
					(byte)SpellByte.LIT3,
					(byte)SpellByte.WALL,
					(byte)SpellByte.FIR3,
					(byte)SpellByte.FOG,
					(byte)SpellByte.LIT3,
				},
				SkillChance2 = 0x40,
				Skills2 = new List<byte>
				{
					(byte)EnemySkills.Crack,
					(byte)EnemySkills.Swirl,
					(byte)EnemySkills.Ink,
					(byte)EnemySkills.Swirl,
				},
			},
			new AlternateFiends
			{
				Name = "VARGAS",
				ElementalWeakness = SpellElement.Poison,
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
				SkillChance2 = 0x40,
				Skills2 = new List<byte>
				{
					(byte)EnemySkills.Tornado,
					(byte)EnemySkills.Poison_Stone,
					(byte)EnemySkills.Toxic,
					(byte)EnemySkills.Tornado,
				},
			},
			new AlternateFiends
			{
				Name = "WELK",
				ElementalWeakness = SpellElement.None,
				MonsterType = MonsterType.MAGICAL,
				SpellChance1 = 0x40,
				Spells1 = new List<byte>
				{
					(byte)SpellByte.SLOW,
					(byte)SpellByte.LIT,
					(byte)SpellByte.SLOW,
					(byte)SpellByte.LIT2,
					(byte)SpellByte.FOG,
					(byte)SpellByte.LIT2,
					(byte)SpellByte.INVS,
					(byte)SpellByte.LIT2,
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
					(byte)SpellByte.LIT3,
					(byte)SpellByte.SLO2,
					(byte)SpellByte.LIT3,
					(byte)SpellByte.FOG2,
					(byte)SpellByte.WALL,
					(byte)SpellByte.LIT3,
					(byte)SpellByte.CUR3,
					(byte)SpellByte.LIT3,
				},
				SkillChance2 = 0x40,
				Skills2 = new List<byte>
				{
					(byte)EnemySkills.Thunder,
					(byte)EnemySkills.Poison_Stone,
					(byte)EnemySkills.Glance,
					(byte)EnemySkills.Thunder,
				},
			},
			new AlternateFiends
			{
				Name = "WREXSOUL",
				ElementalWeakness = SpellElement.Ice,
				MonsterType = MonsterType.UNDEAD,
				SpellChance1 = 0x40,
				Spells1 = new List<byte>
				{
					(byte)SpellByte.LIT,
					(byte)SpellByte.LIT2,
					(byte)SpellByte.RUB,
					(byte)SpellByte.LIT2,
					(byte)SpellByte.STUN,
					(byte)SpellByte.DARK,
					(byte)SpellByte.LIT2,
					(byte)SpellByte.LIT2,
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
					(byte)SpellByte.LIT3,
					(byte)SpellByte.XXXX,
					(byte)SpellByte.SLO2,
					(byte)SpellByte.LIT3,
					(byte)SpellByte.BANE,
					(byte)SpellByte.XXXX,
					(byte)SpellByte.LIT3,
					(byte)SpellByte.ZAP,
				},
				SkillChance2 = 0x40,
				Skills2 = new List<byte>
				{
					(byte)EnemySkills.Thunder,
					(byte)EnemySkills.Toxic,
					(byte)EnemySkills.Crack,
					(byte)EnemySkills.Blaze,
				},
			},
			new AlternateFiends
			{
				Name = "YETI",
				ElementalWeakness = SpellElement.Fire,
				MonsterType = MonsterType.GIANT,
				SpellChance1 = 0x40,
				Spells1 = new List<byte>
				{
					(byte)SpellByte.ICE2,
					(byte)SpellByte.CUR2,
					(byte)SpellByte.ICE2,
					(byte)SpellByte.TMPR,
					(byte)SpellByte.CUR2,
					(byte)SpellByte.ICE2,
					(byte)SpellByte.ICE2,
					(byte)SpellByte.TMPR,
				},
				SkillChance1 = 0x40,
				Skills1 = new List<byte>
				{
					(byte)EnemySkills.Frost,
					(byte)EnemySkills.Frost,
					(byte)EnemySkills.Frost,
					(byte)EnemySkills.Stare,
				},
				SpellChance2 = 0x40,
				Spells2 = new List<byte>
				{
					(byte)SpellByte.ICE3,
					(byte)SpellByte.CUR4,
					(byte)SpellByte.FAST,
					(byte)SpellByte.ICE3,
					(byte)SpellByte.SABR,
					(byte)SpellByte.ICE3,
					(byte)SpellByte.TMPR,
					(byte)SpellByte.ICE3,
				},
				SkillChance2 = 0x40,
				Skills2 = new List<byte>
				{
					(byte)EnemySkills.Blizzard,
					(byte)EnemySkills.Blizzard,
					(byte)EnemySkills.Glance,
					(byte)EnemySkills.Crack,
				},
			},
			new AlternateFiends
			{
				Name = "ZIGFRIED",
				ElementalWeakness = SpellElement.Fire,
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
					(byte)EnemySkills.None,
					(byte)EnemySkills.None,
					(byte)EnemySkills.None,
					(byte)EnemySkills.None,
				},
			},
		};
	}
}
