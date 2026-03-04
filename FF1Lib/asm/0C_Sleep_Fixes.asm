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