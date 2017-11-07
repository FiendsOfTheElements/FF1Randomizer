using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RomUtilities;

namespace FF1Lib
{
    public partial class FF1Rom
    {
	    public const int TyroPaletteOffset = 0x30FC5;
	    public const int TyroSpriteOffset = 0x20560;

	    public void FunEnemyNames(bool teamSteak)
	    {
		    var enemyText = ReadText(EnemyTextPointerOffset, EnemyTextPointerBase, EnemyCount);

		    enemyText[1] = FF1Text.TextToBytes("GrUMP", useDTE: false);
		    enemyText[2] = FF1Text.TextToBytes("RURURU", useDTE: false); // +2
		    enemyText[3] = FF1Text.TextToBytes("GrrrWOLF", useDTE: false); // +2
		    enemyText[28] = FF1Text.TextToBytes("GeORGE", useDTE: false);
		    enemyText[30] = FF1Text.TextToBytes("R.SNEK", useDTE: false); // +3
		    enemyText[31] = FF1Text.TextToBytes("GrSNEK", useDTE: false); // +1
		    enemyText[32] = FF1Text.TextToBytes("SeaSNEK", useDTE: false); // -1
		    enemyText[40] = FF1Text.TextToBytes("iMAGE", useDTE: false);
		    enemyText[56] = FF1Text.TextToBytes("EXPEDE", useDTE: false); // +2
		    enemyText[66] = FF1Text.TextToBytes("White D", useDTE: false);
		    enemyText[72] = FF1Text.TextToBytes("MtlSLIME", useDTE: false); // +3
		    if (teamSteak)
		    {
			    enemyText[85] = FF1Text.TextToBytes("STEAK", useDTE: false); // +1
		    }
			enemyText[92] = FF1Text.TextToBytes("NACHO", useDTE: false); // -1
		    enemyText[106] = FF1Text.TextToBytes("Green D", useDTE: false); // +2
		    enemyText[111] = FF1Text.TextToBytes("OKAYMAN", useDTE: false); // +1

		    var enemyTextPart1 = enemyText.Take(2).ToArray();
		    var enemyTextPart2 = enemyText.Skip(2).ToArray();
		    WriteText(enemyTextPart1, EnemyTextPointerOffset, EnemyTextPointerBase, 0x2CFEC);
		    WriteText(enemyTextPart2, EnemyTextPointerOffset + 4, EnemyTextPointerBase, EnemyTextOffset);
	    }

	    public void TeamSteak()
	    {
		    Put(TyroPaletteOffset, Blob.FromHex("302505"));
		    Put(TyroSpriteOffset, Blob.FromHex(
				"00000000000000000000000000000000" + "00000000000103060000000000000001" + "001f3f60cf9f3f7f0000001f3f7fffff" + "0080c07f7f87c7e60000008080f8f8f9" + "00000080c0e0f0780000000000000080" + "00000000000000000000000000000000" +
				"00000000000000000000000000000000" + "0c1933676f6f6f6f03070f1f1f1f1f1f" + "ffffffffffffffffffffffffffffffff" + "e6e6f6fbfdfffffff9f9f9fcfefefefe" + "3c9e4e26b6b6b6b6c0e0f0f878787878" + "00000000000000000000000000000000" +
				"00000000000000000000000000000000" + "6f6f6f6f673b190f1f1f1f1f1f070701" + "fffffec080f9fbffffffffffff8787ff" + "ff3f1f1f3ffdf9f3fefefefefefefefc" + "b6b6b6b6b6b6b6b67878787878787878" + "00000000000000000000000000000000" +
				"00000000000000000000000000000000" + "07070706060707070100000101010101" + "ffffff793080c0f0fffc3086cfffffff" + "e7fefcf9f26469e3f80103070f9f9e1c" + "264c983060c08000f8f0e0c080000000" + "00000000000000000000000000000000" +
				"00000000000000000000000000000000" + "07070706060301010101010101000000" + "f9f9f9797366ece8fefefefefcf97377" + "c68c98981830606038706060e0c08080" + "00000000000000000000000000000000" + "00000000000000000000000000000000" +
				"00000000000000000000000000000000" + "01010101010000000000000000000000" + "fb9b9b9b98ff7f006767676767000000" + "6060606060c080008080808080000000" + "00000000000000000000000000000000" + "00000000000000000000000000000000"));
	    }
	}
}
