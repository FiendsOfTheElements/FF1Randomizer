namespace FF1Lib
{
	public partial class FF1Rom
	{
		List<AlternateFiends> FF4AltFiendsList = new()
		{
			new AlternateFiends
			{
				Name = "ANTLION",
				ElementalWeakness = SpellElement.Ice,
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
					(byte)EnemySkills.Crack,
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
				Name = "ASURA",
				ElementalWeakness = SpellElement.None,
				MonsterType = MonsterType.MAGE, //Changed from REGENERATIVE to MAGE
				SpellChance1 = 0x40,
				Spells1 = new List<byte>
				{
					(byte)SpellByte.CUR2,
					(byte)SpellByte.CUR3,
					(byte)SpellByte.CUR3,
					(byte)SpellByte.FAST,
					(byte)SpellByte.CUR2,
					(byte)SpellByte.CUR3,
					(byte)SpellByte.CUR3,
					(byte)SpellByte.FAST,
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
					(byte)SpellByte.CUR2,
					(byte)SpellByte.CUR3,
					(byte)SpellByte.CUR2,
					(byte)SpellByte.FAST,
					(byte)SpellByte.CUR3,
					(byte)SpellByte.CUR2,
					(byte)SpellByte.CUR3,
					(byte)SpellByte.CUR4,
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
				Name = "BAIGAN",
				ElementalWeakness = SpellElement.Status,
				MonsterType = MonsterType.MAGICAL,
				SpellChance1 = 0x40,
				Spells1 = new List<byte>
				{
					(byte)SpellByte.SLOW,
					(byte)SpellByte.LIT,
					(byte)SpellByte.ICE2,
					(byte)SpellByte.WALL,
					(byte)SpellByte.FIR2,
					(byte)SpellByte.STOP,
					(byte)SpellByte.TMPR,
					(byte)SpellByte.FIR2,
				},
				SkillChance1 = 0x40,
				Skills1 = new List<byte>
				{
					(byte)EnemySkills.Stinger,
					(byte)EnemySkills.Dazzle,
					(byte)EnemySkills.Cremate,
					(byte)EnemySkills.Frost,
				},
				SpellChance2 = 0x40,
				Spells2 = new List<byte>
				{
					(byte)SpellByte.FIR3,
					(byte)SpellByte.WALL,
					(byte)SpellByte.FAST,
					(byte)SpellByte.ICE3,
					(byte)SpellByte.SABR,
					(byte)SpellByte.SLO2,
					(byte)SpellByte.FOG,
					(byte)SpellByte.LIT3,
				},
				SkillChance2 = 0x40,
				Skills2 = new List<byte>
				{
					(byte)EnemySkills.Poison_Stone,
					(byte)EnemySkills.Crack,
					(byte)EnemySkills.Stare,
					(byte)EnemySkills.Stinger,
				},
			},
			new AlternateFiends
			{
				Name = "BALNAB",
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
					(byte)EnemySkills.Cremate,
					(byte)EnemySkills.Toxic,
					(byte)EnemySkills.Cremate,
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
					(byte)EnemySkills.Poison_Damage,
					(byte)EnemySkills.Thunder,
					(byte)EnemySkills.Nuclear,
					(byte)EnemySkills.Toxic,
				},
			},
			new AlternateFiends
			{
				Name = "BARBRICA",
				ElementalWeakness = SpellElement.Earth,
				MonsterType = MonsterType.MAGICAL,
				SpellChance1 = 0x40,
				Spells1 = new List<byte>
				{
					(byte)SpellByte.LIT,
					(byte)SpellByte.DARK,
					(byte)SpellByte.SLOW,
					(byte)SpellByte.LIT2,
					(byte)SpellByte.LIT,
					(byte)SpellByte.DARK,
					(byte)SpellByte.SLOW,
					(byte)SpellByte.LIT2,
				},
				SkillChance1 = 0x40,
				Skills1 = new List<byte>
				{
					(byte)EnemySkills.Heat,
					(byte)EnemySkills.Flash,
					(byte)EnemySkills.Gaze,
					(byte)EnemySkills.Heat,
				},
				SpellChance2 = 0x40,
				Spells2 = new List<byte>
				{
					(byte)SpellByte.LIT3,
					(byte)SpellByte.LIT2,
					(byte)SpellByte.SLOW,
					(byte)SpellByte.LIT3,
					(byte)SpellByte.LIT2,
					(byte)SpellByte.SLO2,
					(byte)SpellByte.DARK,
					(byte)SpellByte.DARK,
				},
				SkillChance2 = 0x40,
				Skills2 = new List<byte>
				{
					(byte)EnemySkills.Flash,
					(byte)EnemySkills.Thunder,
					(byte)EnemySkills.Glare,
					(byte)EnemySkills.Thunder,
				},
			},
			new AlternateFiends
			{
				Name = "CAGNAZZO",
				ElementalWeakness = SpellElement.Lightning,
				MonsterType = MonsterType.AQUATIC,
				SpellChance1 = 0x40,
				Spells1 = new List<byte>
				{
					(byte)SpellByte.ICE,
					(byte)SpellByte.STUN,
					(byte)SpellByte.ICE,
					(byte)SpellByte.ICE2,
					(byte)SpellByte.ICE,
					(byte)SpellByte.STUN,
					(byte)SpellByte.ICE,
					(byte)SpellByte.ICE2,
				},
				SkillChance1 = 0x40,
				Skills1 = new List<byte>
				{
					(byte)EnemySkills.Frost,
					(byte)EnemySkills.Flash,
					(byte)EnemySkills.Frost,
					(byte)EnemySkills.Flash,
				},
				SpellChance2 = 0x40,
				Spells2 = new List<byte>
				{
					(byte)SpellByte.ICE2,
					(byte)SpellByte.ICE2,
					(byte)SpellByte.ICE3,
					(byte)SpellByte.CUR3,
					(byte)SpellByte.ICE2,
					(byte)SpellByte.ICE3,
					(byte)SpellByte.ICE3,
					(byte)SpellByte.CUR4,
				},
				SkillChance2 = 0x40,
				Skills2 = new List<byte>
				{
					(byte)EnemySkills.Frost,
					(byte)EnemySkills.Dazzle,
					(byte)EnemySkills.Frost,
					(byte)EnemySkills.Blizzard,
				},
			},
			new AlternateFiends
			{
				Name = "CALCABRN",
				ElementalWeakness = SpellElement.Fire,
				MonsterType = MonsterType.GIANT,
				SpellChance1 = 0x40,
				Spells1 = new List<byte>
				{
					(byte)SpellByte.FAST,
					(byte)SpellByte.LIT,
					(byte)SpellByte.LIT2,
					(byte)SpellByte.SLOW,
					(byte)SpellByte.FOG,
					(byte)SpellByte.BLND,
					(byte)SpellByte.BRAK,
					(byte)SpellByte.ICE2,
				},
				SkillChance1 = 0x40,
				Skills1 = new List<byte>
				{
					(byte)EnemySkills.Trance,
					(byte)EnemySkills.Frost,
					(byte)EnemySkills.Gaze,
					(byte)EnemySkills.Stare,
				},
				SpellChance2 = 0x40,
				Spells2 = new List<byte>
				{
					(byte)SpellByte.SABR,
					(byte)SpellByte.ICE3,
					(byte)SpellByte.SLO2,
					(byte)SpellByte.INVS,
					(byte)SpellByte.LIT3,
					(byte)SpellByte.XXXX,
					(byte)SpellByte.MUTE,
					(byte)SpellByte.CUR3,
				},
				SkillChance2 = 0x40,
				Skills2 = new List<byte>
				{
					(byte)EnemySkills.Blizzard,
					(byte)EnemySkills.Crack,
					(byte)EnemySkills.Poison_Damage,
					(byte)EnemySkills.Squint,
				},
			},
			new AlternateFiends
			{
				Name = "D.MIST",
				ElementalWeakness = SpellElement.Fire,
				MonsterType = MonsterType.DRAGON,
				SpellChance1 = 0x40,
				Spells1 = new List<byte>
				{
					(byte)SpellByte.ICE,
					(byte)SpellByte.ICE2,
					(byte)SpellByte.SLOW,
					(byte)SpellByte.FOG,
					(byte)SpellByte.FOG,
					(byte)SpellByte.ICE2,
					(byte)SpellByte.DARK,
					(byte)SpellByte.CUR2,
				},
				SkillChance1 = 0x40,
				Skills1 = new List<byte>
				{
					(byte)EnemySkills.Snorting,
					(byte)EnemySkills.Frost,
					(byte)EnemySkills.Frost,
					(byte)EnemySkills.Gaze,
				},
				SpellChance2 = 0x40,
				Spells2 = new List<byte>
				{
					(byte)SpellByte.ICE3,
					(byte)SpellByte.SLO2,
					(byte)SpellByte.ICE2,
					(byte)SpellByte.FOG,
					(byte)SpellByte.INVS,
					(byte)SpellByte.ICE3,
					(byte)SpellByte.SLO2,
					(byte)SpellByte.FOG,
				},
				SkillChance2 = 0x40,
				Skills2 = new List<byte>
				{
					(byte)EnemySkills.Blizzard,
					(byte)EnemySkills.Tornado,
					(byte)EnemySkills.Dazzle,
					(byte)EnemySkills.Squint,
				},
			},
			new AlternateFiends
			{
				Name = "D.STORM",
				ElementalWeakness = SpellElement.Time,
				MonsterType = MonsterType.DRAGON,
				SpellChance1 = 0x40,
				Spells1 = new List<byte>
				{
					(byte)SpellByte.LIT,
					(byte)SpellByte.FAST,
					(byte)SpellByte.LIT2,
					(byte)SpellByte.SLOW,
					(byte)SpellByte.LIT2,
					(byte)SpellByte.LOCK,
					(byte)SpellByte.LIT2,
					(byte)SpellByte.SLOW,
				},
				SkillChance1 = 0x40,
				Skills1 = new List<byte>
				{
					(byte)EnemySkills.Glare,
					(byte)EnemySkills.Dazzle,
					(byte)EnemySkills.Trance,
					(byte)EnemySkills.Stare,
				},
				SpellChance2 = 0x40,
				Spells2 = new List<byte>
				{
					(byte)SpellByte.LIT3,
					(byte)SpellByte.FAST,
					(byte)SpellByte.HEL3,
					(byte)SpellByte.SABR,
					(byte)SpellByte.XXXX,
					(byte)SpellByte.LIT3,
					(byte)SpellByte.LOK2,
					(byte)SpellByte.ZAP,
				},
				SkillChance2 = 0x40,
				Skills2 = new List<byte>
				{
					(byte)EnemySkills.Tornado,
					(byte)EnemySkills.Thunder,
					(byte)EnemySkills.Glare,
					(byte)EnemySkills.Poison_Stone,
				},
			},
			new AlternateFiends
			{
				Name = "DARKELF",
				ElementalWeakness = SpellElement.Poison,
				MonsterType = MonsterType.GIANT,
				SpellChance1 = 0x40,
				Spells1 = new List<byte>
				{
					(byte)SpellByte.FIRE,
					(byte)SpellByte.ICE,
					(byte)SpellByte.LIT2,
					(byte)SpellByte.MUTE,
					(byte)SpellByte.STUN,
					(byte)SpellByte.FIR2,
					(byte)SpellByte.LIT2,
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
					(byte)SpellByte.LIT3,
					(byte)SpellByte.ICE3,
					(byte)SpellByte.FIR3,
					(byte)SpellByte.INVS,
					(byte)SpellByte.XXXX,
					(byte)SpellByte.WALL,
					(byte)SpellByte.CUR3,
					(byte)SpellByte.SLO2,
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
				Name = "DETHMACH",
				ElementalWeakness = SpellElement.Earth,
				MonsterType = MonsterType.MAGICAL,
				SpellChance1 = 0x40,
				Spells1 = new List<byte>
				{
					(byte)SpellByte.BANE,
					(byte)SpellByte.FOG,
					(byte)SpellByte.BRAK,
					(byte)SpellByte.FAST,
					(byte)SpellByte.STUN,
					(byte)SpellByte.HOLD,
					(byte)SpellByte.QAKE,
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
					(byte)SpellByte.XXXX,
					(byte)SpellByte.XFER,
					(byte)SpellByte.ZAP,
					(byte)SpellByte.XFER,
					(byte)SpellByte.XXXX,
					(byte)SpellByte.QAKE,
					(byte)SpellByte.BANE,
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
				Name = "EVILWALL",
				ElementalWeakness = SpellElement.Earth,
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
					(byte)EnemySkills.Gaze,
					(byte)EnemySkills.Glance,
					(byte)EnemySkills.Dazzle,
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
					(byte)EnemySkills.Poison_Damage,
					(byte)EnemySkills.Toxic,
					(byte)EnemySkills.Squint,
					(byte)EnemySkills.Nuclear,
				},
			},
			new AlternateFiends
			{
				Name = "FLANMAST",
				ElementalWeakness = SpellElement.Status,
				MonsterType = MonsterType.MAGE,
				SpellChance1 = 0x40,
				Spells1 = new List<byte>
				{
					(byte)SpellByte.FIR2,
					(byte)SpellByte.INVS,
					(byte)SpellByte.FIR2,
					(byte)SpellByte.MUTE,
					(byte)SpellByte.RUB,
					(byte)SpellByte.SLOW,
					(byte)SpellByte.FIR2,
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
					(byte)SpellByte.BANE,
					(byte)SpellByte.XXXX,
					(byte)SpellByte.FIR3,
					(byte)SpellByte.SLO2,
					(byte)SpellByte.BRAK,
					(byte)SpellByte.WALL,
					(byte)SpellByte.FIR3,
					(byte)SpellByte.INVS,
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
				Name = "GIGAWORM",
				ElementalWeakness = SpellElement.Ice,
				MonsterType = MonsterType.NONE,
				SpellChance1 = 0x40,
				Spells1 = new List<byte>
				{
					(byte)SpellByte.LIT2,
					(byte)SpellByte.FIR2,
					(byte)SpellByte.QAKE,
					(byte)SpellByte.STUN,
					(byte)SpellByte.MUTE,
					(byte)SpellByte.LIT2,
					(byte)SpellByte.FIR2,
					(byte)SpellByte.RUB,
				},
				SkillChance1 = 0x40,
				Skills1 = new List<byte>
				{
					(byte)EnemySkills.Crack,
					(byte)EnemySkills.Cremate,
					(byte)EnemySkills.Dazzle,
					(byte)EnemySkills.Trance,
				},
				SpellChance2 = 0x40,
				Spells2 = new List<byte>
				{
					(byte)SpellByte.QAKE,
					(byte)SpellByte.FAST,
					(byte)SpellByte.FIR3,
					(byte)SpellByte.SLP2,
					(byte)SpellByte.XXXX,
					(byte)SpellByte.INVS,
					(byte)SpellByte.FIR3,
					(byte)SpellByte.SLO2,
				},
				SkillChance2 = 0x40,
				Skills2 = new List<byte>
				{
					(byte)EnemySkills.Nuclear,
					(byte)EnemySkills.Poison_Stone,
					(byte)EnemySkills.Blaze,
					(byte)EnemySkills.Dazzle,
				},
			},
			new AlternateFiends
			{
				Name = "GOLBEZ",
				ElementalWeakness = SpellElement.Fire,
				MonsterType = MonsterType.MAGICAL,
				SpellChance1 = 0x40,
				Spells1 = new List<byte>
				{
					(byte)SpellByte.QAKE,
					(byte)SpellByte.ICE2,
					(byte)SpellByte.RUB,
					(byte)SpellByte.LIT2,
					(byte)SpellByte.FIRE,
					(byte)SpellByte.FIR2,
					(byte)SpellByte.LIT,
					(byte)SpellByte.ICE,
				},
				SkillChance1 = 0x40,
				Skills1 = new List<byte>
				{
					(byte)EnemySkills.Cremate,
					(byte)EnemySkills.Frost,
					(byte)EnemySkills.Gaze,
					(byte)EnemySkills.Stare,
				},
				SpellChance2 = 0x40,
				Spells2 = new List<byte>
				{
					(byte)SpellByte.NUKE,
					(byte)SpellByte.INVS,
					(byte)SpellByte.XXXX,
					(byte)SpellByte.ICE3,
					(byte)SpellByte.XFER,
					(byte)SpellByte.BRAK,
					(byte)SpellByte.FIR3,
					(byte)SpellByte.STOP,
				},
				SkillChance2 = 0x40,
				Skills2 = new List<byte>
				{
					(byte)EnemySkills.Blaze,
					(byte)EnemySkills.Inferno,
					(byte)EnemySkills.Swirl,
					(byte)EnemySkills.Poison_Stone,
				},
			},
			new AlternateFiends
			{
				Name = "IFRIT",
				ElementalWeakness = SpellElement.Ice,
				MonsterType = MonsterType.MAGICAL,
				SpellChance1 = 0x40,
				Spells1 = new List<byte>
				{
					(byte)SpellByte.FIRE,
					(byte)SpellByte.STUN,
					(byte)SpellByte.FIRE,
					(byte)SpellByte.FIR2,
					(byte)SpellByte.FIRE,
					(byte)SpellByte.STUN,
					(byte)SpellByte.FIRE,
					(byte)SpellByte.FIR2,
				},
				SkillChance1 = 0x40,
				Skills1 = new List<byte>
				{
					(byte)EnemySkills.Heat,
					(byte)EnemySkills.Scorch,
					(byte)EnemySkills.Heat,
					(byte)EnemySkills.Flash,
				},
				SpellChance2 = 0x40,
				Spells2 = new List<byte>
				{
					(byte)SpellByte.FIR2,
					(byte)SpellByte.FIR3,
					(byte)SpellByte.XFER,
					(byte)SpellByte.FIR2,
					(byte)SpellByte.FIR3,
					(byte)SpellByte.WALL,
					(byte)SpellByte.FIR3,
					(byte)SpellByte.BANE,
				},
				SkillChance2 = 0x40,
				Skills2 = new List<byte>
				{
					(byte)EnemySkills.Inferno,
					(byte)EnemySkills.Flash,
					(byte)EnemySkills.Blaze,
					(byte)EnemySkills.Cremate,
				},
			},
			new AlternateFiends
			{
				Name = "LUGAE",
				ElementalWeakness = SpellElement.Fire,
				MonsterType = MonsterType.UNDEAD,
				SpellChance1 = 0x40,
				Spells1 = new List<byte>
				{
					(byte)SpellByte.LIT,
					(byte)SpellByte.FIR2,
					(byte)SpellByte.QAKE,
					(byte)SpellByte.STUN,
					(byte)SpellByte.MUTE,
					(byte)SpellByte.LIT2,
					(byte)SpellByte.FIRE,
					(byte)SpellByte.RUB,
				},
				SkillChance1 = 0x40,
				Skills1 = new List<byte>
				{
					(byte)EnemySkills.Crack,
					(byte)EnemySkills.Cremate,
					(byte)EnemySkills.Dazzle,
					(byte)EnemySkills.Trance,
				},
				SpellChance2 = 0x40,
				Spells2 = new List<byte>
				{
					(byte)SpellByte.QAKE,
					(byte)SpellByte.FAST,
					(byte)SpellByte.FIR3,
					(byte)SpellByte.SLP2,
					(byte)SpellByte.XXXX,
					(byte)SpellByte.INVS,
					(byte)SpellByte.FIR3,
					(byte)SpellByte.SLO2,
				},
				SkillChance2 = 0x40,
				Skills2 = new List<byte>
				{
					(byte)EnemySkills.Nuclear,
					(byte)EnemySkills.Poison_Stone,
					(byte)EnemySkills.Gaze,
					(byte)EnemySkills.Dazzle,
				},
			},
			new AlternateFiends
			{
				Name = "MOMBOMB",
				ElementalWeakness = SpellElement.Ice,
				MonsterType = MonsterType.NONE,
				SpellChance1 = 0x40,
				Spells1 = new List<byte>
				{
					(byte)SpellByte.FIRE,
					(byte)SpellByte.FIR2,
					(byte)SpellByte.RUB,
					(byte)SpellByte.SLOW,
					(byte)SpellByte.FIR2,
					(byte)SpellByte.FIR2,
					(byte)SpellByte.STUN,
					(byte)SpellByte.BLND,
				},
				SkillChance1 = 0x40,
				Skills1 = new List<byte>
				{
					(byte)EnemySkills.Heat,
					(byte)EnemySkills.Cremate,
					(byte)EnemySkills.Trance,
					(byte)EnemySkills.Heat,
				},
				SpellChance2 = 0x40,
				Spells2 = new List<byte>
				{
					(byte)SpellByte.FIR3,
					(byte)SpellByte.NUKE,
					(byte)SpellByte.MUTE,
					(byte)SpellByte.RUB,
					(byte)SpellByte.FIR3,
					(byte)SpellByte.FAST,
					(byte)SpellByte.CUR3,
					(byte)SpellByte.SLOW,
				},
				SkillChance2 = 0x40,
				Skills2 = new List<byte>
				{
					(byte)EnemySkills.Glance,
					(byte)EnemySkills.Cremate,
					(byte)EnemySkills.Inferno,
					(byte)EnemySkills.Trance,
				},
			},
			new AlternateFiends
			{
				Name = "OCTOMAM",
				ElementalWeakness = SpellElement.Lightning,
				MonsterType = MonsterType.AQUATIC,
				SpellChance1 = 0x40,
				Spells1 = new List<byte>
				{
					(byte)SpellByte.ICE,
					(byte)SpellByte.CUR2,
					(byte)SpellByte.ICE2,
					(byte)SpellByte.FOG,
					(byte)SpellByte.ICE2,
					(byte)SpellByte.STUN,
					(byte)SpellByte.SLEP,
					(byte)SpellByte.ICE2,
				},
				SkillChance1 = 0x40,
				Skills1 = new List<byte>
				{
					(byte)EnemySkills.Ink,
					(byte)EnemySkills.Stinger,
					(byte)EnemySkills.Ink,
					(byte)EnemySkills.Stare,
				},
				SpellChance2 = 0x40,
				Spells2 = new List<byte>
				{
					(byte)SpellByte.ICE3,
					(byte)SpellByte.ICE3,
					(byte)SpellByte.FAST,
					(byte)SpellByte.CUR4,
					(byte)SpellByte.SLO2,
					(byte)SpellByte.XXXX,
					(byte)SpellByte.TMPR,
					(byte)SpellByte.ICE3,
				},
				SkillChance2 = 0x40,
				Skills2 = new List<byte>
				{
					(byte)EnemySkills.Swirl,
					(byte)EnemySkills.Ink,
					(byte)EnemySkills.Blizzard,
					(byte)EnemySkills.Stinger,
				},
			},
			new AlternateFiends
			{
				Name = "ODIN",
				ElementalWeakness = SpellElement.Lightning,
				MonsterType = MonsterType.GIANT,
				SpellChance1 = 0x40,
				Spells1 = new List<byte>
				{
					(byte)SpellByte.STUN,
					(byte)SpellByte.CUR2,
					(byte)SpellByte.FAST,
					(byte)SpellByte.SLOW,
					(byte)SpellByte.STUN,
					(byte)SpellByte.CUR2,
					(byte)SpellByte.FAST,
					(byte)SpellByte.SLOW,
				},
				SkillChance1 = 0x40,
				Skills1 = new List<byte>
				{
					(byte)EnemySkills.Gaze,
					(byte)EnemySkills.Flash,
					(byte)EnemySkills.Flash,
					(byte)EnemySkills.Crack,
				},
				SpellChance2 = 0x40,
				Spells2 = new List<byte>
				{
					(byte)SpellByte.STUN,
					(byte)SpellByte.BANE,
					(byte)SpellByte.XXXX,
					(byte)SpellByte.SLO2,
					(byte)SpellByte.CUR3,
					(byte)SpellByte.STOP,
					(byte)SpellByte.SLOW,
					(byte)SpellByte.ZAP,
				},
				SkillChance2 = 0x40,
				Skills2 = new List<byte>
				{
					(byte)EnemySkills.Crack,
					(byte)EnemySkills.Gaze,
					(byte)EnemySkills.Flash,
					(byte)EnemySkills.Dazzle,
				},
			},
			new AlternateFiends
			{
				Name = "OGOPOGO",
				ElementalWeakness = SpellElement.Lightning,
				MonsterType = MonsterType.AQUATIC,
				SpellChance1 = 0x40,
				Spells1 = new List<byte>
				{
					(byte)SpellByte.ICE2,
					(byte)SpellByte.ICE,
					(byte)SpellByte.ICE2,
					(byte)SpellByte.FOG,
					(byte)SpellByte.HEL2,
					(byte)SpellByte.ICE2,
					(byte)SpellByte.SLOW,
					(byte)SpellByte.ICE2,
				},
				SkillChance1 = 0x40,
				Skills1 = new List<byte>
				{
					(byte)EnemySkills.Ink,
					(byte)EnemySkills.Frost,
					(byte)EnemySkills.Ink,
					(byte)EnemySkills.Frost,
				},
				SpellChance2 = 0x40,
				Spells2 = new List<byte>
				{
					(byte)SpellByte.ICE3,
					(byte)SpellByte.ICE2,
					(byte)SpellByte.INVS,
					(byte)SpellByte.BLND,
					(byte)SpellByte.ICE2,
					(byte)SpellByte.BRAK,
					(byte)SpellByte.WALL,
					(byte)SpellByte.ICE3,
				},
				SkillChance2 = 0x40,
				Skills2 = new List<byte>
				{
					(byte)EnemySkills.Swirl,
					(byte)EnemySkills.Ink,
					(byte)EnemySkills.Blizzard,
					(byte)EnemySkills.Frost,
				},
			},
			new AlternateFiends
			{
				Name = "PALEDIM",
				ElementalWeakness = SpellElement.None,
				MonsterType = MonsterType.DRAGON,
				SpellChance1 = 0x40,
				Spells1 = new List<byte>
				{
					(byte)SpellByte.ICE2,
					(byte)SpellByte.FIR2,
					(byte)SpellByte.LIT2,
					(byte)SpellByte.SLOW,
					(byte)SpellByte.ICE2,
					(byte)SpellByte.FIR2,
					(byte)SpellByte.LIT2,
					(byte)SpellByte.SLOW,
				},
				SkillChance1 = 0x40,
				Skills1 = new List<byte>
				{
					(byte)EnemySkills.Crack,
					(byte)EnemySkills.Frost,
					(byte)EnemySkills.Cremate,
					(byte)EnemySkills.Trance,
				},
				SpellChance2 = 0x40,
				Spells2 = new List<byte>
				{
					(byte)SpellByte.ICE3,
					(byte)SpellByte.FIR2,
					(byte)SpellByte.BRAK,
					(byte)SpellByte.SLO2,
					(byte)SpellByte.FOG,
					(byte)SpellByte.LIT3,
					(byte)SpellByte.FAST,
					(byte)SpellByte.SLO2,
				},
				SkillChance2 = 0x40,
				Skills2 = new List<byte>
				{
					(byte)EnemySkills.Thunder,
					(byte)EnemySkills.Inferno,
					(byte)EnemySkills.Blizzard,
					(byte)EnemySkills.Crack,
				},
			},
			new AlternateFiends
			{
				Name = "PLAGUE",
				ElementalWeakness = SpellElement.Status,
				MonsterType = MonsterType.MAGE,
				SpellChance1 = 0x40,
				Spells1 = new List<byte>
				{
					(byte)SpellByte.BRAK,
					(byte)SpellByte.SLOW,
					(byte)SpellByte.QAKE,
					(byte)SpellByte.SLOW,
					(byte)SpellByte.INVS,
					(byte)SpellByte.RUB,
					(byte)SpellByte.BRAK,
					(byte)SpellByte.SLOW,
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
					(byte)SpellByte.BRAK,
					(byte)SpellByte.QAKE,
					(byte)SpellByte.SLO2,
					(byte)SpellByte.INVS,
					(byte)SpellByte.RUB,
					(byte)SpellByte.ZAP,
					(byte)SpellByte.BANE,
				},
				SkillChance2 = 0x40,
				Skills2 = new List<byte>
				{
					(byte)EnemySkills.Poison_Stone,
					(byte)EnemySkills.Trance,
					(byte)EnemySkills.Toxic,
					(byte)EnemySkills.Crack,
				},
			},
			new AlternateFiends
			{
				Name = "RAMUH",
				ElementalWeakness = SpellElement.Earth,
				MonsterType = MonsterType.MAGE,
				SpellChance1 = 0x40,
				Spells1 = new List<byte>
				{
					(byte)SpellByte.LIT,
					(byte)SpellByte.LIT2,
					(byte)SpellByte.CUR2,
					(byte)SpellByte.LIT2,
					(byte)SpellByte.LIT,
					(byte)SpellByte.CUR2,
					(byte)SpellByte.LIT,
					(byte)SpellByte.LIT2,
				},
				SkillChance1 = 0x40,
				Skills1 = new List<byte>
				{
					(byte)EnemySkills.Stinger,
					(byte)EnemySkills.Flash,
					(byte)EnemySkills.Flash,
					(byte)EnemySkills.Stinger,
				},
				SpellChance2 = 0x40,
				Spells2 = new List<byte>
				{
					(byte)SpellByte.LIT2,
					(byte)SpellByte.STUN,
					(byte)SpellByte.LIT3,
					(byte)SpellByte.SLOW,
					(byte)SpellByte.CUR3,
					(byte)SpellByte.LIT2,
					(byte)SpellByte.STOP,
					(byte)SpellByte.LIT3,
				},
				SkillChance2 = 0x40,
				Skills2 = new List<byte>
				{
					(byte)EnemySkills.Thunder,
					(byte)EnemySkills.Stinger,
					(byte)EnemySkills.Flash,
					(byte)EnemySkills.Tornado,
				},
			},
			new AlternateFiends
			{
				Name = "RUBICANT",
				ElementalWeakness = SpellElement.Ice,
				MonsterType = MonsterType.MAGE,
				SpellChance1 = 0x40,
				Spells1 = new List<byte>
				{
					(byte)SpellByte.FIRE,
					(byte)SpellByte.FIRE,
					(byte)SpellByte.FIRE,
					(byte)SpellByte.FIR2,
					(byte)SpellByte.FIRE,
					(byte)SpellByte.FIRE,
					(byte)SpellByte.FIRE,
					(byte)SpellByte.FIR2,
				},
				SkillChance1 = 0x40,
				Skills1 = new List<byte>
				{
					(byte)EnemySkills.Heat,
					(byte)EnemySkills.Scorch,
					(byte)EnemySkills.Flash,
					(byte)EnemySkills.Dazzle,
				},
				SpellChance2 = 0x40,
				Spells2 = new List<byte>
				{
					(byte)SpellByte.FIR2,
					(byte)SpellByte.FIR2,
					(byte)SpellByte.FIR3,
					(byte)SpellByte.NUKE,
					(byte)SpellByte.FIR2,
					(byte)SpellByte.FIR3,
					(byte)SpellByte.FIR3,
					(byte)SpellByte.NUKE,
				},
				SkillChance2 = 0x40,
				Skills2 = new List<byte>
				{
					(byte)EnemySkills.Scorch,
					(byte)EnemySkills.Heat,
					(byte)EnemySkills.Blaze,
					(byte)EnemySkills.Nuclear,
				},
			},
			new AlternateFiends
			{
				Name = "SCARMLIO",
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
					(byte)EnemySkills.Snorting,
					(byte)EnemySkills.Gaze,
					(byte)EnemySkills.Ink,
					(byte)EnemySkills.Crack,
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
					(byte)EnemySkills.Snorting,
					(byte)EnemySkills.Crack,
					(byte)EnemySkills.Snorting,
				},
			},
			new AlternateFiends
			{
				Name = "SHADOW.D",
				ElementalWeakness = SpellElement.Earth,
				MonsterType = MonsterType.DRAGON,
				SpellChance1 = 0x40,
				Spells1 = new List<byte>
				{
					(byte)SpellByte.STUN,
					(byte)SpellByte.HOLD,
					(byte)SpellByte.RUB,
					(byte)SpellByte.STUN,
					(byte)SpellByte.HOLD,
					(byte)SpellByte.RUB,
					(byte)SpellByte.STUN,
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
					(byte)EnemySkills.Poison_Stone,
				},
			},
			new AlternateFiends
			{
				Name = "SHIVA",
				ElementalWeakness = SpellElement.Fire,
				MonsterType = MonsterType.MAGE,
				SpellChance1 = 0x40,
				Spells1 = new List<byte>
				{
					(byte)SpellByte.ICE,
					(byte)SpellByte.ICE,
					(byte)SpellByte.ICE,
					(byte)SpellByte.ICE2,
					(byte)SpellByte.ICE,
					(byte)SpellByte.ICE,
					(byte)SpellByte.ICE,
					(byte)SpellByte.ICE2,
				},
				SkillChance1 = 0x40,
				Skills1 = new List<byte>
				{
					(byte)EnemySkills.Frost,
					(byte)EnemySkills.Flash,
					(byte)EnemySkills.Gaze,
					(byte)EnemySkills.Snorting,
				},
				SpellChance2 = 0x40,
				Spells2 = new List<byte>
				{
					(byte)SpellByte.ICE2,
					(byte)SpellByte.ICE3,
					(byte)SpellByte.CUR3,
					(byte)SpellByte.ICE2,
					(byte)SpellByte.FOG,
					(byte)SpellByte.ICE3,
					(byte)SpellByte.STOP,
					(byte)SpellByte.BRAK,
				},
				SkillChance2 = 0x40,
				Skills2 = new List<byte>
				{
					(byte)EnemySkills.Frost,
					(byte)EnemySkills.Flash,
					(byte)EnemySkills.Blizzard,
					(byte)EnemySkills.Gaze,
				},
			},
			new AlternateFiends
			{
				Name = "TITAN",
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
					(byte)EnemySkills.Dazzle,
					(byte)EnemySkills.Gaze,
					(byte)EnemySkills.Dazzle,
					(byte)EnemySkills.Flash,
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
					(byte)EnemySkills.Crack,
					(byte)EnemySkills.Stare,
					(byte)EnemySkills.Glare,
				},
			},
			new AlternateFiends
			{
				Name = "WYVERN",
				ElementalWeakness = SpellElement.None,
				MonsterType = MonsterType.DRAGON,
				SpellChance1 = 0x40,
				Spells1 = new List<byte>
				{
					(byte)SpellByte.FIR2,
					(byte)SpellByte.SLOW,
					(byte)SpellByte.FIR2,
					(byte)SpellByte.INVS,
					(byte)SpellByte.FIR2,
					(byte)SpellByte.MUTE,
					(byte)SpellByte.FIR2,
					(byte)SpellByte.SLOW,
				},
				SkillChance1 = 0x40,
				Skills1 = new List<byte>
				{
					(byte)EnemySkills.Heat,
					(byte)EnemySkills.Cremate,
					(byte)EnemySkills.Trance,
					(byte)EnemySkills.Heat,
				},
				SpellChance2 = 0x40,
				Spells2 = new List<byte>
				{
					(byte)SpellByte.NUKE,
					(byte)SpellByte.INVS,
					(byte)SpellByte.FIR3,
					(byte)SpellByte.FAST,
					(byte)SpellByte.XXXX,
					(byte)SpellByte.BANE,
					(byte)SpellByte.SLO2,
					(byte)SpellByte.FIR3,
				},
				SkillChance2 = 0x40,
				Skills2 = new List<byte>
				{
					(byte)EnemySkills.Inferno,
					(byte)EnemySkills.Tornado,
					(byte)EnemySkills.Glare,
					(byte)EnemySkills.Tornado,
				},
			},
			new AlternateFiends
			{
				Name = "ZEMUS",
				ElementalWeakness = SpellElement.None,
				MonsterType = MonsterType.GIANT,
				SpellChance1 = 0x40,
				Spells1 = new List<byte>
				{
					(byte)SpellByte.FIR2,
					(byte)SpellByte.LIT2,
					(byte)SpellByte.ICE2,
					(byte)SpellByte.ZAP,
					(byte)SpellByte.SLOW,
					(byte)SpellByte.MUTE,
					(byte)SpellByte.QAKE,
					(byte)SpellByte.ICE2,
				},
				SkillChance1 = 0x40,
				Skills1 = new List<byte>
				{
					(byte)EnemySkills.Trance,
					(byte)EnemySkills.Cremate,
					(byte)EnemySkills.Frost,
					(byte)EnemySkills.Stare,
				},
				SpellChance2 = 0x40,
				Spells2 = new List<byte>
				{
					(byte)SpellByte.FAST,
					(byte)SpellByte.NUKE,
					(byte)SpellByte.FIR3,
					(byte)SpellByte.BANE,
					(byte)SpellByte.SABR,
					(byte)SpellByte.SLO2,
					(byte)SpellByte.FOG,
					(byte)SpellByte.CUR4,
				},
				SkillChance2 = 0x40,
				Skills2 = new List<byte>
				{
					(byte)EnemySkills.Swirl,
					(byte)EnemySkills.Poison_Stone,
					(byte)EnemySkills.Toxic,
					(byte)EnemySkills.Nuclear,
				},
			},
		};

		List<AlternateFiends> FF4AltFiendListHardcore = new()
		{
			new AlternateFiends
			{
				Name = "ANTLION",
				ElementalWeakness = SpellElement.Ice,
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
					(byte)EnemySkills.Crack,
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
				Name = "ASURA",
				ElementalWeakness = SpellElement.None,
				MonsterType = MonsterType.REGENERATIVE,
				SpellChance1 = 0x40,
				Spells1 = new List<byte>
				{
					(byte)SpellByte.CUR2,
					(byte)SpellByte.CUR3,
					(byte)SpellByte.CUR3,
					(byte)SpellByte.FAST,
					(byte)SpellByte.CUR2,
					(byte)SpellByte.CUR3,
					(byte)SpellByte.CUR3,
					(byte)SpellByte.FAST,
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
					(byte)SpellByte.CUR2,
					(byte)SpellByte.CUR3,
					(byte)SpellByte.CUR2,
					(byte)SpellByte.FAST,
					(byte)SpellByte.CUR3,
					(byte)SpellByte.CUR2,
					(byte)SpellByte.CUR3,
					(byte)SpellByte.CUR4,
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
				Name = "BAIGAN",
				ElementalWeakness = SpellElement.Status,
				MonsterType = MonsterType.MAGICAL,
				SpellChance1 = 0x40,
				Spells1 = new List<byte>
				{
					(byte)SpellByte.SLOW,
					(byte)SpellByte.LIT2,
					(byte)SpellByte.ICE2,
					(byte)SpellByte.WALL,
					(byte)SpellByte.FIR2,
					(byte)SpellByte.STOP,
					(byte)SpellByte.TMPR,
					(byte)SpellByte.FIR2,
				},
				SkillChance1 = 0x40,
				Skills1 = new List<byte>
				{
					(byte)EnemySkills.Stinger,
					(byte)EnemySkills.Dazzle,
					(byte)EnemySkills.Cremate,
					(byte)EnemySkills.Frost,
				},
				SpellChance2 = 0x40,
				Spells2 = new List<byte>
				{
					(byte)SpellByte.FIR3,
					(byte)SpellByte.WALL,
					(byte)SpellByte.FAST,
					(byte)SpellByte.ICE3,
					(byte)SpellByte.NUKE,
					(byte)SpellByte.SLO2,
					(byte)SpellByte.FOG,
					(byte)SpellByte.LIT3,
				},
				SkillChance2 = 0x40,
				Skills2 = new List<byte>
				{
					(byte)EnemySkills.Poison_Stone,
					(byte)EnemySkills.Crack,
					(byte)EnemySkills.Stare,
					(byte)EnemySkills.Stinger,
				},
			},
			new AlternateFiends
			{
				Name = "BALNAB",
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
					(byte)EnemySkills.Cremate,
					(byte)EnemySkills.Toxic,
					(byte)EnemySkills.Cremate,
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
					(byte)EnemySkills.Poison_Damage,
					(byte)EnemySkills.Thunder,
					(byte)EnemySkills.Nuclear,
					(byte)EnemySkills.Toxic,
				},
			},
			new AlternateFiends
			{
				Name = "BARBRICA",
				ElementalWeakness = SpellElement.Earth,
				MonsterType = MonsterType.MAGICAL,
				SpellChance1 = 0x40,
				Spells1 = new List<byte>
				{
					(byte)SpellByte.LIT,
					(byte)SpellByte.DARK,
					(byte)SpellByte.SLOW,
					(byte)SpellByte.LIT2,
					(byte)SpellByte.LIT,
					(byte)SpellByte.DARK,
					(byte)SpellByte.SLOW,
					(byte)SpellByte.LIT2,
				},
				SkillChance1 = 0x40,
				Skills1 = new List<byte>
				{
					(byte)EnemySkills.Heat,
					(byte)EnemySkills.Flash,
					(byte)EnemySkills.Gaze,
					(byte)EnemySkills.Heat,
				},
				SpellChance2 = 0x40,
				Spells2 = new List<byte>
				{
					(byte)SpellByte.LIT3,
					(byte)SpellByte.LIT2,
					(byte)SpellByte.SLOW,
					(byte)SpellByte.LIT3,
					(byte)SpellByte.LIT2,
					(byte)SpellByte.SLO2,
					(byte)SpellByte.DARK,
					(byte)SpellByte.DARK,
				},
				SkillChance2 = 0x40,
				Skills2 = new List<byte>
				{
					(byte)EnemySkills.Flash,
					(byte)EnemySkills.Thunder,
					(byte)EnemySkills.Glare,
					(byte)EnemySkills.Thunder,
				},
			},
			new AlternateFiends
			{
				Name = "CAGNAZZO",
				ElementalWeakness = SpellElement.Lightning,
				MonsterType = MonsterType.AQUATIC,
				SpellChance1 = 0x40,
				Spells1 = new List<byte>
				{
					(byte)SpellByte.ICE,
					(byte)SpellByte.STUN,
					(byte)SpellByte.ICE,
					(byte)SpellByte.ICE2,
					(byte)SpellByte.ICE,
					(byte)SpellByte.STUN,
					(byte)SpellByte.ICE,
					(byte)SpellByte.ICE2,
				},
				SkillChance1 = 0x40,
				Skills1 = new List<byte>
				{
					(byte)EnemySkills.Frost,
					(byte)EnemySkills.Flash,
					(byte)EnemySkills.Frost,
					(byte)EnemySkills.Flash,
				},
				SpellChance2 = 0x40,
				Spells2 = new List<byte>
				{
					(byte)SpellByte.ICE2,
					(byte)SpellByte.ICE2,
					(byte)SpellByte.ICE3,
					(byte)SpellByte.CUR3,
					(byte)SpellByte.ICE2,
					(byte)SpellByte.ICE3,
					(byte)SpellByte.ICE3,
					(byte)SpellByte.CUR4,
				},
				SkillChance2 = 0x40,
				Skills2 = new List<byte>
				{
					(byte)EnemySkills.Frost,
					(byte)EnemySkills.Dazzle,
					(byte)EnemySkills.Frost,
					(byte)EnemySkills.Blizzard,
				},
			},
			new AlternateFiends
			{
				Name = "CALCABRN",
				ElementalWeakness = SpellElement.Fire,
				MonsterType = MonsterType.GIANT,
				SpellChance1 = 0x40,
				Spells1 = new List<byte>
				{
					(byte)SpellByte.FAST,
					(byte)SpellByte.LIT,
					(byte)SpellByte.LIT2,
					(byte)SpellByte.SLOW,
					(byte)SpellByte.FOG,
					(byte)SpellByte.BLND,
					(byte)SpellByte.BRAK,
					(byte)SpellByte.ICE2,
				},
				SkillChance1 = 0x40,
				Skills1 = new List<byte>
				{
					(byte)EnemySkills.Trance,
					(byte)EnemySkills.Frost,
					(byte)EnemySkills.Gaze,
					(byte)EnemySkills.Stare,
				},
				SpellChance2 = 0x40,
				Spells2 = new List<byte>
				{
					(byte)SpellByte.SABR,
					(byte)SpellByte.ICE3,
					(byte)SpellByte.SLO2,
					(byte)SpellByte.INVS,
					(byte)SpellByte.LIT3,
					(byte)SpellByte.XXXX,
					(byte)SpellByte.MUTE,
					(byte)SpellByte.CUR4,
				},
				SkillChance2 = 0x40,
				Skills2 = new List<byte>
				{
					(byte)EnemySkills.Blizzard,
					(byte)EnemySkills.Crack,
					(byte)EnemySkills.Poison_Damage,
					(byte)EnemySkills.Squint,
				},
			},
			new AlternateFiends
			{
				Name = "D.MIST",
				ElementalWeakness = SpellElement.Fire,
				MonsterType = MonsterType.DRAGON,
				SpellChance1 = 0x40,
				Spells1 = new List<byte>
				{
					(byte)SpellByte.ICE,
					(byte)SpellByte.ICE2,
					(byte)SpellByte.SLOW,
					(byte)SpellByte.FOG,
					(byte)SpellByte.FOG,
					(byte)SpellByte.ICE2,
					(byte)SpellByte.DARK,
					(byte)SpellByte.CUR2,
				},
				SkillChance1 = 0x40,
				Skills1 = new List<byte>
				{
					(byte)EnemySkills.Snorting,
					(byte)EnemySkills.Frost,
					(byte)EnemySkills.Frost,
					(byte)EnemySkills.Gaze,
				},
				SpellChance2 = 0x40,
				Spells2 = new List<byte>
				{
					(byte)SpellByte.ICE3,
					(byte)SpellByte.SLO2,
					(byte)SpellByte.ICE3,
					(byte)SpellByte.FOG,
					(byte)SpellByte.INVS,
					(byte)SpellByte.ICE3,
					(byte)SpellByte.SLO2,
					(byte)SpellByte.FOG,
				},
				SkillChance2 = 0x40,
				Skills2 = new List<byte>
				{
					(byte)EnemySkills.Blizzard,
					(byte)EnemySkills.Tornado,
					(byte)EnemySkills.Dazzle,
					(byte)EnemySkills.Squint,
				},
			},
			new AlternateFiends
			{
				Name = "D.STORM",
				ElementalWeakness = SpellElement.Time,
				MonsterType = MonsterType.DRAGON,
				SpellChance1 = 0x40,
				Spells1 = new List<byte>
				{
					(byte)SpellByte.LIT,
					(byte)SpellByte.FAST,
					(byte)SpellByte.LIT2,
					(byte)SpellByte.SLOW,
					(byte)SpellByte.LIT2,
					(byte)SpellByte.LOCK,
					(byte)SpellByte.LIT2,
					(byte)SpellByte.SLOW,
				},
				SkillChance1 = 0x40,
				Skills1 = new List<byte>
				{
					(byte)EnemySkills.Glare,
					(byte)EnemySkills.Dazzle,
					(byte)EnemySkills.Trance,
					(byte)EnemySkills.Stare,
				},
				SpellChance2 = 0x40,
				Spells2 = new List<byte>
				{
					(byte)SpellByte.LIT3,
					(byte)SpellByte.FAST,
					(byte)SpellByte.LIT3,
					(byte)SpellByte.SABR,
					(byte)SpellByte.XXXX,
					(byte)SpellByte.LIT3,
					(byte)SpellByte.LOK2,
					(byte)SpellByte.CUR3,
				},
				SkillChance2 = 0x40,
				Skills2 = new List<byte>
				{
					(byte)EnemySkills.Tornado,
					(byte)EnemySkills.Thunder,
					(byte)EnemySkills.Glare,
					(byte)EnemySkills.Poison_Stone,
				},
			},
			new AlternateFiends
			{
				Name = "DARKELF",
				ElementalWeakness = SpellElement.Poison,
				MonsterType = MonsterType.GIANT,
				SpellChance1 = 0x40,
				Spells1 = new List<byte>
				{
					(byte)SpellByte.FIR2,
					(byte)SpellByte.ICE2,
					(byte)SpellByte.LIT2,
					(byte)SpellByte.MUTE,
					(byte)SpellByte.STUN,
					(byte)SpellByte.FIR2,
					(byte)SpellByte.LIT2,
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
					(byte)SpellByte.LIT3,
					(byte)SpellByte.ICE3,
					(byte)SpellByte.FIR3,
					(byte)SpellByte.INVS,
					(byte)SpellByte.XXXX,
					(byte)SpellByte.NUKE,
					(byte)SpellByte.CUR3,
					(byte)SpellByte.SLO2,
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
				Name = "DETHMACH",
				ElementalWeakness = SpellElement.Earth,
				MonsterType = MonsterType.MAGICAL,
				SpellChance1 = 0x40,
				Spells1 = new List<byte>
				{
					(byte)SpellByte.BANE,
					(byte)SpellByte.FOG,
					(byte)SpellByte.BRAK,
					(byte)SpellByte.FAST,
					(byte)SpellByte.STUN,
					(byte)SpellByte.HOLD,
					(byte)SpellByte.QAKE,
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
					(byte)SpellByte.XXXX,
					(byte)SpellByte.XFER,
					(byte)SpellByte.ZAP,
					(byte)SpellByte.XFER,
					(byte)SpellByte.XXXX,
					(byte)SpellByte.QAKE,
					(byte)SpellByte.BANE,
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
				Name = "EVILWALL",
				ElementalWeakness = SpellElement.Earth,
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
					(byte)EnemySkills.Gaze,
					(byte)EnemySkills.Glance,
					(byte)EnemySkills.Dazzle,
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
					(byte)EnemySkills.Poison_Damage,
					(byte)EnemySkills.Toxic,
					(byte)EnemySkills.Squint,
					(byte)EnemySkills.Nuclear,
				},
			},
			new AlternateFiends
			{
				Name = "FLANMAST",
				ElementalWeakness = SpellElement.Status,
				MonsterType = MonsterType.MAGE,
				SpellChance1 = 0x40,
				Spells1 = new List<byte>
				{
					(byte)SpellByte.FIR2,
					(byte)SpellByte.INVS,
					(byte)SpellByte.FIR2,
					(byte)SpellByte.MUTE,
					(byte)SpellByte.RUB,
					(byte)SpellByte.SLOW,
					(byte)SpellByte.FIR2,
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
					(byte)SpellByte.NUKE,
					(byte)SpellByte.XXXX,
					(byte)SpellByte.FIR3,
					(byte)SpellByte.SLO2,
					(byte)SpellByte.BRAK,
					(byte)SpellByte.WALL,
					(byte)SpellByte.FIR3,
					(byte)SpellByte.INVS,
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
				Name = "GIGAWORM",
				ElementalWeakness = SpellElement.Ice,
				MonsterType = MonsterType.NONE,
				SpellChance1 = 0x40,
				Spells1 = new List<byte>
				{
					(byte)SpellByte.LIT2,
					(byte)SpellByte.FIR2,
					(byte)SpellByte.QAKE,
					(byte)SpellByte.STUN,
					(byte)SpellByte.MUTE,
					(byte)SpellByte.LIT2,
					(byte)SpellByte.FIR2,
					(byte)SpellByte.RUB,
				},
				SkillChance1 = 0x40,
				Skills1 = new List<byte>
				{
					(byte)EnemySkills.Crack,
					(byte)EnemySkills.Cremate,
					(byte)EnemySkills.Dazzle,
					(byte)EnemySkills.Trance,
				},
				SpellChance2 = 0x40,
				Spells2 = new List<byte>
				{
					(byte)SpellByte.QAKE,
					(byte)SpellByte.FAST,
					(byte)SpellByte.FIR3,
					(byte)SpellByte.SLP2,
					(byte)SpellByte.XXXX,
					(byte)SpellByte.INVS,
					(byte)SpellByte.FIR3,
					(byte)SpellByte.SLO2,
				},
				SkillChance2 = 0x40,
				Skills2 = new List<byte>
				{
					(byte)EnemySkills.Nuclear,
					(byte)EnemySkills.Poison_Stone,
					(byte)EnemySkills.Blaze,
					(byte)EnemySkills.Dazzle,
				},
			},
			new AlternateFiends
			{
				Name = "GOLBEZ",
				ElementalWeakness = SpellElement.Fire,
				MonsterType = MonsterType.MAGICAL,
				SpellChance1 = 0x40,
				Spells1 = new List<byte>
				{
					(byte)SpellByte.QAKE,
					(byte)SpellByte.ICE2,
					(byte)SpellByte.RUB,
					(byte)SpellByte.LIT2,
					(byte)SpellByte.FIRE,
					(byte)SpellByte.FIR2,
					(byte)SpellByte.LIT,
					(byte)SpellByte.ICE,
				},
				SkillChance1 = 0x40,
				Skills1 = new List<byte>
				{
					(byte)EnemySkills.Cremate,
					(byte)EnemySkills.Frost,
					(byte)EnemySkills.Gaze,
					(byte)EnemySkills.Stare,
				},
				SpellChance2 = 0x40,
				Spells2 = new List<byte>
				{
					(byte)SpellByte.NUKE,
					(byte)SpellByte.INVS,
					(byte)SpellByte.XXXX,
					(byte)SpellByte.ICE3,
					(byte)SpellByte.XFER,
					(byte)SpellByte.BRAK,
					(byte)SpellByte.FIR3,
					(byte)SpellByte.STOP,
				},
				SkillChance2 = 0x40,
				Skills2 = new List<byte>
				{
					(byte)EnemySkills.Blaze,
					(byte)EnemySkills.Inferno,
					(byte)EnemySkills.Swirl,
					(byte)EnemySkills.Poison_Stone,
				},
			},
			new AlternateFiends
			{
				Name = "IFRIT",
				ElementalWeakness = SpellElement.Ice,
				MonsterType = MonsterType.MAGICAL,
				SpellChance1 = 0x40,
				Spells1 = new List<byte>
				{
					(byte)SpellByte.FIRE,
					(byte)SpellByte.STUN,
					(byte)SpellByte.FIRE,
					(byte)SpellByte.FIR2,
					(byte)SpellByte.FIRE,
					(byte)SpellByte.STUN,
					(byte)SpellByte.FIRE,
					(byte)SpellByte.FIR2,
				},
				SkillChance1 = 0x40,
				Skills1 = new List<byte>
				{
					(byte)EnemySkills.Heat,
					(byte)EnemySkills.Scorch,
					(byte)EnemySkills.Heat,
					(byte)EnemySkills.Flash,
				},
				SpellChance2 = 0x40,
				Spells2 = new List<byte>
				{
					(byte)SpellByte.FIR2,
					(byte)SpellByte.FIR3,
					(byte)SpellByte.XFER,
					(byte)SpellByte.FIR2,
					(byte)SpellByte.FIR3,
					(byte)SpellByte.WALL,
					(byte)SpellByte.FIR3,
					(byte)SpellByte.NUKE,
				},
				SkillChance2 = 0x40,
				Skills2 = new List<byte>
				{
					(byte)EnemySkills.Inferno,
					(byte)EnemySkills.Flash,
					(byte)EnemySkills.Blaze,
					(byte)EnemySkills.Nuclear,
				},
			},
			new AlternateFiends
			{
				Name = "LUGAE",
				ElementalWeakness = SpellElement.Fire,
				MonsterType = MonsterType.UNDEAD,
				SpellChance1 = 0x40,
				Spells1 = new List<byte>
				{
					(byte)SpellByte.LIT2,
					(byte)SpellByte.FIR2,
					(byte)SpellByte.QAKE,
					(byte)SpellByte.STUN,
					(byte)SpellByte.MUTE,
					(byte)SpellByte.LIT2,
					(byte)SpellByte.FIR2,
					(byte)SpellByte.RUB,
				},
				SkillChance1 = 0x40,
				Skills1 = new List<byte>
				{
					(byte)EnemySkills.Crack,
					(byte)EnemySkills.Cremate,
					(byte)EnemySkills.Dazzle,
					(byte)EnemySkills.Trance,
				},
				SpellChance2 = 0x40,
				Spells2 = new List<byte>
				{
					(byte)SpellByte.QAKE,
					(byte)SpellByte.FAST,
					(byte)SpellByte.FIR3,
					(byte)SpellByte.SLP2,
					(byte)SpellByte.XXXX,
					(byte)SpellByte.INVS,
					(byte)SpellByte.FIR3,
					(byte)SpellByte.SLO2,
				},
				SkillChance2 = 0x40,
				Skills2 = new List<byte>
				{
					(byte)EnemySkills.Nuclear,
					(byte)EnemySkills.Poison_Stone,
					(byte)EnemySkills.Blaze,
					(byte)EnemySkills.Dazzle,
				},
			},
			new AlternateFiends
			{
				Name = "MOMBOMB",
				ElementalWeakness = SpellElement.Ice,
				MonsterType = MonsterType.NONE,
				SpellChance1 = 0x40,
				Spells1 = new List<byte>
				{
					(byte)SpellByte.FIRE,
					(byte)SpellByte.FIR2,
					(byte)SpellByte.RUB,
					(byte)SpellByte.SLOW,
					(byte)SpellByte.FIR2,
					(byte)SpellByte.FIR2,
					(byte)SpellByte.STUN,
					(byte)SpellByte.BLND,
				},
				SkillChance1 = 0x40,
				Skills1 = new List<byte>
				{
					(byte)EnemySkills.Heat,
					(byte)EnemySkills.Cremate,
					(byte)EnemySkills.Trance,
					(byte)EnemySkills.Heat,
				},
				SpellChance2 = 0x40,
				Spells2 = new List<byte>
				{
					(byte)SpellByte.FIR3,
					(byte)SpellByte.NUKE,
					(byte)SpellByte.MUTE,
					(byte)SpellByte.RUB,
					(byte)SpellByte.FIR3,
					(byte)SpellByte.FAST,
					(byte)SpellByte.CUR3,
					(byte)SpellByte.SLOW,
				},
				SkillChance2 = 0x40,
				Skills2 = new List<byte>
				{
					(byte)EnemySkills.Blaze,
					(byte)EnemySkills.Cremate,
					(byte)EnemySkills.Inferno,
					(byte)EnemySkills.Trance,
				},
			},
			new AlternateFiends
			{
				Name = "OCTOMAM",
				ElementalWeakness = SpellElement.Lightning,
				MonsterType = MonsterType.AQUATIC,
				SpellChance1 = 0x40,
				Spells1 = new List<byte>
				{
					(byte)SpellByte.ICE,
					(byte)SpellByte.CUR2,
					(byte)SpellByte.ICE2,
					(byte)SpellByte.FOG,
					(byte)SpellByte.ICE2,
					(byte)SpellByte.STUN,
					(byte)SpellByte.SLEP,
					(byte)SpellByte.ICE2,
				},
				SkillChance1 = 0x40,
				Skills1 = new List<byte>
				{
					(byte)EnemySkills.Ink,
					(byte)EnemySkills.Stinger,
					(byte)EnemySkills.Ink,
					(byte)EnemySkills.Stare,
				},
				SpellChance2 = 0x40,
				Spells2 = new List<byte>
				{
					(byte)SpellByte.ICE3,
					(byte)SpellByte.ICE3,
					(byte)SpellByte.FAST,
					(byte)SpellByte.CUR4,
					(byte)SpellByte.SLO2,
					(byte)SpellByte.XXXX,
					(byte)SpellByte.TMPR,
					(byte)SpellByte.ICE3,
				},
				SkillChance2 = 0x40,
				Skills2 = new List<byte>
				{
					(byte)EnemySkills.Swirl,
					(byte)EnemySkills.Ink,
					(byte)EnemySkills.Blizzard,
					(byte)EnemySkills.Stinger,
				},
			},
			new AlternateFiends
			{
				Name = "ODIN",
				ElementalWeakness = SpellElement.Lightning,
				MonsterType = MonsterType.GIANT,
				SpellChance1 = 0x40,
				Spells1 = new List<byte>
				{
					(byte)SpellByte.STUN,
					(byte)SpellByte.CUR2,
					(byte)SpellByte.FAST,
					(byte)SpellByte.SLOW,
					(byte)SpellByte.STUN,
					(byte)SpellByte.CUR2,
					(byte)SpellByte.FAST,
					(byte)SpellByte.SLOW,
				},
				SkillChance1 = 0x40,
				Skills1 = new List<byte>
				{
					(byte)EnemySkills.Gaze,
					(byte)EnemySkills.Flash,
					(byte)EnemySkills.Flash,
					(byte)EnemySkills.Crack,
				},
				SpellChance2 = 0x40,
				Spells2 = new List<byte>
				{
					(byte)SpellByte.STUN,
					(byte)SpellByte.CUR3,
					(byte)SpellByte.XXXX,
					(byte)SpellByte.SLO2,
					(byte)SpellByte.CUR3,
					(byte)SpellByte.STOP,
					(byte)SpellByte.SLOW,
					(byte)SpellByte.NUKE,
				},
				SkillChance2 = 0x40,
				Skills2 = new List<byte>
				{
					(byte)EnemySkills.Crack,
					(byte)EnemySkills.Gaze,
					(byte)EnemySkills.Flash,
					(byte)EnemySkills.Dazzle,
				},
			},
			new AlternateFiends
			{
				Name = "OGOPOGO",
				ElementalWeakness = SpellElement.Lightning,
				MonsterType = MonsterType.AQUATIC,
				SpellChance1 = 0x40,
				Spells1 = new List<byte>
				{
					(byte)SpellByte.ICE2,
					(byte)SpellByte.ICE,
					(byte)SpellByte.ICE2,
					(byte)SpellByte.FOG,
					(byte)SpellByte.HEL2,
					(byte)SpellByte.ICE2,
					(byte)SpellByte.SLOW,
					(byte)SpellByte.ICE2,
				},
				SkillChance1 = 0x40,
				Skills1 = new List<byte>
				{
					(byte)EnemySkills.Ink,
					(byte)EnemySkills.Frost,
					(byte)EnemySkills.Ink,
					(byte)EnemySkills.Frost,
				},
				SpellChance2 = 0x40,
				Spells2 = new List<byte>
				{
					(byte)SpellByte.ICE3,
					(byte)SpellByte.ICE3,
					(byte)SpellByte.INVS,
					(byte)SpellByte.BLND,
					(byte)SpellByte.ICE3,
					(byte)SpellByte.BRAK,
					(byte)SpellByte.WALL,
					(byte)SpellByte.ICE3,
				},
				SkillChance2 = 0x40,
				Skills2 = new List<byte>
				{
					(byte)EnemySkills.Swirl,
					(byte)EnemySkills.Ink,
					(byte)EnemySkills.Blizzard,
					(byte)EnemySkills.Frost,
				},
			},
			new AlternateFiends
			{
				Name = "PALEDIM",
				ElementalWeakness = SpellElement.None,
				MonsterType = MonsterType.DRAGON,
				SpellChance1 = 0x40,
				Spells1 = new List<byte>
				{
					(byte)SpellByte.ICE2,
					(byte)SpellByte.FIR2,
					(byte)SpellByte.LIT2,
					(byte)SpellByte.SLOW,
					(byte)SpellByte.ICE2,
					(byte)SpellByte.FIR2,
					(byte)SpellByte.LIT2,
					(byte)SpellByte.SLOW,
				},
				SkillChance1 = 0x40,
				Skills1 = new List<byte>
				{
					(byte)EnemySkills.Crack,
					(byte)EnemySkills.Frost,
					(byte)EnemySkills.Cremate,
					(byte)EnemySkills.Trance,
				},
				SpellChance2 = 0x40,
				Spells2 = new List<byte>
				{
					(byte)SpellByte.ICE3,
					(byte)SpellByte.FIR3,
					(byte)SpellByte.LIT3,
					(byte)SpellByte.SLO2,
					(byte)SpellByte.ICE3,
					(byte)SpellByte.LIT3,
					(byte)SpellByte.FIR3,
					(byte)SpellByte.SLO2,
				},
				SkillChance2 = 0x40,
				Skills2 = new List<byte>
				{
					(byte)EnemySkills.Thunder,
					(byte)EnemySkills.Inferno,
					(byte)EnemySkills.Blizzard,
					(byte)EnemySkills.Crack,
				},
			},
			new AlternateFiends
			{
				Name = "PLAGUE",
				ElementalWeakness = SpellElement.Status,
				MonsterType = MonsterType.MAGE,
				SpellChance1 = 0x40,
				Spells1 = new List<byte>
				{
					(byte)SpellByte.BRAK,
					(byte)SpellByte.SLOW,
					(byte)SpellByte.QAKE,
					(byte)SpellByte.SLOW,
					(byte)SpellByte.INVS,
					(byte)SpellByte.RUB,
					(byte)SpellByte.BRAK,
					(byte)SpellByte.SLOW,
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
					(byte)SpellByte.BRAK,
					(byte)SpellByte.QAKE,
					(byte)SpellByte.SLO2,
					(byte)SpellByte.INVS,
					(byte)SpellByte.RUB,
					(byte)SpellByte.ZAP,
					(byte)SpellByte.BANE,
				},
				SkillChance2 = 0x40,
				Skills2 = new List<byte>
				{
					(byte)EnemySkills.Poison_Stone,
					(byte)EnemySkills.Nuclear,
					(byte)EnemySkills.Toxic,
					(byte)EnemySkills.Crack,
				},
			},
			new AlternateFiends
			{
				Name = "RAMUH",
				ElementalWeakness = SpellElement.Earth,
				MonsterType = MonsterType.MAGE,
				SpellChance1 = 0x40,
				Spells1 = new List<byte>
				{
					(byte)SpellByte.LIT,
					(byte)SpellByte.LIT2,
					(byte)SpellByte.CUR2,
					(byte)SpellByte.LIT2,
					(byte)SpellByte.LIT,
					(byte)SpellByte.CUR2,
					(byte)SpellByte.LIT,
					(byte)SpellByte.LIT2,
				},
				SkillChance1 = 0x40,
				Skills1 = new List<byte>
				{
					(byte)EnemySkills.Stinger,
					(byte)EnemySkills.Flash,
					(byte)EnemySkills.Flash,
					(byte)EnemySkills.Stinger,
				},
				SpellChance2 = 0x40,
				Spells2 = new List<byte>
				{
					(byte)SpellByte.LIT3,
					(byte)SpellByte.STUN,
					(byte)SpellByte.LIT3,
					(byte)SpellByte.SLOW,
					(byte)SpellByte.CUR3,
					(byte)SpellByte.LIT3,
					(byte)SpellByte.STOP,
					(byte)SpellByte.LIT3,
				},
				SkillChance2 = 0x40,
				Skills2 = new List<byte>
				{
					(byte)EnemySkills.Thunder,
					(byte)EnemySkills.Stinger,
					(byte)EnemySkills.Flash,
					(byte)EnemySkills.Tornado,
				},
			},
			new AlternateFiends
			{
				Name = "RUBICANT",
				ElementalWeakness = SpellElement.Ice,
				MonsterType = MonsterType.MAGE,
				SpellChance1 = 0x40,
				Spells1 = new List<byte>
				{
					(byte)SpellByte.FIRE,
					(byte)SpellByte.FIRE,
					(byte)SpellByte.FIRE,
					(byte)SpellByte.FIR2,
					(byte)SpellByte.FIRE,
					(byte)SpellByte.FIRE,
					(byte)SpellByte.FIRE,
					(byte)SpellByte.FIR2,
				},
				SkillChance1 = 0x40,
				Skills1 = new List<byte>
				{
					(byte)EnemySkills.Heat,
					(byte)EnemySkills.Scorch,
					(byte)EnemySkills.Flash,
					(byte)EnemySkills.Dazzle,
				},
				SpellChance2 = 0x40,
				Spells2 = new List<byte>
				{
					(byte)SpellByte.FIR2,
					(byte)SpellByte.FIR2,
					(byte)SpellByte.FIR3,
					(byte)SpellByte.NUKE,
					(byte)SpellByte.FIR2,
					(byte)SpellByte.FIR3,
					(byte)SpellByte.FIR3,
					(byte)SpellByte.NUKE,
				},
				SkillChance2 = 0x40,
				Skills2 = new List<byte>
				{
					(byte)EnemySkills.Scorch,
					(byte)EnemySkills.Heat,
					(byte)EnemySkills.Blaze,
					(byte)EnemySkills.Nuclear,
				},
			},
			new AlternateFiends
			{
				Name = "SCARMLIO",
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
					(byte)EnemySkills.Snorting,
					(byte)EnemySkills.Gaze,
					(byte)EnemySkills.Ink,
					(byte)EnemySkills.Crack,
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
					(byte)EnemySkills.Snorting,
					(byte)EnemySkills.Crack,
					(byte)EnemySkills.Snorting,
				},
			},
			new AlternateFiends
			{
				Name = "SHADOW.D",
				ElementalWeakness = SpellElement.Earth,
				MonsterType = MonsterType.DRAGON,
				SpellChance1 = 0x40,
				Spells1 = new List<byte>
				{
					(byte)SpellByte.STUN,
					(byte)SpellByte.HOLD,
					(byte)SpellByte.RUB,
					(byte)SpellByte.STUN,
					(byte)SpellByte.HOLD,
					(byte)SpellByte.RUB,
					(byte)SpellByte.STUN,
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
					(byte)EnemySkills.Nuclear,
					(byte)EnemySkills.Poison_Stone,
				},
			},
			new AlternateFiends
			{
				Name = "SHIVA",
				ElementalWeakness = SpellElement.Fire,
				MonsterType = MonsterType.MAGE,
				SpellChance1 = 0x40,
				Spells1 = new List<byte>
				{
					(byte)SpellByte.ICE,
					(byte)SpellByte.ICE,
					(byte)SpellByte.ICE,
					(byte)SpellByte.ICE2,
					(byte)SpellByte.ICE,
					(byte)SpellByte.ICE,
					(byte)SpellByte.ICE,
					(byte)SpellByte.ICE2,
				},
				SkillChance1 = 0x40,
				Skills1 = new List<byte>
				{
					(byte)EnemySkills.Frost,
					(byte)EnemySkills.Flash,
					(byte)EnemySkills.Gaze,
					(byte)EnemySkills.Snorting,
				},
				SpellChance2 = 0x40,
				Spells2 = new List<byte>
				{
					(byte)SpellByte.ICE2,
					(byte)SpellByte.ICE3,
					(byte)SpellByte.ICE2,
					(byte)SpellByte.ICE2,
					(byte)SpellByte.ICE3,
					(byte)SpellByte.ICE3,
					(byte)SpellByte.ICE2,
					(byte)SpellByte.BRAK,
				},
				SkillChance2 = 0x40,
				Skills2 = new List<byte>
				{
					(byte)EnemySkills.Frost,
					(byte)EnemySkills.Flash,
					(byte)EnemySkills.Blizzard,
					(byte)EnemySkills.Gaze,
				},
			},
			new AlternateFiends
			{
				Name = "TITAN",
				ElementalWeakness = SpellElement.Lightning,
				MonsterType = MonsterType.GIANT,
				SpellChance1 = 0x40,
				Spells1 = new List<byte>
				{
					(byte)SpellByte.LIT,
					(byte)SpellByte.LIT,
					(byte)SpellByte.LIT,
					(byte)SpellByte.LIT,
					(byte)SpellByte.LIT,
					(byte)SpellByte.LIT,
					(byte)SpellByte.LIT,
					(byte)SpellByte.LIT,
				},
				SkillChance1 = 0x40,
				Skills1 = new List<byte>
				{
					(byte)EnemySkills.Dazzle,
					(byte)EnemySkills.Gaze,
					(byte)EnemySkills.Dazzle,
					(byte)EnemySkills.Flash,
				},
				SpellChance2 = 0x40,
				Spells2 = new List<byte>
				{
					(byte)SpellByte.LIT2,
					(byte)SpellByte.FAST,
					(byte)SpellByte.LIT3,
					(byte)SpellByte.SLO2,
					(byte)SpellByte.STOP,
					(byte)SpellByte.LIT3,
					(byte)SpellByte.LIT2,
					(byte)SpellByte.XFER,
				},
				SkillChance2 = 0x40,
				Skills2 = new List<byte>
				{
					(byte)EnemySkills.Swirl,
					(byte)EnemySkills.Scorch,
					(byte)EnemySkills.Dazzle,
					(byte)EnemySkills.Tornado,
				},
			},
			new AlternateFiends
			{
				Name = "WYVERN",
				ElementalWeakness = SpellElement.None,
				MonsterType = MonsterType.DRAGON,
				SpellChance1 = 0x40,
				Spells1 = new List<byte>
				{
					(byte)SpellByte.FIR2,
					(byte)SpellByte.SLOW,
					(byte)SpellByte.FIR2,
					(byte)SpellByte.INVS,
					(byte)SpellByte.FIR2,
					(byte)SpellByte.MUTE,
					(byte)SpellByte.FIR2,
					(byte)SpellByte.SLOW,
				},
				SkillChance1 = 0x40,
				Skills1 = new List<byte>
				{
					(byte)EnemySkills.Heat,
					(byte)EnemySkills.Cremate,
					(byte)EnemySkills.Trance,
					(byte)EnemySkills.Heat,
				},
				SpellChance2 = 0x40,
				Spells2 = new List<byte>
				{
					(byte)SpellByte.NUKE,
					(byte)SpellByte.INVS,
					(byte)SpellByte.FIR3,
					(byte)SpellByte.FAST,
					(byte)SpellByte.XXXX,
					(byte)SpellByte.NUKE,
					(byte)SpellByte.SLO2,
					(byte)SpellByte.FIR3,
				},
				SkillChance2 = 0x40,
				Skills2 = new List<byte>
				{
					(byte)EnemySkills.Inferno,
					(byte)EnemySkills.Tornado,
					(byte)EnemySkills.Glare,
					(byte)EnemySkills.Tornado,
				},
			},
			new AlternateFiends
			{
				Name = "ZEMUS",
				ElementalWeakness = SpellElement.None,
				MonsterType = MonsterType.GIANT,
				SpellChance1 = 0x40,
				Spells1 = new List<byte>
				{
					(byte)SpellByte.FIR2,
					(byte)SpellByte.LIT2,
					(byte)SpellByte.ICE2,
					(byte)SpellByte.ZAP,
					(byte)SpellByte.SLOW,
					(byte)SpellByte.MUTE,
					(byte)SpellByte.QAKE,
					(byte)SpellByte.ICE2,
				},
				SkillChance1 = 0x40,
				Skills1 = new List<byte>
				{
					(byte)EnemySkills.Trance,
					(byte)EnemySkills.Cremate,
					(byte)EnemySkills.Frost,
					(byte)EnemySkills.Stare,
				},
				SpellChance2 = 0x40,
				Spells2 = new List<byte>
				{
					(byte)SpellByte.FAST,
					(byte)SpellByte.NUKE,
					(byte)SpellByte.FIR3,
					(byte)SpellByte.BANE,
					(byte)SpellByte.SABR,
					(byte)SpellByte.SLO2,
					(byte)SpellByte.NUKE,
					(byte)SpellByte.CUR4,
				},
				SkillChance2 = 0x40,
				Skills2 = new List<byte>
				{
					(byte)EnemySkills.Swirl,
					(byte)EnemySkills.Poison_Stone,
					(byte)EnemySkills.Toxic,
					(byte)EnemySkills.Nuclear,
				},
			},
		};
	}
}
