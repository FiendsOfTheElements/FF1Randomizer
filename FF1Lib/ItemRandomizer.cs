using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using RomUtilities;
using static FF1Lib.ItemLocations;

namespace FF1Lib
{
    public partial class FF1Rom : NesRom
    {

        public void Put(ItemLocation itemToPut)
        {
            var npcItemOffset = ((!itemToPut.IsTreasure &&
                                  !itemToPut.UpdatesVariable &&
                                  itemToPut.Address != CaravanItemShop1.Address)
                                 ? Variables.ItemsBaseForNPC
                                 : 0);
            Put(itemToPut.Address, new byte[] { (byte)(itemToPut.Item + npcItemOffset) });
        }

        public void RandomizeTreasures(MT19337 rng, MapDetails mapDetails, Flags flags)
        {
            PermanentCaravan();
            NormalizeNerrick();
            CheckCanoeItemInsteadOfEventVar();
            var treasurePool = AllTreasures.Where(x => !x.IsUnused).Select(x => x.Item).ToList();
            var incentiveLocations = new List<ItemLocation>();
            if (flags.IncentivizeIceCave) incentiveLocations.Add(IceCaveMajor);
            if (flags.IncentivizeOrdeals) incentiveLocations.Add(OrdealsMajor);
            //if (flags.AdamantMasamune)
            {
                treasurePool.Remove(Items.Masamune);
                treasurePool.Add(Items.Xcalber);
            }
            var incentiveItems = treasurePool.Where(x => new List<byte> { Items.Masamune, Items.Ribbon, Items.Xcalber }.Contains(x)).ToList();
            var newItemLocations = RandomizeTreasures(rng, mapDetails, treasurePool, incentiveLocations, incentiveItems);
            FixCanal(newItemLocations);
            MirrorNPCItemChecksFromTargetItem();
        }

