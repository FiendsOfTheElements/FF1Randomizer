;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;
;;
;;	GameOverUpdateSaveData	[$8FF5 ;; 0x6D005]
;;
;;	Saves game progress and sets the player's overworld map
;;	position to the last inn / overworld location they saved
;;	at.  This is mostly a copy of code elsewhere, except it
;;	does not update the player's map coordinates/vehicle.
;;	This also does the closing animation of fading out the
;;	Combat boxes since we have to replace a JSR in new
;;	GameOver.
;;
;;	Astos: The Talk code for Astos is changed
;;  to store some information so to prevent cheating is item.

btlformation	= $6A	
ch_stats		= $6100
ch_weapons		= ch_stats + $18
ch_armor		= ch_weapons + $04
unsram 			= $6000
gold			= unsram + $1C	
game_flags      = unsram + $0200 
shop_curprice	= $030E
tmp				= $10

	JSR $8BE3	; do a routine which we gutted in GameOver

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

; Check from which battle we died so we can reset that battle
ResetGarland:
	LDA btlformation    ; Check the last battle formation
	CMP #$7F            ;  and check if it was Garland
	BNE ResetBikke      ; If it's not, go to next formation
	JSR ShowThis        ; If it is, make it visible again
	JMP CheckSpawn      ; Skip, no need to check for the other battle formations
ResetBikke:
	CMP #$7E
	BNE ResetAstos
	JSR ShowThis        ; Reset Bikke's flag
	LDA $623F           ; Hide townsfolk 1, 2, 3
	AND #$FE
	STA $623F
	LDA $6240
	AND #$FE
	STA $6240
	LDA $6241
	AND #$FE
	STA $6241
	JMP CheckSpawn
ResetAstos:	            ; Astos is a bit more complicated
	CMP #$7D
	BNE ResetVampire
	JSR ShowThis
	JSR AstosComplex    ; Do the complex routine to retake Astos' item
	JMP CheckSpawn
ResetVampire:           ; All the rest is like Garland
	CMP #$7C           
	BNE ResetLich
	JSR ShowThis
	JMP CheckSpawn
ResetLich:
	CMP #$7A
	BNE ResetKary
	JSR ShowThis
	JMP CheckSpawn	
ResetKary:
	CMP #$79
	BNE ResetKraken
	JSR ShowThis
	JMP CheckSpawn
ResetKraken:
	CMP #$78
	BNE ResetTiamat
	JSR ShowThis
	JMP CheckSpawn
ResetTiamat:
	CMP #$77
	BNE ResetChaos
	JSR ShowThis
	JMP CheckSpawn
ResetChaos:
	CMP #$7B           
	BNE CheckSpawn
	LDA $6218
	ORA #$01
	STA $6218
	LDA $6219
	AND #$FE
	STA $6219
	LDA $621A
	AND #$FE
	STA $621A
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

ShowThis:
	LDX tmp+6            ; Get obj ID
	LDA game_flags, X    ; Get flag
	ORA #$01             ; Make visible
	AND #$FD             ; Reset flag
	STA game_flags, X    ; Store back flag
	RTS

AstosComplex:	         ; Most of the code here comes from the new GiveReward routine
	LDA $13              ; Check what he gave
	CLC
	ADC #$20             ; Add $20 for item offset in RAM
 	CMP #$3C             ; if it's an item
	BCS NoItem
Item:                      
	TAX
	CMP #$0C             ; Check if it's the Canal, because it's not like other item
	BNE NoCanal
	INC unsram, X        ; Hide Canal
	RTS
NoCanal:
	DEC unsram, X        ; If it's not the Canal, just decrease the quantity by 1
	RTS
NoItem:
	LDA $13              ; Reload the item ID
	LDY $67              ; Talk_Astos stored Y after GiveItem for weapon/armor slot and the amount of gold in shop_curprice
	CMP #$6C             ; Is it gold?
	BCC NoGold
	  LDA gold           ; Same routine if you're buying an item in a shop
          SEC            ; Load party's gold and subtract the price
          SBC shop_curprice  
          STA gold

          LDA gold+1
          SBC shop_curprice+1
          STA gold+1

          LDA gold+2
          SBC #0             
          STA gold+2
          RTS
NoGold:
	CMP #$44                    ; Check if it's a weapon or armor
	BCS Armor
        LDX lut_WeaponSlots, Y  ; We compare Y with a lut that will give the right equipement slot where
	LDA #$00                    ;  Astos's item was put
	STA ch_stats, X             ; Zero out item from inventory
	RTS       
Armor:
	LDX lut_ArmorSlots, Y
	LDA #$00           
	STA ch_stats, X
	RTS

lut_WeaponSlots:
  .BYTE <ch_weapons+$00, <ch_weapons+$01, <ch_weapons+$02, <ch_weapons+$03
  .BYTE <ch_weapons+$40, <ch_weapons+$41, <ch_weapons+$42, <ch_weapons+$43
  .BYTE <ch_weapons+$80, <ch_weapons+$81, <ch_weapons+$82, <ch_weapons+$83
  .BYTE <ch_weapons+$C0, <ch_weapons+$C1, <ch_weapons+$C2, <ch_weapons+$C3

lut_ArmorSlots:
  .BYTE <ch_armor+$00, <ch_armor+$01, <ch_armor+$02, <ch_armor+$03
  .BYTE <ch_armor+$40, <ch_armor+$41, <ch_armor+$42, <ch_armor+$43
  .BYTE <ch_armor+$80, <ch_armor+$81, <ch_armor+$82, <ch_armor+$83
  .BYTE <ch_armor+$C0, <ch_armor+$C1, <ch_armor+$C2, <ch_armor+$C3	

;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;
;;
;;	NewAstosRoutine	[$9F45 ;; 0x39F55]
;;
;;	This is a new routine for Astos, where
;;  we load some data in memory from his item.
;;  This is necessary for death-saving to
;;	not break the game by letting you die to Astos
;;	and keep the item.  We use some of the space
;;	that was cleared out in bank 0E's character
;;	creation routines to accomplish this.

item_crown			= $6022
GiveItem			= $DD93
HideMapObject		= $9273
OBJID_ASTOS			= $07
BTL_ASTOS			= $7D
TalkBattle			= $90C5
SetGameEventFlag	= $907F

New_Talk_Astos:
	LDA $6022
	BEQ Default
    LDY #$40
    JSR $9091
    BCS AlreadyFought
      JSR $90A4
      LDA #$7D
      JSR $90C5
      LDA $12
      RTS
AlreadyFought:
	  CLC
      LDA $13
      BEQ Default
      JSR $DD93
      BCS EndRoutine
      LDY #$07
	  JSR $9273
      LDA #$3A
      RTS
Default:
    LDA $11
EndRoutine:
    RTS

;; alternate routine if NPC Shuffle is off

New_Talk_Astos:
  NOP                           ; Leave some space for code modifications 
  NOP
  NOP
  LDA item_crown                ; Load Crown
  BEQ Default1                  ; Do we have it?
    LDA tmp+3                   ; If yes, is there an item to give?
    BEQ Default1                
      JSR GiveItem              ; Give item
      BCS End1                  ; If inventory is full, skip
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
      LDA #$3A                  ; Show give item dialog
      RTS
Default1:
   LDA tmp+1                    ; Show default dialog
End1:
   RTS

