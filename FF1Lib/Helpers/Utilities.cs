namespace FF1Lib
{
	public static class Utilities
	{
		public static string SpoilerCache { get; set; } = "";

		public static string FlagCache { get; set; } = "";

		public static string ArchipelagoCache { get; set; } = "";

		public static string ProcgenWaterfallCache { get; set; } = "";

		/// <summary>
		/// Outputs the spoiler log entry to a text file or to the console
		/// </summary>
		/// <param name="text">Content that will be written to the desired output stream. If going to the spoiler log, a newline will be added for you.</param>
		/// <param name="toConsole">Outputs to the console if true. Default is to output to a text file that will be downloaded to the user.</param>
		public static void WriteSpoilerLine(string text, bool toConsole = false)
		{
			if (toConsole)
			{
				// Easy! Just output it to the console!
				Console.WriteLine(text);
			}
			else
			{
				// Add the text to the spoiler cache
				SpoilerCache += $"{text}\n";
			}
		}
	}
}
