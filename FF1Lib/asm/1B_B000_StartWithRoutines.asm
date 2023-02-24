;
; Routines to modify stats at the start of the game, after partygen
;  and at the start of the battle.
;
; Last Update: 2022-09-05
;

tmp = $10

btl_ib_charstat_ptr = $80
btl_ob_charstat_ptr = $82

class_lut_a = $ED
class_lut_b = $EE

unsram          = $6000  ; $400 bytes
gold            = unsram + $1C 
items           = unsram + $20

ch_stats        = unsram + $0100  ; MUST be on page bound.  Each character allowed $40 bytes, so use 00,40,80,C0 to index ch_stats

ch_class        = ch_stats + $00
ch_ailments     = ch_stats + $01
ch_name         = ch_stats + $02  ; 4 bytes

ch_exp          = ch_stats + $07  ; 3 bytes
ch_curhp        = ch_stats + $0A  ; 2 bytes
ch_maxhp        = ch_stats + $0C  ; 2 bytes

ch_str          = ch_stats + $10
ch_agil         = ch_stats + $11
ch_int          = ch_stats + $12
ch_vit          = ch_stats + $13
ch_luck         = ch_stats + $14

ch_exptonext    = ch_stats + $16  ; 2 bytes -- only for user display, not actually used.
ch_weapons      = ch_stats + $18  ; 4
ch_armor        = ch_weapons + 4  ; 4

ch_substats     = ch_stats + $20
ch_dmg          = ch_substats + $00
ch_hitrate      = ch_substats + $01
ch_absorb       = ch_substats + $02
ch_evade        = ch_substats + $03
ch_resist       = ch_substats + $04
ch_magdef       = ch_substats + $05

ch_level        = ch_stats + $26        ; OB this is 0 based, IB this is 1 based

ch_magicdata    = unsram + $0300  ; must be on page bound
ch_spells       = ch_magicdata
ch_mp           = ch_magicdata + $20
ch_curmp        = ch_mp + $00
ch_maxmp        = ch_mp + $08

NewGamePartyGeneration = $8000
;NewGame_LoadStartingStats = $C76D
NewGame_LoadStartingLevels = $DD9A
AddGPToParty = $DDEA
BattleRNG = $FCE7
SwapPRG_skip = $FE07

BANK_PARTYGEN = $1E
BANK_CLASSMOD = $1B

 .ORG $B000

; Common Routines to identify current class
CheckIfClass:
  LDA ch_class, Y
CheckIfClassQuick:  
  CMP #$0C
  BCC NotANone
    LDA #$0C
NotANone: 
  STY tmp
  TAY
  LDA (class_lut_a), Y
  CMP #$01
  PHP
  LDY tmp
  PLP
  RTS
 
GetValueByClass:
  LDA ch_class, Y
  CMP #$0C
  BCC NotANone2
    LDA #$0C
NotANone2:
  STY tmp
  TAY
  LDA (class_lut_a), Y
  PHP
  LDY tmp
  PLP
  RTS

IsEquipped:
	@Loop:
		LDA (btl_ob_charstat_ptr), Y
		BPL @NoEquip
			RTS
	@NoEquip:
		INY
		CPY #$20
		BNE @Loop
			RTS

 .ORG $B080
 
StartOfBattle:
  LDA #$08
  CMP $F2
  BEQ IsBattle
    JMP ApplyStartOfGame
IsBattle:
  JSR CatClaws
  JSR ThorHammer
  JSR Hunter
  JSR SteelArmor
  JSR WoodArmors
  JMP UnarmedAttack

; Give Black Belt Unarmed Attack
UnarmedAttack: 
  LDA #<lut_Blackbelts                 ; Put the lut to refer to in memory
  STA class_lut_a                 
  LDA #>lut_Blackbelts
  STA class_lut_b
  LDY #$00                             ; Get class
  LDA (btl_ob_charstat_ptr), Y
  JSR CheckIfClassQuick                ; Compare with lut position
  RTS

; Raise Crit% with CatClaws
CatClaws:
  LDA #<lut_CatClaws 
  STA class_lut_a                 
  LDA #>lut_CatClaws
  STA class_lut_b
  
  LDY #$00
  LDA (btl_ob_charstat_ptr), Y
  JSR CheckIfClassQuick
  BNE CC_NotClass
    LDY #$18 ; weapons
	JSR IsEquipped
	BPL CC_NotClass
