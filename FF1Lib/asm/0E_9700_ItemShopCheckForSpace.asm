.include "Constants.inc"
.include "variables.inc"


shop_quantity = $030A

.org $9700

NewCheckForSpace: ; we need a slightly more complex formula for 
	LDA shop_curitem
	CLC
	ADC #$20                   ; Add 0x20 to account for the 'items' offset 6920
	TAX
	CMP #$0C                   ; then check for canal C90C
    BNE :+                     ; If canal then D005
	  LDA #$00
      STA unsram, X 
      JMP WasKeyItem
  : CMP #$36                   ; CMP ItemId with Tent
	BCC :+	
		LDA unsram, X
		CLC
		ADC shop_quantity	; add the shop item quantity
		CMP #$64
		BCC SpaceAvailable
		RTS		; if we would have 100 or more of the item, return with carry set to indicate we have too many
  : LDA #$01
    STA unsram, X
WasKeyItem:
	LDX #$FF
	LDA game_flags, X   ; get the game flags 
	ORA #GMFLG_EVENT    ; set the event bit
	STA game_flags, X   ; and write back
	CLC
	RTS	
SpaceAvailable:
	STA unsram, X	; otherwise, add the items to the player's inventory and return with carry not set to indicate success
	CLC
	RTS