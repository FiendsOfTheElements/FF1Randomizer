// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE_system_commandline in the DotNetAsm directory for full license information.

using System;

namespace System.CommandLine
{
    public sealed class ArgumentCommand<T> : ArgumentCommand
    {
        internal ArgumentCommand(string name, T value)
            : base(name)
        {
            Value = value;
        }

        public new T Value { get; private set; }

        internal override object GetValue()
        {
            return Value;
        }
    }
}
