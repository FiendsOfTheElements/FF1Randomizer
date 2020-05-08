using System;
using System.Collections.Generic;
using System.Linq;

namespace FF1Lib
{
	public static class MysterySeedGeneratorLib
	{
		public static FlagsAndInfo GenerateMysterySeed(Weights weights, int seed)
		{
			var random = new Random(seed);

			var shops = false;
			var randomWares = false;
			var randomWaresIncludesSpecialGear = false;
			switch (GetEnumFromWeights(weights.Shops, random))
			{
				case ShopType.Vanilla:
					break;
				case ShopType.OnlyShops:
					shops = true;
					break;
				case ShopType.ShopsWithCustomGear:
					shops = true;
					randomWares = true;
					break;
				case ShopType.ShopsWithEliteGear:
					shops = true;
					randomWares = true;
					randomWaresIncludesSpecialGear = true;
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}

			var magicShops = false;
			var magicShopLocs = false;
			var magicLevels = false;
			var magicPermissions = false;
			switch (GetEnumFromWeights(weights.Magic, random))
			{
				case MagicType.Vanilla:
					break;
				case MagicType.MagicLevelsOnly:
					magicLevels = true;
					break;
				case MagicType.KeepPermissionsOnly:
					magicLevels = true;
					magicPermissions = true;
					break;
				case MagicType.MagicLevelsAndShops:
					magicLevels = true;
					magicShops = true;
					break;
				case MagicType.MagicLevelsAndLocations:
					magicLevels = true;
					magicShopLocs = true;
					break;
				case MagicType.MagicLevelsAndShopsAndLocations:
					magicLevels = true;
					magicShops = true;
					magicShopLocs = true;
					break;
				case MagicType.KeepPermissionsAndShops:
					magicLevels = true;
					magicPermissions = true;
					magicShops = true;
					break;
				case MagicType.KeepPermissionsAndLocations:
					magicLevels = true;
					magicPermissions = true;
					magicShopLocs = true;
					break;
				case MagicType.KeepPermissionsAndShopsAndLocations:
					magicLevels = true;
					magicPermissions = true;
					magicShops = true;
					magicShopLocs = true;
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}

			var enemyScripts = false;
			var allowUnsafePirates = false;
			var bossScriptsOnly = false;
			switch (GetEnumFromWeights(weights.Enemies.EnemyScripts, random))
			{
				case EnemyScriptType.Vanilla:
					break;
				case EnemyScriptType.Scripts:
					enemyScripts = true;
					break;
				case EnemyScriptType.UnsafePirates:
					enemyScripts = true;
					allowUnsafePirates = true;
					break;
				case EnemyScriptType.OnlyShuffleBosses:
					enemyScripts = true;
					bossScriptsOnly = true;
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}

			var enemySkillsSpells = false;
			var bossSkillsSpellsOnly = false;
			switch (GetEnumFromWeights(weights.Enemies.EnemySkillsSpells, random))
			{
				case EnemySkillsSpellsType.Vanilla:
					break;
				case EnemySkillsSpellsType.SkillsSpells:
					enemySkillsSpells = true;
					break;
				case EnemySkillsSpellsType.OnlyShuffleBosses:
					enemySkillsSpells = true;
					bossSkillsSpellsOnly = true;
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}

			var statusAttacks = false;
			var randomStatusattacks = false;
			switch (GetEnumFromWeights(weights.Enemies.EnemyStatusAttacks, random))
			{
				case EnemyStatusAttackType.Vanilla:
					break;
				case EnemyStatusAttackType.StatusAttacks:
					statusAttacks = true;
					break;
				case EnemyStatusAttackType.StatusAttackAilments:
					statusAttacks = true;
					randomStatusattacks = true;
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}

			var unnrunnableFormations = false;
			var allUnrunnable = false;
			switch (GetEnumFromWeights(weights.Enemies.EnemyUnrunnableFormations, random))
			{
				case EnemyUnrunnableFormationType.Vanilla:
					break;
				case EnemyUnrunnableFormationType.EnemyUnrunnableFormations:
					unnrunnableFormations = true;
					break;
				case EnemyUnrunnableFormationType.AllUnrunnable:
					unnrunnableFormations = true;
					allUnrunnable = true;
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}

			var enemyTrapTiles = false;
			var randomTrapFormations = false;
			switch (GetEnumFromWeights(weights.Enemies.EnemyForcedEncounterTiles, random))
			{
				case EnemyForcedEncounterTilesType.Vanilla:
					break;
				case EnemyForcedEncounterTilesType.EnemyForcedEncounterTiles:
					enemyTrapTiles = true;
					break;
				case EnemyForcedEncounterTilesType.RandomFormations:
					enemyTrapTiles = true;
					randomTrapFormations = true;
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}

			var treasures = GetBasedOnWeight(weights.Treasures.Treasures, random);
			var randomLoot = false;
			var worldWealth = WorldWealthMode.Normal;
			var betterTrapChests = false;
			if (treasures)
			{
				switch (GetEnumFromWeights(weights.Treasures.WealthLevel, random))
				{
					case WorldWealthTypes.Vanilla:
						break;
					case WorldWealthTypes.High:
						randomLoot = true;
						worldWealth = WorldWealthMode.High;
						break;
					case WorldWealthTypes.Normal:
						randomLoot = true;
						break;
					case WorldWealthTypes.Low:
						randomLoot = true;
						worldWealth = WorldWealthMode.Low;
						break;
					case WorldWealthTypes.Impoverished:
						randomLoot = true;
						worldWealth = WorldWealthMode.Impoverished;
						break;
					case WorldWealthTypes.Melmond:
						randomLoot = true;
						worldWealth = WorldWealthMode.Melmond;
						break;
					default:
						throw new ArgumentOutOfRangeException();
				}

				betterTrapChests = GetBasedOnWeight(weights.Treasures.BetterTrapTreasure, random);
			}

			// Start Incentivize Section
			// Determine free items
			var freeItems = new List<FreeItemTypes>();
			var freeShip = GetFreeValue(weights.Incentives.FreeItems, FreeItemTypes.Ship, ref freeItems, random);
			var freeAirship = GetFreeValue(weights.Incentives.FreeItems, FreeItemTypes.Airship, ref freeItems, random);
			var freeBridge = GetFreeValue(weights.Incentives.FreeItems, FreeItemTypes.Bridge, ref freeItems, random);
			var freeCanal = GetFreeValue(weights.Incentives.FreeItems, FreeItemTypes.Canal, ref freeItems, random);


			// Determine number of allowable loose
			var guaranteedLocationsCount = weights.Incentives.GuaranteedIncentiveLocations
				.ConvertAll(ConvertToIncentiveLocationCount).Sum();
			var guaranteedIncentiveCount = weights.Incentives.GuaranteedIncentiveItems
				.ConvertAll(ConvertToIncentiveItemCount(freeItems)).Sum();
			var possibleAdditionalItems = weights.Incentives.IncentiveItemWeights
				.Where(kvp => kvp.Value > 0)
				.Select(kvp => ConvertToIncentiveItemCount(freeItems, kvp.Key))
				.Sum();
			var possibleAdditionalLocations = weights.Incentives.IncentiveLocationWeights
				.Where(kvp => kvp.Value > 0)
				.Select(kvp => ConvertToIncentiveLocationCount(kvp.Key))
				.Sum();

			var maxLoose = guaranteedIncentiveCount + possibleAdditionalItems - guaranteedLocationsCount;
			var minLoose = guaranteedIncentiveCount - guaranteedLocationsCount - possibleAdditionalLocations;
			var maxTrash = guaranteedLocationsCount + possibleAdditionalLocations - guaranteedIncentiveCount;
			var minTrash = guaranteedLocationsCount - guaranteedIncentiveCount - possibleAdditionalItems;

			var allowedLooseCounts = new HashSet<LooseItemCounts>();
			if (maxLoose >= 3)
			{
				allowedLooseCounts.Add(LooseItemCounts.Loose3);
			}
			if (maxLoose >= 2)
			{
				allowedLooseCounts.Add(LooseItemCounts.Loose2);
			}
			if (maxLoose >= 1)
			{
				allowedLooseCounts.Add(LooseItemCounts.Loose1);
			}
			if (minLoose <= 0 && 0 <= maxLoose && minTrash <= 0 && maxTrash >= 0)
			{
				allowedLooseCounts.Add(LooseItemCounts.None);
			}
			if (maxTrash >= 1)
			{
				allowedLooseCounts.Add(LooseItemCounts.Trash1);
			}
			if (maxTrash >= 2)
			{
				allowedLooseCounts.Add(LooseItemCounts.Trash2);
			}

			var allowedLooseItemWeights = weights.Incentives.LooseItems.Where(x => allowedLooseCounts.Contains(x.Key)).ToDictionary(x => x.Key, x => x.Value);
			var looseCount = 0;
			switch (GetEnumFromWeights(allowedLooseItemWeights, random))
			{
				case LooseItemCounts.Trash2:
					looseCount = -2;
					break;
				case LooseItemCounts.Trash1:
					looseCount = -1;
					break;
				case LooseItemCounts.None:
					looseCount = 0;
					break;
				case LooseItemCounts.Loose1:
					looseCount = 1;
					break;
				case LooseItemCounts.Loose2:
					looseCount = 2;
					break;
				case LooseItemCounts.Loose3:
					looseCount = 3;
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}

			// Determine incentive count and location count
			var maxHypotheticalIncentiveCount = guaranteedIncentiveCount + possibleAdditionalItems;
			var maxLocationCount = guaranteedLocationsCount + possibleAdditionalLocations;
			var maxIncentiveCount = maxHypotheticalIncentiveCount;
			if (maxHypotheticalIncentiveCount >= looseCount + maxLocationCount)
			{
				maxIncentiveCount = looseCount + maxLocationCount;
			}

			var minIncentiveCount = guaranteedIncentiveCount;
			if (guaranteedIncentiveCount <= looseCount + guaranteedLocationsCount)
			{
				minIncentiveCount = looseCount + guaranteedLocationsCount;
			}
			var incentiveCount = GetRandomNumber(random, maxIncentiveCount, minIncentiveCount);
			var locationCount = incentiveCount - looseCount;

			// Determine Incentive Items
			var incentivizeMainProgressionItems = true;
			var incentivizeOtherQuestItems = true;
			var incentivizeAirship = false;
			var incentivizeShipAndCanal = false;
			var incentivizeCanoe = false;
			var incentivizeTail = false;
			var incentivizeMasamune = false;
			var incentivizeVorpal = false;
			var incentivizeDefense = false;
			var incentivizeThorHammer = false;
			var incentivizeOpalBracelet = false;
			var incentivizePowerBonk = false;
			var incentivizeWhiteShirt = false;
			var incentivizeBlackShirt = false;
			var incentivizeRibbon = false;

			void UpdateFlagForIncentive(Incentives incentive)
			{
				switch (incentive)
				{
					case Incentives.Floater:
						incentivizeAirship = true;
						break;
					case Incentives.Canoe:
						incentivizeCanoe = true;
						break;
					case Incentives.Tail:
						incentivizeTail = true;
						break;
					case Incentives.Masamune:
						incentivizeMasamune = true;
						break;
					case Incentives.Vorpal:
						incentivizeVorpal = true;
						break;
					case Incentives.Defense:
						incentivizeDefense = true;
						break;
					case Incentives.ThorHammer:
						incentivizeThorHammer = true;
						break;
					case Incentives.OpalBracelet:
						incentivizeOpalBracelet = true;
						break;
					case Incentives.PowerGauntlet:
						incentivizePowerBonk = true;
						break;
					case Incentives.WhiteShirt:
						incentivizeWhiteShirt = true;
						break;
					case Incentives.BlackShirt:
						incentivizeBlackShirt = true;
						break;
					case Incentives.Ribbon:
						incentivizeRibbon = true;
						break;
					case Incentives.ShipCanal:
						incentivizeShipAndCanal = true;
						break;
					case Incentives.MainProgressionItems:
						incentivizeMainProgressionItems = true;
						break;
					case Incentives.OtherQuestItems:
						incentivizeOtherQuestItems = true;
						break;
					default:
						throw new ArgumentOutOfRangeException();
				}
			}

			var addedIncentivesCount = 0;
			var addedIncentives = new HashSet<Incentives>();
			// For now have to have MainProgression and OtherQuestItems in pool
			if (!weights.Incentives.GuaranteedIncentiveItems.Contains(Incentives.MainProgressionItems))
			{
				weights.Incentives.GuaranteedIncentiveItems.Add(Incentives.MainProgressionItems);
			}
			if (!weights.Incentives.GuaranteedIncentiveItems.Contains(Incentives.OtherQuestItems))
			{
				weights.Incentives.GuaranteedIncentiveItems.Add(Incentives.OtherQuestItems);
			}

			foreach (var incentive in weights.Incentives.GuaranteedIncentiveItems)
			{
				if (weights.Incentives.IncentiveItemWeights.Keys.Contains(incentive))
				{
					throw new Exception("Cannot have an item guaranteed and weighted");
				}
				addedIncentives.Add(incentive);
				addedIncentivesCount += ConvertToIncentiveItemCount(freeItems, incentive);
				UpdateFlagForIncentive(incentive);
			}

			var remainingIncentiveWeights = new Dictionary<Incentives, int>(weights.Incentives.IncentiveItemWeights);
			while (addedIncentivesCount < incentiveCount)
			{
				foreach (var incentive in remainingIncentiveWeights.Keys.Where(incentive => addedIncentivesCount + ConvertToIncentiveItemCount(freeItems, incentive) > incentiveCount))
				{
					remainingIncentiveWeights.Remove(incentive);
				}

				var chosenIncentive = GetEnumFromWeights(remainingIncentiveWeights, random);
				addedIncentives.Add(chosenIncentive);
				addedIncentivesCount += ConvertToIncentiveItemCount(freeItems, chosenIncentive);
				remainingIncentiveWeights.Remove(chosenIncentive);
				UpdateFlagForIncentive(chosenIncentive);
			}

			// Determine Incentive Locations
			var incentivizeFreeNPCs = true;
			var incentivizeFetchNPCs = true;
			var incentivizeIceCave = false;
			var incentivizeMarshCave = false;
			var incentivizeOrdeals = false;
			var incentivizeTitansTrove = false;
			var incentivizeEarthCave  = false;
			var incentivizeVolcano = false;
			var incentivizeSeaShrine = false;
			var incentivizeSkyPalace = false;
			var incentivizeCorneriaLocked = false;
			var incentivizeMarshLocked = false;

			void UpdateFlagForIncentiveLocation(IncentiveLocations location)
			{
				switch (location)
				{
					case IncentiveLocations.MainNPCs:
						incentivizeFreeNPCs = true;
						break;
					case IncentiveLocations.FetchQuestNPCs:
						incentivizeFetchNPCs = true;
						break;
					case IncentiveLocations.IceCave:
						incentivizeIceCave = true;
						break;
					case IncentiveLocations.Ordeals:
						incentivizeOrdeals = true;
						break;
					case IncentiveLocations.MarshCave:
						incentivizeMarshCave = true;
						break;
					case IncentiveLocations.TitansTrove:
						incentivizeTitansTrove = true;
						break;
					case IncentiveLocations.EarthCave:
						incentivizeEarthCave = true;
						break;
					case IncentiveLocations.Volcano:
						incentivizeVolcano = true;
						break;
					case IncentiveLocations.SeaShrine:
						incentivizeSeaShrine = true;
						break;
					case IncentiveLocations.SkyPalace:
						incentivizeSkyPalace = true;
						break;
					case IncentiveLocations.ConeriaLocked:
						incentivizeCorneriaLocked = true;
						break;
					case IncentiveLocations.MarshLocked:
						incentivizeMarshLocked = true;
						break;
					default:
						throw new ArgumentOutOfRangeException(nameof(location), location, null);
				}
			}

			var addedIncentiveLocationCount = 0;
			var addedIncentiveLocations = new HashSet<IncentiveLocations>();
			// For now have to have MainNPCs and FetchQuestNPC in pool
			if (!weights.Incentives.GuaranteedIncentiveLocations.Contains(IncentiveLocations.MainNPCs))
			{
				weights.Incentives.GuaranteedIncentiveLocations.Add(IncentiveLocations.MainNPCs);
			}
			if (!weights.Incentives.GuaranteedIncentiveLocations.Contains(IncentiveLocations.FetchQuestNPCs))
			{
				weights.Incentives.GuaranteedIncentiveLocations.Add(IncentiveLocations.FetchQuestNPCs);
			}

			foreach (var location in weights.Incentives.GuaranteedIncentiveLocations)
			{
				if (weights.Incentives.IncentiveLocationWeights.Keys.Contains(location))
				{
					throw new Exception("Cannot have an location guaranteed and weighted");
				}
				addedIncentiveLocations.Add(location);
				addedIncentiveLocationCount += ConvertToIncentiveLocationCount(location);
				UpdateFlagForIncentiveLocation(location);
			}

			var remainingIncentiveLocationWeights = new Dictionary<IncentiveLocations, int>(weights.Incentives.IncentiveLocationWeights);
			while (addedIncentiveLocationCount < locationCount)
			{
				foreach (var incentiveLocation in remainingIncentiveLocationWeights.Keys.Where(incentive => addedIncentiveLocationCount + ConvertToIncentiveLocationCount( incentive) > locationCount))
				{
					remainingIncentiveLocationWeights.Remove(incentiveLocation);
				}

				var chosenIncentiveLocation = GetEnumFromWeights(remainingIncentiveLocationWeights, random);
				addedIncentiveLocations.Add(chosenIncentiveLocation);
				addedIncentiveLocationCount += ConvertToIncentiveLocationCount(chosenIncentiveLocation);
				remainingIncentiveLocationWeights.Remove(chosenIncentiveLocation);
				UpdateFlagForIncentiveLocation(chosenIncentiveLocation);
			}
			var freeOrbs = false;
			var shardHunt = false;
			var shardCount = ShardCount.Count16;
			switch (GetEnumFromWeights(weights.Orbs, random))
			{
				case OrbType.Vanilla:
					break;
				case OrbType.ShardHunt16:
					shardHunt = true;
					shardCount = ShardCount.Count16;
					break;
				case OrbType.ShardHunt20:
					shardHunt = true;
					shardCount = ShardCount.Count20;
					break;
				case OrbType.ShardHunt24:
					shardHunt = true;
					shardCount = ShardCount.Count24;
					break;
				case OrbType.ShardHunt28:
					shardHunt = true;
					shardCount = ShardCount.Count28;
					break;
				case OrbType.ShardHunt32:
					shardHunt = true;
					shardCount = ShardCount.Count32;
					break;
				case OrbType.ShardHunt36:
					shardHunt = true;
					shardCount = ShardCount.Count36;
					break;
				case OrbType.FreeOrbs:
					freeOrbs = true;
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}

			var shortToFR = false;
			var fiendTiles = false;
			switch (GetEnumFromWeights(weights.TempleOfFiends.TempleOfFiendsLength, random))
			{
				case ToFRType.Vanilla:
					break;
				case ToFRType.Shortened:
					shortToFR = true;
					break;
				case ToFRType.ShortenedWithFiendTiles:
					shortToFR = true;
					fiendTiles = true;
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}

			var entrances = false;
			var floors = false;
			var towns = false;
			var deepTowns = false;
			var mixedEntrances = false;
			switch (GetEnumFromWeights(weights.EntranceShuffle, random))
			{
				case EntranceShuffleType.Vanilla:
					break;
				case EntranceShuffleType.Entrances:
					entrances = true;
					break;
				case EntranceShuffleType.Floors:
					entrances = true;
					floors = true;
					break;
				case EntranceShuffleType.EntrancesAndTownsSeparate:
					entrances = true;
					towns = true;
					break;
				case EntranceShuffleType.EntrancesAndTownsTogether:
					entrances = true;
					towns = true;
					mixedEntrances = true;
					break;
				case EntranceShuffleType.FloorsAndTownsSeparate:
					entrances = true;
					floors = true;
					towns = true;
					break;
				case EntranceShuffleType.FloorsAndTownsTogether:
					entrances = true;
					floors = true;
					towns = true;
					mixedEntrances = true;
					break;
				case EntranceShuffleType.FloorsAndTownsWithDeepTowns:
					entrances = true;
					floors = true;
					towns = true;
					mixedEntrances = true;
					deepTowns = true;
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}

			var priceScale = GetRandomNumberFromScale(weights.Scaling.Price, random);
			var priceClamped = GetBasedOnWeight(weights.Scaling.Price.Clamped, random);
			var enemiesScale = GetRandomNumberFromScale(weights.Scaling.Enemies, random);
			var enemiesClamped = GetBasedOnWeight(weights.Scaling.Enemies.Clamped, random);
			var bossesScale =  GetRandomNumberFromScale(weights.Scaling.Bosses, random);
			var bossesClamped = GetBasedOnWeight(weights.Scaling.Bosses.Clamped, random);

			var expMultiplier = GetRandomNumberFromScale(weights.Scaling.ExpGold.Multiplier, random);
			var expAddedValue = GetRandomNumberFromScale(weights.Scaling.ExpGold.AddedValue, random);

			var progressiveScaleMode = ProgressiveScaleMode.Disabled;
			switch (GetEnumFromWeights(weights.Scaling.ExpGold.Progressive, random))
			{
				case ProgressiveType.PerItem5:
					progressiveScaleMode = ProgressiveScaleMode.Progressive5Percent;
					break;
				case ProgressiveType.PerItem10:
					progressiveScaleMode = ProgressiveScaleMode.Progressive10Percent;
					break;
				case ProgressiveType.PerItem20:
					progressiveScaleMode = ProgressiveScaleMode.Progressive20Percent;
					break;
				case ProgressiveType.PerOrb12:
					progressiveScaleMode = ProgressiveScaleMode.OrbProgressiveSlow;
					break;
				case ProgressiveType.PerOrb25:
					progressiveScaleMode = ProgressiveScaleMode.OrbProgressiveMedium;
					break;
				case ProgressiveType.PerOrb50:
					progressiveScaleMode = ProgressiveScaleMode.OrbProgressiveFast;
					break;
				case ProgressiveType.PerOrb100:
					progressiveScaleMode = ProgressiveScaleMode.OrbProgressiveVFast;
					break;
			}

			var encounterRate = GetRandomNumberFromScale(weights.Scaling.EncounterRate.Overworld, random) * 30;
			var dungeonEncounterRate = GetRandomNumberFromScale(weights.Scaling.EncounterRate.Dungeon, random) * 30;

			var force1 = false;
			var force2 = false;
			var force3 = false;
			var force4 = false;

			switch (GetEnumFromWeights(weights.Party.Forced, random))
			{
				case ForcedTypes.Four:
					force4 = true;
					force3 = true;
					force2 = true;
					force1 = true;
					break;
				case ForcedTypes.Three:
					force3 = true;
					force2 = true;
					force1 = true;
					break;
				case ForcedTypes.Two:
					force2 = true;
					force1 = true;
					break;
				case ForcedTypes.One:
					force1 = true;
					break;
			}

			var flags = new Flags
			{
				// Always shuffle NPCItems and FetchItems for ease of understanding
				NPCItems = true,
				NPCFetchItems = true,
				Spoilers = weights.Spoilers,

				TournamentSafe = weights.TournamentSafe,

				Shops = shops,
				RandomWares = randomWares,
				RandomWaresIncludesSpecialGear = randomWaresIncludesSpecialGear,

				MagicShops = magicShops,
				MagicShopLocs = magicShopLocs,
				MagicLevels = magicLevels,
				MagicPermissions = magicPermissions,

				ItemMagic = GetBasedOnWeight(weights.ItemMagic, random),

				Rng = GetBasedOnWeight(weights.RngTable, random),

				EnemyScripts = enemyScripts,
				BossScriptsOnly = bossScriptsOnly,
				AllowUnsafePirates = allowUnsafePirates,

				EnemySkillsSpells = enemySkillsSpells,
				BossSkillsOnly = bossSkillsSpellsOnly,

				EnemyStatusAttacks = statusAttacks,
				RandomStatusAttacks = randomStatusattacks,

				EnemyFormationsUnrunnable = unnrunnableFormations,
				EverythingUnrunnable = allUnrunnable,

				UnrunnablesStrikeFirstAndSurprise = GetBasedOnWeight(weights.Enemies.UnrunnableFirstStrikeSurprise, random),

				EnemyFormationsSurprise = GetBasedOnWeight(weights.Enemies.EnemySurpriseBonus, random),

				FormationShuffleMode = GetEnumFromWeights(weights.Enemies.EnemyFormations, random),

				EnemyTrapTiles = enemyTrapTiles,
				RandomTrapFormations = randomTrapFormations,

				WarMECHMode = GetEnumFromWeights(weights.WarMECH, random),

				Treasures = treasures,
				WorldWealth = worldWealth,
				RandomLoot = randomLoot,
				BetterTrapChests = betterTrapChests,

				ShuffleObjectiveNPCs = GetBasedOnWeight(weights.Incentives.ObjectiveNpcs, random),

				FreeShip = freeShip,
				FreeAirship = freeAirship,
				FreeBridge = freeBridge,
				FreeCanal = freeCanal,

				IncentivizeMainItems = incentivizeMainProgressionItems,
				IncentivizeFetchItems = incentivizeOtherQuestItems,
				IncentivizeAirship = incentivizeAirship,
				IncentivizeShipAndCanal = incentivizeShipAndCanal,
				IncentivizeCanoeItem = incentivizeCanoe,
				IncentivizeTail = incentivizeTail,
				IncentivizeMasamune = incentivizeMasamune,
				IncentivizeVorpal = incentivizeVorpal,
				IncentivizeDefCastWeapon = incentivizeDefense,
				IncentivizeOffCastWeapon = incentivizeThorHammer,
				IncentivizeOpal = incentivizeOpalBracelet,
				IncentivizeOtherCastArmor = incentivizePowerBonk,
				IncentivizeDefCastArmor = incentivizeWhiteShirt,
				IncentivizeOffCastArmor = incentivizeBlackShirt,
				IncentivizeRibbon = incentivizeRibbon,

				IncentivizeFreeNPCs = incentivizeFreeNPCs,
				IncentivizeFetchNPCs = incentivizeFetchNPCs,
				IncentivizeIceCave = incentivizeIceCave,
				IncentivizeMarsh = incentivizeMarshCave,
				IncentivizeOrdeals = incentivizeOrdeals,
				IncentivizeTitansTrove = incentivizeTitansTrove,
				IncentivizeEarth = incentivizeEarthCave,
				IncentivizeVolcano = incentivizeVolcano,
				IncentivizeSeaShrine = incentivizeSeaShrine,
				IncentivizeSkyPalace = incentivizeSkyPalace,
				IncentivizeConeria = incentivizeCorneriaLocked,
				IncentivizeMarshKeyLocked = incentivizeMarshLocked,

				FreeOrbs = freeOrbs,
				ShardHunt = shardHunt,
				ShardCount = shardCount,

				TransformFinalFormation = GetBasedOnWeight(weights.TempleOfFiends.AlternateBoss, random),
				ShortToFR = shortToFR,
				PreserveFiendRefights = fiendTiles,

				Entrances = entrances,
				Floors = floors,
				Towns = towns,
				AllowDeepTowns = deepTowns,
				EntrancesMixedWithTowns = mixedEntrances,

				// TODO randomize allowed classes
				FIGHTER1 = true,
				THIEF1 = true,
				BLACK_BELT1 = true,
				RED_MAGE1 = true,
				WHITE_MAGE1 = true,
				BLACK_MAGE1 = true,
				FORCED1 = force1,

				FIGHTER2 = true,
				THIEF2 = true,
				BLACK_BELT2 = true,
				RED_MAGE2 = true,
				WHITE_MAGE2 = true,
				BLACK_MAGE2 = true,
				NONE_CLASS2 = true,
				FORCED2 = force2,

				FIGHTER3 = true,
				THIEF3 = true,
				BLACK_BELT3 = true,
				RED_MAGE3 = true,
				WHITE_MAGE3 = true,
				BLACK_MAGE3 = true,
				NONE_CLASS3 = true,
				FORCED3 = force3,

				FIGHTER4 = true,
				THIEF4 = true,
				BLACK_BELT4 = true,
				RED_MAGE4 = true,
				WHITE_MAGE4 = true,
				BLACK_MAGE4 = true,
				NONE_CLASS4 = true,
				FORCED4 = force4,

				// Set some to true so the logic can be consistent while this is a WIP
				OrdealsPillars = true,
				TitansTrove = true,
				LefeinShops = true,
				MapOpenProgression = true,
				MapOpenProgressionDocks = true,
				EarlySarda = true,
				EarlySage = true,
				EarlyOrdeals = true,

				// Always bugfix and provide conveniences
				WrapPriceOverflow = true,
				WrapStatOverflow = true,
				IncludeMorale = true,
				NoDanMode = false,
				StartingGold = false,
				EasyMode = false,

				NoPartyShuffle = true,
				SpeedHacks = true,
				IdentifyTreasures = true,
				Dash = true,
				BuyTen = true,
				WaitWhenUnrunnable = true,
				EnableCritNumberDisplay = true,

				HouseMPRestoration = true,
				WeaponStats = true,
				ChanceToRun = true,
				SpellBugs = true,
				EnemyStatusAttackBug = true,
				BlackBeltAbsorb = true,
				EnemyElementalResistancesBug = true,
				EnemySpellsTargetingAllies = true,
				ImproveTurnOrderRandomization = true,
				FixHitChanceCap = true,

				BBCritRate = true,
				WeaponCritRate = true,
				WeaponBonuses = true,
				HousesFillHp = true,
				ThiefHitRate = true,
				MDefMode = MDEFGrowthMode.None,
				RebalanceSpells = true, //LOCK always hits

				PriceScaleFactor = priceScale,
				ClampMinimumPriceScale = priceClamped,
				EnemyScaleFactor = enemiesScale,
				ClampMinimumStatScale = enemiesClamped,
				BossScaleFactor = bossesScale,
				ClampMinimumBossStatScale = bossesClamped,
				ExpMultiplier = expMultiplier,
				ExpBonus = expAddedValue,
				ProgressiveScaleMode = progressiveScaleMode,
				EncounterRate = encounterRate,
				DungeonEncounterRate = dungeonEncounterRate,
			};
			return new FlagsAndInfo()
			{
				Flags = flags,
				LooseCount = looseCount,
				Incentives = addedIncentives,
				IncentiveLocations = addedIncentiveLocations
			};
		}

