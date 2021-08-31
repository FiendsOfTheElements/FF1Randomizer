.include "Constants.inc"
.include "variables.inc"

PlaySFX_Error = $DB26
EnterItemMenu = $B11D
MenuWaitForBtn_SFX = $B613
ItemTargetMenuLoop = $B3A0
DrawItemDescBox = $B92B
DrawItemTargetMenu = $B400
MenuRecoverHP_Abs = $B561

item_highp = item_stop + 1

.org $9748

UseItem_HighP:
    JSR DrawItemTargetMenu     ; 2000B4 Draw the item target menu (need to know who to use this heal potion on)
    LDA #$08					;A908
    JSR DrawItemDescBox        ; 202BB9 open up the description box with text ID $20

  _UseItem_HighP_Loop:
    JSR ItemTargetMenuLoop     ; 20A0B3 run the item target loop.
    BCS UseItem_Exit           ; B028 if B was pressed (C set), exit this menu(40byte)

    LDA cursor                 ; A562 otherwise... A was pressed.
    ROR A                      ; 6A  get the cursor (target character)
    ROR A                      ; 6A  left shift by 6 (make char index:  $40, $80, $C0)
    ROR A						; 6A
    AND #$C0                   ; 29C0 mask out relevent bits
    TAX                        ; AA and put in X

    LDA ch_ailments, X         ; BD0161 check their OB ailments
    CMP #$01					; C901
    BEQ _UseItem_HighP_CantUse  ; F01C if dead... can't use
    CMP #$02					; C902
    BEQ _UseItem_HighP_CantUse  ; F018 if stone... can't use

	LDA #160                    
    JSR MenuRecoverHP_Abs      ;   recover 128 HP for target (index is still in X).  Can use _Abs version

    JSR DrawItemTargetMenu     ; 2000B4  because we already checked the ailments
    JSR MenuWaitForBtn_SFX     ; 2013B6 then redraw the menu to reflect the HP change, and wait for the user to press a button

    DEC item_highp             ; CE3C60 then remove a highp potion from the inventory

UseItem_Exit:
    JMP EnterItemMenu          ; 4C1DB1 re-enter item menu (item menu needs to be redrawn)

_UseItem_HighP_CantUse:       ; can't make this local because of stupid UseItem_Pure hijacking the above label
    JSR PlaySFX_Error          ; 2026DB play the error sound effect
    JMP _UseItem_HighP_Loop    ; 4C0097 and keep looping until they select a legal target or escape with B
