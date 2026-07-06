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



;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;
;; New DrawShardBox routine
;;
;; This has to work well with Deep Dungeon, which also makes changes to the orb box.
;; In the Deep Dungeon asm, it calls the DrawOrbBox routine before making changes.
;; Therefore, this needs to be called from within the DrawOrbBox routine itself.
;; Randomizer needs to handle the dimensions of the box and the location of Deep Dungeon's
;; floor
DrawShardDisplay = $ACE3


;; redirect from bank $0E to bank $1B
.ORG $B8A5 ; bank $0E, in DrawOrbBox
LDA #>(DrawShardDisplay-1)
PHA
LDA #<(DrawShardDisplay-1)
PHA
LDA #DEST_BANK
JMP SwapPRG
NOP
NOP
NOP
NOP
NOP
NOP
NOP

  ; (18 bytes)

SHARD = $63
.ORG $AEC0 ; bank $1B

;written by randomizer
; the number of shards needed, and the two number tiles associated with them

lut_ShardGoal:
.BYTE 24 $82 $84

.ORG $AEC3
;; stuff from $0E overwritten by the redirect above
LDA $2002    ; reset PPU toggle
LDA #>$23C9
STA $2006
LDA #<$23C9
STA $2006    ; attribute byte at $23C9
LDA tmp+7    ; load computed attribute byte
STA $2007    ;   and draw it

;;; at this point, the orb box and orbs have been drawn, as well as the attributes bytes for lit orbs.

;; now we assemble the display tiles to the buffer. Could use game's string routines, but those are overkill for this little
;; bit. Printing 2-digit numbers is in bank $0E, but that's a minefield and it's easier to just do it here.
LDX #0
LDA #SHARD ; the shard tile
STA display_buf,X
INX
LDY #0
LDA shards ;; load the current number of collected shards
CMP #10
BCS TwoDigits
  LDA #$FF ; blank tile
  STA display_buf,X
  INX
  LDA shards
  JMP OnesPlace
TwoDigits:
    INY
    SEC
    SBC #10
    CMP #10
    BCS TwoDigits
  PHA
  TYA
  ORA #$80
  STA display_buf,X
  INX
  PLA
OnesPlace:
  ORA #$80
  STA display_buf,X
  INX
  ;;; now the slash, and the tiles stored in the lut above
  LDA #$7A ; slash
  STA display_buf,X
  INX
  LDA lut_ShardGoal+1
  STA display_buf,X
  INX
  LDA lut_ShardGoal+2
  STA display_buf,X
  ;; draw
  LDA #$03
  STA dest_x
  LDA #$02
  STA dest_y
  JSR CoordToNTAddr
  LDX #0
  LDA #6 ; 6 tiles to draw
  STA tmp
DrawShardTiles:
  LDA display_buf,X
  LDY $2002 ; reset PPU toggle
  LDY ppu_dest+1
  STY $2006
  LDY ppu_dest
  STY $2006
  STA $2007
  INX
  INC ppu_dest
  DEC tmp
  BNE DrawShardTiles
; draw attribute bytes
LDA #%00111111 ; lower right metatile has palette 0; all others have palette 3
LDY $2002 ; reset PPU toggle
LDY #>$23C0 ; enter address for this attribute byte
STY $2006
LDY #<$23C0
STY $2006
STA $2007
;; check if we've met goal
LDA shards
CMP lut_ShardGoal
BCC GoalNotMet   ; if goal not met, we don't need to do anything. Otherwise... 
  LDA #%11001111 ; lower left metatile has palette 0; all others have palette 3
  LDY $2002 ; reset PPU toggle
  LDY #>$23C1 ; enter address for this attribute byte
  STY $2006
  LDY #<$23C1
  STY $2006
  STA $2007
GoalNotMet:
;; swap to bank $0E and return from JSR DrawOrbBox
LDA #SOURCE_BANK
JMP SwapPRG












