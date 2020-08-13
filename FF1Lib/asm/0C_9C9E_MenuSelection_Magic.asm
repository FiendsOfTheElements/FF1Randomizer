
;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;
;;
;;  MenuSelection_Magic  [$9C14 :: 0x31C24]
;;
;;    Same idea as MenuSelection_2x4, but rewritten as it is more complex than
;;  a basic 2x4 menu.  See MenuSelection_2x4 for input/output and other details.
;;
;;  additional output:   $6AF8 = magic page
;;
;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;

btlcurs_x = $6AAA
btlcurs_y = $6AAB
btl_input = $68B3

DrawBattleMagicBox_L = $F21B
UndrawNBattleBlocks_L = $F20F
Cursor_Right = $9CFE
Cursor_Left = $9CF0


 .ORG $9C9E              ; Change only at the Dpad portion of the routine

CheckDPad:               ; Same as original routine
    LDA btl_input
    AND #$F0
    CMP #$80
    BEQ Cursor_Right
    CMP #$40
    BEQ Cursor_Left
    CMP #$20
    BEQ Cursor_Down
    CMP #$10
    BEQ Cursor_Up
    RTS
    
Cursor_Down:                   
    LDA btlcurs_y            
    AND #$03
    CMP #$03                   ; see if it's at the bottom of the page
    BNE NormalMove_Down        ; if not, do a normal move
    JSR SwitchPage             ; Instead of stopping or wrapping down, we jump to new switch page routine
NormalMove_Down:
    INC btlcurs_y
    RTS

Cursor_Up:
    LDA btlcurs_y             
    AND #$03                   ; top row of page?
    BNE NormalMove_Up          ; no?  then normal move
    JSR SwitchPage             ; Instead of stopping or wrapping up, we jump to new switch page routine
NormalMove_Up:
    DEC btlcurs_y
    RTS

SwitchPage:                    ; New routine to switch page
    INC $6AF8                  ; 0 is first page, and 1 is second page, so we increase the page count
    LDA $6AF8                  ; then we zero out all bits except the first one
    AND #$01                   ; so even is always first page, odd is always second page
    STA $6AF8                  ; update the new page
    LDA #$01                   ; load number of battle block to erase
    JSR UndrawNBattleBlocks_L  ; erase
    JSR DrawBattleMagicBox_L   ; draw new box
    RTS