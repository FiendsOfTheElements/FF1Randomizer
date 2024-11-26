move_speed = $34
sq2_sfx = $7E
step_count = $60A0
ch_ailments = $6101
ch_curhp = $610A

.IF 0
.ORG $C7FB
MapPoisonDamage:            ; Vanilla is 102 BYTES total
    LDA move_speed          ; 2 
    BEQ @DoPoison           ; 2
    RTS                     ; 1
    ;;;;;;;;;;;;;;;;;;;;;;;;; 5 BYTES
  @DoPoison:
    LDY #0                  ; 2     | poison sfx flag, set if a character is poisoned
    LDX #0                  ; 2     | X will be our loop counter and char index
    ;;;;;;;;;;;;;;;;;;;;;;;;; 4 BYTES

  ;-----------------------
  ; Largely Vanilla:
  ;-----------------------
  @DmgLoop:
    LDA ch_ailments, X      ; 3     | get this character's ailments
    CMP #$03                ; 2     | see if it = 3 (poison)
    BNE @DmgSkip            ; 2     | if not... skip this character


    LDY #$01                ; 2     | NEW: set the SFX flag if a character is poisoned

    LDA ch_curhp+1, X       ; 3     | check high byte of HP
    BNE @DmgSubtract        ; 2     | if nonzero (> 255 HP), deal this character damage

    LDA ch_curhp, X         ; 3     | otherwise, check low byte of HP
    CMP #2                  ; 2     | see if >= 2 (don't take away their last HP)
    BCC @DmgSkip            ; 2     | if < 2, skip this character
                            ;       | otherwise... deal him damage
    ;;;;;;;;;;;;;;;;;;;;;;;;; 21 BYTES

  @DmgSubtract:
    LDA ch_curhp, X         ; 3     | subtract 1 from HP
    SEC                     ; 1
    SBC #1                  ; 2
    STA ch_curhp, X         ; 3
    LDA ch_curhp+1, X       ; 3
    SBC #0                  ; 2
    STA ch_curhp+1, X       ; 3
    ;;;;;;;;;;;;;;;;;;;;;;;;; 17 BYTES

  @DmgSkip:
    TXA                     ; 1     | add $40 char index
    CLC                     ; 1
    ADC #$40                ; 2
    TAX                     ; 1

    BNE @DmgLoop            ; 2     | and loop until it wraps (4 iterations)
    ;;;;;;;;;;;;;;;;;;;;;;;;; 7 BYTES

  ;------------------------
  ; NEW: Check step_count
  ;------------------------
    TYA                     ; 1     | skip sfx if flag not set
    BEQ @Exit               ; 2
    LDA step_count          ; 3     | otherwise, check step_count low byte
    AND #%00000011          ; 2     | MOD 4
    BNE @Exit               ; 2     | if step_count % 4 != 0, skip to exit...
    ;;;;;;;;;;;;;;;;;;;;;;;;; 10 BYTES

    ;;;;;;;;;;;;;;;;;;;;;;;;; 64 BYTES so far

    ;; Assembled Bytes
    ;; A534F00160A000A200BD0161C903D01FA001BD0B61D007BD0A61C9029011BD0A6138E9019D0A61BD0B61E9009D0B618A186940AAD0D398F01FADA0602903D018
 


  ; otherwise:
  ;------------------------------
  ; play sound here (see below)
  ;------------------------------
  ;;;;;;;;;;;;;;;;;;;;;;;;;;; 24 BYTES
  ;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;
  ;;;;;;;;;;;;;;;;;;;;;;;;;;; 88 BYTES = $58 total so far


  ;;;;; $C7FB + $58 = $C853
