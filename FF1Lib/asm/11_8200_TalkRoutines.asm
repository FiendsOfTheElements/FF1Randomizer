;;  TalkRoutines - 2020-12-24
;;
;;  This create 3 new talk routines and modify others
;;   - Talk_GiveItemOnFlag : Allow a NPC to give any item if the required flag is enabled
;;   - Talk_GiveItemOnItem : Allow a NPC to give any item if the required item is owned by the player
;;   - Talk_TradeItems :  Allow a NPC to give any item if the required item is owned; the required item is then taken
;;
;;  Some routines are deleted as they can be reproduced by other routines (Talk_Garland > talk_fight)

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


btlformation    = $6A
dlgflg_reentermap = $56  ; flag to indicate the map needs re-entering due to dialogue (Bahamut/class change)
talkarray = $70

dialogue_1 = talkarray
dialogue_2 = talkarray+1
dialogue_3 = talkarray+2
item_id = talkarray+3
required_id = talkarray+4
battle_id = talkarray+5
object_id = talkarray+6

dlg_itemid = $61
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
GiveItem = $9F10

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

mapobj          = $6F00   ; $100 bytes -- page
mapobj_id      = mapobj + $00  ; rearranging these is ill advised

lut_MapObjTalkData = $95D5
lut_MapObjTalkData_moved = $BA00
lut_MapObjTalkJumpTbl = $90D3
lut_MapObjTalkJumpTbl_moved = $8000


 .ORG $901B
;;  TalkToObject
;;    Called to talk to a object on the map (townsperson, etc).
;;
;;  IN:        X = index to map object (to index 'mapobj' buffer)
;;        dlgsfx, dlgflg_reentermap = assumed to be zero
;;
;;  OUT:       A = ID of dialogue text to print onscreen
TalkToObject:
    LDA mapobj_id, X    ; get the ID of the object they're talking to
    STA talkarray+6           ; back the ID up for later

    LDY #0              ; mulitply the ID by 6 (6 bytes of talk data per object)
    STY tmp+5
    ASL
    ROL tmp+5
    CLC
    ADC talkarray+6
    BCC NoCarry
      INC tmp+5
NoCarry:
    ASL
    ROL tmp+5

    ADC #<lut_MapObjTalkData_moved   ; and add the pointer to the start of the talk data table to that
    STA tmp+4
    LDA #>lut_MapObjTalkData_moved
    ADC tmp+5
    STA tmp+5                        ; (tmp+4) now points to the talk data for this object

    STX tmp+2                        ; backup X as some routines use it
    LDA #$00 
    TAX
    TAY
CopyLoop:                            ; copy all 6 bytes 
    LDA (tmp+4), Y
    STA talkarray, X
    INY
    INX
    CPX #$06
    BNE CopyLoop
    
    LDX tmp+2
    LDA talkarray+6           ; get the object ID (previously backed up)
    ASL                       ; *2 (two bytes per pointer) (high bit shifted into C)
    TAY                       ; throw in Y for indexing
    BCC LowerHalf             ; if C clear, we read from bottom half of table, otherwise, top half

    LDA lut_MapObjTalkJumpTbl_moved+$100, Y  ; copy the desired pointer from the talk jump table
    STA tmp+6
    LDA lut_MapObjTalkJumpTbl_moved+$101, Y
    STA tmp+7
    JMP (tmp+6)                        ; and jump to it, then exit

LowerHalf:
  LDA lut_MapObjTalkJumpTbl_moved, Y       ; same, but with low half of table
  STA tmp+6
  LDA lut_MapObjTalkJumpTbl_moved+1, Y
  STA tmp+7
  JMP (tmp+6)

 .ORG $8200
;; All updated talk routines
;; We use NOPx2 to divide them as they are seperatly inserted by the randomizer

;; none (Blank sprite -- object ID=0)
Talk_None:
  RTS
 
  NOP
  NOP
;; Generic eventless object 
;;  [1] always
Talk_norm:
  LDA talkarray+1             ; uneventful object -- just print [1]
  RTS

  NOP
  NOP
;; Generic condition check based on object visibility 
;;  [1] if object ID [0] is hidden
;;  [2] if it's visible
Talk_ifvis:
  LDY talkarray                ; check to see if object [0] is visible
  JSR IsObjectVisible
  BCS IsVis               ; if it is, print [2]
    LDA talkarray+1             ; otherwise, print [1]
    RTS
IsVis:     
  LDA talkarray+2
  RTS

  NOP
  NOP
