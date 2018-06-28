# FF1 Randomizer

**The online version of the randomizer is now live!**  Check it out at http://finalfantasyrandomizer.com!  The Windows and command-line versions will still continue to be updated, in case your internet connection (or the server's!) is spotty.

This is a randomizer for Final Fantasy 1 for the NES.  It currently operates on the US version of the ROM only.

## How it works

The randomizer has many options, each with unique gameplay implications.  An explanation of what each option does, and some of the possible consequences is listed in each section.

### Shuffle Tab
This tab contains options for shuffling various bits of game data.  Shuffling means that all the same data will be in the ROM, but in a randomized order.

#### Treasures & NPC Items
This option randomly shuffles the contents of all treasure chests and/or NPC item gifts in the game.  There are several implications to this:
* Important quest items, like the SLAB, FLOATER, SHIP etc. can be relocated.  The randomizer will do a sanity check to ensure that it is possible to complete the game.
* Some quest items are not actually useful until certain game events have been completed.  For example, if you find the RUBY early, you may think you can go straight to Sarda and get the ROD.  Sarda, however, will not grant you the ROD until you have killed the VAMPIRE (who is usually guarding the chest containing the RUBY).  Similarly, the sages will not grant you the CANOE until you have defeated LICH, making an early FLOATER all but useless since you can't reach Ryukahn Desert.  Enable the Early Rod or Early Canoe options to fix these issues.
* Since treasures in the original game are roughly sorted in increasing order of "goodness", shuffling them is likely to grant better equipment to the player early in the game.  This can help or hurt the player, depending on whether the rewards are usable by the early classes.
* Speaking of classes, the TAIL can be anywhere except the Temple of Fiends Revisited.  You may find it and the FLOATER early on, and be able to class change very early, or you may have to play almost the entire game without class change.
* The TAIL and ADAMANT will never be in the Temple of Fiends Revisited.  Although these items are not necessary for game completion, they are deemed important enough to be accessible before the final dungeon.  In particular, the WARP and EXIT spells are not obtainable without the TAIL (if you don't shuffle magic permissions), and you can't leave ToFR without them.
* The SHIP and BRIDGE cannot be directly in key-locked chests. The SHIP cannot be directly placed in Ice Cave even if you get an early CANOE making Ice Cave accessible, however, it is possible (<1%) for Matoya to have the SHIP and her CRYSTAL to be in the Ice Cave. If you incentivize Ice Cave please be aware that this increases the chance of that happening to about ~3%.
* It is possible to walk across the CANAL before it is blown up, which is particularly relevant if you have an early CANOE. The randomizer will automatically extend the nearby mountains to prevent you from walking across if a softlock is possible. Some additional map edit options on the Map tab will further reduce the chances of the softlock (reducing the chance of extended mountains).
* NPC Item Shuffle drastically alters the items that are required to beat the game. Fetch quest items, CROWN, CRYSTAL, HERB, TNT, BOTTLE, and SLAB are not necessarily required, and it is possible that ADAMANT is a required item if the Dwarf Smith gives a required item.
* NPC Item Shuffle does not change Unne's role in translating the SLAB (aka. the longest fetch quest ever). Likewise, the Elf Doctor wakes the Prince in exchange for the HERB, and the Dragon still gives Class Change for the TAIL.

#### Shops
This option randomly shuffles the contents of each type of equipment and item shop in the game.
* Weapons will still be in weapon shops, armor in armor shops, etc.  This is necessary because weapons, armor, and items are dealt with in separate inventory screens, so when you buy something from a weapon shop, it goes in one of your weapon slots.
* Shops can contain different numbers of items than before.  Every shop will contain at least one item, and no shop will sell the same item twice.
* The caravan is just an item shop, so the BOTTLE can end up in any of the item shops.  And of course the caravan may end up selling CABINs, HEALs, etc.

#### Magic Shops
This option randomly shuffles the contents of the magic shops.
* Just like equipment shops, white magic shops will always contain white magic, and black magic shops will always contain black magic.  Towns always have an equal number of white/black magic shops anyway, so mixing them wouldn't change much.
* Also like equipment shops, magic shops may sell any number of spells, from at least one up to five.
* Early shops may end up selling high-level spells.  These spells will still go to the spell slots of their respective level, so a level 3 spell is a level 3 spell no matter where you buy it.

#### Magic Levels
This option rearranges the order of spells in the ROM, changing their levels.
* There will always be 4 spells of each level.  The level of a spell is hard-coded to its position in the spell table.
* Yes, NUKE can become a level 1 spell.  Yes, you will have lots of spell charges for it.
* Conversely, things like CURE could become level 8 spells and nearly impossible to use.
* You can enable this option with or without shuffling magic shops.  Shuffling levels but not shops is a fairly compelling way to play, since you'll have spell charges for the early spells and be able to buy them early, but you don't necessarily know what the spells are.
* By default, permissions for each spell are retained, even if the spell becomes an earlier level.  Red Wizard can't use NUKE even if it's level 1.  If "Keep Permissions" is unchecked, then spells will end up with whatever permissions are in the slot they get moved to.

#### RNG Table
This option randomizes the table of numbers that the game uses to generate pseudo-random values.  Shuffling these values means you will get enemy encounters in a different order.  This prevents using known routes to avoid certain encounters.

#### Enemy Scripts, Skills, and Spells
These options change enemy behavior during battle.
* If scripts is checked, and skills/spells is unchecked, then the existing scripts will be reassigned to random enemies.  So an IMP may behave exactly like a MEDUSA, for example.
* If skills/spells is checked and scripts is unchecked, then the skills and spells will be shuffled around to different scripts.  Enemies that normally use skills and/or spells will use different ones.
* The scripts, skills, and spells are shuffled in three distinct groups, to avoid IMPs casting NUKE or using NUCLEAR.  The three groups are regular enemies, fiends, and a group consisting of WarMECH, fiends revisited, and CHAOS.  It is of course still possible for early enemies to be significantly overpowered.

#### Enemy Status Attacks
This option assigns enemy status attacks applied on hit (poison, dark, stun, sleep, stone, death) to different enemies.
* There will always be the same number of enemies that have a given status attack.
* Status attacks may end up on enemies with more or fewer hits, making them more or less problematic.
* This option is likely to "spread out" the status attacks, making certain groups of enemies less irritating (e.g. large groups of undead probably won't stunlock you anymore).  Of course, it could also give IMPs death touch, so beware.
* If shops are also shuffled, the randomizer will ensure that PURE and SOFT potions are available in Coneria's item shop, since there would otherwise be no remedy for these ailments in the early game.

### Incentives Tab
These options force an important item to be in their respective locations.
* The CROWN chest in Marsh Cave will contain the incentivized item.
* The TNT chest in Coneria will contain the incentivized item.
* The RUBY chest in Earth Cave will contain the incentivized item.
* The EYE chest in the Ice Cave will contain the incentivized item.
* The Zombie D chest in Castle Ordeals will contain the incentivized item.
* The Key-Locked chest in Sea Shrine will contain the incentivized item.
* The Red D chest in Volcano will contain the incentivized item.
* The possible items that can appear in the incentivized chests are also adjustable on the Incentives tab.
* The Junk Item option will add a junk item (Cloth or similar) to the incentivized item pool.
* If Treasures or NPC items are turned off on the Shuffle tab, the Incentives options for corresponding items or locations will have no impact.

### Map Tab

#### Castle Ordeals
This option randomizes the teleporters in Castle Ordeals.
* The game arranges all the rooms in a random order, then picks one teleporter from each room to go to the next room.
  - An extra room with a single teleporter has been added to the route.  The vanilla game skips this area.
  - Three of the rooms have two teleporters.  The second teleporter in each of these rooms goes back to a previous room, or to the same room.
  - One of the rooms has four teleporters, and no two of these will go to the same place.  Two of them will go backwards (or to the room you're currently in), one will go to the next room, and the fourth will go to a purely random room (possibly even skipping far ahead).
* **CAUTION:** The original game has a bug that can cause a softlock if you enter too many teleporters or take too many staircases.  Enabling this option might cause you to get lost in the castle and end up reaching this limit.
  - The softlock occurs somewhere around 30 teleports, and only happens when you enter the menu.
  - It is strongly recommended that you save your game with a TENT, CABIN, or HOUSE before entering the castle.
  - The WARP spell is very useful if you get sent too far back and are worried about softlocking.

#### Titan's Trove
The Titan has moved down a few tiles so that he now blocks access to the 4 treasure chests in his tunnel. This increases the chance that RUBY is required.

#### Coneria to Dwarves
A pathway has been opened to walk to Dwarf Cave from the starting area. In NPC item shuffle where the CANOE can be shuffled to very early locations this allows for some substantially different routing in the early game. With this flag, it is possible that SHIP is not required to beat the game.

#### Volcano and Ice River
The rivers giving access to Volcano and Ice Cave have been combined. In NPC item shuffle where the CANOE can be shuffled to very early locations this allows for some substantially different routing in the early game. Combined with "Coneria to Dwarves", it means that the CANOE gives access to all early game locations normally accessible with the SHIP prior to CANAL, plus Volcano and Crescent Lake. With this flag, it is possible that SHIP is not required to beat the game.

### Scale Tab
This tab contains sliders that will multiply bits of game data by random values within a range determined by the slider.  The distribution of random multipliers is exponential.

#### Easy Mode
Sometimes seeds with certain flags take a long time to complete. This option should make things a bit quicker by reducing enemy HP to 10% (after considering the randomized scaled HP), and by reducing the overall frequency of random encounters.

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
This option reduces the amount of experience required to gain levels, and lowers all prices.
* Previous versions of the randomizer (prior to 1.4.8) increased the experience and gold rewards given by enemies.  This was found to overflow in late game battles, so the approach was changed.
* The top slider changes the experience and gold multiplier.  Level requirements, prices, and your initial gold amount will be divided by the multiplier, so a 3x multiplier means you will gain levels 3x faster, and items will be 3x cheaper.  Your starting gold will also be 3x less to account for cheaper prices.
* The bottom slider adds a fixed amount of experience and gold reward to enemies.  This bonus is divided by the multiplier, so 1x + 500 would add 500 exp/gold to each enemy, whereas 5x + 500 would add 100 exp/gold to each enemy (but requirements and prices will be 5x lower, so it's essentially the same).  This is useful to speed up the early game, which can require a lot of grinding, without overpowering the party too much in the late game.
* There is no randomization for this option, requirements, prices, and rewards are scaled by exactly the amount indicated.  There is of course a separate option to randomize prices.

### Conveniences Tab
This tab contains options to make the game play a bit faster.
* Early Rod allows you to obtain the ROD from Sarda before killing the VAMPIRE in the Earth Cave.  Without this option, obtaining the RUBY early doesn't help you much (other than giving you access to the treasures in the Titan's Tunnel).
* Early Canoe allows you to obtain the CANOE from Crescent Lake before killing LICH.  Without this option, obtaining the FLOATER early doesn't grant you use of the AIRSHIP until LICH is defeated, since you can't get to the Ryukahn Desert without the CANOE.
* Early Ordeals allows you to enter Castle Ordeals without the CROWN.
* No Party Shuffle prevents the game from reordering your party members when one of them is poisoned, petrified, or slain during battle.
* Speed Hacks enables lots of things that speed up the gameplay, particularly battles and screen transitions.  This option also moves some NPCs out of your way, such as the bats in Earth Cave.
* Identify Treasures lets you see what's in a treasure chest when your inventory is full.
* Enable Dash allows you to walk at double speed by holding the B button.  Also, if you hold B while flying the airship, you will move slower, which can help you land accurately.
* Buy 10 Items lets you buy 10 items at once from item shops, so you can stock up on 99 heal potions more quickly.
* Modern battlefield shows a battle UI similar to later games in the FF series.

### Bug Fixes Tab
This tab contains options to fix some of the game's many bugs.
* House MP Restoration makes the HOUSE restore your MP **before** saving the game, so that your MP is restored if you load the game.
* Weapon Stats fixes the weapons so that they get the correct bonuses.
  - Some weapons get a bonus against monster types (e.g. Were Sword)
  - Some get a bonus against monsters weak to certain elements (e.g. Ice Sword)
  - The critical hit rates of all weapons now use the originally intended values.  Because this is a significant nerf to late-game weapons, the critical rates for all weapons are doubled, and the Black Belt/Master's critical rate is halved.
  - The bonus for the right element or monster type is +10 damage and +40 hit%.  The bonus is only applied once regardless of how many elements or monster types match.
* Chance to Run fixes the run logic to be based purely on your character's level and luck stats (and not having anything to do with the status of other party members).  This is a significant boost to Thief, especially in the early game.
* Various spells now work as originally intended.
  - LOCK and LOK2 use the correct spell effect.
  - HEL2 no longer functions like HEL3 in battle.
  - TMPR and SABR both work, which is very important against late game bosses with high absorb, especially with large scale values for enemy stats.
* Enemy status attacks now only get a chance to work for attacks that hit the player.

### Fun % Tab
This tab contains options that are not included in the Flags String. For competitive play, these options can safely be selected per the player's individual preference. Similarly, but not shown on this tab, a new feature is available to change the menu's background color by pressing `select` while on the start menu in game.
