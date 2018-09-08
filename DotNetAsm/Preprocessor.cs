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

namespace DotNetAsm
{
    /// <summary>
    /// A preprocessor class for the 6502.Net assembler.
    /// </summary>
    public sealed class Preprocessor : AssemblerBase
    {
        #region Members

        Func<string, bool> _symbolNameFunc;

        #endregion

        #region Constructors

        /// <summary>
        /// Constructs a Preprocessor object.
        /// </summary>
        /// <param name="controller">The assembly controller.</param>
        /// <param name="checkSymbol">A function to check for symbols such as labels or variables.</param>
        public Preprocessor(IAssemblyController controller,
                            Func<string, bool> checkSymbol)
            : base(controller)
        {
            FileRegistry = new HashSet<string>();
            _symbolNameFunc = checkSymbol;
            Reserved.DefineType("Directives", ".binclude", ".include", ".comment",  ".endcomment");
        }

        #endregion

        #region Methods

        /// <summary>
        /// Preprocess all comment blocks, macro and segment definitions.
        /// </summary>
        /// <param name="sourcelines">The <see cref="T:System.Collections.Generic.IEnumerable&lt;DotNetAsm.SourceLine&gt;"/>collection</param>
        /// <returns></returns>
        public IEnumerable<SourceLine> Preprocess(IEnumerable<SourceLine> sourcelines)
        {
            // we can't do this check until all commenting has been processed
            ProcessCommentBlocks(sourcelines);
            return ProcessIncludes(sourcelines.Where(l => !l.IsComment));
        }

        /// <summary>
        /// Process Includes as source.
        /// </summary>
        /// <param name="listing">The source listing containing potential ".include" directives.</param>
        /// <returns>Returns the new source with the included sources expanded.</returns>
        IEnumerable<SourceLine> ProcessIncludes(IEnumerable<SourceLine> listing)
        {
            var includedLines = new List<SourceLine>();
            foreach (var line in listing)
            {
                if (line.Instruction.Equals(".include", Controller.Options.StringComparison))
                {
                    string filename = line.Operand;
                    if (filename.EnclosedInQuotes() == false)
                    {
                        Controller.Log.LogEntry(line, ErrorStrings.FilenameNotSpecified);
                        continue;
                    }
                    var inclistings = ConvertToSource(filename.TrimOnce('"'));
                    includedLines.AddRange(inclistings);
                }
                else if (line.Instruction.Equals(".binclude", Controller.Options.StringComparison))
                {
                    var args = line.Operand.CommaSeparate();
                    var openblock = new SourceLine();
                    if (args.Count > 1)
                    {
                        if (string.IsNullOrEmpty(line.Label) == false && _symbolNameFunc(line.Label) == false)
                        {
                            Controller.Log.LogEntry(line, ErrorStrings.LabelNotValid, line.Label);
                            continue;
                        }
                        if (line.Operand.EnclosedInQuotes() == false)
                        {
                            Controller.Log.LogEntry(line, ErrorStrings.None);
                            continue;
                        }
                        if (FileRegistry.Add(line.Operand.TrimOnce('"')) == false)
                        {
                            throw new Exception(string.Format(ErrorStrings.FilePreviouslyIncluded, line.Operand));
                        }
                        openblock.Label = line.Label;
                    }
                    else if (line.Operand.EnclosedInQuotes() == false)
                    {
                        Controller.Log.LogEntry(line, ErrorStrings.FilenameNotSpecified);
                        continue;
                    }
                    openblock.Instruction = ConstStrings.OPEN_SCOPE;
                    includedLines.Add(openblock);
                    var inclistings = ConvertToSource(line.Operand.TrimOnce('"'));

                    includedLines.AddRange(inclistings);
                    includedLines.Add(new SourceLine());
                    includedLines.Last().Instruction = ConstStrings.CLOSE_SCOPE;
                }
                else
                {
                    includedLines.Add(line);
                }
            }
            return includedLines;
        }

        /// <summary>
        /// Marks all SourceLines within comment blocks as comments.
        /// </summary>
        /// <param name="source">The source listing.</param>
        void ProcessCommentBlocks(IEnumerable<SourceLine> source)
        {
            bool incomments = false;
            foreach(var line in source)
            {
                if (!incomments)
                    incomments = line.Instruction.Equals(".comment", Controller.Options.StringComparison);

                line.IsComment = incomments;
                if (line.Instruction.Equals(".endcomment", Controller.Options.StringComparison))
                {
                    if (incomments)
                        incomments = false;
                    else
                        Controller.Log.LogEntry(line, ErrorStrings.ClosureDoesNotCloseBlock, line.Instruction);
                }
            }
            if (incomments)
                throw new Exception(ErrorStrings.MissingClosure);
        }

        /// <summary>
        /// Converts a file to a SourceLine list.
        /// </summary>
        /// <param name="file">The filename.</param>
        /// <returns>A <see cref="T:System.Collections.Generic.IEnumerable&lt;DotNetAsm.SourceLine&gt;"/> d.</returns>
        public IEnumerable<SourceLine> ConvertToSource(string file)
        {
            if (FileRegistry.Add(file) == false)
                throw new Exception(string.Format(ErrorStrings.FilePreviouslyIncluded, file));

            if (File.Exists(file))
            {
                Console.WriteLine("Processing input file " + file + "...");
                int currentline = 1;
                var sourcelines = new List<SourceLine>();
                using (StreamReader reader = new StreamReader(File.Open(file, FileMode.Open)))
                {
                    while (reader.EndOfStream == false)
                    {
                        var unprocessedline = reader.ReadLine();
                        try
                        {
                            var line = new SourceLine(file, currentline, unprocessedline);
                            line.Parse(
                                delegate(string token)
                                {
                                    return Controller.IsInstruction(token) || Reserved.IsReserved(token) ||
                                        (token.StartsWith(".") && Macro.IsValidMacroName(token.Substring(1))) ||
                                        token == "=";
                                });
                            sourcelines.Add(line);
                        }
                        catch (Exception ex)
                        {
                            Controller.Log.LogEntry(file, currentline, ex.Message);
                        }
                        currentline++;
                    }
                    sourcelines = Preprocess(sourcelines).ToList();
                }
                return sourcelines;
            }
            throw new FileNotFoundException(string.Format("Unable to open source file \"{0}\"", file));
        }

        public override bool IsReserved(string token) => Reserved.IsReserved(token);

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the file registry of input files.
        /// </summary>
        public HashSet<string> FileRegistry { get; set; }

        #endregion
    }
}
