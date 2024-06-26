using System.Collections;

namespace BrawlhallaReplayLibrary;

public class ReplayOwnedTaunts
{
    public required BitArray TauntsBitfield { get; set; } // 256 bits
    public bool this[int index] => TauntsBitfield[index];

    internal static ReplayOwnedTaunts CreateFrom(BitReader bits)
    {
        return new()
        {
            TauntsBitfield = new(bits.ReadManyBits(256))
        };
    }

    internal void WriteTo(BitWriter bits)
    {
        bits.WriteManyBits(TauntsBitfield);
    }

    public uint[] ToPacked()
    {
        uint[] packed = new uint[8];
        for (int i = 0; i < 256; ++i)
        {
            int whichPacked = i / 32;
            int bitIndex = i % 32;
            packed[whichPacked] |= (TauntsBitfield[i] ? 1u : 0u) << (31 - bitIndex);
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