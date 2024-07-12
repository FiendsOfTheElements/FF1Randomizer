using System.Threading;

namespace FF1Lib
{
	public partial class FF1Rom : NesRom
	{
		enum OOBSpellEffects
		{
			OOBSPELL_CURE,
			OOBSPELL_CUR2,
			OOBSPELL_CUR3,
			OOBSPELL_CUR4,
			OOBSPELL_HEAL,
			OOBSPELL_HEL3,
			OOBSPELL_HEL2,
			OOBSPELL_PURE,
			OOBSPELL_LIFE,
			OOBSPELL_LIF2,
			OOBSPELL_WARP,
			OOBSPELL_SOFT,
			OOBSPELL_EXIT,
			NUM_OOB_SPELLS
		}

		public void CraftNewSpellbook(EnemyScripts enemyScripts, Flags flags, MT19337 rng) // generates a new spellbook and does all necessary steps to ensure that the new spells are assigned where they need to be for the game to work properly
		{
			bool enableSpellcrafter = (bool)flags.GenerateNewSpellbook;
			bool mixWhiteBlack = (bool)flags.SpellcrafterMixSpells;
			LockHitMode lockMode = flags.LockMode;
			bool levelShuffle = (bool)flags.MagicLevels;
			bool keepPermissions = (bool)flags.SpellcrafterRetainPermissions;

			if (!enableSpellcrafter)
			{
				return;
			}

			bool WhiteSpell(int id) => mixWhiteBlack || id % 8 < 4;
			bool BlackSpell(int id) => mixWhiteBlack || id % 8 > 3;
			int SpellTier(int id) => id / 8;

			SpellInfo[] spell = new SpellInfo[MagicCount]; // the spells we are creating.  every 4 indices split between white magic and black magic.
			//EnemyScriptInfo[] script = new EnemyScriptInfo[ScriptCount]; // enemy scripts (we will need to modify these after the spell list has been created)
			string[] spellNames = new string[MagicCount + 1];
			byte[] spellMessages = new byte[MagicCount];
			byte[] spellPermissions = new byte[MagicPermissionsSize * MagicPermissionsCount]; // we store the permissions in a giant array
			int[] oldTiers = new int[MagicCount];
			for (int i = 0; i < MagicCount; ++i)
			{
				spell[i] = new SpellInfo();
				spell[i].decompressData(Get(MagicOffset + MagicSize * i, MagicSize));
				spell[i].calc_Enemy_SpellTier();
				oldTiers[i] = spell[i].tier;
				spell[i].decompressData(new byte[] {  }); // reset
				spellNames[i] = ""; // no name until we assign one
				spellMessages[i] = 0; // no message until we assign one
			}
			spellNames[MagicCount] = "";
			if (levelShuffle)
				spellNames[MagicCount] = "L";
			for (int i = 0; i < MagicPermissionsCount * MagicPermissionsSize; ++i)
				{
				if (i >= 24 && i < 28)
					spellPermissions[i] = 0x00; // red mage can learn all white and black magic between tiers 1 and 4 except WARP, EXIT, CUR4, and LIF2 (but many permissions are taken away from them)
				else if (i >= 32 && i < 38)
					spellPermissions[i] = 0x0F; // white mage can learn all white magic between tiers 1 and 6 except WARP, EXIT, CUR4, LIF2 and some tier 7 spell effects
				else if (i >= 40 && i < 46)
					spellPermissions[i] = 0xF0; // black mage can learn all black magic between tiers 1 and 6 except WARP, EXIT, CUR4, LIF2 and some tier 7 spell effects
				else if (i >= 48 && i < 51)
					spellPermissions[i] = 0x0F; // knight can learn all white magic between tiers 1 and 3 (their permissions will never be revoked)
				else if (i >= 56 && i < 60)
					spellPermissions[i] = 0xF0; // ninja can learn all black magic between tiers 1 and 4 (their permissions will never be revoked)
				else if (i >= 72 && i < 79)
					spellPermissions[i] = 0x00; // red wizard can learn all white and black magic up to tier 7 (but many permissions are taken from away from them)
				else if (i >= 80 && i < 88)
					spellPermissions[i] = 0x0F; // white wizard can learn all white magic spells
				else if (i >= 88)
					spellPermissions[i] = 0xF0; // black wizard can learn all black magic spells
				else
					spellPermissions[i] = 0XFF; // all others can not learn any magic (fighter, thief, black belt, master)
			}
				
			List<int> spellindex = Enumerable.Range(0, 64).ToList(); // a list of spell indexes (0-63)

			// roll the elementals first
			int[] elemspell = new int[6]; // roll two spells for each of the base elements; one low spell for the first two tiers, and another that can appear in tier 3-4 and is always multitarget
			elemspell[0] = 4; // this must be 4 because it is also used for the confusion effect
			spellindex.Remove(4);
			elemspell[1] = spellindex.Where(id => BlackSpell(id) && id > 15 && id < 32).ToList().PickRandom(rng);
			spellindex.Remove(elemspell[1]);
			SPCR_CraftDamageSpell(rng, spell[elemspell[0]], SpellTier(elemspell[0]), 0b00010000, false, false);
			SPCR_CraftDamageSpell(rng, spell[elemspell[1]], SpellTier(elemspell[1]), 0b00010000, false, true);
			for (int i = 2; i < 6; i += 2)
			{
				elemspell[i] = spellindex.Where(id => BlackSpell(id) && id < 16).ToList().PickRandom(rng);
				elemspell[i + 1] = spellindex.Where(id => BlackSpell(id) && id > 15 && id < 32).ToList().PickRandom(rng);
				SPCR_CraftDamageSpell(rng, spell[elemspell[i]], SpellTier(elemspell[i]), (byte)(0b00010000 << (i / 2)), false, false);
				SPCR_CraftDamageSpell(rng, spell[elemspell[i + 1]], SpellTier(elemspell[i + 1]), (byte)(0b00010000 << (i / 2)), false, true);
				spellindex.Remove(elemspell[i]);
				spellindex.Remove(elemspell[i + 1]);
			}

			// roll and craft all of the out of battle spells
			int selected = spellindex.Where(id => WhiteSpell(id) && id < 8).ToList().PickRandom(rng);
			spell[selected].targeting = 0x10;
			spell[selected].effect = 32;
			spell[selected].routine = 0x07;
			spell[selected].gfx = 0xC0;
			spell[selected].palette = 0x29;
			spellMessages[selected] = 0x01; // HP up!
			Put(MagicOutOfBattleOffset, new[] { (byte)(selected + 0xB0) });
			Put(0x3AF5F, Blob.FromHex("1F0920")); // changing the oob code for CURE to reflect the above effect
			spellindex.Remove(selected);
			selected = spellindex.Where(id => WhiteSpell(id) && id < 24 && id > 7).ToList().PickRandom(rng);
			spell[selected].targeting = 0x10;
			spell[selected].effect = 64;
			spell[selected].routine = 0x07;
			spell[selected].gfx = 0xC0;
			spell[selected].palette = 0x29;
			spellMessages[selected] = 0x01; // HP up!
			Put(MagicOutOfBattleOffset + MagicOutOfBattleSize, new[] { (byte)(selected + 0xB0) });
			Put(0x3AF67, Blob.FromHex("3F0940")); // changing the oob code for CUR2 to reflect the above effect
			spellindex.Remove(selected);
			selected = spellindex.Where(id => WhiteSpell(id) && id < 40 && id > 23).ToList().PickRandom(rng);
			spell[selected].targeting = 0x10;
			spell[selected].effect = 128;
			spell[selected].routine = 0x07;
			spell[selected].gfx = 0xC0;
			spell[selected].palette = 0x29;
			spellMessages[selected] = 0x01; // HP up!
			SPCR_SetPermissionFalse(spellPermissions, selected, 3); // red mage banned
			Put(0x3AF6F, Blob.FromHex("7F0980")); // changing the oob code for CUR3 to reflect the above effect
			Put(MagicOutOfBattleOffset + MagicOutOfBattleSize * 2, new[] { (byte)(selected + 0xB0) });
			spellindex.Remove(selected);
			int cur4spell = spellindex.Where(id => WhiteSpell(id) && id < 56 && id > 39).ToList().PickRandom(rng);
			spell[cur4spell].targeting = 0x10;
			spell[cur4spell].routine = 0x0F;
			spell[cur4spell].gfx = 0xC0;
			spell[cur4spell].palette = 0x21;
			spellMessages[cur4spell] = 0x18; // HP max!
			SPCR_SetPermissionFalse(spellPermissions, cur4spell, 3); // red mage banned
			SPCR_SetPermissionFalse(spellPermissions, cur4spell, 4); // white mage banned
			SPCR_SetPermissionFalse(spellPermissions, cur4spell, 5); // black mage banned
			SPCR_SetPermissionFalse(spellPermissions, cur4spell, 9); // red wizard banned
			Put(MagicOutOfBattleOffset + MagicOutOfBattleSize * 3, new[] { (byte)(cur4spell + 0xB0) });
			spellindex.Remove(cur4spell);
			int healspell = spellindex.Where(id => WhiteSpell(id) && id < 24).ToList().PickRandom(rng); // we store the index to assign to spellcasting items
			spell[healspell].targeting = 0x08;
			spell[healspell].effect = 16;
			spell[healspell].routine = 0x07;
			spell[healspell].gfx = 0xC0;
			spell[healspell].palette = 0x28;
			spellMessages[healspell] = 0x01; // HP up!
			SPCR_SetPermissionFalse(spellPermissions, healspell, 3); // red mage banned
			SPCR_SetPermissionFalse(spellPermissions, healspell, 9); // red wizard banned
			Put(MagicOutOfBattleOffset + MagicOutOfBattleSize * 4, new[] { (byte)(healspell + 0xB0) });
			Put(0x3AFDF, Blob.FromHex("0F")); // changing the oob code for HEAL to reflect the above effect
			spellindex.Remove(healspell);
			int healspellLevel = healspell / 8;
			selected = spellindex.Where(id => WhiteSpell(id) && id < healspellLevel * 8 + 24 && id > healspellLevel * 8 + 7).ToList().PickRandom(rng);
			spell[selected].targeting = 0x08;
			spell[selected].effect = 32;
			spell[selected].routine = 0x07;
			spell[selected].gfx = 0xC0;
			spell[selected].palette = 0x28;
			spellMessages[selected] = 0x01; // HP up!
			SPCR_SetPermissionFalse(spellPermissions, selected, 3); // red mage banned
			SPCR_SetPermissionFalse(spellPermissions, selected, 9); // red wizard banned
			Put(0x3AFE8, Blob.FromHex("1F")); // changing the oob code for HEL2 to reflect the above effect
			Put(MagicOutOfBattleOffset + MagicOutOfBattleSize * 6, new[] { (byte)(selected + 0xB0) });
			spellindex.Remove(selected);
			healspellLevel = selected / 8;
			selected = spellindex.Where(id => WhiteSpell(id) && id < healspellLevel * 8 + 24 && id > healspellLevel * 8 + 7).ToList().PickRandom(rng);
			spell[selected].targeting = 0x08;
			spell[selected].effect = 64;
			spell[selected].routine = 0x07;
			spell[selected].gfx = 0xC0;
			spell[selected].palette = 0x28;
			spellMessages[selected] = 0x01; // HP up!
			SPCR_SetPermissionFalse(spellPermissions, selected, 3); // red mage banned
			SPCR_SetPermissionFalse(spellPermissions, selected, 9); // red wizard banned
			Put(MagicOutOfBattleOffset + MagicOutOfBattleSize * 5, new[] { (byte)(selected + 0xB0) });
			Put(0x3AFF1, Blob.FromHex("3F")); // changing the oob code for HEL3 to reflect the above effect
			spellindex.Remove(selected);

			int purespell = spellindex.Where(id => WhiteSpell(id) && id < 16).ToList().PickRandom(rng);
			spell[purespell].targeting = 0x10;
			spell[purespell].effect = 0b00011100; // pure now cures darkness and paralysis in battle, woo
			spell[purespell].routine = 0x08;
			spell[purespell].gfx = 0xE0;
			spell[purespell].palette = 0x2A;
			Put(MagicOutOfBattleOffset + MagicOutOfBattleSize * 7, new[] { (byte)(purespell + 0xB0) });
			spellindex.Remove(purespell);
			selected = spellindex.Where(id => WhiteSpell(id) && id < 40 && id > 15).ToList().PickRandom(rng);
			spell[selected].accuracy = 0xFF;
			spellNames[selected] = levelShuffle ? "LIF" + (SpellTier(selected) + 1).ToString() : "LIFE";
			spellMessages[selected] = 0x4A; // Ineffective now
			SPCR_SetPermissionFalse(spellPermissions, selected, 6); // knight banned
			SPCR_SetPermissionFalse(spellPermissions, selected, 7); // ninja banned
			SPCR_SetPermissionFalse(spellPermissions, selected, 3); // red mage banned
			Put(MagicOutOfBattleOffset + MagicOutOfBattleSize * 8, new[] { (byte)(selected + 0xB0) });
			spellindex.Remove(selected);
			selected = spellindex.Where(id => WhiteSpell(id) && id > 47).ToList().PickRandom(rng);
			spell[selected].accuracy = 0xFF;
			spellNames[selected] = levelShuffle ? "LIF" + (SpellTier(selected) + 1).ToString() : "LIF2";
			spellMessages[selected] = 0x4A; // Ineffective now
			SPCR_SetPermissionFalse(spellPermissions, selected, 3); // red mage banned
			SPCR_SetPermissionFalse(spellPermissions, selected, 4); // white mage banned
			SPCR_SetPermissionFalse(spellPermissions, selected, 5); // black mage banned
			SPCR_SetPermissionFalse(spellPermissions, selected, 9); // red wizard banned
			Put(MagicOutOfBattleOffset + MagicOutOfBattleSize * 9, new[] { (byte)(selected + 0xB0) });
			spellindex.Remove(selected);
			selected = spellindex.Where(id => id % 8 > 3 && id > 15).ToList().PickRandom(rng); // warp will appear in black magic regardless of whether we mix spells or not
			spell[selected].accuracy = 0xFF;
			spellNames[selected] = levelShuffle ? "WRP" + (SpellTier(selected) + 1).ToString() : "WARP";
			spellMessages[selected] = 0x4A; // Ineffective now
			SPCR_SetPermissionFalse(spellPermissions, selected, 3); // red mage banned
			SPCR_SetPermissionFalse(spellPermissions, selected, 4); // white mage banned
			SPCR_SetPermissionFalse(spellPermissions, selected, 5); // black mage banned
			Put(MagicOutOfBattleOffset + MagicOutOfBattleSize * 10, new[] { (byte)(selected + 0xB0) });
			spellindex.Remove(selected);
			selected = spellindex.Where(id => WhiteSpell(id) && id < 48).ToList().PickRandom(rng);
			spell[selected].accuracy = 0xFF;
			spellNames[selected] = levelShuffle ? "SFT" + (SpellTier(selected) + 1).ToString() : "SOFT";
			spellMessages[selected] = 0x4A; // Ineffective now
			SPCR_SetPermissionFalse(spellPermissions, selected, 3); // red mage banned
			SPCR_SetPermissionFalse(spellPermissions, selected, 9); // red wizard banned
			Put(MagicOutOfBattleOffset + MagicOutOfBattleSize * 11, new[] { (byte)(selected + 0xB0) });
			spellindex.Remove(selected);
			selected = spellindex.Where(id => id % 8 < 4 && id > 15).ToList().PickRandom(rng); // exit will appear in white magic regardless of whether we mix spells or not
			spell[selected].accuracy = 0xFF;
			spellNames[selected] = levelShuffle ? "EXT" + (SpellTier(selected) + 1).ToString() : "EXIT";
			spellMessages[selected] = 0x4A; // Ineffective now
			SPCR_SetPermissionFalse(spellPermissions, selected, 3); // red mage banned
			SPCR_SetPermissionFalse(spellPermissions, selected, 4); // white mage banned
			SPCR_SetPermissionFalse(spellPermissions, selected, 5); // black mage banned
			Put(MagicOutOfBattleOffset + MagicOutOfBattleSize * 12, new[] { (byte)(selected + 0xB0) });
			spellindex.Remove(selected);

			// draw guaranteed spells
			int rusespell = spellindex.Where(id => WhiteSpell(id) && id < 32).ToList().PickRandom(rng); // guaranteed RUSE, we will keep track of this when assigning spells to items
			SPCR_CraftEvasionSpell(rng, spell[rusespell], SpellTier(rusespell), 0x04);
			spellMessages[rusespell] = 0x03; // Evasion up
			SPCR_SetPermissionFalse(spellPermissions, rusespell, 3); // red mage banned
			spellindex.Remove(rusespell);
			int sabrspell = spellindex.Where(id => BlackSpell(id) && id > 7 && id < 32).ToList().PickRandom(rng); // guaranteed SABR, again we track this for assigning spells to itmes
			SPCR_CraftAttackUpSpell(rng, spell[sabrspell], SpellTier(sabrspell), true);
			SPCR_SetPermissionFalse(spellPermissions, sabrspell, 3); // red mage banned
			spellindex.Remove(sabrspell);
			int fastspell = spellindex.Where(id => BlackSpell(id)).ToList().PickRandom(rng); // guaranteed FAST, we do NOT want this to land on an item and we need to track this
			SPCR_CraftFastSpell(spell[fastspell], SpellTier(fastspell));
			spellMessages[fastspell] = 0x12; // Quick Shot
			if (spell[fastspell].targeting != 0x04)
			{
				SPCR_SetPermissionFalse(spellPermissions, fastspell, 3); // red mage banned
			}
			if (spell[fastspell].targeting == 0x08)
			{
				SPCR_SetPermissionFalse(spellPermissions, fastspell, 9); // red wizard banned from AoE FAST
			}
			spellindex.Remove(fastspell);
			int slowspell = spellindex.Where(id => BlackSpell(id) && id > 31 && id < 48).ToList().PickRandom(rng); // guaranteed SLO2 equivalent
			spell[slowspell].routine = 0x04;
			spell[slowspell].accuracy = 64;
			spell[slowspell].targeting = 0x02;
			spellMessages[slowspell] = 0x0B; // Lost intelligence
			spellindex.Remove(slowspell);

			// set limits and track the kind of spells being drawn
			bool rollSecondSlow = true; // if there is a slow spell, we only allow a second slow spell to be drawn.
			bool rollMoraleSpell = true; // we only allow one morale spell to be drawn
			int sleepspell = -1; // we only allow two sleep spells to be drawn.  spells which apply another status DO count
			int lockspell = -1; ; // we only allow two lock spells to be drawn
			List<int> stunspell = new List<int> { }; // track which spells have paralysis as a status infliction, and ensure that paralysis spells are either two tiers apart or in the opposite color family
			List<int> mutespell = new List<int> { }; // we maintain the same restriction for silencing spells
			bool rollDarknessSpell = true; // we only allow one spell to roll darkness as the ONLY status effect
			bool rollConfusionSpell = true; // we only allow one spell to roll confusion, unless that spell is also attached to Silence or Paralysis effects
			bool rollSecondFast = true; // if we rolled a second FAST, we disallow rolling another such spell (in either spell realm)
			bool rollDispel = true; // we only allow one XFER-type spell to be drawn
			bool rollEsuna = true; // we allow one status recovery spell outside of PURE and SOFT to roll
			List<int> killspell = new List<int> { }; // do not allow duplication of death/stone spells that have the same element and targeting byte
			bool powerWordKill = true; // we only allow one power word kill (with a few exceptions)
			bool powerWordStun = true; // we only allow one power word stun
			bool powerWordConf = true; // we allow power word confusion (even if a confusion spell has already been drawn)
			bool powerWordMute = true; // we allow power word mute
			List<int> harmspell = new List<int> { }; // harm spells when drawn must be separated by two tiers or be in the opposite color family, and cannot exist alongside a damage spell of the same family
			List<int>[] elemSpell = new List<int>[9]; // lists of elemental damage spells.  damage spells of the same element cannot be in the same tier
			elemSpell[0] = new List<int> { }; // status
			elemSpell[1] = new List<int> { }; // poison
			elemSpell[2] = new List<int> { }; // time
			elemSpell[3] = new List<int> { }; // death
			elemSpell[4] = new List<int> { elemspell[0], elemspell[1] }; // fire
			elemSpell[5] = new List<int> { elemspell[2], elemspell[3] }; // ice
			elemSpell[6] = new List<int> { elemspell[4], elemspell[5] }; // lit
			elemSpell[7] = new List<int> { }; // earth
			elemSpell[8] = elemSpell[2]; // non-elemntal list points to the time elemental list
			List<byte> resistelems = new List<byte> { }; // do not allow duplication of resistance spells
			bool rollWall = true; // we can only roll one WALL spell
			List<int> defenseupspell = new List<int> { }; // defense raising spells must be separated by three tiers (except self-targeting spells, which are under the same restriction with each other)
			List<int> selfdefenseupspell = new List<int> { };
			List<int> evasionspell = new List<int> { }; // same deal for evasion increase spells
			List<int> selfevasionspell = new List<int> { rusespell };
			List<int> attackupspell = new List<int> { sabrspell }; // attack increasing spells must be separated by three tiers regardless of targeting, whether they are white or black magic

			spellindex.Shuffle(rng);

			// draw remaining spells
			foreach(int index in spellindex)
			{
				// first, we determine the routines we can select in the first place
				List<byte> validroutines = new List<byte> { 0x01, 0x03 };
				if (WhiteSpell(index))
				{
					validroutines.Add(0x02);
					validroutines.Add(0x02);
					validroutines.Add(0x09);
					validroutines.Add(0x0A);
					validroutines.Add(0x10);
					if (SpellTier(index) > SpellTier(purespell) && rollEsuna && SpellTier(index) < 6)
						validroutines.Add(0x08);
				}
				if (BlackSpell(index))
				{
					validroutines.Add(0x0D);
					if(lockspell != -2 && SpellTier(index) < 6 && SpellTier(lockspell) != SpellTier(index))
					{
						if(lockspell == -1)
							validroutines.Add(0x0E);
						else
						{
							if (Math.Abs(SpellTier(lockspell) - SpellTier(index)) > 2)
								validroutines.Add(0x0E);
						}
					}					
					if(rollSecondSlow && SpellTier(index) < 4)
						validroutines.Add(0x04);
					if(rollSecondFast)
						validroutines.Add(0x0C);
				}
				if (rollMoraleSpell && SpellTier(index) < 4)
					validroutines.Add(0x05);
				if (SpellTier(index) > 3)
				{
					if(rollDispel)
						validroutines.Add(0x11);
					validroutines.Add(0x12);
				}
				bool looping = true;
				while(looping)
				{
					if(validroutines.Count == 0)
					{
						var nulls = spellNames.Where(s => s != null && s.StartsWith("NUL")).Count();

						if (nulls == 0)
						{
							spellNames[index] = "NULL";
						}
						else
						{
							spellNames[index] = $"NUL{nulls + 1}";
						}

						spellMessages[index] = 0x4A;
						spell[index].routine = 0x00;
						// if there are no valid routines, the NULL spell is created which does absolutely nothing.  all classes can learn it assuming it fits normal criteria.
						break;
					}
					byte routine = validroutines.PickRandom(rng);
					if(routine == 0x01) // damage spells
					{
						int elem = 0;
						if(WhiteSpell(index) && !mixWhiteBlack) // if spells are not mixed, white magic can only be status or non elemental
						{
							if(elemSpell[7].Exists(id => WhiteSpell(id) == WhiteSpell(index) && SpellTier(id) == SpellTier(index)) ||
								elemSpell[8].Exists(id => WhiteSpell(id) == WhiteSpell(index) && SpellTier(id) == SpellTier(index)))
							{
								validroutines.Remove(0x01); // remove damage routine if spell is white magic damage and such a spell already exists on this tier
								continue;
							}
							if (SpellTier(index) < 5)
							{
								if (rng.Between(0, 1) == 0)
									elem = 7;
								else
									elem = 8;
							}
							else
								elem = 8; // tier 6 and up are always non-elemental
						}
						else
						{
							elem = rng.Between(0, mixWhiteBlack ? 8 : 7);
							if (!mixWhiteBlack && elem == 7)
								elem = 8;
						}
						if(SpellTier(index) < 4) // do not allow fire/ice/lit elementals in the first four tiers, since we have guaranteed spells for those anyway
						{
							if (elem > 0 && elem < 4)
							{
								elem = rng.Between(4, 9); // can roll any other element, with 9 being converted to 0 for earth element
								if (elem == 9)
									elem = 0;
								if (elem == 7 && !mixWhiteBlack)
									elem = 8;
							}								
						}
						if (elemSpell[elem].Exists(i => BlackSpell(index) == BlackSpell(i) && SpellTier(index) == SpellTier(i)))
							continue; // return to start if this spell is in the same realm and at the same tier as another spell of the same element
						SPCR_CraftDamageSpell(rng, spell[index], SpellTier(index), (byte)(0b10000000 >> elem), WhiteSpell(index) && !mixWhiteBlack, false);
						if(elem == 4 || elem == 5 || elem == 8)
						{
							SPCR_SetPermissionFalse(spellPermissions, index, 3); // red mage banned
							SPCR_SetPermissionFalse(spellPermissions, index, 9); // red wizard banned
						}
						if (SpellTier(index) > 5)
							SPCR_SetPermissionFalse(spellPermissions, index, 9); // red wizard banned
						elemSpell[elem].Add(index);
					}
					if(routine == 0x02) // damage undead (or other enemy type)
					{
						if (harmspell.Exists(i => BlackSpell(index) == BlackSpell(i) && Math.Abs(SpellTier(index) - SpellTier(i)) < 2))
						{
							validroutines.Remove(0x02); // don't pick this routine again
							validroutines.Remove(0x02);
							continue; // return to start if this spell is in the same realm and not at least two tiers away from any other such spell
						}
						SPCR_CraftHarmSpell(spell[index], SpellTier(index));
						SPCR_SetPermissionFalse(spellPermissions, index, 3); // red mage banned
						SPCR_SetPermissionFalse(spellPermissions, index, 9); // red wizard banned
						harmspell.Add(index);
					}
					if(routine == 0x03) // inflict negative effect
					{
						List<byte> validStatuses = new List<byte> { };
						if (rollConfusionSpell && SpellTier(index) < 4)
							validStatuses.Add(0b10000000);
						if (rollDarknessSpell && BlackSpell(index) && SpellTier(index) < 2)
							validStatuses.Add(0b00001000);
						if (!stunspell.Exists(id => Math.Abs(SpellTier(index) - SpellTier(id)) < 2) && SpellTier(index) > 1 && BlackSpell(index))
							validStatuses.Add(0b00010000);
						if (SpellTier(index) < 4 && sleepspell != -2 && SpellTier(sleepspell) != SpellTier(index))
							validStatuses.Add(0b00100000);
						if (SpellTier(index) < 4 && !mutespell.Exists(id => Math.Abs(SpellTier(index) - SpellTier(id)) < 2))
							validStatuses.Add(0b01000000);
						if (SpellTier(index) > 2 && BlackSpell(index) && !killspell.Exists(id => SpellTier(id) == SpellTier(index) && BlackSpell(index) == BlackSpell(index)))
							validStatuses.Add(0b00000001);
						if (validStatuses.Count == 0)
						{
							validroutines.Remove(0x03);
							continue;
						}
						byte effect = validStatuses.PickRandom(rng);
						byte elem = 0;
						List<byte> validElements = new List<byte> { };
						switch(effect)
						{
							case 0b00000001: // death / stone
								validElements.Add(0b00001000);
								if(SpellTier(index) > 3)
								{
									validElements.Add(0b10000000);
									validElements.Add(0b00000010);
								}
								if (SpellTier(index) > 4)
									validElements.Add(0b00000100);
								if (SpellTier(index) > 5)
									validElements.Add(0b00100000);
								if (SpellTier(index) > 6)
								{
									validElements.Add(0b01000000);
									validElements.Add(0b00000000);
								}
								elem = validElements.PickRandom(rng);
								if (killspell.Exists(id => Math.Abs(SpellTier(id) - SpellTier(index)) < 2 && spell[id].elem == elem))
									continue; // continue if there is a kill spell of the same element within this or an adjacent tier
								SPCR_SetPermissionFalse(spellPermissions, index, 3); // red mage banned
								SPCR_SetPermissionFalse(spellPermissions, index, 9); // red wizard banned
								killspell.Add(index);
								switch (elem)
								{
									case 0b00001000:
										switch(SpellTier(index))
										{
											case 3:
												spell[index].targeting = 0x02;
												spell[index].elem = elem;
												spell[index].effect = effect;
												spell[index].accuracy = 32;
												spell[index].routine = routine;
												break;
											case 4:
												spell[index].targeting = 0x02;
												spell[index].elem = elem;
												spell[index].effect = effect;
												spell[index].accuracy = 40;
												spell[index].routine = routine;
												break;
											case 5:
												spell[index].targeting = 0x02;
												spell[index].elem = elem;
												spell[index].effect = effect;
												spell[index].accuracy = 48;
												spell[index].routine = routine;
												break;
											case 6:
												spell[index].targeting = (byte)rng.Between(1, 2);
												spell[index].elem = elem;
												spell[index].effect = effect;
												spell[index].accuracy = 48;
												spell[index].routine = routine;
												break;
											case 7:
												spell[index].targeting = 0x01;
												spell[index].elem = elem;
												spell[index].effect = effect;
												spell[index].accuracy = 64;
												spell[index].routine = routine;
												break;
										}
										spellMessages[index] = 0x15; // Erased
										break;
									case 0b10000000:
										switch (SpellTier(index))
										{
											case 4:
												spell[index].targeting = (byte)rng.Between(1, 2);
												spell[index].elem = elem;
												spell[index].effect = effect;
												spell[index].accuracy = (byte)(spell[index].targeting == 0x01 ? 40 : 52);
												spell[index].routine = routine;
												break;
											case 5:
												spell[index].targeting = (byte)rng.Between(1, 2);
												spell[index].elem = elem;
												spell[index].effect = effect;
												spell[index].accuracy = (byte)(spell[index].targeting == 0x01 ? 40 : 52);
												spell[index].routine = routine;
												break;
											case 6:
												spell[index].targeting = (byte)rng.Between(1, 2);
												spell[index].elem = elem;
												spell[index].effect = effect;
												spell[index].accuracy = (byte)(spell[index].targeting == 0x01 ? 48 : 64);
												spell[index].routine = routine;
												break;
											case 7:
												spell[index].targeting = 0x01;
												spell[index].elem = elem;
												spell[index].effect = effect;
												spell[index].accuracy = 0;
												spell[index].routine = 0x12;
												break;
										}
										spellMessages[index] = 0x16; // Fell into crack
										break;
									case 0b00000010:
										switch (SpellTier(index))
										{
											case 4:
												spell[index].targeting = (byte)rng.Between(1, 2);
												spell[index].elem = elem;
												spell[index].effect = (byte)(spell[index].targeting == 0x01 ? 0b00000001 : 0b00000010);
												spell[index].accuracy = (byte)(spell[index].targeting == 0x01 ? 40 : 52);
												spell[index].routine = routine;
												break;
											case 5:
												spell[index].targeting = (byte)rng.Between(1, 2);
												spell[index].elem = elem;
												spell[index].effect = (byte)(spell[index].targeting == 0x01 ? 0b00000001 : 0b00000010);
												spell[index].accuracy = (byte)(spell[index].targeting == 0x01 ? 40 : 52);
												spell[index].routine = routine;
												break;
											case 6:
												spell[index].targeting = (byte)rng.Between(1, 2);
												spell[index].elem = elem;
												spell[index].effect = (byte)(spell[index].targeting == 0x01 ? 0b00000001 : 0b00000010);
												spell[index].accuracy = (byte)(spell[index].targeting == 0x01 ? 48 : 64);
												spell[index].routine = routine;
												break;
											case 7:
												spell[index].targeting = 0x01;
												spell[index].elem = elem;
												spell[index].effect = (byte)(rng.Between(0, 1) == 0 ? 0b00000001 : 0b00000010);
												spell[index].accuracy = 67;
												spell[index].routine = routine;
												break;
										}
										spellMessages[index] = (byte)(spell[index].effect == 0b00000001 ? 0x4D : 0x00);
										break;
									case 0b00000100:
										switch (SpellTier(index))
										{
											case 5:
												spell[index].targeting = 0x02;
												spell[index].elem = elem;
												spell[index].effect = effect;
												spell[index].accuracy = 36;
												spell[index].routine = routine;
												break;
											case 6:
												spell[index].targeting = (byte)rng.Between(1, 2);
												spell[index].elem = elem;
												spell[index].effect = effect;
												spell[index].accuracy = (byte)(spell[index].targeting == 0x01 ? 32 : 46);
												spell[index].routine = routine;
												break;
											case 7:
												spell[index].targeting = 0x01;
												spell[index].elem = elem;
												spell[index].effect = effect;
												spell[index].accuracy = 55;
												spell[index].routine = routine;
												break;
										}
										spellMessages[index] = 0x1F; // Exiled to 4th dimension
										break;
									case 0b00100000:
										switch (SpellTier(index))
										{
											case 6:
												spell[index].targeting = (byte)rng.Between(1, 2);
												spell[index].elem = elem;
												spell[index].effect = 0b00000010;
												spell[index].accuracy = (byte)(spell[index].targeting == 0x01 ? 36 : 56);
												spell[index].routine = routine;
												break;
											case 7:
												spell[index].targeting = 0x01;
												spell[index].elem = elem;
												spell[index].effect = 0b00000010;
												spell[index].accuracy = 67;
												spell[index].routine = routine;
												break;
										}
										break;
									case 0b01000000:
										spell[index].targeting = 0x01;
										spell[index].elem = elem;
										spell[index].effect = effect;
										spell[index].accuracy = 67;
										spell[index].routine = routine;
										break;
									case 0b00000000:
										spell[index].targeting = 0x02;
										spell[index].elem = elem;
										spell[index].effect = effect;
										spell[index].accuracy = 40;
										spell[index].routine = routine;
										break;
								}
								break;
							case 0b00010000: // paralysis
								switch(SpellTier(index))
								{
									case 2:
										spell[index].targeting = 0x02;
										spell[index].elem = (byte)(rng.Between(0, 1) == 0 ? 0b00000001 : 0b10000000);
										spell[index].effect = effect;
										spell[index].accuracy = 64;
										spell[index].routine = routine;
										break;
									case 3:
										spell[index].targeting = 0x02;
										spell[index].elem = (byte)(rng.Between(0, 1) == 0 ? 0b00000001 : 0b10000000);
										spell[index].effect = effect;
										spell[index].accuracy = 64;
										spell[index].routine = routine;
										break;
									case 4:
										spell[index].targeting = 0x02;
										spell[index].elem = 0b00000100;
										spell[index].effect = effect;
										spell[index].accuracy = 48;
										spell[index].routine = routine;
										SPCR_SetPermissionFalse(spellPermissions, index, 9); // red wizard banned
										break;
									case 5:
										spell[index].targeting = (byte)rng.Between(1, 2);
										spell[index].elem = (byte)(spell[index].targeting == 0x01 ? 0b00000001 : 0b00000100);
										spell[index].effect = effect;
										spell[index].accuracy = (byte)(spell[index].targeting == 0x01 ? 40 : 64);
										spell[index].routine = routine;
										SPCR_SetPermissionFalse(spellPermissions, index, 9); // red wizard banned
										break;
									case 6:
										spell[index].targeting = 0x01;
										spell[index].elem = 0b00000100;
										spell[index].effect = effect;
										spell[index].accuracy = 64;
										spell[index].routine = routine;
										SPCR_SetPermissionFalse(spellPermissions, index, 9); // red wizard banned
										break;
									case 7:
										spell[index].targeting = 0x02;
										spell[index].elem = 0b00000000;
										spell[index].effect = effect;
										spell[index].accuracy = 107;
										spell[index].routine = routine;
										break;
								}
								spellMessages[index] = (byte)(spell[index].elem == 0b00000100 ? 0x1E : 0x0D);
								stunspell.Add(index);
								break;
							case 0b10000000: // confusion
								switch(SpellTier(index))
								{
									case 0:
										spell[index].targeting = 0x02;
										spell[index].elem = 0b00000001;
										spell[index].effect = effect;
										spell[index].accuracy = 40;
										spell[index].routine = routine;
										break;
									case 1:
										spell[index].targeting = 0x02;
										spell[index].elem = 0b00000001;
										spell[index].effect = (byte)(effect | GenerateRandomBits(0b00101000, rng));
										spell[index].accuracy = 64;
										spell[index].routine = routine;
										break;
									case 2:
										spell[index].targeting = 0x01;
										spell[index].elem = 0b00000001;
										spell[index].effect = effect;
										spell[index].accuracy = 64;
										spell[index].routine = routine;
										break;
									case 3:
										spell[index].targeting = 0x01;
										spell[index].elem = 0b00000001;
										spell[index].effect = (byte)(effect | GenerateRandomBits(0b00101000, rng));
										spell[index].accuracy = 64;
										spell[index].routine = routine;
										break;
								}
								rollConfusionSpell = false;
								break;
							case 0b01000000: // silence
								switch (SpellTier(index))
								{
									case 0:
										spell[index].targeting = 0x02;
										spell[index].elem = 0b00000001;
										spell[index].effect = effect;
										spell[index].accuracy = 64;
										spell[index].routine = routine;
										break;
									case 1:
										spell[index].targeting = 0x01;
										spell[index].elem = 0b00000001;
										spell[index].effect = effect;
										spell[index].accuracy = 64;
										spell[index].routine = routine;
										break;
									case 2:
										spell[index].targeting = 0x01;
										spell[index].elem = 0b00000001;
										spell[index].effect = effect;
										spell[index].accuracy = 64;
										spell[index].routine = routine;
										break;
									case 3:
										spell[index].targeting = 0x01;
										spell[index].elem = 0b00000001;
										spell[index].effect = (byte)(effect | GenerateRandomBits(0b00101000, rng));
										spell[index].accuracy = 64;
										spell[index].routine = routine;
										break;
								}
								mutespell.Add(index);
								break;
							case 0b00100000: // sleep
								switch (SpellTier(index))
								{
									case 0:
										spell[index].targeting = 0x01;
										spell[index].elem = 0b00000001;
										spell[index].effect = effect;
										spell[index].accuracy = 64;
										spell[index].routine = routine;
										break;
									case 1:
										spell[index].targeting = 0x02;
										spell[index].elem = 0b00000000;
										spell[index].effect = effect;
										spell[index].accuracy = 64;
										spell[index].routine = routine;
										break;
									case 2:
										spell[index].targeting = 0x01;
										spell[index].elem = 0b00000000;
										spell[index].effect = effect;
										spell[index].accuracy = 40;
										spell[index].routine = routine;
										break;
									case 3:
										spell[index].targeting = 0x02;
										spell[index].elem = 0b00000001;
										spell[index].effect = effect;
										spell[index].accuracy = 0;
										spell[index].routine = 0x12;
										break;
								}
								if (sleepspell == -1)
									sleepspell = index;
								else
									sleepspell = -2;
								break;
							case 0b00001000: // (sigh) darkness
								switch(SpellTier(index))
								{
									case 0:
										spell[index].targeting = 0x01;
										spell[index].elem = 0b00000001;
										spell[index].effect = effect;
										spell[index].accuracy = 64;
										spell[index].routine = routine;
										break;
									case 1:
										spell[index].targeting = 0x01;
										spell[index].elem = 0b00000001;
										spell[index].effect = effect;
										spell[index].accuracy = 0;
										spell[index].routine = 0x12;
										break;
								}
								rollDarknessSpell = false;
								break;
						}
					}
					if(routine == 0x12) // inflict negative effect if hp < 300
					{
						List<byte> validStatuses = new List<byte> { };
						if (powerWordConf && SpellTier(index) < 7)
							validStatuses.Add(0b10000000);
						if (powerWordMute && SpellTier(index) < 7)
							validStatuses.Add(0b01000000);
						if (powerWordStun && SpellTier(index) < 7 && BlackSpell(index))
							validStatuses.Add(0b00010000);
						if (powerWordKill && SpellTier(index) > 5 && BlackSpell(index))
							validStatuses.Add(0b00000001);
						if(validStatuses.Count == 0)
						{
							validroutines.Remove(0x12);
							continue;
						}
						byte effect = validStatuses.PickRandom(rng);
						switch(effect)
						{
							case 0b10000000:
								switch(SpellTier(index))
								{
									case 4:
										spell[index].effect = effect;
										spell[index].targeting = 0x02;
										spell[index].routine = routine;
										spell[index].elem = 0b00000001;
										break;
									case 5:
										spell[index].effect = (byte)(effect | GenerateRandomBits(0b00101000, rng));
										spell[index].targeting = 0x02;
										spell[index].routine = routine;
										spell[index].elem = 0b00000001;
										break;
									case 6:
										spell[index].effect = 0b10000000;
										spell[index].targeting = 0x01;
										spell[index].routine = routine;
										spell[index].elem = 0b00000001;
										break;
								}
								powerWordConf = false;
								break;
							case 0b01000000:
								switch (SpellTier(index))
								{
									case 4:
										spell[index].effect = effect;
										spell[index].targeting = 0x02;
										spell[index].routine = routine;
										spell[index].elem = 0b00000001;
										break;
									case 5:
										spell[index].effect = (byte)(effect | GenerateRandomBits(0b00101000, rng));
										spell[index].targeting = 0x02;
										spell[index].routine = routine;
										spell[index].elem = 0b00000100;
										break;
									case 6:
										spell[index].effect = 0b01010000;
										spell[index].targeting = 0x01;
										spell[index].routine = routine;
										spell[index].elem = 0b00000100;
										break;
								}
								powerWordMute = false;
								break;
							case 0b00010000:
								switch (SpellTier(index))
								{
									case 4:
										spell[index].effect = effect;
										spell[index].targeting = 0x02;
										spell[index].routine = routine;
										spell[index].elem = 0b00000001;
										break;
									case 5:
										spell[index].effect = effect;
										spell[index].targeting = 0x02;
										spell[index].routine = routine;
										spell[index].elem = 0b00000001;
										break;
									case 6:
										spell[index].effect = effect;
										spell[index].targeting = 0x01;
										spell[index].routine = routine;
										spell[index].elem = 0b00000001;
										break;
								}
								powerWordStun = false;
								break;
							case 0b00000001:
								spell[index].effect = effect;
								spell[index].targeting = 0x02;
								spell[index].routine = routine;
								spell[index].elem = (byte)(SpellTier(index) == 7 ? 0b00000000 : 0b00001000);
								powerWordKill = false;
								spellMessages[index] = 0x15;
								break;
						}
						SPCR_SetPermissionFalse(spellPermissions, index, 9); // red wizard banned from all power word spells
					}
					if(routine == 0x04) // decreases enemy speed
					{
						switch(SpellTier(index))
						{
							case 0:
								spell[index].targeting = 0x02;
								spell[index].elem = 0b00000001;
								spell[index].accuracy = 64;
								break;
							case 1:
								spell[index].targeting = 0x01;
								spell[index].elem = 0b00000001;
								spell[index].accuracy = 64;
								break;
							case 2:
								spell[index].targeting = 0x01;
								spell[index].elem = 0b00000001;
								spell[index].accuracy = 64;
								break;
							case 3:
								spell[index].targeting = 0x02;
								spell[index].elem = 0b00000000;
								spell[index].accuracy = 64;
								break;
							case 4:
								spell[index].targeting = 0x02;
								spell[index].elem = 0b00000000;
								spell[index].accuracy = 64;
								break;
							case 5:
								spell[index].targeting = 0x01;
								spell[index].elem = 0b00000000;
								spell[index].accuracy = 64;
								SPCR_SetPermissionFalse(spellPermissions, index, 9); // red wizard banned
								break;
						}
						spell[index].effect = 0;
						spell[index].routine = routine;
						spellMessages[index] = 0x0B; // Lost intelligence
						rollSecondSlow = false;
					}
					if(routine == 0x05) // morale down
					{
						spell[index].elem = 0b00000001;
						spell[index].effect = (byte)(40 * (SpellTier(index) + 1));
						spell[index].targeting = 0x01;
						spell[index].routine = routine;
						spell[index].accuracy = 24;
						spellMessages[index] = 0x0F; // Became terrified
						SPCR_SetPermissionFalse(spellPermissions, index, 3); // red mage banned
						SPCR_SetPermissionFalse(spellPermissions, index, 9); // red wizard banned
						rollMoraleSpell = false;
					}
					if(routine == 0x08) // recover status (this will be the AMUT equivalent automatically)
					{
						spell[index].effect = 0b11110000; // cures confusion, silence, sleep, and stun
						spell[index].targeting = (byte)(SpellTier(index) < 2 ? 0x10 : 0x08); // tier 1 and 2 are single ally, tier 3 and 4 are all allies
						spell[index].routine = routine;
						SPCR_SetPermissionFalse(spellPermissions, index, 3); // red mage banned
						rollEsuna = false;
					}
					if(routine == 0x09) // armor up
					{
						byte targeting = 0;
						switch (rng.Between(0, 4))
						{
							case 0:
								targeting = 0x04;
								break;
							case 1:
								targeting = 0x08;
								break;
							case 2:
								targeting = 0x10;
								break;
							case 3:
								targeting = 0x10;
								break;
							case 4:
								targeting = 0x08;
								break;
						}
						if (targeting == 0x08 && SpellTier(index) < 4)
							targeting = 0x10;
						if (SpellTier(index) == 7)
							targeting = 0x04;
						if (targeting == 0x04 && SpellTier(index) != 7)
						{
							if (selfdefenseupspell.Exists(id => Math.Abs(SpellTier(id) - SpellTier(index)) < 3))
							{
								validroutines.Remove(0x09);
								continue;
							}
						}
						else
						{
							if (defenseupspell.Exists(id => Math.Abs(SpellTier(id) - SpellTier(index)) < 3))
							{
								validroutines.Remove(0x09);
								continue;
							}
						}
						spell[index].targeting = targeting;
						spell[index].effect = (byte)(spell[index].targeting == 0x08 ? (SpellTier(index) - 3) * 8 : (SpellTier(index) + 2) * 4);
						if (spell[index].targeting == 0x04)
							spell[index].effect = (byte)(spell[index].effect + spell[index].effect / 2);
						if (SpellTier(index) == 7)
							spell[index].targeting = 0x08;
						spell[index].routine = routine;
						spellMessages[index] = 0x02; // Armor up
						if (spell[index].targeting == 0x04)
							selfdefenseupspell.Add(index);
						else
							defenseupspell.Add(index);
					}
					if(routine == 0x0A) // resist elements
					{
						byte resistances = 0x00;
						if(SpellTier(index) < 4)
						{
							List<byte> validElements = new List<byte> { 0b01000000, 0b00100000, 0b00010000, 0b00000010, 0b00000001 };
							validElements = validElements.Except(resistelems).ToList();
							if(validElements.Count == 0)
							{
								validroutines.Remove(0x0A);
								continue;
							}
							switch(validElements.PickRandom(rng))
							{
								case 0b01000000:
									resistances = 0b01000000;
									spellMessages[index] = 0x08; // Defend lightning
									break;
								case 0b00100000:
									resistances = 0b00100000;
									spellMessages[index] = 0x10; // Defend cold
									break;
								case 0b00010000:
									resistances = 0b00010000;
									spellMessages[index] = 0x0C; // Defend fire
									break;
								case 0b00000010:
									resistances = 0b00000010;
									spellMessages[index] = 0x19; // Defend magic
									break;
								case 0b00000001:
									resistances = 0b00000001;
									spellMessages[index] = 0x19; // Defend magic
									break;
							}
							spell[index].targeting = 0x08;
							resistelems.Add(resistances);
						}
						else if(SpellTier(index) < 7)
						{
							List<byte> validElements = new List<byte> { 0b01110000, 0b10001001, 0b10000101, 0b11111111 };
							validElements = validElements.Except(resistelems).ToList();
							if (validElements.Count == 0)
							{
								validroutines.Remove(0x0A);
								continue;
							}
							switch (validElements.PickRandom(rng))
							{
								case 0b01110000:
									resistances = 0b01110000;
									spellMessages[index] = 0x19; // Defend magic
									spell[index].targeting = 0x08;
									break;
								case 0b10001001:
									resistances = 0b10001001;
									spellMessages[index] = 0x19; // Defend magic
									spell[index].targeting = 0x08;
									break;
								case 0b10000101:
									resistances = 0b10000101;
									spellMessages[index] = 0x19; // Defend magic
									spell[index].targeting = 0x08;
									break;
								case 0b11111111:
									resistances = 0b11111111;
									spellMessages[index] = 0x1C; // Defend all
									spell[index].targeting = 0x10;
									SPCR_SetPermissionFalse(spellPermissions, index, 3); // red mage banned
									SPCR_SetPermissionFalse(spellPermissions, index, 9); // red wizard banned
									break;
							}
							resistelems.Add(resistances);
						}
						else
						{
							if(rollWall)
							{
								resistances = 0xFF; // tier 8 is always WALL
								spell[index].targeting = 0x08;
								spellMessages[index] = 0x1C; // Defend all
								rollWall = false;
							}
							else
							{
								validroutines.Remove(0x0A);
								continue;
							}
						}
						spell[index].effect = resistances;
						spell[index].routine = routine;
					}
					if(routine == 0x0C) // double number of hits (FAST)
					{
						if ((SpellTier(fastspell) < 3 && SpellTier(index) < 3) || (SpellTier(fastspell) == SpellTier(index)) || (SpellTier(fastspell) > 2 && SpellTier(fastspell) < 6 && SpellTier(index) > 2 && SpellTier(index) < 6) || (SpellTier(fastspell) > 5 && SpellTier(index) > 5 )) // if both spells would have the same effect, do not roll
						{
							validroutines.Remove(0x0C);
							continue;
						}
						SPCR_CraftFastSpell(spell[index], SpellTier(index));
						spellMessages[index] = 0x12; // Quick shot
						if(spell[index].targeting != 0x04)
						{
							SPCR_SetPermissionFalse(spellPermissions, index, 3); // red mage banned
						}
						if(spell[index].targeting == 0x08)
						{
							SPCR_SetPermissionFalse(spellPermissions, index, 9); // red wizard banned
						}
						rollSecondFast = false;
					}
					if(routine == 0x0D) // attack up (TMPR, SABR)
					{
						if (attackupspell.Exists(id => Math.Abs(SpellTier(id) - SpellTier(index)) < 3))
						{
							validroutines.Remove(0x0D);
							continue;
						}
						SPCR_CraftAttackUpSpell(rng, spell[index], SpellTier(index), false);
						SPCR_SetPermissionFalse(spellPermissions, index, 3); // red mage banned
						attackupspell.Add(index);
					}
					if(routine == 0x0E) // decrease evade (LOCK)
					{
						spell[index].targeting = (byte)rng.Between(1, 2);
						spell[index].effect = (byte)(20 * (SpellTier(index) + 1) * spell[index].targeting);
						if (lockMode == LockHitMode.AutoHit)
							spell[index].effect >>= 1; // LSR effect byte by one (divide by two) if lock fix is on
						spell[index].accuracy = 107; // max accuracy for LOCK spells
						spell[index].routine = routine;
						spellMessages[index] = 0x05; // Easy to hit
						SPCR_SetPermissionFalse(spellPermissions, index, 3); // red mage banned
						if (lockspell == -1)
							lockspell = index;
						else
							lockspell = -2;
					}
					if(routine == 0x10) // increase evade (INVS)
					{
						byte targeting = 0;
						switch (rng.Between(0, 4))
						{
							case 0:
								targeting = 0x04;
								break;
							case 1:
								targeting = 0x08;
								break;
							case 2:
								targeting = 0x10;
								break;
							case 3:
								targeting = 0x10;
								break;
							case 4:
								targeting = 0x08;
								break;
						}
						if (SpellTier(index) < 4 && targeting == 0x08)
							targeting = 0x10;
						if (targeting == 0x04 || SpellTier(index) == 7)
						{
							if (selfevasionspell.Exists(id => Math.Abs(SpellTier(id) - SpellTier(index)) < 3))
							{
								validroutines.Remove(0x10);
								continue;
							}
						}
						if (targeting != 0x04 || SpellTier(index) == 7)
						{
							if (evasionspell.Exists(id => Math.Abs(SpellTier(id) - SpellTier(index)) < 3))
							{
								validroutines.Remove(0x10);
								continue;
							}
						}
						SPCR_CraftEvasionSpell(rng, spell[index], SpellTier(index), targeting);
						if(spell[index].targeting == 0x04)
							SPCR_SetPermissionFalse(spellPermissions, index, 3); // red mage banned
						spellMessages[index] = 0x03; // Easy to dodge
						if (spell[index].targeting == 0x04)
							selfevasionspell.Add(index);
						else
							evasionspell.Add(index);
					}
					if(routine == 0x11) // remove resistances (XFER)
					{
						spell[index].targeting = (byte)(SpellTier(index) == 7 ? 0x01 : 0x02);
						spell[index].accuracy = (byte)(37 + SpellTier(index) * 10);
						spell[index].routine = routine;
						spellMessages[index] = 0x1D; // Defenseless
						SPCR_SetPermissionFalse(spellPermissions, index, 3); // red mage banned
						SPCR_SetPermissionFalse(spellPermissions, index, 9); // red wizard banned
						rollDispel = false;
					}
					break; // if we reached the end without continuing, break the loop
				}
			}

			// create spell names (this has to be done separately), and also set graphics
			for(int i = 0; i < MagicCount; ++i)
			{
				if(spell[i].routine == 0x01)
				{
					if (spell[i].targeting == 0x01)
					{
						switch (spell[i].elem)
						{
							case 0b10000000:
								SPCR_SetName(spellNames, i, "METO", "MET");
								spell[i].gfx = 0xB8;
								spell[i].palette = 0x26;
								break;
							case 0b01000000:
								SPCR_SetName(spellNames, i, "LIT", "LIT");
								spell[i].gfx = 0xC8;
								spell[i].palette = 0x28;
								break;
							case 0b00100000:
								SPCR_SetName(spellNames, i, "ICE", "ICE");
								spell[i].gfx = 0xD0;
								spell[i].palette = 0x21;
								break;
							case 0b00010000:
								SPCR_SetName(spellNames, i, "FIRE", "FIR");
								spell[i].gfx = 0xD0;
								spell[i].palette = 0x26;
								break;
							case 0b00001000:
								SPCR_SetName(spellNames, i, "NECR", "NEC");
								spell[i].gfx = 0xD8;
								spell[i].palette = 0x20;
								break;
							case 0b00000100:
								SPCR_SetName(spellNames, i, "TIME", "TIM");
								spell[i].gfx = 0xD8;
								spell[i].palette = 0x2B;
								break;
							case 0b00000010:
								SPCR_SetName(spellNames, i, "GAS", "GAS");
								spell[i].gfx = 0xE8;
								spell[i].palette = 0x22;
								break;
							case 0b00000001:
								SPCR_SetName(spellNames, i, "PAIN", "PAN");
								spell[i].gfx = 0xE8;
								spell[i].palette = 0x29;
								break;
							case 0b00000000:
								if (i % 8 < 4)
								{
									if (SpellTier(i) < 5)
										SPCR_SetName(spellNames, i, "RAY", "RAY");
									else
										SPCR_SetName(spellNames, i, "FADE", "FAD");
									spell[i].gfx = 0xC8;
									spell[i].palette = 0x24;
								}
								else
								{
									if (SpellTier(i) < 5)
										SPCR_SetName(spellNames, i, "BOMB", "BOM");
									else
										SPCR_SetName(spellNames, i, "NUKE", "NUK");
									spell[i].gfx = 0xD0;
									spell[i].palette = 0x28;
								}
								break;
						}
					}
					else
					{
						switch (spell[i].elem)
						{
							case 0b10000000:
								SPCR_SetName(spellNames, i, "ROCK", "ROK");
								spell[i].gfx = 0xB8;
								spell[i].palette = 0x26;
								break;
							case 0b01000000:
								if (SpellTier(i) < 2)
									SPCR_SetName(spellNames, i, "LIT", "LIT");
								else
									SPCR_SetName(spellNames, i, "BOLT", "BLT");
								spell[i].gfx = 0xC8;
								spell[i].palette = 0x28;
								break;
							case 0b00100000:
								if (SpellTier(i) < 2)
									SPCR_SetName(spellNames, i, "ICE", "ICE");
								else
									SPCR_SetName(spellNames, i, "SNOW", "SNO");
								spell[i].gfx = 0xD0;
								spell[i].palette = 0x21;
								break;
							case 0b00010000:
								if (SpellTier(i) < 2)
									SPCR_SetName(spellNames, i, "FIRE", "FIR");
								else
									SPCR_SetName(spellNames, i, "BURN", "BRN");
								spell[i].gfx = 0xD0;
								spell[i].palette = 0x26;
								break;
							case 0b00001000:
								SPCR_SetName(spellNames, i, "ROT", "ROT");
								spell[i].gfx = 0xD8;
								spell[i].palette = 0x20;
								break;
							case 0b00000100:
								SPCR_SetName(spellNames, i, "RASP", "RSP");
								spell[i].gfx = 0xD8;
								spell[i].palette = 0x2B;
								break;
							case 0b00000010:
								SPCR_SetName(spellNames, i, "BIO", "BIO");
								spell[i].gfx = 0xE8;
								spell[i].palette = 0x22;
								break;
							case 0b00000001:
								SPCR_SetName(spellNames, i, "HURT", "HRT");
								spell[i].gfx = 0xE8;
								spell[i].palette = 0x29;
								break;
							case 0b00000000:
								if (i % 8 < 4)
								{
									if (SpellTier(i) < 5)
										SPCR_SetName(spellNames, i, "WORD", "WRD");
									else
										SPCR_SetName(spellNames, i, "HOLY", "HLY");
									spell[i].gfx = 0xC8;
									spell[i].palette = 0x24;
								}
								else
								{
									if (SpellTier(i) < 5)
										SPCR_SetName(spellNames, i, "MG.M", "M.M");
									else
										SPCR_SetName(spellNames, i, "FLAR", "FLA");
									spell[i].gfx = 0xD0;
									spell[i].palette = 0x28;
								}
								break;
						}
					}
				}
				if(spell[i].routine == 0x02)
				{
					spell[i].gfx = 0xC8;
					spell[i].palette = 0x21;
					SPCR_SetName(spellNames, i, "HARM", "HRM");
				}
				if(spell[i].routine == 0x03)
				{
					if((spell[i].effect & 0b10000000) != 0)
					{
						if(spell[i].effect == 0b10000000)
							SPCR_SetName(spellNames, i, "CONF", "CNF");
						else
							SPCR_SetName(spellNames, i, "CHRM", "CHM");
						spell[i].gfx = 0xE8;
						spell[i].palette = 0x26;
					}
					else if((spell[i].effect & 0b01000000) != 0)
					{
						spell[i].gfx = 0xE8;
						spell[i].palette = 0x2C;
						SPCR_SetName(spellNames, i, "MUTE", "MUT");
					}
					else if(spell[i].effect == 0b00001000)
					{
						spell[i].gfx = 0xE8;
						spell[i].palette = 0x23;
						SPCR_SetName(spellNames, i, "DARK", "DRK");
					}
					else if ((spell[i].effect & 0b00010000) != 0)
					{
						if(spell[i].elem == 0b00000001)
							SPCR_SetName(spellNames, i, "HOLD", "HLD");
						else if (spell[i].elem == 0b00000100)
							SPCR_SetName(spellNames, i, "STOP", "STP");
						else if (spell[i].elem == 0)
							SPCR_SetName(spellNames, i, "HALT", "HLT");
						else if (spell[i].elem == 0b10000000)
							SPCR_SetName(spellNames, i, "TNGL", "TGL");
						spell[i].gfx = 0xE8;
						spell[i].palette = 0x20;
					}
					else if (spell[i].effect == 0b00100000)
					{
						spell[i].gfx = 0xE8;
						spell[i].palette = 0x29;
						SPCR_SetName(spellNames, i, "SLEP", "SLP");
					}
					else if (spell[i].effect == 0b00000010)
					{
						if(spell[i].elem == 0b00100000)
						{
							spell[i].gfx = 0xD0;
							spell[i].palette = 0x21;
							SPCR_SetName(spellNames, i, "CRYO", "CRY");
						}
						else if(spell[i].elem == 0b00000010)
						{
							SPCR_SetName(spellNames, i, "BRAK", "BRK");
							spell[i].gfx = 0xC8;
							spell[i].palette = 0x20;
						}
					}
					else if (spell[i].effect == 0b00000001)
					{
						switch(spell[i].elem)
						{
							case 0b10000000:
								SPCR_SetName(spellNames, i, "QAKE", "QAK");
								spell[i].gfx = 0xB8;
								spell[i].palette = 0x26;
								break;
							case 0b01000000:
								SPCR_SetName(spellNames, i, "VOLT", "VLT");
								spell[i].gfx = 0xC8;
								spell[i].palette = 0x28;
								break;
							case 0b00001000:
								SPCR_SetName(spellNames, i, "RUB", "RUB");
								spell[i].gfx = 0xD8;
								spell[i].palette = 0x20;
								break;
							case 0b00000100:
								SPCR_SetName(spellNames, i, "ZAP!", "ZAP");
								spell[i].gfx = 0xD8;
								spell[i].palette = 0x2B;
								break;
							case 0b00000010:
								SPCR_SetName(spellNames, i, "BANE", "BAN");
								spell[i].gfx = 0xE8;
								spell[i].palette = 0x22;
								break;
							case 0b00000000:
								SPCR_SetName(spellNames, i, "DOOM", "DOM");
								spell[i].gfx = 0xD8;
								spell[i].palette = 0x21;
								break;
						}
					}
				}
				if (spell[i].routine == 0x04)
				{
					spell[i].gfx = 0xE8;
					spell[i].palette = 0x2A;
					SPCR_SetName(spellNames, i, "SLOW", "SLO");
				}
				if (spell[i].routine == 0x05)
				{
					SPCR_SetName(spellNames, i, "FEAR", "FEA");
					spell[i].gfx = 0xE8;
					spell[i].palette = 0x25;
				}
				if (spell[i].routine == 0x07 || spell[i].routine == 0x0F)
				{
					if (spell[i].targeting == 0x08)
						SPCR_SetName(spellNames, i, "HEAL", "HEL");
					else
						SPCR_SetName(spellNames, i, "CURE", "CUR");
				}
				if (spell[i].routine == 0x08)
				{
					if ((spell[i].effect & 0b01000000) != 0)
						SPCR_SetName(spellNames, i, "VOX", "VOX");
					else
						SPCR_SetName(spellNames, i, "PURE", "PUR");
					spell[i].gfx = 0xE0;
					spell[i].palette = 0x2C;
				}				
				if (spell[i].routine == 0x09)
				{
					if (spell[i].targeting == 0x04)
						SPCR_SetName(spellNames, i, "GARD", "GRD");
					else
						SPCR_SetName(spellNames, i, "FOG", "FOG");
					spell[i].gfx = 0xB0;
					spell[i].palette = 0x29;
				}
				if (spell[i].routine == 0x0A)
				{
					if (spell[i].effect == 0xFF)
						SPCR_SetName(spellNames, i, "WALL", "WAL");
					else if (spell[i].effect == 0b01000000)
						SPCR_SetName(spellNames, i, "ALIT", "ALT");
					else if (spell[i].effect == 0b00100000)
						SPCR_SetName(spellNames, i, "AICE", "AIC");
					else if (spell[i].effect == 0b00010000)
						SPCR_SetName(spellNames, i, "AFIR", "AFR");
					else if (spell[i].effect == 0b00000010)
						SPCR_SetName(spellNames, i, "APSN", "APS");
					else if (spell[i].effect == 0b00000001)
						SPCR_SetName(spellNames, i, "ASTA", "AST");
					else if (spell[i].effect == 0b01110000)
						SPCR_SetName(spellNames, i, "AMAG", "AMG");
					else if (spell[i].effect == 0b10001001)
						SPCR_SetName(spellNames, i, "ARUB", "ARB");
					else if (spell[i].effect == 0b10000101)
						SPCR_SetName(spellNames, i, "ATIM", "ATI");
					else
						SPCR_SetName(spellNames, i, "AMAG", "AMG");
					spell[i].gfx = 0xB0;
					spell[i].palette = 0x20;
				}
				if (spell[i].routine == 0x0C)
				{
					if(spell[i].targeting == 0x04)
						SPCR_SetName(spellNames, i, "QUIK", "QIK");
					else
						SPCR_SetName(spellNames, i, "FAST", "FST");
					spell[i].gfx = 0xB8;
					spell[i].palette = 0x2A;
				}
				if (spell[i].routine == 0x0D)
				{
					if(spell[i].targeting == 0x04)
					{
						spellMessages[i] = 0x1B;
						SPCR_SetName(spellNames, i, "SABR", "SBR");
					}
					else
					{
						spellMessages[i] = 0x0A;
						SPCR_SetName(spellNames, i, "TMPR", "TMP");
					}
						
					spell[i].gfx = 0xB8;
					spell[i].palette = 0x2A;
				}
				if (spell[i].routine == 0x0E)
				{
					spell[i].gfx = 0xB8;
					spell[i].palette = 0x28;
					SPCR_SetName(spellNames, i, "LOCK", "LOK");
				}
				if (spell[i].routine == 0x10)
				{
					if (spell[i].targeting == 0x04)
						SPCR_SetName(spellNames, i, "RUSE", "RUS");
					else
						SPCR_SetName(spellNames, i, "INVS", "INV");
					spell[i].gfx = 0xB0;
					spell[i].palette = 0x22;
				}
				if (spell[i].routine == 0x11)
				{
					spell[i].gfx = 0xB8;
					spell[i].palette = 0x20;
					SPCR_SetName(spellNames, i, "XFER", "XFR");
				}
				if (spell[i].routine == 0x12)
				{
					if ((spell[i].effect & 0b10000000) != 0)
					{
						spell[i].gfx = 0xE8;
						spell[i].palette = 0x26;
						SPCR_SetName(spellNames, i, "MUDL", "MDL");
					}				
					else if ((spell[i].effect & 0b01000000) != 0)
					{
						spell[i].gfx = 0xE8;
						spell[i].palette = 0x2C;
						SPCR_SetName(spellNames, i, "AVOX", "AVX");
					}	
					else if (spell[i].effect == 0b00001000)
					{
						spell[i].gfx = 0xE8;
						spell[i].palette = 0x23;
						SPCR_SetName(spellNames, i, "DARK", "DRK");
					}	
					else if ((spell[i].effect & 0b00010000) != 0)
					{
						spell[i].gfx = 0xE8;
						spell[i].palette = 0x20;
						SPCR_SetName(spellNames, i, "STUN", "STU");
					}
					else if (spell[i].effect == 0b00100000)
					{
						spell[i].gfx = 0xE8;
						spell[i].palette = 0x29;
						SPCR_SetName(spellNames, i, "SLEP", "SLP");
					}
					else if (spell[i].effect == 0b00000001)
					{
						switch (spell[i].elem)
						{
							case 0b10000000:
								SPCR_SetName(spellNames, i, "CRAK", "CRK");
								spell[i].gfx = 0xB8;
								spell[i].palette = 0x26;
								break;
							case 0b00001000:
								SPCR_SetName(spellNames, i, "XXXX", "XXX");
								spell[i].gfx = 0xD8;
								spell[i].palette = 0x28;
								break;
							case 0b00000000:
								SPCR_SetName(spellNames, i, "KILL", "KIL");
								spell[i].gfx = 0xD8;
								spell[i].palette = 0x21;
								break;
						}
					}
				}
			}

			// write all spell data to the ROM
			for (int i = 0; i < MagicCount; ++i)
			{
				Put(MagicOffset + MagicSize * i, spell[i].compressData());
				while (spellNames[i].Length < 4)
					spellNames[i] += " ";

				ItemsText[176 + i] = spellNames[i];
				spell[i].calc_Enemy_SpellTier();
			}

			if (!keepPermissions)
			{
				// write the permissions as one giant chunk
				SpellPermissions.ImportRawPermissions(spellPermissions);
			}

			Put(MagicTextPointersOffset, spellMessages); // write the spell messages as one giant chunk

			// modify spellcasting items
			// ensure the Defense Sword is turned into an item that increases evasion, or some other appropriate effect, otherwise it will cast a random level 3-5 white magic spell
			// ensure the Power Gauntlet is turned into an item that increases attack power on self (or single ally), or some other appropriate effect, otherwise it will cast a random level 3-5 black magic spell
			// all other items receive a random level 3-5 spell, either white magic or black magic, that is fit to cast in battle

			var Spells = GetSpells(); // we have to do it this way because what's the rest of the randomizer lol
			WriteItemSpellData(Spells[elemspell[1]], Item.MageRod); // write our FIR2 to the Mage Staff
			WriteItemSpellData(Spells[elemspell[3]], Item.BlackShirt); // write our ICE2 to the Black Shirt
			WriteItemSpellData(Spells[elemspell[5]], Item.ThorHammer); // write our LIT2 to the Thor Hammer
			WriteItemSpellData(Spells[healspell], Item.HealHelm); // write our HEAL to the Heal Helmet
			WriteItemSpellData(Spells[rusespell], Item.Defense); // write our RUSE to the Defense Sword
			WriteItemSpellData(Spells[sabrspell], Item.PowerGauntlets); // write our SABR tp the Power Gauntlets
			var goodspells = spellindex.Where(id => (spell[id].routine == 0x01 && spell[id].effect < 50 * spell[id].targeting && spell[id].effect > 20 * spell[id].targeting && spell[id].elem != 0) ||
				(spell[id].routine == 0x02 && spell[id].effect < 80) ||
				(spell[id].routine == 0x03 && (spell[id].effect & 0b11001011) != 0 && id < 48) ||
				spell[id].routine == 0x04 || (spell[id].routine == 0x07 && spell[id].targeting == 0x10) ||
				(spell[id].routine == 0x09 && id < 48) || (spell[id].routine == 0x0A && id < 56) ||
				spell[id].routine == 0x0E || (spell[id].routine == 0x10 && id < 48)).ToList();
			if(goodspells.Count < 6) // if we don't have enough spells for the remaining items, expand the eligibility criteria to include ALL spells except the guaranteed ones and those that have no effect
				goodspells = spellindex.Where(id => spell[id].routine != 0x00).ToList();
			goodspells.Shuffle(rng);
			WriteItemSpellData(Spells[goodspells[0]], Item.ZeusGauntlets);
			WriteItemSpellData(Spells[goodspells[1]], Item.HealRod);
			WriteItemSpellData(Spells[goodspells[2]], Item.LightAxe);
			WriteItemSpellData(Spells[goodspells[3]], Item.WizardRod);
			WriteItemSpellData(Spells[goodspells[4]], Item.BaneSword);
			goodspells = goodspells.Skip(5).ToList();
			var goodcolorspells = goodspells.Where(id => WhiteSpell(id) && SpellTier(id) > 3).ToList();
			if (goodcolorspells.Count() > 0)
				WriteItemSpellData(Spells[goodcolorspells.PickRandom(rng)], Item.WhiteShirt);
			else
				WriteItemSpellData(Spells[goodspells[0]], Item.WhiteShirt);

			// fill enemy scripts with tier-equivalent skills (using enemizer's calc_Enemy_SpellTier feature)
			// we use special rules for Warmech, Fiend, and Astos scripts

			for (int i = 0; i < enemyScripts.Count() - 10; ++i) // exclude the last 10 scripts
			{
				// start replacing each spell with another spell from the same tier
				for (byte j = 0; j < 8; ++j)
				{
					if (enemyScripts[i].spell_list[j] == 0xFF)
						continue; // skip blank spells
					int whichTier = oldTiers[enemyScripts[i].spell_list[j]];
					if (whichTier == 0)
						whichTier = 1; // don't allow tier 0s to exist
					List<byte> eligibleSpellIDs = new List<byte> { };
					while(eligibleSpellIDs.Count == 0 && whichTier > 0)
					{
						for (byte k = 0; k < 64; ++k)
						{
							if (spell[k].tier == whichTier)
								eligibleSpellIDs.Add(k);
						}
						--whichTier;
					}
					if (eligibleSpellIDs.Count == 0)
						eligibleSpellIDs.Add(4); // force FIRE if no other spell is available for some reason
					enemyScripts[i].spell_list[j] = eligibleSpellIDs.PickRandom(rng);
				}
			}
			spellindex = Enumerable.Range(0, 64).ToList(); // refilling the spell indexes to include all spells again
			var middamagespells = spellindex.Where(id => spell[id].routine == 0x01 && spell[id].tier == 3).ToList(); // this will include some of the guaranteed spells, so there will always be entries
			var highdamagespells = spellindex.Where(id => spell[id].routine == 0x01 && spell[id].tier == 4).ToList();
			if (highdamagespells.Count() == 0)
				highdamagespells = middamagespells.ToList(); // if there are no tier 4 damage spells, use tier 3s instead
			var statusspells = spellindex.Where(id => (spell[id].routine == 0x03 || spell[id].routine == 0x12) && (spell[id].effect & 0b00000011) == 0 && spell[id].tier <= 3 && spell[id].tier > 0).ToList();
			if (statusspells.Count() == 0)
				statusspells = spellindex.Where(id => spell[id].routine == 0x01 && spell[id].tier > 0 && spell[id].tier < 3).ToList(); // this includes guarantees so it is fine
			var instakills = spellindex.Where(id => (spell[id].routine == 0x03 || spell[id].routine == 0x12) && (spell[id].effect & 0b00000011) != 0 && spell[id].tier != 5).ToList();
			if (instakills.Count() == 0)
				instakills = middamagespells.Union(highdamagespells).ToList();
			var single_instas = instakills.Where(id => spell[id].targeting == 0x02).ToList();
			if (single_instas.Count() == 0)
				single_instas = middamagespells.ToList();
			var multi_instas = instakills.Where(id => spell[id].targeting == 0x01).ToList();
			if (multi_instas.Count() == 0)
				multi_instas = highdamagespells.ToList();
			var tier5 = spellindex.Where(id => spell[id].tier == 5).ToList();
			int highesttier = 7;
			var toptierblack = spellindex.Where(id => BlackSpell(id) && id >= highesttier * 8 && spell[id].tier > 0).Except(tier5).ToList();
			while (toptierblack.Count() < 3 && highesttier > 0)
			{
				toptierblack = spellindex.Where(id => BlackSpell(id) && id >= highesttier * 8 && spell[id].tier > 0).Except(tier5).ToList();
				--highesttier;
			}
			if (tier5.Count() == 0)
				tier5 = highdamagespells.ToList();
			if (toptierblack.Count() == 0)
				toptierblack = highdamagespells.ToList();
			var slowingspell = spellindex.Where(id => spell[id].routine == 0x04).ToList(); // this will always include the guaranteed slow spell

			// Lich 1 script
			enemyScripts[34].spell_list[0] = (byte)middamagespells.PickRandom(rng);
			enemyScripts[34].spell_list[1] = (byte)statusspells.PickRandom(rng);
			enemyScripts[34].spell_list[2] = (byte)fastspell;
			enemyScripts[34].spell_list[3] = (byte)middamagespells.PickRandom(rng);
			enemyScripts[34].spell_list[4] = (byte)statusspells.PickRandom(rng);
			enemyScripts[34].spell_list[5] = (byte)middamagespells.PickRandom(rng);
			enemyScripts[34].spell_list[6] = (byte)slowingspell.PickRandom(rng);
			enemyScripts[34].spell_list[7] = (byte)statusspells.PickRandom(rng);
			// Lich 2 script
			enemyScripts[35].spell_list[0] = (byte)tier5.PickRandom(rng);
			enemyScripts[35].spell_list[1] = (byte)toptierblack.PickRandom(rng);
			enemyScripts[35].spell_list[2] = (byte)toptierblack.PickRandom(rng);
			enemyScripts[35].spell_list[3] = (byte)toptierblack.PickRandom(rng);
			enemyScripts[35].spell_list[4] = (byte)tier5.PickRandom(rng);
			enemyScripts[35].spell_list[5] = (byte)toptierblack.PickRandom(rng);
			enemyScripts[35].spell_list[6] = (byte)toptierblack.PickRandom(rng);
			enemyScripts[35].spell_list[7] = (byte)toptierblack.PickRandom(rng);
			// Kary 1 script
			enemyScripts[36].spell_list[0] = (byte)middamagespells.PickRandom(rng);
			enemyScripts[36].spell_list[1] = (byte)statusspells.PickRandom(rng);
			enemyScripts[36].spell_list[2] = (byte)middamagespells.PickRandom(rng);
			enemyScripts[36].spell_list[3] = (byte)statusspells.PickRandom(rng);
			enemyScripts[36].spell_list[4] = (byte)middamagespells.PickRandom(rng);
			enemyScripts[36].spell_list[5] = (byte)statusspells.PickRandom(rng);
			enemyScripts[36].spell_list[6] = (byte)middamagespells.PickRandom(rng);
			enemyScripts[36].spell_list[7] = (byte)statusspells.PickRandom(rng);
			// Kary 2 script
			enemyScripts[37].spell_list[0] = (byte)highdamagespells.PickRandom(rng);
			enemyScripts[37].spell_list[1] = (byte)single_instas.PickRandom(rng);
			enemyScripts[37].spell_list[2] = (byte)highdamagespells.PickRandom(rng);
			enemyScripts[37].spell_list[3] = (byte)statusspells.PickRandom(rng);
			enemyScripts[37].spell_list[4] = (byte)highdamagespells.PickRandom(rng);
			enemyScripts[37].spell_list[5] = (byte)single_instas.PickRandom(rng);
			enemyScripts[37].spell_list[6] = (byte)highdamagespells.PickRandom(rng);
			enemyScripts[37].spell_list[7] = (byte)statusspells.PickRandom(rng);
			// Kraken 2 script
			for (int i = 0; i < 8; ++i)
				enemyScripts[39].spell_list[i] = (byte)middamagespells.PickRandom(rng);
			// Tiamat 2 script
			for (int i = 0; i < 8; i += 4)
			{
				enemyScripts[41].spell_list[i] = (byte)multi_instas.PickRandom(rng);
				enemyScripts[41].spell_list[i + 1] = (byte)highdamagespells.PickRandom(rng);
				enemyScripts[41].spell_list[i + 2] = (byte)highdamagespells.PickRandom(rng);
				enemyScripts[41].spell_list[i + 3] = (byte)highdamagespells.PickRandom(rng);
			}
			// Chaos script
			enemyScripts[42].spell_list[0] = (byte)highdamagespells.PickRandom(rng);
			enemyScripts[42].spell_list[1] = (byte)highdamagespells.PickRandom(rng);
			enemyScripts[42].spell_list[2] = (byte)slowspell;
			enemyScripts[42].spell_list[3] = (byte)cur4spell;
			enemyScripts[42].spell_list[4] = (byte)highdamagespells.PickRandom(rng);
			enemyScripts[42].spell_list[5] = (byte)highdamagespells.PickRandom(rng);
			enemyScripts[42].spell_list[6] = (byte)fastspell;
			enemyScripts[42].spell_list[7] = (byte)tier5.PickRandom(rng);
			// Astos script
			enemyScripts[43].spell_list[0] = (byte)single_instas.PickRandom(rng);
			enemyScripts[43].spell_list[1] = (byte)slowspell;
			enemyScripts[43].spell_list[2] = (byte)fastspell;
			enemyScripts[43].spell_list[3] = (byte)middamagespells.PickRandom(rng);
			enemyScripts[43].spell_list[4] = (byte)middamagespells.PickRandom(rng);
			enemyScripts[43].spell_list[5] = (byte)statusspells.PickRandom(rng);
			enemyScripts[43].spell_list[6] = (byte)statusspells.PickRandom(rng);
			enemyScripts[43].spell_list[7] = (byte)statusspells.PickRandom(rng);
		}

