using FF1Lib.Helpers;
using Newtonsoft.Json;
using RomUtilities;
using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static FF1Lib.FF1Rom;

namespace FF1Lib
{
	[JsonObject(MemberSerialization.OptIn)]
	public class EnemyScriptInfo
	{
		private const int ScriptSize = 16;

		[JsonProperty]
		public byte index;

		[JsonProperty]
		public byte spell_chance;

		[JsonProperty]
		public byte skill_chance;

		public byte[] spell_list = new byte[8];
		public byte[] skill_list = new byte[4];

		public List<MagicSpell> allGameSpells;
		public List<MagicSpell> allEnemySkills;

		[JsonProperty]
		List<string> SpellList
		{
			get
			{
				return spell_list.Where(s => s != 0xff).Select(s => this.allGameSpells[s].Name).ToList();
			}
			set
			{
			}
		}

		[JsonProperty]
		List<string> SkillList
		{
			get
			{
				return skill_list.Where(s => s != 0xff).Select(s => this.allEnemySkills[s].Name).ToList();
			}
			set
			{
			}
		}
		public EnemyScriptInfo(byte[] scriptdata)
		{
			decompressData(scriptdata);
		}
		public byte[] GetArray()
		{
			return compressData();
		}

		public byte[] compressData()
		{
			byte[] scriptData = new byte[16];
			scriptData[0] = spell_chance;
			scriptData[1] = skill_chance;
			scriptData[2] = spell_list[0];
			scriptData[3] = spell_list[1];
			scriptData[4] = spell_list[2];
			scriptData[5] = spell_list[3];
			scriptData[6] = spell_list[4];
			scriptData[7] = spell_list[5];
			scriptData[8] = spell_list[6];
			scriptData[9] = spell_list[7];
			scriptData[10] = 0xFF;
			scriptData[11] = skill_list[0];
			scriptData[12] = skill_list[1];
			scriptData[13] = skill_list[2];
			scriptData[14] = skill_list[3];
			scriptData[15] = 0xFF;
			return scriptData;
		}

		public void writeData(FF1Rom rom)
		{
			var d = compressData();
			//rom.Put(ScriptOffset + index * ScriptSize, d);
		}

		public void decompressData(byte[] data)
		{
			if (data.Length != ScriptSize)
				return; // don't do anything if this is not a valid script
			spell_chance = data[0];
			skill_chance = data[1];
			spell_list[0] = data[2];
			spell_list[1] = data[3];
			spell_list[2] = data[4];
			spell_list[3] = data[5];
			spell_list[4] = data[6];
			spell_list[5] = data[7];
			spell_list[6] = data[8];
			spell_list[7] = data[9];
			skill_list[0] = data[11];
			skill_list[1] = data[12];
			skill_list[2] = data[13];
			skill_list[3] = data[14];
		}
	}

	public class EnemyScripts
	{
		private const int ScriptOffset = 0x31020;
		private const int ScriptSize = 16;
		private const int ScriptCount = 44;

		private List<EnemyScriptInfo> scripts;
		public Dictionary<SpellByte, SpellByte> SwitchedSpells { get; private set; }

		public EnemyScriptInfo this[int index]
		{
			get => scripts[index];
			set => scripts[index] = value;
		}
		public int Count()
		{
			return scripts.Count;
		}
		public List<EnemyScriptInfo> GetList()
		{
			return scripts;
		}
		public EnemyScripts(FF1Rom rom)
		{
			scripts = rom.Get(ScriptOffset, ScriptSize * ScriptCount).Chunk(ScriptSize).Select(s => new EnemyScriptInfo(s)).ToList();
			SwitchedSpells = Enum.GetValues<SpellByte>().ToDictionary(s => s, s => s);
		}

		public void Write(FF1Rom rom)
		{
			rom.Put(ScriptOffset, scripts.SelectMany(s => s.GetArray()).ToArray());
		}
		public void UpdateSpellsIndices(Dictionary<SpellByte, SpellByte> switchedspells)
		{
			SwitchedSpells = switchedspells;
			if (!SwitchedSpells.ContainsKey(SpellByte.NONE))
			{
				SwitchedSpells.Add(SpellByte.NONE, SpellByte.NONE);
			}
			SwitchSpells();
		}
		public void ImportVanillaSpellList(int scriptindex, List<byte> spellList)
		{
			scripts[scriptindex].spell_list = spellList.Select(s => (byte)SwitchedSpells[(SpellByte)s]).ToArray();
		}
		private void SwitchSpells()
		{
			foreach (var script in scripts)
			{
				script.spell_list = script.spell_list.Select(s => (byte)SwitchedSpells[(SpellByte)s]).ToArray();
			}
		}
		/// <summary>
		/// Returns index of first occurrence where an enemy AI script has NONE for all spells and skills
		/// Returns -1 if none exist
		/// </summary>
		/// <returns>int</returns>
		public int SearchForNoSpellNoAbilityEnemyScript()
		{
			int firstResult = -1;

			for (int i = 0; i < scripts.Count; i++)
			{
				if (!scripts[i].spell_list.Where(s => s != 0xFF).Any() && !scripts[i].skill_list.Where(s => s != 0xFF).Any())
				{
					firstResult = i;
					break;
				}
			}
			return firstResult;
		}
		public void ShuffleEnemySkillsSpells(FF1Rom rom, MT19337 rng, Flags flags)
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
				rom.GenerateBalancedEnemyScripts(this, rng, buffedPirates);
			}
			// Remove two instances each of CRACK and TOXIC since they're likely to get spread out to several enemies.
			const int SandW = 7; // CRACK
			const int IronGol = 27; // TOXIC
			scripts[SandW].skill_list[2] = 0xFF;
			scripts[SandW].skill_list[3] = 0xFF;
			scripts[IronGol].skill_list[2] = 0xFF;
			scripts[IronGol].skill_list[3] = 0xFF;

