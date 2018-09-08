//-----------------------------------------------------------------------------
// Copyright (c) 2017, 2018 informedcitizenry <informedcitizenry@gmail.com>
//
// Licensed under the MIT license. See LICENSE_6502net for full license information.
//
//-----------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace DotNetAsm
{
    /// <summary>
    /// Handler for the DisplayBanner event.
    /// </summary>
    public delegate string DisplayBannerEventHandler(object sender, bool showVersion);

    /// <summary>
    /// Handler for the WriteBytes event.
    /// </summary>
    public delegate byte[] WriteBytesEventHandler(object sender);

    /// <summary>
    /// Represents an error that occurs when an undefined symbol is referenced.
    /// </summary>
    public class SymbolNotDefinedException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:DotNetAsm.SymbolNotDefinedException"/> class.
        /// </summary>
        /// <param name="symbol">Symbol.</param>
        public SymbolNotDefinedException(string symbol)
        {
            Symbol = symbol;
        }

        /// <summary>
        /// Gets the undefined symbol.
        /// </summary>
        /// <value>The symbol name.</value>
        public string Symbol { get; private set; }
    }

    /// <summary>
    /// Implements an assembly controller to process source input and convert
    /// to assembled output.
    /// </summary>
    public class AssemblyController : AssemblerBase, IAssemblyController
    {
        #region Members

        protected Preprocessor _preprocessor;
        Stack<ILineAssembler> _assemblers;
        List<IBlockHandler> _blockHandlers;
        List<SourceLine> _processedLines;
        SourceLine _currentLine;

        int _passes;

        Regex _specialLabels;
        string _localLabelScope;

        #endregion

        #region Constructors

        /// <summary>
        /// Constructs an instance of a <see cref="T:DotNetAsm.AssemblyController"/>, which controls the
        /// assembly process.
        /// </summary>
        /// <param name="args">The array of <see cref="T:System.String"/> args passed by the commandline.</param>
        public AssemblyController(string[] args)
        {
            Controller = this;

            Options = new AsmCommandLineOptions();
            Options.ProcessArgs(args);

            Reserved.Comparer = Options.StringComparar;

            Reserved.DefineType("Directives",
                    ".cpu", ".endrelocate", ".equ", ".pseudopc", ".realpc", ".relocate", ".end",
                    ".endrepeat", ".proff", ".pron", ".repeat", ConstStrings.VAR_DIRECTIVE
                );

            Reserved.DefineType("Functions",
                     "abs", "acos", "asin", "atan", "cbrt", "ceil", "cos", "cosh", "count", "deg",
                     "exp", "floor", "frac", "hypot", "ln", "log10", "pow", "rad", "random",
                     "round", "sgn", "sin", "sinh", "sizeof", "sqrt", "tan", "tanh", "trunc",
                     "str", "format"
                );

            Reserved.DefineType("UserDefined");


            Log = new ErrorLog();

            _processedLines = new List<SourceLine>();

            Output = new Compilation(!Options.BigEndian);

            _specialLabels = new Regex(@"^\*|\+|-$", RegexOptions.Compiled);

            Encoding = new AsmEncoding(Options.CaseSensitive);

            Evaluator = new Evaluator(Options.CaseSensitive);

            Evaluator.DefineSymbolLookup(SymbolsToValues);

            Symbols = new SymbolManager(this);
            _localLabelScope = string.Empty;

            _preprocessor = new Preprocessor(this, s => IsSymbolName(s.TrimEnd(':'), true, false));
            _assemblers = new Stack<ILineAssembler>();
            _assemblers.Push(new PseudoAssembler(this, arg =>
                {
                    return IsReserved(arg) || Symbols.Labels.IsScopedSymbol(arg, _currentLine.Scope);
                }));

            _assemblers.Push(new MiscAssembler(this));

            _blockHandlers = new List<IBlockHandler>
            {
                new ConditionHandler(this),
                new MacroHandler(this, IsInstruction),
                new ForNextHandler(this),
                new RepetitionHandler(this),
                new ScopeBlockHandler(this)
            };
            Disassembler = new Disassembler(this);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Checks if a given token is actually an instruction or directive, either
        /// for the <see cref="T:DotNetAsm.AssemblyController"/> or any line assemblers.
        /// </summary>
        /// <param name="token">The token to check</param>
        /// <returns><c>True</c> if the token is an instruction or directive, otherwise <c>false</c>.</returns>
        public bool IsInstruction(string token) => Reserved.IsOneOf("Directives", token) ||
                                                    _preprocessor.IsReserved(token) ||
                                                    _blockHandlers.Any(handler => handler.Processes(token)) ||
                                                    _assemblers.Any(assembler => assembler.AssemblesInstruction(token));

        /// <summary>
        /// Determines whether the token is a reserved keyword, such as an instruction
        /// or assembler directive, or a user-defined reserved word.
        /// </summary>
        /// <param name="token">The token to test.</param>
        /// <returns>True, if the token is a reserved word, otherwise false.</returns>
        public override bool IsReserved(string token) => IsInstruction(token) || Reserved.IsReserved(token);

        bool IsSymbolName(string token, bool allowLeadUnderscore = true, bool allowDot = true)
        {
            // empty string
            if (string.IsNullOrEmpty(token))
                return false;

            // is a reserved word
            if (IsReserved(token))
                return false;

            // if leading underscore not allowed
            if (!allowLeadUnderscore && token.StartsWith("_", Options.StringComparison))
                return false;

            // if no dots allowed or trailing dot
            if (token.Contains(".") && (!allowDot || token.EndsWith(".", Options.StringComparison)))
                return false;

            // otherwise...
            return Symbols.Labels.IsSymbolValid(token, true);
        }

        string SymbolsToValues(string expression)
            => Symbols.TranslateExpressionSymbols(_currentLine, expression, _localLabelScope, _passes > 0);

        /// <remarks>
        /// Define macros and segments, and add included source files.
        /// </remarks>
        protected virtual IEnumerable<SourceLine> Preprocess()
        {
            var source = new List<SourceLine>();

            source.AddRange(ProcessDefinedLabels());
            foreach (var file in Options.InputFiles)
            {
                source.AddRange(_preprocessor.ConvertToSource(file));

                if (Log.HasErrors)
                    break;
            }

            if (Log.HasErrors == false)
            {
                source.ForEach(line =>
                    line.Operand = Regex.Replace(line.Operand, @"\s?\*\s?", "*"));

                return source;
            }
            if (!string.IsNullOrEmpty(Options.CPU))
                OnCpuChanged(new SourceLine { SourceString = ConstStrings.COMMANDLINE_ARG, Operand = Options.CPU });

            return null;
        }


        /// <remarks>
        /// Add labels defined with command-line -D option
        /// </remarks>
        protected IEnumerable<SourceLine> ProcessDefinedLabels()
        {
            var labels = new List<SourceLine>();

            foreach (var label in Options.LabelDefines)
            {
                string name = label;
                string definition = "1";

                if (label.Contains("="))
                {
                    var def = label.Split(new string[] { "=" }, StringSplitOptions.RemoveEmptyEntries);

                    if (def.Count() != 2)
                        throw new Exception("Bad argument in label definition '" + label + "'");

                    name = def.First(); definition = def.Last();
                }

                if (IsSymbolName(name, false, false) == false)
                    throw new Exception(string.Format(ErrorStrings.LabelNotValid, name));

                labels.Add(new SourceLine
                {
                    Label = name,
                    Instruction = "=",
                    Operand = definition,
                    SourceString = string.Format($"{name}={definition} ;-D {label}", name, definition, label)
                });
            }
            return labels;
        }

        protected void OnCpuChanged(SourceLine line)
        {
            if (CpuChanged != null)
                CpuChanged.Invoke(new CpuChangedEventArgs { Line = line });
            else
                Log.LogEntry(line, ErrorStrings.UnknownInstruction, line.Instruction);
        }

        protected void FirstPass(IEnumerable<SourceLine> source)
        {
            _passes = 0;
            int id = 1;

            var sourceList = source.ToList();

            for (int i = 0; i < sourceList.Count; i++)
            {
                _currentLine = sourceList[i];
                try
                {
                    if (_currentLine.DoNotAssemble)
                    {
                        if (_currentLine.IsComment)
                            _processedLines.Add(_currentLine);
                        continue;
                    }
                    if (_currentLine.Instruction.Equals(".end", Options.StringComparison))
                        break;

                    var currentHandler = _blockHandlers.FirstOrDefault(h => h.IsProcessing());
                    if (currentHandler == null)
                        currentHandler = _blockHandlers.FirstOrDefault(h => h.Processes(_currentLine.Instruction));
                    if (currentHandler != null)
                    {
                        sourceList.RemoveAt(i--);

                        currentHandler.Process(_currentLine);
                        if (currentHandler.IsProcessing() == false)
                        {
                            sourceList.InsertRange(i + 1, currentHandler.GetProcessedLines());
                            currentHandler.Reset();
                        }
                    }
                    else
                    {
                        _currentLine.Id = id++;
                        FirstPassLine();
                    }
                }
                catch (SymbolCollectionException symExc)
                {
                    if (symExc.Reason == SymbolCollectionException.ExceptionReason.SymbolExists)
                        Log.LogEntry(_currentLine, ErrorStrings.LabelRedefinition, _currentLine.Label);
                    else
                        Log.LogEntry(_currentLine, ErrorStrings.LabelNotValid, _currentLine.Label);
                }
                catch (SymbolNotDefinedException symNotFound)
                {
                    Log.LogEntry(_currentLine, ErrorStrings.LabelNotDefined, symNotFound.Symbol);
                }
                catch (ExpressionException exprEx)
                {
                    Log.LogEntry(_currentLine, exprEx.Message);
                }
                catch (Exception)
                {
                    Log.LogEntry(_currentLine, ErrorStrings.None);
                }
            }

            if (_blockHandlers.Any(h => h.IsProcessing()))
                Log.LogEntry(_processedLines.Last(), ErrorStrings.MissingClosure);
        }

        void FirstPassLine()
        {
            try
            {
                if (_currentLine.Instruction.Equals(".cpu", Options.StringComparison))
                {
                    if (!_currentLine.Operand.EnclosedInQuotes())
                        Controller.Log.LogEntry(_currentLine, ErrorStrings.QuoteStringNotEnclosed);
                    else
                        OnCpuChanged(_currentLine);
                    return;
                }
                if (_currentLine.Instruction.Equals(ConstStrings.VAR_DIRECTIVE, Options.StringComparison))
                {
                    var varname = Symbols.Variables.GetVariableFromExpression(_currentLine.Operand, _currentLine.Scope);
                    try
                    {
                        var varAssignment = Symbols.Variables.SetVariable(_currentLine.Operand, _currentLine.Scope).Value;
                        _currentLine.PC = Evaluator.Eval(varAssignment);
                    }
                    catch (SymbolNotDefinedException)
                    {
                        Symbols.Variables.SetSymbol(varname, 0, false);
                        _currentLine.PC = 0;
                    }
                }

                UpdatePC();

                _currentLine.PC = Output.LogicalPC;

                DefineLabel();

                if (!string.IsNullOrEmpty(_currentLine.Label) &&
                    !_currentLine.Label.Equals("*") &&
                    _currentLine.SourceString.StartsWith(" ", Options.StringComparison) &&
                    Options.WarnLeft)
                    Log.LogEntry(_currentLine, ErrorStrings.LabelNotLeft, Options.WarningsAsErrors);

                if (!IsAssignmentDirective())
                    Output.AddUninitialized(GetInstructionSize());
            }
            catch (Exception ex)
            {
                // most exceptions resulting from calculations we don't care
                // about until final pass, since they are subject to correction
                if (ex is DivideByZeroException ||
                    ex is Compilation.InvalidPCAssignmentException ||
                    ex is OverflowException)
                { } // do nothing
                else
                    throw;
            }
            finally
            {
                // always add
                _processedLines.Add(_currentLine);
            }

        }

        bool SecondPassLine(bool finalPass)
        {
            UpdatePC();
            bool passNeeded = false;

            string label = string.Empty;
            if (!string.IsNullOrEmpty(_currentLine.Label))
            {
                if (_currentLine.Label.First() == '_')
                    label = string.Concat(_localLabelScope, _currentLine.Label);
                else if (!_currentLine.Label.Equals("*") &&
                         !_currentLine.Label.Equals("-") &&
                         !_currentLine.Label.Equals("+"))
                    label = _localLabelScope = _currentLine.Label;
            }

            if (IsAssignmentDirective())
            {
                if (_currentLine.Label.Equals("*")) return false;

                long val = long.MinValue;

                // for .vars initialization is optional
                if (string.IsNullOrEmpty(_currentLine.Operand) == false)
                    val = Evaluator.Eval(_currentLine.Operand, int.MinValue, uint.MaxValue);

                if (_currentLine.Label.Equals("-") || _currentLine.Label.Equals("+"))
                {
                    passNeeded = val != _currentLine.PC;
                }
                else
                {
                    if (val.Equals(long.MinValue))
                    {
                        Controller.Log.LogEntry(_currentLine, ErrorStrings.TooFewArguments, _currentLine.Instruction);
                        return false;
                    }
                    passNeeded = val != Symbols.Labels.GetScopedSymbolValue(label, _currentLine.Scope);
                    Symbols.Labels.SetLabel(_currentLine.Scope + label, val, false, false);

                }
                _currentLine.PC = val;
            }
            else if (_currentLine.Instruction.Equals(ConstStrings.VAR_DIRECTIVE, Options.StringComparison))
            {
                var varparts = Symbols.Variables.SetVariable(_currentLine.Operand, _currentLine.Scope);
                passNeeded = _currentLine.PC != Evaluator.Eval(varparts.Value);
                _currentLine.PC = Evaluator.Eval(varparts.Value);
            }
            else if (_currentLine.Instruction.Equals(".cpu", Options.StringComparison))
            {
                OnCpuChanged(_currentLine);
            }
            else
            {
                if (Symbols.Labels.IsScopedSymbol(label, _currentLine.Scope))
                    Symbols.Labels.SetLabel(_currentLine.Scope + label, Output.LogicalPC, false, false);
                passNeeded = _currentLine.PC != Output.LogicalPC;
                _currentLine.PC = Output.LogicalPC;
                if (finalPass)
                    AssembleLine();
                else
                    Output.AddUninitialized(GetInstructionSize());
            }
            return passNeeded;
        }

        protected void SecondPass()
        {
            const int MAX_PASSES = 4;
            bool passNeeded = true;
            bool finalPass = false;
            _passes++;

            var assembleLines = _processedLines.Where(l => l.DoNotAssemble == false);

            while (_passes <= MAX_PASSES && Log.HasErrors == false)
            {
                passNeeded = false;
                Output.Reset();

                Symbols.Variables.Clear();

                _localLabelScope = string.Empty;

                foreach (SourceLine line in assembleLines)
                {
                    try
                    {
                        if (line.Instruction.Equals(".end", Options.StringComparison))
                            break;

                        _currentLine = line;

                        var needpass = SecondPassLine(finalPass);
                        if (!passNeeded)
                            passNeeded = needpass;
                    }
                    catch (SymbolNotDefinedException symEx)
                    {
                        Log.LogEntry(line, ErrorStrings.LabelNotDefined, symEx.Symbol);
                    }
                    catch (ExpressionException exprEx)
                    {
                        Log.LogEntry(line, ErrorStrings.BadExpression, exprEx.Message);
                    }
                    catch (OverflowException overflowEx)
                    {
                        if (finalPass)
                            Log.LogEntry(line, ErrorStrings.IllegalQuantity, overflowEx.Message);
                    }
                    catch (Compilation.InvalidPCAssignmentException ex)
                    {
                        if (finalPass)
                            Log.LogEntry(line, ErrorStrings.InvalidPCAssignment, ex.Message);
                    }
                    catch (Exception ex)
                    {
                        Log.LogEntry(line, ex.Message);
                    }
                }
                if (finalPass)
                    break;
                _passes++;
                finalPass = !passNeeded;
            }
            if (_passes > MAX_PASSES)
                throw new Exception("Too many passes attempted.");
        }

        public void AddAssembler(ILineAssembler lineAssembler) => _assemblers.Push(lineAssembler);

        public void AddSymbol(string symbol) => Reserved.AddWord("UserDefined", symbol);

        public void AssembleLine()
        {
            if (string.IsNullOrEmpty(_currentLine.Instruction))
            {
                if (!string.IsNullOrEmpty(_currentLine.Operand))
                    Log.LogEntry(_currentLine, ErrorStrings.None);
                return;
            }

            if (IsInstruction(_currentLine.Instruction) == false)
            {
                Log.LogEntry(_currentLine, ErrorStrings.UnknownInstruction, _currentLine.Instruction);
            }
            else
            {
                var asm = _assemblers.FirstOrDefault(a => a.AssemblesInstruction(_currentLine.Instruction));
                asm?.AssembleLine(_currentLine);
            }
        }

        /// <remarks>
        /// This does a quick and "dirty" look at instructions. It will catch
        /// some but not all syntax errors, concerned mostly with the probable
        /// size of the instruction.
        /// </remarks>
        int GetInstructionSize()
        {
            try
            {
                var asm = _assemblers.FirstOrDefault(a => a.AssemblesInstruction(_currentLine.Instruction));
                var size = (asm != null) ? asm.GetInstructionSize(_currentLine) : 0;
                return size;
            }
            catch (SymbolNotDefinedException)
            {
                return 0;
            }
        }

        void DefineLabel()
        {
            if (!string.IsNullOrEmpty(_currentLine.Label))
            {
                if (_currentLine.Label.Equals("*"))
                    return;

                if (_specialLabels.IsMatch(_currentLine.Label))
                {
                    if (IsAssignmentDirective())
                    {
                        try
                        {
                            _currentLine.PC = Convert.ToInt32(Evaluator.Eval(_currentLine.Operand));
                        }
                        catch (SymbolNotDefinedException)
                        {
                            _currentLine.PC = 0;
                        }
                    }
                    else
                    {
                        _currentLine.PC = Output.LogicalPC;
                    }

                    Symbols.AddAnonymousLine(_currentLine);
                }
                else
                {
                    string scopedLabel = string.Empty;

                    _currentLine.Label = _currentLine.Label.TrimEndOnce(':');

                    if (Reserved.IsReserved(_currentLine.Label) ||
                        IsInstruction(_currentLine.Label) ||
                        _currentLine.Label.Contains("."))
                    {
                        Log.LogEntry(_currentLine, ErrorStrings.LabelNotValid, _currentLine.Label);
                        return;
                    }

                    if (_currentLine.Label.First() == '_')
                    {
                        scopedLabel = _currentLine.Scope + _localLabelScope + _currentLine.Label;
                    }
                    else
                    {
                        _localLabelScope = _currentLine.Label;
                        scopedLabel = _currentLine.Scope + _currentLine.Label;
                    }


                    long val = _currentLine.PC;
                    if (IsAssignmentDirective())
                    {
                        try
                        {
                            val = Evaluator.Eval(_currentLine.Operand, int.MinValue, uint.MaxValue);
                        }
                        catch (SymbolNotDefinedException)
                        {
                            val = 0;
                        }
                    }
                    Symbols.Labels.SetLabel(scopedLabel, val, false, true);
                }
            }
        }

        // Are we updating the program counter?
        void UpdatePC()
        {
            long val = 0;
            if (_currentLine.Label.Equals("*"))
            {
                if (IsAssignmentDirective())
                {
                    try
                    {
                        val = Evaluator.Eval(_currentLine.Operand, UInt16.MinValue, UInt16.MaxValue);
                    }
                    catch (SymbolNotDefinedException)
                    {
                    }
                    Output.SetPC(Convert.ToUInt16(val));
                }
                else
                {
                    Log.LogEntry(_currentLine, ErrorStrings.None);
                }
                return;
            }
            string instruction = Options.CaseSensitive ? _currentLine.Instruction :
                _currentLine.Instruction.ToLower();

            if (instruction.Equals(".relocate") || instruction.Equals(".pseudopc"))
            {
                if (string.IsNullOrEmpty(_currentLine.Operand))
                {
                    Log.LogEntry(_currentLine, ErrorStrings.TooFewArguments, _currentLine.Instruction);
                    return;
                }
                try
                {
                    val = Evaluator.Eval(_currentLine.Operand, uint.MinValue, uint.MaxValue);
                }
                catch (SymbolNotDefinedException)
                {
                }
                Output.SetLogicalPC(Convert.ToUInt16(val));
            }
            else if (instruction.Equals(".endrelocate") || instruction.Equals(".realpc"))
            {
                if (string.IsNullOrEmpty(_currentLine.Operand) == false)
                {
                    Log.LogEntry(_currentLine, ErrorStrings.TooManyArguments, _currentLine.Instruction);
                    return;
                }
                Output.SynchPC();
            }
        }

        bool IsAssignmentDirective()
        {
            if (_currentLine.Operand.EnclosedInQuotes())
                return false; // define a constant string??

            if (_currentLine.Instruction.Equals("=") ||
                _currentLine.Instruction.Equals(".equ", Options.StringComparison))
                return true;

            return false;
        }

        void PrintStatus(DateTime asmTime)
        {
            if (Log.HasWarnings && !Options.NoWarnings)
            {
                Console.WriteLine();
                Log.DumpWarnings();
            }
            if (Log.HasErrors == false)
            {
                Console.WriteLine("\n*********************************");
                Console.WriteLine("Assembly start: ${0:X4}", Output.ProgramStart);
                Console.WriteLine("Assembly end:   ${0:X4}", Output.ProgramEnd);
                Console.WriteLine("Passes: {0}", _passes);
            }
            else
            {
                Log.DumpErrors();
            }

            Console.WriteLine("Number of errors: {0}", Log.ErrorCount);
            Console.WriteLine("Number of warnings: {0}", Log.WarningCount);

            if (Log.HasErrors == false)
            {
                var ts = DateTime.Now.Subtract(asmTime);

                Console.WriteLine("{0} bytes, {1} sec.",
                                    Output.GetCompilation().Count,
                                    ts.TotalSeconds);
                Console.WriteLine("*********************************");
                Console.WriteLine("Assembly completed successfully.");
            }
        }

        void ToListing()
        {
            if (string.IsNullOrEmpty(Options.ListingFile) && string.IsNullOrEmpty(Options.LabelFile))
                return;

            string listing;

            if (!string.IsNullOrEmpty(Options.ListingFile))
            {
                listing = GetListing();
                using (StreamWriter writer = new StreamWriter(Options.ListingFile))
                {
                    var exec = Path.GetFileName(System.Reflection.Assembly.GetEntryAssembly().Location);
                    var argstring = string.Join(" ", Options.Arguments);
                    var bannerstring = DisplayingBanner != null ? DisplayingBanner.Invoke(this, false) : string.Empty;

                    writer.WriteLine(";; {0}", bannerstring.Split(new char[] { '\n', '\r' }).First());
                    writer.WriteLine(";; {0} {1}", exec, argstring);
                    writer.WriteLine(";; {0:f}\n", DateTime.Now);
                    writer.WriteLine(";; Input files:\n");

                    _preprocessor.FileRegistry.ToList().ForEach(f => writer.WriteLine(";; {0}", f));

                    writer.WriteLine();
                    writer.Write(listing);
                }
            }
            if (!string.IsNullOrEmpty(Options.LabelFile))
            {
                listing = GetLabelsAndVariables();
                using (StreamWriter writer = new StreamWriter(Options.LabelFile, false))
                {
                    writer.WriteLine(";; Input files:\n");
                    _preprocessor.FileRegistry.ToList().ForEach(f => writer.WriteLine(";; {0}", f));
                    writer.WriteLine();
                    writer.WriteLine(listing);
                }
            }
        }

        string GetSymbolListing(string symbol, long value, bool isVar)
        {
            var symbolname = Regex.Replace(symbol, @"(?<=^|\.)[0-9]+(?=\.|$)", "::");
            var maxlen = symbolname.Length > 30 ? 30 : symbolname.Length;
            if (maxlen < 0) maxlen++;
            symbolname = symbolname.Substring(symbolname.Length - maxlen, maxlen);
            var size = value.Size() * 2;
            var assignsym = isVar ? ":=" : " =";

            return string.Format("{0,-30} {1} ${2,-4:x" + size.ToString() + "} : ({2}){3}",
                                symbolname,
                                assignsym,
                                value,
                                Environment.NewLine);
        }

        /// <remarks>
        /// Used by the ToListing method to get a listing of all defined labels.
        /// </remarks>
        string GetLabelsAndVariables()
        {
            var listing = new StringBuilder();

            foreach (var label in Symbols.Labels)
                listing.Append(GetSymbolListing(label.Key, label.Value, false));

            foreach (var variable in Symbols.Variables)
                listing.Append(GetSymbolListing(variable.Key, variable.Value, true));

            return listing.ToString();
        }

        /// <remarks>
        /// Used by the ToListing method to get the full listing.</remarks>
        protected string GetListing()
        {
            var listing = new StringBuilder();

            _processedLines.ForEach(l => Disassembler.DisassembleLine(l, listing));

            if (listing.ToString().EndsWith(Environment.NewLine, Options.StringComparison))
                return listing.ToString().Substring(0, listing.Length - Environment.NewLine.Length);

            return listing.ToString();
        }

        void SaveOutput()
        {
            if (!Options.GenerateOutput)
                return;

            var outputfile = Options.OutputFile;
            if (string.IsNullOrEmpty(Options.OutputFile))
                outputfile = "a.out";

            using (BinaryWriter writer = new BinaryWriter(new FileStream(outputfile, FileMode.Create, FileAccess.Write)))
            {
                if (WritingHeader != null)
                    writer.Write(WritingHeader.Invoke(this));

                writer.Write(Output.GetCompilation().ToArray());

                if (WritingFooter != null)
                    writer.Write(WritingFooter.Invoke(this));
            }
        }

        public void Assemble()
        {
            if (Options.InputFiles.Count == 0)
                return;

            if (Options.PrintVersion && DisplayingBanner != null)
            {
                Console.WriteLine(DisplayingBanner.Invoke(this, true));
                if (Options.ArgsPassed > 1)
                    Console.WriteLine("Additional options ignored.");
                return;
            }

            if (Options.Quiet)
                Console.SetOut(TextWriter.Null);

            if (DisplayingBanner != null)
                Console.WriteLine(DisplayingBanner.Invoke(this, false));

            DateTime asmTime = DateTime.Now;

            var source = Preprocess();

            if (Log.HasErrors == false)
            {
                FirstPass(source);

                if (Log.HasErrors == false)
                {
                    SecondPass();

                    if (Log.HasErrors == false)
                    {
                       SaveOutput();

                       ToListing();
                    }
                }
            }
            PrintStatus(asmTime);
        }

        #endregion

        #region Properties

        public AsmCommandLineOptions Options { get; private set; }

        public Compilation Output { get; private set; }

        public AsmEncoding Encoding { get; private set; }

        public ErrorLog Log { get; private set; }

        public ISymbolManager Symbols { get; private set; }

        public IEvaluator Evaluator { get; private set; }

        public ILineDisassembler Disassembler { get; set; }

        #endregion

        #region Events

        public event CpuChangeEventHandler CpuChanged;

        public event DisplayBannerEventHandler DisplayingBanner;

        public event WriteBytesEventHandler WritingHeader;

        public event WriteBytesEventHandler WritingFooter;

        #endregion
    }
}
