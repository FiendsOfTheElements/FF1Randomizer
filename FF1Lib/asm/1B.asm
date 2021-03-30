.org $8000
.alias ch_stats $6100
.alias ch_magicdata $6300
.alias SwapPRG $FE03
.alias BankC_CrossBankJumpList $9300
.alias btltmp $90
.alias BattleCrossPageJump_L $F209

.outfile "1B.bin"
; @2DAF6  009B

; LDA #$7F
; PHA
; LDA #$FF
; PHA
; LDA #$1B
; JMP $FE03

; LDA #$80
; PHA
; LDA #$2E
; PHA
; LDA #$1B
; JMP $FE03
; RTS
; @2DB05  A97F48A9FF48A91B4C03FEA98048A92E48A91B4C03FE60


; LDA #$1B
; JSR $FE03
; LDA #$84
; LDX #$4C
; @7fc36  A91B2003FEA984A24C





;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;
;;
;;  GameOver  [$8000 :: 0x6c010]
;;
;;    Called when the party has been defeated.  YOU LOSE MOFO!!!
;;
;;
;;
;;    Modified from original
;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;

GameOver:
	lda #$1B
	sta $6BB1
    jsr $d82e
	sta $6ba7
	lda #$00
	sta $6af8
	lda #$04
	ldx #$09
	jsr DrawEOBCombatBox
	jsr WaitForAnyInput
	jsr RespondDelay_UndrawAllCombatBoxes
	lda #$03
    
	
	CLC
    ADC #<BankC_CrossBankJumpList
    STA btltmp+6
    LDA #$00
    ADC #>BankC_CrossBankJumpList
    STA btltmp+7
    LDA #$0C
    JMP BattleCrossPageJump_L        ; jump to BattleFadeOutAndRestartGame_L

;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;
;;
;;  EndOfBattleWrapUp  [$802F :: 0x6C03F]
;;
;;    Gives GP/Exp rewards for a victorious battle, levels up
;;  characters when appropriate, and displays all the necessary crap
;;  on screen
;;
;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;

EndOfBattleWrapUp:
	lda #$1B
	sta $6BB1
	lda #$00
	sta $6af8
	tax
	lda #$04
	jsr DrawEOBCombatBox
	jsr WaitForAnyInput
	jsr RespondDelay_UndrawAllCombatBoxes
	ldy #$0d
	jsr SumBattleReward
	jsr DivideRewardBySurvivors
	lda $88
	sta $6878
	lda $89
	sta $6879
	ora $88
	bne L9b3e
	inc $6878
L9b3e:				  ldy #$0f
	jsr SumBattleReward
	lda $88
	sta $6876
	lda $89
	sta $6877
	lda #$1c
	sta $80
	lda #$60
	sta $81
	jsr GiveRewardToParty
	lda #$00
	sta $6aa6
	lda #$01
	sta $6aa7
	jsr Draw4EobBoxes
	jsr WaitForAnyInput
	jsr RespondDelay_UndrawAllCombatBoxes
	lda #$00
	jsr LvlUp_AwardAndUpdateExp
	lda #$01
	jsr LvlUp_AwardAndUpdateExp
	lda #$02
	jsr LvlUp_AwardAndUpdateExp
	lda #$03
	JSR LvlUp_AwardAndUpdateExp
	LDA #$9B
	PHA
	LDA #$0A
	PHA
	LDA #$0B
	JMP SwapPRG

;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;
;;
;;  Battle Messages  [$80AA :: 0x6C0BA]
;;
;;  This block of text is weird because the pointer table comes AFTER the text
;;
;;
data_BattleMessages_Raw:                        ; actual text data
  .INCBIN "0B_8C40_battlemessages - changes.bin"
  
        ; pointer table
		
;; $8456 - "Nothing happens" string
  .BYTE $97, $B2, $B7, $AB, $AC, $B1, $AA, $FF, $AB, $A4, $B3, $B3, $A8, $B1, $B6, $00
  .BYTE $00, $00, $00, $00


;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;
;;
;;  LvlUp_CheckIfMax  [$846A :: 0x6C47E]
;;
;;
;;	new routine, patches bug when hitting lvl 50
;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;

LvlUp_CheckIfMax:
    LDY #$26   
    LDA ($86),Y
    CMP #$31
    BEQ LCheckIfMaxSkip
    PHA
    LDA #$32                  ; bool set for when we are level 50 to 
    STA $10                   ; check if we have leveled up 
    PLA
    RTS
  LCheckIfMaxSkip:
    PLA
    PLA
    PLA
    TAX
    PLA
    TXA
    CMP #$4F
    BNE LTavernModeExit
    LDA $10
    LDX #$00
    STX $10
    CMP #$32
    BNE LNoDisplay
    JMP $8853                 ; LvlUp_Display
  LTavernModeExit:
    JMP $87D5                 ; Tavern Mode, exits to bank 0E
  LNoDisplay:
    PLA
    PLA
    RTS


