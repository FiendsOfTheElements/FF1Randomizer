//-----------------------------------------------------------------------------
// Copyright (c) 2017, 2018 informedcitizenry <informedcitizenry@gmail.com>
//
// Licensed under the MIT license. See LICENSE_6502net for full license information.
//
//-----------------------------------------------------------------------------

namespace DotNetAsm
{
    /// <summary>
    /// Defines an interface for a line assembler.
    /// </summary>
    public interface ILineAssembler
    {
        /// <summary>
        /// Assemble the line of source into output bytes of the
        /// target architecture.
        /// </summary>
        /// <param name="line">The source line to assemble.</param>
        void AssembleLine(SourceLine line);

        /// <summary>
        /// Gets the size of the instruction in the source line. This value might not be valid
        /// on first pass, but is guaranteed to be valid before final pass.
        /// </summary>
        /// <param name="line">The source line to query.</param>
        /// <returns>Returns the size in bytes of the instruction or directive.</returns>
        int GetInstructionSize(SourceLine line);

        /// <summary>
        /// Indicates whether this line assembler will assemble the
        /// given instruction or directive.
        /// </summary>
        /// <param name="instruction">The instruction.</param>
        /// <returns><c>True</c> if the line assembler can assemble the source,
        /// otherwise <c>false</c>.</returns>
        bool AssemblesInstruction(string instruction);
    }
}
