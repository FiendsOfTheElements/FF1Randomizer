; 
; TrappedChests - 2021-05-23
;
; Modify OpenTreasureChest to check for traps
;  and trigger a battle if there's one for a
;  given treasure chest

unsram 		= $6000
items		= unsram + $20
game_flags      = unsram + $0200 

tmp		= $10
tileprop        = $44 ; 2 bytes
ret_bank        = $58
dlg_itemid 	= $61
btlformation    = $6A
btl_SpikeTileFlag = $0341 ; fort SetRNG compatibility
talkarray 	= $70
dlgsfx 		= $7D

btl_result      = $6B86

BANK_TREASURE	 	= $00
BANK_TALKROUTINE	= $11

GMFLG_TCOPEN		= $04

lut_TrappedChest	= $8F00
lut_Treasure		= $B100   ; BANK_TREASURE

InTalkBattleNoRun   = $9600 
InTalkReenterMap 	= $9618
CheckCanTake        = $B180
InTalkDialogueBox	= $963D
SkipDialogueBox		= $9643
GiveReward 		= $B410
VictoryLoop             = $C938
SwapPRG_L 		= $FE03

 .ORG $DD78
  
  LDA #BANK_TREASURE       ; swap to bank containing treasure chest info A900
  JSR SwapPRG_L            ; 2003FE

  LDX tileprop+1           ; put chest index in X A645
  LDA lut_Treasure, X      ; use it to get the contents of the chest BD00B1
  STA dlg_itemid
  
  LDA #BANK_TALKROUTINE   ; Swap to bank with the trap routine
  JSR SwapPRG_L
  JSR CheckTrap            ; Check for trap
  TXA                      ; X has the dialog ID, either Can't hold or In this chest you found
  RTS

 .ORG $8EA0
  
CheckTrap:
  LDA dlg_itemid              ; Load item and check if we have
  JSR CheckCanTake            ;  the inventory space
  BCS CantTake                ; If not branch
    LDX tileprop+1            ; Get tile property (chest ID)
    LDA lut_TrappedChest, X   ; Check if that chest is trapped
    BEQ NoTrap                ; If $00, no trap, branch and gve the item		
      STA btlformation        ; If it is, store the battle formation
	  LDA dlg_itemid		  ; save dlg_itemid
	  PHA					  ; save dlg_itemid
	  LDA #$0
	  STA dlg_itemid		  ; clear dlg_itemid
      LDA #$C0                ; Show "Monster-in-a-box!"
      JSR InTalkDialogueBox   
      LDA btlformation        ; Get back battle formation
      JSR InTalkBattleNoRun   ; Trigger the battle
      LDA #$03
      CMP btl_result          ; Check if we ran from battle
      BNE WonBattle           ; If we did
DontGiveItem:    
		PLA					  ; remove dlg_itemid from stack
        JSR InTalkReenterMap  ; Skip giving the item
        PLA                   ; Clear an extra address in the stack
        PLA                   ;  since we're one routine deeper
        JMP SkipDialogueBox   
WonBattle:
	  PLA					  ; restore dlg_itemid
	  STA dlg_itemid 	      ; restore dlg_itemid
      LDA #$7B
      CMP btlformation          ; Check if we killed Chaos
      BEQ KilledChaos                 
      JSR GiveItem            ; Give the item
      JSR InTalkReenterMap    ; And reenter the map
      LDX #$F0                ; Load "In this chest you've found..."
	  STX tileprop+1		  ; Fake a TileProp(must be non zero)
      RTS
KilledChaos:
  JMP VictoryLoop
NoTrap:
  JSR GiveItem                ; GiveItem only
  RTS
CantTake:
  TAX                         ; Transfer dialog ID
  RTS

GiveItem:
  CLC
  LDA dlg_itemid           ; Get item
  JSR GiveReward           ; Give item as normal
  LDY tileprop+1           ; get the ID of this chest A445
  LDA game_flags, Y        ; flip on the TCOPEN flag to mark this TC as open
  ORA #GMFLG_TCOPEN  
  STA game_flags, Y  
  RTS

 .ORG $8E80
; Trigger a battle inside the talk routine
InTalkBattleNoRun:
  STA btlformation         ; store battle formation
  STA btl_SpikeTileFlag    ; treat chest fight like spike Tile for SetRNG
  JSR BattleTransition     ; Do transition
  LDA #$00
  STA $2001
  STA $4015
  LDY #$02
  JSR JumpThenSwapPRG      ; LoadBattleCHRPal
  LDY #$01
  JSR JumpThenSwapPRG      ; EnterBattle
  RTS
  
