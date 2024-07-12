using FF1Lib.Procgen;
using System.IO.Compression;

namespace FF1Lib
{
	public class MapDataGroup
	{
		public Map Map;
		public MapObjects MapObjects;
		public TileSets MapTileSet;

		public MapDataGroup(Map _map, MapObjects _mapobjects, TileSets _tileset)
		{
			Map = _map;
			MapObjects = _mapobjects;
			MapTileSet = _tileset;
		}
	}

	public partial class StandardMaps
	{
		private const int MapPointerOffset = 0x10000;
		private const int MapPointerSize = 2;
		private const int MapCount = 61;
		private const int MapDataOffset = 0x10080;

		private FF1Rom rom;
		private Teleporters teleporters;
		private List<Map> maps { get => mapDataGroups.Select(m => m.Map).ToList(); }
		private List<MapObjects> mapObjects { get => mapDataGroups.Select(m => m.MapObjects).ToList(); }
		public List<TileSets> MapTileSets { get => mapDataGroups.Select(m => m.MapTileSet).ToList(); }
		private List<MapDataGroup> mapDataGroups;
		private Flags flags;
		public List<MapIndex> VerticalFlippedMaps { get; private set; }
		public List<MapIndex> HorizontalFlippedMaps { get; private set; }
		public MapIndex AttackedTown { get; private set; }

		public StandardMaps(FF1Rom _rom, Teleporters _teleporters, Flags _flags)
		{
			rom = _rom;
			teleporters = _teleporters;
			flags = _flags;

			VerticalFlippedMaps = new();
			HorizontalFlippedMaps = new();
			AttackedTown = MapIndex.Melmond;

			LoadMapsFromRom();
			//mapDataGroups = maps.Select((m, i) => (m, mapObjects[i])).ToList();
		}
		public StandardMaps(FF1Rom _rom)
		{
			rom = _rom;
			teleporters = new Teleporters(rom, null);
			flags = new Flags();

			VerticalFlippedMaps = new();
			HorizontalFlippedMaps = new();
			AttackedTown = MapIndex.Melmond;

			LoadMapsFromRom();
			//mapDataGroups = maps.Select((m, i) => (m, mapObjects[i])).ToList();
		}
		private void LoadMapsFromRom()
		{
			var pointers = rom.Get(MapPointerOffset, MapCount * MapPointerSize).ToUShorts();
			var tempMaps = pointers.Select(pointer => new Map(rom.Get(MapPointerOffset + pointer, Map.RowCount * Map.RowLength))).ToList();
			var tempMapObjects = Enumerable.Range(0, MapCount).Select(m => new MapObjects(rom, (MapIndex)m)).ToList();
			var tempMapTileSets = new MapTileSets(rom);
			tempMapTileSets.LoadTable();

			//var tempBackrops = rom.GetFromBank(lut_BtlBackdrops_Bank, lut_BtlBackdrops, 0x80).ToBytes();
			mapDataGroups = tempMaps.Select((m, i) => new MapDataGroup(m, tempMapObjects[i], (TileSets)tempMapTileSets[(MapIndex)i])).ToList();

		}
		public MapDataGroup this[MapIndex index]
		{
			get => mapDataGroups[(int)index];
		}
		public List<Map> GetMapList()
		{
			return maps;
		}
		public void ImportCustomMap(Teleporters teleporters, CompleteMap newmap)
		{
			for(int x = 0; x < 64; x++)
			{
				for (int y = 0; y < 64; y++)
				{
					maps[(int)newmap.MapIndex][(x, y)] = newmap.Map[(x, y)];
				}
			}

			foreach (var dest in newmap.MapDestinations)
			{
				// Update the teleport information, this
				// consists of the corresponding map location
				// (this is a walkable area of the map,
				// because some dungeons have multiple parts
				// that are actually on the same map), the map
				// index, where the teleport puts the player,
				// and what teleports (but not warps) are
				// accessible in this map location.
				if (dest.Key != TeleportIndex.Overworld) {
					teleporters.StandardMapTeleporters[dest.Key] = dest.Value;
				}
			}

			foreach (var kv in newmap.OverworldEntrances) {
				teleporters.OverworldTeleporters[kv.Key] = kv.Value;
			 }
			foreach (var npc in newmap.NPCs)
			{
				mapObjects[(int)newmap.MapIndex][npc.Index].CopyFrom(npc);
			}
		}

		void LoadPregenDungeon(MT19337 rng, Teleporters teleporters, string name)
		{
			var assembly = System.Reflection.Assembly.GetExecutingAssembly();
			var resourcePath = assembly.GetManifestResourceNames().First(str => str.EndsWith(name));

			using Stream stream = assembly.GetManifestResourceStream(resourcePath);

			var archive = new ZipArchive(stream);

			var maplist = archive.Entries.Where(e => e.Name.EndsWith(".json")).Select(e => e.Name).ToList();

			var map = maplist.PickRandom(rng);

			var test = archive.GetEntry(map);
			var test2 = test.Open();

			var loadedmaps = CompleteMap.LoadJson(test2);

			foreach (var m in loadedmaps)
			{
				ImportCustomMap(teleporters, m);
			}
		}
		public void Write()
		{
			var data = maps.Select(map => map.GetCompressedData()).ToList();

			var pointers = new ushort[MapCount];
			pointers[0] = MapDataOffset - MapPointerOffset;
			for (int i = 1; i < MapCount; i++)
			{
				pointers[i] = (ushort)(pointers[i - 1] + data[i - 1].Length);
			}

			rom.Put(MapPointerOffset, Blob.FromUShorts(pointers));
			for (int i = 0; i < MapCount; i++)
			{
				rom.Put(MapPointerOffset + pointers[i], data[i]);
			}

			foreach (var mapobject in mapObjects)
			{
				mapobject.Write(rom);
			}

			MapTileSets tempTileSets = new(rom);
			tempTileSets.LoadTable();
			for (int i = 0; i < MapTileSets.Count; i++)
			{
				tempTileSets[i] = (byte)MapTileSets[i];
			}
			tempTileSets.StoreTable();
		}
	}
}
