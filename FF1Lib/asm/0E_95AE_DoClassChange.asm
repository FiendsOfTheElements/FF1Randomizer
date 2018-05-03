define lut_promotion $9586
define dlgflg_reentermap $56

DoClassChange:
LDY #$00

loop:
TYA
ROR A                ; 1 becomes 40, 2 -> 80, 3 -> C0
ROR A
ROR A
TAX
LDA $6100,X          ; 6100,6140,6180,61C0
CMP #$FF
BEQ skip
TAX
STX $14
LDA lut_promotion,X
LDX $14
STA $6100,X

skip:
INY                   ; increment counter to go through all the party members
CPY #$04
BNE loop

INC dlgflg_reentermap6
RTS         
