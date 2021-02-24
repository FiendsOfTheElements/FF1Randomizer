using System;
using System.Runtime.InteropServices;

using System.IO;

namespace FFR.Common
{
	public static class UserSettings
	{
		private const string ffrDir = "FinalFantasyRandomizer";

		public static bool IsWindows
			=> RuntimeInformation.IsOSPlatform(OSPlatform.Windows);

		private static string directory;
		internal static string UserDirectory
		{
			get
			{
				if (!string.IsNullOrEmpty(directory))
				{
					return directory;
				}

				string envKey = IsWindows ? "LOCALAPPDATA" : "HOME";
				string baseDir = Environment.GetEnvironmentVariable(envKey);

				directory = IsWindows
					? Path.Combine(baseDir, ffrDir)
					: Path.Combine(baseDir, ".config", ffrDir);

				return directory;
			}
		}

		public static void WriteFile(string name, string contents, string category = "")
		{
			string path = GetFilePath(name, category);

			Directory.CreateDirectory(Path.GetDirectoryName(path));
			File.WriteAllText(path, contents);
		}

		public static void RemoveFile(string name, string category = "")
		{
			string path = GetFilePath(name, category);

			if (File.Exists(path))
			{
				File.Delete(path);
			}
		}

		public static string ReadFile(string name, string category = "")
		{
			string path = GetFilePath(name, category);

			if (!File.Exists(path))
			{
				throw new FileNotFoundException(path);
			}

			return File.ReadAllText(path);
		}

		internal static string GetFilePath(string file, string subdir = "")
		{
			return string.IsNullOrEmpty(subdir)
						   ? Path.Combine(UserDirectory, file)
						   : Path.Combine(UserDirectory, subdir, file);
		}
	}
}
