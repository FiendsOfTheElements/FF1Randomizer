; Code for repeated heal potion use
BANK_THIS = $1A

tmp             = $10
joy_a           = $24
joy_b           = $25
music_track     = $4B
cur_bank        = $57
ret_bank        = $58
menustall       = $37
box_x           = $38
box_y           = $39
box_wd          = $3C ; shared
box_ht          = $3D ; shared
dest_x          = $3A
dest_y          = $3B
text_ptr        = $3E ; 2 bytes
cursor          = $62
joy_prevdir     = $61
joy             = $20
sq2_sfx         = $7E
spr_x           = $40
spr_y           = $41
framecounter    = $F0
soft2000        = $FF
oam             = $0200

unsram          = $6000  ; $400 bytes
items           = unsram + $20
item_orb_start  = items + $12
item_qty_start  = item_orb_start + 4
item_heal       = item_qty_start + 3
ch_stats        = unsram + $0100
ch_curhp        = ch_stats + $0A  ; 2 bytes
ch_maxhp        = ch_stats + $0C  ; 2 bytes

SwapPRG                     = $FE03
Restore                     = $E04E ; the @Restore from DrawComplexString, jump here from $84FB
DrawBox                     = $E063
UseItem_Heal                = $B301 ;bank 0E
UseItem_Heal_LoopContinue   = $B30E
UseItem_Exit                = $B32F ;bank 0E
DrawComplexString           = $DE36
CallMusicPlay               = $C689 ;load this bank into cur_bank before calling
ClearOAM                    = $C43C
WaitForVBlank               = $FEA8
DrawPalette                 = $D850
UpdateJoy                   = $D7C2
DrawCursor                  = $EC95


;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;
; $B321 addr :: $3B331 rom (includes header)
; Patch to UseItem_Heal in bank 0E to call the new routines in bank 1A
;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;

.org $B321 ;_UseItem_Heal_Loop, $3B331
    _UseItem_Heal_LoopPatch:
        LDA #>(UseHealNewCode-1)
        PHA
        LDA #<(UseHealNewCode-1)
        PHA
        LDA #$1A ;bank with new heal potion routine
        JMP SwapPRG
;assembled bytes
;A9 85 48 A9 FF 48 A9 1A 4C 03 FE


;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;
; $84FB addr :: $6850B rom (includes header)
; This goes in bank 1A, it's the entire reason we can't put this in bank 1E
; a previous bug fix had a return to here but the code it calls swaps banks
; so instead of swapping to the bug fix, it thinks the return is in this bank.
; This is the only code outside of the original calls that uses a different bank.
; As long as this JMP statement is at $84FB in whatever bank we use, everything
; works out fine.  See 1F_DF2D_CustomIconAccess.asm for the fix referenced.
;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;

.org $84FB
        JMP Restore
;assembled bytes
;4C 4E E0


;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;
; $8600 addr :: $68610 rom (includes header)
; New parts of the UseItem_Heal loop
; Because of where the loop is being patched, there are a few exit points
; MenuRecoverHP_Abs and DrawItemTargetMenu are the two calls being replaced
;   in the original code, but DrawItemTargetMenu would have to be here anyways
;   to redraw the menu to reflect HP changes.
; The original ItemTargetMenuLoop when the loop is first entered, and then ours
;  is executed on subsequent loops (otherwise the cursor location would be reset)
; When we exit out to UseItem_Heal_LoopContinue, we're going back right after the
;   "BCS UseItem_Exit", so the original "can't use" message drawing remains there.
; This code has a call to resume music track $51, so that will be patched if the
;   "disable music" flag is enabled.
;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;

