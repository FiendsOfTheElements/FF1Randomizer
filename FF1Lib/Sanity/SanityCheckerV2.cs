using FF1Lib.Sanity;
using System.Diagnostics;
using static FF1Lib.FF1Rom;

namespace FF1Lib
{
	public class SanityCheckerV2 : ISanityChecker
	{
		FF1Rom rom;
		StandardMaps maps;
		OverworldMap overworldMap;
		Overworld overworld;
		Teleporters teleporters;
		TileSetsData tileSets;

		NpcObjectData npcdata;

		short shipDockAreaIndex;
		bool airShipLocationAccessible;
		bool slabTranslated;
		bool herbCheckedIn;
		bool princessRescued;
		bool vampireAccessible;
		bool airShipLiftOff;
		bool airBoat;

		HashSet<short> processedAreas;
		HashSet<OverworldTeleportIndex> processedDungeons;
		HashSet<IRewardSource> rewardSources;
		AccessRequirement requirements;
		MapChange changes;
		HashSet<short> immediateAreas;
		HashSet<SCDeferredAreaQueueEntry> deferredAreas;
		Queue<SCPointOfInterest> deferredPointOfInterests;

		Dictionary<byte, TreasureChest> chests;
		Dictionary<ObjectId, NpcReward> npcs;
		ItemShopSlot shopslot;

		List<IRewardSource> treasurePlacements;

		Dictionary<byte, TreasureChest> allTreasures;
		Dictionary<ObjectId, NpcReward> allQuestNpcs;
		ItemShopSlot declaredShopSlot;

		OwLocationData locations;
		//public ShipLocations Shiplocations { get; private set; }

		int maxqueue = 0;

		public SCMain Main { get; private set; }

		public SanityCheckerV2(StandardMaps _maps, Overworld _overworld, NpcObjectData _npcdata, Teleporters _teleporters, TileSetsData _tileSets, FF1Rom _rom, ItemShopSlot _declaredShopSlot)
		{
			rom = _rom;
			overworld = _overworld;
			overworldMap = _overworld.OverworldMap;
			tileSets = _tileSets;
			maps = _maps;
			npcdata = _npcdata;
			teleporters = _teleporters;

			locations = overworld.Locations;
			//locations.LoadData();

			//Shiplocations = _shiplocations;

			allTreasures = ItemLocations.AllTreasures.Select(r => r as TreasureChest).Where(r => r != null).ToDictionary(r => (byte)(r.Address - 0x3100));
			allQuestNpcs = ItemLocations.AllNPCItemLocations.Select(r => r as NpcReward).Where(r => r != null).ToDictionary(r => r.ObjectId);
			declaredShopSlot = _declaredShopSlot;

			UpdateNpcRequirements();

			Main = new SCMain(maps, overworld, _npcdata, tileSets, teleporters, locations, _rom);
		}
		public void SetShipLocation(int dungeonindex)
		{
			overworld.SetShipLocation(dungeonindex);
		}

		private void UpdateNpcRequirements()
		{
			foreach (var npc in allQuestNpcs.Values)
			{
				UpdateNpcRequirements(npc, npcdata[npc.ObjectId]);
			}
		}

		private void UpdateNpcRequirements(NpcReward npc, NpcObject npcdata)
		{
			if (npcdata.Script == TalkScripts.Talk_Nerrick)
			{
				npc.AccessRequirement = AccessRequirement.Tnt;
			}
			else if (npcdata.Script == TalkScripts.Talk_Astos)
			{
				npc.AccessRequirement = AccessRequirement.Crown;
			}
			else if (npcdata.Script == TalkScripts.Talk_TradeItems || npcdata.Script == TalkScripts.Talk_GiveItemOnItem)
			{
				var req = (Item)npcdata.Requirement;

				npc.AccessRequirement = req.ToAccessRequirement();
			}
		}

