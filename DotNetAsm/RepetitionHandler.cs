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
    /// Handles repetitions in assembly source.
    /// </summary>
    public sealed class RepetitionHandler : AssemblerBase, IBlockHandler
    {
        #region Private Classes

        /// <summary>
        /// A block of repetitions implemented as a linked list.
        /// </summary>
        class RepetitionBlock
        {
            /// <summary>
            /// An entry in a <see cref="T:DotNetAsm.RepetitionHandler.RepetitionBlock"/>.
            /// </summary>
            public class RepetitionEntry
            {
                /// <summary>
                /// The <see cref="T:DotNetAsm.SourceLine "/> in the block.
                /// </summary>
                public SourceLine Line { get; set; }

                /// <summary>
                /// The <see cref="T:DotNetAsm.RepetitionHandler.RepetitionBlock"/> to link to.
                /// </summary>
                public RepetitionBlock LinkedBlock { get; set; }

                /// <summary>
                /// Constructs a <see cref="T:DotNetAsm.RepetitionHandler.RepetitionBlock.RepetitionEntry"/>.
                /// </summary>
                /// <param name="line">The <see cref="T:DotNetAsm.SourceLine "/> to add. This value can
                /// be null.</param>
                /// <param name="block">The <see cref="T:DotNetAsm.RepetitionHandler.RepetitionBlock"/>
                /// to link to. This value can be null.</param>
                public RepetitionEntry(SourceLine line, RepetitionBlock block)
                {
                    if (line != null)
                        Line = line.Clone() as SourceLine;
                    else
                        Line = null;
                    LinkedBlock = block;
                }
            }

            /// <summary>
            /// Constructs a <see cref="T:DotNetAsm.RepetitionHandler.RepetitionBlock"/>.
            /// </summary>
            public RepetitionBlock()
            {
                Entries = new List<RepetitionEntry>();
                BackLink = null;
                RepeatAmounts = 0;
            }

            /// <summary>
            /// The <see cref="T:System.Collections.Generic.List&lt;DotNetAsm.RepetitionHandler.RepetitionBlock.RepetitionEntry&lt;"/>
            /// </summary>
            public List<RepetitionEntry> Entries { get; set; }

            /// <summary>
            /// A back link to a <see cref="T:DotNetAsm.RepetitionHandler.RepetitionBlock"/>.
            /// </summary>
            public RepetitionBlock BackLink { get; set; }

            /// <summary>
            /// The amount of times the block should be repeated in final assembly
            /// </summary>
            public long RepeatAmounts { get; set; }
        }

        #endregion

        #region Members

        RepetitionBlock _rootBlock;
        RepetitionBlock _currBlock;

        readonly List<SourceLine> _processedLines;

        int _levels;

        #endregion

        #region Constructors

        /// <summary>
        /// Constructs an instance of a <see cref="T:DotNetAsm.RepetitionHandler"/> object.
        /// </summary>
        /// <param name="controller">The <see cref="T:DotNetAsm.IAssemblyController"/> for this
        /// handler.</param>
        public RepetitionHandler(IAssemblyController controller) :
            base(controller)
        {
            Reserved.DefineType("Directives", ".repeat", ".endrepeat");

            _currBlock =
            _rootBlock = new RepetitionBlock();
            _levels = 0;
            _processedLines = new List<SourceLine>();
        }

        #endregion

        #region Methods

        /// <summary>
        /// Processes the <see cref="T:DotNetAsm.SourceLine"/> for repetitions, or within a repetition block.
        /// </summary>
        /// <param name="line">The <see cref="T:DotNetAsm.SourceLine"/> to process</param>
        public void Process(SourceLine line)
        {
            if (line.Instruction.Equals(".repeat", Controller.Options.StringComparison))
            {
                if (string.IsNullOrEmpty(line.Operand))
                {
                    Controller.Log.LogEntry(line, ErrorStrings.TooFewArguments, line.Instruction);
                    return;
                }
                if (string.IsNullOrEmpty(line.Label) == false)
                {
                    Controller.Log.LogEntry(line, ErrorStrings.None);
                    return;
                }

                if (_levels > 0)
                {
                    var block = new RepetitionBlock
                    {
                        BackLink = _currBlock
                    };
                    var entry = new RepetitionBlock.RepetitionEntry(null, block);
                    _currBlock.Entries.Add(entry);
                    _currBlock = block;
                }
                _levels++;
                _currBlock.RepeatAmounts = Controller.Evaluator.Eval(line.Operand, int.MinValue, uint.MaxValue);
            }
            else if (line.Instruction.Equals(".endrepeat", Controller.Options.StringComparison))
            {
                if (_levels == 0)
                {
                    Controller.Log.LogEntry(line, ErrorStrings.ClosureDoesNotCloseBlock, line.Instruction);
                }
                else if (string.IsNullOrEmpty(line.Operand) == false)
                {
                    Controller.Log.LogEntry(line, ErrorStrings.TooManyArguments, line.Instruction);
                }
                else if (string.IsNullOrEmpty(line.Label) == false)
                {
                    Controller.Log.LogEntry(line, ErrorStrings.None);
                }
                else
                {
                    _levels--;
                    _currBlock = _currBlock.BackLink;
                    if (_levels == 0)
                        ProcessLines(_rootBlock);
                }
            }
            else
            {
                var entry = new RepetitionBlock.RepetitionEntry(line, null);
                _currBlock.Entries.Add(entry);
            }
        }

        /// <summary>
        /// Perform final processing on the processed lines of the <see cref="T:DotNetAsm.RepetitionHandler"/>.
        /// </summary>
        /// <param name="block">The <see cref="T:DotNetAsm.RepetitionHandler.RepetitionBlock"/> to process</param>
        void ProcessLines(RepetitionBlock block)
        {
            for (int i = 0; i < block.RepeatAmounts; i++)
            {
                foreach (var entry in block.Entries)
                {
                    if (entry.LinkedBlock != null)
                        ProcessLines(entry.LinkedBlock);
                    else
                        _processedLines.Add(entry.Line.Clone() as SourceLine);
                }
            }
        }

        /// <summary>
        /// Reset the <see cref="T:DotNetAsm.RepetitionHandler"/>. All processed lines will be cleared.
        /// </summary>
        public void Reset()
        {
            _processedLines.Clear();
            _rootBlock.Entries.Clear();
            _rootBlock.RepeatAmounts = 0;
            _currBlock = _rootBlock;
        }

        /// <summary>
        /// Determines whether the DotNetAsm.RepetitionHandler processes the given token.
        /// </summary>
        /// <param name="token">The token to determine if is an instruction that
        /// the handler processes.</param>
        /// <returns><c>True</c> if the <see cref="T:DotNetAsm.RepetitionHandler"/> processes this token,
        /// otherwise <c>false</c>.</returns>
        public bool Processes(string token) => Reserved.IsReserved(token);

        /// <summary>
        /// Gets the flag that determines if the DotNetAsm.RepetitionHandler is currently in
        /// processing mode.
        /// </summary>
        public bool IsProcessing() => _levels > 0;

        /// <summary>
        /// Gets the processed blocks of repeated lines.
        /// </summary>
        public IEnumerable<SourceLine> GetProcessedLines() => _processedLines;

        #endregion
    }
}
