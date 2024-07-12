using System.Reflection;
using FF1Lib;
using FF1Lib.Sanity;
using System.Diagnostics;
using MapGenerationTask = FF1Lib.Procgen.ProgenFramework.GenerationTaskType<FF1Lib.Procgen.MapResult>;

namespace FF1Lib.Procgen
{


    public partial class MapState : ProcgenState<MapResult, MapGenerationStep, ProgenFramework.GenerationTaskType<MapResult>>{
        public const int MAPSIZE = 64;
	private bool ownTilemap;
        public Map Tilemap;
	public DungeonTiles dt;
	int roomCount;
	public TileSet tileSet;

	private bool ownFeatures;
	public List<byte> Stairs;
	public List<byte> Chests;
	public List<byte> Traps;
	public List<byte> RoomFloorTiles;
	public List<byte> RoomBattleTiles;
	public List<NPC> NPCs;

	public MapLocation mapLocation;
	public MapIndex MapIndex;
	public Dictionary<OverworldTeleportIndex, TeleportDestination> OverworldEntrances;
	public Dictionary<TeleportIndex, TeleportDestination> MapDestinations;
	public SCCoords Entrance;

        public MapState(MT19337 rng, List<MapGenerationStep> steps, MapIndex MapIndex, Map tilemap, DungeonTiles dt, TileSet tileSet, FF1Rom.ReportProgress progress) : base(rng, steps, progress) {
            this.ownTilemap = true;
	    this.Tilemap = tilemap.Clone();
	    this.dt = dt;
	    this.roomCount = 0;
	    this.tileSet = tileSet;
	    this.Stairs = new();
	    this.Chests = new();
	    this.Traps = new();
	    this.NPCs = new();
	    this.RoomFloorTiles = new();
	    this.RoomBattleTiles = new();
	    this.OverworldEntrances = new();
	    this.MapDestinations = new();
	    this.MapIndex = MapIndex;
	}

        public MapState(MapState copy) : base(copy) {
            this.ownTilemap = false;
	    this.Tilemap = copy.Tilemap;
	    this.dt = copy.dt;
	    this.roomCount = copy.roomCount;
	    this.tileSet = copy.tileSet;
	    this.Stairs = copy.Stairs;
	    this.Chests = copy.Chests;
	    this.Traps = copy.Traps;
	    this.NPCs = copy.NPCs;
	    this.Entrance = copy.Entrance;
	    this.RoomFloorTiles = copy.RoomFloorTiles;
	    this.RoomBattleTiles = copy.RoomBattleTiles;
	    this.mapLocation = copy.mapLocation;
	    this.MapIndex = copy.MapIndex;
	    this.OverworldEntrances = copy.OverworldEntrances;
	    this.MapDestinations = copy.MapDestinations;
	}

        void OwnTilemap() {
            if (this.ownTilemap) {
                return;
            }
	    this.Tilemap = this.Tilemap.Clone();
            this.ownTilemap = true;
        }

        void OwnFeatures() {
            if (this.ownFeatures) {
                return;
            }
	    this.Chests = new (this.Chests);
	    this.Stairs = new (this.Stairs);
	    this.Traps = new (this.Traps);
	    this.NPCs = new (this.NPCs);
            this.ownFeatures = true;
        }

	public void RenderFeature(SCCoords point, byte[,] feature) {
	    this.OwnTilemap();
	    for (int j = 0; j < feature.GetLength(0); j++) {
		for (int i = 0; i < feature.GetLength(1); i++) {
		    if (feature[j,i] != OverworldTiles.None) {
			this.Tilemap[point.Y+j, point.X+i] = feature[j,i];
		    }
		}
	    }
	}

	// Apply a 3x3 filter over the entire map.
        public Task<MapResult> ApplyFilter(PgTileFilter filter, bool repeat) {
            this.OwnTilemap();
            this.Tilemap.MapBytes = filter.ApplyFilter(this.Tilemap.MapBytes, repeat);
            return this.NextStep();
        }

    }

    public class MapGenerationStep : GenerationStep<ProcgenState<MapResult, MapGenerationStep, ProgenFramework.GenerationTaskType<MapResult>>, MapResult> {
	public MapGenerationStep(string methodName, object[] parameters, string message="") : base("FF1Lib.Procgen.MapState", methodName, parameters, message) { }
    }

