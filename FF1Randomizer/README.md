# FF1 Randomizer

This is a randomizer for Final Fantasy 1 for the NES.  It currently operates on the US version of the ROM only.

## How it works

The randomizer has several options, each with unique gameplay implications.  An explanation of what each option does, and some of the possible consequences is listed in each section.

### Shuffle Tab
This tab contains options for shuffling various bits of game data.  Shuffling means that all the same data will be in the ROM, but in a randomized order.

#### Shuffle Treasures  
This option randomly shuffles the contents of all treasure chests in the game.  There are several implications to this:
* Important quest items, like the CROWN, FLOATER, etc. can be relocated.  The randomizer will do a sanity check to ensure that it is possible to complete the game.
* Some quest items are not actually useful until certain game events have been completed.  For example, if you find the RUBY early, you may think you can go straight to Sarda and get the ROD.  Sarda, however, will not grant you the ROD until you have killed the VAMPIRE (who is usually guarding the chest containing the RUBY).  Similarly, the sages will not grant you the CANOE until you have defeated LICH, making an early FLOATER all but useless since you can't reach Ryukahn Desert.  Enable the Early Rod or Early Canoe options to fix these issues.
* Since treasures in the original game are roughly sorted in increasing order of "goodness", shuffling them is likely to grant better equipment to the player early in the game.  This can help or hurt the player, depending on whether the rewards are usable by the early classes.
* Speaking of classes, the TAIL can be anywhere except the Temple of Fiends Revisited.  You may find it and the FLOATER early on, and be able to class change very early, or you may have to play almost the entire game without class change.
* The TAIL and ADAMANT will never be in the Temple of Fiends Revisited.  Although these items are not necessary for game completion, they are deemed important enough to be accessible before the final dungeon.  In particular, the WARP and EXIT spells are not obtainable without the TAIL (if you don't shuffle magic permissions), and you can't leave ToFR without them.

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
* By default, permissions for each spell are retained, even if the spell becomes an earlier level.  Red Wizard can't use NUKE even if it's level 1.  If "Keep Permissions" is unchecked, then spells will end up with whatever permissions are in the slot they get moved to.

#### Shuffle Enemy Scripts, Skills, and Spells
These options change enemy behavior during battle.
* If scripts is checked, and skills/spells is unchecked, then the existing scripts will be reassigned to random enemies.  So an IMP may behave exactly like a MEDUSA, for example.
* If skills/spells is checked and scripts is unchecked, then the skills and spells will be shuffled around to different scripts.  Enemies that normally use skills and/or spells will use different ones.
* The scripts, skills, and spells are shuffled in three distinct groups, to avoid IMPs casting NUKE or using NUCLEAR.  The three groups are regular enemies, fiends, and a group consisting of WarMECH, fiends revisited, and CHAOS.  It is of course still possible for early enemies to be significantly overpowered.

### Scale Tab
This tab contains sliders that will multiply bits of game data by random values within a range determined by the slider.  The distribution of random multipliers is exponential.

#### Prices
This slider randomly adjusts prices of equipment, items, and spells higher and lower.
* Prices cannot go higher than 65535, so on balance, this will probably save you money on the most expensive gear.
* This option also adjusts the amount of gold found in treasure chests, since these are considered "items" by the game.

#### Enemy Stats
This slider randomly adjusts enemy stats higher and lower.
* Not all stats are adjusted equally, because some stats massively unbalance the game if adjusted too much.
  - Accuracy and Evade are scaled normally
  - Defense, critical hit rate, and number of hits use half the scale factor
  - Strength and morale use 1/4 of the scale factor
* "Half" the scale factor means half the distance to 100%.  So if the scale factor is 500%, "half" is 300%, and "1/4" is 200%.  The same goes for lower values; "half" of a 20% scale factor would be 60%.

#### Exp/Gold Boost
This option increases the amount of experience and gold given by enemies to speed up the game.
* There is no randomization for this option, all exp and gold rewards are increased by exactly the amount indicated.
* The bonus slider adds a fixed amount of exp/gold to every enemy.  This is useful to speed up the early game, which is especially grindy, without overpowering the party too much in the late game.
* Rewards are capped at 32767, the most the game can handle.

### Conveniences Tab
This tab contains options to make the game play a bit faster.
* Early Rod allows you to obtain the ROD from Sarda before killing the VAMPIRE in the Earth Cave.  Without this option, obtaining the RUBY early doesn't help you much (other than giving you access to the treasures in the Titan's Tunnel).
* Early Canoe allows you to obtain the CANOE from Crescent Lake before killing LICH.  Without this option, obtaining the FLOATER early doesn't grant you use of the AIRSHIP until LICH is defeated, since you can't get to the Ryukahn Desert without the CANOE.
* No Party Shuffle prevents the game from reordering your party members when one of them is poisoned, petrified, or killed during battle.
