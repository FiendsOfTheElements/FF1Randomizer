using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FF1Randomizer
{
	public enum QuestItems
	{
		Tnt = 0x06,
		Crown = 0x02,
		Ruby = 0x09,
		Floater = 0x0b,
		Tail = 0x0d,
		Slab = 0x08
	}

	public static class TreasureConditions
	{
		public static List<int> NotUsed =
			Enumerable.Range(0, 1).Concat(
			Enumerable.Range(145, 4).Concat(
			Enumerable.Range(187, 9).Concat(
			Enumerable.Range(255, 1)))).ToList();

		public static List<int> TntBlocked = Enumerable.Range(46, 210).Except(NotUsed).ToList(); // Most of the game.
		public static List<int> FloaterBlocked = Enumerable.Range(132, 124).Except(NotUsed).ToList(); // Just the northern hemisphere.

		public static List<int> CrownBlocked =
			Enumerable.Range(1, 6).Concat( // Coneria
			Enumerable.Range(10, 3).Concat( // ToF east side
			Enumerable.Range(13, 4).Concat( // Elfland
			Enumerable.Range(17, 3).Concat( // Northwest Castle
			Enumerable.Range(30, 3).Concat( // Marsh Cave
			Enumerable.Range(35, 8)))))).ToList(); // Dwarf Cave

		public static List<int> RubyBlocked =
			Enumerable.Range(62, 8).Concat( // Earth Cave B4
			Enumerable.Range(70, 4).Concat( // Titan's Tunnel
			Enumerable.Range(74, 33).Concat( // Gurgu Volcano
			Enumerable.Range(107, 16).Concat( // Ice Cave
			Enumerable.Range(123, 9).Concat( // Castle of Ordeals
			FloaterBlocked))))).ToList(); // RUBY blocks CANOE, which blocks AIRSHIP.

		public static List<int> ToFR = Enumerable.Range(248, 7).ToList(); // Anything that blocks an ORB will also block these.
		public static List<int> SlabBlocked = Enumerable.Range(196, 52).Concat(ToFR).ToList(); // Mirage Tower + Sky Castle + ToFR
		public static List<int> BottleBlocked = Enumerable.Range(149, 32).Concat(ToFR).ToList(); // Sea Shrine + ToFR
	}
}
