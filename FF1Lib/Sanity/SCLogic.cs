using static FF1Lib.FF1Rom;

namespace FF1Lib.Sanity
{
	public class SCLogic
	{
		private FF1Rom rom;
		private SCMain main;
		List<IRewardSource> itemPlacement;
		private OwLocationData locations;
		IVictoryConditionFlags victoryConditions;
		bool excludeBridge;

		Dictionary<short, SCLogicArea> processedAreas = new Dictionary<short, SCLogicArea>();

		Queue<SCLogicAreaQueueEntry> todoset = new Queue<SCLogicAreaQueueEntry>(256);

		SCOwArea shipDockArea;

		Dictionary<short, List<SCLogicAreaQueueEntry>> tunnels = new Dictionary<short, List<SCLogicAreaQueueEntry>>();

		public List<SCLogicRewardSource> RewardSources { get; }

		Dictionary<int, SCLogicRewardSource> rewardSourceDic = new Dictionary<int, SCLogicRewardSource>();

		public SCLogic(FF1Rom _rom, SCMain _main, List<IRewardSource> _itemPlacement, OwLocationData _locations, IVictoryConditionFlags _victoryConditions, bool _excludeBridge)
		{
			rom = _rom;
			main = _main;
			itemPlacement = _itemPlacement;
			victoryConditions = _victoryConditions;
			excludeBridge = _excludeBridge;

			locations = _locations;
			//locations.LoadData();

			GetShipDockArea();

			BuildTunnels();

			DoPathing();

			DoAirShipPathing();

			RemoveEmptyAreas();

			ProcessTreasures();

			ProcessShopSlot();

			ProcessNPCs();

			RewardSources = rewardSourceDic.Values.ToList();

			ProcessFreebies();
		}

		private void GetShipDockArea()
		{
			var coords = locations.ShipLocation;

			SetShipDock(coords.OwLeft);
			SetShipDock(coords.OwRight);
			SetShipDock(coords.OwUp);
			SetShipDock(coords.OwDown);
		}

		private void SetShipDock(SCCoords coords)
		{
			if ((main.Overworld.Tiles[coords.X, coords.Y].Tile & SCBitFlags.ShipDock) > 0) shipDockArea = main.Overworld.Areas[main.Overworld.Tiles[coords.X, coords.Y].Area];
		}

		private void BuildTunnels()
		{
			foreach(var area in main.Overworld.Areas.Values)
			{
				foreach(var enter in area.PointsOfInterest.Where(p => p.Type == SCPointOfInterestType.Tele))
				{
					var dungeon = main.Dungeons.First(d => d.OverworldTeleport == enter.Teleport.OverworldTeleport);
					var exits = dungeon.PointsOfInterest.Where(p => p.Type == SCPointOfInterestType.Exit);

					foreach (var exit in exits)
					{
						var exitAreaId = main.Overworld.Tiles[exit.Teleport.TargetCoords.X, exit.Teleport.TargetCoords.Y].Area;
						var exitArea = main.Overworld.Areas[exitAreaId];

						if (exitAreaId != area.Index)
						{
							var tunnel = new SCLogicAreaQueueEntry { AreaId = exitAreaId, Requirements = exit.Requirements };

							if (!tunnels.ContainsKey(area.Index)) tunnels.Add(area.Index, new List<SCLogicAreaQueueEntry>());
							tunnels[area.Index].Add(tunnel);
						}
					}
				}
			}
		}

		private void DoPathing()
		{
			short areaIndex = main.Overworld.Tiles[locations.StartingLocation.X, locations.StartingLocation.Y].Area;
			SCOwArea area = main.Overworld.Areas[areaIndex];

			foreach (var link in area.Links)
			{
				CheckArea(link, new SCRequirementsSet(SCRequirements.None));
			}

			while (todoset.Count > 0)
			{
				var link = todoset.Dequeue();

				CheckArea(link.AreaId, link.Requirements);
			}
		}

