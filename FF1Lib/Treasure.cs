
using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using RomUtilities;
using System.Text;

namespace FF1Lib
{
    public static class TreasureConditions
    {
        public static readonly List<int> UnusedIndices =
            Enumerable.Range(0, 1).Concat(
            Enumerable.Range(145, 4)).Concat(
            Enumerable.Range(187, 9)).Concat(
            Enumerable.Range(255, 1))
            .ToList();

        public static readonly List<int> UsedIndices = Enumerable.Range(0, 256).Except(UnusedIndices).ToList(); // This maps a compacted list back to the game's array, skipping the unused slots.
    }

    public partial class FF1Rom : NesRom
    {
        public const int TreasureOffset = 0x03100;
        public const int TreasureSize = 1;
        public const int TreasureCount = 256;

        public const int lut_MapObjTalkJumpTblAddress = 0x390D3;
        public const string giveRewardRoutineAddress = "93DD";

        //******************************STATS***********************************
        private static Dictionary<IReadOnlyCollection<IRewardSource>, string> 
            incentiveLocationAreas = new Dictionary<IReadOnlyCollection<IRewardSource>, string>
                {
                    { ItemLocations.Coneria, nameof(ItemLocations.Coneria)},
                    { ItemLocations.TempleOfFiends, nameof(ItemLocations.TempleOfFiends)},
                    { ItemLocations.MatoyasCave, nameof(ItemLocations.MatoyasCave)},
                    { ItemLocations.Elfland, nameof(ItemLocations.Elfland)},
                    { ItemLocations.MarshCave, nameof(ItemLocations.MarshCave)},
                    { ItemLocations.NorthwestCastle, nameof(ItemLocations.NorthwestCastle)},
                    { ItemLocations.DwarfCave, nameof(ItemLocations.DwarfCave)},
                    { ItemLocations.EarthCave, nameof(ItemLocations.EarthCave)},
                    { ItemLocations.TitansTunnel, nameof(ItemLocations.TitansTunnel)},
                    { ItemLocations.Volcano, nameof(ItemLocations.Volcano)},
                    { ItemLocations.IceCave, nameof(ItemLocations.IceCave)},
                    { ItemLocations.Ordeals, nameof(ItemLocations.Ordeals)},
                    { ItemLocations.Cardia, nameof(ItemLocations.Cardia)},
                    { ItemLocations.SeaShrine, nameof(ItemLocations.SeaShrine)},
                    { ItemLocations.Waterfall, nameof(ItemLocations.Waterfall)},
                    { ItemLocations.MirageTower, nameof(ItemLocations.MirageTower)},
                    { ItemLocations.SkyPalace, nameof(ItemLocations.SkyPalace)},
                    { ItemLocations.ToFR, nameof(ItemLocations.ToFR)}
                };
        private static string PrintStats(int maxIterations, Dictionary<Item, List<int>> incentiveLocations, Dictionary<Item, List<string>> incentiveZones)
        {
            var sb = new StringBuilder();
            sb.Append("Location         ,");
            foreach (Item item in incentiveLocations.Keys)
            {
                var name = Enum.GetName(typeof(Item), item);
                name = $"{string.Join("", Enumerable.Repeat(" ", Math.Max(1, 9 - name.Length)))}{name},";
                sb.Append(name.Substring(0, Math.Min(12, name.Length)));
            }
            sb.Append("\n");
            foreach (var rewardSource in ItemLocations.AllQuestItemLocations.Where(x => x.Address < 0x80000)) // && x.Address > 0x31FF))
            {
                sb.Append($"{rewardSource.Name}" +
                          $"{string.Join("", Enumerable.Repeat(" ", Math.Max(1, 17 - rewardSource.Name.Length)))},");
                foreach (var itemPlacements in incentiveLocations.Values)
                {
                    var percentage = $"   {100.0 * itemPlacements.Count(x => x == rewardSource.Address) / maxIterations:g2}";
                    percentage = $"{string.Join("", Enumerable.Repeat(" ", Math.Max(1, 9 - percentage.Length)))}{percentage},";
                    sb.Append(percentage);
                }
                sb.Append("\n");
            }
            foreach (var zoneName in incentiveLocationAreas.Values)
            {
                sb.Append($"{zoneName}" +
                          $"{string.Join("", Enumerable.Repeat(" ", Math.Max(1, 17 - zoneName.Length)))},");
                foreach (var itemPlacements in incentiveZones.Values)
                {
                    var percentage = $"   {100.0 * itemPlacements.Count(x => x == zoneName) / maxIterations:g2}";
                    percentage = $"{string.Join("", Enumerable.Repeat(" ", Math.Max(1, 9 - percentage.Length)))}{percentage},";
                    sb.Append(percentage);
                }
                sb.Append("\n");
            }
            return sb.ToString();
        }
        //***************************END STATS**********************************

