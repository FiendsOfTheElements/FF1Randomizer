namespace FFR.Common.Tests
{
	using System;
	using Xunit;

	using FFR.Common;

	public class PresetTests
	{
		[Theory]
		[InlineData("I shouldn't exist, really", "!AK9f1!BAABAPI!fPgPoooPABPPAAAAGHABA")]
		[InlineData("Swiss Style Test", "HACNHEBFYAAAHI!fPAPoYIeAAePAAAAAAABB")]
		public void SavingAndLoadingCustom(string name, string flags)
		{
			Presets.Add(name, flags);

			var loaded = Presets.Load(name);

			Assert.Equal(name, loaded.Name);
			Assert.Equal(flags, loaded.FlagString);

			Presets.Remove(name);
		}
	}
}
