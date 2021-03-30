
;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;
;;
;;  NewGame_LoadStartingLevels  [$DD9A]
;;
;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;
	JSR NewGame_LoadStartingStats ;206DC7 we replace this call with a call to this routine, so first thing we do the replaced call, right?
	
	LDA #$1B ;A91B LevelUp code is in 1B
    JSR SwapPRG_L ;2003FE
	
	JSR Copy_StartingEquipment ;202085 
	JSR AwardStartingEXP ;209684 give the characters the starting exp and heal them
	
	LDA #$1E ;A91E swap in the menu bank for restoring the magic charges
    JSR SwapPRG_L ;2003FE
	
	JSR MenuRecoverPartyMP ;20F3AB restore the magic charges
	
	RTS ;60
