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
    /// A <see cref="T:DotNetAsm.SymbolCollectionBase"/> implemented as a label collection.
    /// </summary>
    public sealed class LabelCollection : SymbolCollectionBase
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="T:DotNetAsm.LabelCollection"/> class.
        /// </summary>
        /// <param name="comparer">Comparer.</param>
        public LabelCollection(StringComparer comparer)
            : base(comparer)
        {

        }

        #endregion

        #region Methods

        /// <summary>
        /// Sets the label.
        /// </summary>
        /// <param name="labelName">Label name.</param>
        /// <param name="value">Value.</param>
        /// <param name="isStrict">If set to <c>true</c> the label name is strictly enforced.</param>
        /// <param name="isDeclaration">If set to <c>true</c> is a declaration.</param>
        public void SetLabel(string labelName, long value, bool isStrict, bool isDeclaration)
        {
            if (isDeclaration && IsSymbol(labelName))
                throw new SymbolCollectionException(labelName, SymbolCollectionException.ExceptionReason.SymbolExists);
            SetSymbol(labelName, value, isStrict);
        }

        #endregion
    }
}
