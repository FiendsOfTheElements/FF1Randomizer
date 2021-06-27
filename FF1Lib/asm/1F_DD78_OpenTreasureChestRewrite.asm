OpenTreasureChest:           ;
    LDA #BANK_TREASURE       ; swap to bank containing treasure chest info A900
    JSR SwapPRG_L            ; 2003FE

    LDX tileprop+1           ; put chest index in X A645
    LDY lut_Treasure, X      ; use it to get the contents of the chest BC00B1

	LDA #BANK_TALKROUTINE    ; swap to bank containing new talkroutines A911
    JSR SwapPRG_L            ; 2003FE
	
	TYA						; Put ItemId into A 98
	
    JSR GiveReward            ; Jump to new sub routine 2010B0
    BCS :+                   ; if 'C' is set jump ahead, otherwise mark the chest as open B00A
      LDY tileprop+1           ; get the ID of this chest A445
      LDA game_flags, Y        ; flip on the TCOPEN flag to mark this TC as open B90062
      ORA #GMFLG_TCOPEN        ; 0904
      STA game_flags, Y        ; 990062
:
    TXA                        ; 8A
    RTS                        ; 60 
