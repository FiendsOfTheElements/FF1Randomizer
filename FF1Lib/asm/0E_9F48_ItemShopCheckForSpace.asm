.include "Constants.inc"
.include "variables.inc"


shop_quantity = $030A
shop_curitem  = $030C
items		  = $6020

.org $9F48

NewCheckForSpace:
	LDX shop_curitem
	CPX #$16
	BCS NormalItem
		LDY #$FF
		LDA game_flags,Y
		ORA #GMFLG_EVENT
		STA game_flags,Y
		CLC
		NOP					; AP: RTS
	NormalItem:
		LDA items,X
		CLC
		ADC shop_quantity
		CMP #$64
		BCC SpaceAvailable
			RTS
	SpaceAvailable:
		STA items,X
		CLC
		RTS
	;(34 bytes)

; NewCheckForSpace: ; we need a slightly more complex formula for 
; 	LDX shop_curitem
; 	CPX #$16                   ; CMP ItemId with Tent
; 	BCC :+	
; 	  Quantity:
; 		LDA items,X
; 		CLC
; 		ADC shop_quantity	; add the shop item quantity
; 		CMP #$64
; 		BCC SpaceAvailable
; 		RTS		; if we would have 100 or more of the item, return with carry set to indicate we have too many
;   : LDY #$FF
; 	LDA game_flags,Y   ; get the game flags 
; 	ORA #GMFLG_EVENT    ; set the event bit
; 	STA game_flags,Y   ; and write back
; 	BNE Quantity	   ; AP: CLC
; 					   ;     RTS	
; SpaceAvailable:
; 	STA items, X	; otherwise, add the items to the player's inventory and return with carry not set to indicate success
; 	CLC
; 	RTS
; 	;(36 bytes)


