.include "Constants.inc"
.include "variables.inc"

SwapPRG = $FE03
BattleDraw_AddBlockToBuffer = $F690
ClearUnformattedCombatBoxBuffer = $F757
DrawBlockBuffer = $F648
CancelBattleAction = $94AF
DoNothingMessageBox = $96FE
SelectPlayerTarget = $9A3A
UndrawNBattleBlocks_L = $F6B3
MenuSelection_Drink = $9C06
BtlMag_Effect_CureAilment_Return = $B9D6-1
BtlMag_Effect_CureAilment_Return2 = $B9D9-1
BtlMag_Effect_RecoverHP = $B999-1
BtlMag_Effect_Smoke_Return = $A424-1

lut_drinkbox_order = $F930
lut_CombatDrinkBox = $A010

btl_unfmtcbtbox_buffer2 = $6B1A ; i don't use the first buffer because there's an inconvenient page break there

tmp1 = $6856
tmp2 = $6857
tmp3 = $6858

.org $A020

;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;
;;
;;  DrawDrinkBox  [$A020 :: 0x3F931]
;;
;;    Draws the "Drink box" that appears in the battle menu when the player
;;  selects the DRINK option in the battle menu
;;
;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;

DrawDrinkBox:
    LDY #$05
    : LDA lut_CombatDrinkBox-1, Y       ; load the specs for the drink box
      STA btl_msgdraw_hdr-1, Y          ; -1 because Y is 1-based
      DEY
      BNE :-
    JSR BattleDraw_AddBlockToBuffer     ; add the box to the block buffer
    
    JSR ClearUnformattedCombatBoxBuffer ; clear the unformatted buffer (we'll be drawing to it)
    
    INC btl_msgdraw_hdr                 ; For text, hdr=1
    INC btl_msgdraw_x                   ; move text right+down 1 tile from where the box was drawn
    INC btl_msgdraw_y
	
	LDX #$00
	
	LDY #$00							; load buffer index
	JSR DrawDrinkBoxRow
	
	LDY #$18							; load buffer index
	JSR DrawDrinkBoxRow
	
	LDY #$30							; load buffer index
	JSR DrawDrinkBoxRow
	
	LDY #$48							; load buffer index
	JSR DrawDrinkBoxRow
	
	JMP DrawBlockBuffer					
	
DrawDrinkBoxRow:	
	TYA						; save buffer index
	PHA
	
	JSR DrawDrinkBoxEntry	; draw item
	INX						; increase drinkbox index
	JSR DrawDrinkBoxEntry	; draw item
	INX						; increase drinkbox index
	
	LDA #$00
	STA btl_unfmtcbtbox_buffer2, Y	; terminate the buffer
	
	CLC									; calculate and set the block buffer srcptr
	PLA
	ADC #<btl_unfmtcbtbox_buffer2
    STA btl_msgdraw_srcptr
	LDA #>btl_unfmtcbtbox_buffer2
	ADC #$00
	STA btl_msgdraw_srcptr+1
	
    JSR BattleDraw_AddBlockToBuffer     ; and add the block (drawing the Heal Potions)
	
	INC btl_msgdraw_y                   ; move down 2 rows
    INC btl_msgdraw_y
	
	RTS
	
DrawDrinkBoxEntry:
	TXA										; save drinkbox index
	PHA
	
	LDA lut_drinkbox_order, X				; load item id
	PHA
	BEQ NothingToDraw						; if item id not set in lut_drinkbox_order, don't draw anything
	TAX
	LDA items, X
	BEQ NothingToDraw						; if item count is 0, don't draw anything
	TAX
	
	INY
	INY
	LDA #$0E
	STA btl_unfmtcbtbox_buffer2, Y		; print item name cmd
	INY
	PLA
	STA btl_unfmtcbtbox_buffer2, Y		; item		
	TXA
	CMP #100
	BCS LessThan100
	INY
LessThan100:
	TXA
	CMP #10
	BCS LessThan10
	INY
LessThan10:	
	INY
	LDA #$11
	STA btl_unfmtcbtbox_buffer2, Y		; print number cmd
	INY
	TXA
	STA btl_unfmtcbtbox_buffer2, Y		; item count low byte
	INY
	LDA #$00
    STA btl_unfmtcbtbox_buffer2, Y       ; item count high byte
	INY
	
	PLA										; restore drinkbox index
	TAX		
	RTS
	
NothingToDraw:
	LDA #$10
	STA btl_unfmtcbtbox_buffer2, Y		; print multiple spaces cmd
	INY
	LDA #$0A
	STA btl_unfmtcbtbox_buffer2, Y		; print multiple spaces count
	INY
	
	PLA
	PLA										; restore drinkbox index
	TAX		
	RTS


;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;
;;
;;  BtlMag_Effect_CureAilment  [$A0C6]
;;
;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;

BtlMag_Effect_CureAilment:
    LDA btlmag_effectivity          ; Check for Smoke Spell
	BEQ BtlMag_Effect_Smoke
	
    LDA btlmag_defender_ailments    ; Get the defender's ailments
    AND btlmag_effectivity          ; See if they have any of the ailments we're trying to cure
    BEQ BtlMag_Effect_CureAil_RTS   ;  If not, no effect, just exit
    
    LDA btlmag_effectivity          ; Otherwise, cure ailment bits
    EOR #$FF
    AND btlmag_defender_ailments
    STA btlmag_defender_ailments
    
	LDA btlmag_effectivity          ; Check for Life Spell
	LSR
	BEQ BtlMag_Effect_Life
	
	LDA #>BtlMag_Effect_CureAilment_Return
	PHA
	LDA #<BtlMag_Effect_CureAilment_Return
	PHA
	LDA #$0C
	JMP SwapPRG
BtlMag_Effect_CureAil_RTS:
	LDA #>BtlMag_Effect_CureAilment_Return2
	PHA
	LDA #<BtlMag_Effect_CureAilment_Return2
	PHA
	LDA #$0C
	JMP SwapPRG
BtlMag_Effect_Smoke:
	PLA								;StackManip to end the BattleRound
	PLA
	PLA
	PLA
	PLA
	PLA
	LDA #>BtlMag_Effect_Smoke_Return
	PHA
	LDA #<BtlMag_Effect_Smoke_Return
	PHA
	
	LDA btlformation				;Fiend2Fix
	CMP #$73
	BEQ BtlMag_Effect_Smoke_Fiend2
	CMP #$74
	BEQ BtlMag_Effect_Smoke_Fiend2
	CMP #$75
	BEQ BtlMag_Effect_Smoke_Fiend2
	CMP #$76
	BEQ BtlMag_Effect_Smoke_Fiend2
BtlMag_Effect_Smoke_End:
	LDA #$0C
	JMP SwapPRG
BtlMag_Effect_Life:
	LDA #$10
	STA btlmag_effectivity
	LDA #>BtlMag_Effect_RecoverHP
	PHA
	LDA #<BtlMag_Effect_RecoverHP
	PHA
	LDA #$0C
	JMP SwapPRG
BtlMag_Effect_Smoke_Fiend2:
	LDA facing           ; check which direction we're facing
    LSR A                ; shift until we find the appropriate direction, and branch to it
    BCS @Right
    LSR A
    BCS @Left
    LSR A
    BCS @Down
    LSR A
    BCS @Up
	JMP BtlMag_Effect_Smoke_End
@Right:
	DEC sm_scroll_x
	JMP BtlMag_Effect_Smoke_End
@Left:
	INC sm_scroll_x
	JMP BtlMag_Effect_Smoke_End
@Down:
	DEC sm_scroll_y
	JMP BtlMag_Effect_Smoke_End
@Up:
	INC sm_scroll_y
	JMP BtlMag_Effect_Smoke_End











	