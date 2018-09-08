//-----------------------------------------------------------------------------
// Copyright (c) 2017, 2018 informedcitizenry <informedcitizenry@gmail.com>
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to 
// deal in the Software without restriction, including without limitation the 
// rights to use, copy, modify, merge, publish, distribute, sublicense, and/or 
// sell copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in 
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING 
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS 
// IN THE SOFTWARE.
//-----------------------------------------------------------------------------

using System;
using System.IO;

namespace DotNetAsm
{
    /// <summary>
    /// Defines an interface for an assembly controller.
    /// </summary>
    public interface IAssemblyController
    {
        /// <summary>
        /// Add a line assembler to the <see cref="T:DotNetAsm.IAssemblyController"/>'s list of assemblers.
        /// </summary>
        /// <param name="lineAssembler">The DotNetAsm.ILineAssembler</param>
        void AddAssembler(ILineAssembler lineAssembler);

        /// <summary>
        /// Add a user-defined symbol to the <see cref="T:DotNetAsm.IAssemblyController"/>'s reserved words.
        /// </summary>
        /// <param name="symbol">The special symbol to add to the <see cref="T:DotNetAsm.IAssemblyController"/>'s
        /// reserved words.</param>
        void AddSymbol(string symbol);

        /// <summary>
        /// Performs assembly operations based on the command line arguments passed,
        /// including output to an object file and assembly listing.
        /// </summary>
        void Assemble();

        /// <summary>
        /// Gets the command-line arguments passed by the end-user and parses into 
        /// strongly-typed options.
        /// </summary>
        AsmCommandLineOptions Options { get; }

        /// <summary>
        /// Checks if a given token is actually an instruction or directive, either
        /// for the <see cref="T:DotNetAsm.IAssemblyController"/> or any line assemblers.
        /// </summary>
        /// <param name="token">The token to check</param>
        /// <returns><c>True</c> if the token is an instruction or directive, otherwise <c>false</c>.</returns>
        bool IsInstruction(string token);
 
        /// <summary>
        /// Gets or sets the disassembler. 
        /// </summary>
        ILineDisassembler Disassembler { get; set; }

        /// <summary>
        /// The Compilation object to handle binary output.
        /// </summary>
        Compilation Output { get; }

        /// <summary>
        /// The controller's error log to track errors and warnings.
        /// </summary>
        ErrorLog Log { get; }

        /// <summary>
        /// Gets the symbols for the controller.
        /// </summary>
        /// <value>The symbols.</value>
        ISymbolManager Symbols { get; }

        /// <summary>
        /// Gets expression evaluator for the controller.
        /// </summary>
        IEvaluator Evaluator { get; }

        /// <summary>
        /// Gets the custom DotNetAsm.AsmEncoding for encoding text strings.
        /// </summary>
        AsmEncoding Encoding { get; }

        /// <summary>
        /// Occurs when the CPU has changed.
        /// </summary>
        event CpuChangeEventHandler CpuChanged;

        /// <summary>
        /// Occurs when the assembler is displaying banner information (copyright, application name, etc.).
        /// </summary>
        event DisplayBannerEventHandler DisplayingBanner;

        /// <summary>
        /// Occurs when the assembler is writing header data to the binary output.
        /// </summary>
        event WriteBytesEventHandler WritingHeader;

        /// <summary>
        /// Occurs when the assembler is writing footer data to the binary output.
        /// </summary>
        event WriteBytesEventHandler WritingFooter;
    }
}