        public void ShuffleTreasures(MT19337 rng, ITreasureShuffleFlags flags, 
                                     IEnumerable<IRewardSource> incentiveLocations, 
                                     IEnumerable<Item> incentiveItems)
        {
            var forcedItems = ItemLocations.AllOtherItemLocations.ToList();
            var incentiveLocationPool =
                incentiveLocations
                             .Where(x => !forcedItems.Any(y => y.Address == x.Address))
                             .ToList();
            if (!flags.ForceVanillaNPCs)
            {
                CheckCanoeItemInsteadOfEventVar();
                EnableBridgeShipCanalAnywhere();
                EnableNPCsGiveAnyItem();
                // This extends Vampire's routine to set a flag for Sarda, but it also clobers Sarda's routine
                if (!flags.EarlyRod && !forcedItems.Any(x => x.Address == ItemLocations.Sarda.Address))
                {
                    Put(0x393E1, Blob.FromHex("207F90A51160"));
                }
                else
                {
                    incentiveLocationPool =
                        incentiveLocationPool
                            .Select(x => x.Address == ItemLocations.Sarda.Address
                                    ? new MapObject(ObjectId.Sarda, MapLocation.SardasCave, Item.Rod)
                                    : x).ToList();
                }
            }
            else
            {
                forcedItems = ItemLocations.AllNonTreasureItemLocations.ToList();
                incentiveLocationPool = 
                    incentiveLocations
                                 .Where(x => !forcedItems.Any(y => y.Address == x.Address))
                                 .ToList(); 
            }
            var incentivePool = 
                incentiveItems
                    .Where(x => !forcedItems.Any(y => y.Item == x))
                    .ToList();

            var shipLocations = 
                ItemLocations.ValidShipLocations
                             .Where(x => !forcedItems.Any(y => y.Address == x.Address))
                             .ToList();
            if (!flags.AllowForcedEarlyIceCave) 
            {
                shipLocations = shipLocations.Except(ItemLocations.IceCave).ToList();
            }

            var placedItems = new List<IRewardSource>();
            var treasureBlob = Get(TreasureOffset, TreasureSize * TreasureCount);

            //****************************STATS*********************************
            long sanityCounter = 0;
            const int maxIterations = 1;
            var forcedIceCount = 0;
            var itemPlacementStats = ItemLists.AllQuestItems.ToDictionary(x => x, x => new List<int>());
            itemPlacementStats[Item.Ribbon] = new List<int>();
            var itemPlacementZones = ItemLists.AllQuestItems.ToDictionary(x => x, x => new List<string>());
            itemPlacementZones[Item.Ribbon] = new List<string>();
            long iterations = 0;
            while (iterations < maxIterations)
            {
            iterations++;
            //*************************END STATS********************************
            var treasureChestPool =
                ItemLocations.AllTreasures
                             .Where(x => !x.IsUnused && !forcedItems.Any(y => y.Address == x.Address))
                             .ToList();
            if (flags.EarlyOrdeals)
            {
                treasureChestPool =
                    treasureChestPool
                        .Select(x => ((x as TreasureChest)?.AccessRequirement.HasFlag(AccessRequirement.Crown) ?? false)
                                ? new TreasureChest(x, x.Item, x.AccessRequirement & ~AccessRequirement.Crown)
                                : x).ToList();
                incentiveLocationPool =
                    incentiveLocationPool
                        .Select(x => ((x as TreasureChest)?.AccessRequirement.HasFlag(AccessRequirement.Crown) ?? false)
                            ? new TreasureChest(x, x.Item, x.AccessRequirement & ~AccessRequirement.Crown)
                            : x).ToList();
            }

            var treasurePool = TreasureConditions.UsedIndices.Select(x => (Item)treasureBlob[x]).ToList();
            foreach (var incentive in incentivePool)
            {
                treasurePool.Remove(incentive);
            }
            foreach (var placement in forcedItems)
            {
                treasurePool.Remove(placement.Item);
            }
            do
            {
                sanityCounter++;
                // 1. (Re)Initialize lists inside of loop
                placedItems = forcedItems.ToList();
                var incentives = incentivePool.ToList();
                incentives.Shuffle(rng);

                if (!flags.ForceVanillaNPCs)
                {
                    // 2. Place caravan item first because among incentive locations it has the smallest set of possible items
                    if (!placedItems.Any(x => x.Address == ItemLocations.CaravanItemShop1.Address))
                    {
                        var itemPick = 
                                incentives.Where(x => x < Item.Canoe && x != Item.Floater)
                                          .ToList().PickRandom(rng);
                        incentives.Remove(itemPick);

                        placedItems.Add(new ItemShopSlot(ItemLocations.CaravanItemShop1, itemPick));
                    }

                    // 3. Place Bridge and Ship next since the valid location lists are so small
                    IRewardSource bridgePlacement = null;
                    if (!flags.EarlyBridge)
                    {
                        bridgePlacement = 
                                ItemLocations.ValidBridgeLocations.ToList().PickRandom(rng);
                        incentives.Remove(Item.Bridge);
                        placedItems.Add(NewItemPlacement(bridgePlacement, Item.Bridge));
                    }
                    var shipPlacement = 
                            shipLocations
                                .Where(x => x.Address != bridgePlacement?.Address)
                                .ToList().PickRandom(rng);
                    placedItems.Add(NewItemPlacement(shipPlacement, Item.Ship));
                }

                // 4. Then place all incentive locations that don't have special logic
                foreach (var incentiveLocation in incentiveLocationPool.Where(x => !placedItems.Any(y => y.Address == x.Address)))
                {
                    placedItems.Add(NewItemPlacement(incentiveLocation, incentives.SpliceRandom(rng)));
                }

                // 5. Then place remanining incentive items in any other chest
                var treasureChestPoolForExtraIncentiveItems =
                    treasureChestPool
                         .Where(x => !ItemLocations.ToFR.Any(y => y.Address == x.Address) &&
                                !x.IsUnused && !placedItems.Any(y => y.Address == x.Address))
                         .ToList();
                foreach (var incentive in incentives)
                {
                    placedItems.Add(new TreasureChest(treasureChestPoolForExtraIncentiveItems.SpliceRandom(rng), incentive));
                }

                // 6. Check sanity and loop if needed
            } while (!CheckSanity(placedItems, flags));

            // 7. Place all remaining unincentivized treasures
            var i = 0;
            treasureChestPool =
                treasureChestPool
                     .Where(x => !x.IsUnused && !placedItems.Any(y => y.Address == x.Address))
                    .ToList();
            treasurePool.Shuffle(rng);
            foreach (var remainingTreasure in treasureChestPool)
            {
                placedItems.Add(new TreasureChest(remainingTreasure, treasurePool[i]));
                i++;
            }

            //****************************STATS*********************************
            var outputIndexes = placedItems.ToLookup(x => x.Item, x => x.Address);
            foreach (Item item in itemPlacementStats.Keys)
            {
                itemPlacementStats[item].AddRange(outputIndexes[item].ToList());
            }
            var outputZones =
                placedItems
                    .ToLookup(x => x.Item,
                              x => incentiveLocationAreas
                                .Where(y => y.Key.Any(z => z.Address == x.Address))
                              .Select(y => y.Value).SingleOrDefault());
            foreach (Item item in itemPlacementZones.Keys)
            {
                if (!outputZones[item].Any()) continue;
                itemPlacementZones[item].AddRange(outputZones[item]);
            }
            var matoyaShip = placedItems.Any(x => x.Address == ItemLocations.Matoya.Address && x.Item == Item.Ship);
            var crystalIceCave = placedItems.Any(x => x.Item == Item.Crystal &&
                                x.Address >= ItemLocations.IceCave1.Address &&
                                                 x.Address <= ItemLocations.IceCaveMajor.Address);
            var keyIceCave = placedItems.Any(x => x.Item == Item.Key &&
                            x.Address >= ItemLocations.IceCave1.Address &&
                                             x.Address <= ItemLocations.IceCaveMajor.Address);
            var keyLockedCrystal = placedItems.Any(x => x.Item == Item.Crystal &&
                                                   x.AccessRequirement.HasFlag(AccessRequirement.Key));
            var keyLockedShip = placedItems.Any(x => x.Item == Item.Ship &&
                                                x.AccessRequirement.HasFlag(AccessRequirement.Key));
            if ((keyLockedShip && keyIceCave) ||
                (matoyaShip && crystalIceCave) ||
                (matoyaShip && keyLockedCrystal && keyIceCave))
                forcedIceCount++;
            }

            if (iterations > 10) {
                Debug.WriteLine(PrintStats(maxIterations, itemPlacementStats, itemPlacementZones));
                Debug.WriteLine($"Forced Early Ice Cave for Ship: {forcedIceCount} out of {maxIterations}");
            }
            Debug.WriteLine($"Sanity Check Fails per run: {(double)sanityCounter / maxIterations}");
            //***********************END STATS**********************************

            // Output the results tothe ROM
            foreach (var item in placedItems.Where(x => !x.IsUnused && x.Address < 0x80000))
            {
                Debug.WriteLine(item.SpoilerText);
                item.Put(this);
            }
        }

