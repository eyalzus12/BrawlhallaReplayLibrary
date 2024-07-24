using System.Collections.Generic;
using System.Linq;

namespace BrawlhallaReplayLibrary;

public class ReplayGameData
{
    public required ReplayGameSettings Settings { get; set; }
    public required uint LevelId { get; set; }
    public required ReplayEntityData[] Entities { get; set; }
    public required uint Checksum { get; set; }

    internal static ReplayGameData CreateFrom(BitReader bits)
    {
        ReplayGameSettings settings = ReplayGameSettings.CreateFrom(bits);
        uint levelId = bits.ReadUInt();
        ushort heroCount = bits.ReadUShort();
        if (heroCount < 1 || 5 < heroCount)
            throw new InvalidReplayDataException($"Hero count is {heroCount}, but must be between 1 and 5");
        List<ReplayEntityData> entities = [];
        while (bits.ReadBool())
            entities.Add(ReplayEntityData.CreateFrom(bits, heroCount));
        if (entities.Count == 0)
            throw new InvalidReplayDataException("No entities were found in the replay");
        uint checksum = bits.ReadUInt();

        return new()
        {
            Settings = settings,
            LevelId = levelId,
            Entities = [.. entities],
            Checksum = checksum,
        };
    }

    internal void WriteTo(BitWriter bits, bool calculateChecksum = true)
    {
        Settings.WriteTo(bits);
        bits.WriteUInt(LevelId);
        if (Entities.Length == 0)
            throw new ReplaySerializationException("There must be atleast one entity in the replay");
        if (!Entities.Select(e => e.PlayerData.HeroTypes.Length).AllTheSame())
            throw new ReplaySerializationException("All entites must have the same number of heros");
        ushort heroCount = (ushort)Entities[0].PlayerData.HeroTypes.Length;
        if (heroCount < 1 || 5 < heroCount)
            throw new ReplaySerializationException("Entities can only have between 1 and 5 heros");
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