.include "Constants.inc"
.include "variables.inc"

BANK_TALKROUTINE	= $11

FindEmptyArmorSlot = $DD46
FindEmptyWeaponSlot = $DD34
AddGPToParty = $DDEA
LoadPrice = $ECB9
lut_ConsStack = $B000
lut_TreasureJingle = $B200

DrawDialogueBox = $D4B1
DialogueBox_Sfx = $D6C7
DialogueBox_Frame = $D6A1
UpdateJoy = $D7C2


DLGID_TCGET_GOLD_OR_EXP = $FD
DLGID_TCGET_EXP = $FE
DLGID_TCGET_GOLD = $FF

.org $B010

;; Bank 11 $B010
;; New jump point for NPC-only items:
;; IN:
;;       'A' should be set to the item ID, 224-255 are special values for variables
;; Result:
;;       'C' set if can't carry any more, otherwise clear
;;       'X' Dialog ID
;;       'A' Also Dialog ID
GiveReward:                    ; (8 bytes)
    LDY #BANK_TALKROUTINE      ; Get return bank AC 11 00
    STY ret_bank               ; for LoadPrice 8C 58 00
    STA dlg_itemid             ; record that as the item id so it can be printed in the dialogue box 8561
    ADC #$20                   ; Add 0x20 to account for the 'items' offset 6920
    CMP #$3C                   ; see if the ID is >= item_stop C93C
    BCS @NotItem               ; B028
  @Item:                       ; (5 + 15 + 7 bytes)
    TAX                        ; put item ID in X AA
    CMP #$0C                   ; then check for canal C90C
    BNE :+                     ; If canal then D005
      DEC unsram, X            ; decrement DE0060
      BCS @WasCanal            ; and open it B018
:   CMP #$36                   ; CMP ItemId with Tent C936
    BCC :+                     ; < Tent normal Add One routine 9011
		SBC #$36               ; Subtract TentOffset E936
		TAY                    ; Punt Index into Y A8
        LDA unsram, X          ; load inventory count BD0060
        CMP #$63               ; Compare with 99 C963
        BCS @TooFull           ; don't pick it up if consumable >= 99 B03D
        ADC lut_ConsStack, Y   ; add ConsumableStackSize (actual StackSize minus 1)	 7900B0	
        STA unsram, X          ; MadMartin: store into inventory 9D0060
		TXA					   ; Restore ItemId into A 8A
:	INC unsram, X              ; otherwise give them one of this item FE0060
@WasCanal:
    CMP #$31                   ; if >= item_canoe (shard) then play regular jingle C931
    BCS @ClearChest            ; B02A
    BCC @OpenChest             ; 902B
  @NotItem:                    ; (6 + 9 + 4 + 9 + 7 + 5 = 40 bytes)
    LDA dlg_itemid             ; restore item id A561
    CMP #$6C                   ; check if gold C96C
    BCC :+                     ; Continue if gold 9009
     JSR LoadPrice             ; get the price of the item (the amount of gold in the chest) 20B9EC
     JSR AddGPToParty          ; add that price to the party's GP 20EADD
     JMP @ClearChest           ; then mark the chest as open, and exit 4C63B0
:   CMP #$44                   ; >= 68 means it's armor C944
    BCS :+                     ; B009
      JSR FindEmptyWeaponSlot  ; Find an available slot to place this weapon in 2034DD
      BCS @TooFull             ; if there are no available slots, jump to 'Too Full' message B007
      LDA #$E5                 ; convert to index where 1 is first weapon A9E5
      BCC @EquipmentGet        ; 9007
:   JSR FindEmptyArmorSlot     ; Find an empty slot to put this armor 2046DD
    BCS @TooFull               ;  if there are no available slots, jump to 'Too Full' message B00C
    LDA #$BD                   ; convert to index where 1 is first weapon/armor A9BD
  @EquipmentGet:               ; 'A' should hold the equipment ID and 'X' the item slot
    ADC dlg_itemid             ; 6561
    STA ch_stats, X            ; add it to the previously found empty slot 9D0061
  @ClearChest:                 ; Cleanup, set jingle and dialog id (12 bytes)
    CLC                        ; 18
  @OpenRegularChest:           ;  then continue on to mark the chest as open
    LDX tileprop+1             ; look at the chest id, key items and npcs will skip this check anyway
    LDA lut_TreasureJingle, X  ; look at the list of chests that were marked as incentive holding by the randomizer
    BNE @OpenChest             ; if it was > 0
        INC dlgsfx             ; set dlgsfx to play the TC jingle E67D
  @OpenChest:                  ;
    INC dlgsfx                 ; set dlgsfx to play the TC jingle E67D
  @TooFull:                    ; jump here with C set to show "Can't Hold" text and no jingle
    LDX #DLGID_TCGET           ; and select "In This chest you found..." text A2F0
    BCC :+                     ; 9004
      INC $60B9                ; EEB760
      INX                      ; E8
