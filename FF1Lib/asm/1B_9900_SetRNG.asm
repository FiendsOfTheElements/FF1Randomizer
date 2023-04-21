.include "Constants.inc"
.include "variables.inc"

SwapPRG = $FE03
;uses these globals that are available throughout a given battle
btl_rngstateTurnStart = $0340
btl_SpikeTileFlag = $0341

ClearAltMessageBuffer = $B00C
GetEnemyStatPtr = $BB86
GetBattleFormation = $C54A
BattleStepRNG = $C571
enram = $9A
playerid = $88

;DoFrame_WithInput  [$97C7 :: 0x317D7]
;bank 0C
;317D7
.org $97C7
NOP
NOP
NOP

;EA EA EA

;patch the battle entry point to set the rng
;0x2D9C8
.org $99B8
PrepBattleVarsAndEnterBattle:
    ;put the destination address on the stack for after the bank swap
    LDA #>(PrepBattleVarsAndEnterBattleNew-1) ;2
    PHA ;1
    LDA #<(PrepBattleVarsAndEnterBattleNew-1) ;2
    PHA ;1
    ;load the new bank
    LDA #$1B ;2
    JMP SwapPRG ;3
    
    PrepBattleBankReturn:
    NOP
    NOP
    NOP
    NOP
    NOP

;A9 95 48 A9 FF 48 A9 1B 4C 03 FE EA EA EA EA EA

;bank 0C
;0x3143C
.org $942C
BattleLogicLoop:
    ;put the destination address on the stack for after the bank swap
    LDA #>(BattleLogicLoopExt-1) ;2
    PHA ;1
    LDA #<(BattleLogicLoopExt-1) ;2
    PHA ;1
    ;load the new bank
    LDA #$1B ;2
    JMP SwapPRG ;3

    BattleLogicLoopReturn:
    NOP
    NOP 

;old 13 BYTES
;A9 96 48 A9 3F 48 A9 1B 4C 03 FE EA EA

;0x331A7
.org $B197
Battle_DoEnemyTurn:
    TAX ;1

    ;put the destination address on the stack for after the bank swap
    LDA #>(Battle_DoEnemyTurnExt-1) ;2
    PHA ;1
    LDA #<(Battle_DoEnemyTurnExt-1) ;2
    PHA ;1
    ;load the new bank
    LDA #$1B ;2
    JMP SwapPRG ;3

    Battle_DoEnemyTurnReturn:
    TXA ;1
    NOP
    NOP
    NOP
    JSR GetEnemyStatPtr        ; do the rest of the stuff we left off
    STA enram                  ; put pointer to this enemy's stats in $9A,9B
    STX enram+1
    JSR ClearAltMessageBuffer

;AA A9 96 48 A9 7F 48 A9 1B 4C 03 FE 8A EA EA EA 20 86 BB 85 9A 86 9B 20 0C B0

;0x32367
.org $A357
Battle_DoPlayerTurn:
    ;put the destination address on the stack for after the bank swap
    LDA #>(Battle_DoPlayerTurnExt-1) ;2
    PHA ;1
    LDA #<(Battle_DoPlayerTurnExt-1) ;2
    PHA ;1
    ;load the new bank
    LDA #$1B ;2
    JMP SwapPRG ;3

    Battle_DoPlayerTurnReturn:
    TXA ;1
    PLP ;1

    ;A9 96 48 A9 BF 48 A9 1B 4C 03 FE 8A 28


;extensions
;bank 1b 0x6d910
.ORG $9900
PrepBattleVarsAndEnterBattleNew:
    JSR PrepBattleAudio
    LDA btlformation
    STA tmp
    AND #$7F
    CMP #$73
    BCS BossFormation
    ;check to see if it is also a spike tile battle, in which case we should start at a known value for that too
    LDA btl_SpikeTileFlag
    BNE BossFormation
        LDA battlecounter ;15x battle counter
        ASL A
        ASL A
        ASL A
        ASL A
        SEC
        SBC battlecounter
        STA tmp

    BossFormation:
    LDA btlformation ;17x battle formation, or 18x battle formation if boss
    ASL A
    ASL A
    ASL A
    ASL A
    CLC
    ADC btlformation
    CLC
    ADC tmp
    CLC

    STA btl_rngstateTurnStart
    STA btl_rngstate

    ;clear the spike tile battle flag
    LDA #$00
    STA btl_SpikeTileFlag

    ;put return address for after bank swap back
    LDA #>(PrepBattleBankReturn-1)
    PHA
    LDA #<(PrepBattleBankReturn-1)
    PHA

    ;go back to bank 0B
    LDA #$0B
    JMP SwapPRG

;20 00 9A A5 6A 85 10 29 7F C9 73 B0 10 AD 41 03 D0 0B A5 F7 0A 0A 0A 0A 38 E5 F7 85 10 A5 6A 0A 0A 0A 0A 18 65 6A 18 65 10 18 8D 40 03 8D 8A 68 A9 00 8D 41 03 A9 99 48 A9 C2 48 A9 0B 4C 03 FE

.org $9940
BattleLogicLoopExt:
    LDA btl_rngstateTurnStart ;3
    CLC
    ADC #$43
    STA btl_rngstate ;3
    STA btl_rngstateTurnStart ;3

    LDY #$1C
    LDA #$00
    STA btl_attackid
    : STA btl_charcmdbuf-1, Y       ; clear the character battle command buffer
      DEY
      BNE :-

    ;put return address for after bank swap back
    LDA #>(BattleLogicLoopReturn-1)
    PHA
    LDA #<(BattleLogicLoopReturn-1)
    PHA

    ;go back to bank 0C
    LDA #$0C
    JMP SwapPRG

