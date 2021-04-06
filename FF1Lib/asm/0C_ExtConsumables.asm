.include "Constants.inc"
.include "variables.inc"

SwapPRG = $FE03
CharWalkAnimationRight = $9E06

lut_drinkbox_order = $F930
lut_drinkbox_effect = $F938
lut_drinkbox_target = $F940
DrawDrinkBox = $F921
CancelBattleAction = $94AF
DoNothingMessageBox = $96FE
SelectPlayerTarget = $9A3A
UndrawNBattleBlocks_L = $F6B3
MenuSelection_Drink = $9C06
SetCharacterBattleCommand = $935D
BtlMag_MarkSpellConnected = $B885
BtlMag_Effect_CureAilment_1C = $A0C6-1
Battle_PlayerTryWakeup = $A42F
Battle_PlayerTryUnstun = $A481
PrepCharStatPointers = $A145
Player_DoMagic = $B3C5
Battle_PlayerTryRun = $A3D8
Player_DoItem = $B3B5
PlayerAttackEnemy_Physical = $A4BA
CheckForEndOfBattle = $933B
UndoCharDrink = $9399

btl_unfmtcbtbox_buffer2 = $6B1A ; i don't use the first buffer because there's an inconvenient page break there

tmp1 = $6856
tmp2 = $6857
tmp3 = $6858

.org $95EC

;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;
;;
;;  BattleSubMenu_Drink  [$95F5 :: 0x31605]
;;
;;  Called when the player selects 'DRINK'
;;
;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;

BattleSubMenu_Drink:
	JSR DrawDrinkBox          	; draw the drink box
	
    JSR MenuSelection_Drink     ; get menu selection from the player  
	PHA                         ; backup the A/B button press
    LDA #$01
    JSR UndrawNBattleBlocks_L   ; undraw the drink menu
    PLA
	CMP #$02
	BEQ Cancel                  ; was B pressed to get out of the drink menu?	
	
    LDA btlcurs_y               ; assemble cursor index
	ASL
	ORA btlcurs_x
	TAY
	
	LDX lut_drinkbox_order, Y	; get item in X
	BEQ Nothing
	LDA items, X				; get item count
	BEQ Nothing             
	STX tmp1					; store itemid in temp memory
	STY tmp2					; store index in temp memory

	LDA lut_drinkbox_target, Y
	BEQ SetConsume

	LDA btlcmd_curchar          ; get input for SelectPlayerTarget
    JSR SelectPlayerTarget      ; and call it!
    CMP #$02
    BEQ BattleSubMenu_Drink              ; if they pressed B...
	
SetConsume:
	LDY btlcmd_curchar
    LDA #$02
    STA btl_charcmdconsumetype, Y   ; store 02 as the consumable type (to indicate DRINK)	
    LDA tmp1
    STA btl_charcmditem, Y   
    STA btl_charcmdconsumeid, Y     ; store menu selection as consumed ID -- to indicate which potion  (00/01 for Heal/Pure potion)  
	TAX
	
	DEC items, X				; remove one item

    LDY tmp2                	; get the menu selection
	LDX lut_drinkbox_effect, Y	; get the effect into X

	LDA btlcurs_y           	; get target
    AND #$03
    ORA #$80                	; OR with 80 to indicate it's a player target
    TAY
		
    LDA #$10					; we do it as item magic
	
	JMP SetCharacterBattleCommand
Nothing:	
	JSR DoNothingMessageBox
Cancel:
    JMP CancelBattleAction  
	  
	  
;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;
;;
;;  Battle_PlMag_IsPlayerValid  [$9652]
;;
;;    Used by Battle_PlMag_XXX routines to see if a player target is valid.
;;    Player targets are invalid if they are dead/stone.
;;	  Cure ailments is always allowed
;;
;;  input:  btl_entityptr_obrom - should point to target's OB stats
;;
;;  output: Z = set if target is valid, clear if invalid
;;
;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;

Battle_PlMag_IsPlayerValid:
	LDA btlmag_effect               ; Get this spell's effect ID
	CMP #$08						; Cure Ailments
	BEQ :+
    LDY #ch_ailments - ch_stats
    LDA (btl_entityptr_obrom), Y
    AND #AIL_DEAD | AIL_STONE
  : RTS
  
  
 BRK
 NOP
 BRK
 
 .org $B9CD
 
