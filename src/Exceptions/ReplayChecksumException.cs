using System;
using System.Runtime.Serialization;

namespace BrawlhallaReplayLibrary;

[Serializable]
public class ReplayChecksumException : Exception
{
    public ReplayChecksumException() { }
    public ReplayChecksumException(string message) : base(message) { }
    public ReplayChecksumException(string message, Exception inner) : base(message, inner) { }
    protected ReplayChecksumException(SerializationInfo info, StreamingContext context) : base(info, context) { }
}