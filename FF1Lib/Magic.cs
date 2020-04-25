using System.Collections.Generic;
using System.Linq;
using RomUtilities;

namespace FF1Lib
{
	public partial class FF1Rom : NesRom
	{
		public const int MagicOffset = 0x301E0;
		public const int MagicSize = 8;
		public const int MagicCount = 64;
		public const int MagicNamesOffset = 0x2BE03;
		public const int MagicNameSize = 5;
		public const int MagicTextPointersOffset = 0x304C0;
		public const int MagicPermissionsOffset = 0x3AD18;
		public const int MagicPermissionsSize = 8;
		public const int MagicPermissionsCount = 12;
		public const int MagicOutOfBattleOffset = 0x3AEFA;
		public const int MagicOutOfBattleSize = 7;
		public const int MagicOutOfBattleCount = 13;

		public const int OldLevelUpDataOffset = 0x2D094; // this was moved to bank 1B
		public const int NewLevelUpDataOffset = 0x6CDA9; // this was moved from bank 1B

		public const int ConfusedSpellIndexOffset = 0x3321E;
		public const int FireSpellIndex = 4;

		public const int WeaponOffset = 0x30000;
		public const int WeaponSize = 8;
		public const int WeaponCount = 40;

		public const int ArmorOffset = 0x30140;
		public const int ArmorSize = 4;
		public const int ArmorCount = 40;

		private struct MagicSpell
		{
			public byte Index;
			public Blob Data;
			public Blob Name;
			public byte TextPointer;
		}

