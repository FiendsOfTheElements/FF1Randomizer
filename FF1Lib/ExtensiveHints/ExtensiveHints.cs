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
		MT19337 rng;
		NPCdata npcData;
		Flags flags;
		OverworldMap overworldMap;
		FF1Rom rom;

		IHintSource[] hintSources;
		IHintPlacementProvider[] hintPlacementProviders;

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

			var (forced, prioritized, common, fill) = ProcessCoverage(hints);

			HashSet<ObjectId> usedIds = new HashSet<ObjectId>();
			Dictionary<ObjectId, string> hintPlacement = new Dictionary<ObjectId, string>();

			PlaceBin(rng, forced, usedIds, hintPlacement);
			PlaceBin(rng, prioritized, usedIds, hintPlacement);
			PlaceBin(rng, common, usedIds, hintPlacement);
			PlaceBin(rng, fill, usedIds, hintPlacement);

			var availableIDs = ScavengeDialogIds(hintPlacement.Keys);

			Dictionary<int, string> dialogs = new Dictionary<int, string>();
			int i = 0;

			//If we didn't scavenge enough Ids, make a placeholder text.
			if (hintPlacement.Count > availableIDs.Count)
			{
				dialogs.Add(availableIDs[i], "I dont know what to say.\nMy master gave my TextId\nto someone else.");
				i++;
			}

			foreach (var e in hintPlacement)
			{
				if (i < availableIDs.Count)
				{
					SetNpcHint((ObjectId)e.Key, availableIDs[i], e.Value, dialogs);
					i++;
				}
				else
				{
					SetNpcHint((ObjectId)e.Key, availableIDs[0], null, dialogs);
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
			npcData.SetRoutine(npc, newTalkRoutines.Talk_norm);
			if (text != null) dialogs.Add(textId, text);

			foreach(var npcinstance in rom.FindNpc(npc))
			{
				rom.SetNpc(npcinstance.Item1, npcinstance.Item2.Index, npc, npcinstance.Item2.Coord.x, npcinstance.Item2.Coord.y, npcinstance.Item2.InRoom, true);
			}
		}

		private (List<GeneratedHint>, List<GeneratedHint>, List<GeneratedHint>, List<GeneratedHint>) ProcessCoverage(IEnumerable<GeneratedHint> hints)
		{
			var forced = new List<GeneratedHint>();
			var prioritized = new List<GeneratedHint>();
			var common = new List<GeneratedHint>();
			var fill = new List<GeneratedHint>();


			foreach (var category in hints.GroupBy(h => h.Coverage))
			{
				var cathints = category.ToList();
				int cnt = cathints.Count * GetCoveragePercent(category.Key) / 100;

				cathints.Shuffle(rng);

				var taken = cathints.Take(cnt).ToList();

				//Take the ones with fixedNpc out, they need to be placed first
				forced.AddRange(taken.Where(h => h.FixedNpc != ObjectId.None));
				var rest = taken.Where(h => h.FixedNpc == ObjectId.None);

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
			HashSet<ObjectId> usedNPCSet = new HashSet<ObjectId>(usedNPCs);
			List<int> usedDialogs = new List<int>();
			List<int> everythingElse = new List<int>();

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

			var usedDialogSet = new HashSet<int>(usedDialogs.Distinct());
			foreach (var d in everythingElse) if (usedDialogSet.Contains(d)) usedDialogSet.Remove(d);

			return usedDialogSet.ToList();
		}

		private void PlaceBin(MT19337 rng, List<GeneratedHint> hints, HashSet<ObjectId> usedIds, Dictionary<ObjectId, string> dialogs)
		{
			if (hints.Count == 0) return;

			foreach (var hint in hints)
			{
				if (hint.FixedNpc != ObjectId.None && !usedIds.Contains(hint.FixedNpc))
				{
					dialogs.Add(hint.FixedNpc, hint.Text);
					usedIds.Add(hint.FixedNpc);
				}
				else
				{
					var placementProvider = hintPlacementProviders.First(p => p.SupportedStrategies.Contains(hint.PlacementStrategy));
					var pool = placementProvider.GetNpcPool(hint, usedIds);
					if (pool.Count > 0)
					{
						var npc = pool.PickRandom(rng);
						dialogs.Add(npc, hint.Text);
						usedIds.Add(npc);
					}
				}
			}
		}		
	}
}
