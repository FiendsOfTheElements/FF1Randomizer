;; 2020-11-16
;; Show characters in a victory pose in shop if the can equip/learn something
;; Pressing Select in shops will show a description of current select item
;; Pressing Select in equip menu will show a description of current item

tmp             = $10
joy_select      = $22
joy_a           = $24
joy_b           = $25
cursor          = $62
shopcurs_x      = $64 ; $28 = lower menu, $A8 right menu
shop_type       = $66
equipoffset     = shop_type  ; MUST be shared with shop_type
submenu_targ    = $66  ; shared with shop_type

item_box        = $0300

unsram          = $6000  ; $400 bytes
sram            = $6400  ; $400 bytes
ch_stats        = unsram + $0100
ch_ailments     = ch_stats + $01
ch_class        = ch_stats + $00
ch_weapons      = ch_stats + $18  ; 4

btl_chardrawinfo = $6AD3        ;$10 bytes, 4 bytes for each character
btl_chardraw_pose   = btl_chardrawinfo+3

ShopFrame            = $A727
DrawSimple2x3Sprite  = $EC24
EquipMenuFrame       = $BCF9
EnterEquipMenu       = $BACA
DrawEquipMenu        = $BDF3
DrawEquipMenuStrings = $ECDA
TurnMenuScreenOn_ClearOAM = $B780
TurnMenuScreenOn     = $B783
ClearOAM             = $C43C
DrawMagicMenuMainBox = $BA6D
MenuFrame            = $B65D
MG_START             = $B0
ch_magicdata         = unsram + $0300  ; must be on page bound
ch_spells            = ch_magicdata
DrawCharMenuString   = $B959
DrawMainItemBox      = $B8EF
ClearNT              = $9C02
descboxopen          = $7F
MenuWaitForBtn_SFX   = $B613
MenuWaitForBtn   = $B625

LEFT_MENU = $28
RIGHT_MENU = $A8

lut_BIT = $AC38   ; Bank 0E
lut_MagicPermisPtr = $AD00 ;Bank 0E
lut_ClassEquipBit = $BCB9 ;Bank 0E
lut_WeaponPermissions = $BF50 ;Bank 0E
lut_ArmorPermissions = $BFA0 ;Bank 0E
lutClassBatSprPalette = $ECA4

.ORG $BB8F
;;; Patch loop in EquipMenu_EQUIP, replacing
;;;
;;;  0E:BB8F   LDA joy_b
;;;  0E:BB91   BNE @B_Pressed
;;;
;;; With the following jump to the extended equip menu
JMP UpgradedEquipMenu
NOP
OriginalEquipMenuLoop: 		; Gives us a target to jump back to

;; the only purpose these serve is to generate 00EA00 in the
;; machine code hex dump so it is possible separate the code patches
;; by hand when we copy them into Hacks.cs
;; however they _should not_ be included in the actual patch
	BRK
	NOP
	BRK

.ORG $AECD
;;; Patch MagicMenu_Loop, replacing
;;;
;;;  0E:AECD   LDA joy_b
;;;  0E:AECF   BNE @B_Pressed
;;;
;;; With the following jump to the extended magic menu
JMP UpgradedMagicMenu
NOP
OriginalMagicMenuLoop: ; Gives us a target to jump back to

;; the only purpose these serve is to generate 00EA00 in the
;; machine code hex dump so it is possible separate the code patches
;; by hand when we copy them into Hacks.cs
;; however they _should not_ be included in the actual patch
	BRK
	NOP
	BRK

.ORG $9500

BeforeShopFrame:
  LDA shopcurs_x            ; Check if cursor is in the right shop menu
  CMP #LEFT_MENU            ;  can't get the info screen when selecting Buy/Sell or Character
  BEQ InLeftMenu
    LDA shop_type           ; Check shop type, only show info for Weapon/Armor/Spell shops
    CMP #$04                ; 0-1 Weapon/Armor shop, 2-3 spell shops
    BCS GoToFrame
    CMP #$02
    BCS SpellShop
      LDX #$00              ; Check for each character if they can equip current select item
      JSR IsEquipLegal
      LDX #$40
      JSR IsEquipLegal
      LDX #$80
      JSR IsEquipLegal
      LDX #$C0
      JSR IsEquipLegal
      JMP GoToFrame
SpellShop:
      LDX #$00              ; Check for each character if they can learn current select spell
      JSR CanLearnSpell
      LDX #$40
      JSR CanLearnSpell
      LDX #$80
      JSR CanLearnSpell
      LDX #$C0
      JSR CanLearnSpell
      JMP GoToFrame
InLeftMenu:
    JSR ZeroOutVictory      ; If we're in left menu, have everyone lower their arms
GoToFrame:
  JSR ShopFrame             ; Do ShopFrame as normal
  JMP DrawInfo              ; Check if we need to draw an info box

