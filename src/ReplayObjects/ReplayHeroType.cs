namespace BrawlhallaReplayLibrary;

public class ReplayHeroType
{
    public required uint HeroId { get; set; }
    public required uint CostumeId { get; set; }
    public required uint StanceIndex { get; set; }
    public required ushort WeaponSkin1 { get; set; }
    public required ushort WeaponSkin2 { get; set; }
    public required bool MorphWeapon2 { get; set; } // true if second weapon is used for morph, false if first

    internal static ReplayHeroType CreateFrom(BitReader bits)
    {
        uint heroId = bits.ReadUInt();
        uint costumeId = bits.ReadUInt();
        uint stanceIndex = bits.ReadUInt();
        _ = bits.ReadBool(); // ignored by the game
        ushort weaponSkin2 = (ushort)bits.ReadBits(15);
        bool morphWeapon2 = bits.ReadBool();
        ushort weaponSkin1 = (ushort)bits.ReadBits(15);
        return new()
        {
            HeroId = heroId,
            CostumeId = costumeId,
            StanceIndex = stanceIndex,
            WeaponSkin1 = weaponSkin1,
            WeaponSkin2 = weaponSkin2,
            MorphWeapon2 = morphWeapon2,
        };
    }

    internal void WriteTo(BitWriter bits)
    {
        bits.WriteUInt(HeroId);
        bits.WriteUInt(CostumeId);
        bits.WriteUInt(StanceIndex);
        bits.WriteBool(false); // ignored field
        if (WeaponSkin2 >= 32768)
            throw new ReplaySerializationException($"{nameof(ReplayHeroType)}.{nameof(WeaponSkin2)} cannot exceed 32767");
        bits.WriteBits(WeaponSkin2, 15);
        bits.WriteBool(MorphWeapon2);
        if (WeaponSkin1 >= 32768)
            throw new ReplaySerializationException($"{nameof(ReplayHeroType)}.{nameof(WeaponSkin1)} cannot exceed 32767");
        bits.WriteBits(WeaponSkin1, 15);
    }

    internal uint CalculateChecksum(uint index)
    {
        uint checksum = 0;
        checksum += HeroId * (17u + index);
        checksum += CostumeId * (7u + index);
        checksum += StanceIndex * (3u + index);
        // setting the upper bit doesn't seem to affect the checksum?
        uint packed = (uint)(WeaponSkin2 << 16) | WeaponSkin1 | (MorphWeapon2 ? 1u << 15 : 0);// | (1u << 31);
        checksum += packed * (2u + index);
        return checksum;
    }
}