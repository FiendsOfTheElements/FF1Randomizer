tmp = $10
rng_state = $6FF0
state_lo = rng_state + $0
state_hi = rng_state + $1

;;; put some unused bytes to work 
lut_BattleStepSeed = $DB09
NewGame_LoadStartingStats = $C76D



;;;;;;;;;;;;;;;;;;;
;; BANK 1E
;;;;;;;;;;;;;;;;;;;
.ORG $806B
    NOP
    NOP
    JSR PartyGen_ShutOffPPUAndLoadBattleStepSeed

    ;;; assembled bytes 
    ;;; EAEA20008C



.ORG $8C00
PartyGen_ShutOffPPUAndLoadBattleStepSeed:
    ;;; we call this subroutine from bank $1E $806B
    ;;; do the PPU reset that was there
    LDA #$00
    STA $2001
    ;;; load the battle step seed into sram
    LDA lut_BattleStepSeed
    STA state_lo
    LDA lut_BattleStepSeed + 1
    STA state_hi
    RTS

    ;;; assembled bytes
    ;;; A9008D0120AD09DB8DF06FAD0ADB8DF16F60




;;;;;;;;;;;;;;;
;; BANK 1F
;;;;;;;;;;;;;;;
.ORG $C571  ;; location of BattleStepRNG routine

;;;;; 4 16-bit xorshift PRNGs of period 65,535
;;;;; Rando chooses one of them to replace BattleStepRNG
;;;;; Vanilla BattleStepRNG is 27 bytes
;; x ^= x << 7
;; x ^= x >> 9
;; x ^= x << 8
BattleStepRNG1:
    LDA state_hi        ; 3
    LSR                 ; 1 -- prev bit 8 in carry
    LDA state_lo        ; 3
    ROR                 ; 1 -- A has prev bits [8,7,6,5,4,3,2,1], prev bit 0 in carry 
    EOR state_hi        ; 3 -- hi part of x ^= x<<7; the lo part (EOR bits 0 and 7) happens below
    STA tmp + 1         ; 2
    ROR                 ; 1 -- prev bit 0 from x ^= x<<7 shift rotated in
    EOR state_lo        ; 3 -- and applied here w/ x ^= x >> 9
    STA state_lo        ; 3
    EOR tmp + 1         ; 2 -- x ^= x << 8
    STA state_hi        ; 3                 ; 1 
    RTS                 ; 1 -- hi byte in A
    NOP                 ; 1
    ;------------------------
                        ; 27 bytes
    ;;; assembled bytes
    ;;; ADF16F4AADF06F6A4DF16F85116A4DF06F8DF06F45118DF16F60EA



;; same as above w/ opposite shifts.
;; order of PRNG is different
;; x ^= x >> 7
;; x ^= x << 9
;; x ^= x >> 8
BattleStepRNG2:
    LDA state_lo        ; 3
    ASL                 ; 1
    LDA state_hi        ; 3
    ROL                 ; 1
    EOR state_lo        ; 3
    STA tmp + 1         ; 2
    ROL                 ; 1
    EOR state_hi        ; 3
    STA state_hi        ; 3
    EOR tmp + 1         ; 2
    STA state_lo        ; 3
    RTS                 ; 1 -- lo byte in A
    NOP                 ; 1
    ;------------------------
                        ; 27 bytes

    ;; assembled bytes
    ;; ADF06F0AADF16F2A4DF06F85112A4DF16F8DF16F45118DF06F60EA




;;;; rearranging the shift order also
;;;; reorders the PRNG 
;; x ^= x << 8
;; x ^= x << 7
;; x ^= x >> 9
BattleStepRNG3:
    LDA state_lo        ; 3
    EOR state_hi        ; 3 -- x ^= x << 8
    STA tmp + 1         ; 2
    LSR                 ; 1 -- bit 8 now in carry
    LDA state_lo        ; 3
    ROR                 ; 1 -- bit 0 now in carry
    EOR tmp + 1         ; 2 -- hi part of x ^= x << 7
    STA state_hi        ; 3
    ROR                 ; 1 -- previous bit 0 now in bit 15
    EOR state_lo        ; 3 -- lo part of x ^= x << 7, and x ^= x >> 8
    STA state_lo        ; 3
    RTS                 ; 1 -- lo byte in A
    NOP                 ; 1
    ;------------------------
                        ; 27 bytes

    ;; assembled bytes 
    ;; ADF06F4DF16F85114AADF06F6A45118DF16F6A4DF06F8DF06F60EA


;; x ^= x >> 8
;; x ^= x >> 7
;; x ^= x << 9
BattleStepRNG4:
    LDA state_hi        ; 3
    EOR state_lo        ; 3
    STA tmp + 1         ; 2
    ASL                 ; 1
    LDA state_hi        ; 3
    ROL                 ; 1
    EOR tmp + 1         ; 2
    STA state_lo        ; 3
    ROL                 ; 1
    EOR state_hi        ; 3
    STA state_hi        ; 3
    RTS                 ; 1 -- hi byte in A 
    NOP
    ;------------------------
                        ; 27 bytes 

    ;; assembled bytes
    ;; ADF16F4DF06F85110AADF16F2A45118DF06F2A4DF16F8DF16F60EA