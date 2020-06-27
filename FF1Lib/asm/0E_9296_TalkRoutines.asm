;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;
;;
;;  TalkRoutines  [$9296 :: 0x392A6]
;;
;;  This create 3 new talk routines
;;   - Talk_GiveItemOnFlag : Allow a NPC to give any item if the required flag is enabled
;;   - Talk_GiveItemOnItem : Allow a NPC to give any item if the required item is owned by the player
;;   - Talk_TradeItems :  Allow a NPC to give any item if the required item is owned; the required item is then taken
;;
;;  Other routines are deleted since their function can be reproduced by other routines (Talk_Garland > talk_fight)
;;
;;  This replace the modified routines: 0E_92D0_BikkeAnyItem.asm; 0E_9338_AstosAnyItem.asm; 0E_9356_StandardNPCItemTrade.asm; 0E_956B_StandardNPCItem.asm
  
unsram 		= $6000
items = unsram + $20
game_flags      = unsram + $0200 
tmp = $10
shop_curprice   = $030E 
item_lute       = items + $01
item_crown      = items + $02
item_crystal    = items + $03
item_herb       = items + $04
item_mystickey  = items + $05
item_tnt        = items + $06
item_adamant    = items + $07
item_slab       = items + $08
item_ruby       = items + $09
item_rod        = items + $0A
item_floater    = items + $0B
item_chime      = items + $0C
item_tail       = items + $0D
item_cube       = items + $0E
item_bottle     = items + $0F
item_oxyale     = items + $10
;item_canoe      = items + $11
item_orb_start  = items + $11
orb_earth       = item_orb_start + 0
orb_fire        = item_orb_start + 1
orb_water       = item_orb_start + 2
orb_air         = item_orb_start + 3
bridge_vis = unsram + $08

dlgsfx = $7D

CheckGameEventFlag = $9079
SetGameEventFlag = $907F
IsObjectVisible = $9091
HideThisMapObject = $9096
ShowMapObject = $90A4
TalkBattle = $90C5
TalkNormTeleport = $90CC
HideMapObject = $9273
DoClassChange = $95AE
GiveItem = $DD93

BTL_VAMPIRE        = $7C
BTL_ASTOS          = $7D
BTL_BIKKE          = $7E
BTL_GARLAND        = $7F

NORMTELE_SAVEDPRINCESS = $3F

OBJID_PRINCESS_1   = $03   ; kidnapped princess (in ToF)
OBJID_VAMPIRE      = $0C   ; Earth Cave's Vampire
OBJID_BAHAMUT      = $0E   ; Bahamut
OBJID_PRINCESS_2   = $12   ; rescued princess (in Coneria Castle)

OBJID_PIRATETERR_1 = $3F   ; townspeople that were terrorized by the
OBJID_PIRATETERR_2 = $40   ;   pirates... they don't become visible until after
OBJID_PIRATETERR_3 = $41   ;   you beat Bikke and claim the ship

OBJID_BLACKORB     = $CA


;; none (Blank sprite -- object ID=0)
Talk_None:
  RTS

;; Generic eventless object 
;;  [1] always
Talk_norm:
  LDA tmp+1             ; uneventful object -- just print [1]
  RTS
  
;; Generic condition check based on object visibility 
;;  [1] if object ID [0] is hidden
;;  [2] if it's visible
Talk_ifvis:
  LDY tmp                 ; check to see if object [0] is visible
  JSR IsObjectVisible
  BCS IsVis               ; if it is, print [2]
    LDA tmp+1             ; otherwise, print [1]
    RTS
IsVis:     
  LDA tmp+2
  RTS
;; Generic condition based on item index
;;  [1] if party contains at least one of item index [0]
;;  [2] otherwise (none of that item)
Talk_ifitem:
  LDA tmp              ; use [0] as an item index
  ADC #$20             ; check also for SHIP, CANOE, BRIDGE, CANAL and AIRSHIP
  TAY
  LDA unsram, Y
  BEQ DontHave        ; if they do have it
    LDA tmp+1          ; print [1]
    RTS
DontHave:
  LDA tmp+2            ; otherwise, print [2]
  RTS

