using System.Reflection;
using FF1Lib.Sanity;
using System.Diagnostics;

namespace FF1Lib.Procgen
{

    // Something we're going to place on the map.
    // A town, castle, dungeon, cave, etc.
    public class PgFeature {
        public byte[,] Tiles;
        public Dictionary<string, SCCoords> Entrances;
	public bool MountainCave;

        public PgFeature(byte[,] tiles,
            Dictionary<string, SCCoords> entrances) {
                this.Tiles = tiles;
                this.Entrances = entrances;
		this.MountainCave = false;
            }

        public PgFeature(byte tile, string entrance, bool mountainCave) {
	    this.MountainCave = mountainCave;
	    this.Entrances = new Dictionary<string, SCCoords>();
	    if (this.MountainCave) {
		this.Tiles = new byte[,] {
		    { tile, },
		};
		    this.Entrances[entrance] = new SCCoords(0, 0);
	    } else {
		this.Tiles = new byte[,] {
		    { OverworldTiles.None, OverworldTiles.None, OverworldTiles.None },
		    { OverworldTiles.None, tile, OverworldTiles.None },
		    { OverworldTiles.None, OverworldTiles.None, OverworldTiles.None },
		};
		this.Entrances[entrance] = new SCCoords(1, 1);
	    }
	}

	public PgFeature NoneToMarsh() {
	    if (this.MountainCave) {
		return this;
	    }
	    var newtiles = new byte[this.Tiles.GetLength(0),this.Tiles.GetLength(1)];
	    for (int j = 0; j < this.Tiles.GetLength(0); j++) {
		for (int i = 0; i < this.Tiles.GetLength(1); i++) {
		    if (this.Tiles[j,i] == OverworldTiles.None) {
			newtiles[j,i] = OverworldTiles.MARSH;
		    } else {
			newtiles[j,i] = this.Tiles[j,i];
		    }
		}
	    }
	    return new PgFeature(newtiles, this.Entrances);
	}
    }

    public abstract class ProcgenState<ResultType, GenStepType, GenerationTaskType>
	where ResultType : Result<ProcgenState<ResultType, GenStepType, GenerationTaskType>, GenStepType, GenerationTaskType>, new()
	where GenStepType : GenerationStep<ProcgenState<ResultType, GenStepType, GenerationTaskType>, ResultType>
    {
        protected MT19337 rng;
        public Queue<GenStepType> StepQueue;
	public FF1Rom.ReportProgress progress;

	public ProcgenState(MT19337 rng, List<GenStepType> steps, FF1Rom.ReportProgress progress) {
	    this.rng = rng;
	    this.StepQueue = new Queue<GenStepType>(steps);
	    this.progress = progress;
	}

	public ProcgenState(ProcgenState<ResultType, GenStepType, GenerationTaskType> copy) {
	    this.rng = copy.rng;
	    this.StepQueue = copy.StepQueue;
	    this.progress = copy.progress;
	}

        public async Task<ResultType> NextStep() {
            if (this.StepQueue.Count == 0) {
                var rt = new ResultType();
		rt.SetState(this);
		return rt;
            }
            this.StepQueue = new Queue<GenStepType>(this.StepQueue);
            var nextStep = this.StepQueue.Dequeue();
	    await this.progress($"{(nextStep.message != "" ? nextStep.message : nextStep.method.Name)}");
	    Console.WriteLine(nextStep.method.Name);
            return await nextStep.RunStep(this);
        }
    }

    public abstract class Result<StateT, GenStepType, GenerationTaskType> {
        public StateT final;
        public List<GenerationTaskType> additionalTasks;

	public bool Success { get { return this.final != null || this.additionalTasks != null; } }

	public void SetState(StateT st) {
	    this.final = st;
	    this.additionalTasks = null;
	}

	public Result() { }

	public Result(bool f) {
	    this.final = default(StateT);
	    this.additionalTasks = null;
	}

        public Result(StateT f) {
            this.final = f;
            this.additionalTasks = null;
        }

        public Result(List<GenerationTaskType> tasks) {
            this.final = default(StateT);
            this.additionalTasks = tasks;
        }
    }

    public abstract class GenerationStep<StateT, ResultType> {
        public MethodInfo method;
        object[] parameters;
	public string message;
        public GenerationStep(string className, string methodName, object[] parameters, string message="") {
            Type magicType = Type.GetType(className);
            this.method = magicType.GetMethod(methodName);
	    if (method == null) {
		Console.WriteLine($"No method {methodName}");
	    }
            Debug.Assert(method != null);
            this.parameters = parameters;
	    this.message = message;
        }

        public Task<ResultType> RunStep(StateT st) {
            return (Task<ResultType>)method.Invoke(st, this.parameters);
        }
    }

    public static class ProgenFramework {

	public delegate Task<ResultType> GenerationTaskType<ResultType>();

	public static async Task<StateT> RunSteps<StateT, ResultType, GenStepType>(StateT startingState, int maxTasksCount, FF1Rom.ReportProgress progress)
	    where ResultType : Result<ProcgenState<ResultType, GenStepType, GenerationTaskType<ResultType>>, GenStepType, GenerationTaskType<ResultType>>, new()
	    where StateT : ProcgenState<ResultType, GenStepType, GenerationTaskType<ResultType>>
	    where GenStepType : GenerationStep<ProcgenState<ResultType, GenStepType, GenerationTaskType<ResultType>>, ResultType>
{
	    Stack<GenerationTaskType<ResultType>> workStack = new Stack<GenerationTaskType<ResultType>>();

	    workStack.Push(startingState.NextStep);

	    StateT finalState = null;
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
		    finalState = (StateT)r.final;
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
    }

}
