; replaced code in drawitembox
LDA #$0F
JSR $FE1A ; swap to bank 0F
JSR $8D00
NOP
NOP
NOP



;Inventory Re-Sort routine start
cursor = $62
cursor_max = $63
item_box = $0300
item_heal = $6039
items_start = $6020
potions_start = $6039
lodgings_start = $6036
orbs_start = $6031
shards_addr = $6035
items_stop = $603C


.org $8D00

;moved code from DrawItemBox, 11 bytes
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

LDX #$1C
ExtConsumablesLoop:
LDA items_start, X
BEQ SkipExtConsumableDest ;skip if qty = 0
TXA
STA item_box, Y ;positive quantity, store in list
INY
SkipExtConsumableDest:
INX
CPX #$20 ;see if we've run through the extconsumables
BNE ExtConsumablesLoop
;done ext consumables

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

;INY
;LDA shards_addr
;BEQ EndList
;STA item_box, Y

EndList:
RTS
