using RomUtilities;
using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FF1Lib
{
	public enum OwTiles : byte
	{
		// Reference OverworldTiles.cs in the procGen if you need a tile not yet listed here, e.g. more Docks
		GrassTile = 0x00,
		OceanTile = 0x17,

		MarshTile = 0x55,
		MarshTopLeft = 0x62,
		MarshTopRight = 0x63,
		MarshBottomLeft = 0x72,
		MarshBottomRight = 0x73,

		MountainTopLeft = 0x10,
		MountainTopMid = 0x11,
		MountainTopRight = 0x12,
		MountainMidLeft = 0x20,
		MountainMid = 0x21,
		MountainMidRight = 0x22,
		MountainBottomLeft = 0x30,
		MountainBottomMid = 0x31,
		MountainBottomRight = 0x33,

		RiverTile = 0x44,
		RiverTopLeft = 0x40,
		RiverTopRight = 0x41,
		RiverBottomLeft = 0x50,
		RiverBottomRight = 0x51,

		ForestTopLeft = 0x03,
		ForestTopMid = 0x04,
		ForestTopRight = 0x05,
		ForestMidLeft = 0x13,
		ForestMid = 0x14,
		ForestMidRight = 0x15,
		ForestBottomLeft = 0x23,
		ForestBottomMid = 0x24,
		ForestBottomRight = 0x25,

		DockBottomMid = 0x78,
		DockRightMid = 0x1F,
		DockLeftMid = 0x0F,

	//public const byte Ocean = 0x17,
	// These are the tiny bits of jaggedness at the edge of a grass tile to make it look nice next to Ocean
		CoastLeft = 0x16,
		CoastRight = 0x18,
		CoastTop = 0x07,
		CoastBottom = 0x27,

	// The directions here refer to where the grass-side is, so "CoastTopLeft" is placed on the bottom-right
		CoastTopLeft = 0x06,
		CoastBottomLeft = 0x26,
		CoastTopRight = 0x08,
		CoastBottomRight = 0x28,

	// The special grassy effect around e.g. Elfland or Gaia
		GrassyMid = 0x54,
		GrassTopLeft = 0x60,
		GrassTopRight = 0x61,
		GrassBottomLeft = 0x70,
		GrassBottomRight = 0x71,

		DesertMid = 0x45,
		DesertTopLeft = 0x42,
		DesertTopRight = 0x43,
		DesertBottomLeft = 0x52,
		DesertBottomRight = 0x53,
	}

	public partial class OverworldMap
	{
		public static List<MapEdit> OnracDock =
			new List<MapEdit>
			{
				new MapEdit{X = 50, Y = 78, Tile = (byte)OwTiles.ForestBottomRight},
				new MapEdit{X = 51, Y = 78, Tile = (byte)OwTiles.DockBottomMid},
				new MapEdit{X = 52, Y = 78, Tile = (byte)OwTiles.DockBottomMid},
				new MapEdit{X = 51, Y = 77, Tile = (byte)OwTiles.ForestBottomMid},
				new MapEdit{X = 52, Y = 77, Tile = (byte)OwTiles.ForestBottomMid},
				new MapEdit{X = 51, Y = 79, Tile = (byte)OwTiles.OceanTile},
				new MapEdit{X = 52, Y = 79, Tile = (byte)OwTiles.OceanTile}
			};
		public static List<MapEdit> MirageDock =
			new List<MapEdit>
			{
				new MapEdit{X = 208, Y = 90, Tile = (byte)OwTiles.DockBottomMid},
				new MapEdit{X = 209, Y = 90, Tile = (byte)OwTiles.DockBottomMid},
				new MapEdit{X = 208, Y = 91, Tile = (byte)OwTiles.OceanTile},
				new MapEdit{X = 209, Y = 91, Tile = (byte)OwTiles.OceanTile}
			};
		public static List<MapEdit> AirshipDock =
			new List<MapEdit>
			{
				new MapEdit{X = 216, Y = 244, Tile = (byte)OwTiles.DockBottomMid},
				new MapEdit{X = 217, Y = 244, Tile = (byte)OwTiles.DockBottomMid},
				new MapEdit{X = 216, Y = 245, Tile = (byte)OwTiles.OceanTile},
				new MapEdit{X = 217, Y = 245, Tile = (byte)OwTiles.OceanTile}
			};
		public static List<MapEdit> ConeriaToDwarves =
			new List<MapEdit>
			{
				new MapEdit{X = 124, Y = 138, Tile = (byte)OwTiles.MountainBottomLeft},
				new MapEdit{X = 124, Y = 139, Tile = (byte)OwTiles.GrassTile},
				new MapEdit{X = 125, Y = 139, Tile = (byte)OwTiles.MountainBottomLeft},
				new MapEdit{X = 125, Y = 140, Tile = (byte)OwTiles.GrassTile},
				new MapEdit{X = 126, Y = 140, Tile = (byte)OwTiles.MountainBottomLeft},
				new MapEdit{X = 127, Y = 140, Tile = (byte)OwTiles.MountainBottomMid},
				new MapEdit{X = 128, Y = 140, Tile = (byte)OwTiles.MountainBottomMid},
				new MapEdit{X = 129, Y = 140, Tile = (byte)OwTiles.MountainBottomMid},
				new MapEdit{X = 126, Y = 141, Tile = (byte)OwTiles.GrassTile},
				new MapEdit{X = 127, Y = 141, Tile = (byte)OwTiles.GrassTile},
				new MapEdit{X = 128, Y = 141, Tile = (byte)OwTiles.GrassTile},
				new MapEdit{X = 129, Y = 141, Tile = (byte)OwTiles.GrassTile},
				new MapEdit{X = 130, Y = 141, Tile = (byte)OwTiles.MountainBottomLeft}
			};
		public static List<MapEdit> VolcanoIceRiver =
			new List<MapEdit>
			{
				new MapEdit{X = 209, Y = 189, Tile = (byte)OwTiles.MountainBottomRight},
				new MapEdit{X = 210, Y = 189, Tile = (byte)OwTiles.GrassTile},
				new MapEdit{X = 208, Y = 190, Tile = (byte)OwTiles.RiverTile},
				new MapEdit{X = 209, Y = 190, Tile = (byte)OwTiles.RiverTile},
				new MapEdit{X = 210, Y = 190, Tile = (byte)OwTiles.RiverTile},
				new MapEdit{X = 211, Y = 190, Tile = (byte)OwTiles.RiverTile},
				new MapEdit{X = 209, Y = 191, Tile = (byte)OwTiles.MountainTopLeft},
				new MapEdit{X = 210, Y = 191, Tile = (byte)OwTiles.MountainTopMid},
				new MapEdit{X = 211, Y = 191, Tile = (byte)OwTiles.MountainTopMid}
			};
		public static List<MapEdit> CanalSoftLockMountain =
			new List<MapEdit>
			{
				new MapEdit{X = 101, Y = 161, Tile = (byte)OwTiles.MountainTopLeft},
				new MapEdit{X = 102, Y = 161, Tile = (byte)OwTiles.MountainTopMid},
				new MapEdit{X = 103, Y = 161, Tile = (byte)OwTiles.MountainMid},
				new MapEdit{X = 101, Y = 162, Tile = (byte)OwTiles.MountainBottomLeft},
				new MapEdit{X = 102, Y = 162, Tile = (byte)OwTiles.MountainBottomMid},
				new MapEdit{X = 103, Y = 162, Tile = (byte)OwTiles.MountainBottomRight}
			};
		public static List<MapEdit> DwarvesNorthwestGrass =
			new List<MapEdit>
			{
				new MapEdit{X = 104, Y = 171, Tile = (byte)OwTiles.GrassTile},
				new MapEdit{X = 105, Y = 171, Tile = (byte)OwTiles.GrassTile},
				new MapEdit{X = 106, Y = 171, Tile = (byte)OwTiles.CoastLeft}
			};
		public static List<MapEdit> BahamutCardiaDock =
			new List<MapEdit>
			{
				new MapEdit{X = 0x5f, Y = 0x33, Tile = (byte)OwTiles.ForestBottomRight},
				new MapEdit{X = 0x5f, Y = 0x34, Tile = (byte)OwTiles.GrassTile},
				new MapEdit{X = 0x60, Y = 0x34, Tile = (byte)OwTiles.GrassTile},
				new MapEdit{X = 0x61, Y = 0x34, Tile = (byte)OwTiles.GrassTile},
				new MapEdit{X = 0x62, Y = 0x34, Tile = (byte)OwTiles.GrassTile},
				new MapEdit{X = 0x60, Y = 0x35, Tile = (byte)OwTiles.GrassTile},
				new MapEdit{X = 0x61, Y = 0x35, Tile = (byte)OwTiles.DockBottomMid},
				new MapEdit{X = 0x62, Y = 0x35, Tile = (byte)OwTiles.DockBottomMid},
				new MapEdit{X = 0x63, Y = 0x35, Tile = (byte)OwTiles.GrassTile},
			};
		public static List<MapEdit> LefeinRiverDock =
			new List<MapEdit>
			{
				new MapEdit{X = 0xE0, Y = 0x3A, Tile = (byte)OwTiles.RiverTile},
				new MapEdit{X = 0xE0, Y = 0x3B, Tile = (byte)OwTiles.RiverTile},
				new MapEdit{X = 0xE0, Y = 0x3C, Tile = (byte)OwTiles.RiverBottomLeft},
				new MapEdit{X = 0xE1, Y = 0x3C, Tile = (byte)OwTiles.RiverTopRight},
				new MapEdit{X = 0xE1, Y = 0x3D, Tile = (byte)OwTiles.RiverTile},
				new MapEdit{X = 0xE1, Y = 0x3E, Tile = (byte)OwTiles.RiverTile},
				new MapEdit{X = 0xDF, Y = 0x3B, Tile = (byte)OwTiles.ForestTopRight},
				new MapEdit{X = 0xDF, Y = 0x3C, Tile = (byte)OwTiles.ForestMidRight},
				new MapEdit{X = 0xE0, Y = 0x3D, Tile = (byte)OwTiles.ForestTopRight},
				new MapEdit{X = 0xE0, Y = 0x3E, Tile = (byte)OwTiles.ForestBottomRight},
				new MapEdit{X = 0xE1, Y = 0x3B, Tile = (byte)OwTiles.ForestBottomLeft},
				new MapEdit{X = 0xE2, Y = 0x3C, Tile = (byte)OwTiles.ForestMidLeft},
				new MapEdit{X = 0xE2, Y = 0x3D, Tile = (byte)OwTiles.ForestMidLeft},
				new MapEdit{X = 0xE2, Y = 0x3E, Tile = (byte)OwTiles.ForestBottomLeft},
			};
		public static List<MapEdit> BridgeToLefein =
			new List<MapEdit>
			{
					//Top Lefein Side
					new MapEdit{X = 228, Y = 120, Tile = (byte)OwTiles.CoastRight},
					new MapEdit{X = 229, Y = 120, Tile = (byte)OwTiles.ForestMidLeft},
					new MapEdit{X = 230, Y = 120, Tile = (byte)OwTiles.ForestMidRight},
					new MapEdit{X = 231, Y = 120, Tile = (byte)OwTiles.CoastLeft},
					new MapEdit{X = 228, Y = 121, Tile = (byte)OwTiles.CoastRight},
					new MapEdit{X = 229, Y = 121, Tile = (byte)OwTiles.ForestBottomLeft},
					new MapEdit{X = 230, Y = 121, Tile = (byte)OwTiles.ForestBottomRight},
					new MapEdit{X = 231, Y = 121, Tile = (byte)OwTiles.CoastLeft},
					new MapEdit{X = 229, Y = 122, Tile = (byte)OwTiles.CoastTopRight},
					new MapEdit{X = 230, Y = 122, Tile = (byte)OwTiles.GrassTile},
					new MapEdit{X = 231, Y = 122, Tile = (byte)OwTiles.CoastLeft},
					// Bottom Pravoka side
					new MapEdit{X = 229, Y = 124, Tile = (byte)OwTiles.CoastRight},
					new MapEdit{X = 230, Y = 124, Tile = (byte)OwTiles.GrassTile},
					new MapEdit{X = 231, Y = 124, Tile = (byte)OwTiles.CoastBottomLeft},
					new MapEdit{X = 229, Y = 125, Tile = (byte)OwTiles.CoastRight},
					new MapEdit{X = 230, Y = 125, Tile = (byte)OwTiles.GrassTile},
					new MapEdit{X = 231, Y = 125, Tile = (byte)OwTiles.GrassTile},
					new MapEdit{X = 232, Y = 125, Tile = (byte)OwTiles.CoastLeft},
					new MapEdit{X = 229, Y = 126, Tile = (byte)OwTiles.CoastRight},
					new MapEdit{X = 230, Y = 126, Tile = (byte)OwTiles.GrassTile},
					new MapEdit{X = 231, Y = 126, Tile = (byte)OwTiles.GrassTile},
					new MapEdit{X = 232, Y = 126, Tile = (byte)OwTiles.CoastLeft},
					// Landbridge above Coneria
					new MapEdit{X = 150, Y = 151, Tile = (byte)OwTiles.CoastRight},
					new MapEdit{X = 150, Y = 152, Tile = (byte)OwTiles.CoastBottomRight},
					new MapEdit{X = 151, Y = 152, Tile = (byte)OwTiles.GrassTile},
					new MapEdit{X = 152, Y = 152, Tile = (byte)OwTiles.GrassTile},
					new MapEdit{X = 153, Y = 152, Tile = (byte)OwTiles.GrassTile},
					new MapEdit{X = 154, Y = 152, Tile = (byte)OwTiles.CoastTopLeft},
					// Delete Matoya Dock
					new MapEdit{X = 156, Y = 141, Tile = (byte)OwTiles.GrassTile},
					new MapEdit{X = 157, Y = 141, Tile = (byte)OwTiles.GrassTile},
					new MapEdit{X = 158, Y = 141, Tile = (byte)OwTiles.GrassTile},
					new MapEdit{X = 159, Y = 141, Tile = (byte)OwTiles.GrassTile},
					new MapEdit{X = 156, Y = 142, Tile = (byte)OwTiles.CoastTopRight},
					new MapEdit{X = 157, Y = 142, Tile = (byte)OwTiles.GrassTile},
					new MapEdit{X = 158, Y = 142, Tile = (byte)OwTiles.GrassTile},
					new MapEdit{X = 159, Y = 142, Tile = (byte)OwTiles.GrassTile},
			};
		public static List<MapEdit> HighwayToOrdeals =
		new List<MapEdit>
		{
					// Mirage to Ordeals
					new MapEdit{X = 186, Y = 49, Tile = (byte)OwTiles.MountainBottomRight},
					new MapEdit{X = 187, Y = 49, Tile = (byte)OwTiles.DesertTopLeft},
					new MapEdit{X = 188, Y = 49, Tile = (byte)OwTiles.DesertTopRight},
					new MapEdit{X = 189, Y = 49, Tile = (byte)OwTiles.MountainBottomLeft},

					new MapEdit{X = 186, Y = 48, Tile = (byte)OwTiles.MountainTopRight},
					new MapEdit{X = 187, Y = 48, Tile = (byte)OwTiles.GrassBottomLeft},
					new MapEdit{X = 188, Y = 48, Tile = (byte)OwTiles.GrassBottomRight},
					new MapEdit{X = 189, Y = 48, Tile = (byte)OwTiles.MountainTopLeft},

					new MapEdit{X = 185, Y = 47, Tile = (byte)OwTiles.MountainTopRight},
					new MapEdit{X = 186, Y = 47, Tile = (byte)OwTiles.GrassBottomLeft},
					new MapEdit{X = 187, Y = 47, Tile = (byte)OwTiles.GrassyMid},
					new MapEdit{X = 188, Y = 47, Tile = (byte)OwTiles.GrassyMid},
					new MapEdit{X = 189, Y = 47, Tile = (byte)OwTiles.GrassTopRight},
					new MapEdit{X = 190, Y = 47, Tile = (byte)OwTiles.MountainTopLeft},

					//Lefein to Mirage
					new MapEdit{X = 209, Y = 50, Tile = (byte)OwTiles.DesertMid},
					new MapEdit{X = 209, Y = 51, Tile = (byte)OwTiles.DesertBottomRight},
					new MapEdit{X = 208, Y = 51, Tile = (byte)OwTiles.DesertMid},
					new MapEdit{X = 208, Y = 52, Tile = (byte)OwTiles.DesertBottomRight},

					new MapEdit{X = 209, Y = 52, Tile = (byte)OwTiles.GrassTile},
					new MapEdit{X = 210, Y = 52, Tile = (byte)OwTiles.GrassTile},
					new MapEdit{X = 210, Y = 53, Tile = (byte)OwTiles.GrassTile},
					new MapEdit{X = 211, Y = 53, Tile = (byte)OwTiles.GrassTile},
					new MapEdit{X = 210, Y = 54, Tile = (byte)OwTiles.GrassTile},

					new MapEdit{X = 208, Y = 53, Tile = (byte)OwTiles.MountainTopLeft},
					new MapEdit{X = 209, Y = 53, Tile = (byte)OwTiles.MountainTopRight},
					new MapEdit{X = 209, Y = 54, Tile = (byte)OwTiles.MountainMidRight},

					new MapEdit{X = 210, Y = 51, Tile = (byte)OwTiles.MountainBottomLeft},
					new MapEdit{X = 211, Y = 52, Tile = (byte)OwTiles.MountainBottomLeft},
		};

		public static List<MapEdit> RiverToMelmond =
			new List<MapEdit>
			{
					//Top Side Mountain
					new MapEdit{X = 84, Y = 146, Tile = (byte)OwTiles.CoastBottomRight},
					new MapEdit{X = 80, Y = 147, Tile = (byte)OwTiles.CoastBottomRight},
					new MapEdit{X = 77, Y = 149, Tile = (byte)OwTiles.CoastBottomRight},
					new MapEdit{X = 78, Y = 148, Tile = (byte)OwTiles.CoastBottomRight},
					new MapEdit{X = 83, Y = 146, Tile = (byte)OwTiles.CoastBottom},
					new MapEdit{X = 82, Y = 146, Tile = (byte)OwTiles.CoastBottom},
					new MapEdit{X = 81, Y = 146, Tile = (byte)OwTiles.CoastBottom},
					new MapEdit{X = 79, Y = 147, Tile = (byte)OwTiles.CoastBottom},

					new MapEdit{X = 81, Y = 147, Tile = (byte)OwTiles.MountainTopLeft},
					new MapEdit{X = 81, Y = 148, Tile = (byte)OwTiles.MountainMid},
					new MapEdit{X = 82, Y = 147, Tile = (byte)OwTiles.MountainTopMid},
					new MapEdit{X = 83, Y = 147, Tile = (byte)OwTiles.MountainTopMid},
					new MapEdit{X = 84, Y = 147, Tile = (byte)OwTiles.MountainTopRight},
					new MapEdit{X = 78, Y = 151, Tile = (byte)OwTiles.MountainMidLeft},
					new MapEdit{X = 78, Y = 152, Tile = (byte)OwTiles.MountainBottomLeft},
					new MapEdit{X = 79, Y = 152, Tile = (byte)OwTiles.MountainBottomRight},
					new MapEdit{X = 79, Y = 151, Tile = (byte)OwTiles.MountainMidRight},
					new MapEdit{X = 79, Y = 150, Tile = (byte)OwTiles.MountainMidRight},
					new MapEdit{X = 80, Y = 149, Tile = (byte)OwTiles.MountainBottomMid},
					new MapEdit{X = 78, Y = 149, Tile = (byte)OwTiles.MountainTopLeft},
					new MapEdit{X = 80, Y = 148, Tile = (byte)OwTiles.MountainTopMid},
					new MapEdit{X = 79, Y = 148, Tile = (byte)OwTiles.MountainTopLeft},
					new MapEdit{X = 79, Y = 149, Tile = (byte)OwTiles.MountainMid},

					//Bottom Side Mountain
					new MapEdit{X = 86, Y = 151, Tile = (byte)OwTiles.CoastTopLeft},
					new MapEdit{X = 87, Y = 150, Tile = (byte)OwTiles.CoastTopLeft},
					new MapEdit{X = 85, Y = 152, Tile = (byte)OwTiles.CoastTop},
					new MapEdit{X = 88, Y = 149, Tile = (byte)OwTiles.MarshBottomLeft},

					new MapEdit{X = 82, Y = 148, Tile = (byte)OwTiles.MountainBottomMid},
					new MapEdit{X = 83, Y = 148, Tile = (byte)OwTiles.MountainBottomRight},
					new MapEdit{X = 85, Y = 149, Tile = (byte)OwTiles.MountainTopLeft},
					new MapEdit{X = 84, Y = 150, Tile = (byte)OwTiles.MountainTopMid},
					new MapEdit{X = 84, Y = 151, Tile = (byte)OwTiles.MountainBottomMid},
					new MapEdit{X = 85, Y = 151, Tile = (byte)OwTiles.MountainBottomRight},
					new MapEdit{X = 83, Y = 150, Tile = (byte)OwTiles.MountainTopLeft},
					new MapEdit{X = 81, Y = 149, Tile = (byte)OwTiles.MountainBottomRight},
					new MapEdit{X = 86, Y = 147, Tile = (byte)OwTiles.MountainTopLeft},
					new MapEdit{X = 87, Y = 147, Tile = (byte)OwTiles.MountainTopRight},
					new MapEdit{X = 86, Y = 148, Tile = (byte)OwTiles.MountainMidLeft},
					new MapEdit{X = 87, Y = 148, Tile = (byte)OwTiles.MountainMidRight},
					new MapEdit{X = 87, Y = 149, Tile = (byte)OwTiles.MountainBottomRight},
					new MapEdit{X = 86, Y = 149, Tile = (byte)OwTiles.MountainMid},
					new MapEdit{X = 86, Y = 150, Tile = (byte)OwTiles.MountainBottomRight},
					new MapEdit{X = 85, Y = 150, Tile = (byte)OwTiles.MountainMid},
					new MapEdit{X = 82, Y = 151, Tile = (byte)OwTiles.MountainTopMid},
					new MapEdit{X = 81, Y = 151, Tile = (byte)OwTiles.MountainTopLeft},
					new MapEdit{X = 81, Y = 152, Tile = (byte)OwTiles.MountainMidLeft},
					new MapEdit{X = 81, Y = 153, Tile = (byte)OwTiles.MountainBottomLeft},

					// River from Dwarf to Melmond
					new MapEdit{X = 85, Y = 147, Tile = (byte)OwTiles.RiverTile},
					new MapEdit{X = 85, Y = 148, Tile = (byte)OwTiles.RiverBottomRight},
					new MapEdit{X = 84, Y = 148, Tile = (byte)OwTiles.RiverTopLeft},
					new MapEdit{X = 84, Y = 149, Tile = (byte)OwTiles.RiverBottomRight},
					new MapEdit{X = 83, Y = 149, Tile = (byte)OwTiles.RiverTile},
					new MapEdit{X = 82, Y = 149, Tile = (byte)OwTiles.RiverTopLeft},
					new MapEdit{X = 82, Y = 150, Tile = (byte)OwTiles.RiverBottomRight},
					new MapEdit{X = 81, Y = 150, Tile = (byte)OwTiles.RiverTile},
					new MapEdit{X = 80, Y = 150, Tile = (byte)OwTiles.RiverTopLeft},
					new MapEdit{X = 80, Y = 151, Tile = (byte)OwTiles.RiverTile},
					new MapEdit{X = 80, Y = 152, Tile = (byte)OwTiles.RiverTile},
					new MapEdit{X = 80, Y = 153, Tile = (byte)OwTiles.MarshTile},
					new MapEdit{X = 79, Y = 153, Tile = (byte)OwTiles.MarshTile},
			};
		public static List<MapEdit> GaiaMountainPass =
			new List<MapEdit>
			{
				new MapEdit{X = 0xD4, Y = 0x22, Tile = (byte)OwTiles.MountainBottomRight},
				new MapEdit{X = 0xD3, Y = 0x23, Tile = (byte)OwTiles.MountainMidRight},
				new MapEdit{X = 0xD4, Y = 0x23, Tile = (byte)OwTiles.GrassTopLeft},
				new MapEdit{X = 0xD5, Y = 0x23, Tile = (byte)OwTiles.GrassyMid},
				new MapEdit{X = 0xD6, Y = 0x23, Tile = (byte)OwTiles.GrassBottomRight},
				new MapEdit{X = 0xD7, Y = 0x23, Tile = (byte)OwTiles.MountainMidLeft},

				new MapEdit{X = 0xD3, Y = 0x24, Tile = (byte)OwTiles.MountainMidRight},
				new MapEdit{X = 0xD4, Y = 0x24, Tile = (byte)OwTiles.GrassBottomLeft},
				new MapEdit{X = 0xD5, Y = 0x24, Tile = (byte)OwTiles.GrassBottomRight},
				new MapEdit{X = 0xD6, Y = 0x24, Tile = (byte)OwTiles.MountainTopLeft},

				new MapEdit{X = 0xD3, Y = 0x25, Tile = (byte)OwTiles.MountainMidRight},
				new MapEdit{X = 0xD4, Y = 0x25, Tile = (byte)OwTiles.GrassTile},
				new MapEdit{X = 0xD5, Y = 0x25, Tile = (byte)OwTiles.MountainTopLeft},
				new MapEdit{X = 0xD3, Y = 0x26, Tile = (byte)OwTiles.MountainMidRight},
				new MapEdit{X = 0xD4, Y = 0x26, Tile = (byte)OwTiles.GrassTile},
				new MapEdit{X = 0xD5, Y = 0x26, Tile = (byte)OwTiles.MountainMidLeft},
				new MapEdit{X = 0xD3, Y = 0x27, Tile = (byte)OwTiles.MountainMidRight},
				new MapEdit{X = 0xD4, Y = 0x27, Tile = (byte)OwTiles.GrassTile},
				new MapEdit{X = 0xD5, Y = 0x27, Tile = (byte)OwTiles.MountainMidLeft},
				new MapEdit{X = 0xD3, Y = 0x28, Tile = (byte)OwTiles.MountainMidRight},
				new MapEdit{X = 0xD4, Y = 0x28, Tile = (byte)OwTiles.GrassTile},
				new MapEdit{X = 0xD5, Y = 0x28, Tile = (byte)OwTiles.MountainBottomLeft},
				new MapEdit{X = 0xD3, Y = 0x29, Tile = (byte)OwTiles.MountainMidRight},
				new MapEdit{X = 0xD4, Y = 0x29, Tile = (byte)OwTiles.GrassTile},
				new MapEdit{X = 0xD5, Y = 0x29, Tile = (byte)OwTiles.CoastTopLeft},
				new MapEdit{X = 0xD3, Y = 0x2A, Tile = (byte)OwTiles.MountainMidRight},
				new MapEdit{X = 0xD4, Y = 0x2A, Tile = (byte)OwTiles.GrassTile},
				new MapEdit{X = 0xD5, Y = 0x2A, Tile = (byte)OwTiles.CoastLeft},
				new MapEdit{X = 0xD3, Y = 0x2B, Tile = (byte)OwTiles.MountainMidRight},
				new MapEdit{X = 0xD4, Y = 0x2B, Tile = (byte)OwTiles.GrassTile},
				new MapEdit{X = 0xD5, Y = 0x2B, Tile = (byte)OwTiles.CoastLeft},
				new MapEdit{X = 0xD3, Y = 0x2C, Tile = (byte)OwTiles.MountainMidRight},
				new MapEdit{X = 0xD4, Y = 0x2C, Tile = (byte)OwTiles.GrassTile},
				new MapEdit{X = 0xD5, Y = 0x2C, Tile = (byte)OwTiles.CoastBottomLeft},
				new MapEdit{X = 0xD4, Y = 0x2D, Tile = (byte)OwTiles.MountainTopRight},
				new MapEdit{X = 0xD5, Y = 0x2D, Tile = (byte)OwTiles.GrassTile},
				new MapEdit{X = 0xD6, Y = 0x2D, Tile = (byte)OwTiles.CoastBottomLeft},
				new MapEdit{X = 0xD5, Y = 0x2E, Tile = (byte)OwTiles.MountainTopRight},
				new MapEdit{X = 0xD6, Y = 0x2E, Tile = (byte)OwTiles.GrassTile},
				new MapEdit{X = 0xD7, Y = 0x2E, Tile = (byte)OwTiles.CoastBottomLeft},
				new MapEdit{X = 0xD6, Y = 0x2F, Tile = (byte)OwTiles.MountainTopRight},
				new MapEdit{X = 0xD7, Y = 0x2F, Tile = (byte)OwTiles.GrassTile},
				new MapEdit{X = 0xD8, Y = 0x2F, Tile = (byte)OwTiles.CoastBottomLeft},
				new MapEdit{X = 0xD7, Y = 0x30, Tile = (byte)OwTiles.MountainTopRight},
				new MapEdit{X = 0xD8, Y = 0x30, Tile = (byte)OwTiles.ForestTopLeft},
				new MapEdit{X = 0xD9, Y = 0x30, Tile = (byte)OwTiles.ForestTopRight},
			};


	}
}
