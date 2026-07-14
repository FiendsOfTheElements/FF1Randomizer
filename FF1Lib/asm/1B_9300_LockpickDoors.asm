; UPDATED 06/06/26 for use with tracker

.include "Constants.inc"
.include "variables.inc"

door_bits = $6E03   ; unused expansion ram ($6E00 is used as a tmp gold value elsewhere)
lockpicking_status = $6E04


SwapPRG = $FE03
SMMove_Norm_RTS = $CE52

tileprop = $44
item_mystickey = $6025
BANK_MENUS     = $0E
TP_SPEC_DOOR   = %00000010
TP_SPEC_LOCKED = %00000100

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

;; new version
;

game_flags = $6200
OBJID_REVEALUNLOCKEDDOOR = $FA
;bank 1b
.ORG $9300
CheckDoorLocked:
  TXA
  LSR A                                       ; downshift to get the door bits into the low 2 bits
  AND #%00000011;  #(TP_SPEC_DOOR|TP_SPEC_LOCKED)>>1   ; mask out the door bits
  STA door_bits
  CMP #%00000010;  #TP_SPEC_LOCKED>>1    ; see if the door is locked
  BNE DoorUnlocked          ; if not.. open the door
    LDX #0                    ; otherwise (door is locked)
    STX tileprop+1            ; erase the secondary attribute byte (prevent it from being a locked shop)
    LDX item_mystickey        ; check to see if the player has the key
    BNE SetDoorFlag             ; if they do, open the door
      LDX #$00
      STX lockpicking_status
      JSR CheckLockPicking
      LDX #$40
      JSR CheckLockPicking
      LDX #$80
      JSR CheckLockPicking
      LDX #$C0
      JSR CheckLockPicking
      ; A has lockpicking_status 1 if lockpicking achieved, 0 otherwise
      EOR #1 ; flip the bit
      TAY
      BNE Exit
  SetDoorFlag:
    LDA game_flags+OBJID_REVEALUNLOCKEDDOOR
    ORA #GAME_EVENT_FLAG
    STA game_flags+OBJID_REVEALUNLOCKEDDOOR
  DoorUnlocked:
    LDY #$00
  Exit:
    LDX door_bits
    LDA #BANK_MENUS
    JMP SwapPRG




.ORG $934D
; (set by randomizer)
; level requirement - 1
; base class
; promoclass 
lut_LockPicking:
.BYTE #$0E #$01 #$07



;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;
;; CheckLockPicking
;; 
;; in:
;;                      X: ch slot ($00, $40, $80, or $C0)
;;     lockpicking_status: 1 for lockpicking achieved, 0 otherwise
;;
;; out:
;;                      A: most recent lockpicking_status
;;                      Y: 1 lockpicking achieved for this slot, 0 otherwise
;;     lockpicking_status: updated lockpicking_status
;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;

.ORG $9350
CheckLockPicking:
ch_class = $6100
ch_level = $6126

LDA ch_level,X
CMP lut_LockPicking ;; check level first
BCC NoLockPicking
  LDA ch_class,X
  CMP lut_LockPicking+1
  BEQ LockPickingAchieved
    CMP lut_LockPicking+2
    BEQ LockPickingAchieved
NoLockPicking:
  LDA #0
  BEQ ExitCheckLockPicking
LockPickingAchieved
  LDA #1
ExitCheckLockPicking
  TAY
  ORA lockpicking_status
  STA lockpicking_status
RTS





;; old version
;bank 1b
; .ORG $9300
; CheckDoorLocked:
;   TXA
;   LSR A                                       ; downshift to get the door bits into the low 2 bits
;   AND #(TP_SPEC_DOOR | TP_SPEC_LOCKED) >> 1   ; mask out the door bits

;   CMP #TP_SPEC_LOCKED >> 1  ; see if the door is locked
;   BNE DoorUnlocked             ; if not.. open the door
;   LDX #0                    ; otherwise (door is locked)
;   STX tileprop+1            ; erase the secondary attribute byte (prevent it from being a locked shop)
;   LDX item_mystickey        ; check to see if the player has the key
;   BNE DoorUnlocked             ; if they do, open the door
;     ;all of this checking is rom space ineffecient but I don't know if we have available temp memory to do an index
;     ;check to see if they have a thief/ninja in the party, and they're at or above level 10, the class and level are randomizable at rom creation
;     LDX ch_level
;     CPX #$09
;     BCC Slot1UnderLeveled
;     LDX ch_class
;     CPX #$01
;     BEQ DoorUnlocked
;     CPX #$07
;     BEQ DoorUnlocked

;     Slot1UnderLeveled:
;     LDX ch_level+$40
;     CPX #$09
;     BCC Slot2UnderLeveled
;     LDX ch_class+$40
;     CPX #$01
;     BEQ DoorUnlocked
;     CPX #$07
;     BEQ DoorUnlocked

;     Slot2UnderLeveled:
;     LDX ch_level+$80
;     CPX #$09
;     BCC Slot3UnderLeveled
;     LDX ch_class+$80
;     CPX #$01
;     BEQ DoorUnlocked
;     CPX #$07
;     BEQ DoorUnlocked

;     Slot3UnderLeveled:
;     LDX ch_level+$C0
;     CPX #$09
;     BCC Slot4UnderLeveled
;     LDX ch_class+$C0
;     CPX #$01
;     BEQ DoorUnlocked
;     CPX #$07
;     BEQ DoorUnlocked

;     Slot4UnderLeveled:
;     LDY #$01
;     TAX
;     LDA #BANK_MENUS
;     JMP SwapPRG

;   DoorUnlocked:
;     LDY #$00
;     TAX
;     LDA #BANK_MENUS
;     JMP SwapPRG

; ;105 bytes
; ;8A 4A 29 03 C9 02 D0 59 A2 00 86 45 AE 25 60 D0 50 AE 26 61 E0 09 90 0B AE 00 61 E0 01 F0 42 E0 07 F0 3E AE 66 61 E0 09 90 0B AE 40 61 E0 01 F0 30 E0 07 F0 2C AE A6 61 E0 09 90 0B AE 80 61 E0 01 F0 1E E0 07 F0 1A AE E6 61 E0 09 90 0B AE C0 61 E0 01 F0 0C E0 07 F0 08 A0 01 AA A9 0E 4C 03 FE A0 00 AA A9 0E 4C 03 FE
