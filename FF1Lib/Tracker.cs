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
		Sarah = 24,
		King = 25,
		Bikke = 26,
		CrescentSage = 27,
		Sarda = 28,
		Robot = 29,
		SprintShoes = 30,
		Repel = 31,
		Mark = 32,
		Sigil = 33,
		AirBoat = 34,
		Boulder = 35,

		EmptyCheckbox = 36,
		FilledCheckbox = 37,
		LockPicking = 38,
		Hints = 39,
		GoMode = 40
	}
	public partial class FF1Rom
	{
		void InGameTracker(Flags flags, Flags unmodifiedFlags)
		{
			if (!flags.ItemMenuTracker)
			{
				return;
			}
			WriteTrackerIcons(flags);

			// set the width of the "ITEM" box, which we'll use for the tracker:
			PutInBank(0x0E, 0xBAC0, 0x1E);

			// redirect "ITEM" box draw to our tracker
			PutInBank(0x0E, 0xB12D, Blob.FromHex("EAEA200097"));

			PutInBank(0x0E,0x9700,Blob.FromHex("A90720EFB8C63B20ABDCA9FFA2379D006ECA10FAA200AD0860F005A9019D006EE8AD0C60D005A9029D006EE8AD0060F005A9039D006EE8AD1260F005A9049D006EE8AD2B60F004A906D009AD0460F007A905D0009D006EE8A200A91C85108511A5548DF06D202EE1BD006EAC0220A4558C0620A4548C06208D0720E8E654C610D0E6E63B20ABDCA5548DF86DBD006EAC0220A4558C0620A4548C06208D0720E8E654C611D0E660"));
		}

		void WriteTrackerIcons(Flags flags)
		{
			const int TRACKER_ICON_BANK = 0x12;
			const int TRACKER_ICON_OFFSET = 0x8810;
			const int TRACKER_CHECKBOX_OFFSET = 0x8F40;

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

				TrackerIcons[icon] = EncodeForPPU(makeTile(trackerIconImage,top,left,TrackerIconIndex));
			}


			for (int i = 0; i<32; i++)
			{
				byte[] tile;
				if (i == (int)TrackerIcon.Canoe)
				{
					tile = flags.NoOverworld? TrackerIcons[TrackerIcon.Mark]  : TrackerIcons[TrackerIcon.Canoe];
				}
				else if (i == (int)TrackerIcon.Floater)
				{
					if (flags.NoOverworld)
					{ 
						tile = TrackerIcons[TrackerIcon.Sigil];
					}
					else
					{
						tile = (bool)flags.AirBoat? TrackerIcons[TrackerIcon.AirBoat] : TrackerIcons[TrackerIcon.Floater];
					}
				}
				else if (i == (int)TrackerIcon.Airship)
				{
					tile = (bool)flags.AirBoat? TrackerIcons[TrackerIcon.AirBoat] : TrackerIcons[TrackerIcon.Airship];
				}
				else
				{
					tile = TrackerIcons[(TrackerIcon)i];
				}
				PutInBank(TRACKER_ICON_BANK, i*0x10 + TRACKER_ICON_OFFSET, tile);
			}

			PutInBank(TRACKER_ICON_BANK, TRACKER_CHECKBOX_OFFSET, TrackerIcons[TrackerIcon.EmptyCheckbox]);
			PutInBank(TRACKER_ICON_BANK, TRACKER_CHECKBOX_OFFSET + 0x10, TrackerIcons[TrackerIcon.FilledCheckbox]);
		}
			
		

	}
}
