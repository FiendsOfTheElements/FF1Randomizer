; [3B294]
UseItem_Tent:

    LDA #2             ; First ether recovers an MP to first TWO spell levels
    JMP UseItem_Ether  ; Jump to the ether code

; [3B299]
UseItem_Cabin:

    LDA #4             ; Second ether recovers an MP to first FOUR spell levels
    JMP UseItem_Ether  ; Jump to the ether code

; [3B29E]
UseItem_House:

    LDA #8             ; Third ether recovers MP to all EIGHT spell levels
                       ; (No JMP needed, fall directly into ether code)

; [3B2A0]                
UseItem_Ether:

    STA hp_recovery         ; Store the loaded max spell level in variable hp_recovery [$64]
    JSR DrawItemTargetMenu  ; Draw the character targeting menu
    LDA #$1B                ; Load $1B (a message index, "Who will recover MP" or some such)
    JSR DrawItemDescBox     ; Show the loaded message

; [3B2AA]
UseItemEtherLoop:

    JSR ItemTargetMenuLoop  ; Target a character
    BCS UseItem_Exit        ; If B was pressed to cancel targeting, exit the menu
    LDA cursor              ; Load the cursor index
    ROR                     ; Do math on the cursor index to get the correct character offset
    ROR                     ;  "
    ROR                     ;  "
    AND #$C0                ;  "
    TAX                     ; Put the character offset into X
    LDA ch_ailments, X      ; Load the character's status byte
    CMP #1                  ; See if character is KO
    BEQ Ether_CantUse       ; If so, target is illegal
    CMP #2                  ; See if character is Petrified
    BEQ Ether_CantUse       ; If so, target is illegal
    LDY hp_recovery         ; Y will represent the number of spell levels to recover
    JSR MenuRecoverMP       ; Recover the appropriate MP
    LDA hp_recovery         ; Load the recovered spell levels
    LSR                     ; Do math on the spell level to recover the item index
    LSR                     ;  "
    TAX                     ; Put it in X so we can use it as an offset
    DEC item_tent, X        ; Reduce the appropriate item quantity (tent index plus the calculated offset)

; [3B2CF]
UseItemExit:

    JMP EnterItemMenu       ; Return to item menu

; [3B2D2]
Ether_CantUse:

    JSR PlaySFXError        ; Play the error buzz
    JMP UseItemEtherLoop    ; Try to target someone else

;=======================

; [3B26F]
MenuRecoverMP:

    INY                     ; Previous code wasn't incrementing the highest level so let's inc Y first
 RestoreMPLoop:
    DEY                     ; Next spell level
    BEQ @Skip               ; If we're at 0, we're done
    LDA ch_curmp, X         ; Load character's current MP for that spell level
    CMP ch_maxmp, X         ; Compare it to their max
    BCS @NoInc              ; If it's greater or equal, don't increase
    INC ch_curmp, X         ; Otherwise, increase current MP by 1
 @NoInc:
    INX                     ; Next spell level offset
    JMP RestoreMPLoop                ; Continue looping
 @Skip:
    JMP MenuRecoverHPDone   ; Play healing sound and return from subroutine [3B589]

