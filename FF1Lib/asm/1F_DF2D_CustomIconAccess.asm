; allows printing icons from the lower half of the pattern table
; ControlCode $10 followed by tile + $80

.include "Constants.inc"
.include "variables.inc"

PrintCharStatSetupMinus1 = $84F3
PrintCharStat = $8D70
StallAndDraw = $DE3E
ReturnHome = $DF36
SwapPRG = $FE03
Restore = $E04E ; the @Restore from DrawComplexString


.org $8600 ; 

; Replaces the character with our Custom Icon
PrintCharStatOrIcon:
    CMP #$80            ; if (currentChar < 0x80)... basically if it's 0x10
    BCC @CharStat       ;   Jump ahead to @CharStat
    EOR #$80            ; remove highest bit (so 0x81 is tile 0x1)
    LDX $2002         ; reset PPU toggle
    LDX ppu_dest+1    ;  load and set desired PPU address
    STX $2006         ;  do this with X, as to not disturb A, which is still our character
    LDX ppu_dest
    STX $2006
    STA $2007           ; draw the character
    INC ppu_dest        ; increment dest PPU address, which should advance the place to draw the text on screen
    JMP Restore         ; then restore original string state and continue

; Bugger off to the original Char Stat function
@CharStat:
    TAX
    LDA #$84
    PHA
    LDA #$F3
    PHA
    LDA #$0E
    JMP SwapPRG


; Complete the DF2D routine since we had to STOMP on the stack

.org $84F4  ; in bank $0E

PrintCharStatSetup:
    TXA
    JSR PrintCharStat
    JSR StallAndDraw
    JMP Restore


; Hook function

.org $DF2D ; in bank $1F

PrintCharStatBreakout:
    LDA #$1E
    JSR SwapPRG
    TXA
    JMP PrintCharStatOrIcon


; Overwrites most of LoadMenuBGCHRAndPalettes in Bank F to allow for new symbols/gylphs in memory
.include "variables.inc"

    LDA #$0C                 
    JSR SwapPRG_L                    
    LDX #$08             ; 8 rows of tiles
    LDA #<$8800                 
    STA tmp
    LDA #>$8800
    STA tmp+1
    LDA #$0 