		private static double GetRandomNumberFromScale(DoubleScale doubleScale, Random random)
		{
			// Single decimal precision only
			var doubleValue = (double)(GetRandomNumber(random, (int)(doubleScale.Max * 10 + 1), (int)(doubleScale.Min * 10)));
			return doubleValue / 10.0;
		}

		private static int GetRandomNumberFromScale(IntScale intScale, Random random)
		{
			return GetRandomNumber(random, intScale.Max + 1, intScale.Min);
		}

		private static bool GetBasedOnWeight(int weight, Random random)
		{
			if (weight < 0 || weight > 100)
			{
				throw new Exception("Invalid weight");
			}

			return weight > GetRandomNumber(random);
		}

		private static int GetRandomNumber(Random random, int maximum = 100, int minimum = 0)
		{
			return random.Next(minimum, maximum);
		}


		private static T GetEnumFromWeights<T>(Dictionary<T, int> weightDict, Random random) where T : Enum
		{
			var randomNumber = GetRandomNumber(random, weightDict.Values.Sum());
			var total = 0;
			foreach (KeyValuePair<T, int> entry in weightDict)
			{
				total += entry.Value;
				if (randomNumber <= total)
				{
					return entry.Key;
				}
			}
			throw new Exception("Illegal weights passed into dictionary");
		}

