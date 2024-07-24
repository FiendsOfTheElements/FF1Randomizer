using System.Numerics;
using System.Reflection;
using Newtonsoft.Json;
using System.IO.Compression;
using static FF1Lib.FF1Rom;
using FF1Lib.Sanity;

namespace FF1Lib
{
	public partial class Flags : IIncentiveFlags, IScaleFlags, IVictoryConditionFlags, IFloorShuffleFlags, IItemPlacementFlags
	{
		public bool? AllFighters
		{
			get
			{
				if (FIGHTER1 == FIGHTER2 == FIGHTER3 == FIGHTER4)
				{
					return FIGHTER1;
				}
				else
				{
					return false;
				}
			}
			set
			{
				FIGHTER1 = FIGHTER2 = FIGHTER3 = FIGHTER4 = value;
			}
		}
		public bool? AllThieves
		{
			get
			{
				if (THIEF1 == THIEF2 == THIEF3 == THIEF4)
				{
					return THIEF1;
				}
				else
				{
					return false;
				}
			}
			set
			{
				THIEF1 = THIEF2 = THIEF3 = THIEF4 = value;
			}
		}
		public bool? AllBlackBelts
		{
			get
			{
				if (BLACK_BELT1 == BLACK_BELT2 == BLACK_BELT3 == BLACK_BELT4)
				{
					return BLACK_BELT1;
				}
				else
				{
					return false;
				}
			}
			set
			{
				BLACK_BELT1 = BLACK_BELT2 = BLACK_BELT3 = BLACK_BELT4 = value;
			}
		}
		public bool? AllRedMages
		{
			get
			{
				if (RED_MAGE1 == RED_MAGE2 == RED_MAGE3 == RED_MAGE4)
				{
					return RED_MAGE1;
				}
				else
				{
					return false;
				}
			}
			set
			{
				RED_MAGE1 = RED_MAGE2 = RED_MAGE3 = RED_MAGE4 = value;
			}
		}
		public bool? AllWhiteMages
		{
			get
			{
				if (WHITE_MAGE1 == WHITE_MAGE2 == WHITE_MAGE3 == WHITE_MAGE4)
				{
					return WHITE_MAGE1;
				}
				else
				{
					return false;
				}
			}
			set
			{
				WHITE_MAGE1 = WHITE_MAGE2 = WHITE_MAGE3 = WHITE_MAGE4 = value;
			}
		}
		public bool? AllBlackMages
		{
			get
			{
				if (BLACK_MAGE1 == BLACK_MAGE2 == BLACK_MAGE3 == BLACK_MAGE4)
				{
					return BLACK_MAGE1;
				}
				else
				{
					return false;
				}
			}
			set
			{
				BLACK_MAGE1 = BLACK_MAGE2 = BLACK_MAGE3 = BLACK_MAGE4 = value;
			}
		}
		public bool? AllKnights
		{
			get
			{
				if (KNIGHT1 == KNIGHT2 == KNIGHT3 == KNIGHT4)
				{
					return KNIGHT1;
				}
				else
				{
					return false;
				}
			}
			set
			{
				KNIGHT1 = KNIGHT2 = KNIGHT3 = KNIGHT4 = value;
			}
		}
		public bool? AllNinjas
		{
			get
			{
				if (NINJA1 == NINJA2 == NINJA3 == NINJA4)
				{
					return NINJA1;
				}
				else
				{
					return false;
				}
			}
			set
			{
				NINJA1 = NINJA2 = NINJA3 = NINJA4 = value;
			}
		}
		public bool? AllMasters
		{
			get
			{
				if (MASTER1 == MASTER2 == MASTER3 == MASTER4)
				{
					return MASTER1;
				}
				else
				{
					return false;
				}
			}
			set
			{
				MASTER1 = MASTER2 = MASTER3 = MASTER4 = value;
			}
		}
		public bool? AllRedWizards
		{
			get
			{
				if (RED_WIZ1 == RED_WIZ2 == RED_WIZ3 == RED_WIZ4)
				{
					return RED_WIZ1;
				}
				else
				{
					return false;
				}
			}
			set
			{
				RED_WIZ1 = RED_WIZ2 = RED_WIZ3 = RED_WIZ4 = value;
			}
		}
		public bool? AllWhiteWizards
		{
			get
			{
				if (WHITE_WIZ1 == WHITE_WIZ2 == WHITE_WIZ3 == WHITE_WIZ4)
				{
					return WHITE_WIZ1;
				}
				else
				{
					return false;
				}
			}
			set
			{
				WHITE_WIZ1 = WHITE_WIZ2 = WHITE_WIZ3 = WHITE_WIZ4 = value;
			}
		}
		public bool? AllBlackWizards
		{
			get
			{
				if (BLACK_WIZ1 == BLACK_WIZ2 == BLACK_WIZ3 == BLACK_WIZ4)
				{
					return BLACK_WIZ1;
				}
				else
				{
					return false;
				}
			}
			set
			{
				BLACK_WIZ1 = BLACK_WIZ2 = BLACK_WIZ3 = BLACK_WIZ4 = value;
			}
		}
		public bool? AllNones
		{
			get
			{
				if (NONE_CLASS2 == NONE_CLASS3 == NONE_CLASS4)
				{
					return NONE_CLASS2;
				}
				else
				{
					return false;
				}
			}
			set
			{
				NONE_CLASS2 = NONE_CLASS3 = NONE_CLASS4 = value;
			}
		}
		public bool? AllForced
		{
			get
			{
				if (FORCED1 == FORCED2 == FORCED3 == FORCED4)
				{
					return FORCED1;
				}
				else
				{
					return false;
				}
			}
			set
			{
				FORCED1 = FORCED2 = FORCED3 = FORCED4 = value;
			}
		}
	}
}
