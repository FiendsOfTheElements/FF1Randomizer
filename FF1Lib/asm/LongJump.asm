; General Purpose Long Jump with modified SwapPRG routine
; Uses 7 bytes of zero-page temp ram, $E8-$EE
; Called from a table with the following structure:
;		JSR LongJump
;		.byte A, B, C
; where A B make up the address to jump to and C is the bank to jump to
; in that bank of the routine. Table entries need not be contiguous.
; Previously swapped in bank will be automatically swapped back to.
; The previous bank to swap back to is stored at $60FC, and is
; saved before any swapping takes place

define tmp_oldbank $E8
define tmp_A $E9
define tmp_Y $EA
define tmp_addr_a $EB
define tmp_addr_b $EC
define tmp_dest $ED
define tmp_dest_b $EE

LongJump:
	STA tmp_A           ; save A and Y
	TYA
	STA tmp_Y
	PLA                 ; pop return addr from stack, put in zero page
	STA tmp_addr_a
	PLA
	STA tmp_addr_b
	LDY #$1
	LDA (tmp_addr_a), Y ; get jump address, write to temp+$D
	STA tmp_dest
	INY
	LDA (tmp_addr_a), Y
	STA tmp_dest_b
	INY
	LDA $60FC           ; get bank currently loaded and save it
	STA tmp_oldbank
	LDA (tmp_addr_a), Y ; get jump bank
	JSR $FE03           ; SwapPRG_L
	LDA #$D7            ; write post-jump address to stack to masquerade as a JSR, $D7F5
	PHA
	LDA #$F5
	PHA
	LDA tmp_A           ; restore registers
	LDY tmp_Y
	JMP ($00ED)         ; actual function jump
	; <----------  post-jump address mentioned above
	STA tmp_A
	LDA tmp_oldbank     ; get bank to jump back to
	JSR $FE03           ; SwapPRG_L
	LDA tmp_A
	RTS
	; total size: 56 bytes
