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
        uint ItemSpawnRuleSetId,
        uint WeaponSpawnRateId,
        uint GadgetSpawnRateId,
        ReplayGadgetSelectFlags CustomGadgetSelection,
        uint Variation
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
        uint itemSpawnRuleSetId = bits.ReadUInt();
        uint weaponSpawnRateId = bits.ReadUInt();
        uint gadgetSpawnRateId = bits.ReadUInt();
        ReplayGadgetSelectFlags customGadgetSelection = (ReplayGadgetSelectFlags)bits.ReadUInt();
        uint variation = bits.ReadUInt();

        return new(
            Flags: flags,
            MaxPlayers: maxPlayers,
            Duration: duration,
            RoundDuration: roundDuration,
            StartingLives: startingLives,
            ScoringTypeId: scoringTypeId,
            ScoreToWin: scoreToWin,
            GameSpeed: gameSpeed,
            DamageMultiplier: damageMultiplier,
            LevelSetId: levelSetId,
            ItemSpawnRuleSetId: itemSpawnRuleSetId,
            WeaponSpawnRateId: weaponSpawnRateId,
            GadgetSpawnRateId: gadgetSpawnRateId,
            CustomGadgetSelection: customGadgetSelection,
            Variation: variation
        );
    }
}