Black Belt Unarmed LUT Changes

Blobs can be found in BugFixes.cs



The following ASM changes were made to make Black Belts use the Unarmed LUTs instead of hardcoded values. All cases were tested in both vanilla and Transmooglifier both. This also means there can be an Unarmed Blursing by simply setting the UnarmedAttack flag to true.


Bank 1B - LvlUp_AdjustBBSubStats:

`
LDY #$00
LDA ($86),Y
CMP #$02
BEQ $8757
CMP #$08
BEQ $8757
`

was moved by 5 bytes and changed to:


`
LDA #<lut_Blackbelts           
STA class_lut_a                 
LDA #>lut_Blackbelts
STA class_lut_b
LDY #$00                             
LDA (btl_ob_charstat_ptr), Y
JSR CheckIfClassQuick       
BEQ $8757
`

This code executes on Level Up, and subsequently sets Black Belts and Masters Absorb (Lv) and Damage (Lv*2) stats (depending on equipment).






Bank 1F - UnadjustBBEquipStats and ReadjustBBEquipStats:

`
LDX $17
LDA $6100,X
CMP #$02
BEQ $EECA
CMP #$08
BNE $EECF
`

was changed to:


`
LDA #BANK_CLASSMOD ($1B)
JSR SwapPRG_L
JSR $B040
TYA
BEQ

@$B040:
LDA #<lut_Blackbelts          
STA class_lut_a
LDA #>lut_Blackbelts
STA class_lut_b
LDA ch_class, X
JSR CheckIfClassQuick
TAY

LDA #BANK_EQUIPSTATS ($0C)
JMP SwapPRG_L
`

This code executes on Entering and Exiting the Equip Menus, and subsequently sets Black Belts and Masters Absorb (Lv) and Damage (Lv*2) stats (depending on equipment and menu you are interacting with).