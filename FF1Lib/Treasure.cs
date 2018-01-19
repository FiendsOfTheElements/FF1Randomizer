using System.Collections.Generic;
using System.Linq;
using RomUtilities;

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
			var treasureBlob = Get(TreasureOffset, TreasureSize * TreasureCount);
			var usedTreasures = TreasureConditions.UsedIndices.Select(i => (Item)treasureBlob[i]).ToList();

			do
			{
				usedTreasures.Shuffle(rng);
				if (incentivizeIceCave || incentivizeOrdeals)
				{
					const int OrdealsTreasureLocation = 130; // Really 131, because 0 is unused, and usedTreasures doesn't include it.
					const int IceCaveTreasureLocation = 113; // Really 114
					var incentiveTreasures = new List<Item>
					{
					    Item.Floater,
						Item.Slab,
						Item.Adamant,
					    Item.Tail,
						Item.Masamune,
						Item.Ribbon
					};
					if (earlyCanoe)
					{
						incentiveTreasures.Add(Item.Ruby);
					}

					if (incentivizeOrdeals)
					{
						if (earlyOrdeals)
						{
							incentiveTreasures.Add(Item.Crown);
						}

						var choice = rng.Between(0, incentiveTreasures.Count - 1);
						var selectedTreasure = incentiveTreasures[choice];
						incentiveTreasures.RemoveAt(choice);
						var location = usedTreasures.IndexOf(selectedTreasure);
						usedTreasures.Swap(location, OrdealsTreasureLocation);
					}

					if (incentivizeIceCave)
					{
						if (incentivizeOrdeals && !earlyOrdeals) // Don't add this twice!
						{
							incentiveTreasures.Add(Item.Crown);
						}

						var choice = rng.Between(0, incentiveTreasures.Count - 1);
						var selectedTreasure = incentiveTreasures[choice];
						incentiveTreasures.RemoveAt(choice);
						var location = usedTreasures.IndexOf(selectedTreasure);
						usedTreasures.Swap(location, IceCaveTreasureLocation);
					}
				}
				for (int i = 0; i < TreasureConditions.UsedIndices.Count; i++)
				{
					treasureBlob[TreasureConditions.UsedIndices[i]] = (byte)usedTreasures[i];
				}
			} while (!CheckSanity(treasureBlob, earlyCanoe, earlyOrdeals));

			Put(TreasureOffset, treasureBlob);
		}

		private bool CheckSanity(Blob treasureBlob, bool earlyCanoe, bool earlyOrdeals)
		{
			if (TreasureConditions.ToFR.Select(i => (Item)treasureBlob[i]).Intersect(TreasureConditions.AllQuestItems).Any())
			{
				return false;
			}

			var accessibleTreasures = new HashSet<int>(TreasureConditions.Beginning);
			var questItems = new HashSet<Item>();
			int lastCount;
			do
			{
				lastCount = accessibleTreasures.Count;
				questItems.UnionWith(accessibleTreasures.Select(i => (Item)treasureBlob[i]).Intersect(TreasureConditions.AllQuestItems));

				if (questItems.Contains(Item.Crown))
				{
					accessibleTreasures.UnionWith(TreasureConditions.EarlyCrown);
				}
				if (questItems.Contains(Item.Tnt))
				{
					accessibleTreasures.UnionWith(TreasureConditions.Tnt);

					if (questItems.Contains(Item.Ruby) || earlyCanoe && questItems.Contains(Item.Floater))
					{
						accessibleTreasures.UnionWith(TreasureConditions.Rod);
					}
					if (earlyCanoe || questItems.Contains(Item.Ruby))
					{
						accessibleTreasures.UnionWith(TreasureConditions.FireAndIce);

						if (earlyOrdeals || questItems.Contains(Item.Crown))
						{
							accessibleTreasures.UnionWith(TreasureConditions.Ordeals);
						}
						if (questItems.Contains(Item.Floater))
						{
							accessibleTreasures.UnionWith(TreasureConditions.Airship);

							if (questItems.Contains(Item.Crown))
							{
								accessibleTreasures.UnionWith(TreasureConditions.LateCrown);
							}
							if (questItems.Contains(Item.Slab))
							{
								accessibleTreasures.UnionWith(TreasureConditions.Chime);
							}
						}
					}
				}
			} while (accessibleTreasures.Count > lastCount && accessibleTreasures.Count < TreasureConditions.UsedIndices.Count - TreasureConditions.ToFR.Count);

			return accessibleTreasures.Count == TreasureConditions.UsedIndices.Count - TreasureConditions.ToFR.Count;
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
        /// Data[ItemLocations.KingConeria.Address] = (byte) Item.Key;
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
        /// Data[ItemLocations.KingConeria.Address] = (byte) Item.Key;
        /// </summary>
        public void EnableNPCsGiveAnyItem()
        {
            // These 3 are safe to call even if NPC items aren't being shuffled
            SplitOpenTreasureRoutine();
            SetNPCFetchQuestRequirements();
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
            var itemTradeRoutineAddress = Blob.FromHex("6C93");
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
            var eventFlagRoutineAddress = Blob.FromHex("6B95");
            // Put at CubeBotBad and overruns into Lefein
            Put(0x3956B, Blob.FromHex(eventFlagGiveNPCRoutine));
            // See source: ~/asm/0E_9586_StandardNPCItem.asm

            // *** Only Smith is required to give an item since only his routine is overwritten
            // so set the vanilla item here so he still gives it if nothing else changes
            Data[ItemLocations.Smith.Address] = (byte)Item.Xcalber;

            // *** Handle special cases (Bikke and Astos)
            EnableBikkeAnyItem();

            EnableAstosAnyItem();

            // *** Normal cases (King, Prince, Matoya, Nerrick, Sarda, CubeBot, Princess, Fairy, CanoeSage)
            Put(lut_MapObjTalkJumpTblAddress + 2 * ObjectId.King, eventFlagRoutineAddress);
            Put(lut_MapObjTalkJumpTblAddress + 2 * ObjectId.ElfPrince, eventFlagRoutineAddress);
            Put(lut_MapObjTalkJumpTblAddress + 2 * ObjectId.Matoya, itemTradeRoutineAddress);
            Put(lut_MapObjTalkJumpTblAddress + 2 * ObjectId.Nerrick, itemTradeRoutineAddress);
            Put(lut_MapObjTalkJumpTblAddress + 2 * ObjectId.Sarda, eventFlagRoutineAddress);
            Put(lut_MapObjTalkJumpTblAddress + 2 * ObjectId.CubeBot, eventFlagRoutineAddress);
            Put(lut_MapObjTalkJumpTblAddress + 2 * ObjectId.Princess2, eventFlagRoutineAddress);
            Put(lut_MapObjTalkJumpTblAddress + 2 * ObjectId.Fairy, eventFlagRoutineAddress);
            Put(lut_MapObjTalkJumpTblAddress + 2 * ObjectId.CanoeSage, eventFlagRoutineAddress); 
        }

        private void EnableAstosAnyItem()
        {
            var newAstosRoutine =
                "AD2260F016A513F01220" + giveRewardRoutineAddress +
                "B00FA007207392A97D" +
                "20C590A93A60A51160";
            Put(0x39338, Blob.FromHex(newAstosRoutine));
            // See source: ~/asm/0E_9338_AstosAnyItem.asm
        }

        private void EnableBikkeAnyItem()
        {
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
