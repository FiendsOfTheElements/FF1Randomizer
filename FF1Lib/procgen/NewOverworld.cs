using System.Reflection;
using FF1Lib.Sanity;
using System.Diagnostics;
using OwGenerationTask = FF1Lib.Procgen.ProgenFramework.GenerationTaskType<FF1Lib.Procgen.OwResult>;

namespace FF1Lib.Procgen
{

	public static class RNGExtensions {
       public static double Uniform(this MT19337 rng, double low, double high)
        {
                double range = (high - low);
                return low + range * ((double)rng.Next() / uint.MaxValue);
        }
    }

    public class FailedToGenerate : Exception {
	public FailedToGenerate(string message) : base(message) { }
    }

    // A contigious region of the map.
    // There are two types of regions, computed separately.
    //
    // - Traversable regions (defined by TraversableRegionTypes)
    // these are contigous walkable, canoe-able, or sail-able regions
    //
    // - Biome regions (defined by BiomeRegionTypes)
    // this breaks down the walkable regions into forest, marsh, desert, etc.
    public class OwRegion {
        public int RegionType;
        public short RegionId;
        public List<SCCoords> Points;
        public List<short> Adjacent;

        public OwRegion(int regionType, short regionid) {
            this.RegionType = regionType;
            this.RegionId = regionid;
            this.Points = new List<SCCoords>();
            this.Adjacent = new List<short>();
        }

        public OwRegion(OwRegion copy) {
            this.RegionType = copy.RegionType;
            this.RegionId = copy.RegionId;
            this.Points = new List<SCCoords>(copy.Points);
            this.Adjacent = new List<short>(copy.Adjacent);
        }

        public void AddPoint(SCCoords p) {
            this.Points.Add(p);
        }

	// Walks the entire map to produce a region list and a region map.
	// The region list is a list of all regions.
	// The region map is the assignment of every position on the map to a region,
	// as in index into the corresponding region list.
	//
	// Because we start in the upper left corner, region 0 is
	// always the main ocean (OverworldTiles.MainOceanRegionId)
        public static Tuple<short[,], List<OwRegion>> FindRegions(byte[,] tilemap, Dictionary<byte, int> tileRegionTypeMap) {
            var regionMap = new short[OverworldState.MAPSIZE,OverworldState.MAPSIZE];
            var regionList = new List<OwRegion>();

            for (int j = 0; j < OverworldState.MAPSIZE; j++) {
                for (int i = 0; i < OverworldState.MAPSIZE; i++) {
                    regionMap[j,i] = -1;
                }
            }

            regionList.Add(new OwRegion(tileRegionTypeMap[tilemap[0,0]], 0));

            var workingStack = new Stack<ValueTuple<SCCoords, OwRegion>>();
            var workingQueue = new Queue<ValueTuple<SCCoords, OwRegion>>();

            workingStack.Push(ValueTuple.Create(new SCCoords(0, 0), regionList[0]));

            while (workingStack.Count > 0) {
                while (workingStack.Count > 0) {
                    var tp = workingStack.Pop();
                    var p = tp.Item1;
                    var rg = tp.Item2;
                    if (regionMap[p.Y,p.X] != -1) {
                        continue;
                    }

                    regionMap[p.Y,p.X] = rg.RegionId;
                    rg.AddPoint(p);

                    var curRegionType = tileRegionTypeMap[tilemap[p.Y,p.X]];

                    var adjacent = new SCCoords[] {
                        p.OwUp, p.OwRight, p.OwDown, p.OwLeft
                    };
                    foreach (var adj in adjacent) {
                        var adjRegionType = tileRegionTypeMap[tilemap[adj.Y,adj.X]];
                        if (adjRegionType == curRegionType) {
                            if (regionMap[adj.Y,adj.X] == -1) {
                                workingStack.Push(ValueTuple.Create(adj, rg));
                            }
                        } else {
                            workingQueue.Enqueue(ValueTuple.Create(adj, rg));
                        }
                    }
                }
                while (workingQueue.Count > 0 && workingStack.Count == 0) {
                    var tp = workingQueue.Dequeue();
                    var p = tp.Item1;
                    var rg = tp.Item2;
                    OwRegion nextRegion;
                    if (regionMap[p.Y,p.X] == -1) {
                        nextRegion = new OwRegion(tileRegionTypeMap[tilemap[p.Y,p.X]], (short)regionList.Count);
                        regionList.Add(nextRegion);
                        workingStack.Push(ValueTuple.Create(p, nextRegion));
                    } else {
                        nextRegion = regionList[regionMap[p.Y,p.X]];
                    }
                    if (rg != nextRegion) {
                        if (!regionList[rg.RegionId].Adjacent.Contains(nextRegion.RegionId)) {
                            regionList[rg.RegionId].Adjacent.Add(nextRegion.RegionId);
                        }
                        if (!regionList[nextRegion.RegionId].Adjacent.Contains(rg.RegionId)) {
                            regionList[nextRegion.RegionId].Adjacent.Add(rg.RegionId);
                        }
                    }
                }
            }

            return Tuple.Create(regionMap, regionList);
        }

	// Cut out a rectangular section of a region, adding a new
	// region for the cut out, and replacing the entry in the
	// region list with an updated region.
	public static short Cutout(short[,] regionMap, List<OwRegion> regionList,
				  SCCoords cp, int w, int h) {
	    OwRegion orig = regionList[regionMap[cp.Y, cp.X]];

	    var cutout = new OwRegion(OverworldTiles.OTHER_REGION, (short)regionList.Count);
	    regionList.Add(cutout);

	    var replacement = new OwRegion(orig.RegionType, orig.RegionId);
	    regionList[replacement.RegionId] = replacement;

	    foreach (var tp in orig.Points) {
		if (tp.X >= cp.X &&
		    tp.X < (cp.X+w) &&
		    tp.Y >= cp.Y &&
		    tp.Y < (cp.Y+h))
		{
		    cutout.AddPoint(tp);
		    regionMap[tp.Y, tp.X] = cutout.RegionId;
		} else {
		    replacement.AddPoint(tp);
		}
	    }

	    foreach (var adj in orig.Adjacent) {
		replacement.Adjacent.Add(adj);
	    }
	    replacement.Adjacent.Add(cutout.RegionId);

	    for (int x = 0; x < w+2; x++) {
		for (int y = 0; y < h+2; y++) {
		    // top edge
		    var px = cp.X-1+x;
		    var py = cp.Y-1;
		    px = ((px%OverworldState.MAPSIZE) + OverworldState.MAPSIZE) % OverworldState.MAPSIZE;
		    py = ((py%OverworldState.MAPSIZE) + OverworldState.MAPSIZE) % OverworldState.MAPSIZE;
		    var pr = regionMap[py, px];
		    if (! cutout.Adjacent.Contains(pr)) {
			cutout.Adjacent.Add(pr);
		    }

		    // right edge
		    px = cp.X+w;
		    py = cp.Y-1+y;
		    px = ((px%OverworldState.MAPSIZE) + OverworldState.MAPSIZE) % OverworldState.MAPSIZE;
		    py = ((py%OverworldState.MAPSIZE) + OverworldState.MAPSIZE) % OverworldState.MAPSIZE;
		    pr = regionMap[py, px];
		    if (! cutout.Adjacent.Contains(pr)) {
			cutout.Adjacent.Add(pr);
		    }

		    // bottom edge
		    px = cp.X-1+x;
		    py = cp.Y+h;
		    px = ((px%OverworldState.MAPSIZE) + OverworldState.MAPSIZE) % OverworldState.MAPSIZE;
		    py = ((py%OverworldState.MAPSIZE) + OverworldState.MAPSIZE) % OverworldState.MAPSIZE;
		    pr = regionMap[py, px];
		    if (! cutout.Adjacent.Contains(pr)) {
			cutout.Adjacent.Add(pr);
		    }

		    // left edge
		    px = cp.X-1;
		    py = cp.Y-1+y;
		    px = ((px%OverworldState.MAPSIZE) + OverworldState.MAPSIZE) % OverworldState.MAPSIZE;
		    py = ((py%OverworldState.MAPSIZE) + OverworldState.MAPSIZE) % OverworldState.MAPSIZE;
		    pr = regionMap[py, px];
		    if (! cutout.Adjacent.Contains(pr)) {
			cutout.Adjacent.Add(pr);
		    }
		}
	    }

	    return cutout.RegionId;
	}

	public static void Merge(short[,] regionMap, List<OwRegion> regionList,
				 OwRegion src, OwRegion dst)
	{
	    src = new OwRegion(src);
	    dst = new OwRegion(dst);
	    regionList[src.RegionId] = src;
	    regionList[dst.RegionId] = dst;
	    foreach (var p in src.Points) {
		dst.AddPoint(p);
		regionMap[p.Y, p.X] = dst.RegionId;
	    }
	    src.Points.Clear();
	    foreach (var a in src.Adjacent) {
		// Make anything adjacent to src,
		// adjacent to dst
		if (!dst.Adjacent.Contains(a)) {
		    dst.Adjacent.Add(a);
		}
		regionList[a] = new OwRegion(regionList[a]);
		regionList[a].Adjacent.Remove(src.RegionId);
		if (!regionList[a].Adjacent.Contains(dst.RegionId)) {
		    regionList[a].Adjacent.Add(dst.RegionId);
		}
	    }
	    src.Adjacent.Clear();
	}
    }


    // This is the class that does most of the work.  It holds the
    // state of the map generation up to this point, and defines all
    // the methods that apply transformations to the map.
    public partial class OverworldState : ProcgenState<OwResult, OwGenerationStep, ProgenFramework.GenerationTaskType<OwResult>> {
        public const int MAPSIZE = 256;
        float[,] Basemap;
        public byte[,] Tilemap;
        short[,] Biome_regionmap;
        List<OwRegion> Biome_regionlist;
        public short[,] Traversable_regionmap;
        public List<OwRegion> Traversable_regionlist;
        byte[,] Feature_weightmap;
        byte[,] Dock_weightmap;
        public Dictionary<string, SCCoords> FeatureCoordinates;
        List<int> Reachable_regions;
        List<int> Exclude_docks;
	List<int> Exclude_airship;
        private bool ownBasemap;
        private bool ownTilemap;
        private bool ownRegions;
        private bool ownPlacements;

        float heightmax;
        float mountain_elevation;
        float sea_elevation;
	public List<ValueTuple<short, SCCoords>> DockPlacements;

        private OverworldTiles overworldTiles;

	short startingRegion;
	short bridgeOriginRegion;
	short bridgedRegion;
	bool shouldPlaceBridge;
	short canalRegion;
	short airshipDesertRegion;

