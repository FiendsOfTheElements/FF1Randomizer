using System.Diagnostics;
using FF1Lib.Sanity;
using FF1Lib.Procgen;
using System;

namespace FF1Lib
{
	public struct MapEdit
	{
		public byte X { get; set; }
		public byte Y { get; set; }
		public byte Tile { get; set; }
	}

	public partial class OverworldMap
	{
		private readonly FF1Rom _rom;
		private readonly Dictionary<Palette, Blob> _palettes;

		private List<List<byte>> _decompressedMap;

		public const int MapPaletteOffset = 0x2000;
		public const int MapPaletteSize = 48;
		public const int MapCount = 64;

		public List<List<byte>> MapBytes { get => _decompressedMap; }

		public OverworldMap(FF1Rom rom, IMapEditFlags flags)
		{
			_rom = rom;
			_palettes = GeneratePalettes(rom.Get(MapPaletteOffset, MapCount * MapPaletteSize).Chunk(MapPaletteSize));

			_decompressedMap = DecompressMapRows(GetCompressedMapRows());

			if (flags.GameMode == GameModes.Standard && flags.OwMapExchange == OwMapExchanges.None) {
			    // Can only apply map edits to vanilla-ish maps

			if ((bool)flags.MapOnracDock)
			{
				MapEditsToApply.Add(OnracDock);
			}
			if ((bool)flags.MapMirageDock)
			{
				MapEditsToApply.Add(MirageDock);
			}
			if ((bool)flags.MapAirshipDock && !flags.DisableOWMapModifications)
			{
				MapEditsToApply.Add(AirshipDock);
			}
			if ((bool)flags.MapBahamutCardiaDock && !flags.DisableOWMapModifications)
			{
				MapEditsToApply.Add(BahamutCardiaDock);
			}
			if ((bool)flags.MapLefeinRiver && !flags.DisableOWMapModifications) {
			    MapEditsToApply.Add(LefeinRiverDock);
			}
			if ((bool)flags.MapBridgeLefein && !flags.DisableOWMapModifications) {
			    MapEditsToApply.Add(BridgeToLefein);
			}
			if ((bool)flags.MapGaiaMountainPass && !flags.DisableOWMapModifications) {
			    MapEditsToApply.Add(GaiaMountainPass);
			}
			if ((bool)flags.MapHighwayToOrdeals && !flags.DisableOWMapModifications)
			{
				MapEditsToApply.Add(HighwayToOrdeals);
			}
			if ((bool)flags.MapRiverToMelmond && !flags.DisableOWMapModifications) {
				MapEditsToApply.Add(RiverToMelmond);
			}
			if ((bool)flags.MapVolcanoIceRiver)
			{
				MapEditsToApply.Add(VolcanoIceRiver);
			}
			if ((bool)flags.MapConeriaDwarves)
			{
				MapEditsToApply.Add(ConeriaToDwarves);
			}
			}
		}
		public static Dictionary<Palette, Blob> GeneratePalettes(List<Blob> vanillaPalettes)
		{
			Dictionary<Palette, Blob> palettes = new Dictionary<Palette, Blob>();
			palettes.Add(Palette.Town, vanillaPalettes[(int)MapIndex.ConeriaTown]);
			palettes.Add(Palette.Castle, vanillaPalettes[(int)MapIndex.ConeriaCastle1F]);
			palettes.Add(Palette.Greyscale, vanillaPalettes[(int)MapIndex.TempleOfFiends]);
			palettes.Add(Palette.Orange, vanillaPalettes[(int)MapIndex.EarthCaveB1]);
			palettes.Add(Palette.DarkOrange, vanillaPalettes[(int)MapIndex.EarthCaveB4]);
			palettes.Add(Palette.PaleRed, vanillaPalettes[(int)MapIndex.GurguVolcanoB1]);
			palettes.Add(Palette.Red, vanillaPalettes[(int)MapIndex.GurguVolcanoB4]);
			palettes.Add(Palette.PaleBlue, vanillaPalettes[(int)MapIndex.IceCaveB1]);
			palettes.Add(Palette.Teal, vanillaPalettes[(int)MapIndex.Cardia]);
			palettes.Add(Palette.DarkTeal, vanillaPalettes[(int)MapIndex.Waterfall]);
			palettes.Add(Palette.Purple, vanillaPalettes[(int)MapIndex.MatoyasCave]);
			palettes.Add(Palette.Yellow, vanillaPalettes[(int)MapIndex.SardasCave]);
			palettes.Add(Palette.PaleGreen, vanillaPalettes[(int)MapIndex.MarshCaveB1]);
			palettes.Add(Palette.Green, vanillaPalettes[(int)MapIndex.MarshCaveB3]);
			palettes.Add(Palette.Blue, vanillaPalettes[(int)MapIndex.SeaShrineB5]);
			palettes.Add(Palette.LightBlue, vanillaPalettes[(int)MapIndex.SeaShrineB1]);
			palettes.Add(Palette.DarkBlue, vanillaPalettes[(int)MapIndex.TitansTunnel]);
			palettes.Add(Palette.Bluescale, vanillaPalettes[(int)MapIndex.SkyPalace1F]);

			palettes.Add(Palette.YellowGreen, Blob.Concat(
				Blob.FromHex("0F3838380F3838180F0A19290F000130"),
				vanillaPalettes[(int)MapIndex.BahamutCaveB2].SubBlob(16, 16),
				Blob.FromHex("0F10170F0F0F18080F0F0B180F000130")
			));
			palettes.Add(Palette.Pink, Blob.Concat(
				Blob.FromHex("0F2424240F2424140F0414280F000130"),
				vanillaPalettes[(int)MapIndex.MarshCaveB2].SubBlob(16, 16),
				Blob.FromHex("0F10000F0F0F14250F0F04080F000130")
			));
			palettes.Add(Palette.Flame, Blob.Concat(
				Blob.FromHex("0F3838380F3838180F0616280F000130"),
				vanillaPalettes[(int)MapIndex.MirageTower2F].SubBlob(16, 16),
				Blob.FromHex("0F10170F0F0F07180F0F06180F000130")
			));

			return palettes;
		}

