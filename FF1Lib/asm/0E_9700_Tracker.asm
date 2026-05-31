tmp             = $10
dest_x          = $3A
dest_y          = $3B
ppu_dest        = $54
str_buf         = $0300 ; $39 bytes at least -- buffer must not cross page
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



CheckGameEventFlag = $9079 ; bank 0E, in: OBJID in Y, out: carry flag set 
IsObjectVisible    = $9091 ; bank 0E
DrawMainItemBox    = $B8EF ; bank 0E
DrawItemTitleBox   = $B91D ; bank 0E
CoordToNTAddr      = $DCAB ; bank 1F
MenuCondStall      = $E12E ; bank 1F


;;;;;;;;;;;;;;;;;;;;;;
;; Tracker Icon Tiles
;;;;;;;;;;;;;;;;;;;;;;
BRIDGE  = $01
CANAL   = $02
SHIP    = $03
CANOE   = $04
MARK    = $04
AIRSHIP = $05
FLOATER = $06
SIGIL   = $06
CROWN   = $07
CRYSTAL = $08
HERB    = $09
DOC     = $0A
ADAMANT = $0B
TNT     = $0C
RUBY    = $0D
TAIL    = $0E
BOTTLE  = $0F
FAIRY   = $10
SLAB    = $11
TRSLAB  = $12
ROD     = $13
LUTE    = $14
KEY     = $15
OXYALE  = $16
CHIME   = $17
CUBE    = $18
SARAH   = $19
KING    = $1A
BIKKE   = $1B
SAGE    = $1C
SARDA   = $1D
ROBOT   = $1E
SHOES   = $1F
REPEL   = $20

EMPTYCH = $74
FILLEDCH= $75
BLANK   = $FF

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
OBJID_SUBENGINEER  = $10   ; Submarine Engineer (blocking Sea Shrine in Onrac)
OBJID_PRINCESS_2   = $12   ; rescued princess (in Coneria Castle)
OBJID_FAIRY        = $13   ; fairy that appears from the bottle
OBJID_TITAN        = $14   ; Titan in Titan's Tunnel
OBJID_RODPLATE     = $16   ; plate that is removed with the Rod
OBJID_LUTEPLATE    = $17   ; plate that is removed with the Lute

OBJID_SKYWAR_FIRST = $3A                   ; start of the 5 sky warriors
OBJID_SKYWAR_LAST  = OBJID_SKYWAR_FIRST+4  ; last of the 5 sky warriors

OBJID_PIRATETERR_1 = $3F   ; townspeople that were terrorized by the
OBJID_PIRATETERR_2 = $40   ;   pirates... they don't become visible until after
OBJID_PIRATETERR_3 = $41   ;   you beat Bikke and claim the ship


.ORG $B12D
LDA #07  ;; load the box ID (dimensions must be changed from $08 to $1C at $BAC0 in bank $0E)
JSR DrawItemTitleBox

.ORG $B91D
DrawItemTitleBox:
  JSR DrawMainItemBox ;; dest_x & dest_y now set
  DEC dest_y ;; we need one row above the usual menu start location
  JMP InitItemTracker
  NOP
  NOP
  ; 10 bytes -- reusing this space saves 5 bytes in main routine

.ORG $9700
InitItemTracker:
    
  JSR CoordToNTAddr ;; convert to ppu address and store in ppu_dest (2 bytes)
  LDA #BLANK
  LDX #55
  initloop:         ; fill 56 str_buf values with blank menu tile
    STA str_buf,X
    DEX
    BPL initloop

  ; 13 bytes

  ;; begin processing items immediately:

ProcessItems:
  ;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;
  ; begin tracking items
  ;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;
  ;
  ; We could draw these as strings, but it's much easier to just use the string buffer.
  ;
  LDX #$00        ; start at str_buf[0]
; CheckBridge:
  LDA bridge_vis
  BEQ no_bridge
    LDA #BRIDGE
    STA str_buf,X
  no_bridge:  
    INX
  ; 10 bytes


; CheckCanal:
  LDA canal_vis
  BNE no_canal
    LDA #CANAL
    STA str_buf,X
  no_canal:
    INX
  ; 10 bytes

; CheckShip -- should work with AirBoat
  LDA ship_vis
  BEQ no_ship
    LDA #SHIP
    STA str_buf,X
  no_ship:
    INX
  ; 10 bytes

; CheckCanoe:
  LDA has_canoe
  BEQ no_canoe
    LDA #CANOE
    STA str_buf,X
  no_canoe:    
    INX
  ; 10 bytes

; CheckFloaterAirship -- should work with AirBoat
  LDA airship_vis
  BEQ no_airship
    LDA #AIRSHIP
    BNE got_airship
  no_airship:
    LDA item_floater
    BEQ no_floater
      LDA #FLOATER
  got_airship:
    STA str_buf,X
  no_floater:
    INX
  RTS
  ; 19 bytes

; CheckCrown:
  LDA item_crown
  BEQ no_crown
    LDA #CROWN
    STA str_buf,X
    LDY #OBJID_ASTOS
    JSR CheckGameEventFlag
    BCC no_astos
      LDA #FILLEDCH
      BNE astos_checkbox
    no_astos:
      LDA #EMPTYCH
    astos_checkbox:
      STA str_buf+28,X
  no_crown:
    INX
  RTS
  ; 27 bytes

; CheckCrystal
  LDY #OBJID_MATOYA
  JSR CheckGameEventFlag
  LDA #0
  ADC item_crystal
  BEQ no_crystal
    LDA #CRYSTAL
    STA str_buf,X
    CMP #2
    BNE no_matoya
      LDA #FILLEDCH
      BNE crystal_checkbox
    no_matoya:
      LDA #EMPTYCH
    crystal_checkbox:
      STA str_buf+28,X
  no_crystal:
    INX
  RTS
  ; 31 bytes

; CheckHerb
  LDY #OBJID_ELFPRINCE
  JSR CheckGameEventFlag
  LDA #0
  ADC item_herb
  BEQ no_herb
    LDA #HERB
    STA str_buf,X
    CMP #2
    BNE no_prince
      LDA #FILLEDCH
      BNE herb_checkbox
    no_prince:
      LDA #EMPTYCH
      BNE herb_checkbox
  no_herb:
    LDY #OBJID_ELFDOC
    JSR CheckGameEventFlag
    BCC no_doc
      LDA #DOC
      STA str_buf,X
      LDA #EMPTYCH
  herb_checkbox
    STA str_buf+28,X
  no_doc:
    INX
  RTS
  ; 47 bytes


DrawIconsToItemMenu:
  LDX #$00
  LDA #28
  STA tmp
  JSR MenuCondStall ;; might not be necessary since PPU is off?
  JSR DrawIcons
  INC dest_y
  JSR CoordToNTAddr
  LDA #28
  STA tmp
  JSR DrawIcons
  ; 24 bytes
  

;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;
;; Draw Icons
;;
;; in: tmp = number of icons to draw
;;       X = index in str_buf
;; out:  X = index in str_buf we ended at -- can be reused
;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;
DrawIcons: 
    LDA str_buf,X
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

  exit:
  RTS
  ; 27 bytes


    




