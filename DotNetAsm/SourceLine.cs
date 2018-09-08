//-----------------------------------------------------------------------------
// Copyright (c) 2017, 2018 informedcitizenry <informedcitizenry@gmail.com>
//
// Licensed under the MIT license. See LICENSE_6502net for full license information.
//
//-----------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Text;

namespace DotNetAsm
{
    /// <summary>
    /// Encapsulates a single line of assembly source.
    /// </summary>
    public sealed class SourceLine : IEquatable<SourceLine>, ICloneable
    {
        #region Members

        bool _doNotAssemble;

        bool _comment;

        #endregion

        #region Constructors

        /// <summary>
        /// Constructs an instance of a <see cref="T:DotNetAsm.SourceLine"/> object.
        /// </summary>
        /// <param name="filename">The original source filename.</param>
        /// <param name="linenumber">The original source line number.</param>
        /// <param name="source">The unprocessed source string.</param>
        public SourceLine(string filename, int linenumber, string source)
        {
            Assembly = new List<byte>();
            Filename = filename;
            LineNumber = linenumber;
            Scope =
            Label =
            Instruction =
            Operand =
            Disassembly = string.Empty;
            SourceString = source;
        }

        /// <summary>
        /// Constructs an instance of a <see cref="T:DotNetAsm.SourceLine"/> object.
        /// </summary>
        public SourceLine() :
            this(string.Empty, 0, string.Empty)
        {

        }

        #endregion

        #region Methods

        /// <summary>
        /// Parse the <see cref="DotNetAsm.SourceLine"/>'s SourceString property
        /// into its component line, instruction and operand.
        /// </summary>
        /// <param name="isInstruction">A callback to determine which part of the source
        /// is the instruction.</param>
        public void Parse(Func<string, bool> isInstruction)
        {
            var tokenBuilder = new StringBuilder();
            var trimmedSource = SourceString.TrimStart();
            var len = trimmedSource.Length;
            Label = Instruction = Operand = string.Empty;

            for (var i = 0; i < len; i++)
            {
                var c = trimmedSource[i];

                if (char.IsWhiteSpace(c) || c == ';' || i == len - 1)
                {
                    // stop at a white space or the last character in the string
                    if (!char.IsWhiteSpace(c) && c != ';')
                        tokenBuilder.Append(c);
                    var token = tokenBuilder.ToString();
                    if (string.IsNullOrEmpty(Instruction))
                    {
                        if (string.IsNullOrEmpty(Label))
                        {
                            if (isInstruction(token))
                                Instruction = token;
                            else
                                Label = token;
                        }
                        else
                        {
                            Instruction = token;
                        }
                        tokenBuilder.Clear();
                    }
                    else if (char.IsWhiteSpace(c) && tokenBuilder.Length > 0)
                    {
                        // operand can include white spaces, so capture...
                        tokenBuilder.Append(c);
                    }
                    if (c == ';') // semicolon means hard break!
                        break;
                }
                else if (c == '"' || c == '\'')
                {
                    // process quotes separately
                    var quoted = trimmedSource.GetNextQuotedString(atIndex: i);
                    tokenBuilder.Append(quoted);
                    i += quoted.Length - 1;
                }
                else if (c == '=' && string.IsNullOrEmpty(Instruction))
                {
                    // constructions such as label=value must be picked up
                    // so the instruction is the assignment operator
                    if (string.IsNullOrEmpty(Label))
                        Label = tokenBuilder.ToString();
                    Instruction = "=";
                    tokenBuilder.Clear();
                }
                else
                {
                    tokenBuilder.Append(c);
                }
            }
            Operand = tokenBuilder.ToString().TrimEnd();
        }
        /// <summary>
        /// A unique identifier combination of the source's filename and line number.
        /// </summary>
        /// <returns>The identifier string.</returns>
        public string SourceInfo()
        {
            string file = Filename;
            if (file.Length > 14)
                file = Filename.Substring(0, 14) + "...";
            return string.Format("{0, -17}({1})", file, LineNumber);
        }

        #endregion

        #region Override Methods

        public override string ToString()
        {
            if (DoNotAssemble)
                return string.Format("Do Not Assemble {0}", SourceString);
            return string.Format("Line {0} ${1:X4} L:{2} I:{3} O:{4}",
                                                        LineNumber
                                                      , PC
                                                      , Label
                                                      , Instruction
                                                      , Operand);
        }

        public override int GetHashCode() => LineNumber.GetHashCode() +
                                             Filename.GetHashCode() +
                                             SourceString.GetHashCode();

        #endregion

        #region IEquatable

        public bool Equals(SourceLine other) =>
                   (other.LineNumber == this.LineNumber &&
                    other.Filename == this.Filename &&
                    other.SourceString == this.SourceString);

        #endregion

        #region ICloneable

        public object Clone() => new SourceLine
        {
            LineNumber = this.LineNumber,
            Filename = this.Filename,
            Label = this.Label,
            Operand = this.Operand,
            Instruction = this.Instruction,
            SourceString = this.SourceString,
            Scope = this.Scope,
            PC = this.PC
        };

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the <see cref="T:DotNetAsm.SourceLine"/>'s unique id number.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="T:DotNetAsm.SourceLine"/>'s Line number in the original source file.
        /// </summary>
        public int LineNumber { get; set; }

        /// <summary>
        /// Gets or sets the Program Counter of the assembly at the SourceLine.
        /// </summary>
        public long PC { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="T:DotNetAsm.SourceLine"/>'s original source filename.
        /// </summary>
        public string Filename { get; set; }

        /// <summary>
        /// Gets or sets the SourceLine scope.
        /// </summary>
        public string Scope { get; set; }

        /// <summary>
        /// Gets or sets the individual assembled bytes.
        /// </summary>
        public List<byte> Assembly { get; set; }

        /// <summary>
        /// Gets or sets the original (unparsed) source string.
        /// </summary>
        public string SourceString { get; set; }

        /// <summary>
        /// Gets or sets the disassembled representation of the source.
        /// </summary>
        public string Disassembly { get; set; }

        /// <summary>
        /// Gets or sets the flag determining whether the <see cref="T:DotNetAsm.SourceLine"/>
        /// is actually part of a comment block. Setting this flag
        /// also sets the flag to determine whether the SourceLine
        /// is to be assembled.
        /// </summary>
        public bool IsComment
        {
            get { return _comment; }
            set
            {
                _comment = value;
                if (_comment)
                    _doNotAssemble = _comment;
            }
        }

        public bool DoNotAssemble
        {
            get { return _doNotAssemble; }
            set
            {
                if (!IsComment)
                    _doNotAssemble = value;
            }
        }

        /// <summary>
        /// The <see cref="T:DotNetAsm.SourceLine"/>'s label/symbol. This can be determined using the Parse method.
        /// </summary>
        public string Label { get; set; }

        /// <summary>
        /// The <see cref="T:DotNetAsm.SourceLine"/>'s instruction. This can be determined using the Parse method.
        /// </summary>
        public string Instruction { get; set; }

        /// <summary>
        /// The <see cref="T:DotNetAsm.SourceLine"/>'s operand. This can be determined using the Parse method.
        /// </summary>
        public string Operand { get; set; }

        #endregion
    }
}