;; Generic condition based on item index
;;  [1] if party contains at least one of item index [0]
;;  [2] otherwise (none of that item)
Talk_ifitem:
  LDA talkarray              ; use [0] as an item index
  ADC #$20                   ; check also for SHIP, CANOE, BRIDGE, CANAL and AIRSHIP
  TAY
  LDA unsram, Y
  BEQ DontHave8              ; if they do have it
    LDA talkarray+1          ; print [1]
    RTS
DontHave8:
  LDA talkarray+2            ; otherwise, print [2]
  RTS

  NOP
  NOP
;; Invisible Lady  (infamous invisible lady in Coneria Castle) 
;;  [1] if princess not rescued and you don't have the Lute
;;  [2] if princess rescued or you do have the Lute
Talk_Invis:
  LDY #OBJID_PRINCESS_2
  JSR IsObjectVisible      ; see if the princess has been rescued (rescued princess object visible)
  BCS IsVis9               ; if she's not rescued...
    LDA item_lute
    BNE IsVis9             ; ... and if you don't have the lute (redundant)
      LDA talkarray+1      ; print [1]
      RTS
IsVis9:
  LDA talkarray+2          ; otherwise print [2]
  RTS

  NOP
  NOP
;; Generic condition based on game event flag 
;;  [1] if game event flag ID [0] is clear
;;  [2] if it's set
Talk_ifevent:
  LDY talkarray                ; use [0] as an event flag index
  JSR CheckGameEventFlag       ;  see if that event flag has been set
  BCS EventA                   ; if not...
    LDA talkarray+1            ; ... print [1]
    RTS
EventA:
  LDA talkarray+2              ; otherwise print [2]
  RTS

  NOP
  NOP
;; Some guard in Coneria town 
;;  [1] if princess has been saved, but bridge isn't built yet
;;  [2] if princess still kidnapped or bridge is built
Talk_GoBridge:
  LDY #OBJID_PRINCESS_2        ; check to see if princess has been rescued
  JSR IsObjectVisible
  BCC NotSaved                 ; if she has...
    LDA bridge_vis             ; see if bridge has been built
    BNE NotSaved               ; if not... (princess saved, but bridge not built yet...)
      LDA talkarray+1          ;  ... print [1]
      RTS
NotSaved:        
  LDA talkarray+2              ; otherwise print [2]
  RTS

  NOP
  NOP
;; Conditional Check for 4 Orbs 
;;  [1] if all 4 orbs lit
;;  [2] otherwise
Talk_4Orb:
  LDA orb_fire                 ; see if all orbs have been lit
  AND orb_water
  AND orb_air
  AND orb_earth
  BEQ NotAllLit2               ; if they have...
    LDA talkarray+1            ; print [1]
    RTS
NotAllLit2:      
  LDA talkarray+2              ; otherwise (not all of them lit)
  RTS                          ;  print [2]

  NOP
  NOP
;; Conditional check for key+TNT  (some dwarf) 
;;  [1] if have key, but not TNT
;;  [2] if no key, or have TNT
Talk_ifkeytnt:
  LDA item_mystickey           ; check to see if the party has the key
  BEQ DefaultB                 ; if they do...
    LDA item_tnt               ; check to see if they have the TNT
    BNE DefaultB               ; if they don't  (key but no TNT)
      LDA talkarray+1          ;  print [1]
      RTS
DefaultB:        
  LDA talkarray+2              ; otherwise, print [2]
  RTS

  NOP
  NOP
;; Conditional check for Earth Orb and Vampire (people in Melmond) [$9559 :: 0x39569]
;;  [1] if Vampire is dead and Earth Orb not lit
;;  [2] if Vampire lives, or Earth Orb has been lit
Talk_ifearthvamp:
  LDY #OBJID_VAMPIRE          ; see if the vampire has been killed yet
  JSR IsObjectVisible
  BCS DefaultC                ; if not...
    LDA orb_earth             ; check to see if player revived earth orb
    BNE DefaultC              ; if not... (Vampire killed, Earth Orb not lit yet)
      LDA talkarray+1         ;  print [1]
      RTS
DefaultC:
  LDA talkarray+2             ; otherwise print [2]
  RTS

  NOP
  NOP
