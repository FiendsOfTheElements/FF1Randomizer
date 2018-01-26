LDA #$43      ; $20 away from the top left corner THIS VALUE IS OVERWRITTEN in OrbHunt.cs
STA $11       ; store low byte around for multiple rows

LDA #$77      ; Character to draw
LDY #0        ; total counter

DrawFullRow:
PHA           ; Push printable character onto stack
LDA $2002     ; reset PPU toggle
LDA #$20      ; set PPU addr high byte
STA $2006
LDA $11       ; load saved low byte and add $20
CLC
ADC #$20
STA $11       ; Add a row and save
STA $2006
BCC Continue  ; If it didn't carry we're good
  LDA $2002   ; But the last row of orbs does...
  LDA #$21    ; And is always $2103
  STA $2006
  LDA #$03
  STA $2006

Continue:     ; Here we are ready to draw 6 shards
PLA           ; Pull printable character off stack
LDX #0        ; row counter

DrawNextOrb:
CPY $6035
BNE Cont0
  LDA #$76    ; Switch gfx to empty orb

Cont0:
CPY #36       ; Quit when finished. THIS VALUE IS OVERWRITTEN in OrbHunt.cs
BNE Cont1
  RTS

Cont1:
STA $2007     ; Write graphic to PPU
INY           ; increment total counter
INX           ; increment row counter
CPX #6
BNE DrawNextOrb

CLC
BCC DrawFullRow ; Always Branch




