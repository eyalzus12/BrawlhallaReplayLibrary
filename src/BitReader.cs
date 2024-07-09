using System;
using System.Collections;
using System.IO;
using System.Text;

namespace BrawlhallaReplayLibrary;

internal class BitReader(Stream stream, bool leaveOpen = false) : IDisposable
{
    private bool disposedValue;

    private long _byteIndex = -1;
    private byte _currentByte;
    private int _indexInByte = 8;

    public long Length => 8 * stream.Length;
    public long Position => 8 * _byteIndex + _indexInByte;

    public bool ReadBool()
    {
        if (_indexInByte == 8)
        {
            _byteIndex++;
            int newByte = stream.ReadByte();
            if (newByte == -1)
                throw new EndOfStreamException();
            _currentByte = (byte)(newByte ^ ReplayUtils.GetReplayByteXor(_byteIndex));
            _indexInByte = 0;
        }

        bool result = (_currentByte & (1 << (7 - _indexInByte))) != 0;
        _indexInByte++;
        return result;
    }

    public ulong ReadBits(int count)
    {
        ulong result = 0;
        while (count != 0)
        {
            result |= (ReadBool() ? 1u : 0u) << (count - 1);
            count--;
        }
        return result;
    }

    public BitArray ReadManyBits(int count)
    {
        BitArray result = new(count);
        for (int i = 0; i < count; ++i)
        {
            result[i] = ReadBool();
        }
        return result;
    }

    public byte ReadByte() => (byte)ReadBits(8);

    public byte[] ReadBytes(int amount)
    {
        byte[] buffer = new byte[amount];
        for (int i = 0; i < amount; ++i)
        {
            buffer[i] = ReadByte();
        }
        return buffer;
    }

    public ushort ReadUShort() => (ushort)ReadBits(16);
    public short ReadShort() => (short)ReadUShort();
    public uint ReadUInt() => (uint)ReadBits(32);
    public int ReadInt() => (int)ReadUInt();
    public char ReadChar() => (char)ReadBits(8);

    public string ReadString()
    {
        ushort size = ReadUShort();
        byte[] bytes = ReadBytes(size);
        string content = Encoding.UTF8.GetString(bytes);
        return content;
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
            {
                if (!leaveOpen) stream.Dispose();
            }

            disposedValue = true;
        }
    }

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}