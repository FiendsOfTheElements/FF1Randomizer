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

        public MapState(MT19337 rng, List<MapGenerationStep> steps, FF1Rom.ReportProgress progress) : base(rng, steps, progress) {
            this.ownTilemap = true;
	    this.Tilemap = new Map(0);
	}

        public MapState(MapState copy) : base(copy) {
            this.ownTilemap = false;
	    this.Tilemap = copy.Tilemap;
	}

        void OwnTilemap() {
            if (this.ownTilemap) {
                return;
            }
	    this.Tilemap = this.Tilemap.Clone();
            this.ownTilemap = true;
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

	public async static Task<CompleteMap> GenerateNewMap(MT19337 rng, MapId mapId, List<Map> maps, FF1Rom.ReportProgress progress) {
	    await progress("", 1);

	    List<MapGenerationStep> mapGenSteps = null;

	    while (true) {
		switch (mapId) {
		    case MapId.Coneria:
			mapGenSteps = new () {
			    new MapGenerationStep("WipeMap", new object[] { (byte)0x10 }),
			    new MapGenerationStep("PlaceShop", new object[] { }),
			};
			break;
		    case MapId.EarthCaveB1:
			mapGenSteps = new () {
			    new MapGenerationStep("WipeMap", new object[] { DungeonTiles.CAVE_BLANK }),
			    new MapGenerationStep("EarthB1Style", new object[] { }),
			    new MapGenerationStep("PlaceTreasureRoom", new object[] { }),
			};
			break;
		    default:
			return new CompleteMap { Map = maps[(int)mapId] };
		}

		if (mapGenSteps != null) {
		    var blankState = new MapState(rng, mapGenSteps, progress);
		    var worldState = await ProgenFramework.RunSteps<MapState, MapResult, MapGenerationStep>(blankState, progress);
		    if (worldState != null) {
			return new CompleteMap { Map = worldState.Tilemap };
		    }
		}
	    }

	}
    }

}
