; Changed slightly from the original on July 2022 for compatibility with adjustable lava damage

tmp             = $10 ; 16 bytes
palcyc_mode     = tmp+$C  ; shared tmp
btl_combatboxcount  = $6AF8     ; the number of combat boxes that have been drawn
music_track     = $4B
cur_bank        = $57
mapflags        = $2D  ; bit 0 set when in standard map.  bit 1 set to indicate column drawing instead of row drawing= $F2

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

tmpA = $10
tmpY = $11
location = $F2
cur_map = $48
ch_armor = ch_stats + $1C

 .ORG $B100

AssignMapTileDamage:
  LDX #$00              ; zero loop counter and char index
  LDY #$00              ; dead characters counter
Loop:
  LDA ch_ailments, X
  CMP #$01
  BEQ AlreadyDead

	LDA #$01				; only do any of the special armor checks if the randomizer has set this to 0
	BNE AssignDamage
	LDA location            ; see if we're on the overworld, or in a standard map
	CMP #$03
	BCC AssignDamage        ; if we're on the overworld, skip the armor check

	@VolcanoCheck:
		LDA cur_map				; get the current map
		CMP #$0E				; see if it's Volcano
		BEQ @Volcano
		CMP #$21
		BEQ @Volcano
		CMP #$22
		BEQ @Volcano
		CMP #$23
		BEQ @Volcano
		CMP #$24
		BEQ @Volcano
		JMP @IceCheck

	@Volcano:
		LDA #$88 ; Ice Armor ID
		JSR ArmorCheck
		BNE AssignDamage
		JMP AliveOrDying

	@IceCheck:
		CMP #$0F					; see if it's Ice
		BEQ @Ice
		CMP #$25
		BEQ @Ice
		CMP #$26
		BEQ @Ice
		CMP #$27
		BEQ @Ice

	@Ice:
		LDA #$87 ; Flame Armor ID
		JSR ArmorCheck
		BEQ AssignDamage
			JMP AliveOrDying

  AssignDamage:
    LDA ch_curhp+1, X     ; check high byte of HP
    BNE DmgSubtract       ; if nonzero (> 255 HP), deal this guy damage

    LDA ch_curhp, X       ; otherwise, check low byte
    CMP #2                ; if >=2, just substract hp
    BCS DmgSubtract
  
    LDA #$01
    STA ch_ailments, X		; set dead status

      LDA #0
	  STA ch_curhp, X				; sets HP directly to 0 -- added to prevent problems with lava damage > 1
	  JMP AlreadyDead

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
  RTS

DmgSubtract:
  LDA ch_curhp, X       ; subtract 1 HP
  SEC
    SBC #1
    STA ch_curhp, X
    LDA ch_curhp+1, X
    SBC #0
    STA ch_curhp+1, X
  JMP AliveOrDying         ; else exit

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
  LDA #<GameStart-1         ;  before restarting the game
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

ArmorCheck:
	STY tmpY	; back up the "dead characters" counter
	STA tmpA	; store the armor ID so we can compare later
	TXA			; we need to increment this for each piece of armor, so make a copy in Y
	TAY
@ArmorLoop:
	LDA ch_armor, Y	; next armor
	CMP tmpA
	BNE @NoEquip
		LDY tmpY	; restore the "dead characters" counter
		LDA #$00
		RTS
@NoEquip:
	INY
	TYA
	AND #$04
	CMP #$04	; out of armor range
	BNE @ArmorLoop
		LDY tmpY	; restore the "dead characters" counter
		LDA #$01
		RTS

; A200A000BD0161C901F05FA901D042A52D4A903DA548C90EF013C921F00FC922F00BC923F007C924F0034C37B1A98820FCB1D01D4C6BB1C90FF00CC925F008C926F004C927F000A98720FCB1F0034C6BB1BD0B61D021BD0A61C902B01AA9019D0161A9009D0A614C6AB1C88A186940AAD092C004F01560BD0A6138E9019D0A61BD0B61E9009D0B614C6BB1A980854BA91E8557A52D6AA9009002A902851C2000FE2082D920B3D9A9064820EFD968A88898D0F6A0402000FE2018D92082D9A91E8D01202089C688D0ECA952854B20EEB1A9008D00208D01204C12C0A9C048A91148A98F48A9F748A91B85574C03FE2028D8482000FE2089C668F0F360841185108AA8B91C61C510D005A411A90060C8982904C904D0ECA411A90160
