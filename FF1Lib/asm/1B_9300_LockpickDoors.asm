.include "Constants.inc"
.include "variables.inc"

SwapPRG = $FE03
SMMove_Norm_RTS = $CE52

;currently in the MENU_BANK
;this replacement could be a lot less bytes if we use a temporary memory to store A
;A gets overwriten by the bank swap method and we cant grab it from the stack without some heavy stack manipulation
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
    ;all of this checking is rom space ineffecient but I don't know if we have available temp memory to do an index
    ;check to see if they have a thief/ninja in the party, and they're at or above level 10, the class and level are randomizable at rom creation
    LDX ch_level
    CPX #$09
    BCC Slot1UnderLeveled
    LDX ch_class
    CPX #$01
    BEQ DoorUnlocked
    CPX #$07
    BEQ DoorUnlocked

    Slot1UnderLeveled:
    LDX ch_level+$40
    CPX #$09
    BCC Slot2UnderLeveled
    LDX ch_class+$40
    CPX #$01
    BEQ DoorUnlocked
    CPX #$07
    BEQ DoorUnlocked

    Slot2UnderLeveled:
    LDX ch_level+$80
    CPX #$09
    BCC Slot3UnderLeveled
    LDX ch_class+$80
    CPX #$01
    BEQ DoorUnlocked
    CPX #$07
    BEQ DoorUnlocked

    Slot3UnderLeveled:
    LDX ch_level+$C0
    CPX #$09
    BCC Slot4UnderLeveled
    LDX ch_class+$C0
    CPX #$01
    BEQ DoorUnlocked
    CPX #$07
    BEQ DoorUnlocked

    Slot4UnderLeveled:
    LDY #$01
    TAX
    LDA #BANK_MENUS
    JMP SwapPRG

  DoorUnlocked:
    LDY #$00
    TAX
    LDA #BANK_MENUS
    JMP SwapPRG

;105 bytes
;8A 4A 29 03 C9 02 D0 59 A2 00 86 45 AE 25 60 D0 50 AE 26 61 E0 09 90 0B AE 00 61 E0 01 F0 42 E0 07 F0 3E AE 66 61 E0 09 90 0B AE 40 61 E0 01 F0 30 E0 07 F0 2C AE A6 61 E0 09 90 0B AE 80 61 E0 01 F0 1E E0 07 F0 1A AE E6 61 E0 09 90 0B AE C0 61 E0 01 F0 0C E0 07 F0 08 A0 01 AA A9 0E 4C 03 FE A0 00 AA A9 0E 4C 03 FE
