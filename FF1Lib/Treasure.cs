
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

        public void ShuffleTreasures(MT19337 rng, bool earlyCanoe, bool earlyOrdeals, bool incentivizeIceCave, bool incentivizeOrdeals)
        {
            const int maxIterations = 1;
            if (incentivizeIceCave) TreasureConditions.UsedIndices.Remove(ItemLocations.IceCaveMajor.Address - 0x3100);
            if (incentivizeOrdeals) TreasureConditions.UsedIndices.Remove(ItemLocations.OrdealsMajor.Address - 0x3100);
            var listStatItems = ItemLists.AllQuestItems;
            var incentiveLocations = listStatItems.ToDictionary(x => x, x => new List<int>());
            var treasureBlob = Get(TreasureOffset, TreasureSize * TreasureCount);
            var forcedItems = ItemLocations.AllOtherItemLocations; // ItemLocations.AllNonTreasureItemLocations;
            var placedItems = new List<IRewardSource>();
            var treasureArray = ItemLocations.AllTreasures.OrderBy(x => x.Address).ToList();
            var treasurePool = TreasureConditions.UsedIndices.Select(x => (Item)treasureBlob[x]).ToList();
            var usedTreasures = new List<Item>();
            var sanityCounter = 0;
            for (var iterations = 0; iterations < maxIterations; iterations++)
            {
                do
                {
                    sanityCounter++;
                    placedItems = forcedItems.ToList();
                    usedTreasures = treasurePool.ToList();
                    var incentives =
                        ItemLists.AllQuestItems
                                 .Where(x => x < Item.Canoe && x != Item.Floater)
                                 .Where(x => !placedItems.Any(y => y.Item == x))
                                 .ToList();
                    // Place caravan first because among incentive locations it has the smallest set of possible items
                    if (!placedItems.Any(x => x.Address == ItemLocations.CaravanItemShop1.Address))
                        placedItems.Add(new ItemShopSlot(ItemLocations.CaravanItemShop1, incentives.SpliceRandom(rng)));
                    incentives =
                        ItemLists.AllQuestItems.ToList();
                    incentives.Add(Item.Xcalber);
                    incentives.Add(Item.Masamune);
                    incentives.Add(Item.Ribbon);
                    incentives =
                        incentives
                            .Where(x => !placedItems.Any(y => y.Item == x))
                            .ToList();

                    foreach (var npc in ItemLocations.AllNPCItemLocations.Where(x => !placedItems.Any(y => y.Address == x.Address)))
                    {
                        placedItems.Add(new MapObject(npc, incentives.SpliceRandom(rng)));
                    }

                    if (incentivizeIceCave)
                    {
                        usedTreasures.Remove(Item.Floater);
                        placedItems.Add(new TreasureChest(ItemLocations.IceCaveMajor, incentives.SpliceRandom(rng)));
                    }

                    if (incentivizeOrdeals)
                    {
                        usedTreasures.Remove(Item.Tail);
                        if (!earlyOrdeals)
                        {
                            incentives.Remove(Item.Crown);
                        }

                        placedItems.Add(new TreasureChest(ItemLocations.OrdealsMajor, incentives.SpliceRandom(rng)));
                    }
                    foreach(var placement in placedItems)
                    {
                        usedTreasures.Remove(placement.Item);
                    }
                    foreach(var leftoverIncentive in incentives)
                    {
                        usedTreasures.Remove(leftoverIncentive);
                        usedTreasures.Add(leftoverIncentive);
                    }
                    usedTreasures.Shuffle(rng);

                    for (int i = 0; i < TreasureConditions.UsedIndices.Count; i++)
                    {
                        placedItems.Add(new TreasureChest(treasureArray[TreasureConditions.UsedIndices[i] - 1], usedTreasures[i]));
                    }
                } while (!CheckSanity(placedItems, earlyCanoe, earlyOrdeals));
                var outputIndexes = placedItems.ToLookup(x => x.Item, x => x.Address);
                foreach (Item item in listStatItems)
                {
                    incentiveLocations[item].AddRange(outputIndexes[item].ToList());
                }
            }
            PrintStats(maxIterations, incentiveLocations);
            Debug.WriteLine($"Sanity Check Fails per run: {(double)sanityCounter / maxIterations}");

            foreach (var item in placedItems.Where(x => !x.IsUnused && x.Address < 0x80000))
            {
                Debug.WriteLine(item.SpoilerText);
                item.Put(this);
            }
        }

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

        private bool CheckSanity(List<IRewardSource> treasureBlob, bool earlyCanoe, bool earlyOrdeals, bool earlyBridge = true)
        {
            const int maxIterations = 20;
            var currentIteration = 0;
            var currentAccess = AccessRequirement.None; 
            var currentMapChanges = MapChange.None;
            if (earlyBridge)
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
