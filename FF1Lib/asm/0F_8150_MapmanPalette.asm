unsram          = $6000  ; $400 bytes
ch_stats        = unsram + $0100  ; MUST be on page bound.  Each character allowed $40 bytes, so use 00,40,80,C0 to index ch_stats
ch_class        = ch_stats + $00
cur_pal         = $03C0       ; 32 bytes
SwapPRG_L = $FE03             ; Bank $1F

	;; Upgrade the mapman sprite palette

	;; the original game has two palettes for the mapman sprites,
	;; one for the top half and one for the bottom half
	;; however, the entry for black (index 1) and "skin tone" (index 3)
	;; are the same for every character, and the game only changes
	;; the character's "primary" color (index 2).
	;;
	;; It is possible to change the "black" and "skin tone"
	;; entries, but only for all characters at once.
	;;
	;; To fix this, we replace the code that loads a single color into the
	;; top/bottom palettes with code that loads complete 4 color palettes.


.ORG $8150
lut_MapmanPalettes:

	;;  12 classes
	;;   2 mapman palettes each (top/bottom)
	;;   4 bytes per palette
	;; + 1 extra pair of palettes for the "none" mapman

.ORG $81C0
LoadMapmanPalette:
	LDA ch_class            ; get lead party member's class
	CMP #$FF
	BNE PaletteLoad
	LDA #13
PaletteLoad:
	ASL A                   ; multiply by 8
	ASL A
	ASL A
	ADC #$08        	; add 8
	TAX 			; put it in X

	LDY #$08                ; Y is loop counter
	@Loop:
	LDA lut_MapmanPalettes, X   ; get color from mapman palette
	STA cur_pal+$10, Y        ; copy to mapman palettes in RAM
	DEX
	DEY
	BPL @Loop             ; loop until Y wraps (8 iterations)

	RTS

	NOP
	BRK
	NOP


    ;; LDA #BANK_MAPMANPAL
    ;; JSR SwapPRG_L           ; swap to bank containing mapman palettes

    ;; LDA ch_class            ; get lead party member's class
    ;; ASL A                   ; double it, and put it in X
    ;; TAX

    ;; LDA lut_MapmanPalettes, X   ; use that as an index to get that class's mapman palette
    ;; STA cur_pal+$12
    ;; LDA lut_MapmanPalettes+1, X
    ;; STA cur_pal+$16


.ORG $D8B6

	LDA #$0F
	JSR SwapPRG_L           ; swap to bank containing mapman palettes
	JMP LoadMapmanPalette
	NOP
	NOP
	NOP
	NOP
	NOP
	NOP
	NOP
	NOP
	NOP
	NOP
	NOP
	NOP
	NOP
	NOP
	NOP
