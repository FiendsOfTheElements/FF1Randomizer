using RomUtilities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

		public void ShuffleMusic(MusicShuffle _mode, MT19337 rng)
		{
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
			rom[0x3A226] = (byte)Tracks[SongTracks.Crystal2];

			//Shop
			rom[0x3A351] = (byte)Tracks[SongTracks.Shop];
			rom[0x3A56E] = (byte)Tracks[SongTracks.Shop];
			rom[0x3A597] = (byte)Tracks[SongTracks.Shop];

			//Menu
			rom[0x3B677] = (byte)Tracks[SongTracks.Crystal]; // Lineup menu
			rom[0x3997F] = (byte)Tracks[SongTracks.Crystal]; // Lineup menu?

			rom[0x78546] = (byte)Tracks[SongTracks.Menu]; // Moved for stats tracking, not sure about this one?
			rom[0x7BB71] = (byte)Tracks[SongTracks.Menu]; // Previously 0x3ADB4, moved by stats tracking

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
		}

	}

}
