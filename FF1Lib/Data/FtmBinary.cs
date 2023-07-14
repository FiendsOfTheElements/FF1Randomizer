using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace FF1Lib
{
    /// <summary>
    /// FamiTracker module binary class for parsing and modifying FTMs. Mainly used for rebasing and swapping square channels.
    /// </summary>
    public class FtmBinary
    {
        public class Instrument
        {
            public int Flags = 0;
            public int?[] SeqAddrs = new int?[5];

            public Instrument(BinaryBuffer buffer)
            {
                Flags = buffer.ReadByte();

                byte chanMask = buffer.ReadByte();
                for (int i = 0; i < SeqAddrs.Length; i++)
                {
                    SeqAddrs[i] = (chanMask & 1) != 0 ? buffer.ReadUInt16LE() : null;
                    chanMask >>= 1;
                }
            }
        }

        public class Song
        {
            public int BaseAddr;

            public int FrameListAddr;

            public int PatternLength;
            public int Speed;
            public int Tempo;
            public int GroovePos;
            public int InitialBank;

            public List<int> FrameAddrs;
            public List<int[]> FramePatAddrs;

            public Song(BinaryBuffer buffer, int baseAddr, int numChans)
            {
                BaseAddr = baseAddr;

                FrameListAddr = buffer.ReadUInt16LE();

                int numFrames = buffer.ReadByte();
                PatternLength = buffer.ReadByte();
                Speed = buffer.ReadByte();
                Tempo = buffer.ReadByte();
                GroovePos = buffer.ReadByte();
                InitialBank = buffer.ReadByte();

                buffer.Seek(FrameListAddr - baseAddr);
                FrameAddrs = (from x in buffer.GetUInt16LEEnumerator().Take(numFrames) select (int)x).ToList();

                FramePatAddrs = new List<int[]>();
                foreach (int frameAddr in FrameAddrs)
                {
                    buffer.Seek(frameAddr - baseAddr);
                    FramePatAddrs.Add((from x in buffer.GetUInt16LEEnumerator().Take(numChans) select (int)x).ToArray());
                }
            }
        }

        public int BaseAddr { get; private set; }
        private BinaryBuffer buffer;

        public int SongListAddr { get; private set; }
        public int InstrListAddr { get; private set; }
        public int SampleMapAddr { get; private set; }
        public int SamplesAddr { get; private set; }
        public int GroovesAddr { get; private set; }

        public byte Flags { get; private set; }

        public int NtscSpeed { get; private set; }
        public int PalSpeed { get; private set; }

        private List<int> songAddrs;
        private List<int> instrAddrs;

        private List<Song> songs;
        private List<Instrument> instrs;

        public FtmBinary(IList<Byte> in_Buffer, Int32 in_BaseAddress = 0, Int32 in_NumChannels = 5)
        {
            buffer = new BinaryBuffer(in_Buffer);
            BaseAddr = in_BaseAddress;

            SongListAddr = buffer.ReadUInt16LE();
            InstrListAddr = buffer.ReadUInt16LE();
            SampleMapAddr = buffer.ReadUInt16LE();
            SamplesAddr = buffer.ReadUInt16LE();
            GroovesAddr = buffer.ReadUInt16LE();

            Flags = buffer.ReadByte();

            NtscSpeed = buffer.ReadUInt16LE();
            PalSpeed = buffer.ReadUInt16LE();

            // Parse instruments
            instrAddrs = new List<int>();
            instrs = new List<Instrument>();
            if (InstrListAddr != SampleMapAddr)
            {
                buffer.Seek(InstrListAddr - in_BaseAddress);

                int firstInstrAddr = buffer.ReadUInt16LE(false);
                int numInstrs = (firstInstrAddr - InstrListAddr) / 2;
                instrAddrs = (from x in buffer.GetUInt16LEEnumerator().Take(numInstrs) select (int)x).ToList();

                foreach (int instrAddr in instrAddrs)
                {
                    buffer.Seek(instrAddr - in_BaseAddress);
                    instrs.Add(new Instrument(buffer));
                }
            }

            // Parse songs
            songAddrs = new List<int>();
            songs = new List<Song>();
            if (SongListAddr != InstrListAddr)
            {
                buffer.Seek(SongListAddr - in_BaseAddress);

                int firstSongAddr = buffer.ReadUInt16LE(false);
                int numSongs = (firstSongAddr - SongListAddr) / 2;
                songAddrs = (from x in buffer.GetUInt16LEEnumerator().Take(numSongs) select (int)x).ToList();

                songs = new List<Song>();
                foreach (int songAddr in songAddrs)
                {
                    buffer.Seek(songAddr - in_BaseAddress);
                    songs.Add(new Song(buffer, in_BaseAddress, in_NumChannels));
                }
            }
        }

        /// <summary>
        /// Relocate the FTM to a new base address, fixing up the internal addresses.
        /// </summary>
        /// <param name="in_NewAddress"></param>
        public void Rebase(Int32 in_NewAddress)
        {
            int addrDelta = in_NewAddress - BaseAddr;

            // Update the instruments
            for (int instrIdx = 0; instrIdx < instrAddrs.Count; instrIdx++)
            {
                buffer.Seek(instrAddrs[instrIdx] + 2 - BaseAddr);

                IList<int?> seqAddrs = instrs[instrIdx].SeqAddrs;
                for (int seqIdx = 0; seqIdx < seqAddrs.Count; seqIdx++)
                {
                    int? seqAddr = seqAddrs[seqIdx];
                    if (seqAddr is not null)
                    {
                        seqAddrs[seqIdx] = (seqAddr += addrDelta);
                        buffer.WriteLE((UInt16)seqAddr);
                    }
                }

                instrAddrs[instrIdx] += addrDelta;
            }

            buffer.Seek(InstrListAddr - BaseAddr);
            buffer.WriteLE(from addr in instrAddrs select (UInt16)addr, instrAddrs.Count);

            // Update the songs
            for (int songIdx = 0; songIdx < songAddrs.Count; songIdx++)
            {
                int songAddr = songAddrs[songIdx];
                Song song = songs[songIdx];

                for (int frameIdx = 0; frameIdx < song.FrameAddrs.Count; frameIdx++)
                {
                    int frameAddr = song.FrameAddrs[frameIdx];
                    int[] patAddrs = song.FramePatAddrs[frameIdx];

                    for (int chanIdx = 0; chanIdx < patAddrs.Length; chanIdx++)
                        patAddrs[chanIdx] += addrDelta;

                    buffer.Seek(frameAddr - BaseAddr);
                    buffer.WriteLE(from addr in patAddrs select (UInt16)addr, patAddrs.Length);

                    song.FrameAddrs[frameIdx] = frameAddr + addrDelta;
                }

                buffer.Seek(song.FrameListAddr - BaseAddr);
                buffer.WriteLE(from addr in song.FrameAddrs select (UInt16)addr, song.FrameAddrs.Count);

                song.FrameListAddr += addrDelta;
                buffer.Seek(songAddr - BaseAddr);
                buffer.WriteLE((UInt16)song.FrameListAddr);

                song.BaseAddr += addrDelta;

                songAddrs[songIdx] += addrDelta;
            }

            buffer.Seek(SongListAddr - BaseAddr);
            buffer.WriteLE(from addr in songAddrs select (UInt16)addr, songAddrs.Count);

            // Update the header
            SongListAddr += addrDelta;
            InstrListAddr += addrDelta;
            SampleMapAddr += addrDelta;
            SamplesAddr += addrDelta;
            GroovesAddr += addrDelta;

            buffer.Seek(0);

            buffer.WriteLE((UInt16)SongListAddr);
            buffer.WriteLE((UInt16)InstrListAddr);
            buffer.WriteLE((UInt16)SampleMapAddr);
            buffer.WriteLE((UInt16)SamplesAddr);
            buffer.WriteLE((UInt16)GroovesAddr);

            BaseAddr = in_NewAddress;
        }

        /// <summary>
        /// Swap the channel data for square 1 and square 2.
        /// </summary>
        /// <param name="songIdx"></param>
        public void SwapSquareChans(int songIdx)
        {
            Song song = songs[songIdx];
            HashSet<int> doneFrameAddrs = new();

            for (int frameIdx = 0; frameIdx < song.FrameAddrs.Count; frameIdx++)
            {
                int frameAddr = song.FrameAddrs[frameIdx];
                if (!doneFrameAddrs.Add(frameAddr))
                    continue;

                int[] frame = song.FramePatAddrs[frameIdx];
                int square1Addr = frame[0];
                frame[0] = frame[1];
                frame[1] = square1Addr;

                buffer.Seek(frameAddr - BaseAddr);
                buffer.WriteLE(from addr in frame select (UInt16)addr, 2);
            }
        }
    }
}
