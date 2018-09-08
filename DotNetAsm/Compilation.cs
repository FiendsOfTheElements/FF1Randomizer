//-----------------------------------------------------------------------------
// Copyright (c) 2017, 2018 informedcitizenry <informedcitizenry@gmail.com>
//
// Licensed under the MIT license. See LICENSE_6502net for full license information.
//
//-----------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace DotNetAsm
{
    /// <summary>
    /// A Compilation manages the internal state of a compiled assembly, including
    /// Program Counters and binary data.
    /// </summary>
    public class Compilation
    {
        #region Exception

        public class InvalidPCAssignmentException : Exception
        {
            readonly int _pc;

            public InvalidPCAssignmentException(int value)
            {
                _pc = value;
            }
            public override string Message
            {
                get
                {
                    return _pc.ToString();
                }
            }
        }

        #endregion

        #region Members

        List<byte> _bytes;

        int _logicalPc;

        int _pc;

        bool _overflow;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new compilation
        /// </summary>
        /// <param name="isLittleEndian">Determines whether to compile as little endian</param>
        public Compilation(bool isLittleEndian)
        {
            Transforms = new Stack<Func<byte, byte>>();
            IsLittleEndian = isLittleEndian;
            _bytes = new List<byte>();
            Reset();
        }

        /// <summary>
        /// Initializes a new compilation.
        /// </summary>
        public Compilation()
            : this(true)
        {

        }

        #endregion

        #region Static Methods

        /// <summary>
        /// Get the actual amount by which the PC must be increased to align to the given amount.
        /// </summary>
        /// <param name="amount">The amount to align</param>
        /// <returns></returns>
        public static int GetAlignmentSize(int pc, int amount)
        {
            int align = 0;
            while ((pc + align) % amount != 0)
                align++;
            return align;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Reset the compilation, clearing all bytes and resetting the Program Counter.
        /// </summary>
        public void Reset()
        {
            _bytes.Clear();
            _pc = _logicalPc = 0;
            MaxAddress = ushort.MaxValue;
            _overflow = false;
        }

        /// <summary>
        /// Change the last value in the compilation
        /// </summary>
        /// <param name="value">The value of the last compiled data</param>
        /// <param name="size">The size of the data to change</param>
        public void ChangeLast(int value, int size)
        {
            if ((size % 5) > _bytes.Count)
                size = _bytes.Count;
            if (size == 0)
                return;
            var ix = _bytes.Count - size;
            var bytes = BitConverter.GetBytes(value).ToList().GetRange(0, size);
            if (BitConverter.IsLittleEndian != this.IsLittleEndian)
                bytes.Reverse();
            _bytes.RemoveAt(ix);
            _bytes.AddRange(bytes);
        }

        /// <summary>
        /// Sets both the logical and real Program Counters.
        /// </summary>
        /// <param name="value">The program counter value</param>
        /// <exception cref="T:InvallidPCAssignmentException"></exception>
        public void SetPC(int value)
        {
            LogicalPC = value;
            ProgramCounter = value;
        }

        /// <summary>
        /// Set the logical Program Counter. This is useful when compiling re-locatable code.
        /// </summary>
        /// <param name="value">The logical program counter value</param>
        public void SetLogicalPC(int value)
        {
            if (value < 0 || value > MaxAddress)
                throw new InvalidPCAssignmentException(value);
            _logicalPc = value;
        }

        /// <summary>
        /// Reset the logical Program Counter back to the internal Program Counter value.
        /// Used with SetLogicalPC().
        /// </summary>
        /// <returns>The new logical Program Counter</returns>
        public int SynchPC()
        {
            _logicalPc = ProgramCounter;
            return LogicalPC;
        }

        /// <summary>
        /// Add a value to the compilation
        /// </summary>
        /// <param name="value">The value to add.</param>
        /// <param name="size">The size, in bytes, of the value.</param>
        /// <returns>A <see cref="T:System.Collections.Generic.List&lt;byte&gt;"/>
        /// of the bytes added to the compilation.</returns>
        public List<byte> Add(Int64 value, int size)
        {
            var bytes = BitConverter.GetBytes(value);
            return AddBytes(bytes, size, false);
        }

        /// <summary>
        /// Add uninitialized memory to the compilation. This is the same as incrementing the
        /// logical Program Counter by the specified size, but without adding any data to the
        /// compilation.
        /// </summary>
        /// <param name="size">The number of bytes to add to the memory space.</param>
        public void AddUninitialized(int size)
        {
            ProgramCounter += size;
            LogicalPC += size;
        }

        /// <summary>
        /// Add the bytes for a string to the compilation.
        /// </summary>
        /// <param name="s">The string to add.</param>
        /// <returns>A <see cref="T:System.Collections.Generic.List&lt;byte&gt;"/>
        /// of the bytes added to the compilation.</returns>
        public List<byte> Add(string s) => Add(s, Encoding.UTF8);

        /// <summary>
        /// Add the bytes for a string to the compilation.
        /// </summary>
        /// <param name="s">The string to add.</param>
        /// <param name="encoding">The <see cref="T:System.Text.Encoding"/> class to encode the output</param>
        /// <returns>A <see cref="T:System.Collections.Generic.List&lt;byte&gt;"/> of the bytes added to the compilation.</returns>
        public List<byte> Add(string s, Encoding encoding) => AddBytes(encoding.GetBytes(s));

        /// <summary>
        /// Add a 32-bit integral value to the compilation.
        /// </summary>
        /// <param name="value">The value to add</param>
        /// <returns>A System.Collections.Generic.List&lt;byte&gt; of the bytes added to the compilation.</returns>
        public List<byte> Add(int value) => Add(value, 4);

        /// <summary>
        /// Add a byte value to the compilation.
        /// </summary>
        /// <param name="value">The value to add.</param>
        /// <returns>A <see cref="T:System.Collections.Generic.List&lt;byte&gt;"/> of the bytes added to the compilation.</returns>
        public List<byte> Add(byte value) => Add(Convert.ToInt32(value), 1);

        /// <summary>
        /// Add a 16-bit integral value to the compilation.
        /// </summary>
        /// <param name="value">The value to add.</param>
        /// <returns>A <see cref="T:System.Collections.Generic.List&lt;byte&gt;"/> of the bytes added to the compilation.</returns>
        public List<byte> Add(ushort value) => Add(Convert.ToInt32(value), 2);

        /// <summary>
        /// Reserve uninitialized memory in the compilation by an unspecified amount.
        /// </summary>
        /// <param name="amount">The amount to reserve.</param>
        public void Fill(int amount)
        {
            LogicalPC += amount;
            ProgramCounter += amount;
        }

        /// <summary>
        /// Fill memory with the specified values by the specified amount.
        /// </summary>
        /// <param name="amount">The amount to fill.</param>
        /// <param name="value">The fill value.</param>
        /// <returns>A <see cref="T:System.Collections.Generic.List&lt;byte&gt;"/>
        /// of the bytes added to the compilation.</returns>
        public List<byte> Fill(int amount, Int64 value)
        {
            var size = value.Size();
            byte[] fillbytes;

            if (BitConverter.IsLittleEndian != IsLittleEndian)
                fillbytes = BitConverter.GetBytes(value).Reverse().Take(size).ToArray();
            else
                fillbytes = BitConverter.GetBytes(value).Take(size).ToArray();

            var repeated = new List<byte>();
            for (int i = 0; i < amount; i++)
                repeated.AddRange(fillbytes);

            return AddBytes(repeated.GetRange(0, amount), true);
        }

        /// <summary>
        /// Offset the compilation by a specified amount without updating the logical Program Counter.
        /// This can be used to create re-locatable code.
        /// </summary>
        /// <param name="amount">The offset amount.</param>
        /// <returns>A <see cref="T:System.Collections.Generic.List&lt;byte&gt;"/>
        ///  of the bytes added to the compilation.</returns>
        public List<byte> Offset(int amount) => AddBytes(new List<byte>(amount), amount, true, false);

        /// <summary>
        /// Align the compilation to the specified boundary and fill with the specified values.
        /// For instance, to align the next byte(s) in the compilation to a page boundary you would
        /// set the align amount to 256.
        /// </summary>
        /// <param name="amount">The amount to align the compilation.</param>
        /// <param name="value">The value to fill before the alignment.</param>
        /// <returns>A <see cref="T:System.Collections.Generic.List&lt;byte&gt;"/>
        /// of the bytes added to the compilation.</returns>
        public List<byte> Align(int amount, long value)
        {
            var align = GetAlignmentSize(LogicalPC, amount);
            return Fill(align, value);
        }

        /// <summary>
        /// Align the compilation to the specified boundary. For instance, to align the next byte(s)
        /// in the compilation to a page boundary you would set the align amount to 256.
        /// </summary>
        /// <param name="amount">The amount to align.</param>
        /// <returns>Returns the offset needed to align the Program Counter.</returns>
        public void Align(int amount)
        {
            var align = GetAlignmentSize(LogicalPC, amount);
            LogicalPC += align;
            ProgramCounter += align;
            //return align;
        }

        /// <summary>
        /// Add a range of bytes to the compilation.
        /// </summary>
        /// <param name="bytes">The collection of bytes to add.</param>
        /// <param name="size">The number of bytes in the collection to add.</param>
        /// <param name="ignoreEndian">Ignore the endianness when adding to the compilation.</param>
        /// <param name="updateProgramCounter">Update the Program Counter automatically.</param>
        /// <returns>A <see cref="T:System.Collections.Generic.List&lt;byte&gt;"/>
        /// of the bytes added to the compilation.</returns>
        public List<byte> AddBytes(IEnumerable<byte> bytes, int size, bool ignoreEndian, bool updateProgramCounter)
        {
            if (CompilingHasStarted == false)
            {
                ProgramStart = ProgramCounter;
            }
            else
            {
                int diff = ProgramCounter - (ProgramStart + _bytes.Count);
                if (diff > 0)
                    _bytes.AddRange(new byte[diff]);
            }

            if (updateProgramCounter)
            {
                ProgramCounter += size;
                LogicalPC += size;
            }

            if (ignoreEndian == false && BitConverter.IsLittleEndian != IsLittleEndian)
                bytes = bytes.Reverse();

            if (Transforms.Count > 0)
            {
                var transformed = bytes.ToList().GetRange(0, size);
                for (int i = 0; i < size; i++)
                {
                    foreach (var t in Transforms)
                        transformed[i] = t(transformed[i]);
                }
                _bytes.AddRange(transformed);
                return transformed;
            }
            var bytesAdded = bytes.ToList().GetRange(0, size);
            _bytes.AddRange(bytesAdded);
            return bytesAdded;
        }

        /// <summary>
        /// Add a range of bytes to the compilation.
        /// </summary>
        /// <param name="bytes">The collection of bytes to add.</param>
        /// <param name="size">The number of bytes in the collection to add.</param>
        /// <param name="ignoreEndian">Ignore the endianness when adding to the compilation.</param>
        /// <returns>A <see cref="T:System.Collections.Generic.List&lt;byte&gt;"/>
        /// of the bytes added to the compilation.</returns>
        public List<byte> AddBytes(IEnumerable<byte> bytes, int size, bool ignoreEndian)
        {
            return AddBytes(bytes, size, ignoreEndian, true);
        }

        /// <summary>
        /// Add a range of bytes to the compilation.
        /// </summary>
        /// <param name="bytes">The collection of bytes to add.</param>
        /// <param name="ignoreEndian">Ignore the endianness when adding to the compilation.</param>
        /// <returns>A <see cref="T:System.Collections.Generic.List&lt;byte&gt;"/>
        /// of the bytes added to the compilation.</returns>
        public List<byte> AddBytes(IEnumerable<byte> bytes, bool ignoreEndian)
        {
            return AddBytes(bytes, bytes.Count(), ignoreEndian);
        }

        /// <summary>
        /// Add a range of bytes to the compilation.
        /// </summary>
        /// <param name="bytes">The collection of bytes to add.</param>
        /// <param name="size">The number of bytes in the collection to add.</param>
        /// <returns>A <see cref="T:System.Collections.Generic.List&lt;byte&gt;"/>
        /// of the bytes added to the compilation.</returns>
        public List<byte> AddBytes(IEnumerable<byte> bytes, int size) => AddBytes(bytes, size, true);

        /// <summary>
        /// Add a range of bytes to the compilation.
        /// </summary>
        /// <param name="bytes">The collection of bytes to add.</param>
        /// <returns>A <see cref="T:System.Collections.Generic.List&lt;byte&gt;"/>
        /// of the bytes added to the compilation.</returns>
        public List<byte> AddBytes(IEnumerable<byte> bytes) => AddBytes(bytes, true);

        /// <summary>
        /// Get the compilation bytes.
        /// </summary>
        /// <returns>The bytes of the compilation.</returns>
        public ReadOnlyCollection<byte> GetCompilation() => _bytes.AsReadOnly();

        /// <summary>
        /// Get the relative offset between two addresses. Useful for calculating short jumps.
        /// </summary>
        /// <param name="address1">Current address.</param>
        /// <param name="address2">Destination address.</param>
        /// <returns>Returns the relative offset between the two addresses.</returns>
        public int GetRelativeOffset(int address1, int address2)
        {
            address2 = address2 & MaxAddress;
            int offset = address1 - address2;
            if (Math.Abs(offset) > (MaxAddress / 2))
            {
                if (offset < 0)
                    offset = Math.Abs(offset) - (MaxAddress + 1);
                else
                    offset = MaxAddress + 1 - offset;

                if (address1 > address2)
                    offset = -offset;
            }
            return offset;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the program start addressed based on the value of the Program Counter
        /// when compilation first occurred.
        /// </summary>
        public int ProgramStart { get; private set; }

        /// <summary>
        /// Gets the program end address, which is the address of the final assembled byte
        /// from the program start.
        /// </summary>
        public int ProgramEnd
        {
            get
            {
                if (_bytes.Count > 0)
                    return (ProgramStart + (_bytes.Count - 1)) & MaxAddress;
                return 0;
            }
        }

        /// <summary>
        /// Gets or sets a the collections of functions that apply transforms to the bytes before
        /// they are added to the compilation. The transform functions are called from last to
        /// first in the order they are pushed onto the transform stack.
        /// </summary>
        public Stack<Func<byte, byte>> Transforms { get; set; }

        /// <summary>
        /// Gets or sets the maximum address allowed for the Program Counter until
        /// it overflows.
        /// </summary>
        public int MaxAddress { get; set; }

        /// <summary>
        /// Gets the status of the compilation, if it is currently compiling
        /// </summary>
        bool CompilingHasStarted { get { return _bytes.Count > 0; } }

        /// <summary>
        /// Gets the real Program Counter
        /// </summary>
        public int ProgramCounter
        {
            get { return _pc; }
            set
            {
                if (value < 0 || value < _pc)
                    throw new InvalidPCAssignmentException(value);
                _pc = value & MaxAddress;
            }
        }

        /// <summary>
        /// Gets the endianness of the compilation
        /// </summary>
        public bool IsLittleEndian { get; private set; }

        /// <summary>
        /// Gets a flag that indicates if a PC overflow has occurred. This flag will
        /// only be cleared with a call to the Reset method.
        /// </summary>
        public bool PCOverflow { get { return _overflow; } }

        /// <summary>
        /// Gets the current logical Program Counter.
        /// </summary>
        public int LogicalPC
        {
            get { return _logicalPc; }
            set
            {
                if (value < 0 || value < _logicalPc)
                    throw new InvalidPCAssignmentException(value);
                if (!_overflow)
                    _overflow = value > MaxAddress;
                _logicalPc = value & MaxAddress;
            }
        }

        #endregion
    }
}
