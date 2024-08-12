using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FF1Lib
{
	[DebuggerDisplay("${address.ToString(\"X04\"),nq}")]
	public class FF1Pointer : IComparable<FF1Pointer>
	{
		public ushort address { get; set; }
		public FF1Pointer(ushort address)
		{
			this.address = address;
		}
		public FF1Pointer(byte[] address_le)
		{
			this.address = (ushort)((address_le[1] << 8) + address_le[0]);
		}
		public FF1Pointer(int address)
		{
			if (address < 0 || address > 0xFFFF)
			{
				throw new ArgumentOutOfRangeException();
			}
			this.address = (ushort)address;
		}
		public static implicit operator FF1Pointer(ushort address)
		{
			return new FF1Pointer(address);
		}
		public static implicit operator FF1Pointer(byte[] address_le)
		{
			return new FF1Pointer(address_le);
		}
		public static implicit operator ushort(FF1Pointer value)
		{
			return value.address;
		}
		public string ToHexLittleEndian()
		{
			return $"{(this.address & 0xFF):X02}{(this.address >> 8):X02}";
		}
		public ushort ToShortLittleEndian()
		{
			ushort temp = (ushort)((this.address & 0xFF) << 8);
			temp += (ushort)(this.address >> 8);
			return temp;
		}
		public byte[] ToBytesLittleEndian()
		{
			byte[] temp = new byte[2];
			temp[0] = (byte)(this.address & 0xFF);
			temp[1] = (byte)(this.address >> 8);
			return temp;
		}

		public int CompareTo(FF1Pointer other)
		{
			return this.address.CompareTo(other.address);
		}

		public static FF1Pointer operator +(FF1Pointer left, FF1Pointer right)
		{
			int temp = left.address + right.address;
			if (temp >= 0xFFFF)
			{
				throw new ArgumentOutOfRangeException();
			}
			return new FF1Pointer((ushort)temp);
		}
		public static FF1Pointer operator -(FF1Pointer left, FF1Pointer right)
		{
			int temp = left.address - right.address;
			if (temp < 0)
			{
				throw new ArgumentOutOfRangeException();
			}
			return new FF1Pointer((ushort)temp);
		}
		public static bool operator ==(FF1Pointer left, FF1Pointer right)
		{
			return left.address == right.address;
		}
		public static bool operator !=(FF1Pointer left, FF1Pointer right)
		{
			return left.address != right.address;
		}
		public override bool Equals(Object o)
		{
			if (!(o is FF1Pointer))
				return false;
			return address == ((FF1Pointer)o).address;
		}
		public override int GetHashCode() {return this.address.GetHashCode();}
	}
}
