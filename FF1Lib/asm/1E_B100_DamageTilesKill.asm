tmp             = $10 ; 16 bytes
palcyc_mode     = tmp+$C  ; shared tmp
btl_combatboxcount  = $6AF8     ; the number of combat boxes that have been drawn
music_track     = $4B
cur_bank        = $57
mapflags        = $2D  ; bit 0 set when in standard map.  bit 1 set to indicate column drawing instead of row drawing

unsram          = $6000  ; $400 bytes
ch_stats        = unsram + $0100  ; MUST be on page bound.  Each character allowed $40 bytes, so use 00,40,80,C0 to index ch_stats
ch_ailments     = ch_stats + $01
ch_curhp        = ch_stats + $0A  ; 2 bytes

SaveOnDeath = $8FF8
GameStart = $C012
CallMusicPlay = $C689
GetJoyInput = $D828
WaitVBlank_NoSprites = $D89F
PalCyc_DrawPalette = $D918
PalCyc_SetScroll = $D982
PalCyc_GetInitialPal = $D9B3
PalCyc_Step = $D9EF
DrawOWSprites = $E225
DrawSMSprites = $E40F
WaitForVBlank_L = $FE00
SwapPRG_L = $FE03

 .ORG $B100

AssignMapTileDamage:
  LDX #$00              ; zero loop counter and char index
  LDY #$00              ; dead characters counter
Loop:
  LDA ch_ailments, X
  CMP #$01
  BEQ AlreadyDead
  
  LDA ch_curhp+1, X     ; check high byte of HP
  BNE DmgSubtract       ; if nonzero (> 255 HP), deal this guy damage

  LDA ch_curhp, X       ; otherwise, check low byte
  CMP #2                ; if >=2, just substract hp
  BCS DmgSubtract
  
  LDA #$01
  STA ch_ailments, X
  INY

DmgSubtract:
  LDA ch_curhp, X       ; subtract 1 HP
  SEC
    SBC #1
    STA ch_curhp, X
    LDA ch_curhp+1, X
    SBC #0
    STA ch_curhp+1, X
  JMP AliveOrDying
AlreadyDead:
  INY
AliveOrDying:
  TXA                   ; add $40 to char index (next character in party)
  CLC
  ADC #$40
  TAX

  BNE Loop             ; loop until it wraps (4 iterations)
  
  CPY #$04             ; is everyone dead?
  BEQ MapGameOver
  RTS                  ; else exit

MapGameOver:
  LDA #$80
  STA music_track      ; Stop music
  LDA #$1E
  STA cur_bank
  LDA mapflags         ; Check if we're on OW or on MSM
  ROR
  LDA #$00
  BCC PalCycModeSet
    LDA #$02
PalCycModeSet:  
  STA palcyc_mode           ; record the mode
  JSR WaitForVBlank_L       ; wait for VBlank
  JSR PalCyc_SetScroll      ; set the scroll
  JSR PalCyc_GetInitialPal  ; load up the initial palette
 
  LDA #$06
CyclePalette:               ; Cycle palette 6 times
  PHA
  JSR PalCyc_Step
  PLA
  TAY
  DEY
  TYA
  BNE CyclePalette
  
  LDY #$40 
WaitLoop:                   ; Dramatic pause for a second
  JSR WaitForVBlank_L       ; wait for VBlank
  JSR PalCyc_DrawPalette    ; draw the new palette
  JSR PalCyc_SetScroll      ; set the scroll
  
  LDA #$1E                  ; Turn PPU on
  STA $2001 
  
  JSR CallMusicPlay         ; and update music  (all the typical frame work)
  DEY
  BNE WaitLoop              ; and keep looping until cycling is complete
  
  LDA #$52                  ; Load party defeated track
  STA music_track
  
  JSR WaitForAnyInput       
  
  LDA #$00                  ; Turn off PPU
  STA $2000
  STA $2001

  JMP GameStart             ; Restart game
  
  LDA #>GameStart           ; If SaveOnDeath is enable, 
  PHA                       ;  NOP previous JMP to jump to it
  LDA #<GameStart-1         ;  berfore restarting the game
  PHA
  LDA #>SaveOnDeath       
  PHA                     
  LDA #<SaveOnDeath-1  
  PHA
  
  LDA #$1B
  STA cur_bank     
  JMP SwapPRG_L
  
WaitForAnyInput:
  JSR GetJoyInput
  PHA
  JSR WaitForVBlank_L
  JSR CallMusicPlay
  PLA
  BEQ WaitForAnyInput
  RTS