;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;
;;
;;  BtlMag_Effect_CureAilment  [$B9CD :: 0x339DD]
;;
;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;

BtlMag_Effect_CureAilment:
	LDA #>BtlMag_Effect_CureAilment_1C
	PHA
	LDA #<BtlMag_Effect_CureAilment_1C
	PHA
	LDA #$1C
	JMP SwapPRG	
    JSR BtlMag_MarkSpellConnected   ; and mark the spell as connected
	RTS     

 BRK
 NOP
 BRK
 
 .org $93AE
 
;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;
;;
;;  Battle_AfterFadeIn   [$A13B]
;;
;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;
LDY #$04
LDA #$0
  :	DEY
	STA btl_charcmdconsumetype, Y
	CPY #$00
	BNE :-

 BRK
 NOP
 BRK

 .org $A357

;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;
;;
;;  Battle_DoPlayerTurn  [$A357 :: 0x32367]
;;
;;  input:  A = player ID  ($80-83)
;;
;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;

Battle_DoPlayerTurn:
        @playerid = $88         ; local - holds player ID (0-3)
        
    AND #$03                    ; mask off the high bit to get the player ID
    STA @playerid               ;  and record it for future use
    
    ASL A
    ASL A
    TAY                         ; id*4 in Y (command index)
    
    LDA btl_charcmdbuf, Y       ; get command byte
    LSR A                       ; shift out bit 0 ('dead' bit)      and throw away
    LSR A                       ; shift out bit 1 ('stone' bit)     and throw away
    
    LSR A                       ; shift out bit 2 ('attack' bit)
    BCC :+                      ; if set... attack the enemy
      LDX btl_charcmdbuf+2, Y           ; X = enemy target
      LDA @playerid                     ; A = attacker
      JMP PlayerAttackEnemy_Physical    ; Do the attack!
      
  : LSR A                       ; shift out bit 3 ('drink' bit)
	LSR A                       ; shift out bit 4 ('item' bit)
    BCC :+                      ; if set... use item!
	  LDX @playerid
	  LDA #$00
	  STA btl_charcmdconsumetype, X	; clear btl_charcmdconsumetype to mark consumable as consumed 
	  
      LDA btl_charcmdbuf+1, Y       ; A = effect ID
      LDX btl_charcmdbuf+2, Y       ; X = target
      LDY @playerid                 ; Y = attacker
      JMP Player_DoItem
      
  : LSR A                       ; shift out bit 5 ('run' bit)
    BCC :+
      LDA @playerid                 ; load this player's ID
      JMP Battle_PlayerTryRun       ; try to run!
      
  : LSR A                       ; shift out bit 6 ('magic' bit)
    BCC :+
        TYA                         ; back up command index
        PHA
        
        LDY @playerid
        LDX btl_charcmdconsumeid, Y ; get the level of this spell
        DEC ch_mp, X                ; take away a spell charge
        
        PLA                         ; restore command index
        TAY
        
        LDA btl_charcmdbuf+1, Y     ; A = effect
        LDX btl_charcmdbuf+2, Y     ; X = target
        LDY @playerid               ; Y = attacker
        JMP Player_DoMagic
    
    ;;  Code reaches here if the player had no command, which would only happen if they are
    ;;  immobilized or dead.
    
    :
        @ail = $89          ; local, temp ram to hold ailments
    
    LDA @playerid
    JSR PrepCharStatPointers        ; load player's ailments
    LDY #ch_ailments - ch_stats
    LDA (btl_ob_charstat_ptr), Y
    STA @ail
    
    AND #AIL_SLEEP          ; are they asleep?
    BEQ :+
      LDA @playerid         ; if yes, try to wake up
      JMP Battle_PlayerTryWakeup
      
  : LDA @ail
    AND #AIL_STUN           ; are they stunned?
    BEQ :+
      LDA @playerid
      JMP Battle_PlayerTryUnstun    ; if yes, try to unstun
      
    ; otherwise, they simply don't have an action or they're dead/stone
    ;   so just exit without doing anything
  : RTS
  
  
;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;
;;
;;  EndOfBattleRoundChecks  [$A3C6]
;;
;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;
EndOfBattleRoundChecks:
	LDY #$04
  :	DEY
	JSR UndoCharDrink
	CPY #$00
	BNE :-
	JMP CheckForEndOfBattle

  
  
  