        public OverworldState(MT19337 rng, List<OwGenerationStep> steps, OverworldTiles overworldTiles, FF1Rom.ReportProgress progress) : base(rng, steps, progress) {
            this.ownBasemap = true;
            this.ownTilemap = true;
            this.ownRegions = true;
            this.ownPlacements = true;
            this.Basemap = new float[MAPSIZE,MAPSIZE];
            this.Tilemap = new byte[MAPSIZE,MAPSIZE];
            this.Biome_regionmap = new short[MAPSIZE,MAPSIZE];
            this.Biome_regionlist = new List<OwRegion>();
            this.Traversable_regionmap = new short[MAPSIZE,MAPSIZE];
            this.Traversable_regionlist = new List<OwRegion>();
            this.Feature_weightmap  = new byte[MAPSIZE,MAPSIZE];
            this.Dock_weightmap = new byte[MAPSIZE,MAPSIZE];
            this.FeatureCoordinates = new Dictionary<string, SCCoords>();
            this.Reachable_regions = new List<int>();
            this.Exclude_docks = new List<int>();
            this.Exclude_airship = new List<int>();
	    this.DockPlacements = new List<ValueTuple<short, SCCoords>>();
            this.overworldTiles = overworldTiles;
	    this.startingRegion = -1;
	    this.bridgeOriginRegion = -1;
	    this.bridgedRegion = -1;
	    this.canalRegion = -1;
	    this.airshipDesertRegion = -1;
	    this.shouldPlaceBridge = true;
        }

	// Shallow copy construtor.  To be memory efficient, this uses
	// a copy-on-write strategy.  The Own* methods below perform
	// deep copies only when needed to have a local copy on which
	// to make changes.
        public OverworldState(OverworldState copy) : base(copy) {
            this.ownBasemap = false;
            this.ownTilemap = false;
            this.ownRegions = false;
            this.ownPlacements = false;
            this.Basemap = copy.Basemap;
            this.Tilemap = copy.Tilemap;
            this.Biome_regionmap = copy.Biome_regionmap;
            this.Biome_regionlist = copy.Biome_regionlist;
            this.Traversable_regionmap = copy.Traversable_regionmap;
            this.Traversable_regionlist = copy.Traversable_regionlist;
            this.Feature_weightmap  = copy.Feature_weightmap;
            this.Dock_weightmap = copy.Dock_weightmap;
            this.FeatureCoordinates = copy.FeatureCoordinates;
            this.Reachable_regions = copy.Reachable_regions;
            this.Exclude_docks = copy.Exclude_docks;
            this.Exclude_airship = copy.Exclude_airship;
	    this.DockPlacements = copy.DockPlacements;
            this.heightmax = copy.heightmax;
            this.mountain_elevation = copy.mountain_elevation;
            this.sea_elevation = copy.sea_elevation;
            this.overworldTiles = copy.overworldTiles;
	    this.startingRegion = copy.startingRegion;
	    this.bridgeOriginRegion = copy.bridgeOriginRegion;
	    this.bridgedRegion = copy.bridgedRegion;
	    this.canalRegion = copy.canalRegion;
	    this.airshipDesertRegion = copy.airshipDesertRegion;
	    this.shouldPlaceBridge = copy.shouldPlaceBridge;
        }

	public void SetSteps(List<OwGenerationStep> steps) {
	    this.StepQueue = new Queue<OwGenerationStep>(steps);
	}

        void OwnBasemap() {
            if (this.ownBasemap) {
                return;
            }
            var fromBasemap = this.Basemap;
            this.Basemap = new float[MAPSIZE,MAPSIZE];
            Array.Copy(fromBasemap, this.Basemap, fromBasemap.Length);
            this.ownBasemap = true;
        }

        void OwnTilemap() {
            if (this.ownTilemap) {
                return;
            }
            var fromTilemap = this.Tilemap;
            this.Tilemap = new byte[MAPSIZE,MAPSIZE];
            Array.Copy(fromTilemap, this.Tilemap, fromTilemap.Length);
            this.ownTilemap = true;
        }

        void OwnRegions() {
                if (this.ownRegions) {
                    return;
                }
                var fromBiome_regionmap = this.Biome_regionmap;
                this.Biome_regionmap = new short[MAPSIZE,MAPSIZE];
                Array.Copy(fromBiome_regionmap, this.Biome_regionmap, fromBiome_regionmap.Length);
                this.Biome_regionlist = new List<OwRegion>(this.Biome_regionlist);

                var fromTraversable_regionmap = this.Traversable_regionmap;
                this.Traversable_regionmap = new short[MAPSIZE,MAPSIZE];
                Array.Copy(fromTraversable_regionmap, this.Traversable_regionmap, fromTraversable_regionmap.Length);
                this.Traversable_regionlist = new List<OwRegion>(this.Traversable_regionlist);
                this.ownRegions = true;
        }

        void OwnPlacements() {
            if (this.ownPlacements) {
                return;
            }
            var fromFeature_weightmap = this.Feature_weightmap;
            this.Feature_weightmap = new byte[MAPSIZE,MAPSIZE];
            Array.Copy(fromFeature_weightmap, this.Feature_weightmap, fromFeature_weightmap.Length);

            var fromDock_weightmap = this.Dock_weightmap;
            this.Dock_weightmap = new byte[MAPSIZE,MAPSIZE];
            Array.Copy(fromDock_weightmap, this.Dock_weightmap, fromDock_weightmap.Length);

            this.FeatureCoordinates = new Dictionary<string, SCCoords>(this.FeatureCoordinates);
            this.Reachable_regions = new List<int>(this.Reachable_regions);
            this.Exclude_docks = new List<int>(this.Exclude_docks);
            this.Exclude_airship = new List<int>(this.Exclude_airship);
	    this.DockPlacements = new List<ValueTuple<short, SCCoords>>(this.DockPlacements);
            this.ownPlacements = true;
        }

        const float UNSET = -1000000;
        const float perturb_reduction = .63f;

	// The heightmap generation function.  It implements the
	// diamond square algorithm.  The general idea is to start by
	// generating random heights in the corners and center of the
	// box, then recursively apply the function to each quadrant,
	// interpolating heights on the midpoint sides and center of
	// the box, and applying a random factor, which gets smaller
	// with each iteration.
        void PerturbPoints(int x0, int y0, int x1, int y1, float r0) {
            if (Math.Abs(x0 - x1) <= 1 && Math.Abs(y0 - y1) <= 1) {
                return;
            }

            int x2, y2;

            if (x0+1 == x1-1) {
                x2 = x0+1;
            } else if (x0 == x1) {
                x2 = x0;
            } else if (x0+1 == x1) {
                x2 = x0;
            } else {
                x2 = (x0+x1)/2;
            }

            if (y0+1 == y1-1) {
                y2 = y0+1;
            } else if (y0 == y1) {
                y2 = y0;
            } else if (y0+1 == y1) {
                y2 = y0;
            } else {
                y2 = (y0+y1)/2;
            }

            float midp;
            // Middle
            if (this.Basemap[y2,x2] == UNSET) {
                midp = (this.Basemap[y0,x0]+this.Basemap[y1,x0]+this.Basemap[y0,x1]+this.Basemap[y1,x1])/4.0f;
                this.Basemap[y2,x2] = midp + (float)rng.Uniform(-r0, r0);
            }

            // Left middle
            if (this.Basemap[y2,x0] == UNSET) {
                midp = (this.Basemap[y0,x0]+this.Basemap[y1,x0])/2.0f;
                this.Basemap[y2,x0] = midp + (float)rng.Uniform(-r0, r0);
            }

            // Right middle
            if (this.Basemap[y2,x1] == UNSET) {
                midp = (this.Basemap[y0,x1]+this.Basemap[y1,x1])/2.0f;
                this.Basemap[y2,x1] = midp + (float)rng.Uniform(-r0, r0);
            }

            // top middle
            if (this.Basemap[y0,x2] == UNSET) {
                midp = (this.Basemap[y0,x0]+this.Basemap[y0,x1])/2.0f;
                this.Basemap[y0,x2] = midp + (float)rng.Uniform(-r0, r0);
            }

            // bottom middle
            if (this.Basemap[y1,x2] == UNSET) {
                midp = (this.Basemap[y1,x0]+this.Basemap[y1,x1])/2.0f;
                this.Basemap[y1,x2] = midp + (float)rng.Uniform(-r0, r0);
            }

            r0 *= perturb_reduction;

            this.PerturbPoints(x0, y0, x2, y2, r0);
            this.PerturbPoints(x2, y0, x1, y2, r0);
            this.PerturbPoints(x0, y2, x2, y1, r0);
            this.PerturbPoints(x2, y2, x1, y1, r0);
        }

	// This is the starting point for map generation.  It uses
	// PeturbPoints to create a height map, then iteratively
	// determines the elevations that result in a certain minimum
	// amount of mountains and walkable land.  It then creates the
	// initial tile map of consisting of just mountain, land, and
	// ocean.
        public async Task<OwResult> CreateInitialMap() {
            this.OwnBasemap();
            this.OwnTilemap();

            for (int y = 0; y < MAPSIZE; y++) {
                for (int x = 0; x < MAPSIZE; x++) {
                    this.Basemap[y,x] = UNSET;
                }
            }

            const int border = 8;
            for (int b = 0; b <= border; b++) {
                for (int i = 0; i < MAPSIZE; i++) {
                    this.Basemap[b,i] = 0;
                    this.Basemap[MAPSIZE-1-b,i] = 0;
                    this.Basemap[i,b] = 0;
                    this.Basemap[i,MAPSIZE-1-b] = 0;
                }
            }
            this.Basemap[MAPSIZE/2-1, MAPSIZE/2-1] = 0;

            this.PerturbPoints(border, border, MAPSIZE-1-border, MAPSIZE-1-border, 1);

            this.heightmax = -100;
	    for (int y = 0; y < MAPSIZE; y++) {
		for (int x = 0; x < MAPSIZE; x++) {
		    var height = this.Basemap[y,x];
		    if (height > heightmax) {
			this.heightmax = height;
		    }
		}
	    }

            const double land_pct = .28;
            const double mountain_pct = 0.045;

            this.mountain_elevation = this.heightmax;
            this.sea_elevation = this.heightmax;

            int mountain_count = 0;
            int land_count = 0;
            int min_land_tiles = (int)(MAPSIZE*MAPSIZE*land_pct);
            int min_mtn_tiles = (int)(MAPSIZE*MAPSIZE*mountain_pct);

            int lowering_iter = 0;
            while (land_count < min_land_tiles || mountain_count < min_mtn_tiles) {
		if (lowering_iter % 10 == 0) {
		    await this.progress("", 10);
		}

                lowering_iter += 1;
                mountain_count = 0;
                land_count = 0;
                for (int y = 0; y < MAPSIZE; y++) {
                    for (int x = 0; x < MAPSIZE; x++) {
                        var height = this.Basemap[y,x];
                        if (height > heightmax) {
                            this.heightmax = height;
                        }
                        if (height > mountain_elevation) {
                            mountain_count += 1;
                        }
                        else if (height > sea_elevation) {
                            land_count += 1;
                        }
                    }
                }

                if (land_count < min_land_tiles) {
                    sea_elevation -= .002f;
                }
                if (mountain_count < min_mtn_tiles) {
                    mountain_elevation -= .002f;
                }
                if (sea_elevation <= 0) {
                    return new OwResult(false);
                }
            }

            for (int y = 0; y < MAPSIZE; y++) {
                for (int x = 0; x < MAPSIZE; x++) {
                    var height = this.Basemap[y,x];
                    if (height > mountain_elevation) {
                        this.Tilemap[y,x] = OverworldTiles.MOUNTAIN;
                    }
                    else if (height > sea_elevation) {
                        this.Tilemap[y,x] = OverworldTiles.LAND;
                    }
                    else {
                        this.Tilemap[y,x] = OverworldTiles.OCEAN;
                    }
                }
            }

            return await this.NextStep();
        }

