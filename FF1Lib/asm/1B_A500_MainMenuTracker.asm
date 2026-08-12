tmp             = $10
event_flag      = tmp+$0D
icon_id1        = tmp+$0E
icon_id2        = tmp+$0F
dest_x          = $3A
dest_y          = $3B
ppu_dest        = $54
display_buf     = $6E10
unsram          = $6000  ; $400 bytes
ship_vis        = unsram+$00
airship_vis     = unsram+$04
bridge_vis      = unsram+$08
canal_vis       = unsram+$0C
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
orb_fire        = items+$12
orb_water       = items+$13
orb_air         = items+$14
shards          = items+$15

game_flags      = unsram+$200




DrawMainItemBox    = $B8EF ; bank 0E

CoordToNTAddr      = $DCAB ; bank 1F
MenuCondStall      = $E12E ; bank 1F
SwapPRG            = $FE03 ; bnak 1F

SOURCE_BANK = $0E
DEST_BANK   = $1B

;; this requires a new lockpicking routine. See 1B_9300_LockpickDoors.asm for details. The main thing
;; is a new CheckLockpicking routine that produces a 1 (lockpicking achieved) or 0 (no) for a given character.
CheckLockpicking = $9350  ; bank $1B


DrawMainMenuGoldBox = $B86E ; bank $0E
DrawMainItemBox = $B8EF ; bank $0E
DrawMainMenuOptionBox = $B911 ; bank $0E
DrawMainMenuCharBoxBody = $B982 ; bank $0E

.ORG $B843 ; bank $0E
; JSR ClearNT
; JSR DrawOrbBox 
; JSR DrawMainMenuGoldBox
;;  switch order of character boxes and option box. We're going to break out of DrawMainMenuCharBoxBody to
;; draw icons for characters that have achieved lockpicking.
LDA #1                         ; then draw the boxes for each character
JSR DrawMainItemBox            ;  stats...starting with the first character
LDA #$00
JSR DrawMainMenuCharBoxBody

LDA #2                         ; second
JSR DrawMainItemBox
LDA #$40
JSR DrawMainMenuCharBoxBody

LDA #3                         ; third
JSR DrawMainItemBox
LDA #$80
JSR DrawMainMenuCharBoxBody

LDA #4                         ; fourth
JSR DrawMainItemBox
LDA #$C0
JSR DrawMainMenuCharBoxBody
JMP DrawMainMenuOptionBox

;; option box will now contain reminders for
;; Hintgivers in towns
;; Set RNG
;; Save on game over
;; if any of those are set.
;; at the end of this routine, it will also draw a GO mode icon in the orb box if we're in GO mode
;; sometimes GO mode will need to know about lockpicking, so we'll need to record lockpicking achieved
;; above

; JMP OptionBoxAndGoMode



; bank $0E near beginning of DrawMainMenuCharBoxBody

lockpicking_status = $6E04

; PHA
; TAX ;; leave the first two instructions intact
.ORG $B984 
BNE :+
  STA lockpicking_status; if character index == 0, reset lockpicking_status
:
  LDA #>(Lockpicking-1)
  PHA
  LDA #<(Lockpicking-1)
  PHA
  LDA #DEST_BANK
  JMP SwapPRG               ; 16 of 28 bytes overwritten at this point
  NOP
  NOP
  NOP
  NOP
  NOP
  NOP
  NOP
  NOP
  NOP
  NOP                       ; + 10 NOP's = 26 of 28 bytes overwritten at this point
  LDA tmp+$E  ;; Lockpicking subroutine returns here, and we load the value it stored
  ; BNE @DrawMP  ; main routine continues
  ;   JMP @NoMP

; we'll do the first 28 bytes after the first two instructions
; in the lockpicking routine


LockpickingReturnAddress = $B99E
str_buf = $0300
text_ptr = $3E
ch_maxmp = $6328
LOCKPICKING_ICON = $44
OBJID_REVEALUNLOCKEDDOOR = $FA
GMFLG_EVENT = $02


