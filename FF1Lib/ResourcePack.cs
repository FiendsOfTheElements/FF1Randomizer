using System.IO.Compression;
using Newtonsoft.Json;

namespace FF1Lib
{
	public partial class FF1Rom : NesRom
	{

	    bool ResourcePackHasGameplayChanges(Stream stream) {
	        var resourcePackArchive = new ZipArchive(stream);
                if (resourcePackArchive.GetEntry("spellbook.json") != null) return true;
                if (resourcePackArchive.GetEntry("overworld.json") != null) return true;
		return false;
	    }

	

		async Task LoadFunTiles(Preferences preferences)
		{
			if (preferences.MapDerp)
			{
				var assembly = System.Reflection.Assembly.GetExecutingAssembly();
				var mapderpPath = assembly.GetManifestResourceNames().First(str => str.EndsWith("mapderp.png"));
				var mapderpStream = assembly.GetManifestResourceStream(mapderpPath);
				await SetCustomMapGraphics(mapderpStream, 245, 4,
							new int[] { OVERWORLDPALETTE_OFFSET },
							OVERWORLDPALETTE_ASSIGNMENT,
							OVERWORLDPATTERNTABLE_OFFSET,
							OVERWORLDPATTERNTABLE_ASSIGNMENT);
			}
		}

		// split Resource Pack loading into two task sets: those which should be done pre ROM expansion
		// and those which should be done after. This is a little slippery, but has mostly
		// to do with the order in which things are done in the randomizer itself.
		// In the future, there may need to be a third set, which applies changes after
		// randomization. 

