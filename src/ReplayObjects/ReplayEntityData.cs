namespace BrawlhallaReplayLibrary;

public record ReplayEntityData(int EntId, string Name, ReplayPlayerData PlayerData)
{
    internal static ReplayEntityData CreateFrom(BitStream bits, int heroCount)
    {
        int entId = bits.ReadInt();
        string name = bits.ReadString();
        ReplayPlayerData playerData = ReplayPlayerData.CreateFrom(bits, heroCount);
        return new(entId, name, playerData);
    }

    public uint CalculateChecksum() => PlayerData.CalculateChecksum();
}