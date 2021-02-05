;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;
;;
;;	GameOverUpdateSaveData	[$8FF5 ;; 0x6D005] - 2020-12-13
;;
;;	Saves game progress and sets the player's overworld map
;;	position to the last inn / overworld location they saved
;;	at.  This is mostly a copy of code elsewhere, except it
;;	does not update the player's map coordinates/vehicle.
;;	This also does the closing animation of fading out the
;;	Combat boxes since we have to replace a JSR in new
;;	GameOver.
;;
;;

 .ORG $8FF5
 
btlformation    = $6A	
ch_stats	= $6100
ch_weapons	= ch_stats + $18
ch_armor	= ch_weapons + $04
unsram 		= $6000
gold		= unsram + $1C	
game_flags      = unsram + $0200 
shop_curprice = $030E
tmp = $10
	
 JSR $8BE3             	; do a routine which we gutted in GameOver
 
GameOverUpdateSaveData:
  LDX #$00
RestorePlayerLoop:	; first, we restore the player's party to max hp/mp and restore status
  LDA $6100,X           ; we don't restore Nones, so skip them
  CMP #$FF
  BEQ SkipNone
    LDA $610C,X         ; restore hp
    STA $610A,X
    LDA $610D,X
    STA $610B,X
    LDA $6328,X         ; restore mp
    STA $6320,X
    LDA $6329,X
    STA $6321,X
    LDA $632A,X
    STA $6322,X
    LDA $632B,X
    STA $6323,X
    LDA $632C,X
    STA $6324,X
    LDA $632D,X
    STA $6325,X
    LDA $632E,X
    STA $6326,X
    LDA $632F,X
    STA $6327,X
    LDA #$00            ; reset status
    STA $6101,X
SkipNone:
  TXA
  CLC
  ADC #$40
  TAX
  BNE RestorePlayerLoop

; This section is different for DW mode, see below
; here we check each transport mode to see where to spawn
CheckSpawn:
	LDA $6004              ; Check if we have the arship
	BNE AirshipSpawn
	LDA $6000
	BEQ StartCopying
ShipSpawn:
	LDA $6001
	CMP $6401
	BNE SpawnAtShip
	LDA $6002
	CMP $6402
	BEQ StartCopying
SpawnAtShip:
	LDA $6001
	SEC
	SBC #$07
	STA $6010
	LDA $6002
	SEC
	SBC #$07
	STA $6011
	LDA #$04
	STA $6014
	BNE StartCopying
AirshipSpawn:
	LDA $6005
	SEC
	SBC #$07
	STA $6010
	LDA $6006
	SEC
	SBC #$07
	STA $6011
	LDA #$01
	STA $6014
	LDA $6000
	BEQ StartCopying
	LDA #$98
	STA $6001
	LDA #$A9
	STA $6002

; Start saving
StartCopying:
	LDX #$00
CopyLoop:
    LDA $6000,X
    STA $6400,X
	LDA $6100,X
    STA $6500,X
	LDA $6200,X
	STA $6600,X
    LDA $6300,X
    STA $6700,X
    INX
    BNE CopyLoop

    LDA #$55
    STA $64FE
	LDA #$AA
    STA $64FF

    LDA #$00
    STA $64FD
	LDX #$00
    CLC

ChecksumLoop:
    ADC $6400,X
    ADC $6500,X
	ADC $6600,X
    ADC $6700,X
    INX
    BNE ChecksumLoop
    EOR #$FF
    STA $64FD

    JMP $801D
 

; CheckSpawn section for DWmode
;  here we just move transport modes if we have them
;  then halves the gold
CheckSpawn:
	LDA $6004              ; Check if we have the arship
	BEQ NoAirship
	LDA #$99
	STA $6005
	LDA #$A5
	STA $6006
NoAirship:
	LDA $6000
	BEQ NoShip
	LDA #$98
	STA $6001
	LDA #$A9
	STA $6002
NoShip:
	LDA #$92
	STA $6010
	LDA #$9E
	STA $6011	
    LSR $601E
    ROR $601D
    ROR $601C
