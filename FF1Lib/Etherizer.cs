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

			// All new ether items replace the Tent use string
			MenuText.MenuStrings[(int)FF1Text.MenuString.UseTent] = FF1Text.TextToBytes("Who needs to recover MP?", useDTE: true, delimiter: FF1Text.Delimiter.Null);

			// Update the item names
			itemsText[(int)Item.Tent] = "ETHR@p";
			itemsText[(int)Item.Cabin] = "DRY@p ";
			itemsText[(int)Item.House] = "XETH@p";
		}
	}
}
