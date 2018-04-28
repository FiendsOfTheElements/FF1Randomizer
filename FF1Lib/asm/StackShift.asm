DoStandardMapJumpOut:
	LDA #$0F
	JSR $FE03 ; SwapPRG_L
	JMP $8B00 ; ShiftStack



ShiftStack:
	TSX            ; get the stack pointer
	CPX #$30       ; see if it's getting dangerously high (< $20)
	BCS PushAndJumpBack
	TXA
	STA $0110      ; save it to a spot above the stack
	LDA #$F4       ; start the copy from $FB - 7 bytes (sizeof DoStandardMap stack frame)
	TAX            ; X is copy source
	LDA #$FB       ; start copying to $FB
	TAY            ; Y is copy destination
	Loop:
		LDA $0100, X   ; read what's on the stack at source
		STA $0100, Y   ; copy 
		DEX
		DEY
		CPX #$10       ; stop when we've copied the whole stack down (and then some, for good measure)
		BNE Loop
	LDA $0110      ; get where we were on the stack when we started
	CLC
	ADC #7         ; add 7 (because we shifted the stack underneath ourselves!)
	TAX
	TXS            ; put it in the stack pointer

PushAndJumpBack:   ; flow into code copied out of DoStandardMap
    LDA $29 ; sm_scroll_x
    PHA
    LDA $2A ; sm_scroll_y
    PHA
    LDA $0D ; inroom
    PHA
    LDA $48 ; cur_map
    PHA
    LDA $49 ; cur_tileset
    PHA
	JMP $C965     ; jump back to DoStandardMap
