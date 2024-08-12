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

		//async Task LoadResourcePack(Stream stream, DialogueData dialogues)
		async Task LoadResourcePack(string resourcepack, DialogueData dialogues, EnemyScripts enemyScripts, Preferences preferences)
		{
			if (resourcepack == null)
			{
				return;
			}
			using (var stream = new MemoryStream(Convert.FromBase64String(resourcepack)))
			{
				var resourcePackArchive = new ZipArchive(stream);

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

				var spritesheet = resourcePackArchive.GetEntry("heroes.png");
				if (spritesheet != null)
				{
					using (var s = spritesheet.Open())
					{
						await SetCustomPlayerSprites(s, true, preferences.MapmanSlot);
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
