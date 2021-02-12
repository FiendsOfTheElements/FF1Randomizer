using FF1Lib.Sanity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static FF1Lib.FF1Rom;

namespace FF1Lib
{
	public class SanityCheckerV2 : ISanityChecker
	{
		SCMain main;

		SCCoords start;
		SCCoords ship;
		SCCoords airShip;
		short shipDockAreaIndex;
		bool airShipLocationAccessible;

		HashSet<short> processedAreas;
		List<IRewardSource> rewardSources;
		AccessRequirement requirements;
		MapChange changes;
		Queue<short> queue;

		public SanityCheckerV2(List<Map> _maps, OverworldMap _overworldMap, NPCdata _npcdata, FF1Rom _rom)
		{
			main = new SCMain(_maps, _overworldMap, _npcdata, _rom);

			start = new SCCoords(0x99, 0xA5);
			ship = new SCCoords(0xD2, 0x99);
			airShip = new SCCoords(0xDD, 0xED);
		}

		public List<MapLocation> AccessibleMapLocations(AccessRequirement currentAccess, MapChange currentMapChanges, Dictionary<MapLocation, Tuple<List<MapChange>, AccessRequirement>> fullLocationRequirements)
		{
			throw new NotImplementedException();
		}

		public (bool Complete, List<MapLocation> MapLocations, AccessRequirement Requirements) CheckSanity(List<IRewardSource> treasurePlacements, Dictionary<MapLocation, Tuple<List<MapChange>, AccessRequirement>> fullLocationRequirements, IVictoryConditionFlags victoryConditions)
		{
			throw new NotImplementedException();
		}

		public bool IsRewardSourceAccessible(IRewardSource source, AccessRequirement currentAccess, List<MapLocation> locations)
		{
			throw new NotImplementedException();
		}


		private (IEnumerable<IRewardSource> rewardSources, AccessRequirement requirements, MapChange changes) Crawl()
		{
			queue = new Queue<short>(64);
			processedAreas = new HashSet<short>();
			rewardSources = new List<IRewardSource>();
			requirements = AccessRequirement.None;
			changes = MapChange.None;
			shipDockAreaIndex = -1;
			airShipLocationAccessible = false;


			short areaIndex = main.Overworld.Tiles[start.X, start.Y].Area;
			SCOwArea area = main.Overworld.Areas[areaIndex];

			CrawlOwArea(area);

			while (queue.Count > 0)
			{
				var entry = queue.Dequeue();
				if (!processedAreas.Contains(entry))
				{
					SCOwArea nextArea = main.Overworld.Areas[entry];
					CrawlOwArea(nextArea);
				}
			}

			return (rewardSources, requirements, changes);
		}

		private void CrawlOwArea(SCOwArea area)
		{
			processedAreas.Add(area.Index);

			foreach (var poi in area.PointsOfInterest)
			{
				ProcessOwPointOfInterest(area, poi);
			}

			foreach (var link in area.Links)
			{
				CheckLink(area, link);
			}

			if (airShipLocationAccessible && (changes & MapChange.Airship) > 0)
			{
				LiftOff();
			}
		}

		private void LiftOff()
		{
			foreach (var area in main.Overworld.Areas.Values)
			{
				if (!processedAreas.Contains(area.Index) && (area.Tile & SCBitFlags.AirDock) > 0) queue.Enqueue(area.Index);
			}
		}

		private void CheckLink(SCOwArea area, short link)
		{
			SCOwArea linked = main.Overworld.Areas[link];

			if (linked.Tile == SCBitFlags.Bridge)
			{
				short cross = Cross(area, linked, true, (changes & MapChange.Bridge) > 0);
				if (cross >= 0 && !processedAreas.Contains(cross)) queue.Enqueue(cross);
			}
			else if (linked.Tile == SCBitFlags.Canal)
			{
				short cross = Cross(area, linked, (changes & MapChange.Canal) > 0, true);
				queue.Enqueue(linked.Index);
				if (cross >= 0 && !processedAreas.Contains(cross)) queue.Enqueue(cross);
			}
			else if (IsAccessible(area, linked))
			{
				if (!processedAreas.Contains(linked.Index)) queue.Enqueue(linked.Index);
			}
		}

		private void ProcessOwPointOfInterest(SCOwArea area, SCPointOfInterest poi)
		{
			if (poi.Type == SCPointOfInterestType.Shop)
			{
				ProcessShop(poi);
			}
			else if (poi.Type == SCPointOfInterestType.Tele)
			{
				ProcessEnterTele(area, poi);
			}
			else if (poi.Type == SCPointOfInterestType.AirShip)
			{
				airShipLocationAccessible = true;
			}
		}


		private void ProcessShop(SCPointOfInterest poi)
		{
			throw new NotImplementedException();
		}

		private void ProcessEnterTele(SCOwArea area, SCPointOfInterest poi)
		{
			var dungeon = main.Dungeons.First(d => d.OverworldTeleport == poi.Teleport.OverworldTeleport);

			foreach(var dpoi in dungeon.PointsOfInterest)
			{
				ProcessSmPointOfInterest(dpoi);
			}
		}

		private void ProcessSmPointOfInterest(SCPointOfInterest dpoi)
		{

		}

		private short Cross(SCOwArea area, SCOwArea linked, bool swim, bool walk)
		{
			if ((area.Tile & SCBitFlags.Ocean) > 0 && swim)
			{
				foreach (var nextLink in linked.Links)
				{
					var nextArea = main.Overworld.Areas[nextLink];
					if (nextArea.Index != area.Index && (nextArea.Tile & SCBitFlags.Ocean) > 0)
					{
						return nextArea.Index;
					}
				}
			}
			else if((area.Tile & SCBitFlags.Land) > 0 && walk)
			{
				foreach (var nextLink in linked.Links)
				{
					var nextArea = main.Overworld.Areas[nextLink];
					if (nextArea.Index != area.Index && (nextArea.Tile & SCBitFlags.Land) > 0)
					{
						return nextArea.Index;
					}
				}
			}

			return -1;
		}

		private bool IsAccessible(SCOwArea area, SCOwArea linked)
		{
			if ((linked.Tile & SCBitFlags.River) > 0 && (changes & MapChange.Canoe) > 0)
			{
				return true;
			}
			else if ((linked.Tile & SCBitFlags.Land) > 0)
			{
				return true;
			}
			else if ((linked.Tile & SCBitFlags.Ocean) > 0 && (changes & MapChange.Ship) > 0 && area.Index == shipDockAreaIndex)
			{
				return true;
			}

			return false;
		}
	}
}
