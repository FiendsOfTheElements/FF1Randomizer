﻿using System;
using System.Collections.Generic;
using RomUtilities;
using System.Linq;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System.IO;

/* To Implement

• Special Bonuses for Classes
	- improved HARM
	- Lockpicking
	- Hunter
	- Other Bonuses/Maluses

• Special gear additions or resistances if you have a single element/status spell school (as a focus, not one of many)

*/

namespace FF1Lib
{
	public class Transmooglifier
	{
		// Armor definitions. Why not an enum? Who knows...
		private int CLOTH = 0;            // cloth -- everyone
		private int LIGHT = 1;            // light -- fi/th/bb/rm
		private int MEDIUM = 2;           // medium -- fi/ni/rm
		private int HEAVY = 3;            // heavy -- fi/ni
		private int KNIGHT = 4;           // legendary (dragon, opal, aegis) -- kn

		// Magic Domains
		public static Dictionary<string, List<MagicSpell>> spellFamilies = new Dictionary<string, List<MagicSpell>>();

		public List<string> classDescriptions = new List<string>();

		const int lut_MapmanPalettes = 0x8150;

		public void Transmooglify(MT19337 rng, FF1Rom rom)
		{
			ClassDef.rom = rom;
			ClassDef.rng = rng;
			ClassDef.newPermissions = new GearPermissions(0x3BFA0, (int)Item.Cloth, rom);

			AddNewIcons(rom);

			if (spellFamilies.Count <= 0)
				PopulateSpellTypes(rom);

			List<ClassDef> classes = CreateClasses();

			classes.Shuffle(rng);

			for (int i = 0; i < 6; i++)
			{
				Console.WriteLine($"Class Name: { classes[i].name }");
				classDescriptions.Add(classes[i].PublishToClass(i));
			}

			LoadImages(classes);
		}

		public void LoadImages(List<ClassDef> classes)
		{
			ClassDef.rom.PutInBank(0x1F, 0xD8B6, Blob.FromHex("A90F2003FE4CC081EAEAEAEAEAEAEAEAEAEAEAEAEAEAEA"));
			ClassDef.rom.PutInBank(0x0F, 0x81C0, Blob.FromHex("AD0061C9FFD002A90D0A0A0A6908AAA008BD508199D003CA8810F660"));

			for (int i = 0; i < 6; i++)
			{
				ClassDef c = classes[i];
				Console.WriteLine("Class Palettes " + c.name);

				ClassDef.rom.ImportMapmanSync(c.getImage(), c.classIndex, 24, 0, ClassDef.rom.NESpalette);
				ClassDef.rom.ImportBattleSpriteSync(c.getImage(), c.classIndex, 0, 0, ClassDef.rom.NESpalette);

				Console.WriteLine("Class Palettes " + c.promoName);
				ClassDef.rom.ImportMapmanSync(c.getImage(true), c.classIndex+6, 24, 0, ClassDef.rom.NESpalette);
				ClassDef.rom.ImportBattleSpriteSync(c.getImage(true), c.classIndex+6, 0, 0, ClassDef.rom.NESpalette);
			}

			// add palette for "none" mapman
			ClassDef.rom.PutInBank(0x0F, lut_MapmanPalettes + (13 << 3), new byte[] {0x0F, 0x0F, 0x12, 0x36,
										0x0F, 0x0F, 0x21, 0x36});
		}

		public byte[] getTile(int imageTileIndex, Dictionary<Rgba32, byte> index, Image<Rgba32> image)
		{
			var newtile = new byte[64];
			int px = 0;
			for (int y = 0; y < (0 + 8); y++)
			{
				for (int x = (imageTileIndex * 8); x < ((imageTileIndex * 8) + 8); x++)
				{
					newtile[px] = index[image[x, y]];
					px++;
				}
			}

			return newtile;
		}

		public void AddNewIcons(FF1Rom rom)
		{
			// New Icon Hacks PLAN
			//	Remove the code from AddElementIcons
			//	New image blobs are imported into Bank 12, 0x8800. There is 0x800 of space but we can only use 0x600. as the last 0x200 is reserved for orbs.
			//
			//	In bank F's LoadMenuBGCHRAndPalettes, which is roughly at 0xEA9F, jmp to this afterwards
			//		LDA #$0C		// Bank 12...?
			//		JSR SwapPRG_L	// Swap to Bank 12
			//
			//		LDA #<00		// from source address 0x8800. I hope I have upper and lower bits set correctly and not backwards
			//		STA tmp
			//		LDA #>88
			//		STA tmp+1
			//
			//		LDX #$06		// Grab 6 rows of tiles
			//		LDA #$0			// dest PPU address $0000
			//		JSR CHRLoadToA  // load up icons 
			//		// end scene, jump back? or forward
			//
			//
			//	Then assign all the icons to values from 0x00-0x60 in FF1Text.cs

			IImageFormat format;

			var assembly = System.Reflection.Assembly.GetExecutingAssembly();
			var weaponiconsPath = assembly.GetManifestResourceNames().First(str => str.EndsWith("weapon_icons.png"));
			var spelliconsPath = assembly.GetManifestResourceNames().First(str => str.EndsWith("spell_icons.png"));

			Image<Rgba32> image = Image.Load<Rgba32>(assembly.GetManifestResourceStream(weaponiconsPath), out format);

			// 0 = black
			// 1 = grey
			// 2 = blue
			// 3 = white

			Dictionary<Rgba32, byte> index = new Dictionary<Rgba32, byte> {
				{ new Rgba32(0x00, 0x00, 0x00), 0 },
				{ new Rgba32(0x7f, 0x7f, 0x7f), 1 },
				{ new Rgba32(116, 116, 116), 1 },
				{ new Rgba32(0x00, 0x00, 0xff), 2 },
				{ new Rgba32(36, 24, 140), 2 },
				{ new Rgba32(0xff, 0xff, 0xff), 3 },
				{ new Rgba32(252, 252, 252), 3 }
			};

			for (int w = 0; w < 5; w++)
			{
				rom.PutInBank(0x09, 0x8D40 + 0x1E0 + (w * 16), rom.EncodeForPPU(getTile(w, index, image)));
				//rom.PutInBank(0x12, 0x8D40 + (w * 16), rom.EncodeForPPU(newtile));
			}

			rom.PutInBank(0x09, 0x8800 + 0x10 * 0x48, rom.EncodeForPPU(getTile(5, index, image)));

			// For now, there's no room for new spells T_T

			//image = Image.Load<Rgba32>(assembly.GetManifestResourceStream(spelliconsPath), out format);
			//for (int w = 0; w < 4; w++)
			//{
			//	rom.PutInBank(0x09, 0x8D40 + 0x1E0 + (w * 16), rom.EncodeForPPU(getTile(w, index, image)));
			//	//rom.PutInBank(0x12, 0x8D40 + (w * 16), rom.EncodeForPPU(newtile));
			//}

			//for (int w = 4; w < 11; w++)
			//{
			//	rom.PutInBank(0x09, 0x8D40 - 0x120 + (w * 16), rom.EncodeForPPU(getTile(w, index, image)));
			//	//rom.PutInBank(0x12, 0x8D40 + (w * 16), rom.EncodeForPPU(newtile));
			//}
		}