;; Invisible Lady  (infamous invisible lady in Coneria Castle) 
;;  [1] if princess not rescued and you don't have the Lute
;;  [2] if princess rescued or you do have the Lute
Talk_Invis:
  LDY #OBJID_PRINCESS_2
  JSR IsObjectVisible      ; see if the princess has been rescued (rescued princess object visible)
  BCS IsVis               ; if she's not rescued...
    LDA item_lute
    BNE IsVis             ; ... and if you don't have the lute (redundant)
      LDA tmp+1            ; print [1]
      RTS
IsVis:
  LDA tmp+2                ; otherwise print [2]
  RTS

;; Generic condition based on game event flag 
;;  [1] if game event flag ID [0] is clear
;;  [2] if it's set
Talk_ifevent:
  LDY tmp                 ; use [0] as an event flag index
  JSR CheckGameEventFlag  ;  see if that event flag has been set
  BCS Event                  ; if not...
    LDA tmp+1             ; ... print [1]
    RTS
Event:
  LDA tmp+2               ; otherwise print [2]
  RTS

;; Some guard in Coneria town 
;;  [1] if princess has been saved, but bridge isn't built yet
;;  [2] if princess still kidnapped or bridge is built
Talk_GoBridge:
  LDY #OBJID_PRINCESS_2   ; check to see if princess has been rescued
  JSR IsObjectVisible
  BCC NotSaved                  ; if she has...
    LDA bridge_vis        ; see if bridge has been built
    BNE NotSaved                ; if not... (princess saved, but bridge not built yet...)
      LDA tmp+1           ;  ... print [1]
      RTS
NotSaved:        
  LDA tmp+2               ; otherwise print [2]
  RTS
  
;; Conditional Check for 4 Orbs 
;;  [1] if all 4 orbs lit
;;  [2] otherwise
Talk_4Orb:
  LDA orb_fire        ; see if all orbs have been lit
  AND orb_water
  AND orb_air
  AND orb_earth
  BEQ NotAllLit              ; if they have...
    LDA tmp+1         ; print [1]
    RTS
NotAllLit:      
  LDA tmp+2           ; otherwise (not all of them lit)
  RTS                 ;  print [2]

;; Conditional check for key+TNT  (some dwarf) 
;;  [1] if have key, but not TNT
;;  [2] if no key, or have TNT
Talk_ifkeytnt:
  LDA item_mystickey  ; check to see if the party has the key
  BEQ Default              ; if they do...
    LDA item_tnt      ; check to see if they have the TNT
    BNE Default            ; if they don't  (key but no TNT)
      LDA tmp+1       ;   print [1]
      RTS
Default:        
  LDA tmp+2           ; otherwise, print [2]
  RTS

;; Conditional check for Earth Orb and Vampire (people in Melmond) [$9559 :: 0x39569]
;;  [1] if Vampire is dead and Earth Orb not lit
;;  [2] if Vampire lives, or Earth Orb has been lit
Talk_ifearthvamp:
  LDY #OBJID_VAMPIRE    ; see if the vampire has been killed yet
  JSR IsObjectVisible
  BCS Default                ; if not...
    LDA orb_earth       ; check to see if player revived earth orb
    BNE DefaultC              ; if not... (Vampire killed, Earth Orb not lit yet)
      LDA tmp+1         ; print [1]
      RTS
Default:
  LDA tmp+2             ; otherwise print [2]
  RTS

;; Conditional check for earth/fire orbs
;;  [1] if Earth Orb not lit, or Fire Orb Lit
;;  [2] if Earth Orb lit, and Fire Obj not lit
Talk_ifearthfire:
  LDA orb_earth        ; see if the Earth Orb has been recovered
  BEQ Default               ; if it has...
    LDA orb_fire       ; check Fire Orb
    BNE Default             ; if it's still dark (Earth lit, but not Fire)
      LDA tmp+1        ; print [1]
      RTS
Default:        
  LDA tmp+2            ; otherwise, print [2]
  RTS

;; Replacable Object (first 2 of the 3 ToFR Garlands)
;;  [1] always --- hide THIS object whose ID is [0], and show object ID [3]
Talk_Replace:
  LDY tmp+6             ; get object ID from [0]
  JSR HideThisMapObject ; hide that object (this object)
  LDY tmp+3
  JSR ShowMapObject     ; show object ID [3]
  LDA tmp+1             ; and print [1]
  RTS

