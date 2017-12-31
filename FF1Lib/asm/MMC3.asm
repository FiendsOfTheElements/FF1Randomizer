InitializeMMC3:

	STA $E000 ; disable IRQ (A can be any value at all)

	LDA #$80
	STA $A001 ; enable PRG RAM, unprotected

	LDY #$00
	STY $A000 ; vertical mirroring

	; Here we initialize CHR RAM.  We want it to act like 8 kB of flat RAM,
	; since FF1 expects a single 8 kB bank.  This is never swapped, so we just
	; set it up and leave it.
	; MMC3 in CHR mode 0 has two banks of 2 kB and four banks of 1 kB.

	STY $8000 ; select R0
	STY $8001 ; map CHR bank 0
	INY
	STY $8000 ; select R1
	INY
	STY $8001 ; map CHR bank 2
	STY $8000 ; select R2
	INY
	INY
	STY $8001 ; map CHR bank 4
	LDA #$03
	STA $8000 ; select R3
	INY
	STY $8001 ; map CHR bank 5
	LDA #$04
	STA $8000 ; select R4
	INY
	STY $8001 ; map CHR bank 6
	LDA #$05
	STA $8000 ; select R5
	INY
	STY $8001 ; map CHR bank 7



SwapPRG:

	; FF1 expects to map banks of 16 kB, numbered $00-0E (and after expansion,
	; $0F-1E).  MMC3 uses 8 kB banks, so we need to map two banks every time
	; FF1 wants to map one.  The low bank index will be twice the requested
	; index; e.g., if bank 3 is requested, we need to map banks 6 and 7.

	STA $60FC ; save the bank being jumped to for LongJump routines
	PHA       ; save the bank we want
	LDA #$06
	STA $8000 ; select R6
	PLA       ; retrieve the bank
	ASL       ; multiply by 2
	STA $8001 ; map the low bank
	PHA       ; save the low bank
	LDA #$07
	STA $8000 ; select R7
	PLA       ; retrieve the low bank
	ADC #1    ; add 1 to get the high bank
	STA $8001 ; map the high bank
	LDA #$00  ; A must be 0 when this function returns
	RTS
