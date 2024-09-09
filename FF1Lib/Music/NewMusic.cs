using RomUtilities;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using static FF1Lib.Music.NewMusic;

namespace FF1Lib.Music
{
	public class SongChannel
	{
		public byte[] TrackData;

		public SongChannel(byte[] trackData)
		{
			TrackData = trackData;
		}
		public SongChannel(byte[] trackData, FF1Pointer originalLocation)
		{
			// This constructor is used when we need to normalize pointers for relocating later
			// Pointers end up being as if the track started at $0000
			TrackData = trackData;
			for (int i = 0; i < trackData.Length; i++)
			{
				if (TrackData[i] >= 0xD0 && TrackData[i] <= 0xD7)
				{
					FF1Pointer loopPointer = new FF1Pointer(new byte[] { TrackData[i + 1], TrackData[i + 2] });
					loopPointer -= originalLocation;
					byte[] normPointer = loopPointer.ToBytesLittleEndian();
					TrackData[i + 1] = normPointer[0];
					TrackData[i + 2] = normPointer[1];
					i += 2;
				}
			}
		}
		public byte[] GetBytesForLocation(FF1Pointer destination)
		{
			byte[] newTrackData = new byte[TrackData.Length];
			TrackData.CopyTo(newTrackData, 0);
			// Rewrite the pointers to work for where we're writing the song
			for (int i = 0; i < newTrackData.Length; i++)
			{
				if (newTrackData[i] >= 0xD0 && newTrackData[i] <= 0xD7)
				{
					FF1Pointer loopPointer = new FF1Pointer(new byte[] { newTrackData[i + 1], newTrackData[i + 2] });
					loopPointer += destination;
					byte[] normPointer = loopPointer.ToBytesLittleEndian();
					newTrackData[i + 1] = normPointer[0];
					newTrackData[i + 2] = normPointer[1];
					i += 2;
				}
			}

			return newTrackData;
		}
	}
	public class Song
	{
		public string Name { get; set; }
		public SongChannel ChannelSq1 { get; set; }
		public SongChannel ChannelSq2 { get; set; }
		public SongChannel ChannelTri { get; set; }
		public List<ReplacableSongs> ReplacementCandidates { get; set; }

		public Song(SongChannel ChannelSq1, SongChannel ChannelSq2, SongChannel ChannelTri, List<ReplacableSongs> ReplacementCandidates, string Name = "Untitled")
		{
			this.ChannelSq1 = ChannelSq1;
			this.ChannelSq2 = ChannelSq2;
			this.ChannelTri = ChannelTri;
			this.Name = Name;
			this.ReplacementCandidates = ReplacementCandidates;
		}
		//public Song(SongChannel ChannelSq1, SongChannel ChannelSq2, SongChannel ChannelTri, string Name = "Untitled") : this(ChannelSq1, ChannelSq2, ChannelTri, new List<ReplacableSongs>(), Name)
		//{	}
	}
	