        public IReadOnlyCollection<ItemLocation>
                RandomizeTreasures(MT19337 rng, MapDetails mapDetails,
                                       IReadOnlyCollection<byte> treasurePool,
                                       IReadOnlyCollection<ItemLocation> incentiveLocations,
                                       IReadOnlyCollection<byte> incentiveItems)
        {
            // TODO: Update text for NPCs to show the item actually given
            // TODO: Allow more shop items to randomize
            // TODO: Give option to randomize treasure within ranges of gold, weapons, armor
            var placedItemLocations = new Dictionary<int, ItemLocation>
            {
                // Place vanilla BridgeVis and ShipVis. Logic for shuffling these needs more testing and doens't change much without map alterations
                //{ KingConeria.Address, KingConeria },
                //{ Bikke.Address, Bikke },
                //// Place vanilla CanalVis. More logic needs to be coded and tested to include this in the shuffle without introducing many soft lock scenarios
                //{ Nerrick.Address, Nerrick }
            };
            var placedItems = new Func<List<byte>>(() =>
                                                   placedItemLocations.Values
                                                   .Where(x => x.IsTreasure || !x.UpdatesVariable)
                                                   .Select(x => x.Item).ToList());
            var placedVisibility = new Func<List<byte>>(() =>
                                                   placedItemLocations.Values
                                                   .Where(x => !x.IsTreasure && x.UpdatesVariable)
                                                   .Select(x => x.Item).ToList());
            var mapChangesAcquired = new HashSet<MapChanges>();
            int countMapChanges;
            var mapLocationsAvailable =
                new Func<List<MapLocations>>(() =>
                    mapDetails.LocationRequirements
                                             .Where(x => x.Value(mapChangesAcquired))
                                             .Select(x => x.Key).ToList());
            var checkMapChanges = new Action(() =>
            {
                if (mapChangesAcquired.Count >= Enum.GetValues(typeof(MapChanges)).Length) return;
                var currentItems = placedItems();
                do
                {
                    countMapChanges = mapChangesAcquired.Count;
                    var currentMapLocations = mapLocationsAvailable();
                    if (currentMapLocations.Contains(MapLocations.BridgeLocation) && placedVisibility().Contains(Variables.BridgeVis))
                        mapChangesAcquired.Add(MapChanges.Bridge);
                    if (currentMapLocations.Contains(MapLocations.ShipLocation) && placedVisibility().Contains(Variables.ShipVis))
                        mapChangesAcquired.Add(MapChanges.Ship);
                    if (currentMapLocations.Contains(MapLocations.CanalLocation) && placedVisibility().Contains(Variables.CanalVis))
                        mapChangesAcquired.Add(MapChanges.Canal);
                    if (currentItems.Contains(Items.Canoe))
                        mapChangesAcquired.Add(MapChanges.Canoe);
                    if (currentMapLocations.Contains(MapLocations.AirshipLocation) && currentItems.Contains(Items.Floater))
                        mapChangesAcquired.Add(MapChanges.Airship);
                    if (currentMapLocations.Contains(MapLocations.TitansTunnelA) && currentItems.Contains(Items.Ruby))
                        mapChangesAcquired.Add(MapChanges.TitanFed);
                } while (countMapChanges != mapChangesAcquired.Count);
            });
            checkMapChanges();

            var workingLocationsPool =
                new Func<List<ItemLocation>>(() =>
                {
                    var currentItems = placedItems();
                    var results = AllQuestItemLocations
                        .Where(x => !placedItemLocations.ContainsKey(x.Address)).ToList();
                    if (mapDetails.ItemsRequired.All(y => currentItems.Contains(y)))
                        return results;
                    return results.Where(AvailableLocationSelector(mapLocationsAvailable(), placedItems())).ToList();
                });
            var workingProgressionPool =
                new Func<List<byte>>(() =>
                {
                    var result =
                          Items.AllQuestItems.Except(placedItems())
                               .Where(x => mapDetails.RequiredMapChanges.All(y => mapChangesAcquired.Contains(y)) ||
                                      (x == Items.Canoe && (mapChangesAcquired.Contains(MapChanges.Bridge) || mapChangesAcquired.Contains(MapChanges.Ship))) ||
                                      (x == Items.Floater && mapLocationsAvailable().Contains(MapLocations.AirshipLocation)) ||
                                     IsItemProgression(mapLocationsAvailable(), placedItems(), x))
                               .ToList();
                    return result;
                });
            var incentivePool =
                new Func<List<byte>>(() =>
                {
                    var result = Items.AllQuestItems.Concat(incentiveItems).ToList();
                    foreach (var item in placedItems()) result.Remove(item);
                    return result.Distinct().ToList();
                });
            var variableProgressionPool =
                new Func<List<byte>>(() =>
                {
                    var result =
                        new List<byte> { Variables.BridgeVis, Variables.ShipVis, Variables.CanalVis }
                    .Except(placedVisibility())
                    .Where(x => mapDetails.RequiredMapChanges.All(y => mapChangesAcquired.Contains(y)) ||
                           (x == Variables.BridgeVis && mapLocationsAvailable().Contains(MapLocations.BridgeLocation) &&
                            (!mapDetails.ShipObsoletesBridge || !mapChangesAcquired.Contains(MapChanges.Ship))) ||
                           (x == Variables.ShipVis && mapLocationsAvailable().Contains(MapLocations.ShipLocation)) ||
                           (x == Variables.CanalVis && mapLocationsAvailable().Contains(MapLocations.CanalLocation)))
                    .ToList();
                    return result;
                });
            // Main Loop #1: Aquire all necessary changes of map access
            while (!mapDetails.RequiredMapChanges.All(x => mapChangesAcquired.Contains(x)) || variableProgressionPool().Any())
            {
                var remainingNonTreasuresCount = AllNonTreasureItemLocations.Count - placedItemLocations.Values.Count(x => !x.IsTreasure);
                var possibleVariableLocationsCount = workingLocationsPool().Count(x => !x.IsTreasure && x.Address != CaravanItemShop1.Address);
                var selectedLocation = workingLocationsPool().PickRandom(rng);
                byte selectedItem = 0;
                var updateVariable = false;
                if (workingLocationsPool().Any(x => x.IsTreasure && incentiveLocations.Any(y => y.Address == x.Address)))
                {
                    selectedLocation = workingLocationsPool().First(x => x.IsTreasure && incentiveLocations.Any(y => y.Address == x.Address));
                    selectedItem = incentivePool().PickRandom(rng);
                }
                else if (variableProgressionPool().Any() && possibleVariableLocationsCount > 0 &&
                    (possibleVariableLocationsCount >= workingProgressionPool().Count ||
                     variableProgressionPool().Count >= mapDetails.RequiredMapChanges.Count(x => !mapChangesAcquired.Contains(x))))
                {
                    selectedItem = variableProgressionPool().PickRandom(rng);
                    selectedLocation = workingLocationsPool().Where(x => !x.IsTreasure && x.Address != CaravanItemShop1.Address).ToList().PickRandom(rng);
                    updateVariable = true;
                }
                else if (remainingNonTreasuresCount > 0 &&
                         remainingNonTreasuresCount >= workingLocationsPool().Count(x => !x.IsTreasure) &&
                         selectedLocation.IsTreasure && workingLocationsPool().Any(x => !x.IsTreasure))
                { // We have just enough items left to give the remaining ones to only NonTreasures plus one fetch quest item if needed
                    selectedLocation = workingLocationsPool().Where(x => !x.IsTreasure).ToList().PickRandom(rng);
                }
                if (selectedItem == 0 && !updateVariable)
                    selectedItem = workingProgressionPool().PickRandom(rng);
                var placement = new ItemLocation(selectedLocation, selectedItem, updateVariable);
                placedItemLocations.Add(selectedLocation.Address, placement);
                checkMapChanges();
            }

            // Main Loop #2: Aquire all remaining quest items
            while (workingProgressionPool().Any())
            {
                var remainingNonTreasuresCount = AllNonTreasureItemLocations.Count - placedItemLocations.Values.Count(x => !x.IsTreasure);
                var selectedLocation = workingLocationsPool().PickRandom(rng);
                var selectedItem = workingProgressionPool().PickRandom(rng);
                if (remainingNonTreasuresCount > 0 && remainingNonTreasuresCount >= workingProgressionPool().Count && selectedLocation.IsTreasure)
                { // We have just enough items left to give the remaining ones to only NonTreasures plus one fetch quest item if needed
                    if (!workingLocationsPool().Any(x => !x.IsTreasure))
                    { // we pick an NonTreasure if we didn't already select one in the first place
                        selectedLocation = AllNonTreasureItemLocations.Where(x => !placedItemLocations.Values.Any(y => y.Address == x.Address)).ToList().PickRandom(rng);
                    }
                    else
                    {
                        selectedLocation = workingLocationsPool().Where(x => !x.IsTreasure).ToList().PickRandom(rng);
                    }
                }
                else if (workingLocationsPool().Any(x => incentiveLocations.Any(y => y.Address == x.Address)))
                {
                    selectedLocation = workingLocationsPool().First(x => incentiveLocations.Any(y => y.Address == x.Address));
                    if (selectedLocation.IsTreasure) // If it's a NonTreasure then we can't give masamune or ribbon easily so we leave the item alone
                        selectedItem = incentivePool().PickRandom(rng);
                }
                var placement = new ItemLocation(selectedLocation, selectedItem);
                placedItemLocations.Add(selectedLocation.Address, placement);
                checkMapChanges();
            }
            Debug.Assert(placedItemLocations.Values.Count(x => !x.IsTreasure) == AllNonTreasureItemLocations.Count(),
                         $"There cannot be any NonTreasures left at this point or someone won't have a valid item. \n" +
                         $"Placed: \n{string.Join("\n ", placedItemLocations.Values.Where(x => !x.IsTreasure).Select(x => x.SpoilerText))}\n" +
                         $"Remaining: \n{string.Join("\n ", workingLocationsPool().Where(x => !x.IsTreasure).Select(x => x.SpoilerText))}");

            var treasures = treasurePool.ToList();
            foreach (var placedItem in placedItems()) treasures.Remove(placedItem);
            var remainingLocations = workingLocationsPool();
            Debug.Assert(treasures.Count >= remainingLocations.Count,
                         $"Had {treasures.Count} treasures but expected at least {remainingLocations.Count} treasures.");
            // Cleanup NonProgression Loop
            foreach (var remainingLocation in remainingLocations)
            {
                placedItemLocations.Add(remainingLocation.Address, new ItemLocation(remainingLocation, treasures.SpliceRandom(rng)));
            }

            foreach (var placement in placedItemLocations.Values)
            {
                Put(placement);
                //Console.WriteLine($"{placement.SpoilerText}");
            }
            var expectedPlacements = (256/*treasures*/ - 15/*unused*/ + 13/*NonTreasures*/);
            var actualPlacements = placedItemLocations.Keys.Distinct().Count();
            Debug.Assert(actualPlacements == expectedPlacements,
                         $"placed {actualPlacements} items but expected {expectedPlacements} items.");
            return placedItemLocations.Values;
        }

