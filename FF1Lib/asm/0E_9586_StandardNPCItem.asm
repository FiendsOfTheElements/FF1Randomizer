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
 StandardNPCItem:       ; (35 bytes)
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