.ORG $A500 ; bank $1B
; written by the randomizer
lut_ShowTristateLockpicking:
.BYTE 1

.ORG $A501
Lockpicking:
  ; CheckLockpicking is also on bank $1B -- we're using the same bank for this reason
  JSR CheckLockpicking ; Y now has lockpicking achieved for this character
                         ; lockpicking_status also updated
  CPY #0
  BEQ NoLockpicking
    LDA lut_ShowTristateLockpicking
    BNE Draw
      LDA game_flags+OBJID_REVEALUNLOCKEDDOOR
      AND #GMFLG_EVENT
      BEQ NoLockpicking
  Draw:
    STX tmp+$E
    JSR CoordToNTAddr ; get ppu address for the string start location of this box
    LDA ppu_dest
    CLC
    ADC #$64 ; we're going to draw our lockpicking tile to a location 3 rows down and 4 columns to the right;
             ; this is 32*3 + 4 = 100 = $64 
    STA ppu_dest
    LDA #LOCKPICKING_ICON
    LDY $2002  ; reset ppu toggle
    LDY ppu_dest+1
    STY $2006
    LDY ppu_dest
    STY $2006
    STA $2007
    LDX tmp+$E
  NoLockpicking:
    ; Now we finish the stuff we overwrote in the main routine
    LDA #>str_buf     ; the strings we will be drawing here will be from a RAM buffer
    STA text_ptr+1    ;  so set the high byte of the string pointer
                    ; X still has character index
    LDA ch_maxmp,X   ; check all of this character's Max MP
    ORA ch_maxmp+1,X ;  if any level of spells is nonzero, we'll need to draw the MP
    ORA ch_maxmp+2,X ;  for this character.  Otherwise, we won't need to.
    ORA ch_maxmp+3,X ;  this is checked easily by just ORing all the max MP bytes
    ORA ch_maxmp+4,X
    ORA ch_maxmp+5,X
    ORA ch_maxmp+6,X
    ORA ch_maxmp+7,X
    STA tmp+$E ; put it in a random tmp location so we can pull it back when we return to the main routine
    LDA #>(LockpickingReturnAddress-1)
    PHA
    LDA #<(LockpickingReturnAddress-1)
    PHA
    LDA #SOURCE_BANK
    JMP SwapPRG  ; return
  



  








OptionBoxAndGoMode = $A580

.ORG $B911 ; bank 0E
DrawMainMenuOptionBox:
LDA #>(OptionBoxAndGoMode-1)
PHA
LDA #<(OptionBoxAndGoMode-1)
PHA
LDA #DEST_BANK ; 1B
JMP SwapPRG
NOP


DrawBox         = $E063 ; bank 1F
box_x           = $38
box_y           = $39
box_wd          = $3C
box_ht          = $3D
cur_bank        = $57

GOMODE_ICON     = $45
BLACKORB_ICON   = $46
HINT1_ICON      = $47
HINT2_ICON      = $48
SRNG1_ICON      = $49
SRNG2_ICON      = $4A
SOGO1_ICON      = $4B
SOGO2_ICON      = $4C
BLANK           = $FF

OBJID_BLACKORB  = $CA

CheckGameEventFlag = $A3F5
IsObjectVisible = $A3FB


.ORG $A560 ; bank 1B
; written by the randomizer. These are found at $BABA (!) in bank $0E, but we need them here in order to draw the box.
; X Y Width Height
; if there are three reminders to print, we need X = $01 and Width = $0A
lut_OptionBoxDims:
.BYTE $02,$0F,$08,$0E

; written by the randomizer.
; Values:
;   0. total reminders to draw
;   1. draw hintgivers?
;   2. draw set rng?
;   3. draw save on game over?
;   4. number of tiles to draw (2 tiles for 1 reminder, 6 for 2, and 8 for 3)
lut_ReminderSettings:
.BYTE $03,$01,$01,$01,$08

