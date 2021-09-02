using System.Collections.Generic;
using System;
using System.Reflection;
using RomUtilities;
using FF1Lib.Sanity;
using System.Diagnostics;

namespace FF1Lib.Procgen
{

    public static class RNGExtensions {
       public static double Uniform(this MT19337 rng, double low, double high)
        {
                double range = (high - low);
                return low + range * ((double)rng.Next() / uint.MaxValue);
        }
    }

    public class OwRegion {
        public int RegionType;
        public short RegionId;
        public List<SCCoords> Points;
        public List<int> Adjacent;

        public OwRegion(int regionType, short regionid) {
            this.RegionType = regionType;
            this.RegionId = regionid;
            this.Points = new List<SCCoords>();
            this.Adjacent = new List<int>();
        }

        public void AddPoint(SCCoords p) {
            this.Points.Add(p);
        }

        public static Tuple<short[,], List<OwRegion>> FindRegions(byte[,] tilemap, Dictionary<byte, int> tileRegionTypeMap) {
            var regionMap = new short[OverworldState.MAPSIZE,OverworldState.MAPSIZE];
            var regionList = new List<OwRegion>();

            for (int j = 0; j < OverworldState.MAPSIZE; j++) {
                for (int i = 0; i < OverworldState.MAPSIZE; i++) {
                    regionMap[j,i] = -1;
                }
            }

            regionList.Add(new OwRegion(tileRegionTypeMap[OverworldTiles.OCEAN], 0));

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

	public static void Cutout(short[,] regionMap, List<OwRegion> regionList,
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
	    cutout.Adjacent.Add(replacement.RegionId);
	}
    }

    public class OwFeature {
        public byte[,] Tiles;
        public Dictionary<string, SCCoords> Entrances;
	public bool MountainCave;

        public OwFeature(byte[,] tiles,
            Dictionary<string, SCCoords> entrances) {
                this.Tiles = tiles;
                this.Entrances = entrances;
		this.MountainCave = false;
            }

        public OwFeature(byte tile, string entrance, bool mountainCave) {
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

    }

    public partial class OverworldState {
        public const int MAPSIZE = 256;
        double[,] Basemap;
        public byte[,] Tilemap;
        short[,] Biome_regionmap;
        List<OwRegion> Biome_regionlist;
        short[,] Traversable_regionmap;
        List<OwRegion> Traversable_regionlist;
        byte[,] Feature_weightmap;
        byte[,] Dock_weightmap;
        public Dictionary<string, SCCoords> FeatureCoordinates;
        List<int> Reachable_regions;
        List<int> Exclude_docks;
        private bool ownBasemap;
        private bool ownTilemap;
        private bool ownRegions;
        private bool ownPlacements;
        Queue<GenerationStep> StepQueue;
        double heightmax;
        double mountain_elevation;
        double sea_elevation;

        private MT19337 rng;

        private OverworldTiles overworldTiles;

	short startingRegion;
	short bridgedRegion;
	bool shouldPlaceBridge;

        public OverworldState(MT19337 rng, GenerationStep[] steps, OverworldTiles overworldTiles) {
            this.rng = rng;
            this.ownBasemap = true;
            this.ownTilemap = true;
            this.ownRegions = true;
            this.ownPlacements = true;
            this.Basemap = new double[MAPSIZE,MAPSIZE];
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
            this.StepQueue = new Queue<GenerationStep>(steps);
            this.overworldTiles = overworldTiles;
	    this.startingRegion = -1;
	    this.bridgedRegion = -1;
	    this.shouldPlaceBridge = false;
        }

        public OverworldState(OverworldState copy) {
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
            this.rng = copy.rng;
            this.StepQueue = copy.StepQueue;
            this.heightmax = copy.heightmax;
            this.mountain_elevation = copy.mountain_elevation;
            this.sea_elevation = copy.sea_elevation;
            this.overworldTiles = copy.overworldTiles;
	    this.startingRegion = copy.startingRegion;
	    this.bridgedRegion = copy.bridgedRegion;
	    this.shouldPlaceBridge = false;
        }

