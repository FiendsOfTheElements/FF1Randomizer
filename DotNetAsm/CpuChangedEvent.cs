//-----------------------------------------------------------------------------
// Copyright (c) 2017, 2018 informedcitizenry <informedcitizenry@gmail.com>
//
// Licensed under the MIT license. See LICENSE_6502net for full license information.
//
//-----------------------------------------------------------------------------

using System;

namespace DotNetAsm
{
    /// <summary>
    /// Cpu changed event arguments.
    /// </summary>
    public class CpuChangedEventArgs : EventArgs
    {
        public SourceLine Line { get; set; }
    }

    /// <summary>
    /// CPU change event handler delegate
    /// </summary>
    public delegate void CpuChangeEventHandler(CpuChangedEventArgs args);
}
