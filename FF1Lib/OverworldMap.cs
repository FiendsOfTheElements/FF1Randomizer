using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using RomUtilities;

namespace FF1Lib
{
	public struct MapEdit
	{
		public byte X { get; set; }
		public byte Y { get; set; }
		public byte Tile { get; set; }
	}

	public class OverworldMap
	{
		private readonly FF1Rom _rom;
		public OverworldMap(FF1Rom rom, IMapEditFlags flags)
		{
			_rom = rom;
			var mapLocationRequirements = ItemLocations.MapLocationRequirements.ToDictionary(x => x.Key, x => x.Value.ToList());

			if (flags.MapOnracDock)
			{
				MapEditsToApply.Add(OnracDock);
				mapLocationRequirements[MapLocation.Onrac].Add(MapChange.Ship | MapChange.Canal);
				mapLocationRequirements[MapLocation.Caravan].Add(MapChange.Ship | MapChange.Canal | MapChange.Canoe);
				mapLocationRequirements[MapLocation.Waterfall].Add(MapChange.Ship | MapChange.Canal | MapChange.Canoe);
			}
			if (flags.MapMirageDock)
			{
				MapEditsToApply.Add(MirageDock);
				mapLocationRequirements[MapLocation.MirageTower].Add(MapChange.Ship | MapChange.Canal | MapChange.Chime);
			}
			if (flags.MapVolcanoIceRiver)
			{
				MapEditsToApply.Add(VolcanoIceRiver);
				mapLocationRequirements[MapLocation.GurguVolcano].Add(MapChange.Bridge | MapChange.Canoe);
				mapLocationRequirements[MapLocation.CresentLake].Add(MapChange.Bridge | MapChange.Canoe);
				mapLocationRequirements[MapLocation.ElflandTown].Add(MapChange.Bridge | MapChange.Canoe);
				mapLocationRequirements[MapLocation.ElflandCastle].Add(MapChange.Bridge | MapChange.Canoe);
				mapLocationRequirements[MapLocation.NorthwestCastle].Add(MapChange.Bridge | MapChange.Canoe);
				mapLocationRequirements[MapLocation.MarshCave].Add(MapChange.Bridge | MapChange.Canoe);
				mapLocationRequirements[MapLocation.AirshipLocation].Add(MapChange.Bridge | MapChange.Canoe);
				if(flags.MapCanalBridge)
					mapLocationRequirements[MapLocation.DwarfCave].Add(MapChange.Bridge | MapChange.Canoe);
			}
			if (flags.MapConeriaDwarves)
			{
				MapEditsToApply.Add(ConeriaToDwarves);
				mapLocationRequirements[MapLocation.DwarfCave].Add(MapChange.None);
				if(flags.MapCanalBridge)
				{
					mapLocationRequirements[MapLocation.GurguVolcano].Add(MapChange.Canoe);
					mapLocationRequirements[MapLocation.CresentLake].Add(MapChange.Canoe);
					mapLocationRequirements[MapLocation.ElflandTown].Add(MapChange.Canoe);
					mapLocationRequirements[MapLocation.ElflandCastle].Add(MapChange.Canoe);
					mapLocationRequirements[MapLocation.NorthwestCastle].Add(MapChange.Canoe);
					mapLocationRequirements[MapLocation.MarshCave].Add(MapChange.Canoe);
					mapLocationRequirements[MapLocation.AirshipLocation].Add(MapChange.Canoe);
					if (flags.MapVolcanoIceRiver)
					{
						mapLocationRequirements[MapLocation.IceCave].Add(MapChange.Canoe);
						mapLocationRequirements[MapLocation.Pravoka].Add(MapChange.Canoe);
						mapLocationRequirements[MapLocation.MatoyasCave].Add(MapChange.Canoe);
					}
				}
			}
			
			if (flags.TitansTrove)
			{
				mapLocationRequirements[MapLocation.TitansTunnelWest] = new List<MapChange> {
					MapChange.Canal | MapChange.Ship | MapChange.TitanFed, MapChange.Airship | MapChange.TitanFed };
			}
			MapLocationRequirements = mapLocationRequirements;
		}
		public Dictionary<MapLocation, List<MapChange>> MapLocationRequirements;
		
		public const byte GrassTile = 0x00;
		public const byte GrassBottomRightCoast = 0x06;
		public const byte OceanTile = 0x17;
		public const byte RiverTile = 0x44;
		public const byte MountainTopLeft = 0x10;
		public const byte MountainTopMid = 0x11;
		public const byte MountainMid = 0x21;
		public const byte MountainBottomLeft = 0x30;
		public const byte MountainBottomMid = 0x31;
		public const byte MountainBottomRight = 0x33;
		public const byte ForestMid = 0x14;
		public const byte ForestBottomMid = 0x24;
		public const byte ForestBottomRight = 0x25;
		public const byte ForestBottomLeft = 0x23;
		public const byte DockBottomMid = 0x78;
		public const byte DockRightMid = 0x1F;