;; Conditional check for earth/fire orbs
;;  [1] if Earth Orb not lit, or Fire Orb Lit
;;  [2] if Earth Orb lit, and Fire Obj not lit
Talk_ifearthfire:
  LDA orb_earth               ; see if the Earth Orb has been recovered
  BEQ DefaultD                ; if it has...
    LDA orb_fire              ; check Fire Orb
    BNE DefaultD              ; if it's still dark (Earth lit, but not Fire)
      LDA talkarray+1         ; print [1]
      RTS
DefaultD:        
  LDA talkarray+2             ; otherwise, print [2]
  RTS

  NOP
  NOP
;; Replacable Object (first 2 of the 3 ToFR Garlands)
;;  [1] always --- hide THIS object whose ID is [0], and show object ID [3]
Talk_Replace:
  LDY talkarray+6             ; get this object ID [6]
  JSR HideThisMapObject       ; hide that object (this object)
  LDY talkarray+3
  JSR ShowMapObject           ; show object ID [3]
  LDA talkarray+1             ; and print [1]
  RTS

  NOP
  NOP
;; Mysterious Sage (in the CoO -- disappears after you talk to him) 
;;  [1] always --- hide THIS object whose ID is [0]
Talk_CoOGuy:
  LDY talkarray+6
  JSR HideThisMapObject       ; hide object ID [0] (this object)
  LDA talkarray+1             ; and print [1]
  RTS

  NOP
  NOP
;; Generic fight 
;;  [1] always --- hide THIS object whose ID is [0], and initiate battle ID [3]
Talk_fight:
  LDA dialogue_2              ; Print dialogue box right away
  JSR InTalkDialogueBox
  LDA battle_id               ; Then initiate the fight
  JSR InTalkBattle
  LDY object_id               ; After the fight,
  JSR SetGameEventFlag        ;  set object's flag
  JSR HideMapObject           ;  and hide it
  JSR InTalkReenterMap        ; Then reenter the map
  JMP SkipDialogueBox         ; Skip showing a second dialogue box
  RTS

  NOP
  NOP
;; The Black Orb 
;;  [1] if all 4 orbs are lit
;;  [2] otherwise
Talk_BlackOrb:
  LDA orb_fire            ; see if all orbs have been lit
  AND orb_water
  AND orb_air
  AND orb_earth
  BEQ NotAllLit           ; if all of them are lit...
    LDY #OBJID_BLACKORB   ; hide the black orb object (this object)
    JSR HideThisMapObject
    INC dlgsfx            ; play TC sound effect  (not fanfare)
    INC dlgsfx
    LDA talkarray+1       ; and print [1]
    RTS
NotAllLit:
  LDA talkarray+2         ; otherwise, (not all orbs lit), print [2]
  RTS

  NOP
  NOP
;; Kidnapped Princess (in the ToF)
Talk_Princess1:
  LDY talkarray+6              ; Load this object (the princess)
  JSR SetGameEventFlag         ; Set its flag
  JSR HideThisMapObject        ; Hide it
  LDY #OBJID_PRINCESS_2        ; Load saved princess
  JSR ShowMapObject            ; Show
  LDA #NORMTELE_SAVEDPRINCESS  ; Trigger the teleport back to Coneria Castle
  JSR TalkNormTeleport
  LDA talkarray+1              ; Print [1]
  RTS

  NOP
  NOP
;; Submarine Engineer (in Onrac, blocking enterance to Sea Shrine) 
;;  [1] if you don't have the Oxyale
;;  [2] if you do
Talk_SubEng:
  LDA item_oxyale             ; see if the player has the Oxyale
  BNE HaveOxyale              ; if they don't...
    LDA talkarray+1           ; ...print [1]
    RTS
HaveOxyale:      
  LDY talkarray+6             ; otherwise (they do)
  JSR HideMapObject           ; hide the sub engineer object (this object)
  LDA talkarray+2             ; and print [2]
  RTS

  NOP
  NOP
;; Titan
;;  [1] if you don't have the Ruby
;;  [2] if you do
Talk_Titan:
  LDA item_ruby              ; does the player have the ruby?
  BNE HaveRuby               ; if not...
    LDA talkarray+1          ; ... simply print [1]
    RTS
HaveRuby:      
  DEC item_ruby              ; if they do have it, take it away
  LDY talkarray+6            ; hide/remove Titan (this object)
  JSR HideMapObject
  LDA talkarray+2            ; print [2]
  INC dlgsfx                 ; and play fanfare
  RTS

  NOP
  NOP
