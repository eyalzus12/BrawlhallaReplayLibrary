using System;
using System.IO;
using System.IO.Compression;

namespace BrawlhallaReplayLibrary;

public record Replay
    (
        ReplayHeader Header,
        ReplayGameData GameData,
        ReplayResult Result,
        ReplayFaces KnockoutFaces,
        ReplayFaces? OtherFaces,
        ReplayInputList Inputs
    )
{

    public static Replay Load(Stream stream, bool ignoreChecksum = false, bool ignoreVersionMatch = false)
    {
        //decompress
        using MemoryStream bufferStream = new();
        using (ZLibStream zlibStream = new(stream, CompressionMode.Decompress))
        {
            zlibStream.CopyTo(bufferStream);
        }
        byte[] replayBytes = bufferStream.ToArray();
        //decrypt
        ReplayUtils.CipherReplayBytes(replayBytes);
        //create bit stream
        BitStream bits = new(replayBytes);
        //create replay
        return CreateFrom(bits, ignoreChecksum, ignoreVersionMatch);
    }

    internal static Replay CreateFrom(BitStream bits, bool ignoreChecksum = false, bool ignoreVersionMatch = false)
    {
        ReplayHeader? header = null;
        ReplayGameData? gameData = null;
        ReplayResult? result = null;
        ReplayFaces? knockoutFaces = null;
        ReplayFaces? otherFaces = null;
        ReplayInputList? inputs = null;

        bool reachedReplayEnd = false;
        while (bits.Position < bits.Length && !reachedReplayEnd)
        {
            ReplayObjectTypeEnum replayObjectType = (ReplayObjectTypeEnum)bits.ReadBits(3);
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
                    break;
                case ReplayObjectTypeEnum.Results:
                    ReplayResult newResult = ReplayResult.CreateFrom(bits);
                    if (result is null) result = newResult;
                    else result = ReplayResult.Merge(result, newResult);
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
                default:
                    throw new InvalidReplayDataException($"Unknown replay object type {replayObjectType}");
            }
        }

        if (header is null)
            throw new InvalidReplayDataException("No replay header found in replay");
        if (gameData is null)
            throw new InvalidReplayDataException("No game data found in replay");
        if (result is null)
            throw new InvalidReplayDataException("No game result found in replay");
        if (knockoutFaces is null)
            throw new InvalidReplayDataException("No knockout faces found in replay");
        if (inputs is null)
            throw new InvalidReplayDataException("No input data found in replay");

        if (!ignoreChecksum)
            gameData.ValidateChecksum();

        if (!ignoreVersionMatch)
        {
            if (header.Version != gameData.Version)
                throw new ReplayVersionException($"Replay header has version {header.Version} but game data has version {gameData.Version}");
            if (header.Version != result.Version)
                throw new ReplayVersionException($"Replay header has version {header.Version} but result has version {result.Version}");
            if (gameData.Version != result.Version)
                throw new ReplayVersionException($"Replay game data has version {gameData.Version} but result has version {result.Version}");
        }

        return new(header, gameData, result, knockoutFaces, otherFaces, inputs);
    }
}