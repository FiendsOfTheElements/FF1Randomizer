﻿using RomUtilities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

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
		private const int TotalOrbsToInsert = 32;

		public void ShiftEarthOrbDown()
		{
			// The orb rewarding code is inefficient enough there was room to add in giving you a shard as well.
			// When not playing shard hunt this value is unused, but we always run this code because we always
			// shuffle the earth orb orbs down 4 location to make the code simpler throughout FFR.
			// Getting free shards for killing fiends is an interesting semi-incentive and adds an edge to this game.
			// See OF_CE12_ShardRewards.asm
			Put(0x7CE12, Blob.FromHex("A202A000F010A202A001D00AA204A002D004A204A003B93160D00FA901993160188A6D35608D3560E66C1860"));
			Put(0x7CDB9, Blob.FromHex("12CE18CE1ECE24CE")); // Orb handling jump table.

			// Now anyplace that refers to orb_earth in the assembly outside of the above code
			// is going to need updating to the new address. Earth Orb is pretty popular actually.
			List<int> earthOrbPtrsLowBytes = new List<int> {
				0x39483, // Canoe Sage when not using Early Canoe
				0x3950C, // Talk_BlackOrb
				0x39529, // Talk_4Orb (bats in ToF)
				0x39561, // Talk_ifearthvamp
				0x39577, // Talk_ifearthfire
				0x3B8A0, // DrawOrbBox in the main menu
				0x7CE04, // SMMove_4Orbs
			};
			earthOrbPtrsLowBytes.ForEach(address =>
			{
				// It's entirely possible some future mods might touch these addresses so
				// let's put a litle guard here.
				System.Diagnostics.Debug.Assert(Data[address] == 0x35);
				Data[address] = 0x31;
			});
			
			// Fix for four NPCs checking for the Earth Orb in the wrong position (1 in Dwarf Cave, 3 in Melmond)
			Data[MapObjOffset + 0x5D * MapObjSize] = 0x11;
			Data[MapObjOffset + 0x6B * MapObjSize] = 0x11;
			Data[MapObjOffset + 0x70 * MapObjSize] = 0x11;
			Data[MapObjOffset + 0x74 * MapObjSize] = 0x11;

			Data[0x7EF45] = 0x11; // Skip over orbs and shards when printing the item menu
		}

		public void ShortenToFR(List<Map> maps, bool includeRefightTiles, bool refightAll, bool addExitTile, MT19337 rng)
		{
			// Black Orb tile Warp destination change straight to an edit Chaos floor with all the ToFR Chests.
			Data[0x00D80] = 0x80; // Map edits
			Data[0x3F001] = 0x0F;
			Data[0x3F101] = 0x03;
			Data[0x3F201] = 0x3B;

			// ToFR Map Hack
			List<Blob> landingArea = new List<Blob>
			{
				Blob.FromHex("3F3F000101010101023F3F"),
				Blob.FromHex("3F00045D5E5F606104023F"),
				Blob.FromHex("0004622020202020630402"),
				Blob.FromHex("0304042020202020040405"),
				Blob.FromHex("0604042020202020040408"),
				Blob.FromHex("3006040410041104040830"),
				Blob.FromHex("3130060710071107083031"),
				Blob.FromHex("31313030303B3030303131"),
				Blob.FromHex("31383831383A3831383831"),
			};

			if (includeRefightTiles)
			{
				var battles = new List<byte> { 0x57, 0x58, 0x59, 0x5A };
				if (refightAll)
				{
					landingArea.Add(Blob.FromHex($"31{battles[3]:X2}{battles[2]:X2}{battles[1]:X2}{battles[0]:X2}31{battles[0]:X2}{battles[1]:X2}{battles[2]:X2}{battles[3]:X2}31"));
				}
				else
				{
					battles.Shuffle(rng);
					landingArea.Add(Blob.FromHex($"31{battles[0]:X2}3131{battles[1]:X2}31{battles[2]:X2}3131{battles[3]:X2}31"));
				}
			}
			maps[(int)MapId.TempleOfFiendsRevisitedChaos].Put((0x0A, 0x00), landingArea.ToArray());

			if (addExitTile)
			{
				// add warp portal to alternate map, allowing player to Exit ToFR
				maps[(byte)MapId.TempleOfFiendsRevisitedChaos][3, 15] = (byte)Tile.PortalWarp;
			}

			AddLutePlateToChaosFloor(maps);
		}

		private static readonly List<string> ShardNames = new List<string>
		{
			"SHARD", "JEWEL", "PIECE", "CHUNK", "PRISM", "STONE", "SLICE", "WEDGE", "BIGGS", "SLIVR", "ORBLT", "ESPER", "FORCE",
		};

		public void EnableShardHunt(MT19337 rng, TalkRoutines talkroutines, ShardCount count)
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

			string shardName = ShardNames.PickRandom(rng);

			// Replace unused CANOE string and EarthOrb pointer with whatever we're calling the scavenged item.
			ItemsText[(int)Item.Shard] = shardName;

			// Replace the upper two tiles of the unlit orb with an empty and found shard.
			// These are at tile address $76 and $77 respectively.
			Put(0x37760, Blob.FromHex("001C22414141221CFFE3DDBEBEBEDDE3001C3E7F7F7F3E1CFFFFE3CFDFDFFFFF"));

			int ppu = 0x2043;
			ppu = ppu + (goal <= 24 ? 0x20 : 0x00);

			// Fancy shard drawing code, see 0E_B8D7_DrawShardBox.asm
			Put(0x3B87D, Blob.FromHex($"A9{ppu & 0xFF:X2}8511A9{(ppu & 0xFF00) >> 8:X2}8512A977A00048AD0220A5128D0620A51118692085118D0620900DAD0220E612A5128D0620A5118D062068A200CC3560D002A976C0{goal:X2}D001608D0720C8E8E006D0EB1890C1"));

			// Black Orb Override to check for shards rather than ORBs.
			BlackOrbChecksShardsCountFor(goal,talkroutines);

			// A little narrative overhaul.
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

			InsertDialogs(new Dictionary<int, string>() {
				{ 0x21, $"The {shardName}S coalesce to\nrestore the Black ORB.\n\nBrave Light Warriors....\nDestroy the Evil within!" }, // Black Orb Text
				{ 0x2E, $"Ah, the Light Warriors!\n\nSo you have collected\nthe {shardName}S and restored\nthe BLACK ORB." },
				{ 0x2F, "Thus you've travelled\n2000 years into the past\nto try to stop me?\n\nStep forward then,\nto your peril!" },
				{ 0x30, "Oh, Light Warriors!\nSuch arrogant bravery.\n\nLet us see whom history\nremembers. En Garde!" },
			});
		}

		public Item ShardHuntTreasureSelector(Item item)
		{
			// The following pile of trash, plus Gold chests from 20 to 400 inclusive amount to precisely 32 chests.
			List<Item> trash = new List<Item> { Item.Heal, Item.Pure, Item.SmallKnife, Item.LargeKnife,
				Item.WoodenRod, Item.Cloth, Item.WoodenShield, Item.Cap, Item.WoodenHelm, Item.Gloves };

			return (trash.Contains(item) || item >= Item.Gold20 && item <= Item.Gold350) ? Item.Shard : item;
		}

		private void AddLutePlateToChaosFloor(List<Map> maps)
		{
			// add lute plate (can't use mapNpcIndex 0-2, those belong to Garland)
			SetNpc(MapId.TempleOfFiendsRevisitedChaos, mapNpcIndex: 3, ObjectId.LutePlate, 15, 5, inRoom: true, stationary: true);

			// replace "The tune plays,\nrevealing a stairway." text (0x385BA) originally "9DAB1AB7B8B11AB3AFA4BCB6BF05B5A8B92BAF1FAA2024B7A4ACB55DBCC000"
			Put(0x385BA, FF1Text.TextToBytes("The tune plays,\nopening the pathway.", useDTE: true));

			// make lute plate a single color
			MakeGarlandsBorderTransparent(); // so lute plate change doesn't conflict with Garland, he'll look the same on a black background
			Put(0x02B2D, Blob.FromHex("27")); // change bottom lute plate palette
		}

		private void MakeGarlandsBorderTransparent()
		{
			// replaces the outline of Garland, stuff that normally displays in dark blue or black
			// with transparency, for use with making a single color lute plate
			Put(0x0B400, Blob.FromHex("0000000601050703"));
			Put(0x0B410, Blob.FromHex("0000006080A0E0C0"));
			Put(0x0B420, Blob.FromHex("0000006060000000"));
			Put(0x0B430, Blob.FromHex("0006060000000000"));
		}

		public enum OrbsRequiredMode
		{
			[Description("Any")]
			Any,
			[Description("Specific")]
			Random,
		}

		public void SetOrbRequirement(MT19337 rng, TalkRoutines talkroutines, int orbsRequiredCount, OrbsRequiredMode mode, bool spoilersEnabled)
		{
			int goal = 0;
			switch (orbsRequiredCount)
			{
				case 4: return; // do nothing
				case 3: goal = 3; break;
				case 2: goal = 2; break;
				case 1: goal = 1; break;
				case 0: { goal = 0; mode = OrbsRequiredMode.Any; }  break; // for 0 Orbs, force Any
				case 5: goal = rng.Between(1, 3); break;
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

			if (mode.Equals(OrbsRequiredMode.Any))
			{
				// Orb Requirement is Any 3, Any 2, Any 1, or 0

				// Modify "shift earth orb down code" that normally assigns shard values 2 for earth / fire, and 4 for water / wind
				// (now assigns shard value of 1 for all orbs; AKA modded 0F_CE12_OrbRewards.asm)
				Put(0x7CE12, Blob.FromHex("A201A000F010A201A001D00AA201A002D004A201A003B93160D00FA901993160188A6D35608D3560E66C1860"));

				// Adjust Black Orb Behavior to check $6035 for goal "shards" (in this case, the orb count)
				BlackOrbChecksShardsCountFor(goal, talkroutines);

				if (spoilersEnabled && goal != 0)
				{
					String total = "";
					switch (goal)
					{
						case 1: total = "ONE"; break;
						case 2: total = "TWO"; break;
						case 3: total = "THREE"; break;
					}
					updatedBlackOrbDialogue.Add(0x22, $"The black ORB\nwhispers ominously..\nBring me {total}.");
				}
			} else {
				// Orb Requirement is Random 3, Random 2, or Random 1

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
								if (orbsNeeded.Count == 3) { hintLine2 += ", "; } else { hintLine2 += " "; };
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
			InsertDialogs(updatedBlackOrbDialogue);
		}

		private void BlackOrbChecksShardsCountFor(int goal, TalkRoutines talkroutines)
		{
			// black orb typically checks for earth($6031) fire($6032) water ($6033) wind ($6034)
			// ShiftEarthOrbDown() creates a count at $6035, and this NPC talkroutine compares the $6035 value to goal
			talkroutines.Replace(newTalkRoutines.Talk_BlackOrb, Blob.FromHex($"AD3560C9{goal:X2}300CA0CA209690E67DE67DA57160A57260"));

			// make portal under Black Orb walkable
			Remove4OrbRequirementForToFRPortal();
		}

		private List<String> BlackOrbRequiresSpecificOrbs(MT19337 rng, int goal, TalkRoutines talkroutines)
		{
			List<String> availableOrbs = new List<String> {	"earth", "fire", "water", "wind" };
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
			//                                      ^fire && watr && wind && erth^
			//
			// Example that needs just water orb: AD 3360 2D 3360 2D 3360 2D 3360 F00CA0CA209690E67DE67DA57160A57260
			//                                      ^watr && watr && watr && watr^

			StringBuilder asm = new StringBuilder();
			asm.Append("AD");
			for (int i = 0; i < 4; i++) // substituting 4 comparisons
			{
				string orbName = requiredOrbs[0];
				switch(orbName)
				{
					case "earth":
						asm.Append("31602D"); // 6031 AND
						break;
					case "fire":
						asm.Append("32602D"); // 6032 AND
						break;
					case "water":
						asm.Append("33602D"); // 6033 AND
						break;
					case "wind":
						asm.Append("34602D"); // 6034 AND
						break;
				}
				if (requiredOrbs.Count > 1)
				{
					requiredOrbs.RemoveAt(0);
				}
			}
			asm.Remove(24, 2); // removes unneeded trailing "2D" from appends above
			asm.Append("F00CA0CA209690E67DE67DA57160A57260"); // trailing asm from original talkroutine
			talkroutines.Replace(newTalkRoutines.Talk_BlackOrb, Blob.FromHex(asm.ToString()));

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
