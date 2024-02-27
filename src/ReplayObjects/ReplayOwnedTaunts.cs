using System.Collections;

namespace BrawlhallaReplayLibrary;

public class ReplayOwnedTaunts
{
    private readonly BitArray _bits = new(256);
    public bool this[int index] => _bits[index];

    public ReplayOwnedTaunts(uint[] packed)
    {
        for (int i = 0; i < 8; ++i)
        {
            for (int j = 0; j < 32; ++j)
            {
                _bits[32 * i + j] = (packed[i] & (1 << (31 - j))) != 0;
            }
        }
    }

    public uint[] ToPacked()
    {
        uint[] packed = new uint[8];
        for (int i = 0; i < 256; ++i)
        {
            int whichPacked = i / 32;
            int bitIndex = i % 32;
            packed[whichPacked] |= (_bits[i] ? 1u : 0u) << (31 - bitIndex);
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