        public async Task<OwResult> CreateLostWoodsMap() {
            this.OwnBasemap();
            this.OwnTilemap();

            for (int y = 0; y < MAPSIZE; y++) {
                for (int x = 0; x < MAPSIZE; x++) {
                    this.Basemap[y,x] = UNSET;
                }
            }

            const int border = 8;
            for (int b = 0; b <= border; b++) {
                for (int i = 0; i < MAPSIZE; i++) {
                    this.Basemap[b,i] = 0;
                    this.Basemap[MAPSIZE-1-b,i] = 0;
                    this.Basemap[i,b] = 0;
                    this.Basemap[i,MAPSIZE-1-b] = 0;
                }
            }
            this.Basemap[MAPSIZE/2-1, MAPSIZE/2-1] = 0;

            this.PerturbPoints(border, border, MAPSIZE-1-border, MAPSIZE-1-border, 1);

            this.heightmax = -100;
	    for (int y = 0; y < MAPSIZE; y++) {
		for (int x = 0; x < MAPSIZE; x++) {
		    var height = this.Basemap[y,x];
		    if (height > heightmax) {
			this.heightmax = height;
		    }
		}
	    }

            const double mountain_pct = 0.20;

            this.mountain_elevation = this.heightmax;

            int mountain_count = 0;
            int min_mtn_tiles = (int)(MAPSIZE*MAPSIZE*mountain_pct);

            int lowering_iter = 0;
            while (mountain_count < min_mtn_tiles) {
                lowering_iter += 1;
                mountain_count = 0;
                for (int y = 0; y < MAPSIZE; y++) {
                    for (int x = 0; x < MAPSIZE; x++) {
                        var height = this.Basemap[y,x];
                        if (height > heightmax) {
                            this.heightmax = height;
                        }
                        if (height > mountain_elevation) {
                            mountain_count += 1;
                        }
                    }
                }

                if (mountain_count < min_mtn_tiles) {
                    mountain_elevation -= .002f;
                }
                if (mountain_elevation <= 0) {
                    return new OwResult(false);
                }
            }

	    var rev = mountain_elevation + (heightmax - mountain_elevation) / 12;

            for (int y = 0; y < MAPSIZE; y++) {
                for (int x = 0; x < MAPSIZE; x++) {
		    if (this.Basemap[y,x] > rev) {
			this.Basemap[y,x] = rev + (rev - this.Basemap[y,x]);
		    }
		    var height = this.Basemap[y,x];
                    if (height > mountain_elevation) {
                        this.Tilemap[y,x] = OverworldTiles.MOUNTAIN;
                    }
                    else {
                        this.Tilemap[y,x] = OverworldTiles.LAND;
                    }
                }
            }

            for (int y = 0; y < MAPSIZE; y++) {
                for (int x = 0; x < MAPSIZE; x++) {
                    var height = this.Basemap[y,x];
                    if (height > mountain_elevation) {
                        this.Tilemap[y,x] = OverworldTiles.MOUNTAIN;
                    }
                    else {
                        this.Tilemap[y,x] = OverworldTiles.LAND;
                    }
                }
            }

            return await this.NextStep();
        }

	// Apply a 3x3 filter over the entire map.
        public Task<OwResult> ApplyFilter(PgTileFilter filter, bool repeat) {
            this.OwnTilemap();
            this.Tilemap = filter.ApplyFilter(this.Tilemap, repeat);
            return this.NextStep();
        }

	// Use gradient decent to create a river.  Starting from a
	// certain tile at a certain elevation, it follows the lowest
	// adjacent tile, then finds the tile adjacent to that tile.
	// It iteratively follows the next lowest adjacent tile until
	// reaching the ocean, or running out of "volume".
        public int FlowRiver(SCCoords p, int volume) {
            this.OwnTilemap();

	    int size = 0;
            var pending = new SortedList<double, SCCoords>();
            pending[-this.Basemap[p.Y,p.X]] = p;
            while (volume > 0 && pending.Count > 0) {
                p = pending.Values[pending.Count-1];
                pending.RemoveAt(pending.Count-1);

                if (this.Tilemap[p.Y,p.X] == OverworldTiles.OCEAN) {
                    return size;
                }

                volume -= 1;

                if (this.Tilemap[p.Y,p.X] == OverworldTiles.RIVER) {
                    continue;
                }

		this.Tilemap[p.Y,p.X] = OverworldTiles.RIVER;
		size += 1;

		var right = p.OwRight;
		var down = p.OwDown;
		var left = p.OwLeft;
		var up = p.OwUp;
		pending[-this.Basemap[right.Y,right.X]] = right;
		pending[-this.Basemap[down.Y,down.X]] = down;
		pending[-this.Basemap[left.Y,left.X]] = left;
		pending[-this.Basemap[up.Y,up.X]] = up;
            }

	    return size;
        }

	// Pick 'count' random points between the lower and upper
	// elevations and flow rivers from those positions.
        public void FlowRivers(double lower_elev, double upper_elev, int count, int volume) {
            var points = new List<SCCoords>();
            for (int y = 0; y < MAPSIZE; y++) {
                for (int x = 0; x < MAPSIZE; x++) {
                    double height = this.Basemap[y,x];
                    if (height >= lower_elev && height <= upper_elev) {
                        points.Add(new SCCoords(x, y));
                    }
                }
            }

            points.Shuffle(this.rng);

            if (count > points.Count) {
                count = points.Count;
            }

            for (int i = 0; i < count; i++) {
                this.FlowRiver(points[i], volume);
            }
        }

	// Try to break up the mountains a bit by introducing valleys.
	// This is somewhat similar to flowing rivers.  Starting from
	// a point, find all points in a contigous area that are lower
	// than that point, and turn them into land.
        public int MakeValley(SCCoords p) {
            this.OwnTilemap();

	    int volume = 128;
	    int size = 0;
            var pending = new Queue<SCCoords>();
            pending.Enqueue(p);
	    double height = this.Basemap[p.Y,p.X];
            while (volume > 0 && pending.Count > 0) {
                p = pending.Dequeue();

                volume -= 1;

                if (this.Basemap[p.Y,p.X] > height ||
		    this.Tilemap[p.Y,p.X] != OverworldTiles.MOUNTAIN) {
                    continue;
                }

		this.Tilemap[p.Y,p.X] = OverworldTiles.LAND;
		size += 1;

		pending.Enqueue(p.OwRight);
		pending.Enqueue(p.OwDown);
		pending.Enqueue(p.OwLeft);
		pending.Enqueue(p.OwUp);
            }
	    return size;
	}

        public Task<OwResult> MakeValleys(int count) {
	    double lower_elev = this.mountain_elevation + (this.heightmax-this.mountain_elevation)*.5;
	    double upper_elev = this.heightmax;

            var points = new List<SCCoords>();
            for (int y = 0; y < MAPSIZE; y++) {
                for (int x = 0; x < MAPSIZE; x++) {
                    float height = this.Basemap[y,x];
                    if (height >= lower_elev && height <= upper_elev) {
                        points.Add(new SCCoords(x, y));
                    }
                }
            }

            points.Shuffle(this.rng);

            if (count > points.Count) {
                count = points.Count;
            }

	    int pt = 0;
            for (int i = 0; pt < count && i < points.Count; i++) {
                int sz = this.MakeValley(points[i]);
		if (sz >= 5) {
		    pt++;
		}
            }

	    return this.NextStep();
        }

        public Task<OwResult> FlowMountainRivers(int count) {
            this.FlowRivers(this.mountain_elevation + (this.heightmax-this.mountain_elevation)*.5, this.heightmax, count, 256);
            return this.NextStep();
        }

        public Task<OwResult> FlowMountainRiversLostWoods(int count) {
            this.FlowRivers(mountain_elevation + (heightmax - mountain_elevation) / 18, this.heightmax, count, 350);
            return this.NextStep();
        }

        public Task<OwResult> FlowPlainsRivers(int count) {
            this.FlowRivers(this.sea_elevation + (this.mountain_elevation-this.sea_elevation)*.5, this.mountain_elevation, count, 256);
            return this.NextStep();
        }

        public void UpdateBiomeRegions() {
            var biome = OwRegion.FindRegions(this.Tilemap, overworldTiles.BiomeRegionTypeMap);
            this.Biome_regionmap = biome.Item1;
            this.Biome_regionlist = biome.Item2;
        }

        public void UpdateTraversableRegions() {
            var traversable = OwRegion.FindRegions(this.Tilemap, overworldTiles.TraversableRegionTypeMap);
            this.Traversable_regionmap = traversable.Item1;
            this.Traversable_regionlist = traversable.Item2;
        }

        public Task<OwResult> UpdateRegions() {
            this.UpdateBiomeRegions();
            this.UpdateTraversableRegions();
            return this.NextStep();
        }

	// Go through the region list and delete small islands, which
	// mostly just clutter up the map and don't look good.  Delete
	// all the "tiny" islands and keep a handful of "small"
	// islands.
        public Task<OwResult> RemoveSmallIslands() {
            this.OwnTilemap();

            const int tiny_island_size = 5;
            const int small_island_size = 12;
            const int keep_small_islands = 6;

            var tiny = new List<OwRegion>();
            var small = new List<OwRegion>();
            foreach (var r in this.Traversable_regionlist) {
                if (r.Adjacent.Count == 1 && this.Traversable_regionlist[r.Adjacent[0]].RegionType == OverworldTiles.OCEAN_REGION) {
                    if (r.Points.Count <= tiny_island_size) {
                        tiny.Add(r);
                    } else if (r.Points.Count <= small_island_size) {
                        small.Add(r);
                    }
                }
            }

            small.Shuffle(this.rng);
            var keep_small = Math.Min(keep_small_islands, small.Count);

            for (int i = keep_small; i < small.Count; i++) {
                var r = small[i];
                foreach (var p in r.Points) {
                    this.Tilemap[p.Y,p.X] = OverworldTiles.OCEAN;
                }
            }
            foreach (var r in tiny) {
                foreach (var p in r.Points) {
                    this.Tilemap[p.Y,p.X] = OverworldTiles.OCEAN;
                }
            }

            return this.UpdateRegions();
        }

	// Find tiny regions and merge them into one of the adjacent
	// regions.
	public Task<OwResult> RemoveTinyRegions(int tiny_region_size) {
            this.OwnTilemap();

            foreach (var r in this.Biome_regionlist) {
                if (r.Points.Count <= tiny_region_size) {
                    var mergeWith = r.Adjacent[rng.Between(0, r.Adjacent.Count-1)];
                    byte tile = this.overworldTiles.BiomeRegionTypes[this.Biome_regionlist[mergeWith].RegionType][0];
                    foreach (var p in r.Points) {
                        this.Tilemap[p.Y,p.X] = tile;
                    }
                }
            }
            return this.UpdateRegions();
        }

