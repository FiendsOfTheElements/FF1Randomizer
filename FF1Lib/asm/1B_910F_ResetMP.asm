; Reset MP when doing class change for Randomized Promotions - 2020-12-17

lvlupptr = $82         ; local, pointer to level up data
lvlup_chmagic       = $84 
tmp = $10
lut_LevelUpDataPtrs = $8A71

unsram          = $6000 
ch_stats        = unsram + $0100
ch_class        = ch_stats
ch_level	= ch_stats + $26
ch_magicdata    = unsram + $0300  ; must be on page bound
ch_spells       = ch_magicdata
ch_mp           = ch_magicdata + $20
ch_curmp        = ch_mp + $00
ch_maxmp        = ch_mp + $08

SwapPRG_L = $FE03                          ; Bank $1F
SwapPRG_skip = $FE07                       ; Bank $1F, jump to skip putting the bank on the stack

lut_MaxMP = $8902
lut_promote = $9DF0

; Bank 11
 .ORG $8000

DoClassChangeMod:
  BMI Skip			    ; Skip Nones
  LDA lut_promote, Y	            ; Find to which class it promote in the table
  STA ch_class, X		    ; Change class
  JSR JumpResetMP                   ; Reset MPs
Skip:
  RTS				   
 
 .ORG $9DD0
 
JumpResetMP:
  LDA #$11                     ; Push bank
  PHA
  LDA #>SwapPRG_skip           ; Put RTS address on stack
  PHA                          ;  so we switch back to bank 11
  LDA #<SwapPRG_skip-1         ;  after we ResetMP in bank 1B
  PHA
  LDA #>ResetMP                ; Put RTS address on stack
  PHA                          ;  because we'll be switching bank
  LDA #<ResetMP-1
  PHA
  LDA #$1B
  JMP SwapPRG_L

; Bank 1B
 
 .ORG $9100

; lut generated to check if we need to recalculate MP when promoting
lut_Recomp:
 .BYTE $00, $00, $00, $00, $00, $00, $00, $00, $00, $00, $00, $00  

 .ORG $910F
NoRecomp:
  RTS

ResetMP:
  LDA ch_class,X            ; Load class and check
  TAY                       ;  with lut
  LDA lut_Recomp, Y
  BEQ NoRecomp              ; RTS if not

  LDA #>ch_maxmp            ; Set max mp address
  STA lvlup_chmagic+1       ;  into lvlup_chmagic
  STX lvlup_chmagic
  LDA #<ch_maxmp
  CLC
  ADC lvlup_chmagic
  STA lvlup_chmagic
  LDY #$00
  LDA #$02
  STA (lvlup_chmagic), Y   ; Set lv1 MP at 2
  INY                      
  LDA #$00
ZeroMP:                    ; Zero out all the other levels
  STA (lvlup_chmagic), Y
  INY
  CPY #$08
  BNE ZeroMP
  
  LDA ch_class, X                  
  ASL 
  TAY
  LDA lut_LevelUpDataPtrs, Y    ; Set pointer to levelup data
  STA lvlupptr
  LDA lut_LevelUpDataPtrs+1, Y
  STA lvlupptr+1

  LDA #$01                          
  STA tmp
  LDA ch_level, X
  STA tmp+1
  INC tmp+1                     ; Set counter from 1 to level+1
  STX tmp+2                     ; Backup X 
   
  LDA ch_class, X               
  TAX

MPLoop:
  LDY #$01
  LDA (lvlupptr), Y              ; get leveldata[1] byte (MP gains)
  LDY #$00                       ; set Y to index map MP
MagicLoop:  
  PHA                            ; Push level data on the stack
  LDA (lvlup_chmagic), Y         ;  and get current slot MP
  CMP lut_MaxMP, X               ; Check if we're at max MP
  BCC NotMaxMP                   
  PLA                            ; If we are, get level data from stack 
  LSR                            ;  and check next slot
  JMP SkipMP                      
NotMaxMP:
  PLA                            ; If we're not, get level data back
  LSR                            ;  and see if we're gaining MP
  BCC SkipMP                     
  PHA                            ; Push level data (again)
  LDA (lvlup_chmagic), Y         ; Increase max MP for this level by 1
  CLC
  ADC #$01
  STA (lvlup_chmagic), Y
  PLA
SkipMP:
  INY                            ; INY to look at next spell level
  CPY #$08                       ; loop for all 8 bits (and all 8 spell levels)
  BNE MagicLoop
  CLC                            
  LDA #$02                       ; Move level up pointer up 2 bytes
  ADC lvlupptr                   ;  to next level
  STA lvlupptr                   
  LDA #$00
  ADC lvlupptr+1
  STA lvlupptr+1
  INC tmp                        ; Inc the counter to current level
  LDA tmp
  CMP tmp+1
  BNE MPLoop                     ; Loop if we didn't do all levels yet
  
  LDX tmp+2
  LDA #>ch_mp                    ; Set mp address
  STA tmp+1
  STX tmp
  LDA #<ch_mp
  CLC
  ADC tmp
  STA tmp
  LDY #$00
FreeMP:                        
  LDA (lvlup_chmagic), Y        ; Get max MP
  LSR                           ; Halve it
  STA (tmp), Y                  ; Store it in current MP
  INY
  CPY #$08
  BNE FreeMP                    ; Loop for all levels
  RTS 
    



 