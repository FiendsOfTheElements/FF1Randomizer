using RomUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FF1Lib.SpellCrafter
{
	public class Spell
	{
		public byte[] Data { get; private set; }

		public int Index { get; set; }

		public string Name { get; set; }

		public SpellTag Tag { get; set; }

		public SpellMessage Message { get; set; }

		public SpellPermission Permissions { get; set; }

		public int Level => GetSpellLevel(Index);

		public SpellSchool School => GetSpellSchool(Index);

		public byte Accuracy
		{
			get { return Data[0]; }
			set { Data[0] = value; }
		}

		public byte Effectivity
		{
			get { return Data[1]; }
			set { Data[1] = value; }
		}

		public SpellStatus Status
		{
			get { return (SpellStatus)Data[1]; }
			set { Data[1] = (byte)value; }
		}
		public SpellElement Protection
		{
			get { return (SpellElement)Data[1]; }
			set { Data[1] = (byte)value; }
		}

		public SpellElement Element
		{
			get { return (SpellElement)Data[2]; }
			set { Data[2] = (byte)value; }
		}

		public SpellTargeting Targeting
		{
			get { return (SpellTargeting)Data[3]; }
			set { Data[3] = (byte)value; }
		}

		public SpellRoutine Type
		{
			get { return (SpellRoutine)Data[4]; }
			set { Data[4] = (byte)value; }
		}

		public SpellGraphic GraphicEffect
		{
			get { return (SpellGraphic)Data[5]; }
			set { Data[5] = (byte)value; }
		}

		public SpellPalette Palette
		{
			get { return (SpellPalette)Data[6]; }
			set { Data[6] = (byte)value; }
		}

		public Spell(int i, byte[] blob, string name, byte msg, SpellPermission permissions)
		{
			Index = i;
			Data = blob;
			Name = name;
			Message = (SpellMessage)msg;
			Permissions = permissions;
		}

		public Spell()
		{
			Data = new byte[8];
		}

		public static SpellSchool GetSpellSchool(int index)
		{
			return index % 8 > 3 ? SpellSchool.Black : SpellSchool.White;
		}

		public static int GetSpellLevel(int index)
		{
			return index / 8 + 1;
		}
	}
}