        private IRewardSource NewItemPlacement(IRewardSource copyFromSource, Item newItem)
        {
            if (copyFromSource is MapObject)
            {
                return new MapObject(copyFromSource as MapObject, newItem);
            }
            else
            {
                return new TreasureChest(copyFromSource, newItem);
            }
        }

        private bool CheckSanity(List<IRewardSource> treasureBlob, ISanityCheckFlags flags)
        {
            if (!flags.EarlyOrdeals && 
                treasureBlob.Any(x => x.Item == Item.Crown && ItemLocations.Ordeals.Any(y => y.Address == x.Address)))
                return false;
            const int maxIterations = 20;
            var currentIteration = 0;
            var currentAccess = AccessRequirement.None; 
            var currentMapChanges = MapChange.None;
            if (flags.EarlyBridge)
                currentMapChanges |= MapChange.Bridge;
            var canoeRequiresEarthOrb = flags.ForceVanillaNPCs && !flags.EarlyCanoe;
            var rodRequiresEarthCave = flags.ForceVanillaNPCs && !flags.EarlyRod;
            var allMapLocations = Enum.GetValues(typeof(MapLocation))
                                      .Cast<MapLocation>().ToList();
            Func<IEnumerable<MapLocation>> currentMapLocations =
                () => allMapLocations
                    .Where(x => ItemLocations.MapLocationRequirements[x]
                           .Any(y => currentMapChanges.HasFlag(y)));
            Func<IEnumerable<IRewardSource>> currentItemLocations =
                () => treasureBlob
                           .Where(x => currentMapLocations().ToList().Contains(x.MapLocation) &&
                                  currentAccess.HasFlag(x.AccessRequirement));
            
            var winTheGameAccess = ItemLocations.ChaosReward.AccessRequirement;
            var winTheGameLocation = ItemLocations.ChaosReward.MapLocation;
            var accessibleLocationCount = currentItemLocations().Count();

            while(!currentAccess.HasFlag(winTheGameAccess) || 
                  !currentMapLocations().Contains(winTheGameLocation))
            {
                if (currentIteration > maxIterations) 
                {
                    throw new InvalidOperationException($"Sanity Check hit max iterations: {currentIteration}");
                }
                currentIteration++;
                var currentItems = currentItemLocations().Select(x => x.Item).ToList();

                if (!currentAccess.HasFlag(AccessRequirement.Key) &&
                    currentItems.Contains(Item.Key))
                    currentAccess |= AccessRequirement.Key;
                if (!currentMapChanges.HasFlag(MapChange.Bridge) &&
                    currentItems.Contains(Item.Bridge) &&
                    currentMapLocations().Contains(MapLocation.BridgeLocation))
                    currentMapChanges |= MapChange.Bridge;
                if (!currentAccess.HasFlag(AccessRequirement.Crown) &&
                    currentItems.Contains(Item.Crown))
                    currentAccess |= AccessRequirement.Crown;
                if (!currentAccess.HasFlag(AccessRequirement.Crystal) &&
                    currentItems.Contains(Item.Crystal))
                    currentAccess |= AccessRequirement.Crystal;
                if (!currentAccess.HasFlag(AccessRequirement.Herb) &&
                    currentItems.Contains(Item.Herb))
                    currentAccess |= AccessRequirement.Herb;
                if (!currentMapChanges.HasFlag(MapChange.Canoe) &&
                    currentItems.Contains(Item.Canoe) && 
                    (!canoeRequiresEarthOrb || currentItems.Contains(Item.EarthOrb)))
                    currentMapChanges |= MapChange.Canoe;
                if (!currentMapChanges.HasFlag(MapChange.Ship) &&
                    currentItems.Contains(Item.Ship) &&
                    currentMapLocations().Contains(MapLocation.ShipLocation))
                    currentMapChanges |= MapChange.Ship;
                if (!currentAccess.HasFlag(AccessRequirement.Tnt) &&
                    currentItems.Contains(Item.Tnt))
                    currentAccess |= AccessRequirement.Tnt;
                if (!currentAccess.HasFlag(AccessRequirement.Adamant) &&
                    currentItems.Contains(Item.Adamant))
                    currentAccess |= AccessRequirement.Adamant;
                if (!currentMapChanges.HasFlag(MapChange.Canal) &&
                    currentItems.Contains(Item.Canal) &&
                    currentMapChanges.HasFlag(MapChange.Ship))
                    currentMapChanges |= MapChange.Canal;
                if (!currentMapChanges.HasFlag(MapChange.TitanFed) &&
                    currentItems.Contains(Item.Ruby) &&
                    currentMapLocations().Contains(MapLocation.TitansTunnelA))
                    currentMapChanges |= MapChange.TitanFed;
                if (!currentAccess.HasFlag(AccessRequirement.Rod) &&
                    currentItems.Contains(Item.Rod) && 
                   (!rodRequiresEarthCave || currentMapLocations().Contains(MapLocation.EarthCave)))
                    currentAccess |= AccessRequirement.Rod;
                if (!currentAccess.HasFlag(AccessRequirement.Slab) &&
                    currentItems.Contains(Item.Slab))
                    currentAccess |= AccessRequirement.Slab;
                if (!currentMapChanges.HasFlag(MapChange.Airship) &&
                    (currentItems.Contains(Item.Floater)) && // || currentItems.Contains(Item.Airship)) &&
                    currentMapLocations().Contains(MapLocation.AirshipLocation))
                    currentMapChanges |= MapChange.Airship;

                if (!currentAccess.HasFlag(AccessRequirement.Bottle) &&
                    currentItems.Contains(Item.Bottle))
                    currentAccess |= AccessRequirement.Bottle;
                if (!currentAccess.HasFlag(AccessRequirement.Oxyale) &&
                    currentItems.Contains(Item.Oxyale))
                    currentAccess |= AccessRequirement.Oxyale;
                if (!currentMapChanges.HasFlag(MapChange.Chime) &&
                    currentItems.Contains(Item.Chime) &&
                    currentMapChanges.HasFlag(MapChange.Airship))
                    currentMapChanges |= MapChange.Chime;
                if (!currentAccess.HasFlag(AccessRequirement.Cube) &&
                    currentItems.Contains(Item.Cube))
                    currentAccess |= AccessRequirement.Cube;
                if (!currentAccess.HasFlag(AccessRequirement.BlackOrb) &&
                    ItemLists.AllOrbs.All(y => currentItems.Contains(y)))
                    currentAccess |= AccessRequirement.BlackOrb;
                if (!currentAccess.HasFlag(AccessRequirement.Lute) &&
                    currentItems.Contains(Item.Lute))
                    currentAccess |= AccessRequirement.Lute;

                var newCount = currentItemLocations().Count();
                if (newCount <= accessibleLocationCount)
                { 
                    return false; 
                }
                accessibleLocationCount = newCount;
            }
            return true;
        }

