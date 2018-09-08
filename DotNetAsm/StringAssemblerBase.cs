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
    /// A base class to assemble string pseudo operations.
    /// </summary>
    public abstract class StringAssemblerBase : AssemblerBase
    {
        #region Members

        Regex _regStrFunc, _regEncName;

        static Regex _regFmtFunc;

        #endregion

        #region Constructors

        /// <summary>
        /// Constructs a <see cref="T:DotNetAsm.StringAssemblerBase"/> class.
        /// </summary>
        /// <param name="controller">The <see cref="T:DotNetAsm.IAssemblyController"/> to associate</param>
        public StringAssemblerBase(IAssemblyController controller) :
            base(controller)
        {
            Reserved.DefineType("Directives",
                    ".cstring", ".lsstring", ".nstring", ".pstring", ".string"
                );

            Reserved.DefineType("Encoding", ".encoding", ".map", ".unmap");

            _regStrFunc = new Regex(@"str(\(.+\))",
                Controller.Options.RegexOption | RegexOptions.Compiled);

            _regFmtFunc = new Regex(@"format(\(.+\))",
                Controller.Options.RegexOption | RegexOptions.Compiled);

            _regEncName = new Regex("^" + Patterns.SymbolBasic + "$",
                Controller.Options.RegexOption | RegexOptions.Compiled);

        }

        #endregion

        #region Methods

        /// <summary>
        /// Update the controller's encoding
        /// </summary>
        /// <param name="line">The <see cref="T:DotNetAsm.SourceLine"/> containing the encoding update</param>
        void UpdateEncoding(SourceLine line)
        {
            line.DoNotAssemble = true;
            var instruction = line.Instruction.ToLower();
            var encoding = Controller.Options.CaseSensitive ? line.Operand : line.Operand.ToLower();
            if (instruction.Equals(".encoding"))
            {
                if (!_regEncName.IsMatch(line.Operand))
                {
                    Controller.Log.LogEntry(line, ErrorStrings.EncodingNameNotValid, line.Operand);
                    return;
                }
                Controller.Encoding.SelectEncoding(encoding);
            }
            else
            {
                var parms = line.Operand.CommaSeparate().ToList();
                if (parms.Count == 0)
                    throw new ArgumentException(line.Operand);
                try
                {
                    var firstparm = parms.First();
                    var lastparm = parms.Last();

                    if (instruction.Equals(".map"))
                    {
                        if (parms.Count < 2 || parms.Count > 3)
                            throw new ArgumentException(line.Operand);

                        int translation = 0;

                        if (lastparm.EnclosedInQuotes())
                        {
                            if (lastparm.First().Equals('"'))
                            {
                                var transString = EvalEncodingParam(lastparm);
                                var translationBytes = System.Text.Encoding.UTF8.GetBytes(transString);
                                if (translationBytes.Length < 4)
                                    Array.Resize(ref translationBytes, 4);
                                translation = BitConverter.ToInt32(translationBytes, 0);
                            }
                            else
                            {
                                translation = char.ConvertToUtf32(EvalEncodingParam(lastparm), 0);
                            }
                        }
                        else
                        {
                            translation = (int)Controller.Evaluator.Eval(lastparm, int.MinValue, int.MaxValue);
                        }

                        if (parms.Count == 2)
                        {
                            var mapchar = EvalEncodingParam(firstparm);
                            Controller.Encoding.Map(mapchar, translation);
                        }
                        else
                        {
                            var firstRange = EvalEncodingParam(firstparm);
                            var lastRange = EvalEncodingParam(parms[1]);
                            Controller.Encoding.Map(string.Concat(firstRange, lastRange), translation);
                        }
                    }
                    else
                    {
                        if (parms.Count > 2)
                            throw new ArgumentException(line.Operand);

                        if (parms.Count == 1)
                        {
                            var unmap = EvalEncodingParam(firstparm);
                            Controller.Encoding.Unmap(unmap);
                        }
                        else
                        {
                            var firstunmap = EvalEncodingParam(firstparm);
                            var lastunmap = EvalEncodingParam(parms[1]);
                            Controller.Encoding.Unmap(string.Concat(firstunmap, lastunmap));
                        }
                    }
                }
                catch (ArgumentException)
                {
                    Controller.Log.LogEntry(line, ErrorStrings.BadExpression, line.Operand);
                }
            }
        }

        /// <summary>
        /// Evaluate parameter the string as either a char literal or expression
        /// </summary>
        /// <param name="p">The string parameter</param>
        /// <returns>A string representation of the parameter</returns>
        string EvalEncodingParam(string p)
        {
            // if char literal return the char itself
            var quoted = p.GetNextQuotedString();
            if (string.IsNullOrEmpty(quoted))
            {
                var result = (int)Controller.Evaluator.Eval(p, 0, 0x10FFFF);
                try
                {
                    return char.ConvertFromUtf32(result);
                }
                catch (ArgumentOutOfRangeException)
                {
                    throw new ArgumentException(p);
                }
            }
            var unescaped = Regex.Unescape(quoted);
            if (unescaped.First().Equals('"'))
                return unescaped.TrimOnce('"');
            return unescaped.TrimOnce('\'');
        }

        /// <summary>
        /// Converts the numerical constant of a mathematical expression or to a string.
        /// </summary>
        /// <param name="line">The <see cref="T:DotNetAsm.SourceLine"/> associated to the expression</param>
        /// <param name="arg">The string expression to convert</param>
        /// <returns>The expression as a string</returns>
        string ExpressionToString(SourceLine line, string arg)
        {
            if (_regStrFunc.IsMatch(arg))
            {
                var m = _regStrFunc.Match(arg);
                string strval = m.Groups[1].Value;
                var param = strval.TrimStartOnce('(').TrimEndOnce(')');
                if (string.IsNullOrEmpty(strval) ||
                    strval.FirstParenEnclosure() != m.Groups[1].Value)
                {
                    Controller.Log.LogEntry(line, ErrorStrings.None);
                    return string.Empty;
                }
                var val = Controller.Evaluator.Eval(param, int.MinValue, uint.MaxValue);
                return val.ToString();
            }
            return GetFormattedString(arg, Controller.Evaluator);
        }

        /// <summary>
        /// Get the size of a string expression.
        /// </summary>
        /// <param name="line">The <see cref="T:DotNetAsm.SourceLine"/> associated to the expression</param>
        /// <returns>The size in bytes of the string expression</returns>
        protected int GetExpressionSize(SourceLine line)
        {
            if (Reserved.IsOneOf("Encoding", line.Instruction))
                return 0;

            var csvs = line.Operand.CommaSeparate();
            int size = 0;
            foreach (string s in csvs)
            {
                if (s.EnclosedInQuotes())
                {
                    size += Controller.Encoding.GetByteCount(Regex.Unescape(s.TrimOnce(s.First())));
                }
                else
                {
                    if (s == "?")
                    {
                        size++;
                    }
                    else
                    {
                        var atoi = ExpressionToString(line, s);
                        if (string.IsNullOrEmpty(atoi))
                        {
                            var v = Controller.Evaluator.Eval(s);
                            size += v.Size();
                        }
                        else
                        {
                            size += atoi.Length;//Controller.Encoding.GetByteCount(atoi);
                        }
                    }
                }
            }
            if (line.Instruction.Equals(".cstring", Controller.Options.StringComparison) ||
                line.Instruction.Equals(".pstring", Controller.Options.StringComparison))
                size++;
            return size;
        }

        public override bool IsReserved(string token)
        {
            return Reserved.IsOneOf("Directives", token) ||
                   Reserved.IsOneOf("Encoding", token);
        }

        /// <summary>
        /// Assemble strings to the output.
        /// </summary>
        /// <param name="line">The <see cref="T:DotNetAsm.SourceLine"/> to assemble.</param>
        protected void AssembleStrings(SourceLine line)
        {
            if (Reserved.IsOneOf("Encoding", line.Instruction))
            {
                UpdateEncoding(line);
                return;
            }
            var format = line.Instruction.ToLower();

            if (format.Equals(".pstring"))
            {
                try
                {
                    // we need to get the instruction size for the whole length, including all args
                    line.Assembly = Controller.Output.Add(Convert.ToByte(GetExpressionSize(line) - 1), 1);
                }
                catch (OverflowException)
                {
                    Controller.Log.LogEntry(line, ErrorStrings.PStringSizeTooLarge);
                    return;
                }
            }
            else if (format.Equals(".lsstring"))
            {
                Controller.Output.Transforms.Push(b => Convert.ToByte(b << 1));
            }
            var args = line.Operand.CommaSeparate();

            foreach (var arg in args)
            {
                List<byte> encoded;
                var quoted = arg.GetNextQuotedString();
                if (string.IsNullOrEmpty(quoted))
                {
                    if (arg == "?")
                    {
                        Controller.Output.AddUninitialized(1);
                        continue;
                    }
                    var atoi = ExpressionToString(line, arg);

                    if (string.IsNullOrEmpty(atoi))
                    {
                        var val = Controller.Evaluator.Eval(arg);
                        encoded = Controller.Output.Add(val, val.Size());
                    }
                    else
                    {
                        encoded = Controller.Output.Add(atoi, Controller.Encoding);
                    }
                }
                else
                {
                    if (!quoted.Equals(arg))
                    {
                        Controller.Log.LogEntry(line, ErrorStrings.None);
                        return;
                    }
                    var unescaped = Regex.Unescape(quoted.TrimOnce(quoted.First()));
                    encoded = Controller.Output.Add(unescaped, Controller.Encoding);
                }
                if (format.Equals(".nstring"))
                {
                    var neg = encoded.FirstOrDefault(b => b > 0x7f);
                    if (neg > 0x7f)
                    {
                        Controller.Log.LogEntry(line, ErrorStrings.IllegalQuantity, neg);
                        return;
                    }
                }
                line.Assembly.AddRange(encoded);
            }
            var lastbyte = Controller.Output.GetCompilation().Last();

            if (format.Equals(".lsstring"))
            {
                line.Assembly[line.Assembly.Count - 1] = (byte)(lastbyte | 1);
                Controller.Output.ChangeLast(lastbyte | 1, 1);
                Controller.Output.Transforms.Pop(); // clean up again :)
            }
            else if (format.Equals(".nstring"))
            {
                line.Assembly[line.Assembly.Count - 1] = Convert.ToByte((lastbyte + 128));
                Controller.Output.ChangeLast(Convert.ToByte(lastbyte + 128), 1);
            }

            if (format.Equals(".cstring"))
            {
                line.Assembly.Add(0);
                Controller.Output.Add(0, 1);
            }
        }
        #region Static Methods

        /// <summary>
        /// Gets the formatted string.
        /// </summary>
        /// <returns>The formatted string.</returns>
        /// <param name="operand">The line's operand.</param>
        /// <param name="evaluator">The <see cref="T:DotNetAsm.IEvaluator"/> to evaluate non-string objects.</param>
        public static string GetFormattedString(string operand, IEvaluator evaluator)
        {
            var m = _regFmtFunc.Match(operand);
            if (string.IsNullOrEmpty(m.Value))
                return string.Empty;
            var parms = m.Groups[1].Value;
            if (string.IsNullOrEmpty(parms) || m.Groups[1].Value.FirstParenEnclosure() != parms)
                throw new Exception(ErrorStrings.None);

            var csvs = parms.TrimStartOnce('(').TrimEndOnce(')').CommaSeparate();
            var fmt = csvs.First();
            if (fmt.Length < 5 || !fmt.EnclosedInQuotes())
                throw new Exception(ErrorStrings.None);
            var parmlist = new List<object>();

            for (int i = 1; i < csvs.Count; i++)
            {
                if (string.IsNullOrEmpty(csvs[i]))
                    throw new Exception(ErrorStrings.None);
                if (csvs[i].EnclosedInQuotes())
                    parmlist.Add(Regex.Unescape(csvs[i].TrimOnce('"')));
                else
                    parmlist.Add(evaluator.Eval(csvs[i]));
            }
            return string.Format(Regex.Unescape(fmt.TrimOnce('"')), parmlist.ToArray());
        }

        #endregion

        #endregion
    }
}