;; Bikke the Pirate 
;;  [0] after you have the item
;;  [1] if haven't fought him yet
;;  [2] giving the item
;;  [3] item to give
Talk_Bikke:
  LDY #OBJID_PIRATETERR_1   ; Check if the hiding townsfolk are visible
  JSR IsObjectVisible
  BCS AlreadyFought         ; if not...   
    LDA dialogue_2          ; Show dialogue
    JSR InTalkDialogueBox
    LDA battle_id
    JSR InTalkBattle        ; Initiate the fight
    LDY #OBJID_PIRATETERR_1
    JSR ShowMapObject       ; Then show townsfolk
    LDY #OBJID_PIRATETERR_2 
    JSR ShowMapObject
    LDY #OBJID_PIRATETERR_3
    JSR ShowMapObject
    JSR InTalkReenterMap    ; Reenter the map
    JMP SkipDialogueBox     ; Skip showing second dialogue
AlreadyFought:
   LDY object_id            ; Check Bikke's flag
   JSR CheckGameEventFlag 
   BCS Default1             ; If it's not set...
     LDA item_id            ; Load item to give
     BEQ Default1           ; If there's one to give
       CLC
       JSR GiveItem         ; Give it
       BCS End1             ; Can't hold
       LDY object_id        ; Set Bikke's flag
       JSR SetGameEventFlag   
       LDA dialogue_3       
       RTS
Default1:
  LDA dialogue_1
End1:
  RTS


  NOP
  NOP
;; Nerrick (dwarf who opens the Canal) 
;;  [0] if you don't have the TNT
;;  [1] unused
;;  [2] giving item
;;  [3] item to give
Talk_Nerrick:
  LDA item_tnt             ; check to see if the player has TNT
  BEQ Default5             ; if not...
    LDA item_id            ; load item to give [3]
    BEQ Default5           ; if there's one
      CLC
      JSR GiveItem         ; Give it
      BCS End5             ; Can't hold
      DEC item_tnt         ; otherwise, remove the TNT from the party
      LDY object_id         
      JSR SetGameEventFlag ; set flag 
      JSR HideMapObject    ; hide Nerrick
      LDA dialogue_3
      RTS
Default5:
  LDA dialogue_1
End5:
  RTS

  NOP
  NOP
;; Bahamut 
;;  [1] if haven't been promoted, and don't have the Tail
;;  [2] if haven't been promoted, and DO have the Tail
;;  [3] once promoted
Talk_Bahamut:
  LDY #OBJID_BAHAMUT      ; Check Bahamut's Event flag (see if he promoted you yet)
  JSR CheckGameEventFlag
  BCC CheckTail           ; if he has...
    LDA talkarray+3       ; ... print [3]
    RTS
CheckTail:
  LDA item_tail           ; he hasn't promoted you yet... so check to see if you have the tail
  BNE ClassChange         ; if you don't...
    LDA talkarray+1       ; ... print [1]
    RTS
ClassChange:              ; otherwise (have tail), do the class change!
  DEC item_tail           ; remove the tail from inventory
  JSR SetGameEventFlag    ; set Bahamut's event flag
  JSR DoClassChange       ; do class change
  INC dlgsfx              ; play fanfare
  LDA talkarray+2         ; and print [2]
  RTS

  NOP
  NOP
;; Elf Doctor and Unne
;;  [0] if you don't have the required item
;;  [1] if you have given it
;;  [2] if you do have the item
Talk_ElfDocUnne:
  LDY object_id               ; Check flag
  JSR CheckGameEventFlag
  BCS ItemGiven               ; If it's not set...
    LDX required_id
    LDA items, X           
    BEQ Default2              ; If we have it...
      DEC items, X            ; Take it away
      JSR SetGameEventFlag    ; Set flag
      INC dlgsfx              ; Play jingle
      LDA talkarray+2         ; Print [2]
      RTS
ItemGiven:
  LDA talkarray+1             ; Print [1]
  RTS
Default2:
  LDA talkarray               ; Print [0]
  RTS

  NOP
  NOP
;; Give Item on Flag
;; NPC give an item if the right flag is set
;;  [0] If it's not set
;;  [1] item given
;;  [2] giving the item
;;  [3] Item to give
Talk_GiveItemOnFlag:
  LDY object_id
  JSR CheckGameEventFlag
  BCS ItemGiven3                ; If it's not set...
    LDY required_id
    BEQ NoFlag                  ; If there's no flag, go ahead with giving the item
      JSR CheckGameEventFlag    ; If there is, check if it's set
      BCC Default3              ; If not, end 