CC_IsEquipWeapon:
      AND #$7F
      CMP #$23 ; catclaw weapon Id
      BNE CC_NotClass
        LDY #$0F ; crit rate
        LDA #$FF ; hardcoded value
        STA (btl_ib_charstat_ptr), Y
CC_NotClass:
  RTS

; Double damage with Thor Hammers
ThorHammer:
  LDA #<lut_ThorHammer 
  STA class_lut_a                 
  LDA #>lut_ThorHammer
  STA class_lut_b
  
  LDY #$00
  LDA (btl_ob_charstat_ptr), Y
  JSR CheckIfClassQuick
  BNE Th_NotClass
    LDY #$18 ; weapons
	JSR IsEquipped
	BPL Th_NotClass
Th_IsEquipWeapon:
      AND #$7F
      CMP #$24 ; Thor Hammer weapon Id
      BNE Th_NotClass
        LDY #$09 ; damage
        LDA (btl_ib_charstat_ptr), Y
        CLC
        ASL
        STA (btl_ib_charstat_ptr), Y
Th_NotClass:
  RTS
  
; Steal Armor cast Fast
SteelArmor:
  LDA #<lut_SteelArmor
  STA class_lut_a                 
  LDA #>lut_SteelArmor
  STA class_lut_b
  
  LDY #$00
  LDA (btl_ob_charstat_ptr), Y
  JSR CheckIfClassQuick
  BNE St_NotSteel
    LDY #$1C ; armors
	JSR IsEquipped
	BPL St_NotSteel
		AND #$7F
		CMP #$05 ; Steel Armor Id
		BNE St_NotSteel
		  TYA
		  PHA
		  LDY #$0B ; Hit Multiplier
		  LDA #$02
		  STA (btl_ib_charstat_ptr), Y
		  PLA
		  TAY
St_NotSteel:
  RTS
  
; Wood Armor set give max evade
WoodArmors:
  LDA #<lut_WoodArmors
  STA class_lut_a                 
  LDA #>lut_WoodArmors
  STA class_lut_b
  
  TXA
  PHA

  LDX #$00
  LDY #$00
  LDA (btl_ob_charstat_ptr), Y
  JSR CheckIfClassQuick
  BNE Wo_NotWood
    LDY #$1C ; armors
	JSR IsEquipped
	BPL Wo_NotWood
    JSR Wo_IsEquipArmor
    
    CPX #$03
    BNE Wo_NotWood
      CLC
      LDY #$07 ; Evade
      LDA (btl_ib_charstat_ptr), Y
      ADC #$78
      BCC Wo_StoreValue
        LDA #$FF
Wo_StoreValue:     
      STA (btl_ib_charstat_ptr), Y
      PLA
      TAX
      RTS
Wo_IsEquipArmor:
    AND #$7F
    CMP #$02 ; Wood0 Armor Id
    BNE :+
      INX
      RTS
:
    CMP #$1B ; Wood Helmet Id
    BNE :+
      INX
      RTS
:
    CMP #$11 ; Wood Shield Id
    BNE :+
      INX
:
      RTS
Wo_NotWood:
  PLA
  TAX
  RTS  
  
; Hunter hurt all enemy type
Hunter:  
  LDY #$00
  LDA (btl_ob_charstat_ptr), Y
  ;character class in A
  TAX
  LDA lut_Hunter, X 
  BEQ Hu_NotClass
    LDY #$0D ; attacker weakness type
    ORA (btl_ib_charstat_ptr), Y
    STA (btl_ib_charstat_ptr), Y
Hu_NotClass:
  RTS
  NOP
  NOP
  NOP
  NOP
  NOP
  NOP
  NOP
;NOPs added because I couldnt rebuild the entire start with asm file.
;Had to swap the bugged hunter function, and maintain instruction length to perserve addressing.
;A0 00 B1 82 AA BD 27 B2 F0 06 A0 0D 11 80 91 80 60 EA EA EA EA EA EA EA     
  
; Always start battle asleep
Sleepy:
  LDA #<lut_Sleepy 
  STA class_lut_a                 
  LDA #>lut_Sleepy
  STA class_lut_b
 
  LDY #$00
  LDA (btl_ob_charstat_ptr), Y
  JSR CheckIfClassQuick
  BNE Slp_NotClass
    LDY #$01 ; ailment
    LDA (btl_ob_charstat_ptr), Y
    ORA #$20 ; sleep
    STA (btl_ob_charstat_ptr), Y
Slp_NotClass:  
  RTS  

