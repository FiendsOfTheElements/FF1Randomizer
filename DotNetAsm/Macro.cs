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
    /// Represents a macro definition. Macros are small snippets of code that can be re-used
    /// and even parameterized. Parameters are text-substituted.
    /// </summary>
    public class Macro
    {
        #region Exception

        /// <summary>
        /// Represents a macro-related error that occurs during runtime.
        /// </summary>
        public class MacroException : Exception
        {
            #region Exception.Members

            string _macroExceptMsg;

            #endregion

            #region Exception.Constructors

            /// <summary>
            /// Constructs a new macro exception.
            /// </summary>
            /// <param name="line">The <see cref="T:DotNetAsm.SourceLine"/> where the exception occurred.</param>
            /// <param name="message">The error message.</param>
            public MacroException(SourceLine line, string message)
            {
                Line = line;
                _macroExceptMsg = message;
            }

            #endregion

            #region Exception.Methods

            /// <summary>
            /// Gets the exception message.
            /// </summary>
            public override string Message
            {
                get
                {
                    return _macroExceptMsg;
                }
            }

            #endregion

            #region Exception.Properties

            /// <summary>
            /// Gets the SourceLine for which the exception occurred
            /// </summary>
            public SourceLine Line { get; private set; }

            #endregion
        }

        #endregion

        #region Subclass

        /// <summary>
        /// Represents a macro parameter.
        /// </summary>
        public class Param
        {
            #region Param.Constructor

            /// <summary>
            /// Constructs an new instance of a parameter.
            /// </summary>
            public Param()
            {
                Name = DefaultValue = Passed = string.Empty;
                Number = 0;
                SourceLines = new List<string>();
            }
            #endregion

            #region Param.Properties

            /// <summary>
            /// Gets or sets the parameter name.
            /// </summary>
            public string Name { get; set; }

            /// <summary>
            /// Gets or sets the parameter number reference in the macro source.
            /// </summary>
            public int Number { get; set; }

            /// <summary>
            /// Gets or sets the default value of the parameter if no parameter
            /// passed by the client.
            /// </summary>
            public string DefaultValue { get; set; }

            /// <summary>
            /// Gets or sets the value passed by the client.
            /// </summary>
            public string Passed { get; set; }

            /// <summary>
            /// Gets or sets the source line lists (SourceLine.SourceInfo)
            /// that reference the parameter in macro source.
            /// </summary>
            public List<string> SourceLines { get; set; }

            #endregion
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Constructs a new macro instance.
        /// </summary>
        public Macro()
        {
            Params = new List<Param>();
            Source = new List<SourceLine>();
        }

        #endregion

        #region Methods

        /// <summary>
        /// Expands the macro into source from the invocation.
        /// </summary>
        /// <param name="macrocall">The <see cref="T:SourceLine"/> that is invoking the macro. The macro
        /// name is in the instruction, while the list of parameters passed
        /// are in the operand.</param>
        /// <returns>A <see cref="T:System.Collections.Generic.IEnumerable&lt;DotNetAsm.SourceLine&gt;"/>
        /// from the expanded macro, included substituted parameters in source.</returns>
        public IEnumerable<SourceLine> Expand(SourceLine macrocall)
        {
            var parms = new List<Param>(Params);

            if (IsSegment == false)
            {
                // any parameters passed?
                // get passed parameters
                var passed = macrocall.Operand.CommaSeparate();


                // no passed parameters is ok
                if (passed == null)
                    passed = new List<string>();

                //  if passed exceeds defined parameters raise an error
                if (passed.Count > parms.Count)
                    throw new MacroException(macrocall, "Too many arguments");
                if (passed.Count < parms.Count) // else pad passed to match defined
                    passed.AddRange(Enumerable.Repeat(string.Empty, parms.Count - passed.Count));

                for (int i = 0; i < passed.Count; i++)
                {
                    if (string.IsNullOrEmpty(passed[i]))
                    {
                        if (string.IsNullOrEmpty(parms[i].DefaultValue))
                            throw new MacroException(macrocall,
                                string.Format("Macro '{0}' expects a value for parameter {1}; no default value defined",
                                macrocall.Instruction.TrimStartOnce('.'),
                                parms[i].Number));

                        parms[i].Passed = parms[i].DefaultValue;
                    }
                    else
                    {
                        parms[i].Passed = passed[i];
                    }
                }
            }
            if (!string.IsNullOrEmpty(macrocall.Label))
                yield return new SourceLine
                {
                    Label = macrocall.Label,
                    Scope = macrocall.Scope
                };

            foreach (var src in Source)
            {
                var repl = src.Clone() as SourceLine;
                repl.Scope = macrocall.Scope + repl.Scope;

                if (IsSegment == false)
                {
                    var insertedparm = parms.FirstOrDefault(pa =>
                                            pa.SourceLines.Any(pas =>
                                             pas == src.SourceInfo()));
                    if (insertedparm != null)
                    {
                        var passed_pattern = string.Format(@"\\{0}|\\{1}", insertedparm.Number, insertedparm.Name);
                        repl.Operand = Regex.Replace(src.Operand, passed_pattern,
                            m => insertedparm.Passed, RegexOptions.IgnoreCase);
                        repl.SourceString = Regex.Replace(repl.SourceString, passed_pattern,
                            insertedparm.Passed, RegexOptions.IgnoreCase);
                    }
                }
                yield return repl;
            }
        }

        #endregion

        #region Static Methods

        /// <summary>
        /// Create the specified definition, closure, source, comparer, openBlock and closeBlock.
        /// </summary>
        /// <returns>The resulting macro.</returns>
        /// <param name="definition">Definition.</param>
        /// <param name="closure">Closure.</param>
        /// <param name="source">Source.</param>
        /// <param name="comparer">Comparer.</param>
        /// <param name="openBlock">Open block.</param>
        /// <param name="closeBlock">Close block.</param>
        public static Macro Create(SourceLine definition,
                                   SourceLine closure,
                                   IEnumerable<SourceLine> source,
                                   StringComparison comparer,
                                   string openBlock,
                                   string closeBlock)
        {
            var macro = new Macro();
            string name = definition.Label;
            string class_ = ".macro";
            bool isSegment = false;
            if (definition.Instruction.Equals(".segment", comparer))
            {
                isSegment = true;
                name = definition.Operand;
                class_ = ".segment";
            }
            if (definition.IsComment == false)
            {

                macro.IsSegment = isSegment;
                if (macro.IsSegment == false)
                {
                    macro.Source.Add(new SourceLine());
                    macro.Source.First().Filename = definition.Filename;
                    macro.Source.First().LineNumber = definition.LineNumber;
                    macro.Source.First().Instruction = openBlock;

                    if (string.IsNullOrEmpty(definition.Operand) == false)
                    {
                        var parms = definition.Operand.CommaSeparate();
                        if (parms == null)
                        {
                            throw new MacroException(definition, "Invalid parameter(s) (" + definition.Operand + ")");
                        }
                        for (int i = 0; i < parms.Count; i++)
                        {
                            var p = parms[i];
                            var parm = new Macro.Param
                            {
                                Number = i + 1
                            };
                            if (p.Contains("="))
                            {
                                var ps = p.Split(new char[] { '=' }, StringSplitOptions.RemoveEmptyEntries);
                                var pname = ps.First().Trim();
                                if (ps.Count() != 2)
                                {
                                    throw new MacroException(definition, "Default parameter assignment error");
                                }
                                if (Regex.IsMatch(pname, Patterns.SymbolUnicode) == false)
                                {
                                    throw new MacroException(definition, "Parameter name '" + pname + "' invalid");
                                }
                                parm.Name = pname;
                                parm.DefaultValue = ps.Last().Trim();
                            }
                            else if (Regex.IsMatch(p, Patterns.SymbolUnicode) == false)
                            {
                                throw new MacroException(definition, "Parameter name '" + p + "' invalid");
                            }
                            else
                            {
                                parm.Name = p;
                                parm.DefaultValue = string.Empty;
                            }
                            // check for duplicate param names
                            if (macro.Params.Any(prm => parm.Name.Equals(prm.Name, comparer)))
                            {
                                throw new MacroException(definition, "Duplicate parameter name found: " + parm.Name);
                            }
                            macro.Params.Add(parm);
                        }
                    }
                }
            }

            foreach (var line in source.Where(l => !l.IsComment))
            {
                if (object.ReferenceEquals(definition, line) ||
                    object.ReferenceEquals(closure, line)) continue;

                if ((isSegment && line.Instruction.Equals("." + name, comparer))
                    || line.Instruction.Equals("." + name, comparer))
                {
                    throw new MacroException(line, string.Format(ErrorStrings.RecursiveMacro, line.Label));
                }
                var param_ix = line.Operand.IndexOf('\\');
                if (param_ix >= 0 && isSegment == false)
                {
                    if (line.Operand.EndsWith("\\"))
                        throw new MacroException(line, ErrorStrings.MacroParamNotSpecified);

                    string param = String.Empty;
                    if (char.IsLetterOrDigit(line.Operand.ElementAt(param_ix + 1)) == false)
                    {
                        throw new MacroException(line, ErrorStrings.MacroParamIncorrect);
                    }
                    foreach (var c in line.Operand.Substring(param_ix + 1, line.Operand.Length - param_ix - 1))
                    {
                        if (Regex.IsMatch(c.ToString(), Patterns.SymbolUnicodeChar))
                            param += c;
                        else
                            break;
                    }
                    if (string.IsNullOrEmpty(param))
                    {
                        throw new MacroException(line, ErrorStrings.MacroParamNotSpecified);
                    }


                    // is the parameter in the operand a number or named
                    if (int.TryParse(param, out int paramref))
                    {
                        // if it is a number and higher than the number of explicitly
                        // defined params, just add it as a param
                        int paramcount = macro.Params.Count;
                        if (paramref > paramcount)
                        {
                            while (paramref > paramcount)
                                macro.Params.Add(new Macro.Param { Number = ++paramcount });
                        }
                        else if (paramref < 1)
                        {
                            throw new MacroException(line, string.Format(ErrorStrings.InvalidParamRef, param));
                        }
                        paramref--;
                        macro.Params[paramref].SourceLines.Add(line.SourceInfo());
                    }
                    else
                    {
                        if (macro.Params.Any(p => p.Name == param) == false)
                        {
                            throw new MacroException(line, string.Format(ErrorStrings.InvalidParamRef, param));
                        }
                        var macparm = macro.Params.First(p => p.Name == param);
                        macparm.SourceLines.Add(line.SourceInfo());
                    }
                }
                macro.Source.Add(line);
            }
            if (closure.IsComment)
                throw new MacroException(closure, string.Format(ErrorStrings.MissingClosureMacro, class_));

            if (string.IsNullOrEmpty(closure.Operand) == false)
            {
                if (isSegment && !name.Equals(closure.Operand, comparer))
                    throw new MacroException(closure, string.Format(ErrorStrings.ClosureDoesNotCloseMacro, definition.Instruction, "segment"));
                if (!isSegment)
                    throw new MacroException(closure, string.Format(ErrorStrings.DirectiveTakesNoArguments, definition.Instruction));
            }
            if (macro.IsSegment == false)
            {
                macro.Source.Add(new SourceLine
                {
                    Filename = closure.Filename,
                    LineNumber = closure.LineNumber,
                    Label = closure.Label,
                    Instruction = closeBlock
                });
            }
            return macro;
        }

        #region Static Methods

        /// <summary>
        /// Determines whether the given token is a valid macro name.
        /// </summary>
        /// <param name="token">The token to check</param>
        /// <returns><c>True</c> if the token is a valid macro name,
        /// otherwise<c>false</c>.</returns>
        public static bool IsValidMacroName(string token) =>
            Regex.IsMatch(token, "^" + Patterns.SymbolUnicodeNoLeadingUnderscore + "$");

        #endregion

        #endregion

        #region Properties

        /// <summary>
        /// Gets the list of parameters associated with the defined macro.
        /// </summary>
        public List<Param> Params { get; private set; }

        /// <summary>
        /// Gets the macro source.
        /// </summary>
        public List<SourceLine> Source { get; private set; }

        /// <summary>
        /// Gets or sets the flag that determines whether the macro definition is a segment.
        /// </summary>
        public bool IsSegment { get; set; }

        #endregion
    }
}
