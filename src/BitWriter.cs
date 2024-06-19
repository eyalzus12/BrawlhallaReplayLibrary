using System;
using System.Buffers.Binary;
using System.Collections;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;

namespace BrawlhallaReplayLibrary;

internal class BitWriter(Stream stream, bool leaveOpen = false) : IDisposable
{
    private static readonly byte[] REPLAY_BYTE_XOR = [0x6B, 0x10, 0xDE, 0x3C, 0x44, 0x4B, 0xD1, 0x46, 0xA0, 0x10, 0x52, 0xC1, 0xB2, 0x31, 0xD3, 0x6A, 0xFB, 0xAC, 0x11, 0xDE, 0x06, 0x68, 0x08, 0x78, 0x8C, 0xD5, 0xB3, 0xF9, 0x6A, 0x40, 0xD6, 0x13, 0x0C, 0xAE, 0x9D, 0xC5, 0xD4, 0x6B, 0x54, 0x72, 0xFC, 0x57, 0x5D, 0x1A, 0x06, 0x73, 0xC2, 0x51, 0x4B, 0xB0, 0xC9, 0x8C, 0x78, 0x04, 0x11, 0x7A, 0xEF, 0x74, 0x3E, 0x46, 0x39, 0xA0, 0xC7, 0xA6];

    private readonly Stream _stream = stream;
    private bool disposedValue;

    private long _byteIndex = -1;
    private byte _currentByte;
    private int _indexInByte = 8;

    public long Length => 8 * _stream.Length;
    public long Position => 8 * _byteIndex + _indexInByte;

    public void WriteBool(bool bit)
    {
        if (_indexInByte == 8)
        {
            _byteIndex++;
            _stream.WriteByte(_currentByte);
            _currentByte = 0;
            _indexInByte = 0;
        }

        _currentByte |= (byte)((bit ? 1 : 0) << (7 - _indexInByte));
        _indexInByte++;
    }

    public void WriteBits(ulong value, int amount)
    {
        for (int i = amount - 1; i >= 0; --i)
            WriteBool((value & (1u << i)) != 0);
    }

    public void WriteManyBits(BitArray bits)
    {
        foreach (bool bit in bits)
            WriteBool(bit);
    }

    public void WriteByte(byte value) => WriteBits(value, 8);

    public void WriteBytes(byte[] bytes)
    {
        foreach (byte value in bytes)
            WriteByte(value);
    }

    public void WriteUShort(ushort value) => WriteBits(value, 16);
    public void WriteShort(short value) => WriteUShort((ushort)value);
    public void WriteUInt(uint value) => WriteBits(value, 32);
    public void WriteInt(int value) => WriteUInt((uint)value);
    public void WriteChar(char value) => WriteBits(value, 8);

    public void WriteString(string str)
    {
        WriteUShort((ushort)str.Length);
        byte[] bytes = Encoding.UTF8.GetBytes(str);
        WriteBytes(bytes);
    }

    public void ReadFloat(float value)
    {
        Span<byte> buffer = stackalloc byte[4];
        BinaryPrimitives.WriteSingleBigEndian(buffer, value);
        uint bits = BinaryPrimitives.ReadUInt32BigEndian(buffer);
        WriteBits(bits, 32);
    }

    public void Flush()
    {
        if (_indexInByte != 8)
            _stream.WriteByte(_currentByte);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            Flush();

            if (disposing)
            {
                if (!leaveOpen) _stream.Dispose();
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