		public List<ClassDef> CreateClasses()
		{
			List<ClassDef> classes = new();

			/* Sample Class Template. Does not include the special blursings
			classes.Add(new ClassDef() {
				name = "", promoName = "", shortName = "", promoShortName = "",
				HP = 25, STR = 25, AGI = 25, VIT = 25, LCK = 25, HIT = 2, MDEF = 2,
				guaranteedMagic = new List<string> { "black", "white" }, mageLevel = 5, spellChargeGrowth = 72, spellChargeMax = 9,
				possibleMagic = new List<string> { "elem", "status", "holy" }, MagicSchoolsMax = 2, averageMagicSchools = 1,
				armourWeight = CLOTH,
				guaranteedWeapon = new List<WeaponSprite> { WeaponSprite.AXE }, 
				possibleWeapon = new List<WeaponSprite> { WeaponSprite.CHUCK, WeaponSprite.SHORTSWORD }, weaponsMax = 2, averageWeapons = 1
				});
			*/

			// Basegame Classes, but Random!
			classes.Add(new ClassDef()
			{
				name = "FIGHTER",
				promoName = "KNIGHT",
				shortName = "Fi",
				promoShortName = "Kn",
				HP = 27,
				STR = 50,
				AGI = 35,
				VIT = 25,
				LCK = 24,
				HIT = 3,
				MDEF = 3,
				mageLevel = 3,
				mageLevelPromotion = 4,
				spellChargeGrowth = 20,
				spellChargeMax = 4,
				possibleMagic = new List<string> { "recovery", "holy" },
				MagicSchoolsMax = 1,
				averageMagicSchools = 0.5f,
				promoMagic = new List<string> { "white", "recovery", "holy", "buff" },
				armourWeight = HEAVY,
				possibleWeapon = new List<WeaponSprite> { WeaponSprite.SHORTSWORD, WeaponSprite.LONGSWORD, WeaponSprite.CHUCK, WeaponSprite.SCIMITAR,
					WeaponSprite.RAPIER, WeaponSprite.FALCHION, WeaponSprite.AXE, WeaponSprite.HAMMER, WeaponSprite.KNIFE },
				weaponsMax = 6,
				averageWeapons = 6f
			});

			classes.Add(new ClassDef()
			{
				name = "THIEF",
				promoName = "NINJA",
				shortName = "Th",
				promoShortName = "Ni",
				HP = 18,
				STR = 33,
				AGI = 50,
				VIT = 16,
				LCK = 50,
				HIT = 4,
				MDEF = 2,
				mageLevel = 3,
				mageLevelPromotion = 4,
				spellChargeGrowth = 20,
				spellChargeMax = 4,
				possibleMagic = new List<string> { "status", "space", "poison", "death" },
				MagicSchoolsMax = 1,
				averageMagicSchools = 0.5f,
				promoMagic = new List<string> { "black", "status", "elem", "space", "poison", "death" },
				armourWeight = LIGHT,
				promoArmourWeight = HEAVY,
				guaranteedWeapon = new List<WeaponSprite> { WeaponSprite.KNIFE },
				possibleWeapon = new List<WeaponSprite> { WeaponSprite.RAPIER, WeaponSprite.SCIMITAR, WeaponSprite.FALCHION, WeaponSprite.CHUCK },
				weaponsMax = 3,
				averageWeapons = 2f
			});

			classes.Add(new ClassDef()
			{
				name = "Bl.BELT",
				promoName = "MASTER",
				shortName = "BB",
				promoShortName = "Ma",
				HP = 20,
				STR = 24,
				AGI = 25,
				VIT = 50,
				LCK = 30,
				HIT = 3,
				MDEF = 4,
				mageLevel = 3,
				mageLevelPromotion = 4,
				spellChargeGrowth = 20,
				spellChargeMax = 4,
				possibleMagic = new List<string> { "earth" },
				MagicSchoolsMax = 1,
				averageMagicSchools = 0.25f,
				promoMagic = new List<string> { "ailment", "health", "buff" },
				armourWeight = CLOTH,
				promoArmourWeight = CLOTH,
				guaranteedWeapon = new List<WeaponSprite> { WeaponSprite.CHUCK },
				possibleWeapon = new List<WeaponSprite> { WeaponSprite.STAFF, WeaponSprite.KNIFE },
				weaponsMax = 2,
				averageWeapons = 1f,
				UnarmedAttack = 0.75f,
				CatClawMaster = 0.75f,
			});

			classes.Add(new ClassDef()
			{
				name = "RedMAGE",
				promoName = "RedWIZ",
				shortName = "RdM",
				promoShortName = "RdW",
				HP = 13,
				STR = 25,
				AGI = 17,
				VIT = 22,
				LCK = 25,
				HIT = 2,
				MDEF = 2,
				guaranteedMagic = new List<string> { },
				mageLevel = 4,
				mageLevelPromotion = 3,
				spellChargeGrowth = 60,
				spellChargeMax = 9,
				possibleMagic = new List<string> { "black", "white", "elem", "status", "buff", "recovery", "grey" },
				MagicSchoolsMax = 2,
				averageMagicSchools = 3f,
				armourWeight = MEDIUM,
				guaranteedWeapon = new List<WeaponSprite> { },
				possibleWeapon = new List<WeaponSprite> { WeaponSprite.SCIMITAR, WeaponSprite.RAPIER, WeaponSprite.KNIFE, WeaponSprite.SHORTSWORD, WeaponSprite.FALCHION },
				weaponsMax = 5,
				averageWeapons = 2.5f
			});

			classes.Add(new ClassDef()
			{
				name = "Wh.MAGE",
				promoName = "Wh.WIZ",
				shortName = "WhM",
				promoShortName = "WhW",
				HP = 15,
				STR = 18,
				AGI = 18,
				VIT = 20,
				LCK = 19,
				HIT = 1,
				MDEF = 2,
				guaranteedMagic = new List<string> { "white" },
				mageLevel = 5,
				spellChargeGrowth = 72,
				spellChargeMax = 9,
				armourWeight = CLOTH,
				promoArmourWeight = CLOTH,
				guaranteedWeapon = new List<WeaponSprite> { WeaponSprite.STAFF },
				possibleWeapon = new List<WeaponSprite> { WeaponSprite.IRONSTAFF, WeaponSprite.HAMMER },
				weaponsMax = 2,
				averageWeapons = 1
			});

			classes.Add(new ClassDef()
			{
				name = "Bl.MAGE",
				promoName = "Bl.WIZ",
				shortName = "BlM",
				promoShortName = "BlW",
				HP = 12,
				STR = 13,
				AGI = 13,
				VIT = 14,
				LCK = 14,
				HIT = 1,
				MDEF = 2,
				guaranteedMagic = new List<string> { "black" },
				mageLevel = 5,
				spellChargeGrowth = 72,
				spellChargeMax = 9,
				armourWeight = CLOTH,
				promoArmourWeight = CLOTH,
				guaranteedWeapon = new List<WeaponSprite> { WeaponSprite.IRONSTAFF },
				possibleWeapon = new List<WeaponSprite> { WeaponSprite.STAFF, WeaponSprite.KNIFE },
				weaponsMax = 2,
				averageWeapons = 1
			});


			// Ronin / Samurai
			classes.Add(new ClassDef()
			{
				name = "RONIN",
				promoName = "SAMURAI",
				shortName = "Rn",
				promoShortName = "Sm",
				HP = 25,
				STR = 40,
				AGI = 25,
				VIT = 35,
				LCK = 10,
				HIT = 4,
				MDEF = 1,
				mageLevel = 8,
				mageLevelPromotion = 0,
				spellChargeGrowth = 40,
				spellChargeMax = 5,
				possibleMagic = new List<string> { "elem", "fire", "holy" },
				MagicSchoolsMax = 1,
				averageMagicSchools = 0.33f,
				armourWeight = HEAVY,
				guaranteedWeapon = new List<WeaponSprite> { WeaponSprite.KNIFE, WeaponSprite.SCIMITAR },
				possibleWeapon = new List<WeaponSprite> { WeaponSprite.RAPIER, WeaponSprite.SHORTSWORD },
				weaponsMax = 1,
				averageWeapons = 0.5f,
				WoodAdept = 0.2f,
				SteelLord = 0.2f
			});

			// Pugilist / Monk
			classes.Add(new ClassDef()
			{
				name = "PUGILIST",
				promoName = "MONK",
				shortName = "Pg",
				promoShortName = "Mk",
				HP = 30,
				STR = 32,
				AGI = 20,
				VIT = 40,
				LCK = 40,
				HIT = 4,
				MDEF = 3,
				mageLevel = 3,
				mageLevelPromotion = 4,
				spellChargeGrowth = 20,
				spellChargeMax = 4,
				possibleMagic = new List<string> { "earth", "fire", "lit", "ice" },
				MagicSchoolsMax = 4,
				averageMagicSchools = 0.25f,
				promoMagic = new List<string> { "elem" },
				armourWeight = CLOTH,
				promoArmourWeight = CLOTH,
				guaranteedWeapon = new List<WeaponSprite> { WeaponSprite.CHUCK },
				possibleWeapon = new List<WeaponSprite> { WeaponSprite.STAFF, WeaponSprite.IRONSTAFF },
				weaponsMax = 2,
				averageWeapons = 1f,
				UnarmedAttack = 0.75f,
			});

			// Marauder / Viking
			classes.Add(new ClassDef()
			{
				name = "MARAUDER",
				promoName = "VIKING",
				shortName = "Mr",
				promoShortName = "Vk",
				HP = 20,
				STR = 70,
				AGI = 20,
				VIT = 50,
				LCK = 15,
				HIT = 3,
				MDEF = 2,
				armourWeight = MEDIUM,
				guaranteedWeapon = new List<WeaponSprite> { WeaponSprite.AXE },
				possibleWeapon = new List<WeaponSprite> { WeaponSprite.SHORTSWORD, WeaponSprite.LONGSWORD, WeaponSprite.SCIMITAR,
					WeaponSprite.RAPIER, WeaponSprite.FALCHION, WeaponSprite.HAMMER, WeaponSprite.KNIFE },
				weaponsMax = 6,
				averageWeapons = 1f
			});

			// Lancer / Dragoon
			classes.Add(new ClassDef()
			{
				name = "LANCER",
				promoName = "DRAGOON",
				shortName = "Ln",
				promoShortName = "Drg",
				HP = 24,
				STR = 70,
				AGI = 15,
				VIT = 40,
				LCK = 20,
				HIT = 3,
				MDEF = 4,
				armourWeight = KNIGHT,
				guaranteedWeapon = new List<WeaponSprite> { WeaponSprite.LONGSWORD },
				possibleWeapon = new List<WeaponSprite> { WeaponSprite.AXE, WeaponSprite.SCIMITAR,
					WeaponSprite.RAPIER, WeaponSprite.FALCHION, WeaponSprite.SHORTSWORD },
				weaponsMax = 3,
				averageWeapons = 3f
			});

			// Pirate / Corsair
			classes.Add(new ClassDef()
			{
				name = "PIRATE",
				promoName = "CORSAIR",
				shortName = "Pi",
				promoShortName = "Cr",
				HP = 21,
				STR = 33,
				AGI = 40,
				VIT = 24,
				LCK = 70,
				HIT = 2,
				MDEF = 1,
				mageLevel = 8,
				mageLevelPromotion = 0,
				spellChargeGrowth = 40,
				spellChargeMax = 5,
				possibleMagic = new List<string> { "death" },
				MagicSchoolsMax = 1,
				averageMagicSchools = 0.5f,
				armourWeight = LIGHT,
				promoArmourWeight = LIGHT,
				guaranteedWeapon = new List<WeaponSprite> { WeaponSprite.FALCHION, WeaponSprite.SCIMITAR },
				possibleWeapon = new List<WeaponSprite> { WeaponSprite.RAPIER, WeaponSprite.KNIFE },
				weaponsMax = 2,
				averageWeapons = 0.75f
			});

			// Fencer / Assassin
			classes.Add(new ClassDef()
			{
				name = "FENCER",
				promoName = "ASSASSIN",
				shortName = "Fr",
				promoShortName = "As",
				HP = 18,
				STR = 40,
				AGI = 75,
				VIT = 20,
				LCK = 30,
				HIT = 5,
				MDEF = 2,
				mageLevel = 8,
				mageLevelPromotion = 0,
				spellChargeGrowth = 40,
				spellChargeMax = 5,
				possibleMagic = new List<string> { "death", "poison", "status" },
				MagicSchoolsMax = 1,
				averageMagicSchools = 0.5f,
				promoMagic = new List<string> { "death", "poison", "space", "status" },
				armourWeight = CLOTH,
				guaranteedWeapon = new List<WeaponSprite> { WeaponSprite.RAPIER, WeaponSprite.KNIFE },
				possibleWeapon = new List<WeaponSprite> { WeaponSprite.FALCHION, WeaponSprite.SCIMITAR },
				weaponsMax = 2,
				averageWeapons = 0.75f,
				SteelLord = 0.4f
			});

			// Berserker / Gladiator
			classes.Add(new ClassDef()
			{
				name = "BERSERKR",
				promoName = "GLADIATR",
				shortName = "Br",
				promoShortName = "Gd",
				HP = 35,
				STR = 75,
				AGI = 10,
				VIT = 60,
				LCK = 5,
				HIT = 5,
				MDEF = 0,
				armourWeight = LIGHT,
				guaranteedWeapon = new List<WeaponSprite> { WeaponSprite.AXE, WeaponSprite.HAMMER },
				possibleWeapon = new List<WeaponSprite> { WeaponSprite.SHORTSWORD, WeaponSprite.LONGSWORD, WeaponSprite.SCIMITAR,
					WeaponSprite.RAPIER, WeaponSprite.FALCHION },
				weaponsMax = 3,
				averageWeapons = 0.75f,
				UnarmedAttack = 0.33f,
				ThorMaster = 0.33f
			});

			// Hunter / Ranger
			classes.Add(new ClassDef()
			{
				name = "HUNTER",
				promoName = "RANGER",
				shortName = "Hn",
				promoShortName = "Rng",
				HP = 18,
				STR = 20,
				AGI = 45,
				VIT = 24,
				LCK = 45,
				HIT = 4,
				MDEF = 1,
				mageLevel = 8,
				mageLevelPromotion = 0,
				spellChargeGrowth = 40,
				spellChargeMax = 5,
				possibleMagic = new List<string> { "time", "space", "health", "recovery", "ailment", "life" },
				MagicSchoolsMax = 3,
				averageMagicSchools = 1f,
				armourWeight = LIGHT,
				promoArmourWeight = LIGHT,
				guaranteedWeapon = new List<WeaponSprite> { WeaponSprite.FALCHION, WeaponSprite.RAPIER, WeaponSprite.KNIFE },
				WoodAdept = 0.5f
			});

			// Defender / Templar
			classes.Add(new ClassDef()
			{
				name = "DEFENDER",
				promoName = "TEMPLAR",
				shortName = "Def",
				promoShortName = "Tmp",
				HP = 35,
				STR = 40,
				AGI = 10,
				VIT = 70,
				LCK = 20,
				HIT = 3,
				MDEF = 5,
				mageLevel = 3,
				mageLevelPromotion = 3,
				spellChargeGrowth = 24,
				spellChargeMax = 3,
				possibleMagic = new List<string> { "earth", "buff", "ailment" },
				MagicSchoolsMax = 2,
				averageMagicSchools = 1.25f,
				promoMagic = new List<string> { "life", "ailment", "health", "elem" },
				armourWeight = HEAVY,
				guaranteedWeapon = new List<WeaponSprite> { WeaponSprite.HAMMER, WeaponSprite.LONGSWORD },
				possibleWeapon = new List<WeaponSprite> { WeaponSprite.FALCHION, WeaponSprite.AXE },
				weaponsMax = 2,
				averageWeapons = 0.75f,
				SteelLord = 0.4f
			});

			// Soldier / Dark Knight
			classes.Add(new ClassDef()
			{
				name = "SOLDIER",
				promoName = "D.KNIGHT",
				shortName = "Sld",
				promoShortName = "DrK",
				HP = 22,
				STR = 45,
				AGI = 15,
				VIT = 50,
				LCK = 33,
				HIT = 3,
				MDEF = 3,
				mageLevel = 0,
				mageLevelPromotion = 8,
				spellChargeGrowth = 24,
				spellChargeMax = 3,
				MagicSchoolsMax = 4,
				averageMagicSchools = 2f,
				promoMagic = new List<string> { "status", "grey", "death", "poison", "space", "time", "fire", "ice" },
				armourWeight = HEAVY,
				guaranteedWeapon = new List<WeaponSprite> { WeaponSprite.HAMMER, WeaponSprite.LONGSWORD },
				possibleWeapon = new List<WeaponSprite> { WeaponSprite.FALCHION, WeaponSprite.AXE },
				weaponsMax = 2,
				averageWeapons = 0.75f
			});

			// Squire / Paladin
			classes.Add(new ClassDef()
			{
				name = "SQUIRE",
				promoName = "PALADIN",
				shortName = "Sq",
				promoShortName = "Pl",
				HP = 35,
				STR = 40,
				AGI = 5,
				VIT = 40,
				LCK = 10,
				HIT = 3,
				MDEF = 3,
				mageLevel = 3,
				mageLevelPromotion = 5,
				spellChargeGrowth = 40,
				spellChargeMax = 5,
				possibleMagic = new List<string> { "health", "ailment", "life" },
				MagicSchoolsMax = 1,
				averageMagicSchools = 1f,
				promoMagic = new List<string> { "white", "recovery" },
				armourWeight = HEAVY,
				guaranteedWeapon = new List<WeaponSprite> { WeaponSprite.HAMMER, WeaponSprite.LONGSWORD },
				possibleWeapon = new List<WeaponSprite> { WeaponSprite.STAFF, WeaponSprite.IRONSTAFF },
				weaponsMax = 2,
				averageWeapons = 0.75f,
				ThorMaster = 0.2f
			});

			// Mystic Knight / Spellblade
			classes.Add(new ClassDef()
			{
				name = "M.KNIGHT",
				promoName = "SPLLBLDE",
				shortName = "Sq",
				promoShortName = "Pl",
				HP = 35,
				STR = 40,
				AGI = 5,
				VIT = 40,
				LCK = 10,
				HIT = 3,
				MDEF = 3,
				mageLevel = 8,
				mageLevelPromotion = 4,
				spellChargeGrowth = 60,
				spellChargeMax = 9,
				possibleMagic = new List<string> { "fire", "ice", "lit", "earth", "space" },
				MagicSchoolsMax = 1,
				averageMagicSchools = 3f,
				promoMagic = new List<string> { "elem", "grey", "space", "black", "time" },
				armourWeight = MEDIUM,
				guaranteedWeapon = new List<WeaponSprite> { WeaponSprite.RAPIER },
				possibleWeapon = new List<WeaponSprite> { WeaponSprite.LONGSWORD, WeaponSprite.SCIMITAR, WeaponSprite.KNIFE, WeaponSprite.FALCHION, WeaponSprite.SHORTSWORD },
				weaponsMax = 5,
				averageWeapons = 3.5f
			});

			// Juggler / Bard
			classes.Add(new ClassDef()
			{
				name = "JUGGLER",
				promoName = "BARD",
				shortName = "Hn",
				promoShortName = "Rng",
				HP = 9,
				STR = 12,
				AGI = 75,
				VIT = 8,
				LCK = 75,
				HIT = 1,
				MDEF = 1,
				mageLevel = 8,
				mageLevelPromotion = 0,
				spellChargeGrowth = 40,
				spellChargeMax = 5,
				possibleMagic = new List<string> { "buff", "status", "time", "space", "health", "recovery", "ailment" },
				MagicSchoolsMax = 3,
				averageMagicSchools = 3f,
				armourWeight = CLOTH,
				promoArmourWeight = CLOTH,
				guaranteedWeapon = new List<WeaponSprite> { WeaponSprite.KNIFE }
			});

			// Scholar / Sage
			classes.Add(new ClassDef()
			{
				name = "SCHOLAR",
				promoName = "SAGE",
				shortName = "Sc",
				promoShortName = "Sag",
				HP = 14,
				STR = 8,
				AGI = 17,
				VIT = 17,
				LCK = 5,
				HIT = 1,
				MDEF = 5,
				guaranteedMagic = new List<string> { "all" },
				mageLevel = 5,
				mageLevelPromotion = 5,
				spellChargeGrowth = 80,
				spellChargeMax = 9,
				armourWeight = CLOTH,
				promoArmourWeight = CLOTH,
				guaranteedWeapon = new List<WeaponSprite> { },
				possibleWeapon = new List<WeaponSprite> { WeaponSprite.STAFF, WeaponSprite.IRONSTAFF },
				weaponsMax = 2,
				averageWeapons = 0.5f
			});

			// Animist / Geomancer
			classes.Add(new ClassDef()
			{
				name = "ELMNTIST",
				promoName = "GEOMANCR",
				shortName = "An",
				promoShortName = "Gm",
				HP = 16,
				STR = 14,
				AGI = 14,
				VIT = 22,
				LCK = 25,
				HIT = 1,
				MDEF = 3,
				guaranteedMagic = new List<string> { "elem" },
				mageLevel = 5,
				mageLevelPromotion = 4,
				spellChargeGrowth = 72,
				spellChargeMax = 9,
				possibleMagic = new List<string> { "grey", "holy" },
				MagicSchoolsMax = 2,
				averageMagicSchools = 1f,
				armourWeight = CLOTH,
				promoArmourWeight = CLOTH,
				guaranteedWeapon = new List<WeaponSprite> { WeaponSprite.CHUCK, WeaponSprite.RAPIER },
				possibleWeapon = new List<WeaponSprite> { WeaponSprite.STAFF, WeaponSprite.IRONSTAFF },
				weaponsMax = 2,
				averageWeapons = 0.5f,
				UnarmedAttack = 0.2f
			});

			// Magus / Arcanist
			classes.Add(new ClassDef()
			{
				name = "MAGUS",
				promoName = "ARCANIST",
				shortName = "Mag",
				promoShortName = "Ar",
				HP = 16,
				STR = 18,
				AGI = 14,
				VIT = 16,
				LCK = 20,
				HIT = 1,
				MDEF = 3,
				guaranteedMagic = new List<string> { },
				mageLevel = 8,
				mageLevelPromotion = 4,
				spellChargeGrowth = 72,
				spellChargeMax = 9,
				possibleMagic = new List<string> { "fire", "lit", "health", "ice", "life", "earth", "death", "space", "holy", "time", "poison", "ailment" },
				MagicSchoolsMax = 3,
				averageMagicSchools = 3.25f,
				promoMagic = new List<string> { "fire", "lit", "health", "ice", "life", "earth", "death", "space", "holy", "time", "poison", "ailment" },
				armourWeight = CLOTH,
				promoArmourWeight = CLOTH,
				guaranteedWeapon = new List<WeaponSprite> { WeaponSprite.STAFF, WeaponSprite.IRONSTAFF }
			});

			// Priest / Cleric
			classes.Add(new ClassDef()
			{
				name = "SQUIRE",
				promoName = "PALADIN",
				shortName = "Sq",
				promoShortName = "Pl",
				HP = 23,
				STR = 10,
				AGI = 5,
				VIT = 20,
				LCK = 10,
				HIT = 1,
				MDEF = 3,
				mageLevel = 8,
				mageLevelPromotion = 5,
				spellChargeGrowth = 72,
				spellChargeMax = 9,
				guaranteedMagic = new List<string> { "recovery" },
				possibleMagic = new List<string> { "holy", "status" },
				MagicSchoolsMax = 1,
				averageMagicSchools = 0.75f,
				promoMagic = new List<string> { "holy", "buff" },
				armourWeight = MEDIUM,
				guaranteedWeapon = new List<WeaponSprite> { WeaponSprite.HAMMER },
				possibleWeapon = new List<WeaponSprite> { WeaponSprite.STAFF, WeaponSprite.IRONSTAFF },
				weaponsMax = 2,
				averageWeapons = 1.5f,
				ThorMaster = 0.2f
			});

			// Time Mage / Time Wizard
			classes.Add(new ClassDef()
			{
				name = "TimeMAGE",
				promoName = "TimeWIZ",
				shortName = "TmM",
				promoShortName = "TmW",
				HP = 12,
				STR = 8,
				AGI = 44,
				VIT = 12,
				LCK = 60,
				HIT = 1,
				MDEF = 5,
				guaranteedMagic = new List<string> { "time" },
				mageLevel = 8,
				mageLevelPromotion = 4,
				spellChargeGrowth = 72,
				spellChargeMax = 9,
				possibleMagic = new List<string> { "space", "lit", "ice" },
				MagicSchoolsMax = 1,
				averageMagicSchools = 1.5f,
				promoMagic = new List<string> { "space", "holy" },
				armourWeight = CLOTH,
				promoArmourWeight = CLOTH,
				guaranteedWeapon = new List<WeaponSprite> { WeaponSprite.HAMMER },
				possibleWeapon = new List<WeaponSprite> { WeaponSprite.STAFF, WeaponSprite.IRONSTAFF },
				weaponsMax = 2,
				averageWeapons = 0.5f
			});

			// Moogle / Mog Knight
			classes.Add(new ClassDef()
			{
				name = "MOOGLE",
				promoName = "MogKNGHT",
				shortName = "Mog",
				promoShortName = "MgK",
				HP = 20,
				STR = 38,
				AGI = 34,
				VIT = 42,
				LCK = 30,
				HIT = 3,
				MDEF = 3,
				guaranteedMagic = new List<string> { },
				mageLevel = 6,
				mageLevelPromotion = 3,
				spellChargeGrowth = 72,
				spellChargeMax = 2,
				possibleMagic = new List<string> { "lit", "ice", "fire" },
				MagicSchoolsMax = 3,
				averageMagicSchools = 2.5f,
				promoMagic = new List<string> { "earth", "holy", "space" },
				armourWeight = CLOTH,
				promoArmourWeight = KNIGHT,
				guaranteedWeapon = new List<WeaponSprite> { },
				possibleWeapon = new List<WeaponSprite> { WeaponSprite.HAMMER, WeaponSprite.LONGSWORD, WeaponSprite.SCIMITAR, WeaponSprite.FALCHION, WeaponSprite.AXE, WeaponSprite.SHORTSWORD, WeaponSprite.KNIFE, WeaponSprite.STAFF, WeaponSprite.IRONSTAFF, WeaponSprite.RAPIER },
				weaponsMax = 5,
				averageWeapons = 3f
			});

			// Chocobo / Black Chocobo
			classes.Add(new ClassDef()
			{
				name = "CHOCOBO",
				promoName = "Bl.CHOCO",
				shortName = "Ch",
				promoShortName = "BCh",
				HP = 22,
				STR = 48,
				AGI = 44,
				VIT = 32,
				LCK = 30,
				HIT = 5,
				MDEF = 2,
				armourWeight = CLOTH,
				promoArmourWeight = CLOTH,
				UnarmedAttack = 1f,
				CatClawMaster = 1f,
				SteelLord = 1f
			});

			return classes;
		}

