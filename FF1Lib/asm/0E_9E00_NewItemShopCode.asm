;; 0E-9E00

; 9E00
PrintQuantity:	; used to display the quantity to buy
	LDA $030A	; we are storing the quantity in shop_charindex, which is otherwise unused in item shops
	STA $10		; printnumber_2digit uses tmp
	JMP $8E66	; jump to the routine which formats and prints two-digit numbers

; 9E08
NewCheckForSpace: ; we need a slightly more complex formula for 
	LDX $030C
	LDA $6020, X
	CLC
	ADC $030A					; add the shop item quantity
	CMP #$64
	BCC NewCheckForSpace2
	RTS		; if we would have 100 or more of the item, return with carry set to indicate we have too many
NewCheckForSpace2:
	STA $6020, X	; otherwise, add the items to the player's inventory and return with carry not set to indicate success
	RTS

; 9E1B
ShopBuyQuantity:
	LDA #$03
	JSR $AA3B	; draw the box that normally contains dialgoue, routine name is "DrawShopBox" in source code
	LDA #$01
	STA $030A			; store the quantity we are going to purchase in tmp+6
BuyQuantityInputLoop:
	LDA $20			; $20 is the joystick arrow buttons read
	AND #$0F		; we are interested in up/down/left/right
	STA $61			; $61 stores the joypad's previous direction
MoveDone:
	JSR $9F70	; Clamp quantity (below), then call the PrintQuantity subroutine we created above
	JSR $AA32	; and draw the complex string that is the quantity
Loop:
	JSR $A743		; call ShopFrameNoCursor
	LDA $25			; $25 is "joy_b", the state of the B button
	BNE B_Pressed
	LDA $24			; and $24 is the state of the A button
	BNE A_Pressed
	LDA $20
	AND #$0F
	CMP $61
	BEQ Loop
	STA $61
	CMP #$00
	BEQ Loop
	CMP #$04
	BEQ Down
	CMP #$08
	BEQ Up
	CMP #$01
	BEQ Right
Left:
	DEC $030A
	BNE MoveDone
	INC $030A
	BNE MoveDone	; this always branches
Right:
	INC $030A
	CMP #$64	; if quantity is 100, we must decrement back to 99
	BNE MoveDone
	DEC $030A
	BNE MoveDone	; this always branches
Up:
	LDA $030A
	CLC
	ADC #$0A	; add 10
Up2:
	STA $030A
	CMP #$64
	BCC MoveDone
	LDA #$63	; if reached maximum, set to 99
	BPL Up2		; make the comparison again, which will be less than 100 every time
Down:
	LDA $030A
	SEC
	SBC #$0A	; subtract 10
	BEQ Down2	; if zero, set to 1
	BPL Down3	; if positive, skip to end
Down2:
	LDA #$01	
Down3:
	STA $030A
	BPL MoveDone	; this always branches
B_Pressed:
	SEC		; cancel purchase and return to main shop loop
ButtonDone:           ; reached when the player has pressed B or A (exit this shop loop)
    LDA #$00
    STA $24            ; zero joy_a and joy_b so further buttons will be detected
    STA $25
    RTS
A_Pressed:            ; if A pressed...
    CLC
	BCC ButtonDone		; always branches

;; DisplayShopQuantity - Draws the confirmation dialogue and tallies the final cost.  large parts of this are copied from DrawShopConfirm in the original code, but it changes at the end
; 9E99
DisplayShopQuantity:
	LDA #$0E
    JSR $AA5B		; draws the dialogue box but no string
    LDA $62            ; get the cursor
    ASL A
    ASL A
    ASL A               ; multiply by 8
    CLC
    ADC #$16			; add str_buf+$16 (this points to the item ID)
	TAX					; transfer accumulator to X for indexing
    LDA $0300, X        ; this gets the item ID from the string
    STA $030C			; store A in shop_curitem ($030C)
	JSR $ECB9	; call LoadPrice subroutine in the fixed bank (0F in vanilla/1F in randomizer)
	LDX $030A			; here is where things are different.  we're going to loop for as many items as we have selected and add the price to the shop price
	LDA #$00
	STA $030B	; we will store the low byte of the price in $030B, normally used for spells in the spell shop
	STA $030E
	STA $030F
	CLC
Loop:
	LDA $030B
	ADC $10
	STA $030B
	LDA $030E
	ADC $11
	STA $030E
	LDA $030F
	ADC #$00	; item prices are never higher than 65535 so this will always be 0
	STA $030F
	BCS PriceOverflow		; if price exceeds 256^3, we handle price overflow, otherwise
	DEX
	BEQ BreakLoop	; end loop at X = 0
	BNE Loop		; otherwise go back to loop
PriceOverflow:
	LDA #$FF		; if carry was set on the third byte, we have overflowed the price - set price to the maximum
	STA $030B
	STA $030E
	STA $030F
BreakLoop:
	LDA $030F
	STA $12
	LDA $030E
	STA $11
	LDA $030B
	STA $10
	JSR $9F42	; format the 6-digit number
	JSR $AA32	; and draw the complex string in the box
	RTS

