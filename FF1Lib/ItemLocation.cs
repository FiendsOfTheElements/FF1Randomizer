using System.Diagnostics.CodeAnalysis;

namespace FF1Lib
{
	[Flags]
	public enum AccessRequirement
	{
		None = 0x0000,
		Key = 0x0001,
		Rod = 0x0002,
		Oxyale = 0x0004,
		Cube = 0x0008,
		Bottle = 0x0010,
		Lute = 0x0020,
		Crown = 0x0040,
		Crystal = 0x0080,
		Herb = 0x0100,
		Tnt = 0x0200,
		Adamant = 0x0400,
		Slab = 0x0800,
		Ruby = 0x1000,
		EarthOrb = 0x10000,
		FireOrb = 0x20000,
		WaterOrb = 0x40000,
		AirOrb = 0x80000,
		BlackOrb = 0xF0000,
		AllExceptEnding = 0x00001FFF,
		All = 0x000F1FFF
	}
	public interface IRewardSource
	{
		int Address { get; }
		string Name { get; }
		MapLocation MapLocation { get; }
		Item Item { get; }
		AccessRequirement AccessRequirement { get; }
		OverworldTeleportIndex Entrance { get; }
		bool IsUnused { get; }
		string SpoilerText { get; }

		void Put(FF1Rom rom);
	}
	public abstract class RewardSourceBase : IRewardSource
	{
		public int Address { get; protected set; }
		public string Name { get; protected set; }
		public MapLocation MapLocation { get; protected set; }
		public Item Item { get; protected set; }
		public AccessRequirement AccessRequirement { get; set; }
		public OverworldTeleportIndex Entrance { get; set; }
		public bool IsUnused { get; protected set; }

		public virtual bool IsTreasure => false;
		public string SpoilerText =>
		$"{Name}{string.Join("", Enumerable.Repeat(" ", Math.Max(1, 30 - Name.Length)).ToList())}" +
		$"\t{Enum.GetName(typeof(Item), Item)}";

		protected RewardSourceBase(int address, string name, MapLocation mapLocation, Item item,
							AccessRequirement accessRequirement = AccessRequirement.None,
							bool isUnused = false)
		{
			Address = address;
			Name = name;
			Item = item;
			MapLocation = mapLocation;
			AccessRequirement = accessRequirement;
			IsUnused = isUnused;
		}
		protected RewardSourceBase(IRewardSource copyFromRewardSource, Item item)
		{
			Address = copyFromRewardSource.Address;
			Name = copyFromRewardSource.Name;
			Item = item;
			MapLocation = copyFromRewardSource.MapLocation;
			AccessRequirement = copyFromRewardSource.AccessRequirement;
			Entrance = copyFromRewardSource.Entrance;
			IsUnused = false;
		}
		protected RewardSourceBase(IRewardSource copyFromRewardSource, OverworldTeleportIndex entrance)
		{
			Address = copyFromRewardSource.Address;
			Name = copyFromRewardSource.Name;
			Item = copyFromRewardSource.Item;
			MapLocation = copyFromRewardSource.MapLocation;
			AccessRequirement = copyFromRewardSource.AccessRequirement;
			Entrance = entrance;
			IsUnused = false;
		}
		public override int GetHashCode() => Address.GetHashCode();

		public virtual void Put(FF1Rom rom) => rom.Put(Address, new[] { (byte)Item });
	}

	public class TreasureChest : RewardSourceBase
	{
		private const int _treasureChestBaseAddress = 0x3100;
		public override bool IsTreasure => true;
		public TreasureChest(int address, string name, MapLocation mapLocation, Item item,
							 AccessRequirement accessRequirement = AccessRequirement.None,
							 bool isUnused = false)
			: base(address, name, mapLocation, item, accessRequirement, isUnused) { }

		public TreasureChest(IRewardSource copyFromRewardSource, Item item)
			: base(copyFromRewardSource, item) { }

		public TreasureChest(IRewardSource copyFromRewardSource, OverworldTeleportIndex entrance)
			: base(copyFromRewardSource, entrance) { }

		public TreasureChest(IRewardSource copyFromRewardSource, Item item, AccessRequirement access)
			: base(copyFromRewardSource, item)
		{
			AccessRequirement = access;
		}

		public void ChangeMapLocation(MapLocation mapLocation) {
		    MapLocation = mapLocation;
		}

	}
	public class NpcReward : RewardSourceBase
	{
		//private const int _mapObjectTalkDataAddress = 0x395D5;
		private const int _mapObjectTalkDataAddress = 0x47A00;
		private const int _mapObjectTalkDataSize = 6;
		private const int _giftItemIndex = 3;

		private const int _mapObjTalkJumpTblAddress = 0x450D3;
		private const int _mapObjTalkJumpTblDataSize = 2;
		private readonly Blob _eventFlagRoutineAddress = Blob.FromHex("0393");
		private readonly Blob _itemTradeRoutineAddress = Blob.FromHex("7893");
		private readonly Blob _itemCheckRoutineAddress = Blob.FromHex("A193");

