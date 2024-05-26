using System.ComponentModel;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using FF1Lib.Helpers;

namespace FF1Lib
{

	public enum SpellRoutine : byte
	{
	        None = 0,
		Damage = 0x01,
		DamageUndead = 0x02,
		Heal = 0x07,
		CureAilment = 0x08,
		FullHeal = 0x0F,
		ArmorUp = 0x09,
		DefElement = 0x0A,
		Fast = 0x0C,
		Sabr = 0x0D,
		Lock = 0x0E,
		Ruse = 0x10,
		PowerWord = 0x12,
		InflictStatus = 0x03,
		Life = 0xF0,
		Smoke = 0xF1,
		Slow = 0x04,
		Fear = 0x05,
		Xfer = 0x11,
	}

	public enum SpellTargeting : byte
	{
		Any = 0xFF,
		None = 0,
		AllEnemies = 0x01,
		OneEnemy = 0x02,
		Self = 0x04,
		AllCharacters = 0x08,
		OneCharacter = 0x10
	}

	[Flags]
	public enum SpellElement : byte
	{
		Any       = 0b10101010,
		None      = 0x00,
		Earth     = 0b10000000,
		Lightning = 0b01000000,
		Ice       = 0b00100000,
		Fire      = 0b00010000,
		Death     = 0b00001000,
		Time      = 0b00000100,
		Poison    = 0b00000010,
		Status    = 0b00000001,
		All       = 0xFF
	}

	[Flags]
	public enum SpellStatus : byte
	{
	        None = 0,
		Any = 0xFF,
		Confuse = 0b10000000,
		Mute = 0b01000000,
		Dark = 0b00001000,
		Stun = 0b00010000,
		Sleep = 0b00100000,
		Stone = 0b00000010,
		Death = 0b00000001,
		Poison = 0b00000100
	}

	public enum OOBSpellRoutine : byte {
	    CURE = 0,
	    CUR2 = 1,
	    CUR3 = 2,
	    CUR4 = 3,
	    HEAL = 4,
	    HEL3 = 5,
	    HEL2 = 6,
	    PURE = 7,
	    LIFE = 8,
	    LIF2 = 9,
	    WARP = 10,
	    SOFT = 11,
	    EXIT = 12,
	    None = 255
	}

	public enum MagicGraphic : byte  {
	    None = 0,
	    BarOfLight = 176,
	    FourSparkles = 184,
	    Stars = 192,
	    EnergyBeam = 200,
	    EnergyFlare = 208,
	    GlowingBall = 216,
	    LargeSparkle = 224,
	    SparklingHand = 232
	}

	public enum SpellColor : byte {
	    White = 0x20,
	    Blue = 0x21,
	    Violet = 0x22,
	    Purple = 0x23,
	    Pink = 0x24,
	    PinkOrange = 0x25,
	    LightOrange = 0x26,
	    DarkOrange = 0x27,
	    Yellow = 0x28,
	    Green = 0x29,
	    LightGreen = 0x2A,
	    BlueGreen = 0x2B,
	    Teal = 0x2C,
	    Gray = 0x2D,
	    Black1 = 0x2E,
	    Black2 = 0x2F
	}

	public enum SpellSchools
	{
		White = 0,
		Black
	}

	[JsonObject(MemberSerialization.OptIn)]
	public class MagicSpell
	{
	    [JsonProperty]
	    public byte Index;

	    public bool ShouldSerializeIndex() {
		return !isRegularSpell;
	    }

	    public Blob Data;

	    [JsonProperty]
	    public string Name;

	    [JsonProperty]
	    public byte TextPointer;

	    public bool ShouldSerializeTextPointer() {
		return isRegularSpell;
	    }

	    [JsonProperty]
	    public string Message;

	    public bool ShouldSerializeMessage() {
		return isRegularSpell;
	    }

	    [JsonProperty]
	    public byte accuracy = 0;

	    [JsonProperty]
	    [JsonConverter(typeof(StringEnumConverter))]
	    public SpellElement elem = 0;

	    [JsonProperty]
	    [JsonConverter(typeof(StringEnumConverter))]
	    public SpellTargeting targeting = 0;

	    [JsonProperty]
	    [JsonConverter(typeof(StringEnumConverter))]
	    public SpellRoutine routine = 0;

	    [JsonProperty]
	    public byte effect = 0;

	    public bool ShouldSerializeeffect() {
		return routine != SpellRoutine.CureAilment &&
		    routine != SpellRoutine.InflictStatus &&
		    routine != SpellRoutine.DefElement &&
		    routine != SpellRoutine.Slow &&
		    routine != SpellRoutine.None &&
		    routine != SpellRoutine.FullHeal &&
		    routine != SpellRoutine.Xfer;

	    }

	    [JsonProperty]
	    [JsonConverter(typeof(StringEnumConverter))]
	    public SpellStatus status {
		get { return (SpellStatus)effect; }
		set { effect = (byte)value; }
	    }
	    public bool ShouldSerializestatus() {
		return routine == SpellRoutine.CureAilment || routine == SpellRoutine.InflictStatus;
	    }

	    [JsonProperty]
	    [JsonConverter(typeof(StringEnumConverter))]
	    public SpellElement defelement {
		get {
		    return (SpellElement)effect;
		}
		set {
		    effect = (byte)value;
		}
	    }
	    public bool ShouldSerializedefelement() {
		return routine == SpellRoutine.DefElement;
	    }

	    public byte gfx = 0;

	    [JsonProperty]
	    [JsonConverter(typeof(StringEnumConverter))]
	    public MagicGraphic MagicGraphic {
		get {
		    return (MagicGraphic)gfx;
		}
		set {
		    gfx = (byte)value;
		}
	    }

	    public bool ShouldSerializegfx() {
		return isRegularSpell;
	    }

	    public byte palette = 0;