	// Fill in an area with a biome type.  This is a flood fill
	// that expands the frontier at random.  Gaps are fixed by
	// subsequent steps.
        public void Splat(SCCoords p, byte biome, int max) {
	    int sz = 0;
	    if (max > 200) {
		sz = this.rng.Between(max/2, max);
	    } else {
		sz = this.rng.Between(9, max);
	    }

            var pending = new List<SCCoords>();
            pending.Add(p);

            while (sz > 0 && pending.Count > 0) {
                p = pending.SpliceRandom(this.rng);
                if (this.Tilemap[p.Y,p.X] != OverworldTiles.LAND &&
                    this.Tilemap[p.Y,p.X] != OverworldTiles.RIVER) {
                    continue;
                }
                if (this.Tilemap[p.Y,p.X] == OverworldTiles.LAND) {
                    this.Tilemap[p.Y,p.X] = (byte)biome;
                }
                sz -= 1;

		var up = p.OwUp;
		var right = p.OwRight;
		var down = p.OwDown;
		var left = p.OwLeft;

                if (this.Tilemap[up.Y,up.X] == OverworldTiles.LAND ||
                    this.Tilemap[up.Y,up.X] == OverworldTiles.RIVER)
		{
		    pending.Add(up);
		}
                if (this.Tilemap[right.Y,right.X] == OverworldTiles.LAND ||
                    this.Tilemap[right.Y,right.X] == OverworldTiles.RIVER)
		{
		    pending.Add(right);
		}
                if (this.Tilemap[down.Y,down.X] == OverworldTiles.LAND ||
                    this.Tilemap[down.Y,down.X] == OverworldTiles.RIVER)
		{
		    pending.Add(down);
		}
                if (this.Tilemap[left.Y,left.X] == OverworldTiles.LAND ||
                    this.Tilemap[left.Y,left.X] == OverworldTiles.RIVER)
		{
		    pending.Add(left);
		}
            }
        }

        public Task<OwResult> AddBiomes(int max, bool extraForest) {
            this.OwnTilemap();

            byte[] biome_types;
	    if (extraForest) {
		biome_types = new byte[] {
		    OverworldTiles.FOREST, OverworldTiles.FOREST,
		    OverworldTiles.FOREST, OverworldTiles.FOREST,
		    OverworldTiles.FOREST, OverworldTiles.FOREST,
		    OverworldTiles.FOREST, OverworldTiles.MARSH,
		    OverworldTiles.MARSH, OverworldTiles.MARSH,
		    OverworldTiles.GRASS, OverworldTiles.DESERT };
	    } else {
		biome_types = new byte[] { OverworldTiles.FOREST, OverworldTiles.FOREST,
		    OverworldTiles.GRASS, OverworldTiles.MARSH, OverworldTiles.DESERT };
	    }
            foreach (var r in this.Biome_regionlist) {
                if (r.RegionType != OverworldTiles.LAND_REGION) {
                    continue;
                }
                for (int i = 0; i < (r.Points.Count/(max/4))+1; i++) {
                    var b = biome_types[rng.Between(0, biome_types.Length-1)];
                    var p = r.Points[rng.Between(0, r.Points.Count-1)];
		    int splatmax = max;
		    if (extraForest && b != OverworldTiles.FOREST) {
			splatmax = max/2;
		    }
		    if (r.Adjacent.Count == 1) {
			splatmax = r.Points.Count-6;
		    }
		    if (splatmax > 9) {
			this.Splat(p, b, splatmax);
		    }
                }
            }

            return this.UpdateRegions();
        }

	// Turn small patches of inland ocean into river tiles.
        public Task<OwResult> SmallSeasBecomeLakes() {
            this.OwnTilemap();
            foreach (var r in this.Biome_regionlist) {
                if (r.RegionType != OverworldTiles.OCEAN_REGION) {
                    continue;
                }
                if (r.Points.Count > 40) {
                    continue;
                }
                foreach (var p in r.Points) {
                    this.Tilemap[p.Y,p.X] = OverworldTiles.RIVER;
                }
            }
            return this.UpdateRegions();
        }

	public OwRegion StartingRegion {
	    get { return this.Traversable_regionlist[this.startingRegion]; }
	    set { this.startingRegion = value.RegionId; }
	}

	// Check if a feature fits entirely into given region without
	// overlapping any other regions.

	public bool CheckFit(short[,] regionMap, OwRegion region, SCCoords p, int w, int h, byte[,] weightmap, int useweight) {

	    if (p.X+w-1 >= OverworldState.MAPSIZE) {
		return false;
	    }
	    if (p.Y+h-1 >= OverworldState.MAPSIZE) {
		return false;
	    }

	    // Checks the weight map, this tells us if we're too close
	    // to another feature that's already been placed, prevents
	    // features from bunching up.
	    if (weightmap[p.Y+h/2, p.X+w/2] != useweight) {
		return false;
	    }
	    // Check the corners first before checking the entire box.
	    if (regionMap[p.Y+h-1, p.X+w-1] != region.RegionId) {
		return false;
	    }
	    if (regionMap[p.Y, p.X+w-1] != region.RegionId) {
		return false;
	    }
	    if (regionMap[p.Y+h-1, p.X] != region.RegionId) {
		return false;
	    }
	    for (int j = 0; j < h; j++) {
		for (int i = 0; i < w; i++) {
		    if (regionMap[p.Y+j, p.X+i] != region.RegionId) {
			return false;
		    }
		}
	    }
	    return true;
	}

	// Render a feature onto the tilemap, and updates regions by
	// cutting out the feature.
	public void RenderFeature(SCCoords point, byte[,] feature) {
	    this.OwnTilemap();
	    for (int j = 0; j < feature.GetLength(0); j++) {
		for (int i = 0; i < feature.GetLength(1); i++) {
		    if (feature[j,i] != OverworldTiles.None) {
			this.Tilemap[point.Y+j, point.X+i] = feature[j,i];
		    }
		}
	    }
	    this.OwnRegions();
	    this.OwnPlacements();
	    var orig = this.Traversable_regionmap[point.Y, point.X];
	    var cutout = OwRegion.Cutout(this.Traversable_regionmap, this.Traversable_regionlist, point, feature.GetLength(1), feature.GetLength(0));

	    if (this.Exclude_airship.Contains(orig)) {
		this.Exclude_airship.Add(cutout);
	    }

	    OwRegion.Cutout(this.Biome_regionmap, this.Biome_regionlist, point, feature.GetLength(1), feature.GetLength(0));
	}

	// Place a feature somewhere in the region at random.
	public ValueTuple<bool,SCCoords> PlaceFeature(short[,] regionMap, OwRegion region, PgFeature feature, int maxweight=0) {
	    var h = feature.Tiles.GetLength(0);
	    var w = feature.Tiles.GetLength(1);

	    var points = new List<SCCoords>(region.Points);
	    points.Shuffle(this.rng);
	    SCCoords point = new SCCoords(0, 0);
	    bool found = false;
	    if (feature.MountainCave) {
		maxweight = 16;
	    }
	    for (int tryweight = 0; !found && tryweight <= maxweight; tryweight++) {
		foreach (var p in points) {
		    if (feature.MountainCave) {
			var n = p.OwUp;
			var nw = n.OwLeft;
			var ne = n.OwRight;

			var nn = n.OwUp;
			var nnw = nn.OwLeft;
			var nne = nn.OwRight;

			if (this.Feature_weightmap[n.Y, n.X] != tryweight) {
			    continue;
			}
			if (this.Tilemap[nw.Y, nw.X] == OverworldTiles.MOUNTAIN &&
			    this.Tilemap[n.Y, n.X] == OverworldTiles.MOUNTAIN &&
			    this.Tilemap[ne.Y, ne.X] == OverworldTiles.MOUNTAIN &&
			    this.Tilemap[nnw.Y, nnw.X] == OverworldTiles.MOUNTAIN &&
			    this.Tilemap[nn.Y, nn.X] == OverworldTiles.MOUNTAIN &&
			    this.Tilemap[nne.Y, nne.X] == OverworldTiles.MOUNTAIN)
			{
			    point = new SCCoords(n.X, n.Y);
			    found = true;
			    break;
			}
		    } else {
			if (this.CheckFit(regionMap, region, p, w, h, this.Feature_weightmap, tryweight)) {
			    point = p;
			    found = true;
			    break;
			}
		    }
		}
	    }
	    if (!found) {
		return ValueTuple.Create(found, point);
	    }

	    return this.PlaceFeatureAt(regionMap, region, point, feature, false);
	}

	// Place a feature at a specific position.  Calls
	// RenderFeature and updates the appropriate weight map.
	public ValueTuple<bool,SCCoords> PlaceFeatureAt(short[,] regionMap, OwRegion region, SCCoords point,
							PgFeature feature, bool checkfit=true,
							bool useDockWeight=false) {
	    var h = feature.Tiles.GetLength(0);
	    var w = feature.Tiles.GetLength(1);

	    // We have separate weight maps for docks and other
	    // features.  We don't want docks to be too close to one
	    // another, but it is okay if a dock is close to a regular
	    // feature.
	    byte[,] weightmap;
	    if (useDockWeight) {
		weightmap = this.Dock_weightmap;
	    } else {
		weightmap = this.Feature_weightmap;
	    }

	    if (checkfit && !this.CheckFit(regionMap, region, point, w, h, weightmap, 0)) {
		return ValueTuple.Create(false, point);
	    }

	    this.RenderFeature(point, feature.Tiles);

	    this.OwnPlacements();

	    if (useDockWeight) {
		weightmap = this.Dock_weightmap;
	    } else {
		weightmap = this.Feature_weightmap;
	    }

	    byte radius = 16;
	    int x = point.X;
	    int y = point.Y;
	    for (int y2 = Math.Max(point.Y-radius, 0); y2 < Math.Min(point.Y+radius, 255); y2++) {
		for (int x2 = Math.Max(point.X-radius, 0); x2 < Math.Min(point.X+radius, 255); x2++) {
		    byte dist = (byte)Math.Sqrt((x-x2)*(x-x2) + (y-y2)*(y-y2));
		    if (dist <= radius) {
			weightmap[y2, x2] = (byte)Math.Max(weightmap[y2, x2], radius - dist);
		    }
		}
	    }

	    foreach (var kv in feature.Entrances) {
		this.FeatureCoordinates[kv.Key] = new SCCoords(point.X+kv.Value.X, point.Y+kv.Value.Y);
		Console.WriteLine($"Placed {kv.Key} at {point.X+kv.Value.X}, {point.Y+kv.Value.Y}");
	    }

	    return ValueTuple.Create(true, point);
	}

