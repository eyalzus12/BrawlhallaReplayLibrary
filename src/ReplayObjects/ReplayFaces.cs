using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace BrawlhallaReplayLibrary;

public record ReplayFaces(ReadOnlyCollection<ReplayFace> Faces)
{
    internal static ReplayFaces CreateFrom(BitStream bits)
    {
        List<ReplayFace> faces = [];
        while (bits.ReadBool())
        {
            ReplayFace face = ReplayFace.CreateFrom(bits);
            faces.Add(face);
        }
        faces.Sort((a, b) => a.TimeStamp.CompareTo(b.TimeStamp));

        return new(faces.AsReadOnly());
    }
}