        void OwnBasemap() {
            if (this.ownBasemap) {
                return;
            }
            var fromBasemap = this.Basemap;
            this.Basemap = new double[MAPSIZE,MAPSIZE];
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
            this.ownPlacements = true;
        }
        public Result NextStep() {
            if (this.StepQueue.Count == 0) {
                return new Result(this);
            }
            this.StepQueue = new Queue<GenerationStep>(this.StepQueue);
            var nextStep = this.StepQueue.Dequeue();
	    Console.WriteLine(nextStep.method.Name);
            return nextStep.RunStep(this);
        }

        const double UNSET = -1000000;
        const double perturb_reduction = .63;

        void PerturbPoints(int x0, int y0, int x1, int y1, double r0) {
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

            double midp;
            // Middle
            if (this.Basemap[y2,x2] == UNSET) {
                midp = (this.Basemap[y0,x0]+this.Basemap[y1,x0]+this.Basemap[y0,x1]+this.Basemap[y1,x1])/4.0;
                this.Basemap[y2,x2] = midp + rng.Uniform(-r0, r0);
            }

            // Left middle
            if (this.Basemap[y2,x0] == UNSET) {
                midp = (this.Basemap[y0,x0]+this.Basemap[y1,x0])/2.0;
                this.Basemap[y2,x0] = midp + rng.Uniform(-r0, r0);
            }

            // Right middle
            if (this.Basemap[y2,x1] == UNSET) {
                midp = (this.Basemap[y0,x1]+this.Basemap[y1,x1])/2.0;
                this.Basemap[y2,x1] = midp + rng.Uniform(-r0, r0);
            }

            // top middle
            if (this.Basemap[y0,x2] == UNSET) {
                midp = (this.Basemap[y0,x0]+this.Basemap[y0,x1])/2.0;
                this.Basemap[y0,x2] = midp + rng.Uniform(-r0, r0);
            }

            // bottom middle
            if (this.Basemap[y1,x2] == UNSET) {
                midp = (this.Basemap[y1,x0]+this.Basemap[y1,x1])/2.0;
                this.Basemap[y1,x2] = midp + rng.Uniform(-r0, r0);
            }

            r0 *= perturb_reduction;

            this.PerturbPoints(x0, y0, x2, y2, r0);
            this.PerturbPoints(x2, y0, x1, y2, r0);
            this.PerturbPoints(x0, y2, x2, y1, r0);
            this.PerturbPoints(x2, y2, x1, y1, r0);
        }

        public Result CreateInitialMap() {
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
                    sea_elevation -= .002;
                }
                if (mountain_count < min_mtn_tiles) {
                    mountain_elevation -= .002;
                }
                if (sea_elevation <= 0) {
                    return new Result(false);
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

            return this.NextStep();
        }

        public Result ApplyFilter(OwTileFilter filter, bool repeat) {
            this.OwnTilemap();
            this.Tilemap = filter.ApplyFilter(this.Tilemap, repeat);
            return this.NextStep();
        }

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

               pending[-this.Basemap[p.Y,p.X+1]] = p.OwRight;
               pending[-this.Basemap[p.Y+1,p.X]] = p.OwDown;
               pending[-this.Basemap[p.Y,p.X-1]] = p.OwLeft;
               pending[-this.Basemap[p.Y-1,p.X]] = p.OwUp;
            }

	    return size;
        }

        public void FlowRivers(double lower_elev, double upper_elev, int count) {
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
                this.FlowRiver(points[i], 256);
            }
        }

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