:   TXA                        ; 8A
    RTS                        ; 60
	
	
NOP
BRK
NOP
BRK
NOP
	
.org $B0A0

	LDA #DLGID_TCGET_GOLD_OR_EXP
	JSR DrawDialogueBox
	JSR ShowDialogueBox_AB
	BCS TakeGold
TakeExp:	
	JSR AddExpToParty
	CLC
	INC dlgsfx
	INC dlgsfx
	LDA #DLGID_TCGET_EXP
	RTS
TakeGold:
    JSR AddGPToParty          ; add that price to the party's GP 20EADD	 
	CLC
	INC dlgsfx
	INC dlgsfx
	LDA #DLGID_TCGET_GOLD
	RTS

AddExpToParty:

	RTS

;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;
;;
;;  Show Dialogue Box [$D602 :: 0x3D612]
;;
;;    This makes the dialogue box and contained text visible (but doesn't draw it to NT,
;;  that must've already been done -- see DrawDialogueBox).  Once the box is fully visible,
;;  it plays any special TC sound effect or fanfare music associated with the box and waits
;;  for player input to close the box -- and returns once the box is no longer visible.
;;
;;  IN:  dlgsfx = 0 if no special sound effect needed.  1 if special fanfare, else do treasure chest ditty.
;;
;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;

ShowDialogueBox_AB:
	LDA #0
    STA joy_a              ; clear A button marker
    STA joy_b              ; clear B button marker

    LDA #3
    STA tmp+2              ; reset the 3-step counter for WaitScanline

	LDA #53
    STA sq2_sfx            ; indicate sq2 is going to be playing a sound effect for the next 53 frames
    LDA #$8E
    JSR DialogueBox_Sfx    ; and play the "upward sweep" sound effect that plays when the dialogue box opened.

    LDA soft2000           ; get the onscreen NT
    EOR #$01               ; toggle the NT bit to make it the offscreen NT (where the dialogue box is drawn)
    STA tmp+10             ; store "offscreen" NT in tmp+10

    LDA #$08               ; start the visibility scanline at 8(+8).  This means the first scanline of the box
    STA tmp+11             ;  that's visible will be on scanline 16 -- which is the start of where the box is drawn

OpenLoop:
    JSR DialogueBox_Frame; do a frame

    LDA tmp+11
    CLC
    ADC #2
    STA tmp+11           ; increment the visible scanlines by 2 (box grows 2 pixels/frame)

    CMP #$60             ; see if visiblity lines >= $60 (bottom row of dialogue box)
    BCC OpenLoop        ; keep looping until the entire box is visible

WaitForButton:           	
    JSR DialogueBox_Frame   ; Do a frame
    JSR UpdateJoy           ; update joypad data
    LDA joy_a               ; check A button
	SEC
    BNE ExitDialogue      	; and exit if A pressed
    LDA joy_b               ; check B button
	CLC						;
    BEQ WaitForButton       ; and exit if A pressed
	
ExitDialogue:
	PHP

    LDA #37
    STA sq2_sfx            ; indicate that sq2 is to be playing a sfx for the next 37 frames
    LDA #$95
    JSR DialogueBox_Sfx    ; and start the downward sweep sound effect you hear when you close the dialogue box
	
CloseLoop:
    JSR DialogueBox_Frame; do a frame

    LDA tmp+11        		; subtract 3 from the dialogue visibility scanline (move it 3 lines up
    SEC               ;    retracting box visibility)
    SBC #3
    STA tmp+11        ; box closes 3 pixels/frame.

    CMP #$12          ; and keep looping until line is below $12
    BCS CloseLoop
	
    LDA #0
    STA joy_a              ; clear A button marker
    STA joy_b              ; clear B button marker
	
	PLP
    RTS          			; then the dialogue box is done!





