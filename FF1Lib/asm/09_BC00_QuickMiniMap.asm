.include "Constants.inc"
.include "variables.inc"

MinimapFrame = $BD49
DrawPalette_L = $D850
Minimap_YouAreHere = $BF34
WaitForVBlank_L = $FE00
CHRLoadToA = $E95A
DrawDungeonSprite = $BE30

lut_MinimapBGPal = $B520
mm_nt_data = $B000
mm_chr_data = $8000
lut_MinimapSprCHR = $B500
mm_entrance_data_x = $B400
mm_entrance_data_y = $B440
mm_entrance_count = $B480


.org $BC00

EnterMinimap:
	LDA #$09
	STA cur_bank           ; set cur bank (required for music routine)

	LDA #$06			; map low half of 19
	STA $8000 			; select R6
	LDA #$32	
	STA $8001 			; map the low bank

	LDA #$00
	STA $2001
	STA $4015              ; turn off PPU and APU

	LDA #$41               ; switch to music track $41 (crystal theme)
    STA music_track        ;   but it's not heard until after all drawing is complete (music routine isn't called)

	LDA #$08           ; write to soft2000 to clear NT scroll
    STA soft2000

	JSR Minimap_YouAreHere     ; Load the "You are here" graphic -- clear OAM, and draw the "you are here" sprite		

	LDX mm_entrance_count
   @DungeonEntranceLoop:
	  DEX
	  TXA
	  PHA
	  LDA mm_entrance_data_y, X
	  STA mm_maprow
	  LDY mm_entrance_data_x, X
	  JSR DrawDungeonSprite
	  PLA
	  TAX
	  CPX #$00
	  BNE @DungeonEntranceLoop
	  

LoadSpriteCHR:
    LDA $2002          ; reset PPU toggle
    LDA #>$1800        ; PPU address = $1800  (start of CHR for sprite tile $80)
    STA $2006
    LDA #<$1800
    STA $2006

    LDX #$00
   @SpriteCHRLoop:
      LDA lut_MinimapSprCHR, X
      STA $2007                      ; copy the CHR to the PPU
      INX                            ; inc loop counter
      CPX #$20
      BCC @SpriteCHRLoop             ; loop until $20 bytes copied (2 tiles)

LoadBackgroundCHR:
	LDA #<mm_chr_data           ; low byte of source pointer
    STA tmp
    LDA #>mm_chr_data			; high byte of source pointer
    STA tmp+1

	LDA #$00
	LDX #$10
	JSR CHRLoadToA

LoadNameTable:	
	LDA #<mm_nt_data            ; low byte of source pointer
    STA tmp
    LDA #>mm_nt_data			; high byte of source pointer
    STA tmp+1
	LDA #$20
	LDX #$04
	JSR CHRLoadToA

LoadPalette:
    LDX #$0F
   @BGPalLoop:
	  LDA lut_MinimapBGPal, X   ; copy color from LUT
      STA cur_pal, X            ;  to palette
      DEX
      BPL @BGPalLoop            ; loop until X wraps ($10 iterations)

    LDA cur_pal           ; copy the background color to the mirrored copy in the sprite palette
    STA cur_pal+$10       ; this ensures the BG color will be what's desired when the palette is drawn

    LDA #$0F
    STA cur_pal+$11       ; start the "you are here" sprite
    STA cur_pal+$12       ; and the town/dungeon marker sprite at $0F black
	
    JSR WaitForVBlank_L   ; wait for VBlank
    LDA #>oam             ; then do sprite DMA
    STA $4014

    JSR DrawPalette_L     ; draw the palette

    LDA soft2000
    STA $2000             ; set NT scroll and pattern page assignments
    LDA #$0A
    STA $2001             ; turn on BG rendering, but leave sprites invisible for now

    LDA #$00              ; set scroll to $00,$E8  (8 pixels up from bottom of the NT)
    STA $2005             ;   since there's vertical mirroring, this scrolls up 8 pixels,
    LDA #$E8              ;   which makes the screen appear to be 8 pixels lower than
    STA $2005             ;   what you might expect.  This centers the image on the screen a bit better
                          ; The NT could've just been drawn 1 tile down.. but that would mess with
                          ; attribute alignment

@ExitLoop:
    JSR MinimapFrame      ; do a frame... animating sprite palettes and whatnot

    LDA joy_a
    ORA joy_b
    BEQ @ExitLoop         ; and simply loop until the user presses A or B

    RTS                   ; at which point... exit!