		private void DoAirShipPathing()
		{
			var areaIndex = (bool)victoryConditions.AirBoat ? main.Overworld.Tiles[locations.ShipLocation.X, locations.ShipLocation.Y].Area : main.Overworld.Tiles[locations.AirShipLocation.X, locations.AirShipLocation.Y].Area;
			var area = main.Overworld.Areas[areaIndex];

			if (processedAreas.TryGetValue(areaIndex, out var logicArea))
			{
				foreach (var landable in main.Overworld.Areas.Values.Where(a => (a.Tile & SCBitFlags.AirDock) > 0))
				{
					CheckArea(landable.Index, logicArea.Requirements.Restrict(SCRequirements.Floater | ((bool)victoryConditions.AirBoat ? SCRequirements.Ship : SCRequirements.None)));
				}

				while (todoset.Count > 0)
				{
					var link = todoset.Dequeue();

					CheckArea(link.AreaId, link.Requirements);
				}
			}
		}

		private void CheckArea(short areaId, SCRequirementsSet flags)
		{
			var area = main.Overworld.Areas[areaId];

			var newFlags = flags.Restrict(area.Requirements);

			if (processedAreas.TryGetValue(areaId, out var logicArea))
			{
				if (logicArea.Requirements.Merge(newFlags))
				{
					foreach (var link in area.Links)
					{
						AddToDo(area, link, newFlags);
					}
				}
			}
			else
			{
				var newLogicArea = new SCLogicArea(areaId, area, newFlags);
				processedAreas.Add(areaId, newLogicArea);

				foreach (var link in area.Links)
				{
					AddToDo(area, link, newFlags);
				}
			}
		}

		private void AddToDo(SCOwArea currentArea, short areaId, SCRequirementsSet requirements)
		{
			var area = main.Overworld.Areas[areaId];

			if ((area.Tile & SCBitFlags.Ocean) > 0 && (currentArea.Tile & SCBitFlags.Ocean) == 0 && currentArea != shipDockArea) return;

			if (tunnels.TryGetValue(areaId, out var tlist))
			{
				foreach (var tunnel in tlist)
				{
					var newRequirements = requirements?.Restrict(tunnel.Requirements);
					todoset.Enqueue(new SCLogicAreaQueueEntry { AreaId = tunnel.AreaId, Requirements = newRequirements });
				}
			}

			if ((area.Tile & SCBitFlags.Bridge) > 0)
			{
				CrossBridge(currentArea, requirements, area);
			}
			else if ((area.Tile & SCBitFlags.Canal) > 0)
			{
				CrossCanal(currentArea, requirements, area);
			}
			else
			{
				todoset.Enqueue(new SCLogicAreaQueueEntry { AreaId = areaId, Requirements = requirements });
			}
		}

		private void CrossBridge(SCOwArea currentArea, SCRequirementsSet requirements, SCOwArea area)
		{
			if ((currentArea.Tile & SCBitFlags.Ocean) > 0)
			{
				foreach (var link in area.Links)
				{
					var area2 = main.Overworld.Areas[link];

					if ((area2.Tile & SCBitFlags.Ocean) > 0) todoset.Enqueue(new SCLogicAreaQueueEntry { AreaId = link, Requirements = requirements });
				}
			}
			else
			{
				var newRequirements = requirements?.Restrict(SCRequirements.Bridge);

				foreach (var link in area.Links)
				{
					var area2 = main.Overworld.Areas[link];

					if ((area2.Tile & SCBitFlags.Ocean) == 0) todoset.Enqueue(new SCLogicAreaQueueEntry { AreaId = link, Requirements = newRequirements });
				}
			}
		}

