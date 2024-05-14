namespace BrawlhallaReplayLibrary;

public class ReplayInput
{
    public required int TimeStamp { get; set; }
    public required ReplayInputFlags InputFlags { get; set; }

    internal static ReplayInput CreateFrom(BitStream bits)
    {
        int timeStamp = bits.ReadInt();
        bool hasInput = bits.ReadBool();
        ReplayInputFlags inputFlags = (ReplayInputFlags)(hasInput ? bits.ReadBits(14) : 0);
        return new()
        {
            TimeStamp = timeStamp,
            InputFlags = inputFlags,
        };
    }
}