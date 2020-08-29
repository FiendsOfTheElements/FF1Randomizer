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
 
 .ORG $88D7
    
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
  LDA (lvlup_chmagic), Y         ; Increase max MP for this level by 1
  CLC
  ADC #$01
  STA (lvlup_chmagic), Y
  PLA
SkipMP:
  INY                               ; INY to look at next spell level
  CPY #ch_maxmp - ch_magicdata + 8  ; loop for all 8 bits (and all 8 spell levels)
  BNE MagicLoop
  JMP SkipMPGain
  
lut_MaxMP:                            
 ; Max MP for all classes: Fi, Th, BB, RM, WM, BM, Kn, Ni, Ma, RW, WW, BW
 .BYTE $00, $00, $00, $09, $09, $09, $04, $04, $00, $09, $09, $09  


 