using System;
using System.Collections.Generic;
using System.Text;
using Patcher6502;

namespace FF1Lib.Assembly
{
	/// <summary>
	/// Assemble 6502 asm with FF1's symbols.
	/// </summary>
	public static class Assembler
	{

		public static byte[] Assemble(BA origin, string name, string src)
		{
			PatchAssembler assembler = new PatchAssembler();

			return assembler.Assemble(origin.Addr, name, src, variables: Symbols.AsDictionaries.VariablesAndConstants, labels: Symbols.AsDictionaries.LabelsWithRunAddresses);
		}

	}
}
