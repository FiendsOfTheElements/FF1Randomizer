using System.ComponentModel;

namespace FF1Lib
{
	public enum ShardCount
	{
		[Description("Exactly 16")]
		Count16,
		[Description("Exactly 20")]
		Count20,
		[Description("Exactly 24")]
		Count24,
		[Description("Exactly 28")]
		Count28,
		[Description("Exactly 32")]
		Count32,
		[Description("Exactly 36")]
		Count36,
		[Description("From 16-24")]
		Range16_24,
		[Description("From 24-32")]
		Range24_32,
		[Description("From 16-36")]
		Range16_36,
	}

	public partial class FF1Rom : NesRom
	{
		// the following global variables are used by the tracker to indicate Go Mode
		// how many orbs or shards needed? 
		int OrbShardGoal = 4;

		// true only if "specific" orbs required below
		bool EarthOrbRequired = false;
		bool FireOrbRequired = false;
		bool WaterOrbRequired = false;
		bool AirOrbRequired = false;

		private void BlackOrbMode(TalkRoutines talkRoutines, DialogueData dialogues, Flags flags, Preferences pref, MT19337 rng, MT19337 funRng)
		{
			if (((bool)flags.Treasures) && flags.ShardHunt)
			{
				EnableShardHunt(rng, TalkRoutines, Dialogues, flags.ShardCount, pref.randomShardNames, flags.SpookyFlag, pref.LegacyShardDisplay, funRng);
			}

			if (!flags.ShardHunt && (flags.GameMode != GameModes.DeepDungeon))
			{
				SetOrbRequirement(rng, TalkRoutines, Dialogues, flags.OrbsRequiredCount, flags.OrbsRequiredMode, (bool)flags.OrbsRequiredSpoilers);
			}
		}

		private const int TotalOrbsToInsert = 32;

		private static readonly List<string> ShardNames = new List<string>
		{
			"JEWEL", "PIECE", "CHUNK", "PRISM", "STONE", "SLICE", "WEDGE", "BIGGS", "SLIVR", "ORBLT", "ESPER", "FORCE",
		};

		public async Task AddShardGraphics(int bank, int address, bool legacyShardDisplay, bool orbGraphicsInResourcePack)
		{
			Console.WriteLine("Entering AddShardGraphics");
			if (legacyShardDisplay)
			{
				// Replace the upper two tiles of the unlit orb with an empty and found shard.
				// These are at tile address $76 and $77 respectively.
				PutInBank(bank, address + 0x760, Blob.FromHex("001C22414141221CFFE3DDBEBEBEDDE3001C3E7F7F7F3E1CFFFFE3CFDFDFFFFF"));
			}
			else if (!orbGraphicsInResourcePack)
			{
				Console.WriteLine("AddShardGraphics Step 2");
				var assembly = System.Reflection.Assembly.GetExecutingAssembly();
				var shardGraphicsFile = assembly.GetManifestResourceNames()
					.Single(str => str.EndsWith("orbs_shards.png"));
				var shardGraphicsStream = assembly.GetManifestResourceStream(shardGraphicsFile);
				await SetCustomOrbGraphics(shardGraphicsStream, bank, address + 0x640);
			}
		}

