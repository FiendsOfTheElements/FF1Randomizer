using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RomUtilities;

namespace FF1Lib
{
	public enum FormationShuffleMode
	{
		[Description("Vanilla")]
		None = 0,
		[Description("Shuffle Rarity")]
		Intrazone,
		[Description("Shuffle Across Zones")]
		InterZone,
		[Description("Totally Random")]
		Randomize
	}
	public partial class FF1Rom : NesRom
	{
		public const int EnemyOffset = 0x30520;
		public const int EnemySize = 20;
		public const int EnemyCount = 128;

		public const int ScriptOffset = 0x31020;
		public const int ScriptSize = 16;
		public const int ScriptCount = 44;

		public const int ZoneFormationsOffset = 0x2C000;
		public const int ZoneFormationsSize = 8;
		public const int ZoneCount = 128;

		public const int FormationDataOffset = 0x2C400;
		public const int FormationDataSize = 16;
		public const int FormationDataCount = 128;

		public abstract class Enemy
		{
			public const int Imp = 0;
			public const int Pirate = 15;
			public const int Crawl = 24;
			public const int Phantom = 51;
			public const int Mancat = 55;
			public const int Vampire = 60;
			public const int Coctrice = 81;
			public const int Sorceror = 104;
			public const int Garland = 105;
			public const int Astos = 113;
			public const int Nitemare = 117;
			public const int WarMech = 118;
			public const int Lich = 119;
			public const int Lich2 = 120;
			public const int Kary = 121;
			public const int Kary2 = 122;
			public const int Kraken = 123;
			public const int Kraken2 = 124;
			public const int Tiamat = 125;
			public const int Tiamat2 = 126;
			public const int Chaos = 127;
		}
		public byte[] StartingZones = { 0x1B, 0x1C, 0x24, 0x2C };

		public void ShuffleEnemyFormations(MT19337 rng, FormationShuffleMode shuffleMode)
		{

			if (shuffleMode == FormationShuffleMode.Intrazone)
			{
				// intra-zone shuffle, does not change which formations are in zomes.
				var oldFormations = Get(ZoneFormationsOffset, ZoneFormationsSize * ZoneCount).Chunk(ZoneFormationsSize);
				var newFormations = Get(ZoneFormationsOffset, ZoneFormationsSize * ZoneCount).Chunk(ZoneFormationsSize);

				for (int i = 0; i < ZoneCount; i++)
				{

					var lowFormations = oldFormations[i].Chunk(4)[0].Chunk(1); // shuffle the first 4 formations first
					lowFormations.Shuffle(rng);
					newFormations[i][0] = lowFormations[0][0];
					newFormations[i][1] = lowFormations[1][0];
					newFormations[i][2] = lowFormations[2][0];
					newFormations[i][3] = lowFormations[3][0];

					var shuffleFormations = newFormations[i].SubBlob(2, 6).Chunk(1); // get formations 2-8
					shuffleFormations.Shuffle(rng);
					for (int j = 2; j < 8; j++)
					{
						newFormations[i][j] = shuffleFormations[j - 2][0];
					}

				}

				Put(ZoneFormationsOffset, newFormations.SelectMany(formation => formation.ToBytes()).ToArray());
			}
			if (shuffleMode == FormationShuffleMode.InterZone)
			{
				// Inter-zone shuffle
				// Get all encounters from zones not surrounding starting area
				List<Blob> newFormations = new List<Blob>();
				SortedSet<byte> exclusionZones = new SortedSet<byte>();
				exclusionZones.UnionWith(StartingZones);

				for (byte i = 0; i < ZoneCount; i++)
				{
					if (StartingZones.Contains(i))
					{
						continue;
					}
					var zone = Get(ZoneFormationsOffset + (i * ZoneFormationsSize), ZoneFormationsSize);
					if (zone.ToLongs()[0] == 0)
					{
						//some unused overworld zones are zero filled so we catch them here to not pollute the formations list
						exclusionZones.Add(i);
					}
					else
					{
						newFormations.AddRange(zone.Chunk(1));
					}
				}

				newFormations.Shuffle(rng);
				// after shuffling, put original starting zones in so only one write is required
				foreach (byte i in exclusionZones)
				{
					var startZone = Get(ZoneFormationsOffset + (i * ZoneFormationsSize), ZoneFormationsSize).Chunk(1);
					newFormations.InsertRange(i * ZoneFormationsSize, startZone);
				}
				Put(ZoneFormationsOffset, newFormations.SelectMany(formation => formation.ToBytes()).ToArray());
			}

			if (shuffleMode == FormationShuffleMode.Randomize)
			{
				// no-pants mode
				var oldFormations = Get(ZoneFormationsOffset, ZoneFormationsSize * ZoneCount).Chunk(ZoneFormationsSize);
				var newFormations = Get(ZoneFormationsOffset, ZoneFormationsSize * ZoneCount).Chunk(ZoneFormationsSize);
				var allowableEncounters = Enumerable.Range(0, 256).ToList();
				var unallowableEncounters = new List<int>() { 0x73, 0x74, 0x75, 0x76, 0x77, 0x78, 0x79, 0x7A, 0x7B, 0x7C, 0x7D, 0x7E, 0x7F, 0xF3, 0xF4, 0xF5, 0xF6, 0xF7, 0xF8, 0xF9, 0xFA, 0xFB, 0xFC, 0xFD };
				allowableEncounters.RemoveAll(x => unallowableEncounters.Contains(x));
				for (byte i = 0; i < ZoneCount; i++)
				{
					if (StartingZones.Contains(i))
					{
						continue;
					}
					for (int j = 0; j < ZoneFormationsSize; j++)
					{
						newFormations[i][j] = (byte)allowableEncounters.PickRandom(rng);
					}
				}
				Put(ZoneFormationsOffset, newFormations.SelectMany(formation => formation.ToBytes()).ToArray());
			}
		}

		private void UnleashWarMECH()
		{
			const int WarMECHsToAdd = 1;
			const int WarMECHIndex = 6;
			const byte WarMECHEncounter = 0x56;
			var oldFormations = Get(ZoneFormationsOffset, ZoneFormationsSize * ZoneCount).Chunk(ZoneFormationsSize);
			var newFormations = new List<byte>();

			foreach(var zone in oldFormations)
			{
				var bytes = zone.ToBytes().ToList();
				if (!bytes.Any(x => x > 0))
				{
					newFormations.AddRange(bytes);
					continue;
				}
				bytes = bytes.Skip(WarMECHsToAdd).ToList();
				newFormations.AddRange(bytes.Take(WarMECHIndex));
				newFormations.AddRange(Enumerable.Repeat(WarMECHEncounter, WarMECHsToAdd));
				newFormations.AddRange(bytes.Skip(WarMECHIndex));
			}

			Put(ZoneFormationsOffset, newFormations.ToArray());
		}

