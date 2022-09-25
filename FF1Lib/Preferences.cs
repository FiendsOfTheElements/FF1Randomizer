﻿namespace FF1Lib
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
		public bool DisableDamageTileSFX { get; set; }
		public MenuColor MenuColor { get; set; } = MenuColor.Blue;
		public MapmanSlot MapmanSlot { get; set; } = MapmanSlot.Leader;
		public bool DisableSpellCastFlash { get; set; } = true;
		public bool LockRespondRate { get; set; } = true;
		public bool UninterruptedMusic { get; set; } = false;
		public bool ChangeLute { get; set; } = false;
		public TitanSnack TitanSnack { get; set; } = TitanSnack.Ruby;
		public bool randomShardNames { get; set; } = false;
		public Fate HurrayDwarfFate { get; set; } = Fate.Spare;
		public bool RenounceAutosort { get; set; } = false;
		public bool RenounceChestInfo { get; set; } = false;
		public bool RenounceCantHoldRed { get; set; } = false;
		public bool AccessibleSpellNames { get; set; } = false;
		public bool CleanBlursedEquipmentNames { get; set; } = false;
		public bool ShopInfoIcons { get; set; } = false;
		public bool NoTabLayout { get; set; } = false;
	  public string SpriteSheet { get; set; } = null;
		public bool CropScreen { get; set; } = false;
		public bool OptOutSpeedHackWipes { get; set; } = false;
		public bool OptOutSpeedHackMessages { get; set; } = false;
		public bool OptOutSpeedHackDash { get; set; } = false;
		public bool QuickJoy2Reset { get; set; } = false;

		public string PlayerName { get; set; } = "Player 01";
		public bool BlandSite { get; set; } = false;
	}
}
