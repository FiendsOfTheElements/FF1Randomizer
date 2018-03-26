;; The top half of this file draws boxes forwards; the latter draws backwards

;; Install at 0x8A00
SetupBattleDrawMessageBuffer:
    LDA #$40    ; Setup DST pointer
    STA $8A     ; $2240 into $008A
    LDA #$22
    STA $8B

    LDA #$1E    ; Setup SRC pointer
    STA $88     ; $691E into $0088
    LDA #$69
    STA $89
    RTS

;; Install at 0x8A20
BattleDrawMessageBuffer:
    LDA #$03    ; Loop counter overridden by BattleBoxDrawRows
    STA $68B9

    JSR $F4A1   ; BattleWaitVBlank - called outside of the loop to just do it once

Loop:
    JSR $F4E8   ; Battle_DrawMessageRow (without _VBlank)

    LDA $88     ; Update all pointers used by above
    CLC
    ADC #$20
    STA $88
    LDA $89
    ADC #$00
    STA $89

    LDA $8A
    CLC
    ADC #$20
    STA $8A
    LDA $8B
    ADC #$00
    STA $8B

    DEC $68B9
    BNE Loop

RTS

;; Bottom half for reverse draw. Nearly identical.
;; Install at 0x8A80
SetupBattleDrawMessageBufferReverse:
    LDA #$A0    ; Setup DST pointer
    STA $8A     ; $2240 into $008A
    LDA #$23
    STA $8B

    LDA #$7E    ; Setup SRC pointer
    STA $88     ; $691E into $0088
    LDA #$6A
    STA $89
    RTS

;; Install at 0x8AA0
BattleDrawMessageBufferReverse:
    LDA #$03    ; Loop counter  overridden by BattleBoxUndrawRows
    STA $68B9

    JSR $F4A1   ; VBlank

Loop:
    JSR $F4E8   ; Battle_DrawMessageRow without _VBlank

    LDA $88     ; subtract $20 from the source pointer
    SEC
    SBC #$20
    STA $88
    LDA $89
    SBC #$00
    STA $89

    LDA $8A     ; and from the dest pointer
    SEC
    SBC #$20
    STA $8A
    LDA $8B
    SBC #$00
    STA $8B


    DEC $68B9
    BNE Loop

RTS

;;
;; Code for Hacks.cs that draws the above. This replaces the real BattleDrawMessageBuffer:
;; Install at 0x7F4AA for the drawing operation, and 0x7F4AA for the undraw animation.
;;
BattleDrawMessageBuffer:
	LDA $60FC ; Classic push of bank onto stack
	PHA
	LDA #$0F  ; Select Bank 0x0F
	JSR $FE03 ; SwapPRG_L

	JSR $8A00 ; Setup routine primes some pointers (Change to $8A80 for backwards)

	LDA #$04  ; Setup Loop number of frames (overridden by BattleBoxDrawInFrames)
	STA $17   ; tmp + 7

	FrameLoop:
	  LDA #$0F  ; Reload Bank 0x0F
	  JSR $FE03 ; SwapPRG_L
	  JSR $8A20 ; BattleDrawMessageBuffer (Does 1 Vsync)  (Change to $8AA0 for backwards)
	  JSR $F485 ; Battle_UpdatePPU_UpdateAudio_FixedBank. This clobbers the loaded bank.
	  DEC $17   ; Check and Loop
	  BNE FrameLoop

	PLA         ; Pull and restore old cur_bank
	JSR $FE03   ; SwapPRG_L
	RTS
