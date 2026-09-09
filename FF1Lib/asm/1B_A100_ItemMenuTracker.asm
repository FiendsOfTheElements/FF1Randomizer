tmp             = $10
event_flag      = tmp+$0D
icon_id1        = tmp+$0E
icon_id2        = tmp+$0F
dest_x          = $3A
dest_y          = $3B
ppu_dest        = $54
icon_buf        = $6E10
unsram          = $6000  ; $400 bytes
ship_vis        = unsram+$00
ship_x          = unsram+$01
ship_y          = unsram+$02
airship_vis     = unsram+$04
airship_x       = unsram+$05
airship_y       = unsram+$06
bridge_vis      = unsram+$08
bridge_x        = unsram+$09
bridge_y        = unsram+$0A
canal_vis       = unsram+$0C
canal_x         = unsram+$0D
canal_y         = unsram+$0E
has_canoe       = unsram+$12 ; (not to be confused with item_canoe)

items           = unsram+$20

item_lute       = items+$01
item_crown      = items+$02
item_crystal    = items+$03
item_herb       = items+$04
item_mystickey  = items+$05
item_tnt        = items+$06
item_adamant    = items+$07
item_slab       = items+$08
item_ruby       = items+$09
item_rod        = items+$0A
item_floater    = items+$0B
item_chime      = items+$0C
item_tail       = items+$0D
item_cube       = items+$0E
item_bottle     = items+$0F
item_oxyale     = items+$10
orb_earth       = items+$11

game_flags      = unsram+$200




DrawMainItemBox    = $B8EF ; bank 0E

CoordToNTAddr      = $DCAB ; bank 1F
MenuCondStall      = $E12E ; bank 1F
SwapPRG            = $FE03 ; bnak 1F

SOURCE_BANK = $0E
DEST_BANK   = $1B


;;;;;;;;;;;;;;;;;;;;;;
;; Tracker Icon Tiles
;;;;;;;;;;;;;;;;;;;;;;
BRIDGE   = $01
CANAL    = $02
SHIP     = $03
CANOE    = $04
MARK     = $04
AIRSHIP  = $05
FLOATER  = $06
SIGIL    = $06
CROWN    = $07
CRYSTAL  = $08
HERB     = $09
ELFDOC   = $0A
ADAMANT  = $0B
TNT      = $0C
RUBY     = $0D
TAIL     = $0E
BOTTLE   = $0F
FAIRY    = $10
SLAB     = $11
TRSLAB   = $12
ROD      = $13
LUTE     = $14
KEY      = $15
OXYALE   = $16
CHIME    = $17
CUBE     = $18
SARA     = $19
KING     = $1A
BIKKE    = $1B
SAGE     = $1C
SARDA    = $1D
ROBOT    = $1E
SHOP     = $1F
;SHOES    = $1F
;REPEL    = $20

EMPTYCH  = $74
FILLEDCH = $75
BLANK    = $FF

