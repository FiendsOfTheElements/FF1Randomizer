using System;
using System.Collections.Generic;
using System.Linq;

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
		IronSword = 38,
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
		//Airship = 228,
		Bridge = 232,
		Canal = 236,
		Canoe = 242
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
		//OverworldScrollX = 16
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

		WarMECH = 15, // This is for WarMECH NPC.  Unused in the original (seems to be a copy of BlackOrb).

		SubEngineer = 16,
		CubeBot = 17,
		Princess2 = 18,
		Fairy = 19,
		Titan = 20,
		CanoeSage = 21,
		RodPlate = 22,
		LutePlate = 23,

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

		Lefein = 187,

		BlackOrb = 202
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
		public static byte[] Coneria = new byte[] { 0x98, 0xA9 };
		public static byte[] DwarfCave = new byte[] { 0x79, 0x8B };
		public static byte[] MatoyasCave = new byte[] { 0x9E, 0x8E };
		public static byte[] Pravoka = new byte[] { 0xD2, 0x99 };
		public static byte[] Elfland = new byte[] { 0x8D, 0xD3 };
	};
}
