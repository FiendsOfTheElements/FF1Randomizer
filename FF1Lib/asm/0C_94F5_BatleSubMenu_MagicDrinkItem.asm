;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;
;;
;;  BattleSubMenu_Magic  [$94F5 :: 0x31505]
;;
;;  Called when the player selects 'MAGIC'
;;
;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;

BattleSubMenu_Magic:
    JSR MenuSelection_Magic
    
    PHA                             ; backup A/B button press
    LDA #$01
    JSR UndrawNBattleBlocks_L       ; undraw the magic box
    PLA                             ; restore A/B state
    
    CMP #$02
    BNE :+                          ; if they pressed B to exit the menu box
      JMP CancelBattleAction        ;   then cancel this action
      
  : LDA btlcmd_curchar
    JSR ShiftLeft6 ;                ; put usable char index in 68B3,4  (00,40,80,C0)
    STA $68B3                       ;  68B3 will eventually be index to the player's magic list
    STA $68B4                       ;  68B4 will eventually be index to the player's MP
    
    LDA $6AF8                       ; this was the "magic page".  So this will be 1 if the user selected a L5-8 spell, and 0 if L1-4 spell
    AND #$01
    ASL A
    ASL A
    PHA                             ; push Page*4
    ASL A
    ASL A
    CLC
    ADC $68B3
    STA $68B3                       ; 68B3 = CharIndex + Page*4*4.  (4 spells per level * 4 levels per page)
    
    PLA                             ; pull Page*4
    CLC
    ADC $68B4
    STA $68B4                       ; 68B4 = CharIndex + Page*4.  (4 levels per page)
    
    LDA btlcurs_y                   ; get Y position of their selection
    ASL A
    ASL A                           ; *4 (4 spells per level)
    CLC
    ADC btlcurs_x                   ; + X position
    CLC
    ADC $68B3                       ; add to 68B3.  index is now complete:  Index from start of char's spell list, to their chosen spell.
    
    TAY                             ; put that index in Y, and use it to get the chosen spell
    LDA ch_spells, Y
    BNE :+                          ; if they selected an empty slot, do the @NothingBox -- otherwise skip over it
    
  @NothingBox:
      JSR DoNothingMessageBox       ; Show the nothing box
      JMP BattleSubMenu_Magic       ; and redo the magic selection submenu from the beginning
  : STA $6B7D                       ; store spell in 6B7D
    LDA btlcurs_y                   ; get Y selection (effectively the spell level)
    AND #$03
    CLC
    ADC $68B4                       ; add to 68B4.  This index is now complete:  Index from start of char's MP to the level of their chosen spell
    
    TAX
    LDA ch_curmp, X                 ; use that index to get their MP for this level
    BEQ @NothingBox                 ; if no more MP for this level, cut to "nothing" box and repeat.
    
    LDY btlcmd_curchar
    LDA #$01
    STA btl_charcmdconsumetype, Y   ; put 01 as consumable type (to indicate magic)
    TXA
    STA btl_charcmdconsumeid, Y     ; put the spell level as the consumable ID
    
    DEC $6B7D                       ; dec index of selected spell (make it 0 based instead of 1 based)
    LDA $6B7D
    JSR GetPointerToMagicData
    STA $80                         ; $80,81 points to this spell's data
    STX $81
    
    LDY #$05
    LDA ($80), Y
    STA $68B3                       ; put magic graphic at 68B3 (temp)
    
    LDY #$06
    LDA ($80), Y
    STA $68B4                       ; put magic palette at 68B4 (temp)
    
    LDA btlcmd_curchar
    ASL A
    TAY                             ; use 2*char as index for btlcmd_magicgfx
    
    LDA $68B3
    STA btlcmd_magicgfx, Y          ; record magic graphic
    LDA $68B4
    STA btlcmd_magicgfx+1, Y        ; and magic palette

    LDA #$40                      ; mark as spell cast
    JSR Player_Target_Spell       ; get spell's target
    CMP #$FF                      ; check if target selection was cancelled, i.e. b was pressed
    BEQ :+ 
      JMP SetCharacterBattleCommand    ; set the battle command
  : JMP BattleSubMenu_Magic 
    
;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;
;;
;;  Player_Target_Spell
;;
;;  Called when the player selects a spell
;;  or a gear piece to set the battle command
;;
;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;