        /// <summary>
        /// Start of NPC item shuffle should call the following 4 methods:
        /// 
        /// PermanentCaravan();
        /// CheckCanoeItemInsteadOfEventVar();
        /// EnableBridgeShipCanalAnywhere();
        /// EnableNPCsGiveAnyItem();
        /// 
        /// Then any item can be assigned to an NPC like this:
        /// new MapObject(ItemLocations.KingConeria, Item.Key).Put(this);
        /// </summary>
        public void EnableBridgeShipCanalAnywhere()
        {
            // Replace a long unused dialog string with text for game variables
            var gameVariableText =
                "9C91929900000000" + // SHIP
                "8A929B9C91929900" + // AIRSHIP
                "8B9B928D908E0000" + // BRIDGE
                "8C8A978A95000000";  // CANAL
            Put(0x2825C, Blob.FromHex(gameVariableText));

            // Add processing in control code 3 to check for variables
            var controlCode3 =
                "A5616920901D0A" +
                "695A853E" +
                "A982853F" +
                "184C9ADB";
            Put(0x7DBF8, Blob.FromHex(controlCode3));
            // See source: ~/asm/1F_DBF8_ControlCode3.asm

            // Use control code 3 instead of 2 for normal treasure
            Data[0x2B187] = 0x03;
        }

