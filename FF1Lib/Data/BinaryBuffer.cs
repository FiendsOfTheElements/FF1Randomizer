using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FF1Lib
{
    using Buffer = IList<byte>;

    /// <summary>
    /// Buffer for binary data that can read and write individual and series of 8-bit and 16-bit values with automatic endian conversion.
    /// </summary>
    public class BinaryBuffer
    {
        /// <summary>
        /// Interface for encoding 16-bit values with different endians. Made an interface rather than a delegate or parameter in the hope of generating optimized inlined loops.
        /// </summary>
        private interface IEndianType16
        {
            UInt16 Read(Buffer in_Buffer, ref int Pos);
            void Write(Buffer in_Buffer, ref int Pos, UInt16 in_Value);
        }

        private struct BigEndian16 : IEndianType16
        {
            public UInt16 Read(Buffer in_Buffer, ref int Pos)
            {
                byte msb = in_Buffer[Pos++];
                return (UInt16)(((UInt16)msb << 8) | in_Buffer[Pos++]);
            }

            public void Write(Buffer in_Buffer, ref int Pos, UInt16 in_Value)
            {
                in_Buffer[Pos++] = (byte)(in_Value >> 8);
                in_Buffer[Pos++] = (byte)in_Value;
            }
        }

        private struct LittleEndian16 : IEndianType16
        {
            public UInt16 Read(Buffer in_Buffer, ref int Pos)
            {
                byte lsb = in_Buffer[Pos++];
                return (UInt16)(((UInt16)in_Buffer[Pos++] << 8) | lsb);
            }

            public void Write(Buffer in_Buffer, ref int Pos, UInt16 in_Value)
            {
                in_Buffer[Pos++] = (byte)in_Value;
                in_Buffer[Pos++] = (byte)(in_Value >> 8);
            }
        }

        private Buffer buffer;
        private int position = 0;

        /// <summary>
        /// Verify that there is enough space for a read.
        /// </summary>
        /// <param name="in_BufferLength"></param>
        /// <param name="in_Index"></param>
        /// <param name="in_Count"></param>
        private void CheckReadSize(int in_BufferLength, int in_Index, int in_Count)
        {
            Debug.Assert(in_BufferLength >= 0);

            if (in_Index < 0 || in_Count < 0)
                throw new ArgumentOutOfRangeException();
            else if (checked(in_Index + in_Count) > in_BufferLength)
                throw new EndOfStreamException();
        }

        /// <summary>
        /// Verify that there's enough space for a write from an enumerable.
        /// </summary>
        /// <param name="in_ElementSize"></param>
        /// <param name="in_Count"></param>
        private void CheckWriteSize(int in_ElementSize, int? in_Count)
        {
            Debug.Assert(in_ElementSize > 0);

            if (in_Count is null)
                return;

            if (in_Count < 0)
                throw new ArgumentOutOfRangeException();
            else if (checked(position + in_Count * in_ElementSize) > buffer.Count)
                throw new OverflowException();
        }

        /// <summary>
        /// Verify that there's enough space for a write from a list-like buffer.
        /// </summary>
        /// <param name="in_ElementSize"></param>
        /// <param name="in_BufferLength"></param>
        /// <param name="in_Index"></param>
        /// <param name="in_Count"></param>
        private void CheckWriteSize(int in_ElementSize, int in_BufferLength, int in_Index, int in_Count)
        {
            Debug.Assert(in_ElementSize > 0);
            Debug.Assert(in_BufferLength >= 0);

            if (in_Index < 0 || in_Count < 0)
                throw new ArgumentOutOfRangeException();
            else if (checked(in_Index + in_Count) > in_BufferLength)
                throw new ArgumentException();
            else if (checked(position + in_Count * in_ElementSize) > buffer.Count)
                throw new OverflowException();
        }

        /// <summary>
        /// Read a single UInt16 from the current position.
        /// </summary>
        /// <typeparam name="Encoding"></typeparam>
        /// <param name="in_Advance"></param>
        /// <returns></returns>
        private UInt16 InternalRead<Encoding>(bool in_Advance = true)
            where Encoding : IEndianType16, new()
        {
            int pos = position;
            Encoding enc = new();
            ushort value = enc.Read(buffer, ref pos);
            if (in_Advance)
                position = pos;

            return value;
        }

        /// <summary>
        /// Read multiple UInt16s from the current position.
        /// </summary>
        /// <typeparam name="Encoding"></typeparam>
        /// <param name="out_Buffer"></param>
        /// <param name="in_Index"></param>
        /// <param name="in_Count"></param>
        /// <param name="in_Advance"></param>
        /// <returns>The number of UInt16s read.</returns>
        private int InternalRead<Encoding>(IList<UInt16> out_Buffer, int in_Index = 0, int? in_Count = null, bool in_Advance = true)
            where Encoding : IEndianType16, new()
        {
            if (in_Count is null)
                in_Count = out_Buffer.Count;

            CheckReadSize(out_Buffer.Count, in_Index, (int)in_Count);

            int pos = position;
            Encoding enc = new();
            for (int i = in_Index; i < in_Index + in_Count; i++)
                out_Buffer[i] = enc.Read(buffer, ref pos);

            if (in_Advance)
                position = pos;

            return (int)in_Count;
        }

        /// <summary>
        /// Write a single UInt16 to the current position.
        /// </summary>
        /// <typeparam name="Encoding"></typeparam>
        /// <param name="in_Value"></param>
        /// <param name="in_Advance"></param>
        private void InternalWrite<Encoding>(UInt16 in_Value, bool in_Advance = true)
             where Encoding : IEndianType16, new()
        {
            if (checked(position + 2) > buffer.Count)
                throw new OverflowException();

            int pos = position;
            Encoding enc = new();
            enc.Write(buffer, ref pos, in_Value);

            if (in_Advance)
                position = pos;
        }

        /// <summary>
        /// Write multiple UInt16s from an enumerator to the current position.
        /// </summary>
        /// <typeparam name="Encoding"></typeparam>
        /// <param name="in_Iter"></param>
        /// <param name="in_Count"></param>
        /// <param name="in_Advance"></param>
        /// <returns>The number of UInt16s written.</returns>
        private int InternalWrite<Encoding>(IEnumerator<UInt16> in_Iter, int? in_Count = null, bool in_Advance = true)
             where Encoding : IEndianType16, new()
        {
            CheckWriteSize(1, in_Count);

            int pos = position;
            Encoding enc = new();
            if (in_Count is not null)
            {
                int endPos = position + (int)in_Count * 2;
                while (pos < endPos && in_Iter.MoveNext())
                    enc.Write(buffer, ref pos, in_Iter.Current);
            }
            else
            {
                while (in_Iter.MoveNext())
                    enc.Write(buffer, ref pos, in_Iter.Current);
            }

            int count = (pos - position) / 2;
            if (in_Advance)
                position = pos;

            return count;
        }

        /// <summary>
        /// Write multiple UInt16s from a list-like buffer to the current position.
        /// </summary>
        /// <typeparam name="Encoding"></typeparam>
        /// <param name="in_Buffer"></param>
        /// <param name="in_Index"></param>
        /// <param name="in_Count"></param>
        /// <param name="in_Advance"></param>
        /// <returns>The number of UInt16s written.</returns>
        private int InternalWrite<Encoding>(IReadOnlyList<UInt16> in_Buffer, int in_Index = 0, int? in_Count = null, bool in_Advance = true)
             where Encoding : IEndianType16, new()
        {
            if (in_Count is null)
                in_Count = in_Buffer.Count - in_Index;

            CheckWriteSize(2, in_Buffer.Count, in_Index, (int)in_Count);

            int pos = position;
            Encoding enc = new();
            for (int inPos = in_Index; inPos < in_Index + in_Count; inPos++)
                enc.Write(buffer, ref pos, in_Buffer[inPos]);

            if (in_Advance)
                position = pos;

            return (int)in_Count;
        }

        public BinaryBuffer(Buffer in_Buffer)
        {
            buffer = in_Buffer;
        }

        /// <summary>
        /// The current position that subsequent reads or writes will occur at.
        /// </summary>
        public int Position
        {
            get { return position; }
            set { Seek(value); }
        }

        /// <summary>
        /// Access an arbitary byte in the buffer without using/updating Position.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public byte this[int index]
        {
            get { return buffer[index]; }
            set { buffer[index] = value; }
        }

        /// <summary>
        /// Set the buffer position for subsequent reads and writes.
        /// </summary>
        /// <param name="in_Offset"></param>
        /// <param name="in_Origin"></param>
        /// <returns>The new absolute position.</returns>
        public int Seek(int in_Offset, SeekOrigin in_Origin = SeekOrigin.Begin)
        {
            int newPos = position;
            checked
            {
                if (in_Origin == SeekOrigin.Begin)
                    newPos = in_Offset;
                else if (in_Origin == SeekOrigin.Current)
                    newPos += in_Offset;
                else if (in_Origin == SeekOrigin.End)
                    newPos = buffer.Count + in_Offset;
                else
                    throw new ArgumentException();
            }

            if (newPos < 0)
                throw new OverflowException();

            position = newPos;

            return position;
        }

        /// <summary>
        /// Try to read a byte from the current position.
        /// </summary>
        /// <param name="in_Advance"></param>
        /// <returns>The byte as an int, or -1 if past the end of the buffer.</returns>
        public int Read(bool in_Advance = true)
        {
            int value = -1;
            if (position < buffer.Count)
            {
                value = buffer[position];

                if (in_Advance)
                    position++;
            }

            return value;
        }

        /// <summary>
        /// Read a byte from the current position.
        /// </summary>
        /// <param name="in_Advance"></param>
        /// <returns></returns>
        public byte ReadByte(bool in_Advance = true)
        {
            byte value = buffer[position];

            if (in_Advance)
                position++;

            return value;
        }

        /// <summary>
        /// Read an sbyte from the current position.
        /// </summary>
        /// <param name="in_Advance"></param>
        /// <returns></returns>
        public sbyte ReadSByte(bool in_Advance = true)
        {
            return (sbyte)ReadByte(in_Advance);
        }

        /// <summary>
        /// Read multiple bytes from the current position into a buffer.
        /// </summary>
        /// <param name="out_Buffer"></param>
        /// <param name="in_Index"></param>
        /// <param name="in_Count"></param>
        /// <param name="in_Advance"></param>
        /// <returns>The number of bytes read.</returns>
        public int Read(IList<byte> out_Buffer, int in_Index = 0, int? in_Count = null, bool in_Advance = true)
        {
            if (in_Count is null)
                in_Count = out_Buffer.Count;

            CheckReadSize(out_Buffer.Count, in_Index, (int)in_Count);

            int pos = position;
            for (int i = in_Index; i < in_Index + in_Count; i++)
                out_Buffer[i] = buffer[pos++];

            if (in_Advance)
                position = pos;

            return (int)in_Count;
        }

        /// <summary>
        /// Read multiple sbytes from the current position into a buffer.
        /// </summary>
        /// <param name="out_Buffer"></param>
        /// <param name="in_Index"></param>
        /// <param name="in_Count"></param>
        /// <param name="in_Advance"></param>
        /// <returns>The number of sbytes read.</returns>
        public int Read(IList<sbyte> out_Buffer, int in_Index = 0, int? in_Count = null, bool in_Advance = true)
        {
            return Read((IList<byte>)out_Buffer, in_Index, in_Count, in_Advance);
        }

        /// <summary>
        /// Get an enumerator to read bytes beginning from the current position. The enumerator has its own position, and does not advance the position of the BinaryBuffer object.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<byte> GetByteEnumerator()
        {
            for (int pos = position; pos < buffer.Count; pos++)
                yield return buffer[pos++];
        }

        /// <summary>
        /// Get an enumerator to read sbytes beginning from the current position. The enumerator has its own position, and does not advance the position of the BinaryBuffer object.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<sbyte> GetSByteEnumerator()
        {
            for (int pos = position; pos < buffer.Count; pos++)
                yield return (sbyte)buffer[pos++];
        }

        /// <summary>
        /// Read a single big-endian UInt16 from the current position.
        /// </summary>
        /// <param name="in_Advance"></param>
        /// <returns></returns>
        public UInt16 ReadUInt16BE(bool in_Advance = true)
        {
            return InternalRead<BigEndian16>(in_Advance);
        }

        /// <summary>
        /// Read a single little-endian UInt16 from the current position.
        /// </summary>
        /// <param name="in_Advance"></param>
        /// <returns></returns>
        public UInt16 ReadUInt16LE(bool in_Advance = true)
        {
            return InternalRead<LittleEndian16>(in_Advance);
        }

        /// <summary>
        /// Read a single big-endian Int16 from the current position.
        /// </summary>
        /// <param name="in_Advance"></param>
        /// <returns></returns>
        public Int16 ReadInt16BE(bool in_Advance = true)
        {
            return (short)ReadUInt16BE(in_Advance);
        }

        /// <summary>
        /// Read a single little-endian Int16 from the current position.
        /// </summary>
        /// <param name="in_Advance"></param>
        /// <returns></returns>
        public Int16 ReadInt16LE(bool in_Advance = true)
        {
            return (short)ReadUInt16LE(in_Advance);
        }

        /// <summary>
        /// Read multiple big-endian UInt16s from the currently position into a buffer.
        /// </summary>
        /// <param name="out_Buffer">The buffer to read into.</param>
        /// <param name="in_Index">The initial index to read into.</param>
        /// <param name="in_Count">The number of UInt16s to read, or null to fill the whole buffer starting at in_Index.</param>
        /// <param name="in_Advance">Whether to advance the position after read.</param>
        /// <returns>The number of UInt16s read.</returns>
        public int ReadBE(IList<UInt16> out_Buffer, int in_Index = 0, int? in_Count = null, bool in_Advance = true)
        {
            return InternalRead<BigEndian16>(out_Buffer, in_Index, in_Count, in_Advance);
        }

        /// <summary>
        /// Read multiple little-endian UInt16s from the currently position into a buffer.
        /// </summary>
        /// <param name="out_Buffer">The buffer to read into.</param>
        /// <param name="in_Index">The initial index to read into.</param>
        /// <param name="in_Count">The number of UInt16s to read, or null to fill the whole buffer starting at in_Index.</param>
        /// <param name="in_Advance">Whether to advance the position after read.</param>
        /// <returns>The number of UInt16s read.</returns>
        public int ReadLE(IList<UInt16> out_Buffer, int in_Index = 0, int? in_Count = null, bool in_Advance = true)
        {
            return InternalRead<LittleEndian16>(out_Buffer, in_Index, in_Count, in_Advance);
        }

        /// <summary>
        /// Read multiple big-endian Int16s from the current position into a buffer.
        /// </summary>
        /// <param name="out_Buffer"></param>
        /// <param name="in_Index"></param>
        /// <param name="in_Count"></param>
        /// <param name="in_Advance"></param>
        /// <returns>The number of Int16s read.</returns>
        public int ReadBE(IList<Int16> out_Buffer, int in_Index = 0, int? in_Count = null, bool in_Advance = true)
        {
            return ReadBE((IList<UInt16>)out_Buffer, in_Index, in_Count, in_Advance);
        }

        /// <summary>
        /// Read multiple little-endian Int16s from the current position into a buffer.
        /// </summary>
        /// <param name="out_Buffer"></param>
        /// <param name="in_Index"></param>
        /// <param name="in_Count"></param>
        /// <param name="in_Advance"></param>
        /// <returns>The number of Int16s read.</returns>
        public int ReadLE(IList<Int16> out_Buffer, int in_Index = 0, int? in_Count = null, bool in_Advance = true)
        {
            return ReadLE((IList<UInt16>)out_Buffer, in_Index, in_Count, in_Advance);
        }

        /// <summary>
        /// Get an enumerator to read big-endian UInt16s beginning from the current position. The enumerator has its own position, and does not advance the position of the BinaryBuffer object.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<UInt16> GetUInt16BEEnumerator()
        {
            int pos = position;
            BigEndian16 enc = new();
            while (pos + 1 < buffer.Count)
                yield return enc.Read(buffer, ref pos);
        }

        /// <summary>
        /// Get an enumerator to read little-endian UInt16s beginning from the current position. The enumerator has its own position, and does not advance the position of the BinaryBuffer object.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<UInt16> GetUInt16LEEnumerator()
        {
            int pos = position;
            LittleEndian16 enc = new();
            while (pos + 1 < buffer.Count)
                yield return enc.Read(buffer, ref pos);
        }

        /// <summary>
        /// Get an enumerator to read big-endian Int16s beginning from the current position. The enumerator has its own position, and does not advance the position of the BinaryBuffer object.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Int16> GetInt16BEEnumerator()
        {
            return from x in GetUInt16BEEnumerator() select (Int16)x;
        }

        /// <summary>
        /// Get an enumerator to read little-endian Int16s beginning from the current position. The enumerator has its own position, and does not advance the position of the BinaryBuffer object.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Int16> GetInt16LEEnumerator()
        {
            return from x in GetUInt16LEEnumerator() select (Int16)x;
        }

        /// <summary>
        /// Write a byte to the current position.
        /// </summary>
        /// <param name="in_Value"></param>
        /// <param name="in_Advance"></param>
        public void Write(byte in_Value, bool in_Advance = true)
        {
            buffer[position] = in_Value;

            if (in_Advance)
                position++;
        }

        /// <summary>
        /// Write multiple bytes from an enumerator to the current position. Begins by calling MoveNext, so the initial position should be prior to the first element to be written.
        /// </summary>
        /// <param name="in_Iter"></param>
        /// <param name="in_Count"></param>
        /// <param name="in_Advance"></param>
        /// <returns>The number of bytes written.</returns>
        public int Write(IEnumerator<byte> in_Iter, int? in_Count = null, bool in_Advance = true)
        {
            CheckWriteSize(1, in_Count);

            int pos = position;
            if (in_Count is not null)
            {
                int endPos = position + (int)in_Count;
                while (pos < endPos && in_Iter.MoveNext())
                    buffer[pos++] = in_Iter.Current;
            }
            else
            {
                while (in_Iter.MoveNext())
                    buffer[pos++] = in_Iter.Current;
            }

            int count = pos - position;
            if (in_Advance)
                position = pos;

            return count;
        }

        /// <summary>
        /// Write multiple bytes from an enumerable to the current position.
        /// </summary>
        /// <param name="in_Data"></param>
        /// <param name="in_Count"></param>
        /// <param name="in_Advance"></param>
        /// <returns>The number of bytes written.</returns>
        public int Write(IEnumerable<byte> in_Data, int? in_Count = null, bool in_Advance = true)
        {
            return Write(in_Data.GetEnumerator(), in_Count, in_Advance);
        }

        /// <summary>
        /// Write multiple bytes from a list-like object to the current position.
        /// </summary>
        /// <param name="in_Buffer"></param>
        /// <param name="in_Index"></param>
        /// <param name="in_Count"></param>
        /// <param name="in_Advance"></param>
        /// <returns>The number of bytes written.</returns>
        public int Write(IReadOnlyList<byte> in_Buffer, int in_Index = 0, int? in_Count = null, bool in_Advance = true)
        {
            if (in_Count is null)
                in_Count = in_Buffer.Count - in_Index;

            CheckWriteSize(1, in_Buffer.Count, in_Index, (int)in_Count);

            int pos = position;
            for (int inPos = in_Index; inPos < in_Index + in_Count; inPos++)
                buffer[pos++] = in_Buffer[inPos];

            if (in_Advance)
                position = pos;

            return (int)in_Count;
        }

        /// <summary>
        /// Write an sbyte to the current position.
        /// </summary>
        /// <param name="in_Value"></param>
        /// <param name="in_Advance"></param>
        public void Write(sbyte in_Value, bool in_Advance = true)
        {
            Write((byte)in_Value, in_Advance);
        }

        /// <summary>
        /// Write multiple sbytes from an enumerator to the current position. Begins by calling MoveNext, so the initial position should be prior to the first element to be written.
        /// </summary>
        /// <param name="in_Iter"></param>
        /// <param name="in_Count"></param>
        /// <param name="in_Advance"></param>
        /// <returns>The number of sbytes written.</returns>
        public int Write(IEnumerator<sbyte> in_Iter, int? in_Count = null, bool in_Advance = true)
        {
            return Write((IEnumerator<byte>)in_Iter, in_Count, in_Advance);
        }

        /// <summary>
        /// Write multiple sbytes from an enumerable to the current position.
        /// </summary>
        /// <param name="in_Data"></param>
        /// <param name="in_Count"></param>
        /// <param name="in_Advance"></param>
        /// <returns>The number of sbytes written.</returns>
        public int Write(IEnumerable<sbyte> in_Data, int? in_Count = null, bool in_Advance = true)
        {
            return Write((IEnumerable<byte>)in_Data, in_Count, in_Advance);
        }

        /// <summary>
        /// Write multiple bytes from a list-like object to the current position.
        /// </summary>
        /// <param name="in_Buffer"></param>
        /// <param name="in_Index"></param>
        /// <param name="in_Count"></param>
        /// <param name="in_Advance"></param>
        /// <returns>The number of sbytes written.</returns>
        public int Write(IReadOnlyList<sbyte> in_Buffer, int in_Index = 0, int? in_Count = null, bool in_Advance = true)
        {
            return Write((IReadOnlyList<byte>)in_Buffer, in_Index, in_Count, in_Advance);
        }

        /// <summary>
        /// Write a big-endian UInt16 to the current position.
        /// </summary>
        /// <param name="in_Value"></param>
        /// <param name="in_Advance"></param>
        public void WriteBE(UInt16 in_Value, bool in_Advance = true)
        {
            InternalWrite<BigEndian16>(in_Value, in_Advance);
        }

        /// <summary>
        /// Write a little-endian UInt16 to the current position.
        /// </summary>
        /// <param name="in_Value"></param>
        /// <param name="in_Advance"></param>
        public void WriteLE(UInt16 in_Value, bool in_Advance = true)
        {
            InternalWrite<LittleEndian16>(in_Value, in_Advance);
        }

        /// <summary>
        /// Write multiple big-endian UInt16s from an enumerator to the current position. Begins by calling MoveNext, so the initial position should be prior to the first element to be written.
        /// </summary>
        /// <param name="in_Iter"></param>
        /// <param name="in_Count"></param>
        /// <param name="in_Advance"></param>
        /// <returns>The number of UInt16s written.</returns>
        public int WriteBE(IEnumerator<UInt16> in_Iter, int? in_Count = null, bool in_Advance = true)
        {
            return InternalWrite<BigEndian16>(in_Iter, in_Count, in_Advance);
        }

        /// <summary>
        /// Write multiple little-endian UInt16s from an enumerator to the current position. Begins by calling MoveNext, so the initial position should be prior to the first element to be written.
        /// </summary>
        /// <param name="in_Iter"></param>
        /// <param name="in_Count"></param>
        /// <param name="in_Advance"></param>
        /// <returns>The number of UInt16s written.</returns>
        public int WriteLE(IEnumerator<UInt16> in_Iter, int? in_Count = null, bool in_Advance = true)
        {
            return InternalWrite<LittleEndian16>(in_Iter, in_Count, in_Advance);
        }

        /// <summary>
        /// Write multiple big-endian UInt16s from an enumerable to the current position.
        /// </summary>
        /// <param name="in_Data"></param>
        /// <param name="in_Count"></param>
        /// <param name="in_Advance"></param>
        /// <returns>The number of UInt16s written.</returns>
        public int WriteBE(IEnumerable<UInt16> in_Data, int? in_Count = null, bool in_Advance = true)
        {
            return WriteBE(in_Data.GetEnumerator(), in_Count, in_Advance);
        }

        /// <summary>
        /// Write multiple little-endian UInt16s from an enumerable to the current position.
        /// </summary>
        /// <param name="in_Data"></param>
        /// <param name="in_Count"></param>
        /// <param name="in_Advance"></param>
        /// <returns>The number of UInt16s written.</returns>
        public int WriteLE(IEnumerable<UInt16> in_Data, int? in_Count = null, bool in_Advance = true)
        {
            return WriteLE(in_Data.GetEnumerator(), in_Count, in_Advance);
        }

        /// <summary>
        /// Write multiple big-endian UInt16s from a list-like object to the current position.
        /// </summary>
        /// <param name="in_Buffer"></param>
        /// <param name="in_Index"></param>
        /// <param name="in_Count"></param>
        /// <param name="in_Advance"></param>
        /// <returns>The number of UInt16s written.</returns>
        public int WriteBE(IReadOnlyList<UInt16> in_Buffer, int in_Index = 0, int? in_Count = null, bool in_Advance = true)
        {
            return InternalWrite<BigEndian16>(in_Buffer, in_Index, in_Count, in_Advance);
        }

        /// <summary>
        /// Write multiple little-endian UInt16s from a list-like object to the current position.
        /// </summary>
        /// <param name="in_Buffer"></param>
        /// <param name="in_Index"></param>
        /// <param name="in_Count"></param>
        /// <param name="in_Advance"></param>
        /// <returns>The number of UInt16s written.</returns>
        public int WriteLE(IReadOnlyList<UInt16> in_Buffer, int in_Index = 0, int? in_Count = null, bool in_Advance = true)
        {
            return InternalWrite<LittleEndian16>(in_Buffer, in_Index, in_Count, in_Advance);
        }

        /// <summary>
        /// Write a big-endian Int16 to the current position.
        /// </summary>
        /// <param name="in_Value"></param>
        /// <param name="in_Advance"></param>
        public void WriteBE(Int16 in_Value, bool in_Advance = true)
        {
            WriteBE((UInt16)in_Value, in_Advance);
        }

        /// <summary>
        /// Write a little-endian Int16 to the current position.
        /// </summary>
        /// <param name="in_Value"></param>
        /// <param name="in_Advance"></param>
        public void WriteLE(Int16 in_Value, bool in_Advance = true)
        {
            WriteLE((UInt16)in_Value, in_Advance);
        }

        /// <summary>
        /// Write multiple big-endian Int16s from an enumerator to the current position. Begins by calling MoveNext, so the initial position should be prior to the first element to be written.
        /// </summary>
        /// <param name="in_Iter"></param>
        /// <param name="in_Count"></param>
        /// <param name="in_Advance"></param>
        /// <returns>The number of Int16s written.</returns>
        public int WriteBE(IEnumerator<Int16> in_Iter, int? in_Count = null, bool in_Advance = true)
        {
            return WriteBE((IEnumerator<UInt16>)in_Iter, in_Count, in_Advance);
        }

        /// <summary>
        /// Write multiple little-endian Int16s from an enumerator to the current position. Begins by calling MoveNext, so the initial position should be prior to the first element to be written.
        /// </summary>
        /// <param name="in_Iter"></param>
        /// <param name="in_Count"></param>
        /// <param name="in_Advance"></param>
        /// <returns>The number of Int16s written.</returns>
        public int WriteLE(IEnumerator<Int16> in_Iter, int? in_Count = null, bool in_Advance = true)
        {
            return WriteLE((IEnumerator<UInt16>)in_Iter, in_Count, in_Advance);
        }

        /// <summary>
        /// Write multiple big-endian Int16s from an enumerable to the current position.
        /// </summary>
        /// <param name="in_Data"></param>
        /// <param name="in_Count"></param>
        /// <param name="in_Advance"></param>
        /// <returns>The number of Int16s written.</returns>
        public int WriteBE(IEnumerable<Int16> in_Data, int? in_Count = null, bool in_Advance = true)
        {
            return WriteBE((IEnumerable<UInt16>)in_Data, in_Count, in_Advance);
        }

        /// <summary>
        /// Write multiple little-endian Int16s from an enumerable to the current position.
        /// </summary>
        /// <param name="in_Data"></param>
        /// <param name="in_Count"></param>
        /// <param name="in_Advance"></param>
        /// <returns>The number of Int16s written.</returns>
        public int WriteLE(IEnumerable<Int16> in_Data, int? in_Count = null, bool in_Advance = true)
        {
            return WriteLE((IEnumerable<UInt16>)in_Data, in_Count, in_Advance);
        }

        /// <summary>
        /// Write multiple big-endian Int16s from a list-like object to the current position.
        /// </summary>
        /// <param name="in_Buffer"></param>
        /// <param name="in_Index"></param>
        /// <param name="in_Count"></param>
        /// <param name="in_Advance"></param>
        /// <returns>The number of Int16s written.</returns>
        public int WriteBE(IReadOnlyList<Int16> in_Buffer, int in_Index = 0, int? in_Count = null, bool in_Advance = true)
        {
            return WriteBE((IReadOnlyList<UInt16>)in_Buffer, in_Index, in_Count, in_Advance);
        }

        /// <summary>
        /// Write multiple little-endian Int16s from a list-like object to the current position.
        /// </summary>
        /// <param name="in_Buffer"></param>
        /// <param name="in_Index"></param>
        /// <param name="in_Count"></param>
        /// <param name="in_Advance"></param>
        /// <returns>The number of Int16s written.</returns>
        public int WriteLE(IReadOnlyList<Int16> in_Buffer, int in_Index = 0, int? in_Count = null, bool in_Advance = true)
        {
            return WriteLE((IReadOnlyList<UInt16>)in_Buffer, in_Index, in_Count, in_Advance);
        }
    }
}
