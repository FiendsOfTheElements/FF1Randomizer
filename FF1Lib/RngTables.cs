using RomUtilities;
using SixLabors.ImageSharp.Metadata.Profiles.Icc;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.IO.Compression;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using static System.Math;

namespace FF1Lib
{

	public enum AdvancedEncounterRNG
	{
		[Description("None")]
		None,
		[Description("True RNG")]
		PRNG,
		[Description("Curated")]
		Curated,
		[Description("Random")]
		Random
	}
	public enum EncounterSpacing
	{
		[Description("None")]
		None,
		[Description("Even(ish)")]
		Even,
		[Description("Clustered")]
		Clustered,
		[Description("Random")]
		Random
	}

	
	public class RngTables
	{
		private const int RngOffset = 0xF100;
		private const int BattleRngOffset = 0xFCF1;
		private const int RngBank = 0x0F;
		private const int NewRngOffset = 0xF100;
		private const int NewBattleRngOffset = 0xFCF1;
		private const int NewRngBank = 0x1F;
		private const int RngSize = 256;
		private const int BattleStepRNGOffset = 0xC571; // Bank 1F
		private ushort BattleStepSeed;
		private const int PRNGSeedRoutineBank = 0x1E;
		private const int BattleStepSeedOffset = 0xDB09; // two unused bytes in Bank 1F
		private const int LoadPRNGSeedOnPartyGenOffset = 0x86E0; // Bank 1E

		private FF1Rom _rom;

		private List<byte> BattleRNG;
		private List<byte> EncounterRNG;
		private List<string> PRNGAlgorithms = new List<string> {
			"ADF16F4AADF06F6A4DF16F85116A4DF06F8DF06F45118DF16F60EA",
			"ADF06F0AADF16F2A4DF06F85112A4DF16F8DF16F45118DF06F60EA",
			"ADF06F4DF16F85114AADF06F6A45118DF16F6A4DF06F8DF06F60EA",
			"ADF16F4DF06F85110AADF16F2A45118DF06F2A4DF16F8DF16F60EA"
		};
		private string PRNGAlgorithm;
		public RngTables(FF1Rom rom)
		{
			_rom = rom;
			BattleRNG = rom.GetFromBank(RngBank, BattleRngOffset, RngSize, false).ToBytes().ToList();
			EncounterRNG = rom.GetFromBank(RngBank, RngOffset, RngSize, false).ToBytes().ToList();
		}

