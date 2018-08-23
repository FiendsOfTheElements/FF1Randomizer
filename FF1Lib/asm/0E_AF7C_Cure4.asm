DrawItemTargetMenu = $B400
DrawItemDescBox = $B92B
hp_recovery = $64
MenuRecoverHP_Abs = $B561
ch_maxhp = $610C
ch_curhp = $610A
                              ; Note, this was not all assembled together, only the needed parts were,
							  ; more variables would be needed if you intended to do so

CureFamily_Loop:
    JSR ItemTargetMenuLoop  ; handle the item target menu loop
    BCS CureFamily_Exit     ; if they pressed B, just exit

    LDA cursor              ; otherwise... get cursor
    ROR A
    ROR A
    ROR A
    AND #$C0                ; shift it to get a usable index
    TAX                     ; and put in X

    LDA ch_ailments, X      ; get target's OB ailments
    CMP #$01
    BEQ CureFamily_CantUse  ; if dead... can't target
    CMP #$02
    BEQ CureFamily_CantUse  ; can't target if stone, either

    LDA hp_recovery         ; otherwise, we can target.  Get the HP to recover
	                          ; CHANGES FROMT HE DISSASSEMBLY START HERE
    CMP #$0                   ; some of this is the same, but moved, and the rest is new from me
    BEQ cure4

    JSR MenuRecoverHP_Abs   ; and recover it
    JMP skipcure4stuff

    cure4:
      LDA ch_maxhp+1, X     
      STA ch_curhp+1, X       
      LDA ch_maxhp, X        
      STA ch_curhp, X        

    skipcure4stuff:
    JSR DrawItemTargetMenu  ; then redraw the target menu screen to reflect changes
    LDX mp_required         ; put mp required index in X
    DEC ch_magicdata, X     ; and subtract 1 MP from the proper level

    JSR MenuWaitForBtn_SFX  ; Then just wait for the player to press a button.  Then exit by re-entering magic menu

  CureFamily_Exit:
    JMP EnterMagicMenu      ; to exit, re-enter (redraw) magic menu

  CureFamily_CantUse:
    JSR PlaySFX_Error       ; if can't use, play the error sound effect
    JMP CureFamily_Loop     ; and loop until you get a proper target

;;;;;;;;;;;;;;

UseMagic_CUR4:

    JSR DrawItemTargetMenu  ; draw item target menu
    LDA #$2B
    JSR DrawItemDescBox     ; and appropriate description text
        
    LDA #$0
    STA hp_recovery
    JMP CureFamily_Loop

;;;;;;;;;;;;;;
