namespace FF1Lib
{
	public partial class FF1Rom
	{
		List<AlternateFiends> FF1MasterFiendList = new()
		{
			new AlternateFiends
			{
				Name = "LICH",
				SpriteSheet = FormationSpriteSheet.KaryLich,
				FormationPattern = FormationPattern.Fiends,
				GFXOffset = FormationGFX.Sprite3,
				Palette1 = 0x23,
				Palette2 = 0x25,
				ElementalWeakness = SpellElement.Fire,
				MonsterType = MonsterType.UNDEAD,
				SpellChance1 = 0x40,
				Spells1 = new List<byte>
				{
					(byte)SpellByte.ICE2,
					(byte)SpellByte.SLP2,
					(byte)SpellByte.FAST,
					(byte)SpellByte.LIT2,
					(byte)SpellByte.HOLD,
					(byte)SpellByte.FIR2,
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
					(byte)SpellByte.NUKE,
					(byte)SpellByte.STOP,
					(byte)SpellByte.ZAP,
					(byte)SpellByte.XXXX,
					(byte)SpellByte.NUKE,
					(byte)SpellByte.STOP,
					(byte)SpellByte.ZAP,
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
				Name = "KARY",
				SpriteSheet = FormationSpriteSheet.KrakenTiamat,
				FormationPattern = FormationPattern.Fiends,
				GFXOffset = FormationGFX.Sprite1,
				Palette1 = 0x23,
				Palette2 = 0x25,
				ElementalWeakness = SpellElement.Status,
				MonsterType = MonsterType.MAGE,
				SpellChance1 = 0x40,
				Spells1 = new List<byte>
				{
					(byte)SpellByte.FIR2,
					(byte)SpellByte.DARK,
					(byte)SpellByte.FIR2,
					(byte)SpellByte.DARK,
					(byte)SpellByte.FIR2,
					(byte)SpellByte.HOLD,
					(byte)SpellByte.FIR2,
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
					(byte)SpellByte.FIR3,
					(byte)SpellByte.RUB,
					(byte)SpellByte.FIR3,
					(byte)SpellByte.RUB,
					(byte)SpellByte.FIR3,
					(byte)SpellByte.STUN,
					(byte)SpellByte.FIR3,
					(byte)SpellByte.STUN,
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
				Name = "KRAKEN",
				SpriteSheet = FormationSpriteSheet.KaryLich,
				FormationPattern = FormationPattern.Fiends,
				GFXOffset = FormationGFX.Sprite2,
				Palette1 = 0x23,
				Palette2 = 0x25,
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
					(byte)EnemySkills.Ink,
					(byte)EnemySkills.Ink,
					(byte)EnemySkills.Ink,
					(byte)EnemySkills.Ink,
				},
				SpellChance2 = 0x40,
				Spells2 = new List<byte>
				{
					(byte)SpellByte.LIT2,
					(byte)SpellByte.LIT2,
					(byte)SpellByte.LIT2,
					(byte)SpellByte.LIT2,
					(byte)SpellByte.LIT2,
					(byte)SpellByte.LIT2,
					(byte)SpellByte.LIT2,
					(byte)SpellByte.LIT2,
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
				Name = "TIAMAT",
				SpriteSheet = FormationSpriteSheet.KrakenTiamat,
				FormationPattern = FormationPattern.Fiends,
				GFXOffset = FormationGFX.Sprite4,
				Palette1 = 0x23,
				Palette2 = 0x25,
				ElementalWeakness = SpellElement.Poison,
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
					(byte)EnemySkills.Thunder,
					(byte)EnemySkills.Poison_Damage,
					(byte)EnemySkills.Blizzard,
					(byte)EnemySkills.Blaze,
				},
				SpellChance2 = 0x40,
				Spells2 = new List<byte>
				{
					(byte)SpellByte.BANE,
					(byte)SpellByte.ICE2,
					(byte)SpellByte.LIT2,
					(byte)SpellByte.FIR2,
					(byte)SpellByte.BANE,
					(byte)SpellByte.ICE2,
					(byte)SpellByte.LIT2,
					(byte)SpellByte.FIR2,
				},
				SkillChance2 = 0x40,
				Skills2 = new List<byte>
				{
					(byte)EnemySkills.Thunder,
					(byte)EnemySkills.Poison_Damage,
					(byte)EnemySkills.Blizzard,
					(byte)EnemySkills.Blaze,
				},
			},
		};

		List<AlternateFiends> FF1BonusFiendsList = new()
		{
			new AlternateFiends
			{
				Name = "BEHOLDER",
				ElementalWeakness = SpellElement.Fire,
				MonsterType = MonsterType.MAGICAL,
				SpellChance1 = 0x40,
				Spells1 = new List<byte>
				{
					(byte)SpellByte.DARK,
					(byte)SpellByte.RUB,
					(byte)SpellByte.ICE,
					(byte)SpellByte.LIT,
					(byte)SpellByte.SLEP,
					(byte)SpellByte.SLOW,
					(byte)SpellByte.ICE2,
					(byte)SpellByte.LIT2,
				},
				SkillChance1 = 0x40,
				Skills1 = new List<byte>
				{
					(byte)EnemySkills.Stinger,
					(byte)EnemySkills.Ink,
					(byte)EnemySkills.Trance,
					(byte)EnemySkills.Frost,
				},
				SpellChance2 = 0x40,
				Spells2 = new List<byte>
				{
					(byte)SpellByte.FAST,
					(byte)SpellByte.XXXX,
					(byte)SpellByte.ICE3,
					(byte)SpellByte.INVS,
					(byte)SpellByte.LIT2,
					(byte)SpellByte.QAKE,
					(byte)SpellByte.LIT3,
					(byte)SpellByte.SLP2,
				},
				SkillChance2 = 0x40,
				Skills2 = new List<byte>
				{
					(byte)EnemySkills.Poison_Stone,
					(byte)EnemySkills.Blizzard,
					(byte)EnemySkills.Stare,
					(byte)EnemySkills.Toxic,
				},
			},
			new AlternateFiends
			{
				Name = "BIKKE",
				ElementalWeakness = SpellElement.Lightning,
				MonsterType = MonsterType.GIANT,
				SpellChance1 = 0x40,
				Spells1 = new List<byte>
				{
					(byte)SpellByte.STUN,
					(byte)SpellByte.HOLD,
					(byte)SpellByte.CUR2,
					(byte)SpellByte.SLEP,
					(byte)SpellByte.HOLD,
					(byte)SpellByte.CUR2,
					(byte)SpellByte.BRAK,
					(byte)SpellByte.STUN,
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
					(byte)SpellByte.STUN,
					(byte)SpellByte.BRAK,
					(byte)SpellByte.CUR3,
					(byte)SpellByte.QAKE,
					(byte)SpellByte.STUN,
					(byte)SpellByte.BRAK,
					(byte)SpellByte.CUR3,
					(byte)SpellByte.ZAP,
				},
				SkillChance2 = 0x40,
				Skills2 = new List<byte>
				{
					(byte)EnemySkills.Frost,
					(byte)EnemySkills.Blizzard,
					(byte)EnemySkills.Frost,
					(byte)EnemySkills.Tornado,
				},
			},
			new AlternateFiends
			{
				Name = "BUBBLES",
				ElementalWeakness = SpellElement.Lightning,
				MonsterType = MonsterType.AQUATIC,
				SpellChance1 = 0x40,
				Spells1 = new List<byte>
				{
					(byte)SpellByte.ICE,
					(byte)SpellByte.DARK,
					(byte)SpellByte.ICE2,
					(byte)SpellByte.DARK,
					(byte)SpellByte.ICE,
					(byte)SpellByte.DARK,
					(byte)SpellByte.ICE2,
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
					(byte)SpellByte.ICE3,
					(byte)SpellByte.XFER,
					(byte)SpellByte.FADE,
					(byte)SpellByte.ZAP,
					(byte)SpellByte.ICE2,
					(byte)SpellByte.BRAK,
					(byte)SpellByte.FADE,
					(byte)SpellByte.ZAP,
				},
				SkillChance2 = 0x40,
				Skills2 = new List<byte>
				{
					(byte)EnemySkills.Frost,
					(byte)EnemySkills.Poison_Stone,
					(byte)EnemySkills.Frost,
					(byte)EnemySkills.Blizzard,
				},
			},
			new AlternateFiends
			{
				Name = "CARAVAN",
				ElementalWeakness = SpellElement.Earth,
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
					(byte)EnemySkills.Gaze,
					(byte)EnemySkills.Trance,
					(byte)EnemySkills.Ink,
					(byte)EnemySkills.Stare,
				},
				SpellChance2 = 0x40,
				Spells2 = new List<byte>
				{
					(byte)SpellByte.FAST,
					(byte)SpellByte.ICE2,
					(byte)SpellByte.FOG,
					(byte)SpellByte.FIR2,
					(byte)SpellByte.CUR3,
					(byte)SpellByte.LIT2,
					(byte)SpellByte.SLO2,
					(byte)SpellByte.SABR,
				},
				SkillChance2 = 0x40,
				Skills2 = new List<byte>
				{
					(byte)EnemySkills.Poison_Damage,
					(byte)EnemySkills.Toxic,
					(byte)EnemySkills.Crack,
					(byte)EnemySkills.Dazzle,
				},
			},
			new AlternateFiends
			{
				Name = "DR.UNNE",
				ElementalWeakness = SpellElement.Fire,
				MonsterType = MonsterType.UNDEAD,
				SpellChance1 = 0x40,
				Spells1 = new List<byte>
				{
					(byte)SpellByte.ICE2,
					(byte)SpellByte.DARK,
					(byte)SpellByte.LIT,
					(byte)SpellByte.WALL,
					(byte)SpellByte.ICE,
					(byte)SpellByte.DARK,
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
					(byte)SpellByte.ICE3,
					(byte)SpellByte.ZAP,
					(byte)SpellByte.ICE2,
					(byte)SpellByte.WALL,
					(byte)SpellByte.ICE2,
					(byte)SpellByte.HOLD,
					(byte)SpellByte.ICE3,
					(byte)SpellByte.MUTE,
				},
				SkillChance2 = 0x40,
				Skills2 = new List<byte>
				{
					(byte)EnemySkills.Blizzard,
					(byte)EnemySkills.Poison_Stone,
					(byte)EnemySkills.Tornado,
					(byte)EnemySkills.Toxic,
				},
			},
			new AlternateFiends
			{
				Name = "EVILELF",
				ElementalWeakness = SpellElement.Poison,
				MonsterType = MonsterType.MAGE,
				SpellChance1 = 0x40,
				Spells1 = new List<byte>
				{
					(byte)SpellByte.LIT,
					(byte)SpellByte.FIRE,
					(byte)SpellByte.ICE2,
					(byte)SpellByte.CUR2,
					(byte)SpellByte.LIT2,
					(byte)SpellByte.FIR2,
					(byte)SpellByte.ICE2,
					(byte)SpellByte.CUR3,
				},
				SkillChance1 = 0x40,
				Skills1 = new List<byte>
				{
					(byte)EnemySkills.Ink,
					(byte)EnemySkills.Ink,
					(byte)EnemySkills.Ink,
					(byte)EnemySkills.Ink,
				},
				SpellChance2 = 0x40,
				Spells2 = new List<byte>
				{
					(byte)SpellByte.LIT3,
					(byte)SpellByte.XXXX,
					(byte)SpellByte.ICE3,
					(byte)SpellByte.CUR4,
					(byte)SpellByte.LIT3,
					(byte)SpellByte.BANE,
					(byte)SpellByte.ICE3,
					(byte)SpellByte.HEL3,
				},
				SkillChance2 = 0x40,
				Skills2 = new List<byte>
				{
					(byte)EnemySkills.Ink,
					(byte)EnemySkills.Ink,
					(byte)EnemySkills.Ink,
					(byte)EnemySkills.Tornado,
				},
			},
			new AlternateFiends
			{
				Name = "HAFGUFA",
				ElementalWeakness = SpellElement.Ice,
				MonsterType = MonsterType.AQUATIC,
				SpellChance1 = 0x40,
				Spells1 = new List<byte>
				{
					(byte)SpellByte.LIT2,
					(byte)SpellByte.DARK,
					(byte)SpellByte.LIT,
					(byte)SpellByte.INVS,
					(byte)SpellByte.LIT2,
					(byte)SpellByte.DARK,
					(byte)SpellByte.LIT,
					(byte)SpellByte.LOCK,
				},
				SkillChance1 = 0x40,
				Skills1 = new List<byte>
				{
					(byte)EnemySkills.Ink,
					(byte)EnemySkills.Ink,
					(byte)EnemySkills.Ink,
					(byte)EnemySkills.Ink,
				},
				SpellChance2 = 0x40,
				Spells2 = new List<byte>
				{
					(byte)SpellByte.LIT3,
					(byte)SpellByte.DARK,
					(byte)SpellByte.FOG,
					(byte)SpellByte.XXXX,
					(byte)SpellByte.LIT3,
					(byte)SpellByte.XFER,
					(byte)SpellByte.LOCK,
					(byte)SpellByte.BANE,
				},
				SkillChance2 = 0x40,
				Skills2 = new List<byte>
				{
					(byte)EnemySkills.Ink,
					(byte)EnemySkills.Swirl,
					(byte)EnemySkills.Ink,
					(byte)EnemySkills.Thunder,
				},
			},
			new AlternateFiends
			{
				Name = "HURRAY",
				ElementalWeakness = SpellElement.Poison,
				MonsterType = MonsterType.REGENERATIVE,
				SpellChance1 = 0x40,
				Spells1 = new List<byte>
				{
					(byte)SpellByte.FIRE,
					(byte)SpellByte.HOLD,
					(byte)SpellByte.MUTE,
					(byte)SpellByte.FIR2,
					(byte)SpellByte.MUTE,
					(byte)SpellByte.SLOW,
					(byte)SpellByte.SLEP,
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
				SpellChance2 = 0x40,
				Spells2 = new List<byte>
				{
					(byte)SpellByte.XXXX,
					(byte)SpellByte.BANE,
					(byte)SpellByte.BRAK,
					(byte)SpellByte.FIR3,
					(byte)SpellByte.XXXX,
					(byte)SpellByte.ZAP,
					(byte)SpellByte.BANE,
					(byte)SpellByte.SLO2,
				},
				SkillChance2 = 0x40,
				Skills2 = new List<byte>
				{
					(byte)EnemySkills.Poison_Stone,
					(byte)EnemySkills.Glance,
					(byte)EnemySkills.Crack,
					(byte)EnemySkills.Squint,
				},
			},
			new AlternateFiends
			{
				Name = "KOPE",
				ElementalWeakness = SpellElement.Poison,
				MonsterType = MonsterType.MAGICAL,
				SpellChance1 = 0x40,
				Spells1 = new List<byte>
				{
					(byte)SpellByte.SABR,
					(byte)SpellByte.INVS,
					(byte)SpellByte.LOCK,
					(byte)SpellByte.DARK,
					(byte)SpellByte.SABR,
					(byte)SpellByte.INVS,
					(byte)SpellByte.LOCK,
					(byte)SpellByte.QAKE,
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
					(byte)SpellByte.SABR,
					(byte)SpellByte.STOP,
					(byte)SpellByte.LOK2,
					(byte)SpellByte.FADE,
					(byte)SpellByte.SABR,
					(byte)SpellByte.MUTE,
					(byte)SpellByte.LOK2,
					(byte)SpellByte.CUR4,
				},
				SkillChance2 = 0x40,
				Skills2 = new List<byte>
				{
					(byte)EnemySkills.Blaze,
					(byte)EnemySkills.Blaze,
					(byte)EnemySkills.Inferno,
					(byte)EnemySkills.Nuclear,
				},
			},
			new AlternateFiends
			{
				Name = "LOTAN",
				ElementalWeakness = SpellElement.Lightning,
				MonsterType = MonsterType.AQUATIC,
				SpellChance1 = 0x40,
				Spells1 = new List<byte>
				{
					(byte)SpellByte.FIR2,
					(byte)SpellByte.BANE,
					(byte)SpellByte.ICE,
					(byte)SpellByte.DARK,
					(byte)SpellByte.FIRE,
					(byte)SpellByte.BRAK,
					(byte)SpellByte.ICE2,
					(byte)SpellByte.WALL,
				},
				SkillChance1 = 0x40,
				Skills1 = new List<byte>
				{
					(byte)EnemySkills.Cremate,
					(byte)EnemySkills.Frost,
					(byte)EnemySkills.Cremate,
					(byte)EnemySkills.Thunder,
				},
				SpellChance2 = 0x40,
				Spells2 = new List<byte>
				{
					(byte)SpellByte.FIR3,
					(byte)SpellByte.WALL,
					(byte)SpellByte.ICE3,
					(byte)SpellByte.BANE,
					(byte)SpellByte.FIR3,
					(byte)SpellByte.STOP,
					(byte)SpellByte.ICE3,
					(byte)SpellByte.BRAK,
				},
				SkillChance2 = 0x40,
				Skills2 = new List<byte>
				{
					(byte)EnemySkills.Inferno,
					(byte)EnemySkills.Blizzard,
					(byte)EnemySkills.Tornado,
					(byte)EnemySkills.Thunder,
				},
			},
			new AlternateFiends
			{
				Name = "MASTVAMP",
				ElementalWeakness = SpellElement.Fire,
				MonsterType = MonsterType.UNDEAD,
				SpellChance1 = 0x40,
				Spells1 = new List<byte>
				{
					(byte)SpellByte.SLEP,
					(byte)SpellByte.LIT2,
					(byte)SpellByte.STUN,
					(byte)SpellByte.LIT2,
					(byte)SpellByte.MUTE,
					(byte)SpellByte.ICE,
					(byte)SpellByte.ICE2,
					(byte)SpellByte.STUN,
				},
				SkillChance1 = 0x40,
				Skills1 = new List<byte>
				{
					(byte)EnemySkills.Dazzle,
					(byte)EnemySkills.Flash,
					(byte)EnemySkills.Stare,
					(byte)EnemySkills.Glare,
				},
				SpellChance2 = 0x40,
				Spells2 = new List<byte>
				{
					(byte)SpellByte.WALL,
					(byte)SpellByte.LIT3,
					(byte)SpellByte.XXXX,
					(byte)SpellByte.XFER,
					(byte)SpellByte.ICE3,
					(byte)SpellByte.MUTE,
					(byte)SpellByte.LIT2,
					(byte)SpellByte.BANE,
				},
				SkillChance2 = 0x40,
				Skills2 = new List<byte>
				{
					(byte)EnemySkills.Tornado,
					(byte)EnemySkills.Toxic,
					(byte)EnemySkills.Glance,
					(byte)EnemySkills.Poison_Stone,
				},
			},
			new AlternateFiends
			{
				Name = "MATOYA",
				ElementalWeakness = SpellElement.Status,
				MonsterType = MonsterType.MAGE,
				SpellChance1 = 0x40,
				Spells1 = new List<byte>
				{
					(byte)SpellByte.ICE2,
					(byte)SpellByte.FIRE,
					(byte)SpellByte.INVS,
					(byte)SpellByte.LIT2,
					(byte)SpellByte.MUTE,
					(byte)SpellByte.STUN,
					(byte)SpellByte.ICE2,
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
					(byte)SpellByte.BRAK,
					(byte)SpellByte.XXXX,
					(byte)SpellByte.XFER,
					(byte)SpellByte.ICE3,
					(byte)SpellByte.CUR4,
					(byte)SpellByte.ZAP,
					(byte)SpellByte.FIR3,
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
				Name = "MOHAWK",
				ElementalWeakness = SpellElement.Time,
				MonsterType = MonsterType.WERE,
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
					(byte)EnemySkills.Glare,
					(byte)EnemySkills.Gaze,
					(byte)EnemySkills.Dazzle,
				},
				SpellChance2 = 0x40,
				Spells2 = new List<byte>
				{
					(byte)SpellByte.HOLD,
					(byte)SpellByte.STOP,
					(byte)SpellByte.SLP2,
					(byte)SpellByte.FOG,
					(byte)SpellByte.XFER,
					(byte)SpellByte.STOP,
					(byte)SpellByte.QAKE,
					(byte)SpellByte.SLP2,
				},
				SkillChance2 = 0x40,
				Skills2 = new List<byte>
				{
					(byte)EnemySkills.Poison_Stone,
					(byte)EnemySkills.Nuclear,
					(byte)EnemySkills.Squint,
					(byte)EnemySkills.Tornado,
				},
			},
			new AlternateFiends
			{
				Name = "REVENANT",
				ElementalWeakness = SpellElement.Fire,
				MonsterType = MonsterType.UNDEAD,
				SpellChance1 = 0x40,
				Spells1 = new List<byte>
				{
					(byte)SpellByte.BANE,
					(byte)SpellByte.FOG,
					(byte)SpellByte.ICE2,
					(byte)SpellByte.INVS,
					(byte)SpellByte.RUB,
					(byte)SpellByte.SLOW,
					(byte)SpellByte.LIT2,
					(byte)SpellByte.MUTE,
				},
				SkillChance1 = 0x40,
				Skills1 = new List<byte>
				{
					(byte)EnemySkills.Frost,
					(byte)EnemySkills.Glance,
					(byte)EnemySkills.Trance,
					(byte)EnemySkills.Dazzle,
				},
				SpellChance2 = 0x40,
				Spells2 = new List<byte>
				{
					(byte)SpellByte.NUKE,
					(byte)SpellByte.INVS,
					(byte)SpellByte.FAST,
					(byte)SpellByte.ICE3,
					(byte)SpellByte.SLO2,
					(byte)SpellByte.FOG,
					(byte)SpellByte.MUTE,
					(byte)SpellByte.ZAP,
				},
				SkillChance2 = 0x40,
				Skills2 = new List<byte>
				{
					(byte)EnemySkills.Thunder,
					(byte)EnemySkills.Crack,
					(byte)EnemySkills.Poison_Stone,
					(byte)EnemySkills.Squint,
				},
			},
			new AlternateFiends
			{
				Name = "R.MEDUSA",
				ElementalWeakness = SpellElement.Ice,
				MonsterType = MonsterType.MAGICAL,
				SpellChance1 = 0x40,
				Spells1 = new List<byte>
				{
					(byte)SpellByte.FIRE,
					(byte)SpellByte.STUN,
					(byte)SpellByte.FIR2,
					(byte)SpellByte.STUN,
					(byte)SpellByte.FIRE,
					(byte)SpellByte.STUN,
					(byte)SpellByte.FIR2,
					(byte)SpellByte.STUN,
				},
				SkillChance1 = 0x40,
				Skills1 = new List<byte>
				{
					(byte)EnemySkills.Trance,
					(byte)EnemySkills.Blaze,
					(byte)EnemySkills.Trance,
					(byte)EnemySkills.Poison_Stone,
				},
				SpellChance2 = 0x40,
				Spells2 = new List<byte>
				{
					(byte)SpellByte.FIR3,
					(byte)SpellByte.FIR2,
					(byte)SpellByte.FIR3,
					(byte)SpellByte.STOP,
					(byte)SpellByte.FIR3,
					(byte)SpellByte.FIR2,
					(byte)SpellByte.FIR3,
					(byte)SpellByte.SLO2,
				},
				SkillChance2 = 0x40,
				Skills2 = new List<byte>
				{
					(byte)EnemySkills.Blaze,
					(byte)EnemySkills.Poison_Stone,
					(byte)EnemySkills.Glance,
					(byte)EnemySkills.Inferno,
				},
			},
			new AlternateFiends
			{
				Name = "SARDA",
				ElementalWeakness = SpellElement.Earth,
				MonsterType = MonsterType.MAGE,
				SpellChance1 = 0x40,
				Spells1 = new List<byte>
				{
					(byte)SpellByte.SLEP,
					(byte)SpellByte.FIR2,
					(byte)SpellByte.CUR2,
					(byte)SpellByte.SLOW,
					(byte)SpellByte.ICE2,
					(byte)SpellByte.LIT2,
					(byte)SpellByte.BRAK,
					(byte)SpellByte.STUN,
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
					(byte)SpellByte.ICE3,
					(byte)SpellByte.BANE,
					(byte)SpellByte.FADE,
					(byte)SpellByte.SLO2,
					(byte)SpellByte.SLP2,
					(byte)SpellByte.FOG,
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
				Name = "VAMAKALI",
				ElementalWeakness = SpellElement.Ice,
				MonsterType = MonsterType.MAGICAL,
				SpellChance1 = 0x40,
				Spells1 = new List<byte>
				{
					(byte)SpellByte.FIRE,
					(byte)SpellByte.STUN,
					(byte)SpellByte.FIR2,
					(byte)SpellByte.INVS,
					(byte)SpellByte.SLOW,
					(byte)SpellByte.FIRE,
					(byte)SpellByte.MUTE,
					(byte)SpellByte.RUB,
				},
				SkillChance1 = 0x40,
				Skills1 = new List<byte>
				{
					(byte)EnemySkills.Cremate,
					(byte)EnemySkills.Heat,
					(byte)EnemySkills.Trance,
					(byte)EnemySkills.Stinger,
				},
				SpellChance2 = 0x40,
				Spells2 = new List<byte>
				{
					(byte)SpellByte.XFER,
					(byte)SpellByte.FIR3,
					(byte)SpellByte.FAST,
					(byte)SpellByte.SABR,
					(byte)SpellByte.BRAK,
					(byte)SpellByte.SLO2,
					(byte)SpellByte.FIR2,
					(byte)SpellByte.FIR3,
				},
				SkillChance2 = 0x40,
				Skills2 = new List<byte>
				{
					(byte)EnemySkills.Inferno,
					(byte)EnemySkills.Poison_Stone,
					(byte)EnemySkills.Crack,
					(byte)EnemySkills.Toxic,
				},
			},
			new AlternateFiends
			{
				Name = "WRONGEYE",
				ElementalWeakness = SpellElement.None,
				MonsterType = MonsterType.REGENERATIVE,
				SpellChance1 = 0x40,
				Spells1 = new List<byte>
				{
					(byte)SpellByte.SLOW,
					(byte)SpellByte.FIR2,
					(byte)SpellByte.FOG,
					(byte)SpellByte.FIRE,
					(byte)SpellByte.RUB,
					(byte)SpellByte.SLEP,
					(byte)SpellByte.DARK,
					(byte)SpellByte.FIR2,
				},
				SkillChance1 = 0x40,
				Skills1 = new List<byte>
				{
					(byte)EnemySkills.Ink,
					(byte)EnemySkills.Trance,
					(byte)EnemySkills.Heat,
					(byte)EnemySkills.Gaze,
				},
				SpellChance2 = 0x40,
				Spells2 = new List<byte>
				{
					(byte)SpellByte.FAST,
					(byte)SpellByte.FIR3,
					(byte)SpellByte.BANE,
					(byte)SpellByte.XXXX,
					(byte)SpellByte.LIT3,
					(byte)SpellByte.SLP2,
					(byte)SpellByte.XFER,
					(byte)SpellByte.FIR3,
				},
				SkillChance2 = 0x40,
				Skills2 = new List<byte>
				{
					(byte)EnemySkills.Toxic,
					(byte)EnemySkills.Swirl,
					(byte)EnemySkills.Stare,
					(byte)EnemySkills.Poison_Stone,
				},
			},
		};

		List<AlternateFiends> FF1AltFiendsListHardcore = new() { };
	}
}
