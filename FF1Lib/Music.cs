using System.Collections;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace FF1Lib
{
	public enum MusicShuffle
	{
		[Description("None")]
		None = 0,
		[Description("Standard")]
		Standard,
		[Description("Nonsensical")]
		Nonsensical,
		[Description("Disable Music")]
		MusicDisabled
	}

	[Flags]
	public enum MusicUsage
	{
		None = 0,
		Title = 1,
		Overworld = 2,
		Town = 4,
		Shop = 8,
		Dungeon = 0x10,
		Ships = 0x20,
		Menu = 0x40,
		Battle = 0x80,
		Boss = 0x100,
		Other = 0x200,
	}

	public static class MusicUsageExtensions
	{
		public readonly static Dictionary<string, MusicUsage> UsesMap = new(
			from usage in Enum.GetValues<MusicUsage>()
				where usage != MusicUsage.None
				select new KeyValuePair<string, MusicUsage>(usage.ToString(), usage),
			StringComparer.InvariantCultureIgnoreCase);

		public static MusicUsage ToMusicUsage(this string use)
		{
			return UsesMap[use];
		}

		public static MusicUsage ToMusicUsage(this IEnumerable<string> uses)
		{
			MusicUsage usage = MusicUsage.None;
			foreach (string str in uses)
				usage |= UsesMap[str];

			return usage;
		}

		public static MusicUsesConverter ToMusicUsages(this IEnumerable<string> uses)
		{
			return new MusicUsesConverter(uses);
		}

		public struct MusicUsesConverter : IEnumerable<MusicUsage>
		{
			public IEnumerable<string> Uses { get; private set; }

			public MusicUsesConverter(IEnumerable<string> uses)
			{
				Uses = uses;
			}

			IEnumerator IEnumerable.GetEnumerator()
			{
				return GetEnumerator();
			}

			public IEnumerator<MusicUsage> GetEnumerator()
			{
				foreach (var use in Uses)
					yield return UsesMap[use];
			}
		}
	}

	public abstract class SongBase
	{
		[XmlElement(DataType = "int")]
		public int Number { get; set; } = 0;

		[XmlArray(ElementName = "Uses")]
		[XmlArrayItem(ElementName = "Usage")]
		public HashSet<string> Uses { get; set; } = new(StringComparer.InvariantCultureIgnoreCase);

		[XmlIgnore]
		public MusicUsageExtensions.MusicUsesConverter Usages
		{
			get { return Uses.ToMusicUsages();  }
		}

		[XmlIgnore]
		public MusicUsage Usage
		{
			get { return Uses.ToMusicUsage(); }
		}
	}

	public class NativeSong : SongBase
	{
		public NativeSong(int num, IEnumerable<string> uses)
		{
			Number = num;
			Uses.UnionWith(uses);
		}

		public NativeSong(int num, MusicUsage uses)
			: this(
				num,
				from usage in Enum.GetValues<MusicUsage>()
					where (uses & usage) != 0
					select usage.ToString())
		{
		}
	}

	[Serializable]
	public class FtSong : SongBase
	{
		[XmlElement(DataType = "boolean")]
		public bool? Enabled { get; set; } = null;

		[XmlElement]
		public string Title { get; set; } = "";

		[XmlElement(DataType = "boolean")]
		public bool? SwapSquareChans { get; set; } = null;
	}

	[Serializable]
	public class FtModule
	{
		[XmlElement(DataType = "boolean")]
		public bool Enabled { get; set; } = true;

		[XmlElement]
		public string Title { get; set; } = "";

		[XmlElement]
		public string Author { get; set; } = "";

		[XmlElement(DataType = "boolean")]
		public bool SwapSquareChans { get; set; } = false;

		[XmlArray(ElementName = "Tags")]
		[XmlArrayItem(ElementName = "Tag")]
		public HashSet<string> Tags { get; set; } = new(StringComparer.InvariantCultureIgnoreCase);

		[XmlArray(ElementName = "Uses")]
		[XmlArrayItem(ElementName = "Usage")]
		public HashSet<string> Uses { get; set; } = new(StringComparer.InvariantCultureIgnoreCase);

		[XmlIgnore]
		public MusicUsageExtensions.MusicUsesConverter Usages
		{
			get { return Uses.ToMusicUsages(); }
		}

		[XmlIgnore]
		public MusicUsage Usage
		{
			get { return Uses.ToMusicUsage(); }
		}

		[XmlElement("StartAddress")]
		public string StartAddressString { get; set; } = "0";

		[XmlIgnore]
		public int StartAddress
		{
			get { return Convert.ToInt32(StartAddressString, 16); }
			set { StartAddressString = $"{value:X}"; }
		}

		[XmlElement(DataType = "hexBinary")]
		public byte[] TrackData { get; set; } = new byte[] { };

		[XmlIgnore]
		public int Size { get { return TrackData.Length; } }

		[XmlArray(ElementName = "Songs")]
		[XmlArrayItem(ElementName = "Song", Type = typeof(FtSong))]
		public List<FtSong> Songs { get; set; } = new List<FtSong>();
	}


	[Serializable]
	[XmlRoot("ModuleSet")]
	public class FtModuleSet
	{
		[XmlArray(ElementName = "Modules")]
		[XmlArrayItem("Module", typeof(FtModule))]
		public List<FtModule> Modules { get; set; } = new List<FtModule>();

		public IEnumerator<FtModule> GetEnumerator()
		{
			return ((IEnumerable<FtModule>)this.Modules).GetEnumerator();
		}

		public void Add(FtModule element)
		{
			this.Modules.Add(element);
		}
	}

	public struct BankOffset
	{
		public int bank;
		public int offset;

		public BankOffset(int bank, int address)
		{
			this.bank = bank;
			this.offset = address;
		}
	}

	struct SongMapEntry
	{
		public byte bankIdx;
		public byte trackIdx;

		public SongMapEntry(byte bankIdx, byte trackIdx)
		{
			this.bankIdx = bankIdx;
			this.trackIdx = trackIdx;
		}
	}
	public partial class FF1Rom
	{
		const int FtMusicBankBaseAddr = 0xa000;
		const int BankSize = 0x2000;

		const int NumNatSongs = 0x18;
		const int MaxSongs = 0x3f;
		readonly static BankOffset SongMapPos = new BankOffset(0x26, 0);
		readonly static BankOffset ModBaseAddrsPos = new BankOffset(0x26, 0x80);
		readonly static BankOffset FormSongIdcsPos = new BankOffset(0x26, 0x100);

		// Currently have a budget of 12 banks
		readonly static Dictionary<MusicUsage, int> MaxUsageSongs = new()
		{
			{ MusicUsage.Title, 1 },
			{ MusicUsage.Overworld, 1 },
			{ MusicUsage.Town, 2 }, // Town and castle, respectively
			{ MusicUsage.Shop, 1 },
			{ MusicUsage.Dungeon, 6 }, // <= 6
			{ MusicUsage.Ships, 2 }, // <= 2
			{ MusicUsage.Menu, 1 },
			{ MusicUsage.Battle, 3 },
			{ MusicUsage.Boss, 3 }, // <= 13. 1 is reserved for last boss.
			{ MusicUsage.Other, 0 },
		};

		const int MaxNonsensicalOtherSongs = 6;

		readonly static Dictionary<int, MusicUsage> NatSongUsages = new()
		{
			{ 0, MusicUsage.Title | MusicUsage.Overworld | MusicUsage.Town | MusicUsage.Dungeon },
			{ 1, MusicUsage.Overworld | MusicUsage.Town | MusicUsage.Dungeon },
			{ 3, MusicUsage.Overworld },
			{ 4, MusicUsage.Overworld | MusicUsage.Town | MusicUsage.Dungeon | MusicUsage.Ships },
			{ 5, MusicUsage.Overworld | MusicUsage.Town | MusicUsage.Dungeon | MusicUsage.Ships },
			{ 6, MusicUsage.Overworld | MusicUsage.Town | MusicUsage.Dungeon },
			{ 7, MusicUsage.Town | MusicUsage.Dungeon },
			{ 8, MusicUsage.Dungeon },
			{ 9, MusicUsage.Overworld | MusicUsage.Town | MusicUsage.Dungeon },
			{ 10, MusicUsage.Dungeon },
			{ 11, MusicUsage.Dungeon },
			{ 12, MusicUsage.Dungeon },
			{ 13, MusicUsage.Dungeon },
			{ 14, MusicUsage.Overworld | MusicUsage.Town | MusicUsage.Shop },
			{ 15, MusicUsage.Battle | MusicUsage.Boss },
			{ 16, MusicUsage.Town | MusicUsage.Menu },
			{ 17, MusicUsage.Dungeon },
			{ 18, MusicUsage.Dungeon },
		};

		readonly static Dictionary<MusicUsage, int[]> SongUsagesRomOffsets = new()
		{
			{ MusicUsage.Title, new[]{ 0x3A226 } },
			{ MusicUsage.Overworld, new[]{ 0x7C649, 0x7C6F9, 0x7C75A, 0x7C75B } },
			{ MusicUsage.Town, new[]{ 0x7CFC3, 0x7CFC4 } }, // Town and castle, respectively
			{ MusicUsage.Shop, new[]{ 0x3A351, 0x3A56E, 0x3A597 } },
			{ MusicUsage.Dungeon, new[]{ 0x7CFC5, 0x7CFC6, 0x7CFC7, 0x7CFC8, 0x7CFC9, 0x7CFCA } },
			{ MusicUsage.Menu, new[]{ 0x3ADB4, 0x3B677 } },
			// Ships, Battle, and Boss are special cases as they map multiple:multiple
			// Other special cases are lineup, ending, bridge cutscene, victory fanfare, gameover, and minigames
		};

		readonly static HashSet<int> BossFormIdcs = new() { 0x73, 0x74, 0x75, 0x76, 0x77, 0x78, 0x79, 0x7a, 0x7c, 0x7d, 0x7e, 0x7f };
		const int LastBossFormIdx = 0x7b;

		List<int> freeBankIdcs = Enumerable.Range(0x27, 0x32 - 0x27).ToList();
		List<FtModule> musicMods = new();
		Dictionary<SongBase, FtModule> songMods = new(ReferenceEqualityComparer.Instance);

		public MemTable<T> GetMemTable<T>(BankOffset bankOffs, int count) where T : unmanaged
		{
			MemTable<T> table = new(this, bankOffs.bank * BankSize + bankOffs.offset, count);

			return table;
		}

		void LoadMusicLibrary(Stream stream, string tag = null)
		{
			XmlSerializer serializer = new(typeof(FtModuleSet));
			FtModuleSet modSet = (FtModuleSet)serializer.Deserialize(stream);
			foreach (var mod in modSet.Modules)
			{
				foreach (var song in mod.Songs)
				{
					foreach (var useStr in song.Uses)
					{
						if (!MusicUsageExtensions.UsesMap.ContainsKey(useStr))
							throw new Exception($"'{useStr}' is not a valid music usage");
					}
				}

				// Normalize by creating an explicit song for implicit songs and propagating inheritables
				if (mod.Songs.Count == 0)
				{
					FtSong song = new()
					{
						Number = 0,
						Title = mod.Title,
					};

					mod.Songs.Add(song);
					songMods[song] = mod;
				}

				mod.Tags.Add("all");
				mod.Tags.Add("ft");
				if ((tag ?? "").Length > 0)
					mod.Tags.Add(tag);
			}

			musicMods.AddRange(modSet.Modules);
		}

		bool TryMakeUsageSongListsStandard(
			List<SongBase> allSongs,
			Dictionary<MusicUsage, List<SongBase>> usagesSongs,
			IList<MusicUsage> defUses,
			bool force = false)
		{
			// Split up the songs by usage
			foreach (var isong in allSongs)
			{
				IEnumerable<MusicUsage> uses = isong.Usages;
				if (isong.Uses.Count == 0)
				{
					FtModule mod = songMods[(FtSong)isong];
					uses = (mod.Uses.Count > 0) ? mod.Usages : defUses;
				}

				foreach (var usage in uses)
				{
					List<SongBase> songs = null;
					if (usagesSongs.TryGetValue(usage, out songs))
						songs.Add(isong);
				}
			}

			// Try to ensure each song has only a single usage, but may need to violate if there aren't enough songs of a given usage
			HashSet<SongBase> usedSongs = new(ReferenceEqualityComparer.Instance);
			var sortedUses = usagesSongs.Keys.ToList();
			sortedUses.Sort((a, b) => MaxUsageSongs[a].CompareTo(MaxUsageSongs[b]));

			foreach (var usage in sortedUses)
			{
				var songs = usagesSongs[usage];

				Debug.Assert(songs.Count > 0);

				int idx = 0, lastIdx = songs.Count - 1, maxSongs = MaxUsageSongs[usage];
				while (idx <= lastIdx && idx < maxSongs)
				{
					var song = songs[idx];
					if (usedSongs.Add(song))
						idx++;
					else
					{
						songs[idx] = songs[lastIdx];
						songs[lastIdx] = song;

						lastIdx--;
					}
				}

				if (idx > 0)
				{
					// Got some number of songs
					if (idx < songs.Count)
						songs.RemoveRange(idx, songs.Count - idx);
				}
				else
				{
					// No unused songs
					if (!force)
						return false;

					// Force 1 to be reused and git er done.
					songs.RemoveRange(1, songs.Count - 1);
				}
			}

			return true;
		}

		void MakeUsageSongListsNonsensical(
			List<SongBase> allSongs,
			Dictionary<MusicUsage, List<SongBase>> usagesSongs)
		{
			// No need for anything fancy, just fill the usage song lists
			allSongs = new(allSongs);
			int songIdx = 0;

			foreach (var (usage, songs) in usagesSongs)
			{
				int numNeeded = (usage != MusicUsage.Other)
					? MaxUsageSongs[usage]
					: MaxNonsensicalOtherSongs;

				if (numNeeded > 0)
				{
					while (songIdx + numNeeded > allSongs.Count)
						allSongs.AddRange(allSongs.Take(allSongs.Count));

					songs.AddRange(allSongs.Skip(songIdx).Take(numNeeded));
				}
			}
		}

		Dictionary<MusicUsage, List<SongBase>> MakeUsageSongLists(
			MusicShuffle mode,
			MT19337 rng,
			IList<string> validTags,
			int numAttempts = 1)
		{
			Debug.Assert(mode != MusicShuffle.None && mode != MusicShuffle.MusicDisabled);

			// Create the list of candidates
			MusicUsage[] defUses = { MusicUsage.Other };
			List<SongBase> allSongs = new();
			HashSet<string> natTags = new(StringComparer.InvariantCultureIgnoreCase) { "all", "builtin", "native" };
			if (natTags.Overlaps(validTags))
			{
				foreach (var (idx, usage) in NatSongUsages)
					allSongs.Add(new NativeSong(idx, usage));
			}
			
			// Add the FT tracks
			foreach (var (isong, mod) in songMods)
			{
				FtSong song = (FtSong)isong;
				if (song.Enabled ?? mod.Enabled
					&& mod.Tags.Overlaps(validTags))
					allSongs.Add(song);
			}

			Dictionary<MusicUsage, List<SongBase>> usagesSongs = new();
			foreach (MusicUsage usage in MusicUsageExtensions.UsesMap.Values)
				usagesSongs[usage] = new();

			numAttempts = Math.Max(numAttempts, 1);
			bool retry = false;
			do
			{
				numAttempts--;
				allSongs.Shuffle(rng);

				if (mode == MusicShuffle.Standard)
				{
					retry = !TryMakeUsageSongListsStandard(allSongs, usagesSongs, defUses, numAttempts == 0);
					if (retry)
					{
						Debug.Assert(numAttempts > 0);

						foreach (var (usage2, songs2) in usagesSongs)
							songs2.Clear();
					}
				}
				else
					MakeUsageSongListsNonsensical(allSongs, usagesSongs);
			} while (retry);

			return usagesSongs;
		}

		Dictionary<FtModule, BankOffset> ImportFtMusic(Dictionary<MusicUsage, List<SongBase>> usagesSongs)
		{
			Dictionary<FtModule, BankOffset> modsPos = new(ReferenceEqualityComparer.Instance);

			// Build the list of modules
			HashSet<FtModule> mods = new(ReferenceEqualityComparer.Instance);
			Dictionary<FtModule, List<FtSong>> modsSongs = new(ReferenceEqualityComparer.Instance);
			foreach (var (usage, songs) in usagesSongs)
			{
				foreach (var isong in songs)
				{
					FtModule mod;
					if (songMods.TryGetValue(isong, out mod))
					{
						mods.Add(mod);

						List<FtSong> modSongs = null;
						if (!modsSongs.TryGetValue(mod, out modSongs))
							modSongs = modsSongs[mod] = new();

						modSongs.Add((FtSong)isong);
					}
				}
			}

			// Place the modules
			if (mods.Count == 0)
				return modsPos;

			List<FtModule> sortedMods = new(mods);
			sortedMods.Sort((a, b) => -a.Size.CompareTo(b.Size));

			int freeBankListIdx = 0;
			while (freeBankListIdx < freeBankIdcs.Count && sortedMods.Count > 0)
			{
				int bankIdx = freeBankIdcs[freeBankListIdx++];
				int modIdx = 0;
				int curOffs = 0;
				int bytesLeft = BankSize;

				while (modIdx < sortedMods.Count)
				{
					var mod = sortedMods[modIdx];
					if (mod.Size <= bytesLeft)
					{
						modsPos[mod] = new BankOffset(bankIdx, curOffs);
						curOffs += mod.Size;

						sortedMods.RemoveAt(modIdx);
						bytesLeft -= mod.Size;
					}
					else
						// Can this be replaced with binary search?
						modIdx++;
				}
			}

			if (sortedMods.Count > 0)
				return null; // Couldn't fit it

			// Import the modules
			foreach (var (mod, pos) in modsPos)
			{
				var data = mod.TrackData.ToArray(); // Copy
				FtmBinary bin = new(data);

				// Swap square chans if necessary
				var songs = modsSongs[mod];
				foreach (FtSong song in songs)
				{
					if (song.SwapSquareChans ?? mod.SwapSquareChans)
						bin.SwapSquareChans(song.Number);
				}

				bin.Rebase(pos.offset + FtMusicBankBaseAddr);

				Put(pos.bank * BankSize + pos.offset, (Blob)data);
			}

			freeBankIdcs.RemoveRange(0, freeBankListIdx);

			return modsPos;
		}

		Dictionary<MusicUsage, List<int>> ImportMusic(
			IList<string> validTags,
			Dictionary<MusicUsage, List<SongBase>> usagesSongs)
		{
			var modsPos = ImportFtMusic(usagesSongs);
			if (modsPos is null)
				return null;

			// Assign the tracks to track numbers and build the tables
			var trackMap = GetMemTable<SongMapEntry>(SongMapPos, MaxSongs);
			var modBaseAddrs = GetMemTable<ushort>(ModBaseAddrsPos, MaxSongs);

			Dictionary<MusicUsage, List<int>> usagesIdcs = new();
			foreach (var usage in usagesSongs.Keys)
				usagesIdcs[usage] = new();

			int songIdx = NumNatSongs;
			foreach (var (usage, songs) in usagesSongs)
			{
				foreach (var isong in songs)
				{
					FtModule mod = null;
					if (songMods.TryGetValue(isong, out mod))
					{
						// FTM
						var pos = modsPos[mod];
						trackMap[songIdx] = checked(new SongMapEntry((byte)(pos.bank ^ 0xff), (byte)isong.Number));
						modBaseAddrs[songIdx] = checked((ushort)(pos.offset + FtMusicBankBaseAddr));

						usagesIdcs[usage].Add(songIdx);

						Console.Write($"{songIdx:x2} ({usage.ToString()}): {((FtSong)isong).Title}\n");

						songIdx++;
					}
					else
						// Native
						usagesIdcs[usage].Add(isong.Number);
				}
			}

			trackMap.StoreTable();
			modBaseAddrs.StoreTable();

			return usagesIdcs;
		}

		void WriteSongIdxList(
			List<int> songIdcs,
			IList<byte> tgtData,
			ICollection<int> tgtIdcs,
			int songIdxBase = 0)
		{
			Debug.Assert(tgtIdcs.Count > 0);

			while (songIdcs.Count < tgtIdcs.Count)
				songIdcs.AddRange(songIdcs.Take(songIdcs.Count));

			foreach (var (offs, idx) in tgtIdcs.Zip(songIdcs))
				tgtData[offs] = checked((byte)(idx + songIdxBase));
		}

		public void ShuffleMusic(MusicShuffle mode, MT19337 rng)
		{
			if (mode == MusicShuffle.None)
				return;
			else if (mode == MusicShuffle.MusicDisabled)
			{
				//Set Sq1, Sq2, and Tri channels for crystal theme all point to the same music data
				Put(0x34000, Blob.FromHex("C080C080C080"));
				//Overwrite beginning of crystal theme with a song that initializes properly but plays no notes
				Put(0x340C0, Blob.FromHex("FDF805E0D8C7D0C480"));

				List<int> AllSongs = new List<int>
				{
					0x7C649, 0x7C6F9, 0x7C75A, 0x7C75B, 0x7C62D, 0x7C75D,
					0x7C235, 0x7C761, 0x7CFC3, 0x7CFC4, 0x7CFC5, 0x7CFC6,
					0x7CFC7, 0x7CFC8, 0x7CFC9, 0x7CFCA, 0x3A226, 0x3A351,
					0x3A56E, 0x3A597, 0x3ADB4, 0x3B677, 0x3997F, 0x37804,
					0x3784E, 0x31E44, 0x3C5EF, 0x2D9C1, 0x36E86, 0x27C0D
				};
				//Set all music playback calls to play the new empty song
				foreach (int address in AllSongs)
				{
					Data[address] = 0x41;
				}

				return;
			}

			// Build the track list
			var assembly = System.Reflection.Assembly.GetExecutingAssembly();
			var resourcePath = assembly.GetManifestResourceNames().First(str => str.EndsWith("FtModules.xml"));
			using (Stream stream = assembly.GetManifestResourceStream(resourcePath))
				LoadMusicLibrary(stream, "builtin");

			// TODO: Load resource pack library if necessary

			string[] validTags = { "all" };
			Dictionary<MusicUsage, List<int>> usagesSongIdcs = null;

			for (int numRetries = 10; numRetries > 0 && usagesSongIdcs is null; numRetries--)
			{
				var usagesSongs = MakeUsageSongLists(mode, rng, validTags, 4);
				usagesSongIdcs = ImportMusic(validTags, usagesSongs);
			}

			Debug.Assert(usagesSongIdcs is not null);

			// Apply it

			// Start with the easy part
			foreach (var (usage, offsets) in SongUsagesRomOffsets)
				WriteSongIdxList(usagesSongIdcs[usage], (byte[])Data, offsets, 0x41);

			// Battles
			var formSongIdcs = GetMemTable<byte>(FormSongIdcsPos, 0x100);

			var trashFormIdcs = Enumerable.Range(0, 0x100).ToHashSet();
			trashFormIdcs.ExceptWith(BossFormIdcs);
			trashFormIdcs.Remove(LastBossFormIdx);

			WriteSongIdxList(usagesSongIdcs[MusicUsage.Battle], formSongIdcs.Data, trashFormIdcs);

			var bossSongIdcs = usagesSongIdcs[MusicUsage.Boss];
			formSongIdcs[LastBossFormIdx] = (byte)bossSongIdcs.Last();
			if (bossSongIdcs.Count > 1)
				bossSongIdcs.RemoveRange(bossSongIdcs.Count - 1, 1);

			WriteSongIdxList(usagesSongIdcs[MusicUsage.Boss], formSongIdcs.Data, BossFormIdcs);

			formSongIdcs.StoreTable();

			// Ship
			var shipSongIdcs = usagesSongIdcs[MusicUsage.Ships];
			Data[0x7C62D] = (byte)shipSongIdcs[0];
			Data[0x7C75D] = (byte)shipSongIdcs[0];

			// Airship
			Data[0x7C235] = (byte)shipSongIdcs[1 % shipSongIdcs.Count];
			Data[0x7C761] = (byte)shipSongIdcs[1 % shipSongIdcs.Count];

			if (mode == MusicShuffle.Nonsensical)
			{
				var otherIdcs = usagesSongIdcs[MusicUsage.Other];

				//Lineup menu
				Data[0x3997F] = (byte)otherIdcs[0];

				//Ending
				Data[0x37804] = (byte)otherIdcs[1];

				//Bridge Cutscene
				Data[0x3784E] = (byte)otherIdcs[2];

				//Battle Fanfare
				Data[0x31E44] = (byte)otherIdcs[3];

				//Gameover
				Data[0x3C5EF] = (byte)otherIdcs[4];

				//Mini Things
				Data[0x36E86] = (byte)otherIdcs[5]; //minigame
				Data[0x27C0D] = (byte)otherIdcs[6]; //minimap
			}
		}
	}
}
