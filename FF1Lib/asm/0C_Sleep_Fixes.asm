Basic Sleep can be from 0-256 now

;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;
;;
;;  EnemyAsleep  [$B1D5 :: 0x331D4]
;;
;;  Called when the enemy is asleep
;;  Bugs: Incorrect index into enemy stat data
;;        Incorrect math buffer index for subtraction
;;        Incorrect branch instruction after subtraction
;;
;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;

  LDY #$02                          ; Changed Index to use correct enemy HP
  LDA ($9A),Y
  STA $6856
  INY
  LDA ($9A),Y
  STA $6857
  LDA #$00                          ; Starting value for RNG
  LDX #$FF                          ; Ending value for RNG
  JSR RandAX
  TAX
  LDA #$00                          ; Index for math buffer 
  JSR MathBuf_Sub                   ; mathbuf -= random
                                    ; Removed unneeded load
  BEQ @EnemySleep                   ; Changed branch to check zero flag set by MathBuf_Sub
                                    ; Branches to sleep if zero flag is set
  LDA #$DF
  JSR ApplyEnemyAilmentMask
  LDA #$0F
  BNE @PrintAndEnd
@EnemySleep
  LDA #$02
@PrintAndEnd:
  JMP $B28E
  NOP
  NOP
  NOP

;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;
;;
;;  RandAX [$AE5D :: 0x32E5D]
;;
;;  Generates a random number between [A,X] inclusive.
;;  Generated number is stored in A on return
;;  Bugs: Extra increment of X causes overflow at max value
;;        results in only lower bound being used
;;        Store of X is unused
;;
;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;

  STA $68AF
  NOP                               ; Removed unnecessary X increment
  NOP                               ; Removed unused store
  NOP
  NOP

   
;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;
;;
;;  Battle_PlayerTryWakeup  [$A42F :: 0x3242F]
;;
;;  Player taking their turn trying to wake themselves up.
;;
;;  input:  A = player ID
;;
;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;

  PHA
  JSR DrawPlayerAttackerCombatBox
  PLA
  JSR PrepCharStatPointers
  LDY #ch_maxhp - ch_stats          ; put max HP in mathbuf
  LDA (btl_ob_charstat_ptr), Y
  STA btl_mathbuf
  INY
  LDA (btl_ob_charstat_ptr), Y
  STA btl_mathbuf+1
  LDA #$00                          ; Starting value for RNG
  LDX #$FF                          ; Ending value for RNG
  JSR RandAX
  TAX
  LDA #$00
  JSR MathBuf_Sub                   ; mathbuf -= random
  
  NOP                               ;
  NOP                               ;
  NOP                               ;
  NOP                               ;
  NOP                               ;
  NOP                               ;
  NOP                               ;
  NOP                               ;
  NOP                               ;
  NOP                               ; Removed unneeded clipping and extra comparison
  NOP                               ;
  NOP                               ;
  NOP                               ;
  NOP                               ;
  NOP                               ;
  NOP                               ;
  NOP                               ;
  NOP                               ;
  NOP                               ;
  NOP                               ;

  BNE @PlayerWake                   ; Checks zero flag set by MathBuf_Sub
                                    ; Branches to wake if zero flag is not set
  LDA #$28
  JMP $AA07
@PlayerWake
  LDY #$02
  LDA ($80),Y
  AND #$DF
  STA ($80),Y
  LDY #$01
  LDA ($82),Y
  AND #$DF
  STA ($82),Y
  LDA #$27
  JMP $AA07


Sleep only removed on hit

;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;
;;
;;  Battle_DoPlayerTurn  [$A357 :: 0x32367]
;;
;;  Remove the call to random player wake up
;;
;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;

$A3B1 :: 0x323B1
  NOP
  NOP
  NOP
  NOP
  NOP
  NOP
  NOP
  NOP
  NOP

;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;
;;
;;  Battle_DoEnemyTurn  [$B197 :: 0x331A7]
;;
;;  Remove the call to random enemy wake up
;;
;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;

$B1BC :: 0x331BC
  NOP
  NOP
  NOP
  NOP

