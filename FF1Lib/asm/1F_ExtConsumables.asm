SwapPRG = $FE1A
DrawDrinkBox = $A020

;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;
;;
;;  DrawDrinkBox  [$F921 :: 0x3F931]
;;
;;    Breaks out of 1F into 1C and draws a DrinkBattleBox
;;
;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;

DrawDrinkBox_L:
	LDA #$1C
	JSR SwapPRG
	JSR DrawDrinkBox
	LDA #$0C
	JSR SwapPRG
	RTS