		private readonly int _objectRoutineAddress;
		private readonly ObjectId _requiredGameEventFlag;
		private readonly Item _requiredItemTrade;
		private readonly bool _useVanillaRoutineAddress;

		public ObjectId ObjectId { get; private set; }

		public MapLocation SecondLocation { get; protected set; } = MapLocation.StartingLocation;

		public NpcReward(ObjectId objectId, MapLocation mapLocation, Item item,
						 AccessRequirement accessRequirement = AccessRequirement.None,
						 ObjectId requiredGameEventFlag = ObjectId.None,
						 Item requiredItemTrade = Item.None,
						 MapLocation requiredSecondLocation = MapLocation.StartingLocation,
						bool useVanillaRoutineAddress = true)
			: base(_mapObjectTalkDataAddress + _giftItemIndex +
				   _mapObjectTalkDataSize * (byte)objectId,
				   Enum.GetName(typeof(ObjectId), objectId),
				   mapLocation,
				   item,
				   accessRequirement)
		{
			ObjectId = objectId;

			_objectRoutineAddress = (byte)objectId * _mapObjTalkJumpTblDataSize + _mapObjTalkJumpTblAddress;
			_requiredGameEventFlag = requiredGameEventFlag;
			_requiredItemTrade = requiredItemTrade;
			_useVanillaRoutineAddress = useVanillaRoutineAddress;
			SecondLocation = requiredSecondLocation;
			if (_requiredGameEventFlag != ObjectId.None && _requiredItemTrade != Item.None)
				throw new InvalidOperationException(
					$"Attempted to Put invalid npc item placement: \n{SpoilerText}");
		}
		public NpcReward(IRewardSource copyFromRewardSource, OverworldTeleportIndex entrance)
			: base(copyFromRewardSource, entrance) { }

		public NpcReward(IRewardSource copyFromRewardSource, Item item)
			: base(copyFromRewardSource, item)
		{
			if (!(copyFromRewardSource is NpcReward copyFromMapObject))
				return;


			ObjectId = copyFromMapObject.ObjectId;

			_objectRoutineAddress = copyFromMapObject._objectRoutineAddress;
			_requiredGameEventFlag = copyFromMapObject._requiredGameEventFlag;
			_requiredItemTrade = copyFromMapObject._requiredItemTrade;
			_useVanillaRoutineAddress = copyFromMapObject._useVanillaRoutineAddress;
			SecondLocation = copyFromMapObject.SecondLocation;
			if (_requiredGameEventFlag != ObjectId.None && _requiredItemTrade != Item.None)
				throw new InvalidOperationException(
					$"Attempted to Put invalid npc item placement: \n{SpoilerText}");
		}

		public override void Put(FF1Rom rom)
		{
			if (_useVanillaRoutineAddress)
			{
				base.Put(rom);
				return;
			}

			if (_requiredItemTrade != Item.None)
			{
				rom.Put(_objectRoutineAddress, _itemTradeRoutineAddress);
				rom.Put(Address - _giftItemIndex, new[] { (byte)_requiredItemTrade });
			}
			else
			{
				rom.Put(_objectRoutineAddress, _eventFlagRoutineAddress);
				rom.Put(Address - _giftItemIndex, new[] { (byte)_requiredGameEventFlag });
			}

			base.Put(rom);
		}
	}

	public class ItemShopSlot : RewardSourceBase
	{
		public byte ShopIndex { get; private set; }

		public ItemShopSlot(int address, string name, MapLocation mapLocation, Item item, byte shopIndex)
			: base(address, name, mapLocation, item)
		{
			ShopIndex = shopIndex;
		}

		public ItemShopSlot(ItemShopSlot copyFromRewardSource, Item item)
			: base(copyFromRewardSource, item)
		{
			ShopIndex = copyFromRewardSource.ShopIndex;
		}

		public ItemShopSlot(ItemShopSlot copyFromRewardSource, OverworldTeleportIndex entrance)
			: base(copyFromRewardSource, entrance)
		{
			ShopIndex = copyFromRewardSource.ShopIndex;
		}

		public override void Put(FF1Rom rom)
		{
			if (Item > Item.Soft)
				throw new InvalidOperationException(
					$"Attempted to Put invalid item shop placement: \n{SpoilerText}");
			base.Put(rom);
		}
	}

	public class StaticItemLocation : RewardSourceBase
	{
		private static int address = 0x80000;
		public StaticItemLocation(string name, MapLocation mapLocation, Item item,
							 AccessRequirement accessRequirement = AccessRequirement.None)
			: base(address++, name, mapLocation, item, accessRequirement)
		{
			if (address >= int.MaxValue)
				address = 0x80000;
		}
		public override void Put(FF1Rom rom) => throw new NotImplementedException();
	}

	public class RewardSourceEqualityComparer : IEqualityComparer<IRewardSource>
	{
		public bool Equals(IRewardSource x, IRewardSource y)
		{
			return x.Address == y.Address;
		}

		public int GetHashCode([DisallowNull] IRewardSource obj)
		{
			return obj.Address;
		}
	}
}
