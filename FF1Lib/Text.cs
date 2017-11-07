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
		    int offset = textOffset;
		    var pointers = new ushort[textBlobs.Length];
		    for (int i = 0; i < textBlobs.Length; i++)
		    {
			    var blob = Blob.Concat(textBlobs[i], new byte[] { 0x00 });
			    Put(offset, blob);

			    pointers[i] = (ushort)(offset - pointerBase);
			    offset += blob.Length;
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

		    return bytes.ToArray();
	    }
    }
}