		public List<MapLocation> AccessibleMapLocations(AccessRequirement currentAccess, MapChange currentMapChanges, Dictionary<MapLocation, Tuple<List<MapChange>, AccessRequirement>> fullLocationRequirements)
		{
			throw new NotSupportedException("not needed?");
		}

		public (bool Complete, List<MapLocation> MapLocations, AccessRequirement Requirements) CheckSanity(List<IRewardSource> _treasurePlacements, Dictionary<MapLocation, Tuple<List<MapChange>, AccessRequirement>> fullLocationRequirements, IVictoryConditionFlags victoryConditions, bool layoutcheck)
		{
			treasurePlacements = _treasurePlacements;

			//kids, don't try this at home. Calculating an index from an address is usually not the way to go.
			chests = treasurePlacements.Select(r => r as TreasureChest).Where(r => r != null).ToDictionary(r => (byte)(r.Address - 0x3100));
			npcs = treasurePlacements.Select(r => r as NpcReward).Where(r => r != null).ToDictionary(r => r.ObjectId);
			shopslot = (ItemShopSlot)treasurePlacements.FirstOrDefault(r => r is ItemShopSlot);

			var result = Crawl(victoryConditions, layoutcheck);

			var mapLocations = result.rewardSources.Select(r => r.MapLocation).Distinct().ToList();

			return (result.complete, mapLocations, result.requirements);
		}

		public bool IsRewardSourceAccessible(IRewardSource source, AccessRequirement currentAccess, List<MapLocation> locations)
		{
			//if (currentAccess != requirements) throw new InvalidOperationException("no can do");

			return rewardSources.Contains(source);
		}

		public (bool complete, IEnumerable<IRewardSource> rewardSources, AccessRequirement requirements, MapChange changes) Crawl(IVictoryConditionFlags victoryConditions, bool layoutcheck)
		{
			Stopwatch w = Stopwatch.StartNew();

			immediateAreas = new HashSet<short>(256);
			processedAreas = new HashSet<short>(2048);
			processedDungeons = new HashSet<OverworldTeleportIndex>(32);
			deferredAreas = new HashSet<SCDeferredAreaQueueEntry>(64, new SCDeferredAreaQueueEntryEqualityComparer());
			deferredPointOfInterests = new Queue<SCPointOfInterest>();

			rewardSources = new HashSet<IRewardSource>(256, new RewardSourceEqualityComparer());
			shipDockAreaIndex = -1;
			slabTranslated = false;
			herbCheckedIn = false;
			princessRescued = false;
			vampireAccessible = false;
			airShipLiftOff = false;
			airBoat = (bool)victoryConditions.AirBoat;

			BuildInitialRequirements(victoryConditions, layoutcheck);

			SetAirShipPoi();

			short areaIndex = Main.Overworld.Tiles[locations.StartingLocation.X, locations.StartingLocation.Y].Area;
			SCOwArea area = Main.Overworld.Areas[areaIndex];

			CrawlOwArea(area);

			ProcessImmediateAreas();

			while (ProcessDeferredAreas())
			{
				ProcessImmediateAreas();
			}

			//that for just means, that there's a fault in the function. Actually the mere existence of the call is a fault  in the above functions
			for (int i = 0; i < 10; i++) ProcessDeferredPointsOfInterest();

			w.Stop();

			var requiredAccess = AccessRequirement.All;
			var requiredMapChanges = MapChange.All;

			if ((bool)victoryConditions.IsFloaterRemoved)
			{
				requiredMapChanges &= ~MapChange.Airship;
			}

			if (victoryConditions.GameMode == GameModes.DeepDungeon)
			{
				requiredMapChanges = MapChange.None;
				requiredAccess = AccessRequirement.Tnt | AccessRequirement.Ruby | AccessRequirement.Oxyale;
			}

			bool complete = changes.HasFlag(requiredMapChanges) && requirements.HasFlag(requiredAccess);

			return (complete, rewardSources, requirements, changes);
		}

