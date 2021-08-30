﻿using RomUtilities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace FF1Lib
{
	public partial class FF1Rom : NesRom
	{
		public const int TreasureJingleOffset = 0x47600;
		public const int TreasureOffset = 0x03100;
		public const int TreasureSize = 1;
		public const int TreasurePoolCount = 256;
		public const int TreasureCount = 256;

		public const int lut_MapObjTalkJumpTblAddress = 0x390D3;
		public const string giveRewardRoutineAddress = "10B4";
		public static readonly List<int> UnusedTreasureIndices =
			Enumerable.Range(0, 1).Concat(
			Enumerable.Range(145, 4)).Concat(
			Enumerable.Range(187, 9)).Concat(
			Enumerable.Range(255, 1))
			.ToList();

		public static readonly List<int> UsedTreasureIndices = Enumerable.Range(0, 256).Except(UnusedTreasureIndices).ToList(); // This maps a compacted list back to the game's array, skipping the unused slots.

		public List<IRewardSource> ShuffleTreasures(MT19337 rng,
													IItemPlacementFlags flags,
													IncentiveData incentivesData,
													ItemShopSlot caravanItemLocation,
													OverworldMap overworldMap,
													TeleportShuffle teleporters,
													ISanityChecker checker)
		{
			Dictionary<MapLocation, Tuple<List<MapChange>, AccessRequirement>> fullFloorRequirements = overworldMap.FullLocationRequirements;
			Dictionary<MapLocation, OverworldTeleportIndex> overridenOverworld = overworldMap.OverriddenOverworldLocations;

			var vanillaNPCs = !(flags.NPCItems ?? false) && !(flags.NPCFetchItems ?? false);

			var treasureBlob = Get(TreasureOffset, TreasureSize * TreasureCount);
			var treasurePool = UsedTreasureIndices.Select(x => (Item)treasureBlob[x])
							.Concat(ItemLists.AllNonTreasureChestItems).ToList();

			if (flags.ShardHunt)
			{
				treasurePool = treasurePool.Select(ShardHuntTreasureSelector).ToList();
				int shardsAdded = treasurePool.Count(item => item == Item.Shard);
				Debug.Assert(shardsAdded == TotalOrbsToInsert);
			}

			ItemPlacement placement = ItemPlacement.Create(flags, incentivesData, treasurePool, caravanItemLocation, overworldMap, checker);
			var placedItems = placement.PlaceSaneItems(rng, this);
			
			// Output the results to the ROM
			foreach (var item in placedItems.Where(x => !x.IsUnused && x.Address < 0x80000 && (!vanillaNPCs || x is TreasureChest)))
			{
				//Debug.WriteLine(item.SpoilerText);
				item.Put(this);
			}
			// Move the ship someplace closer to where it really ends up.
			if (!(flags.IsShipFree ?? false) && !(flags.OwMapExchange == OwMapExchanges.Desert))
			{
				MapLocation shipLocation = placedItems.Find(reward => reward.Item == Item.Ship).MapLocation;
				if (overridenOverworld != null && overridenOverworld.TryGetValue(shipLocation, out var overworldIndex))
				{
					shipLocation = teleporters.OverworldMapLocations.TryGetValue(overworldIndex, out var vanillaShipLocation) ? vanillaShipLocation : shipLocation;
				}
				MoveShipToRewardSource(shipLocation);
			}
			return placedItems;
		}

		private void MoveShipToRewardSource(MapLocation vanillaMapLocation)
		{
			if (!ItemLocations.ShipLocations.TryGetValue(vanillaMapLocation, out Blob location))
			{
				location = Dock.Coneria;
			}

			Put(0x3000 + UnsramIndex.ShipX, location);
		}

		public void CraftRuseItem()
		{
			var newspell = GetSpells();
			WriteItemSpellData(newspell.Where(x => x.Data[4] == 0x10 && x.Data[3] == 0x04).First(), Item.PowerRod);
		}
	}
}
