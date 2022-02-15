;
; Pressing Select in the Main Menu will show Stats
; Last Update: 2022-02-14
;

tmp_gold = $6E00

tmp = $10

joy             = $20
joy_ignore      = $21
joy_select      = $22
joy_start       = $23
joy_a           = $24
joy_b           = $25
joy_prevdir     = $61

menustall       = $37 

box_x           = $38
box_y           = $39
box_wd          = $3C ; shared
box_ht          = $3D ; shared

dest_x          = $3A
dest_y          = $3B
dest_wd         = $3C
dest_ht         = $3D

text_ptr        = $3E ; 2 bytes

cur_bank        = $57
ret_bank        = $58

cur_pal         = $03C0       ; 32 bytes

unsram          = $6000  ; $400 bytes
gold            = unsram + $1C   ; 3 bytes

NTsoft2000      = $FD  ; same as soft2000, but used to track coarse NT scroll
unk_FE          = $FE
soft2000        = $FF

ClearOAM = $C43C
DrawComplexString = $DE36      ; Bank $1F
DrawBox = $E063                ; Bank $1F
SwapPRG_L = $FE03              ; Bank $1F
SwapPRG_skip = $FE07                       ; Bank $1F, jump to skip putting the bank on the stack
ClearNT = $9C02
ResetScroll = $E19A

ResumeMainMenu = $ADCD
PlaySFX_MenuSel = $84EB
TurnMenuScreenOn = $B780
MenuWaitForBtn_SFX_0E = $B613     ; Bank $0E
MenuWaitForBtn_SFX_1E = $851A  ; Bank $1E
LoadMenuCHRPal = $E906
LoadMenuCHR = $E98E
WaitForVBlank_L = $FE00
CallMusicPlay = $C689
UpdateJoy = $D7C2

lutMenuPalettes = $AD78


 .ORG $B665
MenuFrame:          ; Bank 0E
  JSR ResetScroll
  LDA #$00
  STA joy_select
  NOP
  NOP
  NOP
  NOP
  NOP
  NOP


 .ORG $ADF4
 
A_Pressed = $AE0E
B_Pressed = $AE02
 
MainMenuLoop:	   ; Bank OE
  JSR CheckInput
  BNE A_Pressed
  BCS B_Pressed

 .ORG $BA00

lut_StatsPage:
 .BYTE $00, $00, $00, $00, $00, $00

lut_BoxSize:
 .BYTE $01, $0B, $0C, $11

lut_TextPosition:
 .BYTE $02, $0D, $15
   
 .ORG $BA0D
  
CheckInput: 
  LDA joy_select
  BEQ noselect
    JMP EnterStatsMenu
noselect:
  LDA joy_a
  BEQ noa
    LDA #$01
    RTS
noa:  
  LDA joy_b
  BEQ nob
    LDA #$00
    SEC
    RTS
nob:
  LDA #$00
  CLC
  RTS

EnterStatsMenu: 
  JSR DrawStatsScreen
  JSR ClearOAM               ; clear OAM
  JSR TurnMenuScreenOn_L     ; turn the screen on
  
  JSR MenuWaitForBtn_SFX_1E  ; then just wait for the user to press a button before exiting
  
  PLA
  PLA
  
  LDA #>ResumeMainMenu
  PHA
  LDA #<ResumeMainMenu-1
  PHA
  LDA #$0E
  JMP SwapPRG_L

DrawStatsScreen: 
  LDA #0
  STA $2001               ; turn off the PPU
  LDA #0
  STA menustall           ; disable menu stalling
  JSR ClearNT_L           ; clear the NT
 
  LDA gold                ; backup gold
  STA tmp_gold            ;  since drawcomplexstring wrecks that value
  LDA gold+1
  STA tmp_gold+1
  LDA gold+2
  STA tmp_gold+2
   
  
  LDA #$01
  STA box_x
  LDA #$1E
  STA box_wd
  
  LDA #$02
  STA dest_x
  
  LDX #$00
  
loopBox:
  STX tmp+4
  LDY lut_BoxSize, X
  STY box_y
  STY dest_y
  LDA lut_BoxSize+1,X
  STA box_ht

  JSR DrawBox

  LDX tmp+4
  INX
  INX
  CPX #$04
  BCC loopBox

  LDX #$00
  
loopText:
  STX tmp+4
  LDA lut_StatsPage, X                     ; Write template
  STA text_ptr
  LDA lut_StatsPage+1, X
  STA text_ptr+1
  
  TXA
  LSR
  TAX
  
  LDY lut_TextPosition, X
  STY dest_y
  
  LDA #$1E               ; Set banks
  STA ret_bank
  LDA #$0D
  STA cur_bank  
  
  JSR DrawComplexString
  
  LDX tmp+4
  INX
  INX
  CPX #$06
  BCC loopText
  
  LDA tmp_gold         ; Restore gold
  STA gold
  LDA tmp_gold+1
  STA gold+1
  LDA tmp_gold+2
  STA gold+2

  RTS
  
ClearNT_L:
  LDA #$1E
  PHA
  LDA #>SwapPRG_skip
  PHA
  LDA #<SwapPRG_skip-1
  PHA
  
  LDA #>ClearNT
  PHA
  LDA #<ClearNT-1
  PHA
  LDA #$0E
  JMP SwapPRG_L
  
TurnMenuScreenOn_L:
  LDA #$1E
  PHA
  LDA #>SwapPRG_skip
  PHA
  LDA #<SwapPRG_skip-1
  PHA
  
  LDA #>TurnMenuScreenOn
  PHA
  LDA #<TurnMenuScreenOn-1
  PHA
  LDA #$0E
  JMP SwapPRG_L
  