	bool canTraverse(int curRegionType, SCCoords adj) {
	    int adjRegionType = -1;
	    var tile = this.Tilemap[adj.Y, adj.X];

	    if (tile == OverworldTiles.WATERFALL) {
		adjRegionType = OverworldTiles.RIVER_REGION;
	    } else if (OverworldTiles.Entrances.Contains(tile)) {
		adjRegionType = OverworldTiles.ENTRANCES_REGION;
	    } else if (overworldTiles.TraversableRegionTypeMap.ContainsKey(tile)) {
		adjRegionType = overworldTiles.TraversableRegionTypeMap[tile];
	    }

	    if (adjRegionType == -1 || adjRegionType == OverworldTiles.MOUNTAIN_REGION) {
		return false;
	    }

	    if (curRegionType == adjRegionType) {
		return true;
	    }

	    if ((curRegionType == OverworldTiles.LAND_REGION &&
		 adjRegionType == OverworldTiles.DOCK_REGION) ||
		(curRegionType == OverworldTiles.DOCK_REGION &&
		 adjRegionType == OverworldTiles.LAND_REGION))
	    {
		return true;
	    }

	    if ((curRegionType == OverworldTiles.LAND_REGION &&
		 adjRegionType == OverworldTiles.ENTRANCES_REGION) ||
		(curRegionType == OverworldTiles.ENTRANCES_REGION &&
		 adjRegionType == OverworldTiles.LAND_REGION))
	    {
		return true;
	    }

	    if ((curRegionType == OverworldTiles.LAND_REGION &&
		 adjRegionType == OverworldTiles.RIVER_REGION) ||
		(curRegionType == OverworldTiles.RIVER_REGION &&
		 adjRegionType == OverworldTiles.LAND_REGION))
	    {
		return true;
	    }

	    if ((curRegionType == OverworldTiles.OCEAN_REGION &&
		 adjRegionType == OverworldTiles.RIVER_REGION) ||
		(curRegionType == OverworldTiles.RIVER_REGION &&
		 adjRegionType == OverworldTiles.OCEAN_REGION))
	    {
		return true;
	    }

	    if ((curRegionType == OverworldTiles.OCEAN_REGION &&
		 adjRegionType == OverworldTiles.DOCK_REGION) ||
		(curRegionType == OverworldTiles.DOCK_REGION &&
		 adjRegionType == OverworldTiles.OCEAN_REGION))
	    {
		return true;
	    }

	    if ((curRegionType == OverworldTiles.BRIDGE_REGION &&
		 adjRegionType == OverworldTiles.LAND_REGION))
	    {
		return true;
	    }

	    return false;
	}

	public bool CheckSanity() {
	    bool[,] visited = new bool[OverworldState.MAPSIZE, OverworldState.MAPSIZE];
	    var workingStack = new Stack<SCCoords>();
	    var remainingEntrances = new List<byte>(OverworldTiles.Entrances);
	    remainingEntrances.Add(OverworldTiles.AIRSHIP_DESERT);

	    workingStack.Push(this.FeatureCoordinates["StartingLocation"]);

	    while (workingStack.Count > 0 && remainingEntrances.Count > 0) {
		while (workingStack.Count > 0 && remainingEntrances.Count > 0) {
		    var current = workingStack.Pop();
		    if (visited[current.Y, current.X]) {
			continue;
		    }
		    visited[current.Y, current.X] = true;

		    int regionType = -1;
		    var tile = this.Tilemap[current.Y, current.X];

		    if (remainingEntrances.Contains(tile)) {
			remainingEntrances.Remove(tile);
			regionType = OverworldTiles.ENTRANCES_REGION;

			if (tile == OverworldTiles.TITAN_CAVE_E) {
			    workingStack.Push(this.FeatureCoordinates["TitansTunnelWest"]);
			}
			if (tile == OverworldTiles.TITAN_CAVE_W) {
			    workingStack.Push(this.FeatureCoordinates["TitansTunnelEast"]);
			}

		    } else if (OverworldTiles.Entrances.Contains(tile)) {
			regionType = OverworldTiles.ENTRANCES_REGION;
		    } else if (overworldTiles.TraversableRegionTypeMap.ContainsKey(tile)) {
			regionType = overworldTiles.TraversableRegionTypeMap[tile];
		    }

		    if (regionType == -1 || regionType == OverworldTiles.MOUNTAIN_REGION) {
			continue;
		    }

		    if (canTraverse(regionType, current.OwUp)) {
			workingStack.Push(current.OwUp);
		    }
		    if (canTraverse(regionType, current.OwRight)) {
			workingStack.Push(current.OwRight);
		    }
		    if (canTraverse(regionType, current.OwDown)) {
			workingStack.Push(current.OwDown);
		    }
		    if (canTraverse(regionType, current.OwLeft)) {
			workingStack.Push(current.OwLeft);
		    }

		    if (regionType == OverworldTiles.LAND) {
			if (current.OwUp == this.FeatureCoordinates["Bridge"] && canTraverse(OverworldTiles.BRIDGE_REGION, current.OwUp.OwUp)) {
			    workingStack.Push(current.OwUp.OwUp);
			}
			if (current.OwRight == this.FeatureCoordinates["Bridge"] && canTraverse(OverworldTiles.BRIDGE_REGION, current.OwRight.OwRight)) {
			    workingStack.Push(current.OwRight.OwRight);
			}
			if (current.OwDown == this.FeatureCoordinates["Bridge"] && canTraverse(OverworldTiles.BRIDGE_REGION, current.OwDown.OwDown)) {
			    workingStack.Push(current.OwDown.OwDown);
			}
			if (current.OwLeft == this.FeatureCoordinates["Bridge"] && canTraverse(OverworldTiles.BRIDGE_REGION, current.OwLeft.OwLeft)) {
			    workingStack.Push(current.OwLeft.OwLeft);
			}
		    }
		}

		if (!remainingEntrances.Contains(OverworldTiles.AIRSHIP_DESERT)) {
		    for (int y = 0; y < OverworldState.MAPSIZE; y++) {
			for (int x = 0; x < OverworldState.MAPSIZE; x++) {
			    if (!visited[y, x] && overworldTiles.airship_landable.Contains(this.Tilemap[y, x])) {
				workingStack.Push(new SCCoords(x, y));
			    }
			}
		    }
		}
	    }

	    if (remainingEntrances.Count > 0) {
		foreach (var e in remainingEntrances) {
		    Console.WriteLine($"Procgen map failed sanity check: couldn't reach {e:X}");
		}
		return false;
	    }
	    return true;
	}
    }

    public class OwGenerationStep : GenerationStep<ProcgenState<OwResult, OwGenerationStep, ProgenFramework.GenerationTaskType<OwResult>>, OwResult> {
	public OwGenerationStep(string methodName, object[] parameters, string message="") : base("FF1Lib.Procgen.OverworldState", methodName, parameters, message) { }
    }

    public class OwResult : Result<ProcgenState<FF1Lib.Procgen.OwResult, OwGenerationStep, ProgenFramework.GenerationTaskType<OwResult>>,
	OwGenerationStep, ProgenFramework.GenerationTaskType<OwResult>> {

	public OwResult() : base() { }
	public OwResult(bool f) : base(f) {}
        public OwResult(ProcgenState<FF1Lib.Procgen.OwResult, OwGenerationStep, ProgenFramework.GenerationTaskType<OwResult>> f) : base(f) {}
	public OwResult(List<OwGenerationTask> tasks) : base(tasks) { }
    }

    public static class NewOverworld {

