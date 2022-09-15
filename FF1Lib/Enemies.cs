﻿using System.ComponentModel;
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

		public const int ZoneFormationsOffset = 0x2C000;
		public const int ZoneFormationsSize = 8;
		public const int ZoneCount = 128;

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

		public void ShuffleEnemyFormations(MT19337 rng, FormationShuffleMode shuffleMode)
		{

			if (shuffleMode == FormationShuffleMode.Intrazone)
			{
				// intra-zone shuffle, does not change which formations are in zones.
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
			if(shuffleMode == FormationShuffleMode.ShuffleRarityTiered) {
				// intra-zone shuffle, does not change which formations are in zones.
				var oldFormations = Get(ZoneFormationsOffset, ZoneFormationsSize * ZoneCount).Chunk(ZoneFormationsSize);
				var newFormations = new List<Blob>();

				for (int i = 0; i < ZoneCount; i++)
				{
					var currentEncounterZone = oldFormations[i].Chunk(1);

					List<(Blob, int)> weightedEncounterPool = new List<(Blob, int)>();

					//3 tiers
					//encounter groups 1-4 in highest frequency
					//5-6 in mid
					//7-8 in low

					weightedEncounterPool.Add((currentEncounterZone[0], 48));
					weightedEncounterPool.Add((currentEncounterZone[1], 48));
					weightedEncounterPool.Add((currentEncounterZone[2], 48));
					weightedEncounterPool.Add((currentEncounterZone[3], 48));
					weightedEncounterPool.Add((currentEncounterZone[4], 24));
					weightedEncounterPool.Add((currentEncounterZone[5], 24));
					weightedEncounterPool.Add((currentEncounterZone[6], 8));
					weightedEncounterPool.Add((currentEncounterZone[7], 8));

					newFormations.Add(weightedEncounterPool.SpliceRandomItemWeighted(rng));
					newFormations.Add(weightedEncounterPool.SpliceRandomItemWeighted(rng));
					newFormations.Add(weightedEncounterPool.SpliceRandomItemWeighted(rng));
					newFormations.Add(weightedEncounterPool.SpliceRandomItemWeighted(rng));
					newFormations.Add(weightedEncounterPool.SpliceRandomItemWeighted(rng));
					newFormations.Add(weightedEncounterPool.SpliceRandomItemWeighted(rng));
					newFormations.Add(weightedEncounterPool.SpliceRandomItemWeighted(rng));
					newFormations.Add(weightedEncounterPool.SpliceRandomItemWeighted(rng));
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
			spellBytes.Shuffle(rng);
			skillBytes.Shuffle(rng);

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
			public SpellElement ElementalWeakness;
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
					ElementalWeakness = SpellElement.Fire,
					MonsterType = MonsterType.MAGE,
					SpellChance1 = 0x40,
					Spells1 = new List<byte> { (byte)SpellByte.FIRE, (byte)SpellByte.FIRE, (byte)SpellByte.BRAK, (byte)SpellByte.FIRE, (byte)SpellByte.FIRE, (byte)SpellByte.FIRE, (byte)SpellByte.BRAK, (byte)SpellByte.FIRE },
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
					ElementalWeakness = SpellElement.None,
					MonsterType = MonsterType.REGENERATIVE,
					SpellChance1 = 0x40,
					Spells1 = new List<byte> { (byte)SpellByte.CUR2, (byte)SpellByte.CUR3, (byte)SpellByte.CUR3, (byte)SpellByte.FAST, (byte)SpellByte.CUR2, (byte)SpellByte.CUR3, (byte)SpellByte.CUR3, (byte)SpellByte.FAST },
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
					ElementalWeakness = SpellElement.Earth,
					MonsterType = MonsterType.MAGICAL,
					SpellChance1 = 0x40,
					Spells1 = new List<byte> { (byte)SpellByte.LIT, (byte)SpellByte.DARK, (byte)SpellByte.SLOW, (byte)SpellByte.LIT2, (byte)SpellByte.LIT, (byte)SpellByte.DARK, (byte)SpellByte.SLOW, (byte)SpellByte.LIT2 },
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
					ElementalWeakness = SpellElement.Lightning,
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
					ElementalWeakness = SpellElement.Ice,
					MonsterType = MonsterType.UNDEAD,
					SpellChance1 = 0x40,
					Spells1 = new List<byte> { (byte)SpellByte.FIR2, (byte)SpellByte.STUN, (byte)SpellByte.FIR2, (byte)SpellByte.FAST, (byte)SpellByte.FIR2, (byte)SpellByte.STUN, (byte)SpellByte.FIR2, (byte)SpellByte.FAST },
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
					ElementalWeakness = SpellElement.Poison,
					MonsterType = MonsterType.MAGE,
					SpellChance1 = 0x40,
					Spells1 = new List<byte> { (byte)SpellByte.FAST, (byte)SpellByte.SLOW, (byte)SpellByte.STUN, (byte)SpellByte.STOP, (byte)SpellByte.FAST, (byte)SpellByte.SLOW, (byte)SpellByte.STUN, (byte)SpellByte.STOP },
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
					ElementalWeakness = SpellElement.Lightning,
					MonsterType = MonsterType.AQUATIC,
					SpellChance1 = 0x40,
					Spells1 = new List<byte> { (byte)SpellByte.ICE, (byte)SpellByte.STUN, (byte)SpellByte.ICE, (byte)SpellByte.ICE2, (byte)SpellByte.ICE, (byte)SpellByte.STUN, (byte)SpellByte.ICE, (byte)SpellByte.ICE2 },
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
					ElementalWeakness = SpellElement.None,
					MonsterType = MonsterType.MAGICAL,
					SpellChance1 = 0x40,
					Spells1 = new List<byte> { (byte)SpellByte.CURE, (byte)SpellByte.FAST, (byte)SpellByte.CURE, (byte)SpellByte.SLOW, (byte)SpellByte.CURE, (byte)SpellByte.FAST, (byte)SpellByte.CURE, (byte)SpellByte.SLOW },
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
					ElementalWeakness = SpellElement.Ice,
					MonsterType = MonsterType.MAGICAL,
					SpellChance1 = 0x40,
					Spells1 = new List<byte> { (byte)SpellByte.FIRE, (byte)SpellByte.FIRE, (byte)SpellByte.FIRE, (byte)SpellByte.FAST, (byte)SpellByte.FIRE, (byte)SpellByte.FIRE, (byte)SpellByte.FIRE, (byte)SpellByte.FAST },
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
					ElementalWeakness = SpellElement.Fire,
					MonsterType = MonsterType.MAGE,
					SpellChance1 = 0x40,
					Spells1 = new List<byte> { (byte)SpellByte.STUN, (byte)SpellByte.STOP, (byte)SpellByte.STUN, (byte)SpellByte.SLOW, (byte)SpellByte.STUN, (byte)SpellByte.STOP, (byte)SpellByte.STUN, (byte)SpellByte.SLOW },
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
					ElementalWeakness = SpellElement.Time,
					MonsterType = MonsterType.NONE,
					SpellChance1 = 0x40,
					Spells1 = new List<byte> { (byte)SpellByte.FAST, (byte)SpellByte.XFER, (byte)SpellByte.SLOW, (byte)SpellByte.WALL, (byte)SpellByte.FAST, (byte)SpellByte.XFER, (byte)SpellByte.SLOW, (byte)SpellByte.WALL },
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
					ElementalWeakness = SpellElement.Ice,
					MonsterType = MonsterType.MAGICAL,
					SpellChance1 = 0x40,
					Spells1 = new List<byte> { (byte)SpellByte.FIRE, (byte)SpellByte.STUN, (byte)SpellByte.FIRE, (byte)SpellByte.FIR2, (byte)SpellByte.FIRE, (byte)SpellByte.STUN, (byte)SpellByte.FIRE, (byte)SpellByte.FIR2 },
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
					ElementalWeakness = SpellElement.Lightning,
					MonsterType = MonsterType.GIANT,
					SpellChance1 = 0x40,
					Spells1 = new List<byte> { (byte)SpellByte.ICE, (byte)SpellByte.ICE2, (byte)SpellByte.ICE, (byte)SpellByte.STOP, (byte)SpellByte.ICE, (byte)SpellByte.ICE2, (byte)SpellByte.ICE, (byte)SpellByte.STOP },
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
					ElementalWeakness = SpellElement.Poison,
					MonsterType = MonsterType.MAGE,
					SpellChance1 = 0x40,
					Spells1 = new List<byte> { (byte)SpellByte.STUN, (byte)SpellByte.STOP, (byte)SpellByte.STUN, (byte)SpellByte.BRAK, (byte)SpellByte.STUN, (byte)SpellByte.STOP, (byte)SpellByte.STUN, (byte)SpellByte.BRAK },
					SkillChance1 = 0x40,
					Skills1 = new List<byte> { (byte)EnemySkills.Gaze, (byte)EnemySkills.Glance, (byte)EnemySkills.Gaze, (byte)EnemySkills.Glare },
					SpellChance2 = 0x40,
					Spells2 = new List<byte> { (byte)SpellByte.STUN, (byte)SpellByte.STOP, (byte)SpellByte.XXXX, (byte)SpellByte.BRAK, (byte)SpellByte.STUN, (byte)SpellByte.STOP, (byte)SpellByte.XXXX, (byte)SpellByte.BRAK },
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
					ElementalWeakness = SpellElement.Lightning,
					MonsterType = MonsterType.GIANT,
					SpellChance1 = 0x40,
					Spells1 = new List<byte> { (byte)SpellByte.STUN, (byte)SpellByte.CUR2, (byte)SpellByte.FAST, (byte)SpellByte.SLOW, (byte)SpellByte.STUN, (byte)SpellByte.CUR2, (byte)SpellByte.FAST, (byte)SpellByte.SLOW },
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
					ElementalWeakness = SpellElement.Ice,
					MonsterType = MonsterType.MAGE,
					SpellChance1 = 0x40,
					Spells1 = new List<byte> { (byte)SpellByte.FIRE, (byte)SpellByte.FIRE, (byte)SpellByte.FIRE, (byte)SpellByte.FIR2, (byte)SpellByte.FIRE, (byte)SpellByte.FIRE, (byte)SpellByte.FIRE, (byte)SpellByte.FIR2 },
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
					ElementalWeakness = SpellElement.Ice,
					MonsterType = MonsterType.DRAGON,
					SpellChance1 = 0x40,
					Spells1 = new List<byte> { (byte)SpellByte.FIRE, (byte)SpellByte.DARK, (byte)SpellByte.FIRE, (byte)SpellByte.FIR2, (byte)SpellByte.FIRE, (byte)SpellByte.DARK, (byte)SpellByte.FIRE, (byte)SpellByte.FIR2 },
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
					ElementalWeakness = SpellElement.Fire,
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
					ElementalWeakness = SpellElement.Ice,
					MonsterType = MonsterType.AQUATIC,
					SpellChance1 = 0x40,
					Spells1 = new List<byte> { (byte)SpellByte.LIT, (byte)SpellByte.LIT2, (byte)SpellByte.LIT, (byte)SpellByte.LIT2, (byte)SpellByte.LIT, (byte)SpellByte.LIT2, (byte)SpellByte.LIT, (byte)SpellByte.LIT2 },
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
					ElementalWeakness = SpellElement.Ice,
					MonsterType = MonsterType.MAGE,
					SpellChance1 = 0x40,
					Spells1 = new List<byte> { (byte)SpellByte.ICE, (byte)SpellByte.ICE, (byte)SpellByte.ICE, (byte)SpellByte.ICE2, (byte)SpellByte.ICE, (byte)SpellByte.ICE, (byte)SpellByte.ICE, (byte)SpellByte.ICE2 },
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
					ElementalWeakness = SpellElement.Lightning,
					MonsterType = MonsterType.GIANT,
					SpellChance1 = 0x40,
					Spells1 = new List<byte> { (byte)SpellByte.LIT, (byte)SpellByte.LIT, (byte)SpellByte.LIT, (byte)SpellByte.LIT, (byte)SpellByte.LIT, (byte)SpellByte.LIT, (byte)SpellByte.LIT, (byte)SpellByte.LIT },
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

			var assembly = System.Reflection.Assembly.GetExecutingAssembly();

			while (true) {
			    // Shuffle alternate
			    alternateFiendsList.Shuffle(rng);

			    while (alternateFiendsList.Count >= 4) {
				var resourcePath1 = assembly.GetManifestResourceNames().First(str => str.EndsWith(alternateFiendsList[0].Name + ".png"));
				var resourcePath2 = assembly.GetManifestResourceNames().First(str => str.EndsWith(alternateFiendsList[1].Name + ".png"));
				using (Stream stream1 = assembly.GetManifestResourceStream(resourcePath1)) {
				    using (Stream stream2 = assembly.GetManifestResourceStream(resourcePath2)) {
					//if (await SetLichKaryGraphics(stream1, stream2)) {
					if (SetLichKaryGraphics(stream1, stream2)) {
					    break;
					}
					// The graphics didn't fit, throw out the first element and try the next pair
					alternateFiendsList.RemoveAt(0);
				    }
				}
			    }
			    if (alternateFiendsList.Count < 4) {
				// Couldn't find a pair where the graphics fit, reshuffle
				continue;
			    }

			    while (alternateFiendsList.Count >= 4) {
				var resourcePath1 = assembly.GetManifestResourceNames().First(str => str.EndsWith(alternateFiendsList[2].Name + ".png"));
				var resourcePath2 = assembly.GetManifestResourceNames().First(str => str.EndsWith(alternateFiendsList[3].Name + ".png"));
				using (Stream stream1 = assembly.GetManifestResourceStream(resourcePath1)) {
				    using (Stream stream2 = assembly.GetManifestResourceStream(resourcePath2)) {
					//if (await SetKrakenTiamatGraphics(stream1, stream2)) {
					if (SetKrakenTiamatGraphics(stream1, stream2)) {
					    break;
					}
					alternateFiendsList.RemoveAt(2);
				    }
				}
			    }
			    if (alternateFiendsList.Count < 4) {
				continue;
			    }
			    break;
			}

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

				/*
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
				*/
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
