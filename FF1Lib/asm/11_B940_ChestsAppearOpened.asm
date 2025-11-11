tmp             = $10 ; 16 bytes
doorppuaddr     = $0E ; 2 bytes

tileprop        = $44 ; 2 bytes
cur_map         = $48

tileset_data       = $0400
load_map_pal       = tileset_data+$380
TP_SPEC_TREASURE   = %00001000

lut_SMPalettes     = $A000   ; BANK_SMINFO - $1000 byte bound
lut_NTLo           = $D5E2
lut_NTHi           = $D5F2

unsram             = $6000  ; $400 bytes
game_flags         = unsram+$0200
GMFLG_TCOPEN       = %00000100
DLGID_TCGET        = $F0   ; "In this chest you find..."
DLGID_CANTCARRY    = $F1   ; "You can't carry anymore"
DLGID_EMPTYTC      = $F2   ; "this treasure chest is empty"

SwapPRG           = $FE03
Copy256           = $CC74
OpenTreasureChest = $DD78
WaitForVBlank     = $FEA8
SetSMScroll       = $CCA1

scroll_y        = $2F  ; Y scroll in tiles (16x16).  range=0-E
facing          = $33  ; 1=R  2=L  4=D  8=U
FACING_RIGHT    = $01
FACING_LEFT     = $02
FACING_DOWN     = $04
FACING_UP       = $08

PPU_STATUS      = $2002
PPU_ADDR        = $2006
PPU_DATA        = $2007

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



; Original TalkToSMTile @ $CBF9

;  @TreasureChest:             ; if the tile is a treasure chest
;    LDX tileprop+1            ; put the chest ID in X
;    LDA game_flags, X         ; get the game flag associated with that chest
;    AND #GMFLG_TCOPEN         ;   to see if the chest has already been opened
;    BEQ :+                    ; if it has....
;      LDA #DLGID_EMPTYTC      ; select "The Chest is empty" text, and exit
;      RTS

;:   JMP OpenTreasureChest     ; otherwise, open the chest

; Patch
.ORG $CBF9
    LDA #$11
    JSR SwapPRG
    JMP WrapOpenTreasureChest

; A9 11 20 03 FE 4C 8A B9

.ORG $B98A
WrapOpenTreasureChest:
    ; Replicate the patched out code
    LDX tileprop+1            ; put the chest ID in X
    LDA game_flags,X          ; get the game flag associated with that chest
    AND #GMFLG_TCOPEN         ;   to see if the chest has already been opened
    BEQ :+                    ; if it has....
      LDA #DLGID_EMPTYTC      ; select "The Chest is empty" text, and exit
      RTS

    ; Now we call the original OpenTreasureChest
:   JSR OpenTreasureChest
    CMP #DLGID_CANTCARRY
    BEQ @End
    PHA                       ; save whatever the return value was from OpenTreasureChest
    ; So that we can come back here and draw the new tiles
    LDA scroll_y              ; get the scroll value to see where the player is
    CLC                       ; necessary because the vertical position can't be simply masked out
    ADC #$07                  ; add 7 to get the row the player is on
    TAY
    LDA facing                ; get the facing direction
    CMP #FACING_UP            ; is the player facing up?
    BNE @NotUp
    DEY                       ; player is facing up, so go one row up
    JMP @SetAddr
    @NotUp:
    CMP #FACING_DOWN
    BNE @SetAddr
    INY                       ; player is facing down, so go one row down
    @SetAddr:
    TYA                       ; get the row back
    CMP #$0F                  ; if the row is >= 15, clamp it to 15
    BCC @RowGood
    SBC #$0F
    @RowGood:
    TAY                       ; Y is now the actual row
    LDA tmp+4                 ; get the X coordinate
    AND #$1F                  ; this one we can mask off
    CMP #$10                  ; see if the high bit is set
    BCS @SecondPage

    @FirstPage:
    ASL A                     ; the X tile coordinate is double the map coordinate
    ORA lut_NTLo,Y            ; the nametable addresses for each row are in this lookup table
    STA doorppuaddr           ; reuse this variable
    LDA lut_NTHi,Y            ; get the high byte
    STA doorppuaddr+1
    JMP @Draw

    @SecondPage:
    AND #$0F                  ; mask off the high bit
    ASL A                     ; the X tile coordinate is double the map coordinate
    ORA lut_NTLo,Y            ; the nametable addresses for each row are in this lookup table
    STA doorppuaddr           ; reuse this variable
    LDA lut_NTHi,Y            ; get the high byte
    ORA #$04                  ; second nametable page
    STA doorppuaddr+1

    @Draw:
    JSR WaitForVBlank
    LDA PPU_STATUS            ; reset address
    LDA doorppuaddr+1         ; get high byte
    STA PPU_ADDR
    LDA doorppuaddr           ; get low byte
    STA PPU_ADDR
    LDA #$7E                  ; opened chest top left tile
    STA PPU_DATA
    LDA #$7F                  ; opened chest top right tile
    STA PPU_DATA

    JSR SetSMScroll           ; reset the scroll (don't know why this is needed, but it prevents flicker)
    PLA                       ; restore the return value from OpenTreasureChest
    @End:
    RTS

; A6 45 BD 00 62 29 04 F0 03 A9 F2 60 20 78 DD C9 F1 F0 61 48 A5 2F 18 69 07 A8 A5 33 C9 08 D0 04
; 88 4C B3 B9 C9 04 D0 01 C8 98 C9 0F 90 02 E9 0F A8 A5 14 29 1F C9 10 B0 0E 0A 19 E2 D5 85 0E B9
; F2 D5 85 0F 4C E0 B9 29 0F 0A 19 E2 D5 85 0E B9 F2 D5 09 04 85 0F 20 A8 FE AD 02 20 A5 0F 8D 06
; 20 A5 0E 8D 06 20 A9 7E 8D 07 20 A9 7F 8D 07 20 20 A1 CC 68 60