	    [JsonProperty]
	    [JsonConverter(typeof(StringEnumConverter))]
	    public SpellColor SpellColor {
		get {
		    return (SpellColor)palette;
		}
		set {
		    palette = (byte)value;
		}
	    }

	    public bool ShouldSerializeSpellColor() {
		return isRegularSpell;
	    }

	    void updateMagicIndex(byte level, byte slot, SpellSchools type) {
		this.Index = (byte)((level-1) * 8 + (slot-1));
		if (type == SpellSchools.Black) {
		    this.Index += 4;
		}
	    }

	    [JsonProperty]
	    public byte Level {
		get {
		    return (byte)((Index / 8)+1);
		}
		set {
		    if (value < 1 || value > 8) {
			throw new Exception("Spell level must be between 1 and 8");
		    }
		    this.updateMagicIndex(value, Slot, SpellSchool);
		}
	    }

	    public bool ShouldSerializeLevel() {
		return isRegularSpell;
	    }

	    [JsonProperty]
	    public byte Slot {
		get {
		    return (byte)((Index % 4) + 1);
		}
		set {
		    if (value < 1 || value > 4) {
			throw new Exception("Spell slot must be between 1 and 4");
		    }
		    this.updateMagicIndex(Level, value, SpellSchool);
		}
	    }

	    public bool ShouldSerializeSlot() {
		return isRegularSpell;
	    }

	    [JsonProperty]
	    [JsonConverter(typeof(StringEnumConverter))]
	    public SpellSchools SpellSchool {
		get {
		    if (Index % 8 < 4) {
			return SpellSchools.White;
		    }
		    return SpellSchools.Black;
		}
		set {
		    this.updateMagicIndex(Level, Slot, value);
		}
	    }

	    public bool ShouldSerializeMagicType() {
		return isRegularSpell;
	    }

	    [JsonProperty]
	    [JsonConverter(typeof(StringEnumConverter))]
	    public OOBSpellRoutine oobSpellRoutine = OOBSpellRoutine.None;

	    public bool ShouldSerializeoobSpellRoutine() {
		return isRegularSpell;
	    }

	    List<Classes> _permissions = new();

	    [JsonProperty]
	    public string permissions {
		get {
		    if (_permissions == null) {
			return "";
		    }
		    string ret = "";
		    foreach (var c in _permissions) {
			if (ret != "") {
			    ret += ", ";
			}
			ret += Enum.GetName(c);
		    }
		    return ret;
		}
		set {
		    _permissions.Clear();
		    var sp = value.Split(",");
		    foreach (var cl in sp) {
			_permissions.Add(Enum.Parse<Classes>(cl));
		    }
		}
	    }

	    public bool ShouldSerializepermissions() {
		return isRegularSpell;
	    }

	    bool isRegularSpell;

	    public MagicSpell(byte _Index,
			      Blob _Data,
			      string _Name,
			      byte _TextPointer,
			      string _Message,
			      List<Classes> __permissions)
	    {
		Index = _Index;
		Data = _Data;
		Name = _Name;
		TextPointer = _TextPointer;
		Message = _Message;
		_permissions = __permissions;
		isRegularSpell = true;
		this.decompressData(Data);
	    }

	    public MagicSpell(byte _Index,
			      Blob _Data,
			      string _Name,
			      bool _isRegularSpell)
	    {
		Index = _Index;
		Data = _Data;
		Name = _Name;
		isRegularSpell = _isRegularSpell;
		this.decompressData(Data);
	    }

	    public MagicSpell()
	    {
		Data = (Blob)new byte[8];
		isRegularSpell = true;
	    }

	    public byte[] compressData()
	    {
		Data[0] = accuracy;
		Data[1] = effect;
		Data[2] = (byte)elem;
		Data[3] = (byte)targeting;
		Data[4] = (byte)routine;
		Data[5] = gfx;
		Data[6] = palette;
		Data[7] = 0x00; // last byte is always 00
		return Data;
	    }

	    public void writeData(FF1Rom rom) {
		compressData();
		if (isRegularSpell) {
		    rom.Put(FF1Rom.MagicOffset + FF1Rom.MagicSize * Index, Data);
		    rom.ItemsText[176 + Index] = Name;
		}
	    }

	    public void decompressData(byte[] data)
	    {
		accuracy = data[0];
		effect = data[1];
		elem = (SpellElement)data[2];
		targeting = (SpellTargeting)data[3];
		routine = (SpellRoutine)data[4];
		gfx = data[5];
		palette = data[6];
	    }

	    public override string ToString()
	    {
		return Index.ToString() + ": " + Name;
	    }
	}

	public partial class FF1Rom : NesRom
	{
		public const int MagicOffset = 0x301E0;
		public const int MagicSize = 8;
		public const int MagicCount = 64;
		public const int MagicNamesOffset = 0x2BE03;
		public const int MagicNameSize = 5;
		public const int MagicTextPointersOffset = 0x304C0;
		public const int MagicPermissionsOffset = 0x3AD18;
		public const int MagicPermissionsSize = 8;
		public const int MagicPermissionsCount = 12;
		public const int MagicOutOfBattleOffset = 0x3AEFA;
		public const int MagicOutOfBattleSize = 7;
		public const int MagicOutOfBattleCount = 13;

		public const int OldLevelUpDataOffset = 0x2D094; // this was moved to bank 1B
		public const int NewLevelUpDataOffset = 0x6CDA9; // this was moved from bank 1B
		public const int oldKnightNinjaMaxMPOffset = 0x6C907;
		public const int newKnightNinjaMaxMPOffset = 0x6D344;

		public const int ConfusedSpellIndexOffset = 0x3321E;
		public const int FireSpellIndex = 4;

		public const int WeaponOffset = 0x30000;
		public const int WeaponSize = 8;
		public const int WeaponCount = 40;

		public const int ArmorOffset = 0x30140;
		public const int ArmorSize = 4;
		public const int ArmorCount = 40;

