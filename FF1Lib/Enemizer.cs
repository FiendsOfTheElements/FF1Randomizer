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
		public const int GenericTilesetsCount = 13;
		public const int EnemySkillCount = 26;
		public const int EnemySkillSize = 5;
		public const int EnemySkillOffset = 0x303F0;

		public class Enemizer
		{
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
			}

			public class SpellData
			{
				public byte accuracy;
				public byte effect;
				public byte elem;
				public byte targeting;
				public byte routine;
				public byte gfx;
				public byte palette;

				public byte[] CompressData()
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
			}

			public class EnemySkillData
			{
				public byte accuracy;
				public byte effect;
				public byte elem;
				public byte targeting;
				public byte routine;
			}

			public class EnemyData
			{
				public int exp;
				public int gp;
				public int hp;
				public int morale;
				public byte AIscript;
				public int agility;
				public int absorb;
				public int num_hits;
				public int accuracy;
				public int damage;
				public int critrate;
				public byte atk_elem;
				public byte atk_ailment;
				public byte monster_type;
				public int mdef;
				public byte elem_weakness;
				public byte elem_resist;

				public byte[] CompressData() // compresses the information of the enemy into an array of bytes to be placed in the game code
				{
					byte[] enemyInfo = new byte[20];
					if (exp < 0)
						exp = 0; // make sure experience isn't a negative number
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
			}

			public class EnemyScript
			{
				public byte spell_chance;
				public byte skill_chance;
				public byte[] spell_list = new byte[8];
				public byte[] skill_list = new byte[4];

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
			}

			public class Formation
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
				public bool unrunnable;

				public byte Top
				{
					get => id[0];
					set => id[0] = value;
				}

				public byte[] CompressData() // compresses the information in the formation into an array of bytes to be placed in the game code
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
					if (unrunnable)
						formationData[13] |= 0x01;
					formationData[14] = (byte)((monMin[4] << 4) | monMax[4]);
					formationData[15] = (byte)((monMin[5] << 4) | monMax[5]);
					return formationData;
				}
			}

			public class FormationMarker
			{
				public List<byte> mons = new List<byte> { };
				public List<int> moncount = new List<int> { };
			}

			public class EnemyPicPlusCount
			{
				char count;
				byte picID;
				EnemyPicPlusCount(char num, byte pict)
				{
					count = num;
					picID = pict;
				}
			}

			public byte[] spellDataFile = new byte[MagicSize * MagicCount];
			public SpellData[] spells = new SpellData[MagicCount];
			public byte[] spelltiers_human = new byte[]
			{
				0, 0, 0, 0, 0, 0, 0, 0,
				0, 0, 0, 0, 0, 0, 0, 0,
				0, 0, 0, 0, 0, 0, 0, 0,
				0, 0, 0, 0, 0, 0, 0, 0,
				0, 0, 0, 0, 0, 0, 0, 0,
				0, 0, 0, 0, 0, 0, 0, 0,
				0, 0, 0, 0, 0, 0, 0, 0,
				0, 0, 0, 0, 0, 0, 0, 0
			};
			public byte[] spelltiers_enemy = new byte[]
			{
				1, 0, 1, 3, 1, 2, 1, 1,
				0, 2, 2, 1, 2, 1, 2, 2,
				2, 0, 2, 2, 3, 3, 3, 2,
				0, 0, 2, 1, 2, 3, 0, 3,
				3, 0, 0, 3, 4, 4, 0, 3,
				0, 0, 3, 3, 4, 3, 4, 3,
				4, 0, 3, 4, 4, 4, 2, 1,
				0, 5, 4, 4, 5, 4, 4, 4
			};
			public byte[] skillDataFile = new byte[EnemySkillCount * EnemySkillSize];
			public EnemySkillData[] skills = new EnemySkillData[EnemySkillCount];
			public byte[] skilltiers_human = new byte[]
			{
				0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0
			};
			public byte[] skilltiers_enemy = new byte[]
			{
				3, 2, 3, 1, 2, 1, 4, 3, 3, 4, 4, 4, 5, 3, 4, 3, 4, 4, 4, 1, 5, 2, 2, 1, 5, 5
			};
			public byte[] enemyDataFile = new byte[EnemySize * EnemyCount];
			public EnemyData[] enemyStats = new EnemyData[EnemyCount];
			public string[] enemyNames = new string[EnemyCount];
			public int numEnemyNameCharacters = 0;
			public byte[] enemyScriptDataFile = new byte[ScriptSize * ScriptCount];
			public EnemyScript[] enemyScripts = new EnemyScript[ScriptCount];
			public byte[] formationData = new byte[FormationDataSize * FormationDataCount];
			public Formation[] formationInfo = new Formation[FormationDataCount];
			public List<byte>[] enemiesInTileset = new List<byte>[GenericTilesetsCount];
			public List<byte>[] palettesInTileset = new List<byte>[GenericTilesetsCount];
			public List<byte> enemyTilesets;
			public List<byte> enemyPalettes;
			public List<bool> enemySmallOrBig;
			public List<int> enemyPics;
			public List<int> enemyImage; // this combines tileset and pic
			public List<byte> uniqueEnemyIDs = new List<byte> { 0x00, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x0A, 0x0B, 0x0C, 0x0D, 0x0E, 0x10, 0x11, 0x12, 0x13, 0x14, 0x15, 0x16, 0x17, 0x18, 0x19,
														0x1A, 0x1B, 0x1C, 0x1D, 0x1E, 0x1F, 0x20, 0x21, 0x22, 0x23, 0x24, 0x25, 0x26, 0x27, 0x28, 0x29, 0x2A, 0x2B, 0x2C, 0x2D, 0x2E, 0x2F, 0x30, 0x31, 0x32, 0x34,
														0x35, 0x36, 0x37, 0x38, 0x39, 0x3A, 0x3B, 0x3C, 0x3D, 0x3E, 0x3F, 0x40, 0x41, 0x42, 0x43, 0x44, 0x45, 0x46, 0x47, 0x48, 0x49, 0x4A, 0x4B, 0x4C, 0x4D,
														0x4E, 0x4F, 0x50, 0x51, 0x52, 0x53, 0x54, 0x55, 0x56, 0x57, 0x58, 0x59, 0x5A, 0x5B, 0x5C, 0x5D, 0x5E, 0x5F, 0x60, 0x61, 0x62, 0x63, 0x64, 0x65, 0x66, 0x67,
														0x68, 0x6A, 0x6B, 0x6C, 0x6D, 0x6E, 0x6F, 0x70, 0x72, 0x73, 0x74, 0x75 };

			public byte[] traptile_formations = new byte[(int)TrapTiles.NUM_TRAP_TILES]; // formation pointers for each trap tile
			public int[] traptile_addresses = new int[(int)TrapTiles.NUM_TRAP_TILES]; // ROM addresses of trap tiles

			public List<int>[] enemyZones = new List<int>[EnemyCount];

			public bool[] featured = new bool[EnemyCount];
			public int[] zonecountmin; // counter to tell the enemizer how many enemies we want to roll for each zone
			public int[] zonecountmax;
			public List<byte>[] zone; // array of lists which contain the formation indices we have assigned to each zone
			public int[,] zonexpreqs;
			public List<byte>[] zonemons; // array of which monsters are compatible with which zones
			public List<int>[] zoneaddr; // list of addresses to place zone data
			public int[][] zoneclone; // list of addresses which are clones of other domain addresses - the first entry in the array is the base to read from, the other entries will copy that

			public byte[] domainData = new byte[ZoneFormationsSize * ZoneCount];
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
			public byte garland_encounter = 0x7F;
			public Enemizer()
			{
				enemyTilesets = Enumerable.Repeat((byte)0x00, EnemyCount).ToList();
				enemyPalettes = Enumerable.Repeat((byte)0x00, EnemyCount).ToList();
				enemyPics = Enumerable.Repeat(0x00, EnemyCount).ToList();
				enemySmallOrBig = Enumerable.Repeat(false, EnemyCount).ToList();
				enemyImage = Enumerable.Repeat(0x00, EnemyCount).ToList();
				for (int i = 0; i < GenericTilesetsCount; ++i)
				{
					enemiesInTileset[i] = new List<byte> { };
					palettesInTileset[i] = new List<byte> { };
				}
				zonecountmin = new int[] { 8, 36, 16, 10, 14, 12, 12, 22, 14, 10, 14 };
				zonecountmax = new int[] { 12, 48, 24, 15, 21, 18, 18, 33, 21, 15, 21 };
				zonexpreqs = new int[11, 3]
				{
					{18, 40, 300}, // early game
					{150, 400, 800}, // ocean/pravoka/elfland/marsh
					{600, 800, 1200}, // melmond and other overworld area
					{600, 800, 2400}, // earth cave
					{800, 2000, 4000}, // crescent/onrac/cardia/power peninsula overworld
					{1200, 1500, 3600}, // gurgu/river south
					{1350, 2800, 4800}, // ordeals ice waterfall
					{1050, 2500, 5200}, // lefein/mirage desert/gaia overworld/river north
					{2400, 3600, 7200}, // mirage tower and sea shrine
					{2400, 4800, 10000}, // sky castle
					{6000, 6001, 32000}, // tofr
					
				};
				zone = new List<byte>[zonecountmin.Length];
				for (int i = 0; i < zone.Length; ++i)
					zone[i] = new List<byte> { };
				for (int i = 0; i < EnemyCount; ++i)
					enemyZones[i] = new List<int> { };
				zonemons = new List<byte>[zonecountmin.Length];
				for (int i = 0; i < zone.Length; ++i)
					zonemons[i] = new List<byte> { };
				zoneaddr = new List<int>[zonecountmin.Length];
				zoneaddr[0] = new List<int> { 0x0D8, 0x120, 0x260 };
				zoneaddr[1] = new List<int> { 0x128, 0x130, 0x0E8, 0x1E0, 0x118, 0x128, 0x158, 0x168, 0x170, 0x190, 0x198, 0x1A0, 0x1D0, 0x1D8, 0x210, 0x2B0, 0x2D8, 0x2E0 };
				zoneaddr[2] = new List<int> { 0x108, 0x110, 0x138, 0x140, 0x148, 0x150, 0x178, 0x3E0 };
				zoneaddr[3] = new List<int> { 0x268, 0x2E8, 0x2F0, 0x2F8, 0x300 };
				zoneaddr[4] = new List<int> { 0x0F8, 0x1A8, 0x1B0, 0x1B8, 0x1E8, 0x1F0, 0x1F8 };
				zoneaddr[5] = new List<int> { 0x208, 0x270, 0x308, 0x310, 0x318, 0x320 };
				zoneaddr[6] = new List<int> { 0x278, 0x290, 0x2C8, 0x2D0, 0x328, 0x330 };
				zoneaddr[7] = new List<int> { 0x000, 0x010, 0x018, 0x028, 0x030, 0x040, 0x048, 0x050, 0x068, 0x078, 0x200 };
				zoneaddr[8] = new List<int> { 0x2B8, 0x340, 0x348, 0x350, 0x358, 0x360, 0x368 };
				zoneaddr[9] = new List<int> { 0x378, 0x380, 0x388, 0x390, 0x398 };
				zoneaddr[10] = new List<int> { 0x3A0, 0x3A8, 0x3B0, 0x3B8, 0x3C0, 0x3C8, 0x3D0 };
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

			public void SetVanillaInfo()
			{
				
			}


			public void PutF(int offset, byte[] data)
			{
				if (offset > formationData.Length || offset < 0)
					return;
				for (int i = 0; i < data.Length; ++i)
				{
					formationData[offset + i] = data[i];
				}
			}

			public byte[] GetF(uint offset, int length)
			{
				byte[] return_value;
				if (length < 0)
				{
					if (-length + 1 < 0)
					{
						return null;
					}
					return_value = new byte[-length];
					for (int i = 0; i < -length; ++i)
					{
						return_value[i] = formationData[i + length + 1];
					}
					return return_value;
				}
				return_value = new byte[length];
				for (int i = 0; i < length; ++i)
				{
					return_value[i] = formationData[i];
				}
				return return_value;
			}

			public void PutAllFormationData(byte[] ROMinfo)
			{
				formationData = ROMinfo;
			}

			public byte[] GetFormationData()
			{
				return formationData;
			}

			public void PutD(int offset, int entry)
			{
				domainData[offset] = (byte)entry;
			}

			public byte GetD(int offset)
			{
				return domainData[offset];
			}

			public void PutAllDomainData(byte[] ROMinfo)
			{
				domainData = ROMinfo;
			}

			public byte[] GetDomainData()
			{
				return domainData;
			}

			public byte GetTileset(byte mon)
			{
				return enemyTilesets[mon];
			}

			public byte GetPalette(byte mon)
			{
				return enemyPalettes[mon];
			}

			public int GetMonLimit(byte mon, int shape)
			{
				switch (shape)
				{
					case 0x00:
						if (Small(mon))
							return 9;
						return 0;
					case 0x01:
						if (Large(mon))
							return 4;
						return 0;
					case 0x02:
						if (Large(mon))
							return 2;
						return 6;
					default:
						return 0;
				}
			}

			public bool Small(byte mon)
			{
				return !enemySmallOrBig[mon];
			}

			public bool Large(byte mon)
			{
				return enemySmallOrBig[mon];
			}

			public bool Featured(byte id)
			{
				return featured[id];
			}

			public bool NotFeatured(byte id)
			{
				return !featured[id];
			}

			public void ClearFeaturedList()
			{
				foreach (byte mon in uniqueEnemyIDs)
					featured[mon] = false;
			}

			public SpellData decompressSpellData(byte[] rawData)
			{
				SpellData sp = new SpellData();
				if (rawData.Length != MagicSize)
					return sp;
				sp.accuracy = rawData[0];
				sp.effect = rawData[1];
				sp.elem = rawData[2];
				sp.targeting = rawData[3];
				sp.routine = rawData[4];
				sp.gfx = rawData[5];
				sp.palette = rawData[6];
				return sp;
			}

			public EnemyData decompressEnemyData(byte[] rawData)
			{
				EnemyData e = new EnemyData();
				if (rawData.Length != EnemySize)
					return e; // return a blank enemy if rawdata is not the right size
				e.exp = rawData[0] + rawData[1] * 256;
				e.gp = rawData[2] + rawData[3] * 256;
				e.hp = rawData[4] + rawData[5] * 256;
				e.morale = rawData[6];
				e.AIscript = rawData[7];
				e.agility = rawData[8];
				e.absorb = rawData[9];
				e.num_hits = rawData[10];
				e.accuracy = rawData[11];
				e.damage = rawData[12];
				e.critrate = rawData[13];
				e.atk_elem = rawData[14];
				e.atk_ailment = rawData[15];
				e.monster_type = rawData[16];
				e.mdef = rawData[17];
				e.elem_weakness = rawData[18];
				e.elem_resist = rawData[19];
				return e;
			}

			public EnemyScript decompressEnemyScript(byte[] rawData)
			{
				EnemyScript s = new EnemyScript();
				if (rawData.Length != ScriptSize)
				{
					s.spell_chance = 0;
					s.skill_chance = 0;
					s.spell_list[0] = 0xFF;
					s.spell_list[1] = 0xFF;
					s.spell_list[2] = 0xFF;
					s.spell_list[3] = 0xFF;
					s.spell_list[4] = 0xFF;
					s.spell_list[5] = 0xFF;
					s.spell_list[6] = 0xFF;
					s.spell_list[7] = 0xFF;
					s.skill_list[0] = 0xFF;
					s.skill_list[1] = 0xFF;
					s.skill_list[2] = 0xFF;
					s.skill_list[3] = 0xFF;
				}
				s.spell_chance = rawData[0];
				s.skill_chance = rawData[1];
				s.spell_list[0] = rawData[2];
				s.spell_list[1] = rawData[3];
				s.spell_list[2] = rawData[4];
				s.spell_list[3] = rawData[5];
				s.spell_list[4] = rawData[6];
				s.spell_list[5] = rawData[7];
				s.spell_list[6] = rawData[8];
				s.spell_list[7] = rawData[9];
				s.skill_list[0] = rawData[11];
				s.skill_list[1] = rawData[12];
				s.skill_list[2] = rawData[13];
				s.skill_list[3] = rawData[14];
				return s;
			}

			public Formation decompressFormation(byte[] rawData)
			{
				Formation f = new Formation();
				if (rawData.Length != FormationDataSize)
					return f; // return a blank formation if rawdata is not the right size
				f.shape = (rawData[0] & 0xF0) >> 4;
				f.tileset = (byte)(rawData[0] & 0x0F);
				f.pics = rawData[1];
				f.id[0] = rawData[2];
				f.id[1] = rawData[3];
				f.id[2] = rawData[4];
				f.id[3] = rawData[5];
				f.monMin[0] = (rawData[6] & 0xF0) >> 4;
				f.monMax[0] = rawData[6] & 0x0F;
				f.monMin[1] = (rawData[7] & 0xF0) >> 4;
				f.monMax[1] = rawData[7] & 0x0F;
				f.monMin[2] = (rawData[8] & 0xF0) >> 4;
				f.monMax[2] = rawData[8] & 0x0F;
				f.monMin[3] = (rawData[9] & 0xF0) >> 4;
				f.monMax[3] = rawData[9] & 0x0F;
				f.pal1 = rawData[10];
				f.pal2 = rawData[11];
				f.surprise = rawData[12];
				f.paletteassignment = (byte)(rawData[13] & 0xF0);
				f.unrunnable = (rawData[13] & 0x01) == 0x01 ? true : false;
				f.monMin[4] = (rawData[14] & 0xF0) >> 4;
				f.monMax[4] = rawData[14] & 0x0F;
				f.monMin[5] = (rawData[15] & 0xF0) >> 4;
				f.monMax[5] = rawData[15] & 0x0F;
				return f;
			}

			public void LogAndCompressFormation(Formation f, int slot)
			{
				for (int i = 0; i < 4; ++i)
				{
					if (f.id[i] == 0xFF)
						continue;
					if (f.monMin[i] > 0)
						featured[f.id[i]] = true;
				}
				for (int i = 0; i < 2; ++i)
				{
					if (f.id[i] == 0xFF)
						continue;
					if (f.monMin[i + 4] > 0)
						featured[f.id[i]] = true;
				}
				PutF(slot * FormationDataSize, f.CompressData());
			}

			public void ReadSpellDataFromMagicBytes(SpellData sp, int index)
			{
				spells[index] = sp;
			}

			public void ReadEnemyDataFromEnemies(EnemyData e, int index)
			{
				enemyStats[index] = e;
			}

			public void ReadScriptDataFromScript(EnemyScript s, int index)
			{
				enemyScripts[index] = s;
			}

			public void ReadEnemyDataFromFormation(Formation f, int index)
			{
				// enter the formation into the formations array
				formationInfo[index] = f;
				// pull information about the enemy's graphical data and other things that are held in formation data rather than the enemy data proper
				if (f.tileset < 0x00 || f.tileset > GenericTilesetsCount - 1)
				{
					enemyTilesets[f.id[0]] = f.tileset;
					enemyPalettes[f.id[0]] = 0xFF;
					return; // don't enter data for enemies that use fiend tilesets or invalid tilesets, except for their tileset ID and a placeholder for palette
				}
				for (int i = 0; i < 4; ++i)
				{
					if(i < 2)
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
					if (!enemiesInTileset[f.tileset].Contains(f.id[i])) {
						// place enemy in appropriate tileset
						enemiesInTileset[f.tileset].Add(f.id[i]);
						enemyTilesets[f.id[i]] = f.tileset;
						// determine which pic is being used
						int pict = (f.pics >> (i * 2)) & 0b00000011;
						enemyPics[f.id[i]] = pict;
						if ((pict & 0b00000001) == 1)
							enemySmallOrBig[f.id[i]] = true;
						else
							enemySmallOrBig[f.id[i]] = false;
						enemyImage[f.id[i]] = (int)enemyTilesets[f.id[i]] << 2 | pict;
						bool paletteassigned = (f.paletteassignment & (0b10000000 >> i)) != 0;
						if (paletteassigned)
							enemyPalettes[f.id[i]] = f.pal2;
						else
							enemyPalettes[f.id[i]] = f.pal1;
						if (!palettesInTileset[f.tileset].Contains(f.pal1))
							palettesInTileset[f.tileset].Add(f.pal1);
						if (!palettesInTileset[f.tileset].Contains(f.pal2))
							palettesInTileset[f.tileset].Add(f.pal2);
					}
				}
			}

			public void PurgeIDFromEnemyTilesetList(byte mon)
			{
				for(int i = 0; i < GenericTilesetsCount; ++i)
				{
					enemiesInTileset[i].RemoveAll(id => id == mon);
				}
			}

			public void PrintFormationInfo()
			{
				for (int i = 0; i < zonecountmin.Length; ++i)
				{
					Console.WriteLine("Zone " + i + " " + zone[i].Count + "/" + zonecountmin[i]);
				}
			}

			public bool DoEnemies(MT19337 rng)
			{
				int num_enemy_name_chars = 0;
				for(int i = 0; i < EnemyCount; ++i)
				{
					num_enemy_name_chars += enemyNames[i].Length;
				}
				Console.WriteLine(num_enemy_name_chars);
				// set enemy default tier list.  these listings are based on a combination of where the enemy is placed in the game, its xp yield, its rough difficulty, whether it has a script or not, and gut feels
				// -1 indicates a monster with special rules for enemy generation
				int[] enemyTierList = new int[] { -1, 0, 0, 1, 1, 3, 1, 6, 5, 4, 6, 6, 0, 1, 4, -1,
												  1, 2, 5, 0, 7, 0, 3, 1, 2, 2, 4, 2, 2, 5, 1, 2,
												  5, 2, 5, 3, 4, 4, 5, 1, 2, 3, 5, 0, 1, 1, 2, 8,
												  5, 5, 7, -1, 4, 5, 4, 4, 4, 5, 3, 4, 5, 7, 1, 3,
												  4, 5, 5, 6, 6, 1, 2, 2, 7, 0, 1, 5, 4, 6, 7, 3,
												  5, 2, 3, 5, 5, 7, 9, 2, 4, 4, 5, 4, 7, 4, 5, 6,
												  8, 6, 6, 6, 7, 6, 8, 2, 4, -1, 8, 8, 5, 6, 9, 6,
												  7, -1, 4, 7, 1, 5, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1};
				List<int> enemyImageLUT = new List<int>   { 0b00000000, 0b00000010, 0b00000001, 0b00000011, 0b00000100, 0b00000110, 0b00000101, 0b00000111,
															0b00001000, 0b00001010, 0b00001001, 0b00001011, 0b00001100, 0b00001110, 0b00001101, 0b00001111,
															0b00010000, 0b00010010, 0b00010001, 0b00010011, 0b00010100, 0b00010110, 0b00010101, 0b00010111,
															0b00011000, 0b00011010, 0b00011001, 0b00011011, 0b00011100, 0b00011110, 0b00011101, 0b00011111,
															0b00100000, 0b00100010, 0b00100001, 0b00100011, 0b00100100, 0b00100110, 0b00100101, 0b00100111,
															0b00101000, 0b00101010, 0b00101001, 0b00101011, 0b00101100, 0b00101110, 0b00101101, 0b00101111,
															0b00110000, 0b00110010, 0b00110001, 0b00110011 }; // these are the default tilesets + pics of the enemies we wish to shuffle
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
				monsterClassVariants[40] = new List<string> {  }; // bot variants with special perks
				monsterClassVariants[41] = new List<string> { "Fr", "Z.", "Sea" }; // naga variants with special perks
				monsterClassVariants[42] = new List<string> { }; // small elem variants with special perks
				monsterClassVariants[43] = new List<string> { "Z.", "Wr", "Wz" }; // chimera variants with special perks
				monsterClassVariants[44] = new List<string> { "Z.", "Wz" }; // piscodemon variants with special perks
				monsterClassVariants[45] = new List<string> {  }; // dragon 2 variants with special perks
				monsterClassVariants[46] = new List<string> { "Fr", "R.", "Z.", "Wz" }; // knight variants with special perks
				monsterClassVariants[47] = new List<string> {  }; // golem variants with special perks
				monsterClassVariants[48] = new List<string> { "Fr", "R.", "Z.", "Wz" }; // badman variants with special perks
				monsterClassVariants[49] = new List<string> { "Fr", "R.", "Z.", "Wr", "Sea" }; // pony variants with special perks
				monsterClassVariants[50] = new List<string> { "Fr", "R.", "Z.", "Sea" }; // elf variants with special perks
				monsterClassVariants[51] = new List<string> { }; // mech variants with special perks
				List<string>[] monsterNameVariants = new List<string>[52]; // name variants for monsters to prevent duplicates
				bool[] monsterBaseNameUsed = new bool[52]; // true if the base name has been used, false if not
				for(int i = 0; i < 52; ++i)
				{
					monsterBaseNameUsed[i] = false; // if enemy is not part of a variant class, by default it uses the base name for the monster
					monsterNameVariants[i] = new List<string> { "A.", "B.", "C.", "D.", "E.", "F.", "G.", "H.", "I.", "K.", "L.", "M.", "N.", "P.", "S.", "T.", "V.", "W.", "X." };
				}
				monsterBaseNameUsed[25] = true; // large elemental
				monsterBaseNameUsed[27] = true; // dragon 1
				monsterBaseNameUsed[42] = true; // small elemental
				monsterBaseNameUsed[45] = true; // dragon 2
				enemyImageLUT.Shuffle(rng); // shuffle the LUT - whatever image was there in vanilla will be replaced with what is at its position in the LUT
				enemyImage.RemoveRange(Enemy.Lich, Enemy.Chaos - Enemy.Lich); // remove Lich 1 thru Chaos from the enemyImage array so they're not shuffled to a generic enemy
				enemyImage.Shuffle(rng); // and shuffle the enemy images themselves
				int whatIsPirate = enemyImageLUT[enemyImage[Enemy.Pirate]]; // find out what monster was assigned to the pirate
				for (int i = 0; i < enemyImageLUT.Count(); ++i)
				{
					if (enemyImageLUT[i] == 0b00000110)
					{
						enemyImageLUT[i] = whatIsPirate; // assign whatever what given the pirate image the image that was assigned to the pirate monster's slot
						break;
					}
				}
				enemyImageLUT[enemyImage[Enemy.Pirate]] = 0b00000110; // and now assign the pirate himself his proper slot

				// generate the palettes for each tileset
				for (int i = 0; i < GenericTilesetsCount; ++i)
				{
					palettesInTileset[i].Clear();
					while (palettesInTileset[i].Count < 4)
					{
						int newPal = rng.Between(0, 0x3F);
						if (palettesInTileset[i].Contains((byte)newPal))
							continue;
						palettesInTileset[i].Add((byte)newPal);
					}
				}

				// clear the list of enemies in each tileset
				enemiesInTileset[0].Clear();
				enemiesInTileset[1].Clear();
				enemiesInTileset[2].Clear();
				enemiesInTileset[3].Clear();
				enemiesInTileset[4].Clear();
				enemiesInTileset[5].Clear();
				enemiesInTileset[6].Clear();
				enemiesInTileset[7].Clear();
				enemiesInTileset[8].Clear();
				enemiesInTileset[9].Clear();
				enemiesInTileset[10].Clear();
				enemiesInTileset[11].Clear();
				enemiesInTileset[12].Clear();

				// List of elementals already selected
				List<int> elementalsSelected = new List<int> { 0, 1, 2, 3, 4, 5, 6, 7, 8 };
				List<int> dragonsSelected = new List<int> { 0, 1, 2, 3, 4, 5, 6, 7, 8 };

				// create lists of palettes used for each enemy image
				List<byte>[] enemyImagePalettes = new List<byte>[52];
				for(int i = 0; i < 52; ++i)
				{
					enemyImagePalettes[i] = new List<byte> { };
				}

				// now start generating the enemies themselves.  we stop before Lich and make special exceptions for various monsters
				for (byte i = 0; i < Enemy.Lich; ++i)
				{
					// set up variables we want to work with
					List<MonsterPerks> perks = new List<MonsterPerks> { MonsterPerks.PERK_GAINSTAT10, MonsterPerks.PERK_LOSESTAT10 }; // list of perks this monster is eligible to receive

					// assign the monster's pic
					enemyImage[i] = enemyImageLUT[enemyImage[i]];
					int enemyTileset = (enemyImage[i] & 0b00111100) >> 2;
					if(enemyTileset < 0 || enemyTileset > 12)
					{
						// enemy not in a valid tileset - abort enemizer
						return false;
					}
					enemyPics[i] = enemyImage[i] & 0b00000011;
					enemyTilesets[i] = (byte)enemyTileset;
					bool wasSmallOrBig = enemySmallOrBig[i]; // remember whether the base enemy was a small or big
					if ((enemyImage[i] & 1) == 1) // and determine whether the new image is a small or a big
						enemySmallOrBig[i] = true;
					else
						enemySmallOrBig[i] = false;
					// assign the monster's palette.  do not allow the same palette to be used for the same pic
					byte thisEnemyPalette = palettesInTileset[enemyTileset].PickRandom(rng);
					while (enemyImagePalettes[enemyImage[i]].Contains(thisEnemyPalette))
					{
						thisEnemyPalette = palettesInTileset[enemyTileset].PickRandom(rng);
					}
					enemyImagePalettes[enemyImage[i]].Add(thisEnemyPalette);
					enemyPalettes[i] = thisEnemyPalette;
					// generate the stats for each monster
					if(enemyTierList[i] == -1)
					{
						// generate special monsters
						switch(i)
						{
							case Enemy.Imp:
								enemyNames[i] = "BUM";
								break;
							case Enemy.Pirate:
								break;
							case Enemy.Phantom:
								break;
							case Enemy.Garland:
								break;
							case Enemy.Astos:
								break;
							case Enemy.WarMech:
								break;
						}
					}
					else
					{
						enemiesInTileset[enemyTileset].Add(i); // add enemy to enemiesInTileset unless it is a boss
						// adjust HP, EXP, and GP if enemy changes size
						if (wasSmallOrBig && !enemySmallOrBig[i]) // large became small
						{
							enemyStats[i].hp *= 4;
							enemyStats[i].hp /= 5;
							enemyStats[i].exp *= 9;
							enemyStats[i].exp /= 10;
							enemyStats[i].gp *= 9;
							enemyStats[i].gp /= 10;
						}
						else if (!wasSmallOrBig && enemySmallOrBig[i]) // small became large
						{
							enemyStats[i].hp *= 5;
							enemyStats[i].hp /= 4;
							enemyStats[i].exp *= 10;
							enemyStats[i].exp /= 9;
							enemyStats[i].gp *= 10;
							enemyStats[i].gp /= 9;
						}
						int elemental = 9; // track elemental affinity for certain classes of monsters (elementals and dragons)
						// generate BASE stats for generic monsters and their base name
						switch (enemyImage[i])
						{
							case 0: // Imp
								enemyNames[i] = "IMP";
								enemyStats[i].num_hits = rng.Between(1, 3);
								enemyStats[i].critrate = rng.Between(1, 5);
								enemyStats[i].damage = rollEnemyStrength(enemyTierList[i], 4);
								enemyStats[i].accuracy = rollEnemyAccuracy(enemyTierList[i], 4);
								enemyStats[i].absorb = rollEnemyAbsorb(enemyTierList[i], 5);
								enemyStats[i].agility = rollEnemyEvade(enemyTierList[i], 3);
								enemyStats[i].atk_elem = 0b00000000;
								enemyStats[i].atk_ailment = 0b00000000;
								enemyStats[i].monster_type = 0b00000100;
								enemyStats[i].elem_resist = 0b00000000;
								enemyStats[i].elem_weakness = 0b00000000;
								perks.Add(MonsterPerks.PERK_LOWRESIST);
								perks.Add(MonsterPerks.PERK_HIGHRESIST);
								perks.Add(MonsterPerks.PERK_LOWWEAKNESS);
								perks.Add(MonsterPerks.PERK_HIGHWEAKNESS);
								perks.Add(MonsterPerks.PERK_PLUSONEHIT);
								perks.Add(MonsterPerks.PERK_POISONTOUCH);
								perks.Add(MonsterPerks.PERK_STUNSLEEPTOUCH);
								perks.Add(MonsterPerks.PERK_MUTETOUCH);
								break;
							case 1: // Iguana
								enemyNames[i] = "SAUR";
								enemyStats[i].num_hits = rng.Between(1, 3);
								enemyStats[i].critrate = rng.Between(1, 5);
								enemyStats[i].damage = rollEnemyStrength(enemyTierList[i], 4);
								enemyStats[i].accuracy = rollEnemyAccuracy(enemyTierList[i], 5);
								enemyStats[i].absorb = rollEnemyAbsorb(enemyTierList[i], 5);
								enemyStats[i].agility = rollEnemyEvade(enemyTierList[i], 2);
								enemyStats[i].atk_elem = 0b00000000;
								enemyStats[i].atk_ailment = 0b00000000;
								enemyStats[i].monster_type = 0b00000010;
								enemyStats[i].elem_resist = 0b00000000;
								enemyStats[i].elem_weakness = 0b00000000;
								perks.Add(MonsterPerks.PERK_LOWRESIST);
								perks.Add(MonsterPerks.PERK_HIGHRESIST);
								perks.Add(MonsterPerks.PERK_LOWWEAKNESS);
								perks.Add(MonsterPerks.PERK_HIGHWEAKNESS);
								perks.Add(MonsterPerks.PERK_PLUSONEHIT);
								perks.Add(MonsterPerks.PERK_POISONTOUCH);
								perks.Add(MonsterPerks.PERK_STUNSLEEPTOUCH);
								perks.Add(MonsterPerks.PERK_MUTETOUCH);
								break;
							case 2: // Wolf
								enemyNames[i] = "WOLF";
								enemyStats[i].num_hits = 1;
								enemyStats[i].critrate = rng.Between(1, 5);
								enemyStats[i].damage = rollEnemyStrength(enemyTierList[i], 4);
								enemyStats[i].accuracy = rollEnemyAccuracy(enemyTierList[i], 5);
								enemyStats[i].absorb = 0;
								enemyStats[i].agility = rollEnemyEvade(enemyTierList[i], 6);
								enemyStats[i].atk_elem = 0b00000000;
								enemyStats[i].atk_ailment = 0b00000000;
								enemyStats[i].monster_type = 0b00000000;
								enemyStats[i].elem_resist = 0b00000000;
								enemyStats[i].elem_weakness = 0b00000000;
								perks.Add(MonsterPerks.PERK_LOWRESIST);
								perks.Add(MonsterPerks.PERK_HIGHRESIST);
								perks.Add(MonsterPerks.PERK_LOWWEAKNESS);
								perks.Add(MonsterPerks.PERK_HIGHWEAKNESS);
								perks.Add(MonsterPerks.PERK_PLUSONEHIT);
								break;
							case 3: // Giant
								enemyNames[i] = "GIANT";
								enemyStats[i].num_hits = 1;
								enemyStats[i].critrate = rng.Between(1, 5);
								enemyStats[i].damage = rollEnemyStrength(enemyTierList[i], 6);
								enemyStats[i].accuracy = rollEnemyAccuracy(enemyTierList[i],6);
								enemyStats[i].absorb = rollEnemyAbsorb(enemyTierList[i], 3);
								enemyStats[i].agility = rollEnemyEvade(enemyTierList[i], 4);
								enemyStats[i].atk_elem = 0b00000000;
								enemyStats[i].atk_ailment = 0b00000000;
								enemyStats[i].monster_type = 0b00000100;
								enemyStats[i].elem_resist = 0b00000000;
								enemyStats[i].elem_weakness = 0b00000000;
								perks.Add(MonsterPerks.PERK_LOWRESIST);
								perks.Add(MonsterPerks.PERK_HIGHRESIST);
								perks.Add(MonsterPerks.PERK_LOWWEAKNESS);
								perks.Add(MonsterPerks.PERK_HIGHWEAKNESS);
								break;
							case 4: // Sahag
								enemyNames[i] = "SAHAG";
								enemyStats[i].num_hits = 1;
								enemyStats[i].critrate = 1;
								enemyStats[i].damage = rollEnemyStrength(enemyTierList[i], 4);
								enemyStats[i].accuracy = rollEnemyAccuracy(enemyTierList[i], 2);
								enemyStats[i].absorb = rollEnemyAbsorb(enemyTierList[i], 3);
								enemyStats[i].agility = 72 + enemyStats[i].exp / 40;
								if (enemyStats[i].agility > 100)
									enemyStats[i].agility = 100;
								enemyStats[i].atk_elem = 0b00000000;
								enemyStats[i].atk_ailment = 0b00000000;
								enemyStats[i].monster_type = 0b00100000;
								enemyStats[i].elem_resist = 0b10010000;
								enemyStats[i].elem_weakness = 0b01000000;
								perks.Add(MonsterPerks.PERK_HIGHRESIST);
								perks.Add(MonsterPerks.PERK_HIGHWEAKNESS);
								perks.Add(MonsterPerks.PERK_PLUSONEHIT);
								perks.Add(MonsterPerks.PERK_POISONTOUCH);
								perks.Add(MonsterPerks.PERK_STUNSLEEPTOUCH);
								perks.Add(MonsterPerks.PERK_MUTETOUCH);
								break;
							case 5: // Shark
								enemyNames[i] = "SHARK";
								enemyStats[i].num_hits = 1;
								enemyStats[i].critrate = 1;
								enemyStats[i].damage = rollEnemyStrength(enemyTierList[i], 6);
								enemyStats[i].accuracy = rollEnemyAccuracy(enemyTierList[i], 5);
								enemyStats[i].absorb = rng.Between(0, 8);
								enemyStats[i].agility = 72 + enemyStats[i].exp / 40;
								if (enemyStats[i].agility > 100)
									enemyStats[i].agility = 100;
								enemyStats[i].atk_elem = 0b00000000;
								enemyStats[i].atk_ailment = 0b00000000;
								enemyStats[i].monster_type = 0b00100000;
								enemyStats[i].elem_resist = 0b10010000;
								enemyStats[i].elem_weakness = 0b01000000;
								perks.Add(MonsterPerks.PERK_HIGHRESIST);
								perks.Add(MonsterPerks.PERK_HIGHWEAKNESS);
								perks.Add(MonsterPerks.PERK_PLUSONEHIT);
								perks.Add(MonsterPerks.PERK_POISONTOUCH);
								break;
							case 6: // Pirate
								enemyNames[i] = "BRUTE";
								enemyStats[i].num_hits = rng.Between(1, 2);
								enemyStats[i].critrate = rng.Between(1, 5);
								enemyStats[i].damage = rollEnemyStrength(enemyTierList[i], 4);
								enemyStats[i].accuracy = rollEnemyAccuracy(enemyTierList[i], 4);
								enemyStats[i].absorb = rollEnemyAbsorb(enemyTierList[i], 4);
								enemyStats[i].agility = rollEnemyEvade(enemyTierList[i], 4);
								enemyStats[i].atk_elem = 0b00000000;
								enemyStats[i].atk_ailment = 0b00000000;
								enemyStats[i].monster_type = 0b00000000;
								enemyStats[i].elem_resist = 0b10000000;
								enemyStats[i].elem_weakness = 0b00000000;
								perks.Add(MonsterPerks.PERK_LOWRESIST);
								perks.Add(MonsterPerks.PERK_HIGHRESIST);
								perks.Add(MonsterPerks.PERK_LOWWEAKNESS);
								perks.Add(MonsterPerks.PERK_HIGHWEAKNESS);
								perks.Add(MonsterPerks.PERK_PLUSONEHIT);
								perks.Add(MonsterPerks.PERK_POISONTOUCH);
								perks.Add(MonsterPerks.PERK_STUNSLEEPTOUCH);
								perks.Add(MonsterPerks.PERK_MUTETOUCH);
								break;
							case 7: // Oddeye
								enemyNames[i] = "EYE";
								enemyStats[i].num_hits = rng.Between(1, 4);
								enemyStats[i].critrate = 1;
								enemyStats[i].damage = rollEnemyStrength(enemyTierList[i], 2);
								enemyStats[i].accuracy = rollEnemyAccuracy(enemyTierList[i], 5);
								enemyStats[i].absorb = rollEnemyAbsorb(enemyTierList[i], 2);
								enemyStats[i].agility = enemyStats[i].exp > 400 ? rollEnemyEvade(enemyTierList[i], 4) : rollEnemyEvade(enemyTierList[i], 7);
								enemyStats[i].atk_elem = 0b00000000;
								enemyStats[i].atk_ailment = 0b00000000;
								enemyStats[i].monster_type = 0b00100000;
								enemyStats[i].elem_resist = 0b10010000;
								enemyStats[i].elem_weakness = 0b01000000;
								perks.Add(MonsterPerks.PERK_HIGHRESIST);
								perks.Add(MonsterPerks.PERK_HIGHWEAKNESS);
								perks.Add(MonsterPerks.PERK_PLUSONEHIT);
								perks.Add(MonsterPerks.PERK_POISONTOUCH);
								perks.Add(MonsterPerks.PERK_STUNSLEEPTOUCH);
								perks.Add(MonsterPerks.PERK_MUTETOUCH);
								break;
							case 8: // Skeleton
								enemyNames[i] = "BONE";
								enemyStats[i].num_hits = 1;
								enemyStats[i].critrate = rng.Between(1, 2);
								enemyStats[i].damage = rollEnemyStrength(enemyTierList[i], 5);
								enemyStats[i].accuracy = rollEnemyAccuracy(enemyTierList[i], 4);
								enemyStats[i].absorb = rollEnemyAbsorb(enemyTierList[i], 2);
								enemyStats[i].agility = rollEnemyEvade(enemyTierList[i], 4);
								enemyStats[i].atk_elem = 0b00000000;
								enemyStats[i].atk_ailment = 0b00000000;
								enemyStats[i].monster_type = 0b00001000;
								enemyStats[i].elem_resist = 0b00101011;
								enemyStats[i].elem_weakness = 0b00010000;
								break;
							case 9: // Hyena
								enemyNames[i] = "DOG";
								enemyStats[i].num_hits = rng.Between(1, 2);
								enemyStats[i].critrate = rng.Between(1, 3);
								enemyStats[i].damage = rollEnemyStrength(enemyTierList[i], 4);
								enemyStats[i].accuracy = rollEnemyAccuracy(enemyTierList[i], 4);
								enemyStats[i].absorb = rng.Between(0, 12);
								enemyStats[i].agility = rollEnemyEvade(enemyTierList[i], 5);
								enemyStats[i].atk_elem = 0b00000000;
								enemyStats[i].atk_ailment = 0b00000000;
								enemyStats[i].monster_type = 0b00000000;
								enemyStats[i].elem_resist = 0b00000000;
								enemyStats[i].elem_weakness = 0b00000000;
								perks.Add(MonsterPerks.PERK_LOWRESIST);
								perks.Add(MonsterPerks.PERK_HIGHRESIST);
								perks.Add(MonsterPerks.PERK_LOWWEAKNESS);
								perks.Add(MonsterPerks.PERK_HIGHWEAKNESS);
								perks.Add(MonsterPerks.PERK_PLUSONEHIT);
								perks.Add(MonsterPerks.PERK_POISONTOUCH);
								perks.Add(MonsterPerks.PERK_STUNSLEEPTOUCH);
								perks.Add(MonsterPerks.PERK_MUTETOUCH);
								break;
							case 10: // Creep
								enemyNames[i] = "CRAWL";
								enemyStats[i].num_hits = 1;
								enemyStats[i].critrate = 1;
								enemyStats[i].damage = rollEnemyStrength(enemyTierList[i], 6);
								enemyStats[i].accuracy = rollEnemyAccuracy(enemyTierList[i], 4);
								enemyStats[i].absorb = rollEnemyAbsorb(enemyTierList[i], 4);
								enemyStats[i].agility = rollEnemyEvade(enemyTierList[i], 4);
								enemyStats[i].atk_elem = 0b00000000;
								enemyStats[i].atk_ailment = 0b00000000;
								enemyStats[i].monster_type = 0b00000000;
								enemyStats[i].elem_resist = 0b00000000;
								enemyStats[i].elem_weakness = 0b00010000;
								perks.Add(MonsterPerks.PERK_LOWRESIST);
								perks.Add(MonsterPerks.PERK_HIGHRESIST);
								perks.Add(MonsterPerks.PERK_LOWWEAKNESS);
								perks.Add(MonsterPerks.PERK_HIGHWEAKNESS);
								perks.Add(MonsterPerks.PERK_POISONTOUCH);
								perks.Add(MonsterPerks.PERK_STUNSLEEPTOUCH);
								perks.Add(MonsterPerks.PERK_MUTETOUCH);
								break;
							case 11: // Ogre
								enemyNames[i] = "OGRE";
								enemyStats[i].num_hits = 1;
								enemyStats[i].critrate = rng.Between(1, 5);
								enemyStats[i].damage = rollEnemyStrength(enemyTierList[i], 5);
								enemyStats[i].accuracy = rollEnemyAccuracy(enemyTierList[i], 4);
								enemyStats[i].absorb = rollEnemyAbsorb(enemyTierList[i], 4);
								enemyStats[i].agility = rollEnemyEvade(enemyTierList[i], 4);
								enemyStats[i].atk_elem = 0b00000000;
								enemyStats[i].atk_ailment = 0b00000000;
								enemyStats[i].monster_type = 0b00000100;
								enemyStats[i].elem_resist = 0b00000000;
								enemyStats[i].elem_weakness = 0b00000000;
								perks.Add(MonsterPerks.PERK_LOWRESIST);
								perks.Add(MonsterPerks.PERK_HIGHRESIST);
								perks.Add(MonsterPerks.PERK_LOWWEAKNESS);
								perks.Add(MonsterPerks.PERK_HIGHWEAKNESS);
								perks.Add(MonsterPerks.PERK_PLUSONEHIT);
								perks.Add(MonsterPerks.PERK_POISONTOUCH);
								break;
							case 12: // Asp
								enemyNames[i] = "ASP";
								enemyStats[i].num_hits = 1;
								enemyStats[i].critrate = rng.Between(1, 31);
								enemyStats[i].damage = rollEnemyStrength(enemyTierList[i], 4);
								enemyStats[i].accuracy = rollEnemyAccuracy(enemyTierList[i], 4);
								enemyStats[i].absorb = rollEnemyAbsorb(enemyTierList[i], 3);
								enemyStats[i].agility = rollEnemyEvade(enemyTierList[i], 5);
								enemyStats[i].atk_elem = 0b00000000;
								enemyStats[i].atk_ailment = 0b00000000;
								enemyStats[i].monster_type = 0b00000010;
								enemyStats[i].elem_resist = 0b00000000;
								enemyStats[i].elem_weakness = 0b00000000;
								perks.Add(MonsterPerks.PERK_LOWRESIST);
								perks.Add(MonsterPerks.PERK_HIGHRESIST);
								perks.Add(MonsterPerks.PERK_LOWWEAKNESS);
								perks.Add(MonsterPerks.PERK_HIGHWEAKNESS);
								perks.Add(MonsterPerks.PERK_POISONTOUCH);
								perks.Add(MonsterPerks.PERK_STUNSLEEPTOUCH);
								perks.Add(MonsterPerks.PERK_MUTETOUCH);
								break;
							case 13: // Bull
								enemyNames[i] = "BULL";
								enemyStats[i].num_hits = 2;
								enemyStats[i].critrate = rng.Between(1, 3);
								enemyStats[i].damage = rollEnemyStrength(enemyTierList[i], 4);
								enemyStats[i].accuracy = rollEnemyAccuracy(enemyTierList[i], 5);
								enemyStats[i].absorb = rollEnemyAbsorb(enemyTierList[i], 4);
								enemyStats[i].agility = rollEnemyEvade(enemyTierList[i], 5);
								enemyStats[i].atk_elem = 0b00000000;
								enemyStats[i].atk_ailment = 0b00000000;
								enemyStats[i].monster_type = 0b00000000;
								enemyStats[i].elem_resist = 0b00000000;
								enemyStats[i].elem_weakness = 0b00000000;
								perks.Add(MonsterPerks.PERK_LOWRESIST);
								perks.Add(MonsterPerks.PERK_HIGHRESIST);
								perks.Add(MonsterPerks.PERK_LOWWEAKNESS);
								perks.Add(MonsterPerks.PERK_HIGHWEAKNESS);
								perks.Add(MonsterPerks.PERK_PLUSONEHIT);
								perks.Add(MonsterPerks.PERK_POISONTOUCH);
								perks.Add(MonsterPerks.PERK_STUNSLEEPTOUCH);
								perks.Add(MonsterPerks.PERK_MUTETOUCH);
								break;
							case 14: // Crab
								enemyNames[i] = "CRAB";
								enemyStats[i].num_hits = rng.Between(2, 4);
								enemyStats[i].critrate = 1;
								enemyStats[i].damage = rollEnemyStrength(enemyTierList[i], 5);
								enemyStats[i].accuracy = rollEnemyAccuracy(enemyTierList[i], 3);
								enemyStats[i].absorb = rollEnemyAbsorb(enemyTierList[i], 4);
								enemyStats[i].agility = rollEnemyEvade(enemyTierList[i], 6);
								enemyStats[i].atk_elem = 0b00000000;
								enemyStats[i].atk_ailment = 0b00000000;
								enemyStats[i].monster_type = 0b00000000;
								enemyStats[i].elem_resist = 0b00000000;
								enemyStats[i].elem_weakness = 0b00000000;
								perks.Add(MonsterPerks.PERK_LOWRESIST);
								perks.Add(MonsterPerks.PERK_HIGHRESIST);
								perks.Add(MonsterPerks.PERK_LOWWEAKNESS);
								perks.Add(MonsterPerks.PERK_HIGHWEAKNESS);
								perks.Add(MonsterPerks.PERK_PLUSONEHIT);
								perks.Add(MonsterPerks.PERK_POISONTOUCH);
								break;
							case 15: // Troll
								enemyNames[i] = "TROLL";
								enemyStats[i].num_hits = 3;
								enemyStats[i].critrate = 1;
								enemyStats[i].damage = rollEnemyStrength(enemyTierList[i], 4);
								enemyStats[i].accuracy = rollEnemyAccuracy(enemyTierList[i], 4);
								enemyStats[i].absorb = rollEnemyAbsorb(enemyTierList[i], 4);
								enemyStats[i].agility = rollEnemyEvade(enemyTierList[i], 4);
								enemyStats[i].atk_elem = 0b00000000;
								enemyStats[i].atk_ailment = 0b00000000;
								enemyStats[i].monster_type = 0b10000000;
								enemyStats[i].elem_resist = 0b00000000;
								enemyStats[i].elem_weakness = 0b00000000;
								perks.Add(MonsterPerks.PERK_LOWRESIST);
								perks.Add(MonsterPerks.PERK_HIGHRESIST);
								perks.Add(MonsterPerks.PERK_LOWWEAKNESS);
								perks.Add(MonsterPerks.PERK_HIGHWEAKNESS);
								perks.Add(MonsterPerks.PERK_PLUSONEHIT);
								perks.Add(MonsterPerks.PERK_POISONTOUCH);
								perks.Add(MonsterPerks.PERK_STUNSLEEPTOUCH);
								perks.Add(MonsterPerks.PERK_MUTETOUCH);
								break;
							case 16: // Spectral Undead
								enemyNames[i] = "GHOST";
								enemyStats[i].num_hits = 1;
								enemyStats[i].critrate = 1;
								enemyStats[i].damage = rollEnemyStrength(enemyTierList[i], 6);
								enemyStats[i].accuracy = rollEnemyAccuracy(enemyTierList[i], 3);
								enemyStats[i].absorb = rng.Between(0, 4) == 4 ? rollEnemyAbsorb(enemyTierList[i], 4) : rng.Between(0, 8);
								enemyStats[i].agility = rng.Between(0, 2) == 2 ? rollEnemyEvade(enemyTierList[i], 4) : rollEnemyEvade(enemyTierList[i], 7);
								enemyStats[i].atk_elem = 0b00000001;
								enemyStats[i].atk_ailment = enemyStats[i].exp < 150 ? (byte)0b00001000 : (byte)0b00010000; // darkness for low xp enemies, stun for high xp enemies
								enemyStats[i].monster_type = 0b00001001;
								enemyStats[i].elem_resist = 0b10101011;
								enemyStats[i].elem_weakness = 0b00010000;
								break;
							case 17: // Worm
								enemyNames[i] = "WORM";
								enemyStats[i].num_hits = 1;
								enemyStats[i].critrate = 1;
								enemyStats[i].damage = rollEnemyStrength(enemyTierList[i], 5);
								enemyStats[i].accuracy = rollEnemyAccuracy(enemyTierList[i], 7);
								enemyStats[i].absorb = enemyStats[i].exp < 100 ? rng.Between(0, 10) : (enemyStats[i].exp > 1200 ? rng.Between(10, 40) : rng.Between(10, 20));
								enemyStats[i].agility = rng.Between(0, 60);
								enemyStats[i].atk_elem = 0b00000000;
								enemyStats[i].atk_ailment = 0b00000000;
								enemyStats[i].monster_type = 0b00000000;
								enemyStats[i].elem_resist = 0b10000000;
								enemyStats[i].elem_weakness = 0b00000000;
								perks.Add(MonsterPerks.PERK_LOWRESIST);
								perks.Add(MonsterPerks.PERK_HIGHRESIST);
								perks.Add(MonsterPerks.PERK_LOWWEAKNESS);
								perks.Add(MonsterPerks.PERK_HIGHWEAKNESS);
								perks.Add(MonsterPerks.PERK_PLUSONEHIT);
								perks.Add(MonsterPerks.PERK_POISONTOUCH);
								perks.Add(MonsterPerks.PERK_STUNSLEEPTOUCH);
								perks.Add(MonsterPerks.PERK_MUTETOUCH);
								break;
							case 18: // Zombie Undead
								enemyNames[i] = "WIGHT";
								enemyStats[i].num_hits = rng.Between(0, 4) == 4 || enemyStats[i].exp < 100 ? 1 : 3;
								enemyStats[i].critrate = 1;
								enemyStats[i].damage = enemyStats[i].num_hits == 3 ? rollEnemyStrength(enemyTierList[i], 2) : rollEnemyStrength(enemyTierList[i], 4);
								enemyStats[i].accuracy = rollEnemyAccuracy(enemyTierList[i], 2);
								enemyStats[i].absorb = rollEnemyAbsorb(enemyTierList[i], 2);
								enemyStats[i].agility = rollEnemyEvade(enemyTierList[i], 5);
								enemyStats[i].atk_elem = 0b00000001;
								enemyStats[i].atk_ailment = enemyStats[i].exp > 80 ? (byte)0b00010000 : (byte)0b00000000;
								enemyStats[i].monster_type = 0b00001000;
								enemyStats[i].elem_resist = 0b00101011;
								enemyStats[i].elem_weakness = 0b00010000;
								break;
							case 19: // Eyes that are totally not Beholders plz no sue
								enemyNames[i] = "DRUJ";
								enemyStats[i].num_hits = 1;
								enemyStats[i].critrate = 1;
								enemyStats[i].damage = rollEnemyStrength(enemyTierList[i], 4);
								enemyStats[i].accuracy = rollEnemyAccuracy(enemyTierList[i], 4);
								enemyStats[i].absorb = rollEnemyAbsorb(enemyTierList[i], 6);
								enemyStats[i].agility = rollEnemyEvade(enemyTierList[i], 2);
								enemyStats[i].atk_elem = 0b00000000;
								enemyStats[i].atk_ailment = 0b00000000;
								enemyStats[i].monster_type = 0b01000000;
								enemyStats[i].elem_resist = 0b10000000;
								enemyStats[i].elem_weakness = 0b00000000;
								if (enemyStats[i].AIscript == 0xFF)
									enemyStats[i].AIscript = ENE_PickForcedAIScript(enemyTierList[i]);
								perks.Add(MonsterPerks.PERK_LOWRESIST);
								perks.Add(MonsterPerks.PERK_HIGHRESIST);
								perks.Add(MonsterPerks.PERK_LOWWEAKNESS);
								perks.Add(MonsterPerks.PERK_HIGHWEAKNESS);
								perks.Add(MonsterPerks.PERK_PLUSONEHIT);
								perks.Add(MonsterPerks.PERK_POISONTOUCH);
								perks.Add(MonsterPerks.PERK_STUNSLEEPTOUCH);
								perks.Add(MonsterPerks.PERK_MUTETOUCH);
								break;
							case 20: // Medusa
								enemyNames[i] = "LAMIA";
								enemyStats[i].num_hits = rng.Between(0, 1) == 1 ? 1 : rng.Between(6, 10);
								enemyStats[i].critrate = rng.Between(1, 5);
								enemyStats[i].damage = rollEnemyStrength(enemyTierList[i], 4);
								enemyStats[i].accuracy = rollEnemyAccuracy(enemyTierList[i], 2);
								enemyStats[i].absorb = rollEnemyAbsorb(enemyTierList[i], 2);
								enemyStats[i].agility = rollEnemyEvade(enemyTierList[i], 5);
								enemyStats[i].atk_elem = 0b00000001;
								enemyStats[i].atk_ailment = enemyStats[i].exp > 200 ? (byte)0b00010000 : (byte)0b00000000;
								enemyStats[i].monster_type = 0b00000000;
								enemyStats[i].elem_resist = 0b00000000;
								enemyStats[i].elem_weakness = 0b00000000;
								perks.Add(MonsterPerks.PERK_LOWRESIST);
								perks.Add(MonsterPerks.PERK_HIGHRESIST);
								perks.Add(MonsterPerks.PERK_LOWWEAKNESS);
								perks.Add(MonsterPerks.PERK_HIGHWEAKNESS);
								perks.Add(MonsterPerks.PERK_PLUSONEHIT);
								break;
							case 21: // Pede
								enemyNames[i] = "PEDE";
								enemyStats[i].num_hits = 1;
								enemyStats[i].critrate = 1;
								enemyStats[i].damage = rollEnemyStrength(enemyTierList[i], 7);
								enemyStats[i].accuracy = rollEnemyAccuracy(enemyTierList[i], 5);
								enemyStats[i].absorb = rollEnemyAbsorb(enemyTierList[i], 5);
								enemyStats[i].agility = rollEnemyEvade(enemyTierList[i], 4);
								enemyStats[i].atk_elem = 0b00000000;
								enemyStats[i].atk_ailment = 0b00000000;
								enemyStats[i].monster_type = 0b00000000;
								enemyStats[i].elem_resist = 0b00000000;
								enemyStats[i].elem_weakness = 0b00000000;
								perks.Add(MonsterPerks.PERK_LOWRESIST);
								perks.Add(MonsterPerks.PERK_LOWWEAKNESS);
								perks.Add(MonsterPerks.PERK_HIGHWEAKNESS);
								perks.Add(MonsterPerks.PERK_POISONTOUCH);
								break;
							case 22: // Were
								enemyNames[i] = "CAT";
								enemyStats[i].num_hits = rng.Between(2, 3);
								enemyStats[i].critrate = rng.Between(1, 2);
								enemyStats[i].damage = rollEnemyStrength(enemyTierList[i], 5);
								enemyStats[i].accuracy = rollEnemyAccuracy(enemyTierList[i], 4);
								enemyStats[i].absorb = rollEnemyAbsorb(enemyTierList[i], 4);
								enemyStats[i].agility = rollEnemyEvade(enemyTierList[i], 5);
								enemyStats[i].atk_elem = 0b00000010;
								enemyStats[i].atk_ailment = 0b00000100;
								enemyStats[i].monster_type = 0b00010000;
								enemyStats[i].elem_resist = 0b00000000;
								enemyStats[i].elem_weakness = 0b00000000;
								perks.Add(MonsterPerks.PERK_LOWRESIST);
								perks.Add(MonsterPerks.PERK_HIGHRESIST);
								perks.Add(MonsterPerks.PERK_LOWWEAKNESS);
								perks.Add(MonsterPerks.PERK_HIGHWEAKNESS);
								break;
							case 23: // Tiger
								enemyNames[i] = "TIGER";
								enemyStats[i].num_hits = 2;
								enemyStats[i].critrate = rng.Between(20, 100);
								enemyStats[i].damage = rollEnemyStrength(enemyTierList[i], 4);
								enemyStats[i].accuracy = rollEnemyAccuracy(enemyTierList[i], 4);
								enemyStats[i].absorb = 8;
								enemyStats[i].agility = rollEnemyEvade(enemyTierList[i], 5);
								enemyStats[i].atk_elem = 0b00000000;
								enemyStats[i].atk_ailment = 0b00000000;
								enemyStats[i].monster_type = 0b00000000;
								enemyStats[i].elem_resist = 0b00000000;
								enemyStats[i].elem_weakness = 0b00000000;
								perks.Add(MonsterPerks.PERK_LOWRESIST);
								perks.Add(MonsterPerks.PERK_HIGHRESIST);
								perks.Add(MonsterPerks.PERK_LOWWEAKNESS);
								perks.Add(MonsterPerks.PERK_HIGHWEAKNESS);
								perks.Add(MonsterPerks.PERK_POISONTOUCH);
								perks.Add(MonsterPerks.PERK_STUNSLEEPTOUCH);
								perks.Add(MonsterPerks.PERK_MUTETOUCH);
								break;
							case 24: // Blah, it's a Vampire!
								enemyNames[i] = "VAMP";
								enemyStats[i].num_hits = 1;
								enemyStats[i].critrate = rng.Between(1, 5);
								enemyStats[i].damage = rollEnemyStrength(enemyTierList[i], 6);
								enemyStats[i].accuracy = rollEnemyAccuracy(enemyTierList[i], 4);
								enemyStats[i].absorb = rollEnemyAbsorb(enemyTierList[i], 5);
								enemyStats[i].agility = rollEnemyEvade(enemyTierList[i], 6);
								enemyStats[i].atk_elem = 0b00000001;
								enemyStats[i].atk_ailment = 0b00010000;
								enemyStats[i].monster_type = 0b10001001;
								enemyStats[i].elem_resist = 0b10101011;
								enemyStats[i].elem_weakness = 0b00010000;
								break;
							case 25: // Large Elemental
								if (elementalsSelected.Count > 0)
									elemental = elementalsSelected.PickRandom(rng);
								rollLargeElemental(rng, ref enemyStats[i], enemyTierList[i], elemental);
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
								enemyStats[i].num_hits = 4;
								enemyStats[i].critrate = 1;
								enemyStats[i].damage = rollEnemyStrength(enemyTierList[i], 4);
								enemyStats[i].accuracy = rollEnemyAccuracy(enemyTierList[i], 3);
								enemyStats[i].absorb = rollEnemyAbsorb(enemyTierList[i], 4);
								enemyStats[i].agility = rollEnemyEvade(enemyTierList[i], 5);
								enemyStats[i].atk_elem = 0b00000000;
								enemyStats[i].atk_ailment = 0b00000000;
								enemyStats[i].monster_type = 0b00000001;
								enemyStats[i].elem_resist = 0b10000000;
								enemyStats[i].elem_weakness = 0b00000000;
								perks.Add(MonsterPerks.PERK_LOWRESIST);
								perks.Add(MonsterPerks.PERK_HIGHRESIST);
								perks.Add(MonsterPerks.PERK_LOWWEAKNESS);
								perks.Add(MonsterPerks.PERK_HIGHWEAKNESS);
								perks.Add(MonsterPerks.PERK_PLUSONEHIT);
								perks.Add(MonsterPerks.PERK_POISONTOUCH);
								perks.Add(MonsterPerks.PERK_STUNSLEEPTOUCH);
								perks.Add(MonsterPerks.PERK_MUTETOUCH);
								break;
							case 27: // Dragon Type 1
								if (dragonsSelected.Count > 0)
									elemental = dragonsSelected.PickRandom(rng);
								rollDragon(rng, ref enemyStats[i], enemyTierList[i], elemental);
								switch(elemental)
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
								enemyStats[i].num_hits = 1;
								enemyStats[i].critrate = 1;
								enemyStats[i].damage = rollEnemyStrength(enemyTierList[i], 6);
								enemyStats[i].accuracy = rollEnemyAccuracy(enemyTierList[i], 2);
								enemyStats[i].absorb = rng.Between(0, 2) == 2 ? rng.Between(0, 12) : 255;
								enemyStats[i].agility = rollEnemyEvade(enemyTierList[i], 1);
								enemyStats[i].atk_elem = 0b00000000;
								enemyStats[i].atk_ailment = 0b00000000;
								enemyStats[i].monster_type = 0b00000001;
								enemyStats[i].elem_weakness = (byte)(rng.Between(1, 6) << 4); // can be fire, ice, fire+ice, lit, fire+lit, or ice+lit weak
								enemyStats[i].elem_resist = (byte)((0b11111111 ^ enemyStats[i].elem_weakness) & 0b11111011); // resist all other elements except time
								break;
							case 29: // Manticore
								enemyNames[i] = "MANT";
								enemyStats[i].num_hits = rng.Between(2, 3);
								enemyStats[i].critrate = rng.Between(1, 3);
								enemyStats[i].damage = rollEnemyStrength(enemyTierList[i], 4);
								enemyStats[i].accuracy = rollEnemyAccuracy(enemyTierList[i], 5);
								enemyStats[i].absorb = rollEnemyAbsorb(enemyTierList[i], 2);
								enemyStats[i].agility = rollEnemyEvade(enemyTierList[i], 7);
								enemyStats[i].atk_elem = 0b00000000;
								enemyStats[i].atk_ailment = 0b00000000;
								enemyStats[i].monster_type = 0b00000000;
								enemyStats[i].elem_resist = 0b00000000;
								enemyStats[i].elem_weakness = 0b00000000;
								perks.Add(MonsterPerks.PERK_LOWRESIST);
								perks.Add(MonsterPerks.PERK_HIGHRESIST);
								perks.Add(MonsterPerks.PERK_LOWWEAKNESS);
								perks.Add(MonsterPerks.PERK_HIGHWEAKNESS);
								perks.Add(MonsterPerks.PERK_PLUSONEHIT);
								perks.Add(MonsterPerks.PERK_POISONTOUCH);
								perks.Add(MonsterPerks.PERK_STUNSLEEPTOUCH);
								perks.Add(MonsterPerks.PERK_MUTETOUCH);
								break;
							case 30: // Spider
								enemyNames[i] = "BUG";
								enemyStats[i].num_hits = 1;
								enemyStats[i].critrate = 1;
								enemyStats[i].damage = rollEnemyStrength(enemyTierList[i], 3);
								enemyStats[i].accuracy = rollEnemyAccuracy(enemyTierList[i], 4);
								enemyStats[i].absorb = rollEnemyAbsorb(enemyTierList[i], 4);
								enemyStats[i].agility = rollEnemyEvade(enemyTierList[i], 4);
								enemyStats[i].atk_elem = 0b00000000;
								enemyStats[i].atk_ailment = 0b00000000;
								enemyStats[i].monster_type = 0b00000000;
								enemyStats[i].elem_resist = 0b00000000;
								enemyStats[i].elem_weakness = 0b00000000;
								perks.Add(MonsterPerks.PERK_LOWRESIST);
								perks.Add(MonsterPerks.PERK_HIGHRESIST);
								perks.Add(MonsterPerks.PERK_PLUSONEHIT);
								perks.Add(MonsterPerks.PERK_POISONTOUCH);
								perks.Add(MonsterPerks.PERK_STUNSLEEPTOUCH);
								perks.Add(MonsterPerks.PERK_MUTETOUCH);
								break;
							case 31: // Ankylo
								enemyNames[i] = "ANK";
								enemyStats[i].num_hits = 1;
								enemyStats[i].critrate = rng.Between(1, 3);
								enemyStats[i].damage = rollEnemyStrength(enemyTierList[i], 7);
								enemyStats[i].accuracy = rollEnemyAccuracy(enemyTierList[i], 6);
								enemyStats[i].absorb = rollEnemyAbsorb(enemyTierList[i], 7);
								enemyStats[i].agility = rollEnemyEvade(enemyTierList[i], 4);
								enemyStats[i].atk_elem = 0b00000000;
								enemyStats[i].atk_ailment = 0b00000000;
								enemyStats[i].monster_type = 0b00000000;
								enemyStats[i].elem_resist = 0b00000000;
								enemyStats[i].elem_weakness = 0b00000000;
								break;
							case 32: // Mummy
								enemyNames[i] = "MUMMY";
								enemyStats[i].num_hits = 1;
								enemyStats[i].critrate = rng.Between(1, 3);
								enemyStats[i].damage = rollEnemyStrength(enemyTierList[i], 3);
								enemyStats[i].accuracy = rollEnemyAccuracy(enemyTierList[i], 4);
								enemyStats[i].absorb = rollEnemyAbsorb(enemyTierList[i], 5);
								enemyStats[i].agility = rollEnemyEvade(enemyTierList[i], 3);
								enemyStats[i].atk_elem = 0b00000001;
								enemyStats[i].atk_ailment = 0b00100000;
								enemyStats[i].monster_type = 0b00001000;
								enemyStats[i].elem_resist = 0b00101011;
								enemyStats[i].elem_weakness = 0b00010000;
								break;
							case 33: // Wyvern
								enemyNames[i] = "WYRM";
								enemyStats[i].num_hits = 1;
								enemyStats[i].critrate = 1;
								enemyStats[i].damage = rollEnemyStrength(enemyTierList[i], 5);
								enemyStats[i].accuracy = rollEnemyAccuracy(enemyTierList[i], 6);
								enemyStats[i].absorb = rollEnemyAbsorb(enemyTierList[i], 4);
								enemyStats[i].agility = 72 + enemyStats[i].exp / 40;
								if (enemyStats[i].agility > 100)
									enemyStats[i].agility = 100;
								enemyStats[i].atk_elem = 0b00000000;
								enemyStats[i].atk_ailment = 0b00000000;
								enemyStats[i].monster_type = 0b00000010;
								enemyStats[i].elem_resist = 0b10000000;
								enemyStats[i].elem_weakness = 0b00000000;
								perks.Add(MonsterPerks.PERK_LOWRESIST);
								perks.Add(MonsterPerks.PERK_HIGHRESIST);
								perks.Add(MonsterPerks.PERK_LOWWEAKNESS);
								perks.Add(MonsterPerks.PERK_HIGHWEAKNESS);
								perks.Add(MonsterPerks.PERK_PLUSONEHIT);
								perks.Add(MonsterPerks.PERK_POISONTOUCH);
								break;
							case 34: // Jerk Bird
								enemyNames[i] = "BIRD";
								enemyStats[i].num_hits = 1;
								enemyStats[i].critrate = 1;
								enemyStats[i].damage = rollEnemyStrength(enemyTierList[i], 3);
								enemyStats[i].accuracy = rollEnemyAccuracy(enemyTierList[i], 3);
								enemyStats[i].absorb = rng.Between(0, 8);
								enemyStats[i].agility = 72;
								enemyStats[i].atk_elem = 0b00000000;
								enemyStats[i].atk_ailment = 0b00000000;
								enemyStats[i].monster_type = 0b00000000;
								enemyStats[i].elem_resist = 0b10000000;
								enemyStats[i].elem_weakness = 0b00000000;
								perks.Add(MonsterPerks.PERK_LOWRESIST);
								perks.Add(MonsterPerks.PERK_HIGHRESIST);
								perks.Add(MonsterPerks.PERK_LOWWEAKNESS);
								perks.Add(MonsterPerks.PERK_HIGHWEAKNESS);
								perks.Add(MonsterPerks.PERK_PLUSONEHIT);
								perks.Add(MonsterPerks.PERK_POISONTOUCH);
								perks.Add(MonsterPerks.PERK_STUNSLEEPTOUCH);
								perks.Add(MonsterPerks.PERK_MUTETOUCH);
								break;
							case 35: // Steak
								enemyNames[i] = "TYRO";
								enemyStats[i].num_hits = 1;
								enemyStats[i].critrate = rng.Between(20, 40);
								enemyStats[i].damage = rollEnemyStrength(enemyTierList[i], 7);
								enemyStats[i].accuracy = rollEnemyAccuracy(enemyTierList[i], 7);
								enemyStats[i].absorb = rollEnemyAbsorb(enemyTierList[i], 2);
								enemyStats[i].agility = rollEnemyEvade(enemyTierList[i], 6);
								enemyStats[i].atk_elem = 0b00000000;
								enemyStats[i].atk_ailment = 0b00000000;
								enemyStats[i].monster_type = 0b00000010;
								enemyStats[i].elem_resist = 0b00000000;
								enemyStats[i].elem_weakness = 0b00000000;
								perks.Add(MonsterPerks.PERK_LOWRESIST);
								perks.Add(MonsterPerks.PERK_HIGHRESIST);
								perks.Add(MonsterPerks.PERK_LOWWEAKNESS);
								perks.Add(MonsterPerks.PERK_HIGHWEAKNESS);
								break;
							case 36: // Pirahna
								enemyNames[i] = "FISH";
								enemyStats[i].num_hits = 1;
								enemyStats[i].critrate = 1;
								enemyStats[i].damage = rollEnemyStrength(enemyTierList[i], 4);
								enemyStats[i].accuracy = rollEnemyAccuracy(enemyTierList[i], 4);
								enemyStats[i].absorb = rollEnemyAbsorb(enemyTierList[i], 2);
								enemyStats[i].agility = 72;
								enemyStats[i].atk_elem = 0b00000000;
								enemyStats[i].atk_ailment = 0b00000000;
								enemyStats[i].monster_type = 0b00100000;
								enemyStats[i].elem_resist = 0b10010000;
								enemyStats[i].elem_weakness = 0b01000000;
								perks.Add(MonsterPerks.PERK_LOWRESIST);
								perks.Add(MonsterPerks.PERK_HIGHRESIST);
								perks.Add(MonsterPerks.PERK_HIGHWEAKNESS);
								perks.Add(MonsterPerks.PERK_PLUSONEHIT);
								perks.Add(MonsterPerks.PERK_POISONTOUCH);
								perks.Add(MonsterPerks.PERK_STUNSLEEPTOUCH);
								break;
							case 37: // Ocho
								enemyNames[i] = "OCHO";
								enemyStats[i].num_hits = 3;
								enemyStats[i].critrate = 1;
								enemyStats[i].damage = rollEnemyStrength(enemyTierList[i], 4);
								enemyStats[i].accuracy = rollEnemyAccuracy(enemyTierList[i], 6);
								enemyStats[i].absorb = rollEnemyAbsorb(enemyTierList[i], 5);
								enemyStats[i].agility = rollEnemyEvade(enemyTierList[i], 4);
								enemyStats[i].atk_elem = 0b00000010;
								enemyStats[i].atk_ailment = 0b00000100;
								enemyStats[i].monster_type = 0b00100000;
								enemyStats[i].elem_resist = 0b10010000;
								enemyStats[i].elem_weakness = 0b01000000;
								perks.Add(MonsterPerks.PERK_LOWRESIST);
								perks.Add(MonsterPerks.PERK_HIGHRESIST);
								perks.Add(MonsterPerks.PERK_HIGHWEAKNESS);
								perks.Add(MonsterPerks.PERK_PLUSONEHIT);
								perks.Add(MonsterPerks.PERK_POISONTOUCH);
								perks.Add(MonsterPerks.PERK_STUNSLEEPTOUCH);
								perks.Add(MonsterPerks.PERK_MUTETOUCH);
								break;
							case 38: // Gator
								enemyNames[i] = "GATOR";
								enemyStats[i].num_hits = rng.Between(2, 3);
								enemyStats[i].critrate = 1;
								enemyStats[i].damage = rollEnemyStrength(enemyTierList[i], 6);
								enemyStats[i].accuracy = rollEnemyAccuracy(enemyTierList[i], 5);
								enemyStats[i].absorb = rollEnemyAbsorb(enemyTierList[i], 4);
								enemyStats[i].agility = 48;
								enemyStats[i].atk_elem = 0b00000000;
								enemyStats[i].atk_ailment = 0b00000000;
								enemyStats[i].monster_type = 0b00100010;
								enemyStats[i].elem_resist = 0b10010000;
								enemyStats[i].elem_weakness = 0b01000000;
								perks.Add(MonsterPerks.PERK_HIGHRESIST);
								perks.Add(MonsterPerks.PERK_HIGHWEAKNESS);
								perks.Add(MonsterPerks.PERK_PLUSONEHIT);
								perks.Add(MonsterPerks.PERK_POISONTOUCH);
								perks.Add(MonsterPerks.PERK_STUNSLEEPTOUCH);
								perks.Add(MonsterPerks.PERK_MUTETOUCH);
								break;
							case 39: // Hydra
								enemyNames[i] = "HYDRA";
								enemyStats[i].num_hits = 4;
								enemyStats[i].critrate = 1;
								enemyStats[i].damage = rollEnemyStrength(enemyTierList[i], 4);
								enemyStats[i].accuracy = rollEnemyAccuracy(enemyTierList[i], 5);
								enemyStats[i].absorb = rollEnemyAbsorb(enemyTierList[i], 4);
								enemyStats[i].agility = rollEnemyEvade(enemyTierList[i], 5);
								enemyStats[i].atk_elem = 0b00000000;
								enemyStats[i].atk_ailment = 0b00000000;
								enemyStats[i].monster_type = 0b00000010;
								enemyStats[i].elem_resist = 0b00000000;
								enemyStats[i].elem_weakness = 0b00000000;
								perks.Add(MonsterPerks.PERK_LOWRESIST);
								perks.Add(MonsterPerks.PERK_HIGHRESIST);
								perks.Add(MonsterPerks.PERK_LOWWEAKNESS);
								perks.Add(MonsterPerks.PERK_HIGHWEAKNESS);
								perks.Add(MonsterPerks.PERK_POISONTOUCH);
								perks.Add(MonsterPerks.PERK_STUNSLEEPTOUCH);
								perks.Add(MonsterPerks.PERK_MUTETOUCH);
								break;
							case 40: // Robot
								enemyNames[i] = "BOT";
								enemyStats[i].num_hits = rng.Between(1, 2);
								enemyStats[i].critrate = rng.Between(1, 5);
								enemyStats[i].damage = enemyStats[i].num_hits == 1 ? rollEnemyStrength(enemyTierList[i], 7) : rollEnemyStrength(enemyTierList[i], 5);
								enemyStats[i].accuracy = rollEnemyAccuracy(enemyTierList[i], 6);
								enemyStats[i].absorb = rollEnemyAbsorb(enemyTierList[i], 7);
								enemyStats[i].agility = rollEnemyEvade(enemyTierList[i], 6);
								enemyStats[i].atk_elem = 0b00000001;
								enemyStats[i].atk_ailment = enemyStats[i].num_hits == 1 ? (byte)0b00000000 : (byte)0b00010000;
								enemyStats[i].monster_type = 0b00000000;
								enemyStats[i].elem_resist = 0b00001011;
								enemyStats[i].elem_weakness = 0b01000000;
								perks.Add(MonsterPerks.PERK_LOWRESIST);
								perks.Add(MonsterPerks.PERK_LOWWEAKNESS);
								perks.Add(MonsterPerks.PERK_PLUSONEHIT);
								perks.Add(MonsterPerks.PERK_POISONTOUCH);
								perks.Add(MonsterPerks.PERK_STUNSLEEPTOUCH);
								perks.Add(MonsterPerks.PERK_MUTETOUCH);
								break;
							case 41: // Naga
								enemyNames[i] = "NAGA";
								enemyStats[i].num_hits = 1;
								enemyStats[i].critrate = 1;
								enemyStats[i].damage = rollEnemyStrength(enemyTierList[i], 1);
								enemyStats[i].accuracy = rollEnemyAccuracy(enemyTierList[i], 7);
								enemyStats[i].absorb = rollEnemyAbsorb(enemyTierList[i], 2);
								enemyStats[i].agility = rollEnemyEvade(enemyTierList[i], 6);
								enemyStats[i].atk_elem = 0b00000010;
								enemyStats[i].atk_ailment = 0b00000100;
								enemyStats[i].monster_type = 0b01000000;
								enemyStats[i].elem_resist = 0b00000000;
								enemyStats[i].elem_weakness = 0b00000000;
								if (enemyStats[i].AIscript == 0xFF)
									enemyStats[i].AIscript = ENE_PickForcedAIScript(enemyTierList[i]);
								perks.Add(MonsterPerks.PERK_LOWRESIST);
								perks.Add(MonsterPerks.PERK_HIGHRESIST);
								perks.Add(MonsterPerks.PERK_LOWWEAKNESS);
								perks.Add(MonsterPerks.PERK_HIGHWEAKNESS);
								perks.Add(MonsterPerks.PERK_PLUSONEHIT);
								break;
							case 42: // Small Elemental
								enemyNames[i] = "AIR";
								if (elementalsSelected.Count > 0)
									elemental = elementalsSelected.PickRandom(rng);
								rollSmallElemental(rng, ref enemyStats[i], enemyTierList[i], elemental);
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
								enemyStats[i].num_hits = 4;
								enemyStats[i].critrate = rng.Between(1, 3);
								enemyStats[i].damage = rollEnemyStrength(enemyTierList[i], 6);
								enemyStats[i].accuracy = rollEnemyAccuracy(enemyTierList[i], 6);
								enemyStats[i].absorb = rollEnemyAbsorb(enemyTierList[i], 4);
								enemyStats[i].agility = rollEnemyEvade(enemyTierList[i], 6);
								enemyStats[i].atk_elem = 0b00000000;
								enemyStats[i].atk_ailment = 0b00000000;
								enemyStats[i].monster_type = 0b00000010;
								enemyStats[i].elem_resist = 0b10010000;
								enemyStats[i].elem_weakness = 0b00100000;
								perks.Add(MonsterPerks.PERK_LOWRESIST);
								perks.Add(MonsterPerks.PERK_HIGHRESIST);
								perks.Add(MonsterPerks.PERK_LOWWEAKNESS);
								perks.Add(MonsterPerks.PERK_HIGHWEAKNESS);
								perks.Add(MonsterPerks.PERK_PLUSONEHIT);
								perks.Add(MonsterPerks.PERK_POISONTOUCH);
								perks.Add(MonsterPerks.PERK_STUNSLEEPTOUCH);
								perks.Add(MonsterPerks.PERK_MUTETOUCH);
								break;
							case 44: // Piscodemon
								enemyNames[i] = "SQUID";
								enemyStats[i].num_hits = rng.Between(2, 3);
								enemyStats[i].critrate = 1;
								enemyStats[i].damage = rollEnemyStrength(enemyTierList[i], 5);
								enemyStats[i].accuracy = rollEnemyAccuracy(enemyTierList[i], 4);
								enemyStats[i].absorb = rollEnemyAbsorb(enemyTierList[i], 5);
								enemyStats[i].agility = rollEnemyEvade(enemyTierList[i], 6);
								enemyStats[i].atk_elem = 0b00000000;
								enemyStats[i].atk_ailment = 0b00000000;
								enemyStats[i].monster_type = 0b00000000;
								enemyStats[i].elem_resist = 0b00110011;
								enemyStats[i].elem_weakness = 0b00000000;
								break;
							case 45: // Dragon Type 2
								if (dragonsSelected.Count > 0)
									elemental = dragonsSelected.PickRandom(rng);
								rollDragon(rng, ref enemyStats[i], enemyTierList[i], elemental);
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
								enemyStats[i].num_hits = 1;
								enemyStats[i].critrate = rng.Between(1, 10);
								enemyStats[i].damage = rollEnemyStrength(enemyTierList[i], 5);
								enemyStats[i].accuracy = rollEnemyAccuracy(enemyTierList[i], 5);
								enemyStats[i].absorb = rollEnemyAbsorb(enemyTierList[i], 5);
								enemyStats[i].agility = rollEnemyEvade(enemyTierList[i], 5);
								enemyStats[i].atk_elem = 0b00000000;
								enemyStats[i].atk_ailment = 0b00000000;
								enemyStats[i].monster_type = 0b00000000;
								enemyStats[i].elem_resist = 0b00000000;
								enemyStats[i].elem_weakness = 0b00000000;
								perks.Add(MonsterPerks.PERK_LOWRESIST);
								perks.Add(MonsterPerks.PERK_HIGHRESIST);
								perks.Add(MonsterPerks.PERK_LOWWEAKNESS);
								perks.Add(MonsterPerks.PERK_HIGHWEAKNESS);
								perks.Add(MonsterPerks.PERK_PLUSONEHIT);
								perks.Add(MonsterPerks.PERK_POISONTOUCH);
								perks.Add(MonsterPerks.PERK_STUNSLEEPTOUCH);
								perks.Add(MonsterPerks.PERK_MUTETOUCH);
								break;
							case 47: // Golem
								enemyNames[i] = "GOLEM";
								enemyStats[i].num_hits = 1;
								enemyStats[i].critrate = 1;
								enemyStats[i].damage = rollEnemyStrength(enemyTierList[i], 6);
								enemyStats[i].accuracy = rollEnemyAccuracy(enemyTierList[i], 4);
								enemyStats[i].absorb = rng.Between(7, 60);
								if (enemyStats[i].exp > 4000)
									enemyStats[i].absorb = (enemyStats[i].absorb * 2) + 30;
								enemyStats[i].agility = rollEnemyEvade(enemyTierList[i], 4);
								enemyStats[i].atk_elem = 0b00000000;
								enemyStats[i].atk_ailment = 0b00000000;
								enemyStats[i].monster_type = 0b00000000;
								enemyStats[i].elem_resist = 0b11111011;
								if(enemyStats[i].absorb > 60)
								{
									switch(rng.Between(0, 2))
									{
										case 0:
											enemyStats[i].elem_resist &= 0b10111111;
											break;
										case 1:
											enemyStats[i].elem_resist &= 0b11011111;
											break;
										case 2:
											enemyStats[i].elem_resist &= 0b11101111;
											break;
									}
								}
								enemyStats[i].elem_weakness = 0b00000000;
								break;
							case 48: // Knight Type 2
								enemyNames[i] = "RANGER";
								enemyStats[i].num_hits = rng.Between(1, 2);
								enemyStats[i].critrate = rng.Between(1, 5);
								enemyStats[i].damage = rollEnemyStrength(enemyTierList[i], 6);
								enemyStats[i].accuracy = rollEnemyAccuracy(enemyTierList[i], 6);
								enemyStats[i].absorb = rollEnemyAbsorb(enemyTierList[i], 6);
								enemyStats[i].agility = rollEnemyEvade(enemyTierList[i], 4);
								enemyStats[i].atk_elem = 0b00000000;
								enemyStats[i].atk_ailment = 0b00000000;
								enemyStats[i].monster_type = 0b00000000;
								enemyStats[i].elem_resist = 0b00000000;
								enemyStats[i].elem_weakness = 0b00000000;
								perks.Add(MonsterPerks.PERK_LOWRESIST);
								perks.Add(MonsterPerks.PERK_HIGHRESIST);
								perks.Add(MonsterPerks.PERK_LOWWEAKNESS);
								perks.Add(MonsterPerks.PERK_HIGHWEAKNESS);
								perks.Add(MonsterPerks.PERK_PLUSONEHIT);
								perks.Add(MonsterPerks.PERK_POISONTOUCH);
								perks.Add(MonsterPerks.PERK_STUNSLEEPTOUCH);
								perks.Add(MonsterPerks.PERK_MUTETOUCH);
								break;
							case 49: // Pony
								enemyNames[i] = "PONY";
								enemyStats[i].num_hits = rng.Between(2, 4);
								enemyStats[i].critrate = rng.Between(1, 3);
								enemyStats[i].damage = rollEnemyStrength(enemyTierList[i], 5);
								enemyStats[i].accuracy = rollEnemyAccuracy(enemyTierList[i], 5);
								enemyStats[i].absorb = rollEnemyAbsorb(enemyTierList[i], 4);
								enemyStats[i].agility = rollEnemyEvade(enemyTierList[i], 6);
								enemyStats[i].atk_elem = 0b00000000;
								enemyStats[i].atk_ailment = 0b00000000;
								enemyStats[i].monster_type = 0b00000000;
								enemyStats[i].elem_resist = 0b00000000;
								enemyStats[i].elem_weakness = 0b00000000;
								perks.Add(MonsterPerks.PERK_LOWRESIST);
								perks.Add(MonsterPerks.PERK_HIGHRESIST);
								perks.Add(MonsterPerks.PERK_LOWWEAKNESS);
								perks.Add(MonsterPerks.PERK_HIGHWEAKNESS);
								break;
							case 50: // Elf
								enemyNames[i] = "ELF";
								enemyStats[i].num_hits = 1;
								enemyStats[i].critrate = rng.Between(1, 5);
								enemyStats[i].damage = rollEnemyStrength(enemyTierList[i], 4);
								enemyStats[i].accuracy = rollEnemyAccuracy(enemyTierList[i], 4);
								enemyStats[i].absorb = rollEnemyAbsorb(enemyTierList[i], 6);
								enemyStats[i].agility = rollEnemyEvade(enemyTierList[i], 7);
								enemyStats[i].atk_elem = 0b00000000;
								enemyStats[i].atk_ailment = 0b00000000;
								enemyStats[i].monster_type = 0b01000000;
								enemyStats[i].elem_resist = 0b00000000;
								enemyStats[i].elem_weakness = 0b00000000;
								if (enemyStats[i].AIscript == 0xFF)
									enemyStats[i].AIscript = ENE_PickForcedAIScript(enemyTierList[i]);
								perks.Add(MonsterPerks.PERK_LOWRESIST);
								perks.Add(MonsterPerks.PERK_HIGHRESIST);
								perks.Add(MonsterPerks.PERK_LOWWEAKNESS);
								perks.Add(MonsterPerks.PERK_HIGHWEAKNESS);
								perks.Add(MonsterPerks.PERK_PLUSONEHIT);
								perks.Add(MonsterPerks.PERK_POISONTOUCH);
								perks.Add(MonsterPerks.PERK_STUNSLEEPTOUCH);
								perks.Add(MonsterPerks.PERK_MUTETOUCH);
								break;
							case 51: // War Machine
								enemyNames[i] = "MECH";
								enemyStats[i].num_hits = 2;
								enemyStats[i].critrate = 1;
								enemyStats[i].damage = rollEnemyStrength(enemyTierList[i], 7);
								enemyStats[i].accuracy = rollEnemyAccuracy(enemyTierList[i], 7);
								enemyStats[i].absorb = rollEnemyAbsorb(enemyTierList[i], 7);
								enemyStats[i].agility = rollEnemyEvade(enemyTierList[i], 4);
								enemyStats[i].atk_elem = 0b00000000;
								enemyStats[i].atk_ailment = 0b00000000;
								enemyStats[i].monster_type = 0b10000000;
								enemyStats[i].elem_resist = 0b00111011;
								enemyStats[i].elem_weakness = 0b01000000;
								enemyStats[i].exp *= 9;
								enemyStats[i].exp /= 8;
								break;
						}
						bool hasGlobalPerk = false;
						bool didntChooseVariantClass = true; // false if this monster rolled a variant class and thus doesn't need a name modifier
						// if enemy has a global perk, apply it now and skip all other perks
						if (i == Enemy.Crawl)
						{
							// Global Perk - multi-hitter with weak attacks but stun touch guaranteed (overrides existing atk ailment)
							enemyStats[i].atk_elem = 0b00000001;
							enemyStats[i].atk_ailment = 0b00010000;
							enemyStats[i].num_hits = 8;
							enemyStats[i].damage = 1;
							hasGlobalPerk = true;
						}
						else if(i == Enemy.Coctrice)
						{
							// Global Perk - stonetouch (overrides other atk_elem and ailment), -1 hit if num_hits > 1
							enemyStats[i].atk_elem = 0b00000010;
							enemyStats[i].atk_ailment = 0b00000010;
							if (enemyStats[i].num_hits > 1)
								enemyStats[i].num_hits--;
							hasGlobalPerk = true;
						}
						else if(i == Enemy.Mancat)
						{
							// Global Perk - Mancat (resistance to all elements except time and no weaknesses)
							enemyStats[i].elem_resist = 0b11111011;
							enemyStats[i].elem_weakness = 0b00000000;
							hasGlobalPerk = true;
						}
						else if(i == Enemy.Sorceror)
						{
							// Global Perk - deathtouch
							enemyStats[i].atk_elem = 0b00000000;
							enemyStats[i].atk_ailment = 0b00000001;
							hasGlobalPerk = true;
						}
						else if(i == Enemy.Nitemare)
						{
							// Global Perk - resist all except one low element and time, weak to selected element
							switch(rng.Between(0,2))
							{
								case 0:
									enemyStats[i].elem_resist = 0b10111011;
									enemyStats[i].elem_weakness = 0b01000000;
									break;
								case 1:
									enemyStats[i].elem_resist = 0b11011011;
									enemyStats[i].elem_weakness = 0b00100000;
									break;
								case 2:
									enemyStats[i].elem_resist = 0b11101011;
									enemyStats[i].elem_weakness = 0b00010000;
									break;
							}
						}
						else
						{
							// else, roll the chance for an enemy to get a class modifier.  Each class of monster can have Frost, Red, Zombie, Were, and Wizard variants, though some monster classes can't use some of these
							// this perk restricts the monster from rolling any other perks
							if (rng.Between(0, 11) < monsterClassVariants[enemyImage[i]].Count())
							{
								didntChooseVariantClass = false;
								string classModifier = monsterClassVariants[enemyImage[i]].PickRandom(rng);
								switch (classModifier) // apply the traits of this enemy class
								{
									case "Wz":
										if(enemyStats[i].AIscript == 0xFF)
										{
											enemyStats[i].AIscript = ENE_PickForcedAIScript(enemyTierList[i]);
										}
										break;
									case "Fr":
										enemyStats[i].elem_resist |= 0b00100000;
										enemyStats[i].elem_resist &= 0b11101111;
										enemyStats[i].elem_weakness |= 0b00010000;
										enemyStats[i].elem_weakness &= 0b11011111;
										enemyStats[i].atk_elem = 0b00100000;
										break;
									case "R.":
										enemyStats[i].elem_resist |= 0b00010000;
										enemyStats[i].elem_resist &= 0b11011111;
										enemyStats[i].elem_weakness |= 0b00100000;
										enemyStats[i].elem_weakness &= 0b11101111;
										enemyStats[i].atk_elem = 0b00010000;
										break;
									case "Z.":
										enemyStats[i].elem_resist |= 0b00101011;
										enemyStats[i].elem_resist &= 0b11101111;
										enemyStats[i].elem_weakness |= 0b00010000;
										enemyStats[i].elem_weakness &= 0b11010100;
										enemyStats[i].monster_type |= 0b00001000;
										if(enemyStats[i].atk_ailment == 0)
										{
											enemyStats[i].atk_ailment = 0b00010000;
											enemyStats[i].atk_elem = 0b00000001;
										}
										break;
									case "Sea":
										enemyStats[i].elem_resist |= 0b10010000;
										enemyStats[i].elem_resist &= 0b10111111;
										enemyStats[i].elem_weakness |= 0b01000000;
										enemyStats[i].elem_weakness &= 0b01101111;
										enemyStats[i].monster_type |= 0b00100000;
										break;
									case "Wr":
										enemyStats[i].atk_ailment = 0b00000100;
										enemyStats[i].atk_elem = 0b00000010;
										enemyStats[i].monster_type |= 0b00010000;
										break;
								}
								enemyNames[i] = classModifier + enemyNames[i]; // change the enemy's name
								monsterClassVariants[enemyImage[i]].Remove(classModifier); // remove this variant from the list of variants available for this enemy image
							}
						}
						if(didntChooseVariantClass)
						{
							// else, roll minor perks.  there is a 75% chance to gain a perk, then 62.5%, 50%, and so on.  monsters cannot gain more than 4 perks
							int num_perks = 0;
							if(!hasGlobalPerk)
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
													enemyStats[i].hp *= 11;
													enemyStats[i].hp /= 10;
													break;
												case 1:
													enemyStats[i].damage *= 11;
													enemyStats[i].damage /= 10;
													break;
												case 2:
													enemyStats[i].absorb *= 11;
													enemyStats[i].absorb /= 10;
													break;
											}
											enemyStats[i].exp *= 103;
											enemyStats[i].exp /= 100;
											break;
										case MonsterPerks.PERK_LOSESTAT10: // pick a random stat to decrease by 10%, -3% XP
											switch (rng.Between(0, 2))
											{
												case 0:
													enemyStats[i].hp *= 9;
													enemyStats[i].hp /= 10;
													break;
												case 1:
													enemyStats[i].damage *= 9;
													enemyStats[i].damage /= 10;
													break;
												case 2:
													enemyStats[i].absorb *= 9;
													enemyStats[i].absorb /= 10;
													break;
											}
											enemyStats[i].exp *= 97;
											enemyStats[i].exp /= 100;
											break;
										case MonsterPerks.PERK_LOWRESIST: // pick a low resist or remove the corresponding weakness, +4% XP
											switch (rng.Between(0, 3))
											{
												case 0:
													if ((enemyStats[i].elem_weakness & 0b10000000) == 0b10000000)
														enemyStats[i].elem_weakness &= 0b01111111;
													else
														enemyStats[i].elem_resist |= 0b10000000;
													break;
												case 1:
													if ((enemyStats[i].elem_weakness & 0b01000000) == 0b01000000)
														enemyStats[i].elem_weakness &= 0b10111111;
													else
														enemyStats[i].elem_resist |= 0b01000000;
													break;
												case 2:
													if ((enemyStats[i].elem_weakness & 0b00100000) == 0b00100000)
														enemyStats[i].elem_weakness &= 0b11011111;
													else
														enemyStats[i].elem_resist |= 0b00100000;
													break;
												case 3:
													if ((enemyStats[i].elem_weakness & 0b00010000) == 0b00001000)
														enemyStats[i].elem_weakness &= 0b11101111;
													else
														enemyStats[i].elem_resist |= 0b00010000;
													break;
											}
											enemyStats[i].exp *= 104;
											enemyStats[i].exp /= 100;
											break;
										case MonsterPerks.PERK_LOWWEAKNESS: // pick a low weakness (or cancel a resist, or ignore for earth resist), -5% XP
											switch (rng.Between(0, 3))
											{
												case 0:
													if ((enemyStats[i].elem_resist & 0b10000000) == 0b10000000)
														didPerkRoll = false;
													else
														enemyStats[i].elem_weakness |= 0b10000000;
													break;
												case 1:
													if ((enemyStats[i].elem_resist & 0b01000000) == 0b01000000)
														enemyStats[i].elem_resist &= 0b10111111;
													else
														enemyStats[i].elem_weakness |= 0b01000000;
													break;
												case 2:
													if ((enemyStats[i].elem_resist & 0b00100000) == 0b00100000)
														enemyStats[i].elem_resist &= 0b11011111;
													else
														enemyStats[i].elem_weakness |= 0b00100000;
													break;
												case 3:
													if ((enemyStats[i].elem_resist & 0b00010000) == 0b00010000)
														enemyStats[i].elem_resist &= 0b11101111;
													else
														enemyStats[i].elem_weakness |= 0b00010000;
													break;
											}
											if (didPerkRoll)
											{
												enemyStats[i].exp *= 95;
												enemyStats[i].exp /= 100;
											}
											break;
										case MonsterPerks.PERK_HIGHRESIST:  // pick a high resist and remove the corresponding weakness, +3% XP
											switch (rng.Between(0, 3))
											{
												case 0:
													enemyStats[i].elem_resist |= 0b00001000;
													enemyStats[i].elem_weakness &= 0b11110111;
													break;
												case 1:
													enemyStats[i].elem_resist |= 0b00000100;
													enemyStats[i].elem_weakness &= 0b11111011;
													break;
												case 2:
													enemyStats[i].elem_resist |= 0b00000010;
													enemyStats[i].elem_weakness &= 0b11111101;
													break;
												case 3:
													enemyStats[i].elem_resist |= 0b00000001;
													enemyStats[i].elem_weakness &= 0b11111110;
													break;
											}
											enemyStats[i].exp *= 103;
											enemyStats[i].exp /= 100;
											break;
										case MonsterPerks.PERK_HIGHWEAKNESS:
											switch (rng.Between(0, 3))
											{
												case 0:
													if ((enemyStats[i].elem_resist & 0b00001000) == 0b00001000)
														enemyStats[i].elem_resist &= 0b11110111;
													else
														enemyStats[i].elem_weakness |= 0b00001000;
													break;
												case 1:
													if ((enemyStats[i].elem_resist & 0b00000100) == 0b00000100)
														enemyStats[i].elem_resist &= 0b11111011;
													else
														enemyStats[i].elem_weakness |= 0b00000100;
													break;
												case 2:
													if ((enemyStats[i].elem_resist & 0b00000010) == 0b00000010)
														enemyStats[i].elem_resist &= 0b11111101;
													else
														enemyStats[i].elem_weakness |= 0b00000010;
													break;
												case 3:
													if ((enemyStats[i].elem_resist & 0b00000001) == 0b00000001)
														enemyStats[i].elem_resist &= 0b11111110;
													else
														enemyStats[i].elem_weakness |= 0b00000001;
													break;
											}
											if (didPerkRoll)
											{
												enemyStats[i].exp *= 97;
												enemyStats[i].exp /= 100;
											}
											break;
										case MonsterPerks.PERK_PLUSONEHIT: // +1 hit, +5/3/2/1... XP for an addition hit
											enemyStats[i].num_hits++;
											switch (enemyStats[i].num_hits)
											{
												case 2:
													enemyStats[i].exp *= 21;
													enemyStats[i].exp /= 20;
													break;
												case 3:
													enemyStats[i].exp *= 103;
													enemyStats[i].exp /= 100;
													break;
												case 4:
													enemyStats[i].exp *= 51;
													enemyStats[i].exp /= 50;
													break;
												default:
													enemyStats[i].exp *= 101;
													enemyStats[i].exp /= 100;
													break;
											}
											break;
										case MonsterPerks.PERK_POISONTOUCH: // adds poisontouch if enemy has no atk_ailment already, no XP increase
											if (enemyStats[i].atk_ailment == 0)
											{
												enemyStats[i].atk_ailment = 0b00000100;
												enemyStats[i].atk_elem = 0b00000010;
											}
											break;
										case MonsterPerks.PERK_STUNSLEEPTOUCH: // adds stun or sleep touch if enemy has no atk_ailment already
											if (enemyStats[i].atk_ailment == 0)
											{
												enemyStats[i].atk_ailment = rng.Between(0, 1) == 0 ? (byte)0b00100000 : (byte)0b00010000;
												enemyStats[i].atk_elem = 0b00000001;
												enemyStats[i].exp *= 103;
												enemyStats[i].exp /= 100;
											}
											break;
										case MonsterPerks.PERK_MUTETOUCH: // adds mute touch if enemy has no atk_ailment already
											if (enemyStats[i].atk_ailment == 0)
											{
												enemyStats[i].atk_ailment = 0b01000000;
												enemyStats[i].atk_elem = 0b00000001;
												enemyStats[i].exp *= 51;
												enemyStats[i].exp /= 50;
											}
											break;
									}
									num_perks++;
								}
							}
							// if enemy's vanilla name has already been used, add a name modifier
							if(elemental == 9) // don't make alternate names for elementals
							{
								if (monsterBaseNameUsed[enemyImage[i]])
								{
									string nameModifier = monsterNameVariants[enemyImage[i]].PickRandom(rng);
									enemyNames[i] = nameModifier + enemyNames[i];
								}
								else
									monsterBaseNameUsed[enemyImage[i]] = true;
							}
						}
						if (enemyStats[i].AIscript != 0xFF) // set monster type to Mage if it has a script
							enemyStats[i].monster_type |= 0b01000000;
						for(int j = 1; j < enemyStats[i].num_hits; ++j) // for each hit past the first, reduce the base damage by one for every 15 points of damage rating
						{
							enemyStats[i].damage = enemyStats[i].damage - enemyStats[i].damage / 15;
						}
						enemyStats[i].damage = rng.Between(enemyStats[i].damage - enemyStats[i].damage / 25, enemyStats[i].damage + enemyStats[i].damage / 25); // variance for damage rating
						enemyStats[i].hp = rng.Between(enemyStats[i].hp - enemyStats[i].hp / 30, enemyStats[i].hp + enemyStats[i].hp / 30); // variance for hp rating
						enemyStats[i].gp = rng.Between(enemyStats[i].gp - enemyStats[i].gp / 20, enemyStats[i].gp + enemyStats[i].gp / 20); // variance for gp reward
						enemyStats[i].exp = rng.Between(enemyStats[i].exp - enemyStats[i].exp / 40, enemyStats[i].exp + enemyStats[i].exp / 40); // variance for exp reward
					}
				}
				// remove palettes from tilesets where there are no mons using those palettes
				for(int i = 0; i < GenericTilesetsCount; ++i)
				{
					List<byte> palRemoveList = new List<byte> { };
					foreach(byte pal in palettesInTileset[i])
					{
						bool nopalettematch = true;
						foreach(byte mon in enemiesInTileset[i])
						{
							if(enemyPalettes[mon] == pal)
							{
								nopalettematch = false;
								break;
							}
						}
						if(nopalettematch)
						{
							palRemoveList.Add(pal);
						}
					}
					foreach(byte pal in palRemoveList)
					{
						palettesInTileset[i].Remove(pal);
					}
				}
				// modify scripts - all scripts that are skills-only gain a magic list full of spells in the same tier (with 24/128 chance of activating)
				// each spell is then selected from a list of spells available for that tier.  it is not possible to promote to a higher tier
				// skills will remain the same
				for(int i = 0; i < ScriptCount - 10; ++i) // exclude the last 10 scripts
				{
					if(enemyScripts[i].spell_chance == 0)
					{
						byte whichSpell = 0; // index for CURE, a tier 1 enemy spell
						switch (ENE_GetEnemySkillTier(enemyScripts[i].skill_list[0]))
						{
							case 2:
								whichSpell = 5; // these are just spell indices of spells that correspond to the tier, we will replace them later
								break;
							case 3:
								whichSpell = 21;
								break;
							case 4:
								whichSpell = 37;
								break;
							case 5:
								whichSpell = 0xFF; // if a tier 5 is encountered, we don't fill the spell list (the skill is nasty enough!) - by default this only applies to warmech
								break;
						}
						for(byte j = 0; j < 8; ++j)
						{
							enemyScripts[i].spell_list[j] = whichSpell;
						}
						if(whichSpell != 0xFF)
							enemyScripts[i].spell_chance = 24;
					}
					// start replacing each spell with another spell from the same tier
					for(byte j = 0; j < 8; ++j)
					{
						if (enemyScripts[i].spell_list[j] == 0xFF)
							continue; // skip blank spells
						int whichTier = ENE_GetEnemySpellTier(enemyScripts[i].spell_list[j]);
						List<byte> eligibleSpellIDs = new List<byte> { };
						for(byte k = 0; k < 64; ++k)
						{
							if (ENE_GetEnemySpellTier(k) == whichTier)
								eligibleSpellIDs.Add(k);
						}
						enemyScripts[i].spell_list[j] = eligibleSpellIDs.PickRandom(rng);
					}
				}

				num_enemy_name_chars = 0;
				for (int i = 0; i < EnemyCount; ++i)
				{
					num_enemy_name_chars += enemyNames[i].Length;
				}
				Console.WriteLine(num_enemy_name_chars);
				return true;
			}

			public int rollEnemyStrength(int tier, int level)
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

			public int rollEnemyAccuracy(int tier, int level)
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

			public int rollEnemyAbsorb(int tier, int level)
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

			public int rollEnemyEvade(int tier, int level)
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

			public void rollLargeElemental(MT19337 rng, ref EnemyData thisEnemy, int tier, int element)
			{
				switch(element)
				{
					case 1: // Earth Elemental
						thisEnemy.num_hits = 1;
						thisEnemy.critrate = 1;
						thisEnemy.damage = rollEnemyStrength(tier, 7);
						thisEnemy.accuracy = rollEnemyAccuracy(tier, 7);
						thisEnemy.absorb = rollEnemyAbsorb(tier, 5);
						thisEnemy.agility = rollEnemyEvade(tier, 2);
						thisEnemy.atk_elem = 0b10000000;
						thisEnemy.atk_ailment = 0b00000000;
						thisEnemy.monster_type = 0b00000001;
						thisEnemy.elem_resist = 0b11101011;
						thisEnemy.elem_weakness = 0b00010000;
						break;
					case 2: // Lightning Elemental
						thisEnemy.num_hits = 1;
						thisEnemy.critrate = 20;
						thisEnemy.damage = rollEnemyStrength(tier, 5);
						thisEnemy.accuracy = rollEnemyAccuracy(tier, 5);
						thisEnemy.absorb = rollEnemyAbsorb(tier, 2);
						thisEnemy.agility = rollEnemyEvade(tier, 6);
						thisEnemy.atk_elem = 0b01000000;
						thisEnemy.atk_ailment = 0b00100000;
						thisEnemy.monster_type = 0b00000001;
						thisEnemy.elem_resist = 0b11011001;
						thisEnemy.elem_weakness = 0b00000110;
						break;
					case 3: // Ice Elemental
						thisEnemy.num_hits = 1;
						thisEnemy.critrate = 1;
						thisEnemy.damage = rollEnemyStrength(tier, 5);
						thisEnemy.accuracy = rollEnemyAccuracy(tier, 7);
						thisEnemy.absorb = rollEnemyAbsorb(tier, 6);
						thisEnemy.agility = rollEnemyEvade(tier, 2);
						thisEnemy.atk_elem = 0b00100000;
						thisEnemy.atk_ailment = 0b00010000;
						thisEnemy.monster_type = 0b00000001;
						thisEnemy.elem_resist = 0b11101011;
						thisEnemy.elem_weakness = 0b00010000;
						break;
					case 4: // Fire Elemental
						thisEnemy.num_hits = 1;
						thisEnemy.critrate = 1;
						thisEnemy.damage = rollEnemyStrength(tier, 6);
						thisEnemy.accuracy = rollEnemyAccuracy(tier, 7);
						thisEnemy.absorb = rollEnemyAbsorb(tier, 5);
						thisEnemy.agility = rollEnemyEvade(tier, 2);
						thisEnemy.atk_elem = 0b00010000;
						thisEnemy.atk_ailment = 0b00000000;
						thisEnemy.monster_type = 0b00000001;
						thisEnemy.elem_resist = 0b11011011;
						thisEnemy.elem_weakness = 0b00100000;
						break;
					case 5: // Death Elemental
						thisEnemy.num_hits = 2;
						thisEnemy.critrate = 1;
						thisEnemy.damage = rollEnemyStrength(tier, 7);
						thisEnemy.accuracy = rollEnemyAccuracy(tier, 4);
						thisEnemy.absorb = rollEnemyAbsorb(tier, 4);
						thisEnemy.agility = rollEnemyEvade(tier, 4);
						thisEnemy.atk_elem = 0b00001000;
						thisEnemy.atk_ailment = 0b00001000;
						thisEnemy.monster_type = 0b00001001;
						thisEnemy.elem_resist = 0b10101011;
						thisEnemy.elem_weakness = 0b00010000;
						break;
					case 6: // Time Elemental
						thisEnemy.num_hits = rng.Between(4, 6);
						thisEnemy.critrate = 1;
						thisEnemy.damage = rollEnemyStrength(tier, 4);
						thisEnemy.accuracy = rollEnemyAccuracy(tier, 4);
						thisEnemy.absorb = rollEnemyAbsorb(tier, 4);
						thisEnemy.agility = rollEnemyEvade(tier, 5);
						thisEnemy.atk_elem = 0b00000100;
						thisEnemy.atk_ailment = 0b00010000;
						thisEnemy.monster_type = 0b00000001;
						thisEnemy.elem_resist = 0b10001111;
						thisEnemy.elem_weakness = 0b01110000;
						break;
					case 7: // Poison Elemental
						thisEnemy.num_hits = 1;
						thisEnemy.critrate = 1;
						thisEnemy.damage = rollEnemyStrength(tier, 6);
						thisEnemy.accuracy = rollEnemyAccuracy(tier, 7);
						thisEnemy.absorb = rollEnemyAbsorb(tier, 2);
						thisEnemy.agility = rollEnemyEvade(tier, 6);
						thisEnemy.atk_elem = 0b00000010;
						thisEnemy.atk_ailment = 0b00000100;
						thisEnemy.monster_type = 0b10000001;
						thisEnemy.elem_resist = 0b10001011;
						thisEnemy.elem_weakness = 0b00000000;
						break;
					case 8: // Status Elemental
						thisEnemy.num_hits = 1;
						thisEnemy.critrate = 3;
						thisEnemy.damage = rollEnemyStrength(tier, 4);
						thisEnemy.accuracy = rollEnemyAccuracy(tier, 7);
						thisEnemy.absorb = rollEnemyAbsorb(tier, 5);
						thisEnemy.agility = rollEnemyEvade(tier, 5);
						thisEnemy.atk_elem = 0b00000001;
						thisEnemy.atk_ailment = 0b01000000;
						thisEnemy.monster_type = 0b00000001;
						thisEnemy.elem_resist = 0b11111011;
						thisEnemy.elem_weakness = 0b00000000;
						if (thisEnemy.AIscript == 0xFF)
						{
							thisEnemy.AIscript = ENE_PickForcedAIScript(tier);
						}
						break;
					default: // Raw Mana Elemental
						thisEnemy.num_hits = 1;
						thisEnemy.critrate = 1;
						thisEnemy.damage = rollEnemyStrength(tier, 5);
						thisEnemy.accuracy = rollEnemyAccuracy(tier, 5);
						thisEnemy.absorb = rollEnemyAbsorb(tier, 5);
						thisEnemy.agility = rollEnemyEvade(tier, 2);
						thisEnemy.atk_elem = 0b00000000;
						thisEnemy.atk_ailment = 0b00000000;
						thisEnemy.monster_type = 0b00000001;
						thisEnemy.elem_resist = 0b11111011;
						thisEnemy.elem_weakness = 0b00000000;
						break;
				}
			}

			public void rollSmallElemental(MT19337 rng, ref EnemyData thisEnemy, int tier, int element)
			{
				switch (element)
				{
					case 1: // Earth Elemental
						thisEnemy.num_hits = 1;
						thisEnemy.critrate = 1;
						thisEnemy.damage = rollEnemyStrength(tier, 6);
						thisEnemy.accuracy = rollEnemyAccuracy(tier, 7);
						thisEnemy.absorb = rollEnemyAbsorb(tier, 5);
						thisEnemy.agility = rollEnemyEvade(tier, 2);
						thisEnemy.atk_elem = 0b10000000;
						thisEnemy.atk_ailment = 0b00000000;
						thisEnemy.monster_type = 0b00000001;
						thisEnemy.elem_resist = 0b10101011;
						thisEnemy.elem_weakness = 0b01010000;
						break;
					case 2: // Lightning Elemental
						thisEnemy.num_hits = 1;
						thisEnemy.critrate = 10;
						thisEnemy.damage = rollEnemyStrength(tier, 5);
						thisEnemy.accuracy = rollEnemyAccuracy(tier, 5);
						thisEnemy.absorb = rollEnemyAbsorb(tier, 2);
						thisEnemy.agility = rollEnemyEvade(tier, 6);
						thisEnemy.atk_elem = 0b01000000;
						thisEnemy.atk_ailment = 0b00100000;
						thisEnemy.monster_type = 0b00000001;
						thisEnemy.elem_resist = 0b11011001;
						thisEnemy.elem_weakness = 0b00000110;
						break;
					case 3: // Water Elemental
						thisEnemy.num_hits = 1;
						thisEnemy.critrate = 1;
						thisEnemy.damage = rollEnemyStrength(tier, 6);
						thisEnemy.accuracy = rollEnemyAccuracy(tier, 6);
						thisEnemy.absorb = rollEnemyAbsorb(tier, 4);
						thisEnemy.agility = rollEnemyEvade(tier, 5);
						thisEnemy.atk_elem = 0b00100000;
						thisEnemy.atk_ailment = 0b00000000;
						thisEnemy.monster_type = 0b00000001;
						thisEnemy.elem_resist = 0b10011011;
						thisEnemy.elem_weakness = 0b00100000;
						break;
					case 4: // Fire Elemental
						thisEnemy.num_hits = 1;
						thisEnemy.critrate = 1;
						thisEnemy.damage = rollEnemyStrength(tier, 5);
						thisEnemy.accuracy = rollEnemyAccuracy(tier, 6);
						thisEnemy.absorb = rollEnemyAbsorb(tier, 3);
						thisEnemy.agility = rollEnemyEvade(tier, 6);
						thisEnemy.atk_elem = 0b00010000;
						thisEnemy.atk_ailment = 0b00000000;
						thisEnemy.monster_type = 0b00000001;
						thisEnemy.elem_resist = 0b10011001;
						thisEnemy.elem_weakness = 0b00100000;
						break;
					case 5: // Death Elemental
						thisEnemy.num_hits = 2;
						thisEnemy.critrate = 1;
						thisEnemy.damage = rollEnemyStrength(tier, 7);
						thisEnemy.accuracy = rollEnemyAccuracy(tier, 4);
						thisEnemy.absorb = rollEnemyAbsorb(tier, 4);
						thisEnemy.agility = rollEnemyEvade(tier, 4);
						thisEnemy.atk_elem = 0b00001000;
						thisEnemy.atk_ailment = 0b00001000;
						thisEnemy.monster_type = 0b00001001;
						thisEnemy.elem_resist = 0b10101011;
						thisEnemy.elem_weakness = 0b00010000;
						break;
					case 6: // Time Elemental
						thisEnemy.num_hits = rng.Between(2, 5);
						thisEnemy.critrate = 1;
						thisEnemy.damage = rollEnemyStrength(tier, 4);
						thisEnemy.accuracy = rollEnemyAccuracy(tier, 4);
						thisEnemy.absorb = rollEnemyAbsorb(tier, 4);
						thisEnemy.agility = rollEnemyEvade(tier, 5);
						thisEnemy.atk_elem = 0b00000100;
						thisEnemy.atk_ailment = 0b00010000;
						thisEnemy.monster_type = 0b00000001;
						thisEnemy.elem_resist = 0b10001111;
						thisEnemy.elem_weakness = 0b01110000;
						break;
					case 7: // Poison Elemental
						thisEnemy.num_hits = 1;
						thisEnemy.critrate = 1;
						thisEnemy.damage = rollEnemyStrength(tier, 6);
						thisEnemy.accuracy = rollEnemyAccuracy(tier, 7);
						thisEnemy.absorb = rollEnemyAbsorb(tier, 2);
						thisEnemy.agility = rollEnemyEvade(tier, 6);
						thisEnemy.atk_elem = 0b00000010;
						thisEnemy.atk_ailment = 0b00100000;
						thisEnemy.monster_type = 0b10000001;
						thisEnemy.elem_resist = 0b10001011;
						thisEnemy.elem_weakness = 0b00000100;
						break;
					case 8: // Status Elemental
						thisEnemy.num_hits = 2;
						thisEnemy.critrate = 3;
						thisEnemy.damage = rollEnemyStrength(tier, 4);
						thisEnemy.accuracy = rollEnemyAccuracy(tier, 4);
						thisEnemy.absorb = rollEnemyAbsorb(tier, 4);
						thisEnemy.agility = rollEnemyEvade(tier, 4);
						thisEnemy.atk_elem = 0b00000001;
						thisEnemy.atk_ailment = 0b01000000;
						thisEnemy.monster_type = 0b00000001;
						thisEnemy.elem_resist = 0b11111011;
						thisEnemy.elem_weakness = 0b00000000;
						if(thisEnemy.AIscript == 0xFF)
						{
							thisEnemy.AIscript = ENE_PickForcedAIScript(tier);
						}
						break;
					default: // Air Elemental
						thisEnemy.num_hits = 1;
						thisEnemy.critrate = 1;
						thisEnemy.damage = rollEnemyStrength(tier, 5);
						thisEnemy.accuracy = rollEnemyAccuracy(tier, 6);
						thisEnemy.absorb = rollEnemyAbsorb(tier, 2);
						thisEnemy.agility = rollEnemyEvade(tier, 7);
						thisEnemy.atk_elem = 0b00000000;
						thisEnemy.atk_ailment = 0b00000000;
						thisEnemy.monster_type = 0b00000001;
						thisEnemy.elem_resist = 0b10001011;
						thisEnemy.elem_weakness = 0b00000000;
						break;
				}
			}

			public void rollDragon(MT19337 rng, ref EnemyData thisEnemy, int tier, int element)
			{
				switch (element)
				{
					case 1: // Earth Elemental
						thisEnemy.num_hits = 1;
						thisEnemy.critrate = 1;
						thisEnemy.damage = rollEnemyStrength(tier, 6);
						thisEnemy.accuracy = rollEnemyAccuracy(tier, 4);
						thisEnemy.absorb = rollEnemyAbsorb(tier, 7);
						thisEnemy.agility = rollEnemyEvade(tier, 1);
						thisEnemy.atk_elem = 0b10000000;
						thisEnemy.atk_ailment = 0b00000000;
						thisEnemy.monster_type = 0b00000010;
						thisEnemy.elem_resist = 0b11100000;
						thisEnemy.elem_weakness = 0b00000010;
						break;
					case 2: // Lightning Elemental
						thisEnemy.num_hits = 1;
						thisEnemy.critrate = 5;
						thisEnemy.damage = rollEnemyStrength(tier, 7);
						thisEnemy.accuracy = rollEnemyAccuracy(tier, 6);
						thisEnemy.absorb = rollEnemyAbsorb(tier, 3);
						thisEnemy.agility = rollEnemyEvade(tier, 6);
						thisEnemy.atk_elem = 0b01000000;
						thisEnemy.atk_ailment = 0b00000000;
						thisEnemy.monster_type = 0b00000010;
						thisEnemy.elem_resist = 0b11000000;
						thisEnemy.elem_weakness = 0b00010000;
						break;
					case 3: // Ice Elemental
						thisEnemy.num_hits = 1;
						thisEnemy.critrate = 1;
						thisEnemy.damage = rollEnemyStrength(tier, 5);
						thisEnemy.accuracy = rollEnemyAccuracy(tier, 6);
						thisEnemy.absorb = rollEnemyAbsorb(tier, 2);
						thisEnemy.agility = rollEnemyEvade(tier, 7);
						thisEnemy.atk_elem = 0b00100000;
						thisEnemy.atk_ailment = 0b00000000;
						thisEnemy.monster_type = 0b00000010;
						thisEnemy.elem_resist = 0b10100010;
						thisEnemy.elem_weakness = 0b01010000;
						break;
					case 4: // Fire Elemental
						thisEnemy.num_hits = 1;
						thisEnemy.critrate = 1;
						thisEnemy.damage = rollEnemyStrength(tier, 7);
						thisEnemy.accuracy = rollEnemyAccuracy(tier, 7);
						thisEnemy.absorb = rollEnemyAbsorb(tier, 5);
						thisEnemy.agility = rollEnemyEvade(tier, 6);
						thisEnemy.atk_elem = 0b00010000;
						thisEnemy.atk_ailment = 0b00000000;
						thisEnemy.monster_type = 0b00000010;
						thisEnemy.elem_resist = 0b10010000;
						thisEnemy.elem_weakness = 0b00100010;
						break;
					case 5: // Death Elemental
						thisEnemy.num_hits = 1;
						thisEnemy.critrate = 1;
						thisEnemy.damage = rollEnemyStrength(tier, 6);
						thisEnemy.accuracy = rollEnemyAccuracy(tier, 6);
						thisEnemy.absorb = rollEnemyAbsorb(tier, 6);
						thisEnemy.agility = rollEnemyEvade(tier, 3);
						thisEnemy.atk_elem = 0b00001000;
						thisEnemy.atk_ailment = 0b00010000;
						thisEnemy.monster_type = 0b00001010;
						thisEnemy.elem_resist = 0b10101011;
						thisEnemy.elem_weakness = 0b00010000;
						break;
					case 6: // Time Elemental
						thisEnemy.num_hits = 1;
						thisEnemy.critrate = 12;
						thisEnemy.damage = rollEnemyStrength(tier, 6);
						thisEnemy.accuracy = rollEnemyAccuracy(tier, 7);
						thisEnemy.absorb = rollEnemyAbsorb(tier, 6);
						thisEnemy.agility = rollEnemyEvade(tier, 6);
						thisEnemy.atk_elem = 0b00000100;
						thisEnemy.atk_ailment = 0b00010000;
						thisEnemy.monster_type = 0b00000011;
						thisEnemy.elem_resist = 0b10001111;
						thisEnemy.elem_weakness = 0b01110000;
						break;
					case 7: // Poison Elemental
						thisEnemy.num_hits = 1;
						thisEnemy.critrate = 1;
						thisEnemy.damage = rollEnemyStrength(tier, 6);
						thisEnemy.accuracy = rollEnemyAccuracy(tier, 5);
						thisEnemy.absorb = rollEnemyAbsorb(tier, 2);
						thisEnemy.agility = rollEnemyEvade(tier, 6);
						thisEnemy.atk_elem = 0b00000010;
						thisEnemy.atk_ailment = 0b00000000;
						thisEnemy.monster_type = 0b00000010;
						thisEnemy.elem_resist = 0b10000000;
						thisEnemy.elem_weakness = 0b00100000;
						break;
					case 8: // Status Elemental
						thisEnemy.num_hits = 1;
						thisEnemy.critrate = 3;
						thisEnemy.damage = rollEnemyStrength(tier, 4);
						thisEnemy.accuracy = rollEnemyAccuracy(tier, 5);
						thisEnemy.absorb = rollEnemyAbsorb(tier, 5);
						thisEnemy.agility = rollEnemyEvade(tier, 5);
						thisEnemy.atk_elem = 0b00000001;
						thisEnemy.atk_ailment = 0b01000000;
						thisEnemy.monster_type = 0b00000011;
						thisEnemy.elem_resist = 0b01110001;
						thisEnemy.elem_weakness = 0b00000000;
						if (thisEnemy.AIscript == 0xFF)
						{
							thisEnemy.AIscript = ENE_PickForcedAIScript(tier);
						}
						break;
					default: // Standard Dragon
						thisEnemy.num_hits = 1;
						thisEnemy.critrate = 1;
						thisEnemy.damage = rollEnemyStrength(tier, 6);
						thisEnemy.accuracy = rollEnemyAccuracy(tier, 6);
						thisEnemy.absorb = rollEnemyAbsorb(tier, 6);
						thisEnemy.agility = rollEnemyEvade(tier, 6);
						thisEnemy.atk_elem = 0b00000000;
						thisEnemy.atk_ailment = 0b00000000;
						thisEnemy.monster_type = 0b00000010;
						thisEnemy.elem_resist = 0b00000000;
						thisEnemy.elem_weakness = 0b00000000;
						break;
				}
			}

			public byte ENE_PickForcedAIScript(int tier)
			{
				if (tier >= 0 && tier <= 2)
					return 0x1A; // tier 0-2 uses RockGOL script
				else if (tier >= 3 && tier <= 4)
					return 0x0D; // tier 3-4 uses R.GOYLE script
				else if (tier == 5)
					return 0x19; // tier 5 uses MudGOL script
				else if (tier >= 6 && tier <= 7)
					return 0x1D; // tier 6-7 uses MAGE script
				else if (tier >= 8 && tier <= 9)
					return 0x1C; // tier 8-9 uses EVILMAN script
				else
					return 0xFF; // invalid tier returns no script
			}

			public int ENE_GetEnemySkillTier(byte skill)
			{
				if (skill >= 64)
					return 0;
				return skilltiers_enemy[skill];
			}

			public int ENE_GetEnemySpellTier(byte spell)
			{
				if (spell >= 64)
					return 0;
				return spelltiers_enemy[spell];
			}

			public bool DoFormations(MT19337 rng, bool didEnemies)
			{
				// sort enemies into zones
				foreach(byte mon in uniqueEnemyIDs) // only sort through generic enemies that appear in uniqueEnemyIDs
				{
					int limit = Large(mon) ? 4 : 9;
					for(int i = 0; i < zonecountmin.Length; ++i)
					{
						if(enemyStats[mon].exp <= zonexpreqs[i, 2] && enemyStats[mon].exp * limit >= zonexpreqs[i, 0])
						{
							enemyZones[mon].Add(i);
							zonemons[i].Add(mon);
						}
					}
				}
				// roll special encounters for: imp, phantom, warmech, vampire, astos, pirates, garland (and ignore fiend encounters
				ENF_DrawBumFight(rng, 0x00);
				ENF_DrawForcedSingleFight(rng, 3, 0x01); // this is the wizard fight
				ENF_DrawForcedSingleFight(rng, 6, 0x02); // this is the eye fight
				ENF_DrawForcedSingleFight(rng, 6, 0x03); // this is the zombieD fight
				ENF_DrawBossEncounter(rng, Enemy.Phantom, 1, 0x02, true, 4, 0x04);
				ENF_DrawBossEncounter(rng, ENF_PickAVamp(rng), 1, 0x02, true, 4, 0x7C); // vampire fight tries to pick a vampire, or any monster < 3000 xp (yes, this can mean fighting a literal Bum!)
				ENF_DrawBossEncounter(rng, Enemy.Astos, 1, 0x02, true, 4, 0x7D);
				ENF_DrawBossEncounter(rng, Enemy.Pirate, 9, 0x00, true, 4, 0x7E);
				ENF_DrawBossEncounter(rng, Enemy.Garland, 1, 0x02, true, 4, 0x7F);
				ENF_DrawBossEncounter(rng, Enemy.WarMech, 1, 0x02, false, 75, warmech_encounter);
				imp_encounter = 0x80; // resetting important encounter IDs
				wizard_encounter = 0x81;
				eye_encounter = 0x82;
				zombieD_encounter = 0x83;
				phantom_encounter = 0x04;
				// reserve so many slots for different surprise rates and unrunnables
				int[,] surprisetiers = { { 10, 19 }, { 24, 36 }, { 48, 62 }, { 70, 100 } };
				const int surp1 = 9;
				const int surp2 = 12;
				const int surp3 = 4;
				const int surp4 = 2;
				int[] surpcount = { surp1, surp2, surp3, surp4 };
				const int regularformations = 0x05; // we start rolling formations from where we stopped rolling special-case formations
				const int bossformations = 0x73; // we stop rolling formations once we reach boss formations
				for(byte i = regularformations; i < bossformations; ++i) // general loop for formation generation
				{
					if (i == imp_encounter || i == phantom_encounter || i == warmech_encounter)
						continue; // don't do these encounters (we will roll them as special exceptions)
					Formation f = new Formation(); // this is the formation we are hoping to write
					// we will start narrowing down the list of monsters we can feature
					List<byte> availablemons = new List<byte> { };
					// first, we check if all monsters have been featured yet
					if(uniqueEnemyIDs.Where(NotFeatured).Count() == 0)
					{
						// if we have featured all monsters, reset the featured array
						foreach (byte mon in uniqueEnemyIDs)
						{
							featured[mon] = false;
						}
					}
					// first, we determine which zones we have yet to reach the minimum for.  if they have yet to reach the minimum, they can join the union if the mon hasn't been featured yet
					for(int j = 0; j < zonecountmin.Length; ++j)
					{
						if(zone[j].Count < zonecountmin[j])
						{
							availablemons = availablemons.Union(zonemons[j].Where(NotFeatured)).ToList();
						}
					}
					// if we have already met the minimums for each zone, then any zone which has not reached its maximum can be chosen, if they have not been featured yet in this cycle
					if(availablemons.Count == 0)
					{
						for (int j = 0; j < zonecountmax.Length; ++j)
						{
							if (zone[j].Count < zonecountmax[j])
							{
								availablemons = availablemons.Union(zonemons[j].Where(NotFeatured)).ToList();
							}
						}
					}
					// if we still don't have any mons available, then we can freely pick from all mon IDs that have yet to be featured
					if (availablemons.Count == 0)
					{
						availablemons = uniqueEnemyIDs.Where(NotFeatured).ToList();
						// and if we simply don't have mons available because they've all been featured, all mons are considered available (this should never execute)
						if(availablemons.Count == 0)
						{
							availablemons = uniqueEnemyIDs.ToList();
						}
					}
					// now we pick one of the availablemons at random as our first mon
					f.Top = availablemons.PickRandom(rng);
					f.tileset = enemyTilesets[f.Top]; // and we pick the corresponding tileset
					// we need to select which zone we are aiming to fill on the B-Side (and if we require a Large-only or Small-only formation to do that, we set the shape of the formation accordingly)
					List<int> availablezones = enemyZones[f.Top].Where(index => zone[index].Count < zonecountmin[index]).ToList();
					if (availablezones.Count == 0) // if there are no zones available that haven't reached their mincount, check for maxcount instead
						availablezones = enemyZones[f.Top].Where(index => zone[index].Count < zonecountmax[index]).ToList();
					if (availablezones.Count == 0) // and if we've filled mincount and maxcount, just list all available zones
						availablezones = enemyZones[f.Top].ToList();
					if(availablezones.Count == 0) // and if by some freak chance the enemy has not been assigned any zones, write an error report to Console and abort Formation shuffle
					{
						Console.WriteLine(f.Top);
						Console.WriteLine(enemyStats[f.Top].exp);
						Console.WriteLine("This Enemy Has No Zones");
						return false;
					}
					int zoneB = availablezones.PickRandom(rng); // the B-Side zone we are trying to fill
					int limit = Large(f.Top) ? 2 : 6;
					if (enemyStats[f.Top].exp * limit < zonexpreqs[zoneB, 1])
						f.shape = Large(f.Top) ? 0x01 : 0x00;
					else
					{
						if (rng.Between(0, 2) == 0)
							f.shape = Large(f.Top) ? 0x01 : 0x00;
						else
							f.shape = 0x02;
					}
					// now we look for what we can pick for the second mon
					if(f.shape == 0x00)
					{
						availablemons = enemiesInTileset[f.tileset].Where(mon => Small(mon) && NotFeatured(mon) && mon != f.Top).ToList();
						if (availablemons.Count == 0)
							availablemons = enemiesInTileset[f.tileset].Where(Small).ToList();
					}
					else if (f.shape == 0x01)
					{
						availablemons = enemiesInTileset[f.tileset].Where(mon => Large(mon) && NotFeatured(mon) && mon != f.Top).ToList();
						if (availablemons.Count == 0)
							availablemons = enemiesInTileset[f.tileset].Where(Large).ToList();
					}
					else
					{
						availablemons = enemiesInTileset[f.tileset].Where(mon => NotFeatured(mon) && mon != f.Top).ToList();
						if (availablemons.Count == 0)
							availablemons = enemiesInTileset[f.tileset].Where(mon=> mon != f.Top).ToList(); // if all other mons in this tileset have been featured, all mons in this tileset are eligible as a second mon
					}
					f.id[1] = availablemons.PickRandom(rng); // and pick a random mon from the available IDs to be mon 2 (and the primary monster of the A-side)
					// we will attempt to hit a zone with the A-Side but if we can't we'll just try to make an encounter as close as possible to the goal
					availablezones = enemyZones[f.id[1]].Where(index => zone[index].Count < zonecountmin[index]).ToList();
					if (availablezones.Count == 0) // if there are no zones available that haven't reached their mincount, check for maxcount instead
						availablezones = enemyZones[f.id[1]].Where(index => zone[index].Count < zonecountmax[index]).ToList();
					if (availablezones.Count == 0) // and if we've filled mincount and maxcount, just list all available zones
						availablezones = enemyZones[f.id[1]].ToList();
					if (availablezones.Count == 0) // and if by some freak chance the enemy has not been assigned any zones, write an error report to Console and abort Formation shuffle
					{
						Console.WriteLine(f.id[1]);
						Console.WriteLine(enemyStats[f.id[1]].exp);
						Console.WriteLine("This Enemy Has No Zones");
						return false;
					}
					int zoneA = availablezones.PickRandom(rng);
					// now we set the palettes
					f.pal1 = enemyPalettes[f.Top];
					if (enemyPalettes[f.Top] != enemyPalettes[f.id[1]])
						f.pal2 = enemyPalettes[f.id[1]];
					else // if first and second enemies in the formation have the same palette, draw a second palette at random
					{
						List<byte> availablepalettes = palettesInTileset[f.tileset].Where(pal => pal != f.pal1).ToList();
						if (availablepalettes.Count == 0)
							f.pal2 = f.pal1; // if there is only one palette in the tileset, just set the second palette to the same value as the first
						else
							f.pal2 = availablepalettes.PickRandom(rng);
					}
					// and we draw any monsters that conform to both the tileset and palette choices as the third and fourth monsters (if there are more than two, they are dropped after a shuffle of the list)
					availablemons = enemiesInTileset[f.tileset].Where(mon => mon != f.Top && mon != f.id[1] && (enemyPalettes[mon] == f.pal1 || enemyPalettes[mon] == f.pal2)).ToList();
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
					zoneB = ENF_Picker_BSide(rng, f, zoneB, 0);
					zoneA = ENF_Picker_ASide(rng, f, zoneA, 1);
					if(zoneA == -1 || zoneB == -1)
					{
						featured[f.Top] = true;
						featured[f.id[1]] = true;
						--i;
						continue; // loop again after setting the problematic monsters to featured status so they are de-prioritized and won't cause an infinite loop problem
					}
					// assign the pic and palette bytes to each monster slot
					ENF_AssignPicAndPaletteBytes(f);
					// set surprise rate and unrunnability flags
					f.unrunnable = rng.Between(0, 29) + (zoneA > zoneB ? zoneA : zoneB) >= 32 ? true : false; // unrunnable chance is higher for later zones
					if (f.unrunnable)
						f.surprise = (byte)rng.Between(3, 30);
					else
					{
						if(bossformations - surpcount.Sum() <= i)
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
					zone[zoneA].Add(i);
					zone[zoneB].Add((byte)(i | 0x80));
					// log and compress this formation
					LogAndCompressFormation(f, i);
				}
				PrintFormationInfo();
				return true;
			}

			public int ENF_Picker_BSide(MT19337 rng, Formation f, int zone, int priority)
			{
				// draw B-Side formation, prioritizing the priority monster (defaults to f.id[0] if given an invalid priority)
				if (priority < 0 || priority > 1)
					priority = 0;
				if (f.id[priority] == 0xFF)
					priority = 0;
				if (f.Top == 0xFF)
					return -1; // do not draw mons if there is no valid mon as priority
				f.monMin[priority + 4] = 1; // there will always be at least one mon of the priority
				f.monMax[priority + 4] = 1;
				int minXP = enemyStats[f.id[priority]].exp;
				int maxXP = enemyStats[f.id[priority]].exp;
				// add to the maxcount for priority until we reach the minimum-maximum for this zone, or we run out of space in this formation to place more
				int smallLimit = 0, largeLimit = 0;
				switch(f.shape)
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
				if (Large(f.id[priority]))
					largeLimit--;
				else
					smallLimit--;
				while(smallLimit > 0 && largeLimit > 0 && maxXP < zonexpreqs[zone, 1])
				{
					f.monMax[priority + 4]++;
					if (Large(f.id[priority]))
						largeLimit--;
					else
						smallLimit--;
					maxXP += enemyStats[f.id[priority]].exp;
					if (maxXP > zonexpreqs[zone, 2])
					{
						f.monMax[priority + 4]--;
						if (Large(f.id[priority]))
							largeLimit++;
						else
							smallLimit++;
						maxXP -= enemyStats[f.id[priority]].exp;
						break;
					}
				}
				// now, we can add to the maxcount from either mon until we surpass the max XP for this zone, but monsters that don't contribute enough to zone requirements will not be selected
				List<int> validindices = new List<int> { priority };
				for(int i = 0; i < 1; ++i)
				{
					if (f.id[i] == 0xFF)
						continue; // don't include mons with an invalid index
					int sizeFactor = Large(f.id[i]) ? 6 : 12;
					if (enemyStats[f.id[i]].exp > zonexpreqs[zone, 2] / sizeFactor)
						validindices.Add(i); // valid monsters must contribute experience equal to 1/12th of the xp cap for this zone
				}
				while((smallLimit > 0 || largeLimit > 0) && validindices.Count > 0 && rng.Between(0, 14) > 0) // there is a 6.66% chance of ending new monster addition every cycle
				{
					int index = validindices.PickRandom(rng);
					if(maxXP + enemyStats[f.id[index]].exp <= zonexpreqs[zone, 2] && ((Large(f.id[index]) && largeLimit > 0) || (Small(f.id[index]) && smallLimit > 0)))
					{
						f.monMax[index + 4]++;
						maxXP += enemyStats[f.id[index]].exp;
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
				// now that we have the maximums, we determine the minimums. first, we ensure that we can hit the minimum for this zone
				validindices = new List<int> { };
				for (int i = 0; i < 1; ++i)
				{
					if (f.monMax[i + 4] > 0)
						validindices.Add(i); // add any monster that has a maxcount
				}
				while (minXP < zonexpreqs[zone, 0] && validindices.Count > 0)
				{
					int index = validindices.PickRandom(rng);
					if (f.monMin[index + 4] < f.monMax[index + 4])
					{
						f.monMin[index + 4]++;
						minXP += enemyStats[f.id[index]].exp;
					}
					else
						validindices.Remove(index);
				}
				// set the minimums to a random number between the current minimum and the maximum
				if (f.monMax[4] > 0)
					f.monMin[4] = rng.Between(f.monMin[4], f.monMax[4]);
				if (f.monMax[5] > 0)
					f.monMin[5] = rng.Between(f.monMin[5], f.monMax[5]);
				// if we were able to conform to all requirements of the zone, return false, but if we couldn't conform to the zone requirements return true so we know to calc the new zone
				return ENF_Calc_BSide_Zone(f);
			}

			public int ENF_Picker_ASide(MT19337 rng, Formation f, int zone, int priority)
			{
				// draw A-Side formation, prioritizing the priority monster (defaults to f.id[0] if given an invalid priority)
				if (priority < 0 || priority > 3)
					priority = 0;
				if (f.id[priority] == 0xFF)
					priority = 0;
				if (f.Top == 0xFF)
					return -1; // do not draw mons if there is no valid mon as priority
				f.monMin[priority] = 1; // there will always be at least one mon of the priority
				f.monMax[priority] = 1;
				int minXP = enemyStats[f.id[priority]].exp;
				int maxXP = enemyStats[f.id[priority]].exp;
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
				if (Large(f.id[priority]))
					largeLimit--;
				else
					smallLimit--;
				// unlike the B-Side where we force the priority monster to be placed early, here there is no such requirement.  we just start adding to maxcounts
				List<int> validindices = new List<int> { }; // no weighting selection towards priority in A-Side
				List<int> trashindices = new List<int> { }; // list of "trash mons" that may be optionally added in after general mon selection
				for (int i = 0; i < 3; ++i)
				{
					if (f.id[i] == 0xFF)
						continue; // don't include mons with an invalid index
					int sizeFactor = Large(f.id[i]) ? 6 : 12;
					if (enemyStats[f.id[i]].exp > zonexpreqs[zone, 2] / sizeFactor)
						validindices.Add(i); // valid monsters must contribute experience equal to 1/12th of the xp cap for this zone
					else if (Small(f.id[i]))
						trashindices.Add(i);
				}
				if(validindices.Count == 0) // if we have no valid indices (possible if some zones), we use the minimum-maximum instead of total max
				{
					trashindices.Clear();
					for (int i = 0; i < 3; ++i)
					{
						if (f.id[i] == 0xFF)
							continue; // don't include mons with an invalid index
						int sizeFactor = Large(f.id[i]) ? 6 : 12;
						if (enemyStats[f.id[i]].exp > zonexpreqs[zone, 1] / sizeFactor)
							validindices.Add(i); // valid monsters must contribute experience equal to 1/12th of the xp cap for this zone
						else if (Small(f.id[i]))
							trashindices.Add(i);
					}
				}
				while ((smallLimit > 0 || largeLimit > 0) && validindices.Count > 0)
				{
					if (rng.Between(0, 14) == 0 && maxXP > zonexpreqs[zone, 1])
						break; // 6.66% chance of ending the while loop if we have already met the minimum-maximum
					int index = validindices.PickRandom(rng);
					if (maxXP + enemyStats[f.id[index]].exp <= zonexpreqs[zone, 2] && ((Large(f.id[index]) && largeLimit > 0) || (Small(f.id[index]) && smallLimit > 0)))
					{
						f.monMax[index]++;
						maxXP += enemyStats[f.id[index]].exp;
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
				for (int i = 0; i < 3; ++i)
				{
					if (f.id[i] == 0xFF)
						continue;
					if (f.monMax[i] > 0)
						validindices.Add(i); // add any monster that has a maxcount
				}
				while (minXP < zonexpreqs[zone, 0] && validindices.Count > 0)
				{
					int index = validindices.PickRandom(rng);
					if (f.monMin[index] < f.monMax[index])
					{
						f.monMin[index]++;
						minXP += enemyStats[f.id[index]].exp;
					}
					else
						validindices.Remove(index);
				}
				// set the minimums to a random number between the current minimum and the maximum
				for(int i = 0; i < 3; ++i)
				{
					f.monMin[i] = rng.Between(f.monMin[i], f.monMax[i]);
				}
				// if there is space, we can add 1-2 of each trash index (so long as they do not exceed the zone's maximum).  only smalls can be trash mons.
				if(trashindices.Count > 0 && smallLimit > 0)
				{
					for(int i = 0; i < trashindices.Count; ++i)
					{
						if(rng.Between(0, 2) == 0) // 33% chance of rejecting a trash index
						{
							if(Small(f.id[trashindices[i]]))
							{
								if(maxXP + enemyStats[f.id[trashindices[i]]].exp < zonexpreqs[zone, 2])
								{
									smallLimit--;
									f.monMax[trashindices[i]]++;
									maxXP += enemyStats[f.id[trashindices[i]]].exp;
								}
								if(rng.Between(0, 1) == 0) // 50% chance of adding a second trash mon of the same type
								{
									if (maxXP + enemyStats[f.id[trashindices[i]]].exp < zonexpreqs[zone, 2])
									{
										smallLimit--;
										f.monMax[trashindices[i]]++;
										maxXP += enemyStats[f.id[trashindices[i]]].exp;
									}
								}
							}
						}
					}
				}
				// if we were able to conform to all requirements of the zone, return false, but if we couldn't conform to the zone requirements return true so we know to calc the new zone
				return ENF_Calc_ASide_Zone(f);
			}

			public int ENF_Calc_BSide_Zone(Formation f)
			{
				// calculates a zone for the encounter to fit if we couldn't reach our goal zone (return -1 if the zone is completely invalid)
				int return_value = -1;
				List<int> acceptableValues = new List<int> { };
				int minXP = 0, maxXP = 0;
				if(f.monMax[4] > 0)
				{
					minXP += f.monMin[4] * enemyStats[f.id[0]].exp;
					maxXP += f.monMax[4] * enemyStats[f.id[0]].exp;
				}
				if(f.monMax[5] > 0)
				{
					minXP += f.monMin[5] * enemyStats[f.id[1]].exp;
					maxXP += f.monMax[5] * enemyStats[f.id[1]].exp;
				}
				for(int i = 0; i < zonecountmin.Length; ++i)
				{
					if (minXP >= zonexpreqs[i, 0] && maxXP >= zonexpreqs[i, 1] && maxXP <= zonexpreqs[i, 2])
						acceptableValues.Add(i);
				}
				List<int> mincountValues = acceptableValues.Where(id => zone[id].Count < zonecountmin[id]).ToList();
				if (mincountValues.Count == 0)
					mincountValues = acceptableValues.Where(id => zone[id].Count < zonecountmax[id]).ToList();
				if (mincountValues.Count > 0)
					return_value = mincountValues.Max();
				else
				{
					if(acceptableValues.Count > 0)
						return_value = acceptableValues.Max();
				}
				return return_value;
			}

			public int ENF_Calc_ASide_Zone(Formation f)
			{
				// calculates a zone for the encounter to fit if we couldn't reach our goal zone (return -1 if the zone is completely invalid)
				int return_value = -1;
				List<int> acceptableValues = new List<int> { };
				int minXP = 0, maxXP = 0;
				for(int i = 0; i < 3; ++i)
				{
					if (f.id[i] == 0xFF)
						continue;
					if(f.monMax[i] > 0)
					{
						minXP += f.monMin[i] * enemyStats[f.id[i]].exp;
						maxXP += f.monMax[i] * enemyStats[f.id[i]].exp;
					}
				}
				for (int i = 0; i < zonecountmin.Length; ++i)
				{
					if (minXP >= zonexpreqs[i, 0] && maxXP >= zonexpreqs[i, 1] && maxXP <= zonexpreqs[i, 2])
						acceptableValues.Add(i);
				}
				List<int> mincountValues = acceptableValues.Where(id => zone[id].Count < zonecountmin[id]).ToList();
				if (mincountValues.Count == 0)
					mincountValues = acceptableValues.Where(id => zone[id].Count < zonecountmax[id]).ToList();
				if (mincountValues.Count > 0)
					return_value = mincountValues.Max();
				else
				{
					if (acceptableValues.Count > 0)
						return_value = acceptableValues.Max();
				}
				return return_value;
			}

			public void ENF_AssignPicAndPaletteBytes(Formation f)
			{
				for (int i = 0; i < 4; ++i)
				{
					if (f.id[i] == 0xFF)
						continue;
					if (enemyPalettes[f.id[i]] == f.pal2)
						f.paletteassignment |= (byte)(0b00010000 << (3 - i));
					f.pics |= (byte)(enemyPics[f.id[i]] << (i * 2));
				}
			}

			public void ENF_DrawForcedSingleFight(MT19337 rng, int zoneB, byte id)
			{
				// draws an encounter whose B-Side is always a group of one monster type, pulled from the specified.  it will always produce a fight near the maximum XP for this zone.
				// the A-Side will be a random assortment of monsters that may or may not include the B-Side monster.  if it is possible to fit the zone requirement in a 2-6 formation, that will always be selected
				// this formation is always unrunnable (this routine is used for the replacements for the Wizard, Eye, and Zombie Dragon fight replacements)
				Formation f = new Formation();
				f.Top = zonemons[zoneB].PickRandom(rng);
				f.tileset = enemyTilesets[f.Top];
				int limit = Large(f.Top) ? 2 : 6;
				if (enemyStats[f.Top].exp * limit < zonexpreqs[zoneB, 2])
					f.shape = Large(f.Top) ? 0x01 : 0x00;
				else
					f.shape = 0x02;
				f.monMax[4] = zonexpreqs[zoneB, 2] / enemyStats[f.Top].exp;
				if (f.monMax[4] > GetMonLimit(f.Top, f.shape))
					f.monMax[4] = GetMonLimit(f.Top, f.shape);
				f.monMin[4] = zonexpreqs[zoneB, 1] / enemyStats[f.Top].exp;
				if (f.monMin[4] > f.monMax[4])
					f.monMin[4] = f.monMax[4];
				if(f.monMax[4] == 0) // this should never execute but just in case
				{
					f.monMin[4] = 1;
					f.monMax[4] = 1;
				}
				if (f.monMin[4] == 0)
					f.monMin[4] = 1;
				// draw all potential mons for A-Side
				List<byte> availablemons = enemiesInTileset[f.tileset].Where(mon => mon != f.Top).ToList();
				switch(f.shape)
				{
					case 0x00:
						availablemons = availablemons.Where(Small).ToList();
						break;
					case 0x01:
						availablemons = availablemons.Where(Large).ToList();
						break;
				}
				f.id[1] = availablemons.PickRandom(rng);
				f.pal1 = enemyPalettes[f.Top];
				f.pal2 = enemyPalettes[f.id[1]]; // if these are both the same, i don't really care
				availablemons = enemiesInTileset[f.tileset].Where(mon => mon != f.Top && mon != f.id[1] && (enemyPalettes[mon] == f.pal1 || enemyPalettes[mon] == f.pal2)).ToList();
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
				int zoneA = enemyZones[f.id[1]].PickRandom(rng); // pick a random zone to aim for from the second mon
				// now we actually run the generic picker
				zoneA = ENF_Picker_ASide(rng, f, zoneA, 1);
				zone[zoneB].Add((byte)(id | 0x80));
				if (zoneA != -1)
					zone[zoneA].Add(id); // if for some reason we didn't fill any valid zone from the A-side, we simply don't add it to any zones (and thus it will never be seen)
				f.surprise = 4;
				f.unrunnable = true; // these fights are always unrunnable
				// put on the finishing touches and log and compress this formation
				ENF_AssignPicAndPaletteBytes(f);
				LogAndCompressFormation(f, id);
			}

			public void ENF_DrawBossEncounter(MT19337 rng, int bossmonster, int quantity, byte shape, bool unrunnable, byte surpriserate, byte id)
			{
				// draws a boss encounter on the A side, with the specified monster, quantity thereof, and shape on the A-Side, and information about runnability (true for most, but false for warmech) / surprise rate
				// the B-Side will feature one or two monsters from the same tileset that are compatible with the boss' palette and another palette chosen at random
				// the B-Side will be available as a random encounter.
				Formation f = new Formation();
				f.id[2] = (byte)bossmonster; // we place the boss monster in the third slot
				f.tileset = enemyTilesets[f.id[2]];
				f.pal1 = enemyPalettes[f.id[2]];
				f.shape = shape;
				f.monMin[2] = quantity;
				f.monMax[2] = quantity; // always produce the necessary number of enemies
				// draw mons for B-Side
				List<byte> availablemons = enemiesInTileset[f.tileset].ToList();
				switch (f.shape)
				{
					case 0x00:
						availablemons = availablemons.Where(Small).ToList();
						break;
					case 0x01:
						availablemons = availablemons.Where(Large).ToList();
						break;
				}
				if(availablemons.Count > 0)
				{
					f.Top = availablemons.PickRandom(rng);
					f.pal2 = enemyPalettes[f.Top];
					availablemons = availablemons.Where(mon => mon != f.Top && mon != f.id[2] && (enemyPalettes[mon] == f.pal1 || enemyPalettes[mon] == f.pal2)).ToList();
					if (availablemons.Count > 0)
					{
						f.id[1] = availablemons.PickRandom(rng);
					}
					int zoneB = enemyZones[f.Top].PickRandom(rng);
					zoneB = ENF_Picker_BSide(rng, f, zoneB, 0);
					if (zoneB != -1)
						zone[zoneB].Add((byte)(id | 0x80)); // if for some reason we didn't fill any valid zone from the B-side, we simply don't add it to any zones (and thus it will never be seen)
				}			
				// if for some reason there were no available mons, then a B-Side is not drawn at all and no formation is added to the zones, so this slot will remain unused.  this shouldn't happen, though
				ENF_AssignPicAndPaletteBytes(f);
				LogAndCompressFormation(f, id);
			}

			public void ENF_DrawBumFight(MT19337 rng, byte id)
			{
				// draws the bum fight around Coneria Castle.  this will always be 3-4 weak noodle enemies (default IMPs) that are placed on the B-Side.  the A-side will be a generic battle which may or may not
				// include more bums
				Formation f = new Formation();
				f.Top = Enemy.Imp;
				f.tileset = enemyTilesets[f.Top];
				f.shape = Large(f.Top) ? 0x01 : 0x00;
				f.pal1 = enemyPalettes[f.Top];
				f.monMin[4] = 3;
				f.monMax[4] = 4;
				// draw mons for A-Side
				List<byte> availablemons = enemiesInTileset[f.tileset].Where(mon => mon != f.Top && enemyPalettes[mon] != f.pal1).ToList();
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
				f.pal2 = enemyPalettes[f.id[1]];
				availablemons = enemiesInTileset[f.tileset].Where(mon => mon != f.Top && mon != f.id[1] && (enemyPalettes[mon] == f.pal1 || enemyPalettes[mon] == f.pal2)).ToList();
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
				int zoneA = enemyZones[f.id[1]].PickRandom(rng);
				zoneA = ENF_Picker_ASide(rng, f, zoneA, 1);
				if (zoneA != -1)
					zone[zoneA].Add(id); // if for some reason we didn't fill any valid zone from the A-side, we simply don't add it to any zones (and thus it will never be seen)
				// if for some reason there were no available mons to draw, we don't draw an A-Side and it will never be added to a zone, thus it will never be used
				ENF_AssignPicAndPaletteBytes(f);
				LogAndCompressFormation(f, id);
			}

			public byte ENF_PickAVamp(MT19337 rng)
			{
				// picks a random vampire for the Earth Cave fight.  priority is given to vampires that give out less than 3000 experience points, then to any monster under 3000 experience points
				List<byte> vamps = uniqueEnemyIDs.Where(mon => enemyImage[mon] == 24 && enemyStats[mon].exp <= 3000).ToList();
				if (vamps.Count == 0)
					vamps = uniqueEnemyIDs.Where(mon => enemyStats[mon].exp <= 3000).ToList();
				return vamps.PickRandom(rng);
			}

			public void DoDomains(MT19337 rng)
			{
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
				zone[2].Shuffle(rng);
				zone[3].Shuffle(rng);
				zone[4].Shuffle(rng);
				zone[5].Shuffle(rng);
				zone[6].Shuffle(rng);
				zone[8].Shuffle(rng);
				zone[9].Shuffle(rng);
				traptile_formations[(int)TrapTiles.TRAP_IMAGES] = zone[3][0];
				traptile_formations[(int)TrapTiles.TRAP_MUMMIES] = zone[3][1];
				traptile_formations[(int)TrapTiles.TRAP_MUDGOLS] = zone[6][0];
				traptile_formations[(int)TrapTiles.TRAP_NITEMARES] = zone[6][1];
				traptile_formations[(int)TrapTiles.TRAP_ZOMBIE_D] = zombieD_encounter;
				traptile_formations[(int)TrapTiles.TRAP_GARGOYLES] = zone[2][0];
				traptile_formations[(int)TrapTiles.TRAP_SEAFOOD_PARTY_MIX] = zone[8][0];
				traptile_formations[(int)TrapTiles.TRAP_SHARKNADO] = zone[8][1];
				traptile_formations[(int)TrapTiles.TRAP_WATERS] = zone[8][2];
				traptile_formations[(int)TrapTiles.TRAP_SEASHRINE_MUMMIES] = zone[8][3];
				traptile_formations[(int)TrapTiles.TRAP_ZOMBIES] = zone[0][0];
				traptile_formations[(int)TrapTiles.TRAP_GARGOYLES2] = zone[2][1];
				traptile_formations[(int)TrapTiles.TRAP_GIANTS] = zone[4][0];
				traptile_formations[(int)TrapTiles.TRAP_GIANTS_IGUANAS] = zone[4][1];
				traptile_formations[(int)TrapTiles.TRAP_EARTH] = zone[4][2];
				traptile_formations[(int)TrapTiles.TRAP_OGRES_HYENAS] = zone[4][3];
				traptile_formations[(int)TrapTiles.TRAP_SPHINX] = zone[5][0];
				traptile_formations[(int)TrapTiles.TRAP_FIRE] = zone[5][1];
				traptile_formations[(int)TrapTiles.TRAP_GREYWORM] = zone[5][2];
				traptile_formations[(int)TrapTiles.TRAP_AGAMA] = zone[5][3];
				traptile_formations[(int)TrapTiles.TRAP_RED_D] = zone[5][4];
				traptile_formations[(int)TrapTiles.TRAP_UNDEAD_PARTYMIX] = zone[5][5];
				traptile_formations[(int)TrapTiles.TRAP_FROSTRURUS] = zone[5][6];
				traptile_formations[(int)TrapTiles.TRAP_FROSTGIANT] = zone[5][7];
				traptile_formations[(int)TrapTiles.TRAP_MAGES] = zone[5][8];
				traptile_formations[(int)TrapTiles.TRAP_FROST_D] = zone[5][9];
				traptile_formations[(int)TrapTiles.TRAP_EYE] = eye_encounter;
				traptile_formations[(int)TrapTiles.TRAP_WATERFALL_MUMMIES] = zone[8][3];
				traptile_formations[(int)TrapTiles.TRAP_WIZARDS] = wizard_encounter;
				traptile_formations[(int)TrapTiles.TRAP_WIZARDS2] = wizard_encounter;
				traptile_formations[(int)TrapTiles.TRAP_COBRAS] = zone[3][2];
				traptile_formations[(int)TrapTiles.TRAP_BLUE_D] = zone[9][0];
				traptile_formations[(int)TrapTiles.TRAP_SLIMES] = zone[9][1];
				traptile_formations[(int)TrapTiles.TRAP_PHANTOM] = phantom_encounter;
				// proceed to place mons in all zones
				zone[2].Shuffle(rng); // re-shuffling zone lists
				zone[3].Shuffle(rng);
				zone[4].Shuffle(rng);
				zone[5].Shuffle(rng);
				zone[6].Shuffle(rng);
				zone[8].Shuffle(rng);
				zone[9].Shuffle(rng);
				for (int i = 0; i < zonecountmin.Length; ++i)
				{
					int loop = 0;
					int f = 0;
					int a = 0;
					while(f < zone[i].Count)
					{
						if(loop == 0)
						{
							for(int j = 0; j < 8; ++j)
							{
								PutD(zoneaddr[i][a] + j, zone[i][f]); // first loop: fill the domain with the encounter type
							}
						}
						if(loop == 1)
						{
							for (int j = 0; j < 8; j += 2)
							{
								PutD(zoneaddr[i][a] + j, zone[i][f]); // second loop: 1, 3, 5, and 7 are filled with the second formation
							}
						}
						if(loop == 2)
						{
							PutD(zoneaddr[i][a] + 2, zone[i][f]); // third loop: 3, 6, and 8 are filled with the third encounter
							PutD(zoneaddr[i][a] + 5, zone[i][f]);
							PutD(zoneaddr[i][a] + 7, zone[i][f]);
						}
						if(loop == 3)
						{
							PutD(zoneaddr[i][a] + 3, zone[i][f]); // fourth loop: 4 and 7 are filled with the 4th encounter
							PutD(zoneaddr[i][a] + 6, zone[i][f]);
						}
						if(loop == 4)
						{
							PutD(zoneaddr[i][a] + 4, zone[i][f]); // fifth loop: 5 and 6 are filled with the 5th encounter
							PutD(zoneaddr[i][a] + 5, zone[i][f]);
						}
						if(loop > 4)
						{
							PutD(zoneaddr[i][a] + loop, zone[i][f]); // sicth-eigth loop: slot 6, 7, or 8 is filled depending on which loop we are at
						}
						f++;
						a++;
						if(a == zoneaddr[i].Count)
						{
							a = 0;
							loop++;
							if (loop == 8)
								break; // place no more than 8 encounters per domain even if formations are left to be placed (this should basically never happen)
						}
					}
				}
				// duplicate entries in the overworld that clone other areas
				foreach(int[] array in zoneclone)
				{
					byte[] dataToCopy = new byte[8];
					for(int i = 0; i < 8; ++i)
					{
						dataToCopy[i] = GetD(array[0] + i);
					}
					for(int i = 1; i < array.Length; ++i)
					{
						for(int j = 0; j < 8; ++j)
						{
							PutD(array[i] + j, dataToCopy[j]);
						}
					}
				}
				// special zone placement for overworld 4,4 and 4,5: replace the first four encounters with encounter 0x80 (imp group)
				PutD(0x120, imp_encounter); PutD(0x121, imp_encounter); PutD(0x122, imp_encounter); PutD(0x123, imp_encounter); PutD(0x160, imp_encounter); PutD(0x161, imp_encounter); PutD(0x162, imp_encounter); PutD(0x163, imp_encounter);
				PutD(0x39E, 0x56); // replace 7th encounter on sky 5F with warmech
			}
		}

		public void DoEnemizer(MT19337 rng, bool enemies, bool formations, bool battledomains)
		{
			Enemizer en = new Enemizer();
			// load vanilla values from ROM into the enemizer
			en.PutAllFormationData(Get(FormationDataOffset, FormationDataSize * FormationDataCount));
			en.PutAllDomainData(Get(ZoneFormationsOffset, ZoneFormationsSize * ZoneCount));
			for(int i = 0; i < MagicCount; ++i)
			{
				byte[] spellInfo = Get(MagicOffset + i * MagicSize, MagicSize);
				en.ReadSpellDataFromMagicBytes(en.decompressSpellData(spellInfo), i);
			}
			for (int i = 0; i < EnemyCount; ++i)
			{
				byte[] enemyInfo = Get(EnemyOffset + i * EnemySize, EnemySize);
				en.ReadEnemyDataFromEnemies(en.decompressEnemyData(enemyInfo), i);
			}
			en.enemyNames = ReadText(EnemyTextPointerOffset, EnemyTextPointerBase, EnemyCount); // load all the enemy names into the array for use by enemizer
			for(int i = 0; i < EnemyCount; ++i) // we want to know how many characters are in enemy names so we don't make random names that are too long for the ROM to hold
			{
				en.numEnemyNameCharacters += en.enemyNames[i].Length;
			}
			for (int i = 0; i < ScriptCount; ++i)
			{
				byte[] scriptInfo = Get(ScriptOffset + i * ScriptSize, ScriptSize);
				en.ReadScriptDataFromScript(en.decompressEnemyScript(scriptInfo), i);
			}
			for (int i = 0; i < FormationCount; ++i)
			{
				byte[] formationData = Get(FormationDataOffset + FormationDataSize * i, FormationDataSize).ToBytes();
				en.ReadEnemyDataFromFormation(en.decompressFormation(formationData), i); // read information about enemy formations from the ROM
			}
			en.PurgeIDFromEnemyTilesetList(Enemy.Imp);
			en.PurgeIDFromEnemyTilesetList(Enemy.Pirate);
			en.PurgeIDFromEnemyTilesetList(Enemy.Phantom);
			en.PurgeIDFromEnemyTilesetList(Enemy.Astos);
			en.PurgeIDFromEnemyTilesetList(Enemy.Garland);
			en.PurgeIDFromEnemyTilesetList(Enemy.WarMech); // purging enemies from the generic enemy lists that we don't want to appear outside of set battles
			if (enemies)
			{
				// do enemizer stuff
				if(en.DoEnemies(rng))
				{
					formations = true; // must use formation generator with enemizer
					for(int i = 0; i < EnemyCount; ++i)
					{
						Put(EnemyOffset + EnemySize * i, en.enemyStats[i].CompressData()); // move every entry from the enemizer to the ROM
					}
					for(int i = 0; i < ScriptCount; ++i)
					{
						Put(ScriptOffset + ScriptSize * i, en.enemyScripts[i].compressData()); // and move the modified scripts as well
					}
					var enemyTextPart1 = en.enemyNames.Take(2).ToArray();
					var enemyTextPart2 = en.enemyNames.Skip(2).ToArray();
					WriteText(enemyTextPart1, EnemyTextPointerOffset, EnemyTextPointerBase, 0x2CFEC);
					WriteText(enemyTextPart2, EnemyTextPointerOffset + 4, EnemyTextPointerBase, EnemyTextOffset);
					en.PurgeIDFromEnemyTilesetList(Enemy.Imp);
					en.PurgeIDFromEnemyTilesetList(Enemy.Pirate);
					en.PurgeIDFromEnemyTilesetList(Enemy.Phantom);
					en.PurgeIDFromEnemyTilesetList(Enemy.Astos);
					en.PurgeIDFromEnemyTilesetList(Enemy.Garland);
					en.PurgeIDFromEnemyTilesetList(Enemy.WarMech); // making doubleplus sure these don't appear, probably not necessary
				}
				else
				{
					Console.WriteLine("Fission Mailed - Abort Enemy Shuffle");
					throw new InsaneException("Something went wrong with Enemy Generation (Enemizer)!");
				}
			}
			if(formations)
			{
				if(en.DoFormations(rng, enemies))
				{
					Put(FormationsOffset, en.GetFormationData());
					if(en.imp_encounter != 0x80)
						throw new InsaneException(en.imp_encounter.ToString());
					// we must also do the domains
					// This code is partially lifted from ShuffleTrapTiles
					Data[0x7CDC5] = 0xD0; // changes the game's programming

					bool IsBattleTile(Blob tuple) => tuple[0] == 0x0A;
					bool IsRandomBattleTile(Blob tuple) => IsBattleTile(tuple) && (tuple[1] & 0x80) != 0x00;

					var tilesets = Get(TilesetDataOffset, TilesetDataCount * TilesetDataSize * TilesetCount).Chunk(TilesetDataSize).ToList();
					tilesets.ForEach(tile => { if (IsRandomBattleTile(tile)) tile[1] = 0x00; });
					Put(TilesetDataOffset, tilesets.SelectMany(tileset => tileset.ToBytes()).ToArray());// set all random battle tiles to zero
					
					en.DoDomains(rng);
					// write domains information
					Put(ZoneFormationsOffset, en.GetDomainData());

					// write trap tile information
					for (int i = 0; i < en.traptile_addresses.Length; ++i)
					{
						Data[TilesetDataOffset + en.traptile_addresses[i]] = en.traptile_formations[i];
					}
				}
					
				else
				{
					Console.WriteLine("Fission Mailed - Abort Formation Shuffle");
					throw new InsaneException("Something went wrong with Formation Generation (Enemizer)!");
				}
			}
		}
	}
}
