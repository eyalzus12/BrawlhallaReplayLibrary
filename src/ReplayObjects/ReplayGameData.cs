using System.Collections.Generic;
using System.Linq;

namespace BrawlhallaReplayLibrary;

public class ReplayGameData
{
    public required ReplayGameSettings Settings { get; set; }
    public required uint LevelId { get; set; }
    public required List<ReplayEntityData> Entities { get; set; }
    public required uint Checksum { get; set; }

    internal static ReplayGameData CreateFrom(BitReader bits)
    {
        ReplayGameSettings settings = ReplayGameSettings.CreateFrom(bits);
        uint levelId = bits.ReadUInt();
        ushort heroCount = bits.ReadUShort();
        if (heroCount < 1 || 5 < heroCount)
            throw new InvalidReplayDataException($"HeroCount is {heroCount}, but must be between 1 and 5");
        List<ReplayEntityData> entities = [];
        while (bits.ReadBool())
            entities.Add(ReplayEntityData.CreateFrom(bits, heroCount));
        uint checksum = bits.ReadUInt();

        return new()
        {
            Settings = settings,
            LevelId = levelId,
            Entities = entities,
            Checksum = checksum,
        };
    }

    internal void WriteTo(BitWriter bits, bool calculateChecksum = true)
    {
        Settings.WriteTo(bits);
        bits.WriteUInt(LevelId);
        if (Entities.Select(e => e.PlayerData.HeroTypes.Count).Distinct().Count() != 1)
            throw new ReplaySerializationException("All entites must have the same number of heros");
        ushort heroCount = (ushort)Entities[0].PlayerData.HeroTypes.Count;
        bits.WriteUShort(heroCount);
        foreach (ReplayEntityData entity in Entities)
        {
            bits.WriteBool(true);
            entity.WriteTo(bits);
        }
        bits.WriteBool(false);

        if (calculateChecksum)
            bits.WriteUInt(CalculateChecksum());
        else
            bits.WriteUInt(Checksum);
    }

    public uint CalculateChecksum()
    {
        uint checksum = 0;
        foreach (ReplayEntityData entity in Entities)
        {
            checksum += entity.CalculateChecksum();
        }
        checksum += LevelId * 47u;
        return checksum % 173;
    }

    public void ValidateChecksum()
    {
        uint calculatedChecksum = CalculateChecksum();
        if (calculatedChecksum != Checksum)
        {
            throw new ReplayChecksumException($"Expected {Checksum} but checksum was {calculatedChecksum}");
        }
    }
}