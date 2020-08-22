;; SetRunnability Subroutine
LDA $006a					; load formation # to check if it is B-Side
BPL ASIDE                   ; if it is, replace A-formation values with the B-formation values
	LDA $6d92				; use formation B min/max values (first slot)
	STA $6d8a
	LDA $6d93				; use formation B min/max values (second slot)
	STA $6d8b
	LDX #$00
	STX $6d88				; zero the enemy ID and min/max for enemies 2,3
	STX $6d89				; (only enemies 0,1 are used for B formations)
	STX $6d8c
	STX $6d8d
	LDA $6d91				; load the byte for unrunnability
	AND #$FE				; mask out the last bit (last bit is A-Side's runnability)
	STA $6d91				; and store the new runnability bit
	RTS						; then leave the subroutine
ASIDE:
	LDA $6d91				; load the byte for unrunnability
	AND #$FD				; mask out the second to last bit (second to last bit is B-Side's runnability)
	STA $6d91				; and store the new runnability bit
	RTS

;; For this subroutine to work properly, battle formation data must be set by the randomizer to account for the new runnability (if both sides are unrunnable, set both bits)
;; Additionally, areas of the code that refer to the unrunnability bit must be set to AND #$03 instead of AND #$01