		public void ShuffleMagicLevels(MT19337 rng, bool keepPermissions, bool tieredShuffle, bool mixSpellbooks)
		{
			var magicSpells = GetSpells();
			if(tieredShuffle)
			{
				// if we are doing a tiered shuffle, swap the position of TMPR and SABR before further shuffling for balance purposes
				MagicSpell tmpTMPR = magicSpells[14];
				magicSpells[14] = magicSpells[54];
				magicSpells[54] = tmpTMPR;
			}

			// First we have to un-interleave white and black spells.
			var whiteSpells = magicSpells.Where((spell, i) => (i / 4) % 2 == 0).ToList();
			var blackSpells = magicSpells.Where((spell, i) => (i / 4) % 2 == 1).ToList();

			if(tieredShuffle)
			{
				// weigh spell probability of landing in a tier based on where it was in the original game
				var whiteSpellList = new List<MagicSpell>[3];
				var blackSpellList = new List<MagicSpell>[3];
				var whiteSpellFinalList = new List<MagicSpell>[3];
				var blackSpellFinalList = new List<MagicSpell>[3];
				int mergedSpellDoubler = 1;
				whiteSpellList[0] = magicSpells.Where((spell, i) => (i / 4) % 2 == 0 && i < 24).ToList();
				whiteSpellList[1] = magicSpells.Where((spell, i) => (i / 4) % 2 == 0 && i < 48 && i >= 24).ToList();
				whiteSpellList[2] = magicSpells.Where((spell, i) => (i / 4) % 2 == 0 && i >= 48).ToList();
				blackSpellList[0] = magicSpells.Where((spell, i) => (i / 4) % 2 == 1 && i < 24).ToList();
				blackSpellList[1] = magicSpells.Where((spell, i) => (i / 4) % 2 == 1 && i < 48 && i >= 24).ToList();
				blackSpellList[2] = magicSpells.Where((spell, i) => (i / 4) % 2 == 1 && i >= 48).ToList();
				if(mixSpellbooks)
				{
					whiteSpellList[0] = whiteSpellList[0].Concat(blackSpellList[0]).ToList();
					whiteSpellList[1] = whiteSpellList[1].Concat(blackSpellList[1]).ToList();
					whiteSpellList[2] = whiteSpellList[2].Concat(blackSpellList[2]).ToList();
					mergedSpellDoubler = 2;
				}
				whiteSpellFinalList[0] = new List<MagicSpell> { };
				whiteSpellFinalList[1] = new List<MagicSpell> { };
				whiteSpellFinalList[2] = new List<MagicSpell> { };
				blackSpellFinalList[0] = new List<MagicSpell> { };
				blackSpellFinalList[1] = new List<MagicSpell> { };
				blackSpellFinalList[2] = new List<MagicSpell> { };
				whiteSpells.Clear();
				blackSpells.Clear();
				foreach (MagicSpell spell in whiteSpellList[2])
				{
					// 60% chance of tier 7-8, 30% chance of tier 4-6, 10% chance of tier 1-3
					int diceRoll = rng.Between(0, 9);
					if(diceRoll < 6)
					{
						whiteSpellFinalList[2].Add(spell);
					}
					else if (diceRoll < 9)
					{
						whiteSpellFinalList[1].Add(spell);
					}
					else
					{
						whiteSpellFinalList[0].Add(spell);
					}
				}
				foreach (MagicSpell spell in whiteSpellList[1])
				{
					// 60% chance of tier 4-6, 20% chance of tier 1-3, 20% chance of tier 7-8
					// if a section of the final list is full, move to another section
					int diceRoll = rng.Between(0, 9);
					if(diceRoll < 6)
					{
						if(whiteSpellFinalList[1].Count >= 12 * mergedSpellDoubler)
						{
							if(whiteSpellFinalList[0].Count >= 12 * mergedSpellDoubler)
							{
								whiteSpellFinalList[2].Add(spell);
							}
							else
							{
								whiteSpellFinalList[0].Add(spell);
							}
						}
						else
						{
							whiteSpellFinalList[1].Add(spell);
						}
					}
					else if (diceRoll < 8)
					{
						if(whiteSpellFinalList[0].Count >= 12 * mergedSpellDoubler)
						{
							if(whiteSpellFinalList[1].Count >= 12 * mergedSpellDoubler)
							{
								whiteSpellFinalList[2].Add(spell);
							}
							else
							{
								whiteSpellFinalList[1].Add(spell);
							}
						}
						else
						{
							whiteSpellFinalList[0].Add(spell);
						}
					}
					else
					{
						if(whiteSpellFinalList[2].Count >= 8 * mergedSpellDoubler)
						{
							if(whiteSpellFinalList[1].Count >= 12 * mergedSpellDoubler)
							{
								whiteSpellFinalList[0].Add(spell);
							}
							else
							{
								whiteSpellFinalList[1].Add(spell);
							}
						}
						else
						{
							whiteSpellFinalList[2].Add(spell);
						}
					}
				}
				foreach(MagicSpell spell in whiteSpellList[0])
				{
					// fill the remaining tiers with the tier 1-3 base magic
					if(whiteSpellFinalList[0].Count >= 12 * mergedSpellDoubler)
					{
						if(whiteSpellFinalList[1].Count >= 12 * mergedSpellDoubler)
						{
							whiteSpellFinalList[2].Add(spell);
						}
						else
						{
							whiteSpellFinalList[1].Add(spell);
						}
					}
					else
					{
						whiteSpellFinalList[0].Add(spell);
					}
				}
				// and repeat the process for black magic if we didn't mix spellbooks
				if(mixSpellbooks)
				{
					// if we mixed spellbooks, split the white (merged) spellbook in halves to set the black spell list
					blackSpellFinalList[0] = whiteSpellFinalList[0].Take(12).ToList();
					whiteSpellFinalList[0] = whiteSpellFinalList[0].Except(blackSpellFinalList[0]).ToList();
					blackSpellFinalList[1] = whiteSpellFinalList[1].Take(12).ToList();
					whiteSpellFinalList[1] = whiteSpellFinalList[1].Except(blackSpellFinalList[1]).ToList();
					blackSpellFinalList[2] = whiteSpellFinalList[2].Take(8).ToList();
					whiteSpellFinalList[2] = whiteSpellFinalList[2].Except(blackSpellFinalList[2]).ToList();
				}
				else
				{
					foreach (MagicSpell spell in blackSpellList[2])
					{
						// 60% chance of tier 7-8, 30% chance of tier 4-6, 10% chance of tier 1-3
						int diceRoll = rng.Between(0, 9);
						if (diceRoll < 6)
						{
							blackSpellFinalList[2].Add(spell);
						}
						else if (diceRoll < 9)
						{
							blackSpellFinalList[1].Add(spell);
						}
						else
						{
							blackSpellFinalList[0].Add(spell);
						}
					}
					foreach (MagicSpell spell in blackSpellList[1])
					{
						// 60% chance of tier 4-6, 20% chance of tier 1-3, 20% chance of tier 7-8
						// if a section of the final list is full, move to another section
						int diceRoll = rng.Between(0, 9);
						if (diceRoll < 6)
						{
							if (blackSpellFinalList[1].Count >= 12)
							{
								if (blackSpellFinalList[0].Count >= 12)
								{
									blackSpellFinalList[2].Add(spell);
								}
								else
								{
									blackSpellFinalList[0].Add(spell);
								}
							}
							else
							{
								blackSpellFinalList[1].Add(spell);
							}
						}
						else if (diceRoll < 8)
						{
							if (blackSpellFinalList[0].Count >= 12)
							{
								if (blackSpellFinalList[1].Count >= 12)
								{
									blackSpellFinalList[2].Add(spell);
								}
								else
								{
									blackSpellFinalList[1].Add(spell);
								}
							}
							else
							{
								blackSpellFinalList[0].Add(spell);
							}
						}
						else
						{
							if (blackSpellFinalList[2].Count >= 8)
							{
								if (blackSpellFinalList[1].Count >= 12)
								{
									blackSpellFinalList[0].Add(spell);
								}
								else
								{
									blackSpellFinalList[1].Add(spell);
								}
							}
							else
							{
								blackSpellFinalList[2].Add(spell);
							}
						}
					}
					foreach (MagicSpell spell in blackSpellList[0])
					{
						// fill the remaining tiers with the tier 1-3 base magic
						if (blackSpellFinalList[0].Count >= 12)
						{
							if (blackSpellFinalList[1].Count >= 12)
							{
								blackSpellFinalList[2].Add(spell);
							}
							else
							{
								blackSpellFinalList[1].Add(spell);
							}
						}
						else
						{
							blackSpellFinalList[0].Add(spell);
						}
					}
				}		
				// shuffle each of the final lists
				foreach(List<MagicSpell> spellList in whiteSpellFinalList)
				{
					spellList.Shuffle(rng);
				}
				if(!mixSpellbooks)
				{
					foreach (List<MagicSpell> spellList in blackSpellFinalList)
					{
						spellList.Shuffle(rng);
					}
				}
				// and append each in turn to the whitespells / blackspells list
				whiteSpells = whiteSpells.Concat(whiteSpellFinalList[0]).ToList();
				whiteSpells = whiteSpells.Concat(whiteSpellFinalList[1]).ToList();
				whiteSpells = whiteSpells.Concat(whiteSpellFinalList[2]).ToList();
				blackSpells = blackSpells.Concat(blackSpellFinalList[0]).ToList();
				blackSpells = blackSpells.Concat(blackSpellFinalList[1]).ToList();
				blackSpells = blackSpells.Concat(blackSpellFinalList[2]).ToList();
			}
			else
			{
				if(mixSpellbooks)
				{
					magicSpells.Shuffle(rng);
					whiteSpells = magicSpells.Where((spell, i) => (i / 4) % 2 == 0).ToList();
					blackSpells = magicSpells.Where((spell, i) => (i / 4) % 2 == 1).ToList();
				}
				else
				{
					whiteSpells.Shuffle(rng);
					blackSpells.Shuffle(rng);
				}
			}

			// Now we re-interleave the spells.
			var shuffledSpells = new List<MagicSpell>();
			for (int i = 0; i < MagicCount; i++)
			{
				var sourceIndex = 4 * (i / 8) + i % 4;
				if ((i / 4) % 2 == 0)
				{
					shuffledSpells.Add(whiteSpells[sourceIndex]);
				}
				else
				{
					shuffledSpells.Add(blackSpells[sourceIndex]);
				}
			}

			Put(MagicOffset, shuffledSpells.Select(spell => spell.Data).Aggregate((seed, next) => seed + next));
			Put(MagicNamesOffset, shuffledSpells.Select(spell => spell.Name).Aggregate((seed, next) => seed + next));
			Put(MagicTextPointersOffset, shuffledSpells.Select(spell => spell.TextPointer).ToArray());

			if (keepPermissions)
			{
				// Shuffle the permissions the same way the spells were shuffled.
				for (int c = 0; c < MagicPermissionsCount; c++)
				{
					var oldPermissions = Get(MagicPermissionsOffset + c * MagicPermissionsSize, MagicPermissionsSize);

					var newPermissions = new byte[MagicPermissionsSize];
					for (int i = 0; i < 8; i++)
					{
						for (int j = 0; j < 8; j++)
						{
							var oldIndex = shuffledSpells[8 * i + j].Index;
							var oldPermission = (oldPermissions[oldIndex / 8] & (0x80 >> oldIndex % 8)) >> (7 - oldIndex % 8);
							newPermissions[i] |= (byte)(oldPermission << (7 - j));
						}
					}

					Put(MagicPermissionsOffset + c * MagicPermissionsSize, newPermissions);
				}
			}

			// Map old indices to new indices.
			var newIndices = new byte[MagicCount];
			for (byte i = 0; i < MagicCount; i++)
			{
				newIndices[shuffledSpells[i].Index] = i;
			}

			// Fix enemy spell pointers to point to where the spells are now.
			var scripts = Get(ScriptOffset, ScriptSize * ScriptCount).Chunk(ScriptSize);
			foreach (var script in scripts)
			{
				// Bytes 2-9 are magic spells.
				for (int i = 2; i < 10; i++)
				{
					if (script[i] != 0xFF)
					{
						script[i] = newIndices[script[i]];
					}
				}
			}
			Put(ScriptOffset, scripts.SelectMany(script => script.ToBytes()).ToArray());

			// Fix weapon and armor spell pointers to point to where the spells are now.
			var weapons = Get(WeaponOffset, WeaponSize * WeaponCount).Chunk(WeaponSize);
			foreach (var weapon in weapons)
			{
				if (weapon[3] != 0x00)
				{
					weapon[3] = (byte)(newIndices[weapon[3] - 1] + 1);
				}
			}
			Put(WeaponOffset, weapons.SelectMany(weapon => weapon.ToBytes()).ToArray());

			var armors = Get(ArmorOffset, ArmorSize * ArmorCount).Chunk(ArmorSize);
			foreach (var armor in armors)
			{
				if (armor[3] != 0x00)
				{
					armor[3] = (byte)(newIndices[armor[3] - 1] + 1);
				}
			}
			Put(ArmorOffset, armors.SelectMany(armor => armor.ToBytes()).ToArray());

			// Fix the crazy out of battle spell system.
			var outOfBattleSpellOffset = MagicOutOfBattleOffset;
			for (int i = 0; i < MagicOutOfBattleCount; i++)
			{
				var oldSpellIndex = Data[outOfBattleSpellOffset] - 0xB0;
				var newSpellIndex = newIndices[oldSpellIndex];

				Put(outOfBattleSpellOffset, new[] { (byte)(newSpellIndex + 0xB0) });

				outOfBattleSpellOffset += MagicOutOfBattleSize;
			}

			// Confused enemies are supposed to cast FIRE, so figure out where FIRE ended up.
			var newFireSpellIndex = shuffledSpells.FindIndex(spell => spell.Data == magicSpells[FireSpellIndex].Data);
			Put(ConfusedSpellIndexOffset, new[] { (byte)newFireSpellIndex });
		}

		List<MagicSpell> GetSpells() {
			var spells = Get(MagicOffset, MagicSize * MagicCount).Chunk(MagicSize);
			var names = Get(MagicNamesOffset, MagicNameSize * MagicCount).Chunk(MagicNameSize);
			var pointers = Get(MagicTextPointersOffset, MagicCount);

			return spells.Select((spell, i) => new MagicSpell
			{
				Index = (byte)i,
				Data = spell,
				Name = names[i],
				TextPointer = pointers[i]
			})
			.ToList();
		}
	}
}
