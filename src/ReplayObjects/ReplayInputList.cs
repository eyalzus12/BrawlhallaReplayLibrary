using System.Collections.Generic;

namespace BrawlhallaReplayLibrary;

public class ReplayInputList
{
    public required Dictionary<byte, List<ReplayInput>> Inputs { get; set; } // key is 5 bits

    internal static ReplayInputList CreateFrom(BitReader bits)
    {
        Dictionary<byte, List<ReplayInput>> inputs = [];

        while (bits.ReadBool())
        {
            byte entId = (byte)bits.ReadBits(5);
            int inputCount = bits.ReadInt();
            inputs.TryAdd(entId, []);
            for (int i = 0; i < inputCount; ++i)
            {
                ReplayInput input = ReplayInput.CreateFrom(bits);
                inputs[entId].Add(input);
            }
        }

        return new()
        {
            Inputs = inputs,
        };
    }

    internal void WriteTo(BitWriter bits)
    {
        foreach ((byte entId, List<ReplayInput> inputs) in Inputs)
        {
            if (entId >= (1 << 6))
                throw new ReplaySerializationException($"the keys of {nameof(ReplayInputList)}.{nameof(Inputs)} cannot be larger than 63");
            bits.WriteBool(true);
            bits.WriteBits(entId, 5);
            bits.WriteInt(inputs.Count);
            foreach (ReplayInput input in inputs)
                input.WriteTo(bits);
        }
        bits.WriteBool(false);
    }
}