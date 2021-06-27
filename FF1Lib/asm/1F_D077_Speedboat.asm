joy             = $20  ; gamepad state
move_speed      = $34  ; pixels to move per frame (map)
vehicle         = $42  ; 1=walking, 2=canoe, 4=ship, 8=airship

 .ORG $D077

	LDA vehicle ; load vehicle
	LSR         ; shift right, if walking it'll set the carry flag
	BCS walking ; branch if carry, i.e. vehicle=1
	LSR         ; shift right again (if canoe it'll set the carry flag)
walking:
	LDA #$02             ; set move speed 2, clears N, Z  (but not carry)
	BCS walkingOrCanoe   ; branch if carry (vehicle was 1 or 2)
	ASL                  ; shift left, to get move speed 4, vehicle was ship/airship
walkingOrCanoe:
	BIT joy      ; check joystick state: set Z if bit 5 is not set, V if bit 6 is set, N if bit 7 is set
	BVC dash     ; branch if V is clear (I guess that means bit 6 is B button)
	LSR          ; V was set, which means B was pressed, so shift right (cut speed in half)
dash:
	STA move_speed  ; store movement speed
	RTS             ; return
	NOP
	NOP
	NOP
