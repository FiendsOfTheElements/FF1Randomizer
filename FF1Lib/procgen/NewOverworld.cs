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
        public int RegionId;
        public List<SCCoords> Points;
        public List<OwRegion> Adjacent;
        //SCCoords NWCorner;
        //SCCoords SECorner;

        public OwRegion(int regionType, int regionid) {
            this.RegionType = regionType;
            this.RegionId = regionid;
            this.Points = new List<SCCoords>();
            this.Adjacent = new List<OwRegion>();
        }

        public void AddPoint(SCCoords p) {
            this.Points.Add(p);
        }

        public static Tuple<OwRegion[,], List<OwRegion>> FindRegions(byte[,] tilemap, Dictionary<byte, int> tileRegionTypeMap) {
            var regionMap = new OwRegion[OverworldState.MAPSIZE,OverworldState.MAPSIZE];
            var regionList = new List<OwRegion>();

            regionList.Add(new OwRegion(tileRegionTypeMap[OverworldTiles.OCEAN], 0));

            var workingStack = new Stack<ValueTuple<SCCoords, OwRegion>>();
            var workingQueue = new Queue<ValueTuple<SCCoords, OwRegion>>();

            workingStack.Push(ValueTuple.Create(new SCCoords(0, 0), regionList[0]));

            while (workingStack.Count > 0) {
                while (workingStack.Count > 0) {
                    var tp = workingStack.Pop();
                    var p = tp.Item1;
                    var rg = tp.Item2;
                    if (regionMap[p.Y,p.X] != null) {
                        continue;
                    }

                    regionMap[p.Y,p.X] = rg;
                    rg.AddPoint(p);

                    var curRegionType = tileRegionTypeMap[tilemap[p.Y,p.X]];

                    var adjacent = new SCCoords[] {
                        p.OwUp, p.OwRight, p.OwDown, p.OwLeft
                    };
                    foreach (var adj in adjacent) {
                        var adjRegionType = tileRegionTypeMap[tilemap[adj.Y,adj.X]];
                        if (adjRegionType == curRegionType) {
                            if (regionMap[adj.Y,adj.X] == null) {
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
                    if (regionMap[p.Y,p.X] == null) {
                        nextRegion = new OwRegion(tileRegionTypeMap[tilemap[p.Y,p.X]], regionList.Count);
                        regionList.Add(nextRegion);
                        workingStack.Push(ValueTuple.Create(p, nextRegion));
                    } else {
                        nextRegion = regionMap[p.Y,p.X];
                    }
                    if (rg != nextRegion) {
                        if (!regionList[rg.RegionId].Adjacent.Contains(nextRegion)) {
                            regionList[rg.RegionId].Adjacent.Add(nextRegion);
                        }
                        if (!regionList[nextRegion.RegionId].Adjacent.Contains(rg)) {
                            regionList[nextRegion.RegionId].Adjacent.Add(rg);
                        }
                    }
                }
            }

            return Tuple.Create(regionMap, regionList);
        }
    }

    public class OverworldState {
        public const int MAPSIZE = 256;
        double[,] Basemap;
        public byte[,] Tilemap;
        OwRegion[,] Biome_regionmap;
        List<OwRegion> Biome_regionlist;
        OwRegion[,] Traversable_regionmap;
        List<OwRegion> Traversable_regionlist;
        int[,] Feature_weightmap;
        int[,] Dock_weightmap;
        public Dictionary<string, SCCoords> FeatureCoordinates;
        public SCCoords Airship;
        public SCCoords Bridge;
        public SCCoords Canal;
        public SCCoords Ship;
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

        public OverworldState(MT19337 rng, GenerationStep[] steps, OverworldTiles overworldTiles) {
            this.rng = rng;
            this.ownBasemap = true;
            this.ownTilemap = true;
            this.ownRegions = true;
            this.ownPlacements = true;
            this.Basemap = new double[MAPSIZE,MAPSIZE];
            this.Tilemap = new byte[MAPSIZE,MAPSIZE];
            this.Biome_regionmap = new OwRegion[MAPSIZE,MAPSIZE];
            this.Biome_regionlist = new List<OwRegion>();
            this.Traversable_regionmap = new OwRegion[MAPSIZE,MAPSIZE];
            this.Traversable_regionlist = new List<OwRegion>();
            this.Feature_weightmap  = new int[MAPSIZE,MAPSIZE];
            this.Dock_weightmap = new int[MAPSIZE,MAPSIZE];
            this.FeatureCoordinates = new Dictionary<string, SCCoords>();
            this.Reachable_regions = new List<int>();
            this.Exclude_docks = new List<int>();
            this.StepQueue = new Queue<GenerationStep>(steps);
            this.overworldTiles = overworldTiles;
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
            this.Airship = copy.Airship;
            this.Bridge = copy.Bridge;
            this.Canal = copy.Canal;
            this.Ship = copy.Ship;
            this.rng = copy.rng;
            this.StepQueue = copy.StepQueue;
            this.heightmax = copy.heightmax;
            this.mountain_elevation = copy.mountain_elevation;
            this.sea_elevation = copy.sea_elevation;
            this.overworldTiles = copy.overworldTiles;
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
                this.Biome_regionmap = new OwRegion[MAPSIZE,MAPSIZE];
                Array.Copy(fromBiome_regionmap, this.Tilemap, fromBiome_regionmap.Length);
                this.Biome_regionlist = new List<OwRegion>(this.Biome_regionlist);

                var fromTraversable_regionmap = this.Traversable_regionmap;
                this.Traversable_regionmap = new OwRegion[MAPSIZE,MAPSIZE];
                Array.Copy(fromTraversable_regionmap, this.Tilemap, fromTraversable_regionmap.Length);
                this.Traversable_regionlist = new List<OwRegion>(this.Traversable_regionlist);
                this.ownRegions = true;
        }

        void OwnPlacements() {
            if (this.ownPlacements) {
                return;
            }
            var fromFeature_weightmap = this.Feature_weightmap;
            this.Feature_weightmap = new int[MAPSIZE,MAPSIZE];
            Array.Copy(fromFeature_weightmap, this.Tilemap, fromFeature_weightmap.Length);

            var fromDock_weightmap = this.Dock_weightmap;
            this.Dock_weightmap = new int[MAPSIZE,MAPSIZE];
            Array.Copy(fromDock_weightmap, this.Tilemap, fromDock_weightmap.Length);

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

            const double land_pct = .26;
            const double mountain_pct = 0.055;
            this.heightmax = -100;
            this.mountain_elevation = 1-mountain_pct;
            this.sea_elevation = 1-land_pct;

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
                    sea_elevation -= .005;
                }
                if (mountain_count < min_mtn_tiles) {
                    mountain_elevation -= .005;
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

        public Result ApplyFilter(OwTileFilter filter) {
            this.OwnTilemap();
            this.Tilemap = filter.ApplyFilter(this.Tilemap);
            return this.NextStep();
        }

        public void FlowRiver(SCCoords p) {
            this.OwnTilemap();

            int volume = 256;
            var pending = new SortedList<double, SCCoords>();
            pending[-this.Basemap[p.Y,p.X]] = p;
            while (volume > 0 && pending.Count > 0) {
                p = pending.Values[pending.Count-1];
                pending.RemoveAt(pending.Count-1);

                if (this.Tilemap[p.Y,p.X] == OverworldTiles.OCEAN) {
                    return;
                }

                volume -= 1;

                if (this.Tilemap[p.Y,p.X] == OverworldTiles.RIVER) {
                    continue;
                }

               this.Tilemap[p.Y,p.X] = OverworldTiles.RIVER;
            
               pending[-this.Basemap[p.Y,p.X+1]] = p.OwRight;
               pending[-this.Basemap[p.Y+1,p.X]] = p.OwDown;
               pending[-this.Basemap[p.Y,p.X-1]] = p.OwLeft;
               pending[-this.Basemap[p.Y-1,p.X]] = p.OwUp;
            }

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
                this.FlowRiver(points[i]);
            }
        }

        public Result FlowMountainRivers() {
            this.FlowRivers(this.mountain_elevation + (this.heightmax-this.mountain_elevation)*.5, this.heightmax, 10);
            return this.NextStep();            
        }

        public Result FlowPlainsRivers() {
            this.FlowRivers(this.sea_elevation + (this.mountain_elevation-this.sea_elevation)*.5, this.mountain_elevation, 10);
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
                if (r.Adjacent.Count == 1 && r.Adjacent[0].RegionType == OverworldTiles.OCEAN_REGION) {
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
                    byte tile = this.overworldTiles.PreShoreRegionTypes[mergeWith.RegionType][0];
                    foreach (var p in r.Points) {
                        this.Tilemap[p.Y,p.X] = tile;
                    }
                }
            }
            return this.UpdateRegions();
        }


        public void Splat(SCCoords p, byte biome) {
            var sz = this.rng.Between(200, 400);

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
                    this.Splat(p, b);
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
        MethodInfo method;
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

        public ReplacementMap(OverworldState st) {
            Tiles = new List<List<byte>>();
            for (int y = 0; y < OverworldState.MAPSIZE; y++) {
                Tiles.Add(new List<byte>());
                for (int x = 0; x < OverworldState.MAPSIZE; x++) {
                    Tiles[y].Add(st.Tilemap[y,x]);
                }
            }
            ExchangeData = new OwMapExchangeData();
            //ExchangeData.StartingLocation = new SCCoords(st.FeatureCoordinates["ConeriaCastle1"].X, st.FeatureCoordinates["ConeriaCastle1"].Y+5);
            ExchangeData.AirShipLocation = st.Airship;
            ExchangeData.BridgeLocation = st.Bridge;
            ExchangeData.CanalLocation = st.Canal;
            ExchangeData.ShipLocations = new ShipLocation[] {
                new ShipLocation(st.Ship.X, st.Ship.Y, 255)
            };
            //ExchangeData.TeleporterFixups = new TeleportFixup[] {
            //    new TeleportFixup(FF1Lib.TeleportType.Exit, 0, new TeleData((MapId)0xFF, st.FeatureCoordinates["TitansTunnelEast"].X, st.FeatureCoordinates["TitansTunnelEast"].Y))
            //};
            ExchangeData.OverworldCoordinates = st.FeatureCoordinates;
        }
        public List<List<byte>> Tiles;
        public OwMapExchangeData ExchangeData;
    }

    public static class NewOverworld {

        public static ReplacementMap GenerateNewOverworld(MT19337 rng) {
            var mt = new OverworldTiles();
            GenerationStep[] steps = new GenerationStep[] {
                new GenerationStep("CreateInitialMap", new object[]{}),
                new GenerationStep("ApplyFilter", new object[] {mt.expand_mountains}),
                new GenerationStep("ApplyFilter", new object[] {mt.expand_oceans}),
                new GenerationStep("FlowMountainRivers", new object[] {}),
                new GenerationStep("FlowPlainsRivers", new object[] {}),
                new GenerationStep("ApplyFilter", new object[] {mt.connect_diagonals}),
                new GenerationStep("UpdateRegions", new object[]{}),
                new GenerationStep("RemoveSmallIslands", new object[]{}),
                new GenerationStep("AddBiomes", new object[]{}),

                new GenerationStep("ApplyFilter", new object[]{mt.remove_salients}),
                new GenerationStep("ApplyFilter", new object[]{mt.remove_salients}),
                new GenerationStep("ApplyFilter", new object[]{mt.remove_salients}),
                new GenerationStep("UpdateRegions", new object[]{}),
                new GenerationStep("RemoveTinyRegions", new object[]{}),

                new GenerationStep("ApplyFilter", new object[]{mt.remove_salients}),
                new GenerationStep("ApplyFilter", new object[]{mt.remove_salients}),
                new GenerationStep("ApplyFilter", new object[]{mt.remove_salients}),
                new GenerationStep("UpdateRegions", new object[]{}),
                new GenerationStep("RemoveTinyRegions", new object[]{}),

                new GenerationStep("SmallSeasBecomeLakes", new object[]{}),

                new GenerationStep("ApplyFilter", new object[]{mt.apply_shores1}),
                new GenerationStep("ApplyFilter", new object[]{mt.apply_shores2}),
                new GenerationStep("ApplyFilter", new object[]{mt.apply_shores3}),
                new GenerationStep("ApplyFilter", new object[]{mt.apply_shores4}),
                new GenerationStep("ApplyFilter", new object[]{mt.mountain_borders}),
                new GenerationStep("ApplyFilter", new object[]{mt.river_borders}),
                new GenerationStep("ApplyFilter", new object[]{mt.desert_borders}),
                new GenerationStep("ApplyFilter", new object[]{mt.marsh_borders}),
                new GenerationStep("ApplyFilter", new object[]{mt.grass_borders}),
                new GenerationStep("ApplyFilter", new object[]{mt.forest_borders}),
            };

            Stack<GenerationTask> workStack = new Stack<GenerationTask>();

            workStack.Push(new OverworldState(rng, steps, mt).NextStep);

            OverworldState final = null;
            int taskCount = 0;
            while (workStack.Count > 0) {
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

            return new ReplacementMap(final);
        }
    }

}
