namespace BrawlhallaReplayLibrary;

public record ReplayEntityData
{
    public required int EntId { get; set; }
    public required string Name { get; set; }
    public required ReplayPlayerData PlayerData { get; set; }

    internal static ReplayEntityData CreateFrom(BitStream bits, int heroCount)
    {
        int entId = bits.ReadInt(); // why is this 32 bits while every other ent id is 5?
        string name = bits.ReadString();
        ReplayPlayerData playerData = ReplayPlayerData.CreateFrom(bits, heroCount);
        return new()
        {
            EntId = entId,
            Name = name,
            PlayerData = playerData,
        };
    }

    public uint CalculateChecksum() => PlayerData.CalculateChecksum();
}