namespace BrawlhallaReplayLibrary;

public readonly record struct ReplayFace(int TimeStamp, byte EntId)
{
    internal static ReplayFace CreateFrom(BitStream bits)
    {
        byte entId = (byte)bits.ReadBits(5);
        int timeStamp = bits.ReadInt();
        return new(timeStamp, entId);
    }
}