        public Result MakeValleys(int count) {
	    double lower_elev = this.mountain_elevation + (this.heightmax-this.mountain_elevation)*.5;
	    double upper_elev = this.heightmax;

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

	    int pt = 0;
            for (int i = 0; pt < count && i < points.Count; i++) {
                int sz = this.MakeValley(points[i]);
		if (sz >= 5) {
		    pt++;
		}
            }

	    return this.NextStep();
        }

        public Result FlowMountainRivers(int count) {
            this.FlowRivers(this.mountain_elevation + (this.heightmax-this.mountain_elevation)*.5, this.heightmax, count);
            return this.NextStep();
        }

        public Result FlowPlainsRivers(int count) {
            this.FlowRivers(this.sea_elevation + (this.mountain_elevation-this.sea_elevation)*.5, this.mountain_elevation, count);
            return this.NextStep();
        }

        public void UpdateBiomeRegions() {
            var biome = OwRegion.FindRegions(this.Tilemap, overworldTiles.PreShoreRegionTypeMap);
            this.Biome_regionmap = biome.Item1;
            this.Biome_regionlist = biome.Item2;
        }

        public void UpdateTraversableRegions() {
            var traversable = OwRegion.FindRegions(this.Tilemap, overworldTiles.TraversableRegionTypeMap);
            this.Traversable_regionmap = traversable.Item1;
            this.Traversable_regionlist = traversable.Item2;
        }

        public Result UpdateRegions() {
            this.UpdateBiomeRegions();
            this.UpdateTraversableRegions();
            return this.NextStep();
        }

