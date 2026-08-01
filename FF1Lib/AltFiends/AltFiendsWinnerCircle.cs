namespace FF1Lib
{
	public partial class FF1Rom
	{
		List<AlternateFiends> WinnerCircleFiendsList = new()
		{
			new AlternateFiends
			{
				// Contributor: Serisan
				Name = "AGNEA",
				ElementalWeakness = SpellElement.Earth,
				MonsterType = MonsterType.MAGE,
				SpellChance1 = 0x40,
				Spells1 = new List<byte>
				{
					(byte)SpellByte.INVS,
					(byte)SpellByte.CUR2,
					(byte)SpellByte.FOG2,
					(byte)SpellByte.SABR,
					(byte)SpellByte.INVS,
					(byte)SpellByte.CUR2,
					(byte)SpellByte.SABR,
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
					(byte)SpellByte.WALL,
					(byte)SpellByte.INVS,
					(byte)SpellByte.SABR,
					(byte)SpellByte.FAST,
					(byte)SpellByte.INVS,
					(byte)SpellByte.FOG,
					(byte)SpellByte.SABR,
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
				// Contributor: Darkmoon
				Name = "B.VAMP",
				ElementalWeakness = SpellElement.Lightning,
				MonsterType = MonsterType.UNDEAD,
				SpellChance1 = 0x40,
				Spells1 = new List<byte>
				{
					(byte)SpellByte.FIRE,
					(byte)SpellByte.FOG,
					(byte)SpellByte.FIR2,
					(byte)SpellByte.INVS,
					(byte)SpellByte.FIRE,
					(byte)SpellByte.FIR2,
					(byte)SpellByte.FOG,
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
					(byte)SpellByte.FIR3,
					(byte)SpellByte.FOG,
					(byte)SpellByte.INVS,
					(byte)SpellByte.NUKE,
					(byte)SpellByte.FIR3,
					(byte)SpellByte.FOG,
					(byte)SpellByte.INVS,
					(byte)SpellByte.CUR3,
				},
				SkillChance2 = 0x40,
				Skills2 = new List<byte>
				{
					(byte)EnemySkills.Blaze,
					(byte)EnemySkills.Inferno,
					(byte)EnemySkills.Blaze,
					(byte)EnemySkills.Inferno,
				},
			},
			new AlternateFiends
			{
				// Contributor: Darkmoon
				Name = "COUNTESS",
				ElementalWeakness = SpellElement.Fire,
				MonsterType = MonsterType.UNDEAD,
				SpellChance1 = 0x40,
				Spells1 = new List<byte>
				{
					(byte)SpellByte.DARK,
					(byte)SpellByte.STUN,
					(byte)SpellByte.LIT2,
					(byte)SpellByte.AFIR,
					(byte)SpellByte.DARK,
					(byte)SpellByte.LIT,
					(byte)SpellByte.LIT2,
					(byte)SpellByte.CUR2,
				},
				SkillChance1 = 0x40,
				Skills1 = new List<byte>
				{
					(byte)EnemySkills.Glance,
					(byte)EnemySkills.Gaze,
					(byte)EnemySkills.Glance,
					(byte)EnemySkills.Stare,
				},
				SpellChance2 = 0x40,
				Spells2 = new List<byte>
				{
					(byte)SpellByte.DARK,
					(byte)SpellByte.STUN,
					(byte)SpellByte.LIT3,
					(byte)SpellByte.AFIR,
					(byte)SpellByte.DARK,
					(byte)SpellByte.LIT3,
					(byte)SpellByte.NUKE,
					(byte)SpellByte.CUR3,
				},
				SkillChance2 = 0x40,
				Skills2 = new List<byte>
				{
					(byte)EnemySkills.Thunder,
					(byte)EnemySkills.Tornado,
					(byte)EnemySkills.Dazzle,
					(byte)EnemySkills.Poison_Stone,
				},
			},
			new AlternateFiends
			{
				// Contributor: GoldenShocker
				Name = "CptUMARO",
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
					(byte)SpellByte.DARK,
					(byte)SpellByte.DARK,
					(byte)SpellByte.HOLD,
					(byte)SpellByte.HOLD,
					(byte)SpellByte.FAST,
					(byte)SpellByte.FAST,
					(byte)SpellByte.SABR,
					(byte)SpellByte.SABR,
				},
				SkillChance2 = 0x40,
				Skills2 = new List<byte>
				{
					(byte)EnemySkills.Poison_Stone,
					(byte)EnemySkills.Gaze,
					(byte)EnemySkills.Glance,
					(byte)EnemySkills.Stare,
				},
			},
			new AlternateFiends
			{
				// Contributor: SirLinkalot
				Name = "ENSINGER",
				ElementalWeakness = SpellElement.Time,
				MonsterType = MonsterType.MAGE,
				SpellChance1 = 0x40,
				Spells1 = new List<byte>
				{
					(byte)SpellByte.FIR2,
					(byte)SpellByte.INVS,
					(byte)SpellByte.LIT2,
					(byte)SpellByte.HOLD,
					(byte)SpellByte.ICE2,
					(byte)SpellByte.RUB,
					(byte)SpellByte.LIT,
					(byte)SpellByte.HOLD,
				},
				SkillChance1 = 0x40,
				Skills1 = new List<byte>
				{
					(byte)EnemySkills.Cremate,
					(byte)EnemySkills.Glance,
					(byte)EnemySkills.Trance,
					(byte)EnemySkills.Frost,
				},
				SpellChance2 = 0x40,
				Spells2 = new List<byte>
				{
					(byte)SpellByte.XFER,
					(byte)SpellByte.INVS,
					(byte)SpellByte.LIT3,
					(byte)SpellByte.XFER,
					(byte)SpellByte.NUKE,
					(byte)SpellByte.INVS,
					(byte)SpellByte.LIT3,
					(byte)SpellByte.STOP,
				},
				SkillChance2 = 0x40,
				Skills2 = new List<byte>
				{
					(byte)EnemySkills.Poison_Stone,
					(byte)EnemySkills.Toxic,
					(byte)EnemySkills.Trance,
					(byte)EnemySkills.Thunder,
				},
			},
			new AlternateFiends
			{
				// Contributor: Wewbear
				Name = "MALENIA",
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
					(byte)EnemySkills.Poison_Damage,
					(byte)EnemySkills.Stinger,
					(byte)EnemySkills.Glance,
					(byte)EnemySkills.Dazzle,
				},
				SpellChance2 = 0x40,
				Spells2 = new List<byte>
				{
					(byte)SpellByte.FAST,
					(byte)SpellByte.BANE,
					(byte)SpellByte.CUR4,
					(byte)SpellByte.NUKE,
					(byte)SpellByte.INV2,
					(byte)SpellByte.XXXX,
					(byte)SpellByte.SABR,
					(byte)SpellByte.HOLD,
				},
				SkillChance2 = 0x40,
				Skills2 = new List<byte>
				{
					(byte)EnemySkills.Poison_Damage,
					(byte)EnemySkills.Stinger,
					(byte)EnemySkills.Glance,
					(byte)EnemySkills.Dazzle,
				},
			},
			new AlternateFiends
			{
				// Contributor: DemonFrog
				Name = "MASAMUNE",
				ElementalWeakness = SpellElement.Earth,
				MonsterType = MonsterType.MAGE,
				SpellChance1 = 0x40,
				Spells1 = new List<byte>
				{
					(byte)SpellByte.LIT2,
					(byte)SpellByte.LIT2,
					(byte)SpellByte.RUSE,
					(byte)SpellByte.SLOW,
					(byte)SpellByte.LIT2,
					(byte)SpellByte.LIT2,
					(byte)SpellByte.RUSE,
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
					(byte)SpellByte.LIT3,
					(byte)SpellByte.RUSE,
					(byte)SpellByte.SLO2,
					(byte)SpellByte.FAST,
					(byte)SpellByte.LIT3,
					(byte)SpellByte.RUSE,
					(byte)SpellByte.SLO2,
					(byte)SpellByte.FAST,
				},
				SkillChance2 = 0x40,
				Skills2 = new List<byte>
				{
					(byte)EnemySkills.Tornado,
					(byte)EnemySkills.Tornado,
					(byte)EnemySkills.Tornado,
					(byte)EnemySkills.Tornado,
				},
			},
			new AlternateFiends
			{
				// Contributor: guardianmarcus
				Name = "MOOGLE",
				ElementalWeakness = SpellElement.Fire,
				MonsterType = MonsterType.GIANT,
				SpellChance1 = 0x40,
				Spells1 = new List<byte>
				{
					(byte)SpellByte.HOLD,
					(byte)SpellByte.ICE2,
					(byte)SpellByte.SLEP,
					(byte)SpellByte.STUN,
					(byte)SpellByte.LIT2,
					(byte)SpellByte.DARK,
					(byte)SpellByte.SLOW,
					(byte)SpellByte.MUTE,
				},
				SkillChance1 = 0x40,
				Skills1 = new List<byte>
				{
					(byte)EnemySkills.Dazzle,
					(byte)EnemySkills.Frost,
					(byte)EnemySkills.Stinger,
					(byte)EnemySkills.Blaze,
				},
				SpellChance2 = 0x40,
				Spells2 = new List<byte>
				{
					(byte)SpellByte.BRAK,
					(byte)SpellByte.ICE3,
					(byte)SpellByte.SLP2,
					(byte)SpellByte.LIT3,
					(byte)SpellByte.ZAP,
					(byte)SpellByte.SLO2,
					(byte)SpellByte.BANE,
					(byte)SpellByte.STOP,
				},
				SkillChance2 = 0x40,
				Skills2 = new List<byte>
				{
					(byte)EnemySkills.Blizzard,
					(byte)EnemySkills.Trance,
					(byte)EnemySkills.Poison_Damage,
					(byte)EnemySkills.Nuclear,
				},
			},
			new AlternateFiends
			{
				// Contributor: meklin89
				Name = "NIMBUS",
				ElementalWeakness = SpellElement.Fire,
				MonsterType = MonsterType.MAGICAL,
				SpellChance1 = 0x40,
				Spells1 = new List<byte>
				{
					(byte)SpellByte.SLEP,
					(byte)SpellByte.ICE2,
					(byte)SpellByte.STUN,
					(byte)SpellByte.FOG,
					(byte)SpellByte.SLOW,
					(byte)SpellByte.ICE2,
					(byte)SpellByte.WALL,
					(byte)SpellByte.BLND,
				},
				SkillChance1 = 0x40,
				Skills1 = new List<byte>
				{
					(byte)EnemySkills.Stare,
					(byte)EnemySkills.Snorting,
					(byte)EnemySkills.Frost,
					(byte)EnemySkills.Glance,
				},
				SpellChance2 = 0x40,
				Spells2 = new List<byte>
				{
					(byte)SpellByte.SLP2,
					(byte)SpellByte.ICE3,
					(byte)SpellByte.XFER,
					(byte)SpellByte.SLO2,
					(byte)SpellByte.FOG2,
					(byte)SpellByte.ICE3,
					(byte)SpellByte.XFER,
					(byte)SpellByte.ZAP,
				},
				SkillChance2 = 0x40,
				Skills2 = new List<byte>
				{
					(byte)EnemySkills.Swirl,
					(byte)EnemySkills.Flash,
					(byte)EnemySkills.Blizzard,
					(byte)EnemySkills.Poison_Stone,
				},
			},
			new AlternateFiends
			{
				// Contributor: Darkmoon
				Name = "NOS4ATU",
				ElementalWeakness = SpellElement.Fire,
				MonsterType = MonsterType.UNDEAD,
				SpellChance1 = 0x40,
				Spells1 = new List<byte>
				{
					(byte)SpellByte.DARK,
					(byte)SpellByte.HOLD,
					(byte)SpellByte.ICE2,
					(byte)SpellByte.DARK,
					(byte)SpellByte.HOLD,
					(byte)SpellByte.STUN,
					(byte)SpellByte.ICE2,
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
					(byte)SpellByte.DARK,
					(byte)SpellByte.HOLD,
					(byte)SpellByte.ICE2,
					(byte)SpellByte.DARK,
					(byte)SpellByte.HOLD,
					(byte)SpellByte.STUN,
					(byte)SpellByte.ICE2,
					(byte)SpellByte.CUR2,
				},
				SkillChance2 = 0x40,
				Skills2 = new List<byte>
				{
					(byte)EnemySkills.Blizzard,
					(byte)EnemySkills.Dazzle,
					(byte)EnemySkills.Tornado,
					(byte)EnemySkills.Frost,
				},
			},
			new AlternateFiends
			{
				// Contributor: JShydell
				Name = "PEPPER",
				ElementalWeakness = SpellElement.Fire,
				MonsterType = MonsterType.REGENERATIVE,
				SpellChance1 = 0x40,
				Spells1 = new List<byte>
				{
					(byte)SpellByte.FAST,
					(byte)SpellByte.CUR2,
					(byte)SpellByte.FOG,
					(byte)SpellByte.CUR2,
					(byte)SpellByte.FAST,
					(byte)SpellByte.CUR2,
					(byte)SpellByte.FOG,
					(byte)SpellByte.CUR2,
				},
				SkillChance1 = 0x40,
				Skills1 = new List<byte>
				{
					(byte)EnemySkills.Crack,
					(byte)EnemySkills.Frost,
					(byte)EnemySkills.Frost,
					(byte)EnemySkills.Frost,
				},
				SpellChance2 = 0x40,
				Spells2 = new List<byte>
				{
					(byte)SpellByte.FAST,
					(byte)SpellByte.CUR4,
					(byte)SpellByte.ICE3,
					(byte)SpellByte.FOG,
					(byte)SpellByte.FAST,
					(byte)SpellByte.CUR4,
					(byte)SpellByte.ICE3,
					(byte)SpellByte.FOG,
				},
				SkillChance2 = 0x40,
				Skills2 = new List<byte>
				{
					(byte)EnemySkills.Crack,
					(byte)EnemySkills.Frost,
					(byte)EnemySkills.Blizzard,
					(byte)EnemySkills.Crack,
				},
			},
			new AlternateFiends
			{
				// Contributor: Kumo
				Name = "SEPHROTH",
				ElementalWeakness = SpellElement.Status,
				MonsterType = MonsterType.MAGE,
				SpellChance1 = 0x40,
				Spells1 = new List<byte>
				{
					(byte)SpellByte.FIR2,
					(byte)SpellByte.ICE2,
					(byte)SpellByte.LIT2,
					(byte)SpellByte.CUR2,
					(byte)SpellByte.FIR2,
					(byte)SpellByte.LIT2,
					(byte)SpellByte.ICE2,
					(byte)SpellByte.CUR2,
				},
				SkillChance1 = 0x40,
				Skills1 = new List<byte>
				{
					(byte)EnemySkills.Frost,
					(byte)EnemySkills.Heat,
					(byte)EnemySkills.Flash,
					(byte)EnemySkills.Trance,
				},
				SpellChance2 = 0x40,
				Spells2 = new List<byte>
				{
					(byte)SpellByte.XFER,
					(byte)SpellByte.ICE3,
					(byte)SpellByte.FIR3,
					(byte)SpellByte.LIT3,
					(byte)SpellByte.FAST,
					(byte)SpellByte.WALL,
					(byte)SpellByte.CUR4,
					(byte)SpellByte.SABR,
				},
				SkillChance2 = 0x40,
				Skills2 = new List<byte>
				{
					(byte)EnemySkills.Nuclear,
					(byte)EnemySkills.Blaze,
					(byte)EnemySkills.Thunder,
					(byte)EnemySkills.Blizzard,
				},
			},
			new AlternateFiends
			{
				// Contributor: RazorOne
				Name = "S.LVTHAN",
				ElementalWeakness = SpellElement.Poison,
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
					(byte)EnemySkills.Glare,
					(byte)EnemySkills.Dazzle,
					(byte)EnemySkills.Glare,
				},
				SpellChance2 = 0x40,
				Spells2 = new List<byte>
				{
					(byte)SpellByte.FAST,
					(byte)SpellByte.SLO2,
					(byte)SpellByte.FOG,
					(byte)SpellByte.XXXX,
					(byte)SpellByte.FAST,
					(byte)SpellByte.SLO2,
					(byte)SpellByte.FOG,
					(byte)SpellByte.XXXX,
				},
				SkillChance2 = 0x40,
				Skills2 = new List<byte>
				{
					(byte)EnemySkills.Glare,
					(byte)EnemySkills.Trance,
					(byte)EnemySkills.Flash,
					(byte)EnemySkills.Trance,
				},
			},
			new AlternateFiends
			{
				// Contributor: Wewbear
				Name = "TASBOT",
				ElementalWeakness = SpellElement.Lightning,
				MonsterType = MonsterType.MAGE,
				SpellChance1 = 0x40,
				Spells1 = new List<byte>
				{
					(byte)SpellByte.LIT2,
					(byte)SpellByte.MUTE,
					(byte)SpellByte.FEAR,
					(byte)SpellByte.LIT3,
					(byte)SpellByte.BLND,
					(byte)SpellByte.LIT2,
					(byte)SpellByte.MUTE,
					(byte)SpellByte.SABR,
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
					(byte)SpellByte.MUTE,
					(byte)SpellByte.FADE,
					(byte)SpellByte.CUR3,
					(byte)SpellByte.BANE,
					(byte)SpellByte.XFER,
					(byte)SpellByte.STOP,
					(byte)SpellByte.LIT3,
					(byte)SpellByte.FEAR,
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
				// Contributor: Wewbear
				Name = "TCHNODRM",
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
					(byte)EnemySkills.Crack,
					(byte)EnemySkills.Blaze,
					(byte)EnemySkills.Thunder,
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
					(byte)EnemySkills.Nuclear,
					(byte)EnemySkills.Inferno,
					(byte)EnemySkills.Crack,
					(byte)EnemySkills.Thunder,
				},
			},
			new AlternateFiends
			{
				// Contributor: rubes000
				Name = "THNDBIRD",
				ElementalWeakness = SpellElement.Lightning,
				MonsterType = MonsterType.MAGE,
				SpellChance1 = 0x40,
				Spells1 = new List<byte>
				{
					(byte)SpellByte.FIRE,
					(byte)SpellByte.FIRE,
					(byte)SpellByte.STUN,
					(byte)SpellByte.FIR2,
					(byte)SpellByte.FIRE,
					(byte)SpellByte.FIRE,
					(byte)SpellByte.LOCK,
					(byte)SpellByte.FIR2,
				},
				SkillChance1 = 0x40,
				Skills1 = new List<byte>
				{
					(byte)EnemySkills.Heat,
					(byte)EnemySkills.Scorch,
					(byte)EnemySkills.Heat,
					(byte)EnemySkills.Glance,
				},
				SpellChance2 = 0x40,
				Spells2 = new List<byte>
				{
					(byte)SpellByte.FIR2,
					(byte)SpellByte.FAST,
					(byte)SpellByte.LOCK,
					(byte)SpellByte.FIR3,
					(byte)SpellByte.FIR2,
					(byte)SpellByte.INVS,
					(byte)SpellByte.WALL,
					(byte)SpellByte.FIR3,
				},
				SkillChance2 = 0x40,
				Skills2 = new List<byte>
				{
					(byte)EnemySkills.Blaze,
					(byte)EnemySkills.Inferno,
					(byte)EnemySkills.Poison_Damage,
					(byte)EnemySkills.Inferno,
				},
			},
			new AlternateFiends
			{
				// Contributor: LordFizzleBeef
				Name = "WHEELS",
				ElementalWeakness = SpellElement.Ice,
				MonsterType = MonsterType.MAGE,
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
					(byte)EnemySkills.Heat,
					(byte)EnemySkills.Ink,
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
					(byte)EnemySkills.Blaze,
					(byte)EnemySkills.Thunder,
					(byte)EnemySkills.Trance,
				},
			},
			new AlternateFiends
			{
				// Contributor: Chanigan
				Name = "ZINOGRE",
				ElementalWeakness = SpellElement.Ice,
				MonsterType = MonsterType.MAGICAL,
				SpellChance1 = 0x40,
				Spells1 = new List<byte>
				{
					(byte)SpellByte.LIT,
					(byte)SpellByte.LIT2,
					(byte)SpellByte.HOLD,
					(byte)SpellByte.STUN,
					(byte)SpellByte.LOCK,
					(byte)SpellByte.INVS,
					(byte)SpellByte.LIT,
					(byte)SpellByte.LIT2,
				},
				SkillChance1 = 0x40,
				Skills1 = new List<byte>
				{
					(byte)EnemySkills.Gaze,
					(byte)EnemySkills.Flash,
					(byte)EnemySkills.Stare,
					(byte)EnemySkills.Gaze,
				},
				SpellChance2 = 0x40,
				Spells2 = new List<byte>
				{
					(byte)SpellByte.LIT3,
					(byte)SpellByte.FIR3,
					(byte)SpellByte.STOP,
					(byte)SpellByte.RUSE,
					(byte)SpellByte.LOK2,
					(byte)SpellByte.ZAP,
					(byte)SpellByte.LIT3,
					(byte)SpellByte.FIR3,
				},
				SkillChance2 = 0x40,
				Skills2 = new List<byte>
				{
					(byte)EnemySkills.Thunder,
					(byte)EnemySkills.Trance,
					(byte)EnemySkills.Blaze,
					(byte)EnemySkills.Inferno,
				},
			},
		};
	}
}
