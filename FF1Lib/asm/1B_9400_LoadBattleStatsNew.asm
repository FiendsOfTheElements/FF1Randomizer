.include "Constants.inc"
.include "variables.inc"

SwapPRG = $FE03

;currently in the 0C
.org $AD21
;put the destination address on the stack for after the bank swap
LDA #>(LoadIBStatsNew-1)
PHA
LDA #<(LoadIBStatsNew-1)
PHA
;load the new bank
LDA #$1B
JMP SwapPRG
LoadBattleStatsReturn:
NOP
;nop until the rest of the function is rewriten
;assembled bytes
;A9 94 48 A9 15 48 A9 1B 4C 03 FE EA EA EA EA EA EA EA EA EA EA EA EA EA EA EA EA EA EA EA EA EA EA EA EA EA EA EA EA EA EA EA EA EA EA EA EA EA EA EA EA EA EA EA

;bank 1b
.ORG $9400
lut_ClassInnateResist:
  .BYTE 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0

;moved this from bank 0C
;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;
;;
;;  TransferByte  [$ACE1 :: 0x32CF1]
;;
;;  Transfer a byte between two buffers (specifically for transferring OB char stats to IB stats)
;;
;;  input:
;;    btl_ib_charstat_ptr = pointer to dest buffer
;;    btl_ob_charstat_ptr = pointer to source buffer
;;                      A = dest index
;;                      Y = source index
;;1c
;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;
TransferByte:
    PHA                             ; backup A
    LDA (btl_ob_charstat_ptr), Y    ; Load the source stat
    TAX                             ; stick it in X
    PLA                             ; restore A, put in Y
    TAY
    TXA                             ; get the source stat
    STA (btl_ib_charstat_ptr), Y    ; write it to the dest
    RTS

LoadIBStatsNew:
    ;current unit pointer in btl_tmpindex
    ; Copy a bunch of stats verbatim from the OB stats to IB stats
    ;   Y = src index
    ;   A = dst index
    LDY #ch_class      - ch_stats
    LDA #btlch_class
    JSR TransferByte
    
    LDY #ch_ailments   - ch_stats
    LDA #btlch_ailments
    JSR TransferByte
    
    LDY #ch_curhp      - ch_stats
    LDA #btlch_hp
    JSR TransferByte
    
    LDY #ch_curhp + 1  - ch_stats
    LDA #btlch_hp + 1
    JSR TransferByte
    
    LDY #ch_hitrate    - ch_stats
    LDA #btlch_hitrate
    JSR TransferByte
    
    LDY #ch_magdef     - ch_stats
    LDA #btlch_magdef
    JSR TransferByte
    
    LDY #ch_evade      - ch_stats
    LDA #btlch_evade
    JSR TransferByte
    
    LDY #ch_absorb     - ch_stats
    LDA #btlch_absorb
    JSR TransferByte
    
    LDY #ch_dmg        - ch_stats
    LDA #btlch_dmg
    JSR TransferByte
    
    ;new elemental resist, with class based LUT
    LDY #ch_class - ch_stats
    LDA (btl_ob_charstat_ptr), Y    ; get char class
    TAX                             ; use as index to get innate resistance

    LDY #ch_resist     - ch_stats
    LDA (btl_ob_charstat_ptr), Y
    ORA lut_ClassInnateResist, X

    LDY #btlch_elemresist
    STA (btl_ib_charstat_ptr), Y

    ;put return address for after bank swap back
    LDA #>(LoadBattleStatsReturn-1)
    PHA
    LDA #<(LoadBattleStatsReturn-1)
    PHA

    ;go back to bank 0C
    LDA #$0C
	JMP SwapPRG

;00 00 00 00 00 00 00 00 00 00 00 00 48 B1 82 AA 68 A8 8A 91 80 60 A0 00 A9 01 20 0C 94 A0 01 A9 02 20 0C 94 A0 0A A9 03 20 0C 94 A0 0B A9 04 20 0C 94 A0 21 A9 05 20 0C 94 A0 25 A9 06 20 0C 94 A0 23 A9 07 20 0C 94 A0 22 A9 08 20 0C 94 A0 20 A9 09 20 0C 94 A0 00 B1 82 AA A0 24 B1 82 1D 00 94 A0 0A 91 80 A9 AD 48 A9 2B 48 A9 0C 4C 03 FE
