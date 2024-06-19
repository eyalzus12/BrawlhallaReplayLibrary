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
    public required List<uint> Taunts { get; set; }
    public required uint AvatarId { get; set; }
    public required int Team { get; set; }
    public required int ConnectionTime { get; set; }
    public required List<ReplayHeroType> HeroTypes { get; set; }
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
        uint avatarId = bits.ReadUShort();
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
            Taunts = taunts,
            AvatarId = avatarId,
            Team = team,
            ConnectionTime = connectionTime,
            HeroTypes = heroTypes,
            IsBot = isBot,
            HandicapsEnabled = handicapsEnabled,
            HandicapStockCount = handicapStockCount,
            HandicapDamageDoneMult = handicapDamageDoneMult,
            HandicapDamageTakenMult = handicapDamageTakenMult,
        };
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
        for (int i = 0; i < Taunts.Count; ++i)
            checksum += (uint)(BitOperations.PopCount(Taunts[i]) * (11u + i));
        checksum += (uint)Team * 43u;
        for (int i = 0; i < HeroTypes.Count; ++i)
            checksum += HeroTypes[i].CalculateChecksum((uint)i);

        if (!HandicapsEnabled)
            checksum += 29;
        else
        {
            checksum += (uint)HandicapStockCount! * 31u;
            checksum += (uint)Math.Round((uint)HandicapDamageDoneMult! / 10.0) * 3u;
            checksum += (uint)Math.Round((uint)HandicapDamageTakenMult! / 10.0) * 23u;
        }

        return checksum;
    }
}