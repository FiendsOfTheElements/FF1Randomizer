.include "Constants.inc"
.include "variables.inc"

InTalkReenterMap = $9618
SkipDialogueBox = $9643
InTalkBattle = $B080

.org $9600

InTalkBattleRunFix:
	JSR InTalkBattle        ; Trigger the battle
    LDA #$03
    CMP btl_result          ; Check if we ran from battle
    BNE WonBattle           ; If we did
    JSR InTalkReenterMap  ; Skip giving the item
    PLA                   ; Clear an extra address in the stack
    PLA                   ;  since we're one routine deeper
    JMP SkipDialogueBox
WonBattle:
	RTS
	