Sleep Removed At Start Of Hit
;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;
;;
;;  DoPhysicalAttack  [$A67B :: 0x3268B]
;;
;;  Removed unneeded code
;;  Corrected hit loop to only apply touch effects on hit
;;  Added Remove sleep on hit
;;
;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;

  DoPhysicalAttack:
  LDA #$00
  TAX
  LDY #$02
  JSR PrepareAndDrawSimpleCombatBox
  LDA #$02
  LDY #$03
  LDX #$00
  JSR PrepareAndDrawSimpleCombatBox
  JSR $A65A
  LDA #$A8
  STA $6856
  LDA $686C
  STA $6858
  LDA $6872
  STA $6862
  LDA $6888
  AND #$08
  BEQ $A6B0
  LDA $6856
  SEC
  SBC #$28
  STA $6856
  LDA $6889
  AND #$08
  BEQ $A6C0
  LDA $6856
  CLC
  ADC #$28
  STA $6856
  LDA $686D
  AND $6876
  STA $685C
  LDA $686E
  AND $6877
  STA $685E
  LDA $685C
  ORA $685E
  BEQ $A700
  LDA #$00
  LDX #$28
  JSR MathBuf_Add
  LDY $6856
  LDX $6857
  JSR $A4AA
  STY $6856
  STX $6857
  LDA $6858
  CLC
  ADC #$1E
  STA $6858
  BCC $A700
  LDA #$FF
  STA $6858
  LDA $6889
  AND #$30
  BEQ @DefenderMobile
  LDA $6858
  LSR A
  LSR A
  CLC
  ADC $6858
  STA $6858
  BCC $A74B
  LDA #$FF
  STA $6858
  JMP $A74B
@DefenderMobile:
  LDA #$00
  LDX $686F
  JSR MathBuf_Add
  LDY $6856
  LDX $6857
  JSR $A4AA
  STY $6856
  STX $6857
  LDA #$00
  LDX $6878
  JSR MathBuf_Sub
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
  NOP
  NOP
  NOP
  NOP
  LDA $6856
  BNE $A753
  INC $6856
  LDA $6862
  CMP $6856
  BCC $A761
  LDA $6856
  STA $6862
  LDA $6871
  LDX $6870
  JSR MultiplyXA
  STA $685A
  LDA $685A
  BNE $A775
  INC $685A
  LDA $6881
  LDX #$00
  JSR $AB2F
  LDA $6887
  BNE $A7A6
  LDX $6880
  BNE $A78A
  INC $6AEE
  LDX $6880
  LDY #$00
  LDA $6884
  JSR $9E70
  LDA $6889
  AND #$03
  BNE $A79F
  JSR $BBFA
  LDA #$00
  STA $6AEE
  BEQ $A7BD
  LDA #$01
  JSR $BEB8
  JSR $F22A
  LDA $6889
  AND #$03
  BNE $A7BD
  LDA $6885
  AND #$03
  JSR FlashCharacterSprite
  LDA $6889
  AND #$03
  BEQ $A7C9
  LDA #$21
  JMP $AA07
  LDA #$00
  STA $686B
  STA $686A
  STA $6882
  STA $6883
  LDA $6858
  STA $6BAD
@HitLoop:
  JSR $A65A
  STA $6860
  LDX #$C8
  JSR RandAX
  STA $685E
  LDA $685E
  CMP #$C8
  BNE $A7F5
  JMP @NextHitIteration
  LDX $6BAD
  LDA #$00
  JSR RandAX
  CLC
  ADC $6BAD
  STA $6858
  BCC $A80B
  LDA #$FF
  STA $6858
  LDA $6856
  CMP $685E
  BCC @Miss
  LDA $6858
  STA $6860
  LDA #$05
  LDX $6879
  JSR MathBuf_Sub
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
  NOP
  NOP
  NOP
  NOP
  NOP
  NOP
  NOP
  LDA $6860
  BNE $A83B
  INC $6860
  INC $686A
@Miss:
  LDA $6862
  CMP $685E
  BCC $A851
  LDA #$05
  LDX $6858
  JSR MathBuf_Add
  INC $686B
  LDA #$16
  LDX #$16
  LDY #$05
  JSR MathBuf_Add16
  LDA #$DF
  AND $6889
  STA $6889
  LDA $6887
  BEQ @NextHitIteration
  LDA $6873
  BEQ @NextHitIteration
  LDA $6856
  CMP $685E
  BCC @NextHitIteration
  LDA #$64
  STA $6864
  LDA $687B
  AND $686E
  NOP
  NOP
  NOP
  NOP
  NOP
  NOP
  BEQ $A88C
  LDA #$00
  STA $6864
  LDA #$07
  LDX $687A
  JSR MathBuf_Sub
  NOP
  NOP
  NOP
  NOP
  NOP
  NOP
  LDA $6864
  BNE $A8A2
  INC $6864
  LDA #$00
  LDX #$C8
  JSR RandAX
  STA $6866
  CMP #$C8
  BEQ @NextHitIteration
  LDA $6864
  CMP $6866
  BCC @NextHitIteration
  LDA $6889
  EOR #$FF
  AND $6873
  JSR $A988
  LDA $6889
  ORA $6873
  STA $6889