		private static int ConvertToIncentiveLocationCount(IncentiveLocations incentiveLocation)
		{
			if (incentiveLocation == IncentiveLocations.FetchQuestNPCs ||
			    incentiveLocation == IncentiveLocations.MainNPCs)
			{
				return 7;
			}

			return 1;
		}

		private static Converter<Incentives, int> ConvertToIncentiveItemCount(List<FreeItemTypes> freeItems)
		{
			return x => ConvertToIncentiveItemCount(freeItems, x);
		}

		private static int ConvertToIncentiveItemCount(List<FreeItemTypes> freeItems, Incentives incentive)
		{
			switch (incentive)
			{
				case Incentives.MainProgressionItems:
					return 6;
				case Incentives.OtherQuestItems:
					return 8;
				case Incentives.Floater:
					return freeItems.Contains(FreeItemTypes.Airship) ? 0 : 1;
				case Incentives.ShipCanal:
					var freeShip = freeItems.Contains(FreeItemTypes.Ship);
					var freeCanal = freeItems.Contains(FreeItemTypes.Canal);

					if (freeShip && freeCanal)
					{
						return 0;
					}
					if (freeShip ||  freeCanal)
					{
						return 1;
					}

					return 2;
				default:
					return 1;
			}
		}

		private static bool GetFreeValue(Dictionary<FreeItemTypes, int> freeItemWeights, FreeItemTypes freeItem, ref List<FreeItemTypes> freeItems, Random random)
		{
			if (freeItemWeights.ContainsKey(freeItem))
			{
				var isFree = GetBasedOnWeight(freeItemWeights[freeItem], random);
				if (isFree)
				{
					freeItems.Add(freeItem);
				}

				return isFree;
			}
			return false;
		}
	}

