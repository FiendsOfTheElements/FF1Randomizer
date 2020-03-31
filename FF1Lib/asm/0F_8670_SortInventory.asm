;replaced code in DrawItemBox, 11 bytes:
LDA #$0F
JSR $FE1A ; swap to bank 0F
JSR $8D10
NOP
NOP
NOP



;Inventory Re-Sort routine start
define cursor $62
define cursor_max $63
define item_box $0300
define item_heal $6039
define items_start $6020
define potions_start $6039
define lodgings_start $6036
define orbs_start $6031
define shards_addr $6035
define items_stop $603C

;moved code from DrawItemBox, 11 bytes
define cursor $62
define cursor_max $63
STX cursor_max     ; the number of items they have becomes the cursor maximum, $EF58
LDA #0
STA item_box, X    ; put a null terminator at the end of the item box (needed for following loop)
LDA #0
STA cursor         ; also reset the cursor to 0 (which will be used as a loop counter below)

;;;;;Begin sorting
CLC
LDY #$0 ;item_box index
LDX #$19 ;items offset for potions start

PotionsLoop:
LDA items_start, X
BEQ SkipPotionDest ;skip if qty = 0
TXA
STA item_box, Y ;positive quantity, store in list
INY
SkipPotionDest:
INX
CPX #$1C ;see if we've run through the potions
BNE PotionsLoop
;done potions
LDX #$16 ;items offset for lodgings start

LodgingsLoop:
LDA items_start, X
BEQ SkipLodgingDest ;skip if qty = 0
TXA
STA item_box, Y ;positive quantity, store in list
INY
SkipLodgingDest:
INX
CPX #$19 ;see if we've run through the lodgings
BNE LodgingsLoop
;done lodgings
LDX #$0

UniqueItemsLoop:
LDA items_start, X
BEQ SkipUniqueDest ;skip if qty = 0
TXA
STA item_box, Y ;positive quantity, store in list
INY
SkipUniqueDest:
INX
CPX #$11 ;see if we've run through the unique items
BNE UniqueItemsLoop

EndList:
RTS
