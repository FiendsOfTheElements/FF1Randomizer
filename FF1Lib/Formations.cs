using RomUtilities;
using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FF1Lib
{
	public static class FormationLists
	{
		private const int FirstBossEncounterIndex = 0x73;
		private const int LastBossEncounterIndex = 0x7F;
		private const int WarMECHFormationIndex = 86;
		private const int IrongGol = 0xFF;
		private const int Bahamut = 0x71; // ANKYLO (used for Bahamut)

		public static List<byte> WarMechEncounter = new List<byte>() { WarMECHFormationIndex };
		public static List<byte> IronGolEncounter = new List<byte>() { IrongGol };
		public static List<byte> BahamutEncounter = new List<byte>() { Bahamut, Bahamut + 0x80 };
		public static List<byte> ASideEncounters = Enumerable.Range(1, FirstBossEncounterIndex).Select(value => (byte)value).Except(WarMechEncounter).ToList();
		public static List<byte> BSideEncounters = Enumerable.Range(128, FirstBossEncounterIndex).Select(value => (byte)value).Concat(IronGolEncounter).ToList();
		public static List<byte> AllRandomEncounters = ASideEncounters.Concat(BSideEncounters).ToList();


	}

	public struct ZoneFormation
	{
		public int Index { get; set;}
		public List<byte> Formations { get; set; }

	}
	public class ZoneFormations
	{
		private const int ZoneFormationsOffset = 0x2C000;
		private const int ZoneFormationsSize = 8;
		private const int ZoneCount = 128;
		private byte[] StartingZones = { 0x1B, 0x1C, 0x24, 0x2C };

		private List<ZoneFormation> formations;

		public ZoneFormations(FF1Rom rom)
		{
			formations = rom.Get(ZoneFormationsOffset, ZoneFormationsSize * ZoneCount).Chunk(ZoneFormationsSize).Select((z, i) => new ZoneFormation() { Index = i, Formations = z.ToBytes().ToList() }).ToList();
		}
		public List<List<byte>> GetBytes()
		{
			return formations.Select(z => z.Formations.ToList()).ToList();
		}
		public void SwapDomains(int from, int to)
		{
			var tmp = formations[to];
			formations[to] = new ZoneFormation() { Index = formations[from].Index, Formations = formations[from].Formations };
			formations[from] = new ZoneFormation() { Index = tmp.Index, Formations = tmp.Formations };
		}
		public void UpdateFromBlob(Blob zonedata)
		{
			formations = zonedata.Chunk(ZoneFormationsSize).Select((z, i) => new ZoneFormation() { Index = i, Formations = z.ToBytes().ToList() }).ToList();
		}
		public void ReplaceEncounter(byte originalencounter, byte newencounter)
		{
			foreach (var formation in formations)
			{
				for (int i = 0; i < formation.Formations.Count; i++)
				{
					if (formation.Formations[i] == originalencounter)
					{
						formation.Formations[i] = newencounter;
					}
				}
			}
		}
		public void ShuffleEnemyFormations(MT19337 rng, FormationShuffleMode shuffleMode, bool enemizerenabled)
		{
			if (shuffleMode == FormationShuffleMode.None || enemizerenabled)
			{
				return;
			}


			if (shuffleMode == FormationShuffleMode.Intrazone)
			{
				// intra-zone shuffle, does not change which formations are in zones.
				var oldFormations = formations.Select(f => new ZoneFormation() { Index = f.Index, Formations = new(f.Formations) }).ToList();
				var newFormations = formations.Select(f => new ZoneFormation() { Index = f.Index, Formations = new(f.Formations) }).ToList();

				for (int i = 0; i < ZoneCount; i++)
				{
					var lowFormations = oldFormations[i].Formations.GetRange(0, 4); // shuffle the first 4 formations first
					lowFormations.Shuffle(rng);
					newFormations[i].Formations[0] = lowFormations[0];
					newFormations[i].Formations[1] = lowFormations[1];
					newFormations[i].Formations[2] = lowFormations[2];
					newFormations[i].Formations[3] = lowFormations[3];

					var shuffleFormations = newFormations[i].Formations.GetRange(2, 6); // get formations 2-8
					shuffleFormations.Shuffle(rng);
					for (int j = 2; j < 8; j++)
					{
						newFormations[i].Formations[j] = shuffleFormations[j - 2];
					}

				}

				formations = newFormations;
			}
			if (shuffleMode == FormationShuffleMode.ShuffleRarityTiered)
			{
				// intra-zone shuffle, does not change which formations are in zones.
				var oldFormations = formations.Select(f => new ZoneFormation() { Index = f.Index, Formations = new(f.Formations) }).ToList();
				var newFormations = new List<ZoneFormation>();

				for (int i = 0; i < ZoneCount; i++)
				{
					var currentEncounterZone = oldFormations[i];

					List<(byte, int)> weightedEncounterPool = new List<(byte, int)>();

					//3 tiers
					//encounter groups 1-4 in highest frequency
					//5-6 in mid
					//7-8 in low

					weightedEncounterPool.Add((currentEncounterZone.Formations[0], 48));
					weightedEncounterPool.Add((currentEncounterZone.Formations[1], 48));
					weightedEncounterPool.Add((currentEncounterZone.Formations[2], 48));
					weightedEncounterPool.Add((currentEncounterZone.Formations[3], 48));
					weightedEncounterPool.Add((currentEncounterZone.Formations[4], 24));
					weightedEncounterPool.Add((currentEncounterZone.Formations[5], 24));
					weightedEncounterPool.Add((currentEncounterZone.Formations[6], 8));
					weightedEncounterPool.Add((currentEncounterZone.Formations[7], 8));

					newFormations.Add(new ZoneFormation()
					{
						Index = i,
						Formations = new()
						{
							weightedEncounterPool.SpliceRandomItemWeighted(rng),
							weightedEncounterPool.SpliceRandomItemWeighted(rng),
							weightedEncounterPool.SpliceRandomItemWeighted(rng),
							weightedEncounterPool.SpliceRandomItemWeighted(rng),
							weightedEncounterPool.SpliceRandomItemWeighted(rng),
							weightedEncounterPool.SpliceRandomItemWeighted(rng),
							weightedEncounterPool.SpliceRandomItemWeighted(rng),
							weightedEncounterPool.SpliceRandomItemWeighted(rng),
						}
					});
				}

				formations = newFormations;
			}
			if (shuffleMode == FormationShuffleMode.InterZone)
			{
				// Inter-zone shuffle
				// Get all encounters from zones not surrounding starting area
				List<ZoneFormation> newFormations = new List<ZoneFormation>();
				SortedSet<byte> exclusionZones = new SortedSet<byte>();
				exclusionZones.UnionWith(StartingZones);

				for (byte i = 0; i < ZoneCount; i++)
				{
					if (StartingZones.Contains(i))
					{
						continue;
					}
					var zone = formations[i];
					if (zone.Formations.Sum(f => f) == 0)
					{
						//some unused overworld zones are zero filled so we catch them here to not pollute the formations list
						exclusionZones.Add(i);
					}
					else
					{
						newFormations.Add(zone);
					}
				}

				newFormations.Shuffle(rng);
				// after shuffling, put original starting zones in so only one write is required
				foreach (byte i in exclusionZones)
				{
					//var startZone = ;
					newFormations.Insert(i, formations[i]);
				}

				formations = newFormations.Select((f, i) => new ZoneFormation() { Index = i, Formations = new(f.Formations) }).ToList();
			}

			if (shuffleMode == FormationShuffleMode.Randomize)
			{
				// no-pants mode
				var allowableEncounters = Enumerable.Range(0, 256).ToList();
				var unallowableEncounters = new List<int>() { 0x73, 0x74, 0x75, 0x76, 0x77, 0x78, 0x79, 0x7A, 0x7B, 0x7C, 0x7D, 0x7E, 0x7F, 0xF3, 0xF4, 0xF5, 0xF6, 0xF7, 0xF8, 0xF9, 0xFA, 0xFB, 0xFC, 0xFD };
				allowableEncounters.RemoveAll(x => unallowableEncounters.Contains(x));
				for (byte i = 0; i < ZoneCount; i++)
				{
					if (StartingZones.Contains(i))
					{
						continue;
					}
					for (int j = 0; j < ZoneFormationsSize; j++)
					{
						formations[i].Formations[j] = (byte)allowableEncounters.PickRandom(rng);
					}
				}
			}
		}
		public void UnleashWarMECH(bool unleashwarmech)
		{
			if (!unleashwarmech)
			{
				return;
			}
			//const int WarMECHsToAdd = 1;
			const int WarMECHIndex = 6;
			const byte WarMECHEncounter = 0x56;

			foreach (var zone in formations)
			{
				if (zone.Formations.Sum(f => f) > 0)
				{
					zone.Formations[WarMECHIndex] = WarMECHEncounter;
				}
			}
		}
		public void Write(FF1Rom rom)
		{
			formations = formations.OrderBy(z => z.Index).ToList();
			rom.Put(ZoneFormationsOffset, formations.SelectMany(z => z.Formations.ToArray()).ToArray());
		}
		public ZoneFormation this[int idx]
		{
			get
			{
				return formations[idx];
			}
			set
			{
				formations[idx] = value;
			}
		}

	}
}
