using System.Collections.Generic;

namespace BrawlhallaReplayLibrary;

public class ReplayFaces
{
    public required List<ReplayFace> Faces { get; set; }

    internal static ReplayFaces CreateFrom(BitReader bits)
    {
        List<ReplayFace> faces = [];
        while (bits.ReadBool())
        {
            ReplayFace face = ReplayFace.CreateFrom(bits);
            faces.Add(face);
        }
        faces.Sort((a, b) => a.TimeStamp.CompareTo(b.TimeStamp));

        return new()
        {
            Faces = faces
        };
    }
}