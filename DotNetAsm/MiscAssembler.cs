//-----------------------------------------------------------------------------
// Copyright (c) 2017, 2018 informedcitizenry <informedcitizenry@gmail.com>
//
// Licensed under the MIT license. See LICENSE_6502net for full license information.
//
//-----------------------------------------------------------------------------

using System;
using System.Linq;

namespace DotNetAsm
{
    /// <summary>
    /// A class that assembles miscellaneous directives, such as error and warn messages.
    /// </summary>
    public sealed class MiscAssembler : AssemblerBase, ILineAssembler
    {
        #region Constructors

        /// <summary>
        /// Constructs a DotNetAsm.MiscAssembler class.
        /// </summary>
        /// <param name="controller">The DotNetAsm.IAssemblyController to associate</param>
        public MiscAssembler(IAssemblyController controller) :
            base(controller)
        {
            Reserved.DefineType("Directives",
                    "assert", ".eor", ".echo", ".target",
                    ".error", ".errorif",
                    ".warnif", ".warn"
                );
        }

        #endregion

        #region Methods

        /// <summary>
        /// Throw a conditional error or warning.
        /// </summary>
        /// <param name="line">The <see cref="T:DotNetAsm.SourceLine"/> with the operand condition.</param>
        void ThrowConditional(SourceLine line)
        {
            var csv = line.Operand.CommaSeparate();
            if (csv.Count < 2)
                Controller.Log.LogEntry(line, ErrorStrings.TooFewArguments, line.Instruction);
            else if (csv.Count > 2)
                Controller.Log.LogEntry(line, ErrorStrings.TooManyArguments, line.Instruction);
            else if (Controller.Evaluator.EvalCondition(csv.First()))
                Output(line, csv.Last());
        }

        /// <summary>
        /// Sets the byte value to XOR all values when outputted to assembly.
        /// Used by the .eor directive.
        /// </summary>
        /// <param name="line">The <see cref="T:DotNetAsm.SourceLine"/>.</param>
        void SetEor(SourceLine line)
        {
            if (string.IsNullOrEmpty(line.Operand))
            {
                Controller.Log.LogEntry(line, ErrorStrings.TooFewArguments, line.Instruction);
                return;
            }
            var eor = Controller.Evaluator.Eval(line.Operand, sbyte.MinValue, byte.MaxValue);
            var eor_b = Convert.ToByte(eor);
            Controller.Output.Transforms.Push(delegate(byte b)
            {
                b ^= eor_b;
                return b;
            });
        }

        public void AssembleLine(SourceLine line)
        {
            string instruction = Controller.Options.CaseSensitive ? line.Instruction : line.Instruction.ToLower();
            switch (instruction)
            {
                case ".assert":
                    DoAssert(line);
                    break;
                case ".warnif":
                case ".errorif":
                    ThrowConditional(line);
                    break;
                case ".echo":
                case ".error":
                case ".warn":
                    Output(line, line.Operand);
                    break;
                case ".eor":
                    SetEor(line);
                    break;
                case ".target":
                    if (!line.Operand.EnclosedInQuotes())
                        Controller.Log.LogEntry(line, ErrorStrings.QuoteStringNotEnclosed);
                    else
                        Controller.Options.Architecture = line.Operand.TrimOnce('"');
                    break;
                default:
                    Controller.Log.LogEntry(line, ErrorStrings.UnknownInstruction, line.Instruction);
                    break;
            }
        }

        /// <summary>
        /// Output the specified line and operand either to console output or to the error log.
        /// </summary>
        /// <param name="line">The <see cref="T:DotNetAsm.SourceLine"/>.</param>
        /// <param name="operand">The operand to output..</param>
        void Output(SourceLine line, string operand)
        {
            if (!operand.EnclosedInQuotes())
            {
                operand = StringAssemblerBase.GetFormattedString(line.Operand, Controller.Evaluator);
                if (string.IsNullOrEmpty(operand))
                    Controller.Log.LogEntry(line, ErrorStrings.QuoteStringNotEnclosed);
            }
            else
            {
                operand = operand.TrimOnce('"');
            }
            var type = line.Instruction.Substring(0, 5).ToLower();
            switch (type)
            {
                case ".echo":
                    Console.WriteLine(operand);
                    break;
                case ".warn":
                    Controller.Log.LogEntry(line, operand, Controller.Options.WarningsAsErrors);
                    break;
                default:
                    Controller.Log.LogEntry(line, operand);
                    break;
            }
        }

        /// <summary>
        /// Dos the assert.
        /// </summary>
        /// <param name="line">The <see cref="T:DotNetAsm.SourceLine"/>.</param>
        void DoAssert(SourceLine line)
        {
            var parms = line.Operand.CommaSeparate();
            if (parms.Count == 0)
            {
                Controller.Log.LogEntry(line, ErrorStrings.TooFewArguments, line.Instruction);
            }
            else if (parms.Count > 2)
            {
                Controller.Log.LogEntry(line, ErrorStrings.TooManyArguments, line.Instruction);
            }
            else if (!Controller.Evaluator.EvalCondition(parms.First()))
            {
                if (parms.Count > 1)
                    Output(line, parms.Last());
                else
                    Controller.Log.LogEntry(line, ErrorStrings.AssertionFailure, line.Operand);
            }
        }

        public int GetInstructionSize(SourceLine line) => 0;

        public bool AssemblesInstruction(string instruction) => Reserved.IsReserved(instruction);

        #endregion
    }
}