NoFlag:
      LDA item_id               ; Load item to give from [3]
      BEQ Default3
        CLC
        JSR GiveItem            ; Give it
        BCS End3                ; Can't hold
        LDY object_id
        JSR SetGameEventFlag 
        LDA dialogue_3
        RTS
ItemGiven3:
  LDA dialogue_2
  RTS
Default3:
  LDA dialogue_1
End3:
  RTS

  NOP
  NOP
;; Trade quest item, trade an item for another one 
;;  [0] if you don't have the item
;;  [1] item given
;;  [2] giving item
;;  [3] item to give
Talk_TradeItems:
  LDY object_id               
  JSR CheckGameEventFlag      ; Check this object's event flag 
  BCS ItemGiven6              ; if it is set, 
  LDX required_id
  LDA items, X 
  BEQ Default6                
    LDA item_id
    BEQ Default6              ; if there's an item to give 
      CLC
      JSR GiveItem            ; give it 
      BCS End6                ; if we don't already have it (Can't hold text) 
      LDX required_id
      DEC items, X            ; Take the item
      LDY object_id
      JSR SetGameEventFlag     
      LDA dialogue_3
      RTS                     
ItemGiven6:
  LDA dialogue_2
  RTS 
Default6:
  LDA dialogue_1
End6:
  RTS                            

  NOP
  NOP
;; Give Item on Item, don't take required item away
;;  [0] if you don't have the item
;;  [1] item given
;;  [2] giving item
;;  [3] item to give
Talk_GiveItemOnItem:
  LDY object_id
  JSR CheckGameEventFlag      ; Check this object's event flag 
  BCS ItemGivenG              ; if it is set, 
  LDX required_id
  BEQ NoItem                  ; If there's no required item, go ahead with giving the item
  LDA items, X 
  BEQ DefaultG                
NoItem:   
    LDA item_id
    BEQ DefaultG              ; if there's an item to give 
      CLC
      JSR GiveItem            ; give it 
      BCS EndG                ; if we don't already have it (Can't hold text) 
      LDY object_id
      JSR SetGameEventFlag     
      LDA dialogue_3
      RTS                     
ItemGivenG:
  LDA dialogue_2
  RTS 
DefaultG:
  LDA dialogue_1
EndG:
  RTS                        

  NOP
  NOP
;; Astos
Talk_Astos:
  LDX required_id                 ; Check required item
  BEQ NoReq_As
    LDA items, X
    BEQ Default_As                ; Do we have it?
NoReq_As:    
  LDA item_id
  STA dlg_itemid
  JSR CheckCanTake                ; Check if we can take it
  BEQ End_As
    INC dlgsfx                    ; Play jingle  
    LDA dialogue_3      
    JSR InTalkDialogueBox         ; Show dialogue box
    LDA battle_id
    JSR InTalkBattle              ; Then do the fight
    LDY object_id                 ; Load this object
    JSR SetGameEventFlag          ; Set its flag
    JSR HideMapObject             ; Hide it
    LDA dlg_itemid
    CLC
    JSR GiveItem                  ; Actually give the item
    JSR InTalkReenterMap          ; Reenter the map
    JMP SkipDialogueBox           ; Skip second dialogue box
Default_As:
  LDA dialogue_1
End_As:
  RTS

  NOP
  NOP
;; Talk for NPC guillotine, add extra safety so it doesn't break anything
;; [1] always
Talk_kill:
  LDY #$00
  JSR HideThisMapObject ; hide object ID (this object)
  LDA dialogue_2
  RTS

  NOP
  NOP

;; Vanilla Chaos fight so we can trigger the ending
Talk_chaos:   
  LDA battle_id           ; Load battle ID
  JSR TalkBattle          ; Set up fight tile
  LDA dialogue_2          ; Show dialogue box
  RTS

  NOP
  NOP  

 .ORG $9600
 
BattleTransition = $D8CD
LoadBattleCHRPal = $E900
LoadBattleCHRPal_R = $E8FF
EnterBattle_L = $F200
EnterBattle_R = $F1FF
ReenterStandardMap = $CF3A
DialogueBox = $CA03
StandardMapLoop = $C8B6
SwapPRG_L = $FE03
SwapPRG_skip = $FE07  
DrawSMSprites = $E40F
FindEmptyWeaponSlot = $DD34
FindEmptyArmorSlot = $DD46