		private void CuratedEncounterRNG(Flags flags, MT19337 rng)
		{
			// Console.WriteLine("Curated Encounter Table");
			bool earlyEncounter = (bool)flags.EarlyEncounter;
			bool earlyLongRun = (bool)flags.EarlyLongRun;
			EncounterSpacing spacing = flags.EncounterSpacing;
			if (spacing == EncounterSpacing.Random)
			{
				List<EncounterSpacing> encSpacings = new() {EncounterSpacing.None,EncounterSpacing.Even,EncounterSpacing.Clustered};
				spacing = encSpacings.PickRandom(rng);
			}
			bool spacingClustered = spacing == EncounterSpacing.Clustered;
			bool spacingEven = spacing == EncounterSpacing.Even;
			bool spacingNone = spacing == EncounterSpacing.None;
			

			if (!earlyEncounter && !earlyLongRun && spacingNone)
			{
				return;
			}
			
			int earlyMaxSize = flags.MaxEarlySteps;
			int earlySize = 0;

			List<byte> curatedEncounterTable = new();
			List<byte> earlySeg = new();
			List<byte> runSeg = new();
			List<byte> lastSeg = new();
			List<byte> encounters = new();
			//List<List<byte>> segs = new();

			// this gets a copy so we can do whatever we want with it
			List<byte> encounterRates = _rom.EncounterRates.Get();
			int rateOverworld = encounterRates[0];
			int rateOcean= encounterRates[1];
			
			encounterRates.RemoveRange(0,2);
			// most dungeons is the mode of the collection
			int rateDungeon = encounterRates.GroupBy(i => i).OrderByDescending(grp => grp.Count()).Select(grp => grp.Key).First();
			//int rateLow = encounterRates.Min();
			// bridge of destiny; likely the biggest rate, but depends on scaling
			int rateBoD = encounterRates.Max();
			List<int> importantRates = new() {rateOverworld, rateOcean, rateDungeon, rateBoD};
			int rateMax = importantRates.Max();
			//int rateCluster = rateHigh * 4;
			// full list of encounters
			// encounterRates = encounterRates.Distinct().ToList();
			// encounterRates.Sort();

			if (rateMax == 0)
			{
				return;
			}
			

			

			// list of numbers 0-255
			List<byte> sourceSeg = Enumerable.Range(0,RngSize).Select(i => (byte)i).ToList();
			int sourceLength = RngSize;

			if (earlyEncounter)
			{
				// Console.WriteLine("Early Encounter");
			}
			// first, if there's an early long run, reserve the run
			if (earlyLongRun)
			{
				// Console.WriteLine("Early Long Run");
				int runLength = rng.Between(flags.RunLengthLow, flags.RunLengthHigh);
				runSeg = sourceSeg.TakeLast(runLength).ToList();
				sourceSeg = sourceSeg.SkipLast(runLength).ToList();
				sourceLength = sourceSeg.Count;
				// probably not necessary to shuffle, but one day encounter rng might be used for something else...
				runSeg.Shuffle(rng);
			}

			
			if (spacingNone)
			{
				// Console.WriteLine("Normal Spacing");
				// we only enter this if spacing is random and there is an early encounter, long run, or both,
				// so we don't need to check for those here; either way there's an early segment.
				sourceSeg.Shuffle(rng);
				earlySize = rng.Between(1,earlyMaxSize);
				if (earlyEncounter)
				{
					earlySeg.Add(0);
					sourceSeg.Remove(0);
					earlySize--;
				}
				earlySeg.AddRange(sourceSeg.Take(earlySize));
				earlySeg.Shuffle(rng);
				lastSeg = sourceSeg.Skip(earlySize).ToList();
			}
			else // (spacingClustered || spacingEven)
			{
				
				// separate all the encounters in the game from the safe steps.
				encounters = sourceSeg.Take(rateMax).ToList();
				sourceSeg = sourceSeg.Skip(rateMax).ToList();
				// sourceLength now == enounters.Count + sourceSeg.Count
				sourceSeg.Shuffle(rng);
				
			}
			
			if (spacingEven)
			{
				// Console.WriteLine("Even Spacing");
				/// we really only care about the spacing in the following locations:
				/// 1. ocean
				/// 2. overworld
				/// 3. most dungeons
				/// 4. bridge of destiny / hall of dragons
				/// 
				/// lots of scaling options makes this difficult, but in general,
				/// the spacing of dungeons and overworld is most important.
				/// If we wanted to, we could just do an optimal spacing taking
				/// all of these locations into account equally. This weights things
				/// a little more towards those locations.

				static List<double> evenSpacing(int divBy)
				{
					List<double> even = new();
					for (double i = 0; i < divBy; i++)
					{
						even.Add(i / divBy);
					}
					return even;
				}

				Queue<byte> encounterQueue = new(encounters);
				Queue<byte> sourceQueue = new(sourceSeg);
				const int evenOcean = 0;
				const int evenOverworld = 1;
				const int evenDungeon = 2;
				const int evenBoD = 3;
				Dictionary<int,List<double>> evenSpacings = new()
				{
					{evenOcean, evenSpacing(rateOcean)},
					{evenOverworld, evenSpacing(rateOverworld)},
					{evenDungeon, evenSpacing(rateDungeon)},
					{evenBoD, evenSpacing(rateBoD)}
				};
				
				Dictionary<int,double> weightings = new()
				{
					{evenOcean, 2.0},
					{evenOverworld, 4.0},
					{evenDungeon, 8.0},
					{evenBoD, 1.0}
				};
				foreach (int key in evenSpacings.Keys)
				{
					if (evenSpacings[key].Count == 0)
					{
						evenSpacings.Remove(key);
						weightings.Remove(key);
					}
				}
				
				
				List<int> spacingsByLength = evenSpacings.Keys.OrderBy(i => evenSpacings[i].Count).ToList();
				List<double> weightingsByLength = evenSpacings.Keys.OrderBy(i => evenSpacings[i].Count).Select(i => weightings[i]).ToList();
				// Console.WriteLine($"Spacings By Length: " + string.Join(" ", spacingsByLength));
				// Console.WriteLine($"Weightings By Length: " + string.Join(" ", weightingsByLength));
				int N = spacingsByLength.Count;
				// because encounter scaling always takes Ceiling(rate*encounterMultiplier), N will always be 0, 2, or 4

				Dictionary<int,byte> spacedEncounters = new();
				List<byte> newSeg = new();

				List<int> jitter = new() {-2,-1,-1,0,0,0,0,0,0,1,1,2};
				// if (sourceLength / rateMax <= 4)
				// {
				// 	jitter.Remove(-2);
				// 	jitter.Remove(2);
				// }


				// iterate through N spacings
				for (int n = 0; n < N ; n++)
				{
					List<double> baseSpacing = evenSpacings[spacingsByLength[n]];
					// Console.WriteLine($"This Spacing: " + string.Join(" ",baseSpacing.Select(i => i*sourceLength)));
					double totalWeighting = weightingsByLength.Skip(n).Sum();
					// iterate through the values of the baseSpacing
					for (int i = 0; i < baseSpacing.Count; i++)
					{
						double target = baseSpacing[i];
						double sum = target * weightingsByLength[n];
					
						// iterate through all of the next spacings
						// should skip this for loop if we're on the last n of N

						for (int m = n + 1; m < N; m++)
						{
							List<double> thisSpacing = evenSpacings[spacingsByLength[m]];
							double thisWeighting = weightingsByLength[m];
							double smallestDiff = 1e10;
							
							List<int> nearestIndices = new();
							for (int j = 0; j < thisSpacing.Count; j++)
							{
								double diff = Abs(target - thisSpacing[j]);
								if (diff < smallestDiff)
								{
									smallestDiff = diff;
									nearestIndices.Clear();
									nearestIndices.Add(j);
								}
								else if (diff == smallestDiff)
								{
									nearestIndices.Add(j);
								}
							}
							int nearestIndex = nearestIndices.PickRandom(rng);
							sum += thisSpacing[nearestIndex] * thisWeighting;
							thisSpacing.RemoveAt(nearestIndex);
						}
						// sourceLength == encounters.Count + sourceSeg.Count;
						int newStep = (int)Round(sourceLength*sum / totalWeighting);
						newStep = newStep == 0? 0 : newStep + jitter.PickRandom(rng);
						int correction = newStep > sourceLength/2 ? -1 : 1;
						// while (spacedEncounters.ContainsKey(newStep))
						// {
						// 	Console.WriteLine($"Correcting step {newStep}");
						// 	newStep += correction;
						// }
						spacedEncounters[newStep] = encounterQueue.Dequeue();
					}
				}

				// Console.WriteLine("Step, Encounter");
				// var entries = spacedEncounters.Keys.OrderBy(i => i);
				// foreach (var key in entries)
				// {
				// 	Console.WriteLine($"{key}, {spacedEncounters[key]}");
				// }
				// Console.WriteLine($"Number of Encounters: {spacedEncounters.Count}");
				// Console.WriteLine($"Number of safe steps: {sourceQueue.Count}");

				// sourceLength == encounters.Count + sourceSeg.Count;
				for (int i = 0; i < sourceLength; i++)
				{
					newSeg.Add(spacedEncounters.TryGetValue(i, out byte step) ? step : sourceQueue.Dequeue());
				}

				int rotateBy = rng.Between(0,earlyEncounter? (earlyMaxSize - 1) : (sourceLength - 1));
				earlySize = earlyEncounter? rng.Between(rotateBy + 1,earlyMaxSize) : rng.Between(0,earlyMaxSize);
				newSeg = newSeg.TakeLast(rotateBy).Concat(newSeg.SkipLast(rotateBy)).ToList();
				earlySeg = newSeg.Take(earlySize).ToList();
				lastSeg = newSeg.Skip(earlySize).ToList();
			}


			///// CLUSTERED SPACING
			/// group encounters near to each other
			if (spacingClustered)
			{
				// Console.WriteLine("Clustered Spacing");
				List<List<byte>> segs = new();
				List<int> encountersInClusters = new();

				Dictionary<int,int> clusterDistribution = new()
				{
					{2,3},
					{3,7},
					{4,5},
					{5,1}	
				};
				
				HashSet<int> forbiddenNumbers = new() {rateOcean - 1, rateOverworld - 1, rateDungeon - 1, rateBoD - 1};
				forbiddenNumbers = forbiddenNumbers.Where(i => i > 0).ToHashSet();
				Dictionary<int,List<int>> stepMap = new();
				bool empties = false;

				for (int step = 0; step < rateMax; step++)
				{
					if (forbiddenNumbers.Contains(step))
					{
						continue;
					}
					List<int> stepList = new();
					foreach (int size in clusterDistribution.Keys)
					{
						if (!forbiddenNumbers.Contains(step+size))
						{
							if (step != 0 || !earlyEncounter || !earlyLongRun || size <= earlyMaxSize)
							{
								stepList.AddRange(Enumerable.Repeat(size,clusterDistribution[size]));
							}
						}
					}
					if (step == 0 && stepList.Count == 0)
					{
						stepList.Add(1);
					}
					if (stepList.Count != 0)
					{
						stepMap[step] = stepList;
					}
					else
					{
						empties = true;
					}
				}
				// this is very likely unneeded, but there could be a future where the encounter threat
				// levels are randomized or shuffled.
				while (empties)
				{
					// Console.WriteLine("Entered the Empties Loop");
					empties = false;
					foreach (int step in stepMap.Keys)
					{
						HashSet<int> sizes = stepMap[step].ToHashSet();
						foreach (int size in sizes)
						{
							if (!stepMap.ContainsKey(step+size))
							{
								stepMap[step].RemoveAll(i => i == size);
							}
						}
						if (stepMap[step].Count == 0)
						{
							empties = true;
							stepMap.Remove(step);
						}
					}
				}

				/// get the number of encounters in each successive cluster
				for (int step = 0; step <= stepMap.Keys.Max();)
				{
					
					int size = stepMap[step].PickRandom(rng);
					encountersInClusters.Add(step+size <= rateMax? size : rateMax-step);
					step += size;	
				}

				// Console.WriteLine($"Forbidden Steps: " + string.Join(" ", forbiddenNumbers));
				// Console.WriteLine("Cluster Start, Number of Encounters");
				// int steppy = 0;
				// foreach (int size in encountersInClusters)
				// {
				// 	Console.WriteLine($"{steppy}, {size}");
				// 	steppy+=size;
				// }

				/// make some clusters!
				/// first, an early cluster if needed
				if (earlyEncounter)
				{
					/// we already made sure the number of encounters in the early cluster is
					/// <= the maximum early size if we need an early cluster.
					int numEncs = encountersInClusters[0];
					encountersInClusters.RemoveAt(0);
					// nominal cluster size
					int clusterSize = rng.Between(numEncs*2,numEncs*3);
					if (earlyLongRun)
					{
						// cluster size has to be strict in order to get the encounter and the
						// long run in
						clusterSize = Min(clusterSize,earlyMaxSize);
					}
					int remainder = clusterSize - numEncs;
					List<byte> earlyCluster = new();

					earlyCluster.AddRange(encounters.Take(numEncs));
					encounters = encounters.Skip(numEncs).ToList();
					earlyCluster.AddRange(sourceSeg.Take(remainder));
					sourceSeg = sourceSeg.Skip(remainder).ToList();

					do
					{
						earlyCluster.Shuffle(rng);
					} while (earlyCluster.IndexOf(0) >= earlyMaxSize);
					
					earlySeg = earlyCluster;					
				}

				/// next, the rest of the clusters
				foreach (int numEncs in encountersInClusters)
				{
					List<byte> cluster = new();
					int remainder = rng.Between(numEncs,numEncs*2);
					cluster.AddRange(encounters.Take(numEncs));
					encounters = encounters.Skip(numEncs).ToList();
					cluster.AddRange(sourceSeg.Take(remainder));
					sourceSeg = sourceSeg.Skip(remainder).ToList();
					cluster.Shuffle(rng);
					segs.Add(cluster);		
				}

				// if there's an early long run but 
				// not a forced early encounter,
				// then 40% of the time, pop in an early cluster if one fits
				if (earlyLongRun && !earlyEncounter)
				{
					List<List<byte>> pool = segs.Where(seg => seg.Count < earlyMaxSize).ToList();
					if (pool.Count > 0 && rng.Between(1,10) <= 4)
					{
						earlySeg = pool.PickRandom(rng);
						segs.Remove(earlySeg);							
					}
					
				}

				/// next, safe segments
				while (sourceSeg.Count !=0 )
				{
					/// this could be any size since we're shuffling
					int size = rng.Between(1, 4);
					segs.Add(sourceSeg.Take(size).ToList());
					sourceSeg = sourceSeg.Skip(size).ToList();
				}

				// shuffle all the segments
				segs.Shuffle(rng);

				// add them one by one to the final segment
				foreach (List<byte> seg in segs)
				{
					lastSeg.AddRange(seg);
				}
			}
			EncounterRNG = earlySeg.Concat(runSeg).Concat(lastSeg).ToList();	
		}