@NextHitIteration:
  DEC $685A
  BEQ $A8D4
  JMP @HitLoop
  LDA $686A
  CMP #$02
  BCC $A901
  STA $6B0B
  LDA #$11
  STA $6B0A
  LDA #$00
  STA $6B0C
  STA $6B0F
  LDA #$0F
  STA $6B0D
  LDA #$2B
  STA $6B0E
  LDA #$01
  LDX #$0A
  LDY #$6B
  JSR DrawCombatBox_L
  INC $6AF8
  LDA $6882
  ORA $6883
  BNE $A91A
  LDA #$0F
  STA $6B2A
  LDA #$40
  STA $6B2B
  LDA #$00
  STA $6B2C
  BEQ @OutputDamageBox
  LDA #$11
  STA $6B2A
  LDA $6882
  STA $6B2B
  LDA $6883
  STA $6B2C
  LDA #$0F
  STA $6B2D
  LDA #$2E
  STA $6B2E
  LDA #$00
  STA $6B2F
@OutputDamageBox:
  LDA #$03
  LDX #$2A
  LDY #$6B
  JSR DrawCombatBox_L
  INC $6AF8
  LDA $686B
  BEQ $A953
  LDA $2C
  JSR DrawBattleMessageCombatBox
  JSR RespondDelay
  LDA #$13
  LDX #$13
  LDY #$16
  JSR MathBuf_Sub16
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
  NOP
  NOP
  NOP
  NOP
  NOP
  NOP
  LDA $687C
  BNE DoPhysicalAttack_Exit
  LDA #$01
  STA $6889
  LDA $6887
  BEQ $A980
  LDA #$20
  BNE @DeathMsg
  LDA #$3F
@DeathMsg:
  JSR DrawBattleMessageCombatBox
DoPhysicalAttack_Exit:
  JMP RespondDelay_ClearCombatBoxes


Sleep Removed After All Hits
DoPhysicalAttack:
  LDA #$00
  TAX
  LDY #$02
  JSR PrepareAndDrawSimpleCombatBox
  LDA #$02
  LDY #$03
  LDX #$00
  JSR PrepareAndDrawSimpleCombatBox
  JSR $A65A
  LDA #$A8
  STA $6856
  LDA $686C
  STA $6858
  LDA $6872
  STA $6862
  LDA $6888
  AND #$08
  BEQ $A6B0
  LDA $6856
  SEC
  SBC #$28
  STA $6856
  LDA $6889
  AND #$08
  BEQ $A6C0
  LDA $6856
  CLC
  ADC #$28
  STA $6856
  LDA $686D
  AND $6876
  STA $685C
  LDA $686E
  AND $6877
  STA $685E
  LDA $685C
  ORA $685E
  BEQ $A700
  LDA #$00
  LDX #$28
  JSR MathBuf_Add
  LDY $6856
  LDX $6857
  JSR $A4AA
  STY $6856
  STX $6857
  LDA $6858
  CLC
  ADC #$1E
  STA $6858
  BCC $A700
  LDA #$FF
  STA $6858
  LDA $6889
  AND #$30
  BEQ @DefenderMobile
  LDA $6858
  LSR A
  LSR A
  CLC
  ADC $6858
  STA $6858
  BCC $A74B
  LDA #$FF
  STA $6858
  JMP $A74B
@DefenderMobile:
  LDA #$00
  LDX $686F
  JSR MathBuf_Add
  LDY $6856
  LDX $6857
  JSR $A4AA
  STY $6856
  STX $6857
  LDA #$00
  LDX $6878
  JSR MathBuf_Sub
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
  NOP
  NOP
  NOP
  NOP
  LDA $6856
  BNE $A753
  INC $6856
  LDA $6862
  CMP $6856
  BCC $A761
  LDA $6856
  STA $6862
  LDA $6871
  LDX $6870
  JSR MultiplyXA
  STA $685A
  LDA $685A
  BNE $A775
  INC $685A
  LDA $6881
  LDX #$00
  JSR $AB2F
  LDA $6887
  BNE $A7A6
  LDX $6880
  BNE $A78A
  INC $6AEE
  LDX $6880
  LDY #$00
  LDA $6884
  JSR $9E70
  LDA $6889
  AND #$03
  BNE $A79F
  JSR $BBFA
  LDA #$00
  STA $6AEE
  BEQ $A7BD
  LDA #$01
  JSR $BEB8
  JSR $F22A
  LDA $6889
  AND #$03
  BNE $A7BD
  LDA $6885
  AND #$03
  JSR FlashCharacterSprite
  LDA $6889
  AND #$03
  BEQ $A7C9
  LDA #$21
  JMP $AA07
  LDA #$00
  STA $686B
  STA $686A
  STA $6882
  STA $6883
  LDA $6858
  STA $6BAD
