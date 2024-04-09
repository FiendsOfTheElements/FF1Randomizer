using System.Diagnostics;
using static FF1Lib.FF1Rom;

namespace FF1Lib.Sanity
{
	public class SCMain
	{
		FF1Rom rom;
		List<Map> maps;
		OverworldMap overworldMap;


		MapTileSets mapTileSets;
		SCTileSet[] tileSets = new SCTileSet[8];
		SCTileSet owtileset;

		EnterTeleData enter;
		ExitTeleData exit;
		NormTeleData tele;
		NPCdata npcdata;

		Dictionary<MapIndex, SCMap> scmaps;

		public List<SCDungeon> Dungeons { get; private set; } = new List<SCDungeon>();

		public SCOwMap Overworld { get; private set; }

		public SCMain(List<Map> _maps, OverworldMap _overworldMap, NPCdata _npcdata, OwLocationData locations, FF1Rom _rom)
		{
			maps = _maps;
			overworldMap = _overworldMap;
			rom = _rom;
			npcdata = _npcdata;

			mapTileSets = new MapTileSets(rom);
			enter = new EnterTeleData(rom);
			tele = new NormTeleData(rom);
			exit = new ExitTeleData(rom);

			mapTileSets.LoadTable();
			enter.LoadData();
			tele.LoadData();
			exit.LoadData();

			for (int i = 0; i < 8; i++) tileSets[i] = new SCTileSet(rom, (byte)i);

			owtileset = new SCTileSet(rom, TileSet.OverworldIndex);

			Stopwatch w = Stopwatch.StartNew();

			List<SCMap> tmpscmaps = new List<SCMap>();
			//Parallel.ForEach(Enum.GetValues<MapIndex>(), MapIndex => ProcessMap(MapIndex, tmpscmaps));

			foreach(var MapIndex in Enum.GetValues<MapIndex>()) ProcessMap(MapIndex, tmpscmaps);

			scmaps = tmpscmaps.ToDictionary(m => m.MapIndex);

			ComposeDungeons();

			//SCCoords bridge = new SCCoords(0x98, 0x98);
			//SCCoords canal = new SCCoords(0x66, 0xA4);
			Overworld = new SCOwMap(overworldMap, SCMapCheckFlags.None, _rom, owtileset, enter, exit, locations.BridgeLocation, locations.CanalLocation);

			w.Stop();
		}

		private void ProcessMap(MapIndex MapIndex, List<SCMap> tmpscmaps)
		{
			var e1 = maps[(int)MapIndex];
			var ts = tileSets[mapTileSets[MapIndex]];

			SCMapCheckFlags cflags = SCMapCheckFlags.None;
			if (MapIndex <= MapIndex.CastleOrdeals1F) cflags |= SCMapCheckFlags.NoWarp;
			if (MapIndex == MapIndex.SkyPalace2F) cflags |= SCMapCheckFlags.NoUseTiles;

			SCMap scmap = new SCMap(MapIndex, e1, cflags, rom, npcdata, ts, enter, exit, tele);
			tmpscmaps.Add(scmap);
		}

		private void ComposeDungeons()
		{
			int teleportindex = -1;
			HashSet<SCTeleport> usedEnterTeles = new HashSet<SCTeleport>(new SCTEleportTargetEqualityComparer());
			foreach (var et in enter.Select(e => new SCTeleport(e, SCPointOfInterestType.OwEntrance)))
			{
				teleportindex++;
				if (usedEnterTeles.Contains(et)) continue;
				Dungeons.Add(new SCDungeon(et, (OverworldTeleportIndex)teleportindex, scmaps, usedEnterTeles));
			}
		}
	}
}
