; Last Update: 2021-02-02
; Modify DivideRewardBySurvivors and LvlUp_AwardExp to
;  give XP to Nones



 .ORG $8B43 ;$1B
DivideRewardBySurvivors:
  JSR CountNones      ; Count nones first
  NOP                 ;  then resume normal dead chars counting
  
  .ORG $8D20
CountNones: 
    LDY #$00          ; Reset count
    LDA $6801         ;  Is character's class None?
    CMP #$FF
    BNE NotNone1
      INY             ; If yes, increase count by 1
NotNone1:
    LDA $6813
    CMP #$FF
    BNE NotNone2
      INY
NotNone2:
    LDA $6825
    CMP #$FF
    BNE NotNone3
      INY
NotNone3:
    LDA $6837
    CMP #$FF
    BNE NotNone4
      INY
NotNone4:
    LDX #$04        ; Set loop count to count dead chars
    RTS
    

ch_stats = $6100
ch_ailments = $6101
eob_exp_reward = $6878
battlereward = $88

LvlUp_GetCharExp = $8860
GiveRewardToParty = $8AA0


 .ORG $8826
LvlUp_AwardExp:          
  JSR CheckCanGetXP        ; Jump to new routine to check if char can
  BCS AwardXP              ;  get XP, carry set if they can, clear if they can't 
    RTS
    
 .ORG $8833  
AwardXP:

 .ORG $8710
 
CheckCanGetXP:
  LDY #$00                          ; check their class
  LDA ($86), Y                       
  CMP #$FF                          ; Is class none?
  BEQ IsNone                        ;  go ahead and award XP
    LDY #ch_ailments - ch_stats     ; Otherwise check their ailments
    LDA ($86), Y
    AND #$03
    BEQ GetXP                       ; no ailments = reward
      CMP #$03
      BEQ GetXP                     ; poison = reward
        CLC
        RTS                         ; anything else (stone or dead) = exit without getting a reward
GetXP:
  SEC                               ; Set carry, char can get XP
  RTS
 
IsNone:                             ; Reproduce par of LvlUp_AwardExp for Nones only so we can skip it
  LDA eob_exp_reward                ; move the exp reward back into battlereward
  STA battlereward
  LDA eob_exp_reward+1
  STA battlereward+1
    
  JSR LvlUp_GetCharExp              ; get this char's exp pointer
  JSR GiveRewardToParty             ; add the exp reward to it
  CLC
  RTS