using System;
using System.Collections;
using System.IO;
using System.Text;

namespace BrawlhallaReplayLibrary;

internal class BitReader(Stream stream) : IDisposable
{
    private static readonly byte[] REPLAY_BYTE_XOR = [0x6B, 0x10, 0xDE, 0x3C, 0x44, 0x4B, 0xD1, 0x46, 0xA0, 0x10, 0x52, 0xC1, 0xB2, 0x31, 0xD3, 0x6A, 0xFB, 0xAC, 0x11, 0xDE, 0x06, 0x68, 0x08, 0x78, 0x8C, 0xD5, 0xB3, 0xF9, 0x6A, 0x40, 0xD6, 0x13, 0x0C, 0xAE, 0x9D, 0xC5, 0xD4, 0x6B, 0x54, 0x72, 0xFC, 0x57, 0x5D, 0x1A, 0x06, 0x73, 0xC2, 0x51, 0x4B, 0xB0, 0xC9, 0x8C, 0x78, 0x04, 0x11, 0x7A, 0xEF, 0x74, 0x3E, 0x46, 0x39, 0xA0, 0xC7, 0xA6];

    private bool disposedValue;
    private readonly Stream _stream = stream;

    private long _byteIndex = -1;
    private byte _currentByte;
    private int _indexInByte = 8;

    public long Length => 8 * _stream.Length;
    public long Position => 8 * _byteIndex + _indexInByte;

    public bool ReadBool()
    {
        if (_indexInByte == 8)
        {
            _byteIndex++;
            int newByte = _stream.ReadByte();
            if (newByte == -1)
                throw new EndOfStreamException();
            _currentByte = (byte)(newByte ^ REPLAY_BYTE_XOR[_byteIndex % REPLAY_BYTE_XOR.Length]);
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

    public float ReadFloat()
    {
        uint bits = (uint)ReadBits(32);
        byte[] floatBytes = BitConverter.GetBytes(bits);
        float result = BitConverter.ToSingle(floatBytes, 0);
        return result;
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
            {
                _stream.Dispose();
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