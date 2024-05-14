using System.Collections.Generic;

namespace BrawlhallaReplayLibrary;

public class ReplayInputList
{
    public required Dictionary<byte, List<ReplayInput>> Inputs { get; set; } // key is 5 bits

    internal static ReplayInputList CreateFrom(BitStream bits)
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
}