using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace FF1Lib.Music
{
	public static class MusicAssembler
	{
		private static readonly Regex NotePattern = new Regex("^(\\d+\\.?)([A-G]#?b?)(\\d)$", RegexOptions.Compiled);
		private static readonly Regex RestPattern = new Regex("^(\\d+\\.?)R$", RegexOptions.Compiled);
		private static readonly Regex ControlPattern = new Regex("^([est])(\\d)$", RegexOptions.Compiled);

		private static readonly Dictionary<string, byte> NoteValues = new Dictionary<string, byte>
		{
			["Cb"] = 11, ["C"] = 0,  ["C#"] = 1,
			["Db"] = 1,  ["D"] = 2,  ["D#"] = 3,
			["Eb"] = 3,  ["E"] = 4,  ["E#"] = 5,
			["Fb"] = 4,  ["F"] = 5,  ["F#"] = 6,
			["Gb"] = 6,  ["G"] = 7,  ["G#"] = 8,
			["Ab"] = 8,  ["A"] = 9,  ["A#"] = 10,
			["Bb"] = 10, ["B"] = 11, ["B#"] = 0,
		};

		private static readonly Dictionary<string, byte> LengthValues = new Dictionary<string, byte>
		{
			["1."] = 0,
			["1"] = 1,
			["2."] = 2,
			["2"] = 3,
			["4."] = 4,
			["4"] = 5,
			["8."] = 6,
			["8"] = 7,
			["16."] = 8,
			["16"] = 9,
			["32."] = 10,
			["32"] = 11
		};

		public static (List<byte> square1, List<byte> square2, List<byte> triangle) AssembleMusic(string inputFile)
		{
			var square1 = new List<byte>();
			var square2 = new List<byte>();
			var triangle = new List<byte>();

			var square1Octave = -1;
			var square2Octave = -1;
			var triangleOctave = -1;

			var lines = inputFile.Split(new[] { "\r\n", "\n\r", "\n", "\r" }, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
			foreach (var line in lines)
			{
				var uncommentedLine = line;
				var commentIndex = line.IndexOf("//");
				if (commentIndex != -1)
					uncommentedLine = line.Substring(0, commentIndex);

				if (uncommentedLine.Trim() == "")
					continue;

				var tokens = uncommentedLine.Split(new[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

				if (tokens[0] == "sq1" || tokens[0] == "square1")
					square1.AddRange(ProcessLine(tokens, ref square1Octave));
				if (tokens[0] == "sq2" || tokens[0] == "square2")
					square2.AddRange(ProcessLine(tokens, ref square2Octave));
				if (tokens[0] == "tri" || tokens[0] == "triangle")
					triangle.AddRange(ProcessLine(tokens, ref triangleOctave));
			}

			square1.Add(0xFF);
			square2.Add(0xFF);
			triangle.Add(0xFF);

			return (square1, square2, triangle);
		}

		private static List<byte> ProcessLine(string[] tokens, ref int lastOctave)
		{
			var bytes = new List<byte>();
			foreach (var token in tokens)
			{
				if (token == "|")
					continue;

				var noteMatch = NotePattern.Match(token);
				if (noteMatch.Success)
				{
					var note = NoteValues[noteMatch.Groups[2].Value];
					var length = LengthValues[noteMatch.Groups[1].Value];
					var octave = int.Parse(noteMatch.Groups[3].Value);

					if (length < 0 || length > 15)
						throw new InvalidOperationException("Invalid note length " + noteMatch);
					if (octave < 0 || octave > 3)
						throw new InvalidOperationException("Invalid note octave " + noteMatch);

					if (octave != lastOctave)
					{
						bytes.Add((byte)(0xD8 + octave));
						lastOctave = octave;
					}

					bytes.Add((byte)((note << 4) | length));
				}

				var restMatch = RestPattern.Match(token);
				if (restMatch.Success)
				{
					var length = LengthValues[restMatch.Groups[1].Value];
					if (length < 0 || length > 15)
						throw new InvalidOperationException("Invalid rest length " + restMatch);

					bytes.Add((byte)(0xC0 | length));
				}

				var controlMatch = ControlPattern.Match(token);
				if (controlMatch.Success)
				{
					var control = controlMatch.Groups[1].Value;
					var value = int.Parse(controlMatch.Groups[2].Value);
					if (control == "e")
					{
						if (value < 0 || value > 15)
							throw new InvalidOperationException("Invalid envelope " + control + value.ToString());

						bytes.Add((byte)(0xE0 | value));
					}
					if (control == "s")
					{
						if (value < 0 || value > 15)
							throw new InvalidOperationException("Invalid envelope speed " + control + value.ToString());

						bytes.Add(0xF8);
						bytes.Add((byte)value);
					}
					if (control == "t")
					{
						if (value < 0 || value > 5)
							throw new InvalidOperationException("Invalid tempo " + control + value.ToString());

						bytes.Add((byte)(0xF9 + value));
					}
				}
			}

			return bytes;
		}
	}
}
