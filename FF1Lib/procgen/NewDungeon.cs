using System.Reflection;
using FF1Lib.Sanity;
using System.Diagnostics;

namespace FF1Lib.Procgen
{

    /*
    public partial class MapState {
        public const int MAPSIZE = 64;
        public byte[,] Tilemap;
	public Queue<OwGenerationStep> StepQueue;

	FF1Rom.ReportProgress progress;

        public MapState(MT19337 rng, List<GenerationStep> steps, OverworldTiles overworldTiles, FF1Rom.ReportProgress progress) {
            this.rng = rng;
            this.ownTilemap = true;
	    this.progress = progress;
	    this.Tilemap = new byte[MAPSIZE,MAPSIZE];
	    this.StepQueue = new Queue<OwGenerationStep>(steps);
	}

        public MapState(MapState copy) {
            this.ownTilemap = false;
            this.rng = copy.rng;
	    this.Tilemap = copy.Tilemap;
	    this.StepQueue = copy.StepQueue;
	    this.progress = copy.progress;
	}

        public async Task<Result> NextStep() {
            if (this.StepQueue.Count == 0) {
                return new Result(this);
            }
            this.StepQueue = new Queue<MapGenerationStep>(this.StepQueue);
            var nextStep = this.StepQueue.Dequeue();
	    await this.progress($"{(nextStep.message != "" ? nextStep.message : nextStep.method.Name)}");
	    Console.WriteLine(nextStep.method.Name);
            return await nextStep.RunStep(this);
        }

    }

    public class MapGenerationStep {
        public MethodInfo method;
        object[] parameters;
	public string message;
        public GenerationStep(string methodName, object[] parameters, string message="") {
            Type magicType = Type.GetType("FF1Lib.Procgen.MapState");
            this.method = magicType.GetMethod(methodName);
            Debug.Assert(method != null);
            this.parameters = parameters;
	    this.message = message;
        }

        public Task<Result> RunStep(MapState st) {
            return (Task<Result>)method.Invoke(st, this.parameters);
        }
    }

    public static class NewDungeon {
	public static async Task<OverworldState> RunSteps(OverworldState startingState, FF1Rom.ReportProgress progress) {
	    Stack<GenerationTask> workStack = new Stack<GenerationTask>();

	    workStack.Push(startingState.NextStep);

	    OverworldState finalState = null;
	    int maxTasksCount = 150;
	    int taskCount = 0;

	    await progress("", startingState.StepQueue.Count);
	    await progress("", maxTasksCount);
	    while (workStack.Count > 0 && taskCount < maxTasksCount) {
		await progress();
		System.GC.Collect(System.GC.MaxGeneration);

		taskCount += 1;
		var p = workStack.Pop();
		var r = await p();
		if (r.final != null) {
		    finalState = r.final;
		    break;
		}
		if (r.additionalTasks != null) {
		    await progress("", r.additionalTasks.Count);
		    foreach (var v in r.additionalTasks) {
			workStack.Push(v);
		    }
		}
	    }
	    return finalState;
	}

	public async static Task<CompleteMap> GenerateNewMap(MT19337 rng, FF1Rom.ReportProgress progress) {

	}
    }
    */
}
