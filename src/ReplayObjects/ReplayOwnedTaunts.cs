using System.Collections;

namespace BrawlhallaReplayLibrary;

public record ReplayOwnedTaunts(BitArray Bits)
{
    public bool this[int index] => Bits[index];

    internal static ReplayOwnedTaunts CreateFrom(BitStream bits)
    {
        return new(bits.ReadManyBits(256));
    }

    public uint[] ToPacked()
    {
        uint[] packed = new uint[8];
        for (int i = 0; i < 256; ++i)
        {
            int whichPacked = i / 32;
            int bitIndex = i % 32;
            packed[whichPacked] |= (Bits[i] ? 1u : 0u) << (31 - bitIndex);
        }
        return packed;
    }

    public uint CalculateChecksum()
    {
        uint checksum = 0;
        uint[] packed = ToPacked();
        for (int i = 0; i < 8; ++i) checksum += (uint)(packed[i] * (13u + i));
        return checksum;
    }
}