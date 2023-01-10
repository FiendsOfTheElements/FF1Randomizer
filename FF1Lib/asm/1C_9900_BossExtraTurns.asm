SwapPRG = $FE03
btl_turnorder = $6848 
btlformation = $6A
btl_battletype  = $6C92

;readjust JSR DoBattleRound call
;31462
.org 9452
JSR DoBattleRoundNew

;replace turn order lut and do battle round start with
;3216C
.ORG $A15C
DoBattleRoundNew:
  ;put the destination address on the stack for after the bank swap
  LDA #>(InitTurnOrderBuffer-1) ;2
  PHA ;1
  LDA #<(InitTurnOrderBuffer-1) ;2
  PHA ;1
  ;load the new bank
  LDA #$1B ;2
  JMP SwapPRG ;3
  DoBattleRoundReturn:
  NOP
  NOP
  NOP
  NOP
  NOP
  NOP
  NOP
  NOP
  NOP
  NOP
  NOP

;extensions
;bank 1b 0x6d910
.ORG $9900
lut_InitialTurnOrder:
  .BYTE $00, $01, $02, $03, $04, $05, $06, $07, $08     ; enemy IDs
  .BYTE $80, $81, $82, $83                              ; character IDs

lut_BossInitialTurnOrder:
  .BYTE $00, $01, $02, $03, $04, $05, $06, $07, $00     ; enemy IDs
  .BYTE $80, $81, $82, $83                              ; character IDs

lut_BossMixInitialTurnOrder:
  .BYTE $00, $01, $02, $03, $04, $05, $06, $07, $02     ; enemy IDs
  .BYTE $80, $81, $82, $83                              ; character IDs

InitTurnOrderBuffer:
    ;check to see if this is a boss fight
    LDA btlformation
    CMP #$56 ;add warmech
    BEQ BossFormation
    CMP #$7E ;skip pirates
    BEQ RegularFormation
    CMP #$7C ;skip earth 3 vampire
    BEQ RegularFormation
    CMP #$73
    BCC RegularFormation
    CMP #$80
    BCS RegularFormation
    BossFormation:
      JSR ExtraTurnsSetup
      JMP TurnOrderReturn
    RegularFormation:
    LDY #$0D                        ; initialize turn order buffer
    TurnOrderInitLoop:
        LDA lut_InitialTurnOrder-1, Y ;  by just copying values from a lut
        STA btl_turnorder-1, Y
        DEY
        BNE TurnOrderInitLoop
        JMP TurnOrderReturn

    TurnOrderReturn:
    ;put return address for after bank swap back
    LDA #>(DoBattleRoundReturn-1)
    PHA
    LDA #<(DoBattleRoundReturn-1)
    PHA

    ;go back to bank 0C
    LDA #$0C
    JMP SwapPRG

ExtraTurnsSetup:
  LDY #$0D                        ; initialize turn order buffer
  TurnOrderInitBossLoop:
      LDA btl_battletype
      CMP #$02
      BNE LargeFormation
        ;small formation
        LDA lut_BossMixInitialTurnOrder-1, Y ;astos + garland are set up as a 2-6 mix formation for some reason
        JMP StoreTurnOrder
      LargeFormation:
      LDA lut_BossInitialTurnOrder-1, Y ;  by just copying values from a lut, use the extra turn in the Boss list
      StoreTurnOrder:
      STA btl_turnorder-1, Y
      DEY
      BNE TurnOrderInitBossLoop
  RTS
