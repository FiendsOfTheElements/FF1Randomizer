namespace FF1Lib
{
	public interface ISanityChecker
	{
		bool IsRewardSourceAccessible(IRewardSource source, AccessRequirement currentAccess, List<MapLocation> locations);

		List<MapLocation> AccessibleMapLocations(
										AccessRequirement currentAccess,
										MapChange currentMapChanges,
										Dictionary<MapLocation, Tuple<List<MapChange>, AccessRequirement>> fullLocationRequirements);

		(bool Complete, List<MapLocation> MapLocations, AccessRequirement Requirements) CheckSanity(List<IRewardSource> treasurePlacements,
										Dictionary<MapLocation, Tuple<List<MapChange>, AccessRequirement>> fullLocationRequirements,
										IVictoryConditionFlags victoryConditions, bool layoutcheck);

		IEnumerable<IRewardSource> GetNearRewardSources(IEnumerable<IRewardSource> sources, IRewardSource source);
	}
}