		public void PopulateSpellTypes(FF1Rom rom)
		{
			var allMagic = rom.GetSpells();

			// All Spells
			spellFamilies.Add("all", allMagic); // 64 spells

			spellFamilies.Add("black", allMagic.Where(x => x.SpellSchool == SpellSchools.Black).ToList()); // 32 spells
			spellFamilies.Add("white", allMagic.Where(x => x.SpellSchool == SpellSchools.White).ToList()); // 32 spells

			spellFamilies.Add("buff", allMagic.Where(x => x.routine == SpellRoutine.ArmorUp || x.routine == SpellRoutine.DefElement || x.routine == SpellRoutine.Fast || x.routine == SpellRoutine.Sabr || x.routine == SpellRoutine.Lock || x.routine == SpellRoutine.Ruse || x.routine == SpellRoutine.Xfer).ToList()); // 15
			spellFamilies.Add("elem", allMagic.Where(x => x.elem == SpellElement.Fire || x.elem == SpellElement.Ice || x.elem == SpellElement.Lightning || x.elem == SpellElement.Earth).ToList()); // 13
			spellFamilies.Add("status", allMagic.Where(x => x.elem == SpellElement.Status || x.routine == SpellRoutine.Fear).ToList()); // 12
			spellFamilies.Add("recovery", allMagic.Where(x => x.routine == SpellRoutine.Heal || x.routine == SpellRoutine.CureAilment || x.routine == SpellRoutine.FullHeal || x.routine == SpellRoutine.Life).ToList()); // 11
			spellFamilies.Add("grey", allMagic.Where(x => x.elem == SpellElement.Death || x.elem == SpellElement.Poison || x.elem == SpellElement.Time).ToList()); // 10

			spellFamilies.Add("health", allMagic.Where(x => x.routine == SpellRoutine.Heal || x.routine == SpellRoutine.FullHeal).ToList()); // 6

			spellFamilies.Add("fire", allMagic.Where(x => x.elem == SpellElement.Fire).ToList()); // 5
			spellFamilies.Add("lit", allMagic.Where(x => x.elem == SpellElement.Lightning).ToList()); // 5
			spellFamilies.Add("ice", allMagic.Where(x => x.elem == SpellElement.Ice).ToList()); // 5
			spellFamilies.Add("time", allMagic.Where(x => x.elem == SpellElement.Time || x.routine == SpellRoutine.Fast || x.routine == SpellRoutine.Slow).ToList()); // 5

			spellFamilies.Add("death", allMagic.Where(x => x.elem == SpellElement.Death).ToList()); // 4
			spellFamilies.Add("earth", allMagic.Where(x => x.elem == SpellElement.Earth).ToList()); // 3
			spellFamilies.Add("poison", allMagic.Where(x => x.elem == SpellElement.Poison).ToList()); // 3

			spellFamilies.Add("ailment", allMagic.Where(x => x.routine == SpellRoutine.CureAilment).ToList()); // 3
			spellFamilies.Add("life", allMagic.Where(x => x.routine == SpellRoutine.Life).ToList()); // 2
			spellFamilies.Add("holy", allMagic.Where(x => x.SpellSchool == SpellSchools.White && (x.routine == SpellRoutine.Damage || x.routine == SpellRoutine.DamageUndead)).ToList()); // 5
			spellFamilies.Add("space", allMagic.Where(x => x.oobSpellRoutine == OOBSpellRoutine.EXIT || x.oobSpellRoutine == OOBSpellRoutine.WARP || (x.routine == SpellRoutine.Damage || x.elem == SpellElement.None)).ToList()); // 3
		}
	}