;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;
;;
;;  AwardStartingEXP  [$8496]
;;
;;
;; Simulates 16 BattleRewards with fixed exp
;; Heals all characters afterwards
;; Really bad hack with the lute
;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;
    LDA game_flags ;AD0062 basically i set bit 7 of item_lute here, because i din't want to use up a byte of sram and i'm pretty sure it's never set again ever.
	ORA #$80 ;0980
	STA game_flags ;8D0062

	LDA #$10 ; A910
	STA eob_gp_reward ;8D7668 just a loop counter, eob_gp_reward isn't used here anyway


	LDA #$FF ;A9FF this sets the low and high bytes of the eob_exp_reward. the FFs will be replaced depending on what level you should receive.
	STA eob_exp_reward	;8D7868
	LDA #$FF ;A9FF
	STA eob_exp_reward + 1 ;8D7968

loop:		
	LDA #$00 ;A900
    JSR LvlUp_AwardAndUpdateExp ;20DA87 this basically awards the exp and does the levelup.
    LDA #$01 ;A901
    JSR LvlUp_AwardAndUpdateExp ;20DA87
    LDA #$02 ;A902
    JSR LvlUp_AwardAndUpdateExp ;20DA87
    LDA #$03 ;A903
    JSR LvlUp_AwardAndUpdateExp ;20DA87
	
	DEC eob_gp_reward ;CE7668 loop stuff
	BNE loop ;D0E7
	
	LDA game_flags ;AD0062 clear bit 7 of the item_lute, it shall never be set of the rest of the game, i hope.
	AND #$7F ;297F
	STA game_flags ;8D0062
	
	LDX #$00 ;A200 heal all characters, crappy code i know
	LDA ch_maxhp, X; BD0C619D0A61BD0D619D0B61
    STA ch_curhp, X
    LDA ch_maxhp+1, X
    STA ch_curhp+1, X	
	
	LDX #$40	;A240
	LDA ch_maxhp, X; BD0C619D0A61BD0D619D0B61
    STA ch_curhp, X
    LDA ch_maxhp+1, X
    STA ch_curhp+1, X	
	
	LDX #$80	;A280
	LDA ch_maxhp, X; BD0C619D0A61BD0D619D0B61
    STA ch_curhp, X
    LDA ch_maxhp+1, X
    STA ch_curhp+1, X	
	
	LDX #$C0	;A2C0
	LDA ch_maxhp, X; BD0C619D0A61BD0D619D0B61
    STA ch_curhp, X
    LDA ch_maxhp+1, X
    STA ch_curhp+1, X
    
	RTS ;60

;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;
;;
;;  LvlUp_DisplaySwitcher  [$8496]
;;
;;
;; Routine to en/disable LvlUp_Display. If the bit 7 of item_lute is set, we are at gamestart and don't want LvlUp_Display to be called, because it crashes the game.
;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;
	LDA game_flags ;AD0062 
	AND #$80 ;2980
	BNE exit ;D003
	JSR LvlUp_Display ;20E389
exit:
	RTS ;60

;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;
;;
;;  Copy_StartingEquipment  [$8520]
;;
;;
;; StartingEquipment Table 32 bytes
;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;


;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;
;;
;;  Copy_StartingEquipment  [$8540]
;;
;;
;; Copies the Starting Equipment into sram
;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;
Copy_StartingEquipment: ;0x8540
    LDX #$00			; A200
    LDY #$00			; A000
    JSR copy_8			; 205785
    LDY #$40			; A040
    JSR copy_8			; 205785
    LDY #$80			; A080
    JSR copy_8			; 205785
    LDY #$C0			; A0C0
    JSR copy_8			; 205785
    RTS					; 60


copy_8:				    ; 0x8557
    LDA source, X		; BD2085
    STA ch_weapons, Y	; 991861
    INX					; E8
    INY					; C8
    TYA					; 98
    AND #$0F			; 290F
    CMP #$08			; C908
    BNE copy_8			; D0F3
    RTS					; 60

.advance $874A
; extra room for future level up changes

;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;
;;
;;  LvlUp_AdjustBBSubStats  [$874A :: 0x6C75A]
;;
;;  Adjusts post level up substats for Black Belts / Masters
;;
;;  input:  lvlup_chstats should be prepped
;;
;; same as original but with extra space for the fix
;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;
.alias ch_class ch_stats
.alias ch_weapons ch_stats + $18
.alias ch_level ch_stats + $26
.alias ch_absorb ch_stats + $20 + $02
.alias ch_dmg ch_stats + $20
.alias lvlup_chstats $86
.alias CLS_BB $02
.alias CLS_MA $08
.alias tmp $10

