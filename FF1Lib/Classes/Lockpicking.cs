namespace FF1Lib
{
	public partial class FF1Rom : NesRom
	{
		public void EnableLockpicking()
		{
			//put in the base hack: see 1B_9300_LockpickDoors.asm for more info
			PutInBank(0x1F, 0xCE53, Blob.FromHex("AA9848A91B2003FE200093C00168A88AB0ED"));
			PutInBank(0x1B,0x9300,Blob.FromHex("8A4A29038D036EC902D026A2008645AE2560D01DA2008E046E204093A240204093A280204093A2C02040934901A84C3393A000AE036EA90E4C03FE00000E0107BD2661CD3D93900DBD0061CD3E93F009CD3F93F004A900F002A901A80D046E8D046E60"));
			// PutInBank(0x1B, 0x9300, Blob.FromHex("8A4A2903C902D059A2008645AE2560D050AE2661E009900BAE0061E001F042E007F03EAE6661E009900BAE4061E001F030E007F02CAEA661E009900BAE8061E001F01EE007F01AAEE661E009900BAEC061E001F00CE007F008A001AAA90E4C03FEA000AAA90E4C03FE"));
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
				PutInBank(0x1B, 0x933D, (byte)(requiredLevel - 1));
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
				PutInBank(0x1B, 0x933E, (byte)requiredClass);
				PutInBank(0x1B, 0x933F, (byte)(requiredClass + 6));
			}
		}
	}
}
