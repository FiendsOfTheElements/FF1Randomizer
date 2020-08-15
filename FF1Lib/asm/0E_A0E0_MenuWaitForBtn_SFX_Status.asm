; New MenuWaitForBtn_SFX for the Status screen
;  so we can show the info box when pressing A or B
;  the code is in bank 1E, so we put the return address
;  on the stack (StatusWaitForBtn_SFX) so we go there
;  after switching bank instead of trying to come back here.

cur_bank        = $57
ret_bank        = $58

SwapPRG_L = $FE03              ; Bank $1F

StatusWaitForBtn_SFX = $8910   ; Bank $1E

 .ORG $A0E0

MenuWaitForBtn_SFX_Status:
  LDA #>StatusWaitForBtn_SFX    ; Put RTS address on the stack
  PHA                           ;  because we're switching bank
  LDA #<StatusWaitForBtn_SFX-1  
  PHA
  LDA #$1E
  STA cur_bank     
  JMP SwapPRG_L
