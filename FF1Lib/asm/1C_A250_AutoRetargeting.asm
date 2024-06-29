btl_enemyIDs = $6BB7
battle_defender_index = $6885
btl_defender = $6C8A
SwapPRG = $FE03

;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;
; Bank $0C
; rom location $324D2 (includes header)
; jumps to JumpToRetarget
; replaces the first 2 STX instructions in PlayerAttackEnemy_Physical from
;     bank C
;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;
.org $A4C2
	JMP $92F4 ;unused space in bank C
	NOP
	NOP
	NOP

;assembled bytes
;4C F4 92 EA EA EA


;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;
; Bank $0C
; rom location $31304 (includes header)
; uses unused space to push the address of Retarget onto the stack and
;    call Swap_PRG to change banks
;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;
RetargetAddress = $A250
.org $92F4
JumpToRetarget:
	LDA #>(RetargetAddress-1)
	PHA
	LDA #<(RetargetAddress-1)
	PHA
	LDA #$1C ;bank with Retarget function
    JMP SwapPRG

;assembled bytes
;A9 A2 48 A9 4F 48 A9 1C 4C 03 FE


;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;
; Bank $1C
; rom location $72260 (includes header)
; Takes in targeted enemy index as X (range 0-8) and if that enemy is
;     dead, changes X to the index of an enemy that is still alive
;
; The battle system sets an enemy ID to $FF if they don't exist or are dead
;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;
.org $A250
Retarget:
	LDA btl_enemyIDs,X ;check original target
	CMP #$FF
	BNE @Exit ;original target is still alive, do nothing
	
	LDX #9 ;start at 9 (1-based counter makes the branch simpler)
	@EnemyLoop:
		LDA btl_enemyIDs-1,X ;load the in-battle ID of that enemy
		CMP #$FF ;ID will be set to $FF if enemy is dead or didn't exist
		BNE @Found ;leave if enemy is alive
		DEX 
		BNE @EnemyLoop ;otherwise loop to next enemy
	@Found:
		DEX ;returned index is 0-based
	@Exit:
		STX battle_defender_index ;code we replaced from PlayerAttackEnemy_Physical
		STX btl_defender
		LDA #$A4 ;jump back to regular bank C battle code (PlayerAttackEnemy_Physical)
		PHA
		LDA #$C7
		PHA
		LDA #$0C ;we're going back to bank C
		JMP SwapPRG

;assembled bytes
;BD B7 6B C9 FF D0 0D A2 09 BD B6 6B C9 FF D0 03 CA D0 F6 CA 8E 85 68 8E 8A 6C A9 A4 48 A9 C7 48 A9 0C 4C 03 FE
