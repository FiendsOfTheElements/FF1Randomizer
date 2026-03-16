; Bank Usage - Last Update 2024-12-23
; Whenever a feature adds code to the rom, this table must be updated.
; Doesn't include the new features placed at the freed space at 0B and 0E. This is already a mess and no feature should be written there if possible.
; NOTE This list was done retroactively, so some ranges might be missing.
;
; ~ = No upper range is enforced in the code, data could potentially overlap other used ranges.
; ! = Potential bug, to correct

Address = offset - (bank * 0x4000) + 0x8000 - 0x10

0000-07FF RAM
0800-1FFF mirror of RAM
2000      VRAM
5000      Memory Mapper
6000-63FF expansion RAM (copied to SRAM on save)
6400-67FF SRAM
6856-686A Math Buffer
6800-7FFF Data/Rest of the SRAM chip
8000-BFFF low bank (program banks loaded here)
C000-FFFF always bank 1F

Bank Offset Range Description
-----------------------------
00000-03FFF
00   A618-A620    Alt fiend earth orb palette
00   A6D8-A6E0    Alt fiend volcano orb palette
00   A7F8-A800    Alt fiend sea shrine orb palette
00   A9A8-A9B0    Alt fiend sky palace orb palette
00   BF90-BF9A    Add item lookup to Bank 00

04000-07FFF
01

08000-0BFFF
02

0C000-09FFF
03   8220-8230    Palm tree tile used for pub sign (also 8320-8330)

; Freed  (write only after SM have been read)
10000-13FFF
04   8000-FFFF    Unused

14000-17FFF
05   8000-FFFF    Unused

18000-1BFFF
06   8000-FFFF    Unused

1C000-1FFFF
07

20000-23FFF
08

24000-27FFF
09

28000-2BFFF
0A

2C000-2FFFF
0B

30000-33FFF
0C   92F4-92FF    Enable Auto Retargeting
0C   93A5-93A8    Modify Undo Battle Command
0C   93AE-93BA    Modify After Fade In
0C   93D4-93D6    Allow strike first and surprise
0C   942C-9439    Set RNG
0C   9455-9458    Modify Battle Logic Loop
0C   95EC-9660    Write Drink Box Menu
0C   96E7-96F0    Castable Item Targeting
0C   97C7-97CA    Set RNG
0C   97CA-97D4    Controller Patch
0C   9910-991C    Better Battle Sprite
0C   9FE7-9FF7    ExtConsumables Write Cursor Position
0C   A03C-????    Palette per class in battle (redundant, but needed)
0C   A250-A275    Enable Auto Retargeting
0C   A26A-A26B    Increase Regen
0C   A2D7-A2E2    Poison Mode
0C   A357-A364    Set RNG
0C   A364-A3D3    Write Battle Do Turn
0C   A3E0-A3F2    Prevent running if first strike on unrunnable
0C   A445-A446    Minimum Player Sleep Bound
0C   A447-A448    Maximum Player Sleep Bound
0C   A4C2-A4C8    Enable Auto Retargeting
0C   A94B-A950    Enable Crit Number Display
0C   9C9E-9CE6    Battle Magic Menu Wrapping
0C   9CE6-9CF0    Zero Empty Space
0C   AD21-AD57    Move Load Player IB Stats
0C   ADBB-ADC7    Battle Hijack part of BB unarmed check
0C   B197-B1B1    Set RNG
0C   B1E2-B1E3    Minimum Enemy Sleep Bound
0C   B1E4-B1E5    Maximum Enemy Sleep Bound
0C   B1D5-B1D6    Scale Sleep Fix Enemy Stat
OC   B363-B378    Replaced MagicPtr Lookup With Call To GetPointerToMagicData Function
0C   B3CD-B3D8    Enable Auto Retargeting
0C   B43D-B440    Modify Battle PlMag Target One Player
0C   B453-B45B    Skip None Spells
0C   B7A2-B7A4    Prevent BtlMag_SavePlayerDefenderStats from saving mdef
0C   B8ED-B8F3    Magic Damage
0C   B905-B923    White Mage Harm All
0C   B9CD-B9DC    Write Cure Ailments Breakout
0C   BA46-BA68    Lock Mode

34000-37FFF
0D   B89F-????
0D   B85A-????
0D   B819-????
0D   BA64-????
0D   BA69-????
0D   A77B-????
0D   A779-????
0D   A77A-????

38000-3BFFF
0E   8DE4-8E10    Fix character stat rendering
OE   9079-90D9    Restore npc manip routines
0E   9273-92A3    Restore map object
0E   96B0-96F8    Item Jump Table
0E   9C54-9C7D    Exit Boss
0E   B16A-B16D    Exchange affected LDAs
0E   B16F-B172    Exchange affected LDAs
0E   B267-B268    Expand OWTP_SPEC_MASK

3C000-3FFFF
; Freed  (write only after data has been moved to 1F)
0F   8000-8B35    Stats Tracking code
0F   9000-93CD    Stats Tracking code
0F   8AD0-8AE3    Castable Items targeting
0F   8B40-8C06    Nones code
0F   8D00-8D53    Autosort
0F   9100-91AC    Progressive Scaling
0F   B000-B300    Expanded Teleporter tables
0F   F100-F200    Encounter RNG Table
0F   FCF1-FDF1    Battle RNG Table

40000-43FFF
10   8000-BFFF    Dialogues (whole bank is reserved)

