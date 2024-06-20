using System;

namespace BrawlhallaReplayLibrary;

[Serializable]
public class ReplaySerializationException : Exception
{
    public ReplaySerializationException() { }
    public ReplaySerializationException(string message) : base(message) { }
    public ReplaySerializationException(string message, Exception inner) : base(message, inner) { }
}