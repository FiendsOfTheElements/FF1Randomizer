;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;
;;
;;  Enter Magic Shop  [$A357 :: 0x3A357]
;;
;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;

// QoS to make magic shop function like weapon/armor shops

[$A360 :: 0x3A360]
EnterShop_Magic:
  LDA #$0D
  JSR DrawShopDialogueBox
MagicShop_Loop:
  JSR ShopSelectBuyMagic                    ; Selecting magic placed first like weapon and armor shop
  BCS MagicShop_Exit
  LDX $62
  LDA $0300,X
  STA $030C
  JSR DrawShopBuyItemConfirm
  JSR ShopLoop_YesNo
  BCS MagicShop_CancelPurchase
  LDA $62
  BNE MagicShop_CancelPurchase
  JSR Shop_CanAfford
  BCC $A38B
  LDA #$10
  JSR DrawShopDialogueBox
  JMP MagicShop_Loop
  JMP FinalizeOutput
  LDA $62
  ROR A
  ROR A
  ROR A
  AND #$C0
  STA $030A
  JSR MagicShop_AssertLearn
  JSR ShopPayPrice
  LDX $030A
  LDA $030B
  STA $6300,X
  JMP EnterShop_Magic

[$94E0 :: 0x394E0]
FinalizeOutput:
  LDA #$17
  JSR DrawShopDialogueBox
  JSR ShopLoop_CharNames
  BCS $94ED
  JMP $A38E
  JMP MagicShop_Loop