LvlUp_AdjustBBSubStats:
    LDY #ch_class - ch_stats        ; check to make sure this is a BB/Master
    LDA (lvlup_chstats), Y
    CMP #CLS_BB
    BEQ continue
    CMP #CLS_MA
    BEQ continue                          ; if yes, jump ahead, otherwise just exit
  @Exit:
    RTS

  continue: LDY #ch_weapons - ch_stats      ; see if they have any weapon equipped.
    LDA (lvlup_chstats), Y          ; check all 4 weapon slots, if any of them have an
    BMI @Exit                       ; equipped weapon, exit
    INY
    LDA (lvlup_chstats), Y
    BMI @Exit
    INY
    LDA (lvlup_chstats), Y
    BMI @Exit
    INY
    LDA (lvlup_chstats), Y
    BMI @Exit
    
    LDY #ch_level - ch_stats        ; reaches here if no weapon equipped.  Get the level
    LDA (lvlup_chstats), Y          ;  Add 1 to make it 1-based
    CLC
    ADC #$01
      LDY #ch_absorb - ch_stats       ; Absorb = Level -- BUGGED:  This is the infamous BB Armor Bug
      STA (lvlup_chstats), Y          ;   This should only happen if the character has no ARMOR equipped.
                                      ;   Weapons shouldn't matter.  This cannot be easily fixed here,
                                      ;   as you'd pretty much have to write a new routine.
      ASL
      LDY #ch_dmg - ch_stats          ; Damage = 2*Level
      STA (lvlup_chstats), Y
      RTS
	  
; padding for the BB fix      
.advance $87A2

; [$87A2 :: 0x6C7B2]
data_MaxRewardPlusOne:
  .BYTE $40, $42, $0F

  
data_MaxHPPlusOne:
  .BYTE $E8, $03


GetJoyInput:         jmp $d828

;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;
;;
;;  LvlUp_NoDisplay  [$87AA :: 0x6C7BA]
;;
;;  input:  tmp is the ID of the character to be leveled up (0,1,2,3)
;;
;;  New routine
;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;

LvlUp_NoDisplay:						 lda tmp
		ora #$80
        sta $6c89
        asl
        tay
        lda lut_CharStatsPtrTable,y
        sta $86
        lda lut_CharStatsPtrTable+1,y
        sta $87
        lda lut_CharMagicPtrTable,y
        sta $84
        lda lut_CharMagicPtrTable+1,y
        sta $85
		JSR LvlUp_GetCharExp
		
		
		

L2:     jsr LvLUp_CompareExp
        bcc Lexit
		jsr LvlUp_LevelUp
		jmp L2
		
		
Lexit:  LDA #$0E
	JMP SwapPRG            


;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;
;;
;;  LvlUp_AwardAndUpdateExp  [$87DA :: 0x6C7EA]
;;
;;  input:  A = character index whose XP to update
;;
;;  same as original
;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;
	
LvlUp_AwardAndUpdateExp:                 jsr LvlUp_AwardExp
        jsr LvlUp_GetCharExp
        jsr LvlUp_GetExpToAdvance
        ldy #$26
        lda ($86),y
        cmp #$31
        bne L9b97
        lda #$00
        pha
        pha
        jmp L9ba6
        
L9b97:               ldx #$03
        ldy #$00
        sec
L9b9c:               lda ($82),y
        sbc ($80),y
        pha
        iny
        dex
        bne L9b9c
        pla
L9ba6:               ldy #$17
        pla
        sta ($86),y
        dey
        pla
        sta ($86),y
        rts
        
		
;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;
;;
;;  LvlUp_AwardExp  [$880B :: 0x6C81B]
;;
;;  input:  A = ID of character to reward
;;
;;  Gives Exp to a single party member, and levels them up (once) if necessary
;;
;;  small changes only
;;  
;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;		
		
LvlUp_AwardExp:      ora #$80
        sta $6c89
        asl
        tay
        lda lut_CharStatsPtrTable,y
        sta $86
        lda lut_CharStatsPtrTable+1,y
        sta $87
        lda lut_CharMagicPtrTable,y
        sta $84
        lda lut_CharMagicPtrTable+1,y
        sta $85
        ldy #$01
        lda ($86),y
        and #$03
        beq L9bd8
        cmp #$03
        beq L9bd8
        rts
        
L9bd8:               lda $6878
        sta $88
        lda $6879
        sta $89
        jsr LvlUp_GetCharExp
        jsr GiveRewardToParty
		
		
		
L9be8:               jsr LvLUp_CompareExp
					 bcc L9bf2
