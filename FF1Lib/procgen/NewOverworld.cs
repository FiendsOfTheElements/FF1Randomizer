using System.Collections.Generic;
using System;
using System.Reflection;
using RomUtilities;
using FF1Lib.Sanity;

namespace FF1Lib.Procgen
{

    public static class RNGExtensions {
       public static double Uniform(this MT19337 rng, double low, double high)
        {
                double range = (high - low);
                return low + range * ((double)rng.Next() / uint.MaxValue);
        }
    }

     
    class OwRegion {
        byte Tile;
        int RegionId;
        List<SCCoords> SCCoordss;
        List<int> Adjacent;
        //SCCoords NWCorner;
        //SCCoords SECorner;

        OwRegion(byte tile, int regionid) {
            this.Tile = tile;
            this.RegionId = regionid;
            this.SCCoordss = new List<SCCoords>();
            this.Adjacent = new List<int>();
            }
        }

    public class OverworldState {
        public const int MAPSIZE = 256;
        double[,] Basemap;
        public byte[,] Tilemap;
        int[,] Biome_regionmap;
        List<OwRegion> Biome_regionlist;
        int[,] Traversable_regionmap;
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

        private MT19337 rng;

        public OverworldState(MT19337 rng, GenerationStep[] steps) {
            this.rng = rng;
            this.ownBasemap = true;
            this.ownTilemap = true;
            this.ownRegions = true;
            this.ownPlacements = true;
            this.Basemap = new double[MAPSIZE,MAPSIZE];
            this.Tilemap = new byte[MAPSIZE,MAPSIZE];
            this.Biome_regionmap = new int[MAPSIZE,MAPSIZE];
            this.Biome_regionlist = new List<OwRegion>();
            this.Traversable_regionmap = new int[MAPSIZE,MAPSIZE];
            this.Traversable_regionlist = new List<OwRegion>();
            this.Feature_weightmap  = new int[MAPSIZE,MAPSIZE];
            this.Dock_weightmap = new int[MAPSIZE,MAPSIZE];
            this.FeatureCoordinates = new Dictionary<string, SCCoords>();
            this.Reachable_regions = new List<int>();
            this.Exclude_docks = new List<int>();
            this.StepQueue = new Queue<GenerationStep>(steps);
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
                this.Biome_regionmap = new int[MAPSIZE,MAPSIZE];
                Array.Copy(fromBiome_regionmap, this.Tilemap, fromBiome_regionmap.Length);
                this.Biome_regionlist = new List<OwRegion>(this.Biome_regionlist);

                var fromTraversable_regionmap = this.Traversable_regionmap;
                this.Traversable_regionmap = new int[MAPSIZE,MAPSIZE];
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

        const double UNSET = -1000000;
        const double perturb_reduction = .63;

        void PerturbSCCoords(int x0, int y0, int x1, int y1, double r0) {
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
 
            this.PerturbSCCoords(x0, y0, x2, y2, r0);
            this.PerturbSCCoords(x2, y0, x1, y2, r0);
            this.PerturbSCCoords(x0, y2, x2, y1, r0);
            this.PerturbSCCoords(x2, y2, x1, y1, r0);
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

            this.PerturbSCCoords(border, border, MAPSIZE-1-border, MAPSIZE-1-border, 1);

            const double land_pct = .26;
            const double mountain_pct = 0.055;
            double mountain_elevation = 1-mountain_pct;
            double sea_elevation = 1-land_pct;

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
                        this.Tilemap[y,x] = MapTiles.MOUNTAIN;
                    }
                    else if (height > sea_elevation) {
                        this.Tilemap[y,x] = MapTiles.LAND;
                    }
                    else {
                        this.Tilemap[y,x] = MapTiles.OCEAN;
                    }
                }
            }

            return this.NextStep();
        }

        public Result NextStep() {
            if (this.StepQueue.Count == 0) {
                return new Result(this);
            }
            this.StepQueue = new Queue<GenerationStep>(this.StepQueue);
            var nextStep = this.StepQueue.Dequeue();
            return nextStep.RunStep(new OverworldState(this));
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
            GenerationStep[] steps = new GenerationStep[] {
                new GenerationStep("CreateInitialMap", new object[]{}),
            };

            Stack<GenerationTask> workStack = new Stack<GenerationTask>();

            workStack.Push(new OverworldState(rng, steps).NextStep);

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