		private void SPCR_SetName(string[] spellnames, int index, string initialname, string altname)
		{
			if(spellnames[MagicCount] == "L")
			{
				spellnames[index] = altname + ((index >> 3) + 1).ToString();
			}
			else
			{
				if (SPCR_ContainsName(spellnames, initialname))
				{
					int i = 2;
					while (SPCR_ContainsName(spellnames, altname + i.ToString()))
						++i;
					spellnames[index] = altname + i.ToString();
				}
				else
					spellnames[index] = initialname;
			}
		}

		private bool SPCR_ContainsName(string[] spellnames, string check)
		{
			foreach(string name in spellnames)
			{
				if (name == check)
					return true;
			}
			return false;
		}

		public void SPCR_SetPermissionFalse(byte[] spellpermissions, int index, int job)
		{
			spellpermissions[(job << 3) | (index >> 3)] |= (byte)(0b10000000 >> (index & 0b00000111));
		}

		public void SPCR_CraftDamageSpell(MT19337 rng, SpellInfo spell, int tier, byte element, bool whiteMagic, bool forceAoE)
		{
			if (forceAoE)
				spell.targeting = 0x01;
			else
			{
				if (tier < 2)
					spell.targeting = 0x02;
				else
					spell.targeting = (byte)(rng.Between(0, 2) == 0 ? 0x02 : 0x01);
			}
			if(spell.targeting == 0x01)
			{
				if((element & 0b11111011) != 0)
				{
					switch(tier)
					{
						case 2:
							spell.effect = (byte)rng.Between(30, 36);
							break;
						case 3:
							spell.effect = (byte)rng.Between(40, 48);
							break;
						case 4:
							spell.effect = (byte)rng.Between(50, 62);
							break;
						case 5:
							spell.effect = (byte)rng.Between(70, 84);
							break;
						case 6:
							spell.effect = (byte)rng.Between(90, 110);
							break;
						case 7:
							spell.effect = (byte)rng.Between(120, 150);
							break;
					}
					spell.accuracy = 24;
				}
				else if (element == 0b00000100)
				{
					switch(tier)
					{
						case 2:
							spell.effect = (byte)rng.Between(25, 30);
							break;
						case 3:
							spell.effect = (byte)rng.Between(35, 40);
							break;
						case 4:
							spell.effect = (byte)rng.Between(48, 52);
							break;
						case 5:
							spell.effect = (byte)rng.Between(60, 72);
							break;
						case 6:
							spell.effect = (byte)rng.Between(75, 88);
							break;
						case 7:
							spell.effect = (byte)rng.Between(100, 130);
							break;
					}
					spell.accuracy = 64;
				}
				else
				{
					switch (tier)
					{
						case 2:
							spell.effect = 24;
							break;
						case 3:
							spell.effect = 32;
							break;
						case 4:
							spell.effect = 46;
							break;
						case 5:
							spell.effect = 60;
							break;
						case 6:
							spell.effect = 80;
							break;
						case 7:
							spell.effect = (byte)rng.Between(90, 120);
							break;
					}
					if (tier < 5)
						spell.accuracy = 40;
					else
						spell.accuracy = 107;
				}
			}
			else
			{
				if ((element & 0b11111011) != 0)
				{
					switch (tier)
					{
						case 0:
							spell.effect = (byte)rng.Between(12, 18);
							break;
						case 1:
							spell.effect = (byte)rng.Between(25, 32);
							break;
						case 2:
							spell.effect = (byte)rng.Between(50, 56);
							break;
						case 3:
							spell.effect = (byte)rng.Between(60, 66);
							break;
						case 4:
							spell.effect = (byte)rng.Between(80, 90);
							break;
						case 5:
							spell.effect = (byte)rng.Between(100, 110);
							break;
						case 6:
							spell.effect = (byte)rng.Between(120, 130);
							break;
						case 7:
							spell.effect = (byte)rng.Between(175, 200);
							break;
					}
					spell.accuracy = 48;
				}
				else
				{
					switch (tier)
					{
						case 0:
							spell.effect = 10;
							break;
						case 1:
							spell.effect = 20;
							break;
						case 2:
							spell.effect = 40;
							break;
						case 3:
							spell.effect = 50;
							break;
						case 4:
							spell.effect = 64;
							break;
						case 5:
							spell.effect = 80;
							break;
						case 6:
							spell.effect = 108;
							break;
						case 7:
							spell.effect = 152;
							break;
					}
					if (tier > 4 && spell.elem == 0)
						spell.accuracy = 107;
					else
						spell.accuracy = 64;
				}
			}
			if (whiteMagic)
			{
				spell.effect = (byte)(spell.effect - spell.effect / 4);
			}
			if (tier == 1)
			{
				if (element < 0b0001000 && element > 0 && rng.Between(0, 1) == 0)
				{
					spell.effect *= 7;
					spell.effect /= 10;
					spell.targeting = 0x01;
					spell.accuracy = 24;
				}
			}
				
			spell.routine = 0x01;
			spell.elem = element;
		}

