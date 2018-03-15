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

		private static readonly Dictionary<string, byte> BytesByText;
		private static readonly string[] TextByBytes;

		private static readonly Dictionary<char, byte> SymbolsByText = new Dictionary<char, byte>
		{
			{ '0', 0xD4 },
			{ '1', 0xD5 },
			{ '2', 0xD6 },
			{ '3', 0xD7 },
			{ '4', 0xD8 },
			{ '5', 0xD9 },
			{ '6', 0xDA },
			{ '7', 0xDB },
			{ '8', 0xDC },
			{ '9', 0xDD },
			{ 'A', 0xDE },
			{ 'B', 0xDF },
			{ 'C', 0xE0 },
			{ 'D', 0xE1 },
			{ 'E', 0xC2 },
			{ 'F', 0xC3 },
		};

		static FF1Text()
		{
			BytesByText = new Dictionary<string, byte>();
			TextByBytes = new string[256];

			var lines = File.ReadAllLines($"{AppContext.BaseDirectory}/DTETable.txt");
			foreach (var line in lines)
			{
				var sides = line.Split('=');
				if (sides[1] == "\\n")
				{
					sides[1] = "\n";
				}

				var b = byte.Parse(sides[0], NumberStyles.HexNumber);
				BytesByText[sides[1]] = b;
				TextByBytes[b] = sides[1];
			}
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
				if (useDTE && BytesByText.ContainsKey(twoChars))
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
				buffers.Add(TextToBytes(lines[i], useDTE: false, delimiter: Delimiter.Line));
			}

			if (buffers.Count != 0)
			{
				var lastBuffer = buffers[buffers.Count - 1];
				lastBuffer[lastBuffer.Length - 1] = (byte)Delimiter.Null;
			}

			return Blob.Concat(buffers);
		}

		public static Blob HexStringToSymbols(string hexStr, Delimiter delimiter = Delimiter.Null)
		{
			var bytes = hexStr.ToList().Select(digit => SymbolsByText[digit]).ToList();
			if (delimiter != Delimiter.Empty)
			{
				bytes.Add((byte)delimiter);
			}
			return bytes.ToArray();
		}
	}
}
