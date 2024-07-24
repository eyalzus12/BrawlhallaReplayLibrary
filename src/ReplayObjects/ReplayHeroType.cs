namespace BrawlhallaReplayLibrary;

public class ReplayHeroType
{
    public required uint HeroId { get; set; }
    public required uint CostumeId { get; set; }
    public required uint StanceIndex { get; set; }
    public required ushort WeaponSkin1 { get; set; }
    public required ushort WeaponSkin2 { get; set; }

    internal static ReplayHeroType CreateFrom(BitReader bits)
    {
        uint heroId = bits.ReadUInt();
        uint costumeId = bits.ReadUInt();
        uint stanceIndex = bits.ReadUInt();
        ushort weaponSkin2 = bits.ReadUShort(); // no, this order is not a mistake
        ushort weaponSkin1 = bits.ReadUShort();
        return new()
        {
            HeroId = heroId,
            CostumeId = costumeId,
            StanceIndex = stanceIndex,
            WeaponSkin1 = weaponSkin1,
            WeaponSkin2 = weaponSkin2,
        };
    }

    internal void WriteTo(BitWriter bits)
    {
        bits.WriteUInt(HeroId);
        bits.WriteUInt(CostumeId);
        bits.WriteUInt(StanceIndex);
        bits.WriteUShort(WeaponSkin2);
        bits.WriteUShort(WeaponSkin1);
    }

    internal uint CalculateChecksum(uint index)
    {
        uint checksum = 0;
        checksum += HeroId * (17u + index);
        checksum += CostumeId * (7u + index);
        checksum += StanceIndex * (3u + index);
        checksum += (uint)(WeaponSkin2 << 16 | WeaponSkin1) * (2u + index);
        return checksum;
    }
}