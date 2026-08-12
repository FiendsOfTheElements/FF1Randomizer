BANK_THIS = $0E
BANK_NEWMENUS = $12
lut_MenuText = $8500

DrawComplexString = $DE36
text_ptr = $3E
cur_bank = $57
ret_bank = $58

.ORG $8600
NewDrawMenuString:
    ASL A                   ; double A (pointers are 2 bytes)
    TAX                     ; put in X to index menu string pointer table
    LDA lut_MenuText,X
    STA text_ptr
    LDA lut_MenuText+1,X   ; load pointer from table, store to text_ptr  (source pointer for DrawComplexString)
    STA text_ptr+1
    LDA #BANK_NEWMENUS
    STA cur_bank
    LDA #BANK_THIS
    STA ret_bank
    JMP DrawComplexString


.ORG $B938
JMP NewDrawMenuString