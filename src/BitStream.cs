using System;
using System.IO;
using System.Text;

namespace BrawlhallaReplayLibrary;

internal class BitStream : IDisposable
{
    private readonly Stream _stream;
    private byte _currentByte = 0;
    private int _currentByteIndex = 0;

    public long Length => 8 * _stream.Length;
    public long Position => 8 * _stream.Position + _currentByteIndex;

    public BitStream(Stream stream)
    {
        _stream = stream; _stream.Position = 0;
    }

    public uint ReadBits(int count)
    {
        uint result = 0;
        while (count != 0)
        {
            if (_currentByteIndex == 8)
            {
                _currentByteIndex = 0;
                int nextByte = _stream.ReadByte();
                if (nextByte == -1) throw new EndOfStreamException();
                _currentByte = (byte)nextByte;
            }
            bool bit = (_currentByte & (1 << _currentByteIndex)) != 0;
            result |= (bit ? 1u : 0u) << (count - 1);
            count--;
            _currentByteIndex++;
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

    private bool disposedValue;

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