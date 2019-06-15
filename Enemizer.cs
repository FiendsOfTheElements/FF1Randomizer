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

		public class Enemizer
		{
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

			public byte[] formationData = new byte[FormationDataSize * FormationDataCount];
			public List<byte>[] enemiesInTileset = new List<byte>[GenericTilesetsCount];
			public List<byte>[] palettesInTileset = new List<byte>[GenericTilesetsCount];
			// initialize these arrays with their vanilla values.  there is no easy way to read these values and tie them to monsters so we initialize by hand.
			// note: if using a hack changes the pattern table assignments or palettes for monsters, this will not work and cause problems
			public List<byte> enemyTilesets;
			public List<byte> enemyPalettes;
			public List<bool> enemySmallOrBig;
			public List<int> enemyPics;
			public List<byte> uniqueEnemyIDs = new List<byte> { 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x0A, 0x0B, 0x0C, 0x10, 0x11, 0x12, 0x13, 0x14, 0x15, 0x16, 0x17, 0x18, 0x19, 0x1A, 0x1B, 0x1C,
														 0x1D, 0x1E, 0x1F, 0x20, 0x21, 0x22, 0x23, 0x24, 0x25, 0x26, 0x27, 0x28, 0x29, 0x2B, 0x2C, 0x2D, 0x2E, 0x2F, 0x30, 0x31, 0x32, 0x34, 0x35, 0x36, 0x37,
														 0x38, 0x39, 0x3A, 0x3B, 0x3C, 0x3D, 0x3E, 0x3F, 0x40, 0x41, 0x42, 0x43, 0x44, 0x45, 0x46, 0x47, 0x48, 0x49, 0x4A, 0x4B, 0x4C, 0x4D, 0x4E, 0x4F, 0x50,
														 0x51, 0x52, 0x53, 0x54, 0x55, 0x56, 0x57, 0x58, 0x59, 0x5A, 0x5B, 0x5C, 0x5D, 0x5E, 0x5F, 0x60, 0x61, 0x62, 0x63, 0x64, 0x65, 0x66, 0x68, 0x6A, 0x6B,
														 0x6C, 0x6D, 0x6F, 0x70, 0x72, 0x73, 0x74, 0x75 };

			public byte[] traptile_formations = new byte[(int)TrapTiles.NUM_TRAP_TILES]; // formation pointers for each trap tile
			public int[] traptile_addresses = new int[(int)TrapTiles.NUM_TRAP_TILES]; // ROM addresses of trap tiles

			public int[] enemyXPLUT = new int[EnemyCount];
			public List<int>[] enemyZones = new List<int>[EnemyCount];
			public List<byte>[] enemyPaired = new List<byte>[EnemyCount];
			public List<List<int>> fmarkers = new List<List<int>> { };

			public bool[] featured = new bool[EnemyCount];
			public bool allfeatured = false;
			public int[] zonecountmin; // counter to tell the enemizer how many enemies we want to roll for each zone
			public int[] zonecountmax;
			public List<byte>[] zone; // array of lists which contain the formation indices we have assigned to each zone
			public int[,] zonexpreqs;
			public List<byte>[] zonemons; // array of which monsters are compatible with which zones
			public List<int>[] zoneaddr; // list of addresses to place zone data
			public int[][] zoneclone; // list of addresses which are clones of other domain addresses - the first entry in the array is the base to read from, the other entries will copy that

			public byte[] domainData = new byte[ZoneFormationsSize * ZoneCount];
			public byte phantom_encounter = 0x46; // note the location of the phantom encounter, which will not be placed as a random (but can move around)
			public byte warmech_encounter = 0x56; // note the location of the warmech encounter, which will not be placed as a random (we will not move this around since it is needed for other randomizer stuff)

			public Enemizer()
			{
				enemyTilesets = Enumerable.Repeat((byte)0x00, EnemyCount).ToList();
				enemyPalettes = Enumerable.Repeat((byte)0x00, EnemyCount).ToList();
				enemyPics = Enumerable.Repeat(0x00, EnemyCount).ToList();
				enemySmallOrBig = Enumerable.Repeat(false, EnemyCount).ToList();
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
				for (int i = 0; i < EnemyCount; ++i)
					enemyPaired[i] = new List<byte> { };
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

			public Formation decompressFormation(byte[] rawData)
			{
				Formation f = new Formation();
				if (rawData.Length < 16)
					return f; // return a blank formation if rawdata is not large enough
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
				int minXP_a = 0, minXP_b = 0, maxXP_a = 0, maxXP_b = 0;
				for (int i = 0; i < 4; ++i)
				{
					if (f.id[i] == 0xFF)
						continue;
					if (f.monMin[i] > 0)
						featured[f.id[i]] = true;
					minXP_a += f.monMin[i] * enemyXPLUT[f.id[i]];
					maxXP_a += f.monMax[i] * enemyXPLUT[f.id[i]];
				}
				for (int i = 0; i < 2; ++i)
				{
					if (f.id[i] == 0xFF)
						continue;
					if (f.monMin[i + 4] > 0)
						featured[f.id[i]] = true;
					minXP_b += f.monMin[i + 4] * enemyXPLUT[f.id[i]];
					maxXP_b += f.monMax[i + 4] * enemyXPLUT[f.id[i]];
				}
				PutF(slot * FormationDataSize, f.CompressData());
			}

			public void ReadEnemyDataFromFormation(Formation f)
			{
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
				if (allfeatured)
					Console.WriteLine("All mons have been featured");
				else
				{
					int monsfeatured = 0;
					foreach (bool yes in featured)
					{
						if (yes)
							monsfeatured++;
					}
					Console.WriteLine(monsfeatured + " mons have been featured");
				}
				for (int i = 0; i < zonecountmin.Length; ++i)
				{
					Console.WriteLine("Zone " + i + " " + zone[i].Count + "/" + zonecountmin[i]);
				}
			}

			public bool DoFormations(MT19337 rng)
			{
				phantom_encounter = 0x03; // moving the phantom encounter to 0x03 A-Side and noting this for the domain generator

				const byte startingformation = 0x04; // where we start creating random formations
				const byte warmechformation = 0x56; // warmech's encounter - this must be at 0x56 in order to ensure compatibility with patrolling or required WarMech
				const byte bossformations = 0x73; // number of formations for which special exceptions are made (imps, marsh guardian, ice guardian, phantom).  these encounters are rolled before the main loop.
				byte cur = startingformation; // we start counting at formation 0x04 and work up to 0x72

				// Draw obligatory formations first and exclude boss fights from the formation pool
				ENF_Imps(rng);    // formations 0x00 and 0x80
				ENF_Wizards(rng); // formations 0x01 and 0x81
				ENF_Eye(rng);     // formations 0x02 and 0x82
				ENF_Phantom(rng); // formations 0x03 and 0x83
				ENF_VampireZombieD(rng); // formations 0x7C and 0xFC (we will put the ordeals trap tile on the B-Side of Vampire to save space)
				ENF_Warmech(rng); // formations 0x56 and 0xD6
				ENF_Pirates(rng); // formations 0x7E and 0xFE
				ENF_Garland(rng); // formations 0x7F and 0xFF

				// build enemy lists and zone lists
				for (byte i = 0; i < enemyZones.Length; ++i)
				{
					if (GetTileset(i) > 0x0C)
						continue; // skip over bosses
					if (i == Enemy.Astos || i == Enemy.Pirate || i == Enemy.Garland || i == Enemy.Phantom || i == Enemy.WarMech)
						continue; // don't include these enemies in the list
					int limit = Large(i) ? 4 : 9;
					int lowerlimit = Large(i) ? 3 : 5;
					for (int j = 0; j < zonecountmin.Length; ++j)
					{
						if (enemyXPLUT[i] <= zonexpreqs[j, 2] && enemyXPLUT[i] * limit >= zonexpreqs[j, 1])
						{
							zonemons[j].Add(i);
							enemyZones[i].Add(j);
						}
					}
				}
				// reserve so many slots for different surprise rates and unrunnables
				int[,] surprisetiers = { { 10, 19 }, { 24, 36 }, { 48, 62 }, { 70, 100 } };
				const int surp1 = 12;
				const int surp2 = 15;
				const int surp3 = 5;
				const int surp4 = 3;
				int[] surpcount = { surp1, surp2, surp3, surp4 };

				// main loop
				uniqueEnemyIDs.Shuffle(rng);
				int stage = 0;
				int failcount_draw = 0;
				while (cur < bossformations)
				{
					if (cur == warmechformation)
					{
						cur++;
						continue;
					}
					int goalzone1 = -1, goalzone2 = -1, picker1 = -1, picker2 = -1;
					Formation f = new Formation();
					if (ENF_DrawMons_Zone(rng, f, ref goalzone1, ref goalzone2, ref picker1, ref picker2, ref stage))
					{
						failcount_draw++;
						continue;
					}

					// decide which picker we will use to generate the encounter
					/* For now we are using a generic picker that is not too complicated, this can be modified in the future */
					int outcome = RollFormation(rng, f, ref goalzone1, ref goalzone2, ref picker1, ref picker2, ref stage);
					if (outcome == -2)
					{
						continue; // if this function returns true, our formation rolling failed and we have to restart
					}

					if (ENF_ApproveFormation(f, goalzone1, goalzone2))
					{
						enemyPaired[f.Top].Add(f.id[1]);
						enemyPaired[f.id[1]].Add(f.Top);

						zone[goalzone1].Add((byte)(cur | 0x80));
						zone[goalzone2].Add(cur);
						int maxXPB = 0;
						int maxXPA = 0;
						for(int i = 4; i < 6; ++i)
						{
							if (f.id[i - 4] != 0xFF)
								maxXPB += enemyXPLUT[f.id[i - 4]] * f.monMax[i];
						}
						for (int i = 0; i < 4; ++i)
						{
							if (f.id[i] != 0xFF)
								maxXPA += enemyXPLUT[f.id[i]] * f.monMax[i];
						}

						// set surprise rate and unrunnability flags
						f.unrunnable = rng.Between(0, 29) + (goalzone1 > goalzone2 ? goalzone1 : goalzone2) >= 32 ? true : false; // unrunnable chance is higher for later zones
						if(f.unrunnable)
						{
							f.surprise = 4; // set surprise rate to default for unrunnables
						}
						else
						{
							if (cur + surpcount.Sum() >= bossformations)
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
								int s_roll = rng.Between(startingformation, bossformations);
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
						

						// log the formation we have created and put it in the bank
						LogAndCompressFormation(f, cur);

						// decrement formations left and loop again
						cur++;
					}
				}
				PrintFormationInfo();
				return true; // we have succeeded
			}

			public void ENF_StandardAssignments(Formation f, MT19337 rng)
			{
				// draw palettes
				f.pal1 = GetPalette(f.Top);
				f.pal2 = GetPalette(f.id[1]);
				if (f.pal2 == f.pal1) // pick a second palette at random if pal2 = pal1 or this palette combo has already been used
				{
					List<byte> legalPalettes = new List<byte> { };
					foreach (byte pal in palettesInTileset[f.tileset])
					{
						if (pal != f.pal1)
						{
							legalPalettes.Add(pal);
						}
					}
					if (legalPalettes.Count > 0)
						f.pal2 = legalPalettes.PickRandom(rng);
				}

				// draw monsters from those which are legal for these conditions
				List<byte> legalMonsters = new List<byte> { f.Top, f.id[1] };
				enemiesInTileset[f.tileset].Shuffle(rng);
				foreach (byte mon in enemiesInTileset[f.tileset])
				{
					if (mon == f.Top || mon == f.id[1])
						continue;
					if (enemyPalettes[mon] == f.pal1 || enemyPalettes[mon] == f.pal2)
					{
						legalMonsters.Add(mon);
					}
				}

				// if there are large or small mons in the list when the first two are larges or smalls, there is a 50% chance that the formation will convert to a 2/6 mixed, otherwise the monster is dropped
				for (int i = 0; i < 4 && i < legalMonsters.Count; ++i)
				{
					if (Large(legalMonsters[i]) && f.shape == 0x01 || Small(legalMonsters[i]) && f.shape == 0x00 || f.shape == 0x02)
					{
						f.id[i] = legalMonsters[i];
					}
				}

				// set palette and pics bytes, and determine the limits for each monster
				ENF_AssignPicAndPaletteBytes(f);
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

			public int ENF_GetRandomZone(MT19337 rng)
			{
				List<int> validzones = new List<int> { };
				for (int i = 0; i < zonecountmin.Length; ++i)
				{
					if (zone[i].Count < zonecountmin[i])
						validzones.Add(i);
				}
				if (validzones.Count == 0)
					return -2;
				return validzones.PickRandom(rng);
			}

			public bool ENF_DrawMons_Zone(MT19337 rng, Formation f, ref int zone1, ref int zone2, ref int picker1, ref int picker2, ref int stage) // draw a monster to fill a zone.
			{
				if (stage == 0)
				{
					// look up a random zone that has yet to be filled
					zone1 = ENF_GetRandomZone(rng);
					if (zone1 == -2)
					{
						stage = 1; // we are ready to move to the next stage
					}
					else
					{
						// pick a monster compatible with the zone
						List<byte> availablemons = zonemons[zone1].Where(NotFeatured).ToList();
						if (availablemons.Count == 0)
							f.Top = zonemons[zone1].PickRandom(rng);
						else
							f.Top = availablemons.PickRandom(rng);
					}
				}
				if (stage == 1)
				{
					// look up unique enemy ID
					List<byte> availablemons = uniqueEnemyIDs.Where(NotFeatured).ToList();
					if (availablemons.Count == 0)
						stage = 2;
					else
					{
						f.Top = availablemons.PickRandom(rng);
						zone1 = enemyZones[f.Top].PickRandom(rng);
					}
				}
				if (stage == 2)
				{
					// pick a zone freely and a mon to match
					zone1 = rng.Between(0, zonemons.Length - 1);
					f.Top = zonemons[zone1].PickRandom(rng);
				}

				f.tileset = GetTileset(f.Top); // get the tileset for top mon
				// pick second mon from this tileset
				enemiesInTileset[f.tileset].Shuffle(rng);
				foreach (byte mon in enemiesInTileset[f.tileset])
				{
					// reject second monster IF:
					// -second mon is the same as first mon
					// -second mon has already been paired with first mon
					if (mon == f.Top)
						continue;
					if (enemyPaired[f.Top].Contains(mon))
						continue;
					f.id[1] = mon;
				}
				if (f.id[1] == 0xFF)
				{
					Console.WriteLine("Could not select mon2");
					return true; // if we could not select a mon, return true to indicate a failure and roll another pair
				}
				// set shape according to mons
				f.shape = Large(f.Top) ? 0x01 : 0x00;
				if (Large(f.Top) != Large(f.id[1]))
					f.shape = 0x02;
				// decide which picker we will use for the first and second formation, and the second formation's zone
				List<int> validpickers = new List<int> { 0 }; // we will always have the option to do single mon
				// mixed-group picker - allowed if second monster is the same shape and if tier requirements are maintained (for minimum and maximum) with half the limit for each mon
				if (Large(f.Top) == Large(f.id[1]))
				{
					int limit = GetMonLimit(f.Top, f.shape) / 2;
					if (enemyXPLUT[f.Top] + enemyXPLUT[f.id[1]] <= zonexpreqs[zone1, 2] && (enemyXPLUT[f.Top] + enemyXPLUT[f.id[1]]) * limit >= zonexpreqs[zone1, 1])
					{
						validpickers.Add(1);
					}
				}
				// big-and-small picker - allowed if shape is 2-6 and two large / 6 small can still fit the zone requirements for experience
				if (f.shape == 0x02)
				{
					int limit1 = GetMonLimit(f.Top, f.shape), limit2 = GetMonLimit(f.id[1], f.shape);
					int modifier1 = Large(f.Top) ? 1 : 2;
					int modifier2 = Large(f.id[1]) ? 1 : 2;
					if (enemyXPLUT[f.Top] * modifier1 + enemyXPLUT[f.id[1]] * modifier2 <= zonexpreqs[zone1, 2] && enemyXPLUT[f.Top] * limit1 + enemyXPLUT[f.id[1]] * limit2 >= zonexpreqs[zone1, 1])
						validpickers.Add(2);
					else if (enemyXPLUT[f.Top] * limit1 < zonexpreqs[zone1, 1])
						return true; // don't allow mixed formations if the formation can't reach zone xp minimum requirements with 2-6 mixed and the second monster
				}
				picker1 = validpickers.PickRandom(rng);
				ENF_StandardAssignments(f, rng);
				// now select a zone and picker for the A-Side
				validpickers.Clear();
				validpickers.Add(0);
				if (Large(f.Top) == Large(f.id[1]))
					validpickers.Add(1);
				if (f.shape == 0x02)
					validpickers.Add(2);

				List<int> validzones = enemyZones[f.id[1]].Where(id => zone[id].Count < zonecountmax[id]).ToList();
				validzones.Shuffle(rng);
				for (int i = 0; i < validzones.Count; ++i)
				{
					validpickers.Shuffle(rng);
					for (int j = 0; j < validpickers.Count; ++j)
					{
						if ((validpickers[j] == 0 && validzones[i] != zone1) || validpickers[j] != picker1 || validzones[i] != zone1)
						{
							picker2 = validpickers[j];
							zone2 = validzones[i];
						}
						if (picker2 != -1)
							break;
					}
					if (zone2 != -1)
						break;
				}
				if (picker2 == -1 || zone2 == -1)
				{
					return true;
				}
				return false;
			}

			public bool ENF_ApproveFormation(Formation f, int goalzone1, int goalzone2)
			{
				if (f.monMax[0] == 0 && f.monMax[1] == 0 && f.monMax[2] == 0 && f.monMax[3] == 0 || f.monMax[4] == 0 && f.monMax[5] == 0)
					return false; // do not allow empty formations
				int minXPB = 0;
				int minXPA = 0;
				int maxXPB = 0;
				int maxXPA = 0;
				for (int i = 4; i < 6; ++i)
				{
					if (f.id[i - 4] != 0xFF)
					{
						minXPB += enemyXPLUT[f.id[i - 4]] * f.monMin[i];
 						maxXPB += enemyXPLUT[f.id[i - 4]] * f.monMax[i];
					}
				}
				for (int i = 0; i < 4; ++i)
				{
					if (f.id[i] != 0xFF)
					{
						minXPA += enemyXPLUT[f.id[i]] * f.monMin[i];
						maxXPA += enemyXPLUT[f.id[i]] * f.monMax[i];
					}
				}
				if(goalzone1 != -1 && goalzone2 != -1)
					if (maxXPB > zonexpreqs[goalzone1, 2] || maxXPA > zonexpreqs[goalzone2, 2] || minXPB < zonexpreqs[goalzone1, 0] || minXPA < zonexpreqs[goalzone2, 0])
						return false;
				List<int> newmarker1 = new List<int> { };
				List<int> newmarker2 = new List<int> { };
				for(int i = 0; i < 4; ++i)
				{
					if (f.id[i] == 0xFF)
						continue;
					if(f.monMax[i] > 0)
					{
						newmarker1.Add(f.id[i] + f.monMax[i] * 0x100);
					}
					if (i < 2)
					{
						if (f.monMax[i + 4] > 0)
						{
							newmarker2.Add(f.id[i] + f.monMax[i + 4] * 0x100);
						}
					}
				}
				if (newmarker1.Count == 0 || newmarker2.Count == 0)
					return false; // do not allow empty formations
				if(newmarker1.Count == newmarker2.Count)
				{
					if (newmarker1.Contains(newmarker2[0]))
					{
						if (newmarker2.Count == 1)
							return false;
						else if (newmarker1.Contains(newmarker2[1]))
							return false;
					}
				}
				bool nomatch = true;
				if (fmarkers.Count == 0)
				{
					fmarkers.Add(newmarker1);
					fmarkers.Add(newmarker2);
					return nomatch;
				}
				if (nomatch)
				{
					for (int i = 0; i < fmarkers.Count; ++i)
					{
						nomatch = false;
						for(int j = 0; j < newmarker1.Count; ++j)
						{
							if(!fmarkers[i].Contains(newmarker1[j]))
							{
								nomatch = true;
								break;
							}
						}
						if (nomatch)
						{
							if(fmarkers[i].Count <= 2)
							{
								nomatch = false;
								for (int j = 0; j < newmarker2.Count; ++j)
								{
									if (!fmarkers[i].Contains(newmarker2[j]))
									{
										nomatch = true;
										break;
									}
								}
							}
						}
					}
				}
				if (nomatch)
				{
					fmarkers.Add(newmarker1);
					fmarkers.Add(newmarker2);
				}
				else
				{
					Console.WriteLine("Formation rejected for duplication");
				}
				return nomatch;
			}

			public int RollFormation(MT19337 rng, Formation f, ref int zone1, ref int zone2, ref int picker1, ref int picker2, ref int stage)
			{
				bool failure = false;
				switch (picker1) // B-Side is created first
				{
					case 0:
						failure = ENF_SingleMon(rng, f, ref zone1, 0, true);
						break;
					case 1:
						failure = ENF_MixedTwo(rng, f, ref zone1, true);
						break;
					case 2:
						failure = ENF_BigAndSmall(rng, f, ref zone2, true);
						break;
					case 3:
						break;
				}
				switch (picker2) // then A-Side is created
				{
					case 0:
						failure = ENF_SingleMon(rng, f, ref zone2, 1, false);
						break;
					case 1:
						failure = ENF_MixedTwo(rng, f, ref zone2, false);
						break;
					case 2:
						failure = ENF_BigAndSmall(rng, f, ref zone2, false);
						break;
					case 3:
						break;
				}
				return 0;
			}

			public bool ENF_SingleMon(MT19337 rng, Formation f, ref int tier, int p, bool sideB)
			{
				int o = sideB ? 4 : 0; // set offset so we increase the monmax of 4-5 if we are rolling for B-Side formations
				int limit = GetMonLimit(f.id[p], f.shape);
				int maxXPgoal = rng.Between(zonexpreqs[tier, 1], zonexpreqs[tier, 2]);
				f.monMax[p + o] = maxXPgoal / enemyXPLUT[f.id[p]];
				if (f.monMax[p + o] < 1)
					f.monMax[p + o] = 1;
				if (f.monMax[p + o] > limit)
					f.monMax[p + o] = limit;
				f.monMin[p + o] = zonexpreqs[tier, 0] / enemyXPLUT[f.id[p]] + 1;
				if (f.monMin[p + o] * 3 < f.monMax[p + o])
					f.monMin[p + o] = f.monMax[p + o] / 3 + 1;
				if (f.monMin[p + o] > f.monMax[p + o])
					f.monMin[p + o] = f.monMax[p + o];
				return false;
			}

			public bool ENF_MixedTwo(MT19337 rng, Formation f, ref int tier, bool sideB)
			{
				int o = sideB ? 4 : 0; // set offset so we increase the monmax of 4-5 if we are rolling for B-Side formations
				int limit = GetMonLimit(f.Top, f.shape);
				int modifier = Large(f.Top) ? 1 : 2;
				int maxXPgoal = rng.Between(zonexpreqs[tier, 1], zonexpreqs[tier, 2]);
				f.monMax[o] = modifier;
				f.monMin[o] = modifier;
				f.monMax[o + 1] = modifier;
				f.monMin[o + 1] = modifier;
				int moncount = modifier * 2;
				int maxxp = modifier * (enemyXPLUT[f.Top] + enemyXPLUT[f.id[1]]);
				List<int> validIDs = new List<int> { 0, 1 };
				if(!sideB && rng.Between(0,2) > 0)
				{
					if (f.id[2] != 0xFF)
						if(Large(f.id[2]) == Large(f.Top))
							validIDs.Add(2);
					if (f.id[3] != 0xFF)
						if(Large(f.id[3]) == Large(f.Top))
							validIDs.Add(3);
				}
				while (maxxp < maxXPgoal && moncount < limit && validIDs.Count > 0)
				{
					int p = validIDs.PickRandom(rng);
					if (maxxp + enemyXPLUT[f.id[p]] <= zonexpreqs[tier, 2])
					{
						f.monMax[p + o]++;
						maxxp += enemyXPLUT[f.id[p]];
						moncount++;
						if (maxxp < zonexpreqs[tier, 0])
							f.monMin[p + o]++;
					}
					else
					{
						validIDs.Remove(p);
					}
				}
				if (f.monMin[o] * 3 < f.monMax[o])
					f.monMin[o] = f.monMax[o] / 3 + 1;
				if (f.monMin[o] > f.monMax[o])
					f.monMin[o] = f.monMax[o];
				if (f.monMin[o + 1] * 3 < f.monMax[o + 1])
					f.monMin[o + 1] = f.monMax[o + 1] / 3 + 1;
				if (f.monMin[o + 1] > f.monMax[o + 1])
					f.monMin[o + 1] = f.monMax[o + 1];
				return false;
			}

			public bool ENF_PartyMix(MT19337 rng, Formation f, ref int tier)
			{
				return false;
			}

			public bool ENF_BigAndSmall(MT19337 rng, Formation f, ref int tier, bool sideB)
			{
				int o = sideB ? 4 : 0; // set offset so we increase the monmax of 4-5 if we are rolling for B-Side formations
				int maxXPgoal = rng.Between(zonexpreqs[tier, 1], zonexpreqs[tier, 2]);
				int limit1 = GetMonLimit(f.Top, f.shape);
				int limit2 = GetMonLimit(f.id[1], f.shape);
				int mod1 = Large(f.Top) ? 1 : 2;
				int mod2 = Large(f.id[1]) ? 1 : 2;
				f.monMax[o] = mod1;
				f.monMin[o] = mod1;
				f.monMax[o + 1] = mod2;
				f.monMin[o + 1] = mod2;
				int smalllimit = 4;
				int largelimit = 1;
				int maxxp = f.monMax[o] * mod1 + f.monMax[o + 1] * mod2;
				List<int> validIDs = new List<int> { 0, 1 };
				if (!sideB && rng.Between(0, 1) > 0)
				{
					if (f.id[2] != 0xFF)
						validIDs.Add(2);
					if (f.id[3] != 0xFF)
						validIDs.Add(3);
				}
				while (maxxp < maxXPgoal)
				{
					int p = validIDs.PickRandom(rng);
					if (maxxp + enemyXPLUT[f.id[p]] <= zonexpreqs[tier, 2])
					{
						if ((Large(f.id[p]) && largelimit > 0) || (Small(f.id[p]) && smalllimit > 0))
						{
							f.monMax[p + o]++;
							maxxp += enemyXPLUT[f.id[p]];
							if (maxxp < zonexpreqs[tier, 0])
								f.monMin[p + o]++;
							if (Large(f.id[p]))
								largelimit--;
							else
								smalllimit--;
						}
						else
							validIDs.Remove(p);
					}
					else
					{
						validIDs.Remove(p);
					}
					if (validIDs.Count == 0)
						break;
					if (smalllimit + largelimit == 0)
						break;
				}
				if (f.monMin[o] * 3 < f.monMax[o])
					f.monMin[o] = f.monMax[o] / 3 + 1;
				if (f.monMin[o] > f.monMax[o])
					f.monMin[o] = f.monMax[o];
				if (f.monMin[o + 1] * 3 < f.monMax[o + 1])
					f.monMin[o + 1] = f.monMax[o + 1] / 3 + 1;
				if (f.monMin[o + 1] > f.monMax[o + 1])
					f.monMin[o + 1] = f.monMax[o + 1];
				return false;
			}

			public void ENF_Imps(MT19337 rng)
			{
				Formation f = new Formation();
				f.shape = 0x00;
				f.tileset = 0x00;
				f.pal1 = 0x00;
				f.pal2 = 0x01;
				f.id[0] = 0x00;
				f.id[1] = 0x01;
				f.id[2] = 0x02;
				ENF_AssignPicAndPaletteBytes(f);
				f.monMin[0] = 1;
				f.monMax[0] = 2;
				f.monMin[1] = 2;
				f.monMax[1] = 4;
				f.monMin[2] = 2;
				f.monMax[2] = 3;
				f.monMax[4] = rng.Between(4, 6);
				f.monMin[4] = rng.Between(3, 4);
				f.surprise = 4;
				f.unrunnable = false;
				LogAndCompressFormation(f, 0x00);
				ENF_ApproveFormation(f, -1, -1);
				zone[0].Add(0x00);
			}

			public void ENF_Wizards(MT19337 rng)
			{
				Formation f = new Formation();
				f.shape = 0x00;
				f.tileset = 0x0B;
				f.pal1 = 0x32;
				f.pal2 = 0x33;
				f.id[0] = 0x67;
				f.id[1] = 0x68;
				ENF_AssignPicAndPaletteBytes(f);
				f.monMin[0] = 2;
				f.monMax[0] = 4;
				f.monMax[5] = rng.Between(6, 9);
				f.monMin[5] = rng.Between(3, 5);
				f.surprise = 4;
				f.unrunnable = true;
				LogAndCompressFormation(f, 0x01);
				ENF_ApproveFormation(f, -1, -1);
				zone[3].Add(0x01);
				zone[6].Add(0x81);
			}

			public void ENF_Phantom(MT19337 rng)
			{
				Formation f = new Formation();
				f.shape = 0x02;
				f.tileset = 0x04;
				f.pal1 = 0x16;
				f.pal2 = 0x07;
				f.id[0] = 0x33;
				f.id[1] = 0x2A;
				ENF_AssignPicAndPaletteBytes(f);
				f.monMin[0] = 1;
				f.monMax[0] = 1;
				f.monMax[5] = 5;
				f.monMin[5] = 2;
				f.surprise = 4;
				f.unrunnable = true;
				LogAndCompressFormation(f, 0x03);
				ENF_ApproveFormation(f, -1, -1);
				zone[8].Add(0x83);
			}

			public void ENF_Eye(MT19337 rng)
			{
				Formation f = new Formation();
				f.shape = 0x01;
				f.tileset = 0x04;
				f.pal1 = 0x17;
				f.pal2 = 0x17;
				f.id[0] = 0x32;
				ENF_AssignPicAndPaletteBytes(f);
				f.monMin[0] = 1;
				f.monMax[0] = 1;
				f.monMin[4] = 2;
				f.monMax[4] = 3;
				f.surprise = 4;
				f.unrunnable = true;
				LogAndCompressFormation(f, 0x02);
				ENF_ApproveFormation(f, -1, -1);
				zone[8].Add(0x02);
				zone[9].Add(0x82);
			}

			public void ENF_VampireZombieD(MT19337 rng)
			{
				Formation f = new Formation();
				f.shape = 0x02;
				f.tileset = 0x06;
				f.pal1 = 0x1F;
				f.pal2 = 0x16;
				f.id[0] = 0x3C;
				f.id[1] = 0x44;
				ENF_AssignPicAndPaletteBytes(f);
				f.monMin[0] = 1;
				f.monMax[0] = 1;
				f.monMin[5] = 1;
				f.monMax[5] = 2;
				f.surprise = 4;
				f.unrunnable = true;
				LogAndCompressFormation(f, 0x7C);
				ENF_ApproveFormation(f, -1, -1);
				zone[6].Add(0xFC);
			}

			public void ENF_Warmech(MT19337 rng)
			{
				Formation f = new Formation();
				f.shape = 0x02;
				f.tileset = 0x0C;
				f.pal1 = 0x2F;
				f.pal2 = 0x2E;
				f.id[0] = 0x76;
				f.id[1] = 0x73;
				ENF_AssignPicAndPaletteBytes(f);
				f.monMin[0] = 1;
				f.monMax[0] = 1;
				f.monMax[5] = 2;
				f.monMin[5] = 1;
				f.surprise = 75;
				f.unrunnable = false;
				LogAndCompressFormation(f, 0x56);
				ENF_ApproveFormation(f, -1, -1);
				zone[9].Add(0xD6);
			}

			public void ENF_Pirates(MT19337 rng)
			{
				Formation f = new Formation();
				f.shape = 0x00;
				f.tileset = 0x01;
				f.pal1 = 0x08;
				f.pal2 = 0x0B;
				f.id[0] = 0x0D;
				f.id[1] = 0x0E;
				f.id[2] = 0x0F;
				ENF_AssignPicAndPaletteBytes(f);
				f.monMin[2] = 9;
				f.monMax[2] = 9;
				f.monMin[4] = 1;
				f.monMax[4] = 2;
				f.monMax[5] = 8;
				f.monMin[5] = 8;
				f.surprise = 4;
				f.unrunnable = true;
				LogAndCompressFormation(f, 0x7E);
				ENF_ApproveFormation(f, -1, -1);
				zone[8].Add(0xFE);
			}

			public void ENF_Garland(MT19337 rng)
			{
				Formation f = new Formation();
				f.shape = 0x02;
				f.tileset = 0x0B;
				f.pal1 = 0x13;
				f.pal2 = 0x2E;
				f.id[0] = 0x69;
				f.id[1] = 0x6E;
				ENF_AssignPicAndPaletteBytes(f);
				f.monMin[0] = 1;
				f.monMax[0] = 1;
				f.monMax[5] = 1;
				f.monMin[5] = 2;
				f.surprise = 4;
				f.unrunnable = true;
				LogAndCompressFormation(f, 0x7F);
				ENF_ApproveFormation(f, -1, -1);
				zone[10].Add(0xFF);
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
				traptile_formations[(int)TrapTiles.TRAP_ZOMBIE_D] = 0xFC;
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
				traptile_formations[(int)TrapTiles.TRAP_EYE] = 0x02;
				traptile_formations[(int)TrapTiles.TRAP_WATERFALL_MUMMIES] = zone[8][3];
				traptile_formations[(int)TrapTiles.TRAP_WIZARDS] = 0x01;
				traptile_formations[(int)TrapTiles.TRAP_WIZARDS2] = 0x01;
				traptile_formations[(int)TrapTiles.TRAP_COBRAS] = zone[3][2];
				traptile_formations[(int)TrapTiles.TRAP_BLUE_D] = zone[9][0];
				traptile_formations[(int)TrapTiles.TRAP_SLIMES] = zone[9][1];
				traptile_formations[(int)TrapTiles.TRAP_PHANTOM] = 0x03;
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
				PutD(0x120, 0x80); PutD(0x121, 0x80); PutD(0x122, 0x80); PutD(0x123, 0x80); PutD(0x160, 0x80); PutD(0x161, 0x80); PutD(0x162, 0x80); PutD(0x163, 0x80);
				PutD(0x39E, 0x56); // replace 7th encounter on sky 5F with warmech
			}
		}

		public void DoEnemizer(MT19337 rng, bool enemies, bool formations, bool battledomains)
		{
			Enemizer en = new Enemizer();
			// load vanilla values from ROM into the enemizer
			en.PutAllFormationData(Get(FormationDataOffset, FormationDataSize * FormationDataCount));
			en.PutAllDomainData(Get(ZoneFormationsOffset, ZoneFormationsSize * ZoneCount));
			for (int i = 0; i < EnemyCount; ++i)
			{
				var jumbledXP = Get(EnemyOffset + i * EnemySize, 2);
				en.enemyXPLUT[i] = jumbledXP[1] * 256 + jumbledXP[0];
			}
			for (int i = 0; i < FormationCount; ++i)
			{
				byte[] formationData = Get(FormationDataOffset + FormationDataSize * i, 16).ToBytes();
				en.ReadEnemyDataFromFormation(en.decompressFormation(formationData)); // read information about enemies from the ROM
			}
			en.PurgeIDFromEnemyTilesetList(Enemy.Pirate);
			en.PurgeIDFromEnemyTilesetList(Enemy.Phantom);
			en.PurgeIDFromEnemyTilesetList(Enemy.Astos);
			en.PurgeIDFromEnemyTilesetList(Enemy.Garland);
			en.PurgeIDFromEnemyTilesetList(Enemy.WarMech); // purging enemies from the generic enemy lists that we don't want to appear outside of set battles
			if (enemies)
			{
				// do enemizer stuff
				formations = true; // must use formation generator with enemizer
			}
			if(formations)
			{
				if(en.DoFormations(rng))
				{
					battledomains = true; // must use domain generator with formation shuffle
					Put(FormationsOffset, en.GetFormationData());
				}
					
				else
				{
					Console.WriteLine("Fission Mailed - Abort Formation Shuffle");
				}
			}
			if (battledomains)
			{
				// This code is partially lifted from ShuffleTrapTiles
				Data[0x7CDC5] = 0xD0; // changes the game's programming

				bool IsBattleTile(Blob tuple) => tuple[0] == 0x0A;
				bool IsRandomBattleTile(Blob tuple) => IsBattleTile(tuple) && (tuple[1] & 0x80) != 0x00;
				bool IsNonBossTrapTile(Blob tuple) => IsBattleTile(tuple) && tuple[1] > 0 && tuple[1] < FirstBossEncounterIndex && tuple[1] > LastBossEncounterIndex;

				var tilesets = Get(TilesetDataOffset, TilesetDataCount * TilesetDataSize * TilesetCount).Chunk(TilesetDataSize).ToList();
				tilesets.ForEach(tile => { if (IsRandomBattleTile(tile)) tile[1] = 0x00; });
				Put(TilesetDataOffset, tilesets.SelectMany(tileset => tileset.ToBytes()).ToArray());// set all random battle tiles to zero
				/* we will eventually allow the formation shuffle to judge what should be at which tile based on the ROM's encounter, but for now we will use hardcoded values
				var traps_addresses = Enumerable.Range(0, tilesets.Count).Where(addr => IsNonBossTrapTile(tilesets[addr])).ToList();
				var traps_formation = tilesets.Where(IsNonBossTrapTile).Select(trap => trap[1]).ToList();
				// feed the information to Enemizer
				en.traptile_addresses = traps_addresses;
				en.traptile_formations = traps_formation; */

				en.DoDomains(rng);
				// write domains information
				Put(ZoneFormationsOffset, en.GetDomainData());

				// write trap tile information
				for(int i = 0; i < en.traptile_addresses.Length; ++i)
				{
					Data[TilesetDataOffset + en.traptile_addresses[i]] = en.traptile_formations[i];
				}
			}
		}
	}
}
