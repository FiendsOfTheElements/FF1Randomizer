 .ORG $E231
 BEQ InShipCanoe ; $F03B

 .ORG $E26A

vehicle = $42
DrawPlayerMapmanSprite = $E281
DrawOWObj_BridgeCanal = $E3A6
DrawOWObj_Ship = $E373
DrawOWObj_Airship = $E38C

InShipCanoe:                       
  JSR DrawOWObj_BridgeCanal    
  LDY vehicle
  JSR DrawPlayerMapmanSprite   
  LDY vehicle
  CPY #$04
  BEQ InCanoe
  JSR DrawOWObj_Ship
InCanoe:                      
  JMP DrawOWObj_Airship
