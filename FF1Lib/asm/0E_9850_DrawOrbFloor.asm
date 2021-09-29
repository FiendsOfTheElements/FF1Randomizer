
tmp		        = $10
dest_x          = $3A
dest_y          = $3B
text_ptr        = $3E ; 2 bytes
cur_map         = $48
format_buf      = $60  ; 7 bytes (5A-60) -- must not cross page bound

PrintNumber_2Digit = $8E66
DrawOrbBox = $B878
DrawMenuComplexString = $B944

 ; Bank 0E
 .ORG $B83D
  JSR DrawOrbFloor
  
 .ORG $9850
DrawOrbFloor:
  JSR DrawOrbBox

  LDA cur_map
  CMP #$3C
  BCS FloorZero
    CMP #$08
    BCC TownFloor   
      SEC
      SBC #$07
      JMP FloorNumber
TownFloor:
      TAY
      LDA lut_TownFirstLetter, Y
      STA format_buf-6
      LDA lut_TownSecondLetter, Y
      STA format_buf-5
      JMP PrintText
FloorZero:
  LDA #$00
FloorNumber:  
  STA tmp
  JSR PrintNumber_2Digit
  LDY #$00
  LDA (text_ptr), Y
  STA format_buf-6
  INY
  LDA (text_ptr), Y
  STA format_buf-5
PrintText:               ; "/52" + End of line
  LDA #$7A
  STA format_buf-4
  LDA #$85
  STA format_buf-3
  LDA #$82
  STA format_buf-2
  LDA #$00
  STA format_buf-1

  LDA #<format_buf-6
  STA text_ptr     
  LDA #>format_buf 
  STA text_ptr+1
  
  LDA #$02
  STA dest_y
  LDA #$04
  STA dest_x
  
  JMP DrawMenuComplexString
  
lut_TownFirstLetter: 
 .BYTE $8C, $99, $8E, $96, $8C, $98, $90, $95
lut_TownSecondLetter:
 .BYTE $B2, $B5, $AF, $A8, $B5, $B1, $A4, $A8

 .ORG $BAA2
   
 .BYTE $02, $01, $08, $09   ; Expand Orbs Box
 