		public void PutPalette(OverworldTeleportIndex source, MapIndex index)
		{
			var palette = OverworldToPalette[source];

			if (index <= MapIndex.Lefein) // Towns
			{
				// Towns are given arbitrary palettes to help provide color when dungeons take their place.
				if (source != OverworldTeleportIndex.Coneria)
				{
					_rom.Put(MapPaletteOffset + (int)index * MapPaletteSize + 1, _palettes[palette].SubBlob(9, 2));
					_rom.Put(MapPaletteOffset + (int)index * MapPaletteSize + 6, _palettes[palette].SubBlob(9, 1));
					_rom.Put(MapPaletteOffset + (int)index * MapPaletteSize + 33, _palettes[palette].SubBlob(9, 2));
					_rom.Put(MapPaletteOffset + (int)index * MapPaletteSize + 38, _palettes[palette].SubBlob(9, 1));
				}
			}
			else if (index < MapIndex.EarthCaveB1) // Castles - just tint the lawns.
			{
				_rom.Put(MapPaletteOffset + (int)index * MapPaletteSize + 8, _palettes[palette].SubBlob(8, 8));
				_rom.Put(MapPaletteOffset + (int)index * MapPaletteSize + 40, _palettes[palette].SubBlob(40, 8));
			}
			else // Dungeons
			{
				_rom.Put(MapPaletteOffset + (int)index * MapPaletteSize, _palettes[palette].SubBlob(0, 16));

				// Some maps have greyscale objects (Chests / Pillars) that look wrong when tinted (usually brown).
				var paletteIndex = FixedObjectPaletteDestinations.Contains(index) ? 36 : 32;
				_rom.Put(MapPaletteOffset + (int)index * MapPaletteSize + paletteIndex, _palettes[palette].SubBlob(paletteIndex, 48 - paletteIndex));
			}
		}

