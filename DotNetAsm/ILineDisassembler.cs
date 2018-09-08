//-----------------------------------------------------------------------------
// Copyright (c) 2017, 2018 informedcitizenry <informedcitizenry@gmail.com>
//
// Licensed under the MIT license. See LICENSE_6502net for full license information.
//
//-----------------------------------------------------------------------------

using System.Text;

namespace DotNetAsm
{
    /// <summary>
    /// Represents an interface for a line disassembler.
    /// </summary>
    public interface ILineDisassembler
    {
        /// <summary>
        /// Disassemble a line of assembly source.
        /// </summary>
        /// <param name="line">The <see cref="T:DotNetAsm.SourceLine"/> to disassemble.</param>
        /// <returns>A string representation of the disassembled source.</returns>
        string DisassembleLine(SourceLine line);

        /// <summary>
        /// Disassemble a line of assembly source to a supplied
        /// <see cref="T:System.Text.StringBuilder"/>.
        /// </summary>
        /// <param name="line">The SourceLine to disassemble.</param>
        /// <param name="sb">A <see cref="T:System.Text.StringBuilder"/> to output disassembly.</param>
        void DisassembleLine(SourceLine line, StringBuilder sb);

        /// <summary>
        /// Gets a flag indicating if printing is on.
        /// </summary>
        bool PrintingOn { get; }
    }
}
