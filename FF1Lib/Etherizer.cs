namespace FF1Lib
{
	public partial class FF1Rom
	{
		public void Etherizer(bool enable, ItemNames itemsText)
		{
			if (!enable)
			{
				return;
			}

			// The MP restoration subroutine, overwriting the TENT/CABIN/HOUSE item use code
			// See etherizer.asm
			Put(0x3B284, Blob.FromHex("A9024C90B2A9044C90B2A90885642000B4A91B202BB920A0B3B020A5626A6A6A29C0AABD0161C901F014C902F010A46420C8B2A5644A4AAADE36604C1DB12026DB4C9AB2C888F00FBD2063DD2863B003FE2063E84CC9B24C79B5"));

			// The pointers to the item use code for CABIN and HOUSE (TENT remains the same)
			Put(0x3B1A5, Blob.FromHex("89B28EB2"));

			// The item targeting prompt "Who will recover MP?"
			// It's not using ReadText and WriteText because this area of text contains a lot
			// of control codes that mess up when those functions are used.
			Put(0x387CC, Blob.FromHex("A0AB2E5A40B61B2E23A6B2B925FF9699C500"));

			// Update the item names
			itemsText[(int)Item.Tent] = "ETHR@p";
			itemsText[(int)Item.Cabin] = "DRY@p ";
			itemsText[(int)Item.House] = "XETH@p";
		}
	}
}
