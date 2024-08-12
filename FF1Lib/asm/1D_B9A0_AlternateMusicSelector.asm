Music_NewSong           = $BA03

music_track             = $4B
cur_map                 = $48
btlformation            = $6A

TRACK_SHRINE            = $4C
TRACK_SEA               = $59
TRACK_BATTLE            = $50
TRACK_BOSS_BATTLE       = $5A
TRACK_MERMAIDS          = $5B
MAP_TOF                 = $0C
MAP_MERMAIDS            = $2E
CHAOS_FORMATION         = $7B



.org $B9A0  ;bank 1D
; Selects alternate music tracks in certain situations
; Track to be played is in A
SelectMusic:
        CMP #TRACK_SHRINE
        BNE Towns
        TAX
        LDA cur_map
        CMP #MAP_TOF
        BNE ShrineNotTof
        LDA #TRACK_SHRINE
        JMP Done
ShrineNotTof:
        CMP #MAP_MERMAIDS
        BNE ShrineNotMermaids
        LDA #TRACK_MERMAIDS
        JMP Done
ShrineNotMermaids:
        LDA #TRACK_SEA
        JMP Done
Towns:
Battles:
        CMP #TRACK_BATTLE
        BNE Done
        LDA btlformation
        AND #$7F
        CMP #CHAOS_FORMATION
        BNE RegularBattle
        LDA #TRACK_BOSS_BATTLE
        JMP Done
RegularBattle:
        LDA #TRACK_BATTLE
        JMP Done
Done:
        STA music_track
        JMP Music_NewSong




SwapPRG                 = $FE03
MusicPlay_L             = $B9F0
.org $B099  ;bank 0D
; Redirect for a few routines like ending and bridge scene are in 
;   bank D and call MusicPlay directly without going through bank 1F
CallMusicPlay:
        LDA #>(MusicPlay_L-1)
        PHA
        LDA #<(MusicPlay_L-1)
        PHA
        LDA #$1D
        JMP SwapPRG


.org $B9F0  ;bank 1D
MusicPlay               = $BA00
SwapPRG                 = $FE03
; Call MusicPlay and then jump back to bank 0D
MusicPlay_L:
    JSR MusicPlay
    LDA #$0D
    JMP SwapPRG