    public class MapResult : Result<ProcgenState<FF1Lib.Procgen.MapResult, MapGenerationStep, ProgenFramework.GenerationTaskType<MapResult>>,
	MapGenerationStep, ProgenFramework.GenerationTaskType<MapResult>> {

	public MapResult() : base() { }
	public MapResult(bool f) : base(f) {}
        public MapResult(ProcgenState<FF1Lib.Procgen.MapResult, MapGenerationStep, ProgenFramework.GenerationTaskType<MapResult>> f) : base(f) {}
	public MapResult(List<MapGenerationTask> tasks) : base(tasks) { }
    }

    public static class NewDungeon {

	public async static Task<CompleteMap> GenerateMap(MT19337 rng, FF1Rom rom, StandardMaps maps, MapIndex mapindex,
							  List<MapGenerationStep> mapGenSteps, DungeonTiles dt, FF1Rom.ReportProgress progress) {
	    var tileset = new TileSet(rom, rom.GetMapTilesetIndex(mapindex));
	    var blankState = new MapState(rng, mapGenSteps, mapindex, maps[mapindex].Map, dt, tileset, progress);
	    var worldState = await ProgenFramework.RunSteps<MapState, MapResult, MapGenerationStep>(blankState, 2000, progress);
	    if (worldState != null) {
		return new CompleteMap {
		    MapIndex = mapindex,
		    Map = worldState.Tilemap,
		    OverworldEntrances = worldState.OverworldEntrances,
		    MapDestinations = worldState.MapDestinations,
		    NPCs = worldState.NPCs,
		};
	    } else {
		return null;
	    }
	}

