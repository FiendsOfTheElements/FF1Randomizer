; Replaces DrawEquipment_BUGGED in Bank 0F for credits screens
; This code should only be used in the end game credits because
; it clobbers the party's current gold value.

DrawIntegerAtAddr:
	LDY #$00 ; Y kept at 0 for indirect read
	LDX #$00 ; Counter for 3 bytes

	Next1:
	LDA ($3E), Y ; Read from text_ptr and increment it
	INC $3E
	BNE Skip1
		INC $3F

	Skip1:
	STA $10, X ; Write to $tmp

	INX
	CPX #$03 ; do three bytes
	BNE Next1

	; Blow away current gold's upper bytes. Low one always
	; going to get clobbered anyway.
	STY $601D
	STY $601E

	Next2:
	LDA ($11), Y ; Pointed to value to overwrite gold
	STA $601C, Y ; Print to gold
	INY
	CPY $10 ; Continue until number of bytes specified
	BNE Next2
	
	JMP $DE45 ; JMP to Draw_NoStall and continue string
