namespace FF1Lib
{
	public partial class FF1Rom
    {
		//public IEnumerable<string> AllMenuStrings;
	    public const int ItemTextPointerOffset = 0x2B700;
	    public const int ItemTextPointerCount = 256;
	    public const int ItemTextPointerBase = 0x20000;
	    public const int ItemTextOffset = 0x2B900;
	    public const int MagicNamesIndexInItemText = 176;

	    public const int EnemyTextPointerOffset = 0x2D4E0;
	    public const int EnemyTextPointerBase = 0x24000;
	    public const int EnemyTextOffset = 0x2D5E0;

	    public const int DialogueTextPointerOffset = 0x28000;
	    public const int DialogueTextPointerCount = 256;
	    public const int DialogueTextPointerBase = 0x20000;
	    public const int DialogueTextOffset = 0x28200;

		public string[] ReadText(int pointerOffset, int pointerBase, int count)
	    {
		    var pointers = Get(pointerOffset, 2 * count).ToUShorts().ToList();

		    var texts = new string[count];
		    for (int i = 0; i < pointers.Count; i++)
		    {
			    texts[i] = FF1Text.BytesToText(ReadUntil(pointerBase + pointers[i], 0x00));
		    }

		    return texts;
	    }

		public string[] ReadTextFromBank(int bank, int pointerOffset, int count)
		{
			var pointers = GetFromBank(bank,pointerOffset,2*count).ToUShorts().ToList();

			var texts = new string[count];
			for (int i = 0; i < pointers.Count; i++)
			{
				texts[i] = FF1Text.BytesToText(ReadFromBankUntil(bank,pointers[i],0x00));
			}

			return texts;
		}

	    public void WriteText(string[] texts, int pointerOffset, int pointerBase, int textOffset)
	    {
		    WriteText(texts, pointerOffset, pointerBase, textOffset, new List<int>());
	    }

	    public void WriteText(string[] texts, int pointerOffset, int pointerBase, int textOffset, List<int> skipThese)
	    {
			int offset = textOffset;
			var pointers = new ushort[texts.Length];
			for (int i = 0; i < texts.Length; i++)
			{
				if (skipThese.Contains(i))
				{
					// Don't write a blob, and point to the null-terminator at the end of the previous string.
					pointers[i] = (ushort)(offset - pointerBase - 1);
				}
				else
				{
					var blob = FF1Text.TextToBytes(texts[i], useDTE: false);
					Put(offset, blob);

					pointers[i] = (ushort)(offset - pointerBase);
					offset += blob.Length;
				}
			}

			Put(pointerOffset, Blob.FromUShorts(pointers));
	    }
	    public Blob ReadUntil(int offset, byte delimiter)
	    {
			var bytes = new List<byte>();
			while (Data[offset] != delimiter && offset < Data.Length)
			{
				bytes.Add(Data[offset++]);
			}
			bytes.Add(delimiter);

			return bytes.ToArray();
	    }

		public Blob ReadFromBankUntil(int bank, int offset, byte delimiter, bool extended = true)
		{
			int lastbank = extended ? 0x1F : 0x0F;
			int bankAddress = bank*0x4000;
			offset += bankAddress - (bank == lastbank? 0xC000 : 0x8000);
			List<byte> bytes = [];
			while (Data[offset] != delimiter && offset < bankAddress+0x4000)
			{
				bytes.Add(Data[offset++]);
			}
			bytes.Add(delimiter);
			return bytes.ToArray();
		}
	}
}
