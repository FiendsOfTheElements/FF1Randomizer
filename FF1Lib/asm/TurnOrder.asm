; Bank 0C, DoBattleRound

    LDA #13 - 1
    STA btl_curturn             ; temp loop counter (loop from 12 down to 1)
  @ShuffleTurnOrderLoop:
    LDA #$00
    LDX btl_curturn
    JSR RandAX                  ; pick a random index
    TAY
    LDX btl_curturn             ; swap that entry with the top not-already-shuffled entry
    
    LDA btl_turnorder, Y        ; swap those entries in the turn order
    PHA
    LDA btl_turnorder, X
    STA btl_turnorder, Y
    PLA
    STA btl_turnorder, X
    
    DEC btl_curturn             ; keep looping until counter expires
    BNE @ShuffleTurnOrderLoop
