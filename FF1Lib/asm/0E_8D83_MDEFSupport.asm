PrintNumber = $8DF8
.org $8D83  ;bank 0E, replaces the part that check for 7 and branches, then checks for 8 and branches, etc, etc
			       ;we can instead subtract 9 from all the regular stats to give the correct offset into ch_stats
				   ;reducing the amount of code needed and making room to check for the new MDEF code
	  CMP #$50         ;we're using stat $50 to indicate MDEF
	  BNE NotMDEF
	  SBC #$2B         ;subtracting $2B gives us the correct offset to ch_stats for MDEF
	  BNE PrintNumber  
NotMDEF:
	  CMP #$0D         ;Check if a different regular stat is being drawn
	  BPL Continue     ;If not, continue checking other codes
	  ADC #$09         ;Adding 9 to other stats gives the correct offset into ch_stats
	  BNE PrintNumber
	  NOP
	  NOP
	  NOP
	  NOP
Continue:
