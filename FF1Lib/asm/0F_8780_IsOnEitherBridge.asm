;; IsOnAnyBridge
;; A replacement to IsOnBridge that also allows walking on the canal / canal bridge
;; Last revision: 2021-08-05

canal_vis = $600C
canal_x = $600D
canal_y = $600E

IsOnAnyBridge:
    LDA $6008
    BEQ IsOnOtherBridge; if no bridge, canal?

    LDA $12
    CMP $6009
    BNE IsOnOtherBridge; if wrong X, canal?

    LDA $13
    CMP $600A
    BNE IsOnOtherBridge; same with Y coord

    LDA #0             ; otherwise... success!
    STA $45            ; zero the tileprop+1
    CLC                ; CLC to indicate success
    RTS                ; and exit

IsOnOtherBridge:      ; check the canal tile
    LDA $12
    CMP $600D
    BNE Fail          ; X coord

    LDA $13
    CMP $600E
    BNE Fail          ; Y coord

    LDA #0             ; do all same stuff on success
    STA $45
    CLC
    RTS

Fail:
    SEC                ; SEC to indicate failure
    RTS                ; and exit

;; ------------------------------------------------------------ ;;

;; Bridge or Canal Drawing Code
;; We leave the exit here to make the code assemble more easily.
;; When pasting into FFR just take everything from DrawOWObj_BridgeCanal onward.
Exit:
    RTS

; This filler code doesn't work. But it's the same size as a block
; in the vanilla rom so that the BCS Exit calculates the right delta.
; Just Put in the code from the DrawOWObj_BridgeCanal method.
FillerCodeToMakeBranchToExitWork:
	LDA $6000
	BEQ Exit
	LDX $6000
	LDY $6000
	JSR $6000
	BCS Exit
	LDA #$08
	JSR Draw

DrawOWObj_BridgeCanal:
    LDX canal_x  
    LDY canal_y
	JSR $E3DF     ; ConvertOWToSprite
	BCS Exit      ; oo bounds, exit

	LDA #$08      ; Isthmus/Bridge is on screen. Start by assuming bridge
	LDX canal_vis ; Check canal_vis
	BEQ Draw
	ASL           ; Switch table to offset $10 (canal) if necessary (ASL saves 1 byte from LDA #$10)

Draw:
; This is where the sprite is drawn.
; We take over one byte (CLC) since C is always clear if this part is reached.
