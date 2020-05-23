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
;;	Certain game flags will need to be reset under particular
;;	conditions.  Particularly, this means that Garland,
;;	Bikke, Astos, Vampire, and the four fiends - map objects
;;	which trigger a fight - must be reset if particular
;;	conditions are not set.  These conditions are:
;;	Garland: If did not talk to the princess (princess 1 in
;;	tof is visible), Garland is reset to visible.
;;	Bikke (no NPC shuffle): If the terrified townsfolk are
;;	not visible, Bikke's flag to indicate that he has been
;;	fought is reset.
;;	Bikke (NPC shuffle): If the event flag for bikke is not
;;	1, then the terrified townsfolk are turned to invisible.
;;	Astos: The NPC code for Astos is changed
;;	to function like Bikke (see additional code).  A bat in
;;	Astos' castle is set to turn on visibility if Astos is
;;	defeated.  If this bat is not visible, then Astos'
;;	event flag is reset.
;;	Vampire: If the chest Vampire is guarding is not open,
;;	then Vampire's visibility is reset.  This does mean that
;;	if the chest is not picked up because of inventory space
;;	then Vampire will respawn (and if Early Sarda is off,
;;	you are in for a rude awakening).
;;	Earth/Fire/Water/Air orb: If the appropriate orb is not
;;	in the player's inventory, then the visibility flag for
;;	the the fiend fights is turned back on.
;;	Garland 1/2/3 (the version you encounter in ToFR) will
;;	always be reset to their default values as if you never
;;	entered the room.

	JSR $8BE3

GameOverUpdateSaveData:
	LDX #$00
RestorePlayerLoop:
	LDA $610C,X
	STA $610A,X
    LDA $610D,X
	STA $610B,X
	LDA $6328,X
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
	LDA #$00
	STA $6101,X
	TXA
    CLC
    ADC #$40
    TAX
    BNE RestorePlayerLoop

ResetGarlandFlagIfPrincessStillVisible:
	LDA $6203
	AND #$01
	BEQ ResetBikkeIfDidNotReceiveItem
	LDA $6202
	ORA #$01
	STA $6202
ResetBikkeIfDidNotReceiveItem:
	LDA $6204
	AND #$02
	BNE ResetAstosIfStillVisible
	LDA $623F
	AND #$FE
	STA $623F
ResetAstosIfStillVisible:
	LDA $6207
	AND #$01
	BEQ ResetVampireIfDidNotReceiveRubyChest
	LDA $6240
	AND #$FE
	STA $6240
ResetVampireIfDidNotReceiveRubyChest:
	LDA $623D
	AND #$04
	BNE ResetLichIfDidNotReceiveEarthOrb
	LDA $620C
	ORA #$01
	STA $620C
ResetLichIfDidNotReceiveEarthOrb:
	LDA $6031
	BNE RsetKaryIfDidNotReceiveFireOrb
	LDA $621B
	ORA #$01
	STA $621B
RsetKaryIfDidNotReceiveFireOrb:
	LDA $6032
	BNE RsetKrakenIfDidNotReceiveWaterOrb
	LDA $621C
	ORA #$01
	STA $621C
RsetKrakenIfDidNotReceiveWaterOrb:
	LDA $6033
	BNE ResetTiamatIfDidNotReceiveAirOrb
	LDA $621D
	ORA #$01
	STA $621D
ResetTiamatIfDidNotReceiveAirOrb:
	LDA $6034
	BNE ResetChaosAlways
	LDA $621E
	ORA #$01
	STA $621E
ResetChaosAlways:
	LDA $6218
	ORA #$01
	STA $6218
	LDA $6219
	AND #$FE
	STA $6219
	LDA $621A
	AND #$FE
	STA $621A

	LDA $6004
	BNE AirshipSpawn
	LDA $6000
	BEQ StartCopying
ShipSpawn:
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

;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;
;;
;;	NewAstosRoutine	[$9F45 ;; 0x39F55]
;;
;;	This is a new routine for Astos, in which he
;;	behaves like Bikke and will give you his item
;;	AFTER you have beaten him, not when you fight
;;	him.  This is necessary for death-saving to
;;	not break the game by letting you die to Astos
;;	and keep the item.  We use some of the space
;;	that was cleared out in bank 0E's character
;;	creation routines to accomplish this.
;;	If NPC Shuffle is off, instead of loading the
;;	GiveItem text it will load the dialogue for
;;	"Hurray!!" to indicate that you defeated Astos

New_Talk_Astos:
	LDA $6022
	BEQ Default
    LDY #$40
    JSR $9091
    BCS AlreadyFought
      JSR $90A4
      LDA #$7D
      JSR $90C5
      LDA $11
      RTS
AlreadyFought:
      LDA $13
      BEQ Default
      JSR $D094
      BCS EndRoutine
      LDY #$07
	  JSR $9273
      LDA #$3A
      RTS
Default:
    LDA $12
EndRoutine:
    RTS

;; alternate routine if NPC Shuffle is off

New_Talk_Astos:
    LDA $6022
    BEQ Default
    LDY #$40
    JSR $9091
    BCS AlreadyFought
      JSR $90A4
      LDA #$7D
      JSR $90C5
      LDA $11
      RTS
AlreadyFought:
      INC $6023
      LDY #$07
      JSR $9273
      INC $7D
      LDA #$77
      RTS
Default:
    LDA $12
EndRoutine:
    RTS
