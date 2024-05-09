﻿using System.Collections;
using System.ComponentModel;
using System.Security.Cryptography.X509Certificates;
using FF1Lib.Helpers;

namespace FF1Lib
{
	public enum FormationShuffleMode
	{
		[Description("Vanilla")]
		None = 0,
		[Description("Shuffle Rarity")]
		Intrazone,
		[Description("Shuffle Rarity Tiered")]
		ShuffleRarityTiered,
		[Description("Shuffle Across Zones")]
		InterZone,
		[Description("Totally Random")]
		Randomize
	}
	public enum TouchPool
	{
		[Description("All")]
		All = 0,
		[Description("All except Stun Status")]
		RemoveStun,
		[Description("Balanced Rude")]
		BalancedRude,
		[Description("Only Death Status")]
		OnlyDeath,
		[Description("Random")]
		Random
	}
	public enum TouchMode
	{
		[Description("Vanilla")]
		Standard = 0,
		[Description("Shuffle")]
		Shuffle,
		[Description("Randomize")]
		Randomize,
		[Description("Random")]
		Random
	}
	public partial class FF1Rom : NesRom
	{
		public const int EnemyOffset = 0x30520;
		public const int EnemySize = 20;
		public const int EnemyCount = 128;

		public const int ScriptOffset = 0x31020;
		public const int ScriptSize = 16;
		public const int ScriptCount = 44;

		public const int FormationDataOffset = 0x2C400;
		public const int FormationDataSize = 16;
		public const int FormationDataCount = 128;

		public abstract class Enemy
		{
			public const int Imp = 0;
			public const int RSahag = 13;
			public const int WzSahag = 14;
			public const int Pirate = 15;
			public const int Crawl = 24;
			public const int Asp = 30;
			public const int Cobra = 31;
			public const int Phantom = 51;
			public const int Mancat = 55;
			public const int Vampire = 60;
		        public const int Ankylo = 78;
			public const int Coctrice = 81;
			public const int Sorceror = 104;
			public const int Garland = 105;
			public const int Evilman = 112;
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
		public void ShuffleEnemyScripts(MT19337 rng, Flags flags)
		{

			var oldEnemies = Get(EnemyOffset, EnemySize * EnemyCount).Chunk(EnemySize);
			var newEnemies = Get(EnemyOffset, EnemySize * EnemyCount).Chunk(EnemySize);

			bool shuffleNormalEnemies = (bool)flags.ShuffleScriptsEnemies;
			bool shuffleBosses = (bool)flags.ShuffleScriptsBosses;
			bool allowUnsafePirates = (bool)flags.AllowUnsafePirates;
			bool excludeImps = (bool)flags.EnemySkillsSpellsTiered;
			ScriptTouchMultiplier scriptMultiplier = flags.ScriptMultiplier;

			if (!shuffleNormalEnemies && !shuffleBosses)
			{
				return;
			}

			if (shuffleNormalEnemies)
			{
				var normalOldEnemies = oldEnemies.Take(EnemyCount - 10).ToList(); // all but WarMECH, fiends, fiends revisited, and CHAOS
				if (!allowUnsafePirates) normalOldEnemies.RemoveAt(Enemy.Pirate);
				if (excludeImps) normalOldEnemies.RemoveAt(Enemy.Imp);
				normalOldEnemies.Shuffle(rng);
				if (excludeImps) normalOldEnemies.Insert(Enemy.Imp, oldEnemies[Enemy.Imp]);
				if (!allowUnsafePirates) normalOldEnemies.Insert(Enemy.Pirate, oldEnemies[Enemy.Pirate]);

				var allScripts = normalOldEnemies.Select(e => e[EnemyStat.Scripts]).Distinct().ToList();
				allScripts.Remove(0xFF);

				if (scriptMultiplier == ScriptTouchMultiplier.Vanilla)
				{
					for (int i = 0; i < EnemyCount - 10; i++)
					{
						newEnemies[i][EnemyStat.Scripts] = normalOldEnemies[i][EnemyStat.Scripts];
					}
				}
				else
				{
					var count = 0;
					switch (scriptMultiplier)
					{
						case ScriptTouchMultiplier.None:
							count = 0;
							break;
						case ScriptTouchMultiplier.Half:
							count = 18;
							break;
						case ScriptTouchMultiplier.Vanilla:
							count = 36;//should never happen
							break;
						case ScriptTouchMultiplier.OneAndHalf:
							count = 54;
							break;
						case ScriptTouchMultiplier.Double:
							count = 72;
							break;
						case ScriptTouchMultiplier.All:
							count = EnemyCount - 10;
							break;
						case ScriptTouchMultiplier.Random:
							count = rng.Between(18, 90);
							break;
					}

					List<int> indices = new List<int>();
					for (int i = 0; i < EnemyCount - 10; i++)
					{
						newEnemies[i][EnemyStat.Scripts] = 0xFF;
						indices.Add(i);
					}

					indices.Shuffle(rng);

					foreach (var i in indices.Take(count))
					{
						if (i == 15 && !allowUnsafePirates) continue;
						newEnemies[i][EnemyStat.Scripts] = allScripts.PickRandom(rng);
					}
				}
			}

			if(shuffleBosses)
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
				oldBigBosses.Shuffle(rng);

				newEnemies[Enemy.WarMech][EnemyStat.Scripts] = oldBigBosses[0][EnemyStat.Scripts];
				newEnemies[Enemy.Lich2][EnemyStat.Scripts] = oldBigBosses[1][EnemyStat.Scripts];
				newEnemies[Enemy.Kary2][EnemyStat.Scripts] = oldBigBosses[2][EnemyStat.Scripts];
				newEnemies[Enemy.Kraken2][EnemyStat.Scripts] = oldBigBosses[3][EnemyStat.Scripts];
				newEnemies[Enemy.Tiamat2][EnemyStat.Scripts] = oldBigBosses[4][EnemyStat.Scripts];
				newEnemies[Enemy.Chaos][EnemyStat.Scripts] = oldBigBosses[5][EnemyStat.Scripts];
			}