		private void CrossCanal(SCOwArea currentArea, SCRequirementsSet requirements, SCOwArea area)
		{
			if ((currentArea.Tile & SCBitFlags.Ocean) == 0)
			{
				foreach (var link in area.Links)
				{
					var area2 = main.Overworld.Areas[link];

					if ((area2.Tile & SCBitFlags.Ocean) == 0) todoset.Enqueue(new SCLogicAreaQueueEntry { AreaId = link, Requirements = requirements });
				}
			}
			else
			{
				var newRequirements = requirements?.Restrict(SCRequirements.Canal);

				foreach (var link in area.Links)
				{
					var area2 = main.Overworld.Areas[link];

					if ((area2.Tile & SCBitFlags.Ocean) > 0) todoset.Enqueue(new SCLogicAreaQueueEntry { AreaId = link, Requirements = newRequirements });
				}
			}
		}

		private void RemoveEmptyAreas()
		{
			processedAreas = processedAreas.Values.Where(a => a.Area.PointsOfInterest.Count > 0).ToDictionary(a => a.AreaId, a => a);
		}

		private void ProcessTreasures()
		{
			var chests = itemPlacement.Select(r => r as TreasureChest).Where(r => r != null).ToDictionary(r => (byte)(r.Address - 0x3100));

			foreach (var logicArea in processedAreas.Values)
			{
				var area = logicArea.Area;

				foreach (var enter in area.PointsOfInterest.Where(p => p.Type == SCPointOfInterestType.Tele))
				{
					var dungeon = main.Dungeons.First(d => d.OverworldTeleport == enter.Teleport.OverworldTeleport);

					foreach (var treasure in dungeon.PointsOfInterest.Where(p => p.Type == SCPointOfInterestType.Treasure))
					{
						if (chests.TryGetValue(treasure.TreasureId, out var rewardSource))
						{
							if (rewardSourceDic.TryGetValue(rewardSource.Address, out var x))
							{
								x.Requirements.Merge(logicArea.Requirements.Restrict(treasure.Requirements));
							}
							else
							{
								rewardSourceDic.Add(rewardSource.Address, new SCLogicRewardSource
								{
									Requirements = logicArea.Requirements.Restrict(treasure.Requirements),
									RewardSource = rewardSource
								});
							}
						}
					}
				}
			}
		}

		private void ProcessShopSlot()
		{
			//ToDo: add caravan shop
			var shopslot = (ItemShopSlot)itemPlacement.FirstOrDefault(r => r is ItemShopSlot);

			if (shopslot == null) return;

			foreach (var logicArea in processedAreas.Values)
			{
				var area = logicArea.Area;

				foreach (var enter in area.PointsOfInterest.Where(p => p.Type == SCPointOfInterestType.Tele))
				{
					var dungeon = main.Dungeons.First(d => d.OverworldTeleport == enter.Teleport.OverworldTeleport);

					foreach (var shop in dungeon.PointsOfInterest.Where(p => p.Type == SCPointOfInterestType.Shop))
					{
						if (shopslot.ShopIndex == (shop.ShopId - 1))
						{
							if (rewardSourceDic.TryGetValue(shopslot.Address, out var x))
							{
								x.Requirements.Merge(logicArea.Requirements.Restrict(shop.Requirements));
							}
							else
							{
								rewardSourceDic.Add(shopslot.Address, new SCLogicRewardSource
								{
									Requirements = logicArea.Requirements.Restrict(shop.Requirements),
									RewardSource = shopslot
								});
							}
						}
					}
				}

				foreach (var shop in area.PointsOfInterest.Where(p => p.Type == SCPointOfInterestType.Shop))
				{
					if (shopslot.ShopIndex == (shop.ShopId - 1))
					{
						if (rewardSourceDic.TryGetValue(shopslot.Address, out var x))
						{
							x.Requirements.Merge(logicArea.Requirements);
						}
						else
						{
							rewardSourceDic.Add(shopslot.Address, new SCLogicRewardSource
							{
								Requirements = logicArea.Requirements,
								RewardSource = shopslot
							});
						}
					}
				}
			}
		}

