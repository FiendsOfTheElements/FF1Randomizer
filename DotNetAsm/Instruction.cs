//-----------------------------------------------------------------------------
// Copyright (c) 2017, 2018 informedcitizenry <informedcitizenry@gmail.com>
//
// Licensed under the MIT license. See LICENSE_6502net for full license information.
//
//-----------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace DotNetAsm
{
    /// <summary>
    /// A class that represents information about an instruction, including its
    /// size, CPU and opcode.
    /// </summary>
    public class Instruction
    {
        /// <summary>
        /// The instruction size (including operands).
        /// </summary>
        public int Size { get; set; }

        /// <summary>
        /// The opcode of the instruction.
        /// </summary>
        public int Opcode { get; set; }

        /// <summary>
        /// Gets or sets the CPU of this instruction.
        /// </summary>
        /// <value>The cpu.</value>
        public string CPU { get; set; }
    }

    /// <summary>
    /// Represents an operand format, including captured expressions
    /// </summary>
    public class OperandFormat
    {
        /// <summary>
        /// The format string of the operand
        /// </summary>
        public string FormatString;

        /// <summary>
        /// The first captured evaluated expression.
        /// </summary>
        public long Eval1;

        /// <summary>
        /// The second captured evaluated expression.
        /// </summary>
        public long Eval2;
    }
}
