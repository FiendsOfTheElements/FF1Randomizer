PrintGold               lda $601c
                    sta $10
                    lda $601d
                    sta $11
                    lda $601e
                    sta $12
                    jmp L8e8e

PrintNumber_5Digit               jsr FormatNumber_5Digits
                    jsr TrimZeros_5Digits
                    lda #$5b
                    bne L8e98
L8e8e               jsr FormatNumber_6Digits
                    jsr TrimZeros_6Digits
                    lda #$5a
                    bne L8e98
L8e98               sta $3e
                    lda #$00
                    sta $3f
                    rts
                    
TrimZeros_6Digits               lda $5a
                    cmp #$80
                    bne TrimZeros_RTS
                    lda #$ff
                    sta $5a
TrimZeros_5Digits               lda $5b
                    cmp #$80
                    bne TrimZeros_RTS
                    lda #$ff
                    sta $5b
                    lda $5c
                    cmp #$80
                    bne TrimZeros_RTS
                    lda #$ff
                    sta $5c
                    lda $5d
                    cmp #$80
                    bne TrimZeros_RTS
                    lda #$ff
                    sta $5d
                    lda $5e
                    cmp #$80
                    bne TrimZeros_RTS
                    lda #$ff
                    sta $5e
TrimZeros_RTS               rts
                    
FormatNumber_6Digits               ldx #$08
L8ed4               lda $12
                    cmp $8fd1,x
                    beq L8ee7
                    bcs L8ef
L8edd               dex
                    bpl L8ed4
                    ldx #$80
                    stx $5a
                    jmp FormatNumber_5Digits
                    
L8ee7               lda $11
                    cmp $8fda,x
                    beq L8ef2
                    bcc L8edd
                    bcs L8ef9
L8ef2               lda $10
                    cmp $8fe3,x
                    bcc L8edd
L8ef9               lda $10
                    sec
                    sbc $8fe3,x
                    sta $10
                    lda $11
                    sbc $8fda,x
                    sta $11
                    lda $12
                    sbc $8fd1,x
                    sta $12
                    txa
                    clc
                    adc #$81
                    sta $5a
FormatNumber_5Digits               ldx #$08
L8f17               lda $12
                    cmp $8fec,x
                    beq L8f2a
                    bcs L8f3c
L8f20               dex
                    bpl L8f17
                    ldx #$80
                    stx $5b
                    jmp FormatNumber_4Digits
                    
L8f2a               lda $11
                    cmp $8ff5,x
                    beq L8f35
                    bcc L8f20
                    bcs L8f3c
L8f35               lda $10
                    cmp $8ffe,x
                    bcc L8f20
L8f3c               lda $10
                    sec
                    sbc $8ffe,x
                    sta $10
                    lda $11
                    sbc $8ff5,x
                    sta $11
                    lda $12
                    sbc $8fec,x
                    sta $12
                    txa
                    clc
                    adc #$81
                    sta $5b
FormatNumber_4Digits               ldx #$08
L8f5a               lda $11
                    cmp $9007,x
                    beq L8f6d
                    bcs L8f74
L8f63               dex
                    bpl L8f5a
                    ldx #$80
                    stx $5c
                    jmp FormatNumber_3Digits
                    
L8f6d               lda $10
                    cmp $9010,x
                    bcc L8f63
L8f74               lda $10
                    sec
                    sbc $9010,x
                    sta $10
                    lda $11
                    sbc $9007,x
                    sta $11
                    txa
                    clc
                    adc #$81
                    sta $5c
FormatNumber_3Digits               ldx #$08
L8f8b               lda $11
                    cmp $9019,x
                    beq L8f9e
                    bcs L8fa5
L8f94               dex
                    bpl L8f8b
                    ldx #$80
                    stx $5d
                    jmp FormatNumber_2Digits
                    
L8f9e               lda $10
                    cmp $9022,x
                    bcc L8f94
L8fa5               lda $10
                    sec
                    sbc $9022,x
                    sta $10
                    lda $11
                    sbc $9019,x
                    sta $11
                    txa
                    clc
                    adc #$81
                    sta $5d
FormatNumber_2Digits               ldx #$00
                    lda $10
L8fbe               cmp #$0a
                    bcc L8fc7
                    sbc #$0a
                    inx
                    bne L8fbe
L8fc7               ora #$80
                    sta $5f
                    txa
                    ora #$80
                    sta $5e
                    rts
					
