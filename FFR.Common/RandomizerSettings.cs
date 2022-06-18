namespace FFR.Common
{
	using System;
	using System.IO;
	using FF1Lib;
	using RomUtilities;

	/// <summary>
	/// Represents a set of randomization settings to supply
	/// to ROM generation.
	/// </summary>
	public struct RandomizerSettings {
		public Blob Seed { get; }
		public Flags Flags { get; }
		public Preferences Preferences { get; }

		public string SeedString
			=> Seed.ToHex();

		public string FlagString
			=> FF1Lib.Flags.EncodeFlagsText(Flags);

		public RandomizerSettings(Flags flags, Preferences preferences)
			: this(Blob.Random(4), flags, preferences) { }

		public RandomizerSettings(string seed, Flags flags, Preferences preferences)
			: this(SettingsUtils.ConvertSeed(seed), flags, preferences) { }

		public RandomizerSettings(Blob seed, Flags flags, Preferences preferences)
		{
			Seed = seed;
			Flags = flags;
			Preferences = preferences;
		}

		public RandomizerSettings(string seed, string flags)
			: this(
				SettingsUtils.ConvertSeed(seed),
				SettingsUtils.ConvertFlags(flags),
				new Preferences()
			) { }

		public static RandomizerSettings FromImportString(string import)
		{
			var seed = import.Substring(0, 8);
			var flags = import.Substring(9);

			return new RandomizerSettings(seed, flags);
		}

	    public static RandomizerSettings FromJson(string seed, string jsonpath)
		{
		    string txt = File.ReadAllText(jsonpath);
		    var flags = new Flags();
		    flags.LoadFromJson(txt);
		    return new RandomizerSettings(seed, flags, new Preferences());
		}

	}

	static class SettingsUtils {
		/// <summary>
		/// Convert a string composed of hexadecimal characters into a Blob.
		/// Returns a random Blob on an empty string.
		/// </summary>
		// Will raise an exception if the input string has characters outside
		// the [a-fA-f0-9] range.
		public static Blob ConvertSeed(string maybeHex)
			=> String.IsNullOrEmpty(maybeHex)
				? Blob.Random(4)
				: Blob.FromHex(maybeHex.Substring(0, 8));


		public static Flags ConvertFlags(string maybeFlags)
			=> String.IsNullOrEmpty(maybeFlags)
				? new Flags()
				: Flags.DecodeFlagsText(maybeFlags);
	}
}
