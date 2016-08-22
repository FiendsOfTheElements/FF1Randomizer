# FF1 Randomizer

This is a randomizer for Final Fantasy 1 for the NES.  It currently operates on the US version of the ROM only.

## How it works

The randomizer has several options, each with unique gameplay implications.  An explanation of what each option does, and some of the possible consequences is listed in each section.

#### Shuffle Treasures  
This option randomly shuffles the contents of all treasure chests in the game.  There are several implications to this:
* Important quest items, like the CROWN, FLOATER, etc. can be relocated.  The randomizer will do a sanity check to ensure that it is possible to complete the game.
* Some quest items are not actually useful until certain game events have been completed.  For example, if you find the RUBY early, you may think you can go straight to Sarda and get the ROD.  Sarda, however, will not grant you the ROD until you have killed the VAMPIRE (who is usually guarding the chest containing the RUBY).  Similarly, the sages will not grant you the CANOE until you have defeated LICH, making an early FLOATER all but useless since you can't reach Ryukahn Desert.
* Since treasures in the original game are roughly sorted in increasing order of "goodness", shuffling them is likely to grant better equipment to the player early in the game.  This can help or hurt the player, depending on whether the rewards are usable by the early classes.
* Speaking of classes, the TAIL can be anywhere -- including the Temple of Fiends Revisited.  You may find it and the FLOATER early on, and be able to class change as soon as LICH is defeated, or you may have to play almost the entire game without class change.

#### Shuffle Equipment Shops
This option randomly shuffles the contents of each type of shop in the game.
* Weapons will still be in weapon shops, armor in armor shops, etc.  This is necessary because weapons, armor, and items are dealt with in separate inventory screens, so when you buy something from a weapon shop, it goes in one of your weapon slots.
* Shops can contain different numbers of items than before.  Every shop will contain at least one item, and no shop will sell the same item twice.
* The caravan is just an item shop, so the BOTTLE can end up in any of the item shops.  And of course the caravan may end up selling CABINs, HEALs, etc.

#### Shuffle Magic Shops
This option randomly shuffles the contents of the magic shops.
* Just like equipment shops, white magic shops will always contain white magic, and black magic shops will always contain black magic.  Towns always have an equal number of white/black magic shops anyway, so mixing them wouldn't change much.
* Also like equipment shops, magic shops may sell any number of spells, from at least one up to five.
* Early shops may end up selling high-level spells.  These spells will still go to the spell slots of their respective level, so a level 3 spell is a level 3 spell no matter where you buy it.

#### Shuffle Magic Levels
This option rearranges the order of spells in the ROM, changing their levels.
* There will always be 4 spells of each level.  The level of a spell is hard-coded to its position in the spell table.
* Yes, NUKE can become a level 1 spell.  Yes, you will have lots of spell charges for it.
* Conversely, things like CURE could become level 8 spells and nearly impossible to use.
* You can enable this option with or without shuffling magic shops.  Shuffling levels but not shops is a fairly compelling way to play, since you'll have spell charges for the early spells and be able to buy them early, but you don't necessarily know what the spells are.
* Permissions for each spell are retained, even if the spell becomes an earlier level.  Red Wizard can't use NUKE even if it's level 1.  Shuffling magic permissions may become a future feature.

#### Exp/Gold Boost
This option increases the amount of experience and gold given by enemies to speed up the game.
* The bonus slider adds a fixed amount of exp/gold to every enemy.  This is useful to speed up the early game, which is especially grindy, without overpowering the party too much in the late game.
* The multiplier slider multiplies enemies' exp/gold by the amount set.
* Rewards are capped at 32767, the most the game can handle.
