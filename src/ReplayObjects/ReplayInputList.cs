using System.Collections.Generic;

namespace BrawlhallaReplayLibrary;

public class ReplayInputList
{
    public required Dictionary<byte, ReplayInput[]> Inputs { get; set; } // key is 5 bits

    internal static ReplayInputList CreateFrom(BitReader bits)
    {
        Dictionary<byte, ReplayInput[]> inputs = [];

        while (bits.ReadBool())
        {
            byte entId = (byte)bits.ReadBits(5);
            int inputCount = bits.ReadInt();
            List<ReplayInput> entityInputs = [.. inputs.GetValueOrDefault(entId, [])];
            for (int i = 0; i < inputCount; ++i)
            {
                ReplayInput input = ReplayInput.CreateFrom(bits);
                entityInputs.Add(input);
            }
            inputs.TryAdd(entId, [.. entityInputs]);
        }

        return new()
        {
            Inputs = inputs,
        };
    }

    internal void WriteTo(BitWriter bits)
    {
        foreach ((byte entId, ReplayInput[] inputs) in Inputs)
        {
            if (entId >= 32)
                throw new ReplaySerializationException($"the keys of {nameof(ReplayInputList)}.{nameof(Inputs)} cannot exceed 31");
            bits.WriteBool(true);
            bits.WriteBits(entId, 5);
            bits.WriteInt(inputs.Length);
            foreach (ReplayInput input in inputs)
                input.WriteTo(bits);
        }
        bits.WriteBool(false);
    }
}