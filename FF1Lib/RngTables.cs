using RomUtilities;
using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
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

		private List<byte> BattleRNG;
		private List<byte> EncounterRNG;
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
		}
		public void Write(FF1Rom rom)
		{
			rom.PutInBank(NewRngBank, NewRngOffset, EncounterRNG.ToArray());
			rom.PutInBank(NewRngBank, NewBattleRngOffset, BattleRNG.ToArray());
		}
	}
}
