using RomUtilities;
using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FF1Lib
{
	public static partial class FlagRules
	{
		public static FlagRule GlobalImprovements { get; set; } = new FlagRule()
		{
			Conditions = new() { new() { new FlagCondition() { Name = "GlobalImprovements", Type = SettingType.Toggle, Value = 1 } } },
			Actions = new List<FlagAction>()
				{
					new FlagAction() { Setting = "NoPartyShuffle", Action = FlagActions.Enable },
					new FlagAction() { Setting = "IdentifyTreasures", Action = FlagActions.Enable },
					new FlagAction() { Setting = "EnableBuyQuantity", Action = FlagActions.Enable },
					new FlagAction() { Setting = "WaitWhenUnrunnable", Action = FlagActions.Enable },
					new FlagAction() { Setting = "EnableCritNumberDisplay", Action = FlagActions.Enable },
					new FlagAction() { Setting = "SpeedHacks", Action = FlagActions.Enable },
					new FlagAction() { Setting = "InventoryAutosort", Action = FlagActions.Enable },
					new FlagAction() { Setting = "EnableDash", Action = FlagActions.Enable },
					new FlagAction() { Setting = "SpeedBoat", Action = FlagActions.Enable },
				}
		};
	}
	public partial class FF1Rom
	{
		public const int PartyShuffleOffset = 0x312E0;
		public const int PartyShuffleSize = 3;

		public void GlobalImprovements(Settings settings, Preferences preferences)
		{
			if (settings.GetBool("NoPartyShuffle"))
			{
				DisablePartyShuffle();
			}

			if (settings.GetBool("IdentifyTreasures"))
			{
				EnableIdentifyTreasures();
			}

			if (settings.GetBool("EnableBuyQuantity") || settings.GetBool("ArchipelagoEnabled"))
			{
				EnableBuyQuantity(settings.GetBool("ArchipelagoEnabled"));
			}

			if (settings.GetBool("WaitWhenUnrunnable"))
			{
				ChangeUnrunnableRunToWait();
			}

			if (settings.GetBool("SpeedHacks") && settings.GetBool("EnableCritNumberDisplay"))
			{
				EnableCritNumberDisplay();
			}

			if (settings.GetBool("InventoryAutosort") && !(preferences.RenounceAutosort))
			{
				EnableInventoryAutosort(settings.GetInt("GameMode") == (int)GameModes.NoOverworld);
			}

			if (settings.GetBool("SpeedHacks"))
			{
				EnableSpeedHacks(preferences);
			}

			if (settings.GetBool("EnableDash") || settings.GetBool("SpeedBoat"))
			{
				EnableDash(settings.GetBool("SpeedBoat"), preferences.OptOutSpeedHackDash);
			}

			if (settings.GetBool("SpeedHacks"))
			{
				SpeedHacksMoveNpcs(!settings.GetBool("ProcgenEarth"));
			}

		}

		public void DisablePartyShuffle()
		{
			var nops = new byte[PartyShuffleSize];
			for (int i = 0; i < nops.Length; i++)
			{
				nops[i] = Nop;
			}
			Put(PartyShuffleOffset, nops);

			nops = new byte[2];
			for (int i = 0; i < nops.Length; i++)
			{
				nops[i] = Nop;
			}
			Put(0x39A6B, nops);
			Put(0x39A74, nops);
		}

		// Dialogues class
		public void EnableIdentifyTreasures()
		{
			InsertDialogs(0xF1 + 0x50, "Can't hold\n#");
			InsertDialogs(0xF1, "Can't hold\n#");
		}

		public void EnableBuyQuantity(bool archipelagoenabled)
		{
			Put(0x39E00, Blob.FromHex("ad0a0385104c668eae0c03bd2060186d0a03c9649001609d206060a903203baaa9018d0a03a520290f856120009e2032aa2043a7a525d056a524d05aa520290fc561f0ed8561c900f0e7c904f02fc908f01ac901f00ace0a03d0d0ee0a03d0cbee0a03c964d0c4ce0a03d0bfad0a0318690a8d0a03c96490b2a96310f5ad0a0338e90af0021002a9018d0a03109d38a90085248525601890f6"));
			Put(0x39E99, Blob.FromHex("a90e205baaa5620a0a0a186916aabd00038d0c0320b9ecae0a03a9008d0b038d0e038d0f0318ad0b0365108d0b03ad0e0365118d0e03ad0f0369008d0f03b005caf00dd0e1a9ff8d0b038d0e038d0f03ad0f038512ad0e038511ad0b03851020429f2032aa60"));
			Put(0x39EFF, Blob.FromHex("ad1e60cd0f03f0049016b016ad1d60cd0e03f004900ab00aad1c60cd0b03b00238601860ad1c6038ed0b038d1c60ad1d60ed0e038d1d60ad1e60ed0f038d1e604cefa74c8e8e"));
			Put(0x3A494, Blob.FromHex("201b9eb0e820999e20c2a8b0e0a562d0dc20ff9e9008a910205baa4c81a420089e9008a90c205baa4c81a420239fa913205baa4c81a4eaeaea"));

			if (archipelagoenabled)
			{
				//Replace NewCheckforSpace with patch in 0E_9F48_ItemShopCheckForSpace.asm
				PutInBank(0x0E, 0xA4B2, Blob.FromHex("20489F"));
				PutInBank(0x0E, 0x9F48, Blob.FromHex("AE0C03E016900CBD2060186D0A03C964900D60A2FFBD006209029D006218609D20601860"));
			}
		}

		public void ChangeUnrunnableRunToWait()
		{
			// See Unrunnable.asm
			// Replace DrawCommandMenu with a cross page jump to a replacement that swaps RUN for WAIT if the battle is unrunnable.
			// The last 5 bytes here are the null terminated WAIT string (stashed in some leftover space of the original subroutine)
			Put(0x7F700, Blob.FromHex("ADFC6048A90F2003FE204087682003FE4C48F6A08A929D00"));

			// Replace some useless code with a special handler for unrunnables that prints 'Nothing Happens'
			// We then update the unrunnable branch to point here instead of the generic Can't Run handler
			// See Disch's comments here: Battle_PlayerTryRun  [$A3D8 :: 0x323E8]
			Put(0x32409, Blob.FromHex("189005A94E4C07AAEAEAEAEAEAEAEA"));
			// new delta to special unrunnable message handler done in
		}
		public void EnableInventoryAutosort(bool noOverworld)
		{
			string finalChunk = "60";

			if (noOverworld)
			{
				finalChunk = "AD1260F00DA911990003E663C8A90099000360";
			}

			//see 0F_8670_SortInventory.asm
			Put(0x7EF58, Blob.FromHex("A90F2003FE20008DEAEAEA"));
			PutInBank(0x0F, 0x8D00, Blob.FromHex("8663A9009D0003A900856218A000A219BD2060F0058A990003C8E8E01CD0F1A216BD2060F0058A990003C8E8E019D0F1A21CBD2060F0058A990003C8E8E020D0F1A200BD2060F0058A990003C8E8E011D0F1" + finalChunk));
		}

		public void EnableCritNumberDisplay()
		{
			// Overwrite the normal critical hit handler by calling ours instead
			PutInBank(0x0C, 0xA94B, Blob.FromHex("20D1CFEAEA"));
			PutInBank(0x1F, 0xCFD1, CreateLongJumpTableEntry(0x0F, 0x92A0));
		}

		public void ActivateCropScreen()
		{
			//PutInBank(0x0E, 0xA222, Blob.FromHex("20D0A0"));
			//PutInBank(0x0E, 0xA0D0, Blob.FromHex("2006E9A9038DD00360"));

			PutInBank(0x1F, 0xE8FD, Blob.FromHex("4CE0DD"));
			PutInBank(0x1F, 0xDDE0, Blob.FromHex("20B9EAA9038DD00360"));
		}



	}
}
