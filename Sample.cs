using System;
using System.Collections;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace BrawlhallaReplayLibrary;

internal class Sample
{
    private static readonly JsonSerializerOptions JSON_OPTIONS = new() { WriteIndented = true };

    static Sample()
    {
        JSON_OPTIONS.Converters.Add(new JsonStringEnumConverter());
        JSON_OPTIONS.Converters.Add(new JsonOwnedTauntsConverter());
    }

    public static void ParseToJson(string replayPath, string outputPath)
    {
        Replay replay;
        using (FileStream file = new(replayPath, FileMode.Open, FileAccess.Read))
        {
            replay = Replay.Load(file);
        }

        using FileStream outFile = new(outputPath, FileMode.Create, FileAccess.Write);
        JsonSerializer.Serialize(outFile, replay, JSON_OPTIONS);
    }

    // represent owned taunts as a bit string
    private sealed class JsonOwnedTauntsConverter : JsonConverter<ReplayOwnedTaunts>
    {
        public override ReplayOwnedTaunts? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            string? bitString = reader.GetString();
            if (bitString is null)
                return null;
            if (bitString.Length != 256)
                throw new ArgumentException($"Size of ReplayOwnedTaunts bit array must be 256");
            BitArray bits = new(256);
            for (int i = 0; i < 256; ++i)
            {
                bits[i] = bitString[i] switch
                {
                    '0' => false,
                    '1' => true,
                    _ => throw new ArgumentException($"Invalid char {bitString[i]} in ReplayOwnedTaunts string")
                };
            }
            return new() { TauntsBitfield = bits };
        }

        public override void Write(Utf8JsonWriter writer, ReplayOwnedTaunts value, JsonSerializerOptions options)
        {
            BitArray bits = value.TauntsBitfield;
            StringBuilder builder = new(bits.Length);
            foreach (bool bit in bits) builder.Append(bit ? '1' : '0');
            string bitString = builder.ToString();
            writer.WriteStringValue(bitString);
        }
    }
}