			Put(EnemyOffset, newEnemies.SelectMany(enemy => enemy.ToBytes()).ToArray());
		}

		public void ShuffleEnemySkillsSpells(MT19337 rng, Flags flags)
		{
			bool shuffleNormalEnemies = (bool)flags.ShuffleSkillsSpellsEnemies;
			bool shuffleBosses = (bool)flags.ShuffleSkillsSpellsBosses;
			bool noConsecutiveNukes = (bool)flags.NoConsecutiveNukes;
			bool nonEmpty = (bool)flags.NoEmptyScripts;
			bool buffedPirates = (bool)flags.SwolePirates;
			bool generateBalancedscript = (bool)flags.EnemySkillsSpellsTiered && shuffleNormalEnemies;

			if (!shuffleBosses && !shuffleNormalEnemies)
			{
				return;
			}

			if (generateBalancedscript)
			{
				GenerateBalancedEnemyScripts(rng, buffedPirates);
			}

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

			if (shuffleNormalEnemies)
			{
			    ShuffleIndexedSkillsSpells(scriptBytes, normalIndices, noConsecutiveNukes, nonEmpty, rng);
			}

			if (shuffleBosses)
			{
			    ShuffleIndexedSkillsSpells(scriptBytes, bossIndices, noConsecutiveNukes, false, rng);
			    ShuffleIndexedSkillsSpells(scriptBytes, bigBossIndices, noConsecutiveNukes, false, rng);
			}

			Put(ScriptOffset, scriptBytes.SelectMany(script => script.ToBytes()).ToArray());
		}