		public void ShuffleEnemyScripts(MT19337 rng, bool AllowUnsafePirates, bool doNormals, bool doBosses, bool excludeImps, bool scaryImps)
		{
			var oldEnemies = Get(EnemyOffset, EnemySize * EnemyCount).Chunk(EnemySize);
			var newEnemies = Get(EnemyOffset, EnemySize * EnemyCount).Chunk(EnemySize);

			if(doNormals)
			{
				var normalOldEnemies = oldEnemies.Take(EnemyCount - 10).ToList(); // all but WarMECH, fiends, fiends revisited, and CHAOS
				if (!AllowUnsafePirates) normalOldEnemies.RemoveAt(Enemy.Pirate);
				if (excludeImps) normalOldEnemies.RemoveAt(Enemy.Imp);
				normalOldEnemies.Shuffle(rng);
				if (excludeImps) normalOldEnemies.Insert(Enemy.Imp, oldEnemies[Enemy.Imp]);
				if (!AllowUnsafePirates) normalOldEnemies.Insert(Enemy.Pirate, oldEnemies[Enemy.Pirate]);

				for (int i = 0; i < EnemyCount - 10; i++)
				{
					newEnemies[i][EnemyStat.Scripts] = normalOldEnemies[i][EnemyStat.Scripts];
				}
			}

			if(doBosses)
			{
				var oldBosses = new List<Blob>
				{
					oldEnemies[Enemy.Lich],
					oldEnemies[Enemy.Kary],
					oldEnemies[Enemy.Kraken],
					oldEnemies[Enemy.Tiamat]
				};
				oldBosses.Shuffle(rng);

				newEnemies[Enemy.Lich][EnemyStat.Scripts] = oldBosses[0][EnemyStat.Scripts];
				newEnemies[Enemy.Kary][EnemyStat.Scripts] = oldBosses[1][EnemyStat.Scripts];
				newEnemies[Enemy.Kraken][EnemyStat.Scripts] = oldBosses[2][EnemyStat.Scripts];
				newEnemies[Enemy.Tiamat][EnemyStat.Scripts] = oldBosses[3][EnemyStat.Scripts];

				var oldBigBosses = new List<Blob>
				{
					oldEnemies[Enemy.WarMech],
					oldEnemies[Enemy.Lich2],
					oldEnemies[Enemy.Kary2],
					oldEnemies[Enemy.Kraken2],
					oldEnemies[Enemy.Tiamat2],
					oldEnemies[Enemy.Chaos]
				};
				if (scaryImps) oldBigBosses.Add(oldEnemies[Enemy.Imp]);
				oldBigBosses.Shuffle(rng);

				newEnemies[Enemy.WarMech][EnemyStat.Scripts] = oldBigBosses[0][EnemyStat.Scripts];
				newEnemies[Enemy.Lich2][EnemyStat.Scripts] = oldBigBosses[1][EnemyStat.Scripts];
				newEnemies[Enemy.Kary2][EnemyStat.Scripts] = oldBigBosses[2][EnemyStat.Scripts];
				newEnemies[Enemy.Kraken2][EnemyStat.Scripts] = oldBigBosses[3][EnemyStat.Scripts];
				newEnemies[Enemy.Tiamat2][EnemyStat.Scripts] = oldBigBosses[4][EnemyStat.Scripts];
				newEnemies[Enemy.Chaos][EnemyStat.Scripts] = oldBigBosses[5][EnemyStat.Scripts];
				if (scaryImps) newEnemies[Enemy.Imp][EnemyStat.Scripts] = oldBigBosses[6][EnemyStat.Scripts];
			}

			Put(EnemyOffset, newEnemies.SelectMany(enemy => enemy.ToBytes()).ToArray());
		}

		public void ShuffleEnemySkillsSpells(MT19337 rng, bool doNormals, bool doBosses)
		{
			var scriptBytes = Get(ScriptOffset, ScriptSize * ScriptCount).Chunk(ScriptSize);

			// Remove two instances each of CRACK and TOXIC since they're likely to get spread out to several enemies.
			const int SandW = 7; // CRACK
			const int IronGol = 27; // TOXIC
			scriptBytes[SandW][13] = 0xFF;
			scriptBytes[SandW][14] = 0xFF;
			scriptBytes[IronGol][13] = 0xFF;
			scriptBytes[IronGol][14] = 0xFF;

			var normalIndices = Enumerable.Range(0, 32).Concat(new[] { 33, 43 }).ToList();
			var bossIndices = new List<int> { 34, 36, 38, 40 };
			var bigBossIndices = new List<int> { 32, 35, 37, 39, 41, 42 };

			if(doNormals)
				ShuffleIndexedSkillsSpells(scriptBytes, normalIndices, rng);

			if(doBosses)
			{
				ShuffleIndexedSkillsSpells(scriptBytes, bossIndices, rng);
				ShuffleIndexedSkillsSpells(scriptBytes, bigBossIndices, rng);
			}

			Put(ScriptOffset, scriptBytes.SelectMany(script => script.ToBytes()).ToArray());
		}

		private void ShuffleIndexedSkillsSpells(List<Blob> scriptBytes, List<int> indices, MT19337 rng)
		{
			var scripts = indices.Select(i => scriptBytes[i]).ToList();

			var spellBytes = scripts.SelectMany(script => script.SubBlob(2, 8).ToBytes()).Where(b => b != 0xFF).ToList();
			var skillBytes = scripts.SelectMany(script => script.SubBlob(11, 4).ToBytes()).Where(b => b != 0xFF).ToList();
			spellBytes.Shuffle(rng);
			skillBytes.Shuffle(rng);

			var spellBuckets = Bucketize(spellBytes, scripts.Count(script => script[0] != 0), 8, rng);
			var skillBuckets = Bucketize(skillBytes, scripts.Count(script => script[1] != 0), 4, rng);

			var spellChances = scripts.Select(script => script[0]).ToList();
			var skillChances = scripts.Select(script => script[1]).ToList();
			spellChances.Shuffle(rng);
			skillChances.Shuffle(rng);

			int spellBucketIndex = 0, skillBucketIndex = 0;
			for (int i = 0; i < scripts.Count; i++)
			{
				var script = scriptBytes[indices[i]];
				script[0] = spellChances[i];
				script[1] = skillChances[i];

				for (int j = 2; j < 16; j++)
				{
					script[j] = 0xFF;
				}
				if (spellChances[i] != 0)
				{
					for (int j = 0; j < spellBuckets[spellBucketIndex].Count; j++)
					{
						script[j + 2] = spellBuckets[spellBucketIndex][j];
					}
					spellBucketIndex++;
				}
				if (skillChances[i] != 0)
				{
					for (int j = 0; j < skillBuckets[skillBucketIndex].Count; j++)
					{
						script[j + 11] = skillBuckets[skillBucketIndex][j];
					}
					skillBucketIndex++;
				}
			}
		}

		private List<List<byte>> Bucketize(List<byte> bytes, int bucketCount, int bucketSize, MT19337 rng)
		{
			var buckets = new List<List<byte>>();
			for (int i = 0; i < bucketCount; i++)
			{
				buckets.Add(new List<byte>());
			}

			int index = 0;
			while (index < bucketCount)
			{
				buckets[index].Add(bytes[index++]);
			}
			while (index < bytes.Count)
			{
				var bucket = rng.Between(0, buckets.Count - 1);
				if (buckets[bucket].Count < bucketSize)
				{
					buckets[bucket].Add(bytes[index++]);
				}
			}

			return buckets;
		}

		public void ShuffleEnemyStatusAttacks(MT19337 rng, bool AllowUnsafePirates)
		{
			var oldEnemies = Get(EnemyOffset, EnemySize * EnemyCount).Chunk(EnemySize);
			var newEnemies = Get(EnemyOffset, EnemySize * EnemyCount).Chunk(EnemySize);

			oldEnemies.Shuffle(rng);

			for (int i = 0; i < EnemyCount; i++)
			{
				if (!AllowUnsafePirates)
				{
					if (i == 15) //pirates
					{
						continue;
					}
				}

				newEnemies[i][14] = oldEnemies[i][14];
				newEnemies[i][15] = oldEnemies[i][15];
			}

			Put(EnemyOffset, newEnemies.SelectMany(enemy => enemy.ToBytes()).ToArray());
		}