	public class ClassDef
	{
		// For conveniece
		public static FF1Rom rom;
		public static MT19337 rng;
		public static GearPermissions newPermissions;
		public int classIndex;

		// Armor Permission sets
		public static ushort CLOTH = (ushort)(EquipPermission.BlackBelt | EquipPermission.BlackMage | EquipPermission.BlackWizard | EquipPermission.Fighter | EquipPermission.Knight | EquipPermission.Master | EquipPermission.Ninja | EquipPermission.RedMage | EquipPermission.RedWizard | EquipPermission.Thief | EquipPermission.WhiteMage | EquipPermission.WhiteWizard);
		public static ushort CAPE = (ushort)(EquipPermission.BlackMage | EquipPermission.BlackWizard | EquipPermission.Fighter | EquipPermission.Knight | EquipPermission.Ninja | EquipPermission.RedMage | EquipPermission.RedWizard | EquipPermission.Thief | EquipPermission.WhiteMage | EquipPermission.WhiteWizard);

		public static ushort LIGHT = (ushort)(EquipPermission.BlackBelt | EquipPermission.Fighter | EquipPermission.Knight | EquipPermission.Master | EquipPermission.Ninja | EquipPermission.RedMage | EquipPermission.RedWizard | EquipPermission.Thief);
		public static ushort BUCKLR = (ushort)(EquipPermission.Fighter | EquipPermission.Knight | EquipPermission.Ninja | EquipPermission.RedMage | EquipPermission.RedWizard | EquipPermission.Thief);