		private void ProcessNPCs()
		{
			var npcRewardSources = itemPlacement.Select(r => r as NpcReward).Where(r => r != null).ToDictionary(r => r.ObjectId);

			Dictionary<ObjectId, SCRequirementsSet> allNpcs = new Dictionary<ObjectId, SCRequirementsSet>();

			foreach (var logicArea in processedAreas.Values)
			{
				var area = logicArea.Area;

				foreach (var enter in area.PointsOfInterest.Where(p => p.Type == SCPointOfInterestType.Tele))
				{
					var dungeon = main.Dungeons.First(d => d.OverworldTeleport == enter.Teleport.OverworldTeleport);

					foreach (var poi in dungeon.PointsOfInterest.Where(p => p.Type == SCPointOfInterestType.QuestNpc))
					{
						if (allNpcs.TryGetValue(poi.Npc.ObjectId, out var requirements))
						{
							requirements.Merge(logicArea.Requirements.Restrict(poi.Requirements));
						}
						else
						{
							allNpcs.Add(poi.Npc.ObjectId, logicArea.Requirements.Restrict(poi.Requirements));
						}
					}
				}
			}

			Dictionary<Item, SCRequirementsSet> allOrbs = new Dictionary<Item, SCRequirementsSet>();

			foreach (var logicArea in processedAreas.Values)
			{
				var area = logicArea.Area;

				foreach (var enter in area.PointsOfInterest.Where(p => p.Type == SCPointOfInterestType.Tele))
				{
					var dungeon = main.Dungeons.First(d => d.OverworldTeleport == enter.Teleport.OverworldTeleport);

					foreach (var poi in dungeon.PointsOfInterest.Where(p => p.Type == SCPointOfInterestType.Orb))
					{
						if (allOrbs.TryGetValue(poi.ItemId, out var requirements))
						{
							requirements.Merge(poi.Requirements);
						}
						else
						{
							allOrbs.Add(poi.ItemId, logicArea.Requirements.Restrict(poi.Requirements));
						}
					}
				}
			}

			foreach (var logicArea in processedAreas.Values)
			{
				var area = logicArea.Area;

				foreach (var enter in area.PointsOfInterest.Where(p => p.Type == SCPointOfInterestType.Tele))
				{
					var dungeon = main.Dungeons.First(d => d.OverworldTeleport == enter.Teleport.OverworldTeleport);

					foreach (var poi in dungeon.PointsOfInterest.Where(p => p.Type == SCPointOfInterestType.QuestNpc))
					{
						if (npcRewardSources.TryGetValue(poi.Npc.ObjectId, out var rewardSource))
						{
							var requirements = logicArea.Requirements.Restrict(poi.Requirements);

							var secondaryRequirements = GetSecondaryRequirements(poi, rewardSource, allNpcs, allOrbs);

							if (rewardSourceDic.TryGetValue(rewardSource.Address, out var x))
							{
								x.Requirements.Merge(requirements.Restrict(secondaryRequirements));
							}
							else
							{
								rewardSourceDic.Add(rewardSource.Address, new SCLogicRewardSource
								{
									Requirements = requirements.Restrict(secondaryRequirements),
									RewardSource = rewardSource
								});
							}
						}
					}
				}
			}
		}

		private SCRequirementsSet GetSecondaryRequirements(SCPointOfInterest poi, NpcReward rewardSource, Dictionary<ObjectId, SCRequirementsSet> allNpcs, Dictionary<Item, SCRequirementsSet> allOrbs)
		{
			if (poi.TalkRoutine == newTalkRoutines.Talk_ElfDocUnne)
			{
				if (poi.TalkArray[(int)TalkArrayPos.requirement_id] == (byte)Item.Herb)
				{
					return new SCRequirementsSet(SCRequirements.Herb);
				}
				else if (poi.TalkArray[(int)TalkArrayPos.requirement_id] == (byte)Item.Slab)
				{
					return new SCRequirementsSet(SCRequirements.Herb);
				}
			}
			else
			{
				switch(poi.TalkRoutine)
				{
					case newTalkRoutines.Talk_Bikke:
						return new SCRequirementsSet(SCRequirements.None);
					case newTalkRoutines.Talk_GiveItemOnFlag:
						return ProcessItemOnFlag(poi, rewardSource, allNpcs);
					case newTalkRoutines.Talk_Nerrick:
					case newTalkRoutines.Talk_TradeItems:
					case newTalkRoutines.Talk_GiveItemOnItem:
					case newTalkRoutines.Talk_Astos:
						return ProcessItemOnItem(poi, rewardSource, allNpcs, allOrbs);
				}
			}

			throw new NotSupportedException();
		}

