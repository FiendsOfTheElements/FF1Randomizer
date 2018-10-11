define btl_drawflagsA $6AD1
define btl_usepalette $6B87
define btl_usepalette_m1 $6B86
define btl_palettes_m1 $6D33
define BattleWaitForVBlank $F4A1
define oam_lowbyte $02

LDA btl_drawflagsA
AND #$10 ; reset palette if drawing cursor
BEQ SkipCursor
  LDY #$20 ; copy $20 bytes over
  PaletteLoop:
      LDA btl_palettes_m1, Y ; Y is 1-based, stops at 0
      STA btl_usepalette_m1, Y
      DEY
      BNE PaletteLoop

SkipCursor:
LDA btl_drawflagsA ; clear drawflags
AND #$0F
STA btl_drawflagsA

JSR BattleWaitForVBlank

LDA $2002
LDA #oam_lowbyte
STA $4014

LDA #$3F  ; set PPU addr to point to palettes
STA $2006
LDA #$00
STA $2006
    
LDY #$00  ; draw the usepalette to the PPU
Loop:
  LDA btl_usepalette, Y
  STA $2007
  INY
  CPY #$20
  BNE Loop
      
LDA #$3F  ; reset PPU address
STA $2006
LDA #$00
STA $2006
STA $2006
STA $2006

RTS