;; NewCanAfford - we have to make a slight alteration to Shop_CanAfford to support 3-byte prices
; 9EFF
NewCanAfford:
	LDA $601E
	CMP $030F
	BEQ CheckMid
	BCC No
	BCS Yes
CheckMid:
	LDA $601D
	CMP $030E
	BEQ CheckLow
	BCC No
	BCS Yes
CheckLow:
	LDA $601C
	CMP $030B
	BCS Yes
No:
	SEC
	RTS
Yes:
	CLC
	RTS

;; NewShopPayPrice - we have to support 3 byte prices now
; 9F23
NewShopPayPrice:
    LDA $601C
    SEC
    SBC $030B       ; subtract low byte
    STA $601C

    LDA $601D
    SBC $030E       ; mid byte
    STA $601D

    LDA $601E
    SBC $030F		; high byte
    STA $601E

    JMP $A7EF       ; then redraw the gold box to reflect changes, and return

;; PrintTotalCost - Prints the sum total
; 9F42
PrintTotalCost:
	JMP $8E8E	; this just jumps to PrintNumber_6Digit, which will RTS for us


;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;
PrintQuantity = $9E00
shop_items = $300
cursor = $62
char_items = $6020
temp   = $6AEF  ;used for in battle target list
.org $9F70
ClampQuantity:            ;checks which item we're trying to buy, and then limit
						  ;  the number to how many we can hold
    LDA cursor   ;get cursor
    ASL
    ASL
    ASL
    CLC
    ADC #$16
    TAY
    LDX shop_items,Y    ;get ID of selected shop item
    LDY char_items,X    ;Use that to index into inventory to get quantity of that item
    LDA $30A            ;load set quantity

    STY temp            ;can't Add/subtract from a register 
    CLC
    ADC temp            ;add quantity owned to quantity purchasing
    SBC #99
    BMI QuantityOk      ;branch if quantity is valid
    ;otherwise set quantity to max
    LDA #99
    SBC temp            ;99 - owned quantity
    BEQ CantHold      ;set quantity to 1 if we can't hold anymore
    BPL StoreNew      ;otherwise 99-owned is ok
    
CantHold:
    LDA #1     ;set quantity to 1 for message displaying purposes
StoreNew:
    STA $30A   ;buy quantity
QuantityOk:
    JMP PrintQuantity    ;this is where the code originally expected to go
;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;

; here is the rewritten ItemShop_Loop found in the old location, starting with ItemShop_Exit
; A471
ItemShop_Exit:
    RTS
ItemShop_CancelBuy:           ; jumped to for cancelled purchases
    LDA #$25
    JSR $AA5B     ; "too bad, something else?" dialogue
    JMP $A481           ; return to loop
EnterShop_Caravan:
    BNE EnterShop_Item          ; this always branches.. but even if it didn't....
                                ;  EnterShop_Item is the next instruction.  Pretty silly.
EnterShop_Item:
    LDA #$09
    JSR $AA5B     ; draw the "welcome" dialogue
; A481
ItemShop_Loop:
    JSR $A8B1       ; give them the option to buy or exit
    BCS ItemShop_Exit           ; if they pressed B, exit
    LDA $62						' $62 = cursor
    BNE ItemShop_Exit           ; otherwise if they selected 'exit', then exit

    LDA #$0D
    JSR $AA5B     ; "what would you like" dialogue
    JSR $A857      ; let them choose an item from the shop inventory
    BCS ItemShop_Loop           ; if they pressed B, restart the loop

    JSR $9E1B		; NEW - Select the quantity to purchase.  This ends by calling Yes/No or aborting if the total cost is above 65535, and the carry flag will be set to cancel the purchase
    BCS ItemShop_CancelBuy      ; if they pressed B, cancel the purchase
	JSR $9E99		; NEW - Draw the total price instead of the price of a single item
	JSR $A8C2		; Give the player the option to confirm/deny the purchase
	BCS ItemShop_CancelBuy
	LDA $62
	BNE ItemShop_CancelBuy
    JSR $9EFF       ; check to ensure they can afford this item, this is a new Shop_CanAfford to support 3-byte values
    BCC CheckForSpace          ; if they can, jump ahead to check to see if they have room for this item
      LDA #$10
      JSR $AA5B   ; if they can't, "you can't afford it" dialogue
      JMP $A481   ; and return to loop
 CheckForSpace:
    JSR $9E08		; jump to new subroutine, "NewCheckForSpace"
    BCC CompletePurchase       ; if yes, jump ahead to complete the purchase.  Otherwise...
      LDA #$0C
      JSR $AA5B   ; "you have too many" dialogue
      JMP $A481         ; return to loop
 CompletePurchase:
    JSR $9F23            ; subtract the price from your gold amount (this is a new JSR PayPrice that supports 3 digit costs)
    LDA #$13
    JSR $AA5B     ; "Thank you, anything else?" dialogue
    JMP $A481           ; and continue loop

; the remaining space is padded with NOPs
