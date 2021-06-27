
LoadBatSprCHRPalettes_NewGame = $EAB9
cur_pal = $03C0

.org DDE0

JSR LoadBatSprCHRPalettes_NewGame
LDA #$03
STA STA cur_pal+$10
RTS