Player_Target_Spell:
    PHA                             ; store A
    LDX $6B7D                       ; index of spell
    LDY #$03
    LDA ($80), Y                    ; get the target for this spell (stored as semi-flags:  01,02,04,08,10,20 are valid values)
    
  @CheckTarget_01:
    LSR A                           ; shift out low bit
    BCC @CheckTarget_02             ; if set (target=01 -- target all enemies)...
      LDY #$FF
      PLA
      RTS                           ; RTS to set battle command
  
  @CheckTarget_02:                  ; target 02 = target one enemy
    LSR A
    BCC @CheckTarget_04
      JSR SelectEnemyTarget         ; puts target in Y
      LDX $6B7D
      CMP #$02
      BNE :+                        ; if they pressed B to exit
        PLA                         ; to fix stack if B was pressed
        LDA #$FF                    ; set to check if b was pressed
        RTS                         ; redo submenu from the beginnning
    : PLA
      RTS                           ; command = AA xx TT  (AA = cast type, xx = spell, TT = enemy target)
    
  @CheckTarget_04:                  ; target 04 = target self
    LSR A
    BCC @CheckTarget_08     
      LDA btlcmd_curchar            ; use cur char
      ORA #$80                      ; OR with 80 to indicate targetting a player character
      TAY
      PLA
      RTS
    
  @CheckTarget_08:                  ; target 08 = target whole party
    LSR A
    BCC @Target_10
      PLA
      LDY #$FE                      ; 'FE' targets party
      RTS

  @Target_10:                       ; target 10 = target one player
    LDA btlcmd_curchar
    JSR SelectPlayerTarget          ; get a player target?
    LDX $6B7D
    CMP #$02                        ; did they press B to exit
    BNE :+
      PLA                           ; to fix stack if b was pressed
      LDA #$FF                      ; set to check if b was pressed
      RTS
  : LDA btlcurs_y
    AND #$03
    ORA #$80                        ; otherwise, put the player target in Y
    TAY
    PLA
    RTS

;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;
;;
;;  BattleSubMenu_Drink  [$95EC :: 0x315FC]
;;
;;  Called when the player selects 'DRINK'
;;
;;     Mostly unchanged
;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;

BattleSubMenu_Drink:
    LDA btl_potion_heal         ; see if there are any Heal/Pure potions remaining
    ORA btl_potion_pure
    BNE :+                      ; if there are no potions...
      JSR DoNothingMessageBox   ; show the 'Nothing' box
      JMP CancelBattleAction    ; then cancel
    
  : JSR DrawDrinkBox_L          ; otherwise (have at least 1 potion), draw the drink box
  
    JSR MenuSelection_Drink     ; get menu selection from the player  
    PHA                         ; backup the A/B button press
    LDA #$01
    JSR UndrawNBattleBlocks_L   ; undraw the drink menu
    PLA
    CMP #$02
    BNE :+                      ; was B pressed to get out of the drink menu?
      JMP CancelBattleAction    ;   if yes, cancel the action
      
  : LDA btlcurs_y               ; see if they selected Heal or Pure potion
    AND #$01
    STA $6B7D                   ; stick it in temp mem (0=Heal, 1=Pure)
    BNE @PureSelected           ; see which they selected?  If heal....
      LDA btl_potion_heal       ;   ... make sure they have at least 1 heal potion
      BNE @HealOK               ; if they don't
      JSR DoNothingMessageBox   ;   show the "nothing" box and cancel
      JMP CancelBattleAction
    
  @PureSelected:                ; if pure selected, make sure they have a Pure potion
    LDA btl_potion_pure
    BNE @GetTarget              ; if none, show "nothing" and cancel
      JSR DoNothingMessageBox
      JMP CancelBattleAction
      

  @GetTarget:
    LDA btlcmd_curchar          ; get input for SelectPlayerTarget
    JSR SelectPlayerTarget      ; and call it!
    CMP #$02
    BNE :+                      ; if they pressed B...
      JMP BattleSubMenu_Drink   ; ... return to the Drink sub menu
      
  : LDY btlcmd_curchar
    LDA #$02
    STA btl_charcmdconsumetype, Y   ; store 02 as the consumable type (to indicate DRINK)
    LDA $6B7D
    STA btl_charcmdconsumeid, Y     ; store menu selection as consumed ID -- to indicate which potion  (00/01 for Heal/Pure potion)
    
    LDX $6B7D               ; get menu selection
    DEC btl_potion_heal, X  ; remove the item from the qty
    
    LDA btlcurs_y           ; get target
    AND #$03
    ORA #$80                ; OR with 80 to indicate it's a player target
    TAY                     ; put in Y (for SetCharacterBattleCommand)
    
    LDA $6B7D               ; get the menu selection
    CLC
    ADC #$40                ; + $40.
    TAX                     ; X=40 for heal / 41 for pure
    LDA #$08
    JMP SetCharacterBattleCommand