	public class FlagsAndInfo
	{
		public Flags Flags { get; set; }
		public HashSet<Incentives> Incentives { get; set; }
		public HashSet<IncentiveLocations> IncentiveLocations { get; set; }
		public int LooseCount { get; set; }
	}

	public class Weights
	{
		public bool Spoilers { get; set; } = false;
		public bool TournamentSafe { get; set; } = false;
		public Dictionary<ShopType, int> Shops { get; set; }

		public Dictionary<MagicType, int> Magic { get; set; }
		public int ItemMagic { get; set; } = 0;
		public int RngTable { get; set; } = 100;

		public EnemyWeights Enemies { get; set; }

		public Dictionary<WarMECHMode, int> WarMECH { get; set; }

		public TreasureWeights Treasures { get; set; }

		public IncentiveWeights Incentives { get; set; }
		public Dictionary<OrbType, int> Orbs { get; set; }
		public ToFRWeights TempleOfFiends { get; set; }
		public Dictionary<EntranceShuffleType, int> EntranceShuffle { get; set; }
		public ScalingWeights Scaling { get; set; }
		public PartyWeights Party { get; set; }
	}

	public class EnemyWeights
	{
		public Dictionary<EnemyScriptType, int> EnemyScripts { get; set; }
		public Dictionary<EnemySkillsSpellsType, int> EnemySkillsSpells { get; set; }
		public Dictionary<EnemyStatusAttackType, int> EnemyStatusAttacks { get; set; }
		public Dictionary<EnemyUnrunnableFormationType, int> EnemyUnrunnableFormations { get; set; }
		public int UnrunnableFirstStrikeSurprise { get; set; } = 0;
		public int EnemySurpriseBonus { get; set; } = 0;
		public Dictionary<FormationShuffleMode, int> EnemyFormations { get; set; }
		public Dictionary<EnemyForcedEncounterTilesType, int> EnemyForcedEncounterTiles { get; set; }
	}

