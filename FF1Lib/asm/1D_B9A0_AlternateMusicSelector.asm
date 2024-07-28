Music_NewSong           = $BA03

music_track             = $4B
cur_map                 = $48
btlformation            = $6A

TRACK_SHRINE            = $4C
TRACK_SEA               = $59
MAP_TOF                 = $0C
CHAOS_FORMATION         = $7B
TRACK_BATTLE            = $50
TRACK_BOSS_BATTLE       = $5A


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
