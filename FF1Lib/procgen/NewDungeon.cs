using System.Reflection;
using FF1Lib.Sanity;
using System.Diagnostics;
using MapGenerationTask = FF1Lib.Procgen.ProgenFramework.GenerationTaskType<FF1Lib.Procgen.MapResult>;

namespace FF1Lib.Procgen
{


    public partial class MapState : ProcgenState<MapResult, MapGenerationStep, ProgenFramework.GenerationTaskType<MapResult>>{
        public const int MAPSIZE = 64;
	//private bool ownTilemap;
        public byte[,] Tilemap;

        public MapState(MT19337 rng, List<MapGenerationStep> steps, OverworldTiles overworldTiles, FF1Rom.ReportProgress progress) : base(rng, steps, progress) {
            //this.ownTilemap = true;
	    this.Tilemap = new byte[MAPSIZE,MAPSIZE];
	}

        public MapState(MapState copy) : base(copy) {
            //this.ownTilemap = false;
	    this.Tilemap = copy.Tilemap;
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

	public async static Task<CompleteMap> GenerateNewMap(MT19337 rng, FF1Lib.MapId mapId, List<Map> maps, FF1Rom.ReportProgress progress) {
	    await progress("", 1);
	    return new CompleteMap { Map = maps[(int)mapId] };
	}
    }

}