		public void ShuffleMagicLevels(EnemyScripts enemyScripts, MT19337 rng, bool enable, bool keepPermissions, bool tieredShuffle, bool mixSpellbooks)
		{
			if (!enable)
			{
				return;
			}

			var magicSpells = GetSpells();

			// First we have to un-interleave white and black spells.
			var whiteSpells = magicSpells.Where((spell, i) => (i / 4) % 2 == 0).ToList();
			var blackSpells = magicSpells.Where((spell, i) => (i / 4) % 2 == 1).ToList();

			if(tieredShuffle)
			{
				// weigh spell probability of landing in a tier based on where it was in the original game
				var whiteSpellList = new List<MagicSpell>[3];
				var blackSpellList = new List<MagicSpell>[3];
				var whiteSpellFinalList = new List<MagicSpell>[3];
				var blackSpellFinalList = new List<MagicSpell>[3];
				int mergedSpellDoubler = 1;
				whiteSpellList[0] = magicSpells.Where((spell, i) => (i / 4) % 2 == 0 && i < 24).ToList();
				whiteSpellList[1] = magicSpells.Where((spell, i) => (i / 4) % 2 == 0 && i < 48 && i >= 24).ToList();
				whiteSpellList[2] = magicSpells.Where((spell, i) => (i / 4) % 2 == 0 && i >= 48).ToList();
				blackSpellList[0] = magicSpells.Where((spell, i) => (i / 4) % 2 == 1 && i < 24).ToList();
				blackSpellList[1] = magicSpells.Where((spell, i) => (i / 4) % 2 == 1 && i < 48 && i >= 24).ToList();
				blackSpellList[2] = magicSpells.Where((spell, i) => (i / 4) % 2 == 1 && i >= 48).ToList();
				if(mixSpellbooks)
				{
					whiteSpellList[0] = whiteSpellList[0].Concat(blackSpellList[0]).ToList();
					whiteSpellList[1] = whiteSpellList[1].Concat(blackSpellList[1]).ToList();
					whiteSpellList[2] = whiteSpellList[2].Concat(blackSpellList[2]).ToList();
					mergedSpellDoubler = 2;
				}
				whiteSpellFinalList[0] = new List<MagicSpell> { };
				whiteSpellFinalList[1] = new List<MagicSpell> { };
				whiteSpellFinalList[2] = new List<MagicSpell> { };
				blackSpellFinalList[0] = new List<MagicSpell> { };
				blackSpellFinalList[1] = new List<MagicSpell> { };
				blackSpellFinalList[2] = new List<MagicSpell> { };
				whiteSpells.Clear();
				blackSpells.Clear();
				foreach (MagicSpell spell in whiteSpellList[2])
				{
					// 70% chance of tier 7-8, 25% chance of tier 4-6, 5% chance of tier 1-3
					int diceRoll = rng.Between(0, 19);
					if(diceRoll < 14)
					{
						whiteSpellFinalList[2].Add(spell);
					}
					else if (diceRoll < 19)
					{
						whiteSpellFinalList[1].Add(spell);
					}
					else
					{
						whiteSpellFinalList[0].Add(spell);
					}
				}
				foreach (MagicSpell spell in whiteSpellList[1])
				{
					// 60% chance of tier 4-6, 25% chance of tier 1-3, 15% chance of tier 7-8
					// if a section of the final list is full, move to another section
					int diceRoll = rng.Between(0, 19);
					if(diceRoll < 12)
					{
						if(whiteSpellFinalList[1].Count >= 12 * mergedSpellDoubler)
						{
							if(whiteSpellFinalList[0].Count >= 12 * mergedSpellDoubler)
							{
								whiteSpellFinalList[2].Add(spell);
							}
							else
							{
								whiteSpellFinalList[0].Add(spell);
							}
						}
						else
						{
							whiteSpellFinalList[1].Add(spell);
						}
					}
					else if (diceRoll < 17)
					{
						if(whiteSpellFinalList[0].Count >= 12 * mergedSpellDoubler)
						{
							if(whiteSpellFinalList[1].Count >= 12 * mergedSpellDoubler)
							{
								whiteSpellFinalList[2].Add(spell);
							}
							else
							{
								whiteSpellFinalList[1].Add(spell);
							}
						}
						else
						{
							whiteSpellFinalList[0].Add(spell);
						}
					}
					else
					{
						if(whiteSpellFinalList[2].Count >= 8 * mergedSpellDoubler)
						{
							if(whiteSpellFinalList[1].Count >= 12 * mergedSpellDoubler)
							{
								whiteSpellFinalList[0].Add(spell);
							}
							else
							{
								whiteSpellFinalList[1].Add(spell);
							}
						}
						else
						{
							whiteSpellFinalList[2].Add(spell);
						}
					}
				}
				foreach(MagicSpell spell in whiteSpellList[0])
				{
					// fill the remaining tiers with the tier 1-3 base magic
					if(whiteSpellFinalList[0].Count >= 12 * mergedSpellDoubler)
					{
						if(whiteSpellFinalList[1].Count >= 12 * mergedSpellDoubler)
						{
							whiteSpellFinalList[2].Add(spell);
						}
						else
						{
							whiteSpellFinalList[1].Add(spell);
						}
					}
					else
					{
						whiteSpellFinalList[0].Add(spell);
					}
				}
				// and repeat the process for black magic if we didn't mix spellbooks
				if(mixSpellbooks)
				{
					// if we mixed spellbooks, split the white (merged) spellbook in halves to set the black spell list
					blackSpellFinalList[0] = whiteSpellFinalList[0].Take(12).ToList();
					whiteSpellFinalList[0] = whiteSpellFinalList[0].Except(blackSpellFinalList[0]).ToList();
					blackSpellFinalList[1] = whiteSpellFinalList[1].Take(12).ToList();
					whiteSpellFinalList[1] = whiteSpellFinalList[1].Except(blackSpellFinalList[1]).ToList();
					blackSpellFinalList[2] = whiteSpellFinalList[2].Take(8).ToList();
					whiteSpellFinalList[2] = whiteSpellFinalList[2].Except(blackSpellFinalList[2]).ToList();
				}
				else
				{
					foreach (MagicSpell spell in blackSpellList[2])
					{
						// 70% chance of tier 7-8, 25% chance of tier 4-6, 5% chance of tier 1-3
						int diceRoll = rng.Between(0, 19);
						if (diceRoll < 14)
						{
							blackSpellFinalList[2].Add(spell);
						}
						else if (diceRoll < 19)
						{
							blackSpellFinalList[1].Add(spell);
						}
						else
						{
							blackSpellFinalList[0].Add(spell);
						}
					}
					foreach (MagicSpell spell in blackSpellList[1])
					{
						// 60% chance of tier 4-6, 25% chance of tier 1-3, 15% chance of tier 7-8
						// if a section of the final list is full, move to another section
						int diceRoll = rng.Between(0, 19);
						if (diceRoll < 12)
						{
							if (blackSpellFinalList[1].Count >= 12)
							{
								if (blackSpellFinalList[0].Count >= 12)
								{
									blackSpellFinalList[2].Add(spell);
								}
								else
								{
									blackSpellFinalList[0].Add(spell);
								}
							}
							else
							{
								blackSpellFinalList[1].Add(spell);
							}
						}
						else if (diceRoll < 17)
						{
							if (blackSpellFinalList[0].Count >= 12)
							{
								if (blackSpellFinalList[1].Count >= 12)
								{
									blackSpellFinalList[2].Add(spell);
								}
								else
								{
									blackSpellFinalList[1].Add(spell);
								}
							}
							else
							{
								blackSpellFinalList[0].Add(spell);
							}
						}
						else
						{
							if (blackSpellFinalList[2].Count >= 8)
							{
								if (blackSpellFinalList[1].Count >= 12)
								{
									blackSpellFinalList[0].Add(spell);
								}
								else
								{
									blackSpellFinalList[1].Add(spell);
								}
							}
							else
							{
								blackSpellFinalList[2].Add(spell);
							}
						}
					}
					foreach (MagicSpell spell in blackSpellList[0])
					{
						// fill the remaining tiers with the tier 1-3 base magic
						if (blackSpellFinalList[0].Count >= 12)
						{
							if (blackSpellFinalList[1].Count >= 12)
							{
								blackSpellFinalList[2].Add(spell);
							}
							else
							{
								blackSpellFinalList[1].Add(spell);
							}
						}
						else
						{
							blackSpellFinalList[0].Add(spell);
						}
					}
				}
				// shuffle each of the final lists
				foreach(List<MagicSpell> spellList in whiteSpellFinalList)
				{
					spellList.Shuffle(rng);
				}
				if(!mixSpellbooks)
				{
					foreach (List<MagicSpell> spellList in blackSpellFinalList)
					{
						spellList.Shuffle(rng);
					}
				}
				// and append each in turn to the whitespells / blackspells list
				whiteSpells = whiteSpells.Concat(whiteSpellFinalList[0]).ToList();
				whiteSpells = whiteSpells.Concat(whiteSpellFinalList[1]).ToList();
				whiteSpells = whiteSpells.Concat(whiteSpellFinalList[2]).ToList();
				blackSpells = blackSpells.Concat(blackSpellFinalList[0]).ToList();
				blackSpells = blackSpells.Concat(blackSpellFinalList[1]).ToList();
				blackSpells = blackSpells.Concat(blackSpellFinalList[2]).ToList();
			}
			else
			{
				if(mixSpellbooks)
				{
					var mergedList = magicSpells.ToList();
					mergedList.Shuffle(rng);
					whiteSpells = mergedList.Where((spell, i) => (i / 4) % 2 == 0).ToList();
					blackSpells = mergedList.Where((spell, i) => (i / 4) % 2 == 1).ToList();
				}
				else
				{
					whiteSpells.Shuffle(rng);
					blackSpells.Shuffle(rng);
				}
			}

			// Now we re-interleave the spells.
			var shuffledSpells = new List<MagicSpell>();
			for (int i = 0; i < MagicCount; i++)
			{
				var sourceIndex = 4 * (i / 8) + i % 4;
				if ((i / 4) % 2 == 0)
				{
					shuffledSpells.Add(whiteSpells[sourceIndex]);
				}
				else
				{
					shuffledSpells.Add(blackSpells[sourceIndex]);
				}
			}

			Put(MagicOffset, shuffledSpells.Select(spell => spell.Data).Aggregate((seed, next) => seed + next));
			PutSpellNames(shuffledSpells);
			Put(MagicTextPointersOffset, shuffledSpells.Select(spell => spell.TextPointer).ToArray());

			if (keepPermissions)
			{
				// Shuffle the permissions the same way the spells were shuffled.
				for (int c = 0; c < MagicPermissionsCount; c++)
				{
					SpellPermissions[(Classes)c] = SpellPermissions[(Classes)c].Select(x => (SpellSlots)shuffledSpells.FindIndex(y => y.Index == (int)x)).ToList();
				}
			}

			// Map old indices to new indices.
			var newIndices = new byte[MagicCount];
			for (byte i = 0; i < MagicCount; i++)
			{
				newIndices[shuffledSpells[i].Index] = i;
			}

			// Fix enemy spell pointers to point to where the spells are now.
			enemyScripts.UpdateSpellsIndices(newIndices.Select((s, i) => (i, s)).ToDictionary(s => (SpellByte)s.i, s => (SpellByte)s.s));

			// Fix weapon and armor spell pointers to point to where the spells are now.
			var weapons = Get(WeaponOffset, WeaponSize * WeaponCount).Chunk(WeaponSize);
			foreach (var weapon in weapons)
			{
				if (weapon[3] != 0x00)
				{
					weapon[3] = (byte)(newIndices[weapon[3] - 1] + 1);
				}
			}
			Put(WeaponOffset, weapons.SelectMany(weapon => weapon.ToBytes()).ToArray());

			var armors = Get(ArmorOffset, ArmorSize * ArmorCount).Chunk(ArmorSize);
			foreach (var armor in armors)
			{
				if (armor[3] != 0x00)
				{
					armor[3] = (byte)(newIndices[armor[3] - 1] + 1);
				}
			}
			Put(ArmorOffset, armors.SelectMany(armor => armor.ToBytes()).ToArray());

			// Fix the crazy out of battle spell system.
			var outOfBattleSpellOffset = MagicOutOfBattleOffset;
			for (int i = 0; i < MagicOutOfBattleCount; i++)
			{
				var oldSpellIndex = Data[outOfBattleSpellOffset] - 0xB0;
				var newSpellIndex = newIndices[oldSpellIndex];

				Put(outOfBattleSpellOffset, new[] { (byte)(newSpellIndex + 0xB0) });

				outOfBattleSpellOffset += MagicOutOfBattleSize;
			}

			// Confused enemies are supposed to cast FIRE, so figure out where FIRE ended up.
			var newFireSpellIndex = shuffledSpells.FindIndex(spell => spell.Data == magicSpells[FireSpellIndex].Data);
			Put(ConfusedSpellIndexOffset, new[] { (byte)newFireSpellIndex });
		}

