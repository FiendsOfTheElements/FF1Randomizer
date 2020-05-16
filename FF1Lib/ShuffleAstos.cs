using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Diagnostics;
using RomUtilities;

namespace FF1Lib
{

	public partial class FF1Rom : NesRom
	{
		public void ShuffleAstos(Flags flags, MT19337 rng)
		{
			const int NpcTalkOffset = 0x390D3;
			const int NpcTalkSize = 2;

			// NPC pool to swap Asto with
			// We can't do NPC that checks for flags and give items, because there's not enough space in Talk_Astos
			// So Lefein, ElfPrince and vanilla Sarda are out
			// However ElfDoc is fine and Early Sarda too
			List<ObjectId> npcpool = new List<ObjectId> { ObjectId.Astos, ObjectId.Bahamut, ObjectId.CanoeSage, ObjectId.CubeBot, ObjectId.ElfDoc,
			ObjectId.Fairy, ObjectId.King, ObjectId.Matoya, ObjectId.Nerrick, ObjectId.Princess2, ObjectId.Smith,
			ObjectId.Titan, ObjectId.Unne };

			var scriptStandardNPCItemTrade = Blob.FromHex("5693");
			var scriptTalk_Astos = Blob.FromHex("3893");

			// Add Sarda if the early flag is on
			if (flags.EarlySarda ?? false) npcpool.Add(ObjectId.Sarda);

			// Select random npc
			ObjectId newastos = npcpool.PickRandom(rng);

			// If Astos, we're done here
			if (newastos == ObjectId.Astos) return;

			// If not get NPC talk routine, get NPC object
			var talkscript = Get(NpcTalkOffset + (byte)newastos * NpcTalkSize, 2);
			var talkvalue = Get(MapObjOffset + (byte)newastos * MapObjSize, 4);

			// Switch astos to TalkScripts.StandardNPCItemTrade, set scriptvalue to crown Item.Crown;
			Put(NpcTalkOffset + (byte)ObjectId.Astos * NpcTalkSize, scriptStandardNPCItemTrade);
			Put(MapObjOffset + (byte)ObjectId.Astos * MapObjSize, Blob.FromHex("02")); // 0x02 = Crown

			// Swtich NPC to Astos
			Put(NpcTalkOffset + (byte)newastos * NpcTalkSize, scriptTalk_Astos);

			// Universal fix to Talk_Astos: ObjectID is this object, not Astos
			Put(0x39338 + 14, Blob.FromHex("A416"));

			// Change dialog a bit for non-item giving NPCs
			Put(0x285EF + 18, Blob.FromHex("00"));

			if (newastos == ObjectId.Titan)
			{
				// Check required item, don't try to give any item
				Put(0x39338, Blob.FromHex("AD2960F016EAEAEAEAEAEAEAEAEA"));

				// Change dialog
				Put(0x39338 + 24, Blob.FromHex("A912"));
			}
			else if (newastos == ObjectId.Bahamut)
			{   // Butcher Talk_Astos to make it work with Bahamut, and also modify DoClassChange

				// Change routine to check for Tail, give promotion and trigger the battle at the same time
				var newroutine =
					"AD2D60" +  // LDA item_tail - Load Tail
					"F016" +    // BEQ @Default 
					"A416" +    // LDY tmp+6 - Load this object instead of Astos
					"207F90" +  // JSR SetEventFlag (207F90)
					"207392" +  // JSR HideThisMapObject
					"A97D" +    // LDA #BTL_ASTOS
					"20C590" +  // JSR TalkBattle
					"20AE95" +  // JSR DoClassChange
					"A912" +    // LDA Astos dialog, A512 would be this npc dialog
					"EAEAEA" +  // NOPx3
					"60";       // RTS

				Put(0x39338, Blob.FromHex(newroutine));

				// DoClassChange reload the map to show the new class sprites, this break TalkBattle, so we stop it from reloading the map
				// INC dlgflg_reentermap (E656) => NOPx2 (EAEA)
				PutInBank(0x0E, 0x95AE + 19, Blob.FromHex("EAEA"));
			}
			else if (newastos == ObjectId.Unne)
			{
				// Change required item to Slab
				Put(0x39338, Blob.FromHex("AD2860"));

				// Change routine to teach lefeinish
				Put(0x39338 + 5, Blob.FromHex("EAEAEAEAA416207F90"));

				// Change dialog
				Put(0x39338 + 24, Blob.FromHex("A912"));
			}
			else if (newastos == ObjectId.ElfDoc)
			{
				// Change required item to Herb
				Put(0x39338, Blob.FromHex("AD2460"));

				// Change routine to wake up Prince
				Put(0x39338 + 5, Blob.FromHex("EAEAEAEAA416207F90"));

				// Change dialog
				Put(0x39338 + 24, Blob.FromHex("A912"));
			}
			else
			{
				if (talkscript == scriptStandardNPCItemTrade)
				{
					// Check required item
					Put(0x39338 + 1, Blob.FromSBytes(new sbyte[] { (sbyte)(talkvalue[0] + 32) }));
				}
				else
				{
					// Just skip item check, then we're set
					Put(0x39338, Blob.FromHex("EAEAEAEAEA"));
				}
			}
		}
	}
}
