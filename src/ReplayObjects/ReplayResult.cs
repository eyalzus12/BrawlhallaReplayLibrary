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

    internal static ReplayResult Merge(ReplayResult result1, ReplayResult result2)
    {
        if (result1.Length != result2.Length)
            throw new InvalidReplayDataException($"First result has length {result1.Length}, but second has length {result2.Length}");
        if (result1.EndOfMatchFanfareId != result2.EndOfMatchFanfareId)
            throw new InvalidReplayDataException($"First result has EndOfMatchFanfareId {result1.EndOfMatchFanfareId}, but second has EndOfMatchFanfareId {result2.EndOfMatchFanfareId}");
        if (result1.Version != result2.Version)
            throw new ReplayVersionException($"First result has version {result1.Version}, but second has version {result2.Version}");
        Dictionary<byte, short> scores = new();
        foreach ((byte entId, short score) in result1.Scores)
        {
            scores.TryAdd(entId, 0);
            scores[entId] += score;
        }
        foreach ((byte entId, short score) in result2.Scores)
        {
            scores.TryAdd(entId, 0);
            scores[entId] += score;
        }
        return new(result1.Length, result1.Version, scores.AsReadOnly(), result1.EndOfMatchFanfareId);
    }
}