; written by the randomizer
; Values:
;   00. Check Go Mode?
;   01. Nametable address for icon (different for shard hunt)
;   02. Shard hunt?                (inferable, but easier to use a byte for this)
;   03. Number of orbs/shards required
;   04. Earth orb required?           0: no, 1: yes
;   05. Fire orb required?            0: no, 1: yes
;   06. Water orb required?           0: no, 1: yes
;   07. Air orb required?             0: no, 1: yes
;   08. Talk to black orb necessary?  0: no, 1: yes, 2: yes, but black orb doesn't spoil
;   09. Lockpicking available?
;   0A. Spoil lockpicking?
;   0B. Unlocked ToFR?
;   0C. Spoil unlocked ToFR?

lut_GoModeSettings:
     ;  00  01  02  03  04  05  06  07  08  09  0A  0B  0C
.BYTE  $01,$63,$00,$04,$01,$01,$01,$01,$00,$00,$01,$01,$01

.ORG $A580
OptionBoxAndGoMode:
;; first, draw the option box. We don't have access to the helper routines in this bank, so we do it manually
LDA lut_OptionBoxDims
STA box_x
LDA lut_OptionBoxDims+1
STA box_y
LDA lut_OptionBoxDims+2
STA box_wd
LDA lut_OptionBoxDims+3
STA box_ht
LDA #DEST_BANK
STA cur_bank
JSR DrawBox

DEC dest_y              ; we want to draw in the row above where strings normally begin
JSR CoordToNTAddr       ; get the nametable address (in ppu_dest)
INC dest_y              ; while we're here, we'll prepare dest_y and dest_x for the rest
INC dest_y              ; of the option box contents
LDA lut_OptionBoxDims   ; if we expanded the box width, we need to move the x coord as well
CMP #$02
BEQ :+
  INC dest_x
:
LDA lut_ReminderSettings 
BNE :+
  JMP CheckGoMode 
:
  LDX #$00
  LDA lut_ReminderSettings+1    ; hintgivers?
  BEQ SRNG
    LDA #HINT1_ICON
    STA display_buf,X
    INX
    LDA #HINT2_ICON
    STA display_buf,X
    INX
    LDA #BLANK
    STA display_buf,X
    INX
    LDA lut_ReminderSettings    ; how many reminders?
    AND #$01                    ; if 1 or 3, we only need one blank space
    BNE SRNG                    ; if 2 reminders, we need two spaces.
      LDA #BLANK
      STA display_buf,X
      INX
SRNG:
  LDA lut_ReminderSettings+2    ; set RNG?
  BEQ SOGO
    LDA #SRNG1_ICON
    STA display_buf,X
    INX
    LDA #SRNG2_ICON
    STA display_buf,X
    INX
    LDA #BLANK
    STA display_buf,X
    INX
    LDA lut_ReminderSettings    ; how many reminders? This could be the first one or the last one, 
                                ; so adding spaces to the buffer might be superfluous. Shrug emotiocn
    AND #$01                    ; if 1 or 3, we only need one blank space
    BNE SOGO                    ; if 2 reminders, we need two spaces.
      LDA #BLANK
      STA display_buf,X
      INX
SOGO:
  LDA lut_ReminderSettings+3    ; Save on game over?
  BEQ DrawReminders
    LDA #SOGO1_ICON
    STA display_buf,X
    INX
    LDA #SOGO2_ICON
    STA display_buf,X
DrawReminders:
  LDA lut_ReminderSettings+4    ; number of tiles to draw
  STA tmp
  LDX #$00
  : ; loop
    LDA display_buf,X
    LDY $2002                     ; reset PPU toggle
    LDY ppu_dest+1
    STY $2006
    LDY ppu_dest
    STY $2006
    STA $2007
    INC ppu_dest
    INX
    DEC tmp
    BNE :-
  JMP CheckGoMode

