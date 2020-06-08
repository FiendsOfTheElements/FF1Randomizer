;; Superseded for Hacks.cs/EnableTwelveClasses() - see "0E_95AE_DoClassChange-2.asm"

define ch_class $6100
define lut_ptr $95D0
define dlgflg_reentermap $56

LDX #3 ; Reverse counter

DoClass:
  LDY lut_ptr, X   ; four byte lut of 0xC0, 0x80, 0x40, 0x00
  LDA ch_class, Y  ; $6100 + above offset
  BMI Skip         ; 0xFF is NONE class (only negative class)
  CLC              ; Otherwise just add 6
  ADC #6
  STA ch_class, Y
  Skip:
	DEX            ; Decrement our index from 3 to 0 and repeat
	BPL DoClass

INC dlgflg_reentermap
RTS

; Install this LUT at lut_ptr
.BYTE $C0, $80, $40, $00
