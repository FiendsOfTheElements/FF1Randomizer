using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RomUtilities;

namespace FF1Lib
{
	public partial class FF1Rom : NesRom
	{
		public const int EnemyOffset = 0x30520;
		public const int EnemySize = 20;
		public const int EnemyCount = 128;

		public const int ScriptOffset = 0x31020;
		public const int ScriptSize = 16;
		public const int ScriptCount = 44;

		public const int FormationFrequencyOffset = 0x2C000;
		public const int FormationFrequencySize = 8;
		public const int FormationFrequencyCount = 128;

		public void ShuffleEnemyFormations(MT19337 rng)
		{
			//intra-zone shuffle, does not change which formations are in zomes.
			var oldFormations = Get(FormationFrequencyOffset, FormationFrequencySize * FormationFrequencyCount).Chunk(FormationFrequencySize);
			var newFormations = Get(FormationFrequencyOffset, FormationFrequencySize * FormationFrequencyCount).Chunk(FormationFrequencySize);
			Blob WarMech = new byte[]{ 0x56 };

			for (int i = 0; i < FormationFrequencyCount; i++)
			{
				
				var lowFormations = oldFormations[i].Chunk(4)[0].Chunk(1); // shuffle the first 4 formations first
				lowFormations.Shuffle(rng);
				newFormations[i][0] = lowFormations[0][0];
				newFormations[i][1] = lowFormations[1][0];
				newFormations[i][2] = lowFormations[2][0];
				newFormations[i][3] = lowFormations[3][0];


				var shuffleFormations = newFormations[i].SubBlob(1, 6).Chunk(1); // get formations 2-8
				shuffleFormations.Shuffle(rng);
				if (shuffleFormations.Contains(WarMech)) //preserve WarMech's formation 7 status
				{
					shuffleFormations.Swap(shuffleFormations.IndexOf(WarMech), 4);
				}
				for (int j = 2; j < 8; j++)
				{
					newFormations[i][j] = shuffleFormations[j - 2][0];
				}
				
			}
			Put(FormationFrequencyOffset, newFormations.SelectMany(formation => formation.ToBytes()).ToArray());
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

			const int Pirate = 15;
			const int WarMech = 118;
			const int Lich = 119;
			const int Lich2 = 120;
			const int Kary = 121;
			const int Kary2 = 122;
			const int Kraken = 123;
			const int Kraken2 = 124;
			const int Tiamat = 125;
			const int Tiamat2 = 126;
			const int Chaos = 127;
			var oldBosses = new List<Blob>
			{
				oldEnemies[Lich],
				oldEnemies[Kary],
				oldEnemies[Kraken],
				oldEnemies[Tiamat]
			};
			oldBosses.Shuffle(rng);

			newEnemies[Lich][7] = oldBosses[0][7];
			newEnemies[Kary][7] = oldBosses[1][7];
			newEnemies[Kraken][7] = oldBosses[2][7];
			newEnemies[Tiamat][7] = oldBosses[3][7];

			var oldBigBosses = new List<Blob>
			{
				oldEnemies[WarMech],
				oldEnemies[Lich2],
				oldEnemies[Kary2],
				oldEnemies[Kraken2],
				oldEnemies[Tiamat2],
				oldEnemies[Chaos]
			};
			oldBigBosses.Shuffle(rng);

			newEnemies[WarMech][7] = oldBigBosses[0][7];
			newEnemies[Lich2][7] = oldBigBosses[1][7];
			newEnemies[Kary2][7] = oldBigBosses[2][7];
			newEnemies[Kraken2][7] = oldBigBosses[3][7];
			newEnemies[Tiamat2][7] = oldBigBosses[4][7];
			newEnemies[Chaos][7] = oldBigBosses[5][7];

			if (!AllowUnsafePirates)
			{
				if (newEnemies[Pirate][7] < 0xFF)
				{
					int swapEnemy = newEnemies.IndexOf(newEnemies.First((enemy) => enemy[7] == 0xFF));
					newEnemies[swapEnemy][7] = newEnemies[Pirate][7];
					newEnemies[Pirate][7] = 0xFF;
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
