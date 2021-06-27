.include "Constants.inc"
.include "variables.inc"

SwapPRG = $FE03
CallMusicPlay = $C689
LvlUp_AwardAndUpdateExp = $87DA
DrawDialogueString = $DB64
DrawDialogueString_PtrLoaded = $DB8A


lut_itemDescriptions = $9300 ;bank 0E
lut_ShopInfoWordsPtrTbl = $9A00 ;bank 11
lut_ItemNamePtrTbl2 = $B700 - $2000 ;bank 0A mapped into the wrong half

text_buffer = $6B00 ; shares memory with btl_unfmtcbtbox_buffer

;.org $XXXX	
	PLA
	JSR DrawDialogueString
	JMP DrawDialogueItemInfo_L

NOP
NOP
NOP

.org $DDD0

DrawDialogueItemInfo_L:
	LDA #>(DrawDialogueItemInfo-1)
	PHA
	LDA #<(DrawDialogueItemInfo-1)
	PHA
	
ReturnToBank11:
	LDA #$11
	JMP SwapPRG

LvlUp_AwardAndUpdateExp_L2:
	TXA
	JSR LvlUp_AwardAndUpdateExp
	RTS

.org $B300

DrawDialogueItemInfo:
	LDA #$11
	STA cur_bank
	
	LDA tileprop+1		; prevents from overwriting NPC texts
	BNE :+
		RTS	
  : LDA dlg_itemid
	CMP #$1C
	BCS :+
		RTS
  : CMP #$6C	
	BCC :+
		RTS
  : PHA
	JSR CallMusicPlay    ; keep music playing
	
	LDA #$0
	STA dlg_itemid
	
	LDX box_y        	; get the screen scroll for Y
	INX
	TXA
	CMP #30             ; but wrap 29->0  (NTs are only 30 tiles tall)
	BCC :+
	  SBC #30
  : STA box_y           ; this is our target Y coord for the text
	
	LDA #$06			; map low half of 0E
	STA $8000 			; select R6
	LDA #$1C	
	STA $8001 			; map the low bank
	
	PLA					 ; restore dlg_itemid
    ASL                  ; double it (2 bytes per pointer)
    TAX                  ; and put back into X for indexing
    BCS HiTbl            ; if string ID was >= $80 use 2nd half of table, otherwise use first half
	
LoTbl:
	LDA lut_itemDescriptions, X     ; load up the pointer into text_ptr
	STA text_ptr
	LDA lut_itemDescriptions+1, X
	STA text_ptr+1
	JMP PtrLoaded                   ; then jump ahead
	
HiTbl:
	LDA lut_itemDescriptions+$100, X   ; same, but read from 2nd half of pointer table
	STA text_ptr
	LDA lut_itemDescriptions+$101, X
	STA text_ptr+1
	
PtrLoaded:
	LDA #$06			; map low half of 11
	STA $8000 			; select R6
	LDA #$22	
	STA $8001 			; map the low bank
	
	LDX #$0
	LDY #$03
	
PrepareString:
	LDA (text_ptr), Y
	INY
    CMP #$02
	BEQ Code02
    CMP #$14
	BEQ Code14
	STA text_buffer, X
	INX
	CMP #$00
	BNE PrepareString
	JMP TextPrepared
	
Code14:
	LDA text_ptr
	PHA
	LDA text_ptr+1
	PHA	
	TYA
	PHA	
	LDA (text_ptr), Y
	ASL
	TAY	
	
	BCS HiTbl2            ; if string ID was >= $80 use 2nd half of table, otherwise use first half
	
LoTbl2:
	LDA lut_ShopInfoWordsPtrTbl, Y     ; load up the pointer into text_ptr
	STA text_ptr
	LDA lut_ShopInfoWordsPtrTbl+1, Y
	STA text_ptr+1
	JMP Code02_14PtrLoaded                   ; then jump ahead
	
HiTbl2:
	LDA lut_ShopInfoWordsPtrTbl+$100, Y   ; same, but read from 2nd half of pointer table
	STA text_ptr
	LDA lut_ShopInfoWordsPtrTbl+$101, Y
	STA text_ptr+1
	
Code02_14PtrLoaded:
	LDY #$0
	
Code02_14Copy:
	LDA (text_ptr), Y
	INY
	CMP #$00
	BEQ :+
		STA text_buffer, X
		INX 
		JMP Code02_14Copy	
  : LDA #$06			; map low half of 11
	STA $8000 			; select R6
	LDA #$22	
	STA $8001 			; map the low bank
	
	PLA
	TAY
	INY
	PLA
	STA text_ptr+1
	PLA
	STA text_ptr
	JMP PrepareString
	
Code02:
	LDA text_ptr
	PHA
	LDA text_ptr+1
	PHA	
	TYA
	PHA	
	LDA (text_ptr), Y
	ASL
	TAY	
	
	LDA #$06			; map upper half of 0A
	STA $8000 			; select R6
	LDA #$15	
	STA $8001 			; map the low bank
	
	BCS HiTbl3            ; if string ID was >= $80 use 2nd half of table, otherwise use first half
	
LoTbl3:
	LDA lut_ItemNamePtrTbl2, Y     ; load up the pointer into text_ptr
	STA text_ptr
	LDA lut_ItemNamePtrTbl2+1, Y
	SEC
	SBC #$20
	STA text_ptr+1
	JMP Code02_14PtrLoaded                   ; then jump ahead
	
HiTbl3:
	LDA lut_ItemNamePtrTbl2+$100, Y   ; same, but read from 2nd half of pointer table
	STA text_ptr
	LDA lut_ItemNamePtrTbl2+$101, Y
	SEC
	SBC #$20
	STA text_ptr+1
	JMP Code02_14PtrLoaded                   ; then jump ahead
	
TextPrepared:
	LDA #<text_buffer
	STA text_ptr
	LDA #>text_buffer
	STA text_ptr+1

	JMP DrawDialogueString_PtrLoaded

	
	
	
	
	
	
	