		public void SPCR_CraftHarmSpell(SpellInfo spell, int tier)
		{

			spell.accuracy = 40;
			spell.effect = (byte)(20 * (tier + 1));
			if (tier == 7)
				spell.effect = (byte)(spell.effect + spell.effect / 2);
			spell.targeting = 0x01;
			spell.routine = 0x02;
		}

		public void SPCR_CraftEvasionSpell(MT19337 rng, SpellInfo spell, int tier, byte targeting)
		{
			spell.targeting = targeting;
			if (tier == 7)
				spell.targeting = 0x04; // use singletargeting effect for all tier 8 spells
			spell.effect = (byte)(spell.targeting == 0x08 ? tier * 10 : (tier + 2) * 12);
			if (tier > 3 && targeting != 0x08)
				spell.effect += 12;
			if (spell.targeting == 0x04)
				spell.effect = (byte)(spell.effect + spell.effect / 2); // increase buff by 50% for self-cast
			if (tier == 7)
				spell.targeting = 0x08; // tier 8 spells target all allies with +240 evasion
			spell.routine = 0x10;
		}

		public void SPCR_CraftAttackUpSpell(MT19337 rng, SpellInfo spell, int tier, bool forceSelfTarget)
		{
			if (forceSelfTarget)
				spell.targeting = 0x04;
			else if (tier < 3)
				spell.targeting = (byte)(rng.Between(0, 2) == 0 ? 0x10 : 0x04);
			else if (tier == 7)
				spell.targeting = 0x10;
			else
			{
				int choice = rng.Between(0, 8);
				if (choice < 3)
					spell.targeting = 0x04;
				else if (choice < 6)
					spell.targeting = 0x10;
				else
					spell.targeting = 0x08;
			}
			spell.effect = (byte)(spell.targeting == 0x08 ? tier * 3 - 2 : (tier + 2) * 3 + 1);
			if (spell.targeting == 0x04)
				spell.effect = (byte)(spell.effect + spell.effect / 2);
			if (tier == 7)
				spell.targeting = 0x08; // tier 8 targets all allies with the strength of a SABR spell
			spell.routine = 0x0D;
		}

