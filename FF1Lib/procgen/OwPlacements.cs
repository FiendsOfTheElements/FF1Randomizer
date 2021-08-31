using System.Collections.Generic;
using System;
using System.Reflection;
using RomUtilities;
using FF1Lib.Sanity;
using System.Diagnostics;

namespace FF1Lib.Procgen
{

    public partial class OverworldState {

	public Result BridgeAlternatives() {
	    var o1 = new OverworldState(this);
	    var o2 = new OverworldState(this);
	    o1.shouldPlaceBridge = true;
	    o2.shouldPlaceBridge = false;

	    return new Result(new List<GenerationTask>() {
		    o2.NextStep, o1.NextStep });
	}

	public Result PlaceFeatureInStartingArea(OwFeature feature) {
	    if (this.startingRegion == -1) {
		var tasks = new List<GenerationTask>();
		foreach (var w in this.Traversable_regionlist) {
		    if (w.RegionType == OverworldTiles.LAND_REGION) {
			tasks.Add(() => new OverworldState(this).StartingAreaPlacement(feature, w));
		    }
		}
		tasks.Shuffle(this.rng);
		return new Result(tasks);
	    } else {
		return this.StartingAreaPlacement(feature, this.StartingRegion);
	    }
	}

	public Result StartingAreaPlacement(OwFeature feature, OwRegion region) {
	    var v = this.PlaceFeature(this.Traversable_regionmap, region, feature);
	    if (v.Item1) {
		this.OwnPlacements();
		this.startingRegion = region.RegionId;
		this.Reachable_regions.Add(region.RegionId);

		return this.NextStep();
	    } else {
		return new Result(false);
	    }
	}

	public Result PlaceBridge() {
	    if (!this.shouldPlaceBridge) {
		return this.NextStep();
	    }
	    var tasks = new List<GenerationTask>();
	    foreach (var adj in this.StartingRegion.Adjacent) {
		var w = this.Traversable_regionlist[adj];
		if (w.RegionType == OverworldTiles.RIVER_REGION) {
		    tasks.Add(() => new OverworldState(this).BridgePlacement(w));
		}
	    }
	    tasks.Shuffle(this.rng);
	    return new Result(tasks);
	}

	public Result BridgePlacement(OwRegion riverRegion) {
	    var points = new List<SCCoords>(riverRegion.Points);
	    points.Shuffle(this.rng);
	    int nextRegion = -1;
	    foreach (var p in points) {
		var c1 = this.Traversable_regionmap[p.Y-1, p.X];
		var c2 = this.Traversable_regionmap[p.Y+1, p.X];

		if (c1 == this.startingRegion &&
		    c2 != this.startingRegion &&
		    this.Traversable_regionlist[c2].RegionType == OverworldTiles.LAND_REGION)
		{
		    this.OwnPlacements();
		    this.FeatureCoordinates["Bridge"] = p;
		    nextRegion = c2;
		    break;
		}

		if (c1 != this.startingRegion &&
		    c2 == this.startingRegion &&
		    this.Traversable_regionlist[c1].RegionType == OverworldTiles.LAND_REGION)
		{
		    this.OwnPlacements();
		    this.FeatureCoordinates["Bridge"] = p;
		    nextRegion = c1;
		    break;
		}
	    }

	    if (nextRegion != -1) {
		this.bridgedRegion = nextRegion;
		this.Reachable_regions.Add(nextRegion);
		this.Tilemap[this.FeatureCoordinates["Bridge"].Y, this.FeatureCoordinates["Bridge"].X] = OverworldTiles.DOCK_W;
		return this.NextStep();
	    }
	    return new Result(false);
	}

	public Result PlacePravoka() {
	    if (this.bridgedRegion != -1) {
		return this.CoastalPlacement(OverworldTiles.PRAVOKA_CITY, this.Traversable_regionlist[this.bridgedRegion], true, false);
	    } else {
		return this.CoastalPlacement(OverworldTiles.PRAVOKA_CITY_MOAT, this.StartingRegion, true, false);
	    }
	}