;; Mysterious Sage (in the CoO -- disappears after you talk to him) 
;;  [1] always --- hide THIS object whose ID is [0]
Talk_CoOGuy:
  LDY tmp+6
  JSR HideThisMapObject ; hide object ID [0] (this object)
  LDA tmp+1             ; and print [1]
  RTS

;; Generic fight 
;;  [1] always --- hide THIS object whose ID is [0], and initiate battle ID [3]
Talk_fight:
  LDY tmp+6
  JSR SetGameEventFlag
  JSR HideThisMapObject ; hide object [0] (this object)
  LDA tmp+3
  JSR TalkBattle        ; trigger battle ID [3]
  LDA tmp+1             ; and print [1]
  RTS
 
;; The Black Orb 
;;  [1] if all 4 orbs are lit
;;  [2] otherwise
Talk_BlackOrb:
  LDA orb_fire            ; see if all orbs have been lit
  AND orb_water
  AND orb_air
  AND orb_earth
  BEQ NotAllLit          ; if all of them are lit...
    LDY #OBJID_BLACKORB   ; hide the black orb object (this object)
    JSR HideThisMapObject
    INC dlgsfx            ; play TC sound effect  (not fanfare)
    INC dlgsfx
    LDA tmp+1             ; and print [1]
    RTS
NotAllLit:
  LDA tmp+2               ; otherwise, (not all orbs lit), print [2]
  RTS
      
;; Kidnapped Princess (in the ToF)
Talk_Princess1:
  LDY tmp+6               ; Load this object (the princess)
  JSR SetGameEventFlag    ; Set its flag
  JSR HideThisMapObject   ; Hide it
  LDY #OBJID_PRINCESS_2   ; Load saved princess
  JSR ShowMapObject       ; Show
  LDA #NORMTELE_SAVEDPRINCESS  ; Trigger the teleport back to Coneria Castle
  JSR TalkNormTeleport
  LDA tmp+1               ; Print [1]
  RTS

;; Submarine Engineer (in Onrac, blocking enterance to Sea Shrine) 
;;  [1] if you don't have the Oxyale
;;  [2] if you do
Talk_SubEng:
  LDA item_oxyale         ; see if the player has the Oxyale
  BNE HaveOxyale                 ; if they don't...
    LDA tmp+1             ; ...print [1]
    RTS
HaveOxyale:      
  LDY tmp+6               ; otherwise (they do)
  JSR HideThisMapObject   ; hide the sub engineer object (this object)
  LDA tmp+2               ; and print [2]
  RTS

;; Titan
;;  [1] if you don't have the Ruby
;;  [2] if you do
Talk_Titan:
  LDA item_ruby          ; does the player have the ruby?
  BNE HaveRuby           ; if not...
    LDA tmp+1            ; ... simply print [1]
    RTS
HaveRuby:      
  DEC item_ruby          ; if they do have it, take it away
  LDY tmp+6              ; hide/remove Titan (this object)
  JSR HideThisMapObject
  LDA tmp+2              ; print [2]
  INC dlgsfx             ; and play fanfare
  RTS
    
;; Bikke the Pirate 
;;  [0] after you have the item
;;  [1] if haven't fought him yet
;;  [2] giving the item
;;  [3] item to give
Talk_Bikke:
    LDY #OBJID_PIRATETERR_1   ; Check if the hiding townsfolk are visible
    JSR IsObjectVisible
    BCS AlreadyFought         ; if not...   
      JSR ShowMapObject       ; Show them
      LDY #OBJID_PIRATETERR_2 
      JSR ShowMapObject
      LDY #OBJID_PIRATETERR_3
      JSR ShowMapObject
      LDA #BTL_BIKKE          ; Load Bikke's Battle
      JSR TalkBattle
      LDA tmp+1               ; Print [1]
      RTS
AlreadyFought:
   LDY tmp+6                  ; Check Bikke's flag
   JSR CheckGameEventFlag 
   BCS Default                ; If it's not set...
     LDA tmp+3                ; Load item to give [3]
     BEQ Default              ; If there's one to give
       LDY tmp+2              ; We store [2] in [4], becauce GiveItem
       STY tmp+4              ;  will overwrite it if its giving gold
       JSR GiveItem           ; Give it
       BCS End1               ; Can't hold
       LDY tmp+6              ; Set Bikke's flag
       JSR SetGameEventFlag   
       LDA tmp+4              ; Print [4] = [2]
       RTS
