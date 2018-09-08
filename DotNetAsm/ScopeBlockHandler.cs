//-----------------------------------------------------------------------------
// Copyright (c) 2017, 2018 informedcitizenry <informedcitizenry@gmail.com>
//
// Licensed under the MIT license. See LICENSE_6502net for full license information.
//
//-----------------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DotNetAsm
{
    /// <summary>
    /// Handles all scoped blocks.
    /// </summary>
    public sealed class ScopeBlockHandler : AssemblerBase, IBlockHandler
    {
        #region Members

        int _anon;
        readonly Stack<string> _scope;
        readonly List<SourceLine> _processedLines;

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="T:DotNetAsm.ScopeBlockHandler"/> class.
        /// </summary>
        /// <param name="controller">The <see cref="T:DotNetAsm.IAssemblyController"/> of the handler.</param>
        public ScopeBlockHandler(IAssemblyController controller)
            : base(controller)
        {
            Reserved.DefineType("Scoped", ConstStrings.OPEN_SCOPE, ConstStrings.CLOSE_SCOPE);
            _scope = new Stack<string>();
            _processedLines = new List<SourceLine>();
            _anon = 0;
        }

        public IEnumerable<SourceLine> GetProcessedLines() => _processedLines;

        public bool IsProcessing() => _scope.Count > 0;

        public void Process(SourceLine line)
        {
            var rev = _scope.Reverse();
            var scopeBuilder = new StringBuilder(line.Scope);
            foreach (string s in rev)
                scopeBuilder.AppendFormat("{0}.", s);

            line.Scope = scopeBuilder.ToString();

            if (line.Instruction.Equals(ConstStrings.OPEN_SCOPE, Controller.Options.StringComparison))
            {
                if (string.IsNullOrEmpty(line.Label))
                    _scope.Push((_anon++).ToString());
                else
                    _scope.Push(line.Label);
            }
            else if (line.Instruction.Equals(ConstStrings.CLOSE_SCOPE, Controller.Options.StringComparison))
            {
                if (_scope.Count == 0)
                {
                    Controller.Log.LogEntry(line, ErrorStrings.ClosureDoesNotCloseBlock, line.Instruction);
                    return;
                }
                _scope.Pop();
            }

            var clone = line.Clone() as SourceLine;
            if (Reserved.IsReserved(clone.Instruction))
            {
                clone.SourceString = clone.Label;
                clone.Instruction = string.Empty;
            }
            _processedLines.Add(clone);
        }

        public bool Processes(string token) => Reserved.IsReserved(token);

        public void Reset()
        {
            _scope.Clear();
            _processedLines.Clear();
        }
    }
}
