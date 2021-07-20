.include "Constants.inc"
.include "variables.inc"

BANK_TALKROUTINE	= $11

FindEmptyArmorSlot = $DD46
FindEmptyWeaponSlot = $DD34
AddGPToParty = $DDEA
LoadPrice = $ECB9
lut_ConsStack = $B000
lut_TreasureJingle = $B200

LvlUp_AwardAndUpdateExp = $87DA
DivideRewardBySurvivors = $8B43
SwapPRG = $FE03

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
    CMP #$B0                   ; check if exp
    BCC :+                     ; Continue if gold 9009
     JSR LoadPrice             ; get the price of the item (the amount of gold in the chest) 20B9EC
     JSR AddExpToParty         ; add that exp to the party
     JMP @ClearChest           ; then mark the chest as
  : CMP #$6C                   ; check if gold C96C
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

AddExpToParty:
	LDA game_flags ; set bit 7 of gameflag 0 here
	ORA #$80
	STA game_flags

	LDA tmp
	STA battlereward
	LDA tmp+1
	STA battlereward+1
	
	LDA #$00				;Clear DrawFlags
	STA btl_drawflagsA
	STA btl_drawflagsB

	LDX #$C0				;Set DrawFlags for char 3
	JSR SetDrawFlags	
	
	LDX #$80				;Set DrawFlags for char 2
	JSR SetDrawFlags	
	
	LDX #$40				;Set DrawFlags for char 1
	JSR SetDrawFlags
	
	LDX #$00				;Set DrawFlags for char 0
	JSR SetDrawFlags
	
	JSR DivideRewardBySurvivors_L

	LDA battlereward
    STA eob_exp_reward
    LDA battlereward+1          ; store reward in eob_exp_reward
    STA eob_exp_reward+1

	LDA #$00                    ; award XP to all 4 party members
    JSR LvlUp_AwardAndUpdateExp_L
    LDA #$01
    JSR LvlUp_AwardAndUpdateExp_L
    LDA #$02
    JSR LvlUp_AwardAndUpdateExp_L
    LDA #$03
    JSR LvlUp_AwardAndUpdateExp_L

	LDA game_flags ; clear bit 7 of gameflag 0 here
	AND #$7F
	STA game_flags
	RTS

SetDrawFlags:
	LDA ch_ailments, X
	LSR A
	ROL btl_drawflagsA
	LSR A
	ROL btl_drawflagsB
	RTS

DivideRewardBySurvivors_L:
	LDA #>(ReturnToBank11-1)
	PHA
	LDA #<(ReturnToBank11-1)
	PHA
	LDA #>(DivideRewardBySurvivors-1)
	PHA
	LDA #<(DivideRewardBySurvivors-1)
	PHA
	LDA #$1B
	JMP SwapPRG
	
LvlUp_AwardAndUpdateExp_L:
	TAX
	LDA #>(ReturnToBank11-1)
	PHA
	LDA #<(ReturnToBank11-1)
	PHA
	LDA #>(LvlUp_AwardAndUpdateExp_L2-1)
	PHA
	LDA #<(LvlUp_AwardAndUpdateExp_L2-1)
	PHA
	LDA #$1B
	JMP SwapPRG
	
NOP
BRK
NOP
BRK
NOP

.org $DDD6
	
ReturnToBank11:
	LDA #$11
	JMP SwapPRG

LvlUp_AwardAndUpdateExp_L2:
	TXA
	JSR LvlUp_AwardAndUpdateExp
	RTS
	
