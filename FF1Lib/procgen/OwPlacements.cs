using FF1Lib.Sanity;

namespace FF1Lib.Procgen
{

	public partial class OverworldState {

	public async Task<Result> BridgeAlternatives() {
	    var o1 = new OverworldState(this);
	    var o2 = new OverworldState(this);
	    o1.shouldPlaceBridge = true;
	    o2.shouldPlaceBridge = false;

	    await Task.Yield();
	    return new Result(new List<GenerationTask>() {
			o2.NextStep, o1.NextStep });
	}

	public async Task<Result> PlaceInStartingArea(OwFeature feature) {
	    if (this.startingRegion == -1) {
		var tasks = new List<GenerationTask>();
		var sorted = new List<OwRegion>();
		foreach (var w in this.Traversable_regionlist) {
		    if (w.RegionType == OverworldTiles.LAND_REGION) {
			sorted.Add(w);
		    }
		}
		sorted.Sort((OwRegion x, OwRegion y) => (x.Points.Count - y.Points.Count));
		foreach (var w in sorted) {
		    tasks.Add(() => new OverworldState(this).StartingAreaPlacement(feature, w));
		}
		return new Result(tasks);
	    } else {
		return await this.StartingAreaPlacement(feature, this.StartingRegion);
	    }
	}

	public async Task<Result> StartingAreaPlacement(OwFeature feature, OwRegion region) {
	    var v = this.PlaceFeature(this.Traversable_regionmap, region, feature);
	    if (v.Item1) {
		this.OwnPlacements();
		if (this.startingRegion == -1) {
		    this.startingRegion = region.RegionId;
		    this.DockPlacement(this.StartingRegion);
		}
		this.Reachable_regions.Add(region.RegionId);

		return await this.NextStep();
	    } else {
		return new Result(false);
	    }
	}

	public async Task<Result> PlaceBridge(bool fromStartingRegion, bool makeBridgeRequired) {
	    if (!this.shouldPlaceBridge) {
		return await this.NextStep();
	    }
	    var tasks = new List<GenerationTask>();
	    Action<OwRegion> placementTasks = (OwRegion r) => {
		foreach (var adj in r.Adjacent) {
		    var w = this.Traversable_regionlist[adj];
		    if (w.RegionType == OverworldTiles.RIVER_REGION) {
			tasks.Add(() => new OverworldState(this).BridgePlacement(r, w, makeBridgeRequired));
		    }
		}
	    };
	    if (fromStartingRegion) {
		placementTasks(this.StartingRegion);
	    } else {
		foreach (var r in this.Traversable_regionlist) {
		    if (r.RegionType != OverworldTiles.LAND_REGION) {
			continue;
		    }
		    placementTasks(r);
		}
	    }
	    tasks.Shuffle(this.rng);
	    return new Result(tasks);
	}

	public bool MakeIsolated(int nextRegion, OwRegion originRegion) {
	    var exclude = new List<int>();
	    this.FindAllReachableByRiver(this.Traversable_regionlist[nextRegion], exclude);
	    if (originRegion != null && exclude.Contains(originRegion.RegionId)) {
		// Loops back to starting region
		return false;
	    }

	    this.OwnPlacements();
	    int pointCount = 0;
	    foreach (var e in exclude) {
		var excl = this.Traversable_regionlist[e];
		pointCount +=  excl.Points.Count;
		if (pointCount > 1024) {
		    // This is going to exclude too large of an area
		    return false;
		}
		if (e != nextRegion && this.Reachable_regions.Contains(e)) {
		    // already reachable by some other means
		    return false;
		}
		if (excl.RegionType == OverworldTiles.RIVER_REGION && excl.Adjacent.Contains(OverworldTiles.MainOceanRegionId)) {
		    excl = new OwRegion(excl);
		    foreach (var p in excl.Points) {
			this.Tilemap[p.Y, p.X] = OverworldTiles.MARSH;
		    }
		    excl.RegionType = OverworldTiles.LAND_REGION;
		    this.Traversable_regionlist[excl.RegionId] = excl;

		    var biomeRiverRegion = new OwRegion(this.Biome_regionlist[this.Biome_regionmap[excl.Points[0].Y, excl.Points[0].X]]);
		    biomeRiverRegion.RegionType = OverworldTiles.MARSH_REGION;
		    this.Biome_regionlist[biomeRiverRegion.RegionId] = biomeRiverRegion;
		}
		this.Reachable_regions.Add(e);
		if (!this.Exclude_docks.Contains(e)) {
		    this.Exclude_docks.Add(e);
		}
		if (!this.Exclude_airship.Contains(e)) {
		    this.Exclude_airship.Add(e);
		}
	    }
	    return true;
	}

