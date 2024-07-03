index               = $6BAD  ; local - temp to hold OB stat index (00, 40, 80, C0)
ch_curhp            = $610A  ; 2 bytes
ch_maxhp            = $610C  ; 2 bytes
btl_mathbuf         = $6856
math_hitchance      = $6856
math_basedamage     = $6858
math_numhits        = $685A

MATHBUF_HITCHANCE           = 0
MATHBUF_BASEDAMAGE          = 1
MATHBUF_NUMHITS             = 2

SwapPRG                 = $FE03
ImprovedPoisonAddr      = $A670 ;$72680
RetToApplyPoison        = $A2EA 

;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;
; patch to ApplyPoisonToPlayer in bank 0C
; [$A2D7 :: 322E7 (includes header)]
;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;
.org $A2D7
        LDA #>(ImprovedPoisonAddr-1)
        PHA
        LDA #<(ImprovedPoisonAddr-1)
        PHA
        LDA #$1C
        JMP SwapPRG

;assembled bytes
;A9A648A96F48A91C4C03FE


;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;
; Patched Poison Damage Calculation Routine (bank 1C)
; Loads configuration information statically inserted at the end of
;   this block of code for easy changing per flags.
;   See end of this file for more details
; [$A670 :: 0x72680 (includes header)]
;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;
.org $A670 ;$72680
    DoImprovedPoison:
        LDX index ;retrieve offset for current character

        LDA ch_maxhp,X ;store max hp in MATHBUF_BASEDAMAGE
        STA math_basedamage
        LDA ch_maxhp+1,X
        STA math_basedamage+1

        LDA ch_curhp,X ;store current hp in MATHBUF_HITCHANCE
        STA math_hitchance
        LDA ch_curhp+1,X
        STA math_hitchance+1

        LDY poisonMode  ;branch based on operating mode
        BEQ constantModeStart
        DEY
        BEQ percentModeStart
        DEY
        BEQ diminishingPercentModeStart
        DEY
        BEQ almostAllModeStart

    constantModeStart:
        LDA constantAmount ;new poison damage value to apply (set in configuration)
        STA math_basedamage
        LDA constantAmount+1
        STA math_basedamage+1
        JMP DoSubtract

    diminishingPercentModeStart:
        LDA ch_curhp,X ;store current hp in MATHBUF_BASEDAMAGE
        STA math_basedamage
        LDA ch_curhp+1,X
        STA math_basedamage+1 ;flow into percent mode after switching max hp for current hp

    percentModeStart:
        LDY percentLoopCount ;number of times to half hp to calculate poison damage (set in configuration)
    p_loop:
        CLC
        ROR math_basedamage+1 ;bit rotations will rotate in/out the carry flag, making dividing a 16-bit number by 2 easy
        ROR math_basedamage
        DEY
        BNE p_loop
        JMP DoSubtract

    almostAllModeStart:
        LDA constantAmount
        STA math_numhits
        LDA constantAmount+1
        STA math_numhits+1 ;load constant amount into MATHBUF_NUMHITS

        LDA #MATHBUF_BASEDAMAGE ;subtract constant amount from max hp
        LDX #MATHBUF_BASEDAMAGE
        LDY #MATHBUF_NUMHITS
        JSR MathBuf_Sub16

    DoSubtract:
        LDA #MATHBUF_HITCHANCE ;subtract calculated amount of max hp from current hp
        LDX #MATHBUF_HITCHANCE
        LDY #MATHBUF_BASEDAMAGE
        JSR MathBuf_Sub16

        LDA #>(RetToApplyPoison-1) ;poison routine expects new current hp to be in MATHBUF_HITCHANCE where we return to
        PHA
        LDA #<(RetToApplyPoison-1)
        PHA
        LDA #$0C
        JMP SwapPRG

MathBuf_Sub16:  ;copied from bank 0C
    PHA
    TXA     ; Double X!
    ASL A
    TAX
    
    TYA     ; Double Y!
    ASL A
    TAY
    
    LDA btl_mathbuf,X
    SEC
    SBC btl_mathbuf,Y
    STA $6BCF
    
    LDA btl_mathbuf+1,X
    SBC btl_mathbuf+1,Y
    STA $6BD0
    
    BCS :+
      LDA #$00
      STA $6BCF
      STA $6BD0
      
  : PLA
    ASL A
    TAX
    LDA $6BCF
    STA btl_mathbuf,X
    LDA $6BD0
    STA btl_mathbuf+1,X
    RTS

;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;
;Configuration - these will be changed depending on flags
;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;
poisonMode:
        ;Mode to operate in.
        ; 00 = constant: a 16-bit constant is subtracted from current hp
        ; 01 = percent: fractions of max hp (1/2, 1/4, 1/8, etc)
        ; 02 = dimishing percent: fractions of current hp (1/2, 1/4, 1/8, etc)
        ; 03 = max hp based: (max_hp - constantAmount) is subtracted
        .BYTE $01
percentLoopCount:
        ;Number of times to halve hp to get amount to subtract
        ; 1 = 1/2 hp, 2 = 1/4 hp, etc
        .BYTE $03
constantAmount:
        ;16-bit value of poison damage for 'constant' mode
        .WORD $1A3


;assembled bytes
;AEAD6BBD0C618D5868BD0D618D5968BD0A618D5668BD0B61
;8D5768AC21A7F00988F02188F01288F02BAD23A78D5868AD
;24A78D59684CD9A6BD0A618D5868BD0B618D5968AC22A718
;6E59686E586888D0F64CD9A6AD23A78D5A68AD24A78D5B68
;A901A201A00220EDA6A900A200A00120EDA6A9A248A9E948
;A90C4C03FE488A0AAA980AA8BD566838F956688DCF6BBD57
;68F957688DD06BB008A9008DCF6B8DD06B680AAAADCF6B9D
;5668ADD06B9D5768600103A301