		public static List<MapEdit> OnracDock =
			new List<MapEdit>
			{
				new MapEdit{X = 50, Y = 78, Tile = ForestBottomRight},
				new MapEdit{X = 51, Y = 78, Tile = DockBottomMid},
				new MapEdit{X = 52, Y = 78, Tile = DockBottomMid},
				new MapEdit{X = 51, Y = 77, Tile = ForestBottomMid},
				new MapEdit{X = 52, Y = 77, Tile = ForestBottomMid},
				new MapEdit{X = 51, Y = 79, Tile = OceanTile},
				new MapEdit{X = 52, Y = 79, Tile = OceanTile}
			};
		public static List<MapEdit> MirageDock =
			new List<MapEdit>
			{
				new MapEdit{X = 208, Y = 90, Tile = DockBottomMid},
				new MapEdit{X = 209, Y = 90, Tile = DockBottomMid}
			};
		public static List<MapEdit> ConeriaToDwarves =
			new List<MapEdit>
			{
				new MapEdit{X = 124, Y = 138, Tile = MountainBottomLeft},
				new MapEdit{X = 124, Y = 139, Tile = GrassTile},
				new MapEdit{X = 125, Y = 139, Tile = MountainBottomLeft},
				new MapEdit{X = 125, Y = 140, Tile = GrassTile},
				new MapEdit{X = 126, Y = 140, Tile = MountainBottomLeft},
				new MapEdit{X = 127, Y = 140, Tile = MountainBottomMid},
				new MapEdit{X = 128, Y = 140, Tile = MountainBottomMid},
				new MapEdit{X = 129, Y = 140, Tile = MountainBottomMid},
				new MapEdit{X = 126, Y = 141, Tile = GrassTile},
				new MapEdit{X = 127, Y = 141, Tile = GrassTile},
				new MapEdit{X = 128, Y = 141, Tile = GrassTile},
				new MapEdit{X = 129, Y = 141, Tile = GrassTile},
				new MapEdit{X = 130, Y = 141, Tile = MountainBottomLeft}
			};
		public static List<MapEdit> VolcanoIceRiver =
			new List<MapEdit>
			{
				new MapEdit{X = 209, Y = 189, Tile = MountainBottomRight},
				new MapEdit{X = 210, Y = 189, Tile = GrassTile},
				new MapEdit{X = 208, Y = 190, Tile = RiverTile},
				new MapEdit{X = 209, Y = 190, Tile = RiverTile},
				new MapEdit{X = 210, Y = 190, Tile = RiverTile},
				new MapEdit{X = 211, Y = 190, Tile = RiverTile},
				new MapEdit{X = 209, Y = 191, Tile = MountainTopLeft},
				new MapEdit{X = 210, Y = 191, Tile = MountainTopMid},
				new MapEdit{X = 211, Y = 191, Tile = MountainTopMid}
			};
		public static List<MapEdit> CanalSoftLockMountain =
			new List<MapEdit>
			{
				new MapEdit{X = 101, Y = 161, Tile = MountainTopLeft},
				new MapEdit{X = 102, Y = 161, Tile = MountainTopMid},
				new MapEdit{X = 103, Y = 161, Tile = MountainMid},
				new MapEdit{X = 101, Y = 162, Tile = MountainBottomLeft},
				new MapEdit{X = 102, Y = 162, Tile = MountainBottomMid},
				new MapEdit{X = 103, Y = 162, Tile = MountainBottomRight}
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

		public void ApplyMapEdits()
		{
			var compresedMap = GetCompressedMapRows();
			var decompressedMap = DecompressMapRows(compresedMap);
			var editedMap = decompressedMap;
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

		public List<List<byte>> CompressMapRows(List<List<byte>> decompressedRows)
		{
			var outputMap = new List<List<byte>>();
			foreach (var row in decompressedRows)
			{
				var outputRow = new List<byte>();
				byte tile = 0;
				byte runCount = 1;
				if (row.Distinct().Count() == 1)
				{
					outputMap.Add(new List<byte> { 0x97, 0x00, 0xFF });
					continue;
				}
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

			if (outputOffset > 0x4000)
				throw new InvalidOperationException("Modified map was too large to recompress and fit into a single bank.");
		}
	}
}
