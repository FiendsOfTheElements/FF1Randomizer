using RomUtilities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static FF1Lib.FF1Rom;

namespace FF1Lib
{
	public enum HintCategoryCoverage
	{
		[Description("None - Guaranteed")]
		HintCategoryCoverageNone = 0,

		[Description("20% - Best Effort")]
		HintCategoryCoverage20,

		[Description("40% - Best Effort")]
		HintCategoryCoverage40,

		[Description("60% - Best Effort")]
		HintCategoryCoverage60,

		[Description("80% - Best Effort")]
		HintCategoryCoverage80,

		[Description("All - Best Effort")]
		HintCategoryCoverageAll,

		[Description("All - Prioritized")]
		HintCategoryCoveragePrioritized,

		[Description("All - Least Effort")]
		HintCategoryCoverageFill
	}

	public class ExtensiveHints
	{
		private MT19337 rng;
		private readonly NPCdata npcData;
		private readonly Flags flags;
		private readonly OverworldMap overworldMap;
		private readonly FF1Rom rom;
		private readonly IHintSource[] hintSources;
		private readonly IHintPlacementProvider[] hintPlacementProviders;

		public ExtensiveHints(MT19337 _rng, NPCdata _npcData, Flags _flags, OverworldMap _overworldMap, FF1Rom _rom)
		{
			rng = _rng;
			npcData = _npcData;
			flags = _flags;
			overworldMap = _overworldMap;
			rom = _rom;

			hintSources = new IHintSource[]
			{
				new LooseFloorHintSource(rng, npcData, flags, overworldMap, rom),
				new LooseNameHintSource(rng, npcData, flags, overworldMap, rom),
				new IncNameHintSource(rng, npcData, flags, overworldMap, rom),
				new FloorHintsHintSource(rng, npcData, flags, overworldMap, rom),
				new EqFloorHintSource(rng, npcData, flags, overworldMap, rom),
			};

			hintPlacementProviders = new IHintPlacementProvider[]
			{
				new DefaultHintPlacmentProvider(rng, npcData, flags, overworldMap, rom),
			};
		}

		public void Generate()
		{
			List<GeneratedHint> hints = hintSources.SelectMany(s => s.GetHints()).ToList();

			(List<GeneratedHint> forced, List<GeneratedHint> prioritized, List<GeneratedHint> common, List<GeneratedHint> fill) = ProcessCoverage(hints);

			HashSet<ObjectId> usedIds = new();
			Dictionary<ObjectId, string> hintPlacement = new();

			PlaceBin(rng, forced, usedIds, hintPlacement);
			PlaceBin(rng, prioritized, usedIds, hintPlacement);
			PlaceBin(rng, common, usedIds, hintPlacement);
			PlaceBin(rng, fill, usedIds, hintPlacement);

			List<int> availableIDs = ScavengeDialogIds(hintPlacement.Keys);

			Dictionary<int, string> dialogs = new();
			int i = 0;

			//If we didn't scavenge enough Ids, make a placeholder text.
			if (hintPlacement.Count > availableIDs.Count)
			{
				dialogs.Add(availableIDs[i], "I dont know what to say.\nMy master gave my TextId\nto someone else.");
				i++;
			}

			foreach (KeyValuePair<ObjectId, string> e in hintPlacement)
			{
				if (i < availableIDs.Count)
				{
					SetNpcHint(e.Key, availableIDs[i], e.Value, dialogs);
					i++;
				}
				else
				{
					SetNpcHint(e.Key, availableIDs[0], null, dialogs);
				}
			}

			//That one has got to be in there
			dialogs.Add(0, "Please stop bothering\nthe static scenery.\n\nThank You.");
			dialogs.Add(80, "Please stop bothering\nthe static scenery.\n\nThank You.");

			rom.InsertDialogs(dialogs);
		}

		private void SetNpcHint(ObjectId npc, int textId, string text, Dictionary<int, string> dialogs)
		{
			npcData.GetTalkArray(npc)[(int)TalkArrayPos.dialogue_1] = 0;
			npcData.GetTalkArray(npc)[(int)TalkArrayPos.dialogue_2] = (byte)textId;
			npcData.GetTalkArray(npc)[(int)TalkArrayPos.dialogue_3] = 0;
			npcData.SetRoutine(npc, NewTalkRoutines.Talk_norm);
			if (text != null)
			{
				dialogs.Add(textId, text);
			}

			foreach ((MapId, NPC) npcinstance in rom.FindNpc(npc))
			{
				rom.SetNpc(npcinstance.Item1, npcinstance.Item2.Index, npc, npcinstance.Item2.Coord.x, npcinstance.Item2.Coord.y, npcinstance.Item2.InRoom, true);
			}
		}

