//-----------------------------------------------------------------------------
// Copyright (c) 2017, 2018 informedcitizenry <informedcitizenry@gmail.com>
//
// Licensed under the MIT license. See LICENSE_6502net for full license information.
//
//-----------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace DotNetAsm
{
    /// <summary>
    /// A <see cref="T:DotNetAsm.SymbolCollectionBase"/> implemented as a variable collection.
    /// </summary>
    public sealed class VariableCollection : SymbolCollectionBase
    {
        #region Members

        IEvaluator _evaluator;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="T:DotNetAsm.VariableCollection"/> class.
        /// </summary>
        /// <param name="comparer">A <see cref="T:System.StringComparper"/>.</param>
        /// <param name="evaluator">A <see cref="T:DotNetAsm.IEvaluator"/> to evaluate RValue.</param>
        public VariableCollection(StringComparer comparer, IEvaluator evaluator)
            : base(comparer)
        {
            _evaluator = evaluator;
        }

        #endregion

        #region Methods

        KeyValuePair<string, string> ParseExpression(string expression)
        {
            var symbolBuilder = new StringBuilder();
            var len = expression.Length;
            int i;
            for (i = 0; i < len; i++)
            {
                var c = expression[i];
                if (char.IsWhiteSpace(c))
                    continue;
                if (c == '=')
                    break;
                symbolBuilder.Append(c);
            }
            if (symbolBuilder.Length > 0 && i < len - 1)
                return new KeyValuePair<string, string>(symbolBuilder.ToString(), expression.Substring(i + 1));
            return new KeyValuePair<string, string>(string.Empty, string.Empty);
        }

        /// <summary>
        /// Gets the variable from a variable assignment expression.
        /// </summary>
        /// <returns>The variable from expression.</returns>
        /// <param name="expression">The assignment expression.</param>
        /// <param name="inScope">The current scope the expression is in.</param>
        public string GetVariableFromExpression(string expression, string inScope)
        {
            return string.Concat(inScope, ParseExpression(expression).Key);
        }

        /// <summary>
        /// Gets the assignment (RValue) from the expression.
        /// </summary>
        /// <returns>The assignment from expression.</returns>
        /// <param name="expression">Expression.</param>
        public string GetAssignmentFromExpression(string expression)
        {
            var kv = ParseExpression(expression);
            if (string.IsNullOrEmpty(kv.Key) == false)
                return kv.Value;
            return string.Empty;
        }

        /// <summary>
        /// Sets the variable according to the assignment expression &lt;var&gt; = &lt;operand&gt;.
        /// </summary>
        /// <returns>The variable and its assignment as a
        /// <see cref="T:System.Collections.Generic.KeyValuePair&lt;string, string&gt;"/>.</returns>
        /// <param name="expression">The assignment expression.</param>
        /// <param name="inScope">The current scope the expression is in.</param>
        /// <exception cref="T:DotNetAsm.ExpressionException">DotNetAsm.ExpressionException</exception>
        /// <exception cref="T:DotNetAsm.SymbolCollectionException">DotNetAsm.SymbolCollectionException</exception>
        public KeyValuePair<string, string> SetVariable(string expression, string inScope)
        {
            var result = ParseExpression(expression);
            if (string.IsNullOrEmpty(result.Key))
                throw new ExpressionException(expression);

            var varname = string.Concat(inScope, result.Key);

            if (IsSymbolValid(varname, true) == false)
                throw new SymbolCollectionException(varname, SymbolCollectionException.ExceptionReason.SymbolNotValid);
            SetSymbol(result.Key, _evaluator.Eval(result.Value, int.MinValue, uint.MaxValue), false);
            return result;
        }

        #endregion
    }
}
