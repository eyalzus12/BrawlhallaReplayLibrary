namespace BrawlhallaReplayLibrary;

public class ReplayFace
{
    public required int TimeStamp { get; set; }
    public required byte EntId { get; set; } // 5 bits

    internal static ReplayFace CreateFrom(BitStream bits)
    {
        byte entId = (byte)bits.ReadBits(5);
        int timeStamp = bits.ReadInt();
        return new()
        {
            TimeStamp = timeStamp,
            EntId = entId,
        };
    }
}