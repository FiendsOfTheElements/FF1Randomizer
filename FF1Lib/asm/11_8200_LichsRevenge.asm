; Lich's Revenge modified Talk routines so all NPCs
;  fight the player.
;
; Last Update: 2021-02-12

unsram 		= $6000
items = unsram + $20
game_flags      = unsram + $0200 

item_tail = items + $0D
talkarray = $70

dialogue_1 = talkarray
dialogue_2 = talkarray+1
dialogue_3 = talkarray+2
item_id = talkarray+3
required_id = talkarray+4
battle_id = talkarray+5
object_id = talkarray+6

dlg_itemid = $61
dlgsfx = $7D

SetGameEventFlag = $907F
CheckGameEventFlag = $9079
ShowMapObject = $90A4

HideMapObject = $9273

DoClassChange = $95AE

InTalkBattle = $9600
InTalkReenterMap = $9618
CheckCanTake = $B180
InTalkDialogueBox = $963D
SkipDialogueBox = $9643

GiveItem = $9F10

 .ORG $8200
 
Talk_battleUnne:
  LDX required_id
  BEQ SetFlag_bU
    LDA items, X
    BEQ NoItem_bU
SetFlag_bU:    
      INC dlgsfx
      LDA dialogue_3
      JSR InTalkDialogueBox
      LDA battle_id
      JSR InTalkBattle
      LDY object_id
      JSR SetGameEventFlag
      JSR HideMapObject
      JSR InTalkReenterMap
      JMP SkipDialogueBox
NoItem_bU:      
  LDA dialogue_1
  RTS

  NOP
  NOP
  
Talk_battleGiveOnFlag:
  LDY required_id
  BEQ NoRequirement_bG
    JSR CheckGameEventFlag
    BCC FlagNotSet_bG
NoRequirement_bG:  
      LDA item_id
      STA dlg_itemid
      JSR CheckCanTake
      BCS CantTake_bG
        INC dlgsfx
        LDA dialogue_3
        JSR InTalkDialogueBox
        LDA battle_id
        JSR InTalkBattle
        LDY object_id
        JSR SetGameEventFlag
        JSR HideMapObject
        LDA dlg_itemid
        CLC
        JSR GiveItem
        JSR InTalkReenterMap
        JMP SkipDialogueBox
FlagNotSet_bG:
  LDA dialogue_1
CantTake_bG:  
  RTS
  
  NOP
  NOP
 
Talk_battleGiveOnItem:
  LDX required_id
  BEQ NoRequirement_bI
    LDA items, X
    BEQ NoItem_bI
NoRequirement_bI:    
      LDA item_id
      STA dlg_itemid
      JSR CheckCanTake
      BCS CantTake_bI
        INC dlgsfx
        LDA dialogue_3
        JSR InTalkDialogueBox
        LDA battle_id
        JSR InTalkBattle
        LDY object_id
        JSR SetGameEventFlag
        JSR HideMapObject
        LDA dlg_itemid
        CLC
        JSR GiveItem
        JSR InTalkReenterMap
        JMP SkipDialogueBox
NoItem_bI:
  LDA dialogue_1
CantTake_bI:  
  RTS  
  
  NOP
  NOP

Talk_battleBahamut:
  LDA item_tail		; he hasn't promoted you yet... so check to see if you have the tail
  BNE ClassChange_bB	; if you don't...
    LDA dialogue_2	; Show dialogue_2
    RTS                  
ClassChange_bB:	        ; otherwise (have tail), do the class change!
  INC dlgsfx		; Play jingle
  LDA dialogue_3	; Show dialogue_3
  JSR InTalkDialogueBox
  LDA battle_id		; Load battle ID
  JSR InTalkBattle	; Trigger the battle
  LDY object_id		; Load this object
  JSR SetGameEventFlag	; Set its flag
  JSR HideMapObject	; Hide it  
  JSR DoClassChange	; do class change
  JSR InTalkReenterMap  ; Reenter the map
  JMP SkipDialogueBox	; Skip extra dialogue box
  RTS  
  
  NOP
  NOP
  
Talk_lichReplace:
  LDA dialogue_3
  JSR InTalkDialogueBox
  LDA battle_id
  JSR InTalkBattle
  LDY object_id
  JSR SetGameEventFlag	; Set its flag
  JSR HideMapObject	; Hide it  
  LDY item_id
  JSR ShowMapObject
  JSR InTalkReenterMap
  JMP SkipDialogueBox	; Skip extra dialogue box
