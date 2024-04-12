using RomUtilities;
using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FF1Lib
{
	// Enum
	public enum NewFlagOptions
	{
		None = 0,
		SomeModification
	}

	// DataClasses
	public class GameData
	{
		// ROM addresses are kept in class, no other class should have access
		private const int dataOffsetInTheROM = 0x8000;
		private const int bankInTheROM = 0x1F;
		private const int lengthofData = 0x100;

		private byte[] gamedata;
		// Read once, write once
		public GameData(FF1Rom rom, OtherGameData othergamedata)
		{
			gamedata = rom.GetFromBank(bankInTheROM, dataOffsetInTheROM, lengthofData);
		}
		public void ModifyData(Settings settings)
		{
			gamedata[11] = 0x12;
		}
		public void Write(FF1Rom rom)
		{
			rom.PutInBank(bankInTheROM, dataOffsetInTheROM, gamedata);
		}
	}

	public class OtherGameData
	{
		public OtherGameData() { }
	}

	// FlagRules
	// Used to convert standards flags into advanced flags 
	public static partial class FlagRules
	{
		public static FlagRule StandardFlag { get; set; } = new FlagRule()
		{
			Name = "StandarFlag",
			Type = SettingType.Toggle,
			Value = 1,
			Actions = new List<FlagAction>()
				{
					new FlagAction() { Setting = "SetSomeOtherFlag", Action = FlagActions.Enable },
					new FlagAction() { Setting = "SetAnotherFlag", Action = FlagActions.Enable },
				}
		};
	}

	// Actual ROM structure
	public partial class FF1Rom
	{
		// Only for small hacks and legacy features, settings here isn't strictly necessary
		private void WriteDataDirectly(Settings settings)
		{
			if (settings.GetBool("SomeFlag"))
			{
				PutInBank(0x1F, 0xF000, Blob.FromHex("208180"));
			}
		}

	}
}
