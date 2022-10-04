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
	public List<int> NPCs;

	public SCCoords Entrance;

        public MapState(MT19337 rng, List<MapGenerationStep> steps, Map tilemap, DungeonTiles dt, TileSet tileSet, FF1Rom.ReportProgress progress) : base(rng, steps, progress) {
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

	public async static Task<CompleteMap> GenerateNewMap(MT19337 rng, FF1Rom rom, MapId mapId, List<Map> maps, FF1Rom.ReportProgress progress) {
	    await progress("", 1);

	    List<MapGenerationStep> mapGenSteps = null;

	    var dt = new DungeonTiles();

	    while (true) {
		switch (mapId) {
		    /*
		      case MapId.Coneria:
			mapGenSteps = new () {
			    new MapGenerationStep("WipeMap", new object[] { (byte)0x10 }),
			    new MapGenerationStep("PlaceShop", new object[] { }),
			};
			break;
		    */
		    case MapId.EarthCaveB1:
			mapGenSteps = new () {
			    new MapGenerationStep("CollectInfo", new object[] { }),
			    new MapGenerationStep("WipeMap", new object[] { DungeonTiles.CAVE_BLANK }),
			    new MapGenerationStep("SetEntrance", new object[] { 0x17, 0x18 }),
			    new MapGenerationStep("EarthB1Style", new object[] { }),
			    new MapGenerationStep("PlaceTile", new object[] { 0x17, 0x18, DungeonTiles.CAVE_EARTH_WARP }),
			    new MapGenerationStep("PlaceTreasureRooms", new object[] { }),
			    new MapGenerationStep("ApplyFilter", new object[] { dt.cave_rock_walls, false }),
			    new MapGenerationStep("ApplyFilter", new object[] { dt.cave_room_walls, false }),
			    new MapGenerationStep("ApplyFilter", new object[] { dt.cave_room_walls2, false }),
			    new MapGenerationStep("PlaceExitStairs", new object[] { dt.cave_corners, DungeonTiles.CAVE_EXIT_TO_EARTH_B2, 16, 12 }),
			    new MapGenerationStep("PlaceHallOfGiants", new object[] { new List<byte> {0x1B, 0x1C} }),
			    new MapGenerationStep("PlaceChests", new object[] { rom, maps, mapId, new List<(MapId,byte)> {} }),
			    new MapGenerationStep("SanityCheck", new object[] { }),
			};
			break;
		    case MapId.EarthCaveB2:
			mapGenSteps = new () {
			    new MapGenerationStep("CollectInfo", new object[] { }),
			    new MapGenerationStep("WipeMap", new object[] { DungeonTiles.CAVE_BLANK }),
			    new MapGenerationStep("SetEntrance", new object[] { 0x0A, 0x09 }),
			    new MapGenerationStep("EarthB2Style", new object[] { 29, 33, 6, 6, 2, new PgFeature[] { }, 0 }),
			    new MapGenerationStep("ConnectRegions", new object[] { }),
			    new MapGenerationStep("ApplyFilter", new object[] { dt.earth_cave_walls, false }),
			    new MapGenerationStep("ApplyFilter", new object[] { dt.earth_cave_extend_walls, true }),
			    new MapGenerationStep("ApplyFilter", new object[] { dt.earth_cave_walls2, false }),
			    new MapGenerationStep("ApplyFilter", new object[] { dt.earth_cave_extend_walls, true }),
			    new MapGenerationStep("ApplyFilter", new object[] { dt.earth_cave_walls3, false }),
			    new MapGenerationStep("ApplyFilter", new object[] { dt.earth_cave_extend_walls, true }),
			    new MapGenerationStep("ApplyFilter", new object[] { dt.earth_cave_walls2, false }), // double up to fix gaps
			    new MapGenerationStep("ApplyFilter", new object[] { dt.earth_cave_walls4, true }),
			    new MapGenerationStep("ApplyFilter", new object[] { dt.cave_room_walls, false }),
			    new MapGenerationStep("ApplyFilter", new object[] { dt.cave_room_walls2, false }),
			    new MapGenerationStep("PlaceChests", new object[] { rom, maps, mapId, new List<(MapId,byte)> {} }),
			    new MapGenerationStep("PlaceEntrance", new object[] { DungeonTiles.CAVE_EARTH_WARP }),
			    new MapGenerationStep("PlaceExitStairs", new object[] { dt.cave_wall_corners, DungeonTiles.CAVE_EXIT_TO_EARTH_B3, 36, 12 }),
			};
			break;
		    case MapId.EarthCaveB3:
			mapGenSteps = new () {
			    new MapGenerationStep("CollectInfo", new object[] { }),
			    new MapGenerationStep("WipeMap", new object[] { DungeonTiles.CAVE_BLANK }),
			    new MapGenerationStep("SetEntrance", new object[] { 0x1B, 0x2D }),
			    new MapGenerationStep("EarthB2Style", new object[] { 20, 25, 2, 4, 2, new PgFeature[] { DungeonTiles.VAMPIRE_ROOM, DungeonTiles.ROD_ROOM }, 3 }),
			    new MapGenerationStep("ConnectRegions", new object[] { }),
			    new MapGenerationStep("ApplyFilter", new object[] { dt.earth_cave_walls, false }),
			    new MapGenerationStep("ApplyFilter", new object[] { dt.earth_cave_extend_walls, true }),
			    new MapGenerationStep("ApplyFilter", new object[] { dt.earth_cave_walls2, false }),
			    new MapGenerationStep("ApplyFilter", new object[] { dt.earth_cave_extend_walls, true }),
			    new MapGenerationStep("ApplyFilter", new object[] { dt.earth_cave_walls3, false }),
			    new MapGenerationStep("ApplyFilter", new object[] { dt.earth_cave_extend_walls, true }),
			    new MapGenerationStep("ApplyFilter", new object[] { dt.earth_cave_walls4, true }),
			    new MapGenerationStep("ApplyFilter", new object[] { dt.cave_room_walls, false }),
			    new MapGenerationStep("ApplyFilter", new object[] { dt.cave_room_walls2, false }),
			    new MapGenerationStep("PlaceChests", new object[] { rom, maps, mapId, new List<(MapId,byte)> { (mapId, DungeonTiles.RUBY_CHEST) } }),
			    new MapGenerationStep("PlaceEntrance", new object[] { DungeonTiles.CAVE_EARTH_WARP }),
			    //new MapGenerationStep("SanityCheck", new object[] { }),
			};
			break;
		    case MapId.EarthCaveB4:
			mapGenSteps = new () {
			    new MapGenerationStep("CollectInfo", new object[] { }),
			    new MapGenerationStep("WipeMap", new object[] { DungeonTiles.CAVE_BLANK }),
			    new MapGenerationStep("SetEntrance", new object[] { 0x36, 0x21 }),
			    new MapGenerationStep("EarthB2Style", new object[] { 29, 33, 6, 6, 2, new PgFeature[] { }, 0}),
			    new MapGenerationStep("ConnectRegions", new object[] { }),
			    new MapGenerationStep("ApplyFilter", new object[] { dt.earth_cave_walls, false }),
			    new MapGenerationStep("ApplyFilter", new object[] { dt.earth_cave_extend_walls, true }),
			    new MapGenerationStep("ApplyFilter", new object[] { dt.earth_cave_walls2, false }),
			    new MapGenerationStep("ApplyFilter", new object[] { dt.earth_cave_extend_walls, true }),
			    new MapGenerationStep("ApplyFilter", new object[] { dt.earth_cave_walls3, false }),
			    new MapGenerationStep("ApplyFilter", new object[] { dt.earth_cave_extend_walls, true }),
			    new MapGenerationStep("ApplyFilter", new object[] { dt.earth_cave_walls4, false }),
			    new MapGenerationStep("ApplyFilter", new object[] { dt.cave_room_walls, false }),
			    new MapGenerationStep("ApplyFilter", new object[] { dt.cave_room_walls2, false }),
			    new MapGenerationStep("PlaceChests", new object[] { rom, maps, mapId, new List<(MapId,byte)> {} }),
			    new MapGenerationStep("PlaceEntrance", new object[] { DungeonTiles.CAVE_EARTH_WARP }),
			    new MapGenerationStep("PlaceExitStairs", new object[] { dt.cave_wall_corners, DungeonTiles.CAVE_EXIT_TO_EARTH_B5, 36, 12 }),
			};
			break;
		    case MapId.EarthCaveB5:
			mapGenSteps = new () {
			    new MapGenerationStep("CollectInfo", new object[] { }),
			    new MapGenerationStep("WipeMap", new object[] { DungeonTiles.CAVE_BLANK }),
			    new MapGenerationStep("SetEntrance", new object[] { 0x19, 0x35 }),
			    new MapGenerationStep("EarthB2Style", new object[] { 18, 22, 6, 4, 0, new PgFeature[] { DungeonTiles.LICH_ROOM }, 12 }),
			    new MapGenerationStep("ConnectRegions", new object[] { }),
			    new MapGenerationStep("ApplyFilter", new object[] { dt.earth_cave_walls, false }),
			    new MapGenerationStep("ApplyFilter", new object[] { dt.earth_cave_extend_walls, true }),
			    new MapGenerationStep("ApplyFilter", new object[] { dt.earth_cave_walls2, false }),
			    new MapGenerationStep("ApplyFilter", new object[] { dt.earth_cave_extend_walls, true }),
			    new MapGenerationStep("ApplyFilter", new object[] { dt.earth_cave_walls3, false }),
			    new MapGenerationStep("ApplyFilter", new object[] { dt.earth_cave_extend_walls, true }),
			    new MapGenerationStep("ApplyFilter", new object[] { dt.earth_cave_walls4, true }),
			    new MapGenerationStep("ApplyFilter", new object[] { dt.cave_room_walls, false }),
			    new MapGenerationStep("ApplyFilter", new object[] { dt.cave_room_walls2, false }),
			    new MapGenerationStep("PlaceEntrance", new object[] { DungeonTiles.CAVE_EARTH_WARP }),
			    //new MapGenerationStep("SanityCheck", new object[] { }),
			};
			break;
		    default:
			return new CompleteMap { Map = maps[(int)mapId] };
		}

		if (mapGenSteps != null) {
		    var tileset = new TileSet(rom, rom.GetMapTilesetIndex(mapId));
		    var blankState = new MapState(rng, mapGenSteps, maps[(int)mapId], dt, tileset, progress);
		    var worldState = await ProgenFramework.RunSteps<MapState, MapResult, MapGenerationStep>(blankState, 5000, progress);
		    if (worldState != null) {
			return new CompleteMap { Map = worldState.Tilemap };
		    }
		}
	    }

	}
    }

}
