namespace FFR.Common
{
	using System;
	using System.Collections.Generic;
	using System.IO;
	using System.Linq;
	using Newtonsoft.Json;

	using FF1Lib;
	using FFR.Common.StringExtensions;

	/// <summary>
	/// Represents a set of <c cref="FF1Lib.Flags">Flags</c> given a 
	/// human-friendly name intended to be reused across many runs.
	/// </summary>
	public struct Preset
	{
		public string Name { get; set; }
		public Flags Flags { get; set; }

		public string FlagString =>
			FF1Lib.Flags.EncodeFlagsText(Flags);

		public Preset(string name, Flags flags)
		{
			Name = name;
			Flags = flags;
		}

		public string ToJSON()
			=> Preset.Marshal(this);


		/// <summary>
		/// Convert a Preset to JSON.
		/// </summary>
		public static string Marshal(Preset preset)
			=> JsonConvert.SerializeObject(preset);

		/// <summary>
		/// Convert JSON to a Preset.
		/// </summary>
		public static Preset Unmarshal(string json)
			=> JsonConvert.DeserializeObject<Preset>(json);

	}

	/// <summary>
	/// Public API to interact with the preset system.
	/// Contains methods to add, remove and list presets for the current user.
	/// </summary>
	/// <remarks>
	/// <para>
	/// User presets are stored in the "presets" sub-directory of the 
	/// FFR config directory for the host OS, as a single JSON file using 
	/// preset's name transformed into a "slug".
	/// </para> 
	/// <para>
	/// On Windows, this is <c>%LOCALAPPDATA%\FinalFantasyRandomizer\presets\</c>
	/// </para>
	/// <para>
	/// On other operating systems, it is
	/// <c>$HOME/.config/FinalFantasyRandomizer</c>
	/// </remarks>
	/// <example>
	/// Adding a new Preset:
	/// <code>
	/// // Using a flag string...
	/// Presets.Add("My New Preset", "base64FlagsCopiedFromAUI");
	/// // ...or a Flags object.
	/// Presets.Add("My Other New Preset", new Flags());
	/// </code>
	///
	/// List presets available to the current user:
	/// <code>
	/// foreach(var i in Presets.List())
	/// {
	///   Console.WriteLine(i);
	/// }
	///
	/// // => my-new-preset
	/// // => my-other-new-preset
	/// </code>
	///
	/// Permanently remove a preset:
	/// <code>
	/// // Using the original name...
	/// Preset.Remove("My New Preset");
	///
	/// // ...or the file name (slug)
	/// Preset.Remove("my-other-new-preset");
	/// </code>
	/// </example>
	public static class Presets
	{
		/// <summary>
		/// Permanently add a preset to the current user's settings.
		/// </summary>
		public static void Add(string name, Flags flags)
		{
			var preset = new Preset(name, flags);

			UserSettings.WriteFile(
				$"{preset.Name.ToSlug()}.json",
				preset.ToJSON(),
				"presets"
			);
		}

		/// <summary>
		/// Permanently add a preset to the current user's settings.
		/// </summary>
		public static void Add(string name, string flags)
			=> Add(name, Flags.DecodeFlagsText(flags));

		/// <summary>
		/// Permanently remove a preset from the current user's settings.
		/// </summary>
		public static void Remove(string name)
			=> UserSettings.RemoveFile($"{name.ToSlug()}.json", "presets");

		/// <summary>
		/// List all presets available to the current user.
		/// </summary>
		public static IEnumerable<string> List()
			=> Directory
			     .EnumerateFiles(UserSettings.GetFilePath("presets"), "*.json")
			     .Select(i => Path.GetFileNameWithoutExtension(i));

		/// <summary>
		/// Load an existing preset from the current user's settings.
		/// </summary>
		public static Preset Load(string name)
		{
			var json = UserSettings.ReadFile($"{name.ToSlug()}.json", "presets");
			return Preset.Unmarshal(json);
		}
	}
}
