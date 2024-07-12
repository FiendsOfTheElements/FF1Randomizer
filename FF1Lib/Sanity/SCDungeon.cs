namespace FF1Lib.Sanity
{
	public class SCDungeon
	{
		public OverworldTeleportIndex OverworldTeleport { get; private set; }

		public List<SCPointOfInterest> PointsOfInterest { get; set; }

		public HashSet<SCArea> Areas { get; private set; } = new HashSet<SCArea>();

		HashSet<SCTeleport> usedEnterTeles;
		Dictionary<MapIndex, SCMap> scmaps;

		Dictionary<SCPointOfInterest, SCPointOfInterest> poiDic = new Dictionary<SCPointOfInterest, SCPointOfInterest>(new SCPointOfInterestIdentityComparer());
		HashSet<SCTeleport> entrances = new HashSet<SCTeleport>();
		HashSet<SCTeleport> exits = new HashSet<SCTeleport>();

		public bool Done { get; set; }

		public SCDungeon(SCTeleport et, OverworldTeleportIndex overworldTeleport, Dictionary<MapIndex, SCMap> _scmaps, HashSet<SCTeleport> _usedEnterTeles)
		{
			scmaps = _scmaps;
			usedEnterTeles = _usedEnterTeles;
			OverworldTeleport = overworldTeleport;

			AddEnterTele(et);

			PointsOfInterest = poiDic.Values.ToList();
		}

		private void AddEnterTele(SCTeleport teleport)
		{
			if (usedEnterTeles.Contains(teleport)) return;
			usedEnterTeles.Add(teleport);

			if (entrances.Contains(teleport)) entrances.Add(teleport);

			AddTele(null, teleport, SCBitFlagSet.NoRequirements, true);
		}

		private void AddTele(SCArea currentArea, SCTeleport teleport, SCBitFlagSet bitFlagSet, bool topfloor)
		{
			var map = scmaps[teleport.TargetMap];
			var entrance = map.Entrances.FirstOrDefault(e => e.Coords == teleport.TargetCoords);
			var area = map.Areas.FirstOrDefault(area => area.Entrances.Contains(entrance));

			if (!Areas.Contains(area))
			{
				Areas.Add(area);

				if (currentArea != null)
				{
					currentArea.ChildAreas.Add(area);
					area.IsRoot = false;
				}
			}

			AddEntrance(area, entrance, bitFlagSet, topfloor);
		}

		private void AddEntrance(SCArea currentArea, SCEntrance entrance, SCBitFlagSet requirements, bool topfloor)
		{
			foreach (var poi in entrance.PointsOfInterest.Where(p => p.BitFlagSet.Count > 0))
			{
				poi.MapIndex = entrance.Map.MapIndex;
				SCPointOfInterest dungeonpoi;
				if (!poiDic.TryGetValue(poi, out dungeonpoi))
				{
					dungeonpoi = poi.Clone();
					dungeonpoi.BitFlagSet = new SCBitFlagSet();
					poiDic.Add(dungeonpoi, dungeonpoi);
				}

				bool changed = dungeonpoi.BitFlagSet.Merge(poi.BitFlagSet, requirements);

				if (changed)
				{
					if (dungeonpoi.Type == SCPointOfInterestType.Tele)
					{
						AddTele(currentArea, dungeonpoi.Teleport, dungeonpoi.BitFlagSet, false);
					}
					else if (dungeonpoi.Type == SCPointOfInterestType.Exit)
					{
						if (!exits.Contains(dungeonpoi.Teleport)) exits.Add(dungeonpoi.Teleport);
					}
					else if (dungeonpoi.Type == SCPointOfInterestType.Warp && topfloor)
					{
						if (!exits.Contains(dungeonpoi.Teleport)) exits.Add(dungeonpoi.Teleport);
					}
				}
			}
		}

		public override string ToString()
		{
			return OverworldTeleport.ToString() + "->" + Areas.First().Map.MapIndex.ToString() + (Done ? " - Done" : "");
		}
	}
}
