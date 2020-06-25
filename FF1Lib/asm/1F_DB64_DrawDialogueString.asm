;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;
;;
;;  DrawDialogueString  [$DB64 :: 0x7DB74]
;;
;;  This modify DrawDialogueString so we can move all the dialogues to their own bank (Bank 10), leaving items name in bank 0A,  
;;   and to free some dialogue IDs (all tile dialogues are moved to their own pointer table).
;;
;;  This replace the modifications in 1F_DBF8_ControlCode3.asm

tmp = $10
dlg_itemid = $61
text_ptr = $3E
box_x = $38
box_y = $39
dest_x = $3A
dest_y = $3B
lut_ItemNamePtrTbl = $B700
lut_DialoguePtrTbl = $8000
;; Tile pointer table overlaps normal dialogue table since most
;; of the tile dialogue are at the end of the table in the original game
;; this saves us from modifying talk routines in bank 1F
lut_TileDialoguePtrTbl = $80A0
lut_DTE2 = $F050
lut_DTE1 = $F0A0
BANK_ITEMS = $0A
BANK_DIALOGUE = $10
cur_bank = $57
SwapPRG_L = $FE03
WaitForVBlank_L = $FE00
SetPPUAddrToDest = $DC80
SetSMScroll = $CCA1
CallMusicPlay = $C669
GetPointer = $FC07
talkobj = $67
 
 .ORG $DB60
DrawDialogueString_Done:

 .ORG $DB64

DrawDialogueString:
  TAX                   ; put string ID in X temporarily
  
  LDA #BANK_DIALOGUE
  STA cur_bank          ; set cur_bank to bank containing dialogue text (for Music_Play)
  JSR SwapPRG_L         ; and swap to that bank

  LDA #>lut_DialoguePtrTbl     ; Load high byte of pointer table into A
  STA $97                      ; Store it at $97
  LDA talkobj                      ; Check if it's a tile dialog
  CMP #$F0                     ; CanTalkToMapObject return F0 in talkobj if not object was found
  BNE NotTile                  ; If it is...
    TXA                              ; Put X into A -  string ID
    LDX #<lut_TileDialoguePtrTbl     ; Load low byte of ptr from tile dialogue into X
    JMP LutSelected
NotTile:
    TXA                          ; Put X into A -  string ID
    LDX #<lut_DialoguePtrTbl     ; Load low byte of ptr from normal dialogue into X
LutSelected:      
  JSR GetPointer               ; Routine used for battle text to get the right text pointer
  LDA $94                      ; we use it here to save space
  STA text_ptr
  LDA $95
  STA text_ptr+1               ; Text pointer loaded
    
  LDA #10
  STA tmp+7             ;  set precautionary counter to 10

  JSR WaitForVBlank_L   ; wait for VBlank

  LDA box_x             ; copy placement coords (box_*) to dest coords (dest_*)
  STA dest_x
  LDA box_y
  STA dest_y
    
  JSR SetPPUAddrToDest  ; then set the PPU address appropriately

Loop:
  LDY #0                       ; zero Y for indexing
  LDA (text_ptr), Y            ; get the next byte in the string
  BEQ DrawDialogueString_Done  ; if it's zero (null terminator), exit

  INC text_ptr                 ; otherwise increment the pointer
  BNE NoWrap
    INC text_ptr+1             ;   inc high byte if low byte wrapped
NoWrap:
  CMP #$1A
  BCC ControlCode     ; if the byte is < $1A, it's a control code

  CMP #$7A
  BCC DTE             ; if $1A-$79, it's a DTE code

SingleTile:
  STA $2007            ; otherwise ($7A-$FF), it's a normal single tile.  Draw it

  LDA dest_x           ; increment the dest address by 1
  CLC
  ADC #1
  AND #$3F             ; and mask it with $3F so it wraps around both NTs appropriately
  STA dest_x           ; then write back

  AND #$1F                ; then mask with $1F.  If result is zero, it means we're crossing an NT boundary
  BNE Loop                ;  if not zero, just continue looping
    JSR SetPPUAddrToDest  ;  otherwise if zero, PPU address needs to be reset (NT boundary crossed)
    JMP Loop              ;  then jump back to loop

DTE:                 ; if byte fetched was a DTE code ($1A-79)
  SEC
  SBC #$1A           ; subtract $1A to make the DTE code zero based
  TAX                ; put in X for indexing
  PHA                ; and push it to back it up (will need it again later)

  LDA lut_DTE1, X    ; get the first byte in the DTE pair
  STA $2007          ; and draw it
  JSR IncDest       ; update PPU dest address

  PLA                ; restore DTE code
  TAX                ; and put it in X again (X was corrupted by @IncDest call)
  LDA lut_DTE2, X    ; get second byte in DTE pair
  STA $2007          ; draw it
  JSR IncDest       ; and update PPU address again

  DEC tmp+7            ; decrement cautionary counter
  BNE Loop            ; if it hasn't expired yet, keep drawing.  Otherwise...
    JSR SetSMScroll      ; we could be running out of VBlank time.  So set the scroll
    JSR CallMusicPlay    ; keep music playing
    JSR WaitForVBlank_L  ; then wait another frame before continuing drawing

    LDA #10
    STA tmp+7            ; reload precautionary counter
    JSR SetPPUAddrToDest ; and set PPU address appropriately
    JMP Loop            ; then resume drawing

ControlCode:          ; if the byte fetched was a control code ($01-19)
  CMP #$03             ; was the code $03?
  BNE Code_Not03      ; if not jump ahead

PrintItemName:        ; Control Code $03 = prints the ID of the item stored in dlg_itemid (used for treasure chests)
  LDA text_ptr         ; push the text pointer to the stack to back it up
  PHA
  LDA text_ptr+1
  PHA

  LDA #BANK_ITEMS     ; Swap bank to items bank
  STA cur_bank
  JSR SwapPRG_L 

  LDA dlg_itemid      ; Load item ID
  ADC #$20           
  BCC NormalItem      ; Check if it's SHIP, CANOE, CANAL or BRIDGE
    ASL               ; If it is we calculate pointer directly from ID
    ADC #$D0
    STA text_ptr
    LDA #$B5
    STA text_ptr+1
    CLC
    JMP ItemPtrLoaded 
NormalItem:                   ; If it's a normal item, we
  LDA #>lut_ItemNamePtrTbl    ;  use the same routine to get its pointer
  STA $97                      
  LDA dlg_itemid              
  LDX #<lut_ItemNamePtrTbl    
  JSR GetPointer              
  LDA $94                      
  STA text_ptr
  LDA $95
  STA text_ptr+1
ItemPtrLoaded:
  JSR Loop             ; once pointer is loaded, JSR to the @Loop to draw the item name

  LDA #BANK_DIALOGUE   ; Restore Dialogs bank when we're done drawing the item name
  STA cur_bank
  JSR SwapPRG_L 
    
  PLA                  ; then restore the original string pointer by pulling it from the stack
  STA text_ptr+1
  PLA
  STA text_ptr

  JMP Loop             ; and continue drawing the rest of the string

 .ORG $DC48
Code_Not03:
  JSR LineBreak         ; just do a line break
  JMP Loop              ; then continue
  
 .ORG $DC4E
IncDest:

 .ORG $DC5F
LineBreak:
