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

[$B1D4 :: 0x331D4]
  LDY #$en_hp                       ; Changed Index to use correct enemy HP

[$B1E1 :: 0x331E1]
  LDA #$00                          ; Starting value for RNG
  LDX #$50                          ; Ending value for RNG
 
[$B1E9 :: 0x331E9]
  LDA #$00                          ; Index for math buffer 
  JSR MathBuf_Sub                   ; mathbuf -= random
                                    ; Removed unneeded load
  BEQ @EnemySleep                   ; Changed branch to check zero flag set by MathBuf_Sub
                                    ; Branches to sleep if zero flag is set
  LDA #~AIL_SLEEP
  JSR ApplyEnemyAilmentMask
  LDA #ALTBTLMSG_WOKEUP
  BNE @PrintAndEnd
@EnemySleep
  LDA #ALTBTLMSG_SLEEPING
  NOP
  NOP
  NOP
@PrintAndEnd:
  JMP ShowAltBattleMessage_ClearAllBoxes

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

[$AE5D :: 0x32E5D]
  STA $68AF                         ; temp location for low RNG limit
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

[$A444 :: 0x32444]
  LDA #$00                          ; Starting value for RNG
  LDX #$50                          ; Ending value for RNG

[$A451 :: 0x32451]
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
  LDA #BTLMSG_SLEEPING
  JMP DrawBtlMsg_ClearCombatBoxes
@PlayerWake



Common to all sleep only removed on hit
;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;
;;
;;  Battle_DoPlayerTurn  [$A357 :: 0x32367]
;;
;;  Remove the call to random player wake up
;;
;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;

[$A3B1 :: 0x323B1]
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

[$B1BC :: 0x331BC]
  NOP
  NOP
  NOP
  NOP

;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;
;;
;;  DoPhysicalAttack  [$A67B :: 0x3268B]
;;
;;  Removed unneeded code
;;
;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;

[$A73C :: 0x3273C]
  NOP                                       ;
  NOP                                       ;
  NOP                                       ;
  NOP                                       ;
  NOP                                       ;
  NOP                                       ;
  NOP                                       ;
  NOP                                       ; remove all the excess 0 capping
  NOP                                       ; in @DefenderMobile block
  NOP                                       ;
  NOP                                       ;
  NOP                                       ;
  NOP                                       ;
  NOP                                       ;
  NOP                                       ;

[$A821 :: 0x32821]
  NOP                                       ;
  NOP                                       ;
  NOP                                       ;
  NOP                                       ;
  NOP                                       ;
  NOP                                       ;
  NOP                                       ;
  NOP                                       ;
  NOP                                       ; remove all the excess 0 capping
  NOP                                       ; in base dmg calculation
  NOP                                       ;
  NOP                                       ;
  NOP                                       ;
  NOP                                       ;
  NOP                                       ;
  NOP                                       ;
  NOP                                       ;
  NOP                                       ;
  LDA $6860                                 ; load dmgcalc



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

[$A85A :: 0x3285A]
  LDA #~AIL_SLEEP                           ; load sleep ailment
  AND btl_defender_ailments                 ; compare ailment to player ailments
  STA btl_defender_ailments                 ; remove sleep
  LDA $6887                                 ; check attacker type (player/enemy)
  BEQ @NextHitIteration                     ; start next hit if player
  LDA btl_attacker_attackailment            ; check for attacker ailment applications
  BEQ @NextHitIteration                     ; start next hit if no ailment to apply
  LDA battle_hitsconnected                  ; hits connected
  BEQ @NextHitIteration                     ; start next hit if missed (bugged, but not taken due to prior fix)
  LDA #$64                                  ;
  STA math_ailmentchance                    ; load and store base ailment chance 
  LDA btl_defender_elemresist               ; load defender elemental resist
  AND btl_attacker_element                  ; compare resist to attacker element
  BEQ $A887
  LDA #$00                                  ; if resisted set ailment chance to 0
  STA math_ailmentchance
  LDA #MATHBUF_AILMENTCHANCE
  LDX btl_defender_magdef                   ; subtract defender mdef from ailment chance
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
  LDA math_ailmentchance



Sleep Removed After All Hits
;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;
;;
;;  DoPhysicalAttack  [$A67B :: 0x3268B]
;;
;;  Removed unneeded code
;;  Corrected hit loop to only apply touch effects on hit
;;  Added Remove sleep on hit
;;
;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;

[$A874 :: 0x32874]
  NOP
  NOP
  NOP                                       ; remove useless store and load
  NOP
  NOP
  NOP

[$A889 :: 0x32889]
  NOP
  NOP
  NOP
  NOP
  NOP
  NOP
  NOP
  NOP                                       ; remove all the excess 0 capping
  NOP                                       ; in ailment chance calculation
  NOP
  NOP
  NOP
  NOP
  NOP
  NOP
  LDA math_ailmentchance
 
[$A7F2 :: 0x327F2]
  JMP @NextHitIteration                     ; Update NextHitIteration in HitLoop

[$A85D :: 0x3285D]
  BEQ @NextHitIteration                     ; Update NextHitIteration in miss block

[$A862 :: 0x32862]
  BEQ @NextHitIteration                     ; Update NextHitIteration in miss block

[$A867 :: 0x32867]
  BEQ @NextHitIteration                     ; Update NextHitIteration in miss block

[$A892 :: 0x32892]
  LDA math_ailmentchance                    ;
  BNE $A89A                                 ;
  INC math_ailmentchance                    ;
  LDA #$00                                  ;
  LDX #$C8                                  ;
  JSR RandAX                                ;
  STA battle_ailmentrandchance              ;
  CMP #$C8                                  ;
  BEQ @NextHitIteration                     ;
  LDA math_ailmentchance                    ;
  CMP battle_ailmentrandchance              ; Moved up 8 bytes to accomadate sleep removal
  BCC @NextHitIteration                     ;
  LDA btl_defender_ailments                 ;
  EOR #$FF                                  ;
  AND btl_attacker_attackailment            ;
  JSR PrintPlayerAilmentMessageFromAttack   ;
  LDA btl_defender_ailments                 ;
  ORA btl_attacker_attackailment            ;
  STA btl_defender_ailments                 ;
@NextHitIteration:
  DEC math_numhits                          ;
  BEQ $A8CC                                 ;
  JMP @HitLoop                              ;
  LDA #~AIL_SLEEP                           ; load sleep ailment
  AND btl_defender_ailments                 ; compare ailment to player ailments
  STA btl_defender_ailments                 ; remove sleep
