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
	    public const int ItemTextPointerCount = 252;
	    public const int ItemTextPointerBase = 0x20000;
	    public const int ItemTextOffset = 0x2B900;

	    public const int EnemyTextPointerOffset = 0x2D4E0;
	    public const int EnemyTextPointerBase = 0x24000;
	    public const int EnemyTextOffset = 0x2D5E0;

	    public const int DialogueTextPointerOffset = 0x28000;
	    public const int DialogueTextPointerCount = 256;
	    public const int DialogueTextPointerBase = 0x20000;
	    public const int DialogueTextOffset = 0x28200;

		public Blob[] ReadText(int pointerOffset, int pointerBase, int count)
	    {
		    var pointers = Get(pointerOffset, 2 * count).ToUShorts().ToList();

		    var textBlobs = new Blob[count];
		    for (int i = 0; i < pointers.Count; i++)
		    {
			    textBlobs[i] = ReadUntil(pointerBase + pointers[i], 0x00);
		    }

		    return textBlobs;
	    }

	    public void WriteText(Blob[] textBlobs, int pointerOffset, int pointerBase, int textOffset)
	    {
		    WriteText(textBlobs, pointerOffset, pointerBase, textOffset, new List<int>());
	    }

	    public void WriteText(Blob[] textBlobs, int pointerOffset, int pointerBase, int textOffset, List<int> skipThese)
	    {
			int offset = textOffset;
			var pointers = new ushort[textBlobs.Length];
			for (int i = 0; i < textBlobs.Length; i++)
			{
				if (skipThese.Contains(i))
				{
					// Don't write a blob, and point to the null-terminator at the end of the previous string.
					pointers[i] = (ushort)(offset - pointerBase - 1);
				}
				else
				{
					Put(offset, textBlobs[i]);

					pointers[i] = (ushort)(offset - pointerBase);
					offset += textBlobs[i].Length;
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
	}
}