		private bool ProcessDeferredAreas()
		{
			//that for just means, that there's a fault in the function
			for (int i = 0; i < 10; i++) while (ProcessDeferredPointsOfInterest()) ;

			foreach (var entry in deferredAreas)
			{
				if (CheckDeferredArea(entry))
				{
					return deferredAreas.Remove(entry);
				}
			}

			return false;
		}

		private bool ProcessDeferredPointsOfInterest()
		{
			//thats a problem...
			deferredPointOfInterests = new Queue<SCPointOfInterest>(deferredPointOfInterests.Distinct(new SCPointOfInterestIdentityComparer()));
			var dcount = deferredPointOfInterests.Count;
			for(int i = 0; i< dcount;i++)
			{
				var poi = deferredPointOfInterests.Dequeue();

				if (poi.BitFlagSet.IsAccessible(requirements, changes))
				{
					ProcessSmPointOfInterest(poi, (byte)poi.DungeonIndex);
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

			var area = Main.Overworld.Areas[e.Source];
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
					SCOwArea nextArea = Main.Overworld.Areas[entry];
					CrawlOwArea(nextArea);
				}
			}

			//not sure, but helps
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
			foreach (var area in Main.Overworld.Areas.Values)
			{
				if (!processedAreas.Contains(area.Index) && (area.Tile & SCBitFlags.AirDock) > 0) immediateAreas.Add(area.Index);
			}
		}

		private bool CheckLink(SCOwArea area, short link)
		{
			if (processedAreas.Contains(link)) return true;

			SCOwArea linked = Main.Overworld.Areas[link];

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
				ProcessShop(poi, 255);
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
			if (processedDungeons.Contains(poi.Teleport.OverworldTeleport)) return;
			processedDungeons.Add(poi.Teleport.OverworldTeleport);

			var dungeon = Main.Dungeons.First(d => d.OverworldTeleport == poi.Teleport.OverworldTeleport);
			dungeon.Done = true;

			foreach (var dpoi in dungeon.PointsOfInterest)
			{
				if (dpoi.BitFlagSet.IsAccessible(requirements, changes))
				{
					ProcessSmPointOfInterest(dpoi, (byte)poi.Teleport.OverworldTeleport);
				}
				else if (dpoi.Type == SCPointOfInterestType.Treasure || dpoi.Type == SCPointOfInterestType.Shop || dpoi.Type == SCPointOfInterestType.Orb || dpoi.Type == SCPointOfInterestType.QuestNpc || dpoi.Type == SCPointOfInterestType.Exit)
				{
					dpoi.DungeonIndex = (byte)poi.Teleport.OverworldTeleport;
					deferredPointOfInterests.Enqueue(dpoi);
				}
			}
		}

		private void ProcessSmPointOfInterest(SCPointOfInterest poi, byte dungeonIndex)
		{

			if (poi.Type == SCPointOfInterestType.Treasure)
			{
				ProcessTreasure(poi, dungeonIndex);
			}
			else if (poi.Type == SCPointOfInterestType.Shop)
			{
				ProcessShop(poi, dungeonIndex);
			}
			else if (poi.Type == SCPointOfInterestType.Orb)
			{
				ProcessOrb(poi);
			}
			else if (poi.Type == SCPointOfInterestType.QuestNpc)
			{
				if (!ProcessQuestNpc(poi, dungeonIndex))
				{
					poi.DungeonIndex = dungeonIndex;
					deferredPointOfInterests.Enqueue(poi);
				}
			}
			else if (poi.Type == SCPointOfInterestType.Exit)
			{
				var area = Main.Overworld.Tiles[poi.Teleport.TargetCoords.X, poi.Teleport.TargetCoords.Y].Area;
				if (!processedAreas.Contains(area)) immediateAreas.Add(area);
			}
		}

