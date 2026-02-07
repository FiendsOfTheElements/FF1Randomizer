using System.ComponentModel;
using System.Runtime.Versioning;
using DotNetAsm;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace FF1Lib
{
	public enum MusicShuffle
	{
		[Description("None")]
		None = 0,
		[Description("Standard")]
		Standard,
		[Description("Nonsensical")]
		Nonsensical,
		[Description("Disable Music")]
		MusicDisabled
	}

	public enum MenuColor
	{
		[Description("Default Blue")]
		Blue = 0x01,
		[Description("Dark Blue")]
		DarkBlue = 0x02,
		[Description("Purple")]
		Purple = 0x03,
		[Description("Pink")]
		Pink = 0x04,
		[Description("Red")]
		Red = 0x05,
		[Description("Orange")]
		Orange = 0x06,
		[Description("Dark Orange")]
		DarkOrange = 0x07,
		[Description("Brown")]
		Brown = 0x08,
		[Description("Light Green")]
		LightGreen = 0x09,
		[Description("Green")]
		Green = 0x0A,
		[Description("Dark Green")]
		DarkGreen = 0x0B,
		[Description("Cyan")]
		Cyan = 0x0C,
		[Description("Black")]
		Black = 0x0F,
	}

	public enum MapmanSlot
	{
		[Description("Leader")]
		Leader = 0x00,
		[Description("Second")]
		Second = 0x01,
		[Description("Third")]
		Third = 0x02,
		[Description("Fourth")]
		Fourth = 0x03,
	}
	public enum TitanSnack
	{
		[Description("Ruby")]
		Ruby = 0,
		[Description("Other Minerals")]
		Minerals = 1,
		[Description("Junk Food")]
		Junk = 2,
		[Description("Healthy Food")]
		Healthy = 3,
		[Description("Beverages")]
		Beverages = 4,
		[Description("All")]
		All = 5,
	}

	public enum Fate
	{
		[Description("Spare")]
		Spare = 0,
		[Description("Kill")]
		Kill = 2,
	}


	public enum FunEnemySpritesTeam
	{
		[Description("None")]
		None,
		[Description("One Random Team")]
		Random,
		[Description("All Teams")]
		All,
		[Description("Team Steak")]
		Steak,
		[Description("Team FFR")]
		FFR,
		[Description("Team Steamboat")]
		Steamboat,
		[Description("Team Dumpster Fire")]
		Dumpster,
		[Description("Team Glam")]
		Glam,
		[Description("Team Brillig")]
		Brillig,
		[Description("Team Meme")]
		Meme,
		[Description("Team Sports")]
		Sports,
		[Description("Team Mystaque")]
		Mystaque,
		[Description("Team Toxic")]
		Toxic,
		[Description("Team Party")]
		Party,
		[Description("Team Grind")]
		Grind,
		[Description("Original Team STEAK")]
		Legacy
	}

	public enum FunEnemyNames
	{
		[Description("None")]
		None,
		[Description("All")]
		All,
		[Description("Sprite Names Only")]
		Sprites
	}

	
	public partial class FF1Rom
	{
		public const int TyroPaletteOffset = 0x30FC5;
		public const int TyroSpriteOffset = 0x20560;

		public const int PaletteOffset = 0x30F20;
		public const int PaletteSize = 4;
		public const int PaletteCount = 64;

		//public SteakSprite teamSteak;
		
		

		public HashSet<EnemySprite> FunEnemySpritePool;



		public void FunEnemySprites(Flags flags, Preferences preferences, MT19337 rng)
		{
			FunEnemySpritesTeam team = preferences.FunEnemyTeam;

			//bool modeAll = (preferences.FunEnemyMode == FunEnemySpritesMode.All);

			bool modeRandom = preferences.FunEnemyMode;
			bool extras = preferences.FunEnemyExtras;

			Dictionary<FunEnemySpritesTeam, List<EnemySprite>> teamMembers = new()
			{
				{
					FunEnemySpritesTeam.Steak,
					new List<EnemySprite> {EnemySprite.Creep, EnemySprite.Worm, EnemySprite.Wyvern, EnemySprite.Tyro, EnemySprite.Ocho, EnemySprite.Garland}
				},
				{
					FunEnemySpritesTeam.FFR,
					new List<EnemySprite> {EnemySprite.Wolf, EnemySprite.Sahag, EnemySprite.OddEye, EnemySprite.Spider, EnemySprite.RAnkylo, EnemySprite.Hydra}
				},
				{
					FunEnemySpritesTeam.Steamboat,
					new List<EnemySprite> {EnemySprite.Iguana, EnemySprite.Pirate, EnemySprite.Bone, EnemySprite.Ogre, EnemySprite.Shadow, EnemySprite.Zombie}
				},
				{
					FunEnemySpritesTeam.Dumpster,
					new List<EnemySprite> {EnemySprite.Giant, EnemySprite.Earth, EnemySprite.Manticor, EnemySprite.Naga}
				},
				{
					FunEnemySpritesTeam.Glam,
					new List<EnemySprite> {EnemySprite.Medusa, EnemySprite.Pede, EnemySprite.Tiger, EnemySprite.Vampire, EnemySprite.Badman}
				},
				{
					FunEnemySpritesTeam.Brillig,
					new List<EnemySprite> {EnemySprite.FrostD, EnemySprite.Mummy, EnemySprite.Coctrice, EnemySprite.Madpony}	
				},
				{
					FunEnemySpritesTeam.Meme,
					new List<EnemySprite> {EnemySprite.Shark, EnemySprite.Bull, EnemySprite.Troll, EnemySprite.Catman, EnemySprite.Wizard}
				},
				{
					FunEnemySpritesTeam.Sports,
					new List<EnemySprite> {EnemySprite.Imp, EnemySprite.Hyena, EnemySprite.Gator, EnemySprite.Guard, EnemySprite.Chimera, EnemySprite.MudGol}
				},
				{
					FunEnemySpritesTeam.Mystaque,
					new List<EnemySprite> {EnemySprite.Asp, EnemySprite.Scorpion, EnemySprite.Eye, EnemySprite.Caribe, EnemySprite.GasD}
				},
				{
					FunEnemySpritesTeam.Toxic,
					new List<EnemySprite> {EnemySprite.Gargoyle, EnemySprite.Scum, EnemySprite.Water, EnemySprite.Astos}
				},
				{
					FunEnemySpritesTeam.Party,
					new List<EnemySprite> {EnemySprite.Asp, EnemySprite.Scorpion, EnemySprite.Bull, EnemySprite.Troll, EnemySprite.Vampire, EnemySprite.Wyvern, EnemySprite.Tyro}
				}
			};

			FunEnemySpritePool = new();

			

			List<EnemySprite> overworldGrind = new() {EnemySprite.Worm, EnemySprite.Wyvern, EnemySprite.Tyro};
			List<EnemySprite> vanillaSpike = new() {EnemySprite.Iguana, EnemySprite.Eye, EnemySprite.FrostD};
			List<EnemySprite> a_SideExtras = new() {EnemySprite.GasD};
			List<EnemySprite> b_SideExtras = new() {EnemySprite.GasD, EnemySprite.Vampire, EnemySprite.MudGol};

			HashSet<FunEnemySpritesTeam> allTeams = Enum.GetValues<FunEnemySpritesTeam>()
			   .Except(new FunEnemySpritesTeam[] {FunEnemySpritesTeam.None, FunEnemySpritesTeam.All, FunEnemySpritesTeam.Random, FunEnemySpritesTeam.Legacy})
			   .ToHashSet();

			if (team == FunEnemySpritesTeam.Random)
			{
				team = allTeams.PickRandom(rng);
			}
			// For tournament safe, we want to avoid replacing ALL the sprites.
			// Instead, we'll either choose all the sprites from one team, or one sprite from all the teams.
			if (flags.TournamentSafe && !modeRandom && team == FunEnemySpritesTeam.All)
			{
				if (rng.Between(1,10) > 5)
				{
					team = allTeams.PickRandom(rng);
				}
				else
				{
					modeRandom = true;
				}
			}
			// Team Grind won't work well with Enemizer, and could give hints about settings for blind flags
			if ((flags.BlindSeed || flags.EnemizerEnabled) && team == FunEnemySpritesTeam.Grind)
			{
				allTeams.Remove(FunEnemySpritesTeam.Grind);
				team = allTeams.PickRandom(rng);
			}
			// Build the pool for Team Grind. We care about enemies that could roll in, not about the ones that ACTUALLY roll in
			if (team == FunEnemySpritesTeam.Grind || team == FunEnemySpritesTeam.All)
			{
				List<EnemySprite> grind = new();
				grind.AddRange(overworldGrind);
				//if (flags.EnemyTrapTiles == TrapTileMode.Vanilla || flags.EnemyTrapTiles == TrapTileMode.Shuffle)
				if (new HashSet<TrapTileMode> {TrapTileMode.Vanilla, TrapTileMode.Shuffle, TrapTileMode.Random,
					TrapTileMode.ASideFormations, TrapTileMode.BSideFormations}.Contains(flags.EnemyTrapTiles))
				{
					grind.AddRange(vanillaSpike);
				}
				if (flags.EnemyTrapTiles == TrapTileMode.ASideFormations || flags.EnemyTrapTiles == TrapTileMode.Random)
				{
					grind.AddRange(a_SideExtras);
				}
				if (flags.EnemyTrapTiles == TrapTileMode.BSideFormations || flags.EnemyTrapTiles == TrapTileMode.Random)
				{
					grind.AddRange(b_SideExtras);
				}
				if (flags.EnemyTrapTiles == TrapTileMode.Overpowered)
				{
					grind.AddRange(a_SideExtras);
					grind.AddRange(b_SideExtras);
				}
				teamMembers.Add(FunEnemySpritesTeam.Grind, grind);
			}
			HashSet<FunEnemySpritesTeam> teams;
			if (team == FunEnemySpritesTeam.All)
			{
				teams = allTeams;
			}
			else
			{
				teams = new() {team};
			}
			if (team != FunEnemySpritesTeam.None)
			{
				foreach (FunEnemySpritesTeam t in teams)
				{
					if (modeRandom)
					{
						FunEnemySpritePool.Add(teamMembers[t].PickRandom(rng));
					}
					else
					{
						// foreach (EnemySprite member in teamMembers[t])
						// {
						// 	FunEnemySpritePool.Add(member);
						// }
						FunEnemySpritePool.UnionWith(teamMembers[t]);
					}
				}
			}

			if (preferences.RobotChicken)
			{
				FunEnemySpritePool.Add(EnemySprite.WarMech);
			}

			// some pairs that should appear together even if the "one random" mode is on
			if (FunEnemySpritePool.Contains(EnemySprite.Bull))
			{
				FunEnemySpritePool.Add(EnemySprite.Troll);
			}
			if (FunEnemySpritePool.Contains(EnemySprite.Troll))
			{
				FunEnemySpritePool.Add(EnemySprite.Bull);
			}

			if (FunEnemySpritePool.Contains(EnemySprite.Shadow))
			{
				FunEnemySpritePool.Add(EnemySprite.Zombie);
			}
			if (FunEnemySpritePool.Contains(EnemySprite.Zombie))
			{
				FunEnemySpritePool.Add(EnemySprite.Shadow);
			}

			if (FunEnemySpritePool.Contains(EnemySprite.Mummy))
			{
				FunEnemySpritePool.Add(EnemySprite.Coctrice);
			}
			if (FunEnemySpritePool.Contains(EnemySprite.Coctrice))
			{
				FunEnemySpritePool.Add(EnemySprite.Mummy);
			}

			if (FunEnemySpritePool.Contains(EnemySprite.Wyvern))
			{
				FunEnemySpritePool.Add(EnemySprite.Tyro);
			}
			if (FunEnemySpritePool.Contains(EnemySprite.Tyro))
			{
				FunEnemySpritePool.Add(EnemySprite.Wyvern);
			}

			foreach (EnemySprite sprite in FunEnemySpritePool)
			{
				// Sprites/EnemySprites.cs
				// Console.WriteLine($"Sprite: {sprite}");
				ImportFunEnemyImage(sprite);
			}

			if (team == FunEnemySpritesTeam.Legacy)
			{
				byte[] tyro = Blob.FromHex(
					"00000000000000000000000000000000" + "00000000000103060000000000000001" + "001f3f60cf9f3f7f0000001f3f7fffff" + "0080c07f7f87c7e60000008080f8f8f9" + "00000080c0e0f0780000000000000080" + "00000000000000000000000000000000" +
					"00000000000000000000000000000000" + "0c1933676f6f6f6f03070f1f1f1f1f1f" + "ffffffffffffffffffffffffffffffff" + "e6e6f6fbfdfffffff9f9f9fcfefefefe" + "3c9e4e26b6b6b6b6c0e0f0f878787878" + "00000000000000000000000000000000" +
					"00000000000000000000000000000000" + "6f6f6f6f673b190f1f1f1f1f1f070701" + "fffffec080f9fbffffffffffff8787ff" + "ff3f1f1f3ffdf9f3fefefefefefefefc" + "b6b6b6b6b6b6b6b67878787878787878" + "00000000000000000000000000000000" +
					"00000000000000000000000000000000" + "07070706060707070100000101010101" + "ffffff793080c0f0fffc3086cfffffff" + "e7fefcf9f26469e3f80103070f9f9e1c" + "264c983060c08000f8f0e0c080000000" + "00000000000000000000000000000000" +
					"00000000000000000000000000000000" + "07070706060301010101010101000000" + "f9f9f9797366ece8fefefefefcf97377" + "c68c98981830606038706060e0c08080" + "00000000000000000000000000000000" + "00000000000000000000000000000000" +
					"00000000000000000000000000000000" + "01010101010000000000000000000000" + "fb9b9b9b98ff7f006767676767000000" + "6060606060c080008080808080000000" + "00000000000000000000000000000000" + "00000000000000000000000000000000");
				EnemySprites[EnemySprite.Tyro] = tyro;
			}

		}

		public async Task SetFunFiendSprites(Flags flags, Preferences preferences, MT19337 rng)
		{
			
			if (!preferences.FunFiendSprites || (bool)flags.AlternateFiends || flags.TournamentSafe)
			{
				return;
			}
			var assembly = System.Reflection.Assembly.GetExecutingAssembly();
			var fiends = assembly.GetManifestResourceNames().First(str => str.EndsWith("fiends.png"));
			var chaos = assembly.GetManifestResourceNames().First(str => str.EndsWith("chaos.png"));
			var stream = assembly.GetManifestResourceStream(fiends);
			await SetCustomFiendGraphics(stream);
			stream = assembly.GetManifestResourceStream(chaos);
			await SetCustomChaosGraphics(stream);
		}

		public void SetFunEnemyExtras(Flags flags, Preferences preferences, DialogueData dialogues, MT19337 rng)
		{
			if (!preferences.FunEnemyExtras || flags.TournamentSafe || flags.EnemizerEnabled)
			{
				return;
			}
			// This changes some NPC sprites and dialogues.
			// Changes to Bahamut's enemy name and dialogue have to go in Bahamut.cs

			// some of this is in Enemies/Sprites.cs; eventually there will be a container class
			// for NPCs, and that will go in CharacterSprites.cs
			const int GARLAND_OFFSET = 0x0B400;
			const int BIKKE_OFFSET = 0x0B500;
			const int CANOE_OFFSET = 0x0A000;

			const int GARLAND_DIALOGUE = 0x04;
			const int BIKKE_DIALOGUE = 0x08;
			const int VAMPIRE_DIALOGUE = 0x1D;

			var assembly = System.Reflection.Assembly.GetExecutingAssembly();
			var garland = assembly.GetManifestResourceNames().First(str => str.EndsWith("garlandNPC.png"));
			var bikke = assembly.GetManifestResourceNames().First(str => str.EndsWith("bikkeNPC.png"));
			var canoe = assembly.GetManifestResourceNames().First(str => str.EndsWith("canoeNPC.png"));


			// COMPLETE hack here; later make more elegant
			if (FunEnemySpritePool.Contains(EnemySprite.Garland))
			{
				var stream = assembly.GetManifestResourceStream(garland);
				var image = Image.Load<Rgba32>(stream);
				for (int x = 0; x < 4; x++)
				{
					Put(GARLAND_OFFSET + x*64, MakePatternData(image,NPCGrayscaleIndex,0, x*16,2,2));
				}
				dialogues[GARLAND_DIALOGUE] = "No one touches my\nPrincess!!\nLIGHT WARRIORS??\nYou impertinent fools.\nI, Garland, smell like I\nlook - GARLIC!!";
		
			}
			if (FunEnemySpritePool.Contains(EnemySprite.Pirate))
			{
				var stream = assembly.GetManifestResourceStream(bikke);
				var image = Image.Load<Rgba32>(stream);
				for (int x = 0; x < 4; x++)
				{
					Put(BIKKE_OFFSET + x*64, MakePatternData(image,NPCGrayscaleIndex,0, x*16,2,2));
				}
				stream = assembly.GetManifestResourceStream(canoe);
				image = Image.Load<Rgba32>(stream);
				for (int x = 0; x < 6; x++)
				{
					Put(CANOE_OFFSET + x*64, MakePatternData(image,NPCGrayscaleIndex,0,x*16,2,2));
				}
				dialogues[BIKKE_DIALOGUE] = "Arrr, tis I,\nSteamboat Mikke, and\nsurprised I am that you\nscurvy curs have the\nnerve to face me.\nParrots!\nGet those landlubbers!";
			}
			if (FunEnemySpritePool.Contains(EnemySprite.Vampire))
			{
				dialogues[VAMPIRE_DIALOGUE] = "All living things were\nborn to slay.\nThe Vampire's flash will\ndazzle you.. SAY CHEESE!";
			}

		}

	    public void SetFunEnemyNames(Flags flags, Preferences preferences, MT19337 rng)
		{

			// NOTE: in previous versions, we needed to be careful about the total number of characters
			// in the enemy name strings. These are now distributed in different parts of the ROM,
			// so now the only constraint is for names not to exceed 8 characters total.

			// The fun name for Fight Bahamut is set in Bahamut.cs

			FunEnemyNames funNames = preferences.FunEnemyNames;
			bool altFiends = (bool)flags.AlternateFiends;
			

			if (funNames == FunEnemyNames.None || flags.EnemizerEnabled)
			{
				return;
			}

			Dictionary<int,List<string>> funEnemyNames = new()
			{	
				{Enemy.GrImp, 	new List<string> {"GrUMP"}},
				{Enemy.Wolf, 	new List<string> {"RURURU"}},
				{Enemy.GrWolf, 	new List<string> {"GrrrWOLF"}},
				{Enemy.FrWolf, 	new List<string> {"BrrrWOLF"}},
				{Enemy.FrGiant,	new List<string> {"FROSTY"}},
				{Enemy.GrOgre, 	new List<string> {"GeORGE"}},
				{Enemy.WzOgre, 	new List<string> {"DIRGE", "GROVER"}},
				{Enemy.Asp, 	new List<string> {"R.SNEK"}},
				{Enemy.Cobra,	new List<string> {"GrSNEK"}},
				{Enemy.SeaSnake,new List<string> {"SeaSNEK"}},
				{Enemy.Image,	new List<string> {"iMAGE"}},
				{Enemy.SandW,	new List<string> {"SANDWICH"}},
				{Enemy.Phantom, new List<string> {"WrongEYE"}},
				{Enemy.GrMedusa,new List<string> {"SNEKLADY"}},
				{Enemy.Pede,	new List<string> {"EXPEDE"}},
				{Enemy.WzVamp,	new List<string> {"EDWARD"}},
				{Enemy.RGoyle,	new List<string> {"ARGYLE"}},
				{Enemy.Slime,	new List<string> {"MtlSLIME"}},
				{Enemy.RAnkylo,	new List<string> {"FnPOLICE"}},
				{Enemy.WzMummy,	new List<string> {"MOMMY"}},
				{Enemy.Coctrice,new List<string> {"BIRB","JerkBIRD"}},
				{Enemy.Perilisk,new List<string> {"R.BIRB"}},
				{Enemy.Wyvern,	new List<string> {"Y BURN", "WYNGS"}},
				{Enemy.Naocho,	new List<string> {"NACHO"}},
				{Enemy.RHydra,	new List<string> {"HYDRANT"}},
				{Enemy.GrNaga,	new List<string> {"LadySNEK"}},
				{Enemy.GasD,	new List<string> {"Green D"}},
				{Enemy.Badman,	new List<string> {"BATMAN"}},
				{Enemy.Evilman,	new List<string> {"OKAYMAN"}},
				
			};

			Dictionary<int,List<string>> funFiendNames = new()
			{
				{Enemy.Lich,	new List<string> {"SpeedBMP", "S.BUMP"}},
				{Enemy.Lich2,	new List<string> {"SpeedBMP", "S.BUMP"}},
				{Enemy.Kary,	new List<string> {"KELLY"}},
				{Enemy.Kary2,	new List<string> {"KELLY"}}
			};

			Dictionary<int,List<string>> funEnemySpriteNames = new()
			{
				{Enemy.Imp,		new List<string> {"UMP"}},
				{Enemy.GrImp,	new List<string> {"GrUMP"}},
				{Enemy.Wolf,	new List<string> {"ONTERIER","OTTAWARG"}},
				{Enemy.GrWolf,	new List<string> {"OSLOBO","HOWLIFAX"}},
				{Enemy.WrWolf,	new List<string> {"CALGRURU","WINNIPUG"}},
				{Enemy.FrWolf,	new List<string> {"FrCANIDA","BrCOLOBO"}},
				{Enemy.Iguana,	new List<string> {"COWGUANA"}},
				{Enemy.Agama,	new List<string> {"COWGAMA"}},
				{Enemy.Sauria,	new List<string> {"BOVILISK"}},
				{Enemy.Giant,	new List<string> {"DJENT"}},
				{Enemy.FrGiant,	new List<string> {"UrDJENT"}},
				{Enemy.RGiant,	new List<string> {"TanDJENT"}},
				{Enemy.Sahag,	new List<string> {"S.WAGON"}},
				{Enemy.RSahag,	new List<string> {"R.FLYER"}},
				{Enemy.WzSahag, new List<string> {"WzWAGON","JckWAGON"}},
				{Enemy.Pirate,	new List<string> {"PARROT"}},
				{Enemy.Kyzoku,	new List<string> {"KOKATSU"}},
				{Enemy.Shark,	new List<string> {"BbySHARK"}},
				{Enemy.GrShark,	new List<string> {"GpaSHARK"}},
				{Enemy.OddEye,	new List<string> {"PinatEYE"}},
				{Enemy.BigEye,	new List<string> {"EXPinata"}},
				{Enemy.Bone,	new List<string> {"SKELDNCE"}},
				{Enemy.RBone,	new List<string> {"R.SKELTN"}},
				{Enemy.Creep,	new List<string> {"CREPE"}},
				{Enemy.Crawl,	new List<string> {"CRULLER"}},
				{Enemy.Hyena,	new List<string> {"HYLITE"}},
				{Enemy.Cerebus, new List<string> {"CERBALLR"}},
				{Enemy.Ogre,	new List<string> {"PETE"}},
				{Enemy.GrOgre,	new List<string> {"RePETE"}},
				{Enemy.WzOgre,	new List<string> {"ArmPETE"}},
				{Enemy.Asp,		new List<string> {"OROBOROS"}},
				{Enemy.Cobra,	new List<string> {"COBROROS"}},
				{Enemy.SeaSnake,new List<string> {"ETERNEEL"}},
				{Enemy.Scorpion,new List<string> {"SCIMP"}},
				{Enemy.Lobster,	new List<string> {"SHRIMP"}},
				{Enemy.Bull,	new List<string> {"BLOL"}},
				{Enemy.ZomBull,	new List<string> {"ZomBLOL"}},
				{Enemy.Troll,	new List<string> {"TROLFACE"}},
				{Enemy.SeaTroll,new List<string> {"SeeFOOD"}},
				{Enemy.Shadow,	new List<string> {"BlndMAUS"}},
				{Enemy.Image,	new List<string> {"MINIMAGE"}},
				{Enemy.Wraith,	new List<string> {"HntdMAUS"}},
				{Enemy.Ghost,	new List<string> {"MINREAPR"}},
				{Enemy.Zombie,	new List<string> {"ZOMBILLY"}},
				{Enemy.Ghoul,	new List<string> {"GHOAT"}},
				{Enemy.Geist,	new List<string> {"PLTRGOAT"}},
				{Enemy.Specter,	new List<string> {"REVENANY"}},
				{Enemy.Worm,	new List<string> {"GRUB"}},
				{Enemy.SandW,	new List<string> {"SANDWICH"}},
				{Enemy.GreyW,	new List<string> {"MealWORM"}},
				{Enemy.Eye,		new List<string> {"AWOOGA", "EYEWUV U"}},
				{Enemy.Phantom,	new List<string> {"FANTOOGA", "FANDOM"}},
				{Enemy.Medusa,	new List<string> {"MELISSA"}},
				{Enemy.GrMedusa,new List<string> {"MORGAN"}},
				{Enemy.Catman,	new List<string> {"WINKID"}},
				{Enemy.Mancat,	new List<string> {"KIDWIN"}},
				{Enemy.Pede,	new List<string> {"PdASTAIR"}},
				{Enemy.GrPede,	new List<string> {"PdKELLY"}},
				{Enemy.Tiger,	new List<string> {"KNITYCAT"}},
				{Enemy.SaberT,	new List<string> {"YARNIVOR"}},
				{Enemy.Vampire,	new List<string> {"KODAKULA"}},
				{Enemy.WzVamp,	new List<string> {"NSFRAZZI"}},
				{Enemy.Gargoyle,new List<string> {"GARGLE"}},
				{Enemy.RGoyle,	new List<string> {"ARSENIC"}},
				{Enemy.Earth,	new List<string> {"DARTH"}},
				{Enemy.Fire,	new List<string> {"DIRE"}},
				{Enemy.FrostD,	new List<string> {"Bander S"}},
				{Enemy.RedD,	new List<string> {"Jabber W"}},
				{Enemy.ZombieD,	new List<string> {"ManxomeF"}},
				{Enemy.Scum,	new List<string> {"SCMPANZI"}},
				{Enemy.Muck,	new List<string> {"MUCKAQUE"}},
				{Enemy.Ooze,	new List<string> {"BABOOZE"}},
				{Enemy.Slime,	new List<string> {"SLIMATE"}},
				{Enemy.Spider,	new List<string> {"WhlsSPDR"}},
				{Enemy.Arachnid,new List<string> {"DaniSPDR"}},
				{Enemy.Manticor,new List<string> {"METALCOR","WENDYCAR"}},
				{Enemy.Sphinx,	new List<string> {"SPHYNTH"}},
				{Enemy.RAnkylo, new List<string> {"FnPOLICE"}},
				{Enemy.Ankylo,	new List<string> {"AnkyLEO"}},
				{Enemy.Mummy,	new List<string> {"MomeRATH"}},
				{Enemy.WzMummy, new List<string> {"OUTGRABR"}},
				{Enemy.Coctrice,new List<string> {"JUBJUB"}},
				{Enemy.Perilisk,new List<string> {"BOROGOVE"}},
				{Enemy.Wyvern,	new List<string> {"WYNGS"}},
				{Enemy.Wyrm,	new List<string> {"HotWYNGS"}},
				{Enemy.Tyro,	new List<string> {"STEAK"}},
				{Enemy.TRex,	new List<string> {"BluSTEAK","T BONE"}},
				{Enemy.Caribe,	new List<string> {"CARIBINR"}},
				{Enemy.RCaribe,	new List<string> {"SNAPPER"}},
				{Enemy.Gator,	new List<string> {"G8ER BOI"}},
				{Enemy.FrGator,	new List<string> {"OLLIEG8R"}},
				{Enemy.Ocho,	new List<string> {"GUAC"}},
				{Enemy.Naocho,	new List<string> {"NACHO"}},
				{Enemy.Hydra,	new List<string> {"HYDRANT"}},
				{Enemy.RHydra,	new List<string> {"F.HYDRNT"}},
				{Enemy.Guard,	new List<string> {"L.GUARD"}},
				{Enemy.Sentry,	new List<string> {"CENTER"}},
				{Enemy.Water,	new List<string> {"HURLPOOL"}},
				{Enemy.Air,		new List<string> {"SWAIR"}},
				{Enemy.Naga,	new List<string> {"SnekDRUM"}},
				{Enemy.GrNaga,	new List<string> {"LadyNAGA"}},
				{Enemy.Chimera,	new	List<string> {"CHICEPS"}},
				{Enemy.Jimera,	new List<string> {"GYMERA"}},
				{Enemy.Wizard,	new List<string> {"R.U.A.Wz"}},
				{Enemy.Sorcerer,new List<string> {"MNDFAILR"}},
				{Enemy.Garland,	new List<string> {"GARLIC"}},
				{Enemy.GasD,	new List<string> {"GasDRIVN"}},
				{Enemy.BlueD,	new	List<string> {"VlksDRGN"}},
				{Enemy.MudGol,	new List<string> {"MudGOLIE"}},
				{Enemy.RockGol,	new List<string> {"PtRckROY"}},
				{Enemy.IronGol, new List<string> {"IRonHXTL"}},
				{Enemy.Badman,	new	List<string> {"GADFLY"}},
				{Enemy.Evilman, new List<string> {"GOTHMOTH","ARTHUR"}},
				{Enemy.Astos,	new List<string> {"ZOSTOS"}},
				{Enemy.Mage,	new List<string> {"PHAGE"}},
				{Enemy.Fighter,	new List<string> {"FOMITE"}},
				{Enemy.Madpony,	new List<string> {"SL.TOVE"}},
				{Enemy.Nitemare,new List<string> {"GyreTOVE"}},
				{Enemy.WarMech,	new List<string> {"RoboCHKN","WarBAWK"}},
			};

			Dictionary<int, List<string>> funFiendSpriteNames = new()
			{
				{Enemy.Lich,	new List<string> {"GLICH"}},
				{Enemy.Lich2,	new List<string> {"GLICH"}},
				{Enemy.Kary,	new List<string> {"KELLY"}},
				{Enemy.Kary2,	new List<string> {"KELLY"}},
				{Enemy.Kraken,	new List<string> {"BRAKEN"}},
				{Enemy.Kraken2,	new List<string> {"BRAKEN"}},
				{Enemy.Tiamat,	new List<string> {"TIACAT"}},
				{Enemy.Tiamat2,	new List<string> {"TIACAT"}},
				{Enemy.Chaos,	new List<string> {"CHAOS","MNDLBROT","LORENZ","ENTROPER"}}
			};


			
			if (funNames == FunEnemyNames.All)
			{	
				foreach (int enemy in funEnemyNames.Keys)
				{
					EnemyText[enemy] = funEnemyNames[enemy].PickRandom(rng);
				}

				if (!altFiends)
				{
					foreach (int fiend in funFiendNames.Keys)
					{
						EnemyText[fiend] = funFiendNames[fiend].PickRandom(rng);
					}
				}
			}


			List<int> enemies = funEnemySpriteNames.Keys.ToList();

			foreach (int enemy in enemies)
			{
				if (FunEnemySpritePool.Contains(EnemySpriteMap[enemy]))
				{
					EnemyText[enemy] = funEnemySpriteNames[enemy].PickRandom(rng);
				}
			}

			if (preferences.FunEnemyTeam == FunEnemySpritesTeam.Legacy)
			{
				EnemyText[Enemy.Tyro] = funEnemySpriteNames[Enemy.Tyro].PickRandom(rng);
				EnemyText[Enemy.TRex] = funEnemySpriteNames[Enemy.TRex].PickRandom(rng);
			}

			if (!altFiends && preferences.FunFiendSprites)
			{
				foreach (int fiend in funFiendSpriteNames.Keys)
				{
					EnemyText[fiend] = funFiendSpriteNames[fiend].PickRandom(rng);
				}
			}
			
		}

		public void PaletteSwap(bool enable, MT19337 rng)
		{
			if (!enable)
			{
				return;
			}

			var palettes = Get(PaletteOffset, PaletteSize * PaletteCount).Chunk(PaletteSize);

			palettes.Shuffle(rng);

			Put(PaletteOffset, Blob.Concat(palettes));
		}

		

		public void DynamicWindowColor(MenuColor menuColor)
		{
			// This is an overhaul of LoadBorderPalette_Blue that enhances it to JSR to
			// DrawMapPalette first. That allows us to wrap that with a dynamic load of
			// the bg color after it sets it to the default one.
			/*
				LoadBorderPalette_Dynamic:
				JSR $D862 ; JSR to DrawMapPalette
				LDY $60FB ; Load dynamic palette color to Y

				LoadBorderPalette_Y:
				LDA $60FC ; Back up current bank
				PHA
				LDA #$0F
				JSR $FE03 ; SwapPRG_L
				JSR $8700 ; Jump out to palette writing code. Dynamic Color in Y
				PLA
				JSR $FE03 ; SwapPRG_L
				RTS
			*/
			Put(0x7EB29, Blob.FromHex("2062D8ACFB60ADFC6048A90F2003FE200087682003FE60"));

			// The battle call site needs black not the dynamic color so we jump right to
			// the operation after that when calling from battle init.
			Put(0x7EB90, Blob.FromHex("A00F4C2FEB"));

			// Modify two calls to DrawMapPalette to call our LoadBorderPalette_Dynamic which
			// starts with a JSR to DrawMapPalette and then adds the dynamic menu color.
			Put(0x7CF8F, Blob.FromHex("29EB"));
			Put(0x7CF1C, Blob.FromHex("29EB"));

			// Modify Existing calls to LoadBorderPalette_Blue up three bytes to where it starts
			Put(0x7EAB7, Blob.FromHex("2CEB"));
			Put(0x7EB58, Blob.FromHex("2CEB"));

			// There are two unfinished bugs in the equipment menu that use palettes 1 and 2
			// for no reason and need to use 3 now. They are all mirrors in vanilla.
			Put(0x3BE53, Blob.FromHex("EAEAEAEAEAEAEAEAEAEA"));
			Data[0x3BEF7] = 0x60;

			// Finally we need to also make the lit orb palette dynamic so lit orbs bg matches.
			// I copy the original code and add LDA/STA at the end for the bg color, and put
			// it over some unused garbage at the bottom of Bank E @ [$BF3A :: 0x3BF4A]
			/*
				LDX #$0B ; Straight copy from EnterMainMenu
				Loop:
				  LDA $AD78, X
				  STA $03C0, X
				  DEX
				  BPL Loop

				LDA $60FB ; Newly added to load and set dynamic palette color for lit orb
				STA $03C2
				RTS
			*/
			Put(0x3BF3A, Blob.FromHex("A20BBD78AD9DC003CA10F7ADFB608DC2038DC60360"));
			Put(0x3ADC2, Blob.FromHex("203ABFEAEAEAEAEAEAEAEA"));

			// Dynamic location initial value
			Data[0x30FB] = (byte)menuColor;

			// Hardcoded spot for opening "cinematic"
			Data[0x03A11C] = (byte)menuColor;
			Data[0x03A2D3] = (byte)menuColor;
		}

		public void EnableModernBattlefield(bool enable)
		{
			if (!enable)
			{
				return;
			}

			// Since we're changing the window color in the battle scene we need to ensure that
			// $FF tile remains opaque like in the menu screen. That battle init code
			// overwrites it with transparent so we skip that code here. Since this is fast
			// enough we end up saving a frame we add a wait for VBlank to take the same total time.
			Put(0x7F369, Blob.FromHex("2000FEEAEAEAEAEAEAEAEAEAEAEAEAEAEA"));
			Put(0x7EB90, Blob.FromHex("4C29EB"));

			// Don't draw big battle boxes around the enemies, party, and individual stats.
			// Instead draw one box in the lower right corner around the new player stats.
			Put(0x7F2E4, Blob.FromHex("A9198538A913A206A00A20E2F3EAEAEAEAEAEAEAEAEAEAEAEAEA"));
			Put(0x7F2FB, Blob.FromHex("EAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEAEA"));

			// The bottom row of these boxes was occluded by the Command Menu and enemy list so
			// there is code to redraw it whenever it would be exposed that we early return to skip.
			Data[0x7F62D] = 0x60;

			// To match later games and make better use of screen real estate we move all the bottom
			// boxes down a tile. This requires rewriting most of the battle box and text positioning
			// LUTs. They are largely formatted in a HeaderByte, X, Y, W, H system.
			// lut_EnemyRosterBox      HDXXYYWWHH
			Put(0x7F9E4, Blob.FromHex("0001010B0A"));

			//                           BOX      TEXT
			// lut_CombatBoxes:        HDXXYYWWHHHDXXYY
			Put(0x7F9E9, Blob.FromHex("0001010A04010202" + // attacker name
									  "000B010C04010C02" + // their attack("FROST", "2Hits!" etc)
									  "0001040A04010205" + // defender name
									  "000B040B04010C05" + // damage
									  "0001071804010208"));// bottom message("Terminated", "Critical Hit", etc)

			// lut_BattleCommandBox    HDXXYYWWHH
			Put(0x7FA1B, Blob.FromHex("000C010D0A"));

			// lut for Command Text    HDXXYYTPTR
			Put(0x7FA20, Blob.FromHex("010E0239FA" + // FIGHT
									  "010E043EFA" + // MAGIC
									  "010E0643FA" + // DRINK
									  "010E0848FA" + // ITEM
									  "0114024DFA"));// RUN

			// Edit lut for where to draw character status
			Put(0x32B21, Blob.FromHex("9A22DA221A235A23"));

			// Move character sprites to the right edge and space them a little apart
			byte xCoord = 0xD8;
			Data[0x31741] = xCoord;
			Data[0x3197D] = xCoord;                // overwrite hardcoded when stoned
			Data[0x31952] = (byte)(xCoord - 0x08); // 8px to the left when dead.

			byte yCoord = 0x2B;
			for (int i = 0; i < 4; ++i)
			{
				Data[0x3174F + (i * 5)] = (byte)(yCoord + (i * 28));
			}

			// Update Several 16 Byte LUTS related to cursors
			for (int i = 0; i < 8; ++i)
			{
				Data[0x31F85 + i * 2] = (byte)(xCoord - 0x10);            // X Coord of Character targeting cursor
				Data[0x31F86 + i * 2] = (byte)(yCoord + 4 + ((i % 4) * 0x1C));  // Y Coord of Character targeting cursor
				Data[0x31F76 + i * 2] = (byte)(0xA7 + (Math.Min(i, 4) % 4 * 0x10)); // Y Coord of Command Menu cursor (last four are identical)
			}

			// Start backdrop rows two tiles left and one upward. This matches FF2/3
			Data[0x7F34B] = 0x20;
			Data[0x7F352] = 0x40;
			Data[0x7F359] = 0x60;
			Data[0x7F360] = 0x80;

			// Adjust backdrop to draw in sets of 8 tiles so it appears to keep repeating.
			Put(0x7F38C, Blob.FromHex("A008849B20A0F320A0F320A0F3EAEAEAEA"));

			// Shorten the DrawStatus section to print only name or status, not both. Just one line.
			/* ASM Snippet
				LDY #$01        ; Need a one offset into
				LDA ($82), Y    ; btl_ob_charstat_ptr + 1 is status
				LSR             ; Shift dead bit to carry
				BCS skip        ; If dead print name
				BEQ skip        ; If healthy print name
				LDY #$09        ; otherwise load 9 to print status string
				skip:
				JSR $AAFC       ; JSR DrawStatusRow
			*/
			Put(0x32AB0, Blob.FromHex("A001B1824AB004F002A00920FCAAEAEAEAEAEAEA"));

			// Overwrite the upper portion of the default attribute table to all bg palette
			Put(0x7F400, Blob.FromHex("0000000000000000"));
			Put(0x7F408, Blob.FromHex("0000000000000000"));

			// Fix NT bits inside the drawing sequence of mixed enemies for expanded backdrop.
			// Before it would reset the palette to greyscale because it used to be borders.
			Data[0x2E6C9] = 0x70;
			Data[0x2E6CD] = 0xB0;

			// Fix NT bits inside chaostsa.bin and fiendtsa.bin
			Data[0x2D4C8] = 0xB0;
			for (int i = 0; i < 0x0140; i += 0x50)
			{
				Data[0x2D320 + i] = 0x00;
			}
		}

		public void UseVariablePaletteForCursorAndStone(bool enable)
		{
			if (!enable)
			{
				return;
			}

			// The masamune uses the same palette as the cursor and stone characters
			// so we can free up a whole palette if we reset the varies palette to
			// the masamune palette after every swing and magic annimation. The only
			// drawback is that stoned characters will flash with attacks and magic.

			// Change UpdateVariablePalette to edit Palette 3
			Data[0x32B35] = 0xA4;
			Data[0x32B3B] = 0xA5;
			Data[0x32B41] = 0xA6;
			Data[0x32B46] = 0xA3;
			Data[0x32B4E] = 0xA6;

			// Make magic use palette 3
			Data[0x318F0] = 0x03;

			// Make sparks use palette 3
			Data[0x33E47] = 0x03;

			// Weapon palettes are embedded in this lut with their coordinates.
			Put(0x3202C, Blob.FromHex("0001020303030303"));
			Put(0x32034, Blob.FromHex("0100030243434343"));

			// Increase loop variable to do 12 colors when fading sprites in and out for inn animation.
			Data[0x7FF23] = 12;
			Data[0x7FF43] = 12;
			Data[0x7FF65] = 12;

			// Enable this feature by rewriting the JSR BattleFrame inside UpdateSprites_BattleFrame
			Put(0x31904, Blob.FromHex("20F1FD207CA060"));
		}
		public void ChangeLute(bool changelute, DialogueData dialogues, MT19337 rng)
		{
			if (!changelute)
			{
				return;
			}


			var woodwinds = new List<string> {"FLUTE","PICCOLO","OBOE","CLARNET","SAX","BASSOON","RECORDR","OCARINA","PANFLUT","BAGPIPE","WHISTLE","HRMNICA"};
			var brass 	  = new List<string> {"HORN","TRUMPET","CORNET","FLUGEL","BUGLE","TROMBON","TUBA","FLUBA"};
			var stringInst= new List<string> {"VIOLIN","VIOLA","CELLO","DBLBASS","OCTBASS","LYRE","HARP",
											  "GUITAR","BASS","ELECGTR","BANJO","FIDDLE","MNDOLIN","UKULELE","SITAR","HRDYGRD","DULCIMR"};
			var percussion= new List<string> {"DRUM","SNARE D","BASSDRM","TIMPANI","TMBRINE","CYMBALS","TRIANGL","COWBELL","GONG","FNGRCYM",
											  "MARIMBA","XYLOPHN","GLCKSPL","STLDRUM","BOOMWHK"};
			var keyboards = new List<string> {"PIANO","B GRAND","ORGAN","HRPSCRD","CLVCHRD","SYNTH"};
			var voice     = new List<string> {"SOPRANO","SINGER","SONG","KAZOO","DITTY","JINGLE","LIMRICK","POEM","HAIKU"};
			var noise     = new List<string> {"FOGHORN","AIRHORN","VUVUZELA","THREMIN","TESLA","DGERIDO","WINDCH","AEOLUS"};

			
			var newInstruments = new List<string>()
					.Concat(woodwinds)
					.Concat(brass)
					.Concat(stringInst)
					.Concat(percussion)
					.Concat(keyboards)
					.Concat(voice)
					.Concat(noise)
				.ToList();
			
			
			

			//var dialogs = ReadText(dialogsPointerOffset, dialogsPointerBase, dialogsPointerCount);

			var newLute = newInstruments.PickRandom(rng);
			//Console.WriteLine($"Instrument: {newLute}");
			// handle extra dialogues that might contain the LUTE if the NPChints flag is enabled or if Astos Shuffle is enabled
			var dialogsUpdate = SubstituteKeyItemInExtraNPCDialogues("LUTE", newLute, dialogues); ;
			var princessDialogue = dialogues[0x06].Split(new string[] { "LUTE" }, System.StringSplitOptions.RemoveEmptyEntries);
			var monkDialogue = dialogues[0x35].Split(new string[] { "LUTE" }, System.StringSplitOptions.RemoveEmptyEntries);

			if (princessDialogue.Length > 1)
				dialogsUpdate.Add(0x06, princessDialogue[0] + newLute + princessDialogue[1]);

			if (monkDialogue.Length > 1)
				dialogsUpdate.Add(0x35, monkDialogue[0] + newLute + monkDialogue[1].Substring(0,14) + "\n" + monkDialogue[1].Substring(15, 10).Replace('\n',' '));

			if (dialogsUpdate.Count > 0)
				dialogues.InsertDialogues(dialogsUpdate);

			ItemsText[(int)Item.Lute] = newLute;
		}

		public void ChangeFountainText(bool changeFountain, DialogueData dialogues, MT19337 rng)
		{
			if (!changeFountain)
			{
				return;
			}

			List<string> roasts = new()
			{
				"See your face upon the\nclear water. SO DIRTY!\nCome,\n        wash your face.",
				"See your face upon the\ndirty water. How clean!\nFilter the water.",
				"Sparkling Water Fountain\n    Brought to you by\n    Bubbles Sodo Co.\n\n      Car-Bo-Nate",
				"Fill your cup. You have\na long journey ahead\nof you.\n\nGood Luck Have Fun!",
				"See your face upon the\nclean water. How nice!\nYou have a pretty face.",
				"You take a moment to\nlook at the fountain.\nThe sound of rushing\nwater fills you with\nDetermination.",
				"Are you thirsty?\nTake a moment to drink\nthe endlessly recycled\nwater.",
				"I am no ordinary well.\nI am a unique fountain,\nblessed by the light\nwariors.",
				"WASH YOUR FACE!!!!",
				"You may wash your face,\nbut come to the inn to\nget a full private bath!",
				"Please go to the\nordinary well to fill\nme back up.",
				"Sploosh, Splash.\nThe fountain sprayed\nyour face.",
				"You examine the fountain.\nIt is only a trickle.\nThe water element\nhas been corrupted."
			};

			string roast = roasts.PickRandom(rng);

			//roast = roasts[12]; //for testing

			dialogues[0x136] = roast;
		}

		public void HurrayDwarfFate(Fate fate, NpcObjectData npcdata, DialogueData dialogues, MT19337 rng)
		{
			if (fate == Fate.Spare)
			{
				// Protect Hurray Dwarf from NPC guillotine
				npcdata[ObjectId.DwarfcaveDwarfHurray].Script = TalkScripts.Talk_norm;
			}
			else
			{
				// Whether NPC guillotine is on or not, kill Hurray Dwarf
				npcdata[ObjectId.DwarfcaveDwarfHurray].Script = TalkScripts.Talk_kill;

				// Change the dialogue
				var dialogueStrings = new List<string>
				{
				    "No! I'm gonna disappear.\nYou'll never see\nme again. Please,\nI don't want to die.",
					"If you strike me down,\nI shall become more\npowerful than you can\npossibly imagine.",
					"Freeeeedom!!",
					"I've seen things you\npeople wouldn't believe.\nAll those moments will\nbe lost in time..\nlike tears in rain..\nTime to die.",
					"Become vengeance, David.\nBecome wrath.",
					"My only regret..\nis that I have boneitis.",
					"No, not the bees!\nNOT THE BEES!\nAAAAAAAAGH!\nTHEY'RE IN MY EYES!\nMY EYES! AAAAAAAAAAGH!",
					"This is blasphemy!\nThis is madness!",
					"Not like this..\nnot like this..",
					"Suicide squad, attack!\n\n\n\nThat showed 'em, huh?",
					"Well, what are you\nwaiting for?\nDo it. DO IT!!",
					"The path you walk on has\nno end. Each step you\ntake is paved with the\ncorpses of your enemies.\nTheir souls will haunt\nyou forever. Hear me!\nMy spirit will be\nwatching you!",
					"K-Kefka..!\nY-you're insane.."
				};

				//Put new dialogue to E6 since another Dwarf also says hurray
				dialogues[0xE6] = dialogueStrings.PickRandom(rng);
				npcdata[ObjectId.DwarfcaveDwarfHurray].Dialogue1 = 0xE6;
				npcdata[ObjectId.DwarfcaveDwarfHurray].Dialogue2 = 0xE6;
				npcdata[ObjectId.DwarfcaveDwarfHurray].Dialogue3 = 0xE6;
			}
		}

		public void TitanSnack(TitanSnack snack, NpcObjectData npcdata, DialogueData dialogues, MT19337 rng)
		{
			if (snack == FF1Lib.TitanSnack.Ruby)
			{
				return;
			}

			var snackOptions = new List<string>(); // { "NEWRUBY(max 7 characters);NEWRUBYPLURALIZED(max 8 characters);IS/ARE(relating to plural form);DESCRIPTOR(max 6 characters);ONOMATOPOEIA(max 6 chars, how ingestion sounds)" }
			var mineralSnacks = new List<string> { "DIAMOND;DIAMONDS;ARE;SWEET;CRUNCH", "GEODE;GEODES;ARE;SWEET;CRUNCH", "COAL;COAL;IS;SMOKY;CRUNCH", "PEARL;PEARLS;ARE;SWEET;CRUNCH", "FOSSIL;FOSSILS;ARE;SWEET;CRUNCH", "EMERALD;EMERALDS;ARE;SWEET;CRUNCH", "TOPAZ;TOPAZ;IS;SWEET;CRUNCH", "QUARTZ;QUARTZ;IS;SWEET;CRUNCH", "ONYX;ONYXES;ARE;SWEET;CRUNCH", "MARBLE;MARBLE;IS;SWEET;CRUNCH", "AMETHST;AMETHST;IS;SWEET;CRUNCH", "JADE;JADES;ARE;SWEET;CRUNCH", "SAPHIRE;SAPHIRE;IS;SWEET;CRUNCH", "GRANITE;GRANITE;IS;SWEET;CRUNCH", "OBSDIAN;OBSDIAN;IS;SWEET;CRUNCH", "CONCRET;CONCRET;IS;SALTY;CRUNCH", "ASPHALT;ASPHALT;IS;SALTY;CRUNCH", "PUMICE;PUMICE;IS;SWEET;CRUNCH", "LIMESTN;LIMESTN;IS;SOUR;CRUNCH", "SNDSTON;SNDSTON;IS;SALTY;CRUNCH", "MYTHRL;MYTHRL;IS;SWEET;CRUNCH" };
			var junkFoodSnacks = new List<string> { "DANISH;DANISHES;ARE;SWEET;MUNCH", "HOT DOG;HOT DOGS;ARE;GREAT;MUNCH", "TACO;TACOS;ARE;GREAT;MUNCH", "SUB;SUBS;ARE;GREAT;MUNCH", "PIZZA;PIZZA;IS;YUMMY;MUNCH", "BURGER;BURGERS;ARE;YUMMY;MUNCH", "EGGROLL;EGGROLLS;ARE;YUMMY;MUNCH", "BISCUIT;BISCUITS;ARE;YUMMY;MUNCH", "WAFFLE;WAFFLES;ARE;YUMMY;MUNCH", "CAKE;CAKE;IS;SWEET;MUNCH", "PIE;PIE;IS;SWEET;MUNCH", "DONUT;DONUTS;ARE;SWEET;MUNCH", "FRIES;FRIES;ARE;SALTY;MUNCH", "CHIPS;CHIPS;ARE;SALTY;CRUNCH", "CANDY;CANDY;IS;SWEET;MUNCH", "PANCAKE;PANCAKES;ARE;SWEET;MUNCH", "ICE CRM;ICE CRM;IS;CREAMY;MUNCH", "PUDDING;PUDDING;IS;YUMMY;MUNCH", "BROWNIE;BROWNIES;ARE;SWEET;MUNCH", "CRAYON;CRAYONS;ARE;WEIRD;MUNCH", "GLUE;GLUE;IS;WEIRD;MUNCH", "PASTE;PASTE;IS;WEIRD;MUNCH", "LASAGNA;LASAGNA;IS;YUMMY;MUNCH", "POUTINE;POUTINE;IS;GREAT;MUNCH", "PASTA;PASTA;IS;YUMMY;MUNCH", "RAMEN;RAMEN;IS;GREAT;MUNCH", "STEAK;STEAK;IS;GREAT;MUNCH", "NACHOS;NACHOS;ARE;SALTY;CRUNCH", "BACON;BACON;IS;SALTY;MUNCH", "MUTTON;MUTTON;IS;GREAT;MUNCH", "BAGEL;BAGELS;ARE;GREAT;MUNCH", "CHEESE;CHEESE;IS;GREAT;MUNCH", "POPCORN;POPCORN;IS;SALTY;MUNCH", "CHICKEN;CHICKEN;IS;GREAT;MUNCH", "BEEF;BEEF;IS;GREAT;MUNCH", "HAM;HAM;IS;GREAT;MUNCH", "BOLOGNA;BOLOGNA;IS;GREAT;MUNCH", "HOAGIE;HOAGIES;ARE;GREAT;MUNCH", "FILET;FILET;IS;DIVINE;MUNCH", "LOBSTER;LOBSTER;IS;DIVINE;MUNCH", "SHEPPIE;SHEPPIE;IS;SAVORY;MUNCH", "MEATLOF;MEATLOF;IS;SAVORY;MUNCH", "ENCHLDA;ENCHLDAS;ARE;CHEESY;MUNCH", "BAKLAVA;BAKLAVAS;ARE;SWEET;MUNCH","CANNOLI;CANNOLI;ARE;SWEET;CRUNCH","TIRMISU;TIRMISU;IS;SWEET;MUNCH","CHZQAKE;CHZQAKE;IS;CREAMY;MUNCH","PIEROGI;PIEROGIES;ARE;YUMMY;MUNCH","KEBAB;KEBABS;ARE;YUMMY;MUNCH","KOFTE;KOFTE;IS;YUMMY;MUNCH" };
			var healthySnacks = new List<string> { "EDAMAME;EDAMAME;IS;SALTY;MUNCH", "SALAD;SALAD;IS;GREAT;MUNCH", "APPLE;APPLES;ARE;SWEET;CRUNCH", "PEAR;PEARS;ARE;SWEET;MUNCH", "MELON;MELONS;ARE;SWEET;MUNCH", "ORANGE;ORANGES;ARE;SWEET;MUNCH", "LEMON;LEMONS;ARE;SOUR;MUNCH", "YOGURT;YOGURT;IS;GREAT;MUNCH", "GRANOLA;GRANOLA;IS;GREAT;CRUNCH", "SPINACH;SPINACH;IS;YUMMY;MUNCH", "EGG;EGGS;ARE;YUMMY;MUNCH", "GRAPES;GRAPES;ARE;YUMMY;MUNCH", "OATMEAL;OATMEAL;IS;GREAT;MUNCH", "TOFU;TOFU;IS;WEIRD;MUNCH", "CABBAGE;CABBAGE;IS;FRESH;MUNCH", "LETTUCE;LETTUCE;IS;FRESH;MUNCH", "TOMATO;TOMATOES;ARE;YUMMY;MUNCH", "SUSHI;SUSHI;IS;FISHY;MUNCH", "TUNA;TUNA;IS;FISHY;MUNCH", "SALMON;SALMON;IS;FISHY;MUNCH", "FISH;FISH;IS;FRESH;MUNCH", "BEANS;BEANS;ARE;YUMMY;MUNCH", "CEREAL;CEREAL;IS;GREAT;MUNCH", "PRETZEL;PRETZELS;ARE;SALTY;MUNCH", "EGGSALD;EGGSALAD;IS;GREAT;MUNCH", "RICE;RICE;IS;PLAIN;MUNCH", "CAVIAR;CAVIAR;IS;DIVINE;MUNCH" };
			var beverages = new List<string> { "BEER;BEER;IS;SMOOTH;GULP", "WINE;WINE;IS;RICH;GULP", "TEA;TEA;IS;FRESH;GULP", "COFFEE;COFFEE;IS;FRESH;GULP", "COLA;COLA;IS;SWEET;GULP", "COCOA;COCOA;IS;SWEET;GULP", "ICEDTEA;ICEDTEA;IS;SWEET;GULP", "LMONADE;LEMONADE;IS;SWEET;GULP", "MILK;MILK;IS;GREAT;GULP", "LATTE;LATTES;ARE;CREAMY;GULP", "WATER;WATER;IS;FRESH;GULP", "TEQUILA;TEQUILA;IS;SMOOTH;GULP" };

			switch (snack)
			{
				case FF1Lib.TitanSnack.Minerals:
					snackOptions = mineralSnacks;
					break;
				case FF1Lib.TitanSnack.Junk:
					snackOptions = junkFoodSnacks;
					break;
				case FF1Lib.TitanSnack.Healthy:
					snackOptions = healthySnacks;
					break;
				case FF1Lib.TitanSnack.Beverages:
					snackOptions = beverages;
					break;
				case FF1Lib.TitanSnack.All:
					// combine all lists together
					foreach (string mineral in mineralSnacks) { snackOptions.Add(mineral); }
					foreach (string junkFood in junkFoodSnacks) { snackOptions.Add(junkFood); }
					foreach (string healthySnack in healthySnacks) { snackOptions.Add(healthySnack); }
					foreach (string beverage in beverages) { snackOptions.Add(beverage); }
					break;
				default:
					return;
			}
			var randomRuby = snackOptions.PickRandom(rng);

			var newRubyItemDescription = "A tasty treat."; // Replaces "A large red stone." (can't be too long else it'll overwrite next phrase: "The plate shatters,")
			var newTitanCraving = "is hungry."; // replaces "eat gems." (can't be too long or will appear outside window)
			if (beverages.Contains(randomRuby))
			{
				newRubyItemDescription = "A tasty drink.";
				newTitanCraving = "is thirsty.";
			}
			else if (mineralSnacks.Contains(randomRuby))
			{
				newRubyItemDescription = "Feels heavy.";
			}
			// replace "A red stone." item description (0x38671) originally "8AFFAF2FAA1A23A724285AC000"
			MenuText.MenuStrings[(int)FF1Text.MenuString.UseRuby] = FF1Text.TextToBytes(newRubyItemDescription, useDTE: true);

			// phrase parts
			var newRubyContent = randomRuby.Split(";");
			var newRuby = newRubyContent[0];
			var newRubyPluralized = newRubyContent[1];
			var newRubySubjectVerbAgreement = newRubyContent[2];
			var newRubyTastes = newRubyContent[3];
			var newRubyOnomatopoeia = newRubyContent[4];
			var newRubyArticle = ""; // newRubySubjectVerbAgreement;
			if (newRubySubjectVerbAgreement == "ARE")
			{
				if (newRuby[0] == 'A' || newRuby[0] == 'E' || newRuby[0] == 'I' || newRuby[0] == 'O' || newRuby[0] == 'U')
				{
					newRubyArticle = "an ";
				}
				else
				{
					newRubyArticle = "a ";
				}
			}

			// handle extra dialogues that might contain the RUBY if the NPChints flag is enabled
			var dialogsUpdate = SubstituteKeyItemInExtraNPCDialogues("RUBY", newRuby, dialogues);

			// begin substitute phrase parts
			var titanDeepDungeon = dialogues[0x29].Split(new string[] { "a RUBY" }, System.StringSplitOptions.RemoveEmptyEntries);
			var titanDialogue = dialogues[0x2A].Split(new string[] { "RUBY", "Crunch, crunch, crunch,", "sweet", "Rubies are" }, System.StringSplitOptions.RemoveEmptyEntries);
			var melmondManDialogue = dialogues[0x7B].Split(new string[] { "eats gems.", "RUBIES" }, System.StringSplitOptions.RemoveEmptyEntries);

			// Bring me a {newRuby} if you
			// wish to skip to floor 22.
			if (titanDeepDungeon.Length > 1)
			{
				dialogsUpdate.Add(0x29, titanDeepDungeon[0] + newRubyArticle + newRuby + titanDeepDungeon[1]);
			}

			// If you want pass, give
			// me the {newRuby}..
			// {Onomatopoeia}, {onomatopoeia}, {onomatopoeia},
			// mmm, it tastes so {newRubyTastes}.
			// {newRubyPluralized} {newRubySubjectVerbAgreement} my favorite.
			if (titanDialogue.Length > 3)
			{
				dialogsUpdate.Add(0x2A, titanDialogue[0] + newRuby + titanDialogue[1]
					+ CapitalizeFirstLowercaseRest(newRubyOnomatopoeia) + ", " + newRubyOnomatopoeia.ToLower() + ", " + newRubyOnomatopoeia.ToLower() + "," + titanDialogue[2]
					+ newRubyTastes.ToLower() + titanDialogue[3]
					+ CapitalizeFirstLowercaseRest(newRubyPluralized) + " " + newRubySubjectVerbAgreement.ToLower() + titanDialogue[4]);
			}
			else if (titanDialogue.Length > 1)
			{
				// handle Shuffle Astos, alternate Titan dialog "If you want pass, give\nme the RUBY..\nHa, it mine! Now, you in\ntrouble. Me am Astos,\nKing of the Titans!"
				dialogsUpdate.Add(0x2A, titanDialogue[0] + newRuby + titanDialogue[1]);
			}
			// The Titan who lives in
			// the tunnel {newTitanCraving}
			// He loves {newRubyPluralized}.
			if (melmondManDialogue.Length > 2)
				dialogsUpdate.Add(0x7B, melmondManDialogue[0] + newTitanCraving + melmondManDialogue[1] + newRubyPluralized + melmondManDialogue[2]);
			// end substitute phrase parts

			if (dialogsUpdate.Count > 0)
				dialogues.InsertDialogues(dialogsUpdate);

			// substitute key item
			ItemsText[(int)Item.Ruby] = newRuby;
		}

		private Dictionary <int,String> SubstituteKeyItemInExtraNPCDialogues(string original, string replacement, DialogueData dialogues)
		{
			var dialogsUpdate = new Dictionary<int, string>();
			// Add extra dialogues that might contain the {original} if the NPChints flag is enabled or if Astos Shuffle is enabled
			var otherNPCs = new List<byte> {
				0x45, 0x53, 0x69, 0x82, 0xA0, 0xAA, 0xCB, 0xDC, 0x9D, 0x70, 0xE3, 0xE1, 0xB6, // NPChints
				0x02, 0x0E, 0x12, 0x14, 0x16, 0x19, 0x1E, 0xCD, 0x27, 0x23, 0x2B // ShuffleAstos
			};

			for (int i = 0; i < otherNPCs.Count(); i++)
			{
				var tempDialogue = dialogues[otherNPCs[i]].Split(new string[] { original.ToUpper().Trim() }, System.StringSplitOptions.RemoveEmptyEntries);
				if (tempDialogue.Length > 1)
					dialogsUpdate.Add(otherNPCs[i], tempDialogue[0] + replacement.ToUpper().Trim() + tempDialogue[1]);
			}

			return dialogsUpdate;
		}

		private static string CapitalizeFirstLowercaseRest(string s)
		{
			return String.Format("{0}{1}", s.First().ToString().ToUpper(), s.Substring(1).ToLower());
		}

	}

}
