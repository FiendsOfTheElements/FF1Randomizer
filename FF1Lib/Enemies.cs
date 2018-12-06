using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RomUtilities;

namespace FF1Lib
{
	public enum FormationShuffleModeEnum
	{
		[Description("Don't Shuffle Encounters")]
		None = 0,
		[Description("Shuffle Encounter Rarity")]
		Intrazone,
		[Description("Shuffle All Encounters Across Zones")]
		InterZone,
		[Description("Random Encounter Zones")]
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

		public abstract class Enemy
		{
			public const int Pirate = 15;
			public const int Garland = 105;
			public const int Astos = 113;
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

		public void ShuffleEnemyFormations(MT19337 rng, FormationShuffleModeEnum shuffleMode)
		{

			if (shuffleMode == FormationShuffleModeEnum.Intrazone)
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

					var shuffleFormations = newFormations[i].SubBlob(1, 6).Chunk(1); // get formations 2-8
					shuffleFormations.Shuffle(rng);
					for (int j = 2; j < 8; j++)
					{
						newFormations[i][j] = shuffleFormations[j - 2][0];
					}

				}

				Put(ZoneFormationsOffset, newFormations.SelectMany(formation => formation.ToBytes()).ToArray());
			}
			if (shuffleMode == FormationShuffleModeEnum.InterZone)
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

			if (shuffleMode == FormationShuffleModeEnum.Randomize)
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
		public void ShuffleEnemyScripts(MT19337 rng, bool AllowUnsafePirates)
		{
			var oldEnemies = Get(EnemyOffset, EnemySize * EnemyCount).Chunk(EnemySize);
			var newEnemies = Get(EnemyOffset, EnemySize * EnemyCount).Chunk(EnemySize);

			var normalOldEnemies = oldEnemies.Take(EnemyCount - 10).ToList(); // all but WarMECH, fiends, fiends revisited, and CHAOS
			normalOldEnemies.Shuffle(rng);
			for (int i = 0; i < EnemyCount - 10; i++)
			{
				newEnemies[i][7] = normalOldEnemies[i][7];
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
			oldBigBosses.Shuffle(rng);

			newEnemies[Enemy.WarMech][7] = oldBigBosses[0][7];
			newEnemies[Enemy.Lich2][7] = oldBigBosses[1][7];
			newEnemies[Enemy.Kary2][7] = oldBigBosses[2][7];
			newEnemies[Enemy.Kraken2][7] = oldBigBosses[3][7];
			newEnemies[Enemy.Tiamat2][7] = oldBigBosses[4][7];
			newEnemies[Enemy.Chaos][7] = oldBigBosses[5][7];

			if (!AllowUnsafePirates)
			{
				if (newEnemies[Enemy.Pirate][7] < 0xFF)
				{
					int swapEnemy = newEnemies.IndexOf(newEnemies.First((enemy) => enemy[7] == 0xFF));
					newEnemies[swapEnemy][7] = newEnemies[Enemy.Pirate][7];
					newEnemies[Enemy.Pirate][7] = 0xFF;
				}
			}

			Put(EnemyOffset, newEnemies.SelectMany(enemy => enemy.ToBytes()).ToArray());
		}

		public void ShuffleEnemySkillsSpells(MT19337 rng)
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
	}
}