.org $8600 ;bank 1A, $68610
    UseHealNewCode:
        LDA #30
        JSR MenuRecoverHP_Abs ;heal 30hp to character indexed by X  --okay
        JSR DrawItemTargetMenu
        DEC item_heal           ; consume a heal potion
        BEQ MenuWaitForBtn_SFX  ; and leave if we're out

        ;Otherwise, jump back to bank 0E for the original "can't use" branches
        JSR ItemTargetMenuLoop
        BCS UseItem_ExitLocal ;carry set if B was pressed

        LDA #>(UseItem_Heal_LoopContinue-1)
        PHA
        LDA #<(UseItem_Heal_LoopContinue-1)
        PHA
        LDA #$0E
        JMP SwapPRG 

    UseItem_ExitLocal:
        ;re-enter the use heal potion loop
        LDA #>(UseItem_Exit-1)
        PHA
        LDA #<(UseItem_Exit-1)
        PHA
        LDA #$0E
        JMP SwapPRG 

MenuWaitForBtn_SFX:
    ; This code is a replacement for the MenuWaitForBtn_SFX call which exits after any button press
    ; We only get here if we're out of heal potions and therefore exits back to a jump back
    ;    to the main menu
        JSR MenuFrame           ; do a frame
        LDA joy_a               ;  check A and B buttons
        ORA joy_b
        BEQ MenuWaitForBtn_SFX  ;  if both are zero, keep looping.  Otherwise...
        LDA #0
        STA joy_a               ; clear both joy_a and joy_b
        STA joy_b


        ;return to original end code
        LDA #>(UseItem_Exit-1)
        PHA
        LDA #<(UseItem_Exit-1)
        PHA
        LDA #$0E
        JMP SwapPRG 
    
MenuRecoverHP_Abs:  ;copied from bank 0C
    STA tmp                     ; back up HP to recover by stuffing it in tmp
    CLC
    ADC ch_curhp,X             ; add recover HP to low byte of HP
    STA ch_curhp,X
    LDA ch_curhp+1,X           ; add 0 to high byte of HP (to catch carry from low byte)
    ADC #0
    STA ch_curhp+1,X
    CMP ch_maxhp+1,X           ; then compare against max HP to make sure we didn't go over
    BEQ _MenuRecoverHP_CheckLow ; if high byte of cur = high byte of max, we need to check the low byte
    BCS _MenuRecoverHP_OverMax  ; if high byte of cur > high byte of max... we went over
                                ; otherwise.. we're done...

  _MenuRecoverHP_Done:
    LDA #$57
    STA music_track             ; play music track $57 (the little gain HP jingle)

    LDA #%00110000              ; set vol for sq2 to zero
    STA $4004
    LDA #%01111111              ; disable sweep
    STA $4005
    LDA #0                      ; and clear freq
    STA $4006                   ;  best I can figure... shutting off sq2 here prevents some ugly sounds
    STA $4007                   ;  from happening when the gain HP jingle starts.  Though I don't see why
                                ;  that should happen...

    LDA tmp                     ; restore the HP recovery ammount in A, before exiting
  _MenuRecoverHP_Exit:
    RTS


  _MenuRecoverHP_CheckLow:
    LDA ch_curhp,X             ; check low byte of HP against low byte of max
    CMP ch_maxhp,X
    BCS _MenuRecoverHP_OverMax  ; if cur >= max, we're over the max
    BCC _MenuRecoverHP_Done     ;  otherwise we're not, so we're done (always branches)

  _MenuRecoverHP_OverMax:
    LDA ch_maxhp,X             ; if over max, just replace cur HP with maximum HP.
    STA ch_curhp,X
    LDA ch_maxhp+1,X
    STA ch_curhp+1,X
    JMP _MenuRecoverHP_Done     ; and then jump to done
ClearNT:
    LDA $2002     ; reset PPU toggle
    LDA #$20
    STA $2006
    LDA #$00
    STA $2006     ; set PPU addr to $2000 (start of NT)
    LDY #$00      ; zero out A and Y
    TYA           ;   Y will be the low byte of our counter
    LDX #$03      ; X=3 -- this is the high byte of our counter (loop $0300 times)

