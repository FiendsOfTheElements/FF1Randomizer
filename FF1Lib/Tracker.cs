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

		Shard = 42,
		LockPicking = 43,
		Hints = 44,
		GoMode = 45
	}
	public partial class FF1Rom
	{
		const int TRACKER_ICON_BANK = 0x12;
		const int TRACKER_ICON_OFFSET = 0x8810;
		const int TRACKER_CHECKBOX_OFFSET = 0x8F40;
			
		const int TRACKER_ORB_OFFSET = 0x8E70;


		public void InGameTracker(Flags flags, Flags unmodifiedFlags)
		{
			if (!flags.Tracker)
			{
				return;
			}
			

			byte KingReq  = (bool)flags.EarlyKing  ? (byte)0x00 : (byte)ObjectId.Princess1;
			byte SageReq  = (bool)flags.EarlySage  ? (byte)0x00 : (byte)Item.EarthOrb;
			byte SardaReq = (bool)flags.EarlySarda ? (byte)0x00 : (byte)ObjectId.Vampire;
			byte BahamutReq = (bool)flags.FightBahamut && (bool)flags.NoTail ? (byte)0x00 : (byte)Item.Tail;


			byte[] NPCReqs = [KingReq,SageReq,SardaReq,BahamutReq];


			// set the width of the "ITEM" box, which we'll use for the tracker:
			PutInBank(0x0E, 0xBABE, 0x00); // 0xBABE!!
			PutInBank(0x0E, 0xBAC0, 0x20);

			// redirect "ITEM" box draw to our tracker
			PutInBank(0x0E, 0xB12E, 0x07);
			PutInBank(0x0E, 0xB91D,Blob.FromHex("20EFB8C63B4CD0A0EAEA"));
			PutInBank(0x0E, 0xA0D0, Blob.FromHex("20ABDCA9A148A90348A91B4C03FE"));

			

			PutInBank(0x1B, 0xA100, NPCReqs);

			PutInBank(0x1B,0xA104,Blob.FromHex("A9FFA23B9D006ECA10FAA200AD0860F005A9019D006EE8AD0C60D005A9029D006EE8AD0060F005A9039D006EE8AD1260F005A9049D006EE8AD0460F004A905D007AD2B60F005A9069D006EE8AD2260F015A9079D006EA00720F7A29004A975D002A9749D1E6EE8A908851EA00AAD23602002A3A909851EA90A851FA00520F7A22A2901851DA006AD2460201DA3A90B851EA009AD27602002A3A90C851EA008AD26602002A3A01420FDA2B004A975D007AD2960F00AA9749D1E6EA90D9D006EE8A00E20F7A29004A975D00CAC03A1F005B92060F00AA9749D1E6EA90E9D006EE8A90F851EA910851FA01320FDA22A2901851DAD2F60201DA3A911851EA912851FA00B20F7A22A2901851DA00FAD2860201DA3AD2A60F015A9139D006EA01620FDA2B004A975D002A9749D1E6EE8AD2160F015A9149D006EA01720FDA2B004A975D002A9749D1E6EE8A015AD25602048A3A016AD30602048A3A017AD2C602048A3A018AD2E602048A3E8E8E8A919851EA01220FDA22A29012052A3A91A851EAC00A1D004A901D00620F7A22A2901A0012052A3A91B851EA03F20FDA22A2901A0042052A3A91C851EAC01A1D004A901D003B92060A0152052A3A91D851EAC02A1D004A901D00620F7A22A2901A00D2052A3A91E851EA901A0112052A3A91F9D006EA0FF20F7A29004A975D002A9749D1E6E4C6BA3B900624A4A60B900624A60851020F7A29004A975D006A510F00AA9749D1E6EA51E9D006EE860851020F7A29009A51F9D006EA975D016A51DF007A51F9D006ED009A510F00AA51E9D006EA9749D1E6EE860C900F004989D006EE860C900F013A51E9D006E20F7A29004A975D002A9749D1E6EE860A9008D0120A200A91E8510208BA3E63B20ABDCA91E8510AA208BA3A90E4C03FEBD006EAC0220A4558C0620A4548C06208D0720E8E654C610D0E660"));
		}



		public void AddOrbLetterOverlays()
		{
			byte[][] Overlays = 
			[
				//Fire
				[
					4,4,4,4,4,4,4,4,
					4,4,4,0,0,0,0,0,
					4,4,4,0,1,1,1,0,
					4,4,4,0,1,0,0,0,
					4,4,4,0,1,1,0,4,
					4,4,4,0,1,0,4,4,
					4,4,4,0,1,0,4,4,
					4,4,4,0,0,0,4,4
				],

				//Water
				[
					4,4,4,4,4,4,4,4,
					4,0,0,0,4,0,0,0,
					4,0,1,0,4,0,1,0,
					4,0,1,0,0,0,1,0,
					4,0,1,0,1,0,1,0,
					4,0,1,1,1,1,1,0,
					4,0,0,1,0,1,0,0,
					4,4,0,0,0,0,0,4	
				],

				//Air
				[
					4,4,4,4,4,4,4,4,
					4,4,4,4,0,0,0,4,
					4,4,4,0,0,1,0,0,
					4,4,4,0,1,0,1,0,
					4,4,4,0,1,1,1,0,
					4,4,4,0,1,0,1,0,
					4,4,4,0,1,0,1,0,
					4,4,4,0,0,0,0,0	
				],

				//Earth
				[
					4,4,4,4,4,4,4,4,
					4,4,4,0,0,0,0,0,
					4,4,4,0,1,1,1,0,
					4,4,4,0,1,0,0,0,
					4,4,4,0,1,1,0,4,
					4,4,4,0,1,0,0,0,
					4,4,4,0,1,1,1,0,
					4,4,4,0,0,0,0,0	
				]
			];

			Console.WriteLine("Doing Orb Overlays");
			for (int i = 0; i < 4; i++)
			{
				int ThisOffset = TRACKER_ORB_OFFSET+i*0x40;
				byte[] OrbTile = DecodePPU(GetFromBank(TRACKER_ICON_BANK, ThisOffset,0x10));
				for (int j = 0; j < 64; j++)
				{
					byte OrbByte = OrbTile[j];
					byte OverlayByte = Overlays[i][j];
					OrbTile[j] = OverlayByte == 4 ? OrbByte : OverlayByte;
				}
				PutInBank(TRACKER_ICON_BANK, ThisOffset,EncodeForPPU(OrbTile));
			}
		}

		public void AddTrackerIcons(Flags flags)
		{
			
			byte[] BlankTile = EncodeForPPU(
				[
					3,3,3,3,3,3,3,3,
					3,3,3,3,3,3,3,3,
					3,3,3,3,3,3,3,3,
					3,3,3,3,3,3,3,3,
					3,3,3,3,3,3,3,3,
					3,3,3,3,3,3,3,3,
					3,3,3,3,3,3,3,3,
					3,3,3,3,3,3,3,3
				]
			);
			
			

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

				TrackerIcons[icon] = EncodeForPPU(makeTile(trackerIconImage,top,left,MenuIndex));
			}

			if (flags.NoOverworld)
			{
				TrackerIcons[TrackerIcon.Bridge] = BlankTile;
				TrackerIcons[TrackerIcon.Ship] = BlankTile;
				TrackerIcons[TrackerIcon.Canoe] = TrackerIcons[TrackerIcon.Mark];
				TrackerIcons[TrackerIcon.Floater] = TrackerIcons[TrackerIcon.Sigil];
			}
			else if ((bool)flags.AirBoat)
			{
				TrackerIcons[TrackerIcon.Floater] = TrackerIcons[TrackerIcon.AirBoat];
				TrackerIcons[TrackerIcon.Airship] = TrackerIcons[TrackerIcon.AirBoat];
			}
			if ((bool)flags.FightBahamut)
			{
				TrackerIcons[TrackerIcon.Tail] = TrackerIcons[TrackerIcon.Bahamut];
			}
			
			for (int i = 0; i<31; i++)
			{	
				PutInBank(TRACKER_ICON_BANK, i*0x10 + TRACKER_ICON_OFFSET, TrackerIcons[(TrackerIcon)i]);
			}

			PutInBank(TRACKER_ICON_BANK, TRACKER_ICON_OFFSET + 0x200, TrackerIcons[TrackerIcon.Shard]);

			// TODO add icons for Sprint Shoes, Repel

			PutInBank(TRACKER_ICON_BANK, TRACKER_CHECKBOX_OFFSET, TrackerIcons[TrackerIcon.EmptyCheckbox]);
			PutInBank(TRACKER_ICON_BANK, TRACKER_CHECKBOX_OFFSET + 0x10, TrackerIcons[TrackerIcon.FilledCheckbox]);
		}
	}
}