        public Result RemoveSmallIslands() {
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

       public Result RemoveTinyRegions() {
            this.OwnTilemap();

            const int tiny_region_size = 5;

            foreach (var r in this.Biome_regionlist) {
                if (r.Points.Count <= tiny_region_size) {
                    var mergeWith = r.Adjacent[rng.Between(0, r.Adjacent.Count-1)];
                    byte tile = this.overworldTiles.PreShoreRegionTypes[this.Biome_regionlist[mergeWith].RegionType][0];
                    foreach (var p in r.Points) {
                        this.Tilemap[p.Y,p.X] = tile;
                    }
                }
            }
            return this.UpdateRegions();
        }


        public void Splat(SCCoords p, byte biome, int max) {
	    int sz = 0;
	    if (max > 200) {
		sz = this.rng.Between(200, 400);
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

                pending.Add(p.OwUp);
                pending.Add(p.OwRight);
                pending.Add(p.OwDown);
                pending.Add(p.OwLeft);
            }
        }

        public Result AddBiomes() {
            this.OwnTilemap();

            var biome_types = new byte[] { OverworldTiles.FOREST, OverworldTiles.FOREST, OverworldTiles.GRASS, OverworldTiles.MARSH, OverworldTiles.DESERT };
            foreach (var r in this.Biome_regionlist) {
                if (r.RegionType != OverworldTiles.LAND_REGION) {
                    continue;
                }
                for (int i = 0; i < r.Points.Count/100+1; i++) {
                    var b = biome_types[rng.Between(0, biome_types.Length-1)];
                    var p = r.Points[rng.Between(0, r.Points.Count-1)];
		    int max = 400;
		    if (r.Adjacent.Count == 1) {
			max = r.Points.Count-6;
		    }
		    if (max > 9) {
			this.Splat(p, b, max);
		    }
                }
            }

            return this.UpdateRegions();
        }

        public Result SmallSeasBecomeLakes() {
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

	public bool CheckFit(short[,] regionMap, OwRegion region, SCCoords p, int w, int h, byte[,] weightmap) {

	    if (weightmap[p.Y+h/2, p.X+w/2] > 0) {
		return false;
	    }

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
	    OwRegion.Cutout(this.Traversable_regionmap, this.Traversable_regionlist, point, feature.GetLength(1), feature.GetLength(0));
	    OwRegion.Cutout(this.Biome_regionmap, this.Biome_regionlist, point, feature.GetLength(1), feature.GetLength(0));
	}

	public ValueTuple<bool,SCCoords> PlaceFeature(short[,] regionMap, OwRegion region, OwFeature feature) {
	    var h = feature.Tiles.GetLength(0);
	    var w = feature.Tiles.GetLength(1);

	    var points = new List<SCCoords>(region.Points);
	    points.Shuffle(this.rng);
	    SCCoords point = new SCCoords(0, 0);
	    bool found = false;
	    foreach (var p in points) {
		if (feature.MountainCave) {
		    if (this.Tilemap[p.Y-1, p.X-1] == OverworldTiles.MOUNTAIN &&
			this.Tilemap[p.Y-1, p.X] == OverworldTiles.MOUNTAIN &&
			this.Tilemap[p.Y-1, p.X+1] == OverworldTiles.MOUNTAIN)
		    {
			point = new SCCoords(p.X, p.Y-1);
			found = true;
		    }
		} else {
		    if (this.CheckFit(regionMap, region, p, w, h, this.Feature_weightmap)) {
			point = p;
			found = true;
			break;
		    }
		}
	    }
	    if (!found) {
		return ValueTuple.Create(found, point);
	    }

	    return this.PlaceFeatureAt(regionMap, region, point, feature, false);
	}

	public ValueTuple<bool,SCCoords> PlaceFeatureAt(short[,] regionMap, OwRegion region, SCCoords point,
							OwFeature feature, bool checkfit=true,
							bool useDockWeight=false) {
	    var h = feature.Tiles.GetLength(0);
	    var w = feature.Tiles.GetLength(1);

	    byte[,] weightmap;
	    if (useDockWeight) {
		weightmap = this.Dock_weightmap;
	    } else {
		weightmap = this.Feature_weightmap;
	    }

	    if (checkfit && !this.CheckFit(regionMap, region, point, w, h, weightmap)) {
		return ValueTuple.Create(false, point);
	    }

	    this.RenderFeature(point, feature.Tiles);

	    this.OwnPlacements();

	    if (useDockWeight) {
		weightmap = this.Dock_weightmap;
	    } else {
		weightmap = this.Feature_weightmap;
	    }

	    int radius = 16;
	    int x = point.X;
	    int y = point.Y;
	    for (int y2 = Math.Max(point.Y-radius, 0); y2 < Math.Min(point.Y+radius, 255); y2++) {
		for (int x2 = Math.Max(point.X-radius, 0); x2 < Math.Min(point.X+radius, 255); x2++) {
		    byte dist = (byte)Math.Sqrt((x-x2)*(x-x2) + (y-y2)*(y-y2));
		    if (dist <= radius) {
			weightmap[y2, x2] += dist;
		    }
		}
	    }

	    foreach (var kv in feature.Entrances) {
		this.FeatureCoordinates[kv.Key] = new SCCoords(point.X+kv.Value.X, point.Y+kv.Value.Y);
		Console.WriteLine($"Placed {kv.Key} at {point.X+kv.Value.X}, {point.Y+kv.Value.Y}");
	    }

	    return ValueTuple.Create(true, point);
	}
    }

    public class Result {
        public OverworldState final;
        public List<GenerationTask> additionalTasks;

         public Result(bool f) {
             this.final = null;
             this.additionalTasks = null;
         }

        public Result(OverworldState f) {
            this.final = f;
            this.additionalTasks = null;
        }

        public Result(List<GenerationTask> tasks) {
            this.final = null;
            this.additionalTasks = tasks;
        }
    }

    public delegate Result GenerationTask();

    public class GenerationStep {
        public MethodInfo method;
        object[] parameters;
        public GenerationStep(string methodName, object[] parameters) {
            Type magicType = Type.GetType("FF1Lib.Procgen.OverworldState");
            this.method = magicType.GetMethod(methodName);
            Debug.Assert(method != null);
            this.parameters = parameters;
        }

        public Result RunStep(OverworldState st) {
            return (Result)method.Invoke(st, this.parameters);
        }
    }

    public class ReplacementMap {

        public ReplacementMap(OverworldState st, OverworldTiles mt) {
            Tiles = new List<List<byte>>();
            for (int y = 0; y < OverworldState.MAPSIZE; y++) {
                Tiles.Add(new List<byte>());
                for (int x = 0; x < OverworldState.MAPSIZE; x++) {
                    Tiles[y].Add(st.Tilemap[y,x]);
                }
            }
            ExchangeData = new OwMapExchangeData();
            ExchangeData.StartingLocation = st.FeatureCoordinates["StartingLocation"];
	    st.FeatureCoordinates.Remove("StartingLocation");

            ExchangeData.AirShipLocation = st.FeatureCoordinates["Airship"];
	    st.FeatureCoordinates.Remove("Airship");

            ExchangeData.BridgeLocation = st.FeatureCoordinates["Bridge"];
	    st.FeatureCoordinates.Remove("Bridge");

            ExchangeData.CanalLocation = st.FeatureCoordinates["Canal"];
	    st.FeatureCoordinates.Remove("Canal");

            ExchangeData.ShipLocations = new ShipLocation[] {
                new ShipLocation(st.FeatureCoordinates["Ship"].X, st.FeatureCoordinates["Ship"].Y, 255)
            };
	    st.FeatureCoordinates.Remove("Ship");
            ExchangeData.TeleporterFixups = new TeleportFixup[] {
		new TeleportFixup(FF1Lib.TeleportType.Exit, 0,
				  new TeleData((MapId)OverworldTiles.None,
					       st.FeatureCoordinates["TitansTunnelEast"].X,
					       st.FeatureCoordinates["TitansTunnelEast"].Y)),
		new TeleportFixup(FF1Lib.TeleportType.Exit, 1,
				  new TeleData((MapId)OverworldTiles.None,
					       st.FeatureCoordinates["TitansTunnelWest"].X,
					       st.FeatureCoordinates["TitansTunnelWest"].Y)),
		new TeleportFixup(FF1Lib.TeleportType.Exit, 2,
				  new TeleData((MapId)OverworldTiles.None,
					       st.FeatureCoordinates["IceCave1"].X,
					       st.FeatureCoordinates["IceCave1"].Y)),
		new TeleportFixup(FF1Lib.TeleportType.Exit, 3,
				  new TeleData((MapId)OverworldTiles.None,
					       st.FeatureCoordinates["CastleOrdeals1"].X,
					       st.FeatureCoordinates["CastleOrdeals1"].Y)),
		new TeleportFixup(FF1Lib.TeleportType.Exit, 4,
				  new TeleData((MapId)OverworldTiles.None,
					       st.FeatureCoordinates["ConeriaCastle1"].X,
					       st.FeatureCoordinates["ConeriaCastle1"].Y)),
		new TeleportFixup(FF1Lib.TeleportType.Exit, 5,
				  new TeleData((MapId)OverworldTiles.None,
					       st.FeatureCoordinates["EarthCave1"].X,
					       st.FeatureCoordinates["EarthCave1"].Y)),
		new TeleportFixup(FF1Lib.TeleportType.Exit, 6,
				  new TeleData((MapId)OverworldTiles.None,
					       st.FeatureCoordinates["GurguVolcano1"].X,
					       st.FeatureCoordinates["GurguVolcano1"].Y)),
		new TeleportFixup(FF1Lib.TeleportType.Exit, 7,
				  new TeleData((MapId)OverworldTiles.None,
					       st.FeatureCoordinates["Onrac"].X,
					       st.FeatureCoordinates["Onrac"].Y)),
		new TeleportFixup(FF1Lib.TeleportType.Exit, 8,
				  new TeleData((MapId)OverworldTiles.None,
					       st.FeatureCoordinates["MirageTower1"].X,
					       st.FeatureCoordinates["MirageTower1"].Y))
            };
            ExchangeData.OverworldCoordinates = st.FeatureCoordinates;

	    this.AssignEncounterDomains(st, mt);
        }

        public List<List<byte>> Tiles;
        public OwMapExchangeData ExchangeData;

	public void AssignEncounterDomains(OverworldState state, OverworldTiles mt) {
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
	    var source_encounter_domains = new Dictionary<string, int>{
		{"Coneria",         Convert.ToInt32("44", 8)},
		{"ConeriaCastle1",  Convert.ToInt32("44", 8)},
		{"TempleOfFiends1", Convert.ToInt32("34", 8)},
		{"Pravoka",         Convert.ToInt32("46", 8)},
		{"Gaia",            Convert.ToInt32("06", 8)},
		{"CastleOrdeals1",  Convert.ToInt32("14", 8)},
		{"TitansTunnelWest", Convert.ToInt32("50", 8)},
		{"SardasCave", Convert.ToInt32("50", 8)},
		{"MirageTower1", Convert.ToInt32("16", 8)},
		{"Onrac", Convert.ToInt32("11", 8)},
		{"EarthCave1", Convert.ToInt32("52", 8)},
		{"TitansTunnelEast", Convert.ToInt32("50", 8)},
		{"MatoyasCave", Convert.ToInt32("35", 8)},
		{"DwarfCave", Convert.ToInt32("43", 8)},
		{"Elfland", Convert.ToInt32("64", 8)},
		{"ElflandCastle", Convert.ToInt32("64", 8)},
		{"MarshCave1", Convert.ToInt32("73", 8)},
		{"NorthwestCastle", Convert.ToInt32("53", 8)},
		{"Melmond", Convert.ToInt32("52", 8)},
		{"CrescentLake", Convert.ToInt32("66", 8)},
		{"Lefein", Convert.ToInt32("37", 8)},
		{"BahamutCave1", Convert.ToInt32("13", 8)},
		{"Cardia1", Convert.ToInt32("12", 8)},
		{"Cardia2", Convert.ToInt32("12", 8)},
		{"Cardia4", Convert.ToInt32("12", 8)},
		{"Cardia5", Convert.ToInt32("13", 8)},
		{"Cardia6", Convert.ToInt32("13", 8)},
		{"Waterfall", Convert.ToInt32("01", 8)},
		{"GurguVolcano1", Convert.ToInt32("66", 8)},
		{"IceCave1", Convert.ToInt32("66", 8)},
	    };

	    var domains = new DomainFixup[64];

	    for (int j = 0; j < 8; j++) {
		for (int i = 0; i < 8; i++) {
		    var counts = new Dictionary<string, int>();
		    for (int y = 0; y < 32; y++) {
			for (int x = 0; x < 32; x++) {
			    var nd = nearest_dungeon[j*32+y, i*32+x];
			    if (nd != null) {
				int c = 0;
				counts.TryGetValue(nd, out c);
				counts[nd] = c+1;
			    }
			}

			int mx = 0;
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
			    domains[j*8 + i].From = (byte)source_encounter_domains[pick];
			} else {
			    domains[j*8 + i].From = 0;
			}
		    }
		}
	    }

	    this.ExchangeData.DomainUpdates = domains;
	    this.ExchangeData.DomainFixups = new DomainFixup[] {};
	}
    }

