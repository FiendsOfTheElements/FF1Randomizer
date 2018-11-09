namespace FFR.Common
{
	using System;

	/// <summary>
	/// Represents versioning information for a module or program.
	/// </summary>
	public struct VersionInfo {
		public int Major { get; }
		public int Minor { get; }
		public int Revision { get; }
		public string Tag { get; }

		public VersionInfo(int major, int minor = 0, int revision = 0, string tag = null)
		{
			Major = major;
			Minor = minor;
			Revision = revision;
			Tag = tag;
		}

		public override string ToString()
		{
			var semantics = $"{Major}.{Minor}.{Revision}";

			return string.IsNullOrEmpty(Tag)
				? semantics
				: $"{semantics}-{Tag}";
		}
	}
}