	public async Task<Result> BridgePlacement(OwRegion originRegion, OwRegion riverRegion, bool makeBridgeRequired) {
	    var points = new List<SCCoords>(riverRegion.Points);
	    points.Shuffle(this.rng);
	    short nextRegion = -1;
	    foreach (var p in points) {
		for (int i = 0; i < 2; i++) {
		    short c1, c2;
		    if (i == 0) {
			c1 = this.Traversable_regionmap[p.Y-1, p.X];
			c2 = this.Traversable_regionmap[p.Y+1, p.X];
		    } else {
			c1 = this.Traversable_regionmap[p.Y, p.X-1];
			c2 = this.Traversable_regionmap[p.Y, p.X+1];
		    }

		    if (c1 == originRegion.RegionId &&
			c2 != originRegion.RegionId &&
			c2 != this.startingRegion &&
			this.Traversable_regionlist[c2].RegionType == OverworldTiles.LAND_REGION)
		    {
			this.OwnPlacements();
			this.FeatureCoordinates["Bridge"] = p;
			nextRegion = c2;
			break;
		    }

		    if (c1 != originRegion.RegionId &&
			c2 == originRegion.RegionId &&
			c1 != this.startingRegion &&
			this.Traversable_regionlist[c1].RegionType == OverworldTiles.LAND_REGION)
		    {
			this.OwnPlacements();
			this.FeatureCoordinates["Bridge"] = p;
			nextRegion = c1;
			break;
		    }
		}
	    }

	    if (nextRegion != -1) {
		this.OwnPlacements();
		this.bridgeOriginRegion = originRegion.RegionId;
		this.bridgedRegion = nextRegion;
		this.Reachable_regions.Add(nextRegion);
		this.Exclude_docks.Add(nextRegion);

		this.OwnRegions();
		this.OwnTilemap();
		// turn bridged region into ocean
		foreach (var p in riverRegion.Points) {
		    this.Tilemap[p.Y, p.X] = OverworldTiles.OCEAN;
		}

		if (riverRegion.Adjacent.Contains(OverworldTiles.MainOceanRegionId)) {
		    var biomeRiverRegion = this.Biome_regionlist[this.Biome_regionmap[riverRegion.Points[0].Y, riverRegion.Points[0].X]];
		    OwRegion.Merge(this.Traversable_regionmap,
				   this.Traversable_regionlist,
				   riverRegion, this.Traversable_regionlist[OverworldTiles.MainOceanRegionId]);
		    OwRegion.Merge(this.Biome_regionmap,
				   this.Biome_regionlist,
				   biomeRiverRegion, this.Biome_regionlist[OverworldTiles.MainOceanRegionId]);
		} else {
		    this.Traversable_regionlist[riverRegion.RegionId] = new OwRegion(riverRegion);
		    this.Traversable_regionlist[riverRegion.RegionId].RegionType = OverworldTiles.OCEAN_REGION;

		    riverRegion = this.Biome_regionlist[this.Biome_regionmap[riverRegion.Points[0].Y, riverRegion.Points[0].X]];
		    this.Biome_regionlist[riverRegion.RegionId] = new OwRegion(riverRegion);
		    this.Biome_regionlist[riverRegion.RegionId].RegionType = OverworldTiles.OCEAN_REGION;
		}

		if (makeBridgeRequired) {
		    // If true, make it so that the bridge _must_ be
		    // crossed to get to the next region by banning
		    // docks and airship landing spots.

		    if (!this.MakeIsolated(nextRegion, originRegion)) {
			return new Result(false);
		    }
		}

		if (!this.DockPlacement(originRegion)) {
		    return new Result(false);
		}

		//this.Tilemap[this.FeatureCoordinates["Bridge"].Y, this.FeatureCoordinates["Bridge"].X] = OverworldTiles.DOCK_W;
		return await this.NextStep();
	    }
	    return new Result(false);
	}

