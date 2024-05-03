using RomUtilities;
using System.ComponentModel;
using System.Diagnostics;

namespace FF1Lib
{
	public class InsaneException : Exception
	{
		public InsaneException(string message)
			: base(message) { }
	}

	public abstract class ItemPlacement
	{
		private const Item ReplacementItem = Item.Cabin;

		private const int TreasureJingleOffset = 0x47600;
		private const int TreasureOffset = 0x03100;
		private const int TreasureChestOrderOffset = 0x47F00;
		private const int TreasureSize = 1;
		private const int TreasurePoolCount = 256;
		private const int TreasureCount = 256;

		private static List<IRewardSource> TrapChests = new List<IRewardSource>()
		{
			ItemLocations.ToFTopRight2, // In-Out trap tile forced
			ItemLocations.ToFBottomRight, // Locked trap tile
			ItemLocations.MarshCave12, // Locked, center, trap tile
			ItemLocations.EarthCave10, // Entrance trap, vanilla wooden shield
			ItemLocations.EarthCave12, // Room before Vampire, trapped
			ItemLocations.EarthCave20, // Far corner chest in Earth B4, top room
			ItemLocations.Volcano6, // Armory trapped tile
			ItemLocations.Volcano15, // Top hairpin, trapped, vanilla Giant Sword
			ItemLocations.Volcano31, // Trapped far room B4, vanilla flame shield
			ItemLocations.VolcanoMajor, // Red D chest
			ItemLocations.IceCave6, // Trapped chest to right of floater
			ItemLocations.IceCaveMajor, // Vanilla floater
			ItemLocations.IceCave15, // In-out trap tile forced B3, vanilla Silver Gauntlet
			ItemLocations.SeaShrine9, // B4 trapped chest, vanilla Power Gauntlet
			ItemLocations.SeaShrine3, // In-Out trap Sharknado chest
			ItemLocations.SkyPalace33, // Top chest B3, vanilla Pro-Ring
		};
		public static ItemPlacement Create(FF1Rom rom, Flags flags, PlacementContext placementContext, ItemShopSlot shopSlot, Overworld overworld, ISanityChecker checker)
		{
			ItemPlacement placement;
			placement = new PredictivePlacement();

			placement.flags = flags;
			placement.placementContext = placementContext;
			placement.shopItemLocation = shopSlot;
			placement.overworld = overworld;
			placement.checker = checker;
			placement.rom = rom;

			return placement;
		}

		protected struct ItemPlacementContext
		{
			public List<IRewardSource> Forced;
			public List<Item> Incentivized;
			public List<Item> Unincentivized;
			public List<Item> Shards;
			public List<Item> Removed;
			public List<Item> KeyItemsToPlace;
			public IReadOnlyCollection<Item> AllTreasures;
		}

		protected struct ItemPlacementResult
		{
			public List<IRewardSource> PlacedItems;
			public List<Item> RemainingTreasures;
		}

		protected int sanityCounter = 0;
		protected Flags flags;
		//protected IncentiveData _incentivesData;
		protected PlacementContext placementContext;
		//protected List<Item> _allTreasures;
		protected ItemShopSlot shopItemLocation;
		protected Overworld overworld;
		protected ISanityChecker checker;
		protected FF1Rom rom;
		private List<IRewardSource> jingleChests;
		public List<IRewardSource> PlacedItems;

