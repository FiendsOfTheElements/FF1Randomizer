using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using DotNetAsm;

namespace Inline6502
{
	public class AssemblyError : Exception
	{
		public AssemblyError(string message) : base(message)
		{
		}
	}

	public static class InlineAssembler
	{
		private class InlineController : AssemblyController
		{
			private string _raw_input;
			private string _reference_name;

			public InlineController() : base(new [] {""})
			{
			}


			public byte[] inlineAssemble(string name, string asm)
			{
				_reference_name = name;
				_raw_input = asm;

				var source = Preprocess();
				if (Log.HasErrors)
					return null;

				FirstPass(source);
				if (Log.HasErrors)
					return null;

				SecondPass();

				if (Log.HasErrors)
					return null;


				#if DEBUG
				Console.WriteLine($"DEBUG -- listing for {name}:");
				Console.WriteLine(GetListing());
				Console.WriteLine($"DEBUG -- listing complete for {name}:");
				#endif

				return Output.GetCompilation().ToArray();
			}

			private IEnumerable<SourceLine> GetSource()
			{
				int currentline = 1;
				IList<SourceLine> sourcelines = new List<SourceLine>();
				foreach (string unprocessedline in _raw_input.Replace("\r","").Split('\n'))
				{
					try
					{
						var line = new SourceLine(_reference_name, currentline, unprocessedline);
						line.Parse(
							token => Controller.IsInstruction(token) ||
							         Reserved.IsReserved(token) ||
							         (token.StartsWith(".") && Macro.IsValidMacroName(token.Substring(1))) ||
							         token == "="
						);
						sourcelines.Add(line);
					}
					catch (Exception ex)
					{
						Controller.Log.LogEntry(_reference_name, currentline, ex.Message);
					}
					currentline++;
				}

				sourcelines = _preprocessor.Preprocess(sourcelines).ToList();
				return sourcelines;
			}

			protected override IEnumerable<SourceLine> Preprocess()
			{
				var source = new List<SourceLine>();

				source.AddRange(ProcessDefinedLabels());
				source.AddRange(GetSource());

				if (Log.HasErrors)
					return null;

				// weird hack in AssemblyController, copying here to be consistent
				source.ForEach(line =>
					line.Operand = Regex.Replace(line.Operand, @"\s?\*\s?", "*"));

				return source;
			}

		}

		/// <summary>
		/// Assembles the 6502 asm provided into raw bytes.
		/// </summary>
		/// <param name="origin">The address this code will run at. e.g. 0x8000. Must be between 0 and 0xFFFF.</param>
		/// <param name="name">A human-readable reference, e.g. a subroutine name.</param>
		/// <param name="asm">the 6502 asm source, including newline characters.</param>
		/// <example>
		/// var newBytes = InlineAssembler.assemble(0x8000, "FixTheThing", @"
		///   LDA $30
		///   JMP $8044
		///  ");
		/// </example>
		/// <exception cref="AssemblyError">if something goes wrong.</exception>
		public static byte[] assemble(int origin, string name, string asm)
		{
			if (origin < 0 || origin > 0xFFFF)
				throw new ArgumentOutOfRangeException(nameof(origin));

			var controller = new InlineController();
			controller.AddAssembler(new Asm6502(controller));

			string set_origin_string = $"* = ${origin:X4}\n";

			var result = controller.inlineAssemble(name, set_origin_string + asm);

			#if DEBUG
			controller.Log.DumpAll();
			#else
			if (controller.Log.HasErrors)
				controller.Log.DumpErrors();
			#endif

			if (result is null)
				throw new AssemblyError($"Unable to assemble {name} at {origin}; see console output for details");

			return result;
		}

		/// <summary>
		/// Does the same thing as assemble(), but asserts that the resulting size is what you expect.
		/// Intended to catch accidental patch-too-big type errors.
		///
		/// NOTE: The assert will not check anything in Release builds. This is a designed feature of Debug.Assert.
		/// </summary>
		/// <param name="size">Asserted size of the resulting assembled code in bytes.</param>
		public static byte[] assembleAndAssertSize(int origin, int size, string name, string asm)
		{
			var result = assemble(origin, name, asm);
			Debug.Assert(result.Length == size);
			return result;
		}

	}
}
