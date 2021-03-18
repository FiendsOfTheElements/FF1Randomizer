using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RomUtilities;

namespace FF1Lib
{
	// ReSharper disable once InconsistentNaming
	public static class FF1Text
	{
		public enum Delimiter
		{
			Empty = 0x100,
			Null = 0x00,
			Segment = 0x01,
			Line = 0x05
		}

		private static readonly string[] TextByBytes;
		private static readonly Dictionary<string, byte> BytesByText = new Dictionary<string, byte>
		{
			{ "#", 0x03 },
			{ "\n", 0x05 },
			{ "e ", 0x1A },
			{ " t", 0x1B },
			{ "th", 0x1C },
			{ "he", 0x1D },
			{ "s ", 0x1E },
			{ "in", 0x1F },
			{ " a", 0x20 },
			{ "t ", 0x21 },
			{ "an", 0x22 },
			{ "re", 0x23 },
			{ " s", 0x24 },
			{ "er", 0x25 },
			{ "ou", 0x26 },
			{ "d ", 0x27 },
			{ "to", 0x28 },
			{ "n ", 0x29 },
			{ "ng", 0x2A },
			{ "ea", 0x2B },
			{ "es", 0x2C },
			{ " i", 0x2D },
			{ "o ", 0x2E },
			{ "ar", 0x2F },
			{ "is", 0x30 },
			{ " b", 0x31 },
			{ "ve", 0x32 },
			{ " w", 0x33 },
			{ "me", 0x34 },
			{ "or", 0x35 },
			{ " o", 0x36 },
			{ "st", 0x37 },
			{ " c", 0x38 },
			{ "at", 0x39 },
			{ "en", 0x3A },
			{ "nd", 0x3B },
			{ "on", 0x3C },
			{ "hi", 0x3D },
			{ "se", 0x3E },
			{ "as", 0x3F },
			{ "ed", 0x40 },
			{ "ha", 0x41 },
			{ " m", 0x42 },
			{ " f", 0x43 },
			{ "r ", 0x44 },
			{ "le", 0x45 },
			{ "ow", 0x46 },
			{ "g ", 0x47 },
			{ "ce", 0x48 },
			{ "om", 0x49 },
			{ "GI", 0x4A },
			{ "y ", 0x4B },
			{ "of", 0x4C },
			{ "ro", 0x4D },
			{ "ll", 0x4E },
			{ " p", 0x4F },
			{ " y", 0x50 },
			{ "ca", 0x51 },
			{ "MA", 0x52 },
			{ "te", 0x53 },
			{ "f ", 0x54 },
			{ "ur", 0x55 },
			{ "yo", 0x56 },
			{ "ti", 0x57 },
			{ "l ", 0x58 },
			{ " h", 0x59 },
			{ "ne", 0x5A },
			{ "it", 0x5B },
			{ "ri", 0x5C },
			{ "wa", 0x5D },
			{ "ac", 0x5E },
			{ "al", 0x5F },
			{ "we", 0x60 },
			{ "il", 0x61 },
			{ "be", 0x62 },
			{ "rs", 0x63 },
			{ "u ", 0x64 },
			{ " l", 0x65 },
			{ "ge", 0x66 },
			{ " d", 0x67 },
			{ "li", 0x68 },
			{ "..", 0x69 },
			{ "/", 0x7A },
			{ "+", 0x7B },
			{ "0", 0x80 },
			{ "1", 0x81 },
			{ "2", 0x82 },
			{ "3", 0x83 },
			{ "4", 0x84 },
			{ "5", 0x85 },
			{ "6", 0x86 },
			{ "7", 0x87 },
			{ "8", 0x88 },
			{ "9", 0x89 },
			{ "A", 0x8A },
			{ "B", 0x8B },
			{ "C", 0x8C },
			{ "D", 0x8D },
			{ "E", 0x8E },
			{ "F", 0x8F },
			{ "G", 0x90 },
			{ "H", 0x91 },
			{ "I", 0x92 },
			{ "J", 0x93 },
			{ "K", 0x94 },
			{ "L", 0x95 },
			{ "M", 0x96 },
			{ "N", 0x97 },
			{ "O", 0x98 },
			{ "P", 0x99 },
			{ "Q", 0x9A },
			{ "R", 0x9B },
			{ "S", 0x9C },
			{ "T", 0x9D },
			{ "U", 0x9E },
			{ "V", 0x9F },
			{ "W", 0xA0 },
			{ "X", 0xA1 },
			{ "Y", 0xA2 },
			{ "Z", 0xA3 },
			{ "a", 0xA4 },
			{ "b", 0xA5 },
			{ "c", 0xA6 },
			{ "d", 0xA7 },
			{ "e", 0xA8 },
			{ "f", 0xA9 },
			{ "g", 0xAA },
			{ "h", 0xAB },
			{ "i", 0xAC },
			{ "j", 0xAD },
			{ "k", 0xAE },
			{ "l", 0xAF },
			{ "m", 0xB0 },
			{ "n", 0xB1 },
			{ "o", 0xB2 },
			{ "p", 0xB3 },
			{ "q", 0xB4 },
			{ "r", 0xB5 },
			{ "s", 0xB6 },
			{ "t", 0xB7 },
			{ "u", 0xB8 },
			{ "v", 0xB9 },
			{ "w", 0xBA },
			{ "x", 0xBB },
			{ "y", 0xBC },
			{ "z", 0xBD },
			{ "'", 0xBE },
			{ ",", 0xBF },
			{ ".", 0xC0 },
			{ ";", 0xC1 }, //should never be used as a space character
			{ "-", 0xC2 },
			// this is a duplicate { "..", 0xC3 },
			{ "!", 0xC4 },
			{ "?", 0xC5 },
			{ "@S", 0xD4 },
			{ "@H", 0xD5 },
			{ "@K", 0xD6 },
			{ "@X", 0xD7 },
			{ "@F", 0xD8 },
			{ "@N", 0xD9 },
			{ "@A", 0xDA },
			{ "@s", 0xDB },
			{ "@h", 0xDC },
			{ "@G", 0xDD },
			{ "@B", 0xDE },
			{ "@T", 0xDF },
			{ "%", 0xE0 },
			{ "@p", 0xE1 },
			{ " ", 0xFF }
		};

