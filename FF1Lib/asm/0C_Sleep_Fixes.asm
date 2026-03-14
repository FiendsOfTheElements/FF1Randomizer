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

  NOP                                       ;  0C A73C
  NOP                                       ;
  NOP                                       ;
  NOP                                       ;
  NOP                                       ;
  NOP                                       ;
  NOP                                       ;
  NOP                                       ; remove all the excess 0 capping
  NOP                                       ; 15 bytes
  NOP                                       ;
  NOP                                       ;
  NOP                                       ;
  NOP                                       ;
  NOP                                       ;
  NOP                                       ; 0C A74A
  
  NOP                                       ; 0C A821
  NOP                                       ;
  NOP                                       ;
  NOP                                       ;
  NOP                                       ;
  NOP                                       ;
  NOP                                       ; 21 bytes
  NOP                                       ;
  NOP                                       ; remove all the excess 0 capping
  NOP                                       ;
  NOP                                       ;
  NOP                                       ;
  NOP                                       ;
  NOP                                       ;
  NOP                                       ;
  NOP                                       ;
  NOP                                       ;
  NOP                                       ;
  LDA $6860                                 ; load dmgcalc  ; 0C A835

  LDA #$DF                                  ; load sleep ailment      ; 0C  A85A
  AND $6889                                 ; compare ailment to player ailments
  STA $6889                                 ; remove sleep            ; 0C  A861
  LDA $6887                                 ; check attacker type (player/enemy)
  BEQ $A8CC                                 ; start next hit if player
  LDA $6873                                 ; check for attacker ailment applications
  BEQ $A8CC                                 ; start next hit if no ailment to apply
  LDA $686A                                 ; hits connected
  BEQ $A8CC                                 ; start next hit if missed (bugged, but not taken due to prior fix)
  LDA #$64                                  ;
  STA $6864                                 ; load and store base ailment chance 
  LDA $687B                                 ; load defender elemental resist
  AND $686E                                 ; compare resist to attacker element
  BEQ $A887
  LDA #$00                                  ; if resisted set ailment chance to 0
  STA $6864
  LDA #$07
  LDX $687A                                 ; subtract defender mdef from ailment chance
  JSR MathBuf_Sub
  NOP
  NOP
  NOP
  NOP
  NOP                                       ; 67 bytes
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
  LDA $6864                                 ; 0C A89A

  NOP                                       ; 0C  A95C
  NOP                                       ;
  NOP                                       ;
  NOP                                       ;
  NOP                                       ;
  NOP                                       ; 22 bytes
  NOP                                       ;
  NOP                                       ; removed unnecessary 0 capping
  NOP                                       ;
  NOP                                       ;
  NOP                                       ;
  NOP                                       ;
  NOP                                       ;
  NOP                                       ;
  NOP                                       ;
  NOP                                       ;
  NOP                                       ;
  LDA $687D                                 ; load HP high byte
  BNE DoPhysicalAttack_Exit                 ; if not zero skip death ailment
  LDA $687C                                 ; load HP low byte
  BNE DoPhysicalAttack_Exit                 ; if not zero skip death ailment    0C A971






Sleep Removed After All Hits
DoPhysicalAttack:
  LDA #$00                                  ;
  TAX                                       ; Print attacker name to
  LDY #$02                                  ; attacker combat box
  JSR PrepareAndDrawSimpleCombatBox

  LDA #$02                                  ;
  LDY #$03                                  ; Print defender name to
  LDX #$00                                  ; defender combat box
  JSR PrepareAndDrawSimpleCombatBox

  JSR $A65A                                 ; zero high bytes of math buffers
  LDA #$A8                                  ; base hit chance of 168
  STA $6856                                 ; store hit chance
  LDA $686C                                 ; base damage of attacker's strength value
  STA $6858                                 ; store base damage
  LDA $6872                                 ; base crit chance of attacker's crit rate
  STA $6862                                 ; store crit chance
  
  LDA $6888                                 ; load attacker ailments
  AND #$08                                  ; check for dark

  BEQ $A6B0                                 ; if dark
  LDA $6856                                 ;
  SEC                                       ; reduce attackers hit chance by 40
  SBC #$28                                  ;
  STA $6856                                 ;
 
  LDA $6889                                 ; load defender ailments
  AND #$08                                  ; check for dark

  BEQ $A6C0                                 ; if dark
  LDA $6856                                 ;
  CLC                                       ; increase attackers hit chance by 40
  ADC #$28                                  ;
  STA $6856                                 ;

  LDA $686D                                 ;
  AND $6876                                 ; check if attacker category matches defender category 
  STA $685C                                 ;

  LDA $686E                                 ;
  AND $6877                                 ; check if attacker element matches defender elemental weakness
  STA $685E                                 ;

  LDA $685C                                 ;
  ORA $685E                                 ; merge category and weakness

  BEQ $A700                                 ; if weakness found
  LDA #$00                                  ;
  LDX #$28                                  ;
  JSR MathBuf_Add                           ;
  LDY $6856                                 ; increase attackers hit chance by 40 
  LDX $6857                                 ; capping at 255
  JSR $A4AA                                 ;
  STY $6856                                 ;
  STX $6857                                 ;

  LDA $6858                                 ; 
  CLC                                       ;
  ADC #$1E                                  ; add +4 bonus to base damage
  STA $6858                                 ; capping at 255
  BCC $A700                                 ;
  LDA #$FF                                  ;
  STA $6858                                 ;

  LDA $6889                                 ; load ailments
  AND #$30                                  ; check defender for stun and/or sleep
  BEQ @DefenderMobile                       ; 
  LDA $6858                                 ;
  LSR A                                     ;
  LSR A                                     ; apply 25% bonus to base damage
  CLC                                       ;
  ADC $6858                                 ;
  STA $6858                                 ;
  BCC $A74B                                 ; skip defender block
  LDA #$FF                                  ;
  STA $6858                                 ; cap at 255 base damage
  JMP $A74B                                 ; skip defender block
