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
      BCS @WasCanal           ; and open it B003
:   INC unsram, X              ; otherwise give them one of this item FE0060
  @WasCanal
    CMP #$31                   ; if >= item_canoe (shard) then play regular jingle C936
    BCS @ClearChest            ; B02A
    BCC @OpenChest             ; 902B
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
      INC $60B9                ; EEB760
      INX                      ; E8
:   TXA                        ; 8A
    RTS                        ; 60
