namespace FF1Lib
{
	public partial class FF1Rom
	{
		public void ImprovedClinic(bool enable)
		{
			if (!enable)
			{
				return;
			}

			// See improved-clinic.asm

			// List characters that have ANY statuses, not just Dead
			Put(0x3A6F6, Blob.FromHex("C900F01C"));

			// We only want to set the character's HP to 1 if they were Dead.
			// Conveniently, the code for "Dead" is 01, so we can just transfer it straight to their HP.
			// Otherwise, skip ahead and clear the status byte alongside the A and B button flags
			Put(0x3A5DF, Blob.FromHex("BD0161C901D0039D0A61A900852485259D0161EAEA"));
		}
	}
}