		public List<MagicSpell> GetSpells() {
			var spells = Get(MagicOffset, MagicSize * MagicCount).Chunk(MagicSize);
			var pointers = Get(MagicTextPointersOffset, MagicCount);

			var battleMessages = new BattleMessages(this);

			var spellsList = spells.Select((spell, i) => new MagicSpell((byte)i, spell, ItemsText[176 + i], pointers[i],
										    pointers[i] > 0 ? battleMessages[pointers[i]-1] : "",
										    SpellPermissions.PermissionsFor((SpellSlots)i))
			).ToList();

			for (int i = 0; i < MagicOutOfBattleCount; i++) {
			    var spellIndex = Data[MagicOutOfBattleOffset  + i*MagicOutOfBattleSize] - 0xB0;
			    spellsList[spellIndex].oobSpellRoutine = (OOBSpellRoutine)i;
			}
			return spellsList;
		}

		public void PutSpells(List<MagicSpell> spellsList, EnemyScripts enemyScripts) {

		    spellsList.Sort(delegate(MagicSpell a, MagicSpell b) { return a.Index.CompareTo(b.Index); });

		    var oldSpells = GetSpells();

		    foreach (var sp in spellsList) {
			sp.writeData(this);

			if (sp.oobSpellRoutine == OOBSpellRoutine.None) {
			    continue;
			}

			// update the out of battle magic code, it's a simple hardcoded table
			// that compares the spell index and jumps to the desired routine
			Data[MagicOutOfBattleOffset + (MagicOutOfBattleSize * (int)sp.oobSpellRoutine)] = (byte)(sp.Index + 0xB0);

			// update the effectivity of healing spells
			int mask = 1;
			while (sp.effect >= mask) {
			    mask = mask << 1;
			}
			mask = mask >> 1;
			mask = mask - 1;


			switch (sp.oobSpellRoutine) {
			    case OOBSpellRoutine.CURE:
				// AND #mask
				// ADC #effect
				Put(0x3AF5E, new byte[] { 0x29, (byte)mask, 0x69, sp.effect }); // changing the oob code for CURE to reflect new values
				break;
			    case OOBSpellRoutine.CUR2:
				Put(0x3AF66, new byte[] { 0x29, (byte)mask, 0x69, sp.effect }); // changing the oob code for CUR2 to reflect new values
				break;
			    case OOBSpellRoutine.CUR3:
				Put(0x3AF6E, new byte[] { 0x29, (byte)mask, 0x69, sp.effect }); // changing the oob code for CUR3 to reflect new values
				break;

			    case OOBSpellRoutine.HEAL:
				// AND #mask
				// CLC
				// ADC #effect
				Put(0x3AFDB, new byte[] { 0x29, (byte)mask, 0x18, 0x69, sp.effect }); // changing the oob code for HEAL to reflect the above effect
				break;
			    case OOBSpellRoutine.HEL2:
				Put(0x3AFE4, new byte[] { 0x29, (byte)mask, 0x18, 0x69, sp.effect }); // changing the oob code for HEL2 to reflect the above effect
				break;
			    case OOBSpellRoutine.HEL3:
				Put(0x3AFED, new byte[] { 0x29, (byte)mask, 0x18, 0x69, sp.effect }); // changing the oob code for HEL3 to reflect the above effect
				break;
			    default:
				break;
			}
		    }

		    var sh = new SpellHelper(spellsList);

		    Dictionary<Spell, Spell> oldToNew = new();

		    for (int i = 0; i < oldSpells.Count; i++) {
			var sp = oldSpells[i];
			IEnumerable<(Spell Id, MagicSpell Info)> result;

			result = sh.FindSpells(sp.routine, sp.targeting, sp.elem, sp.status, sp.oobSpellRoutine);

			if (!result.Any()) {
			    // Relax element
			    result = sh.FindSpells(sp.routine, sp.targeting, SpellElement.Any, sp.status, sp.oobSpellRoutine);
			}

			if (!result.Any() && sp.routine != SpellRoutine.None) {
			    // Relax OOB spell routine
			    result = sh.FindSpells(sp.routine, sp.targeting, SpellElement.Any, sp.status, OOBSpellRoutine.None);
			}

			if (!result.Any()) {
			    // Relax targeting
			    if (sp.targeting == SpellTargeting.AllEnemies) {
				result = sh.FindSpells(sp.routine, SpellTargeting.OneEnemy, SpellElement.Any, sp.status, OOBSpellRoutine.None);
			    } else if (sp.targeting == SpellTargeting.Self) {
				result = sh.FindSpells(sp.routine, SpellTargeting.OneCharacter, SpellElement.Any, sp.status, OOBSpellRoutine.None);
				if (!result.Any()) {
				    result = sh.FindSpells(sp.routine, SpellTargeting.AllCharacters, SpellElement.Any, sp.status, OOBSpellRoutine.None);
				}
			    } else if (sp.targeting == SpellTargeting.OneCharacter) {
				result = sh.FindSpells(sp.routine, SpellTargeting.AllCharacters, SpellElement.Any, sp.status, OOBSpellRoutine.None);
				if (!result.Any()) {
				    result = sh.FindSpells(sp.routine, SpellTargeting.Self, SpellElement.Any, sp.status, OOBSpellRoutine.None);
				}
			    } else if (sp.targeting == SpellTargeting.AllCharacters) {
				result = sh.FindSpells(sp.routine, SpellTargeting.OneCharacter, SpellElement.Any, sp.status, OOBSpellRoutine.None);
				if (!result.Any()) {
				    result = sh.FindSpells(sp.routine, SpellTargeting.Self, SpellElement.Any, sp.status, OOBSpellRoutine.None);
				}
			    }
			}

			if (!result.Any()) {
			    throw new Exception($"Cannot find replacement spell for {sp.Name} with {sp.routine} {sp.status}");
			}

			if (result.Count() == 1) {
			    oldToNew[(Spell)((int)Spell.CURE + i)] = result.First().Item1;
			    continue;
			}

			if (sp.routine == SpellRoutine.Damage || sp.routine == SpellRoutine.DamageUndead ||
			    sp.routine == SpellRoutine.Heal || sp.routine == SpellRoutine.ArmorUp ||
			    sp.routine == SpellRoutine.Sabr || sp.routine == SpellRoutine.Lock ||
			    sp.routine == SpellRoutine.Ruse || sp.routine == SpellRoutine.Fear)
			{
			    // Find the new spell that's closest to the old spell
			    // based on effectivity
			    int minimum = 256;
			    foreach (var candidate in result) {
				int diff = Math.Abs(candidate.Item2.effect - sp.effect);
				if (diff < minimum) {
				    minimum = diff;
				    oldToNew[(Spell)((int)Spell.CURE + i)] = candidate.Item1;
				}
			    }
			} else {
			    int minimum = 256;
			    // Find the new spell that's closest to the old spell
			    // based on accuracy
			    foreach (var candidate in result) {
				var diff = Math.Abs(candidate.Item2.accuracy - sp.accuracy);
				if (diff < minimum) {
				    minimum = diff;
				    oldToNew[(Spell)((int)Spell.CURE + i)] = candidate.Item1;
				}
			    }
			}
		    }

			/*
		    foreach (var kv in oldToNew) {
			Console.WriteLine($"{(int)kv.Key - (int)Spell.CURE} -> {(int)kv.Value - (int)Spell.CURE}");
			Console.WriteLine($"{oldSpells[(int)kv.Key - (int)Spell.CURE]} -> {spellsList[(int)kv.Value - (int)Spell.CURE]}");
		    }
		    */

			// Fix enemy spell pointers to point to where the spells are now.
			enemyScripts.UpdateSpellsIndices(oldToNew.ToDictionary(s => (SpellByte)(s.Key - (byte)Spell.CURE), s => (SpellByte)(s.Value - (byte)Spell.CURE)));

		    // Fix weapon and armor spell pointers to point to where the spells are now.
		    foreach (var wep in Weapon.LoadAllWeapons(this, null)) {
			if (wep.Spell != Spell.None) {
			    wep.SpellIndex = (byte)((int)oldToNew[wep.Spell] - (int)Spell.CURE + 1);
			}
			wep.writeWeaponMemory(this);
		    }

		    foreach (var arm in Armor.LoadAllArmors(this, null)) {
			if (arm.Spell != Spell.None) {
			    arm.SpellIndex = (byte)((int)oldToNew[arm.Spell] - (int)Spell.CURE + 1);
			}
			arm.writeArmorMemory(this);
		    }

		    // Confused enemies are supposed to cast FIRE, so
		    // pick a single-target damage spell.
		    var confSpell = sh.FindSpells(SpellRoutine.Damage, SpellTargeting.OneEnemy);
		    if (!confSpell.Any()) {
			throw new Exception("Missing a single-target damage spell to use for confused status");
		    }
		    Put(ConfusedSpellIndexOffset, new[] { (byte)confSpell.First().Item2.Index });
		}

