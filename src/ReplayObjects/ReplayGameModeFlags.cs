using System;

namespace BrawlhallaReplayLibrary;

//size: 2 bytes
[Flags]
public enum ReplayGameModeFlags : ushort
{
    Teams = 0b0000000001,
    TeamDamage = 0b0000000010,
    FixedCamera = 0b00000000100,
    GadgetsOff = 0b00000001000,
    WeaponsOff = 0b00000010000,
    TestLevelsOn = 0b00000100000,
    TestFeaturesOn = 0b00001000000,
    GhostRule = 0b00010000000,
    TurnOffMapArtThemes = 0b00100000000,
    ForceCrewBattleCycle = 0b01000000000,
    AdvancedSettings = 0b10000000000,
}