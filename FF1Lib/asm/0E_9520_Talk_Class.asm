unsram 		= $6000
ch_stats        = unsram + $0100  ; MUST be on page bound.  Each character allowed $40 bytes, so use 00,40,80,C0 to index ch_stats
ch_class        = ch_stats + $00
ch_level        = ch_stats + $26
ch_ailments     = ch_stats + $01
ch_curhp        = ch_stats + $0A  ; 2 bytes
ch_maxhp        = ch_stats + $0C  ; 2 bytes

tmp = $10

OBJID_BAHAMUT      = $0E                   ; Bahamut

CheckGameEventFlag = $9079                 ; Bank $0E
HideMapObject = $9273                      ; Bank $0E
DoClassChange = $95AE                      ; Bank $0E
DoClassChangeOne = $95C5                   ; Bank $0E
SwapPRG_L = $FE03                          ; Bank $1F
SwapPRG_skip = $FE07                       ; Bank $1F, jump to skip putting the bank on the stack
NewGame_LoadStartingStats = $C76D          ; Bank $1F
NewGame_LoadStartingStats_skip = $C783     ; Bank $1F, jump to just load one character's stats
LvlUp_NoDisplay = $87AA                    ; Bank $1B

 .ORG $9520

;; Talk routine to switch the 4th slot party member with a new class
;;  [0] NPC requirement
;;  [1] Dialog when requirement met
;;  [2] Dialog when requirement isn't met
;;  [3] ID of the class to change to
Talk_Class:
  LDY tmp                      ; Check if there's a requirement (Event)
  BEQ NoFlag                   ; If so...
    JSR CheckGameEventFlag     ; Check if the flag is set
    BCC NotSet
NoFlag:
  LDA tmp+1                    ; Put dialogue ID on the stack
  PHA                          ;  because some routines will overwrite it
  LDX #$C0                     ; Select 4th party member slot
  LDA tmp+3                    ;  and change the class ID
  STA ch_class, X
  LDA #$00                     ; Zero out level, ailments and HP
  STA ch_level, X
  STA ch_ailments, X
  STA ch_curhp+1, X
  STA ch_maxhp+1, X
  JSR ResetStats               ; Unequip equipment and remove spells and MP
  JSR LoadStats                ; Load starting stats
  LDY #OBJID_BAHAMUT           ; Check if the party already class changed
  JSR CheckGameEventFlag       ; If so, promote this character
  BCC SkipClassChange
  JSR DoClassChangeOne
SkipClassChange:
  JSR LevelUp                  ; Level Up to the replaced character level
  LDY tmp+6                    ; And hide this NPC
  JSR HideMapObject
  PLA                          ; Get dialogue back from stack
  RTS
NotSet:
  LDA tmp+2                    ; Default dialogue
  RTS  


 .ORG $9F50
LoadStats:
  LDA #$0E                     ; Push bank
  PHA
  LDA #>SwapPRG_skip           ; Put RTS address on stack
  PHA                          ;  so we switch back to bank 0E
  LDA #<SwapPRG_skip-1         ;  after LoadStartingStats switch to bank 00
  PHA
  LDA #>NewGame_LoadStartingStats_skip   ; Put RTS address on stack
  PHA                                    ;  because we'll be switching bank
  LDA #<NewGame_LoadStartingStats_skip-1
  PHA
  LDX #$C0                     ; Select 4th party member
  LDA #$00
  JMP SwapPRG_L
  
ResetStats:
  LDY #$18
UnequipLoop:
  LDA ch_stats+$C0,Y          ; Unequip all equipment
  AND #$7F
  STA ch_stats+$C0,Y
  INY  
  CPY #$1F
  BNE UnequipLoop
  LDY #$00
  LDA #$00
MagicLoop:                    ; Zero out all spell values and MPs
  STA $63C0, Y
  INY
  CPY #$2F
  BNE MagicLoop
  RTS  
 
LevelUp:
  LDA #>LvlUp_NoDisplay       ; Put RTS address on stack
  PHA                          ;  because we'll be switching bank
  LDA #<LvlUp_NoDisplay-1
  PHA
  LDA #$03                    ; Select 4th party member
  STA tmp
  LDA #$1B  
  JMP SwapPRG_L
