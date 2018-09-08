//-----------------------------------------------------------------------------
// Copyright (c) 2017, 2018 informedcitizenry <informedcitizenry@gmail.com>
//
// Licensed under the MIT license. See LICENSE_6502net for full license information.
//
//-----------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace DotNetAsm
{
    /// <summary>
    /// Manages custom text codes from UTF-8 source
    /// to target architectures with different character encoding
    /// schemes.
    /// </summary>
    public sealed class AsmEncoding : Encoding
    {
        #region Members

        Dictionary<string, Dictionary<string, int>> _maps;

        Dictionary<string, int> _currentMap;

        #endregion

        #region Constructors

        /// <summary>
        /// Constructs an instance of the <see cref="T:DotNetAsm.AsmEncoding"/> class, used to encode
        /// strings from UTF-8 source to architecture-specific encodings.
        /// </summary>
        /// <param name="caseSensitive">Indicates whether encoding names
        /// should be treated as case-sensitive. Note: This has no effect on how character
        /// mappings are translated</param>
        public AsmEncoding(bool caseSensitive)
        {
            StringComparer comparer = caseSensitive ? StringComparer.Ordinal : StringComparer.OrdinalIgnoreCase;
            _maps = new Dictionary<string, Dictionary<string, int>>(comparer);
            SelectEncoding("none");
        }

        /// <summary>
        /// Constructs an instance of the <see cref="T:DotNetAsm.AsmEncoding"/> class, used to encode
        /// strings from UTF-8 source to architecture-specific encodings.
        /// </summary>
        public AsmEncoding() :
            this(false)
        {

        }

        #endregion

        #region Methods

        /// <summary>
        /// Get the actual encoded bytes for a string element.
        /// </summary>
        /// <param name="s">The string element to encode.</param>
        /// <returns>An array of encoded bytes for the character.</returns>
        byte[] GetCharBytes(string s)
        {
            if (_currentMap.ContainsKey(s))
            {
                long code = _currentMap[s];
                var codebytes = BitConverter.GetBytes(code);
                return codebytes.Take(code.Size()).ToArray();
            }
            return Encoding.UTF8.GetBytes(s);
        }


        /// <summary>
        /// Get the encoded binary value of the given character as a
        /// <see cref="T:System.Int32" /> value.
        /// </summary>
        /// <param name="s">The string element to encode.</param>
        /// <returns>An unsigned 32-bit integer representing the
        /// encoding of the character.</returns>
        /// <exception cref="T:System.ArgumentException">System.ArgumentException</exception>
        /// <exception cref="T:System.ArgumentNullException"></exception>
        public int GetEncodedValue(string s)
        {
            if (string.IsNullOrEmpty(s))
                throw new ArgumentNullException();

            if (_currentMap.ContainsKey(s))
                return _currentMap[s];

            var stringInfo = StringInfo.ParseCombiningCharacters(s);
            if (stringInfo.Length > 1)
                throw new ArgumentException(s);

            var charbytes = GetCharBytes(s);
            if (charbytes.Length < 4)
                Array.Resize(ref charbytes, 4);
            return BitConverter.ToInt32(charbytes, 0);
        }

        /// <summary>
        /// Adds a character mapping to translate from source to object
        /// </summary>
        /// <param name="mapping">The text element, or range of text elements to map.</param>
        /// <param name="code">The code of the mapping.</param>
        /// <exception cref="System.ArgumentException">System.ArgumentException</exception>
        /// <exception cref="System.ArgumentNullException"></exception>
        public void Map(string mapping, int code)
        {
            // the default encoding cannot be changed
            if (_currentMap == _maps.First().Value)
                return;

            if (string.IsNullOrEmpty(mapping))
                throw new ArgumentNullException();

            var stringInfo = StringInfo.ParseCombiningCharacters(mapping);
            if (stringInfo.Length > 1)
            {
                if (stringInfo.Length > 2)
                    throw new ArgumentException(mapping);

                var first = char.ConvertToUtf32(mapping, stringInfo.First());
                var last = char.ConvertToUtf32(mapping, stringInfo.Last());

                if (first > last)
                    throw new ArgumentException(mapping);

                while (first <= last)
                {
                    var firstAsStr = char.ConvertFromUtf32(first);
                    if (_currentMap.ContainsKey(firstAsStr))
                        _currentMap[firstAsStr] = code++;
                    else
                        _currentMap.Add(firstAsStr, code++);
                    first++;
                }
            }
            else
            {
                if (_currentMap.ContainsKey(mapping))
                    _currentMap[mapping] = code;
                else
                    _currentMap.Add(mapping, code);
            }
        }

        /// <summary>
        /// Adds a character mapping to translate from source to object
        /// </summary>
        /// <param name="mapping">The character to map</param>
        /// <param name="code">The code of the mapping.</param>
        /// <exception cref="T:System.ArgumentException">System.ArgumentException</exception>
        /// <exception cref="T:System.ArgumentNullException"></exception>
        public void Map(char mapping, int code)
        {
            Map(mapping.ToString(), code);
        }

        /// <summary>
        /// Remove a mapping for the current encoding.
        /// </summary>
        /// <param name="mapping">The text element, or range of text elements to unmap.</param>
        /// <exception cref="T:System.ArgumentException">System.ArgumentException</exception>
        /// <exception cref="T:System.ArgumentNullException"></exception>
        public void Unmap(string mapping)
        {
            // the default encoding cannot be changed
            if (_currentMap == _maps.First().Value)
                return;

            if (string.IsNullOrEmpty(mapping))
                throw new ArgumentNullException();

            var stringInfo = StringInfo.ParseCombiningCharacters(mapping);
            if (stringInfo.Length > 1)
            {
                if (stringInfo.Length > 2)
                    throw new ArgumentException(mapping);

                var first = char.ConvertToUtf32(mapping, stringInfo.First());
                var last = char.ConvertToUtf32(mapping, stringInfo.Last());

                if (first > last)
                    throw new ArgumentException(mapping);

                while (first <= last)
                {
                    var firstAsStr = char.ConvertFromUtf32(first++);
                    _currentMap.Remove(firstAsStr);
                }
            }
            else
            {
                _currentMap.Remove(mapping);
            }
        }

        /// <summary>
        /// Select the current encoding to the default UTF-8 encoding
        /// </summary>
        public void SelectDefaultEncoding()
        {
            _currentMap = _maps["none"];
        }

        /// <summary>
        /// Select the current named encoding
        /// </summary>
        /// <param name="encodingName">The encoding name</param>
        public void SelectEncoding(string encodingName)
        {
            if (!_maps.ContainsKey(encodingName))
                _maps.Add(encodingName, new Dictionary<string, int>());
            _currentMap = _maps[encodingName];
        }

        #region Encoding Methods

        /// <summary>
        /// Calculates the number of bytes produced by encoding the string.
        /// </summary>
        /// <param name="s">The string to encode</param>
        public override int GetByteCount(string s)
        {
            int numbytes = 0;
            var textEnumerator = StringInfo.GetTextElementEnumerator(s);
            while (textEnumerator.MoveNext())
            {
                var elem = textEnumerator.GetTextElement();
                if (_currentMap.ContainsKey(elem))
                    numbytes += ((long)_currentMap[elem]).Size();
                else
                    numbytes += UTF8.GetByteCount(elem);
            }
            return numbytes;
        }

        /// <summary>
        /// Calculates the number of bytes produced by encoding all the characters
        /// in the specified character array.
        /// </summary>
        /// <param name="chars">The character array containing the characters to encode</param>
        /// <param name="index">The index of the first character to encode</param>
        /// <param name="count">The number of characters to encode</param>
        /// <returns>The number of bytes produced by encoding the specified characters.</returns>
        /// <exception cref="T:System.IndexOutOfRangeException">System.IndexOutOfRangeException</exception>
        public override int GetByteCount(char[] chars, int index, int count)
        {
            var s = new string(chars.Skip(index).Take(count).ToArray());
            return GetByteCount(s);
        }

        /// <summary>
        /// Encodes a string into a byte array.
        /// </summary>
        /// <param name="s">The string to encode</param>
        /// <returns>The array of bytes representing the string encoding.</returns>
        /// <exception cref="T:System.ArgumentNullException">System.ArgumentNullException</exception>
        /// <exception cref="T:System.ArgumentException">System.ArgumentException</exception>
        public override byte[] GetBytes(string s)
        {
            var bytes = new List<byte>();
            var textEnumerator = StringInfo.GetTextElementEnumerator(s);

            while (textEnumerator.MoveNext())
            {
                var elem = textEnumerator.GetTextElement();
                bytes.AddRange(GetCharBytes(elem));
            }
            return bytes.ToArray();
        }

        /// <summary>
        /// Encodes a set of characters from the specified character
        /// array into the specified byte array.
        /// </summary>
        /// <param name="chars">The character array containing the set of characters to encode</param>
        /// <param name="charIndex">The index of the first character to encode</param>
        /// <param name="charCount">The number of characters to encode</param>
        /// <param name="bytes">The byte array to contain the resulting sequence of bytes</param>
        /// <param name="byteIndex">The index at which to start writing the resulting
        /// sequence of bytes</param>
        /// <returns>The actual number of bytes written into bytes.</returns>
        /// <exception cref="T:System.ArgumentNullException">System.ArgumentNullException</exception>
        /// <exception cref="T:System.ArgumentException">System.ArgumentException</exception>
        /// <exception cref="T:System.IndexOutOfRangeException">System.IndexOutOfRangeException</exception>
        public override int GetBytes(char[] chars, int charIndex, int charCount, byte[] bytes, int byteIndex)
        {
            var s = new string(chars.Skip(charIndex).Take(charCount).ToArray());
            var charBytes = GetBytes(s);
            var j = byteIndex;

            foreach (var b in charBytes)
                bytes[j++] = b;

            return j - byteIndex;
        }

        /// <summary>
        /// Calculates the number of characters produced by decoding a sequence of bytes
        /// from the specified byte array.
        /// </summary>
        /// <param name="bytes">The byte array containing the sequence of bytes to decode</param>
        /// <param name="index">The index of the first byte to decode</param>
        /// <param name="count">The number of bytes to decode</param>
        /// <returns>The number of characters produced by decoding the specified sequence of bytes.</returns>
        /// <exception cref="T:System.ArgumentNullException">System.ArgumentNullException</exception>
        /// <exception cref="T:System.ArgumentOutOfRangeException">System.ArgumentOutOfRangeException</exception>
        /// <exception cref="T:System.Text.TextDecoderFallbackException"></exception>
        public override int GetCharCount(byte[] bytes, int index, int count)
        {
            char[] chars = new char[GetMaxCharCount(count)];
            return GetChars(bytes, index, count, chars, 0);
        }

        /// <summary>
        /// Decodes a sequence of bytes from the specified byte array
        /// into the specified character array.
        /// </summary>
        /// <param name="bytes">The byte array containing the sequence of bytes to decode</param>
        /// <param name="byteIndex">The index of the first byte to decode</param>
        /// <param name="byteCount">The number of bytes to decode</param>
        /// <param name="chars">The character array to contain the resulting set of characters</param>
        /// <param name="charIndex">The index at which to start writing the resulting set of characters</param>
        /// <returns>The actual number of characters written into chars.</returns>
        /// <exception cref="T:System.ArgumentNullException">System.ArgumentNullException</exception>
        /// <exception cref="T:System.ArgumentOutOfRangeException">System.ArgumentOutOfRangeException</exception>
        /// <exception cref="T:System.ArgumentException">System.ArgumentException</exception>
        /// <exception cref="T:System.IndexOutOfRangeException">System.IndexOutOfRangeException</exception>
        public override int GetChars(byte[] bytes, int byteIndex, int byteCount, char[] chars, int charIndex)
        {
            int j = charIndex;
            int i = 0;
            while (i < byteCount)
            {
                int displ = 0;
                int encoding = 0;
                if (i + 3 + byteIndex < byteCount)
                {
                    encoding = bytes[i + byteIndex] |
                               (bytes[i + 1 + byteIndex] * 0x100) |
                               (bytes[i + 2 + byteIndex] * 0x10000) |
                               (bytes[i + 3 + byteIndex] * 0x1000000);
                    if (_currentMap.ContainsValue(encoding))
                    {
                        displ = 4;
                        goto SetChar;
                    }
                }
                if (i + 2 + byteIndex < byteCount)
                {
                    encoding = bytes[i + byteIndex] |
                               (bytes[i + 1 + byteIndex] * 0x100) |
                               (bytes[i + 2 + byteIndex] * 0x10000);
                    if (_currentMap.ContainsValue(encoding))
                    {
                        displ = 3;
                        goto SetChar;
                    }
                }
                if (i + 1 + byteIndex < byteCount)
                {
                    encoding = bytes[i + byteIndex] |
                               (bytes[i + 1 + byteIndex] * 0x100);
                    if (_currentMap.ContainsValue(encoding))
                    {
                        displ = 2;
                        goto SetChar;
                    }
                }
                encoding = bytes[i + byteIndex];
                if (_currentMap.ContainsValue(encoding))
                {
                    displ = 1;
                    goto SetChar;
                }

                var count = 1;
                var utfChars = UTF8.GetChars(bytes.Skip(i).ToArray(), 0, byteCount - i);

                if (char.IsSurrogate(utfChars.First()))
                    count++;

                utfChars = utfChars.Take(count).ToArray();
                foreach (var utfChar in utfChars)
                    chars[j++] = utfChar;

                i += UTF8.GetByteCount(utfChars);
                continue;

            SetChar:
                var key = _currentMap.First(e => e.Value.Equals(encoding)).Key;
                var utfchars = key.ToCharArray();
                foreach (var utfc in utfchars)
                    chars[j++] = utfc;
                i += displ;
            }
            return j - charIndex;
        }

        /// <summary>
        /// Calculates the maximum number of bytes
        /// produced by encoding the specified number of characters.
        /// </summary>
        /// <param name="charCount">The number of characters to encode</param>
        /// <returns>The maximum number of bytes produced by
        /// encoding the specified number of characters.</returns>
        public override int GetMaxByteCount(int charCount)
        {
            return charCount * sizeof(Int32);
        }

        /// <summary>
        /// Calculates the maximum number of characters produced
        /// by decoding the specified number of bytes.
        /// </summary>
        /// <param name="byteCount">The number of bytes to decod</param>
        /// <returns>The maximum number of characters produced by
        /// decoding the specified number of bytes.</returns>
        public override int GetMaxCharCount(int byteCount)
        {
            // An encoding could be mapped to a surrogate pair, so we must double max char count
            return byteCount * sizeof(char);
        }

        #endregion

        #endregion
    }
}
