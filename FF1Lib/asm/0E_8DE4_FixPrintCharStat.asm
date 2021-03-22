char_index      = $67
tmp             = $10 ; 16 bytes
unsram          = $6000  ; $400 bytes
ch_stats        = unsram + $0100  ; MUST be on page bound.  Each character allowed $40 bytes, so use 00,40,80,C0 to index ch_stats
ch_str          = ch_stats + $10
ch_agil         = ch_stats + $11
ch_int          = ch_stats + $12
ch_vit          = ch_stats + $13
ch_luck         = ch_stats + $14
ch_mdef         = ch_stats + $25

PrintNumber_2Digit = $8E66
PrintNumber_3Digit = $8E70

.macro LDABRA v, lbl
    LDA #v
  .if v = 0
    BEQ lbl
  .else
    BNE lbl
  .endif
.endmacro

.ORG $8DE4

@Str:
    LDABRA <ch_str, @Stat3Digit

@Agil:
    LDABRA <ch_agil, @Stat3Digit

@MDef:
    LDABRA <ch_mdef, @Stat3Digit

@Vit:
    LDABRA <ch_vit, @Stat3Digit

@Luck:
    LDABRA <ch_luck, @Stat3Digit

@Stat3Digit:
    CLC
    ADC char_index
    TAX
    LDA ch_stats, X
    STA tmp            ; load a single byte, clear the upper byte
    LDA #0
    STA tmp+1          ; and print it as 3-digits
    JMP PrintNumber_3Digit
	NOP
	NOP
	NOP
	NOP
	NOP
	NOP
	NOP
	NOP


	BRK
	NOP
	BRK
