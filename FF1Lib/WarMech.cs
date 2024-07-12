using RomUtilities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static FF1Lib.FF1Rom;

namespace FF1Lib
{
	public enum WarMECHMode
	{
		Vanilla,
		Patrolling,
		Required,
		Unleashed,
		All,
		Random
	}
	public static class WarMechMode
	{
		private const byte UnusedTextPointer = 0xF7;
		private const byte WarMECHEncounter = 0x56;
		private const byte RobotGfx = 0x15;

		public static void Process(FF1Rom rom, Flags flags, NpcObjectData npcdata, ZoneFormations zoneformations, DialogueData dialogues, MT19337 rng, StandardMaps maps, MapIndex warmechDDmap)
		{
			WarMECHMode mode = flags.WarMECHMode;

			// Weighted; Vanilla, Unleashed, and All are less likely
			if (mode == WarMECHMode.Random)
			{
				int RandWarMECHMode = rng.Between(1, 100);

				if (RandWarMECHMode <= 15)
					mode = WarMECHMode.Vanilla;    // 15%
				else if (RandWarMECHMode <= 45)
					mode = WarMECHMode.Patrolling; // 30%
				else if (RandWarMECHMode <= 75)
					mode = WarMECHMode.Required;   // 30%
				else if (RandWarMECHMode <= 90)
					mode = WarMECHMode.Unleashed;  // 15%
				else
					mode = WarMECHMode.All;        // 10%
			}

			if (mode == WarMECHMode.Vanilla)
			{
				return;
			}

			zoneformations.UnleashWarMECH(flags.WarMECHMode == WarMECHMode.Unleashed || flags.WarMECHMode == WarMECHMode.All);

			if (mode == WarMECHMode.Required || mode == WarMECHMode.Patrolling || mode == WarMECHMode.All)
			{
				CreateWarMechNpc(npcdata, dialogues, rng);

				// Get rid of random WarMECH encounters.  Group 8 is now also group 7.
				var formation = zoneformations[64 + (byte)MapIndex.SkyPalace5F];
				formation.Formations[6] = formation.Formations[7];
			}

			if (mode != WarMECHMode.Unleashed && mode != WarMECHMode.All)
			{
				rom.MakeWarMECHUnrunnable();
			}

			if ((mode == WarMECHMode.Required || mode == WarMECHMode.All) && flags.GameMode != GameModes.DeepDungeon)
			{
				// Can't use mapNpcIndex 0, that's the Wind ORB.
				maps[MapIndex.SkyPalace5F].MapObjects.SetNpc(1, ObjectId.WarMECH, 0x07, 0x0E, false, true);
				rom[0x029AB] = 0x14; // we can only change one color without messing up the Wind ORB.
			}

			if (mode == WarMECHMode.Patrolling || mode == WarMECHMode.All)
			{
				MapIndex warmechMap = (flags.GameMode == GameModes.DeepDungeon) ? warmechDDmap : MapIndex.SkyPalace4F;

				var tile = maps[warmechMap].Map.GetRandomElement(rng, 0x4B);
				maps[warmechMap].MapObjects.SetNpc(0, ObjectId.WarMECH, tile.X, tile.Y, false, false);

				// We can change all the colors here.
				rom.PutInBank(0x00, 0xA000 + ((int)warmechMap * 0x30) + 0x18, Blob.FromHex("0F0F18140F0F1714"));
			}
		}
		private static void CreateWarMechNpc(NpcObjectData npcdata, DialogueData dialogues, MT19337 rng)
		{
			// Update Object
			npcdata[ObjectId.WarMECH].Dialogue2 = UnusedTextPointer;
			npcdata[ObjectId.WarMECH].Battle = WarMECHEncounter;
			npcdata[ObjectId.WarMECH].Sprite = ObjectSprites.Robot;

			// Set the action when you talk to WarMECH.
			npcdata[ObjectId.WarMECH].Script = TalkScripts.Talk_fight;

			// Change the dialogue.
			var dialogueStrings = new List<string>
			{
				"I. aM. WarMECH.",
				"Give me the AllSpark,\nand you may live to be\nmy pet!",
				"I'm afraid that's\nsomething I cannot allow\nto happen, Dave.",
				"Bite my shiny metal ass!",
				"Put down your weapons.\nYou have 15 seconds to\ncomply.",
				"rEsIsTaNcE iS fUtIlE.",
				"Hasta la vista, baby.",
				"NoOo DiSaSsEmBlE!",
				"Bring back life form.\nPriority one.\nAll other priorities\nrescinded."
			};

			dialogues[UnusedTextPointer] = dialogueStrings.PickRandom(rng);
		}
	}
}
