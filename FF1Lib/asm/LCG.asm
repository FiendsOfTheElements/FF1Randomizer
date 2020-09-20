; Linear Congruential Generator (better random number generator)
; https://en.wikipedia.org/wiki/Linear_congruential_generator
; This is a simple but effective RNG with a much longer period than FF1's.

; A research paper on selecting good parameters for the multiplier:
; https://citeseerx.ist.psu.edu/viewdoc/download?doi=10.1.1.34.1024&rep=rep1&type=pdf
; We'll use a = 2891336453, or 0xAC564B05 from Table 4.
; Any odd integer will do for c, so we'll take a random value.



* = $FCE7
;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;
;; Bank 0F, $FCE7 (BattleRNG) ;;
;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;

battle_rng_state = $68AF

BattleRNG:
	LDA battle_rng_state       ; Get the first byte of state
	TAX
	LDA battle_rng_a           ; Get the first byte of m
	JSR MultiplyXA             ; Multiply state by m
	CLC
	ADC battle_rng_c           ; Add c
	BCC :+
		INX                    ; Increment high bits if necessary
		CLC
	:
	STA battle_rng_state       ; Store the low bits back to state
	TXA
	STA btltmp_multA           ; Save the high bits for the next step

	LDA battle_rng_state + 1   ; Now do it again for the next byte of state
	TAX
	LDA battle_rng_a + 1
	JSR MultiplyXA
	CLC
	ADC battle_rng_c + 1
	BCC :+
		INX
		CLC
	:
	ADC btltmp_multA           ; Add the high bits from the previous step
	BCC :+
		INX
		CLC
	:
	STA battle_rng_state + 1
	TXA
	STA btltmp_multA

	LDA battle_rng_state + 2
	TAX
	LDA battle_rng_a + 2
	JSR MultiplyXA
	CLC
	ADC battle_rng_c + 2
	BCC :+
		INX
		CLC
	:
	ADC btltmp_multA
	BCC :+
		INX
		CLC
	:
	STA battle_rng_state + 2
	TXA
	STA btltmp_multA

	LDA battle_rng_state + 3   ; Last byte
	TAX
	LDA battle_rng_a + 3
	JSR MultiplyXA
	CLC
	ADC battle_rng_c + 3
	CLC                        ; No need to save the high bits, so just CLC
	ADC btltmp_multA
	CLC                        ; Just in case
	STA battle_rng_state + 3

	; And we're done.  A already has the highest bits of state, and that's what we want to return.
	RTS

battle_rng_a:
	.BYTE $05, $4B, $56, $AC
battle_rng_c:
	.BYTE $00, $00, $00, $00 ; (this will be replaced by the randomizer)



; MultiplyXA copied from bank 0B
btltmp_multA = $68B3
btltmp_multB = $68B4
btltmp_multC = $68B5

MultiplyXA:
    STA btltmp_multA    ; store the values we'll be multiplying
    STX btltmp_multB
    LDX #$08            ; Use x as a loop counter.  X=8 for 8 bits
    
    LDA #$00            ; A will be the high byte of the product
    STA btltmp_multC    ; multC will be the low byte
    
    ; For each bit in multA
  @Loop:
      LSR btltmp_multA      ; shift out the low bit
      BCC :+
        CLC                 ; if it was set, add multB to our product
        ADC btltmp_multB
    : ROR A                 ; then rotate down our product
      ROR btltmp_multC
      DEX
      BNE @Loop
    
    TAX                     ; put high bits of product in X
    LDA btltmp_multC        ; put low bits in A
    RTS