CheckGameEventFlag               lda cursor00,y
                    lsr a
                    lsr a
                    rts					

					
S9c02               lda $2002
                    lda #$20
                    sta $2006
                    lda #$00
                    sta $2006
                    ldy #$00
                    tya
                    ldx #$03
L9c14               sta $2007
                    iny
                    bne L9c14
                    dex
                    bne L9c14
L9c1d               sta $2007
                    iny
                    cpy #$c0
                    bcc L9c1d
                    lda #$ff
L9c27               sta $2007
                    iny
                    bne L9c27
                    rts				
				
				
@RecordClassAndName               clc
                    txa
                    adc #$18
                    sta $10
                    lda #$61
                    sta $11
                    ldy #$07
L9cf5               lda ($10),y
                    and #$7f
                    sta ($10),y
                    dey
                    bpl L9cf5
                    lda #$00
                    ldy #$08
L9d02               sta ($10),y
                    iny
                    cpy #$0e
                    bne L9d02
                    rts
					
					
DoPartyGen_OnCharacter-jumps-into-routine               lda #$03
                    jsr Saa3b
                    lda #$11
                    sta $3e
                    lda #$9d
                    sta $3f
                    jsr DrawShopComplexString
                    lda #$02
                    sta $0062
                    jmp La907

DoLevelUp	    
					LDA #$9D
					PHA
					LDA #$4A
					PHA
					LDA #$87
					PHA
					LDA #$A9
					PHA
					TXA
					CLC
					ROL A
					ROL A
					ROL A
					STA $10
					LDA #$1B
					JMP $FE03
					LDA #$00
					STA $0024
					STA $0025
					STA $2001
					RTS

ClinicShop_Exit               rts
                    
EnterShop_Clinic               lda #$00
                    sta $24
                    sta $25
                    jsr DrawShopDialogueBox
                    ldy #$ff
La5ad               iny
                    lda $a71d,y
                    sta $0310,y
                    bne La5ad
                    lda #$02
                    sta $0310,y
                    iny
                    ldx cur_map
                    lda $9d0a,x
                    adc #$f0
                    sta $0310,y
                    iny
                    lda #$05
                    sta $0310,y
                    iny
                    lda #$c5
                    sta $0310,y
                    iny
                    lda #$00
                    sta $0310,y
                    lda #$10
                    sta $3e
                    lda #$03
                    sta $3f
                    jsr DrawShopComplexString
                    jsr Clinic_SelectTarget
                    lda cursor
                    sta $030c
                    bcs ClinicShop_Exit
                    jsr DrawInnClinicConfirm
                    jsr ShopLoop_YesNo
                    bcs EnterShop_Clinic
                    lda cursor
                    bne EnterShop_Clinic
                    jsr InnClinic_CanAfford
                    lda $030c
                    clc
                    adc $030c
                    adc $030c
                    tax
                    lda $0310,x
                    ror a
                    ror a
                    ror a
                    and #$c0
                    pha
                    clc
                    jsr DoPartyGen_OnCharacter-jumps-into-routine
                    pla
                    bcs EnterShop_Clinic
                    tax
                    lda cursor
                    bne @ClassChangeSkip
                    txa
                    adc #$0a
                    sta $10
                    lda #$61
                    sta $11
                    txa
                    pha
                    sta $12
                    lda #$63
                    sta $13
                    ldy #$00
                    lda #$00
La631               sta ($10),y
La633               sta ($12),y
                    iny
                    cpy #$0a
                    bmi La631
                    cpy #$40
                    bne La633
                    sta $6126,x
                    jsr @RecordClassAndName
                    ldy cur_map
                    lda $9d0a,y
                    sta $6100,x
                    jsr $c271
                    ldy #$0e
                    jsr CheckGameEventFlag
                    pla
                    tax
                    bcc @ClassChangeSkip
                    clc
                    lda $6100,x
                    adc #$06
                    sta $6100,x
@ClassChangeSkip               lda #$01
                    sta $610a,x
					lda #$00
					sta $6101,x
		    jsr DoLevelUp
                    jsr LoadMenuCHRPal
                    jsr DrawShop
                    lda #$21
                    jsr DrawShopDialogueBox
@ReviveLoop               jsr ShopFrameNoCursor
                    lda $24
                    ora $25
                    beq @ReviveLoop
                    jmp EnterShop_Clinic










InnClinic_CanAfford               lda $601e
                    bne La6b9
                    lda $601d
                    cmp $0301
                    beq La69a
                    bcs La6b9
                    bcc La6a2