		public void EnableShardHunt(MT19337 rng, TalkRoutines talkroutines, DialogueData dialogues, ShardCount count, bool RandomShardNames, bool skipFlavorText, bool LegacyShardDisplay, MT19337 funRngSeed)
		{
			int goal = 16;
			switch (count) {
				case ShardCount.Count16: goal = 16; break;
				case ShardCount.Count20: goal = 20; break;
				case ShardCount.Count24: goal = 24; break;
				case ShardCount.Count28: goal = 28; break;
				case ShardCount.Count32: goal = 32; break;
				case ShardCount.Count36: goal = 36; break;
				case ShardCount.Range16_24: goal = rng.Between(16, 24); break;
				case ShardCount.Range24_32: goal = rng.Between(24, 32); break;
				case ShardCount.Range16_36: goal = rng.Between(16, 36); break;
			}
			OrbShardGoal = goal;

			string shardName = "SHARD";
			if (RandomShardNames)
				shardName = ShardNames.PickRandom(funRngSeed);

			// Replace unused CANOE string and EarthOrb pointer with whatever we're calling the scavenged item.
			ItemsText[(int)Item.Shard] = shardName;

			//addShardIcon(0xD, 0xB760);
			// change orb box dimensions
			PutInBank(0x0E,0xBAA2,Blob.FromHex("01010A09"));
			if (LegacyShardDisplay)
			{
				int ppu = 0x2043;
				ppu = ppu + (goal <= 24 ? 0x20 : 0x00);

				// Fancy shard drawing code, see 0E_B8D7_DrawShardBox.asm
				Put(0x3B87D, Blob.FromHex($"A9{ppu & 0xFF:X2}8511A9{(ppu & 0xFF00) >> 8:X2}8512A977A00048AD0220A5128D0620A51118692085118D0620900DAD0220E612A5128D0620A5118D062068A200CC3560D002A976C0{goal:X2}D001608D0720C8E8E006D0EB1890C1"));
			}
			else
			{
				byte[] ShardGoal = [(byte)goal, (byte)(goal/10 + 0x80), (byte)(goal%10 + 0x80)];
				// New shard display
				
				PutInBank(0x0E,0xB8A5,Blob.FromHex("A9AE48A9C248A91B4C03FEEAEAEAEAEAEAEA"));
				PutInBank(0x1B,0xAEC0,ShardGoal);
				PutInBank(0x1B,0xAEC3,Blob.FromHex("AD0220A9238D0620A9C98D0620A5178D0720A200A9639D106EE8A000AD3560C90AB00CA9FF9D106EE8AD35604C03AFC838E90AC90AB0F8489809809D106EE86809809D106EE8A97A9D106EE8ADC1AE9D106EE8ADC2AE9D106EA903853AA902853B20ABDCA200A9068510BD106EAC0220A4558C0620A4548C06208D0720E8E654C610D0E6A93FAC0220A0238C0620A0C08C06208D0720AD3560CDC0AE9012A9CFAC0220A0238C0620A0C18C06208D0720A90E4C03FE"));

			}

			// Black Orb Override to check for shards rather than ORBs.
			BlackOrbChecksShardsCountFor(goal,talkroutines);

			dialogues[0x21] = $"The {shardName}S coalesce to\nrestore the Black ORB.\n\nBrave Light Warriors....\nDestroy the Evil within!"; // Black Orb Text

			// A little narrative overhaul, skip if something else updated the text
			// We assume that it has stronger narrative importance than shards only
			if (!skipFlavorText)
			{ 
				Blob intro = FF1Text.TextToStory(new string[]
				{
					"The Time Loop has reopened!", "",
					"The ORBS have been smashed!", "", "", "",
					$"The resulting {shardName}S were", "",
					"stolen and scattered around", "",
					"the world to distract while", "",
					"this new evil incubates....", "", "", "",
					"But The Light Warriors return!", "",
					$"They will need {goal} {shardName}S", "",
					"to restore the BLACK ORB and", "",
					"confront this new malevolence.",
				});
				System.Diagnostics.Debug.Assert(intro.Length <= 208);
				Put(0x37F20, intro);

				dialogues.InsertDialogues(new Dictionary<int, string>() {
					{ 0x2E, $"Ah, the Light Warriors!\n\nSo you have collected\nthe {shardName}S and restored\nthe BLACK ORB." },
					{ 0x2F, "Thus you've travelled\n2000 years into the past\nto try to stop me?\n\nStep forward then,\nto your peril!" },
					{ 0x30, "Oh, Light Warriors!\nSuch arrogant bravery.\n\nLet us see whom history\nremembers. En Garde!" },
				});
			}
		}

