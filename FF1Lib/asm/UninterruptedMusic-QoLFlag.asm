;; Last revision: 2022-Apr-20
;; Plays a new sound effect when opening standard treasure chests to avoid resetting the music
;; Uses the same musical notes as the original chest SFX: G B D# G B
;; Notes are played over 27 frames - input is locked while it plays, like normal
;; (Plays notes one octave lower than the original sound, because it sounds less jarring)
;; Main section is called from the end of DialogueBox_Frame, so gets run once per frame

.include "variables.inc"
DialogueBox_Frame = $D6A1
CallMusicPlay_L = $C009
SwapPRG_L = $FE03


;;;; Overwrite D62B - D634 (in Bank 1F) for TreasureChestSFXSetup [10 bytes]
;; A9 0F 20 03 FE 20 65 A0 EA EA
 .ORG $D62B		; 07D63B
		LDA #$0F
		JSR SwapPRG_L
		JSR TreasureChestSFXSetup
		NOP
		NOP

		
;;;; Overwrite D675 - D688 for WaitForSFXToFinish [20 bytes]
;; A9 0F 20 03 FE 20 73 A0 4C 88 D6 A9 0F 20 03 FE 4C 00 A0 EA
 .ORG $D675		; 07D685
		LDA #$0F
		JSR SwapPRG_L
		JSR WaitForSFXToFinish
		JMP ContinueDialogBox_FromSFX
		
	CheckForTreasureSFX_Start:
		LDA #$0F
		JSR SwapPRG_L
		JMP CheckForTreasureSFX
		
	ContinueDialogBox_FromSFX:
		NOP												; Original game code continues from here
	

;;;; Overwrite D6C4 - D6C6 for CheckForTreasureSFX [3 bytes]
;; 4C 80 D6
 .ORG $D6C4		; 07D6D4
		JMP CheckForTreasureSFX_Start



;;;; New Code Section, A000+
;; A5 7D C9 0A 90 5C C6 7D C9 63 90 4C C9 7D B0 17 C9 6A 90 3F C9 6F 90 36 C9 74 90 2D C9 78 F0 1F C9 78 90 20 4C 3A A0 A9 1B 85 7E A9 7F 8D 04 40 A9 7F 8D 05 40 A9 09 8D 07 40 A9 1C 4C 5F A0 A9 08 8D 07 40 A9 E1 4C 5F A0 A9 C9 4C 5F A0 A9 8E 4C 5F A0 A9 70 4C 5F A0 A9 32 85 7D 4C 09 C0 8D 06 40 4C 09 C0 A5 7D C9 01 F0 05 A9 7E 85 7D 60 A2 54 86 4B 60 A5 7D C9 32 F0 14 A5 4B C9 81 F0 0A A9 0F 85 57 20 A1 D6 4C 75 A0 A5 7C 85 4B A9 00 85 7D 60
 .ORG $A000		; 03E010

CheckForTreasureSFX:
		LDA dlgsfx
		CMP #10								; <10; If dlgsfx is 0-2, quit: Either we're done playing, or aren't ready to start
		BCC @Exit							; (Using 10 instead of 3 just in case - anything 3-49 should work the same)

		DEC dlgsfx						; Count down 27 frames (Counter starts at 126 -
													; Counter is +100 to avoid a conflict where dlgsfx will be in use right before we start)
		CMP #99
		BCC @TreasureSFXDone	; <99; we're done (Note5 plays for 7 frames; Notes1-4 play for 5)
		
		CMP #125							; >=25
		BCS @Note1Start

		CMP #106							; <6
		BCC @Note5

		CMP #111							; <11
		BCC @Note4

		CMP #116							; <16
		BCC @Note3

		CMP #120							; =20
		BEQ @Note2Start

		CMP #120  						; <20
		BCC @Note2

		JMP @Note1 						; Else, 21-24

	@Note1Start:						; Sets up the notes to be played
    LDA #27             	; indicate sq2 is busy with sound effects for __ frames
    STA sq2_sfx
		
		LDA #%01111111      	; 25% duty (mid), volume=max - Same for all notes
    STA $4004
    LDA #%01111111     	  ; no sweep - Same for all notes
    STA $4005
		
		LDA #%00001001				; Length=1 frame. First note is low enough to need additional freq. bits from 4007
    STA $4007							; 4007 should only be set once per note (at most), or it'll cause glitchy sounds
	@Note1:
    LDA #%00011100				; 1 00011100 = G4
		JMP @ExitNote
		
	@Note2Start:
		LDA #%00001000				; Used for Notes 2-5 - high enough freq. to not need the low-bits any more
    STA $4007
	@Note2:
    LDA #%11100001				; 11100001 = B4
    JMP @ExitNote
		
	@Note3:
    LDA #%11001001				; 11001001 = D#5
    JMP @ExitNote

	@Note4:
    LDA #%10001110				; 10001110 = G5
    JMP @ExitNote
		
	@Note5:
    LDA #%01110000				; 01110000 = B5
		JMP @ExitNote
		
	@TreasureSFXDone:
		LDA #50
		STA dlgsfx						; Sets dlgsfx to special value 50 so the game knows the sfx is done playing
		JMP CallMusicPlay_L		; Exit to the background music player

	@ExitNote:
    STA $4006							; Apply the note info
	@Exit:
		JMP CallMusicPlay_L		; Exit to the background music player


;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;
;; This replaces the normal dlgsfx check that tells the game to start playing the fanfare/TC SFX
TreasureChestSFXSetup:
		LDA dlgsfx 
		CMP #1
		BEQ @NormalFanfareSetup	; Check if dlgsfx is 1 (fanfare TC) or 2 (normal TC)
			LDA #126							; Hijacking dlgsfx because it isn't needed here any more
			STA dlgsfx						; Set counter for processing new TC sfx, counting down 27 frames
			RTS
			
	@NormalFanfareSetup:			; Play the incentive fanfare like normal (will reset the music)
			LDX #$54
			STX music_track
RTS


;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;
;; This replaces the check that waits for treasure fanfare/sfx to finish playing
WaitForSFXToFinish:
		LDA dlgsfx
		CMP #50									; check if dlgsfx has been set to 50 (regular chest sfx over)
		BEQ @SFXIsDone					; break if so

    LDA music_track
    CMP #$81                ; check if music track is set to $81 (fanfare sfx over)
    BEQ @FanfareIsDone      ; break if so

		LDA #$0F
		STA cur_bank						; saves current memory bank so processing can return here after frame advance
		
    JSR DialogueBox_Frame 	; otherwise, keep doing frames
    JMP WaitForSFXToFinish	; and loop until the sfx is done

  @FanfareIsDone:
    LDA dlgmusic_backup     ; once fanfare is done restore the music track to the backup value
    STA music_track
		
	@SFXIsDone:								; regular sfx doesn't need to reset the music
    LDA #0
    STA dlgsfx              ; clear sfx flag
RTS