		public void PutSpellNames(List<MagicSpell> spells)
		{

			for(int i = 0; i < spells.Count; i++)
			{
				ItemsText[176 + i] = spells[i].Name;
			}
		}

		public void AccessibleSpellNames(Flags flags)
		{
			// If Spellcrafter mode is on, abort. We need a check here as the setting on the site can be in a random state.
			if ((bool)flags.GenerateNewSpellbook)
			{
				return;
			}

			var magicSpells = GetSpells();

			// Since this can be performed independent of the magic shuffling, we can't assume the location of spell names.
			// We will loop through the spell list and replace the appropriate names as we find them.
			for (int i = 0; i < magicSpells.Count; i++)
			{
				MagicSpell newSpell = magicSpells[i];
				string spellName = magicSpells[i].Name;

				switch (spellName)
				{
					// Note that 3 letter spell names actually have a trailing space
					case "LIT ":
						newSpell.Name = "THUN";
						break;
					case "LIT2":
						newSpell.Name = "THN2";
						break;
					case "LIT3":
						newSpell.Name = "THN3";
						break;
					case "FAST":
						newSpell.Name = "HAST";
						break;
					case "SLEP":
						newSpell.Name = "DOZE";
						break;
					case "SLP2":
						newSpell.Name = "DOZ2";
						break;

					case "HARM":
						newSpell.Name = "DIA ";
						break;
					case "HRM2":
						newSpell.Name = "DIA2";
						break;
					case "HRM3":
						newSpell.Name = "DIA3";
						break;
					case "HRM4":
						newSpell.Name = "DIA4";
						break;
					case "ALIT":
						newSpell.Name = "ATHN";
						break;
					case "AMUT":
						newSpell.Name = "VOX ";
						break;
					case "FOG ":
						newSpell.Name = "PROT";
						break;
					case "FOG2":
						newSpell.Name = "PRO2";
						break;
					case "FADE":
						newSpell.Name = "HOLY";
						break;
				}

				// Update the entry in the list
				magicSpells[i] = newSpell;
			}

			// Now update the spell names!
			PutSpellNames(magicSpells);
		}

