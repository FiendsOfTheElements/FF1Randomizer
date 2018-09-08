//-----------------------------------------------------------------------------
// Copyright (c) 2017, 2018 informedcitizenry <informedcitizenry@gmail.com>
//
// Licensed under the MIT license. See LICENSE_6502net for full license information.
//
//-----------------------------------------------------------------------------

namespace DotNetAsm
{
    /// <summary>
    /// Represents a signed 24-bit integer.
    /// </summary>
    public struct Int24
    {
        /// <summary>
        /// Represents the smallest possible value of an Int24. This field is constant.
        /// </summary>
        public const int MinValue = (0 - 8388608);
        /// <summary>
        /// Represents the largest possible value of an Int24. This field is constant.
        /// </summary>
        public const int MaxValue = 8388607;
    }

    /// <summary>
    /// Represents an unsigned 24-bit integer.
    /// </summary>
    public struct UInt24
    {
        /// <summary>
        /// Represents the smallest possible value of a UInt24. This field is constant.
        /// </summary>
        public const int MinValue = 0;
        /// <summary>
        /// Represents the largest possible value of a UInt24. This field is constant.
        /// </summary>
        public const int MaxValue = 0xFFFFFF;
    }
}
