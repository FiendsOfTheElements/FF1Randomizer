namespace FF1Lib
{
	public enum Item : byte
	{
		None = 0,
		Lute = 1, // Hides an object
		Crown = 2, // Chest, trades for a quest item
		Crystal = 3, // Trades for a quest item
		Herb = 4, // Trades for a game event that trades for an item
		Key = 5, // Unlocks doors
		Tnt = 6, // Chest, removes a map object
		Adamant = 7, // Chest, trades for a weapon
		Slab = 8, // Chest, trades for a game event that trades for an item
		Ruby = 9, // Chest, hides an object
		Rod = 10, // Hides an object
		Floater = 11, // Chest, shows a map object
		Chime = 12, // Unlocks single entrance
		Tail = 13, // Chest, trades for class change
		Cube = 14, // Unlocks single entrance
		Bottle = 15, // Shop, shows an object
		Oxyale = 16, // Hides an object
		EarthOrb = 17, // Relocated from vanilla so the Shard can be contiguous with other counted items.
		FireOrb = 18,
		WaterOrb = 19,
		AirOrb = 20,

		Shard = 21, // This Enum is only correct once ShiftEarthOrbDown() is called.
		Tent = 22,
		Cabin = 23,
		House = 24,
		Heal = 25,
		Pure = 26,
		Soft = 27,

		WoodenNunchucks = 28,
		SmallKnife = 29,
		WoodenRod = 30,
		Rapier = 31,
		IronHammer = 32,
		ShortSword = 33,
		HandAxe = 34,
		Scimitar = 35,
		IronNunchucks = 36,
		LargeKnife = 37,
		IronStaff = 38,
		Sabre = 39,
		LongSword = 40,
		GreatAxe = 41,
		Falchon = 42,
		SilverKnife = 43,
		SilverSword = 44,
		SilverHammer = 45,
		SilverAxe = 46,
		FlameSword = 47,
		IceSword = 48,
		DragonSword = 49,
		GiantSword = 50,
		SunSword = 51,
		CoralSword = 52,
		WereSword = 53,
		RuneSword = 54,
		PowerRod = 55,
		LightAxe = 56,
		HealRod = 57,
		MageRod = 58,
		Defense = 59,
		WizardRod = 60,
		Vorpal = 61,
		CatClaw = 62,
		ThorHammer = 63,
		BaneSword = 64,
		Katana = 65,
		Xcalber = 66,
		Masamune = 67,

		Cloth = 68,
		WoodenArmor = 69,
		ChainArmor = 70,
		IronArmor = 71,
		SteelArmor = 72,
		SilverArmor = 73,
		FlameArmor = 74,
		IceArmor = 75,
		OpalArmor = 76,
		DragonArmor = 77,
		Copper = 78,
		Silver = 79,
		Gold = 80,
		Opal = 81,
		WhiteShirt = 82,
		BlackShirt = 83,
		WoodenShield = 84,
		IronShield = 85,
		SilverShield = 86,
		FlameShield = 87,
		IceShield = 88,
		OpalShield = 89,
		AegisShield = 90,
		Buckler = 91,
		ProCape = 92,
		Cap = 93,
		WoodenHelm = 94,
		IronHelm = 95,
		SilverHelm = 96,
		OpalHelm = 97,
		HealHelm = 98,
		Ribbon = 99,
		Gloves = 100,
		CopperGauntlets = 101,
		IronGauntlets = 102,
		SilverGauntlets = 103,
		ZeusGauntlets = 104,
		PowerGauntlets = 105,
		OpalGauntlets = 106,
		ProRing = 107,
		Gold10 = 108,
		Gold20 = 109,
		Gold25 = 110,
		Gold30 = 111,
		Gold55 = 112,
		Gold70 = 113,
		Gold85 = 114,
		Gold110 = 115,
		Gold135 = 116,
		Gold155 = 117,
		Gold160 = 118,
		Gold180 = 119,
		Gold240 = 120,
		Gold255 = 121,
		Gold260 = 122,
		Gold295 = 123,
		Gold300 = 124,
		Gold315 = 125,
		Gold330 = 126,
		Gold350 = 127,
		Gold385 = 128,
		Gold400 = 129,
		Gold450 = 130,
		Gold500 = 131,
		Gold530 = 132,
		Gold575 = 133,
		Gold620 = 134,
		Gold680 = 135,
		Gold750 = 136,
		Gold795 = 137,
		Gold880 = 138,
		Gold1020 = 139,
		Gold1250 = 140,
		Gold1455 = 141,
		Gold1520 = 142,
		Gold1760 = 143,
		Gold1975 = 144,
		Gold2000 = 145,
		Gold2750 = 146,
		Gold3400 = 147,
		Gold4150 = 148,
		Gold5000 = 149,
		Gold5450 = 150,
		Gold6400 = 151,
		Gold6720 = 152,
		Gold7340 = 153,
		Gold7690 = 154,
		Gold7900 = 155,
		Gold8135 = 156,
		Gold9000 = 157,
		Gold9300 = 158,
		Gold9500 = 159,
		Gold9900 = 160,
		Gold10000 = 161,
		Gold12350 = 162,
		Gold13000 = 163,
		Gold13450 = 164,
		Gold14050 = 165,
		Gold14720 = 166,
		Gold15000 = 167,
		Gold17490 = 168,
		Gold18010 = 169,
		Gold19990 = 170,
		Gold20000 = 171,
		Gold20010 = 172,
		Gold26000 = 173,
		Gold45000 = 174,
		Gold65000 = 175,

		// Additions for variables in chests
		Ship = 224,
		Airship = 228,
		Bridge = 232,
		Canal = 236,
		Canoe = 242
	}

	public enum Spell : byte
	{
		None = 175,
		CURE = 0xB0 + 0x00,
		HARM = 0xB0 + 0x01,
		FOG = 0xB0 + 0x02,
		RUSE = 0xB0 + 0x03,
		FIRE = 0xB0 + 0x04,
		SLEP = 0xB0 + 0x05,
		LOCK = 0xB0 + 0x06,
		LIT = 0xB0 + 0x07,
		LAMP = 0xB0 + 0x08,
		MUTE = 0xB0 + 0x09,
		ALIT = 0xB0 + 0x0A,
		INVS = 0xB0 + 0x0B,
		ICE = 0xB0 + 0x0C,
		DARK = 0xB0 + 0x0D,
		TMPR = 0xB0 + 0x0E,
		SLOW = 0xB0 + 0x0F,
		CUR2 = 0xB0 + 0x10,
		HRM2 = 0xB0 + 0x11,
		AFIR = 0xB0 + 0x12,
		HEAL = 0xB0 + 0x13,
		FIR2 = 0xB0 + 0x14,
		HOLD = 0xB0 + 0x15,
		LIT2 = 0xB0 + 0x16,
		LOK2 = 0xB0 + 0x17,
		PURE = 0xB0 + 0x18,
		FEAR = 0xB0 + 0x19,
		AICE = 0xB0 + 0x1A,
		AMUT = 0xB0 + 0x1B,
		SLP2 = 0xB0 + 0x1C,
		FAST = 0xB0 + 0x1D,
		CONF = 0xB0 + 0x1E,
		ICE2 = 0xB0 + 0x1F,
		CUR3 = 0xB0 + 0x20,
		LIFE = 0xB0 + 0x21,
		HRM3 = 0xB0 + 0x22,
		HEL2 = 0xB0 + 0x23,
		FIR3 = 0xB0 + 0x24,
		BANE = 0xB0 + 0x25,
		WARP = 0xB0 + 0x26,
		SLO2 = 0xB0 + 0x27,
		SOFT = 0xB0 + 0x28,
		EXIT = 0xB0 + 0x29,
		FOG2 = 0xB0 + 0x2A,
		INV2 = 0xB0 + 0x2B,
		LIT3 = 0xB0 + 0x2C,
		RUB = 0xB0 + 0x2D,
		QAKE = 0xB0 + 0x2E,
		STUN = 0xB0 + 0x2F,
		CUR4 = 0xB0 + 0x30,
		HRM4 = 0xB0 + 0x31,
		ARUB = 0xB0 + 0x32,
		HEL3 = 0xB0 + 0x33,
		ICE3 = 0xB0 + 0x34,
		BRAK = 0xB0 + 0x35,
		SABR = 0xB0 + 0x36,
		BLND = 0xB0 + 0x37,
		LIF2 = 0xB0 + 0x38,
		FADE = 0xB0 + 0x39,
		WALL = 0xB0 + 0x3A,
		XFER = 0xB0 + 0x3B,
		NUKE = 0xB0 + 0x3C,
		STOP = 0xB0 + 0x3D,
		ZAP = 0xB0 + 0x3E,
		XXXX = 0xB0 + 0x3F
	}

public static class UnsramIndex
	{
		public const byte ShipVis = 0;
		public const byte ShipX = 1;
		public const byte ShipY = 2;
		public const byte AirshipVis = 4;
		public const byte AirshipX = 5;
		public const byte AirshipY = 6;
		public const byte BridgeVis = 8;
		public const byte BridgeX = 9;
		public const byte BridgeY = 10;
		public const byte CanalVis = 12;
		public const byte CanalX = 13;
		public const byte CanalY = 14;
		public const byte OverworldScrollX = 16;
		//OverworldScrollY = 17
		//HasCanoe = 18 // Replaced by Items.Canoe
		//Vehicle = 20
		public const byte BridgeScene = 22;
		//Gold = 28
		public const byte ItemsBaseForNPC = 32;
	}

	public enum ObjectId : byte
	{
		None = 0,
		King = 1,
		Garland = 2,
		Princess1 = 3,
		Bikke = 4,
		ElfDoc = 5,
		ElfPrince = 6,
		Astos = 7,
		Nerrick = 8,
		Smith = 9,
		Matoya = 10,
		Unne = 11,
		Vampire = 12,
		Sarda = 13,
		Bahamut = 14,
		Lefein = 15, // Move Up Lefein Guy. Unused in the original (seems to be a copy of BlackOrb).
		WarMECH = 187, // This is for WarMECH NPC. Take the place of Lefein.

		SubEngineer = 16,
		CubeBot = 17,
		Princess2 = 18,
		Fairy = 19,
		Titan = 20,
		CanoeSage = 21,
		RodPlate = 22,
		LutePlate = 23,

		LichOrb = 27,

		SkyWarrior1 = 58,
		SkyWarrior2 = 59,
		SkyWarrior3 = 60,
		SkyWarrior4 = 61,
		SkyWarrior5 = 62,
		Pirate1 = 63,
		Pirate2 = 64,
		Pirate3 = 65,

		Bat = 87,

		AirshipPerson = 113,

		EarthFirePerson = 128,

		BlackOrb = 202,


		ConeriaCastle1FGuard1 = 32,
		ConeriaCastle1FWoman1 = 34,
		ConeriaCastle1FScholar = 35,
		ConeriaCastle1FGuard2 = 37,
		ConeriaCastle1FWoman2 = 38,
		ConeriaCastle1FRoyal1 = 41,
		ConeriaCastle1FRoyal2 = 42,
		ConeriaCastle1FOldMan1 = 44,
		ConeriaCastle1FOldMan2 = 46,

		ConeriaCastle2FGuard1 = 43,
		ConeriaCastle2FGuard2 = 48,

		ConeriaGuard1 = 49,
		ConeriaGuard2 = 50,
		ConeriaOldMan = 52,
		ConeriaDancer = 53,
		ConeriaWoman1 = 54,
		ConeriaOldWoman = 55,
		ConeriaWoman2 = 56,
		ConeriaMan = 57,

		// Sky Warriors

		PravokaMan1 = 63,
		PravokaOldMan = 64,
		PravokaWoman = 65,
		PravokaPunk = 66,
		PravokaMan2 = 67,

		ElflandCastleElf1 = 69,
		ElflandCastleElf2 = 70,
		ElflandCastleElf3 = 71,
		ElflandCastleScholar1 = 72,
		ElflandCastleElf4 = 73,
		ElflandCastleElf5 = 74,

		ElflandElf1 = 77,
		ElflandElf2 = 78,
		ElflandElf3 = 79,
		ElflandElf4 = 80,
		ElflandElf5 = 81,
		ElflandElf6 = 82,
		ElflandScholar1 = 83,
		ElflandScholar2 = 84,

		// More Elf
		// Bat

		DwarfcaveDwarf1 = 88,
		DwarfcaveDwarf2 = 89,
		DwarfcaveDwarf3 = 90,
		DwarfcaveDwarf4 = 91,
		DwarfcaveDwarf5 = 92,
		DwarfcaveDwarf6 = 93,
		DwarfcaveDwarf7 = 94,
		DwarfcaveDwarf8 = 95,
		DwarfcaveDwarf9 = 96,
		DwarfcaveDwarf10 = 97,
		DwarfcaveDwarfHurray = 99,

		MatoyaBroom1 = 100,
		MatoyaBroom2 = 101,
		MatoyaBroom3 = 102,
		MatoyaBroom4 = 103,

		MelmondMan1 = 104,
		MelmondMan2 = 105,
		MelmondMan3 = 106,
		MelmondOldMan1 = 107,
		MelmondMan4 = 108,
		MelmondMan5 = 109,
		MelmondOldMan2 = 110,
		MelmondMan6 = 111,
		MelmondMan7 = 112,
		MelmondMan8 = 113,
		MelmondWoman1 = 114,
		MelmondWoman2 = 115,
		MelmondDwarf = 116,

		Unused1 = 118,
		// 117,118 What?

		CrescentSage1 = 119,
		CrescentSage2 = 120,
		CrescentSage3 = 121,
		CrescentSage4 = 122,
		CrescentSage5 = 123,
		CrescentSage6 = 124,
		CrescentSage7 = 125,
		CrescentSage8 = 126,
		CrescentSage9 = 127,
		CrescentSage10 = 128,
		CrescentMan = 129,
		CrescentSage11 = 130,
		CrescentWoman = 131,

		CastleOrdealsOldMan = 132,

		CardiaDragon1 = 133,
		CardiaDragon2 = 134,
		CardiaDragon3 = 135,
		CardiaDragon4 = 136,
		CardiaDragon5 = 137,
		CardiaDragon6 = 138,
		CardiaDragon7 = 139,
		CardiaDragon8 = 140,
		CardiaDragon9 = 141,
		CardiaDragon10 = 142,
		CardiaDragon11 = 143,
		CardiaDragon12 = 144,

		Tomb = 145,
		Well = 146,

		OnracWoman1 = 147,
		OnracScholar = 148,
		OnracGuard = 149,
		OnracWitch = 150,
		OnracDancer = 151,
		OnracPunk1 = 152,
		OnracOldMan1 = 153,
		OnracOldMan2 = 154,
		OnracDragon = 155,
		OnracPirate = 156,
		OnracPunk2 = 157,
		OnracWoman2 = 158,
		OnracMan1 = 159,
		OnracMan2 = 160,

		SwordSample = 161,
		BadCubeBot = 162,

		Mermaid1 = 163,
		Mermaid2 = 164,
		Mermaid3 = 165,
		Mermaid4 = 166,
		Mermaid5 = 167,
		Mermaid6 = 168,
		Mermaid7 = 169,
		Mermaid8 = 170,
		Mermaid9 = 171,
		Mermaid10 = 172,

		LockedDoor = 173,

		GaiaMan1 = 174,
		GaiaScholar1 = 175,
		GaiaScholar2 = 176,
		GaiaWoman = 177,
		GaiaDancer = 178,
		GaiaScholar3 = 179,
		GaiaMan2 = 180,
		GaiaMan3 = 181,
		GaiaPirate = 182,
		GaiaMan4 = 183,
		GaiaBroom = 184,
		GaiaWitch = 185,
		GaiaOldWoman = 186,

		// LefeinChime

		LefeinMan1 = 188,
		LefeinMan2 = 189,
		LefeinMan3 = 190,
		LefeinMan4 = 191,
		LefeinMan5 = 192,
		LefeinMan6 = 193,
		LefeinMan7 = 194,
		LefeinMan8 = 195,
		LefeinMan9 = 196,
		LefeinMan10 = 197,
		LefeinMan11 = 198,
		LefeinMan12 = 200,
		LefeinMan13 = 201,

		MirageRobot1 = 204,
		MirageRobot2 = 205,
		MirageRobot3 = 206,
		SkyRobot = 207,
	}
	[Flags]
	public enum MapChange
	{
		None = 0x00,
		Bridge = 0x01,
		Ship = 0x02,
		Canal = 0x04, // As a general rule we can assume the ship cannot require the canal
		TitanFed = 0x08,
		Canoe = 0x10,
		Airship = 0x20,
		Chime = 0x40,
		All = 0x7F
	}

	public static class Dock
	{
		public static byte[] Coneria = new byte[] { 0x98, 0xA9 }; //152, 169
		public static byte[] DwarfCave = new byte[] { 0x79, 0x8B }; //121, 139
		public static byte[] MatoyasCave = new byte[] { 0x9E, 0x8E }; //158, 142
		public static byte[] Pravoka = new byte[] { 0xD2, 0x99 }; //210, 153
		public static byte[] Elfland = new byte[] { 0x8D, 0xD3 }; //141, 211
	};

	public static class ItemExtensions
	{
		public static AccessRequirement ToAccessRequirement(this Item item)
		{
			switch (item)
			{
				case Item.None: return AccessRequirement.None;
				case Item.Crown: return AccessRequirement.Crown;
				case Item.Crystal: return AccessRequirement.Crystal;
				case Item.Herb: return AccessRequirement.Herb;
				case Item.Key: return AccessRequirement.Key;
				case Item.Tnt: return AccessRequirement.Tnt;
				case Item.Adamant: return AccessRequirement.Adamant;
				case Item.Slab: return AccessRequirement.Slab;
				case Item.Ruby: return AccessRequirement.Ruby;
				case Item.Bottle: return AccessRequirement.Bottle;
				case Item.EarthOrb: return AccessRequirement.EarthOrb;
				default: return AccessRequirement.None;
			}
		}
	}
}