La69a               lda $601c
                    cmp $0300
                    bcs La6b9
La6a2               lda #$10
                    jsr DrawShopDialogueBox
                    lda #$00
                    sta $24
                    sta $25
La6ad               jsr ShopFrameNoCursor
                    lda $24
                    ora $25
                    beq La6ad
                    pla
                    pla
                    rts
                    
La6b9               lda $601c
                    sec
                    sbc $0300
                    sta $601c
                    lda $601d
                    sbc $0301
                    sta $601d
                    lda $601e
                    sbc #$00
                    sta $601e
                    jmp DrawShopGoldBox
                    
Clinic_SelectTarget               lda #$03
                    jsr DrawShopBox
                    jsr ClinicBuildNameString
                    lda #$10
                    sta $3e
                    lda #$03
                    sta $3f
                    jsr DrawShopComplexString
                    jmp CommonShopLoop_Cmd
                    
Sa6ed               ldy #$00
                    ldx #$00
                    stx $63
La6f3               txa
                    rol a
                    rol a
                    rol a
                    and #$03
                    adc #$10
                    sta $0310,y
                    lda #$00
                    sta $0311,y
                    lda #$01
                    sta $0312,y
                    tya
                    clc
                    adc #$03
                    tay
                    inc $63
                    txa
                    clc
                    adc #$40
                    tax
                    bne La6f3
                    lda #$00
                    sta $0310,y
                    sec
                    rts
                    
ShopFrame               jsr $c43c
                    jsr DrawShopPartySprites
                    jsr DrawShopCursor
                    jsr $fe00
                    lda #$02
                    sta $4014
                    lda #$0e
                    sta $57
                    jsr $c689
                    jmp La758
                    rts
                     
ShopFrameNoCursor               jsr $c43c
                    jsr DrawShopPartySprites
                    jsr $fe00
                    lda #$02
                    sta $4014
                    lda #$0e
                    sta $57
                    jsr $c689
La758               lda $20
                    and #$0f
                    sta $17
                    jsr $d7c2
                    lda $24
                    ora $25
                    beq La76a
                    jmp PlaySFX_MenuSel
                    
La76a               lda $20
                    and #$0f
                    beq La777
                    cmp $17
                    beq La777
                    jmp PlaySFX_MenuMove
                    
La777               rts
                    
DrawShop               jsr Sa7d1
                    jsr S9c02
                    lda $2002
                    lda #$23
                    sta $2006
                    lda #$c0
                    sta $2006
                    ldx #$00
					
La78d               lda $ac5c,x
                    sta $2007
                    inx
                    cpx #$40
                    bcc La78d
                    ldx $66
                    lda $ac54,x
                    sta $12
                    lda #$06
                    sta $3b
                    lda #$0b
                    sta $3a
                    lda #$0a
                    sta $3c
                    sta $3d
                    lda #$9c
                    sta $3e
                    lda #$ac
                    sta $3f
                    jsr $dcbc
                    lda #$00
                    sta $37
                    lda #$01
                    jsr Saa3b
                    lda $66
                    jsr Laa26
                    jsr Sa7ef
                    jsr Sb780
                    lda #$01
                    sta $37
                    rts
                    
Sa7d1               lda $51
                    asl a
                    tax
                    lda $8300,x
                    sta $10
                    lda $8301,x
                    sta $11
                    ldy #$04
					
La7e1               lda ($10),y
                    sta $0300,y
                    dey
                    bpl La7e1
                    lda #$00
                    sta $0305
                    rts


DrawShopGoldBox               lda #$04
                    jsr DrawShopBox
                    jsr PrintGold
                    jsr DrawShopComplexString
                    lda $3a
                    clc
                    adc #$06
                    sta $3a
                    lda #$08
                    jmp DrawShopString
					
					
ShopLoop_YesNo               lda #$03
                    jsr DrawShopBox
                    lda #$0f
                    jsr DrawShopString
                    lda #$02
                    sta $63
                    jmp CommonShopLoop_Cmd
					
					
CommonShopLoop_Cmd               lda #$77
                    sta $3e
                    lda #$a9
                    sta $3f
                    jmp La91a
                    
La912               lda #$7f
                    sta $3e
                    lda #$a9
                    sta $3f
La91a               lda #$00
                    sta cursor
                    lda $20
                    and #$0c
                    sta $61
La924               lda cursor
                    asl a
                    tay