LJumpBack:           jsr LvLUp_CompareExp
					 bcc LincludeDisplay
					 jsr LvlUp_LevelUp
					 jmp LJumpBack ; replace with NOPs for no multi levelup
LincludeDisplay:     jsr LvlUp_DisplaySwitcher ; we musn't call LvlUp_Display during gamestart, so we jsr to a subroutine that handles the check
L9bf2:               rts




;; small new routine [$8857 :: 0x6C867]
LvLUp_CompareExp:	 jsr LvlUp_GetExpToAdvance
        ldy #$02
        jsr MultiByteCmp
		rts
        
;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;
;;
;;  LvlUp_GetCharExp  [$8860 :: 0x6C870]
;;
;;  input:   lvlup_chstats
;;  output:  lvlup_curexp
;;
;;  same as original
;;
;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;		

LvlUp_GetCharExp:    lda $86
        clc
        adc #$07
        sta $80
        lda #$00
        adc $87
        sta $81
        rts
        
;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;
;;
;;  LvlUp_GetExpToAdvance  [$886E :: 0x6C87E]
;;
;;  input:   lvlup_chstats
;;  output:  lvlup_exptoadv = points to 3-byte value containing total Exp required to advance
;;
;;    Gives nonsense if character is at level 50
;;
;;	same as original
;;
;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;
		
LvlUp_GetExpToAdvance:                   ldy #$26
        lda ($86),y
        asl
        clc
        adc ($86),y
        adc #<lut_ExpToAdvance
        sta $82
        lda #$00
        adc #>lut_ExpToAdvance
        sta $83
        rts
        
		
;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;
;;
;;  LvlUp_LevelUp  [$8881 :: 0x6C891]
;;
;;  input:   lvlup_curexp, lvlup_chmagic, and lvlup_chstats
;;
;;
;;  Large changes, most of the drawing code has been removed, mostly
;;  the same up to that point
;;
;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;
		
LvlUp_LevelUp:
		jsr LvLUp_CheckIfMax
		nop
		nop
		nop
		nop
		nop
        sta $6bad
        clc
        adc #$01
        sta ($86),y
        sta $687a
        ldy #$00
        lda ($86),y
        asl
        tay
        asl $6bad
        lda lut_LevelUpDataPtrs,y
        clc
        adc $6bad
        sta $82
        lda lut_LevelUpDataPtrs+1,y
        adc #$00
        sta $83
        lda #$00
        sta $687b
        inc $687a
        ldx #$00
        lda ($86,x)
        sta $688e
        tax
        ldy #$21
        lda ($86),y
        clc
        adc lut_LvlUpHitRateBonus,x
        jsr CapAAt200
        sta ($86),y
        ldy #$25
        lda ($86),y
        clc
        adc lut_LvlUpMagDefBonus,x
        jsr CapAAt200
        sta ($86),y
        lda $688e
        beq L9caf
        cmp #$01
        beq L9caf
        ldy #$01
        lda ($82),y
        ldy #$28
L9c79:               lsr
        bcc L9c85
        pha
        lda ($84),y
        clc
        adc #$01
        sta ($84),y
        pla
L9c85:               iny
        cpy #$30
        bne L9c79
        lda $688e
        cmp #$06
        beq L9c99
        cmp #$07
        beq L9c99
        lda #$0a
        bne L9c9b
L9c99:               lda #$05
L9c9b:               ldy #$28
L9c9d:               cmp ($84),y
        bne L9caa
        pha
        lda ($84),y
        sec
        sbc #$01
        sta ($84),y
        pla
L9caa:               iny
        cpy #$30
        bne L9c9d
L9caf:               ldy #$00
        lda ($82),y
        sta $688e
        ldy #$13
        lda ($86),y
        lsr
        lsr
        clc
        adc #$01
        pha
        lda $688e
        and #$20
        beq L9cd1
        lda #$14
        ldx #$19
        jsr RandAX
        jmp L9cd3
        
L9cd1:               lda #$00
L9cd3:               sta $68b3
        pla
        clc
        adc $68b3
        sta $88
        lda #$00
        sta $89
        sta $8a
        lda $86
        clc
        adc #$0c
        sta $80
        lda $87
        adc #$00
        sta $81
        jsr GiveHpBonusToChar
        asl $688e
        asl $688e
        asl $688e
        lda #$00
        sta $6856
        lda #$10
        sta $6858
L9d06:               asl $688e
        bcc L9d0f
L9d0b:               lda #$01
        bne L9d18
L9d0f:               jsr $f227
        and #$03
        beq L9d0b
        lda #$00
L9d18:               ldy $6856
        sta $6aac,y
        ldy $6858
        clc
        adc ($86),y
        cmp #$64
        beq L9d2a
        sta ($86),y
