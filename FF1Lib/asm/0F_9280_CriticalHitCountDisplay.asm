; Displays critical hit messages that actually display number of
; crits rather than just displaying "Critical hit!!"
; There's probably a more efficient way to do this, like I guess if I
; just defined all the buffer locations myself, but it's not *that* much
; more efficient and this is probably easier to read.
; We have plenty of space in the banks anyway; you're not my real dad.

define battle_critsconnected $686B
define btl_combatboxcount $6AF8
define DrawCombatBox $F71C
define btl_unfmtcbtbox_buffer_4 $6B3A

LDX #02 ; Start at 02 so we only load battle_critsconnected into A once
LDA #$00 ; This is actually the second half of the number for battle_critsconnected,
STA btl_unfmtcbtbox_buffer_4, X ; but it's impossible to get that many crits so we're just making it 0

INX
LDA #$FF ; empty space
STA btl_unfmtcbtbox_buffer_4, X

INX
LDA #$A6 ; "c"
STA btl_unfmtcbtbox_buffer_4, X

INX
LDA #$B5 ; "r"
STA btl_unfmtcbtbox_buffer_4, X

INX
LDA #$AC ; "i"
STA btl_unfmtcbtbox_buffer_4, X

INX
LDA #$B7 ; "t"
STA btl_unfmtcbtbox_buffer_4, X

INX
LDA #$B6 ; "s"
STA btl_unfmtcbtbox_buffer_4, X
	
INX
LDA #$C4 ; "!"
STA btl_unfmtcbtbox_buffer_4, X

INX
LDA #$00 ; null terminator
STA btl_unfmtcbtbox_buffer_4, X

LDX #00
LDA #$11 ; Control code to write a number
STA btl_unfmtcbtbox_buffer_4, X

INX
LDA battle_critsconnected ; Write the crit count.
STA btl_unfmtcbtbox_buffer_4, X

CMP #01
BNE ActuallyDrawBox

LDX #08
LDA #$C4 ; "!"
STA btl_unfmtcbtbox_buffer_4, X

INX
LDA #$00 ; null terminator
STA btl_unfmtcbtbox_buffer_4, X

INX
LDA #$FF ; blank space - if you don't do this the game crashes in non-hilarious fashion
STA btl_unfmtcbtbox_buffer_4, X

ActuallyDrawBox:
LDX #$3A ; store the low byte in X
LDY #$6B ; and the high byte in Y...

LDA #$04 ; box 04 is the combat box normally used for displaying messages like Terminated, etc
JSR DrawCombatBox ; DRAW BOX
INC btl_combatboxcount ; increment the box counter the same way the normal draw simple message function does
RTS ; cya nerds