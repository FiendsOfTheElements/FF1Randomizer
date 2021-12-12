.include "Constants.inc"
.include "variables.inc"


ChaosDeath = $A052

.org $9ADF

	JSR SetArchipelagoEndingBit

.org $9B40

SetArchipelagoEndingBit:
	LDY #$FE
	LDA game_flags, Y   ; get the game flags
	ORA #GMFLG_EVENT    ; set the event bit
	STA game_flags, Y   ; and write back

	JMP ChaosDeath