ZeroOutVictory:             ; Write 00 for the base standing pose for each character
  LDA #$00
  STA btl_chardraw_pose
  STA btl_chardraw_pose+4
  STA btl_chardraw_pose+8
  STA btl_chardraw_pose+12
  RTS

DrawOBSprite_Exit:          ; Reproduce DrawOBSprite for shops only
    RTS

DrawOBSprite:
    TAX                    ; put character index in X
    LSR                    ;  divide char index by 2
    STA tmp                ;  and put it in tmp  (tmp is now $00,$20,$40, or $60 -- 2 rows of tiles per character)

    LDA ch_class, X               ; get the char's class
    TAY                           ; use it as an index
    LDA lutClassBatSprPalette, Y  ;  to find which palette that class's battle sprite uses
    STA tmp+1                     ;  put palette in tmp+1

    LDA ch_ailments, X     ; get out of battle ailment byte
    BEQ Standing           ;  if zero, no ailments... draw normal stance (standing upright)
    CMP #$01               ; if 1, character is dead
    BEQ DrawOBSprite_Exit  ;  so don't draw any sprite, just exit
    CMP #$03               ; if 3, character is poisoned
    BEQ Crouched           ;  draw in crouched position
    LDA #$03               ; otherwise (ailment byte = 2), character is stoned
    STA tmp+1              ;  change palette byte to 3 (stoned palette)
Crouched:                  ; to draw sprite as crouched... at #$14 to the
    LDA #$14               ;   tile number to draw.
    JMP GoToDraw
Standing:                  ; to really draw sprite as standing, A must be 0 here
    LDA tmp                ;  set to $0E for Victory pose
    LSR
    LSR
    LSR
    TAX
    LDA btl_chardraw_pose, X
GoToDraw:
    CLC                   ; add tile offset to tmp
    ADC tmp               ;  tmp is now the start of the tiles which make up this sprite
    STA tmp               ;  and tmp+1 is the palette to use

    JMP DrawSimple2x3Sprite

CanLearnSpell:             ; Check if a character can learn current select spell
    TXA
    STA tmp+5              ; store char ID
    LDA ch_class, X        ; use it to get his class
    ASL                    ; double it (2 bytes per pointer)
    TAX                    ; and put in X for indexing

    LDA lut_MagicPermisPtr, X     ; get the pointer to this class's
    STA tmp                       ;    magic permissions table
    LDA lut_MagicPermisPtr+1, X   ; put that pointer in (tmp)
    STA tmp+1

    LDX cursor           ; use the cursor position
    LDA item_box, X      ; to get the item ID of the spell we're to learn
    SEC
    SBC #$B0             ; subtract $B0 to convert it to magic ID (magic starts at item $B0)
    STA tmp+2            ; store magic ID in tmp+2 for future use

    AND #$07             ; get low 3 bits.  This will indicate the bit to use for permissions
    STA tmp+3            ; store it in tmp+3 for future use

    LDA tmp+2           ; get the magic ID
    LSR                 ; divide by 8 (gets the level of the spell)
    LSR
    LSR
    TAY                 ; put spell level in Y
    LDA (tmp), Y        ; use it as index to get the desired permissions byte
    STA tmp+4           ; store permissions byte in tmp+4 for future use

    LDX tmp+3           ; get required bit position
    LDA lut_BIT, X      ; use as index in the BIT lut to get the desired bit
    AND tmp+4           ; AND with permissions byte
    BEQ HasPermission   ; if result is zero, they have permission to learn
      LDA #$00          ; if not, write $00 for standing pose
      JMP StorePose
HasPermission:          ; write $0E for victory pose
    LDA #$0E
StorePose:
    STA tmp             ; get back char ID
    LDA tmp+5           ; divide by $10
    LSR
    LSR
    LSR
    LSR
    TAX
    LDA tmp             ; write new pose for that character
    STA btl_chardraw_pose, X
    RTS

IsEquipLegal:                ; Check if a character can equip current select piece of equipment
    TXA
    STA tmp+5                ; store char ID
    LDA ch_class, X          ; use it to get his class
    ASL                      ; double it (2 bytes per pointer)
    TAX                      ; and put in X for indexing

    LDA lut_ClassEquipBit, X        ; get the class permissions bit position word
    STA tmp+2                       ;  and put in tmp+4,5
    LDA lut_ClassEquipBit+1, X
    STA tmp+3

    LDX cursor         ; use the cursor position
    LDA item_box, X    ; to get the item ID of the equipment
    SEC                ; $1c for nunchuk + $28 = $44 for cloth

    CMP #$44           ; check if it's an armor or a weapon
    BCS IsArmor
      SEC              ; substract weapon ID offset to get
      SBC #$1C         ; the index in the permissions lut
      ASL
      TAX
      LDA lut_WeaponPermissions, X   ; use it to get the weapon permissions word (low byte)
      AND tmp+2                      ; mask with low byte of class permissions
      STA tmp                        ;  temporarily store result
      LDA lut_WeaponPermissions+1, X ; then do the same with the high byte of the permissions word
      AND tmp+3                      ;  mask with high byte of class permissions
      ORA tmp                        ; then combine with results of low mask
      JMP CompareResult