La928               lda ($3e),y
                    sta $64
                    iny
                    lda ($3e),y
                    sta $65
                    jsr ShopFrame
                    lda $25
                    bne La96a
                    lda $24
                    bne La974
                    lda $20
                    and #$0c
                    cmp $61
                    beq La924
                    sta $61
                    cmp #$00
                    beq La924
                    cmp #$08
                    bne La95a
                    dec cursor
                    bpl La924
                    lda $63
                    sec
                    sbc #$01
                    jmp La965
                    
La95a               lda cursor
                    clc
                    adc #$01
                    cmp $63
                    bcc La965
                    lda #$00
La965               sta cursor
                    jmp La924
                    
La96a               sec
La96b               lda #$00
                    sta $24
                    sta $25
                    sta $22
                    rts
                    
La974               clc
                    bcc La96b
                   					
					
					
ShopSelectBuyMagic:
LDA #0
STA $63     
LDY #0             			

La98f               ldx $63
                    lda $0300,x
                    beq La9cf
                    sta $0311,y
                    sta $0316,y
                    lda #$02
                    sta $0310,y
                    lda #$03
                    sta $0315,y
                    lda #$01
                    sta $0312,y
                    sta $0317,y
                    lda #$c6
                    sta $0313,y
                    lda $0311,y
                    sec
                    sbc #$b0
                    lsr a
                    lsr a
                    lsr a
                    sec
                    adc #$80
                    sta $0314,y
                    tya
                    clc
                    adc #$08
                    tay
                    inc $63
                    lda $63
                    cmp #$05
                    bcc La98f
La9cf               lda #$00
                    sta $0310,y
                    lda #$02
                    jsr DrawShopBox
                    lda #$10
                    sta $3e
                    lda #$03
                    sta $3f
                    jsr DrawShopComplexString
                    lda #$03
                    jsr LoadShopBoxDims
                    jsr $e146
                    jmp La912
                    
DrawShopCursor               lda $64
                    sta $40
                    lda $65
                    sta $41
                    jmp $ec95
                    
DrawShopPartySprites               lda #$98
                    sta $40
                    lda #$38
                    sta $41
                    lda #$40
                    jsr $ebfd
                    lda #$50
                    sta $41
                    lda #$80
                    jsr $ebfd
                    lda #$68
                    sta $41
                    lda #$c0
                    jsr $ebfd
                    lda #$50
                    sta $41
                    lda #$88
                    sta $40
                    lda #$00
                    jmp $ebfd
                    
DrawShopString               asl a
                    tax
                    lda $8000,x
                    sta $3e
                    lda $8001,x
                    sta $3f
DrawShopComplexString               ldx #$0e
                    stx $57
                    stx $58
                    jmp $de36
                    
DrawShopBox               jsr LoadShopBoxDims
                    jmp $e063
                    
LoadShopBoxDims               tax
                    lda $ac40,x
                    sta $38
                    lda $ac45,x
                    sta $39
                    lda $ac4a,x
                    sta $3c
                    lda $ac4f,x
                    sta $3d
                    lda #$0e
                    sta $57
                    rts
                    
DrawShopDialogueBox               pha
                    lda #$00
                    jsr DrawShopBox
                    pla
                    jmp DrawShopString					
					
					
DrawInnClinicConfirm               lda #$0e
                    jsr DrawShopDialogueBox
                    lda $0300
                    sta $10
                    lda $0301
                    sta $11
                    lda #$00
                    sta $12
                    jsr PrintNumber_5Digit
                    jmp DrawShopComplexString
					
					
					
PlaySFX_MenuSel               lda #$ba
                    sta $4004
                    lda #$ba
                    sta $4005
                    lda #$40
                    sta $4006
                    lda #$00
                    sta $4007
                    lda #$1f
                    sta $7e
                    rts
                    
PlaySFX_MenuMove               lda #$7a
                    sta $4004
                    lda #$9b
                    sta $4005
                    lda #$20
                    sta $4006
                    lsr a
                    sta $4007
                    sta $7e
                    rts		
					
					
Sb780               jsr $c43c
                    jsr $fe00
                    lda #$02
                    sta $4014
                    jsr $d850
                    lda #$08
                    sta $ff
                    sta $2000
                    lda #$1e
                    sta $2001
                    lda #$00
                    sta $2005
                    sta $2005
                    lda #$0e
                    sta $57
                    jmp $c689		
					
					
					
					
					