        public void UpdatePalettes(OverworldTeleportIndex oti, TeleportDestination teleport)
		{
			if (teleport.OwnsPalette)
			{
				var mapIndex = teleport.Index;
				PutPalette(oti, mapIndex);

                if (ContinuedMapIndexForPalettes.TryGetValue(mapIndex, out var list))
				{
					list.ForEach(map => PutPalette(oti, map));
				}
			}
		}
		public void FixUnusedDefaultBackdrops()
		{
			Blob backdrops = _rom.Get(0x03300, 128);
			backdrops[0x2F] = (byte)Backdrop.EarthCave; // Dwarf Cave
			backdrops[0x32] = (byte)Backdrop.SeaShrine; // Matoya's Cave
			backdrops[0x3A] = (byte)Backdrop.MarshCave; // Sarda's Cave
			backdrops[0x4A] = (byte)Backdrop.IceCave; // Pravoka
			backdrops[0x4C] = (byte)Backdrop.SeaShrine; // Elfland
			backdrops[0x4D] = (byte)Backdrop.Volcano; // Melmond
			backdrops[0x4E] = (byte)Backdrop.Marsh; // Crescent Lake
			backdrops[0x5A] = (byte)Backdrop.EarthCave; // Gaia
			backdrops[0x6C] = (byte)Backdrop.Waterfall; // Bahamut's Room
			backdrops[0x6D] = (byte)Backdrop.Tower; // Lefein
			_rom.Put(0x03300, backdrops);
		}
		public static Dictionary<OverworldTeleportIndex, Palette> OverworldToPalette =
			new Dictionary<OverworldTeleportIndex, Palette>
			{
				{OverworldTeleportIndex.Coneria,            Palette.YellowGreen},
				{OverworldTeleportIndex.Pravoka,            Palette.Bluescale},
				{OverworldTeleportIndex.Elfland,            Palette.Pink},
				{OverworldTeleportIndex.Melmond,            Palette.Flame},
				{OverworldTeleportIndex.CrescentLake,       Palette.YellowGreen},
				{OverworldTeleportIndex.Gaia,               Palette.Orange},
				{OverworldTeleportIndex.Onrac,              Palette.Blue},
				{OverworldTeleportIndex.Lefein,             Palette.Purple},
				{OverworldTeleportIndex.ConeriaCastle1,     Palette.Yellow},
				{OverworldTeleportIndex.ElflandCastle,      Palette.Green},
				{OverworldTeleportIndex.NorthwestCastle,    Palette.PaleGreen},
				{OverworldTeleportIndex.CastleOrdeals1,     Palette.Greyscale},
				{OverworldTeleportIndex.TempleOfFiends1,    Palette.Greyscale},
				{OverworldTeleportIndex.DwarfCave,          Palette.DarkOrange},
				{OverworldTeleportIndex.MatoyasCave,        Palette.Purple},
				{OverworldTeleportIndex.SardasCave,         Palette.Yellow},
				{OverworldTeleportIndex.MarshCave1,         Palette.PaleGreen},
				{OverworldTeleportIndex.EarthCave1,         Palette.Orange},
				{OverworldTeleportIndex.GurguVolcano1,      Palette.Red},
				{OverworldTeleportIndex.IceCave1,           Palette.PaleBlue},
				{OverworldTeleportIndex.Cardia1,            Palette.Teal},
				{OverworldTeleportIndex.Cardia2,            Palette.Teal},
				{OverworldTeleportIndex.Cardia4,            Palette.Teal},
				{OverworldTeleportIndex.Cardia5,            Palette.Teal},
				{OverworldTeleportIndex.Cardia6,            Palette.Teal},
				{OverworldTeleportIndex.BahamutCave1,       Palette.DarkTeal},
				{OverworldTeleportIndex.Waterfall,          Palette.DarkTeal},
				{OverworldTeleportIndex.MirageTower1,       Palette.DarkOrange},
				{OverworldTeleportIndex.TitansTunnelEast,   Palette.DarkBlue},
				{OverworldTeleportIndex.TitansTunnelWest,   Palette.DarkBlue},
				{OverworldTeleportIndex.Unused1,            Palette.Greyscale},
				{OverworldTeleportIndex.Unused2,            Palette.Greyscale},
			};
		public static Dictionary<MapIndex, List<MapIndex>> ContinuedMapIndexForPalettes =
            new Dictionary<MapIndex, List<MapIndex>>
		    {
				{ MapIndex.ConeriaCastle1F, new List<MapIndex> { MapIndex.ConeriaCastle2F } },
				{ MapIndex.CastleOrdeals1F, new List<MapIndex> { MapIndex.CastleOrdeals2F, MapIndex.CastleOrdeals3F } },
				{ MapIndex.IceCaveB2, new List<MapIndex> { MapIndex.IceCaveB3 } },
				{ MapIndex.TempleOfFiends, new List<MapIndex> { MapIndex.TempleOfFiendsRevisited1F, MapIndex.TempleOfFiendsRevisited2F,
					MapIndex.TempleOfFiendsRevisited3F, MapIndex.TempleOfFiendsRevisitedEarth, MapIndex.TempleOfFiendsRevisitedFire,
					MapIndex.TempleOfFiendsRevisitedWater, MapIndex.TempleOfFiendsRevisitedAir, MapIndex.TempleOfFiendsRevisitedChaos } }
			};
		public static Dictionary<MapLocation, List<MapLocation>> ConnectedMapLocations =
            new Dictionary<MapLocation, List<MapLocation>>
		    {
				{ MapLocation.ConeriaCastle1, new List<MapLocation> { MapLocation.ConeriaCastle2, MapLocation.ConeriaCastleRoom1, MapLocation.ConeriaCastleRoom2 } },
				{ MapLocation.ElflandCastle, new List<MapLocation> { MapLocation.ElflandCastleRoom1 } },
				{ MapLocation.NorthwestCastle, new List<MapLocation> { MapLocation.NorthwestCastleRoom2 } },
				{ MapLocation.CastleOrdeals1, new List<MapLocation> { MapLocation.CastleOrdealsMaze, MapLocation.CastleOrdealsTop } },
				{ MapLocation.IceCavePitRoom, new List<MapLocation> { MapLocation.IceCave5, MapLocation.IceCaveBackExit, MapLocation.IceCaveFloater } },
				{ MapLocation.DwarfCave, new List<MapLocation> { MapLocation.DwarfCaveRoom3 } },
				{ MapLocation.MarshCaveBottom, new List<MapLocation> { MapLocation.MarshCaveBottomRoom13, MapLocation.MarshCaveBottomRoom14, MapLocation.MarshCaveBottomRoom16 } },
				{ MapLocation.SeaShrine2, new List<MapLocation> { MapLocation.SeaShrine2Room2 } },
				{ MapLocation.TitansTunnelWest, new List<MapLocation> { MapLocation.TitansTunnelRoom } },
				{ MapLocation.TempleOfFiends1, new List<MapLocation> { MapLocation.TempleOfFiends1Room1, MapLocation.TempleOfFiends1Room2,
					MapLocation.TempleOfFiends1Room3, MapLocation.TempleOfFiends1Room4, MapLocation.TempleOfFiends2, MapLocation.TempleOfFiends3,
					MapLocation.TempleOfFiendsPhantom, MapLocation.TempleOfFiendsEarth, MapLocation.TempleOfFiendsFire,
					MapLocation.TempleOfFiendsWater, MapLocation.TempleOfFiendsAir, MapLocation.TempleOfFiendsChaos } }
			};