		public void RandomEnemyStatusAttacks(MT19337 rng, bool AllowUnsafePirates, bool DisableStunTouch)
		{
			var enemies = Get(EnemyOffset, EnemySize * EnemyCount).Chunk(EnemySize);

			List<(byte touch, byte element)> statusElements = new List<(byte touch, byte element)>()
			{
				(0x04, 0x02), //Poison Touch = Poison
				(0x08, 0x01), //Dark Touch = Status
				(0x10, 0x01), //Stun Touch = Status
				(0x20, 0x01), //Sleep Touch = Status
				(0x40, 0x01), //Mute Touch = Status
			};

			if (DisableStunTouch) statusElements.Remove((0x10, 0x01));

			(byte touch, byte element) deathElement = (0x01, 0x08); //Death Touch = Death Element
			(byte touch, byte element) stoneElement = (0x02, 0x02); //Stone Touch = Poison

			for (int i = 0; i < EnemyCount; i++)
			{
				if (!AllowUnsafePirates)
				{
					if (i == 15) //pirates
					{
						continue;
					}
				}

				int roll = rng.Between(0, 128);
				if (roll < 1) //1 vanilla death toucher
				{
					//Death Touch
					var (touch, element) = deathElement;

				}
				else if (roll < 2) //1 vanilla stone toucher
				{
					//Stone Touch
					var (touch, element) = stoneElement;
				}
				else if (roll < 37) //35 enemies with other assorted status touches
				{
					var (touch, element) = statusElements.PickRandom(rng);
					enemies[i][15] = touch;
					enemies[i][14] = element;
				}
				else
				{
					//Otherwise, the enemy has no touch or associated element.
					enemies[i][14] = 0x00;
					enemies[i][15] = 0x00;
				}
			}

			Put(EnemyOffset, enemies.SelectMany(enemy => enemy.ToBytes()).ToArray());
		}

		public enum EnemySkills : byte
		{
			Frost = 0x00,
			Heat = 0x01,
			Glance = 0x02,
			Gaze = 0x03,
			Flash = 0x04,
			Scorch = 0x05,
			Crack = 0x06,
			Squint = 0x07,
			Stare = 0x08,
			Glare = 0x09,
			Blizzard = 0x0A,
			Blaze = 0x0B,
			Inferno = 0x0C,
			Cremate = 0x0D,
			Poison_Stone = 0x0E,
			Trance = 0x0F,
			Poison_Damage = 0x10,
			Thunder = 0x11,
			Toxic = 0x12,
			Snorting = 0x13,
			Nuclear = 0x14,
			Ink = 0x15,
			Stinger = 0x16,
			Dazzle = 0x17,
			Swirl = 0x18,
			Tornado = 0x19,
			None = 0xFF
		}

		public enum SpellByte : byte
		{
			CURE = 0x00,
			HARM = 0x01,
			FOG = 0x02,
			RUSE = 0x03,
			FIRE = 0x04,
			SLEP = 0x05,
			LOCK = 0x06,
			LIT = 0x07,
			LAMP = 0x08,
			MUTE = 0x09,
			ALIT = 0x0A,
			INVS = 0x0B,
			ICE = 0x0C,
			DARK = 0x0D,
			TMPR = 0x0E,
			SLOW = 0x0F,
			CUR2 = 0x10,
			HRM2 = 0x11,
			AFIR = 0x12,
			HEAL = 0x13,
			FIR2 = 0x14,
			HOLD = 0x15,
			LIT2 = 0x16,
			LOK2 = 0x17,
			PURE = 0x18,
			FEAR = 0x19,
			AICE = 0x1A,
			AMUT = 0x1B,
			SLP2 = 0x1C,
			FAST = 0x1D,
			CONF = 0x1E,
			ICE2 = 0x1F,
			CUR3 = 0x20,
			LIFE = 0x21,
			HRM3 = 0x22,
			HEL2 = 0x23,
			FIR3 = 0x24,
			BANE = 0x25,
			WARP = 0x26,
			SLO2 = 0x27,
			SOFT = 0x28,
			EXIT = 0x29,
			FOG2 = 0x2A,
			INV2 = 0x2B,
			LIT3 = 0x2C,
			RUB = 0x2D,
			QAKE = 0x2E,
			STUN = 0x2F,
			CUR4 = 0x30,
			HRM4 = 0x31,
			ARUB = 0x32,
			HEL3 = 0x33,
			ICE3 = 0x34,
			BRAK = 0x35,
			SABR = 0x36,
			BLND = 0x37,
			LIF2 = 0x38,
			FADE = 0x39,
			WALL = 0x3A,
			XFER = 0x3B,
			NUKE = 0x3C,
			STOP = 0x3D,
			ZAP = 0x3E,
			XXXX = 0x3F,
			NONE = 0xFF
		}

		public struct AlternateFiends
		{
			public string Name;
			public List<byte> Spells1;
			public List<byte> Skills1;
			public byte SpellChance1;
			public byte SkillChance1;
			public List<byte> Spells2;
			public List<byte> Skills2;
			public byte SpellChance2;
			public byte SkillChance2;
			public MonsterType MonsterType;
			public Element ElementalWeakness;
			public FormationSpriteSheet SpriteSheet;
			public FormationPattern FormationPattern;
			public int Palette1;
			public int Palette2;
			public FormationGFX GFXOffset;
		}
		public void AlternativeFiends(MT19337 rng)
		{
			const int FiendsIndex = 0x77;
			const int FiendsScriptIndex = 0x22;
			var fiendsFormationOrder = new List<int> { 0x7A, 0x73, 0x79, 0x74, 0x78, 0x75, 0x77, 0x76 };

			var alternateFiendsList = new List<AlternateFiends> {
				new AlternateFiends {
					Name = "ASTAROTH",
					SpriteSheet = FormationSpriteSheet.BadmanAstosMadponyWarmech,
					FormationPattern = FormationPattern.Mixed,
					GFXOffset = FormationGFX.Sprite2,
					Palette1 = 0x39,
					Palette2 = 0x39,
					ElementalWeakness = Element.FIRE,
					MonsterType = MonsterType.MAGE,
					SpellChance1 = 0x40,
					Spells1 = new List<byte> { (byte)SpellByte.FIRE, (byte)SpellByte.FIRE, (byte)SpellByte.BRAK, (byte)SpellByte.FIRE, (byte)SpellByte.NONE, (byte)SpellByte.NONE, (byte)SpellByte.NONE, (byte)SpellByte.NONE },
					SkillChance1 = 0x40,
					Skills1 = new List<byte> { (byte)EnemySkills.Heat, (byte)EnemySkills.Heat, (byte)EnemySkills.Heat, (byte)EnemySkills.Poison_Stone },
					SpellChance2 = 0x40,
					Spells2 = new List<byte> { (byte)SpellByte.FIR2, (byte)SpellByte.FIR3, (byte)SpellByte.FIR2, (byte)SpellByte.BRAK, (byte)SpellByte.FIR2, (byte)SpellByte.FIR3, (byte)SpellByte.FIR2, (byte)SpellByte.BANE },
					SkillChance2 = 0x40,
					Skills2 = new List<byte> { (byte)EnemySkills.Scorch, (byte)EnemySkills.Poison_Damage, (byte)EnemySkills.Blaze, (byte)EnemySkills.Poison_Stone },
				},
				new AlternateFiends {
					Name = "ASURA",
					SpriteSheet = FormationSpriteSheet.BoneCreepHyenaOgre,
					FormationPattern = FormationPattern.Large4,
					GFXOffset = FormationGFX.Sprite4,
					Palette1 = 0x3A,
					Palette2 = 0x3A,
					ElementalWeakness = Element.NONE,
					MonsterType = MonsterType.REGENERATIVE,
					SpellChance1 = 0x40,
					Spells1 = new List<byte> { (byte)SpellByte.CUR2, (byte)SpellByte.CUR3, (byte)SpellByte.CUR3, (byte)SpellByte.FAST, (byte)SpellByte.NONE, (byte)SpellByte.NONE, (byte)SpellByte.NONE, (byte)SpellByte.NONE },
					SkillChance1 = 0x00,
					Skills1 = new List<byte> { (byte)EnemySkills.None, (byte)EnemySkills.None, (byte)EnemySkills.None, (byte)EnemySkills.None },
					SpellChance2 = 0x40,
					Spells2 = new List<byte> { (byte)SpellByte.CUR2, (byte)SpellByte.CUR3, (byte)SpellByte.CUR2, (byte)SpellByte.FAST, (byte)SpellByte.CUR3, (byte)SpellByte.CUR2, (byte)SpellByte.CUR3, (byte)SpellByte.CUR4 },
					SkillChance2 = 0x00,
					Skills2 = new List<byte> { (byte)EnemySkills.None, (byte)EnemySkills.None, (byte)EnemySkills.None, (byte)EnemySkills.None },
				},
				new AlternateFiends {
					Name = "BARBRICA",
					SpriteSheet = FormationSpriteSheet.SentryWaterNagaChimera,
					FormationPattern = FormationPattern.Mixed,
					GFXOffset = FormationGFX.Sprite2,
					Palette1 = 0x36,
					Palette2 = 0x36,
					ElementalWeakness = Element.EARTH,
					MonsterType = MonsterType.MAGICAL,
					SpellChance1 = 0x40,
					Spells1 = new List<byte> { (byte)SpellByte.LIT, (byte)SpellByte.DARK, (byte)SpellByte.SLOW, (byte)SpellByte.LIT2, (byte)SpellByte.NONE, (byte)SpellByte.NONE, (byte)SpellByte.NONE, (byte)SpellByte.NONE },
					SkillChance1 = 0x40,
					Skills1 = new List<byte> { (byte)EnemySkills.Heat, (byte)EnemySkills.Flash, (byte)EnemySkills.Gaze, (byte)EnemySkills.Heat },
					SpellChance2 = 0x40,
					Spells2 = new List<byte> { (byte)SpellByte.LIT3, (byte)SpellByte.LIT2, (byte)SpellByte.SLOW, (byte)SpellByte.LIT3, (byte)SpellByte.LIT2, (byte)SpellByte.SLO2, (byte)SpellByte.DARK, (byte)SpellByte.DARK },
					SkillChance2 = 0x40,
					Skills2 = new List<byte> { (byte)EnemySkills.Flash, (byte)EnemySkills.Thunder, (byte)EnemySkills.Glare, (byte)EnemySkills.Thunder },
				},
				new AlternateFiends {
					Name = "BELZEBUB",
					SpriteSheet = FormationSpriteSheet.SlimeSpiderManticorAnkylo,
					FormationPattern = FormationPattern.Mixed,
					GFXOffset = FormationGFX.Sprite2,
					Palette1 = 0x30,
					Palette2 = 0x30,
					ElementalWeakness = Element.LIGHTNING,
					MonsterType = MonsterType.UNDEAD,
					SpellChance1 = 0x40,
					Spells1 = new List<byte> { (byte)SpellByte.FIRE, (byte)SpellByte.FAST, (byte)SpellByte.FIRE, (byte)SpellByte.FOG, (byte)SpellByte.FIRE, (byte)SpellByte.SLOW, (byte)SpellByte.FIRE, (byte)SpellByte.RUB },
					SkillChance1 = 0x40,
					Skills1 = new List<byte> { (byte)EnemySkills.Flash, (byte)EnemySkills.Stinger, (byte)EnemySkills.Flash, (byte)EnemySkills.Dazzle },
					SpellChance2 = 0x40,
					Spells2 = new List<byte> { (byte)SpellByte.FIR2, (byte)SpellByte.FAST, (byte)SpellByte.FIR3, (byte)SpellByte.FOG, (byte)SpellByte.FIR2, (byte)SpellByte.SLO2, (byte)SpellByte.FIR3, (byte)SpellByte.BANE },
					SkillChance2 = 0x40,
					Skills2 = new List<byte> { (byte)EnemySkills.Nuclear, (byte)EnemySkills.Flash, (byte)EnemySkills.Dazzle, (byte)EnemySkills.Inferno },
				},
				new AlternateFiends {
					Name = "BEHEMOTH",
					SpriteSheet = FormationSpriteSheet.BoneCreepHyenaOgre,
					FormationPattern = FormationPattern.Large4,
					GFXOffset = FormationGFX.Sprite3,
					Palette1 = 0x1D, // Yellow/Orange
					Palette2 = 0x1D,
					ElementalWeakness = Element.ICE,
					MonsterType = MonsterType.UNDEAD,
					SpellChance1 = 0x40,
					Spells1 = new List<byte> { (byte)SpellByte.FIR2, (byte)SpellByte.STUN, (byte)SpellByte.FIR2, (byte)SpellByte.FAST, (byte)SpellByte.NONE, (byte)SpellByte.NONE, (byte)SpellByte.NONE, (byte)SpellByte.NONE },
					SkillChance1 = 0x40,
					Skills1 = new List<byte> { (byte)EnemySkills.Snorting, (byte)EnemySkills.Snorting, (byte)EnemySkills.Snorting, (byte)EnemySkills.Snorting },
					SpellChance2 = 0x40,
					Spells2 = new List<byte> { (byte)SpellByte.FIR3, (byte)SpellByte.STOP, (byte)SpellByte.FIR3, (byte)SpellByte.FAST, (byte)SpellByte.FIR3, (byte)SpellByte.SLO2, (byte)SpellByte.FIR3, (byte)SpellByte.NUKE },
					SkillChance2 = 0x40,
					Skills2 = new List<byte> { (byte)EnemySkills.Snorting, (byte)EnemySkills.Snorting, (byte)EnemySkills.Snorting, (byte)EnemySkills.Blaze },
				},
				new AlternateFiends {
					Name = "B.KNIGHT",
					SpriteSheet = FormationSpriteSheet.BadmanAstosMadponyWarmech,
					FormationPattern = FormationPattern.Mixed,
					GFXOffset = FormationGFX.Sprite1,
					Palette1 = 0x1A, // Grey/Purple
					Palette2 = 0x1A,
					ElementalWeakness = Element.POISON,
					MonsterType = MonsterType.MAGE,
					SpellChance1 = 0x40,
					Spells1 = new List<byte> { (byte)SpellByte.FAST, (byte)SpellByte.SLOW, (byte)SpellByte.STUN, (byte)SpellByte.STOP, (byte)SpellByte.NONE, (byte)SpellByte.NONE, (byte)SpellByte.NONE, (byte)SpellByte.NONE },
					SkillChance1 = 0x40,
					Skills1 = new List<byte> { (byte)EnemySkills.Flash, (byte)EnemySkills.Flash, (byte)EnemySkills.Flash, (byte)EnemySkills.Dazzle },
					SpellChance2 = 0x40,
					Spells2 = new List<byte> { (byte)SpellByte.MUTE, (byte)SpellByte.SLO2, (byte)SpellByte.FAST, (byte)SpellByte.SABR, (byte)SpellByte.STUN, (byte)SpellByte.STOP, (byte)SpellByte.SABR, (byte)SpellByte.BRAK },
					SkillChance2 = 0x40,
					Skills2 = new List<byte> { (byte)EnemySkills.Trance, (byte)EnemySkills.Flash, (byte)EnemySkills.Ink, (byte)EnemySkills.Glare },
				},
				new AlternateFiends {
					Name = "CAGNAZZO",
					SpriteSheet = FormationSpriteSheet.SlimeSpiderManticorAnkylo,
					FormationPattern = FormationPattern.Large4,
					GFXOffset = FormationGFX.Sprite4,
					Palette1 = 0x14, // Blue/Purple
					Palette2 = 0x14,
					ElementalWeakness = Element.LIGHTNING,
					MonsterType = MonsterType.AQUATIC,
					SpellChance1 = 0x40,
					Spells1 = new List<byte> { (byte)SpellByte.ICE, (byte)SpellByte.STUN, (byte)SpellByte.ICE, (byte)SpellByte.ICE2, (byte)SpellByte.NONE, (byte)SpellByte.NONE, (byte)SpellByte.NONE, (byte)SpellByte.NONE },
					SkillChance1 = 0x40,
					Skills1 = new List<byte> { (byte)EnemySkills.Frost, (byte)EnemySkills.Flash, (byte)EnemySkills.Frost, (byte)EnemySkills.Flash },
					SpellChance2 = 0x40,
					Spells2 = new List<byte> { (byte)SpellByte.ICE2, (byte)SpellByte.ICE2, (byte)SpellByte.ICE3, (byte)SpellByte.CUR3, (byte)SpellByte.ICE2, (byte)SpellByte.ICE3, (byte)SpellByte.ICE3, (byte)SpellByte.CUR4 },
					SkillChance2 = 0x40,
					Skills2 = new List<byte> { (byte)EnemySkills.Frost, (byte)EnemySkills.Dazzle, (byte)EnemySkills.Frost, (byte)EnemySkills.Blizzard },
				},
				new AlternateFiends {
					Name = "CARBUNCL",
					SpriteSheet = FormationSpriteSheet.SentryWaterNagaChimera,
					FormationPattern = FormationPattern.Large4,
					GFXOffset = FormationGFX.Sprite4,
					Palette1 = 0x2D, // Blue/Grey
					Palette2 = 0x2D,
					ElementalWeakness = Element.NONE,
					MonsterType = MonsterType.MAGICAL,
					SpellChance1 = 0x40,
					Spells1 = new List<byte> { (byte)SpellByte.CURE, (byte)SpellByte.FAST, (byte)SpellByte.CURE, (byte)SpellByte.SLOW, (byte)SpellByte.NONE, (byte)SpellByte.NONE, (byte)SpellByte.NONE, (byte)SpellByte.NONE },
					SkillChance1 = 0x40,
					Skills1 = new List<byte> { (byte)EnemySkills.Stinger, (byte)EnemySkills.Dazzle, (byte)EnemySkills.Gaze, (byte)EnemySkills.Dazzle },
					SpellChance2 = 0x40,
					Spells2 = new List<byte> { (byte)SpellByte.FAST, (byte)SpellByte.CUR2, (byte)SpellByte.SLO2, (byte)SpellByte.WALL, (byte)SpellByte.CUR2, (byte)SpellByte.XFER, (byte)SpellByte.SABR, (byte)SpellByte.CUR3 },
					SkillChance2 = 0x40,
					Skills2 = new List<byte> { (byte)EnemySkills.Stinger, (byte)EnemySkills.Gaze, (byte)EnemySkills.Glance, (byte)EnemySkills.Dazzle },
				},
				new AlternateFiends {
					Name = "DJINN",
					SpriteSheet = FormationSpriteSheet.SentryWaterNagaChimera,
					FormationPattern = FormationPattern.Large4,
					GFXOffset = FormationGFX.Sprite3,
					Palette1 = 0x23, // Green/White
					Palette2 = 0x23,
					ElementalWeakness = Element.ICE,
					MonsterType = MonsterType.MAGICAL,
					SpellChance1 = 0x40,
					Spells1 = new List<byte> { (byte)SpellByte.FIRE, (byte)SpellByte.FIRE, (byte)SpellByte.FIRE, (byte)SpellByte.FAST, (byte)SpellByte.NONE, (byte)SpellByte.NONE, (byte)SpellByte.NONE, (byte)SpellByte.NONE },
					SkillChance1 = 0x40,
					Skills1 = new List<byte> { (byte)EnemySkills.Scorch, (byte)EnemySkills.Heat, (byte)EnemySkills.Scorch, (byte)EnemySkills.Gaze },
					SpellChance2 = 0x40,
					Spells2 = new List<byte> { (byte)SpellByte.FIR2, (byte)SpellByte.FIR2, (byte)SpellByte.FAST, (byte)SpellByte.FIR3, (byte)SpellByte.FIR2, (byte)SpellByte.SLO2, (byte)SpellByte.FIR2, (byte)SpellByte.NUKE },
					SkillChance2 = 0x40,
					Skills2 = new List<byte> { (byte)EnemySkills.Blaze, (byte)EnemySkills.Scorch, (byte)EnemySkills.Scorch, (byte)EnemySkills.Inferno },
				},
				new AlternateFiends {
					Name = "ECHIDNA",
					SpriteSheet = FormationSpriteSheet.KaryLich,
					FormationPattern = FormationPattern.Fiends,
					GFXOffset = FormationGFX.Sprite1,
					Palette1 = 0x25, // Red/White
					Palette2 = 0x2F,
					ElementalWeakness = Element.FIRE,
					MonsterType = MonsterType.MAGE,
					SpellChance1 = 0x40,
					Spells1 = new List<byte> { (byte)SpellByte.STUN, (byte)SpellByte.STOP, (byte)SpellByte.STUN, (byte)SpellByte.SLOW, (byte)SpellByte.NONE, (byte)SpellByte.NONE, (byte)SpellByte.NONE, (byte)SpellByte.NONE },
					SkillChance1 = 0x40,
					Skills1 = new List<byte> { (byte)EnemySkills.Dazzle, (byte)EnemySkills.Ink, (byte)EnemySkills.Ink, (byte)EnemySkills.Crack },
					SpellChance2 = 0x40,
					Spells2 = new List<byte> { (byte)SpellByte.STUN, (byte)SpellByte.STOP, (byte)SpellByte.ZAP, (byte)SpellByte.STOP, (byte)SpellByte.WALL, (byte)SpellByte.XFER, (byte)SpellByte.STUN, (byte)SpellByte.XXXX },
					SkillChance2 = 0x40,
					Skills2 = new List<byte> { (byte)EnemySkills.Crack, (byte)EnemySkills.Trance, (byte)EnemySkills.Crack, (byte)EnemySkills.Gaze },
				},
				new AlternateFiends {
					Name = "GILGAMSH",
					SpriteSheet = FormationSpriteSheet.WizardGarlandDragon2Golem,
					FormationPattern = FormationPattern.Mixed,
					GFXOffset = FormationGFX.Sprite2,
					Palette1 = 0x36, // Yellow/Purple
					Palette2 = 0x36,
					ElementalWeakness = Element.TIME,
					MonsterType = MonsterType.NONE,
					SpellChance1 = 0x40,
					Spells1 = new List<byte> { (byte)SpellByte.FAST, (byte)SpellByte.XFER, (byte)SpellByte.SLOW, (byte)SpellByte.WALL, (byte)SpellByte.NONE, (byte)SpellByte.NONE, (byte)SpellByte.NONE, (byte)SpellByte.NONE },
					SkillChance1 = 0x00,
					Skills1 = new List<byte> { (byte)EnemySkills.None, (byte)EnemySkills.None, (byte)EnemySkills.None, (byte)EnemySkills.None },
					SpellChance2 = 0x40,
					Spells2 = new List<byte> { (byte)SpellByte.FAST, (byte)SpellByte.XFER, (byte)SpellByte.SLO2, (byte)SpellByte.SABR, (byte)SpellByte.XXXX, (byte)SpellByte.SABR, (byte)SpellByte.SLO2, (byte)SpellByte.WALL },
					SkillChance2 = 0x00,
					Skills2 = new List<byte> { (byte)EnemySkills.None, (byte)EnemySkills.None, (byte)EnemySkills.None, (byte)EnemySkills.None },
				},
				new AlternateFiends {
					Name = "IFRIT",
					SpriteSheet = FormationSpriteSheet.VampGargoyleEarthDragon1,
					FormationPattern = FormationPattern.Large4,
					GFXOffset = FormationGFX.Sprite3,
					Palette1 = 0x00, // Brown/Red
					Palette2 = 0x00,
					ElementalWeakness = Element.ICE,
					MonsterType = MonsterType.MAGICAL,
					SpellChance1 = 0x40,
					Spells1 = new List<byte> { (byte)SpellByte.FIRE, (byte)SpellByte.STUN, (byte)SpellByte.FIRE, (byte)SpellByte.FIR2, (byte)SpellByte.NONE, (byte)SpellByte.NONE, (byte)SpellByte.NONE, (byte)SpellByte.NONE },
					SkillChance1 = 0x40,
					Skills1 = new List<byte> { (byte)EnemySkills.Heat, (byte)EnemySkills.Scorch, (byte)EnemySkills.Heat, (byte)EnemySkills.Flash },
					SpellChance2 = 0x40,
					Spells2 = new List<byte> { (byte)SpellByte.FIR2, (byte)SpellByte.FIR3, (byte)SpellByte.XFER, (byte)SpellByte.FIR2, (byte)SpellByte.FIR3, (byte)SpellByte.WALL, (byte)SpellByte.FIR3, (byte)SpellByte.NUKE },
					SkillChance2 = 0x40,
					Skills2 = new List<byte> { (byte)EnemySkills.Inferno, (byte)EnemySkills.Flash, (byte)EnemySkills.Blaze, (byte)EnemySkills.Nuclear },
				},
				new AlternateFiends {
					Name = "LEVIATHN",
					SpriteSheet = FormationSpriteSheet.ImageGeistWormEye,
					FormationPattern = FormationPattern.Large4,
					GFXOffset = FormationGFX.Sprite3,
					Palette1 = 0x01, // Blue/White
					Palette2 = 0x01,
					ElementalWeakness = Element.LIGHTNING,
					MonsterType = MonsterType.GIANT,
					SpellChance1 = 0x40,
					Spells1 = new List<byte> { (byte)SpellByte.ICE, (byte)SpellByte.ICE2, (byte)SpellByte.ICE, (byte)SpellByte.STOP, (byte)SpellByte.NONE, (byte)SpellByte.NONE, (byte)SpellByte.NONE, (byte)SpellByte.NONE },
					SkillChance1 = 0x40,
					Skills1 = new List<byte> { (byte)EnemySkills.Frost, (byte)EnemySkills.Dazzle, (byte)EnemySkills.Stinger, (byte)EnemySkills.Flash },
					SpellChance2 = 0x40,
					Spells2 = new List<byte> { (byte)SpellByte.ICE2, (byte)SpellByte.ICE2, (byte)SpellByte.ICE3, (byte)SpellByte.ICE3, (byte)SpellByte.ICE2, (byte)SpellByte.ICE2, (byte)SpellByte.XFER, (byte)SpellByte.ICE3 },
					SkillChance2 = 0x40,
					Skills2 = new List<byte> { (byte)EnemySkills.Frost, (byte)EnemySkills.Swirl, (byte)EnemySkills.Blizzard, (byte)EnemySkills.Swirl },
				},
				new AlternateFiends {
					Name = "MEDUSAE",
					SpriteSheet = FormationSpriteSheet.KaryLich,
					FormationPattern = FormationPattern.Fiends,
					GFXOffset = FormationGFX.Sprite1,
					Palette1 = 0x23, // Green/Grey
					Palette2 = 0x23,
					ElementalWeakness = Element.POISON,
					MonsterType = MonsterType.MAGE,
					SpellChance1 = 0x40,
					Spells1 = new List<byte> { (byte)SpellByte.STUN, (byte)SpellByte.STOP, (byte)SpellByte.STUN, (byte)SpellByte.BRAK, (byte)SpellByte.NONE, (byte)SpellByte.NONE, (byte)SpellByte.NONE, (byte)SpellByte.NONE },
					SkillChance1 = 0x40,
					Skills1 = new List<byte> { (byte)EnemySkills.Gaze, (byte)EnemySkills.Glance, (byte)EnemySkills.Gaze, (byte)EnemySkills.Glare },
					SpellChance2 = 0x40,
					Spells2 = new List<byte> { (byte)SpellByte.STUN, (byte)SpellByte.STOP, (byte)SpellByte.XXXX, (byte)SpellByte.BRAK, (byte)SpellByte.NONE, (byte)SpellByte.NONE, (byte)SpellByte.NONE, (byte)SpellByte.NONE },
					SkillChance2 = 0x40,
					Skills2 = new List<byte> { (byte)EnemySkills.Glare, (byte)EnemySkills.Glance, (byte)EnemySkills.Glare, (byte)EnemySkills.Poison_Stone },
				},
				new AlternateFiends {
					Name = "ODIN",
					SpriteSheet = FormationSpriteSheet.ImpWolfIguanaGiant,
					FormationPattern = FormationPattern.Large4,
					GFXOffset = FormationGFX.Sprite4,
					Palette1 = 0x26, // Yellow/Blue
					Palette2 = 0x26,
					ElementalWeakness = Element.LIGHTNING,
					MonsterType = MonsterType.GIANT,
					SpellChance1 = 0x40,
					Spells1 = new List<byte> { (byte)SpellByte.STUN, (byte)SpellByte.CUR2, (byte)SpellByte.FAST, (byte)SpellByte.SLOW, (byte)SpellByte.NONE, (byte)SpellByte.NONE, (byte)SpellByte.NONE, (byte)SpellByte.NONE },
					SkillChance1 = 0x40,
					Skills1 = new List<byte> { (byte)EnemySkills.Gaze, (byte)EnemySkills.Flash, (byte)EnemySkills.Flash, (byte)EnemySkills.Crack },
					SpellChance2 = 0x40,
					Spells2 = new List<byte> { (byte)SpellByte.STUN, (byte)SpellByte.CUR3, (byte)SpellByte.XXXX, (byte)SpellByte.SLO2, (byte)SpellByte.CUR3, (byte)SpellByte.STOP, (byte)SpellByte.SLOW, (byte)SpellByte.NUKE },
					SkillChance2 = 0x40,
					Skills2 = new List<byte> { (byte)EnemySkills.Crack, (byte)EnemySkills.Gaze, (byte)EnemySkills.Flash, (byte)EnemySkills.Dazzle },
				},
				new AlternateFiends {
					Name = "RUBICANT",
					SpriteSheet = FormationSpriteSheet.VampGargoyleEarthDragon1,
					FormationPattern = FormationPattern.Large4,
					GFXOffset = FormationGFX.Sprite3,
					Palette1 = 0x0D, // Red/Red
					Palette2 = 0x0D,
					ElementalWeakness = Element.ICE,
					MonsterType = MonsterType.MAGE,
					SpellChance1 = 0x40,
					Spells1 = new List<byte> { (byte)SpellByte.FIRE, (byte)SpellByte.FIRE, (byte)SpellByte.FIRE, (byte)SpellByte.FIR2, (byte)SpellByte.NONE, (byte)SpellByte.NONE, (byte)SpellByte.NONE, (byte)SpellByte.NONE },
					SkillChance1 = 0x40,
					Skills1 = new List<byte> { (byte)EnemySkills.Heat, (byte)EnemySkills.Scorch, (byte)EnemySkills.Flash, (byte)EnemySkills.Dazzle },
					SpellChance2 = 0x40,
					Spells2 = new List<byte> { (byte)SpellByte.FIR2, (byte)SpellByte.FIR2, (byte)SpellByte.FIR3, (byte)SpellByte.NUKE, (byte)SpellByte.FIR2, (byte)SpellByte.FIR3, (byte)SpellByte.FIR3, (byte)SpellByte.NUKE },
					SkillChance2 = 0x40,
					Skills2 = new List<byte> { (byte)EnemySkills.Scorch, (byte)EnemySkills.Heat, (byte)EnemySkills.Blaze, (byte)EnemySkills.Nuclear },
				},
				new AlternateFiends {
					Name = "SALAMAND",
					SpriteSheet = FormationSpriteSheet.WizardGarlandDragon2Golem,
					FormationPattern = FormationPattern.Large4,
					GFXOffset = FormationGFX.Sprite3,
					Palette1 = 0x27, // Orange/Red
					Palette2 = 0x27,
					ElementalWeakness = Element.ICE,
					MonsterType = MonsterType.DRAGON,
					SpellChance1 = 0x40,
					Spells1 = new List<byte> { (byte)SpellByte.FIRE, (byte)SpellByte.DARK, (byte)SpellByte.FIRE, (byte)SpellByte.FIR2, (byte)SpellByte.NONE, (byte)SpellByte.NONE, (byte)SpellByte.NONE, (byte)SpellByte.NONE },
					SkillChance1 = 0x40,
					Skills1 = new List<byte> { (byte)EnemySkills.Flash, (byte)EnemySkills.Heat, (byte)EnemySkills.Flash, (byte)EnemySkills.Heat },
					SpellChance2 = 0x40,
					Spells2 = new List<byte> { (byte)SpellByte.FIR2, (byte)SpellByte.FIR3, (byte)SpellByte.FIR3, (byte)SpellByte.NUKE, (byte)SpellByte.FIR2, (byte)SpellByte.FIR2, (byte)SpellByte.FIR3, (byte)SpellByte.DARK },
					SkillChance2 = 0x40,
					Skills2 = new List<byte> { (byte)EnemySkills.Scorch, (byte)EnemySkills.Blaze, (byte)EnemySkills.Heat, (byte)EnemySkills.Inferno },
				},
				new AlternateFiends {
					Name = "SCARMLIO",
					SpriteSheet = FormationSpriteSheet.BoneCreepHyenaOgre,
					FormationPattern = FormationPattern.Mixed,
					GFXOffset = FormationGFX.Sprite1,
					Palette1 = 0x1B, // Brown/Blue
					Palette2 = 0x1B,
					ElementalWeakness = Element.FIRE,
					MonsterType = MonsterType.UNDEAD,
					SpellChance1 = 0x00,
					Spells1 = new List<byte> { (byte)SpellByte.NONE, (byte)SpellByte.NONE, (byte)SpellByte.NONE, (byte)SpellByte.NONE, (byte)SpellByte.NONE, (byte)SpellByte.NONE, (byte)SpellByte.NONE, (byte)SpellByte.NONE },
					SkillChance1 = 0x40,
					Skills1 = new List<byte> { (byte)EnemySkills.Snorting, (byte)EnemySkills.Gaze, (byte)EnemySkills.Ink, (byte)EnemySkills.Crack },
					SpellChance2 = 0x00,
					Spells2 = new List<byte> { (byte)SpellByte.NONE, (byte)SpellByte.NONE, (byte)SpellByte.NONE, (byte)SpellByte.NONE, (byte)SpellByte.NONE, (byte)SpellByte.NONE, (byte)SpellByte.NONE, (byte)SpellByte.NONE },
					SkillChance2 = 0x40,
					Skills2 = new List<byte> { (byte)EnemySkills.Crack, (byte)EnemySkills.Snorting, (byte)EnemySkills.Crack, (byte)EnemySkills.Snorting },
				},
				new AlternateFiends {
					Name = "SCYLLA",
					SpriteSheet = FormationSpriteSheet.KrakenTiamat,
					FormationPattern = FormationPattern.Fiends,
					GFXOffset = FormationGFX.Sprite2,
					Palette1 = 0x1A, // Blue/Green
					Palette2 = 0x24,
					ElementalWeakness = Element.ICE,
					MonsterType = MonsterType.AQUATIC,
					SpellChance1 = 0x40,
					Spells1 = new List<byte> { (byte)SpellByte.LIT, (byte)SpellByte.LIT2, (byte)SpellByte.LIT, (byte)SpellByte.LIT2, (byte)SpellByte.NONE, (byte)SpellByte.NONE, (byte)SpellByte.NONE, (byte)SpellByte.NONE },
					SkillChance1 = 0x40,
					Skills1 = new List<byte> { (byte)EnemySkills.Dazzle, (byte)EnemySkills.Flash, (byte)EnemySkills.Glance, (byte)EnemySkills.Flash },
					SpellChance2 = 0x40,
					Spells2 = new List<byte> { (byte)SpellByte.LIT2, (byte)SpellByte.LIT2, (byte)SpellByte.LIT3, (byte)SpellByte.SLO2, (byte)SpellByte.LIT2, (byte)SpellByte.LIT2, (byte)SpellByte.LIT3, (byte)SpellByte.FAST },
					SkillChance2 = 0x40,
					Skills2 = new List<byte> { (byte)EnemySkills.Thunder, (byte)EnemySkills.Dazzle, (byte)EnemySkills.Tornado, (byte)EnemySkills.Flash },
				},
				new AlternateFiends {
					Name = "SHIVA",
					SpriteSheet = FormationSpriteSheet.KaryLich,
					FormationPattern = FormationPattern.Fiends,
					GFXOffset = FormationGFX.Sprite1,
					Palette1 = 0x13, // Blue/Purple
					Palette2 = 0x14,
					ElementalWeakness = Element.ICE,
					MonsterType = MonsterType.MAGE,
					SpellChance1 = 0x40,
					Spells1 = new List<byte> { (byte)SpellByte.ICE, (byte)SpellByte.ICE, (byte)SpellByte.ICE, (byte)SpellByte.ICE2, (byte)SpellByte.NONE, (byte)SpellByte.NONE, (byte)SpellByte.NONE, (byte)SpellByte.NONE },
					SkillChance1 = 0x40,
					Skills1 = new List<byte> { (byte)EnemySkills.Frost, (byte)EnemySkills.Flash, (byte)EnemySkills.Gaze, (byte)EnemySkills.Snorting },
					SpellChance2 = 0x40,
					Spells2 = new List<byte> { (byte)SpellByte.ICE2, (byte)SpellByte.ICE3, (byte)SpellByte.ICE2, (byte)SpellByte.ICE2, (byte)SpellByte.ICE3, (byte)SpellByte.ICE3, (byte)SpellByte.ICE2, (byte)SpellByte.BRAK },
					SkillChance2 = 0x40,
					Skills2 = new List<byte> { (byte)EnemySkills.Frost, (byte)EnemySkills.Flash, (byte)EnemySkills.Blizzard, (byte)EnemySkills.Gaze },
				},
				new AlternateFiends {
					Name = "TITAN",
					SpriteSheet = FormationSpriteSheet.ImpWolfIguanaGiant,
					FormationPattern = FormationPattern.Large4,
					GFXOffset = FormationGFX.Sprite4,
					Palette1 = 0x32, // Brown/White
					Palette2 = 0x32,
					ElementalWeakness = Element.LIGHTNING,
					MonsterType = MonsterType.GIANT,
					SpellChance1 = 0x40,
					Spells1 = new List<byte> { (byte)SpellByte.LIT, (byte)SpellByte.LIT, (byte)SpellByte.LIT, (byte)SpellByte.LIT, (byte)SpellByte.NONE, (byte)SpellByte.NONE, (byte)SpellByte.NONE, (byte)SpellByte.NONE },
					SkillChance1 = 0x40,
					Skills1 = new List<byte> { (byte)EnemySkills.Dazzle, (byte)EnemySkills.Gaze, (byte)EnemySkills.Dazzle, (byte)EnemySkills.Flash },
					SpellChance2 = 0x40,
					Spells2 = new List<byte> { (byte)SpellByte.LIT2, (byte)SpellByte.FAST, (byte)SpellByte.LIT3, (byte)SpellByte.SLO2, (byte)SpellByte.STOP, (byte)SpellByte.LIT3, (byte)SpellByte.LIT2, (byte)SpellByte.XFER },
					SkillChance2 = 0x40,
					Skills2 = new List<byte> { (byte)EnemySkills.Swirl, (byte)EnemySkills.Scorch, (byte)EnemySkills.Dazzle, (byte)EnemySkills.Tornado },
				},
			};

			var encountersData = new Encounters(this);

			EnemyInfo[] fiends = new EnemyInfo[8];
			EnemyScriptInfo[] fiendsScript = new EnemyScriptInfo[8];

			for (int i = 0; i < 8; i++)
			{
				fiends[i] = new EnemyInfo();
				fiends[i].decompressData(Get(EnemyOffset + (FiendsIndex + i) * EnemySize, EnemySize));
				fiendsScript[i] = new EnemyScriptInfo();
				fiendsScript[i].decompressData(Get(ScriptOffset + (FiendsScriptIndex + i) * ScriptSize, ScriptSize));
			}

			// Shuffle alternate
			alternateFiendsList.Shuffle(rng);

			// Replace the 4 fiends and their 2nd version at the same time
			for (int i = 0; i < 4; i++)
			{
				fiends[(i * 2)].monster_type = (byte)alternateFiendsList[i].MonsterType;
				fiends[(i * 2) + 1].monster_type = (byte)alternateFiendsList[i].MonsterType;
				fiends[(i * 2)].elem_weakness = (byte)alternateFiendsList[i].ElementalWeakness;
				fiends[(i * 2) + 1].elem_weakness = 0x00;
				fiends[(i * 2)].elem_resist = (byte)(fiends[(i * 2)].elem_resist & ~(byte)alternateFiendsList[i].ElementalWeakness);
				fiends[(i * 2) + 1].elem_resist = (byte)(fiends[(i * 2) + 1].elem_resist & ~(byte)alternateFiendsList[i].ElementalWeakness);

				if (fiendsScript[(i * 2)].skill_chance == 0x00 || alternateFiendsList[i].SkillChance1 == 0x00)
					fiendsScript[(i * 2)].skill_chance = alternateFiendsList[i].SkillChance1;

				if (fiendsScript[(i * 2) + 1].skill_chance == 0x00 || alternateFiendsList[i].SkillChance2 == 0x00)
					fiendsScript[(i * 2) + 1].skill_chance = alternateFiendsList[i].SkillChance2;

				fiendsScript[(i * 2)].skill_list = alternateFiendsList[i].Skills1.ToArray();
				fiendsScript[(i * 2) + 1].skill_list = alternateFiendsList[i].Skills2.ToArray();

				if (fiendsScript[(i * 2)].spell_chance == 0x00 || alternateFiendsList[i].SpellChance1 == 0x00)
					fiendsScript[(i * 2)].spell_chance = alternateFiendsList[i].SpellChance1;

				if (fiendsScript[(i * 2) + 1].spell_chance == 0x00 || alternateFiendsList[i].SpellChance2 == 0x00)
					fiendsScript[(i * 2) + 1].spell_chance = alternateFiendsList[i].SpellChance2;

				fiendsScript[(i * 2)].spell_list = alternateFiendsList[i].Spells1.ToArray();
				fiendsScript[(i * 2) + 1].spell_list = alternateFiendsList[i].Spells2.ToArray();

				encountersData.formations[fiendsFormationOrder[(i * 2)]].pattern = alternateFiendsList[i].FormationPattern;
				encountersData.formations[fiendsFormationOrder[(i * 2)]].spriteSheet = alternateFiendsList[i].SpriteSheet;
				encountersData.formations[fiendsFormationOrder[(i * 2)]].gfxOffset1 = (int)alternateFiendsList[i].GFXOffset;
				encountersData.formations[fiendsFormationOrder[(i * 2)]].palette1 = alternateFiendsList[i].Palette1;
				encountersData.formations[fiendsFormationOrder[(i * 2)]].palette2 = alternateFiendsList[i].Palette2;

				encountersData.formations[fiendsFormationOrder[(i * 2) + 1]].pattern = alternateFiendsList[i].FormationPattern;
				encountersData.formations[fiendsFormationOrder[(i * 2) + 1]].spriteSheet = alternateFiendsList[i].SpriteSheet;
				encountersData.formations[fiendsFormationOrder[(i * 2) + 1]].gfxOffset1 = (int)alternateFiendsList[i].GFXOffset;
				encountersData.formations[fiendsFormationOrder[(i * 2) + 1]].palette1 = alternateFiendsList[i].Palette1;
				encountersData.formations[fiendsFormationOrder[(i * 2) + 1]].palette2 = alternateFiendsList[i].Palette2;
			}

			encountersData.Write(this);

			for (int i = 0; i < 8; i++)
			{
				Put(EnemyOffset + (FiendsIndex + i) * EnemySize, fiends[i].compressData());
				Put(ScriptOffset + (FiendsScriptIndex + i) * ScriptSize, fiendsScript[i].compressData());
			}

			//Update fiends names, we stack Fiend1 and Fiend2's names to get more space for names
			var enemyText = ReadText(EnemyTextPointerOffset, EnemyTextPointerBase, EnemyCount);

			for (int i = 0; i < 4; i++)
			{
				enemyText[119 + (i * 2)] = alternateFiendsList[i].Name;
				enemyText[120 + (i * 2)] = "";
			}

			WriteText(enemyText, EnemyTextPointerOffset, EnemyTextPointerBase, EnemyTextOffset);

			// Rewrite point so Fiend2's name is Fiend1 name
			for (int i = 0; i < 4; i++)
			{
				var namepointer = Get(EnemyTextPointerOffset + (119 + (i * 2)) * 2, 2);
				Put(EnemyTextPointerOffset + (120 + (i * 2)) * 2, namepointer);
			}
		}

		public void SubstituteFormationTableEncounter(byte oldEncounter, byte newEncounter)
		{
			var oldFormations = Get(ZoneFormationsOffset, ZoneFormationsSize * ZoneCount).Chunk(ZoneFormationsSize);
			var newFormations = Get(ZoneFormationsOffset, ZoneFormationsSize * ZoneCount).Chunk(ZoneFormationsSize);
			for (byte i = 0; i < ZoneCount; i++)
			{
				for (int j = 0; j < ZoneFormationsSize; j++)
				{
					if (newFormations[i][j].Equals(oldEncounter))
					{
						newFormations[i][j] = newEncounter;
					}
				}
			}
			Put(ZoneFormationsOffset, newFormations.SelectMany(formation => formation.ToBytes()).ToArray());
		}
	}
}
