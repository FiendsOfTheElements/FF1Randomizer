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
		Crystal = 0x0,
		Intro = 0x1,
		Ending = 0x2,
		Overworld = 0x3,
		Ship = 0x4,
		Airship = 0x5,
		Town = 0x6,
		Castle = 0x7,
		EarthCave = 0x8,
		DwarfCave = 0x9,
		MarshCave = 0xA,
		TempleOfFiend = 0xB,
		SkyPalace = 0xC,
		TempleOfFiendRevisited = 0xD,
		Shop = 0xE,
		Battle = 0xF,
		Menu = 0x10,
		GameOver = 0x11,
		Victory = 0x12,
		ItemJingle = 0x13,
		Crystal2 = 0x14,
		Rest = 0x15,
		//Sound1 = 0x16,
		//Sound2 = 0x17,
		//Minimap = 0x18,  
	}
	public class MusicTracks
	{
		public Dictionary<SongTracks, SongTracks> Tracks { get; private set; }
		public bool SkipMinimap = false;
		private bool chaosBattleMusic = false;
		private MusicShuffle mode;
		public bool MusicShuffled { get; private set; }
		private Dictionary<int, Blob> MusicPointers = new Dictionary<int, Blob>();
		private List<int> songsToPreserve;

		public MusicTracks()
		{
			Tracks = Enum.GetValues<SongTracks>().ToDictionary(t => t, t => t);
			//Tracks[SongTracks.Minimap] = SongTracks.Crystal;
			MusicShuffled = false;
			songsToPreserve = new List<int> { 0x13, 0x15, 0x16, 0x17 };
		}

		public void ShuffleMusic(FF1Rom rom, MusicShuffle _mode, bool alternateAirshipTheme, bool chaosBattleMusic, MT19337 rng)
		{

			//Read music pointer table from rom
			for (int i = 0; i < 22; i++)
			{
				if (_mode == MusicShuffle.MusicDisabled && !songsToPreserve.Contains(i))
				{
					//Set Sq1, Sq2, and Tri channels for every track to all point to the same music data
					MusicPointers.Add(i, Blob.FromHex("C080C080C0800000"));
				}
				else
				{
					MusicPointers.Add(i, rom.GetFromBank(0x0D, 0x8000 + (i * 8), 8));
				}
			}

			if (alternateAirshipTheme) {
				rom.PutInBank(0x0D, 0xB600, Blob.FromHex("FDF804E2D897D9477797DA07D977974777274777D104B6D897D9477797DA07D977974777274777D517B6D90777A7DA07572747D9A7DA07D95777A7D52AB6D017B6FDF803E2D897979797D546B6979797979797979797979797D54DB6070707070707070707070707D55CB6D04DB6FDF809E7C0C0D994DA27477794DB07DAB77794DB07DAB7779475B744240527D9B577D174B6DA045777A7DB045747DAA7DB045747DAA7DB04DAA5DB27DA7454457725D9A7DA045777A7DB045747DAA7DB045747DAA7DB04A7975770D074B6"));
				rom.PutInBank(0x0D, 0x8028, Blob.FromHex("00B641B66EB6"));
			}

			this.chaosBattleMusic = chaosBattleMusic;


			mode = _mode;
			switch (mode)
			{
				case MusicShuffle.None:
					MusicShuffled = false;
					return;
				case MusicShuffle.Standard:
					MusicShuffled = true;
					List<byte> overworldTracks = new List<byte> { 0x0, 0x1, 0x3, 0x4, 0x5, 0x6, 0x9, 0xE };
					List<byte> townTracks = new List<byte> { 0x0, 0x1, 0x4, 0x5, 0x6, 0x7, 0x9, 0xE, 0x10 };
					List<byte> dungeonTracks = new List<byte> { 0x0, 0x1, 0x4, 0x5, 0x6, 0x7, 0x8, 0x9, 0xA, 0xB, 0xC, 0xD, 0x11, 0x12 };

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
					List<byte> tracks = new List<byte> { 0x0, 0x1, 0x2, 0x3, 0x4, 0x5, 0x6, 0x7, 0x8, 0x9, 0xA, 0xB, 0xC, 0xD, 0xE, 0x10, 0x11, 0x12, 0x14 };
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
					//Tracks[SongTracks.Minimap] = (SongTracks)tracks[rng.Between(0, tracks.Count - 1)];
					break;
				case MusicShuffle.MusicDisabled:
					MusicShuffled = true;
					Tracks = Tracks.ToDictionary(t => t.Key, t => (SongTracks)0x0);
					Tracks[SongTracks.ItemJingle] = SongTracks.ItemJingle;
					Tracks[SongTracks.Rest] = SongTracks.Rest;
					break;
			}
		}
		public void Write(FF1Rom rom, Flags flags)
		{

			//Rewrite music pointer table
			foreach (var track in Tracks)
			{
				rom.PutInBank(0x0D,
					0x8000 + ((int)track.Key * 8),
					MusicPointers[(int)track.Value]
				);
			}
			//if (!SkipMinimap)
			//{
			//	rom[0x27C0D] = (byte)Tracks[SongTracks.Minimap];
			//}

			// assign to address
			if (mode == MusicShuffle.MusicDisabled)
			{
				//Overwrite beginning of crystal theme with a song that initializes properly but plays no notes
				rom.PutInBank(0x0D, 0x80C0, Blob.FromHex("FDF805E0D8C7D0C480"));
			}
			if (chaosBattleMusic)
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

				if (flags.SetRNG)
				{
					//SetRNG also wants PrepBattleVarsAndEnterBattle as an entry point, so we return to its routine instead of where we came from
					//Replaces the end of the PutInBank directly above
					rom.PutInBank(0x1C, 0xA7AC, Blob.FromHex("A99948A90248A91B4C03FE"));
				}

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
