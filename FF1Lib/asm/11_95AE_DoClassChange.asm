; New DoClassChange routine to prevent any bugs with promoting already promoted classes
:  and allow randomizing promotions

define ch_class $6100
define lut_promote $9C80
define dlgflg_reentermap $56

 .ORG $95AE

DoClassChange:
  LDX #$00				; Revert somewhat to classic class change routine
  JSR @DoClass			;  where we cycle manually through each class
  LDX #$40				
  JSR @DoClass			
  LDX #$80				
  JSR @DoClass			
  LDX #$C0				
  JSR @DoClass			
  INC dlgflg_reentermap	; Set bit to reload map and show promoted class
  RTS					

@DoClass:
  LDY ch_class, X		; Get current class 
  BMI @Skip			    ; Skip Nones
  LDA lut_promote, Y	; Find to which class it promote in the table
  STA ch_class, X		; Change class
@Skip:
  RTS				; 60

 .ORG $9C80
; Standard LUT at lut_promote for normal class change
 .BYTE $06, $07, $08, $09, $0A, $0B, $06, $07, $08, $09, $0A, $0B