L9d2a:               inc $6858
        inc $6856
        lda $6856
        cmp #$05
        bne L9d06
        ldy #$10
        lda $6aac
        beq L9d50
        lda ($86),y
        lsr
        bcs L9d50
        ldy #$20
        lda ($86),y
        clc
        adc #$01
        cmp #$c9
        beq L9d50
        sta ($86),y
L9d50:               ldy #$23
        lda $6aad
        beq L9d62
        lda ($86),y
        clc
        adc #$01
        cmp #$c9
        beq L9d62
        sta ($86),y
L9d62:               jsr LvlUp_AdjustBBSubStats
LDA $80
AND #$07
CMP #$07
BEQ exitLvlUp
LDA $80
AND #$F0
ADC #$07
STA $80
exitLvlUp:		rts

		
;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;
;;
;;  LvlUp_Display  [$89E3 :: 0x6C9F3]
;;
;;  New routine, does the drawing, similar to the original
;;  in LvlUp_LevelUp
;;
;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;
		
LvlUp_Display:     lda #$00
        sta $6aa6
        lda #$05
        sta $6aa7
        jsr Draw4EobBoxes
        lda #$00           ; just exit here with a jmp to skip stat up messages
        sta $6aa6
        lda #$33
        sta $6aa7
DisplayLoop:         ldy $6aa6
		lda $6aac,y
		beq DisplayLoop_Next
		lda #$38
		sta $6afd
		lda #$0f
		sta $6afa
		sta $6afc
		lda $6aa7
		sta $6afb
		lda #$00
		sta $6afe
		lda #$1b
		sta $0057
		lda #$04
		ldx #$fa
		ldy #$6a
		jsr $f218
		jsr RespondDelay
		jsr WaitForAnyInput
		lda #$01
		jsr $f20f
DisplayLoop_Next:    inc $6aa7
		inc $6aa6
		lda $6aa6
		cmp #$05
		bne DisplayLoop
		jmp RespondDelay_UndrawAllCombatBoxes
        

