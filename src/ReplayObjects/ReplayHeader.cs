using System;

namespace BrawlhallaReplayLibrary;

public record ReplayHeader
    (
        uint RandomSeed,
        uint PlaylistId,
        string? PlaylistName,
        bool OnlineGame
    )
{
    internal static ReplayHeader CreateFrom(BitStream bits)
    {
        uint randomSeed = bits.ReadUInt();
        uint playlistId = bits.ReadUInt();
        string? playlistName = (playlistId != 0) ? bits.ReadString() : null;
        bool onlineGame = bits.ReadBool();
        return new(randomSeed, playlistId, playlistName, onlineGame);
    }
}