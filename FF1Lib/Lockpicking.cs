namespace FF1Lib
{
	public partial class FF1Rom : NesRom
	{
		public void EnableLockpicking()
		{
			//put in the base hack: see 1B_9300_LockpickDoors.asm for more info
			PutInBank(0x1F, 0xCE53, Blob.FromHex("AA9848A91B2003FE200093C00168A88AB0ED"));
			PutInBank(0x1B, 0x9300, Blob.FromHex("8A4A2903C902D038A2008645AE2560D02F48A90048A000BE2661E009B00B18686940B013A8484C1793BE0061E001F010E007F00C4C1E93A00168AAA90E4C03FEA0006868AAA90E4C03FE"));
		}

		public void SetLockpickingLevel(int requiredLevel)
		{
			//overlay the level requirement
			if (requiredLevel > 0 && requiredLevel <= 50)
			{
				//level is stored zero based
				PutInBank(0x1B, 0x931B, new byte[] { (byte)(requiredLevel - 1) });
			}
		}
	}
}
