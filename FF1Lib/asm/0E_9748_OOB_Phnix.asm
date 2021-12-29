$9748

UseItem_Phnix:
    JSR DrawItemTargetMenu     ; 2000B4 Draw the item target menu (need to know who to use this heal potion on)
    LDA #$08					;A908
    JSR DrawItemDescBox        ; 202BB9 open up the description box with text ID $20

  _UseItem_Phnix_Loop:
    JSR ItemTargetMenuLoop     ; 20A0B3 run the item target loop.
    BCS UseItem_Exit           ; B01F if B was pressed (C set), exit this menu(31byte)

    LDA cursor                 ; A562 otherwise... A was pressed.
    ROR A                      ; 6A  get the cursor (target character)
    ROR A                      ; 6A  left shift by 6 (make char index:  $40, $80, $C0)
    ROR A						; 6A
    AND #$C0                   ; 29C0 mask out relevent bits
    TAX                        ; AA and put in X

    LDA #$01                   ; A901 mark the ailment-to-cure as "death" ($01)
    STA tmp                    ; 8510 put it in tmp for 'CureOBAilment' routine
    JSR CureOBAilment          ; 2088B3 attempt to cure death!
    BCS _UseItem_Phnix_CantUse ; B011 if the char didn't have the death ailment... can't use this spell on him

    LDA #100                   ; A964 otherwise.. can use!
    JSR MenuRecoverHP_Abs      ; 2061B5 recover 100 HP for target (index is still in X).  Can use _Abs version

JSR DrawItemTargetMenu     ; 2000B4  because we already checked the ailments
    JSR MenuWaitForBtn_SFX     ; 2013B6 then redraw the menu to reflect the HP change, and wait for the user to press a button

    DEC item_phnix             ; CE3D60 then remove a Phnix potion from the inventory

UseItem_Exit:
    JMP EnterItemMenu          ; 4C1DB1 re-enter item menu (item menu needs to be redrawn)

_UseItem_Phnix_CantUse:       ; can't make this local because of stupid UseItem_Pure hijacking the above label
    JSR PlaySFX_Error          ; 2026DB play the error sound effect
    JMP _UseItem_Heal_Loop     ; 4C5097 and keep looping until they select a legal target or escape with B

2000B4A908202BB920A0B3B01FA5626A6A6A29C0AAA90185102088B3B011A9642061B52000B42013B6CE3D604C1DB12026DB4C5097