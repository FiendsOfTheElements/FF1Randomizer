using RomUtilities;
using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FF1Lib
{
	public class NpcScriptsLists
	{
		public static class OriginalTalk
		{
			public static readonly Blob Talk_None = Blob.FromHex("9692");
			public static readonly Blob Talk_KingConeria = Blob.FromHex("9792");
			public static readonly Blob Talk_Garland = Blob.FromHex("B192");
			public static readonly Blob Talk_Princess1 = Blob.FromHex("BE92");
			public static readonly Blob Talk_Bikke = Blob.FromHex("D092");
			public static readonly Blob Talk_ElfDoc = Blob.FromHex("0193");
			public static readonly Blob Talk_ElfPrince = Blob.FromHex("1E93");
			public static readonly Blob Talk_Astos = Blob.FromHex("3893");
			public static readonly Blob Talk_Nerrick = Blob.FromHex("5293");
			public static readonly Blob Talk_Smith = Blob.FromHex("6C93");
			public static readonly Blob Talk_Matoya = Blob.FromHex("9893");
			public static readonly Blob Talk_Unne = Blob.FromHex("BA93");
			public static readonly Blob Talk_Vampire = Blob.FromHex("D793");
			public static readonly Blob Talk_Sarda = Blob.FromHex("E493");
			public static readonly Blob Talk_Bahamut = Blob.FromHex("FB93");
			public static readonly Blob Talk_ifvis = Blob.FromHex("1B94");
			public static readonly Blob Talk_SubEng = Blob.FromHex("2894");
			public static readonly Blob Talk_CubeBot = Blob.FromHex("3894");
			public static readonly Blob Talk_Princess2 = Blob.FromHex("4894");
			public static readonly Blob Talk_Fairy = Blob.FromHex("5894");
			public static readonly Blob Talk_Titan = Blob.FromHex("6894");
			public static readonly Blob Talk_CanoeSage = Blob.FromHex("7D94");
			public static readonly Blob Talk_norm = Blob.FromHex("9294");
			public static readonly Blob Talk_Replace = Blob.FromHex("9594");
			public static readonly Blob Talk_CoOGuy = Blob.FromHex("A294");
			public static readonly Blob Talk_fight = Blob.FromHex("AA94");
			public static readonly Blob Talk_Unused = Blob.FromHex("B794");
			public static readonly Blob Talk_ifitem = Blob.FromHex("B894");
			public static readonly Blob Talk_Invis = Blob.FromHex("C594");
			public static readonly Blob Talk_ifbridge = Blob.FromHex("D794");
			public static readonly Blob Talk_ifevent = Blob.FromHex("E294");
			public static readonly Blob Talk_GoBridge = Blob.FromHex("F094");
			public static readonly Blob Talk_BlackOrb = Blob.FromHex("0295");
			public static readonly Blob Talk_4Orb = Blob.FromHex("1F95");
			public static readonly Blob Talk_ifcanoe = Blob.FromHex("3395");
			public static readonly Blob Talk_ifcanal = Blob.FromHex("3E95");
			public static readonly Blob Talk_ifkeytnt = Blob.FromHex("4995");
			public static readonly Blob Talk_ifearthvamp = Blob.FromHex("5995");
			public static readonly Blob Talk_ifairship = Blob.FromHex("6B95");
			public static readonly Blob Talk_ifearthfire = Blob.FromHex("7695");
			public static readonly Blob Talk_CubeBotBad = Blob.FromHex("8695");
			public static readonly Blob Talk_Chime = Blob.FromHex("9495");
		}

		public static Dictionary<Blob, TalkScripts> NewToOldScripts = new()
		{
			{ OriginalTalk.Talk_None, TalkScripts.Talk_None },
			{ OriginalTalk.Talk_KingConeria, TalkScripts.Talk_GiveItemOnFlag },
			{ OriginalTalk.Talk_Garland, TalkScripts.Talk_fight },
			{ OriginalTalk.Talk_Princess1, TalkScripts.Talk_Princess1 },
			{ OriginalTalk.Talk_Bikke, TalkScripts.Talk_Bikke },
			{ OriginalTalk.Talk_ElfDoc, TalkScripts.Talk_ElfDocUnne },
			{ OriginalTalk.Talk_ElfPrince, TalkScripts.Talk_GiveItemOnFlag },
			{ OriginalTalk.Talk_Astos, TalkScripts.Talk_Astos },
			{ OriginalTalk.Talk_Nerrick, TalkScripts.Talk_Nerrick },
			{ OriginalTalk.Talk_Smith, TalkScripts.Talk_TradeItems },
			{ OriginalTalk.Talk_Matoya, TalkScripts.Talk_TradeItems },
			{ OriginalTalk.Talk_Unne, TalkScripts.Talk_ElfDocUnne },
			{ OriginalTalk.Talk_Vampire, TalkScripts.Talk_fight },
			{ OriginalTalk.Talk_Sarda, TalkScripts.Talk_GiveItemOnFlag },
			{ OriginalTalk.Talk_Bahamut, TalkScripts.Talk_Bahamut },
			{ OriginalTalk.Talk_ifvis, TalkScripts.Talk_ifvis },
			{ OriginalTalk.Talk_SubEng, TalkScripts.Talk_SubEng },
			{ OriginalTalk.Talk_CubeBot, TalkScripts.Talk_GiveItemOnFlag },
			{ OriginalTalk.Talk_Princess2, TalkScripts.Talk_GiveItemOnFlag },
			{ OriginalTalk.Talk_Fairy, TalkScripts.Talk_GiveItemOnFlag },
			{ OriginalTalk.Talk_Titan, TalkScripts.Talk_Titan },
			{ OriginalTalk.Talk_CanoeSage, TalkScripts.Talk_GiveItemOnItem },
			{ OriginalTalk.Talk_Replace, TalkScripts.Talk_Replace },
			{ OriginalTalk.Talk_CoOGuy, TalkScripts.Talk_CoOGuy },
			{ OriginalTalk.Talk_fight, TalkScripts.Talk_fight },
			{ OriginalTalk.Talk_Unused, TalkScripts.Talk_None },
			{ OriginalTalk.Talk_ifitem, TalkScripts.Talk_ifitem },
			{ OriginalTalk.Talk_Invis, TalkScripts.Talk_Invis },
			{ OriginalTalk.Talk_ifbridge, TalkScripts.Talk_ifitem },
			{ OriginalTalk.Talk_ifevent, TalkScripts.Talk_ifevent },
			{ OriginalTalk.Talk_GoBridge, TalkScripts.Talk_GoBridge },
			{ OriginalTalk.Talk_BlackOrb, TalkScripts.Talk_BlackOrb },
			{ OriginalTalk.Talk_4Orb, TalkScripts.Talk_4Orb },
			{ OriginalTalk.Talk_ifcanoe, TalkScripts.Talk_ifitem },
			{ OriginalTalk.Talk_ifcanal, TalkScripts.Talk_ifitem },
			{ OriginalTalk.Talk_ifkeytnt, TalkScripts.Talk_ifkeytnt },
			{ OriginalTalk.Talk_ifearthvamp, TalkScripts.Talk_ifearthvamp },
			{ OriginalTalk.Talk_ifairship, TalkScripts.Talk_ifitem },
			{ OriginalTalk.Talk_ifearthfire, TalkScripts.Talk_ifearthfire },
			{ OriginalTalk.Talk_CubeBotBad, TalkScripts.Talk_None },
			{ OriginalTalk.Talk_Chime, TalkScripts.Talk_GiveItemOnFlag },
		};
	}

}