	public class TreasureWeights
	{
		public int Treasures { get; set; } = 100;
		public Dictionary<WorldWealthTypes, int> WealthLevel { get; set; } = new Dictionary<WorldWealthTypes, int>();
		public int BetterTrapTreasure { get; set; } = 100;
	}

	public class IncentiveWeights
	{
		public int ObjectiveNpcs { get; set; } = 0;
		public Dictionary<LooseItemCounts, int> LooseItems { get; set; }
		public Dictionary<FreeItemTypes, int> FreeItems { get; set; }
		public List<Incentives> GuaranteedIncentiveItems { get; set; }
		public Dictionary<Incentives, int> IncentiveItemWeights { get; set; }
		public List<IncentiveLocations> GuaranteedIncentiveLocations { get; set; }
		public Dictionary<IncentiveLocations, int> IncentiveLocationWeights { get; set; }
	}

	public class ToFRWeights
	{
		public int AlternateBoss { get; set; } = 0;
		public Dictionary<ToFRType, int> TempleOfFiendsLength { get; set; }
	}

	public class ScalingWeights
	{
		public ClampedScale Price { get; set; }
		public ClampedScale Enemies { get; set; }
		public ClampedScale Bosses { get; set; }
		public ExpGoldScale ExpGold { get; set; }
		public EncounterRateWeights EncounterRate { get; set; }
	}

