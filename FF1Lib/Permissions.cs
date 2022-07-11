namespace FF1Lib
{
	public class GearPermissions
	{
		public const int GearPermissionsCount = 40;

		private List<(EquipPermission, Item)> _permissions;
		private int _offset;
		private int _itemoffset;

		private List<(Classes, EquipPermission)> ClassEquipConverter = new()
		{
			(Classes.Fighter, EquipPermission.Fighter),
			(Classes.Thief, EquipPermission.Thief),
			(Classes.BlackBelt, EquipPermission.BlackBelt),
			(Classes.RedMage, EquipPermission.RedMage),
			(Classes.WhiteMage, EquipPermission.WhiteMage),
			(Classes.BlackMage, EquipPermission.BlackMage),
			(Classes.Knight, EquipPermission.Knight),
			(Classes.Ninja, EquipPermission.Ninja),
			(Classes.Master, EquipPermission.Master),
			(Classes.RedWizard, EquipPermission.RedWizard),
			(Classes.WhiteWizard, EquipPermission.WhiteWizard),
			(Classes.BlackWizard, EquipPermission.BlackWizard),
		};
		private EquipPermission GetEquipPermission(Classes targetclass) => ClassEquipConverter.Find(y => y.Item1 == targetclass).Item2;
		public GearPermissions(int offset, int itemoffset, FF1Rom rom)
		{
			_offset = offset;
			_itemoffset = itemoffset;

			var rawPermissions = rom.Get(offset, GearPermissionsCount * 2).Chunk(2);

			_permissions = new List<(EquipPermission, Item)>();

			for (int i = 0; i < GearPermissionsCount; i++)
			{
				_permissions.AddRange(Enum.GetValues<EquipPermission>()
					.Where(x => (rawPermissions[i].ToUShorts()[0] & (ushort)x) == 0)
					.Select(x => (x, (Item)(i + _itemoffset)))
					.ToList());
			}
		}
		public void Write(FF1Rom rom)
		{
			for (int i = 0; i < GearPermissionsCount; i++) _permissions.Add((EquipPermission.None, (Item)(_itemoffset + i)));

			rom.Put(_offset, Blob.FromUShorts(_permissions
				.GroupBy(x => x.Item2, x => x.Item1)
				.Select(x => (x.Key, (ushort)((int)x.Aggregate((a, b) => a | b) ^ 0xFFF)))
				.OrderBy(x => x.Key)
				.Select(x => x.Item2)
				.ToArray()));
		}
		public void AddPermission(Classes targetclass, Item targetitem)
		{
			if (!_permissions.Contains((GetEquipPermission(targetclass), targetitem)))
			{
				_permissions.Add((GetEquipPermission(targetclass), targetitem));
			}
		}
		public void AddPermissionsRange(List<(Classes, Item)> gearpermissions)
		{
			if (gearpermissions != null)
			{
				var convertedpermissions = gearpermissions.Select(x => (GetEquipPermission(x.Item1), x.Item2));

				_permissions.AddRange(convertedpermissions.Where(x => !_permissions.Contains(x)).ToList());
			}
		}
		public void RemovePermission(Classes targetclass, Item targetitem)
		{
			if (_permissions.Contains((GetEquipPermission(targetclass), targetitem)))
			{
				_permissions.Remove((GetEquipPermission(targetclass), targetitem));
			}
		}
		public void RemovePermissionsRange(List<(Classes, Item)> gearpermissions)
		{
			var convertedpermissions = gearpermissions.Select(x => (GetEquipPermission(x.Item1), x.Item2));

			_permissions.RemoveAll(x => convertedpermissions.Contains(x));
		}
		public void ClearPermissions(Classes targetclass)
		{
			_permissions.RemoveAll(x => x.Item1 == GetEquipPermission(targetclass));
		}
		public List<Item> this[Classes index]
		{
			get {
				return _permissions
						.Where(x => x.Item1 == GetEquipPermission(index))
						.Select(x => x.Item2)
						.ToList();
			}

			set {
				if (value != null)
				{
					_permissions.RemoveAll(x => x.Item1 == GetEquipPermission(index));
					_permissions.AddRange(value.Select(x => (GetEquipPermission(index), x)).ToList());
				}
			}
		}
		public List<Item> this[EquipPermission index]
		{
			get
			{
				return _permissions
						.Where(x => x.Item1 == index)
						.Select(x => x.Item2)
						.ToList();
			}

			set
			{
				if (value != null)
				{
					_permissions.RemoveAll(x => x.Item1 == index);
					_permissions.AddRange(value.Select(x => (index, x)).ToList());
				}
			}
		}
		/*
		public List<EquipPermission> this[Item index]
		{
			get
			{
				return _permissions
						.Where(x => x.Item2 == (Item)((int)index - _itemoffset))
						.Select(x => x.Item1)
						.ToList();
			}

			set
			{
				_permissions.RemoveAll(x => x.Item2 == (Item)((int)index - _itemoffset));
				_permissions.AddRange(value.Select(x => (x, (Item)((int)index - _itemoffset))));
			}
		}
		*/
		public ushort this[Item index]
		{
			get
			{
				return (ushort)_permissions
						.Where(x => x.Item2 == index)
						.Select(x => x.Item1)
						.DefaultIfEmpty()
						.Aggregate((a, b) => a | b);
			}

			set
			{
				if (value != 0x0000)
				{
					_permissions.RemoveAll(x => x.Item2 == index);
					_permissions.AddRange(Enum.GetValues<EquipPermission>()
						.Where(x => (value & (ushort)x) > 0)
						.Select(x => (x, index))
						.ToList());
				}
			}
		}
	}

	public class SpellPermissions
	{
		public const int MagicPermissionsOffset = 0x3AD18;
		public const int MagicPermissionsSize = 8;
		public const int MagicPermissionsCount = 12;

		private List<(Classes, SpellSlots)> _permissions;

		private List<(SpellSlots, int, byte)> SpellSlotsPositions = new()
		{
			(SpellSlots.White1Slot1, 0, 0x80),
			(SpellSlots.White1Slot2, 0, 0x40),
			(SpellSlots.White1Slot3, 0, 0x20),
			(SpellSlots.White1Slot4, 0, 0x10),
			(SpellSlots.Black1Slot1, 0, 0x08),
			(SpellSlots.Black1Slot2, 0, 0x04),
			(SpellSlots.Black1Slot3, 0, 0x02),
			(SpellSlots.Black1Slot4, 0, 0x01),
			(SpellSlots.White2Slot1, 1, 0x80),
			(SpellSlots.White2Slot2, 1, 0x40),
			(SpellSlots.White2Slot3, 1, 0x20),
			(SpellSlots.White2Slot4, 1, 0x10),
			(SpellSlots.Black2Slot1, 1, 0x08),
			(SpellSlots.Black2Slot2, 1, 0x04),
			(SpellSlots.Black2Slot3, 1, 0x02),
			(SpellSlots.Black2Slot4, 1, 0x01),
			(SpellSlots.White3Slot1, 2, 0x80),
			(SpellSlots.White3Slot2, 2, 0x40),
			(SpellSlots.White3Slot3, 2, 0x20),
			(SpellSlots.White3Slot4, 2, 0x10),
			(SpellSlots.Black3Slot1, 2, 0x08),
			(SpellSlots.Black3Slot2, 2, 0x04),
			(SpellSlots.Black3Slot3, 2, 0x02),
			(SpellSlots.Black3Slot4, 2, 0x01),
			(SpellSlots.White4Slot1, 3, 0x80),
			(SpellSlots.White4Slot2, 3, 0x40),
			(SpellSlots.White4Slot3, 3, 0x20),
			(SpellSlots.White4Slot4, 3, 0x10),
			(SpellSlots.Black4Slot1, 3, 0x08),
			(SpellSlots.Black4Slot2, 3, 0x04),
			(SpellSlots.Black4Slot3, 3, 0x02),
			(SpellSlots.Black4Slot4, 3, 0x01),
			(SpellSlots.White5Slot1, 4, 0x80),
			(SpellSlots.White5Slot2, 4, 0x40),
			(SpellSlots.White5Slot3, 4, 0x20),
			(SpellSlots.White5Slot4, 4, 0x10),
			(SpellSlots.Black5Slot1, 4, 0x08),
			(SpellSlots.Black5Slot2, 4, 0x04),
			(SpellSlots.Black5Slot3, 4, 0x02),
			(SpellSlots.Black5Slot4, 4, 0x01),
			(SpellSlots.White6Slot1, 5, 0x80),
			(SpellSlots.White6Slot2, 5, 0x40),
			(SpellSlots.White6Slot3, 5, 0x20),
			(SpellSlots.White6Slot4, 5, 0x10),
			(SpellSlots.Black6Slot1, 5, 0x08),
			(SpellSlots.Black6Slot2, 5, 0x04),
			(SpellSlots.Black6Slot3, 5, 0x02),
			(SpellSlots.Black6Slot4, 5, 0x01),
			(SpellSlots.White7Slot1, 6, 0x80),
			(SpellSlots.White7Slot2, 6, 0x40),
			(SpellSlots.White7Slot3, 6, 0x20),
			(SpellSlots.White7Slot4, 6, 0x10),
			(SpellSlots.Black7Slot1, 6, 0x08),
			(SpellSlots.Black7Slot2, 6, 0x04),
			(SpellSlots.Black7Slot3, 6, 0x02),
			(SpellSlots.Black7Slot4, 6, 0x01),
			(SpellSlots.White8Slot1, 7, 0x80),
			(SpellSlots.White8Slot2, 7, 0x40),
			(SpellSlots.White8Slot3, 7, 0x20),
			(SpellSlots.White8Slot4, 7, 0x10),
			(SpellSlots.Black8Slot1, 7, 0x08),
			(SpellSlots.Black8Slot2, 7, 0x04),
			(SpellSlots.Black8Slot3, 7, 0x02),
			(SpellSlots.Black8Slot4, 7, 0x01),
			(SpellSlots.None, 8, 0x00),
		};

		public SpellPermissions(FF1Rom rom)
		{
			var rawPermissions = rom.Get(MagicPermissionsOffset, MagicPermissionsCount * MagicPermissionsSize).Chunk(8);

			_permissions = new List<(Classes, SpellSlots)>();

			for (int i = 0; i < MagicPermissionsCount; i++)
			{
				for (int j = 0; j < MagicPermissionsSize; j++)
				{
					_permissions.AddRange(SpellSlotsPositions
						.Where(x => (rawPermissions[i][j] & x.Item3) == 0 && j == x.Item2)
						.Select(x => ((Classes)i, x.Item1))
						.ToList());
				}
			}
		}
		public void Write(FF1Rom rom)
		{
			_permissions.AddRange(Enum.GetValues<Classes>().Select(x => (x, SpellSlots.None)).ToList());

			rom.Put(MagicPermissionsOffset, _permissions
				.GroupBy(x => x.Item1, x => x.Item2)
				.Select(x => (x.Key, new byte[] {
					(byte)(SpellSlotsPositions.Where(y => x.Contains(y.Item1) && y.Item2 == 0).Select(y => y.Item3).DefaultIfEmpty().Aggregate((a, b) => (byte)(a | b) ) ^ 0xFF),
					(byte)(SpellSlotsPositions.Where(y => x.Contains(y.Item1) && y.Item2 == 1).Select(y => y.Item3).DefaultIfEmpty().Aggregate((a, b) => (byte)(a | b) ) ^ 0xFF),
					(byte)(SpellSlotsPositions.Where(y => x.Contains(y.Item1) && y.Item2 == 2).Select(y => y.Item3).DefaultIfEmpty().Aggregate((a, b) => (byte)(a | b) ) ^ 0xFF),
					(byte)(SpellSlotsPositions.Where(y => x.Contains(y.Item1) && y.Item2 == 3).Select(y => y.Item3).DefaultIfEmpty().Aggregate((a, b) => (byte)(a | b) ) ^ 0xFF),
					(byte)(SpellSlotsPositions.Where(y => x.Contains(y.Item1) && y.Item2 == 4).Select(y => y.Item3).DefaultIfEmpty().Aggregate((a, b) => (byte)(a | b) ) ^ 0xFF),
					(byte)(SpellSlotsPositions.Where(y => x.Contains(y.Item1) && y.Item2 == 5).Select(y => y.Item3).DefaultIfEmpty().Aggregate((a, b) => (byte)(a | b) ) ^ 0xFF),
					(byte)(SpellSlotsPositions.Where(y => x.Contains(y.Item1) && y.Item2 == 6).Select(y => y.Item3).DefaultIfEmpty().Aggregate((a, b) => (byte)(a | b) ) ^ 0xFF),
					(byte)(SpellSlotsPositions.Where(y => x.Contains(y.Item1) && y.Item2 == 7).Select(y => y.Item3).DefaultIfEmpty().Aggregate((a, b) => (byte)(a | b) ) ^ 0xFF)	}))
				.OrderBy(x => x.Key)
				.SelectMany(x => x.Item2)
				.ToArray());
		}
		public void ImportRawPermissions(Blob rawpermissions)
		{
			var parsedrawpermissions = rawpermissions.Chunk(8);

			_permissions = new List<(Classes, SpellSlots)>();

			for (int i = 0; i < MagicPermissionsCount; i++)
			{
				for (int j = 0; j < MagicPermissionsSize; j++)
				{
					_permissions.AddRange(SpellSlotsPositions
						.Where(x => (parsedrawpermissions[i][j] & x.Item3) == 0 && j == x.Item2)
						.Select(x => ((Classes)i, x.Item1))
						.ToList());
				}
			}
		}
		public void AddPermission(Classes targetclass, SpellSlots targetspellslots)
		{
			if (!_permissions.Contains((targetclass, targetspellslots)))
			{
				_permissions.Add((targetclass, targetspellslots));
			}
		}
		public void AddPermissionsRange(List<(Classes, SpellSlots)> spellslotspermissions)
		{
			if (spellslotspermissions != null)
			{
				_permissions.AddRange(spellslotspermissions.Where(x => !_permissions.Contains(x)).ToList());
			}
		}
		public bool RemovePermission(Classes targetclass, SpellSlots targetspellslots)
		{
			if (_permissions.Contains((targetclass, targetspellslots)))
			{
				_permissions.Remove((targetclass, targetspellslots));
				return true;
			}
			else
			{
				return false;
			}
		}
		public void RemovePermissionsRange(List<(Classes, SpellSlots)> spellslotspermissions)
		{
			_permissions.RemoveAll(x => spellslotspermissions.Contains(x));
		}
		public void ClearPermissions(Classes targetclass)
		{
			_permissions.RemoveAll(x => x.Item1 == targetclass);
		}
		public List<(Classes, List<SpellSlots>)> GetBlackPermissions()
		{
			return _permissions
				.GroupBy(x => x.Item1, x => x.Item2)
				.Select(x => (x.Key, SpellSlotsPositions.Where(y => x.Contains(y.Item1) && y.Item3 <= 0x08).Select(y => y.Item1).ToList()))
				.ToList();
		}
		public List<(Classes, List<SpellSlots>)> GetWhitePermissions()
		{
			return _permissions
				.GroupBy(x => x.Item1, x => x.Item2)
				.Select(x => (x.Key, SpellSlotsPositions.Where(y => x.Contains(y.Item1) && y.Item3 >= 0x10).Select(y => y.Item1).ToList()))
				.ToList();
		}
		public List<SpellSlots> this[Classes index]
		{
			get
			{
				return _permissions
						.Where(x => x.Item1 == index)
						.Select(x => x.Item2)
						.ToList();
			}

			set
			{
				if (value != null)
				{
					_permissions.RemoveAll(x => x.Item1 == index);
					_permissions.AddRange(value.Select(x => (index, x)).ToList());
				}
			}
		}
	    public List<Classes> PermissionsFor(SpellSlots slot)
	    {
		return _permissions
		    .Where(x => x.Item2 == slot)
		    .Select(x => x.Item1)
		    .ToList();
	    }
	}

}
