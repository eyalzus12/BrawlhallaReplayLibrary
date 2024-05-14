namespace BrawlhallaReplayLibrary;

public class ReplayGameSettings
{
    public required ReplayGameModeFlags Flags { get; set; }
    public required uint MaxPlayers { get; set; }
    public required uint Duration { get; set; }
    public required uint RoundDuration { get; set; }
    public required uint StartingLives { get; set; }
    public required uint ScoringTypeId { get; set; }
    public required uint ScoreToWin { get; set; }
    public required uint GameSpeed { get; set; }
    public required uint DamageMultiplier { get; set; }
    public required uint LevelSetId { get; set; }
    public required uint ItemSpawnRuleSetId { get; set; }
    public required uint WeaponSpawnRateId { get; set; }
    public required uint GadgetSpawnRateId { get; set; }
    public required ReplayGadgetSelectFlags CustomGadgetSelection { get; set; }
    public required uint Variation { get; set; }

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

        return new()
        {
            Flags = flags,
            MaxPlayers = maxPlayers,
            Duration = duration,
            RoundDuration = roundDuration,
            StartingLives = startingLives,
            ScoringTypeId = scoringTypeId,
            ScoreToWin = scoreToWin,
            GameSpeed = gameSpeed,
            DamageMultiplier = damageMultiplier,
            LevelSetId = levelSetId,
            ItemSpawnRuleSetId = itemSpawnRuleSetId,
            WeaponSpawnRateId = weaponSpawnRateId,
            GadgetSpawnRateId = gadgetSpawnRateId,
            CustomGadgetSelection = customGadgetSelection,
            Variation = variation,
        };
    }
}