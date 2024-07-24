using System;
using System.Collections.Generic;
using System.Numerics;

namespace BrawlhallaReplayLibrary;

public class ReplayPlayerData
{
    public required uint ColorSchemeId { get; set; }
    public required uint SpawnBotId { get; set; }
    public required uint EmitterId { get; set; }
    public required uint PlayerThemeId { get; set; }
    public required uint[] Taunts { get; set; }
    public required ushort WinTauntId { get; set; }
    public required ushort LoseTauntId { get; set; }
    public required uint[] OwnedTaunts { get; set; }
    public required ushort AvatarId { get; set; }
    public required int Team { get; set; }
    public required int ConnectionTime { get; set; }
    public required ReplayHeroType[] HeroTypes { get; set; }
    public required bool IsBot { get; set; }
    public required bool HandicapsEnabled { get; set; }
    public required uint? HandicapStockCount { get; set; }
    public required uint? HandicapDamageDoneMult { get; set; }
    public required uint? HandicapDamageTakenMult { get; set; }

    internal static ReplayPlayerData CreateFrom(BitReader bits, int heroCount)
    {
        uint colorSchemeId = bits.ReadUInt();
        uint spawnBotId = bits.ReadUInt();
        uint emitterId = bits.ReadUInt();
        uint playerThemeId = bits.ReadUInt();
        uint[] taunts = new uint[8];
        for (int i = 0; i < 8; ++i)
            taunts[i] = bits.ReadUInt();
        ushort winTauntId = bits.ReadUShort();
        ushort loseTauntId = bits.ReadUShort();
        uint[] ownedTaunts = ReplayUtils.OwnedTauntsFrom(bits);
        ushort avatarId = bits.ReadUShort();
        int team = bits.ReadInt();
        int connectionTime = bits.ReadInt();
        List<ReplayHeroType> heroTypes = new(heroCount);
        for (uint i = 0; i < heroCount; i++)
            heroTypes.Add(ReplayHeroType.CreateFrom(bits));
        bool isBot = bits.ReadBool();
        bool handicapsEnabled = bits.ReadBool();
        uint? handicapStockCount = handicapsEnabled ? bits.ReadUInt() : null;
        uint? handicapDamageDoneMult = handicapsEnabled ? bits.ReadUInt() : null;
        uint? handicapDamageTakenMult = handicapsEnabled ? bits.ReadUInt() : null;

        return new()
        {
            ColorSchemeId = colorSchemeId,
            SpawnBotId = spawnBotId,
            EmitterId = emitterId,
            PlayerThemeId = playerThemeId,
            Taunts = taunts,
            WinTauntId = winTauntId,
            LoseTauntId = loseTauntId,
            OwnedTaunts = ownedTaunts,
            AvatarId = avatarId,
            Team = team,
            ConnectionTime = connectionTime,
            HeroTypes = [.. heroTypes],
            IsBot = isBot,
            HandicapsEnabled = handicapsEnabled,
            HandicapStockCount = handicapStockCount,
            HandicapDamageDoneMult = handicapDamageDoneMult,
            HandicapDamageTakenMult = handicapDamageTakenMult,
        };
    }

    internal void WriteTo(BitWriter bits)
    {
        bits.WriteUInt(ColorSchemeId);
        bits.WriteUInt(SpawnBotId);
        bits.WriteUInt(EmitterId);
        bits.WriteUInt(PlayerThemeId);
        for (int i = 0; i < 8; ++i)
            bits.WriteUInt(Taunts[i]);
        bits.WriteUShort(WinTauntId);
        bits.WriteUShort(LoseTauntId);
        uint[] ownedTauntBitfields = ReplayUtils.OwnedTauntsToBitfields(OwnedTaunts);
        foreach (uint bitfield in ownedTauntBitfields)
        {
            bits.WriteBool(true);
            bits.WriteUInt(bitfield);
        }
        bits.WriteBool(false);
        bits.WriteUShort(AvatarId);
        bits.WriteInt(Team);
        bits.WriteInt(ConnectionTime);
        foreach (ReplayHeroType heroType in HeroTypes)
            heroType.WriteTo(bits);
        bits.WriteBool(IsBot);
        bits.WriteBool(HandicapsEnabled);
        if (HandicapsEnabled)
        {
            bits.WriteUInt(HandicapStockCount ?? throw new ReplaySerializationException($"if {nameof(ReplayPlayerData)}.{nameof(HandicapsEnabled)} is true, {nameof(ReplayPlayerData)}.{nameof(HandicapStockCount)} must be non-null"));
            bits.WriteUInt(HandicapDamageDoneMult ?? throw new ReplaySerializationException($"if {nameof(ReplayPlayerData)}.{nameof(HandicapsEnabled)} is true, {nameof(ReplayPlayerData)}.{nameof(HandicapDamageDoneMult)} must be non-null"));
            bits.WriteUInt(HandicapDamageTakenMult ?? throw new ReplaySerializationException($"if {nameof(ReplayPlayerData)}.{nameof(HandicapsEnabled)} is true, {nameof(ReplayPlayerData)}.{nameof(HandicapDamageTakenMult)} must be non-null"));
        }
    }

    internal uint CalculateChecksum()
    {
        uint checksum = 0;
        checksum += ColorSchemeId * 5u;
        checksum += SpawnBotId * 93u;
        checksum += EmitterId * 97u;
        checksum += PlayerThemeId * 53u;
        for (int i = 0; i < 8; ++i)
            checksum += Taunts[i] * (uint)(13 + i);
        checksum += WinTauntId * 37u;
        checksum += LoseTauntId * 41u;
        uint[] ownedTauntBitfields = ReplayUtils.OwnedTauntsToBitfields(OwnedTaunts);
        for (int i = 0; i < ownedTauntBitfields.Length; ++i)
            checksum += (uint)(BitOperations.PopCount(ownedTauntBitfields[i]) * (11u + i));
        checksum += (uint)Team * 43u;
        for (int i = 0; i < HeroTypes.Length; ++i)
            checksum += HeroTypes[i].CalculateChecksum((uint)i);

        if (!HandicapsEnabled)
            checksum += 29;
        else
        {
            uint handicapStockCount = HandicapStockCount ?? throw new ReplaySerializationException($"if {nameof(ReplayPlayerData)}.{nameof(HandicapsEnabled)} is true, {nameof(ReplayPlayerData)}.{nameof(HandicapStockCount)} must be non-null");
            checksum += handicapStockCount * 31u;
            uint handicapDamageDoneMult = HandicapDamageDoneMult ?? throw new ReplaySerializationException($"if {nameof(ReplayPlayerData)}.{nameof(HandicapsEnabled)} is true, {nameof(ReplayPlayerData)}.{nameof(HandicapDamageDoneMult)} must be non-null");
            checksum += (uint)Math.Round(handicapDamageDoneMult / 10.0) * 3u;
            uint handicapDamageTakenMult = HandicapDamageTakenMult ?? throw new ReplaySerializationException($"if {nameof(ReplayPlayerData)}.{nameof(HandicapsEnabled)} is true, {nameof(ReplayPlayerData)}.{nameof(HandicapDamageTakenMult)} must be non-null");
            checksum += (uint)Math.Round(handicapDamageTakenMult / 10.0) * 23u;
        }

        return checksum;
    }
}