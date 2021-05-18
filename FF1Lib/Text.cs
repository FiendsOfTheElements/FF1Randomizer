using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RomUtilities;

namespace FF1Lib
{
    public partial class FF1Rom
    {
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

		public void UpdateItemName(Item targetitem, string newname)
		{
			var itemnames = ReadText(FF1Rom.ItemTextPointerOffset, FF1Rom.ItemTextPointerBase, FF1Rom.ItemTextPointerCount);

			itemnames[(int)targetitem] = newname;

			WriteText(itemnames, FF1Rom.ItemTextPointerOffset, FF1Rom.ItemTextPointerBase, FF1Rom.ItemTextOffset, FF1Rom.UnusedGoldItems);
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
	}
}
