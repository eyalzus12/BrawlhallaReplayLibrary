using System.Collections.Generic;
using System.IO;
using System.IO.Compression;

namespace BrawlhallaReplayLibrary;

public class Replay
{
    public required uint Version { get; set; }
    public required ReplayHeader Header { get; set; }
    public required ReplayGameData GameData { get; set; }
    public required ReplayResult[] Results { get; set; }
    public required ReplayFaces KnockoutFaces { get; set; }
    public ReplayFaces? OtherFaces { get; set; }
    public required ReplayInputList Inputs { get; set; }

    public static Replay Load(Stream stream, bool ignoreChecksum = false, bool leaveStreamOpen = false)
    {
        using ZLibStream zLibStream = new(stream, CompressionMode.Decompress);
        using BitReader bits = new(zLibStream, leaveOpen: leaveStreamOpen);
        return CreateFrom(bits, ignoreChecksum);
    }

    public void Save(Stream stream, bool calculateChecksum = true, bool leaveStreamOpen = false)
    {
        using ZLibStream zLibStream = new(stream, CompressionLevel.SmallestSize);
        using BitWriter bits = new(zLibStream, leaveOpen: leaveStreamOpen);
        WriteTo(bits, calculateChecksum);
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
            Results = [.. results],
            KnockoutFaces = knockoutFaces,
            OtherFaces = otherFaces,
            Inputs = inputs
        };
    }

    internal void WriteTo(BitWriter bits, bool calculateChecksum = true)
    {
        bits.WriteUInt(Version);
        bits.WriteBits((byte)ReplayObjectTypeEnum.Header, 4);
        Header.WriteTo(bits);
        bits.WriteBits((byte)ReplayObjectTypeEnum.GameData, 4);
        GameData.WriteTo(bits, calculateChecksum);
        foreach (ReplayResult result in Results)
        {
            bits.WriteBits((byte)ReplayObjectTypeEnum.Results, 4);
            result.WriteTo(bits);
        }
        bits.WriteBits((byte)ReplayObjectTypeEnum.KnockoutFaces, 4);
        KnockoutFaces.WriteTo(bits);
        if (OtherFaces is not null)
        {
            bits.WriteBits((byte)ReplayObjectTypeEnum.Faces, 4);
            OtherFaces.WriteTo(bits);
        }
        bits.WriteBits((byte)ReplayObjectTypeEnum.Inputs, 4);
        Inputs.WriteTo(bits);
        bits.WriteBits((byte)ReplayObjectTypeEnum.End, 4);
        bits.WriteBits((byte)ReplayObjectTypeEnum.End, 4);
        bits.Flush();
        bits.WriteUInt(Version);
        bits.WriteUInt(0x7E91A5D0);
    }
}