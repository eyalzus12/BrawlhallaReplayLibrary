using System;

namespace BrawlhallaReplayLibrary;

[Flags]
public enum ReplayGadgetSelectFlags : uint
{
    None = 0,
    BouncyBombs = 0b0000001,
    PressureMines = 0b0000010,
    Spikeballs = 0b0000100,
    SidekickSummoners = 0b0001000,
    HomingBoomerangs = 0b0010000,
    StickyBombs = 0b0100000,
    WeaponCrates = 0b1000000,
}