	public async Task<Result> CheckBridgeShores() {
	    // If any of the previous passes messed up the bridge we need
	    // to fix it.
	    var traversable_tiles = new HashSet<byte>(OverworldTiles.TraversableRegionTypes[OverworldTiles.LAND_REGION]);
	    traversable_tiles.Remove(OverworldTiles.SHORE_NW);
	    traversable_tiles.Remove(OverworldTiles.SHORE_NE);
	    traversable_tiles.Remove(OverworldTiles.SHORE_SW);
	    traversable_tiles.Remove(OverworldTiles.SHORE_SE);

	    var b = this.FeatureCoordinates["Bridge"];

	    if (this.Traversable_regionlist[this.Traversable_regionmap[this.FeatureCoordinates["Bridge"].Y, this.FeatureCoordinates["Bridge"].X+1]].RegionType == OverworldTiles.LAND_REGION) {
		// Horizontal bridge
		if (!traversable_tiles.Contains(this.Tilemap[b.Y, b.X-1])) {
		    this.OwnTilemap();
		    this.Tilemap[b.Y, b.X-1] = OverworldTiles.LAND;
		}
		if (!traversable_tiles.Contains(this.Tilemap[b.Y, b.X+1])) {
		    this.OwnTilemap();
		    this.Tilemap[b.Y, b.X+1] = OverworldTiles.LAND;
		}
	    } else {
		// Vertical bridge
		if (!traversable_tiles.Contains(this.Tilemap[b.Y-1, b.X])) {
		    this.OwnTilemap();
		    this.Tilemap[b.Y-1, b.X] = OverworldTiles.LAND;
		}
		if (!traversable_tiles.Contains(this.Tilemap[b.Y+1, b.X])) {
		    this.OwnTilemap();
		    this.Tilemap[b.Y+1, b.X] = OverworldTiles.LAND;
		}
	    }
	    return await this.NextStep();
	}

	public async Task<Result> PlacePravoka() {
	    if (this.bridgedRegion != -1) {
		return await this.CoastalPlacement(OverworldTiles.PRAVOKA_CITY, this.Traversable_regionlist[this.bridgedRegion], true, false);
	    } else {
		return await this.CoastalPlacement(OverworldTiles.PRAVOKA_CITY_MOAT, this.StartingRegion, true, false);
	    }
	}

	public async Task<Result> PlaceInBridgedRegion(OwFeature feature) {
	    if (this.bridgedRegion == -1) {
		return await this.NextStep();
	    }
	    for (int i = 0; i < 8; i++) {
		var v = this.PlaceFeature(this.Traversable_regionmap, this.Traversable_regionlist[this.bridgedRegion], feature, i);
		if (v.Item1) {
		    return await this.NextStep();
		}
	    }
	    return new Result(false);
	}

	public async Task<Result> PlaceOnCoast(OwFeature feature, bool eastOnly) {
	    var tasks = new List<GenerationTask>();
	    foreach (var w in this.Traversable_regionlist) {
		if (w.RegionType == OverworldTiles.LAND) {
		    if (w.Adjacent.Contains(OverworldTiles.MainOceanRegionId) &&
			(this.CheckReachableByRiver(w, false) || CheckIndirectAirshipAccess(w).Item1))
		    {
			tasks.Add(() => new OverworldState(this).CoastalPlacement(feature, w, false, eastOnly));
		    }
		}
	    }
	    tasks.Shuffle(this.rng);
	    await Task.Yield();
	    return new Result(tasks);
	}

	public async Task<Result> CoastalPlacement(OwFeature feature, OwRegion region, bool fill, bool eastOnly) {
	    var points = new List<SCCoords>(region.Points);
	    points.Shuffle(this.rng);
	    int w = feature.Tiles.GetLength(1);
	    int h = feature.Tiles.GetLength(0);
	    foreach (var p in points) {
			if (p.Y+h-1 >= OverworldState.MAPSIZE || p.X+w >= OverworldState.MAPSIZE) {
				continue;
			}
		int makeland = 0;
		if (!eastOnly && this.Traversable_regionmap[p.Y+h-2, p.X-1] == OverworldTiles.MainOceanRegionId) {
		    makeland = w-1;
		} else if (this.Traversable_regionmap[p.Y+h-2, p.X+w] == OverworldTiles.MainOceanRegionId) {
		    makeland = 0;
		} else {
		    continue;
		}
		var pf = this.PlaceFeatureAt(this.Traversable_regionmap, region, p, feature);
		if (!pf.Item1) {
		    continue;
		}
		if (fill) {
		    this.Tilemap[p.Y+h-1, p.X+makeland] = OverworldTiles.LAND;
		    this.Tilemap[p.Y+h-2, p.X+makeland] = OverworldTiles.LAND;
		}
		if (feature.Entrances.ContainsKey("Ship")) {
		    this.OwnPlacements();
		    var s = feature.Entrances["Ship"];
		    this.DockPlacements.Add(new ValueTuple<short, SCCoords>(region.RegionId, new SCCoords(p.X+s.X, p.Y+s.Y)));
		    this.Reachable_regions.Add(region.RegionId);
		}
		return await this.NextStep();
	    }
	    return new Result(false);
	}