		private (List<GeneratedHint>, List<GeneratedHint>, List<GeneratedHint>, List<GeneratedHint>) ProcessCoverage(IEnumerable<GeneratedHint> hints)
		{
			List<GeneratedHint> forced = new List<GeneratedHint>();
			List<GeneratedHint> prioritized = new List<GeneratedHint>();
			List<GeneratedHint> common = new List<GeneratedHint>();
			List<GeneratedHint> fill = new List<GeneratedHint>();


			foreach (IGrouping<HintCategoryCoverage, GeneratedHint> category in hints.GroupBy(h => h.Coverage))
			{
				List<GeneratedHint> cathints = category.ToList();
				int cnt = cathints.Count * GetCoveragePercent(category.Key) / 100;

				cathints.Shuffle(rng);

				List<GeneratedHint> taken = cathints.Take(cnt).ToList();

				//Take the ones with fixedNpc out, they need to be placed first
				forced.AddRange(taken.Where(h => h.FixedNpc != ObjectId.None));
				IEnumerable<GeneratedHint> rest = taken.Where(h => h.FixedNpc == ObjectId.None);

				//put the rest into the priority bins
				if (category.Key == HintCategoryCoverage.HintCategoryCoveragePrioritized)
				{
					prioritized.AddRange(rest);
				}
				else if (category.Key == HintCategoryCoverage.HintCategoryCoverageFill)
				{
					fill.AddRange(rest);
				}
				else
				{
					common.AddRange(rest);
				}
			}

			return (forced, prioritized, common, fill);
		}

		public int GetCoveragePercent(HintCategoryCoverage coverage)
		{
			switch (coverage)
			{
				case HintCategoryCoverage.HintCategoryCoverageNone: return 0;
				case HintCategoryCoverage.HintCategoryCoverage20: return 20;
				case HintCategoryCoverage.HintCategoryCoverage40: return 40;
				case HintCategoryCoverage.HintCategoryCoverage60: return 60;
				case HintCategoryCoverage.HintCategoryCoverage80: return 80;
				case HintCategoryCoverage.HintCategoryCoverageAll: return 100;
				case HintCategoryCoverage.HintCategoryCoveragePrioritized: return 100;
				case HintCategoryCoverage.HintCategoryCoverageFill: return 100;
				default: return 0;
			}
		}

		private List<int> ScavengeDialogIds(IEnumerable<ObjectId> usedNPCs)
		{
			HashSet<ObjectId> usedNPCSet = new(usedNPCs);
			List<int> usedDialogs = new();
			List<int> everythingElse = new();

			for (int i = 0; i < 208; i++)
			{
				if (usedNPCSet.Contains((ObjectId)i))
				{
					usedDialogs.Add(npcData.GetTalkArray((ObjectId)i)[(int)TalkArrayPos.dialogue_1]);
					usedDialogs.Add(npcData.GetTalkArray((ObjectId)i)[(int)TalkArrayPos.dialogue_2]);
					usedDialogs.Add(npcData.GetTalkArray((ObjectId)i)[(int)TalkArrayPos.dialogue_3]);
				}
				else
				{
					everythingElse.Add(npcData.GetTalkArray((ObjectId)i)[(int)TalkArrayPos.dialogue_1]);
					everythingElse.Add(npcData.GetTalkArray((ObjectId)i)[(int)TalkArrayPos.dialogue_2]);
					everythingElse.Add(npcData.GetTalkArray((ObjectId)i)[(int)TalkArrayPos.dialogue_3]);
				}
			}

			HashSet<int> usedDialogSet = new HashSet<int>(usedDialogs.Distinct());
			foreach (int d in everythingElse)
			{
				if (usedDialogSet.Contains(d))
				{
					usedDialogSet.Remove(d);
				}
			}

			return usedDialogSet.ToList();
		}

		private void PlaceBin(MT19337 rng, List<GeneratedHint> hints, HashSet<ObjectId> usedIds, Dictionary<ObjectId, string> dialogs)
		{
			if (hints.Count == 0)
			{
				return;
			}

			foreach (GeneratedHint hint in hints)
			{
				if (hint.FixedNpc != ObjectId.None && !usedIds.Contains(hint.FixedNpc))
				{
					dialogs.Add(hint.FixedNpc, hint.Text);
					usedIds.Add(hint.FixedNpc);
				}
				else
				{
					IHintPlacementProvider placementProvider = hintPlacementProviders.First(p => p.SupportedStrategies.Contains(hint.PlacementStrategy));
					List<ObjectId> pool = placementProvider.GetNpcPool(hint, usedIds);
					if (pool.Count > 0)
					{
						ObjectId npc = pool.PickRandom(rng);
						dialogs.Add(npc, hint.Text);
						usedIds.Add(npc);
					}
				}
			}
		}
	}
}