		public Item ShardHuntTreasureSelector(Item item)
		{
			// The following pile of trash, plus Gold chests from 20 to 400 inclusive amount to precisely 32 chests.
			List<Item> trash = new List<Item> { Item.Heal, Item.Pure, Item.SmallKnife, Item.LargeKnife,
				Item.WoodenRod, Item.Cloth, Item.WoodenShield, Item.Cap, Item.WoodenHelm, Item.Gloves };

			return (trash.Contains(item) || item >= Item.Gold20 && item <= Item.Gold350) ? Item.Shard : item;
		}
		public enum OrbsRequiredMode
		{
			[Description("Any (1-3)")]
			Any,
			[Description("Any (0-4)")]
			AnyAll,
			[Description("Specific (1-3)")]
			Random,
			[Description("Specific (0-4)")]
			RandomAll,
		}

		public void SetOrbRequirement(MT19337 rng, TalkRoutines talkroutines, DialogueData dialogues, int orbsRequiredCount, OrbsRequiredMode mode, bool spoilersEnabled)
		{
			int goal = 0;
			switch (orbsRequiredCount)
			{
				case 4: return; // do nothing
				case 3: goal = 3; break;
				case 2: goal = 2; break;
				case 1: goal = 1; break;
				case 0: goal = 0;  break; 
				case 5:
					if (mode == OrbsRequiredMode.Any || mode == OrbsRequiredMode.Random)
					{
						goal = rng.Between(1, 3);
					}
					else
					{
						goal = new List<(int, int)>([(0,1),(1,4),(2,6),(3,4),(4,1)]).PickRandomItemWeighted(rng);
					}
					break;
			}

			OrbShardGoal = goal;

			if (goal == 0)
			{
				mode = OrbsRequiredMode.Any;
			}

			Dictionary<int, String> updatedBlackOrbDialogue = new Dictionary<int, String>();
			String orbIntro = "The ORBS now cover";
			if (goal == 1)
			{
				orbIntro = "The ORB now covers";
			} else if (goal == 0)
			{
				orbIntro = "You now approach";
			}
			updatedBlackOrbDialogue.Add(0x21, $"{orbIntro}\nthe black ORB..\nTo take a step forward\nis to go back 2000 years\nin time.");

			if (mode.Equals(OrbsRequiredMode.Any) || mode.Equals(OrbsRequiredMode.AnyAll))
			{
				// Orb Requirement is Any 3, Any 2, Any 1, or 0; or all 4

				// Adjust Black Orb talk routine to check for <GOAL> number of orbs.
				// See 11_8200_TalkRoutines.asm
				// talkroutines.Replace(TalkScripts.Talk_BlackOrb, Blob.FromHex($"18AD31606D32606D33606D3460C9{goal:X2}900CA0CA209690E67DE67DA57160A57260"));
				// this new talkroutine below sets the black orb game event flag when it's talked to. This lets us
				// use it as a requirement for indicating Go Mode in the in-game tracker.
				talkroutines.Replace(TalkScripts.Talk_BlackOrb, Blob.FromHex($"A0CA207F9018AD31606D32606D33606D3460C9{goal:X2}900A209690E67DE67DA57160A57260"));

				
				// make portal under Black Orb walkable
				Remove4OrbRequirementForToFRPortal();

				if (spoilersEnabled && goal != 0)
				{
					String total = "";
					switch (goal)
					{
						case 1: total = "ONE"; break;
						case 2: total = "TWO"; break;
						case 3: total = "THREE"; break;
						case 4: total = "FOUR"; break;
					}
					updatedBlackOrbDialogue.Add(0x22, $"The black ORB\nwhispers ominously..\nBring me {total}.");
				}
			} else {
				// Orb Requirement is Random 3, Random 2, or Random 1, or all 4

				List<String> orbsNeeded = BlackOrbRequiresSpecificOrbs(rng, goal, talkroutines);

				if (spoilersEnabled)
				{
					String hintLine1 = "";
					String hintLine2 = "";

					if (orbsNeeded.Count > 1)
					{
						hintLine1 = "swirls colors of";
						for (int i = 0; i < orbsNeeded.Count; i++)
						{
							if (i < orbsNeeded.Count - 1)
							{
								hintLine2 += orbsNeeded[i].ToUpper();
								if (orbsNeeded.Count >= 3) { hintLine2 += ", "; } else { hintLine2 += " "; };
								if (orbsNeeded.Count == 4 && i == 1) hintLine2 += "\n";
							} else
							{
								hintLine2 += "and " + orbsNeeded[i].ToUpper() + ".";
							}
						}
					} else {
						hintLine1 = "swirls with the";
						hintLine2 = "color of " + orbsNeeded[0].ToUpper() + ".";
					}
					updatedBlackOrbDialogue.Add(0x22, $"The black ORB\n{hintLine1}\n{hintLine2}");
				}
			}
			dialogues.InsertDialogues(updatedBlackOrbDialogue);
		}

