using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FF1Lib.Sanity
{
	public enum SCRequirements : int
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
		Tnt =		0x0200,
		Canoe =		0x0400,
		Floater =	0x0800,
		Bridge =	0x1000,
		Canal =		0x2000,
		Bottle =	0x4000,
		Crystal =	0x8000,
		Herb =		0x10000,
		Adamant =	0x20000,
		Slab =		0x40000,
		Ship =		0x80000,
	}
}