		public void MixUpSpellNames(SpellNameMadness mode, MT19337 rng)
		{
			if (mode == SpellNameMadness.MixedUp)
			{
				string[] spellnames = new string[64];
				Array.Copy(ItemsText.ToList().ToArray(), 176, spellnames, 0, 64);

				var spellnamelist = new List<string>(spellnames);
				spellnamelist.Shuffle(rng);

				for (int i = 0; i < spellnamelist.Count; i++)
				{
					ItemsText[176 + i] = spellnamelist[i];
				}
			}
			else if (mode == SpellNameMadness.Madness)
			{
				List<string> alphabet = new List<string> { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z" };
				List<string> numbers = new List<string> { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9", "0" };

				for (int i = 176; i < 176 + 64; i++)
				{
					ItemsText[i] = alphabet.PickRandom(rng) + alphabet.PickRandom(rng) + numbers.PickRandom(rng) + alphabet.PickRandom(rng);
				}
			}
		}

	}

	public enum SpellNameMadness
	{
		[Description("None")]
		None,

		[Description("MixedUp")]
		MixedUp,

		[Description("Madness")]
		Madness
	}

	public class SpellSlotInfo
	{
		public byte BattleId { get; set; }
		public byte NameId { get; set; }
		public int Level { get; set; }
		public int Slot { get; set; }
		public int MenuId { get; set; }
		public SpellSchools SpellSchool { get; set; }
		public byte PermissionByte { get; set; }

		public SpellSlotInfo(byte _battleId, int _level, int _slot, SpellSchools _spellSchool)
		{
			List<byte> permBytes = new() { 0x80, 0x40, 0x20, 0x10, 0x08, 0x04, 0x02, 0x01 };

			BattleId = _battleId;
			NameId = (byte)(_battleId + 0xB0);
			Level = _level;
			Slot = _slot;
			MenuId = (_slot + 1) + (_spellSchool == SpellSchools.Black ? 4 : 0);
			SpellSchool = _spellSchool;
			//PermissionByte = permBytes[MenuId - 1];
		}

		public SpellSlotInfo()
		{
			List<byte> permBytes = new() { 0x80, 0x40, 0x20, 0x10, 0x08, 0x04, 0x02, 0x01 };

			BattleId = 0x00;
			NameId = 0x00;
			Level = 0;
			Slot = 0;
			MenuId = 0;
			SpellSchool = SpellSchools.White;
			//PermissionByte = permBytes[MenuId - 1];
		}
	}

	public static class SpellSlotStructure
	{
		public static SpellSlotInfo Cure = new SpellSlotInfo(0x00, 1, 0, SpellSchools.White);
		public static SpellSlotInfo Harm = new SpellSlotInfo(0x01, 1, 1, SpellSchools.White);
		public static SpellSlotInfo Fog = new SpellSlotInfo(0x02, 1, 2, SpellSchools.White);
		public static SpellSlotInfo Ruse = new SpellSlotInfo(0x03, 1, 3, SpellSchools.White);
		public static SpellSlotInfo Fire = new SpellSlotInfo(0x04, 1, 0, SpellSchools.Black);
		public static SpellSlotInfo Slep = new SpellSlotInfo(0x05, 1, 1, SpellSchools.Black);
		public static SpellSlotInfo Lock = new SpellSlotInfo(0x06, 1, 2, SpellSchools.Black);
		public static SpellSlotInfo Lit = new SpellSlotInfo(0x07, 1, 3, SpellSchools.Black);
		public static SpellSlotInfo Lamp = new SpellSlotInfo(0x08, 2, 0, SpellSchools.White);
		public static SpellSlotInfo Mute = new SpellSlotInfo(0x09, 2, 1, SpellSchools.White);
		public static SpellSlotInfo Alit = new SpellSlotInfo(0x0A, 2, 2, SpellSchools.White);
		public static SpellSlotInfo Invs = new SpellSlotInfo(0x0B, 2, 3, SpellSchools.White);
		public static SpellSlotInfo Ice = new SpellSlotInfo(0x0C, 2, 0, SpellSchools.Black);
		public static SpellSlotInfo Dark = new SpellSlotInfo(0x0D, 2, 1, SpellSchools.Black);
		public static SpellSlotInfo Tmpr = new SpellSlotInfo(0x0E, 2, 2, SpellSchools.Black);
		public static SpellSlotInfo Slow = new SpellSlotInfo(0x0F, 2, 3, SpellSchools.Black);
		public static SpellSlotInfo Cur2 = new SpellSlotInfo(0x10, 3, 0, SpellSchools.White);
		public static SpellSlotInfo Hrm2 = new SpellSlotInfo(0x11, 3, 1, SpellSchools.White);
		public static SpellSlotInfo Afir = new SpellSlotInfo(0x12, 3, 2, SpellSchools.White);
		public static SpellSlotInfo Heal = new SpellSlotInfo(0x13, 3, 3, SpellSchools.White);
		public static SpellSlotInfo Fir2 = new SpellSlotInfo(0x14, 3, 0, SpellSchools.Black);
		public static SpellSlotInfo Hold = new SpellSlotInfo(0x15, 3, 1, SpellSchools.Black);
		public static SpellSlotInfo Lit2 = new SpellSlotInfo(0x16, 3, 2, SpellSchools.Black);
		public static SpellSlotInfo Lok2 = new SpellSlotInfo(0x17, 3, 3, SpellSchools.Black);
		public static SpellSlotInfo Pure = new SpellSlotInfo(0x18, 4, 0, SpellSchools.White);
		public static SpellSlotInfo Fear = new SpellSlotInfo(0x19, 4, 1, SpellSchools.White);
		public static SpellSlotInfo Aice = new SpellSlotInfo(0x1A, 4, 2, SpellSchools.White);
		public static SpellSlotInfo Amut = new SpellSlotInfo(0x1B, 4, 3, SpellSchools.White);
		public static SpellSlotInfo Slp2 = new SpellSlotInfo(0x1C, 4, 0, SpellSchools.Black);
		public static SpellSlotInfo Fast = new SpellSlotInfo(0x1D, 4, 1, SpellSchools.Black);
		public static SpellSlotInfo Conf = new SpellSlotInfo(0x1E, 4, 2, SpellSchools.Black);
		public static SpellSlotInfo Ice2 = new SpellSlotInfo(0x1F, 4, 3, SpellSchools.Black);
		public static SpellSlotInfo Cur3 = new SpellSlotInfo(0x20, 5, 0, SpellSchools.White);
		public static SpellSlotInfo Life = new SpellSlotInfo(0x21, 5, 1, SpellSchools.White);
		public static SpellSlotInfo Hrm3 = new SpellSlotInfo(0x22, 5, 2, SpellSchools.White);
		public static SpellSlotInfo Hel2 = new SpellSlotInfo(0x23, 5, 3, SpellSchools.White);
		public static SpellSlotInfo Fir3 = new SpellSlotInfo(0x24, 5, 0, SpellSchools.Black);
		public static SpellSlotInfo Bane = new SpellSlotInfo(0x25, 5, 1, SpellSchools.Black);
		public static SpellSlotInfo Warp = new SpellSlotInfo(0x26, 5, 2, SpellSchools.Black);
		public static SpellSlotInfo Slo2 = new SpellSlotInfo(0x27, 5, 3, SpellSchools.Black);
		public static SpellSlotInfo Soft = new SpellSlotInfo(0x28, 6, 0, SpellSchools.White);
		public static SpellSlotInfo Exit = new SpellSlotInfo(0x29, 6, 1, SpellSchools.White);
		public static SpellSlotInfo Fog2 = new SpellSlotInfo(0x2A, 6, 2, SpellSchools.White);
		public static SpellSlotInfo Inv2 = new SpellSlotInfo(0x2B, 6, 3, SpellSchools.White);
		public static SpellSlotInfo Lit3 = new SpellSlotInfo(0x2C, 6, 0, SpellSchools.Black);
		public static SpellSlotInfo Rub = new SpellSlotInfo(0x2D, 6, 1, SpellSchools.Black);
		public static SpellSlotInfo Qake = new SpellSlotInfo(0x2E, 6, 2, SpellSchools.Black);
		public static SpellSlotInfo Stun = new SpellSlotInfo(0x2F, 6, 3, SpellSchools.Black);
		public static SpellSlotInfo Cur4 = new SpellSlotInfo(0x30, 7, 0, SpellSchools.White);
		public static SpellSlotInfo Hrm4 = new SpellSlotInfo(0x31, 7, 1, SpellSchools.White);
		public static SpellSlotInfo Arub = new SpellSlotInfo(0x32, 7, 2, SpellSchools.White);
		public static SpellSlotInfo Hel3 = new SpellSlotInfo(0x33, 7, 3, SpellSchools.White);
		public static SpellSlotInfo Ice3 = new SpellSlotInfo(0x34, 7, 0, SpellSchools.Black);
		public static SpellSlotInfo Brak = new SpellSlotInfo(0x35, 7, 1, SpellSchools.Black);
		public static SpellSlotInfo Sabr = new SpellSlotInfo(0x36, 7, 2, SpellSchools.Black);
		public static SpellSlotInfo Blnd = new SpellSlotInfo(0x37, 7, 3, SpellSchools.Black);
		public static SpellSlotInfo Lif2 = new SpellSlotInfo(0x38, 8, 0, SpellSchools.White);
		public static SpellSlotInfo Fade = new SpellSlotInfo(0x39, 8, 1, SpellSchools.White);
		public static SpellSlotInfo Wall = new SpellSlotInfo(0x3A, 8, 2, SpellSchools.White);
		public static SpellSlotInfo Xfer = new SpellSlotInfo(0x3B, 8, 3, SpellSchools.White);
		public static SpellSlotInfo Nuke = new SpellSlotInfo(0x3C, 8, 0, SpellSchools.Black);
		public static SpellSlotInfo Stop = new SpellSlotInfo(0x3D, 8, 1, SpellSchools.Black);
		public static SpellSlotInfo Zap = new SpellSlotInfo(0x3E, 8, 2, SpellSchools.Black);
		public static SpellSlotInfo XXXX = new SpellSlotInfo(0x3F, 8, 3, SpellSchools.Black);
		public static SpellSlotInfo None = new SpellSlotInfo();

		public static List<SpellSlotInfo> GetSpellSlots()
		{
			var fields = typeof(SpellSlotStructure).GetFields(BindingFlags.Public | BindingFlags.Static);
			return fields.Where(f => f.FieldType == typeof(SpellSlotInfo))
				.Select(f => f.GetValue(null) as SpellSlotInfo)
				.Where(t => t != null)
				.ToList();
		}
	}
}