		private void ShuffleIndexedSkillsSpells(List<Blob> scriptBytes, List<int> indices, bool noConsecutiveNukes, bool nonEmpty, MT19337 rng)
		{
			var scripts = indices.Select(i => scriptBytes[i]).ToList();

			var spellBytes = scripts.SelectMany(script => script.SubBlob(2, 8).ToBytes()).Where(b => b != 0xFF).ToList();
			var skillBytes = scripts.SelectMany(script => script.SubBlob(11, 4).ToBytes()).Where(b => b != 0xFF).ToList();

			List<List<byte>> spellBuckets;
			List<List<byte>> skillBuckets;

			// Spellcrafter compatability, search for
			// nuke/nuclear-equivalent spells (AoE
			// non-elemental damage with a base damage of
			// 80 or greater).  In the normal game this
			// matches NUKE and FADE -- in the normal
			// game, no monsters cast FADE, but with spell
			// crafter, anything can happen.
			var nukes = new SpellHelper(this).FindSpells(SpellRoutine.Damage, SpellTargeting.AllEnemies, SpellElement.None, SpellStatus.Any).Where(
			    spell => spell.Item2.effect >= 80).Select(n => ((byte)n.Item1-(byte)Spell.CURE)).ToList();

			var reroll = false;
			do {
			    reroll = false;
				spellBytes.Shuffle(rng);
				skillBytes.Shuffle(rng);
				spellBuckets = Bucketize(spellBytes, scripts.Count(script => script[0] != 0), 8, nonEmpty, rng);
			    skillBuckets = Bucketize(skillBytes, scripts.Count(script => script[1] != 0), 4, false, rng);

				var spellChances = scripts.Select(script => script[0]).ToList();
				var skillChances = scripts.Select(script => script[1]).ToList();

				if (nonEmpty)
				{
					var spellChancesZero = spellChances.Where(c => c == 0);
					var skillChancesZero = skillChances.Where(c => c == 0);
					var spellChancesNonZero = spellChances.Where(c => c != 0).ToList();
					var skillChancesNonZero = skillChances.Where(c => c != 0).ToList();

					spellChancesNonZero.Shuffle(rng);
					spellChances = spellChancesZero.Concat(spellChancesNonZero).ToList();

					skillChancesNonZero.Shuffle(rng);
					skillChances = skillChancesNonZero.Concat(skillChancesZero).ToList();

					var combinedChances = spellChances.Select((c, i) => (spell: c, skill: skillChances[i])).ToList();
					combinedChances.Shuffle(rng);

					spellChances = combinedChances.Select(c => c.spell).ToList();
					skillChances = combinedChances.Select(c => c.skill).ToList();
				}
				else
				{
					spellChances.Shuffle(rng);
					skillChances.Shuffle(rng);
				}

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

				// Check for a few cases of bad
				// scripts to re-roll: two consecutive
				// casts of NUKE, two consecutive
				// casts of NUCLEAR, or having both a
				// NUKE and a NUCLEAR in the starting
				// slots.
				//
				// Because non-elemental damage can't
				// be resisted, the player is always
				// in for a bad time when one of these
				// spells comes out.  The goal is to
				// greatly decrease (although doesn't
				// totally eliminate) the chance of
				// slot machine party wipe situations
				// where the player is hit twice in a
				// row with the most powerful spells
				// in the game with no chance to heal
				// up or counter attack in between.

				bool startingNuke = false;
				bool startingNuclear = false;
				if (spellChances[i] != 0)
				{
				    startingNuke = nukes.Contains(spellBuckets[spellBucketIndex][0]);
				    bool previousWasNuke = false;
				    for (int j = 0; j < spellBuckets[spellBucketIndex].Count; j++)
				    {
					if (nukes.Contains(spellBuckets[spellBucketIndex][j])) {
					    if (previousWasNuke) {
						reroll = true;
					    } else {
						previousWasNuke = true;
					    }
					} else {
					    previousWasNuke = false;
					}
					script[j + 2] = spellBuckets[spellBucketIndex][j];
				    }
				    spellBucketIndex++;
				}
				if (skillChances[i] != 0)
				{
				    startingNuclear = (skillBuckets[skillBucketIndex][0] == (byte)EnemySkills.Nuclear);
				    bool previousWasNuclear = false;
				    for (int j = 0; j < skillBuckets[skillBucketIndex].Count; j++)
				    {
					if (skillBuckets[skillBucketIndex][j] == (byte)EnemySkills.Nuclear) {
					    if (previousWasNuclear) {
						reroll = true;
					    } else {
						previousWasNuclear = true;
					    }
					} else {
					    previousWasNuclear = false;
					}
					script[j + 11] = skillBuckets[skillBucketIndex][j];
				    }
				    skillBucketIndex++;
				}
				if (startingNuke && startingNuclear) {
				    reroll = true;
				}
			    }
			} while (noConsecutiveNukes && reroll);
		}

