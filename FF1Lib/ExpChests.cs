using RomUtilities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FF1Lib
{
	public class ExpChests
	{
		FF1Rom rom;
		Flags flags;

		public ExpChests(FF1Rom _rom, Flags _flags)
		{
			rom = _rom;
			flags = _flags;
		}

		public void SetExpChests()
		{			
			rom.PutInBank(0x11,0xB0A0,Blob.FromHex("48A9FA2083B1202DB1B0106820B9EC20CBB018E67DE67DA2FB8A606820B9EC20EADD18E67DE67DA2FC8A60AD006209808D0062A5108588A5118589A9008DD16A8DD26AA2C02021B1A2802021B1A2402021B1A2002021B12097B1A5888D7868A5898D7968A90020A8B1A90120A8B1A90220A8B1A9034CA8B1AD0062297F8D006260BD01614A2ED16A4A2ED26A60A90085248525A9038512A935857EA98E20C7D6A5FF4901851AA908851B208EB1A51B18690B851BC96090F2208EB120C2D7A52438D005A52518F0F008208EB1A51B38E90B851BC912B0F2A90085248525A01184582860AAA9DD48A9CF488A4CB1D4A9DD48A9CF484CA1D6A9DD48A9CF48A98B48A94248A91B4C03FEAAA9DD48A9CF48A9DD48A9D448A91B4C03FE"));

			rom.PutInBank(0x11, 0xB04A, Blob.FromHex("4CA0B0"));

			rom.PutInBank(0x1F,0xDDD0,Blob.FromHex("A9114C03FE8A20DA8760"));


			Dictionary<int, string> dialogs = new Dictionary<int, string>();

			dialogs.Add(0x14A, "In the treasure box,\nyou found..\n#\n\n\nA..Take Gold\nB..Take Exp");
			dialogs.Add(0x14B, "A sacrifice for power!\n\nExperience taken.");
			dialogs.Add(0x14C, "The greed has\noverwhelmed you!\n\nGold taken.");

			rom.InsertDialogs(dialogs);
		}
	}
}
