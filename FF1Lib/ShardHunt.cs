using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RomUtilities;

namespace FF1Lib
{
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

			Data[0x7EF45] = 0x11; // Skip over orbs and shards when printing the item menu
		}

		public void ShortenToFR(List<Map> maps, bool includeRefightTiles, MT19337 rng)
		{
			// Black Orb tile Warp destination change straight to an edit Chaos floor with all the ToFR Chests.
			Data[0x00D80] = 0x80; // Map edits
			Data[0x02D01] = 0x0F;
			Data[0x02D41] = 0x03;
			Data[0x02D81] = 0x3B;

			// Free, useless LUTE, for completeness sake.
			Data[0x03021] = 0x01;

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
				battles.Shuffle(rng);
				landingArea.Add(Blob.FromHex($"31{battles[0]:X2}3131{battles[1]:X2}31{battles[2]:X2}3131{battles[3]:X2}31"));
			}
			maps[(int)MapId.TempleOfFiendsRevisitedChaos].Put((0x0A, 0x00), landingArea.ToArray());
		}

		public void EnableShardHunt(MT19337 rng, int goal, bool npcShuffleEnabled)
		{
			if (goal < 1 || goal > 30)
			{
				throw new ArgumentOutOfRangeException();
			}

			if (!npcShuffleEnabled)
			{
				// NPC Shuffle fixes OpenTreasureChest to not play the fanfare for Shards differently
				System.Diagnostics.Debug.Assert(Data[0x7DDA0] == (byte)Item.Tent);
				Data[0x7DDA0] = (byte)Item.Shard;
			}

			var shardName = new List<string>
			{
				"SHARD",
				"JEWEL",
				"PIECE",
				"CHUNK",
				"PRISM",
				"STONE",
				"SLICE",
				"WEDGE",
				"BIGGS",
				"SLIVR",
				"ORBLT",
				"ESPER",
				"FORCE",
			}.PickRandom(rng);

			// Replace unused CANOE string and EarthOrb pointer with whatever we're calling the scavenged item.
			Put(0x2B981, FF1Text.TextToBytes($"{shardName}  ", false, FF1Text.Delimiter.Null));
			Data[0x2B72A] = 0x81;

			// Replace the upper two tiles of the unlit orb with an empty and found shard.
			// These are at tile address $76 and $77 respectively.
			Put(0x37760, Blob.FromHex("001C22414141221CFFE3DDBEBEBEDDE3001C3E7F7F7F3E1CFFFFE3CFDFDFFFFF"));

			String hexCount = goal.ToString("X2");
			String ppuLowByte = goal <= 24 ? "63" : "43";

			// Fancy shard drawing code, see 0E_B8D7_DrawShardBox.asm
			Put(0x3B87D, Blob.FromHex($"A9{ppuLowByte}8511A977A00048AD0220A9208D0620A51118692085118D0620900DAD0220A9218D0620A9038D062068A200CC3560D002A976C0{hexCount}D001608D0720C8E8E006D0EB1890C3"));

			// Black Orb Override to check for shards rather than ORBs.
			Put(0x39502, Blob.FromHex($"AD3560C9{hexCount}300CA0CA209690E67DE67DA51160A51260"));
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
			Put(0x289B2, FF1Text.TextToBytes($"The {shardName}S coalesce to\nrestore the Black ORB.\n\nBrave Light Warriors....\nDestroy the Evil within!")); // Black Orb Text
			Put(0x28CF8, FF1Text.TextToBytes($"Ah, the Light Warriors!\n\nSo you have collected\nthe {shardName}S and restored\nthe BLACK ORB."));
			Put(0x28D57, FF1Text.TextToBytes("Thus you've travelled\n2000 years into the past\nto try to stop me?\n\nStep forward then,\nto your peril!"));
			Put(0x28DAF, FF1Text.TextToBytes("Oh, Light Warriors!\nSuch arrogant bravery.\n\nLet us see whom history\nremembers. En Garde!"));
		}

		public Item ShardHuntTreasureSelector(Item item)
		{
			// The following pile of trash, plus Gold chests from 20 to 400 inclusive amount to precisely 32 chests.
			List<Item> trash = new List<Item> { Item.Heal, Item.Pure, Item.SmallKnife, Item.LargeKnife,
				Item.WoodenRod, Item.Cloth, Item.WoodenShield, Item.Cap, Item.WoodenHelm, Item.Gloves };

			return (trash.Contains(item) || item >= Item.Gold20 && item <= Item.Gold350) ? Item.Shard : item;
		}
	}
}
