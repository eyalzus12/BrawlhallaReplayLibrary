using System;
using System.Runtime.Serialization;

namespace BrawlhallaReplayLibrary;

[Serializable]
public class ReplayVersionException : Exception
{
    public ReplayVersionException() { }
    public ReplayVersionException(string message) : base(message) { }
    public ReplayVersionException(string message, Exception inner) : base(message, inner) { }
    protected ReplayVersionException(SerializationInfo info, StreamingContext context) : base(info, context) { }
}