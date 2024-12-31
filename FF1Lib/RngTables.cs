using RomUtilities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO.Compression;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace FF1Lib
{

	
	public class RngTables
	{
		private const int RngOffset = 0xF100;
		private const int BattleRngOffset = 0xFCF1;
		private const int RngBank = 0x0F;
		private const int NewRngOffset = 0xF100;
		private const int NewBattleRngOffset = 0xFCF1;
		private const int NewRngBank = 0x1F;
		private const int RngSize = 256;
		private const int BattleStepRNGOffset = 0xC571; // Bank 1F
		private ushort BattleStepSeed;
		private const int BattleStepSeedOffset = 0xDB09; // two unused bytes in Bank 1F
		private const int LoadPRNGSeedOnPartyGenOffset = 0x86E0; // Bank 1E

		private List<byte> BattleRNG;
		private List<byte> EncounterRNG;
		private List<string> PRNGAlgorithms = new List<string> {
			"ADF16F4AADF06F6A4DF16F85116A4DF06F8DF06F45118DF16F60EA",
			"ADF06F0AADF16F2A4DF06F85112A4DF16F8DF16F45118DF06F60EA",
			"ADF06F4DF16F85114AADF06F6A45118DF16F6A4DF06F8DF06F60EA",
			"ADF16F4DF06F85110AADF16F2A45118DF06F2A4DF16F8DF16F60EA"
		};
		private string PRNGAlgorithm;
		public RngTables(FF1Rom rom)
		{
			BattleRNG = rom.GetFromBank(RngBank, BattleRngOffset, RngSize, false).ToBytes().ToList();
			EncounterRNG = rom.GetFromBank(RngBank, RngOffset, RngSize, false).ToBytes().ToList();
		}


		public void Update(Flags flags, MT19337 rng)
		{
			if ((bool)flags.FixMissingBattleRngEntry)
			{
				// of the 256 entries in the battle RNG table, the 98th entry (index 97) is a duplicate '00' where '95' hex / 149 int is absent.
				// you could arbitrarily choose the other '00', the 111th entry (index 110), to replace instead
				BattleRNG[97] = 0x95;
			}

			if ((bool)flags.Rng)
			{
				BattleRNG.Shuffle(rng);
				EncounterRNG.Shuffle(rng);
			}

			if ((bool)flags.EncounterPrng)
			{
				BattleStepSeed = (ushort)rng.Between(0x0001, 0xFFFF);
				PRNGAlgorithm = PRNGAlgorithms.PickRandom(rng);
			}
		}
		public void Write(FF1Rom rom, Flags flags)
		{
			rom.PutInBank(NewRngBank, NewRngOffset, EncounterRNG.ToArray());
			rom.PutInBank(NewRngBank, NewBattleRngOffset, BattleRNG.ToArray());

			if ((bool)flags.EncounterPrng)
			{
				// just after partygen is confirmed, this executes the subroutine to
				// write the battlestep seed into sram
				rom.PutInBank(0x1E, 0x806B, Blob.FromHex("EAEA20E086"));
				rom.PutInBank(0x1E, LoadPRNGSeedOnPartyGenOffset, Blob.FromHex("A9008D0120AD09DB8DF06FAD0ADB8DF16F60"));

				// write the prng over the subroutine that followed the encounter table
				rom.PutInBank(0x1F, BattleStepRNGOffset, Blob.FromHex(PRNGAlgorithm));
				rom.PutInBank(0x1F, BattleStepSeedOffset, BattleStepSeed);
			}
		}
	}
}
