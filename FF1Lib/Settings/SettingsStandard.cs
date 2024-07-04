using RomUtilities;
using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FF1Lib
{
	public partial class Settings
	{
		private static List<Setting> StandardSettings = new()
		{
			new Setting("Bugfixes", 1),
			new Setting("ThiefMode", typeof(ThiefOptions), (int)ThiefOptions.DoubleHit),
			new Setting("WhiteMageMode", typeof(WhiteMageOptions), (int)WhiteMageOptions.ImprovedHarm),
			//new NestedSetting("TestFlagList", new List<Setting>() { new Setting("Lute", 0), new Setting("HealPotion", 99) } )

		};
	}
	public partial class LayoutData
	{
		private static List<LayoutSection> StandardLayout = new()
		{
			new LayoutSection() { Name = "Classes", Flags = new()
			{
				new LayoutFlag() { Name = "ThiefMode", DisplayName = "Thief", Tooltip = "Select Thief Mode..." },
				new LayoutFlag() { Name = "WhiteMageMode", DisplayName = "White Mage", Tooltip = "Select White Mage Mode..." },
			} },
			new LayoutSection() { Name = "Misc", Flags = new()
			{
				new LayoutFlag() { Name = "Bugfixes", DisplayName = "Enable Bugfixes", Tooltip = "Allow whole set of different bugfixes..." },
			} },
		};
	}

}