;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;
;;
;;  BattleSubMenu_Item  [$9663 :: 0x31689]
;;
;;  Called when the player selects 'ITEM'
;;
;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;

BattleSubMenu_Item:
    LDA #$00            ; Check the 8 equipment slots and make sure there's at least 1 item
    LDX #$08
    LDY #ch_weapons - ch_stats
    : ORA (btl_ob_charstat_ptr), Y  ; OR all equipment together to see if any are nonzero
      INY
      DEX
      BNE :-
      
    AND #$FF                        ; update Z flag to see if all slots were zero
    BNE :+                          ; if all slots were 0 (no items), flow into @NothingBox, otherwise skip ahead
    
  @NothingBox:
      JSR DoNothingMessageBox       ; Show the "nothing" box
      JMP CancelBattleAction        ; And cancel this battle action
      
  : JSR DrawBattleItemBox_L         ; Draw the item box
    JSR MenuSelection_Item          ; and run the logic for selecting an item
    
    PHA                             ; backup A/B state
    LDA #$01
    JSR UndrawNBattleBlocks_L       ; undraw the item box
    PLA                             ; restore A/B state
    
    CMP #$02
    BNE :+                          ; if B pressed
      JMP CancelBattleAction        ; cancel action
      
  : LDA btlcurs_x               ; Selected column
    AND #$01
    STA $88                     ; (selected column stored in $88 for later)
    ASL A                       ;  * 4
    ASL A
    STA $68B3
    LDA btlcurs_y               ;  + Selected row
    AND #$03
    CLC
    ADC $68B3                   ;  = equip slot of selected item
    
    ADC #ch_weapons - ch_stats  ; + offset for character equipment = index for ob stats
    TAY
    LDA (btl_ob_charstat_ptr), Y; Get the selected equipment
    AND #$7F                    ; mask off the 'equipped' bit
    BEQ @NothingBox             ; if zero, print nothing box and exit
    
    STA $89                     ; if nonzero, stick it in $89
    DEC $89                     ; convert from 1-based to 0-based
    
    LDA $88                     ; get the selected column (0=weapon, 1=armor)
    BNE @GetArmorSpell          ; if armor selected jump ahead
    
    LDA $89                     ; otherwise weapon selected, get the index
    LDY btlcmd_curchar
    CLC
    ADC #TCITYPE_WEPSTART       ; convert from weapon index to item index
    STA btl_charcmditem, Y      ; record it in command buffer as item being used
    
    LDA $89                     ; get weapon index
    JSR GetPointerToWeaponData  ; get a pointer to that weapon's data (in XA)
    JMP @GetSpellCast

  @GetArmorSpell:
    LDA $89                     ; armor index
    LDY btlcmd_curchar
    CLC
    ADC #TCITYPE_ARMSTART       ; convert to item index
    STA btl_charcmditem, Y      ; record as item being used
    
    LDA $89
    JSR GetPointerToArmorData   ; pointer to armor data in XA
    
  @GetSpellCast:
    STA $88                     ; store pointer to wep/armor data in $88,89
    STX $89
    LDY #$03                    ; Y=3 to index, since byte 3 in both wep/armor data is the "spell cast" entry
    LDA ($88), Y                ; get spell cast

    STA $6B7D                   ; store spell index
    DEC $6B7D                   ; dec to make zero based
    
    LDA $6B7D
	TAY
    JSR GetPointerToMagicData
	JSR $C265                     ; Bank swap 

	Bank 0x0F, 0x8AD0
	{
      STA $80                     ; $80,81 points to this spell's data
      STX $81         

	  CPY #$FF
	  BNE: +
	    LDA #$D6
	    STA $80
	    LDA #$96
	    STA $81


	: LDA #$10                    ; mark as item
	  RTS
	}

	NOP
	NOP
    JSR Player_Target_Spell     ; get items's target
    CMP #$FF                    ; check if target selection was cancelled, i.e. b was pressed
    BEQ :+
      JMP SetCharacterBattleCommand    ; set the battle command
  : JMP BattleSubMenu_Item