		protected abstract ItemPlacementResult DoSanePlacement(MT19337 rng, OwLocationData locations);
		public void PlaceItems(MT19337 rng)
		{
			var randomizeTreasureMode = (flags.RandomizeTreasure == RandomizeTreasureMode.Random) ? (RandomizeTreasureMode)rng.Between(0, 2) : flags.RandomizeTreasure;
			List<IRewardSource> placedItems = new();
			List<Item> treasurePool = new();

			var maxRetries = 3;
			for (var i = 0; i < maxRetries; i++)
			{
				try
				{
					// Place Key Item & Shards
					ItemPlacementResult result = DoSanePlacement(rng, overworld.Locations);
					placedItems = result.PlacedItems;

					jingleChests = placedItems.Where(i => i.Item != Item.Shard && i.GetType() == typeof(TreasureChest)).ToList();
					treasurePool = result.RemainingTreasures;

					// 8. Place all remaining unincentivized treasures or incentivized non-quest items that weren't placed
					var itemLocationPool = placementContext.AllValidItemLocations.ToList();
					itemLocationPool = itemLocationPool.Where(x => !x.IsUnused && !placedItems.Any(y => y.Address == x.Address)).ToList();

					if ((bool)flags.BetterTrapChests && randomizeTreasureMode != RandomizeTreasureMode.DeepDungeon)
					{
						// First we'll make a list of all 'notable' treasure.
						var notableTreasureList = new List<Item>()
							.Concat(ItemLists.UberTier)
							.Concat(ItemLists.LegendaryWeaponTier)
							.Concat(ItemLists.LegendaryArmorTier)
							.Concat(ItemLists.RareWeaponTier)
							.Concat(ItemLists.RareArmorTier);
						// Convert the list to a HashSet since we'll be doing lookups in it.
						var notableTreasure = new HashSet<Item>(notableTreasureList);

						// We sort the treasure pool based on value (sort of) and pull out the highest ranked ones to put
						// in the trap chests we picked out.
						var notableTreasurePool = treasurePool.Where(item => notableTreasure.Contains(item)).ToList();

						// Since some chests might be incentivized, remove those that aren't in the pool.
						var trapChestPool = TrapChests.Where(chest => itemLocationPool.Contains(chest));

						foreach (var chest in trapChestPool)
						{
							// It seems unlikely that is possible, but just in case.
							if (!notableTreasurePool.Any()) break;

							// Pick a random treasure and place it.
							var treasure = notableTreasurePool.SpliceRandom(rng);
							placedItems.Add(NewItemPlacement(chest, treasure));

							// Since it was placed, remove both the item and location from the remaining pool.
							treasurePool.Remove(treasure);
							itemLocationPool.Remove(chest);
						}

					}

					if (randomizeTreasureMode == RandomizeTreasureMode.DeepDungeon && flags.DeepDungeonGenerator == DeepDungeonGeneratorMode.Progressive)
					{
						itemLocationPool = itemLocationPool.OrderBy(l => placementContext.WeightedFloors.TryGetValue(l.MapLocation, out var result) ? result : 0).ToList();
						treasurePool.Shuffle(rng);
						treasurePool = treasurePool.OrderBy(t => placementContext.ItemsQuality.TryGetValue(t, out var quality) ? quality : 0).ToList();

						foreach (var location in itemLocationPool)
						{
							var itemRange = treasurePool.GetRange(0, Math.Min(20, treasurePool.Count)).ToList();
							var selectedItem = itemRange.PickRandom(rng);

							placedItems.Add(NewItemPlacement(location, selectedItem));
							treasurePool.Remove(selectedItem);
						}
					}
					else
					{
						treasurePool.Shuffle(rng);
						itemLocationPool.Shuffle(rng);

						var leftovers = treasurePool.Zip(itemLocationPool, (treasure, location) => NewItemPlacement(location, treasure));
						placedItems.AddRange(leftovers);
					}


					break;
				}
				catch (InsaneException e)
				{
					Console.WriteLine(e.Message);
					if (maxRetries > (i + 1)) continue;
					throw new InvalidOperationException(e.Message);
				}
			}

			SetIncentiveChestItemsFanfare();
			SetChestsOrder(treasurePool, rng);

			PlacedItems = placedItems;
		}
		public void Write()
		{
			// Output the results to the ROM
			foreach (var item in PlacedItems.Where(x => !x.IsUnused && x.Address < 0x80000 && (x is TreasureChest)))
			{
				//Debug.WriteLine(item.SpoilerText);
				item.Put(rom);
			}
		}


