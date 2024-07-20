using FF1Lib.Sanity;
using RomUtilities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FF1Lib
{
	public partial class FF1Rom
	{
		public enum DamageTileStrategies
		{
			[Description("Cond. Neighbors")]
			ConditionalNeighbors,
			[Description("Perlin Noise")]
			PerlinNoise,
			[Description("Random")]
			Random
		}
		private async Task DamageTilesHack(Flags flags, Overworld overworld)
		{
			int damageTileAmount = 1;

			// Adjustable lava damage - run if anything other than the default of 1 damage
			if ((int)flags.DamageTileLow != 1 || (int)flags.DamageTileHigh != 1)
			{
				damageTileAmount = rng.Between(flags.DamageTileLow, flags.DamageTileHigh);
				// Always write at vanilla location, it will be ignored if other flags are enabled
				PutInBank(0x1F, 0xC86C, Blob.FromHex($"{(damageTileAmount + 1):X2}"));
				PutInBank(0x1F, 0xC874, Blob.FromHex($"{damageTileAmount:X2}"));
			}

			if ((bool)flags.OWDamageTiles || flags.DesertOfDeath)
			{
				EnableDamageTile(overworld);
			}
			await AddDamageTilesToMaps(flags, rng);
			DamageTilesKillAndCanBeResisted(damageTileAmount, (bool)flags.ArmorResistsDamageTileDamage && !(bool)flags.ArmorCrafter, (bool)flags.DamageTilesKill, flags.SaveGameWhenGameOver);
		}

		public void EnableDamageTile(Overworld overworld)
		{
			// Allow tiles to do Walk Damage, see 1E_B000_DoWalkDamage.asm
			PutInBank(0x1E, 0xB000, Blob.FromHex("A542C901D00E20FBC7A54429E0C9E0D0034CDEC760"));
			PutInBank(0x1F, 0xC33C, Blob.FromHex("A91E2003FE4C00B0"));

			PutInBank(0x0E, 0xB267, Blob.FromHex("E0")); // Expand OWTP_SPEC_MASK to 0b1110_0000
			PutInBank(0x1F, 0xC4DA, Blob.FromHex("E0"));
			PutInBank(0x1F, 0xC6B4, Blob.FromHex("E0"));

			PutInBank(0x1F, 0xC2EC, Blob.FromHex("E1")); // Update mask for docking

			List<int> owDamageTileList = new() { 0x45, 0x55, 0x62, 0x63, 0x72, 0x73 };

			foreach (var tile in owDamageTileList)
			{
				var tileProperty = overworld.TileSet.Tiles[tile];
				tileProperty.PropertyType = (byte)(tileProperty.PropertyType | 0b1110_0000);
			}
		}

		public void DamageTilesKillAndCanBeResisted(int damageTileAmount, bool armorResistEnabled, bool damageTileKill, bool saveonDeath)
		{
			if (!armorResistEnabled && !damageTileKill)
			{
				return;
			}

			// See 1E_B100_DamageTilesKill.asm
			string saveOnDeathJump = saveonDeath ? "EAEAEA" : "4C12C0";
			string armorResists = armorResistEnabled ? "00" : "01";
			string setDeadStatus = damageTileKill ? "A9019D0161A9009D0A614C6BB1" : "EAEAEAEAEAA9019D0A614C6CB1";

			//Hack this out
			PutInBank(0x1E, 0xB100, Blob.FromHex(
				// AssignMapTileDamage
				$"A200A000BD0161C901F060" +
				// Armor Resist Check
				$"A9{armorResists}D043A5F2C903F03D" +
				$"A548C90EF013C921F00FC922F00BC923F007C924F0034C38B1A98820FDB1D01D4C6CB1C90FF00CC925F008C926F004C927D00AA98720FDB1D0034C6CB1" +
				// AssignDamage
				$"BD0B61D021BD0A61C9{(damageTileAmount+1):X2}B01A{setDeadStatus}C88A186940AAD091C004F01560" +
				// DmgSubtract
				$"BD0A6138E9{damageTileAmount:X2}9D0A61BD0B61E9009D0B614C6CB1" +
				// MapGameOver
				$"A980854BA91E8557A52D6AA9009002A902851C2000FE2082D920B3D9A9064820EFD968A88898D0F6A040" +
				// WaitLoop
				$"2000FE2018D92082D9A91E8D01202089C688D0ECA952854B20EFB1A9008D00208D0120{saveOnDeathJump}A9C048A91148A98F48A9F748A91B85574C03FE" +
				// WaitForAnyInput
				$"2028D8482000FE2089C668F0F360841185108AA8B91C61C510D005A411A90060C8982904C904D0ECA411A90160"));

			PutInBank(0x1F, 0xC861, Blob.FromHex("A91E2003FE4C00B1"));
		}
		private struct DamageTileSwapConfig
		{
			public List<byte> ReplacableTiles;
			public byte DamageTile;
		}
		Dictionary<TileSets, DamageTileSwapConfig> DamageTileSwapConfigs = new Dictionary<TileSets, DamageTileSwapConfig>()
		{
			{ TileSets.Castle,							new DamageTileSwapConfig {ReplacableTiles = new() {0x31, 0x60},	DamageTile = 0x7E} },
			{ TileSets.MatoyaDwarfCardiaIceWaterfall,	new DamageTileSwapConfig {ReplacableTiles = new() {0x31, 0x49},	DamageTile = 0x39} },
			{ TileSets.EarthTitanVolcano,				new DamageTileSwapConfig {ReplacableTiles = new() {0x41},		DamageTile = 0x3D} },
			{ TileSets.SkyCastle,						new DamageTileSwapConfig {ReplacableTiles = new() {0x4B},		DamageTile = 0x7E} },
			{ TileSets.ToFSeaShrine,					new DamageTileSwapConfig {ReplacableTiles = new() {0x55, 0x31},	DamageTile = 0x7E} },
			{ TileSets.MarshMirage,						new DamageTileSwapConfig {ReplacableTiles = new() {0x40},		DamageTile = 0x7E} },
			{ TileSets.ToFR,							new DamageTileSwapConfig {ReplacableTiles = new() {0x5C, 0x31},	DamageTile = 0x7E} },
			{ TileSets.Town,                            new DamageTileSwapConfig {
				ReplacableTiles = new() {0x00, 0x01, 0x02, 0x10, 0x11, 0x12, 0x13, 0x14, 0x15, 0x16, 0x2A, 0x2B, 0x2C}, DamageTile = 0x32} },
		};
		
		private void CreateDamageTile(TileSets tileSet, List<byte> tileGraphic, byte attribute)
		{
			byte damageTileID = DamageTileSwapConfigs[tileSet].DamageTile;
			TileSM damageTile = TileSetsData[(int)tileSet].Tiles[damageTileID];

			damageTile.TileGraphic = tileGraphic;  // Indexes (topleft,topright,bottomleft,bottomright) to chr data
			damageTile.Attribute = attribute;  // change palette for tile
			damageTile.PropertyType = (byte)TilePropFunc.TP_SPEC_DAMAGE;  // make tile do damage
		}
		public async Task AddDamageTilesToMaps(Flags flags, MT19337 rng)
		{
			if (!(bool)flags.AddDamageTiles)
			{
				return;
			}
			Dictionary<MapIndex, TileSets> mapsCastles = new()
			{
				{ MapIndex.ConeriaCastle1F, TileSets.Castle },
				{ MapIndex.ConeriaCastle2F, TileSets.Castle },
				{ MapIndex.CastleOrdeals1F, TileSets.Castle },
				{ MapIndex.CastleOrdeals2F, TileSets.Castle },
				{ MapIndex.CastleOrdeals3F, TileSets.Castle },
				{ MapIndex.ElflandCastle,   TileSets.Castle },
				{ MapIndex.NorthwestCastle, TileSets.Castle },
			};
			Dictionary<MapIndex, TileSets> mapsTowns = new()
			{
				{ MapIndex.ConeriaTown,     TileSets.Town },
				{ MapIndex.Pravoka,         TileSets.Town },
				{ MapIndex.Elfland,         TileSets.Town },
				{ MapIndex.CrescentLake,    TileSets.Town },
				{ MapIndex.Onrac,           TileSets.Town },
				{ MapIndex.Gaia,            TileSets.Town },
				{ MapIndex.Lefein,          TileSets.Town },
			};
			Dictionary<MapIndex, TileSets> mapsDungeons = new()
			{
				{ MapIndex.EarthCaveB1,     TileSets.EarthTitanVolcano },
				{ MapIndex.EarthCaveB2,     TileSets.EarthTitanVolcano },
				{ MapIndex.EarthCaveB3,     TileSets.EarthTitanVolcano },
				{ MapIndex.EarthCaveB4,     TileSets.EarthTitanVolcano },
				{ MapIndex.EarthCaveB5,     TileSets.EarthTitanVolcano },
				{ MapIndex.SeaShrineB1,     TileSets.ToFSeaShrine },
				{ MapIndex.SeaShrineB2,     TileSets.ToFSeaShrine },
				{ MapIndex.SeaShrineB3,     TileSets.ToFSeaShrine },
				{ MapIndex.SeaShrineB4,     TileSets.ToFSeaShrine },
				{ MapIndex.SeaShrineB5,     TileSets.ToFSeaShrine },
				{ MapIndex.MirageTower1F,   TileSets.MarshMirage },
				{ MapIndex.MirageTower2F,   TileSets.MarshMirage },
				{ MapIndex.MirageTower3F,   TileSets.MarshMirage },
				{ MapIndex.SkyPalace1F,     TileSets.SkyCastle },
				{ MapIndex.SkyPalace2F,     TileSets.SkyCastle },
				{ MapIndex.SkyPalace3F,     TileSets.SkyCastle },
				{ MapIndex.SkyPalace4F,     TileSets.SkyCastle },
			};
			Dictionary<MapIndex, TileSets> mapsCaves = new()
			{
				{ MapIndex.Cardia,          TileSets.MatoyaDwarfCardiaIceWaterfall },
				{ MapIndex.BahamutCaveB1,   TileSets.MatoyaDwarfCardiaIceWaterfall },
				{ MapIndex.BahamutCaveB2,   TileSets.MatoyaDwarfCardiaIceWaterfall },
				{ MapIndex.Waterfall,       TileSets.MatoyaDwarfCardiaIceWaterfall },
				{ MapIndex.DwarfCave,       TileSets.MatoyaDwarfCardiaIceWaterfall },
				{ MapIndex.MatoyasCave,     TileSets.MatoyaDwarfCardiaIceWaterfall },
				{ MapIndex.SardasCave,      TileSets.MatoyaDwarfCardiaIceWaterfall },
				{ MapIndex.IceCaveB1,       TileSets.MatoyaDwarfCardiaIceWaterfall },
				{ MapIndex.IceCaveB2,       TileSets.MatoyaDwarfCardiaIceWaterfall },
				{ MapIndex.TitansTunnel,    TileSets.EarthTitanVolcano },
				{ MapIndex.MarshCaveB1,     TileSets.MarshMirage },
				{ MapIndex.MarshCaveB2,     TileSets.MarshMirage },
				{ MapIndex.MarshCaveB3,     TileSets.MarshMirage },
			};
			Dictionary<MapIndex, TileSets> mapsTof = new()
			{
				{ MapIndex.TempleOfFiends,					TileSets.ToFSeaShrine },
				{ MapIndex.TempleOfFiendsRevisited1F,		TileSets.ToFR },
				{ MapIndex.TempleOfFiendsRevisited2F,		TileSets.ToFR },
				{ MapIndex.TempleOfFiendsRevisited3F,		TileSets.ToFR },
				{ MapIndex.TempleOfFiendsRevisitedEarth,	TileSets.ToFR },
				{ MapIndex.TempleOfFiendsRevisitedFire,  	TileSets.ToFR },
				{ MapIndex.TempleOfFiendsRevisitedWater,	TileSets.ToFR },
				{ MapIndex.TempleOfFiendsRevisitedAir,  	TileSets.ToFR },
				{ MapIndex.TempleOfFiendsRevisitedChaos,	TileSets.ToFR },
			};

			Dictionary<MapIndex, TileSets> mapsToDamage = new();

			if ((bool)flags.DamageTilesCastles)
			{
				foreach (var kv in mapsCastles) mapsToDamage.Add(kv.Key, kv.Value);
				CreateDamageTile(TileSets.Castle, new() { 0x7F, 0x7F, 0x7F, 0x7F }, 0xFF);
				// overwrite unused tile chr data for use with new damage tiles
				//Put(0xCFF0, Blob.FromHex("870F1E3C78F0E1C3F9F3E7CF9F3F7EFC"));  // Castle DW style
				Put(0xCFF0, Blob.FromHex("01004808222010020040000000000200"));  // Castle
			}
			if ((bool)flags.DamageTilesCaves)
			{
				foreach (var kv in mapsCaves) mapsToDamage.Add(kv.Key, kv.Value);
				CreateDamageTile(TileSets.MarshMirage, new() { 0x7F, 0x7F, 0x7F, 0x7F }, 0xAA);
				TileSetsData[(int)TileSets.MatoyaDwarfCardiaIceWaterfall].Tiles[0x13].PropertyType = 0x0C; //Matoya's skull decorations
				Put(0xE7F0, Blob.FromHex("01004808222010020040000000000200"));  // Marsh/mirage
			}
			if ((bool)flags.DamageTilesTowns)
			{
				foreach (var kv in mapsTowns) mapsToDamage.Add(kv.Key, kv.Value);
				CreateDamageTile(TileSets.Town, new() { 0x3A, 0x3A, 0x3A, 0x3A }, 0x00);
			}
			if ((bool)flags.DamageTilesDungeons)
			{
				foreach (var kv in mapsDungeons) mapsToDamage.Add(kv.Key, kv.Value);
				CreateDamageTile(TileSets.ToFSeaShrine, new() { 0x7F, 0x7F, 0x7F, 0x7F }, 0xAA);
				CreateDamageTile(TileSets.SkyCastle,	new() { 0x7F, 0x7F, 0x7F, 0x7F }, 0xFF);
				CreateDamageTile(TileSets.MarshMirage,  new() { 0x7F, 0x7F, 0x7F, 0x7F }, 0xAA);
				Put(0xE7F0, Blob.FromHex("01004808222010020040000000000200"));  // Marsh/mirage
				Put(0xF7F0, Blob.FromHex("01004808222010020040000000000200"));  // Sky
				Put(0xEFF0, Blob.FromHex("01004808222010020040000000000200"));  // Sea
			}
			if ((bool)flags.DamageTilesTof)
			{
				foreach (var kv in mapsTof) mapsToDamage.Add(kv.Key, kv.Value);
				CreateDamageTile(TileSets.ToFSeaShrine, new() { 0x7F, 0x7F, 0x7F, 0x7F }, 0xAA);
				CreateDamageTile(TileSets.ToFR,			new() { 0x7F, 0x7F, 0x7F, 0x7F }, 0xFF);
				Put(0xEFF0, Blob.FromHex("01004808222010020040000000000200"));  // Sea
				Put(0xFFF0, Blob.FromHex("01004808222010020040000000000200"));  // ToFR
			}

			Dictionary<MapIndex, List<SCCoords>> ReplacableCoordsPerMap = new Dictionary<MapIndex, List<SCCoords>>();
			foreach (var (mapIndex, tileset) in mapsToDamage)
			{
				List<SCCoords> replacableCoords = new();
				List<Byte> candidateTiles = DamageTileSwapConfigs[tileset].ReplacableTiles;
				for (int x = 0; x < 63; x++)
				{
					for (int y = 0; y < 63; y++)
					{
						if (candidateTiles.Contains(Maps[mapIndex].Map[y, x]))
						{
							replacableCoords.Add(new SCCoords(x, y));
						}
					}
				}
				ReplacableCoordsPerMap[mapIndex] = replacableCoords;
			}

			if (flags.DamageTileStrategy == DamageTileStrategies.Random)
			{
				foreach (var (mapIndex, tileset) in mapsToDamage)
				{
					//Get list of candidate coordinates to become damage tiles
					List<SCCoords> replacableCoords = ReplacableCoordsPerMap[mapIndex];
					replacableCoords.Shuffle(rng);

					//Replace a set number of tiles in the map with damage tiles
					for (int i = 0; i < (replacableCoords.Count / 3) - 1; i++)
					{
						byte damageTile = DamageTileSwapConfigs[tileset].DamageTile;
						SCCoords coords = replacableCoords[i];
						Maps[mapIndex].Map[coords.Y, coords.X] = damageTile;
					}
				}
			}
			if (flags.DamageTileStrategy == DamageTileStrategies.PerlinNoise)
			{
				await Progress("Adding damage tiles", mapsToDamage.Count+1);

				foreach (var (mapIndex, tileset) in mapsToDamage)
				{
					await Progress();
					List<SCCoords> replacableCoords = ReplacableCoordsPerMap[mapIndex];
					PerlinNoise perlinNoise = new PerlinNoise(rng);
					bool[,] noiseMask = await CreatePerlinNoiseMask(perlinNoise);

					for (int x = 0; x < 64; x++)
					{
						for (int y = 0; y < 64; y++)
						{
							SCCoords xy = new SCCoords(x, y);
							if (replacableCoords.Contains(xy))
							{
								if (noiseMask[x,y])
								{
									byte damageTile = DamageTileSwapConfigs[tileset].DamageTile;
									Maps[mapIndex].Map[xy.Y, xy.X] = damageTile;
								}
							}
						}
					}
				}
			}

			if (flags.DamageTileStrategy == DamageTileStrategies.ConditionalNeighbors)
			{
				await Progress("Adding damage tiles", (mapsToDamage.Count / 5)+1);
				int curMap = 1;
				foreach (var (mapIndex, tileset) in mapsToDamage)
				{
					
					if (curMap % 5 == 0) await Progress();
					//Get list of candidate coordinates to become damage tiles
					List<SCCoords> replacableCoords = ReplacableCoordsPerMap[mapIndex];
					replacableCoords.Shuffle(rng);

					//Replace a set number of tiles in the map with damage tiles
					for (int i = 0; i < (replacableCoords.Count / 3) - 1; i++)
					{
						byte damageTile = DamageTileSwapConfigs[tileset].DamageTile;
						SCCoords coords = replacableCoords[i];
						Maps[mapIndex].Map[coords.Y, coords.X] = damageTile;

						//Preferentially add neighbors for a more natural pattern
						List<SCCoords> neighbors = new() { coords.SmLeft, coords.SmRight, coords.SmUp, coords.SmDown };
						foreach (SCCoords nCoords in neighbors)
						{
							if (replacableCoords.Contains(nCoords) && rng.Between(0, 1) == 1)
							{
								Maps[mapIndex].Map[nCoords.Y, nCoords.X] = damageTile;
								i++;
							}
						}
					}
					curMap++;
				}
			}
		}
		private async Task<bool[,]> CreatePerlinNoiseMask(PerlinNoise perlinNoise)
		{
			bool[,] mask = new bool[64, 64];
			await Task.Run(() =>
			{
				for (int x = 0; x < 64; x++)
				{
					for (int y = 0; y < 64; y++)
					{
						// scaling the coordinates by 3 seems to be a good tradeoff between detail and feature coherence
						mask[x, y] = perlinNoise.GetBool(((float)x) / 3, ((float)y) / 3, 0.5f);
					}
				}
			});
			return mask;
		}
	}
}
