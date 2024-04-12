tmpA = $10
cur_map = $48
location = $F2
ch_stats = $6100
ch_armor = ch_stats + $1C
ch_curhp = ch_stats + $0A

 .ORG $B100

AssignMapTileDamage:
	LDX #$00					; zero loop counter and char index

	@Loop:
		LDA #$01				; only do any of the special armor checks if the randomizer has set this to 0
		BNE @Damage
		LDA location            ; see if we're on the overworld, or in a standard map
		CMP #$03
		BCC @Damage             ; if we're on the overworld, skip to damage

	@VolcanoCheck:
		LDA cur_map				; get the current map
		CMP #$0E				; see if it's Volcano
		BEQ @Volcano
		CMP #$21
		BEQ @Volcano
		CMP #$22
		BEQ @Volcano
		CMP #$23
		BEQ @Volcano
		CMP #$24
		BEQ @Volcano
		JMP @IceCheck

	@Volcano:
		LDA #$88 ; Ice Armor ID
		JSR @ArmorCheck
		BNE @Damage
		JMP @DmgSkip

	@IceCheck:
		CMP #$0F					; see if it's Ice
		BEQ @Ice
		CMP #$25
		BEQ @Ice
		CMP #$26
		BEQ @Ice
		CMP #$27
		BEQ @Ice

	@Ice:
		LDA #$87 ; Flame Armor ID
		JSR @ArmorCheck
		BEQ @Damage
			JMP @DmgSkip

	@Damage:
		LDA ch_curhp+1, X		; check high byte of HP
		BNE @DmgSubtract		; if nonzero (> 255 HP), deal this guy damage
			LDA ch_curhp, X			; otherwise, check low byte
			CMP #2					; if < 2, skip damage (don't take away their last HP)
			BCS @DmgSubtract
				LDA #$01		; set to 1 HP
				STA ch_curhp, X
				JMP @DmgSkip

	@DmgSubtract:
		LDA ch_curhp, X			 ; subtract 1 HP
		SEC
		SBC #1
		STA ch_curhp, X
		LDA ch_curhp+1, X
		SBC #0
		STA ch_curhp+1, X

	@DmgSkip:
		TXA						; add $40 to char index (next character in party)
		CLC
		ADC #$40
		TAX

		BNE @Loop				; loop until it wraps (4 iterations)
			RTS						; then exit

@ArmorCheck:
	STA tmpA	; store the armor ID so we can compare later
	TXA			; we need to increment this for each piece of armor, so make a copy in Y
	TAY
@ArmorLoop:
	LDA ch_armor, Y	; next armor
	CMP tmpA
	BNE @NoEquip
		LDA #$00
		RTS
@NoEquip:
	INY
	TYA
	AND #$04
	CMP #$04	; out of armor range
	BNE @ArmorLoop
		LDA #$01
		RTS

; A200A901D043A5F2C903903DA548C90EF013C921F00FC922F00BC923F007C924F0034C2FB1A9882076B1D01D4C6EB1C90FF00CC925F008C926F004C927F000A9872076B1F0034C6EB1BD0B61D00FBD0A61C902B008A9019D0A614C6EB1BD0A6138E9019D0A61BD0B61E9009D0B618A186940AAD08D6085108AA8B91C61C510D003A90060C8982904C904D0EEA90160
