;;;;;;;;;;;;;;;;;;;;;;;;;;;;
;;  Address: $DBF8
;;  In Draw Dialogue String @PrintName 
;;  overwrites unused code 3
;;  31 bytes of space available until @Code_Not03
;;;;;;;;;;;;;;;;;;;;;;;;;;;;
    LDA dlg_itemid      ; get the item ID whose name we're to print A561
    ADC #$20            ; Add 0x20 to account for the 'items' offset 6920
    BCC :+              ; if it is set 901D
      ASL A             ; 0A
      ADC #$5A          ; Add 0x5A 695A
      STA text_ptr      ; 853E
      LDA #$82          ; A982
      STA text_ptr+1    ; 853F
        CLC             ; 18
    JMP @Loop           ; and continue printing (to print the name, then quit) 4C9ADB