;AD 40 03 18 69 43 8D 8A 68 8D 40 03 A0 1C A9 00 8D 8C 6C 99 8E 68 88 D0 FA A9 94 48 A9 36 48 A9 0C 4C 03 FE

.org $9980
Battle_DoEnemyTurnExt:
    TXA

    STA btl_attacker
    STA btl_attacker_alt

    LDA btl_curturn
    ;set up the RNG, use current turn counter to grab a different starting section of RNG
    ;rng for this turn = current turn counter * 19 + rng start
    CLC
    ASL A
    ASL A
    ASL A
    ASL A
    ADC btl_curturn
    CLC
    ADC btl_curturn
    CLC
    ADC btl_curturn
    CLC
    ADC btl_rngstateTurnStart
    STA btl_rngstate
    CLC

    LDA #$02 ;2
    STA btlmag_playerhitsfx ;3     ; enemy->player magic plays the "cha" sound effect (ID 2)

    LDA #$00 ;2
    STA btlmag_magicsource     ;3 ; Enemies can't use potions or items -- so their magic source is always 'magic'

    ;put return address for after bank swap back
    LDA #>(Battle_DoEnemyTurnReturn-1)
    PHA
    LDA #<(Battle_DoEnemyTurnReturn-1)
    PHA

    ;go back to bank 0C
    LDA #$0C
    JMP SwapPRG

;8A 8D 89 6C 8D CD 6B AD 8E 68 18 0A 0A 0A 0A 6D 8E 68 18 6D 8E 68 18 6D 8E 68 18 6D 40 03 8D 8A 68 18 A9 02 8D A7 6D A9 00 8D 8F 6C A9 B1 48 A9 A2 48 A9 0C 4C 03 FE

.org $99C0
Battle_DoPlayerTurnExt:
    LDA btl_curturn
    ;set up the RNG, use current turn counter to grab a different starting section of RNG
    ;rng for this turn = current turn counter * 19 + rng start
    CLC
    ASL A
    ASL A
    ASL A
    ASL A
    ADC btl_curturn
    CLC
    ADC btl_curturn
    CLC
    ADC btl_curturn
    CLC
    ADC btl_rngstateTurnStart
    STA btl_rngstate
    CLC

    LDY btl_curturn
    LDA btl_turnorder, Y
    AND #$03                    ; mask off the high bit to get the player ID
    STA playerid               ;  and record it for future use
    
    ASL A
    ASL A
    TAY                         ; id*4 in Y (command index)
    
    LDA btl_charcmdbuf, Y       ; get command byte
    LSR A                       ; shift out bit 0 ('dead' bit)      and throw away
    LSR A                       ; shift out bit 1 ('stone' bit)     and throw away
    LSR A                       ; shift out bit 2 ('attack' bit)

    PHP ;save the C flag status at this point
    TAX ;put a in X to survive the bank swap

    ;put return address for after bank swap back
    LDA #>(Battle_DoPlayerTurnReturn-1)
    PHA
    LDA #<(Battle_DoPlayerTurnReturn-1)
    PHA

    ;go back to bank 0C
    LDA #$0C
    JMP SwapPRG

;AD 8E 68 18 0A 0A 0A 0A 6D 8E 68 18 6D 8E 68 18 6D 8E 68 18 6D 40 03 8D 8A 68 18 AC 8E 68 B9 48 68 29 03 85 88 0A 0A A8 B9 8F 68 4A 4A 4A 08 AA A9 A3 48 A9 61 48 A9 0C 4C 03 FE

.org $9A00
PrepBattleAudio:
    ;had to move these to make room for battle entry jumps
    ;these need to be updated with disable music if applied
    LDA #$00
    STA btl_soft2000            ; clear soft PPU regs
    STA btl_soft2001
    
    LDA #$50
    STA a:music_track           ; set music track and followup
    STA btl_followupmusic
    RTS

;A9 00 8D B7 68 8D B8 68 A9 50 8D 4B 00 8D A7 6B 60

;rewritten SMMove_Battle that is updated in bank 1f
;at 7CDD3
SMMove_Battle:
    LDA tileprop+1         ; check the secondary property byte to see which battle to do
    BNE SMMove_Spiked      ; if high bit is clear, this is a spiked tile (forced battle)
                           ;   otherwise... it's a random encounter
    JSR BattleStepRNG      ; get a pseudo-random number from the battle step RNG
    CMP battlerate         ; if that number is >= the battle rate for this map...
    BCS DoneNobattle       ;  ... then there's no battle

      LDA cur_map             ; otherwise, begin a random encounter
      CLC                     ;  get the current map, and add 8*8 to it to get past the
      ADC #8*8                ;  overworld domains.
      JSR GetBattleFormation  ; Get the battle formation from this map's domain
      BCC DoneBattle

  SMMove_Spiked:
    STA btlformation      ; for spiked tiles, the secondary byte is the battle formation
    STA btl_SpikeTileFlag

  DoneBattle:
    LDA #TP_BATTLEMARKER  ;   record it so the appropriate battle is triggered.
    STA tileprop          ; and also replace the tileprop byte with the battle marker bit to start a battle

  DoneNobattle:
    CLC               ; CLC because movement is A-OK, and exit
    RTS

;A5 45 D0 11 20 71 C5 C5 F8 B0 13 A5 48 18 69 40 20 4A C5 90 05 85 6A 8D 41 03 A9 20 85 44 18 60