	public ValueTuple<bool, SCCoords> CheckAirshipAccess(OwRegion region) {
	    foreach (var p in region.Points) {
		if (this.Tilemap[p.Y, p.X] == OverworldTiles.LAND ||
		    this.Tilemap[p.Y, p.X] == OverworldTiles.GRASS)
		{
		    var count = 0;
		    var up = p.OwUp;
		    var right = p.OwRight;
		    var down = p.OwDown;
		    var left = p.OwLeft;
		    if (this.Tilemap[up.Y, up.X] == OverworldTiles.OCEAN) count++;
		    if (this.Tilemap[right.Y, right.X] == OverworldTiles.OCEAN) count++;
		    if (this.Tilemap[down.Y, down.X] == OverworldTiles.OCEAN) count++;
		    if (this.Tilemap[left.Y, left.X] == OverworldTiles.OCEAN) count++;

		    // Don't consider tiles that are bordered on two
		    // or more sides by water, there is a possibility
		    // that they get modified or eliminated by later
		    // filters.
		    if (count < 2) {
			return ValueTuple.Create(true, p);
		    }
		}
	    }
	    return ValueTuple.Create(false, new SCCoords(0,0));
	}

	public ValueTuple<bool, SCCoords> CheckIndirectAirshipAccess(OwRegion region, List<int> checkedRegions = null) {
	    if (checkedRegions == null) {
		checkedRegions = new List<int>();
	    }
	    if (checkedRegions.Contains(region.RegionId)) {
		return ValueTuple.Create(false, new SCCoords(0,0));
	    }
	    checkedRegions.Add(region.RegionId);

	    if (region.RegionType == OverworldTiles.LAND_REGION) {
		var r = this.CheckAirshipAccess(region);
		if (r.Item1) {
		    return r;
		}
	    }

	    foreach (var adj in region.Adjacent) {
		if (this.Traversable_regionlist[adj].RegionType == OverworldTiles.LAND_REGION ||
		    this.Traversable_regionlist[adj].RegionType == OverworldTiles.RIVER_REGION)
		{
		    var r = this.CheckIndirectAirshipAccess(this.Traversable_regionlist[adj], checkedRegions);
		    if (r.Item1) {
			return r;
		    }
		}
	    }
	    return ValueTuple.Create(false, new SCCoords(0,0));
	}

	public bool CheckReachableByRiver(OwRegion region, bool requireRiver, bool traversedRiver = false, List<int> checkedRegions = null) {
	    if (checkedRegions == null) {
		checkedRegions = new List<int>();
	    }
	    if (checkedRegions.Contains(region.RegionId)) {
		return false;
	    }
	    checkedRegions.Add(region.RegionId);

	    if (region.RegionType == OverworldTiles.LAND_REGION) {
		if (this.Reachable_regions.Contains(region.RegionId)) {
		    if (requireRiver) {
			return traversedRiver;
		    } else {
			return true;
		    }
		}
	    }

	    if (region.RegionType == OverworldTiles.RIVER_REGION) {
		if (region.Adjacent.Contains(OverworldTiles.MainOceanRegionId)) {
		    return true;
		}
		traversedRiver = true;
	    }

	    foreach (var adj in region.Adjacent) {
		if (this.Traversable_regionlist[adj].RegionType == OverworldTiles.RIVER_REGION ||
		    this.Traversable_regionlist[adj].RegionType == OverworldTiles.LAND_REGION)
		{
		    if (this.CheckReachableByRiver(this.Traversable_regionlist[adj], requireRiver, traversedRiver, checkedRegions)) {
			return true;
		    }
		}
	    }
	    return false;
	}

	public void FindAllReachableByRiver(OwRegion region, List<int> checkedRegions) {
	    if (checkedRegions.Contains(region.RegionId)) {
		return;
	    }
	    checkedRegions.Add(region.RegionId);

	    foreach (var adj in region.Adjacent) {
		if (this.Traversable_regionlist[adj].RegionType == OverworldTiles.RIVER_REGION ||
		    this.Traversable_regionlist[adj].RegionType == OverworldTiles.LAND_REGION)
		{
		    this.FindAllReachableByRiver(this.Traversable_regionlist[adj], checkedRegions);
		}
	    }
	}

