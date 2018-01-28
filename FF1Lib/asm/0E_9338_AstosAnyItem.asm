;; Rewrite Astos $9338
    LDA item_crown              ; check required item AD2260
    BEQ @Default                ; F016
      LDA tmp+3                 ; load item to give A513
      BEQ @Default              ; if there's an item to give F012
        JSR GiveItem            ; give it 2094DD
        BCS @End                ; if we don't already have it (Can't hold text) B00F
        LDY #OBJID_ASTOS        ; A007
        JSR HideMapObject       ; hide (kill) Astos' map object (this object) 207392
        LDA #BTL_ASTOS          ; trigger battle with Astos A97D
        JSR TalkBattle          ; 20C590
        LDA tmp+2               ; The NPC generic item gift text A512
        RTS                     ; 60
@Default:
    LDA tmp+1                   ; otherwise print default text A511
@End:
    RTS                         ; 60 