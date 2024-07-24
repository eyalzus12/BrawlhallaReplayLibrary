using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace BrawlhallaReplayLibrary;

internal class Sample
{
    private static readonly JsonSerializerOptions JSON_OPTIONS = new() { WriteIndented = true };

    static Sample()
    {
        JSON_OPTIONS.Converters.Add(new JsonStringEnumConverter());
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
}