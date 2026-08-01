namespace FF1Lib
{
	public partial class FF1Rom
	{
		List<AlternateFiends> FF3AltFiendslist = new()
		{
			new AlternateFiends
			{
				Name = "AHRIMAN",
				ElementalWeakness = SpellElement.None,
				MonsterType = MonsterType.MAGICAL,
				SpellChance1 = 0x40,
				Spells1 = new List<byte>
				{
					(byte)SpellByte.FIR2,
					(byte)SpellByte.ICE2,
					(byte)SpellByte.MUTE,
					(byte)SpellByte.SLO2,
					(byte)SpellByte.LIT2,
					(byte)SpellByte.CUR2,
					(byte)SpellByte.INVS,
					(byte)SpellByte.SLP2,
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
					(byte)SpellByte.SLO2,
					(byte)SpellByte.XXXX,
					(byte)SpellByte.CUR3,
					(byte)SpellByte.ICE2,
					(byte)SpellByte.MUTE,
					(byte)SpellByte.FIR3,
					(byte)SpellByte.XFER,
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
				Name = "AMON",
				ElementalWeakness = SpellElement.Status,
				MonsterType = MonsterType.NONE,
				SpellChance1 = 0x40,
				Spells1 = new List<byte>
				{
					(byte)SpellByte.FIR2,
					(byte)SpellByte.FIR2,
					(byte)SpellByte.LIT2,
					(byte)SpellByte.BRAK,
					(byte)SpellByte.SLOW,
					(byte)SpellByte.FOG,
					(byte)SpellByte.STUN,
					(byte)SpellByte.AICE,
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
					(byte)SpellByte.FOG,
					(byte)SpellByte.LIT3,
					(byte)SpellByte.CUR4,
					(byte)SpellByte.XXXX,
					(byte)SpellByte.BANE,
					(byte)SpellByte.QAKE,
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
				Name = "BIGRAT",
				ElementalWeakness = SpellElement.None,
				MonsterType = MonsterType.WERE,
				SpellChance1 = 0x40,
				Spells1 = new List<byte>
				{
					(byte)SpellByte.FIRE,
					(byte)SpellByte.ICE,
					(byte)SpellByte.MUTE,
					(byte)SpellByte.STUN,
					(byte)SpellByte.ICE,
					(byte)SpellByte.FIRE,
					(byte)SpellByte.LIT,
					(byte)SpellByte.DARK,
				},
				SkillChance1 = 0x40,
				Skills1 = new List<byte>
				{
					(byte)EnemySkills.Ink,
					(byte)EnemySkills.Stinger,
					(byte)EnemySkills.Snorting,
					(byte)EnemySkills.Dazzle,
				},
				SpellChance2 = 0x40,
				Spells2 = new List<byte>
				{
					(byte)SpellByte.FIR2,
					(byte)SpellByte.ICE2,
					(byte)SpellByte.XXXX,
					(byte)SpellByte.LIT2,
					(byte)SpellByte.INVS,
					(byte)SpellByte.ICE2,
					(byte)SpellByte.MUTE,
					(byte)SpellByte.RUB,
				},
				SkillChance2 = 0x40,
				Skills2 = new List<byte>
				{
					(byte)EnemySkills.Cremate,
					(byte)EnemySkills.Blizzard,
					(byte)EnemySkills.Gaze,
					(byte)EnemySkills.Stare,
				},
			},
			new AlternateFiends
			{
				Name = "CARBUNCL",
				ElementalWeakness = SpellElement.None,
				MonsterType = MonsterType.MAGICAL,
				SpellChance1 = 0x40,
				Spells1 = new List<byte>
				{
					(byte)SpellByte.CURE,
					(byte)SpellByte.FAST,
					(byte)SpellByte.CURE,
					(byte)SpellByte.SLOW,
					(byte)SpellByte.CURE,
					(byte)SpellByte.FAST,
					(byte)SpellByte.CURE,
					(byte)SpellByte.SLOW,
				},
				SkillChance1 = 0x40,
				Skills1 = new List<byte>
				{
					(byte)EnemySkills.Stinger,
					(byte)EnemySkills.Dazzle,
					(byte)EnemySkills.Gaze,
					(byte)EnemySkills.Dazzle,
				},
				SpellChance2 = 0x40,
				Spells2 = new List<byte>
				{
					(byte)SpellByte.FAST,
					(byte)SpellByte.CUR2,
					(byte)SpellByte.SLO2,
					(byte)SpellByte.WALL,
					(byte)SpellByte.CUR2,
					(byte)SpellByte.XFER,
					(byte)SpellByte.SABR,
					(byte)SpellByte.CUR3,
				},
				SkillChance2 = 0x40,
				Skills2 = new List<byte>
				{
					(byte)EnemySkills.Stinger,
					(byte)EnemySkills.Gaze,
					(byte)EnemySkills.Glance,
					(byte)EnemySkills.Dazzle,
				},
			},
			new AlternateFiends
			{
				Name = "DJINN",
				ElementalWeakness = SpellElement.Ice,
				MonsterType = MonsterType.MAGICAL,
				SpellChance1 = 0x40,
				Spells1 = new List<byte>
				{
					(byte)SpellByte.FIRE,
					(byte)SpellByte.FIRE,
					(byte)SpellByte.FIRE,
					(byte)SpellByte.FAST,
					(byte)SpellByte.FIRE,
					(byte)SpellByte.FIRE,
					(byte)SpellByte.FIRE,
					(byte)SpellByte.FAST,
				},
				SkillChance1 = 0x40,
				Skills1 = new List<byte>
				{
					(byte)EnemySkills.Scorch,
					(byte)EnemySkills.Heat,
					(byte)EnemySkills.Scorch,
					(byte)EnemySkills.Gaze,
				},
				SpellChance2 = 0x40,
				Spells2 = new List<byte>
				{
					(byte)SpellByte.FIR2,
					(byte)SpellByte.FIR2,
					(byte)SpellByte.FAST,
					(byte)SpellByte.FIR3,
					(byte)SpellByte.FIR2,
					(byte)SpellByte.SLO2,
					(byte)SpellByte.FIR2,
					(byte)SpellByte.NUKE,
				},
				SkillChance2 = 0x40,
				Skills2 = new List<byte>
				{
					(byte)EnemySkills.Blaze,
					(byte)EnemySkills.Scorch,
					(byte)EnemySkills.Scorch,
					(byte)EnemySkills.Inferno,
				},
			},
			new AlternateFiends
			{
				Name = "DOGA",
				ElementalWeakness = SpellElement.None,
				MonsterType = MonsterType.MAGE,
				SpellChance1 = 0x40,
				Spells1 = new List<byte>
				{
					(byte)SpellByte.MUTE,
					(byte)SpellByte.BRAK,
					(byte)SpellByte.SLEP,
					(byte)SpellByte.STUN,
					(byte)SpellByte.QAKE,
					(byte)SpellByte.SLOW,
					(byte)SpellByte.MUTE,
					(byte)SpellByte.BLND,
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
					(byte)SpellByte.BRAK,
					(byte)SpellByte.BRAK,
					(byte)SpellByte.QAKE,
					(byte)SpellByte.QAKE,
					(byte)SpellByte.ZAP,
					(byte)SpellByte.SLO2,
					(byte)SpellByte.RUB,
					(byte)SpellByte.XXXX,
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
				Name = "ECHIDNA",
				ElementalWeakness = SpellElement.Fire,
				MonsterType = MonsterType.MAGE,
				SpellChance1 = 0x40,
				Spells1 = new List<byte>
				{
					(byte)SpellByte.STUN,
					(byte)SpellByte.STOP,
					(byte)SpellByte.STUN,
					(byte)SpellByte.SLOW,
					(byte)SpellByte.STUN,
					(byte)SpellByte.STOP,
					(byte)SpellByte.STUN,
					(byte)SpellByte.SLOW,
				},
				SkillChance1 = 0x40,
				Skills1 = new List<byte>
				{
					(byte)EnemySkills.Dazzle,
					(byte)EnemySkills.Ink,
					(byte)EnemySkills.Ink,
					(byte)EnemySkills.Crack,
				},
				SpellChance2 = 0x40,
				Spells2 = new List<byte>
				{
					(byte)SpellByte.STUN,
					(byte)SpellByte.STOP,
					(byte)SpellByte.ZAP,
					(byte)SpellByte.STOP,
					(byte)SpellByte.WALL,
					(byte)SpellByte.XFER,
					(byte)SpellByte.STUN,
					(byte)SpellByte.XXXX,
				},
				SkillChance2 = 0x40,
				Skills2 = new List<byte>
				{
					(byte)EnemySkills.Crack,
					(byte)EnemySkills.Trance,
					(byte)EnemySkills.Crack,
					(byte)EnemySkills.Gaze,
				},
			},
			new AlternateFiends
			{
				Name = "GARUDA",
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
					(byte)EnemySkills.Dazzle,
					(byte)EnemySkills.Flash,
					(byte)EnemySkills.Ink,
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
					(byte)EnemySkills.Thunder,
					(byte)EnemySkills.Nuclear,
					(byte)EnemySkills.Toxic,
					(byte)EnemySkills.Poison_Damage,
				},
			},
			new AlternateFiends
			{
				Name = "GENERAL",
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
					(byte)EnemySkills.Flash,
					(byte)EnemySkills.Stare,
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
					(byte)EnemySkills.Trance,
					(byte)EnemySkills.Squint,
					(byte)EnemySkills.Toxic,
					(byte)EnemySkills.Crack,
				},
			},
			new AlternateFiends
			{
				Name = "GOLDOR",
				ElementalWeakness = SpellElement.None,
				MonsterType = MonsterType.MAGICAL,
				SpellChance1 = 0x40,
				Spells1 = new List<byte>
				{
					(byte)SpellByte.FIRE,
					(byte)SpellByte.LIT,
					(byte)SpellByte.ICE2,
					(byte)SpellByte.MUTE,
					(byte)SpellByte.AFIR,
					(byte)SpellByte.AICE,
					(byte)SpellByte.ALIT,
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
					(byte)SpellByte.FIR2,
					(byte)SpellByte.CUR3,
					(byte)SpellByte.LIT2,
					(byte)SpellByte.ICE2,
					(byte)SpellByte.WALL,
					(byte)SpellByte.MUTE,
					(byte)SpellByte.INVS,
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
				Name = "GUARDIN",
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
					(byte)EnemySkills.Crack,
					(byte)EnemySkills.Stinger,
					(byte)EnemySkills.Trance,
					(byte)EnemySkills.Gaze,
				},
				SpellChance2 = 0x40,
				Spells2 = new List<byte>
				{
					(byte)SpellByte.WALL,
					(byte)SpellByte.CUR4,
					(byte)SpellByte.SLO2,
					(byte)SpellByte.FAST,
					(byte)SpellByte.INVS,
					(byte)SpellByte.SLP2,
					(byte)SpellByte.QAKE,
					(byte)SpellByte.QAKE,
				},
				SkillChance2 = 0x40,
				Skills2 = new List<byte>
				{
					(byte)EnemySkills.Crack,
					(byte)EnemySkills.Stare,
					(byte)EnemySkills.Dazzle,
					(byte)EnemySkills.Tornado,
				},
			},
			new AlternateFiends
			{
				Name = "GUTSCO",
				ElementalWeakness = SpellElement.Ice,
				MonsterType = MonsterType.GIANT,
				SpellChance1 = 0x40,
				Spells1 = new List<byte>
				{
					(byte)SpellByte.FIRE,
					(byte)SpellByte.FIR2,
					(byte)SpellByte.SLOW,
					(byte)SpellByte.FIRE,
					(byte)SpellByte.FIR2,
					(byte)SpellByte.STUN,
					(byte)SpellByte.BLND,
					(byte)SpellByte.FIRE,
				},
				SkillChance1 = 0x40,
				Skills1 = new List<byte>
				{
					(byte)EnemySkills.Cremate,
					(byte)EnemySkills.Heat,
					(byte)EnemySkills.Stinger,
					(byte)EnemySkills.Trance,
				},
				SpellChance2 = 0x40,
				Spells2 = new List<byte>
				{
					(byte)SpellByte.FIR2,
					(byte)SpellByte.FIR2,
					(byte)SpellByte.FAST,
					(byte)SpellByte.FOG,
					(byte)SpellByte.SLO2,
					(byte)SpellByte.SLP2,
					(byte)SpellByte.FIR2,
					(byte)SpellByte.LOK2,
				},
				SkillChance2 = 0x40,
				Skills2 = new List<byte>
				{
					(byte)EnemySkills.Blaze,
					(byte)EnemySkills.Nuclear,
					(byte)EnemySkills.Cremate,
					(byte)EnemySkills.Squint,
				},
			},
			new AlternateFiends
			{
				Name = "HECATON",
				ElementalWeakness = SpellElement.Earth,
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
					(byte)EnemySkills.Gaze,
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
					(byte)EnemySkills.Crack,
					(byte)EnemySkills.Squint,
					(byte)EnemySkills.Toxic,
					(byte)EnemySkills.Glare,
				},
			},
			new AlternateFiends
			{
				Name = "KUNOICHI",
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
					(byte)EnemySkills.Dazzle,
					(byte)EnemySkills.Gaze,
					(byte)EnemySkills.Trance,
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
					(byte)EnemySkills.Trance,
					(byte)EnemySkills.Poison_Damage,
					(byte)EnemySkills.Glare,
				},
			},
			new AlternateFiends
			{
				Name = "LEVIATHN",
				ElementalWeakness = SpellElement.Lightning,
				MonsterType = MonsterType.GIANT,
				SpellChance1 = 0x40,
				Spells1 = new List<byte>
				{
					(byte)SpellByte.ICE,
					(byte)SpellByte.ICE2,
					(byte)SpellByte.ICE,
					(byte)SpellByte.STOP,
					(byte)SpellByte.ICE,
					(byte)SpellByte.ICE2,
					(byte)SpellByte.ICE,
					(byte)SpellByte.STOP,
				},
				SkillChance1 = 0x40,
				Skills1 = new List<byte>
				{
					(byte)EnemySkills.Frost,
					(byte)EnemySkills.Dazzle,
					(byte)EnemySkills.Stinger,
					(byte)EnemySkills.Flash,
				},
				SpellChance2 = 0x40,
				Spells2 = new List<byte>
				{
					(byte)SpellByte.ICE2,
					(byte)SpellByte.STUN,
					(byte)SpellByte.TMPR,
					(byte)SpellByte.ICE3,
					(byte)SpellByte.ICE2,
					(byte)SpellByte.HOLD,
					(byte)SpellByte.XFER,
					(byte)SpellByte.ICE3,
				},
				SkillChance2 = 0x40,
				Skills2 = new List<byte>
				{
					(byte)EnemySkills.Frost,
					(byte)EnemySkills.Swirl,
					(byte)EnemySkills.Blizzard,
					(byte)EnemySkills.Ink,
				},
			},
			new AlternateFiends
			{
				Name = "LUCIFER",
				ElementalWeakness = SpellElement.Ice,
				MonsterType = MonsterType.REGENERATIVE,
				SpellChance1 = 0x40,
				Spells1 = new List<byte>
				{
					(byte)SpellByte.FIR2,
					(byte)SpellByte.FIRE,
					(byte)SpellByte.FIR2,
					(byte)SpellByte.SLOW,
					(byte)SpellByte.SLEP,
					(byte)SpellByte.BLND,
					(byte)SpellByte.MUTE,
					(byte)SpellByte.FIRE,
				},
				SkillChance1 = 0x40,
				Skills1 = new List<byte>
				{
					(byte)EnemySkills.Heat,
					(byte)EnemySkills.Scorch,
					(byte)EnemySkills.Trance,
					(byte)EnemySkills.Glance,
				},
				SpellChance2 = 0x40,
				Spells2 = new List<byte>
				{
					(byte)SpellByte.NUKE,
					(byte)SpellByte.SABR,
					(byte)SpellByte.XFER,
					(byte)SpellByte.FIR3,
					(byte)SpellByte.XXXX,
					(byte)SpellByte.FAST,
					(byte)SpellByte.FOG2,
					(byte)SpellByte.QAKE,
				},
				SkillChance2 = 0x40,
				Skills2 = new List<byte>
				{
					(byte)EnemySkills.Inferno,
					(byte)EnemySkills.Toxic,
					(byte)EnemySkills.Stare,
					(byte)EnemySkills.Squint,
				},
			},
			new AlternateFiends
			{
				Name = "NEP.DRGN",
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
				SkillChance1 = 0x40,
				Skills1 = new List<byte>
				{
					(byte)EnemySkills.Stare,
					(byte)EnemySkills.Heat,
					(byte)EnemySkills.Cremate,
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
					(byte)EnemySkills.Toxic,
					(byte)EnemySkills.Inferno,
					(byte)EnemySkills.Tornado,
				},
			},
			new AlternateFiends
			{
				Name = "NINJI",
				ElementalWeakness = SpellElement.Time,
				MonsterType = MonsterType.NONE,
				SpellChance1 = 0x40,
				Spells1 = new List<byte>
				{
					(byte)SpellByte.FOG,
					(byte)SpellByte.INVS,
					(byte)SpellByte.BLND,
					(byte)SpellByte.BLND,
					(byte)SpellByte.STUN,
					(byte)SpellByte.SLEP,
					(byte)SpellByte.SLEP,
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
					(byte)SpellByte.FAST,
					(byte)SpellByte.SABR,
					(byte)SpellByte.FOG2,
					(byte)SpellByte.INVS,
					(byte)SpellByte.SLO2,
					(byte)SpellByte.SLP2,
					(byte)SpellByte.BANE,
					(byte)SpellByte.ZAP,
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
				Name = "SALAMAND",
				ElementalWeakness = SpellElement.Ice,
				MonsterType = MonsterType.DRAGON,
				SpellChance1 = 0x40,
				Spells1 = new List<byte>
				{
					(byte)SpellByte.FIRE,
					(byte)SpellByte.DARK,
					(byte)SpellByte.FIRE,
					(byte)SpellByte.FIR2,
					(byte)SpellByte.FIRE,
					(byte)SpellByte.DARK,
					(byte)SpellByte.FIRE,
					(byte)SpellByte.FIR2,
				},
				SkillChance1 = 0x40,
				Skills1 = new List<byte>
				{
					(byte)EnemySkills.Flash,
					(byte)EnemySkills.Heat,
					(byte)EnemySkills.Flash,
					(byte)EnemySkills.Heat,
				},
				SpellChance2 = 0x40,
				Spells2 = new List<byte>
				{
					(byte)SpellByte.XXXX,
					(byte)SpellByte.FIR2,
					(byte)SpellByte.FIR3,
					(byte)SpellByte.NUKE,
					(byte)SpellByte.FIR2,
					(byte)SpellByte.FAST,
					(byte)SpellByte.FIR3,
					(byte)SpellByte.DARK,
				},
				SkillChance2 = 0x40,
				Skills2 = new List<byte>
				{
					(byte)EnemySkills.Scorch,
					(byte)EnemySkills.Blaze,
					(byte)EnemySkills.Heat,
					(byte)EnemySkills.Inferno,
				},
			},
			new AlternateFiends
			{
				Name = "SCYLLA",
				ElementalWeakness = SpellElement.Ice,
				MonsterType = MonsterType.AQUATIC,
				SpellChance1 = 0x40,
				Spells1 = new List<byte>
				{
					(byte)SpellByte.LIT,
					(byte)SpellByte.LIT2,
					(byte)SpellByte.LIT,
					(byte)SpellByte.LIT2,
					(byte)SpellByte.LIT,
					(byte)SpellByte.LIT2,
					(byte)SpellByte.LIT,
					(byte)SpellByte.LIT2,
				},
				SkillChance1 = 0x40,
				Skills1 = new List<byte>
				{
					(byte)EnemySkills.Dazzle,
					(byte)EnemySkills.Flash,
					(byte)EnemySkills.Glance,
					(byte)EnemySkills.Flash,
				},
				SpellChance2 = 0x40,
				Spells2 = new List<byte>
				{
					(byte)SpellByte.LIT2,
					(byte)SpellByte.LIT2,
					(byte)SpellByte.LIT3,
					(byte)SpellByte.SLO2,
					(byte)SpellByte.LIT2,
					(byte)SpellByte.LIT2,
					(byte)SpellByte.LIT3,
					(byte)SpellByte.FAST,
				},
				SkillChance2 = 0x40,
				Skills2 = new List<byte>
				{
					(byte)EnemySkills.Thunder,
					(byte)EnemySkills.Dazzle,
					(byte)EnemySkills.Tornado,
					(byte)EnemySkills.Flash,
				},
			},
			new AlternateFiends
			{
				Name = "UNNE",
				ElementalWeakness = SpellElement.Status,
				MonsterType = MonsterType.MAGE,
				SpellChance1 = 0x40,
				Spells1 = new List<byte>
				{
					(byte)SpellByte.ICE,
					(byte)SpellByte.ICE,
					(byte)SpellByte.SLOW,
					(byte)SpellByte.ICE,
					(byte)SpellByte.FOG,
					(byte)SpellByte.MUTE,
					(byte)SpellByte.SLOW,
					(byte)SpellByte.MUTE,
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
					(byte)SpellByte.SLO2,
					(byte)SpellByte.XFER,
					(byte)SpellByte.ICE3,
					(byte)SpellByte.BANE,
					(byte)SpellByte.MUTE,
					(byte)SpellByte.ICE2,
					(byte)SpellByte.ICE2,
				},
				SkillChance2 = 0x40,
				Skills2 = new List<byte>
				{
					(byte)EnemySkills.Blizzard,
					(byte)EnemySkills.Tornado,
					(byte)EnemySkills.Frost,
					(byte)EnemySkills.Glare,
				},
			},
			new AlternateFiends
			{
				Name = "ZANDE",
				ElementalWeakness = SpellElement.None,
				MonsterType = MonsterType.MAGE,
				SpellChance1 = 0x40,
				Spells1 = new List<byte>
				{
					(byte)SpellByte.FIRE,
					(byte)SpellByte.LIT2,
					(byte)SpellByte.FIR2,
					(byte)SpellByte.CUR2,
					(byte)SpellByte.HOLD,
					(byte)SpellByte.LIT,
					(byte)SpellByte.HOLD,
					(byte)SpellByte.BLND,
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
					(byte)SpellByte.CUR4,
					(byte)SpellByte.LIT3,
					(byte)SpellByte.FIR2,
					(byte)SpellByte.QAKE,
					(byte)SpellByte.FIR3,
					(byte)SpellByte.XXXX,
					(byte)SpellByte.SLP2,
					(byte)SpellByte.RUB,
				},
				SkillChance2 = 0x40,
				Skills2 = new List<byte>
				{
					(byte)EnemySkills.Crack,
					(byte)EnemySkills.Nuclear,
					(byte)EnemySkills.Blaze,
					(byte)EnemySkills.Toxic,
				},
			},
		};

		List<AlternateFiends> FF3AltFiendListHardcore = new()
		{
			new AlternateFiends
			{
				Name = "AHRIMAN",
				ElementalWeakness = SpellElement.None,
				MonsterType = MonsterType.MAGICAL,
				SpellChance1 = 0x40,
				Spells1 = new List<byte>
				{
					(byte)SpellByte.FIR2,
					(byte)SpellByte.ICE2,
					(byte)SpellByte.MUTE,
					(byte)SpellByte.SLO2,
					(byte)SpellByte.LIT2,
					(byte)SpellByte.CUR2,
					(byte)SpellByte.INVS,
					(byte)SpellByte.SLP2,
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
					(byte)SpellByte.SLO2,
					(byte)SpellByte.XXXX,
					(byte)SpellByte.CUR4,
					(byte)SpellByte.ICE3,
					(byte)SpellByte.MUTE,
					(byte)SpellByte.FIR3,
					(byte)SpellByte.XFER,
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
				Name = "AMON",
				ElementalWeakness = SpellElement.Status,
				MonsterType = MonsterType.NONE,
				SpellChance1 = 0x40,
				Spells1 = new List<byte>
				{
					(byte)SpellByte.FIR2,
					(byte)SpellByte.FIR2,
					(byte)SpellByte.LIT2,
					(byte)SpellByte.BRAK,
					(byte)SpellByte.SLOW,
					(byte)SpellByte.FOG,
					(byte)SpellByte.STUN,
					(byte)SpellByte.AICE,
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
					(byte)SpellByte.NUKE,
					(byte)SpellByte.LIT3,
					(byte)SpellByte.CUR4,
					(byte)SpellByte.XXXX,
					(byte)SpellByte.BANE,
					(byte)SpellByte.QAKE,
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
				Name = "BIGRAT",
				ElementalWeakness = SpellElement.None,
				MonsterType = MonsterType.WERE,
				SpellChance1 = 0x40,
				Spells1 = new List<byte>
				{
					(byte)SpellByte.FIRE,
					(byte)SpellByte.ICE,
					(byte)SpellByte.MUTE,
					(byte)SpellByte.STUN,
					(byte)SpellByte.ICE,
					(byte)SpellByte.FIRE,
					(byte)SpellByte.LIT,
					(byte)SpellByte.DARK,
				},
				SkillChance1 = 0x40,
				Skills1 = new List<byte>
				{
					(byte)EnemySkills.Ink,
					(byte)EnemySkills.Stinger,
					(byte)EnemySkills.Snorting,
					(byte)EnemySkills.Dazzle,
				},
				SpellChance2 = 0x40,
				Spells2 = new List<byte>
				{
					(byte)SpellByte.FIR3,
					(byte)SpellByte.ICE3,
					(byte)SpellByte.XXXX,
					(byte)SpellByte.LIT3,
					(byte)SpellByte.INVS,
					(byte)SpellByte.ICE3,
					(byte)SpellByte.MUTE,
					(byte)SpellByte.RUB,
				},
				SkillChance2 = 0x40,
				Skills2 = new List<byte>
				{
					(byte)EnemySkills.Inferno,
					(byte)EnemySkills.Blizzard,
					(byte)EnemySkills.Thunder,
					(byte)EnemySkills.Swirl,
				},
			},
			new AlternateFiends
			{
				Name = "CARBUNCL",
				ElementalWeakness = SpellElement.None,
				MonsterType = MonsterType.MAGICAL,
				SpellChance1 = 0x40,
				Spells1 = new List<byte>
				{
					(byte)SpellByte.CURE,
					(byte)SpellByte.FAST,
					(byte)SpellByte.CURE,
					(byte)SpellByte.SLOW,
					(byte)SpellByte.CURE,
					(byte)SpellByte.FAST,
					(byte)SpellByte.CURE,
					(byte)SpellByte.SLOW,
				},
				SkillChance1 = 0x40,
				Skills1 = new List<byte>
				{
					(byte)EnemySkills.Stinger,
					(byte)EnemySkills.Dazzle,
					(byte)EnemySkills.Gaze,
					(byte)EnemySkills.Dazzle,
				},
				SpellChance2 = 0x40,
				Spells2 = new List<byte>
				{
					(byte)SpellByte.FAST,
					(byte)SpellByte.CUR2,
					(byte)SpellByte.SLO2,
					(byte)SpellByte.WALL,
					(byte)SpellByte.CUR2,
					(byte)SpellByte.XFER,
					(byte)SpellByte.SABR,
					(byte)SpellByte.CUR3,
				},
				SkillChance2 = 0x40,
				Skills2 = new List<byte>
				{
					(byte)EnemySkills.Stinger,
					(byte)EnemySkills.Gaze,
					(byte)EnemySkills.Glance,
					(byte)EnemySkills.Dazzle,
				},
			},
			new AlternateFiends
			{
				Name = "DJINN",
				ElementalWeakness = SpellElement.Ice,
				MonsterType = MonsterType.MAGICAL,
				SpellChance1 = 0x40,
				Spells1 = new List<byte>
				{
					(byte)SpellByte.FIRE,
					(byte)SpellByte.FIRE,
					(byte)SpellByte.FIRE,
					(byte)SpellByte.FAST,
					(byte)SpellByte.FIRE,
					(byte)SpellByte.FIRE,
					(byte)SpellByte.FIRE,
					(byte)SpellByte.FAST,
				},
				SkillChance1 = 0x40,
				Skills1 = new List<byte>
				{
					(byte)EnemySkills.Scorch,
					(byte)EnemySkills.Heat,
					(byte)EnemySkills.Scorch,
					(byte)EnemySkills.Gaze,
				},
				SpellChance2 = 0x40,
				Spells2 = new List<byte>
				{
					(byte)SpellByte.FIR2,
					(byte)SpellByte.FIR2,
					(byte)SpellByte.FAST,
					(byte)SpellByte.FIR3,
					(byte)SpellByte.FIR2,
					(byte)SpellByte.SLO2,
					(byte)SpellByte.FIR2,
					(byte)SpellByte.NUKE,
				},
				SkillChance2 = 0x40,
				Skills2 = new List<byte>
				{
					(byte)EnemySkills.Blaze,
					(byte)EnemySkills.Scorch,
					(byte)EnemySkills.Scorch,
					(byte)EnemySkills.Inferno,
				},
			},
			new AlternateFiends
			{
				Name = "DOGA",
				ElementalWeakness = SpellElement.None,
				MonsterType = MonsterType.MAGE,
				SpellChance1 = 0x40,
				Spells1 = new List<byte>
				{
					(byte)SpellByte.MUTE,
					(byte)SpellByte.BRAK,
					(byte)SpellByte.SLEP,
					(byte)SpellByte.STUN,
					(byte)SpellByte.QAKE,
					(byte)SpellByte.SLOW,
					(byte)SpellByte.MUTE,
					(byte)SpellByte.BLND,
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
					(byte)SpellByte.BRAK,
					(byte)SpellByte.BRAK,
					(byte)SpellByte.QAKE,
					(byte)SpellByte.QAKE,
					(byte)SpellByte.NUKE,
					(byte)SpellByte.SLO2,
					(byte)SpellByte.RUB,
					(byte)SpellByte.XXXX,
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
				Name = "ECHIDNA",
				ElementalWeakness = SpellElement.Fire,
				MonsterType = MonsterType.MAGE,
				SpellChance1 = 0x40,
				Spells1 = new List<byte>
				{
					(byte)SpellByte.STUN,
					(byte)SpellByte.STOP,
					(byte)SpellByte.STUN,
					(byte)SpellByte.SLOW,
					(byte)SpellByte.STUN,
					(byte)SpellByte.STOP,
					(byte)SpellByte.STUN,
					(byte)SpellByte.SLOW,
				},
				SkillChance1 = 0x40,
				Skills1 = new List<byte>
				{
					(byte)EnemySkills.Dazzle,
					(byte)EnemySkills.Ink,
					(byte)EnemySkills.Ink,
					(byte)EnemySkills.Crack,
				},
				SpellChance2 = 0x40,
				Spells2 = new List<byte>
				{
					(byte)SpellByte.STUN,
					(byte)SpellByte.STOP,
					(byte)SpellByte.ZAP,
					(byte)SpellByte.STOP,
					(byte)SpellByte.WALL,
					(byte)SpellByte.XFER,
					(byte)SpellByte.STUN,
					(byte)SpellByte.XXXX,
				},
				SkillChance2 = 0x40,
				Skills2 = new List<byte>
				{
					(byte)EnemySkills.Crack,
					(byte)EnemySkills.Trance,
					(byte)EnemySkills.Crack,
					(byte)EnemySkills.Gaze,
				},
			},
			new AlternateFiends
			{
				Name = "GARUDA",
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
					(byte)EnemySkills.Dazzle,
					(byte)EnemySkills.Flash,
					(byte)EnemySkills.Ink,
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
					(byte)EnemySkills.Thunder,
					(byte)EnemySkills.Nuclear,
					(byte)EnemySkills.Toxic,
					(byte)EnemySkills.Poison_Damage,
				},
			},
			new AlternateFiends
			{
				Name = "GENERAL",
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
					(byte)EnemySkills.Flash,
					(byte)EnemySkills.Stare,
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
					(byte)EnemySkills.Trance,
					(byte)EnemySkills.Squint,
					(byte)EnemySkills.Toxic,
					(byte)EnemySkills.Crack,
				},
			},
			new AlternateFiends
			{
				Name = "GOLDOR",
				ElementalWeakness = SpellElement.None,
				MonsterType = MonsterType.MAGICAL,
				SpellChance1 = 0x40,
				Spells1 = new List<byte>
				{
					(byte)SpellByte.FIR2,
					(byte)SpellByte.LIT2,
					(byte)SpellByte.ICE2,
					(byte)SpellByte.MUTE,
					(byte)SpellByte.AFIR,
					(byte)SpellByte.AICE,
					(byte)SpellByte.ALIT,
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
					(byte)SpellByte.FIR3,
					(byte)SpellByte.NUKE,
					(byte)SpellByte.LIT3,
					(byte)SpellByte.ICE3,
					(byte)SpellByte.WALL,
					(byte)SpellByte.MUTE,
					(byte)SpellByte.SLO2,
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
				Name = "GUARDIN",
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
					(byte)EnemySkills.Crack,
					(byte)EnemySkills.Stinger,
					(byte)EnemySkills.Trance,
					(byte)EnemySkills.Gaze,
				},
				SpellChance2 = 0x40,
				Spells2 = new List<byte>
				{
					(byte)SpellByte.WALL,
					(byte)SpellByte.CUR4,
					(byte)SpellByte.SLO2,
					(byte)SpellByte.FAST,
					(byte)SpellByte.INVS,
					(byte)SpellByte.SLP2,
					(byte)SpellByte.QAKE,
					(byte)SpellByte.QAKE,
				},
				SkillChance2 = 0x40,
				Skills2 = new List<byte>
				{
					(byte)EnemySkills.Crack,
					(byte)EnemySkills.Stare,
					(byte)EnemySkills.Dazzle,
					(byte)EnemySkills.Tornado,
				},
			},
			new AlternateFiends
			{
				Name = "GUTSCO",
				ElementalWeakness = SpellElement.Ice,
				MonsterType = MonsterType.GIANT,
				SpellChance1 = 0x40,
				Spells1 = new List<byte>
				{
					(byte)SpellByte.FIRE,
					(byte)SpellByte.FIR2,
					(byte)SpellByte.SLOW,
					(byte)SpellByte.FIRE,
					(byte)SpellByte.FIR2,
					(byte)SpellByte.STUN,
					(byte)SpellByte.BLND,
					(byte)SpellByte.FIRE,
				},
				SkillChance1 = 0x40,
				Skills1 = new List<byte>
				{
					(byte)EnemySkills.Cremate,
					(byte)EnemySkills.Heat,
					(byte)EnemySkills.Stinger,
					(byte)EnemySkills.Trance,
				},
				SpellChance2 = 0x40,
				Spells2 = new List<byte>
				{
					(byte)SpellByte.FIR3,
					(byte)SpellByte.FIR3,
					(byte)SpellByte.FAST,
					(byte)SpellByte.FOG,
					(byte)SpellByte.SLO2,
					(byte)SpellByte.SLP2,
					(byte)SpellByte.FIR3,
					(byte)SpellByte.LOK2,
				},
				SkillChance2 = 0x40,
				Skills2 = new List<byte>
				{
					(byte)EnemySkills.Blaze,
					(byte)EnemySkills.Nuclear,
					(byte)EnemySkills.Cremate,
					(byte)EnemySkills.Squint,
				},
			},
			new AlternateFiends
			{
				Name = "HECATON",
				ElementalWeakness = SpellElement.Earth,
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
					(byte)EnemySkills.Gaze,
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
					(byte)EnemySkills.Crack,
					(byte)EnemySkills.Squint,
					(byte)EnemySkills.Toxic,
					(byte)EnemySkills.Glare,
				},
			},
			new AlternateFiends
			{
				Name = "KUNOICHI",
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
					(byte)EnemySkills.Dazzle,
					(byte)EnemySkills.Gaze,
					(byte)EnemySkills.Trance,
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
					(byte)EnemySkills.Trance,
					(byte)EnemySkills.Poison_Damage,
					(byte)EnemySkills.Glare,
				},
			},
			new AlternateFiends
			{
				Name = "LEVIATHN",
				ElementalWeakness = SpellElement.Lightning,
				MonsterType = MonsterType.GIANT,
				SpellChance1 = 0x40,
				Spells1 = new List<byte>
				{
					(byte)SpellByte.ICE,
					(byte)SpellByte.ICE2,
					(byte)SpellByte.ICE,
					(byte)SpellByte.STOP,
					(byte)SpellByte.ICE,
					(byte)SpellByte.ICE2,
					(byte)SpellByte.ICE,
					(byte)SpellByte.STOP,
				},
				SkillChance1 = 0x40,
				Skills1 = new List<byte>
				{
					(byte)EnemySkills.Frost,
					(byte)EnemySkills.Dazzle,
					(byte)EnemySkills.Stinger,
					(byte)EnemySkills.Flash,
				},
				SpellChance2 = 0x40,
				Spells2 = new List<byte>
				{
					(byte)SpellByte.ICE2,
					(byte)SpellByte.ICE2,
					(byte)SpellByte.ICE3,
					(byte)SpellByte.ICE3,
					(byte)SpellByte.ICE2,
					(byte)SpellByte.ICE2,
					(byte)SpellByte.XFER,
					(byte)SpellByte.ICE3,
				},
				SkillChance2 = 0x40,
				Skills2 = new List<byte>
				{
					(byte)EnemySkills.Frost,
					(byte)EnemySkills.Swirl,
					(byte)EnemySkills.Blizzard,
					(byte)EnemySkills.Swirl,
				},
			},
			new AlternateFiends
			{
				Name = "LUCIFER",
				ElementalWeakness = SpellElement.Ice,
				MonsterType = MonsterType.REGENERATIVE,
				SpellChance1 = 0x40,
				Spells1 = new List<byte>
				{
					(byte)SpellByte.FIR2,
					(byte)SpellByte.FIRE,
					(byte)SpellByte.FIR2,
					(byte)SpellByte.SLOW,
					(byte)SpellByte.SLEP,
					(byte)SpellByte.BLND,
					(byte)SpellByte.MUTE,
					(byte)SpellByte.FIRE,
				},
				SkillChance1 = 0x40,
				Skills1 = new List<byte>
				{
					(byte)EnemySkills.Heat,
					(byte)EnemySkills.Scorch,
					(byte)EnemySkills.Trance,
					(byte)EnemySkills.Glance,
				},
				SpellChance2 = 0x40,
				Spells2 = new List<byte>
				{
					(byte)SpellByte.NUKE,
					(byte)SpellByte.FIR3,
					(byte)SpellByte.XFER,
					(byte)SpellByte.FIR3,
					(byte)SpellByte.XXXX,
					(byte)SpellByte.FAST,
					(byte)SpellByte.FOG2,
					(byte)SpellByte.TMPR,
				},
				SkillChance2 = 0x40,
				Skills2 = new List<byte>
				{
					(byte)EnemySkills.Inferno,
					(byte)EnemySkills.Toxic,
					(byte)EnemySkills.Poison_Damage,
					(byte)EnemySkills.Squint,
				},
			},
			new AlternateFiends
			{
				Name = "NEP.DRGN",
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
				SkillChance1 = 0x40,
				Skills1 = new List<byte>
				{
					(byte)EnemySkills.Stare,
					(byte)EnemySkills.Heat,
					(byte)EnemySkills.Cremate,
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
					(byte)EnemySkills.Toxic,
					(byte)EnemySkills.Inferno,
					(byte)EnemySkills.Tornado,
				},
			},
			new AlternateFiends
			{
				Name = "NINJI",
				ElementalWeakness = SpellElement.Time,
				MonsterType = MonsterType.NONE,
				SpellChance1 = 0x40,
				Spells1 = new List<byte>
				{
					(byte)SpellByte.FOG,
					(byte)SpellByte.INVS,
					(byte)SpellByte.BLND,
					(byte)SpellByte.BLND,
					(byte)SpellByte.STUN,
					(byte)SpellByte.SLEP,
					(byte)SpellByte.SLEP,
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
					(byte)SpellByte.FAST,
					(byte)SpellByte.SABR,
					(byte)SpellByte.FOG2,
					(byte)SpellByte.INVS,
					(byte)SpellByte.SLO2,
					(byte)SpellByte.SLP2,
					(byte)SpellByte.BANE,
					(byte)SpellByte.ZAP,
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
				Name = "SALAMAND",
				ElementalWeakness = SpellElement.Ice,
				MonsterType = MonsterType.DRAGON,
				SpellChance1 = 0x40,
				Spells1 = new List<byte>
				{
					(byte)SpellByte.FIRE,
					(byte)SpellByte.DARK,
					(byte)SpellByte.FIRE,
					(byte)SpellByte.FIR2,
					(byte)SpellByte.FIRE,
					(byte)SpellByte.DARK,
					(byte)SpellByte.FIRE,
					(byte)SpellByte.FIR2,
				},
				SkillChance1 = 0x40,
				Skills1 = new List<byte>
				{
					(byte)EnemySkills.Flash,
					(byte)EnemySkills.Heat,
					(byte)EnemySkills.Flash,
					(byte)EnemySkills.Heat,
				},
				SpellChance2 = 0x40,
				Spells2 = new List<byte>
				{
					(byte)SpellByte.FIR2,
					(byte)SpellByte.FIR3,
					(byte)SpellByte.FIR3,
					(byte)SpellByte.NUKE,
					(byte)SpellByte.FIR2,
					(byte)SpellByte.FIR2,
					(byte)SpellByte.FIR3,
					(byte)SpellByte.DARK,
				},
				SkillChance2 = 0x40,
				Skills2 = new List<byte>
				{
					(byte)EnemySkills.Scorch,
					(byte)EnemySkills.Blaze,
					(byte)EnemySkills.Heat,
					(byte)EnemySkills.Inferno,
				},
			},
			new AlternateFiends
			{
				Name = "SCYLLA",
				ElementalWeakness = SpellElement.Ice,
				MonsterType = MonsterType.AQUATIC,
				SpellChance1 = 0x40,
				Spells1 = new List<byte>
				{
					(byte)SpellByte.LIT,
					(byte)SpellByte.LIT2,
					(byte)SpellByte.LIT,
					(byte)SpellByte.LIT2,
					(byte)SpellByte.LIT,
					(byte)SpellByte.LIT2,
					(byte)SpellByte.LIT,
					(byte)SpellByte.LIT2,
				},
				SkillChance1 = 0x40,
				Skills1 = new List<byte>
				{
					(byte)EnemySkills.Dazzle,
					(byte)EnemySkills.Flash,
					(byte)EnemySkills.Glance,
					(byte)EnemySkills.Flash,
				},
				SpellChance2 = 0x40,
				Spells2 = new List<byte>
				{
					(byte)SpellByte.LIT2,
					(byte)SpellByte.LIT2,
					(byte)SpellByte.LIT3,
					(byte)SpellByte.SLO2,
					(byte)SpellByte.LIT2,
					(byte)SpellByte.LIT2,
					(byte)SpellByte.LIT3,
					(byte)SpellByte.FAST,
				},
				SkillChance2 = 0x40,
				Skills2 = new List<byte>
				{
					(byte)EnemySkills.Thunder,
					(byte)EnemySkills.Dazzle,
					(byte)EnemySkills.Tornado,
					(byte)EnemySkills.Flash,
				},
			},
			new AlternateFiends
			{
				Name = "UNNE",
				ElementalWeakness = SpellElement.Status,
				MonsterType = MonsterType.MAGE,
				SpellChance1 = 0x40,
				Spells1 = new List<byte>
				{
					(byte)SpellByte.ICE,
					(byte)SpellByte.ICE,
					(byte)SpellByte.SLOW,
					(byte)SpellByte.ICE,
					(byte)SpellByte.FOG,
					(byte)SpellByte.MUTE,
					(byte)SpellByte.SLOW,
					(byte)SpellByte.MUTE,
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
					(byte)SpellByte.SLO2,
					(byte)SpellByte.XFER,
					(byte)SpellByte.ICE3,
					(byte)SpellByte.BANE,
					(byte)SpellByte.MUTE,
					(byte)SpellByte.ICE2,
					(byte)SpellByte.ICE2,
				},
				SkillChance2 = 0x40,
				Skills2 = new List<byte>
				{
					(byte)EnemySkills.Blizzard,
					(byte)EnemySkills.Tornado,
					(byte)EnemySkills.Frost,
					(byte)EnemySkills.Glare,
				},
			},
			new AlternateFiends
			{
				Name = "ZANDE",
				ElementalWeakness = SpellElement.None,
				MonsterType = MonsterType.MAGE,
				SpellChance1 = 0x40,
				Spells1 = new List<byte>
				{
					(byte)SpellByte.FIRE,
					(byte)SpellByte.LIT2,
					(byte)SpellByte.FIR2,
					(byte)SpellByte.CUR2,
					(byte)SpellByte.HOLD,
					(byte)SpellByte.LIT,
					(byte)SpellByte.HOLD,
					(byte)SpellByte.BLND,
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
					(byte)SpellByte.LIT3,
					(byte)SpellByte.FIR3,
					(byte)SpellByte.QAKE,
					(byte)SpellByte.FIR3,
					(byte)SpellByte.XXXX,
					(byte)SpellByte.SLP2,
					(byte)SpellByte.RUB,
				},
				SkillChance2 = 0x40,
				Skills2 = new List<byte>
				{
					(byte)EnemySkills.Crack,
					(byte)EnemySkills.Nuclear,
					(byte)EnemySkills.Blaze,
					(byte)EnemySkills.Toxic,
				},
			},
		};
	}
}