		async Task LoadResourcePackPreROM(string resourcepack, Preferences preferences)
		{
			if (resourcepack == null)
			{
				return;
			}
			using (var stream = new MemoryStream(Convert.FromBase64String(resourcepack)))
			{
				var resourcePackArchive = new ZipArchive(stream);


				// before ROM expansion, map data is loaded and stored in
				// data structures that are easy to read but difficult to edit.
				// If we want the randomizer to operate on a new tileset for any reason,
				// those should be loaded into the vanilla ROM

				var maptiles = resourcePackArchive.GetEntry("maptiles.png");
				if (maptiles != null)
				{
					using (var s = maptiles.Open())
					{
						await SetCustomMapGraphics(s, 245, 4,
								 new int[] { OVERWORLDPALETTE_OFFSET },
								 OVERWORLDPALETTE_ASSIGNMENT,
								 OVERWORLDPATTERNTABLE_OFFSET,
								 OVERWORLDPATTERNTABLE_ASSIGNMENT);
					}
				}

				var towntiles = resourcePackArchive.GetEntry("towntiles.png");
				if (towntiles != null)
				{
					using (var s = towntiles.Open())
					{
						int cur_tileset = 0;
						await SetCustomMapGraphics(s, 128, 4,
								 new int[] {
						 MAPPALETTE_OFFSET + (0 * 0x30),
						 MAPPALETTE_OFFSET + (1 * 0x30),
						 MAPPALETTE_OFFSET + (2 * 0x30),
						 MAPPALETTE_OFFSET + (3 * 0x30),
						 MAPPALETTE_OFFSET + (4 * 0x30),
						 MAPPALETTE_OFFSET + (5 * 0x30),
						 MAPPALETTE_OFFSET + (6 * 0x30),
						 MAPPALETTE_OFFSET + (7 * 0x30),
								 },
								 TILESETPALETTE_ASSIGNMENT + (cur_tileset << 7),
								 TILESETPATTERNTABLE_OFFSET + (cur_tileset << 11),
								 TILESETPATTERNTABLE_ASSIGNMENT + (cur_tileset << 9));
					}
				}

				var npcs = resourcePackArchive.GetEntry("npcs.png");
				if (npcs != null)
				{
					using (var s = npcs.Open())
					{
						SetCustomNPCGraphics(s);
					}
				}

			}

			

		}
		async Task LoadResourcePackPostROM(string resourcepack, DialogueData dialogues, EnemyScripts enemyScripts, Preferences preferences)
		{
			if (resourcepack == null)
			{
				return;
			}
			using (var stream = new MemoryStream(Convert.FromBase64String(resourcepack)))
			{
				var resourcePackArchive = new ZipArchive(stream);

				
				// heroes.png depends on extra ROM space for storing alternate palettes.
				// The other sprite resources are neutral on the pre vs. post ROM expansion
				// so it makes sense to just put them all here.

				var spritesheet = resourcePackArchive.GetEntry("heroes.png");
				if (spritesheet != null)
				{
					using (var s = spritesheet.Open())
					{
						// the "true" here is for three-palette battle sprites. Why do we assume
						// this is true? 
						await SetCustomPlayerSprites(s, true, preferences.MapmanSlot);
					}
				}

				//alternative resource name for two-palette battle sprites
				var spritesheet2 = resourcePackArchive.GetEntry("heroes2.png");
				if (spritesheet2 != null)
				{
					using (var s = spritesheet2.Open())
					{
						await SetCustomPlayerSprites(s,false, preferences.MapmanSlot);
					}
				}

			

				var fiends = resourcePackArchive.GetEntry("fiends.png");
				if (fiends != null)
				{
					using (var s = fiends.Open())
					{
						await SetCustomFiendGraphics(s);
					}
				}

				var chaos = resourcePackArchive.GetEntry("chaos.png");
				if (chaos != null)
				{
					using (var s = chaos.Open())
					{
						await SetCustomChaosGraphics(s);
					}
				}

				var backdrop = resourcePackArchive.GetEntry("battle_backdrops.png");
				if (backdrop != null)
				{
					using (var s = backdrop.Open())
					{
						await SetCustomBattleBackdrop(s);
					}
				}

				var weapons = resourcePackArchive.GetEntry("weapons.png");
				if (weapons != null)
				{
					using (var s = weapons.Open())
					{
						SetCustomWeaponGraphics(s);
					}
				}

				var gear = resourcePackArchive.GetEntry("gear_icons.png");
				if (gear != null)
				{
					using (var s = gear.Open())
					{
						SetCustomGearIcons(s);
					}
				}

				var enemysprites = resourcePackArchive.GetEntry("enemies.png");
				if (enemysprites != null)
				{
					using (var s = enemysprites.Open())
					{
						SetCustomEnemyGraphics(s);
					}
				}


				// dialogues need to go here because they depend on loaded data
				// from the ROM. We'll keep all the other text stuff here as well.
				var dialogue = resourcePackArchive.GetEntry("dialogue.txt");
				if (dialogue != null)
				{
					using (var s = dialogue.Open())
					{
						LoadDialogue(s, dialogues);
					}
				}

				var intro = resourcePackArchive.GetEntry("intro.txt");
				if (intro != null)
				{
					using (var s = intro.Open())
					{
						LoadIntro(s);
					}
				}

				var bridgeStory = resourcePackArchive.GetEntry("bridge.txt");
				if (bridgeStory != null)
				{
					using (var s = bridgeStory.Open())
					{
						
						LoadBridgeStory(s);
					}
				}

				var enemyNames = resourcePackArchive.GetEntry("enemies.txt");
				if (enemyNames != null)
				{
					using (var s = enemyNames.Open())
					{
						await LoadEnemyNames(s);
					}
				}

				var spellbook = resourcePackArchive.GetEntry("spellbook.json");
				if (spellbook != null)
				{
					using (var s = spellbook.Open())
					{
						using (StreamReader reader = new StreamReader(s))
						{
							var allSpells = JsonConvert.DeserializeObject<List<MagicSpell>>(reader.ReadToEnd());
							this.PutSpells(allSpells, enemyScripts);
						}
					}
				}
			}

		}

