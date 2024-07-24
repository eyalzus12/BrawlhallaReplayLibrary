using System.Collections.Generic;
using System.Linq;

namespace BrawlhallaReplayLibrary;

public static class ReplayUtils
{
    private static readonly byte[] REPLAY_BYTE_XOR = [0x6B, 0x10, 0xDE, 0x3C, 0x44, 0x4B, 0xD1, 0x46, 0xA0, 0x10, 0x52, 0xC1, 0xB2, 0x31, 0xD3, 0x6A, 0xFB, 0xAC, 0x11, 0xDE, 0x06, 0x68, 0x08, 0x78, 0x8C, 0xD5, 0xB3, 0xF9, 0x6A, 0x40, 0xD6, 0x13, 0x0C, 0xAE, 0x9D, 0xC5, 0xD4, 0x6B, 0x54, 0x72, 0xFC, 0x57, 0x5D, 0x1A, 0x06, 0x73, 0xC2, 0x51, 0x4B, 0xB0, 0xC9, 0x8C, 0x78, 0x04, 0x11, 0x7A, 0xEF, 0x74, 0x3E, 0x46, 0x39, 0xA0, 0xC7, 0xA6];
    internal static byte GetReplayByteXor(long byteIndex) => REPLAY_BYTE_XOR[byteIndex % REPLAY_BYTE_XOR.Length];

    private const ReplayInputFlags ALL_TAUNTS = ReplayInputFlags.TauntUp | ReplayInputFlags.TauntRight | ReplayInputFlags.TauntDown | ReplayInputFlags.TauntLeft;

    public static int? GetTauntNumber(this ReplayInputFlags inputFlags)
    {
        return (inputFlags & ALL_TAUNTS) switch
        {
            ReplayInputFlags.TauntUp => 1,
            ReplayInputFlags.TauntUp | ReplayInputFlags.TauntRight => 2,
            ReplayInputFlags.TauntRight => 3,
            ReplayInputFlags.TauntRight | ReplayInputFlags.TauntDown => 4,
            ReplayInputFlags.TauntDown => 5,
            ReplayInputFlags.TauntDown | ReplayInputFlags.TauntLeft => 6,
            ReplayInputFlags.TauntLeft => 7,
            ReplayInputFlags.TauntLeft | ReplayInputFlags.TauntUp => 8,
            0 => 0,
            _ => null
        };
    }

    internal static uint[] OwnedTauntsFrom(BitReader bits)
    {
        List<uint> ownedTaunts = [];
        uint taunt = 0;
        while (bits.ReadBool())
        {
            uint bitfield = bits.ReadUInt();
            // each 32 bitfield is used from lsb to msb
            for (int j = 0; j < 32; ++j)
            {
                if ((bitfield & (1u << j)) != 0)
                    ownedTaunts.Add(taunt);
                taunt++;
            }
        }
        return [.. ownedTaunts];
    }

    internal static uint[] OwnedTauntsToBitfields(uint[] ownedTaunts)
    {
        if (ownedTaunts.Length == 0)
            return [];
        uint[] bitfields = new uint[ownedTaunts.Max() / 32 + 1];
        foreach (uint taunt in ownedTaunts)
            bitfields[taunt / 32] |= 1u << (int)(taunt % 32); // each 32 bitfield is used from lsb to msb
        return bitfields;
    }

    internal static bool AllTheSame<T>(this IEnumerable<T> e)
    {
        HashSet<T> set = [];
        foreach (T t in e)
        {
            if (set.Count > 0 && !set.Contains(t))
                return false;
            set.Add(t);
        }
        return true;
    }
}