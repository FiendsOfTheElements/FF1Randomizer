namespace FF1Lib
{
	public struct ItemLogic
	{
		public ItemType Type { get; private set; }
		public ItemQuality Quality { get; private set; }
		public Item Index { get; private set; }

		public ItemLogic(Item _index, ItemType _type, ItemQuality _quality)
		{
			Index = _index;
			Type = _type;
			Quality = _quality;
		}
		public ItemLogic(ItemLogic newItem)
		{
			Index = newItem.Index;
			Type = newItem.Type;
			Quality = newItem.Quality;
		}
	}
	/*public class ItemsLogicData
	{
		private List<ItemLogic> items;
		private FF1Rom rom;
		private List<int> UnusedGoldItems = new List<int> { 110, 111, 112, 113, 114, 116, 120, 121, 122, 124, 125, 127, 132, 158, 165, 166, 167, 168, 170, 171, 172 };

		private const int TreasureOffset = 0x03100;
		private const int TreasureSize = 1;
		private const int TreasurePoolCount = 256;
		private const int TreasureCount = 256;
		public ItemsLogicData(FF1Rom _rom)
		{
			rom = _rom;
			//var pricevalue = new();
			//var name = new();

			//var test = new List<int>


			var treasureBlob = rom.Get(TreasureOffset, TreasureSize * TreasureCount).ToBytes().Select(t => (Item)t).ToList();
			//Item shopItem = Item.Bottle;
			List<Item> npcItems = new() { Item.Lute, Item.Bridge, Item.Ship, Item.Crystal, Item.Herb, Item.Key, Item.Canal, Item.Rod, Item.Canoe, Item.Oxyale, Item.Cube, Item.Chime, Item.Xcalber };






			// list of items, and then we reference the actual values
			// all items have a name, an id, a price value

			// weapon
			//  
			// armor
			// spells




		}





	}*/

	public enum ItemType
	{
		None,
		Consumable,
		ExtConsumable,
		KeyItem,
		Orb,
		Shard,
		Gold,
		Xp,
		Weapon,
		Armor,
	}
	public enum ItemQuality
	{
		Common,
		Rare,
		Legendary,
		Unique
	}
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

		// Extra Items, these IDs must be process before being written
		Xp = 0xB0,
		ApItem = 0xB2,
		FullCure = 0xC0,
		PhoenixDown = 0xC1,
		Blast = 0xC2,
		Smoke = 0xC3,
		Refresh = 0xC4,
		Flare = 0xC5,
		Black = 0xC6,
		Guard = 0xC7,
		Quick = 0xC8,
		High = 0xC9,
		Wizard = 0xCA,
		Cloak = 0xCB,

		// Additions for variables in chests / 0xE0
		Ship = 224,
		Airship = 228,
		Bridge = 232,
		Canal = 236,
		Canoe = 242,
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
		None = 0,					//0x00
		King = 1,					//0x01
		Garland = 2,				//0x02
		Princess1 = 3,				//0x03
		Bikke = 4,					//0x04
		ElfDoc = 5,					//0x05
		ElfPrince = 6,				//0x06
		Astos = 7,					//0x07
		Nerrick = 8,				//0x08
		Smith = 9,					//0x09
		Matoya = 10,				//0x0A
		Unne = 11,					//0x0B
		Vampire = 12,				//0x0C
		Sarda = 13,					//0x0D
		Bahamut = 14,				//0x0E
		Lefein = 15, 				//0x0F Move Up Lefein Guy. Unused in the original (seems to be a copy of BlackOrb).
		WarMECH = 187, 				//0xBB This is for WarMECH NPC. Take the place of Lefein.

		SubEngineer = 16,			//0x10
		CubeBot = 17,				//0x11
		Princess2 = 18,				//0x12
		Fairy = 19,					//0x13
		Titan = 20,					//0x14
		CanoeSage = 21,				//0x15
		RodPlate = 22,				//0x16
		LutePlate = 23,				//0x17

		Chaos1 = 24,				//0x18
		Chaos2 = 25,				//0x19
		Chaos3 = 26,				//0x1A


		LichOrb = 27,				//0x1B
		KaryOrb = 28,				//0x1C
		KrakenOrb = 29,				//0x1D
		TiamatOrb = 30,				//0x1E

		SkyWarrior1 = 58,			//0x3A
		SkyWarrior2 = 59,			//0x3B
		SkyWarrior3 = 60,			//0x3C
		SkyWarrior4 = 61,			//0x3D
		SkyWarrior5 = 62,			//0x3E
		Pirate1 = 63,				//0x3F
		Pirate2 = 64,				//0x40
		Pirate3 = 65,				//0x41

		Bat = 87,					//0x57

		AirshipPerson = 113,		//0x71

		EarthFirePerson = 128,		//0x80

		BlackOrb = 202,				//0xCA


		ConeriaCastle1FGuard1 = 32,	//0x20
		ConeriaCastle1FWoman1 = 34, //0x22
		ConeriaCastle1FScholar = 35,//0x23
		ConeriaCastle1FGuard2 = 37,	//0x25
		ConeriaCastle1FWoman2 = 38,	//0x26
		ConeriaCastle1FRoyal1 = 41,	//0x29
		ConeriaCastle1FRoyal2 = 42,	//0x2A
		ConeriaCastle1FOldMan1 = 44,//0x2C
		ConeriaCastle1FOldMan2 = 46,//0x2E

		ConeriaCastle2FGuard1 = 43, //0x2B
		ConeriaCastle2FGuard2 = 48,	//0x30

		ConeriaGuard1 = 49,			//0x31
		ConeriaGuard2 = 50,			//0x32
		ConeriaOldMan = 52,			//0x34
		ConeriaDancer = 53,			//0x35
		ConeriaWoman1 = 54,			//0x36
		ConeriaOldWoman = 55,		//0x37
		ConeriaWoman2 = 56,			//0x38
		ConeriaMan = 57,			//0x39

		// Sky Warriors

		PravokaMan1 = 63,			//0x3F
		PravokaOldMan = 64,			//0x40
		PravokaWoman = 65,			//0x41
		PravokaPunk = 66,			//0x42
		PravokaMan2 = 67,			//0x43

		ElflandCastleElf1 = 69,		//0x45
		ElflandCastleElf2 = 70,		//0x46
		ElflandCastleElf3 = 71,		//0x47
		ElflandCastleScholar1 = 72,	//0x48
		ElflandCastleElf4 = 73,		//0x49
		ElflandCastleElf5 = 74,		//0x4A

		ElflandElf1 = 77,			//0x4D
		ElflandElf2 = 78,			//0x4E
		ElflandElf3 = 79,			//0x4F
		ElflandElf4 = 80,			//0x50
		ElflandElf5 = 81,			//0x51
		ElflandElf6 = 82,			//0x52
		ElflandScholar1 = 83,		//0x53
		ElflandScholar2 = 84,		//0x54

		// More Elf
		// Bat

		DwarfcaveDwarf1 = 88,		//0x58
		DwarfcaveDwarf2 = 89,		//0x59
		DwarfcaveDwarf3 = 90,		//0x5A
		DwarfcaveDwarf4 = 91,		//0x5B
		DwarfcaveDwarf5 = 92,		//0x5C
		DwarfcaveDwarf6 = 93,		//0x5D
		DwarfcaveDwarf7 = 94,		//0x5E
		DwarfcaveDwarf8 = 95,		//0x5F
		DwarfcaveDwarf9 = 96,		//0x60
		DwarfcaveDwarf10 = 97,		//0x61
		DwarfcaveDwarfHurray = 99,	//0x63

		MatoyaBroom1 = 100,			//0x64
		MatoyaBroom2 = 101,			//0x65
		MatoyaBroom3 = 102,			//0x66
		MatoyaBroom4 = 103,			//0x67

		MelmondMan1 = 104,			//0x68
		MelmondMan2 = 105,			//0x69
		MelmondMan3 = 106,			//0x6A
		MelmondOldMan1 = 107,		//0x6B
		MelmondMan4 = 108,			//0x6C
		MelmondMan5 = 109,			//0x6D
		MelmondOldMan2 = 110,		//0x6E
		MelmondMan6 = 111,			//0x6F
		MelmondMan7 = 112,			//0x70
		MelmondMan8 = 113,			//0x71
		MelmondWoman1 = 114,		//0x72
		MelmondWoman2 = 115,		//0x73
		MelmondDwarf = 116,			//0x74

		Unused1 = 118,				//0x76
		// 117,118 What?

		CrescentSage1 = 119,		//0x77
		CrescentSage2 = 120,		//0x78
		CrescentSage3 = 121,		//0x79
		CrescentSage4 = 122,		//0x7A
		CrescentSage5 = 123,		//0x7B
		CrescentSage6 = 124,		//0x7C
		CrescentSage7 = 125,		//0x7D
		CrescentSage8 = 126,		//0x7E
		CrescentSage9 = 127,		//0x7F
		CrescentSage10 = 128,		//0x80
		CrescentMan = 129,			//0x81
		CrescentSage11 = 130,		//0x82
		CrescentWoman = 131,		//0x83

		CastleOrdealsOldMan = 132,	//0x84

		CardiaDragon1 = 133,		//0x85
		CardiaDragon2 = 134,		//0x86
		CardiaDragon3 = 135,		//0x87
		CardiaDragon4 = 136,		//0x88
		CardiaDragon5 = 137,		//0x89
		CardiaDragon6 = 138,		//0x8A
		CardiaDragon7 = 139,		//0x8B
		CardiaDragon8 = 140,		//0x8C
		CardiaDragon9 = 141,		//0x8D
		CardiaDragon10 = 142,		//0x8E
		CardiaDragon11 = 143,		//0x8F
		CardiaDragon12 = 144,		//0x90

		Tomb = 145,					//0x91
		Well = 146,					//0x92

		OnracWoman1 = 147,			//0x93
		OnracScholar = 148,			//0x94
		OnracGuard = 149,			//0x95
		OnracWitch = 150,			//0x96
		OnracDancer = 151,			//0x97
		OnracPunk1 = 152,			//0x98
		OnracOldMan1 = 153,			//0x99
		OnracOldMan2 = 154,			//0x9A
		OnracDragon = 155,			//0x9B
		OnracPirate = 156,			//0x9C
		OnracPunk2 = 157,			//0x9D
		OnracWoman2 = 158,			//0x9E
		OnracMan1 = 159,			//0x9F
		OnracMan2 = 160,			//0xA0

		SwordSample = 161,			//0xA1
		BadCubeBot = 162,			//0xA2

		Mermaid1 = 163,				//0xA3
		Mermaid2 = 164,				//0xA4
		Mermaid3 = 165,				//0xA5
		Mermaid4 = 166,				//0xA6
		Mermaid5 = 167,				//0xA7
		Mermaid6 = 168,				//0xA8
		Mermaid7 = 169,				//0xA9
		Mermaid8 = 170,				//0xAA
		Mermaid9 = 171,				//0xAB
		Mermaid10 = 172,			//0xAC

		LockedDoor = 173,			//0xAD

		GaiaMan1 = 174,				//0xAE
		GaiaScholar1 = 175,			//0xAF
		GaiaScholar2 = 176,			//0xB0
		GaiaWoman = 177,			//0xB1
		GaiaDancer = 178,			//0xB2
		GaiaScholar3 = 179,			//0xB3
		GaiaMan2 = 180,				//0xB4
		GaiaMan3 = 181,				//0xB5
		GaiaPirate = 182,			//0xB6
		GaiaMan4 = 183,				//0xB7
		GaiaBroom = 184,			//0xB8
		GaiaWitch = 185,			//0xB9
		GaiaOldWoman = 186,			//0xBA

		// LefeinChime

		LefeinMan1 = 188,			//0xBC
		LefeinMan2 = 189,			//0xBD
		LefeinMan3 = 190,			//0xBE
		LefeinMan4 = 191,			//0xBF
		LefeinMan5 = 192,			//0xC0
		LefeinMan6 = 193,			//0xC1
		LefeinMan7 = 194,			//0xC2
		LefeinMan8 = 195,			//0xC3
		LefeinMan9 = 196,			//0xC4
		LefeinMan10 = 197,			//0xC5
		LefeinMan11 = 198,			//0xC6
		LefeinMan12 = 200,			//0xC8
		LefeinMan13 = 201,			//0xC9

		MirageRobot1 = 204,			//0xCC
		MirageRobot2 = 205,			//0xCD
		MirageRobot3 = 206,			//0xCE
		SkyRobot = 207,				//0xCF

		// these are set when these items are revealed on screen for the first time
		// used in the tracker in order not to reveal tri-state flags
		UnlockedDoorRevealed = 249, //0xF9
		BridgeRevealed = 250,		//0xFA
		CanalRevealed = 251,		//0xFB
		ShipRevealed = 252,			//0xFC
		AirshipRevealed = 253,		//0xFD
		CanoeRevealed = 254,		//0xFE

		// set when key item purchased from a shop
		ShopItem = 255,				//0xFF
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
		public static byte[] GaiaDrydock = new byte[] { 0xD3, 0x1F }; //211, 31
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