		public static ushort MEDIUM = (ushort)(EquipPermission.Fighter | EquipPermission.Knight | EquipPermission.Ninja | EquipPermission.RedMage | EquipPermission.RedWizard);
		public static ushort BONKS = (ushort)(EquipPermission.Fighter | EquipPermission.Knight | EquipPermission.Ninja | EquipPermission.RedWizard);

		public static ushort HEAVY = (ushort)(EquipPermission.Fighter | EquipPermission.Knight | EquipPermission.Ninja);
		public static ushort HEALH = (ushort)(EquipPermission.Knight | EquipPermission.Ninja);
		public static ushort ZBONK = (ushort)(EquipPermission.Knight | EquipPermission.Ninja | EquipPermission.RedWizard);

		public static ushort KNIGHT = (ushort)(EquipPermission.Knight);

		public static ushort STEEL = (ushort)(EquipPermission.Fighter | EquipPermission.Knight);
		public static ushort BSHIRT = (ushort)(EquipPermission.BlackWizard);
		public static ushort WSHIRT = (ushort)(EquipPermission.WhiteWizard);
		public static ushort WIZARD = (ushort)(EquipPermission.BlackWizard | EquipPermission.WhiteWizard | EquipPermission.RedWizard);

		// String amd asset data
		public string name;
		public string promoName;

		public string shortName;
		public string promoShortName;

		public string graphicsPath;

		// Stats

		// Stats will be given a number between 0 and 75. Classbuilder will roll each stat ±25%, clamped 0-100.
		// Starting stats will be 1 if <12, 5 if <24, 10 if <36, and 15 if <48, etc For values that were clamped to above 50, the remainder will be added to starting stat.
		public int HP;     // HP starting will always just be 30 ± 5, this variable is for HP scaling. It is divided by half compared to the others
		public int STR;
		public int AGI;
		// int INT;  // INT continues to do nothing at the moment.
		public int VIT;
		public int LCK;

		// Hit and MDEF just get a number clamped from 1 to 5, rolled ±2
		public int HIT;
		public int MDEF;

