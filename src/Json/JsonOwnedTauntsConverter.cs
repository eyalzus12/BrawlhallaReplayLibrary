using System;
using System.Collections;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace BrawlhallaReplayLibrary;

public class JsonOwnedTauntsConverter : JsonConverter<ReplayOwnedTaunts>
{
    public override ReplayOwnedTaunts? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        string? bitString = reader.GetString();
        if (bitString is null)
            return null;
        BitArray bits = new(bitString.Length);
        for (int i = 0; i < bitString.Length; ++i)
        {
            bits[i] = bitString[i] switch
            {
                '0' => false,
                '1' => true,
                _ => throw new InvalidOperationException($"Invalid char {bitString[i]} in ReplayOwnedTaunts string")
            };
        }
        return new(bits);
    }

    public override void Write(Utf8JsonWriter writer, ReplayOwnedTaunts value, JsonSerializerOptions options)
    {
        BitArray bits = value.Bits;
        StringBuilder builder = new(bits.Length);
        foreach (bool bit in bits) builder.Append(bit ? '1' : '0');
        string bitString = builder.ToString();
        writer.WriteStringValue(bitString);
    }
}