Default:
  LDA tmp                     ; Print [0]
End1:
  RTS

;; Nerrick (dwarf who opens the Canal) 
;;  [0] if you don't have the TNT
;;  [1] unused
;;  [2] giving item
;;  [3] item to give
Talk_Nerrick:
  LDA item_tnt           ; check to see if the player has TNT
  BEQ Default            ; if not...
    LDA tmp+3            ; load item to give [3]
    BEQ Default          ; if there's one
      LDY tmp+2          ; We store [2] in [4], becauce GiveItem
      STY tmp+4          ;  will overwrite it if its giving gold
      JSR GiveItem       ; Give it
      BCS End            ; Can't hold
      DEC item_tnt       ; otherwise, remove the TNT from the party
      LDY tmp+6          ;  
      JSR HideMapObject  ; Hide Nerrick
      LDA tmp+4          ; Print [4] = [2]
      RTS
Default:
  LDA tmp                ; Print [0]
End:
  RTS
  
;; Bahamut 
;;  [1] if haven't been promoted, and don't have the Tail
;;  [2] if haven't been promoted, and DO have the Tail
;;  [3] once promoted
Talk_Bahamut:
  LDY #OBJID_BAHAMUT      ; Check Bahamut's Event flag (see if he promoted you yet)
  JSR CheckGameEventFlag
  BCC CheckTail           ; if he has...
    LDA tmp+3             ; ... print [3]
    RTS
CheckTail:
  LDA item_tail           ; he hasn't promoted you yet... so check to see if you have the tail
  BNE ClassChange         ; if you don't...
    LDA tmp+1             ; ... print [1]
    RTS
ClassChange:              ; otherwise (have tail), do the class change!
  DEC item_tail           ; remove the tail from inventory
  JSR SetGameEventFlag    ; set Bahamut's event flag
  JSR DoClassChange       ; do class change
  INC dlgsfx              ; play fanfare
  LDA tmp+2               ; and print [2]
  RTS

;; Elf Doctor and Unne
;;  [0] if you don't have the required item
;;  [1] if you have given it
;;  [2] if you do have the item
Talk_ElfDocUnne:
  LDY tmp+6                   ; Check flag
  JSR CheckGameEventFlag
  BCS ItemGiven               ; If it's not set...
    LDX lut_keyNPCreq, Y      ; Load required item
    LDA items, X           
    BEQ Default              ; If we have it...
      DEC items, X            ; Take it away
      JSR SetGameEventFlag    ; Set flag
      INC dlgsfx              ; Play jingle
      LDA tmp+2               ; Print [2]
      RTS
ItemGiven:
  LDA tmp+1                   ; Print [1]
  RTS
Default:
  LDA tmp                     ; Print [0]
  RTS
  
;; Give Item on Flag
;; NPC give an item if the right flag is set
;;  [0] If it's not set
;;  [1] item given
;;  [2] giving the item
;;  [3] Item to give
Talk_GiveItemOnFlag:
  LDY tmp+6
  JSR CheckGameEventFlag
  BCS ItemGiven                  ; If it's not set...
    LDA lut_keyNPCreq, Y
    BEQ NoFlag                   ; If there's no flag, go ahead with giving the item
      TAY
      JSR CheckGameEventFlag     ; If there is, check if it's set
      BCC Default                ; If not, end 
NoFlag:
      LDA tmp+3                  ; Load item to give from [3]
      BEQ Default
        LDY tmp+2
        STY tmp+4
        CLC
        JSR GiveItem             ; Give it
        BCS End                  ; Can't hold
        LDY tmp+6                ; Set this object's flag
        JSR SetGameEventFlag 
        LDA tmp+4                ; Print [4] = [2]
        RTS
ItemGiven:
  LDA tmp+1                      ; If it's set, print [1]
  RTS
Default:
  LDA tmp                        ; Print [0]
End:
  RTS
  