@Loop:            ; first loop clears the first $0300 bytes of the NT
      STA $2007
      INY
      BNE @Loop      ; once Y wraps
        DEX          ;  decrement X
        BNE @Loop    ;  and stop looping once X expires (total $0300 iterations)

@Loop2:           ; next loop clears the next $00C0 (up to the attribute table)
      STA $2007
      INY
      CPY #$C0       ; loop until Y reaches #$C0
      BCC @Loop2


    LDA #$FF      ; A=FF (this is what we will fill attribute table with
@Loop3:           ;  3rd and final loop fills the last $40 bytes (attribute table) with FF
      STA $2007
      INY
      BNE @Loop3

    RTS



DrawItemTargetMenu:
    LDA #0
    STA $2001            ; turn the PPU off
    STA menustall        ; and disable menu stalling
    JSR ClearNT          ; wipe the NT clean

    LDA #$0B             ; hardcoded box
    STA box_y            ; x,y   = $01,$0B
    LDA #$01             ; wd,ht = $1E,$08
    STA box_x
    LDA #$1E
    STA box_wd
    LDA #$08
    STA box_ht
    JSR DrawBox          ; ;bank 1F

    JSR @DrawBoxBody                ; draw the box body
    JMP TurnMenuScreenOn_ClearOAM   ; then clear OAM and turn the screen back on.  then exit

    RTS     ; useless RTS -- impossible to reach

 @DrawBoxBody:
    LDX #0*2         ; X is ID of string to draw (*2).  Start with string 0 (max hp)
    LDA #$04
    STA dest_x       ; draw this string at $04,$11
    LDA #$11
    STA dest_y
    JSR @DrawString  ; draw it

    DEC dest_y       ; dec Y coord ($04,$10)
    LDX #1*2         ; draw string 1 (cur hp)
    JSR @DrawString

    DEC dest_y       ; dec Y coord again ($04,$0F)
    LDX #2*2         ; draw string 2 (ailment blurb)
    JSR @DrawString

    DEC dest_y       ; dec Y coord by 2  ($04,$0D)
    DEC dest_y
    LDX #3*2         ; draw string 3, then exit

 @DrawString:
    LDA @lut_str_pointertable, X      ; load up the pointer from our pointer table
    STA text_ptr                      ; put it in text_ptr
    LDA @lut_str_pointertable+1, X
    STA text_ptr+1
    JMP DrawMenuComplexString        ; then draw it as a local complex string, and exit


 @lut_str_pointertable:
  .WORD @str_charmaxhp, @str_charcurhp, @str_ailmentblurb, @str_name

  ; These strings all use stat control codes ($10-$13).  See DrawComplexString
  ;  description for details

 @str_charmaxhp:                    ; "/999   /999   /999   /999"
  .BYTE $7A,$10,$06,$FF,$FF,$FF     ; "/999   "  <- character 0's max HP
  .BYTE $7A,$11,$06,$FF,$FF,$FF     ; same, but char 1's
  .BYTE $7A,$12,$06,$FF,$FF,$FF     ; char 2's
  .BYTE $7A,$13,$06,$00             ; char 3's, then null terminator

 @str_charcurhp:                    ; " 999    999    999    999"
  .BYTE $FF,$10,$05,$FF,$FF,$FF     ; " 999   "  <- character 0's cur HP
  .BYTE $FF,$11,$05,$FF,$FF,$FF     ; same, but char 1's
  .BYTE $FF,$12,$05,$FF,$FF,$FF     ; char 2's
  .BYTE $FF,$13,$05,$00             ; char 3's, then null terminator

 @str_ailmentblurb:
  .BYTE $10,$02,$FF,$FF,$FF,$FF     ; character 0's OB Ailment blurb  ("HP" when healthy)
  .BYTE $11,$02,$FF,$FF,$FF,$FF
  .BYTE $12,$02,$FF,$FF,$FF,$FF
  .BYTE $13,$02,$00

 @str_name:
  .BYTE $10,$00,$FF,$FF,$FF         ; character 0's name, followed by 3 spaces
  .BYTE $11,$00,$FF,$FF,$FF
  .BYTE $12,$00,$FF,$FF,$FF
  .BYTE $13,$00,$00


