using FF1Lib.Procgen;
using System.IO.Compression;

namespace FF1Lib
{
	public partial class StandardMaps
	{
		private const int MapPointerOffset = 0x10000;
		private const int MapPointerSize = 2;
		private const int MapCount = 61;
		private const int MapDataOffset = 0x10080;

		private FF1Rom rom;
		private Teleporters teleporters;
		private List<Map> maps;
		private List<MapObjects> mapObjects;
		private List<(Map Map, MapObjects MapObjects)> mapAndMapObjects;
		private Flags flags;
		public List<MapIndex> VerticalFlippedMaps { get; private set; }
		public List<MapIndex> HorizontalFlippedMaps { get; private set; }

		public StandardMaps(FF1Rom _rom, Teleporters _teleporters, Flags _flags)
		{
			rom = _rom;
			teleporters = _teleporters;
			flags = _flags;

			LoadMapsFromRom();
			mapAndMapObjects = maps.Select((m, i) => (m, mapObjects[i])).ToList();
		}
		private void LoadMapsFromRom()
		{
			var pointers = rom.Get(MapPointerOffset, MapCount * MapPointerSize).ToUShorts();
			maps = pointers.Select(pointer => new Map(rom.Get(MapPointerOffset + pointer, Map.RowCount * Map.RowLength))).ToList();

			mapObjects = Enumerable.Range(0, MapCount).Select(m => new MapObjects(rom, (MapIndex)m)).ToList();
		}
		public (Map Map, MapObjects MapObjects) this[MapIndex index]
		{
			get => mapAndMapObjects[(int)index];
		}
		public void ImportCustomMap(Teleporters teleporters, CompleteMap newmap)
		{
			maps[(int)newmap.MapIndex] = newmap.Map;
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
				teleporters.StandardMapTeleporters[dest.Key] = dest.Value;
				//teleporters.Set(dest.Value.Destination.ToString(), dest.Value);
				/*
				if (dest.Key != TeleportIndex.Overworld) {
					overworldMap.PutStandardTeleport(dest.Key, dest.Value, OverworldTeleportIndex.None);
				}*/
			}
			/*foreach (var kv in newmap.OverworldEntrances) {
			 overworldMap.PutOverworldTeleport(kv.Key, kv.Value);
			 }*/
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

			var loadedmaps = CompleteMap.LoadJson(archive.GetEntry(map).Open());

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
		}
	}
}
