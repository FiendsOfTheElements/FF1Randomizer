tmp             = $10 ; 16 bytes

lut_SMTilesetTSA   = $9000   ; BANK_SMINFO - page

tileset_data    = $0400  ; $400 bytes -- must be on page bound

TP_SPEC_TREASURE  = %00001000

cur_tileset = $49
SwapPRG = $FE03
Copy256 = $CC74

; bank 1F
; Original LoadSMTilesetData @ $CC21

 ; load tileset TSA

    LDA cur_tileset           ; get tileset
    ASL A                     ; *2 (it's assumed this clears C as well -- tileset is less than $80)
    ADC #>lut_SMTilesetTSA    ; set high byte of src pointer to lut_SMTilesetTSA+(tileset*512)
    STA tmp+1                 ;  512 bytes of TSA data per tileset

    JSR Copy256               ; copy the first 256 bytes of tsa data
    INC tmp+1                 ; inc src pointer
    JSR Copy256               ; and copy the second 256 bytes of tsa data

    ; at the end of this, tmp+3 = 7, and this is important

; A5 49 0A 69 90 85 11 20 74 CC E6 11 20 74 CC
; Patch

.ORG $CC21
    LDA $11
    JSR SwapPRG
    JSR LoadSMTilesetTSACheckChests
    LDA $00
    JSR SwapPRG
    NOP ; fill in
    NOP

; A9 11 20 03 FE 20 40 B9 A9 00 20 03 FE EA EA

.ORG $B940
LoadSMTilesetTSACheckChests:
    ; The tileset data has been loaded to $0400, we need to loop over it and
    ; load the TSA for each tile.  If the tile is a chest, we overwrite the
    ; top two TSA bytes to make it appear opened.

    ; tmp, tmp+1 points to source TSA data
    ; tmp+2, tmp+3 points to destination TSA data

    LDY #$00
    @Loop:
        ; copy 4 bytes of TSA data, unrolled for simplicity
        LDA (tmp)
        STA (tmp+2)
        INC tmp
        INC tmp+2
        LDA (tmp)
        STA (tmp+2)
        INC tmp
        INC tmp+2
        LDA (tmp)
        STA (tmp+2)
        INC tmp
        INC tmp+2
        LDA (tmp)
        STA (tmp+2)
        INC tmp
        INC tmp+2

        ; check if this was a chest
        TYA
        STA tmp+5
        LDA tileset_data,Y
        AND #TP_SPEC_TREASURE
        BEQ @RollOver

        DEC tmp+2 ; go back two bytes
        DEC tmp+2
        LDA #$7E
        STA (tmp+2) ; set to opened chest tile
        INC tmp+2
        LDA #$7F
        STA (tmp+2)+1
        INC tmp+2

        @RollOver:
        LDA tmp
        BNE @EndLoop
        INC tmp+1
        INC tmp+3

        @EndLoop:
        INY
        CPY $#80
        BNE @Loop

    RTS

; A0 00 A5 10 85 12 E6 10 E6 12 A5 10 85 12 E6 10 E6 12 A5 10 85 12 E6 10 E6 12 A5 10 85 12 E6 10
; E6 12 98 85 15 B9 00 04 29 08 F0 10 C6 12 C6 12 A9 7E 85 12 E6 12 A9 7F 85 13 E6 12 A5 10 D0 04
; E6 11 E6 13 C8 C0 80 D0 B9 60
