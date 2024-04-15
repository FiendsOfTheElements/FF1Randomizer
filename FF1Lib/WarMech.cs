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
	public class WarMechMode
	{
		private const byte UnusedTextPointer = 0xF7;
		private const byte WarMECHEncounter = 0x56;
		private const byte RobotGfx = 0x15;

		public static void Process(FF1Rom rom, Flags flags, NPCdata npcdata, ZoneFormations zoneformations, MT19337 rng, StandardMaps maps, MapIndex warmechDDmap)
		{


		}
		private static void WarMechPatrol(FF1Rom rom, Flags flags, NpcObjectData npcdata, ZoneFormations zoneformations, MT19337 rng, StandardMaps maps, MapIndex warmechDDmap)
		{
			npcdata[ObjectId.WarMECH].Dialogue2 = UnusedTextPointer;
			npcdata[ObjectId.WarMECH].Battle = WarMECHEncounter;
			npcdata[ObjectId.WarMECH].Sprite = ObjectSprites.Robot;



			npcpdata.GetTalkArray(ObjectId.WarMECH)[(int)TalkArrayPos.dialogue_2] = UnusedTextPointer;
			npcpdata.GetTalkArray(ObjectId.WarMECH)[(int)TalkArrayPos.battle_id] = WarMECHEncounter;

		}
		private static void CreateWarMechNpc(NpcObjectData npcdata)
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
		}
	}

	public partial class FF1Rom
	{

		private void WarMechMode(Flags flags)
		{
			private const byte UnusedTextPointer = 0xF7;
			private const byte WarMECHEncounter = 0x56;
			private const byte RobotGfx = 0x15;



		}

		public void WarMECHNpc(WarMECHMode mode, NPCdata npcpdata, ZoneFormations zoneformations, MT19337 rng, StandardMaps maps, bool deepDungeon, MapIndex warmechDDmap)
		{
			const byte UnusedTextPointer = 0xF7;
			const byte WarMECHEncounter = 0x56;
			const byte RobotGfx = 0x15;

			// Set up the map object.
			npcpdata.GetTalkArray(ObjectId.WarMECH)[(int)TalkArrayPos.dialogue_2] = UnusedTextPointer;
			npcpdata.GetTalkArray(ObjectId.WarMECH)[(int)TalkArrayPos.battle_id] = WarMECHEncounter;
			Data[MapObjGfxOffset + (byte)ObjectId.WarMECH] = RobotGfx;

			// Set the action when you talk to WarMECH.
			npcpdata.SetRoutine(ObjectId.WarMECH, newTalkRoutines.Talk_fight);

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

			InsertDialogs(UnusedTextPointer, dialogueStrings.PickRandom(rng));

			// Get rid of random WarMECH encounters.  Group 8 is now also group 7.
			var formation = zoneformations[64 + (byte)MapIndex.SkyPalace5F];
			formation.Formations[6] = formation.Formations[7];

			if (mode != WarMECHMode.Unleashed && mode != WarMECHMode.All)
				MakeWarMECHUnrunnable();

			if ((mode == WarMECHMode.Required || mode == WarMECHMode.All) && !deepDungeon)
			{
				// Can't use mapNpcIndex 0, that's the Wind ORB.
				SetNpc(MapIndex.SkyPalace5F, 1, ObjectId.WarMECH, 0x07, 0x0E, inRoom: false, stationary: true);

				Data[0x029AB] = 0x14; // we can only change one color without messing up the Wind ORB.
			}
			if (mode == WarMECHMode.Patrolling || mode == WarMECHMode.All)
			{
				byte warmechMap = (byte)MapIndex.SkyPalace4F;
			maps[]



				if (!deepDungeon)
				{
					var (x, y) = GetSkyCastleFloorTile(rng, maps[warmechMap]);
					SetNpc((MapIndex)warmechMap, 0, ObjectId.WarMECH, x, y, inRoom: false, stationary: false);
				}
				else
				{
					warmechMap = (byte)warmechDDmap;
				}

				// We can change all the colors here.
				PutInBank(0x00, 0xA000 + (warmechMap * 0x30) + 0x18, Blob.FromHex("0F0F18140F0F1714"));
			}
		}
	}
}