IsArmor:
    SBC #$44                      ; substract armor ID offset to get
    ASL                           ;  the index in the permissions lut
    TAX
    LDA lut_ArmorPermissions, X   ; use it to get the armor permissions word
    AND tmp+2                     ;  and mask it with the class permissions word
    STA tmp
    LDA lut_ArmorPermissions+1, X
    AND tmp+3
    ORA tmp               ; and OR both high and low bytes of result together.  A nonzero result here indicates
                          ;  armor cannot be equipped
CompareResult:
    CMP #$01              ; compare with 1 (any nonzero value will set C)
    BCC CanEquip
      LDA #$00            ; if not, write $00 for standing pose
      JMP StorePose
CanEquip:
    LDA #$0E              ; write $0E for victory pose
    JMP StorePose


DrawComplexString = $DE36
DrawBox = $E063
EraseBox = $E146

joy             = $20
menustall       = $37
box_x           = $38
box_y           = $39
dest_x          = $3A
dest_y          = $3B
box_wd          = $3C ; shared
box_ht          = $3D ; shared


text_ptr        = $3E ; 2 bytes
cur_bank        = $57
ret_bank        = $58
joy_prevdir     = $61

lut_itemDescriptions = $9300

DrawInfo:
  LDA joy_select           ; Check if select button was pressed
  BEQ NoInput
    LDA shopcurs_x         ; If so, check if the cursor is in the right menu
    CMP #LEFT_MENU
    BEQ NoInput
    LDX cursor           ; get the cursor to
    LDA item_box, X      ; get the item ID of the item/spell
    STA tmp+4		 ; put the item ID in tmp+4
    JSR SharedDrawInfo
Loop:                       ; show the box until a button is pressed
    JSR ShopFrame      ; do a frame
    LDA joy                ; get joy
    CMP joy_prevdir        ; compare to previous buttons to see if button state has changed
    BEQ Loop               ; if no change.. do nothing, and continue loop
EraseWindow:
  LDA #$00                 ; if a button was pressed
  STA joy_select           ; reinit select
  JSR LoadDimensions       ; get the box dimensions
  JMP EraseBox             ; and erase it
LoadDimensions:
  LDA #$01
  STA box_x
  LDA #$12
  STA box_y
  LDA #$0E
  STA box_wd
  LDA #$0A
  STA box_ht
NoInput:
  RTS

;;; expect the item id in tmp+4
SharedDrawInfo:
      JSR LoadDimensions   ; Set the dimensions of the box
      LDA #$0E
      STA cur_bank         ; Set current bank
      JSR DrawBox          ;  and draw the box
      LDA text_ptr         ; Back up text_ptr as it's storing the cursor possible position
      STA tmp+2
      LDA text_ptr+1
      STA tmp+3

      LDA tmp+4            ; get the item ID from tmp+4
      SEC
      ASL                  ; double it (2 bytes per pointer)
      TAX                  ; and put back into X for indexing
      BCS HiTbl            ; if string ID was >= $80 use 2nd half of table, otherwise use first half
LoTbl:
      LDA lut_itemDescriptions, X     ; load up the pointer into text_ptr
      STA text_ptr
      LDA lut_itemDescriptions+1, X
      STA text_ptr+1
      JMP PtrLoaded                   ; then jump ahead
HiTbl:
      LDA lut_itemDescriptions+$100, X   ; same, but read from 2nd half of pointer table
      STA text_ptr
      LDA lut_itemDescriptions+$101, X
      STA text_ptr+1
PtrLoaded:
      LDA #$11
      STA cur_bank          ; set data bank (string to draw is on this bank -- or is in RAM)
      LDA #$0E
      STA ret_bank          ; set return bank (we want it to RTS to this bank when complete)
      JSR DrawComplexString ;  Draw Complex String, then exit!
      LDA tmp+2             ; restore backed up cursor positions
      STA text_ptr
      LDA tmp+3
      STA text_ptr+1
      LDA #$00
      STA joy_select        ; reinit select button
      RTS

;; the only purpose these serve is to generate 00EA00 in the
;; machine code hex dump so it is possible separate the code patches
;; by hand when we copy them into Hacks.cs
;; however they _should not_ be included in the actual patch
	BRK
	NOP
	BRK

.ORG $90E0

