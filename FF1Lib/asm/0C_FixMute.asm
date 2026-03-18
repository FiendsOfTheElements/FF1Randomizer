;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;
;;
;;  Battle_PrepareMagic  [$B35C :: 0x3336C]
;;
;;  Replaced the manual ptr calculations with call to GetPointerToMagicData
;;
;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;

[$B363 :: 0x33363]
  JSR GetPointerToMagicData
  STA btl_magdataptr_low
  STX btl_magdataptr_high
  NOP
  NOP
  NOP
  NOP
  NOP
  NOP
  NOP
  NOP
  NOP
  NOP
  NOP
  NOP
  NOP
  NOP
  NOP

20119785988699EAEAEAEAEAEAEAEAEAEAEAEAEAEAEA

Shift Player_DoMagic and Player_DoMagicEffect functions up 7 bytes and updating calls to them.
Shift Up:

[0xB3BA :: 0x333BA]
  JMP Player_DoMagicEffect
  NOP
Player_DoMagic:
  STA btl_attackid
  LDA #$00
Player_DoMagicEffect:
  STA btlmag_magicsource

4CC3B3EA8D8C6CA9008D8F6C

[0xB3C6 :: 0x333C6]
  TYA
  AND #$03
  ORA #$80
  STA $6C89
  STA $6BD1

98290309808D896C8DD16B

or

  LDA #$A1
  PHA
  LDA #$BF
  PHA
  LDA #$1C
  JMP SwapPRG

A9A148A9BF48A91C4C03FE

[0xB3D1 :: 0x333D1]
  STX $6C8A
  LDA #$00
  STA $6DA7
  JSR ClearAltMessageBuffer
  JSR DrawCombatBox_Attacker
  JSR DrawCombatBox_Attack
  LDA btl_attacker
  JSR PrepEntityPtr_Player

8E8A6CA9008DA76D200CB0205EB02074B0AD896C20EBB5

Allow Drink And Item While Mute
;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;
;;
;;  Player_DoMagicEffect [$B3CA :: 0x333DA]
;;
;;  Added Skip Mute Check When Drinking And Using An Item
;;
;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;

[0xB3E8 :: 0x333E8]
    LDA btlmag_magicsource  ; loads source      ; 0 = magic, 2 = item and drink
    CMP #$00                ; compares to 0
    BNE $B3FC               ; skips mute check if not magic

AD8F6CC900D00D

[0xA3A3 :: 0x323A3]
  JMP Player_DoMagic

4CBEB3

[0xA1E7 :: 0x721E7]
  LDA #$D0                  ; update bank swap return address

A9D0