		public enum Palette
		{
			Town = 0,
			Castle = 1,
			Greyscale = 2,
			Orange = 3,
			DarkOrange = 4,
			PaleRed = 5,
			Red = 6,
			PaleBlue = 7,
			Teal = 8,
			DarkTeal = 9,
			Purple = 10,
			Yellow = 11,
			PaleGreen = 12,
			Green = 13,
			Blue = 14,
			LightBlue = 15,
			DarkBlue = 16,
			Bluescale = 17,
			YellowGreen = 18,
			Pink = 19,
			Flame = 20,
		};

		public static List<MapIndex> FixedObjectPaletteDestinations = new List<MapIndex>
		{
			MapIndex.MirageTower1F, MapIndex.MirageTower2F, MapIndex.MirageTower3F, MapIndex.SkyPalace1F,
			MapIndex.SkyPalace2F, MapIndex.SkyPalace3F, MapIndex.SkyPalace4F, MapIndex.SkyPalace5F,
		};

		public List<List<MapEdit>> MapEditsToApply = new List<List<MapEdit>>();

		const int bankStart = 0x4000;

		public List<List<byte>> GetCompressedMapRows()
		{

			var pointers = _rom.Get(bankStart, 512).ToUShorts().Select(x => x - bankStart);
			var mapRows = pointers.Select(x =>
			{
				var mapRow = _rom.Get(x, 256).ToBytes();
				var result = new List<byte>();
				var index = 0;
				while (index < 256 && mapRow[index] != 255)
				{
					result.Add(mapRow[index]);
					index++;
				}
				result.Add(mapRow[index]);
				return result;
			}).ToList();
			return mapRows;
		}

