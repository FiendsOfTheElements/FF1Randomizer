tmp             = $10 ; 16 bytes

cur_map         = $48

tileset_data       = $0400
load_map_pal       = tileset_data+$380
TP_SPEC_TREASURE   = %00001000

lut_SMPalettes     = $A000   ; BANK_SMINFO - $1000 byte bound

unsram             = $6000  ; $400 bytes
game_flags         = unsram+$0200
GMFLG_TCOPEN       = %00000100

SwapPRG = $FE03
Copy256 = $CC74

; bank 1F
; Original LoadSMTilesetData @ $CC45
; load map palettes

;    LDA #0
;    STA tmp+1
;
;    LDA cur_map             ; get current map and multiply it by $30, rotating carry into tmp+1
;    ASL A                   ; first, shift left by 4 to multiply by $10
;    ASL A
;    ASL A
;    ROL tmp+1
;    ASL A
;    ROL tmp+1
;    STA tmp
;
;    LDX tmp+1               ; load high byte into X.  Here X and A are *$10
;
;    ASL tmp                 ; shift RAM by 1 more to multiply by $20
;    ROL tmp+1
;
;    CLC                     ; add *$10 (in A,X) to the *$20 (in tmp,tmp+1) to get *$30
;    ADC tmp
;    STA tmp
;    TXA
;    ADC tmp+1
;    ORA #>lut_SMPalettes    ; OR high byte with high byte of palette LUT
;    STA tmp+1

; Patch

.ORG $CC45
    LDA #$11
    JSR SwapPRG
    JMP ReplaceOpenedChestTSA

.ORG $CC62
    LDA #$00 ; BANK_SMINFO
    JSR SwapPRG

; A9 11 20 03 FE 4C 40 B9
; A9 00 20 03 FE

.ORG $B940
ReplaceOpenedChestTSA:
    ; The tileset data has been loaded to $0400, we need to loop over it, and
    ; if a tile is a chest, and the chest is opened, replace the top two TSA
    ; bytes with the opened chest tile values.

    LDY #$00
    @Loop:
        LDA tileset_data,Y        ; get the first byte of tile properties
        AND #TP_SPEC_TREASURE     ; see if it's a treasure chest
        BEQ @EndLoop
        LDX tileset_data+1,Y      ; get the treasure index
        LDA game_flags,X          ; get the game flags
        AND #GMFLG_TCOPEN         ; get the opened flag
        BEQ @EndLoop

        ; The TSA is stored as all the top left tiles, then all the top right, etc.
        ; So to change the top left, we just offset into that array by the tile index,
        ; which is half of Y (since Y is incrementing by 2 bytes per tile).
        TYA
        LSR A
        TAX
        LDA #$7E                  ; opened chest top left tile
        STA tileset_data+$100,X
        LDA #$7F                  ; opened chest top right tile
        STA tileset_data+$180,X

        @EndLoop:
        INY
        INY                       ; advance two bytes
        BNE @Loop

    ; Here we have to replicate a chunk of LoadSMTilesetData that we jumped around.
    LDA #0
    STA tmp+1

    LDA cur_map             ; get current map and multiply it by $30, rotating carry into tmp+1
    ASL A                   ; first, shift left by 4 to multiply by $10
    ASL A
    ASL A
    ROL tmp+1
    ASL A
    ROL tmp+1
    STA tmp

    LDX tmp+1               ; load high byte into X.  Here X and A are *$10

    ASL tmp                 ; shift RAM by 1 more to multiply by $20
    ROL tmp+1

    CLC                     ; add *$10 (in A,X) to the *$20 (in tmp,tmp+1) to get *$30
    ADC tmp
    STA tmp
    TXA
    ADC tmp+1
    ORA #>lut_SMPalettes    ; OR high byte with high byte of palette LUT
    STA tmp+1

    JMP $CC62               ; jump back to LoadSMTilesetData

; A0 00 B9 00 04 29 08 F0 17 BE 01 04 BD 00 62 29 04 F0 0D 98 4A AA A9 7E 9D 00 05 A9 7F 9D 80 05
; C8 C8 D0 DE A9 00 85 11 A5 48 0A 0A 0A 26 11 0A 26 11 85 10 A6 11 06 10 26 11 18 65 10 85 10 8A
; 65 11 09 A0 85 11 4C 62 CC