		private bool Cross(SCOwArea area, SCOwArea linked, bool swim, bool walk)
		{
			if ((area.Tile & SCBitFlags.Ocean) > 0 && swim)
			{
				foreach (var nextLink in linked.Links)
				{
					var nextArea = Main.Overworld.Areas[nextLink];
					if (nextArea.Index != area.Index && (nextArea.Tile & SCBitFlags.Ocean) > 0)
					{
						if (!processedAreas.Contains(nextArea.Index)) immediateAreas.Add(nextArea.Index);
						return true;
					}
				}
			}
			else if((area.Tile & SCBitFlags.Land) > 0 && walk)
			{
				foreach (var nextLink in linked.Links)
				{
					var nextArea = Main.Overworld.Areas[nextLink];
					if (nextArea.Index != area.Index && (nextArea.Tile & SCBitFlags.Land) > 0)
					{
						if (!processedAreas.Contains(nextArea.Index)) immediateAreas.Add(nextArea.Index);
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
				if (!processedAreas.Contains(linked.Index)) immediateAreas.Add(linked.Index);
				return true;
			}
			else if ((linked.Tile & SCBitFlags.Land) > 0)
			{
				if (!processedAreas.Contains(linked.Index)) immediateAreas.Add(linked.Index);
				return true;
			}
			else if ((linked.Tile & SCBitFlags.Ocean) > 0 && (changes & MapChange.Ship) > 0 && area.Index == shipDockAreaIndex)
			{
				if (!processedAreas.Contains(linked.Index)) immediateAreas.Add(linked.Index);
				return true;
			}

			return false;
		}

		private void SetAirShipPoi()
		{
			var areaIndex = Main.Overworld.Tiles[locations.AirShipLocation.X, locations.AirShipLocation.Y].Area;
			var area = Main.Overworld.Areas[areaIndex];
			area.PointsOfInterest.Add(new SCPointOfInterest { Coords = locations.AirShipLocation, Type = SCPointOfInterestType.AirShip });
		}

		private void SetShipDock()
		{
			var coords = overworld.Locations.ShipLocation;

			SetShipDock(coords.OwLeft);
			SetShipDock(coords.OwRight);
			SetShipDock(coords.OwUp);
			SetShipDock(coords.OwDown);

			//if dock area already processed, try to reenter the ocean
			if (processedAreas.Contains(shipDockAreaIndex))
			{
				var area = Main.Overworld.Areas[shipDockAreaIndex];
				foreach (var link in area.Links)
				{
					CheckLink(area, link);
				}
			}
		}

		private void SetShipDock(SCCoords coords)
		{
			if ((Main.Overworld.Tiles[coords.X, coords.Y].Tile & SCBitFlags.ShipDock) > 0) shipDockAreaIndex = Main.Overworld.Tiles[coords.X, coords.Y].Area;
		}

		private void ProcessShop(SCPointOfInterest poi, byte dungeonIndex)
		{
			if (shopslot != null && shopslot.ShopIndex == poi.ShopId - 1)
			{
				ProcessItem(shopslot.Item, dungeonIndex);
				shopslot.Entrance = (OverworldTeleportIndex)dungeonIndex;
				rewardSources.Add(shopslot);
			}
			else if (declaredShopSlot.ShopIndex == poi.ShopId - 1)
			{
				ProcessItem(declaredShopSlot.Item, dungeonIndex);
				declaredShopSlot.Entrance = (OverworldTeleportIndex)dungeonIndex;
				rewardSources.Add(declaredShopSlot);
				poi.Done = true;
			}
		}

		private void ProcessTreasure(SCPointOfInterest poi, byte dungeonIndex)
		{
			if (chests.TryGetValue(poi.TreasureId, out var chest))
			{
				ProcessItem(chest.Item, dungeonIndex);
				chest.Entrance = (OverworldTeleportIndex)dungeonIndex;
				rewardSources.Add(chest);
				poi.Done = true;
			}
			else if (allTreasures.TryGetValue(poi.TreasureId, out var chest1))
			{
				chest1.Entrance = (OverworldTeleportIndex)dungeonIndex;
				rewardSources.Add(chest1);
				poi.Done = true;
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

			poi.Done = true;
		}

		private bool ProcessQuestNpc(SCPointOfInterest poi, byte dungeonIndex)
		{
			if (poi.Npc.ObjectId == ObjectId.Princess1)
			{
				princessRescued = true;
				poi.Done = true;
				return true;
			}
			else if (poi.Npc.ObjectId == ObjectId.Vampire)
			{
				vampireAccessible = true;
				poi.Done = true;
				return true;
			}
			else if (poi.TalkRoutine == TalkScripts.Talk_ElfDocUnne)
			{
				if (poi.NpcRequirement == (byte)Item.Herb)
				{
					if (requirements.HasFlag(AccessRequirement.Herb))
					{
						herbCheckedIn = true;
						poi.Done = true;
						return true;
					}
				}
				else if (poi.NpcRequirement == (byte)Item.Slab)
				{
					if (requirements.HasFlag(AccessRequirement.Slab))
					{
						slabTranslated = true;
						poi.Done = true;
						return true;
					}
				}
			}
			else if (npcs.TryGetValue(poi.Npc.ObjectId, out var npc))
			{
				switch (poi.TalkRoutine)
				{
					case TalkScripts.Talk_Bikke:
						ProcessItem(npc.Item, dungeonIndex);
						rewardSources.Add(npc);
						poi.Done = true;
						return true;
					case TalkScripts.Talk_GiveItemOnFlag:
						return ProcessItemOnFlag(poi, npc, dungeonIndex);
					case TalkScripts.Talk_Nerrick:
					case TalkScripts.Talk_TradeItems:
					case TalkScripts.Talk_GiveItemOnItem:
					case TalkScripts.Talk_Astos:
						if (requirements.HasFlag(npc.AccessRequirement))
						{
							ProcessItem(npc.Item, dungeonIndex);
							rewardSources.Add(npc);
							poi.Done = true;
							return true;
						}
						break;
					default:
						return true;
				}
			}
			else if (allQuestNpcs.TryGetValue(poi.Npc.ObjectId, out var npc1))
			{
				switch (poi.TalkRoutine)
				{
					case TalkScripts.Talk_Bikke:
						rewardSources.Add(npc1);
						poi.Done = true;
						return true;
					case TalkScripts.Talk_GiveItemOnFlag:
						return ProcessItemOnFlag(poi, npc1, dungeonIndex, false);
					case TalkScripts.Talk_Nerrick:
					case TalkScripts.Talk_TradeItems:
					case TalkScripts.Talk_GiveItemOnItem:
					case TalkScripts.Talk_Astos:
						if (requirements.HasFlag(npc1.AccessRequirement))
						{
							rewardSources.Add(npc1);
							poi.Done = true;
							return true;
						}
						break;
					default:
						return true;
				}
			}

			return false;
		}

		private bool ProcessItemOnFlag(SCPointOfInterest poi, NpcReward npc, byte dungeonIndex, bool giveItem = true)
		{
			var flag = (ObjectId)poi.NpcRequirement;

			if(npc.ObjectId == ObjectId.Fairy)
			{
				if (requirements.HasFlag(AccessRequirement.Bottle))
				{
					if (giveItem) ProcessItem(npc.Item, dungeonIndex);
					npc.Entrance = (OverworldTeleportIndex)dungeonIndex;
					rewardSources.Add(npc);
					poi.Done = true;
					return true;
				}
			}
			else if (npc.ObjectId == ObjectId.Princess2)
			{
				if (princessRescued)
				{
					if (giveItem) ProcessItem(npc.Item, dungeonIndex);
					npc.Entrance = (OverworldTeleportIndex)dungeonIndex;
					rewardSources.Add(npc);
					poi.Done = true;
					return true;
				}
			}
			else if (flag == ObjectId.Unne)
			{
				if (slabTranslated)
				{
					if (giveItem) ProcessItem(npc.Item, dungeonIndex);
					npc.Entrance = (OverworldTeleportIndex)dungeonIndex;
					rewardSources.Add(npc);
					poi.Done = true;
					return true;
				}
			}
			else if (flag == ObjectId.ElfDoc)
			{
				if (herbCheckedIn)
				{
					if (giveItem) ProcessItem(npc.Item, dungeonIndex);
					npc.Entrance = (OverworldTeleportIndex)dungeonIndex;
					rewardSources.Add(npc);
					poi.Done = true;
					return true;
				}
			}
			else if (flag == ObjectId.Princess1)
			{
				if (princessRescued)
				{
					if (giveItem) ProcessItem(npc.Item, dungeonIndex);
					npc.Entrance = (OverworldTeleportIndex)dungeonIndex;
					rewardSources.Add(npc);
					poi.Done = true;
					return true;
				}
			}
			else if (flag == ObjectId.Vampire)
			{
				if (vampireAccessible)
				{
					if (giveItem) ProcessItem(npc.Item, dungeonIndex);
					npc.Entrance = (OverworldTeleportIndex)dungeonIndex;
					rewardSources.Add(npc);
					poi.Done = true;
					return true;
				}
			}
			else if (flag == ObjectId.None)
			{
				if (giveItem) ProcessItem(npc.Item, dungeonIndex);
				npc.Entrance = (OverworldTeleportIndex)dungeonIndex;
				rewardSources.Add(npc);
				poi.Done = true;
				return true;
			}

			return false;
		}

		private void ProcessItem(Item item, byte dungeonIndex)
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
					if (airBoat && ((changes & MapChange.Ship) > 0))
					{
						LiftOff();
					}
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
					SetShipDock();
					if (airBoat && ((changes & MapChange.Airship) > 0))
					{
						LiftOff();
					}
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

		private void BuildInitialRequirements(IVictoryConditionFlags victoryConditions, bool layoutcheck)
		{
			airShipLocationAccessible = false;

			if (layoutcheck || victoryConditions.GameMode == GameModes.DeepDungeon)
			{
				requirements = AccessRequirement.AllExceptEnding;
				changes = MapChange.All;
			}
			else
			{
				requirements = AccessRequirement.None;
				changes = MapChange.None;
			}

			if ((bool)victoryConditions.FreeLute)
			{
				requirements |= AccessRequirement.Lute;
			}
			if ((bool)victoryConditions.FreeRod)
			{
				requirements |= AccessRequirement.Rod;
			}

			if (victoryConditions.IsBridgeFree ?? false)
			{
				changes |= MapChange.Bridge;
			}
			if (victoryConditions.IsShipFree ?? false)
			{
				changes |= MapChange.Ship;
				SetShipDock();
			}
			if (victoryConditions.IsAirshipFree ?? false)
			{
				changes |= MapChange.Airship;
				airShipLocationAccessible = true;
			}
			if (victoryConditions.IsCanalFree ?? false)
			{
				changes |= MapChange.Canal;
			}
			if (victoryConditions.IsCanoeFree ?? false)
			{
				changes |= MapChange.Canoe;
			}
		}

		public IEnumerable<IRewardSource> GetNearRewardSources(IEnumerable<IRewardSource> sources, IRewardSource current)
		{
			if (current is TreasureChest c)
			{
				var chests = sources.Select(r => r as TreasureChest).Where(r => r != null).ToDictionary(r => (byte)(r.Address - 0x3100));
				var chest = (byte)(c.Address - 0x3100);

				foreach (var dungeon in Main.Dungeons)
				{
					var poi = dungeon.PointsOfInterest.Where(p => p.Type == SCPointOfInterestType.Treasure).FirstOrDefault(p => p.TreasureId == chest);

					if (poi != null)
					{
						return dungeon.PointsOfInterest.Where(p => p.Type == SCPointOfInterestType.Treasure)
							.Where(p => p.BitFlagSet.ToString() == poi.BitFlagSet.ToString())
							.Where(p=> chests.ContainsKey(p.TreasureId))
							.Select(p => chests[p.TreasureId]).ToList();
					}
				}
			}

			return Array.Empty<IRewardSource>();
		}
	}
}