		private SCRequirementsSet ProcessItemOnItem(SCPointOfInterest poi, NpcReward rewardSource, Dictionary<ObjectId, SCRequirementsSet> allNpcs, Dictionary<Item, SCRequirementsSet> allOrbs)
		{
			if (rewardSource.AccessRequirement == AccessRequirement.EarthOrb)
			{
				return allOrbs[Item.EarthOrb];
			}
			else if (rewardSource.AccessRequirement == AccessRequirement.FireOrb)
			{
				return allOrbs[Item.FireOrb];
			}
			else if (rewardSource.AccessRequirement == AccessRequirement.WaterOrb)
			{
				return allOrbs[Item.WaterOrb];
			}
			else if (rewardSource.AccessRequirement == AccessRequirement.AirOrb)
			{
				return allOrbs[Item.AirOrb];
			}

			return new SCRequirementsSet(rewardSource.AccessRequirement);
		}

		private SCRequirementsSet ProcessItemOnFlag(SCPointOfInterest poi, NpcReward rewardSource, Dictionary<ObjectId, SCRequirementsSet> allNpcs)
		{
			var flag = (ObjectId)poi.TalkArray[(int)TalkArrayPos.requirement_id];

			if (poi.Npc.ObjectId == ObjectId.Fairy)
			{
				return new SCRequirementsSet(SCRequirements.Bottle);
			}
			else if (poi.Npc.ObjectId == ObjectId.Princess2)
			{
				return allNpcs[ObjectId.Princess1];
			}
			else if (flag == ObjectId.Unne)
			{
				return allNpcs[ObjectId.Unne].Restrict(SCRequirements.Slab);
			}
			else if (flag == ObjectId.ElfDoc)
			{
				return allNpcs[ObjectId.ElfDoc].Restrict(SCRequirements.Herb);
			}
			else if (flag == ObjectId.Princess1)
			{
				return allNpcs[ObjectId.Princess1];
			}
			else if (flag == ObjectId.Vampire)
			{
				return allNpcs[ObjectId.Vampire];
			}
			else if (flag == ObjectId.None)
			{
				return new SCRequirementsSet(SCRequirements.None);
			}

			throw new NotSupportedException();
		}

		private void ProcessFreebies()
		{
			var requirements = SCRequirements.None;
			if ((bool)victoryConditions.FreeLute)
			{
				requirements |= SCRequirements.Lute;
			}
			if ((bool)victoryConditions.FreeRod)
			{
				requirements |= SCRequirements.Rod;
			}
			if ((victoryConditions.IsBridgeFree ?? false) || excludeBridge)
			{
				requirements |= SCRequirements.Bridge;
			}
			if (victoryConditions.IsShipFree ?? false)
			{
				requirements |= SCRequirements.Ship;
			}
			if (victoryConditions.IsAirshipFree ?? false)
			{
				requirements |= SCRequirements.Floater;
			}
			if (victoryConditions.IsCanalFree ?? false)
			{
				requirements |= SCRequirements.Canal;
			}
			if (victoryConditions.IsCanoeFree ?? false)
			{
				requirements |= SCRequirements.Canoe;
			}

			foreach (var rewardSource in RewardSources)
			{
				rewardSource.Requirements = rewardSource.Requirements.Ease(requirements);
			}
		}
	}

	public struct SCLogicAreaQueueEntry
	{
		public SCRequirementsSet Requirements { get; set; }

		public short AreaId { get; set; }
	}

	public class SCLogicRewardSource
	{
		public SCRequirementsSet Requirements { get; set; }

		public IRewardSource RewardSource { get; set; }
	}
}
