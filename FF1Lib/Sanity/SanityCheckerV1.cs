namespace FF1Lib
{
	public class SanityCheckerV1 : ISanityChecker
	{
		public bool IsRewardSourceAccessible(IRewardSource source, AccessRequirement currentAccess, List<MapLocation> locations)
		{
			return locations.Contains(source.MapLocation) && currentAccess.HasFlag(source.AccessRequirement) &&
					locations.Contains((source as NpcReward)?.SecondLocation ?? MapLocation.StartingLocation);
		}

		public List<MapLocation> AccessibleMapLocations(
										AccessRequirement currentAccess,
										MapChange currentMapChanges,
										Dictionary<MapLocation, Tuple<List<MapChange>, AccessRequirement>> fullLocationRequirements)
		{
			return fullLocationRequirements.Where(x => x.Value.Item1.Any(y => currentMapChanges.HasFlag(y) && currentAccess.HasFlag(x.Value.Item2))).Select(x => x.Key).ToList();
		}

		public (bool Complete, List<MapLocation> MapLocations, AccessRequirement Requirements) CheckSanity(List<IRewardSource> treasurePlacements,
										Dictionary<MapLocation, Tuple<List<MapChange>, AccessRequirement>> fullLocationRequirements,
										IVictoryConditionFlags victoryConditions)

		{
			const int maxIterations = 20;
			var currentIteration = 0;
			var currentAccess = AccessRequirement.None;
			if ((bool)victoryConditions.FreeLute)
			{
				currentAccess |= AccessRequirement.Lute;
			}
			if ((bool)victoryConditions.FreeRod)
			{
				currentAccess |= AccessRequirement.Rod;
			}

			var currentMapChanges = MapChange.None;
			if (victoryConditions.IsBridgeFree ?? false)
			{
				currentMapChanges |= MapChange.Bridge;
			}
			if (victoryConditions.IsShipFree ?? false)
			{
				currentMapChanges |= MapChange.Ship;
			}
			if (victoryConditions.IsAirshipFree ?? false)
			{
				currentMapChanges |= MapChange.Airship;
			}
			if (victoryConditions.IsCanalFree ?? false)
			{
				currentMapChanges |= MapChange.Canal;
			}
			if (victoryConditions.IsCanoeFree ?? false)
			{
				currentMapChanges |= MapChange.Canoe;
			}

			IEnumerable<MapLocation> currentMapLocations()
			{
				return fullLocationRequirements.Where(x => x.Value.Item1.Any(y => currentMapChanges.HasFlag(y) && currentAccess.HasFlag(x.Value.Item2))).Select(x => x.Key);
			}
			IEnumerable<IRewardSource> currentItemLocations()
			{
				var locations = currentMapLocations().ToList();
				return treasurePlacements.Where(x =>
				{
					return locations.Contains(x.MapLocation) && currentAccess.HasFlag(x.AccessRequirement) &&
						locations.Contains((x as NpcReward)?.SecondLocation ?? MapLocation.StartingLocation);
				});
			}

			var requiredAccess = AccessRequirement.All;
			var requiredMapChanges = MapChange.All;

			if ((bool)victoryConditions.IsFloaterRemoved) {
			    requiredMapChanges &= ~MapChange.Airship;
			}

			var accessibleLocationCount = 0;
			while (!currentAccess.HasFlag(requiredAccess) ||
				   !currentMapChanges.HasFlag(requiredMapChanges))
			{
				if (currentIteration > maxIterations)
				{
					throw new InvalidOperationException($"Sanity Check hit max iterations: {currentIteration}");
				}

				currentIteration++;
				var accessibleLocations = currentItemLocations().ToList();
				if (accessibleLocations.Count <= accessibleLocationCount)
				{
					return (false, currentMapLocations().ToList(), currentAccess);
				}

				accessibleLocationCount = accessibleLocations.Count;
				var currentItems = accessibleLocations.Select(x => x.Item).ToList();

				if (!currentAccess.HasFlag(AccessRequirement.Key) &&
					currentItems.Contains(Item.Key))
					currentAccess |= AccessRequirement.Key;
				if (!currentMapChanges.HasFlag(MapChange.Bridge) &&
					currentItems.Contains(Item.Bridge))
					currentMapChanges |= MapChange.Bridge;
				if (!currentAccess.HasFlag(AccessRequirement.Crown) &&
					currentItems.Contains(Item.Crown))
					currentAccess |= AccessRequirement.Crown;
				if (!currentAccess.HasFlag(AccessRequirement.Crystal) &&
					currentItems.Contains(Item.Crystal))
					currentAccess |= AccessRequirement.Crystal;
				if (!currentAccess.HasFlag(AccessRequirement.Herb) &&
					currentItems.Contains(Item.Herb))
					currentAccess |= AccessRequirement.Herb;
				if (!currentMapChanges.HasFlag(MapChange.Canoe) &&
					currentItems.Contains(Item.Canoe))
					currentMapChanges |= MapChange.Canoe;
				if (!currentMapChanges.HasFlag(MapChange.Ship) &&
					currentItems.Contains(Item.Ship))
					currentMapChanges |= MapChange.Ship;
				if (!currentAccess.HasFlag(AccessRequirement.Tnt) &&
					currentItems.Contains(Item.Tnt))
					currentAccess |= AccessRequirement.Tnt;
				if (!currentAccess.HasFlag(AccessRequirement.Adamant) &&
					currentItems.Contains(Item.Adamant))
					currentAccess |= AccessRequirement.Adamant;
				if (!currentMapChanges.HasFlag(MapChange.Canal) &&
					currentItems.Contains(Item.Canal) &&
					currentMapChanges.HasFlag(MapChange.Ship))
					currentMapChanges |= MapChange.Canal;
				if (!currentMapChanges.HasFlag(MapChange.TitanFed) &&
					currentItems.Contains(Item.Ruby) &&
					(currentMapLocations().Contains(MapLocation.TitansTunnelEast) ||
					currentMapLocations().Contains(MapLocation.TitansTunnelWest)))
				{
					currentMapChanges |= MapChange.TitanFed;
					currentAccess |= AccessRequirement.Ruby;
				}
				if (!currentAccess.HasFlag(AccessRequirement.Rod) &&
					currentItems.Contains(Item.Rod))
					currentAccess |= AccessRequirement.Rod;
				if (!currentAccess.HasFlag(AccessRequirement.Slab) &&
					currentItems.Contains(Item.Slab))
					currentAccess |= AccessRequirement.Slab;
				if (!currentMapChanges.HasFlag(MapChange.Airship) &&
					(currentItems.Contains(Item.Floater)) &&
					currentMapLocations().Contains(MapLocation.AirshipLocation))
					currentMapChanges |= MapChange.Airship;
				if (!currentAccess.HasFlag(AccessRequirement.Bottle) &&
					currentItems.Contains(Item.Bottle))
					currentAccess |= AccessRequirement.Bottle;
				if (!currentAccess.HasFlag(AccessRequirement.Oxyale) &&
					currentItems.Contains(Item.Oxyale))
					currentAccess |= AccessRequirement.Oxyale;
				if (!currentMapChanges.HasFlag(MapChange.Chime) &&
					currentItems.Contains(Item.Chime))
					currentMapChanges |= MapChange.Chime;
				if (!currentAccess.HasFlag(AccessRequirement.Cube) &&
					currentItems.Contains(Item.Cube))
					currentAccess |= AccessRequirement.Cube;
				if (!currentAccess.HasFlag(AccessRequirement.EarthOrb) &&
					currentItems.Contains(Item.EarthOrb))
					currentAccess |= AccessRequirement.EarthOrb;
				if (!currentAccess.HasFlag(AccessRequirement.FireOrb) &&
					currentItems.Contains(Item.FireOrb))
					currentAccess |= AccessRequirement.FireOrb;
				if (!currentAccess.HasFlag(AccessRequirement.WaterOrb) &&
					currentItems.Contains(Item.WaterOrb))
					currentAccess |= AccessRequirement.WaterOrb;
				if (!currentAccess.HasFlag(AccessRequirement.AirOrb) &&
					currentItems.Contains(Item.AirOrb))
					currentAccess |= AccessRequirement.AirOrb;
				if (!currentAccess.HasFlag(AccessRequirement.Lute) &&
					currentItems.Contains(Item.Lute))
					currentAccess |= AccessRequirement.Lute;
			}

			return (true, currentMapLocations().ToList(), currentAccess);
		}

		public IEnumerable<IRewardSource> GetNearRewardSources(IEnumerable<IRewardSource> sources, IRewardSource source)
		{
			return Array.Empty<IRewardSource>();
		}
	}
}
