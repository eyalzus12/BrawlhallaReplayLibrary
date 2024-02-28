using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Numerics;

namespace BrawlhallaReplayLibrary;

public record ReplayPlayerData
    (
        uint ColorSchemeId,
        uint SpawnBotId,
        uint EmitterId,
        uint PlayerThemeId,
        ReplayOwnedTaunts OwnedTaunts,
        ushort WinTauntId,
        ushort LoseTauntId,
        ReadOnlyCollection<uint> Taunts,
        uint AvatarId,
        int Team,
        int ConnectionTime,
        ReadOnlyCollection<ReplayHeroType> Heroes,
        bool IsBot,
        bool HandicapsEnabled,
        uint? HandicapStockCount,
        uint? HandicapDamageDoneMult,
        uint? HandicapDamageTakenMult
    )
{
    internal static ReplayPlayerData CreateFrom(BitStream bits, int heroCount)
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

        return new
        (
            colorSchemeId,
            spawnBotId,
            emitterId,
            playerThemeId,
            ownedTaunts,
            winTauntId,
            loseTauntId,
            taunts.AsReadOnly(),
            avatarId,
            team,
            connectionTime,
            heroTypes.AsReadOnly(),
            isBot,
            handicapsEnabled,
            handicapStockCount,
            handicapDamageDoneMult,
            handicapDamageTakenMult
        );
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
        for (int i = 0; i < Heroes.Count; ++i)
            checksum += Heroes[i].CalculateChecksum((uint)i);

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