namespace BrawlhallaReplayLibrary;

public class ReplayFace
{
    public required int TimeStamp { get; set; }
    public required byte EntId { get; set; } // 5 bits

    internal static ReplayFace CreateFrom(BitReader bits)
    {
        byte entId = (byte)bits.ReadBits(5);
        int timeStamp = bits.ReadInt();
        return new()
        {
            TimeStamp = timeStamp,
            EntId = entId,
        };
    }

    internal void WriteTo(BitWriter bits)
    {
        if (EntId >= 32)
            throw new ReplaySerializationException($"the value of {nameof(ReplayFace)}.{nameof(EntId)} cannot exceed 31");
        bits.WriteBits(EntId, 5);
        bits.WriteInt(TimeStamp);
    }
}