DrawItemDescBox = $B92B
mp_required = $65
ch_magicdata = $6300
DoOverworld = $C0CB
EnterMagicMenu = $AE97
MenuFrame = $B65D
joy_a = $24
joy_b = $25
PlaySFX_MenuSel = $AD84

MenuWaitForBtn_WithBackOption:
    JSR MenuFrame
    LDA joy_a                                 ; check if a or b was pressed, if a, goto Exit code, if b, return to menu
    BNE Exit
    LDA joy_b
    BEQ MenuWaitForBtn_WithBackOption         ; Jump back to start of routine if no button was pressed
    JSR PlaySFX_MenuSel                       ; Play sound effect like normal Exit usage
    PLA
    PLA
    LDA #0
    STA joy_b
    JMP EnterMagicMenu
  Exit:
    JSR PlaySFX_MenuSel                    ; Prepare to exit to the spell logic, mp is only used if it gets here
    LDA #0
    STA joy_a
    LDX mp_required
    DEC ch_magicdata,X
    RTS                                       ; RTS to the spell logic
