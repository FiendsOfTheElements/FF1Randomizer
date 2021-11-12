
.org $B803

	JSR SetArchipelagoEndingBit
	NOP

.org $B5E0

SetArchipelagoEndingBit:
	LDY #$FE
	LDA game_flags, Y   ; get the game flags
	ORA #GMFLG_EVENT    ; set the event bit
	STA game_flags, Y   ; and write back

	LDA #$43
	STA music_track           ; start music track $43 (epilogue music)
	RTS


A0FE
B90062
0902
990062
A943854B60