		public void SPCR_CraftFastSpell(SpellInfo spell, int tier)
		{
			if (tier < 3)
				spell.targeting = 0x04;
			else if (tier < 6)
				spell.targeting = 0x10;
			else
				spell.targeting = 0x08;
			spell.routine = 0x0C;
		}

		public byte GenerateRandomBits(byte input, MT19337 rng)
		{
			// generates a random byte with the flags of the input determining whether the bit is rolled or is always 0
			byte returnValue = 0x00;
			for(int i = 0; i < 8; ++i)
			{
				if(rng.Between(0, 1) == 1)
					returnValue |= (byte)(input & 1 << i);
			}
			return returnValue;
		}

		public byte PickSingleBit(byte input, MT19337 rng)
		{
			if (input == 0x00)
				return 0; // return zero if the input is zero
			// picks a random single bit from the input bits
			byte returnValue = 0x00;
			while(returnValue == 0x00)
			{
				returnValue = (byte)(input & 1 << rng.Between(0, 7));
			}
			return returnValue;
		}

		public int CountBitsActive(byte input)
		{
			// counts the number of bits turned on
			int returnValue = 0;
			for(int i = 0; i < 8; ++i)
			{
				if ((input & 1 << i) != 0)
					returnValue++;
			}
			return returnValue;
		}
	}
}
