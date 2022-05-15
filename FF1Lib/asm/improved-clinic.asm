;In the routine that lists eligible characters,
;instead of only adding characters if they are
;dead, we add them if they have any status
;(so a non-zero status byte).

;3A706:
    CMP #$00              ; C9 00
    BEQ @NotDead          ; F0 1C


;In the routine that revives the selected character,
;we only want to change HP if the character was dead,
;so we first load their status byte to check.
;Conveniently, "dead" is represented by 1 so we can
;just copy that into their HP, otherwise skip to the
;part that clears the status byte.

;3A5EF
	LDA ch_ailments, X         ; BD 01 61
	CMP #$01                   ; C9 01
	BNE @SkipHP                ; D0 03
   STA ch_curhp, X            ; 9D 0A 61

  @SkipHP:
    LDA #$00                   ; A9 00
    STA joy_a                  ; 85 24
    STA joy_b                  ; 85 25
    STA ch_ailments, X         ; 9D 01 61 EA EA


