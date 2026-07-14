namespace FF1Lib
{
	public partial class FF1Rom : NesRom
	{
		public void EnableLockpicking()
		{
			//put in the base hack: see 1B_9300_LockpickDoors.asm for more info
			PutInBank(0x1F, 0xCE53, Blob.FromHex("AA9848A91B2003FE200093C00168A88AB0ED"));
			PutInBank(0x1B,0x9300,Blob.FromHex("8A4A29038D036EC902D02DA2008645AE2560D01CA2008E046E205093A240205093A280205093A2C02050934901A8D00AADFA6209028DFA62A000AE036EA90E4C03FE00000000000000000000000E0107BD2661CD4D93900DBD0061CD4E93F009CD4F93F004A900F002A901A80D046E8D046E60"));
		}

		public void SetLockpickingLevel(int requiredLevel)
		{
			//overlay the level requirement
			if (requiredLevel > 0 && requiredLevel <= 50)
			{
				// //level is stored zero based
				// PutInBank(0x1B, 0x9315, new byte[] { (byte)(requiredLevel - 1) });
				// PutInBank(0x1B, 0x9327, new byte[] { (byte)(requiredLevel - 1) });
				// PutInBank(0x1B, 0x9339, new byte[] { (byte)(requiredLevel - 1) });
				// PutInBank(0x1B, 0x934B, new byte[] { (byte)(requiredLevel - 1) });
				PutInBank(0x1B, 0x934D, (byte)(requiredLevel - 1));
			}
		}

		public void SetLockpickingClass(int requiredClass)
		{
			if (requiredClass >= 0 && requiredClass < 6)
			{
				// PutInBank(0x1B, 0x9315 + 7, new byte[] { (byte)(requiredClass) });
				// PutInBank(0x1B, 0x9327 + 7, new byte[] { (byte)(requiredClass) });
				// PutInBank(0x1B, 0x9339 + 7, new byte[] { (byte)(requiredClass) });
				// PutInBank(0x1B, 0x934B + 7, new byte[] { (byte)(requiredClass) });

				// PutInBank(0x1B, 0x9315 + 11, new byte[] { (byte)(requiredClass + 6) });
				// PutInBank(0x1B, 0x9327 + 11, new byte[] { (byte)(requiredClass + 6) });
				// PutInBank(0x1B, 0x9339 + 11, new byte[] { (byte)(requiredClass + 6) });
				// PutInBank(0x1B, 0x934B + 11, new byte[] { (byte)(requiredClass + 6) });
				PutInBank(0x1B, 0x934E, (byte)requiredClass);
				PutInBank(0x1B, 0x934F, (byte)(requiredClass + 6));
			}
		}
	}
}