;; This adds item stats when the player presses select in EQUIP mode
;; of the weapon/armor menus
UpgradedEquipMenu:
	LDA joy_b
	BNE @B_Pressed
	LDA joy_select
	BNE @select_Pressed
	JMP OriginalEquipMenuLoop    ; return to original loop

;;; if B pressed, return out of the equip loop
@B_Pressed:
	RTS

@select_Pressed:
    LDX cursor               ; get the cursor in X
    LDA item_box, X          ; use it to index and get the selected item
    BEQ NothingThere         ; if this item is zero, nothing to do
    AND #$7F		     ; mask out the "equipped" flag
    LDY equipoffset	     ; distinguish between weapon/armor menu
    CPY #ch_weapons-ch_stats
    BNE ArmorMenu
    ADC #$1A                 ; add to convert to shop ID for weapon
    JMP	DrawInfoForGear
ArmorMenu:
    ADC #$42                 ; add to convert to shop ID for armor
DrawInfoForGear:
    STA tmp+4
    JSR ClearOAM             ; clear OAM to hide cursor
    JSR SharedDrawInfo

EquipInfoLoop:             ; show the box until a button is pressed
    JSR EquipMenuFrame     ; do a frame
    LDA joy                ; get joy
    CMP joy_prevdir        ; compare to previous buttons to see if button state has changed
    BEQ EquipInfoLoop      ; if no change.. do nothing, and continue loop

;;; User pressed a button, so now redraw the screen
    LDA #0
    STA $2001             ; turn off the PPU
    STA menustall         ; and turn off menu stalling (since the PPU is off)

    JSR DrawEquipMenu               ; draw the equip menu (but not the item text)
    JSR TurnMenuScreenOn            ; then clear OAM and turn the screen on  (even though item names have not been drawn)
    JSR DrawEquipMenuStrings        ; draw the menu strings (item names)

NothingThere:
    JMP OriginalEquipMenuLoop    ; return to original loop

;; This adds spell info when the player presses select in the magic menu
UpgradedMagicMenu:
	LDA joy_b
	BNE @B_Pressed
	LDA joy_select
	BNE @select_Pressed
	JMP OriginalMagicMenuLoop    ; return to original loop

;;; if B pressed, return out of the equip loop
@B_Pressed:
	RTS

@select_Pressed:
	LDA #$01
	STA menustall		; turn on menustall to avoid flicker

; Out of battle, spell data is stored stupidly so valid values are only 00-08, where 01 to 08 are actual spells
;   and 00 is 'empty'.  Each spell is conceptually in a "slot" that belongs to each spell level.  Therefore,
;   both CURE and LAMP are stored as '01' because they're both the first spell in their level, but because
;   they're in a different level slot, the game distinguishes them.

    LDA submenu_targ       ; get character ID
    LSR A		   ; shift right (shifts bit 0->C and zero->bit 7)
    ROR A		   ; rotate right (shifts C->bit 7 and bit 0->C)
    ROR A                  ; rotate right again, now we have the character index
    ORA cursor             ; ORA with cursor to get index to spell
    TAX                    ; and put in X for indexing

    ASL A                  ; double A and mask out the level bits
    AND #$38               ;  this effectively makes A the spell level * 8
    CLC                    ; clear carry (necessary because characters 3 and 4 will have bit 7 shifted into carry)

    ADC ch_spells, X       ; then add the selected spell on this level
    CLC                    ; and add the spell start constant - 1 (-1 because 0 is not a spell)
    ADC #MG_START-1        ;  A is now the ID number of the selected spell
    STA tmp+4
    JSR SharedDrawInfo
    JSR TurnMenuScreenOn_ClearOAM  ; clear OAM and turn the screen on

    JSR MenuWaitForBtn

;;; User pressed a button, so now redraw the screen
    LDA #0
    STA $2001             ; turn off the PPU
    STA menustall         ; and turn off menu stalling (since the PPU is off)
    STA descboxopen                ; and mark description box as closed

    JSR ClearNT                    ; clear the nametable
    LDA cursor
    PHA			           ; save cursor position because DrawMagicMenuMainBox resets it
    JSR DrawMagicMenuMainBox       ; draw the big box containing all the spells
    PLA
    STA cursor			   ; put the cursor position back

    LDA #$07
    JSR DrawMainItemBox            ; draw the title box
    LDA #$29
    JSR DrawCharMenuString         ; and draw the "MAGIC" title text
    JSR TurnMenuScreenOn_ClearOAM  ; clear OAM and turn the screen on

    JMP OriginalMagicMenuLoop    ; return to original loop

	;; the only purpose these serve is to generate 0000 in the
	;; machine code hex dump so it is easier to separate the different code patches
	;; we actually _don't_ want to include them in the patch
  	BRK
	NOP
	BRK