		public List<List<byte>> DecompressMapRows(List<List<byte>> compressedRows)
		{
			var mapRows = new List<List<byte>>();
			var run = 0;
			foreach (var compressedRow in compressedRows)
			{
				byte tile = 0;
				var row = new List<byte>();
				var tileIndex = 0;
				while (row.Count() < 256)
				{
					tile = compressedRow[tileIndex];
					if (tile < 0x80)
					{
						row.Add(tile);
					}
					else if (tile == 0xFF)
					{
						for (var i = tileIndex; i < 256; i++)
						{
							row.Add(0x17);
						}
					}
					else
					{
						tileIndex++;
						run = compressedRow[tileIndex];
						if (run == 0)
							run = 256;
						tile -= 0x80;
						for (var i = 0; i < run; i++)
						{
							row.Add(tile);
						}
					}
					tileIndex++;
				}
				mapRows.Add(row);
			}
			return mapRows;
		}

		public void DebugWriteDecompressedMap(List<List<byte>> decompressedRows)
		{
			foreach (var row in decompressedRows)
			{
				foreach (var tile in row)
				{
					Debug.Write($"{tile:X2}");
				}
				Debug.Write("\n");
			}
		}

		public void SwapMap(string fileName)
		{
			List<List<byte>> decompressedRows = new List<List<byte>>();

			var assembly = System.Reflection.Assembly.GetExecutingAssembly();
			var resourcePath = assembly.GetManifestResourceNames().First(str => str.EndsWith(fileName));

			using (Stream stream = assembly.GetManifestResourceStream(resourcePath))
			using (BinaryReader rd = new BinaryReader(stream))
			{
				for (int i = 0; i < 256; i++)
				{
					var row = rd.ReadBytes(256);
					decompressedRows.Add(new List<byte>(row));
				}
			}
			SwapMap(decompressedRows);
		}

	    public void SwapMap(List<List<byte>> decompressedRows)
		{
			_decompressedMap = decompressedRows;
			var recompressedMap = CompressMapRows(decompressedRows);
			PutCompressedMapRows(recompressedMap);
	    }

	    public void SwapMap(List<string> decompressedRows)
		{
			var rows = new List<List<byte>>();
			foreach (var c in decompressedRows) {
			    rows.Add(new List<byte>(Convert.FromBase64String(c)));
			}
			_decompressedMap = rows;
			var recompressedMap = CompressMapRows(rows);
			PutCompressedMapRows(recompressedMap);
	    }

		public void ApplyMapEdits()
		{
			/*var compresedMap = GetCompressedMapRows();
			var decompressedMap = DecompressMapRows(compresedMap);*/
			var editedMap = _decompressedMap;
			foreach (var mapEdit in MapEditsToApply)
			{
				editedMap = ApplyMapEdits(editedMap, mapEdit);
			}
			var recompressedMap = CompressMapRows(editedMap);
			PutCompressedMapRows(recompressedMap);
		}

		public List<List<byte>> ApplyMapEdits(List<List<byte>> decompressedRows, List<MapEdit> mapEdits)
		{
			foreach (var mapEdit in mapEdits)
			{
				decompressedRows[mapEdit.Y][mapEdit.X] = mapEdit.Tile;
			}
			return decompressedRows;
		}

		public static List<List<byte>> CompressMapRows(List<List<byte>> decompressedRows)
		{
			var outputMap = new List<List<byte>>();
			foreach (var row in decompressedRows)
			{
				var outputRow = new List<byte>();
				byte tile = 0;
				byte runCount = 1;
				//if (row.Distinct().Count() == 1)
				//{
				//	outputMap.Add(new List<byte> { 0x97, 0x00, 0xFF });
				//	continue;
				//}
				for (var tileIndex = 0; tileIndex < 256; tileIndex++)
				{
					tile = row[tileIndex];
					if (tileIndex != 255 && tile == row[tileIndex + 1])
					{
						runCount++;
						continue;
					}
					if (runCount == 1)
					{
						outputRow.Add(tile);
						continue;
					}
					outputRow.Add((byte)(tile + 0x80));
					outputRow.Add(runCount);
					runCount = 1;
				}
				outputRow.Add(0xFF);
				outputMap.Add(outputRow);
			}
			return outputMap;
		}