	public class NewMusic
	{
		private Dictionary<int, string> SongNameStrings = new Dictionary<int, string>
		{
			{ 0x0, "Crystal" },
			{ 0x1, "Intro" },
			{ 0x2, "Ending" },
			{ 0x3, "Overworld" },
			{ 0x4, "Ship" },
			{ 0x5, "Airship" },
			{ 0x6, "Town" },
			{ 0x7, "Castle" },
			{ 0x8, "EarthCave" },
			{ 0x9, "DwarfCave" },
			{ 0xA, "MarshCave" },
			{ 0xB, "TempleOfFiends" },
			{ 0xC, "SkyPalace" },
			{ 0xD, "TempleOfFiendsRevisited" },
			{ 0xE, "Shop" },
			{ 0xF, "Battle" },
			{ 0x10, "Menu" },
			{ 0x11, "GameOver" },
			{ 0x12, "Victory" },
			{ 0x13, "ItemJingle" },
			{ 0x14, "Crystal2" },
			{ 0x15, "Rest" },
			{ 0x16, "UseItemSFX" },
			{ 0x17, "RegularChestSFX" },
		};
		private enum SongNames
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
			UseItemSFX = 0x16,
			ItemGetSFX = 0x17,
			SeaShrine = 0x18,
			ChaosBattle = 0x19,
			Mermaids = 0x1A
		}
		public enum ReplacableSongs
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
			Rest = 0x15,
			SeaShrine = 0x18,
			ChaosBattle = 0x19,
			Mermaids = 0x1A
		}
		private List<Song> OriginalSongs = new List<Song>();

		private void MoveMusicEngine(FF1Rom rom)
		{
			//rom.PutInBank(0x1D, 0x8000, rom.GetFromBank(0x0D, 0x8000, 0x1DA0));  //score data
			rom.PutInBank(0x1D, 0xBA00, rom.GetFromBank(0x0D, 0xB000, 0x5C9));  //music engine code

			//Rewrite music engine pointers
			List<(int, byte)> pointers = new List<(int, byte)> {
				(0xB002, 0xB0),		(0xB04A, 0xB0),		(0xB05B, 0xB1),		(0xB0B3, 0xB0),		(0xB0D4, 0xB0),		(0xB10D, 0xB1),
				(0xB133, 0xB1),		(0xB149, 0xB1),		(0xB14E, 0xB1),		(0xB193, 0xB1),		(0xB1AD, 0xB2),		(0xB1C2, 0xB2),
				(0xB1C5, 0xB1),		(0xB1CC, 0xB2),		(0xB1CF, 0xB1),		(0xB1D6, 0xB2),		(0xB1DB, 0xB1),		(0xB1E2, 0xB2),
				(0xB1E7, 0xB1),		(0xB1FE, 0xB3),		(0xB205, 0xB1),		(0xB20C, 0xB2),		(0xB211, 0xB1),		(0xB216, 0xB1),
				(0xB24F, 0xB2),		(0xB254, 0xB2),		(0xB259, 0xB2),		(0xB25C, 0xB2),		(0xB2D5, 0xB2),		(0xB2ED, 0xB3),
				(0xB2D9, 0xB2),		(0xB2C7, 0xB3)
			};
			foreach (var change in pointers)
			{
				var newbyte = new byte[] { (byte)(change.Item2 + 0x0A) };
				rom.PutInBank(0x1D, change.Item1 + 0x0A00, newbyte);
			}

			//Fixed bank references
			rom.PutInBank(0x1F, 0xC682, Blob.FromHex("1D"));
			rom.PutInBank(0x1F, 0xC68A, Blob.FromHex("1D"));
			rom.PutInBank(0x1F, 0xC688, Blob.FromHex("BA"));
			rom.PutInBank(0x1F, 0xC690, Blob.FromHex("BA"));

			//Code required to handle code in bank 0D that expects to call MusicPlay locally
			rom.PutInBank(0x0D, 0xB099, Blob.FromHex("A9B948A9EF48A91D4C03FE"));
			rom.PutInBank(0x1D, 0xB9F0, Blob.FromHex("2000BAA90D4C03FE"));

			//Set lineup menu to use track $41 ("Crystal") so we don't need 2 references to it
			rom.Put(0x3B677, Blob.FromHex("41"));
			rom.Put(0x3997F, Blob.FromHex("41"));

			//override Music_NewSong to enable finer grained music selection (replaces a JMP address)
			rom.PutInBank(0x1D, 0xBAB2, Blob.FromHex("A0B9"));  
			//Routine for selecting alternate music in certain situations
			//See 1D_B9A0_AlternateMusicSelector.asm
			rom.PutInBank(0x1D, 0xB9A0, Blob.FromHex("C94CD01AAAA548C90CD005A94C4CD4B9C92ED005A95B4CD4B9A9594CD4B9C950D012A56A297FC97BD005A95A4CD4B9A9504CD4B9854B4C03BA"));

		}
		public NewMusic(FF1Rom rom)
		{
			LoadOriginalMusicData(rom);
		}
		public void Write(FF1Rom rom, Preferences preferences, Flags flags, MT19337 rng)
		{
			if (preferences.Music == MusicShuffle.None || preferences.Music == MusicShuffle.MusicDisabled)
			{
				MoveMusicEngine(rom);
				List<Song> songsToWrite = new List<Song>();
				NewMusicData newMusicData = new NewMusicData();

				songsToWrite.InsertRange(0, OriginalSongs);
				// Add default tracks for the 3 additional we add
				songsToWrite.Add(OriginalSongs[(int)SongNames.TempleOfFiend]);
				songsToWrite.Add(OriginalSongs[(int)SongNames.Battle]);
				songsToWrite.Add(OriginalSongs[(int)SongNames.TempleOfFiend]);

				if (preferences.AlternateAirshipTheme)
				{
					songsToWrite[(int)SongNames.Airship] = newMusicData.NewSongs[(int)SongNames.Airship];
				}
				if (preferences.ChaosBattleMusic)
				{
					songsToWrite[(int)SongNames.ChaosBattle] = newMusicData.NewSongs[(int)SongNames.ChaosBattle];
				}
				if (preferences.NewMusic)
				{
					//songsToWrite = newMusicData.NewSongs;
					songsToWrite = SelectMusic(newMusicData, preferences, flags, rng);
				}
				if (preferences.Music == MusicShuffle.MusicDisabled)
				{
					for (int i = 0; i < songsToWrite.Count; i++)
					{
						songsToWrite[i] = newMusicData.Silence;
					}
					songsToWrite[(int)SongNames.Rest] = OriginalSongs[(int)SongNames.Rest];
				}
				WriteSongs(rom, songsToWrite);
			}
			
		}

		private List<Song> SelectMusic(NewMusicData newMusicData, Preferences preferences, Flags flags, MT19337 rng)
		{
			if (preferences.NewMusicStreamSafe || flags.TournamentSafe)
			{
				newMusicData.NewSongs.Remove(newMusicData.NewSongs.Where(x => x.Name == "UnderTheSea").First());
				newMusicData.NewSongs.Remove(newMusicData.NewSongs.Where(x => x.Name == "JeopardyThink").First());

				Song tmp = OriginalSongs[(int)SongNames.TempleOfFiend];
				tmp.ReplacementCandidates = new List<ReplacableSongs>() { ReplacableSongs.Mermaids };
				newMusicData.NewSongs.Add(tmp);
			}

			List<Song> SongsToWrite = new List<Song>();
			for (int i = 0; i < Enum.GetValues(typeof(SongNames)).Length; i++)
			{
				SongsToWrite.Add(newMusicData.Silence);
			}

			List<Song> songPool = newMusicData.NewSongs;
			List<ReplacableSongs> replacableSongs = Enum.GetValues(typeof(ReplacableSongs)).Cast<ReplacableSongs>().ToList();
			var candidatesCount = replacableSongs.ToDictionary(
				x => x,
				x => songPool.Select(y => y.ReplacementCandidates.Contains(x)).ToList()
			);

			List<ReplacableSongs> iterOrder = candidatesCount.Keys.OrderBy(x => candidatesCount[x].Count).ToList();
			foreach (ReplacableSongs song in iterOrder)
			{
				var candidates = songPool.Where(x => x.ReplacementCandidates.Contains(song)).ToList();
				candidates.Shuffle(rng);
				Song selectedSong = candidates.First();
				SongsToWrite[(int)song] = selectedSong;
				songPool.Remove(selectedSong);
			}


			return SongsToWrite;
		}

		public void LoadOriginalMusicData(FF1Rom rom)
		{
			List<FF1Pointer> sortedPointers = new List<FF1Pointer>();
			// Song pointer LUT is groups of 4 pointers (8 bytes) per track, with the 4th always being $0000
			for (int i = 0; i < SongNameStrings.Count; i++)
			{
				var tmp = rom.GetFromBank(0x0D, 0x8000 + (i * 8), 6).ToBytes();
				sortedPointers.Add(new byte[] { tmp[0], tmp[1] });
				sortedPointers.Add(new byte[] { tmp[2], tmp[3] });
				sortedPointers.Add(new byte[] { tmp[4], tmp[5] });
			}
			
			// FF1's tracks are not stored in order so we can't just look up the next pointer to get a track's length
			sortedPointers.Add(new byte[] { 0x1E, 0x9D });  //end of music data pointer for length calculation purposes
			sortedPointers = sortedPointers.Distinct().ToList();
			sortedPointers.Sort();

			for (int i = 0; i < SongNameStrings.Count; i++)
			{
				FF1Pointer sq1ptr = rom.GetFromBank(0x0D, 0x8000 + (i * 8), 2).ToBytes();
				FF1Pointer sq2ptr = rom.GetFromBank(0x0D, 0x8000 + (i * 8) + 2, 2).ToBytes();
				FF1Pointer triptr = rom.GetFromBank(0x0D, 0x8000 + (i * 8) + 4, 2).ToBytes();

				int sq1len = sortedPointers[sortedPointers.FindIndex(p => p == sq1ptr) + 1] - sq1ptr;
				int sq2len = sortedPointers[sortedPointers.FindIndex(p => p == sq2ptr) + 1] - sq2ptr;
				int trilen = sortedPointers[sortedPointers.FindIndex(p => p == triptr) + 1] - triptr;

				SongChannel sq1 = new SongChannel(rom.GetFromBank(0x0D, sq1ptr, sq1len), sq1ptr);
				SongChannel sq2 = new SongChannel(rom.GetFromBank(0x0D, sq2ptr, sq2len), sq2ptr);
				SongChannel tri = new SongChannel(rom.GetFromBank(0x0D, triptr, trilen), triptr);
				Song song = new Song(sq1, sq2, tri, new List<ReplacableSongs> (), SongNameStrings[i]);
				OriginalSongs.Add(song);
			}
		}
		public void WriteSongs(FF1Rom rom, List<Song> songs)
		{

			//Preserve SFX tracks
			songs[(int)SongNames.ItemJingle] = OriginalSongs[(int)SongNames.ItemJingle];
			songs[(int)SongNames.UseItemSFX] = OriginalSongs[(int)SongNames.UseItemSFX];
			songs[(int)SongNames.ItemGetSFX] = OriginalSongs[(int)SongNames.ItemGetSFX];


			int lastLocation = 0x8100;
			for (int i = 0; i < songs.Count; i++)
			{
				Song curSong = songs[i];

				FF1Pointer sq1ptrdest = new FF1Pointer(0x8000 + (i * 8));
				FF1Pointer sq2ptrdest = new FF1Pointer(0x8000 + (i * 8)+2);
				FF1Pointer triptrdest = new FF1Pointer(0x8000 + (i * 8)+4);

				FF1Pointer sq1dest = new FF1Pointer(lastLocation);
				FF1Pointer sq2dest = new FF1Pointer(sq1dest + curSong.ChannelSq1.TrackData.Length);
				FF1Pointer tridest = new FF1Pointer(sq2dest + curSong.ChannelSq2.TrackData.Length);
				lastLocation = tridest.address + curSong.ChannelTri.TrackData.Length;

				rom.PutInBank(0x1D, sq1ptrdest, sq1dest.ToBytesLittleEndian());
				rom.PutInBank(0x1D, sq2ptrdest, sq2dest.ToBytesLittleEndian());
				rom.PutInBank(0x1D, triptrdest, tridest.ToBytesLittleEndian());

				rom.PutInBank(0x1D, sq1dest, curSong.ChannelSq1.GetBytesForLocation(sq1dest));
				rom.PutInBank(0x1D, sq2dest, curSong.ChannelSq2.GetBytesForLocation(sq2dest));
				rom.PutInBank(0x1D, tridest, curSong.ChannelTri.GetBytesForLocation(tridest));

			}
		}
	}
}
