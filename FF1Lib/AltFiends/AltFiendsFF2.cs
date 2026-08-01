namespace FF1Lib
{
	public partial class FF1Rom
	{
		List<AlternateFiends> FF2AltFiendslist = new()
		{
			new AlternateFiends
			{
				Name = "ADMNTOSE",
				ElementalWeakness = SpellElement.Fire,
				MonsterType = MonsterType.DRAGON,
				SpellChance1 = 0x40,
				Spells1 = new List<byte>
				{
					(byte)SpellByte.FAST,
					(byte)SpellByte.FOG,
					(byte)SpellByte.AFIR,
					(byte)SpellByte.AICE,
					(byte)SpellByte.TMPR,
					(byte)SpellByte.FOG,
					(byte)SpellByte.INVS,
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
					(byte)SpellByte.FAST,
					(byte)SpellByte.SABR,
					(byte)SpellByte.INVS,
					(byte)SpellByte.LOCK,
					(byte)SpellByte.FAST,
					(byte)SpellByte.SABR,
					(byte)SpellByte.INVS,
					(byte)SpellByte.CUR4,
				},
				SkillChance2 = 0x40,
				Skills2 = new List<byte>
				{
					(byte)EnemySkills.Ink,
					(byte)EnemySkills.Ink,
					(byte)EnemySkills.Ink,
					(byte)EnemySkills.Ink,
				},
			},
			new AlternateFiends
			{
				Name = "ASTAROTH",
				MonsterType = MonsterType.MAGE,
				SpellChance1 = 0x40,
				Spells1 = new List<byte>
				{
					(byte)SpellByte.FIRE,
					(byte)SpellByte.FIRE,
					(byte)SpellByte.BRAK,
					(byte)SpellByte.FIRE,
					(byte)SpellByte.FIRE,
					(byte)SpellByte.FIRE,
					(byte)SpellByte.BRAK,
					(byte)SpellByte.FIRE,
				},
				SkillChance1 = 0x40,
				Skills1 = new List<byte>
				{
					(byte)EnemySkills.Heat,
					(byte)EnemySkills.Heat,
					(byte)EnemySkills.Heat,
					(byte)EnemySkills.Poison_Stone,
				},
				SpellChance2 = 0x40,
				Spells2 = new List<byte>
				{
					(byte)SpellByte.FIR2,
					(byte)SpellByte.FIR3,
					(byte)SpellByte.FIR2,
					(byte)SpellByte.BRAK,
					(byte)SpellByte.FIR2,
					(byte)SpellByte.SLOW,
					(byte)SpellByte.FIR2,
					(byte)SpellByte.BANE,
				},
				SkillChance2 = 0x40,
				Skills2 = new List<byte>
				{
					(byte)EnemySkills.Scorch,
					(byte)EnemySkills.Poison_Damage,
					(byte)EnemySkills.Blaze,
					(byte)EnemySkills.Poison_Stone,
				},
			},
			new AlternateFiends
			{
				Name = "BELZEBUB",
				SpriteSheet = FormationSpriteSheet.SlimeSpiderManticorAnkylo,
				ElementalWeakness = SpellElement.Lightning,
				MonsterType = MonsterType.UNDEAD,
				SpellChance1 = 0x40,
				Spells1 = new List<byte>
				{
					(byte)SpellByte.FIRE,
					(byte)SpellByte.FAST,
					(byte)SpellByte.FIRE,
					(byte)SpellByte.FOG,
					(byte)SpellByte.FIRE,
					(byte)SpellByte.SLOW,
					(byte)SpellByte.FIRE,
					(byte)SpellByte.RUB,
				},
				SkillChance1 = 0x40,
				Skills1 = new List<byte>
				{
					(byte)EnemySkills.Flash,
					(byte)EnemySkills.Stinger,
					(byte)EnemySkills.Flash,
					(byte)EnemySkills.Dazzle,
				},
				SpellChance2 = 0x40,
				Spells2 = new List<byte>
				{
					(byte)SpellByte.FIR2,
					(byte)SpellByte.FAST,
					(byte)SpellByte.FIR3,
					(byte)SpellByte.FOG,
					(byte)SpellByte.FIR2,
					(byte)SpellByte.SLO2,
					(byte)SpellByte.XFER,
					(byte)SpellByte.BANE,
				},
				SkillChance2 = 0x40,
				Skills2 = new List<byte>
				{
					(byte)EnemySkills.Nuclear,
					(byte)EnemySkills.Flash,
					(byte)EnemySkills.Dazzle,
					(byte)EnemySkills.Inferno,
				},
			},
			new AlternateFiends
			{
				Name = "BORGEN",
				SpriteSheet = FormationSpriteSheet.MedusaCatmanPedeTiger,
				ElementalWeakness = SpellElement.Fire,
				MonsterType = MonsterType.GIANT,
				SpellChance1 = 0x40,
				Spells1 = new List<byte>
				{
					(byte)SpellByte.CUR2,
					(byte)SpellByte.CURE,
					(byte)SpellByte.FIRE,
					(byte)SpellByte.AFIR,
					(byte)SpellByte.FIR2,
					(byte)SpellByte.HEAL,
					(byte)SpellByte.HEL2,
					(byte)SpellByte.CURE,
				},
				SkillChance1 = 0x40,
				Skills1 = new List<byte>
				{
					(byte)EnemySkills.Stinger,
					(byte)EnemySkills.Snorting,
					(byte)EnemySkills.Dazzle,
					(byte)EnemySkills.Trance,
				},
				SpellChance2 = 0x40,
				Spells2 = new List<byte>
				{
					(byte)SpellByte.FOG,
					(byte)SpellByte.FIR3,
					(byte)SpellByte.HOLD,
					(byte)SpellByte.XFER,
					(byte)SpellByte.BANE,
					(byte)SpellByte.ZAP,
					(byte)SpellByte.MUTE,
					(byte)SpellByte.STOP,
				},
				SkillChance2 = 0x40,
				Skills2 = new List<byte>
				{
					(byte)EnemySkills.Gaze,
					(byte)EnemySkills.Trance,
					(byte)EnemySkills.Gaze,
					(byte)EnemySkills.Blaze,
				},
			},
			new AlternateFiends
			{
				Name = "BEHEMOTH",
				ElementalWeakness = SpellElement.Ice,
				MonsterType = MonsterType.UNDEAD,
				SpellChance1 = 0x40,
				Spells1 = new List<byte>
				{
					(byte)SpellByte.FIR2,
					(byte)SpellByte.STUN,
					(byte)SpellByte.FIRE,
					(byte)SpellByte.FAST,
					(byte)SpellByte.FIR2,
					(byte)SpellByte.STUN,
					(byte)SpellByte.FIRE,
					(byte)SpellByte.FAST,
				},
				SkillChance1 = 0x40,
				Skills1 = new List<byte>
				{
					(byte)EnemySkills.Snorting,
					(byte)EnemySkills.Snorting,
					(byte)EnemySkills.Snorting,
					(byte)EnemySkills.Snorting,
				},
				SpellChance2 = 0x40,
				Spells2 = new List<byte>
				{
					(byte)SpellByte.FIR3,
					(byte)SpellByte.STOP,
					(byte)SpellByte.FIR2,
					(byte)SpellByte.FAST,
					(byte)SpellByte.FIR3,
					(byte)SpellByte.SLO2,
					(byte)SpellByte.FIR2,
					(byte)SpellByte.NUKE,
				},
				SkillChance2 = 0x40,
				Skills2 = new List<byte>
				{
					(byte)EnemySkills.Snorting,
					(byte)EnemySkills.Snorting,
					(byte)EnemySkills.Snorting,
					(byte)EnemySkills.Blaze,
				},
			},
			new AlternateFiends
			{
				Name = "B.KNIGHT",
				SpriteSheet = FormationSpriteSheet.BadmanAstosMadponyWarmech,
				ElementalWeakness = SpellElement.Poison,
				MonsterType = MonsterType.MAGE,
				SpellChance1 = 0x40,
				Spells1 = new List<byte>
				{
					(byte)SpellByte.FAST,
					(byte)SpellByte.SLOW,
					(byte)SpellByte.STUN,
					(byte)SpellByte.STOP,
					(byte)SpellByte.FAST,
					(byte)SpellByte.SLOW,
					(byte)SpellByte.STUN,
					(byte)SpellByte.STOP,
				},
				SkillChance1 = 0x40,
				Skills1 = new List<byte>
				{
					(byte)EnemySkills.Flash,
					(byte)EnemySkills.Flash,
					(byte)EnemySkills.Flash,
					(byte)EnemySkills.Dazzle,
				},
				SpellChance2 = 0x40,
				Spells2 = new List<byte>
				{
					(byte)SpellByte.MUTE,
					(byte)SpellByte.SLO2,
					(byte)SpellByte.FAST,
					(byte)SpellByte.SABR,
					(byte)SpellByte.STUN,
					(byte)SpellByte.STOP,
					(byte)SpellByte.SABR,
					(byte)SpellByte.BRAK,
				},
				SkillChance2 = 0x40,
				Skills2 = new List<byte>
				{
					(byte)EnemySkills.Trance,
					(byte)EnemySkills.Flash,
					(byte)EnemySkills.Ink,
					(byte)EnemySkills.Glare,
				},
			},
			new AlternateFiends
			{
				Name = "GOTUS",
				SpriteSheet = FormationSpriteSheet.KrakenTiamat,
				ElementalWeakness = SpellElement.Poison,
				MonsterType = MonsterType.GIANT,
				SpellChance1 = 0x40,
				Spells1 = new List<byte>
				{
					(byte)SpellByte.SLOW,
					(byte)SpellByte.DARK,
					(byte)SpellByte.SLOW,
					(byte)SpellByte.LIT,
					(byte)SpellByte.FIRE,
					(byte)SpellByte.ICE,
					(byte)SpellByte.DARK,
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
					(byte)SpellByte.FAST,
					(byte)SpellByte.FAST,
					(byte)SpellByte.SABR,
					(byte)SpellByte.TMPR,
					(byte)SpellByte.SABR,
					(byte)SpellByte.ZAP,
					(byte)SpellByte.XXXX,
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
				Name = "IROGIANT",
				ElementalWeakness = SpellElement.Lightning,
				MonsterType = MonsterType.GIANT,
				SpellChance1 = 0x40,
				Spells1 = new List<byte>
				{
					(byte)SpellByte.FOG,
					(byte)SpellByte.STUN,
					(byte)SpellByte.BLND,
					(byte)SpellByte.FIR2,
					(byte)SpellByte.HOLD,
					(byte)SpellByte.STUN,
					(byte)SpellByte.FIRE,
					(byte)SpellByte.CURE,
				},
				SkillChance1 = 0x40,
				Skills1 = new List<byte>
				{
					(byte)EnemySkills.Stinger,
					(byte)EnemySkills.Flash,
					(byte)EnemySkills.Trance,
					(byte)EnemySkills.Stinger,
				},
				SpellChance2 = 0x40,
				Spells2 = new List<byte>
				{
					(byte)SpellByte.FIR2,
					(byte)SpellByte.NUKE,
					(byte)SpellByte.XFER,
					(byte)SpellByte.XXXX,
					(byte)SpellByte.SLO2,
					(byte)SpellByte.MUTE,
					(byte)SpellByte.FIR2,
					(byte)SpellByte.CUR3,
				},
				SkillChance2 = 0x40,
				Skills2 = new List<byte>
				{
					(byte)EnemySkills.Poison_Damage,
					(byte)EnemySkills.Blaze,
					(byte)EnemySkills.Glare,
					(byte)EnemySkills.Toxic,
				},
			},
			new AlternateFiends
			{
				Name = "LAMQUEEN",
				ElementalWeakness = SpellElement.Time,
				MonsterType = MonsterType.MAGICAL,
				SpellChance1 = 0x40,
				Spells1 = new List<byte>
				{
					(byte)SpellByte.FOG,
					(byte)SpellByte.STUN,
					(byte)SpellByte.ICE2,
					(byte)SpellByte.FOG,
					(byte)SpellByte.SLEP,
					(byte)SpellByte.SLOW,
					(byte)SpellByte.ICE2,
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
					(byte)SpellByte.INVS,
					(byte)SpellByte.MUTE,
					(byte)SpellByte.XXXX,
					(byte)SpellByte.FOG2,
					(byte)SpellByte.SLO2,
					(byte)SpellByte.FIR3,
					(byte)SpellByte.ICE3,
					(byte)SpellByte.CUR4,
				},
				SkillChance2 = 0x40,
				Skills2 = new List<byte>
				{
					(byte)EnemySkills.Poison_Stone,
					(byte)EnemySkills.Tornado,
					(byte)EnemySkills.Ink,
					(byte)EnemySkills.Poison_Damage,
				},
			},
			new AlternateFiends
			{
				Name = "MEDUSAE",
				ElementalWeakness = SpellElement.Poison,
				MonsterType = MonsterType.MAGE,
				SpellChance1 = 0x40,
				Spells1 = new List<byte>
				{
					(byte)SpellByte.STUN,
					(byte)SpellByte.STOP,
					(byte)SpellByte.STUN,
					(byte)SpellByte.BRAK,
					(byte)SpellByte.STUN,
					(byte)SpellByte.STOP,
					(byte)SpellByte.STUN,
					(byte)SpellByte.BRAK,
				},
				SkillChance1 = 0x40,
				Skills1 = new List<byte>
				{
					(byte)EnemySkills.Gaze,
					(byte)EnemySkills.Glance,
					(byte)EnemySkills.Gaze,
					(byte)EnemySkills.Glare,
				},
				SpellChance2 = 0x40,
				Spells2 = new List<byte>
				{
					(byte)SpellByte.STUN,
					(byte)SpellByte.STOP,
					(byte)SpellByte.XXXX,
					(byte)SpellByte.BRAK,
					(byte)SpellByte.STUN,
					(byte)SpellByte.STOP,
					(byte)SpellByte.XXXX,
					(byte)SpellByte.BRAK,
				},
				SkillChance2 = 0x40,
				Skills2 = new List<byte>
				{
					(byte)EnemySkills.Glare,
					(byte)EnemySkills.Glance,
					(byte)EnemySkills.Glare,
					(byte)EnemySkills.Poison_Stone,
				},
			},
			new AlternateFiends
			{
				Name = "RNDWORM",
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
					(byte)EnemySkills.Crack,
					(byte)EnemySkills.Trance,
					(byte)EnemySkills.Ink,
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
					(byte)EnemySkills.Nuclear,
					(byte)EnemySkills.Toxic,
					(byte)EnemySkills.Thunder,
				},
			},
			new AlternateFiends
			{
				Name = "SERGEANT",
				ElementalWeakness = SpellElement.Lightning,
				MonsterType = MonsterType.GIANT,
				SpellChance1 = 0x40,
				Spells1 = new List<byte>
				{
					(byte)SpellByte.CURE,
					(byte)SpellByte.HEAL,
					(byte)SpellByte.MUTE,
					(byte)SpellByte.XFER,
					(byte)SpellByte.BLND,
					(byte)SpellByte.ICE,
					(byte)SpellByte.CURE,
					(byte)SpellByte.MUTE,
				},
				SkillChance1 = 0x40,
				Skills1 = new List<byte>
				{
					(byte)EnemySkills.Gaze,
					(byte)EnemySkills.Glance,
					(byte)EnemySkills.Stare,
					(byte)EnemySkills.Dazzle,
				},
				SpellChance2 = 0x40,
				Spells2 = new List<byte>
				{
					(byte)SpellByte.FAST,
					(byte)SpellByte.TMPR,
					(byte)SpellByte.BLND,
					(byte)SpellByte.XFER,
					(byte)SpellByte.XXXX,
					(byte)SpellByte.ICE3,
					(byte)SpellByte.BANE,
					(byte)SpellByte.FOG,
				},
				SkillChance2 = 0x40,
				Skills2 = new List<byte>
				{
					(byte)EnemySkills.Blizzard,
					(byte)EnemySkills.Nuclear,
					(byte)EnemySkills.Glare,
					(byte)EnemySkills.Squint,
				},
			},
			new AlternateFiends
			{
				Name = "TWHD.DRG",
				ElementalWeakness = SpellElement.Time,
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
					(byte)EnemySkills.Dazzle,
					(byte)EnemySkills.Flash,
					(byte)EnemySkills.Stinger,
				},
				SpellChance2 = 0x40,
				Spells2 = new List<byte>
				{
					(byte)SpellByte.TMPR,
					(byte)SpellByte.FAST,
					(byte)SpellByte.XFER,
					(byte)SpellByte.INVS,
					(byte)SpellByte.TMPR,
					(byte)SpellByte.SLO2,
					(byte)SpellByte.CUR3,
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
		};

		List<AlternateFiends> FF2AltFiendListHardcore = new()
		{
			new AlternateFiends
			{
				Name = "ADMNTOSE",
				ElementalWeakness = SpellElement.Fire,
				MonsterType = MonsterType.DRAGON,
				SpellChance1 = 0x40,
				Spells1 = new List<byte>
				{
					(byte)SpellByte.FAST,
					(byte)SpellByte.FOG,
					(byte)SpellByte.AFIR,
					(byte)SpellByte.AICE,
					(byte)SpellByte.TMPR,
					(byte)SpellByte.FOG,
					(byte)SpellByte.INVS,
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
					(byte)SpellByte.FAST,
					(byte)SpellByte.SABR,
					(byte)SpellByte.INVS,
					(byte)SpellByte.LOCK,
					(byte)SpellByte.FAST,
					(byte)SpellByte.SABR,
					(byte)SpellByte.INVS,
					(byte)SpellByte.CUR4,
				},
				SkillChance2 = 0x40,
				Skills2 = new List<byte>
				{
					(byte)EnemySkills.Ink,
					(byte)EnemySkills.Ink,
					(byte)EnemySkills.Ink,
					(byte)EnemySkills.Ink,
				},
			},
			new AlternateFiends
			{
				Name = "ASTAROTH",
				MonsterType = MonsterType.MAGE,
				SpellChance1 = 0x40,
				Spells1 = new List<byte>
				{
					(byte)SpellByte.FIRE,
					(byte)SpellByte.FIRE,
					(byte)SpellByte.BRAK,
					(byte)SpellByte.FIRE,
					(byte)SpellByte.FIRE,
					(byte)SpellByte.FIRE,
					(byte)SpellByte.BRAK,
					(byte)SpellByte.FIRE,
				},
				SkillChance1 = 0x40,
				Skills1 = new List<byte>
				{
					(byte)EnemySkills.Heat,
					(byte)EnemySkills.Heat,
					(byte)EnemySkills.Heat,
					(byte)EnemySkills.Poison_Stone,
				},
				SpellChance2 = 0x40,
				Spells2 = new List<byte>
				{
					(byte)SpellByte.FIR2,
					(byte)SpellByte.FIR3,
					(byte)SpellByte.FIR2,
					(byte)SpellByte.BRAK,
					(byte)SpellByte.FIR2,
					(byte)SpellByte.FIR3,
					(byte)SpellByte.FIR2,
					(byte)SpellByte.BANE,
				},
				SkillChance2 = 0x40,
				Skills2 = new List<byte>
				{
					(byte)EnemySkills.Scorch,
					(byte)EnemySkills.Poison_Damage,
					(byte)EnemySkills.Blaze,
					(byte)EnemySkills.Poison_Stone,
				},
			},
			new AlternateFiends
			{
				Name = "BELZEBUB",
				ElementalWeakness = SpellElement.Lightning,
				MonsterType = MonsterType.UNDEAD,
				SpellChance1 = 0x40,
				Spells1 = new List<byte>
				{
					(byte)SpellByte.FIRE,
					(byte)SpellByte.FAST,
					(byte)SpellByte.FIRE,
					(byte)SpellByte.FOG,
					(byte)SpellByte.FIRE,
					(byte)SpellByte.SLOW,
					(byte)SpellByte.FIRE,
					(byte)SpellByte.RUB,
				},
				SkillChance1 = 0x40,
				Skills1 = new List<byte>
				{
					(byte)EnemySkills.Flash,
					(byte)EnemySkills.Stinger,
					(byte)EnemySkills.Flash,
					(byte)EnemySkills.Dazzle,
				},
				SpellChance2 = 0x40,
				Spells2 = new List<byte>
				{
					(byte)SpellByte.FIR2,
					(byte)SpellByte.FAST,
					(byte)SpellByte.FIR3,
					(byte)SpellByte.FOG,
					(byte)SpellByte.FIR2,
					(byte)SpellByte.SLO2,
					(byte)SpellByte.FIR3,
					(byte)SpellByte.BANE,
				},
				SkillChance2 = 0x40,
				Skills2 = new List<byte>
				{
					(byte)EnemySkills.Nuclear,
					(byte)EnemySkills.Flash,
					(byte)EnemySkills.Dazzle,
					(byte)EnemySkills.Inferno,
				},
			},
			new AlternateFiends
			{
				Name = "BORGEN",
				ElementalWeakness = SpellElement.Fire,
				MonsterType = MonsterType.GIANT,
				SpellChance1 = 0x40,
				Spells1 = new List<byte>
				{
					(byte)SpellByte.CUR2,
					(byte)SpellByte.CURE,
					(byte)SpellByte.FIRE,
					(byte)SpellByte.AFIR,
					(byte)SpellByte.FIR2,
					(byte)SpellByte.HEAL,
					(byte)SpellByte.HEL2,
					(byte)SpellByte.CURE,
				},
				SkillChance1 = 0x40,
				Skills1 = new List<byte>
				{
					(byte)EnemySkills.Stinger,
					(byte)EnemySkills.Snorting,
					(byte)EnemySkills.Dazzle,
					(byte)EnemySkills.Trance,
				},
				SpellChance2 = 0x40,
				Spells2 = new List<byte>
				{
					(byte)SpellByte.NUKE,
					(byte)SpellByte.FIR3,
					(byte)SpellByte.HOLD,
					(byte)SpellByte.XFER,
					(byte)SpellByte.BANE,
					(byte)SpellByte.ZAP,
					(byte)SpellByte.MUTE,
					(byte)SpellByte.STOP,
				},
				SkillChance2 = 0x40,
				Skills2 = new List<byte>
				{
					(byte)EnemySkills.Gaze,
					(byte)EnemySkills.Trance,
					(byte)EnemySkills.Gaze,
					(byte)EnemySkills.Blaze,
				},
			},
			new AlternateFiends
			{
				Name = "BEHEMOTH",
				ElementalWeakness = SpellElement.Ice,
				MonsterType = MonsterType.UNDEAD,
				SpellChance1 = 0x40,
				Spells1 = new List<byte>
				{
					(byte)SpellByte.FIR2,
					(byte)SpellByte.STUN,
					(byte)SpellByte.FIR2,
					(byte)SpellByte.FAST,
					(byte)SpellByte.FIR2,
					(byte)SpellByte.STUN,
					(byte)SpellByte.FIR2,
					(byte)SpellByte.FAST,
				},
				SkillChance1 = 0x40,
				Skills1 = new List<byte>
				{
					(byte)EnemySkills.Snorting,
					(byte)EnemySkills.Snorting,
					(byte)EnemySkills.Snorting,
					(byte)EnemySkills.Snorting,
				},
				SpellChance2 = 0x40,
				Spells2 = new List<byte>
				{
					(byte)SpellByte.FIR3,
					(byte)SpellByte.STOP,
					(byte)SpellByte.FIR3,
					(byte)SpellByte.FAST,
					(byte)SpellByte.FIR3,
					(byte)SpellByte.SLO2,
					(byte)SpellByte.FIR3,
					(byte)SpellByte.NUKE,
				},
				SkillChance2 = 0x40,
				Skills2 = new List<byte>
				{
					(byte)EnemySkills.Snorting,
					(byte)EnemySkills.Snorting,
					(byte)EnemySkills.Snorting,
					(byte)EnemySkills.Blaze,
				},
			},
			new AlternateFiends
			{
				Name = "B.KNIGHT",
				ElementalWeakness = SpellElement.Poison,
				MonsterType = MonsterType.MAGE,
				SpellChance1 = 0x40,
				Spells1 = new List<byte>
				{
					(byte)SpellByte.FAST,
					(byte)SpellByte.SLOW,
					(byte)SpellByte.STUN,
					(byte)SpellByte.STOP,
					(byte)SpellByte.FAST,
					(byte)SpellByte.SLOW,
					(byte)SpellByte.STUN,
					(byte)SpellByte.STOP,
				},
				SkillChance1 = 0x40,
				Skills1 = new List<byte>
				{
					(byte)EnemySkills.Flash,
					(byte)EnemySkills.Flash,
					(byte)EnemySkills.Flash,
					(byte)EnemySkills.Dazzle,
				},
				SpellChance2 = 0x40,
				Spells2 = new List<byte>
				{
					(byte)SpellByte.MUTE,
					(byte)SpellByte.SLO2,
					(byte)SpellByte.FAST,
					(byte)SpellByte.SABR,
					(byte)SpellByte.STUN,
					(byte)SpellByte.STOP,
					(byte)SpellByte.SABR,
					(byte)SpellByte.BRAK,
				},
				SkillChance2 = 0x40,
				Skills2 = new List<byte>
				{
					(byte)EnemySkills.Trance,
					(byte)EnemySkills.Flash,
					(byte)EnemySkills.Ink,
					(byte)EnemySkills.Glare,
				},
			},
			new AlternateFiends
			{
				Name = "GOTUS",
				ElementalWeakness = SpellElement.Poison,
				MonsterType = MonsterType.GIANT,
				SpellChance1 = 0x40,
				Spells1 = new List<byte>
				{
					(byte)SpellByte.SLOW,
					(byte)SpellByte.DARK,
					(byte)SpellByte.SLOW,
					(byte)SpellByte.LIT,
					(byte)SpellByte.FIRE,
					(byte)SpellByte.ICE,
					(byte)SpellByte.DARK,
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
					(byte)SpellByte.FAST,
					(byte)SpellByte.FAST,
					(byte)SpellByte.SABR,
					(byte)SpellByte.TMPR,
					(byte)SpellByte.SABR,
					(byte)SpellByte.ZAP,
					(byte)SpellByte.XXXX,
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
				Name = "IROGIANT",
				ElementalWeakness = SpellElement.Lightning,
				MonsterType = MonsterType.GIANT,
				SpellChance1 = 0x40,
				Spells1 = new List<byte>
				{
					(byte)SpellByte.FOG,
					(byte)SpellByte.STUN,
					(byte)SpellByte.BLND,
					(byte)SpellByte.FIR2,
					(byte)SpellByte.HOLD,
					(byte)SpellByte.STUN,
					(byte)SpellByte.FIRE,
					(byte)SpellByte.CURE,
				},
				SkillChance1 = 0x40,
				Skills1 = new List<byte>
				{
					(byte)EnemySkills.Stinger,
					(byte)EnemySkills.Flash,
					(byte)EnemySkills.Trance,
					(byte)EnemySkills.Stinger,
				},
				SpellChance2 = 0x40,
				Spells2 = new List<byte>
				{
					(byte)SpellByte.FIR3,
					(byte)SpellByte.NUKE,
					(byte)SpellByte.XFER,
					(byte)SpellByte.XXXX,
					(byte)SpellByte.SLO2,
					(byte)SpellByte.MUTE,
					(byte)SpellByte.FIR3,
					(byte)SpellByte.CUR3,
				},
				SkillChance2 = 0x40,
				Skills2 = new List<byte>
				{
					(byte)EnemySkills.Poison_Damage,
					(byte)EnemySkills.Blaze,
					(byte)EnemySkills.Glare,
					(byte)EnemySkills.Toxic,
				},
			},
			new AlternateFiends
			{
				Name = "LAMQUEEN",
				ElementalWeakness = SpellElement.Time,
				MonsterType = MonsterType.MAGICAL,
				SpellChance1 = 0x40,
				Spells1 = new List<byte>
				{
					(byte)SpellByte.FOG,
					(byte)SpellByte.STUN,
					(byte)SpellByte.ICE2,
					(byte)SpellByte.FOG,
					(byte)SpellByte.SLEP,
					(byte)SpellByte.SLOW,
					(byte)SpellByte.ICE2,
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
					(byte)SpellByte.INVS,
					(byte)SpellByte.MUTE,
					(byte)SpellByte.XXXX,
					(byte)SpellByte.FOG2,
					(byte)SpellByte.SLO2,
					(byte)SpellByte.FIR3,
					(byte)SpellByte.ICE3,
					(byte)SpellByte.CUR4,
				},
				SkillChance2 = 0x40,
				Skills2 = new List<byte>
				{
					(byte)EnemySkills.Poison_Stone,
					(byte)EnemySkills.Tornado,
					(byte)EnemySkills.Ink,
					(byte)EnemySkills.Poison_Damage,
				},
			},
			new AlternateFiends
			{
				Name = "MEDUSAE",
				ElementalWeakness = SpellElement.Poison,
				MonsterType = MonsterType.MAGE,
				SpellChance1 = 0x40,
				Spells1 = new List<byte>
				{
					(byte)SpellByte.STUN,
					(byte)SpellByte.STOP,
					(byte)SpellByte.STUN,
					(byte)SpellByte.BRAK,
					(byte)SpellByte.STUN,
					(byte)SpellByte.STOP,
					(byte)SpellByte.STUN,
					(byte)SpellByte.BRAK,
				},
				SkillChance1 = 0x40,
				Skills1 = new List<byte>
				{
					(byte)EnemySkills.Gaze,
					(byte)EnemySkills.Glance,
					(byte)EnemySkills.Gaze,
					(byte)EnemySkills.Glare,
				},
				SpellChance2 = 0x40,
				Spells2 = new List<byte>
				{
					(byte)SpellByte.STUN,
					(byte)SpellByte.STOP,
					(byte)SpellByte.XXXX,
					(byte)SpellByte.BRAK,
					(byte)SpellByte.STUN,
					(byte)SpellByte.STOP,
					(byte)SpellByte.XXXX,
					(byte)SpellByte.BRAK,
				},
				SkillChance2 = 0x40,
				Skills2 = new List<byte>
				{
					(byte)EnemySkills.Glare,
					(byte)EnemySkills.Glance,
					(byte)EnemySkills.Glare,
					(byte)EnemySkills.Poison_Stone,
				},
			},
			new AlternateFiends
			{
				Name = "RNDWORM",
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
					(byte)EnemySkills.Crack,
					(byte)EnemySkills.Trance,
					(byte)EnemySkills.Ink,
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
					(byte)EnemySkills.Nuclear,
					(byte)EnemySkills.Toxic,
					(byte)EnemySkills.Thunder,
				},
			},
			new AlternateFiends
			{
				Name = "SERGEANT",
				ElementalWeakness = SpellElement.Lightning,
				MonsterType = MonsterType.GIANT,
				SpellChance1 = 0x40,
				Spells1 = new List<byte>
				{
					(byte)SpellByte.CURE,
					(byte)SpellByte.HEAL,
					(byte)SpellByte.MUTE,
					(byte)SpellByte.XFER,
					(byte)SpellByte.BLND,
					(byte)SpellByte.ICE,
					(byte)SpellByte.CURE,
					(byte)SpellByte.MUTE,
				},
				SkillChance1 = 0x40,
				Skills1 = new List<byte>
				{
					(byte)EnemySkills.Gaze,
					(byte)EnemySkills.Glance,
					(byte)EnemySkills.Stare,
					(byte)EnemySkills.Dazzle,
				},
				SpellChance2 = 0x40,
				Spells2 = new List<byte>
				{
					(byte)SpellByte.FAST,
					(byte)SpellByte.TMPR,
					(byte)SpellByte.BLND,
					(byte)SpellByte.XFER,
					(byte)SpellByte.XXXX,
					(byte)SpellByte.ICE3,
					(byte)SpellByte.BANE,
					(byte)SpellByte.FOG,
				},
				SkillChance2 = 0x40,
				Skills2 = new List<byte>
				{
					(byte)EnemySkills.Blizzard,
					(byte)EnemySkills.Nuclear,
					(byte)EnemySkills.Glare,
					(byte)EnemySkills.Squint,
				},
			},
			new AlternateFiends
			{
				Name = "TWHD.DRG",
				ElementalWeakness = SpellElement.Time,
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
					(byte)EnemySkills.Dazzle,
					(byte)EnemySkills.Flash,
					(byte)EnemySkills.Stinger,
				},
				SpellChance2 = 0x40,
				Spells2 = new List<byte>
				{
					(byte)SpellByte.TMPR,
					(byte)SpellByte.FAST,
					(byte)SpellByte.XFER,
					(byte)SpellByte.INVS,
					(byte)SpellByte.TMPR,
					(byte)SpellByte.SLO2,
					(byte)SpellByte.CUR3,
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
		};
	}
}