@DefenderMobile:
  LDA #$00                                  ;
  LDX $686F                                 ;
  JSR MathBuf_Add                           ;
  LDY $6856                                 ;
  LDX $6857                                 ;
  JSR $A4AA                                 ; remove defender evade from attacker hit
  STY $6856                                 ;
  STX $6857                                 ;
  LDA #$00                                  ;
  LDX $6878                                 ;
  JSR MathBuf_Sub                           ;
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
  LDA $6856                                 ;
  BNE $A753                                 ; minimum hit chance of 1
  INC $6856                                 ;

  LDA $6862                                 ;
  CMP $6856                                 ;
  BCC $A761                                 ; caps crit chance to hit chance
  LDA $6856                                 ;
  STA $6862                                 ;

  LDA $6871                                 ;
  LDX $6870                                 ; load attacker number of hits
  JSR MultiplyXA                            ; fast mult
  STA $685A                                 ;

  LDA $685A                                 ;
  BNE $A775                                 ; minimum 1 hit
  INC $685A                                 ;

  LDA $6881                                 ;
  LDX #$00                                  ; palette stuff
  JSR $AB2F                                 ;
  
  LDA $6887                                 ;
  BNE $A7A6                                 ; jump to enemy attacking

@PlayerAttackingEnemy:
  LDX $6880                                 ;
  BNE $A78A                                 ;
  INC $6AEE                                 ;
  LDX $6880                                 ; weapon draw and walk forward animation
  LDY #$00                                  ;
  LDA $6884                                 ;
  JSR $9E70                                 ;

  LDA $6889                                 ;
  AND #$03                                  ;
  BNE $A79F                                 ; check for dead/stone
  JSR $BBFA                                 ; play sound otherwise
  LDA #$00                                  ; and clear nodraw flag
  STA $6AEE                                 ;
  BEQ $A7BD                                 ;
@EnemyAttackingPlayer:
  LDA #$01
  JSR $BEB8                                 ; sound effect
  JSR $F22A                                 ; screen shake animation

  LDA $6889                                 ;
  AND #$03                                  ; load defender ailments
  BNE $A7BD                                 ; if not dead/stoned
  LDA $6885                                 ; flash sprite
  AND #$03                                  ;
  JSR FlashCharacterSprite                  ;

  LDA $6889                                 ;
  AND #$03                                  ; if defender dead
  BEQ $A7C9                                 ; draw the "Ineffective" battle message,
  LDA #$21                                  ; clear all combat boxes, and exit
  JMP $AA07                                 ;
  
  LDA #$00                                  ;
  STA $686B                                 ;
  STA $686A                                 ; stat tracking
  STA $6882                                 ;
  STA $6883                                 ;

  LDA $6858                                 ;
  STA $6BAD                                 ; store base damage elsewhere due to overwriting
