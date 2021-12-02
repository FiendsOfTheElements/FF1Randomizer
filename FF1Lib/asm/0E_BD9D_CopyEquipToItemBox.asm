.include "Constants.inc"
.include "variables.inc"

ap_weapon_idx = $6007
ap_armor_idx = $600B
ap_item_box_idx = $FC

;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;
;;
;;  Copy Equipment to Item Box  [$BD9D :: 0x3BDAD]
;;
;;    Copies equipment from the player inventory over to
;;  the item box buffer.
;;
;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;

.org $BD9D

CopyEquipToItemBox:    
	LDX equipoffset       ; X is our source index -- start at the equipment offset
    LDY #0                ; Y is our dest index -- start at zero
@Loop1:
	LDA ch_stats, X
    STA item_box, Y
	INX
	INY
	TXA
	AND #$03
	BNE @Loop1
	TXA
	CLC
	ADC #$3C
	TAX
	BCC @Loop1
	CMP #$18
	BEQ :+
		LDA ap_armor_idx
		STA ap_item_box_idx
		RTS
  : LDA ap_weapon_idx
	STA ap_item_box_idx 	
    RTS
	
	
BRK
NOP
BRK
NOP
BRK
NOP	
BRK

;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;
;;
;;  Copy Equipment from Item Box  [$BDC8 :: 0x3BDD8]
;;
;;    Copies equipment from the item box buffer back to player inventory
;;
;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;

.org $BDC8

CopyEquipFromItemBox:    ; this routine is exactly the same as the above
	LDX equipoffset       ; X is our source index -- start at the equipment offset
    LDY #0                ; Y is our dest index -- start at zero
@Loop2:
    LDA item_box, Y
    STA ch_stats, X
	INX
	INY
	TXA
	AND #$03
	BNE @Loop2
	TXA
	CLC
	ADC #$3C
	TAX
	BCC @Loop2
	CMP #$18
	BEQ :+
		LDA ap_item_box_idx
		STA ap_armor_idx
		RTS
  : LDA ap_item_box_idx
	STA ap_weapon_idx	
    RTS
	
	
	
	
	
	