		public void Update(Flags flags, MT19337 rng)
		{
			if ((bool)flags.FixMissingBattleRngEntry)
			{
				// of the 256 entries in the battle RNG table, the 98th entry (index 97) is a duplicate '00' where '95' hex / 149 int is absent.
				// you could arbitrarily choose the other '00', the 111th entry (index 110), to replace instead
				BattleRNG[97] = 0x95;
			}

			if ((bool)flags.Rng)
			{
				BattleRNG.Shuffle(rng);
				EncounterRNG.Shuffle(rng);
			}

			if (flags.AdvancedEncounterRNG == AdvancedEncounterRNG.Random)
			{
				flags.AdvancedEncounterRNG = new List<AdvancedEncounterRNG> {AdvancedEncounterRNG.None,AdvancedEncounterRNG.Curated,AdvancedEncounterRNG.PRNG}
											 .PickRandom(rng);
			}


			
			if (flags.AdvancedEncounterRNG == AdvancedEncounterRNG.PRNG)
			{
				// Console.WriteLine("True RNG Encounters");
				BattleStepSeed = (ushort)rng.Between(0x0001, 0xFFFF);
				PRNGAlgorithm = PRNGAlgorithms.PickRandom(rng);
			}
			if (flags.AdvancedEncounterRNG == AdvancedEncounterRNG.Curated)
			{
				CuratedEncounterRNG(flags,rng);
			}
		}
		public void Write(FF1Rom rom, Flags flags)
		{
			rom.PutInBank(NewRngBank, NewRngOffset, EncounterRNG.ToArray());
			rom.PutInBank(NewRngBank, NewBattleRngOffset, BattleRNG.ToArray());

			if (flags.AdvancedEncounterRNG == AdvancedEncounterRNG.Curated)
			{
				// clears out two instructions so that we always step through the table
				// in a positive direction 
				rom.PutInBank(NewRngBank, BattleStepRNGOffset, Blob.FromHex("EAEAEAEA"));

			}

			if (flags.AdvancedEncounterRNG == AdvancedEncounterRNG.PRNG)
			{
				// just after partygen is confirmed, this executes the subroutine to
				// write the battlestep seed into sram
				rom.PutInBank(PRNGSeedRoutineBank, 0x806B, Blob.FromHex("EAEA20E086")); // Jump to 0x86E0 
				rom.PutInBank(PRNGSeedRoutineBank, LoadPRNGSeedOnPartyGenOffset, Blob.FromHex("A9008D0120AD09DB8DF06FAD0ADB8DF16F60"));

				// write the prng over the subroutine that followed the encounter table
				rom.PutInBank(NewRngBank, BattleStepRNGOffset, Blob.FromHex(PRNGAlgorithm));
				rom.PutInBank(NewRngBank, BattleStepSeedOffset, BattleStepSeed);
			}
		}

		public void PrintEncounterTable()
		{
			string encounterRngString = "Encounter Table:";
			var chunks = EncounterRNG.Chunk(16);
			foreach (var chunk in chunks)
			{
				encounterRngString += "\n" + string.Join(" ", chunk);
				
			}
			Console.WriteLine(encounterRngString);
		}
	}
}