		/*
		public List<IRewardSource> PlaceSaneItems(MT19337 rng, OwLocationData locations, FF1Rom rom)
		{
			_rom = rom;

			var incentivePool = _incentivesData.IncentiveItems.Where(x => _allTreasures.Contains(x)).ToList();
			var forcedItems = _incentivesData.ForcedItemPlacements.ToList();
			var removedItems = _incentivesData.RemovedItems.ToList();

			var unincentivizedQuestItems =
				ItemLists.AllQuestItems
					.Where(x => !incentivePool.Contains(x) &&
								_allTreasures.Contains(x) &&
								!forcedItems.Any(y => y.Item == x) &&
								!removedItems.Contains(x))
								.ToList();

			var shards = new List<Item>();

			var treasurePool = _allTreasures.ToList();

			bool jingleGuaranteedDefenseItem = true;
			bool jingleGuaranteedPowerItem = true;

			if (_flags.GuaranteedDefenseItem != GuaranteedDefenseItem.None && !(_flags.ItemMagicMode == ItemMagicMode.None) && !incentivePool.Contains(Item.PowerRod))
			{
				unincentivizedQuestItems.Add(Item.PowerRod);
				jingleGuaranteedDefenseItem = false;
			}

			if (_flags.GuaranteedPowerItem != GuaranteedPowerItem.None && !(_flags.ItemMagicMode == ItemMagicMode.None) && !incentivePool.Contains(Item.PowerGauntlets))
			{
				unincentivizedQuestItems.Add(Item.PowerGauntlets);
				jingleGuaranteedPowerItem = false;
			}

			foreach (var incentive in incentivePool)
			{
				treasurePool.Remove(incentive);
			}
			foreach (var placement in forcedItems)
			{
				treasurePool.Remove(placement.Item);
			}
			foreach (var questItem in unincentivizedQuestItems)
			{
				treasurePool.Remove(questItem);
			}
			foreach (var item in removedItems)
			{
				treasurePool.Remove(item);
			}
			while (treasurePool.Remove(Item.Shard))
			{
				shards.Add(Item.Shard);
			}

			ItemPlacementContext ctx = new ItemPlacementContext
			{
				Forced = forcedItems,
				Incentivized = incentivePool,
				Unincentivized = unincentivizedQuestItems,
				Shards = shards,
				Removed = removedItems,
				AllTreasures = treasurePool.ToList(),
				KeyItemsToPlace = _incentivesData.KeyItemsToPlace.ToList(),
			};

			ItemPlacementResult result = DoSanePlacement(rng, locations);
			List<IRewardSource> placedItems = result.PlacedItems;

			treasurePool = result.RemainingTreasures;

			//setup jingle for "incentive treasures", the placed items should just be key items, loose incentive items
			if (_flags.IncentiveChestItemsFanfare)
			{
				foreach (var placedItem in placedItems)
				{
					//dont make shards jingle that'd be annoying
					//dont make free items that get replaced, aka cabins, jingle
					if (placedItem is TreasureChest && placedItem.Item != Item.Shard &&
						placedItem.Item != ReplacementItem)
					{
						if ((placedItem.Item == Item.PowerGauntlets && !jingleGuaranteedPowerItem) || (placedItem.Item == Item.PowerRod && !jingleGuaranteedDefenseItem))
						{
							continue;
						}

						rom.Put(placedItem.Address - FF1Rom.TreasureOffset + FF1Rom.TreasureJingleOffset, new byte[] { (byte)placedItem.Item });
					}
				}
			}			

			// 8. Place all remaining unincentivized treasures or incentivized non-quest items that weren't placed
			var itemLocationPool = _incentivesData.AllValidItemLocations.ToList();
			itemLocationPool = itemLocationPool.Where(x => !x.IsUnused && !placedItems.Any(y => y.Address == x.Address)).ToList();

			MoreConsumableChests.Work(_flags, treasurePool, rng);

			if((bool)_flags.GuaranteedMasamune && !incentivePool.Contains(Item.Masamune) && (bool)_flags.SendMasamuneHome)
			{
				// Remove Masamune from treasure pool (This will also causes Masamune to not be placed by RandomLoot)
				treasurePool.Remove(Item.Masamune);
			}

			// Use cabins to balance item population
			int poolDifference = itemLocationPool.Count() - treasurePool.Count();
			if (poolDifference < 0)
			{
				for (int i = 0; i > poolDifference; i--)
				{
					treasurePool.Remove(Item.Cabin);
				}
			}
			else if (poolDifference > 0)
			{
				treasurePool.AddRange(Enumerable.Repeat(Item.Cabin, poolDifference));
			}

			Debug.Assert(treasurePool.Count() == itemLocationPool.Count());

			
			List<IRewardSource> normalTreasures = new();

			var randomizeTreasureMode = (_flags.RandomizeTreasure == RandomizeTreasureMode.Random) ? (RandomizeTreasureMode)rng.Between(0, 2) : _flags.RandomizeTreasure;

			if (randomizeTreasureMode != RandomizeTreasureMode.None)
			{
				IItemGenerator generator;

				// We want to leave out anything incentivized (and thus already placed), but
				// add all the other stuff that you can't find in vanilla.
				var randomTreasure = treasurePool.ToList();
				
				if (randomizeTreasureMode == RandomizeTreasureMode.DeepDungeon)
				{
					generator = new DeepDungeonItemGenerator(itemLocationPool, rom.UnusedGoldItems, removedItems, normalTreasures, (_flags.GameMode == GameModes.DeepDungeon), _flags.Etherizer, rom);
				}
				else
				{
					generator = new ItemGenerator(randomTreasure, rom.UnusedGoldItems, removedItems, _flags.WorldWealth);
				}

				treasurePool = treasurePool.Select(treasure => generator.GetItem(rng)).ToList();
			}

			if ((bool)_flags.BetterTrapChests && randomizeTreasureMode != RandomizeTreasureMode.DeepDungeon)
			{
				// First we'll make a list of all 'notable' treasure.
				var notableTreasureList = new List<Item>()
					.Concat(ItemLists.UberTier)
					.Concat(ItemLists.LegendaryWeaponTier)
					.Concat(ItemLists.LegendaryArmorTier)
					.Concat(ItemLists.RareWeaponTier)
					.Concat(ItemLists.RareArmorTier);
				// Convert the list to a HashSet since we'll be doing lookups in it.
				var notableTreasure = new HashSet<Item>(notableTreasureList);

				// We sort the treasure pool based on value (sort of) and pull out the highest ranked ones to put
				// in the trap chests we picked out.
				var notableTreasurePool = treasurePool.Where(item => notableTreasure.Contains(item)).ToList();

				// Since some chests might be incentivized, remove those that aren't in the pool.
				var trapChestPool = TrapChests.Where(chest => itemLocationPool.Contains(chest));

				foreach (var chest in trapChestPool)
				{
					// It seems unlikely that is possible, but just in case.
					if (!notableTreasurePool.Any()) break;

					// Pick a random treasure and place it.
					var treasure = notableTreasurePool.SpliceRandom(rng);
					placedItems.Add(NewItemPlacement(chest, treasure));

					// Since it was placed, remove both the item and location from the remaining pool.
					treasurePool.Remove(treasure);
					itemLocationPool.Remove(chest);
				}

				// This should still be true at the end, so make sure it is
				Debug.Assert(treasurePool.Count() == itemLocationPool.Count());
			}

			if (randomizeTreasureMode == RandomizeTreasureMode.DeepDungeon && _flags.DeepDungeonGenerator == DeepDungeonGeneratorMode.Progressive)
			{
				placedItems.AddRange(normalTreasures);
			}
			else
			{
				treasurePool.Shuffle(rng);
				itemLocationPool.Shuffle(rng);

				var leftovers = treasurePool.Zip(itemLocationPool, (treasure, location) => NewItemPlacement(location, treasure));
				placedItems.AddRange(leftovers);
			}

			//chest order placement
			var chestOrderPool = treasurePool;
			if (_flags.ShardHunt)
			{
				//add the shards back into the chest order pool
				chestOrderPool = chestOrderPool.Concat(shards).ToList();
			}

			chestOrderPool.Shuffle(rng);

			for (int i = 0; i < chestOrderPool.Count; i++)
			{	
				rom.Put(FF1Rom.TreasureChestOrderOffset + i, new byte[] { (byte)chestOrderPool[i] });
			}

			return placedItems;
		}
		*/
		private void SetIncentiveChestItemsFanfare()
		{
			if (!flags.IncentiveChestItemsFanfare)
			{
				return;
			}

			foreach (var placedItem in jingleChests)
			{
				rom.Put(placedItem.Address - TreasureOffset + TreasureJingleOffset, new byte[] { (byte)placedItem.Item });
			}
		}
		private void SetChestsOrder(List<Item> chestOrderPool, MT19337 rng)
		{
			if (!flags.OpenChestsInOrder)
			{
				return;
			}

			//chest order placement
			//var chestOrderPool = treasurePool;
			if (flags.ShardHunt)
			{
				//add the shards back into the chest order pool
				chestOrderPool = chestOrderPool.Concat(placementContext.Shards).ToList();
			}

			chestOrderPool.Shuffle(rng);

			for (int i = 0; i < chestOrderPool.Count; i++)
			{
				rom.Put(TreasureChestOrderOffset + i, new byte[] { (byte)chestOrderPool[i] });
			}
		}
		

