.include "Constants.inc"
.include "variables.inc"


shop_quantity = $030A

.org $9F48

NewCheckForSpace: ; we need a slightly more complex formula for 
	LDX shop_curitem
	CPX #$16                   ; CMP ItemId with Tent
	BCC :+	
		LDA items, X
		CLC
		ADC shop_quantity	; add the shop item quantity
		CMP #$64
		BCC SpaceAvailable
		RTS		; if we would have 100 or more of the item, return with carry set to indicate we have too many
  : LDX #$FF
	LDA game_flags, X   ; get the game flags 
	ORA #GMFLG_EVENT    ; set the event bit
	STA game_flags, X   ; and write back
	CLC
	RTS	
SpaceAvailable:
	STA items, X	; otherwise, add the items to the player's inventory and return with carry not set to indicate success
	CLC
	RTS
