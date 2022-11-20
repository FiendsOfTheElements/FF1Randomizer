;
; AirBoat
; Last modifications: 2022-11-20
;

tmp		= $10
ow_scroll_x     = $27  ; X scroll of OW in tiles
ow_scroll_y     = $28  ; Y scroll in tiles

vehicle         = $42 ;1=walking, 2=canoe, 4=ship, 8=airship
tileprop        = $44 ; 2 bytes
vehicle_next    = $46 ; vehicle we're walking onto

music_track     = $4B

cur_bank        = $57
ret_bank        = $58

tileset_data    = $0400  ; $400 bytes -- must be on page bound
tileset_prop    = tileset_data  ; 256 bytes, 2 bytes per tile

unsram          = $6000  ; $400 bytes
ship_vis        = unsram + $00
ship_x          = unsram + $01
ship_y          = unsram + $02
airship_vis     = unsram + $04
airship_x       = unsram + $05
airship_y       = unsram + $06
items           = unsram + $20
item_floater    = items + $0B

Board_Fail = $C5E4
CallMusicPlay = $C689
LandAirship = $C6B8
AnimateAirshipTakeoff = $E1A8
DrawOWSprites = $E225
ShowAirship = $E258
SwapPRG_L = $FE03

AIRBOATBANK = $1B

; 1F Modification

 .ORG $C10C 
;EnterOverworldLoop: 
; ...
  JSR NewLandingDrawOWSprites

 .ORG $C25A 
;ProcessOWInput:
; ...
  LDA #AIRBOATBANK
  STA cur_bank 
  JSR SwapPRG_L
  JMP OWButtonA

 .ORG $C609 
;BoardShip: 12b
; ...
  NOP
  NOP
  NOP
  NOP
  NOP
  NOP
  NOP
  LDA #AIRBOATBANK
  STA cur_bank 
  JSR SwapPRG_L
  JSR BoardCheck
  BEQ Board_Fail

 .ORG $C632
;DockShip:
; ...
  LDA #AIRBOATBANK
  STA cur_bank 
  JSR SwapPRG_L
  JMP DockShip

 .ORG $C6D7 
;LandAirship:
; ...
  NOP
  NOP
  JSR NewLandingCheck

 .ORG $E248 
;DrawOWSprites:
; ...
  JMP ShowAirship
NewLandingDrawOWSprites:    ; 13 bytes (10 bytes used)
  LDA #AIRBOATBANK
  STA cur_bank 
  JSR SwapPRG_L
  JMP DrawOWSprites

 .ORG $E373
;DrawOWObj_Ship:
; ...
  JSR CheckShip

 .ORG $E1F6
;AirshipTransitionFrame:
; ...
  JSR CallMusicPlay

;New Routines

 .ORG $A000

; Check the highest bit; draw ship if 0, don't draw if 1
CheckShip:       ; called on foot, canoe, in airship
  LDA ship_vis
  AND #$01       ; do we have ship?
  BNE CS_HaveShip
    RTS
CS_HaveShip:
  LDA ship_vis
  AND #$80
  EOR #$80
  RTS

; What to do if button A is pressed on OW
OWButtonA:
  LDA vehicle
  CMP #$08
  BEQ OWA_InAirship
    JMP FlyAirship
OWA_InAirship:
  JMP LandAirship

; Extra check when landing
NewLandingCheck:
  LDA tileset_prop, X
  AND #$08
  BNE NotAirshipTile
    LDA #$01
    STA airship_vis
    LDA #$00
    RTS
NotAirshipTile:
  LDA tileset_prop, X
  AND #$04
  BEQ IsShipTile
    RTS
IsShipTile:
  LDA ship_vis
  AND #$01
  BNE HaveShip
    LDA #$01
    RTS
HaveShip:
  LDA ow_scroll_x     ; otherwise (legal land)
  CLC
  ADC #$07            ; get X coord again
  STA ship_x        ; park the airship there

  LDA ow_scroll_y
  CLC
  ADC #$07
  STA ship_y       ; same with Y coord

  LDA #$04
  STA vehicle_next    ; change vehicle to make the player on foot
  STA vehicle

  LDA #$45
  STA music_track     ; start music track $45 (ship theme)
    
  LDA #$00
  STA airship_vis
   
  PLA
  PLA
  RTS                 ; and exit

; new FlyAirship routine
FlyAirship:
  LDA airship_vis      ; see if airship is visible
  BNE HaveAirship
    LDA vehicle
    CMP #$04
    BEQ ShipRoutine
      RTS
ShipRoutine:
  LDA item_floater
  BNE LiftOff
Exit:
    RTS
HaveAirship:
  LDA ow_scroll_x      ; get map X scroll
  CLC
  ADC #7               ; +7 to get player's coord
  CMP airship_x        ; see if it matches the airship's X coord
  BNE Exit            ; if not.. exit

  LDA ow_scroll_y      ; do same check with Y coord
  CLC
  ADC #7
  CMP airship_y
  BNE Exit            ; if no match, exit
LiftOff:
  LDA #$08
  STA vehicle_next     ; set current and next vehicle to airship
  STA vehicle

  LDA #$46
  STA music_track      ; change music track to $46 (airship music)

  LDA #$00
  STA airship_vis
  LDA ship_vis
  ORA #$80
  STA ship_vis

  JMP AnimateAirshipTakeoff    ; do the takeoff animation, then exit
  
DockShip:
    LDA ship_vis
    AND #$7F
    STA ship_vis

    LDA ow_scroll_x     ; get X scroll
    CLC
    ADC #$07            ; add 7 to get player coord
    STA ship_x          ; put ship there

    LDA ow_scroll_y     ; same with Y coord
    CLC
    ADC #$07
    STA ship_y

    CLC                 ; CLC to indicate success

    LDA #$30
    STA $400C           ; silence noise (kills the "waves" sound)

    LDA #$44
    STA music_track     ; switch to music track $44 (overworld theme)

    RTS                 ; exit

BoardCheck:
  JSR CheckShip
  BEQ BoardFail
    LDA ship_x          ; is ship docked at current X/Y
    CMP tmp+2           ; coords?
    BNE BoardFail
    LDA ship_y
    CMP tmp+3
    BNE BoardFail      ; if not... fail
      LDA #$01
      RTS
BoardFail:
  LDA #$00
  RTS
  