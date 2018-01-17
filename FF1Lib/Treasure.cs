using System.Collections.Generic;
using System.Linq;
using RomUtilities;

namespace FF1Lib
{
	public static class TreasureConditions
	{
		public static readonly List<byte> AllQuestItems = new List<byte>
		{
			Items.Tnt,
			Items.Crown,
			Items.Ruby,
			Items.Floater,
			Items.Tail,
			Items.Slab,
			Items.Adamant
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
			var usedTreasures = TreasureConditions.UsedIndices.Select(i => treasureBlob[i]).ToList();

			do
			{
				usedTreasures.Shuffle(rng);
				if (incentivizeIceCave || incentivizeOrdeals)
				{
					const int OrdealsTreasureLocation = 130; // Really 131, because 0 is unused, and usedTreasures doesn't include it.
					const int IceCaveTreasureLocation = 113; // Really 114
					var incentiveTreasures = new List<byte>
					{
					    Items.Floater,
						Items.Slab,
						Items.Adamant,
					    Items.Tail,
						Items.Masamune, // Masmune
						Items.Ribbon // Ribbon
					};
					if (earlyCanoe)
					{
						incentiveTreasures.Add(Items.Ruby);
					}

					if (incentivizeOrdeals)
					{
						if (earlyOrdeals)
						{
							incentiveTreasures.Add(Items.Crown);
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
							incentiveTreasures.Add(Items.Crown);
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
					treasureBlob[TreasureConditions.UsedIndices[i]] = usedTreasures[i];
				}
			} while (!CheckSanity(treasureBlob, earlyCanoe, earlyOrdeals));

			Put(TreasureOffset, treasureBlob);
		}

		private bool CheckSanity(Blob treasureBlob, bool earlyCanoe, bool earlyOrdeals)
		{
			if (TreasureConditions.ToFR.Select(i => treasureBlob[i]).Intersect(TreasureConditions.AllQuestItems).Any())
			{
				return false;
			}

			var accessibleTreasures = new HashSet<int>(TreasureConditions.Beginning);
			var questItems = new HashSet<byte>();
			int lastCount;
			do
			{
				lastCount = accessibleTreasures.Count;
				questItems.UnionWith(accessibleTreasures.Select(i => treasureBlob[i]).Intersect(TreasureConditions.AllQuestItems));

				if (questItems.Contains(Items.Crown))
				{
					accessibleTreasures.UnionWith(TreasureConditions.EarlyCrown);
				}
				if (questItems.Contains(Items.Tnt))
				{
					accessibleTreasures.UnionWith(TreasureConditions.Tnt);

					if (questItems.Contains(Items.Ruby) || earlyCanoe && questItems.Contains(Items.Floater))
					{
						accessibleTreasures.UnionWith(TreasureConditions.Rod);
					}
					if (earlyCanoe || questItems.Contains(Items.Ruby))
					{
						accessibleTreasures.UnionWith(TreasureConditions.FireAndIce);

						if (earlyOrdeals || questItems.Contains(Items.Crown))
						{
							accessibleTreasures.UnionWith(TreasureConditions.Ordeals);
						}
						if (questItems.Contains(Items.Floater))
						{
							accessibleTreasures.UnionWith(TreasureConditions.Airship);

							if (questItems.Contains(Items.Crown))
							{
								accessibleTreasures.UnionWith(TreasureConditions.LateCrown);
							}
							if (questItems.Contains(Items.Slab))
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
        /// Data[ItemLocations.KingConeria.Address] = Items.Key;
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
        /*
        ;;;;;;;;;;;;;;;;;;;;;;;;;;;;
        ;;
        ;;  Draw Dialogue String @PrintName $DBF8
        ;;  unused code 3, 31 bytes of space available until @Code_Not03
        ;;;;;;;;;;;;;;;;;;;;;;;;;;;;
            LDA dlg_itemid      ; get the item ID whose name we're to print A561
            ADC #$20            ; Add 0x20 to account for the 'items' offset 6920
            BCC :+              ; if it is set 901D
              ASL A             ; 0A
              ADC #$5A          ; Add 0x5A 695A
              STA text_ptr      ; 853E
              LDA #$82          ; A982
              STA text_ptr+1    ; 853F
                CLC             ; 18
            JMP @Loop           ; and continue printing (to print the name, then quit) 4C9ADB
         */

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
        /// Data[ItemLocations.KingConeria.Address] = Items.Key;
        /// </summary>
        public void EnableNPCsGiveAnyItem(bool bridgeShipCanalAnywhereEnabled = true)
        {
            SplitOpenTreasureRoutine();
            var controlCode = bridgeShipCanalAnywhereEnabled ? "03" : "02";
            // Replace Don't be greedy text with NPC item text "Here, take this"
            var replacementText =
                "91A823" + // Here
                "BF1BA4AE1A" + // , take 
                "31C005" + // this\n
                $"{controlCode}00";
            Put(0x28F35, Blob.FromHex(replacementText));

            // When getting jump address from lut_MapObjTalkJumpTbl (starting 0x3902B), store
            // it in tmp+4 & tmp+5 instead of tmp+6 & tmp+7 so that tmp+6
            // will still have the mapobj_id (allowing optimizations in TalkRoutines)
            Data[0x39063] = 0x14;
            Data[0x39068] = 0x15;
            Data[0x3906A] = 0x14;
            Data[0x39070] = 0x14;
            Data[0x39075] = 0x15;
            Data[0x39077] = 0x14;

            // New routine for NPC item trades
            var itemTradeNPCRoutine =
                "A5106920AABD0060F014" +
                "A513F01020" + giveRewardRoutineAddress +
                "B00DA5106920AADE0060" +
                "A93A60" +
                "A51160";
            var itemTradeRoutineAddress = "6C93";
            // Put at Smith routine
            Put(0x3936C, Blob.FromHex(itemTradeNPCRoutine));
         /*
         ;; New method for npc item exchanges, can be placed at Talk_Smith
         ;; Input:
         ;;      tmp   = Required item index, if 0 then nothing is required
         ;;      tmp+1 = Default text if we don't try to give the item
         ;;      tmp+2 = Unused
         ;;      tmp+3 = Item to give, if 0 then no item is given or taken
         ;;      tmp+6 = object ID
         ;; Output:
         ;;      A          = Dialog ID to print
         ;;      dlg_itemid = item ID to print in dialog (if applicable)
         StandardNPCItemTrade:           ; (33 bytes)
             LDA tmp                     ; check required item A510
             ADC #$20                    ; offset for unsram checks 6920
             TAX                         ; AA
             LDA unsram, X               ; BD0060
             BEQ @Default                ; F014
               LDA tmp+3                 ; load item to give A513
               BEQ @Default              ; if there's an item to give F010
                 JSR GiveItem            ; give it 2094DD
                 BCS @End                ; if we don't already have it (Can't hold text) B00D
                 LDA tmp                 ; check required item A510
                 ADC #$20                ; offset for unsram checks 6920
                 TAX                     ; AA
                 DEC unsram, X           ; DE0060 (take the item)
                 LDA #$3A                ; The NPC generic item gift text A93A
                 RTS                     ; 60
         @Default:
             LDA tmp+1                   ; otherwise print default text A511
         @End:
             RTS                         ; 60
         */

            // New routine for NPC items based on game event flag
            var eventFlagGiveNPCRoutine =
                "A41098F0052079909007" +
                "A4162079909003A51260" +
                "A51320" + giveRewardRoutineAddress +
                "B007A416" +
                "207F90A93A60";
            var eventFlagRoutineAddress = "8695";
            // Put at CubeBotBad and overruns into Lefein
            Put(0x39586, Blob.FromHex(eventFlagGiveNPCRoutine));
            Put(lut_MapObjTalkJumpTblAddress + 2 * ObjectIds.Lefein, Blob.FromHex(eventFlagRoutineAddress));
         /*
         ;; New method for npc item gifts, can be placed at Talk_CubeBotBad ($9586) and overrunning into Talk_Chime
         ;; Input:
         ;;      tmp   = Required game event flag index (if applicable)
         ;;      tmp+1 = Flag to set (equivalent to marking treasure chest as open)
         ;;      tmp+2 = Default text if we don't try to give the item
         ;;      tmp+3 = Item to give, if 0 then no item is given or taken
         ;;      tmp+6 = object ID
         ;; Output:
         ;;      A          = Dialog ID to print
         ;;      dlg_itemid = item ID to print in dialog (if applicable)
         StandardNPCItemGameEvent:       ; (35 bytes)
             LDY tmp                     ; check required event flag A410
             TYA                         ; 98
             BEQ :+                      ; if it's zero jump ahead F005
               JSR CheckGameEventFlag    ; 207990
               BCC @Default              ; if not set, show default 9007
         :   LDY tmp+6                   ; A416
             JSR CheckGameEventFlag      ; Check this object's event flag 207990
             BCC :+                      ; if it is set, 9003
             @Default:
               LDA tmp+2                 ; print default text A512
               RTS                       ; 60
         :   LDA tmp+3                   ; load item to give A513
             JSR GiveItem                ; give item 2094DD
             BCS :+                      ; if we don't already have it (Can't hold text) B007
               LDY tmp+6                 ; A416
               JSR SetGameEventFlag      ; 207F90
               LDA #$3A                  ; The NPC generic item gift text A93A
         :   RTS                         ; 60
         */
            // *** Mandatory cases (Smith and Lefein)
            // Smith and Lefein are the only NPCs required to use new routines since theirs were overwritten

            // updates to lut_MapObjTalkData
            // Update default text position from index 3 to index 2
            Data[ItemLocations.Lefein.Address - 1] = Data[ItemLocations.Lefein.Address];
            // set required item/event flag for trade
            Data[ItemLocations.SmithWeapon.Address - 3] = Items.Adamant;
            Data[ItemLocations.Lefein.Address - 3] = ObjectIds.Unne;
            // *** End Mandatory cases

            // *** Handle special cases (Prince, King, Matoya, and Nerrick)
            // EnableKingAnyItem
            Data[ItemLocations.KingConeria.Address - 1] = Data[ItemLocations.KingConeria.Address];
            Put(lut_MapObjTalkJumpTblAddress + 2 * ObjectIds.King, Blob.FromHex(eventFlagRoutineAddress));

            EnableBikkeAnyItem();

            // EnableMatoyaAnyItem
            Data[ItemLocations.Matoya.Address - 3] = Items.Crystal;
            Put(lut_MapObjTalkJumpTblAddress + 2 * ObjectIds.Matoya, Blob.FromHex(itemTradeRoutineAddress));

            EnableAstosAnyItem();

            // EnableElfPrinceAnyItem
            // Update default text position from index 3 to index 2
            Data[ItemLocations.ElfPrince.Address - 1] = Data[ItemLocations.ElfPrince.Address];
            // Set the external flags to check
            Data[ItemLocations.ElfPrince.Address - 3] = ObjectIds.ElfDoc;
            // And ElfDoc sets his own flag instead of prince's
            Data[0x39302] = ObjectIds.ElfDoc;
            Put(lut_MapObjTalkJumpTblAddress + 2 * ObjectIds.ElfPrince, Blob.FromHex(eventFlagRoutineAddress));

            // EnableNerrickAnyItem
            Data[ItemLocations.Nerrick.Address - 3] = Items.Tnt;
            Put(lut_MapObjTalkJumpTblAddress + 2 * ObjectIds.Nerrick, Blob.FromHex(itemTradeRoutineAddress));
            // *** End Special cases

            // *** Normal cases (Sarda, CubeBot, Princess, Fairy, CanoeSage)
            Put(lut_MapObjTalkJumpTblAddress + 2 * ObjectIds.Sarda, Blob.FromHex(eventFlagRoutineAddress));
            Put(lut_MapObjTalkJumpTblAddress + 2 * ObjectIds.CubeBot, Blob.FromHex(eventFlagRoutineAddress));
            Put(lut_MapObjTalkJumpTblAddress + 2 * ObjectIds.Princess2, Blob.FromHex(eventFlagRoutineAddress));
            Put(lut_MapObjTalkJumpTblAddress + 2 * ObjectIds.Fairy, Blob.FromHex(eventFlagRoutineAddress));
            Put(lut_MapObjTalkJumpTblAddress + 2 * ObjectIds.CanoeSage, Blob.FromHex(eventFlagRoutineAddress)); 
        }

        private void EnableAstosAnyItem()
        {
            // Set required item at index 0
            Data[ItemLocations.Astos.Address - 3] = Items.Crown;
            var newAstosRoutine =
                "AD2260F016A513F01220" + giveRewardRoutineAddress +
                "B00FA007207392A97F" +
                "20C590A93A60A51160";
            Put(0x39338, Blob.FromHex(newAstosRoutine));
        /*
        ;; Rewrite Astos $9338
        ;;  [1] if you don't have the Crown
            LDA item_crown              ; check required item AD2260
            BEQ @Default                ; F016
              LDA tmp+3                 ; load item to give A513
              BEQ @Default              ; if there's an item to give F012
                JSR GiveItem            ; give it 2094DD
                BCS @End                ; if we don't already have it (Can't hold text) B00F
                LDY #OBJID_ASTOS        ; A007
                JSR HideMapObject       ; hide (kill) Astos' map object (this object) 207392
                LDA #BTL_ASTOS          ; trigger battle with Astos A97D
                JSR TalkBattle          ; 20C590
                LDA #$3A                ; The NPC generic item gift text A93A
                RTS                     ; 60
        @Default:
            LDA tmp+1                   ; otherwise print default text A511
        @End:
            RTS                         ; 60 
        */
        }

        private void EnableBikkeAnyItem()
        {
            // Move default text position from index 3 to index 2
            Data[ItemLocations.Bikke.Address - 1] = Data[ItemLocations.Bikke.Address];
            var newBikkeRoutine =
                "A03F209190B00B" +
                "20A490A97F" +
                "20C590A51160" +
                "A004207990B013A513F00F841020" + giveRewardRoutineAddress +
                "B00AA410207F90A93A60A51260";
            Put(0x392D0, Blob.FromHex(newBikkeRoutine));
        /*
        ;; Rewrite Bikke  $92D0
        ;;  [1] if haven't fought him yet
        ;;  [2] if fought him but haven't taken his ship yet
        ;;  [3] after you have the ship
            LDY #OBJID_PIRATETERR_1      ; A03F
            JSR IsObjectVisible          ; 209190
            BCS @AlreadyFought           ; if we already have, skip ahead B00B
              JSR ShowMapObject          ; and show a bunch of scaredy-cat townspeople that the pirates 20A490
              LDA #BTL_BIKKE             ; then start a battle with Bikke (his pirates) A97E
              JSR TalkBattle             ; 20C590
              LDA tmp+1                  ; and print [1] A511
              RTS                        ; 60
          @AlreadyFought:                ; if we've already fought bikke...
            LDY #OBJID_BIKKE             ; A004
            JSR CheckGameEventFlag       ; check Bikke's event flag to see if we got item from him yet 207990
            BCS @Default                 ; if we already have, skip ahead B013
              LDA tmp+3                  ; load item to give A513
              BEQ @Default               ; if there's an item to give F00F
              STY tmp                    ; 8410
              JSR GiveItem               ; give it 2094DD
              BCS @End                   ; if we don't already have it (Can't hold text) B00A
              LDY tmp                    ; A410
              JSR SetGameEventFlag       ; otherwise, set event flag to mark him as done 207F90
              LDA #$3A                   ; The NPC generic item gift text A93A
              RTS                        ; 60
          @Default:                      ; otherwise, if we have the ship already
            LDA tmp+2                    ; just print [3] A512
          @End:
            RTS                          ; 60 
        */
        }

        private void SplitOpenTreasureRoutine()
        {
            // Replace "OpenTreasureChest" routine
            var openTreasureChest =
                $"A9002003FEA645BD00B120{giveRewardRoutineAddress}" +
                "B00AA445B9006209049900628A60"; // 27 bytes
            Put(0x7DD78, Blob.FromHex(openTreasureChest));
            /*
            OpenTreasureChest:           ; (27 bytes)
                LDA #BANK_TREASURE       ; swap to bank containing treasure chest info A900
                JSR SwapPRG_L            ; 2003FE

                LDX tileprop+1           ; put chest index in X A645
                LDA lut_Treasure, X      ; use it to get the contents of the chest BD00B1

                JSR GiveReward            ; Jump to new sub routine 2094DD
                BCS :+                   ; if 'C' is set jump ahead, otherwise mark the chest as open B00A
                  LDY tileprop+1           ; get the ID of this chest A445
                  LDA game_flags, Y        ; flip on the TCOPEN flag to mark this TC as open B90062
                  ORA #GMFLG_TCOPEN        ; 0904
                  STA game_flags, Y        ; 990062
            :
                TXA                        ; 8A
                RTS                        ; 60 
            */

            // New "GiveReward" routine
            const string checkItem =
                "85616920C93CB013AAC90CD005" +
                "DE0060B002FE0060C936B0229023"; // 27 bytes
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
            /*
            ;; $DD93
            ;; New jump point for NPC-only items:
            ;; IN:
            ;;       'A' should be set to the item ID, 224-255 are special values for variables
            ;; Result:
            ;;       'C' set if can't carry any more, otherwise clear
            ;;       'X' Dialog ID
            ;;       'A' Also Dialog ID
            GiveReward:                    ; (8 bytes)
                STA dlg_itemid             ; record that as the item id so it can be printed in the dialogue box 8561
                ADC #$20                   ; Add 0x20 to account for the 'items' offset 6920
                CMP #$3C                   ; see if the ID is >= item_stop C93C
                BCS @NotItem               ; B013
              @Item:                       ; (5 + 15 + 7 bytes)
                TAX                        ; put item ID in X AA
                CMP #$0C                   ; then check for canal C90C
                BNE :+                     ; If canal then D005
                  DEC unsram, X            ; decrement DE0060
                  BCS @OpenChest           ; and open it B002
            :   INC unsram, X              ; otherwise give them one of this item FE0060
                CMP #$36                   ; if >= item_qty_start then play regular jingle C936
                BCS @ClearChest            ; B022
                BCC @OpenChest             ; 9023
              @NotItem:                    ; (6 + 9 + 4 + 9 + 7 + 5 = 40 bytes)
                LDA dlg_itemid             ; restore item id A561
                CMP #$6C                   ; check if gold C96C
                BCC :+                     ; Continue if gold 9009
                 JSR LoadPrice             ; get the price of the item (the amount of gold in the chest) 20B9EC
                 JSR AddGPToParty          ; add that price to the party's GP 20EADD
                 JMP @ClearChest           ; then mark the chest as open, and exit 4CDEDD
            :   CMP #$44                   ; >= 68 means it's armor C944
                BCS :+                     ; B009
                  JSR FindEmptyWeaponSlot  ; Find an available slot to place this weapon in 2034DD
                  BCS @TooFull             ; if there are no available slots, jump to 'Too Full' message B007
                  LDA #$E5                 ; convert to index where 1 is first weapon A9E5
                  BCC @EquipmentGet        ; 9007
                JSR FindEmptyArmorSlot     ; Find an empty slot to put this armor 2046DD
                BCS @TooFull               ;  if there are no available slots, jump to 'Too Full' message B00C
                LDA #$BD                   ; convert to index where 1 is first weapon/armor A9BD
              @EquipmentGet:               ; 'A' should hold the equipment ID and 'X' the item slot
                ADC dlg_itemid             ; 6561
                STA ch_stats, X            ; add it to the previously found empty slot 9D0061
              @ClearChest:                 ; Cleanup, set jingle and dialog id (12 bytes)
                CLC                        ; 18
              @OpenRegularChest:           ;  then continue on to mark the chest as open
                INC dlgsfx                 ; set dlgsfx to play the TC jingle E67D
              @OpenChest:                  ;
                INC dlgsfx                 ; set dlgsfx to play the TC jingle E67D
              @TooFull:                    ; jump here with C set to show "Can't Hold" text and no jingle
                LDX #DLGID_TCGET           ; and select "In This chest you found..." text A2F0
                BCC :+                     ; 9004
                  INC $60B7                ; EEB760
                  INX                      ; E8
            :   TXA                        ; 8A
                RTS                        ; 60 
            */
        }
    }
}
