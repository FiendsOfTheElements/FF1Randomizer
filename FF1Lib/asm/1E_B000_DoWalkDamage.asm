; Allow Overworld tiles to deal damage
; Note: The ship dock bit is used, so any damage tile a boat
;  can reach will allow docking too.
; Last Update: 2021-08-30

vehicle           = $42   ; 1=walking, 2=canoe, 4=ship, 8=airship
tileprop          = $44   ; 2 bytes
MapTileDamage     = $C7DE
MapPoisonDamage   = $C7FB
SwapPRG_L         = $FE03
OWTP_SPEC_MASK    = $E0
OWTP_SPEC_DAMAGE  = $E0

 .ORG $C33C                 ; Bank 1F

OverworldMovement: 
  LDA #$1E
  JSR SwapPRG_L
  JMP DoWalkDamage

 .ORG $B000                 ; Bank 1E

DoWalkDamage:
  LDA vehicle               ; check the current vehicle
  CMP #$01                  ; are they on foot?
  BNE NoDamage              ; if not, just exit
    JSR MapPoisonDamage     ; if they are... distribute poison damage
    LDA tileprop            ; get the properties for this tile
    AND #OWTP_SPEC_MASK     ; mask out the special bits
    CMP #OWTP_SPEC_DAMAGE   ; see if it's a damage tile (frost/lava)
    BNE NoDamage            ; if it is...
      JMP MapTileDamage     ;  ... do map tile damage
NoDamage:
  RTS