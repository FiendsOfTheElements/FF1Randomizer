SwapPRG = $FE03
BtlMag_IntPatchRedirect = $A290

;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;
;BtlMag_Effect_AttackUp (never used by game so we're overwriting it)
;  [$BA00 :: 0x33A10 (includes header)]
; using this space to redirect
; Int fixes overwrite entries in jumptable_MagicEffect to point here
;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;
;bank 0B
.org $BA00
    ;Jump to new routine in bank 1C
    LDA #>(BtlMag_IntPatchRedirect-1)
	PHA
	LDA #<(BtlMag_IntPatchRedirect-1)
	PHA
	LDA #$1C ;bank with Retarget function
    JMP SwapPRG

;assembled bytes
;A9 A2 48 A9 8F 48 A9 1C 4C 03 FE


;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;
;routine pointers
    SwapPRG =                           $FE03
    BtlMag_Effect_Damage_Return =       $B89F
    BtlMag_Effect_DamageUndead_Return = $B908
    BtlMag_Effect_Ailment_Return =      $B932
    BtlMag_Effect_Slow_Return =         $B961
    BtlMag_Effect_LowerMorale_Return =  $B97C
    BtlMag_Effect_RecoverHP_Return =    $B9BA
    BtlMag_Effect_EvadeDown_Return =    $BA49
    BtlMag_Effect_RemoveResist_Return = $BA97
    BtlMag_Effect_Ailment2_Return =     $BAE9
    BtlMag_PrepHitAndDamage =           $B82D
    BattleRNG_L =                       $F227
;ram pointers
    btltmp =                          $90
    btl_attacker =                  $6C89 ;low bytes hold the casting player index, high byte is player yes/no
    ch_int =                        $6112 ;increase by $40 per character
    btl_mathbuf =                   $6856
    math_hitchance =                $6856
    math_magrandhit =               $685A
    btlmag_spellconnected =         $685C
    btlmag_effect =                 $686E
    btlmag_element =                $6878
    btlmag_defender_elemweakness =  $6876
    btlmag_defender_elemresist =    $6877
    btlmag_defender_ailments =      $686D
    btlmag_effectivity =            $6874
    btltmp_multA =                  $68B3    ; shared
    btltmp_multB =                  $68B4    ; shared
    btltmp_multC =                  $68B5
;constants
    AIL_DEAD =                    $01
    MATHBUF_HITCHANCE =           $0
    MATHBUF_MAGRANDHIT =          $2
    MATHBUF_MAGDEFENDERHP =       $12

;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;
;new bank code
; $A290 addr :: $722A0 rom (includes header)
;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;
.org $A290
BtlMag_IntPatchRedirect:
        LDA btlmag_effect ;get spell's effect ID

        ; this is the same jump table idea as the one we're patching from 
        ; BtlMag_PerformSpellEffect in bank 0C
        ASL A
        TAX
        LDA jumptable_PatchedMagicEffect, X
        STA btltmp+6
        LDA jumptable_PatchedMagicEffect+1, X
        STA btltmp+7                    ; pointer to spell logic in btltmp+6
        CLC
        JMP (btltmp+6)      ; call the routine from our jump table
        
jumptable_PatchedMagicEffect:
        .WORD BtlMag_IntPatchRedirect ;reliably hang if we come here with btlmag_effect == 0
        .WORD SpellEffectDamage
        .WORD SpellEffectDamageUndead
        .WORD SpellEffectAilment
        .WORD SpellEffectSlow
        .WORD SpellEffectLowerMorale
        .WORD SpellEffectRecoverHP
        .WORD SpellEffectRecoverHP ; RecoverHP copy
        .WORD 0 ; CureAilment
        .WORD 0 ; AbsorbUp
        .WORD 0 ; ElemResist
        .WORD 0 ; AttackUp
        .WORD 0 ; Fast
        .WORD 0 ; AttackUp2
        .WORD SpellEffectEvadeDown
        .WORD 0 ; CureAll
        .WORD 0 ; EvadeUp
        .WORD SpellEffectRemoveResist
        .WORD SpellEffectInflictAilment2 ; (power word)

SpellEffectDamage:
        LDA #1
        STA btlmag_spellconnected ;first mark spell as connected (original routine does this before the point we're returning to)
        JSR BtlMag_InitBaseHitChance ;load default hit chance into math buffer

        LDA btl_attacker ;check if attacker is player
        AND #$80
        BEQ RetFromDamage ;if not, we're done

        JSR GetIntInX 
        LDA #MATHBUF_HITCHANCE
        JSR MathBuf_Add ;add int to hit chance math buffer
        ;swap bank back and return into BtlMag_Effect_Damage
    RetFromDamage:
        LDA #>(BtlMag_Effect_Damage_Return-1)
        PHA
        LDA #<(BtlMag_Effect_Damage_Return-1)
        PHA
        LDA #$0C ;bank with original function
        JMP SwapPRG

SpellEffectDamageUndead:
        JSR BtlMag_InitBaseHitChance

        ;for completeness, very likely not needed
        LDA btl_attacker ;check if attacker is player
        AND #$80
        BEQ RetFromDamageUndead ;if not, we're done

        JSR GetIntInX 
        LDA #MATHBUF_HITCHANCE
        JSR MathBuf_Add ;add int to hit chance math buffer
    RetFromDamageUndead:
        LDA #>(BtlMag_Effect_DamageUndead_Return-1)
        PHA
        LDA #<(BtlMag_Effect_DamageUndead_Return-1)
        PHA
        LDA #$0C ;bank with original function
        JMP SwapPRG

SpellEffectAilment:
        JSR BtlMag_CalcElemHitChance_IntPatched

        ;swap bank back and return into BtlMag_Effect_Ailment
        LDA #>(BtlMag_Effect_Ailment_Return-1)
        PHA
        LDA #<(BtlMag_Effect_Ailment_Return-1)
        PHA
        ;ailment routine is expecting to have gone here first
        LDA #>(BtlMag_PrepHitAndDamage-1)
        PHA
        LDA #<(BtlMag_PrepHitAndDamage-1)
        PHA
        LDA #$0C ;bank with original function
        JMP SwapPRG

SpellEffectSlow:
        JSR BtlMag_CalcElemHitChance_IntPatched

        ;swap bank back and return into BtlMag_Effect_Slow
        LDA #>(BtlMag_Effect_Slow_Return-1)
        PHA
        LDA #<(BtlMag_Effect_Slow_Return-1)
        PHA
        ;slow routine is expecting to have gone here first
        LDA #>(BtlMag_PrepHitAndDamage-1)
        PHA
        LDA #<(BtlMag_PrepHitAndDamage-1)
        PHA
        LDA #$0C ;bank with original function
        JMP SwapPRG

SpellEffectLowerMorale:
        JSR BtlMag_CalcElemHitChance_IntPatched
        ;swap bank back and return into BtlMag_Effect_LowerMorale
        LDA #>(BtlMag_Effect_LowerMorale_Return-1)
        PHA
        LDA #<(BtlMag_Effect_LowerMorale_Return-1)
        PHA
        ;lower morale routine is expecting to have gone here first
        LDA #>(BtlMag_PrepHitAndDamage-1)
        PHA
        LDA #<(BtlMag_PrepHitAndDamage-1)
        PHA
        LDA #$0C ;bank with original function
        JMP SwapPRG

SpellEffectRecoverHP_RTS:
        LDA #$0C ;routine was already set up to return
        JMP SwapPRG
SpellEffectRecoverHP:
        LDA #1
        STA btlmag_spellconnected       ; set spellconnected var to nonzero to indicate it connected
    
        LDA btlmag_defender_ailments    ; Check defender ailment
        AND #AIL_DEAD
        BNE SpellEffectRecoverHP_RTS      ; If they're dead, do nothing
        LDA btlmag_effectivity
        STA btltmp+6 ;save original effectivity

        LDA btl_attacker ;check if attacker is player
        AND #$80
        BEQ RecoverHPContinue ;if not, skip

        JSR GetIntInX
        TXA
        ADC btlmag_effectivity ;add randomness to effectivity, increasing the maximum heal amount but not minimum
        BCC :+
        LDA #$FF                      ;   (cap at 255)
        : 
        STA btlmag_effectivity ; save new effectivity
    RecoverHPContinue:
        TAX
        LDA #0
        JSR RandAX                      ;   random between [0,effectivity+int]
        CLC
        ADC btltmp+6          ;   add back original effectivity, random becomes between [effectivity, (effectivity*2)+int]
        BCC :+
        LDA #$FF                      ;   (cap at 255)
        :   

        TAX
        LDA #MATHBUF_MAGDEFENDERHP      ; Add X to defender's HP
        JSR MathBuf_Add
        
        LDA btltmp+6                    ;restore original effectivity
        STA btlmag_effectivity          ;AOE heal spells do not reload their effectivity per character so we need to restore it

    ;return to LDX #MATHBUF_MAGDEFENDERMAXHP
        LDA #>(BtlMag_Effect_RecoverHP_Return-1)
        PHA
        LDA #<(BtlMag_Effect_RecoverHP_Return-1)
        PHA
        LDA #$0C ;bank with original function
        JMP SwapPRG

SpellEffectEvadeDown:
        JSR BtlMag_CalcElemHitChance_IntPatched
        ;swap bank back and return into BtlMag_Effect_EvadeDown
        LDA #>(BtlMag_Effect_EvadeDown_Return-1)
        PHA
        LDA #<(BtlMag_Effect_EvadeDown_Return-1)
        PHA
        ;evade down routine is expecting to have gone here first
        LDA #>(BtlMag_PrepHitAndDamage-1)
        PHA
        LDA #<(BtlMag_PrepHitAndDamage-1)
        PHA
        LDA #$0C ;bank with original function
        JMP SwapPRG

SpellEffectRemoveResist:
        JSR BtlMag_InitBaseHitChance
        LDA btl_attacker ;check if attacker is player
        AND #$80
        BEQ RetFromRemoveResist ;if not, we're done

        JSR GetIntInX 
        LDA #MATHBUF_HITCHANCE
        JSR MathBuf_Add ;add int to hit chance math buffer
    RetFromRemoveResist:
        ;return to the rest of the remove resist routine
        LDA #>(BtlMag_Effect_RemoveResist_Return-1)
        PHA
        LDA #<(BtlMag_Effect_RemoveResist_Return-1)
        PHA
        LDA #$0C ;bank with original function
        JMP SwapPRG

SpellEffectInflictAilment2_RTS:
        LDA #$0C ;routine was already set up to return
        JMP SwapPRG
SpellEffectInflictAilment2:
        ; this one is a bit unique being based on HP only
        ; so we add INT to the HP threshold of the power word rather
        ; than hit chance which isn't used here
        LDA btlmag_element                  ; If the defender resists the element
        AND btlmag_defender_elemresist
        BNE SpellEffectInflictAilment2_RTS        ; then they are 100% immune.  Branch to RTS

        LDA #<300 ;load the base power word threshold into math buffer
        STA math_magrandhit
        LDA #>300
        STA math_magrandhit+1

        LDA btl_attacker ;check if attacker is player
        AND #$80
        BEQ RetFromInflictAilment2 ;if not, we're done

        JSR GetIntInX ;add int to threshold
        TXA
        ASL A
        TAX
        LDA #MATHBUF_MAGRANDHIT
        JSR MathBuf_Add
    RetFromInflictAilment2:
        ; come out at LDY #MATHBUF_MAGRANDHIT
        ; to run the rest of the power word routine
        LDA #>(BtlMag_Effect_Ailment2_Return-1)
        PHA
        LDA #<(BtlMag_Effect_Ailment2_Return-1)
        PHA
        LDA #$0C ;bank with original function
        JMP SwapPRG


BtlMag_CalcElemHitChance_IntPatched:
        ;BtlMag_CalcElemHitChance is the first thing called by several routines that
        ; we need to patch
        JSR BtlMag_InitBaseHitChance        ; load base hit chance
    
        LDA btlmag_element
        AND btlmag_defender_elemresist
        BEQ :+                              ; if defender resists
        LDA #0                            ; ... reset hit chance to zero
        STA math_hitchance
        STA math_hitchance+1
        JMP RetFromHitChance ;exit out from resist condition
    : 
        LDA btl_attacker ;check if attacker is player
        AND #$80
        BEQ :+ ;if not, skip the int part

        ;add int to hit chance if no resist
        JSR GetIntInX
        LDA #MATHBUF_HITCHANCE
        JSR MathBuf_Add
    :
        LDA btlmag_element                  
        AND btlmag_defender_elemweakness
        BEQ RetFromHitChance    ;exit out from no resist or weakness condition
        LDA #MATHBUF_HITCHANCE              ; if defender is weak to element
        LDX #40                             ;   add +40 to hit chance
        JSR MathBuf_Add
                                            ;exit from weakness condition
    RetFromHitChance:
        RTS

lut_CharClassPtrTable:
        .byte $00
        .byte $40
        .byte $80
        .byte $C0

GetIntInX:
        ;return attacking character's INT in X
        LDA btl_attacker
        AND #$03
        TAX
        LDA lut_CharClassPtrTable,X
        TAX
        LDA ch_int,X
        TAX
        RTS

;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;
; Below here are routines that needed to be copied from bank 0C 
; in order to move code over to bank 1C
;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;

;just loads 148 into the hit chance math buffer
BtlMag_InitBaseHitChance:
    LDA #<148               ; base hit chance of 148
    STA math_hitchance
    LDA #>148
    STA math_hitchance+1
    RTS

RandAX:
    STA $68AF       ; 68AF is the 'lo' value
    INX
    STX $68B0       ; 68B0 is hi+1.  But this STX is totally unnecessary because this value is never used
    
    TXA
    SEC
    SBC $68AF       ; subtract to get the range.
    STA $68B6       ; 68B6 = range
    
    JSR BattleRNG_L
    LDX $68B6
    JSR MultiplyXA  ; random()*range
    
    TXA             ; drop the low 8 bits, put high 8 bits in A  (effectively:  divide by 256)
    CLC
    ADC $68AF       ; + lo
    
    RTS

MultiplyXA:
    STA btltmp_multA    ; store the values we'll be multiplying
    STX btltmp_multB
    LDX #$08            ; Use x as a loop counter.  X=8 for 8 bits
    
    LDA #$00            ; A will be the high byte of the product
    STA btltmp_multC    ; multC will be the low byte
    
    ; For each bit in multA
  @Loop:
      LSR btltmp_multA      ; shift out the low bit
      BCC :+
        CLC                 ; if it was set, add multB to our product
        ADC btltmp_multB
    : ROR A                 ; then rotate down our product
      ROR btltmp_multC
      DEX
      BNE @Loop
    
    TAX                     ; put high bits of product in X
    LDA btltmp_multC        ; put low bits in A
    RTS   

MathBuf_Add:
    STA $6BCF       ; backup index
    PHA             ; backup A,X,Y
    TXA
    PHA
    TYA
    PHA
    
    LDA $6BCF       ; index value * 2 to use as index
    ASL A
    TAY
    
    TXA             ; value to add from X
    CLC
    ADC btl_mathbuf,Y      ; add it
    STA btl_mathbuf,Y
    LDA #$00
    ADC btl_mathbuf+1,Y
    STA btl_mathbuf+1,Y
    BCC :+                  ; if exceeded FFFF
      LDA #$FF
      STA btl_mathbuf,Y    ; cap at FFFF
      STA btl_mathbuf+1,Y
      
  : PLA                     ; restore backups of all regs
    TAY                     ; before exiting
    PLA
    TAX
    PLA
    RTS


;assembled bytes
;AD 6E 68 0A AA BD A3 A2 85 96 BD A4 A2 85 97 18 6C 96 00 90 A2 C9 A2 EB A2 08 A3 1C A3 30 A3 49
;A3 49 A3 00 00 00 00 00 00 00 00 00 00 00 00 8D A3 00 00 00 00 A1 A3 C3 A3 A9 01 8D 5C 68 20 3A
;A4 AD 89 6C 29 80 F0 08 20 2B A4 A5 00 20 85 A4 A9 B8 48 A9 9E 48 A9 0C 4C 03 FE 20 3A A4 AD 89
;6C 29 80 F0 08 20 2B A4 A5 00 20 85 A4 A9 B9 48 A9 07 48 A9 0C 4C 03 FE 20 F2 A3 A9 B9 48 A9 31
;48 A9 B8 48 A9 2C 48 A9 0C 4C 03 FE 20 F2 A3 A9 B9 48 A9 60 48 A9 B8 48 A9 2C 48 A9 0C 4C 03 FE
;20 F2 A3 A9 B9 48 A9 7B 48 A9 B8 48 A9 2C 48 A9 0C 4C 03 FE A9 0C 4C 03 FE A9 01 8D 5C 68 AD 6D
;68 29 01 D0 EF AD 74 68 85 90 AD 89 6C 29 80 F0 0E 20 2B A4 8A 6D 74 68 90 02 A9 FF 8D 74 68 AA
;A9 00 20 45 A4 18 65 90 90 02 A9 FF AA A9 12 20 85 A4 A9 B9 48 A9 B9 48 A9 0C 4C 03 FE 20 F2 A3
;A9 BA 48 A9 48 48 A9 B8 48 A9 2C 48 A9 0C 4C 03 FE 20 3A A4 AD 89 6C 29 80 F0 08 20 2B A4 A5 00
;20 85 A4 A9 BA 48 A9 96 48 A9 0C 4C 03 FE A9 0C 4C 03 FE AD 78 68 2D 77 68 D0 F3 A9 2C 8D 5A 68
;A9 01 8D 5B 68 AD 89 6C 29 80 F0 0B 20 2B A4 8A 0A AA A9 02 20 85 A4 A9 BA 48 A9 E8 48 A9 0C 4C
;03 FE 20 3A A4 AD 78 68 2D 77 68 F0 0B A9 00 8D 56 68 8D 57 68 4C 26 A4 AD 89 6C 29 80 F0 08 20
;2B A4 A9 00 20 85 A4 AD 78 68 2D 76 68 F0 07 A9 00 A2 28 20 85 A4 60 00 40 80 C0 AD 89 6C 29 03
;AA BD 27 A4 AA BD 12 61 AA 60 A9 94 8D 56 68 A9 00 8D 57 68 60 8D AF 68 E8 8E B0 68 8A 38 ED AF
;68 8D B6 68 20 27 F2 AE B6 68 20 63 A4 8A 18 6D AF 68 60 8D B3 68 8E B4 68 A2 08 A9 00 8D B5 68
;4E B3 68 90 04 18 6D B4 68 6A 6E B5 68 CA D0 F0 AA AD B5 68 60 8D CF 6B 48 8A 48 98 48 AD CF 6B
;0A A8 8A 18 79 56 68 99 56 68 A9 00 79 57 68 99 57 68 90 08 A9 FF 99 56 68 99 57 68 68 A8 68 AA
;68 60