	public Result PlaceOnCoast(OwFeature feature, bool eastOnly) {
	    var tasks = new List<GenerationTask>();
	    foreach (var w in this.Traversable_regionlist) {
		if (w.RegionType == OverworldTiles.LAND) {
		    if (w.Adjacent.Contains(OverworldTiles.MainOceanRegionId)) {
			tasks.Add(() => new OverworldState(this).CoastalPlacement(feature, w, false, eastOnly));
		    }
		}
	    }
	    tasks.Shuffle(this.rng);
	    return new Result(tasks);
	}

	public Result CoastalPlacement(OwFeature feature, OwRegion region, bool fill, bool eastOnly) {
	    var points = new List<SCCoords>(region.Points);
	    points.Shuffle(this.rng);
	    int w = feature.Tiles.GetLength(1);
	    int h = feature.Tiles.GetLength(0);
	    foreach (var p in points) {
		int makeland = 0;
		if (!eastOnly && this.Traversable_regionmap[p.Y+h-1, p.X-1] == OverworldTiles.MainOceanRegionId) {
		    makeland = 0;
		} else if (this.Traversable_regionmap[p.Y+h-1, p.X+w] == OverworldTiles.MainOceanRegionId) {
		    makeland = w-1;
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
		return this.NextStep();
	    }
	    return new Result(false);
	}

	public ValueTuple<bool, SCCoords> CheckAirshipAccess(OwRegion region) {
	    foreach (var p in region.Points) {
		if (this.Tilemap[p.Y, p.X] == OverworldTiles.LAND ||
		    this.Tilemap[p.Y, p.X] == OverworldTiles.GRASS)
		{
		    return ValueTuple.Create(true, p);
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

	public Result PlaceIsolated(OwFeature feature, bool requireAirshipAccess) {
	    var tasks = new List<GenerationTask>();
	    foreach (var w in this.Traversable_regionlist) {
		if (w.RegionType == OverworldTiles.LAND_REGION) {
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
			    tasks.Add(() => new OverworldState(this).IsolatedPlacement(feature, w));
			}
		    }
		}
	    }
	    tasks.Shuffle(this.rng);
	    return new Result(tasks);
	}

	public Result IsolatedPlacement(OwFeature feature, OwRegion region) {
	    var v = this.PlaceFeature(this.Traversable_regionmap, region, feature);
	    if (v.Item1) {
		this.OwnPlacements();
		var exclude = new List<int>();
		this.FindAllReachableByRiver(region, exclude);
		foreach (var e in exclude) {
		    if (!this.Exclude_docks.Contains(e)) {
			this.Exclude_docks.Add(e);
		    }
		}
		return this.NextStep();
	    } else {
		return new Result(false);
	    }
	}

	public Result PlaceRequiringCanoe(OwFeature feature) {
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
	    return new Result(tasks);
	}

	public Result RequiresCanoePlacement(OwFeature feature, OwRegion region) {
	    var v = this.PlaceFeature(this.Traversable_regionmap, region, feature);
	    if (v.Item1) {
		this.OwnPlacements();
		var exclude = new List<int>();
		if (!this.Exclude_docks.Contains(region.RegionId)) {
		    this.Exclude_docks.Add(region.RegionId);
		}
		return this.NextStep();
	    } else {
		return new Result(false);
	    }
	}

	public Result PlaceInBiome(OwFeature feature, int[] biomes,
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
		if (biomelist.Contains(w.RegionType)) {
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
	    foreach (var w in walkable) {
		tasks.Add(() => new OverworldState(this).BiomePlacement(feature, w, shipReachable));
	    }
	    foreach (var w in islands) {
		tasks.Add(() => new OverworldState(this).BiomePlacement(feature, w, shipReachable));
	    }
	    return new Result(tasks);
	}

	public Result BiomePlacement(OwFeature feature, OwRegion region, bool shipReachable) {
	    var trav = this.Traversable_regionlist[this.Traversable_regionmap[region.Points[0].Y, region.Points[0].X]];
	    var v = this.PlaceFeature(this.Biome_regionmap, region, feature);
	    if (!v.Item1) {
		return new Result(false);
	    }
	    if (shipReachable) {
		var d = this.DockPlacement(feature, trav);
		if (!this.Reachable_regions.Contains(trav.RegionId)) {
		    return new Result(false);
		}
	    }

	    return this.NextStep();
	}

	public bool DockPlacement(OwFeature feature, OwRegion region) {
	    var points = new List<SCCoords>(region.Points);
	    points.Shuffle(this.rng);
	    bool placed = false;
	    foreach (var p in points) {
		var np = p.OwUp;
		if (this.Tilemap[np.Y, np.X] == OverworldTiles.OCEAN) {
		    var r = this.PlaceFeatureAt(this.Traversable_regionmap, region,
					new SCCoords(p.X-2, p.Y), OverworldTiles.N_DOCK_STRUCTURE);
		    if (r.Item1) {
			placed = true;
			break;
		    }
		}

		var ep = p.OwRight;
		if (this.Tilemap[ep.Y, ep.X] == OverworldTiles.OCEAN) {
		    var r = this.PlaceFeatureAt(this.Traversable_regionmap, region,
					new SCCoords(p.X-3, p.Y-2), OverworldTiles.E_DOCK_STRUCTURE);
		    if (r.Item1) {
			placed = true;
			break;
		    }
		}

		var sp = p.OwDown;
		if (this.Tilemap[sp.Y, sp.X] == OverworldTiles.OCEAN) {
		    var r = this.PlaceFeatureAt(this.Traversable_regionmap, region,
					new SCCoords(p.X-2, p.Y-2), OverworldTiles.S_DOCK_STRUCTURE);
		    if (r.Item1) {
			placed = true;
			break;
		    }
		}

		var wp = p.OwLeft;
		if (this.Tilemap[wp.Y, wp.X] == OverworldTiles.OCEAN) {
		    var r = this.PlaceFeatureAt(this.Traversable_regionmap, region,
					new SCCoords(p.X, p.Y-2), OverworldTiles.W_DOCK_STRUCTURE);
		    if (r.Item1) {
			placed = true;
			break;
		    }
		}

	    }

	    if (placed) {
		this.OwnPlacements();
		if (!this.Reachable_regions.Contains(region.RegionId)) {
		    this.Reachable_regions.Add(region.RegionId);
		}
		return true;
	    }
	    return false;
	}

	public Result PlaceInTitanWestRegion(OwFeature feature) {
	    var p = this.FeatureCoordinates["TitansTunnelWest"];
	    var region = this.Traversable_regionlist[this.Traversable_regionmap[p.Y+1, p.X]];
	    var r = this.PlaceFeature(this.Traversable_regionmap, region, feature);
	    if (r.Item1) {
		return this.NextStep();
	    } else {
		return new Result(false);
	    }
	}

	public Result PlaceWaterfall(OwFeature feature) {
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
	    return new Result(tasks);
	}

	public Result WaterfallPlacement(OwFeature feature, OwRegion region) {
	    var points = new List<SCCoords>(region.Points);
	    points.Shuffle(this.rng);
	    foreach (var p in points) {
		if (this.Tilemap[p.Y, p.X-1] == OverworldTiles.MOUNTAIN &&
		    this.Tilemap[p.Y, p.X+1] == OverworldTiles.MOUNTAIN &&
		    this.Tilemap[p.Y-1, p.X] == OverworldTiles.RIVER &&
		    this.Tilemap[p.Y+1, p.X] == OverworldTiles.RIVER)
		{
		    this.PlaceFeatureAt(this.Traversable_regionmap, region, new SCCoords(p.X-1, p.Y-1), feature, false);
		    return this.NextStep();
		}
	    }
	    return new Result(false);
	}

	public Result PlaceInMountains(OwFeature feature) {
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
	    return new Result(tasks);
	}

	public Result MountainRiverPlacement(OwFeature feature, OwRegion mtnRegion, OwRegion riverRegion) {
	    var points = new List<SCCoords>(mtnRegion.Points);
	    points.Shuffle(this.rng);
	    int w = feature.Tiles.GetLength(1);
	    int h = feature.Tiles.GetLength(0);
	    foreach (var p in points) {
		if (this.Traversable_regionmap[p.Y+h, p.X+(w/2)] == riverRegion.RegionId) {
		    var v = this.PlaceFeatureAt(this.Traversable_regionmap, mtnRegion, p, feature, true);
		    if (v.Item1) {
			return this.NextStep();
		    }
		}
	    }
	    return new Result(false);
	}

    }
}
