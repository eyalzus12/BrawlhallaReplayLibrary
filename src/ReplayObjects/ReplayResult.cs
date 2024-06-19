using System.Collections.Generic;

namespace BrawlhallaReplayLibrary;

public class ReplayResult
{
    public required uint Length { get; set; }
    public required Dictionary<byte, short> Scores { get; set; } // key is 5 bits
    public required uint EndOfMatchFanfareId { get; set; }

    internal static ReplayResult CreateFrom(BitReader bits)
    {
        uint length = bits.ReadUInt();
        Dictionary<byte, short> scores = [];
        if (bits.ReadBool())
        {
            while (bits.ReadBool())
            {
                byte entId = (byte)bits.ReadBits(5);
                short score = bits.ReadShort();
                if (!scores.TryAdd(entId, score))
                {
                    throw new InvalidReplayDataException($"Score for entity {entId} appears twice");
                }
            }
        }
        uint endOfMatchFanfareId = bits.ReadUInt();

        return new()
        {
            Length = length,
            Scores = scores,
            EndOfMatchFanfareId = endOfMatchFanfareId,
        };
    }
}