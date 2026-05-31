using System;
using System.Collections.Generic;
using System.Text;

namespace FF1Lib
{
	public partial class FF1Rom
	{
		public void ApplyInconveniences(Flags flags, MT19337 rng, DialogueData dialogueData, MenuText menuText, ItemNames itemText)
		{
			if (flags.ObfuscateItemNames == true)
			{
				ObfuscateItemNames(flags, rng, dialogueData, menuText, itemText);
			}
		}
		public void ObfuscateItemNames(Flags flags, MT19337 rng, DialogueData dialogueData, MenuText menuText, ItemNames itemText)
		{
			List<FF1Text.MenuString> keyItemMenuStrings = [
				FF1Text.MenuString.UseLute,
				FF1Text.MenuString.UseCrown,
				FF1Text.MenuString.UseCrystal,
				FF1Text.MenuString.UseHerb,
				FF1Text.MenuString.UseKey,
				FF1Text.MenuString.UseTNT,
				FF1Text.MenuString.UseAdamant,
				FF1Text.MenuString.UseSlab,
				FF1Text.MenuString.UseRuby,
				FF1Text.MenuString.UseRod,
				FF1Text.MenuString.UseFloater,
				FF1Text.MenuString.UseChime,
				FF1Text.MenuString.UseTail,
				FF1Text.MenuString.UseCube,
				FF1Text.MenuString.UseOxyale];

			List<string> newDescriptions = [
				"Looks confusing",
				"Smells weird",
				"It looks pretty",
				"Has a funny smell",
				"It looks neglected",
				"Feels sticky",
				"Probably a paperweight",
				"Tastes salty",
				"Lighter than it looks",
				"Looks neglected",
				"Needs a good cleaning",
				"Not sure what this does",
				"Seems cheaply made",
				"It doesn't seem to have an\nON switch",
				"Belongs in a museum",
				"Definitely not pants",
				"Heavier than it looks",
				"Gives you deja-vu",
				"An item you possess",
				"Yet another item",
				"Definitely a key item",
				"An item you found",
				"You now prossess the item",
				"It feels groovy",
				"It looks funky",
				"It's emanating a strange\naura",
				"For doing the thing",
				"An item you have",
				"It could be useful",
				"It could be important",
				"Probably used for something",
				"An item of some kind",
				"It looks like an item",
				"Wow, a rare item!",
				"It's perfectly normal",
				"It looks like junk",
				"That item ain't right",
				"You swear you've seen\nthis before",
				"An item of unknown use",
				"new item who dis?",
				"Did Brenda from HR regift\nthis?",
				"It's probably not food?",
				"A series of 1's and 0's",
				"This recursive item is\nrecursive",
				"Press A to select",
				"An item that is an item",
				"Stamped on the bottom...\nMADE IN JAPAN",
				"A mysterious item"
			];

			newDescriptions.Shuffle(rng);
			for (int i = 0; i < keyItemMenuStrings.Count; i++)
			{
				menuText.MenuStrings[(int)keyItemMenuStrings[i]] = FF1Text.TextToBytes(newDescriptions[i], useDTE: true);
			}

			List<Item> keyItems = new List<Item>() {
				Item.Lute, Item.Crown, Item.Crystal, Item.Herb, Item.Key, Item.Tnt, Item.Adamant,
				Item.Slab, Item.Ruby, Item.Rod, Item.Floater, Item.Chime, Item.Tail, Item.Cube,
				Item.Bottle, Item.Oxyale };

			foreach (Item item in keyItems)
			{
				itemText[(int)item] = "ITEM?";
			}

			Dictionary<int, string> dialogsUpdate = new Dictionary<int, string>();
			List<string> keyItemNames = new List<string>() {
				"LUTE", "CROWN", "CRYSTAL", "HERB", "KEY", "TNT", "ADAMANT", "SLAB", "RUBY",
				"ROD", "FLOATER", "CHIME", "TAIL", "CUBE", "BOTTLE", "OXYALE" };
			foreach (string item in keyItemNames)
			{
				dialogsUpdate = SubstituteKeyItemInExtraNPCDialogues(item, "ITEM", dialogueData);
			}
			dialogueData.InsertDialogues(dialogsUpdate);
		}
	}
}