44000-47FFF
11   8000-81A0    NPC Objects talk table
11  ~8200-85C6    [NO MAX SET] Talk Routines
11   8EA0-9000    Monster In A Box
11   901B-906D    Talk to object upgraded
11   902B-9915    Moving NPC talk routines
11   95C8-95D4    Random promotions [modify DoClassChanges, which was moved with NPC Talk Routines]
11   9600-9668    New utilities for talk routines
11   99A0-9CE3    Shop Upgrade Text routines
11   9DF0-9DFC    Promotion table
11   9F10-9F19    GiveItem talk routine
11  !A000-ADA7    Shop Upgrade text [Max Set at $B400, potential overlaps]
11   B120-B147    Extra talk routines
11   B180-B1AF    Check can take (C920)
11   B180-B1AF    Check can take (C916)
11   B400-B519    Treasure Stacks
11   B446-????    AP XP item
11   B447-????    non-AP XP item
11   B514-B54E    Build Progressive XP Chests
11   B536-????    Put the comparision in the chest counter
11   B44A-B44D    JSR to prog XP chests
11   B600-B700    Incentive Chests Item Fanfare LUT
11   B700-B831    Shop Upgrade routines
11   B900-B922    Open chests in order
11   B940-B9FF    Chests appear opened
11   BA00-BEE0    NPC Objects data

48000-4BFFF
12   8000-8800    Tileset copy
12   8A00-8C40    New Icons
12   8F61-8F81    Shard Icon
12   8E00-9000    Font Tileset
12   9000-9038    New Icons routines

4C000-4FFFF
13

50000-53FFF
14   8000-BFFF    Standard Maps (moved from 04, 05, 06 for extra space)

54000-57FFF
15   8000-BFFF    Standard Maps

58000-5BFFF
16   8000-BFFF    Standard Maps

5C000-5FFFF
17   8000-BFFF    Standard Maps

60000-63FFF
18

64000-67FFF
19   8000-8F90    QuickMiniMap map

68000-6BFFF
1A   84FB-84FE    Repeated Heal Potions
1A   8600-8852    Repeated Heal Potions

6C000-6FFFF
1B   8000-8FF5    Moving routines from bank 0B to 1B (Xp, levelup)
1B   874A-8759    Overwrite BB check with Unarmed LUT check
1B   874F-879E    BB Absorb bugfix P1
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

70000-73FFF
1C   A010-A015    ExtConsumables LUT
1C   A020-A165    ExtConsumables Drink box
1C   A1C0-A1EF    AutoRetargeting
1C   A200-A246    White Mage Harm Everyone
1C   A221-        Improved Harm Blursing
1C   A225-        Improved Harm Blursing
1C   A250-A275    AutoRetargeting
1C   A290-A4B7    Int Affect Spells
1C   A4F0-A50E    Stats Tracking
1C   A600-A648    Spell Reordering
1C   A670-A724    Poison Mode routines
1C   A790-A7B7    Alt Chaos Battle Music
1C   A7AC-A7B7    SetRNG
72010<  >727B7

74000-77FFF
1D  ~8000-B9A0    [NO MAX SET] Music Tracks
1D   B9A0-B9F8    Moved Music Engine
1D   BA00-BFC9    Moved Music Engine
1D   C682-C691    Moved Music Engine

78000-7BFFF
1E   8000-85B0    Moving routines from bank 0E to 1E (PartyGen and menu stuff)
1E   806B-8070    Battlestep to SRAM Jump
1E   85B0-85C1    Parry Permissions table
1E   8680-86A7    New Icons routine
1E   86E0-B9F1    Encounter Table True PRNG
1E   8800-8933    Class Info Window
1E  ~8970-8AF6    [NO MAX SET] Info Window Content
1E   B000-B015    Enable Damage Tiles
1E   B100-????    Damage tiles kill and can be resisted
1E   B100-B21C    Damage Tiles Kill
1E   BA00-BC0D    Screen Tracking code
1E   BCC0-BCC4    Archipelago

7C000-7FFFF
1F   C000-FFFF    Moving Bank 0F to 1F for MMC3 expansion
1F   C112-C113    Remove wave sound
1F   C1A9-C1AA    Disable Minimap
1F   C216-C217    AirShip visibility
1F   C220-C221    AirShip X
1F   C22A-C22B    AirShip Y
1F   C22F-C230
1F   C238-C23B    AirShip animation
1F   C25D-C25E    Check for ship
1F   C2EC-C2ED    Update mask for docking
1F   C33C-C344    Damage tiles
1F   C4DA-C4DB    Damage tiles
1F   C506-C507    No battle in ship
1F   C571-C575    Encounter Table Traversal
1F   C571-C58B    PRNG Algorithm
1F   DB09-DB10    Battle Step Seed
1F   C6B4-C6B5    Damage tiles
1F   C6BC-C6BF    Land AirShip
1F   C6E8-C6E9    AirShip X
1F   C6F0-C6F1    AirShip Y
1F   C759-C762    Music?
1F   C7E7-C7FC    Remove damage tile sound
1F   C861-C869    Damage tiles kill and can be resisted
1F   C86C-C86D    Damage tile value
1F   C874-C875    Damage tile value
1F   DDD0-DDE0    Build Progressive XP Chests
1F   E289-E28A    Don't animate ship
1F   E28E-E290    Don't animate ship
1F   ECD5-ECD7    Load Price bank switching fix
1F   EEBF-EEC9    Overwrite UnadjustBBEquipStats
1F   EEDD-EEE7    Overwrite UnadjustBBEquipStats
1F   F100-F200    Encounter RNG Table
1F   FCF1-FDF1    Battle RNG Table
