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
				throw new ArgumentOutOfRangeException();
			}

			var flagLeft = new string(' ', (int)Math.Ceiling((32 - text.Length) / 2.0));
			var flagRight = new string(' ', (int)Math.Floor((32 - text.Length) / 2.0));

			return TextToBytes(flagLeft + text + flagRight, false, Delimiter.Empty);
		}
	}
}
