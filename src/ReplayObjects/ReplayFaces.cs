using System.Collections.Generic;

namespace BrawlhallaReplayLibrary;

public class ReplayFaces
{
    public required ReplayFace[] Faces { get; set; }

    internal static ReplayFaces CreateFrom(BitReader bits)
    {
        List<ReplayFace> faces = [];
        while (bits.ReadBool())
        {
            ReplayFace face = ReplayFace.CreateFrom(bits);
            faces.Add(face);
        }

        return new()
        {
            Faces = [.. faces]
        };
    }

    internal void WriteTo(BitWriter bits)
    {
        foreach (ReplayFace face in Faces)
        {
            bits.WriteBool(true);
            face.WriteTo(bits);
        }
        bits.WriteBool(false);
    }
}