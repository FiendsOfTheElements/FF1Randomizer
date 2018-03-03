;; Rewrite Bikke  $92D0
    LDY #OBJID_PIRATETERR_1      ; A03F
    JSR IsObjectVisible          ; 209190
    BCS @AlreadyFought           ; if we already have, skip ahead B00B
      JSR ShowMapObject          ; and show a bunch of scaredy-cat townspeople that the pirates 20A490
      LDA #BTL_BIKKE             ; then start a battle with Bikke (his pirates) A97E
      JSR TalkBattle             ; 20C590
      LDA tmp+1                  ; and print [1] A511
      RTS                        ; 60
  @AlreadyFought:                ; if we've already fought bikke...
    LDY #OBJID_BIKKE             ; A004
    JSR CheckGameEventFlag       ; check Bikke's event flag to see if we got item from him yet 207990
    BCS @Default                 ; if we already have, skip ahead B013
      LDA tmp+3                  ; load item to give A513
      BEQ @Default               ; if there's an item to give F00F
      STY tmp                    ; 8410
      JSR GiveItem               ; give it 2094DD
      BCS @End                   ; if we don't already have it (Can't hold text) B00A
      LDY tmp                    ; A410
      JSR SetGameEventFlag       ; otherwise, set event flag to mark him as done 207F90
      LDA #$3A                   ; The NPC generic item gift text A93A
      RTS                        ; 60
  @Default:                      ; otherwise, if we have the ship already
    LDA tmp+2                    ; just print [3] A512
  @End:
    RTS                          ; 60 