		public void LoadDialogue(Stream stream, DialogueData dialogues)
		{
			var dialogsdict = new Dictionary<int, string>();
			string speech = null;
			int dlg = 0;
			using (StreamReader reader = new StreamReader(stream))
			{
				while (true)
				{
					var line = reader.ReadLine();
					if (line == null)
					{
						break;
					}
					var sp = line.Split(" ");
					if (sp.Length == 2 && sp[1] == "----------------")
					{
						if (speech != null)
						{
							//Console.WriteLine($"{dlg} // '''{speech.Trim()}'''");
							dialogsdict[dlg] = speech.Trim();
						}
						speech = "";
						dlg = Convert.ToInt32(sp[0], 16);
						continue;
					}
					speech += "\n" + line;
				}
			}
			if (speech != null)
			{
				dialogsdict[dlg] = speech.Trim();
			}
			dialogues.InsertDialogues(dialogsdict);
		}

		public void LoadIntro(Stream stream)
		{
			var introText = new List<string>();

			using (StreamReader reader = new StreamReader(stream))
			{
				while (true)
				{
					var line = reader.ReadLine();
					if (line == null)
					{
						break;
					}
					introText.Add(line.TrimEnd());
				}
			}

			Blob intro = FF1Text.TextToStory(introText.ToArray());

			Console.WriteLine(intro.Length);
			System.Diagnostics.Debug.Assert(intro.Length <= 208);
			Put(0x37F20, intro);

		}
		public void LoadBridgeStory(Stream stream)
		{
			
			var pages = new List<string[]>();
			var bridgeText = new List<string>();

			using (StreamReader reader = new StreamReader(stream))
			{
				while (true)
				{
					var line = reader.ReadLine();
					if (line == null)
					{
						break;
					}
					line = line.TrimEnd();
					if (line.Length > 16)
					{
						line = line.Remove(16);
					}
					bridgeText.Add(line);
					if (bridgeText.Count == 8)
					{
						pages.Add(bridgeText.ToArray());
						bridgeText.Clear();
					}
				}

				if (bridgeText.Count > 0)
				{
					pages.Add(bridgeText.ToArray());
					bridgeText.Clear();
				}
			}

			SetBridgeStory(pages);
		}

		public async Task LoadEnemyNames(Stream stream)
		{
			var names = new List<string>();
			int enemycount = 0;

			using (StreamReader reader = new StreamReader(stream))
			{
				while (true)
				{
					var line = reader.ReadLine();
					if (line == null)
					{
						break;
					}
					line = line.TrimEnd();
					string[] tokens = line.Split(new char[0], StringSplitOptions.RemoveEmptyEntries);
					int textcount;
					if (!int.TryParse(tokens[0],out textcount))
					{
						await Progress($"WARNING: enemies.txt, line {enemycount} is missing an enemy number. Skipping import.");
						return;
					}
					if (textcount != enemycount)
					{
						await Progress($"WARNING: enemies.txt, line {enemycount} has the wrong enemy number. Skipping import.");
						return;
					}
					if (tokens.Length < 2)
					{
						await Progress($"WARNING: enemies.txt, line {enemycount} does not have an enemy name. Skipping Import.");
						return;
					}
					string name = String.Join(" ",tokens.Skip(1));

					names.Add(name);

					enemycount++;
				}

				if (names.Count != 128)
				{
					await Progress($"WARNING: enemies.txt needs 128 enemy names, but it had {names.Count} names. Skipping Import.");
					return;
				}
				EnemyText.Set(names.ToArray());

			}
		}


		// this is called independently in Randomize.cs
		void LoadResourcePackMaps(string resourcepack, StandardMaps maps, Teleporters teleporters)
		{
			if (resourcepack == null)
			{
				return;
			}
			using (var stream = new MemoryStream(Convert.FromBase64String(resourcepack)))
			{

				var resourcePackArchive = new ZipArchive(stream);

				for (int i = 0; i <= 60; i++)
				{
					var newdungeon = resourcePackArchive.GetEntry($"dungeonmap{i}.json");
					if (newdungeon != null)
					{
						using (var s = newdungeon.Open())
						{
							using (StreamReader reader = new StreamReader(s))
							{
								var newmaps = FF1Lib.Procgen.CompleteMap.LoadJson(reader);
								foreach (var newmap in newmaps)
								{
									maps.ImportCustomMap(teleporters, newmap);
								}
							}
						}
					}
				}
			}
		}
	}
}