		public Item SelectVendorItem(List<Item> incentives, List<Item> nonincentives, List<Item> treasurePool, IEnumerable<IRewardSource> incentiveLocationPool, MT19337 rng)
		{
			if (!(bool)flags.NPCItems) return Item.Bottle;

			var itemShopItem = Item.Cabin;
			var validShopIncentives = incentives.Where(x => x > Item.None && x <= Item.Soft).ToList();
			if (validShopIncentives.Any() && incentiveLocationPool.Any(x => x.Address == ItemLocations.CaravanItemShop1.Address))
			{
				itemShopItem = validShopIncentives.PickRandom(rng);
				incentives.Remove(itemShopItem);
			}
			else
			{
				itemShopItem = treasurePool.Concat(nonincentives).Where(x => x > Item.None && x <= Item.Soft && x != Item.Shard).ToList().PickRandom(rng);
				if (!nonincentives.Remove(itemShopItem))
				{
					treasurePool.Remove(itemShopItem);
				}
			}

			return itemShopItem;
		}

		public static IRewardSource NewItemPlacement(IRewardSource copyFromSource, Item newItem)
		{
			if (copyFromSource is NpcReward)
			{
				return new NpcReward(copyFromSource as NpcReward, newItem);
			}
			else if (copyFromSource is ItemShopSlot)
			{
				return new ItemShopSlot(copyFromSource as ItemShopSlot, newItem);
			}
			else
			{
				return new TreasureChest(copyFromSource, newItem);
			}
		}

		protected List<IRewardSource> IncentivizedDungeons(List<IRewardSource> preBlackOrbUnincentivizedLocationPool)
		{
			var placedDungeons = new List<IRewardSource>();

			foreach (var incentiveLocation in placementContext.IncentiveLocations.ToList())
			{
				if (incentiveLocation.GetType().Equals(typeof(TreasureChest)))
					placedDungeons.AddRange(placementContext.AllValidPreBlackOrbItemLocations.Where(x => ItemLocations.MapLocationToOverworldLocations[x.MapLocation] == ItemLocations.MapLocationToOverworldLocations[incentiveLocation.MapLocation] && x.GetType().Equals(typeof(TreasureChest))));
			}

			return preBlackOrbUnincentivizedLocationPool.Where(x => !placedDungeons.Contains(x)).ToList();
		}
	}

	public enum KeyItemPlacementMode
	{
		[Description("Guided")]
		Vanilla,

		[Description("Predictive")]
		Predictive,

		[Description("Predictive Unsafe")]
		PredictiveUnsafe
	}

	public enum LoosePlacementMode
	{
		[Description("Chaotic")]
		Vanilla,

		[Description("Spread")]
		Spread,

		[Description("Forward")]
		Forward
	}
}
