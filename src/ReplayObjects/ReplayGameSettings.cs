using System;

namespace BrawlhallaReplayLibrary;

public record ReplayGameSettings
    (
        ReplayGameModeFlags Flags,
        uint MaxPlayers,
        uint Duration,
        uint RoundDuration,
        uint StartingLives,
        uint ScoringTypeId,
        uint ScoreToWin,
        uint GameSpeed,
        uint DamageMultiplier,
        uint LevelSetId,
        ReplayGadgetSelectionEnum GadgetSelection,
        ReplayGadgetSelectFlags CustomGadgets
    )
{
    internal static ReplayGameSettings CreateFrom(BitStream bits)
    {
        ReplayGameModeFlags flags = (ReplayGameModeFlags)bits.ReadUInt();
        uint maxPlayers = bits.ReadUInt();
        uint duration = bits.ReadUInt();
        uint roundDuration = bits.ReadUInt();
        uint startingLives = bits.ReadUInt();
        uint scoringTypeId = bits.ReadUInt();
        uint scoreToWin = bits.ReadUInt();
        uint gameSpeed = bits.ReadUInt();
        uint damageMultiplier = bits.ReadUInt();
        uint levelSetId = bits.ReadUInt();
        ReplayGadgetSelectionEnum gadgetSelection = (ReplayGadgetSelectionEnum)bits.ReadUInt();
        ReplayGadgetSelectFlags customGadgets = (ReplayGadgetSelectFlags)bits.ReadUInt();

        return new(flags, maxPlayers, duration, roundDuration, startingLives, scoringTypeId, scoreToWin, gameSpeed, damageMultiplier, levelSetId, gadgetSelection, customGadgets);
    }
}