	public async static Task<OwMapExchangeData> GenerateNewOverworld(MT19337 rng, OwMapExchanges mode,
									 bool shuffledaccess, bool unsafestart,
									 FF1Rom.ReportProgress progress) {
	    var mt = new OverworldTiles();

	    int maxtries = 1;
	    int tries = maxtries;
	    while (tries > 0) {
		tries--;
		List<OwGenerationStep> worldGenSteps;

		if (mode == OwMapExchanges.LostWoods) {
		    worldGenSteps = new List<OwGenerationStep> {
			new OwGenerationStep("CreateLostWoodsMap", new object[]{}),
			new OwGenerationStep("ApplyFilter", new object[] {mt.expand_mountains, false}, "expand_mountains"),
			new OwGenerationStep("FlowMountainRiversLostWoods", new object[] {16}),
			new OwGenerationStep("ApplyFilter", new object[] {mt.connect_diagonals, false}, "connect_diagonals"),
			new OwGenerationStep("UpdateRegions", new object[]{}),
			new OwGenerationStep("AddBiomes", new object[]{1600, true}),

			new OwGenerationStep("ApplyFilter", new object[]{mt.remove_salients, true}, "remove_salients"),
			new OwGenerationStep("UpdateRegions", new object[]{}),
			new OwGenerationStep("RemoveTinyRegions", new object[]{25}),

			new OwGenerationStep("ApplyFilter", new object[]{mt.remove_salients, true}, "remove_salients"),
			new OwGenerationStep("UpdateRegions", new object[]{}),
			new OwGenerationStep("RemoveTinyRegions", new object[]{25}),
		    };
		} else {
		    worldGenSteps = new List<OwGenerationStep> {
			new OwGenerationStep("CreateInitialMap", new object[]{}),
			new OwGenerationStep("MakeValleys", new object[] {6}),
			new OwGenerationStep("ApplyFilter", new object[] {mt.expand_mountains, false}, "expand_mountains"),
			new OwGenerationStep("ApplyFilter", new object[] {mt.expand_oceans, false}, "expand_oceans"),
			new OwGenerationStep("FlowMountainRivers", new object[] {12}),
			new OwGenerationStep("FlowPlainsRivers", new object[] {12}),
			new OwGenerationStep("ApplyFilter", new object[] {mt.connect_diagonals, false}, "connect_diagonals"),
			new OwGenerationStep("UpdateRegions", new object[]{}),
			new OwGenerationStep("RemoveSmallIslands", new object[]{}),
			new OwGenerationStep("AddBiomes", new object[]{400, false}),

			new OwGenerationStep("ApplyFilter", new object[]{mt.remove_salients, true}, "remove_salients"),
			new OwGenerationStep("UpdateRegions", new object[]{}),
			new OwGenerationStep("RemoveTinyRegions", new object[]{5}),

			new OwGenerationStep("ApplyFilter", new object[]{mt.remove_salients, true}, "remove_salients"),
			new OwGenerationStep("UpdateRegions", new object[]{}),
			new OwGenerationStep("RemoveTinyRegions", new object[]{5}),

			new OwGenerationStep("SmallSeasBecomeLakes", new object[]{}),
		    };
		}

		var blankState = new OverworldState(rng, worldGenSteps, mt, progress);
		var worldState = await ProgenFramework.RunSteps<OverworldState, OwResult, OwGenerationStep>(blankState, 150, progress);

		if (worldState == null) {
		    continue;
		}

		var maxPlacementTries = 8;
		var placementTries = maxPlacementTries;

		OverworldState postPlacementState = null;
		while(placementTries > 0 && postPlacementState == null) {
		    placementTries--;

		    List<OwGenerationStep> placementSteps = new List<OwGenerationStep>();
		    if (shuffledaccess) {

			var earlyRewardLocations = new List<PgFeature> {
			    OverworldTiles.TEMPLE_OF_FIENDS,
			    OverworldTiles.SARDAS_CAVE_FEATURE,
			    OverworldTiles.DWARF_CAVE_FEATURE,
			    OverworldTiles.MATOYAS_CAVE_FEATURE,
			    OverworldTiles.CRESCENT_LAKE_CITY,
			    OverworldTiles.CONERIA_CASTLE,
			    OverworldTiles.CARDIA_2_FEATURE,
			    OverworldTiles.CARDIA_4_FEATURE,
			    OverworldTiles.CARDIA_6_FEATURE,
			};

			var otherFeatures = new List<PgFeature> {
			    OverworldTiles.GAIA_TOWN,
			    OverworldTiles.OASIS2,
			    OverworldTiles.ELFLAND_TOWN,
			    OverworldTiles.ELFLAND_CASTLE,
			    OverworldTiles.MELMOND_TOWN,
			    OverworldTiles.LEFEIN_CITY,
			    OverworldTiles.BAHAMUTS_CAVE_FEATURE,
			    OverworldTiles.ASTOS_CASTLE,
			};

			var dudFeatures = new List<PgFeature> {
			    OverworldTiles.CARDIA_1_FEATURE,
			    OverworldTiles.CARDIA_5_FEATURE,
			};

			var unsafeFeatures = new List<PgFeature> {
			    OverworldTiles.ORDEALS_CASTLE,
			    OverworldTiles.ICE_CAVE_FEATURE,
			    OverworldTiles.EARTH_CAVE_FEATURE,
			    OverworldTiles.DRY_VOLCANO,
			};

			var features = new List<PgFeature>();

			Action<string, int, int, object[]> AddPlacements = (string op, int min, int max, object[] addl) => {
			    int count = rng.Between(min, max);
			    for (int i = 0; i < count && features.Count > 0; i++) {
				var f = features.SpliceRandom(rng);
				var parm = new List<object>();
				parm.Add(f);
				if (addl != null) {
				    parm.AddRange(addl);
				}
				placementSteps.Add(new OwGenerationStep(op, parm.ToArray()));
			    }
			};

			bool rubyRequired = rng.Between(0, 3) > 0;
			bool canalRequired = rng.Between(0, 4) > 0;
			bool bridgeRequired = rng.Between(0, 1) > 0;

			Console.WriteLine($"rubyRequired {rubyRequired} canalRequired {canalRequired} bridgeRequired {bridgeRequired}");

			placementSteps.Add(new OwGenerationStep("PlaceInStartingArea", new object[]{OverworldTiles.CONERIA_CITY}));
			if (bridgeRequired) {
			    // bridge start from any region, but you can't land on the bridged region
			    placementSteps.Add(new OwGenerationStep("PlaceBridge", new object[]{false, true}));
			} else {
			    // bridge starts from starting area,
			    // possibly you can avoid it (but item
			    // placement may still make it part of
			    // progression)
			    placementSteps.Add(new OwGenerationStep("PlaceBridge", new object[]{true, false}));
			}
			placementSteps.Add(new OwGenerationStep("PlaceIsolated", new object[]{OverworldTiles.TITANS_TUNNEL_WEST, !rubyRequired}));

			features.AddRange(earlyRewardLocations);

			if (shuffledaccess && unsafestart) {
			    features.AddRange(unsafeFeatures);
			    unsafeFeatures.Clear();
			}

			AddPlacements("PlaceIsolated", 1, 2, new object[]{true});

			features.Add(OverworldTiles.TITANS_TUNNEL_EAST);

			AddPlacements("PlaceInStartingArea", 1, 2, null);

			features.AddRange(otherFeatures);
			features.AddRange(unsafeFeatures);

			AddPlacements("PlaceInBridgedRegion", 1, 2, null);
			AddPlacements("PlaceRequiringCanoe", 1, 2, null);
			if (features.Contains(OverworldTiles.TITANS_TUNNEL_EAST)) {
			    // If Titan's east hasn't been placed yet,
			    // make sure we don't place it the Titan's west region.
			    features.Remove(OverworldTiles.TITANS_TUNNEL_EAST);
			    AddPlacements("PlaceInTitanWestRegion", 1, 2, null);
			    features.Add(OverworldTiles.TITANS_TUNNEL_EAST);
			} else {
			    AddPlacements("PlaceInTitanWestRegion", 1, 2, null);
			}

			placementSteps.Add(new OwGenerationStep("PlaceCanal", new object[]{canalRequired}));
			AddPlacements("PlaceInCanalRegion", 1, 2, null);

			// Annoying when bridge, canal, or titan
			// tunnel leads to a Cardia dead end, so add
			// them to the mix only after
			// bridge/canal/titan placement.
			features.AddRange(dudFeatures);

			AddPlacements("PlaceInMountains", 2, 5, null);
			placementSteps.Add(new OwGenerationStep("PlaceWaterfall", new object[]{OverworldTiles.WATERFALL_FEATURE}));
			placementSteps.Add(new OwGenerationStep("PlaceInBiome", new object[]{OverworldTiles.MIRAGE_TOWER,
											   new int[]{OverworldTiles.DESERT_REGION},
											   false, true, true, false}));
			placementSteps.Add(new OwGenerationStep("PlaceInBiome", new object[]{OverworldTiles.AIRSHIP_FEATURE,
											   new int[]{OverworldTiles.DESERT_REGION},
											   false, true, false, false}));
			placementSteps.Add(new OwGenerationStep("PlaceOnCoast", new object[]{OverworldTiles.ONRAC_TOWN, true}));
			placementSteps.Add(new OwGenerationStep("PlaceOnCoast", new object[]{OverworldTiles.PRAVOKA_CITY, false}));
			placementSteps.Add(new OwGenerationStep("PlaceInBiome", new object[]{OverworldTiles.MARSH_CAVE_FEATURE,
											   new int[]{OverworldTiles.MARSH_REGION},
											   true, true, true, false}));
			AddPlacements("PlaceInBiome", 3, 5, new object[] { null, true, false, false, false });
			AddPlacements("PlaceInBiome", 2, 3, new object[] { null, false, true, true, true });
			AddPlacements("PlaceInBiome", features.Count, features.Count, new object[] { null, false, false, false, false });
		    } else if (mode == OwMapExchanges.LostWoods) {
			placementSteps.AddRange(new OwGenerationStep[] {
				new OwGenerationStep("PlaceInStartingArea", new object[]{OverworldTiles.CONERIA_CITY}),
				new OwGenerationStep("PlaceIsolated", new object[]{OverworldTiles.TITANS_TUNNEL_WEST, false}),
				new OwGenerationStep("PlaceInTitanWestRegion", new object[]{OverworldTiles.CONERIA_CASTLE}),
				new OwGenerationStep("PlaceInTitanWestRegion", new object[]{OverworldTiles.MELMOND_TOWN}),
				new OwGenerationStep("PlaceInBiome", new object[]{OverworldTiles.AIRSHIP_FEATURE,
										new int[]{OverworldTiles.DESERT_REGION},
										false, true, false, false}),
				new OwGenerationStep("PlaceInBiome", new object[]{OverworldTiles.MARSH_CAVE_FEATURE,
										new int[]{OverworldTiles.MARSH_REGION},
										false, true, false, false}),
				new OwGenerationStep("PlaceInBiome", new object[]{OverworldTiles.TEMPLE_OF_FIENDS, null,
										false, true, false, false}),
				new OwGenerationStep("PlaceInBiome", new object[]{OverworldTiles.LEFEIN_CITY, null,
										false, false, true, false}),
				new OwGenerationStep("PlaceInBiome", new object[]{OverworldTiles.PRAVOKA_CITY_MOAT,
										new int[]{OverworldTiles.MARSH_REGION},
										false, true, false, false}),
				new OwGenerationStep("PlaceInBiome", new object[]{OverworldTiles.MIRAGE_TOWER,
										new int[]{OverworldTiles.DESERT_REGION},
										false, false, true, false}),
				new OwGenerationStep("PlaceInBiome", new object[]{OverworldTiles.OASIS,
										new int[]{OverworldTiles.DESERT_REGION,
										    OverworldTiles.FOREST_REGION},
										false, false, true, false}),
				new OwGenerationStep("PlaceInBiome", new object[]{OverworldTiles.TITANS_TUNNEL_EAST, null,
										false, true, false, false}),
				new OwGenerationStep("PlaceInBiome", new object[]{OverworldTiles.MATOYAS_CAVE_FEATURE, null,
										false, false, true, false}),
				new OwGenerationStep("PlaceInBiome", new object[]{OverworldTiles.CRESCENT_LAKE_CITY,
										new int[]{OverworldTiles.FOREST_REGION},
										false, false, true, false}),
				new OwGenerationStep("PlaceInBiome", new object[]{OverworldTiles.VOLCANO, null,
										false, false, true, false}),
				new OwGenerationStep("PlaceInBiome", new object[]{OverworldTiles.ICE_CAVE_FEATURE, null,
										false, false, true, false}),
				new OwGenerationStep("PlaceInBiome", new object[]{OverworldTiles.ELFLAND_TOWN, null,
										false, true, false, false}),
				new OwGenerationStep("PlaceInBiome", new object[]{OverworldTiles.ELFLAND_CASTLE, null,
										false, false, true, false}),
				new OwGenerationStep("PlaceInBiome", new object[]{OverworldTiles.BAHAMUTS_CAVE_FEATURE, null,
										false, false, true, false}),
				new OwGenerationStep("PlaceInBiome", new object[]{OverworldTiles.CARDIA_1_FEATURE, null,
										false, false, true, false}),
				new OwGenerationStep("PlaceInBiome", new object[]{OverworldTiles.CARDIA_2_FEATURE, null,
										false, false, true, false}),
				new OwGenerationStep("PlaceInBiome", new object[]{OverworldTiles.CARDIA_4_FEATURE, null,
										false, false, true, false}),
				new OwGenerationStep("PlaceInBiome", new object[]{OverworldTiles.CARDIA_5_FEATURE, null,
										false, false, true, false}),
				new OwGenerationStep("PlaceInBiome", new object[]{OverworldTiles.CARDIA_6_FEATURE, null,
										false, false, true, false}),
				new OwGenerationStep("PlaceInBiome", new object[]{OverworldTiles.GAIA_TOWN, null,
										false, false, true, false}),
				new OwGenerationStep("PlaceInBiome", new object[]{OverworldTiles.ORDEALS_CASTLE, null,
										false, false, true, false}),
				new OwGenerationStep("PlaceInBiome", new object[]{OverworldTiles.E_CANAL_STRUCTURE, null,
										false, false, true, false}),
				new OwGenerationStep("PlaceInBiome", new object[]{OverworldTiles.DWARF_CAVE_FEATURE, null,
										false, false, true, false}),

				new OwGenerationStep("PlaceWaterfall", new object[]{OverworldTiles.WATERFALL_FEATURE}),

				new OwGenerationStep("PlaceInMountains", new object[]{OverworldTiles.SARDAS_CAVE_FEATURE}),
				new OwGenerationStep("PlaceInMountains", new object[]{OverworldTiles.EARTH_CAVE_FEATURE}),
				new OwGenerationStep("PlaceInMountains", new object[]{OverworldTiles.ONRAC_TOWN}),
				new OwGenerationStep("PlaceInMountains", new object[]{OverworldTiles.ASTOS_CASTLE}),
			    });
		    } else if (mode == OwMapExchanges.GenerateNewOverworld) {
			placementSteps.AddRange(new OwGenerationStep[] {
				new OwGenerationStep("BridgeAlternatives", new object[]{}),
				new OwGenerationStep("PlaceInStartingArea", new object[]{OverworldTiles.CONERIA_CITY_CASTLE}),
				new OwGenerationStep("PlaceInStartingArea", new object[]{OverworldTiles.TEMPLE_OF_FIENDS}),
				new OwGenerationStep("PlaceBridge", new object[]{true, false}),
				new OwGenerationStep("PlacePravoka", new object[]{}),
				new OwGenerationStep("PlaceIsolated", new object[]{OverworldTiles.GAIA_TOWN, true}),
				new OwGenerationStep("PlaceRequiringCanoe", new object[]{OverworldTiles.ORDEALS_CASTLE}),
				new OwGenerationStep("PlaceIsolated", new object[]{OverworldTiles.TITANS_TUNNEL_WEST, false}),
				new OwGenerationStep("PlaceInTitanWestRegion", new object[]{OverworldTiles.SARDAS_CAVE_FEATURE}),
				new OwGenerationStep("PlaceCanal", new object[]{false}),
				new OwGenerationStep("PlaceInCanalRegion", new object[]{OverworldTiles.EARTH_CAVE_FEATURE}),

				new OwGenerationStep("PlaceInBiome", new object[]{OverworldTiles.MIRAGE_TOWER,
										new int[]{OverworldTiles.DESERT_REGION},
										false, true, true, false}),
				new OwGenerationStep("PlaceInBiome", new object[]{OverworldTiles.AIRSHIP_FEATURE,
										new int[]{OverworldTiles.DESERT_REGION},
										false, true, false, false}),

				new OwGenerationStep("PlaceInBiome", new object[]{OverworldTiles.OASIS,
										new int[]{OverworldTiles.DESERT_REGION},
										false, true, true, false}),
				new OwGenerationStep("PlaceOnCoast", new object[]{OverworldTiles.ONRAC_TOWN, true}),
				new OwGenerationStep("PlaceInBiome", new object[]{OverworldTiles.TITANS_TUNNEL_EAST, null,
										true, false, false, false}),
				new OwGenerationStep("PlaceInBiome", new object[]{OverworldTiles.MATOYAS_CAVE_FEATURE, null,
										true, false, false, false}),
				new OwGenerationStep("PlaceInBiome", new object[]{OverworldTiles.DWARF_CAVE_FEATURE, null,
										true, false, false, false}),
				new OwGenerationStep("PlaceInBiome", new object[]{OverworldTiles.ELFLAND_TOWN_CASTLE,
										new int[]{OverworldTiles.LAND_REGION,
										    OverworldTiles.GRASS_REGION,
										    OverworldTiles.FOREST_REGION},
										true, false, false, false}),
				new OwGenerationStep("PlaceInBiome", new object[]{OverworldTiles.MARSH_CAVE_FEATURE,
										new int[]{OverworldTiles.MARSH_REGION},
										true, false, false, false}),
				new OwGenerationStep("PlaceInBiome", new object[]{OverworldTiles.ASTOS_CASTLE, null,
										true, false, false, false}),
				new OwGenerationStep("PlaceInBiome", new object[]{OverworldTiles.MELMOND_TOWN, null,
										true, false, false, false}),
				new OwGenerationStep("PlaceInBiome", new object[]{OverworldTiles.CRESCENT_LAKE_CITY,
										new int[]{OverworldTiles.LAND_REGION,
										    OverworldTiles.GRASS_REGION,
										    OverworldTiles.FOREST_REGION,
										    OverworldTiles.MARSH_REGION},
										true, false, false, false}),

				new OwGenerationStep("PlaceInBiome", new object[]{OverworldTiles.LEFEIN_CITY, null,
										false, true, true, false}),
				new OwGenerationStep("PlaceInBiome", new object[]{OverworldTiles.BAHAMUTS_CAVE_FEATURE, null,
										false, true, true, true}),
				new OwGenerationStep("PlaceInBiome", new object[]{OverworldTiles.CARDIA_1_FEATURE, null,
										false, true, true, true}),
				new OwGenerationStep("PlaceInBiome", new object[]{OverworldTiles.CARDIA_2_FEATURE, null,
										false, true, true, true}),
				new OwGenerationStep("PlaceInBiome", new object[]{OverworldTiles.CARDIA_4_FEATURE, null,
										false, true, true, true}),
				new OwGenerationStep("PlaceInBiome", new object[]{OverworldTiles.CARDIA_5_FEATURE, null,
										false, true, true, true}),
				new OwGenerationStep("PlaceInBiome", new object[]{OverworldTiles.CARDIA_6_FEATURE, null,
										false, true, true, true}),

				new OwGenerationStep("PlaceWaterfall", new object[]{OverworldTiles.WATERFALL_FEATURE}),

				new OwGenerationStep("PlaceInMountains", new object[]{OverworldTiles.ICE_CAVE_FEATURE}),
				new OwGenerationStep("PlaceInMountains", new object[]{OverworldTiles.VOLCANO}),
			    });
		    } else {
			throw new Exception($"Unknown mode {mode}");
		    }

		    var prePlacementState = new OverworldState(worldState);
		    prePlacementState.SetSteps(placementSteps);
		    postPlacementState = await ProgenFramework.RunSteps<OverworldState, OwResult, OwGenerationStep>(prePlacementState, 150, progress);
		}

		if (postPlacementState == null) {
		    continue;
		}

		var polishSteps = new List<OwGenerationStep> {
		    new OwGenerationStep("PreventAirshipLanding", new object[]{}),
		    new OwGenerationStep("ApplyFilter", new object[]{mt.polish_mountains1, true}, "polish_mountains1"),
		    new OwGenerationStep("ApplyFilter", new object[]{mt.polish_mountains2, true}, "polish_mountains2"),

		    new OwGenerationStep("ApplyFilter", new object[]{mt.apply_shores1, false}, "apply_shores1"),
		    new OwGenerationStep("ApplyFilter", new object[]{mt.apply_shores2, false}, "apply_shores2"),
		    new OwGenerationStep("ApplyFilter", new object[]{mt.apply_shores3, false}, "apply_shores3"),
		    new OwGenerationStep("ApplyFilter", new object[]{mt.apply_shores4, false}, "apply_shores4"),
		    new OwGenerationStep("ApplyFilter", new object[]{mt.apply_shores5, false}, "apply_shores5"),
		    new OwGenerationStep("ApplyFilter", new object[]{mt.apply_shores6, false}, "apply_shores6"),

		    new OwGenerationStep("ApplyFilter", new object[]{mt.prune_forests, true}, "prune_forests"),
		    new OwGenerationStep("PreventAirshipLanding", new object[]{}),

		    new OwGenerationStep("ApplyFilter", new object[]{mt.mountain_borders, false}, "mountain_borders"),
		    new OwGenerationStep("ApplyFilter", new object[]{mt.river_borders, false}, "river_borders"),
		    new OwGenerationStep("ApplyFilter", new object[]{mt.desert_borders, false}, "desert_borders"),
		    new OwGenerationStep("ApplyFilter", new object[]{mt.marsh_borders, false}, "marsh_borders"),
		    new OwGenerationStep("ApplyFilter", new object[]{mt.grass_borders, false}, "grass_borders"),
		    new OwGenerationStep("ApplyFilter", new object[]{mt.forest_borders, false}, "forest_borders"),
		    new OwGenerationStep("CheckBridgeShores", new object[]{}),
		    new OwGenerationStep("ApplyFilter", new object[]{mt.apply_shores7, false}, "apply_shores7"),
		    new OwGenerationStep("PreventAirshipLanding", new object[]{}),
		};

		postPlacementState.SetSteps(polishSteps);
		var finalState = await ProgenFramework.RunSteps<OverworldState, OwResult, OwGenerationStep>(postPlacementState, 150, progress);

		if (finalState != null && finalState.CheckSanity()) {
		    return ReplacementMap(finalState, mt);
		}
	    }
	    throw new FailedToGenerate($"Couldn't generate a map, try a different map generation seed");
	}

