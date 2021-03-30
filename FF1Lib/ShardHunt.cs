using RomUtilities;
using System;
using System.Collections.Generic;
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

		public void ShortenToFR(List<Map> maps, bool includeRefightTiles, bool refightAll, bool addExitTile, bool addLutePlate, MT19337 rng)
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
				Blob.FromHex("0004620404040404630402"),
				Blob.FromHex("0304040404040404040405"),
				Blob.FromHex("0604040404040404040408"),
				Blob.FromHex("3006040410041104040830"),
				Blob.FromHex("3130060707070707083031"),
				Blob.FromHex("3131303030363030303131"),
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

			if (addLutePlate)
			{
				// add lute plate (can't use mapNpcIndex 0-2, those belong to Garland)
				SetNpc(MapId.TempleOfFiendsRevisitedChaos, mapNpcIndex: 3, ObjectId.LutePlate, 15, 5, inRoom: true, stationary: true);

				// set walkable black tiles near lute plate to "lute usable" tiles
				for (int y = 2; y <= 4; y++)
				{
					for (int x = 13; x <= 17; x++)
					{
						if (maps[(byte)MapId.TempleOfFiendsRevisitedChaos][y, x].Equals((byte)0x04))
						{
							maps[(byte)MapId.TempleOfFiendsRevisitedChaos][y, x] = (byte)0x20;
						}
					}

				}

				// add statue decorations
				maps[(byte)MapId.TempleOfFiendsRevisitedChaos][6, 14] = (byte)0x10;
				maps[(byte)MapId.TempleOfFiendsRevisitedChaos][6, 16] = (byte)0x11;

				// replace "The tune plays,\nrevealing a stairway." text (0x385BA) originally "9DAB1AB7B8B11AB3AFA4BCB6BF05B5A8B92BAF1FAA2024B7A4ACB55DBCC000"
				Put(0x385BA, FF1Text.TextToBytes("The tune plays,\nopening the pathway.", useDTE: true));

				// make lute plate a single color
				MakeGarlandsBorderTransparent(); // so lute plate change doesn't conflict with Garland, he'll look the same on a black background
				Put(0x02B2D, Blob.FromHex("27")); // change bottom lute plate palette
			}
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
			Put(0x2B981, FF1Text.TextToBytes($"{shardName}  ", false, FF1Text.Delimiter.Null));
			Data[0x2B72A] = 0x81;

			// Replace the upper two tiles of the unlit orb with an empty and found shard.
			// These are at tile address $76 and $77 respectively.
			Put(0x37760, Blob.FromHex("001C22414141221CFFE3DDBEBEBEDDE3001C3E7F7F7F3E1CFFFFE3CFDFDFFFFF"));

			int ppu = 0x2043;
			ppu = ppu + (goal <= 24 ? 0x20 : 0x00);

			// Fancy shard drawing code, see 0E_B8D7_DrawShardBox.asm
			Put(0x3B87D, Blob.FromHex($"A9{ppu & 0xFF:X2}8511A9{(ppu & 0xFF00) >> 8:X2}8512A977A00048AD0220A5128D0620A51118692085118D0620900DAD0220E612A5128D0620A5118D062068A200CC3560D002A976C0{goal:X2}D001608D0720C8E8E006D0EB1890C1"));

			// Black Orb Override to check for shards rather than ORBs.
			talkroutines.Replace(newTalkRoutines.Talk_BlackOrb, Blob.FromHex($"AD3560C9{goal:X2}300CA0CA209690E67DE67DA57160A57260"));
			Put(0x7CDB3, Blob.FromHex("08CE"));

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

		private void MakeGarlandsBorderTransparent()
		{
			// replaces the outline of Garland, stuff that normally displays in dark blue or black
			// with transparency, for use with making a single color lute plate
			Put(0x0B400, Blob.FromHex("0000000601050703"));
			Put(0x0B410, Blob.FromHex("0000006080A0E0C0"));
			Put(0x0B420, Blob.FromHex("0000006060000000"));
			Put(0x0B430, Blob.FromHex("0006060000000000"));
		}
	}
}
