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
		private static readonly Dictionary<string, byte> BytesByText;
		private static readonly string[] TextByBytes;

		static FF1Text()
		{
			BytesByText = new Dictionary<string, byte>();
			TextByBytes = new string[256];

			var lines = File.ReadAllLines("DTETable.txt");
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

		public static Blob TextToBytes(string text)
		{
			Blob bytes = new byte[text.Length];
			int i = 0, j = 0;
			while (i < text.Length - 1)
			{
				var twoChars = text.Substring(i, 2);
				if (BytesByText.ContainsKey(twoChars))
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

			return bytes.SubBlob(0, i);
		}
	}
}