		private List<List<byte>> Bucketize(List<byte> bytes, int bucketCount, int bucketSize, bool min2, MT19337 rng)
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

			if (min2)
			{
				while (index < 2 * bucketCount)
				{
					buckets[index - bucketCount].Add(bytes[index++]);
				}
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
		public void StatusAttacks(Flags flags, MT19337 rng)
		{
			if (flags.TouchMode == TouchMode.Random)
			{
				var selectedMode = rng.Between(0, Enum.GetNames(typeof(TouchMode)).Length - 1);

				flags.TouchMode = (TouchMode)selectedMode;
			}

			if (flags.TouchMode == TouchMode.Standard)
			{
				return;
			}
			else if (flags.TouchMode == TouchMode.Shuffle)
			{
				ShuffleEnemyStatusAttacks(rng, (bool)flags.AllowUnsafePirates);
			}
			else if (flags.TouchMode == TouchMode.Randomize)
			{
				RandomEnemyStatusAttacks(rng, (bool)flags.AllowUnsafePirates, (bool)flags.TouchIncludeBosses, flags.TouchPool, flags.TouchMultiplier, (bool)flags.IncreaseDarkPenalty);
			}

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

		public void RandomEnemyStatusAttacks(MT19337 rng, bool AllowUnsafePirates, bool includeBosses, TouchPool touchPool, ScriptTouchMultiplier touchMultiplier, bool IncreaseDarkPenalty)
		{
			if (touchPool == TouchPool.Random)
			{
				int selectedPool = rng.Between(0, Enum.GetNames(typeof(TouchPool)).Length - 1);

				touchPool = (TouchPool)selectedPool;
			}

			var enemies = Get(EnemyOffset, EnemySize * EnemyCount).Chunk(EnemySize);

			List<(byte touch, byte element)> statusElements = new List<(byte touch, byte element)>()
			{
				(0x04, 0x02), //Poison Touch = Poison
				(0x08, 0x01), //Dark Touch = Status
				(0x10, 0x01), //Stun Touch = Status
				(0x20, 0x01), //Sleep Touch = Status
				(0x40, 0x01), //Mute Touch = Status
			};

			if (touchPool == TouchPool.RemoveStun || touchPool == TouchPool.BalancedRude)
			{
				statusElements.Remove((0x10, 0x01));
			}

			(byte touch, byte element) deathElement = (0x01, 0x08); //Death Touch = Death Element
			(byte touch, byte element) stoneElement = (0x02, 0x02); //Stone Touch = Poison
			(byte touch, byte element) stunElement = (0x10, 0x01); //Stun Touch = Status
			(byte touch, byte element) muteElement = (0x40, 0x01); //Mute Touch = Status
			(byte touch, byte element) darkElement = (0x08, 0x01); //Dark Touch = Status

			List<(byte touch, byte element)> weightedStatusElements = new();

			if (touchPool == TouchPool.OnlyDeath)
			{
				weightedStatusElements.Add(deathElement);
			}
			else if (touchPool == TouchPool.BalancedRude)
			{
				// Remove Dark from main pool unless Improved Dark is active
				if (!IncreaseDarkPenalty)
				{
					statusElements.Remove((0x08, 0x01));
				}
				while (weightedStatusElements.Count < 8)
				{
					weightedStatusElements.AddRange(statusElements); // Normally: Mute, Sleep, Poison
				}

				// Only one copy of Dark in the pool normally since it's weak without Improved Dark
				weightedStatusElements.Add(darkElement);
				
				// One extra Mute since it's Rude
				weightedStatusElements.Add(muteElement);
				// One each of Stun, Death, Stone
				weightedStatusElements.Add(stunElement);
				weightedStatusElements.Add(deathElement);
				weightedStatusElements.Add(stoneElement);
			}
			// "All" and "All Except Stun"
			else
			{
				while (weightedStatusElements.Count < 20)
				{
					weightedStatusElements.AddRange(statusElements);
				}

				weightedStatusElements.Add(deathElement);
				weightedStatusElements.Add(stoneElement);
			}

			var count = 0;
			switch (touchMultiplier)
			{
				case ScriptTouchMultiplier.None:
					count = 0;
					break;
				case ScriptTouchMultiplier.Half:
					count = 18;
					break;
				case ScriptTouchMultiplier.Vanilla:
					count = 36; //should never happen / now it does!
					break;
				case ScriptTouchMultiplier.OneAndHalf:
					count = 54;
					break;
				case ScriptTouchMultiplier.Double:
					count = 72;
					break;
				case ScriptTouchMultiplier.All:
					count = EnemyCount - 10;
					break;
				case ScriptTouchMultiplier.Random:
					count = rng.Between(18, 90);
					break;
			}

			List<int> indices = new List<int>();
			for (int i = 0; i < (includeBosses ? EnemyCount : (EnemyCount - 10)); i++)
			{
				enemies[i][14] = 0x00;
				enemies[i][15] = 0x00;

				indices.Add(i);
			}

			indices.Shuffle(rng);

			foreach (var i in indices.Take(count))
			{
				if (i == 15 && !AllowUnsafePirates) continue;
				var (touch, element) = weightedStatusElements.PickRandom(rng);
				enemies[i][15] = touch;
				enemies[i][14] = element;
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
		public void TranceHasStatusElement() {
		    // TRANCE is slot 81, give is "status" element so
		    // it can be resisted with a ribbon, ARUB, or
		    // armor crafter gear.
		    var es = new EnemySkillInfo();
		    es.decompressData(Get(MagicOffset + MagicSize * 81, EnemySkillSize));
		    System.Diagnostics.Debug.Assert(es.accuracy == 0);
		    System.Diagnostics.Debug.Assert(es.effect == (byte)SpellStatus.Stun);
		    System.Diagnostics.Debug.Assert(es.elem == (byte)SpellElement.None);
		    System.Diagnostics.Debug.Assert(es.targeting == (byte)SpellTargeting.AllEnemies);
		    System.Diagnostics.Debug.Assert(es.routine == (byte)SpellRoutine.InflictStatus);
		    es.elem = (byte)SpellElement.Status;
		    Put(MagicOffset + MagicSize * 81, es.compressData());
		}

		public List<EnemyInfo> GetEnemies() {
		    var enm = new List<EnemyInfo>();
		    var enemyText = ReadText(EnemyTextPointerOffset, EnemyTextPointerBase, EnemyCount);
		    var scripts = GetEnemyScripts();
		    for (int i = 0; i < EnemyCount; ++i)
		    {
			var enemy = new EnemyInfo();
			enemy.decompressData(Get(EnemyOffset + i * EnemySize, EnemySize));
			enemy.allAIScripts = scripts;
			enemy.name = enemyText[i];
			enemy.index = i;
			enm.Add(enemy);
		    }
		    return enm;
		}


		public List<EnemyScriptInfo> GetEnemyScripts() {
		    var spells = this.GetSpells();
		    var skills = this.GetEnemySkills();
		    var scripts = new List<EnemyScriptInfo>();
		    for (int i = 0; i < ScriptCount; ++i)
		    {
			var script = new EnemyScriptInfo();
			script.index = (byte)i;
			script.allGameSpells = spells;
			script.allEnemySkills = skills;
			script.decompressData(Get(ScriptOffset + i * ScriptSize, ScriptSize));
			scripts.Add(script);
		    }
		    return scripts;
		}

		public List<MagicSpell> GetEnemySkills() {
		    var skills = Get(EnemySkillOffset, MagicSize * EnemySkillCount).Chunk(MagicSize);
		    var skillNames = ReadText(EnemySkillTextPointerOffset, EnemySkillTextPointerBase, EnemySkillCount);
		    return skills.Select((spell, i) => new MagicSpell((byte)i, spell, skillNames[i], false)).ToList();
		}
	}

	public enum ScriptTouchMultiplier
	{
		[Description("None")]
		None,

		[Description("Half")]
		Half,

		[Description("Vanilla")]
		Vanilla,

		[Description("Increased")]
		OneAndHalf,

		[Description("Double")]
		Double,

		[Description("All")]
		All,

		[Description("Random")]
		Random,
	}

}
