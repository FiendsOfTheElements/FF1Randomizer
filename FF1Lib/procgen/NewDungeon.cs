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
			    new MapGenerationStep("CollectInfo", new object[] {  }),
			    new MapGenerationStep("WipeMap", new object[] { DungeonTiles.CAVE_BLANK }),
			    new MapGenerationStep("EarthB1Style", new object[] { }),
			    new MapGenerationStep("SetEntrance", new object[] { 30, 30 }),
			    new MapGenerationStep("PlaceTile", new object[] { 30, 30, DungeonTiles.CAVE_EARTH_B1_ENTRANCE }),
			    new MapGenerationStep("PlaceTreasureRooms", new object[] { }),
			    new MapGenerationStep("ApplyFilter", new object[] { dt.cave_rock_walls, false }),
			    new MapGenerationStep("ApplyFilter", new object[] { dt.cave_room_walls, false }),
			    new MapGenerationStep("ApplyFilter", new object[] { dt.cave_room_walls2, false }),
			    new MapGenerationStep("PlaceExitStairs", new object[] { 30, 30, DungeonTiles.CAVE_EARTH_B1_EXIT }),
			    new MapGenerationStep("PlaceChests", new object[] { }),
			    new MapGenerationStep("SanityCheck", new object[] { }),
			};
			break;
		    default:
			return new CompleteMap { Map = maps[(int)mapId] };
		}

		if (mapGenSteps != null) {
		    var tileset = new TileSet(rom, rom.GetMapTilesetIndex(mapId));
		    var blankState = new MapState(rng, mapGenSteps, maps[(int)mapId], dt, tileset, progress);
		    var worldState = await ProgenFramework.RunSteps<MapState, MapResult, MapGenerationStep>(blankState, progress);
		    if (worldState != null) {
			return new CompleteMap { Map = worldState.Tilemap };
		    }
		}
	    }

	}
    }

}