	public async Task<Result> PlaceIsolated(OwFeature feature, bool requireAirshipAccess) {
	    var tasks = new List<GenerationTask>();
	    foreach (var w in this.Traversable_regionlist) {
		if (w.RegionType == OverworldTiles.LAND_REGION &&
		    !this.Reachable_regions.Contains(w.RegionId))
		{
		    if (feature.MountainCave) {
			var caveCandidate = false;
			foreach (var adj in w.Adjacent) {
			    if (this.Traversable_regionlist[adj].RegionType == OverworldTiles.MOUNTAIN_REGION) {
				caveCandidate = true;
				break;
			    }
			}
			if (!caveCandidate) {
			    continue;
			}
		    }
		    var r = this.CheckIndirectAirshipAccess(w);
		    if (r.Item1 || !requireAirshipAccess) {
			if (!this.CheckReachableByRiver(w, false)) {
			    tasks.Add(() => new OverworldState(this).IsolatedPlacement(feature, w, requireAirshipAccess));
			}
		    }
		}
	    }
	    tasks.Shuffle(this.rng);
	    await Task.Yield();
	    return new Result(tasks);
	}

	public async Task<Result> IsolatedPlacement(OwFeature feature, OwRegion region, bool requireAirshipAccess) {
	    if (!requireAirshipAccess) {
		if (!this.Exclude_airship.Contains(region.RegionId)) {
		    this.OwnPlacements();
		    this.Exclude_airship.Add(region.RegionId);
		}
	    }
	    var v = this.PlaceFeature(this.Traversable_regionmap, region, feature);
	    if (v.Item1) {
		var exclude = new List<int>();
		this.FindAllReachableByRiver(region, exclude);
		foreach (var e in exclude) {
		    if (!this.Exclude_docks.Contains(e)) {
			this.Exclude_docks.Add(e);
		    }
		}
		return await this.NextStep();
	    } else {
		return new Result(false);
	    }
	}

	public async Task<Result> PlaceRequiringCanoe(OwFeature feature) {
	    var tasks = new List<GenerationTask>();
	    foreach (var w in this.Traversable_regionlist) {
		if (w.RegionType == OverworldTiles.LAND_REGION) {
		    var r = CheckAirshipAccess(w);
		    if (!r.Item1) {
			if (this.CheckReachableByRiver(w, true)) {
			    tasks.Add(() => new OverworldState(this).RequiresCanoePlacement(feature, w));
			}
		    }
		}
	    }
	    tasks.Shuffle(this.rng);
	    await Task.Yield();
	    return new Result(tasks);
	}

	public async Task<Result> RequiresCanoePlacement(OwFeature feature, OwRegion region) {
	    if (!this.Exclude_airship.Contains(region.RegionId)) {
		this.OwnPlacements();
		this.Exclude_airship.Add(region.RegionId);
	    }
	    var v = this.PlaceFeature(this.Traversable_regionmap, region, feature);
	    if (v.Item1) {
		var exclude = new List<int>();
		if (!this.Exclude_docks.Contains(region.RegionId)) {
		    this.Exclude_docks.Add(region.RegionId);
		}
		return await this.NextStep();
	    } else {
		return new Result(false);
	    }
	}

	public async Task<Result> PlaceInBiome(OwFeature feature, int[] biomes,
				   bool shipReachable,
				   bool canoeReachable,
				   bool airshipReachable,
				   bool preferIslands) {
	    var walkable = new List<OwRegion>();
	    List<int> biomelist;
	    if (biomes == null) {
		biomelist = new List<int> { OverworldTiles.LAND_REGION,
					     OverworldTiles.GRASS_REGION,
					     OverworldTiles.MARSH_REGION,
					     OverworldTiles.FOREST_REGION,
					     OverworldTiles.DESERT_REGION };
	    } else {
		biomelist = new List<int>(biomes);
	    }

	    var islands = new List<OwRegion>();
	    foreach (var w in this.Biome_regionlist) {
		bool found = false;
		if (biomelist.Contains(w.RegionType) && w.Points.Count > 0) {
		    var trav = this.Traversable_regionlist[this.Traversable_regionmap[w.Points[0].Y, w.Points[0].X]];
		    if (shipReachable &&
			!this.Exclude_docks.Contains(trav.RegionId) &&
			(trav.Adjacent.Contains(OverworldTiles.MainOceanRegionId) || this.Reachable_regions.Contains(trav.RegionId)))
		    {
			found = true;
		    } else if (airshipReachable && canoeReachable && this.CheckIndirectAirshipAccess(trav).Item1) {
			found = true;
		    } else if (airshipReachable && this.CheckAirshipAccess(trav).Item1) {
			found = true;
		    } else if (canoeReachable && this.CheckReachableByRiver(trav, false)) {
			found = true;
		    } else if (airshipReachable == false && shipReachable == false && canoeReachable == false
			       && this.Reachable_regions.Contains(trav.RegionId))
		    {
			// bridge, canal, or titan's tunnel access
			found = true;
		    }
		    if (found) {
			if (preferIslands && trav.Adjacent.Count == 1 && trav.Adjacent[0] == OverworldTiles.MainOceanRegionId) {
			    islands.Add(w);
			} else {
			    walkable.Add(w);
			}
		    }
		}
	    }

	    var tasks = new List<GenerationTask>();
	    walkable.Shuffle(this.rng);
	    islands.Shuffle(this.rng);

	    int maxweight;
	    if (feature.MountainCave) {
		maxweight = 0;
	    } else {
		maxweight = 8;
	    }
	    for (int i = 0; i <= maxweight; i++) {
		foreach (var w in islands) {
		    var r = await new OverworldState(this).BiomePlacement(feature, w, shipReachable, i);
		    if (r.Success) return r;
		}
		foreach (var w in walkable) {
		    var r = await new OverworldState(this).BiomePlacement(feature, w, shipReachable, i);
		    if (r.Success) return r;
		}
	    }

	    return new Result(false);
	}