; put a JMP here because DrawBlackOrbOrGoMode is too far to branch to for much of this
DrawBlackOrbOrGoModeJMP:
  JMP DrawBlackOrbOrGoMode

CheckGoMode:
  LDA lut_GoModeSettings      ; are we checking go mode?
  BNE :+                      ; if so, jump ahead
    JMP DrawOptionMenu        ; if not, just go on to draw the option menu and exit
  :
    LDA #BLANK                ; init black orb / go mode icon with blank tile. 
    STA display_buf           ; will also be used as flag to check whether we've talked to black orb much further down
    LDY #OBJID_BLACKORB       ; load black orb objid here saves some bytes further down
    LDX lut_GoModeSettings+8  ; do we need to talk to black orb?
    BEQ CheckLute             ; if not, skip ahead
      JSR CheckGameEventFlag  ; Have we talked to black orb? (Y has black orb objid)
      BCC CheckLute           ; if we haven't, we might still be in go mode, so go check other stuff
        LDA #BLACKORB_ICON    ; but if we have, we'll put the black orb icon in the display buffer.
        STA display_buf       ; this might be overwritten by the GoMode icon, or if still blank, it indicates we haven't talked to black orb

CheckLute:
  LDA item_lute               ; do we have LUTE?
  BEQ DrawBlackOrbOrGoModeJMP ; if not, jump ahead

