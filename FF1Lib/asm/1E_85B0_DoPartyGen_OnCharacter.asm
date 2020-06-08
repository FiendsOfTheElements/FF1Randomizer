 .ORG $85B0

define ptygen_class $0300
define ptygen_empty $0301

define lut_PtyGenBuf $84AA

; The pool is stored right after ptygen in memory
define pool_maxchar $09
define pool_select $0340     ; Current character selected
define pool_taken $0341      ; Taken mask so you can't take the same character twice
define pool_char $0342       ; Characters pool, 8 bytes

DoPartyGen_OnCharacter:
    LDX char_index           ; Start by restoring last char if B button was pressed
    LDA ptygen_empty,X       ; char availability mask was stored in empty ptygen byte
    ORA pool_taken
    STA pool_taken  
    JSR PtyGen_DrawScreen    ; Normal draw screen

  @MainLoop:
    JSR PtyGen_Frame
    LDX char_index
    LDY pool_select          ; Get the current char selected
      
    LDA joy_a                ; Pressing A button
    BEQ @SkipA
      LDA ptygen_class,X     ; Check if None was choosen
      CMP #$FF
      BEQ :+                 ; If it's not, update availability mask
        LDA lut_CharMask,Y
        STA ptygen_empty,X
        EOR pool_taken
        STA pool_taken      
    : JMP DoNameInput        ; and go to name input
  @SkipA:
    LDA joy_b                ; Pressing B button
    BEQ @SkipB
      TXA                    ; Check if the current slot is the first
      CMP #$00               ;  we don't want to allow 4 nones
      BEQ :+                 ; If it's not, put None before freeing character 
        LDA #$00
        STA ptygen_empty,X     
        LDA #$FF
        STA ptygen_class,X  
    : SEC
      RTS
  @SkipB:
    LDA joy                  ; Check directional input
    AND #$0F
    CMP joy_prevdir
    BEQ @MainLoop

    STA joy_prevdir    
    CMP #$00                 ; if directional input released (rather than pressed)
    BEQ @MainLoop            ;  loop to another frame.
  @retry:
    INY                      ; Cycle available characters
    TYA
    CMP #pool_maxchar
    BCC :+                   ; Loop back if we go over the max
      LDY #$00
  : STY pool_select          
    LDA lut_CharMask,Y      ; Check if the char is still available
    BIT pool_taken
    BEQ @retry               ; If not loop, else load it in memory
    LDA pool_char,Y
    STA ptygen_class,X
     
    LDA #$01
    STA menustall

    JSR PtyGen_DrawOneText  
    JMP @MainLoop

lut_CharMask:
  .BYTE $80, $40, $20, $10, $08, $04, $02, $01      
