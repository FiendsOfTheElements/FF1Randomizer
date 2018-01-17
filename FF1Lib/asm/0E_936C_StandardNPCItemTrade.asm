 ;; New method for npc item exchanges, can be placed at Talk_Smith ($936C)
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