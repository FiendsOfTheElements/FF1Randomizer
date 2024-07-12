using System.Diagnostics;
using static FF1Lib.FF1Rom;

namespace FF1Lib.Sanity
{
	public class SCMain
	{
		FF1Rom rom;
		StandardMaps maps;
		OverworldMap overworldMap;
		Overworld overworld;

		List<TileSets> mapTileSets;
		SCTileSet[] tileSets = new SCTileSet[8];
		SCTileSet owtileset;

		//EnterTeleData enter;
		//ExitTeleData exit;
		//NormTeleData tele;
		Teleporters teleporters;
		NpcObjectData npcdata;

		Dictionary<MapIndex, SCMap> scmaps;

		public List<SCDungeon> Dungeons { get; private set; } = new List<SCDungeon>();

		public SCOwMap Overworld { get; private set; }

		public SCMain(StandardMaps _maps, Overworld _overworld, NpcObjectData _npcdata, TileSetsData tileSetsData, Teleporters _teleporters, OwLocationData locations, FF1Rom _rom)
		{
			maps = _maps;
			overworldMap = _overworld.OverworldMap;
			overworld = _overworld;
			rom = _rom;
			npcdata = _npcdata;
			teleporters = _teleporters;

			mapTileSets = maps.MapTileSets;

			//mapTileSets = new MapTileSets(rom);
			//enter = new EnterTeleData(rom);
			//tele = new NormTeleData(rom);
			//exit = new ExitTeleData(rom);

			//mapTileSets.LoadTable();
			//enter.LoadData();
			//tele.LoadData();
			//exit.LoadData();

			for (int i = 0; i < 8; i++) tileSets[i] = new SCTileSet(tileSetsData[i], i);

			//owtileset = new SCTileSet(rom, TileSet.OverworldIndex);
			owtileset = new SCTileSet(overworld.TileSet, TileSet.OverworldIndex);

			Stopwatch w = Stopwatch.StartNew();

			List<SCMap> tmpscmaps = new List<SCMap>();
			//Parallel.ForEach(Enum.GetValues<MapIndex>(), MapIndex => ProcessMap(MapIndex, tmpscmaps));

			foreach(var MapIndex in Enum.GetValues<MapIndex>()) ProcessMap(MapIndex, tmpscmaps);

			scmaps = tmpscmaps.ToDictionary(m => m.MapIndex);

			ComposeDungeons();

			//SCCoords bridge = new SCCoords(0x98, 0x98);
			//SCCoords canal = new SCCoords(0x66, 0xA4);
			Overworld = new SCOwMap(overworldMap, SCMapCheckFlags.None, _rom, owtileset, teleporters, locations.BridgeLocation, locations.CanalLocation);

			w.Stop();
		}

		private void ProcessMap(MapIndex mapindex, List<SCMap> tmpscmaps)
		{
			var e1 = maps[mapindex];
			var ts = tileSets[(int)mapTileSets[(int)mapindex]];

			SCMapCheckFlags cflags = SCMapCheckFlags.None;
			if (mapindex <= MapIndex.CastleOrdeals1F) cflags |= SCMapCheckFlags.NoWarp;
			if (mapindex == MapIndex.SkyPalace2F) cflags |= SCMapCheckFlags.NoUseTiles;

			SCMap scmap = new SCMap(mapindex, e1, cflags, rom, npcdata, ts, teleporters);
			tmpscmaps.Add(scmap);
		}

		private void ComposeDungeons()
		{
			int teleportindex = -1;
			HashSet<SCTeleport> usedEnterTeles = new HashSet<SCTeleport>(new SCTEleportTargetEqualityComparer());
			foreach (var et in teleporters.OverworldTeleporters.Select(e => new SCTeleport(new TeleData(e.Value), SCPointOfInterestType.OwEntrance)))
			{
				teleportindex++;
				if (usedEnterTeles.Contains(et)) continue;
				Dungeons.Add(new SCDungeon(et, (OverworldTeleportIndex)teleportindex, scmaps, usedEnterTeles));
			}
		}
	}
}
