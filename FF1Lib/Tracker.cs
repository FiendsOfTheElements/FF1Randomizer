using RomUtilities;
using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace FF1Lib
{
	public enum TrackerIcon
	{
		
		Bridge = 0,
		Canal  = 1,
		Ship = 2,
		Canoe = 3,
		Airship = 4,
		Floater = 5,
		Crown = 6,
		Crystal = 7,
		Herb = 8,
		Nurse = 9,
		Adamant = 10,
		TNT = 11,
		Ruby = 12,
		Tail = 13,
		Bottle = 14,
		Fairy = 15,
		Slab = 16,
		TranslatedSlab = 17,
		Rod = 18,
		Lute = 19,
		Key = 20,
		Oxyale = 21,
		Chime = 22,
		Cube = 23,
		Sara = 24,
		King = 25,
		Bikke = 26,
		CrescentSage = 27,
		Sarda = 28,
		Robot = 29,
		ShopItem = 30,
		SprintShoes = 31,
		Repel = 32,
		Mark = 33,
		Sigil = 34,
		AirBoat = 35,
		Boulder = 36,
		Bahamut = 37,
		EmptyCheckbox = 38,
		FilledCheckbox = 39,
		LockPicking = 40,
		GoMode = 41,
		BlackOrb = 42,
		BlackOrb0 = 43,
		BlackOrb1 = 44,
		BlackOrb2 = 45,
		BlackOrb3 = 46,
		BlackOrb4 = 47,
		HintsL = 48,
		HintsR = 49,
		SRNGL = 50,
		SRNGR = 51,
		SOGOL = 52,
		SOGOR = 53,
	}
	public partial class FF1Rom
	{
		const int TrackerIconBank = 0x12;
		const int TrackerIconOffset = 0x8810;
		const int TrackerCheckboxOffset = 0x8F40;
		const int TrackerMainMenuIconOffset = 0x8C40;

		public void InGameTracker(Flags flags, Flags unmodifiedFlags, Preferences preferences)
		{
			if (!flags.Tracker)
			{
				return;
			}
			
			AddTrackerIcons(flags,unmodifiedFlags);
			// set the width of the "ITEM" box, which we'll use for the tracker:
			//PutInBank(0x0E, 0xBABE, 0x00); // 0xBABE!!
			// This value can be customized to conserve space. Note that the width here does not include the two border columns.
			int columns = 26; // maximum is 28 without moving the starting spot left one column -- the commented PutInBank does that if needed.
			PutInBank(0x0E, 0xBAC0, (byte)(columns + 2));

			byte KingReq;
			byte SageReq;
			byte SardaReq;
			byte BahamutReq;
			byte ShowRemovedFloater;
			byte ShowRemovedTail;
			byte ShowFreeBridge = 0x01;
			byte ShowFreeCanal = 0x01;
			byte ShowFreeShip = 0x01;
			byte ShowFreeCanoe = 0x01;

			if (flags.NoTristateSpoilers)
			{
				KingReq    = ((bool)flags.EarlyKing && unmodifiedFlags.EarlyKing is not null)  ? (byte)0x00 : (byte)ObjectId.Princess1;
				SageReq    = ((bool)flags.EarlySage && unmodifiedFlags.EarlySage is not null)  ? (byte)0x00 : (byte)Item.EarthOrb;
				SardaReq   = ((bool)flags.EarlySarda && unmodifiedFlags.EarlySarda is not null)? (byte)0x00 : (byte)ObjectId.Vampire;
				BahamutReq = ((bool)flags.FightBahamut && (bool)flags.NoTail && unmodifiedFlags.FightBahamut is not null && unmodifiedFlags.NoTail is not null) ? 
					(byte)0x00 : (byte)Item.Tail;
				ShowRemovedFloater = ((bool)flags.NoFloater && !flags.NoOverworld && unmodifiedFlags.NoFloater is not null) || flags.DesertOfDeath ? (byte)0x01 : (byte)0x00;
				ShowRemovedTail = ((bool)flags.NoTail && !(bool)flags.FightBahamut && unmodifiedFlags.NoTail is not null && unmodifiedFlags.FightBahamut is not null) ? 
					(byte)0x01 : (byte)0x00;
				ShowFreeBridge = ((bool)flags.FreeBridge && unmodifiedFlags.FreeBridge is null) ? (byte)0x00 : (byte)0x01;
				ShowFreeCanal = ((bool)flags.FreeCanal && unmodifiedFlags.FreeCanal is null) ? (byte)0x00 : (byte)0x01;
				ShowFreeShip = ((bool)flags.FreeShip && unmodifiedFlags.FreeShip is null) ? (byte)0x00 : (byte)0x01;
				ShowFreeCanoe = ((bool)flags.FreeCanoe && unmodifiedFlags.FreeCanoe is null) ? (byte)0x00 : (byte)0x01;
			}
			else
			{
				KingReq    = (bool)flags.EarlyKing ? (byte)0x00 : (byte)ObjectId.Princess1;
				SageReq    = (bool)flags.EarlySage ? (byte)0x00 : (byte)Item.EarthOrb;
				SardaReq   = (bool)flags.EarlySarda ? (byte)0x00 : (byte)ObjectId.Vampire;
				BahamutReq = (bool)flags.FightBahamut && (bool)flags.NoTail ? (byte)0x00 : (byte)Item.Tail;
				ShowRemovedFloater = ((bool)flags.NoFloater && !(bool)flags.FreeAirship && !flags.NoOverworld) || flags.DesertOfDeath ? (byte)0x01 : (byte)0x00;
				ShowRemovedTail = (bool)flags.NoTail && !(bool)flags.FightBahamut ? (byte)0x01 : (byte)0x00;
			}

			byte[] trackerSettings = 
			[
				(byte)columns,
				KingReq, SageReq, SardaReq, BahamutReq,
				ShowRemovedFloater, ShowRemovedTail,
				ShowFreeBridge, ShowFreeCanal, ShowFreeShip, ShowFreeCanoe
			];

			// redirect "ITEM" box drawing routine to our tracker
			PutInBank(0x0E, 0xB12E, 0x07);
			PutInBank(0x0E, 0xB91D,Blob.FromHex("20EFB8C63B4CD0A0EAEA"));
			PutInBank(0x0E, 0xA0D0, Blob.FromHex("20ABDCA9A148A90F48A91B4C03FE"));

			PutInBank(0x1B, 0xA100, trackerSettings);
			

			//PutInBank(0x1B,0xA110,Blob.FromHex("A9FFA23B9D106ECA10FAA200AD07A1D007A0FA20F5A3900AAD0860F005A9019D106EAD09A1D007A0FC20F5A3900AAD0060F005A9039D2E6EE8AD08A1D007A0FB20F5A3900AAD0C60D005A9029D106EAD0AA1D007A0FD20F5A3900AAD1260F005A9049D2E6EE8AD05A1D011AD0BA1D007A0FE20F5A39009AD0460F004A905D007AD2B60F005A9069D2E6EE8E8AD2260F015A9079D106EA00720F5A39004A975D002A9749D2E6EE8A908851EA00AAD23602052A3A909851EA90A851FA00520F5A32A2901851DA006AD2460206DA3A90B851EA009AD27602052A3A90C851EA008AD26602052A3A01420FBA3B004A975D007AD2960F00AA9749D2E6EA90D9D106EE8AD06A1D01AA00E20F5A39004A975D00CAC04A1F005B92060F00AA9749D2E6EA90E9D106EE8A90F851EA910851FA01320FBA32A2901851DAD2F60206DA3A911851EA912851FA00B20F5A32A2901851DA00FAD2860206DA3AD2A60F015A9139D106EA01620FBA3B004A975D002A9749D2E6EE8AD2160F015A9149D106EA01720FBA3B004A975D002A9749D2E6EE8E8AD2560F005A9159D106EAD2C60F005A9179D2E6EE8AD3060F005A9169D106EAD2E60F005A9189D2E6EE8EAE8A919851EA01220FBA32A29012098A3A91A851EAC01A1D004A901D00620F5A32A2901A0012098A3A91B851EA03F20FBA32A2901A0042098A3A91C851EAC02A1D004A901D003B92060A0152098A3A91D851EAC03A1D004A901D00620F5A32A2901A00D2098A3A91E851EA901A0112098A3A91F9D106EA0FF20F5A39004A975D002A9749D2E6E4CB1A3851020F5A39004A975D006A510F00AA9749D2E6EA51E9D106EE860851020F5A39009A51F9D106EA975D016A51DF007A51F9D106ED009A510F00AA51E9D106EA9749D2E6EE860C900F013A51E9D106E20F5A39004A975D002A9749D2E6EE860A9008D0120A200AD00A1851020D4A3E63B20ABDCA21EAD00A1851020D4A3A90E4C03FEBD106EAC0220A4558C0620A4548C06208D0720E8E654C610D0E660000000000000B900624A4A60B900624A60"));
			PutInBank(0x1B,0xA110,Blob.FromHex("A9FFA23B9D106ECA10FAA200AD07A1D007A0FA20F5A3900AAD0860F005A9019D106EAD09A1D007A0FC20F5A3900AAD0060F005A9039D2E6EE8AD08A1D007A0FB20F5A3900AAD0C60D005A9029D106EAD0AA1D007A0FD20F5A3900AAD1260F005A9049D2E6EE8AD05A1D011AD0BA1D007A0FE20F5A39009AD0460F004A905D007AD2B60F005A9069D2E6EE8E8AD2260F015A9079D106EA00720F5A39004A975D002A9749D2E6EE8A908851EA00AAD23602052A3A909851EA90A851FA00520F5A32A2901851DA006AD2460206DA3A90B851EA009AD27602052A3A90C851EA008AD26602052A3A01420FBA3B004A975D007AD2960F00AA9749D2E6EA90D9D106EE8AD06A1D01AA00E20F5A39004A975D00CAC04A1F005B92060F00AA9749D2E6EA90E9D106EE8A90F851EA910851FA01320FBA32A2901851DAD2F60206DA3A911851EA912851FA00B20F5A32A2901851DA00FAD2860206DA3AD2A60F015A9139D106EA01620FBA3B004A975D002A9749D2E6EE8AD3060F015A9169D106EA01020FBA3B004A975D002A9749D2E6EE8E8AD2560F005A9159D106EAD2C60F005A9179D2E6EE8AD2160F005A9149D106EAD2E60F005A9189D2E6EE8EAE8A919851EA01220FBA32A29012098A3A91A851EAC01A1D004A901D00620F5A32A2901A0012098A3A91B851EA03F20FBA32A2901A0042098A3A91C851EAC02A1D004A901D003B92060A0152098A3A91D851EAC03A1D004A901D00620F5A32A2901A00D2098A3A91E851EA901A0112098A3A91F9D106EA0FF20F5A39004A975D002A9749D2E6E4CB1A3851020F5A39004A975D006A510F00AA9749D2E6EA51E9D106EE860851020F5A39009A51F9D106EA975D016A51DF007A51F9D106ED009A510F00AA51E9D106EA9749D2E6EE860C900F013A51E9D106E20F5A39004A975D002A9749D2E6EE860A9008D0120A200AD00A1851020D4A3E63B20ABDCA21EAD00A1851020D4A3A90E4C03FEBD106EAC0220A4558C0620A4548C06208D0720E8E654C610D0E660000000000000B900624A4A60B900624A60"));

			// set game event flags for overworld items so we can track whether we've seen them. For no tri-state spoilers.
			PutInBank(0x1F,0xE225,Blob.FromHex("A91B2003FE4C00A4EAEAEAEAEAEA"));
			PutInBank(0x1B,0xA400,Blob.FromHex("AD0860F013AE0960AC0A6020DFE3B008ADFA6209028DFA62AD0C60D013AE0D60AC0E6020DFE3B008ADFB6209028DFB62AD0060F013AE0160AC026020DFE3B008ADFC6209028DFC62AD0460F013AE0560AC066020DFE3B008ADFE6209028DFE62A442C002D00BADFD6209028DFD624C6AE2C004D0034C6AE2C008D0034C61E24C33E2"));


			// MAIN MENU TRACKING ITEMS
			// reorder some routine calls in the main menu drawing routine
			PutInBank(0x0E,0XB843,Blob.FromHex("A90120EFB8A9002082B9A90220EFB8A9402082B9A90320EFB8A9802082B9A90420EFB8A9C02082B94C11B9"));
			// Show Lockpicking
			if ((bool)flags.Lockpicking)
			{
				byte RevealLockpicking = (flags.NoTristateSpoilers && unmodifiedFlags.Lockpicking is null) ? (byte)0x00 : (byte)0x01;
				
				// break out of the character box writing routine to check lockpicking and draw the icon if neeeded
				PutInBank(0x0E,0xB984,Blob.FromHex("D0038D046EA9A548A90048A91B4C03FEEAEAEAEAEAEAEAEAEAEAA51E"));
				PutInBank(0x1B,0xA500,RevealLockpicking);
				PutInBank(0x1B,0xA501,Blob.FromHex("205093C000F02CAD00A5D007ADFA622902F020861E20ABDCA5541869648554A944AC0220A4558C0620A4548C06208D0720A61EA903853FBD28631D29631D2A631D2B631D2C631D2D631D2E631D2F63851EA9B948A99D48A90E4C03FE"));
				// PutInBank(0x1B,0xA500,Blob.FromHex("204093C000F020861E20ABDCA5541869648554A944AC0220A4558C0620A4548C06208D0720A61EA903853FBD28631D29631D2A631D2B631D2C631D2D631D2E631D2F63851EA9B948A99D48A90E4C03FE"));
			}

			// reminders for hintgivers, set rng, and save-on-game-over

			byte HintGivers = (byte)0x00;
			byte SetRNG = (byte)0x00;
			byte SOGO = (byte)0x00;
			byte totalReminders = (byte)0x00;
			byte tilesToDraw = (byte)0x08;


			if (flags.ShowReminders)
			{
				HintGivers = ((bool)flags.HintsVillage && !(flags.NoTristateSpoilers && unmodifiedFlags.HintsVillage is null)) ? (byte)0x01 : (byte)0x00;
				SetRNG = flags.SetRNG ? (byte)0x01 : (byte)0x00;
				SOGO = flags.SaveGameWhenGameOver ? (byte)0x01 : (byte)0x00;
				totalReminders = (byte)(HintGivers + SetRNG + SOGO);
				if (totalReminders == 1)
				{
					tilesToDraw = (byte)0x02;
				}
				else if (totalReminders == 2)
				{
					tilesToDraw = (byte)0x06;
				}
				else if (totalReminders == 3)
				{
					tilesToDraw = (byte)0x08;
				}
				else
				{
					tilesToDraw = (byte)0x00;
				}
			}
			
			string OptionBoxDims = totalReminders == 3? "010F0A0E": "020F080E";

			byte[] reminderSettings =
			[
				totalReminders, HintGivers, SetRNG, SOGO, tilesToDraw
			];

			// ; Values:
			// ;   00. Check Go Mode?
			// ;   01. Nametable address for icon (different for shard hunt)
			// ;   02. Shard hunt?				  (inferable, but easier to just include it)
			// ;   03. Number of orbs/shards required
			// ;   04. Earth orb required?           0: no, 1: yes
			// ;   05. Fire orb required?            0: no, 1: yes
			// ;   06. Water orb required?           0: no, 1: yes
			// ;   07. Air orb required?             0: no, 1: yes
			// ;   08. Talk to black orb necessary?  0: no, 1: yes, 2: black orb doesn't give information
			// ;   09. Lockpicking available?
			// ;   0A. Spoil lockpicking?
			// ;   0B. Unlocked ToFR?
			// ;   0C. Spoil unlocked ToFR?
			// Go Mode settings
			byte ShowGoMode = flags.ShowGoMode && flags.GameMode != GameModes.DeepDungeon && !((bool)flags.TrappedChaos && unmodifiedFlags.TrappedChaos is not null) ?
				(byte)0x01 : (byte)0x00;
			byte GoModeNTAddress = (byte)0x63;
			byte GoModeShardHunt = (byte)0x00;
			byte GoModeOrbShardGoal = (byte)OrbShardGoal; // defined in BlackOrb.cs
			byte GoModeEarthRequired = EarthOrbRequired ? (byte)0x01 : (byte)0x00;
			byte GoModeFireRequired = FireOrbRequired ? (byte)0x01 : (byte)0x00;
			byte GoModeWaterRequired = WaterOrbRequired ? (byte)0x01 : (byte)0x00;
			byte GoModeAirRequired = AirOrbRequired ? (byte) 0x01 : (byte)0x00;
			byte GoModeTalkToBlackOrb = (byte)0x00; 
			byte GoModeLockpicking = (bool)flags.Lockpicking ? (byte)0x01 : (byte)0x00;
			byte GoModeSpoilLockpicking = (byte)0x01;
			byte GoModeUnlockedToFR = (bool)flags.ChaosRush ? (byte)0x01 : (byte)0x00;
			byte GoModeSpoilUnlockedToFR = (byte)0x01;

			if (flags.OrbsRequiredCount == 5 ||
				flags.OrbsRequiredMode == OrbsRequiredMode.Random ||
				flags.OrbsRequiredMode == OrbsRequiredMode.RandomAll)
			{
				// require talking to black orb? Will the black orb even give any information?
				GoModeTalkToBlackOrb = (bool)flags.OrbsRequiredSpoilers ? (byte)0x01 : (byte)0x02;
			}

			if (flags.ShardHunt)
			{
				GoModeNTAddress = preferences.LegacyShardDisplay? (byte)0x41 : (byte)0x82;
				GoModeShardHunt = (byte)0x01;
			}

			if (flags.NoTristateSpoilers)
			{
				if (unmodifiedFlags.Lockpicking is null)
				{
					GoModeSpoilLockpicking = (byte)0x00;
				}
				if (unmodifiedFlags.ChaosRush is null)
				{
					GoModeSpoilUnlockedToFR = (byte)0x00;
				}
			}

			byte[] goModeSettings =
			[
				ShowGoMode, GoModeNTAddress,
				GoModeShardHunt, GoModeOrbShardGoal, GoModeEarthRequired, GoModeFireRequired, GoModeWaterRequired, GoModeAirRequired, GoModeTalkToBlackOrb,
				GoModeLockpicking, GoModeSpoilLockpicking, GoModeUnlockedToFR, GoModeSpoilUnlockedToFR
			];

			// jump out of the Option Box drawing routine
			PutInBank(0x0E,0xB911,Blob.FromHex("A9A548A97F48A91B4C03FEEA"));
			PutInBank(0x1B,0xA560,Blob.FromHex(OptionBoxDims));
			PutInBank(0x1B,0xA564,reminderSettings);
			PutInBank(0x1B,0xA569,goModeSettings);
			PutInBank(0x1B,0xA580,Blob.FromHex("AD60A58538AD61A58539AD62A5853CAD63A5853DA91B85572063E0C63B20ABDCE63BE63BAD60A5C902F002E63AAD64A5D0034C36A6A200AD65A5F01FA9479D106EE8A9489D106EE8A9FF9D106EE8AD64A52901D006A9FF9D106EE8AD66A5F01FA9499D106EE8A94A9D106EE8A9FF9D106EE8AD64A52901D006A9FF9D106EE8AD67A5F00BA94B9D106EE8A94C9D106EAD68A58510A200BD106EAC0220A4558C0620A4548C06208D0720E654E8C610D0E64C36A64CF3A6AD69A5D0034C07A7A9FF8D106EA0CAAE71A5F00A20F5A39005A9468D106EAD2160F0DAAD6BA5D06320FBA3B0034CC9A6AD3160AE6DA5F004C900F0C18510AD3260AE6EA5F004C900F0B31865108510AD3360AE6FA5F004C900F0A21865108510AD3460AE70A5F004C900F091186510CD6CA59089C904F01BAE71A5F016E002F03CAD106EC9FFD00B4C07A7AD3560CD6CA5902AAD2560D020AD72A5F011AD046EF00CAD73A5D011A0FA20F5A3B00AAD74A5F00AAD75A5F005A9458D106EAD106EAC0220A0208C0620AC6AA58C06208D0720A204A98648A90148A90E4C03FE"));

		}


		public void DarkenIconTile(byte[] iconTile)
		{
			// modifies an unencoded-for-PPU-tile in place (64 bytes)
			for (int i = 0; i < 64; i++)
			{
				// 0 -> 0
				// 1 -> 0
				// 2 -> 2
				// 3 -> 1
				if ((iconTile[i] & 1) == 1)
				{
					iconTile[i] >>= 1;
				}
			}
		}
		

		public void AddTrackerIcons(Flags flags, Flags unmodifiedFlags)
		{
			
			byte[] BlankTile =
				[
					2,2,2,2,2,2,2,2,
					2,2,2,2,2,2,2,2,
					2,2,2,2,2,2,2,2,
					2,2,2,2,2,2,2,2,
					2,2,2,2,2,2,2,2,
					2,2,2,2,2,2,2,2,
					2,2,2,2,2,2,2,2,
					2,2,2,2,2,2,2,2
				]
			;

			byte[] BlackOrbSpecific =
				[
					2,0,0,0,0,0,0,2,
					0,0,0,0,0,0,0,0,
					0,0,0,0,0,0,0,0,
					0,0,0,0,0,0,0,0,
					0,0,0,0,0,0,0,0,
					0,0,0,0,0,0,0,0,
					0,0,0,0,0,0,0,0,
					2,0,0,0,0,0,0,2,
				]
			;

			byte[] BlackOrbSpecificEarth =
				// [
				// 	0,0,0,0,0,0,0,0,
				// 	0,0,0,0,0,0,0,0,
				// 	0,0,0,0,0,0,0,0,
				// 	0,0,0,0,0,0,0,0,
				// 	0,0,0,0,1,1,1,0,
				// 	0,0,0,0,1,1,0,0,
				// 	0,0,0,0,1,0,0,0,
				// 	0,0,0,0,1,1,1,0,
				// ]
				[
					0,0,0,0,0,0,0,0,
					0,0,0,0,0,0,0,0,
					0,0,0,0,0,0,0,0,
					0,0,0,0,0,0,0,0,
					0,0,0,0,0,0,0,0,
					0,0,0,0,0,3,1,0,
					0,0,0,0,1,1,0,1,
					0,0,0,0,0,1,1,0,
				]
			;

			byte[] BlackOrbSpecificFire =
				// [
				// 	1,1,1,0,0,0,0,0,
				// 	1,1,0,0,0,0,0,0,
				// 	1,0,0,0,0,0,0,0,
				// 	1,0,0,0,0,0,0,0,
				// 	0,0,0,0,0,0,0,0,
				// 	0,0,0,0,0,0,0,0,
				// 	0,0,0,0,0,0,0,0,
				// 	0,0,0,0,0,0,0,0,
				// ]
				[
					0,0,1,0,0,0,0,0,
					0,1,1,0,0,0,0,0,
					0,1,3,1,0,0,0,0,
					0,0,3,1,0,0,0,0,
					0,0,0,0,0,0,0,0,
					0,0,0,0,0,0,0,0,
					0,0,0,0,0,0,0,0,
					0,0,0,0,0,0,0,0,
				]
			;

			byte[] BlackOrbSpecificWater =
				// [
				// 	0,0,0,0,3,0,3,0,
				// 	0,0,0,0,3,0,3,0,
				// 	0,0,0,0,3,3,3,0,
				// 	0,0,0,0,3,3,3,0,
				// 	0,0,0,0,0,0,0,0,
				// 	0,0,0,0,0,0,0,0,
				// 	0,0,0,0,0,0,0,0,
				// 	0,0,0,0,0,0,0,0,
				// ]
				[
					0,0,0,0,0,0,3,0,
					0,0,0,0,0,2,3,2,
					0,0,0,0,0,3,3,3,
					0,0,0,0,0,1,3,1,
					0,0,0,0,0,0,0,0,
					0,0,0,0,0,0,0,0,
					0,0,0,0,0,0,0,0,
					0,0,0,0,0,0,0,0,
				]
			;

			byte[] BlackOrbSpecificAir =
				// [
				// 	0,0,0,0,0,0,0,0,
				// 	0,0,0,0,0,0,0,0,
				// 	0,0,0,0,0,0,0,0,
				// 	0,0,0,0,0,0,0,0,
				// 	0,3,0,0,0,0,0,0,
				// 	3,0,3,0,0,0,0,0,
				// 	3,3,3,0,0,0,0,0,
				// 	3,0,3,0,0,0,0,0,
				// ]
				[
					0,0,0,0,0,0,0,0,
					0,0,0,0,0,0,0,0,
					0,0,0,0,0,0,0,0,
					0,0,0,0,0,0,0,0,
					1,0,0,0,0,0,0,0,
					0,1,1,0,0,0,0,0,
					3,0,0,0,0,0,0,0,
					0,3,3,3,0,0,0,0,
				]
			;
			
			

			var assembly = System.Reflection.Assembly.GetExecutingAssembly();
			var trackerIconFile = assembly.GetManifestResourceNames().
				Single(str => str.EndsWith("tracker_icons.png"));
			var trackerIconStream = assembly.GetManifestResourceStream(trackerIconFile);

			Image<Rgba32> trackerIconImage = Image.Load<Rgba32>(trackerIconStream);

			Dictionary<TrackerIcon, byte[]> TrackerIcons = new();
			
			foreach (var icon in Enum.GetValues<TrackerIcon>())
			{
				int left = 8 * ((int)icon % 6);
				int top  = 8 * ((int)icon / 6);

				TrackerIcons[icon] = makeTile(trackerIconImage,top,left,MenuIndex);
			}

			if (flags.NoOverworld)
			{
				TrackerIcons[TrackerIcon.Bridge] = BlankTile;
				TrackerIcons[TrackerIcon.Ship] = BlankTile;
				TrackerIcons[TrackerIcon.Canoe] = TrackerIcons[TrackerIcon.Mark];
				TrackerIcons[TrackerIcon.Floater] = TrackerIcons[TrackerIcon.Sigil];
			}
			else if (flags.GameMode == GameModes.DeepDungeon)
			{
				TrackerIcons[TrackerIcon.Bridge] = BlankTile;
				TrackerIcons[TrackerIcon.Canal] = BlankTile;
				TrackerIcons[TrackerIcon.Sara] = BlankTile;
				TrackerIcons[TrackerIcon.King] = BlankTile;
				TrackerIcons[TrackerIcon.Bikke] = BlankTile;
				TrackerIcons[TrackerIcon.CrescentSage] = BlankTile;
				TrackerIcons[TrackerIcon.Sarda] = BlankTile;
				TrackerIcons[TrackerIcon.Robot] = BlankTile;
				TrackerIcons[TrackerIcon.ShopItem] = BlankTile;
			}
			else if ((bool)flags.AirBoat)
			{
				TrackerIcons[TrackerIcon.Floater] = TrackerIcons[TrackerIcon.AirBoat];
				TrackerIcons[TrackerIcon.Airship] = TrackerIcons[TrackerIcon.AirBoat];
			}

			if ((bool)flags.FightBahamut && !(flags.NoTristateSpoilers && unmodifiedFlags.FightBahamut is null))
			{
				TrackerIcons[TrackerIcon.Tail] = TrackerIcons[TrackerIcon.Bahamut];
			}
			else if ((bool)flags.NoTail && !(flags.NoTristateSpoilers && unmodifiedFlags.NoTail is null))
			{
				DarkenIconTile(TrackerIcons[TrackerIcon.Tail]);
			}

			if (((bool)flags.NoFloater && !flags.NoOverworld && !(flags.NoTristateSpoilers && unmodifiedFlags.NoFloater is null)) || flags.DesertOfDeath )
			{
				DarkenIconTile(TrackerIcons[TrackerIcon.Airship]);
			}

			if ((bool)flags.ShipDrydock && !(bool)flags.AirBoat && !(flags.NoTristateSpoilers && (unmodifiedFlags.ShipDrydock is null || unmodifiedFlags.AirBoat is null)))
			{
				DarkenIconTile(TrackerIcons[TrackerIcon.Ship]);
			}

			if (!flags.ShardHunt && flags.OrbsRequiredCount != 4 && (bool)flags.OrbsRequiredSpoilers)
			{
				if (flags.OrbsRequiredMode == OrbsRequiredMode.Any || flags.OrbsRequiredMode == OrbsRequiredMode.AnyAll)
				{
					switch (OrbShardGoal)
					{
						case 0: TrackerIcons[TrackerIcon.BlackOrb] = TrackerIcons[TrackerIcon.BlackOrb0]; break;
						case 1: TrackerIcons[TrackerIcon.BlackOrb] = TrackerIcons[TrackerIcon.BlackOrb1]; break;
						case 2: TrackerIcons[TrackerIcon.BlackOrb] = TrackerIcons[TrackerIcon.BlackOrb2]; break;
						case 3: TrackerIcons[TrackerIcon.BlackOrb] = TrackerIcons[TrackerIcon.BlackOrb3]; break;
						case 4: TrackerIcons[TrackerIcon.BlackOrb] = TrackerIcons[TrackerIcon.BlackOrb4]; break;
					}
				}
				else
				{
					List<byte[]> requiredOrbs = new();
					if (EarthOrbRequired) requiredOrbs.Add(BlackOrbSpecificEarth);
					if (FireOrbRequired) requiredOrbs.Add(BlackOrbSpecificFire);
					if (WaterOrbRequired) requiredOrbs.Add(BlackOrbSpecificWater);
					if (AirOrbRequired) requiredOrbs.Add(BlackOrbSpecificAir);
					foreach (byte[] overlay in requiredOrbs)
					{
						for (int i = 0; i < 64; i++)
						{
							BlackOrbSpecific[i] |= overlay[i];
						}
					}
					TrackerIcons[TrackerIcon.BlackOrb] = BlackOrbSpecific;
				}
			}
			
			for (int i = 0; i<31; i++)
			{	
				PutInBank(TrackerIconBank, i*0x10 + TrackerIconOffset, EncodeForPPU(TrackerIcons[(TrackerIcon)i]));
			}

			//PutInBank(TrackerIconBank, TrackerIconOffset + 0x200, TrackerIcons[TrackerIcon.Shard]);

			// TODO add icons for Sprint Shoes, Repel, probably over the old ghost at 0x8E00

			PutInBank(TrackerIconBank, TrackerCheckboxOffset, EncodeForPPU(TrackerIcons[TrackerIcon.EmptyCheckbox]));
			PutInBank(TrackerIconBank, TrackerCheckboxOffset + 0x10, EncodeForPPU(TrackerIcons[TrackerIcon.FilledCheckbox]));

			// add tiles for main menu tracker
			for (int i = 0; i<9; i++)
			{
				// Lockpicking, GoMode, BlackOrb
				PutInBank(
					TrackerIconBank, 
					i*0x10 + TrackerMainMenuIconOffset,
					EncodeForPPU(TrackerIcons[(TrackerIcon)(i + (i > 2 ? 45 : 40))]) // 40, 41, 42, 48-53
				);
				
			}
		}
	}
}
