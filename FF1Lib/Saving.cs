using RomUtilities;
using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FF1Lib
{
	public partial class FF1Rom
	{
		public void SavingHacks(Overworld overworld, Flags flags)
		{

			EnableSaveOnDeath(flags, overworld);

			if (flags.DisableTentSaving)
			{
				CannotSaveOnOverworld();
			}

			if (flags.DisableInnSaving)
			{
				CannotSaveAtInns();
			}

		}

		public void EnableSaveOnDeath(Flags flags, Overworld overworld)
		{
			if (!flags.SaveGameWhenGameOver)
			{
				return;
			}

			// rewrite rando's GameOver routine to jump to a new section that will save the game data
			PutInBank(0x1B, 0x801A, Blob.FromHex("4CF58F"));

			byte coneria_x = 0x92;
			byte coneria_y = 0x9E;
			byte airship_x = 0x99;
			byte airship_y = 0xA5;
			byte ship_x = 0x98;
			byte ship_y = 0xA9;

			if (flags.GameMode == GameModes.DeepDungeon)
			{
				coneria_y = 0x9B;
			}

			if (overworld.MapExchange != null && flags.GameMode == GameModes.Standard)
			{
				coneria_x = (byte)(overworld.Locations.StartingLocation.X - 0x07);
				coneria_y = (byte)(overworld.Locations.StartingLocation.Y - 0x07);

				airship_x = overworld.Locations.StartingLocation.X;
				airship_y = overworld.Locations.StartingLocation.Y;

				ship_x = overworld.GetShipLocation((int)OverworldTeleportIndex.Coneria).X;
				ship_y = overworld.GetShipLocation((int)OverworldTeleportIndex.Coneria).Y;
			}

			// write new routine to save data at game over (the game will save when you clear the final textbox and not before), see 1B_8FF5_GameOverAndRestart.asm
			var saveondeath_standardmid = $"AD0460D02EAD0060F04FAD0160CD0164D008AD0260CD0264F03FAD016038E9078D1060AD026038E9078D1160A9048D1460D026AD056038E9078D1060AD066038E9078D1160A9018D1460AD0060F00AA9{ship_x:X2}8D0160A9{ship_y:X2}8D0260";
			var saveondeath_dwmodemid = $"AD0460F00AA9{airship_x:X2}8D0560A9{airship_y:X2}8D0660AD0060F00AA9{ship_x:X2}8D0160A9{ship_y:X2}8D0260A9{coneria_x:X2}8D1060A9{coneria_y:X2}8D11604E1E606E1D606E1C60A9018D1460EAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEA";
			var saveondeath_part1 = "20E38BA200BD0061C9FFF041BD0C619D0A61BD0D619D0B61BD28639D2063BD29639D2163BD2A639D2263BD2B639D2363BD2C639D2463BD2D639D2563BD2E639D2663BD2F639D2763A9009D01618A186940AAD0B1";
			var saveondeath_part2 = "A200BD00609D0064BD00619D0065BD00629D0066BD00639D0067E8D0E5A9558DFE64A9AA8DFF64A9008DFD64A200187D00647D00657D00667D0067E8D0F149FF8DFD644C1D80";

			// Since we want to spawn inside with No Overworld and not at transport, update coordinate to Coneria Castle
			if (flags.NoOverworld)
			{
				coneria_x = GetFromBank(0x0E, 0x9DC0 + 0x08, 1)[0];
				coneria_y = GetFromBank(0x0E, 0x9DD0 + 0x08, 1)[0];

				saveondeath_standardmid = "EAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEA";
				saveondeath_dwmodemid = $"AD0460F00AA9998D0560A9A58D0660AD0060F00AA9988D0160A9A98D0260A9{coneria_x:X2}8D1060A9{coneria_y:X2}8D11604E1E606E1D606E1C60EAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEA";
			}
			var saveondeath = saveondeath_part1 + (flags.SaveGameDWMode ? saveondeath_dwmodemid : saveondeath_standardmid) + saveondeath_part2;

			PutInBank(0x1B, 0x8FF5, Blob.FromHex(saveondeath));
		}

		public void CannotSaveOnOverworld()
		{
			// Hacks the game to disallow saving on the overworld with Tents, Cabins, or Houses
			Put(0x3B2F9, Blob.FromHex("1860"));
			// Change Item using text to avoid confusion
			PutInBank(0x0E, 0x87B0, FF1Text.TextToBytes("\n\nSAVING DISABLED!"));
			PutInBank(0x0E, 0x87E7, FF1Text.TextToBytes("\n\nSAVING DISABLED!"));
			PutInBank(0x0E, 0x8825, FF1Text.TextToBytes("\n\nSAVING DISABLED!"));
		}

		public void CannotSaveAtInns()
		{
			// Hacks the game so that Inns do not save the game
			Put(0x3A53D, Blob.FromHex("EAEAEA"));
			// Change Inn text to avoid confusion
			PutInBank(0x0E, 0x81BB, FF1Text.TextToBytes("Welcome\n  ..\nStay to\nheal\nyour\nwounds?"));
			PutInBank(0x0E, 0x81DC, FF1Text.TextToBytes("Don't\nforget\n.."));
			PutInBank(0x0E, 0x81FC, FF1Text.TextToBytes("Your\ngame\nhasn't\nbeen\nsaved."));
		}
	}
}
