LDA #$43      ; $20 away from the top left corner THIS VALUE IS OVERWRITTEN in ShardHunt.cs
STA $11       ; store low byte around for multiple rows

LDA #$20      ; ppuHighByte (OVERWRITTEN)
STA $12

LDA #$77      ; Character to draw
LDY #0        ; total counter

DrawFullRow:
PHA           ; Push printable character onto stack
LDA $2002     ; reset PPU toggle
LDA $12       ; set PPU addr high byte (Also OVERWRITTEN in ShardHunt.cs)
STA $2006
LDA $11       ; load saved low byte and add $20
CLC
ADC #$20
STA $11       ; Add a row and save
STA $2006
BCC Continue  ; If it didn't carry we're good
  LDA $2002   ; But if it does we reset the ppu and increment our high byte
  INC $12     
  LDA $12     
  STA $2006
  LDA $11     
  STA $2006

Continue:     ; Here we are ready to draw 6 shards
PLA           ; Pull printable character off stack
LDX #0        ; row counter

DrawNextOrb:
CPY $6035
BNE Cont0
  LDA #$76    ; Switch gfx to empty orb

Cont0:
CPY #36       ; Quit when finished. THIS VALUE IS OVERWRITTEN in ShardHunt.cs
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




