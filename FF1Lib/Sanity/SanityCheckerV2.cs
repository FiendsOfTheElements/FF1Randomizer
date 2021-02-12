using FF1Lib.Sanity;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
		bool slabTranslated;
		bool herbCheckedIn;
		bool princessRescued;
		bool vampireAccessible;
		bool airShipLiftOff;

		HashSet<short> processedAreas;
		List<IRewardSource> rewardSources;
		AccessRequirement requirements;
		MapChange changes;
		HashSet<short> immediateAreas;
		HashSet<SCDeferredAreaQueueEntry> deferredAreas;
		Queue<SCPointOfInterest> deferredPointOfInterests;

		Dictionary<byte, TreasureChest> chests;
		Dictionary<ObjectId, MapObject> npcs;
		ItemShopSlot shopslot;

		int maxqueue = 0;
		public SanityCheckerV2(List<Map> _maps, OverworldMap _overworldMap, NPCdata _npcdata, FF1Rom _rom)
		{
			start = new SCCoords(0x99, 0xA5);
			ship = new SCCoords(0xD2, 0x99);
			airShip = new SCCoords(0xDD, 0xED);

			main = new SCMain(_maps, _overworldMap, _npcdata, _rom);
		}

		public List<MapLocation> AccessibleMapLocations(AccessRequirement currentAccess, MapChange currentMapChanges, Dictionary<MapLocation, Tuple<List<MapChange>, AccessRequirement>> fullLocationRequirements)
		{
			throw new NotImplementedException();
		}

		public (bool Complete, List<MapLocation> MapLocations, AccessRequirement Requirements) CheckSanity(List<IRewardSource> treasurePlacements, Dictionary<MapLocation, Tuple<List<MapChange>, AccessRequirement>> fullLocationRequirements, IVictoryConditionFlags victoryConditions)
		{
			//kids, don't try this at home. Calculating an index from an address is usually not the way to go.
			chests = treasurePlacements.Select(r => r as TreasureChest).Where(r => r != null).ToDictionary(r => (byte)(r.Address - 0x3100));
			npcs = treasurePlacements.Select(r => r as MapObject).Where(r => r != null).ToDictionary(r => r.ObjectId);
			shopslot = (ItemShopSlot)treasurePlacements.FirstOrDefault(r => r is ItemShopSlot);

			Crawl(victoryConditions);

			return (false, null, AccessRequirement.None);
		}

		public bool IsRewardSourceAccessible(IRewardSource source, AccessRequirement currentAccess, List<MapLocation> locations)
		{
			throw new NotImplementedException();
		}


		public (IEnumerable<IRewardSource> rewardSources, AccessRequirement requirements, MapChange changes) Crawl(IVictoryConditionFlags victoryConditions)
		{
			Stopwatch w = Stopwatch.StartNew();

			immediateAreas = new HashSet<short>(256);
			processedAreas = new HashSet<short>(2048);
			deferredAreas = new HashSet<SCDeferredAreaQueueEntry>(64, new SCDeferredAreaQueueEntryEqualityComparer());
			deferredPointOfInterests = new Queue<SCPointOfInterest>();

			rewardSources = new List<IRewardSource>();
			shipDockAreaIndex = -1;
			slabTranslated = false;
			herbCheckedIn = false;
			princessRescued = false;
			vampireAccessible = false;

			BuildInitialRequirements(victoryConditions);

			SetShipDock();
			SetAirShipPoi();

			short areaIndex = main.Overworld.Tiles[start.X, start.Y].Area;
			SCOwArea area = main.Overworld.Areas[areaIndex];

			CrawlOwArea(area);

			ProcessImmediateAreas();

			while (ProcessDeferredAreas())
			{
				ProcessImmediateAreas();
			}

			//don't know why, but this helps
			for(int i = 0; i < 10; i++) ProcessDeferredPointsOfInterest();

			w.Stop();

			if (changes != MapChange.All || requirements != AccessRequirement.All)
			{
				vampireAccessible = false;
			}

			return (rewardSources, requirements, changes);
		}

		private bool ProcessDeferredAreas()
		{
			for (int i = 0; i < 10; i++) while (ProcessDeferredPointsOfInterest()) ;

			//default is link from 0 to 0, which is illegal anyway, so it cannot appear in the HashSet
			var entry = deferredAreas.FirstOrDefault(e => CheckDeferredArea(e));

			return deferredAreas.Remove(entry);
		}

		private bool ProcessDeferredPointsOfInterest()
		{
			var dcount = deferredPointOfInterests.Count;
			for(int i = 0; i< dcount;i++)
			{
				var poi = deferredPointOfInterests.Dequeue();

				if (poi.Requirements.IsAccessible(requirements))
				{
					ProcessSmPointOfInterest(poi);
				}
				else
				{
					deferredPointOfInterests.Enqueue(poi);
				}
			}

			return dcount < deferredPointOfInterests.Count;
		}

		private bool CheckDeferredArea(SCDeferredAreaQueueEntry e)
		{
			//remove already processed area, does an unneccesary while spin, but it's the easiest
			if (processedAreas.Contains(e.Target)) return true;

			var area = main.Overworld.Areas[e.Source];
			return CheckLink(area, e.Target);
		}

		private void ProcessImmediateAreas()
		{
			while (immediateAreas.Count > 0)
			{
				maxqueue = Math.Max(maxqueue, immediateAreas.Count);

				var entry = immediateAreas.First();
				immediateAreas.Remove(entry);

				if (!processedAreas.Contains(entry))
				{
					SCOwArea nextArea = main.Overworld.Areas[entry];
					CrawlOwArea(nextArea);
				}
			}

			//not sure
			if (airShipLocationAccessible && (changes & MapChange.Airship) > 0 && !airShipLiftOff)
			{
				LiftOff();
			}
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
				if(!CheckLink(area, link)) deferredAreas.Add(new SCDeferredAreaQueueEntry(area.Index, link));
			}

			if (airShipLocationAccessible && (changes & MapChange.Airship) > 0 && !airShipLiftOff)
			{
				LiftOff();
			}
		}

		private void LiftOff()
		{
			airShipLiftOff = true;
			foreach (var area in main.Overworld.Areas.Values)
			{
				if (!processedAreas.Contains(area.Index) && (area.Tile & SCBitFlags.AirDock) > 0) immediateAreas.Add(area.Index);
			}
		}

		private bool CheckLink(SCOwArea area, short link)
		{
			if (processedAreas.Contains(link)) return true;

			SCOwArea linked = main.Overworld.Areas[link];

			if (linked.Tile == SCBitFlags.Bridge)
			{
				return Cross(area, linked, true, (changes & MapChange.Bridge) > 0);
			}
			else if (linked.Tile == SCBitFlags.Canal)
			{
				return Cross(area, linked, (changes & MapChange.Canal) > 0, true);
			}
			else
			{
				return Enter(area, linked);
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

		private void ProcessEnterTele(SCOwArea area, SCPointOfInterest poi)
		{
			var dungeon = main.Dungeons.First(d => d.OverworldTeleport == poi.Teleport.OverworldTeleport);

			foreach(var dpoi in dungeon.PointsOfInterest)
			{
				if (dpoi.Requirements.IsAccessible(requirements))
				{
					ProcessSmPointOfInterest(dpoi);
				}
				else if (dpoi.Type == SCPointOfInterestType.Treasure || dpoi.Type == SCPointOfInterestType.Shop || dpoi.Type == SCPointOfInterestType.Orb || dpoi.Type == SCPointOfInterestType.QuestNpc || dpoi.Type == SCPointOfInterestType.Exit)
				{
					deferredPointOfInterests.Enqueue(dpoi);
				}
			}
		}

		private void ProcessSmPointOfInterest(SCPointOfInterest poi)
		{

			if (poi.Type == SCPointOfInterestType.Treasure)
			{
				ProcessTreasure(poi);
			}
			else if (poi.Type == SCPointOfInterestType.Shop)
			{
				ProcessShop(poi);
			}
			else if (poi.Type == SCPointOfInterestType.Orb)
			{
				ProcessOrb(poi);
			}
			else if (poi.Type == SCPointOfInterestType.QuestNpc)
			{
				if (!ProcessQuestNpc(poi))
				{
					deferredPointOfInterests.Enqueue(poi);
				}
			}
			else if (poi.Type == SCPointOfInterestType.Exit)
			{
				var area = main.Overworld.Tiles[poi.Teleport.TargetCoords.X, poi.Teleport.TargetCoords.Y].Area;
				if (!processedAreas.Contains(area)) immediateAreas.Add(area);
			}
		}

		private bool Cross(SCOwArea area, SCOwArea linked, bool swim, bool walk)
		{
			if ((area.Tile & SCBitFlags.Ocean) > 0 && swim)
			{
				foreach (var nextLink in linked.Links)
				{
					var nextArea = main.Overworld.Areas[nextLink];
					if (nextArea.Index != area.Index && (nextArea.Tile & SCBitFlags.Ocean) > 0)
					{
						immediateAreas.Add(nextArea.Index);
						return true;
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
						immediateAreas.Add(nextArea.Index);
						return true;
					}
				}
			}

			return false;
		}

		private bool Enter(SCOwArea area, SCOwArea linked)
		{
			if ((linked.Tile & SCBitFlags.Chime) > 0 && (changes & MapChange.Chime) == 0)
			{
				return false;
			}

			if ((linked.Tile & SCBitFlags.River) > 0 && (changes & MapChange.Canoe) > 0)
			{
				immediateAreas.Add(linked.Index);
				return true;
			}
			else if ((linked.Tile & SCBitFlags.Land) > 0)
			{
				immediateAreas.Add(linked.Index);
				return true;
			}
			else if ((linked.Tile & SCBitFlags.Ocean) > 0 && (changes & MapChange.Ship) > 0 && area.Index == shipDockAreaIndex)
			{
				immediateAreas.Add(linked.Index);
				return true;
			}

			return false;
		}

		private void SetAirShipPoi()
		{
			var areaIndex = main.Overworld.Tiles[airShip.X, airShip.Y].Area;
			var area = main.Overworld.Areas[areaIndex];
			area.PointsOfInterest.Add(new SCPointOfInterest { Coords = airShip, Type = SCPointOfInterestType.AirShip });
		}

		private void SetShipDock()
		{
			SetShipDock(ship.OwLeft);
			SetShipDock(ship.OwRight);
			SetShipDock(ship.OwUp);
			SetShipDock(ship.OwDown);
		}

		private void SetShipDock(SCCoords coords)
		{
			if ((main.Overworld.Tiles[coords.X, coords.Y].Tile & SCBitFlags.ShipDock) > 0) shipDockAreaIndex = main.Overworld.Tiles[coords.X, coords.Y].Area;
		}

		private void ProcessShop(SCPointOfInterest poi)
		{
			if (poi.ShopId == shopslot.ShopIndex)
			{
				ProcessItem(shopslot.Item);
			}
		}

		private void ProcessTreasure(SCPointOfInterest poi)
		{
			if (chests.TryGetValue(poi.TreasureId, out var chest))
			{
				ProcessItem(chest.Item);
			}
		}

		private void ProcessOrb(SCPointOfInterest poi)
		{
			switch (poi.ItemId)
			{
				case Item.EarthOrb:
					requirements |= AccessRequirement.EarthOrb;
					break;
				case Item.FireOrb:
					requirements |= AccessRequirement.FireOrb;
					break;
				case Item.WaterOrb:
					requirements |= AccessRequirement.WaterOrb;
					break;
				case Item.AirOrb:
					requirements |= AccessRequirement.AirOrb;
					break;
			}
		}

		private bool ProcessQuestNpc(SCPointOfInterest poi)
		{
			if (poi.Npc.ObjectId == ObjectId.Princess1)
			{
				princessRescued = true;
				return true;
			}
			else if (poi.Npc.ObjectId == ObjectId.Vampire)
			{
				vampireAccessible = true;
				return true;
			}
			else if (poi.TalkRoutine == newTalkRoutines.Talk_ElfDocUnne)
			{
				if (poi.TalkArray[(int)TalkArrayPos.requirement_id] == (byte)Item.Herb)
				{
					if (requirements.HasFlag(AccessRequirement.Herb))
					{
						herbCheckedIn = true;
						return true;
					}
				}
				else if (poi.TalkArray[(int)TalkArrayPos.requirement_id] == (byte)Item.Slab)
				{
					if (requirements.HasFlag(AccessRequirement.Slab))
					{
						slabTranslated = true;
						return true;
					}
				}
			}
			else if (npcs.TryGetValue(poi.Npc.ObjectId, out var npc))
			{
				switch (poi.TalkRoutine)
				{
					case newTalkRoutines.Talk_Bikke:
						ProcessItem(npc.Item);
						return true;
					case newTalkRoutines.Talk_GiveItemOnFlag:
						return ProcessItemOnFlag(poi, npc);
					case newTalkRoutines.Talk_Nerrick:
					case newTalkRoutines.Talk_TradeItems:
					case newTalkRoutines.Talk_GiveItemOnItem:
					case newTalkRoutines.Talk_Astos:
						if (requirements.HasFlag(npc.AccessRequirement))
						{
							ProcessItem(npc.Item);
							return true;
						}
						break;
					default:
						return true;
				}
			}

			return false;
		}

		private bool ProcessItemOnFlag(SCPointOfInterest poi, MapObject npc)
		{
			var flag = (ObjectId)poi.TalkArray[(int)TalkArrayPos.requirement_id];

			if (flag == ObjectId.Unne)
			{
				if (slabTranslated)
				{
					ProcessItem(npc.Item);
					return true;
				}
			}
			else if (flag == ObjectId.ElfDoc)
			{
				if (herbCheckedIn)
				{
					ProcessItem(npc.Item);
					return true;
				}
			}
			else if (flag == ObjectId.Princess1)
			{
				if (princessRescued)
				{
					ProcessItem(npc.Item);
					return true;
				}
			}
			else if (flag == ObjectId.Vampire)
			{
				if (vampireAccessible)
				{
					ProcessItem(npc.Item);
					return true;
				}
			}
			else if (flag == ObjectId.None)
			{
				ProcessItem(npc.Item);
				return true;
			}

			return false;
		}

		private void ProcessItem(Item item)
		{
			switch (item)
			{
				case Item.Lute:
					requirements |= AccessRequirement.Lute;
					break;
				case Item.Crown:
					requirements |= AccessRequirement.Crown;
					break;
				case Item.Crystal:
					requirements |= AccessRequirement.Crystal;
					break;
				case Item.Herb:
					requirements |= AccessRequirement.Herb;
					break;
				case Item.Key:
					requirements |= AccessRequirement.Key;
					break;
				case Item.Tnt:
					requirements |= AccessRequirement.Tnt;
					break;
				case Item.Adamant:
					requirements |= AccessRequirement.Adamant;
					break;
				case Item.Slab:
					requirements |= AccessRequirement.Slab;
					break;
				case Item.Ruby:
					requirements |= AccessRequirement.Ruby;
					changes |= MapChange.TitanFed;
					break;
				case Item.Rod:
					requirements |= AccessRequirement.Rod;
					break;
				case Item.Floater:
					changes |= MapChange.Airship;
					break;
				case Item.Chime:
					changes |= MapChange.Chime;
					break;
				case Item.Cube:
					requirements |= AccessRequirement.Cube;
					break;
				case Item.Bottle:
					requirements |= AccessRequirement.Bottle;
					break;
				case Item.Oxyale:
					requirements |= AccessRequirement.Oxyale;
					break;
				case Item.Ship:
					changes |= MapChange.Ship;
					break;
				case Item.Bridge:
					changes |= MapChange.Bridge;
					break;
				case Item.Canal:
					changes |= MapChange.Canal;
					break;
				case Item.Canoe:
					changes |= MapChange.Canoe;
					break;
			}
		}

		private void BuildInitialRequirements(IVictoryConditionFlags victoryConditions)
		{
			airShipLocationAccessible = false;

			requirements = AccessRequirement.None;
			if ((bool)victoryConditions.FreeLute)
			{
				requirements |= AccessRequirement.Lute;
			}

			changes = MapChange.None;
			if (victoryConditions.FreeBridge ?? false)
			{
				changes |= MapChange.Bridge;
			}
			if (victoryConditions.FreeShip ?? false)
			{
				changes |= MapChange.Ship;
			}
			if (victoryConditions.FreeAirship ?? false)
			{
				changes |= MapChange.Airship;
				airShipLocationAccessible = true;
			}
			if (victoryConditions.FreeCanal ?? false)
			{
				changes |= MapChange.Canal;
			}
			if (victoryConditions.FreeCanoe ?? false)
			{
				changes |= MapChange.Canoe;
			}
		}
	}
}