.ORG $C853
  @Exit:


    RTS                     ; 1     | then exit
    NOP                     ; 1
    NOP                     ; 1
    NOP                     ; 1
    NOP                     ; 1
    NOP                     ; 1
    NOP                     ; 1
    NOP                     ; 1
    NOP                     ; 1
    NOP                     ; 1
    NOP                     ; 1
    NOP                     ; 1
    NOP                     ; 1
    NOP                     ; 1
    ;;;;;;;;;;;;;;;;;;;;;;;;; 14 BYTES

    ;; Assembled Bytes
    ;; 60EAEAEAEAEAEAEAEAEAEAEAEAEA

    ;;;;;;;;;;;;;;;;;;;;;;;;;

    ;;;;;;;;;;;;;;;;;;;;;;;;; 88 + 14 = 102 BYTES total




;----------------------------------------------------------
;OUCH: Play vanilla poison sfx once instead of every frame
;----------------------------------------------------------
    LDA #%00111111          ; 2     | 12.5% duty (harsh), volume=$F (louder, to compensate)
    STA $4004               ; 3
    LDA #%10000001          ; 2     | sweep downward in pitch
    STA $4005               ; 3 
    LDA #$60                ; 2
    STA $4006               ; 3     | start at period=$060
    LDA #0                  ; 2     | 
    STA $4007               ; 3
    LDA #$06                ; 2     | indicate sq2 is busy with sound effects for 6 frames
    STA sq2_sfx             ; 2
    ;;;;;;;;;;;;;;;;;;;;;;;;; 24 BYTES

    ;; Assembled Bytes
    ;; A93F8D0440A9818D0540A9608D0640A9008D0740A906857E



;----------------------------------------
;BEEP: Like Zelda 1 heart warning sound
;----------------------------------------

    LDA #%10111010          ; 2     | 50% duty, volume = $A
    STA $4004               ; 3 
    LDA #%00000000          ; 2
    STA $4005               ; 3     | Disable sweep (not necessary w/ vanilla audio engine)
    LDA #$5E                ; 2     | period=$05E (D-natural 5)
    STA $4006               ; 3 
    LDA #0                  ; 2 
    STA $4007               ; 3
    LDA #$06                ; 2     | indicate sq2 is busy with sound effects for 6 frames
    STA sq2_sfx             ; 2
    ;;;;;;;;;;;;;;;;;;;;;;;;; 24 BYTES

    ;; Assembled Bytes
    ;; A9BA8D0440A9008D0540A95E8D0640A9008D0740A906857E


;----------------------------------------------------
;BONK: Like Dragon Warrior 1 bonk into wall sound
;----------------------------------------------------

    LDA #%10111111          ; 2     | 50% duty, volume = $F
    STA $4004               ; 3 
    LDA #%10110100          ; 2
    STA $4005               ; 3     | Sweep down 
    LDA #$F0                ; 2     | start at period=$2F0
    STA $4006               ; 3 
    LDA #$02                ; 2 
    STA $4007               ; 3
    LDA #$09                ; 2     | indicate sq2 is busy with sound effects for 9 frames
    STA sq2_sfx             ; 2
    ;;;;;;;;;;;;;;;;;;;;;;;;; 24 BYTES
    
    ;; Assembled Bytes
    ;; A9BF8D0440A9B48D0540A9F08D0640A9028D0740A909857E
.ENDIF

;----------------------------------------------------
;OOPS: Like Mario 1 stomp sound
;----------------------------------------------------

    LDA #%10111010          ; 2     | 50% duty, volume = $A
    STA $4004               ; 3
    LDA #%10001100          ; 2
    STA $4005               ; 3     | Sweep up
    LDA #$FF                ; 2     | start at period=$FF
    STA $4006               ; 3
    LDA #$00                ; 2
    STA $4007               ; 3
    LDA #$0A                ; 2     | indicate sq2 is busy with sound effects for 10 frames
    STA sq2_sfx             ; 2
    ;;;;;;;;;;;;;;;;;;;;;;;;; 24 BYTES

    ;; Assembled Bytes
    ;; A9BA8D0440A98C8D0540A9FF8D0640A9008D0740A90A857E