	public class DoubleScale
	{
		public double Max { get; set; }
		public double Min { get; set; }
	}

	public class IntScale
	{
		public int Max { get; set; }
		public int Min { get; set; }
	}

	public class ClampedScale: DoubleScale
	{
		public int Clamped { get; set; } = 0;
	}

	public class ExpGoldScale
	{
		public DoubleScale Multiplier { get; set; }
		public IntScale AddedValue { get; set; }
		public Dictionary<ProgressiveType, int> Progressive { get; set; }
		public EncounterRateWeights EncounterRate { get; set; }
	}

	public class EncounterRateWeights
	{
		public DoubleScale Overworld { get; set; }
		public DoubleScale Dungeon { get; set; }
	}

	public class PartyWeights
	{
		public Dictionary<ForcedTypes, int> Forced { get; set; }
	}

	public enum ForcedTypes
	{
		None,
		One,
		Two,
		Three,
		Four
	}

	public enum ProgressiveType
	{
		PerItem5,
		PerItem10,
		PerItem20,
		PerOrb12,
		PerOrb25,
		PerOrb50,
		PerOrb100
	}

	public enum EntranceShuffleType
	{
		Vanilla,
		Entrances,
		Floors,
		EntrancesAndTownsSeparate,
		EntrancesAndTownsTogether,
		FloorsAndTownsSeparate,
		FloorsAndTownsTogether,
		FloorsAndTownsWithDeepTowns
	}