    public static class NewOverworld {

	public static ReplacementMap GenerateNewOverworld(MT19337 rng) {
	    var mt = new OverworldTiles();
	    GenerationStep[] steps = new GenerationStep[] {
		new GenerationStep("CreateInitialMap", new object[]{}),
		new GenerationStep("MakeValleys", new object[] {6}),
		new GenerationStep("ApplyFilter", new object[] {mt.expand_mountains, false}),
		new GenerationStep("ApplyFilter", new object[] {mt.expand_oceans, false}),
		new GenerationStep("FlowMountainRivers", new object[] {12}),
		new GenerationStep("FlowPlainsRivers", new object[] {12}),
		new GenerationStep("ApplyFilter", new object[] {mt.connect_diagonals, false}),
		new GenerationStep("UpdateRegions", new object[]{}),
		new GenerationStep("RemoveSmallIslands", new object[]{}),
		new GenerationStep("AddBiomes", new object[]{}),

		new GenerationStep("ApplyFilter", new object[]{mt.remove_salients, true}),
		new GenerationStep("UpdateRegions", new object[]{}),
		new GenerationStep("RemoveTinyRegions", new object[]{}),

		new GenerationStep("ApplyFilter", new object[]{mt.remove_salients, true}),
		new GenerationStep("UpdateRegions", new object[]{}),
		new GenerationStep("RemoveTinyRegions", new object[]{}),

		new GenerationStep("SmallSeasBecomeLakes", new object[]{}),

		new GenerationStep("BridgeAlternatives", new object[]{}),
		new GenerationStep("PlaceInStartingArea", new object[]{OverworldTiles.CONERIA_CITY}),
		new GenerationStep("PlaceInStartingArea", new object[]{OverworldTiles.TEMPLE_OF_FIENDS}),
		new GenerationStep("PlaceBridge", new object[]{}),
		new GenerationStep("PlacePravoka", new object[]{}),
		new GenerationStep("PlaceIsolated", new object[]{OverworldTiles.GAIA_TOWN, true}),
		new GenerationStep("PlaceRequiringCanoe", new object[]{OverworldTiles.ORDEALS_CASTLE}),
		new GenerationStep("PlaceIsolated", new object[]{OverworldTiles.TITANS_TUNNEL_WEST, false}),
		new GenerationStep("PlaceInTitanWestRegion", new object[]{OverworldTiles.SARDAS_CAVE_FEATURE}),
		new GenerationStep("PlaceCanal", new object[]{}),
		new GenerationStep("PlaceInCanalRegion", new object[]{OverworldTiles.EARTH_CAVE_FEATURE}),

		new GenerationStep("PlaceInBiome", new object[]{OverworldTiles.MIRAGE_TOWER,
								new int[]{OverworldTiles.DESERT_REGION},
								false, true, true, false}),
		new GenerationStep("PlaceInBiome", new object[]{OverworldTiles.AIRSHIP_FEATURE,
								new int[]{OverworldTiles.DESERT_REGION},
								false, true, false, false}),

		new GenerationStep("PlaceInBiome", new object[]{OverworldTiles.OASIS,
								new int[]{OverworldTiles.DESERT_REGION},
								false, true, true, false}),
		new GenerationStep("PlaceOnCoast", new object[]{OverworldTiles.ONRAC_TOWN, true}),
		new GenerationStep("PlaceInBiome", new object[]{OverworldTiles.TITANS_TUNNEL_EAST, null,
								true, false, false, false}),
		new GenerationStep("PlaceInBiome", new object[]{OverworldTiles.MATOYAS_CAVE_FEATURE, null,
								true, false, false, false}),
		new GenerationStep("PlaceInBiome", new object[]{OverworldTiles.DWARF_CAVE_FEATURE, null,
								true, false, false, false}),
		new GenerationStep("PlaceInBiome", new object[]{OverworldTiles.ELFLAND_TOWN_CASTLE,
								new int[]{OverworldTiles.LAND_REGION,
									  OverworldTiles.GRASS_REGION,
									  OverworldTiles.FOREST_REGION},
								true, false, false, false}),
		new GenerationStep("PlaceInBiome", new object[]{OverworldTiles.MARSH_CAVE_FEATURE,
								new int[]{OverworldTiles.MARSH_REGION},
								true, false, false, false}),
		new GenerationStep("PlaceInBiome", new object[]{OverworldTiles.ASTOS_CASTLE, null,
								true, false, false, false}),
		new GenerationStep("PlaceInBiome", new object[]{OverworldTiles.MELMOND_TOWN, null,
								true, false, false, false}),
		new GenerationStep("PlaceInBiome", new object[]{OverworldTiles.CRESCENT_LAKE_CITY,
								new int[]{OverworldTiles.LAND_REGION,
									  OverworldTiles.GRASS_REGION,
									  OverworldTiles.FOREST_REGION,
									  OverworldTiles.MARSH_REGION},
								true, false, false, false}),

		new GenerationStep("PlaceInBiome", new object[]{OverworldTiles.LEFEIN_CITY, null,
								false, true, true, false}),
		new GenerationStep("PlaceInBiome", new object[]{OverworldTiles.BAHAMUTS_CAVE_FEATURE, null,
								false, true, true, true}),
		new GenerationStep("PlaceInBiome", new object[]{OverworldTiles.CARDIA_1_FEATURE, null,
								false, true, true, true}),
		new GenerationStep("PlaceInBiome", new object[]{OverworldTiles.CARDIA_2_FEATURE, null,
								false, true, true, true}),
		new GenerationStep("PlaceInBiome", new object[]{OverworldTiles.CARDIA_3_FEATURE, null,
								false, true, true, true}),
		new GenerationStep("PlaceInBiome", new object[]{OverworldTiles.CARDIA_4_FEATURE, null,
								false, true, true, true}),
		new GenerationStep("PlaceInBiome", new object[]{OverworldTiles.CARDIA_5_FEATURE, null,
								false, true, true, true}),

		new GenerationStep("PlaceWaterfall", new object[]{OverworldTiles.WATERFALL_FEATURE}),

		new GenerationStep("PlaceInMountains", new object[]{OverworldTiles.ICE_CAVE_FEATURE}),
		new GenerationStep("PlaceInMountains", new object[]{OverworldTiles.VOLCANO}),

		new GenerationStep("ApplyFilter", new object[]{mt.apply_shores1, false}),
		new GenerationStep("ApplyFilter", new object[]{mt.apply_shores2, false}),
		new GenerationStep("ApplyFilter", new object[]{mt.apply_shores3, false}),
		new GenerationStep("ApplyFilter", new object[]{mt.apply_shores4, false}),

		new GenerationStep("ApplyFilter", new object[]{mt.prune_forests, true}),
		new GenerationStep("ApplyFilter", new object[]{mt.polish_mountains, true}),

		new GenerationStep("ApplyFilter", new object[]{mt.mountain_borders, false}),
		new GenerationStep("ApplyFilter", new object[]{mt.river_borders, false}),
		new GenerationStep("ApplyFilter", new object[]{mt.desert_borders, false}),
		new GenerationStep("ApplyFilter", new object[]{mt.marsh_borders, false}),
		new GenerationStep("ApplyFilter", new object[]{mt.grass_borders, false}),
		new GenerationStep("ApplyFilter", new object[]{mt.forest_borders, false}),
	    };

	    int tries = 5;
	    while (tries > 0) {
		tries--;
		Stack<GenerationTask> workStack = new Stack<GenerationTask>();

		System.GC.Collect();

		workStack.Push(new OverworldState(rng, steps, mt).NextStep);

		OverworldState final = null;
		int taskCount = 0;
		while (workStack.Count > 0 && taskCount < 2000) {
		    taskCount += 1;
		    var p = workStack.Pop();
		    var r = p();
		    if (r.final != null) {
			final = r.final;
			break;
		    }
		    if (r.additionalTasks != null) {
			foreach (var v in r.additionalTasks) {
			    workStack.Push(v);
			}
		    }
		}
		if (final != null) {
		    return new ReplacementMap(final, mt);
		}
	    }
	    throw new Exception("Couldn't generate a map after 5 tries");
	}
    }
}
