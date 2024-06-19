namespace BrawlhallaReplayLibrary;

public class ReplayHeader
{
    public required uint RandomSeed { get; set; }
    public required uint PlaylistId { get; set; }
    public required string? PlaylistName { get; set; }
    public required bool OnlineGame { get; set; }

    internal static ReplayHeader CreateFrom(BitReader bits)
    {
        uint randomSeed = bits.ReadUInt();
        uint playlistId = bits.ReadUInt();
        string? playlistName = (playlistId != 0) ? bits.ReadString() : null;
        bool onlineGame = bits.ReadBool();
        return new()
        {
            RandomSeed = randomSeed,
            PlaylistId = playlistId,
            PlaylistName = playlistName,
            OnlineGame = onlineGame,
        };
    }
}