	public async Task<Result> BiomePlacement(OwFeature feature, OwRegion region, bool shipReachable, int maxweight) {
	    var trav = this.Traversable_regionlist[this.Traversable_regionmap[region.Points[0].Y, region.Points[0].X]];
	    var v = this.PlaceFeature(this.Biome_regionmap, region, feature, maxweight);
	    if (!v.Item1) {
		return new Result(false);
	    }
	    if (shipReachable) {
		this.DockPlacement(trav);
		if (!this.Reachable_regions.Contains(trav.RegionId)) {
		    return new Result(false);
		}
	    }

	    if (feature == OverworldTiles.AIRSHIP_FEATURE) {
		this.airshipDesertRegion = region.RegionId;
	    }

	    return await this.NextStep();
	}

	public bool DockPlacement(OwRegion region) {
	    if (this.Exclude_docks.Contains(region.RegionId)) {
		return false;
	    }

	    var points = new List<SCCoords>(region.Points);
	    points.Shuffle(this.rng);
	    OwFeature placed = null;
		SCCoords placedAt = new SCCoords(0, 0);
	    foreach (var p in points) {
		var np = p.OwUp;
		if (this.Traversable_regionmap[np.Y, np.X] == OverworldTiles.MainOceanRegionId) {
		    var r = this.PlaceFeatureAt(this.Traversable_regionmap, region,
						new SCCoords(p.X-2, p.Y),
						OverworldTiles.N_DOCK_STRUCTURE, true);
		    if (r.Item1) {
				placedAt = p;
			placed = OverworldTiles.N_DOCK_STRUCTURE;
			break;
		    }
		}

		var ep = p.OwRight;
		if (this.Traversable_regionmap[ep.Y, ep.X] == OverworldTiles.MainOceanRegionId) {
		    var r = this.PlaceFeatureAt(this.Traversable_regionmap, region,
						new SCCoords(p.X-3, p.Y-2),
						OverworldTiles.E_DOCK_STRUCTURE, true);
		    if (r.Item1) {
				placedAt = p;
			placed = OverworldTiles.E_DOCK_STRUCTURE;
			break;
		    }
		}

		var sp = p.OwDown;
		if (this.Traversable_regionmap[sp.Y, sp.X] == OverworldTiles.MainOceanRegionId) {
		    var r = this.PlaceFeatureAt(this.Traversable_regionmap, region,
						new SCCoords(p.X-2, p.Y-2),
						OverworldTiles.S_DOCK_STRUCTURE, true);
		    if (r.Item1) {
				placedAt = p;
			placed = OverworldTiles.S_DOCK_STRUCTURE;
			break;
		    }
		}

		var wp = p.OwLeft;
		if (this.Traversable_regionmap[wp.Y, wp.X] == OverworldTiles.MainOceanRegionId) {
		    var r = this.PlaceFeatureAt(this.Traversable_regionmap, region,
						new SCCoords(p.X, p.Y-2),
						OverworldTiles.W_DOCK_STRUCTURE, true);
		    if (r.Item1) {
				placedAt = p;
			placed = OverworldTiles.W_DOCK_STRUCTURE;
			break;
		    }
		}
	    }

	    if (placed != null) {
		this.OwnPlacements();
		var s = placed.Entrances["Ship"];
		this.DockPlacements.Add(new ValueTuple<short, SCCoords>(region.RegionId, new SCCoords(placedAt.X+s.X, placedAt.Y+s.Y)));
		if (!this.Reachable_regions.Contains(region.RegionId)) {
		    this.Reachable_regions.Add(region.RegionId);
		}
		return true;
	    }
	    return false;
	}

