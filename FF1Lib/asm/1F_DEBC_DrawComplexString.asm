;
; Update: 2020-12-28
;
; Modify specific sections of DrawComplexString to use some unused control code
;  so we can use as second item table
;  this is only use by ShopInfo, but there's space for more item
;  if necessary

text_ptr        = $3E ; 2 bytes
cur_bank        = $57
ret_bank        = $58

Save = $E03E
Restore = $E04E
Draw_NoStall = $DE45
SwapPRG_L = $FE03
LineBreak = $DE83

lut_ShopInfoWordsPtrTbl = $9A00

BANK_SHOPINFO = $11

 .ORG $DEBC
Code05to19:
    CMP #$10           ; is control code >= $10?
    BCS Code10to19     ;  Branch if yes
                     
SingleLineBreak:       ; otherwise line break
    LDX #$20        
    JMP LineBreak   
                    
Code10to19:
    CMP #$14           ; is control code >= $14?
    BCS StatCodeJMP    ;  Branch to new stat code JMP

 .ORG $DF44
 
StatCodeJMP:
  JMP StatCode_14      ; Just jump, because code is too far for branching
 
 .ORG $DFCE
StatCode_14:   
  LDA #BANK_SHOPINFO   ; #$11
  STA cur_bank         ; Swap bank
  JSR SwapPRG_L
  
  JMP LoadInfoPointer  ;  Get the text pointer
  
 .ORG $99A0  ; Bank 11
  
LoadInfoPointer:
  LDA #BANK_SHOPINFO
  STA cur_bank
  STA ret_bank
    
  LDA (text_ptr), Y     ; get another byte from the string (this byte is the ID of the item string to draw)
  INC text_ptr          ; inc source pointer
  BNE DrawItem
  INC text_ptr+1        ;   and inc high byte if low byte wrapped

DrawItem:
  JSR Save                            ; drawing an item requires a substring.  Save current string
  ASL                                 ; double it (for pointer table lookup)
  TAX                                 ; put low byte in X for indexing
  BCS itemHigh                        ; if doubling A caused a carry (item ID >= $80)... jump ahead
    LDA lut_ShopInfoWordsPtrTbl, X    ;  if item ID was < $80... read pointer from first half of pointer table
    STA text_ptr                      ;  low byte of pointer
    LDA lut_ShopInfoWordsPtrTbl+1, X  ;  high byte of pointer (will be written after jump)
    JMP itemGo
itemHigh:                             ; item high -- if item ID was >= $80
  LDA lut_ShopInfoWordsPtrTbl+$100, X ;  load pointer from second half of pointer table
  STA text_ptr                        ;  write low byte of pointer
  LDA lut_ShopInfoWordsPtrTbl+$101, X ;  high byte (written next inst)
itemGo:
  STA text_ptr+1                      ; finally write high byte of pointer
  JSR Draw_NoStall                    ; recursively draw the substring
  LDA #$0E
  STA ret_bank
  JMP Restore                         ; then restore original string and continue
  
