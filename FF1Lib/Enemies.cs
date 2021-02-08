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

		public void ShuffleEnemyScripts(MT19337 rng, bool AllowUnsafePirates, bool doNormals, bool excludeImps, bool scaryImps)
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
					newEnemies[i][7] = normalOldEnemies[i][7];
				}
			}

			var oldBosses = new List<Blob>
			{
				oldEnemies[Enemy.Lich],
				oldEnemies[Enemy.Kary],
				oldEnemies[Enemy.Kraken],
				oldEnemies[Enemy.Tiamat]
			};
			oldBosses.Shuffle(rng);

			newEnemies[Enemy.Lich][7] = oldBosses[0][7];
			newEnemies[Enemy.Kary][7] = oldBosses[1][7];
			newEnemies[Enemy.Kraken][7] = oldBosses[2][7];
			newEnemies[Enemy.Tiamat][7] = oldBosses[3][7];

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

			newEnemies[Enemy.WarMech][7] = oldBigBosses[0][7];
			newEnemies[Enemy.Lich2][7] = oldBigBosses[1][7];
			newEnemies[Enemy.Kary2][7] = oldBigBosses[2][7];
			newEnemies[Enemy.Kraken2][7] = oldBigBosses[3][7];
			newEnemies[Enemy.Tiamat2][7] = oldBigBosses[4][7];
			newEnemies[Enemy.Chaos][7] = oldBigBosses[5][7];
			if (scaryImps) newEnemies[Enemy.Imp][7] = oldBigBosses[6][7];

			Put(EnemyOffset, newEnemies.SelectMany(enemy => enemy.ToBytes()).ToArray());
		}

		public void ShuffleEnemySkillsSpells(MT19337 rng, bool doNormals)
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
			ShuffleIndexedSkillsSpells(scriptBytes, bossIndices, rng);
			ShuffleIndexedSkillsSpells(scriptBytes, bigBossIndices, rng);

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
			None = 0x00,
			Frost = 0x01,
			Heat = 0x02,
			Glance = 0x03,
			Gaze = 0x04,
			Flash = 0x05,
			Scorch = 0x06,
			Crack = 0x07,
			Squint = 0x08,
			Stare = 0x09,
			Glare = 0x0A,
			Blizzard = 0x0B,
			Blaze = 0x0C,
			Inferno = 0x0D,
			Cremate = 0x0E,
			Poison_Stone = 0x0F,
			Trance = 0x10,
			Poison_Damage = 0x11,
			Thunder = 0x12,
			Toxic = 0x13,
			Snorting = 0x14,
			Nuclear = 0x15,
			Ink = 0x16,
			Stinger = 0x17,
			Dazzle = 0x18,
			Swirl = 0x19,
			Tornado = 0x1A
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
			XXXX = 0x3F
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
		public void AlternativeFiends(MT19337 rng, bool AllowUnsafePirates, bool doNormals, bool excludeImps, bool scaryImps)
		{
			const int FiendsIndex = 0x77;
			//const int FiendsFormationIndex = 0x73; // Lich2 to Tiamat2, and then Tiamat to lich
			const int FiendsScriptIndex = 0x23;
			var fiendsFormationOrder = new List<int> { 0x7A, 0x73, 0x79, 0x74, 0x78, 0x75, 0x77, 0x76 };
			var alternateFiendsList = new List<AlternateFiends> {
				new AlternateFiends {
					Name = "ASTAROTH",
					SpriteSheet = FormationSpriteSheet.BadmanAstosMadponyWarmech,
					FormationPattern = FormationPattern.Mixed,
					GFXOffset = FormationGFX.Sprite2,
					Palette1 = 0x00,
					Palette2 = 0x00,
					ElementalWeakness = Element.FIRE,
					MonsterType = MonsterType.MAGE,
					SpellChance1 = 0x40,
					Spells1 = new List<byte> { (byte)SpellByte.FIRE, (byte)SpellByte.FIRE, (byte)SpellByte.BRAK, (byte)SpellByte.FIRE, (byte)SpellByte.FIRE, (byte)SpellByte.FIRE, (byte)SpellByte.BRAK, (byte)SpellByte.FIRE },
					SkillChance1 = 0x40,
					Skills1 = new List<byte> { (byte)EnemySkills.Heat, (byte)EnemySkills.Heat, (byte)EnemySkills.Heat, (byte)EnemySkills.Poison_Stone },
					SpellChance2 = 0x40,
					Spells2 = new List<byte> { (byte)SpellByte.FIR2, (byte)SpellByte.FIR3, (byte)SpellByte.FIR2, (byte)SpellByte.BRAK, (byte)SpellByte.FIR2, (byte)SpellByte.FIR3, (byte)SpellByte.FIR2, (byte)SpellByte.BANE },
					SkillChance2 = 0x40,
					Skills2 = new List<byte> { (byte)EnemySkills.Scorch, (byte)EnemySkills.Poison_Damage, (byte)EnemySkills.Blaze, (byte)EnemySkills.Poison_Stone },
				}
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

			for (int i = 0; i < 4; i++)
			{
				fiends[i].monster_type = 0x00;
				fiends[i + 4].monster_type = 0x00;
				fiends[i].elem_weakness = 0x00;
				fiends[i + 4].elem_weakness = 0x00;
				fiends[i].elem_resist = (byte)(fiends[i].elem_resist & 0xFF);
				fiends[i + +4].elem_resist = (byte)(fiends[i + 4].elem_resist & 0xFF);

				if (fiendsScript[i].skill_chance == 0x00 && true)
					fiendsScript[i].skill_chance = 0x40;
				else
					fiendsScript[i].skill_chance = 0xFF;

				fiendsScript[i].skill_list = new byte[] { 0x00 };

				if (fiendsScript[i].spell_chance == 0x00 && true)
					fiendsScript[i].spell_chance = 0x40;
				else
					fiendsScript[i].spell_chance = 0xFF;

				fiendsScript[i].spell_list = new byte[] { 0x00 };

				encountersData.formations[fiendsFormationOrder[i]].pattern = FormationPattern.Mixed;
				encountersData.formations[fiendsFormationOrder[i]].spriteSheet = FormationSpriteSheet.KaryLich;
				encountersData.formations[fiendsFormationOrder[i]].gfxOffset1 = 0x00;
				encountersData.formations[fiendsFormationOrder[i]].palette1 = 0x10;
				encountersData.formations[fiendsFormationOrder[i]].palette2 = 0x10;
			}

			encountersData.Write(this);


			//Update enemies names
			var enemyText = ReadText(EnemyTextPointerOffset, EnemyTextPointerBase, EnemyCount);

			for (int i = 0; i < 4; i++)
			{
				enemyText[119 + (i * 2)] = "FIEND";
				enemyText[120 + (i * 2)] = "";
			}

			WriteText(enemyText, EnemyTextPointerOffset, EnemyTextPointerBase, EnemyTextOffset);

			for (int i = 0; i < 4; i++)
			{
				var namepointer = Get(EnemyTextPointerOffset + (119 + (i * 2)) * 2, 2);
				Put(EnemyTextPointerOffset + (120 + (i * 2)) * 2, namepointer);
			}



			// Enemies EnemyInfo
			// 1. Set weakness
			// 2. AND weakness out of resistance
			// 3. Set type
			//
			// SkillSets EnemySkillInfo
			// 1. Set Spells/Skills
			// 2. 0? set to 64, else keep original
			//
			// Formations
			// 1. Set type
			// 2. Sprite
			// 3. Palette
			// 4. Qty?
			// 
			// Name table
			// 1. Change name
			// 2. Pointer to first name for more space

			/*

			if (doNormals)
			{
				var normalOldEnemies = oldEnemies.Take(EnemyCount - 10).ToList(); // all but WarMECH, fiends, fiends revisited, and CHAOS
				if (!AllowUnsafePirates) normalOldEnemies.RemoveAt(Enemy.Pirate);
				if (excludeImps) normalOldEnemies.RemoveAt(Enemy.Imp);
				normalOldEnemies.Shuffle(rng);
				if (excludeImps) normalOldEnemies.Insert(Enemy.Imp, oldEnemies[Enemy.Imp]);
				if (!AllowUnsafePirates) normalOldEnemies.Insert(Enemy.Pirate, oldEnemies[Enemy.Pirate]);

				for (int i = 0; i < EnemyCount - 10; i++)
				{
					newEnemies[i][7] = normalOldEnemies[i][7];
				}
			}

			var oldBosses = new List<Blob>
			{
				oldEnemies[Enemy.Lich],
				oldEnemies[Enemy.Kary],
				oldEnemies[Enemy.Kraken],
				oldEnemies[Enemy.Tiamat]
			};
			oldBosses.Shuffle(rng);

			newEnemies[Enemy.Lich][7] = oldBosses[0][7];
			newEnemies[Enemy.Kary][7] = oldBosses[1][7];
			newEnemies[Enemy.Kraken][7] = oldBosses[2][7];
			newEnemies[Enemy.Tiamat][7] = oldBosses[3][7];

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

			newEnemies[Enemy.WarMech][7] = oldBigBosses[0][7];
			newEnemies[Enemy.Lich2][7] = oldBigBosses[1][7];
			newEnemies[Enemy.Kary2][7] = oldBigBosses[2][7];
			newEnemies[Enemy.Kraken2][7] = oldBigBosses[3][7];
			newEnemies[Enemy.Tiamat2][7] = oldBigBosses[4][7];
			newEnemies[Enemy.Chaos][7] = oldBigBosses[5][7];
			if (scaryImps) newEnemies[Enemy.Imp][7] = oldBigBosses[6][7];

			Put(EnemyOffset, newEnemies.SelectMany(enemy => enemy.ToBytes()).ToArray());
			*/
		}
	}
}
