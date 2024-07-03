SwapPRG = $FE03
MagicMenu_SwapPatched = $A600
;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;
; overwriting unused space with a jump to MagicMenu_SwapPatched
;  bank 0E
;  [$BFF5 :: 0x3C005 (includes header)]
;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;
.ORG $BFF5
    LDA #>(MagicMenu_SwapPatched-1)
    PHA
    LDA #<(MagicMenu_SwapPatched-1)
    PHA
    LDA #$1C
    JMP SwapPRG
;assembled bytes
;A9 A5 48 A9 FF 48 A9 1C 4C 03 FE


;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;
; patch to EnterMagicMenu in bank 0E
; [$AECD :: 0x3AEDD (includes header)]
;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;
.org $AECD
    JMP $BFF5
    NOP

;assembled bytes
;4C F5 BF EA


;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;
; Patched Magic Menu Routine
; If START is pressed, swap the spell pointed to by the cursor with
;   the one next to it.  Otherwise, return to UpgradedMagicMenu
;   defined in 0E_9500_ShopUpgrade.asm
;   (the above code replaces the normal jump to UpgradedMagicMenu)
; [$A600 :: 0x72610 (includes header)]
;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;

joy_start           = $23
cursor              = $62 ;spell we're pointing at
current_character   = $66
ch_magicdata        = $6300 ;add $40 per character

SwapPRG = $FE03
PatchToEnableSpellInfo = $9126 ;return here if we changed nothing
EnterMagicMenu = $AE97 ;return here if anything changed

.org $A600 ;rom location $72610
    MagicMenu_SwapPatched:
        LDA joy_start ;have we pressed start
        BEQ return_to_bank_e ;if not, continue the menu loop

        LDA #0
        STA joy_start ;zero out start pressed so we don't loop back here

        LDA cursor
        AND $02 ;check if we're in one of the first 2 slots
        BNE return_to_bank_e ;don't swap if we are

        LDA current_character
        ROL
        ROL
        ROL
        ROL
        ROL
        ROL ;get character offset from ch_magicdata

        ADC cursor ;get selected spell's offset from magic data
        TAX
        LDA ch_magicdata,X ;using the offset we calculated, get the actual spell
        BEQ return_to_bank_e ;cancel if spell slot is empty
        TAY ;save spell for later

        INX
        LDA ch_magicdata,X ;get next spell
        BEQ return_to_bank_e ;cancel if spell slot is empty
        DEX
        STA ch_magicdata,X ;save second spell in first slot

        TYA ;get first spell again
        INX ;get back to second spell offset
        STA ch_magicdata,X ;complete swap

        LDA #0
        STA joy_start ;clear start button pressed

    return_to_redraw_menu:
        LDA #>(EnterMagicMenu-1)
        PHA
        LDA #<(EnterMagicMenu-1)
        PHA
        LDA #$0E
        JMP SwapPRG
        

    return_to_bank_e:
        LDA #>(PatchToEnableSpellInfo-1)
        PHA
        LDA #<(PatchToEnableSpellInfo-1)
        PHA
        LDA #$0E
        JMP SwapPRG

;assembled bytes
;A600: A5 23 F0 39 A9 00 85 23
;A608: A5 62 25 02 D0 2F A5 66
;A610: 2A 2A 2A 2A 2A 2A 65 62
;A618: AA BD 00 63 F0 1F A8 E8
;A620: BD 00 63 F0 18 CA 9D 00
;A628: 63 98 E8 9D 00 63 A9 00
;A630: 85 23 A9 AE 48 A9 96 48
;A638: A9 0E 4C 03 FE A9 91 48
;A640: A9 25 48 A9 0E 4C 03 FE

