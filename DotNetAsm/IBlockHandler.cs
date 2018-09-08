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
    /// An interface for a block handler to process defined blocks in assembly source.
    /// </summary>
    public interface IBlockHandler
    {
        /// <summary>
        /// Determines if the block handler processes the token (instruction).
        /// </summary>
        /// <param name="token">The instruction to check if the block handler processes</param>
        /// <returns><c>True</c> if the block handler processes, otherwise <c>false</c>.</returns>
        bool Processes(string token);

        /// <summary>
        /// Process the <see cref="T:DotNetAsm.SourceLine"/> if it is processing or the instruction is a
        /// block instruction.
        /// </summary>
        /// <param name="line">The <see cref="T:DotNetAsm.SourceLine"/> to process</param>
        void Process(SourceLine line);

        /// <summary>
        /// Reset the block handler.
        /// </summary>
        void Reset();

        /// <summary>
        /// Check if the block handler is currently processing a block.
        /// </summary>
        /// <returns></returns>
        bool IsProcessing();

        /// <summary>
        /// Get the processed lines. This is typically called after the IsProcessing()
        /// method changes from true to false.
        /// </summary>
        /// <returns>A <see cref="T:System.Collections.Generic.IEnumerable&lt;DotNetAsm.SourceLine&gt;"/>
        /// of the processed block</returns>
        IEnumerable<SourceLine> GetProcessedLines();
    }
}
