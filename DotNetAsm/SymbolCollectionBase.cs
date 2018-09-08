//-----------------------------------------------------------------------------
// Copyright (c) 2017, 2018 informedcitizenry <informedcitizenry@gmail.com>
//
// Licensed under the MIT license. See LICENSE_6502net for full license information.
//
//-----------------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace DotNetAsm
{
    /// <summary>
    /// Represents an error that occurs during an operation in a <see cref="T:DotNetAsm.SymbolCollectionBase"/>
    /// </summary>
    public class SymbolCollectionException : Exception
    {
        /// <summary>
        /// Exception reason.
        /// </summary>
        public enum ExceptionReason
        {
            SymbolNotValid,
            SymbolExists
        }

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="T:DotNetAsm.SymbolCollectionException"/> class.
        /// </summary>
        /// <param name="symbolName">The symbol name in the operation that raised the exception.</param>
        /// <param name="reason">The exception reason.</param>
        public SymbolCollectionException(string symbolName, ExceptionReason reason)
            : base(symbolName)
        {
            Reason = reason;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the exception reason.
        /// </summary>
        /// <value>The reason.</value>
        public ExceptionReason Reason { get; private set; }

        #endregion
    }

    /// <summary>
    /// Represents a symbol collection. This class must be inherited.
    /// </summary>
    public abstract class SymbolCollectionBase : IEnumerable<KeyValuePair<string, long>>
    {
        #region Members

        readonly Dictionary<string, long> _symbols;
        Regex _regVar, _regVarStrict;
        List<SymbolCollectionBase> _crossChecks;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="T:DotNetAsm.SymbolCollectionBase"/> class.
        /// </summary>
        /// <param name="comparer">Comparer.</param>
        protected SymbolCollectionBase(StringComparer comparer)
        {
            RegexOptions option = RegexOptions.Compiled;
            option |= comparer == StringComparer.OrdinalIgnoreCase ? RegexOptions.IgnoreCase : RegexOptions.None;
            _symbols = new Dictionary<string, long>(comparer);
            _regVar = new Regex(@"^(\d+\.)?" + Patterns.SymbolUnicodeDot + "$", option);
            _regVarStrict = new Regex(Patterns.SymbolUnicodeFull, option);
            _crossChecks = new List<SymbolCollectionBase>();
        }

        #endregion

        #region Methods

        /// <summary>
        /// Gets the sub scopes of the current parent scope.
        /// </summary>
        /// <returns>The sub scopes.</returns>
        /// <param name="parent">Parent.</param>
        List<string> GetSubScopes(string parent)
        {
            if (string.IsNullOrEmpty(parent))
                return new List<string>();

            var result = new List<string> { parent };

            var split = parent.Split('.').ToList();
            split.RemoveAt(split.Count - 1);
            var combined = string.Join(".", split);
            result.AddRange(GetSubScopes(combined));
            return result;
        }

        /// <summary>
        /// Gets the nearest scope relative to the current scope.
        /// </summary>
        /// <returns>The nearest scope.</returns>
        /// <param name="symbolName">Symbol name.</param>
        /// <param name="fromScope">The nearest scope.</param>
        string GetNearestScope(string symbolName, string fromScope)
        {
            var scopes = GetSubScopes(fromScope);
            foreach (var s in scopes)
            {
                string scoped = s + "." + symbolName;
                if (_symbols.ContainsKey(scoped.TrimStartOnce('.')))
                {
                    return scoped.TrimStartOnce('.');
                }
            }
            return symbolName;
        }

        /// <summary>
        /// Clear the collection.
        /// </summary>
        public void Clear() => _symbols.Clear();

        /// <summary>
        /// Add a <see cref="T:DotNetAsm.SystemCollectionBase"/> as a cross-check.
        /// Used to avoid symbol clashes across separate collections.
        /// </summary>
        /// <param name="crossCheck">The cross check collection.</param>
        public void AddCrossCheck(SymbolCollectionBase crossCheck)
        {
            _crossChecks.Add(crossCheck);
        }

        /// <summary>
        /// Gets the symbol value.
        /// </summary>
        /// <returns>The symbol value.</returns>
        /// <param name="symbolName">Symbol name.</param>
        public long GetSymbolValue(string symbolName)
        {
            if (_symbols.TryGetValue(symbolName, out long value))
                return value;

            return long.MinValue;
        }

        /// <summary>
        /// Gets the scoped symbol value.
        /// </summary>
        /// <returns>The scoped symbol value.</returns>
        /// <param name="symbolName">Symbol name.</param>
        /// <param name="fromScope">The current scope.</param>
        public long GetScopedSymbolValue(string symbolName, string fromScope)
        {
            return GetSymbolValue(GetNearestScope(symbolName, fromScope));
        }

        /// <summary>
        /// Ises the scoped symbol.
        /// </summary>
        /// <returns><c>true</c>, if the scoped symbol exists in the collection,
        /// <c>false</c> otherwise.</returns>
        /// <param name="symbolName">Symbol name.</param>
        /// <param name="fromScope">The current scope.</param>
        public bool IsScopedSymbol(string symbolName, string fromScope)
        {
            return IsSymbol(GetNearestScope(symbolName, fromScope));
        }

        /// <summary>
        /// Checks if the collection contains the symbol.
        /// </summary>
        /// <returns><c>true</c>, if the symbol exists in the collection,
        /// <c>false</c> otherwise.</returns>
        /// <param name="symbolName">Symbol name.</param>
        public bool IsSymbol(string symbolName) => _symbols.ContainsKey(symbolName);

        /// <summary>
        /// Determines if the symbol is valid.
        /// </summary>
        /// <returns><c>true</c>, if the symbol is valid,
        /// <c>false</c> otherwise.</returns>
        /// <param name="symbolName">Symbol name.</param>
        /// <param name="isStrict">If set to <c>true</c> use strict rules
        /// for the symbol name.</param>
        public bool IsSymbolValid(string symbolName, bool isStrict)
        {
            if (isStrict)
                return _regVarStrict.IsMatch(symbolName);
            return _regVar.IsMatch(symbolName);
        }

        /// <summary>
        /// Sets the symbol.
        /// </summary>
        /// <param name="symbolName">Symbol name.</param>
        /// <param name="value">Value.</param>
        /// <param name="isStrict">If set to <c>true</c> use strict rules
        /// for the symbol name.</param>
        public void SetSymbol(string symbolName, long value, bool isStrict)
        {
            var reg = isStrict ? _regVarStrict : _regVar;
            if (reg.IsMatch(symbolName))
            {
                if (_crossChecks.Any(c => c.IsSymbol(symbolName)))
                    throw new SymbolCollectionException(symbolName, SymbolCollectionException.ExceptionReason.SymbolExists);
                _symbols[symbolName] = value;
            }
            else
            {
                throw new SymbolCollectionException(symbolName, SymbolCollectionException.ExceptionReason.SymbolNotValid);
            }
        }

        /// <summary>
        /// Gets the enumerator.
        /// </summary>
        /// <returns>The enumerator.</returns>
        public IEnumerator<KeyValuePair<string, long>> GetEnumerator()
        {
            return _symbols.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() => _symbols.GetEnumerator();

        #endregion
    }
}