	public async static Task<List<CompleteMap>> GenerateEarthCave(MT19337 rng, FF1Rom rom, StandardMaps maps, FF1Rom.ReportProgress progress) {
	    var dt = new DungeonTiles();

	    List<CompleteMap> newmaps = new();
	    List<MapGenerationStep> mapGenSteps;
	    CompleteMap newmap;

	    mapGenSteps = new () {
		new MapGenerationStep("CollectInfo", new object[] { rom, maps }),
		new MapGenerationStep("WipeMap", new object[] { DungeonTiles.CAVE_BLANK }),
		new MapGenerationStep("SetEntrance", new object[] { new SCCoords(0x17, 0x18) }),
		new MapGenerationStep("AddOverworldEntrance", new object[] { OverworldTeleportIndex.EarthCave1,
									    new TeleportDestination(MapLocation.EarthCave1,
												    MapIndex.EarthCaveB1, new Coordinate(0x17, 0x18, CoordinateLocale.Standard),
												    TeleportIndex.EarthCave2) }),
		new MapGenerationStep("AddMapDestination", new object[] { TeleportIndex.Overworld,
									 new TeleportDestination(MapLocation.EarthCave1, MapIndex.EarthCaveB1,
												 new Coordinate(0x17, 0x18, CoordinateLocale.Standard),
												 TeleportIndex.EarthCave2) }),
		new MapGenerationStep("EarthB1Style", new object[] { rng.Between(2, 5) }),
		new MapGenerationStep("PlaceTile", new object[] { 0x17, 0x18, DungeonTiles.CAVE_EARTH_WARP }),
		new MapGenerationStep("PlaceTreasureRooms", new object[] { }),
		new MapGenerationStep("ApplyFilter", new object[] { dt.cave_rock_walls, false }),
		new MapGenerationStep("ApplyFilter", new object[] { dt.cave_room_walls, false }),
		new MapGenerationStep("ApplyFilter", new object[] { dt.cave_room_walls2, false }),
		new MapGenerationStep("PlaceExitStairs", new object[] { dt.cave_corners, DungeonTiles.CAVE_EXIT_TO_EARTH_B2, 16, 12 }),
		new MapGenerationStep("PlaceHallOfGiants", new object[] { new List<byte> {0x1B, 0x1C} }),
		new MapGenerationStep("PlaceChests", new object[] { rom, maps, new List<(MapIndex,byte)> {} }),
		new MapGenerationStep("SanityCheck", new object[] { }),
	    };

	    newmap = null;
	    do {
		newmap = await GenerateMap(rng, rom, maps, MapIndex.EarthCaveB1, mapGenSteps, dt, progress);
	    } while (newmap == null);
	    newmaps.Add(newmap);

	    mapGenSteps = new () {
		new MapGenerationStep("CollectInfo", new object[] { rom, maps }),
		new MapGenerationStep("WipeMap", new object[] { DungeonTiles.CAVE_BLANK }),
		new MapGenerationStep("SetEntrance", new object[] { new SCCoords(0x0A, 0x09) }),
		new MapGenerationStep("AddMapDestination", new object[] { TeleportIndex.EarthCave2,
			new TeleportDestination(MapLocation.EarthCave2, MapIndex.EarthCaveB2,
											       new Coordinate(0x0A, 0x09, CoordinateLocale.Standard),
											       TeleportIndex.EarthCaveVampire) }),
		new MapGenerationStep("EarthB2Style", new object[] { 1, 1, 0, 3,
			33, 36, 3, 6, rng.Between(2, 5), new PgFeature[] { }, 0 }),
		new MapGenerationStep("ConnectRegions", new object[] { }),
		new MapGenerationStep("ApplyFilter", new object[] { dt.earth_cave_walls, false }),
		new MapGenerationStep("ApplyFilter", new object[] { dt.earth_cave_wall_gaps, false }),
		new MapGenerationStep("ApplyFilter", new object[] { dt.earth_cave_extend_walls, true }),
		new MapGenerationStep("ApplyFilter", new object[] { dt.earth_cave_walls2, false }),
		new MapGenerationStep("ApplyFilter", new object[] { dt.earth_cave_extend_walls, true }),
		new MapGenerationStep("ApplyFilter", new object[] { dt.earth_cave_walls3, false }),
		new MapGenerationStep("ApplyFilter", new object[] { dt.earth_cave_extend_walls, true }),
		new MapGenerationStep("ApplyFilter", new object[] { dt.earth_cave_walls2, false }), // double up to fix gaps
		new MapGenerationStep("ApplyFilter", new object[] { dt.earth_cave_walls4, false }),
		new MapGenerationStep("ApplyFilter", new object[] { dt.earth_cave_walls5, false }),
		new MapGenerationStep("ApplyFilter", new object[] { dt.cave_room_walls, false }),
		new MapGenerationStep("ApplyFilter", new object[] { dt.cave_room_walls2, false }),
		new MapGenerationStep("PlaceChests", new object[] { rom, maps, new List<(MapIndex,byte)> {} }),
		new MapGenerationStep("PlaceEntrance", new object[] { DungeonTiles.CAVE_EARTH_WARP }),
		new MapGenerationStep("PlaceExitStairs", new object[] { dt.cave_wall_corners, DungeonTiles.CAVE_EXIT_TO_EARTH_B3, 36, 12 }),
		new MapGenerationStep("MoveBats", new object[] { }),
		new MapGenerationStep("SanityCheck", new object[] { }),
	    };

	    newmap = null;
	    do {
		newmap = await GenerateMap(rng, rom, maps, MapIndex.EarthCaveB2, mapGenSteps, dt, progress);
	    } while (newmap == null);
	    newmaps.Add(newmap);

	    mapGenSteps = new () {
		new MapGenerationStep("CollectInfo", new object[] { rom, maps }),
		new MapGenerationStep("WipeMap", new object[] { DungeonTiles.CAVE_BLANK }),
		new MapGenerationStep("SetEntrance", new object[] { new SCCoords(0x1B, 0x2D) }),
		new MapGenerationStep("AddMapDestination", new object[] { TeleportIndex.EarthCaveVampire,
									 new TeleportDestination(MapLocation.EarthCaveVampire, MapIndex.EarthCaveB3,
											       new Coordinate(0x1B, 0x2D, CoordinateLocale.Standard),
											       TeleportIndex.EarthCave4) }),
		new MapGenerationStep("EarthB2Style", new object[] { 1, 5, 0, 2,
								    20, 25, 2, 4, rng.Between(2, 5),
								    new PgFeature[] { DungeonTiles.VAMPIRE_ROOM, DungeonTiles.ROD_ROOM }, 4 }),
		new MapGenerationStep("ConnectRegions", new object[] { }),
		new MapGenerationStep("ApplyFilter", new object[] { dt.earth_cave_walls, false }),
		new MapGenerationStep("ApplyFilter", new object[] { dt.earth_cave_wall_gaps, false }),
		new MapGenerationStep("ApplyFilter", new object[] { dt.earth_cave_extend_walls, true }),
		new MapGenerationStep("ApplyFilter", new object[] { dt.earth_cave_walls2, false }),
		new MapGenerationStep("ApplyFilter", new object[] { dt.earth_cave_extend_walls, true }),
		new MapGenerationStep("ApplyFilter", new object[] { dt.earth_cave_walls3, false }),
		new MapGenerationStep("ApplyFilter", new object[] { dt.earth_cave_extend_walls, true }),
		new MapGenerationStep("ApplyFilter", new object[] { dt.earth_cave_walls2, false }), // double up to fix gaps introduced by extend walls
		new MapGenerationStep("ApplyFilter", new object[] { dt.earth_cave_walls4, false }),
		new MapGenerationStep("ApplyFilter", new object[] { dt.earth_cave_walls5, false }),
		new MapGenerationStep("ApplyFilter", new object[] { dt.cave_room_walls, false }),
		new MapGenerationStep("ApplyFilter", new object[] { dt.cave_room_walls2, false }),
		new MapGenerationStep("PlaceChests", new object[] { rom, maps, new List<(MapIndex,byte)> { (MapIndex.EarthCaveB3, DungeonTiles.RUBY_CHEST) } }),
		new MapGenerationStep("PlaceEntrance", new object[] { DungeonTiles.CAVE_EARTH_WARP }),
		new MapGenerationStep("MoveBats", new object[] { }),
		new MapGenerationStep("SanityCheck", new object[] { }),
	    };

	    newmap = null;
	    do {
		newmap = await GenerateMap(rng, rom, maps, MapIndex.EarthCaveB3, mapGenSteps, dt, progress);
	    } while (newmap == null);
	    newmaps.Add(newmap);

	    mapGenSteps = new () {
		new MapGenerationStep("CollectInfo", new object[] { rom, maps }),
		new MapGenerationStep("WipeMap", new object[] { DungeonTiles.CAVE_BLANK }),
		new MapGenerationStep("SetEntrance", new object[] { new SCCoords(0x36, 0x21) }),
		new MapGenerationStep("AddMapDestination", new object[] { TeleportIndex.EarthCave4,
									 new TeleportDestination(MapLocation.EarthCave4, MapIndex.EarthCaveB4,
											       new Coordinate(0x36, 0x21, CoordinateLocale.Standard),
											       TeleportIndex.EarthCaveLich) }),
		new MapGenerationStep("EarthB2Style", new object[] { 3, 0, 0, 2,
								    33, 36, 6, 6, rng.Between(2, 5),
								    new PgFeature[] { }, 0}),
		new MapGenerationStep("ConnectRegions", new object[] { }),
		new MapGenerationStep("ApplyFilter", new object[] { dt.earth_cave_walls, false }),
		new MapGenerationStep("ApplyFilter", new object[] { dt.earth_cave_wall_gaps, false }),
		new MapGenerationStep("ApplyFilter", new object[] { dt.earth_cave_extend_walls, true }),
		new MapGenerationStep("ApplyFilter", new object[] { dt.earth_cave_walls2, false }),
		new MapGenerationStep("ApplyFilter", new object[] { dt.earth_cave_extend_walls, true }),
		new MapGenerationStep("ApplyFilter", new object[] { dt.earth_cave_walls3, false }),
		new MapGenerationStep("ApplyFilter", new object[] { dt.earth_cave_extend_walls, true }),
		new MapGenerationStep("ApplyFilter", new object[] { dt.earth_cave_walls2, false }), // double up to fix gaps
		new MapGenerationStep("ApplyFilter", new object[] { dt.earth_cave_walls4, false }),
		new MapGenerationStep("ApplyFilter", new object[] { dt.earth_cave_walls5, false }),
		new MapGenerationStep("ApplyFilter", new object[] { dt.cave_room_walls, false }),
		new MapGenerationStep("ApplyFilter", new object[] { dt.cave_room_walls2, false }),
		new MapGenerationStep("PlaceChests", new object[] { rom, maps, new List<(MapIndex,byte)> {} }),
		new MapGenerationStep("PlaceEntrance", new object[] { DungeonTiles.CAVE_EARTH_WARP }),
		new MapGenerationStep("PlaceExitStairs", new object[] { dt.cave_wall_corners, DungeonTiles.CAVE_EXIT_TO_EARTH_B5, 24, 12 }),
		new MapGenerationStep("MoveBats", new object[] { }),
		new MapGenerationStep("SanityCheck", new object[] { }),
	    };

	    do {
		newmap = await GenerateMap(rng, rom, maps, MapIndex.EarthCaveB4, mapGenSteps, dt, progress);
	    } while (newmap == null);
	    newmaps.Add(newmap);

	    mapGenSteps = new () {
		new MapGenerationStep("CollectInfo", new object[] { rom, maps }),
		new MapGenerationStep("WipeMap", new object[] { DungeonTiles.CAVE_BLANK }),
		new MapGenerationStep("SetEntrance", new object[] { new SCCoords(0x19, 0x35) }),
		new MapGenerationStep("AddMapDestination", new object[] { TeleportIndex.EarthCaveLich,
									 new TeleportDestination(MapLocation.EarthCaveLich, MapIndex.EarthCaveB5,
												  new Coordinate(0x19, 0x35, CoordinateLocale.Standard),
												  ExitTeleportIndex.ExitEarthCave) }),
		new MapGenerationStep("EarthB2Style", new object[] { 1, 1, 1, 1,
								    18, 22, 6, 4, 0, new PgFeature[] { DungeonTiles.LICH_ROOM }, 12 }),
		new MapGenerationStep("ConnectRegions", new object[] { }),
		new MapGenerationStep("ApplyFilter", new object[] { dt.earth_cave_walls, false }),
		new MapGenerationStep("ApplyFilter", new object[] { dt.earth_cave_wall_gaps, false }),
		new MapGenerationStep("ApplyFilter", new object[] { dt.earth_cave_extend_walls, true }),
		new MapGenerationStep("ApplyFilter", new object[] { dt.earth_cave_walls2, false }),
		new MapGenerationStep("ApplyFilter", new object[] { dt.earth_cave_extend_walls, true }),
		new MapGenerationStep("ApplyFilter", new object[] { dt.earth_cave_walls3, false }),
		new MapGenerationStep("ApplyFilter", new object[] { dt.earth_cave_extend_walls, true }),
		new MapGenerationStep("ApplyFilter", new object[] { dt.earth_cave_walls2, false }), // double up to fix gaps
		new MapGenerationStep("ApplyFilter", new object[] { dt.earth_cave_walls4, false }),
		new MapGenerationStep("ApplyFilter", new object[] { dt.earth_cave_walls5, false }),
		new MapGenerationStep("ApplyFilter", new object[] { dt.cave_room_walls, false }),
		new MapGenerationStep("ApplyFilter", new object[] { dt.cave_room_walls2, false }),
		new MapGenerationStep("PlaceEntrance", new object[] { DungeonTiles.CAVE_EARTH_WARP }),
		new MapGenerationStep("MoveBats", new object[] { }),
		new MapGenerationStep("SanityCheck", new object[] { }),
	    };

	    do {
		newmap = await GenerateMap(rng, rom, maps, MapIndex.EarthCaveB5, mapGenSteps, dt, progress);
	    } while (newmap == null);
	    newmaps.Add(newmap);

	    return newmaps;
	}

	public async static Task<List<CompleteMap>> GenerateNewDungeon(MT19337 rng, FF1Rom rom, MapIndex MapIndex, StandardMaps maps,
								       FF1Rom.ReportProgress progress) {
	    switch (MapIndex) {
		case MapIndex.EarthCaveB1:
		    var v = await GenerateEarthCave(rng, rom, maps, progress);
		    return v;
		default:
		    return new List<CompleteMap>();
	    }
	}
    }

}