	public static OwMapExchangeData ReplacementMap(OverworldState st, OverworldTiles mt) {
	    var ExchangeData = new OwMapExchangeData();

	    ExchangeData.StartingLocation = st.FeatureCoordinates["StartingLocation"];
	    st.FeatureCoordinates.Remove("StartingLocation");

	    ExchangeData.AirShipLocation = st.FeatureCoordinates["Airship"];
	    st.FeatureCoordinates.Remove("Airship");

	    if (st.Traversable_regionlist[st.Traversable_regionmap[st.FeatureCoordinates["Bridge"].Y, st.FeatureCoordinates["Bridge"].X+1]].RegionType == OverworldTiles.LAND_REGION) {
		ExchangeData.HorizontalBridge = true;
	    } else {
		ExchangeData.HorizontalBridge = false;
	    }
	    ExchangeData.BridgeLocation = st.FeatureCoordinates["Bridge"];
	    st.FeatureCoordinates.Remove("Bridge");

	    ExchangeData.CanalLocation = st.FeatureCoordinates["Canal"];
	    st.FeatureCoordinates.Remove("Canal");

	    st.FeatureCoordinates.Remove("Ship");
	    ExchangeData.ShipLocations = AssignShipLocations(st);

	    ExchangeData.TeleporterFixups = new TeleportFixup[] {
		new TeleportFixup(FF1Lib.TeleportType.Exit, 0,
				  new TeleData((MapIndex)OverworldTiles.None,
					       st.FeatureCoordinates["TitansTunnelEast"].X,
					       st.FeatureCoordinates["TitansTunnelEast"].Y)),
		new TeleportFixup(FF1Lib.TeleportType.Exit, 1,
				  new TeleData((MapIndex)OverworldTiles.None,
					       st.FeatureCoordinates["TitansTunnelWest"].X,
					       st.FeatureCoordinates["TitansTunnelWest"].Y)),
		new TeleportFixup(FF1Lib.TeleportType.Exit, 2,
				  new TeleData((MapIndex)OverworldTiles.None,
					       st.FeatureCoordinates["IceCave1"].X,
					       st.FeatureCoordinates["IceCave1"].Y)),
		new TeleportFixup(FF1Lib.TeleportType.Exit, 3,
				  new TeleData((MapIndex)OverworldTiles.None,
					       st.FeatureCoordinates["CastleOrdeals1"].X,
					       st.FeatureCoordinates["CastleOrdeals1"].Y)),
		new TeleportFixup(FF1Lib.TeleportType.Exit, 4,
				  new TeleData((MapIndex)OverworldTiles.None,
					       st.FeatureCoordinates["ConeriaCastle1"].X,
					       st.FeatureCoordinates["ConeriaCastle1"].Y)),
		new TeleportFixup(FF1Lib.TeleportType.Exit, 5,
				  new TeleData((MapIndex)OverworldTiles.None,
					       st.FeatureCoordinates["EarthCave1"].X,
					       st.FeatureCoordinates["EarthCave1"].Y)),
		new TeleportFixup(FF1Lib.TeleportType.Exit, 6,
				  new TeleData((MapIndex)OverworldTiles.None,
					       st.FeatureCoordinates["GurguVolcano1"].X,
					       st.FeatureCoordinates["GurguVolcano1"].Y)),
		new TeleportFixup(FF1Lib.TeleportType.Exit, 7,
				  new TeleData((MapIndex)OverworldTiles.None,
					       st.FeatureCoordinates["Onrac"].X,
					       st.FeatureCoordinates["Onrac"].Y)),
		new TeleportFixup(FF1Lib.TeleportType.Exit, 8,
				  new TeleData((MapIndex)OverworldTiles.None,
					       st.FeatureCoordinates["MirageTower1"].X,
					       st.FeatureCoordinates["MirageTower1"].Y))
	    };
	    ExchangeData.OverworldCoordinates = st.FeatureCoordinates;

	    ExchangeData.DomainUpdates = AssignEncounterDomains(st, mt);
	    ExchangeData.DomainFixups = new DomainFixup[] {};

	    var tiles = new List<string>();
	    var onerow = new byte[OverworldState.MAPSIZE];
	    List<List<byte>> decompressedRows = new List<List<byte>>();
	    for (int y = 0; y < OverworldState.MAPSIZE; y++) {
		decompressedRows.Add(new List<byte>(256));
		for (int x = 0; x < OverworldState.MAPSIZE; x++) {
		    onerow[x] = st.Tilemap[y,x];
		    decompressedRows[y].Add(st.Tilemap[y,x]);
		}
		tiles.Add(Convert.ToBase64String(onerow));
	    }
	    var compressedMap = OverworldMap.CompressMapRows(decompressedRows);
	    var compressedSize = 0;
	    for (int y = 0; y < OverworldState.MAPSIZE; y++) {
		compressedSize += compressedMap[y].Count;
	    }

	    if (compressedSize > OverworldMap.MaximumMapDataSize) {
		throw new FailedToGenerate($"Generated map is too large to fit in the ROM by {compressedSize - OverworldMap.MaximumMapDataSize} bytes, try a different seed.");
	    }

	    ExchangeData.DecompressedMapRows = tiles;

	    return ExchangeData;
	}

