 ;; New method for npc item exchanges, placed a few bytes after new Astos
 ;; routine, overwriting the rest of Talk_Nerrick, and a lot of Talk_Smith as well  
 ;; ($9356)
 ;; Input:
 ;;      tmp   = Required item index, if 0 then nothing is required
 ;;      tmp+1 = Default text if we don't try to give the item
 ;;      tmp+2 = Unused
 ;;      tmp+3 = Item to give, if 0 then no item is given or taken
 ;;      tmp+6 = object ID
 ;; Output:
 ;;      A          = Dialog ID to print
 ;;      dlg_itemid = item ID to print in dialog (if applicable)
 StandardNPCItemTrade:           ; (49 bytes)
     LDY tmp+6                   ; A416
     JSR CheckGameEventFlag      ; Check this object's event flag 207990
     BCS @Default                ; if it is set, B027
     LDA tmp                     ; check required item A510
     ADC #$20                    ; offset for unsram checks 6920
     TAX                         ; AA
     STX tmp+7                   ; 8617
     LDA unsram, X               ; BD0060
     BEQ @Default                ; F01D
       LDA tmp+3                 ; load item to give A513
       BEQ @Default              ; if there's an item to give F019
         JSR GiveItem            ; give it 2094DD
         BCS @End                ; if we don't already have it (Can't hold text) B016
         LDA tmp+7               ; check required item A517
         CMP #$31                ; if >= item_canoe then skip ahead C931
         BCS :+                  ; B004
         TAX                     ; AA
         DEC unsram, X           ; DE0060 (take the item)
         LDY tmp+6               ; A416
         JSR SetGameEventFlag    ; 207F90
:        LDA #$3A                ; The NPC generic item gift text A93A
         RTS                     ; 60
 @Default:
     LDA tmp+1                   ; otherwise print default text A511
 @End:
     RTS                         ; 60