; 25% chance to start battle poisoned
Sick:
  LDA #<lut_Sick 
  STA class_lut_a                 
  LDA #>lut_Sick
  STA class_lut_b
 
  LDY #$00
  LDA (btl_ob_charstat_ptr), Y
  JSR CheckIfClassQuick
  BNE Sik_NotClass
   JSR BattleRNG
   AND #$03 ; 25% chance of being sick
   BNE Sik_NotClass
     LDY #$01 ; ailment
     LDA (btl_ob_charstat_ptr), Y
     ORA #$04 ; poison
     STA (btl_ob_charstat_ptr), Y
Sik_NotClass:  
  RTS  
 
 .ORG $B200

lut_Blackbelts:
 .BYTE $FF, $00, $01, $00, $00, $00 ; $FF added for reference, replace by $00 if copying lut
 .BYTE $00, $00, $01, $00, $00, $00 
 .BYTE $00  

lut_CatClaws:
 .BYTE $00, $00, $00, $00, $00, $00
 .BYTE $00, $00, $00, $00, $00, $00 
 .BYTE $00   
 
lut_ThorHammer:
 .BYTE $00, $00, $00, $00, $00, $00
 .BYTE $00, $00, $00, $00, $00, $00 
 .BYTE $00 

lut_Hunter:
 .BYTE $00, $00, $00, $00, $00, $00
 .BYTE $00, $00, $00, $00, $00, $00 
 .BYTE $00  

lut_Sleepy:
 .BYTE $00, $00, $00, $00, $00, $00
 .BYTE $00, $00, $00, $00, $00, $00 
 .BYTE $00   
  
lut_Sick:
 .BYTE $00, $00, $00, $00, $00, $00
 .BYTE $00, $00, $00, $00, $00, $00 
 .BYTE $00   

lut_SteelArmor:
 .BYTE $00, $00, $00, $00, $00, $00
 .BYTE $00, $00, $00, $00, $00, $00 
 .BYTE $00  

lut_WoodArmors:
 .BYTE $00, $00, $00, $00, $00, $00
 .BYTE $00, $00, $00, $00, $00, $00 
 .BYTE $FF  

 .ORG $B300

ApplyStartOfGame:
  LDA #>ResumeHere 
  PHA              
  LDA #<ResumeHere-1
  PHA
  LDA #BANK_CLASSMOD
  PHA

  LDA #>SwapPRG_skip
  PHA               
  LDA #<SwapPRG_skip-1
  PHA

  LDA #>NewGame_LoadStartingLevels
  PHA                          
  LDA #<NewGame_LoadStartingLevels-1
  PHA
  
  LDA #>NewGamePartyGeneration-1 
  PHA                           
  LDA #<NewGamePartyGeneration-1
  PHA
  LDA #BANK_PARTYGEN            
  PHA
  
  JMP SwapPRG_skip
  
ResumeHere:
  LDY #$00
LoopAll4Warriors:  
  JSR IndividualStartWithLoop
  TYA
  CLC
  ADC #$40
  TAY
  BNE LoopAll4Warriors
  RTS

IndividualStartWithLoop:  
  JSR IncreaseStartingGP
  JSR DecreaseStartingGP
  JSR DoInnateSpells
  JSR DoMpStart
  JSR DoStartWithKI
  RTS

; Increase Starting GP
IncreaseStartingGP:
  LDA #<lut_IncreaseGP  
  STA class_lut_a                 
  LDA #>lut_IncreaseGP
  STA class_lut_b
  LDX #$00
IGP_ClassLoop: 
  JSR GetValueByClass
  BEQ IGP_NotClass
    TAX
    LDA #$00
    STA tmp
    STA tmp+1
    STA tmp+2
IGP_GoldLoop:
  CPX #$00
  BEQ IGP_NoClassLeft
    CLC  
    LDA #$64
    ADC tmp
    STA tmp
    LDA #$00
    ADC tmp+1
    STA tmp+1
    LDA #$00
    ADC tmp+2
    STA tmp+2
    DEX
    CLC
    BCC IGP_GoldLoop
IGP_NoClassLeft:  
  JSR AddGPToParty
IGP_NotClass:  
  RTS

; Decrease Starting GP
DecreaseStartingGP:
  LDA #<lut_DecreaseGP  
  STA class_lut_a                 
  LDA #>lut_DecreaseGP
  STA class_lut_b
  LDX #$00