@HitLoop:
  JSR $A65A                                 ; clear math buffer
  STA $6860                                 ; zero calc
  LDX #$C8                                  ;
  JSR RandAX                                ;
  STA $685E                                 ; generate random hit
  LDA $685E                                 ;
  CMP #$C8                                  ; if 200 skip
  BNE $A7F5                                 ;
  JMP @NextHitIteration                     ;

  LDX $6BAD                                 ;
  LDA #$00                                  ;
  JSR RandAX                                ;
  CLC                                       ; random base damage between
  ADC $6BAD                                 ; basedmg and 2xbasedmg
  STA $6858                                 ;
  BCC $A80B                                 ;
  LDA #$FF                                  ; max at 255
  STA $6858                                 ;

  LDA $6856                                 ;
  CMP $685E                                 ; hit_chance < randhit = miss
  BCC A7F2                                 ;

  LDA $6858                                 ;
  STA $6860                                 ;
  LDA #$05                                  ; subtract absorb from basedmg
  LDX $6879                                 ;
  JSR MathBuf_Sub                           ;
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
  LDA $6860                                 ; load dmgcalc  ; 0C A835
  BNE $A83B                                 ; check if 0
  INC $6860                                 ; minimum 1
  INC $686A                                 ; increment hit counter
@Miss:
  LDA $6862                                 ;
  CMP $685E                                 ; crit_chance < randhit = no crit
  BCC $A851                                 ;

  LDA #$05                                  ;
  LDX $6858                                 ; if crit add random basedmg again
  JSR MathBuf_Add                           ;
  INC $686B                                 ; increment crit counter

  LDA #$16                                  ;
  LDX #$16                                  ; calculate total damage
  LDY #$05                                  ;
  JSR MathBuf_Add16                         ;
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
  BNE $A8A2                                 ; minimum 1 ailment chance
  INC $6864                                 ;

  LDA #$00                                  ;
  LDX #$C8                                  ;
  JSR RandAX                                ; random ailment chance value
  STA $6866                                 ;

  CMP #$C8                                  ;
  BEQ @NextHitIteration                     ; skip if rand value is 200

  LDA $6864                                 ;
  CMP $6866                                 ; if ailment_chance < rand value    ; already patched remove this
  BCC @NextHitIteration                     ; start next hit

  LDA $6889                                 ;
  EOR #$FF                                  ;
  AND $6873                                 ;
  JSR $A988                                 ; apply ailment
  LDA $6889                                 ;
  ORA $6873                                 ;
  STA $6889                                 ;
@NextHitIteration:
  DEC $685A                                 ; decrement remaining hits
  BEQ $A8D4                                 ; if 0 exit hit loop
  JMP @HitLoop                              ; otherwise start next hit

  LDA $686A                                 ;
  CMP #$02                                  ;
  BCC $A901                                 ;
  STA $6B0B                                 ; 
  LDA #$11                                  ;
  STA $6B0A                                 ;
  LDA #$00                                  ;
  STA $6B0C                                 ;
  STA $6B0F                                 ; if hits > 1 display # of hits
  LDA #$0F                                  ;
  STA $6B0D                                 ;
  LDA #$2B                                  ;
  STA $6B0E                                 ;
  LDA #$01                                  ;
  LDX #$0A                                  ;
  LDY #$6B                                  ;
  JSR DrawCombatBox_L                       ;
  INC $6AF8                                 ;

  LDA $6882                                 ;
  ORA $6883                                 ;
  BNE $A91A                                 ;
  LDA #$0F                                  ;
  STA $6B2A                                 ;
  LDA #$40                                  ;
  STA $6B2B                                 ;
  LDA #$00                                  ;
  STA $6B2C                                 ;
  BEQ @OutputDamageBox                      ; Disply missed if 0 dmg or amount
  LDA #$11                                  ; if non-zero
  STA $6B2A                                 ;
  LDA $6882                                 ;
  STA $6B2B                                 ;
  LDA $6883                                 ;
  STA $6B2C                                 ;
  LDA #$0F                                  ;
  STA $6B2D                                 ;
  LDA #$2E                                  ;
  STA $6B2E                                 ;
  LDA #$00                                  ;
  STA $6B2F                                 ;

@OutputDamageBox:
  LDA #$03                                  ;
  LDX #$2A                                  ;
  LDY #$6B                                  ;
  JSR DrawCombatBox_L                       ;
  INC $6AF8                                 ; Draw missed/dmg/crits
  LDA $686B                                 ;
  BEQ $A953                                 ;
  JSR DrawBattleMessageCombatBox            ;
  NOP
  NOP
  JSR RespondDelay                          ;

  LDA #$13                                  ;
  LDX #$13                                  ; HP -= totaldmg
  LDY #$16                                  ; already caps at 0
  JSR MathBuf_Sub16                         ;
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
  LDA #$01                                  ;
  STA $6889                                 ;
  LDA $6887                                 ;
  BEQ $A980                                 ; apply dead ailment
  LDA #$20                                  ; if player print slain message
  BNE @DeathMsg                             ;
  LDA #$3F                                  ;
@DeathMsg:
  JSR DrawBattleMessageCombatBox            ;
DoPhysicalAttack_Exit:
  JMP RespondDelay_ClearCombatBoxes         ;