OBJID_KING         = $01   ; King of Coneria
OBJID_GARLAND      = $02   ; Garland (the first one, not ToFR)
OBJID_PRINCESS_1   = $03   ; kidnapped princess (in ToF)
OBJID_BIKKE        = $04   ; Bikke the Pirate
OBJID_ELFDOC       = $05   ; Attending the Elf Prince (more of a nurse, let's be honest)
OBJID_ELFPRINCE    = $06   ; Elf Prince (sleeping man-beauty)
OBJID_ASTOS        = $07   ; Astos -- the dark king!  omg scarey
OBJID_NERRICK      = $08   ; Nerrick -- the dwarf working on the canal
OBJID_SMITH        = $09   ; Smith, the dwarven blacksmith (no, he's not Watts)
OBJID_MATOYA       = $0A
OBJID_UNNE         = $0B   ; you've never heard of him?
OBJID_VAMPIRE      = $0C   ; Earth Cave's Vampire
OBJID_SARDA        = $0D
OBJID_BAHAMUT      = $0E   ; Bahamut
OBJID_LEFEIN       = $0F   ; Lefein chime guy
OBJID_SUBENGINEER  = $10   ; submarine block in Onrac
OBJID_CUBEBOT      = $11   ; waterfall robot
OBJID_PRINCESS_2   = $12   ; rescued princess (in Coneria Castle)
OBJID_FAIRY        = $13   ; fairy that appears from the bottle
OBJID_TITAN        = $14   ; Titan in Titan's Tunnel
OBJID_CANOESAGE    = $15   ; sage you get canoe from in vanilla
OBJID_RODPLATE     = $16   ; plate that is removed with the Rod
OBJID_LUTEPLATE    = $17   ; plate that is removed with the Lute

OBJID_SKYWAR_FIRST = $3A                   ; start of the 5 sky warriors
OBJID_SKYWAR_LAST  = OBJID_SKYWAR_FIRST+4  ; last of the 5 sky warriors

OBJID_PIRATETERR_1 = $3F   ; townspeople that were terrorized by the
OBJID_PIRATETERR_2 = $40   ;   pirates... they don't become visible until after
OBJID_PIRATETERR_3 = $41   ;   you beat Bikke and claim the ship

OBJID_REVEALBRIDGE = $FA   ; set when free bridge first appears on screen
OBJID_REVEALCANAL  = $FB   ; set when free canal first appears on screen
OBJID_REVEALSHIP   = $FC   ; set when free ship first appears on screen
OBJID_REVEALCANOE  = $FD   ; set when free canoe first appears on screen
OBJID_REVEALAIRSHIP= $FE   ; set when free airhsihp first appears on screen


OBJID_SHOPITEM     = $FF   ; a key item you buy from a shop

BUF_OFFSET         = 30    ; offset from buffer containing top row to buffer containing bottom row


.ORG $B12D ; bank $0E
LDA #07  ;; load the box ID (dimensions must be changed from $08 to $1C at $BAC0 in bank $0E)
JSR DrawItemTitleBox


.ORG $B91D ; bank $0E
DrawItemTitleBox:
  JSR DrawMainItemBox ;; dest_x & dest_y now set
  DEC dest_y ;; we need one row above the usual menu start location
  JMP ItemTrackerRedirect
  NOP
  NOP
  ; 10 bytes -- same as vanilla

.ORG $A0D0 ; bank $0E

ItemTrackerRedirect:
  JSR CoordToNTAddr ;; convert dest_x & dest_y to ppu address and store in ppu_dest (2 bytes)
  LDA #>(ItemTrackerInit-1)
  PHA
  LDA #<(ItemTrackerInit-1)
  PHA
  LDA #DEST_BANK
  JMP SwapPRG
  ; 14 bytes





.ORG $A100 ; bank $1B

;; written by the randomizer.
lut_Columns:
  .BYTE 26

;; written by the randomizer.
;; These signal the tracker when to show icons for the four NPCs that may or may not have game
;; flag or item requirements for them to give their item (or promote the party).
;; Affects:
;;  Early King Item
;;  Early Sage Item
;;  Early Sarda Item
;;  Fight Bahamut (Tail Removed)
;;
;; If any of the above flags are on, the NPC icon will appear immediately; otherwise it will appear
;; once its respective requirement is met.
;;
;; If "No Tri-state Spoilers" is on, the tracker will assume that the requirement is needed for any
;; tri-stated flags.
;; 
;; check flag  = #OBJID
;; check item  = item id (offset from items)
;; noreq       = #0
;; order: KING SAGE SARDA BAHAMUT
lut_NPCReqs: 
  .BYTE OBJID_PRINCESS_1 $11 OBJID_VAMPIRE $0D
  ; (4 bytes)

;; written by the randomizer.
;; These signal the tracker to immediately show a darkened icon for any items that are removed from the game.
;; Affects:
;;  Ship In Drydock (no airboat)
;;  Remove Floater (displays darkend airship)
;;  Remove Tail
;;
;; If "No Tri-state Spoilers" is on, the tracker will not show an icon for any tri-stated flags.
;;
;; show  = #1
;; don't = #0
;; order: FLOATER TAIL
lut_ShowRemoved:
  .BYTE 0 0

;; written by the randomizer.
;;
;; For free items that are not shown in inventory, display the icon in the tracker immediately. These are only
;; unset for tri-stated flags when "No Tri-state Spoilers" is on. In that case, the tracker will display the
;; icon only once the item is visible.
;; show            = #1
;; wait for reveal = #0
;; order: BRIDGE CANAL SHIP CANOE AIRSHIP
lut_ShowFree:
  .BYTE 1 1 1 1 1

;; reserve a few bytes for more lut settings



lut_Columns = $A100
lut_NPCReqs = $A101
lut_ShowRemoved = $A105
lut_ShowFree = $A107

.ORG $A110
;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;
;; SUBROUTINES
;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;

;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;
;; Tracker Logic begins here
;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;

ItemTrackerInit:
  LDA #BLANK
  LDX #59          
  initloop:         ; fill 60 icon_buf values with blank menu tile
    STA icon_buf,X
    DEX
    BPL initloop

  ; 13 bytes

  ;; begin processing items immediately:

ProcessItems:
  LDX #$00        ; start at icon_buf[0]
  ; 2 bytes


CheckBridge:
  LDA lut_ShowFree  ;
  BNE :+
    ; code reaches here if we have a free bridge but we don't want to spoil the tri-state
    LDY #OBJID_REVEALBRIDGE
    JSR CheckGameEventFlag ; has it been seen yet?
    BCC NoBridge ; if not, exit
  :
  LDA bridge_vis
  BEQ NoBridge
    LDA #BRIDGE
    STA icon_buf,X
  NoBridge:

;;; randomizer sets up the lut's above according to the following:
;;; ship can be "removed" -- still in the game, but parked in drydock by gaia. This can be tri-stated.
;;;    N.B. if airboat is on, we can fly out of drydock, so it's not removed
;;; ship can also be free (also possibly tri-stated)
;;;
;;; "Removed" is indicated by a darkened icon, but we don't need to worry about whether to show it on game start.
;;; Instead, we'll use the logic computed by the other flags.
;;; "No Tri-state Spoilers" might be on. This leads to a few possible flag combinations, which collapse down to a few behaviors in the asm.
;;;
;;; Some of the logic is computed in the randomizer, but the asm logic depends on the ship entry in lut_ShowFree                          
CheckShip:
  LDA lut_ShowFree+2 ; ship 
  BNE :+
    ; code reaches here if we have a free ship but we don't want to spoil the tri-state
    LDY #OBJID_REVEALSHIP
    JSR CheckGameEventFlag ; has it been seen yet?
    BCC NoShip ; if not, exit
  :
  LDA ship_vis
  BEQ NoShip
    LDA #SHIP
    STA icon_buf+BUF_OFFSET,X
  NoShip:
    INX

CheckCanal:
  LDA lut_ShowFree+1 ; canal
  BNE :+
    ; code reaches here if we have a free canal but we don't want to spoil the tri-state
    LDY #OBJID_REVEALCANAL
    JSR CheckGameEventFlag ; has it been seen yet?
    BCC NoCanal
  :
  LDA canal_vis
  BNE NoCanal  ; canal logic is flipped from the other overworld items
    LDA #CANAL
    STA icon_buf,X
  NoCanal:


CheckCanoe:
  LDA lut_ShowFree+3 ; canoe
  BNE :+
    ; code reaches here if we have a free canoe but we don't want to spoil the tri-state
    LDY #OBJID_REVEALCANOE
    JSR CheckGameEventFlag ; has it been seen yet?
    BCC NoCanoe
  :
  LDA has_canoe
  BEQ NoCanoe
    LDA #CANOE
    STA icon_buf+BUF_OFFSET,X
  NoCanoe:
    INX

  
;; With AirBoat on, the FLOATER and AIRSHIP icons are identical
;; Free airship/airboat is already spoiled by inventory, so we don't need to worry about that tri-state
;; Remove Floater can be tri-stated, and this shouldn't be spoiled if "No Tri-state Spoilers" is on.
CheckFloaterAirship:
  LDA lut_ShowRemoved ; if we're showing floater removed, we'll use a darkened airship icon,
  BNE ShowAirship     ; so we can just jump to display the airship
    LDA lut_ShowFree+4 ; airship
    BNE :+
      ; code reaches here if we have a free airship but we don't want to spoil the tri-state
      LDY #OBJID_REVEALAIRSHIP
      JSR CheckGameEventFlag ; has it been seen yet?
      BCC NoAirship
    :
    LDA airship_vis
    BEQ NoAirship

    ShowAirship:
      LDA #AIRSHIP
      BNE GotAirship
    NoAirship:
      LDA item_floater
      BEQ NoFloater
        LDA #FLOATER
    GotAirship:
      STA icon_buf+BUF_OFFSET,X
    NoFloater:
      INX




;; a column for spacing:
INX


; CheckCrown:
  LDA item_crown
  BEQ NoCrown
    LDA #CROWN
    STA icon_buf,X
    LDY #OBJID_ASTOS
    JSR CheckGameEventFlag
    BCC NoAstos
      LDA #FILLEDCH
      BNE CrownCheckbox
    NoAstos:
      LDA #EMPTYCH
    CrownCheckbox:
      STA icon_buf+BUF_OFFSET,X
  NoCrown:
    INX
  ; 27 bytes

; CheckCrystal
  LDA #CRYSTAL
  STA icon_id1
  LDY #OBJID_MATOYA
  LDA item_crystal
  JSR CheckTurnInItem
  ; 12 bytes


; CheckHerb:
   LDA #HERB
   STA icon_id1
   LDA #ELFDOC
   STA icon_id2
   LDY #OBJID_ELFDOC
   JSR CheckGameEventFlag
   ROL
   AND #1
   STA event_flag
   LDY #OBJID_ELFPRINCE
   LDA item_herb
   JSR CheckTwoPartTurnInItem
   ; 26 bytes

; CheckAdamant
  LDA #ADAMANT
  STA icon_id1
  LDY #OBJID_SMITH
  LDA item_adamant
  JSR CheckTurnInItem
  ; 12 bytes

; CheckTNT
  LDA #TNT
  STA icon_id1
  LDY #OBJID_NERRICK
  LDA item_tnt
  JSR CheckTurnInItem
  ; 12 bytes

; CheckRuby
  LDY #OBJID_TITAN
  JSR IsObjectVisible
  BCS TitanStillThere
    LDA #FILLEDCH
    BNE RubyCheckbox
  TitanStillThere:
    LDA item_ruby
    BEQ NoRuby
      LDA #EMPTYCH
  RubyCheckbox:
    STA icon_buf+BUF_OFFSET,X
    LDA #RUBY
    STA icon_buf,X
  NoRuby:
    INX
  
  ; 12 bytes

; CheckTail
; Remove Tail can be tri-stated and we shouldn't spoil that if "No Tri-state Spoilers" is on.
; If Fight Bahamut is on, the tail icon is replaced by a Bahamut icon.
; Fight Bahamut can also be tri-stated.
; The randomizer takes care of which icon to use, and populating the relevant lut's above to ensure no spoilers, if needed.
  LDA lut_ShowRemoved+1    ; if we're showing removed tail, we'll go straight to drawing the icon
  BNE TailOnly             ; and bypass the checkbox. 
    LDY #OBJID_BAHAMUT
    JSR CheckGameEventFlag
    BCC NoTailTurnIn
      LDA #FILLEDCH
      BNE TailCheckbox
    NoTailTurnIn:
      LDY lut_NPCReqs+3
      BEQ NoBahamutReq
        LDA items,Y
        BEQ NoTail
      NoBahamutReq:
        LDA #EMPTYCH
    TailCheckbox:
      STA icon_buf+BUF_OFFSET,X
    TailOnly:
      LDA #TAIL
      STA icon_buf,X
    NoTail:
      INX

; CheckBottle:
   LDA #BOTTLE
   STA icon_id1
   LDA #FAIRY
   STA icon_id2
   LDY #OBJID_FAIRY
   JSR IsObjectVisible
   ROL
   AND #1
   STA event_flag
   ; LDY #OBJID_FAIRY  -- Y still set to fairy
   LDA item_bottle
   JSR CheckTwoPartTurnInItem
   ; 24 bytes

; CheckSlab:
  LDA #SLAB
  STA icon_id1
  LDA #TRSLAB
  STA icon_id2
  LDY #OBJID_UNNE
  JSR CheckGameEventFlag
  ROL
  AND #1
  STA event_flag
  LDY #OBJID_LEFEIN
  LDA item_slab
  JSR CheckTwoPartTurnInItem
  ; 26 bytes


;;; inlining these takes fewer bytes
; CheckRod:
  LDA item_rod
  BEQ NoRod
    LDA #ROD
    STA icon_buf,X
    LDY #OBJID_RODPLATE
    JSR IsObjectVisible
    BCS RodPlateNotCleared
      LDA #FILLEDCH
      BNE RodCheckbox
    RodPlateNotCleared:
      LDA #EMPTYCH
    RodCheckbox:
      STA icon_buf+BUF_OFFSET,X
  NoRod:
    INX

; ; CheckLute:
;   LDA item_lute
;   BEQ NoLute
;     LDA #LUTE
;     STA icon_buf,X
;     LDY #OBJID_LUTEPLATE
;     JSR IsObjectVisible
;     BCS LutePlateNotCleared
;       LDA #FILLEDCH
;       BNE LuteCheckbox
;     LutePlateNotCleared:
;       LDA #EMPTYCH
;     LuteCheckbox:
;       STA icon_buf+BUF_OFFSET,X
;   NoLute:
;     INX
CheckOxyale:
  LDA item_oxyale
  BEQ NoOxyale
    LDA #OXYALE
    STA icon_buf,X
    LDY #OBJID_SUBENGINEER
    JSR IsObjectVisible
    BCS SubEngineerNotCleared
      LDA #FILLEDCH
      BNE OxyaleCheckbox
    SubEngineerNotCleared:
      LDA #EMPTYCH
    OxyaleCheckbox:
      STA icon_buf+BUF_OFFSET,X
    NoOxyale:
      INX


; column to separate passive items
INX

; CheckKey:
  LDA item_mystickey
  BEQ NoKey
    LDA #KEY
    STA icon_buf,X
  NoKey:

; CheckChime:
  LDA item_chime
  BEQ NoChime
    LDA #CHIME
    STA icon_buf+BUF_OFFSET,X
  NoChime:
    INX

; ; CheckOxyale:
;   LDA item_oxyale
;   BEQ NoOxyale
;     LDA #OXYALE
;     STA icon_buf,X
;   NoOxyale:
; CheckLute:
  LDA item_lute
  BEQ NoLute
    LDA #LUTE
    STA icon_buf,X
  NoLute:

; CheckCube:
  LDA item_cube
  BEQ NoCube
    LDA #CUBE
    STA icon_buf+BUF_OFFSET,X
  NoCube:
    INX

  ;; a column reserved for future speed shoes and repel icons (NOP if they aren't being included in game)
NOP  ;; INX

; column to separate NPCs
INX 

; Check Princess:
  LDA #SARA
  STA icon_id1

  LDY #OBJID_PRINCESS_2    
  JSR IsObjectVisible      
  ROL                      
  AND #1                   

  JSR CheckNPC   ;; Y still has Sara's OBJID
  ; (15 bytes)
  

; Check King:

  LDA #KING
  STA icon_id1
  LDY lut_NPCReqs
  BNE KingReq
    LDA #1
    BNE KingID
  KingReq:           
    JSR CheckGameEventFlag   
    ROL                      
    AND #1
  KingID:            
  LDY #OBJID_KING
  JSR CheckNPC
  ; (17 bytes)

; Check Bikke:
  LDA #BIKKE
  STA icon_id1
  LDY #OBJID_PIRATETERR_1 
  JSR IsObjectVisible  
  ROL                  
  AND #1   
  LDY #OBJID_BIKKE
  JSR CheckNPC
  ; (17 bytes)


; Check Canoe Sage:
  LDA #SAGE
  STA icon_id1
  LDY lut_NPCReqs+1
  BNE SageReq
    LDA #1
    BNE SageID
  SageReq:
    LDA items,Y
  SageID:           
  LDY #OBJID_CANOESAGE
  JSR CheckNPC
  ; (11 bytes)


; Check Sarda:
  LDA #SARDA
  STA icon_id1
  LDY lut_NPCReqs+2
  BNE SardaReq
    LDA #1
    BNE SardaID
  SardaReq:
    JSR CheckGameEventFlag
    ROL                      
    AND #1
  SardaID                                  
  LDY #OBJID_SARDA
  JSR CheckNPC
  ; (19 bytes)


; Check Robot:
  LDA #ROBOT
  STA icon_id1
  ;; requirement logic ;;;;;; 
  LDA #1                   ;; 
  ;; end requirement logic ;;
  LDY #OBJID_CUBEBOT
  JSR CheckNPC
  ; (11 bytes)

; Check ShopItem:
  LDA #SHOP
  STA icon_buf,X
  LDY #OBJID_SHOPITEM
  JSR CheckGameEventFlag
  BCC NoShopItem
    LDA #FILLEDCH
    BNE ShopItemCheckbox
  NoShopItem:
    LDA #EMPTYCH
  ShopItemCheckbox:
  STA icon_buf+BUF_OFFSET,X
  



  JMP DrawIconsToItemMenu



;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;
;; CheckTurnInItem
;;
;; Check item that is removed from
;; inventory after turn-in
;;
;; applies to these items:
;;   crystal
;;   adamant
;;   tnt
;;   
;;   Not ruby -- Titan's event flag is not updated in talk routines
;;  
;;  in:
;;             A: item flag
;;             X: pointer to location in icon_buf
;;             Y: npc #OBJID
;;      icon_id1: icon to draw
;; out:
;;             X: incremented icon_buf pointer
;;
;; setup: (this could be done more elegantly with a lut, but there aren't enough checks to worry about it)
;;   LDA #ITEMICON
;;   STA icon_id1
;;   LDY #OBJID
;;   LDA item flag
;;   JSR CheckTurnInItem
;;    (11 bytes per check)
;;
;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;; 
CheckTurnInItem:
  ; CMP #0                    ; not strictly needed since setup should load the item flag just before entering
  ; BEQ NoTurnInItem        
  ;   LDA #EMPTYCH 
  ;   BNE TurnInCheckbox
  ; NoTurnInItem:
  ;   JSR CheckGameEventFlag
  ;   BCC NoTurnIn
  ;     LDA #FILLEDCH
  ;   TurnInCheckbox:
  ;     STA icon_buf+BUF_OFFSET,X
  ;     LDA icon_id1
  ;     STA icon_buf,X
  ; NoTurnin:
  ;   INX
  ; RTS
  ; ; (25 bytes)
  STA tmp
  JSR CheckGameEventFlag
  BCC NoTurnIn
    LDA #FILLEDCH
    BNE TurnInCheckbox
  NoTurnIn:
    LDA tmp
    BEQ NoTurnInItem
      LDA #EMPTYCH
    TurnInCheckbox:
      STA icon_buf+BUF_OFFSET,X
      LDA icon_id1
      STA icon_buf,X
  NoTurnInItem:
    INX
  RTS


;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;
;; CheckTwoPartTurnInItem
;; Check item that is removed from
;; inventory after an event, and that
;; then requires talking to an NPC to
;; receive the item.
;;
;; applies to these:
;;    ITEM  ->     EVENT       ->    NPC
;;     herb -> talk to Elf Doc -> Elf Prince
;;     slab -> talk to Unne    -> Lefein Guy
;;   bottle -> open bottle     -> Fairy
;;
;;  in:
;;             A: item flag
;;             X: pointer to location in icon_buf
;;             Y: final npc #OBJID
;;   event_flag: talked to Elf Doc, Talked to Unne, Released Fairy
;;      icon_id1: icon to draw if item held
;;      icon_id2: icon to draw if event has taken place
;;      
;; out:
;;             X: incremented icon_buf pointer
;;
;; setup:
;;    LDA #ITEMICON1
;;    STA icon_id1
;;    LDA #ITEMICON2
;;    STA icon_id2
;;    LDY eventOBJID
;;    JSR CheckGameEventFlag  OR  IsObjectVisible
;;    ROL 
;;    AND #1
;;    STA event_flag
;;    LDY #OBJID
;;    LDA item flag
;;    JSR CheckTwoPartTurnInItem
;;      (26 bytes per check)
;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;
CheckTwoPartTurnInItem:
  STA tmp
  JSR CheckGameEventFlag
  BCC NoFinalTurnIn
    LDA icon_id2
    STA icon_buf,X
    LDA #FILLEDCH
    BNE TwoPartCheckbox
  NoFinalTurnin:
    LDA event_flag
    BEQ NoInitialEvent
      LDA icon_id2
      STA icon_buf,X
      BNE TwoPartEmptyCheckbox
    NoInitialEvent:
      LDA tmp
      BEQ NoTwoPartItem
        LDA icon_id1
        STA icon_buf,X
    TwoPartEmptyCheckbox:
      LDA #EMPTYCH
    TwoPartCheckbox:
      STA icon_buf+BUF_OFFSET,X
  NoTwoPartItem:
    INX
  RTS
  ; CMP #0
  ; BEQ NoTwoPartItem
  ;   LDA icon_id1
  ;   STA icon_buf,X
  ;   BNE NoFinalTurnIn
  ; NoTwoPartItem:
  ;   LDA event_flag 
  ;   BEQ NoTwoPartTurnIn
  ;     LDA icon_id2
  ;     STA icon_buf,X
  ;     JSR CheckGameEventFlag ; Y still set
  ;     BCC NoFinalTurnIn
  ;       LDA #FILLEDCH
  ;       BNE TwoPartCheckBox
  ;     NoFinalTurnIn:
  ;       LDA #EMPTYCH
  ;   TwoPartCheckBox:
  ;     STA icon_buf+BUF_OFFSET,X
  ; NoTwoPartTurnIn:
  ;   INX
  ; RTS
  ; (36 bytes)

;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;
;; CheckPassiveItem
;;
;; Check item that
;; opens a path without turn-in
;;
;; applies to these items:
;;   key
;;   oxyale
;;   chime
;;   cube
;;
;;  in:
;;      A: item flag
;;      X: pointer to icon_buf location
;;      Y: item icon
;; out:
;;      X: incremented icon_buf pointer
;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;
; CheckPassiveItem:
;   CMP #0
;   BEQ NoPassiveItem
;     TYA             ; get the item icon number
;     STA icon_buf,X
;   NoPassiveItem:
;     INX
;   RTS
  ; (10 bytes)


;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;
;; CheckNPC
;;
;; Checks a Key Item NPC
;;
;; Applies to:
;;   Sara
;;   King
;;   Bikke
;;   Canoe Sage
;;   Sarda
;;   Robot
;;
;;  in:
;;               A: requirement for NPC to give item
;;               X: pointer to icon_buf location
;;               Y: NPC id
;;        icon_id1: NPC icon
;; out:
;;               X: incremented icon_buf pointer
;;  
;; setup:
;;  
;;  LDA #ITEMICON
;;  STA icon_id1
;;  LDA  ;; logic to determine whether requirement met 
;;  LDY #OBJID
;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;
CheckNPC:
  CMP #0
  BEQ NoNPC
    LDA icon_id1
    STA icon_buf,X
    JSR CheckGameEventFlag
    BCC NoTalkNPC
      LDA #FILLEDCH
      BNE NPCCheckbox
    NoTalkNPC:
      LDA #EMPTYCH
    NPCCheckbox:
      STA icon_buf+BUF_OFFSET,x
  NoNPC:
    INX
  RTS
  ; (26 bytes)



;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;
;; Drawing Routines
;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;

DrawIconsToItemMenu:
  LDA #0
  STA $2001
  LDX #$00
  LDA lut_Columns
  STA tmp
  JSR DrawIcons
  INC dest_y
  JSR CoordToNTAddr
  LDX #BUF_OFFSET
  LDA lut_Columns
  STA tmp
  JSR DrawIcons
  LDA #SOURCE_BANK
  JMP SwapPRG   ;;; END ITEM MENU TRACKER
  ; 29 bytes
  

;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;
;; Draw Icons
;;
;; in: tmp   = number of icons to draw
;;       X   = index in icon_buf
;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;
DrawIcons: 
  LDA icon_buf,X
  LDY $2002         
  LDY ppu_dest+1
  STY $2006
  LDY ppu_dest
  STY $2006
  STA $2007
  INX
  INC ppu_dest
  DEC tmp
  BNE DrawIcons
  RTS
  ; 27 bytes

.ORG $A3F5
;;;;;; copied from bank $0E
CheckGameEventFlag:
    LDA game_flags,Y     ; Get the game flags using Y as index
    LSR A                ;   and shift the event flag into C
    LSR A
    RTS
  ; 6 bytes

IsObjectVisible:
    LDA game_flags,Y      ; get the game flags using object ID as index
    LSR A                 ; shift object visibility flag into C
    RTS                   ; and exit
  ; 5 bytes



.ORG $E225 ; bank $1F
  LDA #DEST_BANK ; $1B
  JSR SwapPRG
  JMP SetOWSpriteGameFlags
  NOP
  NOP
  NOP
  NOP
  NOP
  NOP
  ; 14 bytes

vehicle   = $42
OnFoot    = $E233
InAirship = $E261
InShipCanoe    = $E26A
;InCanoe   = $E275
ConvertOWToSprite = $E3DF

GMFLG_EVENT = $02


.ORG $A400 ; bank $1B
SetOWSpriteGameFlags:

  CheckBridge:
    LDA bridge_vis
    BEQ CheckCanal
      LDX bridge_x
      LDY bridge_y
      JSR ConvertOWToSprite
      BCS CheckCanal
        LDA game_flags+OBJID_REVEALBRIDGE
        ORA #GMFLG_EVENT
        STA game_flags+OBJID_REVEALBRIDGE
  CheckCanal:
    LDA canal_vis
    BNE CheckShip
      LDX canal_x
      LDY canal_y
      JSR ConvertOWToSprite
      BCS CheckShip
        LDA game_flags+OBJID_REVEALCANAL
        ORA #GMFLG_EVENT
        STA game_flags+OBJID_REVEALCANAL
  CheckShip:
    LDA ship_vis
    BEQ CheckAirship
      LDX ship_x
      LDY ship_y
      JSR ConvertOWToSprite
      BCS CheckAirship        ;; out of bounds
        LDA game_flags+OBJID_REVEALSHIP
        ORA #GMFLG_EVENT
        STA game_flags+OBJID_REVEALSHIP
  CheckAirship:
    LDA airship_vis
    BEQ CheckCanoe
      LDX airship_x
      LDY airship_y
      JSR ConvertOWToSprite
      BCS CheckCanoe
        LDA game_flags+OBJID_REVEALAIRSHIP
        ORA #GMFLG_EVENT
        STA game_flags+OBJID_REVEALAIRSHIP
  CheckCanoe:
    LDY vehicle
    CPY #$02    ; canoe
    BNE CheckInShip
      LDA game_flags+OBJID_REVEALCANOE
      ORA #GMFLG_EVENT
      STA game_flags+OBJID_REVEALCANOE
      JMP InShipCanoe
  CheckInShip:
    CPY #$04
    BNE CheckInAirship
      JMP InShipCanoe
  CheckInAirship
    CPY #$08
    BNE :+
      JMP InAirship
  :
    JMP OnFoot




;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;
;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;
;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;
;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;
;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;
;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;



  
