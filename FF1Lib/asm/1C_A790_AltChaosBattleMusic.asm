btlformation            = $6A
music_track             = $4B
CHAOS_FORMATION         = $7B
BATTLE_SONG             = $50
BOSS_BATTLE_SONG        = $55
ReturnToPrepBattle      = $99C8
btl_followupmusic       = $6BA7
btl_soft2000            = $68B7
btl_soft2001            = $68B8
CalcBossMusic           = $A790
SwapPRG 				= $FE03


;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;
; Bank $0B
; rom location $2D9C8 (includes header)
; jumps to CalcBossMusic in bank 1C
; replaces the first few instructions from PrepBattleVarsAndEnterBattle, 
;     which are moved to the new routine
;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;
.org $99B8
JumpToDetermineBossMusic:
		LDA #>(CalcBossMusic-1)
		PHA
		LDA #<(CalcBossMusic-1)
		PHA
		LDA #$1C ;bank with new routine
		JMP SwapPRG
;assembled bytes
;A9 A7 48 A9 8F 48 A9 1C 4C 03 FE



;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;
; Bank $1C
; rom location $727A0 (includes header)
; Checks battle formation and sets the music track differently whether
;    player is at Chaos, nothing fancy.
;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;
.org $A790
CalcBossMusic:
        LDA #$00
        STA btl_soft2000
        STA btl_soft2001 ;copied from bank 0B
        
        LDA btlformation ;get current formation
        AND #$7F
        CMP #CHAOS_FORMATION ;check if we're at Chaos
        BEQ BossMusic ;jump to set boss music or fall through to normal music
    RegularMusic:
        LDA #BATTLE_SONG
        JMP @Done
    BossMusic:
        LDA #BOSS_BATTLE_SONG
    @Done:
        STA music_track           ; set music track and followup
        STA btl_followupmusic

        LDA #>(ReturnToPrepBattle-1) ;and return
		PHA
		LDA #<(ReturnToPrepBattle-1)
		PHA
		LDA #$0B
		JMP SwapPRG

;assembled bytes
;A9008DB7688DB868A56A297FC97BF005A9504CA7A7A955854B8DA76BA99948A9C748A90B4C03FE