; Trigger a battle inside the talk routine
InTalkBattle:
  STA btlformation         ; store battle formation
  JSR BattleTransition     ; Do transition
  LDA #$00
  STA $2001
  STA $4015
  LDY #$02
  JSR JumpThenSwapPRG      ; LoadBattleCHRPal
  LDY #$01
  JSR JumpThenSwapPRG      ; EnterBattle
  RTS

; Reenter the map inside the talk routine
;  MUST be called after InTalkBattle
InTalkReenterMap:
  LDY #$03
  JSR JumpThenSwapPRG      ; ReenterStandardMap
  JMP DrawSMSprites        ; and draw all sprites 

; Check inventory if there's space for an item
CheckCanTake:
 CMP #$6C               ; We can always take money
 BCS CanTake             
 CMP #$44               ; Otherwise, check space for armor or weapon
 BCS Armor
  CMP #$1C
  BCC CanTake
  JSR FindEmptyWeaponSlot
  BCS TooFull
  JMP CanTake
Armor:
 JSR FindEmptyArmorSlot 
 BCS TooFull
CanTake:                ; Clear carry if we can take, otherwise, carry was set
 CLC
TooFull:
 LDA #$F1 ; Can't hold
 RTS

; Show a dialogue box inside the talk routine
InTalkDialogueBox:
  LDY #$00
  JSR JumpThenSwapPRG
  RTS

; Skip the dialogue box shown once a talk routine is done
;  should be called with InTalkDialogueBox to avoid showing
;  several dialogue boxes one after the other
SkipDialogueBox:
    PLA                    ; Clear the last 4 bytes of stack
    PLA
    PLA
    PLA
    JMP StandardMapLoop    ; Then go back to the map

; Jump to Y routine in bank $1F, then swap back to this bank ($11) when done
JumpThenSwapPRG:
  STA tmp
  LDA #$11
  PHA
  LDA #>SwapPRG_skip       ; Put RTS address on stack
  PHA                      ;  because we'll be switching bank
  LDA #<SwapPRG_skip-1
  PHA
  LDA lut_TargetJump_Hi, Y
  PHA
  LDA lut_TargetJump_Lo, Y
  PHA
  LDA tmp
  RTS 

lut_TargetJump_Hi:
 .BYTE >DialogueBox, >EnterBattle_R, >LoadBattleCHRPal_R, >ReenterStandardMap
lut_TargetJump_Lo:
 .BYTE <DialogueBox-1, <EnterBattle_R, <LoadBattleCHRPal_R, <ReenterStandardMap-1


 .ORG $0000
 
; Alternative routine when Shuffle Astos select Bahamut 
Talk_Astos_Bahamut:
  LDA item_tail		; he hasn't promoted you yet... so check to see if you have the tail
  BNE ClassChange_As5	; if you don't...
    LDA dialogue_2	; Show dialogue_2
    RTS                  
ClassChange_As5:	; otherwise (have tail), do the class change!
  INC dlgsfx		; Play jingle
  LDA dialogue_3	; Show dialogue_3
  JSR InTalkDialogueBox
  LDA battle_id		; Load battle ID
  JSR InTalkBattle	; Trigger the battle
  LDY object_id		; Load this object
  JSR SetGameEventFlag	; Set its flag
  JSR HideMapObject	; Hide it  
  JSR DoClassChange	; do class change
  JSR InTalkReenterMap  ; Reenter the map
  JMP SkipDialogueBox	; Skip extra dialogue box
  RTS

; Alternative Astos when NPC Items, Fetch Quest Items or Shuffle Astos are enabled
Talk_Astos_Fetch:
  LDX required_id
  BEQ NoReq_As6 
    LDA items, X		; Check required item
    BEQ Default_As6		; Do we have it?
NoReq_As6:    
  LDA item_id			; If yes, load item
  STA dlg_itemid 		; And make sure we have the inventory space for  it
  JSR CheckCanTake
  BCS End_As6			; If inventory is full, skip
    LDA dialogue_3		; Otherwise show dialogue_3 
    JSR InTalkDialogueBox
    LDA battle_id		; Load battle ID
    JSR InTalkBattle		; Tigger the battle
    LDY object_id		; Load this object
    JSR SetGameEventFlag	; Set its flag
    JSR HideMapObject		; Hide it
    LDA dlg_itemid		; Reload the item
    CLC
    JSR GiveItem		; And actualy give it
    JSR InTalkReenterMap	; Reenter the map
    LDA #$F0			; Load dialogue for "Received..."
    RTS
Default_As6:
  LDA dialogue_1		; Show default dialog
End_As6:
  RTS