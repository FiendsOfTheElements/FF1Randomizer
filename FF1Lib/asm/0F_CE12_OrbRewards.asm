; Orb Rewards for each of the special tiles behind the fiends.
; For these to work properly the jump table at 0x7CDB9 needs tweaking
; as shown below
Earth:
LDX #2
LDY #0
BEQ Continue ; Always

Fire:
LDX #2       ; Orb reward
LDY #1       ; Orb addr offset
BNE Continue ; Always

Water:
LDX #4
LDY #2
BNE Continue ; Always

Air:
LDX #4
LDY #3

Continue:
LDA $6031, Y ; orb lit?
BNE SMMove_OK

LDA #$01
STA $6031, Y ; set orb

CLC
TXA
ADC $6035    ; add in existing total
STA $6035    ; store total shards
INC $6C      ; effect

SMMove_OK:
CLC
RTS

; For the above code to work properly the jump table at 0x7CDB9 must be overwritten with this.
; Curiously, the orbs themselves are out of order in RAM so to make the
; handling easier we rearrange the handlers here.
.WORD CE12, CE18, CE1E, CE24
