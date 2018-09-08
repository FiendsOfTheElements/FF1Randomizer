; This code is just a humble string printing subroutine with fixed location and size
DrawSeedAndFlags:
	LDA #$23      ; Dest PPU Addr
	STA $2006
	LDA #$20
	STA $2006

	LDX #$0       ; Loop Counter

	CharLoop:
		LDA $8900, X
		STA $2007

		INX
		CPX #$60   ; $60 bytes is three full lines of PPU tiles
		BNE CharLoop

	RTS
