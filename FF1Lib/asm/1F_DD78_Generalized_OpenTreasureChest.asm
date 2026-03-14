

unsram 		        = $6000
items		        = unsram + $20
game_flags          = unsram + $0200 

tmp		            = $10
tileprop            = $44 ; 2 bytes
ret_bank            = $58
dlg_itemid 	        = $61
btlformation        = $6A
btl_SpikeTileFlag   = $0341 ; fort SetRNG compatibility
talkarray 	        = $70
dlgsfx 		        = $7D

btl_result          = $6B86

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
GiveReward 		    = $B410
VictoryLoop         = $C938
SwapPRG_L 		    = $FE03
SwapPRG             = $FE03
lut_TreasureJingle  = $B600 ;bank 11
lut_TreasureNew     = $BF00 ;bank 11

.include "Constants.inc"
.include "variables.inc"


;;;;;
;;
;; This combines all of the writes to $DD78 in bank $1F
;; Making sure it runs in order will be necessary.
;; "normal" treasure-chest opening and monster-in-a-box can share the
;; 16-byte prelude. The chests-in-order prelude is also 16 bytes, and must be
;; written after the "normal" one. Both of them exit the prelude with
;; the item ID in accumulator
.ORG $DD78

OpenTreasureChest:
;;  Normal or MIAB                                                          Chests in Order

    LDA #BANK_TREASURE      ; A900                                          LDA #BANK_TALKROUTINE       ; A911
    JSR SwapPRG_L           ; 2003FE                                        JSR SwapPRG_L               ; 2003FE

    LDX tileprop+1          ; A645    X <- chest index                      LDA lut_TreasureJingle,X    ; BD00B6
    LDY lut_Treasure, X     ; BC00B1  Y <- treasure contents                BNE CheckTrapOrGiveReward   ; D006

    LDA #BANK_TALKROUTINE   ; A911                                              JSR OpenChestCount      ; 2000B9
    JSR SwapPRG_L           ; 2003FE                                            LDA lut_TreasureNew,X   ; BD00BF
    TYA                     ; 98      A <- Item ID

                            ; 16 bytes

;; accumulator now has the item ID in either case

;; Now, the normal or chests-in-order converge -
;; GiveReward just needs the item ID in the accumulator,
;; and exits with the dialogue ID in X

;; Monster-in-a-box's CheckTrap routine also enters with
;; item ID in accumulator and exits with dialogue ID in X

; .ORG $DD88
CheckTrapOrGiveReward:

    ; Normal or Chests in Order                                             MIAB 
    JSR GiveReward          ; 2010B4                                        JSR CheckTrap           ; 3 bytes
                                          
;; Save Chest as Open                                                       TXA                     ; 1 byte
    BCS :+                  ; B00A                                          RTS                     ; 1 byte
      LDY tileprop+1        ; A445                                                                  
      LDA game_flags, Y     ; B90062                                                                ; 5 bytes
      ORA #GMFLG_TCOPEN     ; 0904
      STA game_flags, Y     ; 990062

    TXA                     ; 8A      X <- Dialogue ID
    RTS                     ; 60

                            ; 17 bytes

;; NOTE: Rando puts the new-game-staring-levels routine immediately after this at $DD9A,
;; so anything else that needs to happen in the future should happen
;; in bank $11




.ORG $8EA0
  
CheckTrap:
  ;; this was LDA dgl_itmid, because the original MIAB prelude stored the
  ;; item ID at dgl_itemid instead in Y before doing the bank swap.
  
  ;; but because it's now entering with the item ID in A
  ;; we can do the store in dgl_itemid here instead --
  ;; only changes one byte!

  STA dgl_itemid              ; 85 61
  JSR CheckCanTake            ; check the inventory space
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


;;; this was part of open-chests-in-order, but is no longer needed;
;;  in fact it probably wasn't needed in the first place
;moved from bank 1F to 11 to make room
;B915
SaveChestOpen:
	BCS :+                   ; if 'C' is set jump ahead, otherwise mark the chest as open B00A
      LDY tileprop+1           ; get the ID of this chest A445
      LDA game_flags, Y        ; flip on the TCOPEN flag to mark this TC as open B90062
      ORA #GMFLG_TCOPEN        ; 0904
      STA game_flags, Y        ; 990062
	:
	RTS
    
    
   



    