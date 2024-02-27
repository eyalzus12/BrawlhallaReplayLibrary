using System;
using System.Collections;
using System.IO;
using System.Text;

namespace BrawlhallaReplayLibrary;

internal class BitStream
{
    private readonly BitArray _bits;
    private int _currentIndex = 0;

    public long Length => _bits.Length;
    public long Position => _currentIndex;

    public BitStream(byte[] bytes)
    {
        _currentIndex = 0;
        _bits = new(bytes.Length * 8);

        for (int i = 0; i < bytes.Length; ++i)
        {
            for (int k = 0; k < 8; ++k)
            {
                _bits[8 * i + k] = (bytes[i] & (1u << (7 - k))) != 0;
            }
        }
    }

    public uint ReadBits(int count)
    {
        uint result = 0;
        while (count != 0)
        {
            if (_currentIndex >= _bits.Length)
                throw new EndOfStreamException();
            result |= (_bits[_currentIndex] ? 1u : 0u) << (count - 1);
            count--;
            _currentIndex++;
        }
        return result;
    }

    public bool ReadBool() => ReadBits(1) != 0;

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

    public uint ReadUInt() => ReadBits(32);

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
        uint bits = ReadBits(32);
        byte[] floatBytes = BitConverter.GetBytes(bits);
        float result = BitConverter.ToSingle(floatBytes, 0);
        return result;
    }
}