	public async Task<Result> PlaceInTitanWestRegion(OwFeature feature) {
	    var p = this.FeatureCoordinates["TitansTunnelWest"];
	    var region = this.Traversable_regionlist[this.Traversable_regionmap[p.Y+1, p.X]];
	    var r = this.PlaceFeature(this.Traversable_regionmap, region, feature);
	    if (r.Item1) {
		return await this.NextStep();
	    } else {
		return new Result(false);
	    }
	}

	public async Task<Result> PlaceWaterfall(OwFeature feature) {
	    var tasks = new List<GenerationTask>();
	    foreach (var w in this.Traversable_regionlist) {
		if (w.RegionType == OverworldTiles.RIVER_REGION) {
		    var v = this.CheckIndirectAirshipAccess(w);
		    if (v.Item1) {
			tasks.Add(() => new OverworldState(this).WaterfallPlacement(feature, w));
		    }
		}
	    }
	    tasks.Shuffle(this.rng);
	    await Task.Yield();
	    return new Result(tasks);
	}

	public async Task<Result> WaterfallPlacement(OwFeature feature, OwRegion region) {
	    var points = new List<SCCoords>(region.Points);
	    points.Shuffle(this.rng);
	    foreach (var p in points) {
		if (this.Tilemap[p.Y, p.X-1] == OverworldTiles.MOUNTAIN &&
		    this.Tilemap[p.Y, p.X+1] == OverworldTiles.MOUNTAIN &&
		    this.Tilemap[p.Y-1, p.X] == OverworldTiles.RIVER &&
		    this.Tilemap[p.Y+1, p.X] == OverworldTiles.RIVER)
		{
		    this.PlaceFeatureAt(this.Traversable_regionmap, region, new SCCoords(p.X-1, p.Y-1), feature, false);
		    return await this.NextStep();
		}
	    }
	    return new Result(false);
	}

	public async Task<Result> PlaceInMountains(OwFeature feature) {
	    var tasks = new List<GenerationTask>();
	    foreach (var w in this.Traversable_regionlist) {
		if (w.RegionType == OverworldTiles.MOUNTAIN_REGION) {
		    foreach (var i in w.Adjacent) {
			if (this.Traversable_regionlist[i].RegionType == OverworldTiles.RIVER_REGION) {
			    var river = this.Traversable_regionlist[i];
			    var v = this.CheckIndirectAirshipAccess(river);
			    if (v.Item1) {
				tasks.Add(() => new OverworldState(this).MountainRiverPlacement(feature, w, river));
			    }
			}
		    }
		}
	    }
	    tasks.Shuffle(this.rng);
	    await Task.Yield();
	    return new Result(tasks);
	}

	public async Task<Result> MountainRiverPlacement(OwFeature feature, OwRegion mtnRegion, OwRegion riverRegion) {
	    var points = new List<SCCoords>(mtnRegion.Points);
	    points.Shuffle(this.rng);
	    OwFeature caveFeature = null;
	    if (feature.MountainCave) {
		caveFeature = feature;
		if (this.rng.Between(0, 1) == 0) {
		    feature = OverworldTiles.MOUNTAIN_CAVE_FEATURE;
		} else {
		    feature = OverworldTiles.FOREST_MOUNTAIN_CAVE_FEATURE;
		}
	    } else {
		feature = feature.NoneToMarsh();
	    }
	    int w = feature.Tiles.GetLength(1);
	    int h = feature.Tiles.GetLength(0);

	    foreach (var p in points) {
		if (this.Traversable_regionmap[p.Y+h, p.X+(w/2)] == riverRegion.RegionId) {
		    var v = this.PlaceFeatureAt(this.Traversable_regionmap, mtnRegion, p, feature, true);
		    if (v.Item1) {
			if (caveFeature != null) {
			    this.PlaceFeatureAt(this.Traversable_regionmap, mtnRegion, new SCCoords(p.X+2, p.Y+1), caveFeature, false);
			}
			return await this.NextStep();
		    }
		}
	    }
	    return new Result(false);
	}

	public async Task<Result> PlaceCanal(bool makeCanalRequired) {
	    var tasks = new List<GenerationTask>();
	    foreach (var w in this.Traversable_regionlist) {
		if (w.RegionType == OverworldTiles.LAND_REGION &&
		    !this.Reachable_regions.Contains(w.RegionId) &&
		    !this.Exclude_docks.Contains(w.RegionId) &&
		    w.Adjacent.Contains(OverworldTiles.MainOceanRegionId))
		{
		    tasks.Add(() => new OverworldState(this).CanalPlacement(w, makeCanalRequired));
		}
	    }
	    tasks.Shuffle(this.rng);
	    await Task.Yield();
	    return new Result(tasks);
	}

