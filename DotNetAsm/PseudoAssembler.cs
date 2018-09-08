//-----------------------------------------------------------------------------
// Copyright (c) 2017, 2018 informedcitizenry <informedcitizenry@gmail.com>
//
// Licensed under the MIT license. See LICENSE_6502net for full license information.
//
//-----------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace DotNetAsm
{
    /// <summary>
    /// An implementation of the <see cref="T:DotNetAsm.ILineAssembler"/> interface that assembles
    /// pseudo operations such as byte and string assembly.
    /// </summary>
    public sealed class PseudoAssembler : StringAssemblerBase, ILineAssembler
    {
        #region Members

        HashSet<BinaryFile> _includedBinaries;
        readonly Dictionary<string, string> _typeDefs;
        Func<string, bool> _reservedSymbol;

        #endregion

        #region Constructors

        /// <summary>
        /// Constructs an instance of a <see cref="T:DotNetAsm.PseudoAssembler"/> line assembler.
        /// </summary>
        /// <param name="controller">The assembly controller</param>
        /// <param name="reservedSymbolFunc">A function callback to determine if the given token
        /// is a symbol name.</param>
        public PseudoAssembler(IAssemblyController controller, Func<string, bool> reservedSymbolFunc) :
            base(controller)
        {
            _includedBinaries = new HashSet<BinaryFile>();

            Reserved.DefineType("PseudoOps",
                    ".addr", ".align", ".binary", ".byte", ".sbyte",
                    ".dint", ".dword", ".fill", ".lint", ".long",
                    ".sint", ".typedef", ".word"
                );
            _typeDefs = new Dictionary<string, string>();
            _reservedSymbol = reservedSymbolFunc;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Assemble multiple values to the output.
        /// </summary>
        /// <param name="line">The <see cref="T:DotNetAsm.SourceLine"/> to assemble.</param>
        void AssembleFills(SourceLine line)
        {
            var csv = line.Operand.CommaSeparate();

            var alignval = (int)Controller.Evaluator.Eval(csv.First(), ushort.MinValue, ushort.MaxValue);

            if (csv.Count > 1 && csv.Last().Equals("?") == false)
            {
                if (csv.Count > 2)
                {
                    Controller.Log.LogEntry(line, ErrorStrings.TooManyArguments, line.Instruction);
                    return;
                }
                var fillval = Controller.Evaluator.Eval(csv.Last(), int.MinValue, uint.MaxValue);

                if (line.Instruction.Equals(".align", Controller.Options.StringComparison))
                    line.Assembly = Controller.Output.Align(alignval, fillval);
                else
                    line.Assembly = Controller.Output.Fill(alignval, fillval);
            }
            else
            {
                if (line.Instruction.ToLower().Equals(".align"))
                    Controller.Output.Align(alignval);
                else
                    Controller.Output.Fill(alignval);
            }
        }

        /// <summary>
        /// Assemble scalar values to the output.
        /// </summary>
        /// <param name="line">The SourceLine to assemble.</param>
        /// <param name="minval">The minimum value based on the type.</param>
        /// <param name="maxval">The maximum value based on the type.</param>
        /// <param name="size">The precise size in bytes of the assembled value.</param>
        void AssembleValues(SourceLine line, long minval, long maxval, int size)
        {
            var tokens = line.Operand.CommaSeparate();
            foreach (var t in tokens)
            {
                if (t == "?")
                {
                    Controller.Output.AddUninitialized(size);
                }
                else
                {
                    var val = Controller.Evaluator.Eval(t, minval, maxval);
                    line.Assembly.AddRange(Controller.Output.Add(val, size));
                }
            }
        }

        /// <summary>
        /// Get the offset and size of the operand of a .binary file
        /// </summary>
        /// <param name="args">The <see cref="T:System.Collections.Generic.List&lt;string&gt;"/> arguments</param>
        /// <param name="binarysize">The size of the binary file</param>
        /// <param name="offs">The offset</param>
        /// <param name="size">The size</param>
        void GetBinaryOffsetSize(List<string> args, int binarysize, ref int offs, ref int size)
        {
            if (args.Count >= 2)
            {
                offs = (int)Controller.Evaluator.Eval(args[1], ushort.MinValue, ushort.MaxValue);
                if (args.Count == 3)
                    size = (int)Controller.Evaluator.Eval(args[2], ushort.MinValue, ushort.MaxValue);
            }
            if (offs > binarysize - 1)
                offs = binarysize - 1;
            if (size > binarysize - offs)
                size = binarysize - offs;
        }

        /// <summary>
        /// Assemble an included binary file's bytes.
        /// </summary>
        /// <param name="line">The <see cref="T:DotNetAsm.SourceLine"/> to assemble.</param>
        void AssembleBinaryBytes(SourceLine line)
        {
            var args = line.Operand.CommaSeparate();
            var binary = _includedBinaries.FirstOrDefault(b => b.Filename.Equals(args[0].TrimOnce('"')));
            if (binary == null)
                throw new Exception("Unable to find binary file " + args[0]);
            int offs = 0, size = binary.Data.Count;

            GetBinaryOffsetSize(args, size, ref offs, ref size);

            if (size > ushort.MaxValue)
                Controller.Log.LogEntry(line, ErrorStrings.IllegalQuantity, size);
            else
                line.Assembly = Controller.Output.AddBytes(binary.Data.Skip(offs), size);
        }

        /// <summary>
        /// Process the .binary directive and cache the binary file's contents
        /// for later use.
        /// </summary>
        /// <param name="line">The <see cref="T:DotNetAsm.SourceLine"/> to assemble.</param>
        /// <returns>A <see cref="T:DotNetAsm.BinaryFile"/>.</returns>
        BinaryFile IncludeBinary(SourceLine line, List<string> args)
        {
            if (args.Count == 0 || args.First().EnclosedInQuotes() == false)
            {
                if (args.Count == 0)
                    Controller.Log.LogEntry(line, ErrorStrings.TooFewArguments, line.Instruction);
                else
                    Controller.Log.LogEntry(line, ErrorStrings.FilenameNotSpecified);
                return new BinaryFile(string.Empty);
            }
            if (args.Count > 3)
            {
                Controller.Log.LogEntry(line, ErrorStrings.TooManyArguments, line.Instruction);
                return new BinaryFile(string.Empty);
            }
            if (!args.First().EnclosedInQuotes())
            {
                Controller.Log.LogEntry(line, ErrorStrings.QuoteStringNotEnclosed);
                return new BinaryFile(string.Empty);
            }
            var filename = args.First().TrimOnce('"');
            var binary = _includedBinaries.FirstOrDefault(b => b.Filename.Equals(filename));

            if (binary == null)
            {
                binary = new BinaryFile(args.First());

                if (binary.Open())
                    _includedBinaries.Add(binary);
                else
                    Controller.Log.LogEntry(line, ErrorStrings.CouldNotProcessBinary, args.First());
            }
            return binary;
        }

        string GetInstruction(SourceLine line)
        {
            if (_typeDefs.ContainsKey(line.Instruction))
                return _typeDefs[line.Instruction].ToLower();
            return line.Instruction.ToLower();
        }

        public void AssembleLine(SourceLine line)
        {
            if (Controller.Output.PCOverflow)
            {
                Controller.Log.LogEntry(line,
                                        ErrorStrings.PCOverflow,
                                        Controller.Output.LogicalPC);
                return;
            }
            var instruction = GetInstruction(line);

            switch (instruction)
            {
                case ".addr":
                case ".word":
                    AssembleValues(line, ushort.MinValue, ushort.MaxValue, 2);
                    break;
                case ".align":
                case ".fill":
                    AssembleFills(line);
                    break;
                case ".binary":
                    AssembleBinaryBytes(line);
                    break;
                case ".byte":
                    AssembleValues(line, byte.MinValue, byte.MaxValue, 1);
                    break;
                case ".sbyte":
                    AssembleValues(line, sbyte.MinValue, sbyte.MaxValue, 1);
                    break;
                case ".dint":
                    AssembleValues(line, int.MinValue, int.MaxValue, 4);
                    break;
                case ".dword":
                    AssembleValues(line, uint.MinValue, uint.MaxValue, 4);
                    break;
                case ".lint":
                    AssembleValues(line, Int24.MinValue, Int24.MaxValue, 3);
                    break;
                case ".long":
                    AssembleValues(line, UInt24.MinValue, UInt24.MaxValue, 3);
                    break;
                case ".sint":
                    AssembleValues(line, short.MinValue, short.MaxValue, 2);
                    break;
                case ".typedef":
                    DefineType(line);
                    break;
                default:
                    AssembleStrings(line);
                    break;
            }
        }

        /// <summary>
        /// Define an existing type to a user-defined type.
        /// </summary>
        /// <param name="line">The <see cref="T:DotNetAsm.SourceLine"/> to assemble.</param>
        void DefineType(SourceLine line)
        {
            if (string.IsNullOrEmpty(line.Label) == false)
            {
                Controller.Log.LogEntry(line, ErrorStrings.None);
                return;
            }
            var csvs = line.Operand.CommaSeparate();
            if (csvs.Count != 2)
            {
                Controller.Log.LogEntry(line, ErrorStrings.None);
                return;
            }
            var currtype = csvs.First();
            if (!Reserved.IsOneOf("PseudoOps", currtype) ||
                !base.IsReserved(currtype))
            {
                Controller.Log.LogEntry(line, ErrorStrings.DefininingUnknownType, currtype);
                return;
            }

            var newtype = csvs.Last();
            if (!Regex.IsMatch(newtype, @"^\.?" + Patterns.SymbolUnicode + "$"))
            {
                Controller.Log.LogEntry(line, ErrorStrings.None);
            }
            else if (Controller.IsInstruction(newtype))
            {
                Controller.Log.LogEntry(line, ErrorStrings.TypeDefinitionError, newtype);
            }
            else if (_reservedSymbol(newtype))
            {
                Controller.Log.LogEntry(line, ErrorStrings.TypeNameReserved, newtype);
            }
            else
            {
                Controller.Log.LogEntry(line, ErrorStrings.Depricated, ".typedef", Controller.Options.WarningsAsErrors);
            }
        }

        public int GetInstructionSize(SourceLine line)
        {
            if (string.IsNullOrEmpty(line.Operand))
            {
                Controller.Log.LogEntry(line, ErrorStrings.TooFewArguments, line.Instruction);
                return 0;
            }

            var csv = line.Operand.CommaSeparate();
            if (csv.Count == 0)
            {
                Controller.Log.LogEntry(line, ErrorStrings.TooFewArguments, line.Instruction);
                return 0;
            }
            var instruction = GetInstruction(line);

            switch (instruction)
            {
                case ".align":
                    {
                        var alignval = Controller.Evaluator.Eval(csv.First());
                        return Compilation.GetAlignmentSize(Convert.ToUInt16(line.PC), Convert.ToUInt16(alignval));
                    }
                case ".binary":
                    {
                        int boffset = 0;
                        var binary = IncludeBinary(line, csv);
                        int bsize = binary.Data.Count;
                        GetBinaryOffsetSize(csv, bsize, ref boffset, ref bsize);
                        return bsize;
                    }
                case ".byte":
                case ".sbyte":
                    return csv.Count;
                case ".dword":
                case ".dint":
                    return csv.Count * 4;
                case ".fill":
                    return (int)Controller.Evaluator.Eval(csv.First(), ushort.MinValue, ushort.MaxValue);
                case ".long":
                case ".lint":
                    return csv.Count * 3;
                case ".cstring":
                case ".pstring":
                case ".nstring":
                case ".lsstring":
                case ".string":
                    return GetExpressionSize(line);
                case ".addr":
                case ".sint":
                case ".word":
                    return csv.Count * 2;
                default:
                    return 0;
            }
        }

        public bool AssemblesInstruction(string instruction)
        {
            return Reserved.IsOneOf("PseudoOps", instruction) ||
                base.IsReserved(instruction) ||
                _typeDefs.ContainsKey(instruction);
        }
        #endregion
    }
}
