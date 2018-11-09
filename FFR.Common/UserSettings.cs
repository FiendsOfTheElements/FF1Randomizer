namespace FFR.Common
{
	using System;
	using System.Runtime.InteropServices;

	using System.IO;

	public static class UserSettings
	{
		const string ffrDir = "FinalFantasyRandomizer";

		public static bool IsWindows
			=> RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
    
		static string directory;
		internal static string UserDirectory
		{
			get
			{
				if (!string.IsNullOrEmpty(directory)) return directory;

				var envKey = IsWindows ? "LOCALAPPDATA" : "HOME";
				var baseDir = Environment.GetEnvironmentVariable(envKey);

				directory = IsWindows
					? System.IO.Path.Combine(baseDir, ffrDir)
					: System.IO.Path.Combine(baseDir, ".config", ffrDir);

				return directory;
			}
		}

		public static void WriteFile(string name, string contents, string category = "")
		{
			var path = GetFilePath(name, category);

			Directory.CreateDirectory(Path.GetDirectoryName(path));
			File.WriteAllText(path, contents);
		}

		public static void RemoveFile(string name, string category = "")
		{
			var path = GetFilePath(name, category);

			if (File.Exists(path))
				File.Delete(path);
		}

		public static string ReadFile(string name, string category = "")
		{
			var path = GetFilePath(name, category);

			if (!File.Exists(path))
				throw new FileNotFoundException(path);

			return File.ReadAllText(path);
		}

		internal static string GetFilePath(string file, string subdir = "")
			=> String.IsNullOrEmpty(subdir)
				? System.IO.Path.Combine(UserDirectory, file)
				: System.IO.Path.Combine(UserDirectory, subdir, file);
	}
}
