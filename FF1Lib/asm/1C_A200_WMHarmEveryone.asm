;old length = 21 bytes
.include "Constants.inc"
.include "variables.inc"

SwapPRG = $FE03

BtlMag_MarkSpellConnected = $B885
BtlMag_LoadBaseHitChance = $B873
PutEffectivityInDamageMathBuf = $B8F9
BtlMag_PrepHitAndDamage = $B82D
BtlMag_ApplyDamage = $B8DB

;modified bank 0C function
;offset 33915
.org $B905
BtlMag_Effect_DamageUndead:
    JSR BtlMag_LoadBaseHitChance        ; Load base hit/crit chance
    JSR PutEffectivityInDamageMathBuf   ; load up base damage
    JSR BtlMag_PrepHitAndDamage         ; crit rate, randomize damage

    ;put the destination address on the stack for after the bank swap
    LDA #>(HarmHurtWmOnly-1) ;2
    PHA ;1
    LDA #<(HarmHurtWmOnly-1) ;2
    PHA ;1
    ;load the new bank
    LDA #$1C ;2
    JMP SwapPRG ;3

    ;nop out the rest of the function
    NOP 
    NOP 
    NOP
    NOP
    NOP
    NOP
    NOP
    NOP
    NOP
    DamageUndeadZeroRTS:
    RTS
      
    ; Otherwise, if defender is undead..
    HarmHurtWmOnlyReturn:
    JSR BtlMag_MarkSpellConnected       ; Mark the spell as connected
    JMP BtlMag_ApplyDamage              ; And do the actual damage!

;assembled bytes
;20 73 B8 20 F9 B8 20 2D B8 A9 A2 48 A9 03 48 A9 1C 4C 03 FE EA EA EA EA EA EA EA EA EA 60

.ORG $A200
lut_CharClassPtrTable:
  .byte $00
  .byte $40
  .byte $80
  .byte $C0

HarmHurtWmOnly:   
    LDA btlmag_defender_category
    AND #CATEGORY_UNDEAD
    BNE UNDEAD
      ;zero hit chance
      LDA #0
      STA math_hitchance
      STA math_hitchance+1

      ;continue on to actually do the halved damage only if we're a white mage/wizard, otherwise zero it out
      LDA btl_attacker
      AND #$7F
      TAX
      LDA lut_CharClassPtrTable,X
      TAY
      LDA ch_class, Y
      CMP #CLS_WM
      BEQ UNDEAD
      CMP #CLS_WW
      BEQ UNDEAD

      ;otherwise mark spell as a dud
      LDA #0
      STA math_basedamage
      STA math_basedamage+1
      
      ;put return address for after bank swap back
      LDA #>(DamageUndeadZeroRTS-1)
      PHA
      LDA #<(DamageUndeadZeroRTS-1)
      PHA

      ;go back to bank 0C
      LDA #$0C
      JMP SwapPRG

    UNDEAD:
      ;put return address for after bank swap back
      LDA #>(HarmHurtWmOnlyReturn-1)
      PHA
      LDA #<(HarmHurtWmOnlyReturn-1)
      PHA

      ;go back to bank 0C
      LDA #$0C
      JMP SwapPRG

;assembled bytes @ bank 1C A200
;00 40 80 C0 AD 86 68 29 08 D0 30 A9 00 8D 56 68 8D 57 68 AD 89 6C 29 7F AA BD 00 A2 A8 B9 00 61 C9 04 F0 17 C9 0A F0 13 A9 00 8D 58 68 8D 59 68 A9 B9 48 A9 21 48 A9 0C 4C 03 FE A9 B9 48 A9 22 48 A9 0C 4C 03 FE
