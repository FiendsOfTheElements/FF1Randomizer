; Routines to draw and show the info screen for each class when
;  using Randomized Class.


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

submenu_targ    = $66
char_index      = $67

btltmp = $90

format_buf      = $0060

unsram          = $6000  ; $400 bytes 
ch_stats        = unsram + $0100  ; MUST be on page bound.  Each character allowed $40 bytes, so use 00,40,80,C0 to index ch_stats
ch_class        = ch_stats + $00
str_classNone   = $8298
ptygen          = $0300  ; $40 bytes, shared
ptygen_class    = ptygen
ptygen_empty    = ptygen + $01
BANK_THIS = $1E


PtyGen_DrawScreen   = $80A4
PtyGen_Frame        = $820F
DoNameInput         = $812C
PtyGen_DrawOneText  = $82B0

ClearOAM = $C43C
DrawComplexString = $DE36      ; Bank $1F
DrawBox = $E063                ; Bank $1F
SwapPRG_L = $FE03              ; Bank $1F


DrawOne = $835B                ; Bank $0E
EnterStatusMenu = $B4AD        ; Bank $0E
PlaySFX_MenuSel = $84EB
TurnMenuScreenOn = $855E
MenuWaitForBtn_SFX_0E = $B613     ; Bank $0E
MenuWaitForBtn_SFX_1E = $851A  ; Bank $1E

MenuFrame = $852C

lut_DescriptionPointer = $8950
lut_AllowedClasses     = $85B0
lut_ClassMask          = lut_AllowedClasses + $04

 .ORG $80C1

DoPartyGen_OnCharacter:          ; Add select button check to partyGen screen

allowedmask = btltmp

  LDX char_index                 ; Get char_index
  TXA
  LSR
  LSR
  LSR
  LSR                            ; Shift to make it class index
  TAY
  LDA lut_AllowedClasses, Y
  STA allowedmask                 
  JSR PtyGen_DrawScreen          ; Draw the Party generation screen

MainLoop:
  LDA #$00                       ; Reset select button
  STA joy_select
  JSR PtyGen_Frame               ; Do a frame and update joypad input
  LDA joy_select                 ; Check if select was pressed
  BEQ NoSelect
    JMP EnterFromPtygen          ; If yes, show info screen
NoSelect:
  LDA joy_a
  BNE DoNameInput                ; If A was pressed, do name input
  LDA joy_b
  BEQ NotB
    SEC                          ; If B pressed -- just SEC and exit
    RTS

NotB:
 LDA joy
 AND #$0F
 CMP joy_prevdir
 BEQ MainLoop                    ; If there was no change in directional input, loop to another frame
 STA joy_prevdir                 ; otherwise, record new directional input as prevdir
 CMP #$00                        ; if directional input released (rather than pressed)
 BEQ MainLoop                    ;   loop to another frame.
 LDX char_index                  ; Otherwise, if any direction was pressed:
retry:
 LDA ptygen_class,X              ; A = class id.  0=FI, 1=TH, BB, RM, WM, BM, FF=None
 CLC
 ADC #1
 CMP #$0C
 BNE IsNotNone
   LDA #$FF
IsNotNone:
  STA ptygen_class, X
  TAY
  INY                            ; since the indicies are off by 1 to accomidate for class 0xFF
  LDA lut_ClassMask,Y
  BIT allowedmask
  BEQ retry                      ; retry if disallowed.
  LDA #$01
  STA menustall

  JSR PtyGen_DrawOneText         ; X needs to be char_index here (it is)
  JMP MainLoop
 

 .ORG $8800

EnterFromPtygen:               ; Enter info menu from PartyGen
  JSR ClearOAM                 ; Clear sprites
  LDA char_index               ; Get character index
  LSR                          ;  and shift it to check which side 
  LSR                          ;  that character is on
  LSR
  LSR
  LSR
  BCS RightSide                ; Is is the index from a character on the right?
    LDX #$00                   ;  if not, draw the two sprites on the left side
    JSR DrawOne
    LDX #$20  
    JSR DrawOne
    LDA #$11                   ; Then set box coordinates on the right
    STA box_x
    LDA #$02
    STA box_y
    JMP DoneDraw
