namespace FFR.Common
{
  using System;

  public static class UserSettings
  {
    const string ffrDir = "FinalFantasyRandomizer";

    static string directory;
    internal static string Directory
    {
      get
      {
        if (!string.IsNullOrEmpty(directory)) return directory;

        var envKey = FFREnv.IsWindows ? "LOCALAPPDATA" : "HOME";
        var baseDir = Environment.GetEnvironmentVariable(envKey);
        directory = FFREnv.IsWindows
          ? System.IO.Path.Combine(baseDir, ffrDir)
          : System.IO.Path.Combine(baseDir, ".config", ffrDir);

        return directory;
      }
    }

    internal static string GetFilePath(string file)
    {
      return System.IO.Path.Combine(Directory, file);
    }
  }
}
