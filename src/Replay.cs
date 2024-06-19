using System.Collections.Generic;
using System.IO;
using System.IO.Compression;

namespace BrawlhallaReplayLibrary;

public class Replay
{
    public required uint Version { get; set; }
    public required ReplayHeader Header { get; set; }
    public required ReplayGameData GameData { get; set; }
    public required List<ReplayResult> Results { get; set; }
    public required ReplayFaces KnockoutFaces { get; set; }
    public ReplayFaces? OtherFaces { get; set; }
    public required ReplayInputList Inputs { get; set; }

    public static Replay Load(Stream stream, bool ignoreChecksum = false)
    {
        using ZLibStream zlibStream = new(stream, CompressionMode.Decompress);
        using BitReader bits = new(zlibStream);
        return CreateFrom(bits, ignoreChecksum);
    }

    internal static Replay CreateFrom(BitReader bits, bool ignoreChecksum = false)
    {
        uint version = bits.ReadUInt();

        ReplayHeader? header = null;
        ReplayGameData? gameData = null;
        List<ReplayResult> results = [];
        ReplayFaces? knockoutFaces = null;
        ReplayFaces? otherFaces = null;
        ReplayInputList? inputs = null;

        bool reachedReplayEnd = false;
        while (!reachedReplayEnd)
        {
            ReplayObjectTypeEnum replayObjectType = (ReplayObjectTypeEnum)bits.ReadBits(4);
            switch (replayObjectType)
            {
                case ReplayObjectTypeEnum.Header:
                    if (header is not null)
                        throw new InvalidReplayDataException("Duplicate replay header");
                    header = ReplayHeader.CreateFrom(bits);
                    break;
                case ReplayObjectTypeEnum.GameData:
                    if (gameData is not null)
                        throw new InvalidReplayDataException("Duplicate game data");
                    gameData = ReplayGameData.CreateFrom(bits);
                    if (!ignoreChecksum)
                        gameData.ValidateChecksum();
                    break;
                case ReplayObjectTypeEnum.Results:
                    ReplayResult newResult = ReplayResult.CreateFrom(bits);
                    results.Add(newResult);
                    break;
                case ReplayObjectTypeEnum.KnockoutFaces:
                    if (knockoutFaces is not null)
                        throw new InvalidReplayDataException("Duplicate knockout faces");
                    knockoutFaces = ReplayFaces.CreateFrom(bits);
                    break;
                case ReplayObjectTypeEnum.Faces:
                    if (otherFaces is not null)
                        throw new InvalidReplayDataException("Duplicate faces");
                    otherFaces = ReplayFaces.CreateFrom(bits);
                    break;
                case ReplayObjectTypeEnum.Inputs:
                    if (inputs is not null)
                        throw new InvalidReplayDataException("Duplicate inputs");
                    inputs = ReplayInputList.CreateFrom(bits);
                    break;
                case ReplayObjectTypeEnum.End:
                    reachedReplayEnd = true;
                    break;
                case ReplayObjectTypeEnum.InvalidReplay:
                    throw new InvalidReplayDataException("Object type 8 found. Replay is invalid.");
                default:
                    throw new InvalidReplayDataException($"Unknown replay object type {replayObjectType}");
            }
        }

        if (header is null)
            throw new InvalidReplayDataException("No replay header found in replay");
        if (gameData is null)
            throw new InvalidReplayDataException("No game data found in replay");
        if (knockoutFaces is null)
            throw new InvalidReplayDataException("No knockout faces found in replay");
        if (inputs is null)
            throw new InvalidReplayDataException("No input data found in replay");

        return new()
        {
            Version = version,
            Header = header,
            GameData = gameData,
            Results = results,
            KnockoutFaces = knockoutFaces,
            OtherFaces = otherFaces,
            Inputs = inputs
        };
    }
}