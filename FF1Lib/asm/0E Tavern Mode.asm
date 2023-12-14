DSBox = $aa3b
DSComplexString = $aa32
CommonShopLoop_Cmd = $a907
DSDialogueBox = $AA5B
cur_map = $48
cursor = $62
DInnClinicConfirm = $AA9B
ShopLoop_YesNo = $a8c2
InnClinic_CanAfford = $A689
DoPartyGen_OnCharacter-jumps-into-routine = $9D1C
RecordClassAndName = $9CE9
CheckGameEventFlag = $9079
DoLevelUp = $9D34
LoadMenuCHRPal = $E90C
DrawShop = $A778
DSDialogueBox = $AA5B
ShopFrameNoCursor = $A743
TavernModeClassLut = $9d0a
HireA__Text = $9db0
StringBuff+10 = $0310
Clinic_InitialText = $9D58
BuyOrRevive_Routine = $9d1C
CBuildNameString  = $A6ED
CSelectTarget = $a6d7
joy_a = $24
joy_b = $25

* = $9d12
;Revive Hire text options
9BA8B9AC320191AC2300  ; "Revive\nHire"


* = $9d1C

; Draws the text box with the Revive and Hire options, then jumps into the command loop
BuyOrRevive_Routine lda #$03
                    jsr DrawShopBox
                    lda #$12
                    sta $3e
                    lda #$9d
                    sta $3f
                    jsr DrawShopComplexString
                    lda #$02
                    sta $0062
                    jmp CommonShopLoop_Cmd


* = $9D34
DoLevelUp	    
                    LDA $030D   ; check if we're reviving or hiring
                    BEQ :+      ; branch if reviving (no need to do levelup routine)
                    LDA #$9D
                    PHA
                    LDA #$50
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
                    : JMP TurnOffPPU
                    .END

* = $9800           ; Address could already be in use, so check here first in case of crash; update: it was!
TurnOffPPU:
                    LDA #$00
                    STA $0024
                    STA $0025
                    STA $2001
                    RTS

* = $9D58



Clinic_InitialText  jsr DSDialogueBox
                    ldy #$ff
TextLoop            iny
                    lda HireA__Text,y
                    sta StringBuff+10,y
                    bne TextLoop
                    lda #$02
                    sta StringBuff+10,y
                    iny
                    ldx *cur_map
                    lda TavernModeClassLut,x
                    adc #$f0
                    sta StringBuff+10,y
                    iny
                    lda #$05
                    sta StringBuff+10,y
                    iny
                    lda #$c5
                    sta StringBuff+10,y
                    iny
                    lda #$00
                    sta StringBuff+10,y
                    lda #$10
                    sta $3e
                    lda #$03
                    sta $3f
                    jmp DSComplexString




* = $9d95
BuyOrRevive_Routine
                    lda $030d
                    bne hire

                    hire lda ch_class,X
                    cmp #$FF
                    bne notNone
                    lda #$01
                    rts
                    notNone 

                    lda ch_ailments,X
                    cmp #$01
                    rts

                    hire lda ch_class,X
                    cmp #$FF
                    rts


;"Hire a\n" text



                    * = $a5a1




CShop_Exit               rts
                    
EnterShop_Clinic               lda #$00
                    sta *$24
                    sta *$25

                    jsr Clinic_InitialText
                    lda #$02
                    sta *$63

                    clc
                    jsr BuyOrRevive_Routine
                    bcs CShop_Exit
                    lda cursor
                    sta $030d

                    JSR CBuildNameString  
                    BCS SomeoneInList
                    jmp NobodyInList 


     SomeoneInList  jsr CSelectTarget
                    bcs EnterShop_Clinic
                    lda cursor
                    sta $030c

                    jsr DInnClinicConfirm
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
                    tax


                    lda $030d

                    beq ClassChangeSkip
                    txa
                    adc #$0a
                    sta *$10
                    lda #$61
                    sta *$11
                    txa
                    pha
                    sta *$12
                    lda #$63
                    sta *$13
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
                    jsr RecordClassAndName
                    ldy *cur_map
                    lda $9d0a,y
                    sta $6100,x
                    jsr $c271
                    ldy #$0e
                    jsr CheckGameEventFlag
                    pla
                    tax
                    bcc ClassChangeSkip
                    clc
                    lda $6100,x
                    adc #$06
                    sta $6100,x



ClassChangeSkip               lda #$01
                    sta $610a,x
                    lda #$00
                    sta $6101,x
		    jsr DoLevelUp
                    jsr LoadMenuCHRPal
                    jsr DrawShop
                    lda #$21
                    jsr DSDialogueBox

RetryLoop           jsr ShopFrameNoCursor
                    lda *$24
                    ora *$25
                    beq RetryLoop
                    jmp EnterShop_Clinic


ExitLoop            jsr ShopFrameNoCursor
                    lda *$24
                    ora *$25
                    beq ExitLoop
                    jmp CShop_exit

NobodyInList        LDA #$23
                    JSR DSDialogueBox   

                    LDA #0
                    STA joy_a
                    STA joy_b  
                    jmp ExitLoop              



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
                    

* = $a6d7



CSelectTarget               lda #$03
                    jsr DSBox
                    jsr CBuildNameString
                    lda #$10
                    sta *$3e
                    lda #$03
                    sta *$3f
                    jsr DSComplexString
                    jmp CommonShopLoop_Cmd





CBuildNameString
Sa6ed               ldy #$00
                    ldx #$00
                    stx *$63
La6f3     
                    jsr ShouldSkipChar          

                    BNE SkipChar

                    txa
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
                    inc *$63

               SkipChar:  txa
                    clc
                    adc #$40
                    tax
                    bne La6f3
                    lda #$00
                    sta $0310,y
                    lda *$63
                    cmp #$01
                    rts