DGP_ClassLoop: 
  JSR CheckIfClass
  BNE DGP_Done
    INX
    LDA #$00
    STA tmp
    STA tmp+1
    STA tmp+2
    CLC  

    LDA #$32         ; These 3 LDA values are replaced in the c# code
    ADC tmp          ; to dynamically adjust depending on starting gold
    STA tmp
    LDA #$00         ; ^
    ADC tmp+1
    STA tmp+1
    LDA #$00         ; ^
    ADC tmp+2
    STA tmp+2

    LDA gold
    SEC
    SBC tmp         ; subtract low byte
    STA gold

    LDA gold+1
    SBC tmp+1       ; mid byte
    STA gold+1

    LDA gold+2
    SBC tmp+2                    ; and get borrow from high byte
    STA gold+2
    BCS DGP_Done
      LDA #$00
      STA gold
      STA gold+1
      STA gold+2
DGP_Done:
  RTS

; Innate Spells
DoInnateSpells:
  LDX #$03

  LDA #<ch_spells+$1C  ;lv8 spells
  STA tmp+1                 
  LDA #>ch_spells
  STA tmp+2

  LDA #<lut_InnateSpell01 
  STA class_lut_a                 
  LDA #>lut_InnateSpell01
  STA class_lut_b
   
InnateSpellLoop:
  JSR GetValueByClass
  BEQ InnateNoSpells
  STA (tmp+1),Y
  INC tmp+1

  LDA #$0D
  CLC
  ADC class_lut_a
  STA class_lut_a
  BCC InnateNoCarry        ; 4 bytes
    INC class_lut_b
InnateNoCarry:
  DEX
  BNE InnateSpellLoop  
InnateNoSpells:
  CPX #$03
  BEQ InnateDone
    TYA
    CLC
    ADC #$07
    TAX
    LDA ch_mp,X
    CLC
    ADC #$02
    STA ch_mp,X
    STA ch_mp+8,X
InnateDone:
  RTS 

; Start with 1 MP in all level
DoMpStart:
  LDX #$00
  LDA #<lut_MpStart 
  STA class_lut_a                 
  LDA #>lut_MpStart
  STA class_lut_b
StartSpellLoop:
  JSR CheckIfClass  
  BEQ MpStart
    RTS
MpStart:
  TYA
  PHA
  TAX
  LDY #$07
MpLoop:  
  INX
  LDA ch_mp,X
  CLC
  ADC #$01
  STA ch_mp,X
  STA ch_mp+8,X
  DEY  
  BNE MpLoop
  PLA
  TAY
  RTS  

; Start with a Key Item
DoStartWithKI:
  LDX #$00
  LDA #<lut_StartingKeyItems 
  STA class_lut_a                 
  LDA #>lut_StartingKeyItems
  STA class_lut_b
KILoop:
  JSR GetValueByClass  
  BEQ NoKI
    TAX
    LDA #$01
    STA unsram, X                   ; need to offset KI values +$20 so we can also add the canoe
NoKI:
  RTS
 
 .ORG $B480
; luts
lut_IncreaseGP:
 .BYTE $FF, $00, $00, $00, $00, $00 ; $FF added for reference, replace by $00 if copying lut
 .BYTE $00, $00, $00, $00, $00, $00 
 .BYTE $00  

lut_DecreaseGP:
 .BYTE $00, $00, $00, $00, $00, $00
 .BYTE $00, $00, $00, $00, $00, $00 
 .BYTE $00  

lut_InnateSpell01: ;
 .BYTE $00, $00, $00, $00, $00, $00
 .BYTE $00, $00, $00, $00, $00, $00 
 .BYTE $00  

lut_InnateSpell02: ;
 .BYTE $00, $00, $00, $00, $00, $00
 .BYTE $00, $00, $00, $00, $00, $00 
 .BYTE $00 
 
lut_InnateSpell03: ;
 .BYTE $00, $00, $00, $00, $00, $00
 .BYTE $00, $00, $00, $00, $00, $00 
 .BYTE $00     

lut_MpStart: ; 
 .BYTE $00, $00, $00, $00, $00, $00
 .BYTE $00, $00, $00, $00, $00, $00 
 .BYTE $00  
 
lut_StartingKeyItems: ; 
 .BYTE $00, $00, $00, $00, $00, $00
 .BYTE $00, $00, $00, $00, $00, $00 
 .BYTE $00
 
 
 .ORG $B600

LvlUp_NoDisplay = $87AA

RecruitModeJump:
  LDA tmp
  STA tmp+3
  TAY
  LDA lut_CharOffset, Y
  TAY
  JSR IndividualStartWithLoop
  LDA tmp+3
  STA tmp
  JMP LvlUp_NoDisplay
  
lut_CharOffset:
  .BYTE $00, $40, $80, $C0