        /// <summary>
        /// Start of NPC item shuffle should call the following 4 methods:
        /// 
        /// PermanentCaravan();
        /// CheckCanoeItemInsteadOfEventVar();
        /// EnableBridgeShipCanalAnywhere();
        /// EnableNPCsGiveAnyItem();
        /// 
        /// Then any item can be assigned to an NPC like this:
        /// new MapObject(ItemLocations.KingConeria, Item.Key).Put(this);
        /// </summary>
        public void EnableNPCsGiveAnyItem()
        {
            // These first 2 are safe to call even if NPC items aren't being shuffled
            SplitOpenTreasureRoutine();
            CleanupNPCRoutines();

            // Replace Don't be greedy text with NPC item text "Here, take this"
            var replacementText =
                "91A823" + // Here
                "BF1BA4AE1A" + // , take 
                "1C3005" + // this\n
                $"0300";
            Put(0x28F35, Blob.FromHex(replacementText));

            // New routine for NPC item trades
            var itemTradeNPCRoutine =
                "A416207990B027A5106920AABD0060F01D" +
                "A513F01920" + giveRewardRoutineAddress +
                "B016A5106920C931B004AADE0060" +
                "A416207F90A93A60" +
                "A51160";
            // Put at Nerrick routine
            Put(0x39356, Blob.FromHex(itemTradeNPCRoutine));
            // See source: ~/asm/0E_9356_StandardNPCItemTrade.asm

            // New routine for NPC items based on game event flag
            var eventFlagGiveNPCRoutine =
                "A41098F0052079909007" +
                "A4162079909003A51260" +
                "A51320" + giveRewardRoutineAddress +
                "B007A416" +
                "207F90A93A60";
            // Put at Talk_ifairship, overrunning Talk_ifearthfire and most of Talk_CubeBotBad
            Put(0x3956B, Blob.FromHex(eventFlagGiveNPCRoutine));
            // See source: ~/asm/0E_956B_StandardNPCItem.asm

            // *** Only Nerrick and Smith and required to give an item since 
            // those routines are overwritten so we set the vanilla item here 
            // so he still gives it if nothing else changes
            Data[ItemLocations.Nerrick.Address] = (byte)Item.Canal;
            Data[ItemLocations.Smith.Address] = (byte)Item.Xcalber;

            // *** Handle special cases (Bikke and Astos)
            EnableBikkeAnyItem();

            EnableAstosAnyItem();
        }

