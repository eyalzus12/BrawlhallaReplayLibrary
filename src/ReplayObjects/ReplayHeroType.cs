namespace BrawlhallaReplayLibrary;

public record ReplayHeroType
    (
        uint HeroId,
        uint CostumeId,
        uint StanceIndex,
        ushort WeaponSkin1,
        ushort WeaponSkin2
    )
{
    internal static ReplayHeroType CreateFrom(BitStream bits)
    {
        uint heroId = bits.ReadUInt();
        uint costumeId = bits.ReadUInt();
        uint stanceIndex = bits.ReadUInt();
        ushort weaponSkin2 = bits.ReadUShort(); //no, this order is not a mistake
        ushort weaponSkin1 = bits.ReadUShort();
        return new(heroId, costumeId, stanceIndex, weaponSkin1, weaponSkin2);
    }

    public uint CalculateChecksum(uint index)
    {
        uint checksum = 0;
        checksum += HeroId * (17u + index);
        checksum += CostumeId * (7u + index);
        checksum += StanceIndex * (3u + index);
        checksum += (uint)(WeaponSkin2 << 16 | WeaponSkin1) * (2u + index);
        return checksum;
    }
}