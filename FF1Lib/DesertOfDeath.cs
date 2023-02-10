namespace FF1Lib
{
	public partial class FF1Rom
	{
		public void EnableDamageTile()
		{
			// Allow tiles to do Walk Damage, see 1E_B000_DoWalkDamage.asm
			PutInBank(0x1E, 0xB000, Blob.FromHex("A542C901D00E20FBC7A54429E0C9E0D0034CDEC760"));
			PutInBank(0x1F, 0xC33C, Blob.FromHex("A91E2003FE4C00B0"));

			PutInBank(0x0E, 0xB267, Blob.FromHex("E0")); // Expand OWTP_SPEC_MASK to 0b1110_0000
			PutInBank(0x1F, 0xC4DA, Blob.FromHex("E0"));
			PutInBank(0x1F, 0xC6B4, Blob.FromHex("E0"));

			PutInBank(0x1F, 0xC2EC, Blob.FromHex("E1")); // Update mask for docking

			List<int> owDamageTileList = new() { 0x45, 0x55, 0x62, 0x63, 0x72, 0x73 };

			foreach (var tile in owDamageTileList)
			{
				var tileProperty = Get(0x0000 + tile * 2, 1);
				Put(0x0000 + tile * 2, new byte[] { (byte)(tileProperty[0] | 0b1110_0000) });
			}
		}

		public void DamageTilesKill(bool saveonDeath)
		{
			// See 1E_B100_DamageTilesKill.asm
			string saveOnDeathJump = "4C12C0";
			if (saveonDeath)
			{
				saveOnDeathJump = "EAEAEA";
			}

			PutInBank(0x1E, 0xB100, Blob.FromHex($"A200A000BD0161C901F060A901D043A5F2C903903DA548C90EF013C921F00FC922F00BC923F007C924F0034C38B1A98820FDB1D01D4C6CB1C90FF00CC925F008C926F004C927F000A98720FDB1F0034C6CB1BD0B61D021BD0A61C902B01AA9019D0161A9009D0A614C6BB1C88A186940AAD091C004F01560BD0A6138E9019D0A61BD0B61E9009D0B614C6CB1A980854BA91E8557A52D6AA9009002A902851C2000FE2082D920B3D9A9064820EFD968A88898D0F6A0402000FE2018D92082D9A91E8D01202089C688D0ECA952854B20EFB1A9008D00208D0120{saveOnDeathJump}A9C048A91148A98F48A9F748A91B85574C03FE2028D8482000FE2089C668F0F360841185108AA8B91C61C510D005A411A90060C8982904C904D0ECA411A90160"));
			//Original code below - above edit is to prevent a bug with adjustable lava damage (spaces added to line up cleanly with the new code for comparison)
			//PutInBank(0x1E, 0xB100, Blob.FromHex($"A200A000BD0161C901F026BD0B61D00DBD0A61C902B006A9019D0161    C8          BD0A6138E9019D0A61BD0B61E9009D0B614C32B1C88A186940AAD0CBC004F00160A980854BA91E8557A52D6AA9009002A902851C2000FE2082D920B3D9A9064820EFD968A88898D0F6A0402000FE2018D92082D9A91E8D01202089C688D0ECA952854B20A1B1A9008D00208D0120{saveOnDeathJump}A9C048A91148A98F48A9F748A91B85574C03FE2028D8482000FE2089C668F0F360"));
			PutInBank(0x1F, 0xC861, Blob.FromHex("A91E2003FE4C00B1"));
		}

		public void AdjustDamageTileDamage(int DamageTileAmount, bool isDamageTilesKillOn, bool IsArmorResistsDamageTileDamageOn)
		{
			// Overwrites a bit of the newly created DamageTilesKill assembly code to account for the adjustable damage
			if (isDamageTilesKillOn) {
				PutInBank(0x1E, 0xB155, Blob.FromHex($"{DamageTileAmount + 1:X2}"));
				PutInBank(0x1E, 0xB16A, Blob.FromHex($"{DamageTileAmount:X2}"));
			}
			else if (IsArmorResistsDamageTileDamageOn) {
				PutInBank(0x1E, 0xB14C, Blob.FromHex($"{DamageTileAmount + 1:X2}"));
				PutInBank(0x1E, 0xB15C, Blob.FromHex($"{DamageTileAmount:X2}"));
			}
			// No Lethal Damage Tiles Flag, overwrite normal rom code instead
			else {
				Data[0x7C86C] = (byte)(DamageTileAmount + 1); //"HP Less than" check so it doesn't kill
				Data[0x7C874] = (byte)(DamageTileAmount);
			}
		}

		public void EnableArmorDamageTileResist(bool isDamageTilesKillOn)
		{
			// See 1E_B100_DamageTilesArmorResist.asm
			if (isDamageTilesKillOn) {
				// DamageTilesKill already has the necessary code inline, so just enable it
				PutInBank(0x1E, 0xB10C, Blob.FromHex($"00"));
			} else {
				PutInBank(0x1F, 0xC861, Blob.FromHex("A91E2003FE4C00B1"));
				PutInBank(0x1E, 0xB100, Blob.FromHex($"A200A901D043A5F2C903903DA548C90EF013C921F00FC922F00BC923F007C924F0034C2FB1A9882076B1D01D4C6EB1C90FF00CC925F008C926F004C927F000A9872076B1F0034C6EB1BD0B61D00FBD0A61C902B008A9019D0A614C6EB1BD0A6138E9019D0A61BD0B61E9009D0B618A186940AAD08D6085108AA8B91C61C510D003A90060C8982904C904D0EEA90160"));
				PutInBank(0x1E, 0xB103, Blob.FromHex($"00"));
			}
		}
	}

	public class DesertOfDeath
	{
		
		//public enum MapLocation = Enum.GetValues(typeof(FF1Lib.MapLocation));
		public enum MapDirection : int
		{
			North = 0,
			NorthWest = 1,
			West = 2,
			SouthWest = 3,
			South = 4,
			SouthEast = 5,
			East = 6,
			NorthEast = 7
		}

		public enum DoodadsType : int
		{
			CactusField = 0,
			RockField = 1,
			Mountains = 2,
			Oasis = 3,
			CactuarField = 4,
		}

		public enum OWTile : byte
		{
			MountainUpperLeft = 0x10,
			MountainUpperMiddle = 0x11,
			MountainUpperRight = 0x12,
			MountainMiddleLeft = 0x20,
			MountainCenter = 0x21,
			MountainMiddleRight = 0x22,
			MountainLowerLeft = 0x30,
			MountainLowerMiddle = 0x31,
			MountainLowerRight = 0x33,
			RiverUpperLeft = 0x40,
			RiverUpperRight = 0x41,
			RiverLowerLeft = 0x50,
			RiverLowerRight = 0x51,
			RiverCenter = 0x44,
			Desert = 0x45,
			DesertCactuar = 0x03,
			DesertCactus = 0x04,
			DesertCacti = 0x05,
			DesertPalmTree = 0x13,
			DesertRocks = 0x14,
			DesertRock = 0x15,
		}

		public static List<(MapLocation, OverworldTeleportIndex)> MapLocationToOWTeleporterIndex = new()
		{
			(MapLocation.ConeriaCastle1, OverworldTeleportIndex.ConeriaCastle1),
			(MapLocation.Coneria, OverworldTeleportIndex.Coneria),
			(MapLocation.EarthCave1, OverworldTeleportIndex.EarthCave1),
			(MapLocation.ElflandCastle, OverworldTeleportIndex.ElflandCastle),
			(MapLocation.Elfland, OverworldTeleportIndex.Elfland),
			(MapLocation.MirageTower1, OverworldTeleportIndex.MirageTower1),
			(MapLocation.NorthwestCastle, OverworldTeleportIndex.NorthwestCastle),
			(MapLocation.IceCave1, OverworldTeleportIndex.IceCave1),
			(MapLocation.DwarfCave, OverworldTeleportIndex.DwarfCave),
			(MapLocation.MatoyasCave, OverworldTeleportIndex.MatoyasCave),
			(MapLocation.TitansTunnelEast, OverworldTeleportIndex.TitansTunnelEast),
			(MapLocation.TitansTunnelWest, OverworldTeleportIndex.TitansTunnelWest),
			(MapLocation.CastleOrdeals1, OverworldTeleportIndex.CastleOrdeals1),
			(MapLocation.SardasCave, OverworldTeleportIndex.SardasCave),
			(MapLocation.Waterfall, OverworldTeleportIndex.Waterfall),
			(MapLocation.Pravoka, OverworldTeleportIndex.Pravoka),
			(MapLocation.CrescentLake, OverworldTeleportIndex.CrescentLake),
			(MapLocation.TempleOfFiends1, OverworldTeleportIndex.TempleOfFiends1),
			(MapLocation.Gaia, OverworldTeleportIndex.Gaia),
			(MapLocation.Onrac, OverworldTeleportIndex.Onrac),
			(MapLocation.GurguVolcano1, OverworldTeleportIndex.GurguVolcano1),
			(MapLocation.Cardia2, OverworldTeleportIndex.Cardia2),
			(MapLocation.Cardia4, OverworldTeleportIndex.Cardia4),
			(MapLocation.Cardia5, OverworldTeleportIndex.Cardia5),
			(MapLocation.Cardia6, OverworldTeleportIndex.Cardia6),
			(MapLocation.Cardia1, OverworldTeleportIndex.Cardia1),
			(MapLocation.BahamutCave1, OverworldTeleportIndex.BahamutCave1),
			(MapLocation.Lefein, OverworldTeleportIndex.Lefein),
			(MapLocation.MarshCave1, OverworldTeleportIndex.MarshCave1),
		};

		public static List<(MapLocation, byte)> OWTileLink = new()
		{
			(MapLocation.ConeriaCastle1, 0x01),
			(MapLocation.EarthCave1, 0x0E),
			(MapLocation.ElflandCastle, 0x1B),
			(MapLocation.MirageTower1, 0x1D),
			(MapLocation.NorthwestCastle, 0x29),
			(MapLocation.IceCave1, 0x2B),
			(MapLocation.DwarfCave, 0x2F),
			(MapLocation.MatoyasCave, 0x32),
			(MapLocation.TitansTunnelEast, 0x34),
			(MapLocation.TitansTunnelWest, 0x35),
			(MapLocation.Caravan, 0x36),
			(MapLocation.CastleOrdeals1, 0x38),
			(MapLocation.SardasCave, 0x3A),
			(MapLocation.Waterfall, 0x46),
			(MapLocation.Coneria, 0x49),
			(MapLocation.Pravoka, 0x4A),
			(MapLocation.Elfland, 0x4C),
			(MapLocation.Melmond, 0x4D),
			(MapLocation.CrescentLake, 0x4E),
			(MapLocation.TempleOfFiends1, 0x57),
			(MapLocation.Gaia, 0x5A),
			(MapLocation.Onrac, 0x5D),
			(MapLocation.GurguVolcano1, 0x64),
			(MapLocation.Cardia2, 0x66),
			(MapLocation.Cardia4, 0x67),
			(MapLocation.Cardia5, 0x68),
			(MapLocation.Cardia6, 0x69),
			(MapLocation.Cardia1, 0x6A),
			(MapLocation.BahamutCave1, 0x6C),
			(MapLocation.Lefein, 0x6D),
			(MapLocation.MarshCave1, 0x6E),
		};

		public static List<(MapLocation, List<List<byte>>)> MapModifications = new ()
		{
			(MapLocation.ConeriaCastle1, new List<List<byte>>() {
					new List<byte> { 0x45, 0x45, 0x09, 0x0A, 0x45, 0x45 },
					new List<byte> { 0x45, 0x2C, 0x19, 0x1A, 0x2E, 0x45 },
					new List<byte> { 0x3B, 0x3C, 0x01, 0x02, 0x3E, 0x3F },
					new List<byte> { 0x4B, 0x49, 0x3D, 0x3D, 0x49, 0x4F },
					new List<byte> { 0x5B, 0x49, 0x3D, 0x3D, 0x49, 0x5F },
					new List<byte> { 0x6B, 0x49, 0x3D, 0x3D, 0x49, 0x6F },
					new List<byte> { 0x7B, 0x7D, 0x3D, 0x3D, 0x7E, 0x7F },
					new List<byte> { 0x45, 0x45, 0x45, 0x45, 0x45, 0x45 },
			}),
			(MapLocation.TempleOfFiends1, new List<List<byte>>() {
					new List<byte> { 0x45, 0x47, 0x48, 0x45 },
					new List<byte> { 0x56, 0x57, 0x58, 0x59 },
			}),
			(MapLocation.MatoyasCave, new List<List<byte>>() {
					new List<byte> { 0x45, 0x10, 0x11, 0x12 },
					new List<byte> { 0x10, 0x21, 0x21, 0x22 },
					new List<byte> { 0x30, 0x31, 0x32, 0x33 },
					new List<byte> { 0x45, 0x45, 0x45, 0x45 },
			}),
			(MapLocation.Pravoka, new List<List<byte>>() {
					new List<byte> { 0x45, 0x2C, 0x2D, 0x2E, 0x45 },
					new List<byte> { 0x3B, 0x3C, 0x4A, 0x3E, 0x3F },
					new List<byte> { 0x4B, 0x4A, 0x3D, 0x4A, 0x4F },
					new List<byte> { 0x5C, 0x7D, 0x3D, 0x7E, 0x5E },
					new List<byte> { 0x45, 0x45, 0x45, 0x45, 0x45 },
			}),
			(MapLocation.DwarfCave, new List<List<byte>>() {
					new List<byte> { 0x10, 0x11, 0x11, 0x11, 0x12 },
					new List<byte> { 0x20, 0x21, 0x22, 0x30, 0x33 },
					new List<byte> { 0x30, 0x2F, 0x33, 0x45, 0x45 },
					new List<byte> { 0x45, 0x45, 0x45, 0x45, 0x45 },
			}),
			(MapLocation.NorthwestCastle, new List<List<byte>>() {
					new List<byte> { 0x09, 0x0A },
					new List<byte> { 0x29, 0x2A },
			}),
			(MapLocation.ElflandCastle, new List<List<byte>>() {
					new List<byte> { 0x45, 0x0B, 0x0C, 0x45 },
					new List<byte> { 0x4C, 0x1B, 0x1C, 0x4C },
					new List<byte> { 0x4C, 0x45, 0x45, 0x4C },
			}),
			(MapLocation.MarshCave1, new List<List<byte>>() {
					new List<byte> { 0x6E },
			}),
			(MapLocation.Melmond, new List<List<byte>>() {
					new List<byte> { 0x4D, 0x45 },
					new List<byte> { 0x4D, 0x4D },
			}),
			(MapLocation.EarthCave1, new List<List<byte>>() {
					new List<byte> { 0x45, 0x45, 0x10, 0x11, 0x12 },
					new List<byte> { 0x10, 0x11, 0x21, 0x21, 0x33 },
					new List<byte> { 0x20, 0x21, 0x0E, 0x21, 0x12 },
					new List<byte> { 0x30, 0x33, 0x45, 0x30, 0x33 },
					new List<byte> { 0x45, 0x45, 0x45, 0x45, 0x45 },
			}),
			(MapLocation.SardasCave, new List<List<byte>>() {
					new List<byte> { 0x45, 0x10, 0x11, 0x11, 0x11, 0x12, 0x45 },
					new List<byte> { 0x45, 0x30, 0x3A, 0x31, 0x35, 0x21, 0x12 },
					new List<byte> { 0x10, 0x12, 0x45, 0x45, 0x45, 0x30, 0x33 },
					new List<byte> { 0x30, 0x21, 0x11, 0x11, 0x11, 0x11, 0x12 },
					new List<byte> { 0x45, 0x30, 0x31, 0x31, 0x31, 0x34, 0x33 },
					new List<byte> { 0x45, 0x45, 0x45, 0x45, 0x45, 0x45, 0x45 },
			}),
			(MapLocation.CrescentLake, new List<List<byte>>() {
					new List<byte> { 0x45, 0x2C, 0x2D, 0x2E, 0x45 },
					new List<byte> { 0x3B, 0x3C, 0x3D, 0x3E, 0x3F },
					new List<byte> { 0x4B, 0x3D, 0x4E, 0x4E, 0x4F },
					new List<byte> { 0x5B, 0x4E, 0x3D, 0x4E, 0x5F },
					new List<byte> { 0x6B, 0x4E, 0x3D, 0x3D, 0x6F },
					new List<byte> { 0x7B, 0x7D, 0x3D, 0x7E, 0x7F },
					new List<byte> { 0x45, 0x45, 0x45, 0x45, 0x45 },
			}),
			(MapLocation.GurguVolcano1, new List<List<byte>>() {
					new List<byte> { 0x64, 0x65 },
					new List<byte> { 0x74, 0x75 },
			}),
			(MapLocation.IceCave1, new List<List<byte>>() {
					new List<byte> { 0x45, 0x10, 0x11, 0x11, 0x11, 0x12 },
					new List<byte> { 0x45, 0x30, 0x31, 0x2B, 0x21, 0x22 },
					new List<byte> { 0x45, 0x45, 0x45, 0x45, 0x30, 0x33 },
					new List<byte> { 0x45, 0x10, 0x11, 0x11, 0x11, 0x12 },
					new List<byte> { 0x45, 0x30, 0x31, 0x31, 0x31, 0x33 },
			}),
			(MapLocation.Caravan, new List<List<byte>>() {
					new List<byte> { 0x45, 0x13, 0x13, 0x45 }, // palm tree
					new List<byte> { 0x45, 0x36, 0x40, 0x41 }, // change caravan tile to tent
					new List<byte> { 0x13, 0x45, 0x50, 0x51 },
			}),
			(MapLocation.Onrac, new List<List<byte>>() {
					new List<byte> { 0x5D, 0x5D },
					new List<byte> { 0x5D, 0x5D }, 
			}),
			(MapLocation.Waterfall, new List<List<byte>>() {
					new List<byte> { 0x45, 0x10, 0x11, 0x11, 0x12, 0x45 }, // Find something for waterfall
					new List<byte> { 0x10, 0x21, 0x31, 0x31, 0x21, 0x12 },
					new List<byte> { 0x20, 0x22, 0x40, 0x51, 0x20, 0x22 },
					new List<byte> { 0x20, 0x22, 0x44, 0x10, 0x21, 0x33 },
					new List<byte> { 0x30, 0x31, 0x46, 0x31, 0x33, 0x13 },
					new List<byte> { 0x45, 0x45, 0x45, 0x45, 0x13, 0x45 },
					new List<byte> { 0x45, 0x13, 0x45, 0x45, 0x45, 0x45 },
			}),
			(MapLocation.Cardia1, new List<List<byte>>() {
					new List<byte> { 0x6A },
			}),
			(MapLocation.Cardia2, new List<List<byte>>() {
					new List<byte> { 0x66 },
			}),
			(MapLocation.Cardia4, new List<List<byte>>() {
					new List<byte> { 0x67 },
			}),
			(MapLocation.Cardia5, new List<List<byte>>() {
					new List<byte> { 0x68 },
			}),
			(MapLocation.Cardia6, new List<List<byte>>() {
					new List<byte> { 0x69 },
			}),
			(MapLocation.BahamutCave1, new List<List<byte>>() {
					new List<byte> { 0x6C },
			}),
			(MapLocation.CastleOrdeals1, new List<List<byte>>() {
					new List<byte> { 0x0B, 0x0C },
					new List<byte> { 0x38, 0x39 },
			}),
			(MapLocation.MirageTower1, new List<List<byte>>() {
					new List<byte> { 0x0D, 0x45 },
					new List<byte> { 0x1D, 0x1E },
			}),
			(MapLocation.Gaia, new List<List<byte>>() {
					new List<byte> { 0x45, 0x5A, 0x5A },
					new List<byte> { 0x5A, 0x5A, 0x45 },
			}),
			(MapLocation.Lefein, new List<List<byte>>() {
					new List<byte> { 0x45, 0x2C, 0x2D, 0x2E, 0x45 },
					new List<byte> { 0x3B, 0x3C, 0x6D, 0x3E, 0x3F },
					new List<byte> { 0x4B, 0x6D, 0x6D, 0x6D, 0x4F },
					new List<byte> { 0x5C, 0x7D, 0x3D, 0x7E, 0x5E },
					new List<byte> { 0x45, 0x45, 0x45, 0x45, 0x45 },
			}),
		};

		public static void UpdateOWFormations(FF1Rom rom, int safeX, int safeY )
		{
			var encountersData = new FF1Rom.Encounters(rom);

			// Imp+Iguana / Imp+Sauria
			encountersData.formations[0x06].pattern = FF1Rom.FormationPattern.Mixed;
			encountersData.formations[0x06].spriteSheet = FF1Rom.FormationSpriteSheet.ImpWolfIguanaGiant;
			encountersData.formations[0x06].enemy1 = 0x00;
			encountersData.formations[0x06].enemy2 = 0x08;
			encountersData.formations[0x06].enemy3 = 0x06;
			encountersData.formations[0x06].gfxOffset1 = (int)FF1Rom.FormationGFX.Sprite1;
			encountersData.formations[0x06].gfxOffset2 = (int)FF1Rom.FormationGFX.Sprite3;
			encountersData.formations[0x06].gfxOffset3 = (int)FF1Rom.FormationGFX.Sprite3;
			encountersData.formations[0x06].palette1 = 0x1E;
			encountersData.formations[0x06].palette2 = 0x01;
			encountersData.formations[0x06].paletteAssign1 = 2;
			encountersData.formations[0x06].paletteAssign2 = 1;
			encountersData.formations[0x06].paletteAssign3 = 2;
			encountersData.formations[0x06].minmax1 = (2, 4);
			encountersData.formations[0x06].minmax2 = (0, 0);
			encountersData.formations[0x06].minmax3 = (0, 1);
			encountersData.formations[0x06].minmax4 = (0, 0);
			encountersData.formations[0x06].minmaxB1 = (2, 6);
			encountersData.formations[0x06].minmaxB2 = (1, 2);
			encountersData.formations[0x06].unrunnableA = false;
			encountersData.formations[0x06].unrunnableB = false;
			encountersData.formations[0x06].supriseFactor = 0x04;

			// SandShark / SandShark+Kyzoku
			encountersData.formations[0x0C].pattern = FF1Rom.FormationPattern.Mixed;
			encountersData.formations[0x0C].spriteSheet = FF1Rom.FormationSpriteSheet.SahagPirateSharkBigEye;
			encountersData.formations[0x0C].enemy1 = 0x10;
			encountersData.formations[0x0C].enemy2 = 0x11;
			encountersData.formations[0x0C].gfxOffset1 = (int)FF1Rom.FormationGFX.Sprite2;
			encountersData.formations[0x0C].gfxOffset2 = (int)FF1Rom.FormationGFX.Sprite3;
			encountersData.formations[0x0C].palette1 = 0x30;
			encountersData.formations[0x0C].palette2 = 0x35;
			encountersData.formations[0x0C].paletteAssign1 = 2;
			encountersData.formations[0x0C].paletteAssign2 = 1;
			encountersData.formations[0x0C].minmax1 = (0, 0);
			encountersData.formations[0x0C].minmax2 = (1, 1);
			encountersData.formations[0x0C].minmax3 = (0, 0);
			encountersData.formations[0x0C].minmax4 = (0, 0);
			encountersData.formations[0x0C].minmaxB1 = (3, 4);
			encountersData.formations[0x0C].minmaxB2 = (1, 2);
			encountersData.formations[0x0C].unrunnableA = false;
			encountersData.formations[0x0C].unrunnableB = false;
			encountersData.formations[0x0C].supriseFactor = 0x04;

			// Asp+Scorpion / Cobra+Scorpion
			encountersData.formations[0x5B].pattern = FF1Rom.FormationPattern.Small9;
			encountersData.formations[0x5B].spriteSheet = FF1Rom.FormationSpriteSheet.AspLobsterBullTroll;
			encountersData.formations[0x5B].enemy1 = 0x21;
			encountersData.formations[0x5B].enemy2 = 0x1F;
			encountersData.formations[0x5B].enemy3 = 0x1E;
			encountersData.formations[0x5B].gfxOffset1 = (int)FF1Rom.FormationGFX.Sprite2;
			encountersData.formations[0x5B].gfxOffset2 = (int)FF1Rom.FormationGFX.Sprite1;
			encountersData.formations[0x5B].gfxOffset3 = (int)FF1Rom.FormationGFX.Sprite1;
			encountersData.formations[0x5B].palette1 = 0x3F;
			encountersData.formations[0x5B].palette2 = 0x2B;
			encountersData.formations[0x5B].paletteAssign1 = 1;
			encountersData.formations[0x5B].paletteAssign2 = 2;
			encountersData.formations[0x5B].paletteAssign3 = 1;
			encountersData.formations[0x5B].minmax1 = (0, 4);
			encountersData.formations[0x5B].minmax2 = (0, 0);
			encountersData.formations[0x5B].minmax3 = (1, 4);
			encountersData.formations[0x5B].minmax4 = (0, 0);
			encountersData.formations[0x5B].minmaxB1 = (2, 4);
			encountersData.formations[0x5B].minmaxB2 = (2, 4);
			encountersData.formations[0x5B].unrunnableA = false;
			encountersData.formations[0x5B].unrunnableB = false;
			encountersData.formations[0x5B].supriseFactor = 0x04;

			// Sand Worm / Sand Worm
			encountersData.formations[0x5C].pattern = FF1Rom.FormationPattern.Large4;
			encountersData.formations[0x5C].spriteSheet = FF1Rom.FormationSpriteSheet.ImageGeistWormEye;
			encountersData.formations[0x5C].enemy1 = 0x30;
			encountersData.formations[0x5C].gfxOffset1 = (int)FF1Rom.FormationGFX.Sprite3;
			encountersData.formations[0x5C].palette1 = 0x30;
			encountersData.formations[0x5C].palette2 = 0x30;
			encountersData.formations[0x5C].paletteAssign1 = 1;
			encountersData.formations[0x5C].paletteAssign2 = 1;
			encountersData.formations[0x5C].minmax1 = (1, 1);
			encountersData.formations[0x5C].minmax2 = (0, 0);
			encountersData.formations[0x5C].minmax3 = (0, 0);
			encountersData.formations[0x5C].minmax4 = (0, 0);
			encountersData.formations[0x5C].minmaxB1 = (1, 3);
			encountersData.formations[0x5C].minmaxB2 = (0, 0);
			encountersData.formations[0x5C].unrunnableA = false;
			encountersData.formations[0x5C].unrunnableB = false;
			encountersData.formations[0x5C].supriseFactor = 0x04;

			encountersData.Write(rom);

			// Update overworld domains
			const int DomainCount = 64;

			List<byte> easyDomain = new() { 0x06, 0x06, 0x06, 0x0C, 0x0C, 0x86, 0x86, 0x8C };
			List<byte> hardDomain = new() { 0x86, 0x8C, 0x5B, 0xDB, 0x5C, 0x5C, 0xDB, 0xDC };
			
			var newFormations = new List<byte>();

			List<int> safeDomain = new() {
				(safeX + safeY * 8),
				((safeX + 1) + safeY * 8),
				((safeX + 2) + safeY * 8),
				(safeX + (safeY + 1) * 8),
				((safeX + 1) + (safeY + 1) * 8),
				((safeX + 2) + (safeY + 1) * 8),
				(safeX + (safeY + 2) * 8),
				((safeX + 1) + (safeY + 2) * 8),
				((safeX + 2) + (safeY + 2) * 8),
			};

			for (int i = 0; i < DomainCount; i++)
			{
				if (safeDomain.Contains(i))
				{
					newFormations.AddRange(easyDomain);
				}
				else
				{
					newFormations.AddRange(hardDomain);
				}
			}

			rom.Put(FF1Rom.ZoneFormationsOffset, newFormations.ToArray());

			//Update enemies names
			var enemyText = rom.ReadText(FF1Rom.EnemyTextPointerOffset, FF1Rom.EnemyTextPointerBase, FF1Rom.EnemyCount);

			enemyText[0x10] = "SndMAN"; 
			enemyText[0x11] = "SndSHARK";
			enemyText[0x21] = "SCORP";

			rom.WriteText(enemyText, FF1Rom.EnemyTextPointerOffset, FF1Rom.EnemyTextPointerBase, FF1Rom.EnemyTextOffset);
		}
		public static void DoDUpdateDialogues(FF1Rom rom, FF1Rom.NPCdata npcdata)
		{
			Dictionary<int, string> coneriaDialogues = new();

			coneriaDialogues.Add(0x49, "I've spent my life\nlooking for\nthe YGGDRASIL, the\nmystical ship that can\nride the dunes!");
			coneriaDialogues.Add(0x43, "The Desert is a harsh\nenvironment, each step\ntaken will deplete\nyour strength.");
			coneriaDialogues.Add(0x4B, "The key to not get lost\nin the Desert is to take\nnotes of the landmarks\nthat cross your journey.");

			rom.InsertDialogs(coneriaDialogues);

			npcdata.SetRoutine((ObjectId)0x37, FF1Rom.newTalkRoutines.Talk_norm);
			npcdata.GetTalkArray((ObjectId)0x37)[(int)FF1Rom.TalkArrayPos.dialogue_1] = 0x49;
			npcdata.GetTalkArray((ObjectId)0x37)[(int)FF1Rom.TalkArrayPos.dialogue_2] = 0x49;
			npcdata.GetTalkArray((ObjectId)0x37)[(int)FF1Rom.TalkArrayPos.dialogue_3] = 0x49;
		}
		public List<MapEdit> ConvertTileArrayToMapEdit(List<List<byte>> mapedit, int target_x, int target_y)
		{
			List<MapEdit> convertedMapEdit = new();

			for (int y = 0; y < mapedit.Count; y++)
			{
				for (int x = 0; x < mapedit[y].Count; x++)
				{
					convertedMapEdit.Add(new MapEdit { X = (byte)(x + target_x), Y = (byte)(y + target_y), Tile = (byte)mapedit[y][x] });
				}
			}

			return convertedMapEdit;
		}
		public static List<MapDirection> InvalidDirections(MapDirection direction)
		{
			List<MapDirection> invalidDirections = new();

			invalidDirections.Add((MapDirection)(((int)direction + 4) % 8));
			invalidDirections.Add((MapDirection)(((int)direction + 3) % 8));
			invalidDirections.Add((MapDirection)(((int)direction + 5) % 8));

			return invalidDirections;
		}
		public static void CopySegmentToMap(List<List<byte>> target, List<List<byte>> segment, int _x, int _y)
		{

			for (int y = _y; y < _y + segment.Count; y++)
			{
				for (int x = _x; x < _x + segment[y - _y].Count; x++)
				{
					target[y][x] = segment[y - _y][x - _x];
				}

			}
		}
		public static OwMapExchangeData GenerateDesert(MT19337 rng)
		{
			int LoopedValue(int value, int max) => (value < 0) ? (max + value) % max : (value % max);

			List<List<MapLocation>> MapGrid = new();
			var Map = Enumerable.Range(0, 0x100).Select(i => Enumerable.Repeat((byte)OWTile.Desert, 0x100).ToList()).ToList();
			//List<List<byte>> Map = Enumerable.Repeat(Enumerable.Repeat((byte)OWTile.Desert, 0x100).ToList(), 0x100).ToList();

			OwMapExchangeData mapData = new();
			
			List<(MapLocation, MapLocation)> NodePairsList = new()
			{
				(MapLocation.ConeriaCastle1, MapLocation.TempleOfFiends1),
				(MapLocation.ConeriaCastle1, MapLocation.MatoyasCave),
				(MapLocation.ConeriaCastle1, MapLocation.Pravoka),
				(MapLocation.Pravoka, MapLocation.DwarfCave),
				(MapLocation.Pravoka, MapLocation.NorthwestCastle),
				(MapLocation.Pravoka, MapLocation.ElflandCastle),
				(MapLocation.ElflandCastle, MapLocation.MarshCave1),
				(MapLocation.ElflandCastle, MapLocation.Melmond),
				(MapLocation.ElflandCastle, MapLocation.CrescentLake),
				(MapLocation.Melmond, MapLocation.EarthCave1),
				(MapLocation.Melmond, MapLocation.SardasCave),
				(MapLocation.Melmond, MapLocation.Onrac),
				(MapLocation.CrescentLake, MapLocation.GurguVolcano1),
				(MapLocation.CrescentLake, MapLocation.IceCave1),
				(MapLocation.CrescentLake, MapLocation.Gaia),
				(MapLocation.Onrac, MapLocation.Waterfall),
				(MapLocation.Onrac, MapLocation.Caravan),
				(MapLocation.Onrac, MapLocation.CastleOrdeals1),
				(MapLocation.Gaia, MapLocation.MirageTower1),
				(MapLocation.Gaia, MapLocation.Lefein),
				(MapLocation.Gaia, MapLocation.BahamutCave1)
			};

			List<(MapLocation, int)> NodeDistancesList = new()
			{
				(MapLocation.ConeriaCastle1, 1),
				(MapLocation.Pravoka, 1),
				(MapLocation.ElflandCastle, 2),
				(MapLocation.Melmond, 2),
				(MapLocation.CrescentLake, 2),
				(MapLocation.Onrac, 3),
				(MapLocation.Gaia, 3),
			};

			List<(int, int)> DirectionCoordinates = new()
			{
				(0, -1),
				(-1, -1),
				(-1, 0),
				(-1, 1),
				(0, 1),
				(1, 1),
				(1, 0),
				(1, -1)
			};

			List<(MapLocation, byte, byte)> EntrancesCoords = new()
			{
				(MapLocation.ConeriaCastle1, 0x02, 0x03),
				(MapLocation.Coneria, 0x02, 0x03),
				(MapLocation.TempleOfFiends1, 0x01, 0x02),
				(MapLocation.MatoyasCave, 0x02, 0x03),
				(MapLocation.Pravoka, 0x02, 0x02),
				(MapLocation.DwarfCave, 0x01, 0x03),
				(MapLocation.NorthwestCastle, 0x00, 0x02),
				(MapLocation.ElflandCastle, 0x01, 0x02),
				(MapLocation.Elfland, 0x01, 0x02),
				(MapLocation.MarshCave1, 0x00, 0x01),
				(MapLocation.Melmond, 0x00, 0x02),
				(MapLocation.EarthCave1, 0x02, 0x03),
				(MapLocation.TitansTunnelEast, 0x05, 0x05),
				(MapLocation.TitansTunnelWest, 0x04, 0x02),
				(MapLocation.SardasCave, 0x02, 0x02),
				(MapLocation.CrescentLake, 0x02, 0x04),
				(MapLocation.GurguVolcano1, 0x00, 0x02),
				(MapLocation.IceCave1, 0x03, 0x02),
				(MapLocation.Onrac, 0x00, 0x02),
				(MapLocation.Caravan, 0x01, 0x02),
				(MapLocation.Waterfall, 0x02, 0x05),
				(MapLocation.Cardia1, 0x00, 0x01),
				(MapLocation.Cardia2, 0x00, 0x01),
				(MapLocation.Cardia4, 0x00, 0x01),
				(MapLocation.Cardia5, 0x00, 0x01),
				(MapLocation.Cardia6, 0x00, 0x01),
				(MapLocation.BahamutCave1, 0x00, 0x01),
				(MapLocation.CastleOrdeals1, 0x00, 0x02),
				(MapLocation.MirageTower1, 0x00, 0x02),
				(MapLocation.Gaia, 0x00, 0x02),
				(MapLocation.Lefein, 0x02, 0x03),
			};

			List<(MapLocation, byte, byte)> ShipCoords = new()
			{
				(MapLocation.ConeriaCastle1, 0x02, 0x07),
				(MapLocation.Coneria, 0x02, 0x07),
				(MapLocation.TempleOfFiends1, 0x01, 0x02),
				(MapLocation.MatoyasCave, 0x02, 0x03),
				(MapLocation.Pravoka, 0x02, 0x04),
				(MapLocation.DwarfCave, 0x01, 0x03),
				(MapLocation.NorthwestCastle, 0x00, 0x02),
				(MapLocation.ElflandCastle, 0x01, 0x02),
				(MapLocation.Elfland, 0x01, 0x02),
				(MapLocation.MarshCave1, 0x00, 0x01),
				(MapLocation.Melmond, 0x00, 0x02),
				(MapLocation.EarthCave1, 0x02, 0x03),
				(MapLocation.TitansTunnelEast, 0x05, 0x05),
				(MapLocation.TitansTunnelWest, 0x05, 0x05),
				(MapLocation.SardasCave, 0x05, 0x05),
				(MapLocation.CrescentLake, 0x02, 0x06),
				(MapLocation.GurguVolcano1, 0x00, 0x02),
				(MapLocation.IceCave1, 0x03, 0x02),
				(MapLocation.Onrac, 0x00, 0x02),
				(MapLocation.Caravan, 0x01, 0x02),
				(MapLocation.Waterfall, 0x02, 0x05),
				(MapLocation.Cardia1, 0x00, 0x01),
				(MapLocation.Cardia2, 0x00, 0x01),
				(MapLocation.Cardia4, 0x00, 0x01),
				(MapLocation.Cardia5, 0x00, 0x01),
				(MapLocation.Cardia6, 0x00, 0x01),
				(MapLocation.BahamutCave1, 0x00, 0x01),
				(MapLocation.CastleOrdeals1, 0x00, 0x02),
				(MapLocation.MirageTower1, 0x00, 0x02),
				(MapLocation.Gaia, 0x00, 0x02),
				(MapLocation.Lefein, 0x02, 0x04),
			};

			List<(MapLocation, int, int)> nodePositions = new();
			List<(MapLocation, MapDirection)> nodeDirection = new();

			bool validmap = false;
			int attemptCount = 0;

			while (!validmap)
			{
				attemptCount++;

				// Clear previous attempt
				nodePositions.Clear();
				nodeDirection.Clear();
				MapGrid.Clear();

				// Create empty grid
				for (int i = 0; i < 16; i++)
				{
					MapGrid.Add(new List<MapLocation> { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 });
				}

				// Put Coneria Castle
				nodePositions.Add((MapLocation.ConeriaCastle1, Rng.Between(rng, 7, 9), Rng.Between(rng, 7, 9)));

				MapGrid[nodePositions[0].Item2][nodePositions[0].Item3] = nodePositions[0].Item1;

				bool placementerror = false;

				// Place each nodes
				foreach (var node in NodeDistancesList)
				{
					var locationsToPlace = NodePairsList.Where(x => x.Item1 == node.Item1).ToList();
					var origin = (nodePositions.Find(x => x.Item1 == node.Item1).Item2, nodePositions.Find(x => x.Item1 == node.Item1).Item3);
					var originDirection = nodeDirection.Find(x => x.Item1 == node.Item1).Item2;

					foreach (var loc in locationsToPlace)
					{
						MapDirection direction = MapDirection.North;
						bool validSquare = false;
						List<MapDirection> validDirections = Enum.GetValues(typeof(MapDirection)).Cast<MapDirection>().ToList();

						if (loc.Item1 != MapLocation.ConeriaCastle1)
						{
							validDirections = validDirections.Where(x => !InvalidDirections(originDirection).Contains(x)).ToList();
						}

						List<MapDirection> invalidDirections = new();
						(int, int) newOrigin = (0, 0);
						int loopCheck = 0;

						while (!validSquare)
						{
							loopCheck++;
							if (loopCheck > 20)
							{
								placementerror = true;
								break;
							}
							direction = (MapDirection)Rng.Between(rng, 0, 7);
							int target_x = LoopedValue(origin.Item1 + DirectionCoordinates[(int)direction].Item1, 16);
							int target_y = LoopedValue(origin.Item2 + DirectionCoordinates[(int)direction].Item2, 16);

							if (MapGrid[target_x][target_y] == 0)
							{
								validDirections = validDirections.Where(x => !InvalidDirections(direction).Contains(x)).ToList();
								newOrigin = (target_x, target_y);
								validSquare = true;
							}
						}

						List<int> doodadsWeight = new() { 100, 100, 100, 100 };

						for (int i = 0; i < node.Item2; i++)
						{
							MapGrid[newOrigin.Item1][newOrigin.Item2] = SelectWeightedDoodads(doodadsWeight, rng);
							validSquare = false;
							loopCheck = 0;

							while (!validSquare)
							{
								loopCheck++;
								if (loopCheck > 20)
								{
									placementerror = true;
									break;
								}

								direction = validDirections.PickRandom(rng);
								int target_x = LoopedValue(newOrigin.Item1 + DirectionCoordinates[(int)direction].Item1, 16);
								int target_y = LoopedValue(newOrigin.Item2 + DirectionCoordinates[(int)direction].Item2, 16);

								if (MapGrid[target_x][target_y] == 0)
								{
									validDirections = validDirections.Where(x => !InvalidDirections(direction).Contains(x)).ToList();
									newOrigin = (target_x, target_y);
									validSquare = true;
								}
							}
						}

						MapGrid[newOrigin.Item1][newOrigin.Item2] = loc.Item2;

						if (NodeDistancesList.Select(x => x.Item1).Contains(loc.Item2))
						{
							nodePositions.Add((loc.Item2, newOrigin.Item1, newOrigin.Item2));
							nodeDirection.Add((loc.Item2, direction));
						}
					}
				}

				List<MapLocation> Cardias = new() { MapLocation.Cardia1, MapLocation.Cardia2, MapLocation.Cardia4, MapLocation.Cardia5, MapLocation.Cardia6 };

				foreach (var cardia in Cardias)
				{
					var emptyRow = MapGrid.Select((x, i) => new { x, i }).Where(y => y.x.Contains(MapLocation.StartingLocation)).Select(x => x.i).ToList();
					int selectedRow = emptyRow.PickRandom(rng);
					var emptyColumn = MapGrid[selectedRow].Select((x, i) => new { x, i }).Where(y => y.x == MapLocation.StartingLocation).Select(x => x.i).ToList();
					int selectedColumn = emptyColumn.PickRandom(rng);

					MapGrid[selectedRow][selectedColumn] = cardia;
				}

				if (placementerror)
				{
					if (attemptCount > 20)
					{
						/* 
						// Debug
						for (int i = 0; i < 16; i++)
						{
							var rowvalues = string.Join(";", MapGrid.Select(x => ((int)x[i]).ToString("X2")));
							Console.WriteLine(rowvalues);
						}
						*/

						throw new InsaneException("Failed to generate map.");
					}

					validmap = false;
				}
				else
				{
					validmap = true;
				}
			}

			Console.WriteLine("Desert Generation Attempts: " + attemptCount);

			// Place Special Doodad
			var cactusRow = MapGrid.Select((x, i) => new { x, i }).Where(y => y.x.Contains((MapLocation)(DoodadsType.CactusField + 0x100))).Select(x => x.i).ToList();
			if (cactusRow.Any())
			{
				int selectedRow = cactusRow.PickRandom(rng);
				var cactusColumn = MapGrid[selectedRow].Select((x, i) => new { x, i }).Where(y => y.x == (MapLocation)(DoodadsType.CactusField + 0x100)).Select(x => x.i).ToList();
				int selectedColum = cactusColumn.PickRandom(rng);

				var generatedDoodads = GenerateDoodads(DoodadsType.CactuarField, rng);
				CopySegmentToMap(Map, generatedDoodads,
					(selectedColum * 16) + Rng.Between(rng, 0, 15 - (generatedDoodads[0].Count)),
					(selectedRow * 16) + Rng.Between(rng, 0, 15 - (generatedDoodads.Count)));

				MapGrid[selectedColum][selectedRow] = 0x00;
			}

			// Place Locations and Doodads
			for (int x = 0; x < 16; x++)
			{
				for (int y = 0; y < 16; y++)
				{
					if (MapGrid[x][y] != 0)
					{
						if (MapGrid[x][y] >= (MapLocation)0x100)
						{
							var generatedDoodads = GenerateDoodads((DoodadsType)(MapGrid[x][y] - 0x100), rng);
							CopySegmentToMap(Map, generatedDoodads,
								(x * 16) + Rng.Between(rng, 0, 15 - (generatedDoodads[0].Count)),
								(y * 16) + Rng.Between(rng, 0, 15 - (generatedDoodads.Count)));
						}
						else
						{
							if (MapModifications.Where(z => z.Item1 == MapGrid[x][y]).Any())
							{
								var targetmod = MapModifications.Find(z => z.Item1 == MapGrid[x][y]);
								int xPosition = (x * 16) + Rng.Between(rng, 0, 15 - targetmod.Item2[0].Count);
								int yPosition = (y * 16) + Rng.Between(rng, 0, 15 - targetmod.Item2.Count);
								CopySegmentToMap(Map, targetmod.Item2,
									xPosition,
									yPosition);

								var locationToUpdate = EntrancesCoords.FindIndex(z => z.Item1 == MapGrid[x][y]);
								EntrancesCoords[locationToUpdate] = (EntrancesCoords[locationToUpdate].Item1, (byte)(EntrancesCoords[locationToUpdate].Item2 + xPosition), (byte)(EntrancesCoords[locationToUpdate].Item3 + yPosition));
								ShipCoords[locationToUpdate] = (ShipCoords[locationToUpdate].Item1, (byte)(ShipCoords[locationToUpdate].Item2 + xPosition), (byte)(ShipCoords[locationToUpdate].Item3 + yPosition));
							}
							else
							{
								CopySegmentToMap(Map, new List<List<byte>> { new List<byte> { OWTileLink.Find(maploc => maploc.Item1 == MapGrid[x][y]).Item2 } }, (byte)((x * 16) + Rng.Between(rng, 0, 15)), (byte)((y * 16) + Rng.Between(rng, 0, 15)));
							}
						}
					}
				}
			}

			// Update Titan's Tunnel cooords individually
			var titanEastIndex = EntrancesCoords.FindIndex(z => z.Item1 == MapLocation.TitansTunnelEast);
			var titanWestIndex = EntrancesCoords.FindIndex(z => z.Item1 == MapLocation.TitansTunnelWest);
			var sardaIndex = EntrancesCoords.FindIndex(z => z.Item1 == MapLocation.SardasCave);

			EntrancesCoords[titanEastIndex] = (MapLocation.TitansTunnelEast, (byte)(EntrancesCoords[titanEastIndex].Item2 + EntrancesCoords[sardaIndex].Item2 - 2), (byte)(EntrancesCoords[titanEastIndex].Item3 + EntrancesCoords[sardaIndex].Item3 - 2));
			ShipCoords[titanEastIndex] = (MapLocation.TitansTunnelEast, (byte)ShipCoords[sardaIndex].Item2, (byte)ShipCoords[sardaIndex].Item3);
			EntrancesCoords[titanWestIndex] = (MapLocation.TitansTunnelWest, (byte)(EntrancesCoords[titanWestIndex].Item2 + EntrancesCoords[sardaIndex].Item2 - 2), (byte)(EntrancesCoords[titanWestIndex].Item3 + EntrancesCoords[sardaIndex].Item3 - 2));
			ShipCoords[titanWestIndex] = (MapLocation.TitansTunnelWest, (byte)ShipCoords[sardaIndex].Item2, (byte)ShipCoords[sardaIndex].Item3);

			// Update Coneria & Elfland coords
			var coneriaIndex = EntrancesCoords.FindIndex(z => z.Item1 == MapLocation.Coneria);
			var coneriaCastleIndex = EntrancesCoords.FindIndex(z => z.Item1 == MapLocation.ConeriaCastle1);

			EntrancesCoords[coneriaIndex] = (MapLocation.Coneria, (byte)(EntrancesCoords[coneriaIndex].Item2 + EntrancesCoords[coneriaCastleIndex].Item2 - 2), (byte)(EntrancesCoords[coneriaIndex].Item3 + EntrancesCoords[coneriaCastleIndex].Item3 - 2));
			ShipCoords[coneriaIndex] = (MapLocation.Coneria, (byte)ShipCoords[coneriaCastleIndex].Item2, (byte)ShipCoords[coneriaCastleIndex].Item3);


			var elflandIndex = EntrancesCoords.FindIndex(z => z.Item1 == MapLocation.Elfland);
			var elflandCastleIndex = EntrancesCoords.FindIndex(z => z.Item1 == MapLocation.ElflandCastle);

			EntrancesCoords[elflandIndex] = (MapLocation.Elfland, (byte)(EntrancesCoords[elflandIndex].Item2 + EntrancesCoords[elflandCastleIndex].Item2 - 2), (byte)(EntrancesCoords[elflandIndex].Item3 + EntrancesCoords[elflandCastleIndex].Item3 - 2));
			ShipCoords[elflandIndex] = (MapLocation.Elfland, (byte)ShipCoords[elflandCastleIndex].Item2, (byte)ShipCoords[elflandCastleIndex].Item3);

			// Update Teleporters and Locations
			List<MapLocation> exitToUpdate = new() { MapLocation.TitansTunnelEast, MapLocation.TitansTunnelWest, MapLocation.IceCave1, MapLocation.CastleOrdeals1, MapLocation.ConeriaCastle1, MapLocation.EarthCave1, MapLocation.GurguVolcano1, MapLocation.Onrac, MapLocation.MirageTower1 };

			// Set starting location
			var coneria_x = EntrancesCoords.Find(z => z.Item1 == MapLocation.ConeriaCastle1).Item2 - 7;
			var coneria_y = EntrancesCoords.Find(z => z.Item1 == MapLocation.ConeriaCastle1).Item3 - 1;

			//var locationData = new OwLocationData(this);
			List<ShipLocation> newShipLocations = new();
			List<TeleportFixup> exitUpdates = new();
			Dictionary<string, Sanity.SCCoords> coordUpdates = new();

			var entranceList = Enum.GetValues(typeof(OverworldTeleportIndex))
				.Cast<OverworldTeleportIndex>()
				.ToList();

			foreach (var exit in exitToUpdate)
			{
				var targetExitCoord = EntrancesCoords.Find(z => z.Item1 == exit);
				var index = exitToUpdate.FindIndex(z => z == exit);
				exitUpdates.Add(new TeleportFixup { Type = TeleportType.Exit, Index = (int)index, To = new TeleData { X = targetExitCoord.Item2, Y = targetExitCoord.Item3 } });
			}

			foreach (var entrance in entranceList)
			{
				var targetMapLocation = MapLocationToOWTeleporterIndex.Find(x => x.Item2 == entrance).Item1;
				var targetShipCoord = ShipCoords.Find(z => z.Item1 == targetMapLocation);
				var targetEntranceCoord = EntrancesCoords.Find(z => z.Item1 == targetMapLocation);
				newShipLocations.Add(new ShipLocation { TeleporterIndex = (byte)entrance, X = targetShipCoord.Item2, Y = targetShipCoord.Item3 });
				coordUpdates.Add(entrance.ToString(), new Sanity.SCCoords(targetEntranceCoord.Item2, targetEntranceCoord.Item3));
			}

			newShipLocations.Add(new ShipLocation { TeleporterIndex = (byte)255, X = (byte)(coneria_x + 7), Y = (byte)(coneria_y + 7) });

			mapData.ShipLocations = newShipLocations.ToArray();
			mapData.StartingLocation = new Sanity.SCCoords(coneria_x + 7, coneria_y + 7);
			mapData.TeleporterFixups = exitUpdates.ToArray();
			mapData.OverworldCoordinates = coordUpdates;
			mapData.DecompressedMapRows = Map.Select(x => Convert.ToBase64String(x.ToArray())).ToList();

			return mapData;
		}

		public static void ApplyDesertModifications(FF1Rom rom, OwMapExchange owMapExchange, FF1Rom.NPCdata npcdata)
		{
			(int, int) startDomain = ((owMapExchange.StartingLocation.X / 32) - 1, (owMapExchange.StartingLocation.Y / 32) - 1);

			// Update tiles and palette
			for (int i = 1; i < 16; i += 4)
			{
				rom.Put(0x0380 + i, Blob.FromHex("37"));
			}

			List<byte> holes = new List<byte> { 0x66, 0x67, 0x68, 0x69, 0x6A, 0x6C, 0x6E };

			foreach (var hole in holes)
			{
				rom.Put(0x0300 + hole, Blob.FromHex("55"));
			}

			List<byte> newTilesPalette = new List<byte> { 0x14, 0x15 };

			foreach (var newtile in newTilesPalette)
			{
				rom.Put(0x0300 + newtile, Blob.FromHex("55"));
			}

			// New graphic tiles
			var newGraphicTiles =
				"FFFFFFFFFFF7E36300000000000C1EDE" +
				"FFFF7F7F7FFFFF9F0080C0C0C0008080" +
				"E36382E3E3D3B9A0CAFF77161E3E6763" +
				"FFFFFFFFFFFFFBEB0000000000000C1E" +
				"FFBFFFBF7FFF3F1F00C0C0C080000000" +
				"7BEB6F8BEAEBFAEADEDEFE7F1F1E1E1E" +
				"FFFFEFE7ADA7EFE90000181A5E781818" +
				"C680051DF0E2C2CC397DE6870E5A310B" +
				"F926A3091209000006D9509EF1FC9FB1" +
				"FCFDFDFFFFF6E0FC0A0A0A480C040000" +
				"66E2FBFFFF60233F3844404040602030" +
				"E181C0808080C0BFE3D9DCBCBEBEC0FF" +
				"EDBFFBDF767F0BBFFFFFFFFF7F7F1FFF" +
				"EDBFFBC7820100BFFFFFFFC7B37900FF" +
				"F6BFFBDFF6FFE8B9FFFFFFFFFFFFFCFB" +
				"EDBFFBDFF6FF2BDFFFFFFFFFFFFF3FDF" +
				"E1A0C28505A0A0BFF5EAD7AD0DA8A0FF" +
				"EDF743BD808701BFEFF77BBD80B701FF";

			rom.Put(0x8210, Blob.FromHex(newGraphicTiles));

			List<(byte, byte, byte, byte, byte)> tilesToUpdate = new()
			{
				(0x03, 0x21, 0x20, 0x23, 0x22), // Cactuar
				(0x04, 0x24, 0x20, 0x26, 0x25), // Cactus1
				(0x05, 0x24, 0x27, 0x26, 0x25), // Cactus2
				(0x13, 0x28, 0x29, 0x2A, 0x2B), // Palm
				(0x14, 0x54, 0x2E, 0x2C, 0x2D), // Rock1
				(0x15, 0x54, 0x54, 0x2C, 0x2D), // Rock2
				(0x36, 0x2F, 0x30, 0x31, 0x32), // Caravan
			};

			foreach (var tile in tilesToUpdate)
			{
				rom.Put(0x0100 + tile.Item1, new byte[] { tile.Item2 });
				rom.Put(0x0180 + tile.Item1, new byte[] { tile.Item3 });
				rom.Put(0x0200 + tile.Item1, new byte[] { tile.Item4 });
				rom.Put(0x0280 + tile.Item1, new byte[] { tile.Item5 });

				if (tile.Item1 == 0x36)
				{
					continue;
				}

				byte tileprop = 0x0F;     // can't walk over                           
				rom.Put(0x0000 + tile.Item1 * 2, new byte[] { tileprop });
			}

			rom.Put(0x0000 + (int)OWTile.Desert * 2, new byte[] { 0xEA }); // Update desert tile to allow ship
			rom.Put(0x0000 + (int)0x46 * 2, new byte[] { (byte)(rom.Get(0x0000 + 0x46 * 2, 1)[0] & 0b1111_1110) }); // Update Waterfall tile to not require canoe


			// Turn Ship into the Yggdrasil
			// FlyAirship - use ship
			rom.PutInBank(0x1F, 0xC216, Blob.FromHex("00")); // Ship visibility
			rom.PutInBank(0x1F, 0xC220, Blob.FromHex("01")); // Ship X
			rom.PutInBank(0x1F, 0xC22A, Blob.FromHex("02")); // Ship Y
			rom.PutInBank(0x1F, 0xC22F, Blob.FromHex("04"));
			rom.PutInBank(0x1F, 0xC235, Blob.FromHex("44")); //Music track
			rom.PutInBank(0x1F, 0xC238, Blob.FromHex("EAEAEA")); // Airship animation

			// ProcessOWInput - check for ship
			rom.PutInBank(0x1F, 0xC25D, Blob.FromHex("04"));

			// LandAirship
			rom.PutInBank(0x1F, 0xC6BC, Blob.FromHex("4CE2C6"));
			rom.PutInBank(0x1F, 0xC6E8, Blob.FromHex("01")); // Ship X
			rom.PutInBank(0x1F, 0xC6F0, Blob.FromHex("02")); // Ship Y
			rom.PutInBank(0x1F, 0xC6F9, Blob.FromHex("4E")); // Ship Y

			// OWCanMove
			rom.PutInBank(0x1F, 0xC506, Blob.FromHex("00")); // No battle in Ship

			//DrawPlayerMapmanSprite
			rom.PutInBank(0x1F, 0xE289, Blob.FromHex("04")); // Don't animate ship
			rom.PutInBank(0x1F, 0xE28E, Blob.FromHex("A900"));

			// Update music
			rom.PutInBank(0x1F, 0xC759, Blob.FromHex("4E4E4E4E4444444444"));

			// Remove Wave sound
			rom.PutInBank(0x1F, 0xC112, Blob.FromHex("00"));

			var gameVariableText = "A290908D9B8A9C929500"; // SHIP+spill on airship

			rom.Put(0x2B5D0, Blob.FromHex(gameVariableText));

			// Remove damage tile sound
			rom.PutInBank(0x1F, 0xC7E7, Blob.FromHex("EAEAEAEAEAEAEAEAEAEAEAEAEAEAEA"));

			// Disable Minimap
			rom.PutInBank(0x1F, 0xC1A9, Blob.FromHex("00"));

			UpdateOWFormations(rom, startDomain.Item1, startDomain.Item2);
			DoDUpdateDialogues(rom, npcdata);
		}
		public static MapLocation SelectWeightedDoodads(List<int> weight, MT19337 rng)
		{
			int pick = Rng.Between(rng, 0, weight.Sum());
			DoodadsType selectedDoodads;

			if (weight[0] > pick)
			{
				selectedDoodads = DoodadsType.CactusField;
			}
			else if (weight[0] + weight[1] > pick)
			{
				selectedDoodads = DoodadsType.RockField;
			}
			else if (weight[0] + weight[1] + weight[3] > pick)
			{
				selectedDoodads = DoodadsType.Mountains;
			}
			else
			{
				selectedDoodads = DoodadsType.Oasis;
			}

			weight[(int)selectedDoodads] /= 10;

			return (MapLocation)(selectedDoodads + 0x100);
		}
		public static List<List<byte>> GenerateDoodads(DoodadsType type, MT19337 rng)
		{
			List<byte> doodadsTilemap = new();
			int width = 0;
			int height = 0;
			if (type == DoodadsType.CactusField || type == DoodadsType.CactuarField)
			{
				width = Rng.Between(rng, 4, 6);
				height = Rng.Between(rng, 4, 6);
				var cactusCount = Rng.Between(rng, 4, 7);
				List<OWTile> validTiles = new() { OWTile.DesertCacti, OWTile.DesertCactus };

				List<int> availableTiles = Enumerable.Range(0, width * height).ToList();
				doodadsTilemap = Enumerable.Repeat((byte)OWTile.Desert, width * height).ToList();

				for (int i = 0; i < cactusCount; i++)
				{
					doodadsTilemap[availableTiles.SpliceRandom(rng)] = (byte)validTiles.PickRandom(rng);
				}

				if (type == DoodadsType.CactuarField)
				{
					doodadsTilemap[availableTiles.SpliceRandom(rng)] = (byte)OWTile.DesertCactuar;
				}
			}
			else if (type == DoodadsType.RockField)
			{
				width = Rng.Between(rng, 4, 6);
				height = Rng.Between(rng, 4, 6);
				var rockCount = Rng.Between(rng, 4, 7);
				List<OWTile> validTiles = new() { OWTile.DesertRock, OWTile.DesertRocks };

				List<int> availableTiles = Enumerable.Range(0, width * height).ToList();
				doodadsTilemap = Enumerable.Repeat((byte)OWTile.Desert, width * height).ToList();

				for (int i = 0; i < rockCount; i++)
				{
					doodadsTilemap[availableTiles.SpliceRandom(rng)] = (byte)validTiles.PickRandom(rng);
				}
			}
			else if (type == DoodadsType.Oasis)
			{
				width = Rng.Between(rng, 3, 4);
				height = Rng.Between(rng, 3, 4);
				int waterWidth = Rng.Between(rng, 2, width - 1);
				int waterHeight = Rng.Between(rng, 2, height - 1);
				int waterX = Rng.Between(rng, 0, width - waterWidth);
				int waterY = Rng.Between(rng, 0, height - waterHeight);
				var palmCount = Rng.Between(rng, 3, 5);

				List<int> availableTiles = Enumerable.Range(0, width * height).ToList();
				doodadsTilemap = Enumerable.Repeat((byte)OWTile.Desert, width * height).ToList();

				doodadsTilemap[waterX + (waterY * width)] = (byte)OWTile.RiverUpperLeft;
				availableTiles.Remove(waterX + (waterY * width));
				doodadsTilemap[waterX + (waterWidth - 1) + (waterY * width)] = (byte)OWTile.RiverUpperRight;
				availableTiles.Remove(waterX + (waterWidth - 1) + (waterY * width));

				doodadsTilemap[waterX + ((waterY + (waterHeight - 1)) * width)] = (byte)OWTile.RiverLowerLeft;
				availableTiles.Remove(waterX + ((waterY + (waterHeight - 1)) * width));
				doodadsTilemap[waterX + (waterWidth - 1) + ((waterY + (waterHeight - 1)) * width)] = (byte)OWTile.RiverLowerRight;
				availableTiles.Remove(waterX + (waterWidth - 1) + ((waterY + (waterHeight - 1)) * width));

				for (int i = waterX + 1; i < waterX + waterWidth - 1; i++)
				{
					for (int j = waterY; j < waterY + waterHeight; j++)
					{
						doodadsTilemap[i + (j * width)] = (byte)OWTile.RiverCenter;
						availableTiles.Remove(i + (j * width));
					}
				}

				for (int j = waterY + 1; j < waterY + waterHeight - 1; j++)
				{
					for (int i = waterX; i < waterX + waterWidth; i++)
					{
						doodadsTilemap[i + (j * width)] = (byte)OWTile.RiverCenter;
						availableTiles.Remove(i + (j * width));
					}
				}

				for (int i = 0; i < palmCount; i++)
				{
					doodadsTilemap[availableTiles.SpliceRandom(rng)] = (byte)OWTile.DesertPalmTree;
				}
			}
			else if (type == DoodadsType.Mountains)
			{
				int blob1Width = Rng.Between(rng, 2, 4);
				int blob1Height = Rng.Between(rng, 2, 4);
				int blob2Width = Rng.Between(rng, 2, 4);
				int blob2Height = Rng.Between(rng, 2, 4);

				int blob2X = Rng.Between(rng, 0, 3);
				int blob2Y = Rng.Between(rng, 0, 3);

				width = Math.Max(blob2X + blob2Width, blob1Width);
				height = Math.Max(blob2Y + blob2Height, blob1Height);

				List<List<byte>> mountainMap = new();

				for (int i = 0; i < height; i++)
				{
					mountainMap.Add(Enumerable.Repeat((byte)OWTile.Desert, width).ToList());
				}

				for (int x = 0; x < blob1Width; x++)
				{
					for (int y = 0; y < blob1Height; y++)
					{
						mountainMap[y][x] = (byte)OWTile.MountainCenter;
					}
				}

				for (int x = blob2X; x < (blob2X + blob2Width); x++)
				{
					for (int y = blob2Y; y < (blob2Y + blob2Height); y++)
					{
						mountainMap[y][x] = (byte)OWTile.MountainCenter;
					}
				}

				int reverseMountain = Rng.Between(rng, 0, 1);

				if (reverseMountain == 1)
				{
					mountainMap.ForEach(x => x.Reverse());
				}

				// Do borders
				for (int x = 0; x < width; x++)
				{
					for (int y = 0; y < height; y++)
					{
						if (mountainMap[y][x] == (byte)OWTile.Desert)
						{
							continue;
						}
						else if ((y - 1 < 0) || mountainMap[y - 1][x] == (byte)OWTile.Desert)
						{
							mountainMap[y][x] = (byte)OWTile.MountainUpperMiddle;
						}
						else if ((y + 1 >= height) || mountainMap[y + 1][x] == (byte)OWTile.Desert)
						{
							mountainMap[y][x] = (byte)OWTile.MountainLowerMiddle;
						}
						else if ((x - 1 < 0) || mountainMap[y][x - 1] == (byte)OWTile.Desert)
						{
							mountainMap[y][x] = (byte)OWTile.MountainMiddleLeft;
						}
						else if ((x + 1 >= width) || mountainMap[y][x + 1] == (byte)OWTile.Desert)
						{
							mountainMap[y][x] = (byte)OWTile.MountainMiddleRight;
						}
					}
				}

				// Do corners
				for (int x = 0; x < width; x++)
				{
					for (int y = 0; y < height; y++)
					{
						if (mountainMap[y][x] == (byte)OWTile.Desert)
						{
							continue;
						}
						else if (((y - 1 < 0) || mountainMap[y - 1][x] == (byte)OWTile.Desert) && ((x - 1 < 0) || mountainMap[y][x - 1] == (byte)OWTile.Desert))
						{
							mountainMap[y][x] = (byte)OWTile.MountainUpperLeft;
						}
						else if (((y + 1 >= height) || mountainMap[y + 1][x] == (byte)OWTile.Desert) && ((x - 1 < 0) || mountainMap[y][x - 1] == (byte)OWTile.Desert))
						{
							mountainMap[y][x] = (byte)OWTile.MountainLowerLeft;
						}
						else if (((y - 1 < 0) || mountainMap[y - 1][x] == (byte)OWTile.Desert) && ((x + 1 >= width) || mountainMap[y][x + 1] == (byte)OWTile.Desert))
						{
							mountainMap[y][x] = (byte)OWTile.MountainUpperRight;
						}
						else if (((y + 1 >= height) || mountainMap[y + 1][x] == (byte)OWTile.Desert) && ((x + 1 >= width) || mountainMap[y][x + 1] == (byte)OWTile.Desert))
						{
							mountainMap[y][x] = (byte)OWTile.MountainLowerRight;
						}
					}
				}

				doodadsTilemap = mountainMap.SelectMany(x => x).ToList();
			}
			else
			{
				width = 1;
				height = 1;
				doodadsTilemap = Enumerable.Repeat((byte)OWTile.DesertCactus, 1).ToList();
			}

			List<List<byte>> finalDoodadsTilemap = new();
			for (int i = 0; i < height; i++)
			{
				finalDoodadsTilemap.Add(doodadsTilemap.GetRange(i * width, width).ToList());
			}

			return finalDoodadsTilemap;
		}
	}
}
