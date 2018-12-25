btl_charattrib = $6BA8
btl8x8spr_a = $68B3
longjump = $C8A4


JSR longjump				; execute a longjump to bank 0F $8BD0
CMP #$FF					; if class FF, jsr sets A to FF, otherwise it is the character ID (0,1,2, or 3)
BNE CONTINUE
RTS							; dont draw if class is FF
CONTINUE: TXA
ASL A
ASL A
TAY 

							; THE FOLLOWING IS IN BANK 0F


longjump:					; Located in bank 0F $8BD0
TAY
ROR A
ROR A
ROR A
TAX
LDA $6100,X
CMP #$FF
BEQ SKIP
 
TYA
TAX
LDA btl_charattrib,X
STA btl8x8spr_a
SKIP: RTS
