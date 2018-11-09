namespace FFR.Common.Tests
{
	using System;
	using Xunit;

	using FFR.Common;

	public class PresetTests
	{
		[Theory]
		[InlineData("I shouldn't exist, really", "HACBG9ANbBHAHI!fPAPeZZeAAeP")]
		[InlineData("League 6 Test", "PAC!P3hP4EBQHJ!fPAVoYAeAFeV")]
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
