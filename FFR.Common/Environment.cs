namespace FFR.Common
{
  using System;
  using System.Runtime.InteropServices;

  public static class FFREnv
  {
    public static bool IsWindows
      => RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
  }
}