;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;
;;
;;  Draw4EobBoxes  [$8A42 :: 0x6CA52]
;;
;;  input:  eobbox_slotid = expected to be zero  (why doesn't this routine just zero it?)
;;          eobbox_textid = EOB text ID to print in first box
;;
;;    This routine draws 4 EOB combat boxes with text eobbox_textid, eobbox_textid+1, eobbox_textid+2,
;;  and eobbox_textid+3.
;;
;;  same as original
;;
;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;

Draw4EobBoxes:       lda $6aa6
        ldx $6aa7
        jsr DrawEOBCombatBox
        inc $6aa6
        inc $6aa7
        lda $6aa6
        cmp #$04
        bne Draw4EobBoxes
        rts     
		
		
;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;
;;
;;  lut - Hit rate bonus for each class (assigned at level up)  [$8A59 :: 0x6CA69]

lut_LvlUpHitRateBonus:
  .BYTE  3,  2,  3,  2,  1,  1,   3,  2,  3,  2,  1,  1
  ;     FT  TH  BB  RM  WM  BM   KN  NJ  MA  RW  WW  BW
  
;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;
;;
;;  lut - Mag Def bonus for each class (assigned at level up)  [$8A65 :: 0x6CA75]
;;
;;    This is arguably BUGGED -- since MA gets penalized, and fighters get more magdef than
;;  fighters, which doesn't make any sense.  It's almost as if these should be inverted
;;  to be 5-their_value.
  
lut_LvlUpMagDefBonus:
  .BYTE  3,  2,  4,  2,  2,  2,   3,  2,  1,  2,  2,  2
  ;     FT  TH  BB  RM  WM  BM   KN  NJ  MA  RW  WW  BW
  
;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;
;;
;;  [$8A71 :: 0x6CA81]
;;    Lut - pointer table to level up data for each class
;;  see data_LevelUpData_Raw for explanation of data format
  
lut_LevelUpDataPtrs:
  .WORD data_LevelUpData_Raw + [49*2 * 0]      ; Fighter
  .WORD data_LevelUpData_Raw + [49*2 * 1]      ; Thief
  .WORD data_LevelUpData_Raw + [49*2 * 2]      ; BlBelt
  .WORD data_LevelUpData_Raw + [49*2 * 3]      ; RM
  .WORD data_LevelUpData_Raw + [49*2 * 4]      ; WM
  .WORD data_LevelUpData_Raw + [49*2 * 5]      ; BM
  .WORD data_LevelUpData_Raw + [49*2 * 0]      ;  Promoted classes share data with their
  .WORD data_LevelUpData_Raw + [49*2 * 1]      ;  non-promoted counterparts
  .WORD data_LevelUpData_Raw + [49*2 * 2]
  .WORD data_LevelUpData_Raw + [49*2 * 3]
  .WORD data_LevelUpData_Raw + [49*2 * 4]
  .WORD data_LevelUpData_Raw + [49*2 * 5]  
;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;
;;
;;  [$8A89 :: 0x6CA99]
;;    Lut - pointer table to the beginning of each character's OB stats in RAM

lut_CharStatsPtrTable:
  .WORD ch_stats
  .WORD ch_stats + $40
  .WORD ch_stats + $80
  .WORD ch_stats + $C0
  
;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;
;;
;;  [$8A91 :: 0x6CAA1]
;;    Lut - pointer table to the beginning of each character's magic data in RAM
  
lut_CharMagicPtrTable:
  .WORD ch_magicdata
  .WORD ch_magicdata + $40
  .WORD ch_magicdata + $80
  .WORD ch_magicdata + $C0
  
;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;
;;
;;  CapAAt200  [$8A9B :: 0x6CAAB]
;;
;;    Sets A to 200 if it is over 200
;;
;;  same as original
;;
;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;

CapAAt200:           cmp #$c9
        bcc L9e22
        lda #$c8
L9e22:               rts
        
		
;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;
;;
;;  GiveRewardToParty  [$8AA0 :: 0x6CAB0]
;;
;;  input:        $80,81 = destination pointer.  Points to stat to be increased
;;          battlereward = 3-byte reward
;;
;;
;;  same as original
;;
;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;
		
GiveRewardToParty:   jsr AddBattleRewardToVal
        lda #<data_MaxRewardPlusOne
        sta $82
        lda #>data_MaxRewardPlusOne
        sta $83
        ldy #$02
        jsr MultiByteCmp
        bcc L9e43
        ldy #$00
L9e37:               lda ($82),y
        sta ($80),y
        iny
        cpy #$03
        bne L9e37
        jmp SubtractOneFromVal
        
L9e43:               rts
       

;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;
;;
;;  GiveHpBonusToChar  [$8AC1 :: 0x6CAD1]
;;
;;  input:        $80,81 = destination pointer.  Points to stat to be increased (max HP)
;;          battlereward = 3-byte reward (must not be > $7FFF or this will overwrite character strength!)
;;
;;    This routine is basically a copy of GiveRewardToParty.  The only differences are:
;;  - It uses a different cap (1000 instead of 1000000)
;;  - It records the result to eobtext_print_hp so it can be displayed to the user
;;
;;
;;  same as original
;;
;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;
	   
GiveHpBonusToChar:   jsr AddBattleRewardToVal
        lda #<data_MaxHPPlusOne 
        sta $82
        lda #>data_MaxHPPlusOne 
        sta $83
        ldy #$01
        jsr MultiByteCmp
        bcc L9e64
        ldy #$00
L9e58:               lda ($82),y
        sta ($80),y
        iny
        cpy #$02
        bne L9e58
        jsr SubtractOneFromVal
L9e64:               ldy #$00
        lda ($80),y
        sta $687c
        iny
        lda ($80),y
        sta $687d
        rts
     
;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;
;;
;;  AddBattleRewardToVal [$8AEF :: 0x6CAFF]
;;
;;  input:  $80  points to a 3-byte value to add battlereward to.
;;
;;  same as original
;;
;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;

	 
AddBattleRewardToVal:                    lda #$03
        sta $8b
        lda #$00
        tay
        tax
        sta $8a
        clc
L9e7d:               lda ($80),y
        adc $88,x
        sta ($80),y
        inx
        iny
        dec $8b
        bne L9e7d
        rts

;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;
;;
;;  SumBattleReward [$8B07 :: 0x6CB17]
;;
;;  input:   Y = #en_gp or #en_exp, so indicate which reward you want to sum
;;
;;  output:  battlereward
;;
;;    Sums the rewards for all enemies in combat.
;;
;;
;;  same as original
;;
;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;
		
SumBattleReward:     lda #$00
	sta $88
	sta $89
	sta $8a
	ldx #$09
L9e94:               clc
	lda $6bd3,y
	adc $88
	sta $88
	lda $6bd4,y
	adc $89
	sta $89
	lda $8a
	adc #$00
	sta $8a
	tya
	adc #$14
	tay
	dex
	bne L9e94
	rts

	
;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;
;;
;;  MultiByteCmp  [$8B2E :: 0x6CB3E]
;;
;;  input:  $80,81 = ptr to first value
;;          $82,83 = ptr to second value
;;               Y = number of bytes (-1) to compare.  Ex:  Y=1 compares 2 bytes
;;
;;  output:    Z,C = set to result of CMP
;;
;;    C set if ($80) >= ($82)
;;    Z set if they're equal
;;
;;
;;  same as original
;;
;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;	
	
MultiByteCmp:        lda ($80),y
        cmp ($82),y
        beq L9ebe
        php
        pla
        and #$81
        pha
        plp
        rts
        
L9ebe:               dey
        bne MultiByteCmp
        lda ($80),y
        cmp ($82),y
        rts


;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;
;;
;;  DivideRewardBySurvivors  [$8B43 :: 0x6CB53]
;;
;;    Divides the reward (in battlereward) by the number
;;  of surviving party members.
;;
;;  input/output:   battlereward   (2 bytes)
;;
;;  same as original
;;
;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;
		
DivideRewardBySurvivors:                 ldx #$04
	ldy #$00
	lda $6ad1
	ora $6ad2
L9ed0:               lsr
	bcs L9ed4
	iny
L9ed4:               dex
	bne L9ed0
	sty $84
	lda #$00
	sta $85
	ldx #$10
	rol $88
	rol $89
L9ee3:               rol $85
	lda $85
	cmp $84
	bcc L9eef
	sbc $84
	sta $85
L9eef:               rol $88
	rol $89
	dex
	bne L9ee3
	lda $85
	sta $84
	rts
	
;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;
;;
;;  Multiply X,A [$8B78 :: 0x6CB88]
;;
;;    Does unsigned multiplication:  X*A
;;  High 8 bits of result stored in X
;;  Low  8 bits of result stored in A
;;
;;
;;  same as original
;;
;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;
	
MultiplyXA:          sta $68b3
        stx $68b4
        ldx #$08
        lda #$00
        sta $68b5
L9f08:               lsr $68b3
        bcc L9f11
        clc
        adc $68b4
L9f11:               ror
        ror $68b5
        dex
        bne L9f08
        tax
        lda $68b5
        rts

;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;
;;
;;  RandAX [$8B9A :: 0x6CBAA]
;;
;;  Generates a random number between [A,X] inclusive.
;;   Generated number is stored in A on return
;;
;;  This is accomplished by fixed point multiplication:
;;     range = hi - lo + 1
;;     tmp = random() * range / 256
;;     result = tmp + lo
;;
;;  Where:  hi=X, lo=A, and random produces a random number between [0,255]
;;
;;  Note:  I kind of gave up on labelling the scratch variables here.  There are too many temporary battle variables
;;    to reasonably keep track of.
;;
;;
;;  same as original
;;
;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;
		
RandAX:              sta $68af
        inx
        stx $68b0
        txa
        sec
        sbc $68af
        sta $68b6
        jsr $f227
        ldx $68b6
        jsr MultiplyXA
        txa
        clc
        adc $68af
        rts


;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;
;;
;;  DrawEOBCombatBox  [$8BB8 :: 0x6CBC8]
;;
;;    Draws an "End of Battle" (EOB) combat box.  These are boxes containing
;;  text that is shown at the end of battle... like "Level up!" kind of stuff.
;;
;;  input:  A = combat box ID to draw
;;          X = EOB string ID   (see lut_EOBText for valid values)
;;
;;  similar to original but uses stack manipulation trickery to
;;  return here
;;
;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;
        
DrawEOBCombatBox:    sta $68b3
        lda #$1b
        sta $0057
        txa
        asl
        tay
        ldx lut_EOBText,y
        lda lut_EOBText+1,y
        tay
		lda #>return_1
		pha 
		lda #<return_1-1
		pha
		LDA #$D7 
		PHA
		LDA #$F5
		PHA
		lda #$1B
		sta $E8
        lda $68b3
        jmp $f218
return_1:	inc $6af8
        rts
		
 

;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;
;;
;;  RespondDelay_UndrawAllCombatBoxes [$8BE3 :: 0x6CBF3]
;;
;;    Calls RespondDelay, then undraws all combat boxes
;;
;;  similar to original but uses stack manipulation trickery to
;;  return here
;;
;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;
 
RespondDelay_UndrawAllCombatBoxes:       jsr RespondDelay
		lda #>return_2
		pha 
		lda #<return_2-1
		pha
		LDA #$D7 
		PHA
		LDA #$F5
		PHA
		lda #$1B
		sta $E8
		lda $6af8
        jmp $f20f
return_2:	lda #$00
        sta $6af8
        rts
 
;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;
;;
;;  RespondDelay [$8C02 :: 0x6CC12]
;;
;;    Waits the appropriate number of frames, as indicated by 'btl_responddelay'.
;;  Normally that value is dictated by the player's desired respond rate -- however
;;  its value is overwritten by some code for some areas of the game.
;;
;;  same as original
;;
;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;
 
RespondDelay:        lda $6bac
        sta $6ad0
L9f6c:               jsr $fe00
        jsr MusicPlay
        dec $6ad0
        bne L9f6c
        rts
 
;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;
;;
;;  WaitForAnyInput [$8C14 :: 0x6CC24]
;;
;;  Spins and waits full frames until the user presses any button.
;;
;;  Input stored in A and btl_input upon exit.
;;
;;  same as original 
;;
;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;
 
WaitForAnyInput:     jsr GetJoyInput
        pha
        jsr $fe00
        jsr MusicPlay
        pla
        beq WaitForAnyInput
        rts
 
;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;
;;
;;  MusicPlay  [$8C22 :: 0x6CC32]
;;
;;    Call music play -- version for this bank
;;
;;  same as original
;;
;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;

 
MusicPlay:           lda #$1b
        sta $0057
        lda $004b
        bpl L9f96
        lda $6ba7
        sta $004b
L9f96:               jmp $c009                   

;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;
;;
;;  lut for End of Battle text  [$8C32 :: 0x6CC42]
;;
;;    Strings used for DrawEOBCombatBox.  I have no idea why some of the
;; string data is stored here, and some of it is stored way back at $9950.

.alias eob_gp_reward $6876
.alias eob_exp_reward $6878

lut_EOBText:
  .WORD @MnstPrsh               ; 0
  .WORD @ExpUp                  ; 1
  .WORD @ExpVal                 ; 2
  .WORD @Gold                   ; 3
  .WORD @GoldVal                ; 4
  .WORD @LevUp                  ; 5
  .WORD eobtext_NameLN          ; 6
  .WORD eobtext_HPMax           ; 7
  .WORD eobtext_Npts            ; 8
  .WORD eobtext_PartyPerished   ; 9
  
  @MnstPrsh: .BYTE $0F, $3D, $0F, $3C, $00      ; "Monsters perished"
  @ExpUp:    .BYTE $0F, $49, $00                ; "EXP up"
  @ExpVal:   .BYTE $0C
             .WORD eob_exp_reward
             .BYTE $99, $00                     ; "##P"  where ## is the experience reward
  @Gold:     .BYTE $90, $98, $95, $8D, $00      ; "GOLD"
  @GoldVal:  .BYTE $0C
             .WORD eob_gp_reward
             .BYTE $90, $00, $00                ; "##G"   where ## is the GP reward
  @LevUp:    .BYTE $0F, $30, $00                ; "Lev. up!"
  
  
 ;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;
;;  Misc End-Of-Battle text [$8C61 :: 0x6CC71]
;;
;;    I don't know why these strings are stored here and not with the rest of the
;;  end of battle strings at $A00E.  Whatever.

.alias eobtext_print_level $687A
.alias eobtext_print_hp $687C

eobtext_NameLN:
  .BYTE $02, $FF, $95, $0C
  .WORD eobtext_print_level
  .BYTE $00                                 ; "<Name> L##", where <Name> is btl_attacker's name and ## is value at $687A
eobtext_HPMax:
  .BYTE $0F, $31, $00                       ; "HP max"
eobtext_Npts:
  .BYTE $0C
  .WORD eobtext_print_hp
  .BYTE $0F, $32, $00                       ; "##pts." where ## is value at $687C
eobtext_PartyPerished:
  .BYTE $04, $0F, $3E, $0F, $3C, $00        ; "<Name> party perished", where <Name> is the party leader's name
  
;;

;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;
;;
;;  SubtractOneFromVal  [$8C77 :: 0x6CC87]
;;
;;  input:  $80 points to desired value
;;
;;    Subtracts 1 from the 1-byte value stored at the given pointer.
;;
;; same as original
;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;

SubtractOneFromVal:  ldy #$00
        lda ($80),y
        sec
        sbc #$01
        sta ($80),y
        rts



;; Exp to Advance [$8C81 :: 0x6CC91]
lut_ExpToAdvance:
.INCBIN "lut_ExpToAdvance.bin"


.advance $8DA9
 
;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;
;;
;;  Level up data!  [$8DA9 :: 0x6CDB9]
;;
;;  Data consists of 2 bytes per level.
;;  49 levels per class
;;   6 classes (promoted classes share their unpromoted data)
;;
;;  Byte 0:  bit 5:  set if level up is "strong" (extra 20-24 HP bonus)
;;           bit 4:  set for guaranteed Str increase
;;           bit 3:  set for guaranteed Agil increase
;;           bit 2:  set for guaranteed Int increase
;;           bit 1:  set for guaranteed Vit increase
;;           bit 0:  set for guaranteed Luck increase
;;
;;  Byte 1:  MP up.  Each bit corresponds to a level of spell.
;;            Ex:  bit 0 means you'll get a level 1 charge
;;                 bit 7 means you'll get a level 8 charge

data_LevelUpData_Raw:
  .INCBIN "0B_9094_levelupdata.bin"
