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
CHRLoad = $E965
CHRLoadToA = $E95A
LoadMenuCHR = $E98E
LoadMapObjCHR = $E99E;



.org $8680 ; 

; Replaces the character with our Custom Icon
PrintCharStatOrIcon:
    LDX shop_id
    CPX #$0
    BNE @CharStat 

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
.org $EAA2 ; in bank 1F

OverwriteUnusedMenuBGCHR:
    LDA #$12                 
    JSR SwapPRG                    
    LDX #$08             ; 8 rows of tiles
    LDA #<$8800                 
    STA tmp
    LDA #>$8800
    STA tmp+1
    LDA #$0 


.org $EA02 ; in bank 1F
ReplaceShopBGCHRLoad: ; This overwrites things

    LDA #$12             ; swap to icon bank
    JSR SwapPRG
    JSR AddIconsToShopBGCHR

    LDA #BANK_MENUCHR 
    JSR SwapPRG

    LDA #$00           ; dest PPU address = $0000
    LDX #$08           ; 8 rows to load
    JSR CHRLoadToA
    NOP                 ; Blanking out the original LoadMenuCHR because we're doing it ourselves from Bank 12
    NOP
    NOP

.org $9000
AddIconsToShopBGCHR:

    LDA #<$8000          ; Load in the shop icons
    STA tmp
    LDA #>$8000
    STA tmp+1

    LDX #$08             ; 8 rows of tiles
    LDA #$8              ; dest PPU adddress = $8000

    JSR CHRLoadToA

    LDA #<lut_ShopCHR  ; Now complete the original routine
    STA tmp
    LDA #>lut_ShopCHR  ; source pointer (tmp) = lut_ShopCHR
    STA tmp+1

    RTS


.org $E99E
ReplaceSTMBGCHRLoad:
    LDA #$12             ; swap to icon bank
    JSR SwapPRG
    JSR AddIconsToSTMBGCHR


.org $9020
AddIconsToSTMBGCHR:

    LDA #<$8000          ; Load in the shop icons
    STA tmp
    LDA #>$8000
    STA tmp+1

    LDX #$08             ; 8 rows of tiles
    LDA #$8              ; dest PPU adddress = $8000

    JSR CHRLoadToA

    LDA PPU_STATUS
    LDA #$11
    STA PPU_ADDRESS

    RTS