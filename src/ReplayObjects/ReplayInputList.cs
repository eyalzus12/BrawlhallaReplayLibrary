using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace BrawlhallaReplayLibrary;

public record ReplayInputList(ReadOnlyDictionary<byte, ReadOnlyCollection<ReplayInput>> Inputs)
{
    internal static ReplayInputList CreateFrom(BitStream bits)
    {
        Dictionary<byte, List<ReplayInput>> inputs = new();

        while (bits.ReadBool())
        {
            byte entId = (byte)bits.ReadBits(5);
            int inputCount = bits.ReadInt();
            inputs.TryAdd(entId, new());
            for (int i = 0; i < inputCount; ++i)
            {
                ReplayInput input = ReplayInput.CreateFrom(bits);
                inputs[entId].Add(input);
            }
        }

        return new(inputs.ToDictionary(entry => entry.Key, entry => entry.Value.AsReadOnly()).AsReadOnly());
    }
}