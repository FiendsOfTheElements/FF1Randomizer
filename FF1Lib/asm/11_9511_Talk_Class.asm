;; Last Update : 2021-01-22

unsram 		= $6000
ch_stats        = unsram + $0100  ; MUST be on page bound.  Each character allowed $40 bytes, so use 00,40,80,C0 to index ch_stats
ch_class        = ch_stats + $00
ch_level        = ch_stats + $26
ch_ailments     = ch_stats + $01
ch_curhp        = ch_stats + $0A  ; 2 bytes
ch_maxhp        = ch_stats + $0C  ; 2 bytes

talkarray = $70
tmp = $10
dialogue_1 = talkarray
dialogue_2 = talkarray+1
dialogue_3 = talkarray+2
item_id = talkarray+3
required_id = talkarray+4
battle_id = talkarray+5
object_id = talkarray+6

from_select     = $65  ; shared
joy_a           = $24
joy_b           = $25
joy             = $20
lu_joyprev      = $64  ; shared
lu_cursor       = $61  ; shared

OBJID_BAHAMUT      = $0E                   ; Bahamut

CheckGameEventFlag = $9079                 ; Bank $0E
HideMapObject = $9273                      ; Bank $0E
DoClassChange = $95AE                      ; Bank $0E
DoClassChangeOne = $95C5                   ; Bank $0E
EnterLineupMenu = $9911                    ; Bank $0E
SwapPRG_L = $FE03                          ; Bank $1F
SwapPRG_skip = $FE07                       ; Bank $1F, jump to skip putting the bank on the stack
NewGame_LoadStartingStats = $C76D          ; Bank $1F
NewGame_LoadStartingStats_skip = $C783     ; Bank $1F, jump to just load one character's stats
LvlUp_NoDisplay = $87AA                    ; Bank $1B
InTalkReenterMap = $9618
InTalkDialogueBox = $963D
SkipDialogueBox = $9643
LineupMenu_ProcessJoy = $9A14              ; Bank $0E

 .ORG $9520 ; In bank 11

;; Talk routine to switch a party member with a new class
;;  [0] NPC requirement
;;  [1] Dialog when requirement met
;;  [2] Dialog when requirement isn't met
;;  [3] ID of the class to change to
Talk_Class:
  LDY talkarray                ; Check if there's a requirement (Event)
  BEQ NoFlag                   ; If so...
    JSR CheckGameEventFlag     ; Check if the flag is set
    BCC NotSet
NoFlag:
  LDA talkarray+1
  JSR InTalkDialogueBox        ; Show joining dialogue
  JSR EnterLineupMenu_L        ;  and go to lineup menu
  CPY #$00
  BNE CancelChoice             ; Check Y if choice was canceled
    LDA talkarray+3              ;  Change the class ID
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
    LDY talkarray+6              ; And hide this NPC
    JSR HideMapObject
CancelChoice:
    JSR InTalkReenterMap         ; Reenter map
    JMP SkipDialogueBox          ; Don't show dialogue
NotSet:
  LDA talkarray+2              ; Default dialogue
  RTS


 .ORG $9F50 ; In bank 11
LoadStats:
  LDA #$11                     ; Push bank
  PHA
  LDA #>SwapPRG_skip           ; Put RTS address on stack
  PHA                          ;  so we switch back to bank 0E
  LDA #<SwapPRG_skip-1         ;  after LoadStartingStats switch to bank 00
  PHA
  LDA #>NewGame_LoadStartingStats_skip   ; Put RTS address on stack
  PHA                                    ;  because we'll be switching bank
  LDA #<NewGame_LoadStartingStats_skip-1
  PHA
  LDA #$00
  JMP SwapPRG_L
  
ResetStats:
  LDY #$18
  STX tmp
  LDA #>ch_stats
  STA tmp+1
UnequipLoop:
  LDA (tmp),Y          ; Unequip all equipment
  AND #$7F
  STA (tmp),Y
  INY  
  CPY #$20
  BNE UnequipLoop
  LDY #$00
  LDA #$63
  STA tmp+1
  LDA #$00
MagicLoop:                    ; Zero out all spell values and MPs
  STA (tmp), Y
  INY
  CPY #$2F
  BNE MagicLoop
  RTS  
 
LevelUp:
  LDA #$11                     ; Push bank
  PHA
  LDA #>SwapPRG_skip           ; Put RTS address on stack
  PHA                          ;  so we switch back to bank 0E
  LDA #<SwapPRG_skip-1         ;  after LoadStartingStats switch to bank 00
  PHA
  LDA #>LvlUp_NoDisplay       ; Put RTS address on stack
  PHA                          ;  because we'll be switching bank
  LDA #<LvlUp_NoDisplay-1
  PHA
  TXA
  LSR
  LSR
  LSR
  LSR
  LSR
  LSR
  STA tmp
  LDA #$1B  
  JMP SwapPRG_L
  
EnterLineupMenu_L:
  LDA #$11                     ; Push bank
  PHA
  LDA #>SwapPRG_skip           ; Put RTS address on stack
  PHA                          ;  so we switch back to bank 0E
  LDA #<SwapPRG_skip-1         ;  after LoadStartingStats switch to bank 00
  PHA
  LDA #>EnterLineupMenu       ; Put RTS address on stack
  PHA                          ;  because we'll be switching bank
  LDA #<EnterLineupMenu-1
  PHA
  LDA #$00                    ; 
  STA from_select             ; Indicate we're not moving party member around
  LDA #$0E  
  JMP SwapPRG_L  


 .ORG $98C0 ; In bank 0E
 
LineupMenu_PJ_Hijack:
  LDA from_select              ; If select was pressed, we want normal LineupMenu
  BEQ NotSelect
    JMP LineupMenu_ProcessJoy
NotSelect:

  LDA joy_b         ; check for B or A button presses
  BNE B_Pressed
  LDA joy_a
  BNE A_Pressed

  LDA joy           ; otherwise, check for direction presses.  Get joy
  AND #$0C          ; isolate up/down buttons
  CMP lu_joyprev    ; compare to prev down
  BEQ NoButton    ; if no changes, exit
  STA lu_joyprev    ; otherwise, record changes
  AND #$0C          ; mask again (to refresh Z flag)
  BEQ NoButton    ; if no buttons down, exit

  CMP #$08          ; see if they pressed up or down
  BEQ Up

Down:
  LDA lu_cursor
  CLC
  ADC #$08          ; add 8 to cursor (next item down)
  BNE MoveDone
Up:
  LDA lu_cursor
  SEC
  SBC #$08          ; subtract 8 from cursor (next item up)

MoveDone:
  AND #$1F          ; wrap so it stays within the 4 slots
  STA lu_cursor     ; and write it back to the cursor
  RTS               ;  then exit

A_Pressed:
  LDA #0
  STA joy_a         ; clear A button catcher
  TAY               ; set Y to 0, we selected a character
  LDA lu_cursor     ; get char Id from lu_cursor
  ASL
  ASL
  ASL
  TAX               ; put it in X
  BCC DoneExit      

B_Pressed:          ; B was pressed 
  LDY joy_b         ; store B in Y, we canceled
DoneExit:
  PLA                       ; drop return address (so when we return from here,
  PLA                       ;  we exit the Lineup menu completely)
NoButton:
  RTS
    