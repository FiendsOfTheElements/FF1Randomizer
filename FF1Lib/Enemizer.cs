using System.ComponentModel;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace FF1Lib
{
	public partial class FF1Rom : NesRom
	{
		public const int GenericTilesetsCount = 13;
		public const int EnemySkillCount = 26;
		public const int EnemySkillSize = 8; // even though the actual information is only five bytes, each entry in the game ROM uses three extra bytes that are all 00
		public const int EnemySkillOffset = 0x303F0;
		public const int EnemySkillTextPointerOffset = 0x2B600;
		public const int EnemySkillTextPointerBase = 0x20000;
		public const int EnemySkillTextOffset = 0x2B634;
		public const int EnemyPatternTablesOffset = 0x1C000;

		public const int EnemizerUnrunnabilityWeight = 31; // weight given to determine unrunnability.  for the highest level of encounters, this means 8/32 probability of a formation being unrunnable

		private const int ZoneFormationsOffset = 0x2C000;
		private const int ZoneFormationsSize = 8;
		private const int ZoneCount = 128;

		enum MonsterPerks
		{
			PERK_GAINSTAT10, // increases a minor stat by 10%, +2% XP
			PERK_LOSESTAT10, // reduces a minor stat by 10%, -2% XP
			PERK_LOWRESIST, // adds a low resist (fire/ice/lit/earth), +4% XP
			PERK_HIGHRESIST, // adds a high resist (status/poison/time/death), +3% XP
			PERK_LOWWEAKNESS, // adds a low weakness, -5% XP
			PERK_HIGHWEAKNESS, // adds a high weakness, -3% XP
			PERK_PLUSONEHIT, // adds an extra hit, +5% XP for 2hitter, +3% for 3hitter, +2% for 4hitter and +1% for any hits beyond that
			PERK_POISONTOUCH, // adds poisontouch if no status ailment already exists
			PERK_STUNSLEEPTOUCH, // adds stun or sleep touch, +3% XP if stun touch is selected
			PERK_MUTETOUCH, // adds mute touch, +2% XP
			NUM_PERKS // total number of perks that are available for seletion
		};

		enum TrapTiles
		{
			TRAP_IMAGES,
			TRAP_MUMMIES,
			TRAP_MUDGOLS,
			TRAP_NITEMARES,
			TRAP_ZOMBIE_D,
			TRAP_GARGOYLES,
			TRAP_SEAFOOD_PARTY_MIX,
			TRAP_SHARKNADO,
			TRAP_WATERS,
			TRAP_SEASHRINE_MUMMIES,
			TRAP_ZOMBIES,
			TRAP_GARGOYLES2,
			TRAP_GIANTS,
			TRAP_GIANTS_IGUANAS,
			TRAP_EARTH,
			TRAP_OGRES_HYENAS,
			TRAP_SPHINX,
			TRAP_FIRE,
			TRAP_GREYWORM,
			TRAP_AGAMA,
			TRAP_RED_D,
			TRAP_UNDEAD_PARTYMIX,
			TRAP_FROSTRURUS,
			TRAP_FROSTGIANT,
			TRAP_MAGES,
			TRAP_FROST_D,
			TRAP_EYE,
			TRAP_WATERFALL_MUMMIES,
			TRAP_WIZARDS,
			TRAP_WIZARDS2,
			TRAP_COBRAS,
			TRAP_BLUE_D,
			TRAP_SLIMES,
			TRAP_PHANTOM,
			TRAP_LICH2,
			TRAP_KARY2,
			TRAP_KRAKEN2,
			TRAP_TIAMAT2,
			NUM_TRAP_TILES
		};

		public class SpellInfo
		{
			public byte accuracy = 0;
			public byte effect = 0;
			public byte elem = 0;
			public byte targeting = 0;
			public byte routine = 0;
			public byte gfx = 0;
			public byte palette = 0;
			public int tier = 0;

			public byte[] compressData()
			{
				byte[] spellInfo = new byte[8];
				spellInfo[0] = accuracy;
				spellInfo[1] = effect;
				spellInfo[2] = elem;
				spellInfo[3] = targeting;
				spellInfo[4] = routine;
				spellInfo[5] = gfx;
				spellInfo[6] = palette;
				spellInfo[7] = 0x00; // last byte is always 00
				return spellInfo;
			}

			public void decompressData(byte[] data)
			{
				if (data.Length != MagicSize)
				{
					accuracy = 0x00;
					effect = 0x00;
					elem = 0x00;
					targeting = 0x00;
					routine = 0x00;
					gfx = 0x00;
					palette = 0x00;
					tier = 0;
				}
				else
				{
					accuracy = data[0];
					effect = data[1];
					elem = data[2];
					targeting = data[3];
					routine = data[4];
					gfx = data[5];
					palette = data[6];
				}
			}

			public void calc_Enemy_SpellTier() // calculates the usefulness of a spell (from the perspective of a generic random encounter enemy)
			{
				tier = 0; // if no value is assigned by this routine, assume the spell is worthless
				if (routine == 0x01) // inflict damage
				{
					if (targeting == 0x01) // all enemies
					{
						if (elem == 0) // non-elemental tiers
						{
							if (effect < 8)
								tier = 1;
							else if (effect < 16)
								tier = 2;
							else if (effect < 36)
								tier = 3;
							else if (effect < 68)
								tier = 4;
							else
								tier = 5;
						}
						else if ((elem & 0b01111000) != 0) // fire/ice/lit/death elemental - these elements are the easiest to find resistance against so they are the lowest value
						{
							if (effect < 12)
								tier = 1;
							else if (effect < 25)
								tier = 2;
							else if (effect < 50)
								tier = 3;
							else if (effect < 140)
								tier = 4;
							else
								tier = 5;
						}
						else if ((elem & 0b10000111) != 0) // earth/time/poison/status element - these elements are harder for the player to resist so their value is increased
						{
							if (effect < 10)
								tier = 1;
							else if (effect < 20)
								tier = 2;
							else if (effect < 50)
								tier = 3;
							else if (effect < 140)
								tier = 4;
							else
								tier = 5;
						}
					}
					else if (targeting == 0x02) // single enemy
					{
						if (elem == 0) // non-elemental tiers
						{
							if (effect < 15)
								tier = 1;
							else if (effect < 30)
								tier = 2;
							else if (effect < 60)
								tier = 3;
							else if (effect < 120)
								tier = 4;
							else
								tier = 5;
						}
						else if ((elem & 0b01111000) != 0) // fire/ice/lit/death elemental - these elements are the easiest to find resistance against so they are the lowest value
						{
							if (effect < 20)
								tier = 1;
							else if (effect < 45)
								tier = 2;
							else if (effect < 90)
								tier = 3;
							else if (effect < 180)
								tier = 4;
							else
								tier = 5;
						}
						else if ((elem & 0b10000111) != 0) // earth/time/poison/status element - these elements are harder for the player to resist so their value is increased
						{
							if (effect < 20)
								tier = 1;
							else if (effect < 40)
								tier = 2;
							else if (effect < 80)
								tier = 3;
							else if (effect < 160)
								tier = 4;
							else
								tier = 5;
						}
					}
				}
				if (routine == 0x02)
					tier = 0; // HARM spells are always tier 0
				if (routine == 0x03 || routine == 0x12) // negative status effect OR power word spells (both are judged by the same criteria)
				{
					if ((effect & 0b11) != 0) // death/stone overrides all other statuses
					{
						if (targeting == 0x01) // all enemies
						{
							if (elem == 0) // non-elemental tiers
								tier = 5; // all non-blockable instakills that target all are tier 5
							else
								tier = 4; // otherwise they are tier 4
						}
						else if (targeting == 0x02) // single enemy
						{
							if (elem == 0) // non-elemental tiers
							{
								tier = 5; // non-elemental instakills are always considered tier 5
							}
							else // elemental single targets are tier 3 if they have low accuracy, and tier 4 if their accuracy is decent
							{
								if (accuracy < 32 && routine != 0x12)
									tier = 3;
								else
									tier = 4;
							}
						}
					}
					else if ((effect & 0b00010000) != 0) // if no death/stone, then paralysis is the next effect and doesn't care about other effects
					{
						if (targeting == 0x01) // all enemies
						{
							tier = 4;
						}
						else if (targeting == 0x02) // single enemy
						{
							if (elem == 0) // non-elemental tiers
								tier = 4;
							else
								tier = 3; // paralysis spells are always at least tier 3
						}
					}
					else if ((effect & 0b01000000) != 0) // next is mute
					{
						if (targeting == 0x01) // all enemies
						{
							if (elem == 0) // non-elemental tiers
								tier = 3; // all non-elemental mute is tier 3
							else if ((elem & 0b01111000) != 0) // fire/ice/lit/death elemental - these elements are the easiest to find resistance against so they are the lowest value
							{
								if (routine == 0x12)
									tier = 3;
								else
									tier = 2;
							}
							else if ((elem & 0b10000111) != 0) // earth/time/poison/status element - these elements are harder for the player to resist so their value is increased
							{
								if (routine == 0x12)
									tier = 3;
								else
									tier = 2;
							}
						}
						else if (targeting == 0x02) // single enemy
						{
							if (elem == 0) // non-elemental tiers
							{
								if (routine == 0x12)
									tier = 3;
								else
									tier = 2;
							}
							else
								tier = 1;
						}
					}
					else if ((effect & 0b00100000) != 0) // then sleep
					{
						if (targeting == 0x01) // all enemies
							tier = 2;
						else if (targeting == 0x02) // single enemy
							tier = 1;
					}
					else if ((effect & 0b00001000) != 0) // darkness rates lowest of the useful effects
						tier = 1;
					else
						tier = 0; // and the AI doesn't care about inflicting poison or confusion through spells
				}
				if(routine == 0x04) // decrease speed (SLOW)
				{
					if (elem == 0) // only non-elemental slow gets preferential treatment, and it doesn't care about targeting
					{
						if (accuracy >= 48)
							tier = 3;
						else
							tier = 2;
					}
					else
						tier = 2;
				}
				if(routine == 0x07 || routine == 0x06) // HP Up (CURE, HEAL)
				{
					if (targeting == 0x04) // single caster
					{
						if (effect < 10)
							tier = 0;
						else if (effect < 30)
							tier = 1;
						else if (effect < 60)
							tier = 2;
						else if (effect < 120)
							tier = 3;
						else
							tier = 4;
					}
					else if (targeting == 0x08) // all party
					{
						if (effect < 10)
							tier = 0;
						else if (effect < 20)
							tier = 1;
						else if (effect < 40)
							tier = 2;
						else if (effect < 80)
							tier = 3;
						else
							tier = 4;
					}
					else if (targeting == 0x10)
					{
						if (effect < 10)
							tier = 0;
						else if (effect < 30)
							tier = 1;
						else if (effect < 60)
							tier = 2;
						else if (effect < 120)
							tier = 3;
						else
							tier = 4;
					}
				}
				if(routine == 0x08) // neutralize status
				{
					if ((effect & 0b0000_0001) != 0)
					{
						tier = 0; // if Life In Battle is enabled,
					}
					else if (targeting == 0x08)
					{
						if ((effect & 0b11010000) != 0)
							tier = 2; // removing confuse, mute, or stun on the party is a tier 2
					}
					else if (targeting == 0x10)
					{
						if ((effect & 0b11010000) != 0)
							tier = 1; // removing confuse, mute, or stun on a single target is a tier 1
					}
				}
				if(routine == 0x09) // armor up
				{
					if (targeting == 0x04)
					{
						if (effect < 8)
							tier = 0;
						else if (effect < 16)
							tier = 1;
						else if (effect < 24)
							tier = 2;
						else if (effect < 40)
							tier = 3;
						else
							tier = 4;
					}
					else if (targeting == 0x08)
					{
						if (effect < 4)
							tier = 0;
						else if (effect < 8)
							tier = 1;
						else if (effect < 16)
							tier = 2;
						else if (effect < 30)
							tier = 3;
						else
							tier = 4;
					}
					else if (targeting == 0x10)
					{
						if (effect < 8)
							tier = 0;
						else if (effect < 16)
							tier = 1;
						else if (effect < 24)
							tier = 2;
						else if (effect < 40)
							tier = 3;
						else
							tier = 4;
					}
				}
				if(routine == 0x0A) // resist element
				{
					if(targeting == 0x04) // self caster: resist all elements is tier 3, all other elements are tier 1
					{
						if (effect == 0xFF)
							tier = 3;
						else if (effect != 0x00)
							tier = 1;
					}
					if (targeting == 0x08) // whole party: resist 6+ elements is tier 4, three resists or two base resists is tier 3, otherwise tier 2 unless no element
					{
						int baseresists = 0;
						int resists = 0;
						for (int i = 1; i < 4; ++i)
						{
							baseresists += (elem & (0b10000000 >> i)) != 0 ? 1 : 0;
							resists += (elem & (0b10000000 >> i)) != 0 ? 1 : 0;
						}
						resists += (elem & 0b10000000) != 0 ? 1 : 0;
						for (int i = 4; i < 8; ++i)
						{
							resists += (elem & (0b10000000 >> i)) != 0 ? 1 : 0;
						}
						if (resists > 5)
							tier = 4;
						else if (baseresists > 1 || resists > 2)
							tier = 3;
						else if (resists > 0)
							tier = 2;
					}
					else if (targeting == 0x10) // single target: resist all elements is tier 4, all other elements are tier 2
					{
						if (effect == 0xFF)
							tier = 4;
						else if (effect != 0x00)
							tier = 2;
					}
					else
						tier = 0; // spells which assist the enemy are useless
				}
				if(routine == 0x0C) // FAST
				{
					if (targeting == 0x04 || targeting == 0x10)
						tier = 3; // tier 3 fast is fair for a regular monster
					else if (targeting == 0x08)
						tier = 4; // multi-target fast is tier 4 though
				}
				if(routine == 0x0D) // attack up
				{
					if (targeting == 0x04 || targeting == 0x10)
					{
						if (effect < 6)
							tier = 0;
						else if (effect < 12)
							tier = 1;
						else if (effect < 20)
							tier = 2;
						else if (effect < 35)
							tier = 3;
						else
							tier = 4;
					}
					else if (targeting == 0x08)
					{
						if (effect < 4)
							tier = 0;
						else if (effect < 12)
							tier = 2;
						else if (effect < 20)
							tier = 3;
						else
							tier = 4;
					}
				}
				if(routine == 0x0E) // reduce evasion (LOCK)
				{
					if (targeting == 0x04 || targeting == 0x08 || targeting == 0x10)
					{
						if (effect == 0)
							tier = 0;
						else if (effect < 25)
							tier = 1;
						else if (effect < 80)
							tier = 2;
						else if (elem == 0x00)
							tier = 3; // only allow tier 3 for extremely strong locks with no element
					}
				}
				if(routine == 0x0F) // HP Max (CUR4)
				{
					tier = 4; // CUR4 is always tier 4 from an enemy's perspective
				}
				if(routine == 0x10) // increase evasion (RUSE, INVS)
				{
					if (targeting == 0x04 || targeting == 0x10)
					{
						if (effect == 0)
							tier = 0;
						else if (effect < 25)
							tier = 1;
						else if (effect < 80)
							tier = 2;
						else
							tier = 3; // only allow tier 3 for extremely strong evasion
					}
					else if (targeting == 0x08)
					{
						if (effect == 0)
							tier = 0;
						else if (effect < 25)
							tier = 2;
						else if (effect < 80)
							tier = 3;
						else
							tier = 4;
					}
				}
				if(routine == 0x11) // remove resistance (XFER)
				{
					if (targeting == 0x04 || targeting == 0x08 || targeting == 0x10)
					{
						if (elem == 0) // we only care about the element, it can target whatever it likes as long as it isn't a friendly
							tier = 4;
						else
							tier = 3;
					}
				}
			}
		}

		public class EnemySkillInfo
		{
			public byte accuracy = 0;
			public byte effect = 0;
			public byte elem = 0;
			public byte targeting = 0;
			public byte routine = 0;
			public int tier = 0;

			public byte[] compressData()
			{
				byte[] spellInfo = new byte[8];
				spellInfo[0] = accuracy;
				spellInfo[1] = effect;
				spellInfo[2] = elem;
				spellInfo[3] = targeting;
				spellInfo[4] = routine;
				spellInfo[5] = 0x00; // last three bytes are always 0x00
				spellInfo[6] = 0x00;
				spellInfo[7] = 0x00;
				return spellInfo;
			}

			public void decompressData(byte[] data)
			{
				if (data.Length != EnemySkillSize)
				{
					accuracy = 0x00;
					effect = 0x00;
					elem = 0x00;
					targeting = 0x00;
					routine = 0x00;
				}
				else
				{
					accuracy = data[0];
					effect = data[1];
					elem = data[2];
					targeting = data[3];
					routine = data[4];
				}
			}
		}

		public class EnemyInfo
		{
		    public int index;
		    public string name;
			public int exp;
			public int gp;
			public int hp;
			public int morale;

		    [JsonIgnoreAttribute]
			public byte AIscript;


		    public EnemyScriptInfo spellSkillScript {
			get {
			    if (AIscript == 0xff) {
				return null;
			    }
			    return allAIScripts[AIscript];
			}
		    }
			public int agility;
			public int absorb;
			public int num_hits;
			public int accuracy;
			public int damage;
			public int critrate;

		    [JsonIgnoreAttribute]
			public byte atk_elem;

		    [JsonConverter(typeof(StringEnumConverter))]
		    public SpellElement AttackElement {
			get {
			    return (SpellElement)atk_elem;
			}
		    }

		    [JsonIgnoreAttribute]
			public byte atk_ailment;


		    [JsonConverter(typeof(StringEnumConverter))]
		    public SpellStatus AttackAilment {
			get {
			    return (SpellStatus)atk_ailment;
			}
		    }

		    [JsonIgnoreAttribute]
			public byte monster_type;

		    [JsonConverter(typeof(StringEnumConverter))]
		    public MonsterType MonsterType {
			get {
			    return (MonsterType)monster_type;
			}
		    }

			public int mdef;

		    [JsonIgnoreAttribute]
			public byte elem_weakness;

		    [JsonConverter(typeof(StringEnumConverter))]
		    public SpellElement ElementalWeakness {
			get {
			    return (SpellElement)elem_weakness;
			}
			}

		    [JsonIgnoreAttribute]
			public byte elem_resist;

		    [JsonConverter(typeof(StringEnumConverter))]
		    public SpellElement ElementalResist {
			get {
			    return (SpellElement)elem_resist;
			}
		    }

		    [JsonIgnoreAttribute]
			public int tier; // enemy's tier rating, used by Enemizer to determine stats and Formation Generator to enforce certain placement rules
		    [JsonIgnoreAttribute]
			public int skilltier = 0;
		    [JsonIgnoreAttribute]
			public byte image; // the image used by this image, of the 52 unique monster images available to normal enemies (does not include fiends or chaos).
		    [JsonIgnoreAttribute]
			public byte pal; // the palette normally used by this enemy.  this and the enemy's image are not stored in game data directly, rather they are implied by data in the formations

		    [JsonIgnoreAttribute]
			public byte tileset
			{
				get => (byte)((image >> 2) & 0b00001111);
			}

		    [JsonIgnoreAttribute]
			public byte pic
			{
				get => (byte)(image & 0b00000011);
			}

		    [JsonIgnoreAttribute]
			public bool Large
			{
				get => (image & 1) == 1;
			}

		    [JsonIgnoreAttribute]
			public bool Small
			{
				get => (image & 1) == 0;
			}

		    [JsonIgnoreAttribute]
		    public List<EnemyScriptInfo> allAIScripts;

			public byte[] compressData() // compresses the information of the enemy into an array of bytes to be placed in the game code
			{
				byte[] enemyInfo = new byte[20];
				enemyInfo[0] = (byte)(exp & 0xFF); // experience bytes are inverted from the usual format
				enemyInfo[1] = (byte)((exp >> 8) & 0xFF);
				enemyInfo[2] = (byte)(gp & 0xFF); // as is gold
				enemyInfo[3] = (byte)((gp >> 8) & 0xFF);
				enemyInfo[4] = (byte)(hp & 0xFF); // and hp
				enemyInfo[5] = (byte)((hp >> 8) & 0xFF);
				enemyInfo[6] = (byte)morale; // morale is 0-255
				enemyInfo[7] = AIscript;
				enemyInfo[8] = (byte)agility;
				enemyInfo[9] = (byte)absorb;
				enemyInfo[10] = (byte)num_hits;
				enemyInfo[11] = (byte)accuracy;
				enemyInfo[12] = (byte)damage;
				enemyInfo[13] = (byte)critrate;
				enemyInfo[14] = atk_elem;
				enemyInfo[15] = atk_ailment;
				enemyInfo[16] = monster_type;
				enemyInfo[17] = (byte)mdef;
				enemyInfo[18] = elem_weakness;
				enemyInfo[19] = elem_resist;
				return enemyInfo;
			}

			public void decompressData(byte[] data)
			{
				if (data.Length != EnemySize)
					return; // don't bother decompressing data of invalid size
				exp = data[0] + data[1] * 256;
				gp = data[2] + data[3] * 256;
				hp = data[4] + data[5] * 256;
				morale = data[6];
				AIscript = data[7];
				agility = data[8];
				absorb = data[9];
				num_hits = data[10];
				accuracy = data[11];
				damage = data[12];
				critrate = data[13];
				atk_elem = data[14];
				atk_ailment = data[15];
				monster_type = data[16];
				mdef = data[17];
				elem_weakness = data[18];
				elem_resist = data[19];
			}

		    public void writeData(FF1Rom rom) {
			var d = compressData();
			rom.Put(EnemyOffset + index * EnemySize, d);
		    }

		}
		public class FormationInfo
		{
			public int shape;
			public byte tileset;
			public byte pics = 0x00000000;
			public byte[] id = new byte[4] { 0xFF, 0xFF, 0xFF, 0xFF };
			public int[] monMin = new int[6] { 0, 0, 0, 0, 0, 0 };
			public int[] monMax = new int[6] { 0, 0, 0, 0, 0, 0 };
			public byte pal1;
			public byte pal2;
			public byte surprise;
			public byte paletteassignment = 0b00000000;
			public bool unrunnable_a;
			public bool unrunnable_b;

			public byte Top
			{
				get => id[0];
				set => id[0] = value;
			}

			public byte[] compressData() // compresses the information in the formation into an array of bytes to be placed in the game code
			{
				for (int i = 0; i < 4; ++i)
				{
					if (id[i] == 0xFF)
						id[i] = 0; // set empty slots to IMPs
				}
				byte[] formationData = new byte[16];
				formationData[0] = unchecked((byte)(shape << 4));
				formationData[0] |= tileset;
				formationData[1] = pics;
				formationData[2] = id[0];
				formationData[3] = id[1];
				formationData[4] = id[2];
				formationData[5] = id[3];
				formationData[6] = (byte)((monMin[0] << 4) | monMax[0]);
				formationData[7] = (byte)((monMin[1] << 4) | monMax[1]);
				formationData[8] = (byte)((monMin[2] << 4) | monMax[2]);
				formationData[9] = (byte)((monMin[3] << 4) | monMax[3]);
				formationData[10] = pal1;
				formationData[11] = pal2;
				formationData[12] = surprise;
				formationData[13] = paletteassignment;
				if (unrunnable_a)
					formationData[13] |= 0x01;
				if (unrunnable_b)
					formationData[13] |= 0x02;
				formationData[14] = (byte)((monMin[4] << 4) | monMax[4]);
				formationData[15] = (byte)((monMin[5] << 4) | monMax[5]);
				return formationData;
			}

			public void decompressData(byte[] data)
			{
				if (data.Length != FormationDataSize)
					return; // do nothing if this data isn't the right size
				shape = (data[0] & 0xF0) >> 4;
				tileset = (byte)(data[0] & 0x0F);
				pics = data[1];
				id[0] = data[2];
				id[1] = data[3];
				id[2] = data[4];
				id[3] = data[5];
				monMin[0] = (data[6] & 0xF0) >> 4;
				monMax[0] = data[6] & 0x0F;
				monMin[1] = (data[7] & 0xF0) >> 4;
				monMax[1] = data[7] & 0x0F;
				monMin[2] = (data[8] & 0xF0) >> 4;
				monMax[2] = data[8] & 0x0F;
				monMin[3] = (data[9] & 0xF0) >> 4;
				monMax[3] = data[9] & 0x0F;
				pal1 = data[10];
				pal2 = data[11];
				surprise = data[12];
				paletteassignment = (byte)(data[13] & 0xF0);
				unrunnable_a = (data[13] & 0x01) == 0x01;
				unrunnable_b = (data[13] & 0x02) == 0x02;
				monMin[4] = (data[14] & 0xF0) >> 4;
				monMax[4] = data[14] & 0x0F;
				monMin[5] = (data[15] & 0xF0) >> 4;
				monMax[5] = data[15] & 0x0F;
			}
		}

		public class Enemizer_Zone
		{
			public int min; // minimum count for this zone
			public int max; // maximum count for this zone
			public List<byte> forms; // list of which formations belong to this zone
			public int minXP; // minimum XP yield for this zone
			public int midXP; // "minimum maximum" XP yield for this zone
			public int maxXP; // maximum XP yield for this zone
			public List<byte> zonemons; // monsters that are compatible with this zone
			public int[] addr; // list of addresses this zone writes to
			public int zoneskillmax; // highest skill/spell tier allowed for this zone
			public Enemizer_Zone(int mincount, int maxcount, int zminXP, int zmidXP, int zmaxXP, int zskillmax)
			{
				min = mincount; max = maxcount; minXP = zminXP; midXP = zmidXP; maxXP = zmaxXP; zoneskillmax = zskillmax;
				forms = new List<byte> { };
				zonemons = new List<byte> { };
			}
		}

		public class EnemizerTrackingInfo
		{
			public List<byte>[] enemiesInTileset = new List<byte>[GenericTilesetsCount];
			public List<byte>[] palettesInTileset = new List<byte>[GenericTilesetsCount];
			public List<int>[] enemyZones = new List<int>[EnemyCount];
			public bool[] featured = new bool[EnemyCount];
			public List<Enemizer_Zone> zone = new List<Enemizer_Zone> { };
			public int[][] zoneclone; // list of addresses which are clones of other domain addresses - the first entry in the array is the base to read from, the other entries will copy that
			public byte imp_encounter = 0x00; // note the location of the imp encounter
			public byte wizard_encounter = 0x1C; // note the location of the wizard encounter
			public byte phantom_encounter = 0x46; // note the location of the phantom encounter, which will not be placed as a random (but can move around)
			public byte zombieD_encounter = 0x4B; // note the location of the zombieD encounter
			public byte warmech_encounter = 0x56; // note the location of the warmech encounter, which will not be placed as a random (we will not move this around since it is needed for other randomizer stuff)
			public byte eye_encounter = 0x69; // note the location of the eye encounter
			public byte lich1_encounter = 0x73; // lich encounter (we stop making formations after this point, boss encounters have special formations)
			public byte vamp_encounter = 0x7C; // vamp encounter
			public byte astos_encounter = 0x7D; // astos encounter
			public byte pirate_encounter = 0x7E; // pirate encounter
			public byte garland_encounter = 0x7F; // garland encounter

			public EnemizerTrackingInfo()
			{
				// constructor for EnemizerTrackingInfo contains much of the information for zones we have hardcoded
				for (int i = 0; i < GenericTilesetsCount; ++i)
				{
					enemiesInTileset[i] = new List<byte> { };
					palettesInTileset[i] = new List<byte> { };
				}
				for(int i = 0; i < EnemyCount; ++i)
				{
					enemyZones[i] = new List<int> { };
					featured[i] = false;
				}
				int[] zonecountmin = new int[11] { 12, 36, 18, 12, 14, 12, 12, 22, 14, 10, 14 };
				int[] zonecountmax = new int[11] { 18, 48, 27, 18, 21, 18, 18, 33, 21, 15, 21 };
				int[,] zonexpreqs = new int[11, 4]
				{
					{18, 24, 300, 2}, // early game
					{150, 300, 700, 2}, // ocean/pravoka/elfland/marsh
					{500, 800, 1400, 3}, // melmond and other overworld area
					{600, 1200, 2400, 3}, // earth cave
					{800, 2000, 4000, 5}, // crescent/onrac/cardia/power peninsula overworld
					{1200, 1500, 3600, 5}, // gurgu/river south
					{1350, 2800, 4800, 5}, // ordeals ice waterfall
					{1050, 2500, 5200, 5}, // lefein/mirage desert/gaia overworld/river north
					{2400, 3600, 7200, 5}, // mirage tower and sea shrine
					{2400, 4800, 10000, 5}, // sky castle
					{6000, 6001, 21000, 5}, // tofr
				};
				for (int i = 0; i < 11; ++i)
				{
					zone.Add(new Enemizer_Zone(zonecountmin[i], zonecountmax[i], zonexpreqs[i, 0], zonexpreqs[i, 1], zonexpreqs[i, 2], zonexpreqs[i, 3]));
				}
				zone[0].addr = new int[] { 0x0D8, 0x120, 0x128, 0x130, 0x260 };
				zone[1].addr = new int[] { 0x0E8, 0x1E0, 0x118, 0x158, 0x168, 0x170, 0x190, 0x198, 0x1A0, 0x1D0, 0x1D8, 0x210, 0x2B0, 0x2D8, 0x2E0 };
				zone[2].addr = new int[] { 0x108, 0x110, 0x138, 0x140, 0x148, 0x150, 0x178, 0x218, 0x3E0 };
				zone[3].addr = new int[] { 0x268, 0x2E8, 0x2F0, 0x2F8, 0x300 };
				zone[4].addr = new int[] { 0x0F8, 0x1A8, 0x1B0, 0x1B8, 0x1E8, 0x1F0, 0x1F8 };
				zone[5].addr = new int[] { 0x208, 0x270, 0x308, 0x310, 0x318, 0x320 };
				zone[6].addr = new int[] { 0x278, 0x290, 0x2C8, 0x2D0, 0x328, 0x330 };
				zone[7].addr = new int[] { 0x000, 0x010, 0x018, 0x028, 0x030, 0x040, 0x048, 0x050, 0x068, 0x078, 0x200 };
				zone[8].addr = new int[] { 0x2B8, 0x340, 0x348, 0x350, 0x358, 0x360, 0x368 };
				zone[9].addr = new int[] { 0x378, 0x380, 0x388, 0x390, 0x398 };
				zone[10].addr = new int[] { 0x3A0, 0x3A8, 0x3B0, 0x3B8, 0x3C0, 0x3C8, 0x3D0 };

				zoneclone = new int[8][];
				zoneclone[0] = new int[] { 0x000, 0x008 };
				zoneclone[1] = new int[] { 0x040, 0x080, 0x088 };
				zoneclone[2] = new int[] { 0x018, 0x020, 0x058, 0x060 };
				zoneclone[3] = new int[] { 0x030, 0x038 };
				zoneclone[4] = new int[] { 0x068, 0x070, 0x0A8, 0x0B0 };
				zoneclone[5] = new int[] { 0x078, 0x0B8 };
				zoneclone[6] = new int[] { 0x050, 0x098 };
				zoneclone[7] = new int[] { 0x0D8, 0x0E0 };
			}

			public void ReadEnemyDataFromFormation(FormationInfo f, EnemyInfo[] enemystats)
			{
				// pull information about the enemy's graphical data and other things that are held in formation data rather than the enemy data proper
				if (f.tileset < 0x00 || f.tileset >= GenericTilesetsCount)
					return; // we don't care about extracting formation information from fiends or chaos
				for (int i = 0; i < 4; ++i)
				{
					if (i < 2)
					{
						if (f.monMax[i] == 0 && f.monMax[i + 4] == 0)
							continue; // do not bother reading info for enemies that don't have a maxcount
					}
					else
					{
						if (f.monMax[i] == 0)
							continue; // same as the above
					}
					if (f.id[i] >= EnemyCount)
						continue; // don't use IDs that exceed the maximum number of enemies
					if (!enemiesInTileset[f.tileset].Contains(f.id[i]))
					{
						// place enemy in appropriate tileset
						enemiesInTileset[f.tileset].Add(f.id[i]);
						// determine which pic is being used
						int pict = (f.pics >> (i * 2)) & 0b00000011;
						enemystats[f.id[i]].image = (byte)(f.tileset << 2 | pict);
						bool paletteassigned = (f.paletteassignment & (0b10000000 >> i)) != 0;
						if (paletteassigned)
							enemystats[f.id[i]].pal = f.pal2;
						else
							enemystats[f.id[i]].pal = f.pal1;
						if (!palettesInTileset[f.tileset].Contains(f.pal1))
							palettesInTileset[f.tileset].Add(f.pal1);
						if (!palettesInTileset[f.tileset].Contains(f.pal2))
							palettesInTileset[f.tileset].Add(f.pal2);
					}
				}
			}

			public void LogFeatured(FormationInfo f)
			{
				for(int i = 0; i < 3; i++)
				{
					if (f.monMin[i] > 0)
						featured[f.id[i]] = true;
					if(i < 2)
					{
						if (f.monMin[i + 4] > 0)
							featured[f.id[i]] = true;
					}
				}
			}

			public void PurgeIDFromEnemyTilesetList(byte mon)
			{
				for (int i = 0; i < GenericTilesetsCount; ++i)
				{
					enemiesInTileset[i].RemoveAll(id => id == mon);
				}
			}

			public void PrintFormationInfo()
			{
				// this is where we would print formation info to console when debugging the game
			}

			public int GetMonLimit(bool size, int shape)
			{
				switch (shape)
				{
					case 0x00:
						if (!size)
							return 9;
						return 0;
					case 0x01:
						if (size)
							return 4;
						return 0;
					case 0x02:
						if (size)
							return 2;
						return 6;
					default:
						return 0;
				}
			}
		}

		public void ENF_AssignPicAndPaletteBytes(EnemyInfo[] enemy, FormationInfo f)
		{
			for (int i = 0; i < 4; ++i)
			{
				if (f.id[i] == 0xFF)
					continue;
				if (enemy[f.id[i]].pal == f.pal2)
					f.paletteassignment |= (byte)(0b00010000 << (3 - i));
				f.pics |= (byte)(enemy[f.id[i]].pic << (i * 2));
			}
		}

		private bool DoEnemizer_Formations(MT19337 rng, ZoneFormations zones, EnemyInfo[] enemy, EnemizerTrackingInfo en)
		{
			bool NotFeatured(byte mon) => !en.featured[mon];
			bool Small(byte mon) => enemy[mon].Small;
			bool Large(byte mon) => enemy[mon].Large;
			bool SmallAndNotFeatured(byte mon, byte id) => Small(mon) && NotFeatured(mon) && mon != id;
			bool LargeAndNotFeatured(byte mon, byte id) => Large(mon) && NotFeatured(mon) && mon != id;

			var formationData = Get(FormationDataOffset, FormationDataSize * FormationDataCount).Chunk(FormationDataSize); // load base information from the ROM into chunks we can manipulate
			Blob dom = zones.GetBytes().SelectMany(z => z).ToArray();
			//var dom = Get(ZoneFormationsOffset, ZoneFormationsSize * ZoneCount); // however, we are going to adjust the domain data directly
			List<byte> uniqueEnemyIDs = new List<byte> { 0x00, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x0A, 0x0B, 0x0C, 0x0D, 0x0E, 0x10, 0x11, 0x12, 0x13, 0x14, 0x15, 0x16, 0x17, 0x18, 0x19,
			  0x1A, 0x1B, 0x1C, 0x1D, 0x1E, 0x1F, 0x20, 0x21, 0x22, 0x23, 0x24, 0x25, 0x26, 0x27, 0x28, 0x29, 0x2A, 0x2B, 0x2C, 0x2D, 0x2E, 0x2F, 0x30, 0x31, 0x32, 0x34,
			  0x35, 0x36, 0x37, 0x38, 0x39, 0x3A, 0x3B, 0x3C, 0x3D, 0x3E, 0x3F, 0x40, 0x41, 0x42, 0x43, 0x44, 0x45, 0x46, 0x47, 0x48, 0x49, 0x4A, 0x4B, 0x4C, 0x4D,
			  0x4E, 0x4F, 0x50, 0x51, 0x52, 0x53, 0x54, 0x55, 0x56, 0x57, 0x58, 0x59, 0x5A, 0x5B, 0x5C, 0x5D, 0x5E, 0x5F, 0x60, 0x61, 0x62, 0x63, 0x64, 0x65, 0x66, 0x67,
			  0x68, 0x6A, 0x6B, 0x6C, 0x6D, 0x6E, 0x6F, 0x70, 0x72, 0x73, 0x74, 0x75 };
			byte[] traptile_formations = new byte[(int)TrapTiles.NUM_TRAP_TILES]; // formation pointers for each trap tile
			int[] traptile_addresses = new int[(int)TrapTiles.TRAP_LICH2]; // ROM addresses of trap tiles (excluding Lich2 and after
			foreach (byte mon in uniqueEnemyIDs) // sort an enemy into its zone
			{
				int limit = enemy[mon].Large ? 4 : 9;
				for (int i = 0; i < en.zone.Count; ++i)
				{
					if (enemy[mon].exp <= (en.zone[i].maxXP * 2) / 3 && enemy[mon].exp * limit >= en.zone[i].midXP && enemy[mon].skilltier <= en.zone[i].zoneskillmax)
					{
						en.zone[i].zonemons.Add(mon);
						en.enemyZones[mon].Add(i);
					}
				}
				if(en.enemyZones[mon].Count == 0)
				{
					for (int i = 0; i < en.zone.Count; ++i)
					{
						if (enemy[mon].exp <= (en.zone[i].maxXP * 2) / 3 && enemy[mon].exp * limit >= en.zone[i].midXP)
						{
							en.zone[i].zonemons.Add(mon);
							en.enemyZones[mon].Add(i);
						}
					}
				}
			}
			// roll special encounters for: imp, phantom, warmech, vampire, astos, pirates, garland (and ignore fiend encounters
			en.wizard_encounter = 0x81;
			en.eye_encounter = 0x82;
			en.zombieD_encounter = 0x83;
			en.phantom_encounter = 0x04;
			formationData[en.imp_encounter] = ENF_DrawBossEncounter(en, rng, enemy, Enemy.Imp, 4, enemy[Enemy.Imp].Large ? 0x01 : 0x00, false, 4, en.imp_encounter);
			formationData[0x01] = ENF_DrawForcedSingleFight(en, rng, enemy, 2, 0x01);
			formationData[0x02] = ENF_DrawForcedSingleFight(en, rng, enemy, 6, 0x02);
			formationData[0x03] = ENF_DrawForcedSingleFight(en, rng, enemy, 6, 0x03);
			formationData[0x04] = ENF_DrawBossEncounter(en, rng, enemy, Enemy.Phantom, 1, 0x02, true, 4, 0x04);
			formationData[en.vamp_encounter] = ENF_DrawBossEncounter(en, rng, enemy, Enemy.Vampire, 1, 0x02, true, 4, 0x7C);
			formationData[en.astos_encounter] = ENF_DrawBossEncounter(en, rng, enemy, Enemy.Astos, 1, 0x02, true, 4, 0x7D);
			formationData[en.pirate_encounter] = ENF_DrawBossEncounter(en, rng, enemy, Enemy.Pirate, 9, 0x00, true, 4, 0x7E);
			formationData[en.garland_encounter] = ENF_DrawBossEncounter(en, rng, enemy, Enemy.Garland, 1, 0x02, true, 4, 0x7F);
			formationData[en.warmech_encounter] = ENF_DrawBossEncounter(en, rng, enemy, Enemy.WarMech, 1, 0x02, false, 75, en.warmech_encounter);
			// reserve so many slots for different surprise rates and unrunnables
			int[,] surprisetiers = { { 10, 19 }, { 24, 36 }, { 48, 62 }, { 70, 100 } };
			const int surp1 = 9;
			const int surp2 = 12;
			const int surp3 = 4;
			const int surp4 = 2;
			int[] surpcount = { surp1, surp2, surp3, surp4 };
			const int regularformations = 0x05; // we start rolling formations from where we stopped rolling special-case formations
			const int bossformations = 0x73; // we stop rolling formations once we reach boss formations
			for (byte i = regularformations; i < bossformations; ++i) // general loop for formation generation
			{
				if (i == en.imp_encounter || i == en.phantom_encounter || i == en.warmech_encounter)
					continue; // don't do these encounters (we will roll them as special exceptions)
				FormationInfo f = new FormationInfo(); // this is the formation we are hoping to write
				// we will start narrowing down the list of monsters we can feature
				List<byte> availablemons = new List<byte> { };
				// first, we check if all monsters have been featured yet
				if (uniqueEnemyIDs.Where(NotFeatured).Count() == 0)
				{
					// if we have featured all monsters, reset the featured array
					foreach (byte mon in uniqueEnemyIDs)
					{
						en.featured[mon] = false;
					}
				}
				// first, we determine which zones we have yet to reach the minimum for.  if they have yet to reach the minimum, they can join the union if the mon hasn't been featured yet
				for (int j = 0; j < en.zone.Count; ++j)			{
					if (en.zone[j].forms.Count < en.zone[j].min)
					{
						availablemons = availablemons.Union(en.zone[j].zonemons.Where(NotFeatured)).ToList();
					}
				}
				// if we have already met the minimums for each zone, then any zone which has not reached its maximum can be chosen, if they have not been featured yet in this cycle
				if (availablemons.Count == 0)
				{
					for (int j = 0; j < en.zone.Count; ++j)
					{
						if (en.zone[j].forms.Count < en.zone[j].max)
						{
							availablemons = availablemons.Union(en.zone[j].zonemons.Where(NotFeatured)).ToList();
						}
					}
				}
				// if we still don't have any mons available, then we can freely pick from all mon IDs that have yet to be featured
				if (availablemons.Count == 0)
				{
					availablemons = uniqueEnemyIDs.Where(NotFeatured).ToList();
					// and if we simply don't have mons available because they've all been featured, all mons are considered available (this should never execute)
					if (availablemons.Count == 0)
					{
						availablemons = uniqueEnemyIDs.ToList();
					}
				}
				// now we pick one of the availablemons at random as our first mon
				f.Top = availablemons.PickRandom(rng);
				f.tileset = enemy[f.Top].tileset; // and we pick the corresponding tileset
				// we need to select which zone we are aiming to fill on the B-Side (and if we require a Large-only or Small-only formation to do that, we set the shape of the formation accordingly)
				List<int> availablezones = en.enemyZones[f.Top].Where(index => en.zone[index].forms.Count < en.zone[index].min).ToList();
				if (availablezones.Count == 0) // if there are no zones available that haven't reached their mincount, check for maxcount instead
					availablezones = en.enemyZones[f.Top].Where(index => en.zone[index].forms.Count < en.zone[index].max).ToList();
				if (availablezones.Count == 0) // and if we've filled mincount and maxcount, just list all available zones
					availablezones = en.enemyZones[f.Top].ToList();
				if (availablezones.Count == 0) // and if by some freak chance the enemy has not been assigned any zones, write an error report to Console and abort Formation shuffle
				{
					return false;
				}
				int zoneB = availablezones.PickRandom(rng); // the B-Side zone we are trying to fill
				int limit = Large(f.Top) ? 2 : 6;
				if (enemy[f.Top].exp * limit < en.zone[zoneB].midXP)
					f.shape = Large(f.Top) ? 0x01 : 0x00;
				else
				{
					if (rng.Between(0, 2) == 0)
						f.shape = Large(f.Top) ? 0x01 : 0x00;
					else
						f.shape = 0x02;
				}
				// now we look for what we can pick for the second mon
				if (f.shape == 0x00)
				{
					availablemons = en.enemiesInTileset[f.tileset].Where(mon => SmallAndNotFeatured(mon, f.Top)).ToList();
					if (availablemons.Count == 0)
						availablemons = en.enemiesInTileset[f.tileset].Where(mon => Small(mon) && mon != f.Top).ToList();

					// Fix for when there's only 1 small mon in the pool and it already got top billing
					if (availablemons.Count == 0)
						availablemons = en.enemiesInTileset[f.tileset].Where(mon => Small(mon)).ToList();
				}
				else if (f.shape == 0x01)
				{
					availablemons = en.enemiesInTileset[f.tileset].Where(mon=> LargeAndNotFeatured(mon, f.Top)).ToList();
					if (availablemons.Count == 0)
						availablemons = en.enemiesInTileset[f.tileset].Where(mon => Large(mon) && mon != f.Top).ToList();

					// Fix for when there's only 1 large mon in the pool and it already got top billing
					if (availablemons.Count == 0)
						availablemons = en.enemiesInTileset[f.tileset].Where(mon => Large(mon)).ToList();
				}
				else
				{
					availablemons = en.enemiesInTileset[f.tileset].Where(mon => NotFeatured(mon) && mon != f.Top).ToList();
					if (availablemons.Count == 0)
						availablemons = en.enemiesInTileset[f.tileset].Where(mon => mon != f.Top).ToList(); // if all other mons in this tileset have been featured, all mons in this tileset are eligible as a second mon
				}
				f.id[1] = availablemons.PickRandom(rng); // and pick a random mon from the available IDs to be mon 2 (and the primary monster of the A-side)
				// we will attempt to hit a zone with the A-Side but if we can't we'll just try to make an encounter as close as possible to the goal
				availablezones = en.enemyZones[f.id[1]].Where(index => en.zone[index].forms.Count < en.zone[index].min).ToList();
				if (availablezones.Count == 0) // if there are no zones available that haven't reached their mincount, check for maxcount instead
					availablezones = en.enemyZones[f.id[1]].Where(index => en.zone[index].forms.Count < en.zone[index].max).ToList();
				if (availablezones.Count == 0) // and if we've filled mincount and maxcount, just list all available zones
					availablezones = en.enemyZones[f.id[1]].ToList();
				if (availablezones.Count == 0) // and if by some freak chance the enemy has not been assigned any zones, write an error report to Console and abort Formation shuffle
				{
					return false;
				}
				int zoneA = availablezones.PickRandom(rng);
				// now we set the palettes
				f.pal1 = enemy[f.Top].pal;
				if (enemy[f.Top].pal != enemy[f.id[1]].pal)
					f.pal2 = enemy[f.id[1]].pal;
				else // if first and second enemies in the formation have the same palette, draw a second palette at random
				{
					List<byte> availablepalettes = en.palettesInTileset[f.tileset].Where(pal => pal != f.pal1).ToList();
					if (availablepalettes.Count == 0)
						f.pal2 = f.pal1; // if there is only one palette in the tileset, just set the second palette to the same value as the first
					else
						f.pal2 = availablepalettes.PickRandom(rng);
				}
				// and we draw any monsters that conform to both the tileset and palette choices as the third and fourth monsters (if there are more than two, they are dropped after a shuffle of the list)
				availablemons = en.enemiesInTileset[f.tileset].Where(mon => mon != f.Top && mon != f.id[1] && (enemy[mon].pal == f.pal1 || enemy[mon].pal == f.pal2)).ToList();
				switch (f.shape)
				{
					case 0x00:
						availablemons = availablemons.Where(Small).ToList();
						break;
					case 0x01:
						availablemons = availablemons.Where(Large).ToList();
						break;
				}
				if (availablemons.Count > 0)
				{
					availablemons.Shuffle(rng);
					f.id[2] = availablemons[0];
					if (availablemons.Count > 1)
						f.id[3] = availablemons[1];
				}
				// now we can decide how many monsters to aim for in the formation.  we call two functions, one to draw the B-side, and another to draw the A-side
				zoneB = ENF_Picker_Generic(en, enemy, rng, f, zoneB, 0, true);
				if (zoneB != -1)
					zoneA = ENF_Picker_Generic(en, enemy, rng, f, zoneA, 1, false);
				if (zoneA == -1 || zoneB == -1)
				{
					en.featured[f.Top] = true;
					en.featured[f.id[1]] = true;
					--i;
					continue; // loop again after setting the problematic monsters to featured status so they are de-prioritized and won't cause an infinite loop problem
				}
				// assign the pic and palette bytes to each monster slot
				ENF_AssignPicAndPaletteBytes(enemy, f);
				// set surprise rate and unrunnability flags
				f.unrunnable_a = rng.Between(0, EnemizerUnrunnabilityWeight) + zoneA >= EnemizerUnrunnabilityWeight + 3 ? true : false; // unrunnable chance is higher for later zones
				f.unrunnable_b = rng.Between(0, EnemizerUnrunnabilityWeight) + zoneB >= EnemizerUnrunnabilityWeight + 3 ? true : false;
				if (f.unrunnable_a && f.unrunnable_b)
					f.surprise = (byte)rng.Between(3, 30);
				else
				{
					if (bossformations - surpcount.Sum() <= i)
					{
						// automatically roll a surprise chance
						int s_roll = rng.Between(1, surpcount.Sum());
						int s_tier = -1;
						if (s_roll <= surpcount[0])
							s_tier = 0;
						else if (s_roll <= surpcount[0] + surpcount[1] && s_roll > surpcount[0])
							s_tier = 1;
						else if (s_roll <= surpcount[0] + surpcount[1] + surpcount[2] && s_roll > surpcount[0] + surpcount[1])
							s_tier = 2;
						else if (s_roll <= surpcount[0] + surpcount[1] + surpcount[2] + surpcount[3] && s_roll > surpcount[0] + surpcount[1] + surpcount[2])
							s_tier = 3;
						if (s_tier == -1)
						{
							f.surprise = (byte)rng.Between(3, 9);
						}
						else
						{
							f.surprise = (byte)rng.Between(surprisetiers[s_tier, 0], surprisetiers[s_tier, 1]);
							surpcount[s_tier]--;
						}
					}
					else
					{
						// random chance to roll additional surprise rate
						int s_tier = -1;
						int s_roll = rng.Between(regularformations, bossformations);
						if (s_roll > bossformations - surp4)
							s_tier = 3;
						else if (s_roll > bossformations - surp4 - surp3)
							s_tier = 2;
						else if (s_roll > bossformations - surp4 - surp3 - surp2)
							s_tier = 1;
						else if (s_roll > bossformations - surp4 - surp3 - surp2 - surp1)
							s_tier = 0;
						if (s_tier == -1)
						{
							f.surprise = (byte)rng.Between(3, 9);
						}
						else
						{
							if (surpcount[s_tier] > 0)
							{
								f.surprise = (byte)rng.Between(surprisetiers[s_tier, 0], surprisetiers[s_tier, 1]);
								surpcount[s_tier]--;
							}
							else
							{
								f.surprise = (byte)rng.Between(3, 9);
							}
						}
					}
				}
				// add the A-Side and B-Side to their respective zones
				en.zone[zoneA].forms.Add(i);
				en.zone[zoneB].forms.Add((byte)(i | 0x80));
				// log and compress this formation
				en.LogFeatured(f);
				formationData[i] = f.compressData();
			}
			en.PrintFormationInfo();
			// now we will place enemies in their proper domains
			// load the addresses of all known trap tiles (these addresses are hardcoded for now - i'm not going to consider altered maps)
			traptile_addresses[(int)TrapTiles.TRAP_IMAGES] = 0x1B3;
			traptile_addresses[(int)TrapTiles.TRAP_MUMMIES] = 0x1B5;
			traptile_addresses[(int)TrapTiles.TRAP_MUDGOLS] = 0x1B7;
			traptile_addresses[(int)TrapTiles.TRAP_NITEMARES] = 0x1B9;
			traptile_addresses[(int)TrapTiles.TRAP_ZOMBIE_D] = 0x1BB;
			traptile_addresses[(int)TrapTiles.TRAP_GARGOYLES] = 0x541;
			traptile_addresses[(int)TrapTiles.TRAP_SEAFOOD_PARTY_MIX] = 0x543;
			traptile_addresses[(int)TrapTiles.TRAP_SHARKNADO] = 0x545;
			traptile_addresses[(int)TrapTiles.TRAP_WATERS] = 0x547;
			traptile_addresses[(int)TrapTiles.TRAP_SEASHRINE_MUMMIES] = 0x549;
			traptile_addresses[(int)TrapTiles.TRAP_ZOMBIES] = 0x5A5;
			traptile_addresses[(int)TrapTiles.TRAP_GARGOYLES2] = 0x5A9;
			traptile_addresses[(int)TrapTiles.TRAP_GIANTS] = 0x237;
			traptile_addresses[(int)TrapTiles.TRAP_GIANTS_IGUANAS] = 0x239;
			traptile_addresses[(int)TrapTiles.TRAP_EARTH] = 0x23B;
			traptile_addresses[(int)TrapTiles.TRAP_OGRES_HYENAS] = 0x23D;
			traptile_addresses[(int)TrapTiles.TRAP_SPHINX] = 0x243;
			traptile_addresses[(int)TrapTiles.TRAP_FIRE] = 0x245;
			traptile_addresses[(int)TrapTiles.TRAP_GREYWORM] = 0x247;
			traptile_addresses[(int)TrapTiles.TRAP_AGAMA] = 0x25F;
			traptile_addresses[(int)TrapTiles.TRAP_RED_D] = 0x281;
			traptile_addresses[(int)TrapTiles.TRAP_UNDEAD_PARTYMIX] = 0x381;
			traptile_addresses[(int)TrapTiles.TRAP_FROSTRURUS] = 0x383;
			traptile_addresses[(int)TrapTiles.TRAP_FROSTGIANT] = 0x385;
			traptile_addresses[(int)TrapTiles.TRAP_MAGES] = 0x387;
			traptile_addresses[(int)TrapTiles.TRAP_FROST_D] = 0x389;
			traptile_addresses[(int)TrapTiles.TRAP_EYE] = 0x38B;
			traptile_addresses[(int)TrapTiles.TRAP_WATERFALL_MUMMIES] = 0x391;
			traptile_addresses[(int)TrapTiles.TRAP_WIZARDS] = 0x433;
			traptile_addresses[(int)TrapTiles.TRAP_WIZARDS2] = 0x435;
			traptile_addresses[(int)TrapTiles.TRAP_COBRAS] = 0x437;
			traptile_addresses[(int)TrapTiles.TRAP_BLUE_D] = 0x439;
			traptile_addresses[(int)TrapTiles.TRAP_SLIMES] = 0x43B;
			traptile_addresses[(int)TrapTiles.TRAP_PHANTOM] = 0x7AD;

			// assign monsters to trap tiles
			traptile_formations[(int)TrapTiles.TRAP_IMAGES] = en.zone[3].forms[0];
			traptile_formations[(int)TrapTiles.TRAP_MUMMIES] = en.zone[3].forms[1];
			traptile_formations[(int)TrapTiles.TRAP_MUDGOLS] = en.zone[6].forms[2];
			traptile_formations[(int)TrapTiles.TRAP_NITEMARES] = en.zone[6].forms[3];
			traptile_formations[(int)TrapTiles.TRAP_ZOMBIE_D] = en.zombieD_encounter;
			traptile_formations[(int)TrapTiles.TRAP_GARGOYLES] = en.zone[2].forms[1];
			traptile_formations[(int)TrapTiles.TRAP_SEAFOOD_PARTY_MIX] = en.zone[8].forms[0];
			traptile_formations[(int)TrapTiles.TRAP_SHARKNADO] = en.zone[8].forms[1];
			traptile_formations[(int)TrapTiles.TRAP_WATERS] = en.zone[8].forms[2];
			traptile_formations[(int)TrapTiles.TRAP_SEASHRINE_MUMMIES] = en.zone[8].forms[3];
			traptile_formations[(int)TrapTiles.TRAP_ZOMBIES] = en.zone[0].forms[0];
			traptile_formations[(int)TrapTiles.TRAP_GARGOYLES2] = en.zone[2].forms[1];
			traptile_formations[(int)TrapTiles.TRAP_GIANTS] = en.zone[4].forms[0];
			traptile_formations[(int)TrapTiles.TRAP_GIANTS_IGUANAS] = en.zone[4].forms[1];
			traptile_formations[(int)TrapTiles.TRAP_EARTH] = en.zone[4].forms[2];
			traptile_formations[(int)TrapTiles.TRAP_OGRES_HYENAS] = en.zone[4].forms[3];
			traptile_formations[(int)TrapTiles.TRAP_SPHINX] = en.zone[5].forms[0];
			traptile_formations[(int)TrapTiles.TRAP_FIRE] = en.zone[5].forms[1];
			traptile_formations[(int)TrapTiles.TRAP_GREYWORM] = en.zone[5].forms[2];
			traptile_formations[(int)TrapTiles.TRAP_AGAMA] = en.zone[5].forms[3];
			traptile_formations[(int)TrapTiles.TRAP_RED_D] = en.zone[5].forms[4];
			traptile_formations[(int)TrapTiles.TRAP_UNDEAD_PARTYMIX] = en.zone[5].forms[5];
			traptile_formations[(int)TrapTiles.TRAP_FROSTRURUS] = en.zone[5].forms[6];
			traptile_formations[(int)TrapTiles.TRAP_FROSTGIANT] = en.zone[5].forms[7];
			traptile_formations[(int)TrapTiles.TRAP_MAGES] = en.zone[5].forms[8];
			traptile_formations[(int)TrapTiles.TRAP_FROST_D] = en.zone[5].forms[9];
			traptile_formations[(int)TrapTiles.TRAP_EYE] = en.eye_encounter;
			traptile_formations[(int)TrapTiles.TRAP_WATERFALL_MUMMIES] = en.zone[8].forms[3];
			traptile_formations[(int)TrapTiles.TRAP_WIZARDS] = en.wizard_encounter;
			traptile_formations[(int)TrapTiles.TRAP_WIZARDS2] = en.wizard_encounter;
			traptile_formations[(int)TrapTiles.TRAP_COBRAS] = en.zone[3].forms[2];
			traptile_formations[(int)TrapTiles.TRAP_BLUE_D] = en.zone[9].forms[0];
			traptile_formations[(int)TrapTiles.TRAP_SLIMES] = en.zone[9].forms[1];
			traptile_formations[(int)TrapTiles.TRAP_PHANTOM] = en.phantom_encounter;
			// proceed to place mons in all zones
			foreach(Enemizer_Zone zone in en.zone)
			{
				zone.forms.Shuffle(rng);
				int loop = 0;
				int f = 0;
				int a = 0;
				while(f < zone.forms.Count)
				{
					if (loop == 0)
					{
						for (int j = 0; j < 8; ++j)
						{
							dom[zone.addr[a] + j] = zone.forms[f]; // first loop: fill the domain with the encounter
						}
					}
					if (loop == 1)
					{
						for (int j = 0; j < 8; j += 2)
						{
							dom[zone.addr[a] + j] = zone.forms[f]; // second loop: 1, 3, 5, and 7 are filled with the second formation
						}
					}
					if (loop == 2)
					{
						dom[zone.addr[a] + 2] = zone.forms[f]; // third loop: 3, 6, and 8 are filled with the third encounter
						dom[zone.addr[a] + 5] = zone.forms[f];
						dom[zone.addr[a] + 7] = zone.forms[f];
					}
					if (loop == 3)
					{
						dom[zone.addr[a] + 3] = zone.forms[f]; // fourth loop: 4 and 7 are filled with the 4th encounter
						dom[zone.addr[a] + 6] = zone.forms[f];
					}
					if (loop == 4)
					{
						dom[zone.addr[a] + 4] = zone.forms[f]; // fifth loop: 5 and 6 are filled with the 5th encounter
						dom[zone.addr[a] + 5] = zone.forms[f];
					}
					if (loop > 4)
					{
						dom[zone.addr[a] + loop] = zone.forms[f]; // sixth-eigth loop: slot 6, 7, or 8 is filled depending on which loop we are at
					}
					f++;
					a++;
					if (a == zone.addr.Length)
					{
						a = 0;
						loop++;
						if (loop == 8)
							break; // place no more than 8 encounters per domain even if formations are left to be placed (this should basically never happen)
					}
				}
			}
			// duplicate entries in the overworld that clone other areas
			foreach (int[] array in en.zoneclone)
			{
				byte[] dataToCopy = new byte[8];
				for (int i = 0; i < 8; ++i)
				{
					dataToCopy[i] = dom[array[0] + i];
				}
				for (int i = 1; i < array.Length; ++i)
				{
					for (int j = 0; j < 8; ++j)
					{
						dom[array[i] + j] = dataToCopy[j];
					}
				}
			}
			// special zone placement for overworld 4,4 and 4,5: replace the first four encounters with imp encounter
			dom[0x120] = en.imp_encounter; dom[0x121] = en.imp_encounter; dom[0x122] = en.imp_encounter; dom[0x123] = en.imp_encounter;
			dom[0x160] = en.imp_encounter; dom[0x161] = en.imp_encounter; dom[0x162] = en.imp_encounter; dom[0x123] = en.imp_encounter;
			dom[0x39E] = en.warmech_encounter; // replace 7th encounter on sky 5F with warmech
			for (int i = 0; i < FormationDataCount; ++i) // write formation info to ROM
				Put(FormationDataOffset + i * FormationDataSize, formationData[i]);
			zones.UpdateFromBlob(dom);
			//Put(ZoneFormationsOffset, dom); // write the domain data as one big chunk
			// write trap tile information
			for (int i = 0; i < traptile_addresses.Length; ++i)
			{
				Data[TilesetDataOffset + traptile_addresses[i]] = traptile_formations[i];
			}
			return true;
		}

		public int ENF_Picker_Generic(EnemizerTrackingInfo en, EnemyInfo[] enemy, MT19337 rng, FormationInfo f, int zone, int p, bool sideB)
		{
			bool Small(byte mon) => enemy[mon].Small;
			bool Large(byte mon) => enemy[mon].Large;
			// draw A-Side formation, prioritizing the priority monster (defaults to f.id[0] if given an invalid priority)
			if (p < 0 || p > 3)
				p = 0;
			if (f.id[p] == 0xFF)
				p = 0;
			if (f.id[p] == 0xFF)
				return -1; // do not draw mons if there is no valid mon as priority
			int o = sideB ? 4 : 0; // increase offset by 4 when placing a side B encounter
			int s = sideB ? 1 : 3; // indicates where we stop looping through ids (stop at 1 for sideB since ids 2 and 3 cannot be placed in side B encounters, stop at 3 otherwise)
			f.monMin[p + o] = 1; // there will always be at least one mon of the priority
			f.monMax[p + o] = 1;
			int minXP = enemy[f.id[p]].exp;
			int maxXP = enemy[f.id[p]].exp;
			int smallLimit = 0, largeLimit = 0;
			switch (f.shape)
			{
				case 0x00:
					smallLimit = 9;
					largeLimit = 0;
					break;
				case 0x01:
					smallLimit = 0;
					largeLimit = 4;
					break;
				case 0x02:
					smallLimit = 6;
					largeLimit = 2;
					break;
			}
			if (Large(f.id[p]))
				largeLimit--;
			else
				smallLimit--;
			List<int> validindices = new List<int> { }; // no weighting selection towards priority in A-Side
			List<int> trashindices = new List<int> { }; // list of "trash mons" that may be optionally added in after general mon selection
			for (int i = 0; i < s; ++i)
			{
				if (f.id[i] == 0xFF)
					continue; // don't include mons with an invalid index
				int sizeFactor = Large(f.id[i]) ? 6 : 12;
				if (enemy[f.id[i]].exp > en.zone[zone].maxXP / sizeFactor)
					validindices.Add(i); // valid monsters must contribute significant experience for their size
				else if (Small(f.id[i]))
					trashindices.Add(i);
			}
			if (validindices.Count == 0) // if we have no valid indices (possible if some zones), we use the minimum-maximum instead of total max
			{
				trashindices.Clear();
				for (int i = 0; i < s; ++i)
				{
					if (f.id[i] == 0xFF)
						continue; // don't include mons with an invalid index
					int sizeFactor = Large(f.id[i]) ? 6 : 12;
					if (enemy[f.id[i]].exp > en.zone[zone].midXP / sizeFactor)
						validindices.Add(i); // valid monsters must contribute experience equal to 1/12th of the xp cap for this zone
					else if (Small(f.id[i]))
						trashindices.Add(i);
				}
			}
			while ((smallLimit > 0 || largeLimit > 0) && validindices.Count > 0)
			{
				if (rng.Between(0, 14) == 0 && maxXP > en.zone[zone].midXP)
					break; // 6.66% chance of ending the while loop if we have already met the minimum-maximum
				int index = validindices.PickRandom(rng);
				if (maxXP + enemy[f.id[index]].exp <= en.zone[zone].maxXP && ((Large(f.id[index]) && largeLimit > 0) || (Small(f.id[index]) && smallLimit > 0)))
				{
					f.monMax[index + o]++;
					maxXP += enemy[f.id[index]].exp;
					if (Large(f.id[index]))
						largeLimit--;
					else
						smallLimit--;
				}
				else
				{
					validindices.Remove(index);
				}
			}
			// now, hopefully, we got the minimum-maximums we wanted from that.  time to raise minimums
			validindices = new List<int> { };
			for (int i = 0; i < s; ++i)
			{
				if (f.id[i] == 0xFF)
					continue;
				if (f.monMax[i + o] > 0)
					validindices.Add(i); // add any monster that has a maxcount
			}
			while (minXP < en.zone[zone].minXP && validindices.Count > 0)
			{
				int index = validindices.PickRandom(rng);
				if (f.monMin[index + o] < f.monMax[index + o])
				{
					f.monMin[index + o]++;
					minXP += enemy[f.id[index]].exp;
				}
				else
					validindices.Remove(index);
			}
			// set the minimums to a random number between the current minimum and the maximum
			for (int i = 0; i < s; ++i)
			{
				f.monMin[i + o] = rng.Between(f.monMin[i + o], f.monMax[i + o]);
			}
			// if there is space, we can add 1-2 of each trash index (so long as they do not exceed the zone's maximum).  only smalls can be trash mons.
			if (trashindices.Count > 0 && smallLimit > 0)
			{
				for (int i = 0; i < trashindices.Count; ++i)
				{
					if (rng.Between(0, 2) == 0) // 33% chance of rejecting a trash index
					{
						if (Small(f.id[trashindices[i]]))
						{
							if (maxXP + enemy[f.id[trashindices[i]]].exp < en.zone[zone].maxXP)
							{
								smallLimit--;
								f.monMax[trashindices[i]]++;
								maxXP += enemy[f.id[trashindices[i]]].exp;
							}
							if (rng.Between(0, 1) == 0) // 50% chance of adding a second trash mon of the same type
							{
								if (maxXP + enemy[f.id[trashindices[i]]].exp < en.zone[zone].maxXP)
								{
									smallLimit--;
									f.monMax[trashindices[i]]++;
									maxXP += enemy[f.id[trashindices[i]]].exp;
								}
							}
						}
					}
				}
			}
			// if we were able to conform to all requirements of the zone, return false, but if we couldn't conform to the zone requirements return true so we know to calc the new zone
			return ENF_Calc_Zone(en, enemy, f, sideB);
		}

		public int ENF_Calc_Zone(EnemizerTrackingInfo en, EnemyInfo[] enemy, FormationInfo f, bool sideB)
		{
			// calculates a zone for the encounter to fit if we couldn't reach our goal zone (return -1 if the zone is completely invalid)
			int return_value = -1;
			List<int> acceptableValues = new List<int> { };
			int minXP = 0, maxXP = 0;
			int o = sideB ? 4 : 0;
			int s = sideB ? 1 : 3;
			for (int i = 0; i < s; ++i)
			{
				if (f.id[i] == 0xFF)
					continue;
				if (f.monMax[i + o] > 0)
				{
					minXP += f.monMin[i + o] * enemy[f.id[i]].exp;
					maxXP += f.monMax[i + o] * enemy[f.id[i]].exp;
				}
			}
			for (int i = 0; i < en.zone.Count; ++i)
			{
				if (minXP >= en.zone[i].minXP && maxXP >= en.zone[i].midXP && maxXP <= en.zone[i].maxXP)
					acceptableValues.Add(i);
			}
			List<int> mincountValues = acceptableValues.Where(id => en.zone[id].forms.Count < en.zone[id].min).ToList();
			if (mincountValues.Count == 0)
				mincountValues = acceptableValues.Where(id => en.zone[id].forms.Count < en.zone[id].max).ToList();
			if (mincountValues.Count > 0)
				return_value = mincountValues.Max();
			else
			{
				if (acceptableValues.Count > 0)
					return_value = acceptableValues.Max();
			}
			return ENF_ApproveFormation(f, return_value);
		}

		public int ENF_ApproveFormation(FormationInfo f, int zone)
		{
			// approves a formation based on the zone it has been assigned.  this will be used to rule out formations that might be problematic
			// for now this function does nothing but it will be used in future to reject certain formations
			if (zone == -1)
				return zone; // formations which have already been calculated as failed will be rejected
			return zone;
		}

		private byte[] ENF_DrawBossEncounter(EnemizerTrackingInfo en, MT19337 rng, EnemyInfo[] enemy, byte id, int count, int shape, bool unrunnable, byte surprise, byte formid)
		{
			bool Small(byte mon) => enemy[mon].Small;
			bool Large(byte mon) => enemy[mon].Large;

			FormationInfo f = new FormationInfo();
			f.id[2] = id; // we place the boss monster in the third slot
			f.tileset = enemy[id].tileset;
			f.pal1 = enemy[id].pal;
			f.shape = shape;
			f.monMin[2] = count;
			f.monMax[2] = count; // always produce the necessary number of enemies
			// draw mons for B-Side
			List<byte> availablemons = en.enemiesInTileset[f.tileset].ToList();
			switch (f.shape)
			{
				case 0x00:
					availablemons = availablemons.Where(Small).ToList();
					break;
				case 0x01:
					availablemons = availablemons.Where(Large).ToList();
					break;
			}
			if (availablemons.Count > 0)
			{
				f.Top = availablemons.PickRandom(rng);
				f.pal2 = enemy[f.Top].pal;
				availablemons = availablemons.Where(mon => mon != f.Top && mon != f.id[2] && (enemy[mon].pal == f.pal1 || enemy[mon].pal == f.pal2)).ToList();
				if (availablemons.Count > 0)
				{
					f.id[1] = availablemons.PickRandom(rng);
				}
				int zoneB = en.enemyZones[f.Top].PickRandom(rng);
				zoneB = ENF_Picker_Generic(en, enemy, rng, f, zoneB, 0, true);
				if (zoneB != -1)
					en.zone[zoneB].forms.Add((byte)(formid | 0x80)); // if for some reason we didn't fill any valid zone from the B-side, we simply don't add it to any zones (and thus it will never be seen)
				f.unrunnable_b = rng.Between(0, EnemizerUnrunnabilityWeight) + zoneB >= EnemizerUnrunnabilityWeight + 3 ? true : false;
			}
			else
				f.unrunnable_b = false;
			// if for some reason there were no available mons, then a B-Side is not drawn at all and no formation is added to the zones, so this slot will remain unused.  this shouldn't happen, though
			ENF_AssignPicAndPaletteBytes(enemy, f);
			f.unrunnable_a = unrunnable;
			f.surprise = surprise;
			en.LogFeatured(f);
			return f.compressData();
		}

		public byte[] ENF_DrawForcedSingleFight(EnemizerTrackingInfo en, MT19337 rng, EnemyInfo[] enemy, int zoneB, byte formid)
		{
			bool Small(byte mon) => enemy[mon].Small;
			bool Large(byte mon) => enemy[mon].Large;

			FormationInfo f = new FormationInfo();
			f.Top = en.zone[zoneB].zonemons.PickRandom(rng);
			f.tileset = enemy[f.Top].tileset;
			int limit = Large(f.Top) ? 2 : 6;
			if (enemy[f.Top].exp * limit < en.zone[zoneB].maxXP)
				f.shape = Large(f.Top) ? 0x01 : 0x00;
			else
				f.shape = 0x02;
			f.monMax[4] = en.zone[zoneB].maxXP / enemy[f.Top].exp;
			if (f.monMax[4] > en.GetMonLimit(Large(f.Top), f.shape))
				f.monMax[4] = en.GetMonLimit(Large(f.Top), f.shape);
			f.monMin[4] = en.zone[zoneB].midXP / enemy[f.Top].exp;
			if (f.monMin[4] > f.monMax[4])
				f.monMin[4] = f.monMax[4];
			if (f.monMax[4] == 0) // this should never execute but just in case
			{
				f.monMin[4] = 1;
				f.monMax[4] = 1;
			}
			if (f.monMin[4] == 0)
				f.monMin[4] = 1;
			// draw all potential mons for A-Side
			List<byte> availablemons = en.enemiesInTileset[f.tileset].Where(mon => mon != f.Top).ToList();
			switch (f.shape)
			{
				case 0x00:
					availablemons = availablemons.Where(Small).ToList();
					break;
				case 0x01:
					availablemons = availablemons.Where(Large).ToList();
					break;
			}
			f.id[1] = availablemons.PickRandom(rng);
			f.pal1 = enemy[f.Top].pal;
			f.pal2 = enemy[f.id[1]].pal; // if these are both the same, i don't really care
			availablemons = en.enemiesInTileset[f.tileset].Where(mon => mon != f.Top && mon != f.id[1] && (enemy[mon].pal == f.pal1 || enemy[mon].pal == f.pal2)).ToList();
			switch (f.shape)
			{
				case 0x00:
					availablemons = availablemons.Where(Small).ToList();
					break;
				case 0x01:
					availablemons = availablemons.Where(Large).ToList();
					break;
			}
			if (availablemons.Count > 0)
			{
				availablemons.Shuffle(rng);
				f.id[2] = availablemons[0];
				if (availablemons.Count > 1)
					f.id[3] = availablemons[1];
			}
			int zoneA = en.enemyZones[f.id[1]].PickRandom(rng); // pick a random zone to aim for from the second mon
			zoneA = ENF_Picker_Generic(en, enemy, rng, f, zoneA, 1, false); // now we actually run the generic picker
			en.zone[zoneB].forms.Add((byte)(formid | 0x80));
			if (zoneA != -1)
				en.zone[zoneA].forms.Add(formid); // if for some reason we didn't fill any valid zone from the A-side, we simply don't add it to any zones (and thus it will never be seen)
			f.surprise = 4;
			f.unrunnable_a = rng.Between(0, EnemizerUnrunnabilityWeight) + zoneA >= EnemizerUnrunnabilityWeight + 3 ? true : false;
			f.unrunnable_b = true; // the trap side is always unrunnable
			// put on the finishing touches and log and compress this formation
			ENF_AssignPicAndPaletteBytes(enemy, f);
			en.LogFeatured(f);
			return f.compressData();
		}

		public int ENE_rollEnemyStrength(int tier, int level)
		{
			int[,] returnValue = new int[,] {
					{ 3, 6, 11, 16, 19, 25, 33, 40, 50, 61},
					{ 4, 9, 15, 19, 22, 29, 38, 45, 55, 68},
					{ 6, 12, 17, 22, 26, 32, 43, 50, 60, 75},
					{ 8, 14, 20, 25, 30, 37, 49, 55, 67, 85},
					{ 9, 16, 22, 28, 34, 42, 54, 61, 75, 92},
					{ 10, 18, 25, 31, 38, 47, 60, 70, 82, 100},
					{ 12, 20, 28, 35, 42, 53, 70, 85, 95, 110} };
			if (tier < 0 || level < 1 || tier > 9 || level > 7)
				return 0; // return 0 if the tier/level are out of bounds
			return returnValue[level - 1, tier];
		}

		public int ENE_rollEnemyAccuracy(int tier, int level)
		{
			int[,] returnValue = new int[,] {
					{ 1, 6, 11, 16, 19, 25, 33, 40, 50, 61},
					{ 2, 9, 15, 19, 22, 29, 38, 45, 55, 68},
					{ 4, 12, 17, 22, 26, 32, 43, 50, 60, 75},
					{ 6, 14, 20, 25, 30, 37, 49, 55, 67, 85},
					{ 8, 16, 22, 28, 34, 42, 54, 61, 75, 92},
					{ 10, 18, 25, 31, 38, 47, 60, 70, 82, 100},
					{ 12, 20, 30, 40, 50, 60, 70, 85, 95, 110} };
			if (tier < 0 || level < 1 || tier > 9 || level > 7)
				return 0; // return 0 if the tier/level are out of bounds
			return returnValue[level - 1, tier];
		}

		public int ENE_rollEnemyAbsorb(int tier, int level)
		{
			int[,] returnValue = new int[,] {
					{ 1, 2, 3, 4, 5, 6, 7, 8, 10, 12},
					{ 2, 3, 5, 7, 9, 12, 14, 17, 20, 24},
					{ 3, 5, 7, 9, 12, 14, 17, 20, 24, 30},
					{ 4, 6, 9, 12, 15, 18, 21, 25, 29, 40},
					{ 5, 8, 11, 14, 18, 22, 26, 30, 34, 46},
					{ 8, 12, 16, 20, 25, 30, 35, 40, 45, 53},
					{ 10, 14, 19, 25, 30, 36, 42, 49, 56, 68 } };
			if (tier < 0 || level < 1 || tier > 9 || level > 7)
				return 0; // return 0 if the tier/level are out of bounds
			return returnValue[level - 1, tier];
		}

		public int ENE_rollEnemyEvade(int tier, int level)
		{
			int[,] returnValue = new int[,] {
					{ 1, 3, 6, 8, 10, 12, 15, 18, 21, 24},
					{ 3, 6, 9, 12, 15, 18, 21, 24, 27, 30},
					{ 6, 9, 12, 16, 21, 26, 30, 35, 40, 45},
					{ 9, 14, 20, 25, 30, 36, 42, 48, 54, 60},
					{ 20, 24, 30, 36, 42, 48, 54, 60, 70, 80},
					{ 36, 42, 48, 54, 60, 66, 72, 78, 90, 102},
					{ 48, 54, 64, 72, 80, 90, 100, 115, 125, 140} };
			if (tier < 0 || level < 1 || tier > 9 || level > 7)
				return 0; // return 0 if the tier/level are out of bounds
			return returnValue[level - 1, tier];
		}

		public void ENE_rollEnemyStats(MT19337 rng, EnemyInfo enemy)
		{
			// roll number of hits
			enemy.num_hits = rng.Between(1, 1 + (enemy.tier > 5 ? 5 : enemy.tier));
			// roll valid damage tiers based on num_hits
			int minDamageTier = 4 + enemy.tier / 3 - enemy.num_hits;
			if(minDamageTier < 1)
				minDamageTier = 1;
			int maxDamageTier = 8 - enemy.num_hits;
			if (enemy.Large)
				maxDamageTier++;
			if (enemy.num_hits > 1 && maxDamageTier == 7)
				maxDamageTier = 6;
			if (maxDamageTier < minDamageTier)
				maxDamageTier = minDamageTier;
			if (maxDamageTier > 7)
				maxDamageTier = 7;
			int finalDamageTier = rng.Between(minDamageTier, maxDamageTier);
			enemy.damage = ENE_rollEnemyStrength(enemy.tier, finalDamageTier);
			// roll accuracy based on tier
			enemy.accuracy = ENE_rollEnemyAccuracy(enemy.tier, rng.Between(1, 7));
			// roll absorb based on tier
			enemy.absorb = ENE_rollEnemyAbsorb(enemy.tier, rng.Between(1, 7));
			// roll evasion based on tier
			enemy.agility = rng.Between(ENE_rollEnemyEvade(enemy.tier, 1), ENE_rollEnemyEvade(enemy.tier, 7));
		}

		public void ENE_rollLargeElemental(MT19337 rng, ref EnemyInfo thisEnemy, int element)
		{
			int tier = thisEnemy.tier;
			switch (element)
			{
				case 1: // Earth Elemental
					thisEnemy.monster_type = 0b00000001;
					thisEnemy.elem_resist = 0b11101011;
					thisEnemy.elem_weakness = 0b00010000;
					break;
				case 2: // Lightning Elemental
					thisEnemy.monster_type = 0b00000001;
					thisEnemy.elem_resist = 0b11011001;
					thisEnemy.elem_weakness = 0b00000110;
					break;
				case 3: // Ice Elemental
					thisEnemy.monster_type = 0b00000001;
					thisEnemy.elem_resist = 0b11101011;
					thisEnemy.elem_weakness = 0b00010000;
					break;
				case 4: // Fire Elemental
					thisEnemy.monster_type = 0b00000001;
					thisEnemy.elem_resist = 0b11011011;
					thisEnemy.elem_weakness = 0b00100000;
					break;
				case 5: // Death Elemental
					thisEnemy.monster_type = 0b00001001;
					thisEnemy.elem_resist = 0b10101011;
					thisEnemy.elem_weakness = 0b00010000;
					break;
				case 6: // Time Elemental
					thisEnemy.monster_type = 0b00000001;
					thisEnemy.elem_resist = 0b10001111;
					thisEnemy.elem_weakness = 0b01110000;
					break;
				case 7: // Poison Elemental
					thisEnemy.monster_type = 0b10000001;
					thisEnemy.elem_resist = 0b10001011;
					thisEnemy.elem_weakness = 0b00000000;
					break;
				case 8: // Status Elemental
					thisEnemy.monster_type = 0b00000001;
					thisEnemy.elem_resist = 0b11111011;
					thisEnemy.elem_weakness = 0b00000000;
					if (thisEnemy.AIscript == 0xFF)
					{
						thisEnemy.AIscript = ENE_PickForcedAIScript(tier, rng);
					}
					break;
				default: // Raw Mana Elemental
					thisEnemy.monster_type = 0b00000001;
					thisEnemy.elem_resist = 0b11111011;
					thisEnemy.elem_weakness = 0b00000000;
					break;
			}
		}

		public void ENE_rollSmallElemental(MT19337 rng, ref EnemyInfo thisEnemy, int element)
		{
			int tier = thisEnemy.tier;
			switch (element)
			{
				case 1: // Earth Elemental
					thisEnemy.monster_type = 0b00000001;
					thisEnemy.elem_resist = 0b10101011;
					thisEnemy.elem_weakness = 0b01010000;
					break;
				case 2: // Lightning Elemental
					thisEnemy.monster_type = 0b00000001;
					thisEnemy.elem_resist = 0b11011001;
					thisEnemy.elem_weakness = 0b00000110;
					break;
				case 3: // Water Elemental
					thisEnemy.monster_type = 0b00000001;
					thisEnemy.elem_resist = 0b10011011;
					thisEnemy.elem_weakness = 0b00100000;
					break;
				case 4: // Fire Elemental
					thisEnemy.monster_type = 0b00000001;
					thisEnemy.elem_resist = 0b10011001;
					thisEnemy.elem_weakness = 0b00100000;
					break;
				case 5: // Death Elemental
					thisEnemy.monster_type = 0b00001001;
					thisEnemy.elem_resist = 0b10101011;
					thisEnemy.elem_weakness = 0b00010000;
					break;
				case 6: // Time Elemental
					thisEnemy.monster_type = 0b00000001;
					thisEnemy.elem_resist = 0b10001111;
					thisEnemy.elem_weakness = 0b01110000;
					break;
				case 7: // Poison Elemental
					thisEnemy.monster_type = 0b10000001;
					thisEnemy.elem_resist = 0b10001011;
					thisEnemy.elem_weakness = 0b00000100;
					break;
				case 8: // Status Elemental
					thisEnemy.monster_type = 0b00000001;
					thisEnemy.elem_resist = 0b11111011;
					thisEnemy.elem_weakness = 0b00000000;
					if (thisEnemy.AIscript == 0xFF)
					{
						thisEnemy.AIscript = ENE_PickForcedAIScript(tier, rng);
					}
					break;
				default: // Air Elemental
					thisEnemy.monster_type = 0b00000001;
					thisEnemy.elem_resist = 0b10001011;
					thisEnemy.elem_weakness = 0b00000000;
					break;
			}
		}

		public void ENE_rollDragon(MT19337 rng, ref EnemyInfo thisEnemy, int element)
		{
			int tier = thisEnemy.tier;
			switch (element)
			{
				case 1: // Earth Elemental
					thisEnemy.monster_type = 0b00000010;
					thisEnemy.elem_resist = 0b11100000;
					thisEnemy.elem_weakness = 0b00000010;
					break;
				case 2: // Lightning Elemental
					thisEnemy.monster_type = 0b00000010;
					thisEnemy.elem_resist = 0b11000000;
					thisEnemy.elem_weakness = 0b00010000;
					break;
				case 3: // Ice Elemental
					thisEnemy.monster_type = 0b00000010;
					thisEnemy.elem_resist = 0b10100010;
					thisEnemy.elem_weakness = 0b01010000;
					break;
				case 4: // Fire Elemental
					thisEnemy.monster_type = 0b00000010;
					thisEnemy.elem_resist = 0b10010000;
					thisEnemy.elem_weakness = 0b00100010;
					break;
				case 5: // Death Elemental
					thisEnemy.monster_type = 0b00001010;
					thisEnemy.elem_resist = 0b10101011;
					thisEnemy.elem_weakness = 0b00010000;
					break;
				case 6: // Time Elemental
					thisEnemy.monster_type = 0b00000011;
					thisEnemy.elem_resist = 0b10001111;
					thisEnemy.elem_weakness = 0b01110000;
					break;
				case 7: // Poison Elemental
					thisEnemy.monster_type = 0b00000010;
					thisEnemy.elem_resist = 0b10000000;
					thisEnemy.elem_weakness = 0b00100000;
					break;
				case 8: // Status Elemental
					thisEnemy.monster_type = 0b00000011;
					thisEnemy.elem_resist = 0b01110001;
					thisEnemy.elem_weakness = 0b00000000;
					if (thisEnemy.AIscript == 0xFF)
					{
						thisEnemy.AIscript = ENE_PickForcedAIScript(tier, rng);
					}
					break;
				default: // Standard Dragon
					thisEnemy.monster_type = 0b00000010;
					thisEnemy.elem_resist = 0b00000000;
					thisEnemy.elem_weakness = 0b00000000;
					break;
			}
		}

		public byte ENE_PickForcedAIScript(int tier, MT19337 rng)
		{
			if (tier >= 0 && tier <= 2)
			{
				return 0x1A; // RockGol script
			}
			else if (tier >= 3 && tier <= 4)
			{
				switch (rng.Between(0, 2))
				{
					case 0:
						return 0x0D; // R.Goyle script
					case 1:
						return 0x0A; // Mancat script
					default:
						return 0x06; // WzOgre script
				}
			}
			else if (tier == 5)
			{
				switch (rng.Between(0, 2))
				{
					case 0:
						return 0x0C; // WzVamp script
					case 1:
						return 0x19; // MudGol script
					default:
						return 0x12; // Naga script
				}
			}
			else if (tier >= 6 && tier <= 7)
			{
				switch (rng.Between(0, 2))
				{
					case 0:
						return 0x1D; // Mage script
					case 1:
						return 0x1E; // Fighter script
					default:
						return 0x08; // Eye script
				}
			}
			else if (tier >= 8 && tier <= 9)
				return 0x1C; // tier 8-9 uses EVILMAN script
			else
				return 0xFF; // invalid tier returns no script
		}

		private void DoEnemizer_EnemyPatternTablesOnly(MT19337 rng, byte[] patterntabledata, EnemyInfo[] enemy, EnemizerTrackingInfo en)
		{
			// in this function we shuffle the pattern tables and enemy palettes only (we shuffle palettes because otherwise eligible formations would be even more constrained)
			List<byte> smallImages = new List<byte> {   0b00000000, 0b00000010, 0b00000100, 0b00000110, 0b00001000, 0b00001010, 0b00001100, 0b00001110,
														0b00010000, 0b00010010, 0b00010100, 0b00010110, 0b00011000, 0b00011010, 0b00011100, 0b00011110,
														0b00100000, 0b00100010, 0b00100100, 0b00100110, 0b00101000, 0b00101010, 0b00101100, 0b00101110,
														0b00110000, 0b00110010 };
			List<byte> largeImages = new List<byte> {   0b00000001, 0b00000011, 0b00000101, 0b00000111, 0b00001001, 0b00001011, 0b00001101, 0b00001111,
														0b00010001, 0b00010011, 0b00010101, 0b00010111, 0b00011001, 0b00011011, 0b00011101, 0b00011111,
														0b00100001, 0b00100011, 0b00100101, 0b00100111, 0b00101001, 0b00101011, 0b00101101, 0b00101111,
														0b00110001, 0b00110011 };
			smallImages.Shuffle(rng);
			largeImages.Shuffle(rng);
			byte[] newPatternTableData = new byte[0x6800];
			patterntabledata.CopyTo(newPatternTableData, 0);
			List<byte> newEnemyImageLUT = new List<byte> {  0, 2, 1, 3, 4, 6, 5, 7,
															8, 10, 9, 11, 12, 14, 13, 15,
															16, 18, 17, 19, 20, 22, 21, 23,
															24, 26, 25, 27, 28, 30, 29, 31,
															32, 34, 33, 35, 36, 38, 37, 39,
															40, 42, 41, 43, 44, 46, 45, 47,
															48, 50, 49, 51 };
			for (int i = 0; i < 13; ++i)
			{
				int small1offset = 0x800 * (smallImages[i * 2] / 4) + (smallImages[i * 2] % 4 == 0 ? 0x120 : 0x220);
				int small2offset = 0x800 * (smallImages[i * 2 + 1] / 4) + (smallImages[i * 2 + 1] % 4 == 0 ? 0x120 : 0x220);
				int large1offset = 0x800 * (largeImages[i * 2] / 4) + (largeImages[i * 2] % 4 == 1 ? 0x320 : 0x560);
				int large2offset = 0x800 * (largeImages[i * 2 + 1] / 4) + (largeImages[i * 2 + 1] % 4 == 1 ? 0x320 : 0x560);
				newEnemyImageLUT[i * 4] = smallImages[i * 2];
				for (int j = 0; j < 0x100; ++j)
				{
					newPatternTableData[i * 0x800 + 0x120 + j] = patterntabledata[small1offset + j];
				}
				newEnemyImageLUT[i * 4 + 2] = smallImages[i * 2 + 1];
				for (int j = 0; j < 0x100; ++j)
				{
					newPatternTableData[i * 0x800 + 0x220 + j] = patterntabledata[small2offset + j];
				}
				newEnemyImageLUT[i * 4 + 1] = largeImages[i * 2];
				for (int j = 0; j < 0x240; ++j)
				{
					newPatternTableData[i * 0x800 + 0x320 + j] = patterntabledata[large1offset + j];
				}
				newEnemyImageLUT[i * 4 + 3] = largeImages[i * 2 + 1];
				for (int j = 0; j < 0x240; ++j)
				{
					newPatternTableData[i * 0x800 + 0x560 + j] = patterntabledata[large2offset + j];
				}
			}
			newPatternTableData.CopyTo(patterntabledata, 0);

			for (int i = 0; i < GenericTilesetsCount; ++i) // generate the palettes for each tileset
			{
				en.palettesInTileset[i].Clear();
				while (en.palettesInTileset[i].Count < 4)
				{
					int newPal = rng.Between(0, 0x3F);
					if (en.palettesInTileset[i].Contains((byte)newPal))
						continue;
					en.palettesInTileset[i].Add((byte)newPal);
				}
			}
			for (int i = 0; i < GenericTilesetsCount; ++i)// clear the list of enemies in each tileset
				en.enemiesInTileset[i].Clear();
			List<byte>[] enemyImagePalettes = new List<byte>[52]; // create lists of palettes used for each enemy image
			for (int i = 0; i < 52; ++i)
				enemyImagePalettes[i] = new List<byte> { };
			for (byte i = 0; i < Enemy.Lich; ++i) // reassign enemy images to the appropriate image in the pattern table and pick a random palette of those that were drawn
			{
				enemy[i].image = (byte)newEnemyImageLUT.IndexOf(enemy[i].image); // assign the enemy's image to the appropriate pattern table
				switch (i) // fix palettes for Pirate, Garland, and Astos to their originals
				{
					case Enemy.Pirate:
						enemy[i].pal = 0x0B;
						break;
					case Enemy.Garland:
						enemy[i].pal = 0x13;
						break;
					case Enemy.Astos:
						enemy[i].pal = 0x06;
						break;
					default:
						if (enemy[i].tier != -1)
							en.enemiesInTileset[enemy[i].tileset].Add(i); // add enemy to enemiesInTileset unless it is a boss
						List<byte> acceptablepalettes = en.palettesInTileset[enemy[i].tileset].Except(enemyImagePalettes[enemy[i].image]).ToList();
						if (acceptablepalettes.Count == 0)
							acceptablepalettes = en.palettesInTileset[enemy[i].tileset].ToList();
						enemy[i].pal = acceptablepalettes.PickRandom(rng);
						enemyImagePalettes[enemy[i].image].Add(enemy[i].pal);
						break;
				}
			}
			for (int i = 0; i < GenericTilesetsCount; ++i) // remove palettes from tilesets where there are no mons using those palettes
			{
				List<byte> palRemoveList = new List<byte> { };
				foreach (byte pal in en.palettesInTileset[i])
				{
					bool nopalettematch = true;
					foreach (byte mon in en.enemiesInTileset[i])
					{
						if (enemy[mon].pal == pal)
						{
							nopalettematch = false;
							break;
						}
					}
					if (nopalettematch)
					{
						palRemoveList.Add(pal);
					}
				}
				foreach (byte pal in palRemoveList)
				{
					en.palettesInTileset[i].Remove(pal);
				}
			}
		}

		private bool DoEnemizer_Enemies(MT19337 rng, EnemyInfo[] enemy, byte[] patterntabledata, SpellInfo[] spell, EnemySkillInfo[] skill, EnemyScriptInfo[] script, string[] enemyNames, string[] skillNames, bool shuffledSkillsOn, EnemizerTrackingInfo en)
		{
			List<byte> enemyImageLUT = new List<byte> { 0b00000000, 0b00000010, 0b00000001, 0b00000011, 0b00000100, 0b00000110, 0b00000101, 0b00000111,
														0b00001000, 0b00001010, 0b00001001, 0b00001011, 0b00001100, 0b00001110, 0b00001101, 0b00001111,
														0b00010000, 0b00010010, 0b00010001, 0b00010011, 0b00010100, 0b00010110, 0b00010101, 0b00010111,
														0b00011000, 0b00011010, 0b00011001, 0b00011011, 0b00011100, 0b00011110, 0b00011101, 0b00011111,
														0b00100000, 0b00100010, 0b00100001, 0b00100011, 0b00100100, 0b00100110, 0b00100101, 0b00100111,
														0b00101000, 0b00101010, 0b00101001, 0b00101011, 0b00101100, 0b00101110, 0b00101101, 0b00101111,
														0b00110000, 0b00110010, 0b00110001, 0b00110011 }; // these are the default tilesets + pics of the enemies we wish to shuffle
			// first, we shuffle what images appear in which pattern tables.  small enemies can replace small enemies, and large enemies can replace large
			List<byte> smallImages = new List<byte> {	0b00000000, 0b00000010, 0b00000100, 0b00000110, 0b00001000, 0b00001010, 0b00001100, 0b00001110,
														0b00010000, 0b00010010, 0b00010100, 0b00010110, 0b00011000, 0b00011010, 0b00011100, 0b00011110,
														0b00100000, 0b00100010, 0b00100100, 0b00100110, 0b00101000, 0b00101010, 0b00101100, 0b00101110,
														0b00110000, 0b00110010 };
			List<byte> largeImages = new List<byte> {   0b00000001, 0b00000011, 0b00000101, 0b00000111, 0b00001001, 0b00001011, 0b00001101, 0b00001111,
														0b00010001, 0b00010011, 0b00010101, 0b00010111, 0b00011001, 0b00011011, 0b00011101, 0b00011111,
														0b00100001, 0b00100011, 0b00100101, 0b00100111, 0b00101001, 0b00101011, 0b00101101, 0b00101111,
														0b00110001, 0b00110011 };
			smallImages.Shuffle(rng);
			largeImages.Shuffle(rng);
			byte[] newPatternTableData = new byte[0x6800];
			patterntabledata.CopyTo(newPatternTableData, 0);
			List<byte> newEnemyImageLUT = new List<byte> {	0, 2, 1, 3, 4, 6, 5, 7,
															8, 10, 9, 11, 12, 14, 13, 15,
															16, 18, 17, 19, 20, 22, 21, 23,
															24, 26, 25, 27, 28, 30, 29, 31,
															32, 34, 33, 35, 36, 38, 37, 39,
															40, 42, 41, 43, 44, 46, 45, 47,
															48, 50, 49, 51 };
			for(int i = 0; i < 13; ++i)
			{
				int small1offset = 0x800 * (smallImages[i * 2] / 4) + (smallImages[i * 2] % 4 == 0 ? 0x120 : 0x220);
				int small2offset = 0x800 * (smallImages[i * 2 + 1] / 4) + (smallImages[i * 2 + 1] % 4 == 0 ? 0x120 : 0x220);
				int large1offset = 0x800 * (largeImages[i * 2] / 4) + (largeImages[i * 2] % 4 == 1 ? 0x320 : 0x560);
				int large2offset = 0x800 * (largeImages[i * 2 + 1] / 4) + (largeImages[i * 2 + 1] % 4 == 1 ? 0x320 : 0x560);
				newEnemyImageLUT[i * 4] = smallImages[i * 2];
				for(int j = 0; j < 0x100; ++j)
				{
					newPatternTableData[i * 0x800 + 0x120 + j] = patterntabledata[small1offset + j];
				}
				newEnemyImageLUT[i * 4 + 2] = smallImages[i * 2 + 1];
				for (int j = 0; j < 0x100; ++j)
				{
					newPatternTableData[i * 0x800 + 0x220 + j] = patterntabledata[small2offset + j];
				}
				newEnemyImageLUT[i * 4 + 1] = largeImages[i * 2];
				for (int j = 0; j < 0x240; ++j)
				{
					newPatternTableData[i * 0x800 + 0x320 + j] = patterntabledata[large1offset + j];
				}
				newEnemyImageLUT[i * 4 + 3] = largeImages[i * 2 + 1];
				for (int j = 0; j < 0x240; ++j)
				{
					newPatternTableData[i * 0x800 + 0x560 + j] = patterntabledata[large2offset + j];
				}
			}
			newPatternTableData.CopyTo(patterntabledata, 0);
			List<byte> enemyImages = new List<byte> { };
			for (int i = 0; i < Enemy.Lich; ++i)
				enemyImages.Add(enemy[i].image); // we are reproducing the iamge array because we only want to shuffle the images, not the monsters themselves
			List<string>[] monsterClassVariants = new List<string>[52]; // monster class modifiers that have effects on stats
			monsterClassVariants[0] = new List<string> { "Fr", "R.", "Z.", "Wr", "Wz", "Sea" }; // imp variants with special perks
			monsterClassVariants[1] = new List<string> { "Fr", "R.", "Z.", "Wz", "Sea" }; // iguana variants with special perks
			monsterClassVariants[2] = new List<string> { "Fr", "R.", "Z.", "Wr", "Wz" }; // wolf variants with special perks
			monsterClassVariants[3] = new List<string> { "Fr", "R.", "Z.", "Wz", "Sea" }; // giant variants with special perks
			monsterClassVariants[4] = new List<string> { "Fr", "Z.", "Wr", "Wz" }; // sahag variants with special perks
			monsterClassVariants[5] = new List<string> { "Fr", "Z." }; // shark variants with special perks
			monsterClassVariants[6] = new List<string> { "Fr", "R.", "Z.", "Wz" }; // pirate variants with special perks
			monsterClassVariants[7] = new List<string> { "Fr", "Z." }; // oddeye variants with special perks
			monsterClassVariants[8] = new List<string> { "Wz" }; // bone variants with special perks
			monsterClassVariants[9] = new List<string> { "Fr", "R.", "Z.", "Wz", "Sea" }; // ogre variants with special perks
			monsterClassVariants[10] = new List<string> { "Fr", "R.", "Z.", "Wr", "Wz", "Sea" }; // creep variants with special perks
			monsterClassVariants[11] = new List<string> { "Fr", "R.", "Z.", "Wr" }; // hyena variants with special perks
			monsterClassVariants[12] = new List<string> { "Fr", "R.", "Z.", "Sea" }; // asp variants with special perks
			monsterClassVariants[13] = new List<string> { "Fr", "R.", "Z.", "Wr", "Wz", "Sea" }; // bull variants with special perks
			monsterClassVariants[14] = new List<string> { "Fr", "R.", "Z.", "Sea" }; // crab variants with special perks
			monsterClassVariants[15] = new List<string> { "Fr", "R.", "Z.", "Wz", "Sea" }; // troll variants with special perks
			monsterClassVariants[16] = new List<string> { "Wz" }; // ghost variants with special perks
			monsterClassVariants[17] = new List<string> { "Fr", "R.", "Z.", "Sea" }; // worm variants with special perks
			monsterClassVariants[18] = new List<string> { "Sea" }; // wight variants with special perks
			monsterClassVariants[19] = new List<string> { "Fr", "R.", "Z.", "Sea" }; // eye variants with special perks
			monsterClassVariants[20] = new List<string> { "Fr", "R.", "Z.", "Wz", "Sea" }; // medusa variants with special perks
			monsterClassVariants[21] = new List<string> { "Fr", "R.", "Z.", "Sea" }; // pede variants with special perks
			monsterClassVariants[22] = new List<string> { "Fr", "R.", "Z.", "Wz" }; // catman variants with special perks
			monsterClassVariants[23] = new List<string> { "Fr", "R.", "Z.", "Wr" }; // tiger variants with special perks
			monsterClassVariants[24] = new List<string> { "Wz" }; // vamp variants with special perks
			monsterClassVariants[25] = new List<string> { }; // large elem variants with special perks
			monsterClassVariants[26] = new List<string> { "Fr", "R.", "Z.", "Wz", "Sea" }; // goyle variants with special perks
			monsterClassVariants[27] = new List<string> { }; // drake 1 variants with special perks
			monsterClassVariants[28] = new List<string> { "Wz" }; // flan variants with special perks
			monsterClassVariants[29] = new List<string> { "Fr", "R.", "Z.", "Wz", "Sea" }; // manticore variants with special perks
			monsterClassVariants[30] = new List<string> { "Fr", "R.", "Z.", "Sea" }; // spider variants with special perks
			monsterClassVariants[31] = new List<string> { "Fr", "R.", "Z." }; // ankylo variants with special perks
			monsterClassVariants[32] = new List<string> { "Wz" }; // mummy variants with special perks
			monsterClassVariants[33] = new List<string> { "Fr", "R.", "Z.", "Wr", "Wz" }; // wyvern variants with special perks
			monsterClassVariants[34] = new List<string> { "Fr", "R.", "Z." }; // bird variants with special perks
			monsterClassVariants[35] = new List<string> { "Fr", "R.", "Z.", "Sea" }; // tyro variants with special perks
			monsterClassVariants[36] = new List<string> { "Fr", "Z." }; // caribe variants with special perks
			monsterClassVariants[37] = new List<string> { "Fr", "Z.", "Wz" }; // ocho variants with special perks
			monsterClassVariants[38] = new List<string> { "Fr", "Z.", "Wr" }; // gator variants with special perks
			monsterClassVariants[39] = new List<string> { "Fr", "R.", "Z.", "Sea" }; // hydra variants with special perks
			monsterClassVariants[40] = new List<string> { }; // bot variants with special perks
			monsterClassVariants[41] = new List<string> { "Fr", "Z.", "Sea" }; // naga variants with special perks
			monsterClassVariants[42] = new List<string> { }; // small elem variants with special perks
			monsterClassVariants[43] = new List<string> { "Z.", "Wr", "Wz" }; // chimera variants with special perks
			monsterClassVariants[44] = new List<string> { "Z.", "Wz" }; // piscodemon variants with special perks
			monsterClassVariants[45] = new List<string> { }; // dragon 2 variants with special perks
			monsterClassVariants[46] = new List<string> { "Fr", "R.", "Z.", "Wz" }; // knight variants with special perks
			monsterClassVariants[47] = new List<string> { }; // golem variants with special perks
			monsterClassVariants[48] = new List<string> { "Fr", "R.", "Z.", "Wz" }; // badman variants with special perks
			monsterClassVariants[49] = new List<string> { "Fr", "R.", "Z.", "Wr", "Sea" }; // pony variants with special perks
			monsterClassVariants[50] = new List<string> { "Fr", "R.", "Z.", "Sea" }; // elf variants with special perks
			monsterClassVariants[51] = new List<string> { }; // mech variants with special perks
			List<string>[] monsterNameVariants = new List<string>[52]; // name variants for monsters to prevent duplicates
			bool[] monsterBaseNameUsed = new bool[52]; // true if the base name has been used, false if not
			for (int i = 0; i < 52; ++i)
			{
				monsterBaseNameUsed[i] = false; // if enemy is not part of a variant class, by default it uses the base name for the monster
				monsterNameVariants[i] = new List<string> { "A.", "B.", "C.", "D.", "E.", "F.", "G.", "H.", "I.", "K.", "L.", "M.", "N.", "P.", "S.", "T.", "V.", "W.", "X." };
			}
			monsterBaseNameUsed[25] = true; // large elemental
			monsterBaseNameUsed[27] = true; // dragon 1
			monsterBaseNameUsed[42] = true; // small elemental
			monsterBaseNameUsed[45] = true; // dragon 2
			enemyImageLUT.Shuffle(rng); // shuffle the LUT - whatever image was there in vanilla will be replaced with what is at its position in the LUT
			enemyImages.Shuffle(rng); // and shuffle the enemy images themselves
			for (int i = 0; i < GenericTilesetsCount; ++i) // generate the palettes for each tileset
			{
				en.palettesInTileset[i].Clear();
				while (en.palettesInTileset[i].Count < 4)
				{
					int newPal = rng.Between(0, 0x3F);
					if (en.palettesInTileset[i].Contains((byte)newPal))
						continue;
					en.palettesInTileset[i].Add((byte)newPal);
				}
			}
			for (int i = 0; i < GenericTilesetsCount; ++i)// clear the list of enemies in each tileset
				en.enemiesInTileset[i].Clear();
			List<int> elementalsSelected = new List<int> { 0, 1, 2, 3, 4, 5, 6, 7, 8 }; // List of elementals already selected
			List<int> dragonsSelected = new List<int> { 0, 1, 2, 3, 4, 5, 6, 7, 8 };
			List<byte>[] enemyImagePalettes = new List<byte>[52]; // create lists of palettes used for each enemy image
			for (int i = 0; i < 52; ++i)
				enemyImagePalettes[i] = new List<byte> { };
			for (byte i = 0; i < Enemy.Lich; ++i) // now start generating the enemies themselves.  we stop before Lich and make special exceptions for various monsters
			{
				List<MonsterPerks> perks = new List<MonsterPerks> { MonsterPerks.PERK_GAINSTAT10, MonsterPerks.PERK_LOSESTAT10 }; // list of perks this monster is eligible to receive
				enemy[i].image = enemyImageLUT[enemyImages[i]]; // assign the monster's pic
				if (enemy[i].tileset >= GenericTilesetsCount)
					return false; // enemy was outside the boundaries for acceptable tilesets - abort enemizer
				bool oldSize = enemy[i].Large; // remember if enemy was small (false) or large (true)
				List<byte> acceptablepalettes = en.palettesInTileset[enemy[i].tileset].Except(enemyImagePalettes[enemy[i].image]).ToList();
				if(acceptablepalettes.Count == 0)
					acceptablepalettes = en.palettesInTileset[enemy[i].tileset].ToList();
				enemy[i].pal = acceptablepalettes.PickRandom(rng);
				enemyImagePalettes[enemy[i].image].Add(enemy[i].pal);
				// generate the stats for each monster
				if (enemy[i].tier == -1)
				{
					// generate special monsters
					switch (i)
					{
						case Enemy.Imp:
							enemyNames[i] = "CHAMP";
							enemy[i].elem_weakness = 0b11111111;
							enemy[i].monster_type = 0b01111111;
							break;
						case Enemy.Pirate:
							enemy[i].image = (byte)newEnemyImageLUT.IndexOf(6); // image forced to where the PIRATE image was moved
							enemy[i].pal = 0x0B;
							break;
						case Enemy.Phantom:
							break;
						case Enemy.Garland:
							enemy[i].image = (byte)newEnemyImageLUT.IndexOf(46); // image forced to where the GARLAND image was moved
							enemy[i].pal = 0x13;
							break;
						case Enemy.Astos:
							enemy[i].image = (byte)newEnemyImageLUT.IndexOf(50); // image forced to where the ASTOS image was moved
							enemy[i].pal = 0x06;
							break;
						case Enemy.WarMech:
							switch (newEnemyImageLUT[enemy[i].image])
							{
								case 0:
									enemyNames[i] = "WarIMP";
									break;
								case 1:
									enemyNames[i] = "WarAGAMA";
									break;
								case 2:
									enemyNames[i] = "WarWOLF";
									break;
								case 3:
									enemyNames[i] = "WarGIANT";
									break;
								case 4:
									enemyNames[i] = "WarSAHAG";
									break;
								case 5:
									enemyNames[i] = "WarSHARK";
									break;
								case 6:
									enemyNames[i] = "WarBRUTE";
									break;
								case 7:
									enemyNames[i] = "WarEYE";
									break;
								case 8:
									enemyNames[i] = "WarBONE";
									break;
								case 9:
									enemyNames[i] = "WarHYENA";
									break;
								case 10:
									enemyNames[i] = "WarCREEP";
									break;
								case 11:
									enemyNames[i] = "WarOGRE";
									break;
								case 12:
									enemyNames[i] = "WarSNAKE";
									break;
								case 13:
									enemyNames[i] = "WarBULL";
									break;
								case 14:
									enemyNames[i] = "WarLBSTR";
									break;
								case 15:
									enemyNames[i] = "WarTROLL";
									break;
								case 16:
									enemyNames[i] = "WarGHOST";
									break;
								case 17:
									enemyNames[i] = "WarWORM";
									break;
								case 18:
									enemyNames[i] = "WarGEIST";
									break;
								case 19:
									enemyNames[i] = "WarEYE";
									break;
								case 20:
									enemyNames[i] = "WarLAMIA";
									break;
								case 21:
									enemyNames[i] = "WarPEDE";
									break;
								case 22:
									enemyNames[i] = "WarCAT";
									break;
								case 23:
									enemyNames[i] = "WarTIGER";
									break;
								case 24:
									enemyNames[i] = "WarVAMP";
									break;
								case 25:
									enemyNames[i] = "WarDJINN";
									break;
								case 26:
									enemyNames[i] = "WarGOYLE";
									break;
								case 27:
									enemyNames[i] = "WarDRAKE";
									break;
								case 28:
									enemyNames[i] = "WarOOZE";
									break;
								case 29:
									enemyNames[i] = "WarSPHNX";
									break;
								case 30:
									enemyNames[i] = "WarBUG";
									break;
								case 31:
									enemyNames[i] = "WarTURTL";
									break;
								case 32:
									enemyNames[i] = "WarMUMMY";
									break;
								case 33:
									enemyNames[i] = "WarWYRM";
									break;
								case 34:
									enemyNames[i] = "WarBIRD";
									break;
								case 35:
									enemyNames[i] = "WarREX";
									break;
								case 36:
									enemyNames[i] = "WarFISH";
									break;
								case 37:
									enemyNames[i] = "WarOCHO";
									break;
								case 38:
									enemyNames[i] = "WarGATOR";
									break;
								case 39:
									enemyNames[i] = "WarHYDRA";
									break;
								case 40:
									enemyNames[i] = "WarMECH";
									break;
								case 41:
									enemyNames[i] = "WarNAGA";
									break;
								case 42:
									enemyNames[i] = "WarWATER";
									break;
								case 43:
									enemyNames[i] = "WarBEAST";
									break;
								case 44:
									enemyNames[i] = "WarPISCO";
									break;
								case 45:
									enemyNames[i] = "WarDRAKE";
									break;
								case 46:
									enemyNames[i] = "WarGRLND";
									break;
								case 47:
									enemyNames[i] = "WarGOLEM";
									break;
								case 48:
									enemyNames[i] = "WarMAN";
									break;
								case 49:
									enemyNames[i] = "WarPONY";
									break;
								case 50:
									enemyNames[i] = "WarELF";
									break;
								case 51:
									enemyNames[i] = "WarMECH";
									break;
							}
							break;
					}
				}
				else
				{
					en.enemiesInTileset[enemy[i].tileset].Add(i); // add enemy to enemiesInTileset unless it is a boss
					// adjust HP, EXP, and GP if enemy changes size
					if (oldSize && enemy[i].Small) // large became small
					{
						enemy[i].hp *= 4;
						enemy[i].hp /= 5;
						enemy[i].exp *= 9;
						enemy[i].exp /= 10;
						enemy[i].gp *= 9;
						enemy[i].gp /= 10;
					}
					else if (!oldSize && enemy[i].Large) // small became large
					{
						enemy[i].hp *= 5;
						enemy[i].hp /= 4;
						enemy[i].exp *= 10;
						enemy[i].exp /= 9;
						enemy[i].gp *= 10;
						enemy[i].gp /= 9;
					}
					int elemental = 9; // track elemental affinity for certain classes of monsters (elementals and dragons)
					// generate monster's base elemental weakness/resist, type, and base name based on the monster image type
					enemy[i].critrate = 1; // default crit rate of 1 for most enemies
					// generate most enemy BASE stats
					ENE_rollEnemyStats(rng, enemy[i]);
					switch (newEnemyImageLUT[enemy[i].image])
					{
						case 0: // Imp
							enemyNames[i] = "IMP";
							enemy[i].monster_type = 0b00000100;
							enemy[i].elem_resist = 0b00000000;
							enemy[i].elem_weakness = 0b00000000;
							break;
						case 1: // Iguana
							enemyNames[i] = "SAUR";
							enemy[i].monster_type = 0b00000010;
							enemy[i].elem_resist = 0b00000000;
							enemy[i].elem_weakness = 0b00000000;
							break;
						case 2: // Wolf
							enemyNames[i] = "WOLF";
							enemy[i].monster_type = 0b00000000;
							enemy[i].elem_resist = 0b00000000;
							enemy[i].elem_weakness = 0b00000000;
							break;
						case 3: // Giant
							enemyNames[i] = "GIANT";
							enemy[i].monster_type = 0b00000100;
							enemy[i].elem_resist = 0b00000000;
							enemy[i].elem_weakness = 0b00000000;
							break;
						case 4: // Sahag
							enemyNames[i] = "SAHAG";
							enemy[i].monster_type = 0b00100000;
							enemy[i].elem_resist = 0b10010000;
							enemy[i].elem_weakness = 0b01000000;
							break;
						case 5: // Shark
							enemyNames[i] = "SHARK";
							enemy[i].monster_type = 0b00100000;
							enemy[i].elem_resist = 0b10010000;
							enemy[i].elem_weakness = 0b01000000;
							break;
						case 6: // Pirate
							enemyNames[i] = "BRUTE";
							enemy[i].monster_type = 0b00000000;
							enemy[i].elem_resist = 0b10000000;
							enemy[i].elem_weakness = 0b00000000;
							break;
						case 7: // Oddeye
							enemyNames[i] = "EYES";
							enemy[i].monster_type = 0b00100000;
							enemy[i].elem_resist = 0b10010000;
							enemy[i].elem_weakness = 0b01000000;
							break;
						case 8: // Skeleton
							enemyNames[i] = "BONE";
							enemy[i].monster_type = 0b00001000;
							enemy[i].elem_resist = 0b00101011;
							enemy[i].elem_weakness = 0b00010000;
							break;
						case 9: // Hyena
							enemyNames[i] = "DOG";
							enemy[i].monster_type = 0b00000000;
							enemy[i].elem_resist = 0b00000000;
							enemy[i].elem_weakness = 0b00000000;
							break;
						case 10: // Creep
							enemyNames[i] = "CRAWL";
							enemy[i].monster_type = 0b00000000;
							enemy[i].elem_resist = 0b00000000;
							enemy[i].elem_weakness = 0b00010000;
							break;
						case 11: // Ogre
							enemyNames[i] = "OGRE";
							enemy[i].monster_type = 0b00000100;
							enemy[i].elem_resist = 0b00000000;
							enemy[i].elem_weakness = 0b00000000;
							break;
						case 12: // Asp
							enemyNames[i] = "ASP";
							enemy[i].critrate = rng.Between(1, 20);
							enemy[i].monster_type = 0b00000010;
							enemy[i].elem_resist = 0b00000000;
							enemy[i].elem_weakness = 0b00000000;
							break;
						case 13: // Bull
							enemyNames[i] = "BULL";
							enemy[i].monster_type = 0b00000000;
							enemy[i].elem_resist = 0b00000000;
							enemy[i].elem_weakness = 0b00000000;
							break;
						case 14: // Crab
							enemyNames[i] = "CRAB";
							enemy[i].monster_type = 0b00000000;
							enemy[i].elem_resist = 0b00000000;
							enemy[i].elem_weakness = 0b00000000;
							break;
						case 15: // Troll
							enemyNames[i] = "TROLL";
							enemy[i].monster_type = 0b10000000;
							enemy[i].elem_resist = 0b00000000;
							enemy[i].elem_weakness = 0b00000000;
							break;
						case 16: // Spectral Undead
							enemyNames[i] = "GHOST";
							enemy[i].monster_type = 0b00001001;
							enemy[i].elem_resist = 0b10101011;
							enemy[i].elem_weakness = 0b00010000;
							break;
						case 17: // Worm
							enemyNames[i] = "WORM";
							enemy[i].monster_type = 0b00000000;
							enemy[i].elem_resist = 0b10000000;
							enemy[i].elem_weakness = 0b00000000;
							break;
						case 18: // Zombie Undead
							enemyNames[i] = "WIGHT";
							enemy[i].monster_type = 0b00001000;
							enemy[i].elem_resist = 0b00101011;
							enemy[i].elem_weakness = 0b00010000;
							break;
						case 19: // Eyes that are totally not Beholders plz no sue
							enemyNames[i] = "EYE";
							enemy[i].monster_type = 0b01000000;
							enemy[i].elem_resist = 0b10000000;
							enemy[i].elem_weakness = 0b00000000;
							break;
						case 20: // Medusa
							enemyNames[i] = "LAMIA";
							enemy[i].monster_type = 0b00000000;
							enemy[i].elem_resist = 0b00000000;
							enemy[i].elem_weakness = 0b00000000;
							break;
						case 21: // Pede
							enemyNames[i] = "PEDE";
							enemy[i].monster_type = 0b00000000;
							enemy[i].elem_resist = 0b00000000;
							enemy[i].elem_weakness = 0b00000000;
							break;
						case 22: // Were
							enemyNames[i] = "CAT";
							enemy[i].monster_type = 0b00010000;
							enemy[i].elem_resist = 0b00000000;
							enemy[i].elem_weakness = 0b00000000;
							break;
						case 23: // Tiger
							enemyNames[i] = "TIGER";
							enemy[i].critrate = rng.Between(20, 80);
							enemy[i].monster_type = 0b00000000;
							enemy[i].elem_resist = 0b00000000;
							enemy[i].elem_weakness = 0b00000000;
							break;
						case 24: // Blah, it's a Vampire!
							enemyNames[i] = "VAMP";
							enemy[i].monster_type = 0b10001001;
							enemy[i].elem_resist = 0b10101011;
							enemy[i].elem_weakness = 0b00010000;
							break;
						case 25: // Large Elemental
							if (elementalsSelected.Count > 0)
								elemental = elementalsSelected.PickRandom(rng);
							ENE_rollLargeElemental(rng, ref enemy[i], elemental);
							switch (elemental)
							{
								case 1:
									enemyNames[i] = "EARTH";
									break;
								case 2:
									enemyNames[i] = "STORM";
									break;
								case 3:
									enemyNames[i] = "ICE";
									break;
								case 4:
									enemyNames[i] = "FIRE";
									break;
								case 5:
									enemyNames[i] = "DEATH";
									break;
								case 6:
									enemyNames[i] = "CRONO";
									break;
								case 7:
									enemyNames[i] = "VENOM";
									break;
								case 8:
									enemyNames[i] = "DJINN";
									break;
								default:
									enemyNames[i] = "FORCE";
									break;
							}
							if (elemental != 9)
								elementalsSelected.Remove(elemental);
							break;
						case 26: // Gargoyle
							enemyNames[i] = "GOYLE";
							enemy[i].monster_type = 0b00000001;
							enemy[i].elem_resist = 0b10000000;
							enemy[i].elem_weakness = 0b00000000;
							break;
						case 27: // Dragon Type 1
							if (dragonsSelected.Count > 0)
								elemental = dragonsSelected.PickRandom(rng);
							ENE_rollDragon(rng, ref enemy[i], elemental);
							switch (elemental)
							{
								case 1:
									enemyNames[i] = "Earth D";
									break;
								case 2:
									enemyNames[i] = "Elec D";
									break;
								case 3:
									enemyNames[i] = "Ice D";
									break;
								case 4:
									enemyNames[i] = "Fire D";
									break;
								case 5:
									enemyNames[i] = "Death D";
									break;
								case 6:
									enemyNames[i] = "Time D";
									break;
								case 7:
									enemyNames[i] = "Gas D";
									break;
								case 8:
									enemyNames[i] = "Magic D";
									break;
								default:
									enemyNames[i] = "DRAKE";
									break;
							}
							if (elemental != 9)
								dragonsSelected.Remove(elemental);
							break;
						case 28: // Slime
							enemyNames[i] = "FLAN";
							enemy[i].monster_type = 0b00000001;
							enemy[i].elem_weakness = (byte)(rng.Between(1, 6) << 4); // can be fire, ice, fire+ice, lit, fire+lit, or ice+lit weak
							enemy[i].elem_resist = (byte)((0b11111111 ^ enemy[i].elem_weakness) & 0b11111011); // resist all other elements except time
							if (rng.Between(0, 1) == 1)
								enemy[i].absorb = 255; // 50% chance of max absorb for this enemy type
							break;
						case 29: // Manticore
							enemyNames[i] = "MANT";
							enemy[i].monster_type = 0b00000000;
							enemy[i].elem_resist = 0b00000000;
							enemy[i].elem_weakness = 0b00000000;
							break;
						case 30: // Spider
							enemyNames[i] = "BUG";
							enemy[i].monster_type = 0b00000000;
							enemy[i].elem_resist = 0b00000000;
							enemy[i].elem_weakness = 0b00000000;
							break;
						case 31: // Ankylo
							enemyNames[i] = "ANK";
							enemy[i].monster_type = 0b00000000;
							enemy[i].elem_resist = 0b00000000;
							enemy[i].elem_weakness = 0b00000000;
							break;
						case 32: // Mummy
							enemyNames[i] = "MUMMY";
							enemy[i].monster_type = 0b00001000;
							enemy[i].elem_resist = 0b00101011;
							enemy[i].elem_weakness = 0b00010000;
							break;
						case 33: // Wyvern
							enemyNames[i] = "WYRM";
							enemy[i].monster_type = 0b00000010;
							enemy[i].elem_resist = 0b10000000;
							enemy[i].elem_weakness = 0b00000000;
							break;
						case 34: // Jerk Bird
							enemyNames[i] = "BIRD";
							enemy[i].monster_type = 0b00000000;
							enemy[i].elem_resist = 0b10000000;
							enemy[i].elem_weakness = 0b00000000;
							break;
						case 35: // Steak
							enemyNames[i] = "TYRO";
							enemy[i].critrate = rng.Between(20, 80);
							enemy[i].monster_type = 0b00000010;
							enemy[i].elem_resist = 0b00000000;
							enemy[i].elem_weakness = 0b00000000;
							break;
						case 36: // Pirahna
							enemyNames[i] = "FISH";
							enemy[i].monster_type = 0b00100000;
							enemy[i].elem_resist = 0b10010000;
							enemy[i].elem_weakness = 0b01000000;
							break;
						case 37: // Ocho
							enemyNames[i] = "OCHO";
							enemy[i].monster_type = 0b00100000;
							enemy[i].elem_resist = 0b10010000;
							enemy[i].elem_weakness = 0b01000000;
							break;
						case 38: // Gator
							enemyNames[i] = "GATOR";
							enemy[i].monster_type = 0b00100010;
							enemy[i].elem_resist = 0b10010000;
							enemy[i].elem_weakness = 0b01000000;
							break;
						case 39: // Hydra
							enemyNames[i] = "HYDRA";
							enemy[i].monster_type = 0b00000010;
							enemy[i].elem_resist = 0b00000000;
							enemy[i].elem_weakness = 0b00000000;
							break;
						case 40: // Robot
							enemyNames[i] = "BOT";
							enemy[i].monster_type = 0b00000000;
							enemy[i].elem_resist = 0b00001011;
							enemy[i].elem_weakness = 0b01000000;
							break;
						case 41: // Naga
							enemyNames[i] = "NAGA";
							enemy[i].monster_type = 0b01000000;
							enemy[i].elem_resist = 0b00000000;
							enemy[i].elem_weakness = 0b00000000;
							break;
						case 42: // Small Elemental
							enemyNames[i] = "AIR";
							if (elementalsSelected.Count > 0)
								elemental = elementalsSelected.PickRandom(rng);
							ENE_rollSmallElemental(rng, ref enemy[i], elemental);
							switch (elemental)
							{
								case 1:
									enemyNames[i] = "SHARD";
									break;
								case 2:
									enemyNames[i] = "WIND";
									break;
								case 3:
									enemyNames[i] = "WATER";
									break;
								case 4:
									enemyNames[i] = "FLARE";
									break;
								case 5:
									enemyNames[i] = "DOOM";
									break;
								case 6:
									enemyNames[i] = "TIME";
									break;
								case 7:
									enemyNames[i] = "BANE";
									break;
								case 8:
									enemyNames[i] = "MAGIC";
									break;
								default:
									enemyNames[i] = "AIR";
									break;
							}
							if (elemental != 9)
								elementalsSelected.Remove(elemental);
							break;
						case 43: // Chimera
							enemyNames[i] = "BEAST";
							enemy[i].monster_type = 0b00000010;
							enemy[i].elem_resist = 0b10010000;
							enemy[i].elem_weakness = 0b00100000;
							break;
						case 44: // Piscodemon
							enemyNames[i] = "SQUID";
							enemy[i].monster_type = 0b00000000;
							enemy[i].elem_resist = 0b00110011;
							enemy[i].elem_weakness = 0b00000000;
							break;
						case 45: // Dragon Type 2
							if (dragonsSelected.Count > 0)
								elemental = dragonsSelected.PickRandom(rng);
							ENE_rollDragon(rng, ref enemy[i], elemental);
							switch (elemental)
							{
								case 1:
									enemyNames[i] = "Earth D";
									break;
								case 2:
									enemyNames[i] = "Elec D";
									break;
								case 3:
									enemyNames[i] = "Ice D";
									break;
								case 4:
									enemyNames[i] = "Fire D";
									break;
								case 5:
									enemyNames[i] = "Death D";
									break;
								case 6:
									enemyNames[i] = "Time D";
									break;
								case 7:
									enemyNames[i] = "Gas D";
									break;
								case 8:
									enemyNames[i] = "Magic D";
									break;
								default:
									enemyNames[i] = "DRAGON";
									break;
							}
							if (elemental != 9)
								dragonsSelected.Remove(elemental);
							break;
						case 46: // Knight Type 1
							enemyNames[i] = "KNIGHT";
							enemy[i].monster_type = 0b00000000;
							enemy[i].elem_resist = 0b00000000;
							enemy[i].elem_weakness = 0b00000000;
							break;
						case 47: // Golem
							enemyNames[i] = "GOLEM";
							enemy[i].monster_type = 0b00000000;
							enemy[i].elem_resist = 0b01111011;
							if (enemy[i].absorb > (enemy[i].tier < 3 ? 30 : 60))
							{
								switch (rng.Between(0, 2))
								{
									case 0:
										enemy[i].elem_resist &= 0b10111111;
										break;
									case 1:
										enemy[i].elem_resist &= 0b11011111;
										break;
									case 2:
										enemy[i].elem_resist &= 0b11101111;
										break;
								}
							}
							enemy[i].elem_weakness = 0b00000000;
							break;
						case 48: // Knight Type 2
							enemyNames[i] = "RANGER";
							enemy[i].monster_type = 0b00000000;
							enemy[i].elem_resist = 0b00000000;
							enemy[i].elem_weakness = 0b00000000;
							break;
						case 49: // Pony
							enemyNames[i] = "PONY";
							enemy[i].monster_type = 0b00000000;
							enemy[i].elem_resist = 0b00000000;
							enemy[i].elem_weakness = 0b00000000;
							break;
						case 50: // Elf
							enemyNames[i] = "ELF";
							enemy[i].monster_type = 0b01000000;
							enemy[i].elem_resist = 0b00000000;
							enemy[i].elem_weakness = 0b00000000;
							break;
						case 51: // War Machine
							enemyNames[i] = "MECH";
							enemy[i].monster_type = 0b10000000;
							enemy[i].elem_resist = 0b00111011;
							enemy[i].elem_weakness = 0b01000000;
							break;
					}
					// generate perk eligibility
					perks.Add(MonsterPerks.PERK_LOWRESIST);
					perks.Add(MonsterPerks.PERK_HIGHRESIST);
					perks.Add(MonsterPerks.PERK_LOWWEAKNESS);
					perks.Add(MonsterPerks.PERK_HIGHWEAKNESS);
					perks.Add(MonsterPerks.PERK_PLUSONEHIT);
					perks.Add(MonsterPerks.PERK_POISONTOUCH);
					perks.Add(MonsterPerks.PERK_STUNSLEEPTOUCH);
					perks.Add(MonsterPerks.PERK_MUTETOUCH);
					// set attack element and ailments to default value
					enemy[i].atk_ailment = 0b00000000;
					enemy[i].atk_elem = 0b00000000;
					// apply global perks
					bool hasGlobalPerk = false;
					bool didntChooseVariantClass = true; // false if this monster rolled a variant class and thus doesn't need a name modifier
														 // if enemy has a global perk, apply it now and skip all other perks
					if (i == Enemy.Crawl)
					{
						// Global Perk - multi-hitter with weak attacks but stun touch guaranteed (overrides existing atk ailment)
						enemy[i].atk_elem = 0b00000001;
						enemy[i].atk_ailment = 0b00010000;
						enemy[i].num_hits = 8;
						enemy[i].damage = 1;
						hasGlobalPerk = true;
					}
					else if (i == Enemy.Coctrice)
					{
						// Global Perk - stonetouch (overrides other atk_elem and ailment), -1 hit if num_hits > 1
						enemy[i].atk_elem = 0b00000010;
						enemy[i].atk_ailment = 0b00000010;
						if (enemy[i].num_hits > 1)
							enemy[i].num_hits--;
						hasGlobalPerk = true;
					}
					else if (i == Enemy.Sorcerer)
					{
						// Global Perk - deathtouch
						enemy[i].atk_elem = 0b00000000;
						enemy[i].atk_ailment = 0b00000001;
						hasGlobalPerk = true;
					}
					else
					{
						// else, roll the chance for an enemy to get a class modifier.  Each class of monster can have Frost, Red, Zombie, Were, and Wizard variants, though some monster classes can't use some of these
						// this perk restricts the monster from rolling any other perks
						if (rng.Between(0, 11) < monsterClassVariants[newEnemyImageLUT[enemy[i].image]].Count())
						{
							didntChooseVariantClass = false;
							string classModifier = monsterClassVariants[newEnemyImageLUT[enemy[i].image]].PickRandom(rng);
							switch (classModifier) // apply the traits of this enemy class
							{
								case "Wz":
									if (enemy[i].AIscript == 0xFF && !shuffledSkillsOn)
									{
										enemy[i].AIscript = ENE_PickForcedAIScript(enemy[i].tier, rng);
									}
									break;
								case "Fr":
									enemy[i].elem_resist |= 0b00100000;
									enemy[i].elem_resist &= 0b11101111;
									enemy[i].elem_weakness |= 0b00010000;
									enemy[i].elem_weakness &= 0b11011111;
									enemy[i].atk_elem = 0b00100000;
									break;
								case "R.":
									enemy[i].elem_resist |= 0b00010000;
									enemy[i].elem_resist &= 0b11011111;
									enemy[i].elem_weakness |= 0b00100000;
									enemy[i].elem_weakness &= 0b11101111;
									enemy[i].atk_elem = 0b00010000;
									break;
								case "Z.":
									enemy[i].elem_resist |= 0b00101011;
									enemy[i].elem_resist &= 0b11101111;
									enemy[i].elem_weakness |= 0b00010000;
									enemy[i].elem_weakness &= 0b11010100;
									enemy[i].monster_type |= 0b00001000;
									if (enemy[i].atk_ailment == 0)
									{
										enemy[i].atk_ailment = 0b00010000;
										enemy[i].atk_elem = 0b00000001;
									}
									break;
								case "Sea":
									enemy[i].elem_resist |= 0b10010000;
									enemy[i].elem_resist &= 0b10111111;
									enemy[i].elem_weakness |= 0b01000000;
									enemy[i].elem_weakness &= 0b01101111;
									enemy[i].monster_type |= 0b00100000;
									break;
								case "Wr":
									enemy[i].atk_ailment = 0b00000100;
									enemy[i].atk_elem = 0b00000010;
									enemy[i].monster_type |= 0b10010000;
									break;
							}
							enemyNames[i] = classModifier + enemyNames[i]; // change the enemy's name
							monsterClassVariants[newEnemyImageLUT[enemy[i].image]].Remove(classModifier); // remove this variant from the list of variants available for this enemy image
						}
					}
					if (didntChooseVariantClass)
					{
						// else, roll minor perks.  there is a 75% chance to gain a perk, then 62.5%, 50%, and so on.  monsters cannot gain more than 4 perks
						int num_perks = 0;
						if (!hasGlobalPerk)
						{
							while (rng.Between(0, 7) > 1 + num_perks && num_perks < 4)
							{
								// select from the list of available perks
								MonsterPerks this_perk = perks.PickRandom(rng);
								bool didPerkRoll = true;
								switch (this_perk)
								{
									case MonsterPerks.PERK_GAINSTAT10: // pick a random stat to increase by 10%, +3% XP
										switch (rng.Between(0, 2))
										{
											case 0:
												enemy[i].hp *= 11;
												enemy[i].hp /= 10;
												break;
											case 1:
												enemy[i].damage *= 11;
												enemy[i].damage /= 10;
												break;
											case 2:
												enemy[i].absorb *= 11;
												enemy[i].absorb /= 10;
												break;
										}
										enemy[i].exp *= 103;
										enemy[i].exp /= 100;
										break;
									case MonsterPerks.PERK_LOSESTAT10: // pick a random stat to decrease by 10%, -3% XP
										switch (rng.Between(0, 2))
										{
											case 0:
												enemy[i].hp *= 9;
												enemy[i].hp /= 10;
												break;
											case 1:
												enemy[i].damage *= 9;
												enemy[i].damage /= 10;
												break;
											case 2:
												enemy[i].absorb *= 9;
												enemy[i].absorb /= 10;
												break;
										}
										enemy[i].exp *= 97;
										enemy[i].exp /= 100;
										break;
									case MonsterPerks.PERK_LOWRESIST: // pick a low resist or remove the corresponding weakness, +4% XP
										switch (rng.Between(0, 3))
										{
											case 0:
												if ((enemy[i].elem_weakness & 0b10000000) == 0b10000000)
													enemy[i].elem_weakness &= 0b01111111;
												else
													enemy[i].elem_resist |= 0b10000000;
												break;
											case 1:
												if ((enemy[i].elem_weakness & 0b01000000) == 0b01000000)
													enemy[i].elem_weakness &= 0b10111111;
												else
													enemy[i].elem_resist |= 0b01000000;
												break;
											case 2:
												if ((enemy[i].elem_weakness & 0b00100000) == 0b00100000)
													enemy[i].elem_weakness &= 0b11011111;
												else
													enemy[i].elem_resist |= 0b00100000;
												break;
											case 3:
												if ((enemy[i].elem_weakness & 0b00010000) == 0b00001000)
													enemy[i].elem_weakness &= 0b11101111;
												else
													enemy[i].elem_resist |= 0b00010000;
												break;
										}
										enemy[i].exp *= 104;
										enemy[i].exp /= 100;
										break;
									case MonsterPerks.PERK_LOWWEAKNESS: // pick a low weakness (or cancel a resist, or ignore for earth resist), -5% XP
										switch (rng.Between(0, 3))
										{
											case 0:
												if ((enemy[i].elem_resist & 0b10000000) == 0b10000000)
													didPerkRoll = false;
												else
													enemy[i].elem_weakness |= 0b10000000;
												break;
											case 1:
												if ((enemy[i].elem_resist & 0b01000000) == 0b01000000)
													enemy[i].elem_resist &= 0b10111111;
												else
													enemy[i].elem_weakness |= 0b01000000;
												break;
											case 2:
												if ((enemy[i].elem_resist & 0b00100000) == 0b00100000)
													enemy[i].elem_resist &= 0b11011111;
												else
													enemy[i].elem_weakness |= 0b00100000;
												break;
											case 3:
												if ((enemy[i].elem_resist & 0b00010000) == 0b00010000)
													enemy[i].elem_resist &= 0b11101111;
												else
													enemy[i].elem_weakness |= 0b00010000;
												break;
										}
										if (didPerkRoll)
										{
											enemy[i].exp *= 95;
											enemy[i].exp /= 100;
										}
										break;
									case MonsterPerks.PERK_HIGHRESIST:  // pick a high resist and remove the corresponding weakness, +3% XP
										switch (rng.Between(0, 3))
										{
											case 0:
												enemy[i].elem_resist |= 0b00001000;
												enemy[i].elem_weakness &= 0b11110111;
												break;
											case 1:
												enemy[i].elem_resist |= 0b00000100;
												enemy[i].elem_weakness &= 0b11111011;
												break;
											case 2:
												enemy[i].elem_resist |= 0b00000010;
												enemy[i].elem_weakness &= 0b11111101;
												break;
											case 3:
												enemy[i].elem_resist |= 0b00000001;
												enemy[i].elem_weakness &= 0b11111110;
												break;
										}
										enemy[i].exp *= 103;
										enemy[i].exp /= 100;
										break;
									case MonsterPerks.PERK_HIGHWEAKNESS:
										switch (rng.Between(0, 3))
										{
											case 0:
												if ((enemy[i].elem_resist & 0b00001000) == 0b00001000)
													enemy[i].elem_resist &= 0b11110111;
												else
													enemy[i].elem_weakness |= 0b00001000;
												break;
											case 1:
												if ((enemy[i].elem_resist & 0b00000100) == 0b00000100)
													enemy[i].elem_resist &= 0b11111011;
												else
													enemy[i].elem_weakness |= 0b00000100;
												break;
											case 2:
												if ((enemy[i].elem_resist & 0b00000010) == 0b00000010)
													enemy[i].elem_resist &= 0b11111101;
												else
													enemy[i].elem_weakness |= 0b00000010;
												break;
											case 3:
												if ((enemy[i].elem_resist & 0b00000001) == 0b00000001)
													enemy[i].elem_resist &= 0b11111110;
												else
													enemy[i].elem_weakness |= 0b00000001;
												break;
										}
										if (didPerkRoll)
										{
											enemy[i].exp *= 97;
											enemy[i].exp /= 100;
										}
										break;
									case MonsterPerks.PERK_PLUSONEHIT: // +1 hit, +5/3/2/1... XP for an addition hit
										enemy[i].num_hits++;
										switch (enemy[i].num_hits)
										{
											case 2:
												enemy[i].exp *= 21;
												enemy[i].exp /= 20;
												break;
											case 3:
												enemy[i].exp *= 103;
												enemy[i].exp /= 100;
												break;
											case 4:
												enemy[i].exp *= 51;
												enemy[i].exp /= 50;
												break;
											default:
												enemy[i].exp *= 101;
												enemy[i].exp /= 100;
												break;
										}
										break;
									case MonsterPerks.PERK_POISONTOUCH: // adds poisontouch if enemy has no atk_ailment already, no XP increase
										if (enemy[i].atk_ailment == 0)
										{
											enemy[i].atk_ailment = 0b00000100;
											enemy[i].atk_elem = 0b00000010;
										}
										break;
									case MonsterPerks.PERK_STUNSLEEPTOUCH: // adds stun or sleep touch if enemy has no atk_ailment already
										if (enemy[i].atk_ailment == 0)
										{
											enemy[i].atk_ailment = rng.Between(0, 1) == 0 ? (byte)0b00100000 : (byte)0b00010000;
											enemy[i].atk_elem = 0b00000001;
											enemy[i].exp *= 103;
											enemy[i].exp /= 100;
										}
										break;
									case MonsterPerks.PERK_MUTETOUCH: // adds mute touch if enemy has no atk_ailment already
										if (enemy[i].atk_ailment == 0)
										{
											enemy[i].atk_ailment = 0b01000000;
											enemy[i].atk_elem = 0b00000001;
											enemy[i].exp *= 51;
											enemy[i].exp /= 50;
										}
										break;
								}
								num_perks++;
							}
						}
						// if enemy's vanilla name has already been used, add a name modifier
						if (elemental == 9) // don't make alternate names for elementals
						{
							if (monsterBaseNameUsed[newEnemyImageLUT[enemy[i].image]])
							{
								string nameModifier = monsterNameVariants[newEnemyImageLUT[enemy[i].image]].PickRandom(rng);
								enemyNames[i] = nameModifier + enemyNames[i];
								monsterNameVariants[newEnemyImageLUT[enemy[i].image]].Remove(nameModifier);
							}
							else
								monsterBaseNameUsed[newEnemyImageLUT[enemy[i].image]] = true;
						}
					}
					if (enemy[i].AIscript != 0xFF) // set monster type to Mage if it has a script
						enemy[i].monster_type |= 0b01000000;
					for (int j = 1; j < enemy[i].num_hits; ++j) // for each hit past the first, reduce the base damage by one for every 15 points of damage rating
					{
						enemy[i].damage = enemy[i].damage - enemy[i].damage / 15;
					}
					if ((enemy[i].monster_type & 0b00001000) != 0)
					{
						enemy[i].elem_resist |= 0b00001000; // force death resist on undead enemies
						enemy[i].elem_weakness &= 0b11110111; // and remove weakness to death
					}
					enemy[i].damage = rng.Between(enemy[i].damage - enemy[i].damage / 25, enemy[i].damage + enemy[i].damage / 25); // variance for damage rating
					enemy[i].hp = rng.Between(enemy[i].hp - enemy[i].hp / 30, enemy[i].hp + enemy[i].hp / 30); // variance for hp rating
					enemy[i].gp = rng.Between(enemy[i].gp - enemy[i].gp / 20, enemy[i].gp + enemy[i].gp / 20); // variance for gp reward
					enemy[i].exp = rng.Between(enemy[i].exp - enemy[i].exp / 40, enemy[i].exp + enemy[i].exp / 40); // variance for exp reward
					if(enemy[i].AIscript != 0xFF)
					{
						// determine skill tier
						int highestTier = 0;
						foreach(byte id in script[enemy[i].AIscript].skill_list)
						{
							if (id == 0xFF)
								continue;
							if (skill[id].tier > highestTier)
								highestTier = skill[id].tier;
						}
						foreach(byte id in script[enemy[i].AIscript].spell_list)
						{
							if (id == 0xFF)
								continue;
							if (spell[id].tier > highestTier)
								highestTier = spell[id].tier;
						}
						enemy[i].skilltier = highestTier;
					}
				}
			}
			for (int i = 0; i < GenericTilesetsCount; ++i) // remove palettes from tilesets where there are no mons using those palettes
			{
				List<byte> palRemoveList = new List<byte> { };
				foreach (byte pal in en.palettesInTileset[i])
				{
					bool nopalettematch = true;
					foreach (byte mon in en.enemiesInTileset[i])
					{
						if (enemy[mon].pal == pal)
						{
							nopalettematch = false;
							break;
						}
					}
					if (nopalettematch)
					{
						palRemoveList.Add(pal);
					}
				}
				foreach (byte pal in palRemoveList)
				{
					en.palettesInTileset[i].Remove(pal);
				}
			}
			// modify scripts
			// each spell is then selected from a list of spells available for that tier.  it is not possible to promote to a higher tier
			// skills will remain the same
			for (int i = 0; i < script.Length - 10; ++i) // exclude the last 10 scripts
			{
				// start replacing each spell with another spell from the same tier
				for (byte j = 0; j < 8; ++j)
				{
					if (script[i].spell_list[j] == 0xFF)
						continue; // skip blank spells
					int whichTier = spell[script[i].spell_list[j]].tier;
					if (whichTier == 0)
						whichTier = 1; // tier 0 becomes tier 1
					List<byte> eligibleSpellIDs = new List<byte> { };
					for (byte k = 0; k < 64; ++k)
					{
						if (spell[k].tier == whichTier)
							eligibleSpellIDs.Add(k);
					}
					script[i].spell_list[j] = eligibleSpellIDs.PickRandom(rng);
				}
			}
			return true;
		}

		public void DoEnemizer(EnemyScripts enemyScripts, ZoneFormations zones, Flags flags, MT19337 rng)
		{
			if (!(bool)flags.RandomizeFormationEnemizer)
			{
				return;
			}

			bool doEnemies = (bool)flags.RandomizeEnemizer;
			bool doFormations = (bool)flags.RandomizeFormationEnemizer;
			bool shuffledSkillsOn = flags.EnemizerDontMakeNewScripts;

			// code modification to allow any formation except 0x00 to be a trap (lifted from ShuffleTrapTiles)
			//Data[0x7CDC5] = 0xD0; // changes the game's programming
			//bool IsBattleTile(Blob tuple) => tuple[0] == 0x0A;
			//bool IsRandomBattleTile(Blob tuple) => IsBattleTile(tuple) && (tuple[1] & 0x80) != 0x00;
			//var tilesets = Get(TilesetDataOffset, TilesetDataCount * TilesetDataSize * TilesetCount).Chunk(TilesetDataSize).ToList();
			//tilesets.ForEach(tile => { if (IsRandomBattleTile(tile)) tile[1] = 0x00; });
			//Put(TilesetDataOffset, tilesets.SelectMany(tileset => tileset.ToBytes()).ToArray());// set all random battle tiles to zero

			SpellInfo[] spell = LoadSpells(); // list of spells and their appropriate tiers
			EnemySkillInfo[] skill = new EnemySkillInfo[EnemySkillCount]; // list of enemy skills and their appropriate tiers
			EnemyScripts script = enemyScripts; // list of enemy scripts

			// load vanilla values from ROM into the enemizer
			byte[] skilltiers_enemy = new byte[]
			{
				3, 2, 3, 1, 2, 1, 4, 3, 3, 4, 4, 4, 5, 3, 4, 3, 4, 4, 4, 1, 5, 2, 2, 1, 5, 5
			};
			for(int i = 0; i < EnemySkillCount; ++i)
			{
				skill[i] = new EnemySkillInfo();
				skill[i].decompressData(Get(EnemySkillOffset + i * EnemySkillSize, EnemySkillSize));
				skill[i].tier = skilltiers_enemy[i];
			}
			EnemyInfo[] enemy = new EnemyInfo[EnemyCount]; // list of enemies, including information that is either inferred from formation inspection or tier lists that I have just made up
			EnemizerTrackingInfo en = new EnemizerTrackingInfo(); // structure that contains many lists and other information that is helpful for managing formation generation efficiently
			// set enemy default tier list.  these listings are based on a combination of where the enemy is placed in the game, its xp yield, its rough difficulty, whether it has a script or not, and gut feels
			// -1 indicates a monster with special rules for enemy generation (usually a boss_
			int[] enemyTierList = new int[] {	 -1, 0, 0, 1, 1, 3, 1, 6, 5, 4, 6, 6, 0, 1, 4, -1,
												  1, 2, 5, 0, 7, 0, 3, 1, 2, 2, 4, 2, 2, 5, 1, 2,
												  5, 2, 5, 3, 4, 4, 5, 1, 2, 3, 5, 0, 1, 1, 2, 8,
												  5, 5, 7, -1, 4, 5, 4, 4, 4, 5, 3, 4, 5, 7, 1, 3,
												  4, 5, 5, 6, 6, 1, 2, 2, 7, 0, 1, 5, 4, 6, 7, 3,
												  5, 2, 3, 5, 5, 7, 9, 2, 4, 4, 5, 4, 7, 4, 5, 6,
												  8, 6, 6, 6, 7, 6, 8, 2, 4, -1, 8, 8, 5, 6, 9, 6,
												  7, -1, 4, 7, 1, 5, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1};
			for (int i = 0; i < EnemyCount; ++i)
			{
				enemy[i] = new EnemyInfo();
				enemy[i].decompressData(Get(EnemyOffset + i * EnemySize, EnemySize));
				enemy[i].tier = enemyTierList[i];
			}
			string[] enemyNames = EnemyText.Get();
			string[] skillNames = ReadText(EnemySkillTextPointerOffset, EnemySkillTextPointerBase, EnemySkillCount); // load all the names of enemy skills
			for (int i = 0; i < FormationCount; ++i) // we need to scour the formations list for enemy information, and to give the enemizer tracking info construct information it can work with
			{
				FormationInfo f = new FormationInfo();
				f.decompressData(Get(FormationDataOffset + i * FormationSize, FormationSize));
				en.ReadEnemyDataFromFormation(f, enemy);
			}
			en.PurgeIDFromEnemyTilesetList(Enemy.Imp);
			en.PurgeIDFromEnemyTilesetList(Enemy.Pirate);
			en.PurgeIDFromEnemyTilesetList(Enemy.Phantom);
			en.PurgeIDFromEnemyTilesetList(Enemy.Astos);
			en.PurgeIDFromEnemyTilesetList(Enemy.Garland);
			en.PurgeIDFromEnemyTilesetList(Enemy.WarMech); // purging enemies from the generic enemy lists that we don't want to appear outside of set battles
			if (doEnemies)
			{
				byte[] patterntabledata = Get(EnemyPatternTablesOffset, 0x6800); // each pattern table is 0x800 bytes and there are 13 pattern tables that we will edit
				// do enemizer stuff
				if(DoEnemizer_Enemies(rng, enemy, patterntabledata, spell, skill, script.GetList().ToArray(), enemyNames, skillNames, shuffledSkillsOn, en))
				{
					doFormations = true; // must use formation generator with enemizer
					for (int i = 0; i < EnemyCount; ++i)
					{
						Put(EnemyOffset + EnemySize * i, enemy[i].compressData()); // move every entry from the enemizer to the ROM
					}
					Put(EnemyPatternTablesOffset, patterntabledata); // write the new pattern tables as a chunk

					EnemyText.Set(enemyNames);
					WriteText(skillNames, EnemySkillTextPointerOffset, EnemySkillTextPointerBase, EnemySkillTextOffset);
				}
				else
				{
					Console.WriteLine("Fission Mailed - Abort Enemy Shuffle");
					throw new InsaneException("Something went wrong with Enemy Generation (Enemizer)!");
				}
			}
			if(doFormations)
			{
				if (!doEnemies)
				{
					byte[] patterntabledata = Get(EnemyPatternTablesOffset, 0x6800); // each pattern table is 0x800 bytes and there are 13 pattern tables that we will edit
					DoEnemizer_EnemyPatternTablesOnly(rng, patterntabledata, enemy, en); // rewrite the pattern tables and enemy palette assignments
					Put(EnemyPatternTablesOffset, patterntabledata); // write the new pattern tables as a chunk
				}

				if(!DoEnemizer_Formations(rng, zones, enemy, en))
				{
					Console.WriteLine("Fission Mailed - Abort Formation Shuffle");
					throw new InsaneException("Something went wrong with Formation Generation (Enemizer)!");
				}
			}
		}

		public void GenerateBalancedEnemyScripts(EnemyScripts enemyScripts, MT19337 rng, bool swolePirates)
		{
			SpellInfo[] spell = new SpellInfo[MagicCount]; // list of spells and their appropriate tiers
			EnemySkillInfo[] skill = new EnemySkillInfo[EnemySkillCount]; // list of enemy skills and their appropriate tiers
			EnemyScripts script = enemyScripts; // list of enemy scripts

			// load values from the ROM (this can be normal spells or Spellcrafter spells)
			byte[] skilltiers_enemy = new byte[]
			{
				2, 1, 2, 1, 1, 1, 3, 2, 1, 3, 3, 3, 4, 2, 4, 3, 3, 3, 3, 1, 5, 1, 2, 2, 4, 4
			};
			for (int i = 0; i < MagicCount; ++i)
			{
				spell[i] = new SpellInfo();
				spell[i].decompressData(Get(MagicOffset + i * MagicSize, MagicSize));
				spell[i].calc_Enemy_SpellTier();
			}
			for (int i = 0; i < EnemySkillCount; ++i)
			{
				skill[i] = new EnemySkillInfo();
				skill[i].decompressData(Get(EnemySkillOffset + i * EnemySkillSize, EnemySkillSize));
				skill[i].tier = skilltiers_enemy[i];
			}
			/*
			for (int i = 0; i < ScriptCount; ++i)
			{
				script[i] = new EnemyScriptInfo();
				script[i].decompressData(Get(ScriptOffset + i * ScriptSize, ScriptSize));
			}*/

			EnemyInfo[] enemy = new EnemyInfo[EnemyCount]; // list of enemies, including information that is either inferred from formation inspection or tier lists that I have just made up
			// set enemy default tier list.  these tier rankings are different from the enemizer basis, but show the kind of skills/spells that are prioritized
			// when a script lands on that monster.  the final 10 enemies are warmech, the fiends, and chaos.
			int[] enemyTierList = new int[] {     -1, 1, 1, 1, 1, 2, 1, 3, 3, 2, 3, 3, 1, 1, 3, 1,
												  1, 1, 3, 1, 4, 1, 1, 1, 1, 1, 2, 1, 1, 3, 1, 1,
												  3, 1, 2, 2, 2, 2, 3, 1, 1, 2, 3, 1, 1, 1, 1, 4,
												  3, 2, 3, 5, 3, 3, 2, 3, 2, 3, 2, 3, 3, 4, 1, 3,
												  2, 3, 4, 4, 4, 1, 1, 1, 3, 1, 1, 3, 2, 3, 3, 1,
												  3, 2, 2, 3, 3, 4, 5, 1, 2, 2, 3, 2, 4, 2, 2, 3,
												  4, 3, 3, 3, 4, 3, 4, 1, 3, 1, 4, 4, 3, 3, 5, 3,
												  4, 3, 3, 4, 1, 3, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1};
			if (swolePirates)
				enemyTierList[Enemy.Pirate] = 3;
			for (int i = 0; i < EnemyCount; ++i)
			{
				enemy[i] = new EnemyInfo();
				enemy[i].decompressData(Get(EnemyOffset + i * EnemySize, EnemySize));
				enemy[i].tier = enemyTierList[i];
			}

			bool[] scriptRepeat = new bool[script.Count()];
			int[] scriptLowestTier = new int[script.Count()];
			for(int i = 0; i < script.Count(); ++i)
			{
				scriptRepeat[i] = false;
				scriptLowestTier[i] = 10;
			}
			for(int i = 1; i < EnemyCount - 10; ++i)
			{
				if (enemy[i].AIscript == 0xFF)
					continue; // skip any enemy without a script
				if (scriptRepeat[enemy[i].AIscript] && enemy[i].tier >= scriptLowestTier[enemy[i].AIscript])
					continue; // skip medusa script if this enemy is stronger than another monster with the medusa script
				scriptRepeat[enemy[i].AIscript] = true;
				scriptLowestTier[enemy[i].AIscript] = enemy[i].tier;
				int[] tierchance = new int[5];
				int[] skilltierchance = new int[4];
				tierchance[0] = 0; tierchance[1] = 0; tierchance[2] = 0; tierchance[3] = 0; tierchance[4] = 0;
				skilltierchance[0] = 0; skilltierchance[1] = 0; skilltierchance[2] = 0; skilltierchance[3] = 0;
				switch (enemy[i].tier)
				{
					case 0:
						break;
					case 1:
						// enemy will only have weak spells and skills
						skilltierchance[0] = 10;
						tierchance[0] = 6;
						tierchance[1] = 4;
						break;
					case 2:
						// enemy may have medium tier skills but mostly weak spells with small chance of mid-tier spells
						skilltierchance[0] = 2;
						skilltierchance[1] = 8;
						tierchance[0] = 2;
						tierchance[1] = 6;
						tierchance[2] = 2;
						break;
					case 3:
						// enemy tends towards mid-tier spells and skills with a very small chance of higher tier spells
						skilltierchance[0] = 1;
						skilltierchance[1] = 9;
						tierchance[0] = 1;
						tierchance[1] = 1;
						tierchance[2] = 6;
						tierchance[3] = 2;
						break;
					case 4:
						// enemy uses exclusively high tier skills and tends towards high-tier spells
						skilltierchance[2] = 10;
						tierchance[1] = 1;
						tierchance[2] = 3;
						tierchance[3] = 6;
						break;
					case 5:
						// enemy uses exclusively high tier skills with a chance of using god-tier skills (SWIRL/TORNADO) and tends towards high tier spells with a small chance of god-tier spells (NUKE/FADE)
						skilltierchance[2] = 6;
						skilltierchance[3] = 4;
						tierchance[2] = 1;
						tierchance[3] = 7;
						tierchance[4] = 2;
						break;
				}
				// cycle through skills, replacing each skill with a tier appropriate skill
				for(byte j = 0; j < 4; ++j)
				{
					if (script[enemy[i].AIscript].skill_list[j] == 0xFF)
						continue; // skip blank skills
					int diceRoll = rng.Between(0, 9);
					List<byte> eligibleSkillIDs = new List<byte> { };
					int sumRoll = 0;
					for(int k = 0; k < 4; ++k)
					{
						sumRoll += skilltierchance[k];
						if(diceRoll < sumRoll)
						{
							for(byte l = 0; l < EnemySkillCount; ++l)
							{
								if (skill[l].tier == k + 1)
									eligibleSkillIDs.Add(l);
							}
							break;
						}
					}
					if (eligibleSkillIDs.Count == 0)
						script[enemy[i].AIscript].skill_list[j] = 0xFF;
					else
						script[enemy[i].AIscript].skill_list[j] = eligibleSkillIDs.PickRandom(rng);
				}
				// cycle through spells, replacing each spell with a tier appropriate skill (first slot will always be the dominant spell tier for this enemy tier)
				if (script[enemy[i].AIscript].spell_list[0] != 0xFF)
				{
					List<byte> eligibleSpellIDs = new List<byte> { };
					int bestTier = 1;
					int bestValue = -1;
					for(int j = 0; j < 5; ++j)
					{
						if(tierchance[j] > bestValue)
						{
							bestValue = tierchance[j];
							bestTier = j + 1;
						}
					}
					for(byte j = 0; j < MagicCount; ++j)
					{
						if (spell[j].tier == bestTier)
							eligibleSpellIDs.Add(j);
					}
					if (eligibleSpellIDs.Count == 0)
						script[enemy[i].AIscript].spell_list[0] = 0xFF;
					else
						script[enemy[i].AIscript].spell_list[0] = eligibleSpellIDs.PickRandom(rng);
				}
				for (byte j = 1; j < 8; ++j)
				{
					if (script[enemy[i].AIscript].spell_list[j] == 0xFF)
						continue; // skip blank skills
					int diceRoll = rng.Between(0, 9);
					List<byte> eligibleSpellIDs = new List<byte> { };
					int sumRoll = 0;
					for (int k = 0; k < 5; ++k)
					{
						sumRoll += tierchance[k];
						if (diceRoll < sumRoll)
						{
							for (byte l = 0; l < MagicCount; ++l)
							{
								if (spell[l].tier == k + 1)
									eligibleSpellIDs.Add(l);
							}
							break;
						}
					}
					if (eligibleSpellIDs.Count == 0)
						script[enemy[i].AIscript].spell_list[j] = 0xFF;
					else
						script[enemy[i].AIscript].spell_list[j] = eligibleSpellIDs.PickRandom(rng);
				}
			}
		}

		public SpellInfo[] LoadSpells(int count = MagicCount)
		{
			var spell = new SpellInfo[count];
			for (int i = 0; i < count; ++i)
			{
				spell[i] = new SpellInfo();
				spell[i].decompressData(Get(MagicOffset + i * MagicSize, MagicSize));
				spell[i].calc_Enemy_SpellTier();
			}

			return spell;
		}

		public void ObfuscateEnemies(MT19337 rng, Flags flags)
		{
			var flagsValue = EnemyObfuscation.None;

			if (flagsValue == EnemyObfuscation.Imp || flagsValue == EnemyObfuscation.ImpAll)
			{
				List<FormationInfo> formations = LoadFormations();
				var enemyNames = EnemyText;

				List<string> alphabet = new List<string> { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z" };

				var names = alphabet.Join(alphabet, x => true, x => true, (a, b) => a + b.ToLower() + "IMP").ToList();

				List<(string name, byte pal)> variants = new List<(string, byte)>();
				for (int i = 0; i < 256; i++)
				{
					var name = names.SpliceRandom(rng);
					variants.Add((name, (byte)rng.Between(0, 128)));
				}

				int limit = flagsValue == EnemyObfuscation.ImpAll ? 128 : 119;

				for (int i = 0; i < limit; i++) enemyNames[i] = variants[i].name;

				for (int i = 0; i < FormationCount; ++i)
				{
					if (formations[i].id.Any(id => id >= limit)) continue;

					formations[i].pics = 0;
					formations[i].shape = 0;
					formations[i].tileset = 0;
					formations[i].pal1 = variants[formations[i].id[0]].pal;
					formations[i].pal1 = variants[formations[i].id[1]].pal;

					if(flagsValue == EnemyObfuscation.ImpAll && (flags.TrappedChaos ?? false) && formations[i].id.Any(id => id == 127))
					{
						formations[i].unrunnable_a = false;
						formations[i].unrunnable_b = false;
					}
				}

				StoreFormations(formations);
			}
		}


		private void StoreFormations(List<FormationInfo> formations)
		{
			for (int i = 0; i < FormationCount; ++i)
			{
				Put(FormationDataOffset + i * FormationSize, formations[i].compressData());
			}
		}

		private List<FormationInfo> LoadFormations()
		{
			List<FormationInfo> formations = new List<FormationInfo>();

			for (int i = 0; i < FormationCount; ++i)
			{
				FormationInfo f = new FormationInfo();
				f.decompressData(Get(FormationDataOffset + i * FormationSize, FormationSize));
				formations.Add(f);
			}

			return formations;
		}
	}


	public enum EnemyObfuscation
	{
		[Description("None")]
		None,

		[Description("Imp")]
		Imp,

		[Description("Imp (inc. Fiends and Chaos)")]
		ImpAll
	}
}