		// Magic
		public List<String> guaranteedMagic = new List<String>();  // List of all spell groups (defined in Populate Spell Types) that this user will definitely have
		public int mageLevel;                                   // Level that the mage's spells will cap at, clamped 0-8 ±1. -1 is 'no cap', useful for limited things like Fire Magic.
		public int mageLevelPromotion = 4;                      // Number of levels to increase magic when promoted.

		public List<String> possibleMagic = new List<String>();     // List of all spell groups that this user could possibly have additionally
		public int MagicSchoolsMax;                             // Maximum number of additional magic schools to show up
		public float averageMagicSchools;                       // Average number of magic schools to show up. Each school in the list will be chosen or not based on averageMagicSchools / possibleMagic.size()

		public List<String> promoMagic = new List<String>();        // List of all spell groups that this user could possibly gain in addition to the base class spells on promotion

		public int spellChargeGrowth;          // Total spell casts by end of game, Clamped 0-72±50%. Will allocate them lower levels and up
		public int spellChargeMax = 9;         // Maximum spell charges total. Clamped 1-9±2.

		public int[] spellGrowthChart = new int[8];

		private List<String> finalSchools = new List<String>();
		private List<String> finalPromoSchools = new List<String>();

		// Weapons
		public List<WeaponSprite> guaranteedWeapon = new List<WeaponSprite>(); // Uses Weapon Sprites, not weapon types, because those are lopsided AF. Distribution: 2 CHUCK, 4 AXE, 3 HAMMER, 5 KNIFE (Katana and CatClaw), 3 STAFF (Wooden, Power, Wizard), 3 IRONSTAFF (Iron, Heal, Mage), 3 SHORTSWORD (Giant, Were, Short), 2 SCIMITAR (Masa, Scimitar), 4 FALCHION (Falchion, Flame, Vorpal, Rune), 5 LONGSWORD (Silver, XCal, Ice, Sun, Long), 6 RAPIER (Rapier, Sabre, Defense, Bane, Dragon, Coral)
		public List<WeaponSprite> possibleWeapon = new List<WeaponSprite>();       //		Out of those, CHUCK and SHORTSWORD have no end game weapons. Every other end game weapon will be promo locked.
		public int weaponsMax;                         // Maximum number of additional weapon types to show up
		public float averageWeapons;                   // Average number of weapon types to show up. Each types in the list will be chosen or not based on averageWeapons / possibleWeapon.size()
		private List<WeaponSprite> finalSets;

		// Armor
		public int armourWeight;   // See definitions above -> CLOTH, LIGHT, MEDIUM, HEAVY, KNIGHT
								   // Additionally certain conditions will offer excal, white shirt, black shirt, 
		public int promoArmourWeight = -1; // If -1, increase the armour Weight by a class. Other values as specified. 

		// Special abilities and blursings
		public byte InnateResist; // Only used when adding single elemental or status spell groups. "Fire Samurai is strong against Fire" kind of deal

		// Chance of having these abilities
		public float UnarmedAttack;
		public float CatClawMaster;
		public float ThorMaster;
		public float WoodAdept;
		public float SteelLord;

		// Returns a description string
		public String PublishToClass(int classIndex)
		{
			this.classIndex = classIndex;

			CommitNames(classIndex);

			CommitStats(classIndex);

			CommitSpells(classIndex);

			CommitWeapons(classIndex);

			CommitArmor(classIndex);

			CommitBlursingSpecials(classIndex);

			return GenerateDescription(classIndex);
		}

		// Returns the display string
		public String GenerateDescription(int classIndex)
		{
			// String is currently 11 characters wide and 24 lines tall.

			String description =

			// Everyone has these stats
			String.Format("HP{0,9}\n", HP * 20 + 49 * VIT / 4) +
			String.Format("STR.{0,7}\n", STR) +
			String.Format("AGL.{0,7}\n", AGI) +
			String.Format("MDEF{0,7}\n", "+" + MDEF) +
			String.Format("VIT.{0,7}\n", VIT) +
			String.Format("LCK.{0,7}\n", LCK) +
			String.Format("HIT %{0,6}\n", "+" + HIT);

			// If we have magic, add our spell charges
			//if (finalSchools.Count > 0 && spellChargeMax != 0)
			//{
			//	description += String.Format("MP{0,9}\n", spellChargeGrowth);

			//	// If the spell cap is lower than expected (9), inform us
			//	if (spellChargeMax != 9)
			//		description += String.Format("MP Max{0,5}\n", spellChargeMax);

			//	// If our magic level isn't max, let us know
			//	if (mageLevel != 8)
			//		description += String.Format("Lv{0} Magic  \n", mageLevel);
			//}

			// Add Weapon information here. There will never be more than 6 weapons.
			description += "\nEquipment  \n";

			if (finalSets.Count > 0)
			{
				var weaponTypes = "";

				if (rom.ClassData[(Classes)classIndex].UnarmedAttack)
					weaponTypes += "@U ";
				foreach (WeaponSprite w in finalSets)
				{
					switch (w)
					{
						case WeaponSprite.SHORTSWORD:
							weaponTypes += "@w ";
							break;
						case WeaponSprite.SCIMITAR:
							weaponTypes += "@c ";
							break;
						case WeaponSprite.FALCHION:
							weaponTypes += "@f ";
							break;
						case WeaponSprite.LONGSWORD:
							weaponTypes += "@S ";
							break;
						case WeaponSprite.RAPIER:
							weaponTypes += "@r ";
							break;
						case WeaponSprite.HAMMER:
							weaponTypes += "@H ";
							break;
						case WeaponSprite.KNIFE:
							weaponTypes += "@K ";
							break;
						case WeaponSprite.AXE:
							weaponTypes += "@X ";
							break;
						case WeaponSprite.STAFF:
							weaponTypes += "@F ";
							break;
						case WeaponSprite.IRONSTAFF:
							weaponTypes += "@R ";
							break;
						case WeaponSprite.CHUCK:
							weaponTypes += "@N ";
							break;
						default:
							break;
					}
				}

				description += weaponTypes.TrimEnd().PadRight(11);
				description += "\n";
			}

			// Add Armor information. Much shorter to code.
			description += getArmourString(armourWeight) + "\n";

			// Special abilities from Blursings
			if (rom.ClassData[(Classes)classIndex].CatClawMaster)
				description += "CatClaw Ace\n";
			if (rom.ClassData[(Classes)classIndex].ThorMaster)
				description += "Thor Master\n";
			if (rom.ClassData[(Classes)classIndex].SteelLord)
				description += "Steel Lord\n";
			if (rom.ClassData[(Classes)classIndex].WoodAdept)
				description += "Wood Adept\n";

			description += "\n";

			// Add in spells
			if (finalSchools.Count > 0 && spellChargeMax != 0)
			{
				description += "MagicSchool\n";
				foreach (String magicSchool in finalSchools)
				{
					var mSchool = magicSchool.ToUpper();
					var icon = getMagicIcon(magicSchool);

					description += icon + " " + mSchool + "\n";
				}
				description += "\n";

				// Test Spell Growth Example
				description += String.Format("Lv{4} {0}/{1}/{2}/{3}", spellGrowthChart[0], spellGrowthChart[1], spellGrowthChart[2], spellGrowthChart[3], mageLevel).PadRight(11) + "\n";
				description += String.Format("    {0}/{1}/{2}/{3}", spellGrowthChart[4], spellGrowthChart[5], spellGrowthChart[6], spellGrowthChart[7]).PadRight(11) + "\n";
				description += "\n";
			}

			// Lastly, add all the promotion buffs
			description += promoName + "\n";

			if (promoArmourWeight != armourWeight)
			{
				var pam = promoArmourWeight == -1 ? Math.Clamp(armourWeight + 1, 0, 4) : promoArmourWeight;
				description += " +" + getArmourString(pam) + "\n";
			}

			if (finalPromoSchools.Count > 0)
			{
				foreach (String magicSchool in finalPromoSchools)
				{
					var mSchool = magicSchool.ToUpper();
					var icon = getMagicIcon(magicSchool);

					description += " +" + icon + " " + mSchool + "\n";
				}
			}

			// If our magic level isn't max, let us know
			if (mageLevel != 8 && (finalPromoSchools.Count > 0 || finalSchools.Count > 0))
				description += String.Format(" +Lv{0} Magic\n", Math.Clamp(mageLevel + mageLevelPromotion, 0, 8));

			return description.TrimStart();
		}

