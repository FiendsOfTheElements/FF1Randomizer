//-----------------------------------------------------------------------------
// Copyright (c) 2017, 2018 informedcitizenry <informedcitizenry@gmail.com>
//
// Licensed under the MIT license. See LICENSE_6502net for full license information.
//
//-----------------------------------------------------------------------------

using DotNetAsm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Inline6502
{
    /// <summary>
    /// A line assembler that will assemble into 6502 instructions.
    /// </summary>
    public sealed partial class Asm6502 : AssemblerBase, ILineAssembler
    {
        #region Constructors

        /// <summary>
        /// Constructs an instance of a 6502 line assembler. This assembler will output valid
        /// 6502 assembly to instructions.
        /// </summary>
        /// <param name="controller">The assembly controller.</param>
        public Asm6502(IAssemblyController controller) :
            base(controller)
        {
            Reserved.DefineType("Branches",
                    "bcc", "bcs", "beq", "bmi", "bne", "bpl", "bra", "bvc",
                    "bvs", "bra"
                 );

            Reserved.DefineType("Branches16",
                    "brl", "per"
                );

            Reserved.DefineType("ImpliedAccumulator",
                    "asl", "lsr", "rol", "ror"
                );

            Reserved.DefineType("Mnemonics",
                    "adc", "anc", "and", "ane", "arr", "asl", "asr", "bit",
                    "brk", "clc", "cld", "cli", "clv", "cmp", "cop", "cpx",
                    "cpy", "dcp", "dex", "dey", "dop", "eor", "inx", "iny",
                    "isb", "jam", "jml", "jmp", "jsl", "jsr", "las", "lax",
                    "lda", "ldx", "ldy", "lsr", "nop", "ora", "pea", "pei",
                    "pha", "phb", "phd", "phk", "php", "phx", "phy", "pla",
                    "plb", "pld", "plp", "plx", "ply", "rep", "rla", "rol",
                    "ror", "rra", "rti", "rtl", "rts", "sbc", "sax", "sec",
                    "sed", "sei", "sep", "shx", "shy", "slo", "sre", "sha",
                    "sta", "stp", "stx", "sty", "stz", "tas", "tax", "tay",
                    "tcd", "tcs", "tdc", "top", "trb", "tsb", "tsc", "tsx",
                    "txa", "txs", "txy", "tya", "tyx", "xba", "xce", "wai"
                );

            Reserved.DefineType("ImpliedAC02",
                    "dec", "inc"
                );

            Reserved.DefineType("MoveMemory",
                    "mvn", "mvp"
                );

            Reserved.DefineType("ReturnAddress",
                    ".rta"
                );

            Reserved.DefineType("LongShort",
                    ".m16", ".m8", ".x16", ".x8", ".mx16", ".mx8"
                );

            Controller.AddSymbol("a");
            Controller.CpuChanged += SetCpu;

            // set architecture specific encodings
            Controller.Encoding.SelectEncoding("petscii");
            Controller.Encoding.Map("az", 'A');
            Controller.Encoding.Map("AZ", 0xc1);
            Controller.Encoding.Map('£', '\\');
            Controller.Encoding.Map('↑', '^');
            Controller.Encoding.Map('←', '_');
            Controller.Encoding.Map('▌', 0xa1);
            Controller.Encoding.Map('▄', 0xa2);
            Controller.Encoding.Map('▔', 0xa3);
            Controller.Encoding.Map('▁', 0xa4);
            Controller.Encoding.Map('▏', 0xa5);
            Controller.Encoding.Map('▒', 0xa6);
            Controller.Encoding.Map('▕', 0xa7);
            Controller.Encoding.Map('◤', 0xa9);
            Controller.Encoding.Map('├', 0xab);
            Controller.Encoding.Map('└', 0xad);
            Controller.Encoding.Map('┐', 0xae);
            Controller.Encoding.Map('▂', 0xaf);
            Controller.Encoding.Map('┌', 0xb0);
            Controller.Encoding.Map('┴', 0xb1);
            Controller.Encoding.Map('┬', 0xb2);
            Controller.Encoding.Map('┤', 0xb3);
            Controller.Encoding.Map('▎', 0xb4);
            Controller.Encoding.Map('▍', 0xb5);
            Controller.Encoding.Map('▃', 0xb9);
            Controller.Encoding.Map('✓', 0xba);
            Controller.Encoding.Map('┘', 0xbd);
            Controller.Encoding.Map('━', 0xc0);
            Controller.Encoding.Map('♠', 0xc1);
            Controller.Encoding.Map('│', 0xc2);
            Controller.Encoding.Map('╮', 0xc9);
            Controller.Encoding.Map('╰', 0xca);
            Controller.Encoding.Map('╯', 0xcb);
            Controller.Encoding.Map('╲', 0xcd);
            Controller.Encoding.Map('╱', 0xce);
            Controller.Encoding.Map('●', 0xd1);
            Controller.Encoding.Map('♥', 0xd3);
            Controller.Encoding.Map('╭', 0xd5);
            Controller.Encoding.Map('╳', 0xd6);
            Controller.Encoding.Map('○', 0xd7);
            Controller.Encoding.Map('♣', 0xd8);
            Controller.Encoding.Map('♦', 0xda);
            Controller.Encoding.Map('┼', 0xdb);
            Controller.Encoding.Map('π', 0xde);
            Controller.Encoding.Map('◥', 0xdf);

            Controller.Encoding.SelectEncoding("cbmscreen");
            Controller.Encoding.Map("@Z", '\0');
            Controller.Encoding.Map("az", 'A');
            Controller.Encoding.Map('£', '\\');
            Controller.Encoding.Map('π', '^'); // π is $5e in unshifted
            Controller.Encoding.Map('↑', '^'); // ↑ is $5e in shifted
            Controller.Encoding.Map('←', '_');
            Controller.Encoding.Map('▌', '`');
            Controller.Encoding.Map('▄', 'a');
            Controller.Encoding.Map('▔', 'b');
            Controller.Encoding.Map('▁', 'c');
            Controller.Encoding.Map('▏', 'd');
            Controller.Encoding.Map('▒', 'e');
            Controller.Encoding.Map('▕', 'f');
            Controller.Encoding.Map('◤', 'i');
            Controller.Encoding.Map('├', 'k');
            Controller.Encoding.Map('└', 'm');
            Controller.Encoding.Map('┐', 'n');
            Controller.Encoding.Map('▂', 'o');
            Controller.Encoding.Map('┌', 'p');
            Controller.Encoding.Map('┴', 'q');
            Controller.Encoding.Map('┬', 'r');
            Controller.Encoding.Map('┤', 's');
            Controller.Encoding.Map('▎', 't');
            Controller.Encoding.Map('▍', 'u');
            Controller.Encoding.Map('▃', 'y');
            Controller.Encoding.Map('✓', 'z');
            Controller.Encoding.Map('┘', '}');
            Controller.Encoding.Map('━', '@');
            Controller.Encoding.Map('♠', 'A');
            Controller.Encoding.Map('│', 'B');
            Controller.Encoding.Map('╮', 'I');
            Controller.Encoding.Map('╰', 'J');
            Controller.Encoding.Map('╯', 'K');
            Controller.Encoding.Map('╲', 'M');
            Controller.Encoding.Map('╱', 'N');
            Controller.Encoding.Map('●', 'Q');
            Controller.Encoding.Map('♥', 'S');
            Controller.Encoding.Map('╭', 'U');
            Controller.Encoding.Map('╳', 'V');
            Controller.Encoding.Map('○', 'W');
            Controller.Encoding.Map('♣', 'X');
            Controller.Encoding.Map('♦', 'Z');
            Controller.Encoding.Map('┼', '[');
            Controller.Encoding.Map('◥', '_');

            Controller.Encoding.SelectEncoding("atascreen");
            Controller.Encoding.Map(" _", '\0');

            Controller.Encoding.SelectDefaultEncoding();

            ConstructOpcodeTable();

            _filteredOpcodes = _opcodes.Where(o => o.Value.CPU.Equals("6502")).ToDictionary(k => k.Key, k => k.Value, Controller.Options.StringComparar);
        }

        #endregion

        #region Methods

        void SetCpu(CpuChangedEventArgs args)
        {
            if (args.Line.Operand.EnclosedInQuotes() == false &&
                !args.Line.SourceString.Equals(ConstStrings.COMMANDLINE_ARG))
            {
                Controller.Log.LogEntry(args.Line, ErrorStrings.QuoteStringNotEnclosed);
                return;
            }
            var cpu = args.Line.Operand.Trim('"');
            if (!cpu.Equals("6502") && !cpu.Equals("65C02") && !cpu.Equals("65816") && !cpu.Equals("6502i"))
            {
                var error = string.Format("Invalid CPU '{0}' specified", cpu);
                if (args.Line.SourceString.Equals(ConstStrings.COMMANDLINE_ARG))
                    throw new Exception(string.Format(error));

                Controller.Log.LogEntry(args.Line, error);
                return;
            }
            _cpu = cpu;

            switch (_cpu)
            {
                case "65816":
                    _filteredOpcodes = _opcodes.Where(o => o.Value.CPU.Equals("6502") || o.Value.CPU.Equals("65C02") || o.Value.CPU.Equals("65816")).ToDictionary(k => k.Key, k => k.Value, Controller.Options.StringComparar);
                    _filteredOpcodes.Add("stp", new Instruction { CPU = "65816", Size = 1, Opcode = 0xdb });
                    break;
                case "65C02":
                    _filteredOpcodes = _opcodes.Where(o => o.Value.CPU.Equals("6502") || o.Value.CPU.Equals("65C02")).ToDictionary(k => k.Key, k => k.Value, Controller.Options.StringComparar);
                    break;
                case "6502i":
                    _filteredOpcodes = _opcodes.Where(o => o.Value.CPU.Equals("6502") || o.Value.CPU.Equals("6502i")).ToDictionary(k => k.Key, k => k.Value, Controller.Options.StringComparar);
                    break;
                case "6502":
                    _filteredOpcodes = _opcodes.Where(o => o.Value.CPU.Equals("6502")).ToDictionary(k => k.Key, k => k.Value, Controller.Options.StringComparar);
                    break;
                default:
                    break;

            }

            if (_m16)
                SetImmediateA(3);
            if (_x16)
                SetImmediateXY(3);

        }

        void SetImmediateA(int size)
        {
            if (size == 3 && _cpu.StartsWith("6502", StringComparison.Ordinal))
            {
                return;
            }
            string fmt = size == 3 ? " #${0:x4}" : " #${0:x2}";
            string prv = size == 3 ? " #${0:x2}" : " #${0:x4}";

            _filteredOpcodes.Remove("ora" + prv);
            _filteredOpcodes.Remove("and" + prv);
            _filteredOpcodes.Remove("eor" + prv);
            _filteredOpcodes.Remove("adc" + prv);
            _filteredOpcodes.Remove("bit" + prv);
            _filteredOpcodes.Remove("lda" + prv);
            _filteredOpcodes.Remove("cmb" + prv);
            _filteredOpcodes.Remove("sbc" + prv);

            _filteredOpcodes["ora" + fmt] = new Instruction { CPU = "6502",  Size = size, Opcode = 0x09 };
            _filteredOpcodes["and" + fmt] = new Instruction { CPU = "6502",  Size = size, Opcode = 0x29 };
            _filteredOpcodes["eor" + fmt] = new Instruction { CPU = "6502",  Size = size, Opcode = 0x49 };
            _filteredOpcodes["adc" + fmt] = new Instruction { CPU = "6502",  Size = size, Opcode = 0x69 };
            _filteredOpcodes["bit" + fmt] = new Instruction { CPU = "65C02", Size = size, Opcode = 0x89 };
            _filteredOpcodes["lda" + fmt] = new Instruction { CPU = "6502",  Size = size, Opcode = 0xa9 };
            _filteredOpcodes["cmp" + fmt] = new Instruction { CPU = "6502",  Size = size, Opcode = 0xc9 };
            _filteredOpcodes["sbc" + fmt] = new Instruction { CPU = "6502",  Size = size, Opcode = 0xe9 };
        }

        void SetImmediateXY(int size)
        {
            if (size == 3 && _cpu.StartsWith("6502", StringComparison.Ordinal))
            {
                return;
            }
            string fmt = size == 3 ? " #${0:x4}" : " #${0:x2}";
            string prv = size == 3 ? " #${0:x2}" : " #${0:x4}";

            _filteredOpcodes.Remove("ldy" + prv);
            _filteredOpcodes.Remove("ldx" + prv);
            _filteredOpcodes.Remove("cpy" + prv);
            _filteredOpcodes.Remove("cpx" + prv);

            _filteredOpcodes["ldy" + fmt] = new Instruction { CPU = "6502", Size = size, Opcode = 0xa0 };
            _filteredOpcodes["ldx" + fmt] = new Instruction { CPU = "6502", Size = size, Opcode = 0xa2 };
            _filteredOpcodes["cpy" + fmt] = new Instruction { CPU = "6502", Size = size, Opcode = 0xc0 };
            _filteredOpcodes["cpx" + fmt] = new Instruction { CPU = "6502", Size = size, Opcode = 0xe0 };
        }

        void SetRegLongShort(string instruction)
        {
            if (instruction.StartsWith(".x", Controller.Options.StringComparison))
            {
                var x16 = instruction.Equals(".x16", Controller.Options.StringComparison);
                if (x16 != _x16)
                {
                    _x16 = x16;
                    SetImmediateXY(_x16 ? 3 : 2);
                }
            }
            else
            {

                var m16 = instruction.EndsWith("16", Controller.Options.StringComparison);
                if (m16 != _m16)
                {
                    _m16 = m16;
                    SetImmediateA(_m16 ? 3 : 2);
                }
                if (instruction.StartsWith(".mx", Controller.Options.StringComparison))
                {
                    var x16 = instruction.EndsWith("16", Controller.Options.StringComparison);
                    if (x16 != _x16)
                    {
                        _x16 = x16;
                        SetImmediateXY(_x16 ? 3 : 2);
                    }
                }
            }
        }

        void AssembleRta(SourceLine line)
        {
            var csv = line.Operand.CommaSeparate();

            foreach (string rta in csv)
            {
                if (rta.Equals("?"))
                {
                    Controller.Output.AddUninitialized(2);
                }
                else
                {
                    var val = Controller.Evaluator.Eval(rta, ushort.MinValue, ushort.MaxValue + 1);
                    line.Assembly.AddRange(Controller.Output.Add(val - 1, 2));
                }
            }
        }


        long ParseSubExpression(string subexpression, char open, char closure, StringBuilder formatBuilder, bool isAbsolute)
        {
            var commasep = subexpression.Substring(1, subexpression.Length - 2).CommaSeparate(open, closure);
            if (commasep.Count() > 2)
                throw new Exception(ErrorStrings.None);

            string expression = commasep.First().Trim();

            var val = Controller.Evaluator.Eval(expression);
            var expSize = isAbsolute ? 2 : 1;
            if (val.Size() > expSize)
                throw new OverflowException(val.ToString());


            formatBuilder.Append(open);
            formatBuilder.Append("${0:x" + expSize * 2 + "}");

            if (commasep.Count() > 1)
                formatBuilder.AppendFormat(",{0}{1}", commasep.Last(), closure);
            else
                formatBuilder.Append(closure);
            return val;
        }


        Tuple<OperandFormat, Instruction> GetFormatAndOpcode(SourceLine line)
        {

            var instruction = line.Instruction.ToLower();
            string expression1 = string.Empty, expression2 = string.Empty;
            long eval1 = long.MinValue, eval2 = long.MinValue;
            StringBuilder formatBuilder = new StringBuilder();

            formatBuilder.AppendFormat("{0}", instruction);

            bool impliedA = line.Operand.Equals("a", Controller.Options.StringComparison);

            if (impliedA &&
                !(Reserved.IsOneOf("ImpliedAccumulator", instruction) ||
                  (Reserved.IsOneOf("ImpliedAC02", instruction) && !_cpu.Equals("6502"))
                 )
                )
                throw new Exception();

            string finalFormat;
            Instruction opcode;

            if (string.IsNullOrEmpty(line.Operand) || impliedA)
            {
                finalFormat = formatBuilder.ToString();
                if (!_filteredOpcodes.TryGetValue(finalFormat, out opcode))
                {
                    return new Tuple<OperandFormat, Instruction>(null, null);
                }
            }
            else
            {
                formatBuilder.Append(' ');
                int expSize = 1;

                string operand = line.Operand;

                if (operand[0] == '[')
                {
                    // differentiate between 'and [16]' and 'and [16] 16'
                    var firstBracket = operand.FirstParenEnclosure(useBracket: true);
                    var firstBracketLength = firstBracket.Length;
                    if (operand.Length > firstBracketLength)
                    {
                        var delim = operand.Substring(firstBracketLength).First(c => !char.IsWhiteSpace(c));
                        if (delim != ',')
                        {
                            if (!char.IsWhiteSpace(operand[firstBracketLength]))
                                throw new Exception(ErrorStrings.None);
                            expSize = Convert.ToInt32(Controller.Evaluator.Eval(firstBracket.Substring(1, firstBracketLength - 2)));
                            if (expSize != 16 && expSize != 24)
                                throw new Exception(ErrorStrings.None);
                            expSize /= 8;
                            operand = operand.Substring(firstBracketLength + 1);
                        }
                    }
                }
                if (operand[0] == '#')
                {
                    if (operand.Length < 2 || char.IsWhiteSpace(operand[1]))
                        throw new ExpressionException(operand);
                    expression1 = operand.Substring(1);
                    eval1 = Controller.Evaluator.Eval(expression1);
                    expSize = eval1.Size();
                    formatBuilder.Append("#${0:x" + expSize * 2 + "}");
                }
                else
                {
                    IEnumerable<string> commasep;
                    if (line.Operand[0] == '[')
                        commasep = operand.CommaSeparate('[', ']');
                    else
                        commasep = operand.CommaSeparate();
                    if (commasep.Count() > 2)
                        throw new Exception(ErrorStrings.None);


                    string outerexpression = string.Empty;
                    if (commasep.Count() > 1)
                    {
                        if (Reserved.IsOneOf("MoveMemory", instruction))
                        {
                            outerexpression = "${1:x2}";
                            expression2 = commasep.Last();
                            eval2 = Controller.Evaluator.Eval(expression2.Trim());
                        }
                        else
                        {
                            outerexpression = commasep.Last();

                        }
                    }

                    var param1 = commasep.First();
                    if (param1[0] == '(' && param1.Last() == ')' &&
                        param1.FirstParenEnclosure().Equals(param1) &&
                        (commasep.Count() == 1 || commasep.Last().Equals("y", Controller.Options.StringComparison)))
                    {
                        eval1 = ParseSubExpression(param1, '(', ')', formatBuilder, instruction.Equals("jmp"));
                        expression1 = eval1.ToString();
                    }
                    else if (param1[0] == '[')
                    {

                        eval1 = ParseSubExpression(param1, '[', ']', formatBuilder, instruction.Equals("jmp"));
                        expression1 = eval1.ToString();
                    }
                    else
                    {
                        expression1 = param1.TrimEnd();
                        eval1 = Controller.Evaluator.Eval(expression1);
                        if (expSize == 1)
                        {
                            if (Reserved.IsOneOf("Branches16", instruction))
                            {
                                // we have to check this too in case the user does a brl $10ffff
                                expSize++;
                            }
                            else
                            {
                                expSize = eval1.Size();
                            }
                        }
                        formatBuilder.Append("${0:x" + expSize * 2 + "}");
                    }

                    if (commasep.Count() > 1)
                        formatBuilder.AppendFormat(",{0}", outerexpression);
                }
                finalFormat = formatBuilder.ToString();

                while (!_filteredOpcodes.TryGetValue(finalFormat, out opcode))
                {
                    // some instructions the size is bigger than the expression comes out to, so
                    // make the expression size larger
                    if (expSize > 3)
                        return new Tuple<OperandFormat, Instruction>(null, null); // we didn't find it
                    finalFormat = finalFormat.Replace("x" + (expSize++) * 2, "x" + expSize * 2);
                }

            }
            var fmt = new OperandFormat
            {
                FormatString = finalFormat,
                Eval1 = eval1,
                Eval2 = eval2
            };
            return new Tuple<OperandFormat, Instruction>(fmt, opcode);
        }

        #region ILineAssembler.Methods

        public void AssembleLine(SourceLine line)
        {
            if (Controller.Output.PCOverflow)
            {
                Controller.Log.LogEntry(line,
                                        ErrorStrings.PCOverflow,
                                        Controller.Output.LogicalPC);
                return;
            }
            if (Reserved.IsOneOf("ReturnAddress", line.Instruction))
            {
                AssembleRta(line);
                return;
            }
            if (Reserved.IsOneOf("LongShort", line.Instruction))
            {
                if (!string.IsNullOrEmpty(line.Operand))
                {
                    Controller.Log.LogEntry(line, ErrorStrings.TooManyArguments, line.Instruction);
                }
                else
                {
                    if (_cpu == null || !_cpu.Equals("65816"))
                        Controller.Log.LogEntry(line,
                            "The current CPU supports only 8-bit immediate mode instructions. The directive '" + line.Instruction + "' will not affect assembly",
                            Controller.Options.WarningsAsErrors);
                    else
                        SetRegLongShort(line.Instruction);
                }
                return;
            }

            var formatOpcode = GetFormatAndOpcode(line);
            if (formatOpcode.Item1 == null)
            {
                Controller.Log.LogEntry(line, ErrorStrings.BadExpression, line.Operand);
                return;
            }
            if (formatOpcode.Item2 == null)
            {
                Controller.Log.LogEntry(line, ErrorStrings.UnknownInstruction, line.Instruction);
                return;
            }
            long eval1 = formatOpcode.Item1.Eval1, eval2 = formatOpcode.Item1.Eval2;

            // how the (first) evaluated expression will display in disassembly
            long eval1DisplayValue = 0;

            if (Reserved.IsOneOf("Branches", line.Instruction) || Reserved.IsOneOf("Branches16", line.Instruction))
            {
                eval1DisplayValue = eval1 & 0xFFFF;
                try
                {
                    if (Reserved.IsOneOf("Branches", line.Instruction))
                        eval1 = Convert.ToSByte(Controller.Output.GetRelativeOffset((ushort)eval1DisplayValue, Controller.Output.LogicalPC + 2));
                    else
                        eval1 = Convert.ToInt16(Controller.Output.GetRelativeOffset((ushort)eval1DisplayValue, Controller.Output.LogicalPC + 3));
                }
                catch
                {
                    throw new OverflowException(eval1.ToString());
                }

            }
            else
            {
                var operandsize = 0;

                if (eval1 != long.MinValue)
                {
                    if (formatOpcode.Item2.Size == 4)
                    {
                        eval1 &= 0xFFFFFF;
                    }
                    else if (formatOpcode.Item2.Size == 3 && eval2 == long.MinValue)
                    {
                        eval1 &= 0xFFFF;
                    }
                    else
                    {
                        eval1 &= 0xFF;
                        if (eval2 != long.MinValue)
                            eval2 &= 0xFF;
                    }
                    eval1DisplayValue = eval1;
                    operandsize = eval1.Size();
                }
                if (eval2 != long.MinValue)
                    operandsize += eval2.Size();

                if (operandsize >= formatOpcode.Item2.Size)
                    throw new OverflowException(line.Operand);
            }
            long operbytes = 0;
            if (eval1 != long.MinValue)
                operbytes = eval2 == long.MinValue ? (eval1 << 8) : (((eval1 << 8) | eval2) << 8);

            line.Disassembly = string.Format(formatOpcode.Item1.FormatString, eval1DisplayValue, eval2);
            line.Assembly = Controller.Output.Add(Convert.ToInt32(operbytes) | formatOpcode.Item2.Opcode,
                                                  formatOpcode.Item2.Size);
        }

        public int GetInstructionSize(SourceLine line)
        {
            if (Reserved.IsOneOf("ReturnAddress", line.Instruction))
                return 2 * line.Operand.CommaSeparate().Count;

            var formatOpcode = GetFormatAndOpcode(line);
            if (formatOpcode.Item2 != null)
                return formatOpcode.Item2.Size;
            return 0;
        }

        public bool AssemblesInstruction(string instruction)
                    => Reserved.IsReserved(instruction);

        #endregion

        #endregion
    }
}