	public static DomainFixup[] AssignEncounterDomains(OverworldState state, OverworldTiles mt) {
	    var nearest_dungeon = new string[OverworldState.MAPSIZE,OverworldState.MAPSIZE];
	    var working_list = new Queue<KeyValuePair<string, SCCoords>>();

	    foreach (KeyValuePair<string, SCCoords> kv in state.FeatureCoordinates) {
		working_list.Enqueue(kv);
	    }

	    while (working_list.Count > 0) {
		var kv = working_list.Dequeue();

		if (nearest_dungeon[kv.Value.Y, kv.Value.X] != null) {
		    continue;
		}

		nearest_dungeon[kv.Value.Y, kv.Value.X] = kv.Key;

		var adjacent = new SCCoords[] { kv.Value.OwUp, kv.Value.OwRight, kv.Value.OwDown, kv.Value.OwLeft};
		foreach (var adj in adjacent) {
		    if (mt.TraversableRegionTypeMap.ContainsKey(state.Tilemap[adj.Y, adj.X])) {
			var next_tile = mt.TraversableRegionTypeMap[state.Tilemap[adj.Y, adj.X]];
			if (next_tile == OverworldTiles.LAND_REGION || next_tile == OverworldTiles.RIVER_REGION) {
			    working_list.Enqueue(new KeyValuePair<string, SCCoords>(kv.Key, adj));
			}
		    }
		}
	    }

	    // Using octal numbers here because they nicely correspond
	    // to coordinates on the 8x8 source grid of encounter
	    // zones.
	    var source_encounter_domains = new Dictionary<string, (int, double)>{
		{"Coneria",         (Convert.ToInt32("44", 8), 3)},
		{"ConeriaCastle1",  (Convert.ToInt32("44", 8), 3)},
		{"TempleOfFiends1", (Convert.ToInt32("34", 8), 1)},
		{"Pravoka",         (Convert.ToInt32("46", 8), 1)},
		{"Gaia",            (Convert.ToInt32("06", 8), .75)},
		{"CastleOrdeals1",  (Convert.ToInt32("14", 8), .75)},
		{"TitansTunnelWest", (Convert.ToInt32("50", 8), 1)},
		{"SardasCave", (Convert.ToInt32("50", 8), 1)},
		{"MirageTower1", (Convert.ToInt32("16", 8), .75)},
		{"Onrac", (Convert.ToInt32("11", 8), .75)},
		{"EarthCave1", (Convert.ToInt32("52", 8), 1)},
		{"TitansTunnelEast", (Convert.ToInt32("50", 8), 1)},
		{"MatoyasCave", (Convert.ToInt32("35", 8), 1)},
		{"DwarfCave", (Convert.ToInt32("43", 8), 1)},
		{"Elfland", (Convert.ToInt32("64", 8), 1)},
		{"ElflandCastle", (Convert.ToInt32("64", 8), 1)},
		{"MarshCave1", (Convert.ToInt32("73", 8), 1)},
		{"NorthwestCastle", (Convert.ToInt32("53", 8), 1)},
		{"Melmond", (Convert.ToInt32("52", 8), 1)},
		{"CrescentLake", (Convert.ToInt32("66", 8), 1)},
		{"Lefein", (Convert.ToInt32("37", 8), .75)},
		{"BahamutCave1", (Convert.ToInt32("13", 8), .5)},
		{"Cardia1", (Convert.ToInt32("12", 8), .5)},
		{"Cardia2", (Convert.ToInt32("12", 8), .5)},
		{"Cardia4", (Convert.ToInt32("12", 8), .5)},
		{"Cardia5", (Convert.ToInt32("13", 8), .5)},
		{"Cardia6", (Convert.ToInt32("13", 8), .5)},
		{"Waterfall", (Convert.ToInt32("01", 8), 1)},
		{"GurguVolcano1", (Convert.ToInt32("66", 8), 1)},
		{"IceCave1", (Convert.ToInt32("66", 8), 1)},
	    };

	    var domains = new DomainFixup[64];

	    for (int j = 0; j < 8; j++) {
		for (int i = 0; i < 8; i++) {
		    var counts = new Dictionary<string, double>();
		    for (int y = 0; y < 32; y++) {
			for (int x = 0; x < 32; x++) {
			    var nd = nearest_dungeon[j*32+y, i*32+x];
			    if (nd != null) {
				double c = 0;
				counts.TryGetValue(nd, out c);
				counts[nd] = c + source_encounter_domains[nd].Item2;
			    }
			}

			double mx = 0;
			string pick = null;
			foreach (var kv in counts) {
			    if (kv.Value > mx) {
				pick = kv.Key;
				mx = kv.Value;
			    }
			}
			domains[j*8 + i] = new DomainFixup();
			domains[j*8 + i].To = (byte)(j*8 + i);
			if (pick != null) {
			    domains[j*8 + i].From = (byte)source_encounter_domains[pick].Item1;
			} else {
			    domains[j*8 + i].From = 0;
			}
		    }
		}
	    }

	    return domains;
	}

	public static ShipLocation[] AssignShipLocations(OverworldState state) {
	    var EntranceToOWTeleporterIndex = new Dictionary<string, OverworldTeleportIndex> {
		{"ConeriaCastle1", OverworldTeleportIndex.ConeriaCastle1},
		{"Coneria", OverworldTeleportIndex.Coneria},
		{"EarthCave1", OverworldTeleportIndex.EarthCave1},
		{"ElflandCastle", OverworldTeleportIndex.ElflandCastle},
		{"Elfland", OverworldTeleportIndex.Elfland},
		{"MirageTower1", OverworldTeleportIndex.MirageTower1},
		{"NorthwestCastle", OverworldTeleportIndex.NorthwestCastle},
		{"IceCave1", OverworldTeleportIndex.IceCave1},
		{"DwarfCave", OverworldTeleportIndex.DwarfCave},
		{"MatoyasCave", OverworldTeleportIndex.MatoyasCave},
		{"TitansTunnelEast", OverworldTeleportIndex.TitansTunnelEast},
		{"TitansTunnelWest", OverworldTeleportIndex.TitansTunnelWest},
		{"CastleOrdeals1", OverworldTeleportIndex.CastleOrdeals1},
		{"SardasCave", OverworldTeleportIndex.SardasCave},
		{"Waterfall", OverworldTeleportIndex.Waterfall},
		{"Pravoka", OverworldTeleportIndex.Pravoka},
		{"CrescentLake", OverworldTeleportIndex.CrescentLake},
		{"TempleOfFiends1", OverworldTeleportIndex.TempleOfFiends1},
		{"Gaia", OverworldTeleportIndex.Gaia},
		{"Onrac", OverworldTeleportIndex.Onrac},
		{"GurguVolcano1", OverworldTeleportIndex.GurguVolcano1},
		{"Cardia2", OverworldTeleportIndex.Cardia2},
		{"Cardia4", OverworldTeleportIndex.Cardia4},
		{"Cardia5", OverworldTeleportIndex.Cardia5},
		{"Cardia6", OverworldTeleportIndex.Cardia6},
		{"Cardia1", OverworldTeleportIndex.Cardia1},
		{"BahamutCave1", OverworldTeleportIndex.BahamutCave1},
		{"Lefein", OverworldTeleportIndex.Lefein},
		{"MarshCave1", OverworldTeleportIndex.MarshCave1},
		{"Melmond", OverworldTeleportIndex.Melmond},
	    };

	    var locations = new List<ShipLocation>();

	    // Figure out which region each entrance is contained in.
	    // Take advantage of the fact that entrances are part of
	    // features which create cutout regions, which are always
	    // adjacent to the traversable region they were cut out
	    // from.
	    var entranceRegions = new Dictionary<string, short>();
	    foreach (var c in state.FeatureCoordinates) {
		var featureRegion = state.Traversable_regionmap[c.Value.Y, c.Value.X];

		entranceRegions[c.Key] = -1;

		if (state.Traversable_regionlist[featureRegion].RegionType == OverworldTiles.LAND_REGION) {
		    entranceRegions[c.Key] = featureRegion;
		} else {
		    foreach (var adj in state.Traversable_regionlist[featureRegion].Adjacent) {
			if (state.Traversable_regionlist[adj].RegionType == OverworldTiles.LAND_REGION) {
			    entranceRegions[c.Key] = adj;
			    break;
			}
		    }
		}
	    }

	    SCCoords? coneriaDock = null;
	    SCCoords? pravokaDock = null;
	    foreach (var c in state.FeatureCoordinates) {
		// For each feature, go through all the docks and find
		// the ones that are in the same region.  Pick the one
		// that is closest.
		float dist = 1000000;
		var closestDock = new SCCoords(0, 0);
		var entranceRegion = entranceRegions[c.Key];
		if (entranceRegion > -1) {
		    foreach (var dock in state.DockPlacements) {
			if (dock.Item1 != entranceRegion) {
			    continue;
			}
			var featurePosition = c.Value;
			var dockPosition = dock.Item2;
			var d2 = (float)Math.Sqrt((featurePosition.X-dockPosition.X)*(featurePosition.X-dockPosition.X) +
						  (featurePosition.Y-dockPosition.Y)*(featurePosition.Y-dockPosition.Y));
			if (d2 < dist) {
			    dist = d2;
			    closestDock = dockPosition;
			}
		    }
		}
		if (dist < 1000000) {
		    //state.Tilemap[closestDock.Y,closestDock.X] = OverworldTiles.CONERIA_CASTLE_TOP_W;

		    locations.Add(new ShipLocation(closestDock.X,
						   closestDock.Y,
						   (byte)EntranceToOWTeleporterIndex[c.Key]));
		    if (c.Key == "Coneria") {
			coneriaDock = closestDock;
		    }
		    if (c.Key == "Pravoka") {
			pravokaDock = closestDock;
		    }
		}
	    }

	    if (coneriaDock != null) {
		locations.Add(new ShipLocation(coneriaDock.Value.X,
					       coneriaDock.Value.Y,
					       255));
	    } else if (pravokaDock != null) {
		locations.Add(new ShipLocation(pravokaDock.Value.X,
					       pravokaDock.Value.Y,
					       255));
	    } else {
		throw new Exception("Couldn't choose default ship location");
	    }

	    return locations.ToArray();
	}

    }
}
