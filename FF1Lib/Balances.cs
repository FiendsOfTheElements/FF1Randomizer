using System.ComponentModel;

namespace FF1Lib
{
	public partial class FF1Rom
	{
		public void DoubleWeaponCritRates()
		{
			// Increase crit rate of all weapons
			var weapons = Get(WeaponOffset, WeaponSize * WeaponCount).Chunk(WeaponSize);
			foreach (var weapon in weapons)
			{
				weapon[2] *= 2;
			}
			Put(WeaponOffset, weapons.SelectMany(weapon => weapon.ToBytes()).ToArray());
		}

		public void IncreaseWeaponBonus(int weaponBonusValue)
		{
			//change the weapon bonus from +4 to +X
			Put(0x326F5, new byte[] { (byte) weaponBonusValue });
		}
	}
}
