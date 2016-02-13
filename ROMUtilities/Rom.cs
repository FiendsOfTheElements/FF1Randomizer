using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ROMUtilities
{
	public abstract class Rom
	{
		protected Blob Data;
		protected Blob Header;

		public byte this[int index]
		{
			get { return Data[index]; }
			set { Data[index] = value; }
		}

		public abstract void Load(string filename);

		public void Save(string filename)
		{
			using (var fs = new FileStream(filename, FileMode.Create, FileAccess.Write, FileShare.None))
			{
				fs.Write(Header, 0, Header.Length);
				fs.Write(Data, 0, Data.Length);
			}
		}

		public Blob Get(int index, int length)
		{
			return Data.SubBlob(index, length);
		}

		public void Put(int index, Blob data)
		{
			Array.Copy(data, Data, data.Length);
		}
	}
}
