using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace BrawlhallaReplayLibrary;

public record ReplayGameData
    (
        ReplayGameSettings Settings,
        uint LevelId,
        ushort HeroCount,
        ReadOnlyCollection<ReplayEntityData> Entities,
        uint Version,
        uint Checksum
    )
{
    internal static ReplayGameData CreateFrom(BitStream bits)
    {
        ReplayGameSettings settings = ReplayGameSettings.CreateFrom(bits);
        uint levelId = bits.ReadUInt();
        ushort heroCount = bits.ReadUShort();
        if (heroCount <= 0 || 5 < heroCount)
            throw new InvalidReplayDataException($"HeroCount is {heroCount}, but must be between 1 and 5");
        List<ReplayEntityData> entities = new();
        while (bits.ReadBool())
            entities.Add(ReplayEntityData.CreateFrom(bits, heroCount));
        uint version = bits.ReadUInt();
        uint checksum = bits.ReadUInt();

        return new(settings, levelId, heroCount, entities.AsReadOnly(), version, checksum);
    }

    public uint CalculateChecksum()
    {
        uint checksum = 0;
        foreach(ReplayEntityData entity in Entities)
        {
            checksum += entity.CalculateChecksum();
        }
        checksum += LevelId * 47u;
        return checksum % 173;
    }

    public void ValidateChecksum()
    {
        uint calculatedChecksum = CalculateChecksum();
        if(calculatedChecksum != Checksum)
        {
            throw new ReplayChecksumException($"Expected {Checksum} but checksum was {calculatedChecksum}");
        }
    }
}