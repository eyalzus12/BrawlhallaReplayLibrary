using System;

namespace BrawlhallaReplayLibrary;

public static class ReplayUtils
{
    private static readonly byte[] REPLAY_BYTE_XOR = [0x6B, 0x10, 0xDE, 0x3C, 0x44, 0x4B, 0xD1, 0x46, 0xA0, 0x10, 0x52, 0xC1, 0xB2, 0x31, 0xD3, 0x6A, 0xFB, 0xAC, 0x11, 0xDE, 0x06, 0x68, 0x08, 0x78, 0x8C, 0xD5, 0xB3, 0xF9, 0x6A, 0x40, 0xD6, 0x13, 0x0C, 0xAE, 0x9D, 0xC5, 0xD4, 0x6B, 0x54, 0x72, 0xFC, 0x57, 0x5D, 0x1A, 0x06, 0x73, 0xC2, 0x51, 0x4B, 0xB0, 0xC9, 0x8C, 0x78, 0x04, 0x11, 0x7A, 0xEF, 0x74, 0x3E, 0x46, 0x39, 0xA0, 0xC7, 0xA6];

    internal static void CipherReplayBytes(byte[] bytes)
    {
        for (int i = 0; i < bytes.Length; ++i)
        {
            bytes[i] ^= REPLAY_BYTE_XOR[i % REPLAY_BYTE_XOR.Length];
        }
    }

    public static DateTime TimeStampToDateTime(uint timeStamp)
    {
        return new DateTime(year: 1970, month: 1, day: 1, hour: 0, minute: 0, second: 0, kind: DateTimeKind.Utc)
            .AddSeconds(timeStamp)
            .ToLocalTime();
    }

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
}