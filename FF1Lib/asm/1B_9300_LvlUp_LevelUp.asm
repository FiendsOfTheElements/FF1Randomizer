levindex = $6BAD       ; local, stores the index to level up stats
lvlupptr = $82         ; local, pointer to level up data
classid  = $688E 
lvlup_chmagic       = $84 
lvlup_chstats       = $86 
btltmp = $90

unsram          = $6000 
ch_stats        = unsram + $0100
ch_level	= ch_stats + $26
ch_clevel	= ch_level + $01  ; Unused byte after character level to store caster level
ch_magicdata    = unsram + $0300  ; must be on page bound
ch_spells       = ch_magicdata
ch_mp           = ch_magicdata + $20
ch_curmp        = ch_mp + $00
ch_maxmp        = ch_mp + $08

lut_LevelUpDataPtrs = $8A71

CLS_BB = $02
CLS_KN = $06
CLS_NJ = $07
CLS_MA = $08
  
 .ORG $88D7
  LDA classid                    
  CMP #CLS_BB+1
  BCC SkipMPGain                 ; skip MP increase for Fighters, Thieves and Black Belts
  CMP #CLS_MA                    ; and Masters.
  BEQ SkipMPGain                 
    
  LDY #$01
  LDA (lvlupptr), Y              ; get leveldata[1] byte (MP gains)
  LDY #ch_maxmp - ch_magicdata   ; set Y to index map MP

  JSR NewMpRoutine               ; Jump to new routine then go on
  JMP SkipMPGain
  
 .ORG $891C

SkipMPGain:
 
 .ORG $9300

NewMpRoutine:
  LDA classid                   ; check the class
  CMP #CLS_KN
  BEQ IsKnNi
    CMP #CLS_NJ
    BEQ IsKnNi                  ; Knights/Ninjas cap at 4 MP and don't gain caster level
      LDA lvlupptr              ; Backup level up pointer, since we're going to replace it with caster level
      STA btltmp
      LDA lvlupptr+1
      STA btltmp+1
      
      LDY #ch_clevel - ch_stats ; Load current caster level
      LDA (lvlup_chstats), Y
      STA levindex              ; Use is as level up index
      CLC
      ADC #1                    ; Increment caster level
      STA (lvlup_chstats), Y

      ASL levindex              ; Double levindex (2 bytes per level)
      
      LDA classid               
      ASL
      TAY
    
      LDA lut_LevelUpDataPtrs, Y      ; Calc the pointer to the level up data for this level for
      CLC                             ;   this class.
      ADC levindex                    
      STA lvlupptr
      LDA lut_LevelUpDataPtrs+1, Y
      ADC #$00
      STA lvlupptr+1        
      LDA #9-1                  ;  all other classes cap at 9 MP
      BNE NotKnNi
IsKnNi:
  LDA #4-1                      ; (KN/NJ jumps here)
NotKnNi:
  TAX                           ; Use X to backup max mp  
  LDY #$01                      
  LDA (lvlupptr), Y             ; get leveldata[1] byte (MP gains)
  LDY #ch_maxmp - ch_magicdata  ; set Y to index map MP
MagicUpLoop:
  PHA                           ; Push level data on the stack
  TXA                           ;  and check to see if we're at max MP already
  CMP (lvlup_chmagic), Y        
  BCS MPNotMax                  ; If we're not, go on with raising MP
  PLA                           ; If we are, pull level data
  LSR                           ;  shift it
  JMP MPDone                    ;  and skip raising MP
MPNotMax:
  PLA                           ; Pull level data from the stack
  LSR                           ;  shift out low bit
  BCC MPDone                    ;  if set...
    PHA                         ; Push level data (again)
    LDA (lvlup_chmagic), Y      ; increase max MP for this level by 1
    CLC
    ADC #$01
    STA (lvlup_chmagic), Y
    PLA
MPDone:
  INY                               ; INY to look at next spell level
  CPY #ch_maxmp - ch_magicdata + 8  ; loop for all 8 bits (and all 8 spell levels)
  BNE MagicUpLoop

  LDA btltmp                    ; Restore normal level up pointer
  STA lvlupptr
  LDA btltmp+1
  STA lvlupptr+1
  RTS  
  
 

 
