using RomUtilities;
using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FF1Lib
{
	public partial class FF1Rom
	{
		private void StatsTrackingHacks(Flags flags, Preferences preferences)
		{
			ExtraTrackingAndInitCode(flags, preferences);
			GameScreenTracking();
			StatsTrackingScreen();
		}

		private void ExtraTrackingAndInitCode(Flags flags, Preferences preferences)
		{
			// Expanded game init code, does several things:
			//	- Encounter table emu/hardware fix
			//	- track hard/soft resets
			//	- initialize tracking variables if no game is saved
			PutInBank(0x0F, 0x8000, Blob.FromHex("A9008D00208D012085FEA90885FF85FDA51BC901D00160A901851BA94DC5F9F008A9FF85F585F685F7182088C8B049A94DC5F918F013ADA36469018DA364ADA46469008DA464189010ADA56469018DA564ADA66469008DA664A9008DFD64A200187D00647D00657D00667D0067E8D0F149FF8DFD64189010A2A0A9009D00609D0064E8D0F7EEFB64ADFB648DFB60A9008D40038D410360"));
			Put(0x7C012, Blob.FromHex("A90F2003FE200080EAEAEAEAEAEAEAEA"));

			int hardresetbutton = preferences.QuickJoy2Reset ? 0x80 : 0x88;
			int softresetbutton = preferences.QuickJoy2Reset ? 0x40 : 0x48;

			// Move controller handling out of bank 1F
			// This bit of code is also altered to allow a hard reset using Up+A on controller 2
			PutInBank(0x0F, 0x8200, Blob.FromHex("20108220008360"));
			PutInBank(0x0F, 0x8210, Blob.FromHex($"A9018D1640A9008D1640A208AD16402903C9012620AD17402903C901261ECAD0EBA51EC9{hardresetbutton:X2}F008C9{softresetbutton:X2}F001604C2EFE20A8FE20A8FE20A8FEA2FF9AA900851E9500CAD0FBA6004C12C0"));
			PutInBank(0x0F, 0x8300, Blob.FromHex("A5202903F002A2038611A520290CF0058A090C8511A52045212511452185214520AA2910F00EA5202910F002E623A521491085218A2920F00EA5202920F002E622A521492085218A2940F00EA5202940F002E625A521494085218A2980F00EA5202980F002E624A5214980852160"));
			PutInBank(0x1F, 0xD7C2, CreateLongJumpTableEntry(0x0F, 0x8200));

			// Battles use 2 separate and independent controller handlers for a total of 3 (because why not), so we patch these to respond to Up+A also
			PutInBank(0x0F, 0x8580, Blob.FromHex($"A0018C1640888C1640A008AD16404AB0014A6EB368AD17402903C901261E88D0EAA51EC9{hardresetbutton:X2}F00BC9{softresetbutton:X2}F004ADB368604C2EFE20A8FE20A8FE20A8FEA2FF9AA900851E9500CAD0FBA6004C12C0"));
			PutInBank(0x1F, 0xD828, CreateLongJumpTableEntry(0x0F, 0x8580));
			// PutInBank(0x0B, 0x9A06, Blob.FromHex("4C28D8")); Included in bank 1B changes
			PutInBank(0x0C, 0x97CA, Blob.FromHex("2028D82029ABADB36860"));


			// Put LongJump routine 6 bytes after UpdateJoy used to be
			PutInBank(0x1F, 0xD7C8, Blob.FromHex("85E99885EA6885EB6885ECA001B1EB85EDC8B1EB85EEC8ADFC6085E8B1EB2003FEA9D748A9F548A5E9A4EA6CED000885E9A5E82003FEA5E92860"));
			// LongJump entries can start at 0xD806 and must stop before 0xD850 (at which point additional space will need to be freed to make room)

			// Patches for various tracking variables follow:
			// Pedometer + chests opened
			PutInBank(0x0F, 0x8100, Blob.FromHex("18A532D027A52D2901F006A550D01DF00398D018ADA06069018DA060ADA16069008DA160ADA26069008DA260A52F8530A9FF8518A200A000BD00622904F001C8E8D0F5988DB7606060"));
			Put(0x7D023, Blob.FromHex("A90F2003FE200081"));
			// Count number of battles + track battle screen
			PutInBank(0x0F, 0x8400, Blob.FromHex("18ADA76069018DA7609003EEA860A90885F220A8FE60"));
			PutInBank(0x1F, 0xD80C, CreateLongJumpTableEntry(0x0F, 0x8400));
			PutInBank(0x1F, 0xF28D, Blob.FromHex("200CD8"));
			// Ambushes / Strike First
			PutInBank(0x0F, 0x8420, Blob.FromHex("AD5668C90B9015C95A901F18ADAB6069018DAB609014EEAC6018900E18ADA96069018DA9609003EEAA60AC5668AE576860"));
			PutInBank(0x1F, 0xD806, CreateLongJumpTableEntry(0x0F, 0x8420));
			Put(0x313FB, Blob.FromHex("eaeaea2006D8"));
			// Runs
			PutInBank(0x0F, 0x8480, Blob.FromHex("AD5868F00E18ADAD6069018DAD609003EEAE60AD586860"));
			PutInBank(0x1F, 0xD81C, CreateLongJumpTableEntry(0x0F, 0x8480));
			Put(0x32418, Blob.FromHex("201CD8"));
			// Physical Damage
			PutInBank(0x0F, 0x84B0, Blob.FromHex("8E7D68AD8768F01DADB2606D82688DB260ADB3606D83688DB360ADB46069008DB46018901AADAF606D82688DAF60ADB0606D83688DB060ADB16069008DB160AE7D6860"));
			PutInBank(0x1F, 0xD822, CreateLongJumpTableEntry(0x0F, 0x84B0));
			Put(0x32968, Blob.FromHex("2022D8"));
			// Magic Damage
			PutInBank(0x0F, 0x8500, Blob.FromHex("AD8A6C2980F01CADB2606D58688DB260ADB3606D59688DB360ADB46069008DB460901AADAF606D58688DAF60ADB0606D59688DB060ADB16069008DB160A912A212A00160"));
			PutInBank(0x1F, 0xD83A, CreateLongJumpTableEntry(0x0F, 0x8500));
			PutInBank(0x0C, 0xB8ED, Blob.FromHex("203AD8eaeaea"));
			// Party Wipes

			// gotta use use different address when Save On Game Over is on.
			// Normally, a wipe increments a value in sram, which gets copied to
			// the corresponding place in unsram when game resets and loads values
			// from sram to unsram. In save on game over, it needs to do the opposite,
			// because it enters the save routine which copies from unsram to sram
			string PerishCountAddress = flags.SaveGameWhenGameOver? "B560" : "B564";
			PutInBank(0x0F, 0x85D0, Blob.FromHex($"EE{PerishCountAddress}A9008DFD64A200187D00647D00657D00667D0067E8D0F149FF8DFD64A952854B60"));
			PutInBank(0x1F, 0xD82E, CreateLongJumpTableEntry(0x0F, 0x85D0));
			//PutInBank(0x0B, 0x9AF5, Blob.FromHex("202ED8EAEA")); included in 1B changes
			// "Nothing Here"s
			PutInBank(0x0F, 0x8600, Blob.FromHex("A54429C2D005A545F00360A900EEB66060"));
			PutInBank(0x1F, 0xD834, CreateLongJumpTableEntry(0x0F, 0x8600));
			PutInBank(0x1F, 0xCBED, Blob.FromHex("4C34D860"));

			// Add select button handler on game start menu to change color
			PutInBank(0x0F, 0x861C, Blob.FromHex("A9008522203CC4A662A9488540ADFB60D003EEFB60A522F022EEFB60ADFB60C90D300EF007A9018DFB60D005A90F8DFB60A90085222029EBA90060A90160"));
			PutInBank(0x1F, 0xD840, CreateLongJumpTableEntry(0x0F, 0x861C));
			Put(0x3A1B5, Blob.FromHex("2040D8D0034C56A1EA"));
			// Move Most of LoadBorderPalette_Blue out of the way to do a dynamic version.
			PutInBank(0x0F, 0x8700, Blob.FromHex("988DCE038DEE03A90F8DCC03A9008DCD03A9308DCF0360"));

			// Move DrawCommandMenu out of Bank F so we can add no Escape to it
			PutInBank(0x0F, 0x8740, Blob.FromHex("A000A200B91BFA9D9E6AE8C01BD015AD916D2903F00EA9139D9E6AE8C8A9F79D9E6AE8C8E005D0052090F6A200C8C01ED0D260"));

			// Create a clone of IsOnBridge that checks the canal too.
			PutInBank(0x0F, 0x8780, Blob.FromHex("AD0860F014A512CD0960D00DA513CD0A60D006A90085451860A512CD0D60D00DA513CD0E60D006A900854518603860"));

			// BB Absorb fix.
			//PutInBank(0x0F, 0x8800, Blob.FromHex("A000B186C902F005C908F00160A018B186301BC8B1863016C8B1863011C8B186300CA026B1861869010AA0209186A01CB186301AC8B1863015C8B1863010C8B186300BA026B186186901A022918660"));

			// Copyright overhaul, see 0F_8960_DrawSeedAndFlags.asm
			PutInBank(0x0F, 0x8980, Blob.FromHex("A9238D0620A9208D0620A200BD00898D0720E8E060D0F560"));

			var drawinrows = preferences.OptOutSpeedHackMessages ? "01" : BattleBoxDrawInRows;
			var undrawrows = preferences.OptOutSpeedHackMessages ? "02" : BattleBoxUndrawRows;

			// Fast Battle Boxes
			PutInBank(0x0F, 0x8A00, Blob.FromHex("A940858AA922858BA91E8588A969858960"));
			PutInBank(0x0F, 0x8A20, Blob.FromHex($"A9{drawinrows}8DB96820A1F420E8F4A5881869208588A58969008589A58A186920858AA58B6900858BCEB968D0DE60"));

			// Fast Battle Boxes Undraw (Similar... yet different!)
			PutInBank(0x0F, 0x8A80, Blob.FromHex("A9A0858AA923858BA97E8588A96A858960"));
			PutInBank(0x0F, 0x8AA0, Blob.FromHex($"A9{undrawrows}8DB96820A1F420E8F4A58838E9208588A589E9008589A58A38E920858AA58BE900858BCEB968D0DE60"));

			// Softlock fix
			Put(0x7C956, Blob.FromHex("A90F2003FE4C008B"));
			PutInBank(0x0F, 0x8B00, Blob.FromHex("BAE030B01E8A8D1001A9F4AAA9FBA8BD0001990001CA88E010D0F4AD1001186907AA9AA52948A52A48A50D48A54848A549484C65C9"));

			// Adds MDEF to the Status screen
			MenuText.MenuStrings[(int)FF1Text.MenuString.StatusRightPanel] =
				FF1Text.TextToBytesStats(" DAMAGE   {stat}{damage}", delimiter: FF1Text.Delimiter.Segment) +
				FF1Text.TextToBytesStats(" HIT %    {stat}{hit%}", delimiter: FF1Text.Delimiter.Segment) +
				FF1Text.TextToBytesStats(" ABSORB   {stat}{absorb}", delimiter: FF1Text.Delimiter.Segment) +
				FF1Text.TextToBytesStats(" EVADE %  {stat}{luck}", delimiter: FF1Text.Delimiter.Segment) +
				FF1Text.TextToBytesStats(" MDEF     {stat}{mdef}", delimiter: FF1Text.Delimiter.Null);

			//Rewrite left status panel with one fewer space to match spacing with right panel
			MenuText.MenuStrings[(int)FF1Text.MenuString.StatusLeftPanel] =
				FF1Text.TextToBytesStats(" STR.   {stat}{str}", delimiter: FF1Text.Delimiter.Segment) +
				FF1Text.TextToBytesStats(" AGI.   {stat}{agi}", delimiter: FF1Text.Delimiter.Segment) +
				FF1Text.TextToBytesStats(" INT.   {stat}{int}", delimiter: FF1Text.Delimiter.Segment) +
				FF1Text.TextToBytesStats(" VIT.   {stat}{vit}", delimiter: FF1Text.Delimiter.Segment) +
				FF1Text.TextToBytesStats(" LUCK   {stat}{luck}", delimiter: FF1Text.Delimiter.Null);

			//Change PrintCharStat to support an MDEF stat code (0x50), see 0E_8D83_MDEFSupport.asm
			PutInBank(0x0E, 0x8D83, Blob.FromHex("C950D004E92BD06DC90D10086909D065EAEAEAEA"));

			//Key Items + Progressive Scaling
			if (flags.ProgressiveScaleMode == ProgressiveScaleMode.OrbProgressiveSlow || flags.ProgressiveScaleMode == ProgressiveScaleMode.OrbProgressiveMedium || flags.ProgressiveScaleMode == ProgressiveScaleMode.OrbProgressiveFast || flags.ProgressiveScaleMode == ProgressiveScaleMode.OrbProgressiveVFast)
			{
				if (flags.ShardHunt)
				{
					PutInBank(0x0F, 0x9000, Blob.FromHex("AD35608DB86060"));
				}
				else
				{
					PutInBank(0x0F, 0x9000, Blob.FromHex("A200AD3160F001E8AD3260F001E8AD3360F001E8AD3460F001E88EB86060"));
				}
			}
			else
			{
				PutInBank(0x0F, 0x9000, Blob.FromHex("A200AD2160F001E8AD2260F001E8AD2560F001E8AD2A60F001E8AD2B60F001E8AD2C60F001E8AD2E60F001E8AD3060F001E8AD0060F001E8AD1260F001E8AD0460F001E8AD0860F001E8AD0C60D001E8AD2360D007AD0A622902F001E8AD2460D007AD05622902F001E8AD2660D007AD08622902F001E8AD2760D007AD09622902F001E8AD2860D007AD0B622902F001E8AD2960D007AD14622901D001E8AD2D60D007AD0E622902F001E8AD2F60D007AD13622903F001E88EB86060"));
			}
			PutInBank(0x1F, 0xCFCB, CreateLongJumpTableEntry(0x0F, 0x9100));
			//Division routine
			PutInBank(0x0F, 0x90C0, Blob.FromHex("8A48A9008513A210261026112613A513C5129004E512851326102611CAD0EDA513851268AA60"));
			// Progressive scaling also writes to 0x9100 approaching 200 bytes, begin next Put at 0x9200.

			// Replace Overworld to Floor and Floor to Floor teleport code to JSR out to 0x9200 to set X / Y AND inroom from unused high bit of X.
			PutInBank(0x1F, 0xC1E2, Blob.FromHex("A9002003FEA545293FAABD00AC8510BD20AC8511BD40AC8548AABDC0AC8549A90F2003FE200092EAEAEAEAEAEA"));
			PutInBank(0x1F, 0xC968, Blob.FromHex("A90F2003FEA645BD00B08510BD00B18511BD00B28548200092A9002003FEA548AABDC0AC8549"));
			PutInBank(0x0F, 0x9200, Blob.FromHex("A200A5100A9002A2814A38E907293F8529A5110A9002860D4A38E907293F852A60"));

			// Critical hit display for number of hits
			PutInBank(0x0F, 0x9280, FF1Text.TextToBytes("Critical hit!!", false));
			PutInBank(0x0F, 0x9290, FF1Text.TextToBytes(" Critical hits!", false));
			PutInBank(0x0F, 0x92A0, Blob.FromHex("AD6B68C901F01EA2019D3A6BA9118D3A6BA900E89D3A6BA0FFC8E8B990929D3A6BD0F6F00EA2FFA0FFC8E8B980929D3A6BD0F6A23AA06BA904201CF7EEF86A60"));

			// Enable 3 palettes in battle
			PutInBank(0x1F, 0xFDF1, CreateLongJumpTableEntry(0x0F, 0x9380));
			PutInBank(0x0F, 0x9380, Blob.FromHex("ADD16A2910F00BA020B9336D99866B88D0F7ADD16A290F8DD16A20A1F4AD0220A9028D1440A93F8D0620A9008D0620A000B9876B8D0720C8C020D0F5A93F8D0620A9008D06208D06208D062060"));


			// Move ClearZeroPage out of bank, also fixes noise channel battle bug
			// See 1C_A4F0_ClearZeroPageAndMore.asm
			PutInBank(0x1F, 0xC454, Blob.FromHex("A9A448A9EF48A91C4C03FEEAEAEAEAEA"));
			PutInBank(0x1C, 0xA4F0, Blob.FromHex("A2EFA9009500CAD0FBA91B05F485F4A20FA9009D976DCA10FAA91F4C03FE"));
			if (flags.NoOverworld)
			{
				//Same ClearZeroPage routine but with an RTS at the end instead of SwapPRG
				PutInBank(0x1E, 0xA040, Blob.FromHex("A2EFA9009500CAD0FBA91B05F485F4A20FA9009D976DCA10FA60"));
			}
}

		public void GameScreenTracking()
		{
			/*
			 * Track the various screen at $F2 on the zeropage for easy reference.
			 * Intro Story  : $00
			 * Title Screen : $01
			 * Party Gen    : $02
			 * Overworld    : $03
			 * Standard Map : $04 / Read $48 for current map
			 * Shop         : $05 / Read $66 for shop type
			 * Main Menu	: $06
			 * Lineup Menu	: $07
			 * Battle       : $08
			 * Bridge Scene : $09
			 * Ending Scence: $0A
			 */

			// Track Party Gen
			// Included with party gen screen in Bank1E()

			// Track overworld
			PutInBank(0x1E, 0xBB00, GetFromBank(0x1F, 0xC6FD, 0x21) + Blob.FromHex("A90385F260"));
			PutInBank(0x1F, 0xC6FD, Blob.FromHex("A91E2003FE2000BB4C1EC7"));

			// Track standard map
			PutInBank(0x1E, 0xBB30, GetFromBank(0x1F, 0xCF55, 0x1B) + Blob.FromHex("A90485F260"));
			PutInBank(0x1F, 0xCF55, Blob.FromHex("A91E2003FE2030BB4C70CF"));

			// Track Battle
			// Inluded with battle count tracking in ExtraTrackingAndInitCode()

			// Track shop, the subtype can be read from $66
			PutInBank(0x1E, 0xBB50, GetFromBank(0x0E, 0xA330, 0x0C) + Blob.FromHex("A90585F2") + Blob.FromHex("A9A348A93B48A90E4C03FE"));
			PutInBank(0x0E, 0xA330, Blob.FromHex("A9BB48A94F48A91E4C03FE"));

			// Track Main menu
			PutInBank(0x1E, 0xBB70, GetFromBank(0x0E, 0xADB3, 0x0C) + Blob.FromHex("A90685F2") + Blob.FromHex("A9AD48A9BE48A90E4C03FE"));
			PutInBank(0x0E, 0xADB3, Blob.FromHex("A9BB48A96F48A91E4C03FE"));

			// Track Lineup menu
			PutInBank(0x1E, 0xBB90, GetFromBank(0x0E, 0x9915, 0x0C) + Blob.FromHex("A90785F2") + Blob.FromHex("A99948A92048A90E4C03FE"));
			PutInBank(0x0E, 0x9915, Blob.FromHex("A9BB48A98F48A91E4C03FE"));

			// Track Ending Scene
			// changes here fix bug where the first ending story wasn't properly loaded
			// instead of getting 9 bytes from B80D, we only get 4. The remaining 5 set the page in the story to start on
			// but since this is going in a new bank, it points to a random spot in bank 0x1E instead of in 0x0D
			// the proper value is loaded in Credits.cs
			
			PutInBank(0x1E, 0xBBB0, GetFromBank(0x0D, 0xB803, 0x04) + GetFromBank(0x0D, 0xB80D, 0x04) + Blob.FromHex("A90A85F2") + Blob.FromHex("A9B848A91548A90D4C03FE"));
			PutInBank(0x0D, 0xB803, GetFromBank(0x0D, 0xB807, 0x06));
			PutInBank(0x0D, 0xB809, Blob.FromHex("A9BB48A9AF48A91E4C03FE"));


			// Track Bridge Scene - Jump to ending scene because of a lack of space
			PutInBank(0x1E, 0xBBD0, GetFromBank(0x0D, 0xB84D, 0x0A) + Blob.FromHex("A90985F2") + Blob.FromHex("A9B848A95648A90D4C03FE"));
			PutInBank(0x0D, 0xB84D, Blob.FromHex("A9BB48A9CF484C0FB8"));

			// Track Title Screen
			PutInBank(0x1E, 0xBBF0, GetFromBank(0x0E, 0xA159, 0x0E) + Blob.FromHex("A90185F2") + Blob.FromHex("A9A148A96648A90E4C03FE"));
			PutInBank(0x0E, 0xA159, Blob.FromHex("A9BB48A9EF48A91E4C03FE"));
		}

		public void StatsTrackingScreen()
		{
			// Get the 3 stats pages (6 bytes) created in Credits.cs;
			//string statsPageAddresses = GetFromBank(0x0D, 0xA804, 0x06).ToHex();
			// with update to bridges.txt option in resource packs,
			// the bank address is no longer static.
			// StatsPageAddress is calculated in Credits.cs and used here:
			string statsPageAddresses = GetFromBank(0x0D, StatsPageAddress, 0x06).ToHex();
			

			// Give access to the tracked game stats from the main menu, by pressing Select; see 1E_BA00_StatsMenu.asm
			PutInBank(0x1E, 0xBA00, Blob.FromHex($"{statsPageAddresses}010B0C11020D15A522F0034C27BAA524F003A90160A525F004A9003860A90018602040BA203CC420D5BA201A856868A9AD48A9CC48A90E4C03FEA9008D0120A900853720C1BAAD1C608D006EAD1D608D016EAD1E608D026EA9018538A91E853CA902853AA2008614BC06BA8439843BBD07BA853D2063E0A614E8E8E00490E7A2008614BD00BA853EBD01BA853F8A4AAABC0ABA843BA91E8558A90D85572036DEA614E8E8E00690D9AD006E8D1C60AD016E8D1D60AD026E8D1E6060A91E48A9FE48A90648A99C48A90148A90E4C03FEA91E48A9FE48A90648A9B748A97F48A90E4C03FE"));

			PutInBank(0x1F, 0xD846, CreateLongJumpTableEntry(0x1E, 0xBA0D)); // Longjump from MainMenuLoop
			PutInBank(0x0E, 0xB665, Blob.FromHex("209AE1A9008522EAEAEAEAEAEA")); // Change MenuFrame to reset Select button
			PutInBank(0x0E, 0xADF4, Blob.FromHex("2046D8D015B007EA"));           // Change MainMenuLoop to check for SelectButton

			// PutInBank(0x0D, 0xB83E, Blob.FromHex("2012D8"));
			// PutInBank(0x1F, 0xD812, CreateLongJumpTableEntry(0x1E, 0xB100)); // LongJump from Ending Credits
			// PutInBank(0x1E, 0xB100, Blob.FromHex("A900852485252000FEA91E85572089C620C2D7A5240525F0034C1FB14C06B1A9008D01202006E9A20BA90085372040B0203CC420D5B02000FEA91E85572089C64C36B1"));
		}
	}
}
