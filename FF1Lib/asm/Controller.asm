;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;
;;
;;  Read Joypad Data - Modified to hard reset on Up+A from 2nd controller
;;
;;    This strobes the joypad and reads joy data into our 'joy' variable
;;
;;  OUT:  X is 0 on exit
;;
;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;

define joy $20
define joy2 $1E
define joy_ignore $21
define tmp $10
define tmpone $11
define joy_select $22
define joy_start $23
define joy_a $24
define joy_b $25

ReadJoypadData:
    LDA #1
    STA $4016    ; strobe joypad (refreshes the latch with up to date joy data)
    LDA #0
    STA $4016

    LDX #$08     ; loop 8 times (have to read each of the 8 buttons 1 at a time)

Loopone:
		LDA $4016  ; get the button state
		AND #$03   ; button state gets put in bit 0 usually, but it's on bit 1 for the Famicom if
		CMP #$01   ; the user is using the seperate controllers.  So doing this AND+CMP combo will set
		           ; the C flag if either of those bits are set (making this routine FC friendly)
		ROL joy    ; rotate the C flag (the button state) into our RAM
		LDA $4017  ; button state for controller 2
		AND #$03
		CMP #$01
		ROL joy2
		DEX
		BNE Loopone  ; loop until X expires (8 reads, once for each button)

	LDA joy2
	CMP #$88
	BEQ reset
	CMP #$48
	BEQ softreset
    RTS

softreset:
	JMP $FE2E ; jump to where the reset vector is pointing

reset:
	JSR $FEA8
	JSR $FEA8
	JSR $FEA8 ; wait for vblank - simulate PPU warmup
	LDX #$FF
	TXS       ; reset stack
	LDA #$0
	STA joy2
	
	ZeroPageLoop: ; zero zero page
		STA $0, X
		DEX
		BNE ZeroPageLoop

	LDX $0 ;OnReset expects X to be 0
	
	JMP $C012 ; GameStart
