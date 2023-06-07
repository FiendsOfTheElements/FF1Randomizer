namespace FF1Lib
{
	public partial class FF1Rom : NesRom
	{
		public void EnableLockpicking()
		{
			//put in the base hack: see 1B_9300_LockpickDoors.asm for more info
			PutInBank(0x1F, 0xCE53, Blob.FromHex("AA9848A91B2003FE200093C00168A88AB0ED"));
			PutInBank(0x1B, 0x9300, Blob.FromHex("8A4A2903C902D03AA2008645AE2560D03148A90048A96885EDA9B285EEA000204C93D00ABE2661E009B0174C2E9318686940B005A8484C1F93A00168AAA90E4C03FEA0006868AAA90E4C03FEB90061C90C9002A90C8410A8B1EDC90108A4102860"));
		}

		public void SetLockpickingLevel(int requiredLevel)
		{
			//overlay the level requirement
			if (requiredLevel > 0 && requiredLevel <= 50)
			{
				//level is stored zero based
				PutInBank(0x1B, 0x9328, new byte[] { (byte)(requiredLevel - 1) });
			}
		}
	}
}
