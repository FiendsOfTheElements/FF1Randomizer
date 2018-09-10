using FF1Lib.Assembly;
using System;
using System.Diagnostics;

namespace FF1Lib
{
	partial class FF1Rom
	{

		public void DemoPatchAssembler()
		{

			string src = @"
; EnterMainMenu:      ;; commented so it doesn't complain about redefined labels
    LDA #$51
    STA music_track     ; set music track $51 (menu music)
    LDA #0
    STA $2001           ; turn off the PPU (we need to do some drawing)     
    STA $4015           ; and silence the APU.  Music sill start next time MusicPlay is called.
    JSR LoadMenuCHRPal        ; load menu related CHR and palettes
    LDX #$0B
  @Loop:                      ; load a few other main menu related palettes
      LDA lutMenuPalettes, X  ; fetch the palette from the LUT
      STA cur_pal, X          ; and write it to the palette buffer
      DEX
      BPL @Loop               ; loop until X wraps ($0C colors copied)
";
			BA bank_and_address = Symbols.Labels.EnterMainMenu;
			string human_readable_name = "EnterMainMenu duplicate";
			byte[] result = Assembler.Assemble(bank_and_address, human_readable_name, src);

			// this is all you have to do. The rest of the code below is just printing hex and comparing.



			string result_hex = BitConverter.ToString(result).Replace("-", string.Empty);

			Console.WriteLine("result:");
			Console.WriteLine(result_hex);

			var rom_address = bank_and_address.ToRomLocation();
			var size_guess = Symbols.Calculate.SpaceToNextLabel("EnterMainMenu");
			byte[] from_actual_rom = Get(rom_address, size_guess);
			string rom_hex = BitConverter.ToString(from_actual_rom).Replace("-", string.Empty);
			Console.WriteLine("from rom:");
			Console.WriteLine(rom_hex);

			if (rom_hex == result_hex)
				Console.WriteLine("MATCH");
			else
				Console.WriteLine("mismatch");

		}
	}
}