RightSide:                     ; Draw sprites on the right side
  LDX #$10 
  JSR DrawOne
  LDX #$30 
  JSR DrawOne
  LDA #$03                     ; Set box coordinates on the left
  STA box_x
  LDA #$02
  STA box_y
DoneDraw:
  LDX char_index               
  LDA ptygen_class, X          ; Get character's class
  CMP #$FF                     ; Check if it's a None
  BNE NotNoneP 
    LDA #$0C                   ; If it is, set info index to none class
NotNoneP:  
  ASL
  TAX                          ; Shift index (2 bytes per info) and put in X
  JSR DrawInfoBox
  LDA #$80
  PHA
  LDA #$C0
  PHA
  JMP MenuWaitForBtn_SFX_1E
 
EnterFromStatus:               ; Enter info menu from Status screen
  LDA #$11                     ; Set box coordinates
  STA box_x
  LDA #$02
  STA box_y
  
  LDX char_index 
  LDA ch_class, X              ; Get character's class
  CMP #$FF                     ; Check if it's a None
  BNE NotNoneS
    LDA #$0C                   ; If it is, set info index to none class
NotNoneS:
  ASL                          ; Shift index (2 bytes per info) and put in X 
  TAX 
  JSR DrawInfoBox              ; And Draw info Box
  LDA #>MenuWaitForBtn_SFX_0E  ; Put RTS address on stack
  PHA                          ;  because we'll be switching bank
  LDA #<MenuWaitForBtn_SFX_0E-1
  PHA
  LDA #$0E                     ; Switch to bank 0E
  STA cur_bank
  JMP SwapPRG_L
 
DrawInfoBox:
  LDA #$0D                     ; Set box size
  STA box_wd
  LDA #$1A
  STA box_ht

  LDA #$00
  STA $2001                    ; Turn off the PPU
  LDA #$00
  STA menustall                ; Disable menustalling (PPU is off)
  TXA                          ; Get class index back
  PHA                          ;  and put it on the stack
  JSR DrawBox                  ; Draw the box
    
  LDA #$70                     ; Write template
  STA text_ptr
  LDA #$89
  STA text_ptr+1
  LDA box_x
  STA dest_x
  INC dest_x
  LDA box_y
  STA dest_y
  INC dest_y
  
  LDA #BANK_THIS               ; Set banks
  STA cur_bank
  STA ret_bank
  
  JSR DrawComplexString        ; Then draw complex string
  
  PLA                          ; Get class index back from stack
  TAX                          ; And set pointer to info text
  LDA lut_DescriptionPointer, X
  STA text_ptr
  LDA lut_DescriptionPointer+1, X
  STA text_ptr+1
  
  LDA dest_y                   ; Position info text a bit lower
  CLC
  ADC #$02
  STA dest_y
  JSR DrawComplexString        ; Draw info text
  JSR TurnMenuScreenOn         ; Show box on the screen
  RTS

 .ORG $8910
 
StatusWaitForBtn_SFX:
  JSR MenuFrame               ; Do a frame
  LDA joy_a
  ORA joy_select
  BNE OkPressed               ; If A or select was pressed, show info box
  LDA joy_b
  BEQ StatusWaitForBtn_SFX    ; If B was pressed
    LDA #$00
    STA joy_b
    LDA #$0E                  ; Switch to bank 0E
    STA cur_bank              ;  returning to main menu
    JMP SwapPRG_L 
OkPressed:
    LDA #$00
    STA joy_a                 ; clear buttons
    STA joy_b
    STA joy_select
    JMP EnterFromStatus       ; Show info box 