TurnMenuScreenOn_ClearOAM:
    JSR ClearOAM             ; clear OAM
                             ;  then just do the normal stuff
TurnMenuScreenOn:
    JSR WaitForVBlank      ; wait for VBlank (don't want to turn the screen on midway through the frame)
    LDA #>oam                ; do Sprite DMA
    STA $4014
    JSR DrawPalette          ; draw/apply the current palette

    LDA #$08
    STA soft2000             ; set $2000 and soft2000 appropriately
    STA $2000                ;  (no NT scroll, BG uses left pattern table, sprites use right, etc)

    LDA #$1E
    STA $2001                ; enable BG and sprite rendering
    LDA #0
    STA $2005
    STA $2005                ; reset scroll

    LDA #BANK_THIS           ; record current bank and CallMusicPlay
    STA cur_bank
    JMP CallMusicPlay

DrawMenuComplexString:
    LDA #BANK_THIS
    STA cur_bank          ; set data bank (string to draw is on this bank -- or is in RAM)
    STA ret_bank          ; set return bank (we want it to RTS to this bank when complete)
    JMP DrawComplexString ;  Draw Complex String, then exit!

MenuFrame:
    JSR WaitForVBlank    ; wait for VBlank
    LDA #>oam              ; Do sprite DMA (update the 'real' OAM)
    STA $4014

    LDA soft2000           ; reset scroll and PPU data
    STA $2000
    LDA #0
    STA $2005
    STA $2005

    LDA music_track        ; if no music track is playing...
    BPL :+
      LDA #$51             ;  start music track $51  (menu music)
      STA music_track

:   LDA #BANK_THIS         ; record this bank as the return bank
    STA cur_bank           ; then call the music play routine (keep music playing)
    JSR CallMusicPlay

    INC framecounter       ; increment the frame counter to count this frame

    LDA #0                 ; zero joy_a and joy_b so that an increment will bring to a
    STA joy_a              ;   nonzero state
    STA joy_b
    JMP UpdateJoy          ; update joypad info, then exit

ItemTargetMenuLoop:
    ;LDA #0
    ;STA cursor      ; this is what we DON'T do now
  @Loop:
    LDA #0
    STA joy_a       ; clear joy_a and joy_b so that a button press
    STA joy_b       ;  will be recognized

    JSR ClearOAM               ; clear OAM
    JSR DrawItemTargetCursor   ; draw the cursor for this menu
    JSR MenuFrame              ; do a frame

    LDA joy_a
    BNE @A_Pressed     ; check to see if they pressed A
    LDA joy_b
    BNE @B_Pressed     ; or B

    LDA joy            ; get joy data
    AND #$03           ;  isolate left/right buttons
    CMP joy_prevdir    ; compare it to the prev pressed buttons (to see if buttons are pressed or held)
    BEQ @Loop          ; if they match... no new button presses.  Keep looping.

    STA joy_prevdir    ; otherwise... record the change
    CMP #0             ; see if this was a release
    BEQ @Loop          ; if it was, no button press... keep looping

    CMP #$01           ; otherwise.. they just pressed left or right...
    BNE @Left          ;  see which they pressed

  @Right:
    LDA cursor         ; get cursor
    CLC                ;  and add 1 (move it to the right)
    ADC #$01
    JMP @MoveCurs      ; skip over the @Left block

  @Left:
    LDA cursor         ; get cursor
    SEC                ;  and subtract 1 (move it to the left)
    SBC #$01

  @MoveCurs:
    AND #$03               ; whether we moved left or right, AND with 3 to effectively wrap the cursor
    STA cursor             ;  and keep it in bounds.  Then write it back to the 'cursor' var
    JSR PlaySFX_MenuMove   ; Play the "move" sound effect
    JMP @Loop              ; and continue looping

  @A_Pressed:              ; if A was pressed
    JSR PlaySFX_MenuSel    ;  play the selection sound effect
    CLC                    ;  clear carry to indicate A pressed
    RTS                    ;  and exit

  @B_Pressed:              ; if B pressed
    JSR PlaySFX_MenuSel    ;  play selection sound effect
    SEC                    ;  and set carry before exiting
    RTS

DrawItemTargetCursor:
    LDX cursor           ; put the cursor in X
    LDA @lut,X          ; use it to index our LUT
    STA spr_x            ; that lut is the X coord for cursor
    LDA #$68
    STA spr_y            ; Y coord is always $68
    JMP DrawCursor       ; draw it, and exit
    RTS

  @lut:
    .BYTE $10,$48,$80,$B8

PlaySFX_MenuMove:
    LDA #%01111010   ; 25% duty, length counter disabled, decay disabled, volume=$A
    STA $4004

    LDA #%10011011   ; sweep pitch upwards at speed %001 with shift %011
    STA $4005

    LDA #$20
    STA $4006        ; set starting pitch to F=$020
    LSR A
    STA $4007

    STA sq2_sfx      ; indicate square 2 is playing a sound effect for $10 frames
    RTS              ;  and exit!

PlaySFX_MenuSel:
    LDA #%10111010   ; 50% duty, length disabed, decay disabed, volume=$A
    STA $4004

    LDA #%10111010   ; sweep pitch upwards at speed %011 with shift %010
    STA $4005

    LDA #$40         ; set starting pitch to F=$040
    STA $4006
    LDA #$00
    STA $4007

    LDA #$1F
    STA sq2_sfx      ; indicate square 2 is busy with sfx for $1F frames
    RTS              ;  and exit!


;assembled bytes
;A91E20428620B886CE3960F01B20C687B00BA9B348A90D48A90E4C03FEA9B348A92E48A90E4C03FE209787A5240525F0F7A90085248525A9B348
;A92E48A90E4C03FE8510187D0A619D0A61BD0B6169009D0B61DD0D61F01BB023A957854BA9308D0440A97F8D0540A9008D06408D0740A51060BD
;0A61DD0C61B00290DDBD0C619D0A61BD0D619D0B614C5A86AD0220A9208D0620A9008D0620A00098A2038D0720C8D0FACAD0F78D0720C8C0C090
;F8A9FF8D0720C8D0FA60A9008D01208537208C86A90B8539A9018538A91E853CA908853D2063E020DC864C658760A200A904853AA911853B20FD
;86C63BA20220FD86C63BA20420FD86C63BC63BA206BD0A87853EBD0B87853F4C8E87128728873E8753877A1006FFFFFF7A1106FFFFFF7A1206FF
;FFFF7A130600FF1005FFFFFFFF1105FFFFFFFF1205FFFFFFFF1305001002FFFFFFFF1102FFFFFFFF1202FFFFFFFF1302001000FFFFFF1100FFFF
;FF1200FFFFFF130000203CC420A8FEA9028D14402050D8A90885FF8D0020A91E8D0120A9008D05208D0520A91A85574C89C6A91A855785584C36
;DE20A8FEA9028D1440A5FF8D0020A9008D05208D0520A54B1004A951854BA91A85572089C6E6F0A900852485254CC2D7A90085248525203CC420
;1088209787A524D02DA525D02EA5202903C561F0E18561C900F0DBC901D008A5621869014CFC87A56238E901290385622023884CC68720398818
;602039883860A662BD1F888540A96885414C95EC60104880B8A97A8D0440A99B8D0540A9208D06404A8D0740857E60A9BA8D0440A9BA8D0540A9
;408D0640A9008D0740A91F857E60