;;;;;;;;;;;;;
CheckOrbsOrShards:
  ; we could make custom asm for every combination to save bytes,
  ; but it's easier and cleaner to update to put settings in the lut above
  ; we could also write a loop, but doing it inline also gives us more future
  ; flexibility
  LDA lut_GoModeSettings+2    ; shard hunt?
  BNE ShardHunt
  Orbs:
    JSR IsObjectVisible     ; Is black orb visible? (Y still has black orb objid)
    BCS EarthOrb            ; if it is visible, go on to the rest of the orb checks
      JMP CheckKey          ; otherwise, we have already opened the portal, and can check Key requirements
    EarthOrb:
      LDA orb_earth
      LDX lut_GoModeSettings+4  ; Earth Orb Required?
      BEQ EarthOrbNotRequired
        CMP #0
        BEQ DrawBlackOrbOrGoModeJMP
    EarthOrbNotRequired:
      STA tmp
    FireOrb:
      LDA orb_fire
      LDX lut_GoModeSettings+5  ; Fire Orb Required?
      BEQ FireOrbNotRequired
        CMP #0
        BEQ DrawBlackOrbOrGoModeJMP
    FireOrbNotRequired:
      CLC
      ADC tmp
      STA tmp
    WaterOrb:
      LDA orb_water
      LDX lut_GoModeSettings+6  ; Water Orb Required?
      BEQ WaterOrbNotRequired
        CMP #0
        BEQ DrawBlackOrbOrGoModeJMP
    WaterOrbNotRequired:
      CLC
      ADC tmp
      STA tmp
    AirOrb:
      LDA orb_air
      LDX lut_GoModeSettings+7  ; Air Orb Required?
      BEQ AirOrbNotRequired
        CMP #0
        BEQ DrawBlackOrbOrGoModeJMP
    AirOrbNotRequired:
      CLC
      ADC tmp                   ; A now has the count of orbs lit
      CMP lut_GoModeSettings+3  ; compare with required number of orbs
      BCC DrawBlackOrbOrGoModeJMP        ; if we didn't meet the goal, exit
        ; code reaches here if we have lute and have met the orb requirements
        CMP #4                  ; if we have 4 orbs, then we can be sure we've made the Orb requirements,
        BEQ CheckKey            ; and we can go on to check the Key requirement.
          ; otherwise:
          LDX lut_GoModeSettings+8  ; do we need to talk to the black orb?
          BEQ CheckKey              ; if not, go on to check the Key requirement.
            ; if we do:
            CPX #2                  ; does the black orb give us information?
            BEQ DrawBlackOrbOrGoMode   ; if not, we can't know we're in go mode
              ; LDY #OBJID_BLACKORB       ; here we could check the black orb again, but since we've done that above, we
              ; JSR CheckGameEventFlag    ; can use the tile currently in display_buf as a proxy flag
              ; BCC DrawBlackOrbOrGoMode
              ; BCS CheckKey
              LDA display_buf
              CMP #BLANK                  ; if display_buf is blank, we haven't yet talked to black orb
              BNE CheckKey                ; if it's not, go on to CheckKey
                JMP DrawOptionMenu        ; if it is, there's nothing to draw, so we can just go draw option menu and exit
  ShardHunt:
    LDA shards
    CMP lut_GoModeSettings+3  ; goal
    BCC DrawBlackOrbOrGoMode

  CheckKey:
    LDA item_mystickey    ; if we have key, we can show go mode!
    BNE DrawGoMode        ; if not, check lockpicking
      LDA lut_GoModeSettings+9  ; is lockpicking on?
      BEQ UnlockedToFR          ; if not, check for unlocked ToFR
        LDA lockpicking_status  ; have we achieved lockpicking? (set above; we won't reach this if lockpicking isn't on)
        BEQ UnlockedToFR        ; if not, check for unlocked ToFR
          ; code reaches here if we have achieved lockpicking, and are factually in go mode.
          LDA lut_GoModeSettings+10 ; can we spoil lockpicking?
          BNE DrawGoMode            ; if so, we can show go mode!
            LDY #OBJID_REVEALUNLOCKEDDOOR ; this flag is set if we have already unlocked a locked door
            JSR CheckGameEventFlag        
            BCS DrawGoMode                ; if we have, we can show go mode for sure! Otherwise, we still need to check for unlocked ToFR
    UnlockedToFR:
      ; code reaches here if we have met lute and orb/shard requirements, but we either don't have the key, haven't achieved lockpicking
      ; or we have achieved lockpicking but we don't want to spoil it.
      LDA lut_GoModeSettings+11   ; is ToFR unlocked?
      BEQ DrawBlackOrbOrGoMode          ; if not, move on
        LDA lut_GoModeSettings+12 ; otherwise, we are in go mode, but we need to check if we can spoil that.
        BEQ DrawBlackOrbOrGoMode        ; if not, we have to move on. Otherwise, we can show go mode!

  DrawGoMode:
    LDA #GOMODE_ICON
    STA display_buf
  DrawBlackOrbOrGoMode:
    LDA display_buf   ; could be #BLANK, #BLACKORB_ICON, or #GOMODE_ICON
    LDY $2002         ; reset ppu
    LDY #$20            ; we'll draw in the first part of the nametable
    STY $2006         
    LDY lut_GoModeSettings+1  ; get the NT address to write to
    STY $2006
    STA $2007                 ; draw, and move on to finish drawing the option menu

DrawOptionMenu:
; we're going JMP to the DrawMenuString routine, which normally takes the string ID in A as input.
; since we have to swap banks, we can't populate A as input.
; The first two instructions in DrawMenuString are:
; ASL
; TAX
; so if we load X with the string ID * 2, we can jump two bytes into that routine.
;  DrawMenuString2  = $B93A ; bank 0E, skipping first two bytes of that routine
;; DrawMenuString is now at $8600 in order to move the menu texts to bank $12.

DrawMenuString2 = $8602 ; bank 0E, skipping first two bytes of that routine
  LDX #$04    ; string ID is $02, so we LDX 2*2
  LDA #>(DrawMenuString2-1)
  PHA
  LDA #<(DrawMenuString2-1)
  PHA
  LDA #SOURCE_BANK ; 0E
  JMP SwapPRG
; dest_x and dest_y are already set.