;; Trade quest item, trade an item for another one 
;;  [0] if you don't have the item
;;  [1] item given
;;  [2] giving item
;;  [3] item to give
Talk_TradeItems:
  LDY tmp+6                   ; A416
  JSR CheckGameEventFlag      ; Check this object's event flag 207990
  BCS ItemGiven               ; if it is set, B027
  LDX lut_keyNPCreq, Y        ; Load required item
  LDA items, X 
  BEQ Default                 ; F01B
    LDA tmp+3                 ; load item to give A513
    BEQ Default               ; if there's an item to give F017
      LDY tmp+2
      STY tmp+4
      JSR GiveItem            ; give it 2094DD
      BCS End                 ; if we don't already have it (Can't hold text) B014
      LDY tmp+6               ; Set this object flag 
      LDX lut_keyNPCreq, Y        ; Load required item
      DEC items, X            ; DE0060 (take the item)
      JSR SetGameEventFlag     
      LDA tmp+4               ; Print [4] = [2]
      RTS                     
ItemGiven:
  LDA tmp+1                      ; Print [1]
  RTS 
Default:
  LDA tmp                        ; Print [0]
End:
  RTS                            ; 60

;; Give Item on Item, don't take required item away
;;  [0] if you don't have the item
;;  [1] item given
;;  [2] giving item
;;  [3] item to give
Talk_GiveItemOnItem:
  LDY tmp+6                   ; A416
  JSR CheckGameEventFlag      ; Check this object's event flag 207990
  BCS ItemGiven               ; if it is set, B027
  LDX lut_keyNPCreq, Y        ; Load required item
  BEQ NoItem                  ; If there's no required item, go ahead with giving the item
  LDA items, X 
  BEQ Default                 ; F01B
NoItem:   
    LDA tmp+3                 ; load item to give A513
    BEQ Default               ; if there's an item to give F017
      LDY tmp+2
      STY tmp+4
      JSR GiveItem            ; give it 2094DD
      BCS End                 ; if we don't already have it (Can't hold text) B014
      LDY tmp+6               ; Set this object flag 
      JSR SetGameEventFlag     
      LDA tmp+4               ; Print [4] = [2]
      RTS                     
ItemGiven:
  LDA tmp+1                   ; Print [1]
  RTS 
Default:
  LDA tmp                     ; otherwise print [0]
End:
  RTS                         ; 60 
   
;; Astos
;;  [0] if you don't have the Crown
;;  [1] unused
;;  [2] giving item
;;  [3] item to give
      
Talk_Astos:
  LDY tmp+6
  LDX lut_keyNPCreq, Y            ; Load required item
  BEQ NoReq
    NOP
    LDA items, X
    BEQ Default                   ; Do we have it?
NoReq:    
      LDA tmp+3                   ; If yes, is there an item to give?
      BEQ Default  
        LDY tmp+2
        STY tmp+4                  
        CLC
        JSR GiveItem              ; Give item
        BCS End                   ; If inventory is full, skip
        STY $67                   ; Store Y in memory for SaveOnDeath, if it's equipment
        LDA tmp                   ; And store the amount for SaveOnDeath, if it's GP
        STA shop_curprice
        LDA tmp+1
        STA shop_curprice+1
        LDY tmp+6                 ; Load this object
        JSR SetGameEventFlag      ; Set its flag
        JSR HideMapObject         ; Hide it
        LDA #BTL_ASTOS            ; Load Astos' battle ID
        JSR TalkBattle            ; Do battle
        LDA tmp+4                 ; Show give item dialog > 4
        RTS
Default:
   LDA tmp                      ; Show default dialog
End:
   RTS

;; Talk for NPC guillotine, add extra safety so it doesn't break anything
;; [1] always
Talk_kill:
  LDY #$00
  JSR HideThisMapObject ; hide object ID [0] (this object)
  LDA tmp+1             ; and print [1]
  RTS


;; Lut to check items and flags requirements for NPC when using the Talk_GiveItem routines; this only allow for the 32 first NPCs to use them
 .ORG $9580
lut_keyNPCreq:
  .BYTE $00, $03, $00, $00, $00, $04, $05, $02, $06, $07, $03, $08, $00, $0C, $0D, $0B
  .BYTE $10, $00, $00, $00, $09, $11, $00, $00, $00, $00, $00, $00, $00, $00, $00, $00
  
