npcdir_seed         = $F4
btlsfx_backseat     = $6D97 ;16 bytes
NewClearZeroPage    = $A4F0
SwapPRG             = $FE03

;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;
; Bank 1F  [$C454 :: $7C464 rom location (includes header)]
; Patch to ClearZeroPage, jumps to new routine
;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;
.org $C454
JumpToClearZeroPage:
        LDA #>(NewClearZeroPage-1)
        PHA
        LDA #<(NewClearZeroPage-1)
        PHA
        LDA #$1C
        JMP SwapPRG
        NOP
        NOP
        NOP
        NOP
        NOP

;assembled bytes
;A9 A4 48 A9 EF 48 A9 1C 4C 03 FE EA EA EA EA EA


;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;
; Bank 1C  [$A4F0 :: $72500 rom location (includes header)]
; Replacement ClearZeroPage routine
; Zeroes additional locations that the game reads from
;   before initializing
;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;
.org $A4F0
NewClearZeroPage:
        ;copied from original ClearZeroPage routine
        LDX #$EF          ; start from $EF and count down
        LDA #0
    @Loop:
        STA 0,X
        DEX
        BNE @Loop       ; clear RAM from $01-EF

        LDA #$1B          ; scramble the NPC directional RNG seed
        ORA npcdir_seed   ;  to make it a little more random
        STA npcdir_seed

        ;new code
        LDX #$0F        ; clear 16 bytes starting at btlsfx_backseat
        LDA #0          ;   to fix noise channel battle glitch
    @Loop2:
        STA btlsfx_backseat,X
        DEX
        BPL @Loop2

    @Exit              ;ClearZeroPage was JSRed into so return addr is already on stack
        LDA #$1F
        JMP SwapPRG

;assembled bytes
;A2EFA9009500CAD0FBA91B05F485F4A20FA9009D976DCA10FAA91F4C03FE