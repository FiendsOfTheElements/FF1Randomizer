using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FF1Lib.Sanity
{
	public class SCTileDef
	{
		public SCBitFlags BitFlags { get; private set; }

		public SCBitFlags OWBitFlags { get; private set; }

		public SCBitFlags SpBitFlags { get; private set; }

		public TileProp TileProp { get; private set; }

		public SCTileDef(TileProp tileProp)
		{
			TileProp = tileProp;

			BitFlags = SCBitFlags.None;
			OWBitFlags = SCBitFlags.None;
			SpBitFlags = SCBitFlags.None;

			if ((TileProp.TilePropFunc & (TilePropFunc.TP_SPEC_MASK | TilePropFunc.TP_NOMOVE)) == TilePropFunc.TP_NOMOVE)
			{
				BitFlags = SCBitFlags.Blocked;
				OWBitFlags = SCBitFlags.Blocked;
			}
			else if ((TileProp.TilePropFunc & (TilePropFunc.TP_SPEC_DOOR | TilePropFunc.TP_SPEC_LOCKED | TilePropFunc.TP_NOMOVE)) == (TilePropFunc.TP_SPEC_LOCKED | TilePropFunc.TP_NOMOVE))
			{
				BitFlags = SCBitFlags.Key;
			}
			else if ((TileProp.TilePropFunc & TilePropFunc.TP_SPEC_MASK) == TilePropFunc.TP_SPEC_CROWN)
			{
				BitFlags |= SCBitFlags.Crown;
			}
			else if ((TileProp.TilePropFunc & TilePropFunc.TP_SPEC_MASK) == TilePropFunc.TP_SPEC_CUBE)
			{
				BitFlags |= SCBitFlags.Cube;
			}
			else if ((TileProp.TilePropFunc & TilePropFunc.TP_SPEC_MASK) == TilePropFunc.TP_SPEC_4ORBS)
			{
				BitFlags |= SCBitFlags.Orbs;
			}

			if ((TileProp.TilePropFunc & TilePropFunc.TP_SPEC_MASK) == TilePropFunc.TP_SPEC_USEROD)
			{
				SpBitFlags = SCBitFlags.UseRod;
			}
			else if ((TileProp.TilePropFunc & TilePropFunc.TP_SPEC_MASK) == TilePropFunc.TP_SPEC_USELUTE)
			{
				SpBitFlags = SCBitFlags.UseLute;
			}
			else if ((TileProp.TilePropFunc & TilePropFunc.TP_TELE_MASK) == TilePropFunc.TP_TELE_WARP)
			{
				BitFlags |= SCBitFlags.Impassable;
				SpBitFlags = SCBitFlags.Warp;
			}
			else if ((TileProp.TilePropFunc & TilePropFunc.TP_TELE_MASK) == TilePropFunc.TP_TELE_NORM)
			{
				BitFlags |= SCBitFlags.Impassable;
				SpBitFlags = SCBitFlags.Teleport;
			}
			else if ((TileProp.TilePropFunc & TilePropFunc.TP_TELE_MASK) == TilePropFunc.TP_TELE_EXIT)
			{
				//they share the same code
				BitFlags |= SCBitFlags.Impassable;
				SpBitFlags = SCBitFlags.Exit;
				SpBitFlags |= SCBitFlags.UseFloater;
			}
			else if ((TileProp.TilePropFunc & TilePropFunc.TP_SPEC_MASK) == TilePropFunc.TP_SPEC_TREASURE)
			{
				SpBitFlags = SCBitFlags.Treasure;
			}
			else if ((TileProp.TilePropFunc & TilePropFunc.TP_SPEC_MASK) == TilePropFunc.TP_SPEC_EARTHORB)
			{
				SpBitFlags = SCBitFlags.EarthOrb;
			}
			else if ((TileProp.TilePropFunc & TilePropFunc.TP_SPEC_MASK) == TilePropFunc.TP_SPEC_FIREORB)
			{
				SpBitFlags = SCBitFlags.FireOrb;
			}
			else if ((TileProp.TilePropFunc & TilePropFunc.TP_SPEC_MASK) == TilePropFunc.TP_SPEC_WATERORB)
			{
				SpBitFlags = SCBitFlags.WaterOrb;
			}
			else if ((TileProp.TilePropFunc & TilePropFunc.TP_SPEC_MASK) == TilePropFunc.TP_SPEC_AIRORB)
			{
				SpBitFlags = SCBitFlags.AirOrb;
			}

			if ((TileProp.TilePropFunc & TilePropFunc.OWTP_NORMAL) == 0)
			{
				OWBitFlags = SCBitFlags.Land;
			}
			else if ((TileProp.TilePropFunc & TilePropFunc.OWTP_RIVER) == 0)
			{
				OWBitFlags = SCBitFlags.River;
			}
			else if ((TileProp.TilePropFunc & TilePropFunc.OWTP_OCEAN) == 0)
			{
				OWBitFlags = SCBitFlags.Ocean;
			}
			else
			{
				OWBitFlags = SCBitFlags.Blocked;
			}

			if ((TileProp.TilePropFunc & TilePropFunc.OWTP_DOCKAIRSHIP) == 0) OWBitFlags |= SCBitFlags.AirDock;
			if ((TileProp.TilePropFunc & TilePropFunc.OWTP_DOCKSHIP) != 0) OWBitFlags |= SCBitFlags.ShipDock;
			if ((TileProp.TilePropFunc & TilePropFunc.OWTP_SPEC_MASK) ==  TilePropFunc.OWTP_SPEC_CHIME) OWBitFlags |= SCBitFlags.Chime;
			if ((TileProp.TilePropFunc & TilePropFunc.OWTP_SPEC_MASK) == TilePropFunc.OWTP_SPEC_CARAVAN) OWBitFlags |= SCBitFlags.Caravan;
			if (TileProp.Byte2 >= (byte)0x80) OWBitFlags |= SCBitFlags.Enter;
		}
	}
}
