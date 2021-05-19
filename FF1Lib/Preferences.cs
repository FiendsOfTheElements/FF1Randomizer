using System;
using System.Collections.Generic;
using System.Text;

namespace FF1Lib
{
	public class Preferences
	{
		public bool FunEnemyNames { get; set; }
		public bool PaletteSwap { get; set; }
		public bool ModernBattlefield { get; set; }
		public bool ThirdBattlePalette { get; set; }
		public bool TeamSteak { get; set; }
		public MusicShuffle Music { get; set; }
		public bool DisableDamageTileFlicker { get; set; } = true;
		public MenuColor MenuColor { get; set; } = MenuColor.Blue;
		public MapmanSlot MapmanSlot { get; set; } = MapmanSlot.Leader;
		public bool DisableSpellCastFlash { get; set; } = true;
		public bool ChangeLute { get; set; } = false;
		public TitanSnack TitanSnack { get; set; } = TitanSnack.Ruby;
		public Fate HurrayDwarfFate { get; set; } = Fate.Spare;
		public bool RenounceAutosort { get; set; } = false;
		public bool AccessibleSpellNames { get; set; } = false;
		public bool NoTabLayout { get; set; } = false;
	        public string SpriteSheet { get; set; } = null;
	}
}
