
LoadMenuCHRPal = $E906
cur_pal = $03C0

.org A0D0

JSR LoadMenuCHRPal
LDA #$03
STA STA cur_pal+$10
RTS