        private void EnableAstosAnyItem()
        {
            Data[ItemLocations.Astos.Address] = (byte)Item.Crystal;
            var newAstosRoutine =
                "AD2260F016A513F01220" + giveRewardRoutineAddress +
                "B00FA007207392A97D" +
                "20C590A93A60A51160";
            Put(0x39338, Blob.FromHex(newAstosRoutine));
            // See source: ~/asm/0E_9338_AstosAnyItem.asm
        }

        private void EnableBikkeAnyItem()
        {
            Data[ItemLocations.Bikke.Address] = (byte)Item.Ship;
            var newBikkeRoutine =
                "A03F209190B00B" +
                "20A490A97E" +
                "20C590A51160" +
                "A004207990B013A513F00F841020" + giveRewardRoutineAddress +
                "B00AA410207F90A93A60A51260";
            Put(0x392D0, Blob.FromHex(newBikkeRoutine));
            // See source: ~/asm/0E_92D0_BikkeAnyItem.asm
        }

        private void SplitOpenTreasureRoutine()
        {
            // Replace "OpenTreasureChest" routine
            var openTreasureChest =
                $"A9002003FEA645BD00B120{giveRewardRoutineAddress}" +
                "B00AA445B9006209049900628A60"; // 27 bytes
            Put(0x7DD78, Blob.FromHex(openTreasureChest));
            // See source: ~/asm/1F_DD78_OpenTreasureChestRewrite.asm

            // New "GiveReward" routine
            const string checkItem =
                "85616920C93CB013AAC90CD005" +
                "DE0060B003FE0060C936B02A902B"; // 27 bytes
            const string notItem =
                "A561C96C900920B9EC20EADD4CD6DD" +
                "C944B0092034DDB007A9E59007" +
                "2046DDB00CA9BD" +
                "65619D0061"; // 40 bytes
            const string openChest =
                "18E67DE67DA2F09004EEB760E88A60"; // 12 bytes
            var giveRewardRoutine =
                $"{checkItem}{notItem}{openChest}";
            Put(0x7DD93, Blob.FromHex(giveRewardRoutine));
            // See source: ~/asm/1F_DD78_OpenTreasureChestRewrite.asm
        }
    }
}