@HitLoop:
  JSR $A65A
  STA $6860
  LDX #$C8
  JSR RandAX
  STA $685E
  LDA $685E
  CMP #$C8
  BNE $A7F5
  JMP @NextHitIteration
  LDX $6BAD
  LDA #$00
  JSR RandAX
  CLC
  ADC $6BAD
  STA $6858
  BCC $A80B
  LDA #$FF
  STA $6858
  LDA $6856
  CMP $685E
  BCC @Miss
  LDA $6858
  STA $6860
  LDA #$05
  LDX $6879
  JSR MathBuf_Sub
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
  NOP
  NOP
  NOP
  NOP
  NOP
  NOP
  NOP
  LDA $6860
  BNE $A83B
  INC $6860
  INC $686A
@Miss:
  LDA $6862
  CMP $685E
  BCC $A851
  LDA #$05
  LDX $6858
  JSR MathBuf_Add
  INC $686B
  LDA #$16
  LDX #$16
  LDY #$05
  JSR MathBuf_Add16
  LDA $6887
  BEQ @NextHitIteration
  LDA $6873
  BEQ @NextHitIteration
  LDA $6856
  CMP $685E
  BCC @NextHitIteration
  LDA #$64
  STA $6864
  LDA $687B
  AND $686E
  NOP
  NOP
  NOP
  NOP
  NOP
  NOP
  BEQ $A884
  LDA #$00
  STA $6864
  LDA #$07
  LDX $687A
  JSR MathBuf_Sub
  NOP
  NOP
  NOP
  NOP
  NOP
  NOP
  LDA $6864
  BNE $A89A
  INC $6864
  LDA #$00
  LDX #$C8
  JSR RandAX
  STA $6866
  CMP #$C8
  BEQ @NextHitIteration
  LDA $6864
  CMP $6866
  BCC @NextHitIteration
  LDA $6889
  EOR #$FF
  AND $6873
  JSR $A988
  LDA $6889
  ORA $6873
  STA $6889
@NextHitIteration:
  DEC $685A
  BEQ $A8CC
  JMP @HitLoop
  LDA $686A
  CMP #$02
  BCC $A8F9
  STA $6B0B
  LDA #$11
  STA $6B0A
  LDA #$00
  STA $6B0C
  STA $6B0F
  LDA #$0F
  STA $6B0D
  LDA #$2B
  STA $6B0E
  LDA #$01
  LDX #$0A
  LDY #$6B
  JSR DrawCombatBox_L
  INC $6AF8
  LDA $6882
  ORA $6883
  BNE $A912
  LDA #$0F
  STA $6B2A
  LDA #$40
  STA $6B2B
  LDA #$00
  STA $6B2C
  BEQ @OutputDamageBox
  LDA #$DF
  AND $6889
  STA $6889
  LDA #$11
  STA $6B2A
  LDA $6882
  STA $6B2B
  LDA $6883
  STA $6B2C
  LDA #$0F
  STA $6B2D
  LDA #$2E
  STA $6B2E
  LDA #$00
  STA $6B2F
@OutputDamageBox:
  LDA #$03
  LDX #$2A
  LDY #$6B
  JSR DrawCombatBox_L
  INC $6AF8
  LDA $686B
  BEQ $A953
  LDA $2C
  JSR DrawBattleMessageCombatBox
  JSR RespondDelay
  LDA #$13
  LDX #$13
  LDY #$16
  JSR MathBuf_Sub16
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
  NOP
  NOP
  NOP
  NOP
  NOP
  NOP
  LDA $687C
  BNE DoPhysicalAttack_Exit
  LDA #$01
  STA $6889
  LDA $6887
  BEQ $A980
  LDA #$20
  BNE @DeathMsg
  LDA #$3F
@DeathMsg:
  JSR DrawBattleMessageCombatBox
DoPhysicalAttack_Exit:
  JMP RespondDelay_ClearCombatBoxes
