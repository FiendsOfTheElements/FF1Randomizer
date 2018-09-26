define BANK_TELEPORTINFO $0;    

define lut_EntrTele_X $AC00
define lut_EntrTele_Y $AC20
define lut_EntrTele_Map $AC40
define lut_NormTele_X $AD00
define lut_NormTele_Y $AD40
define lut_NormTele_Map $AD80
define lut_Tilesets $ACC0

define sm_scroll_x $29
define sm_scroll_y $2A
define inroom $0D
define tileprop1 $45;
define cur_map $48
define cur_tileset $49

define SwapPRG $FE03;

; ROOM TO ROOM
; This is the Room to Room Teleport patch, it shares a lot of variables with the Overworld to Room patch
; This patch must be exactly 38 bytes as the code carries on right after
LDA #BANK_TELEPORTINFO
JSR SwapPRG;

LDX tileprop1          ; get the teleport ID in X 

LDA lut_NormTele_X, X   ; get the X coord
STA $10

LDA lut_NormTele_Y, X   ; do same with Y coord
STA $11

LDA lut_NormTele_Map, X ; get the map and record it
STA cur_map

TAX                     ; then throw the map in X, 
LDA lut_Tilesets, X     ; the tileset for this map
STA cur_tileset

LDA #$0F
JSR SwapPRG;

JSR $9200
NOP
NOP

; OVERWORLD VERSION
; This version must be 45 bytes

LDA #BANK_TELEPORTINFO
JSR SwapPRG;
LDA tileprop1           ; get the teleport ID
AND #$3F                ;  remove the teleport/battle bits, leaving just the teleport ID
TAX                     ;  put the ID in X for indexing

LDA lut_EntrTele_X, X   ; get the X coord, and subtract 7 from it to get the scroll
STA $10

LDA lut_EntrTele_Y, X   ; do same with Y coord
STA $11

LDA lut_EntrTele_Map, X ; get the map
STA cur_map

TAX                     ; throw map in X
LDA lut_Tilesets, X     ; and use it to get the tileset for this map
STA cur_tileset

LDA #$0F
JSR SwapPRG;

JSR $9200
NOP
NOP
NOP
NOP
NOP
NOP

; Finally the code for Bank 0F and 0x9200 to read those tmp x and y, extract the in room bit, and write it.
LDX #0    ; Put 0 in temporary inroom placeholder

LDA $10
ASL
BCC NotInRoom      
	LDX #$81        ; Set temporary inroom from X high bit ($81 means in room)

NotInRoom:
LSR                 ; Shift back, leaving high bit 0
SEC
SBC #$07
AND #$3F
STA sm_scroll_x

LDA $11             ; Y is a flag to use X
ASL
BCC NoFlag
	STX inroom      ; Write temporary if high bit set

NoFlag:
LSR                 ; Similar shift back leaving high bit 0
SEC
SBC #$07
AND #$3F
STA sm_scroll_y
RTS
