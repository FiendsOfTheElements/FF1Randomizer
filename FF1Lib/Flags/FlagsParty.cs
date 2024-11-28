using System.Numerics;
using System.Reflection;
using Newtonsoft.Json;
using System.IO.Compression;
using static FF1Lib.FF1Rom;
using FF1Lib.Sanity;

namespace FF1Lib
{
	public class FlagIsAllClasses : Attribute
	{
		public bool IsAllClasses { get; set; } = true;
	}
	public partial class Flags : IIncentiveFlags, IScaleFlags, IVictoryConditionFlags, IFloorShuffleFlags, IItemPlacementFlags
	{
		[System.Text.Json.Serialization.JsonIgnore, FlagIsAllClasses]
		public bool? AllFighters
		{
			get
			{
				if (new[] { FIGHTER1, FIGHTER2, FIGHTER3, FIGHTER4 }.Distinct().Count() == 1)
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
		[System.Text.Json.Serialization.JsonIgnore, FlagIsAllClasses]
		public bool? AllThieves
		{
			get
			{
				if (new[] { THIEF1, THIEF2, THIEF3, THIEF4 }.Distinct().Count() == 1)
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
		[System.Text.Json.Serialization.JsonIgnore, FlagIsAllClasses]
		public bool? AllBlackBelts
		{
			get
			{
				if (new[] { BLACK_BELT1, BLACK_BELT2, BLACK_BELT3, BLACK_BELT4 }.Distinct().Count() == 1)
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
		[System.Text.Json.Serialization.JsonIgnore, FlagIsAllClasses]
		public bool? AllRedMages
		{
			get
			{
				if (new[] { RED_MAGE1, RED_MAGE2, RED_MAGE3, RED_MAGE4 }.Distinct().Count() == 1)
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
		[System.Text.Json.Serialization.JsonIgnore, FlagIsAllClasses]
		public bool? AllWhiteMages
		{
			get
			{
				if (new[] { WHITE_MAGE1, WHITE_MAGE2, WHITE_MAGE3, WHITE_MAGE4 }.Distinct().Count() == 1)
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
		[System.Text.Json.Serialization.JsonIgnore, FlagIsAllClasses]
		public bool? AllBlackMages
		{
			get
			{
				if (new[] { BLACK_MAGE1, BLACK_MAGE2, BLACK_MAGE3, BLACK_MAGE4 }.Distinct().Count() == 1)
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
		[System.Text.Json.Serialization.JsonIgnore, FlagIsAllClasses]
		public bool? AllKnights
		{
			get
			{
				if (new[] { KNIGHT1, KNIGHT2, KNIGHT3, KNIGHT4 }.Distinct().Count() == 1)
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
		[System.Text.Json.Serialization.JsonIgnore, FlagIsAllClasses]
		public bool? AllNinjas
		{
			get
			{
				if (new[] { NINJA1, NINJA2, NINJA3, NINJA4 }.Distinct().Count() == 1)
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
		[System.Text.Json.Serialization.JsonIgnore, FlagIsAllClasses]
		public bool? AllMasters
		{
			get
			{
				if (new[] { MASTER1, MASTER2, MASTER3, MASTER4 }.Distinct().Count() == 1)
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
		[System.Text.Json.Serialization.JsonIgnore, FlagIsAllClasses]
		public bool? AllRedWizards
		{
			get
			{
				if (new[] { RED_WIZ1, RED_WIZ2, RED_WIZ3, RED_WIZ4 }.Distinct().Count() == 1)
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
		[System.Text.Json.Serialization.JsonIgnore, FlagIsAllClasses]
		public bool? AllWhiteWizards
		{
			get
			{
				if (new[] { WHITE_WIZ1, WHITE_WIZ2, WHITE_WIZ3, WHITE_WIZ4 }.Distinct().Count() == 1)
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
		[System.Text.Json.Serialization.JsonIgnore, FlagIsAllClasses]
		public bool? AllBlackWizards
		{
			get
			{
				if (new[] { BLACK_WIZ1, BLACK_WIZ2, BLACK_WIZ3, BLACK_WIZ4 }.Distinct().Count() == 1)
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
		[System.Text.Json.Serialization.JsonIgnore, FlagIsAllClasses]
		public bool? AllNones
		{
			get
			{
				if (new[] { NONE_CLASS2, NONE_CLASS3, NONE_CLASS4 }.Distinct().Count() == 1)
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
		[System.Text.Json.Serialization.JsonIgnore, FlagIsAllClasses]
		public bool? AllForced
		{
			get
			{

				if (new[] { FORCED1, FORCED2, FORCED3, FORCED4 }.Distinct().Count() == 1)
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
