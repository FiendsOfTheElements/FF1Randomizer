;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;
;;
;;  BtlMag_Effect_AbsorbUp  [$B9E4 :: 0x339F4]
;;
;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;

[$B9E4 :: 0x339F4]
BankSwap:
    LDA #$A7                        ; High byte of Relocated_AbsorbUp
    PHA
    LDA #$B7                        ; Low byte of Relocated_AbsorbUp
    PHA
    LDA #$1C                        ; Load new bank
    JMP SwapPRG
    JMP BtlMag_MarkSpellConnected   ; This spell always connects.
    NOP
    NOP

A9A748A9B748A91C4C03FE4C85B8EAEA

;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;
;;
;;  Relocated_AbsorbUp  [$A7B8 :: 0x727B8]
;;
;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;

[0xA7B8 :: 0x727B8]
LDA #$08                            ; Fog1 effectivity
CMP btlmag_effectivity              ; compares current spell effectivity to fog1 effectivity
BNE Fog2AbsorbUp                   ; branches to fog2 if not 8
FogAbsorbUp:
    LDA btlmag_defender_absorb      ; get defender absorb
    ADC #$XX                        ; add value to it
    BCC :+
      LDA #$FF                      ; (cap at 255)
  : STA btlmag_defender_absorb      ; that's our new absorb!

FogMDefUp:
    LDA btlmag_defender_magdef      ; get defender magdef
    ADC #$XX                        ; add value to it
    BCC :+
      LDA #$FF                      ; (cap at 255)
  : STA btlmag_defender_magdef      ; that's our new MDef!

    JMP AbsorbBankSwap             ; skips over Fog2

Fog2AbsorbUp:
    LDA btlmag_defender_absorb      ; get defender absorb
    ADC #$XX                        ; add value to it
    BCC :+
      LDA #$FF                      ; (cap at 255)
  : STA btlmag_defender_absorb      ; that's our new absorb!

Fog2MDefUp:
    LDA btlmag_defender_magdef      ; get defender magdef
    ADC #$XX                        ; add value to it
    BCC :+
      LDA #$FF                      ; (cap at 255)
  : STA btlmag_defender_magdef      ; that's our new MDef!

AbsorbBankSwap:
    LDA #$B9                        ; Low byte of Original_AbsorbUp
    PHA
    LDA #$EE                        ; High byte of Original_AbsorbUp
    PHA
    LDA #$0C                        ; Load new bank
    JMP SwapPRG

A908CD7468D01BAD7F6869XX9002A9FF8D7F68AD726869XX9002A9FF8D72684CF2A7AD7F6869XX9002A9FF8D7F68AD726869XX9002A9FF8D7268A9B948A9EE48A90C4C03FE