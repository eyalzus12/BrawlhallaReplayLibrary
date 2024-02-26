using System;
using System.Runtime.Serialization;

namespace BrawlhallaReplayLibrary;

[Serializable]
public class InvalidReplayDataException : Exception
{
    public InvalidReplayDataException() { }
    public InvalidReplayDataException(string message) : base(message) { }
    public InvalidReplayDataException(string message, Exception inner) : base(message, inner) { }
    protected InvalidReplayDataException(SerializationInfo info, StreamingContext context) : base(info, context) { }
}