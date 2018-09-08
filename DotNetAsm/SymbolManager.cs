//-----------------------------------------------------------------------------
// Copyright (c) 2017, 2018 informedcitizenry <informedcitizenry@gmail.com>
//
// Licensed under the MIT license. See LICENSE_6502net for full license information.
//
//-----------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace DotNetAsm
{
    /// <summary>
    /// A concrete implementation of <see cref="DotNetAsm.ISymbolManager" />.
    /// </summary>
    public sealed class SymbolManager : ISymbolManager
    {
        #region Members

        IAssemblyController _controller;

        Dictionary<int, SourceLine> _anonPlusLines, _anonMinusLines, _orderedMinusLines;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="T:DotNetAsm.SymbolManager"/> class.
        /// </summary>
        /// <param name="controller">The <see cref="DotNetAsm.IAssemblyController"/> for
        /// this symbol manager.</param>
        public SymbolManager(IAssemblyController controller)
        {
            _controller = controller;

            Variables = new VariableCollection(controller.Options.StringComparar, controller.Evaluator);

            Labels = new LabelCollection(controller.Options.StringComparar);

            Labels.AddCrossCheck(Variables);
            Variables.AddCrossCheck(Labels);

            _anonPlusLines = new Dictionary<int, SourceLine>();
            _anonMinusLines = new Dictionary<int, SourceLine>();

        }

        #endregion

        #region Methods

        string GetNamedSymbolValue(string symbol, SourceLine line, string scope)
        {
            if (symbol.First() == '_')
                symbol = string.Concat(scope, symbol);
            if (Variables.IsScopedSymbol(symbol, scope))
                return Variables.GetScopedSymbolValue(symbol, line.Scope).ToString();

            var value = Labels.GetScopedSymbolValue(symbol, line.Scope);
            if (value.Equals(long.MinValue))
                throw new SymbolNotDefinedException(symbol);

            return value.ToString();

        }

        string ConvertAnonymous(string symbol, SourceLine line, bool errorOnNotFound)
        {
            var trimmed = symbol.Trim(new char[] { '(', ')' });
            var addr = GetFirstAnonymousLabelFrom(line, trimmed);//GetAnonymousAddress(_currentLine, trimmed);
            if (addr < 0 && errorOnNotFound)
            {
                _controller.Log.LogEntry(line, ErrorStrings.CannotResolveAnonymousLabel);
                return "0";
            }
            return addr.ToString();
        }

        public void AddAnonymousLine(SourceLine line)
        {
            if (line.Label.Equals("+"))
            {
                _anonPlusLines.Add(line.Id, line);
            }
            else if (line.Label.Equals("-"))
            {
                _anonMinusLines.Add(line.Id, line);
                // ordered dictionary is invalid now
                _orderedMinusLines = null;
            }
        }

        long GetFirstAnonymousLabelFrom(SourceLine fromLine, string direction)
        {
            int id = fromLine.Id;

            int count = direction.Length;
            bool forward = direction[0] == '+';
            SourceLine found = null;
            while (count > 0)
            {
                KeyValuePair<int, SourceLine> searched;
                if (forward)
                {
                    searched = _anonPlusLines.FirstOrDefault(l => l.Key > id);
                }
                else
                {
                    if (_orderedMinusLines == null)
                        _orderedMinusLines = _anonMinusLines.OrderByDescending(l => l.Key).ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
                    searched = _orderedMinusLines.FirstOrDefault(l => l.Key < id);
                }
                found = searched.Value;

                if (found == null)
                    break;

                if (string.IsNullOrEmpty(found.Scope) || found.Scope.Equals(fromLine.Scope, _controller.Options.StringComparison) ||
                    (fromLine.Scope.Length > found.Scope.Length &&
                     found.Scope.Equals(fromLine.Scope.Substring(0, found.Scope.Length), _controller.Options.StringComparison)))
                    count--;

                id = found.Id;
            }
            if (found != null)
                return found.PC;
            return -1;
        }

        public string TranslateExpressionSymbols(SourceLine line, string expression, string scope, bool errorOnAnonymousNotFound)
        {
            StringBuilder tokenBuilder = new StringBuilder();
            string foundSymbol = string.Empty;
            char lastChar = char.MinValue;
            for (int i = 0; i < expression.Length; i++)
            {
                var c = expression[i];
                if (c == '\'' || c == '"')
                {
                    var literal = expression.GetNextQuotedString(atIndex: i);
                    var unescaped = Regex.Unescape(literal.Trim('\''));
                    var charval = _controller.Encoding.GetEncodedValue(unescaped.Substring(0, 1)).ToString();

                    expression = string.Concat(expression.Substring(0, i - foundSymbol.Length), charval, expression.Substring(i + literal.Length));

                    i += charval.Length - 1;
                    // we need to make sure we have the updated string
                    lastChar = charval.Last();
                }
                else if (tokenBuilder.Length == 0 && (char.IsLetter(c) || c == '_' || c == '*' || c == '+' || c == '-') &&
                    (lastChar == char.MinValue || lastChar == '(' || lastChar.IsOperator() || lastChar == ','))
                {
                    // this could the be the first character of a special symbol
                    if (c == '*' || c == '+' || c == '-')
                    {
                        if (!char.IsLetterOrDigit(lastChar) && lastChar != ')')
                        {
                            if (c == '-' || c == '+')
                            {
                                var nextChar = expression.Substring(i + 1).FirstOrDefault(chr => !char.IsWhiteSpace(chr));
                                if ((lastChar == '(' && nextChar == ')') || (!char.IsLetterOrDigit(nextChar) && nextChar != '('))
                                    tokenBuilder.Append(c);
                            }
                            else
                            {
                                // Program counter *
                                tokenBuilder.Append(c);
                            }
                        }
                    }
                    else
                    {
                        // letter or underscore, start to build the token
                        tokenBuilder.Append(c);
                    }
                }
                else if (tokenBuilder.Length > 0)
                {
                    if (char.IsWhiteSpace(c) || c.IsOperator() || c == ')')
                    {
                        var last = tokenBuilder[tokenBuilder.Length - 1];
                        if ((c == '-' || c == '+') && (last == '-' || last == '+') && last == c)
                        {
                            // anonymous symbol, i.e. bne -- or bcs ++
                            tokenBuilder.Append(c);
                        }
                        else
                        {
                            foundSymbol = tokenBuilder.ToString();
                            tokenBuilder.Clear();
                        }
                    }
                    else if (char.IsLetterOrDigit(c) || c == '.' || c == '_')
                    {
                        tokenBuilder.Append(c);
                    }
                    else if (c == '(')
                    {
                        // this is a function call (probably), therefore it's not a symbol
                        // so clear the tokenbuilder
                        tokenBuilder.Clear();
                    }
                }
                if (tokenBuilder.Length > 0 && i == expression.Length - 1)
                {
                    foundSymbol = tokenBuilder.ToString();
                    i++;
                }

                if (!string.IsNullOrEmpty(foundSymbol))
                {
                    // oops can't do this!
                    if (foundSymbol.Last() == '.')
                        throw new ExpressionException(expression);

                    string replacement = string.Empty;
                    if (foundSymbol[0] == '-' || foundSymbol[0] == '+')
                        replacement = ConvertAnonymous(foundSymbol, line, errorOnAnonymousNotFound);
                    else if (foundSymbol.Equals("*"))
                        replacement = _controller.Output.LogicalPC.ToString();
                    else
                        replacement = GetNamedSymbolValue(foundSymbol, line, scope);// lookup symbol

                    expression = string.Concat(expression.Substring(0, i - foundSymbol.Length), replacement, expression.Substring(i));
                    // move the cursor forward
                    i += replacement.Length - foundSymbol.Length - 1;

                    // we need to make sure we have the updated string
                    lastChar = replacement.Last();

                    // reset the found symbol
                    foundSymbol = string.Empty;
                }
                else if (!char.IsWhiteSpace(c))
                    lastChar = c;
            }
            return expression;
        }

        #endregion

        #region Properties

        public VariableCollection Variables { get; private set; }

        public LabelCollection Labels { get; private set; }

        #endregion
    }
}
