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
        JSON_OPTIONS.Converters.Add(new JsonOwnedTauntsConverter());
    }

    public static void Main(string[] args)
    {
        string replayPath = args[0];
        string outputPath = args[1];

        Replay replay;
        using (FileStream file = new(replayPath, FileMode.Open, FileAccess.Read))
        {
            replay = Replay.Load(file);
        }

        using FileStream outFile = new(outputPath, FileMode.Create, FileAccess.Write);
        JsonSerializer.Serialize(outFile, replay, JSON_OPTIONS);
    }
}