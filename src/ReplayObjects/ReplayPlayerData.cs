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
    public required ReplayOwnedTaunts OwnedTaunts { get; set; }
    public required ushort WinTauntId { get; set; }
    public required ushort LoseTauntId { get; set; }
    public required uint[] Taunts { get; set; }
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
        ReplayOwnedTaunts ownedTaunts = ReplayOwnedTaunts.CreateFrom(bits);
        ushort winTauntId = bits.ReadUShort();
        ushort loseTauntId = bits.ReadUShort();
        List<uint> taunts = [];
        while (bits.ReadBool())
            taunts.Add(bits.ReadUInt());
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
            OwnedTaunts = ownedTaunts,
            WinTauntId = winTauntId,
            LoseTauntId = loseTauntId,
            Taunts = [.. taunts],
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
        OwnedTaunts.WriteTo(bits);
        bits.WriteUShort(WinTauntId);
        bits.WriteUShort(LoseTauntId);
        foreach (uint taunt in Taunts)
        {
            bits.WriteBool(true);
            bits.WriteUInt(taunt);
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

    public uint CalculateChecksum()
    {
        uint checksum = 0;
        checksum += ColorSchemeId * 5u;
        checksum += SpawnBotId * 93u;
        checksum += EmitterId * 97u;
        checksum += PlayerThemeId * 53u;
        checksum += OwnedTaunts.CalculateChecksum();
        checksum += WinTauntId * 37u;
        checksum += LoseTauntId * 41u;
        for (int i = 0; i < Taunts.Length; ++i)
            checksum += (uint)(BitOperations.PopCount(Taunts[i]) * (11u + i));
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