		String getArmourString(int p)
		{
			if (p == 0)
				return "@T CLOTH";
			else if (p == 1)
				return "@A LIGHT";
			else if (p == 2)
				return "@A MEDIUM";
			else if (p == 3)
				return "@A HEAVY";
			else if (p == 4)
				return "@A KNIGHT";
			return "";
		}

		String getMagicIcon(String magicSchool)
		{
			var mSchool = magicSchool.ToUpper();

			//if (mSchool == "ALL") return "∞A";
			//if (mSchool == "BLACK") return "∞B";
			if (mSchool == "ELEM") return "€f€i€t€e";
			if (mSchool == "FIRE") return "€f";
			if (mSchool == "LIT") return "€t";
			if (mSchool == "ICE") return "€i";
			if (mSchool == "EARTH") return "€e";
			if (mSchool == "STATUS") return "€s";
			//if (mSchool == "GREY") return "∞G";
			if (mSchool == "DEATH") return "€d";
			if (mSchool == "POISON") return "€p";
			if (mSchool == "TIME") return "€T";
			//if (mSchool == "WHITE") return "∞W";
			//if (mSchool == "RECOVERY") return "∞R";
			//if (mSchool == "HEALTH") return "∞c";
			//if (mSchool == "AILMENT") return "∞a";
			//if (mSchool == "LIFE") return "∞l";
			//if (mSchool == "HOLY") return "∞h";
			//if (mSchool == "SPACE") return "∞s";
			//if (mSchool == "BUFF") return "∞U";

			return "";
		}

		// Replaces the display strings with new strings
		public void CommitNames(int classIndex)
		{
			rom.ItemsText[0xF0 + classIndex] = name.PadRight(8);
			rom.ItemsText[0xF0 + classIndex + 6] = promoName.PadRight(8);
		}

		// Commit Stats to the character class
		public void CommitStats(int classIndex)
		{
			Classes c = (Classes)classIndex;
			Classes p = (Classes)(classIndex + 6);

			RollStats();

			rom.ClassData[c].HpStarting = rom.ClassData[p].HpStarting = (byte)(HP % 50 + 30 + Rng.Between(rng, -5, 5));
			rom.ClassData[c].HpGrowth = rom.ClassData[p].HpGrowth = MakeGrowthTable(HP);

			rom.ClassData[c].StrStarting = rom.ClassData[p].StrStarting = (byte)(STR % 50 + Math.Clamp((STR / 12) * 5, 1, 100));
			rom.ClassData[c].StrGrowth = rom.ClassData[p].StrGrowth = MakeGrowthTable(STR);

			rom.ClassData[c].AgiStarting = rom.ClassData[p].AgiStarting = (byte)(AGI % 50 + Math.Clamp((AGI / 12) * 5, 1, 100));
			rom.ClassData[c].AgiGrowth = rom.ClassData[p].AgiGrowth = MakeGrowthTable(AGI);

			rom.ClassData[c].VitStarting = rom.ClassData[p].VitStarting = (byte)(VIT % 50 + Math.Clamp((VIT / 12) * 5, 1, 100));
			rom.ClassData[c].VitGrowth = rom.ClassData[p].VitGrowth = MakeGrowthTable(VIT);

			rom.ClassData[c].LckStarting = rom.ClassData[p].LckStarting = (byte)(LCK % 50 + Math.Clamp((LCK / 12) * 5, 1, 100));
			rom.ClassData[c].LckGrowth = rom.ClassData[p].LckGrowth = MakeGrowthTable(LCK);

			rom.ClassData[c].LckStarting = rom.ClassData[p].LckStarting = (byte)(LCK % 50 + Math.Clamp((LCK / 12) * 5, 1, 100));
			rom.ClassData[c].LckGrowth = rom.ClassData[p].LckGrowth = MakeGrowthTable(LCK);

			rom.ClassData[c].HitStarting = rom.ClassData[p].HitStarting = (byte)Rng.Between(rng, 2, 10);
			rom.ClassData[c].HitGrowth = rom.ClassData[p].HitGrowth = (byte)HIT;

			rom.ClassData[c].MDefStarting = rom.ClassData[p].MDefStarting = (byte)Rng.Between(rng, 10, 25);
			rom.ClassData[c].MDefGrowth = rom.ClassData[p].MDefGrowth = (byte)MDEF;
		}

		public void RollStats()
		{
			float up = 1.25f;
			float dwn = 0.75f;
			int v = 2; // roll modifier for Hit/MDEF

			HP = Math.Clamp(Rng.Between(rng, (int)(HP * dwn), (int)(HP * up)), 0, 100);
			STR = Math.Clamp(Rng.Between(rng, (int)(STR * dwn), (int)(STR * up)), 0, 100);
			AGI = Math.Clamp(Rng.Between(rng, (int)(AGI * dwn), (int)(AGI * up)), 0, 100);
			VIT = Math.Clamp(Rng.Between(rng, (int)(VIT * dwn), (int)(VIT * up)), 0, 100);
			LCK = Math.Clamp(Rng.Between(rng, (int)(LCK * dwn), (int)(LCK * up)), 0, 100);

			HIT = Math.Clamp(Rng.Between(rng, HIT - v, HIT + v), 0, 7);
			MDEF = Math.Clamp(Rng.Between(rng, MDEF - v, MDEF + v), 0, 7);
		}

		public List<bool> MakeGrowthTable(int value)
		{
			value = Math.Clamp(value, 0, 49);
			var growth = Enumerable.Repeat(true, value).ToList();

			growth.AddRange(Enumerable.Repeat(false, 49 - value).ToList());
			growth.Shuffle(rng);

			return growth;
		}

		// Commit Weapons to the character class
		public void CommitWeapons(int i)
		{
			var sets = RollWeaponSet();
			finalSets = sets;

			rom.ClassData._weaponPermissions.ClearPermissions((Classes)i);

			foreach (Weapon w in Weapon.LoadAllWeapons(rom, null))
			{ // First var is ROM
				if (sets.Contains(w.WeaponTypeSprite) & !ItemLists.UberTier.Contains(w.Id) & !ItemLists.LegendaryWeaponTier.Contains(w.Id) && w.Id != Item.Defense && w.Id != Item.LightAxe && w.Id != Item.ThorHammer && w.Id != Item.CatClaw)
					rom.ClassData._weaponPermissions.AddPermission((Classes)i, w.Id);
			}

			rom.ClassData._weaponPermissions.ClearPermissions((Classes)i + 6);

			foreach (Weapon w in Weapon.LoadAllWeapons(rom, null))
			{
				if (sets.Contains(w.WeaponTypeSprite))
					rom.ClassData._weaponPermissions.AddPermission((Classes)i + 6, w.Id);
			}
		}

		public List<WeaponSprite> RollWeaponSet()
		{
			List<WeaponSprite> ret = new();

			foreach (WeaponSprite w in possibleWeapon)
			{
				if (Rng.Between(rng, 0, 100) <= (int)((averageWeapons / possibleWeapon.Count) * 100))   // Times 100 because there's no float option and I'm lazy
					ret.Add(w);
				if (ret.Count >= weaponsMax)
					break;
			}

			ret.AddRange(guaranteedWeapon);

			return ret;
		}

		public void CommitArmor(int classIndex)
		{
			Classes c = (Classes)classIndex;
			Classes p = (Classes)(classIndex + 6);

			var pam = promoArmourWeight == -1 ? Math.Clamp(armourWeight + 1, 0, 4) : promoArmourWeight;
			foreach (Item i in ItemLists.AllArmor) {
				if (isEquippable(i, armourWeight))
					newPermissions.AddPermission(c, i);
				if (isEquippable(i, pam, true))
					newPermissions.AddPermission(p, i);
			}
		}

