.include "Constants.inc"
.include "variables.inc"

SwapPRG = $FE03
SMMove_Norm_RTS = $CE52

;currently in the MENU_BANK
;this replacement could be a lot less bytes if we use a temporary memory to store A
;A gets overwritten by the bank swap method and we cant grab it from the stack without some heavy stack manipulation
;so we store it in X to carry through bank swaps
.org $CE53
TAX
TYA
PHA
LDA #$1B
jsr SwapPRG
jsr CheckDoorLocked
CPY #$01
PLA
TAY
TXA
BCS SMMove_Norm_RTS ;save a byte by just branching to a nearby rts instead of having an rts next line and BCC over it

;exact change replacing 18 bytes at CE53
;AA 98 48 A9 1B 20 03 FE 20 00 93 C0 01 68 A8 8A B0 ED

;bank 1b
.ORG $9300
CheckDoorLocked:
  TXA
  LSR A                                       ; downshift to get the door bits into the low 2 bits
  AND #(TP_SPEC_DOOR | TP_SPEC_LOCKED) >> 1   ; mask out the door bits

  CMP #TP_SPEC_LOCKED >> 1  ; see if the door is locked
  BNE DoorUnlocked             ; if not.. open the door
  LDX #0                    ; otherwise (door is locked)
  STX tileprop+1            ; erase the secondary attribute byte (prevent it from being a locked shop)
  LDX item_mystickey        ; check to see if the player has the key
  BNE DoorUnlocked             ; if they do, open the door
    ;all of this checking is rom space inefficient but I don't know if we have available temp memory to do an index
    ;check to see if they have a thief/ninja in the party, and they're at or above level 10, the class and level are randomizable at rom creation
    PHA
    LDA #$00
    PHA
    LDY #$00
    CheckLevel:
      LDX ch_level,Y
      CPX #$09
      BCS CheckClass
    NextClass:
      CLC
      PLA
      ADC #$40
      BCS UnderLeveled
      TAY
      PHA
      JMP CheckLevel
    CheckClass:
      LDX ch_class,Y
      CPX #$01
      BEQ DoorUnlocked
      CPX #$07
      BEQ DoorUnlocked
      JMP NextClass

  UnderLeveled:
    LDY #$01
    PLA
    TAX
    LDA #BANK_MENUS
    JMP SwapPRG

  DoorUnlocked:
    LDY #$00
    PLA
    PLA
    TAX
    LDA #BANK_MENUS
    JMP SwapPRG

;74 bytes
;8A4A2903C902D038A2008645AE2560D02F48A90048A000BE2661E009B00B18686940B013A8484C1793BE0061E001F010E007F00C4C1E93A00168AAA90E4C03FEA0006868AAA90E4C03FE
