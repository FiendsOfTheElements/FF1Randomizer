using Patcher6502;
using System;
using System.Diagnostics;

namespace FF1Lib
{
	partial class FF1Rom
	{
		public void DemoPatchAssembler()
		{
			string src = @"
; *victory music*

GoodStuff:
LDA #$FF
ADC #1
SBC #1
SEC
BCS Forever

Forever: BCS GoodStuff

NOP ; EA sports

JMP GoodStuff ; just to demo that the address is actually 8000
";
			byte[] result = PatchAssembler.Assemble(0x8000, "test code", src);

			string hex = BitConverter.ToString(result).Replace("-", string.Empty);

			Debug.Assert(hex == "A9FF6901E90138B000B0F5EA4C0080");

			// alternatively, if you wanted to make extra sure your code was the right size, you could call
			byte[] the_same_result = PatchAssembler.AssembleAndAssertSize(0x8000, 15, "test code again", src);

			Debug.Assert(result == the_same_result);
			Debug.Assert(result.Length == 15);

			Console.WriteLine("result:");
			Console.WriteLine(hex);
			Console.WriteLine("--- result finished ---");
			Console.WriteLine("Press enter to close...");
			Console.ReadLine();
		}
	}
}