	    public const int MaximumMapDataSize = 0x3E00;

		public void PutCompressedMapRows(List<List<byte>> compressedRows)
		{
			var pointerBase = 0x4000;
			var outputBase = 0x4200;
			var outputOffset = 0;
			for (int i = 0; i < compressedRows.Count; i++)
			{
				var outputRow = compressedRows[i];
				_rom.Put(pointerBase + i * 2, Blob.FromUShorts(new ushort[] { (ushort)(outputBase + pointerBase + outputOffset) }));
				_rom.Put(outputBase + outputOffset, outputRow.ToArray());
				outputOffset += outputRow.Count;
			}

			if (outputOffset > MaximumMapDataSize)
				throw new InvalidOperationException($"Modified map was too large by {outputOffset - MaximumMapDataSize} bytes to recompress and fit into {MaximumMapDataSize} bytes of available space.");
		}
		//  Should be in NPC class
	    public void ShuffleChime(MT19337 rng, bool includeTowns) {
		List<byte[]> dungeons = new List<byte[]> {
		    new byte[] { OverworldTiles.EARTH_CAVE },
		    new byte[] { OverworldTiles.ELFLAND_CASTLE_W, OverworldTiles.ELFLAND_CASTLE_E },
		    new byte[] { OverworldTiles.MIRAGE_BOTTOM },
		    new byte[] { OverworldTiles.ASTOS_CASTLE_W, OverworldTiles.ASTOS_CASTLE_E },
		    new byte[] { OverworldTiles.ICE_CAVE },
		    new byte[] { OverworldTiles.DWARF_CAVE },
		    new byte[] { OverworldTiles.MATOYAS_CAVE },
		    new byte[] { OverworldTiles.TITAN_CAVE_E, OverworldTiles.TITAN_CAVE_W },
		    new byte[] { OverworldTiles.ORDEALS_CASTLE_W, OverworldTiles.ORDEALS_CASTLE_E },
		    new byte[] { OverworldTiles.SARDAS_CAVE },
		    new byte[] { OverworldTiles.TOF_ENTRANCE_W, OverworldTiles.TOF_ENTRANCE_E },
		    new byte[] { OverworldTiles.VOLCANO_TOP_W, OverworldTiles.VOLCANO_TOP_E },
		    new byte[] { OverworldTiles.BAHAMUTS_CAVE },
		    new byte[] { OverworldTiles.MARSH_CAVE }
		};

		List<byte[]> towns = new List<byte[]>{
		    new byte[] { OverworldTiles.PRAVOKA },
		    new byte[] { OverworldTiles.ELFLAND },
		    new byte[] { OverworldTiles.MELMOND },
		    new byte[] { OverworldTiles.CRESCENT_LAKE },
		    new byte[] { OverworldTiles.GAIA },
		    new byte[] { OverworldTiles.ONRAC },
		    new byte[] { OverworldTiles.LEFEIN },
		};

		var candidates = new List<byte[]>(dungeons);

		if (includeTowns) {
		    candidates.AddRange(towns);
		}

		var tileset = new TileSet(this._rom, TileSet.OverworldIndex);
		tileset.LoadData();

		// clear existing
		for (int i = 0; i < tileset.Tiles.Count; i++) {
		    var tp = tileset.Tiles[i].Properties;
		    if ((tp.TilePropFunc & TilePropFunc.OWTP_SPEC_MASK) == TilePropFunc.OWTP_SPEC_CHIME) {
			tp.TilePropFunc &= ~TilePropFunc.OWTP_SPEC_CHIME;
			tileset.Tiles[i].Properties = tp;
		    }
		}

		var chime = candidates.SpliceRandom(rng);

		foreach (var i in chime) {
		    var tp = tileset.Tiles[i].Properties;
		    tp.TilePropFunc |= TilePropFunc.OWTP_SPEC_CHIME;
		    tileset.Tiles[i].Properties = tp;
		}

		tileset.StoreData();
	    }
	}
}
