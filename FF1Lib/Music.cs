using RomUtilities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace FF1Lib
{
	public enum SongTracks
	{
		Crystal = 0x41,
		Intro = 0x42,
		Ending = 0x43,
		Overworld = 0x44,
		Ship = 0x45,
		Airship = 0x46,
		Town = 0x47,
		Castle = 0x48,
		EarthCave = 0x49,
		DwarfCave = 0x4A,
		MarshCave = 0x4B,
		TempleOfFiend = 0x4C,
		SkyPalace = 0x4D,
		TempleOfFiendRevisited = 0x4E,
		Shop = 0x4F,
		Battle = 0x50,
		Menu = 0x51,
		GameOver = 0x52,
		Victory = 0x53,
		ItemJingle = 0x54,
		Crystal2 = 0x55,
		Rest = 0x56,
		Sound1 = 0x57,
		Sound2 = 0x58,
		Minimap = 0x99,
	}
	public class MusicTracks
	{
		public Dictionary<SongTracks, SongTracks> Tracks { get; private set; }
		public bool SkipMinimap = false;
		private MusicShuffle mode;
		public bool MusicShuffled { get; private set; }

		public MusicTracks()
		{
			Tracks = Enum.GetValues<SongTracks>().ToDictionary(t => t, t => t);
			Tracks[SongTracks.Minimap] = SongTracks.Crystal;
			MusicShuffled = false;
		}

		public void ShuffleMusic(FF1Rom rom, MusicShuffle _mode, bool alternateAirshipTheme, MT19337 rng)
		{
			if (alternateAirshipTheme) {
				rom.PutInBank(0x0D, 0xB600, Blob.FromHex("FDF804E2D897D9477797DA07D977974777274777D104B6D897D9477797DA07D977974777274777D517B6D90777A7DA07572747D9A7DA07D95777A7D52AB6D017B6FDF803E2D897979797D546B6979797979797979797979797D54DB6070707070707070707070707D55CB6D04DB6FDF809E7C0C0D994DA27477794DB07DAB77794DB07DAB7779475B744240527D9B577D174B6DA045777A7DB045747DAA7DB045747DAA7DB04DAA5DB27DA7454457725D9A7DA045777A7DB045747DAA7DB045747DAA7DB04A7975770D074B6"));
				rom.PutInBank(0x0D, 0x8028, Blob.FromHex("00B641B66EB6"));
			}

			mode = _mode;
			// Need some fixes, some of the music offsets are wrong and can cause crashes, maybe there's a better way of keeping tracks of harcoded songtracks
			// Ideally the game would always refer to a lut or something
			switch (mode)
			{
				case MusicShuffle.None:
					MusicShuffled = false;
					return;
				case MusicShuffle.Standard:
					MusicShuffled = true;
					List<byte> overworldTracks = new List<byte> { 0x41, 0x42, 0x44, 0x45, 0x46, 0x47, 0x4A, 0x4F };
					List<byte> townTracks = new List<byte> { 0x41, 0x42, 0x45, 0x46, 0x47, 0x48, 0x4A, 0x4F, 0x51 };
					List<byte> dungeonTracks = new List<byte> { 0x41, 0x42, 0x45, 0x46, 0x47, 0x48, 0x49, 0x4A, 0x4B, 0x4C, 0x4D, 0x4E, 0x52, 0x53 };

					overworldTracks.Shuffle(rng);
					townTracks.Shuffle(rng);
					dungeonTracks.Shuffle(rng);

					//Overworld
					Tracks[SongTracks.Overworld] = (SongTracks)overworldTracks[0];
					Tracks[SongTracks.Ship] = (SongTracks)overworldTracks[1];
					Tracks[SongTracks.Airship] = (SongTracks)overworldTracks[2];

					//Remove used songs from other pools
					var usedTracks = overworldTracks.Take(3).ToList();
					townTracks = townTracks.Except(usedTracks).ToList();

					//Town
					Tracks[SongTracks.Town] = (SongTracks)townTracks[0];
					Tracks[SongTracks.Castle] = (SongTracks)townTracks[1];
					Tracks[SongTracks.Shop] = (SongTracks)townTracks[2];
					Tracks[SongTracks.Menu] = (SongTracks)townTracks[3];

					//Remove used songs from other pools
					usedTracks.AddRange(townTracks.Take(4));
					dungeonTracks = dungeonTracks.Except(usedTracks).ToList();

					//Dungeons
					Tracks[SongTracks.EarthCave] = (SongTracks)dungeonTracks[0];
					Tracks[SongTracks.DwarfCave] = (SongTracks)dungeonTracks[1];
					Tracks[SongTracks.MarshCave] = (SongTracks)dungeonTracks[2];
					Tracks[SongTracks.TempleOfFiend] = (SongTracks)dungeonTracks[3];
					Tracks[SongTracks.SkyPalace] = (SongTracks)dungeonTracks[4];
					Tracks[SongTracks.TempleOfFiendRevisited] = (SongTracks)dungeonTracks[5];
					break;

				case MusicShuffle.Nonsensical: //They asked for it...
					MusicShuffled = true;
					List<byte> tracks = new List<byte> { 0x41, 0x42, 0x43, 0x44, 0x45, 0x46, 0x47, 0x48, 0x49, 0x4A, 0x4B, 0x4C, 0x4D, 0x4E, 0x4F, 0x51, 0x52, 0x53, 0x55 };
					tracks.Shuffle(rng);

					//Overworld
					Tracks[SongTracks.Overworld] = (SongTracks)tracks[0];
					Tracks[SongTracks.Ship] = (SongTracks)tracks[1];
					Tracks[SongTracks.Airship] = (SongTracks)tracks[2];
					Tracks[SongTracks.Town] = (SongTracks)tracks[3];
					Tracks[SongTracks.Castle] = (SongTracks)tracks[4];
					Tracks[SongTracks.EarthCave] = (SongTracks)tracks[5];
					Tracks[SongTracks.DwarfCave] = (SongTracks)tracks[6];
					Tracks[SongTracks.MarshCave] = (SongTracks)tracks[7];
					Tracks[SongTracks.TempleOfFiend] = (SongTracks)tracks[8];
					Tracks[SongTracks.SkyPalace] = (SongTracks)tracks[9];
					Tracks[SongTracks.TempleOfFiendRevisited] = (SongTracks)tracks[10];
					Tracks[SongTracks.Crystal] = (SongTracks)tracks[11];
					Tracks[SongTracks.Shop] = (SongTracks)tracks[12];
					Tracks[SongTracks.Menu] = (SongTracks)tracks[13];
					Tracks[SongTracks.Ending] = (SongTracks)tracks[14];
					Tracks[SongTracks.Intro] = (SongTracks)tracks[15];
					Tracks[SongTracks.Victory] = (SongTracks)tracks[16];
					Tracks[SongTracks.GameOver] = (SongTracks)tracks[17];
					Tracks[SongTracks.Minimap] = (SongTracks)tracks[rng.Between(0, tracks.Count - 1)];
					break;
				case MusicShuffle.MusicDisabled:
					MusicShuffled = true;
					Tracks = Tracks.ToDictionary(t => t.Key, t => (SongTracks)0x41);
					break;
			}
		}
		public void Write(FF1Rom rom)
		{
			//Overworld
			rom[0x7C649] = (byte)Tracks[SongTracks.Overworld]; 
			rom[0x7C6F9] = (byte)Tracks[SongTracks.Overworld]; 
			rom[0x7C75A] = (byte)Tracks[SongTracks.Overworld]; // Transport Track lut
			rom[0x7C75B] = (byte)Tracks[SongTracks.Overworld]; // Transport Track lut

			//Ship
			rom[0x7C62D] = (byte)Tracks[SongTracks.Ship];
			rom[0x7C75D] = (byte)Tracks[SongTracks.Ship]; // Transport Track lut

			//Airship
			rom[0x7C235] = (byte)Tracks[SongTracks.Airship];
			rom[0x7C761] = (byte)Tracks[SongTracks.Airship]; // Transport Track lut

			//Tilesets 1-8
			rom[0x7CFC3] = (byte)Tracks[SongTracks.Town];
			rom[0x7CFC4] = (byte)Tracks[SongTracks.Castle];
			rom[0x7CFC5] = (byte)Tracks[SongTracks.EarthCave];
			rom[0x7CFC6] = (byte)Tracks[SongTracks.DwarfCave];
			rom[0x7CFC7] = (byte)Tracks[SongTracks.MarshCave];
			rom[0x7CFC8] = (byte)Tracks[SongTracks.TempleOfFiend];
			rom[0x7CFC9] = (byte)Tracks[SongTracks.SkyPalace];
			rom[0x7CFCA] = (byte)Tracks[SongTracks.TempleOfFiendRevisited];

			//Title
			rom[0x3A226] = (byte)Tracks[SongTracks.Crystal];

			//Shop
			rom[0x3A351] = (byte)Tracks[SongTracks.Shop];
			rom[0x3A56E] = (byte)Tracks[SongTracks.Shop];
			rom[0x3A597] = (byte)Tracks[SongTracks.Shop];

			//Menu
			rom[0x3B677] = (byte)Tracks[SongTracks.Crystal]; // Lineup menu
			rom[0x3997F] = (byte)Tracks[SongTracks.Crystal]; // Lineup menu?

			rom[0x78546] = (byte)Tracks[SongTracks.Menu]; // Moved for stats tracking, not sure about this one?
			rom[0x7BB71] = (byte)Tracks[SongTracks.Menu]; // Previously 0x3ADB4, moved by stats tracking
			rom[0x687B1] = (byte)Tracks[SongTracks.Menu]; // Moved menu code for repeated heal potion use

			//Ending
			rom[0x7BBB1] = (byte)Tracks[SongTracks.Ending]; // Previously 0x37804, moved by stats tracking

			//Bridge Cutscene
			rom[0x7BBD1] = (byte)Tracks[SongTracks.Intro]; // Previously 0x3784E, moved by stats tracking

			//Battle Fanfare
			rom[0x31E44] = (byte)Tracks[SongTracks.Victory];

			//Gameover
			rom[0x3C5F0] = (byte)Tracks[SongTracks.GameOver]; // Moved by stats tracking

			//Battle
			rom[0x2D9C1] = (byte)Tracks[SongTracks.Battle];

			//Mini Things
			//Data[0x36E86] = tracks[rng.Between(0, tracks.Count - 1)]; //minigame
			if (!SkipMinimap)
			{
				rom[0x27C0D] = (byte)Tracks[SongTracks.Minimap];
			}

			// assign to address
			if (mode == MusicShuffle.MusicDisabled)
			{
				//Set Sq1, Sq2, and Tri channels for crystal theme all point to the same music data
				rom.Put(0x34000, Blob.FromHex("C080C080C080"));
				//Overwrite beginning of crystal theme with a song that initializes properly but plays no notes
				rom.Put(0x340C0, Blob.FromHex("FDF805E0D8C7D0C480"));
			}
			if (mode == MusicShuffle.ChaosBattleMusic)
			{
				//move music data and code to bank 1D
				rom.PutInBank(0x1D, 0x8000, rom.GetFromBank(0x0D, 0x8000, 0x1DA0));
				rom.PutInBank(0x1D, 0xB000, rom.GetFromBank(0x0D, 0xB000, 0x800));

				//point music code to new bank
				rom.PutInBank(0x1F, 0xC682, Blob.FromHex("1D"));
				rom.PutInBank(0x1F, 0xC68A, Blob.FromHex("1D"));

				//logic - see 1C_A790_AltChaosBattleMusic.asm
				rom.PutInBank(0x0B, 0x99B8, Blob.FromHex("A9A748A98F48A91C4C03FE"));
				rom.PutInBank(0x1C, 0xA790, Blob.FromHex("A9008DB7688DB868A56A297FC97BF005A9504CA7A7A955854B8DA76BA99948A9C748A90B4C03FE"));

				//song pointer replaces Crystal2
				rom.PutInBank(0x1D, 0x80A0, Blob.FromHex("70A0A09DB0A20000"));
				//tracks
				rom.PutInBank(0x1D, 0x9DA0, Blob.FromHex("FBE4F800DB08DA38283808382838DB08DA3828380838283888584858085848588858485808584858B858385828583858B858385828583858DB08DA78587838785878DB08DA78587838785878DB38DA88788838887888DB38DA88788838887888DB28DA68486828684868DB28DA68486828684868DB28DA78687828786878DB28DA78687828786878DB08DA48284808482848DB08DA48284808482848DB08DA58485808584858DB08DA58485808584858A858385828583858A858385828583858A878587838785878A8785878387858788878587838785878887858783878587888280828D9A8DA28082888280828D9A8DA28082878D9A888A8DA38D9A888A8DA78D9A888A8DA38D9A888A8DA5808D9A8DA08D998DA08D9A8DA085808D9A8DA08D998DA08D9A8DA0858280828D9B8DA28082858280828D9B8DA28082858280828D9B8DA28082858280828D9B8DA2808283808D9B8DA08D978DA08D9B8DA083808D9B8DA08D978DA08D9B8DA08D958DA38283858382838D958DA38283858382838D968DA08D9B8DA083808D9B8DA08D968DA08D9B8DA083808D9B8DA083808D9B8DA08D978DA08D9B8DA083808D9B8DA08D978DA08D9B8DA086808D9B8DA08D998DA08D9B8DA086808D9B8DA08D998DA08D9B8DA087808D9B8DA082808D9B8DA087808D9B8DA082808D9B8DA088808D9B8DA082808D9B8DA088808D9B8DA082808D9B8DA08D878B8D9285888584858B858DA28D9B888584858D878D9083878DA08D9786878DA3808783808D9887888D87898D968DA083808D9B8DA08680898683808D9B8DA08D8C8DB2808283808DAB8DB08DA98DB08DAB8DB0828DAB898B878B898B8DB08DA98789868987898B878687828DB7858788858385828583858783828380838283858280828DAB8DB2808283808DAB8DB08DA78DB08DAB8DB08DA88DB583858DA78DB382838DA58DB280828DA38DB08DAB8DB08DA885838587838283858280828D0A09D"));
				rom.PutInBank(0x1D, 0xA070, Blob.FromHex("FBE6F800D90878587838785878087858783878587808887888588878880888788858887888088878885888788808887888588878880838283878382838083828387838283808DA08D9A8DA08D988DA08D9A8DA08D908DA08D9A8DA08D988DA08D9A8DA08D908987898689878980898789868987898D8A8D9A898A878A898A8D8A8D9A898A878A898A8D8A8D978587848785878D8A8D978587848785878D888D988788858887888D888D988788858887888D888D928082858280828D888D928082858280828D878D938283878382838D878D9382838783828380838283888382838083828388838283828583858885838582858385888583858387858788878587838785878887858783898789858987898389878985898789828583858885838582858385888583858085848588858485808584858885848580838283858382838D8A8D938283858382838D888D908D8B8D9082808D8B8D908D888D908D8B8D9082808D8B8D908D898D938283808382838D898D938283808382838D878D938283858382838D878D938283858382838D878D938283808382838D878D938283808382838D878D938283858382838D878D938283858382838D878D938283858382838D878D938283858382838D8E0C0C0C0E67078D92808283808D8B8D908D898D908D8B8D90828D8B898B878B898B8D908D888788858987898B878587838D978587888583858285838587838283808382838582808283808D8B8D90828D8B898B8D908382838D858D9280828D838D908D8B8D908D828B898B8D070A0"));
				rom.PutInBank(0x1D, 0xA2B0, Blob.FromHex("FBE0F800D900D880B0D9005020700050D8A0D930D880D920305020D880D900D880D93002D872D9607084B4DA2454D8C0C0C070D978DA2808283808D9B8DA08D998DA08D9B8DA0828D9B898B878B898B8DA08D988788858987898B87858783878587888583858285838587838283808382838582808283808D8B8D90828D8B898B8D908382838D858D9280828D838D908D8B8D908D828B898B8D0B0A2"));
			}
		}

	}

}
