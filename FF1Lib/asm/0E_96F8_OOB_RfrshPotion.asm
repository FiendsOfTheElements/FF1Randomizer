.include "Constants.inc"
.include "variables.inc"

PlaySFX_Error = $DB26
EnterItemMenu = $B11D
MenuWaitForBtn_SFX = $B613
ItemTargetMenuLoop = $B3A0
DrawItemDescBox = $B92B
DrawItemTargetMenu = $B400
MenuRecoverHP_Abs = $B561
MenuWaitForBtn = $B625
ClearOAM = $C43C

item_RfrshP = item_stop

.org $96F8

UseItem_RfrshP:
    JSR DrawItemTargetMenu     ; 2000B4 Draw the item target menu (need to know who to use this heal potion on)
    LDA #$08					;A908
    JSR DrawItemDescBox        ; 202BB9 open up the description box with text ID $20
    JSR ClearOAM               ; clear OAM (no sprites)

_UseItem_RfrshP_Loop:
    JSR MenuWaitForBtn          ; wait for the user to press a button

    LDA joy                     ; see whether the user pressed A or B
    AND #$80                    ; check A
    BEQ UseItem_Exit            ; if not A, they pressed B... so exit

    LDX #$00

UseItem_RfrshP_ChrLoop:
    LDA ch_ailments, X         ; BD0161 check their OB ailments
    CMP #$01					; C901
    BEQ UseItem_RfrshP_ChrSkip  ; F01C if dead... can't use
    CMP #$02					; C902
    BEQ UseItem_RfrshP_ChrSkip  ; F018 if stone... can't use

	LDA #64                    
    JSR MenuRecoverHP_Abs      ;   recover 64 HP for target (index is still in X).  Can use _Abs version

UseItem_RfrshP_ChrSkip:
    TXA
    CLC
    ADC #$40
    TAX
    BCC UseItem_RfrshP_ChrLoop

    JSR DrawItemTargetMenu     ; 2000B4  because we already checked the ailments
    JSR MenuWaitForBtn_SFX     ; 2013B6 then redraw the menu to reflect the HP change, and wait for the user to press a button

    DEC item_RfrshP             ; CE3C60 then remove a RfrshP potion from the inventory

UseItem_Exit:
    JMP EnterItemMenu          ; 4C1DB1 re-enter item menu (item menu needs to be redrawn)