			var normalIndices = Enumerable.Range(0, 32).Concat(new[] { 33, 43 }).ToList();
			var bossIndices = new List<int> { 34, 36, 38, 40 };
			var bigBossIndices = new List<int> { 32, 35, 37, 39, 41, 42 };

			if (shuffleNormalEnemies)
			{
				ShuffleIndexedSkillsSpells(rom, normalIndices, noConsecutiveNukes, nonEmpty, rng);
			}

			if (shuffleBosses)
			{
				ShuffleIndexedSkillsSpells(rom, bossIndices, noConsecutiveNukes, false, rng);
				ShuffleIndexedSkillsSpells(rom, bigBossIndices, noConsecutiveNukes, false, rng);
			}

			//Put(ScriptOffset, scriptBytes.SelectMany(script => script.ToBytes()).ToArray());
		}

		private void ShuffleIndexedSkillsSpells(FF1Rom rom, List<int> indices, bool noConsecutiveNukes, bool nonEmpty, MT19337 rng)
		{
			var validscripts = indices.Select(i => new EnemyScriptInfo(scripts[i].GetArray())).ToList();

			var spellBytes = indices.Select(i => scripts[i]).SelectMany(script => script.spell_list).Where(b => b != 0xFF).ToList();
			var skillBytes = indices.Select(i => scripts[i]).SelectMany(script => script.skill_list).Where(b => b != 0xFF).ToList();

			List<List<byte>> spellBuckets;
			List<List<byte>> skillBuckets;

			// Spellcrafter compatability, search for
			// nuke/nuclear-equivalent spells (AoE
			// non-elemental damage with a base damage of
			// 80 or greater).  In the normal game this
			// matches NUKE and FADE -- in the normal
			// game, no monsters cast FADE, but with spell
			// crafter, anything can happen.
			var nukes = new SpellHelper(rom).FindSpells(SpellRoutine.Damage, SpellTargeting.AllEnemies, SpellElement.None, SpellStatus.Any).Where(
				spell => spell.Item2.effect >= 80).Select(n => ((byte)n.Item1 - (byte)Spell.CURE)).ToList();

			var reroll = false;
			do
			{
				reroll = false;
				spellBytes.Shuffle(rng);
				skillBytes.Shuffle(rng);
				spellBuckets = Bucketize(spellBytes, validscripts.Count(script => script.spell_chance != 0), 8, nonEmpty, rng);
				skillBuckets = Bucketize(skillBytes, validscripts.Count(script => script.skill_chance != 0), 4, false, rng);

				var spellChances = validscripts.Select(script => script.spell_chance).ToList();
				var skillChances = validscripts.Select(script => script.skill_chance).ToList();

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
				for (int i = 0; i < validscripts.Count; i++)
				{
					var script = validscripts[i];
					script.spell_chance = spellChances[i];
					script.skill_chance = skillChances[i];

					for (int j = 0; j < 8; j++)
					{
						script.spell_list[j] = 0xFF;
					}
					for (int j = 0; j < 4; j++)
					{
						script.skill_list[j] = 0xFF;
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
							if (nukes.Contains(spellBuckets[spellBucketIndex][j]))
							{
								if (previousWasNuke)
								{
									reroll = true;
								}
								else
								{
									previousWasNuke = true;
								}
							}
							else
							{
								previousWasNuke = false;
							}
							script.spell_list[j] = spellBuckets[spellBucketIndex][j];
						}
						spellBucketIndex++;
					}
					if (skillChances[i] != 0)
					{
						startingNuclear = (skillBuckets[skillBucketIndex][0] == (byte)EnemySkills.Nuclear);
						bool previousWasNuclear = false;
						for (int j = 0; j < skillBuckets[skillBucketIndex].Count; j++)
						{
							if (skillBuckets[skillBucketIndex][j] == (byte)EnemySkills.Nuclear)
							{
								if (previousWasNuclear)
								{
									reroll = true;
								}
								else
								{
									previousWasNuclear = true;
								}
							}
							else
							{
								previousWasNuclear = false;
							}
							script.skill_list[j] = skillBuckets[skillBucketIndex][j];
						}
						skillBucketIndex++;
					}
					if (startingNuke && startingNuclear)
					{
						reroll = true;
					}
				}
			} while (noConsecutiveNukes && reroll);

			for (int i = 0; i < validscripts.Count; i++)
			{
				scripts[indices[i]] = validscripts[i];
			}
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

	}



}
