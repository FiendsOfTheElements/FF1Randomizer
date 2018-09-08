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
    public static class StringBuilderExtensions
    {
        /// <summary>
        /// Removes all trailing occurrences of white spaces from the
        /// current <see cref="T:System.Text.StringBuilder"/> object.
        /// </summary>
        /// <param name="sb">This <see cref="T:System.Text.StringBuilder"/>.</param>
        /// <returns>The trimmed <see cref="T:System.Text.StringBuilder"/>.</returns>
        public static StringBuilder TrimEnd(this StringBuilder sb)
        {
            if (sb == null || sb.Length == 0) return sb;

            int i = sb.Length - 1;
            for (; i >= 0; i--)
                if (!char.IsWhiteSpace(sb[i]))
                    break;

            if (i < sb.Length - 1)
                sb.Length = i + 1;

            return sb;
        }

        /// <summary>
        /// Removes all leading occurrences of white spaces from the
        /// current <see cref="T:System.Text.StringBuilder"/> object.
        /// </summary>
        /// <param name="sb">This <see cref="T:System.Text.StringBuilder"/>.</param>
        /// <returns>The trimmed <see cref="T:System.Text.StringBuilder"/>.</returns>
        public static StringBuilder TrimStart(this StringBuilder sb)
        {
            if (sb == null || sb.Length == 0) return sb;

            while (sb.Length > 0 && char.IsWhiteSpace(sb[0]))
                sb.Remove(0, 1);

            return sb;
        }

        /// <summary>
        /// Removes all leading and trailing occurrences of white spaces from the
        /// current <see cref="T:System.Text.StringBuilder"/> object.
        /// </summary>
        /// <param name="sb">This <see cref="T:System.Text.StringBuilder"/>.</param>
        /// <returns>The trimmed <see cref="T:System.Text.StringBuilder"/>.</returns>
        public static StringBuilder Trim(this StringBuilder sb) => sb.TrimStart().TrimEnd();
    }

    public static class StringExtensions
    {
        /// <summary>
        /// String to split by into substrings by length.
        /// </summary>
        /// <param name="str">The string to split.</param>
        /// <param name="maxLength">The maximum length per sub-string. "Carry-over"
        /// substrings after split will be their own string.</param>
        /// <returns>An <see cref="T:System.Collections.Generic.IEnumerable&lt;string&gt;"/> class.</returns>
        public static IEnumerable<string> SplitByLength(this string str, int maxLength)
        {
            int index = 0;
            while (index + maxLength < str.Length)
            {
                yield return str.Substring(index, maxLength);
                index += maxLength;
            }

            yield return str.Substring(index);
        }

        /// <summary>
        /// Tests whether the string is enclosed in double quotes.
        /// </summary>
        /// <param name="str">The string to evaluate.</param>
        /// <returns><c>True</c> if string is fully enclosed in quotes, otherwise <c>false</c>.</returns>
        public static bool EnclosedInQuotes(this string str)
        {
            return !string.IsNullOrEmpty(str) && str.Equals(str.GetNextQuotedString(0));
        }

        /// <summary>
        /// Trims one instance of the specified character at the start of the string.
        /// </summary>
        /// <returns>The modified string.</returns>
        /// <param name="str">String.</param>
        /// <param name="c">The character to trim.</param>
        public static string TrimStartOnce(this string str, char c)
        {
            if (string.IsNullOrEmpty(str)) return string.Empty;
            if (str.First().Equals(c))
                return str.Length > 1 ? str.Substring(1) : string.Empty;
            return str;
        }

        /// <summary>
        /// Trims one instance of the specified character at the end of the string.
        /// </summary>
        /// <returns>The modified string.</returns>
        /// <param name="str">String.</param>
        /// <param name="c">The character to trim.</param>
        public static string TrimEndOnce(this string str, char c)
        {
            if (string.IsNullOrEmpty(str)) return string.Empty;
            if (str.Last().Equals(c))
                return str.Length > 1 ? str.Substring(0, str.Length - 1) : string.Empty;
            return str;
        }

        /// <summary>
        /// Trims one instance of the specified character at the start and the end of the string.
        /// </summary>
        /// <returns>The modified string.</returns>
        /// <param name="str">String.</param>
        /// <param name="c">The character to trim.</param>
        public static string TrimOnce(this string str, char c)
        {
            return str.TrimStartOnce(c).TrimEndOnce(c);
        }

        /// <summary>
        /// Capture and return the first parenthetical group in the string.
        /// </summary>
        /// <param name="str">The string to evaluate</param>
        /// <returns>The first instance of a parenthetical group.</returns>
        /// <exception cref="T:System.FormatException"></exception>
        public static string FirstParenEnclosure(this string str)
        {
            return FirstParenEnclosure(str, false);
        }

        /// <summary>
        /// Capture and return the first parenthetical group in the string.
        /// </summary>
        /// <returns>The first instance of a parenthetical group.</returns>
        /// <param name="str">The string to evaluate..</param>
        /// <param name="useBracket">If set to <c>true</c> consider square brackets.
        /// as open and close parentheses.</param>
        public static string FirstParenEnclosure(this string str, bool useBracket)
        {
            char open = '(', closure = ')';
            if (useBracket)
            {
                open = '[';
                closure = ']';
            }
            if (str.Contains(open.ToString()) || str.Contains(closure.ToString()))
            {
                if (str.Length < 3)
                    throw new ExpressionException(str);
                var num_parens = 0;
                var parengroup = new StringBuilder();
                for (int i = 0; i < str.Length; i++)
                {
                    var c = str[i];
                    var quoted = string.Empty;

                    if (c == '"' || c == '\'')
                    {
                        quoted = str.GetNextQuotedString(atIndex: i);
                        if (num_parens >= 1)
                            parengroup.Append(quoted);
                        i += quoted.Length - 1;
                        continue;
                    }
                    else if (num_parens >= 1 || c == open)
                        parengroup.Append(c);

                    if (c == open)
                    {
                        num_parens++;
                    }
                    else if (c == closure)
                    {
                        num_parens--;
                        if (num_parens == 0)
                            return parengroup.ToString();
                    }
                }
                if (num_parens != 0)
                    throw new FormatException();
            }
            return str;
        }
        /// <summary>
        /// Gets the next double- or single-quoted string within the string.
        /// </summary>
        /// <returns>The next quoted string, or empty if no quoted string present.</returns>
        /// <param name="str">String.</param>
        /// <exception cref="T:System.Exception"></exception>
        public static string GetNextQuotedString(this string str)
        {
            return str.GetNextQuotedString(0);
        }

        /// <summary>
        /// Gets the next double- or single-quoted string within the string.
        /// </summary>
        /// <returns>The next quoted string, or empty if no quoted string present.</returns>
        /// <param name="str">String.</param>
        /// <param name="atIndex">The index at which to search the string.</param>
        /// <exception cref="T:System.Exception"></exception>
        public static string GetNextQuotedString(this string str, int atIndex)
        {
            if (!str.Contains("\"") && !str.Contains("'"))
                return string.Empty;

            var quoted = new StringBuilder();
            var enclosestring = char.MinValue;
            int stringsize = 0;

            for (int i = atIndex; i < str.Length; i++)
            {
                var c = str[i];
                if (!enclosestring.Equals(char.MinValue) || c == '\'' || c == '"')
                {
                    quoted.Append(c);
                    stringsize++;
                    if (c.Equals('\'') || c.Equals('"'))
                    {
                        if (enclosestring.Equals(char.MinValue))
                            enclosestring = c;
                        else if (enclosestring.Equals(c))
                            break;
                    }
                    else if (c.Equals('\\'))
                    {
                        // escape cannot be the last char in the string
                        if (++i == str.Length - 1)
                            throw new Exception(ErrorStrings.QuoteStringNotEnclosed);
                        c = str[i];
                        if (c.Equals('u') || c.Equals('x'))
                        {
                            var m = Regex.Match(str.Substring(i), @"^(u[a-fA-F0-9]{4}|x[a-fA-F0-9]{2})");
                            if (!string.IsNullOrEmpty(m.Value))
                            {

                                quoted.Append(m.Value);
                                i += m.Value.Length - 1;
                            }
                        }
                        else
                        {
                            quoted.Append(c);
                        }
                    }
                }
            }
            if (!enclosestring.Equals(char.MinValue))
            {
                if (stringsize < 2 || !quoted[quoted.Length - 1].Equals(enclosestring))
                    throw new Exception(ErrorStrings.QuoteStringNotEnclosed);

                if (enclosestring.Equals('\'') && stringsize > 3)
                    throw new Exception(ErrorStrings.TooManyCharacters);
            }
            return quoted.ToString();
        }

        /// <summary>
        /// Does a comma-separated-value analysis on the <see cref="T:DotNetAsm.SourceLine"/>'s operand
        /// and returns the individual value as a <see cref="T:System.Collections.Generic.List&lt;string&gt;"/>.
        /// </summary>
        /// <param name="str">The string to evaluate.</param>
        /// <returns>A <see cref="T:System.Collections.Generic.List&lt;string&gt;"/> of the values.</returns>
        /// <exception cref="T:System.Exception"></exception>
        public static List<string> CommaSeparate(this string str)
        {
            return CommaSeparate(str, '(', ')');
        }

        /// <summary>
        /// Does a comma-separated-value analysis on the <see cref="T:DotNetAsm.SourceLine"/>'s operand
        /// and returns the individual value as a <see cref="T:System.Collections.Generic.List&lt;string&gt;"/>.
        /// </summary>
        /// <returns>A <see cref="T:System.Collections.Generic.List&lt;string&gt;"/> of the values.</returns>
        /// <param name="str">The string to evaluate.</param>
        /// <param name="open">Open paranethesis character.</param>
        /// <param name="closure">Close paranthesis character.</param>
        /// <remarks>All commas inside paranetheses will be skipped.</remarks>
        public static List<string> CommaSeparate(this string str, char open, char closure)
        {
            var csv = new List<string>();

            if (string.IsNullOrEmpty(str))
                return csv;

            if (!str.Contains(","))
                return new List<string> { str };

            var num_parens = 0;
            var sb = new StringBuilder();

            for (int i = 0; i < str.Length; i++)
            {
                char c = str[i];
                if (c.Equals('\'') || c.Equals('\"'))
                {
                    var quoted = str.GetNextQuotedString(atIndex: i);
                    i += quoted.Length - 1;
                    sb.Append(quoted);
                    if (i >= str.Length - 1)
                        csv.Add(sb.ToString().Trim());
                }
                else if (num_parens > 0)
                {
                    sb.Append(c);
                    if (c == closure)
                    {
                        num_parens--;
                        if (i == str.Length - 1)
                            csv.Add(sb.ToString().Trim());
                    }
                }
                else
                {
                    if (c == open)
                    {
                        num_parens++;
                    }
                    else if (c == ',')
                    {
                        csv.Add(sb.ToString().Trim());
                        sb.Clear();
                        continue;
                    }
                    sb.Append(c);
                    if (i == str.Length - 1)
                        csv.Add(sb.ToString().Trim());
                }
            }
            if (num_parens != 0)
                throw new Exception(ErrorStrings.None);

            if (str.Last().Equals(','))
                csv.Add(string.Empty);
            return csv;
        }
    }

    public static class Char_Extension
    {
        /// <summary>
        /// Indicates whether the specified Unicode character is a mathematical
        /// operator.
        /// </summary>
        /// <returns><c>true</c>, if the character is an operator, <c>false</c> otherwise.</returns>
        /// <param name="c">The Unicode character.</param>
        public static bool IsOperator(this char c)
        {
            return (char.IsSymbol(c) && c != '$') || c == '/' || c == '*' || c == '-' || c == '&' || c == '%' || c == '!';
        }
    }

    public static class Int64_Extension
    {
        /// <summary>
        /// The minimum size required in bytes to store this value.
        /// </summary>
        /// <param name="value">The value to store.</param>
        /// <returns>The size in bytes.</returns>
        public static int Size(this Int64 value)
        {
            if (value < 0)
                value = (~value) << 1;

            if ((value & 0xFFFFFF00) == 0) return 1;
            if ((value & 0xFFFF0000) == 0) return 2;
            if ((value & 0xFF000000) == 0) return 3;
            return 4;
        }
    }
}
