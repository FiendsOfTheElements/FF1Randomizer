using System.Collections.Generic;
using System.Linq;
using RomUtilities;
using System.ComponentModel;
using System;

namespace FF1Lib
{
	public enum LockHitMode
	{
		[Description("Vanilla")]
		Vanilla = 0,
		[Description("107 Accuracy")]
		Accuracy107,
		[Description("162 Accuracy")]
		Accuracy162,
		[Description("Auto Hit")]
		AutoHit
	}

	public enum AutohitThreshold
	{
		[Description("300")]
		Vanilla = 0,
		[Description("600")]
		Autohit600,
		[Description("900")]
		Autohit900,
		[Description("1200")]
		Autohit1200,
		[Description("Unlimited")]
		Autohit65535,
		[Description("300 or 600")]
		Autohit300to600,
		[Description("300, 600, or 900")]
		Autohit300to900,
		[Description("300, 600, 900, or 1200")]
		Autohit300to1200,
		[Description("Any of the above")]
		Any,
	}

	public struct MagicSpell
	{
		public byte Index;
		public Blob Data;
		public string Name;
		public byte TextPointer;

		public override string ToString()
		{
			return Index.ToString() + ": " + Name;
		}
	}

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
		public const int oldKnightNinjaMaxMPOffset = 0x6C907;
		public const int newKnightNinjaMaxMPOffset = 0x6D344;

		public const int ConfusedSpellIndexOffset = 0x3321E;
		public const int FireSpellIndex = 4;

		public const int WeaponOffset = 0x30000;
		public const int WeaponSize = 8;
		public const int WeaponCount = 40;

		public const int ArmorOffset = 0x30140;
		public const int ArmorSize = 4;
		public const int ArmorCount = 40;


		public void BuffHealingSpells()
		{
			// improves CURE and HEAL spells both in and out of battle
			// this is also done by Spellcrafter
			// CURE
			Put(MagicOffset + 1, new byte[] { 0x20 }); // replace CURE effectivity with 32 (was 16)
			Put(0x3AF5F, Blob.FromHex("1F0920")); // changing the oob code for CURE to reflect new values
			// CUR2
			Put(MagicOffset + MagicSize * 16 + 1, new byte[] { 0x40 }); // replace CUR2 effectivity with 64 (was 32)
			Put(0x3AF67, Blob.FromHex("3F0940")); // changing the oob code for CUR2 to reflect the above effect
			// CUR3
			Put(MagicOffset + MagicSize * 32 + 1, new byte[] { 0x80 }); // replace CUR3 effectivity with 128 (was 64)
			Put(0x3AF6F, Blob.FromHex("7F0980")); // changing the oob code for CUR3 to reflect the above effect
			// HEAL
			Put(MagicOffset + MagicSize * 19 + 1, new byte[] { 0x10 }); // replace HEAL effectivity with 16 (was 12)
			Put(0x3AFDF, Blob.FromHex("0F")); // changing the oob code for HEAL to reflect the above effect
			// HEL2
			Put(MagicOffset + MagicSize * 35 + 1, new byte[] { 0x20 }); // replace HEL2 effectivity with 32 (was 24)
			Put(0x3AFE8, Blob.FromHex("1F")); // changing the oob code for HEL2 to reflect the above effect
			// HEL3
			Put(MagicOffset + MagicSize * 51 + 1, new byte[] { 0x40 }); // replace HEL2 effectivity with 64 (was 48)
			Put(0x3AFF1, Blob.FromHex("3F")); // changing the oob code for HEL3 to reflect the above effect
			// LAMP
			Put(MagicOffset + MagicSize * 8 + 1, new byte[] { 0x18 }); // LAMP heals paralysis as well as darkness
			// AMUT
			Put(MagicOffset + MagicSize * 27 + 1, new byte[] { 0x50 }); // AMUT heals paralysis as well as silence
		}