		public bool isEquippable(Item i, int weight, bool promoted = false)
		{

			// Weight Classes
			if (weight >= 0 && (rom.ArmorPermissions[i] == CLOTH || rom.ArmorPermissions[i] == CAPE))
				return true;

			if (weight >= 1 && (rom.ArmorPermissions[i] == LIGHT || rom.ArmorPermissions[i] == BUCKLR))
				return true;

			if (weight >= 2 && (rom.ArmorPermissions[i] == MEDIUM || rom.ArmorPermissions[i] == BONKS))
				return true;

			if (weight >= 3 && (rom.ArmorPermissions[i] == HEAVY || rom.ArmorPermissions[i] == STEEL || rom.ArmorPermissions[i] == HEALH || rom.ArmorPermissions[i] == ZBONK))
				return true;

			if (weight >= 4 && (rom.ArmorPermissions[i] == KNIGHT))
				return true;

			// Special Cases
			if (promoted)
			{
				if (rom.ArmorPermissions[i] == BSHIRT && (guaranteedMagic.Contains("all") || guaranteedMagic.Contains("black") || guaranteedMagic.Contains("elem")))
					return true;

				if (rom.ArmorPermissions[i] == WSHIRT && (guaranteedMagic.Contains("all") || guaranteedMagic.Contains("white") || guaranteedMagic.Contains("recovery") || guaranteedMagic.Contains("holy")))
					return true;

				if (rom.ArmorPermissions[i] == WIZARD && guaranteedMagic.Count > 0)
					return true;
			}

			// TODO: Add elemental equipment here if character is a non-mage and equipment is focused around that element

			return false;
		}

		public void CommitSpells(int classIndex)
		{
			Classes c = (Classes)classIndex;
			Classes p = (Classes)(classIndex + 6);

			// Choosing Spells and rolling for randomness
			var spells = RollSpellSet();
			var promoSpells = RollPromoSpellSet(spells);

			RollSpellCastingStats(classIndex);

			// Spell Permissions Committing
			rom.ClassData._spellPermissions.ClearPermissions(c);
			rom.ClassData._spellPermissions.ClearPermissions(p);

			HashSet<int> availableLevels = new HashSet<int>();
			HashSet<int> availablePromoLevels = new HashSet<int>();

			foreach (MagicSpell s in spells)
			{
				if (s.Level <= mageLevel)
				{
					rom.ClassData._spellPermissions.AddPermission(c, (SpellSlots)s.Index);
					availableLevels.Add(s.Level);
				}
			}

			foreach (MagicSpell sp in promoSpells)
			{
				if (sp.Level <= mageLevel + mageLevelPromotion)
				{
					rom.ClassData._spellPermissions.AddPermission(p, (SpellSlots)sp.Index);
					availablePromoLevels.Add(sp.Level);
				}
			}

			// Spell Charge Growth
			CommitSpellChargeGrowth(c, p, spells.Count == 0);
		}

		public List<MagicSpell> RollSpellSet()
		{
			// TODO 			public byte InnateResist; // Only used when adding single elemental or status spell groups. "Fire Samurai is strong against Fire" kind of deal


			List<MagicSpell> ret = new();

			foreach (string s in possibleMagic)
			{
				if (Rng.Between(rng, 0, 100) <= (int)((averageMagicSchools / possibleMagic.Count) * 100))  // Times 100 because there's no float option and I'm lazy
				{
					ret.AddRange(Transmooglifier.spellFamilies[s]);
					promoMagic.Remove(s); // Make sure we can acquire different spells on level up
					finalSchools.Add(s);
				}
				if (finalSchools.Count >= MagicSchoolsMax)
					break;
			}

			foreach (string s in guaranteedMagic)
			{
				ret.AddRange(Transmooglifier.spellFamilies[s]);
				finalSchools.Add(s);
			}

			return ret;
		}

		public List<MagicSpell> RollPromoSpellSet(List<MagicSpell> ret)
		{
			// TODO 			public byte InnateResist; // Only used when adding single elemental or status spell groups. "Fire Samurai is strong against Fire" kind of deal

			foreach (string s in promoMagic)
			{
				if (Rng.Between(rng, 0, 100) <= (int)((averageMagicSchools / possibleMagic.Count) * 100)) // Times 100 because there's no float option and I'm lazy
				{
					ret.AddRange(Transmooglifier.spellFamilies[s]);
					finalPromoSchools.Add(s);
				}
				if (finalPromoSchools.Count >= MagicSchoolsMax)
					break;
			}

			return ret;
		}

		public void RollSpellCastingStats(int classIndex)
		{
			Classes c = (Classes)classIndex;
			Classes p = (Classes)(classIndex + 6);

			mageLevel = Math.Clamp(Rng.Between(rng, mageLevel - 1, mageLevel + 1), 0, 8); // Level that the mage's spells will cap at, clamped 0-8 ±1. -1 is 'no cap', useful for limited things like Fire Magic.
			spellChargeGrowth = Math.Clamp(Rng.Between(rng, (int)(spellChargeGrowth * .5f), (int)(spellChargeGrowth * 1.5f)), 0, 100); // Total spell casts by end of game, Clamped 0-100±50%. Will allocate them lower levels and up
			spellChargeMax = Math.Clamp(Rng.Between(rng, spellChargeMax - 2, spellChargeMax + 2), 0, 9); // Maximum spell charges total. Clamped 1-9±2.
		}

		public void CommitSpellChargeGrowth(Classes c, Classes p, bool noBaseClassSpells)
		{
			List<byte> spellGrowth = new List<byte>();
			int[] spellCharges = new int[8];

			// Default Lists
			rom.ClassData[p].SpCGrowth = Enumerable.Repeat((byte)0, 49).ToList();
			rom.ClassData[c].SpCGrowth = Enumerable.Repeat((byte)0, 49).ToList();

			// No spells? No work
			if (finalPromoSchools.Count == 0 && finalSchools.Count == 0)
				return;

			var manaPool = spellChargeGrowth;

			// Many spells, much work
			for (int lvl = 49; lvl >= 0; lvl--)
			{
				int spells = (int)Math.Round((float)manaPool / (float)lvl);

				uint spellsLearnt = 0;

				for (; spells >= 0; spells--)
				{
					int spellLvl = ChooseNextLevel(spellCharges);
					if (spellCharges[spellLvl] >= spellChargeMax)
						continue;
					spellCharges[spellLvl]++;
					spellsLearnt = spellsLearnt | (byte)(0b1 << spellLvl);

					manaPool--;
					spellGrowthChart[spellLvl]++;
				}

				spellGrowth.Add((byte)spellsLearnt);
			}

			rom.ClassData[p].SpCGrowth = spellGrowth;
			rom.ClassData[p].MaxSpC = (byte)spellChargeMax;

			if (!noBaseClassSpells)
			{
				rom.ClassData[c].SpCGrowth = spellGrowth;
				rom.ClassData[c].MaxSpC = (byte)spellChargeMax;
				rom.ClassData[c].SpCStarting = 2;
			}
		}

		public int ChooseNextLevel(int[] spellCharges)
		{
			for (int i = 7; i > 0; i--)
			{
				if (i == 0)
					return 0;
				if ((spellCharges[i - 1] > spellCharges[i] + 1 || spellCharges[i - 1] == spellChargeMax) && spellCharges[i] != spellChargeMax)
					return i;
			}
			return 0;
		}

		public void CommitBlursingSpecials(int classIndex)
		{
			Classes c = (Classes)classIndex;
			Classes p = (Classes)(classIndex + 6);

			// Clear BBelt
			rom.ClassData[c].UnarmedAttack = false;

			if (Rng.Between(rng, 0, 100) <= (int)(UnarmedAttack * 100))
				rom.ClassData[c].UnarmedAttack = rom.ClassData[p].UnarmedAttack = true;

			if (Rng.Between(rng, 0, 100) <= (int)(CatClawMaster * 100))
				rom.ClassData[c].CatClawMaster = rom.ClassData[p].CatClawMaster = true;

			if (Rng.Between(rng, 0, 100) <= (int)(ThorMaster * 100))
				rom.ClassData[c].ThorMaster = rom.ClassData[p].ThorMaster = true;

			if (Rng.Between(rng, 0, 100) <= (int)(WoodAdept * 100))
				rom.ClassData[c].WoodAdept = rom.ClassData[p].WoodAdept = true;

			if (Rng.Between(rng, 0, 100) <= (int)(SteelLord * 100))
				rom.ClassData[c].SteelLord = rom.ClassData[p].SteelLord = true;
		}

		public Image<Rgba32> getImage(bool promoted = false)
		{
			var assembly = System.Reflection.Assembly.GetExecutingAssembly();

			string path;

			if (!promoted)
				path = assembly.GetManifestResourceNames().First(str => str.EndsWith(name + ".png"));
			else
				path = assembly.GetManifestResourceNames().First(str => str.EndsWith(promoName + ".png"));

			Image<Rgba32> image = Image.Load<Rgba32>(assembly.GetManifestResourceStream(path));

			return image;
		}

	}
}