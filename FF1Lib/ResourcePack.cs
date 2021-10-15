using System.IO.Compression;
using System.Collections.Generic;
using RomUtilities;
using System.IO;
using System;

namespace FF1Lib
{
    public partial class FF1Rom : NesRom {
	void LoadResourcePack(Stream stream) {
	    var archive = new ZipArchive(stream);

	    var maptiles = archive.GetEntry("maptiles.png");
	    if (maptiles != null) {
		using (var s = maptiles.Open()) {
		    SetCustomOwGraphics(s);
		}
	    }

	    var spritesheet = archive.GetEntry("heroes.png");
	    if (spritesheet != null) {
		using (var s = spritesheet.Open()) {
		    SetCustomPlayerSprites(s, true);
		}
	    }

	    var fiends = archive.GetEntry("fiends.png");
	    if (fiends != null) {
		using (var s = fiends.Open()) {
		    SetCustomFiendGraphics(s);
		}
	    }

	    var chaos = archive.GetEntry("chaos.png");
	    if (chaos != null) {
		using (var s = chaos.Open()) {
		    SetCustomChaosGraphics(s);
		}
	    }

	    var backdrop = archive.GetEntry("battle_backdrops.png");
	    if (backdrop != null) {
		using (var s = backdrop.Open()) {
		    SetCustomBattleBackdrop(s);
		}
	    }

	    var dialogue = archive.GetEntry("dialogue.txt");
	    if (dialogue != null) {
		using (var s = dialogue.Open()) {
		    LoadDialogue(s);
		}
	    }

	    var overworld = archive.GetEntry("overworld.json");
	}

	public void LoadDialogue(Stream stream) {
	    var dialogsdict = new Dictionary<int, string>();
	    string speech = null;
	    int dlg = 0;
	    using (StreamReader reader = new StreamReader(stream)) {
		while (true) {
		    var line = reader.ReadLine();
		    if (line == null) {
			break;
		    }
		    var sp = line.Split(" ");
		    if (sp.Length == 2 && sp[1] == "----------------") {
			if (speech != null) {
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
	    if (speech != null) {
		dialogsdict[dlg] = speech.Trim();
	    }
	    InsertDialogs(dialogsdict);
	}
    }
}
