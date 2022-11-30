namespace FF1Lib
{
	public partial class FF1Rom : NesRom
	{
		public void EnableLockpicking()
		{
			//put in the base hack: see 1B_9300_LockpickDoors.asm for more info
			PutInBank(0x1F, 0xCE53, Blob.FromHex("AA9848A91B2003FE200093C00168A88AB0ED"));
			PutInBank(0x1B, 0x9300, Blob.FromHex("8A4A2903C902D059A2008645AE2560D050AE2661E009900BAE0061E001F042E007F03EAE6661E009900BAE4061E001F030E007F02CAEA661E009900BAE8061E001F01EE007F01AAEE661E009900BAEC061E001F00CE007F008A001AAA90E4C03FEA000AAA90E4C03FE"));
		}

		public void SetLockpickingLevel(int requiredLevel)
		{
			//overlay the level requirement
			if (requiredLevel > 0 && requiredLevel <= 50)
			{
				//level is stored zero based
				PutInBank(0x1B, 0x9315, new byte[] { (byte)(requiredLevel - 1) });
				PutInBank(0x1B, 0x9327, new byte[] { (byte)(requiredLevel - 1) });
				PutInBank(0x1B, 0x9339, new byte[] { (byte)(requiredLevel - 1) });
				PutInBank(0x1B, 0x934B, new byte[] { (byte)(requiredLevel - 1) });
			}
		}

		// Currently for use by Transmoog, but feel free if you want it elsewhere
		public void SetLockpickingClass(int requiredClass)
		{
			if (requiredClass >= 0 && requiredClass < 6)
			{
				PutInBank(0x1B, 0x9315 + 7, new byte[] { (byte)(requiredClass) });
				PutInBank(0x1B, 0x9327 + 7, new byte[] { (byte)(requiredClass) });
				PutInBank(0x1B, 0x9339 + 7, new byte[] { (byte)(requiredClass) });
				PutInBank(0x1B, 0x934B + 7, new byte[] { (byte)(requiredClass) });

				PutInBank(0x1B, 0x9315 + 11, new byte[] { (byte)(requiredClass+6) });
				PutInBank(0x1B, 0x9327 + 11, new byte[] { (byte)(requiredClass+6) });
				PutInBank(0x1B, 0x9339 + 11, new byte[] { (byte)(requiredClass+6) });
				PutInBank(0x1B, 0x934B + 11, new byte[] { (byte)(requiredClass+6) });
			}
		}
	}
}
