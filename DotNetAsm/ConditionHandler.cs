//-----------------------------------------------------------------------------
// Copyright (c) 2017, 2018 informedcitizenry <informedcitizenry@gmail.com>
//
// Licensed under the MIT license. See LICENSE_6502net for full license information.
//
//-----------------------------------------------------------------------------

using System.Collections.Generic;

namespace DotNetAsm
{
    /// <summary>
    /// A class that processes condition blocks inside assembly sources.
    /// </summary>
    public sealed class ConditionHandler : AssemblerBase, IBlockHandler
    {
        #region Members

        int _condLevel;
        Stack<string> _condStack;
        Stack<bool> _resultStack;
        readonly List<SourceLine> _processedLines;
        bool _doNotAsm;

        #endregion

        #region Constructors

        /// <summary>
        /// Initialize a new instance of a <see cref="T:DotNetAsm.ConditionHandler"/> class.
        /// </summary>
        /// <param name="controller">The <see cref="T:DotNetAsm.IAssemblyController"/> associated to this
        /// class.</param>
        public ConditionHandler(IAssemblyController controller)
            : base(controller)
        {
            Reserved.DefineType("Conditions",
                ".if", ".ifdef", ".ifndef",
                ".elif", ".elifdef", ".elifndef",
                ".else", ".endif"
            );
            _condLevel = 0;
            _condStack = new Stack<string>();
            _resultStack = new Stack<bool>();
            _processedLines = new List<SourceLine>();
            _doNotAsm = false;
        }

        #endregion

        #region Methods

        public bool Processes(string token) => Reserved.IsReserved(token);

        public void Process(SourceLine line)
        {
            if (!Processes(line.Instruction))
            {
                if (!_doNotAsm)
                    _processedLines.Add(line);
                return;
            }
            string lastcond = _condStack.Count > 0 ? _condStack.Peek() : string.Empty;

            if (line.Instruction.StartsWith(".if", Controller.Options.StringComparison))
            {
                _resultStack.Push(!_doNotAsm);
                _condStack.Push(line.Instruction);
            }
            else if (line.Instruction.Equals(".else", Controller.Options.StringComparison))
            {
                if (string.IsNullOrEmpty(line.Operand) == false)
                {
                    Controller.Log.LogEntry(line, ErrorStrings.TooManyArguments, line.Instruction);
                    return;
                }
                _condStack.Pop();
                _condStack.Push(line.Instruction);
            }
            else if (line.Instruction.Equals(".endif", Controller.Options.StringComparison))
            {
                // .endif
                if (_condStack.Count == 0)
                {
                    Controller.Log.LogEntry(line, ErrorStrings.ClosureDoesNotCloseBlock, line.Instruction);
                    return;
                }
                _condStack.Pop();
            }
            if (_condStack.Count > _condLevel && _doNotAsm)
                return;

            if (line.Instruction.Equals(".endif", Controller.Options.StringComparison))
            {
                _resultStack.Pop();
                _condLevel = _condStack.Count;
                _doNotAsm = false;
            }
            else
            {
                if (line.Instruction.StartsWith(".if", Controller.Options.StringComparison))
                {
                    _condLevel = _condStack.Count;

                    UpdateDoNotAsm(line);
                }
                else
                {
                    if (string.IsNullOrEmpty(lastcond) ||
                        (!line.Instruction.Equals(".endif", Controller.Options.StringComparison) &&
                        lastcond.Equals(".else", Controller.Options.StringComparison)))
                    {
                        Controller.Log.LogEntry(line, ErrorStrings.None);
                        return;
                    }

                    _doNotAsm = _resultStack.Peek();    // this can seem confusing but in this case
                    // not assembling correlates directly to the last result
                    if (_doNotAsm) return;
                    UpdateDoNotAsm(line);
                }
                _resultStack.Pop(); // in the if ... elif ... else... block change the result
                _resultStack.Push(!_doNotAsm);
            }
        }

        /// <summary>
        /// Update the flag to determine if the current block should not assemble
        /// lines within it based on the condition.
        /// </summary>
        /// <param name="line">The <see cref="T:DotNetAsm.SourceLine"/> containing the conditional
        /// directive.</param>
        void UpdateDoNotAsm(SourceLine line)
        {
            if (line.Instruction.EndsWith("if", Controller.Options.StringComparison))
                _doNotAsm = !Controller.Evaluator.EvalCondition(line.Operand);
            else if (line.Instruction.EndsWith("ifdef", Controller.Options.StringComparison))
                _doNotAsm = !(Controller.Symbols.Labels.IsSymbol(line.Operand) || Controller.Symbols.Variables.IsSymbol(line.Operand));
            else if (line.Instruction.EndsWith("ifndef", Controller.Options.StringComparison))
                _doNotAsm = Controller.Symbols.Labels.IsSymbol(line.Operand) || Controller.Symbols.Variables.IsSymbol(line.Operand);
        }

        public void Reset()
        {
            _doNotAsm = false;
            _condLevel = 0;
            _condStack.Clear();
            _resultStack.Clear();
            _processedLines.Clear();
        }

        public bool IsProcessing() => _condLevel > 0;

        public IEnumerable<SourceLine> GetProcessedLines() => _processedLines;

        #endregion
    }
}
