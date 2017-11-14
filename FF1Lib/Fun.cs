using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RomUtilities;

namespace FF1Lib
{
	public enum MusicShuffle
	{
		None = 0,
		Standard,
		Nonsensical
	}

    public partial class FF1Rom
    {
	    public const int TyroPaletteOffset = 0x30FC5;
	    public const int TyroSpriteOffset = 0x20560;

	    public const int PaletteOffset = 0x30F20;
	    public const int PaletteSize = 4;
	    public const int PaletteCount = 64;

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
			    enemyText[86] = FF1Text.TextToBytes("T.BONE", useDTE: false); // +1
		    }
			enemyText[92] = FF1Text.TextToBytes("NACHO", useDTE: false); // -1
		    enemyText[106] = FF1Text.TextToBytes("Green D", useDTE: false); // +2
		    enemyText[111] = FF1Text.TextToBytes("OKAYMAN", useDTE: false); // +1

			// Moving IMP and GrIMP gives me another 10 bytes, for a total of 19 extra bytes, of which I'm using 16.
			var enemyTextPart1 = enemyText.Take(2).ToArray();
		    var enemyTextPart2 = enemyText.Skip(2).ToArray();
		    WriteText(enemyTextPart1, EnemyTextPointerOffset, EnemyTextPointerBase, 0x2CFEC);
		    WriteText(enemyTextPart2, EnemyTextPointerOffset + 4, EnemyTextPointerBase, EnemyTextOffset);
	    }

	    public void PaletteSwap(MT19337 rng)
	    {
		    var palettes = Get(PaletteOffset, PaletteSize * PaletteCount).Chunk(PaletteSize);

			palettes.Shuffle(rng);

			Put(PaletteOffset, Blob.Concat(palettes));
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

		public void ShuffleMusic(MusicShuffle mode, MT19337 rng)
		{
			switch (mode)
			{
				case MusicShuffle.Standard:
					List<byte> overworldTracks = new List<byte> { 0x41, 0x42, 0x44, 0x45, 0x46, 0x47, 0x4A, 0x4F };
					List<byte> townTracks = new List<byte> { 0x41, 0x42, 0x45, 0x46, 0x47, 0x48, 0x4A, 0x4F, 0x51 };
					List<byte> dungeonTracks = new List<byte> { 0x41, 0x42, 0x45, 0x46, 0x47, 0x48, 0x49, 0x4A, 0x4B, 0x4C, 0x4D, 0x4E, 0x52, 0x53 };

					overworldTracks.Shuffle(rng);
					townTracks.Shuffle(rng);
					dungeonTracks.Shuffle(rng);

					//Overworld
					Data[0x3C649] = overworldTracks[0];
					Data[0x3C6F9] = overworldTracks[0];
					Data[0x3C75A] = overworldTracks[0];
					Data[0x3C75B] = overworldTracks[0];

					//Ship
					Data[0x3C62D] = overworldTracks[1];
					Data[0x3C75D] = overworldTracks[1];

					//Airship
					Data[0x3C235] = overworldTracks[2];
					Data[0x3C761] = overworldTracks[2];

					//Remove used songs from other pools
					var usedTracks = overworldTracks.Take(3).ToList();
					townTracks = townTracks.Except(usedTracks).ToList();

					//Town
					Data[0x3CFC3] = townTracks[0];

					//Castle
					Data[0x3CFC4] = townTracks[1];

					//Shop
					Data[0x3A351] = townTracks[2];
					Data[0x3A56E] = townTracks[2];
					Data[0x3A597] = townTracks[2];

					//Menu
					Data[0x3ADB4] = townTracks[3];
					Data[0x3B677] = townTracks[3];
					Data[0x3997F] = townTracks[3]; //Lineup menu

					//Remove used songs from other pools
					usedTracks.AddRange(townTracks.Take(4));
					dungeonTracks = dungeonTracks.Except(usedTracks).ToList();

					//Dungeons
					Data[0x3CFC5] = dungeonTracks[0];
					Data[0x3CFC6] = dungeonTracks[1];
					Data[0x3CFC7] = dungeonTracks[2];
					Data[0x3CFC8] = dungeonTracks[3];
					Data[0x3CFC9] = dungeonTracks[4];
					Data[0x3CFCA] = dungeonTracks[5];

					break;

				case MusicShuffle.Nonsensical: //They asked for it...
					List<byte> tracks = new List<byte> { 0x41, 0x42, 0x43, 0x44, 0x45, 0x46, 0x47, 0x48, 0x49, 0x4A, 0x4B, 0x4C, 0x4D, 0x4E, 0x4F, 0x51, 0x52, 0x53, 0x55 };
					tracks.Shuffle(rng);

					//Overworld
					Data[0x3C649] = tracks[0];
					Data[0x3C6F9] = tracks[0];
					Data[0x3C75A] = tracks[0];
					Data[0x3C75B] = tracks[0];

					//Ship
					Data[0x3C62D] = tracks[1];
					Data[0x3C75D] = tracks[1];

					//Airship
					Data[0x3C235] = tracks[2];
					Data[0x3C761] = tracks[2];

					//Tilesets 1-8
					Data[0x3CFC3] = tracks[3]; //Town
					Data[0x3CFC4] = tracks[4]; //Castle
					Data[0x3CFC5] = tracks[5];
					Data[0x3CFC6] = tracks[6];
					Data[0x3CFC7] = tracks[7];
					Data[0x3CFC8] = tracks[8];
					Data[0x3CFC9] = tracks[9];
					Data[0x3CFCA] = tracks[10];

					//Title
					Data[0x3A226] = tracks[11];

					//Shop
					Data[0x3A351] = tracks[12];
					Data[0x3A56E] = tracks[12];
					Data[0x3A597] = tracks[12];

					//Menu
					Data[0x3ADB4] = tracks[13];
					Data[0x3B677] = tracks[13];
					Data[0x3997F] = tracks[13]; //Lineup menu

					//Ending
					Data[0x37804] = tracks[14];

					//Bridge Cutscene
					Data[0x3784E] = tracks[15];

					//Battle Fanfare
					Data[0x31E44] = tracks[16];

					//Gameover
					Data[0x2DAF6] = tracks[17];

					//Battle
					//Data[0x2D9C1] = Songs[rng.Between(0, Songs.Count - 1)];

					//Mini Things
					Data[0x36E86] = tracks[rng.Between(0, tracks.Count - 1)]; //minigame
					Data[0x27C0D] = tracks[rng.Between(0, tracks.Count - 1)]; //minimap

					break;
			}
		}
	}
}
