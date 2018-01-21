
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
        public static readonly List<Item> AllQuestItems = new List<Item>
        {
            Item.Tnt,
            Item.Crown,
            Item.Ruby,
            Item.Floater,
            Item.Tail,
            Item.Slab,
            Item.Adamant
        };

        public static readonly List<int> UnusedIndices =
            Enumerable.Range(0, 1).Concat(
            Enumerable.Range(145, 4)).Concat(
            Enumerable.Range(187, 9)).Concat(
            Enumerable.Range(255, 1))
            .ToList();

        public static readonly List<int> UsedIndices = Enumerable.Range(0, 256).Except(UnusedIndices).ToList(); // This maps a compacted list back to the game's array, skipping the unused slots.

        public static readonly List<int> Beginning =
            Enumerable.Range(7, 3).Concat(
            Enumerable.Range(20, 10)).Concat(
            Enumerable.Range(33, 2)).Concat(
            Enumerable.Range(43, 3))
            .ToList();

        public static readonly List<int> EarlyCrown =
            Enumerable.Range(1, 6).Concat( // Coneria
            Enumerable.Range(10, 3)).Concat( // ToF east side
            Enumerable.Range(13, 4)).Concat( // Elfland
            Enumerable.Range(17, 3)).Concat( // Northwest Castle
            Enumerable.Range(30, 3)).Concat( // Marsh Cave
            Enumerable.Range(35, 8)) // Dwarf Cave
            .ToList();

        public static readonly List<int> Tnt = Enumerable.Range(46, 16).ToList(); // Earth Cave B1-B3

        public static readonly List<int> Rod = Enumerable.Range(62, 12).ToList(); // Earth Cave B4, Titan's Tunnel

        public static readonly List<int> FireAndIce = Enumerable.Range(74, 49).ToList();

        public static readonly List<int> Ordeals = Enumerable.Range(123, 9).ToList();

        public static readonly List<int> Airship =
            Enumerable.Range(132, 13).Concat( // Cardia Islands
            Enumerable.Range(149, 32).Except(new[] { 165 })).Concat( // Sea Shrine
            Enumerable.Range(181, 6)) // Waterfall
            .ToList();

        public static readonly List<int> LateCrown = new List<int> { 165 }; // Reverse-C room in Sea Shrine

        public static readonly List<int> Chime = Enumerable.Range(196, 52).ToList(); // Mirage Tower + Sky Castle

        public static readonly List<int> ToFR = Enumerable.Range(248, 7).ToList(); // Anything that blocks an ORB will also block these.
    }

    public partial class FF1Rom : NesRom
    {
        public const int TreasureOffset = 0x03100;
        public const int TreasureSize = 1;
        public const int TreasureCount = 256;

        public const int lut_MapObjTalkJumpTblAddress = 0x390D3;
        public const string giveRewardRoutineAddress = "93DD";

        private static void PrintStats(int maxIterations, Dictionary<Item, List<int>> incentiveLocations)
        {
            var sb = new StringBuilder();
            sb.Append("Location         ");
            foreach (Item item in incentiveLocations.Keys)
            {
                var name = Enum.GetName(typeof(Item), item);
                name = $"{string.Join("", Enumerable.Repeat(" ", Math.Max(1, 9 - name.Length)))}{name}";
                sb.Append(name.Substring(0, Math.Min(12, name.Length)));
            }
            sb.Append("\n");
            foreach (var rewardSource in ItemLocations.AllQuestItemLocations.Where(x => x.Address < 0x80000))
            {
                sb.Append($"{rewardSource.Name}" +
                          $"{string.Join("", Enumerable.Repeat(" ", Math.Max(1, 17 - rewardSource.Name.Length)))}");
                foreach (var itemPlacements in incentiveLocations.Values)
                {
                    var percentage = $"   {100.0 * itemPlacements.Count(x => x == rewardSource.Address) / maxIterations:g2}";
                    percentage = $"{string.Join("", Enumerable.Repeat(" ", Math.Max(1, 9 - percentage.Length)))}{percentage}";
                    sb.Append(percentage);
                }
                sb.Append("\n");
            }
            Debug.WriteLine(sb.ToString());
        }

        public void ShuffleTreasures(MT19337 rng, ITreasureShuffleFlags flags)
        {
            const int maxIterations = 1;
            var sanityCounter = 0;
            var forcedIceCount = 0;
            var itemPlacementStats = ItemLists.AllQuestItems.ToDictionary(x => x, x => new List<int>());

            var forcedItems = ItemLocations.AllOtherItemLocations.ToList();
            if (!flags.ForceVanillaNPCs)
            {
                CheckCanoeItemInsteadOfEventVar();
                EnableBridgeShipCanalAnywhere();
                EnableNPCsGiveAnyItem();
            }
            else
            {
                forcedItems = ItemLocations.AllNonTreasureItemLocations.ToList();
            }
            var normalIncentiveNPCs = 
                ItemLocations.AllNPCItemLocations
                             .Where(x => !forcedItems.Any(y => y.Address == x.Address))
                             .ToList(); 
            var incentivePool = ItemLists.AllQuestItems.ToList();
            incentivePool.Add(Item.Xcalber);
            incentivePool.Add(Item.Masamune);
            incentivePool.Add(Item.Ribbon);
            incentivePool.Remove(Item.Ship);
            incentivePool = 
                incentivePool
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

            var treasureBlob = Get(TreasureOffset, TreasureSize * TreasureCount);
            var placedItems = new List<IRewardSource>();
            for (var iterations = 0; iterations < maxIterations; iterations++)
            {
                var treasurePool =
                    TreasureConditions.UsedIndices.Select(x => (Item)treasureBlob[x]).ToList();

                foreach (var startingIncentive in incentivePool)
                {
                    treasurePool.Remove(startingIncentive);
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
                    shipLocations.Shuffle(rng);

                    // 2. Place caravan item first because among incentive locations it has the smallest set of possible items
                    if (!placedItems.Any(x => x.Address == ItemLocations.CaravanItemShop1.Address))
                    {
                        var itemPick = incentives.PickRandom(rng);
                        if (itemPick == Item.Floater || itemPick >= Item.Canoe)
                        {
                            itemPick = incentives.First(x => x < Item.Canoe && x != Item.Floater);
                        }
                        incentives.Remove(itemPick);

                        placedItems.Add(new ItemShopSlot(ItemLocations.CaravanItemShop1, itemPick));
                    }

                    // 3. Place Bridge and Ship next since the valid location lists are so small
                    IRewardSource bridgePlacement = null;
                    if (!flags.EarlyBridge)
                    {
                        incentives.Remove(Item.Bridge);
                        bridgePlacement = ItemLocations.ValidBridgeLocations.ToList().PickRandom(rng);
                        if (bridgePlacement is MapObject)
                        {
                            placedItems.Add(new MapObject(bridgePlacement, Item.Bridge));
                        }
                        else
                        {
                            placedItems.Add(new TreasureChest(bridgePlacement, Item.Bridge));
                        }
                    }
                    var shipPlacement = shipLocations.First(x => x.Address != bridgePlacement?.Address);
                    if (shipPlacement is MapObject)
                    {
                        placedItems.Add(new MapObject(shipPlacement, Item.Ship));
                    }
                    else
                    {
                        placedItems.Add(new TreasureChest(shipPlacement, Item.Ship));
                    }

                    // 4. Then place all incentive locations that don't have special logic (NPCs)
                    foreach (var npc in normalIncentiveNPCs.Where(x => !placedItems.Any(y => y.Address == x.Address)))
                    {
                        if (incentives.Any()) 
                        {
                            placedItems.Add(new MapObject(npc, incentives.SpliceRandom(rng)));
                        }
                        else 
                        {
                            placedItems.Add(new MapObject(npc, treasurePool.SpliceRandom(rng)));
                        }
                    }

                    // 5. Then place remanining incentive locations with additional logic needed
                    if (flags.IncentivizeIceCave)
                    {
                        var itemPick = incentives.PickRandom(rng);
                        if (!flags.AllowForcedEarlyIceCave && itemPick == Item.Ship)
                        {
                            itemPick = incentives.First(x => x != Item.Ship);
                        }
                        incentives.Remove(itemPick);
                            
                        placedItems.Add(new TreasureChest(ItemLocations.IceCaveMajor, itemPick));
                    }

                    if (flags.IncentivizeOrdeals)
                    {
                        var itemPick = incentives.PickRandom(rng);
                        if (!flags.EarlyOrdeals && itemPick == Item.Crown)
                        {
                            itemPick = incentives.First(x => x != Item.Crown);
                        }
                        incentives.Remove(itemPick);
                            
                        placedItems.Add(new TreasureChest(ItemLocations.OrdealsMajor, itemPick));
                    }

                    var treasureChestPoolForExtraIncentiveItems =
                        ItemLocations.AllTreasures
                                     .Where(x => !x.IsUnused && !placedItems.Any(y => y.Address == x.Address))
                                     .ToList();
                    // 7. Place remaining incentives
                    foreach(var incentive in incentives)
                    {
                        placedItems.Add(new TreasureChest(treasureChestPoolForExtraIncentiveItems.SpliceRandom(rng), incentive));
                    }

                    // 8. Check sanity and loop if needed
                } while (!CheckSanity(placedItems, flags));


                // 8. Place all remaining unincentivized treasures
                var i = 0;
                var treasureChestPool =
                    ItemLocations.AllTreasures
                                 .Where(x => !x.IsUnused && !placedItems.Any(y => y.Address == x.Address))
                                 .ToList();
                treasurePool.Shuffle(rng);
                foreach (var remainingTreasure in treasureChestPool)
                {
                    placedItems.Add(new TreasureChest(remainingTreasure, treasurePool[i]));
                    i++;
                }

                // Record placements in stats
                var outputIndexes = placedItems.ToLookup(x => x.Item, x => x.Address);
                foreach (Item item in itemPlacementStats.Keys)
                {
                    itemPlacementStats[item].AddRange(outputIndexes[item].ToList());
                }
                if (placedItems.Any(x => x.Address == ItemLocations.Matoya.Address && x.Item == Item.Ship) &&
                    placedItems.Any(x => x.Item == Item.Crystal &&
                                    x.Address >= ItemLocations.IceCave1.Address &&
                                    x.Address <= ItemLocations.IceCaveMajor.Address))
                    forcedIceCount++;
            }
            PrintStats(maxIterations, itemPlacementStats);
            Debug.WriteLine($"Forced Early Ice Cave for Matoya Ship: {forcedIceCount} out of {maxIterations}");
            Debug.WriteLine($"Sanity Check Fails per run: {(double)sanityCounter / maxIterations}");

            // Output the results tothe ROM
            foreach (var item in placedItems.Where(x => !x.IsUnused && x.Address < 0x80000))
            {
                Debug.WriteLine(item.SpoilerText);
                item.Put(this);
            }
        }

        private bool CheckSanity(List<IRewardSource> treasureBlob, ISanityCheckFlags flags)
        {
            const int maxIterations = 20;
            var currentIteration = 0;
            var currentAccess = AccessRequirement.None; 
            var currentMapChanges = MapChange.None;
            if (flags.EarlyBridge)
                currentMapChanges |= MapChange.Bridge;

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
                if (currentIteration > maxIterations) {
                    Debug.WriteLine($"Sanity Check hit max iterations: {currentIteration}");
                    return false;
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
                    currentItems.Contains(Item.Canoe))
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
                    currentItems.Contains(Item.Rod))
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
                if (newCount > accessibleLocationCount) {
                    accessibleLocationCount = newCount;
                    continue;
                }
                //Debug.WriteLine($"Sanity Check failed due to no improvement from last iteration: {currentIteration}");
                return false;
            }
            //Debug.WriteLine($"Sanity Check Success iterations: {currentIteration}");
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
                $"{FF1Text.TextToBytes("SHIP").ToHex()}00000000" +
                $"{FF1Text.TextToBytes("AIRSHIP").ToHex()}00" +
                $"{FF1Text.TextToBytes("BRIDGE").ToHex()}0000" +
                $"{FF1Text.TextToBytes("CANAL").ToHex()}000000";
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
                "A5106920AABD0060F014" +
                "A513F01020" + giveRewardRoutineAddress +
                "B00DA5106920AADE0060" +
                "A93A60" +
                "A51160";
            // Put at Smith routine
            Put(0x3936C, Blob.FromHex(itemTradeNPCRoutine));
            // See source: ~/asm/0E_936C_StandardNPCItemTrade.asm

            // New routine for NPC items based on game event flag
            var eventFlagGiveNPCRoutine =
                "A41098F0052079909007" +
                "A4162079909003A51260" +
                "A51320" + giveRewardRoutineAddress +
                "B007A416" +
                "207F90A93A60";
            // Put at Talk_ifairship, overrunning Talk_ifearthfire and most of Talk_CubeBotBad
            Put(0x3956B, Blob.FromHex(eventFlagGiveNPCRoutine));
            // See source: ~/asm/0E_9586_StandardNPCItem.asm

            // *** Only Smith is required to give an item since only his routine is overwritten
            // so we set the vanilla item here so he still gives it if nothing else changes
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
                "DE0060B002FE0060C936B02A902B"; // 27 bytes
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
