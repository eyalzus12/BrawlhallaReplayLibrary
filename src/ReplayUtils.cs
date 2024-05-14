using System;

namespace BrawlhallaReplayLibrary;

public static class ReplayUtils
{
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