		public void ShuffleMagicLevels(MT19337 rng, bool keepPermissions, bool tieredShuffle, bool mixSpellbooks, bool noSpellcrafter)
		{
			var magicSpells = GetSpells();
			if(tieredShuffle && noSpellcrafter)
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
					// 70% chance of tier 7-8, 25% chance of tier 4-6, 5% chance of tier 1-3
					int diceRoll = rng.Between(0, 19);
					if(diceRoll < 14)
					{
						whiteSpellFinalList[2].Add(spell);
					}
					else if (diceRoll < 19)
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
					// 60% chance of tier 4-6, 25% chance of tier 1-3, 15% chance of tier 7-8
					// if a section of the final list is full, move to another section
					int diceRoll = rng.Between(0, 19);
					if(diceRoll < 12)
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
					else if (diceRoll < 17)
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
						// 70% chance of tier 7-8, 25% chance of tier 4-6, 5% chance of tier 1-3
						int diceRoll = rng.Between(0, 19);
						if (diceRoll < 14)
						{
							blackSpellFinalList[2].Add(spell);
						}
						else if (diceRoll < 19)
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
						// 60% chance of tier 4-6, 25% chance of tier 1-3, 15% chance of tier 7-8
						// if a section of the final list is full, move to another section
						int diceRoll = rng.Between(0, 19);
						if (diceRoll < 12)
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
						else if (diceRoll < 17)
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
					var mergedList = magicSpells.ToList();
					mergedList.Shuffle(rng);
					whiteSpells = mergedList.Where((spell, i) => (i / 4) % 2 == 0).ToList();
					blackSpells = mergedList.Where((spell, i) => (i / 4) % 2 == 1).ToList();
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
			PutSpellNames(shuffledSpells);
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

		public void SetMPMax(int redMageMaxMP, int whiteMageMaxMP, int blackMageMaxMP, int knightMaxMP, int ninjaMaxMP)
		{
			const int lut_MaxMP = 0x6C902;

			Put(lut_MaxMP, new List<byte> { 0x00, 0x00, 0x00, (byte)redMageMaxMP, (byte)whiteMageMaxMP, (byte)blackMageMaxMP,
				(byte)knightMaxMP, (byte)ninjaMaxMP, 0x00, (byte)redMageMaxMP, (byte)whiteMageMaxMP, (byte)blackMageMaxMP }.ToArray());
		}

		public void SetClassMaxMp(int classIndex, int maxMp)
		{
			//49 levels per class, 2 bytes
			//spell data is packed into a byte, bit index = spell level gained that level
			//0 0 0 0 1 0 0 0 would mean gaining a level 4 spell slot that level

			//brute force way... count the number of spells and rewrite it to not gain any more after the max
			List<int> spellCount = new List<int> { 2, 0, 0, 0, 0, 0, 0, 0 };
			for (int i = 0; i < 49; i++)
			{
				int currentOffset = NewLevelUpDataOffset + (49 * classIndex * 2) + (i * 2) + 1;
				byte currentSpellData = Get(currentOffset, 1)[0];

				for (int bitTest = 0; bitTest < 8; bitTest++)
				{
					if ((currentSpellData & (1 << bitTest)) != 0)
					{
						spellCount[bitTest]++;
					}

					if (spellCount[bitTest] > maxMp)
					{
						currentSpellData = (byte)(currentSpellData & ~(1 << bitTest));
					}
				}

				Put(currentOffset, new byte[] { currentSpellData });
			}
		}

		public void ChangeLockMode(LockHitMode lockHitMode)
		{
			if (lockHitMode == LockHitMode.Accuracy107)
			{
				Put(MagicOffset + (MagicSize * 6), new byte[] { 107 });
				Put(MagicOffset + (MagicSize * 23), new byte[] { 107 });
			}
			else if (lockHitMode == LockHitMode.Accuracy162)
			{
				Put(MagicOffset + (MagicSize * 6), new byte[] { 162 });
				Put(MagicOffset + (MagicSize * 23), new byte[] { 162 });
			}
			else if (lockHitMode == LockHitMode.AutoHit)
			{
				PutInBank(0x0C, 0xBA46, Blob.FromHex("2029B9AD856838ED7468B002A9008D85682085B860EAEAEAEAEAEAEAEAEAEAEAEAEA"));
			}
		}

		public void UpdateMagicAutohitThreshold(MT19337 rng, AutohitThreshold threshold)
		{
			short limit = 300;
			switch (threshold)
			{
				case AutohitThreshold.Vanilla: limit = 300; break;
				case AutohitThreshold.Autohit600: limit = 600; break;
				case AutohitThreshold.Autohit900: limit = 900; break;
				case AutohitThreshold.Autohit1200: limit = 1200; break;
				case AutohitThreshold.Autohit65535: limit = short.MaxValue; break;
				case AutohitThreshold.Autohit300to600: limit = (short)(rng.Between(1, 2) * 300); break;
				case AutohitThreshold.Autohit300to900: limit = (short)(rng.Between(1, 3) * 300); break;
				case AutohitThreshold.Autohit300to1200: limit = (short)(rng.Between(1, 4) * 300); break;
				case AutohitThreshold.Any:
				{
					short[] any = { 300, 600, 900, 1200, short.MaxValue };
					limit = any.PickRandom(rng);
					break;
				}
			}

			// Set the low and high bytes of the limit which are then loaded and compared to the targets hp.
			Data[0x33AE0] = (byte)(limit & 0x00ff);
			Data[0x33AE5] = (byte)((limit >> 8) & 0x00ff);
		}

		public List<MagicSpell> GetSpells() {
			var spells = Get(MagicOffset, MagicSize * MagicCount).Chunk(MagicSize);
			var itemnames = ReadText(FF1Rom.ItemTextPointerOffset, FF1Rom.ItemTextPointerBase, FF1Rom.ItemTextPointerCount);
			var pointers = Get(MagicTextPointersOffset, MagicCount);

			return spells.Select((spell, i) => new MagicSpell
			{
				Index = (byte)i,
				Data = spell,
				Name = itemnames[176 + i],
				TextPointer = pointers[i]
			})
			.ToList();
		}

		public void PutSpellNames(List<MagicSpell> spells)
		{
			var itemnames = ReadText(FF1Rom.ItemTextPointerOffset, FF1Rom.ItemTextPointerBase, FF1Rom.ItemTextPointerCount);

			for(int i = 0; i < spells.Count; i++)
			{
				itemnames[176 + i] = spells[i].Name;
			}

			WriteText(itemnames, FF1Rom.ItemTextPointerOffset, FF1Rom.ItemTextPointerBase, FF1Rom.ItemTextOffset);
		}

		public void AccessibleSpellNames(Flags flags)
		{
			// If Spellcrafter mode is on, abort. We need a check here as the setting on the site can be in a random state.
			if ((bool)flags.GenerateNewSpellbook)
			{
				return;
			}

			var magicSpells = GetSpells();

			// Since this can be performed independent of the magic shuffling, we can't assume the location of spell names.
			// We will loop through the spell list and replace the appropriate names as we find them.
			for (int i = 0; i < magicSpells.Count; i++)
			{
				MagicSpell newSpell = magicSpells[i];
				string spellName = magicSpells[i].Name;

				switch (spellName)
				{
					// Note that 3 letter spell names actually have a trailing space
					case "LIT ":
						newSpell.Name = "THUN";
						break;
					case "LIT2":
						newSpell.Name = "THN2";
						break;
					case "LIT3":
						newSpell.Name = "THN3";
						break;
					case "FAST":
						newSpell.Name = "HAST";
						break;
					case "SLEP":
						newSpell.Name = "DOZE";
						break;
					case "SLP2":
						newSpell.Name = "DOZ2";
						break;

					case "HARM":
						newSpell.Name = "DIA ";
						break;
					case "HRM2":
						newSpell.Name = "DIA2";
						break;
					case "HRM3":
						newSpell.Name = "DIA3";
						break;
					case "HRM4":
						newSpell.Name = "DIA4";
						break;
					case "ALIT":
						newSpell.Name = "ATHN";
						break;
					case "AMUT":
						newSpell.Name = "VOX ";
						break;
					case "FOG ":
						newSpell.Name = "PROT";
						break;
					case "FOG2":
						newSpell.Name = "PRO2";
						break;
					case "FADE":
						newSpell.Name = "HOLY";
						break;
				}

				// Update the entry in the list
				magicSpells[i] = newSpell;
			}

			// Now update the spell names!
			PutSpellNames(magicSpells);
		}

		public void MixUpSpellNames(SpellNameMadness mode, MT19337 rng)
		{
			if (mode == SpellNameMadness.MixedUp)
			{
				var itemnames = ReadText(FF1Rom.ItemTextPointerOffset, FF1Rom.ItemTextPointerBase, FF1Rom.ItemTextPointerCount);

				string[] spellnames = new string[64];
				Array.Copy(itemnames, 176, spellnames, 0, 64);

				var spellnamelist = new List<string>(spellnames);
				spellnamelist.Shuffle(rng);

				for (int i = 0; i < spellnamelist.Count; i++)
				{
					itemnames[176 + i] = spellnamelist[i];
				}

				WriteText(itemnames, FF1Rom.ItemTextPointerOffset, FF1Rom.ItemTextPointerBase, FF1Rom.ItemTextOffset);
			}
			else if (mode == SpellNameMadness.Madness)
			{
				List<string> alphabet = new List<string> { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z" };
				List<string> numbers = new List<string> { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9", "0" };

				var itemnames = ReadText(FF1Rom.ItemTextPointerOffset, FF1Rom.ItemTextPointerBase, FF1Rom.ItemTextPointerCount);

				for (int i = 176; i < 176 + 64; i++)
				{
					itemnames[i] = alphabet.PickRandom(rng) + alphabet.PickRandom(rng) + numbers.PickRandom(rng) + alphabet.PickRandom(rng);
				}

				WriteText(itemnames, FF1Rom.ItemTextPointerOffset, FF1Rom.ItemTextPointerBase, FF1Rom.ItemTextOffset);
			}
		}
	}

	public enum SpellNameMadness
	{
		[Description("None")]
		None,

		[Description("MixedUp")]
		MixedUp,

		[Description("Madness")]
		Madness
	}
}
