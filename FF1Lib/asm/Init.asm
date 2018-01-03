define hard_reset_count_lo $64A3
define hard_reset_count_hi $64A4
define soft_reset_count_lo $64A5
define soft_reset_count_hi $64A6

ExtendedGameStart:
	LDA #0                      ; Turn off the PPU
	STA $2000
	STA $2001

	STA $FE ; unk_FE            ?? I don't think this is ever used

	LDA #$08                    ; Sprites use pattern table at $1xxx
	STA $FF ; soft2000
	STA $FD ; NTsoft2000

	LDA $1B ; GameStart is called twice, see if we already ran this block
	CMP #$01
	BNE Continue
	RTS

;;;;;;;;Set encounter table location to known value
Continue:
	LDA #$01    ; mark that this block has been executed
	STA $1B
	LDA #$4D    ; Check for hard reset
	CMP $F9
	BEQ enc_end ; Skip if not
	;;; Hard Reset
	LDA #$FF ; Most common init value for emulators
	STA $F5  ; battlestep
	STA $F6  ; battlestep_sign
	STA $F7  ; battlecounter
enc_end:
	CLC
	JSR $C888 ;check sram crc, sets carry if invalid
	BCS no_sram
	LDA #$4D ;Check for hard reset
	CMP $F9
	CLC
	BEQ SoftReset
;;;Hard Reset
	LDA hard_reset_count_lo ;one byte after sram for bridge scene viewed
	ADC #$1
	STA hard_reset_count_lo
	LDA hard_reset_count_hi
	ADC #$0
	STA hard_reset_count_hi
	CLC ;Unconditional branch to make CRC valid again
	BCC RecalculateCRC
SoftReset:
	LDA soft_reset_count_lo
	ADC #$1
	STA soft_reset_count_lo
	LDA soft_reset_count_hi
	ADC #$0
	STA soft_reset_count_hi
RecalculateCRC:
	LDA #$00
    STA $64FD                 ; sram_checksum, clear the checksum byte so that it will not interfere with checksum calculations
    LDX #$00                  ; clear X (loop counter)
    CLC                       ; and clear carry so it isn't included in checksum

	ChecksumLoop:
		ADC $6400, X    ; sum every byte in SRAM
		ADC $6500, X    ;  note that carry is not cleared between additions
		ADC $6600, X
		ADC $6700, X
		INX
		BNE ChecksumLoop     ; loop until X expires ($100 iterations)

                      ; after loop, A is now what the checksum computes to
    EOR #$FF          ;  to force it to compute to FF, invert the value
    STA $64FD ;  and write it to the checksum byte.  Checksum calculations will now result in FF
	CLC
	BCC DoneExtraInit
no_sram:
	;; initialize tracking variables
	LDX #$A0
	LDA #$0
	Loop:
		STA $6000, X
		STA $6400, X
		INX      ; loop until $60A0 through $60FF are zeroed
		BNE Loop ; also $64A0 through $64FF

DoneExtraInit:
	RTS
