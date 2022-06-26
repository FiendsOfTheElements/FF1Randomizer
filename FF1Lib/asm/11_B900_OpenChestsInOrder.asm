.include "Constants.inc"
.include "variables.inc"

BANK_TALKROUTINE	= $11
GiveReward = $B410
SwapPRG = $FE03
lut_TreasureJingle = $B600 ;bank 11
lut_TreasureNew = $BF00 ;bank 11

;bank 1f
;original OpenTreasureChest @ 7DD78
.org $DD78
OpenTreasureChestInOrder:
	LDA #BANK_TALKROUTINE
    JSR SwapPRG
    LDA lut_TreasureJingle,X
    BNE GiveChestReward
        JSR OpenChestCount
        LDA lut_TreasureNew,X
	GiveChestReward:
    JSR GiveReward
    JSR SaveChestOpen
    TXA ;get the dialog id back in A
    RTS

;A9 11 20 03 FE BD 00 B6 D0 06 20 00 B9 BD 00 BF 20 10 B4 20 15 B9 8A 60 EA EA EA EA EA EA

;bank 11
;BA00
.org $B900
OpenChestCount:
	;figure out what chest we're on
    LDY #$00
	LDX #$00

    ChestCountLoop:
    LDA $6200,Y
    AND #$04
    BEQ ChestNotOpenCount
    LDA lut_TreasureJingle,Y
    BNE ChestNotOpenCount
    INX
    ChestNotOpenCount:
    INY
    BNE ChestCountLoop
	RTS

;;A0 00 A2 00 B9 00 62 29 04 F0 06 B9 00 B6 D0 01 E8 C8 D0 F0 60

;moved from bank 1F to make room
;B915
SaveChestOpen:
	BCS :+                   ; if 'C' is set jump ahead, otherwise mark the chest as open B00A
      LDY tileprop+1           ; get the ID of this chest A445
      LDA game_flags, Y        ; flip on the TCOPEN flag to mark this TC as open B90062
      ORA #GMFLG_TCOPEN        ; 0904
      STA game_flags, Y        ; 990062
	:
	RTS

;B0 0A A4 45 B9 00 62 09 04 99 00 62 60

;BF00
;0x100 of chest data, a list of treasures in order

;ca65 1F_DD78_OpenTreasureInOrder.asm --listing list.lst --list-bytes 32
