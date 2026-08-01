using RomUtilities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FF1Lib
{
	public enum FiendDomain
	{
		Earth,
		Volcano,
		Sea,
		Sky
	}

	public struct ExtAltFiend
	{
		public string Name;
		public FiendDomain Domain;
		public BlackOrbFiends Fiend;
		public byte[] OrbPalette;
	}

	public partial class ExtAltFiends
	{
		public bool ExtendedFiends { get; set; } = false;
		public bool NormalAltFiends { get; set; } = false;

		public ExtAltFiend EarthFiend { get; set; }
		public ExtAltFiend VolcanoFiend { get; set; }
		public ExtAltFiend SeaFiend { get; set; }
		public ExtAltFiend SkyFiend { get; set; }

		public ExtAltFiends(Flags flags)
		{
			if (!(bool)flags.AlternateFiends)
			{
				return;
			}
			else
			{
				// Add any standard alt fiends pool, else pandemonium
				NormalAltFiends = (bool)flags.FinalFantasy1BonusFiends ||
					(bool)flags.FinalFantasy2Fiends ||
					(bool)flags.FinalFantasy3Fiends ||
					(bool)flags.FinalFantasy4Fiends ||
					(bool)flags.FinalFantasy5Fiends ||
					(bool)flags.FinalFantasy6Fiends ||
					(bool)flags.WinnerCircleFiends;

				if (!NormalAltFiends && (bool)flags.BlackOrbFiends)
				{
					ExtendedFiends = true;
				}
			}
		}
	}


}
