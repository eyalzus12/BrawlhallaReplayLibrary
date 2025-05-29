using System;
using System.IO;
using System.Text;

namespace BrawlhallaReplayLibrary;

internal class BitWriter(Stream stream, bool leaveOpen = false) : IDisposable
{
    private bool disposedValue;

    private long _byteIndex = 0;
    private byte _currentByte = 0;
    private int _indexInByte = 0;

    public long Length => 8 * stream.Length;
    public long Position => 8 * _byteIndex + _indexInByte;

    private void PushByte()
    {
        stream.WriteByte((byte)(_currentByte ^ ReplayUtils.GetReplayByteXor(_byteIndex)));
        _byteIndex++;
        _currentByte = 0;
        _indexInByte = 0;
    }

    public void WriteBool(bool bit)
    {
        _currentByte |= (byte)((bit ? 1 : 0) << (7 - _indexInByte));
        _indexInByte++;
        if (_indexInByte == 8)
            PushByte();
    }

    public void WriteBits(ulong value, int amount)
    {
        for (int i = amount - 1; i >= 0; --i)
            WriteBool((value & (1u << i)) != 0);
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

    public void WriteString(string str)
    {
        byte[] bytes = Encoding.UTF8.GetBytes(str);
        WriteUShort((ushort)bytes.Length);
        WriteBytes(bytes);
    }

    public void ByteAlign()
    {
        if (_indexInByte != 0)
            PushByte();
    }

    public void Flush()
    {
        ByteAlign();
        stream.Flush();
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            Flush();

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