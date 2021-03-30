using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FF1Lib.Sanity
{
	public enum SCBitFlags : ushort
	{
		None =		0x0000,
		Lute =		0x0001,
		Crown =		0x0002,
		Key =		0x0004,
		Ruby =		0x0008,
		Rod =		0x0010,
		Chime =		0x0020,
		Cube =		0x0040,
		Oxyale =	0x0080,
		Orbs =		0x0100,

		Ship =		0x0200,
		Bridge =	0x0400,
		Canal =		0x0600,
		Canoe =		0x0800,

		Impassable =0x2000,
		Blocked =	0x4000,
		Done =		0x8000,

		Land =		0x0001,
		River =		0x0002,
		Ocean =		0x0004,
		AirDock =	0x0008,
		ShipDock =	0x0010,
		Chime2 = 0x0020,
		Enter =0x0040,
		Caravan =	0x0080,

		UseRod = 0x0001,
		UseLute = 0x0002,
		UseFloater =0x0004,
		Warp = 0x0008,
		Teleport = 0x0010,
		Exit = 0x0020,
		Treasure = 0x0040,
		EarthOrb = 0x0080,
		FireOrb = 0x0100,
		WaterOrb = 0x0200,
		AirOrb = 0x0400,
	}
}