		static FF1Text()
		{
			TextByBytes = new string[256];
			foreach (var key in BytesByText.Keys)
			{
				TextByBytes[BytesByText[key]] = key;
			}

			TextByBytes[0xC3] = "..";
		}

		public static string BytesToText(byte[] bytes)
		{
			var builder = new StringBuilder();
			foreach (var b in bytes)
			{
				builder.Append(TextByBytes[b]);
			}

			return builder.ToString();
		}

		public static Blob TextToBytes(string text, bool useDTE = true, Delimiter delimiter = Delimiter.Null)
		{
			Blob bytes = new byte[text.Length + 1];
			int i = 0, j = 0;
			while (i < text.Length - 1)
			{
				var twoChars = text.Substring(i, 2);
				if (BytesByText.ContainsKey(twoChars) && (useDTE || twoChars[0] == '@'))
				{
					bytes[j++] = BytesByText[twoChars];
					i += 2;
				}
				else
				{
					bytes[j++] = BytesByText[text[i++].ToString()];
				}
			}

			if (i < text.Length)
			{
				bytes[j++] = BytesByText[text[i++].ToString()];
			}

			if (delimiter != Delimiter.Empty)
			{
				bytes[j++] = (byte)delimiter;
			}

			return bytes.SubBlob(0, j);
		}

		public static Blob TextToBytesInfo(string text, bool useDTE = true, Delimiter delimiter = Delimiter.Null)
		{
			Blob bytes = new byte[text.Length + 1];
			int i = 0, j = 0;
			while (i < text.Length - 1)
			{
				var twoChars = text.Substring(i, 2);
				if (twoChars[0] == '¤') // Control Code 0x14 for second words table
				{
					bytes[j++] = 0x14;
					bytes[j++] = Blob.FromHex(text.Substring(i+1,2))[0];
					i += 3;
				}
				else if (twoChars[0] == '$') // Control code 0x02 for itemnames table
				{
					bytes[j++] = 0x02;
					bytes[j++] = Blob.FromHex(text.Substring(i + 1, 2))[0];
					i += 3;
				}
				else if (BytesByText.ContainsKey(twoChars) && (useDTE || twoChars[0] == '@'))
				{
					bytes[j++] = BytesByText[twoChars];
					i += 2;
				}
				else
				{
					bytes[j++] = BytesByText[text[i++].ToString()];
				}
			}

			if (i < text.Length)
			{
				bytes[j++] = BytesByText[text[i++].ToString()];
			}

			if (delimiter != Delimiter.Empty)
			{
				bytes[j++] = (byte)delimiter;
			}

			return bytes.SubBlob(0, j);
		}

		// This wraps TextToBytes for use with Credits pages.
		public static Blob TextToCredits(string[] lines)
		{
			// Starting PPU addr immediately inside the box without any padding.
			// Each line is 0x20 total characters.
			ushort topLeftOfBox = 0x20A5;

			List<Blob> buffers = new List<Blob>();
			for (int i = 0; i < lines.Length; ++i)
			{
				string line = lines[i].Trim();
				if (line == "")
				{
					continue;
				}

				int spaces = lines[i].Length - lines[i].TrimStart(' ').Length;
				ushort[] ppuPtr = { (ushort)(topLeftOfBox + (0x20 * i) + spaces) };
				buffers.Add(Blob.FromUShorts(ppuPtr));
				buffers.Add(TextToBytes(line, useDTE: false, delimiter: Delimiter.Segment));
			}

			if (buffers.Count != 0)
			{
				var lastBuffer = buffers[buffers.Count - 1];
				lastBuffer[lastBuffer.Length - 1] = (byte)Delimiter.Null;
			}

			return Blob.Concat(buffers);
		}

		// This wraps TextToBytes for use with Story pages (Before Credits and End of Game).
		public static Blob TextToStory(string[] lines)
		{
			List<Blob> buffers = new List<Blob>();
			for (int i = 0; i < lines.Length; ++i)
			{
				buffers.Add(TextToBytes(lines[i], useDTE: true, delimiter: Delimiter.Line));
			}

			if (buffers.Count != 0)
			{
				var lastBuffer = buffers[buffers.Count - 1];
				lastBuffer[lastBuffer.Length - 1] = (byte)Delimiter.Null;
			}

			return Blob.Concat(buffers);
		}

		// Returns a centered, whitespace padded line of 32 characters
		public static Blob TextToCopyrightLine(string text)
		{
			if (text.Length > 32)
			{
				text = text.Substring(0, 32);
				//throw new ArgumentOutOfRangeException();
			}

			var flagLeft = new string(' ', (int)Math.Ceiling((32 - text.Length) / 2.0));
			var flagRight = new string(' ', (int)Math.Floor((32 - text.Length) / 2.0));

			return TextToBytes(flagLeft + text + flagRight, false, Delimiter.Empty);
		}
	}
}
