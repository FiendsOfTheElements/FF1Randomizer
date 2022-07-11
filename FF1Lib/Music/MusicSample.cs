using FF1Lib.Music;

namespace FF1Lib
{
	public partial class FF1Rom
	{
		public void MusicSample(string music)
		{
			var (track1, track2, track3) = MusicAssembler.AssembleMusic(music);

			var track1Offset = 0;
			var track2Offset = track1Offset + track1.Count;
			var track3Offset = track2Offset + track2.Count;

			Put(0x340C0 + track1Offset, track1.ToArray());
			Put(0x340C0 + track2Offset, track2.ToArray());
			Put(0x340C0 + track3Offset, track3.ToArray());

			track1Offset += 0x80C0;
			track2Offset += 0x80C0;
			track3Offset += 0x80C0;

			Put(0x34000, Blob.FromUShorts(new ushort[] { (ushort)track1Offset, (ushort)track2Offset, (ushort)track3Offset }));
		}
	}
}
