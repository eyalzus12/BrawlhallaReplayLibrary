namespace BrawlhallaReplayLibrary;

public readonly record struct ReplayInput(int TimeStamp, ReplayInputFlags InputFlags)
{
    internal static ReplayInput CreateFrom(BitStream bits)
    {
        int timeStamp = bits.ReadInt();
        bool hasInput = bits.ReadBool();
        ReplayInputFlags inputFlags = (ReplayInputFlags)(hasInput ? bits.ReadBits(14) : 0);
        return new(timeStamp, inputFlags);
    }
}