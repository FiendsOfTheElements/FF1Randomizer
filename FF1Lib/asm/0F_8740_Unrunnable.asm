; A version of DrawCommandMenu minus the final JMP to draw the block buffer
; with the additional feature that it checks and replaces RUN with WAIT if unrunnable

DrawCommandMenu:
LDY #$00
LDX #$00

Loop:
	LDA $FA1B, Y  ; Loop to eventually copy 30B into block buffer
	STA $6A9E, X  ; block buffer pointer and offset
	INX

	CPY #27       ; After Y reaches 27 we check and print WAIT instead of RUN if unrunnable
	BNE cont0
		LDA $6D91 ; Load and check unrunnable bit
		AND #$03 ; this is changed for compatibility with splitting the unrunnables
		BEQ cont0
			LDA #$13       ; Replace RUN ptr with WAIT ptr
			STA $6A9E, X   ; $F713 is in bank 1F immediately after the code that jumps here
			INX
			INY
			LDA #$F7
			STA $6A9E, X
			INX
			INY
cont0:
	CPX #$05
	BNE cont1     ; every 5 bytes, add the block to the
	JSR $F690     ; output buffer
	LDX #$00
cont1:
	INY
	CPY #30       ; 6 blocks * 5 bytes per block
	BNE Loop

RTS

; The code below replaces the real DrawCommandMenu in 1F and calls out to the code above in Bank 0F

LDA $60FC ; Push current bank and setup jump
PHA
LDA #$0F
JSR $FE03
JSR $8740 ; JSR to above code
PLA
JSR $FE03

JMP $F648 ; JMP DrawBlockBuffer
.BYTE $A0, $8A, $92, $9D, $00 ; "WAIT" String pointed to by above code

; This snippet replaces a useless block of code in the TryRun handler to print a different message
; when the battle is unrunnable as opposed to a failure to run when possible. It must be exactly 15B

CLC             ; When we reach this code normally we want to unconditionally branch past the handler below
BCC $0008
	LDA #$21    ; This is the actual handler. Set the Ineffective id
	JMP $AA07   ; JMP out to print the message and escape
NOP
NOP
NOP
NOP
NOP
NOP
NOP
