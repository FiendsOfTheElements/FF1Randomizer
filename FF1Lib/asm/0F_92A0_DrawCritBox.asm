; Draw the precise number of crits instead of just "Critical Hit!"

define critcount $686B
define strbuf $6B3A
define critstr1 $9280
define critstr2 $9290

DrawCritCount:
	LDA critcount
	CMP #$01         ; Just print vanilla string if 1 crit.
	BEQ Vanilla

	LDX #$01         ; Put crit count in strbuf at index 1
	STA strbuf, X

	LDA #$11
	STA strbuf       ; Control count to print XXYY num at index 0

	LDA #$00
	INX
	STA strbuf, X    ; High byte is 0 for printing crit count

	LDY #$FF         ; Start Y at -1 so we can INY before the load
PrintChar:           ; Print a char of the " Critical Hits!" string
	INY
	INX
	LDA critstr2, Y
	STA strbuf, X
	BNE PrintChar    ; Loop until null terminator - that's why we INY/INX before we LDA
	BEQ DrawCritBox  ; Always branch on null terminator to end

Vanilla:             ; Set up and print a singlular "Critical hit!!" string.
	LDX #$FF
	LDY #$FF

PrintVanillaChar:
	INY
	INX
	LDA critstr1, Y
	STA strbuf, X
	BNE PrintVanillaChar  ; Loop until null terminator - that's why we INY/INX before we LDA

DrawCritBox:
	LDX #$3A         ; Set up string ptr, box counter, draw, and inc box counter
	LDY #$6B
	LDA #$04
	JSR $F71C
	INC $6AF8
	RTS