	public enum ToFRType
	{
		Vanilla,
		Shortened,
		ShortenedWithFiendTiles
	}

	public enum OrbType
	{
		Vanilla,
		ShardHunt16,
		ShardHunt20,
		ShardHunt24,
		ShardHunt28,
		ShardHunt32,
		ShardHunt36,
		FreeOrbs
	}

	public enum LooseItemCounts
	{
		Trash2,
		Trash1,
		None,
		Loose1,
		Loose2,
		Loose3
	}

	public enum FreeItemTypes
	{
		Ship,
		Airship,
		Bridge,
		Canal
	}

	public enum Incentives
	{
		MainProgressionItems,
		OtherQuestItems,
		Floater,
		Canoe,
		Tail,
		Masamune,
		Vorpal,
		Defense,
		ThorHammer,
		OpalBracelet,
		PowerGauntlet,
		WhiteShirt,
		BlackShirt,
		Ribbon,
		ShipCanal
	}

	public enum IncentiveLocations
	{
		MainNPCs,
		FetchQuestNPCs,
		IceCave,
		Ordeals,
		MarshCave,
		TitansTrove,
		EarthCave,
		Volcano,
		SeaShrine,
		SkyPalace,
		ConeriaLocked,
		MarshLocked
	}

	public enum WorldWealthTypes
	{
		Vanilla,
		High,
		Normal,
		Low,
		Impoverished,
		Melmond
	}

	public enum EnemyScriptType
	{
		Vanilla,
		Scripts,
		UnsafePirates,
		OnlyShuffleBosses,
	}

	public enum EnemySkillsSpellsType
	{
		Vanilla,
		SkillsSpells,
		OnlyShuffleBosses
	}

	public enum EnemyStatusAttackType
	{
		Vanilla,
		StatusAttacks,
		StatusAttackAilments
	}

	public enum EnemyUnrunnableFormationType
	{
		Vanilla,
		EnemyUnrunnableFormations,
		AllUnrunnable
	}

	public enum EnemyForcedEncounterTilesType
	{
		Vanilla,
		EnemyForcedEncounterTiles,
		RandomFormations
	}

	public enum MagicType
	{
		Vanilla,
		MagicLevelsOnly,
		KeepPermissionsOnly,
		MagicLevelsAndShops,
		MagicLevelsAndLocations,
		MagicLevelsAndShopsAndLocations,
		KeepPermissionsAndShops,
		KeepPermissionsAndLocations,
		KeepPermissionsAndShopsAndLocations
	}

	public enum ShopType
	{
		Vanilla,
		OnlyShops,
		ShopsWithCustomGear,
		ShopsWithEliteGear
	}
}
