namespace FFR.Common
{
	using System;

	using FF1Lib;
	using RomUtilities;

	/// <summary>
	/// Represents a set of randomization settings to supply
	/// to ROM generation.
	/// </summary>
	public struct RandomizerSettings {
		public Blob Seed { get; }
		public Flags Flags { get; }

		public string SeedString
			=> Seed.ToHex();

		public string FlagString
			=> FF1Lib.Flags.EncodeFlagsText(Flags);

		public RandomizerSettings(Flags flags)
			: this(Blob.Random(4), flags) { }

		public RandomizerSettings(string seed, Flags flags)
			: this(SettingsUtils.ConvertSeed(seed), flags) { }

		public RandomizerSettings(Blob seed, Flags flags)
		{
			Seed = seed;
			Flags = flags;
		}

		public RandomizerSettings(string seed, string flags)
			: this(
				SettingsUtils.ConvertSeed(seed),
				SettingsUtils.ConvertFlags(flags)
			) { }

		public static RandomizerSettings FromImportString(string import)
		{
			var seed = import.Substring(0, 8);
			var flags = import.Substring(9);

			return new RandomizerSettings(seed, flags);
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