	public async Task<Result> CanalPlacement(OwRegion region, bool makeCanalRequired) {
	    if (makeCanalRequired) {
		if (!this.Exclude_airship.Contains(region.RegionId)) {
		    this.OwnPlacements();
		    this.Exclude_airship.Add(region.RegionId);
		}
	    }

	    var points = new List<SCCoords>(region.Points);
	    points.Shuffle(this.rng);
	    bool placed = false;
	    foreach (var p in points) {
		var ep = p.OwRight;
		if (this.Traversable_regionmap[ep.Y, ep.X] == OverworldTiles.MainOceanRegionId) {
		    var r = this.PlaceFeatureAt(this.Traversable_regionmap, region,
					new SCCoords(p.X-6, p.Y-2), OverworldTiles.E_CANAL_STRUCTURE, true);
		    if (r.Item1) {
			placed = true;
			break;
		    }
		}

		var wp = p.OwLeft;
		if (this.Traversable_regionmap[wp.Y, wp.X] == OverworldTiles.MainOceanRegionId) {
		    var r = this.PlaceFeatureAt(this.Traversable_regionmap, region,
					new SCCoords(p.X, p.Y-2), OverworldTiles.W_CANAL_STRUCTURE, true);
		    if (r.Item1) {
			placed = true;
			break;
		    }
		}

	    }

	    if (placed) {
		this.OwnPlacements();
		if (!this.Exclude_docks.Contains(region.RegionId)) {
		    this.Exclude_docks.Add(region.RegionId);
		}
		this.Reachable_regions.Add(region.RegionId);
		this.canalRegion = region.RegionId;

		if (makeCanalRequired) {
		    // If true, make it so that the canal is the only
		    // way to get to the target region by banning
		    // other docks and airship landing spots.

		    if (!this.MakeIsolated(region.RegionId, null)) {
			return new Result(false);
		    }
		}

		return await this.NextStep();
	    }
	    return new Result(false);
	}

	public async Task<Result> PlaceInCanalRegion(OwFeature feature) {
	    var r = this.PlaceFeature(this.Traversable_regionmap, this.Traversable_regionlist[this.canalRegion], feature);
	    if (r.Item1) {
		return await this.NextStep();
	    }
	    return new Result(false);
	}

	public async Task<Result> PreventAirshipLanding() {
	    this.OwnTilemap();
	    foreach (var er in Exclude_airship) {
		if (er == this.airshipDesertRegion) {
		    // Guarantee that you can always get back to the
		    // region where you got the airship, using the
		    // airship, to avoid soft lock.
		    continue;
		}
		var region = this.Traversable_regionlist[er];
		foreach (var p in region.Points) {
		    bool adjMarsh = false;
		    bool adjDesert = false;
		    bool adjRiver = false;
		    bool adjEntrance = false;
		    var adjCoors = new SCCoords[] {
			p, p.OwUp, p.OwRight, p.OwDown, p.OwLeft
		    };
		    foreach (var adj in adjCoors) {
			if (this.Tilemap[adj.Y, adj.X] == OverworldTiles.MARSH) {
			    adjMarsh = true;
			}
			if (this.Tilemap[adj.Y, adj.X] == OverworldTiles.DESERT) {
			    adjDesert = true;
			}
			if (this.Tilemap[adj.Y, adj.X] == OverworldTiles.RIVER) {
			    adjRiver = true;
			}
			if (OverworldTiles.Entrances.Contains(this.Tilemap[adj.Y, adj.X])) {
			    adjEntrance = true;
			}
		    }
		    foreach (var adj in adjCoors) {
			if (this.Tilemap[adj.Y, adj.X] == OverworldTiles.LAND ||
			    this.Tilemap[adj.Y, adj.X] == OverworldTiles.GRASS)
			{
			    if (adjMarsh) {
				this.Tilemap[adj.Y, adj.X] = OverworldTiles.MARSH;
			    } else if (adjDesert) {
				this.Tilemap[adj.Y, adj.X] = OverworldTiles.DESERT;
			    } else if (adjRiver && !adjEntrance) {
				this.Tilemap[adj.Y, adj.X] = OverworldTiles.RIVER;
			    } else {
				this.Tilemap[adj.Y, adj.X] = OverworldTiles.MARSH;
			    }
			}
		    }
		}
	    }
	    return await this.NextStep();
	}
    }
}
