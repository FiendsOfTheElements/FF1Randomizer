; Bank Usage - Last Update 2024-12-23
; Whenever a feature adds code to the rom, this table must be updated.
; Doesn't include the new features placed at the freed space at 0B and 0E. This is already a mess and no feature should be written there if possible.
; NOTE This list was done retroactively, so some ranges might be missing.
;
; ~ = No upper range is enforced in the code, data could potentially overlap other used ranges.
; ! = Potential bug, to correct


Bank Offset Range Description
-----------------------------
; Freed  (write only after SM have been read)
04   8000-FFFF    Unused
05   8000-FFFF    Unused
06   8000-FFFF    Unused

; Freed  (write only after data has been moved to 1F)
0F   8000-8B35    Stats Tracking code
0F   9000-93CD    Stats Tracking code
0F   8AD0-8AE3    Castable Items targeting
0F   8B40-8C06    Nones code
0F   8D00-8D53    Autosort
0F   9100-91AC    Progressive Scaling
0F   B000-B300    Expanded Teleporter tables

10   8000-FFFF    Dialogues (whole bank is reserved)

11   8000-81A0    NPC Objects talk table
11  ~8200-85C6    [NO MAX SET] Talk Routines
11   8EA0-9000    Monster In A Box
11   902B-9915    Moving NPC talk routines
11   95C8-95D4    Random promotions [modify DoClassChanges, which was moved with NPC Talk Routines]
11   99A0-9CE3    Shop Upgrade Text routines
11   9DF0-9DFC    Promotion table
11   9F10-9F19    GiveItem talk routine
11  !A000-ADA7    Shop Upgrade text [Max Set at $B400, potential overlaps]
11   B120-B1AF    Extra talk routines
11   B400-B519    Treasure Stacks
11   B600-B700    Incentive Chests Item Fanfare LUT
11   B700-B831    Shop Upgrade routines
11   BA00-BEE0    NPC Objects data

12   8000-8800    Tileset copy
12   8A00-8C40    New Icons
12   8E00-9000    Font Tileset
12   9000-9038    New Icons routines

14   8000-FFFF    Standard Maps (moved from 04, 05, 06 for extra space)
15   8000-FFFF    Standard Maps
16   8000-FFFF    Standard Maps
17   8000-FFFF    Standard Maps

19   8000-8F90    QuickMiniMap map

1A   84FB-84FE    Repeated Heal Potions
1A   8600-8852    Repeated Heal Potions

1B   8000-8FF5    Moving routines from bank 0B to 1B (Xp, levelup)
1B   8FF5-90E8    Save on Death
1B   9100-91AC    Random Promotion
1B   9300-9369    Thief Lockpicking
1B   9400-9470    LoadPlayerIBStats moved
1B   9500-9535    AltClass XP
1B   9535-9814    Class Level Requirement tables
1B   9830-987B    Gain MP on MP Up
1B   9900-9A11    SetRng
1B   A000-A0DB    AirBoat
1B   AF80-AF94    BB Absorb bugfix
1B   B800-B8C3    Start Game/Start Battle Blursings routines

1C   A010-A165    ExtConsumables Drink box
1C   A1C0-A1EF    AutoRetargeting
1C   A200-A246    White Mage Harm Everyone
1C   A250-A275    AutoRetargeting
1C   A290-A4B7    Int Affect Spells
1C   A4F0-A50E    Stats Tracking
1C   A600-A648    Spell Reordering
1C   A670-A724    Poison Mode routines

1D  ~8000-B9A0    [NO MAX SET] Music Tracks
1D   B9A0-B9F8    Moved Music Engine
1D   BA00-BFC9    Moved Music Engine
1D   C682-C691    Moved Music Engine

1E   8000-85B0    Moving routines from bank 0E to 1E (PartyGen and menu stuff)
1E   85B0-85C1    Parry Permissions table
1E   8680-86A7    New Icons routine
1E   86E0-B9F1    Encounter Table True PRNG
1E   8800-8933    Class Info Window
1E  ~8970-8AF6    [NO MAX SET] Info Window Content
1E   B100-B21C    Damage Tiles Kill
1E   BA00-BC0D    Screen Tracking code
1E   BCC0-BCC4    Archipelago

1F   8000-FFFF    Moving Bank 0F to 1F for MMC3 expansion
