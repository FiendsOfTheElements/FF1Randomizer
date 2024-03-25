using System.ComponentModel;

namespace FF1Lib
{
	public enum ChanceToRunMode
	{
		[Description("Bugged")]
		Vanilla,

		[Description("Corrected")]
		Fixed,

		[Description("Fixed 25%")]
		Fix25,

		[Description("Fixed 40%")]
		Fix40,

		[Description("Fixed 55%")]
		Fix55,

		[Description("Fixed 70%")]
		Fix70,

		[Description("Fixed 85%")]
		Fix85,

		[Description("Fixed 100%")]
		Fix100
	}

	public class ChanceToRun
	{
		FF1Rom rom;
		Settings settings;

		public ChanceToRun(FF1Rom _rom, Settings _settings)
		{
			rom = _rom;
			settings = _settings;
		}

		public void FixChanceToRun()
		{
			switch (settings.GetEnum("ChanceToRun", typeof(ChanceToRunMode)))
			{
				case ChanceToRunMode.Vanilla:
					break;
				case ChanceToRunMode.Fixed:
					//Load Level instead of whatever the vanilla game loads
					rom.Put(0x323EF, new byte[] { 0x82 });
					break;
				case ChanceToRunMode.Fix25:
					//Load 100 instead of level, the CLC can stay, remove Add 16
					rom.Put(0x323EE, new byte[] { 0xA9, 0x63, 0x18, 0xEA, 0xEA });
					//Load 25 instead of Luck
					rom.Put(0x323FF, new byte[] { 0xA9, 0x19});
					break;
				case ChanceToRunMode.Fix40:
					rom.Put(0x323EE, new byte[] { 0xA9, 0x63, 0x18, 0xEA, 0xEA });
					rom.Put(0x323FF, new byte[] { 0xA9, 0x28 });
					break;
				case ChanceToRunMode.Fix55:
					rom.Put(0x323EE, new byte[] { 0xA9, 0x63, 0x18, 0xEA, 0xEA });
					rom.Put(0x323FF, new byte[] { 0xA9, 0x37 });
					break;
				case ChanceToRunMode.Fix70:
					rom.Put(0x323EE, new byte[] { 0xA9, 0x63, 0x18, 0xEA, 0xEA });
					rom.Put(0x323FF, new byte[] { 0xA9, 0x46 });
					break;
				case ChanceToRunMode.Fix85:
					rom.Put(0x323EE, new byte[] { 0xA9, 0x63, 0x18, 0xEA, 0xEA });
					rom.Put(0x323FF, new byte[] { 0xA9, 0x55 });
					break;
				case ChanceToRunMode.Fix100:
					rom.Put(0x323EE, new byte[] { 0xA9, 0x63, 0x18, 0xEA, 0xEA });
					rom.Put(0x323FF, new byte[] { 0xA9, 0x64 });
					break;

			}
		}
	}
}
