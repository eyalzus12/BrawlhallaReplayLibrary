using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace BrawlhallaReplayLibrary;

public record ReplayResult(uint Length, uint Version, ReadOnlyDictionary<byte, short> Scores, uint EndOfMatchFanfareId)
{
    internal static ReplayResult CreateFrom(BitStream bits)
    {
        uint length = bits.ReadUInt();
        uint version = bits.ReadUInt();
        Dictionary<byte, short> scores = new();
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

        return new(length, version, scores.AsReadOnly(), endOfMatchFanfareId);
    }
}