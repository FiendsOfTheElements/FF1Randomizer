;when enabled this is used instead of 1B_8818_LevlUp_levelUp.asm a jump is made from inside the level up function at 88d7 to here.

levindex = $6BAD       ; local, stores the index to level up stats
lvlupptr = $82         ; local, pointer to level up data
classid  = $688E 
lvlup_chmagic       = $84 
lvlup_chstats       = $86 

unsram          = $6000 
ch_stats        = unsram + $0100
ch_level	= ch_stats + $26
ch_magicdata    = unsram + $0300  ; must be on page bound
ch_spells       = ch_magicdata
ch_mp           = ch_magicdata + $20
ch_curmp        = ch_mp + $00
ch_maxmp        = ch_mp + $08

SkipMPGain = $891C
lut_MaxMP = $8902

.ORG $88D7
JMP LevelUpMagicCurMP
 
.ORG $9830
lut_EnableMpUpOnMaxGain:
.byte $01, $01, $01, $01, $01, $01, $01, $01, $01, $01, $01, $01 

LevelUpMagicCurMP:
  LDX classid                    
  LDY #$01
  LDA (lvlupptr), Y              ; get leveldata[1] byte (MP gains)
  LDY #ch_maxmp - ch_magicdata   ; set Y to index map MP
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

  ;Increase Max MP
  LDA (lvlup_chmagic), Y         
  CLC
  ADC #$01
  STA (lvlup_chmagic), Y

  ;Increase Current MP if this class is on the list
  LDA lut_EnableMpUpOnMaxGain, X
  BEQ SkipCurrentMpGain

  ;Increase current MP for this level by 1, the data is at pointer -8
  ;push the counter/pointer
  TYA
  PHA 
  SEC
  SBC #$08
  TAY

  LDA (lvlup_chmagic), Y         
  CLC
  ADC #$01
  STA (lvlup_chmagic), Y

  ;restore the counter/pointer
  PLA 
  TAY 
SkipCurrentMpGain:
  PLA
SkipMP:
  INY                               ; INY to look at next spell level
  CPY #ch_maxmp - ch_magicdata + 8  ; loop for all 8 bits (and all 8 spell levels)
  BNE MagicLoop
  JMP SkipMPGain