        private static bool IsItemProgression(List<MapLocations> mapLocationsAvailable, IList<byte> placedItems, byte testItem)
        {
            var countItemLocationsAvailable = AllQuestItemLocations.Count(AvailableLocationSelector(mapLocationsAvailable, placedItems));
            var countItemLocationsAvailableWithTest =
                AllQuestItemLocations.Count(AvailableLocationSelector(mapLocationsAvailable, placedItems.Concat(new[] { testItem }).ToList()));
            return countItemLocationsAvailableWithTest > countItemLocationsAvailable;
        }

        private static Func<ItemLocation, bool> AvailableLocationSelector(List<MapLocations> mapLocationsAvailable, IList<byte> placedItems)
        {
            var haveCrystal = placedItems.Contains(Items.Crystal);
            var haveHerb = placedItems.Contains(Items.Herb);
            var haveCrown = placedItems.Contains(Items.Crown);
            var haveTnt = placedItems.Contains(Items.Tnt);
            var haveRuby = placedItems.Contains(Items.Ruby);
            var haveSlab = placedItems.Contains(Items.Slab);
            var haveBottle = placedItems.Contains(Items.Bottle);
            var haveKey = placedItems.Contains(Items.Key);
            var haveRod = placedItems.Contains(Items.Rod);
            var haveCube = placedItems.Contains(Items.Cube);
            var haveChime = placedItems.Contains(Items.Chime);
            var haveOxyale = placedItems.Contains(Items.Oxyale);
            var haveLute = placedItems.Contains(Items.Lute);
            return x => !x.RequiresBlackOrb &&
                          (haveKey || !x.RequiresKey) &&
                          (haveRod || !x.RequiresRod) &&
                          (haveCube || !x.RequiresCube) &&
                          (haveLute || !x.RequiresLute) &&
                          (haveChime || !x.RequiresChime) &&
                          (haveOxyale || !x.RequiresOxyale) &&
                          (haveCrystal || x.Address != Matoya.Address) &&
                          (haveHerb || x.Address != ElfPrince.Address) &&
                          (haveCrown || x.Address != Astos.Address) &&
                          (haveTnt || x.Address != Nerrick.Address) &&
                          (haveRuby || x.Address != Sarda.Address) &&
                          (haveSlab || x.Address != Lefein.Address) &&
                          (haveBottle || x.Address != Fairy.Address) &&
                          mapLocationsAvailable.Contains(x.MapLocation);
        }

    }
}