		private void BlackOrbChecksShardsCountFor(int goal, TalkRoutines talkroutines)
		{
			// black orb typically checks for earth($6031) fire($6032) water ($6033) air ($6034)
			// ShiftEarthOrbDown() creates a count at $6035, and this NPC talkroutine compares the $6035 value to goal
			talkroutines.Replace(TalkScripts.Talk_BlackOrb, Blob.FromHex($"AD3560C9{goal:X2}300CA0CA209690E67DE67DA57160A57260"));

			// make portal under Black Orb walkable
			Remove4OrbRequirementForToFRPortal();
		}

		private List<String> BlackOrbRequiresSpecificOrbs(MT19337 rng, int goal, TalkRoutines talkroutines)
		{
			List<String> availableOrbs = new List<String> {	"earth", "fire", "water", "air" };
			List<String> requiredOrbs = new List<String>();

			// choose X random orbs for goal
			for (int i = 0; i < goal; i++)
			{
				// choose random orb from available
				int orb = rng.Between(0, availableOrbs.Count - 1);

				// add to required ; remove from available
				requiredOrbs.Add(availableOrbs[orb]);
				availableOrbs.RemoveAt(orb);
			}

			List<String> requiredOrbsClone = new List<String>(requiredOrbs); // must send copy back for spoiler text

			// change Black Orb requirement for specific orbs

			// Talk_BlackOrb:                     AD 3260 2D 3360 2D 3460 2D 3160 F00CA0CA209690E67DE67DA57160A57260
			//                                      ^fire && watr && air && erth^
			//
			// Example that needs just water orb: AD 3360 2D 3360 2D 3360 2D 3360 F00CA0CA209690E67DE67DA57160A57260
			//                                      ^watr && watr && watr && watr^

			StringBuilder asm = new StringBuilder();
			asm.Append("A0CA207F90AD");
			for (int i = 0; i < 4; i++) // substituting 4 comparisons
			{
				string orbName = requiredOrbs[0];
				switch(orbName)
				{
					case "earth":
						asm.Append("31602D"); // 6031 AND
						EarthOrbRequired = true;
						break;
					case "fire":
						asm.Append("32602D"); // 6032 AND
						FireOrbRequired = true;
						break;
					case "water":
						asm.Append("33602D"); // 6033 AND
						WaterOrbRequired = true;
						break;
					case "air":
						asm.Append("34602D"); // 6034 AND
						AirOrbRequired = true;
						break;
				}
				if (requiredOrbs.Count > 1)
				{
					requiredOrbs.RemoveAt(0);
				}
			}
			asm.Remove(asm.Length - 2, 2); // removes unneeded trailing "2D" from appends above
			// asm.Append("F00CA0CA209690E67DE67DA57160A57260"); // trailing asm from original talkroutine
			// adds a set game event flag when talking to black orb without required orbs, similar to above.
			asm.Append("F00A209690E67DE67DA57160A57260"); // trailing asm from original talkroutine
			talkroutines.Replace(TalkScripts.Talk_BlackOrb, Blob.FromHex(asm.ToString()));
			

			// make portal under Black Orb walkable
			Remove4OrbRequirementForToFRPortal();

			return requiredOrbsClone;
		}

		private void Remove4OrbRequirementForToFRPortal()
		{
			Put(0x7CDB3, Blob.FromHex("08CE"));
		}
	}
}
